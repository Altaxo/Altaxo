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
  /// Interpolation with a sum of Prony terms of the real part of a relaxation or retardation function in the frequency domain.
  /// </summary>
  /// <remarks>
  /// Note that for a relaxation the real part is increasing with frequency (e.g. real part of mechanical modulus),
  /// whereas for a retardation the real part is decreasing with frequency (e.g. real part of electrical permittivity).
  /// </remarks>
  public record PronySeriesFrequencyDomainRealPartInterpolation : PronySeriesInterpolationBase, IInterpolationFunctionOptions
  {
    #region Serialization

    /// <summary>
    /// Serialization surrogate for version 0.
    /// </summary>
    /// <remarks>
    /// 2024-02-18 V0: initial version.
    /// </remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PronySeriesFrequencyDomainRealPartInterpolation), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PronySeriesFrequencyDomainRealPartInterpolation)obj;
        s.SerializeV0(info);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new PronySeriesFrequencyDomainRealPartInterpolation().DeserializeV0(info);
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
        var fit = PronySeriesRelaxation.EvaluateFrequencyDomain(
          xarr: xvec,
          isCircularFrequency: false,
          yarrRe: yvec,
          yarrIm: null,
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
        var fit = PronySeriesRetardation.EvaluateFrequencyDomain(
          xarr: xvec,
          isCircularFrequency: false,
          yarrRe: yvec,
          yarrIm: null,
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
