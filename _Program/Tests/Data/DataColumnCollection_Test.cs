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
