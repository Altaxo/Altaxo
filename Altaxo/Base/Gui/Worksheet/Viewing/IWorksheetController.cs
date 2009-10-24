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
}
