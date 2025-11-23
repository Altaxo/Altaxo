using System;
using Xunit;

namespace Altaxo.Geometry
{
  public class Matrix3x2_Tests
  {
    [Fact]
    public void RotationAroundCenterTest()
    {
      var p = new PointD2D(4, 5);

      var m = Matrix3x2.NewRotationRadian(new PointD2D(3, 5), Math.PI / 2);
      var q = m.Transform(p);

      AssertEx.AreEqual(3, q.X, 1E-12, 1E-12);
      AssertEx.AreEqual(6, q.Y, 1E-12, 1E-12);

      p = new PointD2D(4, 6);
      m = Matrix3x2.NewRotationRadian(new PointD2D(3, 5), Math.PI / 2);
      q = m.Transform(p);
      AssertEx.AreEqual(2, q.X, 1E-12, 1E-12);
      AssertEx.AreEqual(6, q.Y, 1E-12, 1E-12);

      p = new PointD2D(4, 6);
      m = Matrix3x2.NewRotationRadian(new PointD2D(3, 5), Math.PI);
      q = m.Transform(p);
      AssertEx.AreEqual(2, q.X, 1E-12, 1E-12);
      AssertEx.AreEqual(4, q.Y, 1E-12, 1E-12);
    }
  }
}
