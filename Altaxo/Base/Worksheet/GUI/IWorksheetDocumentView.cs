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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Gui;

namespace Altaxo.Worksheet.GUI
{

  // **************************************************************************
  // **************************************************************************
  // **************************************************************************
  //
  //                              IWorksheetView
  //
  // **************************************************************************
  // **************************************************************************
  // **************************************************************************

  /// <summary>
  /// Interface for all classes that can display a DataTable under control of
  /// a IWorksheetController
  /// </summary>
  /// <remarks>I preceded all function and property names with Table... or so to make sure that
  /// one Form or one control can implement more than one view. For instance a form
  /// that want to show a table and a graph has to implement IWorksheetView <b>and</b>
  /// IGraphView.</remarks>
  public interface IWorksheetView /* : IWorkbenchContentView */
  {
    /// <summary>Returns the windows of this view. In case the view is a Form, it returns the form. But if the view is only a control
    /// on a form, it returns the control window.
    /// </summary>
    System.Windows.Forms.Control TableViewWindow { get; }
    /// <summary>
    /// Returns the form of this view. In case the view is a Form, it returns that form itself. In case the view is a control on a form,
    /// it returns not the control but the hosting form of this control.
    /// </summary>
    System.Windows.Forms.Form    TableViewForm   { get; }

    /// <summary>
    /// Returns the controller that controls this view. Sets the controller to this value.
    /// </summary>
    IWorksheetController WorksheetController { get; set;}

    /// <summary>
    /// This sets the menu. The menu itself is created and controlled by the controller.</summary>
    /// <remarks>Any changes should only
    /// be done into the controller class. This function is only to set the menu. The menu should <b>not</b> be cloned
    /// by the view class, since then changed into the controller class are not reflected in the menu.
    /// </remarks>
    MainMenu  TableViewMenu { set; }

    /// <summary>
    /// This sets the title of this table view.
    /// </summary>
    string TableViewTitle { set; }

    /// <summary>
    /// Get / sets the maximum value of the horizontal scroll bar 
    /// </summary>
    int TableViewHorzScrollMaximum{ set; }
    
    /// <summary>
    /// Get / sets the maximum value of the vertical scroll bar 
    /// </summary>
    int TableViewVertScrollMaximum { set; }

    /// <summary>
    /// Get /sets the horizontal scroll value
    /// </summary>
    int TableViewHorzScrollValue { set; }

    /// <summary>
    /// Get /sets the vertical scroll value
    /// </summary>
    int TableViewVertScrollValue { set; }

    /// <summary>
    /// This creates a graphics context for the area where the table is shown.
    /// </summary>
    /// <returns>The graphics context. Should be compatible to the area on the screen where the table is shown.</returns>
    Graphics TableAreaCreateGraphics();


    /// <summary>
    /// This forces redrawing of the entire Table window.
    /// </summary>
    void TableAreaInvalidate();

    /// <summary>
    /// Returns the size (in pixel) of the area, wherein the table is painted.
    /// </summary>
    Size TableAreaSize { get; }

    /// <summary>
    /// Gets / sets if the mouse is captured in the table view area.
    /// </summary>
    bool TableAreaCapture { get; set; }

    /// <summary>
    /// Get / sets the mouse cursor in the table view area.
    /// </summary>
    System.Windows.Forms.Cursor TableAreaCursor { get; set; }

    /// <summary>
    /// The view should set the focus to itself or to a child control which can receive the focus.
    /// </summary>
    void TakeFocus();

  }


  // **************************************************************************
  // **************************************************************************
  // **************************************************************************
  //
  //                              IWorksheetController
  //
  // **************************************************************************
  // **************************************************************************
  // **************************************************************************


  /// <summary>
  /// Interface for all classes that can control a IWorksheetView to show data from a DataTable. 
  /// </summary>
  public interface IWorksheetController : 
    IMVCControllerEx
    // IWorkbenchContentController,
    //ICSharpCode.SharpDevelop.Gui.IEditable,
    //ICSharpCode.SharpDevelop.Gui.IClipboardHandler
  
  {
    /// <summary>
    /// This returns the Table that is managed by this controller.
    /// </summary>
    Altaxo.Data.DataTable Doc { get; }

    /// <summary>
    /// Returns the view that this controller controls.
    /// </summary>
    /// <remarks>Setting the view is only neccessary on deserialization, so the controller
    /// can restrict setting the view only if the own view variable is still null.</remarks>
    IWorksheetView View { get; set; }

    /// <summary>
    /// Handles the scroll event of the vertical scroll bar.
    /// </summary>
    /// <param name="e">The scroll event args.</param>
    void EhView_VertScrollBarScroll(System.Windows.Forms.ScrollEventArgs e);


    /// <summary>
    /// Handles the scroll event of the horizontal scroll bar.
    /// </summary>
    /// <param name="e">The scroll event args.</param>
    void EhView_HorzScrollBarScroll(System.Windows.Forms.ScrollEventArgs e);

    /// <summary>
    /// Handles the mouse up event onto the graph in the controller class.
    /// </summary>
    /// <param name="e">MouseEventArgs.</param>
    void EhView_TableAreaMouseUp(System.Windows.Forms.MouseEventArgs e);

    /// <summary>
    /// Handles the mouse down event onto the graph in the controller class.
    /// </summary>
    /// <param name="e">MouseEventArgs.</param>
    void EhView_TableAreaMouseDown(System.Windows.Forms.MouseEventArgs e);

    /// <summary>
    /// Handles the mouse wheel event onto the graph in the controller class.
    /// </summary>
    /// <param name="e">MouseEventArgs.</param>
    void EhView_TableAreaMouseWheel(System.Windows.Forms.MouseEventArgs e);

    /// <summary>
    /// Handles the mouse move event onto the graph in the controller class.
    /// </summary>
    /// <param name="e">MouseEventArgs.</param>
    void EhView_TableAreaMouseMove(System.Windows.Forms.MouseEventArgs e);

    /// <summary>
    /// Handles the click onto the graph event in the controller class.
    /// </summary>
    /// <param name="e">EventArgs.</param>
    void EhView_TableAreaMouseClick(System.EventArgs e);

    /// <summary>
    /// Handles the double click onto the graph event in the controller class.
    /// </summary>
    /// <param name="e"></param>
    void EhView_TableAreaMouseDoubleClick(System.EventArgs e);
  
  
    /// <summary>
    /// Handles the paint event of that area, where the graph is shown.
    /// </summary>
    /// <param name="e">The paint event args.</param>
    void EhView_TableAreaPaint(System.Windows.Forms.PaintEventArgs e);

  
    /// <summary>
    /// Handles the event when the size of the graph area is changed.
    /// </summary>
    /// <param name="e">EventArgs.</param>
    void EhView_TableAreaSizeChanged(System.EventArgs e);

    
  }
}
