#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.Threading;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Bivariate Akima interpolation for gridded data.
  /// </summary>
  /// <remarks>
  /// References:
  /// <para>[1] H. Akima, „A method of bivariate interpolation and smooth surface fitting based on local procedures“, Commun. ACM, Bd. 17, Nr. 1, S. 18–20, Jan. 1974, doi: 10.1145/360767.360779.</para>
  /// <para>[2] H. Akima, „Algorithm 474: Bivariate interpolation and smooth surface fitting based on local procedures [E2]“, Commun. ACM, Bd. 17, Nr. 1, S. 26–31, Jan. 1974, doi: 10.1145/360767.360797.</para>
  /// <para>The FORTRAN sources were translated to C# by D.Lellinger in 2005, and rewritten eliminating the goto statements in 2026.</para>
  /// </remarks>
  public class BivariateAkimaSpline : IBivariateInterpolationFunctionWithDerivatives
  {
    /// <summary>
    /// Gets the x-coordinates of the grid points.
    /// </summary>
    /// <remarks>
    /// Values are expected to be strictly ascending.
    /// </remarks>
    private readonly IReadOnlyList<double> _x;

    /// <summary>
    /// Gets the y-coordinates of the grid points.
    /// </summary>
    /// <remarks>
    /// Values are expected to be strictly ascending.
    /// </remarks>
    private readonly IReadOnlyList<double> _y;

    /// <summary>
    /// Gets the gridded function values <c>z(x_i, y_j)</c>.
    /// </summary>
    private readonly IROMatrix<double> _z;

    /// <summary>
    /// Optional cache of precomputed bicubic patches for all interior cells.
    /// </summary>
    /// <remarks>
    /// The array is indexed by cell (not by node): <c>[ix-2, iy-2]</c>, where <c>ix</c> and <c>iy</c> are the
    /// FORTRAN-style interval indices returned by <see cref="LocateIntervalIndex"/>.
    /// </remarks>
    private readonly BicubicPatch[,]? _precomputedPatches;

    /// <summary>
    /// Constructs an Akima bivariate interpolator.
    /// </summary>
    /// <param name="x">X coordinates of grid points (ascending).</param>
    /// <param name="y">Y coordinates of grid points (ascending).</param>
    /// <param name="z">Z values on the (x,y) grid with dimensions (x.Count, y.Count).</param>
    public BivariateAkimaSpline(IReadOnlyList<double> x, IReadOnlyList<double> y, IROMatrix<double> z)
      : this(x, y, z, precomputePatches: false, copyArraysLocally: true)
    {
    }

    /// <summary>
    /// Constructs an Akima bivariate interpolator.
    /// </summary>
    /// <param name="x">X coordinates of grid points (ascending).</param>
    /// <param name="y">Y coordinates of grid points (ascending).</param>
    /// <param name="z">Z values on the (x,y) grid with dimensions (x.Count, y.Count).</param>
    /// <param name="precomputePatches">If <see langword="true"/>, precomputes the bicubic patch for every interior grid cell.</param>
    public BivariateAkimaSpline(IReadOnlyList<double> x, IReadOnlyList<double> y, IROMatrix<double> z, bool precomputePatches)
      : this(x, y, z, precomputePatches: precomputePatches, copyArraysLocally: true)
    {
    }

    /// <summary>
    /// Constructs an Akima bivariate interpolator.
    /// </summary>
    /// <param name="x">X coordinates of grid points (ascending).</param>
    /// <param name="y">Y coordinates of grid points (ascending).</param>
    /// <param name="z">Z values on the (x,y) grid with dimensions (x.Count, y.Count).</param>
    /// <param name="precomputePatches">If <see langword="true"/>, precomputes the bicubic patch for every interior grid cell.</param>
    /// <param name="copyArraysLocally">If <see langword="true"/>, creates local copies of the input arrays. If <see langword="false"/>, the input arrays are used directly without copying, so make sure that they don't change during usage of this instance.</param>
    public BivariateAkimaSpline(IReadOnlyList<double> x, IReadOnlyList<double> y, IROMatrix<double> z, bool precomputePatches, bool copyArraysLocally)
    {
      ArgumentNullException.ThrowIfNull(x);
      ArgumentNullException.ThrowIfNull(y);
      ArgumentNullException.ThrowIfNull(z);

      if (x.Count < 2) // LX <= 1
        throw new ArgumentException("Length is less than 2.", nameof(x));
      if (y.Count < 2) // LY <= 1
        throw new ArgumentException("Length is less than 2.", nameof(y));
      if (z.RowCount != x.Count)
        throw new ArgumentException("Number of rows in z must match the length of x.", nameof(z));
      if (z.ColumnCount != y.Count)
        throw new ArgumentException("Number of columns in z must match the length of y.", nameof(z));

      if (copyArraysLocally)
      {
        _x = x.ToArray();
        _y = y.ToArray();
        _z = CreateMatrix.DenseOfMatrix(z); // local copy of z-grid
      }
      else
      {
        _x = x;
        _y = y;
        _z = z;
      }

      EnsureStrictAscending(_x, $"{GetType()}: Identical x-values", $"{GetType()}: Non-ascending x-values");
      EnsureStrictAscending(_y, $"{GetType()}: Identical y-values", $"{GetType()}: Non-ascending y-values");

      if (precomputePatches) // caller requested a fast-path cache for repeated evaluations
      {
        _precomputedPatches = PrecomputePatches();
      }
    }

    /// <summary>
    /// Interpolates the z value for a single point defined by the provided x and y coordinates.
    /// </summary>
    public double GetValueOfXY(double x, double y)
    {
      int lx0 = _x.Count; // number of grid points in X (LX)
      int ly0 = _y.Count; // number of grid points in Y (LY)

      var ix = LocateIntervalIndex(_x, x); // rectangle x-interval index (FORTRAN-style)
      var iy = LocateIntervalIndex(_y, y); // rectangle y-interval index (FORTRAN-style)

      // Fast path: for interior points we can directly use the precomputed patch for the cell.
      // (Extrapolation for points outside the grid still uses the original on-demand computation.)
      if (_precomputedPatches is not null && ix >= 2 && ix <= lx0 && iy >= 2 && iy <= ly0) // point in an interior cell
      {
        var x3 = _x[ix - 2];
        var y3 = _y[iy - 2];
        return _precomputedPatches[ix - 2, iy - 2].Evaluate(x - x3, y - y3);
      }
      else if (_threadLocalCachedPatch.Value is { } localPatch && localPatch.cachedIx == ix && localPatch.cachedIy == iy) // point in cached rectangle
      {
        return localPatch.cachedPatch.Evaluate(x - localPatch.cachedX3, y - localPatch.cachedY3);
      }
      else
      {
        var (cachedPatch, cachedX3, cachedY3) = ComputePatch(ix, iy);
        return cachedPatch.Evaluate(x - cachedX3, y - cachedY3);
      }
    }


    /// <summary>
    /// Interpolates a single point and also returns the first partial derivatives.
    /// </summary>
    /// <param name="x">The x-coordinate of the desired point.</param>
    /// <param name="y">The y-coordinate of the desired point.</param>
    /// <returns>The interpolated z value together with the partial derivatives w.r.t. x and y at (<paramref name="x"/>, <paramref name="y"/>).</returns>
    public (double z, double dzdx, double dzdy) GetValueAndDerivativesOfXY(double x, double y)
    {
      int lx0 = _x.Count; // number of grid points in X (LX)
      int ly0 = _y.Count; // number of grid points in Y (LY)

      var ix = LocateIntervalIndex(_x, x); // rectangle x-interval index (FORTRAN-style)
      var iy = LocateIntervalIndex(_y, y); // rectangle y-interval index (FORTRAN-style)

      if (_precomputedPatches is not null && ix >= 2 && ix <= lx0 && iy >= 2 && iy <= ly0) // point in an interior cell
      {
        var x3 = _x[ix - 2];
        var y3 = _y[iy - 2];
        var patch = _precomputedPatches[ix - 2, iy - 2];
        return patch.EvaluateValueAndDerivatives(x - x3, y - y3);
      }
      else if (_threadLocalCachedPatch.Value is { } localPatch && localPatch.cachedIx == ix && localPatch.cachedIy == iy) // point in cached rectangle
      {
        return localPatch.cachedPatch.EvaluateValueAndDerivatives(x - localPatch.cachedX3, y - localPatch.cachedY3);
      }
      else // compute patch on demand (needed for extrapolation and for non-precomputed instances)
      {
        var (patch, x3, y3) = ComputePatch(ix, iy);
        _threadLocalCachedPatch.Value = new SingleCachedPatch { cachedIx = ix, cachedIy = iy, cachedX3 = x3, cachedY3 = y3, cachedPatch = patch }; // cache the computed patch for potential reuse in subsequent calls with the same cell (e.g., for multiple points in the same cell)
        return patch.EvaluateValueAndDerivatives(x - x3, y - y3);
      }
    }

    /// <summary>
    /// Interpolates a list of points (<paramref name="u"/>, <paramref name="v"/>) and stores results into <paramref name="w"/>.
    /// </summary>
    public static void Interpolate(IReadOnlyList<double> x, IReadOnlyList<double> y, IROMatrix<double> z, IReadOnlyList<double> u, IReadOnlyList<double> v, IVector<double> w)
    {
      CheckUVZ(u, v, w);
      var spline = new BivariateAkimaSpline(x, y, z, precomputePatches: false, copyArraysLocally: false);
      spline.InterpolateCore(u, v, w);
    }



    /// <summary>
    /// Interpolates a list of points (<paramref name="u"/>, <paramref name="v"/>) and stores results into <paramref name="w"/>.
    /// Also computes the first partial derivatives which are stored in <paramref name="dwdx"/> and <paramref name="dwdy"/>.
    /// </summary>
    /// <param name="x">X coordinates of grid points (ascending).</param>
    /// <param name="y">Y coordinates of grid points (ascending).</param>
    /// <param name="z">Z values on the (x,y) grid with dimensions (x.Count, y.Count).</param>
    /// <param name="u">X-coordinates of desired points.</param>
    /// <param name="v">Y-coordinates of desired points.</param>
    /// <param name="w">Receives interpolated z values.</param>
    /// <param name="dwdx">Receives the partial derivative <c>∂z/∂x</c>.</param>
    /// <param name="dwdy">Receives the partial derivative <c>∂z/∂y</c>.</param>
    public static void Interpolate(IReadOnlyList<double> x, IReadOnlyList<double> y, IROMatrix<double> z, IReadOnlyList<double> u, IReadOnlyList<double> v, IVector<double> w, IVector<double> dwdx, IVector<double> dwdy)
    {
      CheckUVWDWDXDWDY(u, v, w, dwdx, dwdy);
      var spline = new BivariateAkimaSpline(x, y, z, precomputePatches: false, copyArraysLocally: false); // create interpolator (fast patch cache)
      spline.InterpolateWithDerivativesCore(u, v, w, dwdx, dwdy);
    }

    /// <summary>
    /// Interpolates a list of points (<paramref name="u"/>, <paramref name="v"/>) and stores results into <paramref name="w"/>.
    /// Also computes the first partial derivatives which are stored in <paramref name="dwdx"/> and <paramref name="dwdy"/>.
    /// </summary>
    /// <param name="x">X coordinates of grid points (ascending).</param>
    /// <param name="y">Y coordinates of grid points (ascending).</param>
    /// <param name="z">Z values on the (x,y) grid with dimensions (x.Count, y.Count) in a column major array.</param>
    /// <param name="u">X-coordinates of desired points.</param>
    /// <param name="v">Y-coordinates of desired points.</param>
    /// <param name="w">Receives interpolated z values.</param>
    public static void Interpolate(double[] x, double[] y, double[] z, double[] u, double[] v, double[] w)
    {
      CheckUVZ(u, v, w.ToVector());
      var zMatrix = MatrixMath.ToROMatrixFromColumnMajorLinearArray(z, x.Length); // view z[] as (LX,LY) matrix in column-major order
      var spline = new BivariateAkimaSpline(x, y, zMatrix, precomputePatches: false, copyArraysLocally: false); // create interpolator
      spline.InterpolateCore(VectorMath.ToROVector(u), VectorMath.ToROVector(v), VectorMath.ToVector(w));
    }

    /// <summary>
    /// Interpolates a list of points (<paramref name="u"/>, <paramref name="v"/>) and stores results into <paramref name="w"/>.
    /// Also computes the first partial derivatives which are stored in <paramref name="dwdx"/> and <paramref name="dwdy"/>.
    /// </summary>
    /// <param name="x">X coordinates of grid points (ascending).</param>
    /// <param name="y">Y coordinates of grid points (ascending).</param>
    /// <param name="z">Z values on the (x,y) grid with dimensions (x.Count, y.Count).</param>
    /// <param name="u">X-coordinates of desired points.</param>
    /// <param name="v">Y-coordinates of desired points.</param>
    /// <param name="w">Receives interpolated z values.</param>
    /// <param name="dwdx">Receives the partial derivative <c>∂z/∂x</c>.</param>
    /// <param name="dwdy">Receives the partial derivative <c>∂z/∂y</c>.</param>
    public static void Interpolate(double[] x, double[] y, double[] z, double[] u, double[] v, double[] w, double[] dwdx, double[] dwdy)
    {
      CheckUVWDWDXDWDY(u, v, w.ToVector(), dwdx.ToVector(), dwdy.ToVector());

      var zMatrix = MatrixMath.ToROMatrixFromColumnMajorLinearArray(z, x.Length); // view z[] as (LX,LY) matrix in column-major order
      var spline = new BivariateAkimaSpline(x, y, zMatrix, precomputePatches: true, copyArraysLocally: false); // create interpolator (fast patch cache)
      spline.InterpolateWithDerivativesCore(VectorMath.ToROVector(u), VectorMath.ToROVector(v), VectorMath.ToVector(w), VectorMath.ToVector(dwdx), VectorMath.ToVector(dwdy));
    }

    /// <summary>
    /// Represents a bicubic polynomial patch for one grid cell.
    /// </summary>
    /// <remarks>
    /// The patch is expressed in local coordinates <c>dx = x - x3</c> and <c>dy = y - y3</c>, where <c>(x3,y3)</c>
    /// is the lower-left corner of the cell. Only the polynomial coefficients are stored.
    /// </remarks>
    private readonly struct BicubicPatch
    {
      /// <summary>Coefficient p00.</summary>
      public readonly double P00; // bicubic coefficient p00
      /// <summary>Coefficient p01.</summary>
      public readonly double P01; // bicubic coefficient p01
      /// <summary>Coefficient p02.</summary>
      public readonly double P02; // bicubic coefficient p02
      /// <summary>Coefficient p03.</summary>
      public readonly double P03; // bicubic coefficient p03

      /// <summary>Coefficient p10.</summary>
      public readonly double P10; // bicubic coefficient p10
      /// <summary>Coefficient p11.</summary>
      public readonly double P11; // bicubic coefficient p11
      /// <summary>Coefficient p12.</summary>
      public readonly double P12; // bicubic coefficient p12
      /// <summary>Coefficient p13.</summary>
      public readonly double P13; // bicubic coefficient p13

      /// <summary>Coefficient p20.</summary>
      public readonly double P20; // bicubic coefficient p20
      /// <summary>Coefficient p21.</summary>
      public readonly double P21; // bicubic coefficient p21
      /// <summary>Coefficient p22.</summary>
      public readonly double P22; // bicubic coefficient p22
      /// <summary>Coefficient p23.</summary>
      public readonly double P23; // bicubic coefficient p23

      /// <summary>Coefficient p30.</summary>
      public readonly double P30; // bicubic coefficient p30
      /// <summary>Coefficient p31.</summary>
      public readonly double P31; // bicubic coefficient p31
      /// <summary>Coefficient p32.</summary>
      public readonly double P32; // bicubic coefficient p32
      /// <summary>Coefficient p33.</summary>
      public readonly double P33; // bicubic coefficient p33

      /// <summary>
      /// Initializes a new instance of the BicubicPatch class using the specified coefficients for a bicubic polynomial
      /// surface patch.
      /// </summary>
      /// <remarks>The coefficients define the shape of the bicubic surface patch over the unit square. Each
      /// coefficient corresponds to a specific term in the bicubic polynomial, allowing precise control over the
      /// surface's curvature and behavior.</remarks>
      /// <param name="p00">The coefficient for the constant term of the bicubic polynomial.</param>
      /// <param name="p01">The coefficient for the y term at the origin.</param>
      /// <param name="p02">The coefficient for the y squared term at the origin.</param>
      /// <param name="p03">The coefficient for the y cubed term at the origin.</param>
      /// <param name="p10">The coefficient for the x term at the origin.</param>
      /// <param name="p11">The coefficient for the x and y mixed term at the origin.</param>
      /// <param name="p12">The coefficient for the x and y squared term at the origin.</param>
      /// <param name="p13">The coefficient for the x and y cubed term at the origin.</param>
      /// <param name="p20">The coefficient for the x squared term at the origin.</param>
      /// <param name="p21">The coefficient for the x squared and y mixed term at the origin.</param>
      /// <param name="p22">The coefficient for the x squared and y squared mixed term at the origin.</param>
      /// <param name="p23">The coefficient for the x squared and y cubed term at the origin.</param>
      /// <param name="p30">The coefficient for the x cubed term at the origin.</param>
      /// <param name="p31">The coefficient for the x cubed and y mixed term at the origin.</param>
      /// <param name="p32">The coefficient for the x cubed and y squared term at the origin.</param>
      /// <param name="p33">The coefficient for the x cubed and y cubed mixed term at the origin.</param>
      public BicubicPatch(
        double p00, // coefficient p00
        double p01, // coefficient p01
        double p02, // coefficient p02
        double p03, // coefficient p03
        double p10, // coefficient p10
        double p11, // coefficient p11
        double p12, // coefficient p12
        double p13, // coefficient p13
        double p20, // coefficient p20
        double p21, // coefficient p21
        double p22, // coefficient p22
        double p23, // coefficient p23
        double p30, // coefficient p30
        double p31, // coefficient p31
        double p32, // coefficient p32
        double p33) // coefficient p33
      {
        // Store bicubic polynomial coefficients.
        P00 = p00;
        P01 = p01;
        P02 = p02;
        P03 = p03;
        P10 = p10;
        P11 = p11;
        P12 = p12;
        P13 = p13;
        P20 = p20;
        P21 = p21;
        P22 = p22;
        P23 = p23;
        P30 = p30;
        P31 = p31;
        P32 = p32;
        P33 = p33;
      }

      /// <summary>
      /// Evaluates the bicubic polynomial at the given local coordinates.
      /// </summary>
      /// <param name="dx">Local x coordinate <c>x - x3</c>.</param>
      /// <param name="dy">Local y coordinate <c>y - y3</c>.</param>
      /// <returns>The interpolated z value.</returns>
      public double Evaluate(double dx, double dy)
      {
        var q0 = P00 + dy * (P01 + dy * (P02 + dy * P03));
        var q1 = P10 + dy * (P11 + dy * (P12 + dy * P13));
        var q2 = P20 + dy * (P21 + dy * (P22 + dy * P23));
        var q3 = P30 + dy * (P31 + dy * (P32 + dy * P33));
        return q0 + dx * (q1 + dx * (q2 + dx * q3));
      }

      /// <summary>
      /// Evaluates the bicubic polynomial and its first derivatives at the given local coordinates.
      /// </summary>
      /// <param name="dx">Local x coordinate <c>x - x3</c>.</param>
      /// <param name="dy">Local y coordinate <c>y - y3</c>.</param>
      /// <returns>
      /// A tuple containing the interpolated value (<c>Z</c>) and the first partial derivatives
      /// (<c>DzDx</c>, <c>DzDy</c>).
      /// </returns>
      public (double z, double dzdx, double dzdy) EvaluateValueAndDerivatives(double dx, double dy)
      {
        var q0 = P00 + dy * (P01 + dy * (P02 + dy * P03)); // coefficient of dx^0 evaluated at dy
        var q1 = P10 + dy * (P11 + dy * (P12 + dy * P13)); // coefficient of dx^1 evaluated at dy
        var q2 = P20 + dy * (P21 + dy * (P22 + dy * P23)); // coefficient of dx^2 evaluated at dy
        var q3 = P30 + dy * (P31 + dy * (P32 + dy * P33)); // coefficient of dx^3 evaluated at dy

        var z = q0 + dx * (q1 + dx * (q2 + dx * q3)); // interpolated z value

        var q0y = P01 + dy * (2 * P02 + dy * (3 * P03)); // d/dy of q0
        var q1y = P11 + dy * (2 * P12 + dy * (3 * P13)); // d/dy of q1
        var q2y = P21 + dy * (2 * P22 + dy * (3 * P23)); // d/dy of q2
        var q3y = P31 + dy * (2 * P32 + dy * (3 * P33)); // d/dy of q3

        var dzdx = q1 + dx * (2 * q2 + dx * (3 * q3)); // ∂v/∂x
        var dzdy = q0y + dx * (q1y + dx * (q2y + dx * q3y)); // ∂v/∂y

        return (z, dzdx, dzdy);
      }
    }

    private static void CheckUVZ(IReadOnlyList<double> u, IReadOnlyList<double> v, IVector<double> z)
    {
      ArgumentNullException.ThrowIfNull(u);
      ArgumentNullException.ThrowIfNull(v);
      ArgumentNullException.ThrowIfNull(z);
      if (u.Count != v.Count || u.Count != z.Count) // input/output vectors must match in length
        throw new ArgumentException("Input and output vectors must have the same length");
      if (u.Count < 1) // N <= 0
        throw new ArgumentException("Input array is empty!", nameof(u));
    }

    private static void CheckUVWDWDXDWDY(IReadOnlyList<double> u, IReadOnlyList<double> v, IVector<double> z, IVector<double> dzdx, IVector<double> dzdy)
    {
      CheckUVZ(u, v, z);
      ArgumentNullException.ThrowIfNull(dzdx);
      ArgumentNullException.ThrowIfNull(dzdy);
      if (u.Count != dzdx.Count)
        throw new ArgumentException("Input and output vectors must have the same length", nameof(dzdx));
      if (u.Count != dzdy.Count)
        throw new ArgumentException("Input and output vectors must have the same length", nameof(dzdy));
    }

    /// <summary>
    /// Ensures that a coordinate vector is strictly ascending.
    /// </summary>
    /// <param name="values">The coordinate vector to validate.</param>
    /// <param name="identicalMessage">Error message to use if duplicate values are found.</param>
    /// <param name="outOfSequenceMessage">Error message to use if the sequence is not strictly ascending.</param>
    private static void EnsureStrictAscending(IReadOnlyList<double> values, string identicalMessage, string outOfSequenceMessage)
    {
      for (int i = 1; i < values.Count; i++) // coordinate index
      {
        var d = values[i] - values[i - 1]; // forward difference between successive coordinates
        if (d == 0) // duplicate coordinate
          throw new ArgumentException($"{identicalMessage} (at index {i} and {i - 1})");
        if (d < 0) // not strictly ascending
          throw new ArgumentException($"{outOfSequenceMessage} (at index {i} and {i - 1})");
      }
    }

    private struct SingleCachedPatch()
    {/*
      public SingleCachedPatch(int cachedIx, int cachedIy, double cachedX3, double cachedY3, BicubicPatch cachedPatch)
      {
        this.cachedIx = cachedIx;
        this.cachedIy = cachedIy;
        this.cachedX3 = cachedX3;
        this.cachedY3 = cachedY3;
        this.cachedPatch = cachedPatch;
      }
      */
      public int cachedIx;
      public int cachedIy;
      public double cachedX3;
      public double cachedY3;
      public BicubicPatch cachedPatch;
    }

    private ThreadLocal<SingleCachedPatch?> _threadLocalCachedPatch = new();

    /// <summary>
    /// Interpolates a set of points (<paramref name="u"/>, <paramref name="v"/>) and stores values and first derivatives
    /// in <paramref name="z"/>, <paramref name="dzdx"/>, and <paramref name="dzdy"/>.
    /// </summary>
    /// <param name="u">X-coordinates of desired points.</param>
    /// <param name="v">Y-coordinates of desired points.</param>
    /// <param name="z">Receives interpolated z values.</param>
    /// <param name="dzdx">Receives the partial derivative <c>∂z/∂x</c>.</param>
    /// <param name="dzdy">Receives the partial derivative <c>∂z/∂y</c>.</param>
    /// <remarks>
    /// If <see cref="_precomputedPatches"/> is available, interior points are evaluated using cached cell patches.
    /// Points outside the grid are handled by computing the patch on demand (which includes the original extrapolation logic).
    /// </remarks>
    private void InterpolateWithDerivativesCore(IReadOnlyList<double> u, IReadOnlyList<double> v, IVector<double> z, IVector<double> dzdx, IVector<double> dzdy)
    {
      int lx0 = _x.Count; // number of grid points in X (LX)
      int ly0 = _y.Count; // number of grid points in Y (LY)

      // Cache the last on-demand patch for extrapolation or when precomputation is disabled.
      bool hasCachedPatch = false;
      int cachedIx = 0;
      int cachedIy = 0;
      double cachedX3 = 0;
      double cachedY3 = 0;
      BicubicPatch cachedPatch = default;

      for (int k = 0; k < z.Count; k++) // query point index
      {
        var uk = u[k]; // current query x-coordinate
        var vk = v[k]; // current query y-coordinate

        var ix = LocateIntervalIndex(_x, uk); // rectangle x-interval index (FORTRAN-style)
        var iy = LocateIntervalIndex(_y, vk); // rectangle y-interval index (FORTRAN-style)

        if (_precomputedPatches is not null && ix >= 2 && ix <= lx0 && iy >= 2 && iy <= ly0) // point in an interior cell
        {
          var x3 = _x[ix - 2];
          var y3 = _y[iy - 2];
          var patch = _precomputedPatches[ix - 2, iy - 2];
          var r = patch.EvaluateValueAndDerivatives(uk - x3, vk - y3);
          z[k] = r.z;
          dzdx[k] = r.dzdx;
          dzdy[k] = r.dzdy;
          continue;
        }

        if (!hasCachedPatch || cachedIx != ix || cachedIy != iy) // point not in cached rectangle
        {
          (cachedPatch, cachedX3, cachedY3) = ComputePatch(ix, iy);
          cachedIx = ix;
          cachedIy = iy;
          hasCachedPatch = true;
        }

        var rr = cachedPatch.EvaluateValueAndDerivatives(uk - cachedX3, vk - cachedY3);
        z[k] = rr.z;
        dzdx[k] = rr.dzdx;
        dzdy[k] = rr.dzdy;
      }
    }

    /// <summary>
    /// Interpolates a list of points (<paramref name="u"/>, <paramref name="v"/>) and stores results into <paramref name="w"/>.
    /// </summary>
    /// <param name="u">X-coordinates of desired points.</param>
    /// <param name="v">Y-coordinates of desired points.</param>
    /// <param name="w">Receives interpolated z values.</param>
    /// <remarks>
    /// If <see cref="_precomputedPatches"/> is available, interior points are evaluated using cached cell patches.
    /// Points outside the grid are handled by computing the patch on demand (which includes the original extrapolation logic).
    /// </remarks>
    private void InterpolateCore(IReadOnlyList<double> u, IReadOnlyList<double> v, IVector<double> w)
    {
      int lx0 = _x.Count; // number of grid points in X (LX)
      int ly0 = _y.Count; // number of grid points in Y (LY)

      // The Akima algorithm builds a bicubic polynomial patch per grid rectangle.
      // For multiple query points, many are often located in the same rectangle,
      // so cache the last computed patch.
      bool hasCachedPatch = false;
      int cachedIx = 0;
      int cachedIy = 0;
      double cachedX3 = 0;
      double cachedY3 = 0;
      BicubicPatch cachedPatch = default;

      for (int k = 0; k < w.Count; k++) // query point index
      {
        var uk = u[k]; // current query x-coordinate
        var vk = v[k]; // current query y-coordinate

        var ix = LocateIntervalIndex(_x, uk); // rectangle x-interval index (FORTRAN-style)
        var iy = LocateIntervalIndex(_y, vk); // rectangle y-interval index (FORTRAN-style)

        // Fast path: for interior points we can directly use the precomputed patch for the cell.
        // (Extrapolation for points outside the grid still uses the original on-demand computation.)
        if (_precomputedPatches is not null && ix >= 2 && ix <= lx0 && iy >= 2 && iy <= ly0) // point in an interior cell
        {
          var x3 = _x[ix - 2];
          var y3 = _y[iy - 2];
          w[k] = _precomputedPatches[ix - 2, iy - 2].Evaluate(uk - x3, vk - y3);
          continue;
        }

        if (!hasCachedPatch || cachedIx != ix || cachedIy != iy) // point not in cached rectangle
        {
          // Compute all local divided differences and partial derivatives needed
          // to build the bicubic polynomial for this rectangle.
          (cachedPatch, cachedX3, cachedY3) = ComputePatch(ix, iy);
          cachedIx = ix;
          cachedIy = iy;
          hasCachedPatch = true;
        }

        w[k] = cachedPatch.Evaluate(uk - cachedX3, vk - cachedY3);
      }
    }



    /// <summary>
    /// Returns a FORTRAN-compatible interval index.
    /// </summary>
    /// <remarks>
    /// The returned value is 1-based and corresponds to the smallest index <c>i</c> for which
    /// <c>value &lt;= arr[i-1]</c> would be false in the original routine.
    /// For values outside the grid, returns 1 (left/below) or <c>n+1</c> (right/above).
    /// </remarks>
    private static int LocateIntervalIndex(IReadOnlyList<double> arr, double value)
    {
      int n = arr.Count; // number of grid coordinates in this dimension
      if (n == 2) // degenerate case: exactly one interval
        return 2;
      if (value >= arr[n - 1]) // right/above of grid
        return n + 1;
      if (value < arr[0]) // left/below of grid
        return 1;

      int imn = 2; // lower bound of binary search (FORTRAN-style)
      int imx = n; // upper bound of binary search (FORTRAN-style)
      while (imx > imn)
      {
        int ix = (imn + imx) / 2; // mid index (FORTRAN-style)
        if (value >= arr[ix - 1]) // search in upper half
          imn = ix + 1;
        else // search in lower half
          imx = ix;
      }
      return imx;
    }

    /// <summary>
    /// Precomputes a <see cref="BicubicPatch"/> for every interior grid cell.
    /// </summary>
    /// <returns>A 2D array of precomputed cell patches indexed by <c>[ix-2, iy-2]</c>.</returns>
    private BicubicPatch[,] PrecomputePatches()
    {
      int lx0 = _x.Count; // number of grid points in X (LX)
      int ly0 = _y.Count; // number of grid points in Y (LY)

      var patches = new BicubicPatch[lx0 - 1, ly0 - 1]; // all interior cells (LX-1)*(LY-1)

      for (int iy = 2; iy <= ly0; iy++) // cell y-interval index (FORTRAN-style)
      {
        for (int ix = 2; ix <= lx0; ix++) // cell x-interval index (FORTRAN-style)
        {
          (patches[ix - 2, iy - 2], _, _) = ComputePatch(ix, iy); // precompute patch coefficients for this cell
        }
      }

      return patches;
    }



    /// <summary>
    /// Computes the bicubic patch coefficients for the cell corresponding to the given interval indices.
    /// </summary>
    /// <param name="ix">FORTRAN-style x-interval index (1..LX+1).</param>
    /// <param name="iy">FORTRAN-style y-interval index (1..LY+1).</param>
    /// <returns>
    /// A tuple containing the patch coefficients and the effective origin (<c>X3</c>, <c>Y3</c>) used to transform
    /// world coordinates into local patch coordinates.
    /// </returns>
    /// <remarks>
    /// When <paramref name="ix"/> or <paramref name="iy"/> is outside the grid (1 or n+1), the method applies the
    /// original algorithm's controlled extrapolation logic, which can shift the effective origin.
    /// </remarks>
    private (BicubicPatch Patch, double X3, double Y3) ComputePatch(int ix, int iy)
    {
      // This method is a structured translation of the original ITPLBV core.
      // It computes a bicubic polynomial for the rectangle identified by (ix, iy)
      // in FORTRAN 1-based interval indices.
      int lx0 = _x.Count; // number of grid points in X (LX)
      int ly0 = _y.Count; // number of grid points in Y (LY)
      int lxm1 = lx0 - 1; // LX - 1
      int lxm2 = lx0 - 2; // LX - 2 (used to detect LX==2)
      int lxp1 = lx0 + 1; // LX + 1 (sentinel interval index for x beyond right edge)
      int lym1 = ly0 - 1; // LY - 1
      int lym2 = ly0 - 2; // LY - 2 (used to detect LY==2)
      int lyp1 = ly0 + 1; // LY + 1 (sentinel interval index for y beyond top edge)

      // Helper indexers matching the original 1D array layouts used by the FORTRAN routine.
      // The indices are intentionally kept close to the original to make it easier to validate
      // formulas against the reference implementation.
      static int DIdx(int jx, int jy) => (jy - 1) * 4 + (jx - 1); // jx/jy in 1..4
      static int ZaIdx(int jx, int jy) => (jy - 2) * 5 + (jx - 1); // jx in 1..5, jy in 2..3
      static int ZbIdx(int jx, int jy) => (jy - 1) * 2 + (jx - 2); // jx in 2..3, jy in 1..5
      static int ZabIdx(int jx, int jy) => (jy - 2) * 3 + (jx - 2); // jx/jy in 2..4

      Span<double> zx = stackalloc double[16]; // estimated partial derivative dZ/dX at local 4x4 stencil points
      Span<double> zy = stackalloc double[16]; // estimated partial derivative dZ/dY at local 4x4 stencil points
      Span<double> zxy = stackalloc double[16]; // estimated mixed partial derivative d^2Z/(dX dY) at local 4x4 stencil points

      Span<double> za = stackalloc double[10]; // first divided differences in X for the 2x5 stencil (Akima's ZA)
      Span<double> zb = stackalloc double[10]; // first divided differences in Y for the 5x2 stencil (Akima's ZB)
      Span<double> zab = stackalloc double[9]; // second order mixed divided differences for the 3x3 stencil (Akima's ZAB)

      // The original algorithm relies on default zero values for entries that are not explicitly written.
      zx.Clear();
      zy.Clear();
      zxy.Clear();
      za.Clear();
      zb.Clear();
      zab.Clear();

      // Clamp rectangle indices (FORTRAN 1-based conventions):
      // ix/iy may be 1 or n+1 for points outside the grid.
      int jx = ix; // rectangle interval index in X (clamped to valid core range)
      if (jx == 1) // left of grid in X
        jx = 2;
      if (jx == lxp1) // right of grid in X
        jx = lx0;

      int jy = iy; // rectangle interval index in Y (clamped to valid core range)
      if (jy == 1) // below grid in Y
        jy = 2;
      if (jy == lyp1) // above grid in Y
        jy = ly0;

      int jxm2 = jx - 2; // 0 when there is no left neighbor rectangle in X
      int jxml = jx - lx0; // 0 when there is no right neighbor rectangle in X
      int jym2 = jy - 2; // 0 when there is no lower neighbor rectangle in Y
      int jyml = jy - ly0; // 0 when there is no upper neighbor rectangle in Y

      // Core area (rectangle containing the desired point):
      // local coordinates are built from (x3,x4) and (y3,y4).
      double x3 = _x[jx - 2]; // left x coordinate of the core rectangle
      double x4 = _x[jx - 1]; // right x coordinate of the core rectangle
      double a3 = 1.0 / (x4 - x3); // 1/dx for the core rectangle

      double y3 = _y[jy - 2]; // lower y coordinate of the core rectangle
      double y4 = _y[jy - 1]; // upper y coordinate of the core rectangle
      double b3 = 1.0 / (y4 - y3); // 1/dy for the core rectangle

      double z33 = _z[jx - 2, jy - 2]; // z at (x3,y3)
      double z43 = _z[jx - 1, jy - 2]; // z at (x4,y3)
      double z34 = _z[jx - 2, jy - 1]; // z at (x3,y4)
      double z44 = _z[jx - 1, jy - 1]; // z at (x4,y4)

      double z3a3 = (z43 - z33) * a3; // ZA at left edge of rectangle (dZ/dX at y3)
      double z4a3 = (z44 - z34) * a3; // ZA at left edge of rectangle (dZ/dX at y4)
      double z3b3 = (z34 - z33) * b3; // ZB at bottom edge of rectangle (dZ/dY at x3)
      double z4b3 = (z44 - z43) * b3; // ZB at bottom edge of rectangle (dZ/dY at x4)
      double za3b3 = (z4b3 - z3b3) * a3; // ZAB in core (mixed divided difference)

      za[ZaIdx(3, 2)] = z3a3;
      za[ZaIdx(3, 3)] = z4a3;

      zb[ZbIdx(2, 3)] = z3b3;
      zb[ZbIdx(3, 3)] = z4b3;

      zab[ZabIdx(3, 3)] = za3b3;

      double x2 = 0, x5 = 0; // neighbor x-coordinates left (x2) and right (x5) of the core rectangle
      double a1 = 0, a2 = 0, a4 = 0, a5 = 0; // inverse x-spacings used for divided differences (1/(x_i-x_{i-1}))
      double z23 = 0, z24 = 0, z53 = 0, z54 = 0; // neighbor z-values used to form ZA differences in X

      // X direction: compute divided differences ZA (w.r.t. x) around the rectangle.
      // Missing neighbor differences at the boundary are estimated by extrapolation,
      // exactly as in the original algorithm.
      if (lxm2 != 0) // LX >= 3: we can look at neighbors in X
      {
        bool hasLeftX = jxm2 != 0; // true if x2 exists (not at far left)
        bool hasRightX = jxml != 0; // true if x5 exists (not at far right)

        if (hasLeftX) // has a valid X neighbor at x2
        {
          x2 = _x[jx - 3];
          a2 = 1.0 / (x3 - x2);
          z23 = _z[jx - 3, jy - 2];
          z24 = _z[jx - 3, jy - 1];
          var z3a2 = (z33 - z23) * a2; // ZA one cell to the left at y3
          var z4a2 = (z34 - z24) * a2; // ZA one cell to the left at y4
          za[ZaIdx(2, 2)] = z3a2;
          za[ZaIdx(2, 3)] = z4a2;
        }

        if (hasRightX) // has a valid X neighbor at x5
        {
          x5 = _x[jx];
          a4 = 1.0 / (x5 - x4);
          z53 = _z[jx, jy - 2];
          z54 = _z[jx, jy - 1];
          var z3a4 = (z53 - z43) * a4; // ZA one cell to the right at y3
          var z4a4 = (z54 - z44) * a4; // ZA one cell to the right at y4
          za[ZaIdx(4, 2)] = z3a4;
          za[ZaIdx(4, 3)] = z4a4;
        }

        // Estimate missing a2/a4 at boundaries
        if (!hasLeftX && hasRightX) // missing left neighbor only
        {
          za[ZaIdx(2, 2)] = za[ZaIdx(3, 2)] + za[ZaIdx(3, 2)] - za[ZaIdx(4, 2)];
          za[ZaIdx(2, 3)] = za[ZaIdx(3, 3)] + za[ZaIdx(3, 3)] - za[ZaIdx(4, 3)];
        }
        else if (hasLeftX && !hasRightX) // missing right neighbor only
        {
          za[ZaIdx(4, 2)] = za[ZaIdx(3, 2)] + za[ZaIdx(3, 2)] - za[ZaIdx(2, 2)];
          za[ZaIdx(4, 3)] = za[ZaIdx(3, 3)] + za[ZaIdx(3, 3)] - za[ZaIdx(2, 3)];
        }
        else if (!hasLeftX && !hasRightX) // missing both neighbors (defensive)
        {
          // lx==2 is handled elsewhere, but keep defensively consistent.
          za[ZaIdx(2, 2)] = za[ZaIdx(3, 2)];
          za[ZaIdx(2, 3)] = za[ZaIdx(3, 3)];
          za[ZaIdx(4, 2)] = za[ZaIdx(3, 2)];
          za[ZaIdx(4, 3)] = za[ZaIdx(3, 3)];
        }

        // zab(*,b3)
        zab[ZabIdx(2, 3)] = (za[ZaIdx(2, 3)] - za[ZaIdx(2, 2)]) * b3;
        zab[ZabIdx(4, 3)] = (za[ZaIdx(4, 3)] - za[ZaIdx(4, 2)]) * b3;

        // a1 / a5
        if (jx > 3) // enough points to compute a1 from real data
        {
          a1 = 1.0 / (x2 - _x[jx - 4]);
          za[ZaIdx(1, 2)] = (z23 - _z[jx - 4, jy - 2]) * a1;
          za[ZaIdx(1, 3)] = (z24 - _z[jx - 4, jy - 1]) * a1;
        }
        else // estimate a1 by linear extrapolation
        {
          za[ZaIdx(1, 2)] = za[ZaIdx(2, 2)] + za[ZaIdx(2, 2)] - za[ZaIdx(3, 2)];
          za[ZaIdx(1, 3)] = za[ZaIdx(2, 3)] + za[ZaIdx(2, 3)] - za[ZaIdx(3, 3)];
        }

        if (jx < lxm1) // enough points to compute a5 from real data
        {
          a5 = 1.0 / (_x[jx + 1] - x5);
          za[ZaIdx(5, 2)] = (_z[jx + 1, jy - 2] - z53) * a5;
          za[ZaIdx(5, 3)] = (_z[jx + 1, jy - 1] - z54) * a5;
        }
        else // estimate a5 by linear extrapolation
        {
          za[ZaIdx(5, 2)] = za[ZaIdx(4, 2)] + za[ZaIdx(4, 2)] - za[ZaIdx(3, 2)];
          za[ZaIdx(5, 3)] = za[ZaIdx(4, 3)] + za[ZaIdx(4, 3)] - za[ZaIdx(3, 3)];
        }
      }
      else // LX == 2: no neighbors in X beyond the core rectangle
      {
        // lx==2
        za[ZaIdx(2, 2)] = za[ZaIdx(3, 2)];
        za[ZaIdx(2, 3)] = za[ZaIdx(3, 3)];
        za[ZaIdx(4, 2)] = za[ZaIdx(3, 2)];
        za[ZaIdx(4, 3)] = za[ZaIdx(3, 3)];
        za[ZaIdx(1, 2)] = za[ZaIdx(2, 2)];
        za[ZaIdx(1, 3)] = za[ZaIdx(2, 3)];
        za[ZaIdx(5, 2)] = za[ZaIdx(4, 2)];
        za[ZaIdx(5, 3)] = za[ZaIdx(4, 3)];

        zab[ZabIdx(2, 3)] = zab[ZabIdx(3, 3)];
        zab[ZabIdx(4, 3)] = zab[ZabIdx(3, 3)];
      }

      double y2 = 0, y5 = 0; // neighbor y-coordinates below (y2) and above (y5) the core rectangle
      double b1 = 0, b2 = 0, b4 = 0, b5 = 0; // inverse y-spacings used for divided differences (1/(y_j-y_{j-1}))
      double z32 = 0, z42 = 0, z35 = 0, z45 = 0; // neighbor z-values used to form ZB differences in Y

      // Y direction: compute divided differences ZB (w.r.t. y) around the rectangle.
      // Again, boundary values are estimated when necessary.
      if (lym2 != 0) // LY >= 3: we can look at neighbors in Y
      {
        bool hasLowerY = jym2 != 0; // true if y2 exists (not at bottom)
        bool hasUpperY = jyml != 0; // true if y5 exists (not at top)

        if (hasLowerY) // has a valid Y neighbor at y2
        {
          y2 = _y[jy - 3];
          b2 = 1.0 / (y3 - y2);
          z32 = _z[jx - 2, jy - 3];
          z42 = _z[jx - 1, jy - 3];
          zb[ZbIdx(2, 2)] = (z33 - z32) * b2;
          zb[ZbIdx(3, 2)] = (z43 - z42) * b2;
        }

        if (hasUpperY) // has a valid Y neighbor at y5
        {
          y5 = _y[jy];
          b4 = 1.0 / (y5 - y4);
          z35 = _z[jx - 2, jy];
          z45 = _z[jx - 1, jy];
          zb[ZbIdx(2, 4)] = (z35 - z34) * b4;
          zb[ZbIdx(3, 4)] = (z45 - z44) * b4;
        }

        // Estimate missing b2/b4 at boundaries
        if (!hasLowerY && hasUpperY) // missing lower neighbor only
        {
          zb[ZbIdx(2, 2)] = zb[ZbIdx(2, 3)] + zb[ZbIdx(2, 3)] - zb[ZbIdx(2, 4)];
          zb[ZbIdx(3, 2)] = zb[ZbIdx(3, 3)] + zb[ZbIdx(3, 3)] - zb[ZbIdx(3, 4)];
        }
        else if (hasLowerY && !hasUpperY) // missing upper neighbor only
        {
          zb[ZbIdx(2, 4)] = zb[ZbIdx(2, 3)] + zb[ZbIdx(2, 3)] - zb[ZbIdx(2, 2)];
          zb[ZbIdx(3, 4)] = zb[ZbIdx(3, 3)] + zb[ZbIdx(3, 3)] - zb[ZbIdx(3, 2)];
        }
        else if (!hasLowerY && !hasUpperY) // missing both neighbors (defensive)
        {
          // ly==2 is handled elsewhere, but keep defensively consistent.
          zb[ZbIdx(2, 2)] = zb[ZbIdx(2, 3)];
          zb[ZbIdx(3, 2)] = zb[ZbIdx(3, 3)];
          zb[ZbIdx(2, 4)] = zb[ZbIdx(2, 3)];
          zb[ZbIdx(3, 4)] = zb[ZbIdx(3, 3)];
        }

        // zab(a3,*)
        zab[ZabIdx(3, 2)] = (zb[ZbIdx(3, 2)] - zb[ZbIdx(2, 2)]) * a3;
        zab[ZabIdx(3, 4)] = (zb[ZbIdx(3, 4)] - zb[ZbIdx(2, 4)]) * a3;

        // b1 / b5
        if (jy > 3) // enough points to compute b1 from real data
        {
          b1 = 1.0 / (y2 - _y[jy - 4]);
          zb[ZbIdx(2, 1)] = (z32 - _z[jx - 2, jy - 4]) * b1;
          zb[ZbIdx(3, 1)] = (z42 - _z[jx - 1, jy - 4]) * b1;
        }
        else // estimate b1 by linear extrapolation
        {
          zb[ZbIdx(2, 1)] = zb[ZbIdx(2, 2)] + zb[ZbIdx(2, 2)] - zb[ZbIdx(2, 3)];
          zb[ZbIdx(3, 1)] = zb[ZbIdx(3, 2)] + zb[ZbIdx(3, 2)] - zb[ZbIdx(3, 3)];
        }

        if (jy < lym1) // enough points to compute b5 from real data
        {
          b5 = 1.0 / (_y[jy + 1] - y5);
          zb[ZbIdx(2, 5)] = (_z[jx - 2, jy + 1] - z35) * b5;
          zb[ZbIdx(3, 5)] = (_z[jx - 1, jy + 1] - z45) * b5;
        }
        else // estimate b5 by linear extrapolation
        {
          zb[ZbIdx(2, 5)] = zb[ZbIdx(2, 4)] + zb[ZbIdx(2, 4)] - zb[ZbIdx(2, 3)];
          zb[ZbIdx(3, 5)] = zb[ZbIdx(3, 4)] + zb[ZbIdx(3, 4)] - zb[ZbIdx(3, 3)];
        }
      }
      else // LY == 2: no neighbors in Y beyond the core rectangle
      {
        // ly==2
        zb[ZbIdx(2, 2)] = zb[ZbIdx(2, 3)];
        zb[ZbIdx(3, 2)] = zb[ZbIdx(3, 3)];
        zb[ZbIdx(2, 4)] = zb[ZbIdx(2, 3)];
        zb[ZbIdx(3, 4)] = zb[ZbIdx(3, 3)];
        zb[ZbIdx(2, 1)] = zb[ZbIdx(2, 2)];
        zb[ZbIdx(3, 1)] = zb[ZbIdx(3, 2)];
        zb[ZbIdx(2, 5)] = zb[ZbIdx(2, 4)];
        zb[ZbIdx(3, 5)] = zb[ZbIdx(3, 4)];

        zab[ZabIdx(3, 2)] = zab[ZabIdx(3, 3)];
        zab[ZabIdx(3, 4)] = zab[ZabIdx(3, 3)];
      }

      // Diagonal directions: compute second order mixed divided differences ZAB.
      // (These are later blended into the mixed derivative estimates.)
      if (lxm2 == 0) // LX == 2
      {
        zab[ZabIdx(2, 2)] = zab[ZabIdx(3, 2)];
        zab[ZabIdx(4, 2)] = zab[ZabIdx(3, 2)];
        zab[ZabIdx(2, 4)] = zab[ZabIdx(3, 4)];
        zab[ZabIdx(4, 4)] = zab[ZabIdx(3, 4)];
      }
      else if (lym2 == 0) // LY == 2
      {
        zab[ZabIdx(2, 2)] = zab[ZabIdx(2, 3)];
        zab[ZabIdx(2, 4)] = zab[ZabIdx(2, 3)];
        zab[ZabIdx(4, 2)] = zab[ZabIdx(4, 3)];
        zab[ZabIdx(4, 4)] = zab[ZabIdx(4, 3)];
      }
      else // general case: both LX >= 3 and LY >= 3
      {
        // Right side diagonals (x5)
        if (jxml != 0) // has right X neighbor
        {
          if (jym2 != 0) // has lower Y neighbor
          {
            zab[ZabIdx(4, 2)] = (((z53 - _z[jx, jy - 3]) * b2) - zb[ZbIdx(3, 2)]) * a4;
            if (jyml == 0) // missing upper Y neighbor
              zab[ZabIdx(4, 4)] = zab[ZabIdx(4, 3)] + zab[ZabIdx(4, 3)] - zab[ZabIdx(4, 2)];
          }

          if (jyml != 0) // has upper Y neighbor
          {
            zab[ZabIdx(4, 4)] = (((_z[jx, jy] - z54) * b4) - zb[ZbIdx(3, 4)]) * a4;
            if (jym2 == 0) // missing lower Y neighbor
              zab[ZabIdx(4, 2)] = zab[ZabIdx(4, 3)] + zab[ZabIdx(4, 3)] - zab[ZabIdx(4, 4)];
          }

          if (jym2 == 0 && jyml == 0) // missing both lower/upper Y neighbors
          {
            zab[ZabIdx(4, 2)] = zab[ZabIdx(4, 3)];
            zab[ZabIdx(4, 4)] = zab[ZabIdx(4, 3)];
          }
        }

        // Left side diagonals (x2)
        if (jxm2 != 0) // has left X neighbor
        {
          if (jym2 != 0) // has lower Y neighbor
          {
            zab[ZabIdx(2, 2)] = (zb[ZbIdx(2, 2)] - ((z23 - _z[jx - 3, jy - 3]) * b2)) * a2;
            if (jyml == 0) // missing upper Y neighbor
              zab[ZabIdx(2, 4)] = zab[ZabIdx(2, 3)] + zab[ZabIdx(2, 3)] - zab[ZabIdx(2, 2)];
          }

          if (jyml != 0) // has upper Y neighbor
          {
            zab[ZabIdx(2, 4)] = (zb[ZbIdx(2, 4)] - ((_z[jx - 3, jy] - z24) * b4)) * a2;
            if (jym2 == 0) // missing lower Y neighbor
              zab[ZabIdx(2, 2)] = zab[ZabIdx(2, 3)] + zab[ZabIdx(2, 3)] - zab[ZabIdx(2, 4)];
          }

          if (jym2 == 0 && jyml == 0) // missing both lower/upper Y neighbors
          {
            zab[ZabIdx(2, 2)] = zab[ZabIdx(2, 3)];
            zab[ZabIdx(2, 4)] = zab[ZabIdx(2, 3)];
          }
        }

        // If one side is missing in X, estimate from the other side
        if (jxm2 == 0) // missing left X neighbor
        {
          zab[ZabIdx(2, 2)] = zab[ZabIdx(3, 2)] + zab[ZabIdx(3, 2)] - zab[ZabIdx(4, 2)];
          zab[ZabIdx(2, 4)] = zab[ZabIdx(3, 4)] + zab[ZabIdx(3, 4)] - zab[ZabIdx(4, 4)];
        }
        else if (jxml == 0) // missing right X neighbor
        {
          zab[ZabIdx(4, 2)] = zab[ZabIdx(3, 2)] + zab[ZabIdx(3, 2)] - zab[ZabIdx(2, 2)];
          zab[ZabIdx(4, 4)] = zab[ZabIdx(3, 4)] + zab[ZabIdx(3, 4)] - zab[ZabIdx(2, 4)];
        }
      }

      // Numerical differentiation: determine partial derivatives ZX, ZY, ZXY as weighted means
      // of divided differences (Akima weighting with |delta| as weights).
      for (int jj = 2; jj <= 3; jj++)
      {
        for (int ii = 2; ii <= 3; ii++)
        {
          var w2x = Math.Abs(za[ZaIdx(ii + 2, jj)] - za[ZaIdx(ii + 1, jj)]); // |ΔZA| on the right side
          var w3x = Math.Abs(za[ZaIdx(ii, jj)] - za[ZaIdx(ii - 1, jj)]); // |ΔZA| on the left side
          var swx = w2x + w3x; // weight sum for X-derivative blend
          var wx2 = swx == 0 ? 0.5 : w2x / swx; // normalized Akima weight for right difference
          var wx3 = swx == 0 ? 0.5 : w3x / swx; // normalized Akima weight for left difference

          zx[DIdx(ii, jj)] = wx2 * za[ZaIdx(ii, jj)] + wx3 * za[ZaIdx(ii + 1, jj)];

          var w2y = Math.Abs(zb[ZbIdx(ii, jj + 2)] - zb[ZbIdx(ii, jj + 1)]); // |ΔZB| above
          var w3y = Math.Abs(zb[ZbIdx(ii, jj)] - zb[ZbIdx(ii, jj - 1)]); // |ΔZB| below
          var swy = w2y + w3y; // weight sum for Y-derivative blend
          var wy2 = swy == 0 ? 0.5 : w2y / swy; // normalized Akima weight for upper difference
          var wy3 = swy == 0 ? 0.5 : w3y / swy; // normalized Akima weight for lower difference

          zy[DIdx(ii, jj)] = wy2 * zb[ZbIdx(ii, jj)] + wy3 * zb[ZbIdx(ii, jj + 1)];

          zxy[DIdx(ii, jj)] = wy2 * (wx2 * zab[ZabIdx(ii, jj)] + wx3 * zab[ZabIdx(ii + 1, jj)])
            + wy3 * (wx2 * zab[ZabIdx(ii, jj + 1)] + wx3 * zab[ZabIdx(ii + 1, jj + 1)]);
        }
      }

      // When u is outside the x-range, the original routine performs a controlled extrapolation
      // by modifying the local divided differences and shifting the (x3,z33) origin.
      int jxForL570 = 0; // non-zero indicates that X extrapolation happened (emulates original label L570)
      if (ix == 1) // query point is left of the X grid
      {
        var w2 = a4 * (a3 * 3.0 + a4); // Akima boundary blending weight (right side)
        var w1 = a3 * 2.0 * (a3 - a4) + w2; // Akima boundary blending weight (left side)

        for (int jj = 2; jj <= 3; jj++)
        {
          zx[DIdx(1, jj)] = (w1 * za[ZaIdx(1, jj)] + w2 * za[ZaIdx(2, jj)]) / (w1 + w2);
          zy[DIdx(1, jj)] = zy[DIdx(2, jj)] + zy[DIdx(2, jj)] - zy[DIdx(3, jj)];
          zxy[DIdx(1, jj)] = zxy[DIdx(2, jj)] + zxy[DIdx(2, jj)] - zxy[DIdx(3, jj)];

          for (int ii = 3; ii >= 2; ii--)
          {
            zx[DIdx(ii, jj)] = zx[DIdx(ii - 1, jj)];
            zy[DIdx(ii, jj)] = zy[DIdx(ii - 1, jj)];
            zxy[DIdx(ii, jj)] = zxy[DIdx(ii - 1, jj)];
          }
        }

        x3 -= 1.0 / a4;
        z33 -= za[ZaIdx(2, 2)] / a4;

        for (int j = 1; j <= 5; j++)
          zb[ZbIdx(3, j)] = zb[ZbIdx(2, j)];

        for (int j = 2; j <= 4; j++)
          zb[ZbIdx(2, j)] -= zab[ZabIdx(2, j)] / a4;

        a3 = a4;
        jxForL570 = 1;
      }
      else if (ix == lxp1) // query point is right of the X grid
      {
        var w4 = a2 * (a3 * 3.0 + a2); // Akima boundary blending weight (left side)
        var w5 = a3 * 2.0 * (a3 - a2) + w4; // Akima boundary blending weight (right side)

        for (int jj = 2; jj <= 3; jj++)
        {
          zx[DIdx(4, jj)] = (w4 * za[ZaIdx(4, jj)] + w5 * za[ZaIdx(5, jj)]) / (w4 + w5);
          zy[DIdx(4, jj)] = zy[DIdx(3, jj)] + zy[DIdx(3, jj)] - zy[DIdx(2, jj)];
          zxy[DIdx(4, jj)] = zxy[DIdx(3, jj)] + zxy[DIdx(3, jj)] - zxy[DIdx(2, jj)];

          for (int ii = 2; ii <= 3; ii++)
          {
            zx[DIdx(ii, jj)] = zx[DIdx(ii + 1, jj)];
            zy[DIdx(ii, jj)] = zy[DIdx(ii + 1, jj)];
            zxy[DIdx(ii, jj)] = zxy[DIdx(ii + 1, jj)];
          }
        }

        x3 = x4;
        z33 = z43;

        for (int j = 1; j <= 5; j++)
          zb[ZbIdx(2, j)] = zb[ZbIdx(3, j)];

        a3 = a2;
        jxForL570 = 3;
      }

      // L570 equivalent (only reached when x is outside)
      if (jxForL570 != 0) // X extrapolation was applied
      {
        // In the original, this re-centers the ZA/ZAB arrays so the polynomial is still computed
        // from the same "central" column after X extrapolation.
        // Matches the original routine's in-place adjustment of the central ZA/ZAB column after X extrapolation.
        // (This is intentionally done with the original 1D layouts/indices.)
        za[2] = za[jxForL570];
        for (int j = 1; j <= 3; j++)
          zab[j * 3 - 2] = zab[jxForL570 + j * 3 - 4];
      }

      // When v is outside the y-range, apply the symmetric extrapolation logic in Y.
      if (iy == 1) // query point is below the Y grid
      {
        var w2 = b4 * (b3 * 3.0 + b4); // Akima boundary blending weight (upper side)
        var w1 = b3 * 2.0 * (b3 - b4) + w2; // Akima boundary blending weight (lower side)

        for (int ii = 2; ii <= 3; ii++)
        {
          bool skip = (ii == 3 && ix == lxp1) || (ii == 2 && ix == 1);
          if (!skip)
          {
            zy[DIdx(ii, 1)] = (w1 * zb[ZbIdx(ii, 1)] + w2 * zb[ZbIdx(ii, 2)]) / (w1 + w2);
            zx[DIdx(ii, 1)] = zx[DIdx(ii, 2)] + zx[DIdx(ii, 2)] - zx[DIdx(ii, 3)];
            zxy[DIdx(ii, 1)] = zxy[DIdx(ii, 2)] + zxy[DIdx(ii, 2)] - zxy[DIdx(ii, 3)];
          }

          for (int jj = 3; jj >= 2; jj--)
          {
            zy[DIdx(ii, jj)] = zy[DIdx(ii, jj - 1)];
            zx[DIdx(ii, jj)] = zx[DIdx(ii, jj - 1)];
            zxy[DIdx(ii, jj)] = zxy[DIdx(ii, jj - 1)];
          }
        }

        y3 -= 1.0 / b4;
        z33 -= zb[ZbIdx(2, 2)] / b4;
        z3a3 -= zab[ZabIdx(3, 2)] / b4;
        z3b3 = zb[ZbIdx(2, 2)];
        za3b3 = zab[ZabIdx(3, 2)];
        b3 = b4;
      }
      else if (iy == lyp1) // query point is above the Y grid
      {
        var w4 = b2 * (b3 * 3.0 + b2); // Akima boundary blending weight (lower side)
        var w5 = b3 * 2.0 * (b3 - b2) + w4; // Akima boundary blending weight (upper side)

        for (int ii = 2; ii <= 3; ii++)
        {
          bool skip = (ii == 3 && ix == lxp1) || (ii == 2 && ix == 1); // avoid overwriting the corner already handled by X extrapolation
          if (!skip)
          {
            zy[DIdx(ii, 4)] = (w4 * zb[ZbIdx(ii, 4)] + w5 * zb[ZbIdx(ii, 5)]) / (w4 + w5);
            zx[DIdx(ii, 4)] = zx[DIdx(ii, 3)] + zx[DIdx(ii, 3)] - zx[DIdx(ii, 2)];
            zxy[DIdx(ii, 4)] = zxy[DIdx(ii, 3)] + zxy[DIdx(ii, 3)] - zxy[DIdx(ii, 2)];
          }

          for (int jj = 2; jj <= 3; jj++)
          {
            zy[DIdx(ii, jj)] = zy[DIdx(ii, jj + 1)];
            zx[DIdx(ii, jj)] = zx[DIdx(ii, jj + 1)];
            zxy[DIdx(ii, jj)] = zxy[DIdx(ii, jj + 1)];
          }
        }

        y3 = y4;
        z33 += z3b3 / b3;
        z3a3 += za3b3 / b3;
        z3b3 = zb[ZbIdx(2, 4)];
        za3b3 = zab[ZabIdx(3, 4)];
        b3 = b2;
      }

      if (ix == 1 || ix == lxp1) // X extrapolation case: apply corner fix-up
      {
        // Corner fix-up when both X and Y are extrapolated.
        int jxTmp = ix / lxp1 + 2; // local stencil x-index of extrapolated side
        int jx1 = 5 - jxTmp; // opposite local stencil x-index
        int jyTmp = iy / lyp1 + 2; // local stencil y-index of extrapolated side
        int jy1 = 5 - jyTmp; // opposite local stencil y-index

        zx[DIdx(jxTmp, jyTmp)] = zx[DIdx(jx1, jyTmp)] + zx[DIdx(jxTmp, jy1)] - zx[DIdx(jx1, jy1)];
        zy[DIdx(jxTmp, jyTmp)] = zy[DIdx(jx1, jyTmp)] + zy[DIdx(jxTmp, jy1)] - zy[DIdx(jx1, jy1)];
        zxy[DIdx(jxTmp, jyTmp)] = zxy[DIdx(jx1, jyTmp)] + zxy[DIdx(jxTmp, jy1)] - zxy[DIdx(jx1, jy1)];
      }

      // Determine bicubic coefficients for
      //   w(x,y) = sum_{i=0..3} sum_{j=0..3} p_ij * (x-x3)^i * (y-y3)^j
      // using the same closed-form expressions as the original implementation.
      double zx33 = zx[DIdx(2, 2)];
      double zx43 = zx[DIdx(3, 2)];
      double zx34 = zx[DIdx(2, 3)];
      double zx44 = zx[DIdx(3, 3)];

      double zy33 = zy[DIdx(2, 2)];
      double zy43 = zy[DIdx(3, 2)];
      double zy34 = zy[DIdx(2, 3)];
      double zy44 = zy[DIdx(3, 3)];

      double zxy33 = zxy[DIdx(2, 2)];
      double zxy43 = zxy[DIdx(3, 2)];
      double zxy34 = zxy[DIdx(2, 3)];
      double zxy44 = zxy[DIdx(3, 3)];

      var zx3b3 = (zx34 - zx33) * b3; // d/dy of zx along left edge
      var zx4b3 = (zx44 - zx43) * b3; // d/dy of zx along right edge
      var zy3a3 = (zy43 - zy33) * a3; // d/dx of zy along bottom edge
      var zy4a3 = (zy44 - zy34) * a3; // d/dx of zy along top edge

      var A = za3b3 - zx3b3 - zy3a3 + zxy33; // helper term (matches original variable a)
      var B = zx4b3 - zx3b3 - zxy43 + zxy33; // helper term (matches original variable b)
      var C = zy4a3 - zy3a3 - zxy34 + zxy33; // helper term (matches original variable c)
      var D = zxy44 - zxy43 - zxy34 + zxy33; // helper term (matches original variable d)
      var E = A + A - B - C; // helper term (matches original variable e)

      var a3sq = a3 * a3; // (1/dx)^2
      var b3sq = b3 * b3; // (1/dy)^2

      double p00 = z33;
      double p01 = zy33;
      double p10 = zx33;
      double p11 = zxy33;

      double p02 = ((z3b3 - zy33) * 2.0 + z3b3 - zy34) * b3;
      double p03 = (z3b3 * -2.0 + zy34 + zy33) * b3sq;

      double p12 = ((zx3b3 - zxy33) * 2.0 + zx3b3 - zxy34) * b3;
      double p13 = (zx3b3 * -2.0 + zxy34 + zxy33) * b3sq;

      double p20 = ((z3a3 - zx33) * 2.0 + z3a3 - zx43) * a3;
      double p21 = ((zy3a3 - zxy33) * 2.0 + zy3a3 - zxy43) * a3;
      double p22 = ((A + E) * 3.0 + D) * a3 * b3;
      double p23 = (E * -3.0 - B - D) * a3 * b3sq;

      double p30 = (z3a3 * -2.0 + zx43 + zx33) * a3sq;
      double p31 = (zy3a3 * -2.0 + zxy43 + zxy33) * a3sq;
      double p32 = (E * -3.0 - C - D) * b3 * a3sq;
      double p33 = (D + E + E) * a3sq * b3sq;

      return (new BicubicPatch(p00, p01, p02, p03, p10, p11, p12, p13, p20, p21, p22, p23, p30, p31, p32, p33), x3, y3);
    }
  }
}
