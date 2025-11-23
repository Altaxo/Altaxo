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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Altaxo.Geometry
{
  public class QuadTree_Tests
  {
    [Fact]
    public void AdditionAndRetrievalSmallRects()
    {
      var rnd = new Random();
      var quadTree = new QuadTree<RectangleD2D>(new RectangleD2D(-1,-1,102,102), x => x);
      for(int i=0;i<=100;++i)
        for(int j=0;j<=100;++j)
          quadTree.Add(new RectangleD2D(i,j,1E-6,1E-6));

      Assert.Equal(101 * 101, quadTree.Count);


      for(int i=0;i<1000;++i)
      {
        var x = rnd.Next(5, 101 - 5);
        var y = rnd.Next(5, 101 - 5);
        var rect = new RectangleD2D(x - 5 - 1E-3, y - 5 - 1E-3, 10 + 2E-3, 10 + 2E-3);
        var items = quadTree.GetItems(rect);
        Assert.Equal(11 * 11, items.Count);

        foreach(var item in items)
        {
          Assert.True(rect.Contains(item));
        }
      }
    }


    [Fact]
    public void AdditionAndRetrievalBigRects()
    {
      var rnd = new Random();
      var quadTree = new QuadTree<RectangleD2D>(new RectangleD2D(-1, -1, 102, 102), x => x);
      for (int i = 0; i <= 100; ++i)
        for (int j = 0; j <= 100; ++j)
          quadTree.Add(new RectangleD2D(i-9.5, j-9.5, 19, 19));

      Assert.Equal(101 * 101, quadTree.Count);


      for (int i = 0; i < 1000; ++i)
      {
        var x = rnd.Next(9, 100 - 9);
        var y = rnd.Next(9, 100 - 9);
        var rect = new RectangleD2D(x - 1E-3, y - 1E-3, 2E-3, 2E-3);
        var items = quadTree.GetItems(rect);
        Assert.Equal(19 * 19, items.Count);

        foreach (var item in items)
        {
          Assert.True(rect.IntersectsWith(item));
        }
      }
    }

    [Fact]
    public void RandomAdditionAndRetrieval()
    {
      var rnd = new Random(nameof(RandomAdditionAndRetrieval).GetHashCode());

      var points = new HashSet<(int x, int y)>();

      for(int i=0;points.Count<500000;++i)
      {
        var x = rnd.Next(500, 20000 - 500);
        var y = rnd.Next(500, 60000 - 500);
        points.Add((x, y));
      }

      var quad = new QuadTree<(int x, int y)>(new RectangleD2D(0, 0, 20000, 60000), p => new RectangleD2D(p.x, p.y, 1, 1));

      foreach (var p in points)
        quad.Add(p);

      Assert.Equal(points.Count, quad.Count);

      var list = quad.GetAllItems();
      Assert.Equal(points.Count, list.Count);

      var hash2 = new HashSet<(int x, int y)>(list);
      Assert.Equal(points.Count, hash2.Count);

      // try to retrieve each of the items

      list = new List<(int x, int y)>();
      foreach(var p in points)
      {
        list.Clear();
        quad.GetItems(new RectangleD2D(p.x - 25, p.y - 25, 50, 50), list);
        Assert.True(list.Count >= 1);
      }

    }
  }
}
