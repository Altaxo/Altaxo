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
using Complex64T = System.Numerics.Complex;

namespace Altaxo.Calc.Probability
{
  using Altaxo.Calc.RootFinding;

  public class StableDistributionFeller : StableDistributionBase
  {
    private double _alpha;
    private double _gamma;
    private double _aga;

    private double _location;
    private double _scale = 1;

    private object? _tempStorePDF;
    private static readonly double _pdfPrecision = Math.Sqrt(DoubleConstants.DBL_EPSILON);

    #region construction

    /// <summary>
    /// Creates a new instance of this distribution with default parameters (alpha=1, beta=0) and the default generator.
    /// </summary>
    public StableDistributionFeller()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with default parameters (alpha=1, beta=0).
    /// </summary>
    /// <param name="generator">Random number generator to be used with this distribution.</param>
    public StableDistributionFeller(Generator generator)
      : this(1, 0, 1, 1, 0, generator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta) and the default random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="gamma">Distribution parameter gamma (skew).</param>
    public StableDistributionFeller(double alpha, double gamma)
      : this(alpha, gamma, GetAgaFromAlphaGamma(alpha, gamma), 1, 0, DefaultGenerator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta, abe) and the default random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="gamma">Distribution parameter gamma (skew).</param>
    /// <param name="aga">Parameter to specify gamma with higher accuracy around it's limit(s). For an explanation how aga is defined, see <see cref="GetAgaFromAlphaGamma"/>.</param>
    public StableDistributionFeller(double alpha, double gamma, double aga)
      : this(alpha, gamma, aga, 1, 0, DefaultGenerator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta, scale, location) and the default random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="gamma">Distribution parameter gamma (skew).</param>
    /// <param name="scale">Scaling parameter (broadness of the distribution).</param>
    /// <param name="location">Location of the distribution.</param>
    public StableDistributionFeller(double alpha, double gamma, double scale, double location)
      : this(alpha, gamma, GetAgaFromAlphaGamma(alpha, gamma), scale, location, DefaultGenerator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta, scale, location) and the provided random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="gamma">Distribution parameter gamma (skew).</param>
    /// <param name="scale">Scaling parameter (broadness of the distribution).</param>
    /// <param name="location">Location of the distribution.</param>
    /// <param name="generator">Random number generator to be used with this distribution.</param>
    public StableDistributionFeller(double alpha, double gamma, double scale, double location, Generator generator)
      : this(alpha, gamma, GetAgaFromAlphaGamma(alpha, gamma), scale, location, generator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta, abe, scale, location) and the default random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="gamma">Distribution parameter gamma (skew).</param>
    /// <param name="aga">Parameter to specify gamma with higher accuracy around it's limit(s). For an explanation how aga is defined, see <see cref="GetAgaFromAlphaGamma"/>.</param>
    /// <param name="scale">Scaling parameter (broadness of the distribution).</param>
    /// <param name="location">Location of the distribution.</param>
    public StableDistributionFeller(double alpha, double gamma, double aga, double scale, double location)
      : this(alpha, gamma, aga, scale, location, DefaultGenerator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta, abe, scale, location) and the provided random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="gamma">Distribution parameter gamma (skew).</param>
    /// <param name="aga">Parameter to specify gamma with higher accuracy around it's limit(s). For an explanation how aga is defined, see <see cref="GetAgaFromAlphaGamma"/>.</param>
    /// <param name="scale">Scaling parameter (broadness of the distribution).</param>
    /// <param name="location">Location of the distribution.</param>
    /// <param name="generator">Random number generator to be used with this distribution.</param>
    public StableDistributionFeller(double alpha, double gamma, double aga, double scale, double location, Generator generator)
      : base(generator)
    {
      Initialize(alpha, gamma, aga, scale, location);
    }

    #endregion construction

    #region Distribution members

    /// <summary>
    /// Initializes this instance of the distribution with the distribution parameters.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="gamma">Distribution parameter gamma (skew).</param>
    /// <param name="aga">Parameter to specify gamma with higher accuracy around it's limit(s). For an explanation how aga is defined, see <see cref="GetAgaFromAlphaGamma"/>.</param>
    /// <param name="scale">Scaling parameter (broadness of the distribution).</param>
    /// <param name="location">Location of the distribution.</param>
    public void Initialize(double alpha, double gamma, double aga, double scale, double location)
    {
      if (!IsValidAlpha(alpha))
        throw new ArgumentOutOfRangeException("Alpha out of range (must be greater 0.1 and smalle or equal than 2)");
      _alpha = alpha;

      if (!IsValidGamma(alpha, gamma))
        throw new ArgumentOutOfRangeException("Beta out of range (must be in the range [-1,1])");
      _gamma = gamma;
      _aga = aga;

      if (!IsValidScale(scale))
        throw new ArgumentOutOfRangeException("Scale out of range (must be >0)");
      _scale = scale;

      if (!IsValidLocation(location))
        throw new ArgumentOutOfRangeException("Location out of range (must be finite)");
      _location = location;

      if (_alpha != 1 && _alpha != 2)
      {
        _gen_t = -TanGammaPiBy2(_alpha, _gamma, _aga);
        _gen_B = -0.5 * Math.PI * _gamma / _alpha;
        _gen_S = PowerOfOnePlusXSquared(_gen_t, 0.5 / _alpha);
        ParameterConversionFellerToS0(_alpha, _gamma, _aga, _scale, _location, out var beta, out var abe, out _gen_Scale, out var mu0);
      }
      else if (alpha == 1) // this case is for the case alpha=1
      {
        if (Math.Abs(gamma) < 0.5)
        {
          _gen_B = CosXPiBy2(_gamma);
          _gen_S = SinXPiBy2(_gamma);
        }
        else
        {
          // it is possible here to use aga directly because alpha is 1
          _gen_B = SinXPiBy2(_aga);
          _gen_S = Math.Sign(gamma) * CosXPiBy2(_aga);
        }
      }
    }

    public static bool IsValidAlpha(double alpha)
    {
      return alpha > 0 && alpha <= 2;
    }

    public static bool IsValidGamma(double alpha, double gamma)
    {
      if (alpha <= 1)
        return Math.Abs(gamma) <= alpha;
      else
        return Math.Abs(gamma) <= (2 - alpha);
    }

    public static bool IsValidScale(double scale)
    {
      return scale > 0;
    }

    public static bool IsValidLocation(double location)
    {
      return location >= double.MinValue && location <= double.MaxValue;
    }

    public override double Minimum
    {
      get
      {
        if (_alpha < 1 && _gamma < 0 && _aga == 0)
          return _location;
        else
          return double.MinValue;
      }
    }

    public override double Maximum
    {
      get
      {
        if (_alpha < 1 && _gamma > 0 && _aga == 0)
          return _location;
        else
          return double.MaxValue;
      }
    }

    public override double Mean
    {
      get { throw new NotImplementedException(); }
    }

    public override double Median
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public override double Variance
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public override double[] Mode
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public override double NextDouble()
    {
      if (_gamma == 0)
      {
        return GenerateSymmetricCase(_alpha) * _scale + _location;
      }
      else
      {
        if (_alpha == 1)
        {
          return (GenerateSymmetricCase(_alpha) * _gen_B - _gen_S) * _scale + _location;
        }
        else
        {
          return GenerateAsymmetricCaseS1_ANe1(_alpha, _gen_t, _gen_B, _gen_S, _gen_Scale) + _location;
        }
      }
    }

    public override double PDF(double x)
    {
      return PDF(x, _alpha, _gamma, _aga, _scale, _location, ref _tempStorePDF, _pdfPrecision);
    }

    #endregion Distribution members

    #region PDF dispatcher

    public static double PDF(double x, double alpha, double gamma)
    {
      object? store = null;
      return PDF(x, alpha, gamma, ref store, Math.Sqrt(DoubleConstants.DBL_EPSILON));
    }

    public static double PDF(double x, double alpha, double gamma, double aga)
    {
      object? store = null;
      return PDF(x, alpha, gamma, aga, ref store, Math.Sqrt(DoubleConstants.DBL_EPSILON));
    }

    /// <summary>
    /// Calculates the probability density using either series expansion for small or big arguments, or a integration
    /// in the intermediate range.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="alpha">First parameter of the distribution (<see cref="StableDistributionFeller"/>).</param>
    /// <param name="gamma">Second parameter of the distribution (<see cref="StableDistributionFeller"/>).</param>
    /// <param name="tempStorage"></param>
    /// <param name="precision">Relative precision goal for the calculation.</param>
    /// <returns></returns>
    public static double PDF(double x, double alpha, double gamma, ref object? tempStorage, double precision)
    {
      double aga = GetAgaFromAlphaGamma(alpha, gamma);
      return PDF(x, alpha, gamma, aga, ref tempStorage, precision);
    }

    public static double PDF(double x, double alpha, double gamma, double aga, double scale, double pos, ref object? tempStorage, double precision)
    {
      return PDF((x - pos) / scale, alpha, gamma, aga, ref tempStorage, precision) / scale;
    }

    /// <summary>
    /// Calculates the probability density using either series expansion for small or big arguments, or a integration
    /// in the intermediate range.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga">For alpha &lt;=1: This is either (alpha-gamma)/alpha for gamma &gt;=0, or (alpha+gamma)/alpha for gamma &lt; 1.</param>
    /// <param name="tempStorage"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDF(double x, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      if (x == 0)
        return PDFforXZero(alpha, gamma, aga);
      if (x < 0)
      {
        x = -x;
        gamma = -gamma;
        // note: aga remains invariant
      }
      if (!(x > 0))
        return double.NaN;

      if (alpha <= 1)
      {
        return PDFforPositiveX(x, alpha, gamma, aga, ref tempStorage, precision);
      }
      else if (alpha > 1)
      {
        double xinv = Math.Pow(x, -alpha);
        double gammainv = (gamma - alpha + 1) / alpha;
        double againv = aga;
        if (gamma > 0)
          againv = gammainv > 0 ? 2 * (alpha - 1) + aga : 2 * (2 - alpha) - aga;

        return PDFforPositiveX(xinv, 1 / alpha, gammainv, againv, ref tempStorage, precision) * xinv / x;
      }
      else // alpha is undetermined
      {
        return double.NaN;
      }
    }

    #endregion PDF dispatcher

    #region CDF dispatcher

    public static double CDF(double x, double alpha, double gamma)
    {
      object? tempStorage = null;
      double aga = GetAgaFromAlphaGamma(alpha, gamma);
      return CDF(x, alpha, gamma, aga, ref tempStorage, DefaultPrecision);
    }

    public static double CDF(double x, double alpha, double gamma, double aga)
    {
      object? temp = null;
      return CDF(x, alpha, gamma, aga, ref temp, DefaultPrecision);
    }

    public static double CDF(double x, double alpha, double gamma, ref object? tempStorage, double precision)
    {
      double aga = GetAgaFromAlphaGamma(alpha, gamma);
      return CDF(x, alpha, gamma, aga, ref tempStorage, precision);
    }

    private static double CDFAtXZero(double alpha, double gamma, double aga)
    {
      double result;
      if (alpha < 1)
      {
        result = gamma >= 0 ? 1 - 0.5 * aga : 0.5 * aga;
      }
      else if (alpha == 1)
      {
        result = gamma >= 0 ? 1 - 0.5 * aga : 0.5 * aga;
      }
      else // alpha>1
      {
        result = (alpha + gamma) / (2 * alpha);
      }
      return result;
    }

    private static double CCDFAtXZero(double alpha, double gamma, double aga)
    {
      double result;
      if (alpha < 1)
      {
        result = gamma >= 0 ? 0.5 * aga : 1 - 0.5 * aga;
      }
      else if (alpha == 1)
      {
        result = gamma >= 0 ? 0.5 * aga : 1 - 0.5 * aga;
      }
      else // alpha>1
      {
        result = (alpha - gamma) / (2 * alpha);
      }
      return result;
    }

    public static double CDF(double x, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));

      double integFromXZero, integFromXInfinity, offs;
      if (x == 0)
      {
        return CDFAtXZero(alpha, gamma, aga);
      }
      else if (x > 0)
      {
        CDFMethodForPositiveX(x, alpha, gamma, aga, ref tempStorage, precision, out integFromXZero, out integFromXInfinity, out offs);
        return integFromXZero < integFromXInfinity ? (1 - offs) + integFromXZero : 1 - integFromXInfinity;
      }
      else // x<0
      {
        CDFMethodForPositiveX(-x, alpha, -gamma, aga, ref tempStorage, precision, out integFromXZero, out integFromXInfinity, out offs);
        return integFromXInfinity;
      }
    }

    public static double CCDF(double x, double alpha, double gamma)
    {
      object? tempStorage = null;
      double aga = GetAgaFromAlphaGamma(alpha, gamma);
      return CCDF(x, alpha, gamma, aga, ref tempStorage, DefaultPrecision);
    }

    public static double CCDF(double x, double alpha, double gamma, double aga)
    {
      object? tempStorage = null;
      return CCDF(x, alpha, gamma, aga, ref tempStorage, DefaultPrecision);
    }

    public static double CCDF(double x, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));

      double integFromXZero, integFromXInfinity, offs;
      if (0 == x)
      {
        return CCDFAtXZero(alpha, gamma, aga);
      }
      else if (x > 0)
      {
        CDFMethodForPositiveX(x, alpha, gamma, aga, ref tempStorage, precision, out integFromXZero, out integFromXInfinity, out offs);
        return integFromXInfinity;
      }
      else // x<0
      {
        CDFMethodForPositiveX(-x, alpha, -gamma, aga, ref tempStorage, precision, out integFromXZero, out integFromXInfinity, out offs);
        return integFromXZero < integFromXInfinity ? (1 - offs) + integFromXZero : 1 - integFromXInfinity;
      }
    }

    public static double XZCDF(double x, double alpha, double gamma)
    {
      object? tempStorage = null;
      double aga = GetAgaFromAlphaGamma(alpha, gamma);
      return XZCDF(x, alpha, gamma, aga, ref tempStorage, DefaultPrecision);
    }

    public static double XZCDF(double x, double alpha, double gamma, double aga)
    {
      object? tempStorage = null;
      return XZCDF(x, alpha, gamma, aga, ref tempStorage, DefaultPrecision);
    }

    public static double XZCDF(double x, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));
      double result = alpha == 1 ? 0.5 - CDFAtXZero(alpha, gamma, aga) : 0;

      double integFromXZero, integFromXInfinity, offs;
      if (x == 0)
      {
        result = 0;
      }
      else if (x > 0)
      {
        CDFMethodForPositiveX(x, alpha, gamma, aga, ref tempStorage, precision, out integFromXZero, out integFromXInfinity, out offs);
        result += integFromXZero;
      }
      else // x<0
      {
        CDFMethodForPositiveX(-x, alpha, -gamma, aga, ref tempStorage, precision, out integFromXZero, out integFromXInfinity, out offs);
        result += -integFromXZero;
      }

      return result;
    }

    public static void CDFMethodForPositiveX(double x, double alpha, double gamma, double aga, ref object? tempStorage, double precision, out double integFromXZero, out double integFromXInfinity, out double offs)
    {
      if (alpha <= 0)
        throw new ArgumentException("Alpha must be in the range alpha>0");

      if (alpha < 1)
      {
        if (gamma <= 0)
        {
          offs = 1 - 0.5 * aga;
          if (x == 0)
          {
            integFromXZero = 0;
            integFromXInfinity = offs;
          }
          else // x != 0
          {
            GetAlt1GnParameterByGamma(x, alpha, gamma, aga, out var factorp, out var facdiv, out var dev, out var logPdfPrefactor);
            var inc = new Alt1GnI(factorp, facdiv, logPdfPrefactor, alpha, dev);
            if (inc.IsMaximumLeftHandSide())
            {
              integFromXZero = inc.CDFIntegrate(ref tempStorage, precision) / Math.PI;
              integFromXInfinity = offs - integFromXZero;
            }
            else
            {
              integFromXInfinity = new Alt1GnD(factorp, facdiv, logPdfPrefactor, alpha, dev).CDFIntegrate(ref tempStorage, precision) / Math.PI;
              integFromXZero = offs - integFromXInfinity;
            }
          }
        }
        else // gamma>0
        {
          offs = 0.5 * aga;
          if (x == 0)
          {
            integFromXZero = 0;
            integFromXInfinity = offs;
          }
          else // x!=0
          {
            GetAlt1GpParameterByGamma(x, alpha, gamma, aga, out var factorp, out var facdiv, out var dev, out var logPdfPrefactor);
            var inc = new Alt1GpI(factorp, facdiv, logPdfPrefactor, alpha, dev);
            if (inc.IsMaximumLeftHandSide())
            {
              integFromXZero = inc.CDFIntegrate(ref tempStorage, precision) / Math.PI;
              integFromXInfinity = offs - integFromXZero;
            }
            else
            {
              integFromXInfinity = new Alt1GpD(factorp, facdiv, logPdfPrefactor, alpha, dev).CDFIntegrate(ref tempStorage, precision) / Math.PI;
              integFromXZero = offs - integFromXInfinity;
            }
          }
        }
      }
      else if (alpha == 1)
      {
        double a; // = Math.Cos(gamma*Math.PI/2);
        double b; // = Math.Sin(gamma*Math.PI/2);
        offs = 0.5;
        a = SinXPiBy2(aga); // for alpha=1 aga is 1-gamma or -1+gamma, thus we can turn cosine into sine
        b = SinXPiBy2(gamma); // for b it is not important to have high accuracy with gamma=1 or -1
        double arg = (b + x) / a;
        if (arg <= 1)
        {
          integFromXZero = Math.Atan(arg) / Math.PI;
          integFromXInfinity = offs - integFromXZero;
        }
        else
        {
          integFromXInfinity = Math.Atan(1 / arg) / Math.PI;
          integFromXZero = offs - integFromXInfinity;
        }
      }
      else if (alpha <= 2)
      {
        double xinv = Math.Pow(x, -alpha);
        double alphainv = 1 / alpha;
        double gammainv = (gamma - alpha + 1) / alpha;
        double againv = aga;
        if (gamma > 0)
          againv = gammainv > 0 ? 2 * (alpha - 1) + aga : 2 * (2 - alpha) - aga;
        else
          againv = aga;

        CDFMethodForPositiveX(xinv, alphainv, gammainv, againv, ref tempStorage, precision, out integFromXZero, out integFromXInfinity, out offs);
        double h = integFromXZero;
        integFromXZero = integFromXInfinity / alpha;
        integFromXInfinity = h / alpha;
        offs /= alpha;
      }
      else
      {
        throw new ArgumentException("Alpha not in the range 0<alpha<=2");
      }
    }

    #endregion CDF dispatcher

    #region Quantile

    public static double Quantile(double p, double alpha, double gamma)
    {
      object? tempStorage = null;
      double aga = GetAgaFromAlphaGamma(alpha, gamma);
      return Quantile(p, alpha, gamma, aga, ref tempStorage, DefaultPrecision);
    }

    public static double Quantile(double p, double alpha, double gamma, double aga)
    {
      object? tempStorage = null;
      return Quantile(p, alpha, gamma, aga, ref tempStorage, DefaultPrecision);
    }

    public static double Quantile(double p, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      if (p == 0.5)
        return 0;

      double xguess = Math.Exp(2 / alpha); // guess value for a nearly constant p value in dependence of alpha
      double x0, x1;

      if (p < 0.5)
      {
        x0 = -xguess;
        x1 = 0;
      }
      else
      {
        x0 = 0;
        x1 = xguess;
      }

      object? temp = tempStorage;
      double root = double.NaN;
      if (QuickRootFinding.BracketRootByExtensionOnly(delegate (double x)
      { return CDF(x, alpha, gamma, aga, ref temp, precision) - p; }, 0, ref x0, ref x1))
      {
        if (QuickRootFinding.ByBrentsAlgorithm(delegate (double x)
        { return CDF(x, alpha, gamma, aga, ref temp, precision) - p; }, x0, x1, 0, DoubleConstants.DBL_EPSILON, out root) is not null)
          root = double.NaN;
      }
      tempStorage = temp;
      return root;
    }

    public static double QuantileCCDF(double p, double alpha, double gamma)
    {
      object? tempStorage = null;
      double aga = GetAgaFromAlphaGamma(alpha, gamma);
      return QuantileCCDF(p, alpha, gamma, aga, ref tempStorage, DefaultPrecision);
    }

    public static double QuantileCCDF(double p, double alpha, double gamma, double aga)
    {
      object? tempStorage = null;
      return QuantileCCDF(p, alpha, gamma, aga, ref tempStorage, DefaultPrecision);
    }

    public static double QuantileCCDF(double q, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      if (q == 0.5)
        return 0;

      double xguess = Math.Exp(2 / alpha); // guess value for a nearly constant p value in dependence of alpha
      double x0, x1;

      if (q > 0.5)
      {
        x0 = -xguess;
        x1 = 0;
      }
      else
      {
        x0 = 0;
        x1 = xguess;
      }

      object? temp = tempStorage;
      double root = double.NaN;
      if (QuickRootFinding.BracketRootByExtensionOnly(delegate (double x)
      { return CCDF(x, alpha, gamma, aga, ref temp, precision) - q; }, 0, ref x0, ref x1))
      {
        if (QuickRootFinding.ByBrentsAlgorithm(delegate (double x)
        { return CCDF(x, alpha, gamma, aga, ref temp, precision) - q; }, x0, x1, 0, DoubleConstants.DBL_EPSILON, out root) is not null)
          root = double.NaN;
      }
      tempStorage = temp;

      return root;
    }

    #endregion Quantile

    #region Aga calculations

    /// <summary>
    /// Aga is defined as follows: For alpha &lt;=1, it is (alpha-gamma)/alpha (for gamma &gt;=0) or (alpha+gamma)/alpha (for gamma &lt;0).
    /// For alpha &gt;1 it is 2-alpha-gamma (for gamma &gt;=0) or 2-alpha+gamma (for gamma &lt;0).
    /// This function calculates aga from alpha and gamma.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="gamma">Distribution parameter gamma (skew).</param>
    /// <returns>The parameter aga.</returns>
    public static double GetAgaFromAlphaGamma(double alpha, double gamma)
    {
      double result;
      if (alpha <= 1)
      {
        if (gamma >= 0)
          result = (alpha - gamma) / alpha;
        else
          result = (alpha + gamma) / alpha;

        if (result < 0)
          result = 0;
        if (result > 1)
          result = 1;
      }
      else // alpha >1
      {
        if (gamma >= 0)
          result = (2 - alpha) - gamma;
        else
          result = (2 - alpha) + gamma;

        if (result < 0)
          result = 0;
        if (result > 1)
          result = 1;
      }

      return result;
    }

    /// <summary>
    /// Calculates gamma in dependence of alpha and aga.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="aga">Aga is defined as follows: For alpha &lt;=1, it is (alpha-gamma)/alpha (for gamma &gt;=0) or (alpha+gamma)/alpha (for gamma &lt;0).
    /// For alpha &gt;1 it is 2-alpha-gamma (for gamma &gt;=0) or 2-alpha+gamma (for gamma &lt;0).
    /// </param>
    /// <param name="isGammaNegative">Because gamma is not defined by aga alone, you have to specify if gamma should be positive or negative.</param>
    /// <returns>Distribution parameter gamma.</returns>
    public static double GetGammaFromAlphaAga(double alpha, double aga, bool isGammaNegative)
    {
      double result;
      if (alpha <= 1)
      {
        if (isGammaNegative)
          result = alpha * (aga - 1);
        else
          result = alpha * (1 - aga);
      }
      else // alpha>1
      {
        if (isGammaNegative)
          result = aga - (2 - alpha);
        else
          result = (2 - alpha) - aga;
      }
      return result;
    }

    public static void GetInvertedAlphaGammaAga(double x, double alpha, double gamma, double aga, out double alphainv, out double xinv, out double gammainv, out double againv)
    {
      alphainv = 1 / alpha;
      gammainv = (gamma - alpha + 1) / alpha;
      xinv = Math.Pow(x, -alpha);

      if (alpha > 1)
      {
        if (gamma > 0)
        {
          if (gammainv > 0)
            againv = 2 * (alpha - 1) + aga;
          else
            againv = 2 * (2 - alpha) - aga;
        }
        else
        {
          againv = aga;
        }
      }
      else if (alpha < 1)
      {
        if (gamma > 0)
        {
          againv = aga + 2 * (1 - alphainv);
        }
        else
        {
          if (gammainv <= 0)
            againv = aga;
          else
            againv = 2 * (2 - alphainv) - aga;
        }
      }
      else
        throw new ArgumentOutOfRangeException("alpha");
    }

    #endregion Aga calculations

    #region PDF calculations for different alpha ranges

    /// <summary>
    /// Calculates the probability density using either series expansion for small or big arguments, or a integration
    /// in the intermediate range.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="alpha"></param>
    /// <param name="gamma">The gamma value.</param>
    /// <param name="aga">For alpha &lt;=1: This is either (alpha-gamma)/alpha for gamma &gt;=0, or (alpha+gamma)/alpha for gamma &lt; 1.
    /// For alpha &gt;1, this is either (alpha-gamma) for gamma &gt; (alpha-1), or (2-alpha+gamma) for gamma &lt; (alpha-1).</param>
    /// <param name="tempStorage">Object that can be used to speed up subsequent calculations of the function. At first use, provide an object initialized with <see langword="null"/> and then provide this object in subsequent calls of this function.</param>
    /// <param name="precision">Relative precision goal.</param>
    /// <returns></returns>
    public static double PDFforPositiveX(double x, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      const double OneMinus2Eps = 1 - 4 * DoubleConstants.DBL_EPSILON;
      if (!(alpha > 0))
        throw new ArgumentException(string.Format("Parameter alpha must be >0, but is: {0}", alpha));
      if (!(alpha <= 2))
        throw new ArgumentException(string.Format("Parameter alpha must be <=2, but is: {0}", alpha));
      if (alpha <= 1)
      {
        if (Math.Abs(gamma * OneMinus2Eps) > alpha)
          throw new ArgumentException(string.Format("Absolute value of parameter gamma must be <={0}, but is {1}", 1 - Math.Abs(1 - alpha), gamma));
      }
      else
      {
        if (gamma > alpha)
          throw new ArgumentException(string.Format("Value of parameter gamma must be <={0}, but is {1}", alpha, gamma));
        else if (gamma < alpha - 2)
          throw new ArgumentException(string.Format("Value of parameter gamma must be >={0}, but is {1}", alpha - 2, gamma));
      }

      if (alpha <= 1)
      {
        // special case alpha==gamma
        if (gamma > 0 && aga == 0)
          return 0;

        if (alpha <= 0.2)
        {
          if (alpha < 0.1)
            return PDFAlphaBetween0And01(x, alpha, gamma, aga, ref tempStorage, precision);
          else
            return PDFAlphaBetween01And02(x, alpha, gamma, aga, ref tempStorage, precision);
        }
        else // alpha >0.2
        {
          if (alpha <= 0.99)
            return PDFAlphaBetween02And099(x, alpha, gamma, aga, ref tempStorage, precision);
          else
            return PDFAlphaBetween099And101(x, alpha, gamma, aga, ref tempStorage, precision);
        }
      }
      else // alpha>1
      {
        if (alpha <= 1.01)
          return PDFAlphaBetween099And101(x, alpha, gamma, aga, ref tempStorage, precision);
        else if (alpha <= 1.99995)
          return PDFAlphaBetween101And199999(x, alpha, gamma, aga, ref tempStorage, precision);
        else
          return PDFAlphaBetween199999And2(x, alpha, gamma, aga, ref tempStorage, precision);
      }
    }

    protected enum PdfEvaluationMethod { XZero, SeriesSmallX, SeriesBigX, Integration, IntegrationA1, AlphaEqualOne };

    protected static PdfEvaluationMethod GetEvaluationMethod(double alpha, double x, bool isGammaAtBoundary, bool isGammaPositive)
    {
      if (x == 0)
        return PdfEvaluationMethod.XZero;

      if (alpha <= 1)
      {
        // special case alpha==gamma
        //if (gamma > 0 && aga == 0)
        //return 0;

        if (alpha <= 0.2)
        {
          if (alpha < 0.1)
          {
            // return PDFAlphaBetween0And01(x, alpha, gamma, aga, ref tempStorage, precision);
            double smallestexp = GetLog10BoundaryForOneTermOfSeriesExpansionSmall(alpha, DoubleConstants.DBL_EPSILON);
            double lgx = Math.Log10(x);

            if (lgx <= smallestexp && (!isGammaAtBoundary || isGammaPositive))
              return PdfEvaluationMethod.XZero;
            else if (lgx > -0.3 / alpha)
              return PdfEvaluationMethod.SeriesBigX;
            else
              return PdfEvaluationMethod.Integration;
          }
          else
          {
            //return PDFAlphaBetween01And02(x, alpha, gamma, aga, ref tempStorage, precision);
            double a15 = alpha * Math.Sqrt(alpha);
            double smallestexp = -9 + 0.217147240951625 * ((-1.92074130618617 / a15 + 1.35936488329912 * a15));

            //  double smallestexp = 80 * alpha - 24; // Exponent is -16 for alpha=0.1 and -8 for alpha=0.2
            double lgx = Math.Log10(x);
            if (lgx <= smallestexp && (!isGammaAtBoundary || isGammaPositive))
              return PdfEvaluationMethod.XZero;
            else if (lgx > 3 / (1 + alpha))
              return PdfEvaluationMethod.SeriesBigX;
            else
              return PdfEvaluationMethod.Integration;
          }
        }
        else // alpha >0.2
        {
          if (alpha <= 0.99)
          {
            //return PDFAlphaBetween02And099(x, alpha, gamma, aga, ref tempStorage, precision);
            double smallestexp;
            if (alpha <= 0.3)
              smallestexp = 30 * alpha - 14; // Exponent is -8 for alpha=0.2 and -5 for alpha=0.3
            else if (alpha <= 0.6)
              smallestexp = 10 * alpha - 8; // Exponent is -5 for alpha=0.3 and -2 for alpha=0.6
            else
              smallestexp = 2.5 * alpha - 3.5; // Exponent is -2 for alpha=0.6 and -1 for alpha=1

            double lgx = Math.Log10(x);
            if (lgx <= smallestexp && (!isGammaAtBoundary || isGammaPositive))
              return PdfEvaluationMethod.SeriesSmallX;
            else if (lgx > 3 / (1 + alpha))
              return PdfEvaluationMethod.SeriesBigX;
            else
              return PdfEvaluationMethod.Integration;
          }
          else
          {
            //return PDFAlphaBetween099And101(x, alpha, gamma, aga, ref tempStorage, precision);
            if (alpha == 1)
              return PdfEvaluationMethod.AlphaEqualOne;
            if (x <= 0.1)
              return PdfEvaluationMethod.SeriesSmallX;
            else if (x >= 10)
              return PdfEvaluationMethod.SeriesBigX;
            else
              return PdfEvaluationMethod.IntegrationA1;
          }
        }
      }
      throw new ArgumentException();
    }

    /// <summary>Calculates the probability density at x=0 for the stable distribution in Feller's parametrization.</summary>
    /// <param name="alpha">Characteristic exponent of the distibution.</param>
    /// <param name="gamma">Skewness parameter gamma.</param>
    /// <param name="aga">'Alternative gamma' value to specify gamma with enhanced accuracy. For an explanation how it is defined, see <see cref="GetAgaFromAlphaGamma"/>.</param>
    /// <returns>Probability density value at x=0 for the stable distribution in Feller's parametrization.</returns>
    public static double PDFforXZero(double alpha, double gamma, double aga)
    {
      // use different methods, which provide the best accuracy for the case
      double result;
      if (alpha <= 1)
      {
        if (Math.Abs(gamma + gamma) < alpha)
          result = GammaRelated.Gamma(1 / alpha) / (alpha * Math.PI) * Math.Cos(0.5 * Math.PI * gamma / alpha);
        else
          result = GammaRelated.Gamma(1 / alpha) / (alpha * Math.PI) * Math.Sin(aga * 0.5 * Math.PI);
      }
      else // alpha>1
      {
        if (Math.Abs(gamma + gamma) < alpha)
          result = GammaRelated.Gamma(1 / alpha) / (alpha * Math.PI) * Math.Cos(0.5 * Math.PI * gamma / alpha);
        else if (gamma >= 0)
          result = GammaRelated.Gamma(1 / alpha) / (alpha * Math.PI) * Math.Sin(0.5 * Math.PI * (alpha - gamma) / alpha);
        else
          result = GammaRelated.Gamma(1 / alpha) / (alpha * Math.PI) * Math.Sin(0.5 * Math.PI * (alpha - gamma) / alpha);
      }
      return result;
    }

    private static double GetLog10BoundaryForOneTermOfSeriesExpansionSmall(double alpha, double precision)
    {
      const double Log10 = 2.302585092994045684017991;
      // we use Stirlings formula to approximate
      // x < precision * Gamma(1+1/alpha)/Gamma(1/2+alpha)
      double OneByAlpha = 1 / alpha;
      double r = Math.Log(1 + OneByAlpha) * (0.5 + OneByAlpha) + Math.Log(1 + 2 * OneByAlpha) * (-0.5 - 2 * OneByAlpha) + OneByAlpha + Math.Log(precision);
      return r / Log10;
    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 0.0 and 0.1.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween0And01(double x, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      double smallestexp = GetLog10BoundaryForOneTermOfSeriesExpansionSmall(alpha, DoubleConstants.DBL_EPSILON);
      double lgx = Math.Log10(x);

      if (lgx <= smallestexp && (aga != 0 || gamma >= 0))
        return PDFforXZero(alpha, gamma, aga);
      else if (lgx > -0.3 / alpha)
        return PDFSeriesBigXFeller(x, alpha, gamma, aga);
      else
        return PDFIntegral(x, alpha, gamma, aga, ref tempStorage, precision);
    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 0.1 and 0.2. For small x (1E-16), the accuracy at alpha=0.1 is only 1E-7.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween01And02(double x, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      double a15 = alpha * Math.Sqrt(alpha);
      double smallestexp = -9 + 0.217147240951625 * ((-1.92074130618617 / a15 + 1.35936488329912 * a15));

      //  double smallestexp = 80 * alpha - 24; // Exponent is -16 for alpha=0.1 and -8 for alpha=0.2
      double lgx = Math.Log10(x);
      if (lgx <= smallestexp && (aga != 0 || gamma >= 0))
        return PDFforXZero(alpha, gamma, aga);
      else if (lgx > 3 / (1 + alpha))
        return PDFSeriesBigXFeller(x, alpha, gamma, aga);
      else
        return PDFIntegral(x, alpha, gamma, aga, ref tempStorage, precision);
    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 0.2 and 0.99. For small x (1E-8), the accuracy at alpha=0.2 is only 1E-7.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween02And099(double x, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      double smallestexp;
      if (alpha <= 0.3)
        smallestexp = 30 * alpha - 14; // Exponent is -8 for alpha=0.2 and -5 for alpha=0.3
      else if (alpha <= 0.6)
        smallestexp = 10 * alpha - 8; // Exponent is -5 for alpha=0.3 and -2 for alpha=0.6
      else
        smallestexp = 2.5 * alpha - 3.5; // Exponent is -2 for alpha=0.6 and -1 for alpha=1

      double lgx = Math.Log10(x);
      if (lgx <= smallestexp && (aga != 0 || gamma >= 0))
        return PDFSeriesSmallXFeller(x, alpha, gamma, aga);
      else if (lgx > 3 / (1 + alpha))
        return PDFSeriesBigXFeller(x, alpha, gamma, aga);
      else
        return PDFIntegral(x, alpha, gamma, aga, ref tempStorage, precision);
    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 0.99 and 1.01.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween099And101(double x, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      if (alpha == 1)
        return PDFAlphaEqualOne(x, alpha, gamma, aga);
      if (x <= 0.1)
        return PDFSeriesSmallXFeller(x, alpha, gamma, aga);
      else if (x >= 10)
        return PDFSeriesBigXFeller(x, alpha, gamma, aga);
      else
        return PDFIntegralA1(x, alpha, gamma, aga, ref tempStorage, precision);
    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 1.01 and 1.99999.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween101And199999(double x, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      if (x <= 1E-2)
        return PDFSeriesSmallXFeller(x, alpha, gamma, aga);
      else if (x >= 100)
        return PDFSeriesBigXFeller(x, alpha, gamma, aga);
      else
        return PDFIntegral(x, alpha, gamma, aga, ref tempStorage, precision);
    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 1.99999 and 2. For small x ( max 7), the asymptotic expansion is used.
    /// For big x, the maximum value resulting from direct integration and series expansion w.r.t. alpha is used.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween199999And2(double x, double alpha, double gamma, double aga, ref object? tempStorage, double precision)
    {
      if (alpha == 2)
        return Math.Exp(-0.25 * x * x) / (2 * Math.Sqrt(Math.PI)); // because gamma must be zero for alpha==2

      if (x <= 7)
        return PDFSeriesSmallXFeller(x, alpha, gamma, aga);
      else
        throw new NotImplementedException(); // normally we use the alpha inversion formula for this case
    }

    #endregion PDF calculations for different alpha ranges

    #region Series expansion for small x

    /// <summary>
    /// Imaginary part of the Fourier transformed derivative of the Kohlrausch function for low frequencies.
    /// </summary>
    /// <param name="z">Circular frequency.</param>
    /// <param name="alpha">Beta parameter.</param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
    /// <remarks>This is the imaginary part of the Fourier transform (in Mathematica notation): Im[Integrate[D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. The sign of
    /// the return value here is positive!.</remarks>
    public static double PDFSeriesSmallXFeller(double z, double alpha, double gamma, double aga)
    {
      int k = 1;
      double mz_pow_k = -1;
      double z_square = z * z;
      double kfac = 1;

      double piga_2a;
      if (alpha <= 1)
      {
        if (gamma >= 0)
        {
          piga_2a = -Math.PI * 0.5 * aga;
        }
        else // gamma<0, we have to take Sin(k*(Pi-x)), which is Sin(k*x)*^(-1)^k-1), so we change the sign of z
        {
          piga_2a = -Math.PI * 0.5 * aga;
          z = -z;
        }
      }
      else // alpha>1
      {
        piga_2a = 0.5 * Math.PI * (gamma - alpha) / alpha;
      }
      if (0 == piga_2a)
        return 0;

      double term1 = GammaRelated.Gamma(1 + k / alpha) * mz_pow_k / kfac * Math.Sin(k * piga_2a);

      if (z_square == 0)
        return term1 / (Math.PI); // if z was so small that z_square can not be evaluated, return after term1

      k = 2;
      kfac = 2;
      mz_pow_k *= -z;
      double term2 = GammaRelated.Gamma(1 + k / alpha) * mz_pow_k / kfac * Math.Sin(k * piga_2a);

      double curr = term1 + term2;
      double sum = curr;
      double prev = curr;

      for (; ; )
      {
        ++k;
        kfac *= k;
        mz_pow_k *= -z;
        term1 = GammaRelated.Gamma(1 + k / alpha) * mz_pow_k / kfac * Math.Sin(k * piga_2a);

        ++k;
        kfac *= k;
        mz_pow_k *= -z;
        term2 = GammaRelated.Gamma(1 + k / alpha) * mz_pow_k / kfac * Math.Sin(k * piga_2a);
        ;

        curr = term1 + term2;

        if (Math.Abs(curr) < Math.Abs(sum * 1E-15))
          break;
        if ((Math.Abs(curr) > Math.Abs(prev)) && (Math.Abs(term2) > Math.Abs(term1))) // sum is not converging
        {
          sum = double.NaN;
          break;
        }
        if (double.IsInfinity(kfac))
        {
          sum = double.NaN;
          break;
        }

        prev = curr;
        sum += curr;
      }
      return sum / (Math.PI);
    }

    #endregion Series expansion for small x

    #region Series expansion for big x

    /// <summary>
    /// Series expansion for big x, Feller parametrization. x should be &gt; 0.
    /// </summary>
    /// <param name="z"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <returns></returns>
    public static double PDFSeriesBigXFeller(double z, double alpha, double gamma, double aga)
    {
      int k = 1;
      double pi_gamma_alpha_2;
      bool isCurrentSinusSignPositive = true;
      bool isSinusSignChanging = false;
      if (alpha <= 1 && gamma >= 0)
      {
        pi_gamma_alpha_2 = -Math.PI * 0.5 * aga * alpha;
      }
      else if (alpha > 1 && gamma < 0) // gamma-alpha is in this case -2+aga
      {
        // pi_gamma_alpha_2 = Math.PI * (-1 + 0.5*aga); // this would be the original calculation, but we replace it by sin(k*(-Pi+x))== (-1)^k * sin(k*x)
        pi_gamma_alpha_2 = Math.PI * 0.5 * aga;
        isCurrentSinusSignPositive = false;
        isSinusSignChanging = true;
      }
      else if (alpha <= 1 && (gamma - alpha) < -1.9)
      {
        // pi_gamma_alpha_2 = Math.PI * (gamma - alpha) / 2; // this would be the original solution
        pi_gamma_alpha_2 = Math.PI * (1 - alpha + 0.5 * aga * alpha);
        isCurrentSinusSignPositive = false;
        isSinusSignChanging = true;
      }
      else
      {
        pi_gamma_alpha_2 = Math.PI * (gamma - alpha) / 2;
      }

      if (pi_gamma_alpha_2 == 0)
        return 0;

      double z_pow_minusAlpha = Math.Pow(z, -alpha);
      double kfac = 1;
      double z_pow_minusKAlpha = -z_pow_minusAlpha;

      double term1a = GammaRelated.Gamma(1 + k * alpha) * z_pow_minusKAlpha / kfac;
      double term1 = term1a * (isCurrentSinusSignPositive ? Math.Sin(k * pi_gamma_alpha_2) : -Math.Sin(k * pi_gamma_alpha_2));
      isCurrentSinusSignPositive ^= isSinusSignChanging;

      double sum = term1;

      for (; ; )
      {
        ++k;
        z_pow_minusKAlpha *= -z_pow_minusAlpha;
        kfac *= k;

        double term2a = GammaRelated.Gamma(1 + k * alpha) * z_pow_minusKAlpha / kfac;
        double term2 = term2a * (isCurrentSinusSignPositive ? Math.Sin(k * pi_gamma_alpha_2) : -Math.Sin(k * pi_gamma_alpha_2));
        isCurrentSinusSignPositive ^= isSinusSignChanging;

        if (Math.Abs(term2a) < Math.Abs(sum * 1E-15))
          break;
        if (Math.Abs(term2a) > Math.Abs(term1a) || double.IsInfinity(kfac)) // sum is not converging
        {
          sum = double.NaN;
          break;
        }

        term1a = term2a; // take as previous term
        sum += term2;
      }
      return sum / (Math.PI * z);
    }

    #endregion Series expansion for big x

    #region Taylor series around alpha=1

    public static double PDFAlphaEqualOne(double x, double alpha, double gamma, double aga)
    {
      Complex64T expIgPi2;

      if (aga < 0.5)
      {
        double agaPi2 = aga * 0.5 * Math.PI; // Note: we can use aga here because alpha==1
        if (gamma < 0)
          expIgPi2 = new Complex64T(Math.Sin(agaPi2), -Math.Cos(agaPi2));
        else
          expIgPi2 = new Complex64T(Math.Sin(agaPi2), Math.Cos(agaPi2));
      }
      else
      {
        double gammaPi2 = gamma * 0.5 * Math.PI;
        expIgPi2 = new Complex64T(Math.Cos(gammaPi2), Math.Sin(gammaPi2));
      }
      Complex64T term0 = 1 / (expIgPi2 + new Complex64T(0, x));
      return term0.Real / Math.PI;
    }

    public static double PDFTaylorExpansionAroundAlphaOne(double x, double alpha, double gamma, double aga)
    {
      const double EulerGamma = 0.57721566490153286060651209008240243;
      //const double EulerGamma_P2 = EulerGamma * EulerGamma;
      //const double EulerGamma_P3 = EulerGamma_P2 * EulerGamma;
      const double Zeta3 = 1.202056903159594285399738; // Zeta[3]
      const double SQR_PI = Math.PI * Math.PI;

      // Note: it is much much easier to calculate with Complex64T numbers than to try to take only the real part
      Complex64T expIgPi2 = Complex64T.Exp(Complex64T.ImaginaryOne * (Math.PI * gamma * 0.5));
      Complex64T log_expix = Complex64T.Log(expIgPi2 + Complex64T.ImaginaryOne * x);

      Complex64T term0, term1, term2, term3;

      // Note: the termp exp_i_pi_g/2 is common to all terms, we use it later on

      term0 = 1 / (expIgPi2 + Complex64T.ImaginaryOne * x);

      term1 = (expIgPi2 * (-1 + EulerGamma + log_expix)) / Complex64T.Pow(expIgPi2 + Complex64T.ImaginaryOne * x, 2);

      term2 = (expIgPi2 * (expIgPi2 * (12 + 6 * (-4 + EulerGamma) * EulerGamma + SQR_PI) - Complex64T.ImaginaryOne * (6 * (-2 + EulerGamma) * EulerGamma + SQR_PI) * x +
       6 * log_expix * (2 * (-2 + EulerGamma) * expIgPi2 - new Complex64T(0, 2) * (-1 + EulerGamma) * x +
          (expIgPi2 - Complex64T.ImaginaryOne * x) * log_expix))) / (12 * Complex64T.Pow(expIgPi2 + Complex64T.ImaginaryOne * x, 3));

      term3 = (expIgPi2 * (((expIgPi2 * expIgPi2) * (36 + 6 * (-6 + EulerGamma) * EulerGamma + SQR_PI) -
          new Complex64T(0, 4) * expIgPi2 * (9 + 3 * EulerGamma * (-7 + 2 * EulerGamma) + SQR_PI) * x - (6 * (-2 + EulerGamma) * EulerGamma + SQR_PI) * RMath.Pow2(x)) *
        log_expix + 6 * ((-3 + EulerGamma) * (expIgPi2).Pow2() - Complex64T.ImaginaryOne * (-7 + 4 * EulerGamma) * expIgPi2 * x -
          (-1 + EulerGamma) * RMath.Pow2(x)) * (log_expix).Pow2() +
       2 * ((expIgPi2).Pow2() - new Complex64T(0, 4) * expIgPi2 * x - RMath.Pow2(x)) * (log_expix).Pow3() +
       RMath.Pow2(x) * (SQR_PI - EulerGamma * (2 * (-3 + EulerGamma) * EulerGamma + SQR_PI) - 4 * Zeta3) +
       (expIgPi2).Pow2() * (-3 * (4 + SQR_PI) + EulerGamma * (36 + 2 * (-9 + EulerGamma) * EulerGamma + SQR_PI) + 4 * Zeta3) -
       Complex64T.ImaginaryOne * expIgPi2 * x * (-7 * SQR_PI + 2 * EulerGamma * (EulerGamma * (-21 + 4 * EulerGamma) + 2 * (9 + SQR_PI)) + 16 * Zeta3))) /
        (12 * Complex64T.Pow(expIgPi2 + Complex64T.ImaginaryOne * x, 4));

      double am1 = alpha - 1;
      Complex64T result = ((term3 * am1 + term2) * am1 + term1) * am1 + term0;
      result /= Math.PI;

      return result.Real;
    }

    #endregion Taylor series around alpha=1

    #region Integration

    public static double PDFIntegral(double x, double alpha, double gamma, double aga, ref object? temp, double precision)
    {
      if (alpha < 1)
      {
        if (gamma < 0)
          return PDFIntegralAlt1Gn(x, alpha, gamma, aga, ref temp, precision);
        else
          return PDFIntegralAlt1Gp(x, alpha, gamma, aga, ref temp, precision);
      }
      else // alpha>1
      {
        return PDFIntegralAgt1Gn(x, alpha, gamma, aga, ref temp, precision);
      }
    }

    /// <summary>
    /// Special version of the PDF-Integral for 0.99&gt;=alpha&lt;1.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralA1(double x, double alpha, double gamma, double aga, ref object? temp, double precision)
    {
      if (gamma < 0)
      {
        double integrand;
        GetAlt1GnParameterByGamma(x, alpha, gamma, aga, out var factorp, out var facdiv, out var dev, out var prefactor);
        var ingI = new Alt1GnIA1(factorp, facdiv, prefactor, alpha, dev);
        if (dev == 0 || ingI.IsMaximumLeftHandSide())
        {
          integrand = ingI.PDFIntegrateAlphaNearOne(ref temp, precision);
        }
        else
        {
          integrand = new Alt1GnDA1(factorp, facdiv, prefactor, alpha, dev).PDFIntegrateAlphaNearOne(ref temp, precision);
        }
        return integrand;
      }
      else // gamma>=0
      {
        double integrand;
        GetAlt1GpParameterByGamma(x, alpha, gamma, aga, out var factorp, out var facdiv, out var dev, out var prefactor);
        var intI = new Alt1GpIA1(factorp, facdiv, prefactor, alpha, dev);
        if (intI.IsMaximumLeftHandSide())
        {
          integrand = intI.PDFIntegrateAlphaNearOne(ref temp, precision); // IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAlt1GpI(factorp, facdiv, alpha, dev, theta); }, 0, dev, ref temp, precision);
        }
        else
        {
          integrand = new Alt1GpDA1(factorp, facdiv, prefactor, alpha, dev).PDFIntegrateAlphaNearOne(ref temp, precision); // IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAlt1GpD(factorp, facdiv, alpha, dev, theta); }, 0, dev, ref temp, precision);
        }
        return integrand;
      }
    }

    #region Integral Alt1Gn

    public static void GetAlt1GnParameterByGamma(double x, double alpha, double gamma, double aga,
      out double factorp, out double facdiv, out double dev, out double logPdfPrefactor)
    {
      double zeta = TanGammaPiBy2(alpha, gamma, aga); // Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = PowerOfOnePlusXSquared(zeta, 0.5 / alpha);
      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral

      // take alphaPlusGammaByAlpha, a value between 0 and 2 (for alpha<1)
      double alphaPlusGammaBy2Alpha;
      if (gamma < 0)
        alphaPlusGammaBy2Alpha = 0.5 * aga;
      else
        alphaPlusGammaBy2Alpha = 1 - 0.5 * aga;

      dev = Math.PI * alphaPlusGammaBy2Alpha;
      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      facdiv = CosGammaPiBy2(alpha, gamma, aga); // Inverse part of the original factor without power
      factorp = xx * facdiv; // part of the factor with power alpha/(alpha-1);

      logPdfPrefactor = Math.Log(sigmaf * alpha / (Math.PI * Math.Abs(alpha - 1) * (xx)));
    }

    /// <summary>
    /// Special case for alpha &lt; 1 and gamma near to -alpha (i.e. gamma at the negative border).
    /// Here gamma was set to -alpha*(1-dev*2/Pi).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAlt1Gn(double x, double alpha, double gamma, double aga, ref object? temp, double precision)
    {
      GetAlt1GnParameterByGamma(x, alpha, gamma, aga, out var factorp, out var facdiv, out var dev, out var prefactor);

      double integrand;
      var ingI = new Alt1GnI(factorp, facdiv, prefactor, alpha, dev);
      if (ingI.IsMaximumLeftHandSide())
      {
        integrand = ingI.PDFIntegrate(ref temp, precision);
        if (double.IsNaN(integrand))
          integrand = new Alt1GnD(factorp, facdiv, prefactor, alpha, dev).Integrate(ref temp, precision);
      }
      else
      {
        integrand = new Alt1GnD(factorp, facdiv, prefactor, alpha, dev).Integrate(ref temp, precision);
        if (double.IsNaN(integrand))
          integrand = ingI.PDFIntegrate(ref temp, precision);
      }

      return integrand;
    }

    /// <summary>
    /// Special case for alpha &lt; 1 and gamma near to -alpha (i.e. gamma at the negative border).
    /// Here gamma was set to -alpha*(1-dev*2/Pi).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAlt1GnI(double x, double alpha, double gamma, double aga, ref object? temp, double precision)
    {
      GetAlt1GnParameterByGamma(x, alpha, gamma, aga, out var factorp, out var facdiv, out var dev, out var prefactor);

      var ing = new Alt1GnI(factorp, facdiv, prefactor, alpha, dev);
      double integrand = ing.PDFIntegrate(ref temp, precision);
      return integrand;
    }

    /// <summary>
    /// Special case for alpha &lt; 1 and gamma near to -alpha (i.e. gamma at the negative border).
    /// Here gamma was set to -alpha*(1-dev*2/Pi). The core function is decreasing.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAlt1GnD(double x, double alpha, double gamma, double aga, ref object? temp, double precision)
    {
      GetAlt1GnParameterByGamma(x, alpha, gamma, aga, out var factorp, out var facdiv, out var dev, out var prefactor);

      double integrand = new Alt1GnD(factorp, facdiv, prefactor, alpha, dev).Integrate(ref temp, precision);
      return integrand;
    }

    #endregion Integral Alt1Gn

    #region Integral Alt1Gp

    public static void GetAlt1GpParameterByGamma(double x, double alpha, double gamma, double aga,
    out double factorp, out double facdiv, out double dev, out double logPdfPrefactor)
    {
      double zeta = TanGammaPiBy2(alpha, gamma, aga); // Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = PowerOfOnePlusXSquared(zeta, 0.5 / alpha); // Math.Pow(1 + zeta * zeta, 0.5 / alpha);
      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral

      // take alphaMinusGammaByAlpha, a value between 0 and 2 (for alpha<1)
      double alphaMinusGammaBy2Alpha;
      if (gamma >= 0)
        alphaMinusGammaBy2Alpha = 0.5 * aga;
      else
        alphaMinusGammaBy2Alpha = 1 - 0.5 * aga;

      dev = Math.PI * alphaMinusGammaBy2Alpha;
      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      facdiv = CosGammaPiBy2(alpha, gamma, aga); // Inverse part of the original factor without power
      factorp = xx * facdiv; // part of the factor with power alpha/(alpha-1);

      logPdfPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * x));
    }

    /// <summary>
    /// Special case for alpha &lt; 1 and gamma near to +alpha (i.e. gamma at the positive border).
    /// Here gamma was set to alpha*(1-dev*2/Pi).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAlt1Gp(double x, double alpha, double gamma, double aga, ref object? temp, double precision)
    {
      GetAlt1GpParameterByGamma(x, alpha, gamma, aga, out var factorp, out var facdiv, out var dev, out var prefactor);

      double integrand;
      var intI = new Alt1GpI(factorp, facdiv, prefactor, alpha, dev);
      if (intI.IsMaximumLeftHandSide())
        integrand = intI.Integrate(ref temp, precision); // IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAlt1GpI(factorp, facdiv, alpha, dev, theta); }, 0, dev, ref temp, precision);
      else
        integrand = new Alt1GpD(factorp, facdiv, prefactor, alpha, dev).Integrate(ref temp, precision); // IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAlt1GpD(factorp, facdiv, alpha, dev, theta); }, 0, dev, ref temp, precision);

      return integrand;
    }

    /// <summary>
    /// Special case for alpha &lt; 1 and gamma near to +alpha (i.e. gamma at the positive border).
    /// Here gamma was set to alpha*(1-dev*2/Pi).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAlt1GpI(double x, double alpha, double gamma, double aga, ref object? temp, double precision)
    {
      GetAlt1GpParameterByGamma(x, alpha, gamma, aga, out var factorp, out var facdiv, out var dev, out var prefactor);

      var intI = new Alt1GpI(factorp, facdiv, prefactor, alpha, dev);
      double integrand = intI.Integrate(ref temp, precision); // IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAlt1GpI(factorp, facdiv, alpha, dev, theta); }, 0, dev, ref temp, precision);
      return integrand;
    }

    /// <summary>
    /// Special case for alpha &lt; 1 and gamma near to +alpha (i.e. gamma at the positive border).
    /// Here gamma was set to alpha*(1-dev*2/Pi).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAlt1GpD(double x, double alpha, double gamma, double aga, ref object? temp, double precision)
    {
      GetAlt1GpParameterByGamma(x, alpha, gamma, aga, out var factorp, out var facdiv, out var dev, out var prefactor);

      var intD = new Alt1GpD(factorp, facdiv, prefactor, alpha, dev);
      double integrand = intD.Integrate(ref temp, precision); // IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAlt1GpD(factorp, facdiv, alpha, dev, theta); }, 0, dev, ref temp, precision);
      return integrand;
    }

    #endregion Integral Alt1Gp

    #region Integral Agt1Gn

    public static void GetAgt1GnParameterByGamma(double x, double alpha, double gamma, double aga,
   out double factorp, out double factorw, out double dev, out double logPrefactor)
    {
      double zeta = TanGammaPiBy2(alpha, gamma, aga); // Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = PowerOfOnePlusXSquared(zeta, 0.5 / alpha); // Math.Pow(1 + zeta * zeta, 0.5 / alpha);
      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral

      if (gamma < 0)
        dev = Math.PI * 0.5 * aga;
      else
        dev = Math.PI * (0.5 * ((2 - alpha) + gamma));
      if (dev < 0)
        dev = 0;

      //double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      // we separate the factor in a power of 1/alpha-1 and the rest
      factorp = xx * CosGammaPiBy2(alpha, gamma, aga); // Math.Cos(-gamma * 0.5 * Math.PI);
      factorw = xx;

      logPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * x));
    }

    /// <summary>
    /// Special case for alpha>1 and gamma near to alpha-2 (i.e. gamma at the negative border).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAgt1Gn(double x, double alpha, double gamma, double aga, ref object? temp, double precision)
    {
      GetAgt1GnParameterByGamma(x, alpha, gamma, aga, out var factorp, out var factorw, out var dev, out var prefactor);

      var intI = new Agt1GnI(factorp, factorw, prefactor, alpha, dev);
      double integrand;
      if (intI.IsMaximumLeftHandSide())
        integrand = intI.Integrate(ref temp, precision); // IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAgt1GnI(factorp, factorw, alpha, dev, theta); }, 0, (Math.PI - dev) / alpha, ref temp, precision);
      else
        integrand = new Agt1GnD(factorp, factorw, prefactor, alpha, dev).Integrate(ref temp, precision); //  IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAgt1GnD(factorp, factorw, alpha, dev, theta); }, 0, (Math.PI - dev) / alpha, ref temp, precision);

      return integrand;
    }

    /// <summary>
    /// Special case for alpha>1 and gamma near to alpha-2 (i.e. gamma at the negative border).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAgt1GnD(double x, double alpha, double gamma, double aga, ref object? temp, double precision)
    {
      GetAgt1GnParameterByGamma(x, alpha, gamma, aga, out var factorp, out var factorw, out var dev, out var prefactor);

      var intD = new Agt1GnD(factorp, factorw, prefactor, alpha, dev);
      double integrand = intD.Integrate(ref temp, precision); // IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAgt1GnD(factorp, factorw, alpha, dev, theta); }, 0, (Math.PI - dev) / alpha, ref temp, precision);
      return integrand;
    }

    /// <summary>
    /// Special case for alpha>1 and gamma near to alpha-2 (i.e. gamma at the negative border).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="aga"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAgt1GnI(double x, double alpha, double gamma, double aga, ref object? temp, double precision)
    {
      GetAgt1GnParameterByGamma(x, alpha, gamma, aga, out var factorp, out var factorw, out var dev, out var prefactor);

      var intI = new Agt1GnI(factorp, factorw, prefactor, alpha, dev);
      double integrand = intI.Integrate(ref temp, precision); // IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAgt1GnI(factorp, factorw, alpha, dev, theta); }, 0, (Math.PI - dev) / alpha, ref temp, precision);
      return integrand;
    }

    #endregion Integral Agt1Gn

    #region CDF Integral

    private static double CDFIntegralForPositiveXAlt1(double x, double alpha, double gamma, double aga, ref object? temp, double precision, out bool inverseRuleUsed)
    {
      double result;
      if (gamma <= 0)
      {
        GetAlt1GnParameterByGamma(x, alpha, gamma, aga, out var factorp, out var facdiv, out var dev, out var logPdfPrefactor);
        var a = new Alt1GnI(factorp, facdiv, logPdfPrefactor, alpha, dev);
        result = a.CDFIntegrate(ref temp, precision);
        inverseRuleUsed = false;
      }
      else
      {
        GetAlt1GpParameterByGamma(x, alpha, gamma, aga, out var factorp, out var facdiv, out var dev, out var logPdfPrefactor);
        var a = new Alt1GpI(factorp, facdiv, logPdfPrefactor, alpha, dev);
        result = a.CDFIntegrate(ref temp, precision);
        inverseRuleUsed = false;
      }
      return result;
    }

    private static double CDFIntegralForPositiveXAgt1(double x, double alpha, double gamma, double aga, ref object? temp, double precision)
    {
      GetAgt1GnParameterByGamma(x, alpha, gamma, aga, out var factorp, out var factorw, out var dev, out var logPdfPrefactor);
      var a = new Agt1GnI(factorp, factorw, logPdfPrefactor, alpha, dev);
      return a.CDFIntegrate(ref temp, precision);
    }

    #endregion CDF Integral

    #endregion Integration
  }
}
