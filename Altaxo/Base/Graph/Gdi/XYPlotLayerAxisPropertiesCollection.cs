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
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Gdi
{
  [Serializable]
  public class XYPlotLayerAxisPropertiesCollection : Main.IChangedEventSource
  {
    XYPlotLayerAxisProperties[] _props = new XYPlotLayerAxisProperties[2];

    /// <summary>
    /// Fired if one of the axis has changed (or its boundaries).
    /// </summary>
    [field: NonSerialized]
    public event EventHandler AxesChanged;

    /// <summary>
    /// Fired if something in this class or in its child has changed.
    /// </summary>
    [field: NonSerialized]
    public event EventHandler Changed;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLayerAxisPropertiesCollection", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayerAxisPropertiesCollection), 1)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotLayerAxisPropertiesCollection s = (XYPlotLayerAxisPropertiesCollection)obj;

        info.CreateArray("Properties", s._props.Length);
        for (int i = 0; i < s._props.Length; ++i)
          info.AddValue("e", s._props[i]);
        info.CommitArray();
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLayerAxisPropertiesCollection s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual XYPlotLayerAxisPropertiesCollection SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLayerAxisPropertiesCollection s = null != o ? (XYPlotLayerAxisPropertiesCollection)o : new XYPlotLayerAxisPropertiesCollection();

        int count = info.OpenArray("Properties");
        s._props = new XYPlotLayerAxisProperties[count];
        for (int i = 0; i < count; ++i)
          s.SetAxisProperties((XYPlotLayerAxisProperties)info.GetValue("e", s), i);
        info.CloseArray(count);

        return s;
      }
    }
    #endregion

    public XYPlotLayerAxisPropertiesCollection()
    {
      _props = new XYPlotLayerAxisProperties[2];
      this.SetAxisProperties(new XYPlotLayerAxisProperties(), 0);
      this.SetAxisProperties(new XYPlotLayerAxisProperties(), 1);
    }

    public XYPlotLayerAxisPropertiesCollection(XYPlotLayerAxisPropertiesCollection from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(XYPlotLayerAxisPropertiesCollection from)
    {
      if (_props != null)
      {
        for (int i = 0; i < _props.Length; ++i)
        {
          if (_props[i] != null)
            _props[i].AxisPropertiesChanged -= new EventHandler(EhAxisPropertiesChanged);
          _props[i] = null;
        }
      }

      _props = new XYPlotLayerAxisProperties[from._props.Length];
      for (int i = 0; i < from._props.Length; i++)
      {
        _props[i] = from._props[i].Clone();
        _props[i].AxisPropertiesChanged += new EventHandler(EhAxisPropertiesChanged);
      }

      OnChanged();
    }

    public XYPlotLayerAxisPropertiesCollection Clone()
    {
      return new XYPlotLayerAxisPropertiesCollection(this);
    }

    public XYPlotLayerAxisProperties X
    {
      get
      {
        return _props[0];
      }
    }

    public XYPlotLayerAxisProperties Y
    {
      get
      {
        return _props[1];
      }
    }

    public Scale Axis(int i)
    {
      return _props[i].Axis;
    }
    public void SetAxis(int i, Scale ax)
    {
      _props[i].Axis = ax;
    }
    public int IndexOf(Scale ax)
    {
      for (int i = 0; i < _props.Length; i++)
      {
        if (_props[i].Axis == ax)
          return i;
      }

      return -1;
    }

    protected void SetAxisProperties(XYPlotLayerAxisProperties newvalue, int i)
    {
      XYPlotLayerAxisProperties oldvalue = _props[i];
      _props[i] = newvalue;

      if (!object.ReferenceEquals(oldvalue, newvalue))
      {
        if (null != oldvalue)
          oldvalue.AxisPropertiesChanged -= new EventHandler(EhAxisPropertiesChanged);
        if (null != newvalue)
          newvalue.AxisPropertiesChanged += new EventHandler(EhAxisPropertiesChanged);
      }
    }

    private void EhAxisPropertiesChanged(object sender, EventArgs e)
    {
      if (AxesChanged != null)
        AxesChanged(this, EventArgs.Empty);

      OnChanged();
    }

    protected virtual void OnChanged()
    {
      if (null != Changed)
        Changed(this, EventArgs.Empty);
    }

  }





}
