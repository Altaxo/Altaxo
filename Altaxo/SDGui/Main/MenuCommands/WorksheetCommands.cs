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
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using Altaxo;
using Altaxo.Main;
using Altaxo.Worksheet;
using Altaxo.Worksheet.GUI;
using Altaxo.Gui.Scripting;
using Altaxo.Scripting;

using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo.Worksheet.Commands
{
  #region Abstract command

  /// <summary>
  /// Provides a abstract class for issuing commands that apply to worksheet controllers.
  /// </summary>
  public abstract class AbstractWorksheetControllerCommand : AbstractMenuCommand
  {
    /// <summary>
    /// Determines the currently active worksheet and issues the command to that worksheet by calling
    /// Run with the worksheet as a parameter.
    /// </summary>
    public override void Run()
    {
      Altaxo.Gui.SharpDevelop.SDWorksheetViewContent ctrl 
        = Current.Workbench.ActiveViewContent
        as Altaxo.Gui.SharpDevelop.SDWorksheetViewContent;

      if(null!=ctrl)
        Run(ctrl.Controller);
    }
  
    /// <summary>
    /// Override this function for adding own worksheet commands. You will get
    /// the worksheet controller in the parameter.
    /// </summary>
    /// <param name="ctrl">The worksheet controller this command is applied to.</param>
    public abstract void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl);
  }

  #endregion

  #region File commands

  public class SaveAs : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.SaveAs(ctrl,false);
    }
  }

  public class SaveAsTemplate : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.SaveAs(ctrl,true);
    }
  }

  public class ImportAscii : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.ImportAscii(ctrl);
    }
  }

  public class ImportAsciiInSingleWorksheet : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.ImportAscii(ctrl,false);
    }
  }

  public class ImportImage : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.ImportImage(ctrl);
    }
  }

  public class ImportGalacticSPC : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.ImportGalacticSPC(ctrl);
    }
  }

  public class ExportAscii : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.ExportAscii(ctrl);
    }
  }

  public class ExportGalacticSPC : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.FileCommands.ExportGalacticSPC(ctrl);
    }
  }

  #endregion

  #region Edit commands

  public class EditRemove : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.RemoveSelected(ctrl);
    }
  }
  public class EditClean : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.CleanSelected(ctrl);
    }
  }
  public class EditCopy : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.CopyToClipboard(ctrl);
    }
  }
  public class EditPaste : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.PasteFromClipboard(ctrl);
    }
  }

  public class XYVToMatrix : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      string msg = Altaxo.Worksheet.Commands.EditCommands.XYVToMatrix(ctrl);
      if (msg != null)
        Current.Gui.ErrorMessageBox(msg);
    }
  }

  #endregion

  #region Plot commands

  public class PlotLine : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLine(ctrl, true, false);
    }
  }
  public class PlotLineArea : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLineArea(ctrl);
    }
  }
  public class PlotLineStack : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLineStack(ctrl);
    }
  }
  public class PlotLineRelativeStack : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLineRelativeStack(ctrl);
    }
  }
  public class PlotLineWaterfall : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLineWaterfall(ctrl);
    }
  }
  public class PlotLinePolar : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLinePolar(ctrl);
    }
  }

  public class PlotScatter : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLine(ctrl, false, true);
    }
  }

  public class PlotLineAndScatter : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLine(ctrl, true, true);
    }
  }

  

  public class PlotBarChartNormal : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotBarChartNormal(ctrl);
    }
  }
  public class PlotBarChartStack : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotBarChartStack(ctrl);
    }
  }
  public class PlotBarChartRelativeStack : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotBarChartRelativeStack(ctrl);
    }
  }

  public class PlotColumnChartNormal : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotColumnChartNormal(ctrl);
    }
  }
  public class PlotColumnChartStack : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotColumnChartStack(ctrl);
    }
  }
  public class PlotColumnChartRelativeStack : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotColumnChartRelativeStack(ctrl);
    }
  }


  public class PlotDensityImage : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotDensityImage(ctrl, true, true);
    }
  }



  #endregion

  #region Worksheet

  public class WorksheetRename : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.Rename(ctrl);
    }
  }


  public class WorksheetDuplicate : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.Duplicate(ctrl);
    }
  }

  public class WorksheetTranspose : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.Transpose(ctrl);
    }
  }

  public class AddDataColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.AddDataColumns(ctrl);
    }
  }

  public class AddPropertyColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.AddPropertyColumns(ctrl);
    }
  }

  public class CreatePropertyColumnOfColumnNames : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.CreatePropertyColumnOfColumnNames(ctrl);
    }
  }

  public class OpenExtractTableDataScriptDialog : AbstractWorksheetControllerCommand
  {
    const string ExtractTableDataScriptPropertyName = "Scripts/ExtractTableData";
    Altaxo.Data.DataTable m_Table;

    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      m_Table = ctrl.DataTable;
      ExtractTableDataScript script = ctrl.DataTable.GetTableProperty(ExtractTableDataScriptPropertyName) as ExtractTableDataScript;

      if(script==null)
        script = new ExtractTableDataScript();

      object[] args = new object[]{script,new ScriptExecutionHandler(this.EhScriptExecution)};
      if(Current.Gui.ShowDialog(args, "WorksheetScript of " + m_Table.Name))
      {
        m_Table.SetTableProperty(ExtractTableDataScriptPropertyName, args[0]);
      }
    
      this.m_Table = null;
    }

    public bool EhScriptExecution(IScriptText script)
    {
      return ((ExtractTableDataScript)script).Execute(m_Table);
    }
  }


  public class OpenTableScriptDialog : AbstractWorksheetControllerCommand
  {
    Altaxo.Data.DataTable m_Table;

    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      m_Table = ctrl.DataTable;
      
      IScriptText script = m_Table.TableScript;

      if(script==null)
        script = new TableScript();

      object[] args = new object[]{script,new Altaxo.Gui.Scripting.ScriptExecutionHandler(this.EhScriptExecution)};
      if(Current.Gui.ShowDialog(args, "WorksheetScript of " + m_Table.Name))
      {
        m_Table.TableScript = (TableScript)args[0];
      }

      this.m_Table = null;
      
    }
    public bool EhScriptExecution(IScriptText script)
    {
      return ((TableScript)script).ExecuteWithSuspendedNotifications(m_Table);
    }
  }



  #endregion

  #region Column commands


  public class SetColumnValues : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      if(ctrl.SelectedDataColumns.Count>0)
        new OpenDataColumnScriptDialog().Run(ctrl); // Altaxo.Worksheet.Commands.ColumnCommands.SetColumnValues(ctrl);
      else
        new OpenPropertyColumnScriptDialog().Run(ctrl);
    }
  }


  public class OpenDataColumnScriptDialog : AbstractWorksheetControllerCommand
  {
    Altaxo.Data.DataColumn m_Column;

    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Data.DataTable dataTable = ctrl.DataTable;
      if(ctrl.SelectedDataColumns.Count==0)
        return;
      m_Column = dataTable.DataColumns[ctrl.SelectedDataColumns[0]];

      IScriptText script = (IScriptText)dataTable.DataColumns.ColumnScripts[m_Column];
      if(script==null)
        script = new DataColumnScript();

      object[] args = new object[]{script,new ScriptExecutionHandler(this.EhScriptExecution)};
      if(Current.Gui.ShowDialog(args, "DataColumnScript of " + m_Column.Name))
      {
        if(null != dataTable.DataColumns.ColumnScripts[m_Column])
          dataTable.DataColumns.ColumnScripts[m_Column] = (IColumnScriptText)args[0];
        else
          dataTable.DataColumns.ColumnScripts.Add(m_Column, args[0]);
      }
      this.m_Column = null;
    }
    public bool EhScriptExecution(IScriptText script)
    {
      return ((DataColumnScript)script).ExecuteWithSuspendedNotifications(m_Column);
    }
  }
  public class OpenPropertyColumnScriptDialog : AbstractWorksheetControllerCommand
  {
    Altaxo.Data.DataColumn m_Column;

    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Data.DataTable dataTable = ctrl.DataTable;
      if(ctrl.SelectedPropertyColumns.Count==0)
        return;
      m_Column = dataTable.PropertyColumns[ctrl.SelectedPropertyColumns[0]];

      IScriptText script = (IScriptText)dataTable.PropertyColumns.ColumnScripts[m_Column];
      if(script==null)
        script = new PropertyColumnScript();

      object[] args = new object[]{script,new ScriptExecutionHandler(this.EhScriptExecution)};
      if(Current.Gui.ShowDialog(args, "PropertyColumnScript of " + m_Column.Name))
      {
        if(null != dataTable.PropertyColumns.ColumnScripts[m_Column])
          dataTable.PropertyColumns.ColumnScripts[m_Column] = (IColumnScriptText)args[0];
        else
          dataTable.PropertyColumns.ColumnScripts.Add(m_Column, args[0]);
      }

      this.m_Column = null;
    }

    public bool EhScriptExecution(IScriptText script)
    {
      return ((PropertyColumnScript)script).ExecuteWithSuspendedNotifications(m_Column);
    }
  }



  public class SetColumnAsX : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsX(ctrl);
    }
  }

  public class SetColumnAsY : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsY(ctrl);
    }
  }

  public class SetColumnAsLabel : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsLabel(ctrl);
    }
  }

  public class SetColumnAsValue : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsValue(ctrl);
    }
  }

  public class SetColumnAsError : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsKind(ctrl,Altaxo.Data.ColumnKind.Err);
    }
  }

  public class SetColumnAsPositiveError : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsKind(ctrl, Altaxo.Data.ColumnKind.pErr);
    }
  }

  public class SetColumnAsNegativeError : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsKind(ctrl, Altaxo.Data.ColumnKind.mErr);
    }
  }


  public class RenameColumn : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.RenameSelectedColumn(ctrl);
    }
  }

  

  public class SetColumnGroupNumber : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnGroupNumber(ctrl);
    }
  }
  

  public class SetColumnPosition : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnPosition(ctrl);
    }
  }

  

 

  public class ExtractPropertyValues : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.ExtractPropertyValues(ctrl);
    }
  }

  #endregion

  #region Row commands

  public class SetRowPosition : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.RowCommands.SetSelectedRowPosition(ctrl);
    }
  }

  public class InsertOneDataRow : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.RowCommands.InsertOneDataRow(ctrl);
    }
  }

  public class InsertDataRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.RowCommands.InsertDataRows(ctrl);
    }
  }

  #endregion

  #region Analysis

  public class AnalysisFFT : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.FFT(ctrl);
    }
  }

  public class Analysis2DFFT : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.TwoDimensionalFFT(ctrl);
    }
  }

  public class Analysis2DCenteredFFT : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.TwoDimensionalCenteredFFT(ctrl);
    }
  }

  public class AnalysisStatisticsOnColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.StatisticCommands.StatisticsOnColumns(ctrl);
    }
  }


  public class AnalysisStatisticsOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.StatisticCommands.StatisticsOnRows(ctrl);
    }
  }

  public class AnalysisMultiplyColumnsToMatrix : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.MultiplyColumnsToMatrix(ctrl);
    }
  }

  public class AnalysisPCAOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PCAOnRows(ctrl);
    }
  }
  public class AnalysisPCAOnCols : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PLSOnColumns(ctrl);
    }
  }
  public class AnalysisPLSOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PLSOnRows(ctrl);
    }
  }
  public class AnalysisPLSOnCols : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PLSOnColumns(ctrl);
    }
  }
  public class AnalysisPLSPredictOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PredictOnRows(ctrl);
    }
  }
  public class AnalysisPLSPredictOnCols : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PredictOnColumns(ctrl);
    }
  }


  public class AnalysisExportPLSCalibration : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.ExportPLSCalibration(ctrl.DataTable);
    }
  }

  public class AnalysisDifferentiateSmooth : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.SavitzkyGolayFiltering(ctrl);
    }
  }

  public class AnalysisInterpolation : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.Interpolation(ctrl);
    }
  }

  public class AnalysisMultivariateLinearRegression : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.MultivariateLinearFit(ctrl);
    }
  }


  #endregion
}
