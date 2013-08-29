#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using Altaxo.Collections;
using NUnit.Framework;


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

			Assert.AreEqual( max, list.Count, "Count of the list unexpected");
			for (int i = 0; i < max; ++i)
				Assert.AreEqual(i, list[i], string.Format("items unequal at index {0}", i));

			Assert.AreEqual(max / 2, evenList.Count, "Count of the even list unexpected");

			for (int i = 0; i < max / 2; ++i)
				Assert.AreEqual(i * 2, evenList[i], string.Format("items of even list unequal at index {0}", i));


			Assert.AreEqual( max/2, oddList.Count, "Count of the odd list unexpected");

			for (int i = 0; i < max/2; ++i)
				Assert.AreEqual(i*2+1, oddList[i], string.Format("items of odd list unequal at index {0}", i));

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

			oddList.RemoveAt(oddList.Count-1); // list contain all elements but not 9

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
			double[] r2 = new double[] {  };
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

		class Foo {}
		class Bloo : Foo {}


		[Test]
		public void TestAddDerivedType1() 
		{
			var list = new PartitionableList<Foo>();
			var part = list.CreatePartialViewOfType<Bloo>();

			part.Add(new Bloo());

			Assert.AreEqual(1,list.Count);
			Assert.AreEqual(1, part.Count);

		}


		#endregion

	}
}
