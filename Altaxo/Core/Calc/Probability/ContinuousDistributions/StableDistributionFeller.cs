using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Probability
{

  public class StableDistributionFeller : StableDistributionBase
  {
    public StableDistributionFeller(Generator gen)
      : base(gen)
    {
    }


    #region PDF dispatcher

    public static double PDF(double x, double alpha, double gamma)
    {
      object store = null;
      return PDF(x, alpha, gamma, ref store, Math.Sqrt(DoubleConstants.DBL_EPSILON));
    }

    public static double PDF(double x, double alpha, double gamma, double aga)
    {
      object store = null;
      return PDF(x, alpha, gamma, aga, ref store, Math.Sqrt(DoubleConstants.DBL_EPSILON));
    }

    /// <summary>
    /// Calculates the probability density using either series expansion for small or big arguments, or a integration
    /// in the intermediate range.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static double PDF(double x, double alpha, double gamma, ref object tempStorage, double precision)
    {
      double aga = GetAgaFromAlphaGamma(alpha, gamma);
      return PDF(x, alpha, gamma, aga, ref tempStorage, precision);
    }

    /// <summary>
    /// Calculates the probability density using either series expansion for small or big arguments, or a integration
    /// in the intermediate range.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static double PDF(double x, double alpha, double gamma, double aga, ref object tempStorage, double precision)
    {
      if (x > 0)
      {
        return PDFforPositiveX(x, alpha, gamma, aga, ref tempStorage, precision);
      }
      else if (x < 0)
      {
        return PDFforPositiveX(-x, alpha, -gamma, aga, ref tempStorage, precision);
      }
      else if (x == 0)
      {
        return PDFforXZero(alpha, gamma, aga);
      }
      else
        return double.NaN;
    }

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
        if (result > 0.5)
          result = 0.5;
      }
      else // alpha >1
      {
        if (gamma >= (alpha-1))
          result = alpha - gamma;
        else
          result = (2 - alpha) + gamma;

        if (result < 0)
          result = 0;
        if (result > 1)
          result = 1;
      }

      return result;
    }

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
          result = aga + alpha - 2;
        else
          result = alpha - aga;
      }
      return result;
    }

    /// <summary>
    /// Calculates the probability density using either series expansion for small or big arguments, or a integration
    /// in the intermediate range.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="alpha"></param>
    /// <param name="gamma">The gamma value.</param>
    /// <param name="aga">For alpha &lt;=1: This is either (alpha-gamma)/alpha for gamma &gt;=0, or (alpha+gamma)/alpha for gamma &lt; 1.
    /// For alpha &gt;1, this is either (alpha-gamma) for gamma &gt; (alpha-1), or (2-alpha+gamma) for gamma &lt; (alpha-1).</param>
    /// <returns></returns>
    public static double PDFforPositiveX(double x, double alpha, double gamma, double aga, ref object tempStorage, double precision)
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
        if(gamma>alpha)
          throw new ArgumentException(string.Format("Value of parameter gamma must be <={0}, but is {1}", alpha, gamma));
        else if (gamma<alpha-2)
          throw new ArgumentException(string.Format("Value of parameter gamma must be >={0}, but is {1}", alpha-2, gamma));
          
      }

      if (alpha <= 1)
      {
        // special case alpha==gamma
        if (gamma>0 && aga==0)
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

    public static double PDFforXZero(double alpha, double gamma, double aga)
    {
      // use different methods, which provide the best accuracy for the case
      double result;
      if (alpha <= 1)
      {
        if(Math.Abs(gamma+gamma)<alpha)
         result= GammaRelated.Gamma(1 / alpha) / (alpha * Math.PI) * Math.Cos(0.5 * Math.PI*gamma/alpha);
        else
          result= GammaRelated.Gamma(1 / alpha) / (alpha * Math.PI) * Math.Sin( aga * 0.5 * Math.PI);
      }
      else // alpha>1
      {
        if (Math.Abs(gamma + gamma) < alpha)
          result = GammaRelated.Gamma(1 / alpha) / (alpha * Math.PI) * Math.Cos(0.5 * Math.PI * gamma/alpha);
        else if (gamma>=(alpha-1))
          result = GammaRelated.Gamma(1 / alpha) / (alpha * Math.PI) * Math.Sin(0.5 * Math.PI * aga / alpha);
        else
          result = GammaRelated.Gamma(1 / alpha) / (alpha * Math.PI) * Math.Sin(0.5 * Math.PI * (alpha-gamma) / alpha);
      }
      return result;
    }

    static double GetLog10BoundaryForOneTermOfSeriesExpansionSmall(double alpha, double precision)
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
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween0And01(double x, double alpha, double gamma, double aga, ref object tempStorage, double precision)
    {
      double smallestexp = GetLog10BoundaryForOneTermOfSeriesExpansionSmall(alpha, DoubleConstants.DBL_EPSILON);
      double lgx = Math.Log10(x);

      if (lgx <= smallestexp && (aga!=0 || gamma>=0))
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
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween01And02(double x, double alpha, double gamma, double aga, ref object tempStorage, double precision)
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
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween02And099(double x, double alpha, double gamma, double aga, ref object tempStorage, double precision)
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
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween099And101(double x, double alpha, double gamma, double aga, ref object tempStorage, double precision)
    {
      if (x <= 0.1)
        return PDFSeriesSmallXFeller(x, alpha, gamma, aga);
      else if (x >= 10)
        return PDFSeriesBigXFeller(x, alpha, gamma, aga);
      else
        return PDFTaylorExpansionAroundAlphaOne(x, alpha, gamma, aga);
    }


    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 1.01 and 1.99999. 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween101And199999(double x, double alpha, double gamma, double aga, ref object tempStorage, double precision)
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
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween199999And2(double x, double alpha, double gamma, double aga, ref object tempStorage, double precision)
    {
      if (alpha == 2)
        return Math.Exp(-0.25 * x * x) / (2 * Math.Sqrt(Math.PI)); // because gamma must be zero for alpha==2

      if (x <= 7)
        return PDFSeriesSmallXFeller(x, alpha, gamma, aga);
      else
        // we use here the inverse formula
        return PDFFromAlphaInversionFormula(x, alpha, gamma, ref tempStorage, precision);
    }

    #endregion

    public static void ConversionFellerToInverseFeller(double x, double alpha, double gamma,
      out double xinv, out double alphainv, out double gammainv, out double scaling)
    {
      xinv = Math.Pow(x, -alpha);
      alphainv = 1 / alpha;
      gammainv = (gamma - alpha + 1) / alpha;
      scaling = xinv / x;
    }

    public static void ConversionFellerToInverseFeller(double x, double alpha, double gamma, double aga,
     out double xinv, out double alphainv, out double gammainv, out double againv, out double scaling)
    {
      xinv = Math.Pow(x, -alpha);
      alphainv = 1 / alpha;
      gammainv = (gamma - alpha + 1) / alpha;
      againv = aga;
      scaling = xinv / x;
    }


    public static double PDFFromAlphaInversionFormula(double x, double alpha, double gamma, ref object tempStorage, double precision)
    {
      double xinv = Math.Pow(x, -alpha);
      return PDF(xinv, 1 / alpha, (gamma - alpha + 1) / alpha, ref tempStorage, precision) * xinv / x;
    }
    public static double PDFFromAlphaInversionFormula(double x, double alpha, double gamma, double aga, ref object tempStorage, double precision)
    {
      if (x < 0)
      {
        x = -x;
        gamma = -gamma;
        aga = GetAgaFromAlphaGamma(alpha, gamma);
      }
      double xinv = Math.Pow(x, -alpha);
      return PDF(xinv, 1 / alpha, (gamma - alpha + 1) / alpha, aga, ref tempStorage, precision) * xinv / x;
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
    public static double PDFSeriesVerySmallXFeller(double z, double alpha, double gamma)
    {
      int k = 1;
      double mz_pow_k = -1;
      double z_square = z * z;
      double kfac = 1;
      double piga_2a = 0.5 * Math.PI * (gamma - alpha) / alpha;


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

      for (; k < 4; )
      {
        ++k;
        kfac *= k;
        mz_pow_k *= -z;
        term1 = GammaRelated.Gamma(1 + k / alpha) * mz_pow_k / kfac * Math.Sin(k * piga_2a);

        ++k;
        kfac *= k;
        mz_pow_k *= -z;
        term2 = GammaRelated.Gamma(1 + k / alpha) * mz_pow_k / kfac * Math.Sin(k * piga_2a); ;

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

    /// <summary>
    /// Imaginary part of the Fourier transformed derivative of the Kohlrausch function for low frequencies.
    /// </summary>
    /// <param name="alpha">Beta parameter.</param>
    /// <param name="z">Circular frequency.</param>
    /// <returns>Imaginary part of the Fourier transformed derivative of the Kohlrausch function for high frequencies, or double.NaN if the series not converges.</returns>
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
      else
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
        term2 = GammaRelated.Gamma(1 + k / alpha) * mz_pow_k / kfac * Math.Sin(k * piga_2a); ;

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



    #endregion

    #region Series expansion for big x
    /// <summary>
    /// Series expansion for big x, Feller parametrization. x should be &gt; 0.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
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
      else if (alpha > 1 && gamma < (alpha-1)) // gamma-alpha is in this case -2+aga
      {
        // pi_gamma_alpha_2 = Math.PI * (-1 + 0.5*aga); // this would be the original calculation, but we replace it by sin(k*(-Pi+x))== (-1)^k * sin(k*x)
        pi_gamma_alpha_2 = Math.PI * 0.5 * aga;
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
    #endregion

    #region Taylor series around alpha=1

    public static double PDFTaylorExpansionAroundAlphaOne(double x, double alpha, double gamma, double aga)
    {
      const double EulerGamma = 0.57721566490153286060651209008240243;
      const double EulerGamma_P2 = EulerGamma * EulerGamma;
      const double EulerGamma_P3 = EulerGamma_P2 * EulerGamma;
      const double Zeta3 = 1.202056903159594285399738; // Zeta[3]
      const double SQR_PI = Math.PI * Math.PI;

      // Note: it is much much easier to calculate with Complex numbers than to try to take only the real part
      Complex expIgPi2 = ComplexMath.Exp(Complex.I * (Math.PI * gamma * 0.5));
      Complex log_expix = ComplexMath.Log(expIgPi2 + Complex.I * x);

      Complex term0, term1, term2, term3;

      // Note: the termp exp_i_pi_g/2 is common to all terms, we use it later on 

      term0 = 1 / (expIgPi2 + Complex.I * x);

      term1 = (expIgPi2 * (-1 + EulerGamma + log_expix)) / ComplexMath.Pow(expIgPi2 + Complex.I * x, 2);

      term2 = (expIgPi2 * (expIgPi2 * (12 + 6 * (-4 + EulerGamma) * EulerGamma + SQR_PI) - Complex.I * (6 * (-2 + EulerGamma) * EulerGamma + SQR_PI) * x +
       6 * log_expix * (2 * (-2 + EulerGamma) * expIgPi2 - Complex.FromRealImaginary(0, 2) * (-1 + EulerGamma) * x +
          (expIgPi2 - Complex.I * x) * log_expix))) / (12 * ComplexMath.Pow(expIgPi2 + Complex.I * x, 3));

      term3 = (expIgPi2 * (((expIgPi2 * expIgPi2) * (36 + 6 * (-6 + EulerGamma) * EulerGamma + SQR_PI) -
          Complex.FromRealImaginary(0, 4) * expIgPi2 * (9 + 3 * EulerGamma * (-7 + 2 * EulerGamma) + SQR_PI) * x - (6 * (-2 + EulerGamma) * EulerGamma + SQR_PI) * RMath.Pow2(x)) *
        log_expix + 6 * ((-3 + EulerGamma) * ComplexMath.Pow2(expIgPi2) - Complex.I * (-7 + 4 * EulerGamma) * expIgPi2 * x -
          (-1 + EulerGamma) * RMath.Pow2(x)) * ComplexMath.Pow2(log_expix) +
       2 * (ComplexMath.Pow2(expIgPi2) - Complex.FromRealImaginary(0, 4) * expIgPi2 * x - RMath.Pow2(x)) * ComplexMath.Pow3(log_expix) +
       RMath.Pow2(x) * (SQR_PI - EulerGamma * (2 * (-3 + EulerGamma) * EulerGamma + SQR_PI) - 4 * Zeta3) +
       ComplexMath.Pow2(expIgPi2) * (-3 * (4 + SQR_PI) + EulerGamma * (36 + 2 * (-9 + EulerGamma) * EulerGamma + SQR_PI) + 4 * Zeta3) -
       Complex.I * expIgPi2 * x * (-7 * SQR_PI + 2 * EulerGamma * (EulerGamma * (-21 + 4 * EulerGamma) + 2 * (9 + SQR_PI)) + 16 * Zeta3))) /
        (12 * ComplexMath.Pow(expIgPi2 + Complex.I * x, 4));

      double am1 = alpha - 1;
      Complex result = ((term3 * am1 + term2) * am1 + term1) * am1 + term0;
      result /= Math.PI;

      return result.Re;
    }



    #endregion

    #region Integration

    public static double PDFIntegral(double x, double alpha, double gamma, double aga, ref object temp, double precision)
    {
      const double OneMinusEps = 1 - 4 * DoubleConstants.DBL_EPSILON;

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

    #region Integral Alt1Gn

    public static void GetAlt1GnParameterByGamma(double x, double alpha, double gamma, double aga,
      out double factorp, out double facdiv, out double dev, out double prefactor)
    {
      double zeta = Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = Math.Pow(1 + zeta * zeta, 0.5 / alpha);
      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral

      // take alphaPlusGammaByAlpha, a value between 0 and 2 (for alpha<1)
      double alphaPlusGammaBy2Alpha;
      if(gamma<0)
        alphaPlusGammaBy2Alpha = 0.5*aga;
      else
        alphaPlusGammaBy2Alpha = 1-0.5*aga;

      dev = Math.PI * alphaPlusGammaBy2Alpha;
      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      facdiv = Math.Cos(-gamma * 0.5 * Math.PI); // Inverse part of the original factor without power
      factorp = xx * facdiv; // part of the factor with power alpha/(alpha-1);

      prefactor = sigmaf * alpha / (Math.PI * Math.Abs(alpha - 1) * (xx));
    }

    /// <summary>
    /// Special case for alpha &lt; 1 and gamma near to -alpha (i.e. gamma at the negative border).
    /// Here gamma was set to -alpha*(1-dev*2/Pi).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAlt1Gn(double x, double alpha, double gamma, double aga, ref object temp, double precision)
    {
      double factorp, facdiv, dev, prefactor;
      GetAlt1GnParameterByGamma(x, alpha, gamma, aga, out factorp, out facdiv, out dev, out prefactor);

      double integrand;
      Alt1GnI ingI = new Alt1GnI(factorp, facdiv, prefactor, alpha, dev);
      if (ingI.IsMaximumLeftHandSide())
      {
        integrand = ingI.Integrate(ref temp, precision);
        if (double.IsNaN(integrand))
          integrand = new Alt1GnD(factorp, facdiv, prefactor, alpha, dev).Integrate(ref temp, precision);
      }
      else
      {
        integrand = new Alt1GnD(factorp, facdiv, prefactor, alpha, dev).Integrate(ref temp, precision);
        if (double.IsNaN(integrand))
          integrand = ingI.Integrate(ref temp, precision);
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
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAlt1GnI(double x, double alpha, double gamma, double aga, ref object temp, double precision)
    {
      double factorp, facdiv, dev, prefactor;
      GetAlt1GnParameterByGamma(x, alpha, gamma, aga, out factorp, out facdiv, out dev, out prefactor);

      Alt1GnI ing = new Alt1GnI(factorp, facdiv, prefactor, alpha, dev);
      double integrand = ing.Integrate(ref temp, precision);
      return integrand;
    }

    /// <summary>
    /// Special case for alpha &lt; 1 and gamma near to -alpha (i.e. gamma at the negative border).
    /// Here gamma was set to -alpha*(1-dev*2/Pi). The core function is decreasing.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAlt1GnD(double x, double alpha, double gamma, double aga, ref object temp, double precision)
    {
      double factorp, facdiv, dev, prefactor;
      GetAlt1GnParameterByGamma(x, alpha, gamma, aga, out factorp, out facdiv, out dev, out prefactor);

      double integrand = new Alt1GnD(factorp, facdiv, prefactor, alpha, dev).Integrate(ref temp, precision);
      return integrand;
    }

    #endregion

    #region Integral Alt1Gp

    public static void GetAlt1GpParameterByGamma(double x, double alpha, double gamma, double aga,
    out double factorp, out double facdiv, out double dev, out double prefactor)
    {
      double zeta = Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = Math.Pow(1 + zeta * zeta, 0.5 / alpha);
      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral

      // take alphaMinusGammaByAlpha, a value between 0 and 2 (for alpha<1)
      double alphaMinusGammaBy2Alpha;
      if(gamma>=0)
        alphaMinusGammaBy2Alpha = 0.5*aga;
      else
        alphaMinusGammaBy2Alpha = 1-0.5*aga;

      dev = Math.PI * alphaMinusGammaBy2Alpha;
      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      facdiv = Math.Cos(-gamma * 0.5 * Math.PI); // Inverse part of the original factor without power
      factorp = xx * facdiv; // part of the factor with power alpha/(alpha-1);

      prefactor = alpha / (Math.PI * Math.Abs(alpha - 1) * x);
    }


    /// <summary>
    /// Special case for alpha &lt; 1 and gamma near to +alpha (i.e. gamma at the positive border).
    /// Here gamma was set to alpha*(1-dev*2/Pi). 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAlt1Gp(double x, double alpha, double gamma, double aga, ref object temp, double precision)
    {
      double factorp, facdiv, dev, prefactor;
      GetAlt1GpParameterByGamma(x, alpha, gamma, aga, out factorp, out facdiv, out dev, out prefactor);

      
      double integrand;
      Alt1GpI intI = new Alt1GpI(factorp, facdiv, prefactor, alpha, dev);
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
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAlt1GpI(double x, double alpha, double gamma, double aga, ref object temp, double precision)
    {
      double factorp, facdiv, dev, prefactor;
      GetAlt1GpParameterByGamma(x, alpha, gamma, aga, out factorp, out facdiv, out dev, out prefactor);

      Alt1GpI intI = new Alt1GpI(factorp, facdiv, prefactor, alpha, dev);
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
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAlt1GpD(double x, double alpha, double gamma, double aga, ref object temp, double precision)
    {
      double factorp, facdiv, dev, prefactor;
      GetAlt1GpParameterByGamma(x, alpha, gamma, aga, out factorp, out facdiv, out dev, out prefactor);

      Alt1GpD intD = new Alt1GpD(factorp, facdiv, prefactor, alpha, dev);
      double integrand = intD.Integrate(ref temp, precision); // IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAlt1GpD(factorp, facdiv, alpha, dev, theta); }, 0, dev, ref temp, precision);
      return integrand;
    }

    #endregion

    #region Integral Agt1Gn
   
    public static void GetAgt1GnParameterByGamma(double x, double alpha, double gamma, double aga,
   out double factorp, out double factorw, out double dev, out double prefactor)
    {
      double zeta = Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = Math.Pow(1 + zeta * zeta, 0.5 / alpha);
      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral

      double minusGammaByAlpha = -gamma / alpha;
      if (minusGammaByAlpha < -1)
        minusGammaByAlpha = -1;
      else if (minusGammaByAlpha > 1)
        minusGammaByAlpha = 1;

      if (gamma < (alpha-1))
        dev = Math.PI * 0.5 * aga;
      else
        dev = Math.PI * (0.5 * ((2 - alpha) + gamma));
      if (dev < 0)
        dev = 0;

      //double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      // we separate the factor in a power of 1/alpha-1 and the rest
      factorp = xx * Math.Cos(-gamma*0.5*Math.PI);
      factorw = xx;
     
      prefactor = alpha / (Math.PI * Math.Abs(alpha - 1) * x);
    }
    

    /// <summary>
    /// Special case for alpha>1 and gamma near to alpha-2 (i.e. gamma at the negative border).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAgt1Gn(double x, double alpha, double gamma, double aga, ref object temp, double precision)
    {
      double factorp, factorw, dev, prefactor;
      GetAgt1GnParameterByGamma(x, alpha, gamma, aga, out factorp, out factorw, out dev, out prefactor);

      Agt1GnI intI = new Agt1GnI(factorp, factorw, prefactor, alpha, dev);
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
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAgt1GnD(double x, double alpha, double gamma, double aga, ref object temp, double precision)
    {
      double factorp, factorw, dev, prefactor;
      GetAgt1GnParameterByGamma(x, alpha, gamma, aga, out factorp, out factorw, out dev, out prefactor);

      Agt1GnD intD = new Agt1GnD(factorp, factorw, prefactor, alpha, dev);
      double integrand = intD.Integrate(ref temp, precision); // IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAgt1GnD(factorp, factorw, alpha, dev, theta); }, 0, (Math.PI - dev) / alpha, ref temp, precision);
      return integrand;
    }


    /// <summary>
    /// Special case for alpha>1 and gamma near to alpha-2 (i.e. gamma at the negative border).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="gamma"></param>
    /// <param name="temp"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    public static double PDFIntegralAgt1GnI(double x, double alpha, double gamma, double aga, ref object temp, double precision)
    {
      double factorp, factorw, dev, prefactor;
      GetAgt1GnParameterByGamma(x, alpha, gamma, aga, out factorp, out factorw, out dev, out prefactor);

      Agt1GnI intI = new Agt1GnI(factorp, factorw, prefactor, alpha, dev);
      double integrand = intI.Integrate(ref temp, precision); // IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAgt1GnI(factorp, factorw, alpha, dev, theta); }, 0, (Math.PI - dev) / alpha, ref temp, precision);
      return integrand;
    }


    #endregion


    #region Core Structures

    public class Alt1GnI
    {
      protected double factorp;
      protected double facdiv;
      protected double alpha;
      protected double dev;
      protected double pdfPrefactor;

      protected double _x0;

      protected ScalarFunctionDD pdfCore;
      protected ScalarFunctionDD pdfFunc;

      public Alt1GnI(double factorp, double facdiv, double pdfPrefactor, double alpha, double dev)
      {
        this.factorp = factorp;
        this.facdiv = facdiv;
        this.pdfPrefactor = pdfPrefactor;
        this.alpha = alpha;
        this.dev = dev;
        pdfCore = null;
        pdfFunc = null;
        Initialize();
      }

      public void Initialize()
      {
        pdfCore = new ScalarFunctionDD(PDFCore);
        pdfFunc = new ScalarFunctionDD(PDFFunc);
      }

      public static Alt1GnI FromAlphaGamma(double x, double alpha, double gamma)
      {
        return FromAlphaGammaAga(x, alpha, gamma, GetAgaFromAlphaGamma(alpha, gamma));
      }

      public static Alt1GnI FromAlphaGammaAga(double x, double alpha, double gamma, double aga)
      {
        double factorp, facdiv, dev, pdfPrefactor;
        GetAlt1GnParameterByGamma(x, alpha, gamma,aga, out factorp, out facdiv, out dev, out pdfPrefactor);
        return new Alt1GnI(factorp, facdiv, pdfPrefactor, alpha, dev);
      }


      public double PDFCore(double thetas)
      {
        double r1;
        double r2;
        if (dev == 0 && thetas < 1E-10)
        {
          r1 = Math.Pow(alpha / factorp, alpha / (1 - alpha));
          r2 = (1 - alpha) / facdiv;
        }
        else
        {
          r1 = Math.Pow(Math.Sin(alpha * thetas) / (factorp * Math.Sin(dev + thetas)), alpha / (1 - alpha));
          r2 = Math.Sin(dev + thetas * (1 - alpha)) / (facdiv * Math.Sin(dev + thetas));
        }
        double result = r1 * r2;
        if (!(result >= 0))
          result = double.MaxValue;

        //System.Diagnostics.Debug.WriteLine(string.Format("CorAlt1GnI theta={0}, result={1}", thetas, result));
        return result;
      }

      public double PDFCoreDerivative(double thetas)
      {
        if (thetas == 0)
          return dev == 0 ? 0 : double.PositiveInfinity;

        double r1, r1s;

        r1 = Math.Pow(Math.Sin(alpha * thetas) / (factorp * Math.Sin(dev + thetas)), alpha / (1 - alpha));
        r1s =  r1 * alpha / (1 - alpha);
        r1s *= alpha / Math.Tan(alpha * thetas) - 1 / Math.Tan(dev + thetas);

        double r2, r2s;
        r2 = Math.Sin(dev + thetas * (1 - alpha)) / (facdiv * Math.Sin(dev + thetas));
        r2s = r2;
        r2s *= (1 - alpha) / Math.Tan(dev + thetas * (1 - alpha)) - 1 / Math.Tan(dev + thetas);

        return r1 * r2s + r1s * r2;
      }

     

      public double PDFFunc(double x)
      {
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(-f);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double PDFFuncLogInt(double z)
      {
        double x = Math.Exp(z);
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(z-f);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }


      public double PDFFuncLogIntToLeft(double z)
      {
        double x = _x0-Math.Exp(z);
        if (x < 0)
          x = 0;

        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(z - f);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }
      public double PDFFuncLogIntToRight(double z)
      {
        double x = _x0 + Math.Exp(z);
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(z - f);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }


      public double Integrate(ref object tempStorage, double precision)
      {
        double integrand;
        if (dev == 0)
        {
          //integrand = IntegrateFuncExpMFuncIncAdvanced(pdfCore, pdfFunc, 0, 0, Math.PI - dev, ref tempStorage, precision);
          integrand = PDFIntegrateZeroDev(ref tempStorage, precision);
        }
          
        else if (dev < 1e-2)
        {
          //double xm = FindIncreasingYEqualToOne(pdfCore, 0, Math.PI - dev);
          //integrand = IntegrateFuncExpMFuncIncAdvanced(pdfCore, pdfFunc, 0, xm, Math.PI - dev, ref tempStorage, precision);
          integrand = PDFIntegrateSmallDev(ref tempStorage, precision);
        }
         
        else
        {
          integrand = IntegrateFuncExpMFuncInc(pdfCore, pdfFunc, 0, Math.PI - dev, ref tempStorage, precision);
        }
        return pdfPrefactor * integrand;
      }

      private double PDFIntegrateZeroDev(ref object tempStorage, double precision)
      {
        // for zero dev we know that the core is constant until x=1E-10
        // so the first part

        double y0 = pdfCore(0);

        if(y0>(MinusLogTiny + 2))
          return 0;

        double xm = 1E-10;

        double resultLeft = xm * pdfFunc(0);

        GSL_ERROR error1;
        double resultRight, abserrRight;
        // now integrate logarithmically
        error1 = Calc.Integration.QagpIntegration.Integration(
 new ScalarFunctionDD(this.PDFFuncLogInt),
 new double[] { Math.Log(xm), Math.Log(Math.PI) }, 2, 0, precision, 100, out resultRight, out abserrRight, ref tempStorage);

        if (null != error1)
          resultRight = double.NaN;

        return resultLeft + resultRight;
      }

      private double PDFIntegrateSmallDev(ref object tempStorage, double precision)
      {
        GSL_ERROR error;
        double resultLeft=0, resultRight;
        double abserrLeft;
        double xm = FindIncreasingYEqualToOne(pdfCore, 0, Math.PI - dev);

        if (xm > 0)
        {
          double yh = pdfCore(xm /2);
          if (yh < 0.5)
          {
            double fac = 1e5;
            // then the area is more or less concentrated at xm
            _x0 = xm * (1 + fac*DoubleConstants.DBL_EPSILON);
            error = Calc.Integration.QagpIntegration.Integration(
         new ScalarFunctionDD(this.PDFFuncLogIntToLeft), new double[] { Math.Log(xm * fac*DoubleConstants.DBL_EPSILON), Math.Log(_x0) }, 2, 0, precision, 100, out resultLeft, out abserrLeft, ref tempStorage);

          }
          else
          {
            // First integrate the left side
            error = Calc.Integration.QagpIntegration.Integration(
            pdfFunc, new double[] { 0, xm }, 2, 0, precision, 100, out resultLeft, out abserrLeft, ref tempStorage);
          }
          if (null != error)
          {
              resultLeft = double.NaN;
          }
        }
        // now the right side
        double[] intgrenzen = new double[100];
        resultRight = PDFIntegrateUnknownRightSideInc(xm, Math.PI-dev, intgrenzen, ref tempStorage, precision);

        return resultLeft + resultRight;
      }


      double PDFIntegrateUnknownRightSideInc(double x0, double x1a, double[] intgrenzen, ref object tempStorage, double precision)
      {

        const double diffOneDecade = 7;
        int count;
        double y1;
        GSL_ERROR error1=null;
        GSL_ERROR error2 = null;
        double result1 = 0;
        double result2 = 0;
        double abserr1, abserr2;

        double x1 = FindIncreasingYEqualTo(pdfCore, x0, x1a, MinusLogTiny + 1, 1, out y1);
        double y0 = pdfCore(x0);

        // When the difference of y values results in a difference able to handle by the algorithm, then return immediately
        if ((y0 >= MinusLogTiny) || ((y1 - y0) < diffOneDecade))
        {
          intgrenzen[0] = x0;
          intgrenzen[0] = x1;
          count = 2;
          error1 = Calc.Integration.QagpIntegration.Integration(
            pdfFunc, intgrenzen, count, 0, precision, 100, out result1, out abserr1, ref tempStorage);
        }
        else
        {
          // now take the overall derivative
          double s01 = (y1 - y0) / (x1 - x0); // overall derivative


          // Take the values in the vicinity of x0 and x1, respectively, but make sure
          // not to use too big differences

          double y00, y11;
          double s0, s1;
          double dx;

          s0 = PDFCoreDerivative(x0);
          s1 = PDFCoreDerivative(x1);

          if (s0 > s01 && s1 < s01)
          {
            error1 = Calc.Integration.QagpIntegration.Integration(
             new ScalarFunctionDD(this.PDFFuncLogInt), 
             new double[]{Math.Log(x0), Math.Log(x1a)}, 2, 0, precision, 100, out result1, out abserr1, ref tempStorage);

            if (null != error1)
            {
              // increasing fast at x0 and slow at x1, so part from the left
              count = PartFromTheLeft(x0, x1, diffOneDecade / s0, intgrenzen, 0);
              error1 = Calc.Integration.QagpIntegration.Integration(
              pdfFunc, intgrenzen, count, 0, precision, 100, out result1, out abserr1, ref tempStorage);
            }
          

          }
          else if (s0 < s01 && s1 > s01)
          {
            _x0 = x1 + diffOneDecade / s1;
            error1 = Calc.Integration.QagpIntegration.Integration(
            new ScalarFunctionDD(this.PDFFuncLogIntToLeft),
            new double[] { Math.Log(diffOneDecade / s1), Math.Log(_x0 - x0) }, 2, 0, precision, 100, out result1, out abserr1, ref tempStorage);

            if (null != error1)
            {
              // increasing slow at x0 and fast at x1, so part from the right
              count = PartFromTheRight(x0, x1, diffOneDecade / s1, intgrenzen, 0);
              error1 = Calc.Integration.QagpIntegration.Integration(
              pdfFunc, intgrenzen, count, 0, precision, 100, out result1, out abserr1, ref tempStorage);
            }
          }
          else if (s0 < s01 && s1 < s01)
          {
            // in this case, there is the fast transition somewhere inbetween the interval, so we have to search for it
            double ym;
            double xm = FindIncreasingYEqualTo(pdfCore, x0, x1, 0.5 * (y0 + y1), 0.1, out ym);
            double sm = PDFCoreDerivative(xm);

            double xinterval = sm > 0 ? diffOneDecade / sm : xm * DoubleConstants.DBL_EPSILON;
            count = PartFromTheRight(x0, xm, xinterval, intgrenzen, 0);
            error1 = Calc.Integration.QagpIntegration.Integration(
            pdfFunc, intgrenzen, count, 0, precision, 100, out result1, out abserr1, ref tempStorage);

            count = PartFromTheLeft(xm, x1, xinterval, intgrenzen, 0);
            error2 = Calc.Integration.QagpIntegration.Integration(
              pdfFunc, intgrenzen, count, 0, precision, 100, out result2, out abserr2, ref tempStorage);
          }
          else if (s0 > s01 && s1 > s01)
          {
            // then we have fast increases both on x0 and x1, so we must have a plateau inbetween
            double xm = 0.5 * (x0 + x1);

            error1 = Calc.Integration.QagpIntegration.Integration(
           new ScalarFunctionDD(this.PDFFuncLogInt),
           new double[] { Math.Log(x0), Math.Log(xm) }, 2, 0, precision, 100, out result1, out abserr1, ref tempStorage);

            if (null != error1)
            {
              count = PartFromTheLeft(x0, xm, diffOneDecade / s0, intgrenzen, 0);
              error1 = Calc.Integration.QagpIntegration.Integration(
              pdfFunc, intgrenzen, count, 0, precision, 100, out result1, out abserr1, ref tempStorage);
            }

            error2 = Calc.Integration.QagpIntegration.Integration(
              pdfFunc, new double[] { xm, x1 }, 2, 0, precision, 100, out result2, out abserr2, ref tempStorage);

            if (null != error2)
            {
              count = PartFromTheRight(xm, x1, diffOneDecade / s1, intgrenzen, 0);
              error2 = Calc.Integration.QagpIntegration.Integration(
                pdfFunc, intgrenzen, count, 0, precision, 100, out result2, out abserr2, ref tempStorage);
            }

          }
          else
          {
            // part linearly spaced between x0 and x1
            double xinc = diffOneDecade / s01;
            double xs = x0;
            for (count = 0; count < intgrenzen.Length; count++)
            {
              if (xs >= x1)
              {
                intgrenzen[count] = x1;
                count++;
                break;
              }
              intgrenzen[count] = xs;
              xs += xinc;
            }

            error1 = Calc.Integration.QagpIntegration.Integration(
              pdfFunc, intgrenzen, count, 0, precision, 100, out result1, out abserr1, ref tempStorage);

          }
        }

        if (error1 == null && error2 == null)
          return result1 + result2;
        else
          return double.NaN;
      }


      public bool IsMaximumLeftHandSide()
      {
        return PDFCore(0.5 * (Math.PI - dev)) > 1;
      }

      public double UpperIntegrationLimit
      {
        get
        {
          return Math.PI - dev;
        }
      }

      public double FindXOfPDFCoreY(double ysearch, double tolerance)
      {
        double yfound;
        double result;
        result = FindIncreasingYEqualTo(pdfCore, 0, UpperIntegrationLimit, ysearch, tolerance, out yfound);
        return result;
      }


    }

    public class Alt1GnIA1 : Alt1GnI
    {
      double _xm;
      double _n;
      double _a;
      double _xmax; // distance from _xm, for which points we use the derivative approximation

       public Alt1GnIA1(double factorp, double facdiv, double pdfPrefactor, double alpha, double dev)
      : base(factorp, facdiv,pdfPrefactor,alpha,dev)
       {
        
      }
      public static Alt1GnI FromAlphaGamma(double x, double alpha, double gamma)
      {
        return FromAlphaGammaAga(x, alpha, gamma, GetAgaFromAlphaGamma(alpha, gamma));
      }

      public static Alt1GnI FromAlphaGammaAga(double x, double alpha, double gamma, double aga)
      {
        double factorp, facdiv, dev, pdfPrefactor;
        GetAlt1GnParameterByGamma(x, alpha, gamma, aga, out factorp, out facdiv, out dev, out pdfPrefactor);
        return new Alt1GnIA1(factorp, facdiv, pdfPrefactor, alpha, dev);
      }

      public double PDFCoreMod(double dx)
      {
        if (Math.Abs(dx) < _xmax)
        {
          //System.Diagnostics.Debug.Write("DN-");
          return Math.Exp(_n * RMath.Log1p(_a * dx));
        }
        else
        {
          //System.Diagnostics.Debug.Write("CO-");
          return PDFCore(_xm + dx);
        }
      }

      public new double PDFFuncLogIntToLeft(double z)
      {
        double x = Math.Max(-_xm, _x0 - Math.Exp(z));
        double f = PDFCoreMod(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(z - f);
        //System.Diagnostics.Debug.WriteLine(string.Format("z={0}, x={1}, f={2}, r={3}", z, x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }
      public new double PDFFuncLogIntToRight(double z)
      {
        double x = _x0 + Math.Exp(z);
        double f = PDFCoreMod(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(z - f);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double PDFIntegrateAlphaNearOne(ref object tempStorage, double precision)
      {
        GSL_ERROR error;
        double resultLeft = 0, resultRight;
        double abserrLeft, abserrRight;
        double yfound;
        // we want to find xm now with very high accuracy
        _xm = FindIncreasingYEqualTo(pdfCore, 0, Math.PI - dev, 1, 0, out yfound);

        double r1c = PDF_R1Core(_xm);
        double r1cderByr1c = alpha / Math.Tan(alpha * _xm) - 1 / Math.Tan(dev + _xm);
        // r1 is now approximated by r1c + r1der*(x-xm)
        // the logarithm of r1 is log(r1c) + r1cder*(x-xm)/r1c - (r1cder*(x-xm))^2/(2*r1c^2) + O((x-xm)^3)
        // in order to keep as much precision as possible, Abs(x-xm) must be smaller than precision*2*r1c/r1cder

        // now we calculate the integration boundaries
        _a = r1cderByr1c;
        _n = alpha / (1 - alpha);

        double d = r1cderByr1c;

        double xinc = 0.125 / (_n*r1cderByr1c); // this is the smallest interval to use for the logarithmic integration
        _xmax = 0.125/r1cderByr1c; // this is the maximum interval to use using the derivative approximation

        if (_xmax < 1e5 * _xm * DoubleConstants.DBL_EPSILON)
          return pdfPrefactor * GammaRelated.Gamma(1 / _n) / (d * _n * _n); // then we use the analytic solution of the integral
        else
          _xmax = 1e5 * _xm * DoubleConstants.DBL_EPSILON;

        // now we integrate
        
        double r2 = Math.Sin(dev + _xm * (1 - alpha)) / (facdiv * Math.Sin(dev + _xm));

        // oder wir versuchen dies

        _x0 = xinc;
        error = Calc.Integration.QagpIntegration.Integration(
          new ScalarFunctionDD(this.PDFFuncLogIntToLeft),
          new double[] { Math.Log(_x0), Math.Log(_xm+_x0) }, 2,
          0, precision, 200, out resultLeft, out abserrLeft, ref tempStorage);

        _x0 = -xinc;
        error = Calc.Integration.QagpIntegration.Integration(
          new ScalarFunctionDD(this.PDFFuncLogIntToRight),
          new double[] { Math.Log(-_x0), Math.Log(Math.PI - dev - _xm - _x0) }, 2,
          0, precision, 100, out resultRight, out abserrRight, ref tempStorage);


        return pdfPrefactor * (resultLeft + resultRight);
      }

      private double PDF_R1Core(double thetas)
      {
        return Math.Sin(alpha * thetas) / (factorp * Math.Sin(dev + thetas));
      }
      private double PDF_R1CoreDerivative(double thetas)
      {
        return (alpha * Math.Cos(alpha * thetas) - Math.Sin(alpha * thetas) / Math.Tan(dev + thetas)) / (factorp * Math.Sin(dev + thetas));
      }


    }

    public class Alt1GnD
    {
      double factorp;
      double facdiv;
      double alpha;
      double dev;
      double pdfPrefactor;
      ScalarFunctionDD pdfCore;
      ScalarFunctionDD pdfFunc;

      public Alt1GnD()
      {
      }

      public Alt1GnD(double factorp, double facdiv, double pdfPrefactor, double alpha, double dev)
      {
        this.factorp = factorp;
        this.facdiv = facdiv;
        this.pdfPrefactor = pdfPrefactor;
        this.alpha = alpha;
        this.dev = dev;
        pdfCore = null;
        pdfFunc = null;
        Initialize();
      }

      public void Initialize()
      {
        pdfCore = new ScalarFunctionDD(PDFCore);
        pdfFunc = new ScalarFunctionDD(PDFFunc);
      }

      static Alt1GnD FromAlphaGamma(double x, double alpha, double gamma)
      {
        return FromAlphaGammaAga(x, alpha, gamma, GetAgaFromAlphaGamma(alpha, gamma));
      }

      static Alt1GnD FromAlphaGammaAga(double x, double alpha, double gamma, double aga)
      {
        double factorp, facdiv, dev, pdfPrefactor;
        GetAlt1GnParameterByGamma(x, alpha, gamma, aga, out factorp, out facdiv, out dev, out pdfPrefactor);
        return new Alt1GnD(factorp, facdiv, pdfPrefactor, alpha, dev);
      }

      public double PDFCore(double thetas)
      {
        double r1;
        double r2;
        r1 = Math.Pow(Math.Sin(alpha * (Math.PI - dev - thetas)) / (factorp * Math.Sin(thetas)), alpha / (1 - alpha));
        r2 = Math.Sin(alpha * (Math.PI - dev) + thetas * (1 - alpha)) / (facdiv * Math.Sin(thetas));
        double result = r1 * r2;
        if (!(result >= 0))
          result = double.MaxValue;

        //System.Diagnostics.Debug.WriteLine(string.Format("Cor1e theta={0}, result={1}", thetas, result));
        return result;
      }
      public double PDFFunc(double x)
      {
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(-f);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double Integrate(ref object tempStorage, double precision)
      {
        return pdfPrefactor * IntegrateFuncExpMFuncDec(pdfCore, pdfFunc, 0, Math.PI - dev, ref tempStorage, precision);
      }



    }

    public class Alt1GpI
    {
      double factorp;
      double facdiv;
      double alpha;
      double dev;
      double pdfPrefactor;
      ScalarFunctionDD pdfCore;
      ScalarFunctionDD pdfFunc;

      public Alt1GpI(double factorp, double facdiv, double pdfPrefactor, double alpha, double dev)
      {
        this.factorp = factorp;
        this.facdiv = facdiv;
        this.pdfPrefactor = pdfPrefactor;
        this.alpha = alpha;
        this.dev = dev;
        pdfCore = null;
        pdfFunc = null;
        Initialize();
      }

      public void Initialize()
      {
        pdfCore = new ScalarFunctionDD(PDFCore);
        pdfFunc = new ScalarFunctionDD(PDFFunc);
      }

      static Alt1GpI FromAlphaGamma(double x, double alpha, double gamma)
      {
        return FromAlphaGammaAga(x, alpha, gamma, GetAgaFromAlphaGamma(alpha, gamma));
      }

      static Alt1GpI FromAlphaGammaAga(double x, double alpha, double gamma, double aga)
      {
        double factorp, facdiv, dev, pdfPrefactor;
        GetAlt1GpParameterByGamma(x, alpha, gamma, aga, out factorp, out facdiv, out dev, out pdfPrefactor);
        return new Alt1GpI(factorp, facdiv, pdfPrefactor, alpha, dev);
      }


      // Note that in the moment we dont have a directional change here, but one can easily change
      // the integration direction by replacing thetas with d-thetas (since the integration goes from 0 to dev).
      public double PDFCore(double thetas)
      {
        double r1;
        double r2;

        r1 = Math.Pow(Math.Sin(alpha * thetas) / (factorp * Math.Sin(dev - thetas)), alpha / (1 - alpha));
        r2 = Math.Sin(dev - thetas * (1 - alpha)) / (facdiv * Math.Sin(dev - thetas));

        double result = r1 * r2;
        if (!(result >= 0))
          result = double.MaxValue;

        //System.Diagnostics.Debug.WriteLine(string.Format("Cor1f theta={0}, result={1}", thetas, result));
        return result;
      }
      public double PDFFunc(double x)
      {
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(-f);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }
      public double Integrate(ref object tempStorage, double precision)
      {
        return pdfPrefactor * IntegrateFuncExpMFuncInc(pdfCore, pdfFunc, 0, dev, ref tempStorage, precision);
      }

      public bool IsMaximumLeftHandSide()
      {
        return PDFCore(0.5 * dev) > 1;
      }

    }

    public class Alt1GpD
    {
      double factorp;
      double facdiv;
      double alpha;
      double dev;
      double pdfPrefactor;
      ScalarFunctionDD pdfCore;
      ScalarFunctionDD pdfFunc;

      public Alt1GpD(double factorp, double facdiv, double pdfPrefactor, double alpha, double dev)
      {
        this.factorp = factorp;
        this.facdiv = facdiv;
        this.pdfPrefactor = pdfPrefactor;
        this.alpha = alpha;
        this.dev = dev;
        pdfCore = null;
        pdfFunc = null;
        Initialize();
      }

      public void Initialize()
      {
        pdfCore = new ScalarFunctionDD(PDFCore);
        pdfFunc = new ScalarFunctionDD(PDFFunc);
      }

      static Alt1GpD FromAlphaGamma(double x, double alpha, double gamma)
      {
        return FromAlphaGammaAga(x, alpha, gamma, GetAgaFromAlphaGamma(alpha, gamma));
      }

      static Alt1GpD FromAlphaGammaAga(double x, double alpha, double gamma, double aga)
      {
        double factorp, facdiv, dev, pdfPrefactor;
        GetAlt1GpParameterByGamma(x, alpha, gamma, aga, out factorp, out facdiv, out dev, out pdfPrefactor);

        return new Alt1GpD(factorp, facdiv, pdfPrefactor, alpha, dev);
      }

      public double PDFCore(double thetas)
      {
        double r1;
        double r2;

        r1 = Math.Pow(Math.Sin(alpha * (dev - thetas)) / (factorp * Math.Sin(thetas)), alpha / (1 - alpha));
        r2 = Math.Sin(alpha * dev + thetas * (1 - alpha)) / (facdiv * Math.Sin(thetas));

        double result = r1 * r2;
        if (!(result >= 0))
          result = double.MaxValue;

        //System.Diagnostics.Debug.WriteLine(string.Format("Cor1f theta={0}, result={1}", thetas, result));
        return result;
      }
      public double PDFFunc(double x)
      {
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(-f);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double Integrate(ref object tempStorage, double precision)
      {
        return pdfPrefactor * IntegrateFuncExpMFuncDec(pdfCore, pdfFunc, 0, dev, ref tempStorage, precision);
      }


    }

    public class Agt1GnI
    {
      double factorp;
      double factorw;
      double alpha;
      double dev;
      double pdfPrefactor;
      ScalarFunctionDD pdfCore;
      ScalarFunctionDD pdfFunc;

      public Agt1GnI(double factorp, double factorw, double pdfPrefactor, double alpha, double dev)
      {
        this.factorp = factorp;
        this.factorw = factorw;
        this.pdfPrefactor = pdfPrefactor;
        this.alpha = alpha;
        this.dev = dev;
        pdfCore = null;
        pdfFunc = null;
        Initialize();
      }

      public void Initialize()
      {
        pdfCore = new ScalarFunctionDD(PDFCore);
        pdfFunc = new ScalarFunctionDD(PDFFunc);
      }

      static Agt1GnI FromAlphaGamma(double x, double alpha, double gamma)
      {
        return FromAlphaGammaAga(x, alpha, gamma, GetAgaFromAlphaGamma(alpha, gamma));
      }

      static Agt1GnI FromAlphaGammaAga(double x, double alpha, double gamma, double aga)
      {
        double factorp, factorw, dev, pdfPrefactor;
        GetAgt1GnParameterByGamma(x, alpha, gamma, aga, out factorp, out factorw, out dev, out pdfPrefactor);
        return new Agt1GnI(factorp, factorw, pdfPrefactor, alpha, dev);
      }

      public double PDFCore(double thetas)
      {
        double r1;
        double r2;
        if (dev == 0 && thetas < 1e-9)
        {
          r1 = Math.Pow(factorp / alpha, 1 / (alpha - 1));
          r2 = factorw * (alpha - 1) / alpha;
        }
        else
        {
          double sin_al_theta_dev = Math.Sin(alpha * thetas + dev);
          r1 = Math.Pow(factorp * Math.Sin(thetas) / sin_al_theta_dev, 1 / (alpha - 1));
          r2 = (factorw * Math.Sin(thetas * (alpha - 1) + dev)) / sin_al_theta_dev;
        }
        double result = r1 * r2;
        if (!(result >= 0))
          result = double.MaxValue;

        //System.Diagnostics.Debug.WriteLine(string.Format("Cor1d theta={0}, result={1}", thetas, result));
        return result;
      }

      public double PDFFunc(double x)
      {
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(-f);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double Integrate(ref object tempStorage, double precision)
      {
        return pdfPrefactor * IntegrateFuncExpMFuncInc(pdfCore, pdfFunc, 0, Math.PI - dev, ref tempStorage, precision);
      }

      public bool IsMaximumLeftHandSide()
      {
        return PDFCore(0.5 * (Math.PI - dev)) > 1;
      }


    }

    public class Agt1GnD
    {
      double factorp;
      double factorw;
      double alpha;
      double dev;
      double pdfPrefactor;
      ScalarFunctionDD pdfCore;
      ScalarFunctionDD pdfFunc;

      public Agt1GnD(double factorp, double factorw, double pdfPrefactor, double alpha, double dev)
      {
        this.factorp = factorp;
        this.factorw = factorw;
        this.pdfPrefactor = pdfPrefactor;
        this.alpha = alpha;
        this.dev = dev;
        pdfCore = null;
        pdfFunc = null;
        Initialize();
      }

      public void Initialize()
      {
        pdfCore = new ScalarFunctionDD(PDFCore);
        pdfFunc = new ScalarFunctionDD(PDFFunc);
      }

      static Agt1GnD FromAlphaGamma(double x, double alpha, double gamma)
      {
        return FromAlphaGammaAga(x, alpha, gamma, GetAgaFromAlphaGamma(alpha, gamma));
      }

      static Agt1GnD FromAlphaGammaAga(double x, double alpha, double gamma, double aga)
      {
        double factorp, factorw, dev, pdfPrefactor;
        GetAgt1GnParameterByGamma(x, alpha, gamma, aga, out factorp, out factorw, out dev, out pdfPrefactor);
        return new Agt1GnD(factorp, factorw, pdfPrefactor, alpha, dev);
      }

      public double PDFCore(double thetas)
      {
        double r1;
        double r2;
        double sin_al_theta = Math.Sin(alpha * thetas);
        double pi_mdev_byalpha = (Math.PI - dev) / alpha;

        {
          r1 = Math.Pow(factorp * Math.Sin(pi_mdev_byalpha - thetas) / sin_al_theta, 1 / (alpha - 1));
          r2 = (factorw * Math.Sin((dev + (alpha - 1) * (Math.PI - alpha * thetas)) / alpha)) / sin_al_theta;
        }
        double result = r1 * r2;
        if (!(result >= 0))
          result = double.MaxValue;

        //System.Diagnostics.Debug.WriteLine(string.Format("Cor1d theta={0}, result={1}", thetas, result));
        return result;
      }
      public double PDFFunc(double x)
      {
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(-f);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double Integrate(ref object tempStorage, double precision)
      {
        return pdfPrefactor * IntegrateFuncExpMFuncDec(pdfCore, pdfFunc, 0, Math.PI - dev, ref tempStorage, precision);
      }


    }

    #endregion




    #endregion


  }
}
  
