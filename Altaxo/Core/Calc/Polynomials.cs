#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

// The following code was translated using Matpack sources (http://www.matpack.de) (Author B.Gammel)
// Original MatPack-1.7.3\Source\hermiteh.cc
//                               laguerrel.cc
//                               jacobip.cc
//                               chebyshevt.cc
//                               legendrep.cc
//                               legendrepa.cc
//                               harmonicy.cc


using System;


namespace Altaxo.Calc
{
  /// <summary>
  /// Polynomials.
  /// </summary>
  public class Polynomials
  {
    #region HermiteH

    /// <summary>
    /// Computes the value of the Hermite polynomial of degree n 
    /// and its first and second derivatives at a given point. 
    /// </summary>
    /// <param name="n">Degree of the polynomial &gt;= 0.</param>
    /// <param name="x">Point in which the computation is performed.</param>
    /// <param name="y">Output: value of the polynomial in x.</param>
    /// <param name="dy">Output: value of the first derivative in x.</param>
    /// <param name="d2y">Output: value of the second derivative in x.</param>
    /// <remarks><code>
    /// Note: 
    ///   This C++ implementation is based on the Fortran function 
    ///      VAHEPO
    ///   from
    ///       "Fortran routines for spectral methods"
    ///   by  Daniele Funaro 
    ///       Department of Mathematics 
    ///       University of Pavia 
    ///       Via Abbiategrasso 209, 27100 Pavia, Italy 
    ///       e-mails: fun18@ipvian.ian.pv.cnr.it 
    ///                funaro@dragon.ian.pv.cnr.it 
    /// </code></remarks>
    public static void HermiteH (int n, double x, out double y, out double dy, out double d2y)
    {
      // check parameters
      if (n < 0)
        throw new ArgumentException("bad argument, n<0");

      double dk, dn, ym, yp, dnn, ypm = 0.0;
      y = 1.0;
      dy = 0.0;
      d2y = 0.0;
      if (n == 0) return;

      y = x * 2.0;
      dy = 2.0;
      d2y = 0.0;
      if (n == 1) return;

      yp = 1.0;
      for (int k = 2; k <= n; ++k) 
      {
        dk = (double) (k - 1);
        ym = y;
        y = x * 2.0 * y - dk * 2.0 * yp;
        ypm = yp;
        yp = ym;
      }
      dn = n * 2.0;
      dnn = (n - 1) * 2.0;
      dy = dn * yp;
      d2y = dn * dnn * ypm;
    }

    #endregion

    #region LaguerreL
   
    /// <summary>
    /// Computes the value of the Laguerre polynomial of degree n 
    /// and its first and second derivatives at a given point. 
    /// </summary>
    /// <param name="n">Degree of the polynomial </param>
    /// <param name="a">Parameter &gt; -1 </param>
    /// <param name="x">Point in which the computation is performed, x &gt;= 0</param>
    /// <param name="y">Output: value of the polynomial in x </param>
    /// <param name="dy">Output: value of the first derivative in x </param>
    /// <param name="d2y">Output: value of the second derivative in x</param>
    /// <remarks><code>
    /// Note: 
    ///   This C++ implementation is based on the Fortran function 
    ///       VALAPO
    ///   from
    ///       "Fortran routines for spectral methods"
    ///   by  Daniele Funaro 
    ///       Department of Mathematics 
    ///       University of Pavia 
    ///       Via Abbiategrasso 209, 27100 Pavia, Italy 
    ///       e-mails: fun18@ipvian.ian.pv.cnr.it 
    ///                funaro@dragon.ian.pv.cnr.it 
    /// </code></remarks>
    public static void LaguerreL (int n, double a, double x, out double y, out double dy, out double d2y)
    {
      // check parameters
      if (n < 0 || a <= -1.0 || x < 0.0)
        throw new ArgumentException("bad argument(s)");

      double b1, b2, dk, ym, yp, dym, dyp, d2ym, d2yp;
 
      y = 1.0;
      dy = 0.0;
      d2y = 0.0;
      if (n == 0) return;

      y = a + 1.0 - x;
      dy = -1.0;
      d2y = 0.0;
      if (n == 1) return;

      yp = 1.0;
      dyp = 0.0;
      d2yp = 0.0;
      for (int k = 2; k <= n; ++k) 
      {
        dk = (double) k;
        b1 = (dk * 2.0 + a - 1.0 - x) / dk;
        b2 = (dk + a - 1.0) / dk;
        ym = y;
        y = b1 * y - b2 * yp;
        yp = ym;
        dym = dy;
        dy = b1 * dy - yp / dk - b2 * dyp;
        dyp = dym;
        d2ym = d2y;
        d2y = b1 * d2y - dyp * 2.0 / dk - b2 * d2yp;
        d2yp = d2ym;
      }
    }
    #endregion

    #region JacobiP
   

    /// <summary>
    /// Computes the value of the Jacobi polynomial of degree n 
    /// and its first and second derivatives at a given point.
    /// </summary>
    /// <param name="n">Degree of the polynomial &gt;= 0.</param>
    /// <param name="a">Parameter &gt; -1.</param>
    /// <param name="b">Parameter &gt; -1.</param>
    /// <param name="x">Point in which the computation is performed, -1 &lt;= x &lt;= 1.</param>
    /// <param name="y">Output: value of the polynomial in x.</param>
    /// <param name="dy">Output: value of the first derivative in x.</param>
    /// <param name="d2y">Output: value of the second derivative in x.</param>
    /// <remarks><code>
    /// Note: 
    ///   This C++ implementation is based on the Fortran function 
    ///       VAJAPO
    ///   from
    ///       "Fortran routines for spectral methods"
    ///   by  Daniele Funaro 
    ///       Department of Mathematics 
    ///       University of Pavia 
    ///       Via Abbiategrasso 209, 27100 Pavia, Italy 
    ///       e-mails: fun18@ipvian.ian.pv.cnr.it 
    ///                funaro@dragon.ian.pv.cnr.it 
    /// </code></remarks>
    public static void JacobiP (int n, double a, double b, double x, 
      out double y, out double dy, out double d2y)
    {
      // check parameters
      if (n < 0 || a <= -1.0 || b <= -1.0 || Math.Abs(x) > 1.0)
        throw new ArgumentException("bad argument(s)");
    
      double c0, c1, c2, c3, c4, ab, di, ym, yp, dym, dyp, d2ym, d2yp;

      y = 1.0;
      dy = 0.0;
      d2y = 0.0;
      if (n == 0) return;

      ab = a + b;
      y = (ab + 2.0) * 0.5 * x + (a - b) * 0.5;
      dy = (ab + 2.0) * 0.5;
      d2y = 0.0;
      if (n == 1) return;

      yp = 1.0;
      dyp = 0.0;
      d2yp = 0.0;
      for (int i = 2; i <= n; ++i) 
      {
        di = (double) i;
        c0 = di * 2.0 + ab;
        c1 = di * 2.0 * (di + ab) * (c0 - 2.0);
        c2 = (c0 - 1.0) * (c0 - 2.0) * c0;
        c3 = (c0 - 1.0) * (a - b) * ab;
        c4 = (di + a - 1.0) * 2.0 * c0 * (di + b - 1.0);
        ym = y;
        y = ((c2 * x + c3) * y - c4 * yp) / c1;
        yp = ym;
        dym = dy;
        dy = ((c2 * x + c3) * dy - c4 * dyp + c2 * yp) / c1;
        dyp = dym;
        d2ym = d2y;
        d2y = ((c2 * x + c3) * d2y - c4 * d2yp + c2 * 2.0 * dyp) / c1;
        d2yp = d2ym;
      }
    }
    #endregion

    #region ChebyshevT
    

    /// <summary>
    /// Computes the value of the Chebyshev polynomial of degree n 
    /// and its first and second derivatives at a given point.
    /// </summary>
    /// <param name="n">Degree of the polynomial &gt;= 0.</param>
    /// <param name="x">Point in which the computation is performed, -1 &lt;= x &lt;= 1.</param>
    /// <param name="y">Output: value of the polynomial in x.</param>
    /// <param name="dy">Output: value of the first derivative in x.</param>
    /// <param name="d2y">Output: value of the second derivative in x.</param>
    /// <remarks><code>
    /// Note: 
    ///   This C++ implementation is based on the Fortran function 
    ///      VACHPO
    ///   from
    ///       "Fortran routines for spectral methods"
    ///   by  Daniele Funaro 
    ///       Department of Mathematics 
    ///       University of Pavia 
    ///       Via Abbiategrasso 209, 27100 Pavia, Italy 
    ///       e-mails: fun18@ipvian.ian.pv.cnr.it 
    ///                funaro@dragon.ian.pv.cnr.it 
    /// </code></remarks>
    public static void ChebyshevT (int n, double x, out double y, out double dy, out double d2y)
    {
      // check parameters
      if (n < 0 || Math.Abs(x) > 1.0)
        throw new ArgumentException("bad argument(s)");

      double ym, yp, dym, dyp, d2ym, d2yp;

      y = 1.0;
      dy = 0.0;
      d2y = 0.0;
      if (n == 0) return;

      y = x;
      dy = 1.0;
      d2y = 0.0;
      if (n == 1) return;

      yp = 1.0;
      dyp = 0.0;
      d2yp = 0.0;
      for (int k = 2; k <= n; ++k) 
      {
        ym = y;
        y = x * 2.0 * y - yp;
        yp = ym;
        dym = dy;
        dy = x * 2.0 * dy + yp * 2.0 - dyp;
        dyp = dym;
        d2ym = d2y;
        d2y = x * 2.0 * d2y + dyp * 4.0 - d2yp;
        d2yp = d2ym;
      }
    }
    #endregion

    #region LegendreP
    

    /// <summary>
    /// Computes the value of the Legendre polynomial of degree n 
    /// and its first and second derivatives at a given point. 
    /// </summary>
    /// <param name="n">Degree of the polynomial  &gt;= 0.</param>
    /// <param name="x">Point in which the computation is performed, -1 &lt;= x &lt;= 1.</param>
    /// <param name="y">Output: value of the polynomial in x.</param>
    /// <param name="dy">Output: value of the first derivative in x.</param>
    /// <param name="d2y">Output: value of the second derivative in x.</param>
    /// <remarks><code>
    /// Note: 
    ///   This C++ implementation is based on the Fortran function 
    ///      VALEPO
    ///   from
    ///       "Fortran routines for spectral methods"
    ///   by  Daniele Funaro 
    ///       Department of Mathematics 
    ///       University of Pavia 
    ///       Via Abbiategrasso 209, 27100 Pavia, Italy 
    ///       e-mails: fun18@ipvian.ian.pv.cnr.it 
    ///                funaro@dragon.ian.pv.cnr.it 
    /// </code></remarks>
    public static void LegendreP (int n, double x, out double y, out double dy, out double d2y)
    {
      // check parameters
      if (n < 0 || Math.Abs(x) > 1.0)
        throw new ArgumentException("bad argument(s)");

      double c1, c2, c4, ym, yp, dym, dyp, d2ym, d2yp;

      y = 1.0;
      dy = 0.0;
      d2y = 0.0;
      if (n == 0) return;

      y = x;
      dy = 1.0;
      d2y = 0.0;
      if (n == 1) return;

      yp = 1.0;
      dyp = 0.0;
      d2yp = 0.0;
      for (int i = 2; i <= n; ++i) 
      {
        c1 = (double) i;
        c2 = c1 * 2.0 - 1.0;
        c4 = c1 - 1.0;
        ym = y;
        y = (c2 * x * y - c4 * yp) / c1;
        yp = ym;
        dym = dy;
        dy = (c2 * x * dy - c4 * dyp + c2 * yp) / c1;
        dyp = dym;
        d2ym = d2y;
        d2y = (c2 * x * d2y - c4 * d2yp + c2 * 2.0 * dyp) / c1;
        d2yp = d2ym;
      }
    }


    //-----------------------------------------------------------------------------//
    //
    // double LegendreP (int l, int m, double x);
    //
    // Computes the value of the associated Legendre polynomial P_lm (x) 
    // of order l at a given point.
    //
    // Input:
    //   l  = degree of the polynomial  >= 0
    //   m  = parameter satisfying 0 <= m <= l, 
    //   x  = point in which the computation is performed, range -1 <= x <= 1.
    // Returns:
    //   value of the polynomial in x
    //
    //-----------------------------------------------------------------------------//

    public static double LegendreP (int l, int m, double x)
    {
      // check parameters
      if (m < 0 || m > l || Math.Abs(x) > 1.0) 
        throw new ArgumentException("bad argument");

      double pmm = 1.0;
      if (m > 0) 
      {
        double h = Math.Sqrt((1.0-x)*(1.0+x)),
          f = 1.0;
        for (int i = 1; i <= m; i++) 
        {
          pmm *= -f * h;
          f += 2.0; 
        }
      }
      if (l == m)
        return pmm;
      else 
      {
        double pmmp1 = x * (2 * m + 1) * pmm;
        if (l == (m+1))
          return pmmp1;
        else 
        {
          double pll = 0.0;
          for (int ll = m+2; ll <= l; ll++) 
          {
            pll = (x * (2 * ll - 1) * pmmp1 - (ll + m - 1) * pmm) / (ll - m);
            pmm = pmmp1;
            pmmp1 = pll;
          }
          return pll;
        }
      }
    }
    #endregion

    #region SphericalHarmonicY


    static bool odd(int x)
    {
      return (x&1)!=0;
    }
    const double M_4PI = 12.56637061435917295385057;

    /// <summary>
    /// Computes the spherical harmonics Y_lm(theta,phi) with l and m 
    /// integers satisfying -l &lt;= m &lt;= l and arbitrary angles theta and phi. 
    /// </summary>
    /// <param name="l">First integer.</param>
    /// <param name="m">Second integer, must be in the range -l &lt;= m &lt;= l.</param>
    /// <param name="theta">First angle.</param>
    /// <param name="phi">Second angle.</param>
    /// <returns>The spherical harmonics Y_lm(theta,phi).</returns>
    public static Complex SphericalHarmonicY (int l, int m, double theta, double phi)
    {
      Complex e; 
      double p,f,smphi;
      int sm;
    
      // save original m with sign
      sm = m;

      // make m positive
      if (m < 0) m = -m;  

      // check parameters
      if (m > l) 
        throw new ArgumentException("m out of range -l <= m <= l");
        
      // normalization factor
      f = Math.Sqrt( (2*l+1)/M_4PI * GammaRelated.Fac(l-m) / GammaRelated.Fac(l+m) );
    
      // associated Legendre polynomial
      p = LegendreP(l,m,Math.Cos(theta));

      // phase factor with signed m
      smphi = sm*phi;
      e = Complex.FromRealImaginary(Math.Cos(smphi),Math.Sin(smphi));

      // sign convention
      sm = odd( (m+sm)/2 ) ? -1 : 1;

      // total
      return (sm * f * p) * e; 
    }

    #endregion
  }
}
