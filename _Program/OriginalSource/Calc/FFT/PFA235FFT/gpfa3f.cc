/*-----------------------------------------------------------------------------*\
| radix-3 section of the GPFA-FFT algorithm                           gpfa3f.cc |
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

#include "gpfa.h"

//-----------------------------------------------------------------------------//
// radix-3 section of self-sorting, in-place generalized PFA
//-----------------------------------------------------------------------------//

void gpfa3f (FLOAT a[], FLOAT b[], FLOAT trigs[], 
	     int inc, int jump, int n, int mm, int lot, int isign)
{
  const FLOAT sin60 = 0.866025403784437;

  // *************************************************************** 
  // *                                                             * 
  // *  n.b. lvr = length of vector registers, set to 128 for c90. * 
  // *  reset to 64 for other cray machines, or to any large value * 
  // *  (greater than or equal to lot) for a scalar computer.      * 
  // *                                                             * 
  // *************************************************************** 

  // System generated locals 
  int i__1, i__2, i__3, i__4, i__5, i__6, i__7, i__8, i__9, i__10;

  FLOAT  s, c1, t1, t2, t3, u1, u2, u3, co1, co2, si1, si2, aja, ajb, 
         ajc, bjb, bjc, bja, ajd, bjd, aje, ajf, ajh, bje, bjf, bjh, 
         aji, ajg, bji, bjg;

  int    ninc, left, nvex, j, k, l, m, ipass, nblox, jstep, n3, ja, jb, 
         la, jc, jd, nb, je, jf, jg, jh, mh, kk, ji, ll, mu, nu, laincl, 
         jstepl, istart, jstepx, jjj, ink, inq;

  // Parameter adjustments
  --trigs;
  --b;
  --a;

  n3 = powii(3,mm);
  inq = n / n3;
  jstepx = (n3 - n) * inc;
  ninc = n * inc;
  ink = inc * inq;
  mu = inq % 3;
  if (isign == -1) mu = 3 - mu;
  m = mm;
  mh = (m + 1) / 2;
  s = (FLOAT) isign;
  c1 = sin60;
  if (mu == 2) c1 = -c1;

  nblox = (lot - 1) / lvr + 1;
  left = lot;
  s = (FLOAT) isign;
  istart = 1;

  //  loop on blocks of lvr transforms 
  //  -------------------------------- 
  i__1 = nblox;
  for (nb = 1; nb <= i__1; ++nb) {

    if (left <= lvr) {
      nvex = left;
    } else if (left < lvr << 1) {
      nvex = left / 2;
      nvex += nvex % 2;
    } else {
      nvex = lvr;
    }
    left -= nvex;

    la = 1;

    //  loop on type I radix-3 passes 
    //  ----------------------------- 
    i__2 = mh;
    for (ipass = 1; ipass <= i__2; ++ipass) {
      jstep = n * inc / (la * 3);
      jstepl = jstep - ninc;

      //  k = 0 loop (no twiddle factors) 
      //  ------------------------------- 
      i__3 = (n - 1) * inc;
      i__4 = jstep * 3;
      for (jjj = 0; i__4 < 0 ? jjj >= i__3 : jjj <= i__3; jjj += i__4) {
	ja = istart + jjj;

	//  "transverse" loop 
	//  ----------------- 
	i__5 = inq;
	for (nu = 1; nu <= i__5; ++nu) {
	  jb = ja + jstepl;
	  if (jb < istart) {
	    jb += ninc;
	  }
	  jc = jb + jstepl;
	  if (jc < istart) {
	    jc += ninc;
	  }
	  j = 0;

	  //  loop across transforms 
	  //  ---------------------- 

	  i__6 = nvex;
	  for (l = 1; l <= i__6; ++l) {
	    ajb = a[jb + j];
	    ajc = a[jc + j];
	    t1 = ajb + ajc;
	    aja = a[ja + j];
	    t2 = aja - t1 * .5;
	    t3 = c1 * (ajb - ajc);
	    bjb = b[jb + j];
	    bjc = b[jc + j];
	    u1 = bjb + bjc;
	    bja = b[ja + j];
	    u2 = bja - u1 * .5;
	    u3 = c1 * (bjb - bjc);
	    a[ja + j] = aja + t1;
	    b[ja + j] = bja + u1;
	    a[jb + j] = t2 - u3;
	    b[jb + j] = u2 + t3;
	    a[jc + j] = t2 + u3;
	    b[jc + j] = u2 - t3;
	    j += jump;
	  }
	  ja += jstepx;
	  if (ja < istart) {
	    ja += ninc;
	  }
	}
      }

      //  finished if n3 = 3 
      //  ------------------ 
      if (n3 == 3) goto L490;
      kk = la << 1;

      //  loop on nonzero k 
      //  ----------------- 
      i__4 = jstep - ink;
      i__3 = ink;
      for (k = ink; i__3 < 0 ? k >= i__4 : k <= i__4; k += i__3) {
	co1 = trigs[kk + 1];
	si1 = s * trigs[kk + 2];
	co2 = trigs[(kk << 1) + 1];
	si2 = s * trigs[(kk << 1) + 2];

	//  loop along transform 
	//  -------------------- 
	i__5 = (n - 1) * inc;
	i__6 = jstep * 3;
	for (jjj = k; i__6 < 0 ? jjj >= i__5 : jjj <= i__5; jjj += i__6) {
	  ja = istart + jjj;

	  //  "transverse" loop 
	  //  ----------------- 
	  i__7 = inq;
	  for (nu = 1; nu <= i__7; ++nu) {
	    jb = ja + jstepl;
	    if (jb < istart) {
	      jb += ninc;
	    }
	    jc = jb + jstepl;
	    if (jc < istart) {
	      jc += ninc;
	    }
	    j = 0;

	    //  loop across transforms 
	    //  ---------------------- 

	    i__8 = nvex;
	    for (l = 1; l <= i__8; ++l) {
	      ajb = a[jb + j];
	      ajc = a[jc + j];
	      t1 = ajb + ajc;
	      aja = a[ja + j];
	      t2 = aja - t1 * .5;
	      t3 = c1 * (ajb - ajc);
	      bjb = b[jb + j];
	      bjc = b[jc + j];
	      u1 = bjb + bjc;
	      bja = b[ja + j];
	      u2 = bja - u1 * .5;
	      u3 = c1 * (bjb - bjc);
	      a[ja + j] = aja + t1;
	      b[ja + j] = bja + u1;
	      a[jb + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
	      b[jb + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
	      a[jc + j] = co2 * (t2 + u3) - si2 * (u2 - t3);
	      b[jc + j] = si2 * (t2 + u3) + co2 * (u2 - t3);
	      j += jump;
	    }
	    // -----( end of loop across transforms ) 
	    ja += jstepx;
	    if (ja < istart) {
	      ja += ninc;
	    }
	  }
	}
	// -----( end of loop along transforms ) 
	kk += la << 1;
      }
      // -----( end of loop on nonzero k ) 
      la *= 3;
    }
    // -----( end of loop on type I radix-3 passes) 

    //  loop on type II radix-3 passes 
    //  ------------------------------ 

    i__2 = m;
    for (ipass = mh + 1; ipass <= i__2; ++ipass) {
      jstep = n * inc / (la * 3);
      jstepl = jstep - ninc;
      laincl = la * ink - ninc;

      //  k=0 loop (no twiddle factors) 
      //  ----------------------------- 
      i__3 = (la - 1) * ink;
      i__4 = jstep * 3;
      for (ll = 0; i__4 < 0 ? ll >= i__3 : ll <= i__3; ll += i__4) {

	i__6 = (n - 1) * inc;
	i__5 = la * 3 * ink;
	for (jjj = ll; i__5 < 0 ? jjj >= i__6 : jjj <= i__6; jjj += i__5) {
	  ja = istart + jjj;

	  //  "transverse" loop 
	  //  ----------------- 
	  i__7 = inq;
	  for (nu = 1; nu <= i__7; ++nu) {
	    jb = ja + jstepl;
	    if (jb < istart) {
	      jb += ninc;
	    }
	    jc = jb + jstepl;
	    if (jc < istart) {
	      jc += ninc;
	    }
	    jd = ja + laincl;
	    if (jd < istart) {
	      jd += ninc;
	    }
	    je = jd + jstepl;
	    if (je < istart) {
	      je += ninc;
	    }
	    jf = je + jstepl;
	    if (jf < istart) {
	      jf += ninc;
	    }
	    jg = jd + laincl;
	    if (jg < istart) {
	      jg += ninc;
	    }
	    jh = jg + jstepl;
	    if (jh < istart) {
	      jh += ninc;
	    }
	    ji = jh + jstepl;
	    if (ji < istart) {
	      ji += ninc;
	    }
	    j = 0;

	    //  loop across transforms 
	    //  ---------------------- 

	    i__8 = nvex;
	    for (l = 1; l <= i__8; ++l) {
	      ajb = a[jb + j];
	      ajc = a[jc + j];
	      t1 = ajb + ajc;
	      aja = a[ja + j];
	      t2 = aja - t1 * .5;
	      t3 = c1 * (ajb - ajc);
	      ajd = a[jd + j];
	      ajb = ajd;
	      bjb = b[jb + j];
	      bjc = b[jc + j];
	      u1 = bjb + bjc;
	      bja = b[ja + j];
	      u2 = bja - u1 * .5;
	      u3 = c1 * (bjb - bjc);
	      bjd = b[jd + j];
	      bjb = bjd;
	      a[ja + j] = aja + t1;
	      b[ja + j] = bja + u1;
	      a[jd + j] = t2 - u3;
	      b[jd + j] = u2 + t3;
	      ajc = t2 + u3;
	      bjc = u2 - t3;
	      // ---------------------- 
	      aje = a[je + j];
	      ajf = a[jf + j];
	      t1 = aje + ajf;
	      t2 = ajb - t1 * .5;
	      t3 = c1 * (aje - ajf);
	      ajh = a[jh + j];
	      ajf = ajh;
	      bje = b[je + j];
	      bjf = b[jf + j];
	      u1 = bje + bjf;
	      u2 = bjb - u1 * .5;
	      u3 = c1 * (bje - bjf);
	      bjh = b[jh + j];
	      bjf = bjh;
	      a[jb + j] = ajb + t1;
	      b[jb + j] = bjb + u1;
	      a[je + j] = t2 - u3;
	      b[je + j] = u2 + t3;
	      a[jh + j] = t2 + u3;
	      b[jh + j] = u2 - t3;
	      // ---------------------- 
	      aji = a[ji + j];
	      t1 = ajf + aji;
	      ajg = a[jg + j];
	      t2 = ajg - t1 * .5;
	      t3 = c1 * (ajf - aji);
	      t1 = ajg + t1;
	      a[jg + j] = ajc;
	      bji = b[ji + j];
	      u1 = bjf + bji;
	      bjg = b[jg + j];
	      u2 = bjg - u1 * .5;
	      u3 = c1 * (bjf - bji);
	      u1 = bjg + u1;
	      b[jg + j] = bjc;
	      a[jc + j] = t1;
	      b[jc + j] = u1;
	      a[jf + j] = t2 - u3;
	      b[jf + j] = u2 + t3;
	      a[ji + j] = t2 + u3;
	      b[ji + j] = u2 - t3;
	      j += jump;
	    }
	    // -----( end of loop across transforms ) 
	    ja += jstepx;
	    if (ja < istart) {
	      ja += ninc;
	    }
	  }
	}
      }
      // -----( end of double loop for k=0 ) 

      //  finished if last pass 
      //  --------------------- 
      if (ipass == m) goto L490;
      kk = la << 1;

      //     loop on nonzero k 
      //     ----------------- 
      i__4 = jstep - ink;
      i__3 = ink;
      for (k = ink; i__3 < 0 ? k >= i__4 : k <= i__4; k += i__3) {
	co1 = trigs[kk + 1];
	si1 = s * trigs[kk + 2];
	co2 = trigs[(kk << 1) + 1];
	si2 = s * trigs[(kk << 1) + 2];

	//  double loop along first transform in block 
	//  ------------------------------------------ 
	i__5 = (la - 1) * ink;
	i__6 = jstep * 3;
	for (ll = k; i__6 < 0 ? ll >= i__5 : ll <= i__5; ll += i__6) {

	  i__7 = (n - 1) * inc;
	  i__8 = la * 3 * ink;
	  for (jjj = ll; i__8 < 0 ? jjj >= i__7 : jjj <= i__7; jjj += i__8) {
	    ja = istart + jjj;

	    //  "transverse" loop 
	    //  ----------------- 
	    i__9 = inq;
	    for (nu = 1; nu <= i__9; ++nu) {
	      jb = ja + jstepl;
	      if (jb < istart) {
		jb += ninc;
	      }
	      jc = jb + jstepl;
	      if (jc < istart) {
		jc += ninc;
	      }
	      jd = ja + laincl;
	      if (jd < istart) {
		jd += ninc;
	      }
	      je = jd + jstepl;
	      if (je < istart) {
		je += ninc;
	      }
	      jf = je + jstepl;
	      if (jf < istart) {
		jf += ninc;
	      }
	      jg = jd + laincl;
	      if (jg < istart) {
		jg += ninc;
	      }
	      jh = jg + jstepl;
	      if (jh < istart) {
		jh += ninc;
	      }
	      ji = jh + jstepl;
	      if (ji < istart) {
		ji += ninc;
	      }
	      j = 0;

	      //  loop across transforms 
	      //  ---------------------- 

	      i__10 = nvex;
	      for (l = 1; l <= i__10; ++l) {
		ajb = a[jb + j];
		ajc = a[jc + j];
		t1 = ajb + ajc;
		aja = a[ja + j];
		t2 = aja - t1 * .5;
		t3 = c1 * (ajb - ajc);
		ajd = a[jd + j];
		ajb = ajd;
		bjb = b[jb + j];
		bjc = b[jc + j];
		u1 = bjb + bjc;
		bja = b[ja + j];
		u2 = bja - u1 * .5;
		u3 = c1 * (bjb - bjc);
		bjd = b[jd + j];
		bjb = bjd;
		a[ja + j] = aja + t1;
		b[ja + j] = bja + u1;
		a[jd + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
		b[jd + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
		ajc = co2 * (t2 + u3) - si2 * (u2 - t3);
		bjc = si2 * (t2 + u3) + co2 * (u2 - t3);
		// ---------------------- 
		aje = a[je + j];
		ajf = a[jf + j];
		t1 = aje + ajf;
		t2 = ajb - t1 * .5;
		t3 = c1 * (aje - ajf);
		ajh = a[jh + j];
		ajf = ajh;
		bje = b[je + j];
		bjf = b[jf + j];
		u1 = bje + bjf;
		u2 = bjb - u1 * .5;
		u3 = c1 * (bje - bjf);
		bjh = b[jh + j];
		bjf = bjh;
		a[jb + j] = ajb + t1;
		b[jb + j] = bjb + u1;
		a[je + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
		b[je + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
		a[jh + j] = co2 * (t2 + u3) - si2 * (u2 - t3);
		b[jh + j] = si2 * (t2 + u3) + co2 * (u2 - t3);
		// ---------------------- 
		aji = a[ji + j];
		t1 = ajf + aji;
		ajg = a[jg + j];
		t2 = ajg - t1 * .5;
		t3 = c1 * (ajf - aji);
		t1 = ajg + t1;
		a[jg + j] = ajc;
		bji = b[ji + j];
		u1 = bjf + bji;
		bjg = b[jg + j];
		u2 = bjg - u1 * .5;
		u3 = c1 * (bjf - bji);
		u1 = bjg + u1;
		b[jg + j] = bjc;
		a[jc + j] = t1;
		b[jc + j] = u1;
		a[jf + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
		b[jf + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
		a[ji + j] = co2 * (t2 + u3) - si2 * (u2 - t3);
		b[ji + j] = si2 * (t2 + u3) + co2 * (u2 - t3);
		j += jump;
	      }
	      // -----(end of loop across transforms) 
	      ja += jstepx;
	      if (ja < istart) {
		ja += ninc;
	      }
	    }
	  }
	}
	// -----( end of double loop for this k ) 
	kk += la << 1;
      }
      // -----( end of loop over values of k ) 
      la *= 3;
    }
    // -----( end of loop on type II radix-3 passes ) 
    // -----( nvex transforms completed) 
  L490:
    istart += nvex * jump;
  }
  // -----( end of loop on blocks of transforms )
}

//-----------------------------------------------------------------------------//
