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

using System.Collections.Generic;
using System.Linq;
using Altaxo.Science.Signals;
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Interpolation with a sum of Prony terms of a complex relaxation or retardation function in frequency domain.
  /// Note that for a relaxation the real part is increasing with frequency (e.g. complex mechanical modulus), whereas for a retardation the real part is decreasing with frequency (e.g. complex electrical permittivity).
  /// We assume here that even for a retardation the imaginary part is positive: eps* = eps' - i eps''.
  /// </summary>
  public record PronySeriesFrequencyDomainComplexInterpolation : PronySeriesInterpolationBase, IComplexInterpolation
  {

    public IComplexInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<Complex64> yvec, IReadOnlyList<Complex64>? yStdDev = null)
    {
      var (workingXMinimum, workingXMaximum, workingNumberOfPoints) = GetWorkingXMinMaxNumberOfPoints(xvec);

      if (IsRelaxation)
      {
        var fit = PronySeriesRelaxation.EvaluateFrequencyDomain(
          xvec,
          isCircularFrequency: false,
          yvec.Select(y => y.Real).ToArray(),
          yvec.Select(y => y.Imaginary).ToArray(),
          workingXMinimum,
          workingXMaximum,
          workingNumberOfPoints,
          withIntercept: UseIntercept,
          regularizationLambda: RegularizationParameter
          );
        return new InterpolationResultComplexWrapper(fit.GetFrequencyDomainYOfFrequency);
      }
      else
      {
        var fit = PronySeriesRetardation.EvaluateFrequencyDomain(
          xvec,
          isCircularFrequency: false,
          yvec.Select(y => y.Real).ToArray(),
          yvec.Select(y => y.Imaginary).ToArray(),
          workingXMinimum,
          workingXMaximum,
          workingNumberOfPoints,
          withIntercept: UseIntercept,
          withFlowTerm: false,
          isRelativePermittivitySpectrum: false,
          regularizationLambda: RegularizationParameter);

        return new InterpolationResultComplexWrapper(fit.GetFrequencyDomainYOfFrequency);
      }
    }

    public IComplexInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yreal, IReadOnlyList<double> yimaginary)
    {
      var (workingXMinimum, workingXMaximum, workingNumberOfPoints) = GetWorkingXMinMaxNumberOfPoints(xvec);

      if (IsRelaxation)
      {
        var fit = PronySeriesRelaxation.EvaluateFrequencyDomain(
          xvec,
          isCircularFrequency: false,
          yreal,
          yimaginary,
          workingXMinimum,
          workingXMaximum,
          workingNumberOfPoints,
          withIntercept: UseIntercept,
          regularizationLambda: RegularizationParameter
          );
        return new InterpolationResultComplexWrapper(fit.GetFrequencyDomainYOfFrequency);
      }
      else
      {
        var fit = PronySeriesRetardation.EvaluateFrequencyDomain(
          xvec,
          isCircularFrequency: false,
          yreal,
          yimaginary,
          workingXMinimum,
          workingXMaximum,
          workingNumberOfPoints,
          withIntercept: UseIntercept,
          withFlowTerm: false,
          isRelativePermittivitySpectrum: false,
          regularizationLambda: RegularizationParameter);

        return new InterpolationResultComplexWrapper(fit.GetFrequencyDomainYOfFrequency);
      }
    }

    public bool IsSupportingSeparateXForRealAndImaginaryPart => false;


    public IComplexInterpolationFunction Interpolate(IReadOnlyList<double> xreal, IReadOnlyList<double> yreal, IReadOnlyList<double> ximaginary, IReadOnlyList<double> yimaginary)
    {
      throw new System.NotImplementedException($"Please ensure that the value of {IsSupportingSeparateXForRealAndImaginaryPart} is true before calling this function");
    }
  }
}
