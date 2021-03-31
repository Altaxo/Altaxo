#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using System.Globalization;
using System.Threading;
using Xunit;

namespace Altaxo.Calc.Ode.Obsolete
{

  public class VectorTests
  {
    private const double Eps = 1e-10;

    [Fact]
    public void MulAddTest()
    {
      var x = new Vector(1.0, 0.0, 1.0);
      var y = new Vector(2.0, 2.0, -2.0);
      x.MulAdd(y, 0.5);
      AssertVectorEqualsEps(x, new Vector(2.0, 1.0, 0.0));
    }

    [Fact]
    public void LerpTest()
    {
      var v0 = new Vector(-1);
      var v1 = new Vector(1);
      AssertEx.Equal(Vector.Lerp(0, 0, v0, 1, v1)[0], -1, Eps);
      AssertEx.Equal(Vector.Lerp(1 / 3.0, 0, v0, 1, v1)[0], -1 / 3.0, Eps);
      AssertEx.Equal(Vector.Lerp(2 / 3.0, 0, v0, 1, v1)[0], 1 / 3.0, Eps);
      AssertEx.Equal(Vector.Lerp(1, 0, v0, 1, v1)[0], 1, Eps);
    }

    [Fact]
    public void AbsTest()
    {
      Vector v = new Vector(-1, 1, -0.5).Abs();
      Assert.Equal(1, v[0]);
      Assert.Equal(1, v[1]);
      Assert.Equal(0.5, v[2]);
    }

    [Fact]
    public void EuclideanNormTest()
    {
      AssertEx.Equal(new Vector(-1, 2, -3).EuclideanNorm, Math.Sqrt(14), Eps);
      Assert.Equal(Vector.GetEuclideanNorm(new Vector(-1, 2, -3), new Vector(1, -1, 2)), Math.Sqrt(38));
    }

    [Fact]
    public void MaxTest()
    {
      var m = Vector.Max(new Vector(-2, 1, 3), new Vector(3, -2, 3));
      Assert.Equal(3, m[0]);
      Assert.Equal(1, m[1]);
      Assert.Equal(3, m[2]);
    }

    [Fact]
    public void ToStringTest()
    {
      var ci = Thread.CurrentThread.CurrentCulture;
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
      try
      {
        Assert.Equal("[1.5]", new Vector(1.5).ToString());
        Assert.Equal("[1.2, -2.3]", new Vector(1.2, -2.3).ToString());
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = ci;
      }
    }

    [Fact]
    public void SumTest()
    {
      AssertEx.Equal(new Vector(-1.0, 3.0, -2.0).Sum, 0.0, Eps);
    }

    [Fact]
    public void ArithmeticTest()
    {
      // Element-wise operations
      var a = new Vector(-1, 2);
      var b = new Vector(3, -4);

      var sum = a + b;
      AssertEx.Equal(sum[0], 2, Eps);
      AssertEx.Equal(sum[1], -2, Eps);

      var diff = a - b;
      AssertEx.Equal(diff[0], -4, Eps);
      AssertEx.Equal(diff[1], 6, Eps);

      var div = a / b;
      AssertEx.Equal(div[0], -1 / 3.0, Eps);
      AssertEx.Equal(div[1], -0.5, Eps);

      // (Vector, scalar) operations
      var prod = a * 2.5;
      AssertEx.Equal(prod[0], -2.5, Eps);
      AssertEx.Equal(prod[1], 5, Eps);

      var div2 = a / 3.0;
      AssertEx.Equal(div2[0], -1 / 3.0, Eps);
      AssertEx.Equal(div2[1], 2 / 3.0, Eps);

      var sum2 = a + 3.0;
      AssertEx.Equal(sum2[0], 2.0, Eps);
      AssertEx.Equal(sum2[1], 5.0, Eps);

      // Dot product
      AssertEx.Equal(a * b, -11, Eps);
    }

    [Fact]
    public void VectorMatrixMultiplyTest()
    {
      var v = new Vector(-1, 1);
      var m = new Matrix(new double[,] { // 3x2 matrix
                { -2,3 },
                { 3,-4 },
                {1, 2 }
            });

      Vector vm = m * v;
      AssertEx.Equal(vm[0], 5, Eps);
      AssertEx.Equal(vm[1], -7, Eps);
      AssertEx.Equal(vm[2], 1, Eps);

      var m2 = m.Transpose(); // 2x3 matrix
      vm = v * m2;
      AssertEx.Equal(vm[0], 5, Eps);
      AssertEx.Equal(vm[1], -7, Eps);
      AssertEx.Equal(vm[2], 1, Eps);
    }

    [Fact]
    public void EqualsTest()
    {
      var v1 = new Vector(-1, 2);
      var v2 = new Vector(3, 2, -5);
      var v3 = new Vector(-1, 2);
      var v4 = new Vector(3, 2, 10);

      Assert.True(v1.Equals(v3));
      Assert.True(v2.Equals(v2));
      Assert.False(v2.Equals(v4));
      Assert.False(v1.Equals(v2));
    }

    private void AssertVectorEqualsEps(Vector A, Vector B)
    {
      double sum = 0.0;
      for (int i = 0; i < A.Length; i++)
      {
        sum += A[i] - B[i];
      }

      AssertEx.Equal(sum, 0.0, Eps);
    }
  }
}
