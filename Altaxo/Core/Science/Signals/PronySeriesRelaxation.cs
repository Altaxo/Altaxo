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
  public record PronySeriesRelaxation : Main.IImmutable
  {
    private double _timeMinimum = 1;

    /// <summary>
    /// Gets or sets smallest relaxation time (the tau_relax of the first Prony term).
    /// </summary>
    public double MinimalRelaxationTime
    {
      get => _timeMinimum;
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException(nameof(MinimalRelaxationTime), "Must be > 0");

        _timeMinimum = value;
      }
    }

    private double _timeMaximum = 1;

    /// <summary>
    /// Gets or sets largest relaxation time (the tau_relax of the last Prony term).
    /// </summary>
    public double MaximalRelaxationTime
    {
      get => _timeMaximum;
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException(nameof(MaximalRelaxationTime), "Must be > 0");
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


    #region Serialization

    /// <summary>
    /// 2023-05-15 initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PronySeriesRelaxation), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PronySeriesRelaxation)obj;
        info.AddValue("MinimalRelaxationTime", s.MinimalRelaxationTime);
        info.AddValue("MaximalRelaxationTime", s.MaximalRelaxationTime);
        info.AddValue("NumberOfRelaxationTimes", s.NumberOfRelaxationTimes);
        info.AddValue("UseIntercept", s.UseIntercept);
        info.AddValue("RegularizationParameter", s.RegularizationParameter);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var minimalValue = info.GetDouble("MinimalRelaxationTime");
        var maximalValue = info.GetDouble("MMaximalRelaxationTime");
        var numberRelax = info.GetInt32("NumberOfRelaxationTimes");
        var intercept = info.GetBoolean("UseIntercept");
        var regularization = info.GetDouble("RegularizationParameter");

        return o is null ? new PronySeriesRelaxation
        {
          MinimalRelaxationTime = minimalValue,
          MaximalRelaxationTime = maximalValue,
          NumberOfRelaxationTimes = numberRelax,
          UseIntercept = intercept,
          RegularizationParameter = regularization
        } :
          ((PronySeriesRelaxation)o) with
          {
            MinimalRelaxationTime = minimalValue,
            MaximalRelaxationTime = maximalValue,
            NumberOfRelaxationTimes = numberRelax,
            UseIntercept = intercept,
            RegularizationParameter = regularization
          };
      }
    }
    #endregion



    /// <summary>
    /// Evaluates a prony series fit in the time domain, using the properties <see cref="MinimalRelaxationTime"/>, <see cref="MaximalRelaxationTime"/>, <see cref="NumberOfRelaxationTimes"/> and <see cref="RegularizationParameter"/>.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="yarr">The y-values of the signal.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRelaxationResult"/>.</returns>
    public PronySeriesRelaxationResult EvaluateTimeDomain(IReadOnlyList<double> xarr, IReadOnlyList<double> yarr)
    {
      return EvaluateTimeDomain(xarr, yarr, MinimalRelaxationTime, MaximalRelaxationTime, NumberOfRelaxationTimes, UseIntercept, RegularizationParameter);
    }

    /// <summary>
    /// Evaluates a prony series fit in the time domain, using the properties <see cref="MinimalRelaxationTime"/>, <see cref="MaximalRelaxationTime"/>, <see cref="NumberOfRelaxationTimes"/> and <see cref="RegularizationParameter"/>.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="isCircularFrequency">True if xarr contains circular frequencies; false if xarr contains normal frequencies.</param>
    /// <param name="yarrRe">The real part of the modulus.</param>
    /// <param name="yarrIm">The imaginary part of the modulus.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRelaxationResult"/>.</returns>
    public PronySeriesRelaxationResult EvaluateFrequencyDomain(IReadOnlyList<double> xarr, bool isCircularFrequency, IReadOnlyList<double>? yarrRe, IReadOnlyList<double>? yarrIm)
    {
      return EvaluateFrequencyDomain(xarr, isCircularFrequency, yarrRe, yarrIm, MinimalRelaxationTime, MaximalRelaxationTime, NumberOfRelaxationTimes, UseIntercept, RegularizationParameter);
    }

    /// <summary>
    /// Evaluates a prony series fit in the time domain, using the properties <see cref="MinimalRelaxationTime"/>, <see cref="MaximalRelaxationTime"/>, <see cref="NumberOfRelaxationTimes"/> and <see cref="RegularizationParameter"/>.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="isCircularFrequency">True if xarr contains circular frequencies; false if xarr contains normal frequencies.</param>
    /// <param name="yarr">The complex modulus.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRelaxationResult"/>.</returns>
    public PronySeriesRelaxationResult EvaluateFrequencyDomain(IReadOnlyList<double> xarr, bool isCircularFrequency, IReadOnlyList<Complex64> yarr)
    {
      return EvaluateFrequencyDomain(xarr, isCircularFrequency, yarr.Select(c => c.Real).ToArray(), yarr.Select(c => c.Imaginary).ToArray(), MinimalRelaxationTime, MaximalRelaxationTime, NumberOfRelaxationTimes, UseIntercept, RegularizationParameter);
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
    /// <param name="allowNegativeCoefficients">If true, negative Prony coefficients are allowed. This should be used only in very special cases.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRelaxationResult"/>.</returns>
    public static PronySeriesRelaxationResult EvaluateTimeDomain(IReadOnlyList<double> xarr, IReadOnlyList<double> yarr, double tmin, double tmax, int numberOfRelaxationTimes, bool withIntercept, double regularizationLambda, bool allowNegativeCoefficients = false)
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
      var numberOfTausPerExpcade = (numberOfRelaxationTimes - 1) / (Math.Log(tmax) - Math.Log(tmin));

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

      // Regularization by minimizing the sum of squares of the 2nd derivative of the parameters
      // we scale the parameter with the square root of the measured points
      // and with the number of relaxation times per decade to the power of 5/2
      regularizationLambda /= 100;
      regularizationLambda *= Math.Sqrt(NR);
      regularizationLambda *= numberOfTausPerExpcade * (numberOfTausPerExpcade * Math.Sqrt(numberOfTausPerExpcade));
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

      return Evaluate(tmin, tmax, numberOfRelaxationTimes, withIntercept, allowNegativeCoefficients: allowNegativeCoefficients, taus, X, y);
    }

    /// <summary>
    /// Evaluates a prony series fit in the frequency domain from the real and imaginary part of a general complex dynamic modulus.
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
      var numberOfTausPerExpcade = (numberOfRelaxationTimes - 1) / (Math.Log(tmax) - Math.Log(tmin));


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

      // Regularization by minimizing the sum of squares of the 2nd derivative of the parameters
      // we scale the parameter with the square root of the measured points
      // and with the number of relaxation times per decade to the power of 5/2
      regularizationLambda /= 100;
      regularizationLambda *= Math.Sqrt(NR) * Math.Sqrt(2);
      regularizationLambda *= numberOfTausPerExpcade * (numberOfTausPerExpcade * Math.Sqrt(numberOfTausPerExpcade));
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
          y[r, 0] = yarrIm[r - xCount];
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

      return Evaluate(tmin, tmax, numberOfRelaxationTimes, withIntercept, allowNegativeCoefficients: false, taus, X, y);
    }

    /// <summary>
    /// Evaluates a prony series fit in the frequency domain from the absolute values (magnitude) of of a general complex dynamic modulus.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="isCircularFrequency">True if xarr contains circular frequencies; false if xarr contains normal frequencies.</param>
    /// <param name="yMagnitude">The magnitude value of the complex dynamic modulus.</param>
    /// <param name="tmin">The smallest relaxation time (tau of the first Prony term).</param>
    /// <param name="tmax">The largest relaxation time (tau of the last Prony term).</param>
    /// <param name="numberOfRelaxationTimes">The number of relaxation times (number of Prony terms).</param>
    /// <param name="withIntercept">If set to <c>true</c>, an offset term is added. This term can be considered to have a infinite relaxation time.</param>
    /// <param name="regularizationLambda">A regularization parameter to smooth the resulting array of Prony terms.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRelaxationResult"/>.</returns>
    public static PronySeriesRelaxationResult EvaluateFrequencyDomainFromMagnitude(IReadOnlyList<double> xarr, bool isCircularFrequency, IReadOnlyList<double>? yMagnitude, double tmin, double tmax, int numberOfRelaxationTimes, bool withIntercept, double regularizationLambda)
    {
      if (xarr is null)
        throw new ArgumentNullException(nameof(xarr));
      if (yMagnitude is null)
        throw new ArgumentNullException(nameof(yMagnitude));
      if (xarr.Count != yMagnitude.Count)
        throw new ArgumentOutOfRangeException(nameof(yMagnitude), $"{nameof(yMagnitude)} should have the same length than xarr");
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
      var numberOfTausPerExpcade = (numberOfRelaxationTimes - 1) / (Math.Log(tmax) - Math.Log(tmin));


      int xCount = xarr.Count;
      int NR = xarr.Count;
      int NC = numberOfRelaxationTimes + (withIntercept ? 1 : 0); // one more column for the intercept (intercept is only in the real part)

      // Basis functions		
      var X = Matrix<double>.Build.Dense(NR + numberOfRelaxationTimes, NC);
      for (int c = 0; c < numberOfRelaxationTimes; ++c)
      {
        for (int r = 0; r < NR; ++r)
        {
          var tauomega = taus[c] * xarr[r % xCount] * (isCircularFrequency ? 1 : 2 * Math.PI);
          var g = tauomega / (tauomega - Complex64.ImaginaryOne);
          X[r, c] = g.Magnitude;
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

      // Regularization by minimizing the sum of squares of the 2nd derivative of the parameters
      // we scale the parameter with the square root of the measured points
      // and with the number of relaxation times per decade to the power of 5/2
      regularizationLambda /= 100;
      regularizationLambda *= Math.Sqrt(NR) * Math.Sqrt(2);
      regularizationLambda *= numberOfTausPerExpcade * (numberOfTausPerExpcade * Math.Sqrt(numberOfTausPerExpcade));
      for (int r = NR; r < NR + numberOfRelaxationTimes - 2; ++r)
      {
        X[r, r - NR] = regularizationLambda;
        X[r, r - NR + 1] = -2 * regularizationLambda;
        X[r, r - NR + 2] = regularizationLambda;
      }

      // read dependent variable to matrix y
      var y = Matrix<double>.Build.Dense(NR + numberOfRelaxationTimes, 1);

      for (int r = 0; r < NR; ++r)
        y[r, 0] = yMagnitude[r];

      return Evaluate(tmin, tmax, numberOfRelaxationTimes, withIntercept, allowNegativeCoefficients: false, taus, X, y);
    }



    protected static PronySeriesRelaxationResult Evaluate(double tmin, double tmax, int numberOfRelaxationTimes, bool withIntercept, bool allowNegativeCoefficients, double[] taus, Matrix<double> X, Matrix<double> y)
    {
      // calculate XtX and XtY
      var XtX = X.TransposeThisAndMultiply(X);
      var Xty = X.TransposeThisAndMultiply(y);

      IMatrix<double> x;
      if (allowNegativeCoefficients)
      {
        // Unusual case: if negative coefficients are allowed,
        // solve the equation, allow both positive and negative coefficients
        x = XtX.Solve(Xty);
      }
      else
      {
        // Usual case: solve the equation, but get nonnegative coefficients only
        FastNonnegativeLeastSquares.Execution(XtX, Xty, null, out x, out var _);
      }

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
