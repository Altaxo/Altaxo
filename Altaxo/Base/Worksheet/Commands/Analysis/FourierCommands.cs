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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable enable
using System;
using Altaxo.Calc.Fourier;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Gui.Worksheet.Viewing;

namespace Altaxo.Worksheet.Commands.Analysis
{
  /// <summary>
  /// Contains commands concerning Fourier transformation, correlation and convolution.
  /// </summary>
  public class FourierCommands
  {
    public static void FFT(IWorksheetController dg)
    {
      int len = dg.SelectedDataColumns.Count;
      if (len == 0)
        return; // nothing selected

      if (!(dg.DataTable[dg.SelectedDataColumns[0]] is Altaxo.Data.DoubleColumn))
        return;

      var col = (Altaxo.Data.DoubleColumn)dg.DataTable[dg.SelectedDataColumns[0]];

      Altaxo.Data.AnalysisRealFourierTransformationCommands.ShowRealFourierTransformDialog(col);
    }

    #region Two dimensional Fourier transformation

    public static RealFourierTransformation2DOptions? _lastUsedOptions;

    public static void TwoDimensionalFFT(IWorksheetController ctrl)
    {
      ShowRealFourierTransformation2DDialog(ctrl.DataTable, ctrl.SelectedDataRows, ctrl.SelectedDataColumns, ctrl.SelectedPropertyColumns);
    }

    public static void TwoDimensionalCenteredFFT(IWorksheetController ctrl)
    {
      ShowRealFourierTransformation2DDialog(ctrl.DataTable, ctrl.SelectedDataRows, ctrl.SelectedDataColumns, ctrl.SelectedPropertyColumns);
    }

    /// <summary>
    /// Shows the dialog in which the user can select options for the 2D Fourier transformation, and then executes the Fourier transformation
    /// The result is stored in a newly created data table in the same folder as the source data table.
    /// </summary>
    /// <param name="table">The table containing the data to transform.</param>
    /// <param name="selectedDataRows">The selected data rows of the table. (A value of <c>null</c> can be provided here).</param>
    /// <param name="selectedDataColumns">The selected data columns of the table. (A value of <c>null</c> can be provided here).</param>
    /// <param name="selectedPropertyColumns">The selected property columns of the table. (A value of <c>null</c> can be provided here).</param>
    public static void ShowRealFourierTransformation2DDialog(DataTable table, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns, IAscendingIntegerCollection selectedPropertyColumns)
    {
      DataTableMatrixProxy? proxy = null;
      RealFourierTransformation2DOptions? options = null;

      try
      {
        proxy = new DataTableMatrixProxy(table, selectedDataRows, selectedDataColumns, selectedPropertyColumns);

        options = _lastUsedOptions is not null ? (RealFourierTransformation2DOptions)_lastUsedOptions.Clone() : new RealFourierTransformation2DOptions();
        proxy.TryGetRowHeaderIncrement(out var rowIncrementValue, out var rowIncrementMessage);
        proxy.TryGetColumnHeaderIncrement(out var columnIncrementValue, out var columnIncrementMessage);

        options.IsUserDefinedRowIncrementValue = false;
        options.RowIncrementValue = rowIncrementValue;
        options.RowIncrementMessage = rowIncrementMessage;

        options.IsUserDefinedColumnIncrementValue = false;
        options.ColumnIncrementValue = columnIncrementValue;
        options.ColumnIncrementMessage = columnIncrementMessage;
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(string.Format("{0}\r\nDetails:\r\n{1}", ex.Message, ex.ToString()), "Error in preparation of Fourier transformation");
        return;
      }

      if (!Current.Gui.ShowDialog(ref options, "Choose fourier transform options", false))
        return;

      _lastUsedOptions = options;

      try
      {
        var resultTable = new DataTable { Name = string.Format("{0}Fourier{1} of {2}", table.Folder, options.OutputKind, table.ShortName) };
        ExecuteFouriertransformation2D(proxy, options, resultTable);

        // Create a DataSource
        var dataSource = new FourierTransformation2DDataSource(proxy, options, new Altaxo.Data.DataSourceImportOptions());
        resultTable.DataSource = dataSource;

        Current.ProjectService.OpenOrCreateWorksheetForTable(resultTable);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(string.Format("{0}\r\nDetails:\r\n{1}", ex.Message, ex.ToString()), "Error in execution of Fourier transformation");
        return;
      }
    }

    /// <summary>
    /// Executes a two dimensional Fourier transformation.
    /// </summary>
    /// <param name="matrixProxy">The proxy containing the matrix data that should be transformed, as well as the row header column and column header column that can be used to determine the spacing between adjacent rows and columns.</param>
    /// <param name="options">The options for the Fourier transformation.</param>
    /// <param name="destinationTable">The destination table that is used to store the Fourier transformed values.</param>
    /// <exception cref="System.NotImplementedException">Data pretreatment has an order which is not implemented yet.</exception>
    public static void ExecuteFouriertransformation2D(DataTableMatrixProxy matrixProxy, RealFourierTransformation2DOptions options, DataTable destinationTable)
    {
      // preparation step

      if (!options.IsUserDefinedRowIncrementValue)
      {
        matrixProxy.TryGetRowHeaderIncrement(out var rowIncrementValue, out var rowIncrementMessage);
        options.RowIncrementValue = rowIncrementValue;
        options.RowIncrementMessage = rowIncrementMessage;
      }

      if (!options.IsUserDefinedColumnIncrementValue)
      {
        matrixProxy.TryGetColumnHeaderIncrement(out var columnIncrementValue, out var columnIncrementMessage);
        options.ColumnIncrementValue = columnIncrementValue;
        options.ColumnIncrementMessage = columnIncrementMessage;
      }

      var matrix = matrixProxy.GetMatrix((r, c) => new Altaxo.Calc.LinearAlgebra.DoubleMatrixInArray1DRowMajorRepresentation(r, c));

      var fft = new Altaxo.Calc.Fourier.RealFourierTransformation2D(matrix, true)
      {
        ColumnSpacing = options.RowIncrementValue,
        RowSpacing = options.ColumnIncrementValue
      };

      if (options.DataPretreatmentCorrectionOrder.HasValue)
      {
        switch (options.DataPretreatmentCorrectionOrder.Value)
        {
          case 0:
            fft.DataPretreatment += RealFourierTransformation2D.RemoveZeroOrderFromMatrixIgnoringInvalidElements;
            break;

          case 1:
            fft.DataPretreatment += RealFourierTransformation2D.RemoveFirstOrderFromMatrixIgnoringInvalidElements;
            break;

          case 2:
            fft.DataPretreatment += RealFourierTransformation2D.RemoveSecondOrderFromMatrixIgnoringInvalidElements;
            break;

          case 3:
            fft.DataPretreatment += RealFourierTransformation2D.RemoveThirdOrderFromMatrixIgnoringInvalidElements;
            break;

          default:
            throw new NotImplementedException(string.Format("Regression of order {0} is not implemented yet", options.DataPretreatmentCorrectionOrder.Value));
        }
      }

      if (options.ReplacementValueForNaNMatrixElements.HasValue)
        Altaxo.Calc.LinearAlgebra.MatrixMath.ReplaceNaNElementsWith(matrix, options.ReplacementValueForNaNMatrixElements.Value);
      if (options.ReplacementValueForInfiniteMatrixElements.HasValue)
        Altaxo.Calc.LinearAlgebra.MatrixMath.ReplaceNaNAndInfiniteElementsWith(matrix, options.ReplacementValueForInfiniteMatrixElements.Value);

      if (options.FourierWindow is not null)
      {
        fft.DataPretreatment += options.FourierWindow.Apply;
      }

      fft.Execute();

      IMatrix<double> resultMatrix;
      IROVector<double> rowFrequencies;
      IROVector<double> columnFrequencies;

      if (options.CenterResult)
        fft.GetResultCentered(options.ResultingFractionOfRowsUsed, options.ResultingFractionOfColumnsUsed, options.OutputKind, out resultMatrix, out rowFrequencies, out columnFrequencies);
      else
        fft.GetResult(options.ResultingFractionOfRowsUsed, options.ResultingFractionOfColumnsUsed, options.OutputKind, out resultMatrix, out rowFrequencies, out columnFrequencies);

      var matTableConverter = new Altaxo.Data.MatrixToDataTableConverter(resultMatrix, destinationTable);

      if (options.OutputFrequencyHeaderColumns)
      {
        matTableConverter.AddMatrixRowHeaderData(rowFrequencies, string.IsNullOrEmpty(options.FrequencyRowHeaderColumnName) ? "RowFrequencies" : options.FrequencyRowHeaderColumnName);
        matTableConverter.AddMatrixColumnHeaderData(columnFrequencies, string.IsNullOrEmpty(options.FrequencyColumnHeaderColumnName) ? "ColumnFrequencies" : options.FrequencyColumnHeaderColumnName);
      }

      if (options.OutputPeriodHeaderColumns)
      {
        matTableConverter.AddMatrixRowHeaderData(VectorMath.ToInverseROVector(rowFrequencies), string.IsNullOrEmpty(options.PeriodRowHeaderColumnName) ? "RowPeriods" : options.PeriodRowHeaderColumnName);
        matTableConverter.AddMatrixColumnHeaderData(VectorMath.ToInverseROVector(columnFrequencies), string.IsNullOrEmpty(options.PeriodColumnHeaderColumnName) ? "ColumnPeriods" : options.PeriodColumnHeaderColumnName);
      }

      matTableConverter.Execute();
    }

    #endregion Two dimensional Fourier transformation

    #region Convolution

    public static void Convolution(IWorksheetController ctrl)
    {
      string? err = Convolution(Current.Project, ctrl);
      if (!string.IsNullOrEmpty(err))
        Current.Gui.ErrorMessageBox(err);
    }

    public static string? Convolution(Altaxo.AltaxoDocument mainDocument, IWorksheetController dg)
    {
      int len = dg.SelectedDataColumns.Count;
      if (len == 0)
        return "No column selected!"; // nothing selected
      if (len > 2)
        return "Too many columns selected!";

      if (!(dg.DataTable[dg.SelectedDataColumns[0]] is Altaxo.Data.DoubleColumn))
        return "First selected column is not numeric!";

      if (dg.SelectedDataColumns.Count == 2 && !(dg.DataTable[dg.SelectedDataColumns[1]] is Altaxo.Data.DoubleColumn))
        return "Second selected column is not numeric!";

      double[] arr1 = ((Altaxo.Data.DoubleColumn)dg.DataTable[dg.SelectedDataColumns[0]]).Array;
      double[] arr2 = arr1;
      if (dg.SelectedDataColumns.Count == 2)
        arr2 = ((Altaxo.Data.DoubleColumn)dg.DataTable[dg.SelectedDataColumns[1]]).Array;

      double[] result = new double[arr1.Length + arr2.Length - 1];
      //Pfa235Convolution co = new Pfa235Convolution(arr1.Length);
      //co.Convolute(arr1, arr2, result, null, FourierDirection.Forward);

      Calc.Fourier.NativeFourierMethods.ConvolutionNonCyclic(arr1, arr2, result);

      var col = new Altaxo.Data.DoubleColumn
      {
        Array = result
      };

      dg.DataTable.DataColumns.Add(col, "Convolute");

      return null;
    }

    #endregion Convolution

    #region Correlation

    public static void Correlation(IWorksheetController ctrl)
    {
      var err = Correlation(Current.Project, ctrl);
      if (!string.IsNullOrEmpty(err))
        Current.Gui.ErrorMessageBox(err);
    }

    public static string? Correlation(Altaxo.AltaxoDocument mainDocument, IWorksheetController dg)
    {
      int len = dg.SelectedDataColumns.Count;
      if (len == 0)
        return "No column selected!"; // nothing selected
      if (len > 2)
        return "Too many columns selected!";

      if (!(dg.DataTable[dg.SelectedDataColumns[0]] is Altaxo.Data.DoubleColumn))
        return "First selected column is not numeric!";

      if (dg.SelectedDataColumns.Count == 2 && !(dg.DataTable[dg.SelectedDataColumns[1]] is Altaxo.Data.DoubleColumn))
        return "Second selected column is not numeric!";

      double[] arr1 = ((Altaxo.Data.DoubleColumn)dg.DataTable[dg.SelectedDataColumns[0]]).Array;
      double[] arr2 = arr1;
      if (dg.SelectedDataColumns.Count == 2)
        arr2 = ((Altaxo.Data.DoubleColumn)dg.DataTable[dg.SelectedDataColumns[1]]).Array;

      double[] result = new double[arr1.Length + arr2.Length - 1];
      //Pfa235Convolution co = new Pfa235Convolution(arr1.Length);
      //co.Convolute(arr1, arr2, result, null, FourierDirection.Forward);

      Calc.Fourier.NativeFourierMethods.CorrelationNonCyclic(arr1, arr2, result);

      var col = new Altaxo.Data.DoubleColumn
      {
        Array = result
      };

      dg.DataTable.DataColumns.Add(col, "Correlate");

      return null;
    }

    #endregion Correlation
  }
}
