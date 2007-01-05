#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

/*
 * Rotmg.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  ///<summary>Computes the modified parameters for a Givens rotation.</summary>
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Rotmg 
  {
#if MANAGED
    private static readonly float  fgam    = 4096.0f;
    private static readonly float  fgamsq  = 4096.0f*4096.0f;
    private static readonly double dgam    = 4096.0;
    private static readonly double dgamsq  = 4096.0*4096.0;
#endif
    private Rotmg(){}
    ///<summary>Compute the function of this class</summary>
    internal static void Compute(ref float d1, ref float d2, ref float x1, float y1, out float[] P)
    { 
 
      P = new float[5];
      
#if MANAGED
      if( d1 < 0.0 ) 
      {
        P[0] = -1;
        d1 = 0;
        d2 = 0;
        x1 = 0;
        return;
      }
      float p1 = d1 * x1;
      float p2 = d2 * y1;
      float q1 = p1 * x1;
      float q2 = p2 * y1;       
      if( p1 == 0.0 ) 
      {
        P[0] = -2;
        P[1] = 1;
        P[4] = 1;
        return;
      }

      float c = System.Math.Abs(q1);
      float s = System.Math.Abs(q2);
      float u;
      if (c > s) 
      {
        P[0] = 0;
        P[1] = 1;
        P[2] = (p2 * y1) / (p1);
        P[4] = -y1 / x1;
        P[4] = 1;

        u = 1 - P[3] * P[2];

        if (u <= 0.0) 
        {           
          P[0] = -1;
          P[1] = 0;
          P[2] = 0;
          P[3] = 0;
          P[4] = 0;
          d1 = 0;
          d2 = 0;
          x1 = 0;
          return;
        }

        d1 /= u;
        d2 /= u;
        x1 *= u;
      } 
      else 
      {
        if (q2 < 0.0) 
        {
          P[0] = -1;
          P[1] = 0;
          P[2] = 0;
          P[3] = 0;
          P[4] = 0;
          d1 = 0;
          d2 = 0;
          x1 = 0;
          return;
        }

        P[0] = 1;

        P[1] = (p1) / (p2);
        P[2] = 1;
        P[3] = -1;
        P[4] = x1 / y1;

        u = 1 + P[1] * P[4];
        d1 /= u;
        d2 /= u; 
        float tmp = d2;
        d2 = d1;
        d1 = tmp;
      }
      x1 = y1 * u;
    
      while (d1 <= 1.0 / fgamsq && d1 != 0.0) 
      {
        P[0] = -1;
        d1 *= fgamsq;
        x1 /= fgam;
        P[1] /= fgam;
        P[2] /= fgam;
      }

      while(d1 >= fgamsq) 
      {
        P[0] = -1;
        d1 /= fgamsq;
        x1 *= fgam;
        P[1] *= fgam;
        P[2] *= fgam;
      }
      while(System.Math.Abs(d2) <= 1.0 / fgamsq && d2 != 0.0) 
      {
        P[0] = -1;
        d2 *= fgamsq;
        P[3] /= fgam;
        P[4] /= fgam;
      }

      while (System.Math.Abs(d2) >= fgamsq) 
      {
        P[0] = -1;
        d2 /= fgamsq;
        P[3] *= fgam;
        P[4] *= fgam;
      }

      if (P[0] == 0.0) 
      {
        P[1] = 1;
        P[4] = 1;
      } 
      else if (P[0] == 1.0) 
      {
        P[2] = 1;
        P[3] = -1;
      }
#else
      dna_blas_srotmg(ref d1, ref d2, ref x1, ref y1, P);
#endif
    }     

    internal static void Compute(ref double d1, ref double d2, ref double x1, double y1, out double[] P)
    {
      P = new double[5];
      
#if MANAGED
      if( d1 < 0.0 ) 
      {
        P[0] = -1;
        d1 = 0;
        d2 = 0;
        x1 = 0;
        return;
      }
      double p1 = d1 * x1;
      double p2 = d2 * y1;
      double q1 = p1 * x1;
      double q2 = p2 * y1;        
      if( p1 == 0.0 ) 
      {
        P[0] = -2;
        P[1] = 1;
        P[4] = 1;
        return;
      }

      double c = System.Math.Abs(q1);
      double s = System.Math.Abs(q2);
      double u;
      if (c > s) 
      {
        P[0] = 0;
        P[1] = 1;
        P[2] = (p2 * y1) / (p1);
        P[3] = -y1 / x1;
        P[4] = 1;

        u = 1 - P[3] * P[2];

        if (u <= 0.0) 
        {           
          P[0] = -1;
          P[1] = 0;
          P[2] = 0;
          P[3] = 0;
          P[4] = 0;
          d1 = 0;
          d2 = 0;
          x1 = 0;
          return;
        }

        d1 /= u;
        d2 /= u;
        x1 *= u;
      } 
      else 
      {
        if (q2 < 0.0) 
        {
          P[0] = -1;
          P[1] = 0;
          P[2] = 0;
          P[3] = 0;
          P[4] = 0;
          d1 = 0;
          d2 = 0;
          x1 = 0;
          return;
        }

        P[0] = 1;

        P[1] = (p1) / (p2);
        P[2] = 1;
        P[3] = -1;
        P[4] = x1 / y1;

        u = 1 + P[1] * P[4];
        d1 /= u;
        d2 /= u; 
        double tmp = d2;
        d2 = d1;
        d1 = tmp;
      }
      x1 = y1 * u;
    
      while (d1 <= 1.0 / dgamsq && d1 != 0.0) 
      {
        P[0] = -1;
        d1 *= dgamsq;
        x1 /= dgam;
        P[1] /= dgam;
        P[2] /= dgam;
      }

      while(d1 >= fgamsq) 
      {
        P[0] = -1;
        d1 /= dgamsq;
        x1 *= dgam;
        P[1] *= dgam;
        P[2] *= dgam;
      }
      while(System.Math.Abs(d2) <= 1.0 / dgamsq && d2 != 0.0) 
      {
        P[0] = -1;
        d2 *= dgamsq;
        P[3] /= dgam;
        P[4] /= dgam;
      }

      while (System.Math.Abs(d2) >= dgamsq) 
      {
        P[0] = -1;
        d2 /= dgamsq;
        P[3] *= dgam;
        P[4] *= dgam;
      }

      if (P[0] == 0.0) 
      {
        P[1] = 1;
        P[4] = 1;
      } 
      else if (P[0] == 1.0) 
      {
        P[2] = 1;
        P[3] = -1;
      }
#else
      dna_blas_drotmg(ref d1, ref d2, ref x1, ref y1, P);
#endif
    }

#if !MANAGED
    ///<summary>P/Invoke to wrapper with native code</summary>
    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_srotmg( ref float d1, ref float d2, ref float x1, ref float y1, [In,Out]float[] P );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern void dna_blas_drotmg( ref double d1, ref double d2, ref double x1, ref double y1, [In,Out]double[] P );
#endif
  }
}
