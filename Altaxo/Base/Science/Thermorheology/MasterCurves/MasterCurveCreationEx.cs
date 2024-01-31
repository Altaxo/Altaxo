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
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Gui.Science.Thermorheology;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// Helps creating the master curve.
  /// </summary>
  public class MasterCurveCreationEx : MasterCurveCreation
  {
    #region Helper classes

    /// <summary>
    /// Information about a single curve.
    /// </summary>
    private class CurveInformation
    {
      /// <summary>
      /// The corresponding index in the shift group collection. If null, this curve does not participate in fitting.
      /// </summary>
      public int? IndexInShiftGroupCollection;

      /// <summary>
      /// Value of property1.
      /// </summary>
      public AltaxoVariant Property1Value;

      /// <summary>
      /// Value of property2.
      /// </summary>
      public AltaxoVariant Property2Value;
    }

    /// <summary>
    /// Accomodates helper information. 
    /// </summary>
    private class FitInformation
    {
      /// <summary>
      /// Gets the correspondence between the curves in the <see cref="MasterCurveData"/> high level class and the curves in the <see cref="ShiftGroupCollection"/> low level class.
      /// </summary>
      public IReadOnlyList<CurveInformation> CurveInformation { get; }

      /// <summary>
      /// Relates the groups in the <see cref="MasterCurveData"/> high level class to the groups in the (<see cref="ShiftGroupCollection"/>) low level class.
      /// If the element has a value, the value designates the corresponding group index in the <see cref="ShiftGroupCollection"/>.
      /// If the element value is null, then this group was not participating in the fit, and thus is not contained in the <see cref="ShiftGroupCollection"/>.
      /// </summary>
      public IReadOnlyList<int?> IndexOfGroupInShiftGroupCollection { get; }

      public FitInformation(CurveInformation[] curveInfo, IReadOnlyList<int?> groupInformation)
      {
        CurveInformation = curveInfo;
        IndexOfGroupInShiftGroupCollection = groupInformation;
      }
    }

    #endregion

    /// <summary>
    /// Creates a master curve, initially showing a dialog.
    /// </summary>
    /// <param name="dataTable">The data table which contains at least some of the curves to master.</param>
    /// <param name="selectedDataColumns">The selected data columns (only columns with the dependent variables should be selected).</param>
    /// <param name="selectedPropertyColumns">Optionally, up to two selected property columns that will be used for the metadata.</param>
    public static void CreateMasterCurveShowDialog(DataTable dataTable, IndexSelection selectedDataColumns, IndexSelection selectedPropertyColumns)
    {
      var groupOfColumns = GetColumnsDividedIntoGroups(dataTable, selectedDataColumns);

      var newData = new MasterCurveData();
      newData.SetCurveData(dataTable, groupOfColumns);

      string prop1Name = string.Empty, prop2Name = string.Empty;
      if (selectedPropertyColumns.Count > 0) prop1Name = dataTable.PropCols.GetColumnName(selectedPropertyColumns[0]);
      if (selectedPropertyColumns.Count > 1) prop2Name = dataTable.PropCols.GetColumnName(selectedPropertyColumns[1]);
      var options = new MasterCurveCreationOptions() { Property1Name = prop1Name, Property2Name = prop2Name };
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


    /// <summary>
    /// Determines whether a column name can be divided into a base name, and a sequence of digits.
    /// </summary>
    /// <param name="nameOfColumn">The name of column.</param>
    /// <returns></returns>
    /// <example>
    /// For example,
    /// the name 'Yabs05423' has a base name 'Yabs'. The name 'Yabs.05423' has the same base name 'Yabs' (dots will be ignored).
    /// The name '05.234' has no base name (it only contains dot and digits).
    /// </example>
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



    /// <summary>
    /// Executes the master curve creation, and writes the results in the table <paramref name="destinationTable"/>.
    /// </summary>
    /// <param name="processData">Contains the curves that participate in the master curve creation.</param>
    /// <param name="processOptions">The options for master curve creation.</param>
    /// <param name="destinationTable">The destination table that accomodates the results.</param>
    /// <exception cref="System.NotImplementedException"></exception>
    public static void Execute(MasterCurveData processData, MasterCurveCreationOptions processOptions, DataTable destinationTable)
    {
      // Test the data

      // at first, we create information about the curves
      int numberOfCurves = processData.CurveData.Max(shiftGroup => shiftGroup.Length);

      // convert the data to the working data - including the options
      var (shiftGroupCollection, fitInfo) = ConvertToShiftGroupCollection(processOptions, processData);

      // create the master curve
      var masterCurveResult = CreateMasterCurve(shiftGroupCollection);

      if (processOptions.MasterCurveImprovementOptions is { } improvementOptions)
      {
        // if improvement options are set, then we set the options in the low level interface anew,
        // and then we re-iterate, but using the already evaluated shift factors

        // create new shiftGroupCollection with the same data, but with the options contained in improvementOptions
        // the data a copied by reference, but this is OK, since we drop the old shiftGroupCollection
        shiftGroupCollection = new ShiftGroupCollection(shiftGroupCollection.Select((group, idx) =>
        {
          var newGroupOptions = (MasterCurveGroupOptionsWithScalarInterpolation)GetGroupOptions(improvementOptions, idx);
          var newShiftGroup = new ShiftGroup(group, newGroupOptions.XShiftBy, newGroupOptions.FittingWeight, newGroupOptions.LogarithmizeXForInterpolation, newGroupOptions.LogarithmizeYForInterpolation, (arg) => newGroupOptions.InterpolationFunction.Interpolate(arg.X, arg.Y, arg.YErr).GetYOfX);
          return newShiftGroup;
        }))
        {
          ShiftOrder = improvementOptions.ShiftOrder,
          NumberOfIterations = improvementOptions.NumberOfIterations,
          OptimizationMethod = improvementOptions.OptimizationMethod,
        };

        ReIterate(shiftGroupCollection, masterCurveResult);
      }

      // fill the table

      int groupNumber = -1;
      var col = destinationTable.DataColumns;
      var pcol = destinationTable.PropCols;
      col.RemoveColumnsAll();
      pcol.RemoveColumnsAll();

      DataColumn? pcol1 = null, pcol2 = null; ;

      if (!string.IsNullOrEmpty(processOptions.Property1Name))
      {
        pcol1 = pcol.EnsureExistence(processOptions.Property1Name, typeof(DoubleColumn), ColumnKind.V, 0);
      }
      if (!string.IsNullOrEmpty(processOptions.Property2Name))
      {
        pcol2 = pcol.EnsureExistence(processOptions.Property2Name, typeof(DoubleColumn), ColumnKind.V, 0);
      }
      var pcolShift = pcol.EnsureExistence("Shift", typeof(DoubleColumn), ColumnKind.V, 0);

      if (processOptions.TableOutputOptions.OutputOriginalCurves) // if the table should be filled with the original data
      {
        // in groups
        IReadOnlyList<double> lastX = new double[0];
        for (int idxGroup = 0; idxGroup < processData.CurveData.Count; idxGroup++)
        {
          var group = processData.CurveData[idxGroup];
          for (int idxCurve = 0; idxCurve < group.Length; idxCurve++)
          {
            var shiftCurve = ConvertToShiftCurve(group[idxCurve]);

            if (lastX.Count != shiftCurve.X.Count || !VectorMath.AreValuesEqual(lastX, shiftCurve.X))
            {
              ++groupNumber;
              var xCol = col.EnsureExistence($"xOrg{(char)('A' + idxGroup)}{idxCurve}", typeof(DoubleColumn), ColumnKind.X, groupNumber);
              xCol.Data = lastX = shiftCurve.X;
            }
            var yCol = col.EnsureExistence($"yOrg{(char)('A' + idxGroup)}{idxCurve}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
            yCol.Data = shiftCurve.Y;

            if (pcol1 is not null)
            {
              pcol1[col.GetColumnNumber(yCol)] = fitInfo.CurveInformation[idxCurve].Property1Value;
            }
            if (pcol2 is not null)
            {
              pcol2[col.GetColumnNumber(yCol)] = fitInfo.CurveInformation[idxCurve].Property2Value;
            }
          }
        }
      }    // end copying the original data

      // for finalization, we need the shift offset
      var (shiftOffset, referenceValueUsed) = GetShiftOffset(masterCurveResult, fitInfo, processOptions);

      if (processOptions.TableOutputOptions.OutputShiftedCurves) // if the table should be filled with the shifted data
      {
        // we want to put the x-columns all together in one place, that's why we accumulate the columns and
        // put them afterwards to the table
        var xColumnList = new List<(int group, string name, DataColumn column)>();
        var yColumnList = new List<(int group, string name, DataColumn column, AltaxoVariant p1, AltaxoVariant p2, double shift)>();

        IReadOnlyList<double> lastX = new double[0];
        groupNumber = (int)(1000 * Math.Ceiling((groupNumber + 1) / 1000d) - 1);
        // in groups
        for (int idxGroup = 0; idxGroup < processData.CurveData.Count; idxGroup++)
        {
          var group = processData.CurveData[idxGroup];
          var groupOptions = GetGroupOptionsOfImprovementOrMain(processOptions, idxGroup);
          for (int idxCurve = 0; idxCurve < group.Length; idxCurve++)
          {
            var idxCurveInShiftGroupCollection = fitInfo.CurveInformation[idxCurve].IndexInShiftGroupCollection;
            if (idxCurveInShiftGroupCollection is null)
              continue; // if this particular curve has no shift information, we skip it

            var shiftValue = masterCurveResult.ResultingShifts[idxCurveInShiftGroupCollection.Value];
            // subtract the shiftOffset
            shiftValue -= shiftOffset;

            var shiftCurve = ConvertToShiftCurve(group[idxCurve]);
            var xshifted = shiftCurve.X.ToArray();
            if (groupOptions.XShiftBy == ShiftXBy.Factor)
              VectorMath.Multiply(xshifted, Math.Exp(shiftValue));
            else if (groupOptions.XShiftBy == ShiftXBy.Offset)
              VectorMath.Add(xshifted, shiftValue);
            else
              throw new NotImplementedException();

            if (lastX.Count != xshifted.Length || !VectorMath.AreValuesEqual(lastX, xshifted))
            {
              ++groupNumber;
              var xCol = new DoubleColumn();
              xCol.Data = xshifted;
              lastX = xshifted;
              xColumnList.Add((groupNumber, $"xSft{(char)('A' + idxGroup)}{idxCurve}", xCol));
            }
            var yCol = new DoubleColumn();
            yCol.Data = shiftCurve.Y;
            yColumnList.Add((groupNumber, $"ySft{(char)('A' + idxGroup)}{idxCurve}", yCol, fitInfo.CurveInformation[idxCurve].Property1Value, fitInfo.CurveInformation[idxCurve].Property2Value, shiftValue));
          }
        }

        // now put the x-columns to the table
        foreach (var xcol in xColumnList)
        {
          col.Add(xcol.column, xcol.name, ColumnKind.X, xcol.group);
        }
        // and now the y-columns
        foreach (var ycol in yColumnList)
        {
          col.Add(ycol.column, ycol.name, ColumnKind.V, ycol.group);
          pcolShift[col.GetColumnNumber(col[ycol.name])] = ycol.shift;

          if (pcol1 is not null)
          {
            pcol1[col.GetColumnNumber(col[ycol.name])] = ycol.p1;
          }
          if (pcol2 is not null)
          {
            pcol2[col.GetColumnNumber(col[ycol.name])] = ycol.p2;
          }
        }
      }    // end copying the original data

      if (processOptions.TableOutputOptions.OutputMergedShiftedCurve) // if the table should be filled with the merged data
      {
        groupNumber = (int)(1000 * Math.Ceiling((groupNumber + 1) / 1000d));
        // in groups
        for (int idxGroup = 0; idxGroup < processData.CurveData.Count; idxGroup++)
        {
          var group = processData.CurveData[idxGroup];
          var groupOptions = GetGroupOptionsOfImprovementOrMain(processOptions, idxGroup);
          var resultCurve = GetMergedCurveData(group, groupOptions, masterCurveResult, fitInfo, shiftOffset);

          var xcol = col.EnsureExistence($"xMerged{(char)('A' + idxGroup)}", typeof(DoubleColumn), ColumnKind.X, groupNumber);
          var ycol = col.EnsureExistence($"yMerged{(char)('A' + idxGroup)}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
          var zcol = col.EnsureExistence($"indexMerged{(char)('A' + idxGroup)}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
          ++groupNumber;

          for (int i = 0; i < resultCurve.Count; ++i)
          {
            xcol[i] = resultCurve[i].x;
            ycol[i] = resultCurve[i].y;
            zcol[i] = resultCurve[i].idxCurve;
          }
        } // for each group
      } // end put merged curves


      if (processOptions.TableOutputOptions.OutputInterpolatedCurve)
      {
        groupNumber = (int)(100 * Math.Ceiling((groupNumber + 1) / 100d));

        // get out the interpolation result, Achtung die Werte sind logarithmiert
        for (int idxGroup = 0; idxGroup < processData.CurveData.Count; idxGroup++)
        {
          var group = processData.CurveData[idxGroup];
          var groupOptions = GetGroupOptionsOfImprovementOrMain(processOptions, idxGroup);
          var resultCurve = GetMergedCurveData(group, groupOptions, masterCurveResult, fitInfo, shiftOffset);

          var xtrans = new List<double>();
          var ytrans = new List<double>();
          for (int i = 0; i < resultCurve.Count; ++i)
          {
            double x = resultCurve[i].x;
            var y = resultCurve[i].y;

            if (groupOptions.LogarithmizeXForInterpolation)
              x = Math.Log(x);
            if (groupOptions.LogarithmizeYForInterpolation)
              y = Math.Log(y);

            if (x.IsFinite() && y.IsFinite())
            {
              xtrans.Add(x);
              ytrans.Add(y);
            }
          }

          var interpolation = (groupOptions as MasterCurveGroupOptionsWithScalarInterpolation).InterpolationFunction;

          if (interpolation is not null)
          {
            var interpolationResult = interpolation.Interpolate(xtrans, ytrans);
            var xcol = col.EnsureExistence($"xInterpolated{(char)('A' + idxGroup)}", typeof(DoubleColumn), ColumnKind.X, groupNumber);
            var ycol = col.EnsureExistence($"yInterpolated{(char)('A' + idxGroup)}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
            ++groupNumber;

            var minX = xtrans[0];
            var maxX = xtrans[^1];
            int numberOfInterpolationPoints = 1001;
            for (int i = 0; i < numberOfInterpolationPoints; ++i)
            {
              var r = i / (numberOfInterpolationPoints + 1d);
              var x = minX * (1 - r) + maxX * r;
              var y = interpolationResult.GetYOfX(x);

              if (groupOptions.LogarithmizeXForInterpolation)
                x = Math.Exp(x);
              if (groupOptions.LogarithmizeYForInterpolation)
                y = Math.Exp(y);

              xcol[i] = x;
              ycol[i] = y;
            }
          }
        }
      }

      {
        // Output the columns with the values of Property1 (most probable the temperature) ....
        groupNumber = (int)(100 * Math.Ceiling((groupNumber + 1) / 100d));
        if (!string.IsNullOrEmpty(processOptions.Property1Name))
        {
          var prop1Col = (DoubleColumn)col.EnsureExistence(processOptions.Property1Name, typeof(DoubleColumn), ColumnKind.X, groupNumber);

          // ... and if Property1 represents indeed the temperature, then also create a column with the inverse temperature...
          DoubleColumn prop1ColInv = null;
          if (processOptions.Property1TemperatureRepresentation.HasValue)
          {
            prop1ColInv = (DoubleColumn)col.EnsureExistence(processOptions.Property1Name + "_Inverse", typeof(DoubleColumn), ColumnKind.X, groupNumber);
          }

          for (int idxCurve = 0; idxCurve < numberOfCurves; ++idxCurve)
          {
            var idxCurveInShiftGroupCollection = fitInfo.CurveInformation[idxCurve].IndexInShiftGroupCollection;
            if (idxCurveInShiftGroupCollection.HasValue)
            {
              prop1Col[idxCurveInShiftGroupCollection.Value] = fitInfo.CurveInformation[idxCurve].Property1Value;

              if (prop1ColInv is not null)
              {
                var t = new Altaxo.Science.Temperature(fitInfo.CurveInformation[idxCurve].Property1Value, processOptions.Property1TemperatureRepresentation.Value);
                prop1ColInv[idxCurveInShiftGroupCollection.Value] = t.InInverseKelvin;
              }
            }
          }
        }

        // ... and a column with the property2 values...
        if (!string.IsNullOrEmpty(processOptions.Property2Name))
        {
          var prop2Col = (DoubleColumn)col.EnsureExistence(processOptions.Property2Name, typeof(DoubleColumn), ColumnKind.V, groupNumber);
          for (int idxCurve = 0; idxCurve < numberOfCurves; ++idxCurve)
          {
            var idxCurveInShiftGroupCollection = fitInfo.CurveInformation[idxCurve].IndexInShiftGroupCollection;
            if (idxCurveInShiftGroupCollection.HasValue)
            {
              prop2Col[idxCurveInShiftGroupCollection.Value] = fitInfo.CurveInformation[idxCurve].Property2Value;
            }
          }
        }

        // now the shift values
        var shiftCol = (DoubleColumn)col.EnsureExistence("Shift", typeof(DoubleColumn), ColumnKind.V, groupNumber);
        shiftCol.Data = masterCurveResult.ResultingShifts;
        shiftCol.Data = shiftCol - shiftOffset; // subtract the shiftOffset


        // Output the factor only if any for the group uses shift by factor
        if (processOptions.GroupOptions.Any(g => g.XShiftBy == ShiftXBy.Factor))
        {
          var shiftFactorCol = (DoubleColumn)col.EnsureExistence("ShiftFactor", typeof(DoubleColumn), ColumnKind.V, groupNumber);
          shiftFactorCol.Data = shiftCol.Array.Select(x => Math.Exp(x)).ToArray();
        }
      }


      // Finally, we put the actually used Reference value to a table property "MasterCurveReferenceValue"
      if (!referenceValueUsed.IsEmpty)
      {
        destinationTable.SetTableProperty("MasterCurveReferenceValue", referenceValueUsed);
      }
    }

    private static IReadOnlyList<(double x, double y, int idxCurve)> GetMergedCurveData(XAndYColumn?[] originalCurves, MasterCurveGroupOptions groupOptions, MasterCurveCreationResult masterCurveResult, FitInformation fitInfo, double shiftOffset)
    {
      var resultCurve = new List<(double x, double y, int idxCurve)>();
      for (int idxCurve = 0; idxCurve < originalCurves.Length; idxCurve++)
      {
        var idxCurveInShiftGroupCollection = fitInfo.CurveInformation[idxCurve].IndexInShiftGroupCollection;
        if (idxCurveInShiftGroupCollection is null)
          continue; // if this particular curve has no shift information, we skip it

        var shiftValue = masterCurveResult.ResultingShifts[idxCurveInShiftGroupCollection.Value] - shiftOffset;
        var shiftCurve = ConvertToShiftCurve(originalCurves[idxCurve]);
        if (shiftCurve is not null)
        {
          for (int i = 0; i < shiftCurve.Count; ++i)
          {
            var x = shiftCurve.X[i];
            if (groupOptions.XShiftBy == ShiftXBy.Factor)
              x *= Math.Exp(shiftValue);
            else if (groupOptions.XShiftBy == ShiftXBy.Offset)
              x += shiftValue;
            else
              throw new NotImplementedException();
            resultCurve.Add((x, shiftCurve.Y[i], idxCurve));
          }
        }
      }
      resultCurve.Sort((a, b) => Comparer<double>.Default.Compare(a.x, b.x));

      return resultCurve;
    }

    /// <summary>
    /// Gets the shift offset. After creation of the master curve using the low level interface, the entire curve can be shifted, so that the value shift=0 is at another point.
    /// The point is determined by the options.
    /// </summary>
    /// <param name="masterCurveResult">The results of master curve creation (contains the actual shift values).</param>
    /// <param name="highToLowLevelMapping">The mapping of the high level data (see <see cref="MasterCurveData"/>), to the low level data (see <see cref="ShiftGroupCollection"/>.</param>
    /// <param name="processOptions">The master curve creation options.</param>
    /// <returns>A tuple containing the shift offset, with which the entire curve should be shifted, and the actually used reference value (e.g. reference temperature).</returns>
    private static (double shiftOffset, AltaxoVariant referenceValue) GetShiftOffset(MasterCurveCreationResult masterCurveResult, FitInformation highToLowLevelMapping, MasterCurveCreationOptions processOptions)
    {
      var useExactReferenceValue = processOptions.UseExactReferenceValue;

StartOfFunction:

      double shiftOffset = 0;
      AltaxoVariant referenceValueUsed = new AltaxoVariant();
      if (processOptions.ReferenceValue.HasValue)
      {
        if (useExactReferenceValue)
        {
          // we have to make an interpolation of the shift values (y) versus the property1 values (x), and then
          // try to get the shift value at the reference property
          var listX = new List<double>();
          var listY = new List<double>();
          for (int idxCurve = 0; idxCurve < highToLowLevelMapping.CurveInformation.Count; ++idxCurve)
          {
            var idxCurveInShiftGroupCollection = highToLowLevelMapping.CurveInformation[idxCurve].IndexInShiftGroupCollection;
            if (idxCurveInShiftGroupCollection.HasValue && !highToLowLevelMapping.CurveInformation[idxCurve].Property1Value.IsEmpty)
            {
              double x = highToLowLevelMapping.CurveInformation[idxCurve].Property1Value;
              double y = masterCurveResult.ResultingShifts[idxCurveInShiftGroupCollection.Value];

              if (!double.IsNaN(x) && !double.IsNaN(y))
              {
                listX.Add(x);
                listY.Add(y);
              }
            }
          }

          if (listX.Count < 2)
          {
            useExactReferenceValue = false;
            goto StartOfFunction;
          }

          Altaxo.Calc.Interpolation.IInterpolationFunctionOptions interpolation;

          if (listX.Count <= 2)
            interpolation = new Altaxo.Calc.Interpolation.PolynomialRegressionAsInterpolationOptions(order: listX.Count - 1);
          else
            interpolation = new Altaxo.Calc.Interpolation.CrossValidatedCubicSplineOptions();

          var interpolationFunc = interpolation.Interpolate(listX, listY);
          shiftOffset = interpolationFunc.GetYOfX(processOptions.ReferenceValue.Value);
          referenceValueUsed = processOptions.ReferenceValue.Value;
        }
        else
        {

          // we search for the nearest index that has shift information available
          var referenceValue = processOptions.ReferenceValue.Value;
          double minDistance = double.PositiveInfinity;
          shiftOffset = 0;
          for (int idxCurve = 0; idxCurve < highToLowLevelMapping.CurveInformation.Count; ++idxCurve)
          {
            var idxCurveInShiftGroupCollection = highToLowLevelMapping.CurveInformation[idxCurve].IndexInShiftGroupCollection;
            if (idxCurveInShiftGroupCollection.HasValue && !highToLowLevelMapping.CurveInformation[idxCurve].Property1Value.IsEmpty)
            {
              var distance = Math.Abs(highToLowLevelMapping.CurveInformation[idxCurve].Property1Value - referenceValue);
              if (distance < minDistance)
              {
                minDistance = distance;
                shiftOffset = masterCurveResult.ResultingShifts[idxCurveInShiftGroupCollection.Value];
                referenceValueUsed = highToLowLevelMapping.CurveInformation[idxCurve].Property1Value;
              }
            }
          }
        }
      }
      else // we use the reference index
      {
        var refIndex = processOptions.IndexOfReferenceColumnInColumnGroup;
        // we search for the nearest index that has shift information available
        double minDistance = double.PositiveInfinity;
        shiftOffset = 0;
        for (int idxCurve = 0; idxCurve < highToLowLevelMapping.CurveInformation.Count; ++idxCurve)
        {
          var idxCurveInShiftGroupCollection = highToLowLevelMapping.CurveInformation[idxCurve].IndexInShiftGroupCollection;

          if (idxCurveInShiftGroupCollection.HasValue)
          {
            var distance = Math.Abs(idxCurve - refIndex);
            if (distance < minDistance)
            {
              minDistance = distance;
              shiftOffset = masterCurveResult.ResultingShifts[idxCurveInShiftGroupCollection.Value];
              referenceValueUsed = highToLowLevelMapping.CurveInformation[idxCurve].Property1Value;
            }
          }
        }

      }
      return (shiftOffset, referenceValueUsed);
    }

    static MasterCurveGroupOptions GetGroupOptions(MasterCurveCreationOptions options, int idxGroup)
    {
      int choiceIndex = options.MasterCurveGroupOptionsChoice == MasterCurveGroupOptionsChoice.SeparateForEachGroup ? idxGroup : 0;
      var groupOptions = options.GroupOptions[choiceIndex];
      return groupOptions;
    }
    static MasterCurveGroupOptions GetGroupOptions(MasterCurveImprovementOptions options, int idxGroup)
    {
      int choiceIndex = options.MasterCurveGroupOptionsChoice == MasterCurveGroupOptionsChoice.SeparateForEachGroup ? idxGroup : 0;
      var groupOptions = options.GroupOptions[choiceIndex];
      return groupOptions;
    }

    /// <summary>
    /// Gets the group options. If improvement options are available, the group options are used from there.
    /// Else, if the improvement options are null, then the group options are used from the main options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="idxGroup">The index group.</param>
    /// <returns></returns>
    static MasterCurveGroupOptions GetGroupOptionsOfImprovementOrMain(MasterCurveCreationOptions options, int idxGroup)
    {
      if (options.MasterCurveImprovementOptions is { } improvementOptions)
      {
        return GetGroupOptions(improvementOptions, idxGroup);
      }
      else
      {
        return GetGroupOptions(options, idxGroup);
      }
    }


    /// <summary>
    /// Converts the <see cref="MasterCurveData"/> to <see cref="ShiftGroupCollection"/>.
    /// Only those curves are converted which participate on the fit, i.e. the curve must have at least two points,
    /// and the fitting weight must be positive.
    /// </summary>
    /// <param name="processData">The process data.</param>
    /// <returns>The <see cref="ShiftGroupCollection"/> containing the data to be shifted.</returns>
    private static (ShiftGroupCollection shiftGroupCollection, FitInformation fitInformation) ConvertToShiftGroupCollection(MasterCurveCreationOptions options, MasterCurveData processData)
    {
      var srcData = processData.CurveData;

      var numberOfGroups = srcData.Count;
      var numberOfCurves = srcData.Max(shiftGroup => shiftGroup.Length);
      var listShiftCollections = Enumerable.Range(0, numberOfGroups).Select(i => new List<ShiftCurve>()).ToList();

      // create an array of extended information that will accomodate info about the curves, e.g. the property1 and property2
      var curveInfo = Enumerable.Range(0, numberOfCurves).Select(i => new CurveInformation()).ToArray();
      // create an array that relates the group of our MasterCurveData to the group in the resulting listShiftCollection
      var groupCorrespondence = new int?[numberOfGroups];

      for (int idxCurve = 0; idxCurve < numberOfCurves; idxCurve++)
      {
        bool curveIndexWillParticipateInFit = false;
        var createdShiftCurves = new ShiftCurve?[numberOfGroups];

        AltaxoVariant property1Value = new AltaxoVariant(), property2Value = new AltaxoVariant();

        for (int idxGroup = 0; idxGroup < numberOfGroups; idxGroup++)
        {
          var groupOptions = GetGroupOptions(options, idxGroup);
          // look if the curve has some points
          var xycol = srcData[idxGroup][idxCurve];
          var shiftCurve = xycol is not null ? ConvertToShiftCurve(xycol) : null;
          if (shiftCurve is not null && shiftCurve.Count >= 2 && groupOptions.FittingWeight > 0) // Curve is appropriate to be used for fitting
          {
            curveIndexWillParticipateInFit = true;
            createdShiftCurves[idxGroup] = shiftCurve;
          }

          var (p1, p2) = GetPropertiesOfCurve(xycol, options.Property1Name, options.Property2Name);

          if (!p1.IsEmpty)
          {
            if (property1Value.IsEmpty)
              property1Value = p1;
            else if (property1Value != p1)
              throw new InvalidOperationException();
          }
          if (!p2.IsEmpty)
          {
            if (property2Value.IsEmpty)
              property2Value = p2;
            else if (property2Value != p2)
              throw new InvalidOperationException();
          }
        }

        curveInfo[idxCurve].Property1Value = property1Value;
        curveInfo[idxCurve].Property2Value = property2Value;

        if (curveIndexWillParticipateInFit)
        {
          curveInfo[idxCurve].IndexInShiftGroupCollection = listShiftCollections[0].Count;
          for (int idxGroup = 0; idxGroup < numberOfGroups; idxGroup++)
          {
            listShiftCollections[idxGroup].Add(createdShiftCurves[idxGroup]);
          }
        }
        else
        {
          curveInfo[idxCurve].IndexInShiftGroupCollection = null; // curve will not participate in fit
        }
      }

      // Use only those shift groups that not have at least one curve in it
      var listOfShiftGroups = new List<ShiftGroup>();
      for (int idxGroup = 0; idxGroup < numberOfGroups; idxGroup++)
      {
        if (listShiftCollections[idxGroup].Any(c => c is not null))
        {
          var groupOptions = GetGroupOptions(options, idxGroup);
          Func<(IReadOnlyList<double> X, IReadOnlyList<double> Y, IReadOnlyList<double>? YErr), Func<double, double>>? createInterpolationFunction = null;
          if (groupOptions is MasterCurveGroupOptionsWithScalarInterpolation scalarOptions)
          {
            createInterpolationFunction = (arg) => scalarOptions.InterpolationFunction.Interpolate(arg.X, arg.Y, arg.YErr).GetYOfX;
          }

          var shiftGroup = new ShiftGroup(listShiftCollections[idxGroup],
            groupOptions.XShiftBy,
            groupOptions.FittingWeight,
            groupOptions.LogarithmizeXForInterpolation,
            groupOptions.LogarithmizeYForInterpolation,
            createInterpolationFunction);

          groupCorrespondence[idxGroup] = listOfShiftGroups.Count; // relate idxGroup to index in listOfShiftGroups
          listOfShiftGroups.Add(shiftGroup);
        }
      }

      var shiftGroupCollection = new ShiftGroupCollection(listOfShiftGroups)
      {
        RequiredRelativeOverlap = options.RequiredRelativeOverlap,
        NumberOfIterations = options.NumberOfIterations,
        OptimizationMethod = options.OptimizationMethod,
        ShiftOrder = options.ShiftOrder,
      };

      return (shiftGroupCollection, new FitInformation(curveInfo, groupCorrespondence));
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

    public static (AltaxoVariant property1Value, AltaxoVariant property2Value) GetPropertiesOfCurve(XAndYColumn curve, string property1Name, string property2Name)
    {
      return (IndependentAndDependentColumns.GetPropertyValueOfCurve(curve, property1Name), IndependentAndDependentColumns.GetPropertyValueOfCurve(curve, property2Name));
    }

  }
}
