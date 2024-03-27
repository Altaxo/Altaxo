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

using System.Collections.Generic;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Calculate the Cardinal cubic spline interpolation for the
  /// given abscissa vector x and ordinate vector y.
  /// All vectors must have conformant dimensions.
  /// </summary>
  /// <remarks>
  /// <code>
  ///
  ///                              / -0.5  1.5 -1.5  0.5 \   / P1 \
  ///  CSpline(t) = (t^3 t^2 t 1) |   1.0 -2.5  2.0 -0.5  | |  P2  | = T M G
  ///                             |  -0.5  0.0  0.5  0.0  | |  P3  |
  ///                              \  0.0  1.0  0.0  0.0 /   \ P4 /
  ///
  ///  T is the polynomial basis vector
  ///  M is the basis matrix of the Cardinal spline
  ///  G is the geometry vector of the control points
  ///
  /// </code>
  /// </remarks>
  public class CardinalCubicSpline : CurveBase
  {
    public override void Interpolate(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
      // verify index range
      if (!MatchingIndexRange(x, y))
        throw new System.ArgumentException("index range mismatch of vectors");

      // link original data vectors into base class
      base.x = x;
      base.y = y;
    }

    //----------------------------------------------------------------------------//

    public override double GetXOfU(double t)
    {
      const int lo = 0;
      int hi = x.Count - 1;
      int i = FindInterval(t, x);

      if (i < lo || i >= hi || hi - lo == 1)
      {
        // linear extrapolation and interpolation for 2 points
        return t;
      }
      else if (i == lo)
      {
        i = lo;
        double u = (t - x[i]) / (x[i + 1] - x[i]),
          u2 = u * u,
          c1 = 1.0 + u * (-1.0 + u * (u - 1.0) / 2.0),
          c2 = u * (1.0 + u * (1.0 - u)),
          c3 = u2 * (u - 1.0) / 2.0;
        return c1 * x[i] + c2 * x[i + 1] + c3 * x[i + 2];
      }
      else if (i == hi - 1)
      {
        i = hi - 1;
        double u = (x[i + 1] - t) / (x[i + 1] - x[i]),
          u2 = u * u,
          c1 = 1.0 + u * (-1.0 + u * (u - 1.0) / 2.0),
          c2 = u * (1.0 + u * (1.0 - u)),
          c3 = u2 * (u - 1.0) / 2.0;
        return c1 * x[i + 1] + c2 * x[i] + c3 * x[i - 1];
      }
      else
      {
        double u = (t - x[i]) / (x[i + 1] - x[i]),
          u2 = u * u,
          u3 = 1.0 - u,
          u4 = u3 * u3,
          c1 = -u4 * u / 2.0,
          c2 = 1.0 + u2 * (3.0 * u - 5.0) / 2.0,
          c3 = u * (1.0 + u * (4.0 - 3.0 * u)) / 2.0,
          c4 = -u2 * u3 / 2.0;
        return c1 * x[i - 1] + c2 * x[i] + c3 * x[i + 1] + c4 * x[i + 2];
      }
    }

    public override double GetYOfU(double t)
    {
      const int lo = 0;
      int hi = x.Count - 1;
      int i = FindInterval(t, x);

      if (i < lo || hi - lo == 1)
      {
        // linear extrapolation and interpolation for 2 points
        return y[lo] + (t - x[lo]) * (y[lo + 1] - y[lo]) / (x[lo + 1] - x[lo]);
      }
      else if (i == lo)
      {
        i = lo;
        double u = (t - x[i]) / (x[i + 1] - x[i]),
          u2 = u * u,
          c1 = 1.0 + u * (-1.0 + u * (u - 1.0) / 2.0),
          c2 = u * (1.0 + u * (1.0 - u)),
          c3 = u2 * (u - 1.0) / 2.0;
        return c1 * y[i] + c2 * y[i + 1] + c3 * y[i + 2];
      }
      else if (i >= hi)
      {
        // linear extrapolation
        return y[hi] + (t - x[hi]) * (y[hi] - y[hi - 1]) / (x[hi] - x[hi - 1]);
      }
      else if (i == hi - 1)
      {
        i = hi - 1;
        double u = (x[i + 1] - t) / (x[i + 1] - x[i]),
          u2 = u * u,
          c1 = 1.0 + u * (-1.0 + u * (u - 1.0) / 2.0),
          c2 = u * (1.0 + u * (1.0 - u)),
          c3 = u2 * (u - 1.0) / 2.0;
        return c1 * y[i + 1] + c2 * y[i] + c3 * y[i - 1];
      }
      else
      {
        double u = (t - x[i]) / (x[i + 1] - x[i]),
          u2 = u * u,
          u3 = 1.0 - u,
          u4 = u3 * u3,
          c1 = -u4 * u / 2.0,
          c2 = 1.0 + u2 * (3.0 * u - 5.0) / 2.0,
          c3 = u * (1.0 + u * (4.0 - 3.0 * u)) / 2.0,
          c4 = -u2 * u3 / 2.0;
        return c1 * y[i - 1] + c2 * y[i] + c3 * y[i + 1] + c4 * y[i + 2];
      }
    }

    #region Scene Drawing

#if IncludeScene
  //----------------------------------------------------------------------------//
//
//  compute a cubic Cardinal polynomial for the points
//  (x[0],y[0])...(x[3],y[3]) and plot it from (x[1],y[1]) to (x[2],y[2])
//
//----------------------------------------------------------------------------//

void MpCardinalCubicSpline::Draw4 (Scene& scene,
           const double *x, const double *y, int &first)
{
  double u,u2,u3,u4,c1,c2,c3,c4,rx,ry;
  int n = GetResolution(scene,x[1],y[1],x[2],y[2]);

  for (int i = 0; i <= n; i++) {
    u  = (double) i / n;
    u2 = u*u;
    u3 = 1.0-u;
    u4 = u3*u3;

    c1 = -u4*u/2.0;
    c2 = 1.0+u2*(3.0*u-5.0)/2.0;
    c3 = u*(1.0+u*(4.0-3.0*u))/2.0;
    c4 = -u2*u3/2.0;

    rx = c1*x[0]+c2*x[1]+c3*x[2]+c4*x[3];
    ry = c1*y[0]+c2*y[1]+c3*y[2]+c4*y[3];
    if (first) {
      scene.MoveTo(rx,ry);
      first = false;
    } else
      scene.LineTo(rx,ry);
  }
}

//----------------------------------------------------------------------------//

void MpCardinalCubicSpline::DrawClosedCurve (Scene &scene)
{
  if ( ! scene.IsOpen() )
    Matpack.Error("MpCardinalCubicSpline::DrawClosedCurve: "
      "scene is not open for drawing");

  // get index range
  int lo = x->Lo(),
      hi = x->Hi();

  // nothing to draw - one point
  if ( lo >= hi ) return;

  // special case - two points
  if ( hi == lo+1 ) {
    scene.Line((*x)(lo),(*y)(lo),(*x)(hi),(*y)(hi));
    return;
  }

  // three or more points

  int first = true;

  double xx[6],yy[6];

  for (int i = 0; i <= 2; i++) {
    xx[i] = (*x)[hi-2+i];
    yy[i] = (*y)[hi-2+i];
  }

  for (int i = 3; i <= 5; i++) {
    xx[i] = (*x)[lo+i-3];
    yy[i] = (*y)[lo+i-3];
  }

  for (int i = lo; i <= hi-3; i++)
    Draw4(scene,&(*x)(i),&(*y)(i),first);

  for (int i = 0; i <= 2; i++)
    Draw4(scene,&xx[i],&yy[i],first);

  // flush graphics buffer
  scene.Flush();
}
#endif

    #endregion Scene Drawing
  }
}
