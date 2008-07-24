using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Probability
{
  public class StableDistributionS1 : StableDistributionBase
  {

    double _alpha;
    double _beta;
    double _abe;

    double _mu;
    double _scale = 1;

    object _tempStorePDF;

    #region Construction
    /// <summary>
    /// Creates a new instance of this distribution with default parameters (alpha=1, beta=0) and the default generator.
    /// </summary>
    public StableDistributionS1()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with default parameters (alpha=1, beta=0).
    /// </summary>
    /// <param name="generator">Random number generator to be used with this distribution.</param>
    public StableDistributionS1(Generator generator)
      : this(1, 0, 1, 1, 0, generator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta) and the default random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    public StableDistributionS1(double alpha, double beta)
      : this(alpha, beta, GetAbeFromBeta(beta), 1, 0, DefaultGenerator)
    {
    }

    /// <summary>
    /// Creates a new instance of this distribution with given parameters (alpha, beta, abe) and the default random number generator.
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is 1-beta for beta&gt;=0 or 1+beta for beta&lt;0.</param>
    public StableDistributionS1(double alpha, double beta, double abe)
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
    public StableDistributionS1(double alpha, double beta, double scale, double location)
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
    public StableDistributionS1(double alpha, double beta, double scale, double location, Generator generator)
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
    public StableDistributionS1(double alpha, double beta, double abe, double scale, double location)
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
    public StableDistributionS1(double alpha, double beta, double abe, double scale, double location, Generator generator)
      : base(generator)
    {
      Initialize(alpha, beta, abe, scale, location);
    }

#endregion

    #region Distribution properties
    /// <summary>
    /// Initializes this instance of the distribution with the distribution parameters. 
    /// </summary>
    /// <param name="alpha">Distribution parameter alpha (broadness exponent).</param>
    /// <param name="beta">Distribution parameter beta (skew).</param>
    /// <param name="abe">Parameter to specify beta with higher accuracy around -1 and 1. Is 1-beta for beta&gt;=0 or 1+beta for beta&lt;0.</param>
    /// <param name="scale">Scaling parameter (broadness of the distribution).</param>
    /// <param name="location">Location of the distribution.</param>
    public void Initialize(double alpha, double beta, double abe, double scale, double location)
    {
      if (!IsValidAlpha(alpha))
        throw new ArgumentOutOfRangeException("Alpha out of range (must be greater 0.1 and smalle or equal than 2)");
      if (!IsValidBeta(beta))
        throw new ArgumentOutOfRangeException("Beta out of range (must be in the range [-1,1])");
      if (!IsValidScale(scale))
        throw new ArgumentOutOfRangeException("Sigma out of range (must be >0)");
      if (!IsValidLocation(location))
        throw new ArgumentOutOfRangeException("Mu out of range (must be finite)");

      this._alpha = alpha;
      this._beta = beta;
      this._abe = abe;
      this._scale = scale;
      this._mu = location;

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

    public static bool IsValidAlpha(double alpha)
    {
      return alpha > 0 && alpha <= 2;
    }
    public static bool IsValidBeta(double beta)
    {
      return beta >= -1 && beta <= 1;
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
        if (_alpha < 1 && _beta == 1)
          return _mu;
        else
          return double.MinValue; 
      }
    }

    public override double Maximum
    {
      get 
      {
        if (_alpha < 1 && _beta == -1)
          return _mu;
        else
        return double.MaxValue; 
      }
    }

    public override double Mean
    {
      get { return _alpha <= 1 ? double.NaN : _mu; }
    }

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

    public override double NextDouble()
    {
      if (_beta == 0)
        return _mu + _scale * GenerateSymmetricCase(_alpha);
      else
      {
        if (_alpha == 1) // c * (X + beta * Math.Log(c) / M_PI_2);
          return (GenerateAsymmetricCaseS1_AEq1(_alpha, _beta)+_beta*Math.Log(_scale)*2/Math.PI) * _scale + _mu;
        else
          return GenerateAsymmetricCaseS1_ANe1(_alpha, _gen_t, _gen_B, _gen_S, _scale) + _mu;
      }
    }

    public override double PDF(double x)
    {
      return PDF(x, _alpha, _beta, _abe, _scale, _mu, ref _tempStorePDF, DefaultPrecision);
    }

    public override double CDF(double x)
    {
      return CDF(x, _alpha, _beta, _abe, _scale, _mu, ref _tempStorePDF, DefaultPrecision);
    }

    public override double Quantile(double p)
    {
      return _mu + _scale * Quantile(p, _alpha, _beta, _abe);
    }

    #endregion

    #region PDF dispatcher

    public static double PDF(double x, double alpha, double beta)
    {
      object tempStore = null;
      return PDF(x, alpha, beta, 1, 0, ref tempStore, Math.Sqrt(DoubleConstants.DBL_EPSILON));
    }

    public static double PDF(double x, double alpha, double beta, double sigma, double mu)
    {
      object tempStore = null;
      return PDF(x, alpha, beta, sigma, mu, ref tempStore, Math.Sqrt(DoubleConstants.DBL_EPSILON));
    }

    
    public static double PDF(double x, double alpha, double beta, double sigma, double mu, ref object tempStorage, double precision)
    {
      double abe;
      if (beta >= 0)
        abe = 1 - beta;
      else
        abe = 1 + beta;

      return PDF(x, alpha, beta, abe, sigma, mu, ref tempStorage, precision);
    }
    
    public static double PDF(double x, double alpha, double beta, double abe, double sigma, double mu, ref object tempStorage, double precision)
    {
      // Test for special case of symmetric destribution, this can be handled much better 
      if (beta == 0)
        return StableDistributionSymmetric.PDF((x-mu)/sigma, alpha, ref tempStorage, precision)/sigma;

      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));
      if (!(beta >= -1 && beta <= 1))
        throw new ArgumentOutOfRangeException(string.Format("Beta must be in the range [-1,1], but was: {0}", beta));

      if (alpha != 1)
      {
        double gamma, aga, sigmaf, muf;
        ParameterConversionS1ToFeller(alpha, beta, abe, sigma, mu, out gamma, out aga, out sigmaf, out muf);
        return StableDistributionFeller.PDF((x - muf) / sigmaf, alpha, gamma, aga) / sigmaf;
      }
      else
      {
        double mu0 = mu + sigma * beta * 2 * Math.Log(sigma) / Math.PI;
        return StableDistributionS0.PDFMethodAlphaOne((x-mu0)/sigma, beta, abe, ref tempStorage, precision)/sigma;
      }
    }

    #endregion

    #region CDF dispatcher

    public static double CDF(double x, double alpha, double beta)
    {
      object tempStorage = null;
      double abe = GetAbeFromBeta(beta);
      return CDF(x, alpha, beta, abe, 1, 0, ref tempStorage, DefaultPrecision);
    }

    public static double CDF(double x, double alpha, double beta, double scale, double location)
    {
      object tempStorage = null;
      double abe = GetAbeFromBeta(beta);
      return CDF(x, alpha, beta, abe, scale, location, ref tempStorage, DefaultPrecision);
    }


    public static double CDF(double x, double alpha, double beta, ref object tempStorage, double precision)
    {
      double abe = GetAbeFromBeta(beta);
      return CDF(x, alpha, beta, abe, 1, 0, ref tempStorage, DefaultPrecision);
    }

    public static double CDF(double x, double alpha, double beta, double scale, double location, ref object tempStorage, double precision)
    {
      double abe = GetAbeFromBeta(beta);
      return CDF(x, alpha, beta, abe, scale, location, ref tempStorage, DefaultPrecision);
    }

    public static double CDF(double x, double alpha, double beta, double abe)
    {
      object temp = null;
      return CDF(x, alpha, beta, abe, 1, 0, ref temp, DefaultPrecision);
    }

    public static double CDF(double x, double alpha, double beta, double abe, double scale, double location)
    {
      object temp = null;
      return CDF(x, alpha, beta, abe, scale, location, ref temp, DefaultPrecision);
    }

    public static double CDF(double x, double alpha, double beta, double abe, double scale, double location, ref object tempStorage, double precision)
    {
      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));
      if (!(beta >= -1 && beta <= 1))
        throw new ArgumentOutOfRangeException(string.Format("Beta must be in the range [-1,1], but was: {0}", beta));


      if (alpha != 1)
      {
        double gamma, aga, sigmaf, muf;
        ParameterConversionS1ToFeller(alpha, beta, abe, scale, location, out gamma, out aga, out sigmaf, out muf);
        return StableDistributionFeller.CDF((x - muf) / sigmaf, alpha, gamma, aga, ref tempStorage, precision);
      }
      else
      {
        double mu0 = location + scale * beta * 2 * Math.Log(scale) / Math.PI;
        return StableDistributionS0.CDFMethodAlphaOne((x-mu0)/scale, beta, abe, ref tempStorage, precision);
      }
    }

    #endregion

    #region Quantile

    public static double Quantile(double p, double alpha, double beta)
    {
      object tempStorage = null;
      double abe = GetAbeFromBeta(beta);
      return Quantile(p, alpha, beta, abe, ref tempStorage, DefaultPrecision);
    }

    public static double Quantile(double p, double alpha, double beta, double abe)
    {
      object tempStorage = null;
      return Quantile(p, alpha, beta, abe, ref tempStorage, DefaultPrecision);
    }

    public static double Quantile(double p, double alpha, double beta, double abe, ref object tempStorage, double precision)
    {
      double xguess = Math.Exp(2 / alpha); // guess value for a nearly constant p value in dependence of alpha
      double x0 = -xguess;
      double x1 = xguess;

      object temp = tempStorage;
      double root = double.NaN;
      if (RootFinding.BracketRootByExtensionOnly(delegate(double x) { return CDF(x, alpha, beta, abe, 1, 0, ref temp, DefaultPrecision) - p; }, 0, ref x0, ref x1))
      {
        if (null != RootFinding.ByBrentsAlgorithm(delegate(double x) { return CDF(x, alpha, beta, abe, 1, 0, ref temp, DefaultPrecision) - p; }, x0, x1, 0, DoubleConstants.DBL_EPSILON, out root))
          root = double.NaN;
      }
      tempStorage = temp;

      return root;
    }

    #endregion

    #region Calculation of integration parameters

    public static void GetAlt1GnParameter(double x, double alpha, double beta, double abe,
                                          out double factorp, out double facdiv, out double dev, out double logPdfPrefactor)
    {
      double tan_pi_alpha_2 = TanXPiBy2(alpha);

      double aga;
      double gamma = GammaFromAlphaBetaTanPiA2(alpha, beta, abe, tan_pi_alpha_2, out aga);
      dev = Math.PI * (gamma < 0 ? 0.5 * aga : 1 - 0.5 * aga);
      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      facdiv = CosGammaPiBy2(alpha, gamma, aga); // Inverse part of the original factor without power
      factorp = x * facdiv; // part of the factor with power alpha/(alpha-1);
      logPdfPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * (x)));
    }

    public static void GetAlt1GpParameter(double x, double alpha, double beta, double abe,
                                          out double factorp, out double facdiv, out double dev, out double logPdfPrefactor)
    {
      double tan_pi_alpha_2 = TanXPiBy2(alpha);

      double aga;
      double gamma = GammaFromAlphaBetaTanPiA2(alpha, beta, abe, tan_pi_alpha_2, out aga);
      dev = Math.PI * (gamma >= 0 ? 0.5 * aga : 1 - 0.5 * aga);
      // double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      facdiv = CosGammaPiBy2(alpha, gamma, aga); // Inverse part of the original factor without power
      factorp = x * facdiv; // part of the factor with power alpha/(alpha-1);
      logPdfPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * x));
    }

    public static void GetAgt1GnParameter(double x, double alpha, double beta, double abe,
                                                 out double factorp, out double factorw, out double dev, out double logPrefactor)
    {
      double tan_pi_alpha_2 = TanXPiBy2(alpha);

      double aga;
      double gamma = GammaFromAlphaBetaTanPiA2(alpha, beta, abe, tan_pi_alpha_2, out aga);

      dev = Math.PI * (gamma < 0 ? 0.5 * aga : (0.5 * ((2 - alpha) + gamma)));
      if (dev < 0)
        dev = 0;

      //double factor = Math.Pow(xx, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      // we separate the factor in a power of 1/alpha-1 and the rest
      factorp = x * CosGammaPiBy2(alpha, gamma, aga); // Math.Cos(-gamma * 0.5 * Math.PI);
      factorw = x;

      logPrefactor = Math.Log(alpha / (Math.PI * Math.Abs(alpha - 1) * x));
    }


    #endregion

  }

}
