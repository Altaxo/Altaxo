using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Probability
{
  public class StableDistributionBase : ContinuousDistribution
  {
    ExponentialDistribution _expDist = new ExponentialDistribution();
    ContinuousUniformDistribution _contDist = new ContinuousUniformDistribution();

    /// <summary>The highest number x that, when taken Exp(-x), gives a result greater than zero.</summary>
    protected static readonly double MinusLogTiny = -Math.Log(double.Epsilon);

    #region Abstract Implementation of ContinuousDistribution

    protected StableDistributionBase(Generator generator)
      : base(generator)
    {
      _expDist = new ExponentialDistribution(generator);
      _contDist = new ContinuousUniformDistribution(generator);
    }

    public override double Minimum
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public override double Maximum
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public override double Mean
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public override double Median
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public override double Variance
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public override double[] Mode
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public override double NextDouble()
    {
      throw new Exception("The method or operation is not implemented.");
    }
    #endregion



    #region Integration

    /// <summary>
    /// Integrates func*Exp(-func) from x0 to x1. It relies on the fact that func is monotonical increasing from x0 to x1.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    protected static double IntegrateFuncExpMFunc(ScalarFunctionDD func, double x0, double x1, bool isDecreasing, ref object tempStorage, double precision)
    {
      try
      {
        double xm = isDecreasing ? FindDecreasingYEqualToOne(func, x0, x1) : FindIncreasingYEqualToOne(func, x0, x1);
        double result = 0, abserr = 0;
        double[] intgrenzen;

        if (isDecreasing)
        {
          // if xm is very near to the upper boundary x1, we add another point which should be roughly 4*(x1-xm)
          // this is to make sure the bisection in the left interval (x0,xm) is fine enough because here
          // the function is very fast decreasing
          double xdiff = x1 - xm;
          if (x1 - 8 * xdiff > x0)
          {
            if (xdiff <= 0)
              xdiff = (x1 - x0) * DoubleConstants.DBL_EPSILON;

            double n = Math.Floor(Math.Log((x1 - x0) / xdiff) / Math.Log(4));
            if (n > 100)
              n = 100;

            if (n >= 2)
            {
              int nn = (int)n;
              intgrenzen = new double[nn + 3];
              intgrenzen[0] = x0;
              intgrenzen[nn + 1] = xm;
              intgrenzen[nn + 2] = x1;
              double fac = 4;
              for (int i = nn; i > 0; i--, fac *= 4)
                intgrenzen[i] = x1 - fac * xdiff;

              // in case of roundoff errors intgrenzen[1] can be lower than the lower integration limit,
              // in that case simply use half the way between the neighbours
              if (intgrenzen[1] <= intgrenzen[0])
                intgrenzen[1] = 0.5 * (intgrenzen[0] + intgrenzen[2]);
            }
            else
            {
              intgrenzen = new double[] { x0, x1 - 4 * xdiff, xm, x1 };
            }
          }
          else
            intgrenzen = new double[] { x0, xm, x1 };
        }
        else // it is increasing
        {
          // if xm is very near to the lower boundary, we add another point which should be roughly 4*xm
          // this is to make sure the bisection in the right interval (xm, x1) is fine enough because here
          // the function is very fast decreasing
          double xdiff = xm - x0;
          if (x0 + 8 * xdiff < x1)
          {
            if (xdiff <= 0)
              xdiff = (x1 - x0) * DoubleConstants.DBL_EPSILON;

            double n = Math.Floor(Math.Log((x1 - x0) / xdiff) / Math.Log(4));
            if (n > 100)
              n = 100;

            if (n >= 2)
            {
              int nn = (int)n;
              intgrenzen = new double[nn + 3];
              intgrenzen[0] = x0;
              intgrenzen[1] = xm;
              intgrenzen[nn + 2] = x1;
              double fac = 4;
              for (int i = 2; i < nn + 2; i++, fac *= 4)
                intgrenzen[i] = x0 + fac * xdiff;

              // in case of roundoff errors intgrenzen[nn+1] can be bigger than the upper integration limit,
              // in that case simply use half the way between the neighbours
              if (intgrenzen[nn + 1] >= intgrenzen[nn + 2])
                intgrenzen[nn + 1] = 0.5 * (intgrenzen[nn] + intgrenzen[nn + 2]);
            }
            else
            {
              intgrenzen = new double[] { x0, xm, x0 + 4 * xdiff, x1 };
            }
          }
          else
          {
            intgrenzen = new double[] { x0, xm, x1 };
          }
        }

        try
        {
          GSL_ERROR error = Calc.Integration.QagpIntegration.Integration(
            delegate(double x)
            {
              double f = func(x);
              if (f < 0)
                throw new ArithmeticException("Function value < 0 at x=" + x.ToString());

              double r = double.IsInfinity(f) ? 0 : f * Math.Exp(-f);
              //System.Diagnostics.Debug.WriteLine(string.Format("x={0}, f={1}, r={2}", x, f, r));
              //Current.Console.WriteLine("x={0}, f={1}, r={2}", x, f, r);
              return r;
            },
            intgrenzen, intgrenzen.Length, 0, precision, 100, out result, out abserr, ref tempStorage);

          if (null != error)
            result = double.NaN;

          return result;
        }
        catch (Exception ex)
        {
          return result;
        }
      }
      catch (Exception ex)
      {
        return double.NaN;
      }
    }

    protected static int GetIntegrationPointsXmNearX0(double x0, double xm, double x1, double[] intgrenzen)
    {
      int used;
      double xdiffl = xm - x0;
      if (xdiffl <= 0)
      {
        used = PartFromTheLeft(xm, x1, xdiffl, intgrenzen, 0);
      }
      else if ((xm - x0) < (x1 - xm))
      {
        intgrenzen[0] = x0;
        used = 1 + PartFromTheLeft(xm, x1, xdiffl, intgrenzen, 1);
      }
      else
      {
        intgrenzen[0] = x0;
        intgrenzen[1] = x1;
        used = 2;
      }
      return used;
    }

    /*
    protected static double[] GetIntegrationPointsXmNearX0(double x0, double xm, double x1)
    {
      const int MaxDivisions = 30;
      const double MaxScale = 100;

      double[] intgrenzen;

      // if xm is very near to the lower boundary, we add another point which should be roughly 4*xm
      // this is to make sure the bisection in the right interval (xm, x1) is fine enough because here
      // the function is very fast decreasing


      double xdiff = xm - x0;
      double n;

      if (xdiff <= 0 || (n=Math.Floor(Math.Log((x1 - x0) / xdiff) / Math.Log(4)))>100)
      {
        // we calculate the subintervals for a given scale of 100
        xdiff = (MaxScale-1)*(x1-x0)/(-1+RMath.Pow(MaxScale,MaxDivisions+2));
        intgrenzen = new double[MaxDivisions + 3];
        intgrenzen[0] = x0;
        intgrenzen[MaxDivisions + 2] = x1;

        double interval = xdiff;
        for (int i = 1; i < MaxDivisions + 2; i++)
        {
          intgrenzen[i] = x0 + interval;
          interval *= MaxScale;
        }
      }
      else if (x0 + 8 * xdiff < x1)
      {
        if (n > 30)
        {
          // the first interval should be roughly xdiff
          // so we try to
          double scale = Math.Pow((x1 - xm) / xdiff + 1, 1.0 / (MaxDivisions + 1));
          intgrenzen = new double[MaxDivisions + 3];
          intgrenzen[0] = x0;
          intgrenzen[1] = xm;
          intgrenzen[MaxDivisions + 2] = x1;

          double interval = xdiff;
          for (int i = 2; i < MaxDivisions + 2; i++)
          {
            intgrenzen[i] = xm + interval;
            interval *= scale;
          }
        }
        else if (n >= 2)
        {
          int nn = (int)n;
          intgrenzen = new double[nn + 3];
          intgrenzen[0] = x0;
          intgrenzen[1] = xm;
          intgrenzen[nn + 2] = x1;
          double fac = 4;
          for (int i = 2; i < nn + 2; i++, fac *= 4)
            intgrenzen[i] = x0 + fac * xdiff;

          // in case of roundoff errors intgrenzen[nn+1] can be bigger than the upper integration limit,
          // in that case simply use half the way between the neighbours
          if (intgrenzen[nn + 1] >= intgrenzen[nn + 2])
            intgrenzen[nn + 1] = 0.5 * (intgrenzen[nn] + intgrenzen[nn + 2]);
        }
        else
        {
          intgrenzen = new double[] { x0, xm, x0 + 4 * xdiff, x1 };
        }
      }
      else
      {
        intgrenzen = new double[] { x0, xm, x1 };
      }

      return intgrenzen;
    }
    */
    static double[] AddIntegrationPointsToTheEndInc(double x0, double x1, ScalarFunctionDD func, double[] intgrenzen, double yatlastdiv)
    {
      const int additionalPoints = 10;

      // the last interval of the original intgrenzen is to long and must be further divided
      double xlast = intgrenzen[intgrenzen.Length - 2];

      double ynext;
      double xnext = FindIncreasingYEqualTo(func, xlast, x1, yatlastdiv + 60, 1, out ynext);


      double[] result = new double[intgrenzen.Length + additionalPoints];
      Array.Copy(intgrenzen, result, intgrenzen.Length);
      result[result.Length - 1] = x1;

      double interval = (xnext - xlast) / additionalPoints;
      for (int i = 0; i < additionalPoints; i++)
        result[result.Length - 2 - i] = xnext - i * interval;


      return result;
    }

    /// <summary>
    /// Integrates func*Exp(-func) from x0 to x1. It relies on the fact that func is monotonical increasing from x0 to x1.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    protected static double IntegrateFuncExpMFuncInc(ScalarFunctionDD func, ScalarFunctionDD funcExpMFunc, double x0, double x1, ref object tempStorage, double precision)
    {
      double y0 = func(x0);
      if (y0 >= 1)
      {
        return IntegrateFuncExpMFuncIncAdvanced(func, funcExpMFunc, x0, x0, x1, ref tempStorage, precision);
      }

      double xm = FindIncreasingYEqualToOne(func, x0, x1);
      double[] intgrenzen = new double[100];
      int nIntPts = GetIntegrationPointsXmNearX0(x0, xm, x1, intgrenzen);
      double result = 0, abserr = 0;
      try
      {
        GSL_ERROR error = Calc.Integration.QagpIntegration.Integration(
        funcExpMFunc, intgrenzen, nIntPts, 0, precision, 200, out result, out abserr, ref tempStorage);

        if (null != error)
        {
          result = IntegrateFuncExpMFuncIncAdvanced(func, funcExpMFunc, x0, xm, x1, ref tempStorage, precision);
        }

        return result;
      }
      catch (Exception ex)
      {
        return result;
      }
    }

    protected static double IntegrateFuncExpMFuncIncAdvanced(ScalarFunctionDD func, ScalarFunctionDD funcExpMFunc, double x0, double xm, double x1, ref object tempStorage, double precision)
    {
      const double ySearchLeftSide = 1E-3;
      const double yToleranceLeftSide = 1E-4;
      const double yOffsetRightSide = 7;
      const double yToleranceRightSide = 0.1;

      const double MinScale = 10;
      const double MaxScale = 80;
      const int MaxDivisions = 30;
      double[] intgrenzen = new double[2 * MaxDivisions + 4];

      double resultLeft = 0;
      double resultRight = 0;
      double abserrLeft, abserrRight;

      // first lets integrate the left side
      if (xm > x0)
      {
        int n;
        double yl;
        double xl = FindIncreasingYEqualTo(func, x0, xm, ySearchLeftSide, yToleranceLeftSide, out yl);
        double xdiffl = xl - x0;
        double xdiffr = xm - xl;
        if (xdiffr < xdiffl) // then we logarithmically space beginning from xm to the left border x0
        {
          n = PartFromTheRight(x0, xm, xdiffr, intgrenzen, 0);
        }
        else
        {
          n = PartFromTheLeft(x0, xm, xdiffl, intgrenzen, 0);
        }
        GSL_ERROR error = Calc.Integration.QagpIntegration.Integration(
         funcExpMFunc, intgrenzen, n, 0, precision, 100, out resultLeft, out abserrLeft, ref tempStorage);

        if (null != error)
        {
          resultLeft = double.NaN;
        }
      }


      // now lets integrate the right
      if (xm < x1)
      {
        /*
        int n;
        double ySearchRightSide = yOffsetRightSide + func(xm);
        double yr;
        double xr = FindIncreasingYEqualTo(func, xm, x1, ySearchRightSide, yToleranceRightSide, out yr);
        double xdiffl = xr - xm;
        double xdiffr = x1 - xr;
        if (xdiffr < xdiffl) // then we logarithmically space beginning from xm to the left border x0
        {
          n = PartFromTheRight(xm, x1, xdiffr, intgrenzen,0);
        }
        else
        {
          n = PartFromTheLeft(xm, x1, xdiffl, intgrenzen,0);
        }
        */

        int n = PartUnknownRightSideInc(func, xm, x1, intgrenzen, 0);
        GSL_ERROR error = Calc.Integration.QagpIntegration.Integration(
          funcExpMFunc, intgrenzen, n, 0, precision, 200, out resultRight, out abserrRight, ref tempStorage);

        if (null != error)
        {
          resultRight = double.NaN;
        }
      }

      return resultLeft + resultRight;
    }

    /// <summary>
    /// Returns the number of logarithmically spaced subdivisions to span a certain range.
    /// This mean that if span==smallestDivision, the function will return 1.
    /// If span = smallestdivision + smallestdivision*span + smallestdivision*span^2, the function will return 3.
    /// </summary>
    /// <param name="span">The range to space logarithmically.</param>
    /// <param name="smallestDivision">The smallest division.</param>
    /// <param name="scale">Value &gt; 1 which is the ratio of the value of the next subdivision to the current subdivision.</param>
    /// <returns></returns>
    static double GetNumberOfLogarithmicDivisions(double span, double smallestDivision, double scale)
    {
      double ges = span / smallestDivision;
      return Math.Log(ges * (scale - 1) + scale) / Math.Log(scale) - 1;
    }

    static double GetScaleOfLogarithmicSubdivision(double span, double smallestDivision, double n)
    {
      double scale = double.MaxValue;
      double OneByN = 1.0 / n;
      double ges = span / smallestDivision;
      if (ges + 1 < double.MaxValue)
      {
        // first guess
        scale = Math.Pow(ges + 1 - ges / scale, OneByN);
        scale = Math.Pow(ges + 1 - ges / scale, OneByN);
        scale = Math.Pow(ges + 1 - ges / scale, OneByN);
      }
      else
      {
        double ln_ges = Math.Log(span) - Math.Log(smallestDivision);
        scale = Math.Exp((ln_ges + Math.Log(1 - 1 / scale)) * OneByN);
        scale = Math.Exp((ln_ges + Math.Log(1 - 1 / scale)) * OneByN);
        scale = Math.Exp((ln_ges + Math.Log(1 - 1 / scale)) * OneByN);
      }
      return scale;
    }



    protected static int PartFromTheRight(double x0, double x1, double xdiffr, double[] intgrenzen, int start)
    {
      const double MinScale = 4;
      const double MaxScale = 100;
      const int MaxDivisions = 30;

      if (xdiffr <= 0)
      {
        xdiffr = (MaxScale - 1) * (x1 - x0) / (-1 + RMath.Pow(MaxScale, MaxDivisions + 2));
      }

      double xdiffl = (x1 - x0) - xdiffr;
      double n = MaxDivisions;
      double scale = GetScaleOfLogarithmicSubdivision(x1 - x0, xdiffr, n);
      if (scale < MinScale)
      {
        scale = MinScale;
        n = Math.Ceiling(GetNumberOfLogarithmicDivisions(x1 - x0, xdiffr, scale));
      }
      else if (scale > MaxScale)
      {
        scale = MaxScale;
        n = Math.Ceiling(GetNumberOfLogarithmicDivisions(x1 - x0, xdiffr, scale));
      }
      else
      {
      }

      int nn = (int)Math.Min(MaxDivisions, Math.Max(1, n));
      intgrenzen[start] = x0;
      intgrenzen[start + nn + 1] = x1;
      double interval = xdiffr;
      for (int i = 0; i < nn; i++)
      {
        double xp = x1-interval;
        intgrenzen[start + nn - i] = x1 - interval;
        interval *= scale;
      }
      return nn + 2;
    }


    protected static int PartFromTheLeft(double x0, double x1, double xdiffl, double[] intgrenzen, int start)
    {
      const double MinScale = 4;
      const double MaxScale = 100;
      const int MaxDivisions = 30;


      if (xdiffl <= 0)
      {
        // we calculate the subintervals for a given scale of 100
        xdiffl = (MaxScale - 1) * (x1 - x0) / (-1 + RMath.Pow(MaxScale, MaxDivisions + 2));
      }

      double xdiffr = (x1 - x0) - xdiffl;
      double n = MaxDivisions;
      double scale = GetScaleOfLogarithmicSubdivision(x1 - x0, xdiffl, n);
      if (scale < MinScale)
      {
        scale = MinScale;
        n = Math.Ceiling(GetNumberOfLogarithmicDivisions(x1 - x0, xdiffl, scale));
      }
      else if (scale > MaxScale)
      {
        scale = MaxScale;
        n = Math.Ceiling(GetNumberOfLogarithmicDivisions(x1 - x0, xdiffl, scale));
      }
      else
      {
      }

      int nn = (int)Math.Min(MaxDivisions, Math.Max(1, n));

      try
      {
        intgrenzen[start] = x0;
        intgrenzen[start + nn + 1] = x1;
        double interval = xdiffl;
        for (int i = 0; i < nn; i++)
        {
          intgrenzen[start + i + 1] = x0 + interval;
          interval *= scale;
        }
        return nn + 2;
      }
      catch (Exception ex)
      {
      }
      return 0;
    }

   

    protected static int PartUnknownRightSideInc(ScalarFunctionDD func, double x0, double x1, double[] intgrenzen, int start)
    {
      const double diffOneDecade = 7;
      int count;
      double y1;


      x1 = FindIncreasingYEqualTo(func, x0, x1, MinusLogTiny + 1, 1, out y1);
      double y0 = func(x0);

      // When the difference of y values results in a difference able to handle by the algorithm, then return immediately
      if ((y0 >= MinusLogTiny) || ((y1 - y0) < diffOneDecade))
      {
        intgrenzen[start] = x0;
        intgrenzen[start + 1] = x1;
        count = 2;
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

        dx = x0==0 ? 16*double.Epsilon : x0*16*DoubleConstants.DBL_EPSILON;
        y00 = func(x0 + dx);
        s0 = (y00 - y0) / dx;

        dx = x1 * 16 * DoubleConstants.DBL_EPSILON;
        y11 = func(x1 - dx);
        s1 = (y1 - y11) / dx; // derivative at point y0

        // now compare the different slopes 
        // note that all slope values should be zero or greater than zero, since the function is increasing

        if (s0 > s01 && s1 < s01)
        {
          // increasing fast at x0 and slow at x1, so part from the left
          double increment = Math.Min(x0>0 ? x0 : double.MaxValue, diffOneDecade / s0);
          count = PartFromTheLeft(x0, x1, increment, intgrenzen, start);
        }
        else if (s0 < s01 && s1 > s01)
        {
          // increasing slow at x0 and fast at x1, so part from the right
          count = PartFromTheRight(x0, x1, diffOneDecade / s1, intgrenzen, start);
        }
        else if (s0 < s01 && s1 < s01)
        {
          // in this case, there is the fast transition somewhere inbetween the interval, so we have to search for it
          double ym;
          double xm = FindIncreasingYEqualTo(func, x0, x1, 0.5 * (y0 + y1), 0.1, out ym);
          double xdist = Math.Min(xm - x0, x1 - xm);
          xdist = Math.Min(xdist, 1 / s01);
          double sm = (func(xm + 0.5 * xdist) - func(xm - 0.5 * xdist)) / xdist;

          double xinterval = sm > 0 ? diffOneDecade / sm : xm * DoubleConstants.DBL_EPSILON;
          count = PartFromTheRight(x0, xm, xinterval, intgrenzen, start);
          count--; // we decrease count because we don't want to have the middle point twice
          count += PartFromTheLeft(xm, x1, xinterval, intgrenzen, start + count);
        }
        else if (s0 > s01 && s1 > s01)
        {
          // then we have fast increases both on x0 and x1, so we must have a plateau inbetween
          double xm = 0.5 * (x0 + x1);
          double increment = Math.Min(x0 > 0 ? x0 : double.MaxValue, diffOneDecade / s0);
          count = PartFromTheLeft(x0, xm, increment, intgrenzen, start);
          count--; // we decrease count because we don't want to have the middle point twice
          count += PartFromTheRight(xm, x1, diffOneDecade / s1, intgrenzen, start + count);
        }
        else
        {
          // part linearly spaced between x0 and x1
          double xinc = diffOneDecade / s01;
          double xs = x0;
          for (count = 0; (start + count) < intgrenzen.Length; count++)
          {
            if (xs >= x1)
            {
              intgrenzen[start + count] = x1;
              count++;
              break;
            }
            intgrenzen[start + count] = xs;
            xs += xinc;
          }
        }
      }
      return count;
    }

    /// <summary>
    /// Integrates func*Exp(-func) from x0 to x1. It relies on the fact that func is monotonical decreasing from x0 to x1.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    protected static double IntegrateFuncExpMFuncDec(ScalarFunctionDD func, ScalarFunctionDD funcExpMFunc, double x0, double x1, ref object tempStorage, double precision)
    {
      const int MaxDivisions = 30;
      double xm = FindDecreasingYEqualToOne(func, x0, x1);

      double result = 0, abserr = 0;
      double[] intgrenzen = new double[100];
      int nIntPts = GetIntegrationPointsXmNearX0(x0, xm, x1, intgrenzen);

      try
      {
        GSL_ERROR error = Calc.Integration.QagpIntegration.Integration(
         funcExpMFunc, intgrenzen, nIntPts, 0, precision, 100, out result, out abserr, ref tempStorage);

        if (null != error)
          result = double.NaN;

        return result;
      }
      catch (Exception ex)
      {
        return result;
      }
    }






    /// <summary>
    /// Integrates Exp(-func) from x0 to x1. It relies on the fact that func is monotonical increasing from x0 to x1, so that the maximum 
    /// of Exp(-func) is at x0.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    protected static double IntegrateExpMFunc(ScalarFunctionDD func, double x0, double x1, bool isDecreasing, ref object tempStorage)
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

    /// <summary>
    /// Finds the x where func(x)==1+-1E-5 between x &lt; x0 &lt; x1 for a monoton increasing function func.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    protected static double FindIncreasingYEqualToOne(ScalarFunctionDD func, double x0, double x1)
    {
      const double ConsideredAsZero = 2 * double.Epsilon;
      double low = x0;
      double high = x1;
      double xm = 0;
      double xmprev = 0;
      double y;
      for (; ; )
      {
        xm = 0.5 * (low + high);
        if (xm == xmprev)
          break;
        xmprev = xm;

        y = func(xm);
        if (Math.Abs(y - 1) < 1E-5)
          break;
        else if (y < 1)
          low = xm;
        else
          high = xm;
      }

      return (x0 == 0 && xm <= ConsideredAsZero) ? 0 : xm;
    }

    /// <summary>
    /// Finds the x where func(x)==1+-1E-5 between x<x0<x1 for a monoton increasing function func.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    protected static double FindIncreasingYEqualTo(ScalarFunctionDD func, double x0, double x1, double ysearch, double tol, out double y)
    {
      const double ConsideredAsZero = 2 * double.Epsilon;

      double low = x0;
      double high = x1;
      double xm = 0;
      double xmprev = 0;
      y = double.NaN;

      for (; ; )
      {
        xm = 0.5 * (low + high);
        if (xm == xmprev)
          break;
        xmprev = xm;

        y = func(xm);
        if (Math.Abs(y - ysearch) < tol)
          break;
        else if (y < ysearch)
          low = xm;
        else
          high = xm;
      }
      return (x0 == 0 && xm <= ConsideredAsZero) ? 0 : xm;
    }

    /// <summary>
    /// Finds the x where func(x)==1+-1E-5 between x<x0<x1 for a monoton increasing function func.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    protected static double FindIncreasingYEqualToTen(ScalarFunctionDD func, double x0, double x1)
    {
      double low = x0;
      double high = x1;
      double xm;
      for (; ; )
      {
        xm = 0.5 * (low + high);
        double y = func(xm);
        if (Math.Abs(y - 10) < 1E-1)
          break;
        else if (y < 10)
          low = xm;
        else
          high = xm;

        if ((high - low) < 1E-15 * Math.Max(Math.Abs(high), Math.Abs(low)))
          break;
      }
      return xm;
    }

    /// <summary>
    /// Finds the x where func(x)==1+-1E-5 between x<x0<x1 for a monoton decreasing function func.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    protected static double FindDecreasingYEqualToOne(ScalarFunctionDD func, double x0, double x1)
    {
      double low = x0;
      double high = x1;
      double xmprev = 0;
      double xm = 0;
      double y;
      for (; ; )
      {
        xm = 0.5 * (low + high);
        if (xm == xmprev)
          break;
        xmprev = xm;

        y = func(xm);
        if (Math.Abs(y - 1) < 1E-5)
          break;
        else if (y < 1)
          high = xm;
        else
          low = xm;
      }
      return xm;
    }


    /// <summary>
    /// Finds the x where func(x)==1+-1E-5 between x<x0<x1 for a monoton decreasing function func.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    protected static double FindDecreasingYEqualTo(ScalarFunctionDD func, double x0, double x1, double ysearch)
    {
      double low = x0;
      double high = x1;
      double xmprev = 0;
      double xm = 0;
      double y;
      for (; ; )
      {
        xm = 0.5 * (low + high);
        if (xm == xmprev)
          break;
        xmprev = xm;

        y = func(xm);
        if (Math.Abs(y - ysearch) < 1E-5)
          break;
        else if (y < ysearch)
          high = xm;
        else
          low = xm;
      }
      return xm;
    }

    protected static bool IsXNearlyEqualToZeta(double x, double zeta)
    {
      if (x == zeta)
        return true;
      if (x == 0)
        return Math.Abs(zeta) < DoubleConstants.DBL_EPSILON;
      if (zeta == 0)
        return Math.Abs(x) < DoubleConstants.DBL_EPSILON;
      else
        return Math.Abs(x - zeta) < Math.Max(Math.Abs(x), Math.Abs(zeta)) * 1000 * DoubleConstants.DBL_EPSILON;
    }


    protected static double PDFMethodAlphaOne(double x, double beta, ref object tempStorage, double precision)
    {
      double factor = Math.Exp(-0.5 * Math.PI * x / beta) * 2 / Math.PI;
      double integrand = IntegrateFuncExpMFunc(delegate(double theta) { return PDFCoreAlphaOne(factor, beta, theta); }, -0.5 * Math.PI, 0.5 * Math.PI, beta < 0, ref tempStorage, precision);
      double pre = 1 / (2 * Math.Abs(beta));
      return pre * integrand;
    }
    protected static double PDFCoreAlphaOne(double factor, double beta, double theta)
    {
      double r1 = (0.5 * Math.PI + beta * theta) / Math.Cos(theta);
      double r2 = Math.Exp((0.5 * Math.PI + beta * theta) * Math.Tan(theta) / beta);
      return factor * r1 * r2;
    }

    #endregion

    #region Parameter conversion between different parametrizations

    private static double GammaFromAlphaBetaTanPiA2(double alpha, double beta, double tan_pi_alpha_2)
    {
      if (Math.Abs(beta) == 1) // Avoid roundoff errors when Abs(beta)==1
        return beta == 1 ? Math.IEEERemainder(-alpha, 2) : Math.IEEERemainder(alpha, 2);
      else
        return 2 / Math.PI * Math.Atan(-beta * tan_pi_alpha_2);
    }
    private static double TanPiAlphaBy2(double alpha)
    {
      if (Math.Floor(alpha) == alpha)
      {
        double rem = Math.IEEERemainder(alpha, 2);
        if (rem == 0)
          return 0;
        else if (rem == 1)
          return double.PositiveInfinity;
        else if (rem == -1)
          return double.NegativeInfinity;
        else
          return double.NaN;
      }
      else
      {
        return Math.Tan(0.5 * Math.PI * alpha);
      }
    }

    public static void ParameterConversionS0ToFeller(double alpha, double beta, double sigma0, double mu0, out double gamma, out double sigmaf, out double muf)
    {
      if (alpha != 1)
      {
        double tan_pi_alpha_2 = TanPiAlphaBy2(alpha);
        gamma = GammaFromAlphaBetaTanPiA2(alpha, beta, tan_pi_alpha_2);
        sigmaf = sigma0 * Math.Pow(1 + RMath.Pow2(beta * tan_pi_alpha_2), 0.5 / alpha);
        muf = mu0 - sigma0 * beta * tan_pi_alpha_2;
      }
      else
      {
        if (beta == 0)
        {
          gamma = 0;
          sigmaf = sigma0;
          muf = mu0;
        }
        else
        {
          throw new ArgumentException("Alpha is 1 and beta!=0, thus the conversion is undefined");
        }
      }
    }

    public static void ParameterConversionS1ToFeller(double alpha, double beta, double sigma1, double mu1, out double gamma, out double sigmaf, out double muf)
    {
      if (alpha != 1)
      {
        double tan_pi_alpha_2 = TanPiAlphaBy2(alpha);
        gamma = GammaFromAlphaBetaTanPiA2(alpha, beta, tan_pi_alpha_2);
        sigmaf = sigma1 * Math.Pow(1 + RMath.Pow2(beta * tan_pi_alpha_2), 0.5 / alpha);
        muf = mu1;
      }
      else
      {
        if (beta == 0)
        {
          gamma = 0;
          sigmaf = sigma1;
          muf = mu1;
        }
        else
        {
          throw new ArgumentException("Alpha is 1 and beta!=0, thus the conversion is undefined");
        }
      }
    }

    public static void ParameterConversionFellerToS0(double alpha, double gamma, double sigmaf, double muf, out double beta, out double sigma0, out double mu0)
    {
      if (alpha != 1 && alpha != 2)
      {
        double tan_pi_alpha_2 = TanPiAlphaBy2(alpha);
        beta = -Math.Tan(0.5 * Math.PI * gamma) / tan_pi_alpha_2;
        sigma0 = sigmaf * Math.Pow(1 + RMath.Pow2(beta * tan_pi_alpha_2), -0.5 / alpha);
        mu0 = muf + sigma0 * beta * tan_pi_alpha_2;
      }
      else
      {
        if (gamma == 0)
        {
          beta = 0;
          sigma0 = sigmaf;
          mu0 = muf;
        }
        else
        {
          throw new ArgumentException("Alpha is 1 or 2 and gamma!=0, thus the conversion is undefined");
        }
      }
    }

    public static void ParameterConversionFellerToS1(double alpha, double gamma, double sigmaf, double muf, out double beta, out double sigma1, out double mu1)
    {
      if (alpha != 1 && alpha != 2)
      {
        double tan_pi_alpha_2 = TanPiAlphaBy2(alpha);
        beta = -Math.Tan(0.5 * Math.PI * gamma) / tan_pi_alpha_2;
        sigma1 = sigmaf * Math.Pow(1 + RMath.Pow2(beta * tan_pi_alpha_2), -0.5 / alpha);
        mu1 = muf;
      }
      else
      {
        if (gamma == 0)
        {
          beta = 0;
          sigma1 = sigmaf;
          mu1 = muf;
        }
        else
        {
          throw new ArgumentException("Alpha is 1 or 2 and gamma!=0, thus the conversion is undefined");
        }
      }
    }

    public static void ParameterConversionS0ToS1(double alpha, double beta, double sigma0, double mu0, out double mu1)
    {
      if (alpha != 1)
      {
        mu1 = mu0 - sigma0 * beta * TanPiAlphaBy2(alpha);
      }
      else
      {
        mu1 = mu0 - sigma0 * beta * 2 * Math.Log(sigma0) / Math.PI;
      }
    }

    public static void ParameterConversionS1ToS0(double alpha, double beta, double sigma1, double mu1, out double mu0)
    {
      if (alpha != 1)
      {
        mu0 = mu1 + sigma1 * beta * TanPiAlphaBy2(alpha);
      }
      else
      {
        mu0 = mu1 + sigma1 * beta * 2 * Math.Log(sigma1) / Math.PI;
      }
    }


    #endregion

    #region Generation

    protected double GenerateSymmetricCase(double alpha)
    {

      double u, v, t, s;

      u = Math.PI * (_contDist.NextDouble() - 0.5);

      if (alpha == 1)               /* cauchy case */
      {
        t = Math.Tan(u);
        return t;
      }

      do
      {
        v = _expDist.NextDouble();
      }
      while (v == 0);

      if (alpha == 2)             /* gaussian case */
      {
        t = 2 * Math.Sin(u) * Math.Sqrt(v);
        return t;
      }

      /* general case */

      t = Math.Sin(alpha * u) / Math.Pow(Math.Cos(u), 1 / alpha);
      s = Math.Pow(Math.Cos((1 - alpha) * u) / v, (1 - alpha) / alpha);

      return t * s;
    }

    /// <summary>
    /// Generates random variates in S1 Parametrization
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    protected double GenerateAsymmetricCaseS1(double alpha, double beta, double c)
    {
      double V, W, X;
      const double M_PI = Math.PI;
      const double M_PI_2 = Math.PI / 2;


      V = M_PI * (_contDist.NextDouble() - 0.5);

      do
      {
        W = _expDist.NextDouble();
      }
      while (W == 0);

      if (alpha == 1)
      {
        X = ((M_PI_2 + beta * V) * Math.Tan(V) -
             beta * Math.Log(M_PI_2 * W * Math.Cos(V) / (M_PI_2 + beta * V))) / M_PI_2;
        // org return c * (X + beta * Math.Log(c) / M_PI_2);
        return c * X;
      }
      else
      {
        double t = beta * Math.Tan(M_PI_2 * alpha);
        double B = Math.Atan(t) / alpha;
        double S = Math.Pow(1 + t * t, 1 / (2 * alpha));

        X = S * Math.Sin(alpha * (V + B)) / Math.Pow(Math.Cos(V), 1 / alpha)
          * Math.Pow(Math.Cos(V - alpha * (V + B)) / W, (1 - alpha) / alpha);
        return c * X;
      }
    }
    #endregion
  }
}
