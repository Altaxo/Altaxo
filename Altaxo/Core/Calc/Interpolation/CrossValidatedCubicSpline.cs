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
  /// If the error variance
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
  public class CrossValidatedCubicSpline : SmoothingCubicSplineBase, IInterpolationFunction
  {
    public CrossValidatedCubicSpline()
    {
      _variance = -1.0; // unknown variance
    }

    protected override void InterpolationKernel(
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
      out int ier)
    {
#nullable disable
      cubgcv(_x, _f, _df, n, _y0, _c, n - 1, _variance, 1, _se, _wkr, _wkt, _wku, _wkv, out ier);
#nullable enable
    }


  }
}
