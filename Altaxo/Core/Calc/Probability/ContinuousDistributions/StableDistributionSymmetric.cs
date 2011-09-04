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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Represents a symmetric stable distribution in Zolotarev's parametrization.
  /// </summary>
  /// <remarks>
  /// <para>References:</para> 
  /// <para>[1] Matsui M., Takemura A.: "Some Improvements in Numerical Evaluation of Symmetric Stable Densities and its Derivatives", Discussion Paper, CIRJE-F-292, Tokio, August 2004</para>
  ///</remarks>
  public class StableDistributionSymmetric : StableDistributionBase
  {
    public StableDistributionSymmetric(Generator gen)
      : base(gen)
    {
    }

    #region Distribution implementation
    #endregion

    #region PDF

    public static double PDF(double x, double alpha)
    {
      object store = null;
      return PDF(x, alpha, ref store, Math.Sqrt(DoubleConstants.DBL_EPSILON));
    }
    
    /// <summary>
    /// Calculates the probability density using either series expansion for small or big arguments, or a integration
    /// in the intermediate range.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static double PDF(double x, double alpha, ref object tempStorage, double precision)
    {
      if (!(alpha > 0))
        throw new ArgumentException(string.Format("Parameter alpha must be >0, but is: {0}", alpha));
      if (!(alpha <= 2))
        throw new ArgumentException(string.Format("Parameter alpha must be <=2, but is: {0}", alpha));

      if (alpha <= 1)
      {
        if (alpha <= 0.2)
        {
          if (alpha < 0.1)
            throw new NotImplementedException("No support for values of alpha smaller than 0.1 in the moment");
          else
            return PDFAlphaBetween01And02(x, alpha, precision, ref tempStorage);
        }
        else // alpha >0.2
        {
          if (alpha <= 0.99)
            return PDFAlphaBetween02And099(x, alpha, precision, ref tempStorage);
          else
            return PDFAlphaBetween099And101(x, alpha, precision, ref tempStorage);
        }
      }
      else // alpha>1
      {
        if (alpha <= 1.01)
          return PDFAlphaBetween099And101(x, alpha, precision, ref tempStorage);
        else if (alpha <= 1.99995)
          return PDFAlphaBetween101And199999(x, alpha, precision, ref tempStorage);
        else
          return PDFAlphaBetween199999And2(x, alpha, precision, ref tempStorage);
      }
    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 0.1 and 0.2. For small x (1E-16), the accuracy at alpha=0.1 is only 1E-7.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween01And02(double x, double alpha, double precision, ref object tempStorage)
    {
      x = Math.Abs(x);

      double a15 = alpha * Math.Sqrt(alpha);
      double smallestexp = -9 + 0.217147240951625 * ((-1.92074130618617 / a15 + 1.35936488329912 * a15));


      //  double smallestexp = 80 * alpha - 24; // Exponent is -16 for alpha=0.1 and -8 for alpha=0.2
      double lgx = Math.Log10(x);

      if (lgx <= smallestexp)
        return GammaRelated.Gamma(1 / alpha) / (alpha * Math.PI);
      else if (lgx > 3 / (1 + alpha))
        return PDFSeriesBigX(x, alpha);
      else
        return PDFIntegration(x, alpha, precision, ref tempStorage);

    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 0.2 and 0.99. For small x (1E-8), the accuracy at alpha=0.2 is only 1E-7.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween02And099(double x, double alpha, double precision, ref object tempStorage)
    {
      x = Math.Abs(x);

      double smallestexp;
      if (alpha <= 0.3)
        smallestexp = 30 * alpha - 14; // Exponent is -8 for alpha=0.2 and -5 for alpha=0.3
      else if (alpha <= 0.6)
        smallestexp = 10 * alpha - 8; // Exponent is -5 for alpha=0.3 and -2 for alpha=0.6
      else
        smallestexp = 2.5 * alpha - 3.5; // Exponent is -2 for alpha=0.6 and -1 for alpha=1

      double lgx = Math.Log10(x);
      if (lgx <= smallestexp)
        return PDFSeriesSmallX(x, alpha);
      else if (lgx > 3 / (1 + alpha))
        return PDFSeriesBigX(x, alpha);
      else
        return PDFIntegration(x, alpha, precision, ref tempStorage);
    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 0.99 and 1.01.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween099And101(double x, double alpha, double precision, ref object tempStorage)
    {
      if (alpha == 1)
        return 1 / (Math.PI * (1 + x * x));

      x = Math.Abs(x);
      if (x <= 0.1)
        return PDFSeriesSmallX(x, alpha);
      else if (x >= 10)
        return PDFSeriesBigX(x, alpha);
      else
        return PDFTaylorExpansionAroundAlphaOne(x, alpha);
    }


    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 0.2 and 0.99. For small x (1E-8), the accuracy at alpha=0.2 is only 1E-7.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween101And199999(double x, double alpha, double precision, ref object tempStorage)
    {
      x = Math.Abs(x);


      if (x <= 1E-2)
        return PDFSeriesSmallX(x, alpha);
      else if (x >= 100)
        return PDFSeriesBigX(x, alpha);
      else
        return PDFIntegration(x, alpha, precision, ref tempStorage);
    }


    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 1.99999 and 2. For small x ( max 7), the asymptotic expansion is used.
    /// For big x, the maximum value resulting from direct integration and series expansion w.r.t. alpha is used.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween199999And2(double x, double alpha, double precision, ref object tempStorage)
    {
      if (alpha == 2)
        return Math.Exp(-0.25 * x * x) / (2 * Math.Sqrt(Math.PI));

      x = Math.Abs(x);
      if (x <= 4)
        return PDFSeriesSmallX(x, alpha);
      else
      {
        double r1 = PDFIntegration(x, alpha, precision, ref tempStorage);
        double r2 = Math.Exp(-0.25 * x * x) / (2 * Math.Sqrt(Math.PI)); // Value at alpha=2
        r2 += 0.5 * GammaRelated.Gamma(alpha + 1) * Math.Pow(x, -alpha - 1) * (2 - alpha); // first term of first derivative of series expansion
        r2 += 0.5 * GammaRelated.Gamma(2 * alpha + 1) * Math.Pow(x, -2 * alpha - 1) * (2 - alpha); // second term of first derivative of series expansion
        r2 += 0.25 * GammaRelated.Gamma(3 * alpha + 1) * Math.Pow(x, -3 * alpha - 1) * (2 - alpha); // third term of first derivative of series expansion
        if (double.IsNaN(r1))
          return r2;
        else
          return Math.Max(r1, r2);
      }
    }
    #region Series expansion for small x

    /// <summary>
    /// Imaginary part of the Fourier transformed derivative of the Kohlrausch function for low frequencies.
    /// </summary>
    /// <param name="alpha">Beta parameter.</param>
    /// <param name="z">Circular frequency.</param>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
    /// <remarks>This is the imaginary part of the Fourier transform (in Mathematica notation): Im[Integrate[D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. The sign of
    /// the return value here is positive!.</remarks>
    public static double PDFSeriesSmallX(double z, double alpha)
    {
      int k = 1;
      double z_pow_2k = 1;
      double z_square = z * z;
      double k2m2fac = 1;

      double term1 = GammaRelated.Gamma((k + k - 1) / alpha) * z_pow_2k / k2m2fac;

      if (z_square == 0)
        return term1 / (Math.PI * alpha); // if z was so small that z_square can not be evaluated, return after term1

      k = 2;
      k2m2fac = 2;
      z_pow_2k *= z_square;
      double term2 = GammaRelated.Gamma((k + k - 1) / alpha) * z_pow_2k / k2m2fac;

      double curr = term1 - term2;
      double sum = curr;
      double prev = curr;

      for (; ; )
      {
        k2m2fac *= (k + k) * (k + k - 1);
        ++k;
        z_pow_2k *= z_square;
        term1 = GammaRelated.Gamma((k + k - 1) / alpha) * z_pow_2k / k2m2fac;

        k2m2fac *= (k + k) * (k + k - 1);
        ++k;
        z_pow_2k *= z_square;
        term2 = GammaRelated.Gamma((k + k - 1) / alpha) * z_pow_2k / k2m2fac;

        curr = term1 - term2;

        if (Math.Abs(curr) < Math.Abs(sum * 1E-15))
          break;
        if ((Math.Abs(curr) > Math.Abs(prev)) && (Math.Abs(term2)>Math.Abs(term1))) // sum is not converging
        {
          sum = double.NaN;
          break;
        }
        if (double.IsInfinity(k2m2fac))
        {
          sum = double.NaN;
          break;
        }

        prev = curr;
        sum += curr;
      }
      return sum / (Math.PI * alpha);
    }

    /// <summary>
    /// Imaginary part of the Fourier transformed derivative of the Kohlrausch function for low frequencies, and beta&lt;=1/20..
    /// </summary>
    /// <param name="beta">Beta parameter.</param>
    /// <param name="z">Circular frequency.</param>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for low frequencies, or double.NaN if the series not converges.</returns>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
    /// <remarks>This is the imaginary part of the Fourier transform (in Mathematica notation): Im[Integrate[D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. The sign of
    /// the return value here is positive!.</remarks>
    public static double PDFSeriesSmallXSmallAlpha(double z, double alpha)
    {


      int k = 1;
      double ln_z_pow_2km1 = k * Math.Log(z);
      double ln_z_square = 2 * Math.Log(z);
      double k2m2fac = 1;

      double curr = GammaRelated.LnGamma((k + k - 1) / alpha) + ln_z_pow_2km1 - Math.Log(k2m2fac);
      double ln_scaling = curr;
      double sum = 1;

      double prev = curr;

      for (; ; )
      {
        k2m2fac *= (k + k) * (k + k - 1);
        ++k;
        ln_z_pow_2km1 += ln_z_square;
        curr = GammaRelated.LnGamma((k + k - 1) / alpha) + ln_z_pow_2km1 - Math.Log(k2m2fac);

        if (curr < (prev - 34.5))
          break;
        if (curr >= prev)
        {
          sum = double.NaN;
          break;
        }
        if (double.IsInfinity(k2m2fac))
        {
          sum = double.NaN;
          break;
        }

        if ((k & 1) == 0)
          sum -= Math.Exp(curr - ln_scaling);
        else
          sum += Math.Exp(curr - ln_scaling);

        prev = curr;
      }
      return (sum / (alpha * Math.PI * z)) * Math.Exp(ln_scaling);
    }

    #endregion

    #region Series expansion for big x

    /// <summary>
    /// Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies.
    /// </summary>
    /// <param name="beta">Beta parameter.</param>
    /// <param name="z">Circular frequency.</param>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
    /// <remarks>This is the imaginary part of the Fourier transform (in Mathematica notation): Im[Integrate[D[Exp[-t^beta],t]*Exp[-I w t],{t, 0, Infinity}]]. The sign of
    /// the return value here is positive!.</remarks>
    public static double PDFSeriesBigX(double z, double alpha)
    {
      int k = 1;
      double z_pow_minusAlpha = Math.Pow(z, -alpha);
      double kfac = 1;
      double z_pow_minusKAlpha = z_pow_minusAlpha;


      double term1a = GammaRelated.Gamma(1 + k * alpha) * z_pow_minusKAlpha / kfac;
      double term1 = term1a * Math.Sin(Math.PI * alpha * k * 0.5);

      double sum = term1;

      for (; ; )
      {

        ++k;
        z_pow_minusKAlpha *= z_pow_minusAlpha;
        kfac *= -k;

        double term2a = GammaRelated.Gamma(1 + k * alpha) * z_pow_minusKAlpha / kfac;
        double term2 = term2a * Math.Sin(Math.PI * alpha * k * 0.5);

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

    #endregion

    #region Direct integration


    public static void GetAlt1GnParameter(double x, double alpha, 
                        out double factorp, out double facdiv, out double dev, out double logPdfPrefactor)
    {
      dev = Math.PI * 0.5;
      facdiv = 1; 
      factorp = x;
      logPdfPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * x));
    }

    public static void GetAlt1GpParameterByGamma(double x, double alpha,
                        out double factorp, out double facdiv, out double dev, out double logPdfPrefactor)
    {
      dev = Math.PI * 0.5;
      facdiv = 1;
      factorp = x;
      logPdfPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * x));
    }


    public static void GetAgt1GnParameter(double x, double alpha, 
                        out double factorp, out double factorw, out double dev, out double logPrefactor)
    {
      dev = Math.PI * (0.5 * ((2 - alpha)));
      factorp = x;
      factorw = x;
      logPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * x));
    }

    public static double PDFIntegration(double x, double alpha, double precision, ref object tempStorage)
    {
      if (alpha < 1)
      {
        double factorp, facdiv, dev, logPdfPrefactor;
        GetAlt1GnParameter(x,alpha,out factorp, out facdiv, out dev, out logPdfPrefactor);
        Alt1GnI intg = new Alt1GnI(factorp,facdiv,logPdfPrefactor,alpha,dev);
        if (intg.IsMaximumLeftHandSide())
          return intg.PDFIntegrate(ref tempStorage, precision);
        else
          return new Alt1GnD(factorp,facdiv,logPdfPrefactor,alpha,dev).Integrate(ref tempStorage,precision);
      }
      else
      {
        double factorp, factorw, dev, logPdfPrefactor;
        GetAgt1GnParameter(x, alpha, out factorp, out factorw, out dev, out logPdfPrefactor);
        Agt1GnI intg = new Agt1GnI(factorp, factorw, logPdfPrefactor, alpha, dev);
        if(intg.IsMaximumLeftHandSide())
          return intg.Integrate(ref tempStorage, precision);
        else
          return new Agt1GnD(factorp, factorw, logPdfPrefactor, alpha, dev).Integrate(ref tempStorage, precision);
      }
    }

    #endregion

    #region Taylor expansion around a=1

    public static double PDFTaylorExpansionAroundAlphaOne(double x, double alpha)
    {
      const double EulerGamma = 0.57721566490153286061;
      double result;
      double OnePlusXX = 1 + x * x;
      double XXMinusOne = x * x - 1;

      if (double.IsInfinity(OnePlusXX))
        return 0;


      // Zero order
      result = 1 / OnePlusXX;



      double d;
      // First order, we try to take care that the power of x will be not too high in order to calculate
      // good values even for high x values
      d = (XXMinusOne / OnePlusXX) * (1 - EulerGamma - 0.5 * Math.Log(OnePlusXX)) + 2 * x * Math.Atan(x) / OnePlusXX;
      d /= OnePlusXX;
      result += (alpha - 1) * d;

      // Second order
      d = ((XXMinusOne - 2 * x) / OnePlusXX) * ((XXMinusOne + 2 * x) / OnePlusXX) / OnePlusXX;
      d *= RMath.Pow2(Math.PI) / 6 + RMath.Pow2(1 - EulerGamma - 0.5 * Math.Log(OnePlusXX)) - 1 - RMath.Pow2(Math.Atan(x));
      d += (XXMinusOne / OnePlusXX) * ((8 * x) / OnePlusXX) / OnePlusXX * Math.Atan(x) * (1.5 - EulerGamma - 0.5 * Math.Log(OnePlusXX));
      d += 2 * ((((1 - 3 * x * x) * (1 - EulerGamma - 0.5 * Math.Log(OnePlusXX)) - x * OnePlusXX * Math.Atan(x)) / OnePlusXX) / OnePlusXX) / OnePlusXX;
      result += 0.5 * RMath.Pow2(alpha - 1) * d;

      // Third order


      return result / Math.PI;
    }
   

    #endregion

    #endregion

    #region CDF

    public static double CDF(double x, double alpha)
    {
      object tempStorage = null;
      return CDF(x, alpha, ref tempStorage, DefaultPrecision);
    }

    public static double CDF(double x, double alpha, ref object tempStorage, double precision)
    {
      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));

      double integFromXZero, integFromXInfinity;
     if (IsXNearlyEqualToZero(x))
        {
          return 0.5;
        }
        else if (x > 0)
        {
          CDFMethodForPositiveX(x, alpha, ref tempStorage, precision, out integFromXZero, out integFromXInfinity);
          return 0.5 + integFromXZero;
        }
        else // x<0
        {
          CDFMethodForPositiveX(-x, alpha, ref tempStorage, precision, out integFromXZero, out integFromXInfinity);
          return integFromXInfinity;
        }
    }

    public static double CCDF(double x, double alpha)
    {
      object tempStorage = null;
      return CCDF(x, alpha, ref tempStorage, DefaultPrecision);
    }

    public static double CCDF(double x, double alpha, ref object tempStorage, double precision)
    {
      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));

      double integFromXZero, integFromXInfinity;
      if (IsXNearlyEqualToZero(x))
      {
        return 0.5;
      }
      else if (x > 0)
      {
        CDFMethodForPositiveX(x, alpha, ref tempStorage, precision, out integFromXZero, out integFromXInfinity);
        return integFromXInfinity;
      }
      else // x<0
      {
        CDFMethodForPositiveX(-x, alpha, ref tempStorage, precision, out integFromXZero, out integFromXInfinity);
        return 0.5 + integFromXZero;
      }
    }


    public static double XZCDF(double x, double alpha)
    {
      object tempStorage = null;
      return XZCDF(x, alpha, ref tempStorage, DefaultPrecision);
    }

    public static double XZCDF(double x, double alpha, ref object tempStorage, double precision)
    {
      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));

      double integFromXZero, integFromXInfinity;
      if (x==0)
      {
        return 0;
      }
      else if (x > 0)
      {
        CDFMethodForPositiveX(x, alpha, ref tempStorage, precision, out integFromXZero, out integFromXInfinity);
        return integFromXZero;
      }
      else // x<0
      {
        CDFMethodForPositiveX(-x, alpha, ref tempStorage, precision, out integFromXZero, out integFromXInfinity);
        return -integFromXZero;
      }
    }

    private static bool IsXNearlyEqualToZero(double x)
    {
      return Math.Abs(x) < DoubleConstants.DBL_EPSILON;
    }

    private static void CDFMethodForPositiveX(double x, double alpha, ref object tempStorage, double precision, out double integFromXZero, out double integFromXInfinity)
    {
      const double offs = 0.5;

      if (alpha < 1)
      {
        double factorp, facdiv, dev, logPdfPrefactor;
        GetAlt1GnParameter(x, alpha, out factorp, out facdiv, out dev, out logPdfPrefactor);
        Alt1GnI inc = new Alt1GnI(factorp, facdiv, logPdfPrefactor, alpha, dev);
        if (inc.IsMaximumLeftHandSide())
        {
          integFromXZero = inc.CDFIntegrate(ref tempStorage, precision) / Math.PI;
          integFromXInfinity = offs - integFromXZero;
        }
        else
        {
          integFromXInfinity = new Alt1GnD(factorp, facdiv, logPdfPrefactor, alpha, dev).CDFIntegrate(ref tempStorage, precision)/Math.PI;
          integFromXZero = offs - integFromXInfinity;
        }
      }
      else if(alpha==1)
      {
        if (x <= 1)
        {
          integFromXZero = Math.Atan(x) / Math.PI;
          integFromXInfinity = offs - integFromXZero;
        }
        else
        {
          integFromXInfinity = Math.Atan(1 / x) / Math.PI;
          integFromXZero = offs - integFromXInfinity;
        }
      }
      else // if(alpha>1)
      {
        double factorp, factorw, dev, logPdfPrefactor;
        GetAgt1GnParameter(x, alpha, out factorp, out factorw, out dev, out logPdfPrefactor);
        Agt1GnI inc = new Agt1GnI(factorp, factorw, logPdfPrefactor, alpha, dev);
        if (inc.IsMaximumLeftHandSide())
        {
          integFromXInfinity = inc.CDFIntegrate(ref tempStorage, precision)/Math.PI;
          integFromXZero = offs - integFromXInfinity;
        }
        else
        {
          integFromXZero= new Agt1GnD(factorp, factorw, logPdfPrefactor, alpha, dev).CDFIntegrate(ref tempStorage, precision)/Math.PI;
          integFromXInfinity = offs - integFromXZero;
        }
      }
    }


   

   


    #endregion

    #region Quantile

    public static double Quantile(double p, double alpha)
    {
      if (p == 0.5)
        return 0;

      double xguess = Math.Exp(2 / alpha); // guess value for a nearly constant p value in dependence of alpha
      double x0,x1;

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

      object tempStorage = null;
      if (RootFinding.BracketRootByExtensionOnly(delegate(double x) { return CDF(x, alpha, ref tempStorage, DefaultPrecision) - p; }, 0, ref x0, ref x1))
      {
        double root;
        if (null == RootFinding.ByBrentsAlgorithm(delegate(double x) { return CDF(x, alpha, ref tempStorage, DefaultPrecision) - p; }, x0, x1, 0, DoubleConstants.DBL_EPSILON, out root))
          return root;
      }
      return double.NaN;
    }

    public static double QuantileCCDF(double q, double alpha)
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

      object tempStorage = null;
      if (RootFinding.BracketRootByExtensionOnly(delegate(double x) { return CCDF(x, alpha, ref tempStorage, DefaultPrecision) - q; }, 0, ref x0, ref x1))
      {
        double root;
        if (null == RootFinding.ByBrentsAlgorithm(delegate(double x) { return CCDF(x, alpha, ref tempStorage, DefaultPrecision) - q; }, x0, x1, 0, DoubleConstants.DBL_EPSILON, out root))
          return root;
      }
      return double.NaN;
    }
    #endregion

  }
}
