﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using Altaxo.Calc.Interpolation;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  public record MasterCurveGroupOptionsWithComplexInterpolation : MasterCurveGroupOptions
  {
    /// <summary>
    /// Gets the complex interpolation function.
    /// </summary>
    public IComplexInterpolation InterpolationFunction { get; init; } = new PronySeriesFrequencyDomainComplexInterpolation() { UseIntercept = true };

    /// <summary>
    /// Gets the fitting weight for the imaginary part.
    /// </summary>
    public double FittingWeightIm { get; init; } = 1;

    #region Serialization

    /// <summary>
    /// V0: 2024-02-16 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MasterCurveGroupOptionsWithComplexInterpolation), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MasterCurveGroupOptionsWithComplexInterpolation)obj;

        info.AddEnum("XShiftBy", s.XShiftBy);
        info.AddValue("LogarithmizeXForInterpolation", s.LogarithmizeXForInterpolation);
        info.AddValue("LogarithmizeYForInterpolation", s.LogarithmizeYForInterpolation);
        info.AddValue("FittingWeightRe", s.FittingWeight);
        info.AddValue("FittingWeightIm", s.FittingWeightIm);
        info.AddValue("Interpolation", s.InterpolationFunction);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var xShiftBy = info.GetEnum<ShiftXBy>("XShiftBy");
        var logarithmizeXForInterpolation = info.GetBoolean("LogarithmizeXForInterpolation");
        var logarithmizeYForInterpolation = info.GetBoolean("LogarithmizeYForInterpolation");
        var fittingWeightRe = info.GetDouble("FittingWeightRe");
        var fittingWeightIm = info.GetDouble("FittingWeightIm");
        var interpolation = info.GetValue<IComplexInterpolation>("Interpolation", null);

        return new MasterCurveGroupOptionsWithComplexInterpolation()
        {
          XShiftBy = xShiftBy,
          LogarithmizeXForInterpolation = logarithmizeXForInterpolation,
          LogarithmizeYForInterpolation = logarithmizeYForInterpolation,
          FittingWeight = fittingWeightRe,
          FittingWeightIm = fittingWeightIm,
          InterpolationFunction = interpolation,
        };
      }
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="MasterCurveGroupOptionsWithComplexInterpolation"/> class.
    /// </summary>
    public MasterCurveGroupOptionsWithComplexInterpolation()
    {
      LogarithmizeXForInterpolation = false;
      LogarithmizeYForInterpolation = false;
    }
  }
}
