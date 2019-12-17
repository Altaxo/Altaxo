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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Interpolation
{
    /// <summary>
    /// Interpolation method for scattered data in any dimension based on radial basis functions.
    /// In 2D this is the so called Thin Plate Spline, which is an interpolation method that finds a "minimally bended"
    /// smooth surface that passes through all given points.
    /// The polyharmonic spline has an arbitrary number of dimensions and arbitrary derivative order.
    /// Note: The allocation space requirement is in the order (N+3)*(N+3), where N is the number of control points. Thus it is not applicable for too many points.
    /// </summary>
    /// <remarks>
    /// Literature:
    /// http://en.wikipedia.org/wiki/Polyharmonic_spline
    ///
    /// Extension to any number of dimensions:
    /// TIM GUTZMER AND JENS MARKUS MELENK, MATHEMATICS OF COMPUTATION, Volume 70, Number 234, Pages 699{703, S 0025-5718(00)01299-0, Article electronically published on October 18, 2000
    ///</remarks>
    public class PolyharmonicSpline
    {
        /// <summary>Default value of the <see cref="RegularizationParameter"/>.</summary>
        public const double DefaultRegularizationParameter = 0;

        /// <summary>Default value of the <see cref="DerivativeOrder"/>.</summary>
        public const int DefaultDerivativeOrder = 2;

        private DoubleVector _mtx_v;

        /// <summary>
        /// This matrix is only neccessary for calculating the bending energy.
        /// </summary>
        private DoubleMatrix _mtx_orig_k;

        private int _derivativeOrder;

        /// <summary>
        /// The dimension of the coordinates. If for instance splining height values of an area, this value is 2.
        /// </summary>
        private int _coordDim;

        /// <summary>
        /// Number of control points. Can be different from the length of the arrays.
        /// </summary>
        private int _numberOfControlPoints;

        /// <summary>
        /// The cached Tps function.
        /// </summary>
        private Func<double, double> _cachedTpsFunc;

        /// <summary>
        /// The coordinate points. The first index is the index to the coordinate component (i.e. 0==x, 1,==y ..). The second index is the index of the control point.
        /// </summary>
        private double[][] _coordinates;

        /// <summary>
        /// Values associated with the control points. If for instance splining height values of an area, this is the height value.
        /// But it can be any other value, like temperature, and so on.
        /// </summary>
        private double[] _values;

        /// <summary>
        /// Regularization parameter (&gt;=0).
        /// If the regularization parameter is zero, interpolation is exact. As it approaches infinity, the resulting spline is reduced to a least squares linear fit (in 2D this is a plane, the bending energy is 0).
        /// </summary>
        private double _regularizationParameter;

        /// <summary>
        /// Initializes the spline with a regularization parameter of zero and an derivative order of 2.
        /// </summary>
        public PolyharmonicSpline()
        {
            _regularizationParameter = DefaultRegularizationParameter;
            _derivativeOrder = DefaultDerivativeOrder;
        }

        /// <summary>Invalidating interpolation by clearing the result.</summary>
        private void Clear()
        {
            _mtx_v = null;
        }

        /// <summary>
        /// Regularization parameter (&gt;=0).
        /// If the regularization parameter is zero, interpolation is exact.
        /// As it approaches infinity, the resulting spline is reduced to a least squares linear fit (in 2D this is a plane, the bending energy is 0).
        /// </summary>
        public double RegularizationParameter
        {
            get
            {
                return _regularizationParameter;
            }
            set
            {
                if (!(value > 0))
                    throw new ArgumentOutOfRangeException("Value have to be >=0");

                var oldValue = _regularizationParameter;
                _regularizationParameter = value;
                if (oldValue != value)
                    Clear();
            }
        }

        /// <summary>
        /// Derivative order of the spline. Effectively, the L2-norm of this derivative is minimized.
        /// </summary>
        public int DerivativeOrder
        {
            get
            {
                return _derivativeOrder;
            }
            set
            {
                if (!(value >= 1))
                    throw new ArgumentOutOfRangeException("DerivativeOrder have to be >=1");

                var oldValue = _derivativeOrder;
                _derivativeOrder = value;
                if (oldValue != value)
                    Clear();
            }
        }

        /// <summary>Number of dimensions of the control points.</summary>
        public int NumberOfDimensions
        {
            get
            {
                return _coordDim;
            }
        }

        /// <summary>
        /// Number of control points.
        /// </summary>
        public int NumberOfControlPoints
        {
            get
            {
                return _numberOfControlPoints;
            }
        }

        /// <summary>
        /// Constructs the interpolation (1 dimensional). The values and the corresponding coordinates of the values are given in separate vectors.
        /// </summary>
        /// <param name="x">X coordinates of the points.</param>
        /// <param name="h">Values of the points that should be interpolated.</param>
        public void Construct(IReadOnlyList<double> x, IReadOnlyList<double> h)
        {
            Construct(new IReadOnlyList<double>[] { x }, h);
        }

        /// <summary>
        /// Constructs the interpolation (2 dimensional). The values and the corresponding coordinates of the values are given in separate vectors.
        /// </summary>
        /// <param name="x">X coordinates of the points.</param>
        /// <param name="y">Y coordinates of the points.</param>
        /// <param name="h">Values of the points that should be interpolated.</param>
        public void Construct(IReadOnlyList<double> x, IReadOnlyList<double> y, IReadOnlyList<double> h)
        {
            Construct(new IReadOnlyList<double>[] { x, y }, h);
        }

        /// <summary>
        /// Constructs the interpolation (3 dimensional). The values and the corresponding coordinates of the values are given in separate vectors.
        /// </summary>
        /// <param name="x">X coordinates of the points.</param>
        /// <param name="y">Y coordinates of the points.</param>
        /// <param name="z">Z coordinates of the points.</param>
        /// <param name="h">Values of the points that should be interpolated.</param>
        public void Construct(IReadOnlyList<double> x, IReadOnlyList<double> y, IReadOnlyList<double> z, IReadOnlyList<double> h)
        {
            Construct(new IReadOnlyList<double>[] { x, y, z }, h);
        }

        /// <summary>
        /// Constructs the interpolation (any dimension). The values and the corresponding coordinates of the values are given in separate vectors.
        /// </summary>
        /// <param name="x">X coordinates of the points.</param>
        /// <param name="h">Values of the points that should be interpolated.</param>
        public void Construct(IReadOnlyList<double>[] x, IReadOnlyList<double> h)
        {
            _coordDim = x.Length;
            if (0 == _coordDim)
                throw new ArgumentException("The spline must have at least one dimension (No coordinates were provided).");

            _numberOfControlPoints = h.Count;
            for (int d = 0; d < _coordDim; d++)
                _numberOfControlPoints = Math.Min(_numberOfControlPoints, x[d].Count);
            if (_numberOfControlPoints <= _coordDim)
                throw new ArgumentException("The number of control points must exceed the number of dimension at least by one.");

            _coordinates = new double[_coordDim][];
            for (int d = 0; d < _coordDim; d++)
            {
                var x_d = x[d];
                var coord_d = _coordinates[d] = new double[_numberOfControlPoints];
                for (int i = 0; i < _numberOfControlPoints; i++)
                    coord_d[i] = x_d[i];
            }

            _values = new double[_numberOfControlPoints];
            for (int i = 0; i < _numberOfControlPoints; i++)
                _values[i] = h[i];

            SetCachedTpsFunction();

            InternalCompute();
        }

        private double tps_base_even_pos(double r)
        {
            return r == 0 ? 0 : RMath.Pow(r, 2 * _derivativeOrder - _coordDim) * Math.Log(r);
        }

        private double tps_base_even_neg(double r)
        {
            return r == 0 ? 0 : -RMath.Pow(r, 2 * _derivativeOrder - _coordDim) * Math.Log(r);
        }

        private double tps_base_odd_pos(double r)
        {
            return r == 0 ? 0 : RMath.Pow(r, 2 * _derivativeOrder - _coordDim);
        }

        private double tps_base_odd_neg(double r)
        {
            return r == 0 ? 0 : -RMath.Pow(r, 2 * _derivativeOrder - _coordDim);
        }

        private bool IsEven(int i)
        {
            return 0 == (i % 2);
        }

        private void SetCachedTpsFunction()
        {
            if (IsEven(_coordDim) && ((2 * _derivativeOrder) >= _coordDim)) // even dimension and 2*m>=n
            {
                if (IsEven(1 + _coordDim / 2))
                    _cachedTpsFunc = tps_base_even_pos;
                else
                    _cachedTpsFunc = tps_base_even_neg;
            }
            else // odd dimension or 2*m<n
            {
                if (IsEven(_derivativeOrder))
                    _cachedTpsFunc = tps_base_odd_pos;
                else
                    _cachedTpsFunc = tps_base_odd_neg;
            }
        }

        private double Pow2(double x)
        {
            return x * x;
        }

        /// <summary>
        /// Returns the distance between the control points at index i and j.
        /// </summary>
        /// <param name="i">Index of one control point.</param>
        /// <param name="j">Index of another control point.</param>
        /// <returns>The distance of coordinates between control points i and j.</returns>
        private double DistanceBetweenControlPoints(int i, int j)
        {
            double sumsqr = 0;
            for (int d = 0; d < _coordDim; d++)
            {
                var coord_d = _coordinates[d];
                sumsqr += Pow2(coord_d[i] - coord_d[j]);
            }
            return Math.Sqrt(sumsqr);
        }

        /// <summary>
        /// Returns the distance between control point at index i and another point.
        /// </summary>
        /// <param name="i">Index of the control point.</param>
        /// <param name="x">Coordinates of the other point.</param>
        /// <returns>Distance between control point at index i and point x.</returns>
        private double DistanceBetweenControlPointAndPoint(int i, params double[] x)
        {
            double sumsqr = 0;
            for (int d = 0; d < _coordDim; d++)
            {
                var coord_d = _coordinates[d];
                sumsqr += Pow2(_coordinates[d][i] - x[d]);
            }
            return Math.Sqrt(sumsqr);
        }

        /// <summary>
        /// Calculate the matrix for the polyharmonic spline and solves the linear equation to calculate the coefficients.
        /// </summary>
        private void InternalCompute()
        {
            var N = _numberOfControlPoints;

            // Allocate the matrix and vector
            var mtx_l = new DoubleMatrix(N + 1 + _coordDim, N + 1 + _coordDim);

            // there is no need for this matrix if we don't need to calculate the bending energy
            _mtx_orig_k = new DoubleMatrix(N, N);

            // Fill K (p x p, upper left of L)
            // K is symmetrical so we really have to calculate only about half of the coefficients.
            double a = 0.0;
            for (int i = 0; i < N; ++i)
            {
                for (int j = i + 1; j < N; ++j)
                {
                    double distance = DistanceBetweenControlPoints(i, j);
                    mtx_l[i, j] = mtx_l[j, i] = _mtx_orig_k[i, j] = _mtx_orig_k[j, i] = _cachedTpsFunc(distance);
                    a += distance * 2; // same for upper & lower triangle
                }
            }
            a /= ((double)N) * N;

            // Fill the rest of L with the values to do regularization and linear interpolation
            for (int i = 0; i < N; ++i)
            {
                // diagonal: regularization parameter (lambda * a^2)
                mtx_l[i, i] = _mtx_orig_k[i, i] = _regularizationParameter * (a * a);

                // P (N x (nCoordDim+1), upper right)
                mtx_l[i, N + 0] = 1; // for the intercept
                for (int d = 0; d < _coordDim; d++)
                    mtx_l[i, N + 1 + d] = _coordinates[d][i];

                // P transposed ((nCoordDim+1) x N, bottom left)
                mtx_l[N + 0, i] = 1; // for the intercept
                for (int d = 0; d < _coordDim; ++d)
                    mtx_l[N + 1 + d, i] = _coordinates[d][i];
            }
            // Zero ((nCoordDim+1) x (nCoordDim+1), lower right)
            for (int i = N; i < N + _coordDim + 1; ++i)
                for (int j = N; j < N + _coordDim + 1; ++j)
                    mtx_l[i, j] = 0;

            // Solve the linear system
            var solver = new DoubleLUDecomp(mtx_l);
            if (solver.IsSingular)
                throw new ArgumentException("The provided points lead to a singular matrix");

            // Fill the right hand vector V with the values to spline; the last nCoordDim+1 elements are zero
            _mtx_v = new DoubleVector(N + 1 + _coordDim);
            for (int i = 0; i < N; ++i)
                _mtx_v[i] = _values[i];
            for (int d = 0; d <= _coordDim; ++d) // comparison '<=' is ok here because of additional intercept
                _mtx_v[N + d] = 0;

            _mtx_v = solver.Solve(_mtx_v);
        }

        /// <summary>
        /// Gets the interpolation value of given coordinates x and y.
        /// </summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <returns>The interpolated value at the point (x,y).</returns>
        public double GetInterpolatedValue(params double[] x)
        {
            int N = _numberOfControlPoints;
            // first calculate the linear interpolation h = intercept + a1*x1 + a2*x2 + ..
            double h = _mtx_v[N + 0]; // intercept
            for (int d = 0; d < _coordDim; d++) // calculate linear interpolation of x, y, ..
                h += _mtx_v[N + 1 + d] * x[d];

            for (int i = 0; i < N; ++i)
            {
                double distance = DistanceBetweenControlPointAndPoint(i, x);
                h += _mtx_v[i] * _cachedTpsFunc(distance);
            }
            return h;
        }

        /// <summary>
        /// Gets the bending energy of the interpolation.
        /// </summary>
        /// <returns></returns>
        public double GetBendingEnergy()
        {
            return MatrixMath.MultiplyVectorFromLeftAndRight(_mtx_orig_k, _mtx_v);
        }
    }
}
