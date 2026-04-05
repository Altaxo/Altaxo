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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Collections;
using Altaxo.Geometry;
using Altaxo.Gui.Workbench;
using Altaxo.Worksheet;
using Altaxo.Worksheet.Commands;

namespace Altaxo.Gui.Worksheet.Viewing
{
  using AM = Altaxo.Worksheet.AreaRetrieval;

  /// <summary>
  /// Default controller which implements IWorksheetController.
  /// </summary>
  public partial class WorksheetController : AbstractViewContent, IWorksheetController, IDisposable
  {
    /// <summary>
    /// Describes the last selection type made in the worksheet.
    /// </summary>
    public enum SelectionType
    {
      /// <summary>
      /// No selection is active.
      /// </summary>
      Nothing,

      /// <summary>
      /// Data rows are selected.
      /// </summary>
      DataRowSelection,

      /// <summary>
      /// Data columns are selected.
      /// </summary>
      DataColumnSelection,

      /// <summary>
      /// Property columns are selected.
      /// </summary>
      PropertyColumnSelection,

      /// <summary>
      /// Property rows are selected.
      /// </summary>
      PropertyRowSelection
    }

    #region Member variables

    /// <summary>Which selection was done last: selection (i) a data column, (ii) a data row, or (iii) a property column.</summary>
    protected SelectionType _lastSelectionType;

    /// <summary>
    /// Horizontal scroll position; number of first column that is shown.
    /// </summary>
    private int _scrollHorzPos;

    /// <summary>
    /// Vertical scroll position; Positive values: number of first data column
    /// that is shown. Negative Values scroll more up in case of property columns.
    /// </summary>
    private int _scrollVertPos;

    private int _scrollHorzMax;
    private int _scrollVertMax;

    /// <summary>
    /// Holds the indizes to the selected data columns.
    /// </summary>
    protected IndexSelection _selectedDataColumns; // holds the selected columns

    /// <summary>
    /// Holds the indizes to the selected rows.
    /// </summary>
    protected IndexSelection _selectedDataRows; // holds the selected rows

    /// <summary>
    /// Holds the indizes to the selected property columns.
    /// </summary>
    protected IndexSelection _selectedPropertyColumns; // holds the selected property columns

    /// <summary>
    /// Holds the indizes to the selected property rows (but only in case property cells are selected alone).
    /// </summary>
    protected IndexSelection _selectedPropertyRows; // holds the selected property rows

    /// <summary>
    /// Cached number of table rows.
    /// </summary>
    protected int _numberOfTableRows; // cached number of rows of the table

    /// <summary>
    /// Cached number of table columns.
    /// </summary>
    protected int _numberOfTableCols;

    /// <summary>
    /// Cached number of property columns.
    /// </summary>
    protected int _numberOfPropertyCols; // cached number of property  columnsof the table

    /// <summary>List of x positions where the horizontal resizing cursor should be shown in order to resize the columns.</summary>
    protected List<double> _columnWidthResizingPositions = new List<double>();

    /// <summary>Index of the first data column that was shown when the list of resizing positions was created.</summary>
    protected int _columnWidthResizingPositionsFirstColumnIndex;

    //private ClickedCellInfoWpf _mouseInfo = new ClickedCellInfoWpf();

    private PointD2D _mouseDownPosition; // holds the position of a double click
    private int _dragColumnWidth_ColumnNumber; // stores the column number if mouse hovers over separator
    private double _dragColumnWidth_OriginalPos;
    private double _dragColumnWidth_OriginalWidth;
    private bool _dragColumnWidth_InCapture;

    /// <summary>
    /// Indicates whether cell editing is currently armed.
    /// </summary>
    protected bool _cellEdit_IsArmed;

    /// <summary>
    /// Indicates whether the current cell-edit content was modified.
    /// </summary>
    protected bool _cellEdit_IsModified;
    private AreaInfo _cellEdit_EditedCell;

    /// <summary>
    /// Weak event handler for data-column changes.
    /// </summary>
    protected WeakEventHandler? _weakEventHandlerDataColumnChanged;

    /// <summary>
    /// Weak event handler for property-column changes.
    /// </summary>
    protected WeakEventHandler? _weakEventHandlerPropertyColumnChanged;

    /// <summary>
    /// Set the member variables to default values. Intended only for use in constructors and deserialization code.
    /// </summary>
    [MemberNotNull(nameof(_selectedDataColumns), nameof(_selectedDataRows), nameof(_selectedPropertyColumns), nameof(_selectedPropertyRows), nameof(_cellEdit_EditedCell))]
    protected virtual void SetMemberVariablesToDefault()
    {
      // The main menu of this controller.

      // Which selection was done last: selection (i) a data column, (ii) a data row, or (iii) a property column.</summary>
      _lastSelectionType = SelectionType.Nothing;

      // Horizontal scroll position; number of first column that is shown.
      _scrollHorzPos = 0;

      // Vertical scroll position; Positive values: number of first data column
      _scrollVertPos = 0;
      _scrollHorzMax = 1;
      _scrollVertMax = 1;

      // Holds the indices of the selected data columns.
      _selectedDataColumns = new IndexSelection(); // holds the selected columns

      // Holds the indices of the selected rows.
      _selectedDataRows = new IndexSelection(); // holds the selected rows

      // Holds the indices of the selected property columns.
      _selectedPropertyColumns = new IndexSelection(); // holds the selected property columns

      // Holds the indizes to the selected property columns.
      _selectedPropertyRows = new IndexSelection(); // holds the selected property columns

      // Cached number of table rows.
      _numberOfTableRows = 0; // cached number of rows of the table

      // Cached number of table columns.
      _numberOfTableCols = 0;

      // Cached number of property columns.
      _numberOfPropertyCols = 0; // cached number of property  columnsof the table

      _mouseDownPosition = new PointD2D(0, 0); // holds the position of a double click
      _dragColumnWidth_ColumnNumber = int.MinValue; // stores the column number if mouse hovers over separator
      _dragColumnWidth_OriginalPos = 0;
      _dragColumnWidth_OriginalWidth = 0;
      _dragColumnWidth_InCapture = false;

      _cellEdit_IsArmed = false;
      _cellEdit_EditedCell = new AreaInfo();

      _cellEdit_IsArmed = false;
      _cellEdit_IsModified = false;
    }

    #endregion Member variables

    #region public properties

    /// <summary>
    /// Gets the width of the visible table area.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double TableAreaWidth
    {
      get { return _view?.TableArea_Size.X ?? 0; }
    }

    /// <summary>
    /// Gets the height of the visible table area.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double TableAreaHeight
    {
      get { return _view?.TableArea_Size.Y ?? 0; }
    }

    #endregion public properties

    #region Selection of rows and columns

    /// <summary>
    /// Gets the currently selected data columns.
    /// </summary>
    public IndexSelection SelectedDataColumns
    {
      get { return _selectedDataColumns; }
    }

    /// <summary>
    /// Returns the currently selected data rows.
    /// </summary>
    public IndexSelection SelectedDataRows
    {
      get { return _selectedDataRows; }
    }

    /// <summary>
    /// Returns the currently selected property columns.
    /// </summary>
    public IndexSelection SelectedPropertyColumns
    {
      get { return _selectedPropertyColumns; }
    }

    /// <summary>
    /// Returns the currently selected property rows if property cells are selected alone. If not selected alone, the SelectedColumn property is returned.
    /// </summary>
    /// <remarks>Normally, if you select one or more data columns, the corresponding property rows are selected as well. Therefore, it would not be possible to select property rows without also selecting the
    /// data columns. To work around this, you can first select property columns and then data columns. In this case the selection is not stored in
    /// SelectedColumns, but in SelectedPropertyRows, and SelectedColumns.Count returns 0.</remarks>
    public IndexSelection SelectedPropertyRows
    {
      get { return _selectedPropertyRows.Count > 0 ? _selectedPropertyRows : _selectedDataColumns; }
    }

    /// <summary>
    /// Returns true if one or more property columns or rows are selected.
    /// </summary>
    public bool ArePropertyCellsSelected
    {
      get
      {
        return DataTable.PropCols.ColumnCount > 0 && (SelectedPropertyColumns.Count > 0 || _selectedPropertyRows.Count > 0);
      }
    }

    /// <summary>
    /// Returns true if one or more data columns or rows are selected.
    /// </summary>
    public bool AreDataCellsSelected
    {
      get { return DataTable.DataColumns.ColumnCount > 0 && SelectedDataColumns.Count > 0 || SelectedDataRows.Count > 0; }
    }

    /// <summary>
    /// Returns true if one or more columns, rows or property columns or rows are selected.
    /// </summary>
    public bool AreColumnsOrRowsSelected
    {
      get { return AreDataCellsSelected || ArePropertyCellsSelected; }
    }

    /// <summary>
    /// Clears all selections of columns, rows or property columns.
    /// </summary>
    public void ClearAllSelections()
    {
      SelectedDataColumns.Clear();
      SelectedDataRows.Clear();
      SelectedPropertyColumns.Clear();
      SelectedPropertyRows.Clear();

      TriggerRedrawing();
    }

    /// <summary>
    /// Forces the view to redraw its table area, updating the display to reflect any recent changes.
    /// </summary>
    /// <remarks>Call this method when changes to the underlying data or view state require the table area to
    /// be refreshed. This method has no effect if the view is not initialized.</remarks>
    public void TriggerRedrawing()
    {
      _view?.TableArea_TriggerRedrawing();
    }

    #endregion Selection of rows and columns

    #region Shortcuts for column style retrieving

    /// <summary>
    /// Retrieves the column style for the data column with index i.
    /// </summary>
    /// <param name="i">The index of the data column for which the style has to be returned.</param>
    /// <returns>The column style of the data column.</returns>
    public Altaxo.Worksheet.ColumnStyle GetDataColumnStyle(int i)
    {
      return _worksheetLayout.DataColumnStyles[_table.DataColumns[i]];
    }

    /// <summary>
    /// Retrieves the column style for the property column with index i.
    /// </summary>
    /// <param name="i">The index of the property column for which the style has to be returned.</param>
    /// <returns>The column style of the property column.</returns>
    public Altaxo.Worksheet.ColumnStyle GetPropertyColumnStyle(int i)
    {
      return _worksheetLayout.PropertyColumnStyles[_table.PropertyColumns[i]];
    }

    #endregion Shortcuts for column style retrieving

    #region Data event handlers

    /// <summary>
    /// Handles changes in worksheet data values.
    /// </summary>
    public void EhTableDataChanged(object? sender, EventArgs e)
    {
      Current.Dispatcher.InvokeAndForget(EhTableDataChanged_Unsynchronized);
    }

    private void EhTableDataChanged_Unsynchronized()
    {
      if (IsDisposeInProgress)
        return;

      if (_numberOfTableRows != DataTable.DataColumns.RowCount)
        SetCachedNumberOfDataRows();

      if (_numberOfTableCols != DataTable.DataColumns.ColumnCount)
        SetCachedNumberOfDataColumns();

      _view?.TableArea_TriggerRedrawing();
    }

    /// <summary>
    /// Recalculates the maximum value of the vertical scroll bar.
    /// </summary>
    public void AdjustYScrollBarMaximum()
    {
      VertScrollMaximum = _numberOfTableRows > 0 ? _numberOfTableRows - 1 : 0;

      if (VerticalScrollPosition >= _numberOfTableRows)
        VerticalScrollPosition = _numberOfTableRows > 0 ? _numberOfTableRows - 1 : 0;

      if (_view is not null)
        _view.TableArea_TriggerRedrawing();
    }

    /// <summary>
    /// Recalculates the maximum value of the horizontal scroll bar.
    /// </summary>
    public void AdjustXScrollBarMaximum()
    {
      HorzScrollMaximum = _numberOfTableCols > 0 ? _numberOfTableCols - 1 : 0;

      if (HorzScrollPos + 1 > _numberOfTableCols)
        HorzScrollPos = _numberOfTableCols > 0 ? _numberOfTableCols - 1 : 0;

      if (_view is not null)
      {
        AdjustXScrollBarViewPortSize();
        _view.TableArea_TriggerRedrawing();
      }
    }

    /// <summary>Adjusts the X scroll bar viewport size (= size of the thumb). Should be called after the X scrollbar maximum is adjusted or when the width of the columns changes considerably.
    /// Here, we want to avoid that the thumb size changing while we scroll through the worksheet. Thus, we calculate an average value for the thumb size that is the relation of the table-area width and
    /// the total width of all data columns.</summary>
    public void AdjustXScrollBarViewPortSize()
    {
      if (_view is not null)
      {
        if (_numberOfTableCols > 0)
        {
          AM.GetXCoordinatesOfColumn(_numberOfTableCols - 1, _worksheetLayout, 0, out var left, out var width);
          _view.TableViewHorzViewPortSize = HorzScrollMaximum * TableAreaWidth / (left + width);
        }
        else
        {
          _view.TableViewHorzViewPortSize = 1000000; // if the worksheet contains no columns, then ensure that the viewport is fully streched horizontally
        }
      }
    }

    /// <summary>
    /// Updates the cached number of data columns.
    /// </summary>
    protected virtual void SetCachedNumberOfDataColumns()
    {
      // ask for table dimensions, compare with cached dimensions
      // and adjust the scroll bars appropriate
      int oldDataCols = _numberOfTableCols;
      _numberOfTableCols = DataTable.DataColumns.ColumnCount;
      if (_numberOfTableCols != oldDataCols)
      {
        AdjustXScrollBarMaximum();
      }
    }

    /// <summary>
    /// Updates the cached number of data rows.
    /// </summary>
    protected virtual void SetCachedNumberOfDataRows()
    {
      // ask for table dimensions, compare with cached dimensions
      // and adjust the scroll bars appropriate
      int oldDataRows = _numberOfTableRows;
      _numberOfTableRows = DataTable.DataColumns.RowCount;

      if (_numberOfTableRows != oldDataRows)
      {
        AdjustYScrollBarMaximum();
      }
    }

    /// <summary>
    /// Updates the cached number of property columns.
    /// </summary>
    protected virtual void SetCachedNumberOfPropertyColumns()
    {
      // ask for table dimensions, compare with cached dimensions
      // and adjust the scroll bars appropriate
      int oldPropCols = _numberOfPropertyCols;
      _numberOfPropertyCols = _table.PropCols.ColumnCount;

      if (oldPropCols != _numberOfPropertyCols)
      {
        // if we was scrolled to the most upper position, we later scroll
        // to the most upper position again
        bool bUpperPosition = (oldPropCols == -VerticalScrollPosition);

        // Adjust Y ScrollBar Maximum();
        AdjustYScrollBarMaximum();

        if (bUpperPosition) // we scroll again to the most upper position
        {
          VerticalScrollPosition = -TotalEnabledPropertyColumns;
        }
        else
        {
          // we first bring the VertScrollPosition to an allowed value
          VerticalScrollPosition = Math.Max(VerticalScrollPosition, -TotalEnabledPropertyColumns);
        }
      }
    }

    /// <summary>
    /// Handles changes in worksheet property-column data.
    /// </summary>
    public void EhPropertyDataChanged(object? sender, EventArgs e)
    {
      Current.Dispatcher.InvokeIfRequired(EhPropertyDataChanged_Unsynchronized, sender, e);
    }

    private void EhPropertyDataChanged_Unsynchronized(object? sender, EventArgs e)
    {
      if (_numberOfPropertyCols != DataTable.PropCols.ColumnCount)
        SetCachedNumberOfPropertyColumns();

      _view?.TableArea_TriggerRedrawing();
    }

    #endregion Data event handlers

    #region CellEditControl logic

    #region CellEditControl event handlers

    private void EhCellEditControl_LostFocus()
    {
      ReadCellEditContentAndHide();
    }

    private void EhCellEditControl_PreviewKeyDown(AltaxoKeyboardKey eKey, HandledEventArgs e)
    {
      if (_view is null)
        return;

      if (eKey == AltaxoKeyboardKey.Left)
      {
        // Navigate to the left if the cursor is already left
        //if(m_CellEditControl.SelectionStart==0 && (m_CellEdit_EditedCell.Row>0 || m_CellEdit_EditedCell.Column>0) )
        if (_view.CellEdit_SelectionStart == 0)
        {
          e.Handled = true;
          // Navigate to the left
          NavigateCellEdit(-1, 0);
        }
      }
      else if (eKey == AltaxoKeyboardKey.Right)
      {
        if (_view.CellEdit_SelectionStart + _view.CellEdit_SelectionLength >= _view.CellEdit_Text.Length)
        {
          e.Handled = true;
          // Navigate to the right
          NavigateCellEdit(1, 0);
        }
      }
      else if (eKey == AltaxoKeyboardKey.Up)
      {
        e.Handled = true;
        // Navigate up
        NavigateCellEdit(0, -1);
      }
      else if (eKey == AltaxoKeyboardKey.Down)
      {
        e.Handled = true;
        // Navigate down
        NavigateCellEdit(0, 1);
      }
      else if (eKey == AltaxoKeyboardKey.Enter)
      {
        // if some text is selected, deselect it and move the cursor to the end
        // else same action like keys.Down
        e.Handled = true;
        if (_view.CellEdit_SelectionLength > 0)
        {
          _view.CellEdit_SelectionLength = 0;
          _view.CellEdit_SelectionStart = _view.CellEdit_Text.Length;
        }
        else
        {
          NavigateCellEdit(0, 1);
        }
      }
      else if (eKey == AltaxoKeyboardKey.Escape)
      {
        e.Handled = true;
        HideCellEditControl();
      }
      else if (eKey == AltaxoKeyboardKey.Tab) // Tab key pressed
      {
        if (_view.CellEdit_SelectionStart + _view.CellEdit_SelectionLength >= _view.CellEdit_Text.Length)
        {
          e.Handled = true;
          // Navigate to the right
          NavigateCellEdit(1, 0);
        }
      }
    }

    private void EhCellEditControl_TextChanged()
    {
      _cellEdit_IsModified = true;
    }

    #endregion CellEditControl event handlers

    #region CellEditControl Show and Hide

    private void HideCellEditControl()
    {
      _view?.CellEdit_Hide();
      _cellEdit_IsArmed = false;
    }

    private void ShowCellEditControl()
    {
      if (_view is null)
        return;

      _view.CellEdit_Show();
      _cellEdit_IsArmed = true;
    }

    private void ReadCellEditContentAndHide()
    {
      if (_view is null)
        return;

      if (_cellEdit_IsArmed)
      {
        if (_cellEdit_IsModified)
        {
          if (_cellEdit_EditedCell.AreaType == AreaType.DataCell)
          {
            GetDataColumnStyle(_cellEdit_EditedCell.ColumnNumber).SetColumnValueAtRow(_view.CellEdit_Text, _cellEdit_EditedCell.RowNumber, DataTable[_cellEdit_EditedCell.ColumnNumber]);
          }
          else if (_cellEdit_EditedCell.AreaType == AreaType.PropertyCell)
          {
            GetPropertyColumnStyle(_cellEdit_EditedCell.ColumnNumber).SetColumnValueAtRow(_view.CellEdit_Text, _cellEdit_EditedCell.RowNumber, DataTable.PropCols[_cellEdit_EditedCell.ColumnNumber]);
          }
        }
        HideCellEditControl();
      }
    }

    private void SetCellEditContentAndShow()
    {
      if (_view is null)
        return;

      if (_cellEdit_EditedCell.AreaType == AreaType.DataCell)
      {
        _view.CellEdit_Text = GetDataColumnStyle(_cellEdit_EditedCell.ColumnNumber).GetColumnValueAtRow(_cellEdit_EditedCell.RowNumber, DataTable[_cellEdit_EditedCell.ColumnNumber]);
      }
      else if (_cellEdit_EditedCell.AreaType == AreaType.PropertyCell)
      {
        _view.CellEdit_Text = GetPropertyColumnStyle(_cellEdit_EditedCell.ColumnNumber).GetColumnValueAtRow(_cellEdit_EditedCell.RowNumber, DataTable.PropCols[_cellEdit_EditedCell.ColumnNumber]);
      }

      _view.CellEdit_SetTextAlignmentAndSelectAll(true);
      _cellEdit_IsModified = false;
      ShowCellEditControl();
    }

    #endregion CellEditControl Show and Hide

    #region CellEditControl Navigation

    /// <summary>
    /// Moves the cell editor to the next editable cell.
    /// </summary>
    /// <param name="dx">move dx cells to the right</param>
    /// <param name="dy">move dy cells down</param>
    /// <returns>True when the cell was moved to a new position, false if moving was not possible.</returns>
    protected bool NavigateCellEdit(int dx, int dy)
    {
      if (_cellEdit_EditedCell.AreaType == AreaType.DataCell)
      {
        return NavigateTableCellEdit(dx, dy);
      }
      else if (_cellEdit_EditedCell.AreaType == AreaType.PropertyCell)
      {
        return NavigatePropertyCellEdit(dx, dy);
      }
      return false;
    }

    /// <summary>
    /// Moves the cell editor to the next data cell.
    /// </summary>
    /// <param name="dx">move dx cells to the right</param>
    /// <param name="dy">move dy cells down</param>
    /// <returns>True when the cell was moved to a new position, false if moving was not possible.</returns>
    protected bool NavigateTableCellEdit(int dx, int dy)
    {
      if (_view is null)
        return false;

      bool bScrolled = false;

      // Calculate the position of the new cell
      int newCellCol = _cellEdit_EditedCell.ColumnNumber + dx;
      if (newCellCol >= DataTable.DataColumns.ColumnCount)
      {
        newCellCol = 0;
        dy += 1;
      }
      else if (newCellCol < 0)
      {
        if (_cellEdit_EditedCell.RowNumber > 0) // move to the last cell only if not on cell 0
        {
          newCellCol = DataTable.DataColumns.ColumnCount - 1;
          dy -= 1;
        }
        else
        {
          newCellCol = 0;
        }
      }

      int newCellRow = _cellEdit_EditedCell.RowNumber + dy;
      if (newCellRow < 0)
        newCellRow = 0;
      // note: we do not catch the condition newCellRow>rowCount here since we want to add new rows

      // look if the cell position has changed
      if (newCellRow == _cellEdit_EditedCell.RowNumber && newCellCol == _cellEdit_EditedCell.ColumnNumber)
        return false; // moving was not possible, so returning false, and do nothing

      // if the cell position has changed, read the old cell content
      // 1. Read content of the cell edit, if neccessary write data back
      ReadCellEditContentAndHide();

      int navigateToCol;
      int navigateToRow;

      if (newCellCol < FirstVisibleColumn)
        navigateToCol = newCellCol;
      else if (newCellCol > LastFullyVisibleColumn)
        navigateToCol = AM.GetFirstVisibleColumnForLastVisibleColumn(newCellCol, _worksheetLayout, HorzScrollPos, TableAreaWidth);
      else
        navigateToCol = FirstVisibleColumn;

      if (newCellRow < FirstVisibleTableRow)
        navigateToRow = newCellRow;
      else if (newCellRow > LastFullyVisibleTableRow)
        navigateToRow = newCellRow + 1 - FullyVisibleTableRows - FullyVisiblePropertyColumns;
      else
        navigateToRow = VerticalScrollPosition;

      if (navigateToCol != FirstVisibleColumn || navigateToRow != FirstVisibleTableRow)
      {
        SetScrollPositionTo(navigateToCol, navigateToRow);
        bScrolled = true;
      }
      // 3. Fill the cell edit control with new content
      _cellEdit_EditedCell.ColumnNumber = newCellCol;
      _cellEdit_EditedCell.RowNumber = newCellRow;
      var cellRect = AM.GetCoordinatesOfDataCell(_cellEdit_EditedCell.ColumnNumber, _cellEdit_EditedCell.RowNumber, _worksheetLayout, HorzScrollPos, VerticalScrollPosition);
      _view.CellEdit_Location = cellRect;
      SetCellEditContentAndShow();

      // 4. Invalidate the client area if scrolled in step (2)
      if (bScrolled)
        _view.TableArea_TriggerRedrawing();

      return true;
    }

    /// <summary>
    /// Moves the cell editor to the next property cell.
    /// </summary>
    /// <param name="dx">move dx cells to the right</param>
    /// <param name="dy">move dy cells down</param>
    /// <returns>True when the cell was moved to a new position, false if moving was not possible.</returns>
    protected bool NavigatePropertyCellEdit(int dx, int dy)
    {
      if (_view is null)
        return false;

      bool bScrolled = false;

      // 2. look whether the new cell coordinates lie inside the client area, if
      // not scroll the worksheet appropriate
      int newCellCol = _cellEdit_EditedCell.ColumnNumber + dy;
      if (newCellCol >= DataTable.PropCols.ColumnCount)
      {
        if (_cellEdit_EditedCell.RowNumber + 1 < DataTable.DataColumns.ColumnCount)
        {
          newCellCol = 0;
          dx += 1;
        }
        else
        {
          newCellCol = DataTable.PropCols.ColumnCount - 1;
          dx = 0;
        }
      }
      else if (newCellCol < 0)
      {
        if (_cellEdit_EditedCell.RowNumber > 0) // move to the last cell only if not on cell 0
        {
          newCellCol = DataTable.PropCols.ColumnCount - 1;
          dx -= 1;
        }
        else
        {
          newCellCol = 0;
        }
      }

      int newCellRow = _cellEdit_EditedCell.RowNumber + dx;
      if (newCellRow >= DataTable.DataColumns.ColumnCount)
      {
        if (newCellCol + 1 < DataTable.PropCols.ColumnCount) // move to the first cell only if not on the very last cell
        {
          newCellRow = 0;
          newCellCol += 1;
        }
        else // we where on the last cell
        {
          newCellRow = DataTable.DataColumns.ColumnCount - 1;
          newCellCol = DataTable.PropCols.ColumnCount - 1;
        }
      }
      else if (newCellRow < 0)
      {
        if (_cellEdit_EditedCell.ColumnNumber > 0) // move to the last cell only if not on cell 0
        {
          newCellRow = DataTable.DataColumns.ColumnCount - 1;
          newCellCol -= 1;
        }
        else
        {
          newCellRow = 0;
        }
      }

      // Fix if newCellCol is outside valid area
      if (newCellCol < 0)
        newCellCol = 0;
      else if (newCellCol >= DataTable.PropCols.ColumnCount)
        newCellCol = DataTable.PropCols.ColumnCount - 1;

      // look if the cell position has changed
      if (newCellRow == _cellEdit_EditedCell.RowNumber && newCellCol == _cellEdit_EditedCell.ColumnNumber)
        return false; // moving was not possible, so returning false, and do nothing

      // 1. Read content of the cell edit, if neccessary write data back
      ReadCellEditContentAndHide();

      int navigateToCol;
      int navigateToRow;

      if (newCellCol < FirstVisiblePropertyColumn)
        navigateToCol = newCellCol - _numberOfPropertyCols;
      else if (newCellCol > LastFullyVisiblePropertyColumn)
        navigateToCol = newCellCol - FullyVisiblePropertyColumns + 1 - _numberOfPropertyCols;
      else
        navigateToCol = VerticalScrollPosition;

      if (newCellRow < FirstVisibleColumn)
        navigateToRow = newCellRow;
      else if (newCellRow > LastFullyVisibleColumn)
        navigateToRow = AM.GetFirstVisibleColumnForLastVisibleColumn(newCellRow, _worksheetLayout, HorzScrollPos, TableAreaWidth);
      else
        navigateToRow = FirstVisibleColumn;

      if (navigateToRow != FirstVisibleColumn || navigateToCol != FirstVisibleTableRow)
      {
        SetScrollPositionTo(navigateToRow, navigateToCol);
        bScrolled = true;
      }
      // 3. Fill the cell edit control with new content
      _cellEdit_EditedCell.ColumnNumber = newCellCol;
      _cellEdit_EditedCell.RowNumber = newCellRow;
      var cellRect = AM.GetCoordinatesOfPropertyCell(_cellEdit_EditedCell.ColumnNumber, _cellEdit_EditedCell.RowNumber, _worksheetLayout, HorzScrollPos, VerticalScrollPosition);
      _view.CellEdit_Location = cellRect;
      SetCellEditContentAndShow();

      // 4. Invalidate the client area if scrolled in step (2)
      if (bScrolled)
        _view.TableArea_TriggerRedrawing();

      return true;
    }

    #endregion CellEditControl Navigation

    #endregion CellEditControl logic

    #region Shortcuts for number and position of rows and columns

    #region Data rows

    /// <summary>
    /// Gets the index of the first visible table row.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FirstVisibleTableRow
    {
      get
      {
        return AM.GetFirstVisibleTableRow(_worksheetLayout, VerticalScrollPosition);
      }
    }

    /// <summary>
    /// This returns the vertical position of the first visible data row.;
    /// </summary>
    public int VerticalPositionOfFirstVisibleDataRow
    {
      get
      {
        return AM.GetVerticalPositionOfFirstVisibleDataRow(_worksheetLayout, VerticalScrollPosition);
      }
    }

    /// <summary>
    /// Gets the number of visible table rows.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int VisibleTableRows
    {
      get
      {
        return AM.GetVisibleTableRows(0, TableAreaHeight, _worksheetLayout, VerticalScrollPosition);
      }
    }

    /// <summary>
    /// Gets the number of fully visible table rows.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FullyVisibleTableRows
    {
      get
      {
        return AM.GetFullyVisibleTableRows(0, TableAreaHeight, _worksheetLayout, VerticalScrollPosition);
      }
    }

    /// <summary>
    /// Gets the index of the last visible table row.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LastVisibleTableRow
    {
      get
      {
        return AM.GetLastVisibleTableRow(_worksheetLayout, VerticalScrollPosition, TableAreaHeight);
      }
    }

    /// <summary>
    /// Gets the index of the last fully visible table row.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LastFullyVisibleTableRow
    {
      get
      {
        return AM.GetLastFullyVisibleTableRow(_worksheetLayout, VerticalScrollPosition, TableAreaHeight);
      }
    }

    #endregion Data rows

    #region Property columns

    /// <summary>Returns the remaining number of property columns that could be shown below the current scroll position.</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int RemainingEnabledPropertyColumns
    {
      get
      {
        return AM.GetRemainingEnabledPropertyColumns(_worksheetLayout, VerticalScrollPosition);
      }
    }

    /// <summary>Returns number of property columns that are enabled for been shown on the grid.</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int TotalEnabledPropertyColumns
    {
      get
      {
        return AM.GetTotalEnabledPropertyColumns(_worksheetLayout);
      }
    }

    /// <summary>
    /// Gets the index of the first visible property column.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FirstVisiblePropertyColumn
    {
      get
      {
        return AM.GetFirstVisiblePropertyColumn(_worksheetLayout, VerticalScrollPosition);
      }
    }

    /// <summary>
    /// Gets the index of the last fully visible property column.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LastFullyVisiblePropertyColumn
    {
      get
      {
        return AM.GetLastFullyVisiblePropertyColumn(_worksheetLayout, VerticalScrollPosition, TableAreaHeight);
      }
    }

    /// <summary>
    /// Gets the number of visible property columns.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int VisiblePropertyColumns
    {
      get
      {
        return AM.GetVisiblePropertyColumns(0, TableAreaHeight, _worksheetLayout, VerticalScrollPosition);
      }
    }

    /// <summary>
    /// Gets the number of fully visible property columns.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FullyVisiblePropertyColumns
    {
      get
      {
        return AM.GetFullyVisiblePropertyColumns(0, TableAreaHeight, _worksheetLayout, VerticalScrollPosition);
      }
    }

    #endregion Property columns

    #region Data columns

    /// <summary>
    /// Gets or sets the index of the first visible data column.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FirstVisibleColumn
    {
      get
      {
        return HorzScrollPos;
      }
      set
      {
        HorzScrollPos = value;
      }
    }

    /// <summary>
    /// Gets the number of visible data columns.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int VisibleColumns
    {
      get
      {
        return AM.GetVisibleColumns(_worksheetLayout, HorzScrollPos, TableAreaWidth);
      }
    }

    /// <summary>
    /// Gets the number of fully visible data columns.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FullyVisibleColumns
    {
      get
      {
        return AM.GetFullyVisibleColumns(_worksheetLayout, HorzScrollPos, TableAreaWidth);
      }
    }

    /// <summary>
    /// Gets the index of the last fully visible data column.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int LastFullyVisibleColumn
    {
      get
      {
        return AM.GetLastFullyVisibleColumn(_worksheetLayout, HorzScrollPos, TableAreaWidth);
      }
    }

    #endregion Data columns

    #endregion Shortcuts for number and position of rows and columns

    #region Scrolling logic

    /// <summary>
    /// Gets or sets the horizontal scroll position.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int HorzScrollPos
    {
      get { return _scrollHorzPos; }
      set
      {
        int oldValue = _scrollHorzPos;
        _scrollHorzPos = value;

        if (value != oldValue)
        {
          if (_cellEdit_IsArmed)
          {
            ReadCellEditContentAndHide();
          }

          if (_view is not null)
          {
            _view.TableViewHorzScrollValue = value;
            _view?.TableArea_TriggerRedrawing();
          }
        }
      }
    }

    /// <summary>
    /// The vertical scroll position is defined as following:
    /// If 0 (zero), the data row 0 is the first visible line (after the column header).
    /// If positive, the data row with the number of VertScrollPos is the first visible row.
    /// If negative, the property column with index PropertyColumnCount+VertScrollPos is the first visible line.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int VerticalScrollPosition
    {
      get { return _scrollVertPos; }
      set
      {
        int oldValue = _scrollVertPos;
        int newValue = value;
        newValue = Math.Min(_scrollVertMax, newValue);
        newValue = Math.Max(-TotalEnabledPropertyColumns, newValue);
        _scrollVertPos = newValue;

        if (newValue != oldValue)
        {
          if (_cellEdit_IsArmed)
          {
            ReadCellEditContentAndHide();
          }

          // The value of the ScrollBar in the view has an offset, since he
          // can not have negative values;
          if (_view is not null)
          {
            newValue += TotalEnabledPropertyColumns;
            _view.TableViewVertScrollValue = newValue;
            _view.TableViewVertViewPortSize = AM.GetVisibleTableRows(0, TableAreaHeight, _worksheetLayout, newValue) + AM.GetVisiblePropertyColumns(0, TableAreaHeight, _worksheetLayout, newValue);
            _view?.TableArea_TriggerRedrawing();
          }
        }
      }
    }

    /// <summary>
    /// Gets or sets the maximum horizontal scroll position.
    /// </summary>
    public int HorzScrollMaximum
    {
      get { return _scrollHorzMax; }
      set
      {
        _scrollHorzMax = value;
        if (_view is not null)
          _view.TableViewHorzScrollMaximum = value;
      }
    }

    /// <summary>
    /// Gets or sets the maximum vertical scroll position.
    /// </summary>
    public int VertScrollMaximum
    {
      get { return _scrollVertMax; }
      set
      {
        _scrollVertMax = value;

        if (_view is not null)
          _view.TableViewVertScrollMaximum = value + TotalEnabledPropertyColumns;
      }
    }

    /// <summary>
    /// SetScrollPositions only sets the scroll positions, and not Invalidates the
    /// Area!
    /// </summary>
    /// <param name="nCol">first visible column (i.e. column at the left)</param>
    /// <param name="nRow">first visible row (i.e. row at the top)</param>
    protected void SetScrollPositionTo(int nCol, int nRow)
    {
      int oldCol = HorzScrollPos;
      if (HorzScrollMaximum < nCol)
        HorzScrollMaximum = nCol;
      HorzScrollPos = nCol;

      if (VertScrollMaximum < nRow)
        VertScrollMaximum = nRow;
      VerticalScrollPosition = nRow;
    }

    #endregion Scrolling logic

    #region IWorksheetController Members

    /// <summary>
    /// Rebuilds the cached list of column-divider positions used for column resizing.
    /// </summary>
    public void CreateResizingPositions()
    {
      _columnWidthResizingPositionsFirstColumnIndex = AM.GetColumnWidthResizingPositions(_columnWidthResizingPositions, _worksheetLayout, HorzScrollPos, TableAreaWidth);
    }

    /// <summary>
    /// Handles scrolling of the vertical scrollbar in the view.
    /// </summary>
    public void EhView_VertScrollBarScroll(int newScrollValue)
    {
      VerticalScrollPosition = newScrollValue - TotalEnabledPropertyColumns;
    }

    /// <summary>
    /// Handles scrolling of the horizontal scrollbar in the view.
    /// </summary>
    public void EhView_HorzScrollBarScroll(int newScrollValue)
    {
      //if (e.ScrollEventType != System.Windows.Controls.Primitives.ScrollEventType.ThumbTrack)
      HorzScrollPos = newScrollValue;
    }

    /// <summary>
    /// Handles mouse-button release over the table area.
    /// </summary>
    public void EhView_TableAreaMouseUp(PointD2D position)
    {
      if (_view is null)
        return;

      if (_dragColumnWidth_InCapture)
      {
        double sizediff = position.X - _dragColumnWidth_OriginalPos;
        Altaxo.Worksheet.ColumnStyle? cs;
        if (-1 == _dragColumnWidth_ColumnNumber)
        {
          cs = _worksheetLayout.RowHeaderStyle;
        }
        else
        {
          if (!_worksheetLayout.DataColumnStyles.TryGetValue(DataTable[_dragColumnWidth_ColumnNumber], out cs))
          {
            Altaxo.Worksheet.ColumnStyle template = GetDataColumnStyle(_dragColumnWidth_ColumnNumber);
            cs = (Altaxo.Worksheet.ColumnStyle)template.Clone();
            _worksheetLayout.DataColumnStyles.Add(DataTable[_dragColumnWidth_ColumnNumber], cs);
          }
        }
        double newWidth = _dragColumnWidth_OriginalWidth + sizediff;
        if (newWidth < 10)
          newWidth = 10;
        cs.WidthD = newWidth;

        _dragColumnWidth_InCapture = false;
        _dragColumnWidth_ColumnNumber = int.MinValue;
        _view.TableArea_IsCaptured = false;
        _view.Cursor_SetToArrow();
        _view.TableArea_TriggerRedrawing();
      }
    }

    /// <summary>
    /// Handles mouse-button presses over the table area.
    /// </summary>
    public void EhView_TableAreaMouseDown(PointD2D position)
    {
      if (_view is null)
        return;

      // base.OnMouseDown(e);
      _mouseDownPosition = position;
      ReadCellEditContentAndHide();

      if (_dragColumnWidth_ColumnNumber >= -1)
      {
        _view.TableArea_IsCaptured = true;
        _dragColumnWidth_OriginalPos = position.X;
        _dragColumnWidth_InCapture = true;
      }
    }

    /// <summary>
    /// Handles the mouse wheel event.
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="mouseDelta">Delta value of the mouse wheel.</param>
    public void EhView_TableAreaMouseWheel(PointD2D position, int mouseDelta)
    {
      int oldScrollPos = VerticalScrollPosition;
      VerticalScrollPosition = VerticalScrollPosition - mouseDelta / 120;
    }

    /// <summary>
    /// Handles mouse movement over the table area.
    /// </summary>
    public void EhView_TableAreaMouseMove(PointD2D position)
    {
      if (_view is null)
        return;

      var Y = position.Y;
      var X = position.X;

      if (_dragColumnWidth_InCapture)
      {
        var sizediff = X - _dragColumnWidth_OriginalPos;

        Altaxo.Worksheet.ColumnStyle? cs;
        if (-1 == _dragColumnWidth_ColumnNumber)
          cs = _worksheetLayout.RowHeaderStyle;
        else
        {
          if (!_worksheetLayout.DataColumnStyles.TryGetValue(DataTable[_dragColumnWidth_ColumnNumber], out cs))
          {
            Altaxo.Worksheet.ColumnStyle template = GetDataColumnStyle(_dragColumnWidth_ColumnNumber);
            cs = (Altaxo.Worksheet.ColumnStyle)template.Clone();
            _worksheetLayout.DataColumnStyles.Add(DataTable[_dragColumnWidth_ColumnNumber], cs);
          }
        }

        var newWidth = _dragColumnWidth_OriginalWidth + sizediff;
        if (newWidth < 10)
          newWidth = 10;
        cs.WidthD = newWidth;
        _view?.TableArea_TriggerRedrawing();
      }
      else // not in Capture mode
      {
        if (Y < _worksheetLayout.ColumnHeaderStyle.Height)
        {
          for (int i = _columnWidthResizingPositions.Count - 1; i >= 0; i--)
          {
            double pos = _columnWidthResizingPositions[i];

            if (pos - 5 < X && X < pos + 5)
            {
              _view.Cursor_SetToResizeWestEast();
              _dragColumnWidth_ColumnNumber = i + FirstVisibleColumn;
              _dragColumnWidth_OriginalWidth = _worksheetLayout.DataColumnStyles[DataTable[_dragColumnWidth_ColumnNumber]].WidthD;
              return;
            }
          } // end for

          if (_worksheetLayout.RowHeaderStyle.Width - 5 < X && X < _worksheetLayout.RowHeaderStyle.Width + 5)
          {
            _view.Cursor_SetToResizeWestEast();
            _dragColumnWidth_ColumnNumber = -1;
            _dragColumnWidth_OriginalWidth = _worksheetLayout.RowHeaderStyle.Width;
            return;
          }
        }

        _dragColumnWidth_ColumnNumber = int.MinValue;
        _view.Cursor_SetToArrow();
      } // end else
    }

    #region MouseClick functions

    /// <summary>
    /// Handles a left-click on a data cell.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    protected virtual void OnLeftClickDataCell(AreaInfo clickedCell)
    {
      if (_view is null)
        return;

      _cellEdit_EditedCell = clickedCell;
      _view.CellEdit_Location = clickedCell.AreaRectangle;
      SetCellEditContentAndShow();
    }

    /// <summary>
    /// Handles a left-click on a property cell.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    protected virtual void OnLeftClickPropertyCell(AreaInfo clickedCell)
    {
      if (_view is null)
        return;

      _cellEdit_EditedCell = clickedCell;
      _view.CellEdit_Location = clickedCell.AreaRectangle;
      SetCellEditContentAndShow();
    }

    /// <summary>
    /// Handles a left-click on a data-column header.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    /// <param name="modifierKeys">The keyboard modifier keys pressed during the click.</param>
    protected virtual void OnLeftClickDataColumnHeader(AreaInfo clickedCell, AltaxoKeyboardModifierKeys modifierKeys)
    {
      if (_view is null)
        return;

      if (!_dragColumnWidth_InCapture)
      {
        bool bControlKey = modifierKeys.HasFlag(AltaxoKeyboardModifierKeys.Control); // Control pressed
        bool bShiftKey = modifierKeys.HasFlag(AltaxoKeyboardModifierKeys.Shift);

        bool bWasSelectedBefore = SelectedDataColumns.IsSelected(clickedCell.ColumnNumber);

        /*
        if(m_LastSelectionType==SelectionType.DataRowSelection && !bControlKey)
            m_SelectedRows.Clear(); // if we click a column, we remove row selections
        */

        if ((!bControlKey && !bShiftKey) || (_lastSelectionType != SelectionType.DataColumnSelection && _lastSelectionType != SelectionType.PropertyRowSelection && !bControlKey))
        {
          _selectedDataColumns.Clear();
          _selectedDataRows.Clear(); // if we click a column, we remove row selections
          _selectedPropertyColumns.Clear();
          _selectedPropertyRows.Clear();
        }

        if (_lastSelectionType == SelectionType.PropertyRowSelection)
        {
          _selectedPropertyRows.Select(clickedCell.ColumnNumber, bShiftKey, bControlKey);
          _lastSelectionType = SelectionType.PropertyRowSelection;
        }
        // if the last selection has only selected any property cells then add the current selection to the property rows
        else if (!AreDataCellsSelected && ArePropertyCellsSelected && bControlKey)
        {
          _selectedPropertyRows.Select(clickedCell.ColumnNumber, bShiftKey, bControlKey);
          _lastSelectionType = SelectionType.PropertyRowSelection;
        }
        else
        {
          if (SelectedDataColumns.Count != 0 || !bWasSelectedBefore)
            _selectedDataColumns.Select(clickedCell.ColumnNumber, bShiftKey, bControlKey);
          _lastSelectionType = SelectionType.DataColumnSelection;
        }

        _view.TableArea_TriggerRedrawing();
      }
    }

    /// <summary>
    /// Handles a left-click on a data-row header.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    /// <param name="modifierKeys">The keyboard modifier keys pressed during the click.</param>
    protected virtual void OnLeftClickDataRowHeader(AreaInfo clickedCell, AltaxoKeyboardModifierKeys modifierKeys)
    {
      if (_view is null)
        return;

      bool bControlKey = modifierKeys.HasFlag(AltaxoKeyboardModifierKeys.Control); // Control pressed
      bool bShiftKey = modifierKeys.HasFlag(AltaxoKeyboardModifierKeys.Shift);

      bool bWasSelectedBefore = SelectedDataRows.IsSelected(clickedCell.RowNumber);

      /*
        if(m_LastSelectionType==SelectionType.DataColumnSelection && !bControlKey)
            m_SelectedColumns.Clear(); // if we click a column, we remove row selections
        */
      if ((!bControlKey && !bShiftKey) || (_lastSelectionType != SelectionType.DataRowSelection && !bControlKey))
      {
        _selectedDataColumns.Clear(); // if we click a column, we remove row selections
        _selectedDataRows.Clear();
        _selectedPropertyColumns.Clear();
        _selectedPropertyRows.Clear();
      }

      // if we had formerly selected property rows, we clear them but add them before as column selection
      if (_selectedPropertyRows.Count > 0)
      {
        if (_selectedDataColumns.Count == 0)
        {
          for (int kk = 0; kk < _selectedPropertyRows.Count; kk++)
            _selectedDataColumns.Add(_selectedPropertyRows[kk]);
        }
        _selectedPropertyRows.Clear();
      }

      if (SelectedDataRows.Count != 0 || !bWasSelectedBefore)
        _selectedDataRows.Select(clickedCell.RowNumber, bShiftKey, bControlKey);
      _lastSelectionType = SelectionType.DataRowSelection;
      _view.TableArea_TriggerRedrawing();
    }

    /// <summary>
    /// Handles a left-click on a property-column header.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    /// <param name="modifierKeys">The keyboard modifier keys pressed during the click.</param>
    protected virtual void OnLeftClickPropertyColumnHeader(AreaInfo clickedCell, AltaxoKeyboardModifierKeys modifierKeys)
    {
      if (_view is null)
        return;

      bool bControlKey = modifierKeys.HasFlag(AltaxoKeyboardModifierKeys.Control); // Control pressed
      bool bShiftKey = modifierKeys.HasFlag(AltaxoKeyboardModifierKeys.Shift);

      bool bWasSelectedBefore = SelectedPropertyColumns.IsSelected(clickedCell.ColumnNumber);

      if ((!bControlKey && !bShiftKey) || (_lastSelectionType != SelectionType.PropertyColumnSelection && !bControlKey))
      {
        _selectedDataColumns.Clear();
        _selectedDataRows.Clear(); // if we click a column, we remove row selections
        _selectedPropertyColumns.Clear();
        _selectedPropertyRows.Clear();
      }

      if (SelectedPropertyColumns.Count != 0 || !bWasSelectedBefore)
        _selectedPropertyColumns.Select(clickedCell.ColumnNumber, bShiftKey, bControlKey);

      _lastSelectionType = SelectionType.PropertyColumnSelection;
      _view.TableArea_TriggerRedrawing();
    }

    /// <summary>
    /// Occurs when the table header is clicked with the left mouse button.
    /// </summary>
    [field: NonSerialized]
    public event Action<object, AreaInfo>? TableHeaderLeftClicked;

    /// <summary>
    /// Raises the event for a left-click on the table header.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    protected virtual void OnLeftClickTableHeader(AreaInfo clickedCell)
    {
      TableHeaderLeftClicked?.Invoke(this, clickedCell);
    }

    /// <summary>
    /// Occurs when the user left-clicks outside any worksheet area.
    /// </summary>
    [field: NonSerialized]
    public event Action<object, AreaInfo>? OutsideAllLeftClicked;

    /// <summary>
    /// Raises the event for a left-click outside all worksheet areas.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    protected virtual void OnLeftClickOutsideAll(AreaInfo clickedCell)
    {
      OutsideAllLeftClicked?.Invoke(this, clickedCell);
    }

    /// <summary>
    /// Occurs when a data cell is clicked with the right mouse button.
    /// </summary>
    [field: NonSerialized]
    public event Action<object, AreaInfo>? DataCellRightClicked;

    /// <summary>
    /// Raises the event for a right-click on a data cell.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    protected virtual void OnRightClickDataCell(AreaInfo clickedCell)
    {
      DataCellRightClicked?.Invoke(this, clickedCell);
    }

    /// <summary>
    /// Occurs when a property cell is clicked with the right mouse button.
    /// </summary>
    [field: NonSerialized]
    public event Action<object, AreaInfo>? PropertyCellRightClicked;

    /// <summary>
    /// Raises the event for a right-click on a property cell.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    protected virtual void OnRightClickPropertyCell(AreaInfo clickedCell)
    {
      PropertyCellRightClicked?.Invoke(this, clickedCell);
    }

    /// <summary>
    /// Occurs when a data-column header is clicked with the right mouse button.
    /// </summary>
    [field: NonSerialized]
    public event Action<object, AreaInfo>? DataColumnHeaderRightClicked;

    /// <summary>
    /// Raises the event for a right-click on a data-column header and shows the context menu.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    protected virtual void OnRightClickDataColumnHeader(AreaInfo clickedCell)
    {
      if (_view is null)
        return;

      DataColumnHeaderRightClicked?.Invoke(this, clickedCell);

      if (!(SelectedDataColumns.Contains(clickedCell.ColumnNumber)) &&
              !(SelectedPropertyRows.Contains(clickedCell.ColumnNumber)))
      {
        ClearAllSelections();
        SelectedDataColumns.Add(clickedCell.ColumnNumber);
        _view.TableArea_TriggerRedrawing();
      }
      Current.Gui.ShowContextMenu(_view, _view, "/Altaxo/Views/Worksheet/DataColumnHeader/ContextMenu", clickedCell.AreaRectangle.X, clickedCell.AreaRectangle.Y);
    }

    /// <summary>
    /// Occurs when a data-row header is clicked with the right mouse button.
    /// </summary>
    [field: NonSerialized]
    public event Action<object, AreaInfo>? DataRowHeaderRightClicked;

    /// <summary>
    /// Raises the event for a right-click on a data-row header and shows the context menu.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    protected virtual void OnRightClickDataRowHeader(AreaInfo clickedCell)
    {
      if (_view is null)
        return;

      if (DataRowHeaderRightClicked is not null)
        DataRowHeaderRightClicked(this, clickedCell);

      if (!(SelectedDataRows.Contains(clickedCell.RowNumber)))
      {
        ClearAllSelections();
        SelectedDataRows.Add(clickedCell.RowNumber);
        _view.TableArea_TriggerRedrawing();
      }
      Current.Gui.ShowContextMenu(_view, _view, "/Altaxo/Views/Worksheet/DataRowHeader/ContextMenu", clickedCell.AreaRectangle.X, clickedCell.AreaRectangle.Y);
    }

    /// <summary>
    /// Occurs when a property-column header is clicked with the right mouse button.
    /// </summary>
    [field: NonSerialized]
    public event Action<object, AreaInfo>? PropertyColumnHeaderRightClicked;

    /// <summary>
    /// Raises the event for a right-click on a property-column header and shows the context menu.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    protected virtual void OnRightClickPropertyColumnHeader(AreaInfo clickedCell)
    {
      if (_view is null)
        return;

      PropertyColumnHeaderRightClicked?.Invoke(this, clickedCell);

      if (!(SelectedPropertyColumns.Contains(clickedCell.ColumnNumber)))
      {
        ClearAllSelections();
        SelectedPropertyColumns.Add(clickedCell.ColumnNumber);
        _view.TableArea_TriggerRedrawing();
      }
      Current.Gui.ShowContextMenu(_view, _view, "/Altaxo/Views/Worksheet/PropertyColumnHeader/ContextMenu", clickedCell.AreaRectangle.X, clickedCell.AreaRectangle.Y);
    }

    /// <summary>
    /// Occurs when the table header is clicked with the right mouse button.
    /// </summary>
    [field: NonSerialized]
    public event Action<object, AreaInfo>? TableHeaderRightClicked;

    /// <summary>
    /// Raises the event for a right-click on the table header and shows the context menu.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    protected virtual void OnRightClickTableHeader(AreaInfo clickedCell)
    {
      if (_view is null)
        return;

      TableHeaderRightClicked?.Invoke(this, clickedCell);

      Current.Gui.ShowContextMenu(_view, _view, "/Altaxo/Views/Worksheet/DataTableHeader/ContextMenu", clickedCell.AreaRectangle.X, clickedCell.AreaRectangle.Y);
    }

    /// <summary>
    /// Occurs when the user right-clicks outside any worksheet area.
    /// </summary>
    [field: NonSerialized]
    public event Action<object, AreaInfo>? OutsideAllRightClicked;

    /// <summary>
    /// Raises the event for a right-click outside all worksheet areas and shows the context menu.
    /// </summary>
    /// <param name="clickedCell">Information about the clicked cell.</param>
    protected virtual void OnRightClickOutsideAll(AreaInfo clickedCell)
    {
      if (_view is null)
        return;

      OutsideAllRightClicked?.Invoke(this, clickedCell);

      Current.Gui.ShowContextMenu(_view, _view, "/Altaxo/Views/Worksheet/OutsideAll/ContextMenu", clickedCell.AreaRectangle.X, clickedCell.AreaRectangle.Y);
    }

    #endregion MouseClick functions

    /// <summary>
    /// Handles mouse clicks in the table area.
    /// </summary>
    public void EhView_TableAreaMouseClick(PointD2D position, AltaxoMouseButtons eButton, AltaxoKeyboardModifierKeys modifierKeys)
    {
      var _mouseInfo = AM.GetAreaType(position.X, position.Y, _worksheetLayout, HorzScrollPos, VerticalScrollPosition);

      //ClickedCellInfoWpf clickedCell = new ClickedCellInfoWpf(this,this.m_MouseDownPosition);

      if (eButton == AltaxoMouseButtons.Left)
      {
        switch (_mouseInfo.AreaType)
        {
          case AreaType.DataCell:
            OnLeftClickDataCell(_mouseInfo);
            break;

          case AreaType.PropertyCell:
            OnLeftClickPropertyCell(_mouseInfo);
            break;

          case AreaType.PropertyColumnHeader:
            OnLeftClickPropertyColumnHeader(_mouseInfo, modifierKeys);
            break;

          case AreaType.DataColumnHeader:
            OnLeftClickDataColumnHeader(_mouseInfo, modifierKeys);
            break;

          case AreaType.DataRowHeader:
            OnLeftClickDataRowHeader(_mouseInfo, modifierKeys);
            break;

          case AreaType.TableHeader:
            OnLeftClickTableHeader(_mouseInfo);
            break;

          case AreaType.OutsideAll:
            OnLeftClickOutsideAll(_mouseInfo);
            break;
        }
      }
      else if (eButton == AltaxoMouseButtons.Right)
      {
        switch (_mouseInfo.AreaType)
        {
          case AreaType.DataCell:
            OnRightClickDataCell(_mouseInfo);
            break;

          case AreaType.PropertyCell:
            OnRightClickPropertyCell(_mouseInfo);
            break;

          case AreaType.PropertyColumnHeader:
            OnRightClickPropertyColumnHeader(_mouseInfo);
            break;

          case AreaType.DataColumnHeader:
            OnRightClickDataColumnHeader(_mouseInfo);
            break;

          case AreaType.DataRowHeader:
            OnRightClickDataRowHeader(_mouseInfo);
            break;

          case AreaType.TableHeader:
            OnRightClickTableHeader(_mouseInfo);
            break;

          case AreaType.OutsideAll:
            OnRightClickOutsideAll(_mouseInfo);
            break;
        }
      }
    }

    /// <summary>
    /// Handles mouse double-clicks in the table area.
    /// </summary>
    public void EhView_TableAreaMouseDoubleClick(PointD2D position)
    {
      // TODO:  Add WorksheetController.EhView_TableAreaMouseDoubleClick implementation
    }

    private void TableArea_TriggerRedrawing()
    {
      if (_view is null)
        return;

      _view.TableArea_TriggerRedrawing();
    }

    /// <summary>
    /// Handles changes in the size of the table area.
    /// </summary>
    public void EhView_TableAreaSizeChanged(EventArgs e)
    {
      _view?.TableArea_TriggerRedrawing();
    }

    #endregion IWorksheetController Members

    #region ClipboardHandler Members

    /// <summary>
    /// Gets a value indicating whether the cut command is currently enabled.
    /// </summary>
    public bool EnableCut
    {
      get { return _cellEdit_IsArmed; }
    }

    /// <summary>
    /// Gets a value indicating whether the copy command is currently enabled.
    /// </summary>
    public bool EnableCopy
    {
      get { return true; }
    }

    /// <summary>
    /// Gets a value indicating whether the paste command is currently enabled.
    /// </summary>
    public bool EnablePaste
    {
      get { return true; }
    }

    /// <summary>
    /// Gets a value indicating whether the delete command is currently enabled.
    /// </summary>
    public bool EnableDelete
    {
      get { return !_cellEdit_IsArmed; }
    }

    /// <summary>
    /// Gets a value indicating whether the select-all command is currently enabled.
    /// </summary>
    public bool EnableSelectAll
    {
      get { return true; }
    }

    /// <summary>
    /// Cuts the current cell-edit selection or the selected worksheet items.
    /// </summary>
    public void Cut()
    {
      if (_view is not null && _cellEdit_IsArmed)
      {
        _view.CellEdit_Cut();
      }
      else if (AreColumnsOrRowsSelected)
      {
        // Copy the selected Columns to the clipboard
        EditCommands.CopyToClipboard(this);
      }
    }

    /// <summary>
    /// Copies the current cell-edit selection or the selected worksheet items.
    /// </summary>
    public void Copy()
    {
      if (_view is not null && _cellEdit_IsArmed)
      {
        _view.CellEdit_Copy();
      }
      else if (AreColumnsOrRowsSelected)
      {
        // Copy the selected Columns to the clipboard
        EditCommands.CopyToClipboard(this);
      }
    }

    /// <summary>
    /// Pastes clipboard content into the active cell editor or worksheet selection.
    /// </summary>
    public void Paste()
    {
      if (_view is not null && _cellEdit_IsArmed)
      {
        _view.CellEdit_Paste();
      }
      else
      {
        EditCommands.PasteFromClipboard(this);
      }
    }

    /// <summary>
    /// Deletes the current cell-edit content, the current selection, or the worksheet itself.
    /// </summary>
    public void Delete()
    {
      if (_view is not null && _cellEdit_IsArmed)
      {
        _view.CellEdit_Clear();
      }
      else if (AreColumnsOrRowsSelected)
      {
        EditCommands.RemoveSelected(this);
      }
      else
      {
        // nothing is selected, we assume that the user wants to delete the worksheet itself
        Current.ProjectService.DeleteTable(DataTable, false);
      }
    }

    /// <summary>
    /// Selects all data columns in the worksheet.
    /// </summary>
    public void SelectAll()
    {
      if (DataTable.DataColumns.ColumnCount > 0)
      {
        SelectedDataColumns.Select(0, false, false);
        SelectedDataColumns.Select(DataTable.DataColumns.ColumnCount - 1, true, false);
        if (_view is not null)
          _view.TableArea_TriggerRedrawing();
      }
    }

    #endregion ClipboardHandler Members

    /// <summary>
    /// Handles keyboard navigation commands from the view.
    /// </summary>
    public void EhView_KeyDown(AltaxoKeyboardKey eKey)
    {
      switch (eKey)
      {
        case AltaxoKeyboardKey.PageDown: // Page-Down-Key
          VerticalScrollPosition = VerticalScrollPosition + VisibleTableRows;
          break;

        case AltaxoKeyboardKey.PageUp: // Page-Up-Key
          VerticalScrollPosition = VerticalScrollPosition - VisibleTableRows;
          break;

        case AltaxoKeyboardKey.Home:
          HorzScrollPos = 0;
          break;

        case AltaxoKeyboardKey.End:
          HorzScrollPos = HorzScrollMaximum;
          break;
      }
    }
  }
}
