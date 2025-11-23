#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

namespace Altaxo.Geometry.Double_2D
{
  
  public class GrahamScan_Test : PolygonTestBase
  {
    private Random _random = new Random(1);

    [Fact]
    public void Test_Includeness()
    {
      var hash = new HashSet<(int, int)>();

      for (var numberOfTests = 0; numberOfTests < 100; ++numberOfTests)
      {
        var numberOfPoints = 20 + numberOfTests * 10;
        var arr = new PointD2DAnnotated[numberOfPoints];

        hash.Clear();
        for (var i = 0; i < numberOfPoints;)
        {
          var x = _random.Next(-1000, 1000);
          var y = _random.Next(-1000, 1000);

          if (!hash.Contains((x, y)))
          {
            hash.Add((x, y));
            arr[i] = new PointD2DAnnotated(x, y, i);
            ++i;
          }
        }

        var convexHull = GrahamScan.GetConvexHull(arr);

        IncludenessTest(convexHull, arr);
      }
    }
  }
}
