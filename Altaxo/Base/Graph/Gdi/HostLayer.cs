#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2013 Dr. Dirk Lellinger
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
using System.Drawing.Drawing2D;
using System.Linq;
using Altaxo.Collections;

namespace Altaxo.Graph.Gdi
{
  using System.Diagnostics.CodeAnalysis;
  using Geometry;
  using Shapes;

  public class HostLayer
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    ITreeListNodeWithParent<HostLayer>,
    IGraphicBase,
    Altaxo.Main.INamedObjectCollection
  {
    #region Constants

    protected const double _xDefPositionLandscape = 0.14;
    protected const double _yDefPositionLandscape = 0.14;
    protected const double _xDefSizeLandscape = 0.76;
    protected const double _yDefSizeLandscape = 0.7;

    protected const double _xDefPositionPortrait = 0.14;
    protected const double _yDefPositionPortrait = 0.14;
    protected const double _xDefSizePortrait = 0.7;
    protected const double _yDefSizePortrait = 0.76;

    #endregion Constants

    #region Cached member variables

    /// <summary>
    /// The cached size of the parent layer. If this here is the root layer, and hence no parent layer exist, the cached size is set to 100 x 100 mm².
    /// </summary>
    protected PointD2D _cachedParentLayerSize = new PointD2D((1000 * 72) / 254.0, (1000 * 72) / 254.0);

    /// <summary>
    /// The cached layer position in points (1/72 inch) relative to the upper left corner
    /// of the parent layer (upper left corner of the printable area).
    /// </summary>
    protected PointD2D _cachedLayerPosition;

    /// <summary>
    /// The absolute size of the layer in points (1/72 inch).
    /// </summary>
    protected PointD2D _cachedLayerSize;

    protected MatrixD2D _transformation = new MatrixD2D();

    /// <summary>
    /// The child layers of this layers (this is a partial view of the <see cref="_graphObjects"/> collection).
    /// </summary>
    protected IObservableList<HostLayer> _childLayers;

    /// <summary>
    /// The number of this layer in the parent's layer collection.
    /// </summary>
    protected int _cachedLayerNumber;

    #endregion Cached member variables

    #region Member variables

    protected IItemLocation _location;

    protected GraphicCollection _graphObjects;

    /// <summary>
    /// Defines a grid that child layers can use to arrange.
    /// </summary>
    private GridPartitioning _grid;

    #endregion Member variables

    #region Event definitions

    /// <summary>Fired when the size of the layer changed.</summary>
    [field: NonSerialized]
    public event System.EventHandler? SizeChanged;

    /// <summary>Fired when the position of the layer changed.</summary>
    [field: NonSerialized]
    public event System.EventHandler? PositionChanged;

    /// <summary>Fired when the child layer collection changed.</summary>
    [field: NonSerialized]
    public event System.EventHandler? LayerCollectionChanged;

    #endregion Event definitions

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2013-11-27 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HostLayer), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (HostLayer)obj;

        // size, position, rotation and scale
        info.AddValue("CachedParentSize", s._cachedParentLayerSize);
        info.AddValue("CachedSize", s._cachedLayerSize);
        info.AddValue("CachedPosition", s._cachedLayerPosition);
        info.AddValue("LocationAndSize", s._location);
        info.AddValue("Grid", s._grid);

        // Graphic objects
        info.AddValue("GraphObjects", s._graphObjects);
      }

      protected virtual HostLayer SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (HostLayer?)o ?? new HostLayer(info);

        if (parent is Main.IDocumentNode parentNode)
          s.ParentObject = parentNode;
        // size, position, rotation and scale
        s._cachedParentLayerSize = (PointD2D)info.GetValue("CachedParentSize", s);
        s._cachedLayerSize = (PointD2D)info.GetValue("CachedSize", s);
        s._cachedLayerPosition = (PointD2D)info.GetValue("CachedPosition", s);
        s.Location = (IItemLocation)info.GetValue("LocationAndSize", s);
        s.Grid = (GridPartitioning)info.GetValue("Grid", s);

        // Graphic objects
        s.GraphObjects.AddRange((IEnumerable<IGraphicBase>)info.GetValue("GraphObjects", s));

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        s.CalculateMatrix();
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    #region Constructors

    #region Copying

    public virtual bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as HostLayer;
      if (from is not null)
      {
        CopyFrom(from, GraphCopyOptions.All);
        return true;
      }
      return false;
    }

    public virtual void CopyFrom(HostLayer from, GraphCopyOptions options)
    {
      if (object.ReferenceEquals(this, from))
        return;

      using (var suspendToken = SuspendGetToken())
      {
        InternalCopyFrom(from, options);
        EhSelfChanged(EventArgs.Empty);  // make sure that change is called after suspend
      }
    }

    /// <summary>
    /// Internal copy from operation. It is presumed, that the events are already suspended. Additionally,
    /// it is not neccessary to call the OnChanged event, since this is called in the calling routine.
    /// </summary>
    /// <param name="from">The layer from which to copy.</param>
    /// <param name="options">Copy options.</param>
    protected virtual void InternalCopyFrom(HostLayer from, GraphCopyOptions options)
    {
      if (_parent is null)
      {
        //this._parent = from._parent; // necessary in order to set Location to GridLocation, where a parent layer is required
        _cachedLayerNumber = from._cachedLayerNumber; // is important when the layer dialog is open: this number must be identical to that of the cloned layer
      }

      ChildCopyToMember(ref _grid, from._grid);

      // size, position, rotation and scale
      if (0 != (options & GraphCopyOptions.CopyLayerSizePosition))
      {
        _cachedLayerSize = from._cachedLayerSize;
        _cachedLayerPosition = from._cachedLayerPosition;
        _cachedParentLayerSize = from._cachedParentLayerSize;
        ChildCopyToMember(ref _location, from._location);
      }

      InternalCopyGraphItems(from, options);

      // copy the properties in the child layer(s) (only the members, not the child layers itself)
      if (0 != (options & GraphCopyOptions.CopyLayerAll))
      {
        // not all properties of the child layers should be cloned -> just copy the layers one by one
        int len = Math.Min(_childLayers.Count, from._childLayers.Count);
        for (int i = 0; i < len; i++)
        {
          _childLayers[i].CopyFrom(from._childLayers[i], options);
          _childLayers[i].ParentLayer = this;
        }
      }

      _transformation = new MatrixD2D();
      CalculateMatrix();
    }

    protected virtual void InternalCopyGraphItems(HostLayer from, GraphCopyOptions options)
    {
      bool bGraphItems = options.HasFlag(GraphCopyOptions.CopyLayerGraphItems);
      bool bChildLayers = options.HasFlag(GraphCopyOptions.CopyChildLayers);

      var criterium = new Func<IGraphicBase, bool>(x =>
      {
        if (x is HostLayer)
          return bChildLayers;

        return bGraphItems;
      });

      InternalCopyGraphItems(from, options, criterium);
    }

    protected virtual void InternalCopyGraphItems(HostLayer from, GraphCopyOptions options, Func<IGraphicBase, bool> selectionCriteria)
    {
      var pwThis = _graphObjects.CreatePartialView(x => selectionCriteria(x));
      var pwFrom = from._graphObjects.CreatePartialView(x => selectionCriteria(x));
      var layersToRecycle = new List<HostLayer>(_childLayers);

      // replace existing items
      int i, j;
      for (i = 0, j = 0; i < pwThis.Count && j < pwFrom.Count; j++)
      {
        var fromObj = pwFrom[j];
        if (!fromObj.IsCompatibleWithParent(this))
          continue;

        IGraphicBase? thisObj = null;

        // if fromObj is a layer, then try to "recycle" all the layers on the This side
        if (fromObj is HostLayer)
        {
          var layerToRecycle = layersToRecycle.FirstOrDefault(x => x.GetType() == fromObj.GetType());
          if (layerToRecycle is not null)
          {
            layersToRecycle.Remove(layerToRecycle); // this layer is now recycled, thus it is no longer available for another recycling
            thisObj = (IGraphicBase)layerToRecycle.Clone(); // we have nevertheless to clone, since true recycling is dangerous, because the layer is still in our own collection
            ((HostLayer)thisObj).CopyFrom((HostLayer)fromObj, options); // copy from the other layer
          }
        }

        if (thisObj is null) // if not otherwise retrieved, simply clone the fromObj
          thisObj = (IGraphicBase)pwFrom[j].Clone();

        pwThis[i++] = thisObj; // include in our own collection
      }
      // remove superfluous items
      for (int k = pwThis.Count - 1; k >= i; --k)
        pwThis.RemoveAt(k);
      // add more layers if neccessary
      for (; j < pwFrom.Count; j++)
        pwThis.Add((IGraphicBase)pwFrom[j].Clone());
    }

    public virtual object Clone()
    {
      return new HostLayer(this);
    }

    #endregion Copying

    /// <summary>
    /// Constructor for deserialization purposes only.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected HostLayer(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      Grid = new GridPartitioning();
      InternalInitializeGraphObjectsCollection();
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    /// <summary>
    /// The copy constructor.
    /// </summary>
    /// <param name="from"></param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public HostLayer(HostLayer from)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
      Grid = new GridPartitioning();

      using (var suspendToken = SuspendGetToken()) // see below, this is to suppress the change event when cloning the layer.
      {
        InternalInitializeGraphObjectsCollection(); // Preparation of graph objects collection and its partial views
        CopyFrom(from, GraphCopyOptions.All);

        suspendToken.ResumeSilently(); // when we clone from another layer, the new layer has still the parent of the old layer. Thus we don't want that the parent of the old layer receives the changed event, since nothing has changed for it.
      }
    }

    /// <summary>
    /// Creates a layer at the designated <paramref name="location"/>.
    /// </summary>
    /// <param name="parentLayer">The parent layer of the newly created layer.</param>
    /// <param name="location">The position and size of this layer</param>
    public HostLayer(HostLayer? parentLayer, IItemLocation location)
    {
      Grid = new GridPartitioning();

      if (parentLayer is not null) // this helps to get the real layer size from the beginning
      {
        ParentLayer = parentLayer;
        _cachedParentLayerSize = parentLayer.Size;
      }

      Location = location;
      InternalInitializeGraphObjectsCollection();
      CalculateMatrix();
    }

    public HostLayer()
      : this(null, new ItemLocationDirect())
    {
    }

    #endregion Constructors

    #region Position and Size

    public static PointD2D DefaultChildLayerRelativePosition
    {
      get { return new PointD2D(0.145, 0.139); }
    }

    /// <summary>
    /// Gets the default child layer position in points (1/72 inch).
    /// </summary>
    /// <value>The default position of a (new) layer in points (1/72 inch).</value>
    public PointD2D DefaultChildLayerPosition
    {
      get { return DefaultChildLayerRelativePosition * Size; }
    }

    public static PointD2D DefaultChildLayerRelativeSize
    {
      get { return new PointD2D(0.763, 0.708); }
    }

    /// <summary>
    /// Gets the default child layer size in points (1/72 inch).
    /// </summary>
    /// <value>The default size of a (new) layer in points (1/72 inch).</value>
    public PointD2D DefaultChildLayerSize
    {
      get { return DefaultChildLayerRelativeSize * Size; }
    }

    public static IItemLocation GetChildLayerDefaultLocation()
    {
      return new ItemLocationDirect
      {
        SizeX = RADouble.NewRel(HostLayer.DefaultChildLayerRelativeSize.X),
        SizeY = RADouble.NewRel(HostLayer.DefaultChildLayerRelativeSize.Y),
        PositionX = RADouble.NewRel(HostLayer.DefaultChildLayerRelativePosition.X),
        PositionY = RADouble.NewRel(HostLayer.DefaultChildLayerRelativePosition.Y)
      };
    }

    public IItemLocation Location
    {
      get
      {
        return _location;
      }
      [MemberNotNull(nameof(_location))]
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(Location));

        if (ChildSetMember(ref _location, value))
        {
          if (_location is ItemLocationDirect directLocation)
            directLocation.SetParentSize(_cachedParentLayerSize, false);

          // Note: there is no event link here to Changed event of new location instance,
          // instead the event is and must be  handled in the EhChildChanged function of this layer

          CalculateCachedSizeAndPosition();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Set this layer to the default size and position.
    /// </summary>
    /// <param name="parentSize">The size of the parent's area.</param>
    public void SizeToDefault(PointD2D parentSize)
    {
      if (parentSize.X > parentSize.Y)
      {
        Size = new PointD2D(parentSize.X * _xDefSizeLandscape, parentSize.Y * _yDefSizeLandscape);
        Position = new PointD2D(parentSize.X * _xDefPositionLandscape, parentSize.Y * _yDefPositionLandscape);
      }
      else // Portrait
      {
        Size = new PointD2D(parentSize.X * _xDefSizePortrait, parentSize.Y * _yDefSizePortrait);
        Position = new PointD2D(parentSize.X * _xDefPositionPortrait, parentSize.Y * _yDefPositionPortrait);
      }
      CalculateMatrix();
    }

    /// <summary>
    /// The boundaries of the printable area of the page in points (1/72 inch).
    /// </summary>
    public PointD2D ParentLayerSize
    {
      get { return _cachedParentLayerSize; }
    }

    public void SetParentSize(PointD2D newParentSize, bool isTriggeringChangedEvent)
    {
      var oldParentSize = _cachedParentLayerSize;
      _cachedParentLayerSize = newParentSize;

      if (_location is ItemLocationDirect)
        ((ItemLocationDirect)_location).SetParentSize(_cachedParentLayerSize, false); // don't trigger change event now

      if (oldParentSize != newParentSize)
      {
        CalculateCachedSizeAndPosition();

        if (isTriggeringChangedEvent)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public PointD2D Position
    {
      get { return _cachedLayerPosition; }
      set
      {
        var ls = _location as ItemLocationDirect;
        if (ls is not null)
        {
          if (ls.PositionX.IsAbsolute)
            ls.PositionX = RADouble.NewAbs(value.X);
          else
            ls.PositionX = RADouble.NewRel(value.X / _cachedParentLayerSize.X);

          if (ls.PositionY.IsAbsolute)
            ls.PositionY = RADouble.NewAbs(value.Y);
          else
            ls.PositionY = RADouble.NewRel(value.Y / _cachedParentLayerSize.Y);
        }
      }
    }

    public PointD2D Size
    {
      get { return _cachedLayerSize; }
      set
      {
        var ls = _location as ItemLocationDirect;
        if (ls is not null)
        {
          if (ls.SizeX.IsAbsolute)
            ls.SizeX = RADouble.NewAbs(value.X);
          else
            ls.SizeX = RADouble.NewRel(value.X / _cachedParentLayerSize.X);

          if (ls.SizeY.IsAbsolute)
            ls.SizeY = RADouble.NewAbs(value.Y);
          else
            ls.SizeY = RADouble.NewRel(value.Y / _cachedParentLayerSize.Y);
        }
      }
    }

    public double Rotation
    {
      get { return _location.Rotation; }
      set
      {
        var oldValue = _location.Rotation;
        _location.Rotation = value;

        if (value != oldValue)
        {
          CalculateMatrix();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public double ShearX
    {
      get { return _location.ShearX; }
      set
      {
        var oldValue = _location.ShearX;
        _location.ShearX = value;

        if (value != oldValue)
        {
          CalculateMatrix();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public double ScaleX
    {
      get { return _location.ScaleX; }
      set
      {
        var oldValue = _location.ScaleX;
        _location.ScaleX = value;

        if (value != oldValue)
        {
          CalculateMatrix();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public double ScaleY
    {
      get { return _location.ScaleY; }
      set
      {
        var oldValue = _location.ScaleY;
        _location.ScaleY = value;

        if (value != oldValue)
        {
          CalculateMatrix();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    protected void CalculateMatrix()
    {
      _transformation.Reset();
      if (_location is ItemLocationDirect)
      {
        var locD = (ItemLocationDirect)_location;
        _transformation.SetTranslationRotationShearxScale(locD.AbsolutePivotPositionX, locD.AbsolutePivotPositionY, -locD.Rotation, locD.ShearX, locD.ScaleX, locD.ScaleY);
        _transformation.TranslatePrepend(locD.AbsoluteVectorPivotToLeftUpper.X, locD.AbsoluteVectorPivotToLeftUpper.Y);
      }
      else
      {
        _transformation.SetTranslationRotationShearxScale(_cachedLayerPosition.X, _cachedLayerPosition.Y, -_location.Rotation, _location.ShearX, _location.ScaleX, _location.ScaleY);
      }
    }

    public PointD2D TransformCoordinatesFromParentToHere(PointD2D pagecoordinates)
    {
      return _transformation.InverseTransformPoint(pagecoordinates);
    }

    public PointD2D TransformCoordinatesFromRootToHere(PointD2D pagecoordinates)
    {
      foreach (var layer in this.TakeFromRootToHere())
        pagecoordinates = layer._transformation.InverseTransformPoint(pagecoordinates);
      return pagecoordinates;
    }

    public MatrixD2D TransformationFromRootToHere()
    {
      var result = new MatrixD2D();
      foreach (var layer in this.TakeFromRootToHere())
        result.PrependTransform(layer._transformation);
      return result;
    }

    public CrossF GraphToLayerCoordinates(CrossF x)
    {
      return new CrossF()
      {
        Center = _transformation.InverseTransformPoint(x.Center),
        Top = _transformation.InverseTransformPoint(x.Top),
        Bottom = _transformation.InverseTransformPoint(x.Bottom),
        Left = _transformation.InverseTransformPoint(x.Left),
        Right = _transformation.InverseTransformPoint(x.Right)
      };
    }

    /// <summary>
    /// This switches the graphics context from printable area coordinates to layer coordinates.
    /// </summary>
    /// <param name="g">The graphics state to change.</param>
    public void TransformCoordinatesFromParentToHere(Graphics g)
    {
      g.MultiplyTransform(_transformation);
    }

    /// <summary>
    /// Converts X,Y differences in page units to X,Y differences in layer units
    /// </summary>
    /// <param name="pagediff">X,Y coordinate differences in graph units</param>
    /// <returns>the convertes X,Y coordinate differences in layer units</returns>
    public PointD2D TransformCoordinateDifferencesFromParentToHere(PointD2D pagediff)
    {
      return _transformation.InverseTransformVector(pagediff);
    }

    /// <summary>
    /// Transforms a graphics path from layer coordinates to graph (page) coordinates
    /// </summary>
    /// <param name="gp">the graphics path to convert</param>
    /// <returns>graphics path now in graph coordinates</returns>
    public GraphicsPath TransformCoordinatesFromHereToParent(GraphicsPath gp)
    {
      gp.Transform(_transformation);
      return gp;
    }

    /// <summary>
    /// Appends the transformation matrix of this instance to the matrix given in <paramref name="matrix"/>.
    /// </summary>
    /// <param name="matrix">The matrix to append to.</param>
    public void PrependTransformationTo(MatrixD2D matrix)
    {
      matrix.PrependTransform(_transformation);
    }

    /// <summary>
    /// Transforms a <see cref="PointD2D" /> from layer coordinates to graph (=printable area) coordinates
    /// </summary>
    /// <param name="layerCoordinates">The layer coordinates to convert.</param>
    /// <returns>graphics path now in graph coordinates</returns>
    public PointD2D TransformCoordinatesFromHereToParent(PointD2D layerCoordinates)
    {
      return _transformation.TransformPoint(layerCoordinates);
    }

    public PointD2D TransformCoordinatesFromHereToRoot(PointD2D coordinates)
    {
      foreach (var layer in this.TakeFromHereToRoot())
        coordinates = layer._transformation.TransformPoint(coordinates);
      return coordinates;
    }

    public void SetPositionSize(RADouble x, RADouble y, RADouble width, RADouble height)
    {
      ItemLocationDirect newlocation;

      if (!(_location is ItemLocationDirect))
        newlocation = new ItemLocationDirect(_location);
      else
        newlocation = (ItemLocationDirect)Location;

      newlocation.SetPositionAndSize(x, y, width, height);

      Location = newlocation;
    }

    /// <summary>
    /// Sets the cached size value in <see cref="_cachedLayerSize"/> by calculating it
    /// from the position values (<see cref="_location"/>.Width and .Height)
    /// and the size types (<see cref="_location"/>.WidthType and .HeightType).
    /// </summary>
    protected void CalculateCachedSizeAndPosition()
    {
      RectangleD2D newRect;

      if (_location is null)
      {
        return; // location is only null during deserialization
      }
      else if (_location is ItemLocationDirect lps)
      {
        newRect = lps.GetAbsoluteEnclosingRectangleWithoutSSRS();
      }
      else if (_location is ItemLocationByGrid gps)
      {
        if (ParentLayer is not null)
        {
          var gridRect = newRect = gps.GetAbsolute(ParentLayer._grid, _cachedParentLayerSize);

          if (gps.ForceFitIntoCell)
          {
            var t = new MatrixD2D();
            t.SetTranslationRotationShearxScale(0, 0, -Rotation, ShearX, ScaleX, ScaleY);
            var ele = t.Elements;
            newRect = RectangleD2DExtensions.GetIncludedTransformedRectangle(gridRect, t.SX, t.RX, t.RY, t.SY);
          }
        }
        else // ParentLayer is null, this is probably the root layer, thus use the _cachedParentLayersSize
        {
          newRect = new RectangleD2D(0, 0, _cachedParentLayerSize.X, _cachedParentLayerSize.Y);
        }
      }
      else
      {
        throw new NotImplementedException(string.Format("Unknown location type: _location is {0}", _location));
      }

      bool isPositionChanged = newRect.LeftTop != _cachedLayerPosition;
      bool isSizeChanged = newRect.Size != _cachedLayerSize;

      _cachedLayerSize = newRect.Size;
      _cachedLayerPosition = newRect.LeftTop;
      CalculateMatrix();

      if (isSizeChanged)
        OnCachedResultingSizeChanged();

      if (isPositionChanged)
        OnCachedResultingPositionChanged();
    }

    #endregion Position and Size

    #region Grid creation

    public GridPartitioning Grid
    {
      get
      {
        return _grid;
      }
      [MemberNotNull(nameof(_grid))]
      private set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(Grid));

        ChildSetMember(ref _grid, value);
      }
    }

    /// <summary>
    /// Creates the default grid. It consists of three rows and three columns. Columns 0 and 2 are the left and right margin, respectively. Rows 0 and 2 are the top and bottom margin.
    /// The cell column 1 / row 1 is intended to hold the child layer.
    /// </summary>
    [MemberNotNull(nameof(_grid))]
    public void CreateDefaultGrid()
    {
      _grid = new GridPartitioning();
      _grid.XPartitioning.Add(RADouble.NewRel(DefaultChildLayerRelativePosition.X));
      _grid.XPartitioning.Add(RADouble.NewRel(DefaultChildLayerRelativeSize.X));
      _grid.XPartitioning.Add(RADouble.NewRel(1 - DefaultChildLayerRelativePosition.X - DefaultChildLayerRelativeSize.X));

      _grid.YPartitioning.Add(RADouble.NewRel(DefaultChildLayerRelativePosition.Y));
      _grid.YPartitioning.Add(RADouble.NewRel(DefaultChildLayerRelativeSize.Y));
      _grid.YPartitioning.Add(RADouble.NewRel(1 - DefaultChildLayerRelativePosition.Y - DefaultChildLayerRelativeSize.Y));
    }

    private static double RoundToFractions(double x, int parts)
    {
      return Math.Round(x * parts) / parts;
    }

    /// <summary>
    /// If the <see cref="Grid"/> is <c>null</c>, then create a grid that represents the boundaries of the child layers.
    /// </summary>
    public void CreateGridIfNullOrEmpty()
    {
      const int RelValueRoundFraction = 1024 * 1024;

      if (_grid is not null && !_grid.IsEmpty)
        return;

      var xPositions = new HashSet<double>();
      var yPositions = new HashSet<double>();

      // Take only those positions into account, that are inside this layer

      foreach (var l in Layers)
      {
        xPositions.Add(RoundToFractions(l.Position.X / Size.X, RelValueRoundFraction));
        xPositions.Add(RoundToFractions((l.Position.X + l.Size.X) / Size.X, RelValueRoundFraction));
        yPositions.Add(RoundToFractions(l.Position.Y / Size.Y, RelValueRoundFraction));
        yPositions.Add(RoundToFractions((l.Position.Y + l.Size.Y) / Size.Y, RelValueRoundFraction));
      }

      xPositions.Add(1);
      yPositions.Add(1);

      var xPosPurified = new SortedSet<double>(xPositions.Where(x => x >= 0 && x <= 1));
      var yPosPurified = new SortedSet<double>(yPositions.Where(y => y >= 0 && y <= 1));

      _grid = new GridPartitioning(); // make a new grid, but assign a parent only below in order to avoid unneccessary change notifications

      double prev;

      prev = 0;
      foreach (var x in xPosPurified)
      {
        _grid.XPartitioning.Add(RADouble.NewRel(x - prev));
        prev = x;
      }
      prev = 0;
      foreach (var y in yPosPurified)
      {
        _grid.YPartitioning.Add(RADouble.NewRel(y - prev));
        prev = y;
      }

      // ensure that we always have an odd number of columns and rows
      // if there is no child layer present, then at least one row and one column should be present
      if (0 == _grid.XPartitioning.Count % 2)
        _grid.XPartitioning.Add(RADouble.NewRel(_grid.XPartitioning.Count == 0 ? 1 : 0));
      if (0 == _grid.YPartitioning.Count % 2)
        _grid.YPartitioning.Add(RADouble.NewRel(_grid.YPartitioning.Count == 0 ? 1 : 0));

      foreach (var l in Layers)
      {
        if (!(l.Location is ItemLocationByGrid))
        {
          var idX1 = Math.Round(_grid.XPartitioning.GetGridIndexFromAbsolutePosition(Size.X, l.Position.X), 3);
          var idX2 = Math.Round(_grid.XPartitioning.GetGridIndexFromAbsolutePosition(Size.X, l.Position.X + l.Size.X), 3);
          var idY1 = Math.Round(_grid.YPartitioning.GetGridIndexFromAbsolutePosition(Size.Y, l.Position.Y), 3);
          var idY2 = Math.Round(_grid.YPartitioning.GetGridIndexFromAbsolutePosition(Size.Y, l.Position.Y + l.Size.Y), 3);

          l.Location = new ItemLocationByGrid() { GridColumn = idX1, GridColumnSpan = idX2 - idX1, GridRow = idY1, GridRowSpan = idY2 - idY1 };
        }
      }

      _grid.ParentObject = this;
      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Determines whether this layer is able to create a grid, so that a child layer with a given location fits into a grid cell.
    /// </summary>
    /// <param name="itemLocation">The item location of the child layer.</param>
    /// <returns><c>True</c> if this layer would be able to create a grid; <c>false otherwise.</c></returns>
    public bool CanCreateGridForLocation(ItemLocationDirect itemLocation)
    {
      if (Layers.Any((childLayer) => childLayer.Location is ItemLocationByGrid))
        return false;

      RectangleD2D enclosingRect = itemLocation.GetAbsoluteEnclosingRectangle();
      if (enclosingRect.Left < 0 || enclosingRect.Top < 0 || enclosingRect.Right > Size.X || enclosingRect.Bottom > Size.Y)
        return false;

      return true;
    }

    /// <summary>
    /// Creates the grid, so that a child layer with the location given by the argument <paramref name="itemLocation"/> fits into the grid at the same position as before.
    /// You should check with <see cref="CanCreateGridForLocation"/> whether it is possible to create a grid for the given item location.
    /// </summary>
    /// <param name="itemLocation">The item location of the child layer.</param>
    /// <returns>The new grid cell location for useage by the child layer. If no grid could be created, the return value may be <c>null</c>.</returns>
    public ItemLocationByGrid? CreateGridForLocation(ItemLocationDirect itemLocation)
    {
      bool isAnyChildLayerPosByGrid = Layers.Any((childLayer) => childLayer.Location is ItemLocationByGrid);

      if (!isAnyChildLayerPosByGrid)
      {
        RectangleD2D enclosingRect = itemLocation.GetAbsoluteEnclosingRectangle();

        if (enclosingRect.Left < 0 || enclosingRect.Top < 0 || enclosingRect.Right > Size.X || enclosingRect.Bottom > Size.Y)
          return null;

        _grid = new GridPartitioning();
        _grid.XPartitioning.Add(RADouble.NewRel(enclosingRect.Left / Size.X));
        _grid.XPartitioning.Add(RADouble.NewRel(enclosingRect.Width / Size.X));
        _grid.XPartitioning.Add(RADouble.NewRel(1 - enclosingRect.Right / Size.X));

        _grid.YPartitioning.Add(RADouble.NewRel(enclosingRect.Top / Size.Y));
        _grid.YPartitioning.Add(RADouble.NewRel(enclosingRect.Height / Size.Y));
        _grid.YPartitioning.Add(RADouble.NewRel(1 - enclosingRect.Bottom / Size.Y));
        _grid.ParentObject = this;

        var result = new ItemLocationByGrid();
        result.CopyFrom(itemLocation);
        result.ForceFitIntoCell = true;
        result.GridColumn = 1;
        result.GridColumnSpan = 1;
        result.GridRow = 1;
        result.GridRowSpan = 1;
        return result;
      }

      return null;
    }

    #endregion Grid creation

    #region XYPlotLayer properties and methods

    /// <summary>
    /// Gets the child layers of this layer.
    /// </summary>
    /// <value>
    /// The child layers.
    /// </value>
    public IList<HostLayer> Layers
    {
      get
      {
        return _childLayers;
      }
    }

    /// <summary>
    /// The layer number.
    /// </summary>
    /// <value>The layer number, i.e. the position of the layer in the layer collection.</value>
    public int Number
    {
      get
      {
        if (_parent is HostLayer hl)
        {
          var childLayers = hl._childLayers;
          for (int i = 0; i < childLayers.Count; ++i)
            if (object.ReferenceEquals(this, childLayers[i]))
              return i;
        }
        return 0;
      }
    }

    /// <summary>
    /// Gets the sibling layers of this layer including this layer itself.
    /// </summary>
    /// <value>
    /// The sibling layers (including this layer). <c>Null</c> is returned if this layer has no parent layer (thus no siblings exist).
    /// </value>
    public IObservableList<HostLayer>? SiblingLayers
    {
      get
      {
        var hl = _parent as HostLayer;
        return hl?._childLayers;
      }
    }

    public HostLayer? ParentLayer
    {
      get { return _parent as HostLayer; }
      set { ParentObject = value; }
    }

    public GraphicCollection GraphObjects
    {
      get { return _graphObjects; }
    }

    /// <summary>
    /// Initialize the graph objects collection internally.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">
    /// _graphObjects was already set!
    /// or
    /// _childLayers was already set!
    /// </exception>
    [MemberNotNull(nameof(_graphObjects), nameof(_childLayers))]
    private void InternalInitializeGraphObjectsCollection()
    {
      if (_graphObjects is not null)
        throw new InvalidOperationException("_graphObjects was already set!");
      if (_childLayers is not null)
        throw new InvalidOperationException("_childLayers was already set!");

      _graphObjects = new GraphicCollection(x => { x.ParentObject = this; x.SetParentSize(Size, false); });
      _graphObjects.CollectionChanged += EhGraphObjectCollectionChanged;

      _childLayers = _graphObjects.CreatePartialViewOfType<HostLayer>(EhBeforeInsertChildLayer);
      _childLayers.CollectionChanged += EhChildLayers_CollectionChanged;
      OnGraphObjectsCollectionInstanceInitialized();
    }

    private void EhGraphObjectCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Called after the instance of the <see cref="GraphicCollection"/> <see cref="GraphObjects"/> has been initialized.
    /// </summary>
    protected virtual void OnGraphObjectsCollectionInstanceInitialized()
    {
    }

    private void EhBeforeInsertChildLayer(HostLayer child)
    {
      child.ParentLayer = this;
      child.SetParentSize(_cachedLayerSize, true);
    }

    /// <summary>
    /// Get the index of this layer in the parent's layer collection.
    /// </summary>
    /// <value>
    /// The layer number.
    /// </value>
    public int LayerNumber { get { return _cachedLayerNumber; } }

    /// <summary>
    /// Is called by the parent layer if the index of this layer has changed.
    /// </summary>
    /// <param name="newLayerNumber">The new layer number. This number is cached in <see cref="HostLayer._cachedLayerNumber"/>.</param>
    protected virtual void OnLayerNumberChanged(int newLayerNumber)
    {
      _cachedLayerNumber = newLayerNumber;
    }

    private void EhChildLayers_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      for (int i = 0; i < _childLayers.Count; ++i)
      {
        if (i != _childLayers[i].LayerNumber)
        {
          _childLayers[i].OnLayerNumberChanged(i);
          _childLayers[i].EhSelfTunnelingEventHappened(Main.DocumentPathChangedEventArgs.Empty, true);
        }
      }

      if (LayerCollectionChanged is not null)
        LayerCollectionChanged(this, EventArgs.Empty);

      var pl = ParentLayer;
      if (pl is not null)
      {
        pl.EhChildLayers_CollectionChanged(sender, e); // DODO is this not an endless loop?
      }
    }

    public virtual void Remove(GraphicBase go)
    {
      if (_graphObjects.Contains(go))
      {
        if (_graphObjects.Remove(go))
        {
          go.Dispose();
        }
      }
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="Altaxo.Main.DocNodeProxy"/> instances to the visitor.</param>
    public virtual void VisitDocumentReferences(Altaxo.Main.DocNodeProxyReporter Report)
    {
      foreach (var hl in _childLayers)
        hl.VisitDocumentReferences(Report);
    }

    public virtual bool IsCompatibleWithParent(object parent)
    {
      return true;
    }

    #endregion XYPlotLayer properties and methods

    #region Painting and Hit testing

    /// <summary>
    /// Adjusts the internal data structures to ensure its validity.
    /// </summary>
    public virtual void FixupInternalDataStructures()
    {
      CalculateCachedSizeAndPosition();

      var mySize = Size;
      foreach (var graphObj in _graphObjects)
      {
        graphObj.SetParentSize(mySize, false);
        graphObj.FixupInternalDataStructures();
      }
    }

    /// <summary>
    /// This function is called by the graph document before _any_ layer is painted. We have to make sure that all of our cached data becomes valid.
    /// </summary>
    /// <param name="context">The paint context.</param>
    public virtual void PaintPreprocessing(IPaintContext context)
    {
      var mySize = Size;
      foreach (var graphObj in _graphObjects)
      {
        graphObj.PaintPreprocessing(context);
      }
    }

    /// <summary>
    /// This function is called when painting is finished. Can be used to release the resources
    /// not neccessary any more.
    /// </summary>
    public virtual void PaintPostprocessing()
    {
      foreach (var obj in _childLayers)
        obj.PaintPostprocessing();
    }

    public virtual void Paint(Graphics g, IPaintContext context)
    {
      GraphicsState savedgstate = g.Save();

      g.MultiplyTransform(_transformation);

      PaintInternal(g, context);

      g.Restore(savedgstate);
    }

    /// <summary>
    /// Internal Paint routine. The graphics state saving and transform is already done here!
    /// </summary>
    /// <param name="g">The graphics context</param>
    /// <param name="context">The paint context.</param>
    protected virtual void PaintInternal(Graphics g, IPaintContext context)
    {
      int len = _graphObjects.Count;
      for (int i = 0; i < len; i++)
      {
        _graphObjects[i].Paint(g, context);
      }
    }

    protected IHitTestObject ForwardTransform(IHitTestObject o)
    {
      o.Transform(_transformation);
      return o;
    }

    public virtual IHitTestObject? HitTest(HitTestPointData hitData)
    {
      return HitTest(hitData, false);
    }

    public virtual IHitTestObject? HitTest(HitTestPointData parentCoord, bool plotItemsOnly)
    {
      IHitTestObject? hit;

      //			HitTestPointData layerHitTestData = pageC.NewFromTranslationRotationScaleShear(Position.X, Position.Y, -Rotation, ScaleX, ScaleY, ShearX);
      HitTestPointData localCoord = parentCoord.NewFromAdditionalTransformation(_transformation);

      if (!plotItemsOnly)
      {
        // hit testing all graph objects, this is done in reverse order compared to the painting, so the "upper" items are found first.
        for (int i = _graphObjects.Count - 1; i >= 0; --i)
        {
          hit = _graphObjects[i].HitTest(localCoord);
          if (hit is not null)
          {
            if (hit.ParentLayer is null)
              hit.ParentLayer = this;

            if (hit.Remove is null && (hit.HittedObject is IGraphicBase))
              hit.Remove = new DoubleClickHandler(EhGraphicsObject_Remove);
            return ForwardTransform(hit);
          }
        }

        // first hit testing all four corners of the layer
        var layercorners = new GraphicsPath();
        float catchrange = 6;
        layercorners.AddEllipse(-catchrange, -catchrange, 2 * catchrange, 2 * catchrange);
        layercorners.AddEllipse((float)(_cachedLayerSize.X - catchrange), 0 - catchrange, 2 * catchrange, 2 * catchrange);
        layercorners.AddEllipse(0 - catchrange, (float)(_cachedLayerSize.Y - catchrange), 2 * catchrange, 2 * catchrange);
        layercorners.AddEllipse((float)(_cachedLayerSize.X - catchrange), (float)(_cachedLayerSize.Y - catchrange), 2 * catchrange, 2 * catchrange);
        layercorners.CloseAllFigures();

        var layerC = localCoord.GetHittedPointInWorldCoord();
        if (layercorners.IsVisible((PointF)layerC))
        {
          hit = new HitTestObject(layercorners, this)
          {
            DoubleClick = LayerPositionEditorMethod
          };
          return ForwardTransform(hit);
        }
      }
      else // Plot Items Only
      {
        // hit testing all graph objects, this is done in reverse order compared to the painting, so the "upper" items are found first.
        for (int i = _graphObjects.Count - 1; i >= 0; --i)
        {
          var layer = _graphObjects[i] as HostLayer;
          if (layer is null)
            continue;
          hit = layer.HitTest(localCoord, plotItemsOnly);
          if (hit is not null)
          {
            if (hit.ParentLayer is null)
              throw new InvalidProgramException("Parent layer must be set, because the hitted plot item originates from another layer!");
            return ForwardTransform(hit);
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Hit test of retangular hit test data against the layer (here treated as IGraphicBase object).
    /// Decision is here not to allow the layer itself to be selected by rectangular selection, instead we
    /// allow all subobjects to be selected.
    /// </summary>
    /// <param name="hitData">The hit data.</param>
    /// <returns>Here: null, because it was decided that the layer can not be selected by rectangular selection.</returns>
    public virtual IHitTestObject? HitTest(HitTestRectangularData hitData)
    {
      return null;
    }

    /// <summary>
    /// Collects all sub-objects of this layer that are selected by a rectangular selection area into a list.
    /// </summary>
    /// <param name="parentHitData">The hit data (having parent coordinates).</param>
    /// <param name="selectedObjects">List where to collect all selected objects.</param>
    public virtual void HitTest(HitTestRectangularData parentHitData, List<IHitTestObject> selectedObjects)
    {
      var localHitData = parentHitData.NewFromAdditionalTransformation(_transformation);

      // hit testing all graph objects, this is done in reverse order compared to the painting, so the "upper" items are found first.
      for (int i = _graphObjects.Count - 1; i >= 0; --i)
      {
        var hit = _graphObjects[i].HitTest(localHitData);
        if (hit is not null)
        {
          if (hit.ParentLayer is null)
            hit.ParentLayer = this;

          if (hit.Remove is null && (hit.HittedObject is IGraphicBase))
            hit.Remove = new DoubleClickHandler(EhGraphicsObject_Remove);

          hit.Transform(localHitData.Transformation); // ins Root-System transformieren
          selectedObjects.Add(hit);
        }
      }

      foreach (var childLayer in Layers)
      {
        childLayer.HitTest(localHitData, selectedObjects);
      }
    }

    private static bool EhGraphicsObject_Remove(IHitTestObject o)
    {
      if (o.ParentLayer is null)
        throw new InvalidProgramException($"Parent layer of hit test object not set!");
      var go = (IGraphicBase)o.HittedObject;
      o.ParentLayer.GraphObjects.Remove(go);
      return true;
    }

    #endregion Painting and Hit testing

    #region Editor methods

    public static DoubleClickHandler? LayerPositionEditorMethod;

    #endregion Editor methods

    #region Event firing

    /// <summary>
    /// Called when the resulting size of this layer has changed. Is intended to inform child layers and own dependend objects of the size change.
    /// Because it is only the cached size, it will not raise changed events. Those events must be raised in the function that caused the change of the resulting size.
    /// </summary>
    protected virtual void OnCachedResultingSizeChanged()
    {
      // first inform our childs
      if (_childLayers is not null)
      {
        foreach (var layer in _childLayers)
          layer.SetParentSize(Size, false); // Do not raise change events here, it is only the cached size that changed
      }

      // now inform other listeners
      if (SizeChanged is not null)
        SizeChanged(this, new System.EventArgs());
    }

    /// <summary>
    /// Called when the resulting position of this layer has changed. Is intended to inform child layers and own dependend objects of the position change.
    /// Because it is only the cached position, it will not raise changed events. Those events must be raised in the function that caused the change of the resulting position.
    /// </summary>
    protected void OnCachedResultingPositionChanged()
    {
      if (PositionChanged is not null)
        PositionChanged(this, new System.EventArgs());
    }

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (sender is IItemLocation)
        CalculateCachedSizeAndPosition();

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Event firing

    #region IDocumentNode Members

    public override Main.IDocumentNode? ParentObject
    {
      get
      {
        return _parent;
      }
      set
      {
        var oldValue = _parent;
        _parent = value;
        if (!object.ReferenceEquals(oldValue, value))
        {
          CalculateCachedSizeAndPosition();
        }
      }
    }

    /// <summary>
    /// Returns the document name of the layer at index i. Actually, this is a name of the form L0, L1, L2 and so on.
    /// </summary>
    /// <param name="layerIndex">The layer index.</param>
    /// <returns>The name of the layer at index i.</returns>
    public static string GetDefaultNameOfLayer(IList<int> layerIndex)
    {
      if (layerIndex.Count == 0)
        return "RL";

      var stb = new System.Text.StringBuilder();

      stb.AppendFormat("L{0}", layerIndex[0]);

      for (int k = 1; k < layerIndex.Count; ++k)

        stb.AppendFormat("-{0}", layerIndex[k]);

      return stb.ToString();
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      // despite the fact that _childLayers is only a partial view of _graphObjects, we use it here because if it is found here, it is never searched for in _graphObjects
      // note also that Disposed is overridden, so that we not use this function for dispose purposes
      if (_childLayers is not null)
      {
        for (int i = 0; i < _childLayers.Count; ++i)
        {
          yield return new Main.DocumentNodeAndName(_childLayers[i], "Layer" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
      }

      if (_graphObjects is not null)
      {
        for (int i = 0; i < _graphObjects.Count; ++i)
        {
          if (_graphObjects[i] is not null)
            yield return new Main.DocumentNodeAndName(_graphObjects[i], "GraphObject" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
      }

      if (_location is not null)
      {
        yield return new Main.DocumentNodeAndName(_location, "Location");
      }

      if (_grid is not null)
      {
        yield return new Main.DocumentNodeAndName(_grid, "Grid");
      }
    }

    protected override void Dispose(bool isDisposing)
    {
      if (_graphObjects is not null)
      {
        var graphObjects = _graphObjects;
        _graphObjects = null!;
        for (int i = 0; i < graphObjects.Count; ++i)
        {
          if (graphObjects[i] is not null)
            graphObjects[i].Dispose();
        }
      }

      ChildDisposeMember(ref _location!);
      ChildDisposeMember(ref _grid!);

      base.Dispose(isDisposing);
    }

    public virtual bool FixAndTestParentChildRelationShipOfLayers()
    {
      return this.FixAndTestParentChildRelations((l, p) => l.ParentLayer = p);
    }

    #endregion IDocumentNode Members

    #region Enumeration through layers

    /// <summary>
    /// Executes an action on each child layer, including this layer, beginning with the topmost child (the first child of the first child of...).
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public void ExecuteFromTopmostChildToRoot(Action<HostLayer> action)
    {
      ExecuteFromTopmostChildToRoot(this, action);
    }

    private static void ExecuteFromTopmostChildToRoot(HostLayer start, Action<HostLayer> action)
    {
      foreach (var l in start._childLayers)
      {
        ExecuteFromTopmostChildToRoot(l, action);
      }
      action(start);
    }

    #endregion Enumeration through layers

    #region ITreeListNodeWithParent implementation

    IList<HostLayer> ITreeListNode<HostLayer>.ChildNodes
    {
      get { return _childLayers; }
    }

    IEnumerable<HostLayer> ITreeNode<HostLayer>.ChildNodes
    {
      get { return _childLayers; }
    }

    Main.IDocumentLeafNode? INodeWithParentNode<Main.IDocumentLeafNode>.ParentNode
    {
      get { return _parent; }
    }

    HostLayer? INodeWithParentNode<HostLayer>.ParentNode
    {
      get { return _parent as HostLayer; }
    }

    #endregion ITreeListNodeWithParent implementation
  }
}
