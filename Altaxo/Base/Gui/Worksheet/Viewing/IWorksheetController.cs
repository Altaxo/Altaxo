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
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Geometry;
using Altaxo.Worksheet;

namespace Altaxo.Gui.Worksheet.Viewing
{
  /// <summary>
  /// Provides the view contract for worksheet views.
  /// </summary>
  public interface IWorksheetView
  {
    /// <summary>
    /// Sets the controller of the view.
    /// </summary>
    Altaxo.Gui.Worksheet.Viewing.IWorksheetController? Controller { set; }

    /// <summary>
    /// Sets the cursor to the default arrow shape.
    /// </summary>
    void Cursor_SetToArrow();

    /// <summary>
    /// Sets the cursor to the horizontal resize shape.
    /// </summary>
    void Cursor_SetToResizeWestEast();

    /// <summary>
    /// Requests a redraw of the table area.
    /// </summary>
    void TableArea_TriggerRedrawing();

    /// <summary>
    /// Sets a value indicating whether the table area has captured the mouse.
    /// </summary>
    bool TableArea_IsCaptured { set; }

    /// <summary>
    /// Gets the current size of the table area.
    /// </summary>
    PointD2D TableArea_Size { get; }

    /// <summary>
    /// Returns the control that should be focused initially.
    /// </summary>
    object GuiInitiallyFocusedElement { get; }

    /// <summary>
    /// Sets the horizontal viewport size of the table view.
    /// </summary>
    double TableViewHorzViewPortSize { set; }

    /// <summary>
    /// Sets the vertical viewport size of the table view.
    /// </summary>
    double TableViewVertViewPortSize { set; }

    /// <summary>
    /// Sets the horizontal scroll position of the table view.
    /// </summary>
    double TableViewHorzScrollValue { set; }

    /// <summary>
    /// Sets the vertical scroll position of the table view.
    /// </summary>
    double TableViewVertScrollValue { set; }

    /// <summary>
    /// Sets the maximum horizontal scroll value of the table view.
    /// </summary>
    double TableViewHorzScrollMaximum { set; }

    /// <summary>
    /// Sets the maximum vertical scroll value of the table view.
    /// </summary>
    double TableViewVertScrollMaximum { set; }

    #region Cell edit

    /// <summary>
    /// Occurs when the cell editor loses focus.
    /// </summary>
    event Action CellEdit_LostFocus;

    /// <summary>
    /// Occurs when a key is pressed while previewing input in the cell editor.
    /// </summary>
    event Action<AltaxoKeyboardKey, HandledEventArgs> CellEdit_PreviewKeyPressed;

    /// <summary>
    /// Occurs when the cell editor text changes.
    /// </summary>
    event Action CellEdit_TextChanged;

    /// <summary>
    /// Gets or sets the selection start within the cell editor.
    /// </summary>
    int CellEdit_SelectionStart { get; set; }

    /// <summary>
    /// Gets or sets the selection length within the cell editor.
    /// </summary>
    int CellEdit_SelectionLength { get; set; }

    /// <summary>
    /// Gets or sets the text of the cell editor.
    /// </summary>
    string CellEdit_Text { get; set; }

    /// <summary>
    /// Cuts the current cell editor selection.
    /// </summary>
    void CellEdit_Cut();

    /// <summary>
    /// Copies the current cell editor selection.
    /// </summary>
    void CellEdit_Copy();

    /// <summary>
    /// Pastes clipboard contents into the cell editor.
    /// </summary>
    void CellEdit_Paste();

    /// <summary>
    /// Clears the current cell editor contents.
    /// </summary>
    void CellEdit_Clear();

    /// <summary>
    /// Hides the cell editor.
    /// </summary>
    void CellEdit_Hide();

    /// <summary>
    /// Shows the cell editor.
    /// </summary>
    void CellEdit_Show();

    /// <summary>
    /// Sets the text alignment of the cell editor and selects all text.
    /// </summary>
    void CellEdit_SetTextAlignmentAndSelectAll(bool textAlignmentRight);

    /// <summary>
    /// Sets the location of the cell editor.
    /// </summary>
    RectangleD2D CellEdit_Location { set; }

    #endregion Cell edit
  }

  /// <summary>
  /// Provides the controller contract for worksheet views.
  /// </summary>
  public interface IWorksheetController : IMVCANController
  {
    /// <summary>
    /// This returns the Table that is managed by this controller.
    /// </summary>
    Altaxo.Data.DataTable DataTable { get; }

    /// <summary>
    /// Forces redraw of the table.
    /// </summary>
    void TableAreaInvalidate();

    /// <summary>
    /// Gets the worksheet layout managed by this controller.
    /// </summary>
    WorksheetLayout WorksheetLayout { get; }

    /// <summary>
    /// Returns the currently selected data columns.
    /// </summary>
    IndexSelection SelectedDataColumns { get; }

    /// <summary>
    /// Returns the currently selected data rows.
    /// </summary>
    IndexSelection SelectedDataRows { get; }

    /// <summary>
    /// Returns the currently selected property columns.
    /// </summary>
    IndexSelection SelectedPropertyColumns { get; }

    /// <summary>
    /// Returns the currently selected property rows if property cells are selected alone. Otherwise, the <c>SelectedColumns</c> property is returned.
    /// </summary>
    /// <remarks>Normally, if you select one or more data columns, the corresponding property rows are selected as well. Therefore, it would not be possible to select property rows without also selecting the
    /// data columns. To work around this, you can first select property columns and then columns. In this case, the selection is not stored in
    /// <c>SelectedColumns</c>, but in <c>SelectedPropertyRows</c>, and <c>SelectedColumns.Count</c> returns 0.</remarks>
    IndexSelection SelectedPropertyRows { get; }

    /// <summary>
    /// Returns true if one or more property columns or rows are selected.
    /// </summary>
    bool ArePropertyCellsSelected { get; }

    /// <summary>
    /// Returns true if one or more data columns or rows are selected.
    /// </summary>
    bool AreDataCellsSelected { get; }

    /// <summary>
    /// Returns true if one or more columns, rows or property columns or rows are selected.
    /// </summary>
    bool AreColumnsOrRowsSelected { get; }

    /// <summary>
    /// Clears all selections.
    /// </summary>
    void ClearAllSelections();

    /// <summary>
    /// Gets a value indicating whether cut is enabled.
    /// </summary>
    bool EnableCut { get; }

    /// <summary>
    /// Gets a value indicating whether copy is enabled.
    /// </summary>
    bool EnableCopy { get; }

    /// <summary>
    /// Gets a value indicating whether paste is enabled.
    /// </summary>
    bool EnablePaste { get; }

    /// <summary>
    /// Gets a value indicating whether delete is enabled.
    /// </summary>
    bool EnableDelete { get; }

    /// <summary>
    /// Gets a value indicating whether select all is enabled.
    /// </summary>
    bool EnableSelectAll { get; }

    /// <summary>
    /// Cuts the current selection.
    /// </summary>
    void Cut();

    /// <summary>
    /// Copies the current selection.
    /// </summary>
    void Copy();

    /// <summary>
    /// Pastes clipboard contents.
    /// </summary>
    void Paste();

    /// <summary>
    /// Deletes the current selection.
    /// </summary>
    void Delete();

    /// <summary>
    /// Selects all cells.
    /// </summary>
    void SelectAll();

    /// <summary>
    /// The vertical scroll position is defined as following:
    /// If 0 (zero), the data row 0 is the first visible line (after the column header).
    /// If positive, the data row with the number of VertScrollPos is the first visible row.
    /// If negative, the property column with index PropertyColumnCount+VertScrollPos is the first visible line.
    /// </summary>
    int VerticalScrollPosition { get; set; }
  }
}
