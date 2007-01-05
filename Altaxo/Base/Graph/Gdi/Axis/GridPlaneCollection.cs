#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.Axis
{
  [Serializable]
  public class GridPlaneCollection 
    :
    IEnumerable<GridPlane>,
    ICloneable,

    Main.IDocumentNode,
    Main.IChangedEventSource
  {
    List<GridPlane> _innerList = new List<GridPlane>();

    [NonSerialized]
    object _parent;

    [field:NonSerialized]
    public event EventHandler Changed;

    #region Serialization
    #endregion

    void CopyFrom(GridPlaneCollection from)
    {
      this.Clear();

      foreach (GridPlane plane in from)
        this.Add((GridPlane)plane.Clone());

    }

    #region Serialization
    #region Version 0
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GridPlaneCollection), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        GridPlaneCollection s = (GridPlaneCollection)obj;

        info.CreateArray("GridPlanes", s.Count);
        foreach (GridPlane plane in s)
          info.AddValue("e", plane);
        info.CommitArray();

      }
      protected virtual GridPlaneCollection SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        GridPlaneCollection s = (o == null ? new GridPlaneCollection() : (GridPlaneCollection)o);

        int count = info.OpenArray("GridPlanes");
        for (int i = 0; i < count; i++)
        {
          GridPlane plane = (GridPlane)info.GetValue("e", s);
          s.Add(plane);
        }
        info.CloseArray(count);

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        GridPlaneCollection s = SDeserialize(o, info, parent);
        return s;
      }
    }
    #endregion
    #endregion


    public GridPlaneCollection()
    {
    }
    public GridPlaneCollection(GridPlaneCollection from)
    {
      CopyFrom(from);
    }
    public GridPlaneCollection Clone()
    {
      return new GridPlaneCollection(this);
    }
    object ICloneable.Clone()
    {
      return new GridPlaneCollection(this);
    }

    public int Count { get { return _innerList.Count; } }

    public GridPlane this[int idx]
    {
      get
      {
        return _innerList[idx];
      }
    }
    public GridPlane this[CSPlaneID planeid]
    {
      get
      {
        foreach (GridPlane plane in _innerList)
        {
          if (plane.PlaneID == planeid)
            return plane;
        }
        return null;
      }
      set
      {
        for (int i = 0; i < Count; i++)
        {
          if (_innerList[i].PlaneID == planeid)
          {
            if (value == null)
              _innerList.RemoveAt(i);
            else
              _innerList[i] = value;
            return;
          }
        }
        // if not found, we add the value to the collection
        if(null!=value)
          Add(value);
      }
    }

    void Attach(GridPlane plane)
    {
      plane.ParentObject = this;
      plane.Changed += EhPlaneChanged;
    }
    void Detach(GridPlane plane)
    {
      plane.Changed -= EhPlaneChanged;
    }
      public void Add(GridPlane plane)
    {
      Attach(plane);
      _innerList.Add(plane);
    }

    public void Clear()
    {
      foreach (GridPlane plane in _innerList)
        Detach(plane);

      _innerList.Clear();
    }

    public void RemoveUnused()
    {
      for (int i = _innerList.Count - 1; i >= 0; i--)
      {
        if (!_innerList[i].IsUsed)
        {
          Detach(_innerList[i]);
          _innerList.RemoveAt(i);
        }
      }
    }

    public bool Contains(CSPlaneID planeid)
    {
      foreach (GridPlane plane in _innerList)
      {
        if (plane.PlaneID == planeid)
          return true;
      }
      return false;
    }

    public void Paint(Graphics g, IPlotArea layer)
    {
      for (int i = 0; i < _innerList.Count; ++i)
        _innerList[i].Paint(g, layer);
    }




    #region IDocumentNode Members

    public object ParentObject
    {
      get
      {
        return _parent;
      }
      set
      {
        _parent = value;
      }
    }

    public string Name
    {
      get { return "GridPlanes"; }
    }

    #endregion

    #region IEnumerable<GridPlane> Members

    public IEnumerator<GridPlane> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion

    #region IChangedEventSource Members

    public void EhPlaneChanged(object sender, EventArgs e)
    {
      OnChanged();
    }

    protected virtual void OnChanged()
    {
      if (Changed != null)
        Changed(this, EventArgs.Empty);
    }

    event EventHandler Altaxo.Main.IChangedEventSource.Changed
    {
      add { throw new Exception("The method or operation is not implemented."); }
      remove { throw new Exception("The method or operation is not implemented."); }
    }

    #endregion
  }
}
