#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Calculates a smoothing cubic spline, whose smoothness is determined by the property <see cref="Smoothness"/>.
  /// </summary>
  public class SmoothingCubicSpline : SmoothingCubicSplineBase, IInterpolationFunction
  {
    protected double _smoothness = 1;

    /// <summary>
    /// Create an instance of <see cref="SmoothingCubicSpline"/> with a default value for <see cref="Smoothness"/> of 1.
    /// </summary>
    public SmoothingCubicSpline()
    {
      _variance = -1.0; // unknown variance
    }

    protected override void InterpolationKernel(
      double[] x,
      double[] f,
      double[] df,
      int n,
      double[] y,
      double[][] C,
      int ic,
      double var,
      int job,
      double[] se,
      double[][] WK0,
      double[][] WK1,
      double[] WK2,
      double[] WK3,
      out int ier)
    {
      ier = 0;
      double[] stat = new double[6];
      double p, q;
      double avh;
      double avar;
      double avdf;
      double gf1;
      int i;

      double r1 = _smoothness;
      spint1(x, out avh, f, df, out avdf, n, y, C, ic, WK0, WK1, ref ier);

      avar = var;
      if (var > 0)
        avar = var * avdf * avdf;

      /* Calculate spline coefficients */
      spfit1(x, avh, df, n, r1, out p, out q, out gf1, avar, stat, y, C, ic, WK0, WK1, WK2, WK3);
      spcof1(x, avh, f, df, n, p, q, y, C, ic, WK2, WK3);

      /* Optionally calculate standard error estimates */
      if (var < 0)
      {
        avar = stat[5];
        var = avar / (avdf * avdf);
      }
      if (job == 1)
        sperr1(x, avh, df, n, WK0, p, avar, se);

      /* Unscale df */
      for (i = 0; i < n; i++) df[i] = df[i] * avdf;

      /* Put statistics in wk */
      WK0[0][0] = stat[0];
      WK0[1][0] = stat[1];
      WK0[2][0] = stat[2];
      WK1[0][0] = stat[3];
      WK1[1][0] = stat[4];
      WK2[0] = stat[5] / (avdf * avdf);
      WK3[0] = avdf * avdf;
    }

    /// <summary>
    /// Get/sets the smoothness parameter. Must be in the interval [0,Infinity], where a
    /// value of 0 means no smoothing (evaluation of a cubic spline), while a value of Infinity
    /// means evaluation of a regression.
    /// </summary>
    /// <remarks>The <see cref="SmoothingCubicSplineBase.SmoothingParameter"/> is calculated by
    /// SmoothingParameter = Smoothness/(1+Smoothness).</remarks>
    public double Smoothness
    {
      get
      {
        return _smoothness;
      }
      set
      {
        if (!(0 <= value))
          throw new ArgumentOutOfRangeException("Must be a value >=0", nameof(value));
        _smoothness = value;
      }
    }
  }
}
