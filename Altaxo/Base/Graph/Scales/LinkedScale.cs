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
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Scales
{
  [Serializable]
  public class LinkedScale
  {
    /// <summary>
    /// The axis.
    /// </summary>
    private Scale _scale; // the X-Axis

    /// <summary>Indicate if x-axis is linked to the linked layer x axis.</summary>
    private bool _isLinked;

    /// <summary>The value a of x-axis link for link of origin: org' = a + b*org.</summary>
    private double _linkOrgA;
    /// <summary>The value b of x-axis link for link of origin: org' = a + b*org.</summary>
    private double _linkOrgB;
    /// <summary>The value a of x-axis link for link of end: end' = a + b*end.</summary>
    private double _linkEndA;
    /// <summary>The value b of x-axis link for link of end: end' = a + b*end.</summary>
    private double _linkEndB;

    /// <summary>
    /// Fired if the axis changed or the axis boundaries changed.
    /// </summary>
    [field:NonSerialized]
    public event EventHandler ScaleInstanceChanged;
    /// <summary>
    /// Fired if the axis properties changed.
    /// </summary>
    [field:NonSerialized]
    public event EventHandler LinkPropertiesChanged;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLayerAxisProperties", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinkedScale), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LinkedScale s = (LinkedScale)obj;
       

        info.AddValue("Axis", s._scale);
        info.AddValue("Link", s._isLinked);
        info.AddValue("OrgA", s._linkOrgA);
        info.AddValue("OrgB", s._linkOrgB);
        info.AddValue("EndA", s._linkEndA);
        info.AddValue("EndB", s._linkEndB);

      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LinkedScale s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual LinkedScale SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LinkedScale s = null != o ? (LinkedScale)o : new LinkedScale();

        s.Scale = (Scale)info.GetValue("Axis", typeof(Scale));
        s._isLinked = info.GetBoolean("Link");
        s._linkOrgA = info.GetDouble("OrgA");
        s._linkOrgB = info.GetDouble("OrgB");
        s._linkEndA = info.GetDouble("EndA");
        s._linkEndB = info.GetDouble("EndB");
       
        return s;
      }
    }
    #endregion



    public LinkedScale()
    {
      Scale = new LinearScale();
      _isLinked = false;
      _linkOrgA = 0;
      _linkOrgB = 1;
      _linkEndA = 0;
      _linkEndB = 1;
    }
    
    void CopyFrom(LinkedScale from)
    {
      this.Scale = from._scale == null ? null : (Scale)from._scale.Clone();
      this._isLinked = from._isLinked;
      this._linkOrgA = from._linkOrgA;
      this._linkOrgB = from._linkOrgB;
      this._linkEndA = from._linkEndA;
      this._linkEndB = from._linkEndB;
    }

    public LinkedScale Clone()
    {
      LinkedScale result = new LinkedScale();
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
        _scale.IsLinked = value;

        if (value != oldValue && value == true)
        {
          // simulate the event, that the axis has changed
          this.OnScaleInstanceChanged();  // this will cause the axis to update with the linked axis
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
        else if (LinkOrgA == 0 && LinkOrgB == 1 && LinkEndA == 0 && LinkEndB == 1)
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
            _linkOrgA = 0;
            _linkOrgB = 1;
            _linkEndA = 0;
            _linkEndB = 1;
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
    public void SetLinkParameter(ScaleLinkType linktype, double orgA, double orgB, double endA, double endB)
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
        (orgA != this.LinkOrgA) ||
        (orgB != this.LinkOrgB) ||
        (endA != this.LinkEndA) ||
        (endB != this.LinkEndB))
      {
        this._isLinked = linkaxis;
        this._linkOrgA = orgA;
        this._linkOrgB = orgB;
        this._linkEndA = endA;
        this._linkEndB = endB;

        if (IsLinked)
          OnScaleInstanceChanged();
      }
    }

    public double LinkOrgA
    {
      get { return _linkOrgA; }
      set
      {
        _linkOrgA = value;
        if (_isLinked)
          OnScaleInstanceChanged();
      }
    }



    public double LinkOrgB
    {
      get { return _linkOrgB; }
      set
      {
        _linkOrgB = value;
        if (_isLinked)
          OnScaleInstanceChanged();
      }
    }



    public double LinkEndA
    {
      get { return _linkEndA; }
      set
      {
        _linkEndA = value;
        if (_isLinked)
          OnScaleInstanceChanged();
      }
    }



    public double LinkEndB
    {
      get { return _linkEndB; }
      set
      {
        _linkEndB = value;
        if (_isLinked)
          OnScaleInstanceChanged();
      }
    }

    public Scale Scale
    {
      get
      {
        return _scale;
      }
      set
      {
        Scale oldvalue = _scale;
        _scale = value;
        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (null != oldvalue)
          {
            oldvalue.Changed -= new EventHandler(this.EhScaleChanged);
            oldvalue.IsLinked = false;
          }
          if (null != value)
          {
            value.Changed += new EventHandler(this.EhScaleChanged);
            value.IsLinked = this._isLinked;
          }

          OnScaleInstanceChanged();
        }
      }
    }

    void EhScaleChanged(object sender, EventArgs e)
    {
      OnLinkPropertiesChanged();
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
        _scale.IsLinked = false;
        _scale.ProcessDataBounds(
          LinkOrgA + LinkOrgB * linkedAxis.OrgAsVariant, true,
          LinkEndA + LinkEndB * linkedAxis.EndAsVariant, true);
        _scale.IsLinked = true; // restore the linked state of the axis

        this.OnLinkPropertiesChanged(); // indicate that the axes boundaries have changed

      }
    }


    protected virtual void OnScaleInstanceChanged()
    {
      if (ScaleInstanceChanged != null)
        ScaleInstanceChanged(this, EventArgs.Empty);
    }

    protected virtual void OnLinkPropertiesChanged()
    {
      if (LinkPropertiesChanged != null)
        LinkPropertiesChanged(this, EventArgs.Empty);
    }

  }
}
