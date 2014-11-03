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

using Altaxo;
using Altaxo.Data;
using Altaxo.Gui.Scripting;
using Altaxo.Gui.Worksheet.Viewing;
using Altaxo.Main;
using Altaxo.Scripting;
using Altaxo.Worksheet;
using ICSharpCode.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;

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

			if (null != ctrl)
				Run((Altaxo.Gui.Worksheet.Viewing.WorksheetController)ctrl.MVCController);
		}

		/// <summary>
		/// Override this function for adding own worksheet commands. You will get
		/// the worksheet controller in the parameter.
		/// </summary>
		/// <param name="ctrl">The worksheet controller this command is applied to.</param>
		public abstract void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl);
	}

	#endregion Abstract command

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

	public class ImportImage : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			Altaxo.Data.FileCommands.ShowImportImageDialog(ctrl.DataTable);
		}
	}

	public class ImportGalacticSPC : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			Altaxo.Data.FileCommands.ShowImportGalacticSPCDialog(ctrl.DataTable);
		}
	}

	public class ImportJcamp : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			Altaxo.Data.FileCommands.ShowImportJcampDialog(ctrl.DataTable);
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
			if (msg != null)
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
			Altaxo.Worksheet.Commands.PlotCommands.PlotDensityImage(ctrl, true, true);
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
			Altaxo.Worksheet.Commands.WorksheetCommands.WorksheetClearData(ctrl);
		}
	}

	public class WorksheetClearDataOnly : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			ctrl.DataTable.DataColumns.ClearData();
		}
	}

	public class WorksheetRemoveDataColumnsOnly : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			ctrl.DataTable.DataColumns.RemoveColumnsAll();
		}
	}

	public class DecomposeCyclingIndependentVariable : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			ctrl.DataTable.ShowExpandCyclingVariableColumnDialog(ctrl.SelectedDataRows, ctrl.SelectedDataColumns);
		}
	}

	public class OpenExtractTableDataScriptDialog : AbstractWorksheetControllerCommand
	{
		private const string ExtractTableDataScriptPropertyName = "Scripts/ExtractTableData";
		private Altaxo.Data.DataTable m_Table;

		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			m_Table = ctrl.DataTable;
			ExtractTableDataScript script = ctrl.DataTable.GetTableProperty(ExtractTableDataScriptPropertyName) as ExtractTableDataScript;

			if (script == null)
				script = new ExtractTableDataScript();

			object[] args = new object[] { script, new ScriptExecutionHandler(this.EhScriptExecution) };
			if (Current.Gui.ShowDialog(args, "WorksheetScript of " + m_Table.Name))
			{
				m_Table.SetTableProperty(ExtractTableDataScriptPropertyName, args[0]);
			}

			this.m_Table = null;
		}

		public bool EhScriptExecution(IScriptText script, IProgressReporter reporter)
		{
			return ((ExtractTableDataScript)script).Execute(m_Table);
		}
	}

	public class OpenTableScriptDialog : AbstractWorksheetControllerCommand
	{
		private Altaxo.Data.DataTable m_Table;

		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			m_Table = ctrl.DataTable;

			IScriptText script = m_Table.TableScript;

			if (script == null)
				script = new TableScript();

			object[] args = new object[] { script, new Altaxo.Gui.Scripting.ScriptExecutionHandler(this.EhScriptExecution) };
			if (Current.Gui.ShowDialog(args, "WorksheetScript of " + m_Table.Name))
			{
				m_Table.TableScript = (TableScript)args[0];
			}

			this.m_Table = null;
		}

		public bool EhScriptExecution(IScriptText script, IProgressReporter reporter)
		{
			return ((TableScript)script).ExecuteWithSuspendedNotifications(m_Table, reporter);
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

			IColumnScriptText script = null;
			dataTable.DataColumns.ColumnScripts.TryGetValue(m_Column, out script);
			if (script == null)
				script = new DataColumnScript();

			object[] args = new object[] { script, new ScriptExecutionHandler(this.EhScriptExecution) };
			if (Current.Gui.ShowDialog(args, "DataColumnScript of " + m_Column.Name))
			{
				dataTable.DataColumns.ColumnScripts[m_Column] = (IColumnScriptText)args[0];
			}
			this.m_Column = null;
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

			IColumnScriptText script;
			dataTable.PropertyColumns.ColumnScripts.TryGetValue(m_Column, out script);
			if (script == null)
				script = new PropertyColumnScript();

			object[] args = new object[] { script, new ScriptExecutionHandler(this.EhScriptExecution) };
			if (Current.Gui.ShowDialog(args, "PropertyColumnScript of " + m_Column.Name))
			{
				dataTable.PropCols.ColumnScripts[m_Column] = (IColumnScriptText)args[0];
			}
			this.m_Column = null;
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
			if (ctrl.SelectedDataColumns.Count == 1)
				Altaxo.Data.Sorting.SortDataRows(ctrl.DataTable, ctrl.DataTable.DataColumns[ctrl.SelectedDataColumns[0]], ascending);
			else if (ctrl.SelectedPropertyColumns.Count == 1)
				Altaxo.Data.Sorting.SortDataColumnsByPropertyColumn(ctrl.DataTable, ctrl.DataTable.PropCols[ctrl.SelectedPropertyColumns[0]], ascending);
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

			Altaxo.Data.Sorting.SortDataColumnsByPropertyColumn(ctrl.DataTable, selectedDataColumns, ctrl.DataTable.PropCols[ctrl.SelectedPropertyColumns[0]], ascending);
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
			Altaxo.Worksheet.Commands.DataSourceCommands.RequeryTableDataSource(ctrl);
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

	public class AnalysisPLSOnRows : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PLSOnRows(ctrl);
		}
	}

	public class AnalysisPLSOnCols : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PLSOnColumns(ctrl);
		}
	}

	public class AnalysisPLSPredictOnRows : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.PredictOnRows(ctrl);
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

	#endregion Analysis
}