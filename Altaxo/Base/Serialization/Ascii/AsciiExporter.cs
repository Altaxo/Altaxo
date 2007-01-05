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
using System.IO;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// AsciiExporter provides some static methods to export tables or columns to ascii files
  /// </summary>
  public class AsciiExporter
  {
    /// <summary>
    /// Exports the data column names of a table into a single line of ascii.
    /// </summary>
    /// <param name="strwr">A stream writer to write the ascii data to.</param>
    /// <param name="table">The data table whichs data column names should be exported.</param>
    /// <param name="separator">The separator char. If the text to export contains the separator char, it is replaced by a space.</param>
    static protected void ExportDataColumnNames(StreamWriter strwr, Altaxo.Data.DataTable table, char separator)
    {
      int nColumns = table.DataColumns.ColumnCount;
      for(int i=0;i<nColumns;i++)
      {
        strwr.Write(table.DataColumns.GetColumnName(i).Replace(separator,' '));
  
        if((i+1)<nColumns)
          strwr.Write(separator);
        else 
          strwr.WriteLine();
  
      }
    }

    /// <summary>
    /// Exports the property columns into Ascii. Each property column is exported into one row (line).
    /// </summary>
    /// <param name="strwr">A stream writer to write the ascii data to.</param>
    /// <param name="columnCollection">The column collection to export.</param>
    /// <param name="nDataColumns">The number of data columns of the table -> is the number of elements in each property column that must be exported.</param>
    /// <param name="separator">The separator char. If the text to export contains the separator char, it is replaced by a space.</param>
    static protected void ExportPropertyColumns(StreamWriter strwr, Altaxo.Data.DataColumnCollection columnCollection, int nDataColumns, char separator)
    {
      int nPropColumns = columnCollection.ColumnCount;

      for(int i=0;i<nPropColumns;i++)
      {
        for(int j=0;j<nDataColumns;j++)
        {
          if(!columnCollection[i].IsElementEmpty(j))
          {
            strwr.Write(columnCollection[i][j].ToString().Replace(separator,' '));
          }
          if((j+1)<nDataColumns)
            strwr.Write(separator);
          else 
            strwr.WriteLine();
        }
      }

    }


    /// <summary>
    /// Exports the data columns into Ascii. Each data row is exported into one row (line).
    /// </summary>
    /// <param name="strwr">A stream writer to write the ascii data to.</param>
    /// <param name="columnCollection">The column collection to export.</param>
    /// <param name="separator">The separator char. If the text to export contains the separator char, it is replaced by a space.</param>
    static protected void ExportDataColumns(StreamWriter strwr, Altaxo.Data.DataColumnCollection columnCollection, char separator)
    {
      int nRows = columnCollection.RowCount;
      int nColumns = columnCollection.ColumnCount;

      for(int i=0;i<nRows;i++)
      {
        for(int j=0;j<nColumns;j++)
        {
          if(!columnCollection[j].IsElementEmpty(i))
          {
            strwr.Write(columnCollection[j][i].ToString().Replace(separator,' '));
          }
          if((j+1)<nColumns)
            strwr.Write(separator);
          else 
            strwr.WriteLine();
        }
      }
    }

    /// <summary>
    /// Exports a table to Ascii using a stream. The stream is <b>not</b> closed at the end of this function.
    /// </summary>
    /// <param name="myStream">The stream the table should be exported to.</param>
    /// <param name="table">The table that is to be exported.</param>
    /// <param name="separator">The separator char that separates the items from each other.</param>
    static public void ExportAscii(System.IO.Stream myStream, Altaxo.Data.DataTable table, char separator)
    {
      StreamWriter strwr = new StreamWriter(myStream, System.Text.Encoding.Default); // Change to Unicode or quest encoding by a dialog box

      ExportDataColumnNames(strwr,table,separator);
    
      ExportPropertyColumns(strwr,table.PropCols,table.DataColumns.ColumnCount,separator);

      ExportDataColumns(strwr,table.DataColumns,separator);

      strwr.Flush();
    }


    /// <summary>
    /// Exports a table into an Ascii file.
    /// </summary>
    /// <param name="filename">The filename used to export the file.</param>
    /// <param name="table">The table to export.</param>
    /// <param name="separator">The separator char used to export the table. Normally, you should use a tabulator here.</param>
    static public void ExportAscii(string filename, Altaxo.Data.DataTable table, char separator)
    {

      System.IO.FileStream myStream = new System.IO.FileStream(filename,System.IO.FileMode.Create);

      ExportAscii(myStream,table,separator);

      myStream.Close();
    }

  }
}
