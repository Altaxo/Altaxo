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
    [field:NonSerialized]
    public event EventHandler AxisInstanceChanged;
    /// <summary>
    /// Fired if the axis properties changed.
    /// </summary>
    [field:NonSerialized]
    public event EventHandler AxisPropertiesChanged;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLayerAxisProperties", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayerAxisProperties), 1)]
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
    public ScaleLinkType AxisLinkType
    {
      get
      {
        if (!IsLinked)
          return ScaleLinkType.None;
        else if (LinkAxisOrgA == 0 && LinkAxisOrgB == 1 && LinkAxisEndA == 0 && LinkAxisEndB == 1)
          return ScaleLinkType.Straight;
        else return ScaleLinkType.Custom;
      }
      set
      {
        if (value == ScaleLinkType.None)
        {
          IsLinked = false;
        }
        else
        {
          if (value == ScaleLinkType.Straight)
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
    public void SetAxisLinkParameter(ScaleLinkType linktype, double orgA, double orgB, double endA, double endB)
    {
      if (linktype == ScaleLinkType.Straight)
      {
        orgA = 0;
        orgB = 1;
        endA = 0;
        endB = 1;
      }

      bool linkaxis = (linktype != ScaleLinkType.None);

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
}
