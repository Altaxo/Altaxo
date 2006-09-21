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

namespace Altaxo.Graph
{
  
  /// <summary>
  /// This class holds the (normally two for 2D) AxisStylesSummaries - for every axis scale one summary.
  /// </summary>
  public class G2DScaleStyleCollection : Main.IChildChangedEventSink, Main.IChangedEventSource, ICloneable
  {


    G2DScaleStyle[] _styles;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerAxisStylesSummaryCollection", 0)]
    // 2006-09-08 renamed to G2DScaleStyleCollection
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DScaleStyleCollection), 1)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        G2DScaleStyleCollection s = (G2DScaleStyleCollection)obj;

        info.CreateArray("Styles", s._styles.Length);
        for (int i = 0; i < s._styles.Length; ++i)
          info.AddValue("e", s._styles[i]);
        info.CommitArray();

      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        G2DScaleStyleCollection s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual G2DScaleStyleCollection SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        G2DScaleStyleCollection s = null != o ? (G2DScaleStyleCollection)o : new G2DScaleStyleCollection();

        int count = info.OpenArray();
        s._styles = new G2DScaleStyle[count];
        for (int i = 0; i < count; ++i)
          s.SetScaleStyle((G2DScaleStyle)info.GetValue("e", s), i);
        info.CloseArray(count);

        return s;
      }
    }
    #endregion


    public G2DScaleStyleCollection()
    {
      _styles = new G2DScaleStyle[2];

      this._styles[0] = new G2DScaleStyle();
      this._styles[0].Changed += new EventHandler(this.EhChildChanged);

      this._styles[1] = new G2DScaleStyle();
      this._styles[1].Changed += new EventHandler(this.EhChildChanged);

      //TODO: Fill the styles with default
    }

    void CopyFrom(G2DScaleStyleCollection from)
    {
      // Remove old event handlers
      for (int i = 0; i < this._styles.Length; ++i)
        if (_styles[i] != null)
          _styles[i].Changed -= new EventHandler(this.EhChildChanged);

      // now clone
      for (int i = 0; i < from._styles.Length; ++i)
      {
        this._styles[i] = from._styles[i] == null ? null : (G2DScaleStyle)from._styles[i].Clone();
        if (this._styles[i] != null)
          this._styles[i].Changed += new EventHandler(this.EhChildChanged);
      }
    }

   

    /// <summary>
    /// Return the axis style with the given id. If this style is not present, the return value is null.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public G2DAxisStyle AxisStyle(A2DAxisStyleIdentifier id)
    {
      G2DScaleStyle scaleStyle = _styles[id.AxisNumber];
      return scaleStyle.AxisStyle(id);
    }

    /// <summary>
    /// This will return an axis style with the given id. If not present, this axis style will be created, added to the collection, and returned.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public G2DAxisStyle AxisStyleEnsured(A2DAxisStyleIdentifier id)
    {
      G2DScaleStyle scaleStyle = _styles[id.AxisNumber];
      return scaleStyle.AxisStyleEnsured(id);
    }

    public void RemoveAxisStyle(A2DAxisStyleIdentifier id)
    {
      G2DScaleStyle scaleStyle = _styles[id.AxisNumber];
      scaleStyle.RemoveAxisStyle(id);
    }


    public IEnumerable<G2DAxisStyle> AxisStyles
    {
      get
      {
        for (int i = 0; i < _styles.Length; i++)
        {
          foreach (G2DAxisStyle style in _styles[i].AxisStyles)
            yield return style;
        }
      }
    }

    public bool ContainsAxisStyle(A2DAxisStyleIdentifier id)
    {
      G2DScaleStyle scalestyle = _styles[id.AxisNumber];
      return scalestyle.ContainsAxisStyle(id);
    }

    public G2DScaleStyle ScaleStyle(int i)
    {
      return _styles[i];
    }

    public void SetScaleStyle(G2DScaleStyle value, int i)
    {
      if (i < 0)
        throw new ArgumentOutOfRangeException("Index i is negative");
      if (i >= _styles.Length)
        throw new ArgumentOutOfRangeException("Index i is greater than length of internal array");

      G2DScaleStyle oldvalue = _styles[i];
      _styles[i] = value;

      if (null != oldvalue)
        oldvalue.Changed -= new EventHandler(this.EhChildChanged);
      if (null != value)
        value.Changed += new EventHandler(this.EhChildChanged);

      if (!object.ReferenceEquals(oldvalue, value))
        OnChanged();
    }

    public G2DScaleStyle X
    {
      get
      {
        return _styles[0];
      }
    }

    public G2DScaleStyle Y
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
      _styles[0].PaintGrid(g, layer, 0);
      _styles[1].PaintGrid(g, layer, 1);
      _styles[0].PaintAxes(g, layer, 0);
      _styles[1].PaintAxes(g, layer, 1);
    }

    public void SetParentLayer(XYPlotLayer layer, bool suppressEvents)
    {
      foreach (G2DScaleStyle style in _styles)
        style.SetParentLayer(layer, suppressEvents);
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
      G2DScaleStyleCollection res = new G2DScaleStyleCollection();
      res.CopyFrom(this);
      return res;
    }

    #endregion
  }

}
