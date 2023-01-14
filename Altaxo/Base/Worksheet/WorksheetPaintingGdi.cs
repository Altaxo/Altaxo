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
using System.Drawing;
using Altaxo.Collections;
using Altaxo.Data;

namespace Altaxo.Worksheet
{
  using Geometry;
  using WA = Altaxo.Worksheet.AreaRetrieval;

  /// <summary>
  /// Methods for drawing a worksheet using Gdi
  /// </summary>
  public class WorksheetPaintingGdi
  {
    public static bool ArePropertyCellsSelected(DataTable table, IAscendingIntegerCollection selectedPropertyColumns, IAscendingIntegerCollection selectedPropertyRows)
    {
      return table.PropCols.ColumnCount > 0 && (selectedPropertyColumns.Count > 0 || selectedPropertyRows.Count > 0);
    }

    /// <summary>
    /// Paints part of the worksheet to the drawing context. Row and column header are always threaten as visible here.
    /// </summary>
    /// <param name="dc">Drawing context.</param>
    /// <param name="layout">Worksheet layout.</param>
    /// <param name="viewSize">Width and height of the viewing area (Pixel or Wpf coordinates).</param>
    /// <param name="clipRectangle">Bounds of the clipping region. Only that parts of the worksheet that are visible within the clipping region are drawn.</param>
    /// <param name="selectedDataColumns">Selected data columns.</param>
    /// <param name="selectedDataRows">Selected data rows.</param>
    /// <param name="selectedPropertyColumns">Selected property columns.</param>
    /// <param name="selectedPropertyRows">Selected property rows.</param>
    /// <param name="horzScrollPos">Horizontal scroll position (0 = first column visible).</param>
    /// <param name="vertScrollPos">Vertical scroll position (0 = first data column visible, negative values: one or more property columns visible).</param>
    public static void PaintTableArea(
      Graphics dc,
      Altaxo.Worksheet.WorksheetLayout layout,
      Size viewSize,
      RectangleD2D clipRectangle,
      IAscendingIntegerCollection selectedDataColumns,
      IAscendingIntegerCollection selectedDataRows,
      IAscendingIntegerCollection selectedPropertyColumns,
      IAscendingIntegerCollection selectedPropertyRows,
      int horzScrollPos, int vertScrollPos
      )
    {
      var dataTable = layout.DataTable;

      bool bDrawColumnHeader = false;

      int firstTableRowToDraw = WA.GetFirstVisibleTableRow(clipRectangle.Top, layout, vertScrollPos);
      int numberOfTableRowsToDraw = WA.GetVisibleTableRows(clipRectangle.Top, clipRectangle.Bottom, layout, vertScrollPos);

      int firstPropertyColumnToDraw = WA.GetFirstVisiblePropertyColumn(clipRectangle.Top, layout, vertScrollPos);
      int numberOfPropertyColumnsToDraw = WA.GetVisiblePropertyColumns(clipRectangle.Top, clipRectangle.Bottom, layout, vertScrollPos);

      bool areColumnsSelected = selectedDataColumns.Count > 0;
      bool bAreRowsSelected = selectedDataRows.Count > 0;
      bool bAreCellsSelected = bAreRowsSelected || areColumnsSelected;

      bool bArePropertyColsSelected = selectedPropertyColumns.Count > 0;
      bool arePropertyRowsSelected = selectedPropertyRows.Count > 0;
      bool bArePropertyCellsSelected = ArePropertyCellsSelected(dataTable, selectedPropertyColumns, selectedPropertyRows);

      int yShift = 0;

      var cellRectangle = new RectangleD2D();
      double left, width;

      if (clipRectangle.Top < layout.ColumnHeaderStyle.Height)
      {
        bDrawColumnHeader = true;
      }

      // if neccessary, draw the row header (the most left column)
      if (clipRectangle.Left < layout.RowHeaderStyle.Width)
      {
        cellRectangle.Height = layout.ColumnHeaderStyle.Height;
        cellRectangle.Width = layout.RowHeaderStyle.Width;
        cellRectangle.X = 0;

        // if visible, draw the top left corner of the table
        if (bDrawColumnHeader)
        {
          cellRectangle.Y = 0;
          layout.RowHeaderStyle.PaintBackground(dc, cellRectangle.ToGdiRectangle(), false);
        }

        // if visible, draw property column header items
        yShift = WA.GetTopCoordinateOfPropertyColumn(firstPropertyColumnToDraw, layout, vertScrollPos);
        cellRectangle.Height = layout.PropertyColumnHeaderStyle.Height;
        for (int nPropCol = firstPropertyColumnToDraw, nInc = 0; nInc < numberOfPropertyColumnsToDraw; nPropCol++, nInc++)
        {
          cellRectangle.Y = yShift + nInc * layout.PropertyColumnHeaderStyle.Height;
          bool bPropColSelected = bArePropertyColsSelected && selectedPropertyColumns.Contains(nPropCol);
          layout.PropertyColumnHeaderStyle.Paint(dc, cellRectangle.ToGdiRectangle(), nPropCol, dataTable.PropCols[nPropCol], bPropColSelected);
        }
      }

      // draw the table row Header Items
      yShift = WA.GetTopCoordinateOfTableRow(firstTableRowToDraw, layout, vertScrollPos);
      cellRectangle.Height = layout.RowHeaderStyle.Height;
      for (int nRow = firstTableRowToDraw, nInc = 0; nInc < numberOfTableRowsToDraw; nRow++, nInc++)
      {
        cellRectangle.Y = yShift + nInc * layout.RowHeaderStyle.Height;
        layout.RowHeaderStyle.Paint(dc, cellRectangle.ToGdiRectangle(), nRow, null, bAreRowsSelected && selectedDataRows.Contains(nRow));
      }

      if (clipRectangle.Bottom >= layout.ColumnHeaderStyle.Height || clipRectangle.Right >= layout.RowHeaderStyle.Width)
      {
        int firstColToDraw = WA.GetFirstAndNumberOfVisibleColumn(clipRectangle.Left, clipRectangle.Right, layout, horzScrollPos, out var numberOfColumnsToDraw);

        // draw the property columns
        for (int nPropCol = firstPropertyColumnToDraw, nIncPropCol = 0; nIncPropCol < numberOfPropertyColumnsToDraw; nPropCol++, nIncPropCol++)
        {
          Altaxo.Worksheet.ColumnStyle cs = layout.PropertyColumnStyles[dataTable.PropCols[nPropCol]];
          bool bPropColSelected = bArePropertyColsSelected && selectedPropertyColumns.Contains(nPropCol);
          bool bPropColIncluded = bArePropertyColsSelected ? bPropColSelected : true; // Property cells are only included if the column is explicite selected

          cellRectangle.Y = WA.GetTopCoordinateOfPropertyColumn(nPropCol, layout, vertScrollPos);
          cellRectangle.Height = layout.PropertyColumnHeaderStyle.Height;

          for (int nCol = firstColToDraw, nIncCol = 0; nIncCol < numberOfColumnsToDraw; nCol++, nIncCol++)
          {
            if (nCol == firstColToDraw)
            {
              WA.GetXCoordinatesOfColumn(nCol, layout, horzScrollPos, out left, out width);
              cellRectangle.X = left;
              cellRectangle.Width = width;
            }
            else
            {
              cellRectangle.X += cellRectangle.Width;
              cellRectangle.Width = layout.DataColumnStyles[dataTable.DataColumns[nCol]].WidthD;
            }

            bool bPropRowSelected = arePropertyRowsSelected && selectedPropertyRows.Contains(nCol);
            bool bPropRowIncluded = arePropertyRowsSelected ? bPropRowSelected : true;

            cs.Paint(dc, cellRectangle.ToGdiRectangle(), nCol, dataTable.PropCols[nPropCol], bArePropertyCellsSelected && bPropColIncluded && bPropRowIncluded);
          }
        }

        // draw the cells
        for (int nCol = firstColToDraw, nIncCol = 0; nIncCol < numberOfColumnsToDraw; nCol++, nIncCol++)
        {
          Altaxo.Worksheet.ColumnStyle cs = layout.DataColumnStyles[dataTable.DataColumns[nCol]];
          if (nCol == firstColToDraw)
          {
            WA.GetXCoordinatesOfColumn(nCol, layout, horzScrollPos, out left, out width);
            cellRectangle.X = left;
            cellRectangle.Width = width;
          }
          else
          {
            cellRectangle.X += cellRectangle.Width;
            cellRectangle.Width = cs.WidthD;
          }

          bool isColumnSelected = areColumnsSelected && selectedDataColumns.Contains(nCol);
          bool isDataColumnIncluded = areColumnsSelected ? isColumnSelected : true;
          bool isPropertyRowSelected = arePropertyRowsSelected && selectedPropertyRows.Contains(nCol);

          if (bDrawColumnHeader) // must the column Header been drawn?
          {
            cellRectangle.Height = layout.ColumnHeaderStyle.Height;
            cellRectangle.Y = 0;
            layout.ColumnHeaderStyle.Paint(dc, cellRectangle.ToGdiRectangle(), 0, dataTable[nCol], isColumnSelected || isPropertyRowSelected);
          }

          yShift = WA.GetTopCoordinateOfTableRow(firstTableRowToDraw, layout, vertScrollPos);
          cellRectangle.Height = layout.RowHeaderStyle.Height;
          for (int nRow = firstTableRowToDraw, nIncRow = 0; nIncRow < numberOfTableRowsToDraw; nRow++, nIncRow++)
          {
            bool bRowSelected = bAreRowsSelected && selectedDataRows.Contains(nRow);
            bool bDataRowIncluded = bAreRowsSelected ? bRowSelected : true;
            cellRectangle.Y = yShift + nIncRow * layout.RowHeaderStyle.Height;
            cs.Paint(dc, cellRectangle.ToGdiRectangle(), nRow, dataTable[nCol], bAreCellsSelected && isDataColumnIncluded && bDataRowIncluded);
          }
        }
      }
    }
  }
}
