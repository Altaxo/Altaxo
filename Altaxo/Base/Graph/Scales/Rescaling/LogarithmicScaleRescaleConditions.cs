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

using System;

namespace Altaxo.Graph.Scales.Rescaling
{
  /// <summary>
  /// Summary description for LogarithmicAxisRescaleConditions.
  /// </summary>
  public class LogarithmicScaleRescaleConditions : NumericScaleRescaleConditions
  {
    public const double DefaultOrgValue = 1;
    public const double DefaultEndValue = 10;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Axes.Scaling.LogarithmicAxisRescaleConditions", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.Rescaling.LogarithmicAxisRescaleConditions", 1)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
				LogarithmicScaleRescaleConditions s = (LogarithmicScaleRescaleConditions)obj;

				info.AddBaseValueEmbedded(s, s.GetType().BaseType);
				*/
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LogarithmicScaleRescaleConditions s = null != o ? (LogarithmicScaleRescaleConditions)o : new LogarithmicScaleRescaleConditions();

        info.GetBaseValueEmbedded(s, "AltaxoBase,Altaxo.Graph.Scales.Rescaling.NumericAxisRescaleConditions,1", parent);

        return s;
      }
    }

    /// <summary>
    /// 2015-02-14 Added because in the former version there exists some versions were the base type was not embedded with the fully type and version.
    /// Thus to deserialize the former version we have to call GetBaseValueEmbedded with the fully qualified name.
    /// 2nd reason is that we renamed from LogarithmicAxisRescaleConditions to LogarithmicScaleRescaleConditions
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LogarithmicScaleRescaleConditions), 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LogarithmicScaleRescaleConditions s = (LogarithmicScaleRescaleConditions)obj;

        info.AddBaseValueEmbedded(s, s.GetType().BaseType);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LogarithmicScaleRescaleConditions s = null != o ? (LogarithmicScaleRescaleConditions)o : new LogarithmicScaleRescaleConditions();

        info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);

        return s;
      }
    }

    #endregion Serialization

    public LogarithmicScaleRescaleConditions()
    {
      _dataBoundsOrg = _resultingOrg = DefaultOrgValue;
      _dataBoundsEnd = _resultingEnd = DefaultEndValue;
    }

    public LogarithmicScaleRescaleConditions(LogarithmicScaleRescaleConditions from)
      : base(from) // all is done here, since CopyFrom is virtual!
    {
    }

    public override object Clone()
    {
      return new LogarithmicScaleRescaleConditions(this);
    }

    public override void SetUserParameters(BoundaryRescaling orgRescaling, BoundariesRelativeTo orgRelativeTo, double orgValue, BoundaryRescaling endRescaling, BoundariesRelativeTo endRelativeTo, double endValue)
    {
      if (double.IsNaN(orgValue) || !Altaxo.Calc.RMath.IsFinite(orgValue) || orgValue <= 0)
      {
        if (orgRescaling == BoundaryRescaling.Auto)
          orgValue = DefaultOrgValue; // ignore this error and set org to 1
        else
          throw new ArgumentOutOfRangeException("orgValue should be a finite and positive number but is " + orgValue.ToString());
      }

      if (double.IsNaN(endValue) || !Altaxo.Calc.RMath.IsFinite(endValue) || endValue <= 0)
      {
        if (endRescaling == BoundaryRescaling.Auto)
          endValue = DefaultEndValue; // ignore this error and set org to 10
        else
          throw new ArgumentOutOfRangeException("endValue should be a finite and positive number but is " + endValue.ToString());
      }

      base.SetUserParameters(orgRescaling, orgRelativeTo, orgValue, endRescaling, endRelativeTo, endValue);
    }

    /// <summary>
    /// Fixes the data bounds org and end. Here we modify the bounds if org and end are equal.
    /// </summary>
    /// <param name="dataBoundsOrg">The data bounds org.</param>
    /// <param name="dataBoundsEnd">The data bounds end.</param>
    protected override void FixValuesForDataBoundsOrgAndEnd(ref double dataBoundsOrg, ref double dataBoundsEnd)
    {
      // ensure that data bounds always have some distance
      if (dataBoundsOrg == dataBoundsEnd)
      {
        dataBoundsOrg = dataBoundsOrg / 10;
        dataBoundsEnd = dataBoundsEnd * 10;
      }
    }

    protected override double GetDataBoundsScaleMean()
    {
      return Math.Sqrt(_dataBoundsOrg * _dataBoundsEnd);
    }

    #region Resulting Org/End to/fron User Org/End

    protected override double GetResultingOrgFromUserProvidedOrg()
    {
      switch (_userProvidedOrgRelativeTo)
      {
        case BoundariesRelativeTo.Absolute:
          return _userProvidedOrgValue;

        case BoundariesRelativeTo.RelativeToDataBoundsOrg:
          return _userProvidedOrgValue * _dataBoundsOrg;

        case BoundariesRelativeTo.RelativeToDataBoundsEnd:
          return _userProvidedOrgValue * _dataBoundsEnd;

        case BoundariesRelativeTo.RelativeToDataBoundsMean:
          return _userProvidedOrgValue * GetDataBoundsScaleMean();

        default:
          throw new NotImplementedException();
      }
    }

    protected override double GetUserProvidedOrgFromResultingOrg(double resultingOrg)
    {
      switch (_userProvidedOrgRelativeTo)
      {
        case BoundariesRelativeTo.Absolute:
          return resultingOrg;

        case BoundariesRelativeTo.RelativeToDataBoundsOrg:
          return resultingOrg / _dataBoundsOrg;

        case BoundariesRelativeTo.RelativeToDataBoundsEnd:
          return resultingOrg / _dataBoundsEnd;

        case BoundariesRelativeTo.RelativeToDataBoundsMean:
          return resultingOrg / GetDataBoundsScaleMean();

        default:
          throw new NotImplementedException();
      }
    }

    protected override double GetResultingEndFromUserProvidedEnd()
    {
      switch (_userProvidedEndRelativeTo)
      {
        case BoundariesRelativeTo.Absolute:
          return _userProvidedEndValue;

        case BoundariesRelativeTo.RelativeToDataBoundsOrg:
          return _userProvidedEndValue * _dataBoundsOrg;

        case BoundariesRelativeTo.RelativeToDataBoundsEnd:
          return _userProvidedEndValue * _dataBoundsEnd;

        case BoundariesRelativeTo.RelativeToDataBoundsMean:
          return _userProvidedEndValue * GetDataBoundsScaleMean();

        default:
          throw new NotImplementedException();
      }
    }

    protected override double GetUserProvidedEndFromResultingEnd(double resultingEnd)
    {
      switch (_userProvidedEndRelativeTo)
      {
        case BoundariesRelativeTo.Absolute:
          return resultingEnd;

        case BoundariesRelativeTo.RelativeToDataBoundsOrg:
          return resultingEnd / _dataBoundsOrg;

        case BoundariesRelativeTo.RelativeToDataBoundsEnd:
          return resultingEnd / _dataBoundsEnd;

        case BoundariesRelativeTo.RelativeToDataBoundsMean:
          return resultingEnd / GetDataBoundsScaleMean();

        default:
          throw new NotImplementedException();
      }
    }

    #endregion Resulting Org/End to/fron User Org/End

    protected override void FixValuesForUserZoomed(ref double zoomOrg, ref double zoomEnd)
    {
      if (zoomOrg > zoomEnd)
      {
        var h = zoomOrg;
        zoomOrg = zoomEnd;
        zoomEnd = h;
      }

      if (zoomOrg == zoomEnd)
      {
        if (zoomOrg <= 0 || zoomEnd <= 0)
        {
          zoomOrg = 1;
          zoomEnd = 10;
        }
        else
        {
          zoomOrg = zoomOrg - 0.5 * Math.Abs(zoomOrg);
          zoomEnd = zoomEnd + 0.5 * Math.Abs(zoomOrg);
        }
      }
      else if (zoomOrg <= 0 || zoomEnd <= 0)
      {
        zoomOrg = 1;
        zoomEnd = 10;
      }
    }

    #region Helper functions for dialog

    public double GetOrgValueToShowInDialog(double currentResultingOrg)
    {
      if (this._orgRescaling == BoundaryRescaling.Auto)
      {
        switch (this._userProvidedOrgRelativeTo)
        {
          case BoundariesRelativeTo.Absolute:
            return currentResultingOrg;

          case BoundariesRelativeTo.RelativeToDataBoundsOrg:
            return currentResultingOrg / _dataBoundsOrg;

          case BoundariesRelativeTo.RelativeToDataBoundsEnd:
            return currentResultingOrg / _dataBoundsEnd;

          case BoundariesRelativeTo.RelativeToDataBoundsMean:
            return currentResultingOrg / Math.Sqrt(_dataBoundsOrg * _dataBoundsEnd);

          default:
            throw new NotImplementedException();
        }
      }
      else
      {
        return this._userProvidedOrgValue;
      }
    }

    public double GetEndValueToShowInDialog(double currentResultingEnd)
    {
      if (this._endRescaling == BoundaryRescaling.Auto)
      {
        switch (this._userProvidedEndRelativeTo)
        {
          case BoundariesRelativeTo.Absolute:
            return currentResultingEnd;

          case BoundariesRelativeTo.RelativeToDataBoundsOrg:
            return currentResultingEnd / _dataBoundsOrg;

          case BoundariesRelativeTo.RelativeToDataBoundsEnd:
            return currentResultingEnd / _dataBoundsEnd;

          case BoundariesRelativeTo.RelativeToDataBoundsMean:
            return currentResultingEnd / Math.Sqrt(_dataBoundsOrg * _dataBoundsEnd);

          default:
            throw new NotImplementedException();
        }
      }
      else
      {
        return this._userProvidedEndValue;
      }
    }

    #endregion Helper functions for dialog
  }
}
