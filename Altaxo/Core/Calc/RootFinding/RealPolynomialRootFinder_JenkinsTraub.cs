// Copyright Dr. Dirk Lellinger 2013. Adapted from Kenneth Haugland to the reqirements of Altaxo.
// Author: Translated to C# by Kenneth Haugland (Code project open license). See http://www.codeproject.com/Articles/552678/Polynomial-Equation-Solver
// from C++ code that was translated by Laurent Bartholdi for Real coefficients from the original Netlib site in FORTRAN
// and from C code written by Henrik Vestermark for complex coefficients translated from the original Netlib site in FORTRAN

using System;
using System.Collections.Generic;

namespace Altaxo.Calc.RootFinding
{
	/// <summary>
	/// Implements the Jenkins-Traub algorithm for polynoms with real coefficients.
	/// </summary>
	public class RealPolynomialRootFinder_JenkinsTraub
	{
		//Global variables that assist the computation, taken from the Visual Studio C++ compiler class float
		// smallest such that 1.0+DBL_EPSILON != 1.0
		private const double DBL_EPSILON = 2.22044604925031E-16;

		// max value
		private const double DBL_MAX = 1.79769313486232E+307;

		// min positive value
		private const double DBL_MIN = 2.2250738585072E-308;

		/// <summary>
		/// The Jenkins–Traub algorithm for finding the roots of a polynomial.
		/// </summary>
		/// <param name="Input">The coefficients for the polynomial starting with the constant (zero degree) and ends with the highest degree. Missing coefficients must be provided as zeros.</param>
		/// <returns>All the real and complex roots that are found are returned in a list of complex numbers. The list is not neccessarily sorted.</returns>
		public List<Complex> Execute(params double[] Input)
		{
			return FindRoots(Input);
		}

		/// <summary>
		/// The Jenkins–Traub algorithm for finding the roots of a polynomial.
		/// </summary>
		/// <param name="Input">The coefficients for the polynomial starting with the constant (zero degree) and ends with the highest degree. Missing coefficients must be provided as zeros.</param>
		/// <returns>All the real and complex roots that are found are returned in a list of complex numbers. The list is not neccessarily sorted.</returns>
		public static List<Complex> FindRoots(params double[] Input)
		{
			if (null == Input)
				throw new ArgumentNullException("Input");

			//Actual degree calculated from the items in the Input ParamArray
			int Degree;
			for (Degree = Input.Length - 1; Degree >= 0 && Input[Degree] == 0; --Degree) ;

			if (Degree <= 0)
				throw new ArgumentException("Provided polynomial has a degree of zero. Root finding is therefore not possible");

			List<Complex> result = new List<Complex>();

			int j = 0;
			int l = 0;
			int N = 0;
			int NM1 = 0;
			int NN = 0;
			int NZ = 0;
			int zerok = 0;

			double[] op = new double[Degree + 1];
			double[] K = new double[Degree + 1];
			double[] p = new double[Degree + 1];
			double[] pt = new double[Degree + 1];
			double[] qp = new double[Degree + 1];
			double[] temp = new double[Degree + 1];
			double[] zeror = new double[Degree + 1];
			double[] zeroi = new double[Degree + 1];
			double bnd = 0;
			double df = 0;
			double dx = 0;
			double ff = 0;
			double moduli_max = 0;
			double moduli_min = 0;
			double x = 0;
			double xm = 0;
			double aa = 0;
			double bb = 0;
			double cc = 0;
			double lzi = 0;
			double lzr = 0;
			double sr = 0;
			double szi = 0;
			double szr = 0;
			double t = 0;
			double u = 0;
			double xx = 0;
			double xxx = 0;
			double yy = 0;

			// These are used to scale the polynomial for more accuracy
			double factor = 0;
			double sc = 0;

			const double RADFAC = 0.017453292519943295769;  // Degrees-to-radians conversion factor = pi/180
			const double lb2 = 0.69314718055994530942; // Math.Log(2.0);
			const double lo = DBL_MIN / DBL_EPSILON;  //Double.MinValue / Double.Epsilon
			double cosr = Math.Cos(94.0 * RADFAC);  // = -0.069756474
			double sinr = Math.Sin(94.0 * RADFAC);  // = 0.99756405

			for (int i = 0, k = Degree; i <= Degree; ++i, --k)
			{
				var coeff = Input[k];
				if (!(coeff >= double.MinValue && coeff <= double.MaxValue))
					throw new ArgumentOutOfRangeException(string.Format("Input[{0}] is {1}. This value is not acceptable. Exiting root finding algorithm.", k, coeff));
				op[i] = coeff;
			}

			N = Degree;
			xx = Math.Sqrt(0.5);
			//= 0.70710678
			yy = -xx;

			// Remove zeros at the origin, if any
			j = 0;
			while ((op[N] == 0))
			{
				zeror[j] = 0;
				zeroi[j] = 0.0;
				N -= 1;
				j += 1;
			}

			NN = N + 1;

			for (int i = 0; i <= NN - 1; i++)
			{
				p[i] = op[i];
			}

			while (N >= 1)
			{
				//Start the algorithm for one zero
				if (N <= 2)
				{
					if (N < 2)
					{
						//1st degree polynomial
						zeror[(Degree) - 1] = -(p[1] / p[0]);
						zeroi[(Degree) - 1] = 0.0;
					}
					else
					{
						//2nd degree polynomial
						Quad_ak1(p[0], p[1], p[2], ref zeror[((Degree) - 2)], ref zeroi[((Degree) - 2)], ref zeror[((Degree) - 1)], ref zeroi[(Degree) - 1]);
					}
					//Solutions have been calculated, so exit the loop
					break; // TODO: might not be correct. Was : Exit While
				}

				moduli_max = 0.0;
				moduli_min = DBL_MAX;

				for (int i = 0; i <= NN - 1; i++)
				{
					x = Math.Abs(p[i]);
					if ((x > moduli_max))
						moduli_max = x;
					if (((x != 0) & (x < moduli_min)))
						moduli_min = x;
				}

				// Scale if there are large or very small coefficients
				// Computes a scale factor to multiply the coefficients of the polynomial. The scaling
				// is done to avoid overflow and to avoid undetected underflow interfering with the
				// convergence criterion.
				// The factor is a power of the base.

				//  Scaling the polynomial
				sc = lo / moduli_min;

				if ((((sc <= 1.0) & (moduli_max >= 10)) | ((sc > 1.0) & (DBL_MAX / sc >= moduli_max))))
				{
					if (sc == 0)
					{
						sc = DBL_MIN;
					}

					l = Convert.ToInt32(Math.Log(sc) / lb2 + 0.5);
					factor = Math.Pow(2.0, l);
					if ((factor != 1.0))
					{
						for (int i = 0; i <= NN; i++)
						{
							p[i] *= factor;
						}
					}
				}

				//Compute lower bound on moduli of zeros
				for (int i = 0; i <= NN - 1; i++)
				{
					pt[i] = Math.Abs(p[i]);
				}
				pt[N] = -(pt[N]);

				NM1 = N - 1;

				// Compute upper estimate of bound
				x = Math.Exp((Math.Log(-pt[N]) - Math.Log(pt[0])) / Convert.ToDouble(N));

				if ((pt[NM1] != 0))
				{
					// If Newton step at the origin is better, use it
					xm = -pt[N] / pt[NM1];
					if (xm < x)
					{
						x = xm;
					}
				}

				// Chop the interval (0, x) until ff <= 0
				xm = x;

				do
				{
					x = xm;
					xm = 0.1 * x;
					ff = pt[0];
					for (int i = 1; i <= NN - 1; i++)
					{
						ff = ff * xm + pt[i];
					}
				} while ((ff > 0));

				dx = x;

				do
				{
					df = pt[0];
					ff = pt[0];
					for (int i = 1; i <= N - 1; i++)
					{
						ff = x * ff + pt[i];
						df = x * df + ff;
					}
					ff = x * ff + pt[N];
					dx = ff / df;
					x -= dx;
				} while ((Math.Abs(dx / x) > 0.005));

				bnd = x;

				// Compute the derivative as the initial K polynomial and do 5 steps with no shift
				for (int i = 1; i <= N - 1; i++)
				{
					K[i] = Convert.ToDouble(N - i) * p[i] / (Convert.ToDouble(N));
				}
				K[0] = p[0];

				aa = p[N];
				bb = p[NM1];
				if ((K[NM1] == 0))
				{
					zerok = 1;
				}
				else
				{
					zerok = 0;
				}

				for (int jj = 0; jj <= 4; jj++)
				{
					cc = K[NM1];
					if ((zerok == 1))
					{
						// Use unscaled form of recurrence
						for (int i = 0; i <= NM1 - 1; i++)
						{
							j = NM1 - i;
							K[j] = K[j - 1];
						}
						K[0] = 0;
						if ((K[NM1] == 0))
						{
							zerok = 1;
						}
						else
						{
							zerok = 0;
						}
					}
					else
					{
						// Used scaled form of recurrence if value of K at 0 is nonzero
						t = -aa / cc;
						for (int i = 0; i <= NM1 - 1; i++)
						{
							j = NM1 - i;
							K[j] = t * K[j - 1] + p[j];
						}
						K[0] = p[0];
						if ((Math.Abs(K[NM1]) <= Math.Abs(bb) * DBL_EPSILON * 10.0))
						{
							zerok = 1;
						}
						else
						{
							zerok = 0;
						}
					}
				}

				// Save K for restarts with new shifts
				for (int i = 0; i <= N - 1; i++)
				{
					temp[i] = K[i];
				}

				for (int jj = 1; jj <= 20; jj++)
				{
					// Quadratic corresponds to a double shift to a non-real point and its
					// complex conjugate. The point has modulus BND and amplitude rotated
					// by 94 degrees from the previous shift.

					xxx = -(sinr * yy) + cosr * xx;
					yy = sinr * xx + cosr * yy;
					xx = xxx;
					sr = bnd * xx;
					u = -(2.0 * sr);

					// Second stage calculation, fixed quadratic
					Fxshfr_ak1(20 * jj, ref NZ, sr, bnd, K, N, p, NN, qp, u,
					ref lzi, ref lzr, ref szi, ref szr);

					if ((NZ != 0))
					{
						// The second stage jumps directly to one of the third stage iterations and
						// returns here if successful. Deflate the polynomial, store the zero or
						// zeros, and return to the main algorithm.

						j = (Degree) - N;
						zeror[j] = szr;
						zeroi[j] = szi;
						NN = NN - NZ;
						N = NN - 1;
						for (int i = 0; i <= NN - 1; i++)
						{
							p[i] = qp[i];
						}
						if ((NZ != 1))
						{
							zeror[j + 1] = lzr;
							zeroi[j + 1] = lzi;
						}

						//Found roots start all calulations again, with a lower order polynomial
						break; // TODO: might not be correct. Was : Exit For
					}
					else
					{
						// If the iteration is unsuccessful, another quadratic is chosen after restoring K
						for (int i = 0; i <= N - 1; i++)
						{
							K[i] = temp[i];
						}
					}
					if ((jj >= 20))
					{
						throw new Exception("Failure. No convergence after 20 shifts. Program terminated.");
					}
				}
			}

			for (int i = 0; i <= Degree - 1; i++)
			{
				result.Add(new Complex(zeror[Degree - 1 - i], zeroi[Degree - 1 - i]));
			}

			return result;
		}

		private static void Fxshfr_ak1(int L2, ref int NZ, double sr, double v, double[] K, int N, double[] p, int NN, double[] qp, double u,

		ref double lzi, ref double lzr, ref double szi, ref double szr)
		{
			// Computes up to L2 fixed shift K-polynomials, testing for convergence in the linear or
			// quadratic case. Initiates one of the variable shift iterations and returns with the
			// number of zeros found.

			// L2 limit of fixed shift steps
			// NZ number of zeros found

			//int fflag = 0;
			int i = 0;
			int iFlag = 0;
			int j = 0;
			int spass = 0;
			int stry = 0;
			int tFlag = 0;
			int vpass = 0;
			int vtry = 0;
			iFlag = 1;
			double a = 0;
			double a1 = 0;
			double a3 = 0;
			double a7 = 0;
			double b = 0;
			double betas = 0;
			double betav = 0;
			double c = 0;
			double d = 0;
			double e = 0;
			double f = 0;
			double g = 0;
			double h = 0;
			double oss = 0;
			double ots = 0;
			double otv = 0;
			double ovv = 0;
			double s = 0;
			double ss = 0;
			double ts = 0;
			double tss = 0;
			double tv = 0;
			double tvv = 0;
			double ui = 0;
			double vi = 0;
			double vv = 0;
			double[] qk = new double[100 + 2];
			double[] svk = new double[100 + 2];

			NZ = 0;
			betav = 0.25;
			betas = 0.25;
			oss = sr;
			ovv = v;

			// Evaluate polynomial by synthetic division
			QuadSD_ak1(NN, u, v, p, qp, ref a, ref b);

			tFlag = calcSC_ak1(N, a, b, ref a1, ref a3, ref a7, ref c, ref d, ref e, ref f,
			ref g, ref h, K, u, v, qk);

			for (j = 0; j <= L2 - 1; j++)
			{
				//fflag = 1;
				// Calculate next K polynomial and estimate v
				nextK_ak1(N, tFlag, a, b, a1, ref a3, ref a7, K, qk, qp);
				tFlag = calcSC_ak1(N, a, b, ref a1, ref a3, ref a7, ref c, ref d, ref e, ref f,
				ref g, ref h, K, u, v, qk);
				newest_ak1(tFlag, ref ui, ref vi, a, a1, a3, a7, b, c, d,
				f, g, h, u, v, K, N, p);

				vv = vi;

				// Estimate s
				if (K[N - 1] != 0)
				{
					ss = -(p[N] / K[N - 1]);
				}
				else
				{
					ss = 0;
				}

				ts = 1;
				tv = 1.0;

				if (((j != 0) & (tFlag != 3)))
				{
					// Compute relative measures of convergence of s and v sequences
					if (vv != 0)
					{
						tv = Math.Abs((vv - ovv) / vv);
					}

					if (ss != 0)
					{
						ts = Math.Abs((ss - oss) / ss);
					}

					// If decreasing, multiply the two most recent convergence measures

					if (tv < otv)
					{
						tvv = tv * otv;
					}
					else
					{
						tvv = 1;
					}

					if (ts < ots)
					{
						tss = ts * ots;
					}
					else
					{
						tss = 1;
					}

					// Compare with convergence criteria

					if (tvv < betav)
					{
						vpass = 1;
					}
					else
					{
						vpass = 0;
					}

					if (tss < betas)
					{
						spass = 1;
					}
					else
					{
						spass = 0;
					}

					if (((spass == 1) | (vpass == 1)))
					{
						// At least one sequence has passed the convergence test.
						// Store variables before iterating

						for (i = 0; i <= N - 1; i++)
						{
							svk[i] = K[i];
						}

						s = ss;

						// Choose iteration according to the fastest converging sequence
						stry = 0;
						vtry = 0;

						do
						{
							if ((spass == 1) & (!(vpass == 1) | (tss < tvv)))
							{
								// Do nothing. Provides a quick "short circuit".
							}
							else
							{
								QuadIT_ak1(N, ref NZ, ui, vi, ref szr, ref szi, ref lzr, ref lzi, qp, NN,
								ref a, ref b, p, qk, ref a1, ref a3, ref a7, ref c, ref d, ref e,
								ref f, ref g, ref h, K);

								if (((NZ) > 0))
									return;

								// Quadratic iteration has failed. Flag that it has been tried and decrease the
								// convergence criterion

								iFlag = 1;
								vtry = 1;
								betav *= 0.25;

								// Try linear iteration if it has not been tried and the s sequence is converging
								if ((stry == 1 | (!(spass == 1))))
								{
									iFlag = 0;
								}
								else
								{
									for (i = 0; i <= N - 1; i++)
									{
										K[i] = svk[i];
									}
								}
							}

							if ((iFlag != 0))
							{
								RealIT_ak1(ref iFlag, ref NZ, ref s, N, p, NN, qp, ref szr, ref szi, K,
								qk);

								if (((NZ) > 0))
									return;

								// Linear iteration has failed. Flag that it has been tried and decrease the
								// convergence criterion
								stry = 1;
								betas *= 0.25;

								if ((iFlag != 0))
								{
									// If linear iteration signals an almost double real zero, attempt quadratic iteration
									ui = -(s + s);
									vi = s * s;
								}
							}

							// Restore variables
							for (i = 0; i <= N - 1; i++)
							{
								K[i] = svk[i];
							}

							// Try quadratic iteration if it has not been tried and the v sequence is converging
							if ((!(vpass == 1) | vtry == 1))
							{
								// Break out of infinite for loop
								break; // TODO: might not be correct. Was : Exit Do
							}
						} while (true);

						// Re-compute qp and scalar values to continue the second stage
						QuadSD_ak1(NN, u, v, p, qp, ref a, ref b);
						tFlag = calcSC_ak1(N, a, b, ref a1, ref a3, ref a7, ref c, ref d, ref e, ref f,
						ref g, ref h, K, u, v, qk);
					}
				}

				ovv = vv;
				oss = ss;
				otv = tv;
				ots = ts;
			}
		}

		private static void QuadSD_ak1(int NN, double u, double v, double[] p, double[] q, ref double a, ref double b)
		{
			// Divides p by the quadratic 1, u, v placing the quotient in q and the remainder in a, b

			int i = 0;

			b = p[0];
			q[0] = p[0];

			a = -((b) * u) + p[1];
			q[1] = -((b) * u) + p[1];

			for (i = 2; i <= NN - 1; i++)
			{
				q[i] = -((a) * u + (b) * v) + p[i];
				b = (a);
				a = q[i];
			}
		}

		private static int calcSC_ak1(int N, double a, double b, ref double a1, ref double a3, ref double a7, ref double c, ref double d, ref double e, ref double f,
		ref double g, ref double h, double[] K, double u, double v, double[] qk)
		{
			// This routine calculates scalar quantities used to compute the next K polynomial and
			// new estimates of the quadratic coefficients.

			// calcSC - integer variable set here indicating how the calculations are normalized
			// to avoid overflow.

			int dumFlag = 3;
			// TYPE = 3 indicates the quadratic is almost a factor of K

			// Synthetic division of K by the quadratic 1, u, v
			QuadSD_ak1(N, u, v, K, qk, ref c, ref d);

			if ((Math.Abs((c)) <= (100.0 * DBL_EPSILON * Math.Abs(K[N - 1]))))
			{
				if ((Math.Abs((d)) <= (100.0 * DBL_EPSILON * Math.Abs(K[N - 2]))))
				{
					return dumFlag;
				}
			}

			h = v * b;
			if ((Math.Abs((d)) >= Math.Abs((c))))
			{
				dumFlag = 2;
				// TYPE = 2 indicates that all formulas are divided by d
				e = a / (d);
				f = (c) / (d);
				g = u * b;
				a3 = (e) * ((g) + a) + (h) * (b / (d));
				a1 = -a + (f) * b;
				a7 = (h) + ((f) + u) * a;
			}
			else
			{
				dumFlag = 1;
				// TYPE = 1 indicates that all formulas are divided by c
				e = a / (c);
				f = (d) / (c);
				g = (e) * u;
				a3 = (e) * a + ((g) + (h) / (c)) * b;
				a1 = -(a * ((d) / (c))) + b;
				a7 = (g) * (d) + (h) * (f) + a;
			}

			return dumFlag;
		}

		private static void nextK_ak1(int N, int tFlag, double a, double b, double a1, ref double a3, ref double a7, double[] K, double[] qk, double[] qp)
		{
			// Computes the next K polynomials using the scalars computed in calcSC_ak1

			int i = 0;
			double temp = 0;

			// Use unscaled form of the recurrence
			if ((tFlag == 3))
			{
				K[1] = 0;
				K[0] = 0.0;

				for (i = 2; i <= N - 1; i++)
				{
					K[i] = qk[i - 2];
				}

				return;
			}

			if (tFlag == 1)
			{
				temp = b;
			}
			else
			{
				temp = a;
			}

			if ((Math.Abs(a1) > (10.0 * DBL_EPSILON * Math.Abs(temp))))
			{
				// Use scaled form of the recurrence

				a7 = a7 / a1;
				a3 = a3 / a1;
				K[0] = qp[0];
				K[1] = -((a7) * qp[0]) + qp[1];

				for (i = 2; i <= N - 1; i++)
				{
					K[i] = -((a7) * qp[i - 1]) + (a3) * qk[i - 2] + qp[i];
				}
			}
			else
			{
				// If a1 is nearly zero, then use a special form of the recurrence

				K[0] = 0.0;
				K[1] = -(a7) * qp[0];

				for (i = 2; i <= N - 1; i++)
				{
					K[i] = -((a7) * qp[i - 1]) + (a3) * qk[i - 2];
				}
			}
		}

		private static void newest_ak1(int tFlag, ref double uu, ref double vv, double a, double a1, double a3, double a7, double b, double c, double d,
		double f, double g, double h, double u, double v, double[] K, int N, double[] p)
		{
			// Compute new estimates of the quadratic coefficients using the scalars computed in calcSC_ak1

			double a4 = 0;
			double a5 = 0;
			double b1 = 0;
			double b2 = 0;
			double c1 = 0;
			double c2 = 0;
			double c3 = 0;
			double c4 = 0;
			double temp = 0;

			vv = 0;
			//The quadratic is zeroed
			uu = 0.0;
			//The quadratic is zeroed

			if ((tFlag != 3))
			{
				if ((tFlag != 2))
				{
					a4 = a + u * b + h * f;
					a5 = c + (u + v * f) * d;
				}
				else
				{
					a4 = (a + g) * f + h;
					a5 = (f + u) * c + v * d;
				}

				// Evaluate new quadratic coefficients
				b1 = -K[N - 1] / p[N];
				b2 = -(K[N - 2] + b1 * p[N - 1]) / p[N];
				c1 = v * b2 * a1;
				c2 = b1 * a7;
				c3 = b1 * b1 * a3;
				c4 = -(c2 + c3) + c1;
				temp = -c4 + a5 + b1 * a4;
				if ((temp != 0.0))
				{
					uu = -((u * (c3 + c2) + v * (b1 * a1 + b2 * a7)) / temp) + u;
					vv = v * (1.0 + c4 / temp);
				}
			}
		}

		private static void QuadIT_ak1(int N, ref int NZ, double uu, double vv, ref double szr, ref double szi, ref double lzr, ref double lzi, double[] qp, int NN,
		ref double a, ref double b, double[] p, double[] qk, ref double a1, ref double a3, ref double a7, ref double c, ref double d, ref double e,

		ref double f, ref double g, ref double h, double[] K)
		{
			// Variable-shift K-polynomial iteration for a quadratic factor converges only if the
			// zeros are equimodular or nearly so.

			int i = 0;
			int j = 0;
			int tFlag = 0;
			int triedFlag = 0;
			j = 0;
			triedFlag = 0;

			double ee = 0;
			double mp = 0;
			double omp = 0;
			double relstp = 0;
			double t = 0;
			double u = 0;
			double ui = 0;
			double v = 0;
			double vi = 0;
			double zm = 0;

			NZ = 0;
			//Number of zeros found
			u = uu;
			//uu and vv are coefficients of the starting quadratic
			v = vv;

			do
			{
				Quad_ak1(1.0, u, v, ref szr, ref szi, ref lzr, ref lzi);

				// Return if roots of the quadratic are real and not close to multiple or nearly
				// equal and of opposite sign.
				if ((Math.Abs(Math.Abs(szr) - Math.Abs(lzr)) > 0.01 * Math.Abs(lzr)))
				{
					break; // TODO: might not be correct. Was : Exit Do
				}

				// Evaluate polynomial by quadratic synthetic division
				QuadSD_ak1(NN, u, v, p, qp, ref a, ref b);

				mp = Math.Abs(-((szr) * (b)) + (a)) + Math.Abs((szi) * (b));

				// Compute a rigorous bound on the rounding error in evaluating p
				zm = Math.Sqrt(Math.Abs(v));
				ee = 2.0 * Math.Abs(qp[0]);
				t = -((szr) * (b));

				for (i = 1; i <= N - 1; i++)
				{
					ee = ee * zm + Math.Abs(qp[i]);
				}

				ee = ee * zm + Math.Abs((a) + t);
				ee = (9.0 * ee + 2.0 * Math.Abs(t) - 7.0 * (Math.Abs((a) + t) + zm * Math.Abs((b)))) * DBL_EPSILON;

				// Iteration has converged sufficiently if the polynomial value is less than 20 times this bound
				if ((mp <= 20.0 * ee))
				{
					NZ = 2;
					break; // TODO: might not be correct. Was : Exit Do
				}

				j += 1;

				// Stop iteration after 20 steps
				if ((j > 20))
					break; // TODO: might not be correct. Was : Exit Do

				if ((j >= 2))
				{
					if (((relstp <= 0.01) & (mp >= omp) & (!(triedFlag == 1))))
					{
						// A cluster appears to be stalling the convergence. Five fixed shift
						// steps are taken with a u, v close to the cluster.
						if (relstp < DBL_EPSILON)
						{
							relstp = Math.Sqrt(DBL_EPSILON);
						}
						else
						{
							relstp = Math.Sqrt(relstp);
						}

						u -= u * relstp;
						v += v * relstp;

						QuadSD_ak1(NN, u, v, p, qp, ref a, ref b);

						for (i = 0; i <= 4; i++)
						{
							tFlag = calcSC_ak1(N, a, b, ref a1, ref a3, ref a7, ref c, ref d, ref e, ref f,
							ref g, ref h, K, u, v, qk);
							nextK_ak1(N, tFlag, a, b, a1, ref a3, ref a7, K, qk, qp);
						}

						triedFlag = 1;
						j = 0;
					}
				}

				omp = mp;

				// Calculate next K polynomial and new u and v
				tFlag = calcSC_ak1(N, a, b, ref a1, ref a3, ref a7, ref c, ref d, ref e, ref f,
				ref g, ref h, K, u, v, qk);
				nextK_ak1(N, tFlag, a, b, a1, ref a3, ref a7, K, qk, qp);
				tFlag = calcSC_ak1(N, a, b, ref a1, ref a3, ref a7, ref c, ref d, ref e, ref f,
				ref g, ref h, K, u, v, qk);
				newest_ak1(tFlag, ref ui, ref vi, a, a1, a3, a7, b, c, d,
				f, g, h, u, v, K, N, p);

				// If vi is zero, the iteration is not converging
				if ((vi != 0))
				{
					relstp = Math.Abs((-v + vi) / vi);
					u = ui;
					v = vi;
				}
			} while ((vi != 0));
		}

		private static void RealIT_ak1(ref int iFlag, ref int NZ, ref double sss, int N, double[] p, int NN, double[] qp, ref double szr, ref double szi, double[] K,

		double[] qk)
		{
			// Variable-shift H-polynomial iteration for a real zero

			// sss - starting iterate
			// NZ - number of zeros found
			// iFlag - flag to indicate a pair of zeros near real axis
			int i = 0;
			int j = 0;
			int nm1 = 0;
			j = 0;
			nm1 = N - 1;
			double ee = 0;
			double kv = 0;
			double mp = 0;
			double ms = 0;
			double omp = 0;
			double pv = 0;
			double s = 0;
			double t = 0;

			iFlag = 0;
			NZ = 0;
			s = sss;

			do
			{
				pv = p[0];

				// Evaluate p at s
				qp[0] = pv;
				for (i = 1; i <= NN - 1; i++)
				{
					qp[i] = pv * s + p[i];
					pv = pv * s + p[i];
				}
				mp = Math.Abs(pv);

				// Compute a rigorous bound on the error in evaluating p
				ms = Math.Abs(s);
				ee = 0.5 * Math.Abs(qp[0]);
				for (i = 1; i <= NN - 1; i++)
				{
					ee = ee * ms + Math.Abs(qp[i]);
				}

				// Iteration has converged sufficiently if the polynomial value is less than
				// 20 times this bound
				if ((mp <= 20.0 * DBL_EPSILON * (2.0 * ee - mp)))
				{
					NZ = 1;
					szr = s;
					szi = 0.0;
					break; // TODO: might not be correct. Was : Exit Do
				}

				j += 1;

				// Stop iteration after 10 steps
				if ((j > 10))
					break; // TODO: might not be correct. Was : Exit Do

				if ((j >= 2))
				{
					if (((Math.Abs(t) <= 0.001 * Math.Abs(-t + s)) & (mp > omp)))
					{
						// A cluster of zeros near the real axis has been encountered                    ' Return with iFlag set to initiate a quadratic iteration

						iFlag = 1;
						sss = s;
						break; // TODO: might not be correct. Was : Exit Do
					}
				}

				// Return if the polynomial value has increased significantly
				omp = mp;

				// Compute t, the next polynomial and the new iterate
				qk[0] = K[0];
				kv = K[0];
				for (i = 1; i <= N - 1; i++)
				{
					kv = kv * s + K[i];
					qk[i] = kv;
				}
				if ((Math.Abs(kv) > Math.Abs(K[nm1]) * 10.0 * DBL_EPSILON))
				{
					// Use the scaled form of the recurrence if the value of K at s is non-zero
					t = -(pv / kv);
					K[0] = qp[0];
					for (i = 1; i <= N - 1; i++)
					{
						K[i] = t * qk[i - 1] + qp[i];
					}
				}
				else
				{
					// Use unscaled form
					K[0] = 0.0;
					for (i = 1; i <= N - 1; i++)
					{
						K[i] = qk[i - 1];
					}
				}

				kv = K[0];
				for (i = 1; i <= N - 1; i++)
				{
					kv = kv * s + K[i];
				}

				if ((Math.Abs(kv) > (Math.Abs(K[nm1]) * 10.0 * DBL_EPSILON)))
				{
					t = -(pv / kv);
				}
				else
				{
					t = 0.0;
				}

				s += t;
			} while (true);
		}

		private static void Quad_ak1(double a, double b1, double c, ref double sr, ref double si, ref double lr, ref double li)
		{
			// Calculates the zeros of the quadratic a*Z^2 + b1*Z + c
			// The quadratic formula, modified to avoid overflow, is used to find the larger zero if the
			// zeros are real and both zeros are complex. The smaller real zero is found directly from
			// the product of the zeros c/a.

			double b = 0;
			double d = 0;
			double e = 0;

			sr = 0;
			si = 0;
			lr = 0;
			li = 0.0;

			if (a == 0)
			{
				if (b1 == 0)
				{
					sr = -c / b1;
				}
			}

			if (c == 0)
			{
				lr = -b1 / a;
			}

			//Compute discriminant avoiding overflow
			b = b1 / 2.0;

			if (Math.Abs(b) < Math.Abs(c))
			{
				if (c >= 0)
				{
					e = a;
				}
				else
				{
					e = -a;
				}

				e = -e + b * (b / Math.Abs(c));
				d = Math.Sqrt(Math.Abs(e)) * Math.Sqrt(Math.Abs(c));
			}
			else
			{
				e = -((a / b) * (c / b)) + 1.0;
				d = Math.Sqrt(Math.Abs(e)) * (Math.Abs(b));
			}

			if ((e >= 0))
			{
				// Real zero
				if (b >= 0)
				{
					d *= -1;
				}
				lr = (-b + d) / a;

				if (lr != 0)
				{
					sr = (c / (lr)) / a;
				}
			}
			else
			{
				// Complex conjugate zeros
				lr = -(b / a);
				sr = -(b / a);
				si = Math.Abs(d / a);
				li = -(si);
			}
		}
	}
}
