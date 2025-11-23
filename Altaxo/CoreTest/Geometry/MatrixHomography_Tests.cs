#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

using System.Linq;
using Xunit;

namespace Altaxo.Geometry
{
  public class MatrixHomography_Tests
  {
    private static PointD3D[] _base = new PointD3D[]
    {
      new PointD3D(0, 0, 0),
      new PointD3D(1, 0, 0),
      new PointD3D(0, 1, 0),
      new PointD3D(1, 1, 0),
      new PointD3D(0, 0, 1),
      new PointD3D(1, 0, 1),
      new PointD3D(0, 1, 1),
      new PointD3D(1, 1, 1)
    };

    [Fact]
    public void TestIndentity()
    {
      var m = MatrixHomography.EvaluateHomography(_base.Select(x => (x, x)).ToArray());

      AssertEx.Equal(1.0, m.M11, 1E-9);
      AssertEx.Equal(1.0, m.M22, 1E-9);
      AssertEx.Equal(1.0, m.M33, 1E-9);
      AssertEx.Equal(1.0, m.M44, 1E-9);

      AssertEx.Equal(0.0, m.M12, 1E-9);
      AssertEx.Equal(0.0, m.M13, 1E-9);
      AssertEx.Equal(0.0, m.M14, 1E-9);

      AssertEx.Equal(0.0, m.M21, 1E-9);
      AssertEx.Equal(0.0, m.M23, 1E-9);
      AssertEx.Equal(0.0, m.M24, 1E-9);

      AssertEx.Equal(0.0, m.M31, 1E-9);
      AssertEx.Equal(0.0, m.M32, 1E-9);
      AssertEx.Equal(0.0, m.M34, 1E-9);

      AssertEx.Equal(0.0, m.M41, 1E-9);
      AssertEx.Equal(0.0, m.M42, 1E-9);
      AssertEx.Equal(0.0, m.M43, 1E-9);

    }

    [Fact]
    public void TestScaling()
    {
      var m = MatrixHomography.EvaluateHomography(_base.Select(x => (x, new PointD3D(x.X * 3, x.Y * 5, x.Z * 7))).ToArray());

      AssertEx.Equal(3.0, m.M11, 1E-9);
      AssertEx.Equal(5.0, m.M22, 1E-9);
      AssertEx.Equal(7.0, m.M33, 1E-9);
      AssertEx.Equal(1.0, m.M44, 1E-9);

      AssertEx.Equal(0.0, m.M12, 1E-9);
      AssertEx.Equal(0.0, m.M13, 1E-9);
      AssertEx.Equal(0.0, m.M14, 1E-9);

      AssertEx.Equal(0.0, m.M21, 1E-9);
      AssertEx.Equal(0.0, m.M23, 1E-9);
      AssertEx.Equal(0.0, m.M24, 1E-9);

      AssertEx.Equal(0.0, m.M31, 1E-9);
      AssertEx.Equal(0.0, m.M32, 1E-9);
      AssertEx.Equal(0.0, m.M34, 1E-9);

      AssertEx.Equal(0.0, m.M41, 1E-9);
      AssertEx.Equal(0.0, m.M42, 1E-9);
      AssertEx.Equal(0.0, m.M43, 1E-9);
    }


    [Fact]
    public void TestRandom()
    {
      var rnd = new System.Random();

      double N() => 20 * (rnd.NextDouble() - 0.5);

      for (int i = 0; i < 10; ++i)
      {

        var n = new Matrix4x4(N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), 1);

        var pairs = _base.Select(x => (x, n.Transform(x))).ToArray();

        var m = MatrixHomography.EvaluateHomography(pairs);

        AssertEx.Equal(n.M11, m.M11, 1E-9);
        AssertEx.Equal(n.M12, m.M12, 1E-9);
        AssertEx.Equal(n.M13, m.M13, 1E-9);
        AssertEx.Equal(n.M14, m.M14, 1E-9);

        AssertEx.Equal(n.M21, m.M21, 1E-9);
        AssertEx.Equal(n.M22, m.M22, 1E-9);
        AssertEx.Equal(n.M23, m.M23, 1E-9);
        AssertEx.Equal(n.M24, m.M24, 1E-9);

        AssertEx.Equal(n.M31, m.M31, 1E-9);
        AssertEx.Equal(n.M32, m.M32, 1E-9);
        AssertEx.Equal(n.M33, m.M33, 1E-9);
        AssertEx.Equal(n.M34, m.M34, 1E-9);

        AssertEx.Equal(n.M41, m.M41, 1E-9);
        AssertEx.Equal(n.M42, m.M42, 1E-9);
        AssertEx.Equal(n.M43, m.M43, 1E-9);
        AssertEx.Equal(n.M44, m.M44, 1E-9);

        for (int k = 0; k < pairs.Length; k++)
        {
          var x = pairs[k].Item1;
          var y = pairs[k].Item2;
          var t = m.Transform(x);
          AssertEx.Equal(y.X, t.X, 1E-9);
          AssertEx.Equal(y.Y, t.Y, 1E-9);
          AssertEx.Equal(y.Z, t.Z, 1E-9);
        }

      }
    }

    [Fact]
    public void TestRandom2()
    {
      var rnd = new System.Random(nameof(TestRandom2).GetHashCode());

      double N() => 20 * (rnd.NextDouble() - 0.5);

      for (int i = 0; i < 10; ++i)
      {

        var n = new Matrix4x4(N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), 1);
        var s = n.Inverse();

        var pairs = _base.Select(x => (s.Transform(x), n.Transform(s.Transform(x)))).ToArray();

        var m = MatrixHomography.EvaluateHomography(pairs);

        AssertEx.Equal(n.M11, m.M11, 1E-9);
        AssertEx.Equal(n.M12, m.M12, 1E-9);
        AssertEx.Equal(n.M13, m.M13, 1E-9);
        AssertEx.Equal(n.M14, m.M14, 1E-9);

        AssertEx.Equal(n.M21, m.M21, 1E-9);
        AssertEx.Equal(n.M22, m.M22, 1E-9);
        AssertEx.Equal(n.M23, m.M23, 1E-9);
        AssertEx.Equal(n.M24, m.M24, 1E-9);

        AssertEx.Equal(n.M31, m.M31, 1E-9);
        AssertEx.Equal(n.M32, m.M32, 1E-9);
        AssertEx.Equal(n.M33, m.M33, 1E-9);
        AssertEx.Equal(n.M34, m.M34, 1E-9);

        AssertEx.Equal(n.M41, m.M41, 1E-9);
        AssertEx.Equal(n.M42, m.M42, 1E-9);
        AssertEx.Equal(n.M43, m.M43, 1E-9);
        AssertEx.Equal(n.M44, m.M44, 1E-9);

        for (int k = 0; k < pairs.Length; k++)
        {
          var x = pairs[k].Item1;
          var y = pairs[k].Item2;
          var t = m.Transform(x);
          AssertEx.Equal(y.X, t.X, 1E-9);
          AssertEx.Equal(y.Y, t.Y, 1E-9);
          AssertEx.Equal(y.Z, t.Z, 1E-9);
        }

      }
    }

    [Fact]
    public void TestAffine3x2()
    {
      var rnd = new System.Random();

      double N() => 20 * (rnd.NextDouble() - 0.5);

      for (int i = 0; i < 10; ++i)
      {
        var n = new Matrix3x2(N(), N(), N(), N(), N(), N());

        var pairs = new (PointD2D x, PointD2D y)[100];
        for (int j = 0; j < pairs.Length; ++j)
        {
          var x = new PointD2D(N(), N());
          var y = n.Transform(x);
          pairs[j] = (x, y);
        }


        var m = MatrixHomography.EvaluateAffine(pairs);

        AssertEx.Equal(n.M11, m.M11, 1E-9);
        AssertEx.Equal(n.M12, m.M12, 1E-9);
        AssertEx.Equal(n.M13, m.M13, 1E-9);

        AssertEx.Equal(n.M21, m.M21, 1E-9);
        AssertEx.Equal(n.M22, m.M22, 1E-9);
        AssertEx.Equal(n.M23, m.M23, 1E-9);

        AssertEx.Equal(n.M31, m.M31, 1E-9);
        AssertEx.Equal(n.M32, m.M32, 1E-9);
        AssertEx.Equal(n.M33, m.M33, 1E-9);
      }
    }

    [Fact]
    public void TestAffine4x3()
    {
      var rnd = new System.Random();

      double N() => 20 * (rnd.NextDouble() - 0.5);

      for (int i = 0; i < 10; ++i)
      {
        var n = new Matrix4x3(N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), N(), N());

        var pairs = new (PointD3D x, PointD3D y)[100];
        for (int j = 0; j < pairs.Length; ++j)
        {
          var x = new PointD3D(N(), N(), N());
          var y = n.Transform(x);
          pairs[j] = (x, y);
        }


        var m = MatrixHomography.EvaluateAffine(pairs);

        AssertEx.Equal(n.M11, m.M11, 1E-9);
        AssertEx.Equal(n.M12, m.M12, 1E-9);
        AssertEx.Equal(n.M13, m.M13, 1E-9);

        AssertEx.Equal(n.M21, m.M21, 1E-9);
        AssertEx.Equal(n.M22, m.M22, 1E-9);
        AssertEx.Equal(n.M23, m.M23, 1E-9);

        AssertEx.Equal(n.M31, m.M31, 1E-9);
        AssertEx.Equal(n.M32, m.M32, 1E-9);
        AssertEx.Equal(n.M33, m.M33, 1E-9);

        AssertEx.Equal(n.M41, m.M41, 1E-9);
        AssertEx.Equal(n.M42, m.M42, 1E-9);
        AssertEx.Equal(n.M43, m.M43, 1E-9);

      }
    }
  }
}
