#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
  public class AxisStyleCollection : ICloneable, Main.IChangedEventSource
  {
    List<AxisStyle> _axisStyles;

    G2DCoordinateSystem _cachedCoordinateSystem;

    [field: NonSerialized]
    public event EventHandler Changed;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerAxisStylesSummary", 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AxisStyleCollection s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual AxisStyleCollection SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AxisStyleCollection s = null != o ? (AxisStyleCollection)o : new AxisStyleCollection();


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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyleCollection), 1)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        AxisStyleCollection s = (AxisStyleCollection)obj;



        info.CreateArray("AxisStyles", s._axisStyles.Count);
        for (int i = 0; i < s._axisStyles.Count; ++i)
          info.AddValue("e", s._axisStyles[i]);
        info.CommitArray();
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AxisStyleCollection s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual AxisStyleCollection SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AxisStyleCollection s = null != o ? (AxisStyleCollection)o : new AxisStyleCollection();

        int count = info.OpenArray();
        //s._axisStyles = new XYPlotLayerAxisStyleProperties[count];
        for (int i = 0; i < count; ++i)
          s._axisStyles.Add((AxisStyle)info.GetValue("e", s));
        info.CloseArray(count);

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
      for (int i = 0; i < _axisStyles.Count; ++i)
      {
        this.Add((AxisStyle)from._axisStyles[i].Clone());
      }
    }

    public void Add(AxisStyle value)
    {
      if (value != null)
      {
        _axisStyles.Add(value);
        value.Changed += new EventHandler(this.EhChildChanged);
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

      if (idx > 0)
        _axisStyles.RemoveAt(idx);
    }

    public AxisStyle AxisStyleEnsured(CSLineID id)
    {
      AxisStyle prop = AxisStyle(id);
      if (prop == null)
      {
        prop = new AxisStyle(id);
        prop.CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(id);
        Add(prop);
      }
      return prop;
    }


    public bool Contains(CSLineID id)
    {
      return null != AxisStyle(id);
    }

    public AxisStyle AxisStyle(CSLineID id)
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

    public IEnumerable<CSLineID> AxisStyleIDs
    {
      get
      {
          foreach (AxisStyle style in _axisStyles)
            yield return style.StyleID;
      }
    }


 
    public void SetParentLayer(XYPlotLayer layer, bool suppressEvents)
    {
      _cachedCoordinateSystem = layer.CoordinateSystem;

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

    protected virtual void OnChanged()
    {
      if (null != Changed)
        Changed(this, EventArgs.Empty);
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
  }


}
