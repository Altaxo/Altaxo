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
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Data.Selections;
using Altaxo.Gui;
using Altaxo.Gui.Common;
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

  /// <summary>
  /// Saves the active worksheet.
  /// </summary>
  public class SaveAs : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      ctrl.WorksheetLayout.ShowSaveAsDialog(false);
    }
  }

  /// <summary>
  /// Saves the active worksheet as a template.
  /// </summary>
  public class SaveAsTemplate : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      ctrl.WorksheetLayout.ShowSaveAsDialog(true);
    }
  }

  /// <summary>
  /// Imports ASCII data into the active worksheet.
  /// </summary>
  public class ImportAscii : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.FileCommands.ShowImportAsciiDialog(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Imports ASCII data horizontally into a single worksheet.
  /// </summary>
  public class ImportAsciiInSingleWorksheetHorizontally : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.FileCommands.ShowImportAsciiDialog(ctrl.DataTable, false, false);
    }
  }

  /// <summary>
  /// Imports ASCII data vertically into a single worksheet.
  /// </summary>
  public class ImportAsciiInSingleWorksheetVertically : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.FileCommands.ShowImportAsciiDialog(ctrl.DataTable, false, true);
    }
  }

  /// <summary>
  /// Imports database data into the active worksheet.
  /// </summary>
  public class ImportDatabase : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.DatabaseCommands.ShowImportDatabaseDialog(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Opens a reduced import dialog for any supported data file type.
  /// </summary>
  public class ImportAnyDataFile : SimpleCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object? parameter)
    {
      return true;
    }

    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      if (!(parameter is IViewContent activeViewContent))
        activeViewContent = Current.Workbench.ActiveViewContent;

      DataFileImporterBase.ImportShowReducedDialogsForAllTypes(activeViewContent);
    }
  }

  /// <summary>
  /// Provides a generic base class for data-file import commands.
  /// </summary>
  /// <typeparam name="TImporter">The importer type.</typeparam>
  public class ImportDataFileCommandBase<TImporter> : SimpleCommand where TImporter : IDataFileImporter, new()
  {
    /// <inheritdoc/>
    public override bool CanExecute(object? parameter)
    {
      return true;
    }

    /// <inheritdoc/>
    public override void Execute(object? parameter)
    {
      if (!(parameter is IViewContent activeViewContent))
        activeViewContent = Current.Workbench.ActiveViewContent;

      DataFileImporterBase.ImportShowDialogs(activeViewContent, new TImporter());
    }
  }

  /// <summary>
  /// Imports Galactic SPC files.
  /// </summary>
  public class ImportGalacticSPC : ImportDataFileCommandBase<GalacticSPCImporter>
  {
  }

  /// <summary>
  /// Imports JCAMP files.
  /// </summary>
  public class ImportJcamp : ImportDataFileCommandBase<JcampImporter>
  {
  }

  /// <summary>
  /// Imports Renishaw WDF files.
  /// </summary>
  public class ImportRenishawWdf : ImportDataFileCommandBase<Altaxo.Serialization.Renishaw.RenishawImporter>
  {
  }

  /// <summary>
  /// Imports NeXus files.
  /// </summary>
  public class ImportNexus : ImportDataFileCommandBase<NexusImporter>
  {
  }


  /// <summary>
  /// Imports Omnic SPA files.
  /// </summary>
  public class ImportOmnicSPA : ImportDataFileCommandBase<OmnicSPAImporter>
  {
  }

  /// <summary>
  /// Imports Omnic SPG files.
  /// </summary>
  public class ImportOmnicSPG : ImportDataFileCommandBase<OmnicSPGImporter>
  {
  }

  /// <summary>
  /// Imports Origin files.
  /// </summary>
  public class ImportOrigin : ImportDataFileCommandBase<OriginImporter>
  {
  }

  /// <summary>
  /// Imports Princeton Instruments SPE files.
  /// </summary>
  public class ImportPrincetonInstrumentsSPE : ImportDataFileCommandBase<PrincetonInstrumentsSPEImporter>
  {
  }

  /// <summary>
  /// Imports Bruker OPUS files.
  /// </summary>
  public class ImportBrukerOpus : ImportDataFileCommandBase<BrukerOpusImporter>
  {
  }

  /// <summary>
  /// Imports WITec files.
  /// </summary>
  public class ImportWiTec : ImportDataFileCommandBase<WITecImporter>
  {
  }

  /// <summary>
  /// Imports image files.
  /// </summary>
  public class ImportImage : ImportDataFileCommandBase<Altaxo.Serialization.Bitmaps.BitmapImporter>
  {
  }

  /// <summary>
  /// Imports Raman CHADA files.
  /// </summary>
  public class ImportRamanCHADA : ImportDataFileCommandBase<Altaxo.Serialization.HDF5.Chada.ChadaImporter>
  {
  }

  /// <summary>
  /// Exports the active worksheet as a Raman CHADA file.
  /// </summary>
  public class ExportRamanCHADA : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Serialization.HDF5.Chada.ChadaExport.ShowExportRamanChadaDialog(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Exports the active worksheet as ASCII text.
  /// </summary>
  public class ExportAscii : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.FileCommands.ShowExportAsciiDialog(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Exports the active worksheet as a Galactic SPC file.
  /// </summary>
  public class ExportGalacticSPC : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.FileCommands.ShowExportGalacticSPCDialog(ctrl.DataTable, ctrl.SelectedDataRows, ctrl.SelectedDataColumns);
    }
  }

  #endregion File commands

  #region Edit commands

  /// <summary>
  /// Removes the selected worksheet content.
  /// </summary>
  public class EditRemove : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.RemoveSelected(ctrl);
    }
  }

  /// <summary>
  /// Removes all worksheet content except the current selection.
  /// </summary>
  public class RemoveAllButSelected : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.RemoveAllButSelected(ctrl);
    }
  }

  /// <summary>
  /// Cleans the selected worksheet content.
  /// </summary>
  public class EditClean : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.CleanSelected(ctrl);
    }
  }

  /// <summary>
  /// Copies the current worksheet selection.
  /// </summary>
  public class EditCopy : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.CopyToClipboard(ctrl);
    }
  }

  /// <summary>
  /// Pastes clipboard content into the active worksheet.
  /// </summary>
  public class EditPaste : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.EditCommands.PasteFromClipboard(ctrl);
    }
  }

  /// <summary>
  /// Converts selected X-Y-value data into a matrix.
  /// </summary>
  public class XYVToMatrix : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      string msg = Altaxo.Worksheet.Commands.EditCommands.XYVToMatrix(ctrl);
      if (msg is not null)
        Current.Gui.ErrorMessageBox(msg);
    }
  }

  /// <summary>
  /// Shows the properties of the active worksheet.
  /// </summary>
  public class TableShowProperties : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
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

  /// <summary>
  /// Plots the selected data as a line plot.
  /// </summary>
  public class PlotLine : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLine(ctrl, true, false);
    }
  }

  /// <summary>
  /// Plots the selected data as a line-area plot.
  /// </summary>
  public class PlotLineArea : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLineArea(ctrl);
    }
  }

  /// <summary>
  /// Plots the selected data as a stacked line plot.
  /// </summary>
  public class PlotLineStack : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLineStack(ctrl);
    }
  }

  /// <summary>
  /// Plots the selected data as a relative stacked line plot.
  /// </summary>
  public class PlotLineRelativeStack : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLineRelativeStack(ctrl);
    }
  }

  /// <summary>
  /// Plots the selected data as a waterfall plot.
  /// </summary>
  public class PlotLineWaterfall : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLineWaterfall(ctrl);
    }
  }

  /// <summary>
  /// Plots the selected data as a polar line plot.
  /// </summary>
  public class PlotLinePolar : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLinePolar(ctrl);
    }
  }

  /// <summary>
  /// Plots the selected data as a scatter plot.
  /// </summary>
  public class PlotScatter : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLine(ctrl, false, true);
    }
  }

  /// <summary>
  /// Plots the selected data as a combined line and scatter plot.
  /// </summary>
  public class PlotLineAndScatter : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotLine(ctrl, true, true);
    }
  }

  /// <summary>
  /// Plots the selected data as a normal bar chart.
  /// </summary>
  public class PlotBarChartNormal : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotBarChartNormal(ctrl);
    }
  }

  /// <summary>
  /// Plots the selected data as a stacked bar chart.
  /// </summary>
  public class PlotBarChartStack : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotBarChartStack(ctrl);
    }
  }

  /// <summary>
  /// Plots the selected data as a relative stacked bar chart.
  /// </summary>
  public class PlotBarChartRelativeStack : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotBarChartRelativeStack(ctrl);
    }
  }

  /// <summary>
  /// Plots the selected data as a normal column chart.
  /// </summary>
  public class PlotColumnChartNormal : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotColumnChartNormal(ctrl);
    }
  }

  /// <summary>
  /// Plots the selected data as a stacked column chart.
  /// </summary>
  public class PlotColumnChartStack : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotColumnChartStack(ctrl);
    }
  }

  /// <summary>
  /// Plots the selected data as a relative stacked column chart.
  /// </summary>
  public class PlotColumnChartRelativeStack : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotColumnChartRelativeStack(ctrl);
    }
  }

  /// <summary>
  /// Plots the selected data as a density image.
  /// </summary>
  public class PlotDensityImage : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotDensityImage(ctrl);
    }
  }

  /// <summary>
  /// Plots the selected XYZ data as a density image.
  /// </summary>
  public class PlotDensityImageFromXYZ : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.PlotCommands.PlotDensityImageFromXYZ(ctrl);
    }
  }

  #endregion Plot commands

  #region Worksheet

  /// <summary>
  /// Renames the active worksheet.
  /// </summary>
  public class WorksheetRename : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Data.DataTableOtherActions.ShowRenameDialog(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Moves the active worksheet to another project folder.
  /// </summary>
  public class WorksheetMoveTo : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Gui.Pads.ProjectBrowser.ProjectBrowserExtensions.MoveDocuments(new[] { ctrl.DataTable });
    }
  }

  /// <summary>
  /// Duplicates the active worksheet.
  /// </summary>
  public class WorksheetDuplicate : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.Duplicate(ctrl);
    }
  }

  /// <summary>
  /// Transposes the active worksheet.
  /// </summary>
  public class WorksheetTranspose : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.Transpose(ctrl);
    }
  }

  /// <summary>
  /// Adds data columns to the active worksheet.
  /// </summary>
  public class AddDataColumns : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.AddDataColumns(ctrl);
    }
  }

  /// <summary>
  /// Adds property columns to the active worksheet.
  /// </summary>
  public class AddPropertyColumns : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.AddPropertyColumns(ctrl);
    }
  }

  /// <summary>
  /// Creates a property column that contains the data column names.
  /// </summary>
  public class CreatePropertyColumnOfColumnNames : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.WorksheetCommands.CreatePropertyColumnOfColumnNames(ctrl);
    }
  }

  /// <summary>
  /// Clears all worksheet data.
  /// </summary>
  public class WorksheetClearDataAll : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      using (var token = ctrl.DataTable.SuspendGetToken())
      {
        Altaxo.Worksheet.Commands.WorksheetCommands.WorksheetClearData(ctrl);
      }
    }
  }

  /// <summary>
  /// Clears only the data values of the active worksheet.
  /// </summary>
  public class WorksheetClearDataOnly : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      using (var token = ctrl.DataTable.SuspendGetToken())
      {
        ctrl.DataTable.DataColumns.ClearData();
      }
    }
  }

  /// <summary>
  /// Removes all data columns from the active worksheet.
  /// </summary>
  public class WorksheetRemoveDataColumnsOnly : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      using (var token = ctrl.DataTable.SuspendGetToken())
      {
        ctrl.DataTable.DataColumns.RemoveColumnsAll();
      }
    }
  }

  /// <summary>
  /// Removes all property columns from the active worksheet.
  /// </summary>
  public class WorksheetRemovePropertyColumnsOnly : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      using (var token = ctrl.DataTable.SuspendGetToken())
      {
        ctrl.DataTable.PropertyColumns.RemoveColumnsAll();
      }
    }
  }

  /// <summary>
  /// Removes all data and property columns from the active worksheet.
  /// </summary>
  public class WorksheetRemoveDataAndPropertyColumns : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      using (var token = ctrl.DataTable.SuspendGetToken())
      {
        ctrl.DataTable.DataColumns.RemoveColumnsAll();
        ctrl.DataTable.PropertyColumns.RemoveColumnsAll();
      }
    }
  }
  /// <summary>
  /// Opens the worksheet cleaning dialog.
  /// </summary>
  public class WorksheetClearDataTableShowDialog : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
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

  /// <summary>
  /// Decomposes a cycling independent variable into separate columns.
  /// </summary>
  public class DecomposeCyclingIndependentVariable : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      ctrl.DataTable.ShowExpandCyclingVariableColumnDialog(ctrl.SelectedDataRows, ctrl.SelectedDataColumns);
    }
  }

  /// <summary>
  /// Decomposes selected data by column content.
  /// </summary>
  public class DecomposeByColumnContent : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      ctrl.DataTable.ShowDecomposeByColumnContentDialog(ctrl.SelectedDataRows, ctrl.SelectedDataColumns);
    }
  }

  /// <summary>
  /// Opens the extract-table-data script dialog.
  /// </summary>
  public class OpenExtractTableDataScriptDialog : AbstractWorksheetControllerCommand
  {
    private const string ExtractTableDataScriptPropertyName = "Scripts/ExtractTableData";
    private Altaxo.Data.DataTable m_Table;

    /// <inheritdoc/>
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

    /// <summary>
    /// Executes the extract-table-data script.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="reporter">The progress reporter.</param>
    /// <returns><see langword="true"/> if the script executed successfully; otherwise, <see langword="false"/>.</returns>
    public bool EhScriptExecution(IScriptText script, IProgressReporter reporter)
    {
      return ((ExtractTableDataScript)script).Execute(m_Table);
    }
  }


  /// <summary>
  /// Creates a table aggregation data source from the active worksheet.
  /// </summary>
  public class DataTablesAggregationCreation : AbstractWorksheetControllerCommand
  {
    private const string ExtractTableDataScriptPropertyName = "Scripts/ExtractTableData";
    private Altaxo.Data.DataTable m_Table;

    /// <inheritdoc/>
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


  /// <summary>
  /// Opens the worksheet table script dialog.
  /// </summary>
  public class OpenTableScriptDialog : AbstractWorksheetControllerCommand
  {
    private Altaxo.Data.DataTable _table;

    /// <inheritdoc/>
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

    /// <summary>
    /// Executes the worksheet table script.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="reporter">The progress reporter.</param>
    /// <returns><see langword="true"/> if the script executed successfully; otherwise, <see langword="false"/>.</returns>
    public bool EhScriptExecution(IScriptText script, IProgressReporter reporter)
    {
      return ((TableScript)script).ExecuteWithSuspendedNotifications(_table, reporter);
    }
  }

  /// <summary>
  /// Opens the file-import script dialog.
  /// </summary>
  public class OpenFileImportScriptDialog : AbstractWorksheetControllerCommand
  {
    private Altaxo.Data.DataTable _table;

    /// <inheritdoc/>
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

    /// <summary>
    /// Executes the file-import script.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="reporter">The progress reporter.</param>
    /// <returns><see langword="true"/> if the import was executed successfully; otherwise, <see langword="false"/>.</returns>
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

  /// <summary>
  /// Opens the process-source-tables script dialog.
  /// </summary>
  public class OpenProcessSourceTablesScriptDialog : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
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

  /// <summary>
  /// Sets values of the selected data or property column.
  /// </summary>
  public class SetColumnValues : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      if (ctrl.SelectedDataColumns.Count > 0)
        new OpenDataColumnScriptDialog().Run(ctrl); // Altaxo.Worksheet.Commands.ColumnCommands.SetColumnValues(ctrl);
      else
        new OpenPropertyColumnScriptDialog().Run(ctrl);
    }
  }

  /// <summary>
  /// Selects columns based on properties of the currently selected data column.
  /// </summary>
  public class SelectColumns : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      if (ctrl.SelectedDataColumns.Count != 1)
        Current.Gui.ErrorMessageBox("Please select exactly one data column or one property column to select by.", "Selection not appropriate");
      else
      {
        var dataTable = ctrl.DataTable;
        var dataColumns = dataTable.DataColumns;
        var selectedColumnNumber = ctrl.SelectedDataColumns[0];
        var selectedColumn = ctrl.DataTable.DataColumns[selectedColumnNumber];
        var selectedColumnKind = ctrl.DataTable.DataColumns.GetColumnKind(selectedColumn);
        var selectedColumnGroupNumber = ctrl.DataTable.DataColumns.GetColumnGroup(selectedColumn);
        var availableProperties = new List<string>();
        for (int i = 0; i < ctrl.DataTable.PropertyColumns.ColumnCount; i++)
        {
          if (!ctrl.DataTable.PropertyColumns[i].IsElementEmpty(selectedColumnNumber))
            availableProperties.Add(ctrl.DataTable.PropertyColumns.GetColumnName(i));
        }

        var document = new MultiChoiceList()
        {
          Description = "Select columns having the same:",
          ColumnNames = new string[] { "Description" },
          List = new SelectableListNodeList()
          {
            new SelectableListNode("Group number", 0, true),
            new SelectableListNode("Column kind", 1, true),
          },
        };
        document.List.AddRange(availableProperties.ConvertAll(p => new SelectableListNode(p, p, false)));

        if (Current.Gui.ShowDialog(ref document, "Select Columns", false))
        {
          bool isSameGroup = document.List[0].IsSelected;
          bool isSameKind = document.List[1].IsSelected;
          var listOfProperties = new List<string>();
          for (int i = 2; i < document.List.Count; i++)
          {
            if (document.List[i].IsSelected)
              listOfProperties.Add((string)document.List[i].Tag);
          }

          // now scan all columns and select those matching the criteria
          for (int idxColumn = 0; idxColumn < dataColumns.ColumnCount; ++idxColumn)
          {
            if (idxColumn == selectedColumnNumber)
              continue;

            var col = dataColumns[idxColumn];
            bool select = true;
            if (isSameGroup)
            {
              var groupNumber = dataColumns.GetColumnGroup(col);
              if (groupNumber != selectedColumnGroupNumber)
                select = false;
            }
            if (isSameKind)
            {
              var kind = dataColumns.GetColumnKind(col);
              if (kind != selectedColumnKind)
                select = false;
            }
            foreach (var propName in listOfProperties)
            {
              var propCol = dataTable.PropertyColumns[propName];
              if (propCol[idxColumn] != propCol[selectedColumnNumber])
              {
                select = false;
                break;
              }
            }
            if (select)
            {
              ctrl.SelectedDataColumns.Add(idxColumn);
            }
          }
          ctrl.TriggerRedrawing();
        }
      }
    }
  }

  /// <summary>
  /// Opens the data-column script dialog.
  /// </summary>
  public class OpenDataColumnScriptDialog : AbstractWorksheetControllerCommand
  {
    private Altaxo.Data.DataColumn m_Column;

    /// <inheritdoc/>
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

    /// <summary>
    /// Executes the data-column script.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="reporter">The progress reporter.</param>
    /// <returns><see langword="true"/> if the script executed successfully; otherwise, <see langword="false"/>.</returns>
    public bool EhScriptExecution(IScriptText script, IProgressReporter reporter)
    {
      return ((DataColumnScript)script).ExecuteWithSuspendedNotifications(m_Column, reporter);
    }
  }

  /// <summary>
  /// Opens the property-column script dialog.
  /// </summary>
  public class OpenPropertyColumnScriptDialog : AbstractWorksheetControllerCommand
  {
    private Altaxo.Data.DataColumn m_Column;

    /// <inheritdoc/>
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

    /// <summary>
    /// Executes the property-column script.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="reporter">The progress reporter.</param>
    /// <returns><see langword="true"/> if the script executed successfully; otherwise, <see langword="false"/>.</returns>
    public bool EhScriptExecution(IScriptText script, IProgressReporter reporter)
    {
      return ((PropertyColumnScript)script).ExecuteWithSuspendedNotifications(m_Column, reporter);
    }
  }

  /// <summary>
  /// Marks the selected column as an X column.
  /// </summary>
  public class SetColumnAsX : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsX(ctrl);
    }
  }

  /// <summary>
  /// Marks the selected column as a Y column.
  /// </summary>
  public class SetColumnAsY : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsY(ctrl);
    }
  }

  /// <summary>
  /// Marks the selected column as a label column.
  /// </summary>
  public class SetColumnAsLabel : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsLabel(ctrl);
    }
  }

  /// <summary>
  /// Marks the selected column as a value column.
  /// </summary>
  public class SetColumnAsValue : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsValue(ctrl);
    }
  }

  /// <summary>
  /// Marks the selected column as an error column.
  /// </summary>
  public class SetColumnAsError : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsKind(ctrl, Altaxo.Data.ColumnKind.Err);
    }
  }

  /// <summary>
  /// Marks the selected column as a positive error column.
  /// </summary>
  public class SetColumnAsPositiveError : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsKind(ctrl, Altaxo.Data.ColumnKind.pErr);
    }
  }

  /// <summary>
  /// Marks the selected column as a negative error column.
  /// </summary>
  public class SetColumnAsNegativeError : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnAsKind(ctrl, Altaxo.Data.ColumnKind.mErr);
    }
  }

  /// <summary>
  /// Renames the selected column.
  /// </summary>
  public class RenameColumn : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.RenameSelectedColumn(ctrl);
    }
  }

  /// <summary>
  /// Sets the group number of the selected columns.
  /// </summary>
  public class SetColumnGroupNumber : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.ShowSetColumnGroupNumberDialog(ctrl);
    }
  }

  /// <summary>
  /// Sets the position of the selected column.
  /// </summary>
  public class SetColumnPosition : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.SetSelectedColumnPosition(ctrl);
    }
  }

  /// <summary>
  /// Extracts property values into worksheet data.
  /// </summary>
  public class ExtractPropertyValues : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.ColumnCommands.ExtractPropertyValues(ctrl);
    }
  }

  /// <summary>
  /// Sorts the table in ascending order.
  /// </summary>
  public class SortTableAscending : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Sort(ctrl, true);
    }

    /// <summary>
    /// Sorts the table using the current selection.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    /// <param name="ascending"><see langword="true"/> to sort ascending; otherwise, <see langword="false"/>.</param>
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



  /// <summary>
  /// Sorts the table in descending order.
  /// </summary>
  public class SortTableDescending : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      SortTableAscending.Sort(ctrl, false);
    }
  }

  #endregion Column commands

  #region Row commands

  /// <summary>
  /// Sets the position of the selected row.
  /// </summary>
  public class SetRowPosition : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.RowCommands.SetSelectedRowPosition(ctrl);
    }
  }

  /// <summary>
  /// Inserts a single data row.
  /// </summary>
  public class InsertOneDataRow : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.RowCommands.InsertOneDataRow(ctrl);
    }
  }

  /// <summary>
  /// Inserts multiple data rows.
  /// </summary>
  public class InsertDataRows : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.RowCommands.InsertDataRows(ctrl);
    }
  }

  /// <summary>
  /// Converts selected rows into property columns.
  /// </summary>
  public class ChangeRowsToPropertyColumns : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      ctrl.DataTable.ChangeRowsToPropertyColumns(ctrl.SelectedDataRows, ctrl.SelectedDataColumns);
      ctrl.ClearAllSelections();
    }
  }

  #endregion Row commands

  #region Property column commands

  /// <summary>
  /// Sorts columns by the selected property column in ascending order.
  /// </summary>
  public class SortColumnsByPropertyColumnAscending : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Sort(ctrl, true);
    }

    /// <summary>
    /// Sorts columns by the selected property column.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    /// <param name="ascending"><see langword="true"/> to sort ascending; otherwise, <see langword="false"/>.</param>
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

  /// <summary>
  /// Sorts columns by the selected property column in descending order.
  /// </summary>
  public class SortColumnsByPropertyColumnDescending : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      SortColumnsByPropertyColumnAscending.Sort(ctrl, false);
    }
  }

  #endregion Property column commands

  #region Analysis

  /// <summary>
  /// Opens the data source editor for the active worksheet.
  /// </summary>
  public class EditTableDataSource : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.DataSourceCommands.ShowDataSourceEditor(ctrl);
    }
  }

  /// <summary>
  /// Requeries the data source of the active worksheet.
  /// </summary>
  public class RequeryTableDataSource : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.DataSourceCommands.ExecuteDataSourceOfTableUserCancellable(ctrl);
    }
  }

  /// <summary>
  /// Performs a Fourier transform on the active worksheet data.
  /// </summary>
  public class AnalysisFFT : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.FFT(ctrl);
    }
  }

  /// <summary>
  /// Performs a two-dimensional Fourier transform on the active worksheet data.
  /// </summary>
  public class Analysis2DFFT : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.TwoDimensionalFFT(ctrl);
    }
  }

  /// <summary>
  /// Performs a centered two-dimensional Fourier transform on the active worksheet data.
  /// </summary>
  public class Analysis2DCenteredFFT : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.TwoDimensionalCenteredFFT(ctrl);
    }
  }

  /// <summary>
  /// Convolves the selected worksheet data.
  /// </summary>
  public class Convolute : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.Convolution(ctrl);
    }
  }

  /// <summary>
  /// Correlates the selected worksheet data.
  /// </summary>
  public class Correlate : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.FourierCommands.Correlation(ctrl);
    }
  }

  /// <summary>
  /// Calculates statistics on columns.
  /// </summary>
  public class AnalysisStatisticsOnColumns : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.StatisticCommands.StatisticsOnColumns(ctrl);
    }
  }

  /// <summary>
  /// Calculates statistics on rows.
  /// </summary>
  public class AnalysisStatisticsOnRows : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.StatisticCommands.StatisticsOnRows(ctrl);
    }
  }

  /// <summary>
  /// Creates a histogram from the selected data.
  /// </summary>
  public class AnalysisStatisticsCreateHistogram : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.StatisticCommands.CreateHistogram(ctrl);
    }
  }

  /// <summary>
  /// Multiplies selected columns to form a matrix.
  /// </summary>
  public class AnalysisMultiplyColumnsToMatrix : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.MultiplyColumnsToMatrix(ctrl);
    }
  }

  /// <summary>
  /// Performs principal component analysis on rows.
  /// </summary>
  public class AnalysisPCAOnRows : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PCAOnRows(ctrl);
    }
  }

  /// <summary>
  /// Performs principal component analysis on columns.
  /// </summary>
  public class AnalysisPCAOnCols : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
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

  /// <summary>
  /// Performs partial least squares analysis on columns.
  /// </summary>
  public class AnalysisPLSOnCols : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PLSOnColumns(ctrl);
    }
  }

  /// <summary>
  /// Predicts values by using a partial least squares model on columns.
  /// </summary>
  public class AnalysisPLSPredictOnCols : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PredictOnColumns(ctrl);
    }
  }

  /// <summary>
  /// Exports a partial least squares calibration.
  /// </summary>
  public class AnalysisExportPLSCalibration : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.ExportPLSCalibration(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Differentiates and smooths the selected data.
  /// </summary>
  public class AnalysisDifferentiateSmooth : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.SavitzkyGolayFiltering(ctrl);
    }
  }

  /// <summary>
  /// Interpolates the selected data.
  /// </summary>
  public class AnalysisInterpolation : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.Interpolation(ctrl);
    }
  }

  /// <summary>
  /// Performs multivariate linear regression.
  /// </summary>
  public class AnalysisMultivariateLinearRegression : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.MultivariateLinearFit(ctrl);
    }
  }
  /// <summary>
  /// Performs Prony relaxation analysis in the time domain.
  /// </summary>
  public class AnalysisPronyRelaxationTimeDomain : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.PronyRelaxationTimeDomain(ctrl);
    }
  }

  /// <summary>
  /// Performs Prony relaxation analysis in the frequency domain.
  /// </summary>
  public class AnalysisPronyRelaxationFrequencyDomain : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.PronyRelaxationFrequencyDomain(ctrl);
    }
  }

  /// <summary>
  /// Performs Prony retardation analysis in the time domain.
  /// </summary>
  public class AnalysisPronyRetardationTimeDomain : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.PronyRetardationTimeDomain(ctrl);
    }
  }

  /// <summary>
  /// Performs Prony retardation analysis in the frequency domain.
  /// </summary>
  public class AnalysisPronyRetardationFrequencyDomain : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Worksheet.Commands.Analysis.CalculusCommands.PronyRetardationFrequencyDomain(ctrl);
    }
  }

  /// <summary>
  /// Opens the master-curve creation dialog.
  /// </summary>
  public class AnalysisMasterCurveCreation : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Altaxo.Science.Thermorheology.MasterCurves.MasterCurveCreation.CreateMasterCurveShowDialog(ctrl.DataTable, ctrl.SelectedDataColumns, ctrl.SelectedDataRows);
    }
  }

  /// <summary>
  /// Opens the spectroscopy preprocessing dialog.
  /// </summary>
  public class SpectroscopyPreprocessing : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Science.Spectroscopy.SpectroscopyCommands.SpectralPreprocessingShowDialog(ctrl);
    }
  }

  /// <summary>
  /// Opens the spectroscopy peak finding and fitting dialog.
  /// </summary>
  public class SpectroscopyPeakFindingFitting : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Science.Spectroscopy.SpectroscopyCommands.SpectralPeakFindingFittingShowDialog(ctrl);
    }
  }

  /// <summary>
  /// Opens the peak finding and fitting dialog for multiple spectra.
  /// </summary>
  public class SpectroscopyFitPeaksInMultipleSpectra
    : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Science.Spectroscopy.SpectroscopyCommands.PeakFindingFittingInMultipleSpectraShowDialog(ctrl);
    }
  }

  /// <summary>
  /// Calibrates Raman data by using a neon spectrum.
  /// </summary>
  public class SpectroscopyRamanNeonCalibration : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Science.Spectroscopy.SpectroscopyCommands.Raman_CalibrateWithNeonSpectrum(ctrl);
    }
  }

  /// <summary>
  /// Calibrates Raman data by using a silicon spectrum.
  /// </summary>
  public class SpectroscopyRamanSiliconCalibration : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Science.Spectroscopy.SpectroscopyCommands.Raman_CalibrateWithSiliconSpectrum(ctrl);
    }
  }

  /// <summary>
  /// Calibrates the Y axis by using an intensity spectrum.
  /// </summary>
  public class SpectroscopyYCalibration : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      Science.Spectroscopy.SpectroscopyCommands.CalibrateWithIntensitySpectrum(ctrl);
    }
  }

  #endregion Analysis
}
