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
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// Helps creating the master curve.
  /// </summary>
  public class MasterCurveCreation
  {
    #region Helper classes

    /// <summary>
    /// Information about a single curve.
    /// </summary>
    private class CurveInformation
    {
      /// <summary>
      /// Value of property1.
      /// </summary>
      public AltaxoVariant Property1Value;

      /// <summary>
      /// Value of property2.
      /// </summary>
      public AltaxoVariant Property2Value;
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
    public static void Execute(MasterCurveData processData, MasterCurveCreationOptions processOptions, DataTable destinationTable, IProgressReporter reporter)
    {
      // Test the data

      // at first, we create information about the curves
      int numberOfCurves = processData.CurveData.Max(shiftGroup => shiftGroup.Length);

      var effectiveProcessOptions = processOptions;
      var (shiftGroupCollection, curveInfo) = ConvertToShiftGroupCollection(effectiveProcessOptions, processData);

      shiftGroupCollection.CreateMasterCurve(reporter.CancellationToken, reporter);

      // Improve the master curve if required
      if (processOptions.MasterCurveImprovementOptions is { } improvementOptions)
      {
        effectiveProcessOptions = processOptions.With(improvementOptions);

        var (shiftGroupCollectionPrevious, curveInfoPrevios) = (shiftGroupCollection, curveInfo);
        (shiftGroupCollection, curveInfo) = ConvertToShiftGroupCollection(effectiveProcessOptions, processData);
        shiftGroupCollection.ReIterate(shiftGroupCollectionPrevious, reporter.CancellationToken, reporter);
      }

      double? referenceValueUsed = null;
      { // Offset the shift values according to the settings, then reinterpolate everything
        var property1Arr = curveInfo.Select(info => info.Property1Value.IsEmpty ? null : (double?)info.Property1Value).ToArray();
        (var shiftOffset, referenceValueUsed) = shiftGroupCollection.GetShiftOffset(effectiveProcessOptions.ReferenceValue, effectiveProcessOptions.UseExactReferenceValue, property1Arr, effectiveProcessOptions.IndexOfReferenceColumnInColumnGroup);
        shiftGroupCollection.SetShiftOffset(-shiftOffset);
        shiftGroupCollection.ReinterpolateAllGroups();
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
              pcol1[col.GetColumnNumber(yCol)] = curveInfo[idxCurve].Property1Value;
            }
            if (pcol2 is not null)
            {
              pcol2[col.GetColumnNumber(yCol)] = curveInfo[idxCurve].Property2Value;
            }
          }
        }
      }    // end copying the original data

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
          var groupOptions = GetGroupOptions(effectiveProcessOptions, idxGroup);
          for (int idxCurve = 0; idxCurve < group.Length; idxCurve++)
          {
            if (!(shiftGroupCollection.ShiftValues[idxCurve] is { } shiftValue))
              continue; // if this particular curve has no shift information, we skip it

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
            yColumnList.Add((groupNumber, $"ySft{(char)('A' + idxGroup)}{idxCurve}", yCol, curveInfo[idxCurve].Property1Value, curveInfo[idxCurve].Property2Value, shiftValue));
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
        for (int idxGroup = 0, idxColumn = 0; idxGroup < shiftGroupCollection.Count; idxGroup++)
        {
          var group = shiftGroupCollection[idxGroup];
          for (int idxComponent = 0; idxComponent < group.NumberOfValueComponents; ++idxComponent)
          {
            var data = group.GetMergedCurvePointsUsedForInterpolation(idxComponent);
            var xcol = col.EnsureExistence($"xMerged{(char)('A' + idxColumn)}", typeof(DoubleColumn), ColumnKind.X, groupNumber);
            var ycol = col.EnsureExistence($"yMerged{(char)('A' + idxColumn)}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
            var zcol = col.EnsureExistence($"indexMerged{(char)('A' + idxColumn)}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
            ++groupNumber;
            ++idxColumn;

            for (int i = 0; i < data.X.Count; ++i)
            {
              xcol[i] = data.X[i];
              ycol[i] = data.Y[i];
              zcol[i] = data.IndexOfCurve[i];
            }
          }
        } // for each group
      } // end put merged curves


      if (processOptions.TableOutputOptions.OutputInterpolatedCurve)
      {
        groupNumber = (int)(100 * Math.Ceiling((groupNumber + 1) / 100d));


        // get out the interpolation result, Achtung die Werte sind logarithmiert
        for (int idxGroup = 0, idxColumn = 0; idxGroup < shiftGroupCollection.Count; idxGroup++)
        {
          var group = shiftGroupCollection[idxGroup];
          for (int idxComponent = 0; idxComponent < group.NumberOfValueComponents; ++idxComponent)
          {
            var resultCurve = group.GetInterpolatedCurvePoints(idxComponent, 1001);

            var xcol = col.EnsureExistence($"xInterpolated{(char)('A' + idxColumn)}", typeof(DoubleColumn), ColumnKind.X, groupNumber);
            var ycol = col.EnsureExistence($"yInterpolated{(char)('A' + idxColumn)}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
            ++groupNumber;
            ++idxColumn;

            for (int i = 0; i < resultCurve.X.Count; ++i)
            {
              xcol[i] = resultCurve.X[i];
              ycol[i] = resultCurve.Y[i];
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
            if (shiftGroupCollection.ShiftValues is { } shiftValue)
            {
              prop1Col[idxCurve] = curveInfo[idxCurve].Property1Value;

              if (prop1ColInv is not null)
              {
                var t = new Altaxo.Science.Temperature(curveInfo[idxCurve].Property1Value, processOptions.Property1TemperatureRepresentation.Value);
                prop1ColInv[idxCurve] = t.InInverseKelvin;
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
            if (shiftGroupCollection.ShiftValues[idxCurve].HasValue)
            {
              prop2Col[idxCurve] = curveInfo[idxCurve].Property2Value;
            }
          }
        }

        // now the shift values
        var shiftCol = (DoubleColumn)col.EnsureExistence("Shift", typeof(DoubleColumn), ColumnKind.V, groupNumber);
        shiftCol.Data = shiftGroupCollection.ShiftValues.Select(shift => shift ?? double.NaN);
        if (shiftGroupCollection.ShiftErrors.Any(x => x.HasValue))
        {
          var shiftColErr = (DoubleColumn)col.EnsureExistence("Shift.Err", typeof(DoubleColumn), ColumnKind.Err, groupNumber);
          shiftColErr.Data = shiftGroupCollection.ShiftErrors.Select(shiftErr => shiftErr ?? double.NaN);
        }

        // Output the factor only if any for the group uses shift by factor
        if (processOptions.GroupOptions.Any(g => g.XShiftBy == ShiftXBy.Factor))
        {
          var shiftFactorCol = (DoubleColumn)col.EnsureExistence("ShiftFactor", typeof(DoubleColumn), ColumnKind.V, groupNumber);
          shiftFactorCol.Data = shiftCol.Array.Select(x => Math.Exp(x)).ToArray();

          if (shiftGroupCollection.ShiftErrors.Any(x => x.HasValue))
          {
            var shiftColmErr = (DoubleColumn)col.EnsureExistence("ShiftFactor.mErr", typeof(DoubleColumn), ColumnKind.mErr, groupNumber);
            var shiftColpErr = (DoubleColumn)col.EnsureExistence("ShiftFactor.pErr", typeof(DoubleColumn), ColumnKind.pErr, groupNumber);
            shiftColmErr.Data = shiftGroupCollection.ShiftValues.Zip(shiftGroupCollection.ShiftErrors, (x, xerr) => (x, xerr)).Select(t => t.x.HasValue && t.xerr.HasValue ? Math.Exp(t.x.Value) - Math.Exp(t.x.Value - t.xerr.Value) : double.NaN);
            shiftColpErr.Data = shiftGroupCollection.ShiftValues.Zip(shiftGroupCollection.ShiftErrors, (x, xerr) => (x, xerr)).Select(t => t.x.HasValue && t.xerr.HasValue ? Math.Exp(t.x.Value + t.xerr.Value) - Math.Exp(t.x.Value) : double.NaN);
          }
        }
      }

      if (processOptions.Property1TemperatureRepresentation.HasValue)
      {
        groupNumber = (int)(100 * Math.Ceiling((groupNumber + 1) / 100d));

        // calculate the Activation energies
        if (curveInfo.Any(i => !(i.Property2Value.IsEmpty)))
        {
          // then we have to calculate the activation energies per property2
          var prop2Col = (DoubleColumn)col.EnsureExistence("Ea_For_" + processOptions.Property2Name, typeof(DoubleColumn), ColumnKind.V, groupNumber);
          var energyCol = (DoubleColumn)col.EnsureExistence("Ea_JoulePerMole", typeof(DoubleColumn), ColumnKind.V, groupNumber);
          var energyErrCol = (DoubleColumn)col.EnsureExistence("Ea_JoulePerMole.Err", typeof(DoubleColumn), ColumnKind.Err, groupNumber);
          var k0Col = (DoubleColumn)col.EnsureExistence("ArrheniusPrefactor", typeof(DoubleColumn), ColumnKind.V, groupNumber);
          var k0ErrCol = (DoubleColumn)col.EnsureExistence("ArrheniusPrefactor.Err", typeof(DoubleColumn), ColumnKind.Err, groupNumber);

          // we first collect all property2 values
          var prop2Hash = new HashSet<AltaxoVariant>();
          for (int idxCurve = 0; idxCurve < numberOfCurves; ++idxCurve)
          {
            if (shiftGroupCollection.ShiftValues[idxCurve] is { } shiftValue &&
              !curveInfo[idxCurve].Property1Value.IsEmpty &&
              !curveInfo[idxCurve].Property2Value.IsEmpty)
            {
              prop2Hash.Add(curveInfo[idxCurve].Property2Value);
            }
          }

          // for every property2, we calculate the activation energy
          int idxRow = 0;
          foreach (var prop2 in prop2Hash.OrderBy(x => x))
          {
            // regress the shift values versus the inverse temperature
            var listShift = new List<double>();
            var listInvTemp = new List<double>();
            for (int idxCurve = 0; idxCurve < numberOfCurves; ++idxCurve)
            {
              if (shiftGroupCollection.ShiftValues[idxCurve] is { } shiftValue &&
                  !curveInfo[idxCurve].Property1Value.IsEmpty &&
                  curveInfo[idxCurve].Property2Value == prop2
                 )
              {
                var t = new Altaxo.Science.Temperature(curveInfo[idxCurve].Property1Value, processOptions.Property1TemperatureRepresentation.Value);
                listShift.Add(shiftValue);
                listInvTemp.Add(t.InInverseKelvin);
              }
            }
            if (listInvTemp.Count >= 2)
            {
              var reg = new Altaxo.Calc.Regression.LinearFitBySvd(listInvTemp.ToArray(), listShift.ToArray(), null, listInvTemp.Count, 2, (x, arr) => { arr[0] = 1; arr[1] = x; }, 1E-12);
              var parameter = reg.Parameter;

              prop2Col[idxRow] = prop2;
              k0Col[idxRow] = Math.Exp(parameter[0]);
              k0ErrCol[idxRow] = reg.StandardErrorOfParameter(0) * Math.Exp(parameter[0]);
              energyCol[idxRow] = parameter[1] * Science.SIConstants.MOLAR_GAS;
              energyErrCol[idxRow] = reg.StandardErrorOfParameter(1) * Science.SIConstants.MOLAR_GAS;
              ++idxRow;
            }
          }
          {
            // now calculate also an activation energy for all together, but with separate prefactors for each property 2
            var xbase = new List<double[]>();
            var listShift = new List<double>();
            var prop2Dict = new Dictionary<AltaxoVariant, int>(); // for each property2, gets the index of this property
            prop2Dict.AddRange(prop2Hash.OrderBy(x => x).Select((x, i) => new KeyValuePair<AltaxoVariant, int>(x, i)));
            for (int idxCurve = 0; idxCurve < numberOfCurves; ++idxCurve)
            {
              if (shiftGroupCollection.ShiftValues[idxCurve] is { } shiftValue &&
                  !curveInfo[idxCurve].Property1Value.IsEmpty &&
                  !curveInfo[idxCurve].Property2Value.IsEmpty
                 )
              {

                var basearr = new double[prop2Dict.Count + 1];
                var t = new Altaxo.Science.Temperature(curveInfo[idxCurve].Property1Value, processOptions.Property1TemperatureRepresentation.Value);
                basearr[0] = t.InInverseKelvin; // attention: here x is at position 0!
                var ii = prop2Dict[curveInfo[idxCurve].Property2Value];
                basearr[ii + 1] = 1; // for each of the properties2, a separate offset value
                xbase.Add(basearr);
                listShift.Add(shiftValue);
              }
            }
            var basematrix = MatrixMath.ToMatrixFromLeftSpineJaggedArray(xbase.ToArray());
            var reg = new Altaxo.Calc.Regression.LinearFitBySvd(basematrix, listShift.ToArray(), null, listShift.Count, prop2Dict.Count + 1, 1E-12);
            var parameter = reg.Parameter;
            energyCol[idxRow] = parameter[0] * Science.SIConstants.MOLAR_GAS;
            energyErrCol[idxRow] = reg.StandardErrorOfParameter(0) * Science.SIConstants.MOLAR_GAS;
            idxRow++;
          }

        }
        else
        {
          var energyCol = (DoubleColumn)col.EnsureExistence("Ea_JoulePerMole", typeof(DoubleColumn), ColumnKind.V, groupNumber);
          var energyErrCol = (DoubleColumn)col.EnsureExistence("Ea_JoulePerMole.Err", typeof(DoubleColumn), ColumnKind.Err, groupNumber);
          var k0Col = (DoubleColumn)col.EnsureExistence("ArrheniusPrefactor", typeof(DoubleColumn), ColumnKind.V, groupNumber);
          var k0ErrCol = (DoubleColumn)col.EnsureExistence("ArrheniusPrefactor.Err", typeof(DoubleColumn), ColumnKind.Err, groupNumber);

          // one activation energy for all
          // regress the shift values versus the inverse temperature
          var listShift = new List<double>();
          var listInvTemp = new List<double>();
          for (int idxCurve = 0; idxCurve < numberOfCurves; ++idxCurve)
          {
            if (shiftGroupCollection.ShiftValues[idxCurve] is { } shiftValue && !curveInfo[idxCurve].Property1Value.IsEmpty)
            {
              var t = new Altaxo.Science.Temperature(curveInfo[idxCurve].Property1Value, processOptions.Property1TemperatureRepresentation.Value);
              listShift.Add(shiftValue);
              listInvTemp.Add(t.InInverseKelvin);
            }
          }
          var reg = new Altaxo.Calc.Regression.LinearFitBySvd(listInvTemp.ToArray(), listShift.ToArray(), null, listInvTemp.Count, 2, (x, arr) => { arr[0] = 1; arr[1] = x; }, 1E-12);
          var parameter = reg.Parameter;

          k0Col[0] = Math.Exp(parameter[0]);
          k0ErrCol[0] = reg.StandardErrorOfParameter(0) * Math.Exp(parameter[0]);
          energyCol[0] = parameter[1] * Science.SIConstants.MOLAR_GAS;
          energyErrCol[0] = reg.StandardErrorOfParameter(1) * Science.SIConstants.MOLAR_GAS;
        }
      }

      // Finally, we put the actually used Reference value to a table property "MasterCurveReferenceValue"
      if (referenceValueUsed.HasValue)
      {
        destinationTable.SetTableProperty("MasterCurveReferenceValue", referenceValueUsed.Value);
      }
    }

    private static IReadOnlyList<(double x, double y, int idxCurve)> GetMergedCurveData(XAndYColumn?[] originalCurves, MasterCurveGroupOptions groupOptions, ShiftGroupCollection masterCurveResult, IReadOnlyList<CurveInformation> curveInfo, double shiftOffset)
    {
      var resultCurve = new List<(double x, double y, int idxCurve)>();
      for (int idxCurve = 0; idxCurve < originalCurves.Length; idxCurve++)
      {

        if (!(masterCurveResult.ShiftValues[idxCurve] is { } shiftValue))
          continue; // if this particular curve has no shift information, we skip it

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
    /// <param name="highToLowLevelMapping">The mapping of the high level data (see <see cref="MasterCurveData"/>), to the low level data (see <see cref="ShiftGroupCollectionDouble"/>.</param>
    /// <param name="processOptions">The master curve creation options.</param>
    /// <returns>A tuple containing the shift offset, with which the entire curve should be shifted, and the actually used reference value (e.g. reference temperature).</returns>
    private static (double shiftOffset, AltaxoVariant referenceValue) GetShiftOffset(ShiftGroupCollection masterCurveResult, IReadOnlyList<CurveInformation> curveInfo, MasterCurveCreationOptions processOptions)
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
          foreach (var idxCurve in masterCurveResult.CurvesParticipatingInFit)
          {
            if (!curveInfo[idxCurve].Property1Value.IsEmpty)
            {
              double x = curveInfo[idxCurve].Property1Value;
              double y = masterCurveResult.ShiftValues[idxCurve] ?? double.NaN;

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
          foreach (var idxCurve in masterCurveResult.CurvesParticipatingInFit)
          {
            if (!curveInfo[idxCurve].Property1Value.IsEmpty)
            {
              var distance = Math.Abs(curveInfo[idxCurve].Property1Value - referenceValue);
              if (distance < minDistance)
              {
                minDistance = distance;
                shiftOffset = masterCurveResult.ShiftValues[idxCurve].Value;
                referenceValueUsed = curveInfo[idxCurve].Property1Value;
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
        foreach (var idxCurve in masterCurveResult.CurvesParticipatingInFit)
        {
          var distance = Math.Abs(idxCurve - refIndex);
          if (distance < minDistance)
          {
            minDistance = distance;
            shiftOffset = masterCurveResult.ShiftValues[idxCurve].Value;
            referenceValueUsed = curveInfo[idxCurve].Property1Value;
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
    /// Converts the <see cref="MasterCurveData"/> to <see cref="ShiftGroupCollectionDouble"/>.
    /// Only those curves are converted which participate on the fit, i.e. the curve must have at least two points,
    /// and the fitting weight must be positive.
    /// </summary>
    /// <param name="processData">The process data.</param>
    /// <returns>The <see cref="ShiftGroupCollectionDouble"/> containing the data to be shifted.</returns>
    private static (ShiftGroupCollection shiftGroupCollection, IReadOnlyList<CurveInformation> fitInformation) ConvertToShiftGroupCollection(MasterCurveCreationOptions options, MasterCurveData processData)
    {
      var listOfShiftGroups = new List<IShiftGroup>();
      var srcData = processData.CurveData;

      for (int idxDataColumn = 0, idxGroup = 0; idxDataColumn < srcData.Count; ++idxGroup)
      {
        var groupOptions = GetGroupOptions(options, idxGroup);
        IShiftGroup shiftGroup;
        if (groupOptions is MasterCurveGroupOptionsWithComplexInterpolation complexGroupOptions)
        {
          if (complexGroupOptions.InterpolationFunction.IsSupportingSeparateXForRealAndImaginaryPart)
          {
            shiftGroup = ConvertToShiftGroupComplexSeparateX(complexGroupOptions, processData, idxGroup, ref idxDataColumn);
          }
          else
          {
            shiftGroup = ConvertToShiftGroupComplexCommonX(complexGroupOptions, processData, idxGroup, ref idxDataColumn);
          }
        }
        else if (groupOptions is MasterCurveGroupOptionsWithScalarInterpolation scalarGroupOptions)
        {
          shiftGroup = ConvertToShiftGroupDouble(scalarGroupOptions, processData, idxGroup, ref idxDataColumn);
        }
        else
        {
          throw new NotImplementedException();
        }
        listOfShiftGroups.Add(shiftGroup);
      }


      // create an array of extended information that will accomodate info about the curves, e.g. the property1 and property2
      int numberOfCurves = listOfShiftGroups.Max(x => x.Count);
      var curveInfo = Enumerable.Range(0, numberOfCurves).Select(i => new CurveInformation()).ToArray();
      for (int idxCurve = 0; idxCurve < numberOfCurves; idxCurve++)
      {
        AltaxoVariant property1Value = new AltaxoVariant(), property2Value = new AltaxoVariant();
        for (int idxDataColumn = 0; idxDataColumn < srcData.Count; idxDataColumn++)
        {
          var xycol = srcData[idxDataColumn][idxCurve];
          var (p1, p2) = GetPropertiesOfCurve(xycol, options.Property1Name, options.Property2Name);

          if (!p1.IsEmpty)
          {
            if (property1Value.IsEmpty)
              property1Value = p1;
            else if (property1Value != p1)
              throw new InvalidOperationException($"The value property {options.Property1Name} of curve[{idxCurve}] in column[{idxDataColumn}] is {p1}, but in a previous column it was {property1Value}");
          }
          if (!p2.IsEmpty)
          {
            if (property2Value.IsEmpty)
              property2Value = p2;
            else if (property2Value != p2)
              throw new InvalidOperationException($"The value property {options.Property2Name} of curve[{idxCurve}] in column[{idxDataColumn}] is {p2}, but in a previous column it was {property2Value}");
          }
        }

        curveInfo[idxCurve].Property1Value = property1Value;
        curveInfo[idxCurve].Property2Value = property2Value;
      }



      var shiftGroupCollection = new ShiftGroupCollection(listOfShiftGroups)
      {
        RequiredRelativeOverlap = options.RequiredRelativeOverlap,
        NumberOfIterations = options.NumberOfIterations,
        OptimizationMethod = options.OptimizationMethod,
        ShiftOrder = options.ShiftOrder,
      };

      return (shiftGroupCollection, curveInfo);
    }

    /// <summary>
    /// Converts the <see cref="MasterCurveData"/> to <see cref="ShiftGroupCollectionDouble"/>.
    /// Only those curves are converted which participate on the fit, i.e. the curve must have at least two points,
    /// and the fitting weight must be positive.
    /// </summary>
    /// <param name="processData">The process data.</param>
    /// <returns>The <see cref="ShiftGroupCollectionDouble"/> containing the data to be shifted.</returns>
    private static ShiftGroupDouble ConvertToShiftGroupDouble(MasterCurveGroupOptionsWithScalarInterpolation groupOptions, MasterCurveData processData, int idxGroup, ref int idxDataColumn)
    {
      var srcData = processData.CurveData;
      if (!(idxDataColumn >= 0 && idxDataColumn < srcData.Count))
        throw new IndexOutOfRangeException(nameof(idxDataColumn));

      var numberOfCurves = srcData.Max(shiftGroup => shiftGroup.Length);
      var listOfShiftCurves = new ShiftCurve<double>?[numberOfCurves];
      for (int idxCurve = 0; idxCurve < numberOfCurves; idxCurve++)
      {
        // look if the curve has some points
        var xycol = srcData[idxGroup][idxCurve];
        listOfShiftCurves[idxCurve] = xycol is not null ? ConvertToShiftCurve(xycol) : null;
      }

      var shiftGroup = new ShiftGroupDouble(listOfShiftCurves,
        groupOptions.XShiftBy,
        groupOptions.FittingWeight,
        groupOptions.LogarithmizeXForInterpolation,
        groupOptions.LogarithmizeYForInterpolation,
        (arg) => groupOptions.InterpolationFunction.Interpolate(arg.X, arg.Y, arg.YErr).GetYOfX);

      idxDataColumn += 1;

      return shiftGroup;
    }

    /// <summary>
    /// Converts the <see cref="MasterCurveData"/> to <see cref="ShiftGroupCollectionDouble"/>.
    /// Only those curves are converted which participate on the fit, i.e. the curve must have at least two points,
    /// and the fitting weight must be positive.
    /// </summary>
    /// <param name="processData">The process data.</param>
    /// <returns>The <see cref="ShiftGroupCollectionDouble"/> containing the data to be shifted.</returns>
    private static ShiftGroupComplexCommonX ConvertToShiftGroupComplexCommonX(MasterCurveGroupOptionsWithComplexInterpolation groupOptions, MasterCurveData processData, int idxGroup, ref int idxDataColumn)
    {
      var srcData = processData.CurveData;
      if (!(idxDataColumn >= 0 && idxDataColumn < srcData.Count - 1))
        throw new IndexOutOfRangeException(nameof(idxDataColumn));

      var numberOfCurves = srcData.Max(shiftGroup => shiftGroup.Length);
      var listShiftCollection = new ShiftCurve<Complex64>?[numberOfCurves];

      for (int idxCurve = 0; idxCurve < numberOfCurves; idxCurve++)
      {
        // look if the curve has some points
        var xycolreal = srcData[0][idxCurve];
        var xycolimag = srcData[1][idxCurve];
        listShiftCollection[idxCurve] = ConvertToShiftCurveComplex(xycolreal, xycolimag);
      }

      var shiftGroup = new ShiftGroupComplexCommonX(
        listShiftCollection,
        groupOptions.XShiftBy,
        groupOptions.FittingWeight,
        groupOptions.FittingWeightIm,
        groupOptions.LogarithmizeXForInterpolation,
        groupOptions.LogarithmizeYForInterpolation,
        (arg) => groupOptions.InterpolationFunction.Interpolate(arg.X, arg.Y, arg.YErr).GetYOfX);

      idxDataColumn += 2;

      return shiftGroup;
    }

    /// <summary>
    /// Converts the <see cref="MasterCurveData"/> to <see cref="ShiftGroupCollectionDouble"/>.
    /// Only those curves are converted which participate on the fit, i.e. the curve must have at least two points,
    /// and the fitting weight must be positive.
    /// </summary>
    /// <param name="processData">The process data.</param>
    /// <returns>The <see cref="ShiftGroupCollectionDouble"/> containing the data to be shifted.</returns>
    private static ShiftGroupComplexSeparateX ConvertToShiftGroupComplexSeparateX(MasterCurveGroupOptionsWithComplexInterpolation groupOptions, MasterCurveData processData, int idxGroup, ref int idxDataColumn)
    {
      var srcData = processData.CurveData;

      if (!(idxDataColumn >= 0 && idxDataColumn < srcData.Count - 1))
        throw new IndexOutOfRangeException(nameof(idxDataColumn));

      var numberOfCurves = srcData.Max(shiftGroup => shiftGroup.Length);
      var listShiftCollectionReal = new ShiftCurve<double>?[numberOfCurves];
      var listShiftCollectionImag = new ShiftCurve<double>?[numberOfCurves];

      // create an array of extended information that will accomodate info about the curves, e.g. the property1 and property2
      var curveInfo = Enumerable.Range(0, numberOfCurves).Select(i => new CurveInformation()).ToArray();

      for (int idxCurve = 0; idxCurve < numberOfCurves; idxCurve++)
      {
        // look if the curve has some points
        var xycolreal = srcData[idxDataColumn + 0][idxCurve];
        var xycolimag = srcData[idxDataColumn + 1][idxCurve];
        listShiftCollectionReal[idxCurve] = ConvertToShiftCurve(xycolreal);
        listShiftCollectionImag[idxCurve] = ConvertToShiftCurve(xycolimag);
      }

      var shiftGroup = new ShiftGroupComplexSeparateX(
        listShiftCollectionReal,
        listShiftCollectionImag,
        groupOptions.XShiftBy,
        groupOptions.FittingWeight,
        groupOptions.FittingWeightIm,
        groupOptions.LogarithmizeXForInterpolation,
        groupOptions.LogarithmizeYForInterpolation,
        (arg) => groupOptions.InterpolationFunction.Interpolate(arg.XRe, arg.YRe, arg.XIm, arg.YIm).GetYOfX
        );

      idxDataColumn += 2;
      return shiftGroup;
    }


    /// <summary>
    /// Converts <see cref="XAndYColumn"/> data to a <see cref="ShiftCurve"/>.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>The shift curve (if there is any data). If the argument contains to data rows, then the return value is null.</returns>
    private static ShiftCurve<double>? ConvertToShiftCurve(XAndYColumn? data)
    {
      if (data is null)
        return null;

      var (x, y, rowCount) = data.GetResolvedXYData();

      if (rowCount == 0)
        return null;
      else
        return new ShiftCurve<double>(x, y);
    }

    /// <summary>
    /// Converts <see cref="XAndYColumn"/> data to a <see cref="ShiftCurve"/>.
    /// </summary>
    /// <param name="dataReal">The data of the real part.</param>
    /// <param name="dataImag">The data of the imaginary part.</param>
    /// <returns>The shift curve (if there is any data). If the argument contains to data rows, then the return value is null.</returns>
    private static ShiftCurve<Complex64>? ConvertToShiftCurveComplex(XAndYColumn? dataReal, XAndYColumn? dataImag)
    {
      if (dataReal is null || dataImag is null)
        return null;

      var (xre, yre, rowCountRe) = dataReal.GetResolvedXYData();
      var (xim, yim, rowCountIm) = dataImag.GetResolvedXYData();

      if (rowCountRe == 0 || rowCountIm == 0)
        return null;

      var dict = new Dictionary<double, double>();
      for (int i = 0; i < rowCountIm; ++i)
        dict[xim[i]] = yim[i];

      var curvePointsX = new List<double>();
      var curvePointsY = new List<Complex64>();
      for (int i = 0; i < Math.Min(rowCountRe, rowCountIm); ++i)
      {
        var x = xre[i];
        var yr = yre[i];
        var yi = double.NaN;
        if (xre[i] == xim[i])
        {
          yi = yim[i];
        }
        else if (dict.TryGetValue(x, out var v))
        {
          yi = v;
        }

        if (!(double.IsNaN(x) || double.IsNaN(yr) || double.IsNaN(yi)))
        {
          curvePointsX.Add(x);
          curvePointsY.Add(new Complex64(yr, yi));
        }
      }

      return new ShiftCurve<Complex64>(curvePointsX, curvePointsY);
    }



    public static (AltaxoVariant property1Value, AltaxoVariant property2Value) GetPropertiesOfCurve(XAndYColumn curve, string property1Name, string property2Name)
    {
      return (IndependentAndDependentColumns.GetPropertyValueOfCurve(curve, property1Name), IndependentAndDependentColumns.GetPropertyValueOfCurve(curve, property2Name));
    }

  }
}
