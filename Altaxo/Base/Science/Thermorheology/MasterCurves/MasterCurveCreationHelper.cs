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
      var options = new MasterCurveCreationOptions();
      var dataSource = new MasterCurveCreationDataSource(newData, options, new DataSourceImportOptions());

      var newTable = new DataTable();
      newTable.DataSource = dataSource;

      // Show the data source dialog
      var ctrl = new MasterCurveCreationDataSourceController();
      ctrl.InitializeDocument(dataSource);

      if (true == Current.Gui.ShowDialog(ctrl, "Master curve creation"))
      {
        var proposedName = dataTable.Name + "_Mastered";
        newTable.Name = proposedName;
        Current.Project.AddItem(newTable);
        newTable.DataSource = (MasterCurveCreationDataSource)ctrl.ModelObject;
        newTable.UpdateTableFromTableDataSourceAsUserCancellable();
        Current.ProjectService.OpenOrCreateViewContentForDocument(newTable);
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



    public void Execute(MasterCurveData processData, MasterCurveCreationOptions processOptions, DataTable destinationTable)
    {
      // Test the data

      // convert the data to the working data - including the options
      var shiftGroupCollection = ConvertToShiftGroupCollection(processOptions, processData);

      // create the master curve
      var masterCurveResult = MasterCurveCreation.CreateMasterCurve(shiftGroupCollection);

      // fill the table

      int groupNumber = -1;
      var col = destinationTable.DataColumns;
      var pcol = destinationTable.PropCols;

      DataColumn? pcol1 = null, pcol2 = null; ;

      if (!string.IsNullOrEmpty(processOptions.Property1))
      {
        pcol1 = pcol.EnsureExistence(processOptions.Property1, typeof(DoubleColumn), ColumnKind.V, 0);
      }
      if (!string.IsNullOrEmpty(processOptions.Property2))
      {
        pcol2 = pcol.EnsureExistence(processOptions.Property2, typeof(DoubleColumn), ColumnKind.V, 0);
      }

      if (true) // if the table should be filled with the original data
      {
        // in groups
        IReadOnlyList<double> lastX = new double[0];
        for (int j = 0; j < shiftGroupCollection.Count; j++)
        {
          var shiftGroup = shiftGroupCollection[j];
          for (int i = 0; i < shiftGroup.Count; i++)
          {
            var shiftCurve = shiftGroup[i];

            if (lastX.Count != shiftCurve.X.Count || !VectorMath.AreValuesEqual(lastX, shiftCurve.X))
            {
              ++groupNumber;
              var xCol = col.EnsureExistence($"xOrg{(char)('A' + j)}{i}", typeof(DoubleColumn), ColumnKind.X, groupNumber);
              xCol.Data = lastX;
              lastX = shiftCurve.X;
            }
            var yCol = col.EnsureExistence($"yOrg{(char)('A' + j)}{i}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
            yCol.Data = shiftCurve.Y;


          }
        }
      }    // end copying the original data

      if (true) // if the table should be filled with the shifted data
      {
        IReadOnlyList<double> lastX = new double[0];
        groupNumber = (int)(1000 * Math.Ceiling(groupNumber / 1000d) - 1);
        // in groups
        for (int j = 0; j < shiftGroupCollection.Count; j++)
        {
          var shiftGroup = shiftGroupCollection[j];
          for (int i = 0; i < shiftGroup.Count; i++)
          {
            var shiftCurve = shiftGroup[i];

            var xshifted = shiftCurve.X.ToArray();
            VectorMath.Multiply(xshifted, masterCurveResult.ResultingShifts[i]);

            if (lastX.Count != xshifted.Length || !VectorMath.AreValuesEqual(lastX, xshifted))
            {
              ++groupNumber;
              var xCol = col.EnsureExistence($"xSft{(char)('A' + j)}{i}", typeof(DoubleColumn), ColumnKind.X, groupNumber);
              xCol.Data = xshifted;
              lastX = xshifted;
            }
            var yCol = col.EnsureExistence($"ySft{(char)('A' + j)}{i}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
            yCol.Data = shiftCurve.Y;
          }
        }
      }    // end copying the original data

      // copy the shift values
      groupNumber = (int)(1000 * Math.Ceiling(groupNumber / 1000d));

      double reallyUsedRefTemperature = 0;
      double shiftOffsetToExactTemp = 0;

      // get out the interpolation result, Achtung die Werte sind logarithmiert
      for (int i = 0; i < masterCurveResult.ResultingInterpolation.Length; i++)
      {
        var minX = masterCurveResult.ResultingInterpolation[i].InterpolationMinimumX;
        var maxX = masterCurveResult.ResultingInterpolation[i].InterpolationMaximumX;

        var xCol = col.EnsureExistence("ipolX", typeof(DoubleColumn), ColumnKind.X, groupNumber);
        var yCol = col.EnsureExistence("ipolY" + i.ToString(), typeof(DoubleColumn), ColumnKind.V, groupNumber);
        var lgyCol = col.EnsureExistence("lgIpolY" + i.ToString(), typeof(DoubleColumn), ColumnKind.V, groupNumber);
        if (pcol1 is not null)
        {
          pcol1[col.GetColumnNumber(yCol)] = reallyUsedRefTemperature;
          pcol1[col.GetColumnNumber(lgyCol)] = reallyUsedRefTemperature;
        }


        for (int j = 0; j <= 1000; j++)
        {
          double x = minX + (maxX - minX) * j / 1000.0;
          double y = masterCurveResult.ResultingInterpolation[i].InterpolationFunction(x);
          xCol[j] = Math.Exp(x + shiftOffsetToExactTemp);
          yCol[j] = Math.Exp(y);
          lgyCol[j] = Math.Log10(Math.Exp(y));
        }
        ++groupNumber;
      }

      var shiftFactors = col.EnsureExistence("ShiftFactors", typeof(DoubleColumn), ColumnKind.V, groupNumber);
      shiftFactors.Data = masterCurveResult.ResultingShifts;
    }

    /// <summary>
    /// Converts the <see cref="MasterCurveData"/> to <see cref="ShiftGroupCollection"/>.
    /// </summary>
    /// <param name="processData">The process data.</param>
    /// <returns>The <see cref="ShiftGroupCollection"/> containing the data to be shifted.</returns>
    private static ShiftGroupCollection ConvertToShiftGroupCollection(MasterCurveCreationOptions options, MasterCurveData processData)
    {
      var srcData = processData.CurveData;
      var listShiftCollections = new List<ShiftGroup>();

      for (int groupNo = 0; groupNo < srcData.Count; ++groupNo)
      {
        var srcSubArr = srcData[groupNo];
        int choiceIndex = options.MasterCurveGroupOptionsChoice == MasterCurveGroupOptionsChoice.SeparateForEachGroup ? groupNo : 0;

        bool logarithmizeXForInterpolation, logarithmizeYForInterpolation;
        double fitWeight;
        ShiftXBy xShiftBy;
        Func<(IReadOnlyList<double> X, IReadOnlyList<double> Y, IReadOnlyList<double>? YErr), Func<double, double>>? createInterpolationFunction = null;

        if (options.GroupOptions[choiceIndex] is MasterCurveGroupOptionsWithScalarInterpolation scalarOptions)
        {
          fitWeight = scalarOptions.FittingWeight;
          if (!(fitWeight > 0))
            continue;
          logarithmizeXForInterpolation = scalarOptions.LogarithmizeXForInterpolation;
          logarithmizeYForInterpolation = scalarOptions.LogarithmizeYForInterpolation;
          xShiftBy = scalarOptions.XShiftBy;
          createInterpolationFunction = (arg) => scalarOptions.InterpolationFunction.Interpolate(arg.X, arg.Y, arg.YErr).GetYOfX;
        }
        else if (options.GroupOptions[choiceIndex] is MasterCurveGroupOptionsWithComplexInterpolation complexOptions)
        {
          fitWeight = groupNo == 0 ? complexOptions.FittingWeight : complexOptions.FittingWeightIm;
          if (!(fitWeight > 0))
            continue;
          logarithmizeXForInterpolation = complexOptions.LogarithmizeXForInterpolation;
          logarithmizeYForInterpolation = groupNo == 0 ? complexOptions.LogarithmizeYForInterpolation : complexOptions.LogarithmizeYImForInterpolation;
          xShiftBy = complexOptions.XShiftBy;
        }
        else
        {
          throw new NotImplementedException();
        }


        var shiftCurves = new ShiftCurve?[srcSubArr.Length];
        bool any = false;
        for (int i = 0; i < srcSubArr.Length; i++)
        {
          var xycol = srcSubArr[i];
          shiftCurves[i] = xycol is not null ? ConvertToShiftCurve(xycol) : null;
          any |= shiftCurves[i] is not null;
        }

        if (any)
        {
          listShiftCollections.Add(new ShiftGroup(shiftCurves, xShiftBy, fitWeight, logarithmizeXForInterpolation, logarithmizeYForInterpolation, createInterpolationFunction));
        }
        else
        {
          break;
        }
      }

      return new ShiftGroupCollection(listShiftCollections)
      {
        RequiredRelativeOverlap = options.RequiredRelativeOverlap,
        NumberOfIterations = options.NumberOfIterations,
        IndexOfReferenceColumnInColumnGroup = options.IndexOfReferenceColumnInColumnGroup,
        OptimizationMethod = options.OptimizationMethod,
      };
    }

    /// <summary>
    /// Converts <see cref="XAndYColumn"/> data to a <see cref="ShiftCurve"/>.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>The shift curve (if there is any data). If the argument contains to data rows, then the return value is null.</returns>
    private static ShiftCurve? ConvertToShiftCurve(XAndYColumn? data)
    {
      var (x, y, rowCount) = data.GetResolvedXYData();

      if (rowCount == 0)
        return null;
      else
        return new ShiftCurve(x, y);
    }
  }
}
