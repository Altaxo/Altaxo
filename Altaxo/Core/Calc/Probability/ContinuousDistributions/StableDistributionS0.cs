using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Represents a stable distribution in Zolotarev's parametrization.
  /// </summary>
  public class StableDistributionS0 : StableDistributionBase
  {
    double _alpha;
    double _beta;

    double _mu;
    double _scale = 1;

    double _muOffsetToS1;

    object _tempStorePDF;
    static readonly double _pdfPrecision = Math.Sqrt(DoubleConstants.DBL_EPSILON);
   

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using a 
    ///   <see cref="StandardGenerator"/> as underlying random number generator.
    /// </summary>
    public StableDistributionS0()
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
    public StableDistributionS0(Generator generator)
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
    public StableDistributionS0(double alpha, double beta)
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
    public StableDistributionS0(double alpha, double beta, double sigma, double mu)
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
    public StableDistributionS0(double alpha, double beta, double sigma, double mu, Generator generator)
      : base(generator)
    {
      Initialize(alpha, beta, sigma, mu);
    }
    #endregion

    #region Distribution members
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
      ParameterConversionS0ToS1(_alpha, _beta, _scale, _mu, out _muOffsetToS1);
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
          return _mu - _scale * Math.Tan(_alpha * 0.5 * Math.PI);
        else
          return double.MinValue; 
      }
    }

    public override double Maximum
    {
      get {
        if (_alpha < 1 && _beta == -1)
          return _mu + _scale * Math.Tan(_alpha * 0.5 * Math.PI);
        else
          return double.MaxValue; 
      }
    }

    public override double Mean
    {
      get { return _alpha <= 1 ? double.NaN : _mu - _beta*_scale*Math.Tan(0.5*Math.PI*_alpha); }
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
        return GenerateSymmetricCase(_alpha) * _scale + _mu;
      else
      {
        if(_alpha==1)
          return GenerateAsymmetricCaseS1(_alpha, _beta, _scale) + _mu;
        else
          return GenerateAsymmetricCaseS1(_alpha, _beta, _scale) + _muOffsetToS1;
      }
    }

    public override double PDF(double x)
    {
      return PDF(x, _alpha, _beta, _scale, _mu, ref _tempStorePDF, _pdfPrecision);
    }
    #endregion

    #region PDF

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
      double abe = beta >= 0 ? 1 - beta : 1 + beta;
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
        ParameterConversionS0ToFeller(alpha, beta, abe, 1, 0, out gamma, out aga, out sigmaf, out muf);
        return StableDistributionFeller.PDF((x - muf) / sigmaf, alpha, gamma,aga) / sigmaf;
      }
      else
      {
        return PDFMethodAlphaOne(x, beta, abe, ref tempStorage, precision);
      }
    }

    public static double CDF(double x, double alpha, double beta)
    {
      double abe = beta >= 0 ? 1 - beta : 1 + beta;
      return CDF(x, alpha, beta, abe);
    }

    public static double CDF(double x, double alpha, double beta, double abe)
    {
      object temp = null;
      return CDF(x,alpha,beta,abe,ref temp,Math.Sqrt(DoubleConstants.DBL_EPSILON));
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
        ParameterConversionS0ToFeller(alpha, beta, abe, 1, 0, out gamma, out aga, out sigmaf, out muf);
        return StableDistributionFeller.CDF((x - muf) / sigmaf, alpha, gamma, aga,ref tempStorage,precision);
      }
      else
      {
        return CDFMethodAlphaOne(x, beta, abe, ref tempStorage, precision);
      }
    }

    

    private static double PDFMethod1(double x, double alpha, double beta, double zeta, ref object tempStorage, double precision)
    {
      double xi = Math.Atan(-zeta) / alpha;
      double factor = Math.Pow(x - zeta, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      double integrand = IntegrateFuncExpMFunc(delegate(double theta) { return PDFCore1(factor, alpha, xi, theta); }, -xi, 0.5 * Math.PI, alpha > 1, ref tempStorage, precision);
      double pre = alpha / (Math.PI * Math.Abs(alpha - 1) * (x - zeta));
      return pre * integrand;
    }

    private static double PDFCore1(double factor, double alpha, double xi, double theta)
    {
      double r1 = Math.Pow(Math.Cos(theta) / Math.Sin(alpha * (theta + xi)), alpha / (alpha - 1));
      double r2 = Math.Cos(alpha * xi + (alpha - 1) * theta) / Math.Cos(theta);
      double result = factor * r1 * r2;
      return result < 0 ? 0 : result; // the result should be always positive, if not this is due to numerical inaccuracies near the borders, we can safely set the result 0 then
    }

    public static double PDFMethodAlphaOne(double x, double beta, double abe, ref object tempStorage, double precision)
    {
      if (0 == beta)
      {
        return 1 / (Math.PI * (1 + x * x));
      }
      else
      {

        Aeq1BpI inc = new Aeq1BpI(x, beta, abe);
        if (inc.IsMaximumLeftHandSide())
        {
          return inc.Integrate(ref tempStorage, precision);
        }
        else
        {
          Aeq1BpD dec = new Aeq1BpD(x, beta, abe);
          return dec.Integrate(ref tempStorage, precision);
        }
      }
    }

    public static double CDFMethodAlphaOne(double x, double beta, double abe, ref object tempStorage, double precision)
    {
      if (0 == beta)
      {
        return 0.5 + Math.Atan(x) / Math.PI;
      }
      else
      {
        Aeq1BpI inc = new Aeq1BpI(x, beta, abe);

        if (beta > 0)
          return inc.CDFIntegrate(ref tempStorage, precision);
        else
          return inc.CDFIntegrate(ref tempStorage, precision);
      }
    }

    public class Aeq1BpI
    {
      double beta;
      double abe;
      double logFactorp;
      double logPdfPrefactor;
      double _x0;

      public Aeq1BpI(double x, double beta, double abe)
      {
        this.beta = beta;
        this.abe = abe;
        this.logFactorp = -0.5 * Math.PI * x / beta;
        this.logPdfPrefactor = -Math.Log(2 * Math.Abs(beta));
      }

      public double PDFCore(double thetas)
      {
        double r1;
        double r2;
        if (abe == 0 && thetas < 1E-10)
        {
          r1 = 2 * Math.Exp(logFactorp) / (Math.PI * Math.E);
          r2 = 1;
        }
        else
        {
          double abeta = Math.Abs(beta);
          double h = Math.PI * abe + 2 * abeta * thetas;
          r1 = Math.Exp(logFactorp - h / (2 * abeta * Math.Tan(thetas)));
          r2 = h / (Math.PI * Math.Sin(thetas));
        }
        double result = r1 * r2;
        if (!(result >= 0))
          result = double.MaxValue;

        //System.Diagnostics.Debug.WriteLine(string.Format("CorAlt1GnI theta={0}, result={1}", thetas, result));
        return result;
      }


      public double PDFCoreDerivativeByCore(double thetas)
      {
        double abeta = Math.Abs(beta);
        double c1 = (abe * Math.PI / (2 * abeta) + thetas);

        double s1 = 1 / c1;
        double s2 = -2 / Math.Tan(thetas);
        double s3 = c1*RMath.Pow(Math.Sin(thetas), -2);
        
        return s1 + s2 + s3;
      }


      public double PDFFunc(double x)
      {
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(-f + logPdfPrefactor);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double PDFFuncLogInt(double z)
      {
        double x = Math.Exp(z);
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(z - f + logPdfPrefactor);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double PDFFuncLogIntToLeft(double z)
      {
        double x = _x0 - Math.Exp(z);
        if (x < 0)
          x = 0;

        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(z - f + logPdfPrefactor);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }
      public double PDFFuncLogIntToRight(double z)
      {
        double x = _x0 + Math.Exp(z);
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(z - f + logPdfPrefactor);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double Integrate(ref object tempStorage, double precision)
      {
        if (abe == 0)
          return IntegrateAbeEqZero(ref tempStorage, precision);
        else
          return IntegrateAbeNotEqZero(ref tempStorage, precision);
      }

      public double IntegrateAbeNotEqZero(ref object tempStorage, double precision)
      {
        GSL_ERROR error;
        double abserr;
        double result;
        double x1 = UpperIntegrationLimit;
        
        double ym;
        double xm = FindIncreasingYEqualTo(PDFCore, 0, x1, 1, 0.125, out ym);

        try
        {
          double xwidth = 1/(PDFCoreDerivativeByCore(xm)*ym);
          if(xwidth*8 < xm)
          {
            _x0 = xm + xwidth;
            error = Calc.Integration.QagpIntegration.Integration(
               PDFFuncLogIntToLeft,
               new double[] { Math.Log(xwidth), Math.Log(xm+xwidth) }, 2,
               0, precision, 100, out result, out abserr, ref tempStorage);

            if (error == null)
            {
              double result1;
              _x0 = xm - xwidth;
              error = Calc.Integration.QagpIntegration.Integration(
               PDFFuncLogIntToRight,
               new double[] { Math.Log(xwidth), Math.Log(x1-_x0) }, 2,
               0, precision, 100, out result1, out abserr, ref tempStorage);

              result += result1;
            }
          }
          else if(xm*8<(x1-xm))
          {
            error = Calc.Integration.QagpIntegration.Integration(
               PDFFunc,
               new double[] { 0, xm }, 2,
               0, precision, 100, out result, out abserr, ref tempStorage);
           
            if (error == null)
            {
              double result1;
              error = Calc.Integration.QagpIntegration.Integration(
               PDFFuncLogInt,
               new double[] { Math.Log(xm), Math.Log(x1) }, 2,
               0, precision, 100, out result1, out abserr, ref tempStorage);

              result += result1;
            }
          }
          else
          {
            error = Calc.Integration.QagpIntegration.Integration(
             PDFFunc,
             new double[] { 0, xm, x1 }, 3,
             0, precision, 100, out result, out abserr, ref tempStorage);
          }
          if (null != error)
            result = double.NaN;

          return result;
        }
        catch (Exception ex)
        {
          return double.NaN;
        }
      }


      public double IntegrateAbeEqZero(ref object tempStorage, double precision)
      {
        GSL_ERROR error;
        double abserr;
        double result;
        double x1 = UpperIntegrationLimit;
        double xm = 1e-10;

        try
        {
           error = Calc.Integration.QagpIntegration.Integration(
               PDFFunc,
               new double[] { 0, xm }, 2,
               0, precision, 100, out result, out abserr, ref tempStorage);

            if (error == null)
            {
              double result1;
              error = Calc.Integration.QagpIntegration.Integration(
               PDFFuncLogInt,
               new double[] { Math.Log(xm), Math.Log(x1) }, 2,
               0, precision, 100, out result1, out abserr, ref tempStorage);

              result += result1;
            }
        
          if (null != error)
            result = double.NaN;

          return result;
        }
        catch (Exception ex)
        {
          return double.NaN;
        }
      }

      public double UpperIntegrationLimit
      {
        get
        {
          return Math.PI;
        }
      }

      public bool IsMaximumLeftHandSide()
      {
        return PDFCore(0.5 * UpperIntegrationLimit) > 1;
      }

      #region CDF

      public double CDFFunc(double x)
      {
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : Math.Exp(-f + logPdfPrefactor);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double CDFFuncLogInt(double z)
      {
        double x = Math.Exp(z) + _x0;
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : Math.Exp(z - f + logPdfPrefactor);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double CDFIntegrate(ref object tempStorage, double precision)
      {
        GSL_ERROR error1;
        double resultRight, abserrRight;

        double xm, yfound;
        if (abe == 0)
          xm = FindIncreasingYEqualTo(PDFCore, 0, UpperIntegrationLimit, PDFCore(0) + 1, 0.1, out yfound);
        else
          xm = FindIncreasingYEqualToOne(PDFCore, 0, UpperIntegrationLimit);

        if ((xm * 10) < UpperIntegrationLimit)
        {
          // logarithmical integration
          _x0 = -xm;
          // now integrate logarithmically
          error1 = Calc.Integration.QagpIntegration.Integration(
   CDFFuncLogInt,
   new double[] { Math.Log(xm), Math.Log(UpperIntegrationLimit + xm) }, 2, 0, precision, 100, out resultRight, out abserrRight, ref tempStorage);

        }
        else // linear integration
        {
          error1 = Calc.Integration.QagpIntegration.Integration(
   CDFFunc,
   new double[] { 0, UpperIntegrationLimit }, 2, 0, precision, 100, out resultRight, out abserrRight, ref tempStorage);

        }

        if (null != error1)
          return double.NaN;
        else
          return resultRight;
      }

      #endregion
    }

    public class Aeq1BpD
    {
      double beta;
      double abe;
      double logFactorp;
      double logPdfPrefactor;
      double _x0;

      public Aeq1BpD(double x, double beta, double abe)
      {
        this.beta = beta;
        this.abe = abe;
        this.logFactorp = -0.5 * Math.PI * x / beta;
        this.logPdfPrefactor = -Math.Log(2 * Math.Abs(beta));
      }

      public double PDFCore(double thetas)
      {
        double r1;
        double r2;
       
        double abeta = Math.Abs(beta);
        double h = Math.PI * (2-abe) - 2 * abeta * thetas;
        r1 = Math.Exp(logFactorp + h / (2 * abeta * Math.Tan(thetas)));
        r2 = h / (Math.PI * Math.Sin(thetas));
        double result = r1 * r2;
        if (!(result >= 0))
          result = double.MaxValue;

        //System.Diagnostics.Debug.WriteLine(string.Format("CorAlt1GnI theta={0}, result={1}", thetas, result));
        return result;
      }

      public double PDFCoreDerivativeByCore(double thetas)
      {
        double abeta = Math.Abs(beta);
        double c1 = 2 * abeta / (Math.PI * (1+abeta) -  abeta * thetas);

        double s1 = c1;
        double s2 = 2 / Math.Tan(thetas);
        double s3 = 1/(c1 * RMath.Pow(Math.Sin(thetas), 2));
        return s1 + s2 + s3;
      }

      public double PDFFunc(double x)
      {
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(-f + logPdfPrefactor);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double PDFFuncLogInt(double z)
      {
        double x = Math.Exp(z);
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(z - f + logPdfPrefactor);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double PDFFuncLogIntToLeft(double z)
      {
        double x = _x0 - Math.Exp(z);
        if (x < 0)
          x = 0;

        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(z - f + logPdfPrefactor);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }
      public double PDFFuncLogIntToRight(double z)
      {
        double x = _x0 + Math.Exp(z);
        double f = PDFCore(x);
        double r = double.IsInfinity(f) ? 0 : f * Math.Exp(z - f + logPdfPrefactor);
        //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
        //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
        return r;
      }

      public double Integrate(ref object tempStorage, double precision)
      {
        GSL_ERROR error;
        double abserr;
        double result;
        double x1 = UpperIntegrationLimit;
        double ym;
        double xm = FindDecreasingYEqualTo(PDFCore, 0, x1, 1, 0.125, out ym);
        try
        {
          double xwidth = 1 / (PDFCoreDerivativeByCore(xm) * ym);
          if (xwidth * 8 < xm)
          {
            _x0 = xm + xwidth;
            error = Calc.Integration.QagpIntegration.Integration(
               PDFFuncLogIntToLeft,
               new double[] { Math.Log(xwidth), Math.Log(xm + xwidth) }, 2,
               0, precision, 100, out result, out abserr, ref tempStorage);

            if (error == null)
            {
              double result1;
              _x0 = xm - xwidth;
              error = Calc.Integration.QagpIntegration.Integration(
               PDFFuncLogIntToRight,
               new double[] { Math.Log(xwidth), Math.Log(x1 - _x0) }, 2,
               0, precision, 100, out result1, out abserr, ref tempStorage);

              result += result1;
            }
          }
          else if (xm * 8 < (x1 - xm))
          {
            error = Calc.Integration.QagpIntegration.Integration(
               PDFFunc,
               new double[] { 0, xm }, 2,
               0, precision, 100, out result, out abserr, ref tempStorage);
            if (error == null)
            {
              double result1;
              error = Calc.Integration.QagpIntegration.Integration(
               PDFFuncLogInt,
               new double[] { Math.Log(xm), Math.Log(x1) }, 2,
               0, precision, 100, out result1, out abserr, ref tempStorage);

              result += result1;
            }
          }
          else
          {
            error = Calc.Integration.QagpIntegration.Integration(
             PDFFunc,
             new double[] { 0, xm, x1 }, 3,
             0, precision, 100, out result, out abserr, ref tempStorage);
          }
          if (null != error)
            result = double.NaN;

          return result;
        }
        catch (Exception ex)
        {
          return double.NaN;
        }
      }

      public double UpperIntegrationLimit
      {
        get
        {
          return Math.PI;
        }
      }

      public bool IsMaximumLeftHandSide()
      {
        return PDFCore(0.5 * UpperIntegrationLimit) > 1;
      }
    }

    #endregion

    #region CDF

    /*
    public static double CDF(double x, double alpha, double beta)
    {
      object tempStorage = null;
      return CDF(x, alpha, beta, ref tempStorage);
    }

    public static double CDF(double x, double alpha, double beta, ref object tempStorage)
    {
      // test input parameter
      if (!(alpha > 0 && alpha <= 2))
        throw new ArgumentOutOfRangeException(string.Format("Alpha must be in the range (0,2], but was: {0}", alpha));
      if (!(beta >= -1 && beta <= 1))
        throw new ArgumentOutOfRangeException(string.Format("Beta must be in the range [-1,1], but was: {0}", beta));
      // if (beta == -1 && x > 0)
      // return 0;
      //if (beta == 1 && x < 0)
      //return 0;

      double zeta = -beta * Math.Tan(alpha * 0.5 * Math.PI);

      //throw new ApplicationException();
      if (alpha != 1)
      {
        if (IsXNearlyEqualToZeta(x, zeta))
        {
          return (Math.PI / 2 - zeta) / Math.PI;
        }
        else if (x > zeta)
        {
          return CDFMethod1(x, alpha, beta, zeta, ref tempStorage);
        }
        else
        {
          return 1 - CDFMethod1(-x, alpha, -beta, -zeta, ref tempStorage);
        }
      }
      else // alpha == 1
      {
        if (beta == 0)
        {
          return 0.5 + Math.Atan(x) / Math.PI;
        }
        else if (beta > 0)
        {
          return CDFMethod2(x, beta, ref tempStorage);
        }
        else // beta < 0
        {
          return CDFMethod2(x, -beta, ref tempStorage);
        }
      }

    }
     */

    private static double CDFMethod1(double x, double alpha, double beta, double zeta, ref object tempStorage)
    {
      double xi = Math.Atan(-zeta) / alpha;
      double factor = Math.Pow(x - zeta, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      double integrand = CDFIntegrate(
        delegate(double theta)
        {
          return PDFCore1(factor, alpha, xi, theta);
        },
        -xi, 0.5 * Math.PI, alpha > 1, ref tempStorage);

      double pre = Math.Sign(1 - alpha) / Math.PI;
      double plus = alpha > 1 ? 1 : (Math.PI / 2 - zeta) / Math.PI;
      return plus + pre * integrand;
    }

    private static double CDFMethod2(double x, double beta, ref object tempStorage)
    {
      double factor = Math.Exp(-0.5 * Math.PI * x / beta) * 2 / Math.PI;
      double integrand = CDFIntegrate(delegate(double theta) { return PDFCoreAlphaOne(factor, beta, theta); }, -0.5 * Math.PI, 0.5 * Math.PI, beta < 0, ref tempStorage);
      double pre = 1 / Math.PI;
      return pre * integrand;
    }

    /// <summary>
    /// Integrates Exp(-func) from x0 to x1. It relies on the fact that func is monotonical increasing from x0 to x1, so that the maximum 
    /// of Exp(-func) is at x0.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    private static double CDFIntegrate(ScalarFunctionDD func, double x0, double x1, bool isDecreasing, ref object tempStorage)
    {
      double result = 0, abserr = 0;
      try
      {
        Calc.Integration.QagpIntegration.Integration(
          delegate(double x)
          {
            double f = func(x);
            double r = double.IsInfinity(f) ? 0 : Math.Exp(-f);
            //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
            //Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
            return r;
          },
          new double[] { x0, x0, x1 }, 3, 0, 1e-6, 100, out result, out abserr, ref tempStorage);
        return result;
      }
      catch (Exception ex)
      {
        return result;
      }
    }

    #endregion

    #region Quantile

    public static double Quantile(double p, double alpha, double beta)
    {
      double xguess = Math.Exp(2 / alpha); // guess value for a nearly constant p value in dependence of alpha
      double x0 = -xguess;
      double x1 = xguess;

      double abe = beta >= 0 ? 1 - beta : 1 + beta;

      object tempStore = null;
      if (RootFinding.BracketRootByExtensionOnly(delegate(double x) { return CDF(x, alpha, beta, abe, ref tempStore, DefaultPrecision) - p; }, 0, ref x0, ref x1))
      {
        double root;
        if (null == RootFinding.ByBrentsAlgorithm(delegate(double x) { return CDF(x, alpha, beta, abe, ref tempStore, DefaultPrecision) - p; }, x0, x1, 0, DoubleConstants.DBL_EPSILON, out root))
          return root;
      }
      return double.NaN;
    }

    #endregion


  }

 

 
}
