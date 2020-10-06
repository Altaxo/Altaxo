#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altaxo.Collections;
using Xunit;

namespace AltaxoTest.Collections
{

  public class TestConcurrentPriorityQueue
  {
    #region Helpers

    private static ConcurrentQueue<int> GetRandomListOfDistinctIntegers(int numberOfEntries)
    {
      var list = new ConcurrentQueue<int>();
      var hash = new HashSet<int>();
      var rnd = new Random();

      int numEntry = 0;

      while (numEntry < numberOfEntries)
      {
        var n = rnd.Next();
        if (!hash.Contains(n))
        {
          list.Enqueue(n);
          ++numEntry;
        }
      }

      return list;
    }

    #endregion Helpers

    private void TestOrderExecute(int? capacity)
    {
      const int numberOfElements = 10000;
      var rnd = new System.Random();

      ConcurrentPriorityQueue<int, int> queue;

      if (capacity.HasValue)
        queue = new ConcurrentPriorityQueue<int, int>(capacity.Value);
      else
        queue = new ConcurrentPriorityQueue<int, int>();

      for (int i = 0; i < numberOfElements; ++i)
        queue.Enqueue(100 + rnd.Next(numberOfElements), 23);

      Assert.Equal(numberOfElements, queue.Count);

      int previous = int.MinValue;
      for (int i = 0; i < numberOfElements; ++i)
      {
        var result = queue.TryDequeue(out var curr, out var val);
        AssertEx.GreaterOrEqual(curr, previous);
        Assert.Equal(23, val);
        previous = curr;
      }

      Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void TestOrder()
    {
      TestOrderExecute(null);
      TestOrderExecute(13);
      TestOrderExecute(22);
      TestOrderExecute(65353);
    }

    [Fact]
    public void Test002_Concurrency()
    {
      const int numberOfItems = 100000;

      var sourceItems = GetRandomListOfDistinctIntegers(numberOfItems);
      var sourceItemsSorted = new List<int>(sourceItems);
      sourceItemsSorted.Sort();

      var destinationItems = new ConcurrentBag<int>();

      var queueToTest = new ConcurrentPriorityQueue<int, int>();

      // 2 Enqueue tasks and 2 Dequeue tasks
      Parallel.Invoke(new Action[]
      {
				// Enqueue
 				() =>
        {
         int item;
         while (sourceItems.TryDequeue(out item))
            queueToTest.Enqueue(item, item + 13);
        },
				// Enqueue
				() =>
        {
           int item;
         while (sourceItems.TryDequeue(out item))
            queueToTest.Enqueue(item, item + 13);
        },
				// Dequeue
				()=>
          {
            int counter = 0;
            while(counter<100)
            {
            while(queueToTest.TryDequeue(out var key, out var value))
            {
              Assert.Equal(key + 13, value);
              destinationItems.Add(key);
              counter = 0;
            }
            System.Threading.Thread.Sleep(10);
            ++counter;
            }
          },
				// Dequeue
					()=>
          {
            int counter = 0;
            while(counter<100)
            {
            while(queueToTest.TryDequeue(out var key, out var value))
            {
              Assert.Equal(key + 13, value);
              destinationItems.Add(key);
              counter = 0;
            }
            System.Threading.Thread.Sleep(10);
            ++counter;
            }
          }
      });

      AssertEx.Equal(0, sourceItems.Count, "All source items should be moved into the queueToTest and then to the destinationItems collection");
      AssertEx.Equal(0, queueToTest.Count, "Queue should be empty, all items should be moved into the destinationItems collection");
      Assert.Equal(numberOfItems, destinationItems.Count);
      var list = new List<int>(destinationItems);

      list.Sort();

      for (int i = 0; i < numberOfItems; ++i)
      {
        Assert.Equal(sourceItemsSorted[i], list[i]);
      }
    }
  }
}
