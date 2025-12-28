#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

#region Acknowledgements

// The following code was translated using Matpack sources (http://www.matpack.de) (Author B.Gammel)
// Original MatPack-1.7.3\Source\mpcurvebase.h
//                               mpcurvebase.cc
//                               mpfcspline.h
//                               mpfcspline.cc
//                               mpaspline.h
//                               mpaspline.cc
//                               mpbspline.h
//                               mpbspline.cc
//                               mpcspline.h
//                               mpcspline.cc
//                               mprspline.h
//                               mprspline.cc
//                               mpespline.h
//                               mpespline.cc
//                               mppolyinterpol.h
//                               mppolyinterpol.cc
//                               mpratinterpol.h
//                               mpratinterpol.cc
//                               mpgcvspline.h
//                               mpgcvspline.cc

#endregion Acknowledgements

using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Exponential Splines.
  /// </summary>
  /// <remarks>
  /// <code>
  /// References:
  /// -----------
  /// (1) D.G. Schweikert, "An Interpolation Curve using a Spline in Tension"
  ///     J. Math. Physics, 45, pp 312-317 (1966).
  /// (2) Dr.rer.nat. Helmuth Spaeth,
  ///     "Spline-Algorithmen zur Konstruktion glatter Kurven und Flaechen",
  ///     3. Auflage, R. Oldenburg Verlag, Muenchen, Wien, 1983.
  /// (3) A. K. Cline, Commun. of the ACM, 17, 4, pp 218-223 (Apr 1974).
  /// (4) This algorithm is also implemented in the Unix spline tool by
  ///     James R. Van Zandt (jrv@mitre-bedford), 1985.
  /// </code>
  /// </remarks>

  public class ExponentialSpline : CurveBase, IInterpolationFunction
  {
    protected BoundaryConditions boundary;
    protected double sigma, r1, r2;
    protected Vector<double> y1 = CreateVector.Dense<double>(0);
    protected Vector<double> tmp = CreateVector.Dense<double>(0);

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialSpline"/> class with default boundary conditions and smoothing.
    /// </summary>
    public ExponentialSpline()
    {
      boundary = BoundaryConditions.FiniteDifferences;
      sigma = 1.0;
    }

    /// <inheritdoc/>
    public override void Interpolate(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
      // check input parameters

      if (!MatchingIndexRange(x, y))
        throw new ArgumentException("index range mismatch of vectors");

      // link original data vectors into base class
      base.x = x;
      base.y = y;

      // Empty data vectors - free auxilliary storage
      if (x.Count == 0)
      {
        y1.Clear();
        tmp.Clear();
        return; // ok
      }

      const int lo = 0;
      int hi = x.Count - 1,
        n = x.Count;

      if (y1.Count != n)
      {
        y1 = CreateVector.Dense<double>(n); // spline coefficients
      }

      if (n == 1)
      {
        y1[lo] = 0.0;
        return; // ok
      }
      if (tmp.Count != n)
      {
        tmp = CreateVector.Dense<double>(n);   // temporary
      }

      double slp1 = 0.0, slpn = 0.0;
      double dels, delx1, delx2, delx12, deln, delnm1, delnn, c1, c2, c3,
        diag1, diag2, diagin, dx1, dx2 = 0.0, exps, sigmap, sinhs, sinhin,
        slpp1, slppn = 0.0, spdiag;

      delx1 = x[lo + 1] - x[lo];
      dx1 = (y[lo + 1] - y[lo]) / delx1;

      slpp1 = dx1;  // to get y1(lo) = 0 in unspecified cases
      if (boundary == BoundaryConditions.Supply1stDerivative)
        slpp1 = slp1;
      else if (n != 2)
      {
        delx2 = x[lo + 2] - x[lo + 1];
        delx12 = x[lo + 2] - x[lo];
        c1 = -(delx12 + delx1) / delx12 / delx1;
        c2 = delx12 / delx1 / delx2;
        c3 = -delx1 / delx12 / delx2;
        slpp1 = c1 * y[lo] + c2 * y[lo + 1] + c3 * y[lo + 2];
      }
      else
      {
        y1[lo] = y1[lo + 1] = 0.0;
      }

      if (boundary == BoundaryConditions.Supply1stDerivative)
        slppn = slpn;
      else if (n != 2)
      {
        deln = x[hi] - x[hi - 1];
        delnm1 = x[hi - 1] - x[hi - 2];
        delnn = x[hi] - x[hi - 2];
        c1 = (delnn + deln) / delnn / deln;
        c2 = -delnn / deln / delnm1;
        c3 = deln / delnn / delnm1;
        slppn = c3 * y[hi - 2] + c2 * y[hi - 1] + c1 * y[hi];
      }
      else
      {
        y1[lo] = y1[lo + 1] = 0.0;
      }

      // denormalize tension factor
      sigmap = Math.Abs(sigma) * (n - 1) / (x[hi] - x[lo]);

      // set up right hand side and tridiagonal system for y1 and perform forward
      // elimination
      dels = sigmap * delx1;
      exps = Math.Exp(dels);
      sinhs = 0.5 * (exps - 1.0 / exps);
      sinhin = 1.0 / (delx1 * sinhs);
      diag1 = sinhin * (dels * 0.5 * (exps + 1.0 / exps) - sinhs);
      diagin = 1.0 / diag1;
      y1[lo] = diagin * (dx1 - slpp1);
      spdiag = sinhin * (sinhs - dels);
      tmp[lo] = diagin * spdiag;
      if (n != 2)
      {
        for (int i = lo + 1; i <= hi - 1; i++)
        {
          delx2 = x[i + 1] - x[i];
          dx2 = (y[i + 1] - y[i]) / delx2;
          dels = sigmap * delx2;
          exps = Math.Exp(dels);
          sinhs = 0.5 * (exps - 1.0 / exps);
          sinhin = 1.0 / (delx2 * sinhs);
          diag2 = sinhin * (dels * (0.5 * (exps + 1.0 / exps)) - sinhs);
          diagin = 1.0 / (diag1 + diag2 - spdiag * tmp[i - 1]);
          y1[i] = diagin * (dx2 - dx1 - spdiag * y1[i - 1]);
          spdiag = sinhin * (sinhs - dels);
          tmp[i] = diagin * spdiag;
          dx1 = dx2;
          diag1 = diag2;
        }
      }

      diagin = 1.0 / (diag1 - spdiag * tmp[hi - 1]);

      // the expression below does not make sense if n == 2
      if (n != 2)
        y1[hi] = diagin * (slppn - dx2 - spdiag * y1[hi - 1]);

      // perform back substitution
      for (int i = hi - 1; i >= lo; i--)
        y1[i] -= tmp[i] * y1[i + 1];

    }

    /// <inheritdoc/>
    public override double GetXOfU(double u)
    {
      return u;
    }

    /// <inheritdoc/>
    public double GetYOfX(double x)
    {
      return GetYOfU(x);
    }

    /// <inheritdoc/>
    public override double GetYOfU(double u)
    {
      const int lo = 0;
      int hi = x.Count - 1,
        n = x.Count,
        i = lo + 1;

      // special cases
      if (n == 1)
        return y[lo];

      // search for x(i-1) <= u < x(i), lo < i <= hi
      while (i < hi && u >= x[i])
        i++;
      while (i > lo + 1 && x[i - 1] > u)
        i--;

      double sigmap = Math.Abs(sigma) * (n - 1) / (x[hi] - x[lo]),
        del1 = u - x[i - 1],
        del2 = x[i] - u,
        dels = x[i] - x[i - 1],
        exps1 = Math.Exp(sigmap * del1),
        sinhd1 = 0.5 * (exps1 - 1.0 / exps1),
        exps = Math.Exp(sigmap * del2),
        sinhd2 = 0.5 * (exps - 1.0 / exps),
        exps2 = exps1 * exps,
        sinhs = 0.5 * (exps2 - 1.0 / exps2);

      return (y1[i] * sinhd1 + y1[i - 1] * sinhd2) / sinhs +
        ((y[i] - y1[i]) * del1 + (y[i - 1] - y1[i - 1]) * del2) / dels;
    }

    /// <summary>
    /// Gets or sets the smoothing parameter that controls the spline tension.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the provided value is not greater than zero.</exception>
    public double Smoothing
    {
      get
      {
        return sigma;
      }
      set
      {
        if (value > 0.0)
          sigma = value;
        else
          throw new ArgumentException("smoothing parameter must be greater than 0.0");
      }
    }

    /// <summary>
    /// Gets or sets the boundary condition mode used for spline interpolation.
    /// </summary>
    public BoundaryConditions BoundaryCondition
    {
      get { return boundary; }
      set { SetBoundaryConditions(value, 0, 0); }
    }

    /// <summary>
    /// Sets the boundary conditions and any supplied endpoint derivatives.
    /// </summary>
    /// <param name="bnd">Type of boundary condition to apply.</param>
    /// <param name="b1">First derivative at the lower boundary when required.</param>
    /// <param name="b2">First derivative at the upper boundary when required.</param>
    /// <exception cref="ArgumentException">Thrown when an unsupported boundary condition is specified.</exception>
    public void SetBoundaryConditions(BoundaryConditions bnd, double b1, double b2)
    {
      // check boundary conditions argument
      if (bnd == BoundaryConditions.Supply1stDerivative || bnd == BoundaryConditions.FiniteDifferences)
      {
        boundary = bnd;
        r1 = b1;
        r2 = b2;
      }
      else
        throw new ArgumentException("Only Supply1stDerivative or FiniteDifferences boundary conditions");
    }

    /// <summary>
    /// Gets the current boundary condition configuration along with endpoint derivatives.
    /// </summary>
    /// <param name="b1">Outputs the lower boundary derivative.</param>
    /// <param name="b2">Outputs the upper boundary derivative.</param>
    /// <returns>The active boundary condition.</returns>
    public BoundaryConditions GetBoundaryConditions(out double b1, out double b2)
    {
      b1 = r1;
      b2 = r2;
      return boundary;
    }
  }
}
