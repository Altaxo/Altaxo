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


namespace Altaxo.Worksheet
{
	/// <summary>The type of area we have clicked into, used by ClickedCellInfo.</summary>
	public enum AreaType
	{
		/// <summary>Outside of all relevant areas.</summary>
		OutsideAll,
		/// <summary>On the table header (top left corner of the data grid).</summary>
		TableHeader,
		/// <summary>Inside a data cell.</summary>
		DataCell,
		/// <summary>Inside a property cell.</summary>
		PropertyCell,
		/// <summary>On the column header.</summary>
		DataColumnHeader,
		/// <summary>On the row header.</summary>
		DataRowHeader,
		/// <summary>On the property column header.</summary>
		PropertyColumnHeader
	}


	public struct AreaInfo
	{
		public Altaxo.Graph.RectangleD AreaRectangle { get; set; }
		public AreaType AreaType { get; set; }
		public int RowNumber { get; set; }
		public int ColumnNumber { get; set; }
	}


	public static class AreaRetrieval
	{
		#region Data Row

		/// <summary>
		/// This returns the vertical position of the first visible data row.;
		/// </summary>
		public static int GetVerticalPositionOfFirstVisibleDataRow(WorksheetLayout layout, int VertScrollPos)
		{
			return layout.ColumnHeaderStyle.Height + (VertScrollPos >= 0 ? 0 : -VertScrollPos * layout.PropertyColumnHeaderStyle.Height);
		}

		/// <summary>
		/// Gets the first table row that is visible.
		/// </summary>
		/// <returns>The first table row that is visible.</returns>
		public static int GetFirstVisibleTableRow(WorksheetLayout layout, int VertScrollPos)
		{
			return Math.Max(0, VertScrollPos);
		}



		/// <summary>
		/// Gets the first table row that is visible under the coordinate top.
		/// </summary>
		/// <param name="top">The upper coordinate of the clipping rectangle.</param>
		/// <param name="layout">Worksheet layout referring to.</param>
		/// <param name="VertScrollPos">Value of the vertical scroll bar.</param>
		/// <returns>The first table row that is visible below the top coordinate.</returns>
		public static int GetFirstVisibleTableRow(double top, WorksheetLayout layout, int VertScrollPos)
		{
			int posOfDataRow0 = GetVerticalPositionOfFirstVisibleDataRow(layout, VertScrollPos);

			//int firstTotRow = (int)Math.Max(RemainingEnabledPropertyColumns,Math.Floor((top-m_TableLayout.ColumnHeaderStyle.Height)/(double)m_TableLayout.RowHeaderStyle.Height));
			//return FirstVisibleTableRow + Math.Max(0,firstTotRow-RemainingEnabledPropertyColumns);
			int firstVis = (int)Math.Floor((top - posOfDataRow0) / (double)layout.RowHeaderStyle.Height);
			return (firstVis < 0 ? 0 : firstVis) + Math.Max(0, VertScrollPos);
		}

		/// <summary>
		/// How many data rows are visible between top and bottom?
		/// </summary>
		/// <param name="top">The top y coordinate.</param>
		/// <param name="bottom">The bottom y coordinate.</param>
		/// <param name="layout">Worksheet layout referring to.</param>
		/// <param name="VertScrollPos">Value of the vertical scroll bar.</param>
		/// <returns>The number of data rows visible between these two coordinates.</returns>
		public static int GetVisibleTableRows(double top, double bottom, WorksheetLayout layout, int VertScrollPos)
		{
			int posOfDataRow0 = GetVerticalPositionOfFirstVisibleDataRow(layout, VertScrollPos);

			if (top < posOfDataRow0)
				top = posOfDataRow0;

			int firstRow = (int)Math.Floor((top - posOfDataRow0) / (double)layout.RowHeaderStyle.Height);
			int lastRow = (int)Math.Ceiling((bottom - posOfDataRow0) / (double)layout.RowHeaderStyle.Height) - 1;
			return Math.Max(0, 1 + lastRow - firstRow);
		}

		public static int GetLastVisibleTableRow(WorksheetLayout layout, int VertScrollPos, double TableAreaHeight)
		{
			return GetFirstVisibleTableRow(layout, VertScrollPos) + GetVisibleTableRows(0, TableAreaHeight, layout, VertScrollPos) - 1;
		}

		public static int GetLastFullyVisibleTableRow(WorksheetLayout layout, int VertScrollPos, double TableAreaHeight)
		{
			return GetFirstVisibleTableRow(layout, VertScrollPos) + GetFullyVisibleTableRows(0, TableAreaHeight, layout, VertScrollPos) - 1;
		}

		public static int GetFullyVisibleTableRows(double top, double bottom, WorksheetLayout layout, int VertScrollPos)
		{
			int posOfDataRow0 = GetVerticalPositionOfFirstVisibleDataRow(layout, VertScrollPos);

			if (top < posOfDataRow0)
				top = posOfDataRow0;

			int firstRow = (int)Math.Floor((top - posOfDataRow0) / (double)layout.RowHeaderStyle.Height);
			int lastRow = (int)Math.Floor((bottom - posOfDataRow0) / (double)layout.RowHeaderStyle.Height) - 1;
			return Math.Max(0, 1 + lastRow - firstRow);
		}

		public static int GetTopCoordinateOfTableRow(int nRow, WorksheetLayout layout, int VertScrollPos)
		{
			return GetVerticalPositionOfFirstVisibleDataRow(layout, VertScrollPos) + (nRow - (VertScrollPos < 0 ? 0 : VertScrollPos)) * layout.RowHeaderStyle.Height;
		}


		#endregion

		#region Property column

		public static int GetTotalEnabledPropertyColumns(WorksheetLayout layout)
		{
			return layout.ShowPropertyColumns ? layout.DataTable.PropertyColumnCount : 0;
		}

		public static int GetFirstVisiblePropertyColumn(WorksheetLayout layout, int VertScrollPos)
		{
			if (layout.ShowPropertyColumns && VertScrollPos < 0)
			{
				// make sure that VertScrollPos does not exceed TotalEnabledPropertyColumns
				if (VertScrollPos < -GetTotalEnabledPropertyColumns(layout))
					VertScrollPos = -GetTotalEnabledPropertyColumns(layout);
				return GetTotalEnabledPropertyColumns(layout) + VertScrollPos;
			}
			else
				return -1;
		}

		public static int GetFirstVisiblePropertyColumn(double top, WorksheetLayout layout, int VertScrollPos)
		{
			if (VertScrollPos >= 0 || !layout.ShowPropertyColumns)
				return -1;

			int firstTotRow = (int)Math.Max(0, Math.Floor((top - layout.ColumnHeaderStyle.Height) / (double)layout.PropertyColumnHeaderStyle.Height));
			return firstTotRow + GetFirstVisiblePropertyColumn(layout, VertScrollPos);
		}

		public static int GetLastFullyVisiblePropertyColumn(WorksheetLayout layout, int VertScrollPos, double TableAreaHeight)
		{
			return GetFirstVisiblePropertyColumn(layout, VertScrollPos) + GetFullyVisiblePropertyColumns(layout, VertScrollPos, TableAreaHeight) - 1;
		}


		public static int GetTopCoordinateOfPropertyColumn(int nCol, WorksheetLayout layout, int VertScrollPos)
		{
			return layout.ColumnHeaderStyle.Height + (nCol - GetFirstVisiblePropertyColumn(layout, VertScrollPos)) * layout.PropertyColumnHeaderStyle.Height;
		}

		public static int GetVisiblePropertyColumns(double top, double bottom, WorksheetLayout layout, int VertScrollPos)
		{
			if (layout.ShowPropertyColumns)
			{
				int firstTotRow = (int)Math.Max(0, Math.Floor((top - layout.ColumnHeaderStyle.Height) / (double)layout.PropertyColumnHeaderStyle.Height));
				int lastTotRow = (int)Math.Ceiling((bottom - layout.ColumnHeaderStyle.Height) / (double)layout.PropertyColumnHeaderStyle.Height) - 1;
				int maxPossRows = Math.Max(0, GetRemainingEnabledPropertyColumns(layout, VertScrollPos) - firstTotRow);
				return Math.Min(maxPossRows, Math.Max(0, 1 + lastTotRow - firstTotRow));
			}
			else
				return 0;
		}

		/// <summary>
		/// Gets the number of fully visible property columns in the full table area, i.e. between the y coordinates 0 to TableAreaHeight.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="VertScrollPos"></param>
		/// <param name="TableAreaHeight"></param>
		/// <returns></returns>
		public static int GetFullyVisiblePropertyColumns(WorksheetLayout layout, int VertScrollPos, double TableAreaHeight)
		{
			return GetFullyVisiblePropertyColumns(0, TableAreaHeight, layout, VertScrollPos);
		}

		public static int GetFullyVisiblePropertyColumns(double top, double bottom, WorksheetLayout layout, int VertScrollPos)
		{
			if (layout.ShowPropertyColumns)
			{
				int firstTotRow = (int)Math.Max(0, Math.Floor((top - layout.ColumnHeaderStyle.Height) / (double)layout.PropertyColumnHeaderStyle.Height));
				int lastTotRow = (int)Math.Floor((bottom - layout.ColumnHeaderStyle.Height) / (double)layout.PropertyColumnHeaderStyle.Height) - 1;
				int maxPossRows = Math.Max(0, GetRemainingEnabledPropertyColumns(layout, VertScrollPos) - firstTotRow);
				return Math.Min(maxPossRows, Math.Max(0, 1 + lastTotRow - firstTotRow));
			}
			else
				return 0;
		}

		/// <summary>Returns the remaining number of property columns that could be shown below the current scroll position.</summary>
		public static int GetRemainingEnabledPropertyColumns(WorksheetLayout layout, int VertScrollPos)
		{
			return layout.ShowPropertyColumns ? Math.Max(0, -VertScrollPos) : 0;
		}

		#endregion

		#region Data column

		public static int GetFirstAndNumberOfVisibleColumn(double left, double right, WorksheetLayout layout, int HorzScrollPos, out int numVisibleColumns)
		{

			var data = layout.DataTable.DataColumns;
			var nCols = data.ColumnCount;
			var colStyles = layout.DataColumnStyles;

			int nFirstCol = -1;
			int nLastCol = nCols; // Attention: this is one more than the index of the last column.
			double x = layout.RowHeaderStyle.Width;
			for (int nCol = HorzScrollPos; nCol < nCols; nCol++)
			{
				x += colStyles[data[nCol]].Width;

				if ((nFirstCol < 0) && (x > left))
				{
					nFirstCol = nCol;
				}

				if (x >= right)
				{
					nLastCol = nCol + 1;
					break;
				}
			}

			numVisibleColumns = nFirstCol < 0 ? 0 : Math.Max(0, nLastCol - nFirstCol);
			return nFirstCol;
		}

		public static int GetVisibleColumns(WorksheetLayout layout, int HorzScrollPos, double TableAreaWidth)
		{
			var data = layout.DataTable.DataColumns;
			var nCols = data.ColumnCount;
			var colStyles = layout.DataColumnStyles;

			int numCols = 0;
			double x = layout.RowHeaderStyle.Width;
			for (int nCol = HorzScrollPos; nCol < nCols; nCol++)
			{
				if (x < TableAreaWidth)
					numCols++;
				else
					break;

				x += colStyles[data[nCol]].Width;
			}
			return numCols;
		}

		public static int GetFullyVisibleColumns(WorksheetLayout layout, int HorzScrollPos, double TableAreaWidth)
		{
			var data = layout.DataTable.DataColumns;
			var nCols = data.ColumnCount;
			var colStyles = layout.DataColumnStyles;

			int numCols = 0;
			double x = layout.RowHeaderStyle.Width;
			for (int nCol = HorzScrollPos; nCol < nCols; nCol++)
			{
				x += colStyles[data[nCol]].Width;
				if (x <= TableAreaWidth)
					numCols++;
				else
					break;
			}
			return numCols;
		}

		public static int GetLastFullyVisibleColumn(WorksheetLayout layout, int HorzScrollPos, double TableAreaWidth)
		{
			return HorzScrollPos + GetFullyVisibleColumns(layout, HorzScrollPos, TableAreaWidth) - 1;
		}

		/// <summary>
		/// Gets the horizontal position and the width of a data column.
		/// </summary>
		/// <param name="nCol">Number of data column.</param>
		/// <param name="layout">Worksheet layout.</param>
		/// <param name="HorzScrollPos">Horizontal scroll position (number of the first column to display).</param>
		/// <param name="left">Returns the x coordinate of the left boundary of the specified column.</param>
		/// <param name="width">Returns the width of the specified column.</param>
		public static void GetXCoordinatesOfColumn(int nCol, WorksheetLayout layout, int HorzScrollPos, out double left, out double width)
		{
			int colOffs = nCol - HorzScrollPos;

			var data = layout.DataTable.DataColumns;
			var colStyles = layout.DataColumnStyles;
			double x = layout.RowHeaderStyle.Width;
			for (int i = HorzScrollPos; i < nCol; i++)
				x += colStyles[data[i]].Width;

			left = x;
			width = colStyles[data[nCol]].Width;
		}

		/// <summary>
		/// retrieves, to which column should be scrolled in order to make
		/// the column nForLastCol the last visible column
		/// </summary>
		/// <param name="nForLastCol">the column number which should be the last visible column</param>
		/// <param name="layout">Worksheet layout referring to.</param>
		/// <param name="HorzScrollPos">Value of the horizontal scroll bar.</param>
		/// <param name="TableAreaWidth">Width of the table area shown.</param>
		/// <returns>the number of the first visible column</returns>
		public static int GetFirstVisibleColumnForLastVisibleColumn(int nForLastCol, WorksheetLayout layout, int HorzScrollPos, double TableAreaWidth)
		{
			var data = layout.DataTable.DataColumns;
			var colStyles = layout.DataColumnStyles;

			int i = nForLastCol;
			int retv = nForLastCol;
			double horzSize = TableAreaWidth - layout.RowHeaderStyle.Width;
			while (i >= 0)
			{
				horzSize -= colStyles[data[i]].WidthD;
				if (horzSize > 0 && i > 0)
					i--;
				else
					break;
			}

			if (horzSize < 0)
				i++; // increase one colum if size was bigger than available size

			return i <= nForLastCol ? i : nForLastCol;
		}

		/// <summary>
		/// Get a list of horizontal positions, where the cursor should switch to a horizontal resizing cursor.
		/// </summary>
		/// <param name="destinationList">List to fill with the positions (old items will be deleted).</param>
		/// <param name="layout">Worksheet layout.</param>
		/// <param name="HorzScrollPos">Horizontal scroll position.</param>
		/// <param name="TableAreaWidth">Width of the table area.</param>
		/// <returns>HorzScrollPos that was given in the argument list.</returns>
		public static int GetColumnWidthResizingPositions(List<double> destinationList, WorksheetLayout layout, int HorzScrollPos, double TableAreaWidth)
		{
			destinationList.Clear();
			var data = layout.DataTable.DataColumns;
			var colStyles = layout.DataColumnStyles;
			double x = layout.RowHeaderStyle.WidthD;
			for (int i = HorzScrollPos; i < data.ColumnCount && x < TableAreaWidth; i++)
			{
				x += colStyles[data[i]].Width;
				destinationList.Add(x);
			}
			return HorzScrollPos;
		}


		#endregion

		#region Cell

		public static Altaxo.Graph.RectangleD GetCoordinatesOfDataCell(int nCol, int nRow, WorksheetLayout layout, int HorzScrollPos, int VertScrollPos)
		{
			double x, y, w, h;
			GetXCoordinatesOfColumn(nCol, layout, HorzScrollPos, out x, out w);

			y = GetTopCoordinateOfTableRow(nRow, layout, VertScrollPos);
			h = layout.RowHeaderStyle.Height;
			return new Altaxo.Graph.RectangleD(x, y, w, h);
		}

		public static Altaxo.Graph.RectangleD GetCoordinatesOfPropertyCell(int nCol, int nRow, WorksheetLayout layout, int HorzScrollPos, int VertScrollPos)
		{
			double x, y, w, h;
			GetXCoordinatesOfColumn(nRow, layout, HorzScrollPos, out x, out w);

			y = GetTopCoordinateOfPropertyColumn(nCol, layout, VertScrollPos);
			h = layout.PropertyColumnHeaderStyle.Height;
			return new Altaxo.Graph.RectangleD(x, y, w, h);
		}


		/// <summary>
		/// Retrieves the column number clicked onto 
		/// </summary>
		/// <param name="positionX">Clicked position.</param>
		/// <param name="layout">Worksheet layout referring to.</param>
		/// <param name="HorzScrollPos">Value of the horizontal scroll bar.</param>
		/// <param name="left">Returns the left position of the clicked cell.</param>
		/// <param name="width">Returns the width of the clicked cell.</param>
		/// <returns>Either -1 when clicked on the row header area, column number when clicked in the column range, or int.MinValue when clicked outside of all.</returns>
		public static int GetColumnNumber(double positionX, WorksheetLayout layout, int HorzScrollPos, out double left, out double width)
		{
			int firstVisibleColumn = HorzScrollPos;
			int actualColumnRight = layout.RowHeaderStyle.Width;
			int columnCount = layout.DataTable.DataColumns.ColumnCount;

			left = 0;

			if (positionX < actualColumnRight)
			{
				width = actualColumnRight;
				return -1;
			}

			for (int i = firstVisibleColumn; i < columnCount; i++)
			{
				left = actualColumnRight;
				Altaxo.Worksheet.ColumnStyle cs = layout.GetDataColumnStyle(i);
				actualColumnRight += cs.Width;
				if (actualColumnRight > positionX)
				{
					width = cs.Width;
					return i;
				}
			} // end for

			width = 0;
			return int.MinValue;
		}

		/// <summary>
		/// Returns the row number of the clicked cell.
		/// </summary>
		/// <param name="positionY">The vertical position value.</param>
		/// <param name="layout">The worksheet area.</param>
		/// <param name="VertScrollPos">Value of the vertical scroll bar.</param>
		/// <param name="top">Returns the top value of the clicked cell.</param>
		/// <param name="height">Returns the height value of the clicked cell.</param>
		/// <param name="bPropertyCol">True if clicked on either the property column header or a property column, else false.</param>
		/// <returns>The row number of the clicked cell, or -1 if clicked on the column header.</returns>
		/// <remarks>If clicked onto a property cell, the function returns the property column number.</remarks>
		public static int GetRowNumber(double positionY, WorksheetLayout layout, int VertScrollPos, out double top, out double height, out bool bPropertyCol)
		{
			top = 0;
			if (positionY < layout.ColumnHeaderStyle.Height)
			{
				height = layout.ColumnHeaderStyle.Height;
				bPropertyCol = false;
				return -1;
			}

			var verticalPositionOfFirstVisibleDataRow = GetVerticalPositionOfFirstVisibleDataRow(layout, VertScrollPos);
			var visiblePropertyColumns = GetVisiblePropertyColumns(0, positionY * 1.125, layout, VertScrollPos); // trick here: because coordinates lower than positionY are not relevant, we use a clipping region slightly larger than positionY

			if (positionY < verticalPositionOfFirstVisibleDataRow && visiblePropertyColumns > 0)
			{
				// calculate the raw row number
				int rawrow = (int)Math.Floor((positionY - layout.ColumnHeaderStyle.Height) / (double)layout.PropertyColumnHeaderStyle.Height);

				top = layout.ColumnHeaderStyle.Height + rawrow * layout.PropertyColumnHeaderStyle.Height;
				height = layout.PropertyColumnHeaderStyle.Height;

				bPropertyCol = true;
				return rawrow + GetFirstVisiblePropertyColumn(layout, VertScrollPos);
			}
			else
			{
				int rawrow = (int)Math.Floor((positionY - verticalPositionOfFirstVisibleDataRow) / (double)layout.RowHeaderStyle.Height);

				top = verticalPositionOfFirstVisibleDataRow + rawrow * layout.RowHeaderStyle.Height;
				height = layout.RowHeaderStyle.Height;
				bPropertyCol = false;
				return rawrow + GetFirstVisibleTableRow(layout, VertScrollPos);
			}
		}



		/// <summary>
		/// Creates the ClickedCellInfo from the data grid and the mouse coordinates of the click.
		/// </summary>
		/// <param name="positionX">The horizontal position value.</param>
		/// <param name="positionY">The vertical position value.</param>
		/// <param name="layout">The worksheet area.</param>
		/// <param name="HorzScrollPos">Value of the horizontal scroll bar.</param>
		/// <param name="VertScrollPos">Value of the vertical scroll bar.</param>
		public static AreaInfo GetAreaType(double positionX, double positionY, WorksheetLayout layout, int HorzScrollPos, int VertScrollPos)
		{
			bool bIsPropertyColumn = false;
			double left, top, width, height; // coordinates of clicked cell
			//var _clickedCellRectangle = new Rect(0,0,0,0);
			var hittedColumn = GetColumnNumber(positionX, layout, HorzScrollPos, out left, out width);
			var hittedRow = GetRowNumber(positionY, layout, VertScrollPos, out top, out height, out bIsPropertyColumn);
			AreaType hittedAreaType;

			if (bIsPropertyColumn)
			{
				if (hittedColumn == -1)
					hittedAreaType = AreaType.PropertyColumnHeader;
				else if (hittedColumn >= 0)
					hittedAreaType = AreaType.PropertyCell;
				else
					hittedAreaType = AreaType.OutsideAll;

				// Swap columns and rows since it is a property column
				int h = hittedColumn;
				hittedColumn = hittedRow;
				hittedRow = h;
			}
			else // it is not a property related cell
			{
				if (hittedRow == -1 && hittedColumn == -1)
					hittedAreaType = AreaType.TableHeader;
				else if (hittedRow == -1 && hittedColumn >= 0)
					hittedAreaType = AreaType.DataColumnHeader;
				else if (hittedRow >= 0 && hittedColumn == -1)
					hittedAreaType = AreaType.DataRowHeader;
				else if (hittedRow >= 0 && hittedColumn >= 0)
					hittedAreaType = AreaType.DataCell;
				else
					hittedAreaType = AreaType.OutsideAll;
			}

			AreaInfo result = new AreaInfo()
			{
				ColumnNumber = hittedColumn,
				RowNumber = hittedRow,
				AreaRectangle = new Altaxo.Graph.RectangleD(left, top, width, height),
				AreaType = hittedAreaType
			};
			return result;
		}



		#endregion
	}
}
