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
using Altaxo.Data;

namespace Altaxo.Graph.Scales
{
  using System.Diagnostics.CodeAnalysis;
  using Boundaries;
  using Rescaling;

  /// <summary>
  /// Summary description for DateTimeAxis.
  /// </summary>
  [Serializable]
  public class DateTimeScale : Scale
  {
    // cached values
    /// <summary>Current axis origin (cached value).</summary>
    protected DateTime _axisOrg = DateTime.MinValue;

    /// <summary>Current axis end (cached value).</summary>
    protected DateTime _axisEnd = DateTime.MaxValue;

    /// <summary>Holds the <see cref="NumericalBoundaries"/> for that axis.</summary>
    protected FiniteDateTimeBoundaries _dataBounds;

    protected DateTimeScaleRescaleConditions _rescaling;

    protected Ticks.DateTimeTickSpacing _tickSpacing;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.DateTimeScale", 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
                DateTimeScale s = (DateTimeScale)obj;

                info.AddValue("Org", s._axisOrg);
                info.AddValue("End", s._axisEnd);
                info.AddValue("Rescaling", s._rescaling);
                info.AddValue("Bounds", s._dataBounds);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DateTimeScale?)o ?? new DateTimeScale(info);

        s._axisOrg = info.GetDateTime("Org");
        s._axisEnd = info.GetDateTime("End");
        s.ChildSetMember(ref s._rescaling, (DateTimeScaleRescaleConditions)info.GetValue("Rescaling", s));
        s.ChildSetMember(ref s._dataBounds, (FiniteDateTimeBoundaries)info.GetValue("Bounds", s));
        s.ChildSetMember(ref s._tickSpacing, new Ticks.DateTimeTickSpacing());

        s.EhChildChanged(s._dataBounds, EventArgs.Empty); // for this old version, rescaling is not fully serialized, thus we have to simulate a DataBoundChanged event to get _rescaling updated, and finally _tickSpacing updated

        return s;
      }
    }

    /// <summary>
    /// 2015-02-13 Added TickSpacing
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DateTimeScale), 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DateTimeScale)obj;

        info.AddValue("Org", s._axisOrg);
        info.AddValue("End", s._axisEnd);
        info.AddValue("Bounds", s._dataBounds);
        info.AddValue("Rescaling", s._rescaling);
        info.AddValue("TickSpacing", s._tickSpacing);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DateTimeScale?)o ?? new DateTimeScale(info);

        s._axisOrg = info.GetDateTime("Org");
        s._axisEnd = info.GetDateTime("End");
        s.ChildSetMember(ref s._dataBounds, (FiniteDateTimeBoundaries)info.GetValue("Bounds", s));
        s.ChildSetMember(ref s._rescaling, (DateTimeScaleRescaleConditions)info.GetValue("Rescaling", s));
        s.ChildSetMember(ref s._tickSpacing, (Ticks.DateTimeTickSpacing)info.GetValue("TickSpacing", s));

        s.UpdateTicksAndOrgEndUsingRescalingObject();

        return s;
      }
    }

    #endregion Serialization

    #region ICloneable Members

    public DateTimeScale(DateTimeScale from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_dataBounds), nameof(_rescaling), nameof(_tickSpacing))]
    protected void CopyFrom(DateTimeScale from)
    {
      using (var suspendToken = SuspendGetToken())
      {
        _axisOrg = from._axisOrg;
        _axisEnd = from._axisEnd;

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

      if (obj is DateTimeScale from)
      {
        CopyFrom(from);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Constructor for deserialization only.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected DateTimeScale(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    public DateTimeScale()
    {
      _dataBounds = new FiniteDateTimeBoundaries() { ParentObject = this };
      _rescaling = new DateTimeScaleRescaleConditions() { ParentObject = this };
      _tickSpacing = new Ticks.DateTimeTickSpacing() { ParentObject = this };
      UpdateTicksAndOrgEndUsingRescalingObject();
    }



    /// <summary>
    /// Creates a copy of the axis.
    /// </summary>
    /// <returns>The cloned copy of the axis.</returns>
    public override object Clone()
    {
      return new DateTimeScale(this);
    }

    #endregion ICloneable Members

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_dataBounds is not null)
        yield return new Main.DocumentNodeAndName(_dataBounds, () => _dataBounds = null!, "DataBounds");
      if (_rescaling is not null)
        yield return new Main.DocumentNodeAndName(_rescaling, () => _rescaling = null!, "Rescaling");
      if (_tickSpacing is not null)
        yield return new Main.DocumentNodeAndName(_tickSpacing, () => _tickSpacing = null!, "TickSpacing");
    }

    /// <summary>
    /// PhysicalToNormal translates physical values into a normal value linear along the axis
    /// a physical value of the axis origin must return a value of zero
    /// a physical value of the axis end must return a value of one
    /// the function physicalToNormal must be provided by any derived class
    /// </summary>
    /// <param name="x">the physical value</param>
    /// <returns>
    /// the normalized value linear along the axis,
    /// 0 for axis origin, 1 for axis end</returns>
    public double PhysicalToNormal(DateTime x)
    {
      return (x - _axisOrg).TotalSeconds / (_axisEnd - _axisOrg).TotalSeconds;
    }

    /// <summary>
    /// NormalToPhysical is the inverse function to PhysicalToNormal
    /// It translates a normalized value (0 for the axis origin, 1 for the axis end)
    /// into the physical value
    /// </summary>
    /// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
    /// <returns>the corresponding physical value</returns>
    public DateTime NormalToPhysical(double x)
    {
      return _axisOrg.AddSeconds(x * (_axisEnd - _axisOrg).TotalSeconds);
    }

    /// <summary>
    /// PhysicalVariantToNormal translates physical values into a normal value linear along the axis
    /// a physical value of the axis origin must return a value of zero
    /// a physical value of the axis end must return a value of one
    /// the function physicalToNormal must be provided by any derived class
    /// </summary>
    /// <param name="x">the physical value</param>
    /// <returns>
    /// the normalized value linear along the axis,
    /// 0 for axis origin, 1 for axis end</returns>
    public override double PhysicalVariantToNormal(Altaxo.Data.AltaxoVariant x)
    {
      if (x.IsType(AltaxoVariant.Content.VDateTime))
        return PhysicalToNormal(x);
      else if (x.CanConvertedToDouble)
        return PhysicalToNormal(new DateTime((long)(x.ToDouble() * 10000000)));
      else
        throw new ArgumentException("Variant x is neither DateTime nor numeric");
    }

    /// <summary>
    /// NormalToPhysicalVariant is the inverse function to PhysicalToNormal
    /// It translates a normalized value (0 for the axis origin, 1 for the axis end)
    /// into the physical value
    /// </summary>
    /// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
    /// <returns>the corresponding physical value</returns>
    public override Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x)
    {
      return new Altaxo.Data.AltaxoVariant(NormalToPhysical(x));
    }

    public override AltaxoVariant OrgAsVariant
    {
      get
      {
        return new AltaxoVariant(Org);
      }
    }

    public override AltaxoVariant EndAsVariant
    {
      get
      {
        return new AltaxoVariant(End);
      }
    }

    protected override string? SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
    {
      var o = (DateTime)org;
      var e = (DateTime)end;

      if (!(o < e))
        return "org is not less than end";

      InternalSetOrgEnd(o, e, false, false);

      return null;
    }

    private void InternalSetOrgEnd(DateTime org, DateTime end, bool isOrgExtendable, bool isEndExtendable)
    {
      bool changed = _axisOrg != org ||
        _axisEnd != end;

      _axisOrg = org;
      _axisEnd = end;

      if (changed)
        EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Returns the rescaling conditions for this axis
    /// </summary>
    public DateTimeScaleRescaleConditions Rescaling
    {
      get
      {
        return _rescaling;
      }
    }

    /// <summary>
    /// Returns the rescaling conditions for this axis
    /// </summary>
    public override Rescaling.IScaleRescaleConditions RescalingObject
    {
      get
      {
        return _rescaling;
      }
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
        if (!(value is Ticks.DateTimeTickSpacing))
          throw new ArgumentException("Value must be of type DateTimeTickSpacing");

        if (ChildSetMember(ref _tickSpacing, (Ticks.DateTimeTickSpacing)value))
          EhChildChanged(Rescaling, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Returns the <see cref="FiniteDateTimeBoundaries" /> object that is associated with that axis.
    /// </summary>
    public FiniteDateTimeBoundaries DataBounds
    {
      get
      {
        return _dataBounds;
      }
    } // return a PhysicalBoundarie object that is associated with that axis

    /// <summary>
    /// Returns the <see cref="IPhysicalBoundaries"/> object that is associated with that axis.
    /// </summary>
    public override IPhysicalBoundaries DataBoundsObject
    {
      get
      {
        return _dataBounds;
      }
    } // return a PhysicalBoundarie object that is associated with that axis

    /// <summary>The axis origin, i.e. the first point in physical units.</summary>
    public DateTime Org
    {
      get
      {
        return _axisOrg;
      }
    }

    /// <summary>The axis end point in physical units.</summary>
    public DateTime End
    {
      get
      {
        return _axisEnd;
      }
    }

    public override void OnUserRescaled()
    {
      Rescaling.OnUserRescaled();
    }

    public override void OnUserZoomed(AltaxoVariant newZoomOrg, AltaxoVariant newZoomEnd)
    {
      _rescaling.OnUserZoomed(newZoomOrg, newZoomEnd);
    }

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (object.ReferenceEquals(sender, DataBounds)) // Data bounds have changed
      {
        if (!DataBounds.IsEmpty)
          Rescaling.OnDataBoundsChanged(DataBounds.LowerBound, DataBounds.UpperBound);
        return false; // no need to handle DataBounds changed further, only if rescaling is changed there is need to do something
      }
      else if (object.ReferenceEquals(sender, Rescaling)) // Rescaling has changed
      {
        UpdateTicksAndOrgEndUsingRescalingObject();
        // Fall through
      }
      else if (object.ReferenceEquals(sender, TickSpacing))
      {
        UpdateTicksAndOrgEndUsingRescalingObject();
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    protected virtual void AdjustResultingOrgEndToValidValues(ref DateTime resultingOrg, ref DateTime resultingEnd)
    {
      if (resultingEnd < resultingOrg)
      {
        var h = resultingOrg;
        resultingOrg = resultingEnd;
        resultingEnd = h;
      }

      if (resultingOrg == resultingEnd)
      {
        if ((resultingOrg - DateTime.MinValue) >= TimeSpan.FromDays(1))
          resultingOrg = resultingOrg - TimeSpan.FromDays(1);
        else
          resultingOrg = DateTime.MinValue;

        if ((DateTime.MaxValue - resultingEnd) >= TimeSpan.FromDays(1))
          resultingEnd = resultingEnd + TimeSpan.FromDays(1);
        else
          resultingEnd = DateTime.MaxValue;
      }
    }

    protected virtual void UpdateTicksAndOrgEndUsingRescalingObject()
    {
      DateTime org = Rescaling.ResultingOrg, end = Rescaling.ResultingEnd;
      AdjustResultingOrgEndToValidValues(ref org, ref end);

      if (TickSpacing is null)
      {
        SetScaleOrgEnd(org, end);
      }
      else
      {
        AltaxoVariant orgV = org, endV = end;
        TickSpacing.PreProcessScaleBoundaries(ref orgV, ref endV, !Rescaling.IsResultingOrgFixed, !Rescaling.IsResultingEndFixed);
        SetScaleOrgEnd(orgV, endV);
        TickSpacing.FinalProcessScaleBoundaries(orgV, endV, this);
      }
    }
  } // end of class
}
