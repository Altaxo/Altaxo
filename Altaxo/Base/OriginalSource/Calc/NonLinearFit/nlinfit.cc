/*-----------------------------------------------------------------------------*\
| LevenbergMarquardtFit()                                            nlinfit.cc |
|                                                                               |
| general multi-dimensional non-linear least-squares fit                        |
| using the Levenberg-Marquardt algorithm                                       |
|                                                                               |
| Last change: Jun 22, 2001							|
|                                                                               |
| Matpack Library Release 1.7.2                                                 |
| Copyright (C) 1991-2001 by Berndt M. Gammel. All rights reserved.             |
|                                                                               |
| Permission to  use, copy, and  distribute  Matpack  in  its entirety  and its |
| documentation  for non-commercial purpose and  without fee is hereby granted, |
| provided that this license information and copyright notice appear unmodified |
| in all copies.  This software is provided 'as is'  without express or implied |
| warranty.  In no event will the author be held liable for any damages arising |
| from the use of this software.						|
| Note that distributing Matpack 'bundled' in with any product is considered to |
| be a 'commercial purpose'.							|
| The software may be modified for your own purposes, but modified versions may |
| not be distributed without prior consent of the author.			|
|                                                                               |
| Read the  COPYRIGHT and  README files in this distribution about registration	|
| and installation of Matpack.							|
|                                                                               |
\*-----------------------------------------------------------------------------*/

#include "matpack.h"

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

typedef void (*LMFunction) (int,int,double*,double*,int&);

void LevenbergMarquardtFit (LMFunction fcn, Vector& x, Vector& f, 
			    double tol, int& info);

void LevenbergMarquardtFit (LMFunction fcn, Vector& x, Vector& f, 
			    double tol, int& info, int *iwa, double *wa, int lwa);

void LevenbergMarquardtFit (LMFunction fcn, Vector& x, Vector& f,
			    double ftol, double xtol, double gtol,
			    int maxfev, double epsfcn, double *diag, int mode, 
			    double factor, int nprint, int& info, int& nfev, double *fjac, 
			    int ldfjac, int *ipvt,
			    double *qtf, double *wa1, double *wa2, double *wa3, double *wa4);


// local function prototypes 

static void lmpar (int n, double *r, int ldr, int *ipvt, double *diag, double *qtb,
		   double delta, double& par, double *x, double *sdiag,
		   double *wa1, double *wa2);

static void qrslov (int n, double *r, int ldr, int *ipvt, 
		    double *diag, double *qtb, double *x, double *sdiag, double *wa);

static void  qrfac (int m, int n, double *a, int lda, int pivot, int *ipvt,
		    int lipvt, double *rdiag, double *acnorm, double *wa);

static void  fdjac2 (LMFunction fcn, int m, int n, double *x, double *fvec, double *fjac, 
		     int ldfjac, int& iflag, double epsfcn, double *wa);

static double enorm (int n, double *x);



void LevenbergMarquardtFit (LMFunction fcn, Vector& xvec, Vector& fvec, 
			    double tol, int& info)
//
// The purpose of LevenbergMarquardtFit is to minimize the sum of the 
// squares of m nonlinear functions in n variables by a modification of the 
// Levenberg-Marquardt algorithm. This is done by using the more 
// general least-squares solver below. The user must provide a 
// subroutine which calculates the functions. The Jacobian is 
// then calculated by a forward-difference approximation. 
//
// This is the most easy-to-use interface with the smallest number of
// arguments. If you need more control over the minimization process and
// auxilliary storage allocation you should use one of the interfaces 
// described below.
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
//       tol is a nonnegative input variable. Termination occurs 
//         when the algorithm estimates either that the relative 
//         error in the sum of squares is at most tol or that 
//         the relative error between x and the solution is at 
//         most tol. 
//
//       info is an integer output variable. If the user has 
//         terminated execution, info is set to the (negative) 
//         value of iflag. See description of fcn. Otherwise, 
//         info is set as follows: 
//
//         info = 0  improper input parameters. 
//
//         info = 1  algorithm estimates that the relative error 
//                   in the sum of squares is at most tol. 
//
//         info = 2  algorithm estimates that the relative error 
//                   between x and the solution is at most tol. 
//
//         info = 3  conditions for info = 1 and info = 2 both hold. 
//
//         info = 4  fvec is orthogonal to the columns of the 
//                   Jacobian to machine precision. 
//
//         info = 5  number of calls to fcn has reached or 
//                   exceeded 200*(n+1). 
//
//         info = 6  tol is too small. No further reduction in 
//                   the sum of squares is possible. 
//
//         info = 7  tol is too small. No further improvement in 
//                   the approximate solution x is possible. 
//
{
    double *wa;
    int m,n,lwa,*iwa;
    
    m = fvec.Hi()-fvec.Lo()+1;       // number of functions
    n = xvec.Hi()-xvec.Lo()+1;       // number of variables

    // allocate working arrays
    lwa = m*n+5*n+m;
    iwa = new int[n];
    wa  = new double[lwa];
    
    LevenbergMarquardtFit (fcn,xvec,fvec,tol,info,iwa,wa,lwa); 

    delete[] iwa;
    delete[] wa;
}


void LevenbergMarquardtFit (LMFunction fcn, Vector& xvec, Vector& fvec, 
			    double tol, int& info, int *iwa, double *wa, int lwa)
//
// The purpose of LevenbergMarquardtFit is to minimize the sum of the 
// squares of m nonlinear functions in n variables by a modification of the 
// Levenberg-Marquardt algorithm. This is done by using the more 
// general least-squares solver below. The user must provide a 
// subroutine which calculates the functions. The Jacobian is 
// then calculated by a forward-difference approximation. 
//
// Use this interface to the Levenberg-Marquardt algorithm if you want
// to control the allocation of intermediate storage yourself, but leave
// control to the minimization process to the machine. Use the more general
// described below to get full control.
//
//     Arguments:
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
//       tol is a nonnegative input variable. Termination occurs 
//         when the algorithm estimates either that the relative 
//         error in the sum of squares is at most tol or that 
//         the relative error between x and the solution is at 
//         most tol. 
//
//       info is an integer output variable. if the user has 
//         terminated execution, info is set to the (negative) 
//         value of iflag. See description of fcn. Otherwise, 
//         info is set as follows: 
//
//         info = 0  improper input parameters. 
//
//         info = 1  algorithm estimates that the relative error 
//                   in the sum of squares is at most tol. 
//
//         info = 2  algorithm estimates that the relative error 
//                   between x and the solution is at most tol. 
//
//         info = 3  conditions for info = 1 and info = 2 both hold. 
//
//         info = 4  fvec is orthogonal to the columns of the 
//                   Jacobian to machine precision. 
//
//         info = 5  number of calls to fcn has reached or 
//                   exceeded 200*(n+1). 
//
//         info = 6  tol is too small. no further reduction in 
//                   the sum of squares is possible. 
//
//         info = 7  tol is too small. no further improvement in 
//                   the approximate solution x is possible. 
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
    m = fvec.Hi()-fvec.Lo()+1;       // number of functions
    n = xvec.Hi()-xvec.Lo()+1;       // number of variables

    --wa;
    --iwa;

    info = 0;

    // check the input parameters for errors
    if (n <= 0 || m < n || tol < 0 || lwa < m * n + n * 5 + m) return;

    factor = 100;
    maxfev = (n + 1) * 200;
    ftol = tol;
    xtol = tol;
    gtol = 0;
    epsfcn = 0;
    mode = 1;
    nprint = 0;

    LevenbergMarquardtFit(fcn, xvec, fvec, ftol, xtol, gtol, maxfev, epsfcn,
			  &wa[1], mode, factor, nprint, info, nfev, &wa[m + n * 5 + 1], m,
			  &iwa[1], &wa[n + 1], &wa[(n << 1) + 1], &wa[n * 3 + 1], &wa[(n << 2) + 1],
			  &wa[n * 5 + 1]);
    
    if (info == 8) info = 4;
}	


void  LevenbergMarquardtFit (LMFunction fcn, Vector& xvec, Vector& fvec, 
			     double ftol, double xtol, double gtol,
			     int maxfev, double epsfcn, double *diag, int mode, 
			     double factor, int nprint, int& info, int& nfev, double *fjac, 
			     int ldfjac, int *ipvt,
			     double *qtf, double *wa1, double *wa2, double *wa3, double *wa4)
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
    static double p1 = 0.1;
    static double p5 = 0.5;
    static double p25 = 0.25;
    static double p75 = 0.75;
    static double p0001 = 1e-4;

    int fjac_dim1, fjac_offset;

    int iter;
    double temp=0.0, temp1, temp2;
    int i, j, l, iflag;
    double delta, ratio;
    double fnorm, gnorm;
    double pnorm, xnorm=0.0, fnorm1, actred, dirder, prered;
    double par, sum;

        
    // Parameter adjustments

    int m,n;
    m = fvec.Hi()-fvec.Lo()+1;  // number of functions
    n = xvec.Hi()-xvec.Lo()+1;  // number of variables

    double *x,*f;
    x = &xvec[ xvec.Lo()-1 ];   // 1-offset solution variables vector
    f = &fvec[ fvec.Lo()-1 ];   // 1-offset functions vector

    --wa4;
    --wa3;
    --wa2;
    --wa1;
    --qtf;
    --ipvt;
    fjac_dim1 = ldfjac;
    fjac_offset = fjac_dim1 + 1;
    fjac -= fjac_offset;
    --diag;

    info =  iflag =  nfev = 0;

    // check the input parameters for errors
    if (n <= 0 || m < n || ldfjac < m || ftol < 0.0 || xtol < 0.0 ||
	gtol < 0.0 || maxfev <= 0 || factor <= 0.0) 
      goto L300;
    
    if (mode != 2) goto L20;
    
    for (j = 1; j <= n; j++) 
      if (diag[j] <= 0.0) goto L300;
    
  L20:

    // evaluate the function at the starting point and calculate its norm
    iflag = 1;
    (*fcn) (m, n, &x[1], &f[1], iflag);

    nfev = 1;
    if (iflag < 0) goto L300;

    fnorm = enorm(m, &f[1]);

    // initialize levenberg-marquardt parameter and iteration counter
    par = 0.0;
    iter = 1;

    // beginning of the outer loop.

  L30:

    // calculate the Jacobian matrix.
    iflag = 2;
    fdjac2(fcn, m, n, &x[1], &f[1], &fjac[fjac_offset], ldfjac, iflag,
	    epsfcn, &wa4[1]);

    nfev += n;
    if (iflag < 0) goto L300;

    // if requested, call fcn to enable printing of iterates
    if (nprint <= 0) goto L40;

    iflag = 0;
    if ((iter - 1) % nprint == 0) 
      (*fcn) (m, n, &x[1], &f[1], iflag);
    
    if (iflag < 0) goto L300;
    
  L40:

    // compute the qr factorization of the Jacobian.
    qrfac(m, n, &fjac[fjac_offset], ldfjac, true, &ipvt[1], n, &wa1[1],
	  &wa2[1], &wa3[1]);

    // on the first iteration and if mode is 1, scale according 
    // to the norms of the columns of the initial Jacobian

    if (iter != 1) goto L80;
    if (mode == 2) goto L60;

    for (j = 1; j <= n; ++j) {
	diag[j] = wa2[j];
	if (wa2[j] == 0.0) diag[j] = 1.0;
    }

  L60:

    // on the first iteration, calculate the norm of the scaled x
    // and initialize the step bound delta

    for (j = 1; j <= n; ++j) 
      wa3[j] = diag[j] * x[j];
    
    xnorm = enorm(n, &wa3[1]);
    delta = factor * xnorm;
    if (delta == 0.0) delta = factor;

  L80:

    // form (q transpose)*f and store the first n components in qtf
    for (i = 1; i <= m; ++i) wa4[i] = f[i];
   
    for (j = 1; j <= n; ++j) {
	if (fjac[j + j * fjac_dim1] != 0.0) {
	    sum = 0.0;
	    for (i = j; i <= m; ++i) 
	      sum += fjac[i + j * fjac_dim1] * wa4[i];
	    temp = -sum / fjac[j + j * fjac_dim1];
	    for (i = j; i <= m; ++i) 
	      wa4[i] += fjac[i + j * fjac_dim1] * temp;
        }
	fjac[j + j * fjac_dim1] = wa1[j];
	qtf[j] = wa4[j];
    }

    // compute the norm of the scaled gradient
    gnorm = 0.0;
    if (fnorm != 0.0)
      for (j = 1; j <= n; ++j) {
	  l = ipvt[j];
	  if (wa2[l] != 0.0) {	
	      sum = 0.0;
	      for (i = 1; i <= j; ++i) 
		sum += fjac[i + j * fjac_dim1] * (qtf[i] / fnorm);
	      gnorm = max( gnorm, fabs(sum/wa2[l]) );
	  }
      }
    
    // test for convergence of the gradient norm
    if (gnorm <= gtol) info = 4;

    if (info != 0) goto L300;

    // rescale if necessary
    if (mode != 2)
      for (j = 1; j <= n; ++j) 
	diag[j] = max( diag[j], wa2[j] );
    
    // beginning of the inner loop
    
  L200:

    // determine the levenberg-marquardt parameter
    lmpar(n, &fjac[fjac_offset], ldfjac, &ipvt[1], &diag[1], &qtf[1], delta,
	  par, &wa1[1], &wa2[1], &wa3[1], &wa4[1]);
    
    // store the direction p and x + p. calculate the norm of p

    for (j = 1; j <= n; ++j) {
	wa1[j] = -wa1[j];
	wa2[j] = x[j] + wa1[j];
	wa3[j] = diag[j] * wa1[j];
    }
    pnorm = enorm(n, &wa3[1]);

    // on the first iteration, adjust the initial step bound
    if (iter == 1) delta = min(delta, pnorm);

    // evaluate the function at x + p and calculate its norm

    iflag = 1;
    (*fcn) (m, n, &wa2[1], &wa4[1], iflag);
    ++nfev;
    if (iflag < 0) goto L300;
    fnorm1 = enorm(m, &wa4[1]);

    // compute the scaled actual reduction

    actred = -1.0;
    if (p1 * fnorm1 < fnorm) 
	actred = 1.0 - sqr(fnorm1 / fnorm);

    // compute the scaled predicted reduction and
    // the scaled directional derivative

    for (j = 1; j <= n; ++j) {
	wa3[j] = 0.0;
	l = ipvt[j];
	temp = wa1[l];
	for (i = 1; i <= j; ++i) {
	    wa3[i] += fjac[i + j * fjac_dim1] * temp;
	}
    }

    temp1 = enorm(n, &wa3[1]) / fnorm;
    temp2 = sqrt(par) * pnorm / fnorm;
    prered = sqr(temp1) + sqr(temp2) / p5;
    dirder = -(sqr(temp1) + sqr(temp2));

    // compute the ratio of the actual to the predicted reduction
    ratio = 0.0;
    if (prered != 0.0) ratio = actred / prered;

    // update the step bound
    if (ratio > p25) goto L240;

    if (actred >= 0.0) temp = p5;
    if (actred < 0.0) 	temp = p5 * dirder / (dirder + p5 * actred);
    if (p1 * fnorm1 >= fnorm || temp < p1) temp = p1;

    delta = temp * min(delta, pnorm / p1);
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
    for (j = 1; j <= n; ++j) {
	x[j] = wa2[j];
	wa2[j] = diag[j] * x[j];
    }

    for (i = 1; i <= m; ++i) {
	f[i] = wa4[i];
    }
    xnorm = enorm(n, &wa2[1]);
    fnorm = fnorm1;
    ++iter;

  L290:

    // tests for convergence
    if (fabs(actred) <= ftol && prered <= ftol 
	&& p5 * ratio <= 1.0) info = 1;
   
    if (delta <= xtol * xnorm) info = 2;
   
    if (fabs(actred) <= ftol && prered <= ftol 
	&& p5 * ratio <= 1.0 && info == 2) info = 3;
   
    if (info != 0) goto L300;
   
    // tests for termination and stringent tolerances

    if (nfev >= maxfev) info = 5;
   
    if (fabs(actred) <= DBL_EPSILON && prered <= DBL_EPSILON 
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
    if (nprint > 0) (*fcn) (m, n, &x[1], &f[1], iflag);
}


static void lmpar (int n, double *r, int ldr, int *ipvt, double *diag, double *qtb,
		   double delta, double& par, double *x, double *sdiag,
		   double *wa1, double *wa2)
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
    static double p1 = 0.1;
    static double p001 = 0.001;

    int r_dim1, r_offset;
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
    --wa2;
    --wa1;
    --sdiag;
    --x;
    --qtb;
    --diag;
    --ipvt;
    r_dim1 = ldr;
    r_offset = r_dim1 + 1;
    r -= r_offset;

    // compute and store in x the gauss-newton direction. if the
    // Jacobian is rank-deficient, obtain a least squares solution

    nsing = n;
    for (j = 1; j <= n; ++j) {
	wa1[j] = qtb[j];
	if (r[j + j * r_dim1] == 0.0 && nsing == n) nsing = j - 1;
	if (nsing < n) wa1[j] = 0.0;
    }

    if (nsing >= 1)
      for (k = 1; k <= nsing; ++k) {
	  j = nsing - k + 1;
	  wa1[j] /= r[j + j * r_dim1];
	  temp = wa1[j];
	  jm1 = j - 1;
	  if (jm1 >= 1)
	    for (i = 1; i <= jm1; ++i) 
	      wa1[i] -= r[i + j * r_dim1] * temp;
      }
    
    for (j = 1; j <= n; ++j) {
	l = ipvt[j];
	x[l] = wa1[j];
    }

    // initialize the iteration counter
    // evaluate the function at the origin, and test
    // for acceptance of the gauss-newton direction

    iter = 0;
    for (j = 1; j <= n; ++j) 
      wa2[j] = diag[j] * x[j];
    
    dxnorm = enorm(n, &wa2[1]);
    fp = dxnorm - delta;
    if (fp <= p1 * delta) goto L220;

    // if the Jacobian is not rank deficient, the newton
    // step provides a lower bound, parl, for the zero of
    // the function. otherwise set this bound to zero

    parl = 0.0;
    if (nsing < n) goto L120;

    for (j = 1; j <= n; ++j) {
	l = ipvt[j];
	wa1[j] = diag[l] * (wa2[l] / dxnorm);
    }

    for (j = 1; j <= n; ++j) {
	sum = 0.0;
	jm1 = j - 1;
	if (jm1 >= 1)
	  for (i = 1; i <= jm1; ++i) 
	    sum += r[i + j * r_dim1] * wa1[i];
	wa1[j] = (wa1[j] - sum) / r[j + j * r_dim1];
    }
    temp = enorm(n, &wa1[1]);
    parl = fp / delta / temp / temp;
    
  L120:
    
    // calculate an upper bound, paru, for the zero of the function

    for (j = 1; j <= n; ++j) {
	sum = 0.0;
	for (i = 1; i <= j; ++i) 
	    sum += r[i + j * r_dim1] * qtb[i];
	l = ipvt[j];
	wa1[j] = sum / diag[l];
    }
    gnorm = enorm(n, &wa1[1]);
    paru = gnorm / delta;
    if (paru == 0.0) paru = DBL_MIN / min(delta, p1);
    
    // if the input par lies outside of the interval (parl,paru),
    // set par to the closer endpoint

    par = max(par, parl);
    par = min(par, paru);
    if (par == 0.0) par = gnorm / dxnorm;

    // beginning of an iteration

  L150:

    ++iter;

    // evaluate the function at the current value of par
    if (par == 0.0) par = max( DBL_MIN, p001 * paru );

    temp = sqrt(par);
    for (j = 1; j <= n; ++j) 
	wa1[j] = temp * diag[j];

    qrslov(n, &r[r_offset], ldr, &ipvt[1], &wa1[1], &qtb[1], &x[1], &sdiag[1] ,&wa2[1]);

    for (j = 1; j <= n; ++j) 
	wa2[j] = diag[j] * x[j];
  
    dxnorm = enorm(n, &wa2[1]);
    temp = fp;
    fp = dxnorm - delta;

    // if the function is small enough, accept the current value
    // of par. also test for the exceptional cases where parl
    // is zero or the number of iterations has reached 10

    if (fabs(fp) <= p1 * delta || parl == 0.0 && fp <= temp 
	&& temp < 0.0 || iter == 10) goto L220;

    // compute the newton correction.

    for (j = 1; j <= n; ++j) {
	l = ipvt[j];
	wa1[j] = diag[l] * (wa2[l] / dxnorm);
    }

    for (j = 1; j <= n; ++j) {
	wa1[j] /= sdiag[j];
	temp = wa1[j];
	jp1 = j + 1;
	if (n >= jp1)
	  for (i = jp1; i <= n; ++i) 
	    wa1[i] -= r[i + j * r_dim1] * temp;
    }
    
    temp = enorm(n, &wa1[1]);
    parc = fp / delta / temp / temp;

    // depending on the sign of the function, update parl or paru
    if (fp > 0.0) parl = max(parl, par);
    if (fp < 0.0) paru = min(paru, par);

    // compute an improved estimate for par
    par = max( parl, par + parc );

    // end of an iteration
    goto L150;

  L220:
    
    // termination
    if (iter == 0) par = 0.0;
}	


static void qrslov (int n, double *r, int ldr, int *ipvt, 
		    double *diag, double *qtb, double *x, double *sdiag, double *wa)
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
    static double p5 = 0.5;
    static double p25 = 0.25;

    int r_dim1, r_offset;
    double temp;
    int i, j, k, l;
    double cotan;
    int nsing;
    double qtbpj;
    int jp1, kp1;
    double tan_, cos_, sin_, sum;

    // Parameter adjustments
    --wa;
    --sdiag;
    --x;
    --qtb;
    --diag;
    --ipvt;
    r_dim1 = ldr;
    r_offset = r_dim1 + 1;
    r -= r_offset;

    // copy r and (q transpose)*b to preserve input and initialize s
    // in particular, save the diagonal elements of r in x

    for (j = 1; j <= n; ++j) {
	for (i = j; i <= n; ++i) 
	  r[i + j * r_dim1] = r[j + i * r_dim1];	
	x[j] = r[j + j * r_dim1];
	wa[j] = qtb[j];
    }

    // eliminate the diagonal matrix d using a givens rotation

    for (j = 1; j <= n; ++j) {

        // prepare the row of d to be eliminated, locating the
        // diagonal element using p from the qr factorization

	l = ipvt[j];
	if (diag[l] == 0.0) goto L90;

	for (k = j; k <= n; ++k) sdiag[k] = 0.0;
	sdiag[j] = diag[l];

	// the transformations to eliminate the row of d
        // modify only a single element of (q transpose)*b
        // beyond the first n, which is initially zero

	qtbpj = 0.0;
	for (k = j; k <= n; ++k) {

	    // determine a givens rotation which eliminates the
	    // appropriate element in the current row of d

	    if (sdiag[k] == 0.0) goto L70;
	    if (fabs(r[k + k * r_dim1]) >= fabs(sdiag[k])) goto L40;
	    cotan = r[k + k * r_dim1] / sdiag[k];
	    sin_ = p5 / sqrt(p25 + p25 * sqr(cotan));
	    cos_ = sin_ * cotan;
	    goto L50;
	    
	  L40:

	    tan_ = sdiag[k] / r[k + k * r_dim1];
	    cos_ = p5 / sqrt(p25 + p25 * sqr(tan_) );
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
	    if (n < kp1) goto L70;
	    for (i = kp1; i <= n; ++i) {
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
    for (j = 1; j <= n; ++j) {
	if (sdiag[j] == 0.0 && nsing == n) nsing = j - 1;
	if (nsing < n) wa[j] = 0.0;      
    }

    if (nsing >= 1)
      for (k = 1; k <= nsing; ++k) {
	  j = nsing - k + 1;
	  sum = 0.0;
	  jp1 = j + 1;
	  if (nsing >= jp1)
	    for (i = jp1; i <= nsing; ++i)
	      sum += r[i + j * r_dim1] * wa[i];
	  wa[j] = (wa[j] - sum) / sdiag[j];
      }
    
    // permute the components of z back to components of x
    for (j = 1; j <= n; ++j) {
	l = ipvt[j];
	x[l] = wa[j];
    }
} 



static void qrfac (int m, int n, double *a, int lda, int pivot, int *ipvt,
		   int lipvt, double *rdiag, double *acnorm, double *wa)
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
    int a_dim1, a_offset;
    int kmax;
    double temp;
    int i, j, k, minmn;
    double ajnorm;
    int jp1;
    double sum;

    // Parameter adjustments
    --wa;
    --acnorm;
    --rdiag;
    --ipvt;
    a_dim1 = lda;
    a_offset = a_dim1 + 1;
    a -= a_offset;

    // compute the initial column norms and initialize several arrays
    for (j = 1; j <= n; ++j) {
	acnorm[j] = enorm(m, &a[j * a_dim1 + 1]);
	rdiag[j] = acnorm[j];
	wa[j] = rdiag[j];
	if (pivot) {
	    ipvt[j] = j;
	}
    }

    // reduce a to r with householder transformations
    minmn = min(m,n);
    for (j = 1; j <= minmn; ++j) {
	if ( ! pivot ) goto L40;

	// bring the column of largest norm into the pivot position
	kmax = j;
	for (k = j; k <= n; ++k) 
	    if (rdiag[k] > rdiag[kmax])	kmax = k;
       
	if (kmax == j) goto L40;
	for (i = 1; i <= m; ++i) {
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

	ajnorm = enorm( m - j + 1, &a[j + j * a_dim1]);
	if (ajnorm == 0.0) goto L100;
	if (a[j + j * a_dim1] < 0.0) ajnorm = -ajnorm;
	for (i = j; i <= m; ++i) 
	  a[i + j * a_dim1] /= ajnorm;
	a[j + j * a_dim1] += 1.0;

	// apply the transformation to the remaining columns
	// and update the norms

	jp1 = j + 1;
	if (n < jp1) goto L100;

	for (k = jp1; k <= n; ++k) {

	    sum = 0.0;
	    for (i = j; i <= m; ++i) 
	      sum += a[i + j * a_dim1] * a[i + k * a_dim1];
	    
	    temp = sum / a[j + j * a_dim1];
	    for (i = j; i <= m; ++i) 
	      a[i + k * a_dim1] -= temp * a[i + j * a_dim1];
	    
	    if ( ! pivot || rdiag[k] == 0.0 ) goto L80;
	    temp = a[j + k * a_dim1] / rdiag[k];

	    rdiag[k] *= sqrt((max( 0.0, 1.0 - sqr(temp) )));
	    if (0.05 * sqr(rdiag[k] / wa[k]) > DBL_EPSILON) goto L80;
	    rdiag[k] = enorm(m-j, &a[jp1 + k * a_dim1]);
	    wa[k] = rdiag[k];
	  L80: ;
	}
      L100:
	rdiag[j] = -ajnorm;
    }
}


static double enorm (int n, double *x)
//
//     given an n-vector x, this function calculates the 
//     euclidean norm of x. 
//
//     the euclidean norm is computed by accumulating the sum of 
//     squares in three different sums. the sums of squares for the 
//     small and large components are scaled so that no overflows 
//     occur. non-destructive underflows are permitted. underflows 
//     and overflows do not occur in the computation of the unscaled 
//     sum of squares for the intermediate components. 
//     the definitions of small, intermediate and large components 
//     depend on two constants, rdwarf and rgiant. the main 
//     restrictions on these constants are that rdwarf**2 not 
//     underflow and rgiant**2 not overflow. the constants 
//     given here are suitable for every known computer. 
//
//     the function statement is 
//
//       double precision function enorm(n,x) 
//
//     where 
//
//       n is a positive integer input variable. 
//
//       x is an input array of length n. 
//     burton s. garbow, kenneth e. hillstrom, jorge j. more 
//
{
    static double rdwarf = 3.834e-20;
    static double rgiant = 1.304e19;
    double ret_val=0.0, xabs, x1max, x3max, s1, s2, s3, agiant, floatn;
    int i;

    // Parameter adjustments
    --x;

    s1 = s2 = s3 = x1max = x3max = 0.0;
    floatn = (double) n;
    agiant = rgiant / floatn;

    for (i = 1; i <= n; i++) {

	xabs = fabs(x[i]);
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
    ret_val = x1max * sqrt(s1 + s2 / x1max / x1max);
    goto L130;

  L100:
    if (s2 == 0.0) goto L110;
    if (s2 >= x3max) 
	ret_val = sqrt(s2 * (1.0 + x3max / s2 * (x3max * s3)));    
    if (s2 < x3max) 
	ret_val = sqrt(x3max * (s2 / x3max + x3max * s3));
    goto L130;

  L110:
    ret_val = x3max * sqrt(s3);

  L130:
    return ret_val;
}


static void fdjac2 (LMFunction fcn, int m, int n, double *x, double *fvec, double *fjac, 
		    int ldfjac, int& iflag, double epsfcn, double *wa)
//
//     this subroutine computes a forward-difference approximation 
//     to the m by n Jacobian matrix associated with a specified 
//     problem of m functions in n variables. 
//
//     the subroutine statement is 
//
//       subroutine fdjac2(fcn,m,n,x,fvec,fjac,ldfjac,iflag,epsfcn,wa) 
//
//     where 
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
//       m is a positive integer input variable set to the number 
//         of functions. 
//
//       n is a positive integer input variable set to the number 
//         of variables. n must not exceed m. 
//
//       x is an input array of length n. 
//
//       fvec is an input array of length m which must contain the 
//         functions evaluated at x. 
//
//       fjac is an output m by n array which contains the 
//         approximation to the Jacobian matrix evaluated at x. 
//
//       ldfjac is a positive integer input variable not less than m 
//         which specifies the leading dimension of the array fjac. 
//
//       iflag is an integer variable which can be used to terminate 
//         the execution of fdjac2. see description of fcn. 
//
//       epsfcn is an input variable used in determining a suitable 
//         step length for the forward-difference approximation. this 
//         approximation assumes that the relative errors in the 
//         functions are of the order of epsfcn. if epsfcn is less 
//         than the machine precision, it is assumed that the relative 
//         errors in the functions are of the order of the machine 
//         precision. 
//
//       wa is a work array of length m. 
//
{
    int fjac_dim1, fjac_offset;
    double temp, h;
    int i, j;
    double eps;

    // Parameter adjustments 
    --wa;
    fjac_dim1 = ldfjac;
    fjac_offset = fjac_dim1 + 1;
    fjac -= fjac_offset;
    --fvec;
    --x;

    eps = sqrt((max(epsfcn, DBL_EPSILON)));

    for (j = 1; j <= n; ++j) {
	temp = x[j];
	h = eps * fabs(temp);
	if (h == 0.0)  h = eps;
	x[j] = temp + h;
	(*fcn) (m, n, &x[1], &wa[1], iflag);
	if (iflag < 0) return;
	x[j] = temp;
	for (i = 1; i <= m; ++i)
	    fjac[i + j * fjac_dim1] = (wa[i] - fvec[i]) / h;
    }
}
