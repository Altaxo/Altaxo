/*-----------------------------------------------------------------------------*\
| complex FFT class for 1/2/3 dimensional FFT                          mpfft.cc |
|                                                                               |
| templated implementation                                                      |
|                                                                               |
| Last change: Jun 13, 2001							|
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

//-----------------------------------------------------------------------------//
// Implementation of class MpFFT<FLOAT>
//-----------------------------------------------------------------------------//

#include "mpfft.h"	// class definition
#include "gpfa.h"	// basic prototypes of GPFA algorithm

//-----------------------------------------------------------------------------//
// Decompose n into factors n = (2**p) * (3**q) * (5**r)
// Returns true if the decomposition is possible, otherwise return false
// if other factors than 2, 3, or 5 are involved.
//-----------------------------------------------------------------------------//

int MpFFT<FLOAT>::Factorize (int n, int pqr[3])
{
  int k, ifac = 2;
  for (int l = 1; l <= 3; l++) {
    k = 0;
  L10:
    if (n % ifac != 0) goto L20;
    ++k;
    n /= ifac;
    goto L10;
  L20:
    pqr[l-1] = k;
    ifac += l;
  }
  return (n == 1); // return false if decomposition failed
}

//-----------------------------------------------------------------------------//
// destructor: remove trigonometric factors
//-----------------------------------------------------------------------------//

MpFFT<FLOAT>::~MpFFT (void)
{
  if (trigs) delete [] trigs;
  trigs = NULL;
  ndim = id = 0;
}

//-----------------------------------------------------------------------------//
// copy constructor
//-----------------------------------------------------------------------------//

MpFFT<FLOAT>::MpFFT (const MpFFT<FLOAT>& fft)
{
  // copy all elements
  id = fft.id;
  ndim = fft.ndim;
  trisize = fft.trisize;
  row_order = fft.row_order;
  for (int i = 0; i < 3; i++) {
    dim[i] = fft.dim[i];
    trindex[i] = fft.trindex[i];
  }

  // allocate and copy trigs
  trigs = new FLOAT[trisize];
  memcpy((void*)trigs,(const void*)fft.trigs, trisize*sizeof(FLOAT));
}

//-----------------------------------------------------------------------------//
// assignment
//-----------------------------------------------------------------------------//

MpFFT<FLOAT>& MpFFT<FLOAT>::operator = (const MpFFT<FLOAT>& fft)
{
  // clear current values
  if (trigs) delete [] trigs;

  // copy stuff
  id = fft.id;
  ndim = fft.ndim;
  trisize = fft.trisize;
  row_order = fft.row_order;
  for (int i = 0; i < 3; i++) 
		{
    dim[i] = fft.dim[i];
    trindex[i] = fft.trindex[i];
  }

  // allocate and copy trigs
  trigs = new FLOAT[trisize];
  memcpy((void*)trigs,(const void*)fft.trigs, trisize*sizeof(FLOAT));

  return *this;
}

//-----------------------------------------------------------------------------//
// uninitialized setup
//-----------------------------------------------------------------------------//

MpFFT<FLOAT>::MpFFT (void) : id(0), ndim(0), row_order(def_row_order), trigs(NULL)
{
  // This is needed for passing an arbitrary FFT instance as argument.
}

//-----------------------------------------------------------------------------//
// 1-dimensional setup
//-----------------------------------------------------------------------------//

MpFFT<FLOAT>::MpFFT (int n1) : ndim(1), row_order(def_row_order)
{
  int pqr[3];
  if ( Factorize(n1,pqr) == false )
    Matpack.Error("MpFFT: %d is not a legal value for dimension",n1);
  dim[0] = id = n1;
  trisize = 2*(powii(2,pqr[0])+powii(3,pqr[1])+powii(5,pqr[2]));
  trigs = new FLOAT[trisize];
  gpfasetup(trigs,n1);
}

//-----------------------------------------------------------------------------//
// 2-dimensional setup
//-----------------------------------------------------------------------------//

MpFFT<FLOAT>::MpFFT (int n1, int n2) : ndim(2), row_order(def_row_order)
{
  int pqr[2][3], d[2];
  if ( Factorize(n1,pqr[0]) == false )
    Matpack.Error("MpFFT: %d is not a legal value for dimension 1", n1);
  if ( Factorize(n2,pqr[1]) == false )
    Matpack.Error("MpFFT: %d is not a legal value for dimension 2", n2);

  dim[0] = n1; 
  dim[1] = n2;
  id = row_order ? n2 : n1;
  for (int i = 0; i <= 1; i++)
    d[i] = 2*(powii(2,pqr[i][0])+powii(3,pqr[i][1])+powii(5,pqr[i][2]));
  trisize = d[0]+d[1];
  trigs = new FLOAT[trisize];
  trindex[0] = 0;
  trindex[1] = d[0]; 
  for (int i = 0; i <= 1; i++)
    gpfasetup(trigs+trindex[i],dim[i]);
}

//-----------------------------------------------------------------------------//
// 3-dimensional setup
//-----------------------------------------------------------------------------//

MpFFT<FLOAT>::MpFFT (int n1, int n2, int n3) : ndim(3), row_order(def_row_order)
{
  int pqr[3][3], d[3];
  if ( Factorize(n1,pqr[0]) == false )
    Matpack.Error("MpFFT: %d is not a legal value for dimension 1", n1);
  if ( Factorize(n2,pqr[1]) == false )
    Matpack.Error("MpFFT: %d is not a legal value for dimension 2", n2);
  if ( Factorize(n3,pqr[2]) == false )
    Matpack.Error("MpFFT: %d is not a legal value for dimension 3", n3);

  dim[0] = n1;
  dim[1] = n2; 
  dim[2] = n3;
  id = row_order ? n3 : n1;
  for (int i = 0; i <= 2; i++)
    d[i] = 2*(powii(2,pqr[i][0])+powii(3,pqr[i][1])+powii(5,pqr[i][2]));
  trisize = d[0]+d[1]+d[2];
  trigs = new FLOAT[trisize];
  trindex[0] = 0;  
  trindex[1] = d[0]; 
  trindex[2] = d[0]+d[1];
  for (int i = 0; i <= 2; i++)
    gpfasetup(trigs+trindex[i],dim[i]);
}

//-----------------------------------------------------------------------------//
// Set information about the row order and the leading dimension.
// If the row order argument is non-zero then the d-dimensional data
// are assumed to be stored in row order (the C convention), otherwise if
// zero then column order (the Fortran convention) is assumed.
// The leading dimension can be choosen different from the first/last 
// dimension of the array. This can give a significant
// speed increase on some vector machines avoiding memory-bank conflicts.
// If the data are stored column-ordered (Fortran style) then the leading
// dimension is the first dimension, otherwise if the data are stored row-
// ordered (C style) then the last dimension is the leading dimension 
// and will be padded!
//-----------------------------------------------------------------------------//

void MpFFT<FLOAT>::SetOrder (int row, int lead)
{
  if (ndim == 0) 
    Matpack.Error("MpFFT::SetOrder: no dimensions are specified");

  // set row/column ordering
  row_order = row;

  // set corresponding leading dimension
  if (row_order) /* C convention */ {
    
    if (lead <= 0)
      id = dim[ndim-1];
    else if (lead < dim[ndim-1])
      Matpack.Error("MpFFT::SetOrder: (%d) is smaller than last dimension (%d)",
		    lead, dim[ndim-1]);
    else
      id = lead;

  } else /* column order, Fortran convention */ {
    
    if (lead <= 0) 
      id = dim[0];
    else if (lead < dim[0])
      Matpack.Error("MpFFT::SetOrder: (%d) is smaller than first dimension (%d)",
		    lead, dim[0]);
    else
      id = lead;
  }
}

//-----------------------------------------------------------------------------//

void  MpFFT<FLOAT>::GetOrder (int &row, int &lead)
{
  row  = row_order;
  lead = id;
}

//-----------------------------------------------------------------------------//
// Complex forward/backward FFT for 1/2/3 dimensions
// Interface with complex<FLOAT> data vector 
//-----------------------------------------------------------------------------//

int MpFFT<FLOAT>::operator () (complex<FLOAT> c[], int isign)
{
  // access as FLOAT - it is important that complex class has base type FLOAT
  FLOAT *d = (FLOAT*)c;

  if (ndim == 0) {

    Matpack.Error("MpFFT: no dimensions have been specified");

  } else if (ndim == 1) {

    // leading dimension is ignored, row_order doesn't matter
    gpfa(d, d+1, trigs, 2, 0, dim[0], 1, -isign);

  } else if (ndim == 2) {

    int one,two;
    if (row_order) {	// C style
      one = 0; two = 1; 
    } else { 		// column order (Fortran style)
      one = 1; two = 0; 
    }

    int lot = (dim[one] + nthreads - 1) / nthreads;
    for (int i = 0; i < nthreads; ++i) {
      int offset = 2 * id * lot * i;
      gpfa(d+offset, d+offset+1, trigs+trindex[two], 
	   2, 2*id, dim[two], min(lot,dim[one]-i*lot), -isign);
    }
    gpfa(d, d+1, trigs+trindex[one], 
	 2*id, 2, dim[one], dim[two], -isign);

  } else if (ndim == 3) {

    int one,two,three;
    if (row_order) {	// C style
      one = 0; two = 1; three = 2;
    } else { 		// column order (Fortran style)
      one = 2; two = 1; three = 0; 
    }

    int lot = (dim[two] * dim[one] + nthreads - 1) / nthreads;
    for (int i = 0; i < nthreads; ++i) {
      int offset = 2 * id * lot * i;
      gpfa(d+offset, d+offset+1, trigs+trindex[three], 
	   2, 2*id, dim[three], min(lot,dim[two]*dim[one]-i*lot), -isign);
    }
    for (int i = 0; i < dim[one]; ++i) {
      int offset = 2 * id * dim[two] * i;
      gpfa(d+offset, d+offset+1, trigs+trindex[two], 
	   2*id, 2, dim[two], dim[three], -isign);
    }
    lot = (id * dim[two] + nthreads - 1) / nthreads;
    for (int i = 0; i < nthreads; ++i) {
      int offset = 2 * lot * i;
      gpfa(d+offset, d+offset+1, trigs+trindex[one],
	   2*id*dim[two], 2, dim[one], min(lot,id*dim[two]-i*lot), -isign);
    }
  }
  return 1;
}

//-----------------------------------------------------------------------------//
// Complex forward/backward FFT for 1/2/3 dimensions
// Interface with separate FLOAT vectors for real and imaginary part
//-----------------------------------------------------------------------------//

int MpFFT<FLOAT>::operator () (FLOAT re[], FLOAT im[], int isign)
{
  if (ndim == 0) {

    Matpack.Error("MpFFT: no dimensions have been specified");

  } else if (ndim == 1) {

    // leading dimension is ignored, row_order doesn't matter
    gpfa(re, im, trigs, 1, 0, dim[0], 1, -isign);

  } else if (ndim == 2) {

    int one,two;
    if (row_order) {	// C style
      one = 0; two = 1; 
    } else { 		// column order (Fortran style)
      one = 1; two = 0; 
    }

    int lot = (dim[one] + nthreads - 1) / nthreads;
    for (int i = 0; i < nthreads; ++i) {
      int offset = id * lot * i;
      gpfa(re+offset, im+offset, trigs+trindex[two], 
	   1, id, dim[two], min(lot,dim[one]-i*lot), -isign);
    }
    gpfa(re, im, trigs+trindex[one], 
	 id, 1, dim[one], dim[two], -isign);

  } else if (ndim == 3) {

    int one,two,three;
    if (row_order) {	// C style
      one = 0; two = 1; three = 2;
    } else { 		// column order (Fortran style)
      one = 2; two = 1; three = 0; 
    }

    int lot = (dim[two] * dim[one] + nthreads - 1) / nthreads;
    for (int i = 0; i < nthreads; ++i) {
      int offset = id * lot * i;
      gpfa(re+offset, im+offset, trigs+trindex[three], 
	   1, id, dim[three], min(lot,dim[two]*dim[one]-i*lot), -isign);
    }
    for (int i = 0; i < dim[one]; ++i) {
      int offset = id * dim[two] * i;
      gpfa(re+offset, im+offset, trigs+trindex[two], 
	   id, 1, dim[two], dim[three], -isign);
    }
    lot = (id * dim[two] + nthreads - 1) / nthreads;
    for (int i = 0; i < nthreads; ++i) {
      int offset = lot * i;
      gpfa(re+offset, im+offset, trigs+trindex[one],
	   id*dim[two], 1, dim[one], min(lot,id*dim[two]-i*lot), -isign);
    }
  }
  return 1;
}

//-----------------------------------------------------------------------------//
