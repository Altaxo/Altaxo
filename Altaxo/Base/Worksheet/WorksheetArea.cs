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
using Altaxo.Geometry;

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

  /// <summary>
  /// Describes a worksheet area hit together with its coordinates and indices.
  /// </summary>
  public struct AreaInfo
  {
    /// <summary>
    /// Gets or sets the hit rectangle.
    /// </summary>
    public RectangleD2D AreaRectangle { get; set; }

    /// <summary>
    /// Gets or sets the hit area type.
    /// </summary>
    public AreaType AreaType { get; set; }

    /// <summary>
    /// Gets or sets the row number.
    /// </summary>
    public int RowNumber { get; set; }

    /// <summary>
    /// Gets or sets the column number.
    /// </summary>
    public int ColumnNumber { get; set; }
  }

  /// <summary>
  /// Provides helper methods for mapping worksheet coordinates to visible rows, columns, and cells.
  /// </summary>
  public static class AreaRetrieval
  {
    #region Data Row

    /// <summary>
    /// This returns the vertical position of the first visible data row.;
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <returns>The vertical position of the first visible data row.</returns>
    public static int GetVerticalPositionOfFirstVisibleDataRow(WorksheetLayout layout, int VertScrollPos)
    {
      return layout.ColumnHeaderStyle.Height + (VertScrollPos >= 0 ? 0 : -VertScrollPos * layout.PropertyColumnHeaderStyle.Height);
    }

    /// <summary>
    /// Gets the first table row that is visible.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
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
      int firstVis = (int)Math.Floor((top - posOfDataRow0) / layout.RowHeaderStyle.Height);
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

      int firstRow = (int)Math.Floor((top - posOfDataRow0) / layout.RowHeaderStyle.Height);
      int lastRow = (int)Math.Ceiling((bottom - posOfDataRow0) / layout.RowHeaderStyle.Height) - 1;
      return Math.Max(0, 1 + lastRow - firstRow);
    }

    /// <summary>
    /// Gets the last visible table row.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <param name="TableAreaHeight">The height of the table area.</param>
    /// <returns>The last visible table row.</returns>
    public static int GetLastVisibleTableRow(WorksheetLayout layout, int VertScrollPos, double TableAreaHeight)
    {
      return GetFirstVisibleTableRow(layout, VertScrollPos) + GetVisibleTableRows(0, TableAreaHeight, layout, VertScrollPos) - 1;
    }

    /// <summary>
    /// Gets the last fully visible table row.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <param name="TableAreaHeight">The height of the table area.</param>
    /// <returns>The last fully visible table row.</returns>
    public static int GetLastFullyVisibleTableRow(WorksheetLayout layout, int VertScrollPos, double TableAreaHeight)
    {
      return GetFirstVisibleTableRow(layout, VertScrollPos) + GetFullyVisibleTableRows(0, TableAreaHeight, layout, VertScrollPos) - 1;
    }

    /// <summary>
    /// Gets the number of fully visible table rows between two vertical coordinates.
    /// </summary>
    /// <param name="top">The top y coordinate.</param>
    /// <param name="bottom">The bottom y coordinate.</param>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <returns>The number of fully visible table rows.</returns>
    public static int GetFullyVisibleTableRows(double top, double bottom, WorksheetLayout layout, int VertScrollPos)
    {
      int posOfDataRow0 = GetVerticalPositionOfFirstVisibleDataRow(layout, VertScrollPos);

      if (top < posOfDataRow0)
        top = posOfDataRow0;

      int firstRow = (int)Math.Floor((top - posOfDataRow0) / layout.RowHeaderStyle.Height);
      int lastRow = (int)Math.Floor((bottom - posOfDataRow0) / layout.RowHeaderStyle.Height) - 1;
      return Math.Max(0, 1 + lastRow - firstRow);
    }

    /// <summary>
    /// Gets the top coordinate of the specified table row.
    /// </summary>
    /// <param name="nRow">The row index.</param>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <returns>The top coordinate of the table row.</returns>
    public static int GetTopCoordinateOfTableRow(int nRow, WorksheetLayout layout, int VertScrollPos)
    {
      return GetVerticalPositionOfFirstVisibleDataRow(layout, VertScrollPos) + (nRow - (VertScrollPos < 0 ? 0 : VertScrollPos)) * layout.RowHeaderStyle.Height;
    }

    #endregion Data Row

    #region Property column

    /// <summary>
    /// Gets the number of enabled property columns.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <returns>The number of enabled property columns.</returns>
    public static int GetTotalEnabledPropertyColumns(WorksheetLayout layout)
    {
      return layout.ShowPropertyColumns ? layout.DataTable.PropertyColumnCount : 0;
    }

    /// <summary>
    /// Gets the first visible property column.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <returns>The first visible property column, or <c>-1</c> if no property column is visible.</returns>
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

    /// <summary>
    /// Gets the first visible property column below the specified top coordinate.
    /// </summary>
    /// <param name="top">The top y coordinate.</param>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <returns>The first visible property column below the specified coordinate, or <c>-1</c> if none is visible.</returns>
    public static int GetFirstVisiblePropertyColumn(double top, WorksheetLayout layout, int VertScrollPos)
    {
      if (VertScrollPos >= 0 || !layout.ShowPropertyColumns)
        return -1;

      int firstTotRow = (int)Math.Max(0, Math.Floor((top - layout.ColumnHeaderStyle.Height) / layout.PropertyColumnHeaderStyle.Height));
      return firstTotRow + GetFirstVisiblePropertyColumn(layout, VertScrollPos);
    }

    /// <summary>
    /// Gets the last fully visible property column.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <param name="TableAreaHeight">The height of the table area.</param>
    /// <returns>The last fully visible property column.</returns>
    public static int GetLastFullyVisiblePropertyColumn(WorksheetLayout layout, int VertScrollPos, double TableAreaHeight)
    {
      return GetFirstVisiblePropertyColumn(layout, VertScrollPos) + GetFullyVisiblePropertyColumns(layout, VertScrollPos, TableAreaHeight) - 1;
    }

    /// <summary>
    /// Gets the top coordinate of the specified property column.
    /// </summary>
    /// <param name="nCol">The property column index.</param>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <returns>The top coordinate of the property column.</returns>
    public static int GetTopCoordinateOfPropertyColumn(int nCol, WorksheetLayout layout, int VertScrollPos)
    {
      return layout.ColumnHeaderStyle.Height + (nCol - GetFirstVisiblePropertyColumn(layout, VertScrollPos)) * layout.PropertyColumnHeaderStyle.Height;
    }

    /// <summary>
    /// Gets the number of visible property columns between two vertical coordinates.
    /// </summary>
    /// <param name="top">The top y coordinate.</param>
    /// <param name="bottom">The bottom y coordinate.</param>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <returns>The number of visible property columns.</returns>
    public static int GetVisiblePropertyColumns(double top, double bottom, WorksheetLayout layout, int VertScrollPos)
    {
      if (layout.ShowPropertyColumns)
      {
        int firstTotRow = (int)Math.Max(0, Math.Floor((top - layout.ColumnHeaderStyle.Height) / layout.PropertyColumnHeaderStyle.Height));
        int lastTotRow = (int)Math.Ceiling((bottom - layout.ColumnHeaderStyle.Height) / layout.PropertyColumnHeaderStyle.Height) - 1;
        int maxPossRows = Math.Max(0, GetRemainingEnabledPropertyColumns(layout, VertScrollPos) - firstTotRow);
        return Math.Min(maxPossRows, Math.Max(0, 1 + lastTotRow - firstTotRow));
      }
      else
        return 0;
    }

    /// <summary>
    /// Gets the number of fully visible property columns in the full table area, i.e. between the y coordinates 0 to TableAreaHeight.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <param name="TableAreaHeight">The height of the table area.</param>
    /// <returns>The number of fully visible property columns.</returns>
    public static int GetFullyVisiblePropertyColumns(WorksheetLayout layout, int VertScrollPos, double TableAreaHeight)
    {
      return GetFullyVisiblePropertyColumns(0, TableAreaHeight, layout, VertScrollPos);
    }

    /// <summary>
    /// Gets the number of fully visible property columns between two vertical coordinates.
    /// </summary>
    /// <param name="top">The top y coordinate.</param>
    /// <param name="bottom">The bottom y coordinate.</param>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <returns>The number of fully visible property columns.</returns>
    public static int GetFullyVisiblePropertyColumns(double top, double bottom, WorksheetLayout layout, int VertScrollPos)
    {
      if (layout.ShowPropertyColumns)
      {
        int firstTotRow = (int)Math.Max(0, Math.Floor((top - layout.ColumnHeaderStyle.Height) / layout.PropertyColumnHeaderStyle.Height));
        int lastTotRow = (int)Math.Floor((bottom - layout.ColumnHeaderStyle.Height) / layout.PropertyColumnHeaderStyle.Height) - 1;
        int maxPossRows = Math.Max(0, GetRemainingEnabledPropertyColumns(layout, VertScrollPos) - firstTotRow);
        return Math.Min(maxPossRows, Math.Max(0, 1 + lastTotRow - firstTotRow));
      }
      else
        return 0;
    }

    /// <summary>Returns the remaining number of property columns that could be shown below the current scroll position.</summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <returns>The remaining number of enabled property columns.</returns>
    public static int GetRemainingEnabledPropertyColumns(WorksheetLayout layout, int VertScrollPos)
    {
      return layout.ShowPropertyColumns ? Math.Max(0, -VertScrollPos) : 0;
    }

    #endregion Property column

    #region Data column

    /// <summary>
    /// Gets the first visible data column and the number of visible columns.
    /// </summary>
    /// <param name="left">The left x coordinate.</param>
    /// <param name="right">The right x coordinate.</param>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="HorzScrollPos">The horizontal scroll position.</param>
    /// <param name="numVisibleColumns">Receives the number of visible columns.</param>
    /// <returns>The first visible data column.</returns>
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

    /// <summary>
    /// Gets the number of visible data columns.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="HorzScrollPos">The horizontal scroll position.</param>
    /// <param name="TableAreaWidth">The width of the table area.</param>
    /// <returns>The number of visible data columns.</returns>
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

    /// <summary>
    /// Gets the number of fully visible data columns.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="HorzScrollPos">The horizontal scroll position.</param>
    /// <param name="TableAreaWidth">The width of the table area.</param>
    /// <returns>The number of fully visible data columns.</returns>
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

    /// <summary>
    /// Gets the last fully visible data column.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="HorzScrollPos">The horizontal scroll position.</param>
    /// <param name="TableAreaWidth">The width of the table area.</param>
    /// <returns>The last fully visible data column.</returns>
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
    /// Gets the first visible column required to show a given last visible column.
    /// </summary>
    /// <param name="nForLastCol">The column number that should become the last visible column.</param>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="HorzScrollPos">The horizontal scroll position.</param>
    /// <param name="TableAreaWidth">The width of the visible table area.</param>
    /// <returns>The number of the first visible column.</returns>
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

    #endregion Data column

    #region Cell

    /// <summary>
    /// Gets the coordinates of a data cell.
    /// </summary>
    /// <param name="nCol">The data column index.</param>
    /// <param name="nRow">The data row index.</param>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="HorzScrollPos">The horizontal scroll position.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <returns>The rectangle describing the data cell.</returns>
    public static RectangleD2D GetCoordinatesOfDataCell(int nCol, int nRow, WorksheetLayout layout, int HorzScrollPos, int VertScrollPos)
    {
      double y, h;
      GetXCoordinatesOfColumn(nCol, layout, HorzScrollPos, out var x, out var w);

      y = GetTopCoordinateOfTableRow(nRow, layout, VertScrollPos);
      h = layout.RowHeaderStyle.Height;
      return new RectangleD2D(x, y, w, h);
    }

    /// <summary>
    /// Gets the coordinates of a property cell.
    /// </summary>
    /// <param name="nCol">The property column index.</param>
    /// <param name="nRow">The data column index.</param>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="HorzScrollPos">The horizontal scroll position.</param>
    /// <param name="VertScrollPos">The vertical scroll position.</param>
    /// <returns>The rectangle describing the property cell.</returns>
    public static RectangleD2D GetCoordinatesOfPropertyCell(int nCol, int nRow, WorksheetLayout layout, int HorzScrollPos, int VertScrollPos)
    {
      double y, h;
      GetXCoordinatesOfColumn(nRow, layout, HorzScrollPos, out var x, out var w);

      y = GetTopCoordinateOfPropertyColumn(nCol, layout, VertScrollPos);
      h = layout.PropertyColumnHeaderStyle.Height;
      return new RectangleD2D(x, y, w, h);
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
        int rawrow = (int)Math.Floor((positionY - layout.ColumnHeaderStyle.Height) / layout.PropertyColumnHeaderStyle.Height);

        top = layout.ColumnHeaderStyle.Height + rawrow * layout.PropertyColumnHeaderStyle.Height;
        height = layout.PropertyColumnHeaderStyle.Height;

        bPropertyCol = true;
        return rawrow + GetFirstVisiblePropertyColumn(layout, VertScrollPos);
      }
      else
      {
        int rawrow = (int)Math.Floor((positionY - verticalPositionOfFirstVisibleDataRow) / layout.RowHeaderStyle.Height);

        top = verticalPositionOfFirstVisibleDataRow + rawrow * layout.RowHeaderStyle.Height;
        height = layout.RowHeaderStyle.Height;
        bPropertyCol = false;
        return rawrow + GetFirstVisibleTableRow(layout, VertScrollPos);
      }
    }

    /// <summary>
    /// Creates area information from worksheet coordinates.
    /// </summary>
    /// <param name="positionX">The horizontal position value.</param>
    /// <param name="positionY">The vertical position value.</param>
    /// <param name="layout">The worksheet area.</param>
    /// <param name="HorzScrollPos">Value of the horizontal scroll bar.</param>
    /// <param name="VertScrollPos">Value of the vertical scroll bar.</param>
    /// <returns>The area information for the specified position.</returns>
    public static AreaInfo GetAreaType(double positionX, double positionY, WorksheetLayout layout, int HorzScrollPos, int VertScrollPos)
    {
      //var _clickedCellRectangle = new Rect(0,0,0,0);
      var hittedColumn = GetColumnNumber(positionX, layout, HorzScrollPos, out var left, out var width);
      var hittedRow = GetRowNumber(positionY, layout, VertScrollPos, out var top, out var height, out var bIsPropertyColumn);
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

      var result = new AreaInfo()
      {
        ColumnNumber = hittedColumn,
        RowNumber = hittedRow,
        AreaRectangle = new RectangleD2D(left, top, width, height),
        AreaType = hittedAreaType
      };
      return result;
    }

    #endregion Cell
  }
}
