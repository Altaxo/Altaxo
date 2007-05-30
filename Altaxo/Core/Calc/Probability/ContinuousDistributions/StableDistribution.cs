using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Represents a stable distribution in Zolotarev's parametrization.
  /// </summary>
  public class StableDistributionS0
  {
    #region PDF
    public static double PDF(double x, double alpha, double beta)
    {
      object tempStorage = null;
      return PDF(x, alpha, beta, ref tempStorage);
    }

    
    public static double PDF(double x, double alpha, double beta, ref object tempStorage)
    {
      // test input parameter
      if (alpha <= 0 || alpha > 2)
        throw new ArgumentOutOfRangeException("Alpha must be in the range (0,2]");
      if(beta<-1 || beta>1)
        throw new ArgumentOutOfRangeException("Beta must be in the range [-1,1]");
      if (beta == -1 && x > 0)
        throw new ArgumentOutOfRangeException("If beta==-1, then x must be negative");
      if (beta == 1 && x < 0)
        throw new ArgumentOutOfRangeException("If beta==-1, then x must be positive");

      double zeta = -beta * Math.Tan(alpha * 0.5 * Math.PI);

      //throw new ApplicationException();
      if (alpha != 1)
      {
        if (IsXNearlyEqualToZeta(x,zeta))
        {
          double xi = Math.Atan(-zeta) / alpha;
          return Calc.GammaRelated.Gamma(1 + 1 / alpha) * Math.Cos(xi) / (Math.PI * Math.Pow(1 + zeta * zeta, 0.5 / alpha));
        }
        else if (x > zeta)
        {
          return PDFMethod1(x, alpha, beta, zeta, ref tempStorage);
        }
        else
        {
          return PDFMethod1(-x, alpha, -beta, -zeta,ref tempStorage);
        }
      }
      else // alpha == 1
      {
        if (beta == 0)
        {
          return 1 / (Math.PI * (1 + x * x));
        }
        else // beta != 0
        {
          return PDFMethod2(x, beta, ref tempStorage);
        }
      }

    }

    private static double PDFMethod1(double x, double alpha, double beta, double zeta, ref object tempStorage)
    {
      double xi = Math.Atan(-zeta) / alpha;
      double factor = Math.Pow(x - zeta, alpha / (alpha - 1)) * Math.Pow(Math.Cos(alpha * xi), 1 / (alpha - 1));
      double integrand = Integrate(delegate(double theta) { return PDFCore1(factor, alpha, xi, theta); }, -xi, 0.5*Math.PI,ref tempStorage);
      double pre = alpha / (Math.PI * Math.Abs(alpha - 1) * (x - zeta));
      return pre * integrand;
    }

    private static double PDFCore1(double factor, double alpha, double xi, double theta)
    {
      double r1 = Math.Pow(Math.Cos(theta) / Math.Sin(alpha * (theta + xi)), alpha / (alpha - 1));
      double r2 = Math.Cos(alpha * xi + (alpha - 1) * theta) / Math.Cos(theta);
      return factor * r1 * r2;
    }

    private static double PDFMethod2(double x, double beta, ref object tempStorage)
    {
      double factor = Math.Exp(0.5 * Math.PI * x / beta) * 2 / Math.PI;
      double integrand = Integrate(delegate(double theta) { return PDFCore2(factor, beta, theta); }, -0.5*Math.PI, 0.5 * Math.PI, ref tempStorage);
      double pre = 1 / (2*Math.Abs(beta));
      return pre * integrand;
    }
    private static double PDFCore2(double factor, double beta, double theta)
    {
      double r1 = (0.5 * Math.PI + beta * theta) / Math.Cos(theta);
      double r2 = Math.Exp((0.5 * Math.PI + beta * theta) * Math.Tan(theta) / beta);
      return factor * r1 * r2;
    }

    /// <summary>
    /// Integrates func*Exp(-func) from x0 to x1. It relies on the fact that func is monotonical increasing from x0 to x1.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    private static double Integrate(ScalarFunctionDD func, double x0, double x1, ref object tempStorage)
    {
      double xm = FindYEqualToOne(func, x0, x1);
      double result, abserr;
      Calc.Integration.QagpIntegration.Integration(delegate(double x) { double f = func(x);  return f * Math.Exp(-f); },
        new double[] { x0, xm, x1 }, 3, 0, 1e-6, 100, out result, out abserr, ref tempStorage);
      return result;
    }

    /// <summary>
    /// Finds the x where func(x)==1+-1E-5 between x<x0<x1 for a monoton increasing function func.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    private static double FindYEqualToOne(ScalarFunctionDD func, double x0, double x1)
    {
      double low = x0;
      double high = x1;
      double xm;
      for(;;)
      {
        xm = 0.5*(low + high);
        double y = func(xm);
        if (Math.Abs(y - 1) < 1E-5)
          break;
        else if (y < 1)
          low = xm;
        else
          high = xm;

        if ((high - low) < 1E-15)
          break;
      }
      return xm;
    }

    private static bool IsXNearlyEqualToZeta(double x, double zeta)
    {
      if (x == zeta)
        return true;
      if (x == 0)
        return Math.Abs(zeta) < DoubleConstants.DBL_EPSILON;
      if (zeta == 0)
        return Math.Abs(x) < DoubleConstants.DBL_EPSILON;
      else
        return Math.Abs(x - zeta) < Math.Max(Math.Abs(x), Math.Abs(zeta)) * 1000*DoubleConstants.DBL_EPSILON;
    }

    #endregion
  }
}
