using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Altaxo.Geometry
{
  public class RectangleD2D_Tests
  {
    [Fact]
    public static void TestContainsPointD2D()
    {
      const int left = -22;
      const int upper = -33;
      const int w = 77;
      const int h = 10;
      const int right = left + w;
      const int lower = upper + h;
      const int xmid = (left + right) / 2;
      const int ymid = (upper + lower) / 2;


      var rect = new RectangleD2D(left, upper, w, h);

      // note: from the 4 corners of the rectangle, only the left upper point should be included in the hit test,
      // as well as the left edge and the right edge
      // the other three corners should not be included!
      // in this way, if 4 rectangles come together, only the rectangle with the left upper point as joining point will win the hit test


      Assert.True(rect.Contains(new PointD2D(left, upper)));  // left upper should be included
      Assert.False(rect.Contains(new PointD2D(left, lower)));  // left lower should not be included
      Assert.False(rect.Contains(new PointD2D(right, upper)));  // right upper should not be included
      Assert.False(rect.Contains(new PointD2D(right, lower)));  // right lower should not be included


      Assert.True(rect.Contains(new PointD2D(xmid, ymid)));  // middle middle should be included
      Assert.True(rect.Contains(new PointD2D(left, ymid)));  // left middle should be included
      Assert.True(rect.Contains(new PointD2D(xmid, upper)));  // middle upper should be included
      Assert.False(rect.Contains(new PointD2D(right, ymid)));  // right middle should not be included
      Assert.False(rect.Contains(new PointD2D(xmid, lower)));  // middle lower should not be included


      // 1 to the left
      Assert.False(rect.Contains(new PointD2D(left - 1, upper)));  // right be-lower should not be included
      Assert.False(rect.Contains(new PointD2D(left - 1, ymid)));  // right lower should not be included
      Assert.False(rect.Contains(new PointD2D(left - 1, lower)));  // lower edge should not be included

      // 1 to the top
      Assert.False(rect.Contains(new PointD2D(left, upper - 1)));
      Assert.False(rect.Contains(new PointD2D(xmid, upper - 1)));
      Assert.False(rect.Contains(new PointD2D(right, upper - 1)));

      // 1 to the right
      Assert.False(rect.Contains(new PointD2D(right + 1, upper)));  // right be-lower should not be included
      Assert.False(rect.Contains(new PointD2D(right + 1, ymid)));  // right lower should not be included
      Assert.False(rect.Contains(new PointD2D(right - 1, lower)));  // lower edge should not be included

      // 1 to the bottom
      Assert.False(rect.Contains(new PointD2D(left, lower + 1)));
      Assert.False(rect.Contains(new PointD2D(xmid, lower + 1)));
      Assert.False(rect.Contains(new PointD2D(right, lower + 1)));

    }

    [Fact]
    public static void TestIntersectsRectangleD2D()
    {
      const int x = -5;
      const int y = -7;
      const int w = 13;
      const int h = 17;

      var rect = new RectangleD2D(0, 0, w, h);

      // Test : neighboring rectangles should not be included
      for (int i = x - 2 * w; i <= x + 2 * w; ++i)
      {
        var r = new RectangleD2D(i, -h, w, h);
        Assert.False(rect.IntersectsWith(r));
        Assert.False(r.IntersectsWith(rect));

        r = new RectangleD2D(i, h, w, h);
        Assert.False(rect.IntersectsWith(r));
        Assert.False(r.IntersectsWith(rect));
      }

      for (int j = y - 2 * h; j <= y + 2 * h; ++j)
      {
        var r = new RectangleD2D(-w, j, w, h);
        Assert.False(rect.IntersectsWith(r));
        Assert.False(r.IntersectsWith(rect));

        r = new RectangleD2D(w, j, w, h);
        Assert.False(rect.IntersectsWith(r));
        Assert.False(r.IntersectsWith(rect));
      }
      // Test: if the left upper point is included, then the rectangle should be also included

      for (int i = x - 1 - w; i <= x + 1 + w; ++i)
      {
        for (int j = y - 1 - h; j <= y + 1 + h; ++j)
        {
          var r = new RectangleD2D(i, j, w, h);
          var c1 = rect.Contains(new PointD2D(r.X, r.Y)) || rect.Contains(new PointD2D(r.X + w - 1e-3, r.Y)) || rect.Contains(new PointD2D(r.X, r.Y + h - 1e-3)) || rect.Contains(new PointD2D(r.X + w - 1e-3, r.Y + h - 1e-3));
          var c2 = r.Contains(new PointD2D(rect.X, rect.Y)) || r.Contains(new PointD2D(rect.X + w - 1e-3, rect.Y)) || r.Contains(new PointD2D(rect.X, rect.Y + h - 1e-3)) || r.Contains(new PointD2D(rect.X + w - 1e-3, rect.Y + h - 13 - 3));

          if (c1 && c2)
          {
            Assert.True(rect.IntersectsWith(r));
            Assert.True(r.IntersectsWith(rect));
          }
          else if (!c1 && !c2)
          {
            Assert.False(rect.IntersectsWith(r));
            Assert.False(r.IntersectsWith(rect));
          }
        }
      }
    }

    [Fact]
    public static void TestIntersectsRectangleD2D_Test2()
    {

      var rect = new RectangleD2D(0, 0, 10, 2);

      // First, an x-Scan
      for (int i = -10; i <= 20; ++i)
      {
        var r = new RectangleD2D(i, -5, 2, 10);

        if (-1 <= i && i <= 9)
        {
          Assert.True(rect.IntersectsWith(r));
          Assert.True(r.IntersectsWith(rect));
        }
        else
        {
          Assert.False(rect.IntersectsWith(r));
          Assert.False(r.IntersectsWith(rect));
        }
      }

      // First, an y-Scan
      for (int j = -20; j <= 30; ++j)
      {
        var r = new RectangleD2D(5, j, 2, 10);

        if (-9 <= j && j <= 1)
        {
          Assert.True(rect.IntersectsWith(r));
          Assert.True(r.IntersectsWith(rect));
        }
        else
        {
          Assert.False(rect.IntersectsWith(r));
          Assert.False(r.IntersectsWith(rect));
        }
      }
    }


    [Fact]
    public static void TestIntersectsRectangleD2D_Test3()
    {
      // an almost empty rectangle should behave like a point
      const int x = -5, y = -7, w = 13, h = 17;

      var rect = new RectangleD2D(x, y, w, h);

      // First, an x-Scan
      for (int i = x - 2 * w; i <= x + 2 * w; ++i)
      {
        for (int j = y - 2 * h; j <= y + 2 * h; ++j)
        {
          var r = new RectangleD2D(i, j, 1e-14, 1e-14);
          var expectedTrue = rect.Contains(new PointD2D(r.X, r.Y));
          if (expectedTrue)
            Assert.True(rect.IntersectsWith(r));
          else
            Assert.False(rect.IntersectsWith(r));
        }
      }
    }
  }
}
