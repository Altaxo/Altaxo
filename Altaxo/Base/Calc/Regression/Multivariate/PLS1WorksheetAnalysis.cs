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

#endregion Copyright

#nullable enable
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// PLS2WorksheetAnalysis performs a PLS1 analysis and
  /// stores the results in a given table
  /// </summary>
  [System.ComponentModel.Description("PLS1")]
  public class PLS1WorksheetAnalysis : WorksheetAnalysis
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PLS1WorksheetAnalysis), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new PLS1WorksheetAnalysis();
      }
    }
    #endregion

    public override string AnalysisName
    {
      get
      {
        return "PLS1";
      }
    }

    public override MultivariateRegression CreateNewRegressionObject()
    {
      return new PLS1Regression();
    }

    public override void StoreCalibrationModelInTable(
      IMultivariateCalibrationModel calibrationSet,
      DataTable table)
    {
      var calib = (PLS1CalibrationModel)calibrationSet;

      for (int yn = 0; yn < calib.NumberOfY; yn++)
      {
        // store the x-loads - careful - they are horizontal in the matrix
        for (int i = 0; i < calib.XLoads[yn].RowCount; i++)
        {
          var col = new Altaxo.Data.DoubleColumn();

          for (int j = 0; j < calib.XLoads[yn].ColumnCount; j++)
            col[j] = calib.XLoads[yn][i, j];

          table.DataColumns.Add(col, GetXLoad_ColumnName(yn, i), Altaxo.Data.ColumnKind.V, 0);
        }

        // now store the y-loads - careful - they are horizontal in the matrix
        for (int i = 0; i < calib.YLoads[yn].RowCount; i++)
        {
          var col = new Altaxo.Data.DoubleColumn();

          for (int j = 0; j < calib.YLoads[yn].ColumnCount; j++)
            col[j] = calib.YLoads[yn][i, j];

          table.DataColumns.Add(col, GetYLoad_ColumnName(yn, i), Altaxo.Data.ColumnKind.V, 1);
        }

        // now store the weights - careful - they are horizontal in the matrix
        for (int i = 0; i < calib.XWeights[yn].RowCount; i++)
        {
          var col = new Altaxo.Data.DoubleColumn();

          for (int j = 0; j < calib.XWeights[yn].ColumnCount; j++)
            col[j] = calib.XWeights[yn][i, j];

          table.DataColumns.Add(col, GetXWeight_ColumnName(yn, i), Altaxo.Data.ColumnKind.V, 0);
        }

        // now store the cross product vector - it is a horizontal vector
        {
          var col = new Altaxo.Data.DoubleColumn();

          for (int j = 0; j < calib.CrossProduct[yn].ColumnCount; j++)
            col[j] = calib.CrossProduct[yn][0, j];
          table.DataColumns.Add(col, GetCrossProduct_ColumnName(yn), Altaxo.Data.ColumnKind.V, 3);
        }
      } // for all y (constituents)
    }

    public override IMultivariateCalibrationModel GetCalibrationModel(
      DataTable calibTable)
    {
      Export(calibTable, out var model);
      return model;
    }

    private static int GetNumberOfX(Altaxo.Data.DataTable table)
    {
      var col = table.DataColumns.TryGetColumn(GetXLoad_ColumnName(0, 0));
      if (col is null)
        NotFound(GetXLoad_ColumnName(0, 0));
      return col.Count;
    }

    private static int GetNumberOfY(Altaxo.Data.DataTable table)
    {
      var col = table.DataColumns.TryGetColumn(GetYLoad_ColumnName(0, 0));
      if (col is null)
        NotFound(GetYLoad_ColumnName(0, 0));
      return col.Count;
    }

    private static int GetNumberOfFactors(Altaxo.Data.DataTable table)
    {
      var col = table.DataColumns.TryGetColumn(GetCrossProduct_ColumnName(0));
      if (col is null)
        NotFound(GetCrossProduct_ColumnName(0));
      return col.Count;
    }

    public static bool IsPLS1CalibrationModel(Altaxo.Data.DataTable table)
    {
      if (!table.DataColumns.Contains(GetXOfX_ColumnName()))
        return false;
      if (!table.DataColumns.Contains(GetXMean_ColumnName()))
        return false;
      if (!table.DataColumns.Contains(GetXScale_ColumnName()))
        return false;
      if (!table.DataColumns.Contains(GetYMean_ColumnName()))
        return false;
      if (!table.DataColumns.Contains(GetYScale_ColumnName()))
        return false;

      if (!table.DataColumns.Contains(GetXLoad_ColumnName(0, 0)))
        return false;
      if (!table.DataColumns.Contains(GetXWeight_ColumnName(0, 0)))
        return false;
      if (!table.DataColumns.Contains(GetYLoad_ColumnName(0, 0)))
        return false;
      if (!table.DataColumns.Contains(GetCrossProduct_ColumnName(0)))
        return false;

      return true;
    }

    /// <summary>
    /// Exports a table to a PLS2CalibrationSet
    /// </summary>
    /// <param name="table">The table where the calibration model is stored.</param>
    /// <param name="calibrationSet"></param>
    public static void Export(
      DataTable table,
      out PLS1CalibrationModel calibrationSet)
    {
      int numberOfX = GetNumberOfX(table);
      int numberOfY = GetNumberOfY(table);
      int numberOfFactors = GetNumberOfFactors(table);

      calibrationSet = new PLS1CalibrationModel
      {
        NumberOfX = numberOfX,
        NumberOfY = numberOfY,
        NumberOfFactors = numberOfFactors
      };
      var preprocessSet = new MultivariatePreprocessingModel();
      if (IsDimensionReductionAndRegressionModel(table, out var dataSource))
      {
        preprocessSet.PreprocessSingleSpectrum = dataSource.ProcessOptions.Preprocessing;
      }

      var sel = new Altaxo.Collections.AscendingIntegerCollection();
      Altaxo.Data.DataColumn? col;

      col = table.DataColumns.TryGetColumn(GetXOfX_ColumnName());
      if (col is null || !(col is INumericColumn))
        NotFound(GetXOfX_ColumnName());
      preprocessSet.XOfX = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector((INumericColumn)col, numberOfX);

      if (table.DataColumns.Contains(GetXMean_ColumnName()) && table.DataColumns.Contains(GetXScale_ColumnName()))
      {
        // the table is an old style table (before 2026-02-16), in which the ensembly mean and ensemble scale were stored as columns. We need to convert this to the new style, where the ensemble mean and scale are stored as auxiliary data of the preprocessor
        var colM = (DoubleColumn)table.DataColumns.TryGetColumn(GetXMean_ColumnName());
        var colS = (DoubleColumn)table.DataColumns.TryGetColumn(GetXMean_ColumnName());


        preprocessSet.AuxiliaryDataX = new EnsembleAuxiliaryDataCompound
        {
          Name = EnsembleMeanScale.AuxiliaryDataName,
          Values = [
            new EnsembleAuxiliaryDataVector() { Name = EnsembleMeanScale.AuxiliaryDataMeanName, VectorType = EnsembleAuxiliaryDataVectorType.Spectrum, Value = (double[])colM.Array.Clone() },
            new EnsembleAuxiliaryDataVector() { Name = EnsembleMeanScale.AuxiliaryDataMeanName, VectorType = EnsembleAuxiliaryDataVectorType.Spectrum, Value = (double[])colS.Array.Clone() },
                  ],
        };
      }
      else if (table.DataSource is DimensionReductionAndRegressionDataSource ds)
      {
        // the table is a new style table (after 2026-02-16), in which the ensemble preprocessing auxiliary data is stored in the data source
        preprocessSet.AuxiliaryDataX = ds.ProcessResult.AuxiliaryData;
      }
      else
      {
        NotFound(GetXMean_ColumnName());
      }

      sel.Clear();
      col = table.DataColumns.TryGetColumn(GetYMean_ColumnName());
      if (col is null)
        NotFound(GetYMean_ColumnName());
      sel.Add(table.DataColumns.GetColumnNumber(col));
      preprocessSet.YMean = DataColumnWrapper.ToROVector(col, numberOfY);

      sel.Clear();
      col = table.DataColumns.TryGetColumn(GetYScale_ColumnName());
      if (col is null)
        NotFound(GetYScale_ColumnName());
      sel.Add(table.DataColumns.GetColumnNumber(col));
      preprocessSet.YScale = DataColumnWrapper.ToROVector(col, numberOfY);

      for (int yn = 0; yn < numberOfY; yn++)
      {
        sel.Clear();
        for (int i = 0; i < numberOfFactors; i++)
        {
          string colname = GetXWeight_ColumnName(yn, i);
          col = table.DataColumns.TryGetColumn(colname);
          if (col is null)
            NotFound(colname);
          sel.Add(table.DataColumns.GetColumnNumber(col));
        }
        calibrationSet.XWeights[yn] = DataTableWrapper.ToRORowMatrix(table.DataColumns, sel, numberOfX);

        sel.Clear();
        for (int i = 0; i < numberOfFactors; i++)
        {
          string colname = GetXLoad_ColumnName(yn, i);
          col = table.DataColumns.TryGetColumn(colname);
          if (col is null)
            NotFound(colname);
          sel.Add(table.DataColumns.GetColumnNumber(col));
        }
        calibrationSet.XLoads[yn] = DataTableWrapper.ToRORowMatrix(table.DataColumns, sel, numberOfX);

        sel.Clear();
        for (int i = 0; i < numberOfFactors; i++)
        {
          string colname = GetYLoad_ColumnName(yn, i);
          col = table.DataColumns.TryGetColumn(colname);
          if (col is null)
            NotFound(colname);
          sel.Add(table.DataColumns.GetColumnNumber(col));
        }
        calibrationSet.YLoads[yn] = DataTableWrapper.ToRORowMatrix(table.DataColumns, sel, numberOfY);

        sel.Clear();
        col = table.DataColumns.TryGetColumn(GetCrossProduct_ColumnName(yn));
        if (col is null)
          NotFound(GetCrossProduct_ColumnName());
        sel.Add(table.DataColumns.GetColumnNumber(col));
        calibrationSet.CrossProduct[yn] = DataTableWrapper.ToRORowMatrix(table.DataColumns, sel, numberOfFactors);
      }
      calibrationSet.SetPreprocessingModel(preprocessSet);
    }
  }
}
