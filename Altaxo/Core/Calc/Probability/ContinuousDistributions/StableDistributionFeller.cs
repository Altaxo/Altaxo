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

    /// <summary>
    /// Calculates the probability density using either series expansion for small or big arguments, or a integration
    /// in the intermediate range.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static double PDF(double x, double alpha, double gamma, ref object tempStorage, double precision)
    {
      if (x > 0)
        return PDFforPositiveX(x, alpha, gamma, ref tempStorage, precision);
      else if (x < 0)
        return PDFforPositiveX(-x, alpha, -gamma, ref tempStorage, precision);
      else if (x == 0)
        return PDFforXZero(alpha, gamma);
      else
        return double.NaN;

    }

    /// <summary>
    /// Calculates the probability density using either series expansion for small or big arguments, or a integration
    /// in the intermediate range.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static double PDFforPositiveX(double x, double alpha, double gamma, ref object tempStorage, double precision)
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
        if (Math.Abs(gamma * OneMinus2Eps) > 2 - alpha)
          throw new ArgumentException(string.Format("Absolute value of parameter gamma must be <={0}, but is {1}", 1 - Math.Abs(1 - alpha), gamma));
      }

      if (alpha <= 1)
      {
        // special case alpha==gamma
        if (gamma >= alpha * OneMinus2Eps)
          return 0;

        if (alpha <= 0.2)
        {
          if (alpha < 0.1)
            return PDFAlphaBetween0And01(x, alpha, gamma, ref tempStorage, precision);
          else
            return PDFAlphaBetween01And02(x, alpha, gamma, ref tempStorage, precision);
        }
        else // alpha >0.2
        {
          if (alpha <= 0.99)
            return PDFAlphaBetween02And099(x, alpha, gamma, ref tempStorage, precision);
          else
            return PDFAlphaBetween099And101(x, alpha, gamma, ref tempStorage, precision);
        }
      }
      else // alpha>1
      {
        if (alpha <= 1.01)
          return PDFAlphaBetween099And101(x, alpha, gamma, ref tempStorage, precision);
        else if (alpha <= 1.99995)
          return PDFAlphaBetween101And199999(x, alpha, gamma, ref tempStorage, precision);
        else
          return PDFAlphaBetween199999And2(x, alpha, gamma, ref tempStorage, precision);
      }
    }

    public static double PDFforXZero(double alpha, double gamma)
    {
      return -GammaRelated.Gamma(1 / alpha) / (alpha * Math.PI) * Math.Sin(0.5 * Math.PI * (gamma - alpha) / alpha);
    }

    static double GetLog10BoundaryForOneTermOfSeriesExpansionSmall(double alpha, double precision)
    {
      const double Log10 = 2.302585092994045684017991;
      // we use Stirlings formula to approximate
      // x < precision * Gamma(1+1/alpha)/Gamma(1/2+alpha)
      double OneByAlpha=1/alpha;
      double r = Math.Log(1 + OneByAlpha) * (0.5 + OneByAlpha) + Math.Log(1 + 2 * OneByAlpha) * (-0.5 - 2 * OneByAlpha) + OneByAlpha + Math.Log(precision);
      return r/Log10;
    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 0.0 and 0.1.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween0And01(double x, double alpha, double gamma, ref object tempStorage, double precision)
    {
      double smallestexp = GetLog10BoundaryForOneTermOfSeriesExpansionSmall(alpha, DoubleConstants.DBL_EPSILON);
      double lgx = Math.Log10(x);
      
      if (lgx <= smallestexp)
        return PDFforXZero(alpha, gamma);
      if (lgx > -0.3 / alpha)
        return PDFSeriesBigXFeller(x, alpha, gamma);
      else
        return PDFIntegral(x, alpha, gamma, ref tempStorage, precision);
    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 0.1 and 0.2. For small x (1E-16), the accuracy at alpha=0.1 is only 1E-7.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween01And02(double x, double alpha, double gamma, ref object tempStorage, double precision)
    {
      double a15 = alpha * Math.Sqrt(alpha);
      double smallestexp = -9 + 0.217147240951625 * ((-1.92074130618617 / a15 + 1.35936488329912 * a15));


      //  double smallestexp = 80 * alpha - 24; // Exponent is -16 for alpha=0.1 and -8 for alpha=0.2
      double lgx = Math.Log10(x);

      if (lgx <= smallestexp)
        return PDFforXZero(alpha, gamma);
      else if (lgx > 3 / (1 + alpha))
        return PDFSeriesBigXFeller(x, alpha, gamma);
      else
        return PDFIntegral(x, alpha, gamma, ref tempStorage, precision);

    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 0.2 and 0.99. For small x (1E-8), the accuracy at alpha=0.2 is only 1E-7.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween02And099(double x, double alpha, double gamma, ref object tempStorage, double precision)
    {
      double smallestexp;
      if (alpha <= 0.3)
        smallestexp = 30 * alpha - 14; // Exponent is -8 for alpha=0.2 and -5 for alpha=0.3
      else if (alpha <= 0.6)
        smallestexp = 10 * alpha - 8; // Exponent is -5 for alpha=0.3 and -2 for alpha=0.6
      else
        smallestexp = 2.5 * alpha - 3.5; // Exponent is -2 for alpha=0.6 and -1 for alpha=1

      double lgx = Math.Log10(x);
      if (lgx <= smallestexp)
        return PDFSeriesSmallXFeller(x, alpha, gamma);
      else if (lgx > 3 / (1 + alpha))
        return PDFSeriesBigXFeller(x, alpha, gamma);
      else
        return PDFIntegral(x, alpha, gamma, ref tempStorage, precision);
    }

    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 0.99 and 1.01.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween099And101(double x, double alpha, double gamma, ref object tempStorage, double precision)
    {
      if (x <= 0.1)
        return PDFSeriesSmallXFeller(x, alpha, gamma);
      else if (x >= 10)
        return PDFSeriesBigXFeller(x, alpha, gamma);
      else
        return PDFTaylorExpansionAroundAlphaOne(x, alpha, gamma);
    }


    /// <summary>
    /// Calculation of the PDF if alpha is inbetween 1.01 and 1.99999. 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="alpha"></param>
    /// <param name="precision"></param>
    /// <param name="tempStorage"></param>
    /// <returns></returns>
    public static double PDFAlphaBetween101And199999(double x, double alpha, double gamma, ref object tempStorage, double precision)
    {
      if (x <= 1E-2)
        return PDFSeriesSmallXFeller(x, alpha, gamma);
      else if (x >= 100)
        return PDFSeriesBigXFeller(x, alpha, gamma);
      else
        return PDFIntegral(x, alpha, gamma, ref tempStorage, precision);
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
    public static double PDFAlphaBetween199999And2(double x, double alpha, double gamma, ref object tempStorage, double precision)
    {
      if (alpha == 2)
        return Math.Exp(-0.25 * x * x) / (2 * Math.Sqrt(Math.PI)); // because gamma must be zero for alpha==2

      if (x <= 7)
        return PDFSeriesSmallXFeller(x, alpha, gamma);
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


    public static double PDFFromAlphaInversionFormula(double x, double alpha, double gamma, ref object tempStorage, double precision)
    {
      double xinv = Math.Pow(x, -alpha);
      return PDF(xinv, 1 / alpha, (gamma - alpha + 1) / alpha, ref tempStorage, precision) * xinv / x;
    }

    public static double PDFfromS0(double x, double alpha, double gamma, double sigma, double mu)
    {
      // Conversion to S0 parameters
      double mu0, sigma0, beta;
      ParameterConversionFellerToS0(alpha, gamma, sigma, mu, out beta, out sigma0, out mu0);
      return StableDistributionS0.PDF(x, alpha, beta, sigma0, mu0);
    }


    public static double PDFSeriesSmallX_S0(double z, double alpha, double beta)
    {
      double gamma, sigmaf, muf;
      ParameterConversionS0ToFeller(alpha, beta, 1, 0, out gamma, out sigmaf, out muf);

      return PDFSeriesSmallXFeller((z - muf) / sigmaf, alpha, gamma) / sigmaf;
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
    public static double PDFSeriesSmallXFeller(double z, double alpha, double gamma)
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
    public static double PDFSeriesBigXFeller(double z, double alpha, double gamma)
    {
      int k = 1;
      double pi_gamma_alpha_2 = Math.PI * (gamma - alpha) / 2;
      double z_pow_minusAlpha = Math.Pow(z, -alpha);
      double kfac = 1;
      double z_pow_minusKAlpha = -z_pow_minusAlpha;


      double term1a = GammaRelated.Gamma(1 + k * alpha) * z_pow_minusKAlpha / kfac;
      double term1 = term1a * Math.Sin(k * pi_gamma_alpha_2);

      double sum = term1;

      for (; ; )
      {

        ++k;
        z_pow_minusKAlpha *= -z_pow_minusAlpha;
        kfac *= k;

        double term2a = GammaRelated.Gamma(1 + k * alpha) * z_pow_minusKAlpha / kfac;
        double term2 = term2a * Math.Sin(k * pi_gamma_alpha_2);

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

    public static double PDFTaylorExpansionAroundAlphaOne(double x, double alpha, double gamma)
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

    public static double PDFIntegral(double x, double alpha, double gamma, ref object temp, double precision)
    {
      const double OneMinusEps = 1 - 4 * DoubleConstants.DBL_EPSILON;

      if (alpha < 1)
      {
        if (gamma<0)
          return PDFIntegralAlt1Gn(x, alpha, gamma, ref temp, precision);
        else
          return PDFIntegralAlt1Gp(x, alpha, gamma, ref temp, precision);
      }
      else // alpha>1
      {
        return PDFIntegralAgt1Gn(x, alpha, gamma, ref temp, precision);
      }
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
    public static double PDFIntegralAgt1Gn(double x, double alpha, double gamma, ref object temp, double precision)
    {
      double zeta = Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = Math.Pow(1 + zeta * zeta, 0.5 / alpha);

      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral
      double minusGammaByAlpha = -gamma / alpha;
      if (minusGammaByAlpha < -1)
        minusGammaByAlpha = -1;
      else if (minusGammaByAlpha > 1)
        minusGammaByAlpha = 1;

      double xi = Math.PI * (0.5 * minusGammaByAlpha); // equivalent to xi in S0 integral
      double xippi2 = Math.PI * (0.5 * (1 + minusGammaByAlpha));
      double dev = Math.PI * (0.5 * ((2 - alpha) + gamma));
      if (dev < 0)
        dev = 0;

      //double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      // we separate the factor in a power of 1/alpha-1 and the rest
      double factorp = xx * Math.Cos(alpha * xi);
      double factorw = xx;
      double integrand;

      if (PDFCoreAgt1GnI(factorp, factorw, alpha, dev, (Math.PI - dev) * 0.5 / alpha) > 1)
        integrand = IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAgt1GnI(factorp, factorw, alpha, dev, theta); }, 0, (Math.PI - dev) / alpha, ref temp, precision);
      else
        integrand = IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAgt1GnD(factorp, factorw, alpha, dev, theta); }, 0, (Math.PI - dev) / alpha, ref temp, precision);

      double pre = alpha / (Math.PI * Math.Abs(alpha - 1) * (xx));
      return pre * integrand * sigmaf;
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
    public static double PDFIntegralAgt1GnD(double x, double alpha, double gamma, ref object temp, double precision)
    {
      double zeta = Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = Math.Pow(1 + zeta * zeta, 0.5 / alpha);

      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral
      double minusGammaByAlpha = -gamma / alpha;
      if (minusGammaByAlpha < -1)
        minusGammaByAlpha = -1;
      else if (minusGammaByAlpha > 1)
        minusGammaByAlpha = 1;

      double xi = Math.PI * (0.5 * minusGammaByAlpha); // equivalent to xi in S0 integral
      double xippi2 = Math.PI * (0.5 * (1 + minusGammaByAlpha));
      double dev = Math.PI * (0.5 * ((2 - alpha) + gamma));
      if (dev < 0)
        dev = 0;

      //double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      // we separate the factor in a power of 1/alpha-1 and the rest
      double factorp = xx * Math.Cos(alpha * xi);
      double factorw = xx;

      double integrand = IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAgt1GnD(factorp, factorw, alpha, dev, theta); }, 0, (Math.PI - dev) / alpha, ref temp, precision);
      double pre = alpha / (Math.PI * Math.Abs(alpha - 1) * (xx));
      return pre * integrand * sigmaf;
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
    public static double PDFIntegralAgt1GnI(double x, double alpha, double gamma, ref object temp, double precision)
    {
      double zeta = Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = Math.Pow(1 + zeta * zeta, 0.5 / alpha);

      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral
      double minusGammaByAlpha = -gamma / alpha;
      if (minusGammaByAlpha < -1)
        minusGammaByAlpha = -1;
      else if (minusGammaByAlpha > 1)
        minusGammaByAlpha = 1;

      double xi = Math.PI * (0.5 * minusGammaByAlpha); // equivalent to xi in S0 integral
      double xippi2 = Math.PI * (0.5 * (1 + minusGammaByAlpha));
      double dev = Math.PI * (0.5 * ((2 - alpha) + gamma));
      if (dev < 0)
        dev = 0;

      //double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      // we separate the factor in a power of 1/alpha-1 and the rest
      double factorp = xx * Math.Cos(alpha * xi);
      double factorw = xx;

      double integrand = IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAgt1GnI(factorp, factorw, alpha, dev, theta); }, 0, (Math.PI - dev) / alpha, ref temp, precision);
      double pre = alpha / (Math.PI * Math.Abs(alpha - 1) * (xx));
      return pre * integrand * sigmaf;
    }



    private static double PDFCoreAgt1GnI(double factorp, double factorw, double alpha, double dev, double thetas)
    {
      double r1;
      double r2;
      if (dev==0 && thetas<1e-9)
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


    private static double PDFCoreAgt1GnD(double factorp, double factorw, double alpha, double dev, double thetas)
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
    public static double PDFIntegralAlt1Gp(double x, double alpha, double gamma, ref object temp, double precision)
    {
      double zeta = Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = Math.Pow(1 + zeta * zeta, 0.5 / alpha);

      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral
      double minusGammaByAlpha = -gamma / alpha;
      if (minusGammaByAlpha < -1)
        minusGammaByAlpha = -1;
      else if (minusGammaByAlpha > 1)
        minusGammaByAlpha = 1;

      double xi = Math.PI * (0.5 * minusGammaByAlpha); // equivalent to xi in S0 integral
      double dev = Math.PI * (0.5 * ((alpha - gamma) / alpha));
      if (dev < 0)
        dev = 0;

      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      double facdiv = Math.Cos(alpha * xi); // Inverse part of the original factor without power
      double factorp = xx * facdiv; // part of the factor with power alpha/(alpha-1);

      double integrand;
      if (PDFCoreAlt1GpI(factorp, facdiv, alpha, dev, 0.5 * dev) > 1)
        integrand = IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAlt1GpI(factorp, facdiv, alpha, dev, theta); }, 0, dev, ref temp, precision);
      else
        integrand = IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAlt1GpD(factorp, facdiv, alpha, dev, theta); }, 0, dev, ref temp, precision);

      double pre = alpha / (Math.PI * Math.Abs(alpha - 1) * (xx));
      return pre * integrand * sigmaf;
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
    public static double PDFIntegralAlt1GpI(double x, double alpha, double gamma, ref object temp, double precision)
    {
      double zeta = Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = Math.Pow(1 + zeta * zeta, 0.5 / alpha);

      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral
      double minusGammaByAlpha = -gamma / alpha;
      if (minusGammaByAlpha < -1)
        minusGammaByAlpha = -1;
      else if (minusGammaByAlpha > 1)
        minusGammaByAlpha = 1;

      double xi = Math.PI * (0.5 * minusGammaByAlpha); // equivalent to xi in S0 integral
      double dev = Math.PI * (0.5 * ((alpha - gamma) / alpha));
      if (dev < 0)
        dev = 0;

      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      double facdiv = Math.Cos(alpha * xi); // Inverse part of the original factor without power
      double factorp = xx * facdiv; // part of the factor with power alpha/(alpha-1);

      double integrand = IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAlt1GpI(factorp, facdiv, alpha, dev, theta); }, 0, dev, ref temp, precision);
      double pre = alpha / (Math.PI * Math.Abs(alpha - 1) * (xx));
      return pre * integrand * sigmaf;
    }



    // Note that in the moment we dont have a directional change here, but one can easily change
    // the integration direction by replacing thetas with d-thetas (since the integration goes from 0 to dev).
    private static double PDFCoreAlt1GpI(double factorp, double facdiv, double alpha, double dev, double thetas)
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
    public static double PDFIntegralAlt1GpD(double x, double alpha, double gamma, ref object temp, double precision)
    {
      double zeta = Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = Math.Pow(1 + zeta * zeta, 0.5 / alpha);

      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral
      double minusGammaByAlpha = -gamma / alpha;
      if (minusGammaByAlpha < -1)
        minusGammaByAlpha = -1;
      else if (minusGammaByAlpha > 1)
        minusGammaByAlpha = 1;

      double xi = Math.PI * (0.5 * minusGammaByAlpha); // equivalent to xi in S0 integral
      double dev = Math.PI * (0.5 * ((alpha - gamma) / alpha));
      if (dev < 0)
        dev = 0;

      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      double facdiv = Math.Cos(alpha * xi); // Inverse part of the original factor without power
      double factorp = xx * facdiv; // part of the factor with power alpha/(alpha-1);

      double integrand = IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAlt1GpD(factorp, facdiv, alpha, dev, theta); }, 0, dev, ref temp, precision);
      double pre = alpha / (Math.PI * Math.Abs(alpha - 1) * (xx));
      return pre * integrand * sigmaf;
    }

    private static double PDFCoreAlt1GpD(double factorp, double facdiv, double alpha, double dev, double thetas)
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
    public static double PDFIntegralAlt1Gn(double x, double alpha, double gamma, ref object temp, double precision)
    {
      double zeta = Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = Math.Pow(1 + zeta * zeta, 0.5 / alpha);

      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral
      double minusGammaByAlpha = -gamma / alpha;
      if (minusGammaByAlpha < -1)
        minusGammaByAlpha = -1;
      else if (minusGammaByAlpha > 1)
        minusGammaByAlpha = 1;

      double xi = Math.PI * (0.5 * minusGammaByAlpha); // equivalent to xi in S0 integral
      double dev = Math.PI * (0.5 * ((alpha + gamma) / alpha));
      if (dev < 0)
        dev = 0;

      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      double facdiv = Math.Cos(alpha * xi); // Inverse part of the original factor without power
      double factorp = xx * facdiv; // part of the factor with power alpha/(alpha-1);

      double integrand;
      if (PDFCoreAlt1GnI(factorp, facdiv, alpha, dev, 0.5 * (Math.PI - dev)) > 1)
      {
        integrand = IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAlt1GnI(factorp, facdiv, alpha, dev, theta); }, 0, Math.PI - dev, ref temp, precision);
        if(double.IsNaN(integrand))
          integrand = IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAlt1GnD(factorp, facdiv, alpha, dev, theta); }, 0, Math.PI - dev, ref temp, precision);
      }
      else
      {
        integrand = IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAlt1GnD(factorp, facdiv, alpha, dev, theta); }, 0, Math.PI - dev, ref temp, precision);
        if(double.IsNaN(integrand))
          integrand = IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAlt1GnI(factorp, facdiv, alpha, dev, theta); }, 0, Math.PI - dev, ref temp, precision);
      }

      double pre = alpha / (Math.PI * Math.Abs(alpha - 1) * (xx));
      return pre * integrand * sigmaf;
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
    public static double PDFIntegralAlt1GnI(double x, double alpha, double gamma, ref object temp, double precision)
    {
      double zeta = Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = Math.Pow(1 + zeta * zeta, 0.5 / alpha);

      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral
      double minusGammaByAlpha = -gamma / alpha;
      if (minusGammaByAlpha < -1)
        minusGammaByAlpha = -1;
      else if (minusGammaByAlpha > 1)
        minusGammaByAlpha = 1;

      double xi = Math.PI * (0.5 * minusGammaByAlpha); // equivalent to xi in S0 integral
      double dev = Math.PI * (0.5 * ((alpha + gamma) / alpha));
      if (dev < 0)
        dev = 0;

      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      double facdiv = Math.Cos(alpha * xi); // Inverse part of the original factor without power
      double factorp = xx * facdiv; // part of the factor with power alpha/(alpha-1);

      double integrand = IntegrateFuncExpMFuncInc(delegate(double theta) { return PDFCoreAlt1GnI(factorp, facdiv, alpha, dev, theta); }, 0, Math.PI - dev, ref temp, precision);
      double pre = alpha / (Math.PI * Math.Abs(alpha - 1) * (xx));
      return pre * integrand * sigmaf;
    }



    private static double PDFCoreAlt1GnI(double factorp, double facdiv, double alpha, double dev, double thetas)
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
    public static double PDFIntegralAlt1GnD(double x, double alpha, double gamma, ref object temp, double precision)
    {
      double zeta = Math.Tan(0.5 * gamma * Math.PI);
      double sigmaf = Math.Pow(1 + zeta * zeta, 0.5 / alpha);

      double xx = x * sigmaf; // equivalent to x-zeta in S0 integral
      double minusGammaByAlpha = -gamma / alpha;
      if (minusGammaByAlpha < -1)
        minusGammaByAlpha = -1;
      else if (minusGammaByAlpha > 1)
        minusGammaByAlpha = 1;

      double xi = Math.PI * (0.5 * minusGammaByAlpha); // equivalent to xi in S0 integral
      double dev = Math.PI * (0.5 * ((alpha + gamma) / alpha));
      if (dev < 0)
        dev = 0;

      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      double facdiv = Math.Cos(alpha * xi); // Inverse part of the original factor without power
      double factorp = xx * facdiv; // part of the factor with power alpha/(alpha-1);

      double integrand = IntegrateFuncExpMFuncDec(delegate(double theta) { return PDFCoreAlt1GnD(factorp, facdiv, alpha, dev, theta); }, 0, Math.PI - dev, ref temp, precision);
      double pre = alpha / (Math.PI * Math.Abs(alpha - 1) * (xx));
      return pre * integrand * sigmaf;
    }

    private static double PDFCoreAlt1GnD(double factorp, double facdiv, double alpha, double dev, double thetas)
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


    #endregion

    
  }
}
  
