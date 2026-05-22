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
using Altaxo.Calc.LinearAlgebra.Storage;
using Xunit;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Contains regression tests for the <see cref="BandMatrixStorage{T}"/> implementation.
  /// </summary>
  public class BandMatrixStorageTests
  {
    /// <summary>
    /// Verifies that initialization by callback uses the expected compact band storage layout.
    /// </summary>
    [Fact]
    public void OfInit_UsesCompactBandLayout()
    {
      var storage = BandMatrixStorage<double>.OfInit(5, 4, 1, 2, static (row, column) => 10 * row + column);

      Assert.Equal(1, storage.LowerBandwidth);
      Assert.Equal(2, storage.UpperBandwidth);
      Assert.Equal(new double[]
      {
        0, 0, 2, 13,
        0, 1, 12, 23,
        0, 11, 22, 33,
        10, 21, 32, 43,
      }, storage.Data);

      Assert.Equal(0, storage.At(0, 3));
      Assert.Equal(23, storage.At(2, 3));
      Assert.Equal(10, storage.At(1, 0));
    }

    /// <summary>
    /// Verifies that assigning values outside the stored band accepts zero only and rejects non-zero values.
    /// </summary>
    [Fact]
    public void SettingOutsideBandAllowsOnlyZero()
    {
      var storage = BandMatrixStorage<double>.OfValue(4, 4, 1, 1, 5);

      storage.At(0, 3, 0);

      var exception = Assert.Throws<IndexOutOfRangeException>(() => storage.At(0, 3, 1));
      Assert.Contains("outside the band", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Verifies that copying to dense storage preserves the in-band entries and clears out-of-band positions.
    /// </summary>
    [Fact]
    public void CopyToDensePreservesBandEntries()
    {
      var storage = BandMatrixStorage<double>.OfInit(5, 4, 1, 2, static (row, column) => 100 * row + column);
      var dense = DenseColumnMajorMatrixStorage<double>.OfValue(5, 4, -1);

      storage.CopyTo(dense);

      for (var i = 0; i < 5; i++)
      {
        for (var j = 0; j < 4; j++)
        {
          var expected = i - j <= 1 && j - i <= 2 ? 100 * i + j : 0;
          Assert.Equal(expected, dense.At(i, j));
        }
      }
    }

    /// <summary>
    /// Verifies that clearing a submatrix affects only the stored band entries that intersect the requested region.
    /// </summary>
    [Fact]
    public void ClearSubMatrix_OnlyClearsIntersectingBandEntries()
    {
      var storage = BandMatrixStorage<double>.OfValue(5, 5, 1, 1, 7);

      storage.Clear(1, 3, 1, 3);

      Assert.Equal(7, storage.At(0, 0));
      Assert.Equal(7, storage.At(0, 1));
      Assert.Equal(0, storage.At(1, 1));
      Assert.Equal(0, storage.At(1, 2));
      Assert.Equal(0, storage.At(2, 1));
      Assert.Equal(0, storage.At(2, 2));
      Assert.Equal(0, storage.At(2, 3));
      Assert.Equal(0, storage.At(3, 2));
      Assert.Equal(0, storage.At(3, 3));
      Assert.Equal(7, storage.At(3, 4));
      Assert.Equal(7, storage.At(4, 4));
    }

    /// <summary>
    /// Verifies that creating band storage from an array rejects non-zero entries outside the requested band.
    /// </summary>
    [Fact]
    public void OfArray_RejectsNonZeroEntriesOutsideBand()
    {
      var data = new double[,]
      {
        { 1, 2, 9 },
        { 3, 4, 5 },
        { 0, 6, 7 },
      };

      Assert.Throws<IndexOutOfRangeException>(() => BandMatrixStorage<double>.OfArray(data, 1, 1));
    }
  }
}
