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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Graph.Gdi;
using Altaxo.Serialization;
using Altaxo.Gui;

namespace Altaxo.Graph.GUI
{
  /*
  /// <summary>
  /// This enumeration declares the current choosen tool for the GraphControl
  /// The numeric values have to match the icon positions in the corresponding toolbar
  /// </summary>
  public enum GraphTools 
  {
    /// <summary>The tool to click to the objects, dragging them, and open the object dialogs by doubleclicking on them</summary>
    ObjectPointer=0,
    /// <summary>The tool to write text, i.e. to create ExtendedTextGraphObjects</summary>
    Text=1,

    /// <summary>
    /// Tool to snap in to a plot items points and read out the data
    /// </summary>
    ReadPlotItemData,

    /// <summary>
    /// Tool to draw a single line.
    /// </summary>
    SingleLine,

    /// <summary>
    /// Tool to zoom the axes, i.e. to change the values of the axes boundaries.
    /// </summary>
    ZoomAxes

  }
*/






  // **************************************************************************
  // **************************************************************************
  // **************************************************************************
  //
  //                              IGraphView
  //
  // **************************************************************************
  // **************************************************************************
  // **************************************************************************


  /// <summary>
  /// Interface to be implemented by a form or a control to be able to show a graph. This can either be a control or a form.
  /// </summary>
  public interface IGraphView /* : IWorkbenchContentView */
  {
    /// <summary>Returns the windows of this view. In case the view is a Form, it returns the form. But if the view is only a control
    /// on a form, it returns the control window.
    /// </summary>
    System.Windows.Forms.Control Window { get; }
    /// <summary>
    /// Returns the form of this view. In case the view is a Form, it returns that form itself. In case the view is a control on a form,
    /// it returns not the control but the hosting form of this control.
    /// </summary>
    System.Windows.Forms.Form    Form   { get; }

    /// <summary>
    /// Returns the controller that controls this view. Sets the controller to this value.
    /// </summary>
    IGraphController Controller { get; set;}

    /// <summary>
    /// This sets the menu. The menu itself is created and controlled by the controller.</summary>
    /// <remarks>Any changes should only
    /// be done into the controller class. This function is only to set the menu. The menu should <b>not</b> be cloned
    /// by the view class, since then changed into the controller class are not reflected in the menu.
    /// </remarks>
    MainMenu  GraphMenu { set; }


    /// <summary>
    /// This sets the title of the graph view.
    /// </summary>
    string GraphViewTitle { set; }


    /// <summary>
    /// Get / sets the AutoScroll size property 
    /// </summary>
    Size GraphScrollSize { get; set; }


    /// <summary>
    /// Get /sets the scroll position of the graph
    /// </summary>
    Point GraphScrollPosition { get; set; }


    /// <summary>
    /// This creates a graphics context for the graph.
    /// </summary>
    /// <returns>The graphics context. Should be compatible to the area on the screen where the graph is shown.</returns>
    Graphics CreateGraphGraphics();


    /// <summary>
    /// This forces redrawing of the entire graph window.
    /// </summary>
    void InvalidateGraph();

    /// <summary>
    /// Returns the size (in pixel) of the area, wherein the graph is painted.
    /// </summary>
    Size GraphSize { get; }

    /// <summary>
    /// Sets the currently active layer. If the view has some means to show the
    /// currently active layer (like a toolbar or so), it has to indicate the current
    /// active layer by setting the state of this indicator.
    /// </summary>
    /// <remarks>The view must not send back a event, if the current layer is changed by this property.
    /// It should only send the CurrentLayerChanged event to the controller, if the _user_ changed the current layer.</remarks>
    int       CurrentLayer { set; }


    /// <summary>
    /// Sets the number of layers that are in the graph. The view has to reflect the change in the number of layers
    /// by adjusting the number of layer buttons or similar. The current layer number should be preserved.
    /// </summary>
    int       NumberOfLayers { set; }


    /// <summary>
    /// Is called when the view is selected.
    /// </summary>
    void OnViewSelection();

    /// <summary>
    /// Is called when the view is deselected
    /// </summary>
    void OnViewDeselection();


    /// <summary>
    /// The view should set the focus to itself or to a control which can receive the focus.
    /// </summary>
    void TakeFocus();


    /// <summary>
    /// Sets the Cursor of the graph panel.
    /// </summary>
    /// <param name="cursor"></param>
    void SetPanelCursor(Cursor cursor);
  }



  

  // **************************************************************************
  // **************************************************************************
  // **************************************************************************
  //
  //                              IGraphController
  //
  // **************************************************************************
  // **************************************************************************
  // **************************************************************************



  /// <summary>
  /// This interface has to be implemented by any controller that wants to control a GraphView
  /// </summary>
  public interface IGraphController : /* IWorkbenchContentController, */ IMVCControllerEx
  {

    /// <summary>
    /// This returns the GraphDocument that is managed by this controller.
    /// </summary>
    GraphDocument Doc { get; }

    /// <summary>
    /// Returns the view that this controller controls.
    /// </summary>
    /// <remarks>Setting the view is only neccessary on deserialization, so the controller
    /// can restrict setting the view only the own view is still null.</remarks>
    IGraphView View { get; set; }


    /// <summary>
    /// This event will be fired if the current graph tool has changed, either by the user
    /// or by the program.
    /// </summary>
    event EventHandler CurrentGraphToolChanged;


    /// <summary>
    /// This function is called if the user changed the active layer (by a toolbar or similar).
    /// </summary>
    /// <param name="currentLayer">The number of the currently active layer (choosen by the user).</param>
    /// <param name="bAlternative">Normally false, can be set to true if the user clicked for instance with the right mouse button on the layer button.</param>
    void EhView_CurrentLayerChoosen(int currentLayer, bool bAlternative);
  
    /// <summary>
    /// The controller should show a data context menu (contains all plots of the currentLayer).
    /// </summary>
    /// <param name="currentLayer">The layer number. The controller has to make this number the CurrentLayerNumber.</param>
    /// <param name="parent">The parent control which is the parent of the context menu.</param>
    /// <param name="pt">The location where the context menu should be shown.</param>
    void EhView_ShowDataContextMenu(int currentLayer, System.Windows.Forms.Control parent, Point pt);

    /// <summary>
    /// This function is called if the user changed the GraphTool.
    /// </summary>
    /// <param name="graphToolType">The type of the new selected GraphTool.</param>
    void EhView_CurrentGraphToolChoosen(System.Type graphToolType);
  

    /// <summary>
    /// Handles the mouse up event onto the graph in the controller class.
    /// </summary>
    /// <param name="e">MouseEventArgs.</param>
    void EhView_GraphPanelMouseUp(System.Windows.Forms.MouseEventArgs e);

    /// <summary>
    /// Handles the mouse down event onto the graph in the controller class.
    /// </summary>
    /// <param name="e">MouseEventArgs.</param>
    void EhView_GraphPanelMouseDown(System.Windows.Forms.MouseEventArgs e);

    /// <summary>
    /// Handles the mouse move event onto the graph in the controller class.
    /// </summary>
    /// <param name="e">MouseEventArgs.</param>
    void EhView_GraphPanelMouseMove(System.Windows.Forms.MouseEventArgs e);

    /// <summary>
    /// Handles the click onto the graph event in the controller class.
    /// </summary>
    /// <param name="e">EventArgs.</param>
    void EhView_GraphPanelMouseClick(System.EventArgs e);

    /// <summary>
    /// Handles the double click onto the graph event in the controller class.
    /// </summary>
    /// <param name="e"></param>
    void EhView_GraphPanelMouseDoubleClick(System.EventArgs e);
  
  
    /// <summary>
    /// Handles the paint event of that area, where the graph is shown.
    /// </summary>
    /// <param name="e">The paint event args.</param>
    void EhView_GraphPanelPaint(System.Windows.Forms.PaintEventArgs e);

  
    /// <summary>
    /// Handles the event when the size of the graph area is changed.
    /// </summary>
    /// <param name="e">EventArgs.</param>
    void EhView_GraphPanelSizeChanged(System.EventArgs e);

    /// <summary>
    /// Called if a key is pressed in the view.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    bool EhView_ProcessCmdKey(ref Message msg, Keys keyData);

  }





}
