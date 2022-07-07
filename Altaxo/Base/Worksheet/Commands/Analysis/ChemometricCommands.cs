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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Multivariate;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui;
using Altaxo.Gui.Common;
using Altaxo.Gui.Worksheet;
using Altaxo.Gui.Worksheet.Viewing;

namespace Altaxo.Worksheet.Commands.Analysis
{
  /// <summary>
  /// Contain commands concerning chemometric operations like PLS and PCA.
  /// </summary>
  public class ChemometricCommands
  {
    #region MultiplyColumnsToMatrix

    public static void MultiplyColumnsToMatrix(IWorksheetController ctrl)
    {
      var err = MultiplyColumnsToMatrix(Current.Project, ctrl.DataTable, ctrl.SelectedDataColumns);
      if (!string.IsNullOrEmpty(err))
        Current.Gui.ErrorMessageBox(err, "An error occured");
    }

    /// <summary>
    /// Multiplies selected columns to form a matrix.
    /// </summary>
    /// <param name="mainDocument"></param>
    /// <param name="srctable"></param>
    /// <param name="selectedColumns"></param>
    /// <returns>Null if successful, else the description of the error.</returns>
    /// <remarks>The user must select an even number of columns. All columns of the first half of the selection
    /// must have the same number of rows, and all columns of the second half of selection must also have the same
    /// number of rows. The first half of selected columns form a matrix of dimensions(firstrowcount,halfselected), and the second half
    /// of selected columns form a matrix of dimension(halfselected, secondrowcount). The resulting matrix has dimensions (firstrowcount,secondrowcount) and is
    /// stored in a separate worksheet.</remarks>
    public static string? MultiplyColumnsToMatrix(
      Altaxo.AltaxoDocument mainDocument,
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns
      )
    {
      // check that there are columns selected
      if (0 == selectedColumns.Count)
        return "You must select at least two columns to multiply!";
      // selected columns must contain an even number of columns
      if (0 != selectedColumns.Count % 2)
        return "You selected an odd number of columns. Please select an even number of columns to multiply!";
      // all selected columns must be numeric columns
      for (int i = 0; i < selectedColumns.Count; i++)
      {
        if (!(srctable[selectedColumns[i]] is Altaxo.Data.INumericColumn))
          return string.Format("The column[{0}] (name:{1}) is not a numeric column!", selectedColumns[i], srctable[selectedColumns[i]].Name);
      }

      int halfselect = selectedColumns.Count / 2;

      // check that all columns from the first half of selected colums contain the same
      // number of rows

      int rowsfirsthalf = int.MinValue;
      for (int i = 0; i < halfselect; i++)
      {
        int idx = selectedColumns[i];
        if (rowsfirsthalf < 0)
          rowsfirsthalf = srctable[idx].Count;
        else if (rowsfirsthalf != srctable[idx].Count)
          return "The first half of selected columns have not all the same length!";
      }

      int rowssecondhalf = int.MinValue;
      for (int i = halfselect; i < selectedColumns.Count; i++)
      {
        int idx = selectedColumns[i];
        if (rowssecondhalf < 0)
          rowssecondhalf = srctable[idx].Count;
        else if (rowssecondhalf != srctable[idx].Count)
          return "The second half of selected columns have not all the same length!";
      }

      // now create the matrices to multiply from the

      var firstMat = new MatrixMath.TopSpineJaggedArrayMatrix<double>(rowsfirsthalf, halfselect);
      for (int i = 0; i < halfselect; i++)
      {
        var col = (Altaxo.Data.INumericColumn)srctable[selectedColumns[i]];
        for (int j = 0; j < rowsfirsthalf; j++)
          firstMat[j, i] = col[j];
      }

      var secondMat = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(halfselect, rowssecondhalf);
      for (int i = 0; i < halfselect; i++)
      {
        var col = (Altaxo.Data.INumericColumn)srctable[selectedColumns[i + halfselect]];
        for (int j = 0; j < rowssecondhalf; j++)
          secondMat[i, j] = col[j];
      }

      // now multiply the two matrices
      var resultMat = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(rowsfirsthalf, rowssecondhalf);
      MatrixMath.Multiply(firstMat, secondMat, resultMat);

      // and store the result in a new worksheet
      var table = new Altaxo.Data.DataTable("ResultMatrix of " + srctable.Name);
      using (var suspendToken = table.SuspendGetToken())
      {
        // first store the factors
        for (int i = 0; i < resultMat.ColumnCount; i++)
        {
          var col = new Altaxo.Data.DoubleColumn();
          for (int j = 0; j < resultMat.RowCount; j++)
            col[j] = resultMat[j, i];

          table.DataColumns.Add(col, i.ToString());
        }
        suspendToken.Dispose();
      }

      mainDocument.DataTableCollection.Add(table);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(table);

      return null;
    }

    #endregion MultiplyColumnsToMatrix

    #region PCA

    public static void PCAOnRows(IWorksheetController ctrl)
    {
      int maxFactors = 3;
      var ivictrl = new IntegerValueInputController(maxFactors, "Please enter the maximum number of factors to calculate:")
      {
        Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator()
      };
      if (Current.Gui.ShowDialog(ivictrl, "Set maximum number of factors", false))
      {
        var err = PrincipalComponentAnalysis(Current.Project, ctrl.DataTable, ctrl.SelectedDataColumns, ctrl.SelectedDataRows, true, ivictrl.EnteredContents);
        if (!string.IsNullOrEmpty(err))
          Current.Gui.ErrorMessageBox(err);
      }
    }

    public static void PCAOnColumns(IWorksheetController ctrl)
    {
      int maxFactors = 3;
      var ivictrl = new IntegerValueInputController(maxFactors, "Please enter the maximum number of factors to calculate:")
      {
        Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator()
      };
      if (Current.Gui.ShowDialog(ivictrl, "Set maximum number of factors", false))
      {
        var err = PrincipalComponentAnalysis(Current.Project, ctrl.DataTable, ctrl.SelectedDataColumns, ctrl.SelectedDataRows, false, ivictrl.EnteredContents);
        if (!string.IsNullOrEmpty(err))
          Current.Gui.ErrorMessageBox(err);
      }
    }

    /// <summary>
    /// Makes a PCA (a principal component analysis) of the table or the selected columns / rows and stores the results in a newly created table.
    /// </summary>
    /// <param name="mainDocument">The main document of the application.</param>
    /// <param name="srctable">The table where the data come from.</param>
    /// <param name="selectedColumns">The selected columns.</param>
    /// <param name="selectedRows">The selected rows.</param>
    /// <param name="bHorizontalOrientedSpectrum">True if a spectrum is a single row, False if a spectrum is a single column.</param>
    /// <param name="maxNumberOfFactors">The maximum number of factors to calculate.</param>
    /// <returns>Null if successfull; otherwise, an error message.</returns>
    public static string? PrincipalComponentAnalysis(
      Altaxo.AltaxoDocument mainDocument,
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection? selectedColumns,
      IAscendingIntegerCollection? selectedRows,
      bool bHorizontalOrientedSpectrum,
      int maxNumberOfFactors
      )
    {
      var usedSelectedColumns = selectedColumns is null || selectedColumns.Count == 0 ? null : selectedColumns;
      int prenumcols = usedSelectedColumns is null ? srctable.DataColumns.ColumnCount : usedSelectedColumns.Count;

      // check for the number of numeric columns
      int numcols = 0;
      for (int i = 0; i < prenumcols; i++)
      {
        int idx = usedSelectedColumns is null ? i : usedSelectedColumns[i];
        if (srctable[i] is Altaxo.Data.INumericColumn)
          numcols++;
      }

      // check the number of rows
      var usedSelectedRows = (selectedRows is null || 0 == selectedRows.Count) ? null : selectedRows;

      int numrows;
      if (!(usedSelectedRows is null))
      {
        numrows = usedSelectedRows.Count;
      }
      else
      {
        numrows = 0;
        for (int i = 0; i < numcols; i++)
        {
          int idx = usedSelectedColumns is null ? i : usedSelectedColumns[i];
          numrows = Math.Max(numrows, srctable[idx].Count);
        }
      }

      // check that both dimensions are at least 2 - otherwise PCA is not possible
      if (numrows < 2)
        return "At least two rows are neccessary to do Principal Component Analysis!";
      if (numcols < 2)
        return "At least two numeric columns are neccessary to do Principal Component Analysis!";

      // Create a matrix of appropriate dimensions and fill it

      MatrixMath.LeftSpineJaggedArrayMatrix<double> matrixX;
      if (bHorizontalOrientedSpectrum)
      {
        matrixX = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numrows, numcols);
        int ccol = 0; // current column in the matrix
        for (int i = 0; i < prenumcols; i++)
        {
          int colidx = usedSelectedColumns is null ? i : usedSelectedColumns[i];
          var col = srctable[colidx] as Altaxo.Data.INumericColumn;
          if (col is not null)
          {
            for (int j = 0; j < numrows; j++)
            {
              int rowidx = usedSelectedRows is null ? j : usedSelectedRows[j];
              matrixX[j, ccol] = col[rowidx];
            }
            ++ccol;
          }
        }
      } // end if it was a horizontal oriented spectrum
      else // if it is a vertical oriented spectrum
      {
        matrixX = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numcols, numrows);
        int ccol = 0; // current column in the matrix
        for (int i = 0; i < prenumcols; i++)
        {
          int colidx = usedSelectedColumns is null ? i : usedSelectedColumns[i];
          var col = srctable[colidx] as Altaxo.Data.INumericColumn;
          if (col is not null)
          {
            for (int j = 0; j < numrows; j++)
            {
              int rowidx = usedSelectedRows is null ? j : usedSelectedRows[j];
              matrixX[ccol, j] = col[rowidx];
            }
            ++ccol;
          }
        }
      } // if it was a vertical oriented spectrum

      // now do PCA with the matrix
      var factors = new MatrixMath.TopSpineJaggedArrayMatrix<double>(0, 0);
      var loads = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(0, 0);
      var residualVariances = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(0, 0);
      var meanX = new MatrixMath.MatrixWithOneRow<double>(matrixX.ColumnCount);
      // first, center the matrix
      MatrixMath.ColumnsToZeroMean(matrixX, meanX);
      MatrixMath.NIPALS_HO(matrixX, maxNumberOfFactors, 1E-9, factors, loads, residualVariances);

      // now we have to create a new table where to place the calculated factors and loads
      // we will do that in a vertical oriented manner, i.e. even if the loads are
      // here in horizontal vectors: in our table they are stored in (vertical) columns
      var table = new Altaxo.Data.DataTable("PCA of " + srctable.Name);

      // Fill the Table
      using (var suspendToken = table.SuspendGetToken())
      {
        // first of all store the meanscore
        {
          double meanScore = MatrixMath.LengthOf(meanX);
          MatrixMath.NormalizeRows(meanX);

          var col = new Altaxo.Data.DoubleColumn();
          for (int i = 0; i < factors.RowCount; i++)
            col[i] = meanScore;
          table.DataColumns.Add(col, "MeanFactor", Altaxo.Data.ColumnKind.V, 0);
        }

        // first store the factors
        for (int i = 0; i < factors.ColumnCount; i++)
        {
          var col = new Altaxo.Data.DoubleColumn();
          for (int j = 0; j < factors.RowCount; j++)
            col[j] = factors[j, i];

          table.DataColumns.Add(col, "Factor" + i.ToString(), Altaxo.Data.ColumnKind.V, 1);
        }

        // now store the mean of the matrix
        {
          var col = new Altaxo.Data.DoubleColumn();

          for (int j = 0; j < meanX.ColumnCount; j++)
            col[j] = meanX[0, j];
          table.DataColumns.Add(col, "MeanLoad", Altaxo.Data.ColumnKind.V, 2);
        }

        // now store the loads - careful - they are horizontal in the matrix
        for (int i = 0; i < loads.RowCount; i++)
        {
          var col = new Altaxo.Data.DoubleColumn();

          for (int j = 0; j < loads.ColumnCount; j++)
            col[j] = loads[i, j];

          table.DataColumns.Add(col, "Load" + i.ToString(), Altaxo.Data.ColumnKind.V, 3);
        }

        // now store the residual variances, they are vertical in the vector
        {
          var col = new Altaxo.Data.DoubleColumn();

          for (int i = 0; i < residualVariances.RowCount; i++)
            col[i] = residualVariances[i, 0];
          table.DataColumns.Add(col, "ResidualVariance", Altaxo.Data.ColumnKind.V, 4);
        }

        suspendToken.Dispose();
      }

      mainDocument.DataTableCollection.Add(table);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(table);

      return null;
    }

    #endregion PCA

    #region PLS Analysis

    public static void PLSOnRows(IWorksheetController ctrl)
    {
      if (!QuestPLSAnalysisOptions(out var options, out var preprocessOptions))
        return;

      WorksheetAnalysis analysis = new PLS2WorksheetAnalysis();

      var err = analysis.ExecuteAnalysis(Current.Project, ctrl.DataTable, ctrl.SelectedDataColumns, ctrl.SelectedDataRows, ctrl.SelectedPropertyColumns, true, options, preprocessOptions);
      if (!string.IsNullOrEmpty(err))
        Current.Gui.ErrorMessageBox(err, "An error occured");
    }

    public static void PLSOnColumns(IWorksheetController ctrl)
    {
      if (!QuestPLSAnalysisOptions(out var options, out var preprocessOptions))
        return;

      var analysis = (WorksheetAnalysis)(System.Activator.CreateInstance(options.AnalysisMethod) ?? throw new InvalidProgramException($"Unable to create instance of type {options.AnalysisMethod}. Is a constructor missing?"));

      var err = analysis.ExecuteAnalysis(Current.Project, ctrl.DataTable, ctrl.SelectedDataColumns, ctrl.SelectedDataRows, ctrl.SelectedPropertyColumns, false, options, preprocessOptions);
      if (err is not null)
        Current.Gui.ErrorMessageBox(err, "An error occured");
    }

    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The spectra are presumed to be horizontally oriented, i.e. each spectrum is in one row.
    /// </summary>
    /// <param name="ctrl">The worksheet controller containing the selected data.</param>
    public static void PredictOnRows(IWorksheetController ctrl)
    {
      PredictValues(ctrl, true);
    }

    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The spectra are presumed to be vertically oriented, i.e. each spectrum is in one column.
    /// </summary>
    /// <param name="ctrl">The worksheet controller containing the selected data.</param>
    public static void PredictOnColumns(IWorksheetController ctrl)
    {
      PredictValues(ctrl, false);
    }

    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The orientation of spectra is given by the parameter <c>spectrumIsRow</c>.
    /// </summary>
    /// <param name="ctrl">The worksheet controller containing the selected data.</param>
    /// <param name="spectrumIsRow">If true, the spectra is horizontally oriented, else it is vertically oriented.</param>
    public static void PredictValues(IWorksheetController ctrl, bool spectrumIsRow)
    {
      if (false == QuestCalibrationModelAndDestinationTable(out var modelName, out var destName) || modelName is null)
        return; // Cancelled by user

      Altaxo.Data.DataTable modelTable = Current.Project.DataTableCollection[modelName];
      Altaxo.Data.DataTable destTable = (destName is null ? new Altaxo.Data.DataTable() : Current.Project.DataTableCollection[destName]);

      if (modelTable is null)
        throw new ArgumentNullException(nameof(modelTable));
      if (destTable is null)
        throw new ArgumentNullException(nameof(destTable));

      int numberOfFactors = 0;

      var memento = modelTable.GetTableProperty("Content") as MultivariateContentMemento;

      if (memento is not null)
        numberOfFactors = memento.PreferredNumberOfFactors;

      if (numberOfFactors == 0)
      {
        QuestPreferredNumberOfFactors(modelTable);
        memento = modelTable.GetTableProperty("Content") as MultivariateContentMemento;
        if (memento is not null)
          numberOfFactors = memento.PreferredNumberOfFactors;
      }

      if (!(memento is null))
      {
        memento.Analysis.PredictValues(
          ctrl.DataTable,
          ctrl.SelectedDataColumns,
          ctrl.SelectedDataRows,
          spectrumIsRow,
          numberOfFactors,
          modelTable,
          destTable);

        // if destTable is new, show it
        if (destTable.ParentObject is null)
        {
          Current.Project.DataTableCollection.Add(destTable);
          Current.ProjectService.OpenOrCreateWorksheetForTable(destTable);
        }
      }
    }

    #endregion PLS Analysis

    #region PLS Model Export

    public static void ExportPLSCalibration(Altaxo.Data.DataTable table)
    {
      // quest the number of factors to export
      var ivictrl = new IntegerValueInputController(1, "Please choose number of factors to export (>0):")
      {
        Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator()
      };
      if (!Current.Gui.ShowDialog(ivictrl, "Number of factors", false))
        return;

      // quest the filename
      var options = new SaveFileOptions();
      options.AddFilter("*.xml", "Xml files (*.xml)");
      options.FilterIndex = 0;
      if (!Current.Gui.ShowSaveFileDialog(options))
        return;

      var exporter = new PredictionModelExporter(table, ivictrl.EnteredContents);
      exporter.Export(options.FileName);
    }

    #endregion PLS Model Export

    #region PLS Retrieving original data

    public static IEnumerable<DataTable> GetAvailablePLSCalibrationTables()
    {
      foreach (Altaxo.Data.DataTable table in Current.Project.DataTableCollection)
      {
        if (table.GetTableProperty("Content") is MultivariateContentMemento)
          yield return table;
      }
    }

    #endregion PLS Retrieving original data

    #region PLS Plot Commands

    public static WorksheetAnalysis GetAnalysis(DataTable table)
    {
      var plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;

      if (plsMemo is null)
        throw new ArgumentException("Table does not contain a PLSContentMemento");

      return plsMemo.Analysis;
    }

    /// <summary>
    /// Asks the user for the maximum number of factors and the cross validation calculation.
    /// </summary>
    /// <param name="options">The PLS options to ask for. On return, this is the user's choice.</param>
    /// <param name="preprocessOptions">The spectral preprocessing options to ask for (output).</param>
    /// <returns>True if the user has made his choice, false if the user pressed the Cancel button.</returns>
    public static bool QuestPLSAnalysisOptions(out MultivariateAnalysisOptions options, out SpectralPreprocessingOptions preprocessOptions)
    {
      options = new MultivariateAnalysisOptions
      {
        MaxNumberOfFactors = 20,
        CrossValidationGroupingStrategy = new CrossValidationGroupingStrategyExcludeGroupsOfSimilarMeasurements()
      };

      var ctrlAA = new PLSStartAnalysisController(options);
      Current.Gui.FindAndAttachControlTo(ctrlAA);

      preprocessOptions = new SpectralPreprocessingOptions();
      var ctrlBB = new SpectralPreprocessingController(preprocessOptions);
      Current.Gui.FindAndAttachControlTo(ctrlBB);

      var tabController = new TabbedElementController();
      tabController.AddTab("Factors", ctrlAA, ctrlAA.ViewObject);
      tabController.AddTab("Preprocessing", ctrlBB, ctrlBB.ViewObject);
      if (Current.Gui.ShowDialog(tabController, "Enter analysis parameters", false))
      {
        options = (MultivariateAnalysisOptions)ctrlAA.ModelObject;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Ask the user (before a prediction is made) for the name of the calibration model table and the destination table.
    /// </summary>
    /// <param name="modelTableName">On return, contains the name of the table containing the calibration model.</param>
    /// <param name="destinationTableName">On return, contains the name of the destination table, or null if a new table should be used as destination.</param>
    /// <returns>True if OK, false if the users pressed Cancel.</returns>
    public static bool QuestCalibrationModelAndDestinationTable([MaybeNullWhen(false)] out string modelTableName, out string? destinationTableName)
    {
      var ctrl = new PLSPredictValueController();

      if (Current.Gui.ShowDialog(ctrl, "Select model and calibration table"))
      {
        var (calibrationTable, destinationTable) = ((DataTable CalibrationTable, DataTable? DestinationTable))ctrl.ModelObject;
        modelTableName = calibrationTable.Name;
        destinationTableName = destinationTable?.Name;
        return true;
      }
      else
      {
        modelTableName = null;
        destinationTableName = null;
        return false;
      }
    }

    /// <summary>
    /// This plots a label plot into the provided layer.
    /// </summary>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="xcol">The x column.</param>
    /// <param name="ycol">The y column.</param>
    /// <param name="labelcol">The label column.</param>
    public static void PlotOnlyLabel(XYPlotLayer layer, Altaxo.Data.DataTable table, Altaxo.Data.DataColumn xcol, Altaxo.Data.DataColumn ycol, Altaxo.Data.DataColumn labelcol)
    {
      var context = layer.GetPropertyContext();

      var groupNumber = table.DataColumns.GetColumnGroup(ycol);
      var pa = new XYColumnPlotData(table, groupNumber, xcol, ycol);

      var ps = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Empty, layer.GetPropertyContext());
      var labelStyle = new LabelPlotStyle(labelcol, context)
      {
        BackgroundStyle = new FilledRectangle(NamedColors.LightCyan)
      };
      ps.Add(labelStyle);

      layer.PlotItems.Add(new XYColumnPlotItem(pa, ps));
    }

    /// <summary>
    /// Plots the residual y values of a given component into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="whichY">The number of the component (y, concentration etc.) for which to plot the residuals.</param>
    /// <param name="numberOfFactors">The number of factors used for calculation of the residuals.</param>
    public static void PlotYResiduals(Altaxo.Data.DataTable table, XYPlotLayer layer, int whichY, int numberOfFactors)
    {
      string yrescolname = WorksheetAnalysis.GetYResidual_ColumnName(whichY, numberOfFactors);
      string yactcolname = WorksheetAnalysis.GetYOriginal_ColumnName(whichY);

      // Calculate the residual if not here
      if (!table.ContainsDataColumn(yrescolname))
      {
        GetAnalysis(table).CalculateYResidual(table, whichY, numberOfFactors);
      }

      PlotOnlyLabel(layer, table, table[yactcolname], table[yrescolname], table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Y original{0}", whichY);
      layer.DefaultYAxisTitleString = string.Format("Y residual{0} (#factors:{1})", whichY, numberOfFactors);
    }

    /// <summary>
    /// Plots the residual y values (of cross prediction) of a given component into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="whichY">The number of the component (y, concentration etc.) for which to plot the residuals.</param>
    /// <param name="numberOfFactors">The number of factors used for calculation of the residuals.</param>
    public static void PlotYCrossResiduals(Altaxo.Data.DataTable table, XYPlotLayer layer, int whichY, int numberOfFactors)
    {
      string yrescolname = WorksheetAnalysis.GetYCrossResidual_ColumnName(whichY, numberOfFactors);
      string yactcolname = WorksheetAnalysis.GetYOriginal_ColumnName(whichY);

      // Calculate the residual if not here
      if (!table.ContainsDataColumn(yrescolname))
      {
        GetAnalysis(table).CalculateCrossPredictedAndResidual(table, whichY, numberOfFactors, false, true, false);
      }

      PlotOnlyLabel(layer, table, table[yactcolname], table[yrescolname], table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Y original{0}", whichY);
      layer.DefaultYAxisTitleString = string.Format("Y cross residual{0} (#factors:{1})", whichY, numberOfFactors);
    }

    /// <summary>
    /// Plots all preprocessed spectra into a newly created graph.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    public static void PlotPreprocessedSpectra(Altaxo.Data.DataTable table)
    {
      var desttable = new DataTable
      {
        Name = table.Name + ".PS"
      };
      GetAnalysis(table).CalculatePreprocessedSpectra(table, desttable);
      Current.Project.DataTableCollection.Add(desttable);

      string newName = string.Format("GPreprocSpectra");
      newName = Main.ProjectFolder.CreateFullName(table.Name, newName);

      Worksheet.Commands.PlotCommands.PlotLine(desttable, ContiguousIntegerRange.FromStartAndCount(1, desttable.DataColumnCount - 1), true, false, newName);
    }

    /// <summary>
    /// Plots all preprocessed spectra into a newly created graph.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    public static void PlotPredictionScores(Altaxo.Data.DataTable table)
    {
      var plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if (plsMemo is null)
        return;
      if (plsMemo.PreferredNumberOfFactors <= 0)
        QuestPreferredNumberOfFactors(plsMemo);

      GetAnalysis(table).CalculateAndStorePredictionScores(table, plsMemo.PreferredNumberOfFactors);

      var sel = new AscendingIntegerCollection();

      for (int i = 0; i < plsMemo.NumberOfConcentrationData; i++)
      {
        string name = WorksheetAnalysis.GetPredictionScore_ColumnName(i, plsMemo.PreferredNumberOfFactors);
        if (table.ContainsDataColumn(name))
          sel.Add(table.DataColumns.GetColumnNumber(table[name]));
      }

      string newName = string.Format("GPredScores#{0}F", plsMemo.PreferredNumberOfFactors);
      newName = Main.ProjectFolder.CreateFullName(table.Name, newName);
      Worksheet.Commands.PlotCommands.PlotLine(table, sel, true, false, newName);
    }

    /// <summary>
    /// Plots the x (spectral) residuals into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="whichY">The number of the component (y, concentration etc.) for which to plot the residuals.</param>
    /// <param name="numberOfFactors">The number of factors used for calculation of the residuals.</param>
    public static void PlotXResiduals(Altaxo.Data.DataTable table, XYPlotLayer layer, int whichY, int numberOfFactors)
    {
      string xresidualcolname = WorksheetAnalysis.GetXResidual_ColumnName(whichY, numberOfFactors);
      string yactcolname = WorksheetAnalysis.GetYOriginal_ColumnName(whichY);

      if (!table.ContainsDataColumn(xresidualcolname))
      {
        GetAnalysis(table).CalculateXResidual(table, whichY, numberOfFactors);
      }

      PlotOnlyLabel(layer, table, table[yactcolname], table[xresidualcolname], table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Y original{0}", whichY);
      layer.DefaultYAxisTitleString = string.Format("X residual{0} (#factors:{1})", whichY, numberOfFactors);
    }

    /// <summary>
    /// Plots the x (spectral) residuals (of cross prediction) into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="whichY">The number of the component (y, concentration etc.) for which to plot the residuals.</param>
    /// <param name="numberOfFactors">The number of factors used for calculation of the residuals.</param>
    public static void PlotXCrossResiduals(Altaxo.Data.DataTable table, XYPlotLayer layer, int whichY, int numberOfFactors)
    {
      string xresidualcolname = WorksheetAnalysis.GetXCrossResidual_ColumnName(whichY, numberOfFactors);
      string yactcolname = WorksheetAnalysis.GetYOriginal_ColumnName(whichY);

      if (!table.ContainsDataColumn(xresidualcolname))
      {
        GetAnalysis(table).CalculateCrossPredictedAndResidual(table, whichY, numberOfFactors, false, false, true);
      }

      PlotOnlyLabel(layer, table, table[yactcolname], table[xresidualcolname], table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Y original{0}", whichY);
      layer.DefaultYAxisTitleString = string.Format("X cross residual{0} (#factors:{1})", whichY, numberOfFactors);
    }

    /// <summary>
    /// Plots the predicted versus actual Y (concentration) into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="whichY">The number of the component (y, concentration etc.) for which to plot the residuals.</param>
    /// <param name="numberOfFactors">The number of factors used for calculation of the residuals.</param>
    public static void PlotPredictedVersusActualY(Altaxo.Data.DataTable table, XYPlotLayer layer, int whichY, int numberOfFactors)
    {
      string ypredcolname = WorksheetAnalysis.GetYPredicted_ColumnName(whichY, numberOfFactors);
      string yactcolname = WorksheetAnalysis.GetYOriginal_ColumnName(whichY);
      if (!table.ContainsDataColumn(ypredcolname))
      {
        GetAnalysis(table).CalculateYPredicted(table, whichY, numberOfFactors);
      }

      PlotOnlyLabel(layer, table, table[yactcolname], table[ypredcolname], table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Y original{0}", whichY);
      layer.DefaultYAxisTitleString = string.Format("Y predicted{0} (#factors:{1})", whichY, numberOfFactors);
    }

    /// <summary>
    /// Plots the cross predicted versus actual Y (concentration) into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="whichY">The number of the component (y, concentration etc.) for which to plot the residuals.</param>
    /// <param name="numberOfFactors">The number of factors used for calculation of the residuals.</param>
    public static void PlotCrossPredictedVersusActualY(Altaxo.Data.DataTable table, XYPlotLayer layer, int whichY, int numberOfFactors)
    {
      string ypredcolname = WorksheetAnalysis.GetYCrossPredicted_ColumnName(whichY, numberOfFactors);
      string yactcolname = WorksheetAnalysis.GetYOriginal_ColumnName(whichY);
      if (!table.ContainsDataColumn(ypredcolname))
      {
        GetAnalysis(table).CalculateCrossPredictedAndResidual(table, whichY, numberOfFactors, true, false, false);
      }

      PlotOnlyLabel(layer, table, table[yactcolname], table[ypredcolname], table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Y original{0}", whichY);
      layer.DefaultYAxisTitleString = string.Format("Y cross predicted{0} (#factors:{1})", whichY, numberOfFactors);
    }

    /// <summary>
    /// Asks the user for the preferred number of factors to use for calculation and plotting and stores that number in the
    /// PLS content tag of the table.
    /// </summary>
    /// <param name="table">The table which contains the PLS model.</param>
    public static void QuestPreferredNumberOfFactors(Altaxo.Data.DataTable table)
    {
      var plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if (plsMemo is null)
        return;

      QuestPreferredNumberOfFactors(plsMemo);
    }

    public static void QuestPreferredNumberOfFactors(MultivariateContentMemento plsMemo)
    {
      // quest the number of factors to export
      var ivictrl = new IntegerValueInputController(1, "Please choose preferred number of factors(>0):")
      {
        Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator()
      };
      if (!Current.Gui.ShowDialog(ivictrl, "Number of factors", false))
        return;

      plsMemo.PreferredNumberOfFactors = ivictrl.EnteredContents;
    }

    /// <summary>
    /// Plots the rediduals of all y components invidually in a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotYResiduals(Altaxo.Data.DataTable table)
    {
      var plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if (plsMemo is null)
        return;
      if (plsMemo.PreferredNumberOfFactors <= 0)
        QuestPreferredNumberOfFactors(plsMemo);

      for (int nComponent = 0; nComponent < plsMemo.NumberOfConcentrationData; nComponent++)
      {
        string newName = string.Format("GYResidualsC{0}#{1}F", nComponent, plsMemo.PreferredNumberOfFactors);
        var graphctrl = CreateNewGraphWithXYLayer(table.GetPropertyContext(), Main.ProjectFolder.CreateFullName(table.Name, newName), table.Name);
        PlotYResiduals(table, graphctrl.Doc.GetFirstXYPlotLayer(), nComponent, plsMemo.PreferredNumberOfFactors);
      }
    }

    /// <summary>
    /// Plots the rediduals from cross prediction of all y components invidually in a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotYCrossResiduals(Altaxo.Data.DataTable table)
    {
      var plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if (plsMemo is null)
        return;
      if (plsMemo.PreferredNumberOfFactors <= 0)
        QuestPreferredNumberOfFactors(plsMemo);

      for (int nComponent = 0; nComponent < plsMemo.NumberOfConcentrationData; nComponent++)
      {
        string newName = string.Format("GYCrossResidualsC{0}#{1}F", nComponent, plsMemo.PreferredNumberOfFactors);
        var graphctrl = CreateNewGraphWithXYLayer(table.GetPropertyContext(), Main.ProjectFolder.CreateFullName(table.Name, newName), table.Name);
        PlotYCrossResiduals(table, graphctrl.Doc.GetFirstXYPlotLayer(), nComponent, plsMemo.PreferredNumberOfFactors);
      }
    }

    /// <summary>
    /// Plots the prediction values of all y components invidually in a  graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotPredictedVersusActualY(Altaxo.Data.DataTable table)
    {
      var plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if (plsMemo is null)
        return;
      if (plsMemo.PreferredNumberOfFactors <= 0)
        QuestPreferredNumberOfFactors(plsMemo);

      for (int nComponent = 0; nComponent < plsMemo.NumberOfConcentrationData; nComponent++)
      {
        string newName = string.Format("GPredVsActC{0}#{1}F", nComponent, plsMemo.PreferredNumberOfFactors);
        var graphctrl = CreateNewGraphWithXYLayer(table.GetPropertyContext(), Main.ProjectFolder.CreateFullName(table.Name, newName), table.Name);
        PlotPredictedVersusActualY(table, graphctrl.Doc.GetFirstXYPlotLayer(), nComponent, plsMemo.PreferredNumberOfFactors);
      }
    }

    /// <summary>
    /// Plots the cross prediction values of all y components invidually in a  graph (without allowing a Gui to catch some errors).
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotCrossPredictedVersusActualY(Altaxo.Data.DataTable table)
    {
      PlotCrossPredictedVersusActualY(table, false);
    }

    /// <summary>
    /// Plots the cross prediction values of all y components invidually in a  graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    /// <param name="allowGuiForMessages">If <see langword="true"/> and an error occurs, an error message box is presented to the user.</param>
    public static void PlotCrossPredictedVersusActualY(Altaxo.Data.DataTable table, bool allowGuiForMessages)
    {
      var plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if (plsMemo is null)
      {
        string msg = string.Format("The table <<{0}>> does not seem to contain multivariate analysis data (content memento is missing)", table.Name);
        if (allowGuiForMessages)
          Current.Gui.ErrorMessageBox(msg);
        else
          throw new ApplicationException(msg);
        return;
      }

      while (string.IsNullOrEmpty(plsMemo.OriginalDataTableName) || !Current.Project.DataTableCollection.Contains(plsMemo.OriginalDataTableName))
      {
        string msg = string.Format("The table of the original spectral data <<{0}>> does not exist (renamed?)", plsMemo.OriginalDataTableName);
        if (allowGuiForMessages)
        {
          Current.Gui.ErrorMessageBox(msg);
          string newName = plsMemo.OriginalDataTableName ?? string.Empty;
          // TODO replace by a TableChoiceDialogBox
          if (Current.Gui.ShowDialog(ref newName, "Please enter the table name of original spectral data", false))
            plsMemo.OriginalDataTableName = newName;
        }
        else
          throw new WorksheetAnalysis.OriginalDataTableNotFoundException(msg);
      }

      if (plsMemo.PreferredNumberOfFactors <= 0)
        QuestPreferredNumberOfFactors(plsMemo);

      for (int nComponent = 0; nComponent < plsMemo.NumberOfConcentrationData; nComponent++)
      {
        string newName = string.Format("GCrossPredVsActC{0}#{1}F", nComponent, plsMemo.PreferredNumberOfFactors);
        var graphctrl = CreateNewGraphWithXYLayer(table.GetPropertyContext(), Main.ProjectFolder.CreateFullName(table.Name, newName), table.Name);
        PlotCrossPredictedVersusActualY(table, graphctrl.Doc.GetFirstXYPlotLayer(), nComponent, plsMemo.PreferredNumberOfFactors);
      }
    }

    /// <summary>
    /// Plots the x (spectral) residuals of all spectra invidually in a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotXResiduals(Altaxo.Data.DataTable table)
    {
      var plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if (plsMemo is null)
        return;
      if (plsMemo.PreferredNumberOfFactors <= 0)
        QuestPreferredNumberOfFactors(plsMemo);

      for (int nComponent = 0; nComponent < plsMemo.NumberOfConcentrationData; nComponent++)
      {
        string newName = string.Format("GXResidualsC{0}#{1}F", nComponent, plsMemo.PreferredNumberOfFactors);
        var graphctrl = CreateNewGraphWithXYLayer(table.GetPropertyContext(), Main.ProjectFolder.CreateFullName(table.Name, newName), table.Name);
        PlotXResiduals(table, graphctrl.Doc.GetFirstXYPlotLayer(), nComponent, plsMemo.PreferredNumberOfFactors);
      }
    }

    /// <summary>
    /// Plots the x (spectral) residuals (of cross prediction) of all spectra invidually in a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotXCrossResiduals(Altaxo.Data.DataTable table)
    {
      var plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if (plsMemo is null)
        return;
      if (plsMemo.PreferredNumberOfFactors <= 0)
        QuestPreferredNumberOfFactors(plsMemo);

      for (int nComponent = 0; nComponent < plsMemo.NumberOfConcentrationData; nComponent++)
      {
        string newName = string.Format("GXCrossResidualsC{0}#{1}F", nComponent, plsMemo.PreferredNumberOfFactors);
        var graphctrl = CreateNewGraphWithXYLayer(table.GetPropertyContext(), Main.ProjectFolder.CreateFullName(table.Name, newName), table.Name);
        PlotXResiduals(table, graphctrl.Doc.GetFirstXYPlotLayer(), nComponent, plsMemo.PreferredNumberOfFactors);
      }
    }

    /// <summary>
    /// Plots the PRESS value into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    public static void PlotPRESSValue(Altaxo.Data.DataTable table, XYPlotLayer layer)
    {
      Altaxo.Data.DataColumn ycol = table[WorksheetAnalysis.GetPRESSValue_ColumnName()];
      Altaxo.Data.DataColumn xcol = table[WorksheetAnalysis.GetNumberOfFactors_ColumnName()];
      var groupNumber = table.DataColumns.GetColumnGroup(ycol);

      var pa = new XYColumnPlotData(table, groupNumber, xcol, ycol);
      var ps = new G2DPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter, layer.GetPropertyContext());
      layer.PlotItems.Add(new XYColumnPlotItem(pa, ps));

      layer.DefaultXAxisTitleString = "Number of factors";
      layer.DefaultYAxisTitleString = "PRESS value";
    }

    /// <summary>
    /// Plots the PRESS value into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    public static void PlotPRESSValue(Altaxo.Data.DataTable table)
    {
      string newName = string.Format("GPRESS");
      var graphctrl = CreateNewGraphWithXYLayer(table.GetPropertyContext(), Main.ProjectFolder.CreateFullName(table.Name, newName), table.Name);
      PlotPRESSValue(table, graphctrl.Doc.GetFirstXYPlotLayer());
    }

    /// <summary>
    /// Plots the cross PRESS value into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    public static void PlotCrossPRESSValue(Altaxo.Data.DataTable table, XYPlotLayer layer)
    {
      Altaxo.Data.DataColumn ycol = table[WorksheetAnalysis.GetCrossPRESSValue_ColumnName()];
      Altaxo.Data.DataColumn xcol = table[WorksheetAnalysis.GetNumberOfFactors_ColumnName()];
      var groupNumber = table.DataColumns.GetColumnGroup(ycol);

      var pa = new XYColumnPlotData(table, groupNumber, xcol, ycol);
      var ps = new G2DPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter, layer.GetPropertyContext());
      layer.PlotItems.Add(new XYColumnPlotItem(pa, ps));

      layer.DefaultXAxisTitleString = "Number of factors";
      layer.DefaultYAxisTitleString = "Cross PRESS value";
    }

    /// <summary>
    /// Plots the cross PRESS value into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    public static void PlotCrossPRESSValue(Altaxo.Data.DataTable table)
    {
      string newName = string.Format("GCrossPRESS");
      var graphctrl = CreateNewGraphWithXYLayer(table.GetPropertyContext(), Main.ProjectFolder.CreateFullName(table.Name, newName), table.Name);
      PlotCrossPRESSValue(table, graphctrl.Doc.GetFirstXYPlotLayer());
    }

    public static Altaxo.Gui.Graph.Gdi.Viewing.IGraphController CreateNewGraphWithXYLayer(Main.Properties.IReadOnlyPropertyBag context, string preferredName, string anyNameInSameFolder)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(
        context,
        preferredName,
        anyNameInSameFolder,
        true);

      var graphctrl = Current.ProjectService.CreateNewGraph(graph);
      return graphctrl;
    }

    /// <summary>
    /// Plots the x (spectral) leverage into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    /// <param name="layer">The layer to plot into.</param>
    /// <param name="preferredNumberOfFactors">The number of factors used for leverage calculation.</param>
    public static void PlotXLeverage(Altaxo.Data.DataTable table, XYPlotLayer layer, int preferredNumberOfFactors)
    {
      string xcolname = WorksheetAnalysis.GetMeasurementLabel_ColumnName();
      string ycolname = WorksheetAnalysis.GetXLeverage_ColumnName(preferredNumberOfFactors);

      if (!table.ContainsDataColumn(ycolname))
      {
        GetAnalysis(table).CalculateXLeverage(table, preferredNumberOfFactors);
      }

      PlotOnlyLabel(layer, table, table[xcolname], table[ycolname], table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Measurement");
      layer.DefaultYAxisTitleString = string.Format("Score leverage (#factors:{0})", preferredNumberOfFactors);
    }

    /// <summary>
    /// Plots the x (spectral) leverage into a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotXLeverage(Altaxo.Data.DataTable table)
    {
      var plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if (plsMemo is null)
        return;
      if (plsMemo.PreferredNumberOfFactors <= 0)
        QuestPreferredNumberOfFactors(plsMemo);

      string newName = string.Format("GXLeverage#{0}F", plsMemo.PreferredNumberOfFactors);
      newName = Main.ProjectFolder.CreateFullName(table.Name, newName);

      var graphctrl = CreateNewGraphWithXYLayer(table.GetPropertyContext(), Main.ProjectFolder.CreateFullName(table.Name, newName), table.Name);
      PlotXLeverage(table, graphctrl.Doc.GetFirstXYPlotLayer(), plsMemo.PreferredNumberOfFactors);
    }

    #endregion PLS Plot Commands
  }
}
