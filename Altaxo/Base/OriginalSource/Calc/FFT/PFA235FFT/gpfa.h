/*-----------------------------------------------------------------------------*\
| Private include file for the GPFA-FFT algorithm                        gpfa.h |
|                                                                               |
| templated version                                                             |
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

#include "matpack.h"

//-----------------------------------------------------------------------------//
// Important optimization options
//-----------------------------------------------------------------------------//

//-----------------------------------------------------------------------------//
//   lvr = length of vector registers, set to 128 for a Cray C90.
//   Reset to 64 for other Cray machines, or to any large value
//   (greater than or equal to lot) for a scalar computer.
//-----------------------------------------------------------------------------//

const int lvr = 1024;

//-----------------------------------------------------------------------------//
// The last three loops in function gpfft3d() in gpfft3d.cc are prime candidates 
// for parallelism as they call independent multi-1D ffts.  Just set the  
// constant "nthreads" to the number of processors to use. 
//-----------------------------------------------------------------------------//

const int nthreads = 1;

//-----------------------------------------------------------------------------//
// prototypes
//-----------------------------------------------------------------------------//

void gpfa2f    (FLOAT a[], FLOAT b[], FLOAT trigs[], 
		int inc, int jump, int n, int mm, int lot, int isign);
void gpfa3f    (FLOAT a[], FLOAT b[], FLOAT trigs[], 
		int inc, int jump, int n, int mm, int lot, int isign);
void gpfa5f    (FLOAT a[], FLOAT b[], FLOAT trigs[], 
		int inc, int jump, int n, int mm, int lot, int isign);
void gpfasetup (FLOAT trigs[], int n);

// the basic gpfa routine can also be used directly
void gpfa      (FLOAT a[], FLOAT b[], FLOAT trigs[], 
		int inc, int jump, int n, int lot, int isign);

// unused routines - removed
// void gpfft3d   (double c[], int id, int nn[3], int isign);
// void gpfexpand (double c[], int nl, int nmn, int isign);
// void cfft3d (double c[], int id, int nn[], int isign, int autoexpand=False);
// void cfft3d (complex<double>c[], int id, int nn[], int isign, int autoexpand=False);

//-----------------------------------------------------------------------------//

