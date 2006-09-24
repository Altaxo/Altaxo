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

namespace Altaxo.Graph.Gdi.AxisStyles
{
  /// <summary>
  /// AxisStylesSummary collects all styles that correspond to one axis scale (i.e. either x-axis or y-axis)
  /// in one class. This contains the grid style of the axis, and one or more axis styles
  /// </summary>
  public class G2DScaleStyle : ICloneable, Main.IChangedEventSource
  {
    GridStyle _gridStyle;
    List<G2DAxisStyle> _axisStyles;

    G2DCoordinateSystem _cachedCoordinateSystem;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLayerAxisStylesSummary", 0)]
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
        G2DScaleStyle s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual G2DScaleStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        G2DScaleStyle s = null != o ? (G2DScaleStyle)o : new G2DScaleStyle();

        s.GridStyle = (GridStyle)info.GetValue("Grid", s);

        int count = info.OpenArray();
        //s._edges = new EdgeType[count];
        for (int i = 0; i < count; ++i)
          info.GetEnum("e", typeof(EdgeType));
        info.CloseArray(count);

        count = info.OpenArray();
        //s._axisStyles = new XYPlotLayerAxisStyleProperties[count];
        for (int i = 0; i < count; ++i)
          s._axisStyles.Add((G2DAxisStyle)info.GetValue("e", s));
        info.CloseArray(count);

        return s;
      }
    }

    // 2006-09-08 - renaming to G2DScaleStyle
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DScaleStyle), 1)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        G2DScaleStyle s = (G2DScaleStyle)obj;


        info.AddValue("Grid", s._gridStyle);

        info.CreateArray("AxisStyles", s._axisStyles.Count);
        for (int i = 0; i < s._axisStyles.Count; ++i)
          info.AddValue("e", s._axisStyles[i]);
        info.CommitArray();
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        G2DScaleStyle s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual G2DScaleStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        G2DScaleStyle s = null != o ? (G2DScaleStyle)o : new G2DScaleStyle();

        s.GridStyle = (GridStyle)info.GetValue("Grid", s);

        int count = info.OpenArray();
        //s._axisStyles = new XYPlotLayerAxisStyleProperties[count];
        for (int i = 0; i < count; ++i)
          s._axisStyles.Add((G2DAxisStyle)info.GetValue("e", s));
        info.CloseArray(count);

        return s;
      }
    }

    #endregion

    /// <summary>
    /// Default constructor. Defines neither a grid style nor an axis style.
    /// </summary>
    public G2DScaleStyle()
    {
      _axisStyles = new List<G2DAxisStyle>();
    }


    void CopyFrom(G2DScaleStyle from)
    {
      this.GridStyle = from._gridStyle == null ? null : (GridStyle)from._gridStyle.Clone();

      this._axisStyles.Clear();
      for (int i = 0; i < _axisStyles.Count; ++i)
      {
        this.AddAxisStyle((G2DAxisStyle)from._axisStyles[i].Clone());
      }
    }

    public void AddAxisStyle(G2DAxisStyle value)
    {
      if (value != null)
      {
        _axisStyles.Add(value);
        value.Changed += new EventHandler(this.EhChildChanged);
        OnChanged();
      }
    }

    public void RemoveAxisStyle(A2DAxisStyleIdentifier id)
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

    public G2DAxisStyle AxisStyleEnsured(A2DAxisStyleIdentifier id)
    {
      G2DAxisStyle prop = AxisStyle(id);
      if (prop == null)
      {
        prop = new G2DAxisStyle(id);
        prop.CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(id);
        AddAxisStyle(prop);
      }
      return prop;
    }


    public bool ContainsAxisStyle(A2DAxisStyleIdentifier id)
    {
      return null != AxisStyle(id);
    }

    public G2DAxisStyle AxisStyle(A2DAxisStyleIdentifier id)
    {

      foreach(G2DAxisStyle p in _axisStyles)
        if(p.StyleID==id)
          return p;

      return null;
    }

    public IEnumerable<G2DAxisStyle> AxisStyles
    {
      get
      {
        return _axisStyles;
      }
    }


    public GridStyle GridStyle
    {
      get { return _gridStyle; }
      set
      {
        GridStyle oldvalue = _gridStyle;
        _gridStyle = value;
        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (oldvalue != null)
            oldvalue.Changed -= new EventHandler(this.EhChildChanged);
          if (value != null)
            value.Changed += new EventHandler(this.EhChildChanged);

          OnChanged();
        }
      }
    }

    public void SetParentLayer(XYPlotLayer layer, bool suppressEvents)
    {
      _cachedCoordinateSystem = layer.CoordinateSystem;

      foreach (G2DAxisStyle style in this._axisStyles)
        style.CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(style.StyleID);
    }


    public bool Remove(GraphicsObject go)
    {
      for (int i = 0; i < this._axisStyles.Count; ++i)
        if (_axisStyles[i] != null && _axisStyles[i].Remove(go))
          return true;

      return false;
    }

    public void Paint(Graphics g, XYPlotLayer layer, int axisnumber)
    {
      PaintGrid(g, layer, axisnumber);
      PaintAxes(g, layer, axisnumber);
    }
    public void PaintGrid(Graphics g, XYPlotLayer layer, int axisnumber)
    {
      Scale axis = axisnumber == 0 ? layer.XAxis : layer.YAxis;

      if (null != _gridStyle)
        _gridStyle.Paint(g, layer, axisnumber);
    }

    public void PaintAxes(Graphics g, XYPlotLayer layer, int axisnumber)
    {

      for (int i = 0; i < _axisStyles.Count; ++i)
        _axisStyles[i].Paint(g, layer, axisnumber);
    }


    #region IChangedEventSource Members

    public event EventHandler Changed;

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
      G2DScaleStyle result = new G2DScaleStyle();
      result.CopyFrom(this);
      return result;
    }

    #endregion
  }

 
}
