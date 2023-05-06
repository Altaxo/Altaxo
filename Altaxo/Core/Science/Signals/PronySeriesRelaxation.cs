#region Copyright

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

using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Complex64 = System.Numerics.Complex;


namespace Altaxo.Science.Signals
{
  /// <summary>
  /// Performs a fit with a Prony series to a relaxation signal, either in the time domain or in the frequency domain.
  /// If the signal is a time domain signal, it is assumed to be a relaxation signal, i.e. is decreasing with time.
  /// If the signal is in the frequency domain, it is assumed to be a modulus, i.e. the real part is increasing with frequency.
  /// </summary>
  public record PronySeriesRelaxation
  {
    private double _timeMinimum = 1;

    /// <summary>
    /// Gets or sets smallest relaxation time (the tau_relax of the first Prony term).
    /// </summary>
    public double TimeMinimum
    {
      get => _timeMinimum;
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException(nameof(TimeMinimum), "Must be > 0");

        _timeMinimum = value;
      }
    }

    private double _timeMaximum = 1;

    /// <summary>
    /// Gets or sets largest relaxation time (the tau_relax of the last Prony term).
    /// </summary>
    public double TimeMaximum
    {
      get => _timeMaximum;
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException(nameof(TimeMaximum), "Must be > 0");
        _timeMaximum = value;
      }
    }


    private int _numberOfRelaxationTimes = 1;

    /// <summary>
    /// Gets the number of relaxation times, i.e. the number of Prony terms.
    /// </summary>
    public int NumberOfRelaxationTimes
    {
      get => _numberOfRelaxationTimes;
      init
      {
        if (!(value >= 1))
          throw new ArgumentOutOfRangeException(nameof(NumberOfRelaxationTimes), "Must be >= 1");

        _numberOfRelaxationTimes = value;
      }
    }



    private double _regularizationParameter;

    /// <summary>
    /// Gets/sets the regularization parameter. Usually zero. The higher the value, the more are the prony coefficients smoothed out.
    /// </summary>
    public double RegularizationParameter
    {
      get => _regularizationParameter;
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(RegularizationParameter), "Must be >= 0");

        _regularizationParameter = value;
      }
    }

    /// <summary>
    /// If true, an intercept (low frequency value) is also fitted. In the result this is included as the Prony coefficient for relaxation time infinity.
    /// </summary>
    public bool UseIntercept { get; init; } = true;


    /// <summary>
    /// Evaluates a prony series fit in the time domain, using the properties <see cref="TimeMinimum"/>, <see cref="TimeMaximum"/>, <see cref="NumberOfRelaxationTimes"/> and <see cref="RegularizationParameter"/>.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="yarr">The y-values of the signal.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRelaxationResult"/>.</returns>
    public PronySeriesRelaxationResult EvaluateTimeDomain(IReadOnlyList<double> xarr, IReadOnlyList<double> yarr)
    {
      return EvaluateTimeDomain(xarr, yarr, TimeMinimum, TimeMaximum, NumberOfRelaxationTimes, UseIntercept, RegularizationParameter);
    }

    /// <summary>
    /// Evaluates a prony series fit in the time domain, using the properties <see cref="TimeMinimum"/>, <see cref="TimeMaximum"/>, <see cref="NumberOfRelaxationTimes"/> and <see cref="RegularizationParameter"/>.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="isCircularFrequency">True if xarr contains circular frequencies; false if xarr contains normal frequencies.</param>
    /// <param name="yarrRe">The real part of the modulus.</param>
    /// <param name="yarrIm">The imaginary part of the modulus.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRelaxationResult"/>.</returns>
    public PronySeriesRelaxationResult EvaluateFrequencyDomain(IReadOnlyList<double> xarr, bool isCircularFrequency, IReadOnlyList<double>? yarrRe, IReadOnlyList<double>? yarrIm)
    {
      return EvaluateFrequencyDomain(xarr, isCircularFrequency, yarrRe, yarrIm, TimeMinimum, TimeMaximum, NumberOfRelaxationTimes, UseIntercept, RegularizationParameter);
    }

    /// <summary>
    /// Evaluates a prony series fit in the time domain, using the properties <see cref="TimeMinimum"/>, <see cref="TimeMaximum"/>, <see cref="NumberOfRelaxationTimes"/> and <see cref="RegularizationParameter"/>.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="isCircularFrequency">True if xarr contains circular frequencies; false if xarr contains normal frequencies.</param>
    /// <param name="yarr">The complex modulus.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRelaxationResult"/>.</returns>
    public PronySeriesRelaxationResult EvaluateFrequencyDomain(IReadOnlyList<double> xarr, bool isCircularFrequency, IReadOnlyList<Complex64> yarr)
    {
      return EvaluateFrequencyDomain(xarr, isCircularFrequency, yarr.Select(c => c.Real).ToArray(), yarr.Select(c => c.Imaginary).ToArray(), TimeMinimum, TimeMaximum, NumberOfRelaxationTimes, UseIntercept, RegularizationParameter);
    }

    /// <summary>
    /// Evaluates a prony series fit in the time domain.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="yarr">The y-values of the signal.</param>
    /// <param name="tmin">The smallest relaxation time (tau of the first Prony term).</param>
    /// <param name="tmax">The largest relaxation time (tau of the last Prony term).</param>
    /// <param name="numberOfRelaxationTimes">The number of relaxation times (number of Prony terms).</param>
    /// <param name="withIntercept">If set to <c>true</c>, an offset term is added. This term can be considered to have a infinite relaxation time.</param>
    /// <param name="regularizationLambda">A regularization parameter to smooth the resulting array of Prony terms.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRelaxationResult"/>.</returns>
    public static PronySeriesRelaxationResult EvaluateTimeDomain(IReadOnlyList<double> xarr, IReadOnlyList<double> yarr, double tmin, double tmax, int numberOfRelaxationTimes, bool withIntercept, double regularizationLambda)
    {
      if (xarr is null)
        throw new ArgumentNullException(nameof(xarr));
      if (yarr is null)
        throw new ArgumentNullException(nameof(yarr));
      if (xarr.Count != yarr.Count)
        throw new ArgumentOutOfRangeException(nameof(yarr), "yarr should have the same length than xarr");
      if (!(tmin > 0))
        throw new ArgumentOutOfRangeException(nameof(tmin), "Must be > 0");
      if (!(tmax > tmin))
        throw new ArgumentOutOfRangeException(nameof(tmax), "Must be > xmin");
      if (!(numberOfRelaxationTimes >= 1))
        throw new ArgumentOutOfRangeException(nameof(numberOfRelaxationTimes), "Must be >= 1");
      if (!(tmax == tmin || numberOfRelaxationTimes >= 2))
        throw new ArgumentOutOfRangeException(nameof(numberOfRelaxationTimes), "Must be >= 2");

      // evaluate the relaxation times (logarithmically spaced)
      double[] taus = new double[numberOfRelaxationTimes];
      for (int c = 0; c < numberOfRelaxationTimes; ++c)
      {
        double r = c == 0 ? 0 : c / (double)(numberOfRelaxationTimes - 1);
        double lntau = (1 - r) * Math.Log(tmin) + r * Math.Log(tmax);
        taus[c] = Math.Exp(lntau);
      }

      int NR = xarr.Count;
      int NC = numberOfRelaxationTimes + (withIntercept ? 1 : 0); // one more column for the intercept

      // Basis functions		
      var X = Matrix<double>.Build.Dense(NR + numberOfRelaxationTimes, NC);
      for (int c = 0; c < numberOfRelaxationTimes; ++c)
      {
        for (int r = 0; r < NR; ++r)
        {
          X[r, c] = Math.Exp(-xarr[r] / taus[c]);
        }
      }

      // Intercept
      if (withIntercept)
      {
        for (int r = 0; r < NR; ++r)
        {
          X[r, NC - 1] = 1; // Basisfunktion für den Offset
        }
      }

      // Regularization
      for (int r = NR; r < NR + numberOfRelaxationTimes - 2; ++r)
      {
        X[r, r - NR] = regularizationLambda;
        X[r, r - NR + 1] = -2 * regularizationLambda;
        X[r, r - NR + 2] = regularizationLambda;
      }

      // read dependent variable to matrix y
      var y = Matrix<double>.Build.Dense(NR + numberOfRelaxationTimes, 1);
      for (int r = 0; r < NR; ++r)
        y[r, 0] = yarr[r];

      return Evaluate(tmin, tmax, numberOfRelaxationTimes, withIntercept, taus, X, y);
    }

    /// <summary>
    /// Evaluates a prony series fit in the time domain.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="isCircularFrequency">True if xarr contains circular frequencies; false if xarr contains normal frequencies.</param>
    /// <param name="yarrRe">The real part of the modulus.</param>
    /// <param name="yarrIm">The imaginary part of the modulus.</param>
    /// <param name="tmin">The smallest relaxation time (tau of the first Prony term).</param>
    /// <param name="tmax">The largest relaxation time (tau of the last Prony term).</param>
    /// <param name="numberOfRelaxationTimes">The number of relaxation times (number of Prony terms).</param>
    /// <param name="withIntercept">If set to <c>true</c>, an offset term is added. This term can be considered to have a infinite relaxation time.</param>
    /// <param name="regularizationLambda">A regularization parameter to smooth the resulting array of Prony terms.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRelaxationResult"/>.</returns>
    public static PronySeriesRelaxationResult EvaluateFrequencyDomain(IReadOnlyList<double> xarr, bool isCircularFrequency, IReadOnlyList<double>? yarrRe, IReadOnlyList<double>? yarrIm, double tmin, double tmax, int numberOfRelaxationTimes, bool withIntercept, double regularizationLambda)
    {
      var bothReAndIm = yarrRe is not null && yarrIm is not null;
      if (xarr is null)
        throw new ArgumentNullException(nameof(xarr));
      if (yarrRe is null && yarrIm is null)
        throw new ArgumentNullException($"Either {nameof(yarrRe)} or {nameof(yarrIm)} must be not null");
      if (yarrRe is not null && yarrIm is not null && yarrRe.Count != yarrIm.Count)
        throw new ArgumentOutOfRangeException(nameof(yarrIm), "yarrRe should have the same length than yarrIm");
      if (xarr.Count != Math.Max(yarrRe?.Count ?? 0, yarrIm?.Count ?? 0))
        throw new ArgumentOutOfRangeException(nameof(yarrRe), "yarr should have the same length than xarr");
      if (!(tmin > 0))
        throw new ArgumentOutOfRangeException(nameof(tmin), "Must be > 0");
      if (!(tmax > tmin))
        throw new ArgumentOutOfRangeException(nameof(tmax), "Must be > xmin");
      if (!(numberOfRelaxationTimes >= 1))
        throw new ArgumentOutOfRangeException(nameof(numberOfRelaxationTimes), "Must be >= 1");
      if (!(tmax == tmin || numberOfRelaxationTimes >= 2))
        throw new ArgumentOutOfRangeException(nameof(numberOfRelaxationTimes), "Must be >= 2");

      withIntercept &= yarrRe is not null; // intercept only if real part is given

      // evaluate the relaxation times (logarithmically spaced)
      double[] taus = new double[numberOfRelaxationTimes];
      for (int c = 0; c < numberOfRelaxationTimes; ++c)
      {
        double r = c == 0 ? 0 : c / (double)(numberOfRelaxationTimes - 1);
        double lntau = (1 - r) * Math.Log(tmin) + r * Math.Log(tmax);
        taus[c] = Math.Exp(lntau);
      }

      int xCount = xarr.Count;
      int NR = bothReAndIm ? 2 * xarr.Count : xarr.Count;
      int NC = numberOfRelaxationTimes + (withIntercept ? 1 : 0); // one more column for the intercept (intercept is only in the real part)

      // Basis functions		
      var X = Matrix<double>.Build.Dense(NR + numberOfRelaxationTimes, NC);
      for (int c = 0; c < numberOfRelaxationTimes; ++c)
      {
        for (int r = 0; r < NR; ++r)
        {
          var tauomega = taus[c] * xarr[r % xCount] * (isCircularFrequency ? 1 : 2 * Math.PI);
          var g = tauomega / (tauomega - Complex64.ImaginaryOne);
          if (bothReAndIm)
          {
            X[r, c] = r < xCount ? g.Real : g.Imaginary;
          }
          else
          {
            X[r, c] = yarrRe is not null ? g.Real : g.Imaginary;
          }
        }
      }

      // Intercept
      if (withIntercept)
      {
        for (int r = 0; r < xCount; ++r) // set intercept only for real part -> only for the first half
        {
          X[r, NC - 1] = 1; // base function for the intercept
        }
      }

      // Regularization
      for (int r = NR; r < NR + numberOfRelaxationTimes - 2; ++r)
      {
        X[r, r - NR] = regularizationLambda;
        X[r, r - NR + 1] = -2 * regularizationLambda;
        X[r, r - NR + 2] = regularizationLambda;
      }

      // read dependent variable to matrix y
      var y = Matrix<double>.Build.Dense(NR + numberOfRelaxationTimes, 1);

      if (yarrRe is not null && yarrIm is not null)
      {
        for (int r = 0; r < xCount; ++r)
          y[r, 0] = yarrRe[r]; // real part has to go first, because the intercept is only in the first half
        for (int r = xCount; r < NR; ++r)
          y[r, 0] = yarrRe[r];
      }
      else if (yarrRe is not null)
      {
        for (int r = 0; r < NR; ++r)
          y[r, 0] = yarrRe[r];
      }
      else if (yarrIm is not null)
      {
        for (int r = 0; r < NR; ++r)
          y[r, 0] = yarrIm[r];
      }
      else
      {
        throw new InvalidOperationException();
      }

      return Evaluate(tmin, tmax, numberOfRelaxationTimes, withIntercept, taus, X, y);
    }

    protected static PronySeriesRelaxationResult Evaluate(double tmin, double tmax, int numberOfRelaxationTimes, bool withIntercept, double[] taus, Matrix<double> X, Matrix<double> y)
    {
      // calculate XtX and XtY
      var XtX = X.TransposeThisAndMultiply(X);
      var Xty = X.TransposeThisAndMultiply(y);

      // Solve the equation, but get nonnegative coefficients only
      FastNonnegativeLeastSquares.Execution(XtX, Xty, null, out var x, out var _);

      // the result (the spectral amplitudes) are now in X
      double spectralDensityFactor = Math.Log(tmax / tmin) / (numberOfRelaxationTimes - 1);
      var resultTauCol = withIntercept ? taus.Concat(new double[] { double.PositiveInfinity }).ToArray() : taus.ToArray();
      var resultPronyCol = new double[resultTauCol.Length];
      var resultRelaxationDensityCol = new double[resultTauCol.Length];

      for (int i = 0; i < resultTauCol.Length; ++i)
      {
        resultPronyCol[i] = x[i, 0];
        resultRelaxationDensityCol[i] = x[i, 0] / spectralDensityFactor;
      }

      var result = new PronySeriesRelaxationResult(resultTauCol, resultPronyCol, resultRelaxationDensityCol);
      return result;
    }
  }
}
