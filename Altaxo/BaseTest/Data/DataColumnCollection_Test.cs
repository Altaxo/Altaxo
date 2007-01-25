#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
      Assert.AreEqual(0, d.ColumnCount);
      Assert.AreEqual(0, d.RowCount);
      Assert.AreEqual(false, d.IsDirty);
      Assert.AreEqual(false, d.IsSuspended);
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

      Assert.AreEqual(10, d.ColumnCount);
      Assert.AreEqual(0, d.RowCount);
      Assert.AreEqual(false, d.IsDirty);
      Assert.AreEqual(false, d.IsSuspended);

      Assert.AreEqual("A", d.GetColumnName(0));
      Assert.AreEqual("A", d[0].Name);

      Assert.AreEqual("J", d.GetColumnName(9));
      Assert.AreEqual("J", d[9].Name);


      // Test index to column resolution
      for(int i=0;i<10;i++)
        Assert.AreEqual(cols[i], d[i]);

      // test name to column resolution

      for(int i=0;i<10;i++)
      {
        char name = (char)('A'+i);
        Assert.AreEqual(cols[i], d[name.ToString()],"Column to name resolution of col " + name.ToString());
      }
      // test column to number resolution
      for(int i=0;i<10;i++)
        Assert.AreEqual(i, d.GetColumnNumber(cols[i]));

    }


  }
}
