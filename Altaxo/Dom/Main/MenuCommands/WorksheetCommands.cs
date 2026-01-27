#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

#nullable disable warnings
using System;
using Altaxo.Data;
using Altaxo.Data.Selections;
using Altaxo.Gui;
using Altaxo.Gui.Data.Sorting;
using Altaxo.Gui.Scripting;
using Altaxo.Gui.Workbench;
using Altaxo.Scripting;
using Altaxo.Serialization;
using Altaxo.Serialization.BrukerOpus;
using Altaxo.Serialization.Galactic;
using Altaxo.Serialization.HDF5.Nexus;
using Altaxo.Serialization.Jcamp;
using Altaxo.Serialization.Omnic;
using Altaxo.Serialization.Origin;
using Altaxo.Serialization.PrincetonInstruments;
using Altaxo.Serialization.WITec;

namespace Altaxo.Worksheet.Commands
{
  #region File commands

  public class SaveAs : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      ctrl.WorksheetLayout.ShowSaveAsDialog(false);
    }
  }

  public class SaveAsTemplate : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      ctrl.WorksheetLayout.ShowSaveAsDialog(true);
    }
  }

  public class ImportAscii : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.FileCommands.ShowImportAsciiDialog(ctrl.DataTable);
    }
  }

  public class ImportAsciiInSingleWorksheetHorizontally : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.FileCommands.ShowImportAsciiDialog(ctrl.DataTable, false, false);
    }
  }

  public class ImportAsciiInSingleWorksheetVertically : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.FileCommands.ShowImportAsciiDialog(ctrl.DataTable, false, true);
    }
  }

  public class ImportDatabase : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.DatabaseCommands.ShowImportDatabaseDialog(ctrl.DataTable);
    }
  }

  public class ImportAnyDataFile : SimpleCommand
  {
    public override bool CanExecute(object? parameter)
    {
      return true;
    }

    public override void Execute(object? parameter)
    {
      if (!(parameter is IViewContent activeViewContent))
        activeViewContent = Current.Workbench.ActiveViewContent;

      DataFileImporterBase.ImportShowReducedDialogsForAllTypes(activeViewContent);
    }
  }


  public class ImportDataFileCommandBase<TImporter> : SimpleCommand where TImporter : IDataFileImporter, new()
  {
    public override bool CanExecute(object? parameter)
    {
      return true;
    }

    public override void Execute(object? parameter)
    {
      if (!(parameter is IViewContent activeViewContent))
        activeViewContent = Current.Workbench.ActiveViewContent;

      DataFileImporterBase.ImportShowDialogs(activeViewContent, new TImporter());
    }
  }

  public class ImportGalacticSPC : ImportDataFileCommandBase<GalacticSPCImporter>
  {
  }

  public class ImportJcamp : ImportDataFileCommandBase<JcampImporter>
  {
  }

  public class ImportRenishawWdf : ImportDataFileCommandBase<Altaxo.Serialization.Renishaw.RenishawImporter>
  {
  }

  public class ImportNexus : ImportDataFileCommandBase<NexusImporter>
  {
  }


  public class ImportOmnicSPA : ImportDataFileCommandBase<OmnicSPAImporter>
  {
  }

  public class ImportOmnicSPG : ImportDataFileCommandBase<OmnicSPGImporter>
  {
  }

  public class ImportOrigin : ImportDataFileCommandBase<OriginImporter>
  {
  }

  public class ImportPrincetonInstrumentsSPE : ImportDataFileCommandBase<PrincetonInstrumentsSPEImporter>
  {
  }

  public class ImportBrukerOpus : ImportDataFileCommandBase<BrukerOpusImporter>
  {
  }

  public class ImportWiTec : ImportDataFileCommandBase<WITecImporter>
  {
  }

  public class ImportImage : ImportDataFileCommandBase<Altaxo.Serialization.Bitmaps.BitmapImporter>
  {
  }

  public class ImportRamanCHADA : ImportDataFileCommandBase<Altaxo.Serialization.HDF5.Chada.ChadaImporter>
  {
  }

  public class ExportRamanCHADA : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Serialization.HDF5.Chada.ChadaExport.ShowExportRamanChadaDialog(ctrl.DataTable);
    }
  }

  public class ExportAscii : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.FileCommands.ShowExportAsciiDialog(ctrl.DataTable);
    }
  }

  public class ExportGalacticSPC : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.FileCommands.ShowExportGalacticSPCDialog(ctrl.DataTable, ctrl.SelectedDataRows, ctrl.SelectedDataColumns);
    }
  }

  #endregion File commands

  #region Edit commands

  public class EditRemove : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.RemoveSelected(ctrl);
    }
  }

  public class RemoveAllButSelected : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.RemoveAllButSelected(ctrl);
    }
  }

  public class EditClean : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.CleanSelected(ctrl);
    }
  }

  public class EditCopy : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.CopyToClipboard(ctrl);
    }
  }

  public class EditPaste : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.PasteFromClipboard(ctrl);
    }
  }

  public class XYVToMatrix : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      string msg = Altaxo.Worksheet.Commands.EditCommands.XYVToMatrix(ctrl);
      if (msg is not null)
        Current.Gui.ErrorMessageBox(msg);
    }
  }

  public class TableShowProperties : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      ctrl.DataTable.ShowPropertyDialog();
    }
  }

  /// <summary>
  /// Opens a dialog to enter a row number,
  /// and then jumps to that row in the worksheet view.
  /// </summary>
  /// <seealso cref="Altaxo.Worksheet.Commands.AbstractWorksheetControllerCommand" />
  public class GotoTableRow : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.GotoRow(ctrl);
    }
  }

  /// <summary>
  /// Opens a dialog to select a value (text, numeric, or DateTime), then
  /// fills the selected cells or the entire table with it.
  /// </summary>
  /// <seealso cref="Altaxo.Worksheet.Commands.AbstractWorksheetControllerCommand" />
  public class FillWithValue : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.FillWithValue(ctrl);
    }
  }

  #endregion Edit commands

  #region Plot commands

  public class PlotLine : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLine(ctrl, true, false);
    }
  }

  public class PlotLineArea : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLineArea(ctrl);
    }
  }

  public class PlotLineStack : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLineStack(ctrl);
    }
  }

  public class PlotLineRelativeStack : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLineRelativeStack(ctrl);
    }
  }

  public class PlotLineWaterfall : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLineWaterfall(ctrl);
    }
  }

  public class PlotLinePolar : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLinePolar(ctrl);
    }
  }

  public class PlotScatter : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLine(ctrl, false, true);
    }
  }

  public class PlotLineAndScatter : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLine(ctrl, true, true);
    }
  }

  public class PlotBarChartNormal : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotBarChartNormal(ctrl);
    }
  }

  public class PlotBarChartStack : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotBarChartStack(ctrl);
    }
  }

  public class PlotBarChartRelativeStack : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotBarChartRelativeStack(ctrl);
    }
  }

  public class PlotColumnChartNormal : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotColumnChartNormal(ctrl);
    }
  }

  public class PlotColumnChartStack : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotColumnChartStack(ctrl);
    }
  }

  public class PlotColumnChartRelativeStack : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotColumnChartRelativeStack(ctrl);
    }
  }

  public class PlotDensityImage : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotDensityImage(ctrl);
    }
  }

  public class PlotDensityImageFromXYZ : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotDensityImageFromXYZ(ctrl);
    }
  }

  #endregion Plot commands

  #region Worksheet

  public class WorksheetRename : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.DataTableOtherActions.ShowRenameDialog(ctrl.DataTable);
    }
  }

  public class WorksheetMoveTo : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Gui.Pads.ProjectBrowser.ProjectBrowserExtensions.MoveDocuments(new[] { ctrl.DataTable });
    }
  }

  public class WorksheetDuplicate : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.Duplicate(ctrl);
    }
  }

  public class WorksheetTranspose : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.Transpose(ctrl);
    }
  }

  public class AddDataColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.AddDataColumns(ctrl);
    }
  }

  public class AddPropertyColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.AddPropertyColumns(ctrl);
    }
  }

  public class CreatePropertyColumnOfColumnNames : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.CreatePropertyColumnOfColumnNames(ctrl);
    }
  }

  public class WorksheetClearDataAll : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      using (var token = ctrl.DataTable.SuspendGetToken())
      {
        Altaxo.Worksheet.Commands.WorksheetCommands.WorksheetClearData(ctrl);
      }
    }
  }

  public class WorksheetClearDataOnly : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      using (var token = ctrl.DataTable.SuspendGetToken())
      {
        ctrl.DataTable.DataColumns.ClearData();
      }
    }
  }

  public class WorksheetRemoveDataColumnsOnly : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      using (var token = ctrl.DataTable.SuspendGetToken())
      {
        ctrl.DataTable.DataColumns.RemoveColumnsAll();
      }
    }
  }

  public class WorksheetRemovePropertyColumnsOnly : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      using (var token = ctrl.DataTable.SuspendGetToken())
      {
        ctrl.DataTable.PropertyColumns.RemoveColumnsAll();
      }
    }
  }

  public class WorksheetRemoveDataAndPropertyColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      using (var token = ctrl.DataTable.SuspendGetToken())
      {
        ctrl.DataTable.DataColumns.RemoveColumnsAll();
        ctrl.DataTable.PropertyColumns.RemoveColumnsAll();
      }
    }
  }
  public class WorksheetClearDataTableShowDialog : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      object optionsO = new DataTableCleaningOptions();
      if (true == Current.Gui.ShowDialog(ref optionsO, "Cleaning options"))
      {
        var options = (DataTableCleaningOptions)optionsO;
        options.ApplyTo(ctrl.DataTable);
      }
    }
  }

  public class DecomposeCyclingIndependentVariable : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      ctrl.DataTable.ShowExpandCyclingVariableColumnDialog(ctrl.SelectedDataRows, ctrl.SelectedDataColumns);
    }
  }

  public class DecomposeByColumnContent : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      ctrl.DataTable.ShowDecomposeByColumnContentDialog(ctrl.SelectedDataRows, ctrl.SelectedDataColumns);
    }
  }

  public class OpenExtractTableDataScriptDialog : AbstractWorksheetControllerCommand
  {
    private const string ExtractTableDataScriptPropertyName = "Scripts/ExtractTableData";
    private Altaxo.Data.DataTable m_Table;

    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      m_Table = ctrl.DataTable;
      var script = ctrl.DataTable.GetTableProperty(ExtractTableDataScriptPropertyName) as ExtractTableDataScript;

      if (script is null)
        script = new ExtractTableDataScript();

      object[] args = new object[] { script, new ScriptExecutionHandler(EhScriptExecution) };
      if (Current.Gui.ShowDialog(args, "WorksheetScript of " + m_Table.Name))
      {
        m_Table.SetTableProperty(ExtractTableDataScriptPropertyName, args[0]);
      }

      m_Table = null;
    }

    public bool EhScriptExecution(IScriptText script, IProgressReporter reporter)
    {
      return ((ExtractTableDataScript)script).Execute(m_Table);
    }
  }


  public class DataTablesAggregationCreation : AbstractWorksheetControllerCommand
  {
    private const string ExtractTableDataScriptPropertyName = "Scripts/ExtractTableData";
    private Altaxo.Data.DataTable m_Table;

    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      var srcTable = ctrl.DataTable;

      var newTableName = Current.Project.DataTableCollection.FindNewItemName(srcTable.Folder + srcTable.ShortName + "_Aggregated");
      var newTable = new DataTable(newTableName);
      Current.Project.DataTableCollection.Add(newTable);

      var ds = new DataTablesAggregationDataSource(new DataTablesAggregationProcessData([new DataTableProxy(srcTable)], new AllRows(), [], true, true), new DataTablesAggregationOptions(), new DataSourceImportOptions());
      newTable.DataSource = ds;

      Current.Gui.ShowDialog<DataTablesAggregationDataSource>(ref ds, "Aggregation of table data", true);

      Current.ProjectService.OpenOrCreateWorksheetForTable(newTable);
    }
  }


  public class OpenTableScriptDialog : AbstractWorksheetControllerCommand
  {
    private Altaxo.Data.DataTable _table;

    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      _table = ctrl.DataTable;

      var script = _table.TableScript ?? new TableScript();
      object[] args = new object[] { script, new Altaxo.Gui.Scripting.ScriptExecutionHandler(EhScriptExecution) };

      if (Current.Gui.ShowDialog(args, "WorksheetScript of " + _table.Name))
      {
        _table.TableScript = (TableScript)args[0];
      }

      _table = null;
    }

    public bool EhScriptExecution(IScriptText script, IProgressReporter reporter)
    {
      return ((TableScript)script).ExecuteWithSuspendedNotifications(_table, reporter);
    }
  }

  public class OpenFileImportScriptDialog : AbstractWorksheetControllerCommand
  {
    private Altaxo.Data.DataTable _table;

    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      _table = ctrl.DataTable;

      var script = _table.GetPropertyValue<FileImportScript>("Temp\\FileImportScript") ?? new FileImportScript();
      object[] args = new object[] { script, new Altaxo.Gui.Scripting.ScriptExecutionHandler(EhScriptExecution) };

      if (Current.Gui.ShowDialog(args, "File import script of " + _table.Name))
      {
        _table.PropertyBag.SetValue<FileImportScript>("Temp\\FileImportScript", (FileImportScript)args[0]);
      }

      _table = null;
    }

    public bool EhScriptExecution(IScriptText script, IProgressReporter reporter)
    {
      if (script is FileImportScript fileImportScript)
      {

        var dlg = new OpenFileOptions();
        foreach (var tuple in fileImportScript.FileFilters)
          dlg.AddFilter(tuple.Filter, tuple.Description);

        dlg.Multiselect = fileImportScript.CanAcceptMultipleFiles;
        if (Current.Gui.ShowOpenFileDialog(dlg))
        {
          _table.DataSource = new FileImportScriptDataSource(dlg.FileNames, fileImportScript);
          return fileImportScript.ExecuteWithSuspendedNotifications(_table, dlg.FileNames, reporter);
        }
      }
      return false;
    }
  }

  public class OpenProcessSourceTablesScriptDialog : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      var table = new DataTable();
      table.Name = Current.Project.DataTableCollection.FindNewItemNameInFolder(ctrl.DataTable.FolderName);
      Current.Project.DataTableCollection.Add(table);

      var script = new ProcessSourceTablesScript();
      var data = new ProcessSourceTablesScriptData(new[] { ("SourceTable", new DataTableProxy(ctrl.DataTable)) });
      var dataSource = new ProcessSourceTablesScriptDataSource(data, script, new DataSourceImportOptions());


      object[] args = new object[] { dataSource };

      if (Current.Gui.ShowDialog(args, "Process source tables script of " + table.Name))
      {
        table.DataSource = dataSource;
        table.UpdateTableFromTableDataSourceAsUserCancellable();
      }

      Current.ProjectService.OpenOrCreateWorksheetForTable(table);
    }
  }


  #endregion Worksheet

  #region Column commands

  public class SetColumnValues : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      if (ctrl.SelectedDataColumns.Count > 0)
        new OpenDataColumnScriptDialog().Run(ctrl); // Altaxo.Worksheet.Commands.ColumnCommands.SetColumnValues(ctrl);
      else
        new OpenPropertyColumnScriptDialog().Run(ctrl);
    }
  }

  public class OpenDataColumnScriptDialog : AbstractWorksheetControllerCommand
  {
    private Altaxo.Data.DataColumn m_Column;

    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.DataTable dataTable = ctrl.DataTable;
      if (ctrl.SelectedDataColumns.Count == 0)
        return;
      m_Column = dataTable.DataColumns[ctrl.SelectedDataColumns[0]];

      dataTable.DataColumns.ColumnScripts.TryGetValue(m_Column, out var script);
      if (script is null)
        script = new DataColumnScript();

      object[] args = new object[] { script, new ScriptExecutionHandler(EhScriptExecution) };
      if (Current.Gui.ShowDialog(args, "DataColumnScript of " + m_Column.Name))
      {
        dataTable.DataColumns.ColumnScripts[m_Column] = (IColumnScriptText)args[0];
      }
      m_Column = null;
    }

    public bool EhScriptExecution(IScriptText script, IProgressReporter reporter)
    {
      return ((DataColumnScript)script).ExecuteWithSuspendedNotifications(m_Column, reporter);
    }
  }

  public class OpenPropertyColumnScriptDialog : AbstractWorksheetControllerCommand
  {
    private Altaxo.Data.DataColumn m_Column;

    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.DataTable dataTable = ctrl.DataTable;
      if (ctrl.SelectedPropertyColumns.Count == 0)
        return;
      m_Column = dataTable.PropertyColumns[ctrl.SelectedPropertyColumns[0]];

      dataTable.PropertyColumns.ColumnScripts.TryGetValue(m_Column, out var script);
      if (script is null)
        script = new PropertyColumnScript();

      object[] args = new object[] { script, new ScriptExecutionHandler(EhScriptExecution) };
      if (Current.Gui.ShowDialog(args, "PropertyColumnScript of " + m_Column.Name))
      {
        dataTable.PropCols.ColumnScripts[m_Column] = (IColumnScriptText)args[0];
      }
      m_Column = null;
    }

    public bool EhScriptExecution(IScriptText script, IProgressReporter reporter)
    {
      return ((PropertyColumnScript)script).ExecuteWithSuspendedNotifications(m_Column, reporter);
    }
  }

  public class SetColumnAsX : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsX(ctrl);
    }
  }

  public class SetColumnAsY : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsY(ctrl);
    }
  }

  public class SetColumnAsLabel : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsLabel(ctrl);
    }
  }

  public class SetColumnAsValue : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsValue(ctrl);
    }
  }

  public class SetColumnAsError : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsKind(ctrl, Altaxo.Data.ColumnKind.Err);
    }
  }

  public class SetColumnAsPositiveError : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsKind(ctrl, Altaxo.Data.ColumnKind.pErr);
    }
  }

  public class SetColumnAsNegativeError : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsKind(ctrl, Altaxo.Data.ColumnKind.mErr);
    }
  }

  public class RenameColumn : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.RenameSelectedColumn(ctrl);
    }
  }

  public class SetColumnGroupNumber : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.ShowSetColumnGroupNumberDialog(ctrl);
    }
  }

  public class SetColumnPosition : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnPosition(ctrl);
    }
  }

  public class ExtractPropertyValues : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.ExtractPropertyValues(ctrl);
    }
  }

  public class SortTableAscending : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Sort(ctrl, true);
    }

    public static void Sort(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl, bool ascending)
    {
      if (ctrl.SelectedDataColumns.Count == 0 && ctrl.SelectedPropertyColumns.Count == 0)
      {
        Current.Gui.ErrorMessageBox("Please select either data columns or property columns for sorting.", "No column selected");
        return;
      }
      else if (ctrl.SelectedDataColumns.Count > 0 && ctrl.SelectedPropertyColumns.Count > 0)
      {
        Current.Gui.ErrorMessageBox("Please select either data columns or property columns for sorting, not both.", "Selection not appropriate");
        return;
      }
      else if (ctrl.SelectedDataColumns.Count == 1)
      {
        Altaxo.Data.Sorting.SortDataRows(ctrl.DataTable, ctrl.DataTable.DataColumns[ctrl.SelectedDataColumns[0]], ascending, treatEmptyElementsAsLowest: false);
      }
      else if (ctrl.SelectedDataColumns.Count > 1)
      {
        var model = new SortingDataColumnsModel() { SortingColumnsArePropertyColumns = false };
        foreach (int colIndex in ctrl.SelectedDataColumns)
        {
          model.ColumnsToSort.Add((ctrl.DataTable.DataColumns[colIndex], ascending));
        }
        var sortController = new SortingDataColumnsController();
        sortController.InitializeDocument(model);
        if (Current.Gui.ShowDialog(sortController, "Sort Data Columns"))
        {
          model = (SortingDataColumnsModel)sortController.ModelObject;
          Altaxo.Data.Sorting.SortRows(ctrl.DataTable.DataColumns, model.ColumnsToSort, treatEmptyElementsAsLowest: model.TreatEmptyElementsAsLowest);
        }
      }
      else if (ctrl.SelectedPropertyColumns.Count == 1)
      {
        Altaxo.Data.Sorting.SortDataColumnsByPropertyColumn(ctrl.DataTable, ctrl.DataTable.PropCols[ctrl.SelectedPropertyColumns[0]], ascending, treatEmptyElementsAsLowest: false);
      }
      else if (ctrl.SelectedPropertyColumns.Count > 1)
      {
        var model = new SortingDataColumnsModel() { SortingColumnsArePropertyColumns = true };
        foreach (int colIndex in ctrl.SelectedPropertyColumns)
        {
          model.ColumnsToSort.Add((ctrl.DataTable.PropertyColumns[colIndex], ascending));
        }

        var sortController = new SortingDataColumnsController();
        sortController.InitializeDocument(model);
        if (Current.Gui.ShowDialog(sortController, "Sort Data Columns"))
        {
          model = (SortingDataColumnsModel)sortController.ModelObject;
          Altaxo.Data.Sorting.SortDataColumnsByPropertyColumns(ctrl.DataTable, model.ColumnsToSort, model.MoveOnlyDataColumnsThatMatchPropertyColumnGroup, treatEmptyElementsAsLowest: model.TreatEmptyElementsAsLowest);
        }
      }
      else
      {
        throw new InvalidProgramException("This case shouldn'nt happen. Please debug!");
      }
    }
  }



  public class SortTableDescending : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      SortTableAscending.Sort(ctrl, false);
    }
  }

  #endregion Column commands

  #region Row commands

  public class SetRowPosition : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.RowCommands.SetSelectedRowPosition(ctrl);
    }
  }

  public class InsertOneDataRow : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.RowCommands.InsertOneDataRow(ctrl);
    }
  }

  public class InsertDataRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.RowCommands.InsertDataRows(ctrl);
    }
  }

  public class ChangeRowsToPropertyColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      ctrl.DataTable.ChangeRowsToPropertyColumns(ctrl.SelectedDataRows, ctrl.SelectedDataColumns);
      ctrl.ClearAllSelections();
    }
  }

  #endregion Row commands

  #region Property column commands

  public class SortColumnsByPropertyColumnAscending : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Sort(ctrl, true);
    }

    public static void Sort(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl, bool ascending)
    {
      Altaxo.Collections.IAscendingIntegerCollection selectedDataColumns = ctrl.SelectedDataColumns;
      if (0 == selectedDataColumns.Count)
        selectedDataColumns = Altaxo.Collections.ContiguousIntegerRange.FromStartAndCount(0, ctrl.DataTable.DataColumnCount);

      if (0 == ctrl.SelectedPropertyColumns.Count || 0 == selectedDataColumns.Count)
        return;

      Altaxo.Data.Sorting.SortDataColumnsByPropertyColumn(ctrl.DataTable, selectedDataColumns, ctrl.DataTable.PropCols[ctrl.SelectedPropertyColumns[0]], ascending, treatEmptyElementsAsLowest: false);
    }
  }

  public class SortColumnsByPropertyColumnDescending : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      SortColumnsByPropertyColumnAscending.Sort(ctrl, false);
    }
  }

  #endregion Property column commands

  #region Analysis

  public class EditTableDataSource : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.DataSourceCommands.ShowDataSourceEditor(ctrl);
    }
  }

  public class RequeryTableDataSource : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.DataSourceCommands.ExecuteDataSourceOfTableUserCancellable(ctrl);
    }
  }

  public class AnalysisFFT : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.FFT(ctrl);
    }
  }

  public class Analysis2DFFT : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.TwoDimensionalFFT(ctrl);
    }
  }

  public class Analysis2DCenteredFFT : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.TwoDimensionalCenteredFFT(ctrl);
    }
  }

  public class Convolute : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.Convolution(ctrl);
    }
  }

  public class Correlate : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.Correlation(ctrl);
    }
  }

  public class AnalysisStatisticsOnColumns : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.StatisticCommands.StatisticsOnColumns(ctrl);
    }
  }

  public class AnalysisStatisticsOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.StatisticCommands.StatisticsOnRows(ctrl);
    }
  }

  public class AnalysisStatisticsCreateHistogram : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.StatisticCommands.CreateHistogram(ctrl);
    }
  }

  public class AnalysisMultiplyColumnsToMatrix : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.MultiplyColumnsToMatrix(ctrl);
    }
  }

  public class AnalysisPCAOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PCAOnRows(ctrl);
    }
  }

  public class AnalysisPCAOnCols : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PCAOnColumns(ctrl);
    }
  }
  /*
  public class AnalysisPLSOnRows : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PLSOnRows(ctrl);
    }
  }
  */

  public class AnalysisPLSOnCols : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PLSOnColumns(ctrl);
    }
  }

  public class AnalysisPLSPredictOnCols : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PredictOnColumns(ctrl);
    }
  }

  public class AnalysisExportPLSCalibration : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.ExportPLSCalibration(ctrl.DataTable);
    }
  }

  public class AnalysisDifferentiateSmooth : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.SavitzkyGolayFiltering(ctrl);
    }
  }

  public class AnalysisInterpolation : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.Interpolation(ctrl);
    }
  }

  public class AnalysisMultivariateLinearRegression : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.MultivariateLinearFit(ctrl);
    }
  }
  public class AnalysisPronyRelaxationTimeDomain : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.PronyRelaxationTimeDomain(ctrl);
    }
  }

  public class AnalysisPronyRelaxationFrequencyDomain : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.PronyRelaxationFrequencyDomain(ctrl);
    }
  }

  public class AnalysisPronyRetardationTimeDomain : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.PronyRetardationTimeDomain(ctrl);
    }
  }

  public class AnalysisPronyRetardationFrequencyDomain : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.PronyRetardationFrequencyDomain(ctrl);
    }
  }

  public class AnalysisMasterCurveCreation : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Science.Thermorheology.MasterCurves.MasterCurveCreation.CreateMasterCurveShowDialog(ctrl.DataTable, ctrl.SelectedDataColumns, ctrl.SelectedDataRows);
    }
  }

  public class SpectroscopyPreprocessing : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Science.Spectroscopy.SpectroscopyCommands.SpectralPreprocessingShowDialog(ctrl);
    }
  }

  public class SpectroscopyPeakFindingFitting : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Science.Spectroscopy.SpectroscopyCommands.SpectralPeakFindingFittingShowDialog(ctrl);
    }
  }

  public class SpectroscopyFitPeaksInMultipleSpectra
    : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Science.Spectroscopy.SpectroscopyCommands.PeakFindingFittingInMultipleSpectraShowDialog(ctrl);
    }
  }

  public class SpectroscopyRamanNeonCalibration : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Science.Spectroscopy.SpectroscopyCommands.Raman_CalibrateWithNeonSpectrum(ctrl);
    }
  }

  public class SpectroscopyRamanSiliconCalibration : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Science.Spectroscopy.SpectroscopyCommands.Raman_CalibrateWithSiliconSpectrum(ctrl);
    }
  }

  public class SpectroscopyYCalibration : AbstractWorksheetControllerCommand
  {
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Science.Spectroscopy.SpectroscopyCommands.CalibrateWithIntensitySpectrum(ctrl);
    }
  }

  #endregion Analysis
}
