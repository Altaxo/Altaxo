#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

using Altaxo.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
  public class GridPartitioning
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    Main.ICopyFrom
  {
    private LinearPartitioning _xPartitioning;
    private LinearPartitioning _yPartitioning;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2013-09-25 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GridPartitioning), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GridPartitioning)obj;

        info.AddValue("XPartitioning", s._xPartitioning);
        info.AddValue("YPartitioning", s._yPartitioning);
      }

      protected virtual GridPartitioning SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (o == null ? new GridPartitioning() : (GridPartitioning)o);

        s.ChildSetMember(ref s._xPartitioning, (LinearPartitioning)info.GetValue("XPartitioning", s));
        s.ChildSetMember(ref s._yPartitioning, (LinearPartitioning)info.GetValue("YPartitioning", s));

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    public GridPartitioning()
    {
      _xPartitioning = new LinearPartitioning() { ParentObject = this };
      _yPartitioning = new LinearPartitioning() { ParentObject = this };
    }

    public GridPartitioning(GridPartitioning from)
    {
      _xPartitioning = new LinearPartitioning() { ParentObject = this };
      _yPartitioning = new LinearPartitioning() { ParentObject = this };
      CopyFrom(from);
    }

    public bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as GridPartitioning;
      if (null != from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          ChildCopyToMember(ref _xPartitioning, from._xPartitioning);
          ChildCopyToMember(ref _yPartitioning, from._yPartitioning);

          suspendToken.Resume();
        }
        return true;
      }
      return false;
    }

    public object Clone()
    {
      return new GridPartitioning(this);
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _xPartitioning)
        yield return new Main.DocumentNodeAndName(_xPartitioning, () => _xPartitioning = null, "XPartitioning");
      if (null != _yPartitioning)
        yield return new Main.DocumentNodeAndName(_yPartitioning, () => _yPartitioning = null, "YPartitioning");
    }

    public LinearPartitioning XPartitioning { get { return _xPartitioning; } }

    public LinearPartitioning YPartitioning { get { return _yPartitioning; } }

    public bool IsEmpty { get { return _xPartitioning.Count == 0 && _yPartitioning.Count == 0; } }

    public RectangleD2D GetTileRectangle(double column, double row, double columnSpan, double rowSpan, PointD2D totalSize)
    {
      double xstart, xsize;
      double ystart, ysize;
      _xPartitioning.GetAbsolutePositionAndSizeFromGridIndexAndSpan(totalSize.X, column, columnSpan, out xstart, out xsize);
      _yPartitioning.GetAbsolutePositionAndSizeFromGridIndexAndSpan(totalSize.Y, row, rowSpan, out ystart, out ysize);
      return new RectangleD2D(xstart, ystart, xsize, ysize);
    }
  }
}
