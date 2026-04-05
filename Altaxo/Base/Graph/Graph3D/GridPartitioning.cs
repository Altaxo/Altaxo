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
using System.Collections.Generic;
using Altaxo.Geometry;

namespace Altaxo.Graph.Graph3D
{
  /// <summary>
  /// Represents the grid partitioning used to arrange child layers in a three-dimensional host layer.
  /// </summary>
  public class GridPartitioning
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    Main.ICopyFrom
  {
    /// <summary>
    /// The partitioning along the x axis.
    /// </summary>
    protected Altaxo.Graph.LinearPartitioning _xPartitioning;

    /// <summary>
    /// The partitioning along the y axis.
    /// </summary>
    protected Altaxo.Graph.LinearPartitioning _yPartitioning;

    /// <summary>
    /// The partitioning along the z axis.
    /// </summary>
    protected Altaxo.Graph.LinearPartitioning _zPartitioning;

    #region Serialization

    #region Version 0

    /// <summary>
    /// Serializes <see cref="GridPartitioning"/> instances.
    /// 2015-09-49 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GridPartitioning), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GridPartitioning)obj;

        info.AddValue("XPartitioning", s._xPartitioning);
        info.AddValue("YPartitioning", s._yPartitioning);
        info.AddValue("ZPartitioning", s._zPartitioning);
      }

      protected virtual GridPartitioning SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (GridPartitioning?)o ?? new GridPartitioning();

        s._xPartitioning = (LinearPartitioning)info.GetValue("XPartitioning", s);
        if (s._xPartitioning is not null)
          s._xPartitioning.ParentObject = s;

        s._yPartitioning = (LinearPartitioning)info.GetValue("YPartitioning", s);
        if (s._yPartitioning is not null)
          s._yPartitioning.ParentObject = s;

        s._zPartitioning = (LinearPartitioning)info.GetValue("ZPartitioning", s);
        if (s._zPartitioning is not null)
          s._zPartitioning.ParentObject = s;

        return s;
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="GridPartitioning"/> class.
    /// </summary>
    public GridPartitioning()
    {
      _xPartitioning = new LinearPartitioning() { ParentObject = this };
      _yPartitioning = new LinearPartitioning() { ParentObject = this };
      _zPartitioning = new LinearPartitioning() { ParentObject = this };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GridPartitioning"/> class by copying from another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public GridPartitioning(GridPartitioning from)
    {
      _xPartitioning = new LinearPartitioning() { ParentObject = this };
      _yPartitioning = new LinearPartitioning() { ParentObject = this };
      _zPartitioning = new LinearPartitioning() { ParentObject = this };
      CopyFrom(from);
    }

    /// <inheritdoc/>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var from = obj as GridPartitioning;
      if (from is not null)
      {
        using (var suspendToken = SuspendGetToken())
        {
          ChildCopyToMember(ref _xPartitioning, from._xPartitioning);
          ChildCopyToMember(ref _yPartitioning, from._yPartitioning);
          ChildCopyToMember(ref _zPartitioning, from._zPartitioning);

          suspendToken.Resume();
        }
        return true;
      }
      return false;
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new GridPartitioning(this);
    }

    /// <inheritdoc/>
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_xPartitioning is not null)
        yield return new Main.DocumentNodeAndName(_xPartitioning, () => _xPartitioning = null!, "XPartitioning");
      if (_yPartitioning is not null)
        yield return new Main.DocumentNodeAndName(_yPartitioning, () => _yPartitioning = null!, "YPartitioning");
      if (_zPartitioning is not null)
        yield return new Main.DocumentNodeAndName(_zPartitioning, () => _zPartitioning = null!, "ZPartitioning");
    }

    /// <summary>
    /// Gets the X partitioning.
    /// </summary>
    public LinearPartitioning XPartitioning { get { return _xPartitioning; } }

    /// <summary>
    /// Gets the Y partitioning.
    /// </summary>
    public LinearPartitioning YPartitioning { get { return _yPartitioning; } }

    /// <summary>
    /// Gets the Z partitioning.
    /// </summary>
    public LinearPartitioning ZPartitioning { get { return _zPartitioning; } }

    /// <summary>
    /// Gets a value indicating whether all partitionings are empty.
    /// </summary>
    public virtual bool IsEmpty { get { return _xPartitioning.Count == 0 && _yPartitioning.Count == 0 && _zPartitioning.Count == 0; } }

    /// <summary>
    /// Gets the rectangle of a grid tile in absolute coordinates.
    /// </summary>
    /// <param name="columnPosX">The x grid position.</param>
    /// <param name="columnPosY">The y grid position.</param>
    /// <param name="columnPosZ">The z grid position.</param>
    /// <param name="columnSpanX">The x span.</param>
    /// <param name="columnSpanY">The y span.</param>
    /// <param name="columnSpanZ">The z span.</param>
    /// <param name="totalSize">The total size of the parent layer.</param>
    /// <returns>The absolute tile rectangle.</returns>
    public RectangleD3D GetTileRectangle(double columnPosX, double columnPosY, double columnPosZ,
      double columnSpanX, double columnSpanY, double columnSpanZ, VectorD3D totalSize)
    {
      _xPartitioning.GetAbsolutePositionAndSizeFromGridIndexAndSpan(totalSize.X, columnPosX, columnSpanX, out var xstart, out var xsize);
      _yPartitioning.GetAbsolutePositionAndSizeFromGridIndexAndSpan(totalSize.Y, columnPosY, columnSpanY, out var ystart, out var ysize);
      _zPartitioning.GetAbsolutePositionAndSizeFromGridIndexAndSpan(totalSize.Z, columnPosZ, columnSpanZ, out var zstart, out var zsize);
      return new RectangleD3D(xstart, ystart, zstart, xsize, ysize, zsize);
    }
  }
}
