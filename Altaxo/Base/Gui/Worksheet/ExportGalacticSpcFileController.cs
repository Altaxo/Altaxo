using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;
using Altaxo.Serialization.Galactic;
using Altaxo.Serialization.Galactic.Options;

using Altaxo.Collections;

namespace Altaxo.Gui.Worksheet
{
	#region Interfaces
	public interface IExportGalacticSpcFileView
	{
		bool CreateSpectrumFromRow { get; set; }
		bool CreateSpectrumFromColumn { get; }
		bool XValuesContinuousNumber { get; set; }
		bool XValuesFromColumn { get; }
		bool ExtendFileName_ContinuousNumber { get; set; }
		bool ExtendFileName_ByColumn { get; }
		string BasicFileName { get; set; }

		void FillXValuesColumnBox(SelectableListNodeList list);
		bool EnableXValuesColumnBox { set; }
		string XValuesColumnName { get; }

		void FillExtFileNameColumnBox(SelectableListNodeList list);
		bool EnableExtFileNameColumnBox { set; }
		string ExtFileNameColumnName { get; }

		event Action BasicFileNameAndPathChoose;
		event Action Change_CreateSpectrumFrom;
		event Action Change__XValuesFromOption;
		event Action Change_ExtendFileNameOptions;
	}


	#endregion

	/// <summary>
	/// The controller class which controls the <see cref="ExportGalacticSpcFileDialog"/> dialog.
	/// </summary>
	[ExpectedTypeOfView(typeof(IExportGalacticSpcFileView))]
	public class ExportGalacticSpcFileDialogController : IMVCAController
	{
		/// <summary>
		/// The dialog to control.
		/// </summary>
		private IExportGalacticSpcFileView _view;
		/// <summary>The table where the data stems from.</summary>
		protected Altaxo.Data.DataTable m_Table;

		/// <summary>The selected rows of the table (only this data are exported).</summary>
		protected IAscendingIntegerCollection m_SelectedRows;

		/// <summary>The selected columns of the table (only this data are exported).</summary>
		protected IAscendingIntegerCollection m_SelectedColumns;

		/// <summary>Stores if one spectrum is created from one table row or from one table column (<see cref="Options.CreateSpectrumFrom"/>).</summary>
		protected CreateSpectrumFrom m_CreateSpectrumFrom;

		/// <summary>Stores if the x values stem from continuous numbering or from a data column (<see cref="Options.XValuesFrom"/></summary>
		protected XValuesFrom m_XValuesFrom;

		/// <summary>Stores if the filename is extended by a continuous number or by the contents of a data column (<see cref="Options.ExtendFileNameWith"/></summary>
		protected ExtendFileNameWith m_ExtendFileNameWith;

		/// <summary>
		/// Creates the controller for the Galactic SPC file export dialog.
		/// </summary>
		/// <param name="table">The data table which is about to be exported.</param>
		/// <param name="selectedRows">The selected rows of the data table.</param>
		/// <param name="selectedColumns">The selected columns of the data table.</param>
		public ExportGalacticSpcFileDialogController(
			Altaxo.Data.DataTable table,
			IAscendingIntegerCollection selectedRows,
			IAscendingIntegerCollection selectedColumns)
		{
			m_Table = table;
			m_SelectedRows = selectedRows;
			m_SelectedColumns = selectedColumns;

			InitializeElements();
		}


		/// <summary>
		/// Initialize the elements of the dialog.
		/// </summary>
		public void InitializeElements()
		{
			if (null != _view)
			{
				this.m_CreateSpectrumFrom = CreateSpectrumFrom.Row;
				_view.CreateSpectrumFromRow = true;

				this.m_XValuesFrom = XValuesFrom.ContinuousNumber;
				_view.XValuesContinuousNumber = true;

				this.m_ExtendFileNameWith = ExtendFileNameWith.ContinuousNumber;
				_view.ExtendFileName_ContinuousNumber = true;
				_view.BasicFileName = "";
			}
		}


		/// <summary>
		/// Fills the "x values column combobox" with the appropriate column names.
		/// </summary>
		/// <remarks>The column names are either from property columns (if a spectrum is from a row) or from a table column
		///  (if the spectrum is from a column).<para/>
		/// In either case, only DataColumns that have the <see cref="Altaxo.Data.INumericColumn"/> interface are shown in the combobox.</remarks>
		public void FillXValuesColumnBox()
		{
			Altaxo.Data.DataColumnCollection colcol;
			if (this.m_CreateSpectrumFrom == CreateSpectrumFrom.Row)
				colcol = m_Table.PropCols;
			else
				colcol = m_Table.DataColumns;

			// Fill the Combo Box with Column names
			var colnames = new SelectableListNodeList();
			for (int i = 0, j = 0; i < colcol.ColumnCount; i++)
				if (colcol[i] is Altaxo.Data.INumericColumn)
					colnames.Add(new SelectableListNode(colcol.GetColumnName(i),colcol[i],0==i));

			// now set the contents of the combo box
			_view.FillXValuesColumnBox(colnames);
			_view.EnableXValuesColumnBox = true;
		}

		/// <summary>
		/// This fills the "Extend file name by column" combobox with appropriate column names.
		/// </summary>
		/// <remarks>The columns shown are either table columns (if a spectrum is a single row), or
		/// a property column (if a spectrum is a single column).</remarks>
		public void FillExtFileNameColumnBox()
		{
			Altaxo.Data.DataColumnCollection colcol;
			if (this.m_CreateSpectrumFrom == CreateSpectrumFrom.Row)
				colcol = m_Table.DataColumns;
			else
				colcol = m_Table.PropCols;

			// Fill the Combo Box with Column names
			var colnames = new SelectableListNodeList();
			for (int i = 0; i < colcol.ColumnCount; i++)
				colnames.Add(new SelectableListNode( colcol.GetColumnName(i), colcol[i], i==0));

			// now set the contents of the combo box
			_view.FillExtFileNameColumnBox(colnames);
			_view.EnableExtFileNameColumnBox = true;
		}


		/// <summary>
		/// This opens the "Save as" dialog box to choose a basic file name for exporting.
		/// </summary>
		public void ChooseBasicFileNameAndPath()
		{
			var saveOptions = new SaveFileOptions();
			saveOptions.AddFilter("*.*", "All Files (*.*)");
			saveOptions.FilterIndex = 0;
			saveOptions.RestoreDirectory = true;


			if (Current.Gui.ShowSaveFileDialog(saveOptions))
			{
				_view.BasicFileName = saveOptions.FileName;
			}
		}

		/// <summary>
		/// Called if a change in the options "Create spectrum from" occurs.
		/// </summary>
		public void EhChange_CreateSpectrumFrom()
		{
			CreateSpectrumFrom oldValue = m_CreateSpectrumFrom;

			if (_view.CreateSpectrumFromRow)
				m_CreateSpectrumFrom = CreateSpectrumFrom.Row;
			else if (_view.CreateSpectrumFromColumn)
				m_CreateSpectrumFrom = CreateSpectrumFrom.Column;

			if (m_CreateSpectrumFrom != oldValue)
			{
				if (this.m_XValuesFrom == XValuesFrom.Column)
					this.FillXValuesColumnBox();

				if (this.m_ExtendFileNameWith == ExtendFileNameWith.Column)
					this.FillExtFileNameColumnBox();
			}
		}

		/// <summary>
		/// Called if a change in the options "X values from" occured.
		/// </summary>
		public void EhChange_XValuesFromOptions()
		{
			XValuesFrom oldValue = m_XValuesFrom;

			if (_view.XValuesContinuousNumber)
				m_XValuesFrom = XValuesFrom.ContinuousNumber;
			else if (_view.XValuesFromColumn)
				m_XValuesFrom = XValuesFrom.Column;

			if (m_XValuesFrom != oldValue)
			{
				if (m_XValuesFrom == XValuesFrom.Column)
				{
					this.FillXValuesColumnBox();
				}
				else
				{
					_view.EnableXValuesColumnBox = false;
				}
			}
		}


		/// <summary>
		/// Called if a change in the options "Extend file name by" occured.
		/// </summary>
		public void EhChange_ExtendFileNameOptions()
		{
			ExtendFileNameWith oldValue = this.m_ExtendFileNameWith;

			if (_view.ExtendFileName_ContinuousNumber)
				this.m_ExtendFileNameWith = ExtendFileNameWith.ContinuousNumber;
			else if (_view.ExtendFileName_ByColumn)
				this.m_ExtendFileNameWith = ExtendFileNameWith.Column;

			if (this.m_ExtendFileNameWith != oldValue)
			{
				if (this.m_ExtendFileNameWith == ExtendFileNameWith.Column)
				{
					// now set the contents of the combo box
					this.FillExtFileNameColumnBox();
				}
				else
				{
					_view.EnableExtFileNameColumnBox = false;
				}

			}
		}

		/// <summary>
		/// Called if user presses the "Ok" button on the dialog box.
		/// </summary>
		public bool Apply()
		{
			if (this.m_CreateSpectrumFrom == CreateSpectrumFrom.Row)
			{
				Altaxo.Data.INumericColumn xcol;

				if (this.m_XValuesFrom == XValuesFrom.Column)
				{
					string colname = _view.XValuesColumnName;
					if (null == colname)
					{
						Current.Gui.ErrorMessageBox("No x-column selected");
						return false;
					}

					xcol = this.m_Table.PropCols[colname] as Altaxo.Data.INumericColumn;
				}
				else // xvalues are continuous number
				{
					xcol = new Altaxo.Data.IndexerColumn();
				}

				Altaxo.Data.DataColumn extFileNameCol = null;
				if (this.m_ExtendFileNameWith == ExtendFileNameWith.Column)
					extFileNameCol = m_Table[_view.ExtFileNameColumnName];



				int i, j;
				bool bUseRowSel = (null != m_SelectedRows && this.m_SelectedRows.Count > 0);
				int numOfSpectra = bUseRowSel ? m_SelectedRows.Count : m_Table.DataColumns.RowCount;

				for (j = 0; j < numOfSpectra; j++)
				{
					i = bUseRowSel ? m_SelectedRows[j] : j;

					string filename = _view.BasicFileName;

					if (null != extFileNameCol)
						filename += "_" + extFileNameCol[i].ToString();
					else
						filename += "_" + j.ToString();


					string error = Export.FromRow(filename, this.m_Table, i, xcol, this.m_SelectedColumns);

					if (null != error)
					{
						Current.Gui.ErrorMessageBox(string.Format("There were error(s) during export: {0}", error));
						return false;
					}
				}
				Current.Gui.InfoMessageBox(string.Format("Export of {0} spectra successfull.", numOfSpectra));
			}
			else if (this.m_CreateSpectrumFrom == CreateSpectrumFrom.Column)
			{
				Altaxo.Data.INumericColumn xcol;

				if (this.m_XValuesFrom == XValuesFrom.Column)
				{
					string colname = _view.XValuesColumnName;
					if (null == colname)
					{
						Current.Gui.ErrorMessageBox("No x-column selected");
						return false;
					}

					xcol = this.m_Table.DataColumns[colname] as Altaxo.Data.INumericColumn;
				}
				else // xvalues are continuous number
				{
					xcol = new Altaxo.Data.IndexerColumn();
				}

				Altaxo.Data.DataColumn extFileNameCol = null;
				if (this.m_ExtendFileNameWith == ExtendFileNameWith.Column)
					extFileNameCol = m_Table.PropCols[_view.ExtFileNameColumnName];



				int i, j;
				bool bUseColSel = (null != m_SelectedColumns && this.m_SelectedColumns.Count > 0);
				int numOfSpectra = bUseColSel ? m_SelectedColumns.Count : m_Table.DataColumns.ColumnCount;

				for (j = 0; j < numOfSpectra; j++)
				{
					i = bUseColSel ? m_SelectedColumns[j] : j;

					string filename = _view.BasicFileName;

					if (null != extFileNameCol)
						filename += extFileNameCol[i].ToString();
					else
						filename += j.ToString() + ".spc";


					string error = Export.FromColumn(filename, this.m_Table, i, xcol, this.m_SelectedRows);

					if (null != error)
					{
						Current.Gui.ErrorMessageBox(string.Format("There were error(s) during export: {0}", error));
						return false;
					}
				}

				Current.Gui.InfoMessageBox(string.Format("Export of {0} spectra successfull.", numOfSpectra));
			}
			return true;
		}




		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					_view.BasicFileNameAndPathChoose -= this.ChooseBasicFileNameAndPath;
					_view.Change_ExtendFileNameOptions -= this.EhChange_ExtendFileNameOptions;
					_view.Change_CreateSpectrumFrom -= this.EhChange_CreateSpectrumFrom;
					_view.Change__XValuesFromOption -= this.EhChange_XValuesFromOptions;
				}

				_view = value as IExportGalacticSpcFileView;

				if (null != _view)
				{
					InitializeElements();

					_view.BasicFileNameAndPathChoose += this.ChooseBasicFileNameAndPath;
					_view.Change_ExtendFileNameOptions += this.EhChange_ExtendFileNameOptions;
					_view.Change_CreateSpectrumFrom += this.EhChange_CreateSpectrumFrom;
					_view.Change__XValuesFromOption += this.EhChange_XValuesFromOptions;

				}
			}
		}

		public object ModelObject
		{
			get { return null; }
		}

		#endregion
	} // end of class ExportGalacticSpcFileDialogController


}
