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
      Assert.False(rect.Contains(new PointD2D(left-1, upper)));  // right be-lower should not be included
      Assert.False(rect.Contains(new PointD2D(left-1, ymid)));  // right lower should not be included
      Assert.False(rect.Contains(new PointD2D(left-1, lower)));  // lower edge should not be included

      // 1 to the top
      Assert.False(rect.Contains(new PointD2D(left, upper-1)));
      Assert.False(rect.Contains(new PointD2D(xmid, upper-1)));
      Assert.False(rect.Contains(new PointD2D(right, upper-1)));

      // 1 to the right
      Assert.False(rect.Contains(new PointD2D(right+1, upper)));  // right be-lower should not be included
      Assert.False(rect.Contains(new PointD2D(right+1, ymid)));  // right lower should not be included
      Assert.False(rect.Contains(new PointD2D(right-1, lower)));  // lower edge should not be included

      // 1 to the bottom
      Assert.False(rect.Contains(new PointD2D(left, lower+1)));
      Assert.False(rect.Contains(new PointD2D(xmid, lower+1)));
      Assert.False(rect.Contains(new PointD2D(right, lower+1)));

    }
  }
}
