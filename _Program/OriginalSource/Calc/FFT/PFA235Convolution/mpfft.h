/*-----------------------------------------------------------------------------*\
| complex FFT class for 1/2/3 dimensions                                mpfft.h |
|                                                                               |
| Matpack Library Release 1.7.2                                                 |
| Copyright (C) 1991-2001 by Berndt M. Gammel                                   |
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

#ifndef _MPFFT_H_
#define _MPFFT_H_

#include "matpack.h"

//-----------------------------------------------------------------------------//
// template class MpFFT:
//
// Short Description:
// ------------------
//  Generalized prime factor complex fast Fourier transform and
//  backtransform in one, two, and three dimensions (d = 1,2,3).
//  Each dimension must be of the form n = (2**p) * (3**q) * (5**r).
//  The complex d-dimensional data can be either given in a vector of
//  double precision complex numbers or in two seperate vectors of doubles
//  for the real and imaginary parts respectively, or in single precision,
//  either in a vector of complex<float> or in two seperate vectors of float.
//  A leading dimension different from the first data dimension can be 
//  specified - this can prevent memory-bank conflicts and therefore  
//  dramatically improves performance on vector machines with interleaved memory.
//  The Fourier transform is always perfored inplace. The data array can be
//  stored either in column (Fortran convention) or row (C convention) order.
//  This makes about 40 different combinations (float - double, dimension,
//  order, complex - real) which can be easily accessed 
//  by a simple class definition:
//
// Definition:
// -----------
//
//   *-------------------------------------------*
//   |  MpFFT<FLOAT> (void)			 |
//   |  MpFFT<FLOAT> (int n1)			 |
//   |  MpFFT<FLOAT> (int n1, int n2)		 |
//   |  MpFFT<FLOAT> (int n1, int n2, int n3)    |
//   *-------------------------------------------*
//
//      "FLOAT" is to be replaced by either "float" or "double".
//	Setup fast Fourier transform / back-transform for one, two or three 
//	dimensions. The dimensions n1,n2,and n3 must be of the form
//	             n = (2**p) * (3**q) * (5**r)
//	otherwise an error will be generated and the error handler function
//	Matpack.Error() is called. On instantiation some trigonometric tables
//	will be allocated and calculated. This approach avoids multiple
//	twiddle factor recalculations if several FFTs are calculated for data 
//	with the same dimensions. Sometimes it is convenient to define an
//	"empty" setup (first constructor) and assign a setup later (see
//	copying and assignment). As default the multi-dimensional data
//	are expected in row order (C convention). If you want to transform
//	data stored in column order (Fortran convention) use the member
//	function SetOrder() to change the order - see below. For optimizations
//	on vector machines with separate memory banks an extra leading dimension
//	can be defined to avoid bank conflicts - also see SetOrder().
//	
//   *-----------------------------------*
//   |  ~MpFFT (void)			 |
//   *-----------------------------------*
//
//	When the destructor is called the trigonometric tables are destroyed.
//
//   *-----------------------------------------------*
//   |  MpFFT (const MpFFT<FLOAT>& fft)		     |
//   |  MpFFT& operator = (const MpFFT<FLOAT>& fft)  |
//   *-----------------------------------------------*
//
//	Copying and assignment work as expected: The trigonometric tables
//	and all other information (as dimensions or ordering) are copied.
//
// FFT Functions:
// --------------
//  
//   *-----------------------------------------------------------------*
//   |  int operator () (FLOAT re[], FLOAT im[], int isign = forward)  |
//   |  int operator () (complex<FLOAT> c[], int isign = forward)      |
//   *-----------------------------------------------------------------*
//    Arguments:
//
//      FLOAT re[]		Input/Output vector of real and imaginary parts,
//      FLOAT im[]		either separate or combined in a complex vector.
//      complex<FLOAT> c[]	Indexing starts with 0. The Fourier transform
//				or the inverse Fourier transform is calculated
//				inplace depending on the value of isign.
//
//      int isign		Forward (-1) or reverse (1) transform. 
//				Defaults to the forward transform if the 
//				argument is omitted.
//
//    Return value:		Currently undefined, not used.
//
//
// Auxilliary Functions:
// ---------------------
//
//   *--------------------------------------*
//   |  int  Factorize (int n, int pqr[3])  |
//   *--------------------------------------*
//    Arguments:
//
//	int n			The dimension n to be factorized into the
//				valid factors n = (2**p) * (3**q) * (5**r)
//
//	int pqr[3]		Returns the powers of the basic prime factors
//				2, 3, and 5 for the given argument n.
//   
//    Return value:		Returns true (non-zero) if the decomposition 
//				is possible, otherwise false if other 
//				factors than 2, 3, or 5 are involved.
//
//   *-----------------------------------------*
//   |  void SetOrder (int row, int lead = 0)  |
//   |  void GetOrder (int &row, int &lead)    |
//   *-----------------------------------------*
//    Arguments:
//
//	int row			Set information about the row order.
// 				If the row order argument is non-zero then the 
//				d-dimensional data are assumed to be stored in 
//				row order (the C convention), otherwise if
// 				zero then column order (the Fortran convention) 
//				is assumed. Initially row order is assumed!
//
//	int lead		The leading dimension can be choosen different
//				from the first/last dimension of the array. This
//				can give a significant speed increase on some 
//				vector machines avoiding memory-bank conflicts.
// 				If the data are stored column-ordered (Fortran 
//				style) then the leading dimension is the first 
//				dimension, otherwise if the data are stored row-
// 				ordered (C style) then the last dimension is the 
//				leading dimension and will be padded!	
//
// Examples: see "matpack/tests/FFT/" and "matpack/demos/"
// ---------
//
// References:
// -----------
//  (1) For a mathematical development of the algorithm used, see: 
//      C. Temperton, "A Generalized Prime Factor FFT Algorithm for 
//      any n = (2**p)(3**q)(5**r)", SIAM J. Sci. Stat. Comp. 13, 
//      No 3, May, 676-686 (1992).
//
//  (2) The original Fortran source files for the basic algorithm can be obtained 
//      freely via ftp://ftp.earth.ox.ac.uc/pub/gpfa.tar.gz
//
//  (3) The C++ implementation of the GPF-Algorithm for the Matpack C++
//      Numerics and Graphics Library is copyrighted by B. M. Gammel, 1996-1997.
//      Read the  COPYRIGHT and README files in the Matpack distribution about
//      registration and installation.
//
//-----------------------------------------------------------------------------//

template <class FLOAT>
class MpFFT {
  public:
    // definitions
    enum { forward = -1, inverse = 1 };
    // constructors and assigment
    MpFFT (void);
    MpFFT (int n1);
    MpFFT (int n1, int n2);
    MpFFT (int n1, int n2, int n3);
   ~MpFFT (void);
    MpFFT (const MpFFT& fft);
    MpFFT& operator = (const MpFFT& fft);
    // FFT functions
    int operator () (FLOAT re[], FLOAT im[], int isign = forward);
    int operator () (complex<FLOAT> c[], int isign = forward);
    // auxilliary functions
    int  Factorize (int n, int pqr[3]);
    void SetOrder (int row, int lead = 0);
    void GetOrder (int &row, int &lead);
  protected:
    enum { def_row_order = true };  // set to true for the C convention
    int id, ndim, dim[3], trindex[3], trisize, row_order;
    FLOAT *trigs;
};

//-----------------------------------------------------------------------------//
// template class MpConvolution:
//
// Short Description:
// ------------------
//  This class is used to convolve or deconvolve a real-valued data set (including 
//  any user supplied zero padding) with a response function. All arrays must
//  have the same dimensions. The data set (and of course the other arrays) 
//  can be either one-dimensional, two-dimensional, or three-dimensional, 
//  i.e. d = 1,2,3.  Each dimension must be of the form n = (2**p)*(3**q)*(5**r), 
//  because of the underlying FFT. The d-dimensional data can be either single 
//  precision (FLOAT := float) or double precision (FLOAT := double).
//  This class is derived from class MpFFT, thus it owns all member functions
//  of class MpFFT.
//
// Definition:
// -----------
//
//   *--------------------------------------------------*
//   | MpConvolution<FLOAT> (void)                      |  
//   | MpConvolution<FLOAT> (int n1)                    |  
//   | MpConvolution<FLOAT> (int n1, int n2)            |  
//   | MpConvolution<FLOAT> (int n1, int n2, int n3)    |  
//   *--------------------------------------------------*
//
//      "FLOAT" is to be replaced by either "float" or "double".
//	Setup convolution/deconvolution for one, two or three 
//	dimensions. The dimensions n1,n2,and n3 must be of the form
//	             n = (2**p) * (3**q) * (5**r)
//	otherwise an error will be generated and the error handler function
//	Matpack.Error() is called. On instantiation the underlying class MpFFT
//      will allocate and calculate some trigonometric tables (cf. MpFFT above).
//	
// Convolution Functions:
// ----------------------
//
//   *-------------------------------------------------------------------------*
//   | int MpConvolution<FLOAT>::operator () (FLOAT data[], FLOAT response[],  |
//   |     			              FLOAT result[],                  |
//   |     			              FLOAT scratch[] = 0,             |
//   |				              int isign = forward)             |
//   *-------------------------------------------------------------------------*
//
//     Description
//     -----------
//     Convolves or deconvolves a real-valued data set data[] (including any
//     user supplied zero padding) with a response function response[].
//     The result is returned in the array result[]. All arrays including
//     the scratch[] array must have the same dimensions (or larger).
//     The data set (and of course the other arrays) can be either one-dimensional,
//     two-dimensional, or three-dimensional, d = 1,2,3.  Each dimension must be 
//     of the form n = (2**p) * (3**q) * (5**r), because of the underlying FFT.
//     The d-dimensional data can be either single precision (FLOAT := float) 
//     or double precision (FLOAT := double).
//
//     Arguments
//     ---------
//     FLOAT data[]          The real-valued data set. Note, that you have to
//                           care for end effects by zero padding. This means, 
//                           that you have to pad the data with a number of zeros
//                           on one end equal to the maximal positive duration
//                           or maximal negative duration of the response function,
//                           whichever is larger!!
//
//     FLOAT response[]      The response function must be stored in wrap-around
//                           order. This means that the first half of the array
//                           response[] (in each dimension) contains the impulse
//                           response function at positive times, while the second
//                           half of the array contains the impulse response
//                           function at negative times, counting down from the
//                           element with the highest index. The array must have 
//                           at least the size of the data array.
//
//     FLOAT result[]        The result array. It must have 
//                           at least the size of the data array.
//
//     FLOAT scratch[] 	     A work array. If a NULL pointer is passed the
//			     work array is allocated and freed auotomatically.
//                           If the array is given by the user it must have 
//                           at least the size of the data array.
//
//     int isign = forward   If isign == forward a convolution is performed. 
//                           If isign == inverse then a deconvolution is performed.
//
//     Return values
//     -------------
//     In the case of a convolution (isign == forward) the value "true" is returned
//     always. In the case of deconvolution (isign == inverse) the value "false" is
//     returned if the FFT transform of the response function is exactly zero for 
//     some value. This indicates that the original convolution has lost all 
//     information at this particular frequency, so that a reconstruction is not
//     possible. If the transform of the response function is non-zero everywhere
//     the deconvolution can be performed and the value "true" is returned.
//
//-----------------------------------------------------------------------------//

template <class FLOAT>
class MpConvolution: public MpFFT<FLOAT> {
  public:
    MpConvolution (void);
    MpConvolution (int n1);
    MpConvolution (int n1, int n2);
    MpConvolution (int n1, int n2, int n3);
    int operator () (FLOAT data[], FLOAT response[], 
		     FLOAT result[], FLOAT scratch[] = 0,
		     int isign = forward);
};

//-----------------------------------------------------------------------------//

#endif
