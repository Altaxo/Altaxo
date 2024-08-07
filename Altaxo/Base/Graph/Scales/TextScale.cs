﻿#region Copyright

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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Scales.Rescaling;

namespace Altaxo.Graph.Scales
{
  [Serializable]
  [DisplayName("${res:ClassNames.Altaxo.Graph.Scales.TextScale}")]
  public class TextScale : Scale
  {
    /// <summary>Holds the <see cref="TextBoundaries"/> for that axis.</summary>
    protected TextBoundaries _dataBounds;

    protected NumericScaleRescaleConditions _rescaling;

    protected Ticks.TickSpacing _tickSpacing;

    // cached values
    /// <summary>Current axis origin (cached value).</summary>
    protected double _cachedAxisOrg = 0;

    /// <summary>Current axis end (cached value).</summary>
    protected double _cachedAxisEnd = 1;

    /// <summary>Current axis span (i.e. end-org) (cached value).</summary>
    protected double _cachedAxisSpan = 1;

    /// <summary>Current inverse of axis span (cached value).</summary>
    protected double _cachedOneByAxisSpan = 1;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.TextScale", 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
                TextScale s = (TextScale)obj;

                info.AddValue("Org", s._cachedAxisOrg);
                info.AddValue("End", s._cachedAxisEnd);

                info.AddValue("Rescaling", s._rescaling);
                info.AddValue("Bounds", s._dataBounds);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (TextScale?)o ?? new TextScale(info);

        s._cachedAxisOrg = info.GetDouble("Org");
        s._cachedAxisEnd = info.GetDouble("End");
        s._cachedAxisSpan = s._cachedAxisEnd - s._cachedAxisOrg;
        s._cachedOneByAxisSpan = 1 / s._cachedAxisSpan;

        s.ChildSetMember(ref s._rescaling, (NumericScaleRescaleConditions)info.GetValue("Rescaling", s));
        s.ChildSetMember(ref s._dataBounds, (TextBoundaries)info.GetValue("Bounds", s));
        s.ChildSetMember(ref s._tickSpacing, new Ticks.TextTickSpacing());

        s.EhChildChanged(s._dataBounds, EventArgs.Empty); // for this old version, rescaling is not fully serialized, thus we have to simulate a DataBoundChanged event to get _rescaling updated, and finally _tickSpacing updated

        return s;
      }
    }

    /// <summary>
    /// 2015-02-13 Added TickSpacing
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextScale), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TextScale)obj;

        info.AddValue("Org", s._cachedAxisOrg);
        info.AddValue("End", s._cachedAxisEnd);

        info.AddValue("Bounds", s._dataBounds);
        info.AddValue("Rescaling", s._rescaling);
        info.AddValue("TickSpacing", s._tickSpacing);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (TextScale?)o ?? new TextScale(info);

        s._cachedAxisOrg = info.GetDouble("Org");
        s._cachedAxisEnd = info.GetDouble("End");
        s._cachedAxisSpan = s._cachedAxisEnd - s._cachedAxisOrg;
        s._cachedOneByAxisSpan = 1 / s._cachedAxisSpan;

        s.ChildSetMember(ref s._dataBounds, (TextBoundaries)info.GetValue("Bounds", s));
        s.ChildSetMember(ref s._rescaling, (NumericScaleRescaleConditions)info.GetValue("Rescaling", s));
        s.ChildSetMember(ref s._tickSpacing, (Ticks.TickSpacing)info.GetValue("TickSpacing", s));

        s.UpdateTicksAndOrgEndUsingRescalingObject();

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// For deserialization purposes only: initializes a new instance of the <see cref="TextScale"/> class.
    /// </summary>
    /// <param name="info">The information.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected TextScale(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    public TextScale()
    {
      ChildSetMember(ref _dataBounds, new TextBoundaries());
      ChildSetMember(ref _rescaling, new LinearScaleRescaleConditions());
      ChildSetMember(ref _tickSpacing, new Ticks.TextTickSpacing());
      UpdateTicksAndOrgEndUsingRescalingObject();
    }

    public TextScale(TextScale from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_dataBounds), nameof(_rescaling), nameof(_tickSpacing))]
    protected void CopyFrom(TextScale from)
    {
      using (var suspendToken = SuspendGetToken())
      {
        _cachedAxisOrg = from._cachedAxisOrg;
        _cachedAxisEnd = from._cachedAxisEnd;
        _cachedAxisSpan = from._cachedAxisSpan;
        _cachedOneByAxisSpan = from._cachedOneByAxisSpan;

        ChildCopyToMember(ref _dataBounds, from._dataBounds);
        ChildCopyToMember(ref _rescaling, from._rescaling);
        ChildCopyToMember(ref _tickSpacing, from._tickSpacing);

        EhSelfChanged(EventArgs.Empty);
        suspendToken.Resume();
      }
    }

    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;


      if (obj is TextScale from)
      {
        CopyFrom(from);

        return true;
      }

      return false;
    }

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_dataBounds is not null)
        yield return new Main.DocumentNodeAndName(_dataBounds, () => _dataBounds = null!, "DataBounds");
      if (_rescaling is not null)
        yield return new Main.DocumentNodeAndName(_rescaling, () => _rescaling = null!, "Rescaling");
      if (_tickSpacing is not null)
        yield return new Main.DocumentNodeAndName(_tickSpacing, () => _tickSpacing = null!, "TickSpacing");
    }

    public override object Clone()
    {
      return new TextScale(this);
    }

    public override double PhysicalVariantToNormal(Altaxo.Data.AltaxoVariant x)
    {
      if (x.IsType(Altaxo.Data.AltaxoVariant.Content.VString))
      {
        int idx = _dataBounds.IndexOf(x.ToString());
        return idx < 0 ? double.NaN : (1 + idx - _cachedAxisOrg) * _cachedOneByAxisSpan;
      }
      else if (x.CanConvertedToDouble)
      {
        return (x.ToDouble() - _cachedAxisOrg) * _cachedOneByAxisSpan;
      }
      else
      {
        return double.NaN;
      }
    }

    public override Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x)
    {
      return new AltaxoVariant(_cachedAxisOrg + x * _cachedAxisSpan);
    }

    public override IScaleRescaleConditions RescalingObject
    {
      get
      {
        return _rescaling;
      }
    }

    public override IPhysicalBoundaries DataBoundsObject
    {
      get { return _dataBounds; }
    }

    public override Ticks.TickSpacing TickSpacing
    {
      get
      {
        return _tickSpacing;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException();

        if (ChildSetMember(ref _tickSpacing, value))
          EhChildChanged(RescalingObject, EventArgs.Empty);
      }
    }

    public override Altaxo.Data.AltaxoVariant OrgAsVariant
    {
      get
      {
        return new AltaxoVariant(_cachedAxisOrg);
      }
    }

    public override Altaxo.Data.AltaxoVariant EndAsVariant
    {
      get
      {
        return new AltaxoVariant(_cachedAxisEnd);
      }
    }

    protected override string? SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
    {
      double o = org.ToDouble();
      double e = end.ToDouble();

      if (!(o < e))
        return "org is not less than end";

      InternalSetOrgEnd(o, e);

      return null;
    }

    private void InternalSetOrgEnd(double org, double end)
    {
      bool changed = _cachedAxisOrg != org || _cachedAxisEnd != end;

      _cachedAxisOrg = org;
      _cachedAxisEnd = end;
      _cachedAxisSpan = end - org;
      _cachedOneByAxisSpan = 1 / _cachedAxisSpan;

      if (changed)
        EhSelfChanged(EventArgs.Empty);
    }

    public override void OnUserRescaled()
    {
      _rescaling.OnUserRescaled();
    }

    public override void OnUserZoomed(AltaxoVariant newZoomOrg, AltaxoVariant newZoomEnd)
    {
      _rescaling.OnUserZoomed(newZoomOrg, newZoomEnd);
    }

    #region Changed event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (object.ReferenceEquals(sender, _dataBounds)) // Data bounds have changed
      {
        double xorg = 0;
        double xend = 1;
        if (_dataBounds is not null && !_dataBounds.IsEmpty)
        {
          xorg = 0.5;
          xend = _dataBounds.NumberOfItems + 0.5;
        }

        var rescalingHasChanged = _rescaling.OnDataBoundsChanged(xorg, xend);
        if (!rescalingHasChanged) // Note: the other case (rescaling has changed) is handled in HandleHighPriorityChildChangeCases
        {
          // even if the data bounds have not changed, we must force the update of the ticks.
          // Example: the mutual exchange of two elements in a text column will not change the data bounds, but the order of our ticks __will__ change!!
          UpdateTicksAndOrgEndUsingRescalingObject();
        }

        e = EventArgs.Empty;
        return false;
      }
      else if (object.ReferenceEquals(sender, _rescaling)) // Rescaling has changed
      {
        UpdateTicksAndOrgEndUsingRescalingObject();
      }
      else if (object.ReferenceEquals(sender, _tickSpacing))
      {
        UpdateTicksAndOrgEndUsingRescalingObject();
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    protected void UpdateTicksAndOrgEndUsingRescalingObject()
    {
      if (TickSpacing is null)
      {
        SetScaleOrgEnd(_rescaling.ResultingOrg, _rescaling.ResultingEnd);
      }
      else
      {
        AltaxoVariant org = _rescaling.ResultingOrg, end = _rescaling.ResultingEnd;
        TickSpacing.PreProcessScaleBoundaries(ref org, ref end, !_rescaling.IsResultingOrgFixed, !_rescaling.IsResultingEndFixed);
        SetScaleOrgEnd(org, end);
        TickSpacing.FinalProcessScaleBoundaries(org, end, this);
      }
    }

    #endregion Changed event handling
  }
}
