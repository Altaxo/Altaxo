/*-----------------------------------------------------------------------------*\
| radix-2 section of the GPFA-FFT algorithm                           gpfa2f.cc |
|                                                                               |
| templated version                                                             |
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

#include "gpfa.h"

//-----------------------------------------------------------------------------//
//     radix-2 section of self-sorting, in-place, generalized pfa 
//     central radix-2 and radix-8 passes included 
//      so that transform length can be any power of 2 
//-----------------------------------------------------------------------------//

void gpfa2f (FLOAT a[], FLOAT b[], FLOAT trigs[], 
	     int inc, int jump, int n, int mm, int lot, int isign)
{

  // *************************************************************** 
  // *                                                             * 
  // *  n.b. lvr = length of vector registers, set to 128 for c90. * 
  // *  reset to 64 for other cray machines, or to any large value * 
  // *  (greater than or equal to lot) for a scalar computer.      * 
  // *                                                             * 
  // *************************************************************** 

  // System generated locals 
  int i__2, i__3, i__4, i__5, i__6, i__7, i__8, i__9, i__10;

  int    ninc, left, nvex, j, k, l, m = 0, ipass, nblox, jstep, m2, n2, m8,
         ja, jb, la, jc, jd, nb, je, jf, jg, jh, mh, kk, ji, ll, jj, jk, 
         jl, jm, jn, jo, jp, mu, nu, laincl, jstepl, istart, jstepx, jjj,
         ink, inq;
  FLOAT  c1, s, c2, c3, t0, t2, t1, t3, u0, u2, u1, u3, ss, co1, co2, co3,
         co4, co5, co6, co7, si1, si2, si3, si4, si5, si6, si7, aja, ajb, 
         ajc, ajd, bja, bjc, bjb, bjd, aje, ajg, ajf, ajh, bje, bjg, bjf, 
         bjh, aji, bjm, ajj, bjj, ajk, ajl, bji, bjk, ajo, bjl, bjo, ajm, 
         ajn, ajp, bjn, bjp;

  // Parameter adjustments 
  --trigs;
  --b;
  --a;

  n2 = powii(2,mm);
  inq = n / n2;
  jstepx = (n2 - n) * inc;
  ninc = n * inc;
  ink = inc * inq;

  m2 = 0;
  m8 = 0;
  if (mm % 2 == 0) {
    m = mm / 2;
  } else if (mm % 4 == 1) {
    m = (mm - 1) / 2;
    m2 = 1;
  } else if (mm % 4 == 3) {
    m = (mm - 3) / 2;
    m8 = 1;
  }
  mh = (m + 1) / 2;

  nblox = (lot - 1) / lvr + 1;
  left = lot;
  s = (FLOAT) isign;
  istart = 1;

  //  loop on blocks of lvr transforms 
  //  -------------------------------- 

  for (nb = 1; nb <= nblox; ++nb) {

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

    //  loop on type I radix-4 passes 
    //  ----------------------------- 
    mu = inq % 4;
    if (isign == -1) mu = 4 - mu;
    ss = 1.0;
    if (mu == 3) ss = -1.0;
    if (mh == 0) goto L200;

    i__2 = mh;
    for (ipass = 1; ipass <= i__2; ++ipass) {
      jstep = n * inc / (la << 2);
      jstepl = jstep - ninc;

      //  k = 0 loop (no twiddle factors) 
      //  ------------------------------- 
      i__3 = (n - 1) * inc;
      i__4 = jstep << 2;
      for (jjj = 0; i__4 < 0 ? jjj >= i__3 : jjj <= i__3; jjj += i__4) {
	ja = istart + jjj;

	//     "transverse" loop 
	//     ----------------- 
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
	  jd = jc + jstepl;
	  if (jd < istart) {
	    jd += ninc;
	  }
	  j = 0;

	  //  loop across transforms 
	  //  ---------------------- 

	  i__6 = nvex;
	  for (l = 1; l <= i__6; ++l) {
	    aja = a[ja + j];
	    ajc = a[jc + j];
	    t0 = aja + ajc;
	    t2 = aja - ajc;
	    ajb = a[jb + j];
	    ajd = a[jd + j];
	    t1 = ajb + ajd;
	    t3 = ss * (ajb - ajd);
	    bja = b[ja + j];
	    bjc = b[jc + j];
	    u0 = bja + bjc;
	    u2 = bja - bjc;
	    bjb = b[jb + j];
	    bjd = b[jd + j];
	    u1 = bjb + bjd;
	    u3 = ss * (bjb - bjd);
	    a[ja + j] = t0 + t1;
	    a[jc + j] = t0 - t1;
	    b[ja + j] = u0 + u1;
	    b[jc + j] = u0 - u1;
	    a[jb + j] = t2 - u3;
	    a[jd + j] = t2 + u3;
	    b[jb + j] = u2 + t3;
	    b[jd + j] = u2 - t3;
	    j += jump;
	  }
	  ja += jstepx;
	  if (ja < istart) {
	    ja += ninc;
	  }
	}
      }

      //  finished if n2 = 4 
      //  ------------------ 
      if (n2 == 4) goto L490;
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
	co3 = trigs[kk * 3 + 1];
	si3 = s * trigs[kk * 3 + 2];

	//  loop along transform 
	//  -------------------- 
	i__5 = (n - 1) * inc;
	i__6 = jstep << 2;
	for (jjj = k; i__6 < 0 ? jjj >= i__5 : jjj <= i__5; jjj += i__6) {
	  ja = istart + jjj;

	  //     "transverse" loop 
	  //     ----------------- 
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
	    jd = jc + jstepl;
	    if (jd < istart) {
	      jd += ninc;
	    }
	    j = 0;

	    //  loop across transforms 
	    //  ---------------------- 

	    i__8 = nvex;
	    for (l = 1; l <= i__8; ++l) {
	      aja = a[ja + j];
	      ajc = a[jc + j];
	      t0 = aja + ajc;
	      t2 = aja - ajc;
	      ajb = a[jb + j];
	      ajd = a[jd + j];
	      t1 = ajb + ajd;
	      t3 = ss * (ajb - ajd);
	      bja = b[ja + j];
	      bjc = b[jc + j];
	      u0 = bja + bjc;
	      u2 = bja - bjc;
	      bjb = b[jb + j];
	      bjd = b[jd + j];
	      u1 = bjb + bjd;
	      u3 = ss * (bjb - bjd);
	      a[ja + j] = t0 + t1;
	      b[ja + j] = u0 + u1;
	      a[jb + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
	      b[jb + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
	      a[jc + j] = co2 * (t0 - t1) - si2 * (u0 - u1);
	      b[jc + j] = si2 * (t0 - t1) + co2 * (u0 - u1);
	      a[jd + j] = co3 * (t2 + u3) - si3 * (u2 - t3);
	      b[jd + j] = si3 * (t2 + u3) + co3 * (u2 - t3);
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
      la <<= 2;
    }
    // -----( end of loop on type I radix-4 passes) 

    //  central radix-2 pass 
    //  -------------------- 
  L200:
    if (m2 == 0) goto L300;

    jstep = n * inc / (la << 1);
    jstepl = jstep - ninc;

    //  k=0 loop (no twiddle factors) 
    //  ----------------------------- 
    i__2 = (n - 1) * inc;
    i__3 = jstep << 1;
    for (jjj = 0; i__3 < 0 ? jjj >= i__2 : jjj <= i__2; jjj += i__3) {
      ja = istart + jjj;

      //     "transverse" loop 
      //     ----------------- 
      i__4 = inq;
      for (nu = 1; nu <= i__4; ++nu) {
	jb = ja + jstepl;
	if (jb < istart) {
	  jb += ninc;
	}
	j = 0;

	//  loop across transforms 
	//  ---------------------- 

	i__6 = nvex;
	for (l = 1; l <= i__6; ++l) {
	  aja = a[ja + j];
	  ajb = a[jb + j];
	  t0 = aja - ajb;
	  a[ja + j] = aja + ajb;
	  a[jb + j] = t0;
	  bja = b[ja + j];
	  bjb = b[jb + j];
	  u0 = bja - bjb;
	  b[ja + j] = bja + bjb;
	  b[jb + j] = u0;
	  j += jump;
	}
	// -----(end of loop across transforms) 
	ja += jstepx;
	if (ja < istart) {
	  ja += ninc;
	}
      }
    }

    //  finished if n2=2 
    //  ---------------- 
    if (n2 == 2) {
      goto L490;
    }

    kk = la << 1;

    //  loop on nonzero k 
    //  ----------------- 
    i__3 = jstep - ink;
    i__2 = ink;
    for (k = ink; i__2 < 0 ? k >= i__3 : k <= i__3; k += i__2) {
      co1 = trigs[kk + 1];
      si1 = s * trigs[kk + 2];

      //  loop along transforms 
      //  --------------------- 
      i__4 = (n - 1) * inc;
      i__6 = jstep << 1;
      for (jjj = k; i__6 < 0 ? jjj >= i__4 : jjj <= i__4; jjj += i__6) {
	ja = istart + jjj;

	//     "transverse" loop 
	//     ----------------- 
	i__5 = inq;
	for (nu = 1; nu <= i__5; ++nu) {
	  jb = ja + jstepl;
	  if (jb < istart) {
	    jb += ninc;
	  }
	  j = 0;

	  //  loop across transforms 
	  //  ---------------------- 
	  if (kk == n2 / 2) {

	    i__7 = nvex;
	    for (l = 1; l <= i__7; ++l) {
	      aja = a[ja + j];
	      ajb = a[jb + j];
	      t0 = ss * (aja - ajb);
	      a[ja + j] = aja + ajb;
	      bjb = b[jb + j];
	      bja = b[ja + j];
	      a[jb + j] = ss * (bjb - bja);
	      b[ja + j] = bja + bjb;
	      b[jb + j] = t0;
	      j += jump;
	    }

	  } else {

	    i__7 = nvex;
	    for (l = 1; l <= i__7; ++l) {
	      aja = a[ja + j];
	      ajb = a[jb + j];
	      t0 = aja - ajb;
	      a[ja + j] = aja + ajb;
	      bja = b[ja + j];
	      bjb = b[jb + j];
	      u0 = bja - bjb;
	      b[ja + j] = bja + bjb;
	      a[jb + j] = co1 * t0 - si1 * u0;
	      b[jb + j] = si1 * t0 + co1 * u0;
	      j += jump;
	    }

	  }

	  // -----(end of loop across transforms) 
	  ja += jstepx;
	  if (ja < istart) {
	    ja += ninc;
	  }
	}
      }
      // -----(end of loop along transforms) 
      kk += la << 1;
    }
    // -----(end of loop on nonzero k) 
    // -----(end of radix-2 pass) 

    la <<= 1;
    goto L400;

    //  central radix-8 pass 
    //  -------------------- 
  L300:
    if (m8 == 0) goto L400;
    jstep = n * inc / (la << 3);
    jstepl = jstep - ninc;
    mu = inq % 8;
    if (isign == -1) mu = 8 - mu;
    c1 = 1.0;
    if (mu == 3 || mu == 7) c1 = -1.0;
    c2 = sqrt(0.5);
    if (mu == 3 || mu == 5) c2 = -c2;
    c3 = c1 * c2;

    //  stage 1 
    //  ------- 
    i__2 = jstep - ink;
    i__3 = ink;
    for (k = 0; i__3 < 0 ? k >= i__2 : k <= i__2; k += i__3) {
      i__6 = (n - 1) * inc;
      i__4 = jstep << 3;
      for (jjj = k; i__4 < 0 ? jjj >= i__6 : jjj <= i__6; jjj += i__4) {
	ja = istart + jjj;

	//     "transverse" loop 
	//     ----------------- 
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
	  jd = jc + jstepl;
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
	  jg = jf + jstepl;
	  if (jg < istart) {
	    jg += ninc;
	  }
	  jh = jg + jstepl;
	  if (jh < istart) {
	    jh += ninc;
	  }
	  j = 0;

	  i__7 = nvex;
	  for (l = 1; l <= i__7; ++l) {
	    aja = a[ja + j];
	    aje = a[je + j];
	    t0 = aja - aje;
	    a[ja + j] = aja + aje;
	    ajc = a[jc + j];
	    ajg = a[jg + j];
	    t1 = c1 * (ajc - ajg);
	    a[je + j] = ajc + ajg;
	    ajb = a[jb + j];
	    ajf = a[jf + j];
	    t2 = ajb - ajf;
	    a[jc + j] = ajb + ajf;
	    ajd = a[jd + j];
	    ajh = a[jh + j];
	    t3 = ajd - ajh;
	    a[jg + j] = ajd + ajh;
	    a[jb + j] = t0;
	    a[jf + j] = t1;
	    a[jd + j] = c2 * (t2 - t3);
	    a[jh + j] = c3 * (t2 + t3);
	    bja = b[ja + j];
	    bje = b[je + j];
	    u0 = bja - bje;
	    b[ja + j] = bja + bje;
	    bjc = b[jc + j];
	    bjg = b[jg + j];
	    u1 = c1 * (bjc - bjg);
	    b[je + j] = bjc + bjg;
	    bjb = b[jb + j];
	    bjf = b[jf + j];
	    u2 = bjb - bjf;
	    b[jc + j] = bjb + bjf;
	    bjd = b[jd + j];
	    bjh = b[jh + j];
	    u3 = bjd - bjh;
	    b[jg + j] = bjd + bjh;
	    b[jb + j] = u0;
	    b[jf + j] = u1;
	    b[jd + j] = c2 * (u2 - u3);
	    b[jh + j] = c3 * (u2 + u3);
	    j += jump;
	  }
	  ja += jstepx;
	  if (ja < istart) {
	    ja += ninc;
	  }
	}
      }
    }

    //  stage 2 
    //  ------- 

    //  k=0 (no twiddle factors) 
    //  ------------------------ 
    i__3 = (n - 1) * inc;
    i__2 = jstep << 3;
    for (jjj = 0; i__2 < 0 ? jjj >= i__3 : jjj <= i__3; jjj += i__2) {
      ja = istart + jjj;

      //     "transverse" loop 
      //     ----------------- 
      i__4 = inq;
      for (nu = 1; nu <= i__4; ++nu) {
	jb = ja + jstepl;
	if (jb < istart) {
	  jb += ninc;
	}
	jc = jb + jstepl;
	if (jc < istart) {
	  jc += ninc;
	}
	jd = jc + jstepl;
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
	jg = jf + jstepl;
	if (jg < istart) {
	  jg += ninc;
	}
	jh = jg + jstepl;
	if (jh < istart) {
	  jh += ninc;
	}
	j = 0;

	i__6 = nvex;
	for (l = 1; l <= i__6; ++l) {
	  aja = a[ja + j];
	  aje = a[je + j];
	  t0 = aja + aje;
	  t2 = aja - aje;
	  ajc = a[jc + j];
	  ajg = a[jg + j];
	  t1 = ajc + ajg;
	  t3 = c1 * (ajc - ajg);
	  bja = b[ja + j];
	  bje = b[je + j];
	  u0 = bja + bje;
	  u2 = bja - bje;
	  bjc = b[jc + j];
	  bjg = b[jg + j];
	  u1 = bjc + bjg;
	  u3 = c1 * (bjc - bjg);
	  a[ja + j] = t0 + t1;
	  a[je + j] = t0 - t1;
	  b[ja + j] = u0 + u1;
	  b[je + j] = u0 - u1;
	  a[jc + j] = t2 - u3;
	  a[jg + j] = t2 + u3;
	  b[jc + j] = u2 + t3;
	  b[jg + j] = u2 - t3;
	  ajb = a[jb + j];
	  ajd = a[jd + j];
	  t0 = ajb + ajd;
	  t2 = ajb - ajd;
	  ajf = a[jf + j];
	  ajh = a[jh + j];
	  t1 = ajf - ajh;
	  t3 = ajf + ajh;
	  bjb = b[jb + j];
	  bjd = b[jd + j];
	  u0 = bjb + bjd;
	  u2 = bjb - bjd;
	  bjf = b[jf + j];
	  bjh = b[jh + j];
	  u1 = bjf - bjh;
	  u3 = bjf + bjh;
	  a[jb + j] = t0 - u3;
	  a[jh + j] = t0 + u3;
	  b[jb + j] = u0 + t3;
	  b[jh + j] = u0 - t3;
	  a[jd + j] = t2 + u1;
	  a[jf + j] = t2 - u1;
	  b[jd + j] = u2 - t1;
	  b[jf + j] = u2 + t1;
	  j += jump;
	}
	ja += jstepx;
	if (ja < istart) {
	  ja += ninc;
	}
      }
    }

    if (n2 == 8) goto L490;

    //  loop on nonzero k 
    //  ----------------- 
    kk = la << 1;

    i__2 = jstep - ink;
    i__3 = ink;
    for (k = ink; i__3 < 0 ? k >= i__2 : k <= i__2; k += i__3) {

      co1 = trigs[kk + 1];
      si1 = s * trigs[kk + 2];
      co2 = trigs[(kk << 1) + 1];
      si2 = s * trigs[(kk << 1) + 2];
      co3 = trigs[kk * 3 + 1];
      si3 = s * trigs[kk * 3 + 2];
      co4 = trigs[(kk << 2) + 1];
      si4 = s * trigs[(kk << 2) + 2];
      co5 = trigs[kk * 5 + 1];
      si5 = s * trigs[kk * 5 + 2];
      co6 = trigs[kk * 6 + 1];
      si6 = s * trigs[kk * 6 + 2];
      co7 = trigs[kk * 7 + 1];
      si7 = s * trigs[kk * 7 + 2];

      i__4 = (n - 1) * inc;
      i__6 = jstep << 3;
      for (jjj = k; i__6 < 0 ? jjj >= i__4 : jjj <= i__4; jjj += i__6) {
	ja = istart + jjj;

	//     "transverse" loop 
	//     ----------------- 
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
	  jd = jc + jstepl;
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
	  jg = jf + jstepl;
	  if (jg < istart) {
	    jg += ninc;
	  }
	  jh = jg + jstepl;
	  if (jh < istart) {
	    jh += ninc;
	  }
	  j = 0;

	  i__7 = nvex;
	  for (l = 1; l <= i__7; ++l) {
	    aja = a[ja + j];
	    aje = a[je + j];
	    t0 = aja + aje;
	    t2 = aja - aje;
	    ajc = a[jc + j];
	    ajg = a[jg + j];
	    t1 = ajc + ajg;
	    t3 = c1 * (ajc - ajg);
	    bja = b[ja + j];
	    bje = b[je + j];
	    u0 = bja + bje;
	    u2 = bja - bje;
	    bjc = b[jc + j];
	    bjg = b[jg + j];
	    u1 = bjc + bjg;
	    u3 = c1 * (bjc - bjg);
	    a[ja + j] = t0 + t1;
	    b[ja + j] = u0 + u1;
	    a[je + j] = co4 * (t0 - t1) - si4 * (u0 - u1);
	    b[je + j] = si4 * (t0 - t1) + co4 * (u0 - u1);
	    a[jc + j] = co2 * (t2 - u3) - si2 * (u2 + t3);
	    b[jc + j] = si2 * (t2 - u3) + co2 * (u2 + t3);
	    a[jg + j] = co6 * (t2 + u3) - si6 * (u2 - t3);
	    b[jg + j] = si6 * (t2 + u3) + co6 * (u2 - t3);
	    ajb = a[jb + j];
	    ajd = a[jd + j];
	    t0 = ajb + ajd;
	    t2 = ajb - ajd;
	    ajf = a[jf + j];
	    ajh = a[jh + j];
	    t1 = ajf - ajh;
	    t3 = ajf + ajh;
	    bjb = b[jb + j];
	    bjd = b[jd + j];
	    u0 = bjb + bjd;
	    u2 = bjb - bjd;
	    bjf = b[jf + j];
	    bjh = b[jh + j];
	    u1 = bjf - bjh;
	    u3 = bjf + bjh;
	    a[jb + j] = co1 * (t0 - u3) - si1 * (u0 + t3);
	    b[jb + j] = si1 * (t0 - u3) + co1 * (u0 + t3);
	    a[jh + j] = co7 * (t0 + u3) - si7 * (u0 - t3);
	    b[jh + j] = si7 * (t0 + u3) + co7 * (u0 - t3);
	    a[jd + j] = co3 * (t2 + u1) - si3 * (u2 - t1);
	    b[jd + j] = si3 * (t2 + u1) + co3 * (u2 - t1);
	    a[jf + j] = co5 * (t2 - u1) - si5 * (u2 + t1);
	    b[jf + j] = si5 * (t2 - u1) + co5 * (u2 + t1);
	    j += jump;
	  }
	  ja += jstepx;
	  if (ja < istart) {
	    ja += ninc;
	  }
	}
      }
      kk += la << 1;
    }

    la <<= 3;

    //  loop on type II radix-4 passes 
    //  ------------------------------ 
  L400:
    mu = inq % 4;
    if (isign == -1) mu = 4 - mu;
    ss = 1.0;
    if (mu == 3) ss = -1.0;

    i__3 = m;
    for (ipass = mh + 1; ipass <= i__3; ++ipass) {
      jstep = n * inc / (la << 2);
      jstepl = jstep - ninc;
      laincl = la * ink - ninc;

      //  k=0 loop (no twiddle factors) 
      //  ----------------------------- 
      i__2 = (la - 1) * ink;
      i__6 = jstep << 2;
      for (ll = 0; i__6 < 0 ? ll >= i__2 : ll <= i__2; ll += i__6) {

	i__4 = (n - 1) * inc;
	i__5 = (la << 2) * ink;
	for (jjj = ll; i__5 < 0 ? jjj >= i__4 : jjj <= i__4; jjj += i__5) {
	  ja = istart + jjj;

	  //     "transverse" loop 
	  //     ----------------- 
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
	    jd = jc + jstepl;
	    if (jd < istart) {
	      jd += ninc;
	    }
	    je = ja + laincl;
	    if (je < istart) {
	      je += ninc;
	    }
	    jf = je + jstepl;
	    if (jf < istart) {
	      jf += ninc;
	    }
	    jg = jf + jstepl;
	    if (jg < istart) {
	      jg += ninc;
	    }
	    jh = jg + jstepl;
	    if (jh < istart) {
	      jh += ninc;
	    }
	    ji = je + laincl;
	    if (ji < istart) {
	      ji += ninc;
	    }
	    jj = ji + jstepl;
	    if (jj < istart) {
	      jj += ninc;
	    }
	    jk = jj + jstepl;
	    if (jk < istart) {
	      jk += ninc;
	    }
	    jl = jk + jstepl;
	    if (jl < istart) {
	      jl += ninc;
	    }
	    jm = ji + laincl;
	    if (jm < istart) {
	      jm += ninc;
	    }
	    jn = jm + jstepl;
	    if (jn < istart) {
	      jn += ninc;
	    }
	    jo = jn + jstepl;
	    if (jo < istart) {
	      jo += ninc;
	    }
	    jp = jo + jstepl;
	    if (jp < istart) {
	      jp += ninc;
	    }
	    j = 0;

	    //  loop across transforms 
	    //  ---------------------- 

	    i__8 = nvex;
	    for (l = 1; l <= i__8; ++l) {
	      aja = a[ja + j];
	      ajc = a[jc + j];
	      t0 = aja + ajc;
	      t2 = aja - ajc;
	      ajb = a[jb + j];
	      ajd = a[jd + j];
	      t1 = ajb + ajd;
	      t3 = ss * (ajb - ajd);
	      aji = a[ji + j];
	      ajc = aji;
	      bja = b[ja + j];
	      bjc = b[jc + j];
	      u0 = bja + bjc;
	      u2 = bja - bjc;
	      bjb = b[jb + j];
	      bjd = b[jd + j];
	      u1 = bjb + bjd;
	      u3 = ss * (bjb - bjd);
	      aje = a[je + j];
	      ajb = aje;
	      a[ja + j] = t0 + t1;
	      a[ji + j] = t0 - t1;
	      b[ja + j] = u0 + u1;
	      bjc = u0 - u1;
	      bjm = b[jm + j];
	      bjd = bjm;
	      a[je + j] = t2 - u3;
	      ajd = t2 + u3;
	      bjb = u2 + t3;
	      b[jm + j] = u2 - t3;
	      // ---------------------- 
	      ajg = a[jg + j];
	      t0 = ajb + ajg;
	      t2 = ajb - ajg;
	      ajf = a[jf + j];
	      ajh = a[jh + j];
	      t1 = ajf + ajh;
	      t3 = ss * (ajf - ajh);
	      ajj = a[jj + j];
	      ajg = ajj;
	      bje = b[je + j];
	      bjg = b[jg + j];
	      u0 = bje + bjg;
	      u2 = bje - bjg;
	      bjf = b[jf + j];
	      bjh = b[jh + j];
	      u1 = bjf + bjh;
	      u3 = ss * (bjf - bjh);
	      b[je + j] = bjb;
	      a[jb + j] = t0 + t1;
	      a[jj + j] = t0 - t1;
	      bjj = b[jj + j];
	      bjg = bjj;
	      b[jb + j] = u0 + u1;
	      b[jj + j] = u0 - u1;
	      a[jf + j] = t2 - u3;
	      ajh = t2 + u3;
	      b[jf + j] = u2 + t3;
	      bjh = u2 - t3;
	      // ---------------------- 
	      ajk = a[jk + j];
	      t0 = ajc + ajk;
	      t2 = ajc - ajk;
	      ajl = a[jl + j];
	      t1 = ajg + ajl;
	      t3 = ss * (ajg - ajl);
	      bji = b[ji + j];
	      bjk = b[jk + j];
	      u0 = bji + bjk;
	      u2 = bji - bjk;
	      ajo = a[jo + j];
	      ajl = ajo;
	      bjl = b[jl + j];
	      u1 = bjg + bjl;
	      u3 = ss * (bjg - bjl);
	      b[ji + j] = bjc;
	      a[jc + j] = t0 + t1;
	      a[jk + j] = t0 - t1;
	      bjo = b[jo + j];
	      bjl = bjo;
	      b[jc + j] = u0 + u1;
	      b[jk + j] = u0 - u1;
	      a[jg + j] = t2 - u3;
	      a[jo + j] = t2 + u3;
	      b[jg + j] = u2 + t3;
	      b[jo + j] = u2 - t3;
	      // ---------------------- 
	      ajm = a[jm + j];
	      t0 = ajm + ajl;
	      t2 = ajm - ajl;
	      ajn = a[jn + j];
	      ajp = a[jp + j];
	      t1 = ajn + ajp;
	      t3 = ss * (ajn - ajp);
	      a[jm + j] = ajd;
	      u0 = bjd + bjl;
	      u2 = bjd - bjl;
	      bjn = b[jn + j];
	      bjp = b[jp + j];
	      u1 = bjn + bjp;
	      u3 = ss * (bjn - bjp);
	      a[jn + j] = ajh;
	      a[jd + j] = t0 + t1;
	      a[jl + j] = t0 - t1;
	      b[jd + j] = u0 + u1;
	      b[jl + j] = u0 - u1;
	      b[jn + j] = bjh;
	      a[jh + j] = t2 - u3;
	      a[jp + j] = t2 + u3;
	      b[jh + j] = u2 + t3;
	      b[jp + j] = u2 - t3;
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
      i__6 = jstep - ink;
      i__2 = ink;
      for (k = ink; i__2 < 0 ? k >= i__6 : k <= i__6; k += i__2) {
	co1 = trigs[kk + 1];
	si1 = s * trigs[kk + 2];
	co2 = trigs[(kk << 1) + 1];
	si2 = s * trigs[(kk << 1) + 2];
	co3 = trigs[kk * 3 + 1];
	si3 = s * trigs[kk * 3 + 2];

	//  double loop along first transform in block 
	//  ------------------------------------------ 
	i__5 = (la - 1) * ink;
	i__4 = jstep << 2;
	for (ll = k; i__4 < 0 ? ll >= i__5 : ll <= i__5; ll += i__4) {

	  i__7 = (n - 1) * inc;
	  i__8 = (la << 2) * ink;
	  for (jjj = ll; i__8 < 0 ? jjj >= i__7 : jjj <= i__7; jjj += i__8) {
	    ja = istart + jjj;

	    //     "transverse" loop 
	    //     ----------------- 
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
	      jd = jc + jstepl;
	      if (jd < istart) {
		jd += ninc;
	      }
	      je = ja + laincl;
	      if (je < istart) {
		je += ninc;
	      }
	      jf = je + jstepl;
	      if (jf < istart) {
		jf += ninc;
	      }
	      jg = jf + jstepl;
	      if (jg < istart) {
		jg += ninc;
	      }
	      jh = jg + jstepl;
	      if (jh < istart) {
		jh += ninc;
	      }
	      ji = je + laincl;
	      if (ji < istart) {
		ji += ninc;
	      }
	      jj = ji + jstepl;
	      if (jj < istart) {
		jj += ninc;
	      }
	      jk = jj + jstepl;
	      if (jk < istart) {
		jk += ninc;
	      }
	      jl = jk + jstepl;
	      if (jl < istart) {
		jl += ninc;
	      }
	      jm = ji + laincl;
	      if (jm < istart) {
		jm += ninc;
	      }
	      jn = jm + jstepl;
	      if (jn < istart) {
		jn += ninc;
	      }
	      jo = jn + jstepl;
	      if (jo < istart) {
		jo += ninc;
	      }
	      jp = jo + jstepl;
	      if (jp < istart) {
		jp += ninc;
	      }
	      j = 0;

	      //  loop across transforms 
	      //  ---------------------- 

	      i__10 = nvex;
	      for (l = 1; l <= i__10; ++l) {
		aja = a[ja + j];
		ajc = a[jc + j];
		t0 = aja + ajc;
		t2 = aja - ajc;
		ajb = a[jb + j];
		ajd = a[jd + j];
		t1 = ajb + ajd;
		t3 = ss * (ajb - ajd);
		aji = a[ji + j];
		ajc = aji;
		bja = b[ja + j];
		bjc = b[jc + j];
		u0 = bja + bjc;
		u2 = bja - bjc;
		bjb = b[jb + j];
		bjd = b[jd + j];
		u1 = bjb + bjd;
		u3 = ss * (bjb - bjd);
		aje = a[je + j];
		ajb = aje;
		a[ja + j] = t0 + t1;
		b[ja + j] = u0 + u1;
		a[je + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
		bjb = si1 * (t2 - u3) + co1 * (u2 + t3);
		bjm = b[jm + j];
		bjd = bjm;
		a[ji + j] = co2 * (t0 - t1) - si2 * (u0 - u1);
		bjc = si2 * (t0 - t1) + co2 * (u0 - u1);
		ajd = co3 * (t2 + u3) - si3 * (u2 - t3);
		b[jm + j] = si3 * (t2 + u3) + co3 * (u2 - t3);
		// ---------------------------------------- 
		ajg = a[jg + j];
		t0 = ajb + ajg;
		t2 = ajb - ajg;
		ajf = a[jf + j];
		ajh = a[jh + j];
		t1 = ajf + ajh;
		t3 = ss * (ajf - ajh);
		ajj = a[jj + j];
		ajg = ajj;
		bje = b[je + j];
		bjg = b[jg + j];
		u0 = bje + bjg;
		u2 = bje - bjg;
		bjf = b[jf + j];
		bjh = b[jh + j];
		u1 = bjf + bjh;
		u3 = ss * (bjf - bjh);
		b[je + j] = bjb;
		a[jb + j] = t0 + t1;
		b[jb + j] = u0 + u1;
		bjj = b[jj + j];
		bjg = bjj;
		a[jf + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
		b[jf + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
		a[jj + j] = co2 * (t0 - t1) - si2 * (u0 - u1);
		b[jj + j] = si2 * (t0 - t1) + co2 * (u0 - u1);
		ajh = co3 * (t2 + u3) - si3 * (u2 - t3);
		bjh = si3 * (t2 + u3) + co3 * (u2 - t3);
		// ---------------------------------------- 
		ajk = a[jk + j];
		t0 = ajc + ajk;
		t2 = ajc - ajk;
		ajl = a[jl + j];
		t1 = ajg + ajl;
		t3 = ss * (ajg - ajl);
		bji = b[ji + j];
		bjk = b[jk + j];
		u0 = bji + bjk;
		u2 = bji - bjk;
		ajo = a[jo + j];
		ajl = ajo;
		bjl = b[jl + j];
		u1 = bjg + bjl;
		u3 = ss * (bjg - bjl);
		b[ji + j] = bjc;
		a[jc + j] = t0 + t1;
		b[jc + j] = u0 + u1;
		bjo = b[jo + j];
		bjl = bjo;
		a[jg + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
		b[jg + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
		a[jk + j] = co2 * (t0 - t1) - si2 * (u0 - u1);
		b[jk + j] = si2 * (t0 - t1) + co2 * (u0 - u1);
		a[jo + j] = co3 * (t2 + u3) - si3 * (u2 - t3);
		b[jo + j] = si3 * (t2 + u3) + co3 * (u2 - t3);
		// ---------------------------------------- 
		ajm = a[jm + j];
		t0 = ajm + ajl;
		t2 = ajm - ajl;
		ajn = a[jn + j];
		ajp = a[jp + j];
		t1 = ajn + ajp;
		t3 = ss * (ajn - ajp);
		a[jm + j] = ajd;
		u0 = bjd + bjl;
		u2 = bjd - bjl;
		a[jn + j] = ajh;
		bjn = b[jn + j];
		bjp = b[jp + j];
		u1 = bjn + bjp;
		u3 = ss * (bjn - bjp);
		b[jn + j] = bjh;
		a[jd + j] = t0 + t1;
		b[jd + j] = u0 + u1;
		a[jh + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
		b[jh + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
		a[jl + j] = co2 * (t0 - t1) - si2 * (u0 - u1);
		b[jl + j] = si2 * (t0 - t1) + co2 * (u0 - u1);
		a[jp + j] = co3 * (t2 + u3) - si3 * (u2 - t3);
		b[jp + j] = si3 * (t2 + u3) + co3 * (u2 - t3);
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
      la <<= 2;
    }
    // -----( end of loop on type II radix-4 passes ) 
    // -----( nvex transforms completed) 
  L490:
    istart += nvex * jump;
  }
  // -----( end of loop on blocks of transforms )
}

//-----------------------------------------------------------------------------//
