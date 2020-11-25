#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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
using Altaxo.Collections;

namespace Altaxo.Data
{

  public static class DecomposeByColumnContentActions
  {
    public static void ShowDecomposeByColumnContentDialog(this DataTable srcTable, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns)
    {
      DataTableMultipleColumnProxy? proxy = null;
      DecomposeByColumnContentOptions? options = null;

      try
      {
        proxy = new DataTableMultipleColumnProxy(DecomposeByColumnContentDataAndOptions.ColumnsParticipatingIdentifier, srcTable, selectedDataRows, selectedDataColumns);
        proxy.EnsureExistenceOfIdentifier(DecomposeByColumnContentDataAndOptions.ColumnWithCyclingVariableIdentifier, 1);

        options = new DecomposeByColumnContentOptions();
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(string.Format("{0}\r\nDetails:\r\n{1}", ex.Message, ex.ToString()), "Error in preparation of 'Decompose by column content'");
        return;
      }

      var dataAndOptions = new DecomposeByColumnContentDataAndOptions(proxy, options);

      // in order to show the column names etc in the dialog, it is neccessary to set the source
      if (true == Current.Gui.ShowDialog(ref dataAndOptions, "Choose options", false))
      {
        var destTable = new DataTable();
        proxy = dataAndOptions.Data;
        options = dataAndOptions.Options;

        string? error = null;
        try
        {
          error = DecomposeByColumnContent(dataAndOptions.Data, dataAndOptions.Options, destTable);
        }
        catch (Exception ex)
        {
          error = ex.ToString();
        }
        if (error is not null)
          Current.Gui.ErrorMessageBox(error);

        destTable.Name = srcTable.Name + "_Decomposed";

        // Create a DataSource
        var dataSource = new DecomposeByColumnContentDataSource(proxy, options, new Altaxo.Data.DataSourceImportOptions());
        destTable.DataSource = dataSource;

        Current.Project.DataTableCollection.Add(destTable);
        Current.IProjectService.ShowDocumentView(destTable);
      }
    }

    /// <summary>
    /// Decompose the source columns according to the provided options. The source table and the settings are provided in the <paramref name="options"/> variable.
    /// The provided destination table is cleared from all data and property values before.
    /// </summary>
    /// <param name="inputData">The data containing the source table, the participating columns and the column with the cycling variable.</param>
    /// <param name="options">The settings for decomposing.</param>
    /// <param name="destTable">The destination table. Any data will be removed before filling with the new data.</param>
    /// <returns>Null if the method finishes successfully, or an error information.</returns>
    public static string? DecomposeByColumnContent(DataTableMultipleColumnProxy inputData, DecomposeByColumnContentOptions options, DataTable destTable)
    {
      var srcTable = inputData.DataTable;
      if (srcTable is null)
        return "Source data table is null";

      try
      {
        DecomposeByColumnContentDataAndOptions.EnsureCoherence(inputData, true);
      }
      catch (Exception ex)
      {
        return ex.Message;
      }

      destTable.DataColumns.RemoveColumnsAll();
      destTable.PropCols.RemoveColumnsAll();

      var srcCycCol = inputData.GetDataColumnOrNull(DecomposeByColumnContentDataAndOptions.ColumnWithCyclingVariableIdentifier);
      if (srcCycCol is null)
        return "Could not get column with cycling variable";

      var decomposedValues = Decompose(srcCycCol);
      // the decomposedValues are not sorted yes

      if (options.DestinationColumnSorting == DecomposeByColumnContentOptions.OutputSorting.Ascending)
      {
        decomposedValues.Sort();
      }
      else if (options.DestinationColumnSorting == DecomposeByColumnContentOptions.OutputSorting.Descending)
      {
        decomposedValues.Sort();
        decomposedValues.Reverse();
      }
      // get the other columns to process

      var srcColumnsToProcess = new List<DataColumn>(inputData.GetDataColumns(DecomposeByColumnContentDataAndOptions.ColumnsParticipatingIdentifier));
      // subtract the column containing the decompose values
      srcColumnsToProcess.Remove(srcCycCol);

      // the only property column that is now usefull is that with the repeated values
      var destPropCol = destTable.PropCols.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCycCol), srcCycCol.GetType(), srcTable.DataColumns.GetColumnKind(srcCycCol), srcTable.DataColumns.GetColumnGroup(srcCycCol));

      if (options.DestinationOutput == DecomposeByColumnContentOptions.OutputFormat.GroupOneColumn)
      {
        // columns originating from the same column but with different property are grouped together, but they will get different group numbers
        foreach (var srcCol in srcColumnsToProcess)
        {
          int nCreatedCol = -1;
          int nCreatedProp = 0;
          foreach (var prop in decomposedValues)
          {
            ++nCreatedCol;
            ++nCreatedProp;
            var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCol) + "." + nCreatedCol.ToString(), srcCol.GetType(), srcTable.DataColumns.GetColumnKind(srcCol), nCreatedProp);
            var nDestCol = destTable.DataColumns.GetColumnNumber(destCol);
            for (int i = 0, j = 0; i < srcCycCol.Count; ++i)
            {
              if (prop == srcCycCol[i])
              {
                destCol[j] = srcCol[i];
                ++j;
              }
            }
            // fill also property column
            destPropCol[nDestCol] = prop;
          }
        }
      }
      else if (options.DestinationOutput == DecomposeByColumnContentOptions.OutputFormat.GroupAllColumns)
      {
        // all columns with the same property are grouped together, and those columns will share the same group number
        int nCreatedCol = -1; // running number of processed range for column creation (Naming)
        int nCreatedProp = -1;
        foreach (var prop in decomposedValues)
        {
          ++nCreatedProp;
          ++nCreatedCol;

          foreach (var srcCol in srcColumnsToProcess)
          {
            var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCol) + "." + nCreatedCol.ToString(), srcCol.GetType(), srcTable.DataColumns.GetColumnKind(srcCol), nCreatedProp);
            var nDestCol = destTable.DataColumns.GetColumnNumber(destCol);
            for (int i = 0, j = 0; i < srcCycCol.Count; ++i)
            {
              if (prop == srcCycCol[i])
              {
                destCol[j] = srcCol[i];
                ++j;
              }
            }
            // fill also property column
            destPropCol[nDestCol] = prop;
          }
        }
      }
      else
      {
        throw new NotImplementedException("The option for destination output is unknown: " + options.DestinationOutput.ToString());
      }

      return null;
    }

    /// <summary>
    /// Decomposes a column into repeat units by analysing the values of the column with increasing index.
    /// If a column value is repeated, the current range is finalized and a new range is started. At the end,
    /// a list of index ranges is returned. Inside each range the column values are guaranteed to be unique.
    /// </summary>
    /// <param name="col">Column to decompose.</param>
    /// <returns>List of integer ranges. Inside a single range the column values are ensured to be unique.</returns>
    public static List<AltaxoVariant> Decompose(DataColumn col)
    {
      var result = new List<AltaxoVariant>();
      var alreadyIn = new HashSet<AltaxoVariant>();
      for (int i = 0; i < col.Count; i++)
      {
        var item = col[i];
        if (!alreadyIn.Contains(item))
        {
          result.Add(item);
          alreadyIn.Add(item);
        }
      }
      return result;
    }
  }
}
