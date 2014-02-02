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
	public class TestPartitionableList
	{
		[Test]
		public void TestList()
		{
			const int max = 10;
			PartitionableList<int> list = new PartitionableList<int>();

			for (int i = 0; i < max; ++i)
				list.Add(i);

			Assert.AreEqual(list.Count, max, "Count of the list unexpected");

			for (int i = 0; i < max; ++i)
				Assert.AreEqual(i, list[i], string.Format("items unequal at index {0}", i));
		}

		/// <summary>
		/// Tests the creation of the partial view after the main list was created and already filled with values.
		/// </summary>
		[Test]
		public void TestPartialViewDelayed()
		{
			const int max = 10;
			PartitionableList<int> list = new PartitionableList<int>();
			for (int i = 0; i < max; ++i)
				list.Add(i);
			Assert.AreEqual(list.Count, max, "Count of the list unexpected");

			var pw = list.CreatePartialView(x => x % 2 == 0);

			Assert.AreEqual(pw.Count, max / 2, "Count of the partial view unexpected");

			for (int i = 0; i < max / 2; ++i)
				Assert.AreEqual(i * 2, pw[i], string.Format("items of partial view unequal at index {0}", i));
		}

		[Test]
		public void TestPartitionCreation()
		{
			IList<int> mainList, oddList, evenList;
			PartitionCreation(out mainList, out evenList, out oddList);
		}

		public void PartitionCreation(out IList<int> mainList, out IList<int> evenList, out IList<int> oddList)
		{
			const int max = 10;
			PartitionableList<int> list = new PartitionableList<int>();
			mainList = list;

			oddList = list.CreatePartialView(x => 0 != (x % 2));
			evenList = list.CreatePartialView(x => 0 == (x % 2));

			for (int i = 0; i < max; ++i)
				list.Add(i);

			Assert.AreEqual(max, list.Count, "Count of the list unexpected");
			for (int i = 0; i < max; ++i)
				Assert.AreEqual(i, list[i], string.Format("items unequal at index {0}", i));

			Assert.AreEqual(max / 2, evenList.Count, "Count of the even list unexpected");

			for (int i = 0; i < max / 2; ++i)
				Assert.AreEqual(i * 2, evenList[i], string.Format("items of even list unequal at index {0}", i));

			Assert.AreEqual(max / 2, oddList.Count, "Count of the odd list unexpected");

			for (int i = 0; i < max / 2; ++i)
				Assert.AreEqual(i * 2 + 1, oddList[i], string.Format("items of odd list unequal at index {0}", i));
		}

		[Test]
		public void TestMainRemoval1() // Removal of the midst of the main list
		{
			double[] r1 = new double[] { 0, 1, 2, 3, 4, 6, 7, 8, 9 };
			double[] r2 = new double[] { 0, 2, 4, 6, 8 };
			double[] r3 = new double[] { 1, 3, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			mainList.RemoveAt(5); // list contain all elements but not 5

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestMainRemoval2() // Removal of the beginning of the main list
		{
			double[] r1 = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			double[] r2 = new double[] { 2, 4, 6, 8 };
			double[] r3 = new double[] { 1, 3, 5, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			mainList.RemoveAt(0); // list contain all elements but not 0

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestMainRemoval3() // Removal of the end of the main list
		{
			double[] r1 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
			double[] r2 = new double[] { 0, 2, 4, 6, 8 };
			double[] r3 = new double[] { 1, 3, 5, 7 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			mainList.RemoveAt(9); // list contain all elements but not 9

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestMainRemoval4() // Removal of multiple items of the main list
		{
			double[] r1 = new double[] { 0, 1, 2, 3, 4, 7, 8, 9 };
			double[] r2 = new double[] { 0, 2, 4, 8 };
			double[] r3 = new double[] { 1, 3, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			mainList.RemoveAt(5); // list contain all elements but not 5
			mainList.RemoveAt(5); // list contain all elements but not 6

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestPartialRemoval1() // Removal in the midst of the odd list
		{
			double[] r1 = new double[] { 0, 1, 2, 3, 4, 6, 7, 8, 9 };
			double[] r2 = new double[] { 0, 2, 4, 6, 8 };
			double[] r3 = new double[] { 1, 3, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			oddList.RemoveAt(2); // list contain all elements but not 5

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestPartialRemoval2() // Removal of the beginning of the odd list
		{
			double[] r1 = new double[] { 0, 2, 3, 4, 5, 6, 7, 8, 9 };
			double[] r2 = new double[] { 0, 2, 4, 6, 8 };
			double[] r3 = new double[] { 3, 5, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			oddList.RemoveAt(0); // list contain all elements but not 1

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestPartialRemoval3() // Removal of the beginning of the even list
		{
			double[] r1 = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			double[] r2 = new double[] { 2, 4, 6, 8 };
			double[] r3 = new double[] { 1, 3, 5, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			evenList.RemoveAt(0); // list contain all elements but not 0

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestPartialRemoval4() // Removal of the end of the odd list
		{
			double[] r1 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
			double[] r2 = new double[] { 0, 2, 4, 6, 8 };
			double[] r3 = new double[] { 1, 3, 5, 7 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			oddList.RemoveAt(oddList.Count - 1); // list contain all elements but not 9

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestPartialRemoval5() // Removal of the end of the even list
		{
			double[] r1 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 9 };
			double[] r2 = new double[] { 0, 2, 4, 6 };
			double[] r3 = new double[] { 1, 3, 5, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			evenList.RemoveAt(evenList.Count - 1); // list contain all elements but not 8

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestPartialRemoval6() // Complete removal of the odd list
		{
			double[] r1 = new double[] { 0, 2, 4, 6, 8 };
			double[] r2 = new double[] { 0, 2, 4, 6, 8 };
			double[] r3 = new double[] { };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			oddList.Clear(); // list contain all elements but not odd elements

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestPartialRemoval7() // Complete removal of the even list
		{
			double[] r1 = new double[] { 1, 3, 5, 7, 9 };
			double[] r2 = new double[] { };
			double[] r3 = new double[] { 1, 3, 5, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			evenList.Clear(); // list contain all elements but not even elements

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestMainInsertion1() // Insertion in the middle of the main list
		{
			double[] r1 = new double[] { 0, 1, 2, 3, 4, 100, 5, 6, 7, 8, 9 };
			double[] r2 = new double[] { 0, 2, 4, 100, 6, 8 };
			double[] r3 = new double[] { 1, 3, 5, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			mainList.Insert(5, 100);

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestMainInsertion2() // Insertion at the beginning of the main list
		{
			double[] r1 = new double[] { 100, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			double[] r2 = new double[] { 100, 0, 2, 4, 6, 8 };
			double[] r3 = new double[] { 1, 3, 5, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			mainList.Insert(0, 100);

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestMainInsertion3() // Insertion at the end of the main list
		{
			double[] r1 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 100 };
			double[] r2 = new double[] { 0, 2, 4, 6, 8, 100 };
			double[] r3 = new double[] { 1, 3, 5, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			mainList.Insert(10, 100);

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestPartialInsertion1() // Insertion in the middle of the even list
		{
			double[] r1 = new double[] { 0, 1, 2, 3, 100, 4, 5, 6, 7, 8, 9 };
			double[] r2 = new double[] { 0, 2, 100, 4, 6, 8 };
			double[] r3 = new double[] { 1, 3, 5, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			evenList.Insert(2, 100);

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestPartialInsertion2() // Insertion at the beginning of the even list
		{
			double[] r1 = new double[] { 100, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			double[] r2 = new double[] { 100, 0, 2, 4, 6, 8 };
			double[] r3 = new double[] { 1, 3, 5, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			evenList.Insert(0, 100);

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestPartialInsertion3() // Insertion at the end of the even list
		{
			double[] r1 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 100, 9 };
			double[] r2 = new double[] { 0, 2, 4, 6, 8, 100 };
			double[] r3 = new double[] { 1, 3, 5, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			evenList.Insert(evenList.Count, 100);

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		[Test]
		public void TestPartialInsertion4() // Insertion of an odd item in the even list should cause an exception
		{
			double[] r1 = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			double[] r2 = new double[] { 0, 2, 4, 6, 8 };
			double[] r3 = new double[] { 1, 3, 5, 7, 9 };

			IList<int> mainList, evenList, oddList;
			PartitionCreation(out mainList, out evenList, out oddList);

			try
			{
				evenList.Insert(2, 5);
				Assert.Fail("Insertion of an odd item in the even list should cause an exception");
			}
			catch (Exception)
			{
			}

			CompareLists(r1, mainList, r2, evenList, r3, oddList);
		}

		private static void CompareLists(double[] r1, IList<int> mainList, double[] r2, IList<int> evenList, double[] r3, IList<int> oddList)
		{
			Assert.AreEqual(r1.Length, mainList.Count, "Count of main list unexpected");
			for (int i = 0; i < r1.Length; ++i)
				Assert.AreEqual(r1[i], mainList[i], string.Format("items of main list unequal at index {0}", i));

			Assert.AreEqual(r2.Length, evenList.Count, "Count of even list unexpected");
			for (int i = 0; i < r2.Length; ++i)
				Assert.AreEqual(r2[i], evenList[i], string.Format("items of even list unequal at index {0}", i));

			Assert.AreEqual(r3.Length, oddList.Count, "Count of odd list unexpected");
			for (int i = 0; i < r3.Length; ++i)
				Assert.AreEqual(r3[i], oddList[i], string.Format("items of odd list unequal at index {0}", i));
		}

		#region Mixing of different types

		private class Foo { }

		private class Bloo : Foo { }

		[Test]
		public void TestAddDerivedType1()
		{
			var list = new PartitionableList<Foo>();
			var part = list.CreatePartialViewOfType<Bloo>();

			part.Add(new Bloo());

			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(1, part.Count);
		}

		[Test]
		public void TestSet1()
		{
			var list = new PartitionableList<int>();
			var part = list.CreatePartialView(x => 0 == x % 2);

			list.Add(33);
			Assert.AreEqual(0, part.Count);

			list[0] = 44;
			Assert.AreEqual(1, part.Count);
			Assert.AreEqual(44, part[0]);
		}

		[Test]
		public void TestSet2()
		{
			var list = new PartitionableList<int>();
			var part = list.CreatePartialView(x => 0 == x % 2);

			list.Add(44);
			Assert.AreEqual(1, part.Count);
			Assert.AreEqual(44, part[0]);

			list[0] = 33;
			Assert.AreEqual(0, part.Count);
		}

		[Test]
		public void TestSet3()
		{
			var list = new PartitionableList<int>();
			var part = list.CreatePartialView(x => 0 == x % 2);

			list.Add(64);
			list.Add(66);
			Assert.AreEqual(2, part.Count);

			list[0] = 51;
			Assert.AreEqual(1, part.Count);
			Assert.AreEqual(66, part[0]);
		}

		[Test]
		public void TestSet4()
		{
			var list = new PartitionableList<int>();
			var part = list.CreatePartialView(x => 0 == x % 3);

			list.Add(31);
			list.Add(42);
			list.Add(30);

			Assert.AreEqual(2, part.Count);

			list[0] = 15;
			Assert.AreEqual(3, part.Count);
			Assert.AreEqual(15, part[0]);
			Assert.AreEqual(42, part[1]);
			Assert.AreEqual(30, part[2]);
		}

		[Test]
		public void TestMove1()
		{
			var list = new PartitionableList<int>();
			var part = list.CreatePartialView(x => 0 == x % 2);

			list.Add(81);
			list.Add(67);
			list.Add(46);
			list.Add(49);

			Assert.AreEqual(1, part.Count);

			list.Move(0, 2);
			Assert.AreEqual(1, part.Count);
			Assert.AreEqual(46, part[0]);
		}

		[Test]
		public void TestMove2()
		{
			var list = new PartitionableList<int>();
			var part = list.CreatePartialView(x => 0 == x % 2);

			list.Add(24);
			list.Add(74);
			list.Add(84);

			Assert.AreEqual(3, part.Count);

			list.Move(1, 2);
			Assert.AreEqual(3, part.Count);
			Assert.AreEqual(24, part[0]);
			Assert.AreEqual(84, part[1]);
			Assert.AreEqual(74, part[2]);
		}

		#endregion Mixing of different types

		#region Test with probabilities

		private enum ListAction
		{
			Clear = 0,
			RemoveAt,
			Add,
			InsertAt,
			Set,
			Move
		}

		private class ActionGenerator
		{
			private System.Random _actionRnd = new Random();

			private double[] _weights = new double[6];

			private double[] _prob = new double[6];

			public ActionGenerator()
			{
				for (int i = 0; i < _weights.Length; ++i)
					_weights[i] = 40;

				_weights[0] = 1; // Clear should be weighted low
			}

			public ListAction GetNextAction<T>(IList<T> list)
			{
				_weights[0] = list.Count;

				double sum = ((IList<double>)_weights).Sum();

				double accu = 0;
				for (int i = 0; i < _weights.Length; ++i)
				{
					_prob[i] = accu + _weights[i] / sum;
					accu = _prob[i];
				}

				var rnd = _actionRnd.NextDouble();

				for (int i = 0; i < _prob.Length; ++i)
				{
					if (_prob[i] >= rnd)
						return (ListAction)i;
				}
				return ListAction.Set;
			}
		}

		private static bool Condition1(int i)
		{
			return 0 == (i % 3);
		}

		private static bool Condition2(int i)
		{
			return 0 == (i % 2);
		}

		private static bool Condition3(int i)
		{
			return 0 != (i % 3);
		}

		[Test]
		public void TestRandomActions1()
		{
			var actionGenerator = new ActionGenerator();
			System.Random rndIndex = new System.Random();
			System.Random rndNewNumber = new System.Random();

			var list = new PartitionableList<int>();

			var part1 = list.CreatePartialView(Condition1);
			var part2 = list.CreatePartialView(Condition2);
			var part3 = list.CreatePartialView(Condition3);

			var maxTimeToRun = TimeSpan.FromSeconds(1);
			var watch = new System.Diagnostics.Stopwatch();
			watch.Start();
			var actionStatistics = new int[Enum.GetValues(typeof(ListAction)).Length];
			long accumulatedListCount = 0;
			int maxListCount = 0;

			int numberOfActionsTested = 0;

			for (; watch.Elapsed < maxTimeToRun; ++numberOfActionsTested)
			{
				var action = actionGenerator.GetNextAction(list);
				++actionStatistics[(int)action];
				int idx;
				int idx2;
				int newNumber;
				int? oldItem = null;

				switch (action)
				{
					case ListAction.Clear:
						list.Clear();
						Assert.AreEqual(0, list.Count);
						break;

					case ListAction.RemoveAt:
						if (list.Count > 0)
						{
							var oldCount = list.Count;
							idx = rndIndex.Next(list.Count);
							oldItem = list[idx];
							list.RemoveAt(idx);
							Assert.AreEqual(oldCount - 1, list.Count);
						}
						break;

					case ListAction.Add:
						{
							var oldCount = list.Count;
							list.Add(newNumber = rndNewNumber.Next(100));
							Assert.AreEqual(oldCount + 1, list.Count);
							Assert.AreEqual(newNumber, list[list.Count - 1]);
						}
						break;

					case ListAction.InsertAt:
						{
							var oldCount = list.Count;
							idx = rndIndex.Next(list.Count + 1);
							list.Insert(idx, newNumber = rndNewNumber.Next(100));
							Assert.AreEqual(oldCount + 1, list.Count);
							Assert.AreEqual(newNumber, list[idx]);
						}
						break;

					case ListAction.Set:
						if (list.Count > 0)
						{
							var oldCount = list.Count;
							idx = rndIndex.Next(list.Count);
							oldItem = list[idx];
							list[idx] = (newNumber = rndNewNumber.Next(100));
							Assert.AreEqual(oldCount, list.Count);
							Assert.AreEqual(newNumber, list[idx]);
						}
						break;

					case ListAction.Move:
						if (list.Count > 0)
						{
							var oldCount = list.Count;
							idx = rndIndex.Next(list.Count);
							idx2 = rndIndex.Next(list.Count);
							oldItem = list[idx];
							list.Move(idx, idx2);
							Assert.AreEqual(oldCount, list.Count);
							Assert.AreEqual(oldItem, list[idx2]);
						}
						break;

					default:
						break;
				}

				accumulatedListCount += list.Count;
				maxListCount = Math.Max(maxListCount, list.Count);

				bool succ1 = IsOrderingTestSuccessfull(list, part1, Condition1);
				bool succ2 = IsOrderingTestSuccessfull(list, part2, Condition2);
				bool succ3 = IsOrderingTestSuccessfull(list, part3, Condition3);

				if (!succ1)
				{
				}
				if (!succ2)
				{
				}
				if (!succ3)
				{
				}

				Assert.IsTrue(succ1);
				Assert.IsTrue(succ2);
				Assert.IsTrue(succ3);
			}

			double averageListCount = accumulatedListCount / numberOfActionsTested;

			watch.Stop();
		}

		[Test]
		public void TestRandomActions2()
		{
			var actionGenerator = new ActionGenerator();
			System.Random rndIndex = new System.Random();
			System.Random rndNewNumber = new System.Random();

			var list = new PartitionableList<int>();

			var part1 = list.CreatePartialView(Condition1);
			var part2 = list.CreatePartialView(Condition2);
			var part3 = list.CreatePartialView(Condition3);

			var maxTimeToRun = TimeSpan.FromSeconds(1);
			var watch = new System.Diagnostics.Stopwatch();
			watch.Start();
			var actionStatistics = new int[6];
			long accumulatedListCount = 0;
			int maxListCount = 0;

			int numberOfActionsTested = 0;
			for (; watch.Elapsed < maxTimeToRun; ++numberOfActionsTested)
			{
				var action = actionGenerator.GetNextAction(list);
				++actionStatistics[(int)action];
				int idx, idx2;
				int newNumber;
				int? oldItem = null;

				switch (action)
				{
					case ListAction.Clear:
						part1.Clear();
						Assert.AreEqual(0, part1.Count);

						idx = rndIndex.Next(5);
						if (idx == 0)
						{
							list.Clear();
							idx = rndIndex.Next(10);
							for (int i = 0; i < idx; ++i)
								list.Add(rndNewNumber.Next(100)); // add some numbers to the parent list that not neccessarily fullfil the criterion of part1
						}

						break;

					case ListAction.RemoveAt:
						if (part1.Count > 0)
						{
							var oldCount = part1.Count;
							idx = rndIndex.Next(part1.Count);
							oldItem = part1[idx];
							part1.RemoveAt(idx);
							Assert.AreEqual(oldCount - 1, part1.Count);
						}
						break;

					case ListAction.Add:
						{
							var oldCount = part1.Count;
							part1.Add(newNumber = 3 * rndNewNumber.Next(100));
							Assert.AreEqual(oldCount + 1, part1.Count);
							Assert.AreEqual(newNumber, part1[part1.Count - 1]);
						}
						break;

					case ListAction.InsertAt:
						{
							var oldCount = part1.Count;
							idx = rndIndex.Next(part1.Count + 1);
							part1.Insert(idx, newNumber = 3 * rndNewNumber.Next(100));
							Assert.AreEqual(oldCount + 1, part1.Count);
							Assert.AreEqual(newNumber, part1[idx]);
						}
						break;

					case ListAction.Set:
						if (part1.Count > 0)
						{
							var oldCount = part1.Count;
							idx = rndIndex.Next(part1.Count);
							part1[idx] = (newNumber = 3 * rndNewNumber.Next(100));
							Assert.AreEqual(oldCount, part1.Count);
							Assert.AreEqual(newNumber, part1[idx]);
						}
						break;

					case ListAction.Move:
						if (part1.Count > 0)
						{
							var oldCount = part1.Count;
							idx = rndIndex.Next(part1.Count);
							idx2 = rndIndex.Next(part1.Count);
							oldItem = part1[idx];
							part1.Move(idx, idx2);
							Assert.AreEqual(oldCount, part1.Count);
							Assert.AreEqual(oldItem, part1[idx2]);
						}
						break;

					default:
						break;
				}

				accumulatedListCount += list.Count;
				maxListCount = Math.Max(maxListCount, list.Count);

				bool succ1 = IsOrderingTestSuccessfull(list, part1, Condition1);
				bool succ2 = IsOrderingTestSuccessfull(list, part2, Condition2);
				bool succ3 = IsOrderingTestSuccessfull(list, part3, Condition3);

				if (!succ1)
				{
				}
				if (!succ2)
				{
				}
				if (!succ3)
				{
				}

				Assert.IsTrue(succ1);
				Assert.IsTrue(succ2);
				Assert.IsTrue(succ3);
			}

			double averageListCount = accumulatedListCount / numberOfActionsTested;

			watch.Stop();
		}

		private static bool IsOrderingTestSuccessfull<T>(IList<T> parent, IList<T> child, Func<T, bool> selection)
		{
			return child.SequenceEqual(parent.Where(selection));
		}

		#endregion Test with probabilities
	}
}