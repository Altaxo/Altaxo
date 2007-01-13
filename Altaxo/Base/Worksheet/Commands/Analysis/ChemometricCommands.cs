#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;

using Altaxo.Collections;
using Altaxo.Worksheet.GUI;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Multivariate;
using Altaxo.Data;
using Altaxo.Gui.Common;

using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Styles;

namespace Altaxo.Worksheet.Commands.Analysis
{
  /// <summary>
  /// Contain commands concerning chemometric operations like PLS and PCA.
  /// </summary>
  public class ChemometricCommands
  {
   
    #region MultiplyColumnsToMatrix
    public static void MultiplyColumnsToMatrix(WorksheetController ctrl)
    {
      string err = MultiplyColumnsToMatrix(Current.Project,ctrl.Doc,ctrl.SelectedDataColumns);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
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
    public static string MultiplyColumnsToMatrix(
      Altaxo.AltaxoDocument mainDocument,
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns
      )
    {
      // check that there are columns selected
      if(0==selectedColumns.Count)
        return "You must select at least two columns to multiply!";
      // selected columns must contain an even number of columns
      if(0!=selectedColumns.Count%2)
        return "You selected an odd number of columns. Please select an even number of columns to multiply!";
      // all selected columns must be numeric columns
      for(int i=0;i<selectedColumns.Count;i++)
      {
        if(!(srctable[selectedColumns[i]] is Altaxo.Data.INumericColumn))
          return string.Format("The column[{0}] (name:{1}) is not a numeric column!",selectedColumns[i],srctable[selectedColumns[i]].Name);
      }


      int halfselect = selectedColumns.Count/2;
    
      // check that all columns from the first half of selected colums contain the same
      // number of rows

      int rowsfirsthalf=int.MinValue;
      for(int i=0;i<halfselect;i++)
      {
        int idx = selectedColumns[i];
        if(rowsfirsthalf<0)
          rowsfirsthalf = srctable[idx].Count;
        else if(rowsfirsthalf != srctable[idx].Count)
          return "The first half of selected columns have not all the same length!";
      }

      int rowssecondhalf=int.MinValue;
      for(int i=halfselect;i<selectedColumns.Count;i++)
      {
        int idx = selectedColumns[i];
        if(rowssecondhalf<0)
          rowssecondhalf = srctable[idx].Count;
        else if(rowssecondhalf != srctable[idx].Count)
          return "The second half of selected columns have not all the same length!";
      }


      // now create the matrices to multiply from the 

      MatrixMath.REMatrix firstMat = new MatrixMath.REMatrix(rowsfirsthalf,halfselect);
      for(int i=0;i<halfselect;i++)
      {
        Altaxo.Data.INumericColumn col = (Altaxo.Data.INumericColumn)srctable[selectedColumns[i]];
        for(int j=0;j<rowsfirsthalf;j++)
          firstMat[j,i] = col[j];
      }
      
      MatrixMath.BEMatrix secondMat = new MatrixMath.BEMatrix(halfselect,rowssecondhalf);
      for(int i=0;i<halfselect;i++)
      {
        Altaxo.Data.INumericColumn col = (Altaxo.Data.INumericColumn)srctable[selectedColumns[i+halfselect]];
        for(int j=0;j<rowssecondhalf;j++)
          secondMat[i,j] = col[j];
      }

      // now multiply the two matrices
      MatrixMath.BEMatrix resultMat = new MatrixMath.BEMatrix(rowsfirsthalf,rowssecondhalf);
      MatrixMath.Multiply(firstMat,secondMat,resultMat);


      // and store the result in a new worksheet 
      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("ResultMatrix of " + srctable.Name);
      table.Suspend();

      // first store the factors
      for(int i=0;i<resultMat.Columns;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        for(int j=0;j<resultMat.Rows;j++)
          col[j] = resultMat[j,i];
        
        table.DataColumns.Add(col,i.ToString());
      }

      table.Resume();
      mainDocument.DataTableCollection.Add(table);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(table);

      return null;
    }

    #endregion

    #region PCA
    public static void PCAOnRows(WorksheetController ctrl)
    {
      int maxFactors = 3;
      IntegerValueInputController ivictrl = new IntegerValueInputController(maxFactors,"Please enter the maximum number of factors to calculate:");

      ivictrl.Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(Current.Gui.ShowDialog(ivictrl,"Set maximum number of factors",false))
      {
        string err= PrincipalComponentAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedDataColumns,ctrl.SelectedDataRows,true,ivictrl.EnteredContents);
        if(null!=err)
          Current.Gui.ErrorMessageBox(err);
      }
    }

    public static void PCAOnColumns(WorksheetController ctrl)
    {
      int maxFactors = 3;
      IntegerValueInputController ivictrl = new IntegerValueInputController(maxFactors,"Please enter the maximum number of factors to calculate:");
        

      ivictrl.Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(Current.Gui.ShowDialog(ivictrl,"Set maximum number of factors",false))
      {
        string err= PrincipalComponentAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedDataColumns,ctrl.SelectedDataRows,false,ivictrl.EnteredContents);
        if(null!=err)
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
    /// <returns></returns>
    public static string PrincipalComponentAnalysis(
      Altaxo.AltaxoDocument mainDocument,
      Altaxo.Data.DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      bool bHorizontalOrientedSpectrum,
      int maxNumberOfFactors
      )
    {
      bool bUseSelectedColumns = (null!=selectedColumns && 0!=selectedColumns.Count);
      int prenumcols = bUseSelectedColumns ? selectedColumns.Count : srctable.DataColumns.ColumnCount;
      
      // check for the number of numeric columns
      int numcols = 0;
      for(int i=0;i<prenumcols;i++)
      {
        int idx = bUseSelectedColumns ? selectedColumns[i] : i;
        if(srctable[i] is Altaxo.Data.INumericColumn)
          numcols++;
      }

      // check the number of rows
      bool bUseSelectedRows = (null!=selectedRows && 0!=selectedRows.Count);

      int numrows;
      if(bUseSelectedRows)
        numrows = selectedRows.Count;
      else
      {
        numrows = 0;
        for(int i=0;i<numcols;i++)
        {
          int idx = bUseSelectedColumns ? selectedColumns[i] : i;
          numrows = Math.Max(numrows,srctable[idx].Count);
        }     
      }

      // check that both dimensions are at least 2 - otherwise PCA is not possible
      if(numrows<2)
        return "At least two rows are neccessary to do Principal Component Analysis!";
      if(numcols<2)
        return "At least two numeric columns are neccessary to do Principal Component Analysis!";

      // Create a matrix of appropriate dimensions and fill it

      MatrixMath.BEMatrix matrixX;
      if(bHorizontalOrientedSpectrum)
      {
        matrixX = new MatrixMath.BEMatrix(numrows,numcols);
        int ccol = 0; // current column in the matrix
        for(int i=0;i<prenumcols;i++)
        {
          int colidx = bUseSelectedColumns ? selectedColumns[i] : i;
          Altaxo.Data.INumericColumn col = srctable[colidx] as Altaxo.Data.INumericColumn;
          if(null!=col)
          {
            for(int j=0;j<numrows;j++)
            {
              int rowidx = bUseSelectedRows ? selectedRows[j] : j;
              matrixX[j,ccol] = col[rowidx];
            }
            ++ccol;
          }
        }
      } // end if it was a horizontal oriented spectrum
      else // if it is a vertical oriented spectrum
      {
        matrixX = new MatrixMath.BEMatrix(numcols,numrows);
        int ccol = 0; // current column in the matrix
        for(int i=0;i<prenumcols;i++)
        {
          int colidx = bUseSelectedColumns ? selectedColumns[i] : i;
          Altaxo.Data.INumericColumn col = srctable[colidx] as Altaxo.Data.INumericColumn;
          if(null!=col)
          {
            for(int j=0;j<numrows;j++)
            {
              int rowidx = bUseSelectedRows ? selectedRows[j] : j;
              matrixX[ccol,j] = col[rowidx];
            }
            ++ccol;
          }
        }
      } // if it was a vertical oriented spectrum

      // now do PCA with the matrix
      MatrixMath.REMatrix factors = new MatrixMath.REMatrix(0,0);
      MatrixMath.BEMatrix loads = new MatrixMath.BEMatrix(0,0);
      MatrixMath.BEMatrix residualVariances = new MatrixMath.BEMatrix(0,0);
      MatrixMath.HorizontalVector meanX = new MatrixMath.HorizontalVector(matrixX.Columns);
      // first, center the matrix
      MatrixMath.ColumnsToZeroMean(matrixX,meanX);
      MatrixMath.NIPALS_HO(matrixX,maxNumberOfFactors,1E-9,factors,loads,residualVariances);

      // now we have to create a new table where to place the calculated factors and loads
      // we will do that in a vertical oriented manner, i.e. even if the loads are
      // here in horizontal vectors: in our table they are stored in (vertical) columns
      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("PCA of " + srctable.Name);

      // Fill the Table
      table.Suspend();

      // first of all store the meanscore
    {
      double meanScore = MatrixMath.LengthOf(meanX);
      MatrixMath.NormalizeRows(meanX);
    
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      for(int i=0;i<factors.Rows;i++)
        col[i] = meanScore;
      table.DataColumns.Add(col,"MeanFactor",Altaxo.Data.ColumnKind.V,0);
    }

      // first store the factors
      for(int i=0;i<factors.Columns;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        for(int j=0;j<factors.Rows;j++)
          col[j] = factors[j,i];
        
        table.DataColumns.Add(col,"Factor"+i.ToString(),Altaxo.Data.ColumnKind.V,1);
      }

      // now store the mean of the matrix
    {
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
      for(int j=0;j<meanX.Columns;j++)
        col[j] = meanX[0,j];
      table.DataColumns.Add(col,"MeanLoad",Altaxo.Data.ColumnKind.V,2);
    }

      // now store the loads - careful - they are horizontal in the matrix
      for(int i=0;i<loads.Rows;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        
        for(int j=0;j<loads.Columns;j++)
          col[j] = loads[i,j];
        
        table.DataColumns.Add(col,"Load"+i.ToString(),Altaxo.Data.ColumnKind.V,3);
      }

      // now store the residual variances, they are vertical in the vector
    {
      Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      
      for(int i=0;i<residualVariances.Rows;i++)
        col[i] = residualVariances[i,0];
      table.DataColumns.Add(col,"ResidualVariance",Altaxo.Data.ColumnKind.V,4);
    }

      table.Resume();
      mainDocument.DataTableCollection.Add(table);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(table);

      return null;
    }
    
    #endregion


    #region PLS Analysis

    public static void PLSOnRows(WorksheetController ctrl)
    {
      MultivariateAnalysisOptions options;
      SpectralPreprocessingOptions preprocessOptions;
      if(!QuestPLSAnalysisOptions(out options, out preprocessOptions))
        return;

      WorksheetAnalysis analysis = new PLS2WorksheetAnalysis();

      string err= analysis.ExecuteAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedDataColumns,ctrl.SelectedDataRows,ctrl.SelectedPropertyColumns,true,options,preprocessOptions);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }
    public static void PLSOnColumns(WorksheetController ctrl)
    {
      MultivariateAnalysisOptions options;
      SpectralPreprocessingOptions preprocessOptions;
      if(!QuestPLSAnalysisOptions(out options, out preprocessOptions))
        return;

      WorksheetAnalysis analysis = (WorksheetAnalysis)System.Activator.CreateInstance(options.AnalysisMethod);

      string err= analysis.ExecuteAnalysis(Current.Project,ctrl.Doc,ctrl.SelectedDataColumns,ctrl.SelectedDataRows,ctrl.SelectedPropertyColumns,false,options,preprocessOptions);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }


    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The spectra are presumed to be horizontally oriented, i.e. each spectrum is in one row.
    /// </summary>
    /// <param name="ctrl">The worksheet controller containing the selected data.</param>
    public static void PredictOnRows(WorksheetController ctrl)
    {
      PredictValues(ctrl,true);
    }
    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The spectra are presumed to be vertically oriented, i.e. each spectrum is in one column.
    /// </summary>
    /// <param name="ctrl">The worksheet controller containing the selected data.</param>
    public static void PredictOnColumns(WorksheetController ctrl)
    {
      PredictValues(ctrl,false);
    }

    
    /// <summary>
    /// This predicts the selected columns/rows against a user choosen calibration model.
    /// The orientation of spectra is given by the parameter <c>spectrumIsRow</c>.
    /// </summary>
    /// <param name="ctrl">The worksheet controller containing the selected data.</param>
    /// <param name="spectrumIsRow">If true, the spectra is horizontally oriented, else it is vertically oriented.</param>
    public static void PredictValues(WorksheetController ctrl, bool spectrumIsRow)
    {
      string modelName, destName;
      if(false==QuestCalibrationModelAndDestinationTable(out modelName, out destName) || null==modelName)
        return; // Cancelled by user

      Altaxo.Data.DataTable modelTable = Current.Project.DataTableCollection[modelName];
      Altaxo.Data.DataTable destTable  = (null==destName ? new Altaxo.Data.DataTable() : Current.Project.DataTableCollection[destName]);

      if(modelTable==null || destTable==null)
        throw new ApplicationException("Unexpected: modelTable or destTable is null");

      int numberOfFactors = 0;

      MultivariateContentMemento memento = modelTable.GetTableProperty("Content") as MultivariateContentMemento;

      if(memento!=null)
        numberOfFactors = memento.PreferredNumberOfFactors;

      if(numberOfFactors==0)
      {
        QuestPreferredNumberOfFactors(modelTable);
        memento = modelTable.GetTableProperty("Content") as MultivariateContentMemento;
        if(memento!=null) numberOfFactors = memento.PreferredNumberOfFactors;
      }

      memento.Analysis.PredictValues(
        ctrl.DataTable,
        ctrl.SelectedDataColumns,
        ctrl.SelectedDataRows,
        spectrumIsRow,
        numberOfFactors,
        modelTable,
        destTable);

     

      // if destTable is new, show it
      if(destTable.ParentObject==null)
      {
        Current.Project.DataTableCollection.Add(destTable);
        Current.ProjectService.OpenOrCreateWorksheetForTable(destTable);
      }

    }

   



    #endregion

    #region PLS Model Export

    public static void ExportPLSCalibration(Altaxo.Data.DataTable table)
    {
      // quest the number of factors to export
      IntegerValueInputController ivictrl = new IntegerValueInputController(1,"Please choose number of factors to export (>0):");
      ivictrl.Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(!Current.Gui.ShowDialog(ivictrl,"Number of factors",false))
        return;

    
      // quest the filename
      System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
      dlg.DefaultExt="xml";
      if(System.Windows.Forms.DialogResult.OK!=dlg.ShowDialog(Current.MainWindow))
        return;

      PredictionModelExporter exporter = new PredictionModelExporter(table,ivictrl.EnteredContents);
      exporter.Export(dlg.FileName);
    }

   
    #endregion

    #region PLS Retrieving original data

    public static string[] GetAvailablePLSCalibrationTables()
    {
      System.Collections.ArrayList result=new System.Collections.ArrayList();
      foreach(Altaxo.Data.DataTable table in Current.Project.DataTableCollection)
      {
        if(table.GetTableProperty("Content") is MultivariateContentMemento)
          result.Add(table.Name);
      }

      return (string[])result.ToArray(typeof(string));
    }



   

    #endregion

    #region PLS Calculating values

  

  

    

   


  
 
    #endregion

    #region PLS Plot Commands

    public  static WorksheetAnalysis GetAnalysis(DataTable table)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;

      if(plsMemo==null)
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
      options = new MultivariateAnalysisOptions();
      options.MaxNumberOfFactors =20;
      options.CrossPRESSCalculation = CrossPRESSCalculationType.ExcludeGroupsOfSimilarMeasurements;

      PLSStartAnalysisController ctrlAA = new PLSStartAnalysisController(options);
      PLSStartAnalysisControl    viewAA = new PLSStartAnalysisControl();
      ctrlAA.View = viewAA;

      preprocessOptions = new SpectralPreprocessingOptions();
      SpectralPreprocessingController  ctrlBB = new SpectralPreprocessingController(preprocessOptions);
      SpectralPreprocessingControl     viewBB = new SpectralPreprocessingControl();
      ctrlBB.View = viewBB;

      TabbedElementController tabController = new TabbedElementController();
      tabController.AddTab("Factors",ctrlAA,viewAA);
      tabController.AddTab("Preprocessing", ctrlBB, viewBB);
      if (Current.Gui.ShowDialog(tabController, "Enter analysis parameters", false))
      {
        options = ctrlAA.Doc;
        return true;
      }

      /*
      TabbedDialogController dialogctrl = new TabbedDialogController("PLS Analysis",false);
      dialogctrl.AddTab("Factors",ctrlAA,viewAA);
      dialogctrl.AddTab("Preprocessing",ctrlBB,viewBB);
      TabbedDialogView  dialogview = new TabbedDialogView();
      dialogctrl.View = dialogview;

      if(dialogctrl.ShowDialog(Current.MainWindow))
      {
        options = ctrlAA.Doc;

        return true;
      }
      */
      return false;
    }


    /// <summary>
    /// Ask the user (before a prediction is made) for the name of the calibration model table and the destination table.
    /// </summary>
    /// <param name="modelTableName">On return, contains the name of the table containing the calibration model.</param>
    /// <param name="destinationTableName">On return, contains the name of the destination table, or null if a new table should be used as destination.</param>
    /// <returns>True if OK, false if the users pressed Cancel.</returns>
    public static bool QuestCalibrationModelAndDestinationTable(out string modelTableName, out string destinationTableName)
    {
      Altaxo.Worksheet.GUI.PLSPredictValueController ctrl = new Altaxo.Worksheet.GUI.PLSPredictValueController();
      Altaxo.Worksheet.GUI.PLSPredictValueControl viewctrl = new PLSPredictValueControl();
      ctrl.View = viewctrl;

      DialogShellController dlgctrl = new DialogShellController(
        new DialogShellView(viewctrl),ctrl);

      if(dlgctrl.ShowDialog(Current.MainWindow))
      {
        modelTableName = ctrl.SelectedCalibrationTableName;
        destinationTableName = ctrl.SelectedDestinationTableName;
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
    /// <param name="xcol">The x column.</param>
    /// <param name="ycol">The y column.</param>
    /// <param name="labelcol">The label column.</param>
    public static void PlotOnlyLabel(XYPlotLayer layer, Altaxo.Data.DataColumn xcol, Altaxo.Data.DataColumn ycol, Altaxo.Data.DataColumn labelcol)  
    {
      XYColumnPlotData pa = new XYColumnPlotData(xcol,ycol);

      G2DPlotStyleCollection ps = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Empty);
      LabelPlotStyle labelStyle = new LabelPlotStyle(labelcol);
      labelStyle.FontSize = 10;
      labelStyle.BackgroundStyle = new FilledRectangle(System.Drawing.Color.LightCyan);
      ps.Add(labelStyle);
      
      layer.PlotItems.Add(new XYColumnPlotItem(pa,ps));
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
      string yrescolname = WorksheetAnalysis.GetYResidual_ColumnName(whichY,numberOfFactors);
      string yactcolname = WorksheetAnalysis.GetYOriginal_ColumnName(whichY);
 
      // Calculate the residual if not here
      if(table[yrescolname]==null)
      {
        GetAnalysis(table).CalculateYResidual(table, whichY, numberOfFactors);
      }

      PlotOnlyLabel(layer,table[yactcolname],table[yrescolname],table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Y original{0}",whichY);
      layer.DefaultYAxisTitleString   = string.Format("Y residual{0} (#factors:{1})",whichY,numberOfFactors);
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
      string yrescolname = WorksheetAnalysis.GetYCrossResidual_ColumnName(whichY,numberOfFactors);
      string yactcolname = WorksheetAnalysis.GetYOriginal_ColumnName(whichY);
 
      // Calculate the residual if not here
      if(table[yrescolname]==null)
      {
        GetAnalysis(table).CalculateCrossPredictedAndResidual(table, whichY, numberOfFactors,false,true,false);
      }

      PlotOnlyLabel(layer,table[yactcolname],table[yrescolname],table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Y original{0}",whichY);
      layer.DefaultYAxisTitleString   = string.Format("Y cross residual{0} (#factors:{1})",whichY,numberOfFactors);
    }

    /// <summary>
    /// Plots all preprocessed spectra into a newly created graph.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    public static void PlotPreprocessedSpectra(Altaxo.Data.DataTable table)
    {
      DataTable desttable = new DataTable();
      desttable.Name = table.Name+".PS";
      GetAnalysis(table).CalculatePreprocessedSpectra(table, desttable);
      Current.Project.DataTableCollection.Add(desttable);

      Worksheet.Commands.PlotCommands.PlotLine(desttable,new IntegerRangeAsCollection(1,desttable.DataColumnCount-1),true,false);
    }

    /// <summary>
    /// Plots all preprocessed spectra into a newly created graph.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    public static void PlotPredictionScores(Altaxo.Data.DataTable table)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if(plsMemo==null)
        return;
      if(plsMemo.PreferredNumberOfFactors<=0)
        QuestPreferredNumberOfFactors(plsMemo);

      GetAnalysis(table).CalculateAndStorePredictionScores(table, plsMemo.PreferredNumberOfFactors);
      
      AscendingIntegerCollection sel = new AscendingIntegerCollection();

      for(int i=0;i<plsMemo.NumberOfConcentrationData;i++)
      {
        string name = WorksheetAnalysis.GetPredictionScore_ColumnName(i,plsMemo.PreferredNumberOfFactors);
        if(null!=table[name])
          sel.Add(table.DataColumns.GetColumnNumber(table[name]));
      }

      Worksheet.Commands.PlotCommands.PlotLine(table,sel,true,false);
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
      string xresidualcolname = WorksheetAnalysis.GetXResidual_ColumnName(whichY,numberOfFactors);
      string yactcolname = WorksheetAnalysis.GetYOriginal_ColumnName(whichY);
      
      if(table[xresidualcolname]==null)
      {
        GetAnalysis(table).CalculateXResidual(table,whichY,numberOfFactors);
      }

      PlotOnlyLabel(layer,table[yactcolname],table[xresidualcolname],table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Y original{0}",whichY);
      layer.DefaultYAxisTitleString   = string.Format("X residual{0} (#factors:{1})",whichY,numberOfFactors);
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
      string xresidualcolname = WorksheetAnalysis.GetXCrossResidual_ColumnName(whichY,numberOfFactors);
      string yactcolname = WorksheetAnalysis.GetYOriginal_ColumnName(whichY);
      
      if(table[xresidualcolname]==null)
      {
        GetAnalysis(table).CalculateCrossPredictedAndResidual(table,whichY,numberOfFactors,false,false,true);
      }

      PlotOnlyLabel(layer,table[yactcolname],table[xresidualcolname],table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Y original{0}",whichY);
      layer.DefaultYAxisTitleString   = string.Format("X cross residual{0} (#factors:{1})",whichY,numberOfFactors);
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
      string ypredcolname = WorksheetAnalysis.GetYPredicted_ColumnName(whichY,numberOfFactors);
      string yactcolname = WorksheetAnalysis.GetYOriginal_ColumnName(whichY);
      if(table[ypredcolname]==null)
      {
        GetAnalysis(table).CalculateYPredicted(table,whichY,numberOfFactors);
      }

      PlotOnlyLabel(layer,table[yactcolname],table[ypredcolname],table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Y original{0}",whichY);
      layer.DefaultYAxisTitleString   = string.Format("Y predicted{0} (#factors:{1})",whichY,numberOfFactors);
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
      string ypredcolname = WorksheetAnalysis.GetYCrossPredicted_ColumnName(whichY,numberOfFactors);
      string yactcolname = WorksheetAnalysis.GetYOriginal_ColumnName(whichY);
      if(table[ypredcolname]==null)
      {
        GetAnalysis(table).CalculateCrossPredictedAndResidual(table,whichY,numberOfFactors,true,false,false);
      }

      PlotOnlyLabel(layer,table[yactcolname],table[ypredcolname],table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Y original{0}",whichY);
      layer.DefaultYAxisTitleString   = string.Format("Y cross predicted{0} (#factors:{1})",whichY,numberOfFactors);
    }

    /// <summary>
    /// Asks the user for the preferred number of factors to use for calculation and plotting and stores that number in the 
    /// PLS content tag of the table.
    /// </summary>
    /// <param name="table">The table which contains the PLS model.</param>
    public static void QuestPreferredNumberOfFactors(Altaxo.Data.DataTable table)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if(plsMemo==null)
        return;

      QuestPreferredNumberOfFactors(plsMemo);
    }

    public static void QuestPreferredNumberOfFactors(MultivariateContentMemento plsMemo)
    {
      // quest the number of factors to export
      IntegerValueInputController ivictrl = new IntegerValueInputController(1,"Please choose preferred number of factors(>0):");
        

      ivictrl.Validator = new IntegerValueInputController.ZeroOrPositiveIntegerValidator();
      if(!Current.Gui.ShowDialog(ivictrl,"Number of factors",false))
        return;

      plsMemo.PreferredNumberOfFactors = ivictrl.EnteredContents;
    }

    /// <summary>
    /// Plots the rediduals of all y components invidually in a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotYResiduals(Altaxo.Data.DataTable table)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if(plsMemo==null)
        return;
      if(plsMemo.PreferredNumberOfFactors<=0)
        QuestPreferredNumberOfFactors(plsMemo);

      for(int nComponent=0;nComponent<plsMemo.NumberOfConcentrationData;nComponent++)
      {
        Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
        PlotYResiduals(table,graphctrl.Doc.Layers[0],nComponent,plsMemo.PreferredNumberOfFactors);
      }
    }


    /// <summary>
    /// Plots the rediduals from cross prediction of all y components invidually in a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotYCrossResiduals(Altaxo.Data.DataTable table)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if(plsMemo==null)
        return;
      if(plsMemo.PreferredNumberOfFactors<=0)
        QuestPreferredNumberOfFactors(plsMemo);

      for(int nComponent=0;nComponent<plsMemo.NumberOfConcentrationData;nComponent++)
      {
        Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
        PlotYCrossResiduals(table,graphctrl.Doc.Layers[0],nComponent,plsMemo.PreferredNumberOfFactors);
      }
    }

    /// <summary>
    /// Plots the prediction values of all y components invidually in a  graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotPredictedVersusActualY(Altaxo.Data.DataTable table)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if(plsMemo==null)
        return;
      if(plsMemo.PreferredNumberOfFactors<=0)
        QuestPreferredNumberOfFactors(plsMemo);

      for(int nComponent=0;nComponent<plsMemo.NumberOfConcentrationData;nComponent++)
      {
        Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
        PlotPredictedVersusActualY(table,graphctrl.Doc.Layers[0],nComponent,plsMemo.PreferredNumberOfFactors);
      }
    }

    /// <summary>
    /// Plots the cross prediction values of all y components invidually in a  graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotCrossPredictedVersusActualY(Altaxo.Data.DataTable table)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if(plsMemo==null)
        return;
      if(plsMemo.PreferredNumberOfFactors<=0)
        QuestPreferredNumberOfFactors(plsMemo);

      for(int nComponent=0;nComponent<plsMemo.NumberOfConcentrationData;nComponent++)
      {
        Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
        PlotCrossPredictedVersusActualY(table,graphctrl.Doc.Layers[0],nComponent,plsMemo.PreferredNumberOfFactors);
      }
    }

    /// <summary>
    /// Plots the x (spectral) residuals of all spectra invidually in a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotXResiduals(Altaxo.Data.DataTable table)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if(plsMemo==null)
        return;
      if(plsMemo.PreferredNumberOfFactors<=0)
        QuestPreferredNumberOfFactors(plsMemo);

      for(int nComponent=0;nComponent<plsMemo.NumberOfConcentrationData;nComponent++)
      {
        Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
        PlotXResiduals(table,graphctrl.Doc.Layers[0],nComponent,plsMemo.PreferredNumberOfFactors);
      }
    }

    /// <summary>
    /// Plots the x (spectral) residuals (of cross prediction) of all spectra invidually in a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotXCrossResiduals(Altaxo.Data.DataTable table)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if(plsMemo==null)
        return;
      if(plsMemo.PreferredNumberOfFactors<=0)
        QuestPreferredNumberOfFactors(plsMemo);

      for(int nComponent=0;nComponent<plsMemo.NumberOfConcentrationData;nComponent++)
      {
        Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
        PlotXResiduals(table,graphctrl.Doc.Layers[0],nComponent,plsMemo.PreferredNumberOfFactors);
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

      XYColumnPlotData pa = new XYColumnPlotData(xcol,ycol);
      G2DPlotStyleCollection ps = new G2DPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter);
      layer.PlotItems.Add(new XYColumnPlotItem(pa,ps));

      layer.DefaultXAxisTitleString = "Number of factors";
      layer.DefaultYAxisTitleString   = "PRESS value";
    }

    /// <summary>
    /// Plots the PRESS value into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    public static void PlotPRESSValue(Altaxo.Data.DataTable table)
    {
      Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
      PlotPRESSValue(table,graphctrl.Doc.Layers[0]);
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

      XYColumnPlotData pa = new XYColumnPlotData(xcol,ycol);
      G2DPlotStyleCollection ps = new G2DPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter);
      layer.PlotItems.Add(new XYColumnPlotItem(pa,ps));

      layer.DefaultXAxisTitleString = "Number of factors";
      layer.DefaultYAxisTitleString   = "Cross PRESS value";
    }

    /// <summary>
    /// Plots the cross PRESS value into a provided layer.
    /// </summary>
    /// <param name="table">The table of PLS output data.</param>
    public static void PlotCrossPRESSValue(Altaxo.Data.DataTable table)
    {
      Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
      PlotCrossPRESSValue(table,graphctrl.Doc.Layers[0]);
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
      
      if(table[ycolname]==null)
      {
        GetAnalysis(table).CalculateXLeverage(table,preferredNumberOfFactors);
      }

      PlotOnlyLabel(layer,table[xcolname],table[ycolname],table[WorksheetAnalysis.GetMeasurementLabel_ColumnName()]);

      layer.DefaultXAxisTitleString = string.Format("Measurement");
      layer.DefaultYAxisTitleString   = string.Format("Score leverage (#factors:{0})",preferredNumberOfFactors);
    }

    /// <summary>
    /// Plots the x (spectral) leverage into a graph.
    /// </summary>
    /// <param name="table">The table with the PLS model data.</param>
    public static void PlotXLeverage(Altaxo.Data.DataTable table)
    {
      MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
      if(plsMemo==null)
        return;
      if(plsMemo.PreferredNumberOfFactors<=0)
        QuestPreferredNumberOfFactors(plsMemo);

      Altaxo.Graph.GUI.IGraphController graphctrl = Current.ProjectService.CreateNewGraph();
      PlotXLeverage(table,graphctrl.Doc.Layers[0],plsMemo.PreferredNumberOfFactors);
    }


    #endregion
  
  }
}
