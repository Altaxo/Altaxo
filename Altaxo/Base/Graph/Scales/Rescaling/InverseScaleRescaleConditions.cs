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
  /// Stores rescaling conditions for inverse scales.
  /// </summary>
  [Serializable]
  public class InverseScaleRescaleConditions : NumericScaleRescaleConditions
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.Rescaling.InverseAxisRescaleConditions", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(InverseScaleRescaleConditions), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (InverseScaleRescaleConditions)obj;

        info.AddBaseValueEmbedded(s, s.GetType().BaseType!);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (InverseScaleRescaleConditions?)o ?? new InverseScaleRescaleConditions();

        info.GetBaseValueEmbedded(s, s.GetType().BaseType!, parent);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="InverseScaleRescaleConditions"/> class.
    /// </summary>
    public InverseScaleRescaleConditions()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InverseScaleRescaleConditions"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public InverseScaleRescaleConditions(InverseScaleRescaleConditions from)
      : base(from) // all is done here, since CopyFrom is virtual!
    {
    }

    /// <inheritdoc/>
    public override object Clone()
    {
      return new InverseScaleRescaleConditions(this);
    }

    /// <inheritdoc/>
    public override double ResultingOrg
    {
      get
      {
        return 1 / _resultingOrg;
      }
    }

    /// <summary>
    /// Gets the resulting origin in inverse representation.
    /// </summary>
    public double ResultingInverseOrg
    {
      get
      {
        return _resultingOrg;
      }
    }

    /// <inheritdoc/>
    public override double ResultingEnd
    {
      get
      {
        return 1 / _resultingEnd;
      }
    }

    /// <summary>
    /// Gets the resulting end in inverse representation.
    /// </summary>
    public double ResultingInverseEnd
    {
      get
      {
        return _resultingEnd;
      }
    }

    /// <inheritdoc/>
    public override double UserProvidedOrgValue
    {
      get
      {
        return 1 / _userProvidedOrgValue;
      }
    }

    /// <inheritdoc/>
    public override double UserProvidedEndValue
    {
      get
      {
        return 1 / _userProvidedEndValue;
      }
    }

    /// <inheritdoc/>
    public override void SetUserParameters(BoundaryRescaling orgRescaling, BoundariesRelativeTo orgRelativeTo, double orgValue, BoundaryRescaling endRescaling, BoundariesRelativeTo endRelativeTo, double endValue)
    {
      orgValue = 1 / orgValue;
      endValue = 1 / endValue;

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

    /// <summary>
    /// Fixes the data bounds org and end. Here we modify the bounds if org and end are equal.
    /// </summary>
    /// <param name="dataBoundsOrg">The data bounds org.</param>
    /// <param name="dataBoundsEnd">The data bounds end.</param>
    protected override void FixValuesForDataBoundsOrgAndEnd(ref double dataBoundsOrg, ref double dataBoundsEnd)
    {
      if (0 == dataBoundsOrg || 0 == dataBoundsEnd)
        throw new ArgumentOutOfRangeException("Either dataBoundsOrg or dataBoundsEnd is null. This should not happend when InverseNumericalBoundaries were used.");

      dataBoundsOrg = 1 / dataBoundsOrg; // invert the boundaries
      dataBoundsEnd = 1 / dataBoundsEnd;

      // ensure that data bounds always have some distance
      if (dataBoundsOrg == dataBoundsEnd)
      {
        if (0 == dataBoundsOrg)
        {
          dataBoundsOrg = -1;
          dataBoundsEnd = 1;
        }
        else
        {
          var offs = 0.5 * Math.Abs(dataBoundsOrg);
          dataBoundsOrg = dataBoundsOrg - offs;
          dataBoundsEnd = dataBoundsEnd + offs;
        }
      }
    }

    /// <inheritdoc/>
    protected override void FixValuesForUserZoomed(ref double zoomOrg, ref double zoomEnd)
    {
      zoomOrg = 1 / zoomOrg;
      zoomEnd = 1 / zoomEnd;

      if (zoomOrg == zoomEnd)
      {
        zoomOrg = -1;
        zoomEnd = 1;
      }
      else if (zoomOrg > zoomEnd)
      {
        var h = zoomOrg;
        zoomOrg = zoomEnd;
        zoomEnd = h;
      }
    }

    /// <inheritdoc/>
    protected override double GetDataBoundsScaleMean()
    {
      return 0.5 * (_dataBoundsOrg + _dataBoundsEnd);
    }

    #region Resulting Org/End to/fron User Org/End

    /// <inheritdoc/>
    protected override double GetResultingOrgFromUserProvidedOrg()
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

    /// <inheritdoc/>
    protected override double GetUserProvidedOrgFromResultingOrg(double resultingOrg)
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

    /// <inheritdoc/>
    protected override double GetResultingEndFromUserProvidedEnd()
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

    /// <inheritdoc/>
    protected override double GetUserProvidedEndFromResultingEnd(double resultingEnd)
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

    #region Helper functions for dialog

    /// <summary>
    /// Gets the origin value that should be shown in the rescaling dialog.
    /// </summary>
    /// <param name="currentResultingInverseOrg">The current resulting inverse origin.</param>
    /// <returns>The origin value to display.</returns>
    public double GetOrgValueToShowInDialog(double currentResultingInverseOrg)
    {
      if (_orgRescaling == BoundaryRescaling.Auto)
      {
        switch (_userProvidedOrgRelativeTo)
        {
          case BoundariesRelativeTo.Absolute:
            return currentResultingInverseOrg;

          case BoundariesRelativeTo.RelativeToDataBoundsOrg:
            return currentResultingInverseOrg - _dataBoundsOrg;

          case BoundariesRelativeTo.RelativeToDataBoundsEnd:
            return currentResultingInverseOrg - _dataBoundsEnd;

          case BoundariesRelativeTo.RelativeToDataBoundsMean:
            return currentResultingInverseOrg - 0.5 * (_dataBoundsOrg + _dataBoundsEnd);

          default:
            throw new NotImplementedException();
        }
      }
      else
      {
        return _userProvidedOrgValue;
      }
    }

    /// <summary>
    /// Gets the end value that should be shown in the rescaling dialog.
    /// </summary>
    /// <param name="currentResultingInverseEnd">The current resulting inverse end.</param>
    /// <returns>The end value to display.</returns>
    public double GetEndValueToShowInDialog(double currentResultingInverseEnd)
    {
      if (_endRescaling == BoundaryRescaling.Auto)
      {
        switch (_userProvidedEndRelativeTo)
        {
          case BoundariesRelativeTo.Absolute:
            return currentResultingInverseEnd;

          case BoundariesRelativeTo.RelativeToDataBoundsOrg:
            return currentResultingInverseEnd - _dataBoundsOrg;

          case BoundariesRelativeTo.RelativeToDataBoundsEnd:
            return currentResultingInverseEnd - _dataBoundsEnd;

          case BoundariesRelativeTo.RelativeToDataBoundsMean:
            return currentResultingInverseEnd - 0.5 * (_dataBoundsOrg + _dataBoundsEnd);

          default:
            throw new NotImplementedException();
        }
      }
      else
      {
        return _userProvidedEndValue;
      }
    }

    #endregion Helper functions for dialog
  }
}
