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
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Main;

namespace Altaxo.Graph.Scales
{
  /// <summary>
  /// Represents a logarithmic axis, i.e. the physical values v correspond to logical values l by v=a*10^(b*l).
  /// </summary>
  [Serializable]
  [DisplayName("${res:ClassNames.Altaxo.Graph.Scales.Log10Scale}")]
  public class Log10Scale : NumericalScale
  {
    /// <summary>Decimal logarithm of axis org. Should always been set together with <see cref="_cachedOrg"/>.</summary>
    private double _log10Org = 0; // Log10 of physical axis org

    /// <summary>Origin of the axis. This value is used to maintain numeric precision. Should always been set together with <see cref="_log10Org"/>.</summary>
    private double _cachedOrg = 1;

    /// <summary>Decimal logarithm of axis end.  Should always been set together with <see cref="_cachedEnd"/>.</summary>
    private double _log10End = 1; // Log10 of physical axis end

    /// <summary>Value of the end of the axis. This value is used to maintain numeric precision. Should always been set together with <see cref="_log10End"/>.</summary>
    private double _cachedEnd = 10;

    /// <summary>The boundary object. It collectes only positive values for the axis is logarithmic.</summary>
    protected NumericalBoundaries _dataBounds;

    protected LogarithmicScaleRescaleConditions _rescaling;

    protected Ticks.TickSpacing _tickSpacing;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.Log10Scale", 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ArgumentOutOfRangeException("Serialization of old version");
        /*
                Log10Scale s = (Log10Scale)obj;
                info.AddValue("Log10Org", s._log10Org);
                info.AddValue("Log10End", s._log10End);

                info.AddValue("Rescaling", s._rescaling);

                info.AddValue("Bounds", s._dataBounds);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (Log10Scale?)o ?? new Log10Scale(info);

        s._log10Org = info.GetDouble("Log10Org");
        s._cachedOrg = Math.Pow(10, s._log10Org);

        s._log10End = info.GetDouble("Log10End");
        s._cachedEnd = Math.Pow(10, s._log10End);
        s._rescaling = (LogarithmicScaleRescaleConditions)info.GetValue("Rescaling", s);
        s._rescaling.ParentObject = s;

        s._dataBounds = (PositiveFiniteNumericalBoundaries)info.GetValue("Bounds", s);
        s._dataBounds.ParentObject = s;

        s._tickSpacing = new Ticks.Log10TickSpacing
        {
          ParentObject = s
        };

        s.EhChildChanged(s._dataBounds, EventArgs.Empty); // for this old version, rescaling is not fully serialized, thus we have to simulate a DataBoundChanged event to get _rescaling updated, and finally _tickSpacing updated

        return s;
      }
    }

    /// <summary>
    /// 2015-02-13 Added TickSpacing
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Log10Scale), 4)]
    private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Log10Scale)obj;
        info.AddValue("Log10Org", s._log10Org);

        info.AddValue("Log10End", s._log10End);

        info.AddValue("Bounds", s._dataBounds);

        info.AddValue("Rescaling", s._rescaling);

        info.AddValue("TickSpacing", s._tickSpacing);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (Log10Scale?)o ?? new Log10Scale(info);

        s._log10Org = info.GetDouble("Log10Org");
        s._cachedOrg = Math.Pow(10, s._log10Org);

        s._log10End = info.GetDouble("Log10End");
        s._cachedEnd = Math.Pow(10, s._log10End);

        s._dataBounds = (PositiveFiniteNumericalBoundaries)info.GetValue("Bounds", s);
        s._dataBounds.ParentObject = s;

        s._rescaling = (LogarithmicScaleRescaleConditions)info.GetValue("Rescaling", s);
        s._rescaling.ParentObject = s;

        s._tickSpacing = (Ticks.TickSpacing)info.GetValue("TickSpacing", s);
        s._tickSpacing.ParentObject = s;

        s.UpdateTicksAndOrgEndUsingRescalingObject();

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Constructor for deserialization only.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected Log10Scale(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
    }

    /// <summary>
    /// Creates a default logarithmic axis with org=1 and end=10.
    /// </summary>
    public Log10Scale()
    {
      _dataBounds = new PositiveFiniteNumericalBoundaries() { ParentObject = this };
      _rescaling = new LogarithmicScaleRescaleConditions() { ParentObject = this };
      _tickSpacing = new Ticks.Log10TickSpacing() { ParentObject = this };
      UpdateTicksAndOrgEndUsingRescalingObject();
    }



    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The axis to copy from.</param>
    public Log10Scale(Log10Scale from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_dataBounds), nameof(_rescaling), nameof(_tickSpacing))]
    protected void CopyFrom(Log10Scale from)
    {
      using (var suspendToken = SuspendGetToken())
      {
        _log10Org = from._log10Org;
        _cachedOrg = from._cachedOrg;
        _log10End = from._log10End;
        _cachedEnd = from._cachedEnd;

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

      if (obj is Log10Scale from)
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

    /// <summary>
    /// Creates a clone copy of this axis.
    /// </summary>
    /// <returns>The cloned copy.</returns>
    public override object Clone()
    {
      return new Log10Scale(this);
    }

    /// <summary>
    /// Returns the rescaling conditions for this axis
    /// </summary>
    public override NumericScaleRescaleConditions Rescaling
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

        if (ChildSetMember(ref _tickSpacing, value))
          EhChildChanged(Rescaling, EventArgs.Empty);
      }
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
    public override double PhysicalToNormal(double x)
    {
      if (x <= 0)
        return double.NaN;

      double log10x = Math.Log10(x);
      return (log10x - _log10Org) / (_log10End - _log10Org);
    }

    /// <summary>
    /// NormalToPhysical is the inverse function to PhysicalToNormal
    /// It translates a normalized value (0 for the axis origin, 1 for the axis end)
    /// into the physical value
    /// </summary>
    /// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
    /// <returns>the corresponding physical value</returns>
    public override double NormalToPhysical(double x)
    {
      double log10x = _log10Org + (_log10End - _log10Org) * x;
      return Math.Pow(10, log10x);
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
      return PhysicalToNormal(x.ToDouble());
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

    public override NumericalBoundaries DataBounds
    {
      get { return _dataBounds; }
    } // return a PhysicalBoundarie object that is associated with that axis

    public override double Org
    {
      get { return _cachedOrg; }
    }

    public override double End
    {
      get { return _cachedEnd; }
    }

    private void HandleInvalidOrgOrEnd(ref double org, ref double end)
    {
      if (org > 0)
      {
        end = org * 10;
      }
      else if (end > 0)
      {
        org = end / 10;
      }
      else
      {
        org = 1;
        end = 10;
      }
    }

    private void InternalSetOrgEnd(double org, double end)
    {
      double lgorg = Math.Log10(org);
      double lgend = Math.Log10(end);

      _cachedOrg = org;
      _cachedEnd = end;

      bool changed = _log10Org != lgorg ||
        _log10End != lgend;

      _log10Org = lgorg;
      _log10End = lgend;

      if (changed)
        EhSelfChanged(EventArgs.Empty);
    }

    protected override string? SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
    {
      double o = org.ToDouble();
      double e = end.ToDouble();

      if (!(o < e))
        return "org is not less than end";
      if (!(o > 0))
        return "org is not positive";
      if (!(e > 0))
        return "end is not positive";

      InternalSetOrgEnd(o, e);
      return null;
    }
  } // end of class Log10Axis
}
