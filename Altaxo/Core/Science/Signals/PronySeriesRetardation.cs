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
  /// Performs a fit with a Prony series to a retardation signal (general susceptibility),
  /// either in the time domain or in the frequency domain.
  /// If the signal is a time-domain signal, it is assumed to be a retardation signal, i.e. it is increasing with time (for instance, strain at constant stress).
  /// If the signal is in the frequency domain, it is assumed to be a susceptibility, i.e. the real part is decreasing with frequency.
  /// </summary>
  public record PronySeriesRetardation : Main.IImmutable
  {
    private double _timeMinimum = 1;

    /// <summary>
    /// Gets or sets smallest retardation time (the tau_retard of the first Prony term).
    /// </summary>
    public double MinimalRetardationTime
    {
      get => _timeMinimum;
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException(nameof(MinimalRetardationTime), "Must be > 0");

        _timeMinimum = value;
      }
    }

    private double _timeMaximum = 1;

    /// <summary>
    /// Gets or sets largest retardation time (the tau_retard of the last Prony term).
    /// </summary>
    public double MaximalRetardationTime
    {
      get => _timeMaximum;
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException(nameof(MaximalRetardationTime), "Must be > 0");
        _timeMaximum = value;
      }
    }


    private int _numberOfRetardationTimes = 1;

    /// <summary>
    /// Gets the number of retardation times, i.e. the number of Prony terms.
    /// </summary>
    public int NumberOfRetardationTimes
    {
      get => _numberOfRetardationTimes;
      init
      {
        if (!(value >= 1))
          throw new ArgumentOutOfRangeException(nameof(NumberOfRetardationTimes), "Must be >= 1");

        _numberOfRetardationTimes = value;
      }
    }



    private double _regularizationParameter;

    /// <summary>
    /// Gets/sets the regularization parameter. Usually zero. The higher the value, the more the Prony coefficients are smoothed out.
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
    /// If true, an intercept (high-frequency retardation value) is also fitted. In the result, this is included as the Prony coefficient for retardation time zero.
    /// </summary>
    public bool UseIntercept { get; init; } = true;

    /// <summary>
    /// If true, a flow term (fluidity, conductivity) is also fitted. Then, in the result, the values for <see cref="PronySeriesRetardationResult.Fluidity"/>
    /// and <see cref="PronySeriesRetardationResult.Viscosity"/> are set.
    /// </summary>
    public bool UseFlowTerm { get; init; } = false;

    /// <summary>
    /// If true, the flow term is multiplied by the dielectric vacuum permittivity in order to obtain the electrical conductivity.
    /// </summary>
    public bool IsDielectricSpectrum { get; init; } = false;

    #region Serialization

    /// <summary>
    /// Serialization surrogate (version 0).
    /// </summary>
    /// <remarks>
    /// 2023-05-24: Initial version.
    /// </remarks>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PronySeriesRetardation), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PronySeriesRetardation)obj;
        info.AddValue("MinimalRelaxationTime", s.MinimalRetardationTime);
        info.AddValue("MaximalRelaxationTime", s.MaximalRetardationTime);
        info.AddValue("NumberOfRelaxationTimes", s.NumberOfRetardationTimes);
        info.AddValue("UseIntercept", s.UseIntercept);
        info.AddValue("UseFlowTerm", s.UseFlowTerm);
        info.AddValue("IsDielectricFlowTerm", s.IsDielectricSpectrum);
        info.AddValue("RegularizationParameter", s.RegularizationParameter);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var minimalValue = info.GetDouble("MinimalRelaxationTime");
        var maximalValue = info.GetDouble("MMaximalRelaxationTime");
        var numberRelax = info.GetInt32("NumberOfRelaxationTimes");
        var intercept = info.GetBoolean("UseIntercept");
        var useFlowTerm = info.GetBoolean("UseFlowTerm");
        var isDielectricFlowTerm = info.GetBoolean("IsDielectricFlowTerm");
        var regularization = info.GetDouble("RegularizationParameter");

        return o is null ? new PronySeriesRetardation
        {
          MinimalRetardationTime = minimalValue,
          MaximalRetardationTime = maximalValue,
          NumberOfRetardationTimes = numberRelax,
          UseIntercept = intercept,
          UseFlowTerm = useFlowTerm,
          IsDielectricSpectrum = isDielectricFlowTerm,
          RegularizationParameter = regularization
        } :
          ((PronySeriesRetardation)o) with
          {
            MinimalRetardationTime = minimalValue,
            MaximalRetardationTime = maximalValue,
            NumberOfRetardationTimes = numberRelax,
            UseIntercept = intercept,
            UseFlowTerm = useFlowTerm,
            IsDielectricSpectrum = isDielectricFlowTerm,
            RegularizationParameter = regularization
          };
      }
    }

    #endregion

    /// <summary>
    /// Evaluates a Prony series fit in the time domain, using the properties <see cref="MinimalRetardationTime"/>, <see cref="MaximalRetardationTime"/>,
    /// <see cref="NumberOfRetardationTimes"/>, <see cref="UseIntercept"/>, <see cref="UseFlowTerm"/>, and <see cref="RegularizationParameter"/>.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="yarr">The y-values of the signal.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRetardationResult"/>.</returns>
    public PronySeriesRetardationResult EvaluateTimeDomain(IReadOnlyList<double> xarr, IReadOnlyList<double> yarr)
    {
      return EvaluateTimeDomain(xarr, yarr, MinimalRetardationTime, MaximalRetardationTime, NumberOfRetardationTimes, UseIntercept, UseFlowTerm, RegularizationParameter);
    }

    /// <summary>
    /// Evaluates a Prony series fit in the time domain, using the properties <see cref="MinimalRetardationTime"/>, <see cref="MaximalRetardationTime"/>,
    /// <see cref="NumberOfRetardationTimes"/>, <see cref="UseIntercept"/>, <see cref="UseFlowTerm"/>, and <see cref="RegularizationParameter"/>.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="isCircularFrequency">True if <paramref name="xarr"/> contains circular frequencies; false if it contains normal frequencies.</param>
    /// <param name="yarrRe">The real part of the susceptibility, or <see langword="null"/> if not available.</param>
    /// <param name="yarrIm">
    /// The imaginary part of the susceptibility, or <see langword="null"/> if not available.
    /// Note that although for a general susceptibility the imaginary part is negative,
    /// in science it is usual to change the sign, e.g. <c>J*(w) = J'(w) - i J''(w)</c>. Thus, here it is expected that the imaginary part is positive.
    /// </param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRetardationResult"/>.</returns>
    public PronySeriesRetardationResult EvaluateFrequencyDomain(IReadOnlyList<double> xarr, bool isCircularFrequency, IReadOnlyList<double>? yarrRe, IReadOnlyList<double>? yarrIm)
    {
      return EvaluateFrequencyDomain(xarr, isCircularFrequency, yarrRe, yarrIm, MinimalRetardationTime, MaximalRetardationTime, NumberOfRetardationTimes, UseIntercept, UseFlowTerm, IsDielectricSpectrum, RegularizationParameter);
    }

    /// <summary>
    /// Evaluates a Prony series fit in the frequency domain from a complex susceptibility, using the properties <see cref="MinimalRetardationTime"/>,
    /// <see cref="MaximalRetardationTime"/>, <see cref="NumberOfRetardationTimes"/>, <see cref="UseIntercept"/>, <see cref="UseFlowTerm"/>,
    /// <see cref="IsDielectricSpectrum"/>, and <see cref="RegularizationParameter"/>.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="isCircularFrequency">True if <paramref name="xarr"/> contains circular frequencies; false if it contains normal frequencies.</param>
    /// <param name="yarr">
    /// The complex susceptibility.
    /// Note that although for a general susceptibility the imaginary part is negative,
    /// in science it is usual to change the sign, e.g. <c>J*(w) = J'(w) - i J''(w)</c>. Thus, here it is expected that the imaginary part is positive.
    /// </param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRetardationResult"/>.</returns>
    public PronySeriesRetardationResult EvaluateFrequencyDomain(IReadOnlyList<double> xarr, bool isCircularFrequency, IReadOnlyList<Complex64> yarr)
    {
      return EvaluateFrequencyDomain(xarr, isCircularFrequency, yarr.Select(c => c.Real).ToArray(), yarr.Select(c => c.Imaginary).ToArray(), MinimalRetardationTime, MaximalRetardationTime, NumberOfRetardationTimes, UseIntercept, UseFlowTerm, IsDielectricSpectrum, RegularizationParameter);
    }

    /// <summary>
    /// Evaluates a Prony series fit in the time domain.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="yarr">The y-values of the signal.</param>
    /// <param name="tmin">The smallest retardation time (tau of the first Prony term).</param>
    /// <param name="tmax">The largest retardation time (tau of the last Prony term).</param>
    /// <param name="numberOfRetardationTimes">The number of retardation times (number of Prony terms).</param>
    /// <param name="withIntercept">If set to <c>true</c>, an offset term is added. This term can be considered to have a retardation time of zero.</param>
    /// <param name="withFlowTerm">If set to <c>true</c>, a flow term (fluidity, conductivity) is also fitted.</param>
    /// <param name="regularizationLambda">A regularization parameter to smooth the resulting array of Prony terms.</param>
    /// <param name="allowNegativeCoefficients">Normally, in a Prony series only nonnegative coefficients are allowed. Set this parameter to true to allow also negative coefficients.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRetardationResult"/>.</returns>
    public static PronySeriesRetardationResult EvaluateTimeDomain(IReadOnlyList<double> xarr, IReadOnlyList<double> yarr, double tmin, double tmax, int numberOfRetardationTimes, bool withIntercept, bool withFlowTerm, double regularizationLambda, bool allowNegativeCoefficients = false)
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
      if (!(numberOfRetardationTimes >= 1))
        throw new ArgumentOutOfRangeException(nameof(numberOfRetardationTimes), "Must be >= 1");
      if (!(tmax == tmin || numberOfRetardationTimes >= 2))
        throw new ArgumentOutOfRangeException(nameof(numberOfRetardationTimes), "Must be >= 2");

      // evaluate the Retardation times (logarithmically spaced)
      double[] taus = new double[numberOfRetardationTimes];
      for (int c = 0; c < numberOfRetardationTimes; ++c)
      {
        double r = c == 0 ? 0 : c / (double)(numberOfRetardationTimes - 1);
        double lntau = (1 - r) * Math.Log(tmin) + r * Math.Log(tmax);
        taus[c] = Math.Exp(lntau);
      }
      var numberOfTausPerExpcade = (numberOfRetardationTimes - 1) / (Math.Log(tmax) - Math.Log(tmin));

      int NR = xarr.Count;
      int NC = numberOfRetardationTimes + (withIntercept ? 1 : 0) + (withFlowTerm ? 1 : 0); // one more column for the intercept

      // Basis functions		
      var X = Matrix<double>.Build.Dense(NR + numberOfRetardationTimes, NC);
      for (int c = 0; c < numberOfRetardationTimes; ++c)
      {
        for (int r = 0; r < NR; ++r)
        {
          X[r, c] = 1 - Math.Exp(-xarr[r] / taus[c]);
        }
      }

      // Intercept
      if (withIntercept)
      {
        int idx = NC - (withFlowTerm ? 2 : 1);
        for (int r = 0; r < NR; ++r)
        {
          X[r, idx] = 1; // base function for intercept is 1
        }
      }

      double flowTermScale = 1 / tmax;
      if (withFlowTerm)
      {
        int idx = NC - 1;
        for (int r = 0; r < NR; ++r)
        {
          X[r, idx] = xarr[r] * flowTermScale; // base function for flow term is x, scale it so that it is in the range [0,1]
        }
      }

      // Regularization by minimizing the sum of squares of the 2nd derivative of the parameters
      // we scale the parameter with the square root of the measured points
      // and with the number of retardation times per decade to the power of 5/2
      regularizationLambda /= 100;
      regularizationLambda *= Math.Sqrt(NR);
      regularizationLambda *= numberOfTausPerExpcade * (numberOfTausPerExpcade * Math.Sqrt(numberOfTausPerExpcade));
      for (int r = NR; r < NR + numberOfRetardationTimes - 2; ++r)
      {
        X[r, r - NR] = regularizationLambda;
        X[r, r - NR + 1] = -2 * regularizationLambda;
        X[r, r - NR + 2] = regularizationLambda;
      }

      // read dependent variable to matrix y
      var y = Matrix<double>.Build.Dense(NR + numberOfRetardationTimes, 1);
      for (int r = 0; r < NR; ++r)
        y[r, 0] = yarr[r];

      return Evaluate(tmin, tmax, numberOfRetardationTimes, withIntercept, withFlowTerm, isRelativePermittivitySpectrum: false, flowTermScale, allowNegativeCoefficients: allowNegativeCoefficients, taus, X, y);
    }

    /// <summary>
    /// Evaluates a Prony series fit in the frequency domain from the real and imaginary part of a general complex dynamic susceptibility.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="isCircularFrequency">True if xarr contains circular frequencies; false if xarr contains normal frequencies.</param>
    /// <param name="yarrRe">The real part of the susceptibility.</param>
    /// <param name="yarrIm">The imaginary part of the susceptibility. Note that although for a general susceptiblity the imaginary part is negative,
    /// in science it is usual to change the sign, e.g. J*(w) = J'(w) - i J''(w). Thus, here it is expected that the imaginary part is positive.</param>
    /// <param name="tmin">The smallest retardation time (tau of the first Prony term).</param>
    /// <param name="tmax">The largest retardation time (tau of the last Prony term).</param>
    /// <param name="numberOfRetardationTimes">The number of retardation times (number of Prony terms).</param>
    /// <param name="withIntercept">If set to <c>true</c>, an offset term is added. This term can be considered to have a infinite Retardation time.</param>
    /// <param name="withFlowTerm">If set to true, the flow term is calculated, too.</param>
    /// <param name="isRelativePermittivitySpectrum">If set to true, it is indicated that the spectrum to be fitted is a dielectric spectrum of relative permittivities.</param>
    /// <param name="regularizationLambda">A regularization parameter to smooth the resulting array of Prony terms.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRetardationResult"/>.</returns>
    public static PronySeriesRetardationResult EvaluateFrequencyDomain(IReadOnlyList<double> xarr, bool isCircularFrequency, IReadOnlyList<double>? yarrRe, IReadOnlyList<double>? yarrIm, double tmin, double tmax, int numberOfRetardationTimes, bool withIntercept, bool withFlowTerm, bool isRelativePermittivitySpectrum, double regularizationLambda)
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
      if (!(numberOfRetardationTimes >= 1))
        throw new ArgumentOutOfRangeException(nameof(numberOfRetardationTimes), "Must be >= 1");
      if (!(tmax == tmin || numberOfRetardationTimes >= 2))
        throw new ArgumentOutOfRangeException(nameof(numberOfRetardationTimes), "Must be >= 2");

      withIntercept &= yarrRe is not null; // intercept only if real part is given
      withFlowTerm &= yarrIm is not null; // flow term only if imaginary part is given

      // evaluate the Retardation times (logarithmically spaced)
      double[] taus = new double[numberOfRetardationTimes];
      for (int c = 0; c < numberOfRetardationTimes; ++c)
      {
        double r = c == 0 ? 0 : c / (double)(numberOfRetardationTimes - 1);
        double lntau = (1 - r) * Math.Log(tmin) + r * Math.Log(tmax);
        taus[c] = Math.Exp(lntau);
      }
      var numberOfTausPerExpcade = (numberOfRetardationTimes - 1) / (Math.Log(tmax) - Math.Log(tmin));


      int xCount = xarr.Count;
      int NR = bothReAndIm ? 2 * xarr.Count : xarr.Count;
      int NC = numberOfRetardationTimes + (withIntercept ? 1 : 0) + (withFlowTerm ? 1 : 0); // one more column for the intercept (intercept is only in the real part), and one more for the flow term

      // Basis functions		
      var X = Matrix<double>.Build.Dense(NR + numberOfRetardationTimes, NC);
      for (int c = 0; c < numberOfRetardationTimes; ++c)
      {
        for (int r = 0; r < NR; ++r)
        {
          var tauomega = taus[c] * xarr[r % xCount] * (isCircularFrequency ? 1 : 2 * Math.PI);
          var g = 1 / (1 + tauomega * Complex64.ImaginaryOne);
          if (bothReAndIm)
          {
            X[r, c] = r < xCount ? g.Real : -g.Imaginary;
          }
          else
          {
            X[r, c] = yarrRe is not null ? g.Real : -g.Imaginary;
          }
        }
      }

      // Intercept
      if (withIntercept)
      {
        int idx = NC - (withFlowTerm ? 2 : 1);
        for (int r = 0; r < xCount; ++r) // set intercept only for real part -> only for the first half
        {
          X[r, idx] = 1; // base function for the intercept
        }
      }
      double flowTermScale = 1 / tmax;
      if (withFlowTerm)
      {
        int offs = yarrRe is null ? 0 : xarr.Count;
        double fac = isCircularFrequency ? 1 : 2 * Math.PI;
        for (int r = 0; r < xCount; ++r) // set flow term only for the imaginary part -> either on the first half or the second half
        {
          X[r + offs, NC - 1] = flowTermScale / (xarr[r] * fac); // base function for the intercept
        }
      }

      // Regularization by minimizing the sum of squares of the 2nd derivative of the parameters
      // we scale the parameter with the square root of the measured points
      // and with the number of retardation times per decade to the power of 5/2
      regularizationLambda /= 100;
      regularizationLambda *= Math.Sqrt(NR) * Math.Sqrt(2);
      regularizationLambda *= numberOfTausPerExpcade * (numberOfTausPerExpcade * Math.Sqrt(numberOfTausPerExpcade));
      for (int r = NR; r < NR + numberOfRetardationTimes - 2; ++r)
      {
        X[r, r - NR] = regularizationLambda;
        X[r, r - NR + 1] = -2 * regularizationLambda;
        X[r, r - NR + 2] = regularizationLambda;
      }

      // read dependent variable to matrix y
      var y = Matrix<double>.Build.Dense(NR + numberOfRetardationTimes, 1);

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

      return Evaluate(tmin, tmax, numberOfRetardationTimes, withIntercept, withFlowTerm, isRelativePermittivitySpectrum, flowTermScale, allowNegativeCoefficients: false, taus, X, y);
    }

    /// <summary>
    /// Evaluates a Prony series fit in the frequency domain from the absolute value (magnitude) of a general complex dynamic susceptibility.
    /// </summary>
    /// <param name="xarr">The x-values of the signal (all elements must be positive).</param>
    /// <param name="isCircularFrequency">True if xarr contains circular frequencies; false if xarr contains normal frequencies.</param>
    /// <param name="yMagnitude">The absolute value (magnitude) of the general complex dynamic susceptibility.</param>
    /// <param name="tmin">The smallest retardation time (tau of the first Prony term).</param>
    /// <param name="tmax">The largest retardation time (tau of the last Prony term).</param>
    /// <param name="numberOfRetardationTimes">The number of retardation times (number of Prony terms).</param>
    /// <param name="withIntercept">If set to <c>true</c>, an offset term is added. This term can be considered to have a infinite Retardation time.</param>
    /// <param name="withFlowTerm">If set to true, the flow term is calculated, too.</param>
    /// <param name="isRelativePermittivitySpectrum">If set to true, it is indicated that the spectrum to be fitted is a dielectric spectrum of relative permittivities.</param>
    /// <param name="regularizationLambda">A regularization parameter to smooth the resulting array of Prony terms.</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRetardationResult"/>.</returns>
    public static PronySeriesRetardationResult EvaluateFrequencyDomainFromMagnitude(IReadOnlyList<double> xarr, bool isCircularFrequency, IReadOnlyList<double>? yMagnitude, double tmin, double tmax, int numberOfRetardationTimes, bool withIntercept, bool withFlowTerm, bool isRelativePermittivitySpectrum, double regularizationLambda)
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
      if (!(numberOfRetardationTimes >= 1))
        throw new ArgumentOutOfRangeException(nameof(numberOfRetardationTimes), "Must be >= 1");
      if (!(tmax == tmin || numberOfRetardationTimes >= 2))
        throw new ArgumentOutOfRangeException(nameof(numberOfRetardationTimes), "Must be >= 2");

      // evaluate the Retardation times (logarithmically spaced)
      double[] taus = new double[numberOfRetardationTimes];
      for (int c = 0; c < numberOfRetardationTimes; ++c)
      {
        double r = c == 0 ? 0 : c / (double)(numberOfRetardationTimes - 1);
        double lntau = (1 - r) * Math.Log(tmin) + r * Math.Log(tmax);
        taus[c] = Math.Exp(lntau);
      }
      var numberOfTausPerExpcade = (numberOfRetardationTimes - 1) / (Math.Log(tmax) - Math.Log(tmin));

      int xCount = xarr.Count;
      int NR = xarr.Count;
      int NC = numberOfRetardationTimes + (withIntercept ? 1 : 0) + (withFlowTerm ? 1 : 0); // one more column for the intercept (intercept is only in the real part), and one more for the flow term

      // Basis functions		
      var X = Matrix<double>.Build.Dense(NR + numberOfRetardationTimes, NC);
      for (int c = 0; c < numberOfRetardationTimes; ++c)
      {
        for (int r = 0; r < NR; ++r)
        {
          var tauomega = taus[c] * xarr[r % xCount] * (isCircularFrequency ? 1 : 2 * Math.PI);
          var g = 1 / (1 + tauomega * Complex64.ImaginaryOne);
          X[r, c] = g.Magnitude;
        }
      }

      // Intercept
      if (withIntercept)
      {
        int idx = NC - (withFlowTerm ? 2 : 1);
        for (int r = 0; r < xCount; ++r) // set intercept only for real part -> only for the first half
        {
          X[r, idx] = 1; // base function for the intercept
        }
      }
      double flowTermScale = 1 / tmax;
      if (withFlowTerm)
      {
        int offs = xarr.Count;
        double fac = isCircularFrequency ? 1 : 2 * Math.PI;
        for (int r = 0; r < xCount; ++r) // set flow term only for the imaginary part -> either on the first half or the second half
        {
          X[r + offs, NC - 1] = flowTermScale / (xarr[r] * fac); // base function for the intercept
        }
      }

      // Regularization by minimizing the sum of squares of the 2nd derivative of the parameters
      // we scale the parameter with the square root of the measured points
      // and with the number of retardation times per decade to the power of 5/2
      regularizationLambda /= 100;
      regularizationLambda *= Math.Sqrt(NR) * Math.Sqrt(2);
      regularizationLambda *= numberOfTausPerExpcade * (numberOfTausPerExpcade * Math.Sqrt(numberOfTausPerExpcade));
      for (int r = NR; r < NR + numberOfRetardationTimes - 2; ++r)
      {
        X[r, r - NR] = regularizationLambda;
        X[r, r - NR + 1] = -2 * regularizationLambda;
        X[r, r - NR + 2] = regularizationLambda;
      }

      // read dependent variable to matrix y
      var y = Matrix<double>.Build.Dense(NR + numberOfRetardationTimes, 1);

      for (int r = 0; r < NR; ++r)
        y[r, 0] = yMagnitude[r];

      return Evaluate(tmin, tmax, numberOfRetardationTimes, withIntercept, withFlowTerm, isRelativePermittivitySpectrum, flowTermScale, allowNegativeCoefficients: false, taus, X, y);
    }


    /// <summary>
    /// Evaluates the Prony series coefficients using the matrix formulation of a (regularized) least-squares problem.
    /// </summary>
    /// <param name="tmin">The smallest retardation time (tau of the first Prony term).</param>
    /// <param name="tmax">The largest retardation time (tau of the last Prony term).</param>
    /// <param name="numberOfRetardationTimes">The number of retardation times (number of Prony terms).</param>
    /// <param name="withIntercept">If set to <c>true</c>, includes an offset term (interpreted as retardation time 0).</param>
    /// <param name="withFlowTerm">If set to <c>true</c>, includes a flow term (fluidity, conductivity).</param>
    /// <param name="isRelativePermittivitySpectrum">If set to <c>true</c>, indicates that the fitted spectrum is a dielectric spectrum of relative permittivities.</param>
    /// <param name="flowTermScale">Scale factor used for the flow term basis function.</param>
    /// <param name="allowNegativeCoefficients">If <c>true</c>, allows negative Prony coefficients.</param>
    /// <param name="taus">The retardation times (tau values) used for the Prony terms.</param>
    /// <param name="X">The design matrix (including optional regularization rows).</param>
    /// <param name="y">The target vector (including optional regularization entries).</param>
    /// <returns>The result of the evaluation, see <see cref="PronySeriesRetardationResult"/>.</returns>
    protected static PronySeriesRetardationResult Evaluate(double tmin, double tmax, int numberOfRetardationTimes, bool withIntercept, bool withFlowTerm, bool isRelativePermittivitySpectrum, double flowTermScale, bool allowNegativeCoefficients, double[] taus, Matrix<double> X, Matrix<double> y)
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
        // Usual case : solve the equation, but get nonnegative coefficients only
        FastNonnegativeLeastSquares.Execution(XtX, Xty, null, out x, out var _);
      }


      // the result (the spectral amplitudes) are now in X
      double spectralDensityFactor = Math.Log(tmax / tmin) / (numberOfRetardationTimes - 1);
      var resultTauCol = withIntercept ? new double[] { 0.0 }.Concat(taus).ToArray() : taus.ToArray();
      var resultPronyCol = new double[resultTauCol.Length];
      var resultRetardationDensityCol = new double[resultTauCol.Length];

      int offs = withIntercept ? 1 : 0;
      for (int i = 0; i < taus.Length; ++i)
      {
        resultPronyCol[i + offs] = x[i, 0];
        resultRetardationDensityCol[i + offs] = x[i, 0] / spectralDensityFactor;
      }

      if (withIntercept)
      {
        resultPronyCol[0] = x[taus.Length, 0];
        resultRetardationDensityCol[0] = x[taus.Length, 0] / spectralDensityFactor;
      }

      double? flowTerm = 0;
      if (withFlowTerm)
      {
        flowTerm = x[x.RowCount - 1, 0] * flowTermScale;
      }


      var result = new PronySeriesRetardationResult(resultTauCol, resultPronyCol, resultRetardationDensityCol, flowTerm, isRelativePermittivitySpectrum);
      return result;
    }
  }
}
