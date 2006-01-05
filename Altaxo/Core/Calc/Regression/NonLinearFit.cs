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


/*-----------------------------------------------------------------------------*\
| LevenbergMarquardtFit()                                            nlinfit.cc |
|                                                                               |
| general multi-dimensional non-linear least-squares fit                        |
| using the Levenberg-Marquardt algorithm                                       |
|                                                                               |
| Last change: Jun 22, 2001             |
|                                                                               |
| Matpack Library Release 1.7.2                                                 |
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

//#include "matpack.h"

//
// The subsequent routines are a port of several MINPACK routines 
// comprising the Levenberg-Marquardt algorithm for a general
// multidimensional nonlinear least square fit from 
// old Fortran 77 to modern C++ !
//
// originally written by
//   Argonne National Laboratory. Minpack Project. March 1980. 
//   Burton S. Garbow, Kenneth E. Hillstrom, Jorge J. More 
//
//

// TODO: 
//
//   should use: typedef void (*LMFunction) (Vector&,Vector&,int&); 
//   the integer arrays iwa,ipvt should be replaced by IntVector();
//   all double auxilliary arrays should be replaced by Vector();
// 


// prototypes
// ----------

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Functions for nonlinear minimization.
  /// </summary>
  public class NLFit
  {
    const double DBL_EPSILON=2.2204460492503131e-016;
    public static double sqr(double x)  { return x*x; }


    /// <summary>
    /// User-supplied subroutine which calculates the functions to minimize.
    /// Calculates <c>numberOfYs</c> functions dependent on <c>numberOfParameter</c> parameters and
    /// returns the calculated y values in array <c>ys</c>. The value of <c>info</c> should
    /// not be changed unless  the user wants to terminate execution of LevenbergMarquardtFit. 
    /// In this case set iflag to a negative integer. 
    /// </summary>
    public delegate void LMFunction(
      int numberOfYs, 
      int numberOfParameter,
      double[] parameter,
      double[] ys,
      ref int info);


    /// <summary>
    /// The purpose of LevenbergMarquardtFit is to minimize the sum of the 
    /// squares of m nonlinear functions in n variables by a modification of the 
    /// Levenberg-Marquardt algorithm. This is done by using the more 
    /// general least-squares solver below. The user must provide a 
    /// subroutine which calculates the functions. The Jacobian is 
    /// then calculated by a forward-difference approximation. 
    /// </summary>
    /// <param name="fcn">The user supplied function which provides the values to minimize.</param>
    /// <param name="xvec">
    /// Array of length n containing the parameter vector. On input x must contain 
    /// an initial estimate of the solution vector. On output x 
    /// contains the final estimate of the solution vector. 
    /// </param>
    /// <param name="fvec">Output array of length m which contains the functions evaluated at the output x. </param>
    /// <param name="tol">
    /// Nonnegative input variable. Termination occurs 
    /// when the algorithm estimates either that the relative 
    /// error in the sum of squares is at most tol or that 
    /// the relative error between x and the solution is at 
    /// most tol. 
    /// </param>
    /// <param name="info">
    /// Info is an integer output variable. If the user has 
    /// terminated execution, info is set to the (negative) 
    /// value of iflag. See description of fcn. Otherwise, 
    /// info is set as follows: 
    ///
    ///         info = 0  improper input parameters. 
    ///
    ///         info = 1  algorithm estimates that the relative error 
    ///                   in the sum of squares is at most tol. 
    ///
    ///         info = 2  algorithm estimates that the relative error 
    ///                   between x and the solution is at most tol. 
    ///
    ///         info = 3  conditions for info = 1 and info = 2 both hold. 
    ///
    ///         info = 4  fvec is orthogonal to the columns of the 
    ///                   Jacobian to machine precision. 
    ///
    ///         info = 5  number of calls to fcn has reached or 
    ///                   exceeded 200*(n+1). 
    ///
    ///         info = 6  tol is too small. No further reduction in 
    ///                   the sum of squares is possible. 
    ///
    ///         info = 7  tol is too small. No further improvement in 
    ///                   the approximate solution x is possible. 
    /// </param>
    /// <remarks>
    /// This is the most easy-to-use interface with the smallest number of
    /// arguments. If you need more control over the minimization process and
    /// auxilliary storage allocation you should use one of the interfaces 
    /// described below.
    /// </remarks>
    public static void LevenbergMarquardtFit(
      LMFunction fcn, 
      double[] xvec, 
      double[] fvec, 
      double tol, 
      ref int info)

    {
      int m,n;
      int[] iwa;
    
      m = fvec.Length;       // number of functions
      n = xvec.Length;       // number of variables

      // allocate working arrays
      // lwa = m*n+5*n+m;
      iwa = new int[n];
      // wa  = new double[lwa];
      double[] diag = new double[n];
      double[] fjac = new double[n*m];
      int[] ipvt = new int[n];
      double[] qtf = new double[n];
      double[] wa1 = new double[n];
      double[] wa2 = new double[n];
      double[] wa3 = new double[n];
      double[] wa4 = new double[m];




      LevenbergMarquardtFit (fcn,xvec,fvec,tol,ref info,iwa, diag, fjac, ipvt, qtf, wa1,
        wa2, wa3,wa4); 

    }


    
    
    /// <summary>
    /// The purpose of LevenbergMarquardtFit is to minimize the sum of the 
    /// squares of m nonlinear functions in n variables by a modification of the 
    /// Levenberg-Marquardt algorithm. This is done by using the more 
    /// general least-squares solver below. The user must provide a 
    /// subroutine which calculates the functions. The Jacobian is 
    /// then calculated by a forward-difference approximation. 
    /// </summary>
    /// <param name="fcn">The user supplied function which provides the values to minimize.</param>
    /// <param name="xvec">
    /// Array of length n containing the parameter vector. On input x must contain 
    /// an initial estimate of the solution vector. On output x 
    /// contains the final estimate of the solution vector. 
    /// </param>
    /// <param name="fvec">Output array of length m which contains the functions evaluated at the output x. </param>
    /// <param name="tol">
    /// Nonnegative input variable. Termination occurs 
    /// when the algorithm estimates either that the relative 
    /// error in the sum of squares is at most tol or that 
    /// the relative error between x and the solution is at 
    /// most tol. 
    /// </param>
    /// <param name="info">
    /// Info is an integer output variable. If the user has 
    /// terminated execution, info is set to the (negative) 
    /// value of iflag. See description of fcn. Otherwise, 
    /// info is set as follows: 
    ///
    ///         info = 0  improper input parameters. 
    ///
    ///         info = 1  algorithm estimates that the relative error 
    ///                   in the sum of squares is at most tol. 
    ///
    ///         info = 2  algorithm estimates that the relative error 
    ///                   between x and the solution is at most tol. 
    ///
    ///         info = 3  conditions for info = 1 and info = 2 both hold. 
    ///
    ///         info = 4  fvec is orthogonal to the columns of the 
    ///                   Jacobian to machine precision. 
    ///
    ///         info = 5  number of calls to fcn has reached or 
    ///                   exceeded 200*(n+1). 
    ///
    ///         info = 6  tol is too small. No further reduction in 
    ///                   the sum of squares is possible. 
    ///
    ///         info = 7  tol is too small. No further improvement in 
    ///                   the approximate solution x is possible. 
    /// </param>
    /// <param name="iwa">Integer working array of length n.</param>
    /// <param name="diag"></param>
    /// <param name="fjac"></param>
    /// <param name="ipvt"></param>
    /// <param name="qtf"></param>
    /// <param name="wa1"></param>
    /// <param name="wa2"></param>
    /// <param name="wa3"></param>
    /// <param name="wa4"></param>
    /// <remarks>
    /// This is the most easy-to-use interface with the smallest number of
    /// arguments. If you need more control over the minimization process and
    /// auxilliary storage allocation you should use one of the interfaces 
    /// described below.
    /// </remarks>
    public static void LevenbergMarquardtFit(
      LMFunction fcn,
      double[] xvec,
      double[] fvec, 
      double tol,
      ref int info,
      int[] iwa,
      double[] diag,
      double[] fjac, 
      int[] ipvt,
      double[] qtf, 
      double[] wa1, 
      double[] wa2,
      double[] wa3,
      double[] wa4)
      //
      //       iwa is an integer work array of length n. 
      //
      //       wa is a work array of length lwa. 
      //
      //       lwa is a positive integer input variable not less than 
      //         m*n+5*n+m. 
      //
      //
    {
      int mode, nfev, maxfev, nprint;
      double factor, ftol, gtol, xtol, epsfcn;
    
      // Parameter adjustments
      int m,n;
      m = fvec.Length;       // number of functions
      n = xvec.Length;       // number of variables

      // --wa;
      // --iwa;

      info = 0;
      nfev = 0;

      // check the input parameters for errors
      if (n <= 0 || m < n || tol < 0 ) return;

      factor = 100;
      maxfev = (n + 1) * 200;
      ftol = tol;
      xtol = tol;
      gtol = 0;
      epsfcn = 0;
      mode = 1;
      nprint = 0;

      LevenbergMarquardtFit(fcn, xvec, fvec, ftol, xtol, gtol, maxfev, epsfcn,
        diag, mode, factor, nprint, ref info, ref nfev, fjac, m,
        ipvt, qtf, wa1, wa2, wa3,
        wa4);
    
      if (info == 8) 
        info = 4;
    } 


    public static void  LevenbergMarquardtFit (LMFunction fcn, double[] xvec, double[] fvec, 
      double ftol, double xtol, double gtol,
      int maxfev, double epsfcn, double[] diag, int mode, 
      double factor, int nprint, ref int info, ref int nfev, double[] fjac, 
      int ldfjac, int[] ipvt,
      double[] qtf, double[] wa1, double[] wa2, double[] wa3, double[] wa4)
      //
      // The purpose of LevenbergMarquardtFit is to minimize the sum of the 
      // squares of m nonlinear functions in n variables by a modification of 
      // the Levenberg-Marquardt algorithm. The user must provide a 
      // subroutine which calculates the functions. The Jacobian is 
      // then calculated by a forward-difference approximation. 
      //
      // This is the most general interface to the Levenberg-Marquardt algorithm
      // which gives you full control over the minimization process and auxilliary
      // storage allocation. Use one of the simpler interfaces above, if you don't 
      // all arguments.
      //
      // Arguments:
      //
      //       fcn is the name of the user-supplied subroutine which 
      //         calculates the functions. fcn should be written as follows: 
      //
      //         void fcn (int m, int n, double* xvec, double* fvec, int& iflag)
      //         {
      //            ...
      //            calculate the functions at xvec[0..n-1] and 
      //            return this vector in fvec[0..m-1]. 
      //            ...
      //         }
      //         
      //         The value of iflag should not be changed by fcn unless 
      //         the user wants to terminate execution of LevenbergMarquardtFit. 
      //         In this case set iflag to a negative integer. 
      //         
      //       xvec is an array of length n. On input x must contain 
      //         an initial estimate of the solution vector. On output x 
      //         contains the final estimate of the solution vector. 
      //
      //       fvec is an output array of length m which contains 
      //         the functions evaluated at the output x. 
      //
      //       ftol is a nonnegative input variable. Termination 
      //         occurs when both the actual and predicted relative 
      //         reductions in the sum of squares are at most ftol. 
      //         therefore, ftol measures the relative error desired 
      //         in the sum of squares. 
      //
      //       xtol is a nonnegative input variable. Termination 
      //         occurs when the relative error between two consecutive 
      //         iterates is at most xtol. Therefore, xtol measures the 
      //         relative error desired in the approximate solution. 
      //
      //       gtol is a nonnegative input variable. Termination 
      //         occurs when the cosine of the angle between fvec and 
      //         any column of the Jacobian is at most gtol in absolute 
      //         value. therefore, gtol measures the orthogonality 
      //         desired between the function vector and the columns 
      //         of the Jacobian. 
      //
      //       maxfev is a positive integer input variable. Termination 
      //         occurs when the number of calls to fcn is at least 
      //         maxfev by the end of an iteration. 
      //
      //       epsfcn is an input variable used in determining a suitable 
      //         step length for the forward-difference approximation. This 
      //         approximation assumes that the relative errors in the 
      //         functions are of the order of epsfcn. If epsfcn is less 
      //         than the machine precision, it is assumed that the relative 
      //         errors in the functions are of the order of the machine 
      //         precision. 
      //
      //       diag is an array of length n. If mode = 1 (see 
      //         below), diag is internally set. If mode = 2, diag 
      //         must contain positive entries that serve as 
      //         multiplicative scale factors for the variables. 
      //
      //       mode is an integer input variable. If mode = 1, the 
      //         variables will be scaled internally. If mode = 2, 
      //         the scaling is specified by the input diag. Other 
      //         values of mode are equivalent to mode = 1. 
      //
      //       factor is a positive input variable used in determining the 
      //         initial step bound. This bound is set to the product of 
      //         factor and the euclidean norm of diag*x if nonzero, or else 
      //         to factor itself. In most cases factor should lie in the 
      //         interval (0.1,100.0). 100.0 is a generally recommended value. 
      //
      //       nprint is an integer input variable that enables controlled 
      //         printing of iterates if it is positive. In this case, 
      //         fcn is called with iflag = 0 at the beginning of the first 
      //         iteration and every nprint iterations thereafter and 
      //         immediately prior to return, with x and fvec available 
      //         for printing. if nprint is not positive, no special calls 
      //         of fcn with iflag = 0 are made. 
      //
      //       info is an integer output variable. If the user has 
      //         terminated execution, info is set to the (negative) 
      //         value of iflag. see description of fcn. Otherwise, 
      //         info is set as follows:
      //
      //         info = 0  improper input parameters. 
      //
      //         info = 1  both actual and predicted relative reductions 
      //                   in the sum of squares are at most ftol. 
      //
      //         info = 2  relative error between two consecutive iterates 
      //                   is at most xtol. 
      //
      //         info = 3  conditions for info = 1 and info = 2 both hold. 
      //
      //         info = 4  the cosine of the angle between fvec and any 
      //                   column of the Jacobian is at most gtol in 
      //                   absolute value. 
      //
      //         info = 5  number of calls to fcn has reached or 
      //                   exceeded maxfev. 
      //
      //         info = 6  ftol is too small. No further reduction in 
      //                   the sum of squares is possible. 
      //
      //         info = 7  xtol is too small. No further improvement in 
      //                   the approximate solution x is possible. 
      //
      //         info = 8  gtol is too small. fvec is orthogonal to the 
      //                   columns of the Jacobian to machine precision. 
      //
      //       nfev is an integer output variable set to the number of 
      //         calls to fcn. 
      //
      //       fjac is an output m by n array. the upper n by n submatrix 
      //         of fjac contains an upper triangular matrix r with 
      //         diagonal elements of nonincreasing magnitude such that 
      //
      //                t     t           t 
      //               p *(jac *jac)*p = r *r, 
      //
      //         where p is a permutation matrix and jac is the final 
      //         calculated Jacobian. column j of p is column ipvt(j) 
      //         (see below) of the identity matrix. the lower trapezoidal 
      //         part of fjac contains information generated during 
      //         the computation of r. 
      //
      //       ldfjac is a positive integer input variable not less than m 
      //         which specifies the leading dimension of the array fjac. 
      //
      //       ipvt is an integer output array of length n. ipvt 
      //         defines a permutation matrix p such that jac*p = q*r, 
      //         where jac is the final calculated jacobian, q is 
      //         orthogonal (not stored), and r is upper triangular 
      //         with diagonal elements of nonincreasing magnitude. 
      //         column j of p is column ipvt(j) of the identity matrix. 
      //
      //       qtf is an output array of length n which contains 
      //         the first n elements of the vector (q transpose)*fvec. 
      //
      //       wa1, wa2, and wa3 are work arrays of length n. 
      //
      //       wa4 is a work array of length m. 
      //
    {

      const double p1 = 0.1;
      const double p5 = 0.5;
      const double p25 = 0.25;
      const double p75 = 0.75;
      const double p0001 = 1e-4;
    
      int fjac_dim1, fjac_offset;

      int iter;
      double temp=0.0, temp1, temp2;
      int i, j, l, iflag;
      double delta=0; // LELLID!! TODO  there was no initialization of delta in the original code
      double ratio;
      double fnorm, gnorm;
      double pnorm, xnorm=0.0, fnorm1, actred, dirder, prered;
      double par, sum;

        
      // Parameter adjustments

      int m,n;
      m = fvec.Length;  // number of functions
      n = xvec.Length;  // number of variables

      double[] x = xvec;
      double[] f = fvec;
      //    x = &xvec[ xvec.Lo()-1 ];   // 1-offset solution variables vector
      //    f = &fvec[ fvec.Lo()-1 ];   // 1-offset functions vector

      //--wa4;
      //--wa3;
      //--wa2;
      //--wa1;
      //--qtf;
      //--ipvt;
      fjac_dim1 = ldfjac;
      fjac_offset = fjac_dim1 + 1;
      //fjac -= fjac_offset;
      //--diag;

      info =  iflag =  nfev = 0;

      // check the input parameters for errors
      if (n <= 0 || m < n || ldfjac < m || ftol < 0.0 || xtol < 0.0 ||
        gtol < 0.0 || maxfev <= 0 || factor <= 0.0) 
        goto L300;
   
      if (mode != 2) goto L20;
    
      for (j = 0; j < n; j++) // LELLID!! 
        if (diag[j] <= 0.0) goto L300;
    
      L20:

        // evaluate the function at the starting point and calculate its norm
        iflag = 1;
      fcn(m, n, x, f, ref iflag);

      nfev = 1;
      if (iflag < 0) goto L300;

      fnorm = enorm(m, f);

      // initialize levenberg-marquardt parameter and iteration counter
      par = 0.0;
      iter = 1;

      // beginning of the outer loop.

      L30:

        // calculate the Jacobian matrix.
        iflag = 2;
      fdjac2(fcn, m, n, x, f, fjac, ldfjac, ref iflag,
        epsfcn, wa4);

      nfev += n;
      if (iflag < 0) goto L300;

      // if requested, call fcn to enable printing of iterates
      if (nprint <= 0) goto L40;

      iflag = 0;
      if ((iter - 1) % nprint == 0) 
        fcn(m, n, x, f, ref iflag);
    
      if (iflag < 0) goto L300;
    
      L40:

        // compute the qr factorization of the Jacobian.
        qrfac(m, n, fjac, ldfjac, true, ipvt, n, wa1,
          wa2, wa3);

      // on the first iteration and if mode is 1, scale according 
      // to the norms of the columns of the initial Jacobian

      if (iter != 1) goto L80;
      if (mode == 2) goto L60;

      for (j = 0; j < n; ++j) 
      { // LELLID!!
        diag[j] = wa2[j];
        if (wa2[j] == 0.0) diag[j] = 1.0;
      }

      L60:

        // on the first iteration, calculate the norm of the scaled x
        // and initialize the step bound delta

        for (j = 0; j < n; ++j) // LELLID
          wa3[j] = diag[j] * x[j];
    
      xnorm = enorm(n, wa3);
      delta = factor * xnorm;
      if (delta == 0.0) delta = factor;

      L80:

        // form (q transpose)*f and store the first n components in qtf
        for (i = 0; i < m; ++i) wa4[i] = f[i]; // LELLID!!
   
      for (j = 0; j < n; ++j) 
      { // LELLID!!
        if (fjac[j + j * fjac_dim1] != 0.0) 
        {
          sum = 0.0;
          for (i = j; i < m; ++i) // LELLID!! 
            sum += fjac[i + j * fjac_dim1] * wa4[i];
          temp = -sum / fjac[j + j * fjac_dim1];
          for (i = j; i < m; ++i) // LELLID!! 
            wa4[i] += fjac[i + j * fjac_dim1] * temp;
        }
        fjac[j + j * fjac_dim1] = wa1[j];
        qtf[j] = wa4[j];
      }

      // compute the norm of the scaled gradient
      gnorm = 0.0;
      if (fnorm != 0.0)
        for (j = 0; j < n; ++j) 
        { // LELLID!!
          l = ipvt[j];
          if (wa2[l] != 0.0) 
          { 
            sum = 0.0;
            for (i = 0; i <= j; ++i) // LELLID!!
              sum += fjac[i + j * fjac_dim1] * (qtf[i] / fnorm);
            gnorm =  Math.Max( gnorm, Math.Abs(sum/wa2[l]) );
          }
        }
    
      // test for convergence of the gradient norm
      if (gnorm <= gtol) info = 4;

      if (info != 0) goto L300;

      // rescale if necessary
      if (mode != 2)
        for (j = 0; j < n; ++j) // LELLID!!
          diag[j] = Math.Max( diag[j], wa2[j] );
    
      // beginning of the inner loop
    
      L200:

        // determine the levenberg-marquardt parameter
        lmpar(n, fjac, ldfjac, ipvt, diag, qtf, delta,
          ref par, wa1, wa2, wa3, wa4);
    
      // store the direction p and x + p. calculate the norm of p

      for (j = 0; j < n; ++j) 
      { // LELLID!!
        wa1[j] = -wa1[j];
        wa2[j] = x[j] + wa1[j];
        wa3[j] = diag[j] * wa1[j];
      }
      pnorm = enorm(n, wa3);

      // on the first iteration, adjust the initial step bound
      if (iter == 1) delta = Math.Min(delta, pnorm);

      // evaluate the function at x + p and calculate its norm

      iflag = 1;
      fcn(m, n, wa2, wa4, ref iflag);
      ++nfev;
      if (iflag < 0) goto L300;
      fnorm1 = enorm(m, wa4);

      // compute the scaled actual reduction

      actred = -1.0;
      if (p1 * fnorm1 < fnorm) 
        actred = 1.0 - sqr(fnorm1 / fnorm);

      // compute the scaled predicted reduction and
      // the scaled directional derivative

      for (j = 0; j < n; ++j) 
      { // Lellid!!
        wa3[j] = 0.0;
        l = ipvt[j];
        temp = wa1[l];
        for (i = 0; i <= j; ++i) 
        { // LELLID!!
          wa3[i] += fjac[i + j * fjac_dim1] * temp;
        }
      }

      temp1 = enorm(n, wa3) / fnorm;
      temp2 = Math.Sqrt(par) * pnorm / fnorm;
      prered = sqr(temp1) + sqr(temp2) / p5;
      dirder = -(sqr(temp1) + sqr(temp2));

      // compute the ratio of the actual to the predicted reduction
      ratio = 0.0;
      if (prered != 0.0) ratio = actred / prered;

      // update the step bound
      if (ratio > p25) goto L240;

      if (actred >= 0.0) temp = p5;
      if (actred < 0.0)   temp = p5 * dirder / (dirder + p5 * actred);
      if (p1 * fnorm1 >= fnorm || temp < p1) temp = p1;

      delta = temp * Math.Min(delta, pnorm / p1);
      par /= temp;
      goto L260;

      L240:
        if (par != 0.0 && ratio < p75) goto L260;
      delta = pnorm / p5;
      par = p5 * par;

      L260:

        // test for successful iteration
        if (ratio < p0001) goto L290;

      // successful iteration. update x, f, and their norms
      for (j = 0; j < n; ++j) 
      { // LELLID
        x[j] = wa2[j];
        wa2[j] = diag[j] * x[j];
      }

      for (i = 0; i < m; ++i) 
      { // LELLID!!
        f[i] = wa4[i];
      }
      xnorm = enorm(n, wa2);
      fnorm = fnorm1;
      ++iter;

      L290:

        // tests for convergence
        if (Math.Abs(actred) <= ftol && prered <= ftol 
          && p5 * ratio <= 1.0) info = 1;
   
      if (delta <= xtol * xnorm) info = 2;
   
      if (Math.Abs(actred) <= ftol && prered <= ftol 
        && p5 * ratio <= 1.0 && info == 2) info = 3;
   
      if (info != 0) goto L300;
   
      // tests for termination and stringent tolerances

      if (nfev >= maxfev) info = 5;
   
      if (Math.Abs(actred) <= DBL_EPSILON && prered <= DBL_EPSILON 
        && p5 * ratio <= 1.0) info = 6;
   
      if (delta <= DBL_EPSILON * xnorm) info = 7;
   
      if (gnorm <= DBL_EPSILON) info = 8;
   
      if (info != 0) goto L300;
   
      // end of the inner loop. repeat if iteration unsuccessful
      if (ratio < p0001) goto L200;

      // end of the outer loop
      goto L30;
    
      L300:

        // termination, either normal or user imposed
        if (iflag < 0) info = iflag;
      iflag = 0;
      if (nprint > 0) fcn(m, n, x, f, ref iflag);
    }


    public static void lmpar (int n, double[] r, int ldr, int[] ipvt, double[] diag, double[] qtb,
      double delta, ref double par, double[] x, double[] sdiag,
      double[] wa1, double[] wa2)
      //
      //     given an m by n matrix a, an n by n nonsingular diagonal 
      //     matrix d, an m-vector b, and a positive number delta, 
      //     the problem is to determine a value for the parameter 
      //     par such that if x solves the system 
      //
      //           a*x = b ,     sqrt(par)*d*x = 0 , 
      //
      //     in the least squares sense, and dxnorm is the euclidean 
      //     norm of d*x, then either par is zero and 
      //
      //           (dxnorm-delta) .le. 0.1*delta , 
      //
      //     or par is positive and 
      //
      //           abs(dxnorm-delta) .le. 0.1*delta . 
      //
      //     this subroutine completes the solution of the problem 
      //     if it is provided with the necessary information from the 
      //     qr factorization, with column pivoting, of a. that is, if 
      //     a*p = q*r, where p is a permutation matrix, q has orthogonal 
      //     columns, and r is an upper triangular matrix with diagonal 
      //     elements of nonincreasing magnitude, then lmpar expects 
      //     the full upper triangle of r, the permutation matrix p, 
      //     and the first n components of (q transpose)*b. on output 
      //     lmpar also provides an upper triangular matrix s such that 
      //
      //            t   t                   t 
      //           p *(a *a + par*d*d)*p = s *s . 
      //
      //     s is employed within lmpar and may be of separate interest. 
      //
      //     only a few iterations are generally needed for convergence 
      //     of the algorithm. if, however, the limit of 10 iterations 
      //     is reached, then the output par will contain the best 
      //     value obtained so far. 
      //
      //     the subroutine statement is 
      //
      //       subroutine lmpar(n,r,ldr,ipvt,diag,qtb,delta,par,x,sdiag, 
      //                        wa1,wa2) 
      //
      //     where 
      //
      //       n is a positive integer input variable set to the order of r. 
      //
      //       r is an n by n array. on input the full upper triangle 
      //         must contain the full upper triangle of the matrix r. 
      //         on output the full upper triangle is unaltered, and the 
      //         strict lower triangle contains the strict upper triangle 
      //         (transposed) of the upper triangular matrix s. 
      //
      //       ldr is a positive integer input variable not less than n 
      //         which specifies the leading dimension of the array r. 
      //
      //       ipvt is an integer input array of length n which defines the 
      //         permutation matrix p such that a*p = q*r. column j of p 
      //         is column ipvt(j) of the identity matrix. 
      //
      //       diag is an input array of length n which must contain the 
      //         diagonal elements of the matrix d. 
      //
      //       qtb is an input array of length n which must contain the first 
      //
      //         n elements of the vector (q transpose)*b. 
      //
      //       delta is a positive input variable which specifies an upper 
      //         bound on the euclidean norm of d*x. 
      //
      //       par is a nonnegative variable. on input par contains an 
      //         initial estimate of the levenberg-marquardt parameter. 
      //         on output par contains the final estimate. 
      //
      //       x is an output array of length n which contains the least 
      //         squares solution of the system a*x = b, sqrt(par)*d*x = 0, 
      //         for the output par. 
      //
      //       sdiag is an output array of length n which contains the 
      //         diagonal elements of the upper triangular matrix s. 
      //
      //       wa1 and wa2 are work arrays of length n. 
      //
    {
      const double p1 = 0.1;
      const double p001 = 0.001;

      int r_dim1 ;
      double parc, parl;
      int iter;
      double temp, paru;
      int i, j, k, l;
      int nsing;
      double gnorm, fp;
      double dxnorm;
      int jm1, jp1;
      double sum;

      // Parameter adjustments
    
      r_dim1 = ldr;

      // compute and store in x the gauss-newton direction. if the
      // Jacobian is rank-deficient, obtain a least squares solution

      nsing = n;
      for (j = 0; j < n; ++j) 
      { // LELLID!!
        wa1[j] = qtb[j];
        if (r[j + j * r_dim1] == 0.0 && nsing == n) nsing = j;
        if (nsing < n) wa1[j] = 0.0;
      }

      if (nsing >= 1) 
        for (k = 0; k < nsing; ++k) 
        { // LELLID!!
          j = nsing - 1 - k ; // LELLID!!
          wa1[j] /= r[j + j * r_dim1];
          temp = wa1[j];
          jm1 = j - 1;
          if (jm1 >= 0) // LELLID!!
            for (i = 0; i <= jm1; ++i) // LELLID!!
              wa1[i] -= r[i + j * r_dim1] * temp;
        }
    
      for (j = 0; j < n; ++j) 
      { // LELLID!!
        l = ipvt[j];
        x[l] = wa1[j];
      }

      // initialize the iteration counter
      // evaluate the function at the origin, and test
      // for acceptance of the gauss-newton direction

      iter = 0;
      for (j = 0; j < n; ++j) // LELLID!! 
        wa2[j] = diag[j] * x[j];
    
      dxnorm = enorm(n, wa2);
      fp = dxnorm - delta;
      if (fp <= p1 * delta) goto L220;

      // if the Jacobian is not rank deficient, the newton
      // step provides a lower bound, parl, for the zero of
      // the function. otherwise set this bound to zero

      parl = 0.0;
      if (nsing < n) goto L120; 

      for (j = 0; j < n; ++j) 
      { // LELLID!!
        l = ipvt[j];
        wa1[j] = diag[l] * (wa2[l] / dxnorm);
      }

      for (j = 0; j < n; ++j) 
      { // LELLID!!
        sum = 0.0;
        jm1 = j - 1;
        if (jm1 >= 0) // LELLID!!
          for (i = 0; i <= jm1; ++i) // LELLID!!
            sum += r[i + j * r_dim1] * wa1[i];
        wa1[j] = (wa1[j] - sum) / r[j + j * r_dim1];
      }
      temp = enorm(n, wa1);
      parl = fp / delta / temp / temp;
    
      L120:
    
        // calculate an upper bound, paru, for the zero of the function

        for (j = 0; j < n; ++j) 
        { // LELLID!!
          sum = 0.0;
          for (i = 0; i <= j; ++i) // LELLID!!
            sum += r[i + j * r_dim1] * qtb[i];
          l = ipvt[j];
          wa1[j] = sum / diag[l];
        }
      gnorm = enorm(n, wa1);
      paru = gnorm / delta;
      if (paru == 0.0) paru = double.Epsilon / Math.Min(delta, p1);
    
      // if the input par lies outside of the interval (parl,paru),
      // set par to the closer endpoint

      par = Math.Max(par, parl);
      par = Math.Min(par, paru);
      if (par == 0.0) par = gnorm / dxnorm;

      // beginning of an iteration

      L150:

        ++iter;

      // evaluate the function at the current value of par
      if (par == 0.0) par = Math.Max( double.Epsilon, p001 * paru );

      temp = Math.Sqrt(par);
      for (j = 0; j < n; ++j) // LELLID!! 
        wa1[j] = temp * diag[j];

      qrsolve(n, r, ldr, ipvt, wa1, qtb, x, sdiag ,wa2);

      for (j = 0; j < n; ++j) // LELLID!!
        wa2[j] = diag[j] * x[j];
  
      dxnorm = enorm(n, wa2);
      temp = fp;
      fp = dxnorm - delta;

      // if the function is small enough, accept the current value
      // of par. also test for the exceptional cases where parl
      // is zero or the number of iterations has reached 10

      if (Math.Abs(fp) <= p1 * delta || parl == 0.0 && fp <= temp 
        && temp < 0.0 || iter == 10) goto L220;

      // compute the newton correction.

      for (j = 0; j < n; ++j) 
      { // LELLID!!
        l = ipvt[j];
        wa1[j] = diag[l] * (wa2[l] / dxnorm);
      }

      for (j = 0; j < n; ++j) 
      { // LELLID!!
        wa1[j] /= sdiag[j];
        temp = wa1[j];
        jp1 = j + 1;
        if (n > jp1)
          for (i = jp1; i < n; ++i) // LELLID!! original: for (i = jp1; i <= n; ++i) 
            wa1[i] -= r[i + j * r_dim1] * temp;
      }
    
      temp = enorm(n, wa1);
      parc = fp / delta / temp / temp;

      // depending on the sign of the function, update parl or paru
      if (fp > 0.0) parl = Math.Max(parl, par);
      if (fp < 0.0) paru = Math.Min(paru, par);

      // compute an improved estimate for par
      par = Math.Max( parl, par + parc );

      // end of an iteration
      goto L150;

      L220:
    
        // termination
        if (iter == 0) par = 0.0;
    } 


    //static void qrslov (int n, double *r, int ldr, int *ipvt, 
    //        double *diag, double *qtb, double *x, double *sdiag, double *wa)
    public static void qrsolve (int n, double[] r, int ldr, int[] ipvt, 
      double[] diag, double[] qtb, double[] x, double[] sdiag, double[] wa)
      //
      //     given an m by n matrix a, an n by n diagonal matrix d, 
      //     and an m-vector b, the problem is to determine an x which 
      //     solves the system 
      //
      //           a*x = b ,     d*x = 0 , 
      //
      //     in the least squares sense. 
      //
      //     this subroutine completes the solution of the problem 
      //     if it is provided with the necessary information from the 
      //     qr factorization, with column pivoting, of a. that is, if 
      //     a*p = q*r, where p is a permutation matrix, q has orthogonal 
      //     columns, and r is an upper triangular matrix with diagonal 
      //     elements of nonincreasing magnitude, then qrsolv expects 
      //     the full upper triangle of r, the permutation matrix p, 
      //     and the first n components of (q transpose)*b. the system 
      //     a*x = b, d*x = 0, is then equivalent to 
      //
      //                  t       t 
      //           r*z = q *b ,  p *d*p*z = 0 , 
      //
      //     where x = p*z. if this system does not have full rank, 
      //     then a least squares solution is obtained. on output qrsolv 
      //     also provides an upper triangular matrix s such that 
      //
      //            t   t               t 
      //           p *(a *a + d*d)*p = s *s . 
      //
      //     s is computed within qrsolv and may be of separate interest. 
      //
      //     the subroutine statement is 
      //
      //       subroutine qrsolv(n,r,ldr,ipvt,diag,qtb,x,sdiag,wa) 
      //
      //     where 
      //
      //       n is a positive integer input variable set to the order of r. 
      //
      //       r is an n by n array. on input the full upper triangle 
      //         must contain the full upper triangle of the matrix r. 
      //         on output the full upper triangle is unaltered, and the 
      //         strict lower triangle contains the strict upper triangle 
      //         (transposed) of the upper triangular matrix s. 
      //
      //       ldr is a positive integer input variable not less than n 
      //         which specifies the leading dimension of the array r. 
      //
      //       ipvt is an integer input array of length n which defines the 
      //         permutation matrix p such that a*p = q*r. column j of p 
      //         is column ipvt(j) of the identity matrix. 
      //
      //       diag is an input array of length n which must contain the 
      //         diagonal elements of the matrix d. 
      //
      //       qtb is an input array of length n which must contain the first 
      //
      //         n elements of the vector (q transpose)*b. 
      //
      //       x is an output array of length n which contains the least 
      //         squares solution of the system a*x = b, d*x = 0. 
      //
      //       sdiag is an output array of length n which contains the 
      //         diagonal elements of the upper triangular matrix s. 
      //
      //       wa is a work array of length n. 
      //
    {
      const double p5 = 0.5;
      const double p25 = 0.25;

      int r_dim1;
      double temp;
      int i, j, k, l;
      double cotan;
      int nsing;
      double qtbpj;
      int jp1, kp1;
      double tan_, cos_, sin_, sum;

      // Parameter adjustments
      r_dim1 = ldr;

      // copy r and (q transpose)*b to preserve input and initialize s
      // in particular, save the diagonal elements of r in x

      for (j = 0; j < n; ++j) 
      { // LELLID!! 
        for (i = j; i < n; ++i) // LELLID!!
          r[i + j * r_dim1] = r[j + i * r_dim1];  
        x[j] = r[j + j * r_dim1];
        wa[j] = qtb[j];
      }

      // eliminate the diagonal matrix d using a givens rotation

      for (j = 0; j < n; ++j) 
      { // LELLID!!

        // prepare the row of d to be eliminated, locating the
        // diagonal element using p from the qr factorization

        l = ipvt[j];
        if (diag[l] == 0.0) goto L90;

        for (k = j; k < n; ++k) sdiag[k] = 0.0; // LELLID!!
        sdiag[j] = diag[l];

        // the transformations to eliminate the row of d
        // modify only a single element of (q transpose)*b
        // beyond the first n, which is initially zero

        qtbpj = 0.0;
        for (k = j; k < n; ++k) 
        { // LELLID!!

          // determine a givens rotation which eliminates the
          // appropriate element in the current row of d

          if (sdiag[k] == 0.0) goto L70;
          if (Math.Abs(r[k + k * r_dim1]) >= Math.Abs(sdiag[k])) goto L40;
          cotan = r[k + k * r_dim1] / sdiag[k];
          sin_ = p5 / Math.Sqrt(p25 + p25 * sqr(cotan));
          cos_ = sin_ * cotan;
          goto L50;
      
        L40:

          tan_ = sdiag[k] / r[k + k * r_dim1];
          cos_ = p5 / Math.Sqrt(p25 + p25 * sqr(tan_) );
          sin_ = cos_ * tan_;

        L50:

          // compute the modified diagonal element of r and
          // the modified element of ((q transpose)*b,0)

          r[k + k * r_dim1] = cos_ * r[k + k * r_dim1] + sin_ * sdiag[k];
          temp = cos_ * wa[k] + sin_ * qtbpj;
          qtbpj = -sin_ * wa[k] + cos_ * qtbpj;
          wa[k] = temp;

          // accumulate the tranformation in the row of s

          kp1 = k + 1;
          // if (n < (kp1+1)) goto L70; LELLID!!
          for (i = kp1; i < n; ++i) 
          { // LELLID!! original: for (i = kp1; i <= n; ++i) {
            temp = cos_ * r[i + k * r_dim1] + sin_ * sdiag[i];
            sdiag[i] = -sin_ * r[i + k * r_dim1] + cos_ * sdiag[i];
            r[i + k * r_dim1] = temp;
          }
        L70: ; 
        }

      L90:

        // store the diagonal element of s and restore
        // the corresponding diagonal element of r

        sdiag[j] = r[j + j * r_dim1];
        r[j + j * r_dim1] = x[j];
      }

      // solve the triangular system for z. if the system is
      // singular, then obtain a least squares solution

      nsing = n; 
      for (j = 0; j < n; ++j) 
      {
        if (sdiag[j] == 0.0 && nsing == n) nsing = j; // LELLID!! (statt nsing = j-1;)
        if (nsing < n) wa[j] = 0.0;
      }

      if (nsing >= 0) 
        for (k = 0; k < nsing; ++k) 
        { // LELLID!!
          j = nsing -1 - k;
          sum = 0.0;
          jp1 = j + 1;
          if (nsing > jp1) 
            for (i = jp1; i < nsing; ++i) // LELLID!!
              sum += r[i + j * r_dim1] * wa[i];
          wa[j] = (wa[j] - sum) / sdiag[j];
        }
    
      // permute the components of z back to components of x
      for (j = 0; j < n; ++j) 
      { // LELLID!!
        l = ipvt[j];
        x[l] = wa[j];
      }
    } 



    static void qrfac (int m, int n, double[] a, int lda, bool pivot, int[] ipvt,
      int lipvt, double[] rdiag, double[] acnorm, double[] wa)
      //
      //     this subroutine uses householder transformations with column 
      //     pivoting (optional) to compute a qr factorization of the 
      //     m by n matrix a. that is, qrfac determines an orthogonal 
      //     matrix q, a permutation matrix p, and an upper trapezoidal 
      //     matrix r with diagonal elements of nonincreasing magnitude, 
      //     such that a*p = q*r. the householder transformation for 
      //     column k, k = 1,2,...,min(m,n), is of the form 
      //
      //                           t 
      //           i - (1/u(k))*u*u 
      //
      //     where u has zeros in the first k-1 positions. the form of 
      //     this transformation and the method of pivoting first 
      //     appeared in the corresponding linpack subroutine. 
      //
      //     the subroutine statement is 
      //
      //       subroutine qrfac(m,n,a,lda,pivot,ipvt,lipvt,rdiag,acnorm,wa) 
      //
      //     where 
      //
      //       m is a positive integer input variable set to the number 
      //         of rows of a. 
      //
      //       n is a positive integer input variable set to the number 
      //         of columns of a. 
      //
      //       a is an m by n array. on input a contains the matrix for 
      //         which the qr factorization is to be computed. on output 
      //         the strict upper trapezoidal part of a contains the strict 
      //         upper trapezoidal part of r, and the lower trapezoidal 
      //         part of a contains a factored form of q (the non-trivial 
      //         elements of the u vectors described above). 
      //
      //       lda is a positive integer input variable not less than m 
      //         which specifies the leading dimension of the array a. 
      //
      //       pivot is a logical input variable. if pivot is set true, 
      //         then column pivoting is enforced. if pivot is set false, 
      //         then no column pivoting is done. 
      //
      //       ipvt is an integer output array of length lipvt. ipvt 
      //         defines the permutation matrix p such that a*p = q*r. 
      //         column j of p is column ipvt(j) of the identity matrix. 
      //         if pivot is false, ipvt is not referenced. 
      //
      //       lipvt is a positive integer input variable. if pivot is false, 
      //
      //         then lipvt may be as small as 1. if pivot is true, then 
      //         lipvt must be at least n. 
      //
      //       rdiag is an output array of length n which contains the 
      //         diagonal elements of r. 
      //
      //       acnorm is an output array of length n which contains the 
      //         norms of the corresponding columns of the input matrix a. 
      //         if this information is not needed, then acnorm can coincide 
      //         with rdiag. 
      //
      //       wa is a work array of length n. if pivot is false, then wa 
      //         can coincide with rdiag. 
      //
    {
      int a_dim1 ;
      int kmax;
      double temp;
      int i, j, k, minmn;
      double ajnorm;
      int jp1;
      double sum;

      // Parameter adjustments
      //--wa;
      //--acnorm;
      //--rdiag;
      //--ipvt;
      a_dim1 = lda;
      //a_offset = a_dim1 + 1;
      //a -= a_offset;

      // compute the initial column norms and initialize several arrays
      for (j = 0; j < n; ++j) 
      { // LELLID!!
        acnorm[j] = enorm(m, a, j * a_dim1); // LELLID!!
        rdiag[j] = acnorm[j];
        wa[j] = rdiag[j];
        if (pivot) 
        {
          ipvt[j] = j;
        }
      }

      // reduce a to r with householder transformations
      minmn = Math.Min(m,n);
      for (j = 0; j < minmn; ++j) 
      { // LELLID!!
        if ( ! pivot ) goto L40;

        // bring the column of largest norm into the pivot position
        kmax = j;
        for (k = j; k < n; ++k) // LELLID!!
          if (rdiag[k] > rdiag[kmax]) kmax = k;
       
        if (kmax == j) goto L40;
        for (i = 0; i < m; ++i) 
        { // LELLID!!
          temp = a[i + j * a_dim1];
          a[i + j * a_dim1] = a[i + kmax * a_dim1];
          a[i + kmax * a_dim1] = temp;
        }
        rdiag[kmax] = rdiag[j];
        wa[kmax] = wa[j];
        k = ipvt[j];
        ipvt[j] = ipvt[kmax];
        ipvt[kmax] = k;

      L40:

        // compute the householder transformation to reduce the
        // j-th column of a to a multiple of the j-th unit vector

        // LELLID: ajnorm = enorm( m - j + 1, a, j + j * a_dim1);
        ajnorm = enorm( m - j , a, j + j * a_dim1);
        if (ajnorm == 0.0) goto L100;
        if (a[j + j * a_dim1] < 0.0) ajnorm = -ajnorm;
        for (i = j; i < m; ++i) // LELLID!! 
          a[i + j * a_dim1] /= ajnorm;
        a[j + j * a_dim1] += 1.0;

        // apply the transformation to the remaining columns
        // and update the norms

        jp1 = j + 1;
        if (n <= jp1) goto L100; // LELLID!!

        for (k = jp1; k < n; ++k) 
        { // LELLID

          sum = 0.0;
          for (i = j; i < m; ++i) // LELLID!!
            sum += a[i + j * a_dim1] * a[i + k * a_dim1];
      
          temp = sum / a[j + j * a_dim1];
          for (i = j; i < m; ++i) // LELLID!! 
            a[i + k * a_dim1] -= temp * a[i + j * a_dim1];
      
          if ( ! pivot || rdiag[k] == 0.0 ) goto L80;
          temp = a[j + k * a_dim1] / rdiag[k];

          rdiag[k] *= Math.Sqrt((Math.Max( 0.0, 1.0 - sqr(temp) )));
          if (0.05 * sqr(rdiag[k] / wa[k]) > DBL_EPSILON) goto L80;
          rdiag[k] = enorm(m-j-1, a, j + k * a_dim1); // VALID??? original: rdiag[k] = enorm(m-j, a, jp1 + k * a_dim1);

          wa[k] = rdiag[k];
        L80: ;
        }
      L100:
        rdiag[j] = -ajnorm;
      }
    }


    /// <summary>
    /// given an n-vector x, this function calculates the 
    /// euclidean norm of x. 
    /// </summary>
    /// <param name="n">n is a positive integer input variable.</param>
    /// <param name="x">x is an input array of length n. </param>
    /// <returns>Euclidean norm of x.</returns>
    /// <remarks>
    /// the euclidean norm is computed by accumulating the sum of 
    /// squares in three different sums. the sums of squares for the 
    /// small and large components are scaled so that no overflows 
    /// occur. non-destructive underflows are permitted. underflows 
    /// and overflows do not occur in the computation of the unscaled 
    /// sum of squares for the intermediate components. 
    /// the definitions of small, intermediate and large components 
    /// depend on two constants, rdwarf and rgiant. the main 
    /// restrictions on these constants are that rdwarf**2 not 
    /// underflow and rgiant**2 not overflow. the constants 
    /// given here are suitable for every known computer. 
    /// <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para> 
    /// </remarks>
    public static double enorm (int n, double[] x)
      //
  
     
    {
      const double rdwarf = 3.834e-20;
      const double rgiant = 1.304e19;
      double ret_val=0.0, xabs, x1max, x3max, s1, s2, s3, agiant, floatn;
      int i;

      // Parameter adjustments
      // --x; LELLID!!

      s1 = s2 = s3 = x1max = x3max = 0.0;
      floatn = (double) n;
      agiant = rgiant / floatn;

      for (i = 0; i < n; i++) 
      { // LELLID!!

        xabs = Math.Abs(x[i]);
        if (xabs > rdwarf && xabs < agiant) goto L70;
        if (xabs <= rdwarf) goto L30;

        //sum for large components
        if (xabs <= x1max)  goto L10;
        s1 = 1.0 + s1 * sqr(x1max / xabs);
        x1max = xabs;
        goto L80;
  
      L10:
        s1 += sqr(xabs / x1max);
        goto L80;

      L30:
        // sum for small components
        if (xabs <= x3max) goto L40;
        s3 = 1.0 + s3 * sqr(x3max / xabs);
        x3max = xabs;
        goto L80;

      L40:
        if (xabs != 0.0)  s3 += sqr(xabs / x3max);
        goto L80;

      L70:  
        // sum for intermediate components
        s2 += sqr(xabs);

      L80: ;
      }
    
      // calculation of norm
      if (s1 == 0.0) goto L100;
      ret_val = x1max * Math.Sqrt(s1 + s2 / x1max / x1max);
      goto L130;

      L100:
        if (s2 == 0.0) goto L110;
      if (s2 >= x3max) 
        ret_val = Math.Sqrt(s2 * (1.0 + x3max / s2 * (x3max * s3)));    
      if (s2 < x3max) 
        ret_val = Math.Sqrt(x3max * (s2 / x3max + x3max * s3));
      goto L130;

      L110:
        ret_val = x3max * Math.Sqrt(s3);

      L130:
        return ret_val;
    }


    /// <summary>Given an n-vector x, this function calculates the
    /// euclidean norm of x. 
    /// </summary>
    /// <param name="n">A positive integer input variable of the number of elements to process.</param>
    /// <param name="x">An input array of length n. </param>
    /// <param name="startindex">The index of the first element in x to process.</param>
    /// <returns>The euclidian norm of the vector of length n, i.e. the square root of the sum of squares of the elements.</returns>
    /// <remarks>
    ///     the euclidean norm is computed by accumulating the sum of 
    ///     squares in three different sums. the sums of squares for the 
    ///     small and large components are scaled so that no overflows 
    ///     occur. non-destructive underflows are permitted. underflows 
    ///     and overflows do not occur in the computation of the unscaled 
    ///     sum of squares for the intermediate components. 
    ///     the definitions of small, intermediate and large components 
    ///     depend on two constants, rdwarf and rgiant. the main 
    ///     restrictions on these constants are that rdwarf**2 not 
    ///     underflow and rgiant**2 not overflow. the constants 
    ///     given here are suitable for every known computer. 
    ///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
    ///      
    /// </remarks>
    public static double enorm (int n, double[] x, int startindex)
    {
      const double rdwarf = 3.834e-20;
      const double rgiant = 1.304e19;
      double ret_val=0.0, xabs, x1max, x3max, s1, s2, s3, agiant, floatn;
      int i;

      // Parameter adjustments
      // --x; LELLID!!

      s1 = s2 = s3 = x1max = x3max = 0.0;
      floatn = (double) n;
      agiant = rgiant / floatn;

      for (i = 0; i < n; i++) 
      { // LELLID!!

        xabs = Math.Abs(x[i+startindex]);
        if (xabs > rdwarf && xabs < agiant) goto L70;
        if (xabs <= rdwarf) goto L30;

        //sum for large components
        if (xabs <= x1max)  goto L10;
        s1 = 1.0 + s1 * sqr(x1max / xabs);
        x1max = xabs;
        goto L80;
  
      L10:
        s1 += sqr(xabs / x1max);
        goto L80;

      L30:
        // sum for small components
        if (xabs <= x3max) goto L40;
        s3 = 1.0 + s3 * sqr(x3max / xabs);
        x3max = xabs;
        goto L80;

      L40:
        if (xabs != 0.0)  s3 += sqr(xabs / x3max);
        goto L80;

      L70:  
        // sum for intermediate components
        s2 += sqr(xabs);

      L80: ;
      }
    
      // calculation of norm
      if (s1 == 0.0) goto L100;
      ret_val = x1max * Math.Sqrt(s1 + s2 / x1max / x1max);
      goto L130;

      L100:
        if (s2 == 0.0) goto L110;
      if (s2 >= x3max) 
        ret_val = Math.Sqrt(s2 * (1.0 + x3max / s2 * (x3max * s3)));    
      if (s2 < x3max) 
        ret_val = Math.Sqrt(x3max * (s2 / x3max + x3max * s3));
      goto L130;

      L110:
        ret_val = x3max * Math.Sqrt(s3);

      L130:
        return ret_val;
    }



    /// <summary>
    /// This subroutine computes a forward-difference approximation 
    /// to the m by n Jacobian matrix associated with a specified 
    /// problem of m functions in n variables. 
    /// </summary>
    /// <param name="fcn">User-supplied subroutine which  calculates the functions</param>
    /// <param name="m">m is a positive integer input variable set to the number of functions.</param>
    /// <param name="n">n is a positive integer input variable set to the number of variables. n must not exceed m.</param>
    /// <param name="x">x is an input array of length n containing the parameters.</param>
    /// <param name="fvec">fvec is an input array of length m which must contain the functions evaluated at x. </param>
    /// <param name="fjac">fjac is an output m by n array which contains the approximation to the Jacobian matrix evaluated at x.</param>
    /// <param name="ldfjac">ldfjac is a positive integer input variable not less than m which specifies the leading dimension of the array fjac. </param>
    /// <param name="iflag">iflag is an integer variable which can be used to terminate the execution of fdjac2. see description of fcn.</param>
    /// <param name="epsfcn">
    /// epsfcn is an input variable used in determining a suitable 
    /// step length for the forward-difference approximation. this 
    /// approximation assumes that the relative errors in the 
    /// functions are of the order of epsfcn. if epsfcn is less 
    /// than the machine precision, it is assumed that the relative 
    /// errors in the functions are of the order of the machine 
    /// precision. 
    /// </param>
    /// <param name="wa">wa is a work array of length m.</param>
    public static void fdjac2(
      LMFunction fcn,
      int m, 
      int n, 
      double[] x, 
      double[] fvec, 
      double[] fjac, 
      int ldfjac,
      ref int iflag,
      double epsfcn,
      double[] wa)
     
    {
      int fjac_dim1;
      double temp, h;
      int i, j;
      double eps;

      // Parameter adjustments 
      // --wa; LELLID!!
      fjac_dim1 = ldfjac;
      // fjac_offset = fjac_dim1 + 1; // LELLID!!
      // fjac -= fjac_offset; // LELLID!!
      // --fvec; LELLID!!
      // --x; // LELLID!!

      eps = Math.Sqrt((Math.Max(epsfcn, DBL_EPSILON)));

      for (j = 0; j < n; ++j) 
      { // LELLID!!
        temp = x[j];
        h = eps * Math.Abs(temp);
        if (h == 0.0)  
          h = eps;
        x[j] = temp + h;
        fcn(m, n, x, wa, ref iflag);
        if (iflag < 0) 
          return;
        x[j] = temp;
        for (i = 0; i < m; ++i) // LELLID!!
          fjac[i + j * fjac_dim1] = (wa[i] - fvec[i]) / h;
      }
    }
 
#if false

    /// <summary>
    /// Compute the covariance matrix  cov = inv (J^T J) by QRP^T decomposition of J.
    /// </summary>
    /// <param name="?"></param>
    /// <returns></returns>
    /// <remarks>The source code of this function is originated from the
    /// GNU scientific library V1.6. multifit/covar.c Author Brian Gough</remarks>
    int gsl_multifit_covar (IROMatrix J, double epsrel, IMatrix covar)
  {
    double tolr;

    size_t i, j, k;
    size_t kmax = 0;

    double[,] r;
    double[] tau;
    double[] norm;
    gsl_permutation * perm;

    int m = J.Columns, n = J.Rows;
  
    if (m < n) 
  {
    throw new ArgumentException("Jacobian be rectangular M x N with M >= N");
  }

  if (covar.Columns != covar.Rows || covar.Columns != n)
{
  throw new ArgumentException("Covariance matrix must be square and match second dimension of jacobian");
}

  r = new double[m, n];
  tau = new double[n];
  perm = gsl_permutation_alloc (n) ;
  norm = new double[n] ;
  
{
  int signum = 0;
  gsl_matrix_memcpy (r, J);
  gsl_linalg_QRPT_decomp (r, tau, perm, &signum, norm);
}
  
  
  /* Form the inverse of R in the full upper triangle of R */

  tolr = epsrel * fabs(gsl_matrix_get(r, 0, 0));

  for (k = 0 ; k < n ; k++)
{
  double rkk = gsl_matrix_get(r, k, k);

  if (fabs(rkk) <= tolr)
{
  break;
}

  gsl_matrix_set(r, k, k, 1.0/rkk);

  for (j = 0; j < k ; j++)
{
  double t = gsl_matrix_get(r, j, k) / rkk;
  gsl_matrix_set (r, j, k, 0.0);

  for (i = 0; i <= j; i++)
{
  double rik = gsl_matrix_get (r, i, k);
  double rij = gsl_matrix_get (r, i, j);
              
  gsl_matrix_set (r, i, k, rik - t * rij);
}
}
  kmax = k;
}

  /* Form the full upper triangle of the inverse of R^T R in the full
     upper triangle of R */

  for (k = 0; k <= kmax ; k++)
{
  for (j = 0; j < k; j++)
{
  double rjk = gsl_matrix_get (r, j, k);

  for (i = 0; i <= j ; i++)
{
  double rij = gsl_matrix_get (r, i, j);
  double rik = gsl_matrix_get (r, i, k);

  gsl_matrix_set (r, i, j, rij + rjk * rik);
}
}
      
{
  double t = gsl_matrix_get (r, k, k);

  for (i = 0; i <= k; i++)
{
  double rik = gsl_matrix_get (r, i, k);

  gsl_matrix_set (r, i, k, t * rik);
};
}
}

  /* Form the full lower triangle of the covariance matrix in the
     strict lower triangle of R and in w */

  for (j = 0 ; j < n ; j++)
{
  size_t pj = gsl_permutation_get (perm, j);
      
  for (i = 0; i <= j; i++)
{
  size_t pi = gsl_permutation_get (perm, i);

  double rij;

  if (j > kmax)
{
  gsl_matrix_set (r, i, j, 0.0);
  rij = 0.0 ;
}
  else 
{
  rij = gsl_matrix_get (r, i, j);
}

  if (pi > pj)
{
  gsl_matrix_set (r, pi, pj, rij); 
} 
  else if (pi < pj)
{
  gsl_matrix_set (r, pj, pi, rij);
}

}
      
{ 
  double rjj = gsl_matrix_get (r, j, j);
  gsl_matrix_set (covar, pj, pj, rjj);
}
}

     
  /* symmetrize the covariance matrix */

  for (j = 0 ; j < n ; j++)
{
  for (i = 0; i < j ; i++)
{
  double rji = gsl_matrix_get (r, j, i);

  gsl_matrix_set (covar, j, i, rji);
  gsl_matrix_set (covar, i, j, rji);
}
}

  gsl_matrix_free (r);
  gsl_permutation_free (perm);
  gsl_vector_free (tau);
  gsl_vector_free (norm);

  return GSL_SUCCESS;
} 

#endif


    /// <summary>
    /// This will compute the covariances at a given parameter set xvec.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="xvec"></param>
    /// <param name="covar"></param>
    public static int ComputeCovariances(LMFunction fcn, double[] x, int n, int m, double[]covar, out double sumchisq)
    {
      int  info=0;
      

      double[] f = new double[n];
      double[] fjac = new double[n*m];
      double [] jactjac = new double[m*m];

      fcn(n,m,x,f, ref info);
      sumchisq=0;
      for(int i=0;i<n;++i)
        sumchisq += f[i]*f[i];

      // calculate the Jacobian matrix.
      int iflag = 2;
      int ldfjac = n;
      double epsfcn=0;
      double[] wa4 = new double[n];
      fdjac2(fcn, n, m, x, f, fjac, ldfjac, ref iflag, epsfcn, wa4);

      // compute jacT*jac
      for(int i=0;i<m;++i)
      {
        for(int j=0;j<m;++j)
        {
          double sum = 0;
          for(int k=0;k<n;++k)
          {
            sum += fjac[k+n*i]*fjac[k+n*j];
          }
          jactjac[j+i*m] = sum;
        }
      }


      return LEVMAR_COVAR(jactjac, covar, sumchisq, m, n);

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


      IMatrix matresult = MatrixMath.PseudoInverse(MatrixMath.ToROMatrix(JtJ,m),out rnk);
      MatrixMath.Copy(matresult,MatrixMath.ToMatrix(C,m));

      fact=sumsq/(double)(n-rnk);
      for(i=0; i<m*m; ++i)
        C[i]*=fact;

      return rnk;
    }

   



  } // end class
} // end namespace
