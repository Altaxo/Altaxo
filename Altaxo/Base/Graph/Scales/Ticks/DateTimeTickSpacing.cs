#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Calc;
using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
  public class DateTimeTickSpacing : TickSpacing
  {
    #region Inner classes

    public enum TimeSpanExUnit { Span, Month, Years }

    public struct TimeSpanEx : IEquatable<TimeSpanEx>, IEquatable<object>
    {
      public TimeSpanExUnit _unit;
      public TimeSpan _span;

      public static TimeSpanEx FromTicks(long ticks)
      {
        return new TimeSpanEx { _unit = TimeSpanExUnit.Span, _span = TimeSpan.FromTicks(ticks) };
      }

      public static TimeSpanEx FromTimeSpan(TimeSpan span)
      {
        return new TimeSpanEx { _unit = TimeSpanExUnit.Span, _span = span };
      }

      public static TimeSpanEx FromYears(long years)
      {
        return new TimeSpanEx { _unit = TimeSpanExUnit.Years, _span = TimeSpan.FromTicks(years) };
      }

      public static TimeSpanEx FromMonths(long months)
      {
        return new TimeSpanEx { _unit = TimeSpanExUnit.Month, _span = TimeSpan.FromTicks(months) };
      }

      public static TimeSpanEx FromDays(long days)
      {
        return new TimeSpanEx() { _unit = TimeSpanExUnit.Span, _span = TimeSpan.FromDays(days) };
      }

      public static TimeSpanEx FromHours(long hours)
      {
        return new TimeSpanEx() { _unit = TimeSpanExUnit.Span, _span = TimeSpan.FromHours(hours) };
      }

      public static TimeSpanEx FromMinutes(long minutes)
      {
        return new TimeSpanEx() { _unit = TimeSpanExUnit.Span, _span = TimeSpan.FromMinutes(minutes) };
      }

      public static TimeSpanEx FromSeconds(long seconds)
      {
        return new TimeSpanEx() { _unit = TimeSpanExUnit.Span, _span = TimeSpan.FromSeconds(seconds) };
      }

      public static TimeSpanEx FromMilliSeconds(long milliseconds)
      {
        return new TimeSpanEx() { _unit = TimeSpanExUnit.Span, _span = TimeSpan.FromMilliseconds(milliseconds) };
      }

      public static TimeSpanEx FromMicroSeconds(long microseconds)
      {
        return new TimeSpanEx() { _unit = TimeSpanExUnit.Span, _span = TimeSpan.FromTicks(10 * microseconds) };
      }

      public double Years
      {
        get
        {
          if (_unit == TimeSpanExUnit.Years)
            return _span.Ticks;
          else if (_unit == TimeSpanExUnit.Month)
            return _span.Ticks / 12.0;
          else
            throw new InvalidOperationException("Can not calculate Years because SpanUnit is not set to Years or Months");
        }
      }

      public long Months
      {
        get
        {
          if (_unit == TimeSpanExUnit.Years)
            return _span.Ticks * 12;
          else if (_unit == TimeSpanExUnit.Month)
            return _span.Ticks;
          else
            throw new InvalidOperationException("Can not calculate Months because SpanUnit is not set to Years or Months");
        }
      }

      public TimeSpan Span
      {
        get
        {
          if (_unit == TimeSpanExUnit.Span)
            return _span;
          else
            throw new InvalidOperationException("Can not calculate Span because SpanUnit is set to Years or to Months");
        }
      }

      public DateTime RoundUp(DateTime d)
      {
        switch (_unit)
        {
          case TimeSpanExUnit.Span:
            return Calc.DateTimeMath.RoundUpSpan(d, _span);

          case TimeSpanExUnit.Month:
            return Calc.DateTimeMath.RoundUpMonths(d, (int)_span.Ticks);

          case TimeSpanExUnit.Years:
            return Calc.DateTimeMath.RoundUpYears(d, (int)_span.Ticks);
        }
        return d;
      }

      public DateTime RoundDown(DateTime d)
      {
        switch (_unit)
        {
          case TimeSpanExUnit.Span:
            return Calc.DateTimeMath.RoundDownSpan(d, _span);

          case TimeSpanExUnit.Month:
            return Calc.DateTimeMath.RoundDownMonths(d, (int)_span.Ticks);

          case TimeSpanExUnit.Years:
            return Calc.DateTimeMath.RoundDownYears(d, (int)_span.Ticks);
        }
        return d;
      }

      public static DateTime Add(DateTime x, TimeSpanEx span)
      {
        switch (span._unit)
        {
          case TimeSpanExUnit.Years:
            return new DateTime(x.Year + (int)(span._span.Ticks), 1, 1, 0, 0, 0, x.Kind);

          case TimeSpanExUnit.Month:
            {
              long month = span._span.Ticks + x.Month - 1;
              int addyears = (int)(month / 12);
              month %= 12;
              if (month < 0)
              {
                month += 12;
                addyears--;
              }
              return new DateTime(x.Year + addyears, (int)(month + 1), 1, 0, 0, 0, x.Kind);
            }
          default:
          case TimeSpanExUnit.Span:
            return x + span._span;
        }
      }

      public static DateTime Subtract(DateTime x, TimeSpanEx span)
      {
        switch (span._unit)
        {
          case TimeSpanExUnit.Years:
            return new DateTime(x.Year - (int)(span._span.Ticks), 1, 1, 0, 0, 0, x.Kind);

          case TimeSpanExUnit.Month:
            {
              long month = x.Month - 1 - span._span.Ticks;
              int addyears = (int)(month / 12);
              month %= 12;
              if (month < 0)
              {
                month += 12;
                addyears--;
              }
              return new DateTime(x.Year + addyears, (int)(month + 1), 1, 0, 0, 0, x.Kind);
            }
          default:
          case TimeSpanExUnit.Span:
            return x - span._span;
        }
      }

      public static double Divide(TimeSpanEx a, TimeSpanEx b)
      {
        switch (a._unit)
        {
          case TimeSpanExUnit.Years:
            {
              switch (b._unit)
              {
                case TimeSpanExUnit.Years:
                  return a._span.Ticks / (double)b._span.Ticks;

                case TimeSpanExUnit.Month:
                  return a._span.Ticks * 12 / (double)b._span.Ticks;

                case TimeSpanExUnit.Span:
                  return TimeSpan.FromDays(365.25).Ticks * (a._span.Ticks / (double)b._span.Ticks);
              }
            }
            break;

          case TimeSpanExUnit.Month:
            {
              switch (b._unit)
              {
                case TimeSpanExUnit.Years:
                  return a._span.Ticks / (double)(12 * b._span.Ticks);

                case TimeSpanExUnit.Month:
                  return a._span.Ticks / (double)b._span.Ticks;

                case TimeSpanExUnit.Span:
                  return TimeSpan.FromDays(365.25 / 12).Ticks * (a._span.Ticks / (double)b._span.Ticks);
              }
            }
            break;

          case TimeSpanExUnit.Span:
            {
              switch (b._unit)
              {
                case TimeSpanExUnit.Years:
                  return a._span.Ticks / ((double)(b._span.Ticks) * TimeSpan.FromDays(365.25).Ticks);

                case TimeSpanExUnit.Month:
                  return a._span.Ticks / ((double)(b._span.Ticks) * TimeSpan.FromDays(365.25 / 12).Ticks);

                case TimeSpanExUnit.Span:
                  return a._span.Ticks / (double)b._span.Ticks;
              }
            }
            break;
        }
        return double.NaN;
      }

      public bool Equals(TimeSpanEx other)
      {
        return _unit == other._unit && _span == other._span;
      }

      bool IEquatable<object>.Equals(object other)
      {
        return (other is TimeSpanEx) ? Equals((TimeSpanEx)other) : false;
      }

      public override bool Equals(object obj)
      {
        return obj is TimeSpanEx other ? Equals(other) : false;
      }

      public override int GetHashCode()
      {
        return _span.GetHashCode() + 13 * _unit.GetHashCode();
      }

      public static bool operator ==(TimeSpanEx x, TimeSpanEx y)
      {
        return x.Equals(y);
      }

      public static bool operator !=(TimeSpanEx x, TimeSpanEx y)
      {
        return !(x.Equals(y));
      }
    }

    private class CachedMajorMinor : ICloneable
    {
      public DateTime Org, End;
      /// <summary>Physical span value between two major ticks.</summary>

      public TimeSpanEx MajorTickSpan;

      /// <summary>Minor ticks per Major tick ( if there is one minor tick between two major ticks m_minorticks is 2!</summary>

      public TimeSpanEx MinorTickSpan;

      public CachedMajorMinor(DateTime org, DateTime end, TimeSpanEx major, TimeSpanEx minor)
      {
        Org = org;
        End = end;
        MajorTickSpan = major;
        MinorTickSpan = minor;
      }

      public object Clone()
      {
        return MemberwiseClone();
      }
    }

    #endregion Inner classes

    #region static fields

    private static readonly TimeSpan[] _possibleMajorTickSpans =
      {
        TimeSpan.FromTicks(1),
        TimeSpan.FromTicks(2),
        TimeSpan.FromTicks(4),
        TimeSpan.FromTicks(5),
        TimeSpan.FromTicks(10),
        TimeSpan.FromTicks(20),
        TimeSpan.FromTicks(25),
        TimeSpan.FromTicks(40),
        TimeSpan.FromTicks(50),
        TimeSpan.FromTicks(100),
        TimeSpan.FromTicks(125),
        TimeSpan.FromTicks(200),
        TimeSpan.FromTicks(250),
        TimeSpan.FromTicks(400),
        TimeSpan.FromTicks(500),
        TimeSpan.FromTicks(1000),
        TimeSpan.FromTicks(1250),
        TimeSpan.FromTicks(2000),
        TimeSpan.FromTicks(2500),
        TimeSpan.FromTicks(4000),
        TimeSpan.FromTicks(5000),
        TimeSpan.FromMilliseconds(1),
        TimeSpan.FromMilliseconds(2),
        TimeSpan.FromMilliseconds(4),
        TimeSpan.FromMilliseconds(5),
        TimeSpan.FromMilliseconds(10),
        TimeSpan.FromMilliseconds(20),
        TimeSpan.FromMilliseconds(25),
        TimeSpan.FromMilliseconds(40),
        TimeSpan.FromMilliseconds(50),
        TimeSpan.FromMilliseconds(100),
        TimeSpan.FromMilliseconds(125),
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(250),
        TimeSpan.FromMilliseconds(400),
        TimeSpan.FromMilliseconds(500),
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(4),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(15),
        TimeSpan.FromSeconds(20),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(2),
        TimeSpan.FromMinutes(4),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(10),
        TimeSpan.FromMinutes(15),
        TimeSpan.FromMinutes(20),
        TimeSpan.FromMinutes(30),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(2),
        TimeSpan.FromHours(4),
        TimeSpan.FromHours(6),
        TimeSpan.FromHours(8),
        TimeSpan.FromHours(12),
        TimeSpan.FromDays(1),
        TimeSpan.FromDays(2),
        TimeSpan.FromDays(3),
        TimeSpan.FromDays(4),
        TimeSpan.FromDays(5),
        TimeSpan.FromDays(6),
        TimeSpan.FromDays(7),
        TimeSpan.FromDays(8),
        TimeSpan.FromDays(9),
        TimeSpan.FromDays(10),
        TimeSpan.FromDays(12),
        TimeSpan.FromDays(14),
        TimeSpan.FromDays(16),
        TimeSpan.FromDays(18),
        TimeSpan.FromDays(20),
        TimeSpan.FromDays(21),
        TimeSpan.FromDays(25),
      };

    private static readonly TimeSpanEx[] _possibleMinorTicksForYears =
      {
        TimeSpanEx.FromYears(1),
        TimeSpanEx.FromMonths(6),
        TimeSpanEx.FromMonths(4),
        TimeSpanEx.FromMonths(3),
        TimeSpanEx.FromMonths(2),
        TimeSpanEx.FromMonths(1),
      };

    private static readonly TimeSpanEx[] _possibleMinorTicksForDays =
      {
        TimeSpanEx.FromDays(1),
        TimeSpanEx.FromHours(12),
        TimeSpanEx.FromHours(8),
        TimeSpanEx.FromHours(6),
        TimeSpanEx.FromHours(4),
        TimeSpanEx.FromHours(2),
        TimeSpanEx.FromHours(1),
        TimeSpanEx.FromMinutes(30),
        TimeSpanEx.FromMinutes(20),
        TimeSpanEx.FromMinutes(15),
        TimeSpanEx.FromMinutes(10),
        TimeSpanEx.FromMinutes(5),
        TimeSpanEx.FromMinutes(1),
        TimeSpanEx.FromSeconds(30),
        TimeSpanEx.FromSeconds(20),
        TimeSpanEx.FromSeconds(15),
        TimeSpanEx.FromSeconds(10),
        TimeSpanEx.FromSeconds(5),
        TimeSpanEx.FromSeconds(1),
        TimeSpanEx.FromMilliSeconds(500),
      };

    #endregion static fields

    /// <summary>Maximum allowed number of ticks in case manual tick input will produce a big amount of ticks.</summary>
    protected static readonly int _maxSafeNumberOfTicks = 10000;

    /// <summary>If set, gives the number of minor ticks choosen by the user.</summary>
    private int? _userDefinedMinorTicks;

    /// <summary>If set, gives the physical value between two major ticks choosen by the user.</summary>
    private TimeSpanEx? _userDefinedMajorSpan;

    private double _orgGrace = 1 / 16.0;
    private double _endGrace = 1 / 16.0;
    private int _targetNumberOfMajorTicks = 6;
    private int _targetNumberOfMinorTicks = 2;

    /// <summary>If true, the boundaries will be set on a minor or major tick.</summary>
    private BoundaryTickSnapping _snapOrgToTick = BoundaryTickSnapping.SnapToMinorOrMajor;

    private BoundaryTickSnapping _snapEndToTick = BoundaryTickSnapping.SnapToMinorOrMajor;

    private SuppressedTicks _suppressedMajorTicks;
    private SuppressedTicks _suppressedMinorTicks;
    private AdditionalTicks _additionalMajorTicks;
    private AdditionalTicks _additionalMinorTicks;

    // Results
    private List<AltaxoVariant> _majorTicks;
    private List<AltaxoVariant> _minorTicks;

    // Cached values
    private CachedMajorMinor _cachedMajorMinor;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DateTimeTickSpacing), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DateTimeTickSpacing)obj;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DateTimeTickSpacing s = SDeserialize(o, info, parent);
        return s;
      }

      protected virtual DateTimeTickSpacing SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DateTimeTickSpacing s = null != o ? (DateTimeTickSpacing)o : new DateTimeTickSpacing();

        return s;
      }
    }

    #endregion Serialization

    public DateTimeTickSpacing()
    {
      _majorTicks = new List<AltaxoVariant>();
      _minorTicks = new List<AltaxoVariant>();
      _suppressedMajorTicks = new SuppressedTicks() { ParentObject = this };
      _suppressedMinorTicks = new SuppressedTicks() { ParentObject = this };
      _additionalMajorTicks = new AdditionalTicks() { ParentObject = this };
      _additionalMinorTicks = new AdditionalTicks() { ParentObject = this };
    }

    public DateTimeTickSpacing(DateTimeTickSpacing from)
      : base(from) // everything is done here, since CopyFrom is virtual!
    {
    }

    public override bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as DateTimeTickSpacing;
      if (null == from)
        return false;

      using (var suspendToken = SuspendGetToken())
      {
        CopyHelper.Copy(ref _cachedMajorMinor, from._cachedMajorMinor);

        _userDefinedMajorSpan = from._userDefinedMajorSpan;
        _userDefinedMinorTicks = from._userDefinedMinorTicks;

        _targetNumberOfMajorTicks = from._targetNumberOfMajorTicks;
        _targetNumberOfMinorTicks = from._targetNumberOfMinorTicks;

        _orgGrace = from._orgGrace;
        _endGrace = from._endGrace;

        _snapOrgToTick = from._snapOrgToTick;
        _snapEndToTick = from._snapEndToTick;

        ChildCopyToMember(ref _suppressedMajorTicks, from._suppressedMajorTicks);
        ChildCopyToMember(ref _suppressedMinorTicks, from._suppressedMinorTicks);
        ChildCopyToMember(ref _additionalMajorTicks, from._additionalMajorTicks);
        ChildCopyToMember(ref _additionalMinorTicks, from._additionalMinorTicks);

        _majorTicks = new List<AltaxoVariant>(from._majorTicks);
        _minorTicks = new List<AltaxoVariant>(from._minorTicks);

        EhSelfChanged();
        suspendToken.Resume();
      }
      return true;
    }

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _suppressedMajorTicks)
        yield return new Main.DocumentNodeAndName(_suppressedMajorTicks, "SuppressedMajorTicks");
      if (null != _suppressedMinorTicks)
        yield return new Main.DocumentNodeAndName(_suppressedMinorTicks, "SuppressedMinorTicks");
      if (null != _additionalMajorTicks)
        yield return new Main.DocumentNodeAndName(_additionalMajorTicks, "AdditionalMajorTicks");
      if (null != _additionalMinorTicks)
        yield return new Main.DocumentNodeAndName(_additionalMinorTicks, "AdditionalMinorTicks");
    }

    public override object Clone()
    {
      return new DateTimeTickSpacing(this);
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;
      else if (!(obj is DateTimeTickSpacing))
        return false;
      else
      {
        var from = (DateTimeTickSpacing)obj;

        if (_userDefinedMajorSpan != from._userDefinedMajorSpan)
          return false;
        if (_userDefinedMinorTicks != from._userDefinedMinorTicks)
          return false;

        if (_targetNumberOfMajorTicks != from._targetNumberOfMajorTicks)
          return false;
        if (_targetNumberOfMinorTicks != from._targetNumberOfMinorTicks)
          return false;

        if (_orgGrace != from._orgGrace)
          return false;
        if (_endGrace != from._endGrace)
          return false;

        if (_snapOrgToTick != from._snapOrgToTick)
          return false;
        if (_snapEndToTick != from._snapEndToTick)
          return false;


        if (!_suppressedMajorTicks.Equals(from._suppressedMajorTicks))
          return false;

        if (!_suppressedMinorTicks.Equals(from._suppressedMinorTicks))
          return false;

        if (!_additionalMajorTicks.Equals(from._additionalMajorTicks))
          return false;

        if (!_additionalMinorTicks.Equals(from._additionalMinorTicks))
          return false;
      }

      return true;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode() + 13 * _targetNumberOfMajorTicks + 31 * _targetNumberOfMinorTicks;
    }

    #region User parameters

    public double OrgGrace
    {
      get
      {
        return _orgGrace;
      }
      set
      {
        var oldValue = _orgGrace;
        _orgGrace = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public double EndGrace
    {
      get
      {
        return _endGrace;
      }
      set
      {
        var oldValue = _endGrace;
        _endGrace = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public BoundaryTickSnapping SnapOrgToTick
    {
      get
      {
        return _snapOrgToTick;
      }
      set
      {
        var oldValue = _snapOrgToTick;
        _snapOrgToTick = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public BoundaryTickSnapping SnapEndToTick
    {
      get
      {
        return _snapEndToTick;
      }
      set
      {
        var oldValue = _snapEndToTick;
        _snapEndToTick = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public int TargetNumberOfMajorTicks
    {
      get
      {
        return _targetNumberOfMajorTicks;
      }
      set
      {
        var oldValue = _targetNumberOfMajorTicks;
        _targetNumberOfMajorTicks = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public int TargetNumberOfMinorTicks
    {
      get
      {
        return _targetNumberOfMinorTicks;
      }
      set
      {
        var oldValue = _targetNumberOfMinorTicks;
        _targetNumberOfMinorTicks = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public int? MinorTicks
    {
      get
      {
        return _userDefinedMinorTicks;
      }
      set
      {
        var oldValue = _userDefinedMinorTicks;
        _userDefinedMinorTicks = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public TimeSpanEx? MajorTickSpan
    {
      get
      {
        return _userDefinedMajorSpan;
      }
      set
      {
        var oldValue = _userDefinedMajorSpan;
        _userDefinedMajorSpan = value;
        if (!oldValue.Equals(value))
          EhSelfChanged();
      }
    }

    public SuppressedTicks SuppressedMajorTicks
    {
      get
      {
        return _suppressedMajorTicks;
      }
    }

    public SuppressedTicks SuppressedMinorTicks
    {
      get
      {
        return _suppressedMinorTicks;
      }
    }

    public AdditionalTicks AdditionalMajorTicks
    {
      get
      {
        return _additionalMajorTicks;
      }
    }

    public AdditionalTicks AdditionalMinorTicks
    {
      get
      {
        return _additionalMinorTicks;
      }
    }

    #endregion User parameters

    public override AltaxoVariant[] GetMajorTicksAsVariant()
    {
      return _majorTicks.ToArray();
    }

    public override AltaxoVariant[] GetMinorTicksAsVariant()
    {
      return _minorTicks.ToArray();
    }

    // GetMajorTicksNormal : no need to override because no transformation available

    // GetMinorTicksNormal : no need to override because no transformation available


    private static void ConvertOrgEndToDateTimeValues(AltaxoVariant org, AltaxoVariant end, out DateTime dorg, out DateTime dend)
    {
      if (org.IsType(AltaxoVariant.Content.VDateTime))
        dorg = org;
      else if (org.CanConvertedToDouble)
        dorg = new DateTime((long)(org.ToDouble() * 1E7));
      else
        throw new ArgumentException("Variant org is not a DateTime nor a numeric value");

      if (end.IsType(AltaxoVariant.Content.VDateTime))
        dend = end;
      else if (end.CanConvertedToDouble)
        dend = new DateTime((long)(end.ToDouble() * 1E7));
      else
        throw new ArgumentException("Variant end is not a DateTime nor a numeric value");
    }

    public override bool PreProcessScaleBoundaries(ref AltaxoVariant org, ref AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable)
    {
      ConvertOrgEndToDateTimeValues(org, end, out var dorg, out var dend);

      //dorg = TransformOriginalToModified(dorg);
      //dend = TransformOriginalToModified(dend);

      if (InternalPreProcessScaleBoundaries(ref dorg, ref dend, isOrgExtendable, isEndExtendable))
      {
        org = dorg; //org = TransformModifiedToOriginal(dorg);
        end = dend; //end = TransformModifiedToOriginal(dend);
        return true;
      }
      else
        return false;
    }

    public override void FinalProcessScaleBoundaries(AltaxoVariant org, AltaxoVariant end, Scale scale)
    {
      ConvertOrgEndToDateTimeValues(org, end, out var dorg, out var dend);

      if (_cachedMajorMinor == null || _cachedMajorMinor.Org != dorg || _cachedMajorMinor.End != dend)
      {
        InternalPreProcessScaleBoundaries(ref dorg, ref dend, false, false); // make sure that _cachedMajorMinor is valid now
      }

      if (!(null != _cachedMajorMinor))
        throw new InvalidProgramException();

      _majorTicks.Clear();
      _minorTicks.Clear();
      InternalCalculateMajorTicks(dorg, dend, _cachedMajorMinor.MajorTickSpan);
      InternalCalculateMinorTicks(dorg, dend, _cachedMajorMinor.MajorTickSpan, _cachedMajorMinor.MinorTickSpan);
    }

    #region Calculation of tick values

    private void InternalCalculateMajorTicks(DateTime org, DateTime end, TimeSpanEx majorSpan)
    {
      _majorTicks.Clear();

      if (majorSpan._span.Ticks <= 0)
        return;

      DateTime x = majorSpan.RoundDown(org);

      while (x <= end)
      {
        if (x >= org)
          _majorTicks.Add(x);

        x = TimeSpanEx.Add(x, majorSpan);
      }

      // Remove suppressed ticks
      _suppressedMajorTicks.RemoveSuppressedTicks(_majorTicks);

      if (!_additionalMajorTicks.IsEmpty)
      {
        foreach (AltaxoVariant v in _additionalMajorTicks.Values)
        {
          _majorTicks.Add(v);
        }
      }
    }

    private void InternalCalculateMinorTicks(DateTime org, DateTime end, TimeSpanEx majorSpan, TimeSpanEx minorSpan)
    {
      _minorTicks.Clear();

      if (majorSpan._span.Ticks <= 0)
        return;

      DateTime currentMajor = majorSpan.RoundDown(org);
      DateTime nextMajor = TimeSpanEx.Add(currentMajor, majorSpan);
      DateTime x = TimeSpanEx.Add(currentMajor, minorSpan);

      while (x <= end)
      {
        while (nextMajor <= x)
        {
          nextMajor = TimeSpanEx.Add(nextMajor, majorSpan);
        }

        if (x >= org && x != nextMajor)
          _minorTicks.Add(x);

        x = TimeSpanEx.Add(x, majorSpan);
      }

      // Remove suppressed ticks
      _suppressedMinorTicks.RemoveSuppressedTicks(_minorTicks);

      if (!_additionalMinorTicks.IsEmpty)
      {
        foreach (AltaxoVariant v in _additionalMinorTicks.Values)
        {
          _minorTicks.Add(v);
        }
      }
    }

    #endregion Calculation of tick values

    #region Functions to predict change of scale by tick snapping, grace, and OneLever

    private bool InternalPreProcessScaleBoundaries(ref DateTime xorg, ref DateTime xend, bool isOrgExtendable, bool isEndExtendable)
    {
      _cachedMajorMinor = null;
      bool modified = false;

      if (!(xorg <= xend))
      {
        var h = xorg;
        xorg = xend;
        xend = h;
        modified = true;
      }
      if (xorg == xend)
      {
        var defaultSpanBy2 = TimeSpan.FromDays(1);
        if (xorg > (DateTime.MinValue + defaultSpanBy2))
          xorg -= defaultSpanBy2;
        else
          xorg = DateTime.MinValue;

        if (xend < DateTime.MaxValue - defaultSpanBy2)
          xend += defaultSpanBy2;
        else
          xend = DateTime.MaxValue;

        modified = true;
      }
      // here xorg should be < xend in any case
      if (!(xorg < xend))
        throw new InvalidProgramException();
      bool modGraceAndOneLever = GetOrgEndWithGraceAndOneLever(xorg, xend, isOrgExtendable, isEndExtendable, out var xOrgWithGraceAndOneLever, out var xEndWithGraceAndOneLever);
      bool modTickSnapping = GetOrgEndWithTickSnappingOnly(xend - xorg, xorg, xend, isOrgExtendable, isEndExtendable, out var xOrgWithTickSnapping, out var xEndWithTickSnapping, out var decadesPerMajorTick, out var minorTicks);

      // now compare the two
      if (xOrgWithTickSnapping <= xOrgWithGraceAndOneLever && xEndWithTickSnapping >= xEndWithGraceAndOneLever)
      {
        // then there is no need to apply Grace and OneLever
        modified |= modTickSnapping;
      }
      else
      {
        modified |= modGraceAndOneLever;
        modified |= GetOrgEndWithTickSnappingOnly(xEndWithGraceAndOneLever - xOrgWithGraceAndOneLever, xOrgWithGraceAndOneLever, xEndWithGraceAndOneLever, isOrgExtendable, isEndExtendable, out xOrgWithTickSnapping, out xEndWithTickSnapping, out decadesPerMajorTick, out minorTicks);
      }

      xorg = xOrgWithTickSnapping;
      xend = xEndWithTickSnapping;

      _cachedMajorMinor = new CachedMajorMinor(xorg, xend, decadesPerMajorTick, minorTicks);

      return modified;
    }

    private static TimeSpan Multiply(double a, TimeSpan b)
    {
      return TimeSpan.FromSeconds(a * b.TotalSeconds);
    }

    /// <summary>
    /// Applies the value for <see cref="OrgGrace"/> and <see cref="EndGrace"/> to the scale and calculated proposed values for the boundaries.
    /// </summary>
    /// <param name="scaleOrg">Scale origin.</param>
    /// <param name="scaleEnd">Scale end.</param>
    /// <param name="isOrgExtendable">True if the scale org can be extended.</param>
    /// <param name="isEndExtendable">True if the scale end can be extended.</param>
    /// <param name="propOrg">Returns the proposed value of the scale origin.</param>
    /// <param name="propEnd">Returns the proposed value of the scale end.</param>
    public bool GetOrgEndWithGraceAndOneLever(DateTime scaleOrg, DateTime scaleEnd, bool isOrgExtendable, bool isEndExtendable, out DateTime propOrg, out DateTime propEnd)
    {
      bool modified = false;

      TimeSpan scaleSpan = (scaleEnd - scaleOrg).Duration();

      propOrg = scaleOrg;
      if (isOrgExtendable)
      {
        TimeSpan orgToTheLeft = Multiply(Math.Abs(_orgGrace), scaleSpan);
        if (orgToTheLeft > (propOrg - DateTime.MinValue))
          orgToTheLeft = (propOrg - DateTime.MinValue);

        propOrg -= orgToTheLeft;
        modified |= (TimeSpan.Zero != orgToTheLeft);
      }

      propEnd = scaleEnd;
      if (isEndExtendable)
      {
        TimeSpan endToTheRight = Multiply(Math.Abs(_endGrace), scaleSpan);
        if (endToTheRight > (DateTime.MaxValue - propEnd))
          endToTheRight = (DateTime.MaxValue - propEnd);

        propEnd += endToTheRight;
        modified |= (TimeSpan.Zero != endToTheRight);
      }

      return modified;
    }

    /// <summary>Applies the tick snapping settings to the scale origin and scale end. This is done by a determination of the number of decades per major tick and the minor ticks per major tick interval.
    /// Then, the snapping values are applied, and the org and end values of the scale are adjusted (if allowed so).</summary>
    /// <param name="overriddenScaleSpan">The overriden scale span.</param>
    /// <param name="scaleOrg">The scale origin.</param>
    /// <param name="scaleEnd">The scale end.</param>
    /// <param name="isOrgExtendable">If set to <c>true</c>, it is allowed to adjust the scale org value.</param>
    /// <param name="isEndExtendable">if set to <c>true</c>, it is allowed to adjust the scale end value.</param>
    /// <param name="propOrg">The adjusted scale orgin.</param>
    /// <param name="propEnd">The adjusted scale end.</param>
    /// <param name="majorSpan">The physical value that corresponds to one major tick interval.</param>
    /// <param name="minorTicks">Number of minor ticks per major tick interval. This variable has some special values (see <see cref="MinorTicks"/>).</param>
    /// <returns>True if at least either org or end were adjusted to a new value.</returns>
    private bool GetOrgEndWithTickSnappingOnly(TimeSpan overriddenScaleSpan, DateTime scaleOrg, DateTime scaleEnd, bool isOrgExtendable, bool isEndExtendable, out DateTime propOrg, out DateTime propEnd, out TimeSpanEx majorSpan, out TimeSpanEx minorTicks)
    {
      bool modified = false;
      propOrg = scaleOrg;
      propEnd = scaleEnd;

      if (null != _userDefinedMajorSpan)
      {
        majorSpan = _userDefinedMajorSpan.Value;
      }
      else
      {
        majorSpan = CalculateMajorSpan(overriddenScaleSpan, _targetNumberOfMajorTicks);
      }

      minorTicks = majorSpan;
      if (null != _userDefinedMinorTicks)
      {
        var mticks = Math.Abs(_userDefinedMinorTicks.Value);
        if (mticks > 0)
        {
          switch (majorSpan._unit)
          {
            case TimeSpanExUnit.Years:
              {
                long years = majorSpan._span.Ticks;
                if (years >= mticks)
                  minorTicks = TimeSpanEx.FromYears(years / mticks);
                else if (years * 12 >= mticks)
                  minorTicks = TimeSpanEx.FromMonths(years * 12 / mticks);
              }
              break;

            case TimeSpanExUnit.Month:
              {
                long months = majorSpan._span.Ticks;
                if (months >= mticks)
                  minorTicks = TimeSpanEx.FromMonths(months / mticks);
              }
              break;

            case TimeSpanExUnit.Span:
              {
                minorTicks = TimeSpanEx.FromTicks(majorSpan._span.Ticks / mticks);
              }
              break;

            default:
              throw new NotImplementedException();
          }
        }
      }
      else
      {
        minorTicks = CalculateNumberOfMinorTicks(majorSpan, _targetNumberOfMinorTicks);
      }

      if (isOrgExtendable)
      {
        propOrg = GetOrgOrEndSnappedToTick(scaleOrg, majorSpan, minorTicks, _snapOrgToTick, false);
        modified |= BoundaryTickSnapping.SnapToNothing != _snapOrgToTick;
      }

      if (isEndExtendable)
      {
        propEnd = GetOrgOrEndSnappedToTick(scaleEnd, majorSpan, minorTicks, _snapEndToTick, true);
        modified |= BoundaryTickSnapping.SnapToNothing != _snapEndToTick;
      }

      return modified;
    }

    private DateTime GetOrgOrEndSnappedToTick(DateTime x, TimeSpanEx majorSpan, TimeSpanEx minorTicks, BoundaryTickSnapping snapping, bool upwards)
    {
      switch (snapping)
      {
        default:
        case BoundaryTickSnapping.SnapToNothing:
          {
            return x;
          }
        case BoundaryTickSnapping.SnapToMajorOnly:
          {
            if (upwards)
              return majorSpan.RoundUp(x);
            else
              return majorSpan.RoundDown(x);
          }
        case BoundaryTickSnapping.SnapToMinorOnly:
          {
            DateTime resultMinor, resultMajor;
            if (upwards)
            {
              resultMinor = minorTicks.RoundUp(x);
              resultMajor = majorSpan.RoundUp(x);
              return resultMinor == resultMajor ? TimeSpanEx.Add(resultMinor, minorTicks) : resultMinor;
            }
            else
            {
              resultMinor = minorTicks.RoundDown(x);
              resultMajor = majorSpan.RoundDown(x);
              return resultMinor == resultMajor ? TimeSpanEx.Subtract(resultMinor, minorTicks) : resultMinor;
            }
          }
        case BoundaryTickSnapping.SnapToMinorOrMajor:
          {
            DateTime resultMinor, resultMajor;
            if (upwards)
            {
              resultMinor = minorTicks.RoundUp(x);
              resultMajor = majorSpan.RoundUp(x);
              return resultMinor < resultMajor ? resultMinor : resultMajor;
            }
            else
            {
              resultMinor = minorTicks.RoundDown(x);
              resultMajor = majorSpan.RoundDown(x);
              return resultMinor > resultMajor ? resultMinor : resultMajor;
            }
          }
      }
    }

    private static int RoundTo1_2_3_4_6_8_12_18(double x)
    {
      if (x < 1.5)
        return 1;
      else if (x < 2.5)
        return 2;
      else if (x < 3.5)
        return 3;
      else if (x < 5)
        return 4;
      else if (x < 7)
        return 6;
      else if (x < 10)
        return 8;
      else if (x < 15)
        return 12;
      else
        return 18;
    }

    private static TimeSpanEx CalculateMajorSpan(TimeSpan span, int targetNumberOfMajorTicks)
    {
      TimeSpanEx majorspan;

      if (span > _possibleMajorTickSpans[_possibleMajorTickSpans.Length - 1])
      {
        if (span >= TimeSpan.FromDays(365 * targetNumberOfMajorTicks + 1))
        {
          var (spanRaw, spanDecadicExponent) = LinearTickSpacing.CalculateMajorSpan(span.TotalDays / 365.25, targetNumberOfMajorTicks);
          double yearPerTick = RMath.ScaleDecadic(spanRaw, spanDecadicExponent);
          if (yearPerTick > 0.75)
            return TimeSpanEx.FromYears((long)Math.Round(yearPerTick));
        }

        {
          var (spanRaw, spanDecadicExponent) = LinearTickSpacing.CalculateMajorSpan(span.TotalDays / (365.25 / 12), targetNumberOfMajorTicks);
          double monthPerTick = RMath.ScaleDecadic(spanRaw, spanDecadicExponent);
          if (monthPerTick >= 0.8)
            return TimeSpanEx.FromMonths(RoundTo1_2_3_4_6_8_12_18(monthPerTick));
        }

      }
      int i = _possibleMajorTickSpans.Length - 1;
      var destMajorTickSpan = new TimeSpan(span.Ticks / targetNumberOfMajorTicks);
      for (i = _possibleMajorTickSpans.Length - 1; i >= 0; i--)
      {
        if (_possibleMajorTickSpans[i] < destMajorTickSpan)
          break;
      }
      majorspan = TimeSpanEx.FromTimeSpan(_possibleMajorTickSpans[Math.Max(i, 0)]);

      return majorspan;
    }

    private static double GetMajorTickDifferencePenalty(double currentNumberOfTicks, int targetNumberOfTicks)
    {
      return Math.Abs(currentNumberOfTicks - targetNumberOfTicks);
    }

    private static double GetMinorTickDifferencePenalty(double currentNumberOfMinorTicks, int targetNumberOfMinorTicks)
    {
      return Math.Abs(currentNumberOfMinorTicks - targetNumberOfMinorTicks);
    }

    /// <summary>
    /// Calculates the number of minor ticks from the major span value and the target number of minor ticks.
    /// </summary>
    /// <param name="majorSpan">Major span value.</param>
    /// <param name="targetNumberOfMinorTicks">Target number of minor ticks.</param>
    /// <returns></returns>
    private static TimeSpanEx CalculateNumberOfMinorTicks(TimeSpanEx majorSpan, int targetNumberOfMinorTicks)
    {
      if (targetNumberOfMinorTicks <= 0)
        return majorSpan;

      switch (majorSpan._unit)
      {
        case TimeSpanExUnit.Years:
          {
            long years = majorSpan._span.Ticks;
            int ticks = LinearTickSpacing.CalculateNumberOfMinorTicks(years, targetNumberOfMinorTicks);
            if (ticks < years && ticks > 0 && 0 == years % ticks)
              return TimeSpanEx.FromYears(years / ticks);
            double minDiff = double.MaxValue;
            var result = TimeSpanEx.FromYears(1);
            var divisors = Calc.PrimeNumberMath.GetNeighbouringDivisors(years, targetNumberOfMinorTicks, years);
            foreach (var c in divisors)
            {
              double dticks = years / c;
              var diff = GetMinorTickDifferencePenalty(dticks, targetNumberOfMinorTicks);
              if (diff < minDiff)
              {
                minDiff = diff;
                result = TimeSpanEx.FromYears(years / c);
              }
            }
            divisors = Calc.PrimeNumberMath.GetNeighbouringDivisors(years * 12, targetNumberOfMinorTicks, years * 12);
            foreach (var c in divisors)
            {
              double dticks = (years * 12) / c;
              var diff = GetMinorTickDifferencePenalty(dticks, targetNumberOfMinorTicks);
              if (diff < minDiff)
              {
                minDiff = diff;
                result = TimeSpanEx.FromMonths((years * 12) / c);
              }
            }
            return result;
          }
        case TimeSpanExUnit.Month:
          {
            long months = majorSpan._span.Ticks;
            int ticks = LinearTickSpacing.CalculateNumberOfMinorTicks(months, targetNumberOfMinorTicks);
            if (ticks < months && ticks > 0 && 0 == months % ticks)
              return TimeSpanEx.FromMonths(months / ticks);

            double minDiff = double.MaxValue;
            var result = TimeSpanEx.FromMonths(1);
            var divisors = Calc.PrimeNumberMath.GetNeighbouringDivisors(months, targetNumberOfMinorTicks, months);
            foreach (var c in divisors)
            {
              double dticks = months / c;
              var diff = GetMinorTickDifferencePenalty(dticks, targetNumberOfMinorTicks);
              if (diff < minDiff)
              {
                minDiff = diff;
                result = TimeSpanEx.FromMonths(months / c);
              }
            }
            return result;
          }
        case TimeSpanExUnit.Span:
          {
            double days = TimeSpanEx.Divide(majorSpan, TimeSpanEx.FromDays(1));
            if (days > 1)
            {
              int ticks = LinearTickSpacing.CalculateNumberOfMinorTicks(days, targetNumberOfMinorTicks);
              if (ticks < days)
                return TimeSpanEx.FromTicks(majorSpan._span.Ticks / ticks);
            }

            var result = TimeSpanEx.FromDays(1);
            double minDiff = double.MaxValue;
            foreach (var c in _possibleMinorTicksForDays)
            {
              double dticks = TimeSpanEx.Divide(majorSpan, c);
              var diff = Math.Abs(dticks - targetNumberOfMinorTicks);
              if (diff < minDiff)
              {
                minDiff = diff;
                result = c;
              }
            }
            return result;
          }
        default:
          throw new NotImplementedException(string.Format("Unit {0} is unknown here", majorSpan._unit));
      }
    }

    #endregion Functions to predict change of scale by tick snapping, grace, and OneLever
  }
}
