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

namespace Altaxo.Worksheet.GUI
{
  /// <remarks>
  /// ClickedCellInfo retrieves (from mouse coordinates of a click), which cell has clicked onto. 
  /// </remarks>
  public class ClickedCellInfo : System.EventArgs
  {
    private System.Windows.Forms.MouseButtons m_LastMouseButtons;
    /// <summary>
    /// Gets which mouse buttons were pressed on the last mouseup/mousedown event
    /// </summary>
    public System.Windows.Forms.MouseButtons LastMouseButtons
    {
      get
      {
        return m_LastMouseButtons;
      }
    }

    private Point m_MousePositionFirstDown;

    /// <summary>
    /// Gets the mouse position of the first mousedown event (after all buttons were up).
    /// </summary>
    public Point MousePositionFirstDown
    {
      get
      {
        return m_MousePositionFirstDown;
      }
    }

    private MouseButtons m_MouseButtonFirstDown;
    /// <summary>
    /// Gets the mouse button that was first pressed down
    /// </summary>
    public MouseButtons MouseButtonFirstDown
    {
      get { return m_MouseButtonFirstDown; }
    }

    private Point m_MousePositionLastUp;
    /// <summary>
    /// Gets the mouse position of the last mouseup event (so that all buttons are up afterwards).
    /// </summary>
    public Point MousePositionLastUp
    {
      get
      {
        return m_MousePositionLastUp;
      }
    }

    private MouseButtons m_MouseButtonLastUp;
    /// <summary>
    /// Gets the mouse button that was release last (before all mouse buttons were up).
    /// </summary>
    public MouseButtons MouseButtonLastUp
    {
      get { return m_MouseButtonLastUp; }
    }

    public void MouseDown(MouseEventArgs e)
    {
      if(m_LastMouseButtons==MouseButtons.None)
      {
        this.m_MousePositionFirstDown = new Point(e.X,e.Y);
        this.m_MouseButtonFirstDown = e.Button;
      }
      m_LastMouseButtons = e.Button;
    }

    public void MouseUp(MouseEventArgs e, MouseButtons buttonsAfterwards)
    {
      if(buttonsAfterwards==MouseButtons.None)
      {
        this.m_MousePositionLastUp = new Point(e.X,e.Y);
        this.m_MouseButtonLastUp = e.Button;
      }
      m_LastMouseButtons = buttonsAfterwards;
    }


    /// <summary>The enclosing Rectangle of the clicked cell</summary>
    private Rectangle m_CellRectangle;

    /// <summary>The data row clicked onto.</summary>
    private int m_Row;
    /// <summary>The data column number clicked onto.</summary>
    private int m_Column;

    /// <summary>What have been clicked onto.</summary>
    private ClickedAreaType m_ClickedArea;


    /// <value>The enclosing Rectangle of the clicked cell</value>
    public Rectangle CellRectangle { get { return m_CellRectangle; }}
    /// <value>The row number clicked onto.</value>
    public int Row 
    {
      get { return m_Row; }
      set { m_Row = value; }
    }
    /// <value>The column number clicked onto.</value>
    public int Column 
    {
      get { return m_Column; }
      set { m_Column = value; }
    }
    /// <value>The type of area clicked onto.</value>
    public ClickedAreaType ClickedArea { get { return m_ClickedArea; }}
 
    /// <summary>
    /// Retrieves the column number clicked onto 
    /// </summary>
    /// <param name="dg">The parent data grid</param>
    /// <param name="mouseCoord">The coordinates of the mouse click.</param>
    /// <param name="cellRect">The function sets the x-properties (X and Width) of the cell rectangle.</param>
    /// <returns>Either -1 when clicked on the row header area, column number when clicked in the column range, or int.MinValue when clicked outside of all.</returns>
    public static int GetColumnNumber(WorksheetController dg, Point mouseCoord, ref Rectangle cellRect)
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
    public static int GetRowNumber(WorksheetController dg, Point mouseCoord, ref Rectangle cellRect, out bool bPropertyCol)
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
    public void MouseClick(WorksheetController dg, Point mouseCoord)
    {

      bool bIsPropertyColumn=false;
      m_CellRectangle = new Rectangle(0,0,0,0);
      m_Column = GetColumnNumber(dg,mouseCoord, ref m_CellRectangle);
      m_Row    = GetRowNumber(dg,mouseCoord,ref m_CellRectangle, out bIsPropertyColumn);

      if(bIsPropertyColumn)
      {
        if(m_Column==-1)
          m_ClickedArea = ClickedAreaType.PropertyColumnHeader;
        else if(m_Column>=0)
          m_ClickedArea = ClickedAreaType.PropertyCell;
        else
          m_ClickedArea = ClickedAreaType.OutsideAll;

        int h=m_Column; m_Column = m_Row; m_Row = h; // Swap columns and rows since it is a property column
      }
      else // it is not a property related cell
      {
        if(m_Row==-1 && m_Column==-1)
          m_ClickedArea = ClickedAreaType.TableHeader;
        else if(m_Row==-1 && m_Column>=0)
          m_ClickedArea = ClickedAreaType.DataColumnHeader;
        else if(m_Row>=0 && m_Column==-1)
          m_ClickedArea = ClickedAreaType.DataRowHeader;
        else if(m_Row>=0 && m_Column>=0)
          m_ClickedArea = ClickedAreaType.DataCell;
        else
          m_ClickedArea = ClickedAreaType.OutsideAll;
      }
    }
  } // end of class ClickedCellInfo


}
