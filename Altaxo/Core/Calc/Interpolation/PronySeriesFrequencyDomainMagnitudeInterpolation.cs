#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using Altaxo.Science.Signals;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Interpolation with a sum of Prony terms of the magnitude of a relaxation or retardation function in frequency domain.
  /// Note that for a relaxation the magnitude is increasing with frequency (e.g. the magnitude of the complex mechanical modulus),
  /// whereas for a retardation the magnitude is decreasing with frequency (e.g. the magnitude of the complex electrical permittivity).
  /// </summary>
  public record PronySeriesFrequencyDomainMagnitudeInterpolation : PronySeriesInterpolationBase, IInterpolationFunctionOptions
  {
    #region Serialization

    /// <summary>
    /// 2024-02-18 V0: initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PronySeriesFrequencyDomainMagnitudeInterpolation), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PronySeriesFrequencyDomainMagnitudeInterpolation)obj;
        s.SerializeV0(info);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new PronySeriesFrequencyDomainMagnitudeInterpolation().DeserializeV0(info);
      }
    }

    #endregion

    /// <inheritdoc/>
    public IInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev = null)
    {
      var (workingXMinimum, workingXMaximum, workingNumberOfPoints) = GetWorkingXMinMaxNumberOfPoints(xvec);

      if (!(workingXMinimum > 0))
        throw new InvalidOperationException($"{this.GetType()}: The minimum frequency must be > 0, but it is: {workingXMinimum}");

      if (IsRelaxation)
      {
        var fit = PronySeriesRelaxation.EvaluateFrequencyDomainFromMagnitude(
          xarr: xvec,
          isCircularFrequency: false,
          yMagnitude: yvec,
          tmin: 1 / (2 * Math.PI * workingXMaximum),
          tmax: 1 / (2 * Math.PI * workingXMinimum),
          numberOfRelaxationTimes: workingNumberOfPoints,
          withIntercept: UseIntercept,
          regularizationLambda: RegularizationParameter
          ); ;
        return new InterpolationResultDoubleWrapper((x) => fit.GetFrequencyDomainYOfFrequency(x).Real);
      }
      else
      {
        var fit = PronySeriesRetardation.EvaluateFrequencyDomainFromMagnitude(
          xarr: xvec,
          isCircularFrequency: false,
          yMagnitude: yvec,
          tmin: 1 / (2 * Math.PI * workingXMaximum),
          tmax: 1 / (2 * Math.PI * workingXMinimum),
          numberOfRetardationTimes: workingNumberOfPoints,
          withIntercept: UseIntercept,
          withFlowTerm: false,
          isRelativePermittivitySpectrum: false,
          regularizationLambda: RegularizationParameter);

        return new InterpolationResultDoubleWrapper((x) => fit.GetFrequencyDomainYOfFrequency(x).Real);
      }
    }

    /// <inheritdoc/>
    IInterpolationCurve IInterpolationCurveOptions.Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev)
    {
      return Interpolate(xvec, yvec, yStdDev);
    }
  }
}
