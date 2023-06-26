#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using Xunit;

namespace Altaxo.Data
{
  /// <summary>
  /// Summary description for DoubleColumn_Test.
  /// </summary>
  public class DataColumnCollection_Test
  {
    [Fact]
    public void ZeroColumns()
    {
      var d = new DataColumnCollection();
      Assert.Equal(0, d.ColumnCount);
      Assert.Equal(0, d.RowCount);
      Assert.False(d.IsDirty);
      Assert.False(d.IsSuspended);
    }

    [Fact]
    public void TenEmptyColumns()
    {
      var d = new DataColumnCollection();

      var cols = new DataColumn[10];
      for (int i = 0; i < 10; i++)
      {
        cols[i] = new DoubleColumn();
        d.Add(cols[i]);
      }

      Assert.Equal(10, d.ColumnCount);
      Assert.Equal(0, d.RowCount);
      Assert.False(d.IsDirty);
      Assert.False(d.IsSuspended);

      Assert.Equal("A", d.GetColumnName(0));
      Assert.Equal("A", d[0].Name);

      Assert.Equal("J", d.GetColumnName(9));
      Assert.Equal("J", d[9].Name);

      // Test index to column resolution
      for (int i = 0; i < 10; i++)
        Assert.Equal(cols[i], d[i]);

      // test name to column resolution

      for (int i = 0; i < 10; i++)
      {
        char name = (char)('A' + i);
        Assert.True(cols[i] == d[name.ToString()], "Column to name resolution of col " + name.ToString());
      }
      // test column to number resolution
      for (int i = 0; i < 10; i++)
        Assert.Equal(i, d.GetColumnNumber(cols[i]));
    }
  }
}
