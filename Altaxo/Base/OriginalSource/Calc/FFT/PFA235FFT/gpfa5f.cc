/*-----------------------------------------------------------------------------*\
| radix-5 section of the GPFA-FFT algorithm                           gpfa5f.cc |
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
// radix-5 section of self-sorting, in-place, generalized pfa 
//-----------------------------------------------------------------------------//

void gpfa5f (FLOAT a[], FLOAT b[], FLOAT trigs[], 
	     int inc, int jump, int n, int mm, int lot, int isign)
{
  const FLOAT  sin36 = 0.587785252292473,
               sin72 = 0.951056516295154,
               qrt5  = 0.559016994374947;

  // *************************************************************** 
  // *                                                             * 
  // *  n.b. lvr = length of vector registers, set to 128 for c90. * 
  // *  reset to 64 for other cray machines, or to any large value * 
  // *  (greater than or equal to lot) for a scalar computer.      * 
  // *                                                             * 
  // *************************************************************** 

  // System generated locals 
  int i__1, i__2, i__3, i__4, i__5, i__6, i__7, i__8, i__9, i__10;

  FLOAT  s, c1, c2, c3, t1, t2, t3, t4, t5, t6, t7, t8, t9, u1, u2, u3, 
         u4, u5, u6, u7, u8, u9, t10, t11, u10, u11, ax, bx, 
         co1=0.0, co2=0.0, co3=0.0, co4=0.0, si1=0.0, si2=0.0, si3=0.0, si4=0.0, 
         aja, ajb, ajc, ajd, aje, bjb, bje, 
         bjc, bjd, bja, ajf, ajk, bjf, bjk, ajg, ajj, ajh, aji, ajl, ajq, 
         bjg, bjj, bjh, bji, bjl, bjq, ajo, ajm, ajn, ajr, ajw, bjo, bjm, 
         bjn, bjr, bjw, ajt, ajs, ajx, ajp, bjt, bjs, bjx, bjp, ajv, ajy, 
         aju, bjv, bjy, bju;

  int    ninc, left, nvex, j, k, l, m, ipass, nblox, jstep, n5, ja, jb, la, 
         jc, jd, nb, je, jf, jg, jh, mh, kk, ll, ji, jj, jk, jl, jm, mu, nu, 
         laincl, jn, jo, jp, jq, jr, js, jt, ju, jv, jw, jx, jy, jstepl,
         istart, jstepx, jjj, ink, inq;

  // Parameter adjustments 
  --trigs;
  --b;
  --a;

  n5 = powii(5, mm);
  inq = n / n5;
  jstepx = (n5 - n) * inc;
  ninc = n * inc;
  ink = inc * inq;
  mu = inq % 5;
  if (isign == -1) mu = 5 - mu;

  m = mm;
  mh = (m + 1) / 2;
  s = (FLOAT) isign;
  c1 = qrt5;
  c2 = sin72;
  c3 = sin36;
  if (mu == 2 || mu == 3) {
    c1 = -c1;
    c2 = sin36;
    c3 = sin72;
  }
  if (mu == 3 || mu == 4) c2 = -c2;
  if (mu == 2 || mu == 4) c3 = -c3;

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

    //  loop on type I radix-5 passes 
    //  ----------------------------- 
    i__2 = mh;
    for (ipass = 1; ipass <= i__2; ++ipass) {
      jstep = n * inc / (la * 5);
      jstepl = jstep - ninc;
      kk = 0;

      //  loop on k 
      //  --------- 
      i__3 = jstep - ink;
      i__4 = ink;
      for (k = 0; i__4 < 0 ? k >= i__3 : k <= i__3; k += i__4) {

	if (k > 0) {
	  co1 = trigs[kk + 1];
	  si1 = s * trigs[kk + 2];
	  co2 = trigs[(kk << 1) + 1];
	  si2 = s * trigs[(kk << 1) + 2];
	  co3 = trigs[kk * 3 + 1];
	  si3 = s * trigs[kk * 3 + 2];
	  co4 = trigs[(kk << 2) + 1];
	  si4 = s * trigs[(kk << 2) + 2];
	}

	//  loop along transform 
	//  -------------------- 
	i__5 = (n - 1) * inc;
	i__6 = jstep * 5;
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
	    je = jd + jstepl;
	    if (je < istart) {
	      je += ninc;
	    }
	    j = 0;

	    //  loop across transforms 
	    //  ---------------------- 
	    if (k == 0) {

	      // dir$ ivdep, shortloop 
	      i__8 = nvex;
	      for (l = 1; l <= i__8; ++l) {
		ajb = a[jb + j];
		aje = a[je + j];
		t1 = ajb + aje;
		ajc = a[jc + j];
		ajd = a[jd + j];
		t2 = ajc + ajd;
		t3 = ajb - aje;
		t4 = ajc - ajd;
		t5 = t1 + t2;
		t6 = c1 * (t1 - t2);
		aja = a[ja + j];
		t7 = aja - t5 * .25;
		a[ja + j] = aja + t5;
		t8 = t7 + t6;
		t9 = t7 - t6;
		t10 = c3 * t3 - c2 * t4;
		t11 = c2 * t3 + c3 * t4;
		bjb = b[jb + j];
		bje = b[je + j];
		u1 = bjb + bje;
		bjc = b[jc + j];
		bjd = b[jd + j];
		u2 = bjc + bjd;
		u3 = bjb - bje;
		u4 = bjc - bjd;
		u5 = u1 + u2;
		u6 = c1 * (u1 - u2);
		bja = b[ja + j];
		u7 = bja - u5 * .25;
		b[ja + j] = bja + u5;
		u8 = u7 + u6;
		u9 = u7 - u6;
		u10 = c3 * u3 - c2 * u4;
		u11 = c2 * u3 + c3 * u4;
		a[jb + j] = t8 - u11;
		b[jb + j] = u8 + t11;
		a[je + j] = t8 + u11;
		b[je + j] = u8 - t11;
		a[jc + j] = t9 - u10;
		b[jc + j] = u9 + t10;
		a[jd + j] = t9 + u10;
		b[jd + j] = u9 - t10;
		j += jump;
	      }

	    } else {

	      // dir$ ivdep,shortloop 
	      i__8 = nvex;
	      for (l = 1; l <= i__8; ++l) {
		ajb = a[jb + j];
		aje = a[je + j];
		t1 = ajb + aje;
		ajc = a[jc + j];
		ajd = a[jd + j];
		t2 = ajc + ajd;
		t3 = ajb - aje;
		t4 = ajc - ajd;
		t5 = t1 + t2;
		t6 = c1 * (t1 - t2);
		aja = a[ja + j];
		t7 = aja - t5 * .25;
		a[ja + j] = aja + t5;
		t8 = t7 + t6;
		t9 = t7 - t6;
		t10 = c3 * t3 - c2 * t4;
		t11 = c2 * t3 + c3 * t4;
		bjb = b[jb + j];
		bje = b[je + j];
		u1 = bjb + bje;
		bjc = b[jc + j];
		bjd = b[jd + j];
		u2 = bjc + bjd;
		u3 = bjb - bje;
		u4 = bjc - bjd;
		u5 = u1 + u2;
		u6 = c1 * (u1 - u2);
		bja = b[ja + j];
		u7 = bja - u5 * .25;
		b[ja + j] = bja + u5;
		u8 = u7 + u6;
		u9 = u7 - u6;
		u10 = c3 * u3 - c2 * u4;
		u11 = c2 * u3 + c3 * u4;
		a[jb + j] = co1 * (t8 - u11) - si1 * (u8 + 
						      t11);
		b[jb + j] = si1 * (t8 - u11) + co1 * (u8 + 
						      t11);
		a[je + j] = co4 * (t8 + u11) - si4 * (u8 - 
						      t11);
		b[je + j] = si4 * (t8 + u11) + co4 * (u8 - 
						      t11);
		a[jc + j] = co2 * (t9 - u10) - si2 * (u9 + 
						      t10);
		b[jc + j] = si2 * (t9 - u10) + co2 * (u9 + 
						      t10);
		a[jd + j] = co3 * (t9 + u10) - si3 * (u9 - 
						      t10);
		b[jd + j] = si3 * (t9 + u10) + co3 * (u9 - 
						      t10);
		j += jump;
	      }

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
      la *= 5;
    }
    // -----( end of loop on type I radix-5 passes) 

    if (n == 5) goto L490;

    //  loop on type II radix-5 passes 
    //  ------------------------------ 

    i__2 = m;
    for (ipass = mh + 1; ipass <= i__2; ++ipass) {
      jstep = n * inc / (la * 5);
      jstepl = jstep - ninc;
      laincl = la * ink - ninc;
      kk = 0;

      //     loop on k 
      //     --------- 
      i__4 = jstep - ink;
      i__3 = ink;
      for (k = 0; i__3 < 0 ? k >= i__4 : k <= i__4; k += i__3) {

	if (k > 0) {
	  co1 = trigs[kk + 1];
	  si1 = s * trigs[kk + 2];
	  co2 = trigs[(kk << 1) + 1];
	  si2 = s * trigs[(kk << 1) + 2];
	  co3 = trigs[kk * 3 + 1];
	  si3 = s * trigs[kk * 3 + 2];
	  co4 = trigs[(kk << 2) + 1];
	  si4 = s * trigs[(kk << 2) + 2];
	}

	//  double loop along first transform in block 
	//  ------------------------------------------ 
	i__6 = (la - 1) * ink;
	i__5 = jstep * 5;
	for (ll = k; i__5 < 0 ? ll >= i__6 : ll <= i__6; ll += i__5) {

	  i__7 = (n - 1) * inc;
	  i__8 = la * 5 * ink;
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
	      je = jd + jstepl;
	      if (je < istart) {
		je += ninc;
	      }
	      jf = ja + laincl;
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
	      ji = jh + jstepl;
	      if (ji < istart) {
		ji += ninc;
	      }
	      jj = ji + jstepl;
	      if (jj < istart) {
		jj += ninc;
	      }
	      jk = jf + laincl;
	      if (jk < istart) {
		jk += ninc;
	      }
	      jl = jk + jstepl;
	      if (jl < istart) {
		jl += ninc;
	      }
	      jm = jl + jstepl;
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
	      jp = jk + laincl;
	      if (jp < istart) {
		jp += ninc;
	      }
	      jq = jp + jstepl;
	      if (jq < istart) {
		jq += ninc;
	      }
	      jr = jq + jstepl;
	      if (jr < istart) {
		jr += ninc;
	      }
	      js = jr + jstepl;
	      if (js < istart) {
		js += ninc;
	      }
	      jt = js + jstepl;
	      if (jt < istart) {
		jt += ninc;
	      }
	      ju = jp + laincl;
	      if (ju < istart) {
		ju += ninc;
	      }
	      jv = ju + jstepl;
	      if (jv < istart) {
		jv += ninc;
	      }
	      jw = jv + jstepl;
	      if (jw < istart) {
		jw += ninc;
	      }
	      jx = jw + jstepl;
	      if (jx < istart) {
		jx += ninc;
	      }
	      jy = jx + jstepl;
	      if (jy < istart) {
		jy += ninc;
	      }
	      j = 0;

	      //  loop across transforms 
	      //  ---------------------- 
	      if (k == 0) {

		// dir$ ivdep, shortloop 
		i__10 = nvex;
		for (l = 1; l <= i__10; ++l) {
		  ajb = a[jb + j];
		  aje = a[je + j];
		  t1 = ajb + aje;
		  ajc = a[jc + j];
		  ajd = a[jd + j];
		  t2 = ajc + ajd;
		  t3 = ajb - aje;
		  t4 = ajc - ajd;
		  ajf = a[jf + j];
		  ajb = ajf;
		  t5 = t1 + t2;
		  t6 = c1 * (t1 - t2);
		  aja = a[ja + j];
		  t7 = aja - t5 * .25;
		  a[ja + j] = aja + t5;
		  t8 = t7 + t6;
		  t9 = t7 - t6;
		  ajk = a[jk + j];
		  ajc = ajk;
		  t10 = c3 * t3 - c2 * t4;
		  t11 = c2 * t3 + c3 * t4;
		  bjb = b[jb + j];
		  bje = b[je + j];
		  u1 = bjb + bje;
		  bjc = b[jc + j];
		  bjd = b[jd + j];
		  u2 = bjc + bjd;
		  u3 = bjb - bje;
		  u4 = bjc - bjd;
		  bjf = b[jf + j];
		  bjb = bjf;
		  u5 = u1 + u2;
		  u6 = c1 * (u1 - u2);
		  bja = b[ja + j];
		  u7 = bja - u5 * .25;
		  b[ja + j] = bja + u5;
		  u8 = u7 + u6;
		  u9 = u7 - u6;
		  bjk = b[jk + j];
		  bjc = bjk;
		  u10 = c3 * u3 - c2 * u4;
		  u11 = c2 * u3 + c3 * u4;
		  a[jf + j] = t8 - u11;
		  b[jf + j] = u8 + t11;
		  aje = t8 + u11;
		  bje = u8 - t11;
		  a[jk + j] = t9 - u10;
		  b[jk + j] = u9 + t10;
		  ajd = t9 + u10;
		  bjd = u9 - t10;
		  // ---------------------- 
		  ajg = a[jg + j];
		  ajj = a[jj + j];
		  t1 = ajg + ajj;
		  ajh = a[jh + j];
		  aji = a[ji + j];
		  t2 = ajh + aji;
		  t3 = ajg - ajj;
		  t4 = ajh - aji;
		  ajl = a[jl + j];
		  ajh = ajl;
		  t5 = t1 + t2;
		  t6 = c1 * (t1 - t2);
		  t7 = ajb - t5 * .25;
		  a[jb + j] = ajb + t5;
		  t8 = t7 + t6;
		  t9 = t7 - t6;
		  ajq = a[jq + j];
		  aji = ajq;
		  t10 = c3 * t3 - c2 * t4;
		  t11 = c2 * t3 + c3 * t4;
		  bjg = b[jg + j];
		  bjj = b[jj + j];
		  u1 = bjg + bjj;
		  bjh = b[jh + j];
		  bji = b[ji + j];
		  u2 = bjh + bji;
		  u3 = bjg - bjj;
		  u4 = bjh - bji;
		  bjl = b[jl + j];
		  bjh = bjl;
		  u5 = u1 + u2;
		  u6 = c1 * (u1 - u2);
		  u7 = bjb - u5 * .25;
		  b[jb + j] = bjb + u5;
		  u8 = u7 + u6;
		  u9 = u7 - u6;
		  bjq = b[jq + j];
		  bji = bjq;
		  u10 = c3 * u3 - c2 * u4;
		  u11 = c2 * u3 + c3 * u4;
		  a[jg + j] = t8 - u11;
		  b[jg + j] = u8 + t11;
		  ajj = t8 + u11;
		  bjj = u8 - t11;
		  a[jl + j] = t9 - u10;
		  b[jl + j] = u9 + t10;
		  a[jq + j] = t9 + u10;
		  b[jq + j] = u9 - t10;
		  // ---------------------- 
		  ajo = a[jo + j];
		  t1 = ajh + ajo;
		  ajm = a[jm + j];
		  ajn = a[jn + j];
		  t2 = ajm + ajn;
		  t3 = ajh - ajo;
		  t4 = ajm - ajn;
		  ajr = a[jr + j];
		  ajn = ajr;
		  t5 = t1 + t2;
		  t6 = c1 * (t1 - t2);
		  t7 = ajc - t5 * .25;
		  a[jc + j] = ajc + t5;
		  t8 = t7 + t6;
		  t9 = t7 - t6;
		  ajw = a[jw + j];
		  ajo = ajw;
		  t10 = c3 * t3 - c2 * t4;
		  t11 = c2 * t3 + c3 * t4;
		  bjo = b[jo + j];
		  u1 = bjh + bjo;
		  bjm = b[jm + j];
		  bjn = b[jn + j];
		  u2 = bjm + bjn;
		  u3 = bjh - bjo;
		  u4 = bjm - bjn;
		  bjr = b[jr + j];
		  bjn = bjr;
		  u5 = u1 + u2;
		  u6 = c1 * (u1 - u2);
		  u7 = bjc - u5 * .25;
		  b[jc + j] = bjc + u5;
		  u8 = u7 + u6;
		  u9 = u7 - u6;
		  bjw = b[jw + j];
		  bjo = bjw;
		  u10 = c3 * u3 - c2 * u4;
		  u11 = c2 * u3 + c3 * u4;
		  a[jh + j] = t8 - u11;
		  b[jh + j] = u8 + t11;
		  a[jw + j] = t8 + u11;
		  b[jw + j] = u8 - t11;
		  a[jm + j] = t9 - u10;
		  b[jm + j] = u9 + t10;
		  a[jr + j] = t9 + u10;
		  b[jr + j] = u9 - t10;
		  // ---------------------- 
		  ajt = a[jt + j];
		  t1 = aji + ajt;
		  ajs = a[js + j];
		  t2 = ajn + ajs;
		  t3 = aji - ajt;
		  t4 = ajn - ajs;
		  ajx = a[jx + j];
		  ajt = ajx;
		  t5 = t1 + t2;
		  t6 = c1 * (t1 - t2);
		  ajp = a[jp + j];
		  t7 = ajp - t5 * .25;
		  ax = ajp + t5;
		  t8 = t7 + t6;
		  t9 = t7 - t6;
		  a[jp + j] = ajd;
		  t10 = c3 * t3 - c2 * t4;
		  t11 = c2 * t3 + c3 * t4;
		  a[jd + j] = ax;
		  bjt = b[jt + j];
		  u1 = bji + bjt;
		  bjs = b[js + j];
		  u2 = bjn + bjs;
		  u3 = bji - bjt;
		  u4 = bjn - bjs;
		  bjx = b[jx + j];
		  bjt = bjx;
		  u5 = u1 + u2;
		  u6 = c1 * (u1 - u2);
		  bjp = b[jp + j];
		  u7 = bjp - u5 * .25;
		  bx = bjp + u5;
		  u8 = u7 + u6;
		  u9 = u7 - u6;
		  b[jp + j] = bjd;
		  u10 = c3 * u3 - c2 * u4;
		  u11 = c2 * u3 + c3 * u4;
		  b[jd + j] = bx;
		  a[ji + j] = t8 - u11;
		  b[ji + j] = u8 + t11;
		  a[jx + j] = t8 + u11;
		  b[jx + j] = u8 - t11;
		  a[jn + j] = t9 - u10;
		  b[jn + j] = u9 + t10;
		  a[js + j] = t9 + u10;
		  b[js + j] = u9 - t10;
		  // ---------------------- 
		  ajv = a[jv + j];
		  ajy = a[jy + j];
		  t1 = ajv + ajy;
		  t2 = ajo + ajt;
		  t3 = ajv - ajy;
		  t4 = ajo - ajt;
		  a[jv + j] = ajj;
		  t5 = t1 + t2;
		  t6 = c1 * (t1 - t2);
		  aju = a[ju + j];
		  t7 = aju - t5 * .25;
		  ax = aju + t5;
		  t8 = t7 + t6;
		  t9 = t7 - t6;
		  a[ju + j] = aje;
		  t10 = c3 * t3 - c2 * t4;
		  t11 = c2 * t3 + c3 * t4;
		  a[je + j] = ax;
		  bjv = b[jv + j];
		  bjy = b[jy + j];
		  u1 = bjv + bjy;
		  u2 = bjo + bjt;
		  u3 = bjv - bjy;
		  u4 = bjo - bjt;
		  b[jv + j] = bjj;
		  u5 = u1 + u2;
		  u6 = c1 * (u1 - u2);
		  bju = b[ju + j];
		  u7 = bju - u5 * .25;
		  bx = bju + u5;
		  u8 = u7 + u6;
		  u9 = u7 - u6;
		  b[ju + j] = bje;
		  u10 = c3 * u3 - c2 * u4;
		  u11 = c2 * u3 + c3 * u4;
		  b[je + j] = bx;
		  a[jj + j] = t8 - u11;
		  b[jj + j] = u8 + t11;
		  a[jy + j] = t8 + u11;
		  b[jy + j] = u8 - t11;
		  a[jo + j] = t9 - u10;
		  b[jo + j] = u9 + t10;
		  a[jt + j] = t9 + u10;
		  b[jt + j] = u9 - t10;
		  j += jump;
		}

	      } else {

		// dir$ ivdep, shortloop 
		i__10 = nvex;
		for (l = 1; l <= i__10; ++l) {
		  ajb = a[jb + j];
		  aje = a[je + j];
		  t1 = ajb + aje;
		  ajc = a[jc + j];
		  ajd = a[jd + j];
		  t2 = ajc + ajd;
		  t3 = ajb - aje;
		  t4 = ajc - ajd;
		  ajf = a[jf + j];
		  ajb = ajf;
		  t5 = t1 + t2;
		  t6 = c1 * (t1 - t2);
		  aja = a[ja + j];
		  t7 = aja - t5 * .25;
		  a[ja + j] = aja + t5;
		  t8 = t7 + t6;
		  t9 = t7 - t6;
		  ajk = a[jk + j];
		  ajc = ajk;
		  t10 = c3 * t3 - c2 * t4;
		  t11 = c2 * t3 + c3 * t4;
		  bjb = b[jb + j];
		  bje = b[je + j];
		  u1 = bjb + bje;
		  bjc = b[jc + j];
		  bjd = b[jd + j];
		  u2 = bjc + bjd;
		  u3 = bjb - bje;
		  u4 = bjc - bjd;
		  bjf = b[jf + j];
		  bjb = bjf;
		  u5 = u1 + u2;
		  u6 = c1 * (u1 - u2);
		  bja = b[ja + j];
		  u7 = bja - u5 * .25;
		  b[ja + j] = bja + u5;
		  u8 = u7 + u6;
		  u9 = u7 - u6;
		  bjk = b[jk + j];
		  bjc = bjk;
		  u10 = c3 * u3 - c2 * u4;
		  u11 = c2 * u3 + c3 * u4;
		  a[jf + j] = co1 * (t8 - u11) - si1 * (u8 
							+ t11);
		  b[jf + j] = si1 * (t8 - u11) + co1 * (u8 
							+ t11);
		  aje = co4 * (t8 + u11) - si4 * (u8 - t11);
		  bje = si4 * (t8 + u11) + co4 * (u8 - t11);
		  a[jk + j] = co2 * (t9 - u10) - si2 * (u9 
							+ t10);
		  b[jk + j] = si2 * (t9 - u10) + co2 * (u9 
							+ t10);
		  ajd = co3 * (t9 + u10) - si3 * (u9 - t10);
		  bjd = si3 * (t9 + u10) + co3 * (u9 - t10);
		  // ---------------------- 
		  ajg = a[jg + j];
		  ajj = a[jj + j];
		  t1 = ajg + ajj;
		  ajh = a[jh + j];
		  aji = a[ji + j];
		  t2 = ajh + aji;
		  t3 = ajg - ajj;
		  t4 = ajh - aji;
		  ajl = a[jl + j];
		  ajh = ajl;
		  t5 = t1 + t2;
		  t6 = c1 * (t1 - t2);
		  t7 = ajb - t5 * .25;
		  a[jb + j] = ajb + t5;
		  t8 = t7 + t6;
		  t9 = t7 - t6;
		  ajq = a[jq + j];
		  aji = ajq;
		  t10 = c3 * t3 - c2 * t4;
		  t11 = c2 * t3 + c3 * t4;
		  bjg = b[jg + j];
		  bjj = b[jj + j];
		  u1 = bjg + bjj;
		  bjh = b[jh + j];
		  bji = b[ji + j];
		  u2 = bjh + bji;
		  u3 = bjg - bjj;
		  u4 = bjh - bji;
		  bjl = b[jl + j];
		  bjh = bjl;
		  u5 = u1 + u2;
		  u6 = c1 * (u1 - u2);
		  u7 = bjb - u5 * .25;
		  b[jb + j] = bjb + u5;
		  u8 = u7 + u6;
		  u9 = u7 - u6;
		  bjq = b[jq + j];
		  bji = bjq;
		  u10 = c3 * u3 - c2 * u4;
		  u11 = c2 * u3 + c3 * u4;
		  a[jg + j] = co1 * (t8 - u11) - si1 * (u8 + t11);
		  b[jg + j] = si1 * (t8 - u11) + co1 * (u8 + t11);
		  ajj = co4 * (t8 + u11) - si4 * (u8 - t11);
		  bjj = si4 * (t8 + u11) + co4 * (u8 - t11);
		  a[jl + j] = co2 * (t9 - u10) - si2 * (u9 + t10);
		  b[jl + j] = si2 * (t9 - u10) + co2 * (u9 + t10);
		  a[jq + j] = co3 * (t9 + u10) - si3 * (u9 - t10);
		  b[jq + j] = si3 * (t9 + u10) + co3 * (u9 - t10);
		  // ---------------------- 
		  ajo = a[jo + j];
		  t1 = ajh + ajo;
		  ajm = a[jm + j];
		  ajn = a[jn + j];
		  t2 = ajm + ajn;
		  t3 = ajh - ajo;
		  t4 = ajm - ajn;
		  ajr = a[jr + j];
		  ajn = ajr;
		  t5 = t1 + t2;
		  t6 = c1 * (t1 - t2);
		  t7 = ajc - t5 * .25;
		  a[jc + j] = ajc + t5;
		  t8 = t7 + t6;
		  t9 = t7 - t6;
		  ajw = a[jw + j];
		  ajo = ajw;
		  t10 = c3 * t3 - c2 * t4;
		  t11 = c2 * t3 + c3 * t4;
		  bjo = b[jo + j];
		  u1 = bjh + bjo;
		  bjm = b[jm + j];
		  bjn = b[jn + j];
		  u2 = bjm + bjn;
		  u3 = bjh - bjo;
		  u4 = bjm - bjn;
		  bjr = b[jr + j];
		  bjn = bjr;
		  u5 = u1 + u2;
		  u6 = c1 * (u1 - u2);
		  u7 = bjc - u5 * .25;
		  b[jc + j] = bjc + u5;
		  u8 = u7 + u6;
		  u9 = u7 - u6;
		  bjw = b[jw + j];
		  bjo = bjw;
		  u10 = c3 * u3 - c2 * u4;
		  u11 = c2 * u3 + c3 * u4;
		  a[jh + j] = co1 * (t8 - u11) - si1 * (u8 + t11);
		  b[jh + j] = si1 * (t8 - u11) + co1 * (u8 + t11);
		  a[jw + j] = co4 * (t8 + u11) - si4 * (u8 - t11);
		  b[jw + j] = si4 * (t8 + u11) + co4 * (u8 - t11);
		  a[jm + j] = co2 * (t9 - u10) - si2 * (u9 + t10);
		  b[jm + j] = si2 * (t9 - u10) + co2 * (u9 + t10);
		  a[jr + j] = co3 * (t9 + u10) - si3 * (u9 - t10);
		  b[jr + j] = si3 * (t9 + u10) + co3 * (u9 - t10);
		  // ---------------------- 
		  ajt = a[jt + j];
		  t1 = aji + ajt;
		  ajs = a[js + j];
		  t2 = ajn + ajs;
		  t3 = aji - ajt;
		  t4 = ajn - ajs;
		  ajx = a[jx + j];
		  ajt = ajx;
		  t5 = t1 + t2;
		  t6 = c1 * (t1 - t2);
		  ajp = a[jp + j];
		  t7 = ajp - t5 * .25;
		  ax = ajp + t5;
		  t8 = t7 + t6;
		  t9 = t7 - t6;
		  a[jp + j] = ajd;
		  t10 = c3 * t3 - c2 * t4;
		  t11 = c2 * t3 + c3 * t4;
		  a[jd + j] = ax;
		  bjt = b[jt + j];
		  u1 = bji + bjt;
		  bjs = b[js + j];
		  u2 = bjn + bjs;
		  u3 = bji - bjt;
		  u4 = bjn - bjs;
		  bjx = b[jx + j];
		  bjt = bjx;
		  u5 = u1 + u2;
		  u6 = c1 * (u1 - u2);
		  bjp = b[jp + j];
		  u7 = bjp - u5 * .25;
		  bx = bjp + u5;
		  u8 = u7 + u6;
		  u9 = u7 - u6;
		  b[jp + j] = bjd;
		  u10 = c3 * u3 - c2 * u4;
		  u11 = c2 * u3 + c3 * u4;
		  b[jd + j] = bx;
		  a[ji + j] = co1 * (t8 - u11) - si1 * (u8 + t11);
		  b[ji + j] = si1 * (t8 - u11) + co1 * (u8 + t11);
		  a[jx + j] = co4 * (t8 + u11) - si4 * (u8 - t11);
		  b[jx + j] = si4 * (t8 + u11) + co4 * (u8 - t11);
		  a[jn + j] = co2 * (t9 - u10) - si2 * (u9 + t10);
		  b[jn + j] = si2 * (t9 - u10) + co2 * (u9 + t10);
		  a[js + j] = co3 * (t9 + u10) - si3 * (u9 - t10);
		  b[js + j] = si3 * (t9 + u10) + co3 * (u9 - t10);
		  // ---------------------- 
		  ajv = a[jv + j];
		  ajy = a[jy + j];
		  t1 = ajv + ajy;
		  t2 = ajo + ajt;
		  t3 = ajv - ajy;
		  t4 = ajo - ajt;
		  a[jv + j] = ajj;
		  t5 = t1 + t2;
		  t6 = c1 * (t1 - t2);
		  aju = a[ju + j];
		  t7 = aju - t5 * .25;
		  ax = aju + t5;
		  t8 = t7 + t6;
		  t9 = t7 - t6;
		  a[ju + j] = aje;
		  t10 = c3 * t3 - c2 * t4;
		  t11 = c2 * t3 + c3 * t4;
		  a[je + j] = ax;
		  bjv = b[jv + j];
		  bjy = b[jy + j];
		  u1 = bjv + bjy;
		  u2 = bjo + bjt;
		  u3 = bjv - bjy;
		  u4 = bjo - bjt;
		  b[jv + j] = bjj;
		  u5 = u1 + u2;
		  u6 = c1 * (u1 - u2);
		  bju = b[ju + j];
		  u7 = bju - u5 * .25;
		  bx = bju + u5;
		  u8 = u7 + u6;
		  u9 = u7 - u6;
		  b[ju + j] = bje;
		  u10 = c3 * u3 - c2 * u4;
		  u11 = c2 * u3 + c3 * u4;
		  b[je + j] = bx;
		  a[jj + j] = co1 * (t8 - u11) - si1 * (u8 + t11);
		  b[jj + j] = si1 * (t8 - u11) + co1 * (u8 + t11);
		  a[jy + j] = co4 * (t8 + u11) - si4 * (u8 - t11);
		  b[jy + j] = si4 * (t8 + u11) + co4 * (u8 - t11);
		  a[jo + j] = co2 * (t9 - u10) - si2 * (u9 + t10);
		  b[jo + j] = si2 * (t9 - u10) + co2 * (u9 + t10);
		  a[jt + j] = co3 * (t9 + u10) - si3 * (u9 - t10);
		  b[jt + j] = si3 * (t9 + u10) + co3 * (u9 - t10);
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
	}
	// -----( end of double loop for this k ) 
	kk += la << 1;
      }
      // -----( end of loop over values of k ) 
      la *= 5;
    }
    // -----( end of loop on type II radix-5 passes ) 
    // -----( nvex transforms completed) 
  L490:
    istart += nvex * jump;
  }
  // -----( end of loop on blocks of transforms )
}

//-----------------------------------------------------------------------------//
