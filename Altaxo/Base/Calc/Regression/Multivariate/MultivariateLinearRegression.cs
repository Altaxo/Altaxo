﻿#region Copyright

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

#endregion Copyright

#nullable enable
using System;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Probability;
using Altaxo.Collections;
using Altaxo.Data;

namespace Altaxo.Calc.Regression.Multivariate
{
  public class MultivariateLinearFitParameters
  {
    protected DataColumnCollection _table;
    protected IAscendingIntegerCollection _selectedDataColumns;
    protected int _DependentColumnIndexIntoSelection;
    protected bool _IncludeIntercept;
    protected bool _GenerateRegressionValues;
    protected bool _GenerateResidualValues;
    protected IAscendingIntegerCollection? _selectedDataRows;

    public MultivariateLinearFitParameters(DataColumnCollection table, IAscendingIntegerCollection selectedDataColumns)
    {
      _table = table;
      _selectedDataColumns = selectedDataColumns;
    }

    public DataColumnCollection Table
    {
      get
      {
        return _table;
      }
    }

    public IAscendingIntegerCollection SelectedDataColumns
    {
      get
      {
        return _selectedDataColumns;
      }
    }

    public IAscendingIntegerCollection? SelectedDataRows
    {
      get
      {
        return _selectedDataRows;
      }
      set
      {
        _selectedDataRows = value;
      }
    }

    public int DependentColumnIndexIntoSelection
    {
      get
      {
        return _DependentColumnIndexIntoSelection;
      }
      set
      {
        _DependentColumnIndexIntoSelection = value;
      }
    }

    public bool IncludeIntercept
    {
      get
      {
        return _IncludeIntercept;
      }
      set
      {
        _IncludeIntercept = value;
      }
    }

    public bool GenerateRegressionValues
    {
      get
      {
        return _GenerateRegressionValues;
      }
      set
      {
        _GenerateRegressionValues = value;
      }
    }

    public bool GenerateResidualValues
    {
      get
      {
        return _GenerateResidualValues;
      }
      set
      {
        _GenerateResidualValues = value;
      }
    }
  }

  /// <summary>
  /// Summary description for MultivariateLinearRegression.
  /// </summary>
  public class MultivariateLinearRegression
  {
    public static LinearFitBySvd? ShowDialogAndRegress(DataColumnCollection table, IAscendingIntegerCollection selectedColumns)
    {
      if (selectedColumns.Count < 2)
        return null;

      object paramobject = new MultivariateLinearFitParameters(table, selectedColumns);

      if (!Current.Gui.ShowDialog(ref paramobject, "Multivariate linear fit"))
        return null;

      var parameters = (MultivariateLinearFitParameters)paramobject;

      LinearFitBySvd result = Regress(parameters, true);

      return result;
    }

    public static LinearFitBySvd Regress(MultivariateLinearFitParameters parameters, out string[] paramNames)
    {
      DataColumnCollection table = parameters.Table;
      IAscendingIntegerCollection selectedCols = parameters.SelectedDataColumns;
      var selectedColsWODependent = new AscendingIntegerCollection(selectedCols);
      selectedColsWODependent.RemoveAt(parameters.DependentColumnIndexIntoSelection);

      IAscendingIntegerCollection validRows = DataTableWrapper.GetCollectionOfValidNumericRows(parameters.Table, selectedCols);
      parameters.SelectedDataRows = validRows;

      IROMatrix<double> xbase;

      if (parameters.IncludeIntercept)
      {
        xbase = DataTableWrapper.ToROColumnMatrixWithIntercept(parameters.Table, selectedColsWODependent, validRows);
      }
      else
      {
        xbase = DataTableWrapper.ToROColumnMatrix(parameters.Table, selectedColsWODependent, validRows);
      }

      paramNames = new string[xbase.ColumnCount];
      if (parameters.IncludeIntercept)
      {
        paramNames[0] = "Intercept";
        for (int i = 0; i < selectedColsWODependent.Count; i++)
          paramNames[i + 1] = table[selectedColsWODependent[i]].Name;
      }
      else
      {
        for (int i = 0; i < selectedColsWODependent.Count; i++)
          paramNames[i] = table[selectedColsWODependent[i]].Name;
      }

      // Fill the y and the error array
      double[] yarr = new double[validRows.Count];
      double[] earr = new double[validRows.Count];

      var ycol = (Altaxo.Data.INumericColumn)table[selectedCols[parameters.DependentColumnIndexIntoSelection]];

      for (int i = 0; i < validRows.Count; i++)
      {
        yarr[i] = ycol[validRows[i]];
        earr[i] = 1;
      }

      var fit =
        new LinearFitBySvd(
        xbase, yarr, earr, xbase.RowCount, xbase.ColumnCount, 1E-5);

      return fit;
    }

    public static LinearFitBySvd Regress(MultivariateLinearFitParameters parameters, bool outputResults)
    {
      LinearFitBySvd fit = Regress(parameters, out var paramNames);

      if (outputResults)
      {
        OutputFitResults(fit, paramNames);

        if (parameters.GenerateRegressionValues)
        {
          GenerateValues(parameters, fit);
        }
      }
      return fit;
    }

    public static void GenerateValues(MultivariateLinearFitParameters parameters, LinearFitBySvd fit)
    {
      DataColumn dependentColumn = parameters.Table[parameters.SelectedDataColumns[parameters.DependentColumnIndexIntoSelection]];
      var selectedDataRows = parameters.SelectedDataRows ?? throw new InvalidProgramException($"{nameof(parameters.SelectedDataRows)} should be != null here");

      if (parameters.GenerateRegressionValues)
      {
        var col = new DoubleColumn();
        VectorMath.Copy(VectorMath.ToROVector(fit.PredictedValues), DataColumnWrapper.ToVector(col, selectedDataRows));
        parameters.Table.Add(col, dependentColumn.Name + "(predicted)", ColumnKind.V, parameters.Table.GetColumnGroup(dependentColumn));
      }

      if (parameters.GenerateResidualValues)
      {
        var col = new DoubleColumn();
        VectorMath.Copy(VectorMath.ToROVector(fit.ResidualValues), DataColumnWrapper.ToVector(col, selectedDataRows));
        parameters.Table.Add(col, dependentColumn.Name + "(residual)", ColumnKind.V, parameters.Table.GetColumnGroup(dependentColumn));
      }
    }

    public static string GetFitResultsDescription(LinearFitBySvd fit, string[] paramNames)
    {
      var stb = new System.Text.StringBuilder();
      // Output of results

      stb.AppendLine("");
      stb.AppendLine($"---- {DateTime.Now} -----------------------");
      stb.AppendLine($"Multivariate regression of order {fit.NumberOfParameter}");

      stb.AppendFormat("{0,-15} {1,20} {2,20} {3,20} {4,20}",
        "Name", "Value", "Error", "F-Value", "Prob>F");
      stb.AppendLine();

      for (int i = 0; i < fit.Parameter.Length; i++)
      {
        stb.AppendFormat("{0,-15} {1,20} {2,20} {3,20} {4,20}",
                    paramNames is null ? string.Format("A{0}", i) : paramNames[i],
          fit.Parameter[i],
          fit.StandardErrorOfParameter(i),
          fit.TofParameter(i),
          1 - FDistribution.CDF(fit.TofParameter(i), fit.NumberOfParameter, fit.NumberOfData - 1)
          );
        stb.AppendLine();
      }

      stb.AppendLine($"R²: {fit.RSquared}, Adjusted R²: {fit.AdjustedRSquared}");

      stb.AppendLine("------------------------------------------------------------");
      stb.AppendFormat("{0,-12} {1,10}", "Source of", "degrees of");
      stb.AppendLine();
      stb.AppendFormat("{0,-12} {1,10} {2,20} {3,20} {4,20} {5,20}", "variation", "freedom",  "Sum of Squares", "Mean Square", "F0", "P value");
        stb.AppendLine();

      double regressionmeansquare = fit.RegressionCorrectedSumOfSquares / fit.NumberOfParameter;
      double residualmeansquare = fit.ResidualSumOfSquares / (fit.NumberOfData - fit.NumberOfParameter - 1);

      stb.AppendFormat("{0,-12} {1,10} {2,20} {3,20} {4,20} {5,20}",
        "Regression",
        fit.NumberOfParameter,
        fit.RegressionCorrectedSumOfSquares,
        fit.RegressionCorrectedSumOfSquares / fit.NumberOfParameter,
        regressionmeansquare / residualmeansquare,
        1 - FDistribution.CDF(regressionmeansquare / residualmeansquare, fit.NumberOfParameter, fit.NumberOfData - 1)
        );
      stb.AppendLine();

      stb.AppendFormat("{0,-12} {1,10} {2,20} {3,20}",
        "Residual",
        fit.NumberOfData - 1 - fit.NumberOfParameter,
        fit.ResidualSumOfSquares,
        residualmeansquare
        );
      stb.AppendLine();

      stb.AppendFormat("{0,-12} {1,10} {2,20}",
        "Total",
        fit.NumberOfData - 1,
        fit.TotalCorrectedSumOfSquares
        );
      stb.AppendLine();

      stb.AppendLine("------------------------------------------------------------");

      return stb.ToString();
    }

    public static string? OutputFitResults(LinearFitBySvd fit, string[] paramNames)
    {
      // Output of results
      Current.Console.Write(GetFitResultsDescription(fit, paramNames));
      return null;
    }
  }
}
