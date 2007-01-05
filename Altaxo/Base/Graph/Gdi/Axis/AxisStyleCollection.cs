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
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Gdi.Shapes;

namespace Altaxo.Graph.Gdi.Axis
{
  /// <summary>
  /// AxisStylesSummary collects all styles that correspond to one axis scale (i.e. either x-axis or y-axis)
  /// in one class. This contains the grid style of the axis, and one or more axis styles
  /// </summary>
  public class AxisStyleCollection 
    :
    ICloneable, 
    Main.IChangedEventSource,
    Main.IDocumentNode,
    IEnumerable<AxisStyle>

  {
    List<AxisStyle> _axisStyles;

    G2DCoordinateSystem _cachedCoordinateSystem;

    [field: NonSerialized]
    event EventHandler _changed;

    [NonSerialized]
    object _parent;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyleCollection), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        AxisStyleCollection s = (AxisStyleCollection)obj;



        info.CreateArray("AxisStyles", s._axisStyles.Count);
        for (int i = 0; i < s._axisStyles.Count; ++i)
          info.AddValue("e", s._axisStyles[i]);
        info.CommitArray();
      }

    


      protected virtual AxisStyleCollection SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AxisStyleCollection s = null != o ? (AxisStyleCollection)o : new AxisStyleCollection();

        int count = info.OpenArray();
        for (int i = 0; i < count; ++i)
          s._axisStyles.Add((AxisStyle)info.GetValue("e", s));
        info.CloseArray(count);

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AxisStyleCollection s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion

    /// <summary>
    /// Default constructor. Defines neither a grid style nor an axis style.
    /// </summary>
    public AxisStyleCollection()
    {
      _axisStyles = new List<AxisStyle>();
    }


    void CopyFrom(AxisStyleCollection from)
    {

      this._axisStyles.Clear();
      for (int i = 0; i < from._axisStyles.Count; ++i)
      {
        this.Add((AxisStyle)from._axisStyles[i].Clone());
      }

      this._parent = from._parent;
      this._cachedCoordinateSystem = from._cachedCoordinateSystem;
    }

    public AxisStyle this[CSLineID id]
  {
    get
    {
      foreach (AxisStyle p in _axisStyles)
        if (p.StyleID == id)
          return p;

      return null;
    }
  }

    public void Add(AxisStyle value)
    {
      if (value != null)
      {
        value.ParentObject = this;
        if(_cachedCoordinateSystem!=null)
          value.CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(value.StyleID);

        _axisStyles.Add(value);
        OnChanged();
      }
    }

    public void Remove(CSLineID id)
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

      if (idx >= 0)
        _axisStyles.RemoveAt(idx);
    }

    public AxisStyle AxisStyleEnsured(CSLineID id)
    {
      AxisStyle prop = this[id];
      if (prop == null)
      {
        prop = new AxisStyle(id);
        prop.CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(id);
        Add(prop);
      }
      return prop;
    }

    /// <summary>
    /// Creates the axis style with ShowAxisLine = true and ShowMajorLabels = true
    /// </summary>
    /// <param name="id">The axis style identifier.</param>
    /// <returns>The newly created axis style, if it was not in the collection before. Returns the unchanged axis style, if it was present already in the collection.</returns>
    public AxisStyle CreateDefault(CSLineID id)
    {
      AxisStyle prop = this[id];
      if (prop == null)
      {
        prop = new AxisStyle(id);
        prop.CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(id);

        prop.ShowAxisLine = true;
        prop.ShowMajorLabels = true;

        Add(prop);
      }
      return prop;
    }


    public bool Contains(CSLineID id)
    {
      return null != this[id];
    }

    public IEnumerable<CSLineID> AxisStyleIDs
    {
      get
      {
          foreach (AxisStyle style in _axisStyles)
            yield return style.StyleID;
      }
    }


 
    public void UpdateCoordinateSystem(G2DCoordinateSystem cs)
    {
      _cachedCoordinateSystem = cs;

      foreach (AxisStyle style in this._axisStyles)
        style.CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(style.StyleID);
    }


    public bool Remove(GraphicBase go)
    {
      for (int i = 0; i < this._axisStyles.Count; ++i)
        if (_axisStyles[i] != null && _axisStyles[i].Remove(go))
          return true;

      return false;
    }

    public void Paint(Graphics g, XYPlotLayer layer)
    {
      for (int i = 0; i < _axisStyles.Count; ++i)
        _axisStyles[i].Paint(g, layer);
    }

    #region IChangedEventSource Members

    
    public event EventHandler Changed
    {
      add { _changed += value; }
      remove { _changed -= value; }
    }

    protected virtual void OnChanged()
    {
      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

      if (null != _changed)
        _changed(this, EventArgs.Empty);
    }

    void EhChildChanged(object sender, EventArgs e)
    {
      OnChanged();
    }

    #endregion

    #region ICloneable Members

    public object Clone()
    {
      AxisStyleCollection result = new AxisStyleCollection();
      result.CopyFrom(this);
      return result;
    }

    #endregion

   
    #region IDocumentNode Members

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public string Name
    {
      get { return "AxisStyles"; }
    }

    #endregion

    #region IEnumerable<AxisStyle> Members

    public IEnumerator<AxisStyle> GetEnumerator()
    {
      return _axisStyles.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _axisStyles.GetEnumerator();
    }

    #endregion
  }


}
