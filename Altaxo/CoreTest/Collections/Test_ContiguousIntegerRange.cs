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

  public class Test_ContiguousIntegerRange
  {
    [Fact]
    public void TestEmpty()
    {
      var r = new ContiguousIntegerRange();

      Assert.True(r.IsEmpty);
      Assert.Empty(r);
      Assert.Equal(0, r.LongCount);
      Assert.Equal(0, r.Start);
    }

    [Fact]
    public void TestFromStartAndCount0()
    {
      var r = ContiguousIntegerRange.FromStartAndCount(0, 0);
      Assert.True(r.IsEmpty);
      Assert.Empty(r);
      Assert.Equal(0, r.LongCount);
      Assert.Equal(0, r.Start);
    }

    [Fact]
    public void TestFromStartAndCount1()
    {
      var r = ContiguousIntegerRange.FromStartAndCount(5, 0);
      Assert.True(r.IsEmpty);
      Assert.Empty(r);
      Assert.Equal(0, r.LongCount);
      Assert.Equal(0, r.Start);
    }

    [Fact]
    public void TestFromStartAndCount2()
    {
      var r = ContiguousIntegerRange.FromStartAndCount(int.MinValue, 0);
      Assert.True(r.IsEmpty);
      Assert.Empty(r);
      Assert.Equal(0, r.LongCount);
      Assert.Equal(0, r.Start);
    }

    [Fact]
    public void TestFromStartAndCount3()
    {
      var r = ContiguousIntegerRange.FromStartAndCount(int.MaxValue, 0);
      Assert.True(r.IsEmpty);
      Assert.Empty(r);
      Assert.Equal(0, r.LongCount);
      Assert.Equal(0, r.Start);
    }

    [Fact]
    public void TestFromStartAndCount4A()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var r = ContiguousIntegerRange.FromStartAndCount(0, -1);
      });
    }

    [Fact]
    public void TestFromStartAndCount4B()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var r = ContiguousIntegerRange.FromStartAndCount(1, int.MaxValue);
      });
    }

    [Fact]
    public void TestFromStartAndCount4C()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        var r = ContiguousIntegerRange.FromStartAndCount(int.MaxValue, 1);
      });
    }

    [Fact]
    public void TestFromStartAndEndExclusive0()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(0, 0);
      Assert.True(r.IsEmpty);
      Assert.Empty(r);
      Assert.Equal(0, r.LongCount);
      Assert.Equal(0, r.Start);
    }

    [Fact]
    public void TestFromStartAndEndExclusive1()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(5, 5);
      Assert.True(r.IsEmpty);
      Assert.Empty(r);
      Assert.Equal(0, r.LongCount);
      Assert.Equal(0, r.Start);
    }

    [Fact]
    public void TestFromStartAndEndExclusive2()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(int.MinValue, int.MinValue);
      Assert.True(r.IsEmpty);
      Assert.Empty(r);
      Assert.Equal(0, r.LongCount);
      Assert.Equal(0, r.Start);
    }

    [Fact]
    public void TestFromStartAndEndExclusive3()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(int.MaxValue, int.MaxValue);
      Assert.True(r.IsEmpty);
      Assert.Empty(r);
      Assert.Equal(0, r.LongCount);
      Assert.Equal(0, r.Start);
    }

    [Fact]
    public void TestFromStartAndEndExclusive4()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(int.MinValue, int.MaxValue);
      Assert.False(r.IsEmpty);
      Assert.Equal(uint.MaxValue, r.LongCount);
      Assert.Equal(int.MinValue, r.Start);
      Assert.Equal(int.MaxValue, r.EndExclusive);
    }

    [Fact]
    public void TestFromStartAndEndExclusive5()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(int.MinValue, int.MinValue + 1);
      Assert.False(r.IsEmpty);
      Assert.Single(r);
      Assert.Equal(1, r.LongCount);
      Assert.Equal(int.MinValue, r.Start);
      Assert.Equal(int.MinValue, r.LastInclusive);
      Assert.Equal(1 + int.MinValue, r.EndExclusive);
    }

    [Fact]
    public void TestFromStartAndEndExclusive6()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(int.MaxValue - 1, int.MaxValue);
      Assert.False(r.IsEmpty);
      Assert.Single(r);
      Assert.Equal(1, r.LongCount);
      Assert.Equal(int.MaxValue - 1, r.Start);
      Assert.Equal(int.MaxValue - 1, r.LastInclusive);
      Assert.Equal(int.MaxValue, r.EndExclusive);
    }

    [Fact]
    public void TestFromFirstAndLastInclusive0()
    {
      var r = ContiguousIntegerRange.FromFirstAndLastInclusive(int.MinValue, int.MaxValue);
      Assert.False(r.IsEmpty);
      Assert.Equal(1L + uint.MaxValue, r.LongCount);
      Assert.Equal(int.MinValue, r.Start);
      Assert.Equal(int.MaxValue, r.LastInclusive);
    }

    [Fact]
    public void TestFromFirstAndLastInclusive1()
    {
      var r = ContiguousIntegerRange.FromFirstAndLastInclusive(int.MinValue + 1, int.MaxValue);
      Assert.False(r.IsEmpty);
      Assert.Equal((long)uint.MaxValue, r.LongCount);
      Assert.Equal(int.MinValue + 1, r.Start);
      Assert.Equal(int.MaxValue, r.LastInclusive);
    }

    [Fact]
    public void TestFromFirstAndLastInclusive2()
    {
      var r = ContiguousIntegerRange.FromFirstAndLastInclusive(int.MinValue, int.MaxValue - 1);
      Assert.False(r.IsEmpty);
      Assert.Equal((long)uint.MaxValue, r.LongCount);
      Assert.Equal(int.MinValue, r.Start);
      Assert.Equal(int.MaxValue - 1, r.LastInclusive);
      Assert.Equal(int.MaxValue, r.EndExclusive);
    }

    [Fact]
    public void TestFromFirstAndLastInclusive3()
    {
      var r = ContiguousIntegerRange.FromFirstAndLastInclusive(int.MinValue, int.MinValue);
      Assert.False(r.IsEmpty);
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
      Assert.Equal(1, r.Count);
#pragma warning restore xUnit2013 // Do not use equality check to check for collection size.
      Assert.Single(r); // we check both the count (previous statement), and here if there is really only one element.
      Assert.Equal(1, r.LongCount);
      Assert.Equal(int.MinValue, r.Start);
      Assert.Equal(int.MinValue, r.LastInclusive);

      int cnt = 0;
      foreach (var e in r)
      {
        Assert.True(0 == cnt, $"Only one element should be included, but there is another element: {e}");
        Assert.Equal(int.MinValue, e);
        ++cnt;
      }
    }

    [Fact]
    public void TestFromFirstAndLastInclusive4()
    {
      var r = ContiguousIntegerRange.FromFirstAndLastInclusive(int.MaxValue, int.MaxValue);
      Assert.False(r.IsEmpty);
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
      Assert.Equal(1, r.Count);
#pragma warning restore xUnit2013 // Do not use equality check to check for collection size.
      Assert.Equal(1, r.LongCount);
      Assert.Equal(int.MaxValue, r.Start);
      Assert.Equal(int.MaxValue, r.LastInclusive);

      int cnt = 0;
      foreach (var e in r)
      {
        Assert.True(0 == cnt, $"Only one element should be included, but there is another element: {e}");
        Assert.Equal(int.MaxValue, e);
        ++cnt;
      }
    }
  }
}
