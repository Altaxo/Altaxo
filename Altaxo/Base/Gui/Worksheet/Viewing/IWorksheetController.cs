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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Worksheet;
using Altaxo.Collections;

namespace Altaxo.Gui.Worksheet.Viewing
{
	public interface IWorksheetView
	{
		Altaxo.Gui.Worksheet.Viewing.IWorksheetController Controller { set; }
		Altaxo.Gui.Worksheet.Viewing.IGuiDependentWorksheetController GuiDependentController { get; }
		void TableAreaInvalidate();
		string TableViewTitle { set; }
		/// <summary>
		/// Returns the control that should be focused initially.
		/// </summary>
		object GuiInitiallyFocusedElement { get; }
	}

  public interface IWorksheetController : IMVCController
  {
    /// <summary>
    /// This returns the Table that is managed by this controller.
    /// </summary>
    Altaxo.Data.DataTable DataTable { get; }

    WorksheetLayout WorksheetLayout { get; }

    /// <summary>
    /// Returns the currently selected data columns
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
    /// Returns the currently selected property rows if property cells are selected alone. If not selected alone, the SelectedColumn property is returned.
    /// </summary>
    /// <remarks>Normally, if you select one or more data column, the corresponding property rows are selected by this. So it would be not possible to selected property rows without selecting the
    /// data column also. In order to fix this, you can first select property columns and then columns. In this case the selection is not stored into 
    /// SelectedColumns, but in SelectedPropertyRows, and SelectedColumns.Count returns 0.</remarks>
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


   void ClearAllSelections();

     /// <summary>
    /// Forces a redraw of the table view.
    /// </summary>
   void UpdateTableView();

	


	 bool EnableCut { get; }
	 bool EnableCopy { get; }
	 bool EnablePaste { get; }
	 bool EnableDelete { get; }
	 bool EnableSelectAll { get; }

	 void Cut();
	 void Copy();
	 void Paste();
	 void Delete();
	 void SelectAll();

	 event EventHandler TitleNameChanged;

  }

	public interface IGuiDependentWorksheetController 
	{

		/// <summary>
		/// Returns the currently selected data columns
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
		/// Returns the currently selected property rows if property cells are selected alone. If not selected alone, the SelectedColumn property is returned.
		/// </summary>
		/// <remarks>Normally, if you select one or more data column, the corresponding property rows are selected by this. So it would be not possible to selected property rows without selecting the
		/// data column also. In order to fix this, you can first select property columns and then columns. In this case the selection is not stored into 
		/// SelectedColumns, but in SelectedPropertyRows, and SelectedColumns.Count returns 0.</remarks>
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


		void ClearAllSelections();

		/// <summary>
		/// Forces a redraw of the table view.
		/// </summary>
		void TableAreaInvalidate();


		bool EnableCut { get; }
		bool EnableCopy { get; }
		bool EnablePaste { get; }
		bool EnableDelete { get; }
		bool EnableSelectAll { get; }

		void Cut();
		void Copy();
		void Paste();
		void Delete();
		void SelectAll();

	}
}
