#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Calculates a natural cubic spline curve which smoothes a given set
  /// of data points, using statistical considerations to determine the amount
  /// of smoothing required as described in reference 2.
  /// </summary>
  /// <remarks>
  ///  If the error variance
  /// is known, it should be supplied to the routine in 'var'. The degree of
  /// smoothing is then determined by minimizing an unbiased estimate of the
  /// true mean square error.  On the other hand, if the error variance is
  /// not known, 'var' should be set to -1.0. The routine then determines the
  /// degree of smoothing by minimizing the generalized cross validation.
  /// This is asymptotically the same as minimizing the true mean square error
  /// (see reference 1).  In this case, an estimate of the error variance is
  /// returned in 'var' which may be compared with any a priori approximate
  /// estimates. In either case, an estimate of the true mean square error
  /// is returned in 'wk[4]'.  This estimate, however, depends on the error
  /// variance estimate, and should only be accepted if the error variance
  /// estimate is reckoned to be correct.
  /// Bayesian estimates of the standard error of each smoothed data value are
  /// returned in the array 'se' (if a non null vector is given for the
  /// paramenter 'se' - use (double*)0 if you don't want estimates).
  /// These also depend on the error variance estimate and should only
  /// be accepted if the error variance estimate is reckoned to be correct.
  /// See reference 4.
  /// The number of arithmetic operations and the amount of storage required by
  /// the routine are both proportional to 'n', so that very large data sets may
  /// be analysed. The data points do not have to be equally spaced or uniformly
  /// weighted. The residual and the spline coefficients are calculated in the
  /// manner described in reference 3, while the trace and various statistics,
  /// including the generalized cross validation, are calculated in the manner
  /// described in reference 2.
  ///
  /// When 'var' is known, any value of 'n' greater than 2 is acceptable. It is
  /// advisable, however, for 'n' to be greater than about 20 when 'var'
  /// is unknown. If the degree of smoothing done by this function when 'var' is
  /// unknown is not satisfactory, the user should try specifying the degree
  /// of smoothing by setting 'var' to a reasonable value.
  /// <code>
  /// Notes:
  ///
  /// Algorithm 642, "cubgcv", collected algorithms from ACM.
  /// Algorithm appeared in ACM-Trans. Math. Software, Vol.12, No. 2,
  /// Jun., 1986, p. 150.
  ///
  /// Originally written by M.F.Hutchinson, CSIRO Division of Mathematics
  /// and Statistics, P.O. Box 1965, Canberra, Act 2601, Australia.
  /// Latest revision 15 august 1985.
  ///
  /// FORTRAN source code translated to C by jheinen and IngoHeimbach (see <see href="https://github.com/sciapp/gr/blob/master/lib/gr/spline.c"/>)
  ///
  /// C source code transfered to C# by Dirk Lellinger.
  ///
  /// References:
  ///
  /// 1.  Craven, Peter and Wahba, Grace, "Smoothing noisy data with spline
  ///     functions", Numer. Math. 31, 377-403 (1979).
  /// 2.  Hutchinson, M.F. and de Hoog, F.R., "Smoothing noisy data with spline
  ///     functions", Numer. Math. 47, 99-106 (1985).
  /// 3.  Reinsch, C.H., "Smoothing by spline functions", Numer. Math. 10,
  ///     177-183 (1967).
  /// 4.  Wahba, Grace, "Bayesian 'confidence intervals' for the cross-validated
  ///     smoothing spline", J.R.Statist. Soc. B 45, 133-150 (1983).
  ///
  /// ----------------------------------------------------------------------------
  /// </code>
  /// </remarks>
  public abstract class SmoothingCubicSplineBase : CurveBase, IInterpolationFunction
  {
    /// <summary>
    /// If true, the array given as arguments are checked.
    /// </summary>
    public bool CheckArguments { get; set; } = true;

    /// <summary>
    /// If true, points with x values that are very similar are combined into one point, which helds the average value of x and y
    /// </summary>
    public bool CombineNeighbouringPoints { get; set; }

    /// <summary>
    /// If true, standard error estimates are calculated and provided in <see cref="ErrorEstimate"/>.
    /// </summary>
    public bool CalculateStandardErrorEstimates { get; set; } = true;



    protected double _variance;
    private bool _interpolationSuccessfullyExecuted;
    private bool _standardErrorEstimatesCalculated;

    // Vector wrappers. Attention: these wrappers may have less length than the underlying arrays!
    protected IROVector<double>? _xVec;
    protected IROVector<double>? _fVec;
    protected IROVector<double>? _c0Vec;
    protected IROVector<double>? _c1Vec;
    protected IROVector<double>? _c2Vec;
    protected IROVector<double>? _c3Vec;
    protected IROVector<double>? _seVec;


    protected double[]? _x; // Abscissa values
    protected double[]? _f; // Ordinate values
    protected double[]? _df; // Stores the known deviations of the ordinate values _f
    protected double[]? _y0; // the calculated spline ordinate values at x[i], i.e. the spline coefficients of order 0
    protected double[]? _se; // the estimated standard error of the points (only valid if doing cross-validation!)
    protected double[][]? _c; // the coefficients of order 1 to 3

    // Work-arrays
    protected double[][]? _wkr;
    protected double[][]? _wkt;
    protected double[]? _wku;
    protected double[]? _wkv;


    public SmoothingCubicSplineBase()
    {
      _variance = -1.0; // unknown variance
    }

    /// <summary>
    /// Evaluates either a cross validated cubic spline (if <see cref="ErrorVariance"/> is negative,
    /// or a smoothing cubic spline (if <see cref="ErrorVariance"/> is greater than or equal to zero).
    /// </summary>
    /// <param name="x">The abscissae values.</param>
    /// <param name="y">The ordinate values.</param>
    public override void Interpolate(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
      Interpolate(x, y, _variance, null);
    }


    /// <summary>
    /// Evaluates either a cross validated cubic spline (<paramref name="variance"/> set to a negative value),
    /// or a smoothing cubic spline (<paramref name="variance"/> set to a non-negative value).
    /// </summary>
    /// <param name="x">The abscissae values.</param>
    /// <param name="y">The ordinate values.</param>
    /// <param name="variance">
    /// If set to a negative value, a cross validated cubic spline is evaluated.
    /// If set to a positive value, the value designates the variance (=square of the standard deviation) of the ordinate values.
    /// </param>
    public void Interpolate(IReadOnlyList<double> x, IReadOnlyList<double> y, double variance)
    {
      Interpolate(x, y, variance, null);
    }

    /// <summary>
    /// Evaluates either a cross validated cubic spline (<paramref name="variance"/> set to a negative value),
    /// or a smoothing cubic spline (<paramref name="variance"/> set to a non-negative value).
    /// </summary>
    /// <param name="x">The abscissae values.</param>
    /// <param name="y">The ordinate values.</param>
    /// <param name="variance">
    /// If set to a negative value, a cross validated cubic spline is evaluated (in this case the parameter <paramref name="dy"/> must be null).
    /// If set to a non-negative value, and parameter <paramref name="dy"/> is null, the value designates the variance (=square of the standard deviation) of the ordinate values.
    /// If set to a non-negative value, and parameter <paramref name="dy"/> is not null, the value designates a scaling factor for the values in <paramref name="dy"/>.
    /// </param>
    /// <param name="dy">
    /// Relative standard deviation of the error associated with the data point i.
    /// Each element must be positive. The values are scaled so that their mean square value is 1, and unscaled again on normal exit.
    /// The mean square value of the elements is returned in <see cref="MeanSquareOfInputVariance"/> on normal exit.
    /// If the absolute standard deviations are known, these should be provided here and the error
    /// variance parameter <paramref name="variance"/> should then be set to 1.
    /// If the relative standard deviations are unknown, set each element to 1, or set this parameter to null.</param>
    public void Interpolate(IReadOnlyList<double> x, IReadOnlyList<double> y, double variance, IReadOnlyList<double>? dy)
    {
      // check input parameters
      if (x is null)
        throw new ArgumentNullException(nameof(x));
      if (y is null)
        throw new ArgumentNullException(nameof(y));
      if (x.Count == 0)
        throw new ArgumentException($"Vector {nameof(x)} is empty");
      if (y.Count == 0)
        throw new ArgumentException($"Vector {nameof(y)} is empty");
      if (!MatchingIndexRange(x, y))
        throw new ArgumentException($"Length mismatch between {nameof(y)} and {nameof(x)}");
      if (dy is not null && !MatchingIndexRange(x, dy))
        throw new ArgumentException($"Length mismatch between {nameof(dy)} and {nameof(x)}");
      if (dy is not null && !(variance > 0))
        throw new ArgumentException($"The parameter {nameof(variance)} must be greater than 0 if the array {nameof(dy)} is provided!");


      if (CheckArguments)
      {
        if (CombineNeighbouringPoints)
          ThrowIfIsNotMonotonicallyIncreasing(x, nameof(x));
        else
          ThrowIfIsNotStrictlyMonotonicallyIncreasing(x, nameof(x));

        ThrowIfContainsNaNOrInfiniteValues(y, nameof(y));
        if (dy is not null)
        {
          ThrowIfContainsNegativeOrNaNOrInfiniteValues(dy, nameof(dy));
        }
      }

      _interpolationSuccessfullyExecuted = false;
      _standardErrorEstimatesCalculated = false;


      // here we must use a copy of the original vectors
      var n = x.Count;

      // In order to save allocations for repeated interpolations with
      // different number of points, we reallocate the arrays only if
      // the required number of points is larger than currently.
      SmartReallocate(n);

#nullable disable

      if (CombineNeighbouringPoints)
      {
        double meanXDiff = 0;
        for (int i = 1; i < n; ++i)
        {
          meanXDiff += Math.Abs(x[i] - x[i - 1]);
        }
        meanXDiff /= (n - 1); // calculate the average of the differences between x[i] and x[i-1]
        double threshold = 1E-7 * meanXDiff;

        int prevI = 0;
        int nextI;
        int j = 0;
        do
        {
          // advance nextI until there is enough distance between the x values
          for (nextI = prevI + 1; (nextI < n) && (x[nextI] - x[prevI]) < threshold; ++nextI) ;


          if (nextI == prevI + 1) // copy the values if no average is neccessary
          {
            _x[j] = x[prevI];
            _f[j] = y[prevI];
            ++j;
          }
          else // or combine the values by averaging
          {
            // average over prevI .. k-1
            double sumX = 0, sumY = 0;
            for (int k = prevI; k < nextI; ++k)
            {
              sumX += x[k];
              sumY += y[k];
            }
            _x[j] = sumX / (nextI - prevI);
            _f[j] = sumY / (nextI - prevI);
            ++j;
          }
          prevI = nextI;
        } while (nextI < n);

        n = j; // assing our new n (number of points
        SmartReallocate(n);
      }
      else // Do not combine neighbouring points - thus simply copy the x and f values
      {
        // Copy x into _x and y into f (!)
        for (int i = 0; i < n; ++i)
        {
          _x[i] = x[i];
          _f[i] = y[i];
        }
      }



      // link original data vectors into base class
      base.x = _xVec;
      base.y = _fVec;

      if (n == 1)
      {
        // set derivatives for a single point
        _y0[0] = y[0];
        _c[0][0] = 0;
        _c[1][0] = 0;
        _c[2][0] = 0;
        _interpolationSuccessfullyExecuted = true;
      }
      else if (n == 2)
      {
        // set derivatives for a line
        _y0[0] = y[0];
        _y0[1] = y[1];
        _c[0][0] = _c[0][1] = (y[1] - y[0]) / (x[1] - x[0]); // first derivative
        _c[1][0] = _c[1][1] = 0; // 2nd derivative
        _c[2][0] = _c[2][1] = 0; // 3rd derivative
        _interpolationSuccessfullyExecuted = true;
      }
      else
      {
        // number of points is >= 3

        _variance = variance;

        if (dy is not null) // if deviations of the points are known, copy them
        {
          for (int i = 0; i < n; ++i)
            _df[i] = dy[i];
        }
        else  // if deviations of the points are not known, then set them to 1
        {
          for (int i = 0; i < n; ++i)
            _df[i] = 1;
        }


        InterpolationKernel(_x, _f, _df, n, _y0, _c, n - 1, _variance, CalculateStandardErrorEstimates ? 1 : 0, _se, _wkr, _wkt, _wku, _wkv, out int error);

        switch (error)
        {
          case 0:
            _interpolationSuccessfullyExecuted = true;
            _standardErrorEstimatesCalculated = CalculateStandardErrorEstimates;
            break;

          case 129:
            throw new InvalidProgramException("IC is less than N-1");
          case 130:
            throw new InvalidProgramException("N is less than 3");
          case 131:
            throw new ArgumentException("Abscissa (x-values) are not ordered! Please order them before calling " + nameof(Interpolate));
          case 132:
            throw new ArgumentException("Of the provided ordinate (y) variance values at least one value is negative");
          case 133:
            throw new InvalidProgramException("The job value is neither 1 nor 2");
        }
      }
    }

    protected abstract void InterpolationKernel(
      double[] x,
      double[] f,
      double[] df,
      int n,
      double[] y,
      double[][] C,
      int ic,
      double var,
      int job,
      double[] se,
      double[][] WK0,
      double[][] WK1,
      double[] WK2,
      double[] WK3,
      out int ier);


    private void SmartReallocate(int n)
    {
      if (n > (_x?.Length ?? 0)) // Create bigger arrays if n exceeds allocated array length
      {
        _x = new double[n];
        _f = new double[n];
        _df = new double[n];
        _y0 = new double[n];
        _se = new double[n];
        _c = new double[][] { new double[n - 1], new double[n - 1], new double[n - 1] };
        _wkr = new double[][] { new double[n + 2], new double[n + 2], new double[n + 2] };
        _wkt = new double[][] { new double[n + 2], new double[n + 2] };
        _wku = new double[n + 2];
        _wkv = new double[n + 2];
      }

      if (n != (_xVec?.Count ?? 0)) // Create new wrappers if current n is different from previous n
      {
        _xVec = VectorMath.ToROVector(_x, n);
        _fVec = VectorMath.ToROVector(_f, n);
        _c0Vec = VectorMath.ToROVector(_y0, n);
        _c1Vec = VectorMath.ToROVector(_c[0], n - 1);
        _c2Vec = VectorMath.ToROVector(_c[1], n - 1);
        _c3Vec = VectorMath.ToROVector(_c[2], n - 1);
        _seVec = VectorMath.ToROVector(_se, n);
      }
    }

#nullable enable

    #region Fit results

    protected Exception NotExecutedException()
    {
      return new InvalidOperationException("Interpolation was not yet successfully executed.");
    }

    /// <summary>
    /// Returns the spline coefficient of order 0. This are the splined y values at the positions given by x.
    /// </summary>
    public IROVector<double> Coefficient0
    {
      get
      {
        if (_interpolationSuccessfullyExecuted && !(_c0Vec is null))
          return _c0Vec;
        else
          throw NotExecutedException();
      }
    }

    /// <summary>
    /// Returns the spline coefficient of order 1 (linear term).
    /// </summary>
    public IROVector<double> Coefficient1
    {
      get
      {
        if (_interpolationSuccessfullyExecuted && !(_c1Vec is null))
          return _c1Vec;
        else
          throw NotExecutedException();

      }
    }

    /// <summary>
    /// Returns the spline coefficient of order 2 (quadratic term).
    /// </summary>
    public IROVector<double> Coefficient2
    {
      get
      {
        if (_interpolationSuccessfullyExecuted && !(_c2Vec is null))
          return _c2Vec;
        else
          throw NotExecutedException();

      }
    }

    /// <summary>
    /// Returns the spline coefficient of order 2 (cubic term).
    /// </summary>
    public IROVector<double> Coefficient3
    {
      get
      {
        if (_interpolationSuccessfullyExecuted && !(_c3Vec is null))
          return _c3Vec;
        else
          throw NotExecutedException();

      }
    }

    /// <summary>
    /// Returns the error estimates of the y points.
    /// </summary>
    public IROVector<double> ErrorEstimate
    {
      get
      {
        if (_interpolationSuccessfullyExecuted && _standardErrorEstimatesCalculated && !(_seVec is null))
          return _seVec;
        else
          throw NotExecutedException();

      }
    }

    /// <summary>
    /// Smoothing parameter = rho/(rho+1), that varies between 0 (not smoothed) and 1 (full smoothed).
    /// If the value is 0 (rho=0) an interpolating natural cubic spline has been calculated.
    /// If  the value is 1 (rho=infinite) a least squares regression line has been calculated.
    /// </summary>
    public double SmoothingParameter
    {
      get
      {
        if (_interpolationSuccessfullyExecuted && !(_wkr is null))
          return _wkr[0][0];
        else
          throw NotExecutedException();
      }
    }

    /// <summary>
    /// Estimate of the number of degrees of
    /// freedom of the residual sum of squares
    /// which reduces to the usual value of n-2
    /// when a least squares regression line
    /// was calculated.
    /// </summary>
    public double EstimatedDegreesOfFreedom
    {
      get
      {
        if (_interpolationSuccessfullyExecuted && !(_wkr is null))
          return _wkr[1][0];
        else
          throw NotExecutedException();
      }
    }

    /// <summary>
    /// Generalized cross validation.
    /// </summary>
    public double GeneralizedCrossValidation
    {
      get
      {
        if (_interpolationSuccessfullyExecuted && !(_wkr is null))
          return _wkr[2][0];
        else
          throw NotExecutedException();
      }
    }

    /// <summary>
    /// Mean square residual.
    /// </summary>
    public double MeanSquareResidual
    {
      get
      {
        if (_interpolationSuccessfullyExecuted && !(_wkt is null))
          return _wkt[0][0];
        else
          throw NotExecutedException();
      }
    }

    /// <summary>
    /// Estimate of the true mean square error at the data points.
    /// </summary>
    public double EstimatedTrueMeanSquareError
    {
      get
      {
        if (_interpolationSuccessfullyExecuted && !(_wkt is null))
          return _wkt[1][0];
        else
          throw NotExecutedException();
      }
    }

    /// <summary>
    /// Estimate of the error variance.
    /// The value coincides with the output value of var if var is negative on input.
    /// It is calculated with the unscaled values of the df[i] to facilitate
    /// comparisons with a priori variance estimates.
    /// </summary>
    public double EstimatedErrorVariance
    {
      get
      {
        if (_interpolationSuccessfullyExecuted && !(_wku is null))
          return _wku[0];
        else
          throw NotExecutedException();
      }
    }

    /// <summary>
    /// Mean square value of the error variances in dy[i] (if they were provided).
    /// The values of <see cref="GeneralizedCrossValidation"/>, <see cref="MeanSquareResidual"/> and <see cref="EstimatedTrueMeanSquareError"/>
    /// are calculated with the dy[i] scaled to have a mean square value 1.
    /// The unscaled values of <see cref="GeneralizedCrossValidation"/>, <see cref="MeanSquareResidual"/> and <see cref="EstimatedTrueMeanSquareError"/>
    /// may be calculated by dividing by this value.
    /// </summary>
    public double MeanSquareOfInputVariance
    {
      get
      {
        if (_interpolationSuccessfullyExecuted && !(_wkv is null))
          return _wkv[0];
        else
          throw NotExecutedException();
      }
    }



    #endregion Fit results

    #region Low level functions

    #region cubgcv

    /// <summary>
    ///
    /// </summary>
    /// <param name="x">Input: Abscissae of the N data points. Must be ordered so that x[i] &lt; x[i+1].</param>
    /// <param name="f">Input: Ordinates (function values) of the N data points.</param>
    /// <param name="df">Input/Output: df[i] is the relative standard deviation of the error associated with the data point i.
    /// Each df[i] must be positive. The values in df are scaled so that their mean square value is 1, and unscaled again on normal exit.
    /// The mean squaree value of the df[i] is returned in WK3[i] on normal exit.
    /// If the absolute standard deviations are known, these should be provided in df and the error
    /// variance parameter <paramref name="var"/> (see below) should then be set to 1.
    /// If the relative standard deviations are unknown, set each df[i] = 1.
    ///  </param>
    /// <param name="n">Number of data points. Must be &gt;=3.</param>
    /// <param name="y">Output: Spline coefficients of order 0.</param>
    /// <param name="C">Output: Spline coefficients of order 1, 2, and 3.
    /// THE VALUE
    /// OF THE SPLINE APPROXIMATION AT T IS
    /// S(T)=((C(I,3)*D+C(I,2))*D+C(I,1))*D+Y(I)
    /// WHERE X(I).LE.T.LT.X(I+1) AND
    ///  D = T-X(I). </param>
    /// <param name="ic">Input:
    /// ROW DIMENSION OF MATRIX C EXACTLY
    /// AS SPECIFIED IN THE DIMENSION
    /// STATEMENT IN THE CALLING PROGRAM.
    /// </param>
    /// <param name="var">Input/Output:
    /// ERROR VARIANCE. (INPUT/OUTPUT)
    /// IF VAR IS NEGATIVE(I.E.UNKNOWN) THEN
    ///                           THE SMOOTHING PARAMETER IS DETERMINED
    ///                           BY MINIMIZING THE GENERALIZED CROSS VALIDATION
    /// AND AN ESTIMATE OF THE ERROR VARIANCE IS
    ///                           RETURNED IN VAR.
    /// IF VAR IS NON-NEGATIVE(I.E.KNOWN) THEN THE
    /// SMOOTHING PARAMETER IS DETERMINED TO MINIMIZE
    ///                           AN ESTIMATE, WHICH DEPENDS ON VAR, OF THE TRUE
    ///                           MEAN SQUARE ERROR, AND VAR IS UNCHANGED.
    ///                           IN PARTICULAR, IF VAR IS ZERO, THEN AN
    /// INTERPOLATING NATURAL CUBIC SPLINE IS CALCULATED.
    /// VAR SHOULD BE SET TO 1 IF ABSOLUTE STANDARD
    ///                           DEVIATIONS HAVE BEEN PROVIDED IN DF (SEE ABOVE).
    /// </param>
    /// <param name="job">Input: JOB SELECTION PARAMETER.
    ///JOB = 0 SHOULD BE SELECTED IF POINT STANDARD ERROR
    ///                           ESTIMATES ARE NOT REQUIRED IN SE.
    ///                         JOB = 1 SHOULD BE SELECTED IF POINT STANDARD ERROR
    ///                           ESTIMATES ARE REQUIRED IN SE.
    ///                           </param>
    /// <param name="se">
    /// SE     - VECTOR OF LENGTH N CONTAINING BAYESIAN STANDARD
    ///                           ERROR ESTIMATES OF THE FITTED SPLINE VALUES IN Y.
    /// SE IS NOT REFERENCED IF JOB=0. (OUTPUT)
    /// </param>
    /// <param name="WK0">
    /// WK     - WORK VECTOR OF LENGTH 7*(N + 2). ON NORMAL EXIT THE
    /// FIRST 7 VALUES OF WK ARE ASSIGNED AS FOLLOWS:-
    ///
    ///                           WK(1) = SMOOTHING PARAMETER(= RHO/(RHO + 1))
    /// WK(2) = ESTIMATE OF THE NUMBER OF DEGREES OF
    ///                                   FREEDOM OF THE RESIDUAL SUM OF SQUARES
    ///                           WK(3) = GENERALIZED CROSS VALIDATION
    ///                           WK(4) = MEAN SQUARE RESIDUAL
    ///                           WK(5) = ESTIMATE OF THE TRUE MEAN SQUARE ERROR
    ///                                   AT THE DATA POINTS
    /// WK(6) = ESTIMATE OF THE ERROR VARIANCE
    ///                           WK(7) = MEAN SQUARE VALUE OF THE DF(I)
    ///
    ///                           IF WK(1)=0 (RHO=0) AN INTERPOLATING NATURAL CUBIC
    /// SPLINE HAS BEEN CALCULATED.
    /// IF WK(1)=1 (RHO=INFINITE) A LEAST SQUARES
    ///                           REGRESSION LINE HAS BEEN CALCULATED.
    /// WK(2) IS AN ESTIMATE OF THE NUMBER OF DEGREES OF
    ///                           FREEDOM OF THE RESIDUAL WHICH REDUCES TO THE
    /// USUAL VALUE OF N-2 WHEN A LEAST SQUARES REGRESSION
    ///                           LINE IS CALCULATED.
    /// WK(3),WK(4),WK(5) ARE CALCULATED WITH THE DF(I)
    /// SCALED TO HAVE MEAN SQUARE VALUE 1.  THE
    ///                           UNSCALED VALUES OF WK(3),WK(4),WK(5) MAY BE
    /// CALCULATED BY DIVIDING BY WK(7).
    /// WK(6) COINCIDES WITH THE OUTPUT VALUE OF VAR IF
    /// VAR IS NEGATIVE ON INPUT.IT IS CALCULATED WITH
    ///                           THE UNSCALED VALUES OF THE DF(I) TO FACILITATE
    /// COMPARISONS WITH A PRIORI VARIANCE ESTIMATES.
    /// </param>
    /// <param name="WK1"></param>
    /// <param name="WK2"></param>
    /// <param name="WK3"></param>
    /// <param name="ier">
    ///  IER    - ERROR PARAMETER. (OUTPUT)
    ///  TERMINAL ERROR
    ///                            IER = 129, IC IS LESS THAN N-1.
    ///  IER = 130, N IS LESS THAN 3.
    ///  IER = 131, INPUT ABSCISSAE ARE NOT
    ///                              ORDERED SO THAT X(I).LT.X(I+1).
    ///  IER = 132, DF(I)IS NOT POSITIVE FOR SOME I.
    ///                            IER = 133, JOB IS NOT 0 OR 1.
    /// </param>
    public static void cubgcv(double[] x, double[] f, double[] df, int n, double[] y, double[][] C, int ic, double var, int job, double[] se,
        double[][] WK0, double[][] WK1, double[] WK2, double[] WK3, out int ier)
    {
      const double tau = 1.6180339887498948482045868343656; // 0.5*(Math.Sqrt(5)+1)
      const double ratio = 2.0;

      double delta, err, gf1, gf2, gf3 = 0, gf4, r1, r2;
      double r3 = 0;
      double r4;
      double avh, avdf = 0.0, avar;
      double[] stat = new double[6];
      double p, q;

      bool done = false;
      int i;

      /* Initialize */
      ier = 133;
      if (job >= 0 && job <= 1)
      {
        spint1(x, out avh, f, df, out avdf, n, y, C, ic, WK0, WK1, ref ier);
        if (ier == 0)
        {
          avar = var;
          if (var > 0)
            avar = var * avdf * avdf;

          /* Check for zero variance */
          if (0 != var)
          {

            /* Find local minimum of gcv or the expected mean square error */
            r1 = 1;
            r2 = ratio * r1;
            spfit1(x, avh, df, n, r2, out p, out q, out gf2, avar, stat, y, C, ic, WK0, WK1, WK2, WK3);
            for (; ; )
            {
              spfit1(x, avh, df, n, r1, out p, out q, out gf1, avar, stat, y, C, ic, WK0, WK1, WK2, WK3);
              if (gf1 > gf2)
              {
                break;
              }
              if (p > 0)
              {
                r2 = r1;
                gf2 = gf1;
                r1 /= ratio;
              }
              else  /* Exit if p zero */
              {
                done = true;
                break;
              }
            }


            if (!done)
            {
              r3 = ratio * r2;
              for (; ; )
              {
                spfit1(x, avh, df, n, r3, out p, out q, out gf3, avar, stat, y, C, ic, WK0, WK1, WK2, WK3);
                if (gf3 > gf2)
                {
                  break;
                }

                if (q > 0)
                {
                  r2 = r3;
                  gf2 = gf3;
                  r3 *= ratio;
                }
                else /* Exit if q zero */
                {
                  done = true;
                  break;
                }
              }
            }

            if (!done)
            {
              r2 = r3;
              gf2 = gf3;
              delta = (r2 - r1) / tau;
              r4 = r1 + delta;
              r3 = r2 - delta;
              spfit1(x, avh, df, n, r3, out p, out q, out gf3, avar, stat, y, C, ic, WK0, WK1, WK2, WK3);
              spfit1(x, avh, df, n, r4, out p, out q, out gf4, avar, stat, y, C, ic, WK0, WK1, WK2, WK3);
              do
              {

                /* Golden section search for local minimum */
                if (!(gf3 > gf4))
                {
                  r2 = r4;
                  gf2 = gf4;
                  r4 = r3;
                  gf4 = gf3;
                  delta /= tau;
                  r3 = r2 - delta;
                  spfit1(x, avh, df, n, r3, out p, out q, out gf3, avar, stat, y, C, ic, WK0, WK1, WK2, WK3);
                }
                else
                {
                  r1 = r3;
                  gf1 = gf3;
                  r3 = r4;
                  gf3 = gf4;
                  delta /= tau;
                  r4 = r1 + delta;
                  spfit1(x, avh, df, n, r4, out p, out q, out gf4, avar, stat, y, C, ic, WK0, WK1, WK2, WK3);
                }

                err = (r2 - r1) / (r1 + r2);
              }
              while (err * err + 1 > 1 && err > 1E-6);
              r1 = (r1 + r2) * 0.5;
              spfit1(x, avh, df, n, r1, out p, out q, out gf1, avar, stat, y, C, ic, WK0, WK1, WK2, WK3);
            } // if !done
          }
          else // var == 0
          {
            r1 = 0;
            spfit1(x, avh, df, n, r1, out p, out q, out gf1, avar, stat, y, C, ic, WK0, WK1, WK2, WK3);
          }

          /* Calculate spline coefficients */
          spcof1(x, avh, f, df, n, p, q, y, C, ic, WK2, WK3);

          /* Optionally calculate standard error estimates */
          if (var < 0)
          {
            avar = stat[5];
            var = avar / (avdf * avdf);
          }
          if (job == 1)
          {
            sperr1(x, avh, df, n, WK0, p, avar, se);
          }

          /* Unscale df */
          for (i = 0; i < n; i++) df[i] = df[i] * avdf;

          /* Put statistics in wk */
          WK0[0][0] = stat[0];
          WK0[1][0] = stat[1];
          WK0[2][0] = stat[2];
          WK1[0][0] = stat[3];
          WK1[1][0] = stat[4];
          WK2[0] = stat[5] / (avdf * avdf);
          WK3[0] = avdf * avdf;
        }
      }
    }
    #endregion cubgcv

    #region spint

    /// <summary>
    /// Initializes the arrays c, r and t for one dimensional cubic
    /// smoothing spline fitting by subroutine spfit1. The values
    /// df(i) are scaled so that the sum of their squares is n
    /// and the average of the differences x(i+1) - x(i) is calculated
    /// in avh in order to avoid underflow and overflow problems in
    /// spfit1.
    /// This subroutine sets ier if elements of x are non-increasing,
    /// if n is less than 3, if ic is less than n-1 or if dy(i) is
    /// not positive for some i.
    /// </summary>
    /// <param name="x">Abscissae of the N data points. Must be ordered so that x[i] &lt; x[i+1].</param>
    /// <param name="avh">Output: contains the average of the differences x(i+1) - x(i).</param>
    /// <param name="y">Ordinates of the N data points.</param>
    /// <param name="dy">Relative standard deviation of the error associated with the data point i.
    /// Each df[i] must be positive. The values are scaled so that the sum of their squares is n.</param>
    /// <param name="avdy">Contains the scaling factor for dy.</param>
    /// <param name="n">Number of data points.</param>
    /// <param name="a">Spline coefficients of order 0 to be initialized.</param>
    /// <param name="C">Spline coeffients of order 1, 2 and 3 to be initialized.</param>
    /// <param name="ic">Number of coefficents of order 1, 2 or 3. Usually one less than n.</param>
    /// <param name="R">Work array to be initialized.</param>
    /// <param name="T">Work array to be initialized.</param>
    /// <param name="ier">Output: 0 on success, or an error number if failed.</param>
    public static void spint1(double[] x, out double avh, double[] y, double[] dy, out double avdy, int n, double[] a, double[][] C, int ic,
                    double[][] R, double[][] T, ref int ier)

    {
      int i;
      int done = 0;
      double e, f, g, h;
      avh = 0;
      avdy = 0;

      /* Initialization and input checking */
      ier = 0;
      if (n < 3)
        ier = 130;
      else if (ic < n - 1)
        ier = 129;
      else
      {

        /* Get average x spacing in avh */
        g = 0;
        for (i = 1; i < n; i++)
        {
          h = x[i] - x[i - 1];
          if (h <= 0)
          {
            done = 1;
            break;
          }
          else
            g += h;
        }
        if (0 == done)
        {
          avh = g / (n - 1);

          /* Scale relative weights */
          g = 0;
          for (i = 0; i < n; i++)
            if (dy[i] <= 0)
            {
              done = 2;
              break;
            }
            else
              g += dy[i] * dy[i];
        }
        if (0 == done)
        {
          avdy = Math.Sqrt(g / n);
          for (i = 0; i < n; i++)
            dy[i] /= avdy;

          /* Initialize h, f */
          h = (x[1] - x[0]) / avh;
          f = (y[1] - y[0]) / h;

          /* Calculate a, t, r */
          for (i = 1; i < n - 1; i++)
          {
            g = h;
            h = (x[i + 1] - x[i]) / avh;
            e = f;
            f = (y[i + 1] - y[i]) / h;
            a[i] = f - e;
            T[0][i + 1] = 2 * (g + h) / 3;
            T[1][i + 1] = h / 3;
            R[2][i + 1] = dy[i - 1] / g;
            R[0][i + 1] = dy[i + 1] / h;
            R[1][i + 1] = -dy[i] / g - dy[i] / h;
          }

          /* Calculate c = r'*r */
          R[1][n] = 0;
          R[2][n] = 0;
          R[2][n + 1] = 0;
          for (i = 1; i < n - 1; i++)
          {
            C[0][i] = R[0][i + 1] * R[0][i + 1] + R[1][i + 1] * R[1][i + 1] + R[2][i + 1] * R[2][i + 1];
            C[1][i] = R[0][i + 1] * R[1][i + 2] + R[1][i + 1] * R[2][i + 2];
            C[2][i] = R[0][i + 1] * R[2][i + 3];
          }
          return;
        }
        if (done == 1)
          ier = 131;
        else
          ier = 132;
      }
    }
    #endregion spint

    #region spfit


    /// <summary>
    /// Fits a cubic smoothing spline to data with relative
    /// weighting dy for a given value of the smoothing parameter
    /// rho using an algorithm based on that of C.H.Reinsch (1967),
    /// Numer. Math. 10, 177-183.
    /// The trace of the influence matrix is calculated using an
    /// algorithm developed by M.F.hutchinson and F.R.de Hoog (Numer.
    /// Math., in press), enabling the generalized cross validation
    /// and related statistics to be calculated in order n operations.
    /// The arrays a, c, r and t are assumed to have been initialized
    /// by the subroutine spint.  Overflow and underflow problems are
    /// avoided by using p=rho/(1 + rho) and q=1/(1 + rho) instead of
    /// rho and by scaling the differences x[i+1] - x[i] by avh.
    /// the values in df are assumed to have been scaled so that the
    /// sum of their squared values is n.  The value in var, when it is
    /// non-negative, is assumed to have been scaled to compensate for
    /// the scaling of the values in df.
    /// The value returned in fun is an estimate of the true mean square
    /// when var is non-negative, and is the generalized cross validation
    /// when var is negative.
    /// </summary>
    /// <param name="x">Abscissae values of the data points.</param>
    /// <param name="avh">Scaling parameter for the x-intervals.</param>
    /// <param name="dy"></param>
    /// <param name="n">Number of data points.</param>
    /// <param name="rho">Smooting parameter (0.. Infinity).</param>
    /// <param name="p">Is equal to rho/(1 + rho).</param>
    /// <param name="q">Is equal to 1/(1 + rho).</param>
    /// <param name="fun">Estimate of the true mean square when var is non-negative, and is the generalized cross validation when var is negative.</param>
    /// <param name="var">Variance of the ordinate values of the data points (if known), or a negative value (if unkown).</param>
    /// <param name="stat">Array holding some statistical values on return.</param>
    /// <param name="a">Spline coefficients of order 0, i.e. the ordinate values of the spline (at the same abscissae values as the original data points).</param>
    /// <param name="C">Spline coefficients of order 1, 2 and 3.</param>
    /// <param name="ic">Number of coefficents of order 1,2 and 3. Is one less the number of data points.</param>
    /// <param name="R">Work array.</param>
    /// <param name="T">Work array.</param>
    /// <param name="u">Work array.</param>
    /// <param name="v">Work array.</param>
    static protected void spfit1(double[] x, double avh, double[] dy, int n, double rho, out double p, out double q, out double fun,
                 double var, double[] stat, double[] a, double[][] C, int ic, double[][] R, double[][] T, double[] u, double[] v)

    {
      double e, f, g, h, rho1;
      int i;

      /* Use p and q instead of rho to prevent overflow or underflow */
      rho1 = rho + 1;
      p = rho / rho1;
      q = 1 / rho1;
      if (1 == rho1)
        p = 0;
      if (rho1 == rho)
        q = 0;

      /* Rational cholesky decomposition of p*c + q*t */
      f = g = h = 0;
      for (i = 0; i < 2; i++)
        R[0][i] = 0;
      for (i = 2; i < n; i++)
      {
        R[2][i - 2] = g * R[0][i - 2];
        R[1][i - 1] = f * R[0][i - 1];
        R[0][i] = 1 / (p * C[0][i - 1] + q * T[0][i] - f * R[1][i - 1] - g * R[2][i - 2]);
        f = p * C[1][i - 1] + q * T[1][i] - h * R[1][i - 1];
        g = h;
        h = p * C[2][i - 1];
      }

      /* Solve for u */
      u[0] = 0;
      u[1] = 0;
      for (i = 2; i < n; i++)
        u[i] = a[i - 1] - R[1][i - 1] * u[i - 1] - R[2][i - 2] * u[i - 2];
      u[n] = 0;
      u[n + 1] = 0;
      for (i = n - 1; i > 1; i--)
        u[i] = R[0][i] * u[i] - R[1][i] * u[i + 1] - R[2][i] * u[i + 2];

      /* Calculate residual vector v */
      e = 0;
      h = 0;
      for (i = 1; i < n; i++)
      {
        g = h;
        h = (u[i + 1] - u[i]) / ((x[i] - x[i - 1]) / avh);
        v[i] = dy[i - 1] * (h - g);
        e += v[i] * v[i];
      }
      v[n] = dy[n - 1] * (-h);
      e += v[n] * v[n];

      /* Calculate upper three bands of inverse matrix */
      R[0][n] = 0;
      R[1][n] = 0;
      R[0][n + 1] = 0;
      for (i = n - 1; i > 1; i--)
      {
        g = R[1][i];
        h = R[2][i];
        R[1][i] = -g * R[0][i + 1] - h * R[1][i + 1];
        R[2][i] = -g * R[1][i + 1] - h * R[0][i + 2];
        R[0][i] -= (g * R[1][i] + h * R[2][i]);
      }

      /* Calculate trace */
      f = g = h = 0;
      for (i = 2; i < n; i++)
      {
        f += (R[0][i] * C[0][i - 1]);
        g += (R[1][i] * C[1][i - 1]);
        h += (R[2][i] * C[2][i - 1]);
      }
      f += 2 * (g + h);

      /* Calculate statistics */
      stat[0] = p;
      stat[1] = f * p;
      stat[2] = n * e / (f * f);
      stat[3] = e * p * p / n;
      stat[5] = e * p / f;
      if (var >= 0)
      {
        stat[4] = Math.Max(stat[3] - 2 * var * stat[1] / n + var, 0);
        fun = stat[4];
      }
      else
      {
        stat[4] = stat[5] - stat[3];
        fun = stat[2];
      }
    }

    #endregion spfit

    #region sperr


    /// <summary>
    /// Calculates bayesian estimates of the standard errors of the fitted
    /// values of a cubic smoothing spline by calculating the diagonal elements
    /// of the influence matrix.
    /// </summary>
    /// <param name="x">Abscissae values of the data points.</param>
    /// <param name="avh">Scaling parameter for the x-intervals.</param>
    /// <param name="dy">Relative standard deviation of the error associated with the data point i.
    /// Each dy[i] must be positive. The values were scaled by spint so that the sum of their squares is n.</param>
    /// <param name="n">Number of data points.</param>
    /// <param name="R">Work array.</param>
    /// <param name="p"></param>
    /// <param name="var"></param>
    /// <param name="se">On return: contains the calculated standard errors.</param>
    protected static void sperr1(double[] x, double avh, double[] dy, int n, double[][] R, double p, double var, double[] se)

    {
      int i;
      double f, g, h, f1, g1, h1;

      /* Initialize */
      h = avh / (x[1] - x[0]);
      se[0] = 1 - (p) * dy[0] * dy[0] * h * h * R[0][2];
      R[0][1] = 0;
      R[1][1] = 0;
      R[2][1] = 0;

      /* Calculate diagonal elements */
      for (i = 2; i < n; i++)
      {
        f = h;
        h = avh / (x[i] - x[i - 1]);
        g = -f - h;
        f1 = f * R[0][i - 1] + g * R[1][i - 1] + h * R[2][i - 1];
        g1 = f * R[1][i - 1] + g * R[0][i] + h * R[1][i];
        h1 = f * R[2][i - 1] + g * R[1][i] + h * R[0][i + 1];
        se[i - 1] = 1 - (p) * dy[i - 1] * dy[i - 1] * (f * f1 + g * g1 + h * h1);
      }
      se[n - 1] = 1 - (p) * dy[n - 1] * dy[n - 1] * h * h * R[0][n - 1];

      /* Calculate standard error estimates */
      for (i = 0; i < n; i++)
        se[i] = (se[i] * (var) >= 0) ? Math.Sqrt(se[i] * (var)) * dy[i] : 0;
    }

    #endregion sperr

    #region spcof

    /// <summary>
    /// Calculates coefficients of a cubic smoothing spline from
    /// parameters calculated by subroutine spfit.
    /// </summary>
    /// <param name="x">Abscissae of the N data points. Must be ordered so that x[i] &lt; x[i+1].</param>
    /// <param name="avh">Average of the differences x(i+1) - x(i).</param>
    /// <param name="y">Ordinates of the N data points.</param>
    /// <param name="dy">Relative standard deviation of the error associated with the data point i. Each element must be positive. The values are scaled so that the sum of their squares is n.</param>
    /// <param name="n">Number of data points.</param>
    /// <param name="p">Is equal to rho/(1 + rho), in which rho is the smoothing parameter.</param>
    /// <param name="q">Is equal to 1/(1 + rho), in which rho is the smoothing parameter.</param>
    /// <param name="a">Spline coefficients of order 0, i.e. the ordinate values of the spline (at the same abscissae values as the original data points).</param>
    /// <param name="C">Spline coefficients of order 1, 2 and 3.</param>
    /// <param name="ic">Number of coefficents of order 1,2 and 3. Is one less the number of data points.</param>
    /// <param name="u">Work array.</param>
    /// <param name="v">Work array.</param>
    static protected void spcof1(double[] x, double avh, double[] y, double[] dy, int n, double p, double q, double[] a,
          double[][] C, int ic, double[] u, double[] v)

    {
      int i;
      double h, qh;

      /* Calculate a */
      qh = q / (avh * avh);
      for (i = 0; i < n; i++)
      {
        a[i] = y[i] - p * dy[i] * v[i + 1];
        u[i + 1] *= qh;
      }

      /* Calculate c */
      for (i = 1; i < n; i++)
      {
        h = x[i] - x[i - 1];
        C[2][i - 1] = (u[i + 1] - u[i]) / (3 * h);
        C[0][i - 1] = (a[i] - a[i - 1]) / h - (h * C[2][i - 1] + u[i]) * h;
        C[1][i - 1] = u[i];
      }
    }

    #endregion spcof

    #endregion Low level functions

    public override double GetXOfU(double u)
    {
      return u;
    }

    public double GetYOfX(double x)
    {
      return GetYOfU(x);
    }

    public double GetY1stDerivativeOfX(double xx)
    {
      if (_interpolationSuccessfullyExecuted)
        return CubicSplineHorner1stDerivative(xx, x, _c0Vec!, _c1Vec!, _c2Vec!, _c3Vec!);
      else
        throw NotExecutedException();
    }

    public override double GetYOfU(double u)
    {
      if (_interpolationSuccessfullyExecuted)
        return CubicSplineHorner(u, x, _c0Vec!, _c1Vec!, _c2Vec!, _c3Vec!);
      else
        throw NotExecutedException();
    }



    /// <summary>
    /// If the error variance of the provided points is unknown, set this value to -1. Then a cross validating cubic spline is fitted to the data.
    /// If the error variance is known and is equal for all points, set this value to the error variance of the points.
    /// If the error variance is known and different for each point, set this value to 1, and provide the error variance for each point
    /// by calling <see cref="Interpolate(IReadOnlyList{double}, IReadOnlyList{double}, double, IReadOnlyList{double})"/>.
    /// </summary>
    public double ErrorVariance
    {
      get
      {
        return _variance;
      }
      set
      {
        if (double.IsNaN(value))
          throw new ArgumentException("Value must not be NaN", nameof(value));

        if (!(value >= 0))
          _variance = -1;
        else
          _variance = value;
      }
    }
  }
}
