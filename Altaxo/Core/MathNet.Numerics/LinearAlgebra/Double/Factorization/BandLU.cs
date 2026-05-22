// <copyright file="BandLU.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2026 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  internal sealed class BandLU : LU
  {
    private readonly BandMatrix _matrix;
    private readonly DenseLU? _denseFallback;
    private object? _pentaDiagonalTempStorage;

    private BandLU(BandMatrix matrix, Matrix<double> factors, int[] pivots, DenseLU? denseFallback)
      : base(factors, pivots)
    {
      _matrix = matrix;
      _denseFallback = denseFallback;
    }

    public static BandLU Create(BandMatrix matrix)
    {
      if (matrix is null)
      {
        throw new ArgumentNullException(nameof(matrix));
      }

      if (matrix.RowCount != matrix.ColumnCount)
      {
        throw new ArgumentException("Matrix must be square.");
      }

      if (matrix.LowerBandwidth == 0 && matrix.UpperBandwidth == 0)
      {
        var pivots = new int[matrix.RowCount];
        for (var i = 0; i < pivots.Length; i++)
        {
          pivots[i] = i;
        }

        var factors = new DiagonalMatrix(matrix.RowCount, matrix.ColumnCount);
        for (var i = 0; i < matrix.RowCount; i++)
        {
          factors[i, i] = matrix[i, i];
        }

        return new BandLU(matrix, factors, pivots, null);
      }

      if (matrix.LowerBandwidth == 1 && matrix.UpperBandwidth == 1)
      {
        var pivots = new int[matrix.RowCount];
        for (var i = 0; i < pivots.Length; i++)
        {
          pivots[i] = i;
        }

        return new BandLU(matrix, DenseMatrix.OfMatrix(matrix), pivots, null);
      }

      if (matrix.LowerBandwidth == 2 && matrix.UpperBandwidth == 2 && matrix.RowCount >= 4)
      {
        var pivots = new int[matrix.RowCount];
        for (var i = 0; i < pivots.Length; i++)
        {
          pivots[i] = i;
        }

        return new BandLU(matrix, DenseMatrix.OfMatrix(matrix), pivots, null);
      }

      var dense = DenseMatrix.OfMatrix(matrix);
      return new BandLU(matrix, dense, new int[matrix.RowCount], DenseLU.Create(dense));
    }

    public override void Solve(Matrix<double> input, Matrix<double> result)
    {
      if (input is null)
      {
        throw new ArgumentNullException(nameof(input));
      }

      if (result is null)
      {
        throw new ArgumentNullException(nameof(result));
      }

      if (result.RowCount != input.RowCount)
      {
        throw new ArgumentException("Matrix row dimensions must agree.");
      }

      if (result.ColumnCount != input.ColumnCount)
      {
        throw new ArgumentException("Matrix column dimensions must agree.");
      }

      if (input.RowCount != _matrix.RowCount)
      {
        throw Matrix.DimensionsDontMatch<ArgumentException>(input, _matrix);
      }

      if (_denseFallback is not null)
      {
        _denseFallback.Solve(input, result);
        return;
      }

      for (var j = 0; j < input.ColumnCount; j++)
      {
        var rhs = new double[input.RowCount];
        for (var i = 0; i < input.RowCount; i++)
        {
          rhs[i] = input[i, j];
        }

        var solution = SolveCore(rhs);
        for (var i = 0; i < result.RowCount; i++)
        {
          result[i, j] = solution[i];
        }
      }
    }

    public override void Solve(Vector<double> input, Vector<double> result)
    {
      if (input is null)
      {
        throw new ArgumentNullException(nameof(input));
      }

      if (result is null)
      {
        throw new ArgumentNullException(nameof(result));
      }

      if (input.Count != result.Count)
      {
        throw new ArgumentException("All vectors must have the same dimensionality.");
      }

      if (input.Count != _matrix.RowCount)
      {
        throw Matrix.DimensionsDontMatch<ArgumentException>(input, _matrix);
      }

      if (_denseFallback is not null)
      {
        var denseInput = input is DenseVector existingDenseInput ? existingDenseInput : DenseVector.OfVector(input);
        var denseResult = new DenseVector(result.Count);
        _denseFallback.Solve(denseInput, denseResult);
        denseResult.CopyTo(result);
        return;
      }

      var rhs = new double[input.Count];
      for (var i = 0; i < rhs.Length; i++)
      {
        rhs[i] = input[i];
      }

      var solution = SolveCore(rhs);
      for (var i = 0; i < solution.Length; i++)
      {
        result[i] = solution[i];
      }
    }

    public override Matrix<double> Inverse()
    {
      if (_denseFallback is not null)
      {
        return _denseFallback.Inverse();
      }

      if (_matrix.LowerBandwidth == 0 && _matrix.UpperBandwidth == 0)
      {
        var inverse = new DiagonalMatrix(_matrix.RowCount, _matrix.ColumnCount);
        for (var i = 0; i < _matrix.RowCount; i++)
        {
          var diagonal = _matrix[i, i];
          if (diagonal == 0.0)
          {
            throw new ArgumentException("Matrix must not be singular.");
          }

          inverse[i, i] = 1.0 / diagonal;
        }

        return inverse;
      }

      var identity = new DenseMatrix(_matrix.RowCount);
      for (var i = 0; i < identity.RowCount; i++)
      {
        identity[i, i] = 1.0;
      }

      return Solve(identity);
    }

    private double[] SolveCore(double[] rhs)
    {
      if (_matrix.LowerBandwidth == 0 && _matrix.UpperBandwidth == 0)
      {
        return SolveDiagonal(rhs);
      }

      if (_matrix.LowerBandwidth == 1 && _matrix.UpperBandwidth == 1)
      {
        return SolveTridiagonal(rhs);
      }

      if (_matrix.LowerBandwidth == 2 && _matrix.UpperBandwidth == 2)
      {
        return SolvePentadiagonal(rhs);
      }

      throw new NotSupportedException("Band solve path is only available for diagonal, tridiagonal, or pentadiagonal matrices without dense fallback.");
    }

    private double[] SolveDiagonal(double[] rhs)
    {
      var result = new double[rhs.Length];
      for (var i = 0; i < rhs.Length; i++)
      {
        var diagonal = _matrix[i, i];
        if (diagonal == 0.0)
        {
          throw new ArgumentException("Matrix must not be singular.");
        }

        result[i] = rhs[i] / diagonal;
      }

      return result;
    }

    private double[] SolveTridiagonal(double[] rhs)
    {
      var n = _matrix.RowCount;
      if (n == 0)
      {
        return [];
      }

      var cPrime = new double[n];
      var dPrime = new double[n];
      var denominator = _matrix[0, 0];
      if (denominator == 0.0)
      {
        throw new ArgumentException("Matrix must not be singular.");
      }

      if (n > 1)
      {
        cPrime[0] = _matrix[0, 1] / denominator;
      }
      dPrime[0] = rhs[0] / denominator;

      for (var i = 1; i < n; i++)
      {
        denominator = _matrix[i, i] - (_matrix[i, i - 1] * cPrime[i - 1]);
        if (denominator == 0.0)
        {
          throw new ArgumentException("Matrix must not be singular.");
        }

        cPrime[i] = i < n - 1 ? _matrix[i, i + 1] / denominator : 0.0;
        dPrime[i] = (rhs[i] - (_matrix[i, i - 1] * dPrime[i - 1])) / denominator;
      }

      var result = new double[n];
      result[n - 1] = dPrime[n - 1];
      for (var i = n - 2; i >= 0; i--)
      {
        result[i] = dPrime[i] - (cPrime[i] * result[i + 1]);
      }

      return result;
    }

    private double[] SolvePentadiagonal(double[] rhs)
    {
      var result = new double[rhs.Length];
      GaussianEliminationSolver.SolvePentaDiagonal(_matrix, rhs, result, ref _pentaDiagonalTempStorage);
      return result;
    }
  }
}
