
// Original MatPack-1.7.3\Source\d9gmit.cc
//                               d9lgmc.cc
//                               dlgams.cc
//                               dlngam.cc
//                               cgamma.cc
//                               dgamma.cc
//                               dgamlm.cc
//																dfac.cc
//                               dgamr.cc
//                               dgamic.cc
//                               dgamit.cc
//                               d9lgic.cc
//                               d9lgit.cc
//                               d9gmic.cc
//                               dlbeta.cc
//                               dbetai.cc
//                               dlnrel.cc
//                               cgamma.cc
//                               clngam.cc
//                               clnrel.cc
//                               c9lgmc.cc

using System;


namespace Altaxo.Calc
{
	/// <summary>
	/// Contains the gamma function and functions related to this function.
	/// </summary>
	public class GammaRelated
	{
		#region Common Constants

		/// <summary>
		/// Represents the smallest number where 1+DBL_EPSILON is not equal to 1.
		/// </summary>
		public const double DBL_EPSILON = 2.2204460492503131e-016;
		/// <summary>
		/// The smallest positive number that can be represented by <see cref="System.Double"/>.
		/// </summary>
		public const double DBL_MIN     = double.Epsilon;
		/// <summary>
		/// The biggest positive number that can be represented by <see cref="System.Double"/>.
		/// </summary>
		public const double DBL_MAX     = double.MaxValue;

		#endregion

		#region Helper functions
		// Square
		private static double sqr(double x)
		{
			return x*x;
		}

		// round towards zero (Fortran convention)
		private static double	Dint(double d) 
		{
			return (d<0) ? Math.Ceiling(d) : Math.Floor(d);
		}

		// modulus (Fortran convention)
		private static double Dmod(double x, double y) 
		{ 
			double quotient;
			if( (quotient = x / y) >= 0)
				quotient = Math.Floor(quotient);
			else
				quotient = -Math.Floor(-quotient);
			return (x - y*quotient);
		}


		/// <summary>
		/// Return first number with sign of second number
		/// </summary>
		/// <param name="x">The first number.</param>
		/// <param name="y">The second number whose sign is used.</param>
		/// <returns>The first number x with the sign of the second argument y.</returns>
		private static double CopySign (double x, double y)
		{
			return (y < 0) ? ((x < 0) ? x : -x) : ((x > 0) ? x : -x);
		}

		#endregion

		#region d9gmit

		/// <summary>
		/// Compute Tricomi's incomplete Gamma function for small arguments.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="x"></param>
		/// <param name="algap1"></param>
		/// <param name="sgngam"></param>
		/// <returns></returns>
		/// <remarks>
		/// This is a translation from the Fortran version of D9LGIT, SLATEC, FNLIB,
		/// CATEGORY C7E, REVISION 900720, originally written by Fullerton W.,(LANL)
		/// to C++.
		/// </remarks>
		static double d9gmit (double a, double x, double algap1, double sgngam)
		{
			// machine constants
			const double eps = 0.25 * DoubleConstants.DBL_EPSILON;
			double bot = Math.Log(DoubleConstants.DBL_MIN);

			if (x <= 0.0) 
				throw new ArgumentException("x must be > 0");

			double algs, sgng2, alg2, ret_val;
			int ma = (a < 0.0) ? (int)(a - 0.5) : (int)(a + 0.5);
			double aeps = a - ma;
			double ae = a;
			if (a < -0.5) ae = aeps;

			double t  = 1.0,
				te = ae,
				s  = t;
			for (int k = 1; k <= 200; ++k) 
			{
				double fk = (double) k;
				te = -x * te / fk;
				t = te / (ae + fk);
				s += t;
				if (Math.Abs(t) < eps * Math.Abs(s)) goto L30;
			}

			throw new ArgumentException("no convergence in 200 terms of Taylor series");

			L30:
				if (a >= -0.5) 
				{
					algs = -(algap1) + Math.Log(s);
					return Math.Exp(algs);
				}

			algs = -LnGamma(aeps + 1.0) + Math.Log(s);
			s = 1.0;
			int m = -ma - 1;
			if (m == 0) goto L50;

			t = 1.0;
			for (int k = 1; k <= m; ++k) 
			{
				t = x * t / (aeps - (m + 1 - k));
				s += t;
				if (Math.Abs(t) < eps * Math.Abs(s)) goto L50;
			}

			L50:
				ret_val = 0.0;
			algs = -ma * Math.Log(x) + algs;
			if (s == 0.0 || aeps == 0.0) return Math.Exp(algs);

			sgng2 = sgngam * Math.Sign(s);
			alg2 = -x - algap1 + Math.Log((Math.Abs(s)));

			if (alg2 > bot) 
				ret_val = sgng2 * Math.Exp(alg2);
  
			if (algs > bot)
				ret_val += Math.Exp(algs);

			return ret_val;
		}

		#endregion

		#region d9lgmc

		static readonly double[] algmcs = 
	{ 
		0.1666389480451863247205729650822,
		-1.384948176067563840732986059135e-5,
		9.810825646924729426157171547487e-9,
		-1.809129475572494194263306266719e-11,
		6.221098041892605227126015543416e-14,
		-3.399615005417721944303330599666e-16,
		2.683181998482698748957538846666e-18,
		-2.868042435334643284144622399999e-20,
		3.962837061046434803679306666666e-22,
		-6.831888753985766870111999999999e-24,
		1.429227355942498147573333333333e-25,
		-3.547598158101070547199999999999e-27,
		1.025680058010470912e-28,
		-3.401102254316748799999999999999e-30,
		1.276642195630062933333333333333e-31 
	};

		static int nalgm;
		static bool d9lgmc_first = true;


		/// <summary>
		/// Compute the log gamma correction factor for x >= 10.0 so that 
		/// log (dgamma(x)) = log(sqrt(2*pi)) + (x-0.5)*log(x) - x + d9lgmc(x) 
		/// </summary>
		/// <param name="x">The function argument x.</param>
		/// <returns>The log gamma correction factor for x>=10.0.</returns>
		/// <remarks>
		/// This is a translation from the Fortran version of SLATEC, FNLIB,
		/// CATEGORY C7E, REVISION 900720, originally written by Fullerton W.,(LANL)
		/// to C++.
		///
		/// Series for ALGM       on the interval  0.          to  1.00000E-02 
		///                                        with weighted error   1.28E-31 
		///
		///                                         log weighted error  30.89 
		///                               significant figures required  29.81 
		///                                    decimal places required  31.48 
		/// </remarks>
		static double d9lgmc(double x)
		{

			double xbig = 1.0 / Math.Sqrt(0.5*DBL_EPSILON);
			double xmax = Math.Exp( Math.Min(Math.Log(DBL_MAX / 12.0), -Math.Log(DBL_MIN * 12.0)) );

			double ret_val;

			if (d9lgmc_first) 
			{
				nalgm = Series.initds(algmcs, 15, 0.5*DBL_EPSILON);
				d9lgmc_first = false;
			}

			if (x < 10.0) 
				throw new ArgumentException("x must be >= 10"); 

			if (x >= xmax) goto L20;

			ret_val = 1.0 / (x * 12.0);
			if (x < xbig) 
				ret_val = Series.dcsevl(sqr(10.0 / x) * 2.0 - 1.0, algmcs, nalgm) / x;
			return ret_val;
    
			L20:
				ret_val = 0.0;	
			throw new ArgumentException("x so big d9lgmc(x) underflows");
		}

		#endregion

		#region d9lgic

		/// <summary>
		/// Compute the log complementary incomplete Gamma function
		/// for large x and for a&lt;=x.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="x"></param>
		/// <param name="alx"></param>
		/// <returns></returns>
		/// <remarks>
		/// This is a translation from the Fortran version of D9LGIC, SLATEC, FNLIB,
		/// CATEGORY C7E, REVISION 900720, originally written by Fullerton W.,(LANL)
		/// to C++.
		/// </remarks>
		static double d9lgic (double a, double x, double alx)
		{
			// machine constants
			const double eps = 0.25 * DBL_EPSILON;

			double xpa = x + 1.0 - a,
				xma = x - 1.0 - a,
				r   = 0.0,
				p   = 1.0,
				s   = p;
			for (int k = 1; k <= 300; ++k) 
			{
				double fk = (double) k,
					t  =  fk * (a - fk) * (r + 1.0);
				r = -t / ((xma + fk * 2.0) * (xpa + fk * 2.0) + t);
				p *= r;
				s += p;
				if (Math.Abs(p) < eps * s) goto result;
			}

			throw new ArgumentException("no convergence in 300 terms of continued fraction");
  
			result:
   
				return ( a * alx - x + Math.Log(s / xpa) );
		}


		#endregion

		#region d9lgit

		static readonly double sqeps_d9lgit = Math.Sqrt(DBL_EPSILON);
		const double           eps_d9lgit   = 0.25 * DBL_EPSILON;

		/// <summary>
		/// Compute the logarithm of Tricomi's incomplete Gamma function 
		/// with Perron's continued fraction for large x and a >= x.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="x"></param>
		/// <param name="algap1"></param>
		/// <returns></returns>
		/// <remarks>
		/// This is a translation from the Fortran version of D9LGIT, SLATEC, FNLIB,
		/// CATEGORY C7E, REVISION 900720, originally written by Fullerton W.,(LANL)
		/// to C++.
		/// </remarks>
		static double d9lgit (double a, double x, double algap1)
		{
			// machine constants

			if (x <= 0.0 || a < x) 
				throw new ArgumentException("x must be > 0.0 and <= a");

			double ax  = a + x,
				a1x = ax + 1.0,
				r = 0.0,
				p = 1.0,
				s = p;
			for (int k = 1; k <= 200; ++k) 
			{
				double fk = (double) k,
					t  =  (a + fk) * x * (r + 1.0);
				r = t / ((ax + fk) * (a1x + fk) - t);
				p *= r;
				s += p;
				if (Math.Abs(p) < eps_d9lgit * s) goto result;
			}

			throw new ArgumentException("no convergence in 200 terms of continued fraction");

			result:
  
				double hstar = 1.0 - x * s / a1x;
			if (hstar < sqeps_d9lgit) 
			{
				System.Diagnostics.Trace.WriteLine("Warning (d9lgit function): answer less than half precision");
			}

			return ( -x - algap1 - Math.Log(hstar) );
		} 


		#endregion

		#region LogRel

		static readonly double[] alnrcs_LogRel = 
	{
		0.10378693562743769800686267719098e+1,
		-0.13364301504908918098766041553133e+0,
		0.19408249135520563357926199374750e-1,
		-0.30107551127535777690376537776592e-2,
		0.48694614797154850090456366509137e-3,
		-0.81054881893175356066809943008622e-4,
		0.13778847799559524782938251496059e-4,
		-0.23802210894358970251369992914935e-5,
		0.41640416213865183476391859901989e-6,
		-0.73595828378075994984266837031998e-7,
		0.13117611876241674949152294345011e-7,
		-0.23546709317742425136696092330175e-8,
		0.42522773276034997775638052962567e-9,
		-0.77190894134840796826108107493300e-10,
		0.14075746481359069909215356472191e-10,
		-0.25769072058024680627537078627584e-11,
		0.47342406666294421849154395005938e-12,
		-0.87249012674742641745301263292675e-13,
		0.16124614902740551465739833119115e-13,
		-0.29875652015665773006710792416815e-14,
		0.55480701209082887983041321697279e-15,
		-0.10324619158271569595141333961932e-15,
		0.19250239203049851177878503244868e-16,
		-0.35955073465265150011189707844266e-17,
		0.67264542537876857892194574226773e-18,
		-0.12602624168735219252082425637546e-18,
		0.23644884408606210044916158955519e-19,
		-0.44419377050807936898878389179733e-20,
		0.83546594464034259016241293994666e-21,
		-0.15731559416479562574899253521066e-21,
		0.29653128740247422686154369706666e-22,
		-0.55949583481815947292156013226666e-23,
		0.10566354268835681048187284138666e-23,
		-0.19972483680670204548314999466666e-24,
		0.37782977818839361421049855999999e-25,
		-0.71531586889081740345038165333333e-26,
		0.13552488463674213646502024533333e-26,
		-0.25694673048487567430079829333333e-27,
		0.48747756066216949076459519999999e-28,
		-0.92542112530849715321132373333333e-29,
		0.17578597841760239233269760000000e-29,
		-0.33410026677731010351377066666666e-30,
		0.63533936180236187354180266666666e-31
	};

		static int nlnrel_LogRel = 0;
		static double xmin_LogRel = 0.0;

		/// <summary>
		/// LogRel(z) = log(1+z) with relative error accuracy near z = 0.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		/// <remarks>
		/// June 1977 edition.   W. Fullerton, c3, Los Alamos Scientific Lab.
		///
		/// series for alnr       on the interval -3.75000e-01 to  3.75000e-01
		///                                        with weighted error   6.35e-32
		///                                         log weighted error  31.20
		///                               significant figures required  30.93
		///                                    decimal places required  32.01
		/// </remarks>
		static double LogRel (double x)
		{

			if (nlnrel_LogRel == 0) 
			{
				nlnrel_LogRel = Series.initds(alnrcs_LogRel, 43, 0.1*0.5 * DBL_EPSILON);
				xmin_LogRel = -1.0 + Math.Sqrt(DBL_EPSILON);
			}
  
			if (x <= -1.0) 
				throw new ArgumentException("x <= -1");

			if (x < xmin_LogRel) 
			{
				System.Diagnostics.Trace.WriteLine("Warning (LogRel function): answer less than half precision because x too near -1");
			}

			if (Math.Abs(x) <= 0.375) 
				return x * (1.0 - x * Series.dcsevl(x/0.375,alnrcs_LogRel,nlnrel_LogRel));
			else 
				return Math.Log(1.0 + x);
		}



		#endregion

		#region d9gmic

		const double euler_d9gmic = 0.5772156649015328606065120900824;
		static readonly double bot_d9gmic = Math.Log(DBL_MIN); 
		const double eps_d9gmic = 0.25 * DBL_EPSILON;

		/// <summary>
		/// Compute the complementary incomplete gamma function for a near 
		/// a negative integer and for small x.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="x"></param>
		/// <param name="alx"></param>
		/// <returns></returns>
		/// <remarks>
		/// This is a translation from the Fortran version of D9GMIC, SLATEC, FNLIB,
		/// CATEGORY C7E, REVISION 900720, originally written by Fullerton W.,(LANL)
		/// to C++.
		///
		/// Routines called: LnGamma(x)
		/// </remarks>
		static double d9gmic (double a, double x, double alx)
		{

			double ret_val, alng, sgng, s, t, fkp1, fk, fm, te;

			if (a > 0.0) 
				throw new ArgumentException("a must be near a negative integer");

			if (x <= 0.0) 
				throw new ArgumentException("x must be > zero");

			int m = (int) (-(a - 0.5));
			fm = (double) m;

			te = 1.0;
			t = 1.0;
			s = t;
			for (int k = 1; k <= 200; ++k) 
			{
				fkp1 = (double) (k + 1);
				te = -x * te / (fm + fkp1);
				t = te / fkp1;
				s += t;
				if (Math.Abs(t) < eps_d9gmic * s) goto L30;
			}

			throw new ArgumentException("no convergence in 200 terms of continued fraction");

			L30:
				ret_val = -alx - euler_d9gmic + x * s / (fm + 1.0);

			if (m == 0) 
				return ret_val;

			if (m == 1) 
				return -ret_val - 1.0 + 1.0 / x;

			te = fm;
			t = 1.0;
			s = t;
			int mm1 = m - 1;
			for (int k = 1; k <= mm1; ++k) 
			{
				fk = (double) k;
				te = -x * te / fk;
				t = te / (fm - fk);
				s += t;
				if (Math.Abs(t) < eps_d9gmic * Math.Abs(s)) goto L50;
			}

			L50:
				for (int k = 1; k <= m; ++k)
					ret_val += 1.0 / k;

			sgng = 1.0;
			if (m % 2 == 1) sgng = -1.0;
			alng = Math.Log(ret_val) - LnGamma(fm + 1.0);

			ret_val = 0.0;
			if (alng > bot_d9gmic)
				ret_val = sgng * Math.Exp(alng);

			if (s != 0.0)
				ret_val += CopySign(Math.Exp(-fm * alx + Math.Log(Math.Abs(s) / fm)),s);

			if (ret_val == 0.0 && s == 0.0)
				throw new ArgumentException("result underflows");

			return ret_val;
		}

		#endregion

		#region dgamlm

		static readonly double alnsml = Math.Log(DBL_MIN);
		static readonly double	alnbig = Math.Log(DBL_MAX);

		/// <summary>
		/// Calculate the minimum and maximum legal bounds for x in Gamma(x). 
		/// xmin and xmax are not the only bounds, but they are the only non- 
		/// trivial ones to calculate. 
		/// </summary>
		/// <param name="xmin">
		/// double precision minimum legal value of x in gamma(x).  Any 
		/// smaller value of x might result in underflow. 
		/// </param>
		/// <param name="xmax">
		/// 	xmax	double precision maximum legal value of x in gamma(x).  Any 
		///      	larger value of x might cause overflow. 
		/// </param>
		/// <remarks>
		/// This is a translation from the Fortran version of SLATEC, FNLIB,
		/// CATEGORY C7A, R2 REVISION 900315, originally written by Fullerton W.,(LANL)
		/// to C++.
		/// </remarks>
		static void dgamlm (out double xmin, out double xmax)
		{

			double xold,xln;
			int i;

			xmin = -alnsml;
			for (i = 1; i <= 10; ++i) 
			{
				xold = xmin;
				xln = Math.Log(xmin);
				xmin -= xmin * ((xmin + 0.5) * xln - xmin - 0.2258 + alnsml)/(xmin * xln + 0.5);
				if (Math.Abs(xmin - xold) < 0.005) goto L20;
			}

			xmin = xmax = double.NaN;
			throw new ArgumentException("unable to find xmin");

			L20:
				xmin = -xmin + 0.01;
			xmax = alnbig;
			for (i = 1; i <= 10; ++i) 
			{
				xold = xmax;
				xln = Math.Log(xmax);
				xmax -= xmax * ((xmax - 0.5) * xln - xmax + 0.9189 - alnbig) 
					/ (xmax * xln - 0.5);
				if (Math.Abs(xmax - xold) < 0.005) goto L40;
			}

			xmin = xmax = double.NaN;
			throw new ArgumentException("unable to find xmax");

			L40:
				xmax += -0.01;
			xmin = Math.Max(xmin,-xmax + 1.0);
		}


		#endregion

		#region LnGamma

		/// <summary>
		/// LnGamma(x,sgn) returns the double precision natural
		/// logarithm of the absolute value of the Gamma function for
		/// double precision argument x. The sign of the gamma function
		/// is returned in sgn.
		/// </summary>
		/// <param name="x">The function argument x.</param>
		/// <param name="sgn">The sign of the gamma function.</param>
		/// <returns>The logarithm of the absolute value of the gamma function.</returns>
		/// <remarks>
		/// This is a translation from the Fortran version of DLGAMS, SLATEC, FNLIB,
		/// CATEGORY C7A, REVISION 891214, originally written by Fullerton W.,(LANL)
		/// to C++.
		///
		/// Routines called: 
		///  LnGamma(x)
		/// </remarks>
		public static double LnGamma (double x, out double sgn)
		{
			if (x <= 0.0 && (int)(Dmod(-Dint(x), 2.0) + 0.1) == 0) 
				sgn = -1.0;
			else
				sgn = 1.0;
  
			return LnGamma(x);
		}

		/// <summary>
		/// Calculates the double precision logarithm of the 
		/// absolute value of the Gamma function for double precision 
		/// argument x. 
		/// </summary>
		/// <param name="x">The function argument x.</param>
		/// <returns>The logarithm of the absolute value of the Gamma function.</returns>
		/// <remarks>
		/// This is a translation from the Fortran version of DLNGAM, SLATEC, FNLIB,
		/// CATEGORY C7A, REVISION 900727, originally written by Fullerton W.,(LANL)
		/// to C++.
		///
		/// Routines called: 
		///   Gamma(x)
		///   d9lgmc(x)
		/// </remarks>
		public static double LnGamma (double x)
		{
			const double sq2pil = 0.91893853320467274178032973640562;
			const double sqpi2l = 0.225791352644727432363097614947441;
			const double pi     = 3.1415926535897932384626433832795;
			double temp   = 1.0 / Math.Log(DoubleConstants.DBL_MAX);
			double xmax   = temp * DoubleConstants.DBL_MAX;
			double dxrel  = Math.Sqrt(DoubleConstants.DBL_EPSILON);

			double y = Math.Abs(x);
			if (y > 10.0) goto L20;

			// log(abs(Gamma(x))) for abs(x) <= 10.0 
			return Math.Log(Math.Abs(Gamma(x)));

			L20:
				if (y > xmax) 
					throw new ArgumentException("Abs(x) so big LnGamma(x) overflows");

			if (x > 0.0)
				return sq2pil + (x - 0.5) * Math.Log(x) - x + d9lgmc(y);
    
			double sinpiy = Math.Abs(Math.Sin(pi * y));

			if (sinpiy == 0.0) 
				throw new ArgumentException("x is a negative integer");

			if (Math.Abs((x - Dint(x - 0.5)) / x) < dxrel) 
				System.Diagnostics.Trace.WriteLine("answer less than half precision because x too near negative integer");
    
			return sqpi2l + (x - 0.5) * Math.Log(y) - x - Math.Log(sinpiy) - d9lgmc(y);
		}

		#endregion

		#region Gamma

		static readonly double[] gamcs = 
	{ 
		0.008571195590989331421920062399942,
		0.004415381324841006757191315771652,
		0.05685043681599363378632664588789,
		-.004219835396418560501012500186624,
		0.001326808181212460220584006796352,
		-1.893024529798880432523947023886e-4,
		3.606925327441245256578082217225e-5,
		-6.056761904460864218485548290365e-6,
		1.055829546302283344731823509093e-6,
		-1.811967365542384048291855891166e-7,
		3.117724964715322277790254593169e-8,
		-5.354219639019687140874081024347e-9,
		9.19327551985958894688778682594e-10,
		-1.577941280288339761767423273953e-10,
		2.707980622934954543266540433089e-11,
		-4.646818653825730144081661058933e-12,
		7.973350192007419656460767175359e-13,
		-1.368078209830916025799499172309e-13,
		2.347319486563800657233471771688e-14,
		-4.027432614949066932766570534699e-15,
		6.910051747372100912138336975257e-16,
		-1.185584500221992907052387126192e-16,
		2.034148542496373955201026051932e-17,
		-3.490054341717405849274012949108e-18,
		5.987993856485305567135051066026e-19,
		-1.027378057872228074490069778431e-19,
		1.762702816060529824942759660748e-20,
		-3.024320653735306260958772112042e-21,
		5.188914660218397839717833550506e-22,
		-8.902770842456576692449251601066e-23,
		1.527474068493342602274596891306e-23,
		-2.620731256187362900257328332799e-24,
		4.496464047830538670331046570666e-25,
		-7.714712731336877911703901525333e-26,
		1.323635453126044036486572714666e-26,
		-2.270999412942928816702313813333e-27,
		3.896418998003991449320816639999e-28,
		-6.685198115125953327792127999999e-29,
		1.146998663140024384347613866666e-29,
		-1.967938586345134677295103999999e-30,
		3.376448816585338090334890666666e-31,
		-5.793070335782135784625493333333e-32 
	};

		static int n_Gamma;
		static bool first_Gamma = true;
		static double xmin_Gamma, xmax_Gamma;

		/// <summary>
		/// Gamma(x) calculates the double precision complete Gamma function 
		/// for double precision argument x. 
		/// </summary>
		/// <param name="x">The function argument x.</param>
		/// <returns>The Gamma function of the argument x.</returns>
		/// <remarks>
		/// This is a translation from the Fortran version of SLATEC, FNLIB,
		/// CATEGORY C7A, REVISION 920618, originally written by Fullerton W.,(LANL)
		/// to C++.
		///
		/// Series for GAM        on the interval  0.          to  1.00000E+00 
		///                                        with weighted error   5.79E-32 
		///                                         log weighted error  31.24 
		///                               significant figures required  30.00 
		///                                    decimal places required  32.05 
		/// </remarks>
		public static double Gamma(double x)
		{
    
			const double pi     = 3.1415926535897932384626433832795;
			const double sq2pil = 0.91893853320467274178032973640562;
			double	dxrel  = Math.Sqrt(DBL_EPSILON);

			double ret_val;

			if (first_Gamma) 
			{
				n_Gamma = Series.initds(gamcs, 42, 0.5 * DBL_EPSILON * 0.1);
				dgamlm(out xmin_Gamma, out xmax_Gamma);
				first_Gamma = false;
			}

			int i,n;

			double y = Math.Abs(x);
			if (y > 10.0) goto L50;    

			// compute gamma(x) for -xbnd <= x <= xbnd.  Reduce interval and find 
			// gamma(1+y) for 0.0 <= y < 1.0 first of all. 

			n = (int)x;
			if (x < 0.0) --n;
			y = x - n;
			--n;

			ret_val = Series.dcsevl(y * 2.0 - 1.0, gamcs, n_Gamma) + 0.9375;
			if (n == 0) return ret_val;
    
			if (n > 0) goto L30;

			// compute gamma(x) for x < 1.0 

			n = -n;
			if (x == 0.0) 
				throw new ArgumentException("x is 0");

			if (x < 0.0 && x + n - 2 == 0.0) 
				throw new ArgumentException("x is a negative integer");

			if (x < -0.5 && Math.Abs((x - Dint(x - 0.5)) / x) < dxrel) 
				System.Diagnostics.Trace.WriteLine("Gamma function: answer less than half precision because x too near negative integer");

			for (i = 1; i <= n; ++i) 
				ret_val /= x + i - 1;
			return ret_val;

			// gamma(x) for x >= 2.0 and x <= 10.0 

			L30:
				for (i = 1; i <= n; ++i) 
					ret_val = (y + i) * ret_val;
			return ret_val;
    
			// gamma(x) for abs(x) > 10.0.  recall y = abs(x).

			L50:
				if (x > xmax_Gamma) 
					throw new ArgumentException("x so big Gamma(x) overflows");	

			ret_val = 0.0;

			if (x < xmin_Gamma)
				System.Diagnostics.Trace.WriteLine("Gamma function: x so small Gamma(x) underflows");
    
			if (x < xmin_Gamma) return ret_val;
    
			ret_val = Math.Exp((y - 0.5) * Math.Log(y) - y + sq2pil + d9lgmc(y));
			if (x > 0.0) return ret_val;
    
			if (Math.Abs((x - Dint(x - 0.5)) / x) < dxrel) 
				System.Diagnostics.Trace.WriteLine("Gamma function: answer less than half precision because x too near negative integer");

			double sinpiy = Math.Sin(pi * y);
			if (sinpiy == 0.0) 
				throw new ArgumentException("x is a negative integer");	

			return -pi / (y * sinpiy * ret_val);
		}

		#endregion

		#region Fac

		static readonly double[] facn = 
	{ 
		1.0,
		1.0,
		2.0,
		6.0,
		24.0,
		120.0,
		720.0,
		5040.0,
		40320.0,
		362880.0,
		3628800.0,
		39916800.0,
		479001600.0,
		6227020800.0,
		87178291200.0,
		1.307674368e12,
		2.0922789888e13,
		3.55687428096e14,
		6.402373705728e15,
		1.21645100408832e17,
		2.43290200817664e18,
		5.109094217170944e19,
		1.12400072777760768e21,
		2.585201673888497664e22,
		6.2044840173323943936e23,
		1.5511210043330985984e25,
		4.03291461126605635584e26,
		1.0888869450418352160768e28,
		3.04888344611713860501504e29,
		8.841761993739701954543616e30,
		2.6525285981219105863630848e32 
	};

		const double sq2pil_Fac = 0.91893853320467274178032973640562;
		static int nmax_Fac = 0;
		static double xmin_Fac, xmax_Fac;

		/// <summary>
		/// Fac(n) calculates the double precision factorial for the integer argument n.
		/// </summary>
		/// <param name="n">The argument n.</param>
		/// <returns>The factorial of n.</returns>
		/// <remarks>
		/// This is a translation from the Fortran version of SLATEC, FNLIB,
		/// CATEGORY C1, REVISION 900315, originally written by Fullerton W.,(LANL)
		/// to C++.
		/// </remarks>
		public static double Fac (int n)
		{
    
    
			if (nmax_Fac == 0) 
			{
				dgamlm(out xmin_Fac, out xmax_Fac);
				nmax_Fac = (int) (xmax_Fac - 1.0);
			}

			if (n < 0) 
				throw new ArgumentException("factorial of negative integer undefined");

			if (n <= 30)
				return facn[n];
			if (n > nmax_Fac) 
				throw new ArgumentException("n so big Fac(n) overflows");

			double x = (double) (n + 1);
			return Math.Exp((x - 0.5) * Math.Log(x) - x + sq2pil_Fac + d9lgmc(x));
		}

		#endregion

		#region RcpGamma

		/// <summary>
		/// RcpGamma(x) calculates the double precision reciprocal of the
		/// complete Gamma function for double precision argument x.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		/// <remarks>
		/// This is a translation from the Fortran version of DGAMR, SLATEC, FNLIB,
		/// CATEGORY C7A, REVISION 900727, originally written by Fullerton W.,(LANL)
		/// to C++.
		///
		/// Routines called: 
		///   Gamma(x)
		///   LnGamma(x,sgn)
		/// </remarks>
		static double RcpGamma (double x)
		{
			if (x <= 0.0 && Dint(x) == x)
				return 0.0;
			else if (Math.Abs(x) > 10.0) 
			{
				double alngx, sgngx;
				alngx = LnGamma(x, out sgngx);
				return sgngx * Math.Exp(-alngx);
			} 
			else
				return 1.0 / Gamma(x); 
		}


		#endregion

		#region GammaI

		/// <summary>
		/// Evaluate the incomplete gamma function defined by
		///
		///   GammaI = integral from t = 0 to x of exp(-t) * t**(a-1.0)
		///
		/// GammaI(x,a) is evaluated for positive values of a and non-negative values
		/// of x.  A slight deterioration of 2 or 3 digits accuracy will occur
		/// when GammaI is very large or very small, because logarithmic variables
		/// are used.  The function and both arguments are double precision.
		/// </summary>
		/// <param name="x">The function value x.</param>
		/// <param name="a">The exponent.</param>
		/// <returns>The incomplete gamma function of x and a.</returns>
		/// <remarks>
		/// This is a translation from the Fortran version of DGAMI, SLATEC, FNLIB,
		/// CATEGORY C7E, REVISION 900315, originally written by Fullerton W.,(LANL)
		/// to C++.
		///
		/// Routines called: 
		///   LnGamma(x)      - logarithm of the Gamma function
		///   GammaIT(x,a)    - Tricomi's incomplete gamma function
		/// </remarks>
		static double GammaI (double x, double a)
		{  
			if (a <= 0.0) 
				throw new ArgumentException("a must be greater than zero");
			if (x < 0.0) 
				throw new ArgumentException("x must be greater or equal than zero");
  
			// The only error possible in the expression below is a fatal overflow.
			return (x == 0.0) ? 0.0 : Math.Exp(LnGamma(a)+a*Math.Log(x)) * GammaIT(x,a);
		}


		#endregion

		#region GammaIT

		static readonly double  alneps_GammaIT = -Math.Log(0.5 * DBL_EPSILON);
		static readonly double	sqeps_GammaIT  = Math.Sqrt(DBL_EPSILON);


		/// <summary>
		/// GammaIT(x,a) evaluates Tricomi's incomplete gamma function defined by
		/// 
		///   GammaIT = x**(-a)/Gamma(a) * integral from 0 to x of exp(-t) * t**(a-1.0)
		/// 
		/// for a > 0.0 and by analytic continuation for a &lt;= 0.0.
		/// Gamma(x) is the complete gamma function of x.
		/// </summary>
		/// <param name="x">The function argument x.</param>
		/// <param name="a">The function argument a.</param>
		/// <returns>Tricomi's incomplete gamma function of x and a.</returns>
		/// <remarks>
		/// GammaIT(x,a) is evaluated for arbitrary real values of a and for
		/// non-negative values of x (even though GammaIT is defined for x &lt; 0.0), 
		/// except that for x = 0 and a &lt;= 0.0, GammaIT is infinite,
		/// which is a fatal error.
		/// 
		/// The function and both arguments are double.
		/// A slight deterioration of 2 or 3 digits accuracy will occur when
		/// GammaIT is very large or very small in absolute value, because log-
		/// arithmic variables are used.  Also, if the parameter  a  is very 
		/// close to a negative integer (but not a negative integer), there is
		/// a loss of accuracy, which is reported if the result is less than
		/// half machine precision.
		///
		/// This is a translation from the Fortran version of DGAMIT, SLATEC, FNLIB,
		/// CATEGORY C7E, REVISION 920528, originally written by Fullerton W.,(LANL)
		/// to C++.
		///
		/// References:
		///    (1) W. Gautschi, A computational procedure for incomplete 
		///        gamma functions, ACM Transactions on Mathematical 
		///        Software 5, 4 (December 1979), pp. 466-481.
		///    (2) W. Gautschi, Incomplete gamma functions, Algorithm 542,
		///        ACM Transactions on Mathematical Software 5, 4 
		///        (December 1979), pp. 482-489.
		///
		/// Routines called: 
		///   LnGamma(x)      - logarithm of the Gamma function
		///   LnGamma(x,sgn)  - logarithm and sign of the Gamma function
		///   RcpGamma(x)     - reciprocal of the Gamma function
		///   d9gmit(a,x,algap1,sgngam)
		///   d9lgit(a,x,algap1) 
		///   d9lgic(a,x,alx)
		/// </remarks>
		static double GammaIT(double x, double a)
		{
			// machine constants

			double alng, aeps, h, t, ainta, sgngam=0.0, sga, alx = 0.0, algap1 = 0.0;
 
			if (x < 0.0) 
				throw new ArgumentException("x must be greater or equal than zero");
  
			if (x != 0.0) 
				alx = Math.Log(x);

			sga = (a < 0.0) ? -1.0 : 1.0;
			ainta = Dint(a + sga * 0.5);
			aeps = a - ainta;
  
			if (x > 0.0) 
			{
				if (x > 1.0) 
				{
					if (a < x) 
					{
						alng = d9lgic(a, x, alx);	
						// evaluate dgamit in terms of log(dgamic(a, x))
						h = 1.0;
						if (aeps == 0.0 && ainta <= 0.0) goto L50;
						algap1 = LnGamma(a + 1.0, out sgngam);
						t = Math.Log(Math.Abs(a)) + alng - algap1;
						if (t > alneps_GammaIT) 
						{
							t -= a * alx;
							return  -sga * sgngam * Math.Exp(t);  
						}

						if (t > -alneps_GammaIT)
							h = 1.0 - sga * sgngam * Math.Exp(t);
	
						if (Math.Abs(h) > sqeps_GammaIT) goto L50;
	
						System.Diagnostics.Trace.WriteLine("Warning (GammaIT function): answer less than half precision");
	
					L50:
						t = -a * alx + Math.Log(Math.Abs(h));
						return CopySign(Math.Exp(t), h);
					} 
					else 
					{ // a >= x
						t = d9lgit(a, x, LnGamma(a + 1.0));
						return Math.Exp(t);
					}
				} 
				else 
				{ // x <= 1.0
					if (a >= -0.5 || aeps != 0.0)
						algap1 = LnGamma(a + 1.0, out sgngam);
					return d9gmit(a, x, algap1, sgngam);
				}
			} 
			else 
			{ // x == 0
				if (ainta > 0.0 || aeps != 0.0)
					return RcpGamma(a + 1.0);
				else
					return 0.0;
			}
		}

		#endregion

		#region GammaIC

		const double eps_GammaIC = 0.25 * DBL_EPSILON;
		static readonly double sqeps_GammaIC = Math.Sqrt(DBL_EPSILON);
		static readonly double alneps_GammaIC = -Math.Log(0.5 * DBL_EPSILON);
		
		/// <summary>
		/// Evaluate the complementary incomplete Gamma function
		///
		///   GammaIC(x,a) = integral from x to infinity of exp(-t) * t**(a-1)
		///
		/// GammaIC(x,a) is evaluated for arbitrary real values of A and for
		/// non-negative values of x (even though GammaIC is defined for x &lt; 0.0), 
		/// except that for x = 0 and a &lt;= 0.0, GammaIC is undefined.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		/// <remarks>
		/// A slight deterioration of 2 or 3 digits accuracy will occur when
		/// GammaIC is very large or very small in absolute value, because log-
		/// arithmic variables are used.  Also, if the parameter A is very close
		/// to a negative integer (but not a negative integer), there is a loss 
		/// of accuracy, which is reported if the result is less than half
		/// machine precision.
		///
		/// This is a translation from the Fortran version of DGAMIC, SLATEC, FNLIB,
		/// CATEGORY C7E, REVISION 920528, originally written by Fullerton W.,(LANL)
		/// to C++.
		///
		/// References:
		///
		/// (1) W. Gautschi, A computational procedure for incomplete
		///     gamma functions, ACM Transactions on Mathematical
		///     Software 5, 4 (December 1979), pp. 466-481.
		/// (2) W. Gautschi, Incomplete gamma functions, Algorithm 542,
		///     ACM Transactions on Mathematical Software 5, 4 
		///     (December 1979), pp. 482-489.
		///
		/// Routines called: d9gmit, d9gmic, d9lgic, d9lgit, LnGamma(x), LnGamma(x,a)
		/// </remarks>
		static double GammaIC (double x, double a)
		{

			double aeps, sgng, e, h, t, ainta, alngs=0, gstar, sgngs=0, algap1, sgngam, sga, alx;
			bool izero;

			if (x < 0.0) 
				throw new ArgumentException("x must be greater or equal than zero");

			if (x > 0.0) goto L20;

			if (a > 0.0)
				return Math.Exp(LnGamma(a + 1.0) - Math.Log(a));
			else 
				throw new ArgumentException("x = 0 and a <= 0 so GammaIC is undefined");

			L20:
				alx = Math.Log(x);
			sga = (a < 0) ? -1.0 : 1.0;
			ainta = Dint(a + sga * 0.5);
			aeps = a - ainta;

			izero = false;
			if (x >= 1.0) goto L40;

			if (a > 0.5 || Math.Abs(aeps) > 0.001) goto L30;

			e = 2.0;
			if (-ainta > 1.0)
				e = (-ainta + 2.0) * 2.0 / (ainta * ainta - 1.0);

			e -= alx * Math.Pow(x, -0.001);
			if (e * Math.Abs(aeps) > eps_GammaIC) goto L30;

			return d9gmic(a, x, alx);

			L30:
				algap1 = LnGamma(a + 1.0, out sgngam);
			gstar = d9gmit(a, x, algap1, sgngam);

			if (gstar == 0.0) 
				izero = true;
			else 
			{
				alngs = Math.Log((Math.Abs(gstar)));
				sgngs = Math.Sign(gstar);
			}

			goto L50;

			L40:
				if (a < x)  
					return Math.Exp(d9lgic(a, x, alx));

			sgngam = 1.0;
			algap1 = LnGamma(a + 1.0);
			sgngs = 1.0;
			alngs = d9lgit(a, x, algap1);

			// evaluation of GammaIC(x,a) in terms of Tricomi's incomplete gamma function

			L50:

				h = 1.0;

			if (!izero) 
			{
				t = a * alx + alngs;
				if (t > alneps_GammaIC) 
				{
					sgng =  -sgngs * sga * sgngam;
					t += algap1 - Math.Log((Math.Abs(a)));
					return sgng * Math.Exp(t);
				}
    
				if (t > -alneps_GammaIC)
					h = 1.0 - sgngs * Math.Exp(t);
    
				if (Math.Abs(h) < sqeps_GammaIC) 
				{
					System.Diagnostics.Trace.WriteLine("Warning (GammaIC function): answer less than half precision");
				}
			}

			sgng = Math.Sign(h) * sga * sgngam;
			t = Math.Log(Math.Abs(h)) + algap1 - Math.Log((Math.Abs(a)));
			return sgng * Math.Exp(t);
		}


		#endregion

		#region LnBeta and Beta
		
		/// <summary>
		/// LnBeta(a,b) calculates the double precision natural logarithm of
		/// the complete beta function for double precision arguments a and b.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns>The logarithm of the complete beta function.</returns>
		/// <remarks>
		/// This is a translation from the Fortran version of DLBETA(A,B), SLATEC, FNLIB,
		/// CATEGORY C7B, REVISION 900727, originally written by Fullerton W.,(LANL)
		/// to C++.
		/// 
		/// Calls d9lgmc(x), LogRel(x), LnGamma(x), Gamma(x)
		/// </remarks>
		public static double LnBeta (double a, double b)
		{
			const double sq2pil = 0.91893853320467274178032973640562;

			double ret_val,
				p = Math.Min(a,b),
				q = Math.Max(a,b);

			if (p <= 0.0) 
				throw new ArgumentException("both arguments must be greater than zero");

			double d1 = p + q;

			if (p >= 10.0) 
			{
				// p and q are big
				double corr = d9lgmc(p) + d9lgmc(q) - d9lgmc(d1);
				double d3 = Math.Log(q) * -0.5 + sq2pil;
				double d2 = d3 + corr;
				d1 = d2 + (p - 0.5) * Math.Log(p / (p + q));
				double d4 = -p / (p + q);
				ret_val = d1 + q * LogRel(d4);

			} 
			else if (q >= 10.0) 
			{
				// p is small, but q is big
				double corr = d9lgmc(q) - d9lgmc(d1);
				d1 = LnGamma(p) + corr;
				double d2 = -p / (p + q);
				ret_val = d1 + p - p * Math.Log(p + q) + (q - 0.5) * LogRel(d2);

			} 
			else 
			{
				// p and q are small
				ret_val = Math.Log( Gamma(p) * (Gamma(q) / Gamma(d1)) );
			}

			return ret_val;
		}


		/// <summary>
		/// Beta(a,b) calculates the complete beta function for double precision arguments a and b using Exp(LnBeta(a,b)).
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns>The complete beta function of arguments a and b.</returns>
		public static double Beta(double a, double b)
		{
			return Math.Exp(LnBeta(a,b));
		}

		#endregion

		#region BetaI and BetaIR

		const double eps_BetaI    = 0.5 * DBL_EPSILON;
		const double sml_BetaI    = DBL_MIN;
		static readonly double alneps_BetaI = Math.Log(eps_BetaI);
		static readonly double alnsml_BetaI = Math.Log(sml_BetaI);

		/// <summary>
		/// BetaIR(x,a,b) calculates the double precision incomplete beta function ratio.
		///
		///                 B_x(a,b)    Integral(0,x) t**(a-1) (1-t)**(b-1) dt
		///     I_x(a,b) = --------- = ---------------------------------------
		///                 B(a,b)                   B(a,b)
		/// </summary>
		/// <param name="x">upper limit of integration.  x must be in (0,1) inclusive.</param>
		/// <param name="pin">first beta distribution parameter. a must be > 0.</param>
		/// <param name="qin">second beta distribution parameter.  b must be > 0.</param>
		/// <returns>The incomplete beta function ratio.</returns>
		/// <remarks>
		/// The incomplete beta function ratio is the probability that a
		/// random variable from a beta distribution having parameters a and b
		/// will be less than or equal to x.
		/// This is a translation from the Fortran version of DBETAI(X,PIN,QIN), SLATEC,
		/// FNLIB, CATEGORY C7F, REVISION 920528, originally written by Fullerton W.,(LANL)
		/// to C++.
		/// 
		/// References: Nancy E. Bosten and E. L. Battiste, Remark on Algorithm 179, 
		///             Communications of the ACM 17, 3 (March 1974), pp. 156.
		/// 
		/// Calls   LnBeta(a,b)
		/// </remarks>
		public static double BetaIR(double x, double pin, double qin)
		{
    
			double ret_val, d__1, term, c__, p, q, y, p1, xb, xi, ps, finsum;
			int i__, i__1, n, ib;

			// check arguments
			if (x < 0.0 || x > 1.0) 
				throw new ArgumentException("x is not in the range (0,1)");

			if (pin <= 0.0 || qin <= 0.0) 
				throw new ArgumentException("p and/or q is less or equal than zero");

			y = x;
			p = pin;
			q = qin;

			if (q <= p && x < 0.8) goto L20;
			if (x < 0.2) goto L20;

			y = 1.0 - y;
			p = qin;
			q = pin;

			L20:
				if ((p + q) * y / (p + 1.0) < eps_BetaI) goto L80;

			// Evaluate the infinite sum first.  Term will equal
			// y**p/beta(ps,p) * (1.-ps)-sub-i * y**i / fac(i).

			ps = q - Dint(q);
			if (ps == 0.0) ps = 1.0;
			xb = p * Math.Log(y) - LnBeta(ps, p) - Math.Log(p);
			ret_val = 0.0;
			if (xb < alnsml_BetaI) goto L40;

			ret_val = Math.Exp(xb);
			term = ret_val * p;
			if (ps == 1.0) goto L40;

			// Computing MAX 
			d__1 = alneps_BetaI / Math.Log(y);
			n = (int) Math.Max(d__1,4.0);
			i__1 = n;
			for (i__ = 1; i__ <= i__1; ++i__) 
			{
				xi = (double) i__;
				d__1 = term * (xi - ps);
				term = d__1 * y / xi;
				ret_val += term / (p + xi);
			}

			// Now evaluate the finite sum, maybe.

			L40:
				if (q <= 1.0) goto L70;

			xb = p * Math.Log(y) + q * Math.Log(1.0 - y) - LnBeta(p, q) - Math.Log(q);

			// Computing MAX
			d__1 = xb / alnsml_BetaI;
			ib = (int) Math.Max(d__1,0.0);
			term = Math.Exp(xb - ib * alnsml_BetaI);
			c__ = 1.0 / (1.0 - y);
			d__1 = p + q;
			p1 = q * c__ / (d__1 - 1.0);

			finsum = 0.0;
			n = (int) q;
			if (q == (double) n) --n;

			i__1 = n;
			for (i__ = 1; i__ <= i__1; ++i__) 
			{
				if (p1 <= 1.0 && term / eps_BetaI <= finsum) goto L60;
				xi = (double) i__;
				d__1 = (q - xi + 1.0) * c__;
				term = d__1 * term / (p + q - xi);
				if (term > 1.0) --ib;
				if (term > 1.0) term *= sml_BetaI;
				if (ib == 0) finsum += term;
			}

			L60:
				ret_val += finsum;

			L70:
				if (y != x || p != pin)
					ret_val = 1.0 - ret_val;

			// Computing MAX
			d__1 = Math.Min(ret_val,1.0);
			ret_val = Math.Max(d__1,0.0);
			return ret_val;
  
			L80:
				ret_val = 0.0;
			xb = p * Math.Log((Math.Max(y,sml_BetaI))) - Math.Log(p) - LnBeta(p,q);
			if (xb > alnsml_BetaI && y != 0.0)
				ret_val = Math.Exp(xb);
			if (y != x || p != pin)
				ret_val = 1.0 - ret_val;
  
			return ret_val;
		}


		/// <summary>
		/// BetaI(x,a,b) calculates the double precision incomplete beta function.
		/// BetaI(x,a,b) = Integral(0,x) t**(a-1) (1-t)**(b-1) dt
		/// </summary>
		/// <param name="x"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns>The incomplete beta function of the parameters x, a and b.</returns>
		public static double BetaI(double x, double a, double b)
		{
			return BetaIR(x,a,b)/Beta(a,b);
		}

		#endregion

		#region LogRel (complex)

		/// <summary>
		/// LogRel(z) = log(1+z) with relative error accuracy near z = 0.
		/// </summary>
		/// <param name="z">The complex argument z.</param>
		/// <returns>Log(1+z) with relative error accuracy near z=0.</returns>
		/// <remarks>
		/// April 1977 version.  W. Fullerton, c3, Los Alamos Scientific Lab.
		///
		/// let   rho = abs(z)  and
		///       r**2 = abs(1+z)**2 = (1+x)**2 + y**2 = 1 + 2*x + rho**2 .
		/// now if rho is small we may evaluate LogRel(z) accurately by
		///       log(1+z) = complex (log(r), arg(1+z))
		///                = complex (0.5*log(r**2), arg(1+z))
		///                = complex (0.5*LogRel(2*x+rho**2), arg(1+z))
		/// </remarks>
		static Complex LogRel(Complex z)
		{
			if (ComplexMath.Abs(1.0 + z) < Math.Sqrt(DBL_EPSILON))
				System.Diagnostics.Trace.WriteLine("Warning (LogRel): answer less than half precision because z too near -1");

			double rho = ComplexMath.Abs(z);
			if (rho > 0.375)
				return ComplexMath.Log(1.0 + z);
		
			return new Complex(0.5*LogRel(2.0*z.Re+rho*rho), ComplexMath.Arg(1.0+z));
		}

		#endregion

		#region c9lgmc

		static readonly double[] _c9lgmc_bern = {
																							0.083333333333333333,
																							-0.0027777777777777778,
																							0.00079365079365079365,
																							-0.00059523809523809524,
																							0.00084175084175084175,
																							-0.0019175269175269175,
																							0.0064102564102564103,
																							-0.029550653594771242,
																							0.17964437236883057,
																							-1.3924322169059011,
																							13.402864044168392
																						};

		static int _c9lgmc_nterm = 0;
		static double _c9lgmc_bound = 0.0; 
		static double 						 _c9lgmc_xbig  = 0.0;
		static double 								 _c9lgmc_xmax  = 0.0;


		/// <summary>
		/// Compute the log gamma correction term for large abs(z) when real(z) &gt; 0.0 
		/// and for large abs(imag(y)) when real(z) &lt; 0.0.  
		/// Matpack special functions - c9lgmc() log gamma correction term 
		/// </summary>
		/// <param name="zin">The complex parameter z.</param>
		/// <returns>The log gamma correction term.</returns>
		/// <remarks>
		/// April 1978 edition.  w. fullerton c3, los alamos scientific lab.
		/// We find c9lgmc so that
		/// clog(cgamma(z)) = 0.5*alog(2.*pi) + (z-0.5)*clog(z) - z + c9lgmc(z).
		/// </remarks>
		static Complex c9lgmc (Complex zin)
		{
			Complex z, z2inv, retval;


			if (_c9lgmc_nterm == 0) 
			{
				_c9lgmc_nterm = (int)(-0.30*Math.Log(0.5 * DBL_EPSILON));
				_c9lgmc_bound = Math.Pow(0.1170*_c9lgmc_nterm*(0.1*0.5*DBL_EPSILON),-1.0/(2.0*_c9lgmc_nterm-1.0));
				_c9lgmc_xbig = 1.0/Math.Sqrt(0.5 * DBL_EPSILON);
				_c9lgmc_xmax = Math.Exp(Math.Min(Math.Log(DBL_MAX/12.0),-Math.Log(12.0*DBL_MIN)));
			}

			z = zin;
			double x = z.Re,
				y = z.Im,
				cabsz = ComplexMath.Abs(z);

			if (x < 0.0 && Math.Abs(y) < _c9lgmc_bound)
				throw new ArgumentException("not valid for negative real(z) and small abs(imag(z))");	

			if (cabsz < _c9lgmc_bound) 
				throw new ArgumentException("not valid for small cabs(z)");	

			if (cabsz >= _c9lgmc_xmax)
			{
				System.Diagnostics.Trace.WriteLine("Warning (c9lgm): z so big c9lgmc underflows");
				return new Complex(0.0, 0.0);

			}
			else
			{
				if (cabsz >= _c9lgmc_xbig)
					return 1.0/(12.0*z);
    
				z2inv = 1.0/(z*z);
				retval = new Complex(0.0,0.0);
				for (int i = 1; i <= _c9lgmc_nterm; i++)
					retval = _c9lgmc_bern[_c9lgmc_nterm - i] + retval * z2inv;
				return  retval / z;
			}
		} 

		#endregion

		#region Gamma(complex)



		/// <summary>
		/// Complex Gamma function.
		/// </summary>
		/// <param name="z">The complex argument.</param>
		/// <returns>The Gamma function of the complex argument z.</returns>
		/// <remarks>
		///-----------------------------------------------------------------------------//
		/// july 1977 edition.  w. fullerton, c3, los alamos scientific lab.
		/// a preliminary version that is portable, but not accurate enough.
		///-----------------------------------------------------------------------------//
		/// </remarks>
		public static Complex Gamma(Complex z)
		{
			return ComplexMath.Exp(LnGamma(z));
		}

		#endregion

		#region LnGamma(complex)

		static double _LnGamma_bound = 0.0;
		static double						 _LnGamma_dxrel = 0.0; 
		static double							 _LnGamma_rmax  = 0.0;

		
		//-----------------------------------------------------------------------------//
		// August 1980 edition.  W. Fullerton c3, Los Alamos Scientific Lab.
		// Eventually clngam should make use of c8lgmc for all z except for
		// z in the vicinity of 1 and 2.
		//-----------------------------------------------------------------------------//

		static Complex LnGamma (Complex zin)
		{
			const double sq2pil = 0.91893853320467274;

			Complex retval;



			if (_LnGamma_bound == 0.0) 
			{
				int nn = (int)(-0.30 * Math.Log(0.5*DBL_EPSILON));
				_LnGamma_bound = 0.1171 * nn * Math.Pow(0.1 * 0.5 * DBL_EPSILON, -1.0/(2.0*nn-1.0));
				_LnGamma_dxrel = Math.Sqrt(DBL_EPSILON);
				_LnGamma_rmax  = DBL_MAX/Math.Log(DBL_MAX);
			}
  
			Complex z = zin;
			double x = zin.Re, 
				y = zin.Im;
  
			double cabsz = ComplexMath.Abs(z);

			if (cabsz > _LnGamma_rmax) 
				throw new ArgumentException("z so big LnGamma(z) overflows");	 

			int n;
			double argsum;
			Complex corr = new Complex(0.0, 0.0);

			if (x >= 0.0 && cabsz  > _LnGamma_bound) goto L50;
			if (x <  0.0 && Math.Abs(y) > _LnGamma_bound) goto L50;
				    
			if (cabsz < _LnGamma_bound) goto L20;

			// use the reflection formula for real(z) negative, 
			// abs(z) large, and abs(imag(y)) small.
			if (y > 0.0) z = z.GetConjugate();
			corr = ComplexMath.Exp(-Complex.FromRealImaginary(0.0,2.0*Math.PI)*z);

			if (corr.Re == 1.0 && corr.Im == 0.0) 
				throw new ArgumentException("z is a negative integer");

			retval = sq2pil + 1.0 - Complex.FromRealImaginary(0.0,Math.PI)*(z-0.5) - LogRel(-corr)
				+ (z-0.5) * ComplexMath.Log(1.0-z) - z - c9lgmc(1.0-z);
			if (y > 0.0) retval = retval.GetConjugate();

			if (Math.Abs(y) > _LnGamma_dxrel) return retval;

			if (0.5 * ComplexMath.Abs((1.0-corr)*retval/z) < _LnGamma_dxrel) 
			{
				System.Diagnostics.Trace.WriteLine("Warning (LnGamma): answer less than half precision because z too near negative integer");
				return retval;
			}

			// Use the recursion relation for cabs(z) small

			L20:
				if (x >= -0.5 || Math.Abs(y) > _LnGamma_dxrel) goto L30;

			if (ComplexMath.Abs( (z-(double)((int)(x-0.5))) / x) < _LnGamma_dxrel)    
				System.Diagnostics.Trace.WriteLine("Warning (LnGamma): answer less than half precision because z too near negative integer"); 
			L30:
				n = (int)(Math.Sqrt(_LnGamma_bound*_LnGamma_bound - y*y) - x + 1.0);
			argsum = 0.0;
			corr = Complex.FromRealImaginary(1.0, 0.0);
			for (int i = 1; i <= n; i++) 
			{
				argsum += z.GetArgument();
				corr *= z;
				z += 1.0;
			}

			if (corr.Re == 0.0 && corr.Im == 0.0) 
				throw new ArgumentException("z is a negative integer");
  
			corr = -Complex.FromRealImaginary(Math.Log(ComplexMath.Abs(corr)), argsum);

			// Use Stirling's approximation for large z

			L50:
				retval = sq2pil + (z-0.5)*ComplexMath.Log(z) - z + corr + c9lgmc(z);
			return retval;
		}

		#endregion

	} // end of class GammaRelated
} // end of namespace
