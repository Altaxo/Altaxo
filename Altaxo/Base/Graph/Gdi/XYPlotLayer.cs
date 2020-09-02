#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Scales.Ticks;

namespace Altaxo.Graph.Gdi
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Main.Properties;
  using Axis;
  using Data;
  using Geometry;
  using Plot;
  using Shapes;

  /// <summary>
  /// XYPlotLayer represents a rectangular area on the graph, which holds plot curves, axes and graphical elements.
  /// </summary>
  public partial class XYPlotLayer
    :
    HostLayer,
    IPlotArea
  {
    #region Member variables

    protected G2DCoordinateSystem _coordinateSystem;

    private ScaleCollection _scales;

    protected GridPlaneCollection _gridPlanes;

    protected AxisStyleCollection _axisStyles;

    /// <summary>If true, the data are clipped to the frame.</summary>
    protected LayerDataClipping _dataClipping = LayerDataClipping.StrictToCS;

    protected PlotItemCollection _plotItems;

    /// <summary>Number of times this event is disables, or 0 if it is enabled.</summary>
    [NonSerialized]
    private Main.SuspendableObject _plotAssociationXBoundariesChanged_EventSuspender = new Main.SuspendableObject();

    /// <summary>Number of times this event is disables, or 0 if it is enabled.</summary>
    [NonSerialized]
    private Main.SuspendableObject _plotAssociationYBoundariesChanged_EventSuspender = new Main.SuspendableObject();

    /// <summary>
    /// Partial list of all <see cref="PlaceHolder"/> instances in <see cref="HostLayer.GraphObjects"/>.
    /// </summary>
    [NonSerialized]
    private IObservableList<PlaceHolder>? _placeHolders;

    [NonSerialized]
    private IObservableList<PlotItemPlaceHolder>? _plotItemPlaceHolders;

    #endregion Member variables

    #region Constructors

    #region Copying

    /// <summary>
    /// The copy constructor.
    /// </summary>
    /// <param name="from"></param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public XYPlotLayer(XYPlotLayer from)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
      : base(from)
    {
    }

    /// <summary>
    /// Internal copy from operation. It is presumed, that the events are already suspended. Additionally,
    /// it is not neccessary to call the OnChanged event, since this is called in the calling routine.
    /// </summary>
    /// <param name="obj">The object (layer) from which to copy.</param>
    /// <param name="options">Copy options.</param>
    protected override void InternalCopyFrom(HostLayer obj, GraphCopyOptions options)
    {
      base.InternalCopyFrom(obj, options); // base copy, but keep in mind that InternalCopyGraphItems is overridden in this class

      var from = obj as XYPlotLayer;
      if (from is null)
        return;

      if (0 != (options & GraphCopyOptions.CopyLayerScales))
      {
        CoordinateSystem = (G2DCoordinateSystem)from.CoordinateSystem.Clone();

        Scales = from._scales.Clone();
        _dataClipping = from._dataClipping;
      }

      // Coordinate Systems size must be updated in any case
      CoordinateSystem.UpdateAreaSize(_cachedLayerSize);

      if (0 != (options & GraphCopyOptions.CopyLayerGrid))
      {
        GridPlanes = from._gridPlanes.Clone();
      }

      // Styles

      if (0 != (options & GraphCopyOptions.CopyLayerAxes))
      {
        AxisStyles = (AxisStyleCollection)from._axisStyles.Clone();
      }

      // Plot items
      if (0 != (options & GraphCopyOptions.CopyLayerPlotItems))
      {
        PlotItems = new PlotItemCollection(this, from._plotItems);
      }
      else if (0 != (options & GraphCopyOptions.CopyLayerPlotStyles))
      {
        // TODO apply the styles from from._plotItems to the PlotItems here
        PlotItems.CopyFrom(from._plotItems, options);
      }
    }

    protected override void InternalCopyGraphItems(HostLayer from, GraphCopyOptions options)
    {
      bool bGraphItems = options.HasFlag(GraphCopyOptions.CopyLayerGraphItems);
      bool bChildLayers = options.HasFlag(GraphCopyOptions.CopyChildLayers);
      bool bLegends = options.HasFlag(GraphCopyOptions.CopyLayerLegends);

      var criterium = new Func<IGraphicBase, bool>(x =>
      {
        if (x is HostLayer)
          return bChildLayers;

        if (x is LegendText)
          return bLegends;

        return bGraphItems;
      });

      InternalCopyGraphItems(from, options, criterium);
    }

    public override object Clone()
    {
      return new XYPlotLayer(this);
    }

    #endregion Copying

    /// <summary>
    /// Constructor for deserialization purposes only.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected XYPlotLayer(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
      : base(info)
    {
      CoordinateSystem = new CS.G2DCartesicCoordinateSystem();
      AxisStyles = new AxisStyleCollection();
      Scales = new ScaleCollection();
      Location = new ItemLocationDirect();
      GridPlanes = new GridPlaneCollection
      {
        new GridPlane(CSPlaneID.Front)
      };
    }

    public XYPlotLayer(HostLayer parentLayer)
      : this(parentLayer, GetChildLayerDefaultLocation(), new CS.G2DCartesicCoordinateSystem())
    {
    }

    public XYPlotLayer(HostLayer parentLayer, G2DCoordinateSystem coordinateSystem)
      : this(parentLayer, GetChildLayerDefaultLocation(), coordinateSystem)
    {
    }

    /// <summary>
    /// Creates a layer at the designated <paramref name="location"/>
    /// </summary>
    /// <param name="parentLayer">The parent layer of the constructed layer.</param>
    /// <param name="location">The location of the constructed layer.</param>
    public XYPlotLayer(HostLayer parentLayer, IItemLocation location)
      : this(parentLayer, location, new CS.G2DCartesicCoordinateSystem())
    {
    }

    /// <summary>
    /// Creates a layer at the provided <paramref name="location"/>.
    /// </summary>
    /// <param name="parentLayer">The parent layer of the newly created layer.</param>
    /// <param name="location">The position of the layer on the printable area in points (1/72 inch).</param>
    /// <param name="coordinateSystem">The coordinate system to use for the layer.</param>
    public XYPlotLayer(HostLayer parentLayer, IItemLocation location, G2DCoordinateSystem coordinateSystem)
      : base(parentLayer, location)
    {
      CoordinateSystem = coordinateSystem;
      AxisStyles = new AxisStyleCollection();
      Scales = new ScaleCollection();
      GridPlanes = new GridPlaneCollection
      {
        new GridPlane(CSPlaneID.Front)
      };
      PlotItems = new PlotItemCollection(this);
    }

    #endregion Constructors

    private void EnsureAppropriateGridAndAxisStylePlaceHolders()
    {
      using (var suspendToken = _graphObjects.GetEventDisableToken())
      {
        var gridIndex = _graphObjects.IndexOfFirst(x => x is GridPlanesPlaceHolder);
        if (gridIndex < 0)
        {
          _graphObjects.Insert(0, new GridPlanesPlaceHolder());
          gridIndex = 0;
        }
        else
        {
          // make sure that no more GridPlanesPlaceHolders are in the collection
          _graphObjects.RemoveWhere((item, i) => (i > gridIndex) && (item is GridPlanesPlaceHolder));
        }

        // we try to place AxisStyleLinePlaceHolders after the GridPlanesPlaceHolder
        // find the first of any placeholders for axis line styles, store as insert position, if not found use insert position after grid place holder
        // from i= to count look for placeholders for axis line styles i, if not found insert it at insert position, store next position as insert position

        var insertIdx = gridIndex + 1;
        insertIdx = InsertMissingAxisStylePlaceHolders<AxisStyleLinePlaceHolder>(insertIdx);
        insertIdx = InsertMissingAxisStylePlaceHolders<AxisStyleMajorLabelPlaceHolder>(insertIdx);
        insertIdx = InsertMissingAxisStylePlaceHolders<AxisStyleMinorLabelPlaceHolder>(insertIdx);
        insertIdx = InsertMissingAxisStylePlaceHolders<AxisStyleTitlePlaceHolder>(insertIdx);

        // remove superfluous place holders
        int maxCount = _axisStyles.Count;
        _graphObjects.RemoveWhere(x => { var s = x as AxisStylePlaceHolderBase; return s is not null && s.Index > maxCount; });

        suspendToken.ResumeSilently();
      }
    }

    private int InsertMissingAxisStylePlaceHolders<T>(int insertIdx) where T : AxisStylePlaceHolderBase, new()
    {
      for (int i = 0; i < _axisStyles.Count; ++i)
      {
        var idx = _graphObjects.IndexOfFirst(x => x is T xT && xT.Index == i);
        if (idx >= 0)
        {
          insertIdx = idx + 1;
        }
        else
        {
          _graphObjects.Insert(insertIdx, new T { Index = i });
          ++insertIdx;
        }
      }
      return insertIdx;
    }

    #region IPlotLayer methods

    public bool Is3D { get { return false; } }

    public Scale? ZAxis { get { return null; } }

    public Scale GetScale(int i)
    {
      return _scales[i];
    }

    public Logical3D GetLogical3D(I3DPhysicalVariantAccessor acc, int idx)
    {
      Logical3D r;
      r.RX = XAxis.PhysicalVariantToNormal(acc.GetXPhysical(idx));
      r.RY = YAxis.PhysicalVariantToNormal(acc.GetYPhysical(idx));
      r.RZ = Is3D ? ZAxis!.PhysicalVariantToNormal(acc.GetZPhysical(idx)) : 0;
      return r;
    }

    public Logical3D GetLogical3D(AltaxoVariant x, AltaxoVariant y)
    {
      Logical3D r;
      r.RX = XAxis.PhysicalVariantToNormal(x);
      r.RY = YAxis.PhysicalVariantToNormal(y);
      r.RZ = 0;
      return r;
    }

    /// <summary>
    /// Returns a list of the used axis style ids for this layer.
    /// </summary>
    public System.Collections.Generic.IEnumerable<CSLineID> AxisStyleIDs
    {
      get { return AxisStyles.AxisStyleIDs; }
    }

    /// <summary>
    /// Updates the logical value of a plane id in case it uses a physical value.
    /// </summary>
    /// <param name="id">The plane identifier</param>
    public CSPlaneID UpdateCSPlaneID(CSPlaneID id)
    {
      if (id.UsePhysicalValue)
      {
        double l = Scales[id.PerpendicularAxisNumber].PhysicalVariantToNormal(id.PhysicalValue);
        id = id.WithLogicalValue(l);
      }

      return id;
    }

    #endregion IPlotLayer methods

    #region XYPlotLayer properties and methods

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="C:Altaxo.Main.DocNodeProxy"/> instances to the visitor.</param>
    public override void VisitDocumentReferences(Altaxo.Main.DocNodeProxyReporter Report)
    {
      base.VisitDocumentReferences(Report);
      PlotItems.VisitDocumentReferences(Report);
    }

    /// <summary>
    /// Collection of the axis styles for the left, bottom, right, and top axis.
    /// </summary>
    public AxisStyleCollection AxisStyles
    {
      get
      {
        return _axisStyles;
      }
      [MemberNotNull(nameof(_axisStyles))]
      protected set
      {
        if (ChildSetMember(ref _axisStyles, value))
        {
          value.UpdateCoordinateSystem(CoordinateSystem);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public ScaleCollection Scales
    {
      get
      {
        return _scales;
      }
      [MemberNotNull(nameof(_scales))]
      protected set
      {
        if (object.ReferenceEquals(value, _scales))
          return;

        var oldscales = _scales;
        _scales = value;
        _scales.ParentObject = this;

        using (var suspendToken = SuspendGetToken())
        {
          for (int i = 0; i < _scales.Count; i++)
          {
            var oldScale = oldscales is null ? null : oldscales[i];
            Scale newScale = _scales[i];
            if (!object.ReferenceEquals(oldScale, newScale))
              EhSelfChanged(new ScaleInstanceChangedEventArgs(oldScale, newScale) { ScaleIndex = i });
          }

          if (oldscales is not null)
            oldscales.Dispose();

          suspendToken.Resume();
        }
      }
    }

    public TextGraphic? Legend
    {
      get
      {
        return (LegendText?)_graphObjects.FirstOrDefault(item => item is LegendText);
      }
      set
      {
        var idx = _graphObjects.IndexOfFirst(item => item is LegendText);
        TextGraphic? oldvalue = idx >= 0 ? (TextGraphic)_graphObjects[idx] : null;

        if (value is not null)
        {
          if (idx < 0)
            _graphObjects.Add(value);
          else
            _graphObjects[idx] = value;
        }
        else
        {
          if (idx >= 0)
            _graphObjects.RemoveAt(idx);
        }

        if (!object.ReferenceEquals(value, oldvalue))
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public override void Remove(GraphicBase go)
    {
      if (_axisStyles.Remove(go))
        return;
      else
        base.Remove(go);
    }

    protected override void OnGraphObjectsCollectionInstanceInitialized()
    {
      base.OnGraphObjectsCollectionInstanceInitialized();

      if (_placeHolders is not null)
      {
      }
      _placeHolders = _graphObjects.CreatePartialViewOfType<PlaceHolder>();
      _plotItemPlaceHolders = _graphObjects.CreatePartialViewOfType<PlotItemPlaceHolder>();
      if (_placeHolders is not null)
      {
      }
    }

    public PlotItemCollection PlotItems
    {
      get
      {
        return _plotItems;
      }
      [MemberNotNull(nameof(_plotItems))]
      protected set
      {
        if (value is null)
          throw new ArgumentNullException("value");

        if (ChildSetMember(ref _plotItems, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Clears all legends from this layer.
    /// </summary>
    public void ClearLegends()
    {
      for (int i = GraphObjects.Count - 1; i >= 0; --i)
      {
        if (GraphObjects[i] is LegendText)
          GraphObjects.RemoveAt(i);
      }
    }

    /// <summary>
    /// Creates a new legend, removing the old one.
    /// </summary>
    /// <remarks>The position of the old legend is <b>only</b> used for the new legend if the old legend's position is
    /// inside the layer. This prevents a "stealth" legend in case it is not visible by accident.
    /// </remarks>
    public void CreateNewLayerLegend()
    {
      // remove the legend if there are no plot curves on the layer
      if (PlotItems.Flattened.Length == 0)
      {
        ClearLegends();
        EhSelfChanged(EventArgs.Empty);

        return;
      }

      TextGraphic tgo;

      var existingLegendIndex = GraphObjects.IndexOfFirst(x => x is LegendText);
      var existingLegend = existingLegendIndex >= 0 ? (LegendText)GraphObjects[existingLegendIndex] : null;

      if (existingLegend is not null)
        tgo = new TextGraphic(existingLegend);
      else
        tgo = new TextGraphic(this.GetPropertyContext());

      var strg = new System.Text.StringBuilder();
      for (int i = 0; i < PlotItems.Flattened.Length; i++)
      {
        strg.AppendFormat("{0}\\L({1}) \\%({2})", (i == 0 ? "" : "\r\n"), i, i);
      }
      tgo.Text = strg.ToString();

      // if the position of the old legend is outside, use a new position
      if (existingLegend is null || existingLegend.Position.X < 0 || existingLegend.Position.Y < 0 ||
        existingLegend.Position.X > Size.X || existingLegend.Position.Y > Size.Y)
        tgo.Position = new PointD2D(0.1 * Size.X, 0.1 * Size.Y);
      else
        tgo.Position = existingLegend.Position;

      if (existingLegendIndex >= 0)
        GraphObjects[existingLegendIndex] = tgo;
      else
        GraphObjects.Add(tgo);

      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// This will create the default axes styles that are given by the coordinate system.
    /// </summary>
    public void CreateDefaultAxes(IReadOnlyPropertyBag context)
    {
      foreach (CSAxisInformation info in CoordinateSystem.AxisStyles)
      {
        if (info.IsShownByDefault)
        {
          AxisStyles.CreateDefault(info.Identifier, context);

          if (info.HasTitleByDefault)
          {
            SetAxisTitleString(info.Identifier, info.Identifier.ParallelAxisNumber == 0 ? "X axis" : "Y axis");
          }
        }
      }
    }

    #endregion XYPlotLayer properties and methods

    #region Scale related

    public TickSpacing XTicks
    {
      get
      {
        return Scales[0].TickSpacing;
      }
    }

    public TickSpacing YTicks
    {
      get
      {
        return Scales[1].TickSpacing;
      }
    }

    /// <summary>Gets or sets the x axis of this layer.</summary>
    /// <value>The x axis of the layer.</value>
    public Scale XAxis
    {
      get
      {
        return _scales.X;
      }
      set
      {
        _scales.X = value;
      }
    }

    /// <summary>Indicates if x axis is linked to the linked layer x axis.</summary>
    /// <value>True if x axis is linked to the linked layer x axis.</value>
    public bool IsXAxisLinked
    {
      get
      {
        return _scales.X is LinkedScale;
      }
    }

    private bool EhXAxisInterrogateBoundaryChangedEvent()
    {
      // do nothing here, for the future we can decide to change the linked axis boundaries
      return IsXAxisLinked;
    }

    /// <summary>
    /// Called when the user pressed the rescale button.
    /// </summary>
    public void OnUserRescaledAxes()
    {
      EhXBoundaryChangedEventFromPlotItem();
      EhYBoundaryChangedEventFromPlotItem();
      _scales.X.OnUserRescaled();
      _scales.Y.OnUserRescaled();
    }

    /// <summary>Gets or sets the y axis of this layer.</summary>
    /// <value>The y axis of the layer.</value>
    public Scale YAxis
    {
      get
      {
        return _scales.Y;
      }
      set
      {
        _scales.Y = value;
      }
    }

    /// <summary>Indicates if y axis is linked to the linked layer y axis.</summary>
    /// <value>True if y axis is linked to the linked layer y axis.</value>
    public bool IsYAxisLinked
    {
      get
      {
        return _scales.Y is LinkedScale;
      }
    }

    private bool EhYAxisInterrogateBoundaryChangedEvent()
    {
      // do nothing here, for the future we can decide to change the linked axis boundaries
      return IsYAxisLinked;
    }

    #endregion Scale related

    #region Style properties

    public LayerDataClipping ClipDataToFrame
    {
      get
      {
        return _dataClipping;
      }
      set
      {
        LayerDataClipping oldvalue = _dataClipping;
        _dataClipping = value;

        if (value != oldvalue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public GridPlaneCollection GridPlanes
    {
      get
      {
        return _gridPlanes;
      }
      [MemberNotNull(nameof(_gridPlanes))]
      set
      {
        if (value is null)
          throw new ArgumentNullException();

        if (ChildSetMember(ref _gridPlanes, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    private string? GetAxisTitleString(CSLineID id)
    {
      return _axisStyles[id]?.Title?.Text;
    }

    private void SetAxisTitleString(CSLineID id, string value)
    {
      var style = _axisStyles[id];
      var oldtitle = style?.Title?.Text;
      var newtitle = string.IsNullOrEmpty(value) ? null : value;

      if (newtitle != oldtitle)
      {
        if (newtitle is null)
        {
          if (style is not null)
            style.Title = null;
        }
        else if (_axisStyles.AxisStyleEnsured(id).Title is { } title)
        {
          title.Text = newtitle;
        }
        else
        {
          var tg = new TextGraphic(this.GetPropertyContext());

          CSAxisInformation info = CoordinateSystem.GetAxisStyleInformation(id);

          tg.SetParentSize(Size, false);
          SetDefaultAxisTitlePositionAndOrientation(tg, id, info);
          tg.Text = newtitle;
          _axisStyles.AxisStyleEnsured(id).Title = tg;
        }
      }
    }

    private void SetDefaultAxisTitlePositionAndOrientation(TextGraphic axisTitle, CSLineID id, CSAxisInformation info)
    {
      // find out the position and orientation of the item
      double rx0 = 0, rx1 = 1, ry0 = 0, ry1 = 1;
      if (id.ParallelAxisNumber == 0)
        ry0 = ry1 = id.LogicalValueOtherFirst;
      else
        rx0 = rx1 = id.LogicalValueOtherFirst;

      Logical3D tdirection = CoordinateSystem.GetLogicalDirection(info.Identifier.ParallelAxisNumber, info.PreferredLabelSide);
      var location = CoordinateSystem.GetNormalizedDirection(new Logical3D(rx0, ry0), new Logical3D(rx1, ry1), 0.5, tdirection, out var normDirection);
      double angle = Math.Atan2(normDirection.Y, normDirection.X) * 180 / Math.PI;

      axisTitle.Location.ParentAnchorX = RADouble.NewRel(location.X / Size.X); // set the x anchor of the parent
      axisTitle.Location.ParentAnchorY = RADouble.NewRel(location.Y / Size.Y); // set the y anchor of the parent

      double distance = 0;
      var axisStyle = _axisStyles[id];
      if (axisStyle?.AxisLineStyle is not null)
        distance += axisStyle.AxisLineStyle.GetOuterDistance(info.PreferredLabelSide);
      double labelFontSize = 0;
      if (axisStyle?.MajorLabelStyle is not null)
        labelFontSize = Math.Max(labelFontSize, axisStyle.MajorLabelStyle.FontSize);
      if (axisStyle?.MinorLabelStyle is not null)
        labelFontSize = Math.Max(labelFontSize, axisStyle.MinorLabelStyle.FontSize);
      const double scaleFontWidth = 4;
      const double scaleFontHeight = 1.5;

      if (-45 <= angle && angle <= 45)
      {
        //case EdgeType.Right:
        axisTitle.Rotation = 90;
        axisTitle.Location.LocalAnchorX = RADouble.NewRel(0.5); // Center
        axisTitle.Location.LocalAnchorY = RADouble.NewRel(0); // Top
        distance += scaleFontWidth * labelFontSize;
      }
      else if (-135 <= angle && angle <= -45)
      {
        //case Top:
        axisTitle.Rotation = 0;
        axisTitle.Location.LocalAnchorX = RADouble.NewRel(0.5); // Center
        axisTitle.Location.LocalAnchorY = RADouble.NewRel(1); // Bottom
        distance += scaleFontHeight * labelFontSize;
      }
      else if (45 <= angle && angle <= 135)
      {
        //case EdgeType.Bottom:
        axisTitle.Rotation = 0;
        axisTitle.Location.LocalAnchorX = RADouble.NewRel(0.5); // Center
        axisTitle.Location.LocalAnchorY = RADouble.NewRel(0); // Top
        distance += scaleFontHeight * labelFontSize;
      }
      else
      {
        //case EdgeType.Left:

        axisTitle.Rotation = 90;
        axisTitle.Location.LocalAnchorX = RADouble.NewRel(0.5); // Center
        axisTitle.Location.LocalAnchorY = RADouble.NewRel(1); // Bottom
        distance += scaleFontWidth * labelFontSize;
      }

      axisTitle.Location.PositionX = RADouble.NewAbs(distance * normDirection.X); // because this is relative to the reference point, we don't need to take the location into account here, it is set above
      axisTitle.Location.PositionY = RADouble.NewAbs(distance * normDirection.Y);
    }

    [MaybeNull]
    public string DefaultYAxisTitleString
    {
      get
      {
        return GetAxisTitleString(CSLineID.Y0);
      }
      set
      {
        SetAxisTitleString(CSLineID.Y0, value);
      }
    }

    [MaybeNull]
    public string DefaultXAxisTitleString
    {
      get
      {
        return GetAxisTitleString(CSLineID.X0);
      }
      set
      {
        SetAxisTitleString(CSLineID.X0, value);
      }
    }

    #endregion Style properties

    #region Painting and Hit testing

    /// <summary>
    /// This function is called by the graph document before _any_ layer is painted. We have to make sure that all of our cached data becomes valid.
    ///
    /// </summary>

    private void EnsureAppropriatePlotItemPlaceHolders()
    {
      if (_plotItemPlaceHolders is null)
        throw new InvalidProgramException();

      using (var suspendToken = _graphObjects.GetEventDisableToken())
      {
        if (0 == _plotItemPlaceHolders.Count) // take special measures if not one plot item place holder -> this can happen when deserializing old versions prior to the introduction of place holders
        {
          InsertTheVeryFirstPlotItemPlaceHolder();
        }

        int idx = -1;
        int maxIdx = _plotItemPlaceHolders.Count;

        foreach (var ele in Altaxo.Collections.TreeNodeExtensions.TakeFromHereToLeavesWithIndex<IGPlotItem>(
          _plotItems,
          0,
          true, x => x is PlotItemCollection ? ((PlotItemCollection)x).ChildIndexDirection : IndexDirection.Ascending))
        {
          PlotItemPlaceHolder placeHolder;
          ++idx;
          if (idx < maxIdx)
            placeHolder = _plotItemPlaceHolders[idx];
          else
            _plotItemPlaceHolders.Add(placeHolder = new PlotItemPlaceHolder());

          placeHolder.PlotItemParent = ele.Item1.ParentCollection;
          placeHolder.PlotItemIndex = ele.Item2;
        }

        // items from 0 including to idx are in use, thus we can delete items from idx-1 up to the end

        for (int i = _plotItemPlaceHolders.Count - 1; i > idx; --i)
          _plotItemPlaceHolders.RemoveAt(i);

        suspendToken.ResumeSilently();
      }
    }

    /// <summary>
    /// Inserts the very first plot item place holder at the right place. It is not tested if this is the first - Thus make sure that no plot item place holder is there.
    /// </summary>
    private void InsertTheVeryFirstPlotItemPlaceHolder()
    {
      // we should place the first plot item place holder after all of these types:
      // Background, all kind of axis styles

      var predicate = new Func<IGraphicBase, int, bool>(
        (item, i) =>
        {
          return (item is GridPlanesPlaceHolder) || (item is AxisStylePlaceHolderBase);
        }
        );

      int idx = _graphObjects.IndexOfLast(predicate);

      if (idx < 0)
        idx = 0; // insert at first position if nothing was found
      else
        ++idx; // insert after the found item

      _graphObjects.Insert(idx, new PlotItemPlaceHolder());
    }

    /// <summary>
    /// Adjusts the internal data structures to ensure its validity.
    /// </summary>
    public override void FixupInternalDataStructures()
    {
      base.FixupInternalDataStructures();

      // Before we paint the axis, we have to make sure that all plot items
      // had their data updated, so that the axes are updated before they are drawn!
      _plotItems.PrepareScales(this);
      _plotItems.PrepareGroupStyles(null, this);
      _plotItems.ApplyGroupStyles(null);

      _axisStyles.FixupInternalDataStructures(this);

      EnsureAppropriateGridAndAxisStylePlaceHolders();
      EnsureAppropriatePlotItemPlaceHolders();
    }

    public override void PaintPreprocessing(IPaintContext context)
    {
      context.PushHierarchicalValue<IPlotArea>(nameof(IPlotArea), this);
      base.PaintPreprocessing(context);
      context.PopHierarchicalValue<IPlotArea>(nameof(IPlotArea));
    }

    protected override void PaintInternal(Graphics g, IPaintContext paintContext)
    {
      // paint the background very first
      _gridPlanes.PaintBackground(g, this);

      // then paint the graph items
      base.PaintInternal(g, paintContext);
    }

    /// <summary>
    /// This function is called when painting is finished. Can be used to release the resources
    /// not neccessary any more.
    /// </summary>
    public override void PaintPostprocessing()
    {
      _plotItems.PaintPostprocessing();

      base.PaintPostprocessing();
    }

    public override IHitTestObject? HitTest(HitTestPointData parentHitTestData, bool plotItemsOnly)
    {
      IHitTestObject? hit;

      // first test the items in the child layers, since they are plotted on top of our own items
      if ((hit = base.HitTest(parentHitTestData, plotItemsOnly)) is not null)
        return hit;

      /*
                if (plotItemsOnly)
                {
                    HitTestPointData layerHitTestData = parentHitTestData.NewFromAdditionalTransformation(_transformation);
                    var layerCoord = layerHitTestData.GetHittedPointInWorldCoord();
                    if (null != (hit = _plotItems.HitTest(this, layerCoord)))
                    {
                        if (hit.ParentLayer == null)
                            hit.ParentLayer = this;

                        if (hit.DoubleClick == null)
                            hit.DoubleClick = PlotItemEditorMethod;
                        return ForwardTransform(hit);
                    }
                }
             */

      if (plotItemsOnly)
      {
        HitTestPointData localCoord = parentHitTestData.NewFromAdditionalTransformation(_transformation);

        // hit testing all graph objects, this is done in reverse order compared to the painting, so the "upper" items are found first.
        for (int i = _graphObjects.Count - 1; i >= 0; --i)
        {
          var plotItemPlaceHolder = _graphObjects[i] as PlotItemPlaceHolder;
          if (plotItemPlaceHolder is null)
            continue;

          hit = plotItemPlaceHolder.HitTest(localCoord);
          if (hit is not null)
          {
            hit.ParentLayer = this;
            return ForwardTransform(hit);
          }
        }
      }

      return null;
    }

    #endregion Painting and Hit testing

    #region Editor methods

    public static DoubleClickHandler? AxisScaleEditorMethod;
    public static DoubleClickHandler? AxisStyleEditorMethod;
    public static DoubleClickHandler? AxisLabelMajorStyleEditorMethod;
    public static DoubleClickHandler? AxisLabelMinorStyleEditorMethod;
    public static DoubleClickHandler? PlotItemEditorMethod;

    private bool EhAxisLabelMajorStyleRemove(IHitTestObject o)
    {
      var als = o.HittedObject as AxisLabelStyle;
      var axisStyle = als?.ParentObject as AxisStyle;
      if (axisStyle is not null)
      {
        axisStyle.HideMajorLabels();
        return true;
      }
      return false;
    }

    private bool EhAxisLabelMinorStyleRemove(IHitTestObject o)
    {
      var als = o.HittedObject as AxisLabelStyle;
      var axisStyle = als?.ParentObject as AxisStyle;
      if (axisStyle is not null)
      {
        axisStyle.HideMinorLabels();
        return true;
      }
      return false;
    }

    #endregion Editor methods

    #region Event firing

    protected override void OnCachedResultingSizeChanged()
    {
      // first update out direct childs
      CoordinateSystem?.UpdateAreaSize(Size);
      base.OnCachedResultingSizeChanged();
    }

    protected virtual void OnCoordinateSystemChanged()
    {
      // if the coordinate system has changed, try to bring all axis titles back to their default position
      _axisStyles.UpdateCoordinateSystem(_coordinateSystem);

      foreach (var axisStyle in _axisStyles)
      {
        if (axisStyle.Title is not null)
          SetDefaultAxisTitlePositionAndOrientation(axisStyle.Title, axisStyle.StyleID, _coordinateSystem.GetAxisStyleInformation(axisStyle.StyleID));
      }
    }

    #endregion Event firing

    #region Handler of child events

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (object.ReferenceEquals(sender, _coordinateSystem))
        OnCoordinateSystemChanged();

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    protected override void OnChanged(EventArgs e)
    {
      if (e is BoundariesChangedEventArgs)
        EhBoundaryChangedEventFromPlotItem((BoundariesChangedEventArgs)e);
      else if (e is PlotItemDataChangedEventArgs)
        EhBoundaryChangedEventFromPlotItem(new BoundariesChangedEventArgs(BoundariesChangedData.ComplexChange));
      else if (e is ScaleInstanceChangedEventArgs)
        EhScaleInstanceChanged((ScaleInstanceChangedEventArgs)e);

      base.OnChanged(e);
    }

    protected void EhBoundaryChangedEventFromPlotItem(BoundariesChangedEventArgs boundaryChangedEventArgs)
    {
      var data = boundaryChangedEventArgs.Data;
      if (data.HasFlag(BoundariesChangedData.XBoundariesChanged))
        EhXBoundaryChangedEventFromPlotItem();
      if (data.HasFlag(BoundariesChangedData.YBoundariesChanged))
        EhYBoundaryChangedEventFromPlotItem();
    }

    /// <summary>
    /// This handler is called if a x-boundary from any of the plotassociations of this layer
    /// has changed. We then have to recalculate the boundaries.
    /// </summary>
    /// <remarks>Unfortunately we do not know if the boundary is extended or shrinked, if is is extended
    /// if would be possible to merge only the changed boundary into the x-axis boundary.
    /// But since we don't know about that, we have to completely recalculate the boundary be using the boundaries of
    /// all PlotAssociations of this layer.</remarks>
    protected void EhXBoundaryChangedEventFromPlotItem()
    {
      if (!_plotAssociationXBoundariesChanged_EventSuspender.IsSuspended)
      {
        // now we have to inform all the PlotAssociations that a new axis was loaded
        using (var suspendToken = _scales.X.DataBoundsObject.SuspendGetToken())
        {
          _scales.X.DataBoundsObject.Reset();
          foreach (IGPlotItem pa in PlotItems)
          {
            var paXB = pa as IXBoundsHolder;
            if (paXB is not null)
            {
              using (var paToken = pa.SuspendGetToken()) // we have to suspend the plotitem. When the boundary data in the plot item are not uptodate, it would otherwise create new BoundaryChangedEventArgs, which would lead to inefficiency or a stack overflow
              {
                // merge the bounds with x and yAxis
                paXB.MergeXBoundsInto(_scales.X.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
                paToken.ResumeSilently(); // resume the plot item silently here in order not to create further BoundaryChangedEventArgs
              }
            }
          }

          suspendToken.Resume();
        }
      }
    }

    /// <summary>
    /// Initializes the x scale data bounds, for instance if the scale instance has changed or was deserialized.
    /// </summary>
    protected void InitializeXScaleDataBounds()
    {
      if (PlotItems is null)
        return; // can happen during deserialization

      var scaleBounds = _scales.X.DataBoundsObject;
      if (scaleBounds is null)
        return;

      // we have to disable our own Handler since by calling MergeXBoundsInto, it is possible that the type of DataBound of the plot item has to change, and that
      // generates a OnBoundaryChanged event, and by handling this the boundaries of all other plot items are merged into the axis boundary,
      // but (alas!) not all boundaries are now of the new type!
      using (var xBoundariesChangedSuspendToken = _plotAssociationXBoundariesChanged_EventSuspender.SuspendGetToken())
      {
        PlotItems.PrepareScales(this); // Prepare the plot items to use the appropriate data bounds

        using (var suspendToken = scaleBounds.SuspendGetToken())
        {
          scaleBounds.Reset();
          foreach (IGPlotItem pa in PlotItems)
          {
            if (pa is IXBoundsHolder)
            {
              // merge the bounds with x and yAxis
              ((IXBoundsHolder)pa).MergeXBoundsInto(scaleBounds); // merge all x-boundaries in the x-axis boundary object
            }
          }

          // take also the axis styles with physical values into account
          foreach (CSLineID id in _axisStyles.AxisStyleIDs)
          {
            if (id.ParallelAxisNumber != 0 && id.UsePhysicalValueOtherFirst)
              scaleBounds.Add(id.PhysicalValueOtherFirst);
          }

          suspendToken.Resume();
        }
        xBoundariesChangedSuspendToken.Resume();
      }
    }

    /// <summary>
    /// This handler is called if a y-boundary from any of the plotassociations of this layer
    /// has changed. We then have to recalculate the boundaries.
    /// </summary>
    /// <remarks>Unfortunately we do not know if the boundary is extended or shrinked, if is is extended
    /// if would be possible to merge only the changed boundary into the y-axis boundary.
    /// But since we don't know about that, we have to completely recalculate the boundary be using the boundaries of
    /// all PlotAssociations of this layer.</remarks>
    protected void EhYBoundaryChangedEventFromPlotItem()
    {
      if (!_plotAssociationYBoundariesChanged_EventSuspender.IsSuspended)
      {
        // now we have to inform all the PlotAssociations that a new axis was loaded
        using (var suspendToken = _scales.Y.DataBoundsObject.SuspendGetToken())
        {
          _scales.Y.DataBoundsObject.Reset();
          foreach (IGPlotItem pa in PlotItems)
          {
            var paYB = pa as IYBoundsHolder;
            if (paYB is not null)
            {
              using (var paToken = pa.SuspendGetToken()) // we have to suspend the plotitem. When the boundary data in the plot item are not uptodate, it would otherwise create new BoundaryChangedEventArgs, which would lead to inefficiency or a stack overflow
              {
                // merge the bounds with x and yAxis
                paYB.MergeYBoundsInto(_scales.Y.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object

                paToken.ResumeSilently(); // resume the plot item silently here in order not to create further BoundaryChangedEventArgs
              }
            }
          }
          suspendToken.Resume();
        }
      }
    }

    /// <summary>
    /// Initializes the y scale data bounds, for instance if the scale instance has changed or was deserialized.
    /// </summary>
    protected void InitializeYScaleDataBounds()
    {
      if (PlotItems is null)
        return; // can happen during deserialization

      var scaleBounds = _scales.Y.DataBoundsObject;

      if (scaleBounds is null)
        return;

      // we have to disable our own Handler since if we change one DataBound of a association,
      //it generates a OnBoundaryChanged, and then all boundaries are merges into the axis boundary,
      //but (alas!) not all boundaries are now of the new type!
      using (var yBoundariesChangedSuspendToken = _plotAssociationYBoundariesChanged_EventSuspender.SuspendGetToken())
      {
        PlotItems.PrepareScales(this);

        using (var suspendToken = scaleBounds.SuspendGetToken())
        {
          scaleBounds.Reset();
          foreach (IGPlotItem pa in PlotItems)
          {
            if (pa is IYBoundsHolder)
            {
              // merge the bounds with x and yAxis
              ((IYBoundsHolder)pa).MergeYBoundsInto(scaleBounds); // merge all x-boundaries in the x-axis boundary object
            }
          }
          // take also the axis styles with physical values into account
          foreach (CSLineID id in _axisStyles.AxisStyleIDs)
          {
            if (id.ParallelAxisNumber == 0 && id.UsePhysicalValueOtherFirst)
              scaleBounds.Add(id.PhysicalValueOtherFirst);
            else if (id.ParallelAxisNumber == 2 && id.UsePhysicalValueOtherSecond)
              scaleBounds.Add(id.PhysicalValueOtherSecond);
          }

          suspendToken.Resume();
        }
        yBoundariesChangedSuspendToken.Resume();
      }
      _scales.Y.OnUserRescaled();
    }

    /// <summary>
    /// Absorbs the event from the ScaleCollection and distributes it further.
    /// </summary>
    /// <param name="e">The event data of the scale.</param>
    private void EhScaleInstanceChanged(ScaleInstanceChangedEventArgs e)
    {
      if (object.ReferenceEquals(_scales?.X, e.NewScale))
        InitializeXScaleDataBounds();

      if (object.ReferenceEquals(_scales?.Y, e.NewScale))
        InitializeYScaleDataBounds();
    }

    #endregion Handler of child events

    #region IDocumentNode Members

    /// <summary>
    /// retrieves the object with the name <code>name</code>.
    /// </summary>
    /// <param name="name">The objects name.</param>
    /// <returns>The object with the specified name.</returns>
    public override Main.IDocumentLeafNode? GetChildObjectNamed(string name)
    {
      if (name == "PlotItems")
        return _plotItems;

      return base.GetChildObjectNamed(name);
    }

    /// <summary>
    /// Retrieves the name of the provided object.
    /// </summary>
    /// <param name="o">The object for which the name should be found.</param>
    /// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
    public override string? GetNameOfChildObject(Main.IDocumentLeafNode o)
    {
      if (object.ReferenceEquals(_plotItems, o))
        return "PlotItems";

      return base.GetNameOfChildObject(o);
    }

    private IEnumerable<Main.DocumentNodeAndName> GetMyDocumentNodeChildrenWithName()
    {
      if (_scales is not null)
        yield return new Main.DocumentNodeAndName(_scales, "Scales");

      if (_plotItems is not null)
        yield return new Main.DocumentNodeAndName(_plotItems, "PlotItems");

      if (_axisStyles is not null)
        yield return new Main.DocumentNodeAndName(_axisStyles, "AxisStyles");

      if (_gridPlanes is not null)
        yield return new Main.DocumentNodeAndName(_gridPlanes, "Grids");

      if (_coordinateSystem is not null)
        yield return new Main.DocumentNodeAndName(_coordinateSystem, "CoordinateSystem");
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      return GetMyDocumentNodeChildrenWithName().Concat(base.GetDocumentNodeChildrenWithName());
    }

    protected override void Dispose(bool isDisposing)
    {
      if (isDisposing)
      {
        ChildDisposeMember(ref _scales!);
        ChildDisposeMember(ref _plotItems!);
        ChildDisposeMember(ref _axisStyles!);
        ChildDisposeMember(ref _gridPlanes!);
        ChildDisposeMember(ref _coordinateSystem!);
      }
      base.Dispose(isDisposing);
    }

    #endregion IDocumentNode Members

    #region Inner types

    public bool IsLinear { get { return XAxis is LinearScale && YAxis is LinearScale; } }

    public G2DCoordinateSystem CoordinateSystem
    {
      get
      {
        return _coordinateSystem;
      }
      [MemberNotNull(nameof(_coordinateSystem))]
      set
      {
        if (value is null)
          throw new ArgumentNullException();

        if (object.ReferenceEquals(_coordinateSystem, value))
          return;

        _coordinateSystem = value;
        _coordinateSystem.ParentObject = this;

        _coordinateSystem.UpdateAreaSize(Size);

        if (AxisStyles is not null)
          AxisStyles.UpdateCoordinateSystem(value);

        EhSelfChanged(EventArgs.Empty);
      }
    }

    #endregion Inner types

    #region Old types no longer in use but needed for deserialization

    /// <summary>
    /// AxisStylesSummary collects all styles that correspond to one axis scale (i.e. either x-axis or y-axis)
    /// in one class. This contains the grid style of the axis, and one or more axis styles
    /// </summary>
    private class ScaleStyle
    {
      private GridStyle? _gridStyle;
      private List<AxisStyle> _axisStyles;

      //G2DCoordinateSystem _cachedCoordinateSystem;

      #region Serialization

      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerAxisStylesSummary", 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          throw new NotSupportedException("Serialization of old versions not supported - probably a programming error");
          /*
                    XYPlotLayerAxisStylesSummary s = (XYPlotLayerAxisStylesSummary)obj;
                    info.AddValue("Grid", s._gridStyle);

                    info.CreateArray("Edges", s._edges.Length);
                    for (int i = 0; i < s._edges.Length; ++i)
                        info.AddEnum("e", s._edges[i]);
                    info.CommitArray();

                    info.CreateArray("AxisStyles",s._axisStyles.Length);
                    for(int i=0;i<s._axisStyles.Length;++i)
                        info.AddValue("e",s._axisStyles[i]);
                    info.CommitArray();
                    */
        }

        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          ScaleStyle s = SDeserialize(o, info, parent);
          return s;
        }

        protected virtual ScaleStyle SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (ScaleStyle?)o ?? new ScaleStyle();

          s.GridStyle = info.GetValueOrNull<GridStyle>("Grid", s);

          int count = info.OpenArray();
          //s._edges = new EdgeType[count];
          for (int i = 0; i < count; ++i)
            info.GetEnum("e", typeof(EdgeType));
          info.CloseArray(count);

          count = info.OpenArray();
          //s._axisStyles = new XYPlotLayerAxisStyleProperties[count];
          for (int i = 0; i < count; ++i)
            s._axisStyles.Add((AxisStyle)info.GetValue("e", s));
          info.CloseArray(count);

          return s;
        }
      }

      // 2006-09-08 - renaming to G2DScaleStyle
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScaleStyle), 1)]
      private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          throw new NotImplementedException("Serialization of old versions is not supported");
          /*
                    ScaleStyle s = (ScaleStyle)obj;

                    info.AddValue("Grid", s._gridStyle);

                    info.CreateArray("AxisStyles", s._axisStyles.Count);
                    for (int i = 0; i < s._axisStyles.Count; ++i)
                        info.AddValue("e", s._axisStyles[i]);
                    info.CommitArray();
                    */
        }

        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          ScaleStyle s = SDeserialize(o, info, parent);
          return s;
        }

        protected virtual ScaleStyle SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (ScaleStyle?)o ?? new ScaleStyle();

          s.GridStyle = info.GetValueOrNull<GridStyle>("Grid", s);

          int count = info.OpenArray();
          //s._axisStyles = new XYPlotLayerAxisStyleProperties[count];
          for (int i = 0; i < count; ++i)
            s._axisStyles.Add((AxisStyle)info.GetValue("e", s));
          info.CloseArray(count);

          return s;
        }
      }

      #endregion Serialization

      /// <summary>
      /// Default constructor. Defines neither a grid style nor an axis style.
      /// </summary>
      public ScaleStyle()
      {
        _axisStyles = new List<AxisStyle>();
      }

      private void CopyFrom(ScaleStyle from)
      {
        if (object.ReferenceEquals(this, from))
          return;

        GridStyle = (GridStyle?)from._gridStyle?.Clone();

        _axisStyles.Clear();
        for (int i = 0; i < _axisStyles.Count; ++i)
        {
          AddAxisStyle((AxisStyle)from._axisStyles[i].Clone());
        }
      }

      public void AddAxisStyle(AxisStyle value)
      {
        if (value is not null)
        {
          _axisStyles.Add(value);
        }
      }

      public void RemoveAxisStyle(CSLineID id)
      {
        int idx = -1;
        for (int i = 0; i < _axisStyles.Count; i++)
        {
          if (_axisStyles[i].StyleID == id)
          {
            idx = i;
            break;
          }
        }

        if (idx > 0)
          _axisStyles.RemoveAt(idx);
      }

      public AxisStyle AxisStyleEnsured(CSLineID id)
      {
        var prop = AxisStyle(id);
        if (prop is null)
        {
          prop = new AxisStyle(id, false, false, false, null, PropertyExtensions.GetPropertyContextOfProject());
          // prop.CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(id);
          AddAxisStyle(prop);
        }
        return prop;
      }

      public bool ContainsAxisStyle(CSLineID id)
      {
        return AxisStyle(id) is not null;
      }

      public AxisStyle? AxisStyle(CSLineID id)
      {
        foreach (AxisStyle p in _axisStyles)
          if (p.StyleID == id)
            return p;

        return null;
      }

      public IEnumerable<AxisStyle> AxisStyles
      {
        get
        {
          return _axisStyles;
        }
      }

      public GridStyle? GridStyle
      {
        get { return _gridStyle; }
        set
        {
          _gridStyle = value;
        }
      }
    }

    /// <summary>
    /// This class holds the (normally two for 2D) AxisStylesSummaries - for every axis scale one summary.
    /// </summary>
    private class G2DScaleStyleCollection
    {
      private ScaleStyle[] _styles;

      #region Serialization

      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerAxisStylesSummaryCollection", 0)]
      // 2006-09-08 renamed to G2DScaleStyleCollection
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DScaleStyleCollection), 1)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          throw new NotImplementedException("Serialization of old versions is not supported");
          /*
                    G2DScaleStyleCollection s = (G2DScaleStyleCollection)obj;

                    info.CreateArray("Styles", s._styles.Length);
                    for (int i = 0; i < s._styles.Length; ++i)
                        info.AddValue("e", s._styles[i]);
                    info.CommitArray();
                    */
        }

        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          G2DScaleStyleCollection s = SDeserialize(o, info, parent);
          return s;
        }

        protected virtual G2DScaleStyleCollection SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (G2DScaleStyleCollection?)o ?? new G2DScaleStyleCollection();

          int count = info.OpenArray();
          s._styles = new ScaleStyle[count];
          for (int i = 0; i < count; ++i)
            s.SetScaleStyle((ScaleStyle)info.GetValue("e", s), i);
          info.CloseArray(count);

          return s;
        }
      }

      #endregion Serialization

      public G2DScaleStyleCollection()
      {
        _styles = new ScaleStyle[2];

        _styles[0] = new ScaleStyle();

        _styles[1] = new ScaleStyle();
      }

      /// <summary>
      /// Return the axis style with the given id. If this style is not present, the return value is null.
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public AxisStyle? AxisStyle(CSLineID id)
      {
        ScaleStyle scaleStyle = _styles[id.ParallelAxisNumber];
        return scaleStyle.AxisStyle(id);
      }

      /// <summary>
      /// This will return an axis style with the given id. If not present, this axis style will be created, added to the collection, and returned.
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public AxisStyle AxisStyleEnsured(CSLineID id)
      {
        ScaleStyle scaleStyle = _styles[id.ParallelAxisNumber];
        return scaleStyle.AxisStyleEnsured(id);
      }

      public void RemoveAxisStyle(CSLineID id)
      {
        ScaleStyle scaleStyle = _styles[id.ParallelAxisNumber];
        scaleStyle.RemoveAxisStyle(id);
      }

      public IEnumerable<AxisStyle> AxisStyles
      {
        get
        {
          for (int i = 0; i < _styles.Length; i++)
          {
            foreach (AxisStyle style in _styles[i].AxisStyles)
              yield return style;
          }
        }
      }

      public IEnumerable<CSLineID> AxisStyleIDs
      {
        get
        {
          for (int i = 0; i < _styles.Length; i++)
          {
            foreach (AxisStyle style in _styles[i].AxisStyles)
              yield return style.StyleID;
          }
        }
      }

      public bool ContainsAxisStyle(CSLineID id)
      {
        ScaleStyle scalestyle = _styles[id.ParallelAxisNumber];
        return scalestyle.ContainsAxisStyle(id);
      }

      public ScaleStyle ScaleStyle(int i)
      {
        return _styles[i];
      }

      public void SetScaleStyle(ScaleStyle value, int i)
      {
        if (i < 0)
          throw new ArgumentOutOfRangeException("Index i is negative");
        if (i >= _styles.Length)
          throw new ArgumentOutOfRangeException("Index i is greater than length of internal array");

        ScaleStyle oldvalue = _styles[i];
        _styles[i] = value;
      }

      public ScaleStyle X
      {
        get
        {
          return _styles[0];
        }
      }

      public ScaleStyle Y
      {
        get
        {
          return _styles[1];
        }
      }
    }

    #endregion Old types no longer in use but needed for deserialization

    #region IGraphicShape placeholder for items in XYPlotLayer

    private abstract class PlaceHolder : Main.SuspendableDocumentLeafNodeWithEventArgs, IGraphicBase
    {
      /// <summary>
      /// Determines whether this graphical object is compatible with the parent specified in the argument.
      /// </summary>
      /// <param name="parentObject">The parent object.</param>
      /// <returns>
      ///   <c>True</c> if this object is compatible with the parent object; otherwise <c>false</c>.
      /// </returns>
      public virtual bool IsCompatibleWithParent(object parentObject)
      {
        return parentObject is XYPlotLayer;
      }

      public virtual void SetParentSize(PointD2D parentSize, bool isTriggeringChangedEvent)
      {
      }

      public virtual IHitTestObject? HitTest(HitTestPointData hitData)
      {
        return null;
      }

      public virtual IHitTestObject? HitTest(HitTestRectangularData hitData)
      {
        return null;
      }

      public virtual void FixupInternalDataStructures()
      {
      }

      public virtual void PaintPreprocessing(IPaintContext paintContext)
      {
      }

      public abstract void Paint(Graphics g, IPaintContext paintContext);

      public virtual PointD2D Position
      {
        get
        {
          return new PointD2D(0, 0);
        }
        set
        {
        }
      }

      public virtual bool CopyFrom(object obj)
      {
        if (object.ReferenceEquals(this, obj))
          return true;
        var from = obj as PlaceHolder;
        if (from is not null)
        {
          //this.ParentObject = from.ParentObject;
          return true;
        }
        return false;
      }

      public abstract object Clone();
    }

    private abstract class AxisStylePlaceHolderBase : PlaceHolder
    {
      public int Index { get; set; }

      public override bool CopyFrom(object obj)
      {
        if (!base.CopyFrom(obj))
          return false;

        var from = obj as AxisStylePlaceHolderBase;
        if (from is not null)
          Index = from.Index;

        return true;
      }

      /// <summary>
      /// Gets the axis style this place holder substitutes.
      /// </summary>
      /// <returns></returns>
      protected AxisStyle? GetAxisStyle()
      {
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null && Index >= 0 && Index < layer._axisStyles.Count)
          return layer._axisStyles.ItemAt(Index);
        else
          return null;
      }
    }

    private class AxisStyleLinePlaceHolder : AxisStylePlaceHolderBase
    {
      #region Serialization

      /// <summary>
      /// 2013-11-27 Initial version
      /// </summary>
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyleLinePlaceHolder), 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          var s = (AxisStyleLinePlaceHolder)obj;
          info.AddValue("Index", s.Index);
        }

        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (AxisStyleLinePlaceHolder?)o ?? new AxisStyleLinePlaceHolder();
          s.Index = info.GetInt32("Index");
          return s;
        }
      }

      #endregion Serialization

      public override string ToString()
      {
        return string.Format("Axis line style #{0}", Index);
      }

      public override object Clone()
      {
        var r = new AxisStyleLinePlaceHolder();
        r.CopyFrom(this);
        return r;
      }

      public override void Paint(Graphics g, IPaintContext paintContext)
      {
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null && Index >= 0 && Index < layer._axisStyles.Count)
          layer._axisStyles.ItemAt(Index).PaintLine(g, layer);
      }

      public override IHitTestObject? HitTest(HitTestPointData hitData)
      {
        IHitTestObject? hit = null;
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null && Index >= 0 && Index < layer._axisStyles.Count)
        {
          var axisStyle = layer._axisStyles.ItemAt(Index);
          if (axisStyle.IsAxisLineEnabled && (hit = axisStyle?.AxisLineStyle?.HitTest(layer, hitData.GetHittedPointInWorldCoord(), false)) is not null)
          {
            hit.DoubleClick = AxisScaleEditorMethod;
            return hit;
          }

          // hit testing the axes - secondly now with the ticks
          // in this case the TitleAndFormat editor for the axis should be shown
          if (axisStyle?.IsAxisLineEnabled == true && (hit = axisStyle?.AxisLineStyle?.HitTest(layer, hitData.GetHittedPointInWorldCoord(), true)) is not null)
          {
            hit.DoubleClick = AxisStyleEditorMethod;
            return hit;
          }
        }
        return base.HitTest(hitData);
      }

      public override IHitTestObject? HitTest(HitTestRectangularData hitData)
      {
        IHitTestObject? hit = null;
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null && Index >= 0 && Index < layer._axisStyles.Count)
        {
          var axisStyle = layer._axisStyles.ItemAt(Index);
          if (axisStyle.IsAxisLineEnabled && (hit = axisStyle?.AxisLineStyle?.HitTest(layer, hitData, false)) is not null)
          {
            hit.DoubleClick = AxisScaleEditorMethod;
            return hit;
          }

          // hit testing the axes - secondly now with the ticks
          // in this case the TitleAndFormat editor for the axis should be shown
          if (axisStyle?.IsAxisLineEnabled == true && (hit = axisStyle?.AxisLineStyle?.HitTest(layer, hitData, true)) is not null)
          {
            hit.DoubleClick = AxisStyleEditorMethod;
            return hit;
          }
        }
        return base.HitTest(hitData);
      }

      public override PointD2D Position // Position of the line is fixed
      {
        get
        {
          return new PointD2D(0, 0);
        }
        set
        {
        }
      }
    }

    private class AxisStyleMajorLabelPlaceHolder : AxisStylePlaceHolderBase
    {
      #region Serialization

      /// <summary>
      /// 2013-11-27 Initial version
      /// </summary>
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyleMajorLabelPlaceHolder), 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          var s = (AxisStyleMajorLabelPlaceHolder)obj;
          info.AddValue("Index", s.Index);
        }

        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (AxisStyleMajorLabelPlaceHolder?)o ?? new AxisStyleMajorLabelPlaceHolder();
          s.Index = info.GetInt32("Index");
          return s;
        }
      }

      #endregion Serialization

      public override string ToString()
      {
        return string.Format("Axis major labels #{0}", Index);
      }

      public override object Clone()
      {
        var r = new AxisStyleMajorLabelPlaceHolder();
        r.CopyFrom(this);
        return r;
      }

      public override IHitTestObject? HitTest(HitTestPointData hitData)
      {
        IHitTestObject? hit = null;
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null && Index >= 0 && Index < layer._axisStyles.Count)
        {
          var axisStyle = layer._axisStyles.ItemAt(Index);
          if (axisStyle.AreMajorLabelsEnabled && (hit = axisStyle.MajorLabelStyle?.HitTest(layer, hitData.GetHittedPointInWorldCoord())) is not null)
          {
            hit.DoubleClick = AxisLabelMajorStyleEditorMethod;
            hit.Remove = layer.EhAxisLabelMajorStyleRemove;
            return hit;
          }
        }
        return base.HitTest(hitData);
      }

      public override IHitTestObject? HitTest(HitTestRectangularData hitData)
      {
        IHitTestObject? hit = null;
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null && Index >= 0 && Index < layer._axisStyles.Count)
        {
          var axisStyle = layer._axisStyles.ItemAt(Index);
          if (axisStyle.AreMajorLabelsEnabled && (hit = axisStyle.MajorLabelStyle?.HitTest(layer, hitData)) is not null)
          {
            hit.DoubleClick = AxisLabelMajorStyleEditorMethod;
            hit.Remove = layer.EhAxisLabelMajorStyleRemove;
            return hit;
          }
        }
        return base.HitTest(hitData);
      }

      public override void Paint(Graphics g, IPaintContext paintContext)
      {
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null)
        {
          if (Index >= 0 && Index < layer._axisStyles.Count)
            layer._axisStyles.ItemAt(Index).PaintMajorLabels(g, layer);
        }
      }

      public override PointD2D Position // Position of the line is fixed
      {
        get
        {
          return new PointD2D(0, 0);
        }
        set
        {
        }
      }
    }

    private class AxisStyleMinorLabelPlaceHolder : AxisStylePlaceHolderBase
    {
      #region Serialization

      /// <summary>
      /// 2013-11-27 Initial version
      /// </summary>
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyleMinorLabelPlaceHolder), 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          var s = (AxisStyleMinorLabelPlaceHolder)obj;
          info.AddValue("Index", s.Index);
        }

        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (AxisStyleMinorLabelPlaceHolder?)o ?? new AxisStyleMinorLabelPlaceHolder();
          s.Index = info.GetInt32("Index");
          return s;
        }
      }

      #endregion Serialization

      public override string ToString()
      {
        return string.Format("Axis minor labels #{0}", Index);
      }

      public override object Clone()
      {
        var r = new AxisStyleMinorLabelPlaceHolder();
        r.CopyFrom(this);
        return r;
      }

      public override IHitTestObject? HitTest(HitTestPointData hitData)
      {
        IHitTestObject? hit = null;
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null && Index >= 0 && Index < layer._axisStyles.Count)
        {
          var axisStyle = layer._axisStyles.ItemAt(Index);
          if (axisStyle.AreMinorLabelsEnabled && (hit = axisStyle.MinorLabelStyle?.HitTest(layer, hitData.GetHittedPointInWorldCoord())) is not null)
          {
            hit.DoubleClick = AxisLabelMinorStyleEditorMethod;
            hit.Remove = layer.EhAxisLabelMinorStyleRemove;
            return hit;
          }
        }
        return base.HitTest(hitData);
      }

      public override IHitTestObject? HitTest(HitTestRectangularData hitData)
      {
        IHitTestObject? hit = null;
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null && Index >= 0 && Index < layer._axisStyles.Count)
        {
          var axisStyle = layer._axisStyles.ItemAt(Index);
          if (axisStyle.AreMinorLabelsEnabled && (hit = axisStyle.MinorLabelStyle?.HitTest(layer, hitData)) is not null)
          {
            hit.DoubleClick = AxisLabelMinorStyleEditorMethod;
            hit.Remove = layer.EhAxisLabelMinorStyleRemove;
            return hit;
          }
        }
        return base.HitTest(hitData);
      }

      public override void Paint(Graphics g, IPaintContext paintContext)
      {
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null)
        {
          if (Index >= 0 && Index < layer._axisStyles.Count)
            layer._axisStyles.ItemAt(Index).PaintMinorLabels(g, layer);
        }
      }

      public override PointD2D Position // Position of the line is fixed
      {
        get
        {
          return new PointD2D(0, 0);
        }
        set
        {
        }
      }
    }

    private class AxisStyleTitlePlaceHolder : AxisStylePlaceHolderBase
    {
      #region Serialization

      /// <summary>
      /// 2013-11-27 Initial version
      /// </summary>
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyleTitlePlaceHolder), 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          var s = (AxisStyleTitlePlaceHolder)obj;
          info.AddValue("Index", s.Index);
        }

        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (AxisStyleTitlePlaceHolder?)o ?? new AxisStyleTitlePlaceHolder();
          s.Index = info.GetInt32("Index");
          return s;
        }
      }

      #endregion Serialization

      public AxisStyleTitlePlaceHolder()
      {
      }

      public override string ToString()
      {
        return string.Format("Axis title #{0}", Index);
      }

      public override object Clone()
      {
        var r = new AxisStyleTitlePlaceHolder();
        r.CopyFrom(this);
        return r;
      }

      public override void SetParentSize(PointD2D parentSize, bool isTriggeringChangedEvent)
      {
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null && layer._axisStyles is not null)
        {
          if (Index >= 0 && Index < layer._axisStyles.Count)
          {
            var title = layer._axisStyles.ItemAt(Index).Title;
            if (title is not null)
              title.SetParentSize(parentSize, isTriggeringChangedEvent);
          }
        }
      }

      public override IHitTestObject? HitTest(HitTestPointData hitData)
      {
        IHitTestObject? hit = null;
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null && Index >= 0 && Index < layer._axisStyles.Count)
        {
          var axisStyle = layer._axisStyles.ItemAt(Index);
          if (axisStyle is not null && axisStyle.Title is not null && (hit = axisStyle.Title.HitTest(hitData)) is not null)
          {
            if (hit.Remove is null)
              hit.Remove = EhRemoveAxisStyleTitle;
            return hit;
          }
        }
        return base.HitTest(hitData);
      }

      public override IHitTestObject? HitTest(HitTestRectangularData hitData)
      {
        IHitTestObject? hit = null;
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null && Index >= 0 && Index < layer._axisStyles.Count)
        {
          var axisStyle = layer._axisStyles.ItemAt(Index);
          if (axisStyle is not null && axisStyle.Title is not null && (hit = axisStyle.Title.HitTest(hitData)) is not null)
          {
            if (hit.Remove is null)
              hit.Remove = EhRemoveAxisStyleTitle;
            return hit;
          }
        }
        return base.HitTest(hitData);
      }

      private static bool EhRemoveAxisStyleTitle(IHitTestObject o)
      {
        var go = (GraphicBase)o.HittedObject;
        var layer = o.ParentLayer as XYPlotLayer;
        if (layer is not null)
        {
          foreach (AxisStyle style in layer._axisStyles)
          {
            if (object.ReferenceEquals(go, style.Title))
            {
              style.Title = null;
              return true;
            }
          }
        }
        return false;
      }

      public override void Paint(Graphics g, IPaintContext paintContext)
      {
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null)
        {
          if (Index >= 0 && Index < layer._axisStyles.Count)
            layer._axisStyles.ItemAt(Index).PaintTitle(g, paintContext, layer);
        }
      }

      public override PointD2D Position // Position of the line is fixed
      {
        get
        {
          var layer = ParentObject as XYPlotLayer;
          if (layer is not null && Index >= 0 && Index < layer._axisStyles.Count)
            return layer._axisStyles.ItemAt(Index)?.Title?.Position ?? PointD2D.Empty;
          else
            return PointD2D.Empty;
        }
        set
        {
        }
      }
    }

    private class GridPlanesPlaceHolder : PlaceHolder
    {
      #region Serialization

      /// <summary>
      /// 2013-11-27 Initial version
      /// </summary>
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GridPlanesPlaceHolder), 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          var s = (GridPlanesPlaceHolder)obj;
        }

        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (GridPlanesPlaceHolder?)o ?? new GridPlanesPlaceHolder();
          return s;
        }
      }

      #endregion Serialization

      public override string ToString()
      {
        return string.Format("Grid plane(s)");
      }

      public override void Paint(Graphics g, IPaintContext paintContext)
      {
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null)
        {
          layer._gridPlanes.PaintGrid(g, layer);
        }
      }

      public override object Clone()
      {
        var r = new GridPlanesPlaceHolder();
        r.CopyFrom(this);
        return r;
      }
    }

    private class PlotItemPlaceHolder : PlaceHolder
    {
      public PlotItemCollection? PlotItemParent { get; set; }

      public int PlotItemIndex { get; set; }

      #region Serialization

      /// <summary>
      /// 2013-11-27 Initial version
      /// </summary>
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotItemPlaceHolder), 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          // Note there is no need to serialize PlotItemParent nor PlotItemIndex, since these variables are used only temporarily
        }

        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (PlotItemPlaceHolder?)o ?? new PlotItemPlaceHolder();
          return s;
        }
      }

      #endregion Serialization

      public override bool CopyFrom(object obj)
      {
        if (!base.CopyFrom(obj))
          return false;

        var from = obj as PlotItemPlaceHolder;
        if (from is not null)
        {
          PlotItemParent = from.PlotItemParent;
          PlotItemIndex = from.PlotItemIndex;
        }

        return true;
      }

      public override string ToString()
      {
        if (PlotItemParent is null)
        {
          return "Plot item collection (root)";
        }
        else
        {
          if (0 <= PlotItemIndex && PlotItemIndex < PlotItemParent.Count)
          {
            var pi = PlotItemParent[PlotItemIndex];
            if (pi is PlotItemCollection)
              return "Plot item collection";
            else
              return "Plot item " + pi.ToString();
          }
        }
        return "Plot item";
      }

      public override void Paint(Graphics g, IPaintContext paintContext)
      {
        var layer = ParentObject as XYPlotLayer;
        if (layer is not null)
        {
          if (layer.ClipDataToFrame == LayerDataClipping.StrictToCS)
          {
            g.Clip = layer.CoordinateSystem.GetRegion();
          }

          if (PlotItemParent is null) // this is the root plot item of the layer
          {
            if (layer._plotItems is null)
            {
              throw new InvalidOperationException("The member _plotItems is null on this layer!");
            }

            layer._plotItems.Paint(g, paintContext, layer, null, null);
          }
          else
          {
            PlotItemParent.PaintChild(g, paintContext, layer, PlotItemIndex);
          }

          if (layer.ClipDataToFrame == LayerDataClipping.StrictToCS)
          {
            g.ResetClip();
          }
        }
      }

      public override object Clone()
      {
        var r = new PlotItemPlaceHolder();
        r.CopyFrom(this);
        return r;
      }

      public override IHitTestObject? HitTest(HitTestPointData hitData)
      {
        if (PlotItemParent is null)
          return null;
        if (PlotItemIndex < 0 || PlotItemIndex >= PlotItemParent.Count)
          return null;
        var layer = ParentObject as IPlotArea;
        if (layer is null)
          return null;

        var xylayer = layer as XYPlotLayer;
        if (xylayer is not null && xylayer.ClipDataToFrame != LayerDataClipping.None) // if data are clipped, we search only if clicked inside the layer
        {
          var region = layer.CoordinateSystem.GetRegion();
          var pt = hitData.GetHittedPointInWorldCoord();
          if (!region.IsVisible((float)pt.X, (float)pt.Y))
            return null;
        }

        var result = PlotItemParent[PlotItemIndex].HitTest(layer, hitData.GetHittedPointInWorldCoord());
        if (result is not null)
        {
          if (result.DoubleClick is null)
            result.DoubleClick = PlotItemEditorMethod;
          if (result.Remove is null)
            result.Remove = PlotItemParent.EhHitTestObject_Remove;
        }

        return result;
      }
    }

    private class LegendText : TextGraphic
    {
      #region Serialization

      /// <summary>
      /// 2013-11-27 Initial version
      /// </summary>
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LegendText), 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          var s = (LegendText)obj;
          info.AddBaseValueEmbedded(s, typeof(LegendText).BaseType!);
        }

        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (LegendText?)o ?? new LegendText(info);
          info.GetBaseValueEmbedded(s, typeof(LegendText).BaseType!, this);
          return s;
        }
      }

      #endregion Serialization

      public LegendText(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
        : base(context)
      {
      }

      protected LegendText(Serialization.Xml.IXmlDeserializationInfo info)
        : base(info)
      {
      }

      public LegendText(TextGraphic from)
        : base(from)
      {
      }

      public override string ToString()
      {
        return string.Format("Legend text");
      }
    }

    #endregion IGraphicShape placeholder for items in XYPlotLayer
  }
}
