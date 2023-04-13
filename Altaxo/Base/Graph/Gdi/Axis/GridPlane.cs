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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Axis
{
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
    private BrushX? _background;

    [NonSerialized]
    private GridIndexer _cachedIndexer;

    [MemberNotNull(nameof(_planeID))]
    private void CopyFrom(GridPlane from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      _planeID = from._planeID;
      GridStyleFirst = from._grid1 is null ? null : (GridStyle)from._grid1.Clone();
      GridStyleSecond = from._grid2 is null ? null : (GridStyle)from._grid2.Clone();
      Background = from._background;
    }

    #region Serialization

    #region Version 0

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GridPlane), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GridPlane)obj;

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
        s.Background = info.GetValueOrNull<BrushX>("Background", s);

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        GridPlane s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    public GridPlane(CSPlaneID id)
    {
      _cachedIndexer = new GridIndexer(this);
      _planeID = id;
    }

    public GridPlane(GridPlane from)
    {
      _cachedIndexer = new GridIndexer(this);
      CopyFrom(from);
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_grid1 is not null)
        yield return new Main.DocumentNodeAndName(_grid1, "Grid1");
      if (_grid2 is not null)
        yield return new Main.DocumentNodeAndName(_grid2, "Grid2");
    }

    public GridPlane Clone()
    {
      return new GridPlane(this);
    }

    object ICloneable.Clone()
    {
      return new GridPlane(this);
    }

    public CSPlaneID PlaneID
    {
      get
      {
        return _planeID;
      }
    }

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

    public Altaxo.Collections.IArray<GridStyle?> GridStyle
    {
      get { return _cachedIndexer; }
    }

    public BrushX? Background
    {
      get { return _background; }
      set
      {
        if (ChildSetMemberAlt(ref _background, value))
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

    public void PaintBackground(Graphics g, IPlotArea layer)
    {
      Region region = layer.CoordinateSystem.GetRegion();
      if (_background is not null)
      {
        RectangleF innerArea = region.GetBounds(g);
        innerArea.Inflate(innerArea.Width * 1e-5f, innerArea.Height * 1e-5f);
        using (var gdiBackgroundBrush = BrushCacheGdi.Instance.BorrowBrush(_background, innerArea.ToAxo(), g, 1))
        {
          try
          {
            g.FillRegion(gdiBackgroundBrush, region);
          }
          catch (System.OverflowException)
          {
            // 2023-04-13 Ignore an overflow exception (especially when exporting EMF files)
            // it is not clear what is the cause of it
            // but it seems that the rendering is still OK
          }
        }
      }
    }

    public void PaintGrid(Graphics g, IPlotArea layer)
    {
      Region region = layer.CoordinateSystem.GetRegion();
      Region oldClipRegion = g.Clip;
      g.Clip = region;
      if (_grid1 is not null)
        _grid1.Paint(g, layer, _planeID.InPlaneAxisNumber1);
      if (_grid2 is not null)
        _grid2.Paint(g, layer, _planeID.InPlaneAxisNumber2);
      g.Clip = oldClipRegion;
    }

    public void Paint(Graphics g, IPlotArea layer)
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
            0 => _parent._grid1,
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
