#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using System;

namespace Altaxo.Calc
{
  class FDistribution
  {

    #region ipmpar

    /*
-----------------------------------------------------------------------
 
     IPMPAR PROVIDES THE INTEGER MACHINE CONSTANTS FOR THE COMPUTER
     THAT IS USED. IT IS ASSUMED THAT THE ARGUMENT I IS AN INTEGER
     HAVING ONE OF THE VALUES 1-10. IPMPAR(I) HAS THE VALUE ...
 
  INTEGERS.
 
     ASSUME INTEGERS ARE REPRESENTED IN THE N-DIGIT, BASE-A FORM
 
               SIGN ( X(N-1)*A**(N-1) + ... + X(1)*A + X(0) )
 
               WHERE 0 .LE. X(I) .LT. A FOR I=0,...,N-1.
 
     IPMPAR(1) = A, THE BASE.
 
     IPMPAR(2) = N, THE NUMBER OF BASE-A DIGITS.
 
     IPMPAR(3) = A**N - 1, THE LARGEST MAGNITUDE.
 
  FLOATING-POINT NUMBERS.
 
     IT IS ASSUMED THAT THE SINGLE AND DOUBLE PRECISION FLOATING
     POINT ARITHMETICS HAVE THE SAME BASE, SAY B, AND THAT THE
     NONZERO NUMBERS ARE REPRESENTED IN THE FORM
 
               SIGN (B**E) * (X(1)/B + ... + X(M)/B**M)
 
               WHERE X(I) = 0,1,...,B-1 FOR I=1,...,M,
               X(1) .GE. 1, AND EMIN .LE. E .LE. EMAX.
 
     IPMPAR(4) = B, THE BASE.
 
  SINGLE-PRECISION
 
     IPMPAR(5) = M, THE NUMBER OF BASE-B DIGITS.
 
     IPMPAR(6) = EMIN, THE SMALLEST EXPONENT E.
 
     IPMPAR(7) = EMAX, THE LARGEST EXPONENT E.
 
  DOUBLE-PRECISION
 
     IPMPAR(8) = M, THE NUMBER OF BASE-B DIGITS.
 
     IPMPAR(9) = EMIN, THE SMALLEST EXPONENT E.
 
     IPMPAR(10) = EMAX, THE LARGEST EXPONENT E.
 
-----------------------------------------------------------------------
 
     TO DEFINE THIS FUNCTION FOR THE COMPUTER BEING USED REMOVE
     THE COMMENT DELIMITORS FROM THE DEFINITIONS DIRECTLY BELOW THE NAME
     OF THE MACHINE
 
-----------------------------------------------------------------------
 
     IPMPAR IS AN ADAPTATION OF THE FUNCTION I1MACH, WRITTEN BY
     P.A. FOX, A.D. HALL, AND N.L. SCHRYER (BELL LABORATORIES).
     IPMPAR WAS FORMED BY A.H. MORRIS (NSWC). THE CONSTANTS ARE
     FROM BELL LABORATORIES, NSWC, AND OTHER SOURCES.
 
-----------------------------------------------------------------------
     .. Scalar Arguments ..
*/


    /*     MACHINE CONSTANTS FOR IEEE ARITHMETIC MACHINES, SUCH AS THE AT&T
           3B SERIES, MOTOROLA 68000 BASED MACHINES (E.G. SUN 3 AND AT&T
           PC 7300), AND 8087 BASED MICROS (E.G. IBM PC AND AT&T 6300). */

    static int[] imach = new int[11]
      {
        0,
        2,
        31,
        2147483647,
        2,
        24,
        -125,
        128,
        53,
        -1021,
        1024
      };

    int ipmpar(int i)
    {
    return imach[i];
  }


    #endregion

    #region spmpar

    static double spmpar(int i)
    {
      return c_spmpar.fspmpar(i);
    }
   

    class c_spmpar
    {
      static int K1 = 4;
      static int K2 = 8;
      static int K3 = 9;
      static int K4 = 10;
      static double spmpar,b,binv,bm1,one,w,z;
      static int emax,emin,ibeta,m;

      public static double fspmpar(int i)
        /*
        -----------------------------------------------------------------------
 
             SPMPAR PROVIDES THE SINGLE PRECISION MACHINE CONSTANTS FOR
             THE COMPUTER BEING USED. IT IS ASSUMED THAT THE ARGUMENT
             I IS AN INTEGER HAVING ONE OF THE VALUES 1, 2, OR 3. IF THE
             SINGLE PRECISION ARITHMETIC BEING USED HAS M BASE B DIGITS AND
             ITS SMALLEST AND LARGEST EXPONENTS ARE EMIN AND EMAX, THEN
 
                SPMPAR(1) = B**(1 - M), THE MACHINE PRECISION,
 
                SPMPAR(2) = B**(EMIN - 1), THE SMALLEST MAGNITUDE,
 
                SPMPAR(3) = B**EMAX*(1 - B**(-M)), THE LARGEST MAGNITUDE.
 
        -----------------------------------------------------------------------
             WRITTEN BY
                ALFRED H. MORRIS, JR.
                NAVAL SURFACE WARFARE CENTER
                DAHLGREN VIRGINIA
        -----------------------------------------------------------------------
        -----------------------------------------------------------------------
             MODIFIED BY BARRY W. BROWN TO RETURN DOUBLE PRECISION MACHINE
             CONSTANTS FOR THE COMPUTER BEING USED.  THIS MODIFICATION WAS
             MADE AS PART OF CONVERTING BRATIO TO DOUBLE PRECISION
        -----------------------------------------------------------------------
        */
      {
 
      /*
           ..
           .. Executable Statements ..
      */
      if(i > 1) goto S10;
      b = ipmpar(K1);
      m = ipmpar(K2);
      spmpar = Math.Pow(b,(double)(1-m));
      return spmpar;
      S10:
      if(i > 2) goto S20;
      b = ipmpar(K1);
      emin = ipmpar(K3);
      one = 1.0;
      binv = one/b;
      w = Math.Pow(b,(double)(emin+2));
      spmpar = w*binv*binv*binv;
      return spmpar;
      S20:
      ibeta = ipmpar(K1);
      m = ipmpar(K2);
      emax = ipmpar(K4);
      b = ibeta;
      bm1 = ibeta-1;
      one = 1.0;
      z = Math.Pow(b,(double)(m-1));
      w = ((z-one)*b+bm1)/(b*z);
      z = Math.Pow(b,(double)(emax-2));
      spmpar = w*z*b*b;
      return spmpar;
    }
  }
    #endregion

    #region CUMF

    static double dsum,prod,xx,yy;
    static int ierr;
    static double T1,T2;

    static void cumf(
      double f, 
      double dfn,
      double dfd,
      ref double cum,
      ref double ccum)
      /*
      **********************************************************************
 
           void cumf(double *f,double *dfn,double *dfd,double *cum,double *ccum)
                          CUMulative F distribution
 
 
                                    Function
 
 
           Computes  the  integral from  0  to  F of  the f-density  with DFN
           and DFD degrees of freedom.
 
 
                                    Arguments
 
 
           F --> Upper limit of integration of the f-density.
                                                        F is DOUBLE PRECISION
 
           DFN --> Degrees of freedom of the numerator sum of squares.
                                                        DFN is DOUBLE PRECISI
 
           DFD --> Degrees of freedom of the denominator sum of squares.
                                                        DFD is DOUBLE PRECISI
 
           CUM <-- Cumulative f distribution.
                                                        CUM is DOUBLE PRECISI
 
           CCUM <-- Compliment of Cumulative f distribution.
                                                        CCUM is DOUBLE PRECIS
 
 
                                    Method
 
 
           Formula  26.5.28 of  Abramowitz and   Stegun   is  used to  reduce
           the cumulative F to a cumulative beta distribution.
 
 
                                    Note
 
 
           If F is less than or equal to 0, 0 is returned.
 
      **********************************************************************
      */
    {

    /*
         ..
         .. Executable Statements ..
    */

const double half=0.5e0;
const double done=1.0e0;


    if(!(f <= 0.0e0)) goto S10;
    cum = 0.0e0;
    ccum = 1.0e0;
    return;
    S10:
    prod = dfn*f;
    /*
         XX is such that the incomplete beta with parameters
         DFD/2 and DFN/2 evaluated at XX is 1 - CUM or CCUM
         YY is 1 - XX
         Calculate the smaller of XX and YY accurately
    */
    dsum = dfd+prod;
    xx = dfd/dsum;
    if(xx > half) 
  {
    yy = prod/dsum;
    xx = done-yy;
  }
  else  yy = done-xx;
  T1 = dfd*half;
  T2 = dfn*half;

  ccum = GammaRelated.BetaIR(xx,T1,T2);
  cum = 1-ccum;
  // bratio(&T1,&T2,&xx,&yy,ccum,cum,&ierr);
  return;

}


    #endregion


    const double tol=1.0e-8;
    const double atol=1.0e-50;
    const double zero=1.0e-300;
    const double inf=1.0e300;

    static int K1 = 1;
    static double K2 = 0.0e0;
    static double K4 = 0.5e0;
    static double K5 = 5.0e0;
    static double pq,fx,cum,ccum;
    static ulong qhi,qleft,qporq;
    static double T3,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15;

    /* *********************************************************************

            void cdff(int *which,double *p,double *q,double *f,double *dfn,
                double *dfd,int *status,double *bound)

                     Cumulative Distribution Function
                     F distribution


                                    Function


           Calculates any one parameter of the F distribution
           given values for the others.


                                    Arguments


           WHICH --> Integer indicating which of the next four argument
                     values is to be calculated from the others.
                     Legal range: 1..4
                     iwhich = 1 : Calculate P and Q from F,DFN and DFD
                     iwhich = 2 : Calculate F from P,Q,DFN and DFD
                     iwhich = 3 : Calculate DFN from P,Q,F and DFD
                     iwhich = 4 : Calculate DFD from P,Q,F and DFN

             P <--> The integral from 0 to F of the f-density.
                    Input range: [0,1].

             Q <--> 1-P.
                    Input range: (0, 1].
                    P + Q = 1.0.

             F <--> Upper limit of integration of the f-density.
                    Input range: [0, +infinity).
                    Search range: [0,1E300]

           DFN < --> Degrees of freedom of the numerator sum of squares.
                     Input range: (0, +infinity).
                     Search range: [ 1E-300, 1E300]

           DFD < --> Degrees of freedom of the denominator sum of squares.
                     Input range: (0, +infinity).
                     Search range: [ 1E-300, 1E300]

           STATUS <-- 0 if calculation completed correctly
                     -I if input parameter number I is out of range
                      1 if answer appears to be lower than lowest
                        search bound
                      2 if answer appears to be higher than greatest
                        search bound
                      3 if P + Q .ne. 1

           BOUND <-- Undefined if STATUS is 0

                     Bound exceeded by parameter number I if STATUS
                     is negative.

                     Lower search bound if STATUS is 1.

                     Upper search bound if STATUS is 2.


                                    Method


           Formula   26.6.2   of   Abramowitz   and   Stegun,  Handbook  of
           Mathematical  Functions (1966) is used to reduce the computation
           of the  cumulative  distribution function for the  F  variate to
           that of an incomplete beta.

           Computation of other parameters involve a seach for a value that
           produces  the desired  value  of P.   The search relies  on  the
           monotinicity of P with the other parameter.

                                    WARNING

           The value of the  cumulative  F distribution is  not necessarily
           monotone in  either degrees of freedom.  There  thus may  be two
           values  that  provide a given CDF  value.   This routine assumes
           monotonicity and will find an arbitrary one of the two values.

      **********************************************************************/
    void cdff(ref int which, ref double p, ref double q, ref double f, ref double dfn,
      ref double dfd, ref int status, ref double bound)
    {

      /*
           ..
           .. Executable Statements ..
      */
      /*
           Check arguments
      */
      if(!(which < 1 || which > 4)) goto S30;
      if(!(which < 1)) goto S10;
      bound = 1.0e0;
      goto S20;
      S10:
        bound = 4.0e0;
      S20:
        status = -1;
      return;
      S30:
        if(which == 1) goto S70;
      /*
           P
      */
      if(!(p < 0.0e0 || p > 1.0e0)) goto S60;
      if(!(p < 0.0e0)) goto S40;
      bound = 0.0e0;
      goto S50;
      S40:
        bound = 1.0e0;
      S50:
        status = -2;
      return;
      S70:
      S60:
        if(which == 1) goto S110;
      /*
           Q
      */
      if(!(q <= 0.0e0 || q > 1.0e0)) goto S100;
      if(!(q <= 0.0e0)) goto S80;
      bound = 0.0e0;
      goto S90;
      S80:
        bound = 1.0e0;
      S90:
        status = -3;
      return;
      S110:
      S100:
        if(which == 2) goto S130;
      /*
           F
      */
      if(!(f < 0.0e0)) goto S120;
      bound = 0.0e0;
      status = -4;
      return;
      S130:
      S120:
        if(which == 3) goto S150;
      /*
           DFN
      */
      if(!(dfn <= 0.0e0)) goto S140;
      bound = 0.0e0;
      status = -5;
      return;
      S150:
      S140:
        if(which == 4) goto S170;
      /*
           DFD
      */
      if(!(dfd <= 0.0e0)) goto S160;
      bound = 0.0e0;
      status = -6;
      return;
      S170:
      S160:
        if(which == 1) goto S210;
      /*
           P + Q
      */
      pq = p+q;
      if(!(Math.Abs(pq-0.5e0-0.5e0) > 3.0e0*spmpar(K1))) goto S200;
      if(!(pq < 0.0e0)) goto S180;
      bound = 0.0e0;
      goto S190;
      S180:
        bound = 1.0e0;
      S190:
        status = 3;
      return;
      S210:
      S200:
        if(!(which == 1)) qporq = (p <= q)? 1UL : 0UL ;
      /*
           Select the minimum of P or Q
           Calculate ANSWERS
      */
      if(1 == which) 
      {
        /*
             Calculating P
        */
        cumf(f,dfn,dfd,ref p,ref q);
        status = 0;
      }
      else if(2 == which) 
      {
        /*
             Calculating F
        */
        f = 5.0e0;
        T3 = inf;
        T6 = atol;
        T7 = tol;
        dstinv(ref K2,ref T3,ref K4, ref K4,ref K5, ref T6, ref T7);
        status = 0;
        dinvr(status,f,ref fx, ref qleft, ref qhi);
      S220:
        if(!(status == 1)) goto S250;
        cumf(f,dfn,dfd,ref cum, ref ccum);
        if(!qporq) goto S230;
        fx = cum-p;
        goto S240;
      S230:
        fx = ccum-q;
      S240:
        dinvr(status,f, ref fx, ref qleft, ref qhi);
        goto S220;
      S250:
        if(!(status == -1)) goto S280;
        if(!qleft) goto S260;
        status = 1;
        bound = 0.0e0;
        goto S270;
      S260:
        status = 2;
        bound = inf;
      S280:
      S270:
        ;
      }
      else if(3 == which) 
      {
        /*
             Calculating DFN
        */
        dfn = 5.0e0;
        T8 = zero;
        T9 = inf;
        T10 = atol;
        T11 = tol;
        dstinv(&T8,&T9,&K4,&K4,&K5,&T10,&T11);
        status = 0;
        dinvr(status,dfn,&fx,&qleft,&qhi);
      S290:
        if(!(status == 1)) goto S320;
        cumf(f,dfn,dfd,&cum,&ccum);
        if(!qporq) goto S300;
        fx = cum-p;
        goto S310;
      S300:
        fx = ccum-q;
      S310:
        dinvr(status,dfn,&fx,&qleft,&qhi);
        goto S290;
      S320:
        if(!(status == -1)) goto S350;
        if(!qleft) goto S330;
        status = 1;
        bound = zero;
        goto S340;
      S330:
        status = 2;
        bound = inf;
      S350:
      S340:
        ;
      }
      else if(4 == which) 
      {
        /*
             Calculating DFD
        */
        dfd = 5.0e0;
        T12 = zero;
        T13 = inf;
        T14 = atol;
        T15 = tol;
        dstinv(&T12,&T13,&K4,&K4,&K5,&T14,&T15);
        status = 0;
        dinvr(status,dfd,&fx,&qleft,&qhi);
      S360:
        if(!(status == 1)) goto S390;
        cumf(f,dfn,dfd,&cum,&ccum);
        if(!qporq) goto S370;
        fx = cum-p;
        goto S380;
      S370:
        fx = ccum-q;
      S380:
        dinvr(status,dfd,&fx,&qleft,&qhi);
        goto S360;
      S390:
        if(!(status == -1)) goto S420;
        if(!qleft) goto S400;
        status = 1;
        bound = zero;
        goto S410;
      S400:
        status = 2;
        bound = inf;
      S410:
        ;
      }
      S420:
        return;

    }
  }
}
