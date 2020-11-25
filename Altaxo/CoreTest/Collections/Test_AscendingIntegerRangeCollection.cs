#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using Altaxo.Collections;
using Xunit;

namespace AltaxoTest.Collections
{

  public class Test_AscendingIntegerRangeCollection
  {
    private AscendingIntegerRangeCollection PrepareCollection_2_3()
    {
      var result = new AscendingIntegerRangeCollection();
      result.AddRangeByFirstAndLastInclusive(2, 3);
      return result;
    }

    private AscendingIntegerRangeCollection PrepareCollection_10_20()
    {
      var result = new AscendingIntegerRangeCollection();
      result.AddRangeByFirstAndLastInclusive(10, 20);
      return result;
    }

    private AscendingIntegerRangeCollection PrepareCollection_10_20_30_40()
    {
      var result = new AscendingIntegerRangeCollection();
      result.AddRangeByFirstAndLastInclusive(10, 20);
      result.AddRangeByFirstAndLastInclusive(30, 40);
      return result;
    }

    private AscendingIntegerRangeCollection PrepareCollection_10_50_60_100()
    {
      var result = new AscendingIntegerRangeCollection();
      result.AddRangeByFirstAndLastInclusive(10, 50);
      result.AddRangeByFirstAndLastInclusive(60, 100);
      return result;
    }

    private AscendingIntegerRangeCollection PrepareCollection_10_20_30_40_50_60()
    {
      var result = new AscendingIntegerRangeCollection();
      result.AddRangeByFirstAndLastInclusive(10, 20);
      result.AddRangeByFirstAndLastInclusive(30, 40);
      result.AddRangeByFirstAndLastInclusive(50, 60);
      return result;
    }

    private AscendingIntegerRangeCollection PrepareCollection_1000_Max()
    {
      var result = new AscendingIntegerRangeCollection();
      result.AddRangeByFirstAndLastInclusive(1000, int.MaxValue);
      return result;
    }

    private AscendingIntegerRangeCollection PrepareCollection_Min_M1000()
    {
      var result = new AscendingIntegerRangeCollection();
      result.AddRangeByFirstAndLastInclusive(int.MinValue, -1000);
      return result;
    }

    #region Add to int.Max and int.Min

    [Fact]
    public void TestAddToMinValue_1()
    {
      var coll = PrepareCollection_Min_M1000();
      coll.AddRangeByFirstAndLastInclusive(-500, 0);
      Assert.Equal(2, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Equal(2, ranges.Length);
      Assert.Equal(int.MinValue, ranges[0].First);
      Assert.Equal(-1000, ranges[0].LastInclusive);
      Assert.Equal(-500, ranges[1].First);
      Assert.Equal(0, ranges[1].LastInclusive);
    }

    [Fact]
    public void TestAddToMinValue_2()
    {
      var coll = PrepareCollection_Min_M1000();
      coll.AddRangeByFirstAndLastInclusive(int.MinValue, int.MinValue);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(int.MinValue, ranges[0].First);
      Assert.Equal(-1000, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestAddToMinValue_3()
    {
      var coll = new AscendingIntegerRangeCollection();
      coll.AddRangeByFirstAndLastInclusive(int.MinValue, int.MinValue);
      coll.AddRangeByFirstAndLastInclusive(int.MinValue, int.MinValue + 1);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(int.MinValue, ranges[0].First);
      Assert.Equal(int.MinValue + 1, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestAddToMinValue_4()
    {
      var coll = new AscendingIntegerRangeCollection();
      coll.AddRangeByFirstAndLastInclusive(int.MinValue, int.MinValue);
      coll.AddRangeByFirstAndLastInclusive(int.MinValue + 1, int.MinValue + 1);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(int.MinValue, ranges[0].First);
      Assert.Equal(int.MinValue + 1, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestAddToMinValue_5()
    {
      var coll = new AscendingIntegerRangeCollection();
      coll.AddRangeByFirstAndLastInclusive(int.MinValue, int.MinValue);
      coll.AddRangeByFirstAndLastInclusive(int.MinValue + 1, int.MinValue + 2);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(int.MinValue, ranges[0].First);
      Assert.Equal(int.MinValue + 2, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestAddToMinValue_51()
    {
      var coll = new AscendingIntegerRangeCollection();
      coll.AddRangeByFirstAndLastInclusive(int.MinValue + 1, int.MinValue + 2);
      coll.AddRangeByFirstAndLastInclusive(int.MinValue, int.MinValue);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(int.MinValue, ranges[0].First);
      Assert.Equal(int.MinValue + 2, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestAddToMinValue_6()
    {
      var coll = new AscendingIntegerRangeCollection();
      coll.AddRangeByFirstAndLastInclusive(int.MinValue, int.MinValue);
      coll.AddRangeByFirstAndLastInclusive(int.MinValue + 2, int.MinValue + 2);
      Assert.Equal(2, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Equal(2, ranges.Length);
      Assert.Equal(int.MinValue, ranges[0].First);
      Assert.Equal(int.MinValue, ranges[0].LastInclusive);
      Assert.Equal(int.MinValue + 2, ranges[1].First);
      Assert.Equal(int.MinValue + 2, ranges[1].LastInclusive);
    }

    [Fact]
    public void TestAddToMinValue_61()
    {
      var coll = new AscendingIntegerRangeCollection();
      coll.AddRangeByFirstAndLastInclusive(int.MinValue + 2, int.MinValue + 2);
      coll.AddRangeByFirstAndLastInclusive(int.MinValue, int.MinValue);
      Assert.Equal(2, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Equal(2, ranges.Length);
      Assert.Equal(int.MinValue, ranges[0].First);
      Assert.Equal(int.MinValue, ranges[0].LastInclusive);
      Assert.Equal(int.MinValue + 2, ranges[1].First);
      Assert.Equal(int.MinValue + 2, ranges[1].LastInclusive);
    }

    [Fact]
    public void TestAddToMaxValue_1()
    {
      var coll = PrepareCollection_1000_Max();
      coll.AddRangeByFirstAndLastInclusive(0, 500);
      Assert.Equal(2, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Equal(2, ranges.Length);
      Assert.Equal(0, ranges[0].First);
      Assert.Equal(500, ranges[0].LastInclusive);
      Assert.Equal(1000, ranges[1].First);
      Assert.Equal(int.MaxValue, ranges[1].LastInclusive);
    }

    [Fact]
    public void TestAddToMaxValue_2()
    {
      var coll = PrepareCollection_1000_Max();
      coll.AddRangeByFirstAndLastInclusive(int.MaxValue, int.MaxValue);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(1000, ranges[0].First);
      Assert.Equal(int.MaxValue, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestAddToMaxValue_3()
    {
      var coll = new AscendingIntegerRangeCollection();
      coll.AddRangeByFirstAndLastInclusive(int.MaxValue, int.MaxValue);
      coll.AddRangeByFirstAndLastInclusive(int.MaxValue - 1, int.MaxValue);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(int.MaxValue - 1, ranges[0].First);
      Assert.Equal(int.MaxValue, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestAddToMaxValue_4()
    {
      var coll = new AscendingIntegerRangeCollection();
      coll.AddRangeByFirstAndLastInclusive(int.MaxValue, int.MaxValue);
      coll.AddRangeByFirstAndLastInclusive(int.MaxValue - 1, int.MaxValue - 1);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(int.MaxValue - 1, ranges[0].First);
      Assert.Equal(int.MaxValue, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestAddToMaxValue_5()
    {
      var coll = new AscendingIntegerRangeCollection();
      coll.AddRangeByFirstAndLastInclusive(int.MaxValue, int.MaxValue);
      coll.AddRangeByFirstAndLastInclusive(int.MaxValue - 2, int.MaxValue - 1);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(int.MaxValue - 2, ranges[0].First);
      Assert.Equal(int.MaxValue, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestAddToMaxValue_6()
    {
      var coll = new AscendingIntegerRangeCollection();
      coll.AddRangeByFirstAndLastInclusive(int.MaxValue, int.MaxValue);
      coll.AddRangeByFirstAndLastInclusive(int.MaxValue - 2, int.MaxValue - 2);
      Assert.Equal(2, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Equal(2, ranges.Length);
      Assert.Equal(int.MaxValue - 2, ranges[0].First);
      Assert.Equal(int.MaxValue - 2, ranges[0].LastInclusive);
      Assert.Equal(int.MaxValue, ranges[1].First);
      Assert.Equal(int.MaxValue, ranges[1].LastInclusive);
    }

    #endregion Add to int.Max and int.Min

    [Fact]
    public void TestLowerNoCoalesce()
    {
      for (int i = -5; i <= 0; ++i)
      {
        var coll = PrepareCollection_2_3();
        coll.AddRangeByFirstAndLastInclusive(i, i);
        AssertEx.Equal(2, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(2, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(i, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(i, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(2, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(3, ranges[1].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestMiddleWithCoalesce()
    {
      for (int i = 1; i <= 4; ++i)
      {
        var coll = PrepareCollection_2_3();
        coll.AddRangeByFirstAndLastInclusive(i, i);
        AssertEx.Equal(1, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(1, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(Math.Min(i, 2), ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(Math.Max(i, 3), ranges[0].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestUpperNoCoalesce()
    {
      for (int i = 5; i <= 10; ++i)
      {
        var coll = PrepareCollection_2_3();
        coll.AddRangeByFirstAndLastInclusive(i, i);
        AssertEx.Equal(2, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(2, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(2, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(3, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(i, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(i, ranges[1].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestLowerNoCoalesceTwo()
    {
      const int WIDTH = 9;
      for (int i = -5 - WIDTH; i < 9; ++i)
      {
        var coll = PrepareCollection_10_20_30_40();
        coll.AddRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(3, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(3, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(i - WIDTH, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(i, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(20, ranges[1].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(30, ranges[2].First, "i=" + i.ToString());
        AssertEx.Equal(40, ranges[2].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void Test1stRangeCoalesceTwo()
    {
      const int WIDTH = 9;
      for (int i = 9; i < 29; ++i)
      {
        var coll = PrepareCollection_10_20_30_40();
        coll.AddRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(2, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(2, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(Math.Min(i - WIDTH, 10), ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(Math.Max(i, 20), ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(30, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(40, ranges[1].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void Test1stAnd2ndRangeCoalesceTwo()
    {
      const int WIDTH = 9;
      for (int i = 29; i < 31; ++i)
      {
        var coll = PrepareCollection_10_20_30_40();
        coll.AddRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(1, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(1, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(40, ranges[0].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void Test2ndRangeCoalesceTwo()
    {
      const int WIDTH = 9;
      for (int i = 31; i < 51; ++i)
      {
        var coll = PrepareCollection_10_20_30_40();
        coll.AddRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(2, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(2, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(20, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(Math.Min(i - WIDTH, 30), ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(Math.Max(i, 40), ranges[1].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestUpperNoCoalesceTwo()
    {
      const int WIDTH = 9;
      for (int i = 52; i < 60; ++i)
      {
        var coll = PrepareCollection_10_20_30_40();
        coll.AddRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(3, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(3, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(20, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(30, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(40, ranges[1].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(i - WIDTH, ranges[2].First, "i=" + i.ToString());
        AssertEx.Equal(i, ranges[2].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestCoalesceThreeRanges_1()
    {
      var coll = PrepareCollection_10_20_30_40_50_60();
      coll.AddRangeByFirstAndLastInclusive(21, 49);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(10, ranges[0].First);
      Assert.Equal(60, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestCoalesceThreeRanges_2()
    {
      var coll = PrepareCollection_10_20_30_40_50_60();
      coll.AddRangeByFirstAndLastInclusive(20, 49);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(10, ranges[0].First);
      Assert.Equal(60, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestCoalesceThreeRanges_3()
    {
      var coll = PrepareCollection_10_20_30_40_50_60();
      coll.AddRangeByFirstAndLastInclusive(21, 65);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(10, ranges[0].First);
      Assert.Equal(65, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestCoalesceThreeRanges_4()
    {
      var coll = PrepareCollection_10_20_30_40_50_60();
      coll.AddRangeByFirstAndLastInclusive(5, 49);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(5, ranges[0].First);
      Assert.Equal(60, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestCoalesceThreeRanges_5()
    {
      var coll = PrepareCollection_10_20_30_40_50_60();
      coll.AddRangeByFirstAndLastInclusive(5, 65);
      Assert.Equal(1, coll.RangeCount);
      var ranges = coll.Ranges.ToArray();
      Assert.Single(ranges);
      Assert.Equal(5, ranges[0].First);
      Assert.Equal(65, ranges[0].LastInclusive);
    }

    [Fact]
    public void TestRemoveFromOneRange_1() // removal range below present range
    {
      var coll = PrepareCollection_10_20();

      for (int i = 0; i < 10; ++i)
      {
        coll.RemoveRangeByFirstAndLastInclusive(i - 4, i);
        AssertEx.Equal(1, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(1, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(20, ranges[0].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromOneRange_2() // removal range above present range
    {
      for (int i = 21; i < 30; ++i)
      {
        var coll = PrepareCollection_10_20();
        coll.RemoveRangeByFirstAndLastInclusive(i, i + 4);
        AssertEx.Equal(1, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(1, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(20, ranges[0].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromOneRange_3() // remove lower half of range
    {
      for (int i = 9; i < 20; ++i)
      {
        var coll = PrepareCollection_10_20();
        coll.RemoveRangeByFirstAndLastInclusive(i - 10, i);
        AssertEx.Equal(1, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(1, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(i + 1, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(20, ranges[0].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromOneRange_4() // remove upper half of range
    {
      for (int i = 11; i <= 21; ++i)
      {
        var coll = PrepareCollection_10_20();
        coll.RemoveRangeByFirstAndLastInclusive(i, i + 10);
        AssertEx.Equal(1, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(1, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(i - 1, ranges[0].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromOneRange_5() // split range into two ranges
    {
      for (int i = 11; i <= 19; ++i)
      {
        var coll = PrepareCollection_10_20();
        coll.RemoveRangeByFirstAndLastInclusive(i, i);
        AssertEx.Equal(2, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(2, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(i - 1, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(i + 1, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(20, ranges[1].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromTwoRanges_1() // below first range
    {
      const int WIDTH = 20;

      for (int i = 0; i < 10; ++i)
      {
        var coll = PrepareCollection_10_50_60_100();
        coll.RemoveRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(2, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(2, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(50, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(60, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(100, ranges[1].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromTwoRanges_2() // in lower half of first range
    {
      const int WIDTH = 20;

      for (int i = 10; i <= 30; ++i)
      {
        var coll = PrepareCollection_10_50_60_100();
        coll.RemoveRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(2, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(2, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(i + 1, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(50, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(60, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(100, ranges[1].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromTwoRanges_3() // in middle of first range
    {
      const int WIDTH = 20;

      for (int i = 31; i < 50; ++i)
      {
        var coll = PrepareCollection_10_50_60_100();
        coll.RemoveRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(3, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(3, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(i - WIDTH - 1, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(i + 1, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(50, ranges[1].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(60, ranges[2].First, "i=" + i.ToString());
        AssertEx.Equal(100, ranges[2].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromTwoRanges_4() // in upper half of first range
    {
      const int WIDTH = 20;

      for (int i = 50; i < 60; ++i)
      {
        var coll = PrepareCollection_10_50_60_100();
        coll.RemoveRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(2, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(2, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(i - WIDTH - 1, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(60, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(100, ranges[1].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromTwoRanges_5() // in upper half of first range and lower half of second range
    {
      const int WIDTH = 20;

      for (int i = 60; i <= 71; ++i)
      {
        var coll = PrepareCollection_10_50_60_100();
        coll.RemoveRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(2, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(2, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(i - WIDTH - 1, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(i + 1, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(100, ranges[1].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromTwoRanges_6() // in  lower half of second range
    {
      const int WIDTH = 20;

      for (int i = 71; i <= 80; ++i)
      {
        var coll = PrepareCollection_10_50_60_100();
        coll.RemoveRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(2, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(2, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(50, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(i + 1, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(100, ranges[1].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromTwoRanges_7() // in middle of second range
    {
      const int WIDTH = 20;

      for (int i = 81; i < 100; ++i)
      {
        var coll = PrepareCollection_10_50_60_100();
        coll.RemoveRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(3, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(3, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(50, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(60, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(i - WIDTH - 1, ranges[1].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(i + 1, ranges[2].First, "i=" + i.ToString());
        AssertEx.Equal(100, ranges[2].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromTwoRanges_8() // in  upper half of second range
    {
      const int WIDTH = 20;

      for (int i = 100; i <= 121; ++i)
      {
        var coll = PrepareCollection_10_50_60_100();
        coll.RemoveRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(2, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(2, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(50, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(60, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(i - WIDTH - 1, ranges[1].LastInclusive, "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestRemoveFromTwoRanges_9() // above second range
    {
      const int WIDTH = 20;

      for (int i = 122; i <= 130; ++i)
      {
        var coll = PrepareCollection_10_50_60_100();
        coll.RemoveRangeByFirstAndLastInclusive(i - WIDTH, i);
        AssertEx.Equal(2, coll.RangeCount, "i=" + i.ToString());
        var ranges = coll.Ranges.ToArray();
        AssertEx.Equal(2, ranges.Length, "i=" + i.ToString());
        AssertEx.Equal(10, ranges[0].First, "i=" + i.ToString());
        AssertEx.Equal(50, ranges[0].LastInclusive, "i=" + i.ToString());
        AssertEx.Equal(60, ranges[1].First, "i=" + i.ToString());
        AssertEx.Equal(100, ranges[1].LastInclusive, "i=" + i.ToString());
      }
    }

    #region Remove from int.Max and int.Min

    [Fact]
    public void TestRemoveFromMinValue_1()
    {
      for (int i = 0; i < 10; ++i)
      {
        var coll = PrepareCollection_Min_M1000();
        coll.RemoveRangeByFirstAndLastInclusive(int.MinValue, int.MinValue + i);
        Assert.Equal(1, coll.RangeCount);
        var ranges = coll.Ranges.ToArray();
        Assert.Single(ranges);
        Assert.Equal(int.MinValue + i + 1, ranges[0].First);
        Assert.Equal(-1000, ranges[0].LastInclusive);
      }
    }

    [Fact]
    public void TestRemoveFromMaxValue_1()
    {
      for (int i = 0; i < 3; ++i)
      {
        var coll = PrepareCollection_1000_Max();
        coll.RemoveRangeByFirstAndLastInclusive(int.MaxValue - i, int.MaxValue);
        Assert.Equal(1, coll.RangeCount);
        var ranges = coll.Ranges.ToArray();
        Assert.Single(ranges);
        Assert.Equal(1000, ranges[0].First);
        Assert.Equal(int.MaxValue - i - 1, ranges[0].LastInclusive);
      }
    }

    [Fact]
    public void TestRemoveFromMinValue_2()
    {
      for (int i = 1; i < 10; ++i)
      {
        var coll = PrepareCollection_Min_M1000();
        coll.RemoveRangeByFirstAndLastInclusive(int.MinValue + i, int.MinValue + 10);
        Assert.Equal(2, coll.RangeCount);
        var ranges = coll.Ranges.ToArray();
        Assert.Equal(2, ranges.Length);
        Assert.Equal(int.MinValue, ranges[0].First);
        Assert.Equal(int.MinValue + i - 1, ranges[0].LastInclusive);
        Assert.Equal(int.MinValue + 11, ranges[1].First);
        Assert.Equal(-1000, ranges[1].LastInclusive);
      }
    }

    [Fact]
    public void TestRemoveFromMaxValue_2()
    {
      for (int i = 1; i < 10; ++i)
      {
        var coll = PrepareCollection_1000_Max();
        coll.RemoveRangeByFirstAndLastInclusive(int.MaxValue - 10, int.MaxValue - i);
        Assert.Equal(2, coll.RangeCount);
        var ranges = coll.Ranges.ToArray();
        Assert.Equal(2, ranges.Length);
        Assert.Equal(1000, ranges[0].First);
        Assert.Equal(int.MaxValue - 11, ranges[0].LastInclusive);
        Assert.Equal(int.MaxValue - i + 1, ranges[1].First);
        Assert.Equal(int.MaxValue, ranges[1].LastInclusive);
      }
    }

    #endregion Remove from int.Max and int.Min

    [Fact]
    public void TestRemoveCompleteRange_1()
    {
      for (int i = -2; i <= 2; ++i)
      {
        var coll = new AscendingIntegerRangeCollection();
        coll.AddRangeByFirstAndLastInclusive(10, 20);
        coll.RemoveRangeByFirstAndLastInclusive(i + 10 - 2, i + 20 + 2);
        AssertEx.Equal(0, coll.RangeCount, "i=" + i.ToString());
      }
    }

    #region Test indexing

    [Fact]
    public void TestIndexing_1()
    {
      var coll = new AscendingIntegerRangeCollection();
      coll.AddRangeByFirstAndLastInclusive(10, 19);
      Assert.Equal(10, coll[0]);
      Assert.Equal(19, coll[9]);

      coll.AddRangeByFirstAndLastInclusive(30, 39);
      Assert.Equal(10, coll[0]);
      Assert.Equal(19, coll[9]);
      Assert.Equal(30, coll[10]);
      Assert.Equal(39, coll[19]);

      coll.RemoveRangeByFirstAndLastInclusive(30, 35);
      Assert.Equal(10, coll[0]);
      Assert.Equal(19, coll[9]);
      Assert.Equal(36, coll[10]);
      Assert.Equal(39, coll[13]);

      coll.RemoveRangeByFirstAndLastInclusive(10, 14);
      Assert.Equal(15, coll[0]);
      Assert.Equal(19, coll[4]);
      Assert.Equal(36, coll[5]);
      Assert.Equal(39, coll[8]);
    }

    [Fact]
    public void TestIndexing_2()
    {
      var rnd = new System.Random();
      var coll = new AscendingIntegerRangeCollection();
      var hashSet = new HashSet<int>();

      for (int i = 0; i < 2000; ++i)
      {
        var r = rnd.Next(10, 4000);
        coll.AddRangeByFirstAndLastInclusive(r, r);
        hashSet.Add(r);
      }

      var hashSorted = hashSet.OrderBy(x => x).ToArray();
      var collSorted = coll.ToArray();

      // Auswertung
      Assert.Equal(hashSorted.Length, collSorted.Length);
      for (int i = 0; i < hashSorted.Length; ++i)
      {
        AssertEx.Equal(hashSorted[i], collSorted[i], "i=" + i.ToString());
      }

      Assert.Equal(hashSorted.Length, coll.Count);
      for (int i = 0; i < hashSorted.Length; ++i)
      {
        AssertEx.Equal(hashSorted[i], coll[i], "i=" + i.ToString());
      }
    }

    [Fact]
    public void TestIndexing_3()
    {
      var rnd = new System.Random();
      var coll = new AscendingIntegerRangeCollection();
      var hashSet = new HashSet<int>();

      for (int i = 0; i < 3000; ++i)
      {
        var r = rnd.Next(10, 4000);
        coll.AddRangeByFirstAndLastInclusive(r, r);
        hashSet.Add(r);
      }

      for (int i = 0; i < 3000; ++i)
      {
        var r = rnd.Next(10, 4000);
        coll.RemoveRangeByFirstAndLastInclusive(r, r);
        hashSet.Remove(r);
      }

      var hashSorted = hashSet.OrderBy(x => x).ToArray();
      var collSorted = coll.ToArray();

      // Evaluation of the sorted arrays
      Assert.Equal(hashSorted.Length, collSorted.Length);
      for (int i = 0; i < hashSorted.Length; ++i)
      {
        AssertEx.Equal(hashSorted[i], collSorted[i], "i=" + i.ToString());
      }

      // Test of indexing
      Assert.Equal(hashSorted.Length, coll.Count);
      for (int i = 0; i < hashSorted.Length; ++i)
      {
        AssertEx.Equal(hashSorted[i], coll[i], "i=" + i.ToString());
      }

      // Test of Contains
      int minElement = hashSorted[0];
      int maxElement = hashSorted[hashSorted.Length - 1];

      for (int ele = minElement - 5; ele < maxElement + 5; ++ele)
      {
        Assert.Equal(hashSet.Contains(ele), coll.Contains(ele));
      }
    }

    private string AssertEqual(AscendingIntegerRangeCollection coll, HashSet<int> hashSet)
    {
      var stb = new System.Text.StringBuilder();
      var hashSorted = hashSet.OrderBy(x => x).ToArray();
      var collSorted = coll.ToArray();

      // Evaluation
      if (hashSorted.Length != collSorted.Length)
      {
        stb.AppendFormat("Length different. Expected {0} but was {1}", hashSorted.Length, collSorted.Length);
        stb.AppendLine();
      }

      var len = Math.Min(hashSorted.Length, collSorted.Length);
      for (int i = 0; i < len; ++i)
      {
        if (hashSorted[i] != collSorted[i])
        {
          stb.AppendFormat("Different elements at index[{0}]. Expected {1} but was {2}", i, hashSorted[i], collSorted[i]);
          break;
        }
      }

      return stb.Length == 0 ? null : stb.ToString();
    }

    [Fact]
    public void TestAddRemoveRandomly_1()
    {
      var rnd = new System.Random();
      var coll = new AscendingIntegerRangeCollection();
      var hashSet = new HashSet<int>();
      var addList = new List<int>();
      var rmvList = new List<int>();
      string error = null;

      int indexThatWentWrong = -1;

      for (int i = 0; i < 2000; ++i)
      {
        var r = rnd.Next(10, 4000);
        addList.Add(r);
      }

      for (int i = 0; i < 1000; ++i)
      {
        var r = rnd.Next(10, 4000);
        rmvList.Add(r);
      }

      for (int i = 0; i < addList.Count; ++i)
      {
        var r = addList[i];
        coll.AddRangeByFirstAndLastInclusive(r, r);
        hashSet.Add(r);
      }

      for (int i = 0; i < rmvList.Count; ++i)
      {
        var r = rmvList[i];
        hashSet.Remove(r);
        coll.RemoveRangeByFirstAndLastInclusive(r, r);

        error = AssertEqual(coll, hashSet);
        if (error is not null)
        {
          indexThatWentWrong = i;
          break;
        }
      }

      // Replay the error

      coll = new AscendingIntegerRangeCollection();
      hashSet = new HashSet<int>();

      for (int i = 0; i < addList.Count; ++i)
      {
        var r = addList[i];
        coll.AddRangeByFirstAndLastInclusive(r, r);
        hashSet.Add(r);
      }

      for (int i = 0; i < rmvList.Count; ++i)
      {
        var r = rmvList[i];
        hashSet.Remove(r);

        if (i == indexThatWentWrong)
        {
          System.Diagnostics.Debugger.Break();
        }

        coll.RemoveRangeByFirstAndLastInclusive(r, r);

        error = AssertEqual(coll, hashSet);
        Assert.Null(error);
      }
    }

    #endregion Test indexing
  }
}
