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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Gui.Worksheet.Viewing;

namespace Altaxo.Data
{

  public static class ConvertXYVToMatrixActions
  {
    /// <summary>
    /// Creates a matrix from three selected columns. This must be a x-column, a y-column, and a value column.
    /// </summary>
    /// <param name="srcTable">The source table containing the columns.</param>
    /// <param name="selectedDataColumns">Indices of the data columns that should be used.</param>
    /// <returns>Null if no error occurs, or an error message.</returns>
    public static string DoMakeActionWithoutDialog(this DataTable srcTable, IAscendingIntegerCollection selectedDataColumns)
    {
      DataColumn xcol = null, ycol = null, vcol = null;

      // for this command to work, there must be exactly 3 data columns selected
      int nCols = selectedDataColumns.Count;
      if (nCols >= 3)
      {
        for (int i = 0; i < nCols; i++)
        {
          if (srcTable.DataColumns.GetColumnKind(selectedDataColumns[i]) == ColumnKind.Y)
          {
            ycol = srcTable.DataColumns[selectedDataColumns[i]];
            break;
          }
        }
        for (int i = 0; i < nCols; i++)
        {
          if (srcTable.DataColumns.GetColumnKind(selectedDataColumns[i]) == ColumnKind.X)
          {
            xcol = srcTable.DataColumns[selectedDataColumns[i]];
            break;
          }
        }

        if (xcol == null || ycol == null)
          return "The selected columns must be a x-column, a y-column, and one or more value columns";
      }
      else
      {
        return "You must select exactly a x-column, a y-column, and one or more value column";
      }

      // use the last column that is a value column as v
      // and use the first column that is an x column as x
      for (int i = 0; i < nCols; i++)
      {
        vcol = srcTable.DataColumns[selectedDataColumns[i]];
        if (object.ReferenceEquals(vcol, xcol) || object.ReferenceEquals(vcol, ycol))
          continue;


        var proxy = new DataTableMultipleColumnProxy(srcTable, srcTable.DataColumns.GetColumnGroup(vcol));
        proxy.EnsureExistenceOfIdentifier(ConvertXYVToMatrixDataAndOptions.ColumnX, 1);
        proxy.EnsureExistenceOfIdentifier(ConvertXYVToMatrixDataAndOptions.ColumnY, 1);
        proxy.EnsureExistenceOfIdentifier(ConvertXYVToMatrixDataAndOptions.ColumnV, 1);

        proxy.AddDataColumn(ConvertXYVToMatrixDataAndOptions.ColumnX, xcol);
        proxy.AddDataColumn(ConvertXYVToMatrixDataAndOptions.ColumnY, ycol);
        proxy.AddDataColumn(ConvertXYVToMatrixDataAndOptions.ColumnV, vcol);

        var newtablename = Current.Project.DataTableCollection.FindNewItemName(srcTable.Name + "-" + vcol.Name);
        var newTable = new DataTable(newtablename);
        Current.Project.DataTableCollection.Add(newTable);
        var options = new ConvertXYVToMatrixOptions();
        ConvertXYVToMatrix(proxy, options, newTable);

        var dataSource = new ConvertXYVToMatrixDataSource(proxy, options, new Altaxo.Data.DataSourceImportOptions());
        newTable.DataSource = dataSource;

        Current.IProjectService.ShowDocumentView(newTable);
      }

      return null;
    }

    public static void ShowActionDialog(this DataTable srcTable, IAscendingIntegerCollection selectedDataColumns)
    {
      DataTableMultipleColumnProxy proxy = null;
      ConvertXYVToMatrixOptions options = null;

      try
      {
        proxy = new DataTableMultipleColumnProxy(ConvertXYVToMatrixDataAndOptions.ColumnV, srcTable, null, selectedDataColumns);
        proxy.EnsureExistenceOfIdentifier(ConvertXYVToMatrixDataAndOptions.ColumnX, 1);
        proxy.EnsureExistenceOfIdentifier(ConvertXYVToMatrixDataAndOptions.ColumnY, 1);

        options = new ConvertXYVToMatrixOptions();
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(string.Format("{0}\r\nDetails:\r\n{1}", ex.Message, ex.ToString()), "Error in preparation of 'Convert X-Y-V values to matrix'");
        return;
      }

      var dataAndOptions = new ConvertXYVToMatrixDataAndOptions(proxy, options);

      // in order to show the column names etc in the dialog, it is neccessary to set the source
      if (true == Current.Gui.ShowDialog(ref dataAndOptions, "Choose options", false))
      {
        var destTable = new DataTable();
        proxy = dataAndOptions.Data;
        options = dataAndOptions.Options;

        string error = null;
        try
        {
          error = ConvertXYVToMatrix(dataAndOptions.Data, dataAndOptions.Options, destTable);
        }
        catch (Exception ex)
        {
          error = ex.ToString();
        }
        if (null != error)
          Current.Gui.ErrorMessageBox(error);

        destTable.Name = srcTable.Name + "_Decomposed";

        // Create a DataSource
        var dataSource = new ConvertXYVToMatrixDataSource(proxy, options, new Altaxo.Data.DataSourceImportOptions());
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
    public static string ConvertXYVToMatrix(DataTableMultipleColumnProxy inputData, ConvertXYVToMatrixOptions options, DataTable destTable)
    {
      var srcTable = inputData.DataTable;

      try
      {
        ConvertXYVToMatrixDataAndOptions.EnsureCoherence(inputData, true);
      }
      catch (Exception ex)
      {
        return ex.Message;
      }

      destTable.DataColumns.RemoveColumnsAll();
      destTable.PropCols.RemoveColumnsAll();

      DataColumn srcXCol = inputData.GetDataColumnOrNull(ConvertXYVToMatrixDataAndOptions.ColumnX);
      DataColumn srcYCol = inputData.GetDataColumnOrNull(ConvertXYVToMatrixDataAndOptions.ColumnY);

      // X-Values
      IReadOnlyList<AltaxoVariant> clusterValuesX;
      IReadOnlyList<int> clusterIndicesX;
      IReadOnlyList<double> clusterStdDevX = null;
      if (options.UseClusteringForX && options.NumberOfClustersX.HasValue && srcXCol is DoubleColumn srcXDbl)
        (clusterValuesX, clusterStdDevX, clusterIndicesX) = ClusterValuesByKMeans(srcXDbl, options.NumberOfClustersX.Value, options.DestinationXColumnSorting, options.CreateStdDevX);
      else
        (clusterValuesX, clusterIndicesX) = ClusterValuesByEquality(srcXCol, options.DestinationXColumnSorting);

      // Y-Values
      IReadOnlyList<AltaxoVariant> clusterValuesY;
      IReadOnlyList<int> clusterIndicesY;
      IReadOnlyList<double> clusterStdDevY = null;
      if (options.UseClusteringForY && options.NumberOfClustersY.HasValue && srcYCol is DoubleColumn srcYDbl)
        (clusterValuesY, clusterStdDevY, clusterIndicesY) = ClusterValuesByKMeans(srcYDbl, options.NumberOfClustersY.Value, options.DestinationYColumnSorting, options.CreateStdDevY);
      else
        (clusterValuesY, clusterIndicesY) = ClusterValuesByEquality(srcYCol, options.DestinationYColumnSorting);


      // get the other columns to process
      var srcColumnsToProcess = new List<DataColumn>(inputData.GetDataColumns(ConvertXYVToMatrixDataAndOptions.ColumnV));
      // subtract the column containing the decompose values
      srcColumnsToProcess.Remove(srcXCol);
      srcColumnsToProcess.Remove(srcYCol);

      int xOffset = 1 + (clusterStdDevY != null ? 1 : 0);
      // the only property column that is now useful is that with the repeated values
      var destXCol = destTable.PropCols.EnsureExistence(srcTable.DataColumns.GetColumnName(srcXCol), srcXCol.GetType(), ColumnKind.X, 0);
      for (int i = 0; i < xOffset; ++i)
        destXCol[0] = double.NaN;
      for (int i = 0; i < clusterValuesX.Count; ++i)
        destXCol[i + xOffset] = clusterValuesX[i]; // leave index 0 and maybe 1for the y-column

      if (clusterStdDevX != null)
      {
        var stdXCol = destTable.PropCols.EnsureExistence(srcTable.DataColumns.GetColumnName(srcXCol) + "_StdDev", srcXCol.GetType(), ColumnKind.Err, 0);
        for (int i = 0; i < xOffset; ++i)
          stdXCol[0] = double.NaN;
        for (int i = 0; i < clusterStdDevX.Count; ++i)
          stdXCol[i + xOffset] = clusterStdDevX[i]; // leave index 0 and maybe 1 for the y-column
      }

      var destYCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcYCol), srcYCol.GetType(), ColumnKind.X, 0);
      for (int i = 0; i < clusterValuesY.Count; ++i)
        destYCol[i] = clusterValuesY[i]; // leave index 0 for the y-column

      if (clusterStdDevY != null)
      {
        var stdYCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcYCol) + "_StdDev", srcYCol.GetType(), ColumnKind.Err, 0);
        for (int i = 0; i < clusterStdDevY.Count; ++i)
          stdYCol[i] = clusterStdDevY[i]; // leave index 0 for the y-column
      }

      var srcVColumn = srcColumnsToProcess[0];

      // Create as many columns as there are values in the destXColumn

      for (int i = 0; i < clusterValuesX.Count; ++i)
      {
        if (options.ColumnNaming == ConvertXYVToMatrixOptions.OutputNaming.ColAndIndex || string.IsNullOrEmpty(options.ColumnNameFormatString))
        {
          destTable.DataColumns.EnsureExistence("Col" + i.ToString(), srcVColumn.GetType(), ColumnKind.V, 0);
        }
        else
        {
          destTable.DataColumns.EnsureExistence(string.Format(options.ColumnNameFormatString, clusterValuesX[i], i), srcVColumn.GetType(), ColumnKind.V, 0);
        }
      }

      var dict = new Dictionary<(int iX, int iY), (AltaxoVariant sum, int count)>();
      for (int i = 0; i < srcVColumn.Count; ++i)
      {
        var iX = xOffset + clusterIndicesX[i];
        var iY = clusterIndicesY[i];

        if (destTable[iX].IsElementEmpty(iY))
        {
          destTable[iX][iY] = srcVColumn[i];
        }
        else
        {
          switch (options.ValueAveraging)
          {
            case ConvertXYVToMatrixOptions.OutputAveraging.NoneIgnoreUseLastValue:
              destTable[iX][iY] = srcVColumn[i];
              break;
            case ConvertXYVToMatrixOptions.OutputAveraging.NoneThrowException:
              throw new Exception(string.Format("Multiple data present for X={0}, Y={1}, and average options has been set to throw an exception in this case!", srcXCol[i], srcYCol[i]));
            case ConvertXYVToMatrixOptions.OutputAveraging.AverageLinear:
              {
                if (!dict.ContainsKey((iX, iY)))
                  dict.Add((iX, iY), (srcVColumn[i], 1)); // if not found in the dictionary, then add the first value that was already in the table

                var (sum, count) = dict[(iX, iY)];
                // and now add the current value
                sum += srcVColumn[i];
                count += 1;
                dict[(iX, iY)] = (sum, count);
                destTable[iX][iY] = sum / count;
              }
              break;
            default:
              break;
          }
        }
      }



      return null;
    }

    public static (IReadOnlyList<AltaxoVariant> ClusterValues, IReadOnlyList<double> ClusterStdDev, IReadOnlyList<int> ClusterIndices) ClusterValuesByKMeans(DoubleColumn col, int numberOfClusters, SortDirection sortDirection, bool createStdDev)
    {
      var clustering = new Altaxo.Calc.Clustering.KMeans_Double1D() { SortingOfClusterValues = sortDirection };
      clustering.Evaluate(col.ToROVector(), numberOfClusters);
      var resultList = new List<AltaxoVariant>(clustering.ClusterMeans.Select(x => new AltaxoVariant(x)));
      var resultIndices = clustering.ClusterIndices;

      IReadOnlyList<double> resultStdDev = null;
      if (createStdDev)
      {
        resultStdDev = clustering.EvaluateClustersStandardDeviation();
      }
      return (resultList, resultStdDev, resultIndices);
    }

    public static (IReadOnlyList<AltaxoVariant> ClusterValues, IReadOnlyList<int> ClusterIndices) ClusterValuesByEquality(DataColumn col, SortDirection sortDirection)
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

      switch (sortDirection)
      {
        case SortDirection.Ascending:
          result.Sort();
          break;
        case SortDirection.Descending:
          result.Sort((x, y) => Comparer<AltaxoVariant>.Default.Compare(y, x));
          break;
        default:
          break;
      }

      int[] clusterIndices = new int[col.Count];

      for (int i = 0; i < clusterIndices[i]; ++i)
      {
        clusterIndices[i] = result.IndexOf(col[i]);
      }

      return (result, clusterIndices);
    }
  }
}
