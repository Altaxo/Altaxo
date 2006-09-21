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

namespace Altaxo.Graph
{
  public class XYPlotLayerAxisProperties
  {
    /// <summary>
    /// The axis.
    /// </summary>
    private Scale _axis; // the X-Axis

    /// <summary>Indicate if x-axis is linked to the linked layer x axis.</summary>
    private bool _isLinked;

    /// <summary>The value a of x-axis link for link of origin: org' = a + b*org.</summary>
    private double _linkAxisOrgA;
    /// <summary>The value b of x-axis link for link of origin: org' = a + b*org.</summary>
    private double _linkAxisOrgB;
    /// <summary>The value a of x-axis link for link of end: end' = a + b*end.</summary>
    private double _linkAxisEndA;
    /// <summary>The value b of x-axis link for link of end: end' = a + b*end.</summary>
    private double _linkAxisEndB;

    /// <summary>
    /// Fired if the axis changed or the axis boundaries changed.
    /// </summary>
    public event EventHandler AxisInstanceChanged;
    /// <summary>
    /// Fired if the axis properties changed.
    /// </summary>
    public event EventHandler AxisPropertiesChanged;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayerAxisProperties), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotLayerAxisProperties s = (XYPlotLayerAxisProperties)obj;
       

        info.AddValue("Axis", s._axis);
        info.AddValue("Link", s._isLinked);
        info.AddValue("OrgA", s._linkAxisOrgA);
        info.AddValue("OrgB", s._linkAxisOrgB);
        info.AddValue("EndA", s._linkAxisEndA);
        info.AddValue("EndB", s._linkAxisEndB);

      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLayerAxisProperties s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual XYPlotLayerAxisProperties SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLayerAxisProperties s = null != o ? (XYPlotLayerAxisProperties)o : new XYPlotLayerAxisProperties();

        s.Axis = (Scale)info.GetValue("Axis", typeof(Scale));
        s._isLinked = info.GetBoolean("Link");
        s._linkAxisOrgA = info.GetDouble("OrgA");
        s._linkAxisOrgB = info.GetDouble("OrgB");
        s._linkAxisEndA = info.GetDouble("EndA");
        s._linkAxisEndB = info.GetDouble("EndB");
       
        return s;
      }
    }
    #endregion


    public XYPlotLayerAxisProperties()
    {
      Axis = new LinearScale();
      _isLinked = false;
      _linkAxisOrgA = 0;
      _linkAxisOrgB = 1;
      _linkAxisEndA = 0;
      _linkAxisEndB = 1;
    }
    
    void CopyFrom(XYPlotLayerAxisProperties from)
    {
      this.Axis = from._axis == null ? null : (Scale)from._axis.Clone();
      this._isLinked = from._isLinked;
      this._linkAxisOrgA = from._linkAxisOrgA;
      this._linkAxisOrgB = from._linkAxisOrgB;
      this._linkAxisEndA = from._linkAxisEndA;
      this._linkAxisEndB = from._linkAxisEndB;
    }

    public XYPlotLayerAxisProperties Clone()
    {
      XYPlotLayerAxisProperties result = new XYPlotLayerAxisProperties();
      result.CopyFrom(this);
      return result;
    }

    public bool IsLinked
    {
      get { return _isLinked; }
      set
      {
        bool oldValue = _isLinked;
        _isLinked = value;
        _axis.IsLinked = value;

        if (value != oldValue && value == true)
        {
          // simulate the event, that the axis has changed
          this.OnAxisInstanceChanged();  // this will cause the axis to update with the linked axis
        }
      }
    }

    /// <summary>The type of x axis link.</summary>
    /// <value>Can be either None, Straight or Custom link.</value>
    public AxisLinkType AxisLinkType
    {
      get
      {
        if (!IsLinked)
          return AxisLinkType.None;
        else if (LinkAxisOrgA == 0 && LinkAxisOrgB == 1 && LinkAxisEndA == 0 && LinkAxisEndB == 1)
          return AxisLinkType.Straight;
        else return AxisLinkType.Custom;
      }
      set
      {
        if (value == AxisLinkType.None)
        {
          IsLinked = false;
        }
        else
        {
          if (value == AxisLinkType.Straight)
          {
            _linkAxisOrgA = 0;
            _linkAxisOrgB = 1;
            _linkAxisEndA = 0;
            _linkAxisEndB = 1;
          }

          IsLinked = true;
        }
      }
    }


    /// <summary>
    /// Set all parameters of the axis link by once.
    /// </summary>
    /// <param name="linktype">The type of the axis link, i.e. None, Straight or Custom.</param>
    /// <param name="orgA">The value a of x-axis link for link of axis origin: org' = a + b*org.</param>
    /// <param name="orgB">The value b of x-axis link for link of axis origin: org' = a + b*org.</param>
    /// <param name="endA">The value a of x-axis link for link of axis end: end' = a + b*end.</param>
    /// <param name="endB">The value b of x-axis link for link of axis end: end' = a + b*end.</param>
    public void SetAxisLinkParameter(AxisLinkType linktype, double orgA, double orgB, double endA, double endB)
    {
      if (linktype == AxisLinkType.Straight)
      {
        orgA = 0;
        orgB = 1;
        endA = 0;
        endB = 1;
      }

      bool linkaxis = (linktype != AxisLinkType.None);

      if (
        (linkaxis != this.IsLinked) ||
        (orgA != this.LinkAxisOrgA) ||
        (orgB != this.LinkAxisOrgB) ||
        (endA != this.LinkAxisEndA) ||
        (endB != this.LinkAxisEndB))
      {
        this._isLinked = linkaxis;
        this._linkAxisOrgA = orgA;
        this._linkAxisOrgB = orgB;
        this._linkAxisEndA = endA;
        this._linkAxisEndB = endB;

        if (IsLinked)
          OnAxisInstanceChanged();
      }
    }

    public double LinkAxisOrgA
    {
      get { return _linkAxisOrgA; }
      set
      {
        _linkAxisOrgA = value;
        if (_isLinked)
          OnAxisInstanceChanged();
      }
    }



    public double LinkAxisOrgB
    {
      get { return _linkAxisOrgB; }
      set
      {
        _linkAxisOrgB = value;
        if (_isLinked)
          OnAxisInstanceChanged();
      }
    }



    public double LinkAxisEndA
    {
      get { return _linkAxisEndA; }
      set
      {
        _linkAxisEndA = value;
        if (_isLinked)
          OnAxisInstanceChanged();
      }
    }



    public double LinkAxisEndB
    {
      get { return _linkAxisEndB; }
      set
      {
        _linkAxisEndB = value;
        if (_isLinked)
          OnAxisInstanceChanged();
      }
    }

    public Scale Axis
    {
      get
      {
        return _axis;
      }
      set
      {
        Scale oldvalue = _axis;
        _axis = value;
        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (null != oldvalue)
          {
            oldvalue.Changed -= new EventHandler(this.EhAxisPropertiesChanged);
            oldvalue.IsLinked = false;
          }
          if (null != value)
          {
            value.Changed += new EventHandler(this.EhAxisPropertiesChanged);
            value.IsLinked = this._isLinked;
          }

          OnAxisInstanceChanged();
        }
      }
    }

    void EhAxisPropertiesChanged(object sender, EventArgs e)
    {
      OnAxisPropertiesChanged();
    }

    /// <summary>
    /// Measures if the linked axis has changed.
    /// </summary>
    /// <param name="linkedAxis">The axis that is the master axis (our axis is linked to this axis).</param>
    public void EhLinkedLayerAxesChanged(Scale linkedAxis)
    {
      if (_isLinked)
      {
        // we must disable our own interrogator because otherwise we can not change the axis
        _axis.IsLinked = false;
        _axis.ProcessDataBounds(
          LinkAxisOrgA + LinkAxisOrgB * linkedAxis.OrgAsVariant, true,
          LinkAxisEndA + LinkAxisEndB * linkedAxis.EndAsVariant, true);
        _axis.IsLinked = true; // restore the linked state of the axis

        this.OnAxisPropertiesChanged(); // indicate that the axes boundaries have changed

      }
    }


    protected virtual void OnAxisInstanceChanged()
    {
      if (AxisInstanceChanged != null)
        AxisInstanceChanged(this, EventArgs.Empty);
    }

    protected virtual void OnAxisPropertiesChanged()
    {
      if (AxisPropertiesChanged != null)
        AxisPropertiesChanged(this, EventArgs.Empty);
    }

  }

  public class XYPlotLayerAxisPropertiesCollection : Main.IChangedEventSource
  {
    XYPlotLayerAxisProperties[] _props = new XYPlotLayerAxisProperties[2];

    /// <summary>
    /// Fired if one of the axis has changed (or its boundaries).
    /// </summary>
    public event EventHandler AxesChanged;

    /// <summary>
    /// Fired if something in this class or in its child has changed.
    /// </summary>
    public event EventHandler Changed;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayerAxisPropertiesCollection), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotLayerAxisPropertiesCollection s = (XYPlotLayerAxisPropertiesCollection)obj;

        info.CreateArray("Properties", s._props.Length);
        for (int i = 0; i < s._props.Length;++i )
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
          s.SetAxisProperties( (XYPlotLayerAxisProperties)info.GetValue("e", s),i);
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
          if(_props[i]!=null)
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
