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

using Altaxo.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AltaxoTest.Collections
{
  [TestFixture]
  public class Test_ContiguousIntegerRange
  {
    [Test]
    public void TestEmpty()
    {
      var r = new ContiguousIntegerRange();

      Assert.IsTrue(r.IsEmpty);
      Assert.AreEqual(0, r.Count);
      Assert.AreEqual(0, r.LongCount);
      Assert.AreEqual(0, r.Start);
    }

    [Test]
    public void TestFromStartAndCount0()
    {
      var r = ContiguousIntegerRange.FromStartAndCount(0, 0);
      Assert.IsTrue(r.IsEmpty);
      Assert.AreEqual(0, r.Count);
      Assert.AreEqual(0, r.LongCount);
      Assert.AreEqual(0, r.Start);
    }

    [Test]
    public void TestFromStartAndCount1()
    {
      var r = ContiguousIntegerRange.FromStartAndCount(5, 0);
      Assert.IsTrue(r.IsEmpty);
      Assert.AreEqual(0, r.Count);
      Assert.AreEqual(0, r.LongCount);
      Assert.AreEqual(0, r.Start);
    }

    [Test]
    public void TestFromStartAndCount2()
    {
      var r = ContiguousIntegerRange.FromStartAndCount(int.MinValue, 0);
      Assert.IsTrue(r.IsEmpty);
      Assert.AreEqual(0, r.Count);
      Assert.AreEqual(0, r.LongCount);
      Assert.AreEqual(0, r.Start);
    }

    [Test]
    public void TestFromStartAndCount3()
    {
      var r = ContiguousIntegerRange.FromStartAndCount(int.MaxValue, 0);
      Assert.IsTrue(r.IsEmpty);
      Assert.AreEqual(0, r.Count);
      Assert.AreEqual(0, r.LongCount);
      Assert.AreEqual(0, r.Start);
    }

    [Test]
    public void TestFromStartAndCount4A()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var r = ContiguousIntegerRange.FromStartAndCount(0, -1);
      });
    }

    [Test]
    public void TestFromStartAndCount4B()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var r = ContiguousIntegerRange.FromStartAndCount(1, int.MaxValue);
      });
    }

    [Test]
    public void TestFromStartAndCount4C()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        var r = ContiguousIntegerRange.FromStartAndCount(int.MaxValue, 1);
      });
    }

    [Test]
    public void TestFromStartAndEndExclusive0()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(0, 0);
      Assert.IsTrue(r.IsEmpty);
      Assert.AreEqual(0, r.Count);
      Assert.AreEqual(0, r.LongCount);
      Assert.AreEqual(0, r.Start);
    }

    [Test]
    public void TestFromStartAndEndExclusive1()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(5, 5);
      Assert.IsTrue(r.IsEmpty);
      Assert.AreEqual(0, r.Count);
      Assert.AreEqual(0, r.LongCount);
      Assert.AreEqual(0, r.Start);
    }

    [Test]
    public void TestFromStartAndEndExclusive2()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(int.MinValue, int.MinValue);
      Assert.IsTrue(r.IsEmpty);
      Assert.AreEqual(0, r.Count);
      Assert.AreEqual(0, r.LongCount);
      Assert.AreEqual(0, r.Start);
    }

    [Test]
    public void TestFromStartAndEndExclusive3()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(int.MaxValue, int.MaxValue);
      Assert.IsTrue(r.IsEmpty);
      Assert.AreEqual(0, r.Count);
      Assert.AreEqual(0, r.LongCount);
      Assert.AreEqual(0, r.Start);
    }

    [Test]
    public void TestFromStartAndEndExclusive4()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(int.MinValue, int.MaxValue);
      Assert.IsFalse(r.IsEmpty);
      Assert.AreEqual(uint.MaxValue, r.LongCount);
      Assert.AreEqual(int.MinValue, r.Start);
      Assert.AreEqual(int.MaxValue, r.EndExclusive);
    }

    [Test]
    public void TestFromStartAndEndExclusive5()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(int.MinValue, int.MinValue + 1);
      Assert.IsFalse(r.IsEmpty);
      Assert.AreEqual(1, r.Count);
      Assert.AreEqual(1, r.LongCount);
      Assert.AreEqual(int.MinValue, r.Start);
      Assert.AreEqual(int.MinValue, r.LastInclusive);
      Assert.AreEqual(1 + int.MinValue, r.EndExclusive);
    }

    [Test]
    public void TestFromStartAndEndExclusive6()
    {
      var r = ContiguousIntegerRange.FromStartAndEndExclusive(int.MaxValue - 1, int.MaxValue);
      Assert.IsFalse(r.IsEmpty);
      Assert.AreEqual(1, r.Count);
      Assert.AreEqual(1, r.LongCount);
      Assert.AreEqual(int.MaxValue - 1, r.Start);
      Assert.AreEqual(int.MaxValue - 1, r.LastInclusive);
      Assert.AreEqual(int.MaxValue, r.EndExclusive);
    }

    [Test]
    public void TestFromFirstAndLastInclusive0()
    {
      var r = ContiguousIntegerRange.FromFirstAndLastInclusive(int.MinValue, int.MaxValue);
      Assert.IsFalse(r.IsEmpty);
      Assert.AreEqual(1L + uint.MaxValue, r.LongCount);
      Assert.AreEqual(int.MinValue, r.Start);
      Assert.AreEqual(int.MaxValue, r.LastInclusive);
    }

    [Test]
    public void TestFromFirstAndLastInclusive1()
    {
      var r = ContiguousIntegerRange.FromFirstAndLastInclusive(int.MinValue + 1, int.MaxValue);
      Assert.IsFalse(r.IsEmpty);
      Assert.AreEqual((long)uint.MaxValue, r.LongCount);
      Assert.AreEqual(int.MinValue + 1, r.Start);
      Assert.AreEqual(int.MaxValue, r.LastInclusive);
    }

    [Test]
    public void TestFromFirstAndLastInclusive2()
    {
      var r = ContiguousIntegerRange.FromFirstAndLastInclusive(int.MinValue, int.MaxValue - 1);
      Assert.IsFalse(r.IsEmpty);
      Assert.AreEqual((long)uint.MaxValue, r.LongCount);
      Assert.AreEqual(int.MinValue, r.Start);
      Assert.AreEqual(int.MaxValue - 1, r.LastInclusive);
      Assert.AreEqual(int.MaxValue, r.EndExclusive);
    }

    [Test]
    public void TestFromFirstAndLastInclusive3()
    {
      var r = ContiguousIntegerRange.FromFirstAndLastInclusive(int.MinValue, int.MinValue);
      Assert.IsFalse(r.IsEmpty);
      Assert.AreEqual(1, r.Count);
      Assert.AreEqual(1, r.LongCount);
      Assert.AreEqual(int.MinValue, r.Start);
      Assert.AreEqual(int.MinValue, r.LastInclusive);
    }

    [Test]
    public void TestFromFirstAndLastInclusive4()
    {
      var r = ContiguousIntegerRange.FromFirstAndLastInclusive(int.MaxValue, int.MaxValue);
      Assert.IsFalse(r.IsEmpty);
      Assert.AreEqual(1, r.Count);
      Assert.AreEqual(1, r.LongCount);
      Assert.AreEqual(int.MaxValue, r.Start);
      Assert.AreEqual(int.MaxValue, r.LastInclusive);
    }
  }
}
