#region Copyright

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
using System.Linq;
using Altaxo.Collections;
using Altaxo.Geometry;
using Altaxo.Main;

namespace Altaxo.Graph.Graph3D
{
  using System.Diagnostics.CodeAnalysis;
  using GraphicsContext;
  using Shapes;

  /// <summary>
  /// Represents a host layer that can contain graphic objects and child layers in a three-dimensional graph.
  /// </summary>
  public class HostLayer :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    ITreeListNodeWithParent<HostLayer>,
    IGraphicBase,
    Altaxo.Main.INamedObjectCollection
  {
    #region Constants

    /// <summary>
    /// Default relative x size for child layers.
    /// </summary>
    protected const double _xDefSizeLandscape = 88 / 128.0;
    /// <summary>
    /// Default relative y size for child layers.
    /// </summary>
    protected const double _yDefSizeLandscape = 88 / 96.0;
    /// <summary>
    /// Default relative z size for child layers.
    /// </summary>
    protected const double _zDefSizeLandscape = 72 / 96.0;

    /// <summary>
    /// Default relative x position for child layers.
    /// </summary>
    protected const double _xDefPositionLandscape = 0.5 * (1 - 88 / 128.0);
    /// <summary>
    /// Default relative y position for child layers.
    /// </summary>
    protected const double _yDefPositionLandscape = 0.5 * (1 - 88 / 96.0);
    /// <summary>
    /// Default relative z position for child layers.
    /// </summary>
    protected const double _zDefPositionLandscape = 0.2;

    #endregion Constants

    #region Cached member variables

    /// <summary>
    /// The cached size of the parent layer. If this here is the root layer, and hence no parent layer exist, the cached size is set to 100 x 100 mm².
    /// </summary>
    protected VectorD3D _cachedParentLayerSize = new VectorD3D((1000 * 72) / 254.0, (1000 * 72) / 254.0, (1000 * 72) / 254.0);

    /// <summary>
    /// The cached layer position in points (1/72 inch) relative to the upper left corner
    /// of the parent layer (upper left corner of the printable area).
    /// </summary>
    protected PointD3D _cachedLayerPosition;

    /// <summary>
    /// The absolute size of the layer in points (1/72 inch).
    /// </summary>
    protected VectorD3D _cachedLayerSize;

    /// <summary>
    /// Stores the local transformation from parent coordinates into this layer.
    /// </summary>
    protected Matrix4x3 _transformation = Matrix4x3.Identity;

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

    /// <summary>
    /// Stores the location object controlling position, size, and transformation.
    /// </summary>
    protected IItemLocation _location;

    /// <summary>
    /// Stores the graphical objects contained in this layer.
    /// </summary>
    protected GraphicCollection _graphObjects;

    /// <summary>
    /// Defines a grid that child layers can use to arrange.
    /// </summary>
    private GridPartitioning _grid;

    #endregion Member variables

    #region Editor methods

    /// <summary>
    /// Gets or sets the editor method used to edit the layer position.
    /// </summary>
    public static DoubleClickHandler? LayerPositionEditorMethod;

    #endregion Editor methods

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
    /// 2015-09-10 initial version.
    /// </summary>
    /// <summary>
    /// Serializes <see cref="HostLayer"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HostLayer), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (HostLayer)o;

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

        s.ParentObject = parent as Main.IDocumentNode;
        // size, position, rotation and scale
        s._cachedParentLayerSize = (VectorD3D)info.GetValue("CachedParentSize", s);
        s._cachedLayerSize = (VectorD3D)info.GetValue("CachedSize", s);
        s._cachedLayerPosition = (PointD3D)info.GetValue("CachedPosition", s);
        s.Location = (IItemLocation)info.GetValue("LocationAndSize", s);
        s.Grid = (GridPartitioning)info.GetValue("Grid", s);

        // Graphic objects
        s.GraphObjects.AddRange((IEnumerable<IGraphicBase>)info.GetValue("GraphObjects", s));

        return s;
      }

      /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is HostLayer from)
      {
        CopyFrom(from, Altaxo.Graph.Gdi.GraphCopyOptions.All);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Copies the contents of another host layer using the specified copy options.
    /// </summary>
    /// <param name="from">The host layer to copy from.</param>
    /// <param name="options">The copy options.</param>
    public virtual void CopyFrom(HostLayer from, Altaxo.Graph.Gdi.GraphCopyOptions options)
    {
      if (ReferenceEquals(this, from))
        return;

      using (var suspendToken = SuspendGetToken())
      {
        InternalCopyFrom(from, options);
        EhSelfChanged(EventArgs.Empty);  // make sure that change is called after suspend
      }
    }

    /// <summary>
    /// Performs the internal copy operation while change notifications are suspended.
    /// </summary>
    /// <param name="from">The host layer to copy from.</param>
    /// <param name="options">The copy options.</param>
    [MemberNotNull(nameof(_grid), nameof(_location))]
    protected virtual void InternalCopyFrom(HostLayer from, Altaxo.Graph.Gdi.GraphCopyOptions options)
    {
      if (_parent is null)
      {
        //this._parent = from._parent; // necessary in order to set Location to GridLocation, where a parent layer is required
        _cachedLayerNumber = from._cachedLayerNumber; // is important when the layer dialog is open: this number must be identical to that of the cloned layer
      }

      ChildCopyToMember(ref _grid, from._grid);

      // size, position, rotation and scale
      if ((0 != (options & Altaxo.Graph.Gdi.GraphCopyOptions.CopyLayerSizePosition)) ||
        (_location is null)) // location is null during construction
      {
        _cachedLayerSize = from._cachedLayerSize;
        _cachedLayerPosition = from._cachedLayerPosition;
        _cachedParentLayerSize = from._cachedParentLayerSize;
        ChildCopyToMember(ref _location, from._location);
      }

      InternalCopyGraphItems(from, options);

      // copy the properties in the child layer(s) (only the members, not the child layers itself)
      if (0 != (options & Altaxo.Graph.Gdi.GraphCopyOptions.CopyLayerAll))
      {
        // not all properties of the child layers should be cloned -> just copy the layers one by one
        int len = Math.Min(_childLayers.Count, from._childLayers.Count);
        for (int i = 0; i < len; i++)
        {
          _childLayers[i].CopyFrom(from._childLayers[i], options);
          _childLayers[i].ParentLayer = this;
        }
      }

      CalculateMatrix();
    }

    /// <summary>
    /// Copies the graphic items of another host layer according to the specified copy options.
    /// </summary>
    /// <param name="from">The host layer to copy from.</param>
    /// <param name="options">The copy options.</param>
    protected virtual void InternalCopyGraphItems(HostLayer from, Altaxo.Graph.Gdi.GraphCopyOptions options)
    {
      bool bGraphItems = options.HasFlag(Altaxo.Graph.Gdi.GraphCopyOptions.CopyLayerGraphItems);
      bool bChildLayers = options.HasFlag(Altaxo.Graph.Gdi.GraphCopyOptions.CopyChildLayers);

      var criterium = new Func<IGraphicBase, bool>(x =>
      {
        if (x is HostLayer)
          return bChildLayers;

        return bGraphItems;
      });

      InternalCopyGraphItems(from, options, criterium);
    }

    /// <summary>
    /// Copies compatible graph items from another layer into this layer.
    /// </summary>
    /// <param name="from">The source layer.</param>
    /// <param name="options">The copy options.</param>
    /// <param name="selectionCriteria">The selection criteria for graph items.</param>
    protected virtual void InternalCopyGraphItems(HostLayer from, Altaxo.Graph.Gdi.GraphCopyOptions options, Func<IGraphicBase, bool> selectionCriteria)
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

    /// <summary>
    /// Creates a copy of this layer.
    /// </summary>
    /// <returns>A cloned host layer.</returns>
    public virtual object Clone()
    {
      return new HostLayer(this);
    }

    #endregion Copying

    /// <summary>
    /// Constructor for deserialization purposes only.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
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
    /// <param name="from">The layer to copy from.</param>
    public HostLayer(HostLayer from)
    {
      Grid = new GridPartitioning();

      using (var suspendToken = SuspendGetToken()) // see below, this is to suppress the change event when cloning the layer.
      {
        InternalInitializeGraphObjectsCollection(); // Preparation of graph objects collection and its partial views
        InternalCopyFrom(from, Altaxo.Graph.Gdi.GraphCopyOptions.All);

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

    /// <summary>
    /// Initializes a new instance of the <see cref="HostLayer"/> class.
    /// </summary>
    public HostLayer()
      : this(null, new ItemLocationDirect())
    {
    }

    #endregion Constructors

    #region Grid creation

    /// <summary>
    /// Gets the grid used to arrange child layers.
    /// </summary>
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
    public void CreateDefaultGrid()
    {
      _grid = new GridPartitioning();
      _grid.XPartitioning.Add(RADouble.NewRel(DefaultChildLayerRelativePosition.X));
      _grid.XPartitioning.Add(RADouble.NewRel(DefaultChildLayerRelativeSize.X));
      _grid.XPartitioning.Add(RADouble.NewRel(1 - DefaultChildLayerRelativePosition.X - DefaultChildLayerRelativeSize.X));

      _grid.YPartitioning.Add(RADouble.NewRel(DefaultChildLayerRelativePosition.Y));
      _grid.YPartitioning.Add(RADouble.NewRel(DefaultChildLayerRelativeSize.Y));
      _grid.YPartitioning.Add(RADouble.NewRel(1 - DefaultChildLayerRelativePosition.Y - DefaultChildLayerRelativeSize.Y));

      _grid.ZPartitioning.Add(RADouble.NewRel(DefaultChildLayerRelativePosition.Z));
      _grid.ZPartitioning.Add(RADouble.NewRel(DefaultChildLayerRelativeSize.Z));
      _grid.ZPartitioning.Add(RADouble.NewRel(1 - DefaultChildLayerRelativePosition.Z - DefaultChildLayerRelativeSize.Z));
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
      var zPositions = new HashSet<double>();

      // Take only those positions into account, that are inside this layer

      foreach (var l in Layers)
      {
        xPositions.Add(RoundToFractions(l.Position.X / Size.X, RelValueRoundFraction));
        xPositions.Add(RoundToFractions((l.Position.X + l.Size.X) / Size.X, RelValueRoundFraction));
        yPositions.Add(RoundToFractions(l.Position.Y / Size.Y, RelValueRoundFraction));
        yPositions.Add(RoundToFractions((l.Position.Y + l.Size.Y) / Size.Y, RelValueRoundFraction));
        zPositions.Add(RoundToFractions(l.Position.Z / Size.Z, RelValueRoundFraction));
        zPositions.Add(RoundToFractions((l.Position.Z + l.Size.Z) / Size.Z, RelValueRoundFraction));
      }

      xPositions.Add(1);
      yPositions.Add(1);

      var xPosPurified = new SortedSet<double>(xPositions.Where(x => x >= 0 && x <= 1));
      var yPosPurified = new SortedSet<double>(yPositions.Where(y => y >= 0 && y <= 1));
      var zPosPurified = new SortedSet<double>(zPositions.Where(z => z >= 0 && z <= 1));

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
      prev = 0;
      foreach (var z in zPosPurified)
      {
        _grid.ZPartitioning.Add(RADouble.NewRel(z - prev));
        prev = z;
      }

      // ensure that we always have an odd number of columns and rows
      // if there is no child layer present, then at least one row and one column should be present
      if (0 == _grid.XPartitioning.Count % 2)
        _grid.XPartitioning.Add(RADouble.NewRel(_grid.XPartitioning.Count == 0 ? 1 : 0));
      if (0 == _grid.YPartitioning.Count % 2)
        _grid.YPartitioning.Add(RADouble.NewRel(_grid.YPartitioning.Count == 0 ? 1 : 0));
      if (0 == _grid.ZPartitioning.Count % 2)
        _grid.ZPartitioning.Add(RADouble.NewRel(_grid.ZPartitioning.Count == 0 ? 1 : 0));

      foreach (var l in Layers)
      {
        if (!(l.Location is ItemLocationByGrid))
        {
          var idX1 = Math.Round(_grid.XPartitioning.GetGridIndexFromAbsolutePosition(Size.X, l.Position.X), 3);
          var idX2 = Math.Round(_grid.XPartitioning.GetGridIndexFromAbsolutePosition(Size.X, l.Position.X + l.Size.X), 3);
          var idY1 = Math.Round(_grid.YPartitioning.GetGridIndexFromAbsolutePosition(Size.Y, l.Position.Y), 3);
          var idY2 = Math.Round(_grid.YPartitioning.GetGridIndexFromAbsolutePosition(Size.Y, l.Position.Y + l.Size.Y), 3);
          var idZ1 = Math.Round(_grid.ZPartitioning.GetGridIndexFromAbsolutePosition(Size.Z, l.Position.Z), 3);
          var idZ2 = Math.Round(_grid.ZPartitioning.GetGridIndexFromAbsolutePosition(Size.Z, l.Position.Z + l.Size.Z), 3);

          l.Location = new ItemLocationByGrid() { GridPosX = idX1, GridSpanX = idX2 - idX1, GridPosY = idY1, GridSpanY = idY2 - idY1, GridPosZ = idZ1, GridSpanZ = idZ2 - idZ1 };
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

      RectangleD3D enclosingRect = itemLocation.GetAbsoluteEnclosingRectangle();
      if (enclosingRect.X < 0 || enclosingRect.Y < 0 || enclosingRect.Z < 0 || enclosingRect.XPlusSizeX > Size.X || enclosingRect.YPlusSizeY > Size.Y || enclosingRect.ZPlusSizeZ > Size.Z)
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
        RectangleD3D enclosingRect = itemLocation.GetAbsoluteEnclosingRectangle();

        if (enclosingRect.X < 0 || enclosingRect.Y < 0 || enclosingRect.Z < 0 || enclosingRect.XPlusSizeX > Size.X || enclosingRect.YPlusSizeY > Size.Y || enclosingRect.ZPlusSizeZ > Size.Z)
          return null;

        _grid = new GridPartitioning();
        _grid.XPartitioning.Add(RADouble.NewRel(enclosingRect.X / Size.X));
        _grid.XPartitioning.Add(RADouble.NewRel(enclosingRect.SizeX / Size.X));
        _grid.XPartitioning.Add(RADouble.NewRel(1 - enclosingRect.XPlusSizeX / Size.X));

        _grid.YPartitioning.Add(RADouble.NewRel(enclosingRect.Y / Size.Y));
        _grid.YPartitioning.Add(RADouble.NewRel(enclosingRect.SizeY / Size.Y));
        _grid.YPartitioning.Add(RADouble.NewRel(1 - enclosingRect.YPlusSizeY / Size.Y));

        _grid.ZPartitioning.Add(RADouble.NewRel(enclosingRect.Z / Size.Z));
        _grid.ZPartitioning.Add(RADouble.NewRel(enclosingRect.SizeZ / Size.Z));
        _grid.ZPartitioning.Add(RADouble.NewRel(1 - enclosingRect.ZPlusSizeZ / Size.Z));

        _grid.ParentObject = this;

        var result = new ItemLocationByGrid();
        result.CopyFrom(itemLocation);
        result.ForceFitIntoCell = true;
        result.GridPosX = 1;
        result.GridSpanX = 1;
        result.GridPosY = 1;
        result.GridSpanY = 1;
        result.GridPosZ = 1;
        result.GridSpanZ = 1;
        return result;
      }

      return null;
    }

    #endregion Grid creation

    /// <inheritdoc/>
    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
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

    /// <inheritdoc/>
    IList<HostLayer> ITreeListNode<HostLayer>.ChildNodes
    {
      get { return _childLayers; }
    }

    /// <inheritdoc/>
    IEnumerable<HostLayer> ITreeNode<HostLayer>.ChildNodes
    {
      get { return _childLayers; }
    }

    /// <inheritdoc/>
    Main.IDocumentLeafNode? INodeWithParentNode<Main.IDocumentLeafNode>.ParentNode
    {
      get { return _parent; }
    }

    /// <inheritdoc/>
    HostLayer? INodeWithParentNode<HostLayer>.ParentNode
    {
      get { return _parent as HostLayer; }
    }

    #endregion ITreeListNodeWithParent implementation

    #region IGraphicBase3D

    /// <summary>
    /// Updates cached geometry and propagates parent size to child graph objects.
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
    /// Performs preprocessing for all contained graph objects.
    /// </summary>
    /// <param name="context">The paint context.</param>
    public virtual void PaintPreprocessing(Altaxo.Graph.IPaintContext context)
    {
      var mySize = Size;
      foreach (var graphObj in _graphObjects)
      {
        graphObj.PaintPreprocessing(context);
      }
    }

    /// <summary>
    /// Paints this layer and its graph objects.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="context">The paint context.</param>
    public virtual void Paint(IGraphicsContext3D g, IPaintContext context)
    {
      var savedgstate = g.SaveGraphicsState();

      g.PrependTransform(_transformation);

      PaintInternal(g, context);

      g.RestoreGraphicsState(savedgstate);
    }

    /// <summary>
    /// Internal Paint routine. The graphics state saving and transform is already done here!
    /// </summary>
    /// <param name="g">The graphics context</param>
    /// <param name="context">The paint context.</param>
    protected virtual void PaintInternal(IGraphicsContext3D g, IPaintContext context)
    {
      int len = _graphObjects.Count;
      for (int i = 0; i < len; i++)
      {
        _graphObjects[i].Paint(g, context);
      }
    }

    /// <summary>
    /// Performs cleanup after painting.
    /// </summary>
    public virtual void PaintPostprocessing()
    {
    }

    /// <summary>
    /// Determines whether this layer can be inserted into the specified parent object.
    /// </summary>
    /// <param name="parentObject">The candidate parent object.</param>
    /// <returns><see langword="true"/> if the parent is supported; otherwise, <see langword="false"/>.</returns>
    public bool IsCompatibleWithParent(object parentObject)
    {
      return true;
    }

    #endregion IGraphicBase3D

    #region Position and Size

    /// <summary>
    /// Gets the default relative position for child layers.
    /// </summary>
    public static VectorD3D DefaultChildLayerRelativePosition
    {
      get { return new VectorD3D(_xDefPositionLandscape, _yDefPositionLandscape, _zDefPositionLandscape); }
    }

    /// <summary>
    /// Gets the default child layer position in points (1/72 inch).
    /// </summary>
    /// <value>The default position of a (new) layer in points (1/72 inch).</value>
    public PointD3D DefaultChildLayerPosition
    {
      get { return (PointD3D)VectorD3D.MultiplicationElementwise(DefaultChildLayerRelativePosition, Size); }
    }

    /// <summary>
    /// Gets the default relative size for child layers.
    /// </summary>
    public static VectorD3D DefaultChildLayerRelativeSize
    {
      get { return new VectorD3D(_xDefSizeLandscape, _yDefSizeLandscape, _zDefSizeLandscape); }
    }

    /// <summary>
    /// Gets the default child layer size in points (1/72 inch).
    /// </summary>
    /// <value>The default size of a (new) layer in points (1/72 inch).</value>
    public VectorD3D DefaultChildLayerSize
    {
      get { return VectorD3D.MultiplicationElementwise(DefaultChildLayerRelativeSize, Size); }
    }

    /// <summary>
    /// Creates the default location used for newly created child layers.
    /// </summary>
    /// <returns>A new item location configured with the default relative size and position.</returns>
    public static IItemLocation GetChildLayerDefaultLocation()
    {
      return new ItemLocationDirect
      {
        SizeX = RADouble.NewRel(HostLayer.DefaultChildLayerRelativeSize.X),
        SizeY = RADouble.NewRel(HostLayer.DefaultChildLayerRelativeSize.Y),
        SizeZ = RADouble.NewRel(HostLayer.DefaultChildLayerRelativeSize.Z),
        PositionX = RADouble.NewRel(HostLayer.DefaultChildLayerRelativePosition.X),
        PositionY = RADouble.NewRel(HostLayer.DefaultChildLayerRelativePosition.Y),
        PositionZ = RADouble.NewRel(HostLayer.DefaultChildLayerRelativePosition.Z)
      };
    }

    /// <summary>
    /// Gets or sets the location object of this layer.
    /// </summary>
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
          if (_location is ItemLocationDirect)
            ((ItemLocationDirect)_location).SetParentSize(_cachedParentLayerSize, false);

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
    public void SizeToDefault(VectorD3D parentSize)
    {
      Size = new VectorD3D(parentSize.X * _xDefSizeLandscape, parentSize.Y * _yDefSizeLandscape, parentSize.Z * _zDefSizeLandscape);
      Position = new PointD3D(parentSize.X * _xDefPositionLandscape, parentSize.Y * _yDefPositionLandscape, parentSize.Z * _zDefPositionLandscape);

      CalculateMatrix();
    }

    /// <summary>
    /// The boundaries of the printable area of the page in points (1/72 inch).
    /// </summary>
    public VectorD3D ParentLayerSize
    {
      get { return _cachedParentLayerSize; }
    }

    /// <summary>
    /// Updates the cached parent size for this layer.
    /// </summary>
    /// <param name="parentSize">The new parent size.</param>
    /// <param name="isTriggeringChangedEvent">If set to <see langword="true"/>, a changed event is raised when the size changes.</param>
    public void SetParentSize(VectorD3D parentSize, bool isTriggeringChangedEvent)
    {
      var oldParentSize = _cachedParentLayerSize;
      _cachedParentLayerSize = parentSize;

      if (_location is ItemLocationDirect)
        ((ItemLocationDirect)_location).SetParentSize(_cachedParentLayerSize, false); // don't trigger change event now

      if (oldParentSize != parentSize)
      {
        CalculateCachedSizeAndPosition();

        if (isTriggeringChangedEvent)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets or sets the cached absolute position of the layer.
    /// </summary>
    public PointD3D Position
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

          if (ls.PositionZ.IsAbsolute)
            ls.PositionZ = RADouble.NewAbs(value.Z);
          else
            ls.PositionZ = RADouble.NewRel(value.Z / _cachedParentLayerSize.Z);
        }
      }
    }

    /// <summary>
    /// Gets or sets the cached absolute size of the layer.
    /// </summary>
    public VectorD3D Size
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

          if (ls.SizeZ.IsAbsolute)
            ls.SizeZ = RADouble.NewAbs(value.Z);
          else
            ls.SizeZ = RADouble.NewRel(value.Z / _cachedParentLayerSize.Z);
        }
      }
    }

    /// <summary>
    /// Gets or sets the x-axis rotation of the layer.
    /// </summary>
    public double RotationX
    {
      get { return _location.RotationX; }
      set
      {
        var oldValue = _location.RotationX;
        _location.RotationX = value;

        if (value != oldValue)
        {
          CalculateMatrix();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the y-axis rotation of the layer.
    /// </summary>
    public double RotationY
    {
      get { return _location.RotationY; }
      set
      {
        var oldValue = _location.RotationY;
        _location.RotationY = value;

        if (value != oldValue)
        {
          CalculateMatrix();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the z-axis rotation of the layer.
    /// </summary>
    public double RotationZ
    {
      get { return _location.RotationZ; }
      set
      {
        var oldValue = _location.RotationZ;
        _location.RotationZ = value;

        if (value != oldValue)
        {
          CalculateMatrix();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the x-axis shear of the layer.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the y-axis shear of the layer.
    /// </summary>
    public double ShearY
    {
      get { return _location.ShearY; }
      set
      {
        var oldValue = _location.ShearY;
        _location.ShearY = value;

        if (value != oldValue)
        {
          CalculateMatrix();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the z-axis shear of the layer.
    /// </summary>
    public double ShearZ
    {
      get { return _location.ShearZ; }
      set
      {
        var oldValue = _location.ShearZ;
        _location.ShearZ = value;

        if (value != oldValue)
        {
          CalculateMatrix();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the x-axis scale factor of the layer.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the y-axis scale factor of the layer.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the z-axis scale factor of the layer.
    /// </summary>
    public double ScaleZ
    {
      get { return _location.ScaleZ; }
      set
      {
        var oldValue = _location.ScaleZ;
        _location.ScaleZ = value;

        if (value != oldValue)
        {
          CalculateMatrix();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Recalculates the transformation matrix from the current location data.
    /// </summary>
    protected void CalculateMatrix()
    {
      if (_location is ItemLocationDirect)
      {
        var locD = (ItemLocationDirect)_location;
        _transformation = Matrix4x3.FromScaleShearRotationDegreeTranslation(
          locD.ScaleX, locD.ScaleY, locD.ScaleZ,
          locD.ShearX, locD.ShearY, locD.ShearZ,
          locD.RotationX, locD.RotationY, locD.RotationZ,
          locD.AbsolutePivotPositionX, locD.AbsolutePivotPositionY, locD.AbsolutePivotPositionZ);
        _transformation.TranslatePrepend(locD.AbsoluteVectorPivotToLeftUpper.X, locD.AbsoluteVectorPivotToLeftUpper.Y, locD.AbsoluteVectorPivotToLeftUpper.Z);
      }
      else
      {
        _transformation = Matrix4x3.FromScaleShearRotationDegreeTranslation(
          _location.ScaleX, _location.ScaleY, _location.ScaleZ,
          _location.ShearX, _location.ShearY, _location.ShearZ,
          _location.RotationX, _location.RotationY, _location.RotationZ,
          _cachedLayerPosition.X, _cachedLayerPosition.Y, _cachedLayerPosition.Z);
      }
    }

    /// <summary>
    /// Transforms coordinates from the parent layer into this layer.
    /// </summary>
    /// <param name="pagecoordinates">The coordinates in parent space.</param>
    /// <returns>The coordinates in local layer space.</returns>
    public PointD3D TransformCoordinatesFromParentToHere(PointD3D pagecoordinates)
    {
      return _transformation.InverseTransform(pagecoordinates);
    }

    /// <summary>
    /// Transforms coordinates from the root layer into this layer.
    /// </summary>
    /// <param name="pagecoordinates">The coordinates in root space.</param>
    /// <returns>The coordinates in local layer space.</returns>
    public PointD3D TransformCoordinatesFromRootToHere(PointD3D pagecoordinates)
    {
      foreach (var layer in this.TakeFromRootToHere())
        pagecoordinates = layer._transformation.InverseTransform(pagecoordinates);
      return pagecoordinates;
    }

    /// <summary>
    /// Gets the transformation matrix from the root layer into this layer.
    /// </summary>
    /// <returns>The accumulated transformation matrix.</returns>
    public Matrix4x3 TransformationFromRootToHere()
    {
      Matrix4x3 result = Matrix4x3.Identity;
      foreach (var layer in this.TakeFromRootToHere())
        result.PrependTransform(layer._transformation);
      return result;
    }

    /// <summary>
    /// Gets the transformation matrix from this layer to the root layer.
    /// </summary>
    /// <returns>The accumulated transformation matrix.</returns>
    public Matrix4x3 TransformationFromHereToRoot()
    {
      Matrix4x3 result = Matrix4x3.Identity;
      foreach (var layer in this.TakeFromHereToRoot())
        result = result.WithAppendedTransformation(layer._transformation);
      return result;
    }

    /// <summary>
    /// Converts X,Y differences in page units to X,Y differences in layer units
    /// </summary>
    /// <param name="pagediff">X,Y coordinate differences in graph units</param>
    /// <returns>the convertes X,Y coordinate differences in layer units</returns>
    public VectorD3D TransformCoordinateDifferencesFromParentToHere(VectorD3D pagediff)
    {
      return _transformation.InverseTransform(pagediff);
    }

    /// <summary>
    /// Transforms a <see cref="PointD2D" /> from layer coordinates to graph (=printable area) coordinates
    /// </summary>
    /// <param name="layerCoordinates">The layer coordinates to convert.</param>
    /// <returns>graphics path now in graph coordinates</returns>
    public PointD3D TransformCoordinatesFromHereToParent(PointD3D layerCoordinates)
    {
      return _transformation.Transform(layerCoordinates);
    }

    /// <summary>
    /// Transforms coordinates from this layer into the root layer.
    /// </summary>
    /// <param name="coordinates">The coordinates in local layer space.</param>
    /// <returns>The coordinates in root space.</returns>
    public PointD3D TransformCoordinatesFromHereToRoot(PointD3D coordinates)
    {
      foreach (var layer in this.TakeFromHereToRoot())
        coordinates = layer._transformation.Transform(coordinates);
      return coordinates;
    }

    /// <summary>
    /// Sets the position and size values stored in the current location object.
    /// </summary>
    /// <param name="x">The x-position value.</param>
    /// <param name="y">The y-position value.</param>
    /// <param name="z">The z-position value.</param>
    /// <param name="width">The width value.</param>
    /// <param name="height">The height value.</param>
    /// <param name="sizeZ">The depth value.</param>
    public void SetPositionSize(RADouble x, RADouble y, RADouble z, RADouble width, RADouble height, RADouble sizeZ)
    {
      ItemLocationDirect newlocation;

      if (!(_location is ItemLocationDirect))
        newlocation = new ItemLocationDirect(_location);
      else
        newlocation = (ItemLocationDirect)Location;

      newlocation.SetPositionAndSize(x, y, z, width, height, sizeZ);

      Location = newlocation;
    }

    /// <summary>
    /// Sets the cached size value in <see cref="_cachedLayerSize"/> by calculating it
    /// from the position values (<see cref="_location"/>.Width and .Height)
    /// and the size types (<see cref="_location"/>.WidthType and .HeightType).
    /// </summary>
    [MemberNotNull(nameof(_cachedLayerSize), nameof(_cachedLayerPosition))]
    protected void CalculateCachedSizeAndPosition()
    {
      RectangleD3D newRect;

      if (_location is null)
      {
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return; // location is only null during deserialization
#pragma warning restore CS8774 // Member must have a non-null value when exiting.
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

          /*
                    if (gps.ForceFitIntoCell)
                    {
                        var t = MatrixD3D.FromTranslationRotationShearScale(
                        0, 0, 0,
                        -this.RotationX, -this.RotationY, -this.RotationZ,
                        this.ShearX, this.ShearY, this.ShearZ,
                        this.ScaleX, this.ScaleY, this.ScaleZ);
                        var ele = t.Elements;
                        newRect = RectangleExtensions.GetIncludedTransformedRectangle(gridRect, t.SX, t.RX, t.RY, t.SY);
                    }
                    */
        }
        else // ParentLayer is null, this is probably the root layer, thus use the _cachedParentLayersSize
        {
          newRect = new RectangleD3D(0, 0, 0, _cachedParentLayerSize.X, _cachedParentLayerSize.Y, _cachedParentLayerSize.Z);
        }
      }
      else
      {
        throw new NotImplementedException(string.Format("Unknown location type: _location is {0}", _location));
      }

      bool isPositionChanged = newRect.Location != _cachedLayerPosition;
      bool isSizeChanged = newRect.Size != _cachedLayerSize;

      _cachedLayerSize = newRect.Size;
      _cachedLayerPosition = newRect.Location;
      CalculateMatrix();

      if (isSizeChanged)
        OnCachedResultingSizeChanged();

      if (isPositionChanged)
        OnCachedResultingPositionChanged();
    }

    #endregion Position and Size

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

    /// <summary>
    /// Handles child changes that require cached geometry updates before normal processing.
    /// </summary>
    /// <param name="sender">The child that raised the change.</param>
    /// <param name="e">The event arguments.</param>
    /// <returns>The result from the base implementation.</returns>
    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (sender is IItemLocation)
        CalculateCachedSizeAndPosition();

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Event firing

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
            if (ReferenceEquals(this, childLayers[i]))
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
        return _parent is HostLayer hl ? hl._childLayers : null;
      }
    }

    /// <summary>
    /// Gets or sets the parent host layer.
    /// </summary>
    public HostLayer? ParentLayer
    {
      get { return _parent as HostLayer; }
      set { ParentObject = value; }
    }

    /// <summary>
    /// Gets the collection of graph objects contained in this layer.
    /// </summary>
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

    /// <summary>
    /// Removes the specified graphics object. Derived classes can override this function not only to remove from the collection of graph objects,
    /// but also from other places were graph objects can be stored, e.g. inside axis styles.
    /// </summary>
    /// <param name="go">The graphics object to remove..</param>
    /// <returns>True if the graph object was removed; otherwise false.</returns>
    public virtual bool Remove(IGraphicBase go)
    {
      if (_graphObjects.Contains(go))
      {
        if (_graphObjects.Remove(go))
        {
          go.Dispose();
          return true;
        }
      }

      return false;
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

    #endregion XYPlotLayer properties and methods

    #region Hit test

    /// <summary>
    /// Performs hit testing against this layer.
    /// </summary>
    /// <param name="hitData">The hit-test data in parent coordinates.</param>
    /// <returns>The matching hit-test object, or <see langword="null"/>.</returns>
    public virtual IHitTestObject? HitTest(HitTestPointData hitData)
    {
      return HitTest(hitData, false);
    }

    /// <summary>
    /// Performs hit testing against this layer.
    /// </summary>
    /// <param name="parentCoord">The hit-test data in parent coordinates.</param>
    /// <param name="plotItemsOnly">If set to <see langword="true"/>, only plot items are considered.</param>
    /// <returns>The matching hit-test object, or <see langword="null"/>.</returns>
    public IHitTestObject? HitTest(HitTestPointData parentCoord, bool plotItemsOnly)
    {
      //			HitTestPointData layerHitTestData = pageC.NewFromTranslationRotationScaleShear(Position.X, Position.Y, -Rotation, ScaleX, ScaleY, ShearX);
      HitTestPointData localCoord = parentCoord.NewFromAdditionalTransformation(_transformation);

      return HitTestWithLocalCoordinates(localCoord, plotItemsOnly);
    }

    /// <summary>
    /// Performs hit testing using coordinates already transformed into the local layer space.
    /// </summary>
    /// <param name="localCoord">The hit-test data in local coordinates.</param>
    /// <param name="plotItemsOnly">If set to <see langword="true"/>, only plot items are considered.</param>
    /// <returns>The matching hit-test object, or <see langword="null"/>.</returns>
    protected virtual IHitTestObject? HitTestWithLocalCoordinates(HitTestPointData localCoord, bool plotItemsOnly)
    {
      IHitTestObject? hit;

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

            return hit;
          }
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

            return hit;
          }
        }
      }

      return null;
    }

    private static bool EhGraphicsObject_Remove(IHitTestObject o)
    {
      var go = (IGraphicBase)o.HittedObject;
      if (o.ParentLayer is { } parentLayer)
        return o.ParentLayer.Remove(go);
      else
        return false;
    }

    #endregion Hit test
  }
}
