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
using Altaxo.Calc.LinearAlgebra.Double;
using Altaxo.Calc.LinearAlgebra.Storage;
using Xunit;

namespace Altaxo.Calc.LinearAlgebra
{




  /// <summary>
  /// Contains regression tests for the <see cref="BandMatrix"/> implementation.
  /// </summary>
  public class BandMatrixTests
  {
    private static (double[,] matrix, int lowerBandwidth, int upperBandwidth, double[] vector, double[] matrixXvector)[] TestData =
    [
    (new double[,]
       {
        { 4, 0, 0, 0 },
        { 0, 5, 0, 0 },
        { 0, 0, 6, 0 },
        { 0, 0, 0, 7 },
      }, 0, 0,
      new double[]{ 4, 3, 2, 1 },
      new double[]{ 16, 15, 12, 7 }),

      (new double[,]
      {
        { 2, -1, 0 },
        { -1, 2, -1 },
        { 0, -1, 2 },
      }, 1, 1,
      new double[]{ 1, 1, 1 },
      new double[]{ 1, 0, 1 } ),

      (new double[,]
      {
        { 4, 1, 0 },
        { 1, 5, 1 },
        { 0, 1, 6 },
      }, 1, 1,
      new double[]{1,1,1},
      new double[]{ 5, 7, 7 }),

    (new double[,]
      {
        { 4, 1, 1, 0 },
        { 1, 5, 1, 1 },
        { 1, 1, 6, 1 },
        { 0, 1, 1, 7 },
      }, 2, 2,
      new double[]{ 1, 1, 1, 1 },
      new double[]{ 6, 8, 9, 9 }),

    (new double[,]
      {
        {  20,  3, -2,  1,  0,  0,  0,  0,  0,  0 },
        {   4, 18,  5, -3,  2,  0,  0,  0,  0,  0 },
        {  -1,  2, 22,  4, -1,  3,  0,  0,  0,  0 },
        {   3, -2,  1, 19,  6, -2,  4,  0,  0,  0 },
        {   0,  2, -3,  5, 21,  3, -1,  2,  0,  0 },
        {   0,  0,  4, -1,  2, 17,  5, -3,  1,  0 },
        {   0,  0,  0,  3, -2,  4, 20,  6, -2,  3 },
        {   0,  0,  0,  0,  1, -3,  2, 23,  4, -1 },
        {   0,  0,  0,  0,  0,  2, -1,  3, 18,  5 },
        {   0,  0,  0,  0,  0,  0,  3, -2,  4, 21 },
      }, 3, 3,
    new double[] { 17, 28, 6, 23, 11, 19, 7, 25, 14, 29 },
    new double[] {435, 555, 309, 494, 484, 320, 472, 570, 503, 636 }
    ),

    (new double[,]
      {
        { 22,  3, -2,  4,  1,  0,  0,  0,  0,  0 },
        {  0, 19,  5, -3,  2,  3,  0,  0,  0,  0 },
        {  0,  0, 21,  4, -1,  2, -2,  0,  0,  0 },
        {  0,  0,  0, 18,  6, -3,  3,  1,  0,  0 },
        {  0,  0,  0,  0, 20,  4, -2,  5,  2,  0 },
        {  0,  0,  0,  0,  0, 23,  3, -1,  4, -2 },
        {  0,  0,  0,  0,  0,  0, 17,  5, -3,  2 },
        {  0,  0,  0,  0,  0,  0,  0, 22,  4, -1 },
        {  0,  0,  0,  0,  0,  0,  0,  0, 19,  3 },
        {  0,  0,  0,  0,  0,  0,  0,  0,  0, 24 },
      }, 0, 4,
    new double[] { 17, 28, 6, 23, 11, 19, 7, 25, 14, 29 },
    new double[] {549, 572, 231, 469, 435, 431, 260, 577, 353, 696 }
    ),


    (new double[,]
      {
        { 22,  3,  0,  0,  0,  0,  0,  0,  0,  0 },
        {  4, 19,  2,  0,  0,  0,  0,  0,  0,  0 },
        { -2,  5, 21,  4,  0,  0,  0,  0,  0,  0 },
        {  3, -1,  6, 18,  1,  0,  0,  0,  0,  0 },
        {  1,  2, -3,  4, 20,  3,  0,  0,  0,  0 },
        {  0,  4, -1,  2, -4, 23,  2,  0,  0,  0 },
        {  0,  0,  3, -2,  5, -1, 17,  4,  0,  0 },
        {  0,  0,  0,  1, -3,  4, -2, 22,  3,  0 },
        {  0,  0,  0,  0,  2, -1,  3, -3, 19,  5 },
        {  0,  0,  0,  0,  0,  3, -2,  4, -1, 24 },
      }, 4, 1,
    new double[] { 17, 28, 6, 23, 11, 19, 7, 25, 14, 29 },
    new double[] {458, 612, 324, 484, 424, 559, 227, 644, 360,825 }
    ),

    ];

    /// <summary>
    /// Validate the test data.
    /// This is done by using dense arithmetic, to make sure that
    /// errors in band matrix class don't affect the validity of the test data itself.
    /// </summary>
    [Fact]
    public void Test_TestData()
    {
      foreach (var (matrix, lowerBandwidth, upperBandwidth, vector, matrixXvector) in TestData)
      {
        var bandMatrix = CreateMatrix.DenseOfArray<double>(matrix);
        var result = bandMatrix.Multiply(new DenseVector(vector));
        for (int i = 0; i < result.Count; i++)
        {
          Assert.Equal(matrixXvector[i], result[i], 12);
        }
      }
    }

    /// <summary>
    /// Verifies that constructing a <see cref="BandMatrix"/> from an existing storage instance preserves its dimensions, bandwidths, and values.
    /// </summary>
    [Fact]
    public void Constructor_UsesProvidedBandStorage()
    {
      var storage = new BandMatrixStorage<double>(4, 5, 1, 2);
      storage.At(0, 2, 12);
      storage.At(1, 0, 10);

      var matrix = new BandMatrix(storage);

      Assert.Equal(1, matrix.LowerBandwidth);
      Assert.Equal(2, matrix.UpperBandwidth);
      Assert.Equal(4, matrix.RowCount);
      Assert.Equal(5, matrix.ColumnCount);
      Assert.Equal(12, matrix[0, 2]);
      Assert.Equal(10, matrix[1, 0]);
      Assert.Equal(0, matrix[3, 0]);
    }

    /// <summary>
    /// Verifies that creating a band matrix from a two-dimensional array preserves the expected in-band entries.
    /// </summary>
    [Fact]
    public void OfArray_CreatesBandMatrixWithExpectedEntries()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 3, 0 },
        { 4, 5, 6, 7 },
        { 0, 8, 9, 10 },
        { 0, 0, 11, 12 },
      }, 1, 2);

      Assert.Equal(1, matrix.LowerBandwidth);
      Assert.Equal(2, matrix.UpperBandwidth);
      Assert.Equal(9, matrix[2, 2]);
      Assert.Equal(3, matrix[0, 2]);
      Assert.Equal(11, matrix[3, 2]);
    }

    /// <summary>
    /// Verifies that creating a matrix wrapper from band storage returns a <see cref="BandMatrix"/> instance with matching metadata and values.
    /// </summary>
    [Fact]
    public void BuilderWithStorage_RoundTripsToBandMatrix()
    {
      var storage = BandMatrixStorage<double>.OfInit(3, 4, 1, 1, static (row, column) => 10 * row + column);

      var matrix = CreateMatrix.WithStorage(storage);

      var band = Assert.IsType<BandMatrix>(matrix);
      Assert.Equal(1, band.LowerBandwidth);
      Assert.Equal(1, band.UpperBandwidth);
      Assert.Equal(21, band[2, 1]);
      Assert.Equal(0, band[0, 3]);
    }

    /// <summary>
    /// Verifies that cloning a band matrix preserves the concrete type, bandwidths, and representative values.
    /// </summary>
    [Fact]
    public void Clone_PreservesBandMatrixTypeAndBandwidth()
    {
      var matrix = BandMatrix.Create(4, 4, 1, 2, static (row, column) => 100 * row + column);

      var clone = Assert.IsType<BandMatrix>(matrix.Clone());

      Assert.Equal(1, clone.LowerBandwidth);
      Assert.Equal(2, clone.UpperBandwidth);
      Assert.Equal(matrix[3, 2], clone[3, 2]);
      Assert.Equal(matrix[0, 2], clone[0, 2]);
      Assert.Equal(matrix[3, 0], clone[3, 0]);
    }

    /// <summary>
    /// Verifies that the specialized diagonal solve path computes the exact solution.
    /// </summary>
    [Fact]
    public void Solve_DiagonalBandMatrix_SolvesExactly()
    {
      var matrix = BandMatrix.Create(3, 3, 0, 0, static (row, column) => row switch
      {
        0 => 2,
        1 => 3,
        _ => 4,
      });

      var solution = matrix.Solve(new DenseVector([4d, 9d, 8d]));

      Assert.Equal(2d, solution[0]);
      Assert.Equal(3d, solution[1]);
      Assert.Equal(2d, solution[2]);
    }

    /// <summary>
    /// Verifies that the specialized diagonal solve path writes into the caller-provided result vector.
    /// </summary>
    [Fact]
    public void Solve_DiagonalBandMatrix_WritesIntoProvidedResultVector()
    {
      var matrix = BandMatrix.Create(3, 3, 0, 0, static (row, column) => row + 2d);
      var result = CreateVector.Dense<double>(3);

      matrix.Solve(new DenseVector([2d, 6d, 12d]), result);

      Assert.Equal(1d, result[0]);
      Assert.Equal(2d, result[1]);
      Assert.Equal(3d, result[2]);
    }

    /// <summary>
    /// Verifies that the tridiagonal specialized solve path based on the Thomas algorithm produces the expected solution.
    /// </summary>
    [Fact]
    public void Solve_TridiagonalBandMatrix_UsesThomasAlgorithmPath()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 2, -1, 0 },
        { -1, 2, -1 },
        { 0, -1, 2 },
      }, 1, 1);

      var solution = matrix.Solve(new DenseVector([1d, 0d, 1d]));

      Assert.Equal(1d, solution[0], 12);
      Assert.Equal(1d, solution[1], 12);
      Assert.Equal(1d, solution[2], 12);
    }

    /// <summary>
    /// Verifies that the tridiagonal specialized solve path writes into the supplied result vector.
    /// </summary>
    [Fact]
    public void Solve_TridiagonalBandMatrix_WritesIntoProvidedResultVector()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 2, -1, 0 },
        { -1, 2, -1 },
        { 0, -1, 2 },
      }, 1, 1);
      var result = CreateVector.Dense<double>(3);

      matrix.Solve(new DenseVector([1d, 0d, 1d]), result);

      Assert.Equal(1d, result[0], 12);
      Assert.Equal(1d, result[1], 12);
      Assert.Equal(1d, result[2], 12);
    }

    /// <summary>
    /// Verifies that wider band matrices still solve correctly when the implementation falls back to the dense LU path.
    /// </summary>
    [Fact]
    public void Solve_WiderBandMatrix_FallsBackToDenseLu()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 4, 1, 1, 0 },
        { 1, 5, 1, 1 },
        { 1, 1, 6, 1 },
        { 0, 1, 1, 7 },
      }, 2, 2);

      var solution = matrix.Solve(new DenseVector([6d, 8d, 9d, 9d]));

      Assert.Equal(1d, solution[0], 12);
      Assert.Equal(1d, solution[1], 12);
      Assert.Equal(1d, solution[2], 12);
      Assert.Equal(1d, solution[3], 12);
    }

    /// <summary>
    /// Verifies that the pentadiagonal specialized solve path produces the expected solution.
    /// </summary>
    [Fact]
    public void Solve_PentadiagonalBandMatrix_UsesExistingPentadiagonalSolver()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 6, 1, 1, 0, 0 },
        { 1, 7, 1, 1, 0 },
        { 1, 1, 8, 1, 1 },
        { 0, 1, 1, 9, 1 },
        { 0, 0, 1, 1, 10 },
      }, 2, 2);

      var solution = matrix.Solve(new DenseVector([8d, 10d, 12d, 12d, 12d]));

      Assert.Equal(1d, solution[0], 12);
      Assert.Equal(1d, solution[1], 12);
      Assert.Equal(1d, solution[2], 12);
      Assert.Equal(1d, solution[3], 12);
      Assert.Equal(1d, solution[4], 12);
    }

    /// <summary>
    /// Verifies that the pentadiagonal specialized solve path writes into the provided result vector.
    /// </summary>
    [Fact]
    public void Solve_PentadiagonalBandMatrix_WritesIntoProvidedResultVector()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 6, 1, 1, 0, 0 },
        { 1, 7, 1, 1, 0 },
        { 1, 1, 8, 1, 1 },
        { 0, 1, 1, 9, 1 },
        { 0, 0, 1, 1, 10 },
      }, 2, 2);
      var result = CreateVector.Dense<double>(5);

      matrix.Solve(new DenseVector([8d, 10d, 12d, 12d, 12d]), result);

      Assert.Equal(1d, result[0], 12);
      Assert.Equal(1d, result[1], 12);
      Assert.Equal(1d, result[2], 12);
      Assert.Equal(1d, result[3], 12);
      Assert.Equal(1d, result[4], 12);
    }

    /// <summary>
    /// Verifies that small pentadiagonal matrices fall back to the dense LU path and still solve correctly.
    /// </summary>
    [Fact]
    public void Solve_SmallPentadiagonalBandMatrix_FallsBackToDenseLu()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 4, 1, 1 },
        { 1, 5, 1 },
        { 1, 1, 6 },
      }, 2, 2);

      var solution = matrix.Solve(new DenseVector([6d, 7d, 8d]));

      Assert.Equal(1d, solution[0], 12);
      Assert.Equal(1d, solution[1], 12);
      Assert.Equal(1d, solution[2], 12);
    }

    /// <summary>
    /// Verifies that scalar multiplication preserves the band entries and their scaled values.
    /// </summary>
    [Fact]
    public void Multiply_ByScalar_PreservesBandEntries()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0 },
        { 3, 4, 5 },
        { 0, 6, 7 },
      }, 1, 1);

      var result = matrix.Multiply(2.0);

      Assert.Equal(2d, result[0, 0]);
      Assert.Equal(4d, result[0, 1]);
      Assert.Equal(10d, result[1, 2]);
      Assert.Equal(12d, result[2, 1]);
      Assert.Equal(0d, result[0, 2]);
    }

    /// <summary>
    /// Verifies that vector multiplication uses only the stored band entries.
    /// </summary>
    [Fact]
    public void Multiply_ByVector_UsesBandLimitedEntries()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0, 0 },
        { 3, 4, 5, 0 },
        { 0, 6, 7, 8 },
      }, 1, 1);

      var result = matrix.Multiply(new DenseVector([1d, 2d, 3d, 4d]));

      Assert.Equal(5d, result[0], 12);
      Assert.Equal(26d, result[1], 12);
      Assert.Equal(65d, result[2], 12);
    }

    /// <summary>
    /// Verifies multiplication by a dense matrix.
    /// </summary>
    [Fact]
    public void Multiply_ByMatrix_ComputesExpectedProduct()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0 },
        { 3, 4, 5 },
        { 0, 6, 7 },
      }, 1, 1);

      var other = DenseMatrix.OfArray(new double[,]
      {
        { 1, 2 },
        { 3, 4 },
        { 5, 6 },
      });

      var result = matrix.Multiply(other);

      Assert.Equal(7d, result[0, 0], 12);
      Assert.Equal(10d, result[0, 1], 12);
      Assert.Equal(40d, result[1, 0], 12);
      Assert.Equal(52d, result[1, 1], 12);
      Assert.Equal(53d, result[2, 0], 12);
      Assert.Equal(66d, result[2, 1], 12);
    }

    /// <summary>
    /// Verifies multiplication by another band matrix.
    /// </summary>
    [Fact]
    public void Multiply_ByBandMatrix_ComputesExpectedProduct()
    {
      var left = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0 },
        { 3, 4, 5 },
        { 0, 6, 7 },
      }, 1, 1);
      var right = BandMatrix.OfArray(new double[,]
      {
        { 1, 1, 0 },
        { 2, 3, 4 },
        { 0, 5, 6 },
      }, 1, 1);

      var result = DenseMatrix.Create(left.RowCount, right.ColumnCount, 0d);
      left.Multiply(right, result);

      Assert.Equal(5d, result[0, 0], 12);
      Assert.Equal(7d, result[0, 1], 12);
      Assert.Equal(8d, result[0, 2], 12);
      Assert.Equal(11d, result[1, 0], 12);
      Assert.Equal(40d, result[1, 1], 12);
      Assert.Equal(46d, result[1, 2], 12);
      Assert.Equal(12d, result[2, 0], 12);
      Assert.Equal(53d, result[2, 1], 12);
      Assert.Equal(66d, result[2, 2], 12);
    }

    /// <summary>
    /// Verifies multiplication by a diagonal matrix.
    /// </summary>
    [Fact]
    public void Multiply_ByDiagonalMatrix_ComputesExpectedProduct()
    {
      var left = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0 },
        { 3, 4, 5 },
        { 0, 6, 7 },
      }, 1, 1);
      var right = DiagonalMatrix.OfDiagonal(3, 3, [10d, 20d, 30d]);

      var result = left.Multiply(right);

      Assert.Equal(10d, result[0, 0], 12);
      Assert.Equal(40d, result[0, 1], 12);
      Assert.Equal(0d, result[0, 2], 12);
      Assert.Equal(30d, result[1, 0], 12);
      Assert.Equal(80d, result[1, 1], 12);
      Assert.Equal(150d, result[1, 2], 12);
      Assert.Equal(0d, result[2, 0], 12);
      Assert.Equal(120d, result[2, 1], 12);
      Assert.Equal(210d, result[2, 2], 12);
    }

    /// <summary>
    /// Verifies multiplication by a sparse matrix.
    /// </summary>
    [Fact]
    public void Multiply_BySparseMatrix_ComputesExpectedProduct()
    {
      var left = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0 },
        { 3, 4, 5 },
        { 0, 6, 7 },
      }, 1, 1);
      var right = SparseMatrix.OfArray(new double[,]
      {
        { 1, 0 },
        { 0, 2 },
        { 3, 0 },
      });

      var result = left.Multiply(right);

      Assert.Equal(1d, result[0, 0], 12);
      Assert.Equal(4d, result[0, 1], 12);
      Assert.Equal(18d, result[1, 0], 12);
      Assert.Equal(8d, result[1, 1], 12);
      Assert.Equal(21d, result[2, 0], 12);
      Assert.Equal(12d, result[2, 1], 12);
    }

    /// <summary>
    /// Verifies addition with another band matrix.
    /// </summary>
    [Fact]
    public void Add_BandMatrix_ComputesExpectedSum()
    {
      var left = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0 },
        { 3, 4, 5 },
        { 0, 6, 7 },
      }, 1, 1);
      var right = BandMatrix.OfArray(new double[,]
      {
        { 10, 20, 0 },
        { 30, 40, 50 },
        { 0, 60, 70 },
      }, 1, 1);

      var result = DenseMatrix.Create(3, 3, 0d);
      left.Add(right, result);

      Assert.Equal(11d, result[0, 0], 12);
      Assert.Equal(22d, result[0, 1], 12);
      Assert.Equal(33d, result[1, 0], 12);
      Assert.Equal(44d, result[1, 1], 12);
      Assert.Equal(55d, result[1, 2], 12);
      Assert.Equal(66d, result[2, 1], 12);
      Assert.Equal(77d, result[2, 2], 12);
    }

    /// <summary>
    /// Verifies addition with a diagonal matrix.
    /// </summary>
    [Fact]
    public void Add_DiagonalMatrix_ComputesExpectedSum()
    {
      var left = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0 },
        { 3, 4, 5 },
        { 0, 6, 7 },
      }, 1, 1);
      var right = DiagonalMatrix.OfDiagonal(3, 3, [10d, 20d, 30d]);

      var result = DenseMatrix.Create(3, 3, 0d);
      left.Add(right, result);

      Assert.Equal(11d, result[0, 0], 12);
      Assert.Equal(2d, result[0, 1], 12);
      Assert.Equal(3d, result[1, 0], 12);
      Assert.Equal(24d, result[1, 1], 12);
      Assert.Equal(5d, result[1, 2], 12);
      Assert.Equal(6d, result[2, 1], 12);
      Assert.Equal(37d, result[2, 2], 12);
    }

    /// <summary>
    /// Verifies addition with a sparse matrix.
    /// </summary>
    [Fact]
    public void Add_SparseMatrix_ComputesExpectedSum()
    {
      var left = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0 },
        { 3, 4, 5 },
        { 0, 6, 7 },
      }, 1, 1);
      var right = SparseMatrix.OfArray(new double[,]
      {
        { 10, 0, 0 },
        { 0, 20, 0 },
        { 0, 0, 30 },
      });

      var result = DenseMatrix.Create(3, 3, 0d);
      left.Add(right, result);

      Assert.Equal(11d, result[0, 0], 12);
      Assert.Equal(2d, result[0, 1], 12);
      Assert.Equal(3d, result[1, 0], 12);
      Assert.Equal(24d, result[1, 1], 12);
      Assert.Equal(5d, result[1, 2], 12);
      Assert.Equal(6d, result[2, 1], 12);
      Assert.Equal(37d, result[2, 2], 12);
    }

    /// <summary>
    /// Verifies subtraction of a dense matrix.
    /// </summary>
    [Fact]
    public void Subtract_DenseMatrix_ComputesExpectedDifference()
    {
      var left = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0 },
        { 3, 4, 5 },
        { 0, 6, 7 },
      }, 1, 1);
      var right = DenseMatrix.OfArray(new double[,]
      {
        { 10, 20, 30 },
        { 40, 50, 60 },
        { 70, 80, 90 },
      });

      var result = DenseMatrix.Create(3, 3, 0d);
      left.Subtract(right, result);

      Assert.Equal(-9d, result[0, 0], 12);
      Assert.Equal(-18d, result[0, 1], 12);
      Assert.Equal(-30d, result[0, 2], 12);
      Assert.Equal(-37d, result[1, 0], 12);
      Assert.Equal(-46d, result[1, 1], 12);
      Assert.Equal(-55d, result[1, 2], 12);
      Assert.Equal(-70d, result[2, 0], 12);
      Assert.Equal(-74d, result[2, 1], 12);
      Assert.Equal(-83d, result[2, 2], 12);
    }

    /// <summary>
    /// Verifies multiplication of the transpose of a band matrix by a vector.
    /// </summary>
    [Fact]
    public void TransposeThisAndMultiply_Vector_ComputesExpectedProduct()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0 },
        { 3, 4, 5 },
        { 0, 6, 7 },
      }, 1, 1);

      var result = matrix.TransposeThisAndMultiply(new DenseVector([1d, 2d, 3d]));

      Assert.Equal(7d, result[0], 12);
      Assert.Equal(28d, result[1], 12);
      Assert.Equal(31d, result[2], 12);
    }

    /// <summary>
    /// Verifies multiplication of the transpose of a band matrix by a dense matrix.
    /// </summary>
    [Fact]
    public void TransposeThisAndMultiply_Matrix_ComputesExpectedProduct()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0 },
        { 3, 4, 5 },
        { 0, 6, 7 },
      }, 1, 1);

      var other = DenseMatrix.OfArray(new double[,]
      {
        { 1, 0 },
        { 0, 1 },
        { 1, 1 },
      });

      var result = matrix.TransposeThisAndMultiply(other);

      Assert.Equal(1d, result[0, 0], 12);
      Assert.Equal(3d, result[0, 1], 12);
      Assert.Equal(8d, result[1, 0], 12);
      Assert.Equal(10d, result[1, 1], 12);
      Assert.Equal(7d, result[2, 0], 12);
      Assert.Equal(12d, result[2, 1], 12);
    }

    /// <summary>
    /// Verifies inversion of a diagonal band matrix.
    /// </summary>
    [Fact]
    public void Inverse_DiagonalBandMatrix_ReturnsReciprocalDiagonal()
    {
      var matrix = BandMatrix.Create(3, 3, 0, 0, static (row, column) => row + 2d);

      var inverse = matrix.Inverse();

      Assert.Equal(0.5d, inverse[0, 0], 12);
      Assert.Equal(1d / 3d, inverse[1, 1], 12);
      Assert.Equal(0.25d, inverse[2, 2], 12);
      Assert.Equal(0d, inverse[0, 1]);
    }

    /// <summary>
    /// Verifies that Gaussian elimination overwrites all entries of a prefilled result vector.
    /// </summary>
    [Fact]
    public void SolveByGaussianElimination_ComputesExpectedSolution()
    {
      foreach (var testCase in TestData)
      {
        var matrix = BandMatrix.OfArray(testCase.matrix, testCase.lowerBandwidth, testCase.upperBandwidth);
        var input = CreateVector.DenseOfArray(testCase.matrixXvector);
        var result = CreateVector.Random<double>(testCase.vector.Length);
        object? tempStorage = null;
        matrix.SolveByGaussianElimination(input, result, ref tempStorage);
        for (var i = 0; i < testCase.vector.Length; i++)
        {
          Assert.Equal(testCase.vector[i], result[i], 12);
        }
      }
    }

    /// <summary>
    /// Verifies that banded Cholesky factorization returns a lower-triangular band factor with the expected values.
    /// </summary>
    [Fact]
    public void Cholesky_SymmetricPositiveDefiniteBandMatrix_ReturnsBandFactor()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 4, 1, 0 },
        { 1, 3, 1 },
        { 0, 1, 2 },
      }, 1, 1);

      var cholesky = matrix.Cholesky();

      var factor = Assert.IsType<BandMatrix>(cholesky.Factor);
      Assert.Equal(1, factor.LowerBandwidth);
      Assert.Equal(0, factor.UpperBandwidth);
      Assert.Equal(2d, factor[0, 0], 12);
      Assert.Equal(0.5d, factor[1, 0], 12);
      Assert.Equal(Math.Sqrt(11d / 4d), factor[1, 1], 12);
      Assert.Equal(1d / Math.Sqrt(11d / 4d), factor[2, 1], 12);
      Assert.Equal(Math.Sqrt(2d - (4d / 11d)), factor[2, 2], 12);
    }

    /// <summary>
    /// Verifies that solving with the banded Cholesky factorization produces the expected solution vector.
    /// </summary>
    [Fact]
    public void Cholesky_SolveVector_ComputesExpectedSolution()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 4, 1, 0 },
        { 1, 3, 1 },
        { 0, 1, 2 },
      }, 1, 1);

      var result = matrix.Cholesky().Solve(new DenseVector([5d, 5d, 3d]));

      Assert.Equal(1d, result[0], 12);
      Assert.Equal(1d, result[1], 12);
      Assert.Equal(1d, result[2], 12);
    }

    /// <summary>
    /// Verifies that Cholesky factorization rejects non-symmetric band matrices.
    /// </summary>
    [Fact]
    public void Cholesky_NonSymmetricBandMatrix_ThrowsArgumentException()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 4, 2, 0 },
        { 1, 3, 1 },
        { 0, 1, 2 },
      }, 1, 1);

      Assert.Throws<ArgumentException>(() => matrix.Cholesky());
    }

    /// <summary>
    /// Verifies that Cholesky factorization rejects band matrices that are not positive definite.
    /// </summary>
    [Fact]
    public void Cholesky_NotPositiveDefiniteBandMatrix_ThrowsArgumentException()
    {
      var matrix = BandMatrix.OfArray(new double[,]
      {
        { 1, 2, 0 },
        { 2, 1, 0 },
        { 0, 0, 1 },
      }, 1, 1);

      Assert.Throws<ArgumentException>(() => matrix.Cholesky());
    }
  }
}
