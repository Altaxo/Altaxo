﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Scales.Ticks;

namespace Altaxo.Graph.Graph3D
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Main.Properties;
  using Axis;
  using Data;
  using GraphicsContext;
  using Plot;
  using Shapes;

  /// <summary>
  /// XYPlotLayer represents a rectangular area on the graph, which holds plot curves, axes and graphical elements.
  /// </summary>
  public partial class XYZPlotLayer
    :
    HostLayer,
    IPlotArea
  {
    #region Member variables

    protected G3DCoordinateSystem _coordinateSystem;

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

    /// <summary>Number of times this event is disables, or 0 if it is enabled.</summary>
    [NonSerialized]
    private Main.SuspendableObject _plotAssociationZBoundariesChanged_EventSuspender = new Main.SuspendableObject();

    #endregion Member variables

    #region Serialization

    /// <summary>
    /// 2015-11-14 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZPlotLayer), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYZPlotLayer)obj;

        info.AddBaseValueEmbedded(obj, typeof(HostLayer));

        // CoordinateSystem
        info.AddValue("CoordinateSystem", s._coordinateSystem);

        // Scales
        info.AddValue("Scales", s._scales);

        // Grid planes
        info.AddValue("GridPlanes", s._gridPlanes);

        // Axis styles
        info.AddValue("AxisStyles", s._axisStyles);

        // Data clipping
        info.AddValue("DataClipping", s._dataClipping);

        // Plots
        info.AddValue("Plots", s._plotItems);
      }

      protected virtual XYZPlotLayer SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (XYZPlotLayer?)o ?? new XYZPlotLayer(info);

        info.GetBaseValueEmbedded(s, typeof(HostLayer), parent);

        // CoordinateSystem
        s.CoordinateSystem = (G3DCoordinateSystem)info.GetValue("CoordinateSystem", s);

        // Scales
        s.Scales = (ScaleCollection)info.GetValue("Scales", s);

        // Grid planes
        s.GridPlanes = (GridPlaneCollection)info.GetValue("GridPlanes", s);

        // Axis Styles
        s.AxisStyles = (AxisStyleCollection)info.GetValue("AxisStyles", s);

        // Data Clipping
        s.ClipDataToFrame = (LayerDataClipping)info.GetValue("DataClipping", s);

        // PlotItemCollection
        s.PlotItems = (PlotItemCollection)info.GetValue("Plots", s);

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        info.DeserializationFinished += s.EhDeserializationFinished;
        return s;
      }
    }

    /// <summary>
    /// Takes final measures after the deserialization has finished, but before the dirty flag is cleared. Here, the scale bounds are updated with
    /// the data from the project.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
    /// <param name="documentRoot">The document root of the current document.</param>
    /// <param name="isFinallyCall">If set to <c>true</c> this is the last callback before the dirty flag is cleared for the document.</param>
    protected virtual void EhDeserializationFinished(Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinallyCall)
    {
      if (isFinallyCall)
      {
        // after deserialisation the data bounds object of the scale is empty:
        // then we have to rescale the axis
        if (Scales.X.DataBoundsObject.IsEmpty)
          InitializeXScaleDataBounds();
        if (Scales.Y.DataBoundsObject.IsEmpty)
          InitializeYScaleDataBounds();
        if (Scales.Z.DataBoundsObject.IsEmpty)
          InitializeZScaleDataBounds();
      }
    }

    #endregion Serialization

    #region Constructors

    #region Copying

    /// <summary>
    /// The copy constructor.
    /// </summary>
    /// <param name="from"></param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public XYZPlotLayer(XYZPlotLayer from)
      : base(from)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    /// <summary>
    /// Internal copy from operation. It is presumed, that the events are already suspended. Additionally,
    /// it is not neccessary to call the OnChanged event, since this is called in the calling routine.
    /// </summary>
    /// <param name="obj">The object (layer) from which to copy.</param>
    /// <param name="options">Copy options.</param>
    protected override void InternalCopyFrom(HostLayer obj, Gdi.GraphCopyOptions options)
    {
      base.InternalCopyFrom(obj, options); // base copy, but keep in mind that InternalCopyGraphItems is overridden in this class

      if (!(obj is XYZPlotLayer from))
        return;

      if (0 != (options & Gdi.GraphCopyOptions.CopyLayerScales))
      {
        CoordinateSystem = from.CoordinateSystem; // immutable

        Scales = from._scales.Clone();
        _dataClipping = from._dataClipping;
      }

      if (0 != (options & Gdi.GraphCopyOptions.CopyLayerGrid))
      {
        GridPlanes = from._gridPlanes.Clone();
      }

      // Styles

      if (0 != (options & Gdi.GraphCopyOptions.CopyLayerAxes))
      {
        AxisStyles = (AxisStyleCollection)from._axisStyles.Clone();
      }

      // Plot items
      if (0 != (options & Gdi.GraphCopyOptions.CopyLayerPlotItems))
      {
        PlotItems = new PlotItemCollection(this, from._plotItems, true);
      }
      else if (0 != (options & Gdi.GraphCopyOptions.CopyLayerPlotStyles))
      {
        // TODO apply the styles from from._plotItems to the PlotItems here
        PlotItems.CopyFrom(from._plotItems, options);
      }
    }

    protected override void InternalCopyGraphItems(HostLayer from, Gdi.GraphCopyOptions options)
    {
      bool bGraphItems = options.HasFlag(Gdi.GraphCopyOptions.CopyLayerGraphItems);
      bool bChildLayers = options.HasFlag(Gdi.GraphCopyOptions.CopyChildLayers);
      bool bLegends = options.HasFlag(Gdi.GraphCopyOptions.CopyLayerLegends);

      var criterium = new Func<IGraphicBase, bool>(x =>
      {
        if (x is Gdi.HostLayer)
          return bChildLayers;

        if (x is LegendText)
          return bLegends;

        return bGraphItems;
      });

      InternalCopyGraphItems(from, options, criterium);
    }

    public override object Clone()
    {
      return new XYZPlotLayer(this);
    }

    #endregion Copying

    /// <summary>
    /// Constructor for deserialization purposes only.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected XYZPlotLayer(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(info)
    {
      CoordinateSystem = new CS.G3DCartesicCoordinateSystem();
      AxisStyles = new AxisStyleCollection();
      Scales = new ScaleCollection(3);
      Location = new ItemLocationDirect();
      GridPlanes = new GridPlaneCollection
      {
        new GridPlane(CSPlaneID.Front)
      };
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    public XYZPlotLayer(HostLayer parentLayer)
      : this(parentLayer, GetChildLayerDefaultLocation(), new CS.G3DCartesicCoordinateSystem())
    {
    }

    public XYZPlotLayer(HostLayer parentLayer, G3DCoordinateSystem coordinateSystem)
      : this(parentLayer, GetChildLayerDefaultLocation(), coordinateSystem)
    {
    }

    /// <summary>
    /// Creates a layer at the designated <paramref name="location"/>
    /// </summary>
    /// <param name="parentLayer">The parent layer of the constructed layer.</param>
    /// <param name="location">The location of the constructed layer.</param>
    public XYZPlotLayer(HostLayer parentLayer, IItemLocation location)
      : this(parentLayer, location, new CS.G3DCartesicCoordinateSystem())
    {
    }

    /// <summary>
    /// Creates a layer at the provided <paramref name="location"/>.
    /// </summary>
    /// <param name="parentLayer">The parent layer of the newly created layer.</param>
    /// <param name="location">The position of the layer on the printable area in points (1/72 inch).</param>
    /// <param name="coordinateSystem">The coordinate system to use for the layer.</param>
    public XYZPlotLayer(HostLayer parentLayer, IItemLocation location, G3DCoordinateSystem coordinateSystem)
      : base(parentLayer, location)
    {
      CoordinateSystem = coordinateSystem;
      AxisStyles = new AxisStyleCollection();
      Scales = new ScaleCollection(3);
      GridPlanes = new GridPlaneCollection
      {
        new GridPlane(CSPlaneID.Front)
      };
      PlotItems = new PlotItemCollection(this);
    }

    #endregion Constructors

    #region IPlotLayer methods

    public bool Is3D { get { return true; } }

    public Scale ZAxis { get { return _scales[2]; } }

    public Scale GetScale(int i)
    {
      return _scales[i];
    }

    public Logical3D GetLogical3D(I3DPhysicalVariantAccessor acc, int idx)
    {
      Logical3D r;
      r.RX = XAxis.PhysicalVariantToNormal(acc.GetXPhysical(idx));
      r.RY = YAxis.PhysicalVariantToNormal(acc.GetYPhysical(idx));
      r.RZ = ZAxis.PhysicalVariantToNormal(acc.GetZPhysical(idx));
      return r;
    }

    public Logical3D GetLogical3D(AltaxoVariant x, AltaxoVariant y, AltaxoVariant z)
    {
      Logical3D r;
      r.RX = XAxis.PhysicalVariantToNormal(x);
      r.RY = YAxis.PhysicalVariantToNormal(y);
      r.RZ = ZAxis.PhysicalVariantToNormal(z);
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
        var oldvalue = _axisStyles;
        _axisStyles = value ?? throw new ArgumentNullException(nameof(AxisStyles));
        value.ParentObject = this;
        value.UpdateCoordinateSystem(CoordinateSystem);

        if (!object.ReferenceEquals(oldvalue, value))
        {
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
        _scales = value ?? throw new ArgumentNullException(nameof(Scales));
        _scales.ParentObject = this;

        using (var suspendToken = SuspendGetToken())
        {
          for (int i = 0; i < _scales.Count; i++)
          {
            Scale? oldScale = oldscales is null ? null : oldscales[i];
            Scale newScale = _scales[i];
            if (!object.ReferenceEquals(oldScale, newScale))
              EhSelfChanged(new ScaleInstanceChangedEventArgs(oldScale, newScale) { ScaleIndex = i });
          }

          oldscales?.Dispose();

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

    /// <summary>
    /// Removes the specified graphics object. Derived classes can override this function not only to remove from the collection of graph objects,
    /// but also from other places were graph objects can be stored, e.g. inside axis styles.
    /// </summary>
    /// <param name="go">The graphics object to remove..</param>
    /// <returns>True if the graph object was removed; otherwise false.</returns>
    public override bool Remove(IGraphicBase go)
    {
      if (_axisStyles.Remove(go))
        return true;
      else
        return base.Remove(go);
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
        if (ChildSetMember(ref _plotItems, value ?? throw new ArgumentNullException(nameof(PlotItems))))
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
        tgo.Position = new PointD3D(0.1 * Size.X, 0.1 * Size.Y, 0.1 * Size.Z);
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
          AxisStyles.CreateDefault(info, context);

          if (info.HasTitleByDefault)
          {
            SetAxisTitleString(info.Identifier, info.Identifier.ParallelAxisNumber == 0 ? "X axis" : info.Identifier.ParallelAxisNumber == 1 ? "Y axis" : "Z axis");
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
      EhZBoundaryChangedEventFromPlotItem();
      _scales.X.OnUserRescaled();
      _scales.Y.OnUserRescaled();
      _scales.Z.OnUserRescaled();
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
        else if (_axisStyles.AxisStyleEnsured(id)?.Title is { } title)
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
      double rx0 = 0, rx1 = 1, ry0 = 0, ry1 = 1, rz0 = 0, rz1 = 1;
      double relOnAxis = 0.5; // where on the axis is the label positioned
      if (id.ParallelAxisNumber == 0)
      {
        ry0 = ry1 = id.LogicalValueOtherFirst;
        rz0 = rz1 = id.LogicalValueOtherSecond;
      }
      else if (id.ParallelAxisNumber == 1)
      {
        rx0 = rx1 = id.LogicalValueOtherFirst;
        rz0 = rz1 = id.LogicalValueOtherSecond;
        relOnAxis = 0.4;
      }
      else if (id.ParallelAxisNumber == 2)
      {
        rx0 = rx1 = id.LogicalValueOtherFirst;
        ry0 = ry1 = id.LogicalValueOtherSecond;
        relOnAxis = 0.4;
      }
      else
      {
        throw new NotImplementedException();
      }

      Logical3D tdirection = CoordinateSystem.GetLogicalDirection(info.Identifier.ParallelAxisNumber, info.PreferredLabelSide);
      var location = CoordinateSystem.GetPositionAndNormalizedDirection(new Logical3D(rx0, ry0, rz0), new Logical3D(rx1, ry1, rz1), relOnAxis, tdirection, out var normDirection);

      axisTitle.Location.ParentAnchorX = RADouble.NewRel(location.X / Size.X); // set the x anchor of the parent
      axisTitle.Location.ParentAnchorY = RADouble.NewRel(location.Y / Size.Y); // set the y anchor of the parent
      axisTitle.Location.ParentAnchorZ = RADouble.NewRel(location.Z / Size.Z); // set the z anchor of the parent

      double distance = 0;
      var axisStyle = _axisStyles[id];
      if (axisStyle?.AxisLineStyle is not null)
        distance += axisStyle.AxisLineStyle.GetOuterDistance(info.PreferredLabelSide);
      double labelFontSize = 0;
      if (axisStyle?.MajorLabelStyle is not null)
        labelFontSize = Math.Max(labelFontSize, axisStyle.MajorLabelStyle.FontSize);
      if (axisStyle?.MinorLabelStyle is not null)
        labelFontSize = Math.Max(labelFontSize, axisStyle.MinorLabelStyle.FontSize);

      axisTitle.RotationX = 90; // Font height now is z, Font depth is y and x remains x

      axisTitle.Location.LocalAnchorX = normDirection.X == 0 ? RADouble.NewRel(0.5) : normDirection.X < 0 ? RADouble.NewRel(1) : RADouble.NewRel(0);
      axisTitle.Location.LocalAnchorY = normDirection.Z == 0 ? RADouble.NewRel(0.5) : normDirection.Z < 0 ? RADouble.NewRel(1) : RADouble.NewRel(0);
      axisTitle.Location.LocalAnchorZ = normDirection.Y == 0 ? RADouble.NewRel(0.5) : normDirection.Y < 0 ? RADouble.NewRel(1) : RADouble.NewRel(0);

      var scaleFont = new VectorD3D(1, 1, 1.4);

      distance += Math.Abs(scaleFont.X * normDirection.X) * labelFontSize;
      distance += Math.Abs(scaleFont.Y * normDirection.Y) * labelFontSize;
      distance += Math.Abs(scaleFont.Z * normDirection.Z) * labelFontSize;

      axisTitle.Location.PositionX = RADouble.NewAbs(distance * normDirection.X); // because this is relative to the reference point, we don't need to take the location into account here, it is set above
      axisTitle.Location.PositionY = RADouble.NewAbs(distance * normDirection.Y);
      axisTitle.Location.PositionZ = RADouble.NewAbs(distance * normDirection.Z);
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
    }

    public override void PaintPreprocessing(IPaintContext context)
    {
      context.PushHierarchicalValue<IPlotArea>(nameof(IPlotArea), this);
      _plotItems.PaintPreprocessing(context);
      base.PaintPreprocessing(context);
      context.PopHierarchicalValue<IPlotArea>(nameof(IPlotArea));
    }

    protected override void PaintInternal(IGraphicsContext3D g, IPaintContext paintContext)
    {
      // paint the background very first
      _gridPlanes.Paint(g, this);

      _axisStyles.Paint(g, paintContext, this);

      _plotItems.Paint(g, paintContext, this, null, null);

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

    protected override IHitTestObject? HitTestWithLocalCoordinates(HitTestPointData localCoord, bool plotItemsOnly)
    {
      IHitTestObject? hit = null;

      if (!plotItemsOnly)
      {
        hit = _axisStyles.HitTest(localCoord, AxisScaleEditorMethod, AxisStyleEditorMethod, AxisLabelMajorStyleEditorMethod, AxisLabelMinorStyleEditorMethod);
      }

      if (hit is null)
      {
        hit = base.HitTestWithLocalCoordinates(localCoord, plotItemsOnly);
      }

      if (hit is not null && hit.ParentLayer is null)
        hit.ParentLayer = this;

      return hit;
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
      if (als?.ParentObject is AxisStyle axisStyle)
      {
        axisStyle.HideMajorLabels();
        return true;
      }
      return false;
    }

    private bool EhAxisLabelMinorStyleRemove(IHitTestObject o)
    {
      var als = o.HittedObject as AxisLabelStyle;
      if (als?.ParentObject is AxisStyle axisStyle)
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
      if (_coordinateSystem is not null)
        _coordinateSystem = _coordinateSystem.WithLayerSize(Size);

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
      if (data.HasFlag(BoundariesChangedData.VBoundariesChanged) || data.HasFlag(BoundariesChangedData.ZBoundariesChanged))
        EhZBoundaryChangedEventFromPlotItem();
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
        PlotItems.PrepareScales(this);
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
    /// This handler is called if a z-boundary from any of the plotassociations of this layer
    /// has changed. We then have to recalculate the boundaries.
    /// </summary>
    /// <remarks>Unfortunately we do not know if the boundary is extended or shrinked, if is is extended
    /// if would be possible to merge only the changed boundary into the z-axis boundary.
    /// But since we don't know about that, we have to completely recalculate the boundary be using the boundaries of
    /// all PlotAssociations of this layer.</remarks>
    protected void EhZBoundaryChangedEventFromPlotItem()
    {
      if (!_plotAssociationZBoundariesChanged_EventSuspender.IsSuspended)
      {
        // now we have to inform all the PlotAssociations that a new axis was loaded
        using (var suspendToken = _scales[2].DataBoundsObject.SuspendGetToken())
        {
          _scales[2].DataBoundsObject.Reset();
          foreach (IGPlotItem pa in PlotItems)
          {
            var paZB = pa as IZBoundsHolder;
            if (paZB is not null)
            {
              using (var paToken = pa.SuspendGetToken()) // we have to suspend the plotitem. When the boundary data in the plot item are not uptodate, it would otherwise create new BoundaryChangedEventArgs, which would lead to inefficiency or a stack overflow
              {
                // merge the bounds with x and yAxis
                paZB.MergeZBoundsInto(_scales[2].DataBoundsObject); // merge all z-boundaries in the z-axis boundary object

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
    protected void InitializeZScaleDataBounds()
    {
      if (PlotItems is null)
        return; // can happen during deserialization

      var scaleBounds = _scales[2].DataBoundsObject;

      if (scaleBounds is null)
        return;

      // we have to disable our own Handler since if we change one DataBound of a association,
      //it generates a OnBoundaryChanged, and then all boundaries are merges into the axis boundary,
      //but (alas!) not all boundaries are now of the new type!
      using (var zBoundariesChangedSuspendToken = _plotAssociationZBoundariesChanged_EventSuspender.SuspendGetToken())
      {
        PlotItems.PrepareScales(this);
        using (var suspendToken = scaleBounds.SuspendGetToken())
        {
          scaleBounds.Reset();
          foreach (IGPlotItem pa in PlotItems)
          {
            if (pa is IZBoundsHolder)
            {
              // merge the bounds with x and yAxis
              ((IZBoundsHolder)pa).MergeZBoundsInto(scaleBounds); // merge all z-boundaries in the x-axis boundary object
            }
          }
          // take also the axis styles with physical values into account
          foreach (CSLineID id in _axisStyles.AxisStyleIDs)
          {
            if (id.ParallelAxisNumber == 0 && id.UsePhysicalValueOtherSecond) // z
              scaleBounds.Add(id.PhysicalValueOtherSecond);
            else if (id.ParallelAxisNumber == 1 && id.UsePhysicalValueOtherFirst)
              scaleBounds.Add(id.PhysicalValueOtherFirst);
          }

          suspendToken.Resume();
        }
        zBoundariesChangedSuspendToken.Resume();
      }
      _scales[2].OnUserRescaled();
    }

    /// <summary>
    /// Absorbs the event from the ScaleCollection and distributes it further.
    /// </summary>
    /// <param name="e">The event data of the scale.</param>
    private void EhScaleInstanceChanged(ScaleInstanceChangedEventArgs e)
    {
      if (IsDisposeInProgress)
        return;

      if (object.ReferenceEquals(_scales.X, e.NewScale))
        InitializeXScaleDataBounds();

      if (object.ReferenceEquals(_scales.Y, e.NewScale))
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
      }
      base.Dispose(isDisposing);
    }

    #endregion IDocumentNode Members

    #region Inner types

    public bool IsLinear { get { return XAxis is LinearScale && YAxis is LinearScale; } }

    public G3DCoordinateSystem CoordinateSystem
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

        _coordinateSystem = value.WithLayerSize(Size);

        if (AxisStyles is not null)
          AxisStyles.UpdateCoordinateSystem(_coordinateSystem);

        EhSelfChanged(EventArgs.Empty);
      }
    }

    #endregion Inner types

    #region IGraphicShape placeholder for items in XYPlotLayer

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
          var s = o is not null ? (LegendText)o : new LegendText(info);
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
