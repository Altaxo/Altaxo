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
#endregion

using System;
using System.IO;
using System.Collections.Generic;

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
    /// <param name="options">The options controlling the export process.</param>
    static protected void ExportDataColumnNames(StreamWriter strwr, Altaxo.Data.DataTable table, AsciiExportOptions options)
    {
      int nColumns = table.DataColumns.ColumnCount;
      for(int i=0;i<nColumns;i++)
      {
        strwr.Write(options.ConvertToSaveString(table.DataColumns.GetColumnName(i)));
  
        if((i+1)<nColumns)
          strwr.Write(options.SeparatorChar);
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
    /// <param name="options">The Ascii export options.</param>
    static protected void ExportPropertyColumns(StreamWriter strwr, Altaxo.Data.DataColumnCollection columnCollection, int nDataColumns, AsciiExportOptions options)
    {
      int nPropColumns = columnCollection.ColumnCount;

      for(int i=0;i<nPropColumns;i++)
      {
        string columnName = options.ConvertToSaveString(columnCollection.GetColumnName(i));
        columnName += "=";
        bool isTextColumn = columnCollection[i] is Data.TextColumn;

        for (int j = 0; j < nDataColumns; j++)
        {
          if(!columnCollection[i].IsElementEmpty(j))
          {
            string data = options.DataItemToString(columnCollection[i], j);
            if(options.ExportPropertiesWithName && !isTextColumn && !data.Contains("="))
              strwr.Write(columnName);

            strwr.Write(data);
          }
          if((j+1)<nDataColumns)
            strwr.Write(options.SeparatorChar);
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
    /// <param name="options">The options used for exporting the data.</param>
    static protected void ExportDataColumns(
      StreamWriter strwr, 
      Altaxo.Data.DataColumnCollection columnCollection, 
      AsciiExportOptions options
      )
    {
      int nRows = columnCollection.RowCount;
      int nColumns = columnCollection.ColumnCount;

      for (int i = 0; i < nRows; i++)
      {
        for (int j = 0; j < nColumns; j++)
        {
          strwr.Write(options.DataItemToString(columnCollection[j], i));

          if ((j + 1) < nColumns)
            strwr.Write(options.SeparatorChar);
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
    /// <param name="options">The options used for exporting of the data.</param>
    static public void ExportAscii(System.IO.Stream myStream, Altaxo.Data.DataTable table, AsciiExportOptions options)
    {
      StreamWriter strwr = new StreamWriter(myStream, System.Text.Encoding.Default); // Change to Unicode or quest encoding by a dialog box

      if(options.ExportDataColumnNames)
        ExportDataColumnNames(strwr,table,options);
    
      if(options.ExportPropertyColumns)
        ExportPropertyColumns(strwr,table.PropCols,table.DataColumns.ColumnCount,options);

      ExportDataColumns(strwr,table.DataColumns,options);

      strwr.Flush();
    }


     /// <summary>
    /// Exports a table to Ascii using a stream. The stream is <b>not</b> closed at the end of this function.
    /// </summary>
    /// <param name="myStream">The stream the table should be exported to.</param>
    /// <param name="table">The table that is to be exported.</param>
    /// <param name="separator">The separator char that separates the items from each other.</param>
    static public void ExportAscii(System.IO.Stream myStream, Altaxo.Data.DataTable table, char separator)
    {
      AsciiExportOptions options = new AsciiExportOptions();
      options.SetSeparator(separator);
      ExportAscii(myStream, table, options);
    }

    /// <summary>
    /// Exports a table into an Ascii file.
    /// </summary>
    /// <param name="filename">The filename used to export the file.</param>
    /// <param name="table">The table to export.</param>
    /// <param name="options">The Ascii export options used for export.</param>
    static public void ExportAscii(string filename, Altaxo.Data.DataTable table, AsciiExportOptions options)
    {

      using (System.IO.FileStream myStream = new System.IO.FileStream(filename, System.IO.FileMode.Create))
      {
        ExportAscii(myStream, table, options);
        myStream.Close();
      }
    }

    /// <summary>
    /// Exports a table into an Ascii file.
    /// </summary>
    /// <param name="filename">The filename used to export the file.</param>
    /// <param name="table">The table to export.</param>
    /// <param name="separator">The separator char used to export the table. Normally, you should use a tabulator here.</param>
    static public void ExportAscii(string filename, Altaxo.Data.DataTable table, char separator)
    {
      AsciiExportOptions options = new AsciiExportOptions();
      options.SetSeparator( separator );
      ExportAscii(filename, table, options);
    }

  }

 
}
