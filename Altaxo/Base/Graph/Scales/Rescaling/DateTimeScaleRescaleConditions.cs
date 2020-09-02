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

namespace Altaxo.Graph.Scales.Rescaling
{
  /// <summary>
  /// Summary description for AxisRescaleConditions.
  /// </summary>
  [Serializable]
  public class DateTimeScaleRescaleConditions
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IUnboundNumericScaleRescaleConditions
  {
    #region InnerClasses

    /// <summary>
    /// Old boundary rescaling until 2015-02
    /// </summary>
    public enum BoundaryRescalingV1
    {
      /// <summary>
      /// Scale this boundary so that the data fits.
      /// </summary>
      Auto = 0,

      /// <summary>
      /// This axis boundary is set to a fixed value.
      /// </summary>
      Fixed = 1,

      /// <summary>
      /// This axis boundary is set to a fixed value.
      /// </summary>
      Equal = 1,

      /// <summary>
      /// The axis boundary is set to fit the data, but is set not greater than a certain value.
      /// </summary>
      NotGreater = 2,

      /// <summary>
      /// The axis boundary is set to fit the data, but is set not greater than a certain value.
      /// </summary>
      LessOrEqual = 2,

      /// <summary>
      /// The axis boundary is set to fit the data, but is set not lesser than a certain value.
      /// </summary>
      GreaterOrEqual = 3,

      /// <summary>
      /// The axis boundary is set to fit the data, but is set not lesser than a certain value.
      /// </summary>
      NotLess = 3,

      /// <summary>
      /// The axis boundary is set to use the span from the other axis boundary.
      /// </summary>
      UseSpan = 4,
    }

    #endregion InnerClasses

    protected BoundaryRescaling _orgRescaling;
    protected BoundaryRescaling _endRescaling;

    protected BoundariesRelativeTo _userProvidedOrgRelativeTo;
    protected BoundariesRelativeTo _userProvidedEndRelativeTo;

    protected long _userProvidedOrgValue;
    protected long _userProvidedEndValue;
    protected DateTimeKind _userProvidedOrgDateTimeKind;
    protected DateTimeKind _userProvidedEndDateTimeKind;

    protected long _dataBoundsOrg = 1;
    protected long _dataBoundsEnd = 2;

    // Results

    protected long _resultingOrg = 1;
    protected long _resultingEnd = 2;
    protected long _resultingMinOrg = UnboundMinOrg;
    protected long _resultingMaxEnd = UnboundMaxEnd;

    private const long UnboundMinOrg = 0;
    private const long UnboundMaxEnd = long.MaxValue;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Axes.Scaling.DateTimeAxisRescaleConditions", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.Rescaling.DateTimeAxisRescaleConditions", 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");

        /*
                DateTimeAxisRescaleConditions s = (DateTimeAxisRescaleConditions)obj;
                info.AddEnum("OrgRescaling", s._orgRescaling);
                info.AddValue("Org", s._org);
                info.AddEnum("EndRescaling", s._endRescaling);
                info.AddValue("End", s._end);
                info.AddEnum("SpanRescaling", s._spanRescaling);
                info.AddValue("Span", s._span);
                 */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DateTimeScaleRescaleConditions?)o ?? new DateTimeScaleRescaleConditions();

        s._userProvidedOrgRelativeTo = BoundariesRelativeTo.Absolute;
        s._userProvidedEndRelativeTo = BoundariesRelativeTo.Absolute;

        var orgRescaling = (BoundaryRescaling)(int)info.GetEnum("OrgRescaling", typeof(BoundaryRescalingV1));
        var org = info.GetDateTime("Org");
        var endRescaling = (BoundaryRescaling)(int)info.GetEnum("EndRescaling", typeof(BoundaryRescalingV1));
        var end = info.GetDateTime("End");
        var spanRescaling = (BoundaryRescaling)(int)info.GetEnum("SpanRescaling", typeof(BoundaryRescalingV1));
        var span = info.GetTimeSpan("Span");

        if (4 == (int)orgRescaling)
          orgRescaling = BoundaryRescaling.Auto;
        if (4 == (int)endRescaling)
          endRescaling = BoundaryRescaling.Auto;

        s._orgRescaling = orgRescaling;
        s._endRescaling = endRescaling;
        s._userProvidedOrgValue = org.Ticks;
        s._userProvidedEndValue = end.Ticks;
        s._userProvidedOrgDateTimeKind = org.Kind;
        s._userProvidedEndDateTimeKind = end.Kind;

        s._resultingOrg = org.Ticks;
        s._resultingEnd = end.Ticks;

        return s;
      }
    }

    /// <summary>
    /// 2015-02-16 added user provided values
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DateTimeScaleRescaleConditions), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DateTimeScaleRescaleConditions)obj;

        // Cached values
        info.AddValue("DataBoundsOrg", s._dataBoundsOrg);
        info.AddValue("DataBoundsEnd", s._dataBoundsEnd);

        // User provided values
        info.AddEnum("OrgRescaling", s._orgRescaling);
        info.AddEnum("EndRescaling", s._endRescaling);
        info.AddEnum("OrgRelativeTo", s._userProvidedOrgRelativeTo);
        info.AddEnum("EndRelativeTo", s._userProvidedEndRelativeTo);
        info.AddValue("UserProvidedOrg", s._userProvidedOrgValue);
        info.AddEnum("UserProvidedOrgDateTimeKind", s._userProvidedOrgDateTimeKind);
        info.AddValue("UserProvidedEnd", s._userProvidedEndValue);
        info.AddEnum("UserProvidedEndDateTimeKind", s._userProvidedEndDateTimeKind);

        // Final result
        // Final result
        info.AddValue("ResultingOrg", s._resultingOrg);
        info.AddValue("ResultingMinOrg", s._resultingMinOrg);
        info.AddValue("ResultingEnd", s._resultingEnd);
        info.AddValue("ResultingMaxEnd", s._resultingMaxEnd);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DateTimeScaleRescaleConditions?)o ?? new DateTimeScaleRescaleConditions();

        // Cached values
        s._dataBoundsOrg = info.GetInt64("DataBoundsOrg");
        s._dataBoundsEnd = info.GetInt64("DataBoundsEnd");

        // User provided values
        s._orgRescaling = (BoundaryRescaling)info.GetEnum("OrgRescaling", typeof(BoundaryRescaling));
        s._endRescaling = (BoundaryRescaling)info.GetEnum("EndRescaling", typeof(BoundaryRescaling));
        s._userProvidedOrgRelativeTo = (BoundariesRelativeTo)info.GetEnum("OrgRelativeTo", typeof(BoundariesRelativeTo));
        s._userProvidedEndRelativeTo = (BoundariesRelativeTo)info.GetEnum("EndRelativeTo", typeof(BoundariesRelativeTo));
        s._userProvidedOrgValue = info.GetInt64("UserProvidedOrg");
        s._userProvidedOrgDateTimeKind = (DateTimeKind)info.GetEnum("UserProvidedOrgDateTimeKind", typeof(DateTimeKind));
        s._userProvidedEndValue = info.GetInt64("UserProvidedEnd");
        s._userProvidedEndDateTimeKind = (DateTimeKind)info.GetEnum("UserProvidedEndDateTimeKind", typeof(DateTimeKind));

        // Final result
        s._resultingOrg = info.GetInt64("ResultingOrg");
        s._resultingMinOrg = info.GetInt64("ResultingMinOrg");
        s._resultingEnd = info.GetInt64("ResultingEnd");
        s._resultingMaxEnd = info.GetInt64("ResultingMaxEnd");
        return s;
      }
    }

    #endregion Serialization

    public DateTimeScaleRescaleConditions()
    {
    }

    public DateTimeScaleRescaleConditions(DateTimeScaleRescaleConditions from)
    {
      CopyFrom(from);
    }

    /// <summary>
    /// Copies the data from another object.
    /// </summary>
    /// <param name="obj">The object to copy the data from.</param>
    public virtual bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as DateTimeScaleRescaleConditions;
      if (from is null)
        return false;

      _orgRescaling = from._orgRescaling;
      _endRescaling = from._endRescaling;

      _userProvidedOrgRelativeTo = from._userProvidedOrgRelativeTo;
      _userProvidedEndRelativeTo = from._userProvidedEndRelativeTo;

      _userProvidedOrgValue = from._userProvidedOrgValue;
      _userProvidedOrgDateTimeKind = from._userProvidedOrgDateTimeKind;
      _userProvidedEndValue = from._userProvidedEndValue;
      _userProvidedEndDateTimeKind = from._userProvidedEndDateTimeKind;

      _resultingOrg = from._resultingOrg;
      _resultingEnd = from._resultingEnd;

      _resultingMinOrg = from._resultingMinOrg;
      _resultingMaxEnd = from._resultingMaxEnd;

      EhSelfChanged(EventArgs.Empty);

      return true;
    }

    public virtual object Clone()
    {
      return new DateTimeScaleRescaleConditions(this);
    }

    #region public properties

    public virtual DateTime ResultingOrg { get { return new DateTime(_resultingOrg, _userProvidedOrgDateTimeKind); } }

    public virtual DateTime ResultingEnd { get { return new DateTime(_resultingEnd, _userProvidedEndDateTimeKind); } }

    public bool IsResultingOrgFixed { get { return _resultingOrg == _resultingMinOrg; } }

    public bool IsResultingEndFixed { get { return _resultingEnd == _resultingMaxEnd; } }

    #endregion public properties

    public virtual void SetUserParameters(BoundaryRescaling orgRescaling, DateTime orgValue, BoundaryRescaling endRescaling, DateTime endValue)
    {
      SetUserParameters(orgRescaling, BoundariesRelativeTo.Absolute, orgValue.Ticks, orgValue.Kind, endRescaling, BoundariesRelativeTo.Absolute, endValue.Ticks, endValue.Kind);
    }

    public virtual void SetUserParameters(BoundaryRescaling orgRescaling, BoundariesRelativeTo orgRelativeTo, long orgValue, DateTimeKind orgValueKind, BoundaryRescaling endRescaling, BoundariesRelativeTo endRelativeTo, long endValue, DateTimeKind endValueKind)
    {
      bool isChange =

      _orgRescaling != orgRescaling ||
      _userProvidedOrgRelativeTo != orgRelativeTo ||
      _userProvidedOrgValue != orgValue ||
      _endRescaling != endRescaling ||
      _userProvidedEndRelativeTo != endRelativeTo ||
      _userProvidedEndValue != endValue;

      _orgRescaling = orgRescaling;
      _userProvidedOrgRelativeTo = orgRelativeTo;
      _userProvidedOrgValue = orgValue;

      _endRescaling = endRescaling;
      _userProvidedEndRelativeTo = endRelativeTo;
      _userProvidedEndValue = endValue;

      if (isChange)
      {
        ProcessOrg_UserParametersChanged();
        ProcessEnd_UserParametersChanged();
        EhSelfChanged();
      }
    }

    #region Accessors

    public BoundaryRescaling OrgRescaling
    {
      get
      {
        return _orgRescaling;
      }
    }

    public BoundaryRescaling EndRescaling
    {
      get
      {
        return _endRescaling;
      }
    }

    public BoundariesRelativeTo OrgRelativeTo { get { return _userProvidedOrgRelativeTo; } }

    public BoundariesRelativeTo EndRelativeTo { get { return _userProvidedEndRelativeTo; } }

    public virtual long UserProvidedOrgValue { get { return _userProvidedOrgValue; } }

    public virtual DateTimeKind UserProvidedOrgKind { get { return _userProvidedOrgDateTimeKind; } }

    public virtual long UserProvidedEndValue { get { return _userProvidedEndValue; } }

    public virtual DateTimeKind UserProvidedEndKind { get { return _userProvidedEndDateTimeKind; } }

    public DateTime DataBoundsOrg { get { return new DateTime(_dataBoundsOrg, DateTimeKind.Utc); } }

    public DateTime DataBoundsEnd { get { return new DateTime(_dataBoundsEnd, DateTimeKind.Utc); } }

    #endregion Accessors

    #region Event handling

    public void OnUserZoomed(DateTime newZoomOrg, DateTime newZoomEnd)
    {
      if (!(newZoomOrg < newZoomEnd))
        throw new ArgumentOutOfRangeException("zoomOrg should be less than zoomEnd");

      var oldResultingOrg = _resultingOrg;
      var oldResultingEnd = _resultingEnd;
      var oldResultingMinOrg = _resultingMinOrg;
      var oldResultingMaxEnd = _resultingMaxEnd;

      ProcessOrg_UserZoomed(newZoomOrg.Ticks);
      ProcessEnd_UserZoomed(newZoomEnd.Ticks);

      if (
        oldResultingOrg != _resultingOrg ||
        oldResultingEnd != _resultingEnd ||
        oldResultingMinOrg != _resultingMinOrg ||
        oldResultingMaxEnd != _resultingMaxEnd
        )
      {
        EhSelfChanged();
      }

    }

    public void OnUserRescaled()
    {
      var oldResultingOrg = _resultingOrg;
      var oldResultingEnd = _resultingEnd;
      var oldResultingMinOrg = _resultingMinOrg;
      var oldResultingMaxEnd = _resultingMaxEnd;

      ProcessOrg_UserRescaled();
      ProcessEnd_UserRescaled();

      if (
        oldResultingOrg != _resultingOrg ||
        oldResultingEnd != _resultingEnd ||
        oldResultingMinOrg != _resultingMinOrg ||
        oldResultingMaxEnd != _resultingMaxEnd
        )
      {
        EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets the mean value of the data bounds. We use the 'scale' mean, i.e. the physical value of the scale where its logical value is 0.5.
    /// </summary>
    /// <returns></returns>
    protected virtual long GetDataBoundsScaleMean()
    {
      return (_dataBoundsOrg + _dataBoundsEnd) / 2;
    }

    /// <summary>
    /// Fixes the data bounds org and end. Here you have to handle special cases, like org and end are equal. At return, org should be strictly less than end.
    /// </summary>
    /// <param name="dataBoundsOrg">The data bounds org.</param>
    /// <param name="dataBoundsEnd">The data bounds end.</param>
    protected virtual void FixValuesForDataBoundsOrgAndEnd(ref DateTime dataBoundsOrg, ref DateTime dataBoundsEnd)
    {
      // ensure that data bounds always have some distance
      if (dataBoundsOrg == dataBoundsEnd)
      {
        if (dataBoundsOrg == DateTime.MinValue)
        {
          dataBoundsOrg = DateTime.MinValue;
          dataBoundsEnd = DateTime.MinValue + TimeSpan.FromDays(1);
        }
        else
        {
          dataBoundsOrg = dataBoundsOrg - TimeSpan.FromDays(1);
          dataBoundsEnd = dataBoundsEnd + TimeSpan.FromDays(1);
        }
      }
    }

    /// <summary>
    /// Fixes the values when the user zoomed. For instance, if org is greater then end, both values are interchanged.
    /// </summary>
    /// <param name="zoomOrg">The zoom org.</param>
    /// <param name="zoomEnd">The zoom end.</param>
    protected virtual void FixValuesForUserZoomed(ref DateTime zoomOrg, ref DateTime zoomEnd)
    {
    }

    public void OnDataBoundsChanged(DateTime dataBoundsOrg, DateTime dataBoundsEnd)
    {
      if (!(dataBoundsOrg <= dataBoundsEnd))
        throw new ArgumentOutOfRangeException("dataBoundsOrg should be less than dataBoundsEnd");

      var oldResultingOrg = _resultingOrg;
      var oldResultingEnd = _resultingEnd;
      var oldResultingMinOrg = _resultingMinOrg;
      var oldResultingMaxEnd = _resultingMaxEnd;

      FixValuesForDataBoundsOrgAndEnd(ref dataBoundsOrg, ref dataBoundsEnd);

      _dataBoundsOrg = dataBoundsOrg.Ticks;
      _dataBoundsEnd = dataBoundsEnd.Ticks;

      ProcessOrg_DataBoundsChanged();
      ProcessEnd_DataBoundsChanged();

      var changed =
        _resultingOrg != oldResultingOrg ||
        _resultingEnd != oldResultingEnd ||
        _resultingMinOrg != oldResultingMinOrg ||
        _resultingMaxEnd != oldResultingMaxEnd;

      if (changed)
        EhSelfChanged();
    }

    #endregion Event handling

    #region Resulting Org/End to/fron User Org/End

    protected virtual long GetResultingOrgFromUserProvidedOrg()
    {
      switch (_userProvidedOrgRelativeTo)
      {
        case BoundariesRelativeTo.Absolute:
          return _userProvidedOrgValue;

        case BoundariesRelativeTo.RelativeToDataBoundsOrg:
          return _userProvidedOrgValue + _dataBoundsOrg;

        case BoundariesRelativeTo.RelativeToDataBoundsEnd:
          return _userProvidedOrgValue + _dataBoundsEnd;

        case BoundariesRelativeTo.RelativeToDataBoundsMean:
          return _userProvidedOrgValue + GetDataBoundsScaleMean();

        default:
          throw new NotImplementedException();
      }
    }

    protected virtual long GetUserProvidedOrgFromResultingOrg(long resultingOrg)
    {
      switch (_userProvidedOrgRelativeTo)
      {
        case BoundariesRelativeTo.Absolute:
          return resultingOrg;

        case BoundariesRelativeTo.RelativeToDataBoundsOrg:
          return resultingOrg - _dataBoundsOrg;

        case BoundariesRelativeTo.RelativeToDataBoundsEnd:
          return resultingOrg - _dataBoundsEnd;

        case BoundariesRelativeTo.RelativeToDataBoundsMean:
          return resultingOrg - GetDataBoundsScaleMean();

        default:
          throw new NotImplementedException();
      }
    }

    protected virtual long GetResultingEndFromUserProvidedEnd()
    {
      switch (_userProvidedEndRelativeTo)
      {
        case BoundariesRelativeTo.Absolute:
          return _userProvidedEndValue;

        case BoundariesRelativeTo.RelativeToDataBoundsOrg:
          return _userProvidedEndValue + _dataBoundsOrg;

        case BoundariesRelativeTo.RelativeToDataBoundsEnd:
          return _userProvidedEndValue + _dataBoundsEnd;

        case BoundariesRelativeTo.RelativeToDataBoundsMean:
          return _userProvidedEndValue + GetDataBoundsScaleMean();

        default:
          throw new NotImplementedException();
      }
    }

    protected virtual long GetUserProvidedEndFromResultingEnd(long resultingEnd)
    {
      switch (_userProvidedEndRelativeTo)
      {
        case BoundariesRelativeTo.Absolute:
          return resultingEnd;

        case BoundariesRelativeTo.RelativeToDataBoundsOrg:
          return resultingEnd - _dataBoundsOrg;

        case BoundariesRelativeTo.RelativeToDataBoundsEnd:
          return resultingEnd - _dataBoundsEnd;

        case BoundariesRelativeTo.RelativeToDataBoundsMean:
          return resultingEnd - GetDataBoundsScaleMean();

        default:
          throw new NotImplementedException();
      }
    }

    #endregion Resulting Org/End to/fron User Org/End

    #region Process Org End

    protected void ProcessOrg_DataBoundsChanged()
    {
      var resultingUserProvidedOrgValue = GetResultingOrgFromUserProvidedOrg();

      switch (_orgRescaling)
      {
        case BoundaryRescaling.Fixed:
        case BoundaryRescaling.FixedManually:
        case BoundaryRescaling.FixedZoomable:
          _resultingOrg = resultingUserProvidedOrgValue;
          _resultingMinOrg = _resultingOrg; // Strictly fixed
          break;

        case BoundaryRescaling.GreaterOrEqual:
          if (resultingUserProvidedOrgValue >= _dataBoundsOrg)
          {
            _resultingOrg = resultingUserProvidedOrgValue;
            _resultingMinOrg = _resultingOrg; // Strictly fixed
          }
          else
          {
            _resultingOrg = _dataBoundsOrg;
            _resultingMinOrg = resultingUserProvidedOrgValue; // not fixed because resultingOrg can go further down to resultingUserProvidedOrgValue
          }
          break;

        case BoundaryRescaling.LessOrEqual:
          if (resultingUserProvidedOrgValue <= _dataBoundsOrg)
          {
            _resultingOrg = resultingUserProvidedOrgValue;
            _resultingMinOrg = UnboundMinOrg; // Not fixed
          }
          else
          {
            _resultingOrg = _dataBoundsOrg;
            _resultingMinOrg = UnboundMinOrg; // Not fixed
          }
          break;

        case BoundaryRescaling.Auto:
          _resultingOrg = _dataBoundsOrg;
          _resultingMinOrg = UnboundMinOrg; // Not fixed
          break;

        case BoundaryRescaling.AutoTempFixed:
          _orgRescaling = BoundaryRescaling.Auto;
          _resultingOrg = _dataBoundsOrg;
          _resultingMinOrg = UnboundMinOrg; // Not fixed
          break;
      }
    }

    protected void ProcessOrg_UserRescaled()
    {
      var resultingUserProvidedOrgValue = GetResultingOrgFromUserProvidedOrg();

      switch (_orgRescaling)
      {
        case BoundaryRescaling.Auto:
          _resultingOrg = _dataBoundsOrg;
          _resultingMinOrg = UnboundMinOrg; // Not fixed
          break;

        case BoundaryRescaling.AutoTempFixed:
          _orgRescaling = BoundaryRescaling.Auto; // Fall back to Auto rescaling
          _resultingOrg = _dataBoundsOrg;
          _resultingMinOrg = UnboundMinOrg; // Not fixed
          break;

        case BoundaryRescaling.Fixed:
          _resultingOrg = resultingUserProvidedOrgValue;
          _resultingMinOrg = _resultingOrg; // Strictly fixed
          break;

        case BoundaryRescaling.FixedManually:
          // TODO ask for switching to AutoTemp or AutoTempFixed
          // HERE as long as fixed manually, we treat this as fixed
          _resultingOrg = resultingUserProvidedOrgValue;
          _resultingMinOrg = _resultingOrg; // Strictly fixed
          break;

        case BoundaryRescaling.FixedZoomable: // treat as fixed
          _resultingOrg = resultingUserProvidedOrgValue;
          _resultingMinOrg = _resultingOrg; // Strictly fixed
          break;

        case BoundaryRescaling.LessOrEqual:
          if (resultingUserProvidedOrgValue <= _dataBoundsOrg)
          {
            _resultingOrg = resultingUserProvidedOrgValue;
            _resultingMinOrg = UnboundMinOrg; // Not fixed
          }
          else
          {
            _resultingOrg = _dataBoundsOrg;
            _resultingMinOrg = UnboundMinOrg; // Not fixed
          }
          break;

        case BoundaryRescaling.GreaterOrEqual:
          if (resultingUserProvidedOrgValue >= _dataBoundsOrg)
          {
            _resultingOrg = resultingUserProvidedOrgValue;
            _resultingMinOrg = _resultingOrg; // fixed
          }
          else
          {
            _resultingOrg = _dataBoundsOrg;
            _resultingMinOrg = resultingUserProvidedOrgValue; // not fixed till resultingUserProvidedOrg
          }
          break;
      }
    }

    protected void ProcessOrg_UserZoomed(long zoomValueOrg)
    {
      var resultingUserProvidedOrgValue = GetResultingOrgFromUserProvidedOrg();

      switch (_orgRescaling)
      {
        case BoundaryRescaling.Auto:
          _orgRescaling = BoundaryRescaling.AutoTempFixed;
          _resultingOrg = zoomValueOrg;
          _resultingMinOrg = _resultingOrg; // Strictly fixed
          _userProvidedOrgValue = GetUserProvidedOrgFromResultingOrg(_resultingOrg);
          break;

        case BoundaryRescaling.AutoTempFixed:
          _resultingOrg = zoomValueOrg;
          _resultingMinOrg = _resultingOrg; // Strictly fixed
          _userProvidedOrgValue = GetUserProvidedOrgFromResultingOrg(_resultingOrg);
          break;

        case BoundaryRescaling.Fixed:
          // Ignore zoom
          break;

        case BoundaryRescaling.FixedManually:
          // use the new values as user provided values
          _resultingOrg = zoomValueOrg;
          _resultingMinOrg = _resultingOrg; // Strictly fixed
          _userProvidedOrgValue = GetUserProvidedOrgFromResultingOrg(zoomValueOrg);
          break;

        case BoundaryRescaling.FixedZoomable:
          // use the new values, but keep user provided values
          _resultingOrg = zoomValueOrg;
          _resultingMinOrg = _resultingOrg; // Strictly fixed
          break;

        case BoundaryRescaling.LessOrEqual:
          _resultingOrg = zoomValueOrg;
          _resultingMinOrg = _resultingOrg; // Strictly fixed
          break;

        case BoundaryRescaling.GreaterOrEqual:
          _resultingOrg = zoomValueOrg;
          _resultingMinOrg = _resultingOrg; // Strictly fixed
          break;
      }
    }

    protected void ProcessOrg_UserParametersChanged()
    {
      var resultingUserProvidedOrgValue = GetResultingOrgFromUserProvidedOrg();

      switch (_orgRescaling)
      {
        case BoundaryRescaling.Fixed:
        case BoundaryRescaling.FixedManually:
        case BoundaryRescaling.FixedZoomable:
          _resultingOrg = resultingUserProvidedOrgValue;
          _resultingMinOrg = _resultingOrg; // Strictly fixed
          break;

        case BoundaryRescaling.GreaterOrEqual:
          if (resultingUserProvidedOrgValue >= _dataBoundsOrg)
          {
            _resultingOrg = resultingUserProvidedOrgValue;
            _resultingMinOrg = _resultingOrg; // fixed
          }
          else
          {
            _resultingOrg = _dataBoundsOrg;
            _resultingMinOrg = resultingUserProvidedOrgValue; // fixed till user value
          }
          break;

        case BoundaryRescaling.LessOrEqual:
          if (resultingUserProvidedOrgValue <= _dataBoundsOrg)
          {
            _resultingOrg = resultingUserProvidedOrgValue;
            _resultingMinOrg = UnboundMinOrg; // not fixed
          }
          else
          {
            _resultingOrg = _dataBoundsOrg;
            _resultingMinOrg = UnboundMinOrg; // not fixed
          }
          break;

        case BoundaryRescaling.Auto:

          _resultingOrg = _dataBoundsOrg;
          _resultingMinOrg = UnboundMinOrg; // not fixed
          break;

        case BoundaryRescaling.AutoTempFixed:
          _resultingOrg = resultingUserProvidedOrgValue;
          _resultingMinOrg = _resultingOrg; // fixed
          break;
      }
    }

    protected void ProcessEnd_DataBoundsChanged()
    {
      var resultingUserProvidedEndValue = GetResultingEndFromUserProvidedEnd();

      switch (_endRescaling)
      {
        case BoundaryRescaling.Fixed:
        case BoundaryRescaling.FixedManually:
        case BoundaryRescaling.FixedZoomable:
          _resultingEnd = resultingUserProvidedEndValue;
          _resultingMaxEnd = _resultingEnd; // strictly fixed
          break;

        case BoundaryRescaling.GreaterOrEqual:
          if (resultingUserProvidedEndValue >= _dataBoundsEnd)
          {
            _resultingEnd = resultingUserProvidedEndValue;
            _resultingMaxEnd = UnboundMaxEnd; // not fixed
          }
          else
          {
            _resultingEnd = _dataBoundsEnd;
            _resultingMaxEnd = UnboundMaxEnd; // not fixed
          }
          break;

        case BoundaryRescaling.LessOrEqual:
          if (resultingUserProvidedEndValue <= _dataBoundsEnd)
          {
            _resultingEnd = resultingUserProvidedEndValue;
            _resultingMaxEnd = _resultingEnd; // fixed
          }
          else
          {
            _resultingEnd = _dataBoundsEnd;
            _resultingMaxEnd = resultingUserProvidedEndValue; // because resultingEnd can go further up to resultingUserProvidedValue
          }
          break;

        case BoundaryRescaling.Auto:
          _resultingEnd = _dataBoundsEnd;
          _resultingMaxEnd = UnboundMaxEnd; // not fixed
          break;

        case BoundaryRescaling.AutoTempFixed:
          _endRescaling = BoundaryRescaling.Auto; // Fall back to auto rescaling
          _resultingEnd = _dataBoundsEnd;
          _resultingMaxEnd = UnboundMaxEnd; // not fixed
          break;
      }
    }

    protected void ProcessEnd_UserRescaled()
    {
      var resultingUserProvidedEndValue = GetResultingEndFromUserProvidedEnd();

      switch (_endRescaling)
      {
        case BoundaryRescaling.Auto:
          _resultingEnd = _dataBoundsEnd;
          _resultingMaxEnd = UnboundMaxEnd; // not fixed
          break;

        case BoundaryRescaling.AutoTempFixed:
          _endRescaling = BoundaryRescaling.Auto; // Fall back to auto
          _resultingEnd = _dataBoundsEnd;
          _resultingMaxEnd = UnboundMaxEnd; // not fixed
          break;

        case BoundaryRescaling.Fixed:
          _resultingEnd = resultingUserProvidedEndValue;
          _resultingMaxEnd = _resultingEnd; // strictly fixed
          break;

        case BoundaryRescaling.FixedManually:
          // TODO ask for switching to AutoTemp or AutoTempFixed
          // HERE as long as fixed manually, we treat this as fixed
          _resultingEnd = resultingUserProvidedEndValue;
          _resultingMaxEnd = _resultingEnd; // strictly fixed
          break;

        case BoundaryRescaling.FixedZoomable: // treat as fixed
          _resultingEnd = resultingUserProvidedEndValue;
          _resultingMaxEnd = _resultingEnd; // strictly fixed
          break;

        case BoundaryRescaling.GreaterOrEqual:
          if (resultingUserProvidedEndValue >= _dataBoundsEnd)
          {
            _resultingEnd = resultingUserProvidedEndValue;
            _resultingMaxEnd = UnboundMaxEnd; // not fixed
          }
          else
          {
            _resultingEnd = _dataBoundsEnd;
            _resultingMaxEnd = UnboundMaxEnd; // not fixed
          }
          break;

        case BoundaryRescaling.LessOrEqual:
          if (resultingUserProvidedEndValue <= _dataBoundsEnd)
          {
            _resultingEnd = resultingUserProvidedEndValue;
            _resultingMaxEnd = _resultingEnd; // fixed
          }
          else
          {
            _resultingEnd = _dataBoundsEnd;
            _resultingMaxEnd = _userProvidedEndValue; // fixed until user provided end value
          }
          break;
      }
    }

    protected void ProcessEnd_UserZoomed(long zoomValueEnd)
    {
      var resultingUserProvidedEndValue = GetResultingEndFromUserProvidedEnd();

      switch (_endRescaling)
      {
        case BoundaryRescaling.Auto:
          _endRescaling = BoundaryRescaling.AutoTempFixed;
          _resultingEnd = zoomValueEnd;
          _resultingMaxEnd = _resultingEnd; // strictly fixed
          _userProvidedEndValue = GetUserProvidedEndFromResultingEnd(_resultingEnd);
          break;

        case BoundaryRescaling.AutoTempFixed:
          _resultingEnd = zoomValueEnd;
          _resultingMaxEnd = _resultingEnd; // strictly fixed
          _userProvidedEndValue = GetUserProvidedEndFromResultingEnd(_resultingEnd);
          break;

        case BoundaryRescaling.Fixed:
          // Ignore zoom
          break;

        case BoundaryRescaling.FixedManually:
          // use the new values as user provided values
          _resultingEnd = zoomValueEnd;
          _resultingMaxEnd = _resultingEnd; // strictly fixed
          _userProvidedEndValue = GetUserProvidedEndFromResultingEnd(zoomValueEnd);
          break;

        case BoundaryRescaling.FixedZoomable:
          // use the new values, but keep user provided values
          _resultingEnd = zoomValueEnd;
          _resultingMaxEnd = _resultingEnd; // strictly fixed
          break;

        case BoundaryRescaling.LessOrEqual:
          _resultingEnd = zoomValueEnd;
          _resultingMaxEnd = _resultingEnd; // strictly fixed
          break;

        case BoundaryRescaling.GreaterOrEqual:
          _resultingEnd = zoomValueEnd;
          _resultingMaxEnd = _resultingEnd; // strictly fixed
          break;
      }
    }

    protected void ProcessEnd_UserParametersChanged()
    {
      var resultingUserProvidedEndValue = GetResultingEndFromUserProvidedEnd();

      switch (_endRescaling)
      {
        case BoundaryRescaling.Fixed:
        case BoundaryRescaling.FixedManually:
        case BoundaryRescaling.FixedZoomable:
          _resultingEnd = resultingUserProvidedEndValue;
          _resultingMaxEnd = _resultingEnd; // strictly fixed
          break;

        case BoundaryRescaling.GreaterOrEqual:
          if (resultingUserProvidedEndValue >= _dataBoundsEnd)
          {
            _resultingEnd = resultingUserProvidedEndValue;
            _resultingMaxEnd = UnboundMaxEnd; // not fixed
          }
          else
          {
            _resultingEnd = _dataBoundsEnd;
            _resultingMaxEnd = UnboundMaxEnd; // not fixed
          }
          break;

        case BoundaryRescaling.LessOrEqual:
          if (resultingUserProvidedEndValue <= _dataBoundsEnd)
          {
            _resultingEnd = resultingUserProvidedEndValue;
            _resultingMaxEnd = _resultingEnd; // fixed
          }
          else
          {
            _resultingEnd = _dataBoundsEnd;
            _resultingMaxEnd = _userProvidedEndValue; // fixed till user value
          }
          break;

        case BoundaryRescaling.Auto:
          _resultingEnd = _dataBoundsEnd;
          _resultingMaxEnd = UnboundMaxEnd; // not fixed
          break;

        case BoundaryRescaling.AutoTempFixed:
          _resultingEnd = resultingUserProvidedEndValue;
          _resultingMaxEnd = _resultingEnd; // strictly fixed
          break;
      }
    }

    #endregion Process Org End

    #region IScaleRescaleConditions implementation

    void IUnboundNumericScaleRescaleConditions.SetUserParameters(BoundaryRescaling orgRescaling, BoundariesRelativeTo orgRelativeTo, Data.AltaxoVariant orgValue, BoundaryRescaling endRescaling, BoundariesRelativeTo endRelativeTo, Data.AltaxoVariant endValue)
    {
      long orgV, endV;
      DateTimeKind orgK, endK;

      if (orgValue.IsType(Data.AltaxoVariant.Content.VDateTime))
      {
        var dt = orgValue.ToDateTime();
        orgV = dt.Ticks;
        orgK = dt.Kind;
      }
      else if (orgValue.CanConvertedToDouble)
      {
        double v = orgValue.ToDouble();
        orgV = (long)(v * 1E7);
        orgK = DateTimeKind.Utc;
      }
      else
      {
        throw new InvalidOperationException("Can not convert orgValue to either a DateTime or a double value.");
      }

      if (endValue.IsType(Data.AltaxoVariant.Content.VDateTime))
      {
        var dt = endValue.ToDateTime();
        endV = dt.Ticks;
        endK = dt.Kind;
      }
      else if (orgValue.CanConvertedToDouble)
      {
        double v = endValue.ToDouble();
        endV = (long)(v * 1E7);
        endK = DateTimeKind.Utc;
      }
      else
      {
        throw new InvalidOperationException("Can not convert endValue to either a DateTime or a double value.");
      }

      SetUserParameters(orgRescaling, orgRelativeTo, orgV, orgK, endRescaling, endRelativeTo, endV, endK);
    }

    #endregion IScaleRescaleConditions implementation
  }
}
