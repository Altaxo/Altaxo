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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

using Altaxo.Graph.Gdi;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Viewing.GraphControllerMouseHandlers
{
	/// <summary>
	/// The abstract base class of all MouseStateHandlers.
	/// </summary>
	/// <remarks>The mouse state handler are used to handle the mouse events of the graph view in different contexts,
	/// depending on which GraphTool is choosen by the user.</remarks>
	public abstract class MouseStateHandler
	{
		/// <summary>Stores the mouse position of the last mouse up event.</summary>
		protected Altaxo.Graph.PointD2D _positionLastMouseUpInMouseCoordinates;
		/// <summary>Stores the mouse position of the last mouse down event.</summary>
		protected Altaxo.Graph.PointD2D _positionLastMouseDownInMouseCoordinates;

		public abstract Altaxo.Gui.Graph.Viewing.GraphToolType GraphToolType { get; }

		/// <summary>
		/// Handles the mouse move event.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">MouseEventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public virtual void OnMouseMove(Altaxo.Graph.PointD2D position, MouseEventArgs e)
		{

		}

		/// <summary>
		/// Handles the mouse up event. Stores the position of the mouse into <see cref="_positionLastMouseUpInMouseCoordinates"/>.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">MouseEventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public virtual void OnMouseUp(Altaxo.Graph.PointD2D position, MouseButtonEventArgs e)
		{
			_positionLastMouseUpInMouseCoordinates = position;
		}

		/// <summary>
		/// Handles the mouse down event. Stores the position of the mouse into <see cref="_positionLastMouseDownInMouseCoordinates"/>.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">MouseEventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public virtual void OnMouseDown(Altaxo.Graph.PointD2D position, MouseButtonEventArgs e)
		{
			_positionLastMouseDownInMouseCoordinates = position;

		}

		/// <summary>
		/// Handles the mouse click event.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">EventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public virtual void OnClick(Altaxo.Graph.PointD2D position, MouseButtonEventArgs e)
		{

		}

		/// <summary>
		/// Handles the mouse doubleclick event.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">EventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public virtual void OnDoubleClick(Altaxo.Graph.PointD2D position, MouseButtonEventArgs e)
		{

		}


		/// <summary>
		/// Is called when the mouse state handler is deselected.
		/// </summary>
		public virtual void OnDeselection()
		{
		}

		/// <summary>
		/// This function is called just after the paint event. The graphic context is in graph coordinates.
		/// </summary>
		/// <param name="g"></param>
		public virtual void AfterPaint(Graphics g)
		{
		}


		/// <summary>
		/// This function is called if a key is pressed.
		/// </summary>
		/// <param name="e">Key event arguments.</param>
		/// <returns></returns>
		public virtual bool ProcessCmdKey(KeyEventArgs e)
		{
			return false; // per default the key is not processed
		}

	}


}
