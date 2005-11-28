#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
// Original MatPack-1.7.3\Source\ei.cc


using System;

namespace Altaxo.Calc
{
  /// <summary>
  /// Exponential integral related functions.
  /// </summary>
  public class ExpIntegralRelated
  {



    /// <summary>
    /// This function program computes approximate values for the
    /// exponential integral  Ei(x), where  x  is real.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>Exponential integral  Ei(x)</returns>
    public static double ExpIntegralEi (double x)
    {
      return calcei(x,1);
    }

    /// <summary>
    /// This function program computes approximate values for the
    /// function  exp(-x) * Ei(x), where  Ei(x)  is the exponential
    /// integral, and  x  is real.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The
    /// function  exp(-x) * Ei(x), where  Ei(x)  is the exponential
    /// integral.</returns>
    public static double ExpIntegralExpEi (double x)
    {
      return calcei(x,3);
    }

    /// <summary>
    /// This function program computes approximate values for the
    /// exponential integral E1(x), where  x  is real.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The exponential integral E1(x).</returns>
    public static double ExpIntegralE1 (double x)
    {
      return calcei(x,2);
    }

  

    //-----------------------------------------------------------------------------//
    //
    // PROTOTYPES:
    //
    //-----------------------------------------------------------------------------//
    // double ExpIntegralEi (double x);
    //-----------------------------------------------------------------------------//
    // 
    //    This routine computes the exponential integral Ei(x) for real 
    //    arguments x where
    //
    //              /  integral (from t=-infinity to t=x) (exp(t)/t),  x > 0
    //     Ei(x) = <
    //              \ -integral (from t=-x to t=infinity) (exp(t)/t),  x < 0
    //
    //    and where the first integral is a principal value integral. 
    //    The argument x must not be equal to 0.
    //
    //    Error returns:  -x >  xbig   underflow  returns  0
    //                     x >= xmax   overflow   returns  xinf
    //                     x == 0      illegal    returns  -xinf
    //                     (xbig = 701.84, xmax = 716.351, xinf = 1.79e308)
    //
    //-----------------------------------------------------------------------------//
    // double ExpIntegralExpEi (double x);
    //-----------------------------------------------------------------------------//
    //
    //    This routine computes the exponential integral  exp(-x) * Ei(x). 
    //    The argument x must not be equal to 0.
    //
    //    Error returns:   x == 0      illegal    returns  -xinf
    //                     (xbig = 701.84, xmax = 716.351, xinf = 1.79e308)
    //
    //-----------------------------------------------------------------------------//
    // double ExpIntegralE1 (double x);
    //-----------------------------------------------------------------------------//
    //
    //    This routine computes the exponential integral E1(x). 
    //    The argument x must be greater than  0.
    //
    //    Error returns:   x >  xbig   underflow  returns  0
    //                     x == 0      illegal    returns  xinf
    //                     x <  0                 returns E1(-x) instead !
    //                     (xbig = 701.84, xmax = 716.351, xinf = 1.79e308)
    //
    //-----------------------------------------------------------------------------//
    //  INTERNAL INFORMATION:
    //-----------------------------------------------------------------------------//
    //
    //  The routine double calcei(double arg, int ii) is intended for internal
    //  use only, all computations within the packet being concentrated 
    //  in this routine.  The parameter usage is as follows:
    //
    //      Function Call                   Parameters
    //                            arg             return           ii
    //
    //      ExpIntegralEi(x)      x != 0          Ei(X)            1
    //      ExpIntegralE1(x)      x >  0         -Ei(-X)           2
    //      ExpIntegralExpEi(x)   x != 0          exp(-X)*Ei(X)    3
    //
    //  The main computation involves evaluation of rational Chebyshev
    //  approximations published in Math. Comp. 22, 641-649 (1968), and
    //  Math. Comp. 23, 289-303 (1969) by Cody and Thacher.  This
    //  transportable program is patterned after the machine-dependent
    //  FUNPACK packet  NATSEI,  but cannot match that version for
    //  efficiency or accuracy.  This version uses rational functions
    //  that theoretically approximate the exponential integrals to
    //  at least 18 significant decimal digits.  The accuracy achieved
    //  depends on the arithmetic system, the compiler, the intrinsic
    //  functions, and proper selection of the machine-dependent
    //  constants.
    //
    //-----------------------------------------------------------------------------//
    // EXPLANATION OF MACHINE-DEPENDENT CONSTANTS:
    //-----------------------------------------------------------------------------//
    //
    //   beta = radix for the floating-point system.
    //   minexp = smallest representable power of beta.
    //   maxexp = smallest power of beta that overflows.
    //   XBIG = largest argument acceptable to E1; solution to
    //          equation:
    //                     exp(-x)/x * (1 + 1/x) = beta ** minexp.
    //   XINF = largest positive machine number; approximately
    //                     beta ** maxexp
    //   XMAX = largest argument acceptable to EI; solution to
    //          equation:  exp(x)/x * (1 + 1/x) = beta ** maxexp.
    //
    //     Approximate values for some important machines are:
    //
    //                           beta      minexp      maxexp
    //
    //  CRAY-1        (S.P.)       2       -8193        8191
    //  Cyber 180/185 
    //    under NOS   (S.P.)       2        -975        1070
    //  IEEE (IBM/XT,
    //    SUN, etc.)  (S.P.)       2        -126         128
    //  IEEE (IBM/XT,
    //    SUN, etc.)  (D.P.)       2       -1022        1024
    //  IBM 3033      (D.P.)      16         -65          63
    //  VAX D-Format  (D.P.)       2        -128         127
    //  VAX G-Format  (D.P.)       2       -1024        1023
    //
    //                           XBIG       XINF       XMAX
    //
    //  CRAY-1        (S.P.)    5670.31  5.45E+2465   5686.21
    //  Cyber 180/185 
    //    under NOS   (S.P.)     669.31  1.26E+322     748.28
    //  IEEE (IBM/XT,
    //    SUN, etc.)  (S.P.)      82.93  3.40E+38       93.24
    //  IEEE (IBM/XT,
    //    SUN, etc.)  (D.P.)     701.84  1.79D+308     716.35
    //  IBM 3033      (D.P.)     175.05  7.23D+75      179.85
    //  VAX D-Format  (D.P.)      84.30  1.70D+38       92.54
    //  VAX G-Format  (D.P.)     703.22  8.98D+307     715.66
    //
    //-----------------------------------------------------------------------------//
    //
    //  Original Author: W. J. Cody
    //                   Mathematics abd Computer Science Division
    //                   Argonne National Laboratory
    //                   Argonne, IL 60439
    //                   Latest modification: September 9, 1988
    //
    // Recoding in C++:  Dr. Berndt M. Gammel
    //                   Physik Department TU Muenchen
    //                   85747 Garching, Germany
    //                   Latest modification: October 30, 1996
    //-----------------------------------------------------------------------------//


    //-------------------------------------------------------------------------//
    // Mathematical constants
    // exp40 = exp(40)
    // x0 = zero of Ei
    // x01/x11 + x02 = zero of Ei to extra precision
    //-------------------------------------------------------------------------//

    const double zero   = 0.0,
      p037   = 0.037,
      half   = 0.5,
      one    = 1.0,
      two    = 2.0,
      three  = 3.0,
      four   = 4.0,
      six    = 6.0,
      twelve = 12.0,
      two4   = 24.0,
      fourty = 40.0,
      exp40  = 2.353852668370199854078999e17, // exp(40.0) 
      x01    = 381.5,
      x11    = 1024.0,
      x02    = -5.1182968633365538008e-5,
      x0     = 3.7250741078136663466e-1;

    //-------------------------------------------------------------------------//
    // Machine-dependent constants for IEEE arithmetics
    //-------------------------------------------------------------------------//

    const double xinf = 1.79e+308,
      xmax = 716.351,
      xbig = 701.84;

    //-------------------------------------------------------------------------//
    // Coefficients  for -1.0 <= x < 0.0
    //-------------------------------------------------------------------------//

    static readonly double[] a = 
    {
      1.1669552669734461083368e2, 2.1500672908092918123209e3,
      1.5924175980637303639884e4, 8.9904972007457256553251e4,
      1.5026059476436982420737e5,-1.4815102102575750838086e5,
      5.0196785185439843791020e0 
    };
    static readonly double[] b = 
    {
      4.0205465640027706061433e1, 7.5043163907103936624165e2,
      8.1258035174768735759855e3, 5.2440529172056355429883e4,
      1.8434070063353677359298e5, 2.5666493484897117319268e5
    };

    //-------------------------------------------------------------------------//
    // Coefficients for -4.0 <= x < -1.0
    //-------------------------------------------------------------------------//

    static readonly double[] c = 
    {
      3.828573121022477169108e-1, 1.107326627786831743809e+1,
      7.246689782858597021199e+1, 1.700632978311516129328e+2,
      1.698106763764238382705e+2, 7.633628843705946890896e+1,
      1.487967702840464066613e+1, 9.999989642347613068437e-1,
      1.737331760720576030932e-8
    };
    static readonly double[] d = 
    {
      8.258160008564488034698e-2, 4.344836335509282083360e+0,
      4.662179610356861756812e+1, 1.775728186717289799677e+2,
      2.953136335677908517423e+2, 2.342573504717625153053e+2,
      9.021658450529372642314e+1, 1.587964570758947927903e+1,
      1.000000000000000000000e+0
    };

    //-------------------------------------------------------------------------//
    // Coefficients for x < -4.0
    //-------------------------------------------------------------------------//

    static readonly double[] e = 
    {
      1.3276881505637444622987e+2,3.5846198743996904308695e+4,
      1.7283375773777593926828e+5,2.6181454937205639647381e+5,
      1.7503273087497081314708e+5,5.9346841538837119172356e+4,
      1.0816852399095915622498e+4,1.0611777263550331766871e03,
      5.2199632588522572481039e+1,9.9999999999999999087819e-1
    };
    static readonly double[] f = 
    {
      3.9147856245556345627078e+4,2.5989762083608489777411e+5,
      5.5903756210022864003380e+5,5.4616842050691155735758e+5,
      2.7858134710520842139357e+5,7.9231787945279043698718e+4,
      1.2842808586627297365998e+4,1.1635769915320848035459e+3,
      5.4199632588522559414924e+1,1.0e0
    };

    //-------------------------------------------------------------------------//
    //  Coefficients for rational approximation to ln(x/a), |1-x/a| < .1
    //-------------------------------------------------------------------------//
    
    static readonly double[] plg = 
    {
      -2.4562334077563243311e+01,2.3642701335621505212e+02,
      -5.4989956895857911039e+02,3.5687548468071500413e+02
    };
    static readonly double[] qlg = 
    {
      -3.5553900764052419184e+01,1.9400230218539473193e+02,
      -3.3442903192607538956e+02,1.7843774234035750207e+02
    };
    
    //-------------------------------------------------------------------------//
    // Coefficients for  0.0 < x < 6.0,
    // ratio of Chebyshev polynomials
    //-------------------------------------------------------------------------//

    static readonly double[] p = 
    {
      -1.2963702602474830028590e01,-1.2831220659262000678155e03,
      -1.4287072500197005777376e04,-1.4299841572091610380064e06,
      -3.1398660864247265862050e05,-3.5377809694431133484800e08,
      3.1984354235237738511048e08,-2.5301823984599019348858e10,
      1.2177698136199594677580e10,-2.0829040666802497120940e11
    };
    static readonly double[] q = 
    {
      7.6886718750000000000000e01,-5.5648470543369082846819e03,
      1.9418469440759880361415e05,-4.2648434812177161405483e06,
      6.4698830956576428587653e07,-7.0108568774215954065376e08,
      5.4229617984472955011862e09,-2.8986272696554495342658e10,
      9.8900934262481749439886e10,-8.9673749185755048616855e10
    };

    //-------------------------------------------------------------------------//
    // j-fraction coefficients for 6.0 <= x < 12.0
    //-------------------------------------------------------------------------//

    static readonly double[] r = 
    {
      -2.645677793077147237806e00,-2.378372882815725244124e00,
      -2.421106956980653511550e01, 1.052976392459015155422e01,
      1.945603779539281810439e01,-3.015761863840593359165e01,
      1.120011024227297451523e01,-3.988850730390541057912e00,
      9.565134591978630774217e00, 9.981193787537396413219e-1
    };
    static readonly double[] s = 
    {
      1.598517957704779356479e-4, 4.644185932583286942650e00,
      3.697412299772985940785e02,-8.791401054875438925029e00,
      7.608194509086645763123e02, 2.852397548119248700147e01,
      4.731097187816050252967e02,-2.369210235636181001661e02,
      1.249884822712447891440e00
    };

    //-------------------------------------------------------------------------//
    // j-fraction coefficients for 12.0 <= x < 24.0
    //-------------------------------------------------------------------------//

    static readonly double[] p1 = 
    {
      -1.647721172463463140042e00,-1.860092121726437582253e01,
      -1.000641913989284829961e01,-2.105740799548040450394e01,
      -9.134835699998742552432e-1,-3.323612579343962284333e01,
      2.495487730402059440626e01, 2.652575818452799819855e01,
      -1.845086232391278674524e00, 9.999933106160568739091e-1
    };
    static readonly double[] q1 = 
    {
      9.792403599217290296840e01, 6.403800405352415551324e01,
      5.994932325667407355255e01, 2.538819315630708031713e02,
      4.429413178337928401161e01, 1.192832423968601006985e03,
      1.991004470817742470726e02,-1.093556195391091143924e01,
      1.001533852045342697818e00
    };

    //-------------------------------------------------------------------------//
    // j-fraction coefficients for  X >= 24.0
    //-------------------------------------------------------------------------//

    static readonly double[] p2 = 
    {
      1.75338801265465972390e02,-2.23127670777632409550e02,
      -1.81949664929868906455e01,-2.79798528624305389340e01,
      -7.63147701620253630855e00,-1.52856623636929636839e01,
      -7.06810977895029358836e00,-5.00006640413131002475e00,
      -3.00000000320981265753e00, 1.00000000000000485503e00
    };
    static readonly double[] q2 = 
    {
      3.97845977167414720840e04, 3.97277109100414518365e00,
      1.37790390235747998793e02, 1.17179220502086455287e02,
      7.04831847180424675988e01,-1.20187763547154743238e01,
      -7.99243595776339741065e00,-2.99999894040324959612e00,
      1.99999999999048104167e00
    };



    static double calcei (double arg, int ii)
    {
      int i;
      double  ei,frac,sump,sumq,t,w,xmx0,y,ysq;
      double[] px = new double[10];
      double[] qx = new double[10];

    
      //-------------------------------------------------------------------------//
      // code starts here
      //-------------------------------------------------------------------------//

      double x = arg;
      if (x == zero) 
      {

        ei = -xinf;
        if (ii == 2) ei = -ei;

      } 
      else if ((x < zero) || (ii == 2)) 
      { 

        //---------------------------------------------------------------------//
        // Calculate Ei for negative argument or for E1.
        //---------------------------------------------------------------------//

        y = Math.Abs(x);
        if (y <= one) 
        {
          sump = a[6] * y + a[0];
          sumq = y + b[0];
          for (i = 1; i < 6; i++) 
          {
            sump = sump * y + a[i];
            sumq = sumq * y + b[i];
          }
          ei = Math.Log(y) - sump / sumq;
          if (ii == 3) ei *= Math.Exp(y);
        } 
        else if (y <= four) 
        {
          w = one / y;
          sump = c[0];
          sumq = d[0];
          for (i = 1; i < 9; i++) 
          {
            sump = sump * w + c[i];
            sumq = sumq * w + d[i];
          }
          ei = -sump / sumq;
          if (ii != 3) ei *= Math.Exp(-y);
        } 
        else 
        {
          if ((y > xbig) && (ii < 3)) 
          {
            ei = zero;
          } 
          else 
          {
            w = one / y;
            sump = e[0];
            sumq = f[0];
            for (i = 1; i < 10; i++) 
            {
              sump = sump * w + e[i];
              sumq = sumq * w + f[i];
            }
            ei = -w * (one - w * sump / sumq );
            if (ii != 3) ei *= Math.Exp(-y);
          }
        }
        if (ii == 2) ei = -ei;

      } 
      else if (x < six) 
      {

        //---------------------------------------------------------------------//
        //  To improve conditioning, rational approximations are expressed
        //  in terms of Chebyshev polynomials for 0 <= x < 6, and in
        //  continued fraction form for larger x.
        //---------------------------------------------------------------------//

        t = x + x;
        t = t / three - two;
        px[0] = zero;
        qx[0] = zero;
        px[1] = p[0];
        qx[1] = q[0];
        for (i = 1; i < 9; i++) 
        {
          px[i+1] = t * px[i] - px[i-1] + p[i];
          qx[i+1] = t * qx[i] - qx[i-1] + q[i];
        }
        sump = half * t * px[9] - px[8] + p[9];
        sumq = half * t * qx[9] - qx[8] + q[9];
        frac = sump / sumq;
        xmx0 = (x - x01/x11) - x02;

        if (Math.Abs(xmx0) >= p037) 
        {

          ei = Math.Log(x/x0) + xmx0 * frac;
          if (ii == 3) ei *= Math.Exp(-x);

        } 
        else 
        {

          //-----------------------------------------------------------------//
          // Special approximation to  ln(x/x0)  for x close to x0
          //-----------------------------------------------------------------//

          y = xmx0 / (x + x0);
          ysq = y*y;
          sump = plg[0];
          sumq = ysq + qlg[0];
          for (i = 1; i < 4; i++) 
          {
            sump = sump*ysq + plg[i];
            sumq = sumq*ysq + qlg[i];
          }
          ei = (sump / (sumq*(x+x0)) + frac) * xmx0;
          if (ii == 3) ei *= Math.Exp(-x);
        }

      } 
      else if (x < twelve) 
      {

        frac = zero;
        for (i = 0; i < 9; i++)
          frac = s[i] / (r[i] + x + frac);
        ei = (r[9] + frac) / x;
        if (ii != 3) ei *= Math.Exp(x);

      } 
      else if (x <= two4) 
      {

        frac = zero;
        for (i = 0; i < 9; i++)
          frac = q1[i] / (p1[i] + x + frac);
        ei = (p1[9] + frac) / x;
        if (ii != 3) ei *=Math.Exp(x);

      } 
      else 
      {

        if ((x >= xmax) && (ii < 3)) 
        {
          ei = xinf;
        } 
        else 
        {
          y = one / x;
          frac = zero;
          for (i = 0; i < 9; i++)
            frac = q2[i] / (p2[i] + x + frac);
          frac = p2[9] + frac;
          ei = y + y * y * frac;
          if (ii != 3) 
          {
            if (x <= xmax-two4) 
            {
              ei *= Math.Exp(x);
            } 
            else 
            {
              //---------------------------------------------------------//
              // Calculation reformulated to avoid premature overflow
              //---------------------------------------------------------//
              ei = (ei * Math.Exp(x-fourty)) * exp40;
            }
          }
        }
      }   
      return ei;
    }
  }
}
