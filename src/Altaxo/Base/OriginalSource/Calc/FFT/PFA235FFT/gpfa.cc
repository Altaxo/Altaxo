/*-----------------------------------------------------------------------------*\
| main routine of the GPFA-FFT algorithm                                gpfa.cc |
|                                                                               |
| templated implementation                                                      |
|                                                                               |
| Matpack Library Release 1.7.2                                                 |
| Copyright (C) 1991-1997 by Berndt M. Gammel                                   |
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

#include "gpfa.h"

//-----------------------------------------------------------------------------//
//
// gpfa: self-sorting in-place generalized prime factor (complex) fft 
//
// Definition:
// -----------
//        x(j) = sum(k=0,...,n-1) ( c(k) * exp(isign*2*i*j*k*pi/n) ) 
//
// Prototype:
// ----------
//   void gpfa (FLOAT a[], FLOAT b[], FLOAT trigs[], 
//              int inc, int jump, int n, int lot, int isign)
//
// Arguments:
// ----------
//   a      is first real input/output vector. The first element is 
//          indexed by a[0].
//
//   b      is first imaginary input/output vector. The first element is 
//          indexed by b[0].
//
//   trigs  is a table of twiddle factors, precalculated 
//          by calling function 'gpfasetup'. The first element is 
//          indexed by trigs[0].
//
//   inc    is the increment within each data vector 
//
//   jump   is the increment between data vectors 
//
//   n      is the length of the transforms which must be of the form
//          n = (2**ip) * (3**iq) * (5**ir) 
//
//   lot    is the number of transforms 
//
//   isign  = +1 for forward transform 
//          = -1 for inverse transform 
//
// References:
// ----------- 
//  1) Originally written by:
//
//           Clive Temperton 
//           Recherche en Prevision Numerique 
//           Atmospheric Environment Service, Canada 
//
//   2) For a mathematical development of the algorithm used, see: 
//
//            C. Temperton, "A Generalized Prime Factor FFT Algorithm for 
//            any n = (2**p)(3**q)(5**r)", SIAM J.Sci.Stat.Comp., May 1992. 
//
//  3) Conversion to C++ based on Clive Tempertons original algorithm 
//     by B. M. Gammel, Jan 1997. Worked over the whole code.
//
//  4) The original Fortran source files can be obtained freely from
//     ftp://ftp.earth.ox.ac.uc/pub/gpfa.tar.gz
//
//-----------------------------------------------------------------------------//

void gpfa (FLOAT a[], FLOAT b[], FLOAT trigs[], 
	   int inc, int jump, int n, int lot, int isign)
{
  int i,kk, nj[3];

  // adjust arguments
  --trigs;
  --b;
  --a;

  // decompose n into factors 2,3,5 
  // ------------------------------ 

  int nn = n,
      ifac = 2;

  for (int ll = 1; ll <= 3; ++ll) {
    kk = 0;
  L10:
    if (nn % ifac != 0) goto L20;
    ++kk;
    nn /= ifac;
    goto L10;
  L20:
    nj[ll - 1] = kk;
    ifac += ll;
  }

  // test arguments
  // --------------

  if (nn != 1) {
    Matpack.Error("gpfa: %d is not a legal value of n", n); 
    return;
  }

  if (isign != 1 && isign != -1) {
    Matpack.Error("gpfa: %d is not a legal value of isign = +1/-1", isign); 
    return;
  }

  int ip = nj[0], 
      iq = nj[1],
      ir = nj[2];

  // compute the transform
  // ---------------------

  i = 1;
  if (ip > 0) {
    gpfa2f (&a[1], &b[1], &trigs[1], inc, jump, n, ip, lot, isign);
    i += powii(2,ip) * 2;
  }
  if (iq > 0) {
    gpfa3f (&a[1], &b[1], &trigs[i], inc, jump, n, iq, lot, isign);
    i += powii(3,iq) * 2;
  }
  if (ir > 0) {
    gpfa5f (&a[1], &b[1], &trigs[i], inc, jump, n, ir, lot, isign);
  }
}

//-----------------------------------------------------------------------------//
