#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using NUnit.Framework;
using Altaxo.Data;

namespace AltaxoTest.Data
{
	/// <summary>
	/// Summary description for DoubleColumn_Test.
	/// </summary>
	[TestFixture]
	public class DataColumnCollection_Test 
	{
				
		[Test]
		public void ZeroColumns()
		{
			DataColumnCollection d = new DataColumnCollection();
			Assertion.AssertEquals(0,d.ColumnCount);
			Assertion.AssertEquals(0,d.RowCount);
			Assertion.AssertEquals(false,d.IsDirty);
			Assertion.AssertEquals(false,d.IsSuspended);
		}

		[Test]
		public void TenEmptyColumns()
		{
			DataColumnCollection d = new DataColumnCollection();

			DataColumn[] cols = new DataColumn[10];
			for(int i=0;i<10;i++)
			{
				cols[i] = new DoubleColumn();
				d.Add(cols[i]);
			}

			Assertion.AssertEquals(10,d.ColumnCount);
			Assertion.AssertEquals(0,d.RowCount);
			Assertion.AssertEquals(false,d.IsDirty);
			Assertion.AssertEquals(false,d.IsSuspended);

			Assertion.AssertEquals("A",d.GetColumnName(0));
			Assertion.AssertEquals("A",d[0].Name);
	
			Assertion.AssertEquals("J",d.GetColumnName(9));
			Assertion.AssertEquals("J",d[9].Name);


			// Test index to column resolution
			for(int i=0;i<10;i++)
				Assertion.AssertEquals(cols[i],d[i]);

			// test name to column resolution

			for(int i=0;i<10;i++)
			{
				char name = (char)('A'+i);
				Assertion.AssertEquals("Column to name resolution of col " + name.ToString(), cols[i],d[name.ToString()]);
			}
			// test column to number resolution
			for(int i=0;i<10;i++)
				Assertion.AssertEquals(i,d.GetColumnNumber(cols[i]));

		}


	}
}
