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
using System.IO;
using Altaxo.Data;
using Xunit;

namespace Altaxo.Serialization.OpenXml.Excel
{
  public class ExcelImporterTests
  {
    public string TestFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Serialization\\OpenXml\\Excel\\TestFiles");

    public FileStream GetFileStream(string fileName)
    {
      return new FileStream(Path.Combine(TestFilePath, fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    [Fact]
    public void Test_AllFilesReadable()
    {
      var testFiles = new DirectoryInfo(TestFilePath).GetFiles("*.xlsx");
      Assert.NotEmpty(testFiles);
      foreach (var file in testFiles)
      {
        var reader = new ExcelImporter();
        DataTable table = new DataTable();
        reader.Import([file.FullName], table, new ExcelImportOptions(), attachDataSource: false);
      }
    }

    [Fact]
    public void Test_TestFile1()
    {
      var testFiles = new DirectoryInfo(TestFilePath).GetFiles("TestFile1.xlsx");
      Assert.Single(testFiles);

      var importOptions = new ExcelImportOptions { UseNeutralColumnName = false };
      var reader = new ExcelImporter();

      DataTable table = new DataTable();
      reader.Import([testFiles[0].FullName], table, importOptions, attachDataSource: false);

      Assert.Equal(4, table.DataColumns.ColumnCount);
      Assert.IsType<DoubleColumn>(table.DataColumns[0]);
      Assert.Equal(3, table.DataColumns[0].Count);
      Assert.Equal(4.5, table.DataColumns[0][0], 0);
      Assert.Equal(5.5, table.DataColumns[0][1], 0);
      Assert.Equal(6.5, table.DataColumns[0][2], 0);

      Assert.IsType<DoubleColumn>(table.DataColumns[1]);
      Assert.Equal(3, table.DataColumns[1].Count);
      Assert.Equal(13, table.DataColumns[1][0], 0);
      Assert.Equal(14, table.DataColumns[1][1], 0);
      Assert.Equal(15, table.DataColumns[1][2], 0);

      // TODO: the third column is currently a double column, but should be a DateTime column.

      Assert.IsType<TextColumn>(table.DataColumns[3]);
      Assert.Equal(3, table.DataColumns[3].Count);
      Assert.Equal("ABC", table.DataColumns[3][0]);
      Assert.Equal("DEF", table.DataColumns[3][1]);
      Assert.Equal("GHI", table.DataColumns[3][2]);

      // test the column names
      Assert.Equal("Col1", table.DataColumns.GetColumnName(0));
      Assert.Equal("Col2", table.DataColumns.GetColumnName(1));
      Assert.Equal("Col3", table.DataColumns.GetColumnName(2));
      Assert.Equal("Col4", table.DataColumns.GetColumnName(3));

    }


    [Fact]
    public void Test_TestFile2()
    {
      var testFiles = new DirectoryInfo(TestFilePath).GetFiles("TestFile2.xlsx");
      Assert.Single(testFiles);

      var importOptions = new ExcelImportOptions { UseNeutralColumnName = false };
      var reader = new ExcelImporter();

      DataTable table = new DataTable();
      reader.Import([testFiles[0].FullName], table, importOptions, attachDataSource: false);

      Assert.Equal(4, table.DataColumns.ColumnCount);
      Assert.IsType<DoubleColumn>(table.DataColumns[0]);
      Assert.Equal(3, table.DataColumns[0].Count);
      Assert.Equal(4.5, table.DataColumns[0][0], 0);
      Assert.Equal(5.5, table.DataColumns[0][1], 0);
      Assert.Equal(6.5, table.DataColumns[0][2], 0);

      Assert.IsType<DoubleColumn>(table.DataColumns[1]);
      Assert.Equal(3, table.DataColumns[1].Count);
      Assert.Equal(13, table.DataColumns[1][0], 0);
      Assert.Equal(14, table.DataColumns[1][1], 0);
      Assert.Equal(15, table.DataColumns[1][2], 0);

      // TODO: the third column is currently a double column, but should be a DateTime column.

      Assert.IsType<TextColumn>(table.DataColumns[3]);
      Assert.Equal(3, table.DataColumns[3].Count);
      Assert.Equal("ABC", table.DataColumns[3][0]);
      Assert.Equal("DEF", table.DataColumns[3][1]);
      Assert.Equal("GHI", table.DataColumns[3][2]);

      // test the column names
      Assert.Equal("Col1", table.DataColumns.GetColumnName(0));
      Assert.Equal("Col2", table.DataColumns.GetColumnName(1));
      Assert.Equal("Col3", table.DataColumns.GetColumnName(2));
      Assert.Equal("Col4", table.DataColumns.GetColumnName(3));
    }

    [Fact]
    public void Test_TestFile3()
    {
      var testFiles = new DirectoryInfo(TestFilePath).GetFiles("TestFile3.xlsx");
      Assert.Single(testFiles);

      var importOptions = new ExcelImportOptions { UseNeutralColumnName = false };
      var reader = new ExcelImporter();

      DataTable table = new DataTable();
      reader.Import([testFiles[0].FullName], table, importOptions, attachDataSource: false);

      Assert.Equal(4, table.DataColumns.ColumnCount);
      Assert.IsType<DoubleColumn>(table.DataColumns[0]);
      Assert.Equal(6, table.DataColumns[0].Count);
      Assert.Equal(4.5, table.DataColumns[0][0], 0);
      Assert.Equal(5.5, table.DataColumns[0][2], 0);
      Assert.Equal(6.5, table.DataColumns[0][5], 0);

      Assert.IsType<DoubleColumn>(table.DataColumns[1]);
      Assert.Equal(6, table.DataColumns[1].Count);
      Assert.Equal(13, table.DataColumns[1][0], 0);
      Assert.Equal(14, table.DataColumns[1][2], 0);
      Assert.Equal(15, table.DataColumns[1][5], 0);

      // TODO: the third column is currently a double column, but should be a DateTime column.

      Assert.IsType<TextColumn>(table.DataColumns[3]);
      Assert.Equal(5, table.DataColumns[3].Count);
      Assert.Equal("ABC", table.DataColumns[3][1]);
      Assert.Equal("DEF", table.DataColumns[3][3]);
      Assert.Equal("GHI", table.DataColumns[3][4]);

      // test the column names
      Assert.Equal("Col1", table.DataColumns.GetColumnName(0));
      Assert.Equal("Col2", table.DataColumns.GetColumnName(1));
      Assert.Equal("Col3", table.DataColumns.GetColumnName(2));
      Assert.Equal("Col4", table.DataColumns.GetColumnName(3));
    }

  }
}
