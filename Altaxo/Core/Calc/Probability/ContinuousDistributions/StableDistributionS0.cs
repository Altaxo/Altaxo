#nullable disable
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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Probability
{
  using Altaxo.Calc.RootFinding;

  /// <summary>
  /// Represents a stable distribution in Nolan's S0 parametrization.
  /// </summary>
  /// <remarks>
  /// The characteristic function in Nolan's S0 parametrization is:
  /// <code>
  /// log(phi(t))= -scale^alpha |t|^alpha (1+i beta Sign(t) Tan(pi alpha/2) (|scale t|^(1-alpha)-1)) + i location t              (for alpha not equal to 1)
  /// </code>
  /// and
  /// <code>
  /// log(phi(t)) = -scale |t| (1+i beta Sign(t) (2/pi) Log(scale |t|)) + i location t  (for alpha equal to 1)
  /// </code>
  /// <para>
  /// Reference: J. P. Nolan, Numerical calculation of stable densities and distribution functions.
  /// Communications in Statistics - Stochastic Models, 13, 759-774, 1999
  /// </para>
  /// <para>
  /// Reference: S. Borak, W. H0rdle, R. Weron, Stable distributions. SFB 649 Discussion paper 2005-2008,
  /// http://sfb649.wiwi.hu-berlin.de, ISSN 1860-5664
  /// </para>
  /// <para/>
  /// <para>
  /// If you are interested in accurate calculations when beta is close to 1 or -1, you should use those functions which
  /// allow you to provide the parameter <c>abe</c>. This helps specify beta with higher accuracy close to +1 or -1.
  /// For instance, by using abe = 1E-30 and beta = 1, it is possible to specify beta = 1 - 1E-30, which is otherwise
  /// impossible with the 64-bit floating-point representation.
  /// </para>
  /// </remarks>
  public class StableDistributionS0 : StableDistributionBase
  {
    private double _alpha;
    private double _beta;
    private double _abe;

    private double _mu;
    private double _scale = 1;

    private double _muOffsetToS1;

    private object? _tempStorePDF;

    #region construction

    /// <summary>
    /// Creates a new instance of this distribution with default parameters (alpha=1, beta=0) and the default generator.
    /// </summary>
    public StableDistributionS0()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with default parameters (alpha=1, beta=0).
    /// </summary>
    /// <param name="generator">Random number generator to be used with this distribution.</param>
    public StableDistributionS0(Generator generator)
      : this(1, 0, 1, 1, 0, generator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta) and the default random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    public StableDistributionS0(double alpha, double beta)
      : this(alpha, beta, GetAbeFromBeta(beta), 1, 0, DefaultGenerator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta, abe) and the default random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is 1-beta for beta&gt;=0 or 1+beta for beta&lt;0.</param>
    public StableDistributionS0(double alpha, double beta, double abe)
      : this(alpha, beta, abe, 1, 0, DefaultGenerator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta, scale, location) and the default random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="scale">Scaling parameter (broadness of the distribution).</param>
    /// <param name="location">Location of the distribution.</param>
    public StableDistributionS0(double alpha, double beta, double scale, double location)
      : this(alpha, beta, GetAbeFromBeta(beta), scale, location, DefaultGenerator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta, scale, location) and the provided random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="scale">Scaling parameter (broadness of the distribution).</param>
    /// <param name="location">Location of the distribution.</param>
    /// <param name="generator">Random number generator to be used with this distribution.</param>
    public StableDistributionS0(double alpha, double beta, double scale, double location, Generator generator)
      : this(alpha, beta, GetAbeFromBeta(beta), scale, location, generator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta, abe, scale, location) and the default random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is 1-beta for beta&gt;=0 or 1+beta for beta&lt;0.</param>
    /// <param name="scale">Scaling parameter (broadness of the distribution).</param>
    /// <param name="location">Location of the distribution.</param>
    public StableDistributionS0(double alpha, double beta, double abe, double scale, double location)
      : this(alpha, beta, abe, scale, location, DefaultGenerator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta, abe, scale, location) and the provided random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is 1-beta for beta&gt;=0 or 1+beta for beta&lt;0.</param>
    /// <param name="scale">Scaling parameter (broadness of the distribution).</param>
    /// <param name="location">Location of the distribution.</param>
    /// <param name="generator">Random number generator to be used with this distribution.</param>
    public StableDistributionS0(double alpha, double beta, double abe, double scale, double location, Generator generator)
      : base(generator)
    {
      Initialize(alpha, beta, abe, scale, location);
    }

    #endregion construction

    #region Distribution members

    /// <summary>
    /// Initializes this distribution instance with the given parameters.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <param name="sigma">Scaling parameter (broadness of the distribution). Must be positive.</param>
    /// <param name="mu">Location parameter of the distribution.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any parameter is outside its valid range.</exception>
    public void Initialize(double alpha, double beta, double abe, double sigma, double mu)
    {
      if (!IsValidAlpha(alpha))
        throw new ArgumentOutOfRangeException("Alpha out of range (must be in the range (0,2])");
      if (!IsValidBeta(beta))
        throw new ArgumentOutOfRangeException("Beta out of range (must be in the range [-1,1])");
      if (!IsValidSigma(sigma))
        throw new ArgumentOutOfRangeException("Sigma out of range (must be > 0)");
      if (!IsValidMu(mu))
        throw new ArgumentOutOfRangeException("Mu out of range (must be finite)");

      _alpha = alpha;
      _beta = beta;
      _abe = abe;
      _scale = sigma;
      _mu = mu;
      ParameterConversionS0ToS1(_alpha, _beta, _scale, _mu, out _muOffsetToS1);

      // Generator variables
      if (_alpha != 1 && _alpha != 2)
      {
        double tanpialpha2 = TanXPiBy2(alpha);
        _gen_t = beta * tanpialpha2;
        _gen_B = Math.Atan(_gen_t) / alpha;
        _gen_S = PowerOfOnePlusXSquared(_gen_t, 0.5 / _alpha);
        _gen_Scale = 1;
      }
    }

    /// <summary>
    /// Checks whether <paramref name="alpha"/> is valid for a stable distribution.
    /// </summary>
    /// <param name="alpha">The alpha parameter to validate.</param>
    /// <returns><see langword="true"/> if 0 &lt; alpha &lt;= 2; otherwise, <see langword="false"/>.</returns>
    public static bool IsValidAlpha(double alpha)
    {
      return alpha > 0 && alpha <= 2;
    }

    /// <summary>
    /// Checks whether <paramref name="beta"/> is valid for a stable distribution.
    /// </summary>
    /// <param name="beta">The beta parameter to validate.</param>
    /// <returns><see langword="true"/> if -1 &lt;= beta &lt;= 1; otherwise, <see langword="false"/>.</returns>
    public static bool IsValidBeta(double beta)
    {
      return beta >= -1 && beta <= 1;
    }

    /// <summary>
    /// Checks whether <paramref name="sigma"/> is a valid scale parameter.
    /// </summary>
    /// <param name="sigma">The scale parameter to validate.</param>
    /// <returns><see langword="true"/> if <paramref name="sigma"/> is positive; otherwise, <see langword="false"/>.</returns>
    public static bool IsValidSigma(double sigma)
    {
      return sigma > 0;
    }

    /// <summary>
    /// Checks whether <paramref name="mu"/> is a valid location parameter.
    /// </summary>
    /// <param name="mu">The location parameter to validate.</param>
    /// <returns><see langword="true"/> if <paramref name="mu"/> is finite; otherwise, <see langword="false"/>.</returns>
    public static bool IsValidMu(double mu)
    {
      return mu >= double.MinValue && mu <= double.MaxValue;
    }

    /// <inheritdoc/>
    public override double Minimum
    {
      get
      {
        if (_alpha < 1 && _beta == 1)
          return _mu - _scale * TanXPiBy2(_alpha);
        else
          return double.NegativeInfinity;
      }
    }

    /// <inheritdoc/>
    public override double Maximum
    {
      get
      {
        if (_alpha < 1 && _beta == -1)
          return _mu + _scale * TanXPiBy2(_alpha);
        else
          return double.PositiveInfinity;
      }
    }

    /// <inheritdoc/>
    public override double Mean
    {
      get { return _alpha <= 1 ? double.NaN : _mu - _beta * _scale * Math.Tan(0.5 * Math.PI * _alpha); }
    }

    /// <inheritdoc/>
    public override double Median
    {
      get
      {
        if (0 == _beta)
          return _mu;
        else
          return double.NaN; // TODO : this is not analytical expressable, but is defined!
      }
    }

    /// <inheritdoc/>
    public override double Variance
    {
      get
      {
        if (_alpha == 2)
          return 2 * _scale * _scale;
        else
          return double.PositiveInfinity;
      }
    }

    /// <inheritdoc/>
    public override double[] Mode
    {
      get
      {
        if (0 == _beta)
          return new double[] { _mu };
        else
          return new double[] { double.NaN }; // TODO : this is not analytical expressable, but is defined!
      }
    }

    /// <inheritdoc/>
    public override double NextDouble()
    {
      if (_beta == 0)
        return GenerateSymmetricCase(_alpha) * _scale + _mu;
      else
      {
        if (_alpha == 1)
          return GenerateAsymmetricCaseS1_AEq1(_alpha, _beta) * _scale + _mu;
        else
          return GenerateAsymmetricCaseS1_ANe1(_alpha, _gen_t, _gen_B, _gen_S, _scale) + _muOffsetToS1;
      }
    }

    /// <inheritdoc/>
    public override double PDF(double x)
    {
      return PDF(x, _alpha, _beta, _abe, _scale, _mu, ref _tempStorePDF, DefaultPrecision);
    }

    /// <inheritdoc/>
    public override double CDF(double x)
    {
      return CDF(x, _alpha, _beta, _abe, _scale, _mu, ref _tempStorePDF, DefaultPrecision);
    }

    /// <inheritdoc/>
    public override double Quantile(double p)
    {
      return _mu + _scale * Quantile(p, _alpha, _beta, _abe);
    }

    #endregion Distribution members

    #region PDF

    /// <summary>
    /// Gets the probability density function (PDF) of the (standardized) S0 stable distribution.
    /// </summary>
    /// <param name="x">The value where the density is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <returns>The probability density at <paramref name="x"/>.</returns>
    public static double PDF(double x, double alpha, double beta)
    {
      object? tempStore = null;
      return PDF(x, alpha, beta, ref tempStore, Math.Sqrt(DoubleConstants.DBL_EPSILON));
    }

    /// <summary>
    /// Gets the probability density function (PDF) for the specified scale and location.
    /// </summary>
    /// <param name="x">The value where the density is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="sigma">Scale parameter.</param>
    /// <param name="mu">Location parameter.</param>
    /// <returns>The probability density at <paramref name="x"/>.</returns>
    public static double PDF(double x, double alpha, double beta, double sigma, double mu)
    {
      object? tempStore = null;
      return PDF(x, alpha, beta, sigma, mu, ref tempStore, Math.Sqrt(DoubleConstants.DBL_EPSILON));
    }

    /// <summary>
    /// Gets the probability density function (PDF) for the specified scale and location.
    /// </summary>
    /// <param name="x">The value where the density is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="sigma">Scale parameter.</param>
    /// <param name="mu">Location parameter.</param>
    /// <param name="tempStorage">Temporary storage that can be reused between calls.</param>
    /// <param name="precision">Goal for the relative precision.</param>
    /// <returns>The probability density at <paramref name="x"/>.</returns>
    public static double PDF(double x, double alpha, double beta, double sigma, double mu, ref object? tempStorage, double precision)
    {
      return PDF((x - mu) / sigma, alpha, beta, ref tempStorage, precision) / sigma;
    }

    /// <summary>
    /// Gets the probability density function (PDF) for the specified S0 parameters, scale, and location.
    /// </summary>
    /// <param name="x">The value where the density is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <param name="sigma">Scale parameter.</param>
    /// <param name="mu">Location parameter.</param>
    /// <param name="tempStorage">Temporary storage that can be reused between calls.</param>
    /// <param name="precision">Goal for the relative precision.</param>
    /// <returns>The probability density at <paramref name="x"/>.</returns>
    public static double PDF(double x, double alpha, double beta, double abe, double sigma, double mu, ref object? tempStorage, double precision)
    {
      return PDF((x - mu) / sigma, alpha, beta, abe, ref tempStorage, precision) / sigma;
    }

    /// <summary>
    /// Gets the probability density function (PDF) of the standardized S0 stable distribution.
    /// </summary>
    /// <param name="x">The value where the density is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="tempStorage">Temporary storage that can be reused between calls.</param>
    /// <param name="precision">Goal for the relative precision.</param>
    /// <returns>The probability density at <paramref name="x"/>.</returns>
    public static double PDF(double x, double alpha, double beta, ref object? tempStorage, double precision)
    {
      double abe = beta >= 0 ? 1 - beta : 1 + beta;
      return PDF(x, alpha, beta, abe, ref tempStorage, precision);
    }

    /// <summary>
    /// Gets the probability density function (PDF) of the standardized S0 stable distribution.
    /// </summary>
    /// <param name="x">The value where the density is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <param name="tempStorage">Temporary storage that can be reused between calls.</param>
    /// <param name="precision">Goal for the relative precision.</param>
    /// <returns>The probability density at <paramref name="x"/>.</returns>
    public static double PDF(double x, double alpha, double beta, double abe, ref object? tempStorage, double precision)
    {
      // Test for special case of symmetric destribution, this can be handled much better
      if (beta == 0)
        return StableDistributionSymmetric.PDF(x, alpha, ref tempStorage, precision);

      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));
      if (!(beta >= -1 && beta <= 1))
        throw new ArgumentOutOfRangeException(string.Format("Beta must be in the range [-1,1], but was: {0}", beta));

      if (alpha < 1)
      {
        ParameterConversionS0ToFeller(alpha, beta, abe, 1, 0, out var gamma, out var aga, out var sigmaf, out var muf);
        return StableDistributionFeller.PDF((x - muf) / sigmaf, alpha, gamma, aga) / sigmaf;
        /*
                PdfEvaluationMethod method = GetEvaluationMethod(alpha, (x - muf) / sigmaf, abe == 0, beta < 0);
                switch (method)
                {
                    case PdfEvaluationMethod.Integration:
                }
                */
      }
      else if (alpha > 1)
      {
        ParameterConversionS0ToFeller(alpha, beta, abe, 1, 0, out var gamma, out var aga, out var sigmaf, out var muf);
        return StableDistributionFeller.PDF((x - muf) / sigmaf, alpha, gamma, aga) / sigmaf;
      }
      else
      {
        return PDFMethodAlphaOne(x, beta, abe, ref tempStorage, precision);
      }
    }

    /// <summary>
    /// Calculates the PDF for the special case alpha = 1.
    /// </summary>
    /// <param name="x">The standardized argument.</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <param name="tempStorage">Temporary storage that can be reused between calls.</param>
    /// <param name="precision">Goal for the relative precision.</param>
    /// <returns>The probability density at <paramref name="x"/>.</returns>
    public static double PDFMethodAlphaOne(double x, double beta, double abe, ref object? tempStorage, double precision)
    {
      if (0 == beta)
      {
        return 1 / (Math.PI * (1 + x * x));
      }
      else
      {
        var inc = new Aeq1I(x, beta, abe);
        if (inc.IsMaximumLeftHandSide())
        {
          return inc.Integrate(ref tempStorage, precision);
        }
        else
        {
          var dec = new Aeq1D(x, beta, abe);
          return dec.Integrate(ref tempStorage, precision);
        }
      }
    }

    #endregion PDF

    #region CDF

    /// <summary>
    /// Gets the cumulative distribution function (CDF) of the standardized S0 stable distribution.
    /// </summary>
    /// <param name="x">The value where the distribution function is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <returns>The cumulative probability P(X &lt;= x).</returns>
    public static double CDF(double x, double alpha, double beta)
    {
      object? tempStorage = null;
      double abe = GetAbeFromBeta(beta);
      return CDF(x, alpha, beta, abe, 1, 0, ref tempStorage, DefaultPrecision);
    }

    /// <summary>
    /// Gets the cumulative distribution function (CDF) for the specified scale and location.
    /// </summary>
    /// <param name="x">The value where the distribution function is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="scale">Scale parameter.</param>
    /// <param name="location">Location parameter.</param>
    /// <returns>The cumulative probability P(X &lt;= x).</returns>
    public static double CDF(double x, double alpha, double beta, double scale, double location)
    {
      object? tempStorage = null;
      double abe = GetAbeFromBeta(beta);
      return CDF(x, alpha, beta, abe, scale, location, ref tempStorage, DefaultPrecision);
    }

    /// <summary>
    /// Gets the cumulative distribution function (CDF) of the standardized S0 stable distribution.
    /// </summary>
    /// <param name="x">The value where the distribution function is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="tempStorage">Temporary storage that can be reused between calls.</param>
    /// <param name="precision">Goal for the relative precision.</param>
    /// <returns>The cumulative probability P(X &lt;= x).</returns>
    public static double CDF(double x, double alpha, double beta, ref object? tempStorage, double precision)
    {
      double abe = GetAbeFromBeta(beta);
      return CDF(x, alpha, beta, abe, 1, 0, ref tempStorage, DefaultPrecision);
    }

    /// <summary>
    /// Gets the cumulative distribution function (CDF) for the specified scale and location.
    /// </summary>
    /// <param name="x">The value where the distribution function is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="scale">Scale parameter.</param>
    /// <param name="location">Location parameter.</param>
    /// <param name="tempStorage">Temporary storage that can be reused between calls.</param>
    /// <param name="precision">Goal for the relative precision.</param>
    /// <returns>The cumulative probability P(X &lt;= x).</returns>
    public static double CDF(double x, double alpha, double beta, double scale, double location, ref object? tempStorage, double precision)
    {
      double abe = GetAbeFromBeta(beta);
      return CDF(x, alpha, beta, abe, scale, location, ref tempStorage, DefaultPrecision);
    }

    /// <summary>
    /// Gets the cumulative distribution function (CDF) of the standardized S0 stable distribution.
    /// </summary>
    /// <param name="x">The value where the distribution function is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <returns>The cumulative probability P(X &lt;= x).</returns>
    public static double CDF(double x, double alpha, double beta, double abe)
    {
      object? temp = null;
      return CDF(x, alpha, beta, abe, 1, 0, ref temp, DefaultPrecision);
    }

    /// <summary>
    /// Gets the cumulative distribution function (CDF) for the specified scale and location.
    /// </summary>
    /// <param name="x">The value where the distribution function is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <param name="scale">Scale parameter.</param>
    /// <param name="location">Location parameter.</param>
    /// <returns>The cumulative probability P(X &lt;= x).</returns>
    public static double CDF(double x, double alpha, double beta, double abe, double scale, double location)
    {
      object? temp = null;
      return CDF(x, alpha, beta, abe, scale, location, ref temp, DefaultPrecision);
    }

    /// <summary>
    /// Gets the cumulative distribution function (CDF) for the specified S0 parameters, scale, and location.
    /// </summary>
    /// <param name="x">The value where the distribution function is evaluated.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <param name="scale">Scale parameter.</param>
    /// <param name="location">Location parameter.</param>
    /// <param name="tempStorage">Temporary storage that can be reused between calls.</param>
    /// <param name="precision">Goal for the relative precision.</param>
    /// <returns>The cumulative probability P(X &lt;= x).</returns>
    public static double CDF(double x, double alpha, double beta, double abe, double scale, double location, ref object? tempStorage, double precision)
    {
      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));
      if (!(beta >= -1 && beta <= 1))
        throw new ArgumentOutOfRangeException(string.Format("Beta must be in the range [-1,1], but was: {0}", beta));

      if (alpha != 1)
      {
        ParameterConversionS0ToFeller(alpha, beta, abe, scale, location, out var gamma, out var aga, out var sigmaf, out var muf);
        return StableDistributionFeller.CDF((x - muf) / sigmaf, alpha, gamma, aga, ref tempStorage, precision);
      }
      else
      {
        return CDFMethodAlphaOne((x - location) / scale, beta, abe, ref tempStorage, precision);
      }
    }

    /// <summary>
    /// Calculates the CDF for the special case alpha = 1.
    /// </summary>
    /// <param name="x">The standardized argument.</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <param name="tempStorage">Temporary storage that can be reused between calls.</param>
    /// <param name="precision">Goal for the relative precision.</param>
    /// <returns>The cumulative probability P(X &lt;= x).</returns>
    public static double CDFMethodAlphaOne(double x, double beta, double abe, ref object? tempStorage, double precision)
    {
      if (0 == beta)
      {
        return 0.5 + Math.Atan(x) / Math.PI;
      }
      else
      {
        var inc = new Aeq1I(x, beta, abe);

        if (beta > 0)
          return inc.CDFIntegrate(ref tempStorage, precision) / Math.PI;
        else
          return 1 - inc.CDFIntegrate(ref tempStorage, precision) / Math.PI;
      }
    }

    #endregion CDF

    #region Quantile

    /// <summary>
    /// Gets the quantile (inverse CDF) for the standardized S0 stable distribution.
    /// </summary>
    /// <param name="p">The probability in [0, 1].</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <returns>The value x such that CDF(x) = <paramref name="p"/>.</returns>
    public static double Quantile(double p, double alpha, double beta)
    {
      object? tempStorage = null;
      double abe = GetAbeFromBeta(beta);
      return Quantile(p, alpha, beta, abe, ref tempStorage, DefaultPrecision);
    }

    /// <summary>
    /// Gets the quantile (inverse CDF) for the standardized S0 stable distribution.
    /// </summary>
    /// <param name="p">The probability in [0, 1].</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <returns>The value x such that CDF(x) = <paramref name="p"/>.</returns>
    public static double Quantile(double p, double alpha, double beta, double abe)
    {
      object? tempStorage = null;
      return Quantile(p, alpha, beta, abe, ref tempStorage, DefaultPrecision);
    }

    /// <summary>
    /// Gets the quantile (inverse CDF) for the standardized S0 stable distribution.
    /// </summary>
    /// <param name="p">The probability in [0, 1].</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <param name="tempStorage">Temporary storage that can be reused between calls.</param>
    /// <param name="precision">Goal for the relative precision.</param>
    /// <returns>The value x such that CDF(x) = <paramref name="p"/>, or <see cref="double.NaN"/> if no root was found.</returns>
    public static double Quantile(double p, double alpha, double beta, double abe, ref object? tempStorage, double precision)
    {
      double xguess = Math.Exp(2 / alpha); // guess value for a nearly constant p value in dependence of alpha
      double x0 = -xguess;
      double x1 = xguess;

      object? temp = tempStorage;
      double root = double.NaN;
      if (QuickRootFinding.BracketRootByExtensionOnly(delegate (double x)
      { return CDF(x, alpha, beta, abe, 1, 0, ref temp, DefaultPrecision) - p; }, 0, ref x0, ref x1))
      {
        if (QuickRootFinding.ByBrentsAlgorithm(delegate (double x)
        { return CDF(x, alpha, beta, abe, 1, 0, ref temp, DefaultPrecision) - p; }, x0, x1, 0, DoubleConstants.DBL_EPSILON, out root) is not null)
          root = double.NaN;
      }
      tempStorage = temp;

      return root;
    }

    #endregion Quantile

    #region Calculation of integration parameters

    /// <summary>
    /// Computes parameters used for the direct integration approach for alpha &lt; 1 (Gn variant).
    /// </summary>
    /// <param name="x">The standardized argument.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <param name="factorp">Returned scaling factor p used by the integrand.</param>
    /// <param name="facdiv">Returned divisor/scaling factor used by the integrand.</param>
    /// <param name="dev">Returned phase shift value.</param>
    /// <param name="logPdfPrefactor">Returned logarithm of the multiplicative PDF prefactor.</param>
    public static void GetAlt1GnParameter(double x, double alpha, double beta, double abe,
                                          out double factorp, out double facdiv, out double dev, out double logPdfPrefactor)
    {
      double tan_pi_alpha_2 = TanXPiBy2(alpha);
      double zeta = -beta * tan_pi_alpha_2;
      double xx = x - zeta;

      double gamma = GammaFromAlphaBetaTanPiA2(alpha, beta, abe, tan_pi_alpha_2, out var aga);
      dev = Math.PI * (gamma < 0 ? 0.5 * aga : 1 - 0.5 * aga);
      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      facdiv = CosGammaPiBy2(alpha, gamma, aga); // Inverse part of the original factor without power
                                                 // for later use: facdiv = PowerOfOnePlusXSquared(beta * tan_pi_alpha_2, -0.5);
      factorp = xx * facdiv; // part of the factor with power alpha/(alpha-1);
      logPdfPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * (xx)));
    }

    /// <summary>
    /// Computes parameters used for the direct integration approach for alpha &lt; 1 (Gp variant).
    /// </summary>
    /// <param name="x">The standardized argument.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <param name="factorp">Returned scaling factor p used by the integrand.</param>
    /// <param name="facdiv">Returned divisor/scaling factor used by the integrand.</param>
    /// <param name="dev">Returned phase shift value.</param>
    /// <param name="logPdfPrefactor">Returned logarithm of the multiplicative PDF prefactor.</param>
    public static void GetAlt1GpParameter(double x, double alpha, double beta, double abe,
                                          out double factorp, out double facdiv, out double dev, out double logPdfPrefactor)
    {
      double tan_pi_alpha_2 = TanXPiBy2(alpha);
      double zeta = -beta * tan_pi_alpha_2;
      double xx = x - zeta;

      double gamma = GammaFromAlphaBetaTanPiA2(alpha, beta, abe, tan_pi_alpha_2, out var aga);
      dev = Math.PI * (gamma >= 0 ? 0.5 * aga : 1 - 0.5 * aga);
      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      facdiv = CosGammaPiBy2(alpha, gamma, aga); // Inverse part of the original factor without power
      factorp = xx * facdiv; // part of the factor with power alpha/(alpha-1);
      logPdfPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * xx));
    }

    /// <summary>
    /// Computes parameters used for the direct integration approach for alpha &gt; 1 (Gn variant).
    /// </summary>
    /// <param name="x">The standardized argument.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <param name="factorp">Returned scaling factor p used by the integrand.</param>
    /// <param name="factorw">Returned scaling factor w used by the integrand.</param>
    /// <param name="dev">Returned phase shift value.</param>
    /// <param name="logPrefactor">Returned logarithm of the multiplicative prefactor.</param>
    public static void GetAgt1GnParameter(double x, double alpha, double beta, double abe,
                                                 out double factorp, out double factorw, out double dev, out double logPrefactor)
    {
      double tan_pi_alpha_2 = TanXPiBy2(alpha);
      double zeta = -beta * tan_pi_alpha_2;
      double xx = x - zeta; // equivalent to x-zeta in S0 integral

      double gamma = GammaFromAlphaBetaTanPiA2(alpha, beta, abe, tan_pi_alpha_2, out var aga);

      dev = Math.PI * (gamma < 0 ? 0.5 * aga : (0.5 * ((2 - alpha) + gamma)));
      if (dev < 0)
        dev = 0;

      //double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      // we separate the factor in a power of 1/alpha-1 and the rest
      factorp = xx * CosGammaPiBy2(alpha, gamma, aga); // Math.Cos(-gamma * 0.5 * Math.PI);
      factorw = xx;

      logPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * xx));
    }

    /// <summary>
    /// Computes parameters used for the direct integration approach for alpha &gt; 1 (Gp variant).
    /// </summary>
    /// <param name="x">The standardized argument.</param>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">
    /// Parameter to specify beta with higher accuracy around -1 and 1.
    /// It is 1 - beta for beta &gt;= 0 or 1 + beta for beta &lt; 0.
    /// </param>
    /// <param name="factorp">Returned scaling factor p used by the integrand.</param>
    /// <param name="factorw">Returned scaling factor w used by the integrand.</param>
    /// <param name="dev">Returned phase shift value.</param>
    /// <param name="logPrefactor">Returned logarithm of the multiplicative prefactor.</param>
    public static void GetAgt1GpParameter(double x, double alpha, double beta, double abe,
                                                 out double factorp, out double factorw, out double dev, out double logPrefactor)
    {
      double tan_pi_alpha_2 = TanXPiBy2(alpha);
      double zeta = -beta * tan_pi_alpha_2;
      double xx = x - zeta; // equivalent to x-zeta in S0 integral

      double gamma = GammaFromAlphaBetaTanPiA2(alpha, beta, abe, tan_pi_alpha_2, out var aga);

      // original formula: dev = Math.PI * 0.5 * (alpha - gamma) / alpha;
      // original formula (2): Math.PI * 0.5 + Math.Atan(beta * tan_pi_alpha_2) / alpha;
      dev = Math.PI * 0.5 * (alpha - gamma) / alpha;
      dev = Math.PI * 0.5 + Math.Atan(beta * tan_pi_alpha_2) / alpha;

      if (dev < 0)
        dev = 0;

      //double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      // we separate the factor in a power of 1/alpha-1 and the rest
      factorp = xx * CosGammaPiBy2(alpha, gamma, aga); // Math.Cos(-gamma * 0.5 * Math.PI);
      factorw = xx;

      logPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * xx));
    }

    #endregion Calculation of integration parameters
  }
}
