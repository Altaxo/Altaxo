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

using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Gui.Science.Thermorheology;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  public class MasterCurveCreationHelper
  {
    public static void CreateMasterCurve(DataTable dataTable, IndexSelection selectedDataColumns, IndexSelection selectedPropertyColumns)
    {
      var groupOfColumns = GetColumnsDividedIntoGroups(dataTable, selectedDataColumns);

      var newData = new MasterCurveData();
      newData.SetCurveData(dataTable, groupOfColumns);
      var options = new MasterCurveCreationOptionsEx();
      var dataSource = new MasterCurveCreationDataSource(newData, options, new DataSourceImportOptions());

      var newTable = new DataTable();
      newTable.DataSource = dataSource;

      // Show the data source dialog
      var ctrl = new MasterCurveCreationDataSourceController();
      ctrl.InitializeDocument(dataSource);

      if (true == Current.Gui.ShowDialog(ctrl, "Master curve creation"))
      {
        newTable.DataSource = (MasterCurveCreationDataSource)ctrl.ModelObject;
      }
    }

    public static IReadOnlyList<IReadOnlyList<DataColumn?>> GetColumnsDividedIntoGroups(DataTable dataTable, IndexSelection selectedDataColumns)
    {
      int numberOfNamesWithBaseName = 0;
      int numberOfOtherNames = 0;

      var baseNames = new HashSet<string>();
      foreach (var selIndex in selectedDataColumns)
      {
        var nameOfColumn = dataTable.DataColumns.GetColumnName(selIndex);
        var (isBaseName, baseName) = HasBaseName(nameOfColumn);

        if (isBaseName)
        {
          ++numberOfNamesWithBaseName;
          baseNames.Add(baseName);
        }
        else
        {
          ++numberOfOtherNames;
        }
      }

      var result = new List<List<DataColumn?>>();
      if (numberOfOtherNames == 0 && baseNames.Count * 3 < numberOfNamesWithBaseName)
      {
        // we want to maintain the order in which the column names appear
        var dict = new Dictionary<string, int>(); // dictionary key = baseName, value = index of group
        foreach (var selIndex in selectedDataColumns)
        {
          var nameOfColumn = dataTable.DataColumns.GetColumnName(selIndex);
          var (isBaseName, baseName) = HasBaseName(nameOfColumn);
          if (!dict.TryGetValue(baseName, out var listIndex))
          {
            listIndex = result.Count;
            dict.Add(baseName, listIndex);
            result.Add(new List<DataColumn?>());
          }
          result[listIndex].Add(dataTable.DataColumns[selIndex]);
        }
      }
      else
      {
        result.Add(selectedDataColumns.Select(idx => dataTable.DataColumns[idx]).ToList());
      }

      return result;
    }


    public static (bool hasBaseName, string baseName) HasBaseName(string nameOfColumn)
    {
      for (int k = nameOfColumn.Length - 1; k >= 0; --k)
      {
        var c = nameOfColumn[k];
        if (!(char.IsDigit(c) || c == '.'))
        {
          return (true, nameOfColumn.Substring(0, k + 1));
        }
      }
      return (false, string.Empty);
    }


    public static ShiftCurveCollections GetShiftCurveCollections(List<List<DoubleColumn>> data)
    {
      var multipleShiftDataList = new List<ShiftCurveCollection>();
      foreach (var listOfColumns in data)
      {
        var shiftDataList = new List<ShiftCurve>();
        foreach (var yCol in listOfColumns)
        {
          var table = DataColumnCollection.GetParentDataColumnCollectionOf(yCol) ?? throw new InvalidOperationException($"Column {yCol.Name} has no parent data table!");
          var xCol = (DoubleColumn)(table.FindXColumnOf(yCol) ?? throw new InvalidOperationException($"Can't find corresponding x-column for column {yCol.Name}"));
          var len = Math.Min(xCol.Count, yCol.Count);

          var x = new double[len];
          var y = new double[len];

          for (int i = 0; i < len; i++)
          {
            x[i] = xCol[i];
            y[i] = yCol[i];
          }
          var shiftData = new ShiftCurve(x, y);
          shiftDataList.Add(shiftData);
        }
        var shiftDataCollection = new ShiftCurveCollection(shiftDataList);

        multipleShiftDataList.Add(shiftDataCollection);
      }

      var shiftCurveCollections = new ShiftCurveCollections(multipleShiftDataList);
      return shiftCurveCollections;
    }

    public void Execute(MasterCurveData processData, MasterCurveCreationOptions processOptions, DataTable destinationTable)
    {
      // Test the data

      // convert the data
      var shiftCurveCollections = ConvertToShiftCollections(processData);

      // create the master curve
      var masterCurveResult = MasterCurveCreation.CreateMasterCurve(processOptions, shiftCurveCollections);

      // fill the table

      if (true) // if the table should be filled with the original data
      {


        var col = destinationTable.DataColumns;

        // in groups
        int groupNumber = -1;
        var lastX = new double[0];
        for (int j = 0; j < shiftCurveCollections.Count; j++)
        {
          var shiftCurveCollection = shiftCurveCollections[j];
          for (int i = 0; i < shiftCurveCollection.Count; i++)
          {
            var shiftCurve = shiftCurveCollection[i];

            if (!VectorMath.AreValuesEqual(lastX, shiftCurve.X))
            {
              ++groupNumber;
              var xCol = col.EnsureExistence($"x{'a' + j}{i}", typeof(DoubleColumn), ColumnKind.X, groupNumber);
              for (int k = 0; k < shiftCurve.Count; k++)
              {
                xCol[k] = shiftCurve.X[k];
              }
            }
            var yCol = col.EnsureExistence($"y{'a' + j}{i}", typeof(DoubleColumn), ColumnKind.Y, groupNumber);
            for (int k = 0; k < shiftCurve.Count; k++)
            {
              yCol[k] = shiftCurve.Y[k];
            }
          }
        }
      }    // end copying the original data

      // copy the shift values


    }





    /// <summary>
    /// Converts the <see cref="MasterCurveData"/> to <see cref="ShiftCurveCollections"/>.
    /// </summary>
    /// <param name="processData">The process data.</param>
    /// <returns>The <see cref="ShiftCurveCollections"/> containing the data to be shifted.</returns>
    private ShiftCurveCollections ConvertToShiftCollections(MasterCurveData processData)
    {
      var listShiftCollections = new List<ShiftCurveCollection>();

      // Convert the data from XAndYColumn to ShiftCurve
      for (int j = 0; ; j++)
      {
        var shiftCurves = new ShiftCurve?[processData.CurveData.Count];
        bool any = false;
        for (int i = 0; i < processData.CurveData.Count; i++)
        {
          var curves = processData.CurveData[i];
          any |= (j < curves.Length);
          shiftCurves[i] = (j < curves.Length && curves[j] is { } curve_j) ? ConvertToShiftCurve(curve_j) : null;
        }
        if (any)
        {
          listShiftCollections.Add(new ShiftCurveCollection(shiftCurves));
        }
        else
        {
          break;
        }
      }
      return new ShiftCurveCollections(listShiftCollections);
    }

    /// <summary>
    /// Converts <see cref="XAndYColumn"/> data to a <see cref="ShiftCurve"/>.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>The shift curve (if there is any data). If the argument contains to data rows, then the return value is null.</returns>
    private ShiftCurve? ConvertToShiftCurve(XAndYColumn? data)
    {
      var (x, y, rowCount) = data.GetResolvedXYData();

      if (rowCount == 0)
        return null;
      else
        return new ShiftCurve(x, y);
    }
  }
}
