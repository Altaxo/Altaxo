#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using Altaxo.Data;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// AsciiExporter provides some static methods to export tables or columns to ascii files
  /// </summary>
  public class AsciiExporter
  {
    /// <summary>
    /// Holds a dictionary of column types (keys) and functions (values), which convert a AltaxoVariant into a string.
    /// Normally the ToString() function is used on AltaxoVariant to convert to string. By using this dictionary it is possible
    /// to add custom converters.
    /// </summary>
    private Dictionary<System.Type, Func<Altaxo.Data.AltaxoVariant, string>> _typeConverters;

    private AsciiExportOptions _exportOptions;

    public AsciiExporter()
      : this(new AsciiExportOptions())
    {
    }

    public AsciiExporter(AsciiExportOptions options)
    {
      _exportOptions = options ?? throw new ArgumentNullException(nameof(options));

      _typeConverters = new Dictionary<Type, Func<Altaxo.Data.AltaxoVariant, string>>
      {
        { typeof(DoubleColumn), GetDefaultConverter(typeof(DoubleColumn), options) },
        { typeof(DateTimeColumn), GetDefaultConverter(typeof(DateTimeColumn), options) },
        { typeof(TextColumn), GetDefaultConverter(typeof(TextColumn), options) }
      };
    }

    public Func<Altaxo.Data.AltaxoVariant, string>? GetConverter(System.Type columnType)
    {
      if (_typeConverters.TryGetValue(columnType, out var result))
        return result;
      else
        return null;
    }

    /// <summary>
    /// Returns the default converter for a given column type.
    /// </summary>
    /// <param name="columnType">The column type.</param>
    /// <returns>Default converter for the given column type.</returns>
    public Func<AltaxoVariant, string> GetDefaultConverter(System.Type columnType, AsciiExportOptions options)
    {
      if (columnType == typeof(DoubleColumn))
      {
        return DefaultDoubleConverter;
      }
      else if (columnType == typeof(DateTimeColumn))
      {
        if (string.IsNullOrEmpty(options.DateTimeFormat))
        {
          return DefaultDateTimeConverter;
        }
        else
        {
          return x => FormattedDateTimeConverter(x, options.DateTimeFormat);
        }
      }
      else
      {
        return DefaultTextConverter;
      }
    }

    /// <summary>
    /// Sets the converter for the items of a specific column type.
    /// </summary>
    /// <param name="columnType">The column type for which to set the converter.</param>
    /// <param name="stringConverter">The converter function, which converts an AltaxoVariant into a string.</param>
    public void SetConverter(System.Type columnType, Func<Altaxo.Data.AltaxoVariant, string> stringConverter)
    {
      if (columnType is null)
        throw new ArgumentNullException(nameof(columnType));

      if (stringConverter is not null)
        _typeConverters[columnType] = stringConverter;
      else // stringConverter is null, try to get the default converter
        _typeConverters[columnType] = GetDefaultConverter(columnType, _exportOptions);
    }

    /// <summary>
    /// Exports the data column names of a table into a single line of ascii.
    /// </summary>
    /// <param name="strwr">A stream writer to write the ascii data to.</param>
    /// <param name="table">The data table whichs data column names should be exported.</param>
    /// <param name="options">The options controlling the export process.</param>
    protected void ExportDataColumnNames(StreamWriter strwr, Altaxo.Data.DataTable table)
    {
      int nColumns = table.DataColumns.ColumnCount;
      for (int i = 0; i < nColumns; i++)
      {
        strwr.Write(ConvertToSaveString(table.DataColumns.GetColumnName(i)));

        if ((i + 1) < nColumns)
          strwr.Write(_exportOptions.SeparatorChar);
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
    protected void ExportPropertyColumns(StreamWriter strwr, Altaxo.Data.DataColumnCollection columnCollection, int nDataColumns)
    {
      int nPropColumns = columnCollection.ColumnCount;

      for (int i = 0; i < nPropColumns; i++)
      {
        string columnName = ConvertToSaveString(columnCollection.GetColumnName(i));
        columnName += "=";
        bool isTextColumn = columnCollection[i] is Data.TextColumn;

        for (int j = 0; j < nDataColumns; j++)
        {
          if (!columnCollection[i].IsElementEmpty(j))
          {
            string data = DataItemToString(columnCollection[i], j);
            if (_exportOptions.ExportPropertiesWithName && !isTextColumn && !data.Contains("="))
              strwr.Write(columnName);

            strwr.Write(data);
          }
          if ((j + 1) < nDataColumns)
            strwr.Write(_exportOptions.SeparatorChar);
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
    protected void ExportDataColumns(
      StreamWriter strwr,
      Altaxo.Data.DataColumnCollection columnCollection
      )
    {
      int nRows = columnCollection.RowCount;
      int nColumns = columnCollection.ColumnCount;

      for (int i = 0; i < nRows; i++)
      {
        for (int j = 0; j < nColumns; j++)
        {
          strwr.Write(DataItemToString(columnCollection[j], i));

          if ((j + 1) < nColumns)
            strwr.Write(_exportOptions.SeparatorChar);
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
    public static void ExportAscii(System.IO.Stream myStream, Altaxo.Data.DataTable table, AsciiExportOptions options)
    {
      var exporter = new AsciiExporter(options);
      exporter.ExportAscii(myStream, table);
    }

    /// <summary>
    /// Exports a table to Ascii using a stream. The stream is <b>not</b> closed at the end of this function.
    /// </summary>
    /// <param name="myStream">The stream the table should be exported to.</param>
    /// <param name="table">The table that is to be exported.</param>
    /// <param name="options">The options used for exporting of the data.</param>
    public void ExportAscii(System.IO.Stream myStream, Altaxo.Data.DataTable table)
    {
      var strwr = new StreamWriter(myStream, System.Text.Encoding.Default); // Change to Unicode or quest encoding by a dialog box

      if (_exportOptions.ExportDataColumnNames)
        ExportDataColumnNames(strwr, table);

      if (_exportOptions.ExportPropertyColumns)
        ExportPropertyColumns(strwr, table.PropCols, table.DataColumns.ColumnCount);

      ExportDataColumns(strwr, table.DataColumns);

      strwr.Flush();
    }

    /// <summary>
    /// Exports a table to Ascii using a stream. The stream is <b>not</b> closed at the end of this function.
    /// </summary>
    /// <param name="myStream">The stream the table should be exported to.</param>
    /// <param name="table">The table that is to be exported.</param>
    /// <param name="separator">The separator char that separates the items from each other.</param>
    public static void ExportAscii(System.IO.Stream myStream, Altaxo.Data.DataTable table, char separator)
    {
      var options = new AsciiExportOptions().WithSeparator(separator);
      ExportAscii(myStream, table, options);
    }

    /// <summary>
    /// Exports a table into an Ascii file.
    /// </summary>
    /// <param name="filename">The filename used to export the file.</param>
    /// <param name="table">The table to export.</param>
    /// <param name="options">The Ascii export options used for export.</param>
    public static void ExportAscii(string filename, Altaxo.Data.DataTable table, AsciiExportOptions options)
    {
      using (var myStream = new System.IO.FileStream(filename, System.IO.FileMode.Create))
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
    public static void ExportAscii(string filename, Altaxo.Data.DataTable table, char separator)
    {
      var options = new AsciiExportOptions().WithSeparator(separator);
      ExportAscii(filename, table, options);
    }

    #region Helper function

    /// <summary>
    /// Converts a data item to a string.
    /// </summary>
    /// <param name="col">The data column.</param>
    /// <param name="index">Index of the item in the data column, which should be converted.</param>
    /// <returns>The converted item as string.</returns>
    public string DataItemToString(Altaxo.Data.DataColumn col, int index)
    {
      if (col.IsElementEmpty(index))
        return string.Empty;

      string result;
      if (_typeConverters.TryGetValue(col.GetType(), out var func))
        result = func(col[index]);
      else
        result = DefaultTextConverter(col[index]);

      return result.Replace(_exportOptions.SeparatorChar, _exportOptions.SubstituteForSeparatorChar);
    }

    /// <summary>
    /// Converts a given string to a string which will not contain the separator char nor contains newlines.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public string ConvertToSaveString(string s)
    {
      s = s.Replace(_exportOptions.SeparatorChar, _exportOptions.SubstituteForSeparatorChar);
      s = s.Replace('\r', ' ');
      s = s.Replace('\n', ' ');
      return s;
    }

    private string DefaultTextConverter(AltaxoVariant x)
    {
      string s = x.ToString();

      s = s.Replace('\r', ' ');
      s = s.Replace('\n', ' ');

      return s;
    }

    private string DefaultDoubleConverter(AltaxoVariant x)
    {
      return ((double)x).ToString("r", _exportOptions.Culture);
    }

    private string DefaultDateTimeConverter(AltaxoVariant x)
    {
      return ((DateTime)x).ToString("o", _exportOptions.Culture);
    }

    private string FormattedDateTimeConverter(AltaxoVariant x, string formatString)
    {
      return ((DateTime)x).ToString(formatString, _exportOptions.Culture);
    }

    #endregion
  }
}
