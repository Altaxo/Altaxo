using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Probability
{
  public class StableDistributionS1 : StableDistributionBase
  {

    double _alpha;
    double _beta;

    double _mu;
    double _scale = 1;

    object _tempStorePDF;
    static readonly double _pdfPrecision = Math.Sqrt(DoubleConstants.DBL_EPSILON);

     #region construction
    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using a 
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public StableDistributionS1()
      : this(DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using the specified 
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public StableDistributionS1(Generator generator)
      : this(1, 0, 1, 0, generator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using the specified 
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public StableDistributionS1(double alpha, double beta)
      : this(alpha, beta, 1, 0, DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using the specified 
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public StableDistributionS1(double alpha, double beta, double sigma, double mu)
      : this(alpha, beta, sigma, mu, DefaultGenerator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using the specified 
    ///   <see cref="Generator"/> as underlying random number generator.
    /// </summary>
    /// <param name="generator">A <see cref="Generator"/> object.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public StableDistributionS1(double alpha, double beta, double sigma, double mu, Generator generator)
      : base(generator)
    {
      Initialize(alpha, beta, sigma, mu);
    }
    #endregion

    #region Distribution properties

    /// <summary>
    /// Updates the helper variables that store intermediate results for generation of exponential distributed random 
    ///   numbers.
    /// </summary>
    public void Initialize(double alpha, double beta, double sigma, double mu)
    {
      if (!IsValidAlpha(alpha))
        throw new ArgumentOutOfRangeException("Alpha out of range (must be greater 0.1 and smalle or equal than 2)");
      if (!IsValidBeta(beta))
        throw new ArgumentOutOfRangeException("Beta out of range (must be in the range [-1,1])");
      if (!IsValidSigma(sigma))
        throw new ArgumentOutOfRangeException("Sigma out of range (must be >0)");
      if (!IsValidMu(mu))
        throw new ArgumentOutOfRangeException("Mu out of range (must be finite)");

      this._alpha = alpha;
      this._beta = beta;
      this._scale = sigma;
      this._mu = mu;
    }

    public static bool IsValidAlpha(double alpha)
    {
      return alpha > 0 && alpha <= 2;
    }
    public static bool IsValidBeta(double beta)
    {
      return beta >= -1 && beta <= 1;
    }
    public static bool IsValidSigma(double sigma)
    {
      return sigma > 0;
    }
    public static bool IsValidMu(double mu)
    {
      return mu >= double.MinValue && mu <= double.MaxValue;
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
        return _mu + _scale*GenerateSymmetricCase(_alpha);
      else
        return _mu + GenerateAsymmetricCaseS1(_alpha, _beta, _scale);
    }

    public override double PDF(double x)
    {
      return PDF(x, _alpha, _beta, _scale, _mu, ref _tempStorePDF, _pdfPrecision);
    }

    #endregion

    #region PDF dispatcher

    public static double PDF(double x, double alpha, double beta)
    {
      object tempStore = null;
      return PDF(x, alpha, beta, ref tempStore, Math.Sqrt(DoubleConstants.DBL_EPSILON));
    }

    public static double PDF(double x, double alpha, double beta, double sigma, double mu)
    {
      object tempStore = null;
      return PDF(x, alpha, beta, sigma, mu, ref tempStore, Math.Sqrt(DoubleConstants.DBL_EPSILON));
    }

    public static double PDF(double x, double alpha, double beta, double sigma, double mu, ref object tempStorage, double precision)
    {
      return PDF((x - mu) / sigma, alpha, beta, ref tempStorage, precision) / sigma;
    }
    
    public static double PDF(double x, double alpha, double beta, ref object tempStorage, double precision)
    {
      double abe;
      if (beta >= 0)
        abe = 1 - beta;
      else
        abe = -1 + beta;

      return PDF(x, alpha, beta, abe, ref tempStorage, precision);
    }
    
    public static double PDF(double x, double alpha, double beta, double abe, ref object tempStorage, double precision)
    {
      // Test for special case of symmetric destribution, this can be handled much better 
      if (beta == 0)
        return StableDistributionSymmetric.PDF(x, alpha, ref tempStorage, precision);

      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));
      if (!(beta >= -1 && beta <= 1))
        throw new ArgumentOutOfRangeException(string.Format("Beta must be in the range [-1,1], but was: {0}", beta));

      if (alpha != 1)
      {
        double gamma, aga, sigmaf, muf;
        ParameterConversionS1ToFeller(alpha, beta, abe, 1, 0, out gamma, out aga, out sigmaf, out muf);
        return StableDistributionFeller.PDF((x - muf) / sigmaf, alpha, gamma, aga) / sigmaf;
      }
      else
      {
        return StableDistributionS0.PDFMethodAlphaOne(x, beta, abe, ref tempStorage, precision);
      }
    }

    #endregion

    #region CDF dispatcher

    public static double CDF(double x, double alpha, double gamma)
    {
      object tempStorage = null;
      double aga = StableDistributionFeller.GetAgaFromAlphaGamma(alpha, gamma);
      return CDF(x, alpha, gamma, aga, ref tempStorage, DefaultPrecision);
    }

    public static double CDF(double x, double alpha, double gamma, ref object tempStorage, double precision)
    {
      double aga = StableDistributionFeller.GetAgaFromAlphaGamma(alpha, gamma);
      return CDF(x, alpha, gamma, aga, ref tempStorage, precision);
    }

    public static double CDF(double x, double alpha, double gamma, double aga)
    {
      object temp = null;
      return CDF(x, alpha, gamma, aga, ref temp, DefaultPrecision);
    }

    public static double CDF(double x, double alpha, double beta, double abe, ref object tempStorage, double precision)
    {
      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));
      if (!(beta >= -1 && beta <= 1))
        throw new ArgumentOutOfRangeException(string.Format("Beta must be in the range [-1,1], but was: {0}", beta));


      if (alpha != 1)
      {
        double gamma, aga, sigmaf, muf;
        ParameterConversionS1ToFeller(alpha, beta, abe, 1, 0, out gamma, out aga, out sigmaf, out muf);
        return StableDistributionFeller.CDF((x - muf) / sigmaf, alpha, gamma, aga, ref tempStorage, precision);
      }
      else
      {
        return StableDistributionS0.CDFMethodAlphaOne(x, beta, abe, ref tempStorage, precision);
      }
    }

    #endregion

  }

}
