using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;

namespace Altaxo.Worksheet
{

	// **************************************************************************
	// **************************************************************************
	// **************************************************************************
	//
	//                              ITableView
	//
	// **************************************************************************
	// **************************************************************************
	// **************************************************************************

	/// <summary>
	/// Interface for all classes that can display a DataTable under control of
	/// a ITableController
	/// </summary>
	public interface ITableView
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
		ITableController Controller { get; set;}

		/// <summary>
		/// This sets the menu. The menu itself is created and controlled by the controller.</summary>
		/// <remarks>Any changes should only
		/// be done into the controller class. This function is only to set the menu. The menu should <b>not</b> be cloned
		/// by the view class, since then changed into the controller class are not reflected in the menu.
		/// </remarks>
		MainMenu	TableViewMenu { set; }


		/// <summary>
		/// Get / sets the maximum value of the horizontal scroll bar 
		/// </summary>
		int HorzScrollMaximum{ set; }
		
		/// <summary>
		/// Get / sets the maximum value of the vertical scroll bar 
		/// </summary>
		int VertScrollMaximum { set; }

		/// <summary>
		/// Get /sets the horizontal scroll value
		/// </summary>
		int HorzScrollValue { set; }

		/// <summary>
		/// Get /sets the vertical scroll value
		/// </summary>
		int VertScrollValue { set; }

		/// <summary>
		/// This creates a graphics context for the area where the table is shown.
		/// </summary>
		/// <returns>The graphics context. Should be compatible to the area on the screen where the table is shown.</returns>
		Graphics CreateTableAreaGraphics();


		/// <summary>
		/// This forces redrawing of the entire Table window.
		/// </summary>
		void InvalidateTableArea();

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

	}


	// **************************************************************************
	// **************************************************************************
	// **************************************************************************
	//
	//                              ITableController
	//
	// **************************************************************************
	// **************************************************************************
	// **************************************************************************


	/// <summary>
	/// Interface for all classes that can control a ITableView to show data from a DataTable. 
	/// </summary>
	public interface ITableController
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
		ITableView View { get; set; }

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

		/// <summary>
		/// Handles the event when the graph view is closed.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		void EhView_Closed(System.EventArgs e);

		/// <summary>
		/// Handles the event when the graph view is about to be closed.
		/// </summary>
		/// <param name="e">CancelEventArgs.</param>
		void EhView_Closing(System.ComponentModel.CancelEventArgs e);
	}
}
