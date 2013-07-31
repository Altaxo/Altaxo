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
//using System.Drawing;
//using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


using Altaxo.Graph.Gdi;
using Altaxo.Data;
using Altaxo.Serialization;
using Altaxo.Serialization.Ascii;
using Altaxo.Collections;
using Altaxo.Worksheet;
using Altaxo.Worksheet.Commands;



namespace Altaxo.Gui.Worksheet.Viewing
{
	using AM = Altaxo.Worksheet.AreaRetrieval;

	/// <summary>
	/// Default controller which implements IWorksheetController.
	/// </summary>
	[ExpectedTypeOfView(typeof(WorksheetViewWpf))]
	[UserControllerForObject(typeof(Altaxo.Worksheet.WorksheetLayout))]
	public class WorksheetControllerWpf : WorksheetController
	{
		public enum SelectionType { Nothing, DataRowSelection, DataColumnSelection, PropertyColumnSelection, PropertyRowSelection }


		#region Member variables


		/// <summary>Holds the view (the window where the graph is visualized).</summary>
		protected WorksheetViewWpf _view;


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

		private Point _mouseDownPosition; // holds the position of a double click
		private int _dragColumnWidth_ColumnNumber; // stores the column number if mouse hovers over separator
		private double _dragColumnWidth_OriginalPos;
		private double _dragColumnWidth_OriginalWidth;
		private bool _dragColumnWidth_InCapture;


		protected bool _cellEdit_IsArmed;
		protected bool _cellEdit_IsModified;
		private AreaInfo _cellEdit_EditedCell;


		protected TextBox _cellEditControl;
		protected VisualHost _visualHost;

		protected WeakEventHandler _weakEventHandlerDataColumnChanged;
		protected WeakEventHandler _weakEventHandlerPropertyColumnChanged;

		/// <summary>
		/// Set the member variables to default values. Intended only for use in constructors and deserialization code.
		/// </summary>
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

			// Holds the indizes to the selected data columns.
			_selectedDataColumns = new IndexSelection(); // holds the selected columns

			// Holds the indizes to the selected rows.
			_selectedDataRows = new IndexSelection(); // holds the selected rows

			// Holds the indizes to the selected property columns.
			_selectedPropertyColumns = new IndexSelection(); // holds the selected property columns

			// Holds the indizes to the selected property columns.
			_selectedPropertyRows = new IndexSelection(); // holds the selected property columns

			// Cached number of table rows.
			_numberOfTableRows = 0; // cached number of rows of the table

			// Cached number of table columns.
			_numberOfTableCols = 0;

			// Cached number of property columns.
			_numberOfPropertyCols = 0; // cached number of property  columnsof the table



			_mouseDownPosition = new Point(0, 0); // holds the position of a double click
			_dragColumnWidth_ColumnNumber = int.MinValue; // stores the column number if mouse hovers over separator
			_dragColumnWidth_OriginalPos = 0;
			_dragColumnWidth_OriginalWidth = 0;
			_dragColumnWidth_InCapture = false;


			_cellEdit_IsArmed = false;
			_cellEdit_EditedCell = new AreaInfo();


			_cellEditControl = new TextBox();
			_cellEditControl.AcceptsTab = true;
			_cellEditControl.BorderThickness = new Thickness(0);
			_cellEditControl.AcceptsReturn = true;
			_cellEditControl.Name = "_cellEditControl";
			_cellEditControl.TabIndex = 0;
			_cellEditControl.Text = "";
			_cellEdit_IsArmed = false;
			_cellEdit_IsModified = false;
			_cellEditControl.PreviewKeyDown += new KeyEventHandler(EhCellEditControl_PreviewKeyDown);
			_cellEditControl.TextChanged += new TextChangedEventHandler(EhCellEditControl_TextChanged);
			_cellEditControl.LostFocus += this.EhCellEditControl_LostFocus;
			_cellEditControl.Visibility = Visibility.Hidden;

			_visualHost = new VisualHost(EhView_TableAreaPaint);
		}

	

		#endregion

		#region Constructors

		public WorksheetControllerWpf()
		{
			SetMemberVariablesToDefault();
		}

		protected override void InternalInitializeWorksheetLayout(WorksheetLayout layout)
		{
			base.InternalInitializeWorksheetLayout(layout);

			_table.DataColumns.Changed += (_weakEventHandlerDataColumnChanged = new WeakEventHandler(this.EhTableDataChanged, x => _table.DataColumns.Changed -= x));
			_table.PropCols.Changed += (_weakEventHandlerPropertyColumnChanged = new WeakEventHandler(this.EhPropertyDataChanged, x => _table.PropCols.Changed -= x));
			this.SetCachedNumberOfDataColumns();
			this.SetCachedNumberOfDataRows();
			this.SetCachedNumberOfPropertyColumns();
		}

		public override void Dispose()
		{
			if (_table != null)
			{
				_weakEventHandlerDataColumnChanged.Remove();
				_weakEventHandlerPropertyColumnChanged.Remove();
			}
			base.Dispose();
		}

		#endregion // Constructors

		#region public properties

	

	

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double TableAreaWidth
		{
			get { return _view.TableAreaSize.Width; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double TableAreaHeight
		{
			get { return _view.TableAreaSize.Height; }
		}

		#endregion

		#region Selection of rows and columns

		/// <summary>
		/// Returns the currently selected data columns
		/// </summary>
		public override IndexSelection SelectedDataColumns
		{
			get { return _selectedDataColumns; }
		}

		/// <summary>
		/// Returns the currently selected data rows.
		/// </summary>
		public override IndexSelection SelectedDataRows
		{
			get { return _selectedDataRows; }
		}

		/// <summary>
		/// Returns the currently selected property columns.
		/// </summary>
		public override IndexSelection SelectedPropertyColumns
		{
			get { return _selectedPropertyColumns; }
		}

		/// <summary>
		/// Returns the currently selected property rows if property cells are selected alone. If not selected alone, the SelectedColumn property is returned.
		/// </summary>
		/// <remarks>Normally, if you select one or more data column, the corresponding property rows are selected by this. So it would be not possible to selected property rows without selecting the
		/// data column also. In order to fix this, you can first select property columns and then columns. In this case the selection is not stored into 
		/// SelectedColumns, but in SelectedPropertyRows, and SelectedColumns.Count returns 0.</remarks>
		public override IndexSelection SelectedPropertyRows
		{
			get { return _selectedPropertyRows.Count > 0 ? _selectedPropertyRows : _selectedDataColumns; }
		}



		/// <summary>
		/// Returns true if one or more property columns or rows are selected.
		/// </summary>
		public override bool ArePropertyCellsSelected
		{
			get
			{
				return this.DataTable.PropCols.ColumnCount > 0 && (SelectedPropertyColumns.Count > 0 || _selectedPropertyRows.Count > 0);
			}
		}


		/// <summary>
		/// Returns true if one or more data columns or rows are selected.
		/// </summary>
		public override bool AreDataCellsSelected
		{
			get { return this.DataTable.DataColumns.ColumnCount > 0 && SelectedDataColumns.Count > 0 || SelectedDataRows.Count > 0; }
		}


		/// <summary>
		/// Returns true if one or more columns, rows or property columns or rows are selected.
		/// </summary>
		public override bool AreColumnsOrRowsSelected
		{
			get { return AreDataCellsSelected || ArePropertyCellsSelected; }
		}

		/// <summary>
		/// Clears all selections of columns, rows or property columns.
		/// </summary>
		public override void ClearAllSelections()
		{
			SelectedDataColumns.Clear();
			SelectedDataRows.Clear();
			SelectedPropertyColumns.Clear();
			SelectedPropertyRows.Clear();

			TableAreaInvalidate();
		}



		#endregion

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

		#endregion

		#region Data event handlers


		public void EhTableDataChanged(object sender, EventArgs e)
		{
			Current.Gui.BeginExecute(EhTableDataChanged_Unsynchronized);
		}
		private void EhTableDataChanged_Unsynchronized()
		{
			if (this._numberOfTableRows != DataTable.DataColumns.RowCount)
				this.SetCachedNumberOfDataRows();

			if (this._numberOfTableCols != DataTable.DataColumns.ColumnCount)
				this.SetCachedNumberOfDataColumns();

			TableAreaInvalidate();
		}

		public void AdjustYScrollBarMaximum()
		{
			VertScrollMaximum = _numberOfTableRows > 0 ? _numberOfTableRows - 1 : 0;

			if (this.VertScrollPos >= _numberOfTableRows)
				VertScrollPos = _numberOfTableRows > 0 ? _numberOfTableRows - 1 : 0;

			if (_view != null)
				_view.TableAreaInvalidate();
		}

		public void AdjustXScrollBarMaximum()
		{

			this.HorzScrollMaximum = _numberOfTableCols > 0 ? _numberOfTableCols - 1 : 0;

			if (HorzScrollPos + 1 > _numberOfTableCols)
				HorzScrollPos = _numberOfTableCols > 0 ? _numberOfTableCols - 1 : 0;

			if (_view != null)
			{
				AdjustXScrollBarViewPortSize();
				_view.TableAreaInvalidate();
			}
			
		}

		/// <summary>Adjusts the X scroll bar view port size (=size of the thumb). Should be called after the XScrollbarMaximum is adusted or when the width of the columns changed considerably.
		/// Here, we want to avoid that the thumb size changed when we scroll through the worksheet. Thus, we calculate a average value for the thumb size that is relation of the table area width and 
		/// the total width of all data columns.</summary>
		public void AdjustXScrollBarViewPortSize()
		{
			if (_view != null)
			{
				double left, width;
				if (_numberOfTableCols > 0)
				{
					AM.GetXCoordinatesOfColumn(_numberOfTableCols - 1, _worksheetLayout, 0, out left, out width);
					_view.TableViewHorzViewPortSize = HorzScrollMaximum * TableAreaWidth / (left + width);
				}
				else
				{
					_view.TableViewHorzViewPortSize = 1000000; // if the worksheet contains no columns, then ensure that the viewport is fully streched horizontally
				}
			}
		}

		protected virtual void SetCachedNumberOfDataColumns()
		{
			// ask for table dimensions, compare with cached dimensions
			// and adjust the scroll bars appropriate
			int oldDataCols = this._numberOfTableCols;
			this._numberOfTableCols = DataTable.DataColumns.ColumnCount;
			if (this._numberOfTableCols != oldDataCols)
			{
				AdjustXScrollBarMaximum();
			}
		}


		protected virtual void SetCachedNumberOfDataRows()
		{
			// ask for table dimensions, compare with cached dimensions
			// and adjust the scroll bars appropriate
			int oldDataRows = this._numberOfTableRows;
			this._numberOfTableRows = DataTable.DataColumns.RowCount;

			if (_numberOfTableRows != oldDataRows)
			{
				AdjustYScrollBarMaximum();
			}

		}

		protected virtual void SetCachedNumberOfPropertyColumns()
		{
			// ask for table dimensions, compare with cached dimensions
			// and adjust the scroll bars appropriate
			int oldPropCols = this._numberOfPropertyCols;
			this._numberOfPropertyCols = _table.PropCols.ColumnCount;

			if (oldPropCols != this._numberOfPropertyCols)
			{
				// if we was scrolled to the most upper position, we later scroll
				// to the most upper position again
				bool bUpperPosition = (oldPropCols == -this.VertScrollPos);

				// Adjust Y ScrollBar Maximum();
				AdjustYScrollBarMaximum();

				if (bUpperPosition) // we scroll again to the most upper position
				{
					this.VertScrollPos = -this.TotalEnabledPropertyColumns;
				}
			}
		}

		public void EhPropertyDataChanged(object sender, EventArgs e)
		{
			Current.Gui.Execute(EhPropertyDataChanged_Unsynchronized, sender, e);
		}
		private void EhPropertyDataChanged_Unsynchronized(object sender, EventArgs e)
		{
			if (this._numberOfPropertyCols != DataTable.PropCols.ColumnCount)
				SetCachedNumberOfPropertyColumns();


			TableAreaInvalidate();
		}

		#endregion

		#region CellEditControl logic

		#region CellEditControl event handlers

		void EhCellEditControl_LostFocus(object sender, RoutedEventArgs e)
		{
			ReadCellEditContentAndHide();
		}

		void EhCellEditControl_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Left)
			{
				// Navigate to the left if the cursor is already left
				//if(m_CellEditControl.SelectionStart==0 && (m_CellEdit_EditedCell.Row>0 || m_CellEdit_EditedCell.Column>0) )
				if (_cellEditControl.SelectionStart == 0)
				{
					e.Handled = true;
					// Navigate to the left
					NavigateCellEdit(-1, 0);
				}
			}
			else if (e.Key == Key.Right)
			{
				if (_cellEditControl.SelectionStart + _cellEditControl.SelectionLength >= _cellEditControl.Text.Length)
				{
					e.Handled = true;
					// Navigate to the right
					NavigateCellEdit(1, 0);
				}
			}
			else if (e.Key == Key.Up)
			{
				e.Handled = true;
				// Navigate up
				NavigateCellEdit(0, -1);
			}
			else if (e.Key == Key.Down)
			{
				e.Handled = true;
				// Navigate down
				NavigateCellEdit(0, 1);
			}
			else if (e.Key == Key.Enter)
			{
				// if some text is selected, deselect it and move the cursor to the end
				// else same action like keys.Down
				e.Handled = true;
				if (_cellEditControl.SelectionLength > 0)
				{
					_cellEditControl.SelectionLength = 0;
					_cellEditControl.SelectionStart = _cellEditControl.Text.Length;
				}
				else
				{
					NavigateCellEdit(0, 1);
				}
			}
			else if (e.Key == Key.Escape)
			{
				e.Handled = true;
				HideCellEditControl();
			}
			else if (e.Key == Key.Tab) // Tab key pressed
			{
				if (_cellEditControl.SelectionStart + _cellEditControl.SelectionLength >= _cellEditControl.Text.Length)
				{
					e.Handled = true;
					// Navigate to the right
					NavigateCellEdit(1, 0);
				}
			}

		}

		void EhCellEditControl_TextChanged(object sender, TextChangedEventArgs e)
		{
			_cellEdit_IsModified = true;
		}

		#endregion CellEditControl event handlers

		#region CellEditControl Show and Hide

		void HideCellEditControl()
		{
			_cellEditControl.Visibility = Visibility.Hidden;
			_cellEdit_IsArmed = false;
			if(null!=_view) // view can already be closed
				_view.Canvas.Focus();
		}

		void ShowCellEditControl()
		{
			if (!_view.Canvas.Children.Contains(_cellEditControl))
				_view.Canvas.Children.Add(_cellEditControl);
			_cellEditControl.Visibility = Visibility.Visible;
			_cellEditControl.Focus();
			_cellEdit_IsArmed = true;
		}

		private void ReadCellEditContentAndHide()
		{
			if (this._cellEdit_IsArmed)
			{
				if (_cellEdit_IsModified)
				{
					if (this._cellEdit_EditedCell.AreaType == AreaType.DataCell)
					{
						GetDataColumnStyle(_cellEdit_EditedCell.ColumnNumber).SetColumnValueAtRow(_cellEditControl.Text, _cellEdit_EditedCell.RowNumber, DataTable[_cellEdit_EditedCell.ColumnNumber]);
					}
					else if (this._cellEdit_EditedCell.AreaType == AreaType.PropertyCell)
					{
						GetPropertyColumnStyle(_cellEdit_EditedCell.ColumnNumber).SetColumnValueAtRow(_cellEditControl.Text, _cellEdit_EditedCell.RowNumber, DataTable.PropCols[_cellEdit_EditedCell.ColumnNumber]);
					}
				}
				HideCellEditControl();
			}
		}

		private void SetCellEditContentAndShow()
		{
			if (this._cellEdit_EditedCell.AreaType == AreaType.DataCell)
			{
				_cellEditControl.Text = GetDataColumnStyle(_cellEdit_EditedCell.ColumnNumber).GetColumnValueAtRow(_cellEdit_EditedCell.RowNumber, DataTable[_cellEdit_EditedCell.ColumnNumber]);
			}
			else if (this._cellEdit_EditedCell.AreaType == AreaType.PropertyCell)
			{
				_cellEditControl.Text = this.GetPropertyColumnStyle(_cellEdit_EditedCell.ColumnNumber).GetColumnValueAtRow(_cellEdit_EditedCell.RowNumber, DataTable.PropCols[_cellEdit_EditedCell.ColumnNumber]);
			}

			_cellEditControl.TextAlignment = TextAlignment.Right;
			_cellEditControl.SelectAll();
			_cellEdit_IsModified = false;
			ShowCellEditControl();
		}

		#endregion


		#region CellEditControl Navigation

		/// <summary>
		/// NavigateCellEdit moves the cell edit control to the next cell
		/// </summary>
		/// <param name="dx">move dx cells to the right</param>
		/// <param name="dy">move dy cells down</param>
		/// <returns>True when the cell was moved to a new position, false if moving was not possible.</returns>
		protected bool NavigateCellEdit(int dx, int dy)
		{
			if (this._cellEdit_EditedCell.AreaType == AreaType.DataCell)
			{
				return NavigateTableCellEdit(dx, dy);
			}
			else if (this._cellEdit_EditedCell.AreaType == AreaType.PropertyCell)
			{
				return NavigatePropertyCellEdit(dx, dy);
			}
			return false;
		}

		/// <summary>
		/// NavigateTableCellEdit moves the cell edit control to the next cell
		/// </summary>
		/// <param name="dx">move dx cells to the right</param>
		/// <param name="dy">move dy cells down</param>
		/// <returns>True when the cell was moved to a new position, false if moving was not possible.</returns>
		protected bool NavigateTableCellEdit(int dx, int dy)
		{
			bool bScrolled = false;

			// Calculate the position of the new cell   
			int newCellCol = this._cellEdit_EditedCell.ColumnNumber + dx;
			if (newCellCol >= DataTable.DataColumns.ColumnCount)
			{
				newCellCol = 0;
				dy += 1;
			}
			else if (newCellCol < 0)
			{
				if (this._cellEdit_EditedCell.RowNumber > 0) // move to the last cell only if not on cell 0
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
				navigateToRow = newCellRow + 1 - FullyVisibleTableRows - this.FullyVisiblePropertyColumns;
			else
				navigateToRow = this.VertScrollPos;

			if (navigateToCol != FirstVisibleColumn || navigateToRow != FirstVisibleTableRow)
			{
				SetScrollPositionTo(navigateToCol, navigateToRow);
				bScrolled = true;
			}
			// 3. Fill the cell edit control with new content
			_cellEdit_EditedCell.ColumnNumber = newCellCol;
			_cellEdit_EditedCell.RowNumber = newCellRow;
			var cellRect = AM.GetCoordinatesOfDataCell(_cellEdit_EditedCell.ColumnNumber, _cellEdit_EditedCell.RowNumber, _worksheetLayout, HorzScrollPos, VertScrollPos);
			_cellEditControl.SetValue(Canvas.LeftProperty, cellRect.Left);
			_cellEditControl.SetValue(Canvas.TopProperty, cellRect.Top);
			_cellEditControl.Width = cellRect.Width;
			_cellEditControl.Height = cellRect.Height;
			SetCellEditContentAndShow();

			// 4. Invalidate the client area if scrolled in step (2)
			if (bScrolled)
				this._view.TableAreaInvalidate();

			return true;
		}


		/// <summary>
		/// NavigatePropertyCellEdit moves the cell edit control to the next cell
		/// </summary>
		/// <param name="dx">move dx cells to the right</param>
		/// <param name="dy">move dy cells down</param>
		/// <returns>True when the cell was moved to a new position, false if moving was not possible.</returns>
		protected bool NavigatePropertyCellEdit(int dx, int dy)
		{
			bool bScrolled = false;


			// 2. look whether the new cell coordinates lie inside the client area, if
			// not scroll the worksheet appropriate
			int newCellCol = this._cellEdit_EditedCell.ColumnNumber + dy;
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
				if (this._cellEdit_EditedCell.RowNumber > 0) // move to the last cell only if not on cell 0
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
				if (this._cellEdit_EditedCell.ColumnNumber > 0) // move to the last cell only if not on cell 0
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
				navigateToCol = newCellCol - this.FullyVisiblePropertyColumns + 1 - _numberOfPropertyCols;
			else
				navigateToCol = this.VertScrollPos;


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
			var cellRect = AM.GetCoordinatesOfPropertyCell(_cellEdit_EditedCell.ColumnNumber, _cellEdit_EditedCell.RowNumber, _worksheetLayout, HorzScrollPos, VertScrollPos);
			_cellEditControl.SetValue(Canvas.LeftProperty, cellRect.Left);
			_cellEditControl.SetValue(Canvas.TopProperty, cellRect.Top);
			_cellEditControl.Width = cellRect.Width;
			_cellEditControl.Height = cellRect.Height;
			SetCellEditContentAndShow();

			// 4. Invalidate the client area if scrolled in step (2)
			if (bScrolled)
				this._view.TableAreaInvalidate();

			return true;
		}

		#endregion CellEditControl Navigation

		#endregion CellEditControl

		#region Shortcuts for number and position of rows and columns

		#region Data rows

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FirstVisibleTableRow
		{
			get
			{
				return AM.GetFirstVisibleTableRow(_worksheetLayout, VertScrollPos);
			}
		}

		/// <summary>
		/// This returns the vertical position of the first visible data row.;
		/// </summary>
		public int VerticalPositionOfFirstVisibleDataRow
		{
			get
			{
				return AM.GetVerticalPositionOfFirstVisibleDataRow(_worksheetLayout, VertScrollPos);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleTableRows
		{
			get
			{
				return AM.GetVisibleTableRows(0, this.TableAreaHeight, _worksheetLayout, VertScrollPos);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FullyVisibleTableRows
		{
			get
			{
				return AM.GetFullyVisibleTableRows(0, TableAreaHeight, _worksheetLayout, VertScrollPos);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastVisibleTableRow
		{
			get
			{
				return AM.GetLastVisibleTableRow(_worksheetLayout, VertScrollPos, TableAreaHeight);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastFullyVisibleTableRow
		{
			get
			{
				return AM.GetLastFullyVisibleTableRow(_worksheetLayout, VertScrollPos, TableAreaHeight);
			}
		}

		#endregion

		#region Property columns

		/// <summary>Returns the remaining number of property columns that could be shown below the current scroll position.</summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int RemainingEnabledPropertyColumns
		{
			get
			{
				return AM.GetRemainingEnabledPropertyColumns(_worksheetLayout, VertScrollPos);
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



		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FirstVisiblePropertyColumn
		{
			get
			{
				return AM.GetFirstVisiblePropertyColumn(_worksheetLayout, VertScrollPos);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastFullyVisiblePropertyColumn
		{
			get
			{
				return AM.GetLastFullyVisiblePropertyColumn(_worksheetLayout, VertScrollPos, TableAreaHeight);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisiblePropertyColumns
		{
			get
			{
				return AM.GetVisiblePropertyColumns(0, this.TableAreaHeight, _worksheetLayout, VertScrollPos);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FullyVisiblePropertyColumns
		{
			get
			{
				return AM.GetFullyVisiblePropertyColumns(0, this.TableAreaHeight, _worksheetLayout, VertScrollPos);
			}
		}

		#endregion property columns

		#region Data columns

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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleColumns
		{
			get
			{

				return AM.GetVisibleColumns(_worksheetLayout, HorzScrollPos, TableAreaWidth);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FullyVisibleColumns
		{
			get
			{
				return AM.GetFullyVisibleColumns(_worksheetLayout, HorzScrollPos, TableAreaWidth);
			}
		}



		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastFullyVisibleColumn
		{
			get
			{
				return AM.GetLastFullyVisibleColumn(_worksheetLayout, HorzScrollPos, TableAreaWidth);
			}
		}

		#endregion Data columns

		#endregion Number and position of rows and columns

		#region Scrolling logic


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

					if (_view != null)
					{
						_view.TableViewHorzScrollValue = value;
						TableAreaInvalidate();
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
		public int VertScrollPos
		{
			get { return _scrollVertPos; }
			set
			{
				int oldValue = _scrollVertPos;
				int newValue = value;
				newValue = Math.Min(this._scrollVertMax, newValue);
				newValue = Math.Max(-this.TotalEnabledPropertyColumns, newValue);
				_scrollVertPos = newValue;

				if (newValue != oldValue)
				{
					if (_cellEdit_IsArmed)
					{
						ReadCellEditContentAndHide();
					}

					// The value of the ScrollBar in the view has an offset, since he
					// can not have negative values;
					if (_view != null)
					{
						newValue += this.TotalEnabledPropertyColumns;
						this._view.TableViewVertScrollValue = newValue;
						this._view.TableViewVertViewPortSize = AM.GetVisibleTableRows(0, this.TableAreaHeight, _worksheetLayout, newValue) + AM.GetVisiblePropertyColumns(0,this.TableAreaHeight,_worksheetLayout,newValue);
						TableAreaInvalidate();
					}
				}
			}
		}

		public int HorzScrollMaximum
		{
			get { return this._scrollHorzMax; }
			set
			{
				this._scrollHorzMax = value;
				if (_view != null)
					_view.TableViewHorzScrollMaximum = value;
			}
		}

		public int VertScrollMaximum
		{
			get { return this._scrollVertMax; }
			set
			{
				this._scrollVertMax = value;

				if (_view != null)
					_view.TableViewVertScrollMaximum = value + this.TotalEnabledPropertyColumns;
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
			if (this.HorzScrollMaximum < nCol)
				this.HorzScrollMaximum = nCol;
			this.HorzScrollPos = nCol;

			if (this.VertScrollMaximum < nRow)
				this.VertScrollMaximum = nRow;
			this.VertScrollPos = nRow;
		}


		#endregion Scrolling logic

		#region IWorksheetController Members

		


		public override object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					_view.Controller = null;
				}

				_view = value as WorksheetViewWpf;

				if (null != _view)
				{
					_view.Controller = this;

					_view.LowerCanvas.Children.Add(_visualHost);
					_view.Canvas.Children.Add(_cellEditControl);

					_view.TableViewTitle = this.TitleName;

					// Werte für gerade vorliegende Scrollpositionen und Scrollmaxima zum (neuen) View senden
					this.VertScrollMaximum = this._scrollVertMax;
					this.HorzScrollMaximum = this._scrollHorzMax;

					this.VertScrollPos = this._scrollVertPos;
					this.HorzScrollPos = this._scrollHorzPos;



					// Simulate a SizeChanged event 
					this.EhView_TableAreaSizeChanged(new EventArgs());
				}
			}
		}

		public void EhView_VertScrollBarScroll(System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			VertScrollPos = (int)e.NewValue - this.TotalEnabledPropertyColumns;
		}

		public void EhView_HorzScrollBarScroll(System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			//if (e.ScrollEventType != System.Windows.Controls.Primitives.ScrollEventType.ThumbTrack)
				HorzScrollPos = (int)e.NewValue;
		}

		public void EhView_TableAreaMouseUp(Point position, MouseButtonEventArgs e)
		{
			if (this._dragColumnWidth_InCapture)
			{
				double sizediff = position.X - this._dragColumnWidth_OriginalPos;
				Altaxo.Worksheet.ColumnStyle cs;
				if (-1 == _dragColumnWidth_ColumnNumber)
				{
					cs = this._worksheetLayout.RowHeaderStyle;
				}
				else
				{
					_worksheetLayout.DataColumnStyles.TryGetValue(DataTable[_dragColumnWidth_ColumnNumber], out cs);
					if (null == cs)
					{
						Altaxo.Worksheet.ColumnStyle template = GetDataColumnStyle(this._dragColumnWidth_ColumnNumber);
						cs = (Altaxo.Worksheet.ColumnStyle)template.Clone();
						_worksheetLayout.DataColumnStyles.Add(DataTable[_dragColumnWidth_ColumnNumber], cs);
					}
				}
				double newWidth = this._dragColumnWidth_OriginalWidth + sizediff;
				if (newWidth < 10)
					newWidth = 10;
				cs.WidthD = newWidth;

				this._dragColumnWidth_InCapture = false;
				this._dragColumnWidth_ColumnNumber = int.MinValue;
				this._view.TableAreaCapture = false;
				this._view.TableAreaCursor = Cursors.Arrow;
				this._view.TableAreaInvalidate();

			}
		}

		public void EhView_TableAreaMouseDown(Point position, MouseButtonEventArgs e)
		{
			// base.OnMouseDown(e);
			this._mouseDownPosition = position;
			ReadCellEditContentAndHide();

			if (this._dragColumnWidth_ColumnNumber >= -1)
			{
				this._view.TableAreaCapture = true;
				_dragColumnWidth_OriginalPos = position.X;
				_dragColumnWidth_InCapture = true;
			}
		}

		/// <summary>
		/// Handles the mouse wheel event.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">MouseEventArgs.</param>
		public void EhView_TableAreaMouseWheel(Point position, MouseWheelEventArgs e)
		{

			int oldScrollPos = VertScrollPos;
			VertScrollPos = VertScrollPos - e.Delta / 120;
		}

		public void EhView_TableAreaMouseMove(Point position, MouseEventArgs e)
		{
			var Y = position.Y;
			var X = position.X;

			if (this._dragColumnWidth_InCapture)
			{
				var sizediff = X - this._dragColumnWidth_OriginalPos;

				Altaxo.Worksheet.ColumnStyle cs;
				if (-1 == _dragColumnWidth_ColumnNumber)
					cs = this._worksheetLayout.RowHeaderStyle;
				else
				{
					if (!_worksheetLayout.DataColumnStyles.TryGetValue(DataTable[_dragColumnWidth_ColumnNumber], out cs))
					{
						Altaxo.Worksheet.ColumnStyle template = GetDataColumnStyle(this._dragColumnWidth_ColumnNumber);
						cs = (Altaxo.Worksheet.ColumnStyle)template.Clone();
						_worksheetLayout.DataColumnStyles.Add(DataTable[_dragColumnWidth_ColumnNumber], cs);
					}
				}

				var newWidth = this._dragColumnWidth_OriginalWidth + sizediff;
				if (newWidth < 10)
					newWidth = 10;
				cs.WidthD = newWidth;
				TableAreaInvalidate();
			}
			else // not in Capture mode
			{
				if (Y < this._worksheetLayout.ColumnHeaderStyle.Height)
				{
					for (int i = _columnWidthResizingPositions.Count - 1; i >= 0; i--)
					{
						double pos = _columnWidthResizingPositions[i];

						if (pos - 5 < X && X < pos + 5)
						{
							this._view.TableAreaCursor = Cursors.SizeWE;
							this._dragColumnWidth_ColumnNumber = i + FirstVisibleColumn;
							this._dragColumnWidth_OriginalWidth = _worksheetLayout.DataColumnStyles[DataTable[_dragColumnWidth_ColumnNumber]].WidthD;
							return;
						}
					} // end for

					if (this._worksheetLayout.RowHeaderStyle.Width - 5 < X && X < _worksheetLayout.RowHeaderStyle.Width + 5)
					{
						this._view.TableAreaCursor = Cursors.SizeWE;
						this._dragColumnWidth_ColumnNumber = -1;
						this._dragColumnWidth_OriginalWidth = this._worksheetLayout.RowHeaderStyle.Width;
						return;
					}
				}

				this._dragColumnWidth_ColumnNumber = int.MinValue;
				this._view.TableAreaCursor = Cursors.Arrow;
			} // end else
		}

		#region MouseClick functions
		protected virtual void OnLeftClickDataCell(AreaInfo clickedCell)
		{
			_cellEdit_EditedCell = clickedCell;
			_cellEditControl.SetValue(Canvas.LeftProperty, clickedCell.AreaRectangle.Left);
			_cellEditControl.SetValue(Canvas.TopProperty, clickedCell.AreaRectangle.Top);
			_cellEditControl.Width = clickedCell.AreaRectangle.Width;
			_cellEditControl.Height = clickedCell.AreaRectangle.Height;
			this.SetCellEditContentAndShow();
		}

		protected virtual void OnLeftClickPropertyCell(AreaInfo clickedCell)
		{
			_cellEdit_EditedCell = clickedCell;
			_cellEditControl.SetValue(Canvas.LeftProperty, clickedCell.AreaRectangle.Left);
			_cellEditControl.SetValue(Canvas.TopProperty, clickedCell.AreaRectangle.Top);
			_cellEditControl.Width = clickedCell.AreaRectangle.Width;
			_cellEditControl.Height = clickedCell.AreaRectangle.Height;
			this.SetCellEditContentAndShow();
		}

		protected virtual void OnLeftClickDataColumnHeader(AreaInfo clickedCell)
		{
			if (!this._dragColumnWidth_InCapture)
			{
				bool bControlKey = Keyboard.Modifiers.HasFlag(ModifierKeys.Control); // Control pressed
				bool bShiftKey = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

				bool bWasSelectedBefore = this.SelectedDataColumns.IsSelected(clickedCell.ColumnNumber);

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
				else if (!this.AreDataCellsSelected && this.ArePropertyCellsSelected && bControlKey)
				{
					_selectedPropertyRows.Select(clickedCell.ColumnNumber, bShiftKey, bControlKey);
					_lastSelectionType = SelectionType.PropertyRowSelection;
				}
				else
				{
					if (this.SelectedDataColumns.Count != 0 || !bWasSelectedBefore)
						_selectedDataColumns.Select(clickedCell.ColumnNumber, bShiftKey, bControlKey);
					_lastSelectionType = SelectionType.DataColumnSelection;
				}

				this._view.TableAreaInvalidate();
			}
		}

		protected virtual void OnLeftClickDataRowHeader(AreaInfo clickedCell)
		{
			bool bControlKey = Keyboard.Modifiers.HasFlag(ModifierKeys.Control); // Control pressed
			bool bShiftKey = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

			bool bWasSelectedBefore = this.SelectedDataRows.IsSelected(clickedCell.RowNumber);

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

			if (this.SelectedDataRows.Count != 0 || !bWasSelectedBefore)
				_selectedDataRows.Select(clickedCell.RowNumber, bShiftKey, bControlKey);
			_lastSelectionType = SelectionType.DataRowSelection;
			this._view.TableAreaInvalidate();
		}

		protected virtual void OnLeftClickPropertyColumnHeader(AreaInfo clickedCell)
		{
			bool bControlKey = Keyboard.Modifiers.HasFlag(ModifierKeys.Control); // Control pressed
			bool bShiftKey = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

			bool bWasSelectedBefore = this.SelectedPropertyColumns.IsSelected(clickedCell.ColumnNumber);

			if ((!bControlKey && !bShiftKey) || (_lastSelectionType != SelectionType.PropertyColumnSelection && !bControlKey))
			{
				_selectedDataColumns.Clear();
				_selectedDataRows.Clear(); // if we click a column, we remove row selections
				_selectedPropertyColumns.Clear();
				_selectedPropertyRows.Clear();
			}



			if (this.SelectedPropertyColumns.Count != 0 || !bWasSelectedBefore)
				_selectedPropertyColumns.Select(clickedCell.ColumnNumber, bShiftKey, bControlKey);

			_lastSelectionType = SelectionType.PropertyColumnSelection;
			this._view.TableAreaInvalidate();
		}

		[field: NonSerialized]
		public event Action<object, AreaInfo> TableHeaderLeftClicked;
		protected virtual void OnLeftClickTableHeader(AreaInfo clickedCell)
		{
			if (null != TableHeaderLeftClicked)
				TableHeaderLeftClicked(this, clickedCell);
		}

		[field: NonSerialized]
		public event Action<object, AreaInfo> OutsideAllLeftClicked;
		protected virtual void OnLeftClickOutsideAll(AreaInfo clickedCell)
		{
			if (null != OutsideAllLeftClicked)
				OutsideAllLeftClicked(this, clickedCell);
		}

		[field: NonSerialized]
		public event Action<object, AreaInfo> DataCellRightClicked;
		protected virtual void OnRightClickDataCell(AreaInfo clickedCell)
		{
			if (null != DataCellRightClicked)
				DataCellRightClicked(this, clickedCell);
		}

		[field: NonSerialized]
		public event Action<object, AreaInfo> PropertyCellRightClicked;
		protected virtual void OnRightClickPropertyCell(AreaInfo clickedCell)
		{
			if (null != PropertyCellRightClicked)
				PropertyCellRightClicked(this, clickedCell);
		}

		[field: NonSerialized]
		public event Action<object, AreaInfo> DataColumnHeaderRightClicked;
		protected virtual void OnRightClickDataColumnHeader(AreaInfo clickedCell)
		{
			if (null != DataColumnHeaderRightClicked)
				DataColumnHeaderRightClicked(this, clickedCell);

			if (!(this.SelectedDataColumns.Contains(clickedCell.ColumnNumber)) &&
					!(this.SelectedPropertyRows.Contains(clickedCell.ColumnNumber)))
			{
				this.ClearAllSelections();
				this.SelectedDataColumns.Add(clickedCell.ColumnNumber);
				this._view.TableAreaInvalidate();
			}
			Current.Gui.ShowContextMenu(_view, _view, "/Altaxo/Views/Worksheet/DataColumnHeader/ContextMenu", clickedCell.AreaRectangle.X, clickedCell.AreaRectangle.Y);
		}

		[field: NonSerialized]
		public event Action<object, AreaInfo> DataRowHeaderRightClicked;
		protected virtual void OnRightClickDataRowHeader(AreaInfo clickedCell)
		{
			if (null != DataRowHeaderRightClicked)
				DataRowHeaderRightClicked(this, clickedCell);

			if (!(this.SelectedDataRows.Contains(clickedCell.RowNumber)))
			{
				this.ClearAllSelections();
				this.SelectedDataRows.Add(clickedCell.RowNumber);
				this._view.TableAreaInvalidate();
			}
			Current.Gui.ShowContextMenu(_view, _view, "/Altaxo/Views/Worksheet/DataRowHeader/ContextMenu", clickedCell.AreaRectangle.X, clickedCell.AreaRectangle.Y);
		}

		[field: NonSerialized]
		public event Action<object, AreaInfo> PropertyColumnHeaderRightClicked;
		protected virtual void OnRightClickPropertyColumnHeader(AreaInfo clickedCell)
		{
			if (null != PropertyColumnHeaderRightClicked)
				PropertyColumnHeaderRightClicked(this, clickedCell);

			if (!(this.SelectedPropertyColumns.Contains(clickedCell.ColumnNumber)))
			{
				this.ClearAllSelections();
				this.SelectedPropertyColumns.Add(clickedCell.ColumnNumber);
				this._view.TableAreaInvalidate();
			}
			Current.Gui.ShowContextMenu(_view, _view, "/Altaxo/Views/Worksheet/PropertyColumnHeader/ContextMenu", clickedCell.AreaRectangle.X, clickedCell.AreaRectangle.Y);
		}

		[field: NonSerialized]
		public event Action<object, AreaInfo> TableHeaderRightClicked;
		protected virtual void OnRightClickTableHeader(AreaInfo clickedCell)
		{
			if (null != TableHeaderRightClicked)
				TableHeaderRightClicked(this, clickedCell);

			Current.Gui.ShowContextMenu(_view, _view, "/Altaxo/Views/Worksheet/DataTableHeader/ContextMenu", clickedCell.AreaRectangle.X, clickedCell.AreaRectangle.Y);
		}

		[field: NonSerialized]
		public event Action<object, AreaInfo> OutsideAllRightClicked;
		protected virtual void OnRightClickOutsideAll(AreaInfo clickedCell)
		{
			if (null != OutsideAllRightClicked)
				OutsideAllRightClicked(this, clickedCell);

			Current.Gui.ShowContextMenu(_view, _view, "/Altaxo/Views/Worksheet/OutsideAll/ContextMenu", clickedCell.AreaRectangle.X, clickedCell.AreaRectangle.Y);
		}
		#endregion

		public void EhView_TableAreaMouseClick(Point position, MouseButtonEventArgs e)
		{
			var _mouseInfo = AM.GetAreaType(position.X, position.Y, _worksheetLayout, HorzScrollPos, VertScrollPos);

			//ClickedCellInfoWpf clickedCell = new ClickedCellInfoWpf(this,this.m_MouseDownPosition);

			if (e.ChangedButton == MouseButton.Left)
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
						OnLeftClickPropertyColumnHeader(_mouseInfo);
						break;
					case AreaType.DataColumnHeader:
						OnLeftClickDataColumnHeader(_mouseInfo);
						break;
					case AreaType.DataRowHeader:
						OnLeftClickDataRowHeader(_mouseInfo);
						break;
					case AreaType.TableHeader:
						OnLeftClickTableHeader(_mouseInfo);
						break;
					case AreaType.OutsideAll:
						OnLeftClickOutsideAll(_mouseInfo);
						break;
				}
			}
			else if (e.ChangedButton == MouseButton.Right)
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

		public void EhView_TableAreaMouseDoubleClick(Point position, MouseButtonEventArgs e)
		{
			// TODO:  Add WorksheetController.EhView_TableAreaMouseDoubleClick implementation
		}

		public void EhView_TableAreaPaint(DrawingContext notUsed)
		{
			if (_view == null || _view.Canvas == null || this.DataTable == null)
				return;

			using (var dc = _visualHost.OpenDrawingContext())
			{
				Rect clipRect = new Rect(0, 0, _view.Canvas.ActualWidth, _view.Canvas.ActualHeight);

				WorksheetPaintingWpf.PaintTableArea(dc, _worksheetLayout, clipRect.Size, clipRect,
					_selectedDataColumns, _selectedDataRows, _selectedPropertyColumns, _selectedPropertyRows,
					HorzScrollPos, VertScrollPos);

				dc.Close();
			}
			// now create the resizing positions
			_columnWidthResizingPositionsFirstColumnIndex = AM.GetColumnWidthResizingPositions(_columnWidthResizingPositions, _worksheetLayout, HorzScrollPos, TableAreaWidth);
		}

		public override void TableAreaInvalidate()
		{
			ForceTableAreaVisualUpdating();
		}

		private void ForceTableAreaVisualUpdating()
		{
			_visualHost.InvalidateDrawing();
		}

		public void EhView_TableAreaSizeChanged(EventArgs e)
		{
			TableAreaInvalidate();
		}

		#endregion

		#region ClipboardHandler Members

		public override bool EnableCut
		{
			get { return _cellEdit_IsArmed; }
		}

		public override bool EnableCopy
		{
			get { return true; }
		}

		public override bool EnablePaste
		{
			get { return true; }
		}

		public override bool EnableDelete
		{
			get { return !_cellEdit_IsArmed; }
		}

		public override bool EnableSelectAll
		{
			get { return true; }
		}

		public override void Cut()
		{
			if (this._cellEdit_IsArmed)
			{
				this._cellEditControl.Cut();
			}
			else if (this.AreColumnsOrRowsSelected)
			{
				// Copy the selected Columns to the clipboard
				EditCommands.CopyToClipboard(this);
			}
		}

		public override void Copy()
		{
			if (this._cellEdit_IsArmed)
			{
				this._cellEditControl.Copy();
			}
			else if (this.AreColumnsOrRowsSelected)
			{
				// Copy the selected Columns to the clipboard
				EditCommands.CopyToClipboard(this);
			}

		}

		public override void Paste()
		{
			if (this._cellEdit_IsArmed)
			{
				this._cellEditControl.Paste();
			}
			else
			{
				EditCommands.PasteFromClipboard(this);
			}
		}

		public override void Delete()
		{
			if (this._cellEdit_IsArmed)
			{
				this._cellEditControl.Clear();
			}
			else if (this.AreColumnsOrRowsSelected)
			{
				EditCommands.RemoveSelected(this);
			}
			else
			{
				// nothing is selected, we assume that the user wants to delete the worksheet itself
				Current.ProjectService.DeleteTable(this.DataTable, false);
			}
		}
		public override void SelectAll()
		{
			if (this.DataTable.DataColumns.ColumnCount > 0)
			{
				this.SelectedDataColumns.Select(0, false, false);
				this.SelectedDataColumns.Select(this.DataTable.DataColumns.ColumnCount - 1, true, false);
				if (_view != null)
					_view.TableAreaInvalidate();
			}
		}

		#endregion


		internal void EhView_KeyDown(KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.PageDown: // Page-Down-Key
					VertScrollPos = VertScrollPos + VisibleTableRows;
					break;
				case Key.PageUp: // Page-Up-Key
					VertScrollPos = VertScrollPos - VisibleTableRows;
					break;
				case Key.Home:
					HorzScrollPos = 0;
					break;
				case Key.End:
					HorzScrollPos = HorzScrollMaximum;
					break;
			}
			
		}
	}
}
