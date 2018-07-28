#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using NUnit.Framework;
using System;
using System.Globalization;
using System.Threading;

namespace Altaxo.Calc.Ode
{
  [TestFixture]
  public class VectorTests
  {
    private const double Eps = 1e-10;

    [Test]
    public void MulAddTest()
    {
      Vector x = new Vector(1.0, 0.0, 1.0);
      Vector y = new Vector(2.0, 2.0, -2.0);
      x.MulAdd(y, 0.5);
      AssertVectorEqualsEps(x, new Vector(2.0, 1.0, 0.0));
    }

    [Test]
    public void LerpTest()
    {
      Vector v0 = new Vector(-1);
      Vector v1 = new Vector(1);
      Assert.AreEqual(Vector.Lerp(0, 0, v0, 1, v1)[0], -1, Eps);
      Assert.AreEqual(Vector.Lerp(1 / 3.0, 0, v0, 1, v1)[0], -1 / 3.0, Eps);
      Assert.AreEqual(Vector.Lerp(2 / 3.0, 0, v0, 1, v1)[0], 1 / 3.0, Eps);
      Assert.AreEqual(Vector.Lerp(1, 0, v0, 1, v1)[0], 1, Eps);
    }

    [Test]
    public void AbsTest()
    {
      Vector v = new Vector(-1, 1, -0.5).Abs();
      Assert.AreEqual(v[0], 1);
      Assert.AreEqual(v[1], 1);
      Assert.AreEqual(v[2], 0.5);
    }

    [Test]
    public void EuclideanNormTest()
    {
      Assert.AreEqual(new Vector(-1, 2, -3).EuclideanNorm, Math.Sqrt(14), Eps);
      Assert.AreEqual(Vector.GetEuclideanNorm(new Vector(-1, 2, -3), new Vector(1, -1, 2)), Math.Sqrt(38));
    }

    [Test]
    public void MaxTest()
    {
      var m = Vector.Max(new Vector(-2, 1, 3), new Vector(3, -2, 3));
      Assert.AreEqual(m[0], 3);
      Assert.AreEqual(m[1], 1);
      Assert.AreEqual(m[2], 3);
    }

    [Test]
    public void ToStringTest()
    {
      var ci = Thread.CurrentThread.CurrentCulture;
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
      try
      {
        Assert.AreEqual(new Vector(1.5).ToString(), "[1.5]");
        Assert.AreEqual(new Vector(1.2, -2.3).ToString(), "[1.2, -2.3]");
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = ci;
      }
    }

    [Test]
    public void SumTest()
    {
      Assert.AreEqual(new Vector(-1.0, 3.0, -2.0).Sum, 0.0, Eps);
    }

    [Test]
    public void ArithmeticTest()
    {
      // Element-wise operations
      var a = new Vector(-1, 2);
      var b = new Vector(3, -4);

      var sum = a + b;
      Assert.AreEqual(sum[0], 2, Eps);
      Assert.AreEqual(sum[1], -2, Eps);

      var diff = a - b;
      Assert.AreEqual(diff[0], -4, Eps);
      Assert.AreEqual(diff[1], 6, Eps);

      var div = a / b;
      Assert.AreEqual(div[0], -1 / 3.0, Eps);
      Assert.AreEqual(div[1], -0.5, Eps);

      // (Vector, scalar) operations
      var prod = a * 2.5;
      Assert.AreEqual(prod[0], -2.5, Eps);
      Assert.AreEqual(prod[1], 5, Eps);

      var div2 = a / 3.0;
      Assert.AreEqual(div2[0], -1 / 3.0, Eps);
      Assert.AreEqual(div2[1], 2 / 3.0, Eps);

      var sum2 = a + 3.0;
      Assert.AreEqual(sum2[0], 2.0, Eps);
      Assert.AreEqual(sum2[1], 5.0, Eps);

      // Dot product
      Assert.AreEqual(a * b, -11, Eps);
    }

    [Test]
    public void VectorMatrixMultiplyTest()
    {
      Vector v = new Vector(-1, 1);
      Matrix m = new Matrix(new double[,] { // 3x2 matrix
                { -2,3 },
                { 3,-4 },
                {1, 2 }
            });

      Vector vm = m * v;
      Assert.AreEqual(vm[0], 5, Eps);
      Assert.AreEqual(vm[1], -7, Eps);
      Assert.AreEqual(vm[2], 1, Eps);

      var m2 = m.Transpose(); // 2x3 matrix
      vm = v * m2;
      Assert.AreEqual(vm[0], 5, Eps);
      Assert.AreEqual(vm[1], -7, Eps);
      Assert.AreEqual(vm[2], 1, Eps);
    }

    [Test]
    public void EqualsTest()
    {
      var v1 = new Vector(-1, 2);
      var v2 = new Vector(3, 2, -5);
      var v3 = new Vector(-1, 2);
      var v4 = new Vector(3, 2, 10);

      Assert.IsTrue(v1.Equals(v3));
      Assert.IsTrue(v2.Equals(v2));
      Assert.IsFalse(v2.Equals(v4));
      Assert.IsFalse(v1.Equals(v2));
    }

    private void AssertVectorEqualsEps(Vector A, Vector B)
    {
      double sum = 0.0;
      for (int i = 0; i < A.Length; i++)
      {
        sum += A[i] - B[i];
      }

      Assert.AreEqual(sum, 0.0, Eps);
    }
  }
}
