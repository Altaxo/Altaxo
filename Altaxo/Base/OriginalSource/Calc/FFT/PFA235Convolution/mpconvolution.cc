/*-----------------------------------------------------------------------------*\
| class for 1/2/3 dimensional convolution/deconvolution        mpconvolution.cc |
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
// Implementation of class MpConvolution<FLOAT>
//-----------------------------------------------------------------------------//

#include "mpfft.h"	// class definition

//-----------------------------------------------------------------------------//


MpConvolution<FLOAT>::MpConvolution (void) 
  : MpFFT<FLOAT>()
{ }

//-----------------------------------------------------------------------------//

MpConvolution<FLOAT>::MpConvolution (int n1) 
  : MpFFT<FLOAT>(n1)
{ }

//-----------------------------------------------------------------------------//

MpConvolution<FLOAT>::MpConvolution (int n1, int n2) 
  : MpFFT<FLOAT>(n1,n2)
{ }

//-----------------------------------------------------------------------------//

MpConvolution<FLOAT>::MpConvolution (int n1, int n2, int n3) 
  : MpFFT<FLOAT>(n1,n2,n3)
{ }

//-----------------------------------------------------------------------------//
// int MpConvolution<FLOAT>::operator () (FLOAT data[], FLOAT response[], 
//	  			          FLOAT result[], FLOAT scratch[],
//				          int isign = forward)
//
// Description
// -----------
// Convolves or deconvolves a real-valued data set data[] (including any
// user supplied zero padding) with a response function response[].
// The result is returned in the array result[]. All arrays including
// the scratch[] array must have the same dimensions (or larger).
// The data set (and of course the other arrays) can be either one-dimensional,
// two-dimensional, or three-dimensional, d = 1,2,3.  Each dimension must be 
// of the form n = (2**p) * (3**q) * (5**r), because of the underlying FFT.
// The d-dimensional data can be either single precision (FLOAT := float) 
// or double precision (FLOAT := double).
//
// Arguments
// ---------
//   FLOAT data[]            The real-valued data set. Note, that you have to
//                           care for end effects by zero padding. This means, 
//                           that you have to pad the data with a number of zeros
//                           on one end equal to the maximal positive duration
//                           or maximal negative duration of the response function,
//                           whichever is larger!!
//
//   FLOAT response[]        The response function must be stored in wrap-around
//                           order. This means that the first half of the array
//                           response[] (in each dimension) contains the impulse
//                           response function at positive times, while the second
//                           half of the array contains the impulse response
//                           function at negative times, counting down from the
//                           element with the highest index. The array must have 
//                           at least the size of the data array.
//
//   FLOAT result[]          The result array. It must have 
//                           at least the size of the data array.
//
//   FLOAT scratch[] = 0     A work array. If a NULL pointer is passed the
//			     work array is allocated and freed auotomatically.
//                           If the array is given by the user it must have 
//                           at least the size of the data array.
//
//   int isign = forward     If isign == forward a convolution is performed. 
//                           If isign == inverse then a deconvolution is performed.
//
// Return values
// -------------
// In the case of a convolution (isign == forward) the value "true" is returned
// always. In the case of deconvolution (isign == inverse) the value "false" is
// returned if the FFT transform of the response function is exactly zero for 
// some value. This indicates that the original convolution has lost all 
// information at this particular frequency, so that a reconstruction is not
// possible. If the transform of the response function is non-zero everywhere
// the deconvolution can be performed and the value "true" is returned.
//
// Implementation notes
// --------------------
// The FFT of the real-valued data array and the real-valued response array is
// calculated in one step. This is done by regarding the two arrays
// as the real part and the imaginary part of one complex-valued array.
// 
// Possible improvements
// ---------------------
// * When doing the backtransform only a real transform is necessary.
//   The upper half of the result/scratch arrays is redundant.
//   (comment: "symmetry"). This should be used to speed-up the backtransform.
//
// * 2D and 3D versions are not yet available !!!
//
//-----------------------------------------------------------------------------//

int MpConvolution<FLOAT>::operator () (FLOAT data[], FLOAT response[], 
				       FLOAT result[], FLOAT scratch[],
				       int isign)
{
  // return status
  int status = true;

  // get total size of data array
  int size = 0;
  if (ndim == 0) {
    Matpack.Error("MpConvolution::operator(): no dimensions have been specified");
  } else if (ndim == 1) {
    size = dim[0];
  } else if (ndim == 2) {
    size = row_order ? (dim[0] * id) : (id * dim[1]);
  } else if (ndim == 3) {
    size = row_order ? (dim[0] * dim[1] * id) : (id * dim[1] * dim[2]);
  }

  // allocate the scratch array
  int auto_scratch = false;
  if ( ! scratch ) {
    scratch = new  FLOAT [size];
    auto_scratch = true;
  }

  //---------------------------------------------------------------------------//
  //  1-dimensional convolution (original data not are overwritten)
  //---------------------------------------------------------------------------//

  if (ndim == 1) {

    // First copy the arrays data and response to result and scratch,
    // respectively, to prevent overwriting of the original data.
    memcpy(result,data, size*sizeof(FLOAT));
    memcpy(scratch,response, size*sizeof(FLOAT));

    // transform both arrays simultaneously - this is a forward FFT
    MpFFT<FLOAT>::operator()(result,scratch,forward);
    
    // multiply FFTs to convolve
    int n = dim[0], n2 = n/2;

    if (isign == forward) {
      
      FLOAT scale = 0.25/n;
      result[0] *= scratch[0] / n;
      scratch[0] = 0;
      for (int i = 1; i <= n2; i++) {
	FLOAT rr = result[i]  + result[n-i], 
	      ri = result[i]  - result[n-i], 
	      sr = scratch[i] + scratch[n-i],
	      si = scratch[i] - scratch[n-i];
	result[i]  = scale * (rr*sr + ri*si);		// real part
	scratch[i] = scale * (si*sr - ri*rr);		// imaginary part 
	result[n-i]  = result[i];			// symmetry
	scratch[n-i] = -scratch[i]; 			// symmetry
      }
      
    } else /* isign == inverse */ {

      FLOAT mag;
      if ((mag = sqr(scratch[0])) == 0.0) {		// check for zero divide
	status = false;
	goto ErrorExit;
      }
      result[0] *= scratch[0] / mag / n;
      scratch[0] = 0;
      for (int i = 1; i <= n2; i++) {
	FLOAT rr = result[i] + result[n-i], 
    	      ri = result[i] - result[n-i], 
	      sr = scratch[i] + scratch[n-i],
	      si = scratch[i] - scratch[n-i];
	if ((mag = sr*sr + ri*ri) == 0.0)  {		// check for zero divide
	  status = false;
	  goto ErrorExit;
	}
	result[i]  =  (rr*sr - ri*si) / (n*mag);	// real part
	scratch[i] =  (si*sr + ri*rr) / (n*mag);	// imaginary part 
	result[n-i]  = result[i];			// symmetry
	scratch[n-i] = -scratch[i]; 			// symmetry
      }
    }

    // transform back - this is an inverse FFT
    MpFFT<FLOAT>::operator()(result,scratch,inverse);

  //---------------------------------------------------------------------------//
  //  2-dimensional convolution
  //---------------------------------------------------------------------------//

  } else if (ndim == 2) {
    
    int n = dim[0],
        m = dim[1];

    // set imaginary parts to zero
    memset(result,0,size*sizeof(FLOAT));  // imaginary part of data
    memset(scratch,0,size*sizeof(FLOAT)); // imaginary part of response

    // transform both arrays - this is a forward FFT
    MpFFT<FLOAT>::operator()(data,result,forward);
    MpFFT<FLOAT>::operator()(response,scratch,forward);

    if (isign == forward) { 
      FLOAT scale = 1.0/n/m;
      for (int i = 0; i < n; i++)
	for (int j = 0; j < m; j++) {
	  int l = row_order ? i*id+j : j*id+i;
	  // do complex multiplication with three real multiplications
	  // (a+ib)(c+id) = ( ac-bd ) + i( (a+b)(c+d)-ac-bd )
	  FLOAT ac = data[l]*response[l],
	        bd = result[l]*scratch[l];
	  scratch[l] = scale*((data[l]+result[l])*(response[l]+scratch[l])-ac-bd);
	  result[l]  = scale*(ac-bd);
	}
    } else /* isign == inverse */ {
      FLOAT mag, scale = 1.0/n/m;
      for (int i = 0; i < n; i++)
	for (int j = 0; j < m; j++) {
	  int l = row_order ? i*id+j : j*id+i;
	  // do complex division (a+ib)/(c+id) = 
	  // (a+ib)(c-id)/(cc+dd) = [(ac+bd) + i((a+b)(c-d)-ac+bd)]/(cc+dd)
	  if ((mag = sqr(response[l])+sqr(scratch[l])) == 0.0) {
	    status = false;
	    goto ErrorExit;
	  }
	  mag = scale/mag;
	  FLOAT ac = data[l]*response[l],
	        bd = result[l]*scratch[l];
	  scratch[l] = mag*((data[l]+result[l])*(response[l]-scratch[l])-ac+bd);
	  result[l]  = mag*(ac+bd);
	}
    } 

    // transform back - this is an inverse FFT
    MpFFT<FLOAT>::operator()(result,scratch,inverse);

  //---------------------------------------------------------------------------//
  //  3-dimensional convolution
  //---------------------------------------------------------------------------//

  } else if (ndim == 3) {

    int n = dim[0],
        m = dim[1],
        p = dim[2];

    // set imaginary parts to zero
    memset(result,0,size*sizeof(FLOAT));  // imaginary part of data
    memset(scratch,0,size*sizeof(FLOAT)); // imaginary part of response

    // transform both arrays - this is a forward FFT
    MpFFT<FLOAT>::operator()(data,result,forward);
    MpFFT<FLOAT>::operator()(response,scratch,forward);

    if (isign == forward) {
      FLOAT scale = 1.0/n/m/p;
      for (int i = 0; i < n; i++)
	for (int j = 0; j < m; j++) 
	  for (int k = 0;  k < p; k++) {
	    int l = row_order ? (i*m+j)*id+k : (k*m+j)*id+i;
	    // do complex multiplication with three real multiplications
	    // (a+ib)(c+id) = ( ac-bd ) + i( (a+b)(c+d)-ac-bd )
	    FLOAT ac = data[l]*response[l],
	          bd = result[l]*scratch[l];
	    scratch[l] = scale*((data[l]+result[l])*(response[l]+scratch[l])-ac-bd);
	    result[l]  = scale*(ac-bd);
	  }
    } else /* isign == inverse */ {
      FLOAT mag, scale = 1.0/n/m/p;
      for (int i = 0; i < n; i++)
	for (int j = 0; j < m; j++) 
	  for (int k = 0;  k < p; k++) {
	    int l = row_order ? (i*m+j)*id+k : (k*m+j)*id+i;
	    // do complex division (a+ib)/(c+id) = 
	    // (a+ib)(c-id)/(cc+dd) = [(ac+bd) + i((a+b)(c-d)-ac+bd)]/(cc+dd)
	    if ((mag = sqr(response[l])+sqr(scratch[l])) == 0.0) {
	      status = false;
	      goto ErrorExit;
	    }
	    mag = scale/mag;
	    FLOAT ac = data[l]*response[l],
	          bd = result[l]*scratch[l];
	    scratch[l] = mag*((data[l]+result[l])*(response[l]-scratch[l])-ac+bd);
	    result[l]  = mag*(ac+bd);
	  } 
    }

    // transform back - this is an inverse FFT
    MpFFT<FLOAT>::operator()(result,scratch,inverse);
  }

 ErrorExit:
  
  // delete scratch array if necessary
  if (auto_scratch) delete [] scratch;
  
  return status;
}

//-----------------------------------------------------------------------------//
