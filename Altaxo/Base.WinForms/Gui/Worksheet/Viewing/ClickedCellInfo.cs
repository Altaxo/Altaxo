#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Windows.Forms;
using System.Drawing;

using Altaxo.Worksheet;

namespace Altaxo.Gui.Worksheet.Viewing
{
  /// <remarks>
  /// ClickedCellInfo retrieves (from mouse coordinates of a click), which cell has clicked onto. 
  /// </remarks>
  public class ClickedCellInfo : System.EventArgs
  {
    /// <summary>Gets which mouse buttons were pressed on the last mouseup/mousedown event</summary>
    private System.Windows.Forms.MouseButtons _lastMouseButtons;

    /// <summary>Gets the mouse position of the first mousedown event (after all buttons were up).</summary>
    private Point _mousePositionFirstDown;

    /// <summary>
    /// Gets the mouse position of the last mouseup event (so that all buttons are up afterwards).
    /// </summary>
    private Point _mousePositionLastUp;


    /// <summary>
    /// Gets the mouse button that was first pressed down
    /// </summary>
    private MouseButtons _mouseButtonFirstDown;


    /// <summary>
    /// Gets the mouse button that was release last (before all mouse buttons were up).
    /// </summary>
    private MouseButtons _mouseButtonLastUp;

    /// <summary>The enclosing Rectangle of the clicked cell</summary>
    private Rectangle _clickedCellRectangle;

    /// <summary>The data row clicked onto.</summary>
    private int _clickedRow;
    /// <summary>The data column number clicked onto.</summary>
    private int _clickedColumn;

    /// <summary>What have been clicked onto.</summary>
    private AreaType _clickedAreaType;


    /// <summary>
    /// Gets which mouse buttons were pressed on the last mouseup/mousedown event
    /// </summary>
    public System.Windows.Forms.MouseButtons LastMouseButtons
    {
      get
      {
        return _lastMouseButtons;
      }
    }

    /// <summary>
    /// Gets the mouse position of the first mousedown event (after all buttons were up).
    /// </summary>
    public Point MousePositionFirstDown
    {
      get
      {
        return _mousePositionFirstDown;
      }
    }

    /// <summary>
    /// Gets the mouse button that was first pressed down
    /// </summary>
    public MouseButtons MouseButtonFirstDown
    {
      get { return _mouseButtonFirstDown; }
    }

    /// <summary>
    /// Gets the mouse position of the last mouseup event (so that all buttons are up afterwards).
    /// </summary>
    public Point MousePositionLastUp
    {
      get
      {
        return _mousePositionLastUp;
      }
    }

    /// <summary>
    /// Gets the mouse button that was release last (before all mouse buttons were up).
    /// </summary>
    public MouseButtons MouseButtonLastUp
    {
      get { return _mouseButtonLastUp; }
    }

    public void MouseDown(MouseEventArgs e)
    {
      if(_lastMouseButtons==MouseButtons.None)
      {
        this._mousePositionFirstDown = new Point(e.X,e.Y);
        this._mouseButtonFirstDown = e.Button;
      }
      _lastMouseButtons = e.Button;
    }

    public void MouseUp(MouseEventArgs e, MouseButtons buttonsAfterwards)
    {
      if(buttonsAfterwards==MouseButtons.None)
      {
        this._mousePositionLastUp = new Point(e.X,e.Y);
        this._mouseButtonLastUp = e.Button;
      }
      _lastMouseButtons = buttonsAfterwards;
    }


   


    /// <value>The enclosing Rectangle of the clicked cell</value>
    public Rectangle CellRectangle { get { return _clickedCellRectangle; }}

    /// <value>The row number clicked onto.</value>
    public int Row 
    {
      get { return _clickedRow; }
      set { _clickedRow = value; }
    }
    /// <value>The column number clicked onto.</value>
    public int Column 
    {
      get { return _clickedColumn; }
      set { _clickedColumn = value; }
    }
    /// <value>The type of area clicked onto.</value>
    public AreaType ClickedArea { get { return _clickedAreaType; }}
 
    /// <summary>
    /// Retrieves the column number clicked onto 
    /// </summary>
    /// <param name="dg">The parent data grid</param>
    /// <param name="mouseCoord">The coordinates of the mouse click.</param>
    /// <param name="cellRect">The function sets the x-properties (X and Width) of the cell rectangle.</param>
    /// <returns>Either -1 when clicked on the row header area, column number when clicked in the column range, or int.MinValue when clicked outside of all.</returns>
    public static int GetColumnNumber(WinFormsWorksheetController dg, Point mouseCoord, ref Rectangle cellRect)
    {
      int firstVisibleColumn = dg.FirstVisibleColumn;
      int actualColumnRight = dg.WorksheetLayout.RowHeaderStyle.Width;
      int columnCount = dg.DataTable.DataColumns.ColumnCount;

      if(mouseCoord.X<actualColumnRight)
      {
        cellRect.X=0; cellRect.Width=actualColumnRight;
        return -1;
      }

      for(int i=firstVisibleColumn;i<columnCount;i++)
      {
        cellRect.X=actualColumnRight;
        Altaxo.Worksheet.ColumnStyle cs = dg.GetDataColumnStyle(i);
        actualColumnRight += cs.Width;
        if(actualColumnRight>mouseCoord.X)
        {
          cellRect.Width = cs.Width;
          return i;
        }
      } // end for
      return int.MinValue;
    }

    /// <summary>
    /// Returns the row number of the clicked cell.
    /// </summary>
    /// <param name="dg">The parent WorksheetController.</param>
    /// <param name="mouseCoord">The mouse coordinates of the click.</param>
    /// <param name="cellRect">Returns the bounding rectangle of the clicked cell.</param>
    /// <param name="bPropertyCol">True if clicked on either the property column header or a property column, else false.</param>
    /// <returns>The row number of the clicked cell, or -1 if clicked on the column header.</returns>
    /// <remarks>If clicked onto a property cell, the function returns the property column number.</remarks>
    public static int GetRowNumber(WinFormsWorksheetController dg, Point mouseCoord, ref Rectangle cellRect, out bool bPropertyCol)
    {
      int firstVisibleColumn = dg.FirstVisibleColumn;
      int actualColumnRight = dg.WorksheetLayout.RowHeaderStyle.Width;
      int columnCount = dg.DataTable.DataColumns.ColumnCount;

      if(mouseCoord.Y<dg.WorksheetLayout.ColumnHeaderStyle.Height)
      {
        cellRect.Y=0; cellRect.Height=dg.WorksheetLayout.ColumnHeaderStyle.Height;
        bPropertyCol=false;
        return -1;
      }

      if(mouseCoord.Y<dg.VerticalPositionOfFirstVisibleDataRow && dg.VisiblePropertyColumns>0)
      {
        // calculate the raw row number
        int rawrow = (int)Math.Floor((mouseCoord.Y-dg.WorksheetLayout.ColumnHeaderStyle.Height)/(double)dg.WorksheetLayout.PropertyColumnHeaderStyle.Height);

        cellRect.Y= dg.WorksheetLayout.ColumnHeaderStyle.Height + rawrow * dg.WorksheetLayout.PropertyColumnHeaderStyle.Height;
        cellRect.Height = dg.WorksheetLayout.PropertyColumnHeaderStyle.Height;

        bPropertyCol=true;
        return dg.FirstVisiblePropertyColumn+rawrow;
      }
      else
      {
        int rawrow = (int)Math.Floor((mouseCoord.Y-dg.VerticalPositionOfFirstVisibleDataRow)/(double)dg.WorksheetLayout.RowHeaderStyle.Height);

        cellRect.Y= dg.VerticalPositionOfFirstVisibleDataRow + rawrow * dg.WorksheetLayout.RowHeaderStyle.Height;
        cellRect.Height = dg.WorksheetLayout.RowHeaderStyle.Height;
        bPropertyCol=false;
        return dg.FirstVisibleTableRow + rawrow;
      }
    }


    public ClickedCellInfo()
    {
    }


    /// <summary>
    /// Creates the ClickedCellInfo from the data grid and the mouse coordinates of the click.
    /// </summary>
    /// <param name="dg">The data grid.</param>
    /// <param name="mouseCoord">The mouse coordinates of the click.</param>
    public void MouseClick(WinFormsWorksheetController dg, Point mouseCoord)
    {

      bool bIsPropertyColumn=false;
      _clickedCellRectangle = new Rectangle(0,0,0,0);
      _clickedColumn = GetColumnNumber(dg,mouseCoord, ref _clickedCellRectangle);
      _clickedRow    = GetRowNumber(dg,mouseCoord,ref _clickedCellRectangle, out bIsPropertyColumn);

      if(bIsPropertyColumn)
      {
        if(_clickedColumn==-1)
          _clickedAreaType = AreaType.PropertyColumnHeader;
        else if(_clickedColumn>=0)
          _clickedAreaType = AreaType.PropertyCell;
        else
          _clickedAreaType = AreaType.OutsideAll;

        int h=_clickedColumn; _clickedColumn = _clickedRow; _clickedRow = h; // Swap columns and rows since it is a property column
      }
      else // it is not a property related cell
      {
        if(_clickedRow==-1 && _clickedColumn==-1)
          _clickedAreaType = AreaType.TableHeader;
        else if(_clickedRow==-1 && _clickedColumn>=0)
          _clickedAreaType = AreaType.DataColumnHeader;
        else if(_clickedRow>=0 && _clickedColumn==-1)
          _clickedAreaType = AreaType.DataRowHeader;
        else if(_clickedRow>=0 && _clickedColumn>=0)
          _clickedAreaType = AreaType.DataCell;
        else
          _clickedAreaType = AreaType.OutsideAll;
      }
    }
  } // end of class ClickedCellInfo


}
