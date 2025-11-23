using System;
using Xunit;

namespace Altaxo.Geometry
{
  public class Line2D_Tests
  {
    [Fact]
    public void TestIntersection()
    {
      const double expectedX = 3;
      const double expectedY = 5;

      // to lines with integer points
      var l0 = new LineD2D(new PointD2D(expectedX + 5, expectedY + 5), new PointD2D(expectedX + 7, expectedY + 7));
      var l1 = new LineD2D(new PointD2D(expectedX + 1, expectedY + 2), new PointD2D(expectedX + 3, expectedY + 6));

      var ps1 = LineD2D.Intersection(l0, l1);
      Assert.True(ps1.HasValue);
      AssertEx.AreEqual(expectedX, ps1.Value.X, 1E-9, 1E-9);
      AssertEx.AreEqual(expectedY, ps1.Value.Y, 1E-9, 1E-9);


      // two lines with arbitrary numbers
      var phi0 = 0.1;
      var phi1 = 2.5;

      var r00 = 11;
      var r01 = -8;
      var r10 = 3;
      var r11 = 13;

      l0 = new LineD2D(new PointD2D(expectedX + r00 * Math.Cos(phi0), expectedY + r00 * Math.Sin(phi0)), new PointD2D(expectedX + r01 * Math.Cos(phi0), expectedY + r01 * Math.Sin(phi0)));
      l1 = new LineD2D(new PointD2D(expectedX + r10 * Math.Cos(phi1), expectedY + r10 * Math.Sin(phi1)), new PointD2D(expectedX + r11 * Math.Cos(phi1), expectedY + r11 * Math.Sin(phi1)));

      ps1 = LineD2D.Intersection(l0, l1);
      Assert.True(ps1.HasValue);
      AssertEx.AreEqual(expectedX, ps1.Value.X, 1E-9, 1E-9);
      AssertEx.AreEqual(expectedY, ps1.Value.Y, 1E-9, 1E-9);

      // two lines which are parallel should result in null
      l0 = new LineD2D(new PointD2D(2, 2), new PointD2D(17, 7));
      var v = new VectorD2D(0, 1);
      var l2 = new LineD2D(l0.P0 + v, l0.P1 + v);
      var ps2 = LineD2D.Intersection(l0, l2);
      Assert.False(ps2.HasValue);
    }
  }
}
