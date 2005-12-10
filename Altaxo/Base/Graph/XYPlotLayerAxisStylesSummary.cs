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
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Axes;
using Altaxo.Graph.Axes.Boundaries;

namespace Altaxo.Graph
{
  public class XYPlotLayerAxisStylesSummary : ICloneable, Main.IChangedEventSource
  {
    GridStyle _gridStyle;
    XYPlotLayerAxisStyleProperties[] _axisStyles;
    EdgeType[] _edges;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayerAxisStylesSummary), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
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
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLayerAxisStylesSummary s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual XYPlotLayerAxisStylesSummary SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLayerAxisStylesSummary s = null != o ? (XYPlotLayerAxisStylesSummary)o : new XYPlotLayerAxisStylesSummary();

        s.GridStyle = (GridStyle)info.GetValue("Grid", s);

        int count = info.OpenArray();
        s._edges = new EdgeType[count];
        for (int i = 0; i < count; ++i)
          s._edges[i] = (EdgeType)info.GetEnum("e", typeof(EdgeType));
        info.CloseArray(count);

        count = info.OpenArray();
        s._axisStyles = new XYPlotLayerAxisStyleProperties[count];
        for (int i = 0; i < count; ++i)
          s.SetAxisStyle((XYPlotLayerAxisStyleProperties)info.GetValue("e", s),i);
        info.CloseArray(count);

        return s;
      }
    }
    #endregion

    /// <summary>
    /// For deserialization only.
    /// </summary>
    protected XYPlotLayerAxisStylesSummary()
    {
    }

    public XYPlotLayerAxisStylesSummary(EdgeType[] edges)
    {
      _edges = (EdgeType[])edges.Clone();
      _axisStyles = new XYPlotLayerAxisStyleProperties[_edges.Length];
      for (int i = 0; i < _edges.Length; ++i)
        SetAxisStyle(new XYPlotLayerAxisStyleProperties(_edges[i]), i);
    }

    void CopyFrom(XYPlotLayerAxisStylesSummary from)
    {
      this.GridStyle = from._gridStyle == null ? null : (GridStyle)from._gridStyle.Clone();

      this._axisStyles = new XYPlotLayerAxisStyleProperties[from._axisStyles.Length];
      for (int i = 0; i < _axisStyles.Length; ++i)
      {
        if (from._axisStyles[i] != null)
          SetAxisStyle((XYPlotLayerAxisStyleProperties)from._axisStyles[i].Clone(), i);
      }
    }

    public void SetAxisStyle(XYPlotLayerAxisStyleProperties value, int i)
    {
      XYPlotLayerAxisStyleProperties oldvalue = _axisStyles[i];
      _axisStyles[i] = value;
      if (!object.ReferenceEquals(value, oldvalue))
      {
        {
          if (oldvalue != null)
            oldvalue.Changed -= new EventHandler(this.EhChildChanged);
          if (value != null)
            value.Changed += new EventHandler(this.EhChildChanged);

          OnChanged();
        }
      }
    }

    public XYPlotLayerAxisStyleProperties AxisStyle(int i)
    {
      if (null == _axisStyles[i])
        _axisStyles[i] = new XYPlotLayerAxisStyleProperties(_edges[i]);
      return _axisStyles[i];
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




    public bool Remove(GraphicsObject go)
    {
      for (int i = 0; i < this._axisStyles.Length; ++i)
        if (_axisStyles[i] != null && _axisStyles[i].Remove(go))
          return true;

      return false;
    }

    public void Paint(Graphics g, XYPlotLayer layer, int axisnumber)
    {
      Axis axis = axisnumber == 0 ? layer.XAxis : layer.YAxis;

      for (int i = 0; i < _axisStyles.Length; ++i)
        if (null != _axisStyles[i])
          _axisStyles[i].Paint(g, layer, axis);

      if (null != _gridStyle)
        _gridStyle.Paint(g, layer, axisnumber);
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
      XYPlotLayerAxisStylesSummary result = new XYPlotLayerAxisStylesSummary(this._edges);
      result.CopyFrom(this);
      return result;
    }

    #endregion
  }


  public class XYPlotLayerAxisStylesSummaryCollection : Main.IChildChangedEventSink, Main.IChangedEventSource, ICloneable
  {


    XYPlotLayerAxisStylesSummary[] _styles;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayerAxisStylesSummaryCollection), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotLayerAxisStylesSummaryCollection s = (XYPlotLayerAxisStylesSummaryCollection)obj;

        info.CreateArray("Styles", s._styles.Length);
        for (int i = 0; i < s._styles.Length; ++i)
          info.AddValue("e", s._styles[i]);
        info.CommitArray();

      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLayerAxisStylesSummaryCollection s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual XYPlotLayerAxisStylesSummaryCollection SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLayerAxisStylesSummaryCollection s = null != o ? (XYPlotLayerAxisStylesSummaryCollection)o : new XYPlotLayerAxisStylesSummaryCollection();

        int count = info.OpenArray();
        s._styles = new XYPlotLayerAxisStylesSummary[count];
        for (int i = 0; i < count; ++i)
          s.SetStyle((XYPlotLayerAxisStylesSummary)info.GetValue("e", s), i);
        info.CloseArray(count);

        return s;
      }
    }
    #endregion


    public XYPlotLayerAxisStylesSummaryCollection()
    {
      _styles = new XYPlotLayerAxisStylesSummary[2];

      this._styles[0] = new XYPlotLayerAxisStylesSummary(new EdgeType[] { EdgeType.Bottom, EdgeType.Top });
      this._styles[0].Changed += new EventHandler(this.EhChildChanged);

      this._styles[1] = new XYPlotLayerAxisStylesSummary(new EdgeType[] { EdgeType.Left, EdgeType.Right });
      this._styles[1].Changed += new EventHandler(this.EhChildChanged);

    }

    void CopyFrom(XYPlotLayerAxisStylesSummaryCollection from)
    {
      // Remove old event handlers
      for (int i = 0; i < this._styles.Length; ++i)
        if (_styles[i] != null)
          _styles[i].Changed -= new EventHandler(this.EhChildChanged);

      // now clone
      for (int i = 0; i < from._styles.Length; ++i)
      {
        this._styles[i] = from._styles[i] == null ? null : (XYPlotLayerAxisStylesSummary)from._styles[i].Clone();
        if (this._styles[i] != null)
          this._styles[i].Changed += new EventHandler(this.EhChildChanged);
      }
    }

    public XYPlotLayerAxisStyleProperties this[EdgeType edge]
    {
      get
      {
        switch (edge)
        {
          case EdgeType.Bottom:
            return _styles[0].AxisStyle(0);

          case EdgeType.Top:
            return _styles[0].AxisStyle(1);

          case EdgeType.Left:
            return _styles[1].AxisStyle(0);

          case EdgeType.Right:
            return _styles[1].AxisStyle(1);
          default:
            return null;
        }
      }
    }

    public XYPlotLayerAxisStylesSummary Axis(int i)
    {
      return _styles[i];
    }

    public void SetStyle(XYPlotLayerAxisStylesSummary value, int i)
    {
      if(i<0)
        throw new ArgumentOutOfRangeException("Index i is negative");
      if(i>=_styles.Length)
        throw new ArgumentOutOfRangeException("Index i is greater than length of internal array");

      XYPlotLayerAxisStylesSummary oldvalue = _styles[i];
      _styles[i] = value;

      if(null!=oldvalue)
        oldvalue.Changed -= new EventHandler(this.EhChildChanged);
      if (null!= value)
        value.Changed += new EventHandler(this.EhChildChanged);

      if(!object.ReferenceEquals(oldvalue,value))
        OnChanged();
    }

    public XYPlotLayerAxisStylesSummary X
    {
      get
      {
        return _styles[0];
      }
    }

    public XYPlotLayerAxisStylesSummary Y
    {
      get
      {
        return _styles[1];
      }
    }

    public bool Remove(GraphicsObject go)
    {
      for (int i = 0; i < this._styles.Length; ++i)
        if (_styles[i] != null && _styles[i].Remove(go))
          return true;

      return false;
    }

    public void Paint(Graphics g, XYPlotLayer layer)
    {
      _styles[0].Paint(g, layer, 0);
      _styles[1].Paint(g, layer, 1);
    }

    #region IChildChangedEventSink Members

    public void EhChildChanged(object child, EventArgs e)
    {
      OnChanged();
    }

    #endregion

    #region IChangedEventSource Members

    public event EventHandler Changed;

    public void OnChanged()
    {
      if (Changed != null)
        Changed(this, EventArgs.Empty);
    }

    #endregion

    #region ICloneable Members

    public object Clone()
    {
      XYPlotLayerAxisStylesSummaryCollection res = new XYPlotLayerAxisStylesSummaryCollection();
      res.CopyFrom(this);
      return res;
    }

    #endregion
  }

}
