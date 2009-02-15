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
	/// Fortran source code transfered to C++ by B.M.Gammel, Physik Department,
	/// TU Muenchen, 8046 Garching, Germany. Revision of september 1992.
	/// 
	/// C++ source code transfered to C# by Dirk Lellinger.
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
	public class CrossValidatedCubicSpline : CurveBase, IInterpolationFunction
	{

		//----------------------------------------------------------------------------//
		// static globals
		//----------------------------------------------------------------------------//

		const double zero = 0.0;
		const double one = 1.0;
		const double two = 2.0;


		//----------------------------------------------------------------------------//
		// error flags
		//----------------------------------------------------------------------------//

		enum ErrorFlag
		{
			no_error = 0,
			too_few_datapoints = 1,
			abscissa_not_ordered = 2,
			stddev_non_positive = 3
		};

		protected bool calculateErrorEstimates;
		protected double var;
		protected DoubleVector xstore = new DoubleVector();
		protected DoubleVector ystore = new DoubleVector();
		protected DoubleVector dy = new DoubleVector();
		protected DoubleVector y0 = new DoubleVector();
		protected DoubleVector y1 = new DoubleVector();
		protected DoubleVector y2 = new DoubleVector();
		protected DoubleVector y3 = new DoubleVector();
		protected DoubleVector se = new DoubleVector();
		protected DoubleVector wkr = new DoubleVector();
		protected DoubleVector wkt = new DoubleVector();
		protected DoubleVector wku = new DoubleVector();
		protected DoubleVector wkv = new DoubleVector();

		public CrossValidatedCubicSpline()
		{
			var = -1.0; // unknown variance
		}

		#region Input parameters

		/// <summary>
		/// Set this value to true when you want to get the error estimate vector.
		/// </summary>
		public bool CalculateErrorEstimates
		{
			get
			{
				return calculateErrorEstimates;
			}
			set
			{
				calculateErrorEstimates = value;
			}
		}

		#endregion


		#region Fit results

		/// <summary>
		/// Returns the spline coefficient of order 0. This are the splined y values at the positions given by x.
		/// </summary>
		public IROVector Coefficient0
		{
			get
			{
				return y0;
			}
		}

		/// <summary>
		/// Returns the spline coefficient of order 1 (linear term).
		/// </summary>
		public IROVector Coefficient1
		{
			get
			{
				return y1;
			}
		}

		/// <summary>
		/// Returns the spline coefficient of order 2 (quadratic term).
		/// </summary>
		public IROVector Coefficient2
		{
			get
			{
				return y2;
			}
		}

		/// <summary>
		/// Returns the spline coefficient of order 2 (cubic term).
		/// </summary>
		public IROVector Coefficient3
		{
			get
			{
				return y3;
			}
		}

		/// <summary>
		/// Returns the error estimates of the y points.
		/// </summary>
		public IROVector ErrorEstimate
		{
			get
			{
				return se;
			}
		}

		/// <summary>
		/// Smoothing parameter = rho/(rho+1).
		/// If the value is 0 (rho=0) an interpolating natural cubic spline has been calculated.
		/// If  the value is 1 (rho=infinite) a least squares regression line has been calculated.
		/// </summary>
		public double SmoothingParameter
		{
			get
			{
				return wkr[0];
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
				return wkr[1];
			}
		}

		/// <summary>
		/// Generalized cross validation.
		/// </summary>
		public double GeneralizedCrossValidation
		{
			get
			{
				return wkr[2];
			}
		}

		/// <summary>
		/// Mean square residual.
		/// </summary>
		public double MeanSquareResidual
		{
			get
			{
				return wkr[3];
			}
		}

		/// <summary>
		/// Estimate of the true mean square error at the data points.
		/// </summary>
		public double EstimatedTrueMeanSquareError
		{
			get
			{
				return wkr[4];
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
				return wkr[5];
			}
		}
		
		/// <summary>
		/// Mean square value of the dyy[i] (i.e. the provided variance values by <see cref="SetErrorVariance"/>).
		/// The values of <see cref="GeneralizedCrossValidation"/>, <see cref="MeanSquareResidual"/> and <see cref="EstimatedTrueMeanSquareError"/>
		/// are calculated with the dyy[i] scaled to have mean square value 1.
		/// The unscaled values of <see cref="GeneralizedCrossValidation"/>, <see cref="MeanSquareResidual"/> and <see cref="EstimatedTrueMeanSquareError"/>
		/// may be calculated by dividing by this value.
		/// </summary>
		public double MeanSquareOfInputVariance
		{
			get
			{
				return wkr[6];
			}
		}

		#endregion



		#region Low level functions

		#region cubgcv

		private int cubgcv(double[] xx, double[] f, double[] df, int n, double[] yy, double[] c1, double[] c2, double[] c3, double[] ss, double[] wwr, double[] wwt, double[] wwu, double[] wwv)
		{
			// three or more points
			const double ratio = 2.0;
			double tau = (Math.Sqrt(5.0) + 1.0) / 2.0;

			ErrorFlag error_flag;
			int i, wk_dim1;

			double avdf, avar, gf1, gf2, gf3, gf4,
				avh, err, p, q, delta, r1, r2, r3, r4;
			double[] stat = new double[6];


			// Parameter adjustments
			wk_dim1 = n + 2;


			spint(n, xx, out avh, f, df, out avdf, yy, c1, c2, c3,
				wwr, wwt, out error_flag); // Note wwr has 3*(N+2), wwt has 2*(N+2)

			if (ErrorFlag.no_error != error_flag)
				return (int)error_flag;

			avar = var;
			if (var > zero) avar = var * avdf * avdf;

			// check for zero variance, i.e. compute a natural cubic spline
			if (var == zero)
			{
				r1 = zero;
				goto natural_spline;
			}

			// find local minimum of gcv or the expected mean square error
			r1 = one;
			r2 = ratio * r1;
			spfit(n, xx, avh, df, r2, out p, out q, out gf2, avar, stat, yy, c1, c2, c3,
				wwr, wwt,
				wwu, // [wk_dim1 * 6],
				wwv //[wk_dim1 * 7]
				);

			for (; ; )
			{
				spfit(n, xx, avh, df, r1, out p, out q, out gf1, avar, stat, yy, c1, c2, c3,
					wwr, //[wk_offset]
					wwt, //[wk_dim1 * 4],
					wwu, //[wk_dim1 * 6]
					wwv  //[wk_dim1 * 7]
					);
				if (gf1 > gf2) break;
				// exit if p is zero
				if (p <= zero) goto spline_coefficients;
				r2 = r1;
				gf2 = gf1;
				r1 /= ratio;
			}

			r3 = ratio * r2;

			for (; ; )
			{
				spfit(n, xx, avh, df, r3, out p, out q, out gf3, avar, stat, yy, c1, c2, c3,
					wwr, // [wk_offset]
					wwt, // [wk_dim1 * 4]
					wwu, // [wk_dim1 * 6]
					wwv //[wk_dim1 * 7]
					);

				if (gf3 > gf2) break;
				// exit if q is zero
				if (q <= zero) goto spline_coefficients;
				r2 = r3;
				gf2 = gf3;
				r3 = ratio * r3;
			}

			r2 = r3;
			gf2 = gf3;
			delta = (r2 - r1) / tau;
			r4 = r1 + delta;
			r3 = r2 - delta;
			spfit(n, xx, avh, df, r3, out p, out q, out gf3, avar, stat, yy, c1, c2, c3,
				wwr, // [wk_offset]
				wwt, // [wk_dim1 * 4]
				wwu, // [wk_dim1 * 6]
				wwv //[wk_dim1 * 7]
				);

			spfit(n, xx, avh, df, r4, out p, out q, out gf4, avar, stat, yy, c1, c2, c3,
				wwr, // [wk_offset]
				wwt, // [wk_dim1 * 4]
				wwu, // [wk_dim1 * 6]
				wwv //[wk_dim1 * 7]
				);

			do
			{  // golden section search for local minimum

				if (gf3 > gf4)
				{
					r1 = r3;
					gf1 = gf3;
					r3 = r4;
					gf3 = gf4;
					delta /= tau;
					r4 = r1 + delta;
					spfit(n, xx, avh, df, r4, out p, out q, out gf4, avar, stat, yy, c1, c2, c3,
						wwr, // [wk_offset]
						wwt, // [wk_dim1 * 4]
						wwu, // [wk_dim1 * 6]
						wwv //[wk_dim1 * 7]
						);
				}
				else
				{
					r2 = r4;
					gf2 = gf4;
					r4 = r3;
					gf4 = gf3;
					delta /= tau;
					r3 = r2 - delta;
					spfit(n, xx, avh, df, r3, out p, out q, out gf3, avar, stat, yy, c1, c2, c3,
						wwr, // [wk_offset]
						wwt, // [wk_dim1 * 4]
						wwu, // [wk_dim1 * 6]
						wwv //[wk_dim1 * 7]
						);
				}

				err = (r2 - r1) / (r1 + r2);

			} while (err * err + one > one && err > 1e-6);

			r1 = (r1 + r2) * 0.5;

		natural_spline:

			spfit(n, xx, avh, df, r1, out p, out q, out gf1, avar, stat, yy, c1, c2, c3,
				wwr, // [wk_offset]
				wwt, // [wk_dim1 * 4]
				wwu, // [wk_dim1 * 6]
				wwv //[wk_dim1 * 7]
				);

		spline_coefficients:

			spcof(n, xx, avh, f, df, p, q, yy, c1, c2, c3,
				wwu, //[wk_dim1 * 6]
				wwv  //[wk_dim1 * 7]
				);

			// optionally calculate standard error estimates
			if (var < zero)
			{
				avar = stat[5];
				var = avar / (avdf * avdf);
			}

			if (calculateErrorEstimates)
				sperr(n, xx, avh, df,
					wwr, //[wk_offset]
					p, avar, ss);

			// unscale df
			for (i = 0; i < n; ++i)
				df[i] *= avdf;

			// put statistics in wk
			for (i = 0; i <= 5; ++i)
				wwr[i] = stat[i];
			wwr[5] = stat[5] / (avdf * avdf);
			wwr[6] = avdf * avdf;

			return (int)error_flag;
		}


		#endregion


		#region spint

		// LelliD spint is now fully zero based
		static void spint(int n,
			double[] x, // Original 1..N , now 0..N-1
			out double avh,
			double[] y, // Original 1..N, now 0..N-1
			double[] dy, // Original 1..N, now 0..N-1
			out double avdy,
			double[] a,
			double[] c1, // Original 1..IC, now 0..N+1
			double[] c2, // Original 1..IC, now 0..N+1
			double[] c3, // Original 1..IC, now 0..N+1
			double[] r, // Original 0..N+1, 3 , now length=3*(N+2)
			double[] t, // Original 0..N+1, 2, now zero based with length=3*(N+2)
			out ErrorFlag error_flag)
		{
			int i, r_dim1, t_dim1;
			double e, f, g, h;

			// Initializes the arrays c1,c2,c3,r and t for one dimensional cubic
			// smoothing spline fitting by subroutine spfit. The values
			// df[i] are scaled so that the sum of their squares is n
			// and the average of the differences x[i+1]-x[i] is calculated
			// in avh in order to avoid underflow and overflow problems in
			// spfit. Subroutine sets error_flag if elements of x are non-increasing,
			// if n is less than 3 or if dy[i] is not positive for some i.

			// Parameter adjustments
			t_dim1 = n + 2;
			r_dim1 = n + 2;

			// initialization and input checking
			error_flag = ErrorFlag.no_error;
			if (n < 3)
			{
				avh = avdy = 0;
				error_flag = ErrorFlag.too_few_datapoints;
				return;
			}

			// get average x spacing in avh
			g = zero;
			for (i = 1; i < n; ++i)
			{
				h = x[i] - x[i - 1];
				// check if abscissae are not increasing
				if (h <= zero)
				{
					avh = avdy = 0;
					error_flag = ErrorFlag.abscissa_not_ordered;
					return;
				}
				g += h;
			}
			avh = g / (n - 1); // average spacing

			// scale relative weights
			g = zero;
			for (i = 0; i < n; ++i) // LelliD modified
			{
				// check for non positive df
				if (dy[i] <= zero)
				{
					avdy = 0;
					error_flag = ErrorFlag.stddev_non_positive; return;
				}
				g += dy[i] * dy[i];
			}
			avdy = Math.Sqrt(g / n);

			for (i = 0; i < n; ++i) // Lellid modified
				dy[i] /= avdy;


			// initialize h,f
			h = (x[1] - x[0]) / avh; // LelliD
			f = (y[1] - y[0]) / h; // LelliD

			// calculate a,t,r
			for (i = 2; i < n; ++i)
			{
				g = h;
				h = (x[i] - x[i - 1]) / avh; // LelliD
				e = f;
				f = (y[i] - y[i - 1]) / h; // LelliD
				a[i - 1] = f - e;   // LelliD
				t[i] = (g + h) * 2.0 / 3.0; // LelliD
				t[i + t_dim1] = h / 3.0; // LelliD
				r[i + r_dim1 * 2] = dy[i - 2] / g; // LelliD
				r[i] = dy[i] / h; // LelliD
				r[i + r_dim1] = -dy[i - 1] / g - dy[i - 1] / h; // LelliD
			}

			// calculate c = r'*r
			r[n + r_dim1] = 0;// LelliD
			r[n + r_dim1 * 2] = 0; // LelliD
			r[n + 1 + r_dim1 * 2] = 0; // LelliD

			for (i = 2; i < n; ++i)
			{
				c1[i - 1] = r[i] * r[i] + r[i + r_dim1] * // LelliD
					r[i + r_dim1] + r[i + r_dim1 * 2] * r[i + r_dim1 * 2]; // lelliD
				c2[i - 1] = r[i] * r[i + 1 + r_dim1] + r[i + r_dim1] * r[i + 1 + r_dim1 * 2]; // LelliD
				c3[i - 1] = r[i] * r[i + 2 + r_dim1 * 2]; // LelliD
			}
		}

		#endregion

		#region spfit
		//
		// Fits a cubic smoothing spline to data with relative
		// weighting dy for a given value of the smoothing parameter
		// rho using an algorithm based on that of C.H.Reinsch (1967),
		// Numer. Math. 10, 177-183.
		// The trace of the influence matrix is calculated using an
		// algorithm developed by M.F.hutchinson and F.R.de Hoog (Numer.
		// Math., in press), enabling the generalized cross validation
		// and related statistics to be calculated in order n operations.
		// The arrays a, c, r and t are assumed to have been initialized
		// by the subroutine spint.  Overflow and underflow problems are
		// avoided by using p=rho/(1 + rho) and q=1/(1 + rho) instead of
		// rho and by scaling the differences x[i+1] - x[i] by avh.
		// the values in df are assumed to have been scaled so that the
		// sum of their squared values is n.  The value in var, when it is
		// non-negative, is assumed to have been scaled to compensate for
		// the scaling of the values in df.
		// The value returned in fun is an estimate of the true mean square
		// when var is non-negative, and is the generalized cross validation
		// when var is negative.
		//
		// now all arrays zero based by LelliD
		static void spfit(int n,
			double[] x, // const double *x,
			double avh, // double *avh, 
			double[] dy, // const double *dy,
			double rho,
			out double p, // double *p,
			out double q, // double *q,
			out double fun, // double *fun,
			double var, // double *var,
			double[] stat, // double *stat,
			double[] a, // double *a,
			double[] c1, // double *c1, 
			double[] c2, // double *c2,
			double[] c3, // double *c3,
			double[] r, // double *r,
			double[] t, // double *t, 
			double[] u, // double *u,
			double[] v // double *v
			)
		{
			int i, r_dim1, t_dim1;
			double e, f, g, h, rho1, d1;

			// Parameter adjustments
			t_dim1 = n + 2;
			r_dim1 = n + 2;

			// use p and q instead of rho to prevent overflow or underflow
			rho1 = one + rho;
			p = rho / rho1;
			q = one / rho1;
			if (rho1 == one) p = zero;
			if (rho1 == rho) q = zero;

			// rational cholesky decomposition of p*c + q*t
			f = g = h = zero;

			for (i = 0; i <= 1; ++i)
				r[i] = zero; // LelliD

			for (i = 2; i < n; ++i)
			{
				r[i - 2 + r_dim1 * 2] = g * r[i - 2]; // LelliD
				r[i - 1 + r_dim1] = f * r[i - 1]; // LelliD
				r[i] = one / (p * c1[i - 1] + q * t[i] - f * r[i - 1 + r_dim1] - g * r[i - 2 + r_dim1 * 2]); // LelliD
				f = p * c2[i - 1] + q * t[i + t_dim1] - h * r[i - 1 + r_dim1]; // LelliD
				g = h;
				h = p * c3[i - 1]; // LelliD
			}

			// solve for u
			u[0] = u[1] = zero; // OK
			for (i = 2; i < n; ++i)
				u[i] = a[i - 1] - r[i - 1 + r_dim1] * u[i - 1] - r[i - 2 + r_dim1 * 2] * u[i - 2]; // LelliD
			u[n] = u[n + 1] = zero; // Ok
			for (i = n - 1; i >= 2; --i)
				u[i] = r[i] * u[i] - r[i + r_dim1] * u[i + 1] - r[i + r_dim1 * 2] * u[i + 2]; // LelliD

			// calculate residual vector v
			e = h = zero;
			for (i = 1; i < n; ++i)
			{
				g = h;
				h = (u[i + 1] - u[i]) / ((x[i] - x[i - 1]) / avh); // LelliD
				v[i] = dy[i - 1] * (h - g); // LelliD
				e += v[i] * v[i];
			}
			v[n] = dy[n - 1] * (-h); // LelliD
			e += v[n] * v[n];

			// calculate upper three bands of inverse matrix
			r[n] = 0; // LelliD
			r[n + r_dim1] = 0; // LelliD
			r[n + 1] = 0; // LelliD
			for (i = n - 1; i >= 2; --i)
			{
				g = r[i + r_dim1]; // LelliD
				h = r[i + r_dim1 * 2]; // LelliD
				r[i + r_dim1] = -g * r[i + 1] - h * r[i + 1 + r_dim1]; // LelliD
				r[i + r_dim1 * 2] = -g * r[i + 1 + r_dim1] - h * r[i + 2]; // LelliD
				r[i] = r[i] - g * r[i + r_dim1] - h * r[i + r_dim1 * 2]; // LelliD
			}

			// calculate trace
			f = g = h = zero;
			for (i = 2; i < n; ++i)
			{
				f += r[i] * c1[i - 1]; // LelliD
				g += r[i + r_dim1] * c2[i - 1]; // LelliD
				h += r[i + r_dim1 * 2] * c3[i - 1]; // LelliD
			}
			f += two * (g + h);

			// calculate statistics
			stat[0] = p; // LelliD
			stat[1] = f * p; // LelliD
			stat[2] = n * e / (f * f); // LelliD
			stat[3] = e * p * p / n; // LelliD
			stat[5] = e * p / f; // LelliD

			if (var >= zero)
			{
				d1 = stat[3] - two * var * stat[1] / n + var; // LelliD
				stat[4] = Math.Max(d1, zero); // LelliD
				fun = stat[4]; // LelliD
			}
			else
			{
				stat[4] = stat[5] - stat[3]; // LelliD
				fun = stat[2]; // LelliD
			}
		}

		#endregion

		#region sperr

		// calculates bayesian estimates of the standard errors of the fitted 
		// values of a cubic smoothing spline by calculating the diagonal elements
		// of the influence matrix. 
		static void sperr( // converted to zero based arrays by LelliD
			int n,
			double[] x,
			double avh,
			double[] dy,
			double[] r,
			double p,
			double var,
			double[] se)
		{
			int i, r_dim1;
			double f, g, h, f1, g1, h1, d1;



			r_dim1 = n + 2;

			// initialize
			h = avh / (x[1] - x[0]); // LelliD
			se[0] = one - p * dy[0] * dy[0] * h * h * r[2]; // LelliD
			r[1] = zero; // LelliD
			r[1 + r_dim1] = zero; // LelliD
			r[1 + r_dim1 * 2] = zero; // LelliD

			// calculate diagonal elements
			for (i = 2; i < n; ++i)
			{
				f = h;
				h = avh / (x[i] - x[i - 1]); // LelliD
				g = -f - h;
				f1 = f * r[i - 1] + g * r[i - 1 + r_dim1] + h * r[i - 1 + r_dim1 * 2]; // LelliD
				g1 = f * r[i - 1 + r_dim1] + g * r[i] + h * r[i + r_dim1]; // LelliD
				h1 = f * r[i - 1 + r_dim1 * 2] + g * r[i + r_dim1] + h * r[i + 1]; // LelliD
				se[i - 1] = one - p * dy[i - 1] * dy[i - 1] * (f * f1 + g * g1 + h * h1); // LelliD
			}
			se[n - 1] = one - p * dy[n - 1] * dy[n - 1] * h * h * r[n - 1]; // LelliD

			// calculate standard error estimates
			for (i = 0; i < n; ++i) // LelliD
			{
				d1 = se[i] * var;
				se[i] = Math.Sqrt((Math.Max(d1, 0))) * dy[i];
			}
		}

		#endregion


		#region spcof


		// calculates coefficients of a cubic smoothing spline from 
		// parameters calculated by subroutine spfit.
		static void spcof( // converted to zero based by LelliD
			int n,
			double[] x,
			double avh,
			double[] y,
			double[] dy,
			double p,
			double q,
			double[] a,
			double[] c1,
			double[] c2,
			double[] c3,
			double[] u,
			double[] v)
		{


			// calculate a
			double qh = q / (avh * avh);
			for (int i = 0; i < n; ++i) // LelliD
			{
				a[i] = y[i] - p * dy[i] * v[i + 1];
				u[i + 1] *= qh; // LelliD
			}

			// calculate c
			for (int i = 1; i < n; ++i)
			{
				double h = x[i] - x[i - 1]; // LelliD
				c3[i - 1] = (u[i + 1] - u[i]) / (h * 3.0); // LelliD
				c1[i - 1] = (a[i] - a[i - 1]) / h - (h * c3[i - 1] + u[i]) * h; // LelliD
				c2[i - 1] = u[i]; // LelliD
			}

			c1[n - 1] = c2[n - 1] = c3[n - 1] = 0.0; // LelliD
		}
		#endregion
		#endregion // low level functions

		//----------------------------------------------------------------------------//
		//
		// int MpCrossValidatedSpline (const Vector &X, 
		//                             const Vector &F, 
		//                             Vector &DF,
		//             Vector &Y, Vector &C1, Vector &C2, Vector &C3,
		//             double& var, Vector &SE, Vector &WK)
		//
		//  Arguments:
		//
		//              X   Vector of length n containing the abscissae of the
		//        n data points (x[i],f[i]).
		//        x must be ordered so that x[i] < x[i+1].
		//
		//    F   Vector of length n containing the ordinates
		//        of the n data points (x[i],f[i]).
		//
		//             DF   Vector df[i] is the relative standard
		//        deviation of the error associated with data point i.
		//                  Each df[i] must be positive. The values in df are
		//        scaled by the subroutine so that their mean square
		//        value is 1, and unscaled again on normal exit.
		//                  The mean square value of the df[i] is returned in
		//        wk[6] on normal exit.
		//                  If the absolute standard deviations are known,
		//                  these should be provided in df and the error
		//                  variance parameter var (see below) should then be
		//                  set to 1.
		//                  If the relative standard deviations are unknown,
		//                  set each df[i]=1.
		//
		//     Y,C1,C2,C3   Spline coefficient arrays of length n. (output)
		//        The value of the spline approximation at t is
		//
		//                    s(t) = ((c3[i]*d+c2[i])*d+c1[i])*d+y[i]
		//
		//                  where x[i] <= t < x[i+1] and d = t-x[i].
		//
		//        That means
		//       y[i]  contains the function value y(x[i])
		//       c1[i] contains the 1st derivative y'(x[i])
		//       c2[i] contains the 2nd derivative y''(x[i])
		//        of the smoothing spline.
		//
		//            var   Error variance. (input/output)
		//                  If var is negative (i.e. unknown) then
		//                  the smoothing parameter is determined
		//                  by minimizing the generalized cross validation
		//                  and an estimate of the error variance is returned in var.
		//                  If var is non-negative (i.e. known) then the
		//                  smoothing parameter is determined to minimize
		//                  an estimate, which depends on var, of the true
		//                  mean square error, and var is unchanged.
		//                  In particular, if var is zero, then an
		//                  interpolating natural cubic spline is calculated.
		//                  var should be set to 1 if absolute standard
		//                  deviations have been provided in df (see above).
		//
		//            SE    Vector se of length n returning Bayesian standard
		//                  error estimates of the fitted spline values in y.
		//                  If a NullVector is passed to the subroutine
		//        then no standard error estimates are computed.
		//
		//            WK    Work vector of length 7*(n+2)+1, arbitrary offset. 
		//                  On normal exit the first 7 values of wk are assigned 
		//                  as follows:
		//
		//                  ( here we arbitrarily start numbering from 0)
		//
		//                  wk[0] = smoothing parameter = rho/(rho+1)
		//                      If w[1]=0 (rho=0) an interpolating natural
		//          cubic spline has been calculated.
		//                          If wk[1]=1 (rho=infinite) a least squares
		//                           regression line has been calculated.
		//                  wk[1] = estimate of the number of degrees of
		//                          freedom of the residual sum of squares
		//                          which reduces to the usual value of n-2
		//                when a least squares regression line
		//                is calculated.
		//                  wk[2] = generalized cross validation
		//                  wk[3] = mean square residual
		//                  wk[4] = estimate of the true mean square error
		//                          at the data points
		//                  wk[5] = estimate of the error variance
		//                          wk[6] coincides with the output value of
		//          var if var is negative on input. It is
		//                calculated with the unscaled values of the
		//          df[i] to facilitate comparisons with a
		//          priori variance estimates.
		//                  wk[6] = mean square value of the df[i]
		//
		//                  wk[2],wk[3],wk[4] are calculated with the df[i]
		//                  scaled to have mean square value 1. The unscaled
		//        values of wk[2],wk[3],wk[4] may be calculated by
		//        dividing by wk[6].
		//
		//  Return value:
		//        = 0  if no errors occured.
		//                  = 1  if number of data points n is less than 3.
		//                  = 2  if input abscissae are not ordered x[i] < x[i+1].
		//                  = 3  if standard deviation df[i] not positive for some i.
		//

		public override int Interpolate(IROVector x, IROVector y)
		{
			// check input parameters

			if (!MatchingIndexRange(x, y))
				throw new ArgumentException("index range mismatch of vectors");

			// here we must use a copy of the original vectors




			// Empty data vectors - free auxilliary storage
			if (x.Length == 0)
			{
				xstore.Clear();
				ystore.Clear();
				y0.Clear();
				y1.Clear();
				y2.Clear();
				y3.Clear();
				se.Clear();
				wkr.Clear();
				wkt.Clear();
				wku.Clear();
				wkv.Clear();
				return 0;
			}

			xstore.CopyFrom(x);
			ystore.CopyFrom(y);

			// link original data vectors into base class
			base.x = xstore;
			base.y = ystore;

			int lo = x.LowerBound,
				hi = x.UpperBound,
				n = x.Length;


			// Resize the auxilliary vectors. Note, that there is no reallocation if the
			// vector already has the appropriate dimension.
			y0.Resize(lo, hi);
			y1.Resize(lo, hi);
			y2.Resize(lo, hi);
			y3.Resize(lo, hi);
			// se.Resize(lo,hi); // currently zero
			wkr.Resize(0, 3 * (n + 2));
			wkt.Resize(0, 2 * (n + 2));
			wku.Resize(0, 1 * (n + 2));
			wkv.Resize(0, 1 * (n + 2));
			if (calculateErrorEstimates)
				se.Resize(lo, hi);

			// set derivatives for a single point
			if (x.Length == 1)
			{
				y0[lo] = y[lo];
				y1[lo] = y2[lo] = y3[lo] = 0.0;
				return 0;
			}

			// set derivatives for a line
			if (x.Length == 2)
			{
				y0[lo] = y[lo];
				y0[hi] = y[hi];
				y1[lo] = y1[hi] = (y[hi] - y[lo]) / (x[hi] - x[lo]);
				y2[lo] = y2[hi] =
					y3[lo] = y3[hi] = 0.0;
				return 0;
			}

		

			// set standard deviation of the points to 1 if dy is not set or has
			// the wrong length
			if (dy.Store() == null || dy.Length != xstore.Length)
			{
				dy.Resize(lo, hi);
				for (int k = lo; k <= hi; ++k)
					dy[k] = 1;
			}

			// adjust pointers to vectors so that indexing starts from 1
			double[] xx = xstore.Store();
			double[] f = ystore.Store();

			double[] yy = y0.Store(); // coefficients calculated
			double[] c1 = y1.Store();
			double[] c2 = y2.Store();
			double[] c3 = y3.Store();
			double[] df = dy.Store();

			// index starts from 0
			double[] wwr = wkr.Store();
			double[] wwt = wkt.Store();
			double[] wwu = wku.Store();
			double[] wwv = wkv.Store();

			// set ss to (double*)0 if a NullVector is given
			double[] ss = null;
			if (se.Length > 0) ss = se.Store();



			return cubgcv(xx, f, df, n, yy, c1, c2, c3, ss, wwr, wwt, wwu, wwv);
		}


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
      return CubicSplineHorner1stDerivative(xx, x, y0, y1, y2, y3);
    }

		public override double GetYOfU(double u)
		{
			return CubicSplineHorner(u, x, y0, y1, y2, y3);
		}


		public void SetErrorVariance(IROVector dyy, double errvar)
		{
			dy.CopyFrom(dyy);
			var = errvar;
		}

		public double ErrorVariance
		{
			get
			{
				return var;
			}
			set
			{
				var = value;
			}
		}
			
	

	}


}
