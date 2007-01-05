#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

// The following code was translated using Matpack sources (http://www.matpack.de) (Author B.Gammel)
// Originalfile Matpack-1.7.3\source\function\derf.cc
//                                            derf.cc
//                                            initds.cc
//                                            mpspecfunp.h
//                                            common.h

  /*-----------------------------------------------------------------------------*\
  | Matpack special functions - Erf(x) error function                     derf.cc |
  |                                                                               |
  | Last change: Feb 1, 2002              |
  |                                                                               |
  | Matpack Library Release 1.7.3                                                 |
  | Copyright (C) 1991-2001 by Berndt M. Gammel. All rights reserved.             |
  |                                                                               |
  | Permission to  use, copy, and  distribute  Matpack  in  its entirety  and its |
  | documentation  for non-commercial purpose and  without fee is hereby granted, |
  | provided that this license information and copyright notice appear unmodified |
  | in all copies.  This software is provided 'as is'  without express or implied |
  | warranty.  In no event will the author be held liable for any damages arising |
  | from the use of this software.            |
  | Note that distributing Matpack 'bundled' in with any product is considered to |
  | be a 'commercial purpose'.              |
  | The software may be modified for your own purposes, but modified versions may |
  | not be distributed without prior consent of the author.     |
  |                                                                               |
  | Read the  COPYRIGHT and  README files in this distribution about registration |
  | and installation of Matpack.              |
  |                                                                               |
  \*-----------------------------------------------------------------------------*/



using System;


namespace Altaxo.Calc
{
  /// <summary>
  /// Hosts the direct and the complementary error function.
  /// </summary>
  public class ErrorFunction
  {
    #region Constants

    /// <summary>
    /// Represents the smallest number where 1+DBL_EPSILON is not equal to 1.
    /// </summary>
    const double DBL_EPSILON = 2.2204460492503131e-016;
    /// <summary>
    /// The smallest positive double number.
    /// </summary>
    const double DBL_MIN     = double.Epsilon;
    /// <summary>
    /// The biggest positive double number.
    /// </summary>
    const double DBL_MAX     = double.MaxValue;


    /// <summary>
    /// The value 2/sqrt(Pi).
    /// </summary>
    const double M_2_SQRTPI = 1.1283791670955125738961589031216;


    /// <summary>
    /// Square root of Pi.
    /// </summary>
    const double sqrtpi = 1.77245385090551602729816748334115;

    /// <summary>
    /// Square root of Epsilon.
    /// </summary>
    private static readonly double sqeps  = Math.Sqrt(DBL_EPSILON);

 
   

    #endregion

    #region Helper functions

    /// <summary>
    /// Returns -1 if argument negative, 0 if argument zero, or 1 if argument is positive.
    /// </summary>
    /// <param name="x">The number whose sign is returned.</param>
    /// <returns>-1 if the argument is negative, 0 if the argument is zero, or 1 if argument is positive.</returns>
    static int sign (double x)
    {
      return (x > 0) ? 1 : (x < 0) ? -1 : 0;
    }

    /// <summary>
    /// Return first number with sign of second number
    /// </summary>
    /// <param name="x">The first number.</param>
    /// <param name="y">The second number whose sign is used.</param>
    /// <returns>The first number x with the sign of the second argument y.</returns>
    static double CopySign (double x, double y)
    {
      return (y < 0) ? ((x < 0) ? x : -x) : ((x > 0) ? x : -x);
    }


    /// <summary>
    /// Round to nearest integer.
    /// </summary>
    /// <param name="d">The argument.</param>
    /// <returns>The nearest integer of the argument d.</returns>
    static int Nint (double d)
    {
      return (d>0) ? (int)(d+0.5) : -(int)(-d+0.5);
    }


    #endregion

    #region initds

    /// <summary>
    /// Initialize the orthogonal series, represented by the array os, so 
    /// that initds is the number of terms needed to insure the error is no 
    /// larger than eta.  Ordinarily, eta will be chosen to be one-tenth 
    /// machine precision. 
    /// </summary>
    /// <param name="os">Double precision array of NOS coefficients in an orthogonal  series.</param>
    /// <param name="nos">Number of coefficients in OS.</param>
    /// <param name="eta"> single precision scalar containing requested accuracy of  series. </param>
    /// <returns>The number of terms neccessary to insure the error is not larger than eta.</returns>
    /// <remarks>
    /// This is a translation from the Fortran version of SLATEC, FNLIB,
    /// CATEGORY C3A2, REVISION 900315, originally written by Fullerton W., (LANL)
    /// to C++.
    /// </remarks>
    static int initds (double[] os, int nos, double eta)
    {
      if (nos < 1) 
        throw new ArgumentException("Number of coefficients is less than 1");

      int i = 0;
      double err = 0.0;
      for (int ii = 1; ii <= nos; ii++) 
      {
        i = nos - ii;
        err += Math.Abs(os[i]);
        if (err > eta) break;
      }
    
      if (i == nos) 
        throw new ArgumentException("Chebyshev series too short for specified accuracy");

      return i;
    }

    #endregion

    #region dcsevl

    /// <summary>
    /// Evaluate the n-term Chebyshev series cs at x.  Adapted from 
    /// a method presented in the paper by Broucke referenced below. 
    /// </summary>
    /// <param name="x">Value at which the series is to be evaluated. </param>
    /// <param name="cs">cs   array of n terms of a Chebyshev series. In evaluating 
    /// cs, only half the first coefficient is summed. 
    /// </param>
    /// <param name="n">number of terms in array cs.</param>
    /// <returns>The n-term Chebyshev series cs at x.</returns>
    /// <remarks>
    /// References:
    ///
    /// R. Broucke, Ten subroutines for the manipulation of Chebyshev series, 
    /// Algorithm 446, Communications of the A.C.M. 16, (1973) pp. 254-256. 
    ///
    /// L. Fox and I. B. Parker, Chebyshev Polynomials in 
    ///      Numerical Analysis, Oxford University Press, 1968,  page 56. 
    ///
    /// This is a translation from the Fortran version of SLATEC, FNLIB,
    /// CATEGORY C3A2, REVISION  920501, originally written by Fullerton W., (LANL) 
    /// to C++.
    /// </remarks>
    static double dcsevl (double x, double[] cs, int n)
    {
#if DEBUG
      if (n < 1)   
        throw new ArgumentException("Number of terms <= 0");
      if (n > 1000) 
        throw new ArgumentException("Number of terms > 1000");
      if (Math.Abs(x) > DBL_EPSILON + 1.0) 
        throw new ArgumentException("X outside the interval (-1,+1)");
#endif

      double b0 = 0.0, b1 = 0.0, b2 = 0.0, twox = x * 2;
      for (int i = 1; i <= n; i++) 
      {
        b2 = b1;
        b1 = b0;
        b0 = twox * b1 - b2 + cs[n - i];
      }

      return (b0 - b2) * 0.5;
    } 
 
    #endregion

    #region Erf function

    /// <summary>
    /// Erf(x) calculates the double precision error function for double 
    /// precision argument x. 
    /// </summary>
    /// <param name="x">The argument x.</param>
    /// <returns>The error function value of the argument x.</returns>
    /// <remarks><code>
    /// This is a translation from the Fortran version of SLATEC, FNLIB,
    /// CATEGORY C8A, L5A1E, REVISION 920618, originally written by Fullerton W.,(LANL)
    /// to C++.
    ///
    /// Series for erf        on the interval  0.          to  1.00000E+00 
    ///                        with weighted error   1.28E-32 
    ///                        log weighted error  31.89 
    ///                        significant figures required  31.05 
    ///                        decimal places required  32.55 
    /// </code></remarks>
    public static double Erf(double x)
    {
      return _Erf.Erf(x, false);
    }

    /// <summary>
    /// Erf(x) calculates the double precision error function for double 
    /// precision argument x. 
    /// </summary>
    /// <param name="x">The argument x.</param>
    /// <param name="bDebug">If true, an exception is thrown if serious errors occur. If false, NaN is returned on errors.</param>
    /// <returns>The error function value of the argument x.</returns>
    /// <remarks><code>
    /// This is a translation from the Fortran version of SLATEC, FNLIB,
    /// CATEGORY C8A, L5A1E, REVISION 920618, originally written by Fullerton W.,(LANL)
    /// to C++.
    ///
    /// Series for erf        on the interval  0.          to  1.00000E+00 
    ///                        with weighted error   1.28E-32 
    ///                        log weighted error  31.89 
    ///                        significant figures required  31.05 
    ///                        decimal places required  32.55 
    /// </code></remarks>
    public static double Erf(double x, bool bDebug)
    {
      return _Erf.Erf(x, bDebug);
    }

    class _Erf
    {
      /// <summary>
      /// Coefficient array for Erf function
      /// </summary>
      static readonly double[] erfcs = 
  { 
    -0.049046121234691808039984544033376,
    -0.14226120510371364237824741899631,
    0.010035582187599795575754676712933,
    -5.7687646997674847650827025509167e-4,
    2.7419931252196061034422160791471e-5,
    -1.1043175507344507604135381295905e-6,
    3.8488755420345036949961311498174e-8,
    -1.1808582533875466969631751801581e-9,
    3.2334215826050909646402930953354e-11,
    -7.9910159470045487581607374708595e-13,
    1.7990725113961455611967245486634e-14,
    -3.7186354878186926382316828209493e-16,
    7.1035990037142529711689908394666e-18,
    -1.2612455119155225832495424853333e-19,
    2.0916406941769294369170500266666e-21,
    -3.253973102931407298236416e-23,
    4.7668672097976748332373333333333e-25,
    -6.5980120782851343155199999999999e-27,
    8.6550114699637626197333333333333e-29,
    -1.0788925177498064213333333333333e-30,
    1.2811883993017002666666666666666e-32 
  };

      // used for Erf
      static readonly double xbig   = Math.Sqrt(-Math.Log(sqrtpi * 0.5*DBL_EPSILON ));

      /// <summary>
      /// Number of terms for the Erf function
      /// </summary>
      static int  nterf = initds(erfcs, 21, 0.5 * DBL_EPSILON * 0.1);



      /// <summary>
      /// Erf(x) calculates the double precision error function for double 
      /// precision argument x. 
      /// </summary>
      /// <param name="x">The argument x.</param>
      /// <param name="bDebug">If true, an exception is thrown if serious errors occur. If false, NaN is returned on errors.</param>
      /// <returns>The error function value of the argument x.</returns>
      /// <remarks>
      /// This is a translation from the Fortran version of SLATEC, FNLIB,
      /// CATEGORY C8A, L5A1E, REVISION 920618, originally written by Fullerton W.,(LANL)
      /// to C++.
      ///
      /// Series for erf        on the interval  0.          to  1.00000E+00 
      ///                        with weighted error   1.28E-32 
      ///                        log weighted error  31.89 
      ///                        significant figures required  31.05 
      ///                        decimal places required  32.55 
      /// </remarks>
      public static double Erf(double x, bool bDebug)
      {



      
        double y = Math.Abs(x);
        if (y > 1.0) goto L20;

        // erf(x) = 1.0 - erfc(x)  for  -1.0 <= x <= 1.0 

        if (y <= sqeps) 
          return x * 2.0 * x / sqrtpi;
        else // if (y > sqeps) 
          return x * ( dcsevl(x * 2.0 * x - 1.0, erfcs, nterf) + 1.0 );

        // erf(x) = 1.0 - erfc(x) for abs(x) > 1.0

        L20:
          if (y <= xbig) 
            return CopySign(1.0 - Erfc(y), x);
          else // if (y > xbig)  
            return sign(x);
      } // Function
    }
    #endregion

    #region Erfc function


    /// <summary>
    /// Erfc(x) calculates the double precision complementary error function 
    /// for double precision argument x.
    /// </summary>
    /// <param name="x">The argument x.</param>
    /// <returns>The complementary error function of the argument x.</returns>
    public static double Erfc(double x)
    {
      return _Erfc.Erfc(x, false);
    }

    /// <summary>
    /// Erfc(x) calculates the double precision complementary error function 
    /// for double precision argument x.
    /// </summary>
    /// <param name="x">The argument x.</param>
    /// <param name="bDebug">If true, an exception is thrown if serious errors occur. If false, NaN is returned on errors.</param>
    /// <returns>The complementary error function of the argument x.</returns>
    public static double Erfc(double x, bool bDebug)
    {
      return _Erfc.Erfc(x, bDebug);
    }

    class _Erfc
    {
    
      /// <summary>
      /// Coefficient array for Erf function
      /// </summary>
      static readonly double[] erfcs = 
  { 
    -0.049046121234691808039984544033376,
    -0.14226120510371364237824741899631,
    0.010035582187599795575754676712933,
    -5.7687646997674847650827025509167e-4,
    2.7419931252196061034422160791471e-5,
    -1.1043175507344507604135381295905e-6,
    3.8488755420345036949961311498174e-8,
    -1.1808582533875466969631751801581e-9,
    3.2334215826050909646402930953354e-11,
    -7.9910159470045487581607374708595e-13,
    1.7990725113961455611967245486634e-14,
    -3.7186354878186926382316828209493e-16,
    7.1035990037142529711689908394666e-18,
    -1.2612455119155225832495424853333e-19,
    2.0916406941769294369170500266666e-21,
    -3.253973102931407298236416e-23,
    4.7668672097976748332373333333333e-25,
    -6.5980120782851343155199999999999e-27,
    8.6550114699637626197333333333333e-29,
    -1.0788925177498064213333333333333e-30,
    1.2811883993017002666666666666666e-32 
  };


      static readonly double[] erc2cs = 
  { 
    -0.06960134660230950112739150826197,
    -0.04110133936262089348982212084666,
    0.003914495866689626881561143705244,
    -4.906395650548979161280935450774e-4,
    7.157479001377036380760894141825e-5,
    -1.153071634131232833808232847912e-5,
    1.994670590201997635052314867709e-6,
    -3.642666471599222873936118430711e-7,
    6.944372610005012589931277214633e-8,
    -1.37122090210436601953460514121e-8,
    2.788389661007137131963860348087e-9,
    -5.814164724331161551864791050316e-10,
    1.23892049175275318118016881795e-10,
    -2.690639145306743432390424937889e-11,
    5.94261435084791098244470968384e-12,
    -1.33238673575811957928775442057e-12,
    3.028046806177132017173697243304e-13,
    -6.966648814941032588795867588954e-14,
    1.620854541053922969812893227628e-14,
    -3.809934465250491999876913057729e-15,
    9.040487815978831149368971012975e-16,
    -2.164006195089607347809812047003e-16,
    5.222102233995854984607980244172e-17,
    -1.26972960236455533637241552778e-17,
    3.109145504276197583836227412951e-18,
    -7.663762920320385524009566714811e-19,
    1.90081925136274520253692973329e-19,
    -4.742207279069039545225655999965e-20,
    1.189649200076528382880683078451e-20,
    -3.000035590325780256845271313066e-21,
    7.602993453043246173019385277098e-22,
    -1.93590944760687288156981104913e-22,
    4.951399124773337881000042386773e-23,
    -1.271807481336371879608621989888e-23,
    3.280049600469513043315841652053e-24,
    -8.492320176822896568924792422399e-25,
    2.206917892807560223519879987199e-25,
    -5.755617245696528498312819507199e-26,
    1.506191533639234250354144051199e-26,
    -3.954502959018796953104285695999e-27,
    1.041529704151500979984645051733e-27,
    -2.751487795278765079450178901333e-28,
    7.29005820549755740899770368e-29,
    -1.936939645915947804077501098666e-29,
    5.160357112051487298370054826666e-30,
    -1.3784193221930940993896448e-30,
    3.691326793107069042251093333333e-31,
    -9.909389590624365420653226666666e-32,
    2.666491705195388413323946666666e-32 
  };

      static readonly double[] erfccs = 
  { 
    0.0715179310202924774503697709496,
    -0.0265324343376067157558893386681,
    0.00171115397792085588332699194606,
    -1.63751663458517884163746404749e-4,
    1.98712935005520364995974806758e-5,
    -2.84371241276655508750175183152e-6,
    4.60616130896313036969379968464e-7,
    -8.22775302587920842057766536366e-8,
    1.59214187277090112989358340826e-8,
    -3.29507136225284321486631665072e-9,
    7.2234397604005554658126115389e-10,
    -1.66485581339872959344695966886e-10,
    4.01039258823766482077671768814e-11,
    -1.00481621442573113272170176283e-11,
    2.60827591330033380859341009439e-12,
    -6.99111056040402486557697812476e-13,
    1.92949233326170708624205749803e-13,
    -5.47013118875433106490125085271e-14,
    1.58966330976269744839084032762e-14,
    -4.7268939801975548392036958429e-15,
    1.4358733767849847867287399784e-15,
    -4.44951056181735839417250062829e-16,
    1.40481088476823343737305537466e-16,
    -4.51381838776421089625963281623e-17,
    1.47452154104513307787018713262e-17,
    -4.89262140694577615436841552532e-18,
    1.64761214141064673895301522827e-18,
    -5.62681717632940809299928521323e-19,
    1.94744338223207851429197867821e-19,
    -6.82630564294842072956664144723e-20,
    2.42198888729864924018301125438e-20,
    -8.69341413350307042563800861857e-21,
    3.15518034622808557122363401262e-21,
    -1.15737232404960874261239486742e-21,
    4.28894716160565394623737097442e-22,
    -1.60503074205761685005737770964e-22,
    6.06329875745380264495069923027e-23,
    -2.31140425169795849098840801367e-23,
    8.88877854066188552554702955697e-24,
    -3.44726057665137652230718495566e-24,
    1.34786546020696506827582774181e-24,
    -5.31179407112502173645873201807e-25,
    2.10934105861978316828954734537e-25,
    -8.43836558792378911598133256738e-26,
    3.39998252494520890627359576337e-26,
    -1.3794523880732420900223837711e-26,
    5.63449031183325261513392634811e-27,
    -2.316490434477065448234277527e-27,
    9.58446284460181015263158381226e-28,
    -3.99072288033010972624224850193e-28,
    1.67212922594447736017228709669e-28,
    -7.04599152276601385638803782587e-29,
    2.97976840286420635412357989444e-29,
    -1.26252246646061929722422632994e-29,
    5.39543870454248793985299653154e-30,
    -2.38099288253145918675346190062e-30,
    1.0990528301027615735972668375e-30,
    -4.86771374164496572732518677435e-31,
    1.52587726411035756763200828211e-31 
  };

      // used for Erfc
      static readonly double eta    = 0.5 * DBL_EPSILON * 0.1;
      static readonly double xsml   = -Math.Sqrt(-Math.Log(sqrtpi * 0.5 * DBL_EPSILON));
      static readonly double txmax  = Math.Sqrt(-Math.Log(sqrtpi * DBL_MIN));
      static readonly double xmax   = txmax - Math.Log(txmax) * 0.5 / txmax - 0.01;


      static readonly int nterf  = initds(erfcs, 21, eta);
      static readonly int nterfc = initds(erfccs, 59, eta);
      static readonly int nterc2 = initds(erc2cs, 49, eta);
    

      /// <summary>
      /// Erfc(x) calculates the double precision complementary error function 
      /// for double precision argument x.
      /// </summary>
      /// <param name="x">The argument x.</param>
      /// <param name="bDebug">If true, an exception is thrown if serious errors occur. If false, NaN is returned on errors.</param>
      /// <returns>The complementary error function of the argument x.</returns>
      /// <remarks><code>
      /// This is a translation from the Fortran version of SLATEC, FNLIB,
      /// CATEGORY C8A, L5A1E, REVISION 920618, originally written by Fullerton W.,(LANL)
      /// to C++.
      ///
      /// Series for erf        on the interval  0.0          to  1.00000E+00 
      ///                                        with weighted Error   1.28E-32 
      ///                                         log weighted Error  31.89 
      ///                               significant figures required  31.05 
      ///                                    decimal places required  32.55 
      ///
      /// Series for erc2       on the interval  2.50000E-01 to  1.00000E+00 
      ///                                        with weighted Error   2.67E-32 
      /// 
      ///                                         log weighted Error  31.57 
      ///                               significant figures required  30.31 
      ///                                    decimal places required  32.42 
      ///
      /// Series for erfc       on the interval  0.0          to  2.50000E-01 
      ///                                        with weighted error   1.53E-31 
      /// 
      ///                                         log weighted error  30.82 
      ///                               significant figures required  29.47 
      ///                                    decimal places required  31.70 
      /// </code></remarks>
      public static double Erfc(double x, bool bDebug)
      {
    

        double ret_val, y;

        if (x > xsml) goto L20;
    
        // erfc(x) = 1.0 - erf(x)  for  x < xsml 

        return 2.0;

        L20:
          if (x > xmax) goto L40;
   
        y = Math.Abs(x);
        if (y > 1.0) goto L30;

        // erfc(x) = 1.0 - erf(x)  for abs(x) <= 1.0 

        if (y < sqeps) 
          return (1.0 - x * 2.0 / sqrtpi);
        else // if (y >= sqeps) 
          return (1.0 - x * (dcsevl(x * 2.0 * x - 1.0, erfcs, nterf) + 1.0));

        // erfc(x) = 1.0 - erf(x)  for  1.0 < abs(x) <= xmax 

        L30:
          y *= y;
        if (y <= 4.0) 
          ret_val = Math.Exp(-y) / Math.Abs(x) 
            * (dcsevl( (8.0 / y - 5.0) / 3.0, erc2cs, nterc2) + 0.5);
        else // if (y > 4.0) 
          ret_val = Math.Exp(-y) / Math.Abs(x) 
            * (dcsevl(8.0 / y - 1.0, erfccs, nterfc) + 0.5);

        if (x < 0.0) 
          ret_val = 2.0 - ret_val;
    
        return ret_val;

        L40:
          if(bDebug)
            System.Diagnostics.Trace.WriteLine("Erfc: x so big that Erfc(x) underflows");
        return 0.0;
      }
    }

    #endregion

    #region Inverse Erf

    /// <summary>
    /// Quantile of the normal distribution function NormalDistribution[0,1].
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>Adopted from Cephes library; Cephes name: ndtri</para>
    /// </remarks>
    public static double QuantileOfNormalDistribution01(double y)
    {
      return _Cephes.ndtri(y);
    }

    /// <summary>
    /// Inverse of the error function Erf(x).
    /// </summary>
    /// <param name="y">Argument.</param>
    /// <returns>A value x so that Erf(x)==y.</returns>
    public static double InverseErf(double y)
    {
      const double oneBySqrt2 = 0.7071067811865475244008444;
      return oneBySqrt2 * _Cephes.ndtri(0.5*(y+1));
    }

    class _Cephes
    {
      /*              polevl.c
       *              p1evl.c
       *
       *  Evaluate polynomial
       *
       *
       *
       * SYNOPSIS:
       *
       * int N;
       * double x, y, coef[N+1], polevl[];
       *
       * y = polevl( x, coef, N );
       *
       *
       *
       * DESCRIPTION:
       *
       * Evaluates polynomial of degree N:
       *
       *                     2          N
       * y  =  C  + C x + C x  +...+ C x
       *        0    1     2          N
       *
       * Coefficients are stored in reverse order:
       *
       * coef[0] = C  , ..., coef[N] = C  .
       *            N                   0
       *
       *  The function p1evl() assumes that coef[N] = 1.0 and is
       * omitted from the array.  Its calling arguments are
       * otherwise the same as polevl().
       *
       *
       * SPEED:
       *
       * In the interest of speed, there are no checks for out
       * of bounds arithmetic.  This routine is used by most of
       * the functions in the library.  Depending on available
       * equipment features, the user may wish to rewrite the
       * program in microcode or assembly language.
       *
       */


      /*
      Cephes Math Library Release 2.1:  December, 1988
      Copyright 1984, 1987, 1988 by Stephen L. Moshier
      Direct inquiries to 30 Frost Street, Cambridge, MA 02140
      */

      //#include "cephes.h"

      public static double polevl(double x, double[] coef, int N )
      {
        double ans;
        int i;
        int j=0;
        double[] p;

        p =  coef;
        ans = p[j++];
        i = N;

        do
          ans = ans * x  +  p[j++];
        while( (--i)>0 );

        return( ans );
      }

      /*              p1evl() */
      /*                                          N
       * Evaluate polynomial when coefficient of x  is 1.0.
       * Otherwise same as polevl.
       */

      public static double p1evl(double x,double[] coef,int N )
      {
        double ans;
        double[] p;
        int i;
        int j=0;

        p = coef;
        ans = x + p[j++];
        i = N-1;

        do
          ans = ans * x  + p[j++];
        while( (--i)>0 );

        return( ans );
      } 






      /*              ndtri.c
 *
 *  Inverse of Normal distribution function
 *
 *
 *
 * SYNOPSIS:
 *
 * double x, y, ndtri();
 *
 * x = ndtri( y );
 *
 *
 *
 * DESCRIPTION:
 *
 * Returns the argument, x, for which the area under the
 * Gaussian probability density function (integrated from
 * minus infinity to x) is equal to y.
 *
 *
 * For small arguments 0 < y < exp(-2), the program computes
 * z = sqrt( -2.0 * log(y) );  then the approximation is
 * x = z - log(z)/z  - (1/z) P(1/z) / Q(1/z).
 * There are two rational functions P/Q, one for 0 < y < exp(-32)
 * and the other for y up to exp(-2).  For larger arguments,
 * w = y - 0.5, and  x/sqrt(2pi) = w + w**3 R(w**2)/S(w**2)).
 *
 *
 * ACCURACY:
 *
 *                      Relative error:
 * arithmetic   domain        # trials      peak         rms
 *    DEC      0.125, 1         5500       9.5e-17     2.1e-17
 *    DEC      6e-39, 0.135     3500       5.7e-17     1.3e-17
 *    IEEE     0.125, 1        20000       7.2e-16     1.3e-16
 *    IEEE     3e-308, 0.135   50000       4.6e-16     9.8e-17
 *
 *
 * ERROR MESSAGES:
 *
 *   message         condition    value returned
 * ndtri domain       x <= 0        -MAXNUM
 * ndtri domain       x >= 1         MAXNUM
 *
 */


      /*
      Cephes Math Library Release 2.1:  January, 1989
      Copyright 1984, 1987, 1989 by Stephen L. Moshier
      Direct inquiries to 30 Frost Street, Cambridge, MA 02140
      */

      //#include <math.h>
      //#include "mconf.h"
      //#include "cephes.h"

      // static double MAXNUM = double.NaN;

      /* sqrt(2pi) */
      static double s2pi = 2.50662827463100050242E0;

      /* approximation for 0 <= |y - 0.5| <= 3/8 */
      static readonly double[] P0 = new double[]
{
  -5.99633501014107895267E1,
  9.80010754185999661536E1,
  -5.66762857469070293439E1,
  1.39312609387279679503E1,
  -1.23916583867381258016E0,
      };
      static readonly double[] Q0 = new double[]
{
  /* 1.00000000000000000000E0,*/
  1.95448858338141759834E0,
  4.67627912898881538453E0,
  8.63602421390890590575E1,
  -2.25462687854119370527E2,
  2.00260212380060660359E2,
  -8.20372256168333339912E1,
  1.59056225126211695515E1,
  -1.18331621121330003142E0,
      };

      /* Approximation for interval z = sqrt(-2 log y ) between 2 and 8
       * i.e., y between exp(-2) = .135 and exp(-32) = 1.27e-14.
       */
      static readonly double[] P1 = new double[] 
{
  4.05544892305962419923E0,
  3.15251094599893866154E1,
  5.71628192246421288162E1,
  4.40805073893200834700E1,
  1.46849561928858024014E1,
  2.18663306850790267539E0,
  -1.40256079171354495875E-1,
  -3.50424626827848203418E-2,
  -8.57456785154685413611E-4,
      };
      static readonly double[] Q1 = new double[] 
{
  /*  1.00000000000000000000E0,*/
  1.57799883256466749731E1,
  4.53907635128879210584E1,
  4.13172038254672030440E1,
  1.50425385692907503408E1,
  2.50464946208309415979E0,
  -1.42182922854787788574E-1,
  -3.80806407691578277194E-2,
  -9.33259480895457427372E-4,
      };

      /* Approximation for interval z = sqrt(-2 log y ) between 8 and 64
       * i.e., y between exp(-32) = 1.27e-14 and exp(-2048) = 3.67e-890.
       */

      static readonly double[] P2 = new double[] 
{
  3.23774891776946035970E0,
  6.91522889068984211695E0,
  3.93881025292474443415E0,
  1.33303460815807542389E0,
  2.01485389549179081538E-1,
  1.23716634817820021358E-2,
  3.01581553508235416007E-4,
  2.65806974686737550832E-6,
  6.23974539184983293730E-9,
      };
      static readonly double[] Q2 = new double[]
{
  /*  1.00000000000000000000E0,*/
  6.02427039364742014255E0,
  3.67983563856160859403E0,
  1.37702099489081330271E0,
  2.16236993594496635890E-1,
  1.34204006088543189037E-2,
  3.28014464682127739104E-4,
  2.89247864745380683936E-6,
  6.79019408009981274425E-9,
      };

      public static double ndtri(double y0)
      {
        double x, y, z, y2, x0, x1;
        int code;

        if( y0 <= 0.0 )
        {
          throw new ArgumentException("y0 has to be positive" );
          // return( double.NegativeInfinity );
        }
        if( y0 >= 1.0 )
        {
          throw new ArgumentException("y0 has to be <1" );
          // return( double.PositiveInfinity);
        }
        code = 1;
        y = y0;
        if( y > (1.0 - 0.13533528323661269189) ) /* 0.135... = exp(-2) */
        {
          y = 1.0 - y;
          code = 0;
        }

        if( y > 0.13533528323661269189 )
        {
          y = y - 0.5;
          y2 = y * y;
          x = y + y * (y2 * polevl( y2, P0, 4)/p1evl( y2, Q0, 8 ));
          x = x * s2pi; 
          return(x);
        }

        x = Math.Sqrt( -2.0 * Math.Log(y) );
        x0 = x - Math.Log(x)/x;

        z = 1.0/x;
        if( x < 8.0 ) /* y > exp(-32) = 1.2664165549e-14 */
          x1 = z * polevl( z, P1, 8 )/p1evl( z, Q1, 8 );
        else
          x1 = z * polevl( z, P2, 8 )/p1evl( z, Q2, 8 );
        x = x0 - x1;
        if( code != 0 )
          x = -x;
        return( x );
      } 
    }
    #endregion

    #region Dawson

    /// <summary>
    /// Dawson(x) evaluates Dawson's integral for a double precision real argument x. 
    /// <code>
    ///                       2  / x   2 
    ///                     -x   |    t 
    ///             F(x) = e     |   e    dt 
    ///                          | 
    ///                          / 0 
    /// </code>
    /// </summary>
    /// <param name="x">The function argument</param>
    /// <returns>Dawson's integral for argument x.</returns>
    public static double Dawson(double x)
    {
      return _Dawson.Dawson(x, false);
    }

    /// <summary>
    /// Dawson(x) evaluates Dawson's integral for a double precision real argument x. 
    /// <code>
    ///                       2  / x   2 
    ///                     -x   |    t 
    ///             F(x) = e     |   e    dt 
    ///                          | 
    ///                          / 0 
    /// </code>
    /// </summary>
    /// <param name="x">The function argument</param>
    /// <param name="bDebug">If true, an exception is thrown if serious errors occur. If false, NaN is returned on errors.</param>
    /// <returns>Dawson's integral for argument x.</returns>
    public static double Dawson(double x, bool bDebug)
    {
      return _Dawson.Dawson(x, bDebug);
    }

    class _Dawson
    {
      private static readonly double zero = 0.0;
      private static readonly double xsmall = Math.Sqrt(0.5*DBL_EPSILON);
      private static readonly double xlarge = 1.0/Math.Sqrt(0.5*DBL_EPSILON);
      private static readonly double xmax_Dawson   = Math.Min(0.5/DBL_MIN,DBL_MAX);

      //----------------------------------------------------------------------
      // Coefficients for R(9,9) approximation for  |x| < 2.5
      //----------------------------------------------------------------------
      private static double[] p1 = 
  { 
    -2.6902039878870478241e-12, 4.18572065374337710778e-10,
    -1.34848304455939419963e-8, 9.28264872583444852976e-7,
    -1.23877783329049120592e-5, 4.07205792429155826266e-4,
    -0.00284388121441008500446, 0.0470139022887204722217,
    -0.138868086253931995101,   1.00000000000000000004 
  };
    
      private static double[] q1 = 
  { 
    1.71257170854690554214e-10, 1.19266846372297253797e-8,
    4.32287827678631772231e-7,  1.03867633767414421898e-5,
    1.7891096528424624934e-4,   0.00226061077235076703171,
    0.0207422774641447644725,   0.132212955897210128811,
    0.527798580412734677256,    1.0 };

      //----------------------------------------------------------------------
      // Coefficients for R(9,9) approximation in J-fraction form
      // for  x in [2.5, 3.5)
      //----------------------------------------------------------------------
    
      private static double[] p2 = 
  { 
    -1.7095380470085549493,  -37.9258977271042880786,
    26.1935631268825992835,   12.5808703738951251885,
    -22.7571829525075891337,    4.56604250725163310122,
    -7.3308008989640287075,   46.5842087940015295573,
    -17.3717177843672791149,    0.500260183622027967838 
  };

      private static double[] q2 = 
  { 
    1.82180093313514478378, 1100.67081034515532891,
    -7.08465686676573000364,  453.642111102577727153,
    40.6209742218935689922,   302.890110610122663923,
    170.641269745236227356,    951.190923960381458747,
    0.206522691539642105009 
  };

      //----------------------------------------------------------------------
      // Coefficients for R(9,9) approximation in J-fraction form
      // for  x in [3.5, 5.0]
      //----------------------------------------------------------------------

      private static double[] p3 = 
  { 
    -4.55169503255094815112,  -18.6647123338493852582,
    -7.36315669126830526754,  -66.8407240337696756838,
    48.450726508149145213,     26.9790586735467649969,
    -33.5044149820592449072,     7.50964459838919612289,
    -1.48432341823343965307,    0.499999810924858824981 
  };

      private static double[] q3 = 
  { 
    44.7820908025971749852,    99.8607198039452081913,
    14.0238373126149385228,  3488.17758822286353588,
    -9.18871385293215873406, 1240.18500009917163023,
    -68.8024952504512254535,    -2.3125157538514514307,
    0.250041492369922381761 
  };

      //----------------------------------------------------------------------
      // Coefficients for R(9,9) approximation in J-fraction form
      // for  |x| > 5.0
      //----------------------------------------------------------------------
 
      private static double[] p4 = 
  { 
    -8.11753647558432685797,  -38.404388247745445343,
    -22.3787669028751886675,   -28.8301992467056105854,
    -5.99085540418222002197,  -11.3867365736066102577,
    -6.5282872752698074159,    -4.50002293000355585708,
    -2.50000000088955834952,    0.5000000000000004884 
  };

      private static double[] q4 = 
  { 
    269.382300417238816428,     50.4198958742465752861,
    61.1539671480115846173,   208.210246935564547889,
    19.7325365692316183531,   -12.2097010558934838708,
    -6.99732735041547247161,   -2.49999970104184464568,
    0.749999999999027092188 
  };



      /// <summary>
      /// Dawson(x) evaluates Dawson's integral for a double precision real argument x. 
      /// <code>
      ///                       2  / x   2 
      ///                     -x   |    t 
      ///             F(x) = e     |   e    dt 
      ///                          | 
      ///                          / 0 
      /// </code>
      /// </summary>
      /// <param name="x">The function argument.</param>
      /// <param name="bDebug">If true, an exception is thrown if serious errors occur. If false, NaN is returned on errors.</param>
      /// <returns></returns>
      /// <remarks><code>
      /// The main computation uses rational Chebyshev approximations 
      /// published in Math. Comp. 24, 171-178 (1970) by Cody, Paciorek 
      /// and Thacher.  This transportable program is patterned after the 
      /// machine-dependent FUNPACK program DDAW(X), but cannot match that 
      /// version for efficiency or accuracy.  This version uses rational 
      /// approximations that are theoretically accurate to about 19 
      /// significant decimal digits.  The accuracy achieved depends on the 
      /// arithmetic system, the compiler, the intrinsic functions, and 
      /// proper selection of the machine-dependent constants. 
      ///
      /// Underflow: The program returns 0.0 for |X| > XMAX. 
      ///
      /// This is a translation from the Fortran version of a SPECFUN (NETLIB)
      /// REVISION June 15, 1988 , originally written by W. J. Cody, (Mathematics 
      /// and Computer Science Division, Argonne National Laboratory, Argonne, 
      /// IL 60439) to C++. 
      ///
      /// Explanation of machine-dependent constants:
      ///
      ///   XINF   = largest positive machine number 
      ///            (ANSI C: DBL_MAX)
      ///   XMIN   = the smallest positive machine number. 
      ///            (ANSI C: DBL_MIN)
      ///   EPS    = smallest positive number such that 1+eps > 1. 
      ///            Approximately  beta**(-p), where beta is the machine 
      ///            radix and p is the number of significant base-beta 
      ///            digits in a floating-point number. 
      ///            (ANSI C: 0.5*DBL_EPSILON)
      ///   XMAX   = absolute argument beyond which DAW(X) underflows. 
      ///            XMAX = min(0.5/xmin, xinf). 
      ///   XSMALL = absolute argument below DAW(X)  may be represented 
      ///            by X.  We recommend XSMALL = sqrt(eps). 
      ///   XLARGE = argument beyond which DAW(X) may be represented by 
      ///            1/(2x).  We recommend XLARGE = 1/sqrt(eps). 
      ///
      /// Approximate values for some important machines are 
      ///
      ///                        beta  p     eps     xmin       xinf 
      ///
      ///  CDC 7600      (S.P.)    2  48  7.11E-15  3.14E-294  1.26E+322 
      ///  CRAY-1        (S.P.)    2  48  7.11E-15  4.58E-2467 5.45E+2465 
      ///  IEEE (IBM/XT, 
      ///    SUN, etc.)  (S.P.)    2  24  1.19E-07  1.18E-38   3.40E+38 
      ///  IEEE (IBM/XT, 
      ///    SUN, etc.)  (D.P.)    2  53  1.11D-16  2.23E-308  1.79D+308 
      ///  IBM 3033      (D.P.)   16  14  1.11D-16  5.40D-79   7.23D+75 
      ///  VAX 11/780    (S.P.)    2  24  5.96E-08  2.94E-39   1.70E+38 
      ///                (D.P.)    2  56  1.39D-17  2.94D-39   1.70D+38 
      ///   (G Format)   (D.P.)    2  53  1.11D-16  5.57D-309  8.98D+307 
      ///
      ///                         XSMALL     XLARGE     XMAX 
      ///
      ///  CDC 7600      (S.P.)  5.96E-08   1.68E+07  1.59E+293 
      ///  CRAY-1        (S.P.)  5.96E-08   1.68E+07  5.65E+2465 
      ///  IEEE (IBM/XT, 
      ///    SUN, etc.)  (S.P.)  2.44E-04   4.10E+03  4.25E+37 
      ///  IEEE (IBM/XT, 
      ///    SUN, etc.)  (D.P.)  1.05E-08   9.49E+07  2.24E+307 
      ///  IBM 3033      (D.P.)  3.73D-09   2.68E+08  7.23E+75 
      ///  VAX 11/780    (S.P.)  2.44E-04   4.10E+03  1.70E+38 
      ///                (D.P.)  3.73E-09   2.68E+08  1.70E+38 
      ///   (G Format)   (D.P.)  1.05E-08   9.49E+07  8.98E+307 
      ///
      ///
      /// These values are not neccessary in ANSI C++ because they are calculated
      /// at compile time from values given in the standard libraries! 
      /// </code></remarks>    
      public static double Dawson(double x, bool bDebug)
      {

        const double half   = 0.5;
        const double one    = 1.0;
        const double six25  = 6.25;
        const double one225 = 12.25;
        const double two5   = 25.0;
        // calculate limits (at compile time)

        double ret_val, frac, sump, sumq, ax, y, w2;

        ax = Math.Abs(x);
        if (ax > xlarge) 
          if (ax <= xmax_Dawson) 
            ret_val = half / x;
          else 
          {
            if(bDebug)
              System.Diagnostics.Trace.WriteLine("Abs(x) so large Dawson(x) underflows");
            ret_val = zero;
          }
        else if (ax < xsmall) 
          ret_val = x;
        else 
        {
          y = x * x;
          if (y < six25) 
          {
            // --------------------------------------------------------------
            //  abs(x) < 2.5 
            // --------------------------------------------------------------
            sump = p1[0];
            sumq = q1[0];
            for (int i = 1; i < 10; ++i) 
            {
              sump = sump * y + p1[i];
              sumq = sumq * y + q1[i];
            }
            ret_val = x * sump / sumq;
          } 
          else if (y < one225) 
          {
            // --------------------------------------------------------------
            //  2.5 <= abs(x) < 3.5 
            // --------------------------------------------------------------
            frac = zero;
            for (int i = 0; i < 9; ++i) 
              frac = q2[i] / (p2[i] + y + frac);
            ret_val = (p2[9] + frac) / x;
          } 
          else if (y < two5) 
          {
            // --------------------------------------------------------------
            //  3.5 <= abs(x) < 5.0 
            // --------------------------------------------------------------
            frac = zero;
            for (int i = 0; i < 9; ++i)
              frac = q3[i] / (p3[i] + y + frac);
            ret_val = (p3[9] + frac) / x;
          } 
          else 
          {
            // --------------------------------------------------------------
            //  5.0 <= abs(x) .<= xlarge 
            // --------------------------------------------------------------
            w2 = one / x / x;
            frac = zero;
            for (int i = 0; i < 9; ++i)
              frac = q4[i] / (p4[i] + y + frac);
            frac = p4[9] + frac;
            ret_val = (half + half * w2 * frac) / x;
          }
        }
        return ret_val;
      }

    }
    #endregion

    #region Faddeeva

    /// <summary>
    /// Given a complex number z = (x,y), this subroutine computes 
    /// the value of the Faddeeva function w(z) = exp(-z^2)*erfc(-i*z), 
    /// where erfc is the complex complementary error function and i means sqrt(-1). 
    /// </summary>
    /// <param name="z">The function argument.</param>
    /// <returns>The faddeeva function w(z).</returns>
    /// <remarks>
    /// <code>
    /// The accuracy of the algorithm for z in the 1st and 2nd quadrant 
    /// is 14 significant digits; in the 3rd and 4th it is 13 significant 
    /// digits outside a circular region with radius 0.126 around a zero 
    /// of the function. 
    ///
    /// All real variables in the program are double precision. 
    /// The parameter M_2_SQRTPI equals 2/sqrt(pi).  
    ///
    /// The routine is not underflow-protected but any variable can be 
    /// put to 0 upon underflow; 
    ///
    /// The routine is overflow-protected: Matpack::Error() is called.
    ///
    /// References:
    ///
    /// (1) G.P.M. Poppe, C.M.J. Wijers; More Efficient Computation of 
    ///     the Complex Error-Function, ACM Trans. Math. Software,
    ///     Vol. 16, no. 1, pp. 47.
    /// (2) Algorithm 680, collected algorithms from ACM.
    /// 
    /// The Fortran source code was translated to C++ by B.M. Gammel
    /// and added to the Matpack library, 1992.
    ///
    /// Last change: B. M. Gammel, 18.03.1996 error handling
    /// </code>
    /// </remarks>
    public static Complex Faddeeva (Complex z)
    {
      return Faddeeva(z, false);
    }

    /// <summary>
    /// Given a complex number z = (x,y), this subroutine computes 
    /// the value of the Faddeeva function w(z) = exp(-z^2)*erfc(-i*z), 
    /// where erfc is the complex complementary error function and i means sqrt(-1). 
    /// </summary>
    /// <param name="z">The function argument.</param>
    /// <param name="bDebug">If true, an exception is thrown if serious errors occur. If false, NaN is returned on errors.</param>
    /// <returns>The faddeeva function w(z).</returns>
    /// <remarks>
    /// <code>
    /// The accuracy of the algorithm for z in the 1st and 2nd quadrant 
    /// is 14 significant digits; in the 3rd and 4th it is 13 significant 
    /// digits outside a circular region with radius 0.126 around a zero 
    /// of the function. 
    ///
    /// All real variables in the program are double precision. 
    /// The parameter M_2_SQRTPI equals 2/sqrt(pi).  
    ///
    /// The routine is not underflow-protected but any variable can be 
    /// put to 0 upon underflow; 
    ///
    /// The routine is overflow-protected: Matpack::Error() is called.
    ///
    /// References:
    ///
    /// (1) G.P.M. Poppe, C.M.J. Wijers; More Efficient Computation of 
    ///     the Complex Error-Function, ACM Trans. Math. Software,
    ///     Vol. 16, no. 1, pp. 47.
    /// (2) Algorithm 680, collected algorithms from ACM.
    /// 
    /// The Fortran source code was translated to C++ by B.M. Gammel
    /// and added to the Matpack library, 1992.
    ///
    /// Last change: B. M. Gammel, 18.03.1996 error handling
    /// </code>
    /// </remarks>
    public static Complex Faddeeva (Complex z, bool bDebug)
    {
      // The maximum value of rmaxreal equals the root of the largest number 
      // rmax which can still be implemented on the computer in double precision
      // floating-point arithmetic
      double rmaxreal = Math.Sqrt(DBL_MAX);

      // rmaxexp  = ln(rmax) - ln(2)
      double rmaxexp  = Math.Log(DBL_MAX) - Math.Log(2.0);

      // the largest possible argument of a double precision goniometric function
      const double rmaxgoni = 1.0 / DBL_EPSILON;

      double xabs, yabs, daux, qrho, xaux, xsum, ysum, c, h, u, v,
        x, y, xabsq, xquad, yquad, h2 = 0.0, u1, v1, u2 = 0.0, v2 = 0.0, w1,
        rx, ry, sx, sy, tx, ty, qlambda = 0.0, xi, yi;
      int    i, j, n, nu, np1, kapn;
      bool a,b;

      xi = z.Re;
      yi = z.Im;
      xabs = Math.Abs(xi);
      yabs = Math.Abs(yi);
      x = xabs / 6.3;
      y = yabs / 4.4;

      // the following statement protects qrho = (x^2 + y^2) against overflow
      if ((xabs > rmaxreal) || (yabs > rmaxreal)) 
      {
        if(bDebug)
          throw new ArgumentException("Absolute value of argument so large w(z) overflows");
        else
          return Complex.NaN;
      }

      qrho = x * x + y * y;
      xabsq = xabs * xabs;
      xquad = xabsq - yabs * yabs;
      yquad = xabs * 2 * yabs;
    
      a = qrho < 0.085264;

      if (a) 
      {
  
        // If (qrho < 0.085264) then the Faddeeva-function is evaluated 
        // using a power-series (Abramowitz/Stegun, equation (7.1.5), p.297). 
        // n is the minimum number of terms needed to obtain the required 
        // accuracy.
  
        qrho = (1 - y * 0.85) * Math.Sqrt(qrho);
        n = Nint(qrho * 72 + 6);
        j = (n << 1) + 1;
        xsum = 1.0 / j;
        ysum = 0.0;
        for (i = n; i >= 1; --i) 
        {
          j -= 2;
          xaux = (xsum * xquad - ysum * yquad) / i;
          ysum = (xsum * yquad + ysum * xquad) / i;
          xsum = xaux + 1.0 / j;
        }
        u1 = (xsum * yabs + ysum * xabs) * -M_2_SQRTPI + 1.0;
        v1 = (xsum * xabs - ysum * yabs) * M_2_SQRTPI;
        daux = Math.Exp(-xquad);
        u2 = daux * Math.Cos(yquad);
        v2 = -daux * Math.Sin(yquad);
  
        u = u1 * u2 - v1 * v2;
        v = u1 * v2 + v1 * u2;
  
      } 
      else 
      {
  
        //  If (qrho > 1.0) then w(z) is evaluated using the Laplace continued 
        //  fraction.  nu is the minimum number of terms needed to obtain the
        //  required accuracy. 
        //  if ((qrho > 0.085264) && (qrho < 1.0)) then w(z) is evaluated
        //  by a truncated Taylor expansion, where the Laplace continued
        //  fraction is used to calculate the derivatives of w(z). 
        //  kapn is the minimum number of terms in the Taylor expansion needed
        //  to obtain the required accuracy. 
        //  nu is the minimum number of terms of the continued fraction needed
        //  to calculate the derivatives with the required accuracy. 
  
        if (qrho > 1.0) 
        {
          h = 0.0;
          kapn = 0;
          qrho = Math.Sqrt(qrho);
          nu = (int) (1442 / (qrho * 26 + 77) + 3);
        } 
        else 
        {
          qrho = (1 - y) * Math.Sqrt(1 - qrho);
          h = qrho * 1.88;
          h2 = h * 2;
          kapn = Nint(qrho * 34 + 7);
          nu   = Nint(qrho * 26 + 16);
        }
  
        b = h > 0.0;
  
        if (b) qlambda = Math.Pow(h2, (double) kapn);
  
        rx = ry = sx = sy = 0.0;
        for (n = nu; n >= 0; --n) 
        {
          np1 = n + 1;
          tx = yabs + h + np1 * rx;
          ty = xabs - np1 * ry;
          c = 0.5 / (tx * tx + ty * ty);
          rx = c * tx;
          ry = c * ty;
          if (b && (n <= kapn)) 
          {
            tx = qlambda + sx;
            sx = rx * tx - ry * sy;
            sy = ry * tx + rx * sy;
            qlambda /= h2;
          }
        }
  
        if (h == 0.0) 
        {
          u = rx * M_2_SQRTPI;
          v = ry * M_2_SQRTPI;
        } 
        else 
        {
          u = sx * M_2_SQRTPI;
          v = sy * M_2_SQRTPI;
        }
  
        if (yabs == 0.0) 
          u = Math.Exp(-(xabs * xabs));
      }
    
      //  evaluation of w(z) in the other quadrants 

      if (yi < 0.0) 
      {

        if (a) 
        {
          u2 *= 2;
          v2 *= 2;
        } 
        else 
        {
          xquad = -xquad;

          // the following statement protects 2*exp(-z**2) against overflow
          if ((yquad > rmaxgoni) || (xquad > rmaxexp)) 
          {
            if(bDebug)
              throw new ArgumentException("Absolute value of argument so large w(z) overflows");
            else
              return Complex.NaN;
          }

          w1 = Math.Exp(xquad) * 2;
          u2 =  w1 * Math.Cos(yquad);
          v2 = -w1 * Math.Sin(yquad);
        }

        u = u2 - u;
        v = v2 - v;
        if (xi > 0.0) v = -v;

      } 
      else if (xi < 0.0) 
        v = -v;
   
      return new Complex(u,v);
    }


    #endregion
  } // class 
} // namespace
