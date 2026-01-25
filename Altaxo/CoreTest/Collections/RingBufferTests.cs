#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.Linq;
using Xunit;

namespace Altaxo.Collections
{
  public class RingBufferTests
  {
    [Fact]
    public void Test()
    {
      var r = new RingBuffer<int>(4);
      Assert.Equal(0, r.Count);

      r.Add(7);
      Assert.Equal(1, r.Count);
      Assert.Equal(7, r.ItemAt(0));

      r.Add(13);
      Assert.Equal(2, r.Count);
      Assert.Equal(7, r.ItemAt(0));
      Assert.Equal(13, r.ItemAt(1));

      r.Add(42);
      Assert.Equal(3, r.Count);
      Assert.Equal(7, r.ItemAt(0));
      Assert.Equal(13, r.ItemAt(1));
      Assert.Equal(42, r.ItemAt(2));

      r.Add(99);
      Assert.Equal(4, r.Count);
      Assert.Equal(7, r.ItemAt(0));
      Assert.Equal(13, r.ItemAt(1));
      Assert.Equal(42, r.ItemAt(2));
      Assert.Equal(99, r.ItemAt(3));

      Assert.Throws<InvalidOperationException>(() => r.Add(123));
      r.RemoveItems(1);
      r.Add(123);
      Assert.Equal(4, r.Count);
      Assert.Equal(13, r.ItemAt(0));
      Assert.Equal(42, r.ItemAt(1));
      Assert.Equal(99, r.ItemAt(2));
      Assert.Equal(123, r.ItemAt(3));

      r.RemoveItems(1);
      r.Add(456);
      Assert.Equal(4, r.Count);
      Assert.Equal(42, r.ItemAt(0));
      Assert.Equal(99, r.ItemAt(1));
      Assert.Equal(123, r.ItemAt(2));
      Assert.Equal(456, r.ItemAt(3));

      r.RemoveItems(2);
      Assert.Equal(2, r.Count);
      Assert.Equal(123, r.ItemAt(0));
      Assert.Equal(456, r.ItemAt(1));
    }

    [Fact]
    public void TestExceptions()
    {
      var r = new RingBuffer<int>(4);
      Assert.Equal(0, r.Count);
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(0));
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(-1));
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(1));
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(2));
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(3));

      r.Add(7);
      Assert.Equal(1, r.Count);
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(1));
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(2));
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(3));
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(-1));

      r.Add(13);
      Assert.Equal(2, r.Count);
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(2));
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(3));
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(-1));

      r.Add(99);
      Assert.Equal(3, r.Count);
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(3));
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(-1));

      r.Add(123);
      Assert.Equal(4, r.Count);
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(-1));

      Assert.Throws<InvalidOperationException>(() => r.Add(456));
      r.RemoveItems(1);
      r.Add(456);
      Assert.Equal(4, r.Count);

      r.RemoveItems(2);
      Assert.Equal(2, r.Count);
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(2));
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(3));
      Assert.Throws<IndexOutOfRangeException>(() => r.ItemAt(-1));
    }

    [Fact]
    public void TestEnumerationSingle()
    {
      var r = new RingBuffer<int>(4);
      Assert.Empty(r.EnumerateOneItem());

      r.Add(7);
      var arr = r.EnumerateOneItem().ToArray();
      Assert.Single(arr);
      Assert.Equal((7, 0), arr[0]);

      r.Add(13);
      arr = r.EnumerateOneItem().ToArray();
      Assert.Equal(2, arr.Length);
      Assert.Equal((7, 0), arr[0]);
      Assert.Equal((13, 1), arr[1]);

      r.Add(42);
      arr = r.EnumerateOneItem().ToArray();
      Assert.Equal(3, arr.Length);
      Assert.Equal((7, 0), arr[0]);
      Assert.Equal((13, 1), arr[1]);
      Assert.Equal((42, 2), arr[2]);

      r.Add(99);
      arr = r.EnumerateOneItem().ToArray();
      Assert.Equal(4, arr.Length);
      Assert.Equal((7, 0), arr[0]);
      Assert.Equal((13, 1), arr[1]);
      Assert.Equal((42, 2), arr[2]);
      Assert.Equal((99, 3), arr[3]);


      r.RemoveItems(2);
      arr = r.EnumerateOneItem().ToArray();
      Assert.Equal(2, arr.Length);
      Assert.Equal((42, 0), arr[0]);
      Assert.Equal((99, 1), arr[1]);

      r.Add(123);
      arr = r.EnumerateOneItem().ToArray();
      Assert.Equal(3, arr.Length);
      Assert.Equal((42, 0), arr[0]);
      Assert.Equal((99, 1), arr[1]);
      Assert.Equal((123, 2), arr[2]);
    }

    [Fact]
    public void TestEnumerationTwo()
    {
      var r = new RingBuffer<int>(4);
      Assert.Empty(r.EnumerateTwoItems());
      r.Add(7);
      var arr = r.EnumerateTwoItems().ToArray();
      Assert.Empty(r.EnumerateTwoItems());
      r.Add(13);
      arr = r.EnumerateTwoItems().ToArray();
      Assert.Single(arr);
      Assert.Equal((7, 13, 0), arr[0]);
      r.Add(42);
      arr = r.EnumerateTwoItems().ToArray();
      Assert.Equal(2, arr.Length);
      Assert.Equal((7, 13, 0), arr[0]);
      Assert.Equal((13, 42, 1), arr[1]);
      r.Add(99);
      arr = r.EnumerateTwoItems().ToArray();
      Assert.Equal(3, arr.Length);
      Assert.Equal((7, 13, 0), arr[0]);
      Assert.Equal((13, 42, 1), arr[1]);
      Assert.Equal((42, 99, 2), arr[2]);

      r.RemoveItems(1);
      r.Add(123);
      arr = r.EnumerateTwoItems().ToArray();
      Assert.Equal(3, arr.Length);
      Assert.Equal((13, 42, 0), arr[0]);
      Assert.Equal((42, 99, 1), arr[1]);
      Assert.Equal((99, 123, 2), arr[2]);
    }
  }
}
