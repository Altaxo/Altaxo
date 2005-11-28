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
using System;

namespace Altaxo.Calc
{
#if IncludeFDistribution
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

  #region E000

    class _E0000
    {

      static bool qxmon(double zx, double zy, double zz)
      {
        return ((zx) <= (zy) && (zy) <= (zz));
      }


      /* DEFINE DINVR */
      static void E0000(int IENTRY,ref int status,
        ref double x, ref double fx,
        ref bool qleft,ref bool qhi, ref double zabsst,
        ref double zabsto,
        ref double zbig,
        ref double zrelst,
        ref double zrelto,
        ref double zsmall,
        ref double zstpmu)
      {

        double absstp,abstol,big,fbig,fsmall,relstp,reltol,small,step,stpmul,xhi,
          xlb,xlo,xsave,xub,yy;
        int i99999;
        bool qbdd,qcond,qdum1,qdum2,qincr,qlim,qok,qup;
        switch(IENTRY)
        {
          case 0: goto DINVR;
          case 1: goto DSTINV;
        }
        DINVR:
          if(status > 0) goto S310;
        qcond = !qxmon(small,x,big);
        if(qcond) 
          throw new ArgumentException("SMALL, X, BIG not monotone in INVR");

        xsave = x;
        /*
             See that SMALL and BIG bound the zero and set QINCR
        */
        x = small;
        /*
             GET-FUNCTION-VALUE
        */
        i99999 = 1;
        goto S300;
        S10:
          fsmall = fx;
        x = big;
        /*
             GET-FUNCTION-VALUE
        */
        i99999 = 2;
        goto S300;
        S20:
          fbig = fx;
        qincr = fbig > fsmall;
        if(!qincr) goto S50;
        if(fsmall <= 0.0e0) goto S30;
        status = -1;
        qleft = qhi = true;
        return;
        S30:
          if(fbig >= 0.0e0) goto S40;
        status = -1;
        qleft = qhi = false;
        return;
        S40:
          goto S80;
        S50:
          if(fsmall >= 0.0e0) goto S60;
        status = -1;
        qleft = true;
        qhi = false;
        return;
        S60:
          if(fbig <= 0.0e0) goto S70;
        status = -1;
        qleft = false;
        qhi = true;
        return;
        S80:
        S70:
          x = xsave;
        step = Math.Max(absstp,relstp*Math.Abs(x));
        /*
              YY = F(X) - Y
             GET-FUNCTION-VALUE
        */
        i99999 = 3;
        goto S300;
        S90:
          yy = fx;
        if(!(yy == 0.0e0)) goto S100;
        status = 0;
        qok = true;
        return;
        S100:
          qup = qincr && yy < 0.0e0 || !qincr && yy > 0.0e0;
        /*
        ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
             HANDLE CASE IN WHICH WE MUST STEP HIGHER
        ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        */
        if(!qup) goto S170;
        xlb = xsave;
        xub = Math.Min(xlb+step,big);
        goto S120;
        S110:
          if(qcond) goto S150;
        S120:
          /*
                YY = F(XUB) - Y
          */
          x = xub;
        /*
             GET-FUNCTION-VALUE
        */
        i99999 = 4;
        goto S300;
        S130:
          yy = fx;
        qbdd = qincr && yy >= 0.0e0 || !qincr && yy <= 0.0e0;
        qlim = xub >= big;
        qcond = qbdd || qlim;
        if(qcond) goto S140;
        step = stpmul*step;
        xlb = xub;
        xub = Math.Min(xlb+step,big);
        S140:
          goto S110;
        S150:
          if(!(qlim && !qbdd)) goto S160;
        status = -1;
        qleft = false;
        qhi = !qincr;
        x = big;
        return;
        S160:
          goto S240;
        S170:
          /*
          ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
               HANDLE CASE IN WHICH WE MUST STEP LOWER
          ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
          */
          xub = xsave;
        xlb = Math.Max(xub-step,small);
        goto S190;
        S180:
          if(qcond) goto S220;
        S190:
          /*
                YY = F(XLB) - Y
          */
          x = xlb;
        /*
             GET-FUNCTION-VALUE
        */
        i99999 = 5;
        goto S300;
        S200:
          yy = fx;
        qbdd = qincr && yy <= 0.0e0 || !qincr && yy >= 0.0e0;
        qlim = xlb <= small;
        qcond = qbdd || qlim;
        if(qcond) goto S210;
        step = stpmul*step;
        xub = xlb;
        xlb = Math.Max(xub-step,small);
        S210:
          goto S180;
        S220:
          if(!(qlim && !qbdd)) goto S230;
        status = -1;
        qleft = true;
        qhi = qincr;
        x = small;
        return;
        S240:
        S230:
          dstzr(&xlb,&xub,&abstol,&reltol);
        /*
        ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
             IF WE REACH HERE, XLB AND XUB BOUND THE ZERO OF F.
        ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        */
        status = 0;
        goto S260;
        S250:
          if(!(status == 1)) goto S290;
        S260:
          dzror(status,x,fx,&xlo,&xhi,&qdum1,&qdum2);
        if(!(*status == 1)) goto S280;
        /*
             GET-FUNCTION-VALUE
        */
        i99999 = 6;
        goto S300;
        S280:
        S270:
          goto S250;
        S290:
          x = xlo;
        status = 0;
        return;
        DSTINV:
          small = zsmall;
        big = zbig;
        absstp = zabsst;
        relstp = zrelst;
        stpmul = zstpmu;
        abstol = zabsto;
        reltol = zrelto;
        return;
        S300:
          /*
               TO GET-FUNCTION-VALUE
          */
          status = 1;
        return;
        S310:
          switch((int)i99999)
          {
            case 1: goto S10;
            case 2: goto S20;
            case 3: goto S90;
            case 4: goto S130;
            case 5: goto S200;
            case 6: goto S270;
            default: break;
          }
      }
    }

  #endregion

  #region E0001

    class _E0001
    {

      static double ftol(double zx)
      {
        return (0.5*Math.Max(abstol,reltol*Math.Abs(zx)));
      }


      /* DEFINE DZROR */
      public static void E0001(
        int IENTRY,
        ref int status,
        ref double x,
        ref double fx,
        ref double xlo,
        ref double xhi,
        ref bool qleft,
        ref bool qhi,
        ref double zabstl,
        ref double zreltl,
        ref double zxhi,
        ref double zxlo)
      {

        double a,abstol,b,c,d,fa,fb,fc,fd,fda,fdb,m,mb,p,q,reltol,tol,w,xxhi,xxlo;
        int ext,i99999;
        bool first,qrzero;
        switch(IENTRY)
        {
          case 0: goto DZROR; 
          case 1: goto DSTZR;
        }

        DZROR:
          if(status > 0) goto S280;
        xlo = xxlo;
        xhi = xxhi;
        b = x = xlo;
        /*
             GET-FUNCTION-VALUE
        */
        i99999 = 1;
        goto S270;
        S10:
          fb = fx;
        xlo = xhi;
        a = x = xlo;
        /*
             GET-FUNCTION-VALUE
        */
        i99999 = 2;
        goto S270;
        S20:
          /*
               Check that F(ZXLO) < 0 < F(ZXHI)  or
                          F(ZXLO) > 0 > F(ZXHI)
          */
          if(!(fb < 0.0e0)) goto S40;
        if(!(fx < 0.0e0)) goto S30;
        status = -1;
        qleft = fx < fb;
        qhi = 0;
        return;
        S40:
        S30:
          if(!(fb > 0.0e0)) goto S60;
        if(!(fx > 0.0e0)) goto S50;
        status = -1;
        qleft = fx > fb;
        qhi = true;
        return;
        S60:
        S50:
          fa = fx;
        first = true;
        S70:
          c = a;
        fc = fa;
        ext = 0;
        S80:
          if(!(Math.Abs(fc) < Math.Abs(fb))) goto S100;
        if(!(c != a)) goto S90;
        d = a;
        fd = fa;
        S90:
          a = b;
        fa = fb;
        xlo = c;
        b = xlo;
        fb = fc;
        c = a;
        fc = fa;
        S100:
          tol = ftol(xlo);
        m = (c+b)*0.5;
        mb = m-b;
        if(!(Math.Abs(mb) > tol)) goto S240;
        if(!(ext > 3)) goto S110;
        w = mb;
        goto S190;
        S110:
          tol = fifdsign(tol,mb);
        p = (b-a)*fb;
        if(!first) goto S120;
        q = fa-fb;
        first = 0;
        goto S130;
        S120:
          fdb = (fd-fb)/(d-b);
        fda = (fd-fa)/(d-a);
        p = fda*p;
        q = fdb*fa-fda*fb;
        S130:
          if(!(p < 0.0e0)) goto S140;
        p = -p;
        q = -q;
        S140:
          if(ext == 3) p *= 2.0e0;
        if(!(p*1.0e0 == 0.0e0 || p <= q*tol)) goto S150;
        w = tol;
        goto S180;
        S150:
          if(!(p < mb*q)) goto S160;
        w = p/q;
        goto S170;
        S160:
          w = mb;
        S190:
        S180:
        S170:
          d = a;
        fd = fa;
        a = b;
        fa = fb;
        b += w;
        xlo = b;
        x = *xlo;
        /*
             GET-FUNCTION-VALUE
        */
        i99999 = 3;
        goto S270;
        S200:
          fb = fx;
        if(!(fc*fb >= 0.0e0)) goto S210;
        goto S70;
        S210:
          if(!(w == mb)) goto S220;
        ext = 0;
        goto S230;
        S220:
          ext += 1;
        S230:
          goto S80;
        S240:
          xhi = c;
        qrzero = fc >= 0.0e0 && fb <= 0.0e0 || fc < 0.0e0 && fb >= 0.0e0;
        if(!qrzero) goto S250;
        status = 0;
        goto S260;
        S250:
          status = -1;
        S260:
          return;
        DSTZR:
          xxlo = zxlo;
        xxhi = zxhi;
        abstol = zabstl;
        reltol = zreltl;
        return;
        S270:
          /*
               TO GET-FUNCTION-VALUE
          */
          status = 1;
        return;
        S280:
          switch((int)i99999)
          {
            case 1: goto S10;
            case 2: goto S20;
            case 3: goto S200;
            default: break;}

      }


    }

    void dzror(ref int status, ref double x, ref double fx, ref double xlo,
      ref double xhi, ref bool qleft, ref bool qhi)
    /*
    **********************************************************************
 
         void dzror(int *status,double *x,double *fx,double *xlo,
               double *xhi,unsigned long *qleft,unsigned long *qhi)

         Double precision ZeRo of a function -- Reverse Communication
 
 
                                  Function
 
 
         Performs the zero finding.  STZROR must have been called before
         this routine in order to set its parameters.
 
 
                                  Arguments
 
 
         STATUS <--> At the beginning of a zero finding problem, STATUS
                     should be set to 0 and ZROR invoked.  (The value
                     of other parameters will be ignored on this call.)
 
                     When ZROR needs the function evaluated, it will set
                     STATUS to 1 and return.  The value of the function
                     should be set in FX and ZROR again called without
                     changing any of its other parameters.
 
                     When ZROR has finished without error, it will return
                     with STATUS 0.  In that case (XLO,XHI) bound the answe
 
                     If ZROR finds an error (which implies that F(XLO)-Y an
                     F(XHI)-Y have the same sign, it returns STATUS -1.  In
                     this case, XLO and XHI are undefined.
                             INTEGER STATUS
 
         X <-- The value of X at which F(X) is to be evaluated.
                             DOUBLE PRECISION X
 
         FX --> The value of F(X) calculated when ZROR returns with
                STATUS = 1.
                             DOUBLE PRECISION FX
 
         XLO <-- When ZROR returns with STATUS = 0, XLO bounds the
                 inverval in X containing the solution below.
                             DOUBLE PRECISION XLO
 
         XHI <-- When ZROR returns with STATUS = 0, XHI bounds the
                 inverval in X containing the solution above.
                             DOUBLE PRECISION XHI
 
         QLEFT <-- .TRUE. if the stepping search terminated unsucessfully
                    at XLO.  If it is .FALSE. the search terminated
                    unsucessfully at XHI.
                        QLEFT is LOGICAL
 
         QHI <-- .TRUE. if F(X) .GT. Y at the termination of the
                  search and .FALSE. if F(X) .LT. Y at the
                  termination of the search.
                        QHI is LOGICAL
 
    **********************************************************************
    */
  {
      double dum1=0,dum2=0,dum3=0,dum4=0;
    _E0001.E0001(0,ref status,ref x,ref fx,ref xlo,ref xhi,ref qleft,ref qhi, ref dum1,ref dum2,ref dum3,ref dum4);
  }
  void dstzr(ref double zxlo, ref double zxhi, ref double zabstl, ref double zreltl)
  /*
  **********************************************************************
       void dstzr(double *zxlo,double *zxhi,double *zabstl,double *zreltl)
       Double precision SeT ZeRo finder - Reverse communication version
                                Function
       Sets quantities needed by ZROR.  The function of ZROR
       and the quantities set is given here.
       Concise Description - Given a function F
       find XLO such that F(XLO) = 0.
            More Precise Description -
       Input condition. F is a double precision function of a single
       double precision argument and XLO and XHI are such that
            F(XLO)*F(XHI)  .LE.  0.0
       If the input condition is met, QRZERO returns .TRUE.
       and output values of XLO and XHI satisfy the following
            F(XLO)*F(XHI)  .LE. 0.
            ABS(F(XLO)  .LE. ABS(F(XHI)
            ABS(XLO-XHI)  .LE. TOL(X)
       where
            TOL(X) = MAX(ABSTOL,RELTOL*ABS(X))
       If this algorithm does not find XLO and XHI satisfying
       these conditions then QRZERO returns .FALSE.  This
       implies that the input condition was not met.
                                Arguments
       XLO --> The left endpoint of the interval to be
             searched for a solution.
                      XLO is DOUBLE PRECISION
       XHI --> The right endpoint of the interval to be
             for a solution.
                      XHI is DOUBLE PRECISION
       ABSTOL, RELTOL --> Two numbers that determine the accuracy
                        of the solution.  See function for a
                        precise definition.
                      ABSTOL is DOUBLE PRECISION
                      RELTOL is DOUBLE PRECISION
                                Method
       Algorithm R of the paper 'Two Efficient Algorithms with
       Guaranteed Convergence for Finding a Zero of a Function'
       by J. C. P. Bus and T. J. Dekker in ACM Transactions on
       Mathematical Software, Volume 1, no. 4 page 330
       (Dec. '75) is employed to find the zero of F(X)-Y.
  **********************************************************************
  */
{
    int idum1;
    double dum1=0, dum2=0, dum3=0, dum4=0;
    bool bdum1=false, bdum2=false;
  _E0001.E0001(1,ref idum1,ref dum1,ref dum2,ref dum3, ref dum4, ref bdum1,ref bdum2,ref zabstl,ref zreltl,ref zxhi,ref zxlo);
}

  #endregion
  #region dstinv

  void dstinv(ref double zsmall, ref double zbig, ref double zabsst,
  ref double zrelst, ref double zstpmu, ref double zabsto,
  ref double zrelto)
  /*
      **********************************************************************
            void dstinv(double *zsmall,double *zbig,double *zabsst,
                  double *zrelst,double *zstpmu,double *zabsto,
                  double *zrelto)

            Double Precision - SeT INverse finder - Reverse Communication
                                    Function
           Concise Description - Given a monotone function F finds X
           such that F(X) = Y.  Uses Reverse communication -- see invr.
           This routine sets quantities needed by INVR.
                More Precise Description of INVR -
           F must be a monotone function, the results of QMFINV are
           otherwise undefined.  QINCR must be .TRUE. if F is non-
           decreasing and .FALSE. if F is non-increasing.
           QMFINV will return .TRUE. if and only if F(SMALL) and
           F(BIG) bracket Y, i. e.,
                QINCR is .TRUE. and F(SMALL).LE.Y.LE.F(BIG) or
                QINCR is .FALSE. and F(BIG).LE.Y.LE.F(SMALL)
           if QMFINV returns .TRUE., then the X returned satisfies
           the following condition.  let
                     TOL(X) = MAX(ABSTOL,RELTOL*ABS(X))
           then if QINCR is .TRUE.,
                F(X-TOL(X)) .LE. Y .LE. F(X+TOL(X))
           and if QINCR is .FALSE.
                F(X-TOL(X)) .GE. Y .GE. F(X+TOL(X))
                                    Arguments
           SMALL --> The left endpoint of the interval to be
                searched for a solution.
                          SMALL is DOUBLE PRECISION
           BIG --> The right endpoint of the interval to be
                searched for a solution.
                          BIG is DOUBLE PRECISION
           ABSSTP, RELSTP --> The initial step size in the search
                is MAX(ABSSTP,RELSTP*ABS(X)). See algorithm.
                          ABSSTP is DOUBLE PRECISION
                          RELSTP is DOUBLE PRECISION
           STPMUL --> When a step doesn't bound the zero, the step
                      size is multiplied by STPMUL and another step
                      taken.  A popular value is 2.0
                          DOUBLE PRECISION STPMUL
           ABSTOL, RELTOL --> Two numbers that determine the accuracy
                of the solution.  See function for a precise definition.
                          ABSTOL is DOUBLE PRECISION
                          RELTOL is DOUBLE PRECISION
                                    Method
           Compares F(X) with Y for the input value of X then uses QINCR
           to determine whether to step left or right to bound the
           desired x.  the initial step size is
                MAX(ABSSTP,RELSTP*ABS(S)) for the input value of X.
           Iteratively steps right or left until it bounds X.
           At each step which doesn't bound X, the step size is doubled.
           The routine is careful never to step beyond SMALL or BIG.  If
           it hasn't bounded X at SMALL or BIG, QMFINV returns .FALSE.
           after setting QLEFT and QHI.
           If X is successfully bounded then Algorithm R of the paper
           'Two Efficient Algorithms with Guaranteed Convergence for
           Finding a Zero of a Function' by J. C. P. Bus and
           T. J. Dekker in ACM Transactions on Mathematical
           Software, Volume 1, No. 4 page 330 (DEC. '75) is employed
           to find the zero of the function F(X)-Y. This is routine
           QRZERO.
      **********************************************************************
      */
{
  E0000(1,NULL,NULL,NULL,NULL,NULL,zabsst,zabsto,zbig,zrelst,zrelto,zsmall,
  zstpmu);
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
#endif
}
