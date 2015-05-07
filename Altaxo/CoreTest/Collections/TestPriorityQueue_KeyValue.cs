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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AltaxoTest.Collections
{
	[TestFixture]
	internal class TestPriorityQueue_KeyValue
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

		[Test]
		public void TestOrder()
		{
			const int numberOfElements = 10000;
			var rnd = new System.Random();

			var queue = new ConcurrentTokenizedPriorityQueue<int, int, int>(1, x => x + 1);
			int token;

			for (int i = 0; i < numberOfElements; ++i)
				queue.Enqueue(100 + rnd.Next(numberOfElements), 23, out token);

			Assert.AreEqual(numberOfElements, queue.Count);

			int previous = int.MinValue;
			for (int i = 0; i < numberOfElements; ++i)
			{
				int curr = queue.Dequeue().Key;
				Assert.GreaterOrEqual(curr, previous);
				previous = curr;
			}

			Assert.AreEqual(0, queue.Count);
		}

		[Test]
		public void Test002_Concurrency()
		{
			const int numberOfItems = 100000;

			var sourceItems = GetRandomListOfDistinctIntegers(numberOfItems);
			var sourceItemsSorted = new List<int>(sourceItems);
			sourceItemsSorted.Sort();

			var destinationItems = new ConcurrentBag<int>();

			var queueToTest = new ConcurrentTokenizedPriorityQueue<int, int, int>(1, x => x + 1);

			// 2 Enqueue tasks and 2 Dequeue tasks
			Parallel.Invoke(new Action[]
			{
				// Enqueue
 				() =>
				{
					int item, token;
					while (sourceItems.TryDequeue(out item))
						queueToTest.Enqueue(item, item + 13, out token);
				},
				// Enqueue
				() =>
				{
					int item, token;
					while (sourceItems.TryDequeue(out item))
						queueToTest.Enqueue(item, item + 13, out token);
				},
				// Dequeue
				()=>
					{
						int key, value, toke;
						int counter = 0;
						while(counter<100)
						{
						while(queueToTest.TryDequeue(out key, out value, out toke))
						{
							Assert.AreEqual(key + 13, value);
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
						int key, value, toke;
						int counter = 0;
						while(counter<100)
						{
						while(queueToTest.TryDequeue(out key, out value, out toke))
						{
							Assert.AreEqual(key + 13, value);
							destinationItems.Add(key);
							counter = 0;
						}
						System.Threading.Thread.Sleep(10);
						++counter;
						}
					}
			});

			Assert.AreEqual(0, sourceItems.Count, "All source items should be moved into the queueToTest and then to the destinationItems collection");
			Assert.AreEqual(0, queueToTest.Count, "Queue should be empty, all items should be moved into the destinationItems collection");
			Assert.AreEqual(numberOfItems, destinationItems.Count);
			var list = new List<int>(destinationItems);

			list.Sort();

			for (int i = 0; i < numberOfItems; ++i)
			{
				Assert.AreEqual(sourceItemsSorted[i], list[i]);
			}
		}
	}
}