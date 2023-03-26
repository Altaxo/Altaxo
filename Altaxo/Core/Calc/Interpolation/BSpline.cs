#region Copyright

/////////////////////////////////////////////////////////////////////////////
//  Software:     SSJ
//  Copyright(C) 2001  Pierre L'Ecuyer and Universite de Montreal
//  Organization: DIRO, Universite de Montreal
//  @author       
//  @since
// 
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright



using System;
using System.Collections.Generic;
using System.Threading;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Implements a BSpline in one dimension.
  /// </summary>
  /// <remarks>
  /// Translated from Java to C#. Original doc: <see href="http://simul.iro.umontreal.ca/ssj-2/doc/html/umontreal/iro/lecuyer/functionfit/BSpline.html"/>
  /// Original source: <see href="https://github.com/umontreal-simul/ssj/blob/master/src/main/java/umontreal/ssj/functionfit/BSpline.java"/>
  /// </remarks>
  public class BSpline1D : IInterpolationFunction
  {
    private double[] _xOrg;     //x original
    private double[] _yOrg;     //y original

    private int _degree;

    // working variables
    private double[] _myX;
    private double[] _myY;
    private double[] _knots;

    /// <summary>
    /// Temporary array for calculation of <see cref="GetXOfU(double)"/> and <see cref="GetYOfU(double)"/>.
    /// </summary>
    private ThreadLocal<double[,]> _temporaryV = new ThreadLocal<double[,]>();

    /// <summary>
    /// The parent spline (neccessary for getting the u of x when the actual spline is a derivative).
    /// </summary>
    public BSpline1D? ParentSpline { get; init; }

    public BSpline1D(int degree = 3)
    {
      _degree = degree;
    }


    public BSpline1D(double[] x, double[] y, int degree)
    {
      if (x.Length != y.Length)
        throw new ArgumentException("The arrays x and y must have the same length");
      _degree = degree;
      _xOrg = (double[])x.Clone();
      _yOrg = (double[])y.Clone();
      Initialize(x, y, null);
    }

    public BSpline1D(double[] x, double[] y, double[] knots)
    {
      if (x.Length != y.Length)
        throw new ArgumentException("The arrays x and y must share the same length");
      if (!(knots.Length >= x.Length + 2))
        throw new ArgumentException("The number of knots must be at least n+2");

      _xOrg = (double[])x.Clone();
      _yOrg = (double[])y.Clone();
      _knots = (double[])knots.Clone();
      Initialize(x, y, knots);
    }

    public void Interpolate(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
      throw new NotImplementedException();
    }

    private void Initialize(double[] x, double[] y, double[]? initialKnots)
    {
      if (initialKnots is null)
      {
        // Create a uniform knot vector between 0 and 1
        _knots = new double[x.Length + _degree + 1];
        for (int i = _degree; i < _knots.Length - _degree; i++)
        {
          _knots[i] = (double)(i - _degree) / (_knots.Length - (2.0 * _degree) - 1);
        }
        for (int i = _knots.Length - _degree; i < _knots.Length; i++)
        {
          _knots[i] = _knots[i - 1];
        }
        for (int i = _degree; i > 0; i--)
        {
          _knots[i - 1] = _knots[i];
        }

        // create our internal vector of Control Points
        // here, no modifications need to be done on the original x/y values
        _myX = x;
        _myY = y;
      }
      else
      {
        _degree = initialKnots.Length - x.Length - 1;


        // we adapt the knot array to our algorithm
        // the knot array needs to have twice the same value at the beginning and at the end of the array
        // we adapt the size of the array X and Y in consequence in order to continue to respect the condition :
        // x.length + degree + 1 = this.knots.length
        // This modification does not influence the result and allows to close our curve


        //Compute the number of values that need to be added to the start and the end of the arrays
        int nAddToStart = 1;
        int nAddToEnd = initialKnots.Length - 2;
        while (AreEqual(initialKnots[nAddToStart], initialKnots[0], 1e-10))
          nAddToStart++;
        if (nAddToStart <= _degree)
          nAddToStart = _degree - nAddToStart + 1;
        else
          nAddToStart = 0; // then we have iBorneInf values to add at the beginning of the array

        while (AreEqual(initialKnots[nAddToEnd], initialKnots[^1], 1e-10))
          nAddToEnd--;
        if (nAddToEnd >= initialKnots.Length - 1 - _degree)
          nAddToEnd = _degree + 1 - (initialKnots.Length - 1 - nAddToEnd);
        else
          nAddToEnd = 0; // then we have iBorneSup values to add at the end of the array

        //add computed values
        _knots = new double[initialKnots.Length + nAddToStart + nAddToEnd];
        _myX = new double[x.Length + nAddToStart + nAddToEnd];
        _myY = new double[y.Length + nAddToStart + nAddToEnd];
        for (int i = 0; i < nAddToStart; i++)
        {
          _knots[i] = initialKnots[0];
          _myX[i] = x[0];
          _myY[i] = y[0];
        }
        for (int i = 0; i < initialKnots.Length; i++)
        {
          _knots[nAddToStart + i] = initialKnots[i];
        }
        for (int i = 0; i < x.Length; i++)
        {
          _myX[nAddToStart + i] = x[i];
          _myY[nAddToStart + i] = y[i];
        }
        for (int i = 0; i < nAddToEnd; i++)
        {
          _knots[_knots.Length - 1 - i] = initialKnots[^1];
          _myX[_myX.Length - 1 - i] = x[^1];
          _myY[_myY.Length - 1 - i] = y[^1];
        }
      }
    }


    public double[] getX()
    {
      return (double[])_myX.Clone();
    }

    public double[] getY()
    {
      return (double[])_myY.Clone();
    }

    public double getMaxKnot()
    {
      return _knots[^1];
    }

    public double getMinKnot()
    {
      return _knots[0];
    }

    public double[] getKnots()
    {
      return (double[])_knots.Clone();
    }

    public static BSpline1D createInterpBSpline(double[] x, double[] y,
                                             int degree, bool createEquallySpacedKnots = false)
    {
      if (x.Length != y.Length)
        throw new ArgumentException("The arrays x and y must share the same length");
      if (x.Length <= degree)
        throw new ArgumentException("The arrays length must be greater than degree");

      double[] t = new double[x.Length];

      //compute U : clamped knots vector uniformly from 0 to 1
      double[] U = new double[x.Length + degree + 1];
      int m = U.Length - 1;

      if (createEquallySpacedKnots)
      {
        double n1d = x.Length - 1;
        //compute t : parameters vector uniformly from 0 to 1
        t = new double[x.Length];
        for (int i = 0; i < t.Length; i++)
        {
          t[i] = i / n1d;
        }

        for (int i = 0; i <= degree; i++)
          U[i] = 0;
        for (int i = 1; i < x.Length - degree; i++)
          U[i + degree] = (double)i / (x.Length - degree);
        for (int i = U.Length - 1 - degree; i < U.Length; i++)
          U[i] = 1;
      }
      else
      {
        // create knots with the same spacing as x
        var xmin = Math.Min(x[0], x[^1]);
        var xmax = Math.Max(x[0], x[^1]);
        var xspan = xmax - xmin;
        for (int i = 0; i < t.Length; i++)
        {
          t[i] = (x[i] - xmin) / xspan;
        }

        for (int i = 0; i <= degree; i++)
          U[i] = 0;
        for (int i = 1; i < x.Length - degree; i++)
          U[i + degree] = (x[i + 1] - xmin) / xspan;
        for (int i = U.Length - 1 - degree; i < U.Length; i++)
          U[i] = 1;
      }

      //compute matrix N : made of BSpline coefficients
      double[][] N = new double[x.Length][];
      for (int i = 0; i < x.Length; i++)
      {
        N[i] = computeN(U, degree, t[i], x.Length);
      }

      //initialize D : initial points matrix
      var D = new double[x.Length, 2];
      for (int i = 0; i < x.Length; i++)
      {
        D[i, 0] = x[i];
        D[i, 1] = y[i];
      }

      //solve the linear equation system using colt library
      var coltN = Matrix<double>.Build.Dense(x.Length, x.Length, (i, j) => N[i][j]);
      var coltD = Matrix<double>.Build.Dense(x.Length, 2, (i, j) => D[i, j]);
      var coltP = Matrix<double>.Build.Dense(x.Length, 2);
      coltN.Solve(coltD, coltP);

      return new BSpline1D(coltP.Column(0).ToArray(), coltP.Column(1).ToArray(), U);
    }

    public static BSpline1D createApproxBSpline(double[] x, double[] y,
                                            int degree, int hp1, bool createEquallySpacedKnots = false)
    {
      if (x.Length != y.Length)
        throw new ArgumentException("The arrays x and y must share the same length");
      if (x.Length <= degree)
        throw new ArgumentException("The arrays length must be greater than degree");
      if (!(hp1 > degree))
        throw new ArgumentOutOfRangeException(nameof(hp1), "Must be > degree");

      int h = hp1 - 1;
      int n = x.Length - 1;

      double[] t = new double[x.Length];
      if (createEquallySpacedKnots)
      {
        double n1d = x.Length - 1;
        //compute t : parameters vector uniformly from 0 to 1
        t = new double[x.Length];
        for (int i = 0; i < t.Length; i++)
        {
          t[i] = i / n1d;
        }
      }
      else
      {
        // create knots with the same spacing as x
        var xmin = Math.Min(x[0], x[^1]);
        var xmax = Math.Max(x[0], x[^1]);
        var xspan = xmax - xmin;
        for (int i = 0; i < t.Length; i++)
        {
          t[i] = (x[i] - xmin) / xspan;
        }
      }

      //compute U : clamped knots vector uniformly from 0 to 1
      int m = h + degree + 1;
      var U = new double[m + 1];
      for (int i = 0; i <= degree; i++)
        U[i] = 0;
      for (int i = 1; i < hp1 - degree; i++)
        U[i + degree] = (double)i / (hp1 - degree);
      for (int i = m - degree; i <= m; i++)
        U[i] = 1;


      //compute matrix N : composed of BSpline coefficients
      double[][] N = new double[n + 1][];
      for (int i = 0; i < N.Length; i++)
      {
        N[i] = computeN(U, degree, t[i], h + 1);
      }

      //initialize D : initial points matrix
      var D = new double[x.Length, 2];
      for (int i = 0; i < x.Length; i++)
      {
        D[i, 0] = x[i];
        D[i, 1] = y[i];
      }

      //compute Q :
      var tempQ = new double[x.Length, 2];
      for (int k = 1; k < n; k++)
      {
        tempQ[k, 0] = D[k, 0] - N[k][0] * D[0, 0] - N[k][h] * D[x.Length - 1, 0];
        tempQ[k, 1] = D[k, 1] - N[k][0] * D[0, 1] - N[k][h] * D[x.Length - 1, 1];
      }
      var Q = new double[h - 1, 2];
      for (int i = 1; i < h; i++)
      {
        for (int k = 1; k < n; k++)
        {
          Q[i - 1, 0] += N[k][i] * tempQ[k, 0];
          Q[i - 1, 1] += N[k][i] * tempQ[k, 1];
        }
      }

      // compute N matrix for computation:
      var N2 = new double[n - 1, h - 1];
      for (int i = 0; i < n - 1; i++)
      {
        for (int j = 0; j < h - 1; j++)
          N2[i, j] = N[i + 1][j + 1];
      }

      //solve the linear equation system using colt library
      var coltQ = Matrix<double>.Build.Dense(h - 1, 2, (i, j) => Q[i, j]);
      var coltN = Matrix<double>.Build.Dense(n - 1, h - 1, (i, j) => N2[i, j]);
      var coltM = Matrix<double>.Build.Dense(h - 1, h - 1);
      coltN.TransposeThisAndMultiply(coltN, coltM);
      var coltP = Matrix<double>.Build.Dense(h - 1, h - 1);
      coltM.Solve(coltQ, coltP);
      var pxTemp = coltP.Column(0);
      var pyTemp = coltP.Column(1);
      double[] px = new double[hp1];
      double[] py = new double[hp1];
      px[0] = D[0, 0];
      py[0] = D[0, 1];
      px[h] = D[x.Length - 1, 0];
      py[h] = D[x.Length - 1, 1];
      for (int i = 0; i < pxTemp.Count; i++)
      {
        px[i + 1] = pxTemp[i];
        py[i + 1] = pyTemp[i];
      }

      return new BSpline1D(px, py, U);
      // return new BSpline(px, py, degree);
    }



    /// <summary>
    /// Returns the derivative B-spline object of the current variable.
    /// Using this function and the returned object, instead of the
    /// derivative` method, is strongly recommended if one wants to compute
    /// many derivative values.
    /// </summary>
    /// <returns></returns>
    public BSpline1D GetDerivativeBSpline()
    {
      var xTemp = new double[_myX.Length - 1];
      var yTemp = new double[_myY.Length - 1];
      for (int i = 0; i < xTemp.Length; i++)
      {
        xTemp[i] = (_myX[i + 1] - _myX[i]) * _degree / (_knots[i + _degree + 1] - _knots[i + 1]);
        yTemp[i] = (_myY[i + 1] - _myY[i]) * _degree / (_knots[i + _degree + 1] - _knots[i + 1]);
      }

      double[] newKnots = new double[_knots.Length - 2];
      for (int i = 0; i < newKnots.Length; i++)
      {
        newKnots[i] = _knots[i + 1];
      }

      // not optimal sorting at all
      var xTemp2 = new double[this._myX.Length - 1];
      var yTemp2 = new double[this._myY.Length - 1];
      for (int i = 0; i < xTemp.Length; i++)
      {
        int k = 0;
        for (int j = 0; j < xTemp.Length; j++)
        {
          if (xTemp[i] > xTemp[j])
            k++;
        }
        while (xTemp2[k] != 0)
          k++;
        xTemp2[k] = xTemp[i];
        yTemp2[k] = yTemp[i];
      }

      return new BSpline1D(xTemp2, yTemp2, newKnots) { ParentSpline = this };
    }


    /// <summary>
    /// Get the b-spline that represents the ith derivatives of this b-spline.
    /// </summary>
    /// <param name="i">The number of derivative.</param>
    /// <returns>A b-spline that represents the ith derivatives of this b-spline</returns>
    public BSpline1D derivativeBSpline(int i)
    {
      BSpline1D bs = this;
      for (; i > 0; i--)
      {
        bs = bs.GetDerivativeBSpline();
      }
      return bs;
    }

    /// <summary>
    /// Gets the y value, for a given x-value. This function can only be used when the spline was
    /// constructed with points that were sorted by x (ascending or descending).
    /// </summary>
    /// <param name="x">The x.</param>
    /// <returns>The y-value of the given x-value.</returns>
    public double GetYOfX(double x)
    {
      var u = GetUOfX(x);
      if (ParentSpline is { } ps)
      {
        var dx = GetXOfU(u);
        var dy = GetYOfU(u);
        return dy / dx;
      }
      else
      {
        return GetYOfU(u);
      }
    }

    /// <summary>
    /// Gets the u value (0..1), for a given x-value. This function can only be used when the spline was
    /// constructed with points that were sorted by x (ascending or descending).
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <returns>The u value of the given x-value.</returns>
    private double GetUOfX(double x)
    {
      if (ParentSpline is { } ps)
      {
        return ps.GetUOfX(x);
      }
      else
      {

        double u;
        if (Math.Min(_myX[0], _myX[^1]) <= x && x <= Math.Max(_myX[0], _myX[^1]))
        {
          u = Altaxo.Calc.RootFinding.Bisection.FindRoot((t) => GetXOfU(t) - x, 0, 1);
        }
        else
        {
          u = Altaxo.Calc.RootFinding.Bisection.FindRootExpand((t) => GetXOfU(t) - x, -1, 2);
        }

        return u;
      }
    }

    public double GetXOfU(double u)
    {
      int k = GetTimeInterval(_knots, 0, _knots.Length - 1, u);
      k = Math.Max(_degree, Math.Min(k, _myX.Length - 1)); // clamp k to valid indices

      var X = _temporaryV.IsValueCreated ? _temporaryV.Value : (_temporaryV.Value = new double[_degree + 1, _myX.Length]);

      for (int i = k - _degree; i <= k; i++)
        X[0, i] = _myX[i];

      for (int j = 1; j <= _degree; j++)
      {
        for (int i = k - _degree + j; i <= k; i++)
        {
          double aij = (u - this._knots[i]) / (this._knots[i + 1 + _degree - j] - this._knots[i]);
          X[j, i] = (1 - aij) * X[j - 1, i - 1] + aij * X[j - 1, i];
        }
      }
      return X[_degree, k];
    }

    public double GetYOfU(double u)
    {
      int k = GetTimeInterval(_knots, 0, _knots.Length - 1, u);
      k = Math.Max(_degree, Math.Min(k, _myX.Length - 1)); // clamp k to valid indices

      var Y = _temporaryV.IsValueCreated ? _temporaryV.Value : (_temporaryV.Value = new double[_degree + 1, _myX.Length]);

      for (int i = k - _degree; i <= k; i++)
        Y[0, i] = _myY[i];
      for (int j = 1; j <= _degree; j++)
      {
        for (int i = k - _degree + j; i <= k; i++)
        {
          double aij = (u - this._knots[i]) / (this._knots[i + 1 + _degree - j] - this._knots[i]);
          Y[j, i] = (1 - aij) * Y[j - 1, i - 1] + aij * Y[j - 1, i];
        }
      }
      return Y[_degree, k];
    }

    private static bool AreEqual(double a, double b, double tol)
    {
      return Math.Abs(a - b) < tol;
    }

    private static double[] computeN(double[] U, int degree, double u, int np1)
    {
      double[] N = new double[np1];

      // special cases at bounds
      if (AreEqual(u, U[0], 1e-10))
      {
        N[0] = 1.0;
        return N;
      }
      else if (AreEqual(u, U[U.Length - 1], 1e-10))
      {
        N[N.Length - 1] = 1.0;
        return N;
      }

      // find the knot index k such that U[k]<= u < U[k+1]
      int k = GetTimeInterval(U, 0, U.Length - 1, u);

      N[k] = 1.0;
      for (int d = 1; d <= degree; d++)
      {
        N[k - d] = N[k - d + 1] * (U[k + 1] - u) / (U[k + 1] - U[k - d + 1]);
        for (int i = k - d + 1; i <= k - 1; i++)
          N[i] = (u - U[i]) / (U[i + d] - U[i]) * N[i] + ((U[i + d + 1] - u) / (U[i + d + 1] - U[i + 1])) * N[i + 1];
        N[k] = (u - U[k]) / (U[k + d] - U[k]) * N[k];
      }
      return N;
    }



    /**
  * Returns the index of the time interval corresponding to time `t`.
  * Let @f$t_0\le\cdots\le t_n@f$ be simulation times stored in a
  * subset of `times`. This method uses binary search to determine the
  * smallest value @f$i@f$ for which @f$t_i\le t < t_{i+1}@f$, and
  * returns @f$i@f$. The value of @f$t_i@f$ is stored in
  * `times[start+i]` whereas @f$n@f$ is defined as `end - start`. If
  * @f$t<t_0@f$, this returns @f$-1@f$. If @f$t\ge t_n@f$, this returns
  * @f$n@f$. Otherwise, the returned value is greater than or equal to
  * 0, and smaller than or equal to @f$n-1@f$. `start` and `end` are
  * only used to set lower and upper limits of the search in the `times`
  * array; the index space of the returned value always starts at 0.
  * Note that if the elements of `times` with indices `start`, …, `end`
  * are not sorted in non-decreasing order, the behavior of this method
  * is undefined.
  *  @param times        an array of simulation times.
  *  @param start        the first index in the array to consider.
  *  @param end          the last index (inclusive) in the array to
  *                      consider.
  *  @param t            the queried simulation time.
  *  @return the index of the interval.
  *
  *  @exception NullPointerException if `times` is `null`.
  *  @exception IllegalArgumentException if `start` is negative, or if
  * `end` is smaller than `start`.
  *  @exception ArrayIndexOutOfBoundsException if `start + end` is
  * greater than or equal to the length of `times`.
  */
    public static int GetTimeInterval(double[] times, int start, int end, double t)
    {
      if (start < 0)
        throw new ArgumentException("The starting index must not be negative");
      int n = end - start;
      if (n < 0)
        throw new ArgumentException("The ending index must be greater than or equal to the starting index");
      if (t < times[start])
        return -1;
      if (t >= times[end])
        return n;

      int start0 = start;
      // Perform binary search to find the interval index
      int mid = (start + end) / 2;
      // Test if t is inside the interval mid.
      // The interval mid starts at times[mid],
      // and the interval mid+1 starts at times[mid + 1].
      while (t < times[mid] || t >= times[mid + 1])
      {
        if (start == end)
          throw new InvalidOperationException(); // Should not happen, safety check to avoid infinite loops.
        if (t < times[mid])
          end = mid - 1;  // time corresponds to an interval before mid.
        else
          start = mid + 1; // time corresponds to an interval after mid.
        mid = (start + end) / 2;
      }
      return mid - start0;
    }
  }
}
