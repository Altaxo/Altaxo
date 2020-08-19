#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Graph.Scales.Deprecated
{
  [Serializable]
  public class LinkedScale : Main.SuspendableDocumentNodeWithSetOfEventArgs
  {
    /// <summary>
    /// The axis.
    /// </summary>
    private Scale _scale; // the X-Axis

    /// <summary>The value a of x-axis link for link of origin: org' = a + b*org.</summary>
    private double _linkOrgA;

    /// <summary>The value b of x-axis link for link of origin: org' = a + b*org.</summary>
    private double _linkOrgB;

    /// <summary>The value a of x-axis link for link of end: end' = a + b*end.</summary>
    private double _linkEndA;

    /// <summary>The value b of x-axis link for link of end: end' = a + b*end.</summary>
    private double _linkEndB;

    private bool _isLinked;

    /// <summary>
    /// Fired if the axis changed or the axis boundaries changed.
    /// </summary>
    [field: NonSerialized]
    public event EventHandler? ScaleInstanceChanged;

    /// <summary>
    /// Fired if the axis properties changed.
    /// </summary>
    [field: NonSerialized]
    public event EventHandler? LinkPropertiesChanged;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerAxisProperties", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.LinkedScale", 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinkedScale)obj;

        info.AddValue("Axis", s._scale);
        info.AddValue("Link", s._isLinked);
        info.AddValue("OrgA", s._linkOrgA);
        info.AddValue("OrgB", s._linkOrgB);
        info.AddValue("EndA", s._linkEndA);
        info.AddValue("EndB", s._linkEndB);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LinkedScale?)o ?? new LinkedScale();

        s.Scale = (Scale)info.GetValue("Axis", s);
        s._isLinked = info.GetBoolean("Link");
        s._linkOrgA = info.GetDouble("OrgA");
        s._linkOrgB = info.GetDouble("OrgB");
        s._linkEndA = info.GetDouble("EndA");
        s._linkEndB = info.GetDouble("EndB");

        return s;
      }


    }

    #endregion Serialization

    public LinkedScale()
    {
      Scale = new LinearScale();
      _linkOrgA = 0;
      _linkOrgB = 1;
      _linkEndA = 0;
      _linkEndB = 1;
    }

    private void CopyFrom(LinkedScale from)
    {
      if (object.ReferenceEquals(this, from))
        return;

      Scale = (Scale)from._scale.Clone();
      _linkOrgA = from._linkOrgA;
      _linkOrgB = from._linkOrgB;
      _linkEndA = from._linkEndA;
      _linkEndB = from._linkEndB;
    }

    public LinkedScale Clone()
    {
      var result = new LinkedScale();
      result.CopyFrom(this);
      return result;
    }

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _scale)
        yield return new Main.DocumentNodeAndName(_scale, "Scale");
    }

    /// <summary>The type of x axis link.</summary>
    /// <value>Can be either None, Straight or Custom link.</value>
    public ScaleLinkType AxisLinkType
    {
      get
      {
        if (!_isLinked)
          return ScaleLinkType.None;
        else if (LinkOrgA == 0 && LinkOrgB == 1 && LinkEndA == 0 && LinkEndB == 1)
          return ScaleLinkType.Straight;
        else
          return ScaleLinkType.Custom;
      }
      set
      {
        if (value == ScaleLinkType.Straight)
        {
          _linkOrgA = 0;
          _linkOrgB = 1;
          _linkEndA = 0;
          _linkEndB = 1;
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

      if (
        (orgA != LinkOrgA) ||
        (orgB != LinkOrgB) ||
        (endA != LinkEndA) ||
        (endB != LinkEndB))
      {
        _linkOrgA = orgA;
        _linkOrgB = orgB;
        _linkEndA = endA;
        _linkEndB = endB;

        OnScaleInstanceChanged();
      }
    }

    public double LinkOrgA
    {
      get { return _linkOrgA; }
      set
      {
        _linkOrgA = value;

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

        OnScaleInstanceChanged();
      }
    }

    public Scale Scale
    {
      get
      {
        return _scale;
      }
      [MemberNotNull(nameof(_scale))]
      set
      {
        var oldvalue = _scale;
        _scale = value;
        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (null != oldvalue)
          {
            oldvalue.Changed -= new EventHandler(EhScaleChanged);
            oldvalue.IsLinked = false;
          }
          if (null != value)
          {
            value.Changed += new EventHandler(EhScaleChanged);
          }

          OnScaleInstanceChanged();
        }
      }
    }

    private void EhScaleChanged(object? sender, EventArgs e)
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

        OnLinkPropertiesChanged(); // indicate that the axes boundaries have changed
      }
    }

    protected virtual void OnScaleInstanceChanged()
    {
      ScaleInstanceChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnLinkPropertiesChanged()
    {
      LinkPropertiesChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
