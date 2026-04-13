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
using System.Diagnostics.CodeAnalysis;
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D.Axis
{
  /// <summary>
  /// Represents a grid plane of a three-dimensional plot area.
  /// </summary>
  [Serializable]
  public class GridPlane :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    ICloneable
  {
    /// <summary>
    /// Identifies the plane by the axis that is perpendicular to the plane.
    /// </summary>
    private CSPlaneID _planeID;

    /// <summary>
    /// Gridstyle of the smaller of the two axis numbers.
    /// </summary>
    private GridStyle? _grid1;

    /// <summary>
    /// Gridstyle of the greater axis number.
    /// </summary>
    private GridStyle? _grid2;

    /// <summary>
    /// Background of the grid plane.
    /// </summary>
    private IMaterial? _background;

    [NonSerialized]
    private GridIndexer _cachedIndexer;

    /// <summary>
    /// Copies values from another <see cref="GridPlane"/> instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    [MemberNotNull(nameof(_planeID))]
    private void CopyFrom(GridPlane from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      _planeID = from._planeID;
      ChildCloneToMember(ref _grid1, from._grid1);
      ChildCloneToMember(ref _grid2, from._grid2);
      ChildCloneToMemberAlt(ref _background, from._background);
    }

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2015-11-14 initial version-
    /// </summary>
    /// <summary>
    /// Serializes <see cref="GridPlane"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GridPlane), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GridPlane)o;

        info.AddValue("ID", s._planeID);
        info.AddValueOrNull("Grid1", s._grid1);
        info.AddValueOrNull("Grid2", s._grid2);
        info.AddValueOrNull("Background", s._background);
      }

      protected virtual GridPlane SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var id = (CSPlaneID)info.GetValue("ID", null);
        GridPlane s = (o is null ? new GridPlane(id) : (GridPlane)o);
        s.GridStyleFirst = info.GetValueOrNull<GridStyle>("Grid1", s);
        s.GridStyleSecond = info.GetValueOrNull<GridStyle>("Grid2", s);
        s.Background = info.GetValueOrNull<IMaterial>("Background", s);

        return s;
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        GridPlane s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="GridPlane"/> class.
    /// </summary>
    /// <param name="id">The plane identifier.</param>
    public GridPlane(CSPlaneID id)
    {
      _cachedIndexer = new GridIndexer(this);
      _planeID = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GridPlane"/> class by copying from another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public GridPlane(GridPlane from)
    {
      _cachedIndexer = new GridIndexer(this);
      CopyFrom(from);
    }

    /// <inheritdoc/>
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_grid1 is not null)
        yield return new Main.DocumentNodeAndName(_grid1, "Grid1");
      if (_grid2 is not null)
        yield return new Main.DocumentNodeAndName(_grid2, "Grid2");
    }

    /// <summary>
    /// Creates a strongly typed clone of this <see cref="GridPlane"/>.
    /// </summary>
    /// <returns>A cloned grid plane.</returns>
    public GridPlane Clone()
    {
      return new GridPlane(this);
    }

    /// <inheritdoc/>
    object ICloneable.Clone()
    {
      return new GridPlane(this);
    }

    /// <summary>
    /// Gets the identifier of this grid plane.
    /// </summary>
    public CSPlaneID PlaneID
    {
      get
      {
        return _planeID;
      }
    }

    /// <summary>
    /// Gets or sets the first grid style of the plane.
    /// </summary>
    public GridStyle? GridStyleFirst
    {
      get { return _grid1; }
      set
      {
        if (ChildSetMember(ref _grid1, value))
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the second grid style of the plane.
    /// </summary>
    public GridStyle? GridStyleSecond
    {
      get { return _grid2; }
      set
      {
        if (ChildSetMember(ref _grid2, value))
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets indexed access to the grid styles of the plane.
    /// </summary>
    public Altaxo.Collections.IArray<GridStyle?> GridStyle
    {
      get { return _cachedIndexer; }
    }

    /// <summary>
    /// Gets or sets the background material of the plane.
    /// </summary>
    public IMaterial? Background
    {
      get { return _background; }
      set
      {
        if(ChildSetMemberAlt(ref _background, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Indicates if this grid is used, i.e. hase some visible elements. Returns false
    /// if Grid1 and Grid2 and Background are null.
    /// </summary>
    public bool IsUsed
    {
      get
      {
        return _grid1 is not null || _grid2 is not null || _background is not null;
      }
    }

    /// <summary>
    /// Paints the background of the grid plane.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="layer">The plot area.</param>
    public void PaintBackground(IGraphicsContext3D g, IPlotArea layer)
    {
      if (_background is null)
        return;

      var cs = layer.CoordinateSystem;
      if (layer.CoordinateSystem is CS.G3DCartesicCoordinateSystem)
      {
        var p = new PointD3D[4];
        p[0] = cs.GetPointOnPlane(_planeID, 0, 0);
        p[1] = cs.GetPointOnPlane(_planeID, 0, 1);
        p[2] = cs.GetPointOnPlane(_planeID, 1, 0);
        p[3] = cs.GetPointOnPlane(_planeID, 1, 1);

        var normal = VectorD3D.CrossProduct(p[1] - p[0], p[2] - p[0]).Normalized;

        var buffer = g.GetPositionNormalIndexedTriangleBuffer(_background);

        if (buffer.PositionNormalIndexedTriangleBuffer is not null)
        {
          // front faces
          var offs = buffer.IndexedTriangleBuffer.VertexCount;
          for (int i = 0; i < 4; ++i)
            buffer.PositionNormalIndexedTriangleBuffer.AddTriangleVertex(p[i].X, p[i].Y, p[i].Z, normal.X, normal.Y, normal.Z);

          buffer.IndexedTriangleBuffer.AddTriangleIndices(0 + offs, 1 + offs, 3 + offs);
          buffer.IndexedTriangleBuffer.AddTriangleIndices(2 + offs, 0 + offs, 3 + offs);

          // back faces
          offs = buffer.IndexedTriangleBuffer.VertexCount;
          for (int i = 0; i < 4; ++i)
            buffer.PositionNormalIndexedTriangleBuffer.AddTriangleVertex(p[i].X, p[i].Y, p[i].Z, -normal.X, -normal.Y, -normal.Z);

          buffer.IndexedTriangleBuffer.AddTriangleIndices(0 + offs, 3 + offs, 1 + offs);
          buffer.IndexedTriangleBuffer.AddTriangleIndices(2 + offs, 3 + offs, 0 + offs);
        }
        else
          throw new NotImplementedException();
      }
      else
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Paints the grid lines of the grid plane.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="layer">The plot area.</param>
    public void PaintGrid(IGraphicsContext3D g, IPlotArea layer)
    {
      if (_grid1 is not null)
        _grid1.Paint(g, layer, _planeID, _planeID.InPlaneAxisNumber1);
      if (_grid2 is not null)
        _grid2.Paint(g, layer, _planeID, _planeID.InPlaneAxisNumber2);
    }

    /// <summary>
    /// Paints the complete grid plane.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="layer">The plot area.</param>
    public void Paint(IGraphicsContext3D g, IPlotArea layer)
    {
      PaintBackground(g, layer);
      PaintGrid(g, layer);
    }

    #region Inner class GridIndexer

    private class GridIndexer : Altaxo.Collections.IArray<GridStyle?>
    {
      private GridPlane _parent;

      public GridIndexer(GridPlane parent)
      {
        _parent = parent;
      }

      #region IArray<GridStyle> Members

      public GridStyle? this[int i]
      {
        get
        {
          return i switch
          { 
            0 =>_parent._grid1,
            1 => _parent._grid2,
            _ => throw new ArgumentOutOfRangeException()
          };

        }
        set
        {
          if (0 == i)
            _parent.GridStyleFirst = value;
          else if (i == 1)
            _parent.GridStyleSecond = value;
          else
            throw new ArgumentOutOfRangeException();
        }
      }

      public int Count
      {
        get { return 2; }
      }

      #endregion IArray<GridStyle> Members
    }

    #endregion Inner class GridIndexer
  }
}
