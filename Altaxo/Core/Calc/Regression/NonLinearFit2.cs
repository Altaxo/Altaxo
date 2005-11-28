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
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression
{

  /// <summary>
  /// Levenberg - Marquard methods adapted to C# from C++ sources from Manolis Lourakis (see below).
  /// </summary>
  /// <remarks>
  ///  Adapted from the following C++ sources:
  ///  "Levenberg - Marquardt non-linear minimization algorithm", 
  ///  Copyright (C) 2004  Manolis Lourakis (lourakis@ics.forth.gr), 
  ///  Institute of Computer Science, Foundation for Research and Technology - Hellas,
  ///  Heraklion, Crete, Greece.</remarks>
  public class NonLinearFit2
  {

    #region constants
    const int LM_OPTS_SZ = 5;
    const int LM_INFO_SZ = 9;
    const double LM_INIT_MU = 1E-03;
    const double LM_STOP_THRESH = 1E-17;
    const double LM_DIFF_DELTA = 1E-06;
    const double LM_REAL_MAX = double.MaxValue; 
    const double LM_REAL_MIN = double.Epsilon; // this is ok, in C++ this is the smallest number near zero
    const double LM_REAL_EPSILON = Altaxo.Calc.DoubleConstants.DBL_EPSILON;
    const double EPSILON = 1E-12;
    const double ONE_THIRD = 1.0/3.0;

    const int __BLOCKSZ__  =  32; /* block size for cache-friendly matrix-matrix multiply. It should be
                              * such that __BLOCKSZ__^2*sizeof(LM_REAL) is smaller than the CPU (L1)
                              * data cache size. Notice that a value of 32 when LM_REAL=double assumes
                              * an 8Kb L1 data cache (32*32*8=8K). This is a concervative choice since
                              * newer Pentium 4s have a L1 data cache of size 16K, capable of holding
                              * up to 45x45 double blocks.
                              */
    const int __BLOCKSZ__SQ = (__BLOCKSZ__) * (__BLOCKSZ__);
    #endregion

    /* Secant version of the LEVMAR_DER() function above: the jacobian is approximated with 
 * the aid of finite differences (forward or central, see the comment for the opts argument)
 */

    public delegate void FitFunction(double[] parameter, double[] output, object additionalData);

    public delegate void JacobianFunction( double [] parameter, double[] output, object additionalData);

    class WorkArrays
    {
      public double[] e;
      public double[] hx;
      public double[] jacTe;
      public double[] jac;
      public double[] jacTjac;
      public double[] Dp;
      public double[] diag_jacTjac;
      public double[] pDp;
      public double[] wrk;

      /// <summary>
      /// Set up the working arrays.
      /// </summary>
      /// <param name="n">Number of data points.</param>
      /// <param name="m">Number of parameter.</param>
      public WorkArrays(int n, int m)
      {
        /* set up work arrays */
        e = new double[n];
        hx = new double[n];
        jacTe = new double[m];
        jac = new double[n * m];
        jacTjac = new double[m * m];
        Dp = new double[m ];
        diag_jacTjac = new double[m];
        pDp = new double[ m] ;
        wrk = new double[n];

      }
    }


    /* 
 * This function seeks the parameter vector p that best describes the measurements vector x.
 * More precisely, given a vector function  func : R^m --> R^n with n>=m,
 * it finds p s.t. func(p) ~= x, i.e. the squared second order (i.e. L2) norm of
 * e=x-func(p) is minimized.
 *
 * This function requires an analytic jacobian. In case the latter is unavailable,
 * use LEVMAR_DIF() bellow
 *
 * Returns the number of iterations (>=0) if successfull, -1 if failed
 *
 * For more details, see H.B. Nielsen's (http://www.imm.dtu.dk/~hbn) IMM/DTU
 * tutorial at http://www.imm.dtu.dk/courses/02611/nllsq.pdf
 */

    public static int LEVMAR_DER(
      FitFunction  func, /* functional relation describing measurements. A p \in R^m yields a \hat{x} \in  R^n */
      JacobianFunction jacf,  /* function to evaluate the jacobian \part x / \part p */ 
      double []p,         /* I/O: initial parameter estimates. On output has the estimated solution */
      double []x,         /* I: measurement vector */
      double []weights,   /* vector of the weights used to scale the fit differences, can be null */
         
      int itmax,          /* I: maximum number of iterations */
      double[] opts,    /* I: minim. options [\mu, \epsilon1, \epsilon2, \epsilon3]. Respectively the scale factor for initial \mu,
                       * stopping thresholds for ||J^T e||_inf, ||Dp||_2 and ||e||_2. Set to NULL for defaults to be used
                       */
      double[] info,
      /* O: information regarding the minimization. Set to NULL if don't care
                      * info[0]= ||e||_2 at initial p.
                      * info[1-4]=[ ||e||_2, ||J^T e||_inf,  ||Dp||_2, mu/max[J^T J]_ii ], all computed at estimated p.
                      * info[5]= # iterations,
                      * info[6]=reason for terminating: 1 - stopped by small gradient J^T e
                      *                                 2 - stopped by small Dp
                      *                                 3 - stopped by itmax
                      *                                 4 - singular matrix. Restart from current p with increased mu 
                      *                                 5 - no further error reduction is possible. Restart with increased mu
                      *                                 6 - stopped by small ||e||_2
                      * info[7]= # function evaluations
                      * info[8]= # jacobian evaluations
                      */
      ref object workingmemory,     /* working memory, allocate if NULL */
      double[] covar,    /* O: Covariance matrix corresponding to LS solution; mxm. Set to NULL if not needed. */
      object adata)       /* pointer to possibly additional data, passed uninterpreted to func & jacf.
                      * Set to NULL if not needed
                      */
    {
      int m = p.Length;  /* I: parameter vector dimension (i.e. #unknowns) */
      int n = x.Length;  /* I: measurement vector dimension */

      int i, j, k, l;
      int issolved;
      /* temp work arrays */
      double[] e,          /* nx1 */
        hx,         /* \hat{x}_i, nx1 */
        jacTe,      /* J^T e_i mx1 */
        jac,        /* nxm */
        jacTjac,    /* mxm */
        Dp,         /* mx1 */
        diag_jacTjac,   /* diagonal of J^T J, mx1 */
        pDp;        /* p + Dp, mx1 */

      double mu,  /* damping constant */
        tmp; /* mainly used in matrix & vector multiplications */
      double p_eL2, jacTe_inf, pDp_eL2; /* ||e(p)||_2, ||J^T e||_inf, ||e(p+Dp)||_2 */
      double p_L2, Dp_L2=LM_REAL_MAX, dF, dL;
      double tau, eps1, eps2, eps2_sq, eps3;
      double init_p_eL2;
      int nu=2, nu2, stop, nfev, njev=0;
      int nm=n*m;

      mu=jacTe_inf=0.0; /* -Wall */

      if(n<m)
      {
        throw new ArithmeticException(string.Format("Cannot solve a problem with fewer measurements {0} than unknowns {1}", n, m));
      }

      if(null==jacf)
      {
        throw new ArgumentException("No function specified for computing the jacobian. If no such function is available, use LEVMAR_DIF instead");
      }

      if(null!=opts)
      {
        tau=opts[0];
        eps1=opts[1];
        eps2=opts[2];
        eps2_sq=opts[2]*opts[2];
        eps3=opts[3];
      }
      else
      { // use default values
        tau=(LM_INIT_MU);
        eps1=(LM_STOP_THRESH);
        eps2=(LM_STOP_THRESH);
        eps2_sq=(LM_STOP_THRESH)*(LM_STOP_THRESH);
        eps3=(LM_STOP_THRESH);
      }

  

      /* set up work arrays */
      WorkArrays work = workingmemory as WorkArrays;
      if(null==work)
      {
        work = new WorkArrays(n, m);
        workingmemory = work;
      }

      /* set up work arrays */
      e=work.e;
      hx = work.hx;
      jacTe = work.jacTe;
      jac = work.jac;
      jacTjac = work.jacTjac;
      Dp = work.Dp;
      diag_jacTjac = work.diag_jacTjac;
      pDp = work.pDp;
  

      /* compute e=x - f(p) and its L2 norm */
      func(p, hx, adata); nfev=1;
      if (weights == null)
      {
        for (i = 0, p_eL2 = 0.0; i < n; ++i)
        {
          e[i] = tmp = x[i] - hx[i];
          p_eL2 += tmp * tmp;
        }
      }
      else
      {
        for (i = 0, p_eL2 = 0.0; i < n; ++i)
        {
          e[i] = tmp = (x[i] - hx[i])*weights[i];
          p_eL2 += tmp * tmp;
        }
      }
      init_p_eL2=p_eL2;

      for(k=stop=0; k<itmax && 0==stop; ++k)
      {
        /* Note that p and e have been updated at a previous iteration */

        if(p_eL2<=eps3)
        { /* error is small */
          stop=6;
          break;
        }

        /* Compute the jacobian J at p,  J^T J,  J^T e,  ||J^T e||_inf and ||p||^2.
         * Since J^T J is symmetric, its computation can be speeded up by computing
         * only its upper triangular part and copying it to the lower part
         */

        jacf(p, jac, adata); ++njev;

        /* J^T J, J^T e */
        if(nm<__BLOCKSZ__SQ)
        { // this is a small problem
          /* This is the straightforward way to compute J^T J, J^T e. However, due to
           * its noncontinuous memory access pattern, it incures many cache misses when
           * applied to large minimization problems (i.e. problems involving a large
           * number of free variables and measurements), in which J is too large to
           * fit in the L1 cache. For such problems, a cache-efficient blocking scheme
           * is preferable.
           *
           * Thanks to John Nitao of Lawrence Livermore Lab for pointing out this
           * performance problem.
           *
           * On the other hand, the straightforward algorithm is faster on small
           * problems since in this case it avoids the overheads of blocking. 
           */

          for(i=0; i<m; ++i)
          {
            for(j=i; j<m; ++j)
            {
              int lm;

              if (weights == null)
              {
                for (l = 0, tmp = 0.0; l < n; ++l)
                {
                  lm = l * m;
                  tmp += jac[lm + i] * jac[lm + j];
                }
              }
              else
              {
                for (l = 0, tmp = 0.0; l < n; ++l)
                {
                  lm = l * m;
                  tmp += jac[lm + i] * jac[lm + j] * weights[i] * weights[i];
                }
              }

              /* store tmp in the corresponding upper and lower part elements */
              jacTjac[i*m+j]=jacTjac[j*m+i]=tmp;
            }

            /* J^T e */
            for(l=0, tmp=0.0; l<n; ++l)
              tmp+=jac[l*m+i]*e[l];
            jacTe[i]=tmp;
          }
        }
        else
        { // this is a large problem
          /* Cache efficient computation of J^T J based on blocking
           */
          TRANS_MAT_MAT_MULT(jac, jacTjac, n, m, __BLOCKSZ__,weights);

          /* cache efficient computation of J^T e */
          for(i=0; i<m; ++i)
            jacTe[i]=0.0;

          for(i=0; i<n; ++i)
          {
            int jacrow;

            for(l=0, jacrow=i*m, tmp=e[i]; l<m; ++l)
              jacTe[l]+=jac[jacrow+l]*tmp;
          }
        }

        /* Compute ||J^T e||_inf and ||p||^2 */
        for(i=0, p_L2=jacTe_inf=0.0; i<m; ++i)
        {
          if(jacTe_inf < (tmp=Math.Abs(jacTe[i]))) jacTe_inf=tmp;

          diag_jacTjac[i]=jacTjac[i*m+i]; /* save diagonal entries so that augmentation can be later canceled */
          p_L2+=p[i]*p[i];
        }
        //p_L2=sqrt(p_L2);

#if false
if(!(k%100)){
  printf("Current estimate: ");
  for(i=0; i<m; ++i)
    printf("%.9g ", p[i]);
  printf("-- errors %.9g %0.9g\n", jacTe_inf, p_eL2);
}
#endif

        /* check for convergence */
        if((jacTe_inf <= eps1))
        {
          Dp_L2=0.0; /* no increment for p in this case */
          stop=1;
          break;
        }

        /* compute initial damping factor */
        if(k==0)
        {
          for(i=0, tmp=LM_REAL_MIN; i<m; ++i)
            if(diag_jacTjac[i]>tmp) tmp=diag_jacTjac[i]; /* find max diagonal element */
          mu=tau*tmp;
        }

        /* determine increment using adaptive damping */
        while(true)
        {
          /* augment normal equations */
          for(i=0; i<m; ++i)
            jacTjac[i*m+i]+=mu;

          /* solve augmented equations */
#if HAVE_LAPACK
      /* 5 alternatives are available: LU, Cholesky, 2 variants of QR decomposition and SVD.
       * Cholesky is the fastest but might be inaccurate; QR is slower but more accurate;
       * SVD is the slowest but most accurate; LU offers a tradeoff between accuracy and speed
       */

      issolved=AX_EQ_B_LU(jacTjac, jacTe, Dp, m);
      //issolved=AX_EQ_B_CHOL(jacTjac, jacTe, Dp, m);
      //issolved=AX_EQ_B_QR(jacTjac, jacTe, Dp, m);
      //issolved=AX_EQ_B_QRLS(jacTjac, jacTe, Dp, m, m);
      //issolved=AX_EQ_B_SVD(jacTjac, jacTe, Dp, m);

#else
    
          /* use the LU included with levmar */
          issolved=AX_EQ_B_LU(jacTjac, jacTe, Dp, m);
#endif // HAVE_LAPACK 

          if(0!=issolved)
          {
            /* compute p's new estimate and ||Dp||^2 */
            for(i=0, Dp_L2=0.0; i<m; ++i)
            {
              pDp[i]=p[i] + (tmp=Dp[i]);
              Dp_L2+=tmp*tmp;
            }
            //Dp_L2=sqrt(Dp_L2);

            if(Dp_L2<=eps2_sq*p_L2)
            { /* relative change in p is small, stop */
              //if(Dp_L2<=eps2*(p_L2 + eps2)){ /* relative change in p is small, stop */
              stop=2;
              break;
            }

            if(Dp_L2>=(p_L2+eps2)/((EPSILON)*(EPSILON)))
            { /* almost singular */
              //if(Dp_L2>=(p_L2+eps2)/CNST(EPSILON)){ /* almost singular */
              stop=4;
              break;
            }

            func(pDp, hx, adata); ++nfev; /* evaluate function at p + Dp */
            if (weights == null)
            {
              for (i = 0, pDp_eL2 = 0.0; i < n; ++i)
              { /* compute ||e(pDp)||_2 */
                hx[i] = tmp = x[i] - hx[i];
                pDp_eL2 += tmp * tmp;
              }
            }
            else // use weights
            {
              for (i = 0, pDp_eL2 = 0.0; i < n; ++i)
              { /* compute ||e(pDp)||_2 */
                hx[i] = tmp = (x[i] - hx[i])*weights[i];
                pDp_eL2 += tmp * tmp;
              }
            }

            for(i=0, dL=0.0; i<m; ++i)
              dL+=Dp[i]*(mu*Dp[i]+jacTe[i]);

            dF=p_eL2-pDp_eL2;

            if(dL>0.0 && dF>0.0)
            { /* reduction in error, increment is accepted */
              tmp=((2.0)*dF/dL-(1.0));
              tmp=(1.0)-tmp*tmp*tmp;
              mu=mu*( (tmp>=(ONE_THIRD))? tmp : (ONE_THIRD) );
              nu=2;

              for(i=0 ; i<m; ++i) /* update p's estimate */
                p[i]=pDp[i];

              for(i=0; i<n; ++i) /* update e and ||e||_2 */
                e[i]=hx[i];
              p_eL2=pDp_eL2;
              break;
            }
          }

          /* if this point is reached, either the linear system could not be solved or
           * the error did not reduce; in any case, the increment must be rejected
           */

          mu*=nu;
          nu2=nu<<1; // 2*nu;
          if(nu2<=nu)
          { /* nu has wrapped around (overflown). Thanks to Frank Jordan for spotting this case */
            stop=5;
            break;
          }
          nu=nu2;

          for(i=0; i<m; ++i) /* restore diagonal J^T J entries */
            jacTjac[i*m+i]=diag_jacTjac[i];
        } /* inner loop */
      }

      if(k>=itmax) stop=3;

      for(i=0; i<m; ++i) /* restore diagonal J^T J entries */
        jacTjac[i*m+i]=diag_jacTjac[i];

      if(null!=info)
      {
        info[0]=init_p_eL2;
        info[1]=p_eL2;
        info[2]=jacTe_inf;
        info[3]=Dp_L2;
        for(i=0, tmp=LM_REAL_MIN; i<m; ++i)
          if(tmp<jacTjac[i*m+i]) tmp=jacTjac[i*m+i];
        info[4]=mu/tmp;
        info[5]=(double)k;
        info[6]=(double)stop;
        info[7]=(double)nfev;
        info[8]=(double)njev;
      }

      /* covariance matrix */
      if(null!=covar)
      {
        LEVMAR_COVAR(jacTjac, covar, p_eL2, m, n);
      }

      return (stop!=4)?  k : -1;
    }




    static public int LEVMAR_DIF(
      FitFunction func, /* functional relation describing measurements. A p \in R^m yields a \hat{x} \in  R^n */
      double [] p,         /* I/O: initial parameter estimates. On output has the estimated solution */
      double [] x,         /* I: measurement vector */
 
      int itmax,          /* I: maximum number of iterations */
      double[] opts,    /* I: opts[0-4] = minim. options [\mu, \epsilon1, \epsilon2, \epsilon3, \delta]. Respectively the
                       * scale factor for initial \mu, stopping thresholds for ||J^T e||_inf, ||Dp||_2 and ||e||_2 and
                       * the step used in difference approximation to the jacobian. Set to NULL for defaults to be used.
                       * If \delta<0, the jacobian is approximated with central differences which are more accurate
                       * (but slower!) compared to the forward differences employed by default. 
                       */
      double[] info,
      /* O: information regarding the minimization. Set to NULL if don't care
                      * info[0]= ||e||_2 at initial p.
                      * info[1-4]=[ ||e||_2, ||J^T e||_inf,  ||Dp||_2, mu/max[J^T J]_ii ], all computed at estimated p.
                      * info[5]= # iterations,
                      * info[6]=reason for terminating: 1 - stopped by small gradient J^T e
                      *                                 2 - stopped by small Dp
                      *                                 3 - stopped by itmax
                      *                                 4 - singular matrix. Restart from current p with increased mu 
                      *                                 5 - no further error reduction is possible. Restart with increased mu
                      *                                 6 - stopped by small ||e||_2
                      * info[7]= # function evaluations
                      * info[8]= # jacobian evaluations
                      */
      ref object workingmemory,     /* working memory, allocate if NULL */
      double[] covar,    /* O: Covariance matrix corresponding to LS solution; mxm. Set to NULL if not needed. */
      object adata)       /* pointer to possibly additional data, passed uninterpreted to func.
                      * Set to NULL if not needed
                      */
    {

      int m = p.Length;              /* I: parameter vector dimension (i.e. #unknowns) */
      int n = x.Length;              /* I: measurement vector dimension */
      int i, j, k, l;

      bool issolved;
      /* temp work arrays */
      double[] e;          /* nx1 */
      double[] hx;         /* \hat{x}_i, nx1 */

      double[] jacTe;      /* J^T e_i mx1 */

      double[] jac;        /* nxm */

      double[] jacTjac;    /* mxm */

      double[] Dp;         /* mx1 */

      double[] diag_jacTjac;   /* diagonal of J^T J, mx1 */

      double[] pDp;        /* p + Dp, mx1 */

      double[] wrk;        /* nx1 */

      bool using_ffdif=true;
      double[] wrk2=null; /* nx1, used for differentiating with central differences only */

      double mu,  /* damping constant */
        tmp; /* mainly used in matrix & vector multiplications */
      double p_eL2, jacTe_inf, pDp_eL2; /* ||e(p)||_2, ||J^T e||_inf, ||e(p+Dp)||_2 */
      double p_L2, Dp_L2=LM_REAL_MAX, dF, dL;
      double tau, eps1, eps2, eps2_sq, eps3, delta;
      double init_p_eL2;
      int nu, nu2, stop, nfev, njap=0, K=(m>=10)? m: 10, updjac;
      bool updp=true;
      bool  newjac;
      int nm=n*m;

      mu=jacTe_inf=p_L2=0.0; /* -Wall */
      stop = updjac = 0;  newjac = false; /* -Wall */

      if(n<m)
      {
        throw new ArithmeticException(string.Format("Cannot solve a problem with fewer measurements {0} than unknowns {1}", n, m));
      }

      if(opts!=null)
      {
        tau=opts[0];
        eps1=opts[1];
        eps2=opts[2];
        eps2_sq=opts[2]*opts[2];
        eps3=opts[3];
        delta=opts[4];
        if(delta<0.0)
        {
          delta=-delta; /* make positive */
          using_ffdif=false; /* use central differencing */
          wrk2 = new double[n];
        }
      }
      else
      { // use default values
        tau=LM_INIT_MU;
        eps1=LM_STOP_THRESH;
        eps2=LM_STOP_THRESH;
        eps2_sq=LM_STOP_THRESH*LM_STOP_THRESH;
        eps3=LM_STOP_THRESH;
        delta=LM_DIFF_DELTA;
      }

      WorkArrays work = workingmemory as WorkArrays;
      if(null==work)
      {
        work = new WorkArrays(n, m);
        workingmemory = work;
      }

      /* set up work arrays */
      e=work.e;
      hx = work.hx;
      jacTe = work.jacTe;
      jac = work.jac;
      jacTjac = work.jacTjac;
      Dp = work.Dp;
      diag_jacTjac = work.diag_jacTjac;
      pDp = work.pDp;
      wrk = work.wrk;
  
      /* compute e=x - f(p) and its L2 norm */
      func(p, hx, adata); nfev=1;
      for(i=0, p_eL2=0.0; i<n; ++i)
      {
        e[i]=tmp=x[i]-hx[i];
        p_eL2+=tmp*tmp;
      }
      init_p_eL2=p_eL2;

      nu=20; /* force computation of J */

      for(k=0; k<itmax; ++k)
      {
        /* Note that p and e have been updated at a previous iteration */

        if(p_eL2<=eps3)
        { /* error is small */
          stop=6;
          break;
        }

        /* Compute the jacobian J at p,  J^T J,  J^T e,  ||J^T e||_inf and ||p||^2.
         * The symmetry of J^T J is again exploited for speed
         */

        if((updp && nu>16) || updjac==K)
        { /* compute difference approximation to J */
          if(using_ffdif)
          { /* use forward differences */
            FDIF_FORW_JAC_APPROX(func, p, hx, wrk, delta, jac, m, n, adata);
            ++njap; nfev+=m;
          }
          else
          { /* use central differences */
            FDIF_CENT_JAC_APPROX(func, p, wrk, wrk2, delta, jac, m, n, adata);
            ++njap; nfev+=2*m;
          }
          nu=2; updjac=0; updp=false; newjac=true;
        }

        if(newjac)
        { /* jacobian has changed, recompute J^T J, J^t e, etc */
          newjac=false;

          /* J^T J, J^T e */
          if(nm<=__BLOCKSZ__SQ)
          { // this is a small problem
            /* This is the straightforward way to compute J^T J, J^T e. However, due to
             * its noncontinuous memory access pattern, it incures many cache misses when
             * applied to large minimization problems (i.e. problems involving a large
             * number of free variables and measurements), in which J is too large to
             * fit in the L1 cache. For such problems, a cache-efficient blocking scheme
             * is preferable.
             *
             * Thanks to John Nitao of Lawrence Livermore Lab for pointing out this
             * performance problem.
             *
             * On the other hand, the straightforward algorithm is faster on small
             * problems since in this case it avoids the overheads of blocking. 
             */
      
            for(i=0; i<m; ++i)
            {
              for(j=i; j<m; ++j)
              {
                int lm;

                for(l=0, tmp=0.0; l<n; ++l)
                {
                  lm=l*m;
                  tmp+=jac[lm+i]*jac[lm+j];
                }

                jacTjac[i*m+j]=jacTjac[j*m+i]=tmp;
              }

              /* J^T e */
              for(l=0, tmp=0.0; l<n; ++l)
                tmp+=jac[l*m+i]*e[l];
              jacTe[i]=tmp;
            }
          }
          else
          { // this is a large problem
            /* Cache efficient computation of J^T J based on blocking
             */
            TRANS_MAT_MAT_MULT(jac, jacTjac, n, m, __BLOCKSZ__,null);

            /* cache efficient computation of J^T e */
            for(i=0; i<m; ++i)
              jacTe[i]=0.0;

            for(i=0; i<n; ++i)
            {
              int jacrow;

              for(l=0, jacrow=i*m, tmp=e[i]; l<m; ++l)
                jacTe[l]+=jac[l+jacrow]*tmp;
            }
          }
      
          /* Compute ||J^T e||_inf and ||p||^2 */
          for(i=0, p_L2=jacTe_inf=0.0; i<m; ++i)
          {
            if(jacTe_inf < (tmp=Math.Abs(jacTe[i]))) jacTe_inf=tmp;

            diag_jacTjac[i]=jacTjac[i*m+i]; /* save diagonal entries so that augmentation can be later canceled */
            p_L2+=p[i]*p[i];
          }
          //p_L2=sqrt(p_L2);
        }

#if false
if(!(k%100)){
  printf("Current estimate: ");
  for(i=0; i<m; ++i)
    printf("%.9g ", p[i]);
  printf("-- errors %.9g %0.9g\n", jacTe_inf, p_eL2);
}
#endif

        /* check for convergence */
        if((jacTe_inf <= eps1))
        {
          Dp_L2=0.0; /* no increment for p in this case */
          stop=1;
          break;
        }

        /* compute initial damping factor */
        if(k==0)
        {
          for(i=0, tmp=LM_REAL_MIN; i<m; ++i)
            if(diag_jacTjac[i]>tmp) tmp=diag_jacTjac[i]; /* find max diagonal element */
          mu=tau*tmp;
        }

        /* determine increment using adaptive damping */

        /* augment normal equations */
        for(i=0; i<m; ++i)
          jacTjac[i*m+i]+=mu;

        /* solve augmented equations */
#if HAVE_LAPACK
    /* 5 alternatives are available: LU, Cholesky, 2 variants of QR decomposition and SVD.
     * Cholesky is the fastest but might be inaccurate; QR is slower but more accurate;
     * SVD is the slowest but most accurate; LU offers a tradeoff between accuracy and speed
     */

    issolved=AX_EQ_B_LU(jacTjac, jacTe, Dp, m);
    //issolved=AX_EQ_B_CHOL(jacTjac, jacTe, Dp, m);
    //issolved=AX_EQ_B_QR(jacTjac, jacTe, Dp, m);
    //issolved=AX_EQ_B_QRLS(jacTjac, jacTe, Dp, m, m);
    //issolved=AX_EQ_B_SVD(jacTjac, jacTe, Dp, m);
#else
        /* use the LU included with levmar */
        issolved=0!=AX_EQ_B_LU(jacTjac, jacTe, Dp, m);
#endif // HAVE_LAPACK 

        if(issolved)
        {
          /* compute p's new estimate and ||Dp||^2 */
          for(i=0, Dp_L2=0.0; i<m; ++i)
          {
            pDp[i]=p[i] + (tmp=Dp[i]);
            Dp_L2+=tmp*tmp;
          }
          //Dp_L2=sqrt(Dp_L2);

          if(Dp_L2<=eps2_sq*p_L2)
          { /* relative change in p is small, stop */
            //if(Dp_L2<=eps2*(p_L2 + eps2)){ /* relative change in p is small, stop */
            stop=2;
            break;
          }

          if(Dp_L2>=(p_L2+eps2)/(EPSILON*EPSILON))
          { /* almost singular */
            //if(Dp_L2>=(p_L2+eps2)/CNST(EPSILON)){ /* almost singular */
            stop=4;
            break;
          }

          func(pDp, wrk, adata); ++nfev; /* evaluate function at p + Dp */
          for(i=0, pDp_eL2=0.0; i<n; ++i)
          { /* compute ||e(pDp)||_2 */
            tmp=x[i]-wrk[i];
            pDp_eL2+=tmp*tmp;
          }

          dF=p_eL2-pDp_eL2;
          if(updp || dF>0)
          { /* update jac */
            for(i=0; i<n; ++i)
            {
              for(l=0, tmp=0.0; l<m; ++l)
                tmp+=jac[i*m+l]*Dp[l]; /* (J * Dp)[i] */
              tmp=(wrk[i] - hx[i] - tmp)/Dp_L2; /* (f(p+dp)[i] - f(p)[i] - (J * Dp)[i])/(dp^T*dp) */
              for(j=0; j<m; ++j)
                jac[i*m+j]+=tmp*Dp[j];
            }
            ++updjac;
            newjac=true;
          }

          for(i=0, dL=0.0; i<m; ++i)
            dL+=Dp[i]*(mu*Dp[i]+jacTe[i]);

          if(dL>0.0 && dF>0.0)
          { /* reduction in error, increment is accepted */
            dF=(2.0*dF/dL-1.0);
            tmp=dF*dF*dF;
            tmp=1.0-tmp*tmp*dF;
            mu=mu*( (tmp>=ONE_THIRD)? tmp : ONE_THIRD );
            nu=2;

            for(i=0 ; i<m; ++i) /* update p's estimate */
              p[i]=pDp[i];

            for(i=0; i<n; ++i)
            { /* update e, hx and ||e||_2 */
              e[i]=x[i]-wrk[i];
              hx[i]=wrk[i];
            }
            p_eL2=pDp_eL2;
            updp=true;
            continue;
          }
        }

        /* if this point is reached, either the linear system could not be solved or
         * the error did not reduce; in any case, the increment must be rejected
         */

        mu*=nu;
        nu2=nu<<1; // 2*nu;
        if(nu2<=nu)
        { /* nu has wrapped around (overflown). Thanks to Frank Jordan for spotting this case */
          stop=5;
          break;
        }
        nu=nu2;

        for(i=0; i<m; ++i) /* restore diagonal J^T J entries */
          jacTjac[i*m+i]=diag_jacTjac[i];
      }

      if(k>=itmax) stop=3;

      for(i=0; i<m; ++i) /* restore diagonal J^T J entries */
        jacTjac[i*m+i]=diag_jacTjac[i];

      if(info!=null)
      {
        info[0]=init_p_eL2;
        info[1]=p_eL2;
        info[2]=jacTe_inf;
        info[3]=Dp_L2;
        for(i=0, tmp=LM_REAL_MIN; i<m; ++i)
          if(tmp<jacTjac[i*m+i]) tmp=jacTjac[i*m+i];
        info[4]=mu/tmp;
        info[5]=(double)k;
        info[6]=(double)stop;
        info[7]=(double)nfev;
        info[8]=(double)njap;
      }

      /* covariance matrix */
      if(covar!=null)
      {
        LEVMAR_COVAR(jacTjac, covar, p_eL2, m, n);
      }

      return (stop!=4)?  k : -1;
    }

    /* forward finite difference approximation to the jacobian of func */
    static void FDIF_FORW_JAC_APPROX(
      FitFunction func,
      double[] p,              /* I: current parameter estimate, mx1 */
      double[] hx,             /* I: func evaluated at p, i.e. hx=func(p), nx1 */
      double[] hxx,            /* W/O: work array for evaluating func(p+delta), nx1 */
      double delta,           /* increment for computing the jacobian */
      double[] jac,            /* O: array for storing approximated jacobian, nxm */
      int m,
      int n,
      object adata)
    {
      int i, j;
      double tmp;
      double d;

      for(j=0; j<m; ++j)
      {
        /* determine d=max(1E-04*|p[j]|, delta), see HZ */
        d=(1E-04)*p[j]; // force evaluation
        d=Math.Abs(d);
        if(d<delta)
          d=delta;

        tmp=p[j];
        p[j]+=d;

        func(p, hxx, adata);

        p[j]=tmp; /* restore */

        d=(1.0)/d; /* invert so that divisions can be carried out faster as multiplications */
        for(i=0; i<n; ++i)
        {
          jac[i*m+j]=(hxx[i]-hx[i])*d;
        }
      }
    }

    
    /* central finite difference approximation to the jacobian of func */
    static void FDIF_CENT_JAC_APPROX(
      FitFunction func,
      /* function to differentiate */
      double[] p,              /* I: current parameter estimate, mx1 */
      double[] hxm,            /* W/O: work array for evaluating func(p-delta), nx1 */
      double[] hxp,            /* W/O: work array for evaluating func(p+delta), nx1 */
      double delta,           /* increment for computing the jacobian */
      double[] jac,            /* O: array for storing approximated jacobian, nxm */
      int m,
      int n,
      object adata)
    {
      int i, j;
      double tmp;
      double d;

      for(j=0; j<m; ++j)
      {
        /* determine d=max(1E-04*|p[j]|, delta), see HZ */
        d=(1E-04)*p[j]; // force evaluation
        d=(d);
        if(d<delta)
          d=delta;

        tmp=p[j];
        p[j]-=d;
        func(p, hxm, adata);

        p[j]=tmp+d;
        func(p, hxp, adata);
        p[j]=tmp; /* restore */

        d=(0.5)/d; /* invert so that divisions can be carried out faster as multiplications */
        for(i=0; i<n; ++i)
        {
          jac[i*m+j]=(hxp[i]-hxm[i])*d;
        }
      }
    }

    /* blocked multiplication of the transpose of the nxm matrix a with itself (i.e. a^T a)
 * using a block size of bsize. The product is returned in b.
 * Since a^T a is symmetric, its computation can be speeded up by computing only its
 * upper triangular part and copying it to the lower part.
 *
 * More details on blocking can be found at 
 * http://www-2.cs.cmu.edu/afs/cs/academic/class/15213-f02/www/R07/section_a/Recitation07-SectionA.pdf
 */
    static void TRANS_MAT_MAT_MULT(double[] a,  double[] b, int n, int m, int bsize, double[] weights)
    {
      int i, j, k, jj, kk;
      double sum;
      int bim;
      int akm;

      /* compute upper triangular part using blocking */
      for(jj=0; jj<m; jj+=bsize)
      {
        for(i=0; i<m; ++i)
        {
          bim=i*m;
          for(j=Math.Max(jj, i); j<Math.Min(jj+bsize, m); ++j)
            b[bim+j]=0.0; //b[i*m+j]=0.0;
        }

        for(kk=0; kk<n; kk+=bsize)
        {
          for(i=0; i<m; ++i)
          {
            bim=i*m;
            for(j=Math.Max(jj, i); j<Math.Min(jj+bsize, m); ++j)
            {
              sum=0.0;
              if (null == weights)
              {
                for (k = kk; k < Math.Min(kk + bsize, n); ++k)
                {
                  akm = k * m;
                  sum += a[akm + i] * a[akm + j]; //a[k*m+i]*a[k*m+j];
                }
              }
              else
              {
                for (k = kk; k < Math.Min(kk + bsize, n); ++k)
                {
                  akm = k * m;
                  sum += a[akm + i] * a[akm + j] * weights[k]*weights[k]; //a[k*m+i]*a[k*m+j];
                }
              }
              b[bim+j]+=sum; //b[i*m+j]+=sum;
            }
          }
        }
      }

      /* copy upper triangular part to the lower one */
      for(i=0; i<m; ++i)
        for(j=0; j<i; ++j)
          b[i*m+j]=b[j*m+i];


    }


    /*
 * This function computes in C the covariance matrix corresponding to a least
 * squares fit. JtJ is the approximate Hessian at the solution (i.e. J^T*J, where
 * J is the jacobian at the solution), sumsq is the sum of squared residuals
 * (i.e. goodnes of fit) at the solution, m is the number of parameters (variables)
 * and n the number of observations. JtJ can coincide with C.
 * 
 * if JtJ is of full rank, C is computed as sumsq/(n-m)*(JtJ)^-1
 * otherwise and if LAPACK is available, C=sumsq/(n-r)*(JtJ)^+
 * where r is JtJ's rank and ^+ denotes the pseudoinverse
 * The diagonal of C is made up from the estimates of the variances
 * of the estimated regression coefficients.
 * See the documentation of routine E04YCF from the NAG fortran lib
 *
 * The function returns the rank of JtJ if successful, 0 on error
 *
 * A and C are mxm
 *
 */
    static int LEVMAR_COVAR(double[] JtJ, double[] C, double sumsq, int m, int n)
    {
      int i;
      int rnk;
      double fact;

#if HAVE_LAPACK
   rnk=LEVMAR_PSEUDOINVERSE(JtJ, C, m);
   if(!rnk) return 0;
#else

#warning LAPACK not available, LU will be used for matrix inversion when computing the covariance; this might be unstable at times

      rnk=LEVMAR_LUINVERSE(JtJ, C, m);
      if(rnk==0) return 0;

      rnk=m; /* assume full rank */
#endif // HAVE_LAPACK

      fact=sumsq/(double)(n-rnk);
      for(i=0; i<m*m; ++i)
        C[i]*=fact;

      return rnk;
    }

    /*
    * This function computes the inverse of A in B. A and B can coincide
    *
    * The function employs LAPACK-free LU decomposition of A to solve m linear
    * systems A*B_i=I_i, where B_i and I_i are the i-th columns of B and I.
    *
    * A and B are mxm
    *
    * The function returns 0 in case of error,
    * 1 if successfull
    *
    */
    static int LEVMAR_LUINVERSE(double[] A, double[] B, int m)
    {

      int i, j, k, l;
      int [] idx;
      int maxi=-1, idx_sz, a_sz, x_sz, work_sz ;
      double []a;
      double []x;
      double []work;
      double max, sum, tmp;

      /* calculate required memory size */
      idx_sz=m;
      a_sz=m*m;
      x_sz=m;
      work_sz=m;

      idx= new int[idx_sz];
      a= new double[a_sz];
      x= new double[x_sz];
      work = new double[work_sz];

      /* avoid destroying A by copying it to a */
      for(i=0; i<a_sz; ++i) a[i]=A[i];

      /* compute the LU decomposition of a row permutation of matrix a; the permutation itself is saved in idx[] */
      for(i=0; i<m; ++i)
      {
        max=0.0;
        for(j=0; j<m; ++j)
          if((tmp=Math.Abs(a[i*m+j]))>max)
            max=tmp;
        if(max==0.0)
        {
          // throw new ArithmeticException("Singular matrix A !");
          return 0;
        }
        work[i]=(1.0)/max;
      }

      for(j=0; j<m; ++j)
      {
        for(i=0; i<j; ++i)
        {
          sum=a[i*m+j];
          for(k=0; k<i; ++k)
            sum-=a[i*m+k]*a[k*m+j];
          a[i*m+j]=sum;
        }
        max=0.0;
        for(i=j; i<m; ++i)
        {
          sum=a[i*m+j];
          for(k=0; k<j; ++k)
            sum-=a[i*m+k]*a[k*m+j];
          a[i*m+j]=sum;
          if((tmp=work[i]*Math.Abs(sum))>=max)
          {
            max=tmp;
            maxi=i;
          }
        }
        if(j!=maxi)
        {
          for(k=0; k<m; ++k)
          {
            tmp=a[maxi*m+k];
            a[maxi*m+k]=a[j*m+k];
            a[j*m+k]=tmp;
          }
          work[maxi]=work[j];
        }
        idx[j]=maxi;
        if(a[j*m+j]==0.0)
          a[j*m+j]=LM_REAL_EPSILON;
        if(j!=m-1)
        {
          tmp=(1.0)/(a[j*m+j]);
          for(i=j+1; i<m; ++i)
            a[i*m+j]*=tmp;
        }
      }

      /* The decomposition has now replaced a. Solve the m linear systems using
       * forward and back substitution
       */
      for(l=0; l<m; ++l)
      {
        for(i=0; i<m; ++i) x[i]=0.0;
        x[l]=(1.0);

        for(i=k=0; i<m; ++i)
        {
          j=idx[i];
          sum=x[j];
          x[j]=x[i];
          if(k!=0)
            for(j=k-1; j<i; ++j)
              sum-=a[i*m+j]*x[j];
          else
            if(sum!=0.0)
            k=i+1;
          x[i]=sum;
        }

        for(i=m-1; i>=0; --i)
        {
          sum=x[i];
          for(j=i+1; j<m; ++j)
            sum-=a[i*m+j]*x[j];
          x[i]=sum/a[i*m+i];
        }

        for(i=0; i<m; ++i)
          B[i*m+l]=x[i];
      }

 

      return 1;
    }


    /*
 * This function returns the solution of Ax = b
 *
 * The function employs LU decomposition followed by forward/back substitution (see 
 * also the LAPACK-based LU solver above)
 *
 * A is mxm, b is mx1
 *
 * The function returns 0 in case of error,
 * 1 if successfull
 *
 * This function is often called repetitively to solve problems of identical
 * dimensions. To avoid repetitive malloc's and free's, allocated memory is
 * retained between calls and free'd-malloc'ed when not of the appropriate size.
 * A call with NULL as the first argument forces this memory to be released.
 */
    static int AX_EQ_B_LU(double[] A, double[] B, double[] x, int m)
    {

      int i, j, k;
      int []idx;
      int maxi=-1, idx_sz, a_sz, work_sz;
      double[]a; double[] work;
      double max, sum, tmp;
   
      /* calculate required memory size */
      idx_sz=m;
      a_sz=m*m;
      work_sz=m;

      // TODO LelliD make sure to retain the memory allocated here for subsequent calls
      // with the same dimensions
      idx = new int[idx_sz];
      a = new double[a_sz];
      work= new double[work_sz];

      /* avoid destroying A, B by copying them to a, x resp. */
      for(i=0; i<m; ++i)
      { // B & 1st row of A
        a[i]=A[i];
        x[i]=B[i];
      }
      for(  ; i<a_sz; ++i) a[i]=A[i]; // copy A's remaining rows
      /****
      for(i=0; i<m; ++i){
        for(j=0; j<m; ++j)
          a[i*m+j]=A[i*m+j];
        x[i]=B[i];
      }
      ****/

      /* compute the LU decomposition of a row permutation of matrix a; the permutation itself is saved in idx[] */
      for(i=0; i<m; ++i)
      {
        max=0.0;
        for(j=0; j<m; ++j)
          if((tmp=Math.Abs(a[i*m+j]))>max)
            max=tmp;
        if(max==0.0)
        {
          throw new ArithmeticException("Singular matrix A !");
        }
        work[i]=(1.0)/max;
      }

      for(j=0; j<m; ++j)
      {
        for(i=0; i<j; ++i)
        {
          sum=a[i*m+j];
          for(k=0; k<i; ++k)
            sum-=a[i*m+k]*a[k*m+j];
          a[i*m+j]=sum;
        }
        max=0.0;
        for(i=j; i<m; ++i)
        {
          sum=a[i*m+j];
          for(k=0; k<j; ++k)
            sum-=a[i*m+k]*a[k*m+j];
          a[i*m+j]=sum;
          if((tmp=work[i]*Math.Abs(sum))>=max)
          {
            max=tmp;
            maxi=i;
          }
        }
        if(j!=maxi)
        {
          for(k=0; k<m; ++k)
          {
            tmp=a[maxi*m+k];
            a[maxi*m+k]=a[j*m+k];
            a[j*m+k]=tmp;
          }
          work[maxi]=work[j];
        }
        idx[j]=maxi;
        if(a[j*m+j]==0.0)
          a[j*m+j]=LM_REAL_EPSILON;
        if(j!=m-1)
        {
          tmp=(1.0)/(a[j*m+j]);
          for(i=j+1; i<m; ++i)
            a[i*m+j]*=tmp;
        }
      }

      /* The decomposition has now replaced a. Solve the linear system using
       * forward and back substitution
       */
      for(i=k=0; i<m; ++i)
      {
        j=idx[i];
        sum=x[j];
        x[j]=x[i];
        if(k!=0)
          for(j=k-1; j<i; ++j)
            sum-=a[i*m+j]*x[j];
        else
          if(sum!=0.0)
          k=i+1;
        x[i]=sum;
      }

      for(i=m-1; i>=0; --i)
      {
        sum=x[i];
        for(j=i+1; j<m; ++j)
          sum-=a[i*m+j]*x[j];
        x[i]=sum/a[i*m+i];
      }



      return 1;
    }


    /*
    * This function computes the pseudoinverse of a square matrix A
    * into B using SVD. A and B can coincide
    * 
    * The function returns 0 in case of error (e.g. A is singular),
    * the rank of A if successfull
    *
    * A, B are mxm
    *
    */
    static int LEVMAR_PSEUDOINVERSE(double[] A, double[] B, int m)
    {

      IROMatrix Amat = MatrixMath.ToROMatrix(A,m);
      DoubleSVDDecomp decomp = new DoubleSVDDecomp(Amat);

      DoubleVector s = decomp.S;
      DoubleMatrix u = decomp.U;
      DoubleMatrix v = decomp.V;

      /* compute the pseudoinverse in B */
      for(int i=0; i<B.Length; i++)
        B[i]=0.0; /* initialize to zero */

      double one_over_denom, thresh;
      int rank;
      for(rank=0, thresh=DoubleConstants.DBL_EPSILON*s[0]; rank<m && s[rank]>thresh; rank++)
      {
        one_over_denom=1.0/s[rank];

        for(int j=0; j<m; j++)
          for(int i=0; i<m; i++)
            B[i*m+j]+=v[i,rank]*u[j,rank]*one_over_denom;
      }

      return rank;
    }


  }
}
