#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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


using Xunit;

namespace Altaxo.Calc.LinearAlgebra
{
  public class RegularizationTests
  {
    [Fact]
    public void TestZeroOrder_NotCircular()
    {
      int numberOfColumns = 5;
      int numberOfRegularRows = 7;
      bool isCircular = false;

      var matrix = Matrix<double>.Build.Random(numberOfRegularRows + numberOfColumns, numberOfColumns);
      var param = Vector<double>.Build.Random(numberOfRegularRows + numberOfColumns);
      double lambda = 777;
      matrix.AddRegularization(param, numberOfRegularRows, lambda, 1, 0, 0, isCircular);

      for (int r = 0; r < numberOfColumns; r++)
      {
        Assert.Equal(0, param[numberOfRegularRows + r]);
        for (int c = 0; c < numberOfColumns; c++)
        {
          if (c != r)
            Assert.Equal(0, matrix[numberOfRegularRows + r, c]);
          else
            Assert.Equal(lambda, matrix[numberOfRegularRows + r, r]);
        }
      }
    }

    [Fact]
    public void TestZeroOrder_Circular()
    {
      int numberOfColumns = 5;
      int numberOfRegularRows = 7;
      bool isCircular = true;

      var matrix = Matrix<double>.Build.Random(numberOfRegularRows + numberOfColumns, numberOfColumns);
      var param = Vector<double>.Build.Random(numberOfRegularRows + numberOfColumns);
      double lambda = 777;
      matrix.AddRegularization(param, numberOfRegularRows, lambda, 1, 0, 0, isCircular);

      for (int r = 0; r < numberOfColumns; r++)
      {
        Assert.Equal(0, param[numberOfRegularRows + r]);
        for (int c = 0; c < numberOfColumns; c++)
        {
          if (c != r)
            Assert.Equal(0, matrix[numberOfRegularRows + r, c]);
          else
            Assert.Equal(lambda, matrix[numberOfRegularRows + r, r]);
        }
      }
    }


    [Fact]
    public void TestFirstOrder_NotCircular()
    {
      int numberOfColumns = 5;
      int numberOfRegularRows = 7;
      bool isCircular = false;

      var matrix = Matrix<double>.Build.Random(numberOfRegularRows + numberOfColumns, numberOfColumns);
      var param = Vector<double>.Build.Random(numberOfRegularRows + numberOfColumns);
      double lambda = 777;
      var lambdaHalf = lambda / 2;
      matrix.AddRegularization(param, numberOfRegularRows, lambda, 2, 1, 1, isCircular);

      for (int r = 0; r < numberOfColumns; r++)
      {
        Assert.Equal(0, param[numberOfRegularRows + r]);
      }

      // check first row
      Assert.Equal(-lambdaHalf, matrix[numberOfRegularRows + 0, 0]);
      Assert.Equal(+lambdaHalf, matrix[numberOfRegularRows + 0, 1]);
      Assert.Equal(0, matrix[numberOfRegularRows + 0, 2]);
      Assert.Equal(0, matrix[numberOfRegularRows + 0, 3]);
      Assert.Equal(0, matrix[numberOfRegularRows + 0, 4]);

      // check 2nd row
      Assert.Equal(-lambdaHalf, matrix[numberOfRegularRows + 1, 0]);
      Assert.Equal(+lambdaHalf, matrix[numberOfRegularRows + 1, 1]);
      Assert.Equal(0, matrix[numberOfRegularRows + 1, 2]);
      Assert.Equal(0, matrix[numberOfRegularRows + 1, 3]);
      Assert.Equal(0, matrix[numberOfRegularRows + 1, 4]);

      // check 3rd row
      Assert.Equal(0, matrix[numberOfRegularRows + 2, 0]);
      Assert.Equal(-lambdaHalf, matrix[numberOfRegularRows + 2, 1]);
      Assert.Equal(+lambdaHalf, matrix[numberOfRegularRows + 2, 2]);
      Assert.Equal(0, matrix[numberOfRegularRows + 2, 3]);
      Assert.Equal(0, matrix[numberOfRegularRows + 2, 4]);

      // check 4th row
      Assert.Equal(0, matrix[numberOfRegularRows + 3, 0]);
      Assert.Equal(0, matrix[numberOfRegularRows + 3, 1]);
      Assert.Equal(-lambdaHalf, matrix[numberOfRegularRows + 3, 2]);
      Assert.Equal(+lambdaHalf, matrix[numberOfRegularRows + 3, 3]);
      Assert.Equal(0, matrix[numberOfRegularRows + 3, 4]);

      // check 5th row
      Assert.Equal(0, matrix[numberOfRegularRows + 4, 0]);
      Assert.Equal(0, matrix[numberOfRegularRows + 4, 1]);
      Assert.Equal(0, matrix[numberOfRegularRows + 4, 2]);
      Assert.Equal(-lambdaHalf, matrix[numberOfRegularRows + 4, 3]);
      Assert.Equal(+lambdaHalf, matrix[numberOfRegularRows + 4, 4]);

    }

    [Fact]
    public void TestFirstOrder_Circular()
    {
      int numberOfColumns = 5;
      int numberOfRegularRows = 7;
      bool isCircular = true;

      var matrix = Matrix<double>.Build.Random(numberOfRegularRows + numberOfColumns, numberOfColumns);
      var param = Vector<double>.Build.Random(numberOfRegularRows + numberOfColumns);
      double lambda = 777;
      var lambdaHalf = lambda / 2;
      matrix.AddRegularization(param, numberOfRegularRows, lambda, 2, 1, 1, isCircular);

      for (int r = 0; r < numberOfColumns; r++)
      {
        Assert.Equal(0, param[numberOfRegularRows + r]);
      }

      // check first row
      Assert.Equal(lambdaHalf, matrix[numberOfRegularRows + 0, 0]);
      Assert.Equal(0, matrix[numberOfRegularRows + 0, 1]);
      Assert.Equal(0, matrix[numberOfRegularRows + 0, 2]);
      Assert.Equal(0, matrix[numberOfRegularRows + 0, 3]);
      Assert.Equal(-lambdaHalf, matrix[numberOfRegularRows + 0, 4]);

      // check 2nd row
      Assert.Equal(-lambdaHalf, matrix[numberOfRegularRows + 1, 0]);
      Assert.Equal(lambdaHalf, matrix[numberOfRegularRows + 1, 1]);
      Assert.Equal(0, matrix[numberOfRegularRows + 1, 2]);
      Assert.Equal(0, matrix[numberOfRegularRows + 1, 3]);
      Assert.Equal(0, matrix[numberOfRegularRows + 1, 4]);

      // check 3rd row
      Assert.Equal(0, matrix[numberOfRegularRows + 2, 0]);
      Assert.Equal(-lambdaHalf, matrix[numberOfRegularRows + 2, 1]);
      Assert.Equal(lambdaHalf, matrix[numberOfRegularRows + 2, 2]);
      Assert.Equal(0, matrix[numberOfRegularRows + 2, 3]);
      Assert.Equal(0, matrix[numberOfRegularRows + 2, 4]);

      // check 4th row
      Assert.Equal(0, matrix[numberOfRegularRows + 3, 0]);
      Assert.Equal(0, matrix[numberOfRegularRows + 3, 1]);
      Assert.Equal(-lambdaHalf, matrix[numberOfRegularRows + 3, 2]);
      Assert.Equal(lambdaHalf, matrix[numberOfRegularRows + 3, 3]);
      Assert.Equal(0, matrix[numberOfRegularRows + 3, 4]);

      // check 5th row
      Assert.Equal(0, matrix[numberOfRegularRows + 4, 0]);
      Assert.Equal(0, matrix[numberOfRegularRows + 4, 1]);
      Assert.Equal(0, matrix[numberOfRegularRows + 4, 2]);
      Assert.Equal(-lambdaHalf, matrix[numberOfRegularRows + 4, 3]);
      Assert.Equal(lambdaHalf, matrix[numberOfRegularRows + 4, 4]);

    }



    [Fact]
    public void TestSecondOrder_NotCircular()
    {
      int numberOfColumns = 5;
      int numberOfRegularRows = 7;
      bool isCircular = false;

      var matrix = Matrix<double>.Build.Random(numberOfRegularRows + numberOfColumns, numberOfColumns);
      var param = Vector<double>.Build.Random(numberOfRegularRows + numberOfColumns);
      double lambda = 777;
      matrix.AddRegularization(param, numberOfRegularRows, lambda, 3, 2, 2, isCircular);

      for (int r = 0; r < numberOfColumns; r++)
      {
        Assert.Equal(0, param[numberOfRegularRows + r]);
      }

      // check first row
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 0, 0], 0, 1E-10);
      AssertEx.AreEqual(-lambda, matrix[numberOfRegularRows + 0, 1], 0, 1E-10);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 0, 2], 0, 1E-10);
      Assert.Equal(0, matrix[numberOfRegularRows + 0, 3]);
      Assert.Equal(0, matrix[numberOfRegularRows + 0, 4]);

      // check 2nd row
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 1, 0], 0, 1E-10);
      AssertEx.AreEqual(-lambda, matrix[numberOfRegularRows + 1, 1], 0, 1E-10);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 1, 2], 0, 1E-10);
      Assert.Equal(0, matrix[numberOfRegularRows + 1, 3]);
      Assert.Equal(0, matrix[numberOfRegularRows + 1, 4]);

      // check 3rd row
      Assert.Equal(0, matrix[numberOfRegularRows + 2, 0]);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 2, 1], 0, 1E-10);
      AssertEx.AreEqual(-lambda, matrix[numberOfRegularRows + 2, 2], 0, 1E-10);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 2, 3], 0, 1E-10);
      Assert.Equal(0, matrix[numberOfRegularRows + 2, 4]);

      // check 4th row
      Assert.Equal(0, matrix[numberOfRegularRows + 3, 0]);
      Assert.Equal(0, matrix[numberOfRegularRows + 3, 1]);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 3, 2], 0, 1E-10);
      AssertEx.AreEqual(-lambda, matrix[numberOfRegularRows + 3, 3], 0, 1E-10);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 3, 4], 0, 1E-10);

      // check 5th row
      Assert.Equal(0, matrix[numberOfRegularRows + 4, 0]);
      Assert.Equal(0, matrix[numberOfRegularRows + 4, 1]);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 4, 2], 0, 1E-10);
      AssertEx.AreEqual(-lambda, matrix[numberOfRegularRows + 4, 3], 0, 1E-10);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 4, 4], 0, 1E-10);

    }

    [Fact]
    public void TestSecondOrder_Circular()
    {
      int numberOfColumns = 5;
      int numberOfRegularRows = 7;
      bool isCircular = true;

      var matrix = Matrix<double>.Build.Random(numberOfRegularRows + numberOfColumns, numberOfColumns);
      var param = Vector<double>.Build.Random(numberOfRegularRows + numberOfColumns);
      double lambda = 777;
      matrix.AddRegularization(param, numberOfRegularRows, lambda, 3, 2, 2, isCircular);

      for (int r = 0; r < numberOfColumns; r++)
      {
        Assert.Equal(0, param[numberOfRegularRows + r]);
      }

      // check first row
      AssertEx.AreEqual(-lambda, matrix[numberOfRegularRows + 0, 0], 0, 1E-10);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 0, 1], 0, 1E-10);
      Assert.Equal(0, matrix[numberOfRegularRows + 0, 2]);
      Assert.Equal(0, matrix[numberOfRegularRows + 0, 3]);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 4, 0], 0, 1E-10);

      // check 2nd row
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 1, 0], 0, 1E-10);
      AssertEx.AreEqual(-lambda, matrix[numberOfRegularRows + 1, 1], 0, 1E-10);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 1, 2], 0, 1E-10);
      Assert.Equal(0, matrix[numberOfRegularRows + 1, 3]);
      Assert.Equal(0, matrix[numberOfRegularRows + 1, 4]);

      // check 3rd row
      Assert.Equal(0, matrix[numberOfRegularRows + 2, 0]);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 2, 1], 0, 1E-10);
      AssertEx.AreEqual(-lambda, matrix[numberOfRegularRows + 2, 2], 0, 1E-10);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 2, 3], 0, 1E-10);
      Assert.Equal(0, matrix[numberOfRegularRows + 2, 4]);

      // check 4th row
      Assert.Equal(0, matrix[numberOfRegularRows + 3, 0]);
      Assert.Equal(0, matrix[numberOfRegularRows + 3, 1]);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 3, 2], 0, 1E-10);
      AssertEx.AreEqual(-lambda, matrix[numberOfRegularRows + 3, 3], 0, 1E-10);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 3, 4], 0, 1E-10);

      // check 5th row
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 4, 0], 0, 1E-10);
      Assert.Equal(0, matrix[numberOfRegularRows + 4, 1]);
      Assert.Equal(0, matrix[numberOfRegularRows + 4, 2]);
      AssertEx.AreEqual(lambda / 2, matrix[numberOfRegularRows + 4, 3], 0, 1E-10);
      AssertEx.AreEqual(-lambda, matrix[numberOfRegularRows + 4, 4], 0, 1E-10);

    }
  }
}
