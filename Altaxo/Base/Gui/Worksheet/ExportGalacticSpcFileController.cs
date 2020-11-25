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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Serialization.Galactic;
using Altaxo.Serialization.Galactic.Options;

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

  #endregion Interfaces

  /// <summary>
  /// The controller class which is responsible for showing a dialog to export into the Galactic SPC file format.
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

    /// <summary>Stores if one spectrum is created from one table row or from one table column.</summary>
    protected CreateSpectrumFrom m_CreateSpectrumFrom;

    /// <summary>Stores if the x values stem from continuous numbering or from a data column.</summary>
    protected XValuesFrom m_XValuesFrom;

    /// <summary>Stores if the filename is extended by a continuous number or by the contents of a data column.</summary>
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
      if (_view is not null)
      {
        m_CreateSpectrumFrom = CreateSpectrumFrom.Row;
        _view.CreateSpectrumFromRow = true;

        m_XValuesFrom = XValuesFrom.ContinuousNumber;
        _view.XValuesContinuousNumber = true;

        m_ExtendFileNameWith = ExtendFileNameWith.ContinuousNumber;
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
      if (m_CreateSpectrumFrom == CreateSpectrumFrom.Row)
        colcol = m_Table.PropCols;
      else
        colcol = m_Table.DataColumns;

      // Fill the Combo Box with Column names
      var colnames = new SelectableListNodeList();
      for (int i = 0; i < colcol.ColumnCount; i++)
        if (colcol[i] is Altaxo.Data.INumericColumn)
          colnames.Add(new SelectableListNode(colcol.GetColumnName(i), colcol[i], 0 == i));

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
      if (m_CreateSpectrumFrom == CreateSpectrumFrom.Row)
        colcol = m_Table.DataColumns;
      else
        colcol = m_Table.PropCols;

      // Fill the Combo Box with Column names
      var colnames = new SelectableListNodeList();
      for (int i = 0; i < colcol.ColumnCount; i++)
        colnames.Add(new SelectableListNode(colcol.GetColumnName(i), colcol[i], i == 0));

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
        if (m_XValuesFrom == XValuesFrom.Column)
          FillXValuesColumnBox();

        if (m_ExtendFileNameWith == ExtendFileNameWith.Column)
          FillExtFileNameColumnBox();
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
          FillXValuesColumnBox();
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
      ExtendFileNameWith oldValue = m_ExtendFileNameWith;

      if (_view.ExtendFileName_ContinuousNumber)
        m_ExtendFileNameWith = ExtendFileNameWith.ContinuousNumber;
      else if (_view.ExtendFileName_ByColumn)
        m_ExtendFileNameWith = ExtendFileNameWith.Column;

      if (m_ExtendFileNameWith != oldValue)
      {
        if (m_ExtendFileNameWith == ExtendFileNameWith.Column)
        {
          // now set the contents of the combo box
          FillExtFileNameColumnBox();
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
    public bool Apply(bool disposeController)
    {
      if (m_CreateSpectrumFrom == CreateSpectrumFrom.Row)
      {
        Altaxo.Data.INumericColumn xcol;

        if (m_XValuesFrom == XValuesFrom.Column)
        {
          string colname = _view.XValuesColumnName;
          if (colname is null)
          {
            Current.Gui.ErrorMessageBox("No x-column selected");
            return false;
          }

          xcol = m_Table.PropCols[colname] as Altaxo.Data.INumericColumn;
        }
        else // xvalues are continuous number
        {
          xcol = new Altaxo.Data.IndexerColumn();
        }

        Altaxo.Data.DataColumn extFileNameCol = null;
        if (m_ExtendFileNameWith == ExtendFileNameWith.Column)
          extFileNameCol = m_Table[_view.ExtFileNameColumnName];

        int i, j;
        bool bUseRowSel = (m_SelectedRows is not null && m_SelectedRows.Count > 0);
        int numOfSpectra = bUseRowSel ? m_SelectedRows.Count : m_Table.DataColumns.RowCount;

        for (j = 0; j < numOfSpectra; j++)
        {
          i = bUseRowSel ? m_SelectedRows[j] : j;

          string filename = _view.BasicFileName;

          if (extFileNameCol is not null)
            filename += "_" + extFileNameCol[i].ToString();
          else
            filename += "_" + j.ToString();

          string error = Export.FromRow(filename, m_Table, i, xcol, m_SelectedColumns);

          if (error is not null)
          {
            Current.Gui.ErrorMessageBox(string.Format("There were error(s) during export: {0}", error));
            return false;
          }
        }
        Current.Gui.InfoMessageBox(string.Format("Export of {0} spectra successfull.", numOfSpectra));
      }
      else if (m_CreateSpectrumFrom == CreateSpectrumFrom.Column)
      {
        Altaxo.Data.INumericColumn xcol;

        if (m_XValuesFrom == XValuesFrom.Column)
        {
          string colname = _view.XValuesColumnName;
          if (colname is null)
          {
            Current.Gui.ErrorMessageBox("No x-column selected");
            return false;
          }

          xcol = m_Table.DataColumns[colname] as Altaxo.Data.INumericColumn;
        }
        else // xvalues are continuous number
        {
          xcol = new Altaxo.Data.IndexerColumn();
        }

        Altaxo.Data.DataColumn extFileNameCol = null;
        if (m_ExtendFileNameWith == ExtendFileNameWith.Column)
          extFileNameCol = m_Table.PropCols[_view.ExtFileNameColumnName];

        int i, j;
        bool bUseColSel = (m_SelectedColumns is not null && m_SelectedColumns.Count > 0);
        int numOfSpectra = bUseColSel ? m_SelectedColumns.Count : m_Table.DataColumns.ColumnCount;

        for (j = 0; j < numOfSpectra; j++)
        {
          i = bUseColSel ? m_SelectedColumns[j] : j;

          string filename = _view.BasicFileName;

          if (extFileNameCol is not null)
            filename += extFileNameCol[i].ToString();
          else
            filename += j.ToString() + ".spc";

          string error = Export.FromColumn(filename, m_Table, i, xcol, m_SelectedRows);

          if (error is not null)
          {
            Current.Gui.ErrorMessageBox(string.Format("There were error(s) during export: {0}", error));
            return false;
          }
        }

        Current.Gui.InfoMessageBox(string.Format("Export of {0} spectra successfull.", numOfSpectra));
      }
      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
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
        if (_view is not null)
        {
          _view.BasicFileNameAndPathChoose -= ChooseBasicFileNameAndPath;
          _view.Change_ExtendFileNameOptions -= EhChange_ExtendFileNameOptions;
          _view.Change_CreateSpectrumFrom -= EhChange_CreateSpectrumFrom;
          _view.Change__XValuesFromOption -= EhChange_XValuesFromOptions;
        }

        _view = value as IExportGalacticSpcFileView;

        if (_view is not null)
        {
          InitializeElements();

          _view.BasicFileNameAndPathChoose += ChooseBasicFileNameAndPath;
          _view.Change_ExtendFileNameOptions += EhChange_ExtendFileNameOptions;
          _view.Change_CreateSpectrumFrom += EhChange_CreateSpectrumFrom;
          _view.Change__XValuesFromOption += EhChange_XValuesFromOptions;
        }
      }
    }

    public object ModelObject
    {
      get { return null; }
    }

    public void Dispose()
    {
    }

    #endregion IMVCController Members
  } // end of class ExportGalacticSpcFileDialogController
}
