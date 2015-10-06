#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Graph3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Altaxo.Gui.Graph3D.Viewing
{
	using Altaxo.Graph;
	using Altaxo.Gui.Graph3D.Viewing.GraphControllerMouseHandlers;

	[UserControllerForObject(typeof(Altaxo.Graph3D.GraphDocument3D))]
	[ExpectedTypeOfView(typeof(IGraph3DView))]
	public class Graph3DControllerWpf : Graph3DController
	{
		/// <summary>A instance of a mouse handler class that currently handles the mouse events..</summary>
		protected MouseStateHandler _mouseState;

		public Graph3DControllerWpf()
		{
			_mouseState = new GraphControllerMouseHandlers.ObjectPointerMouseHandler(this);
		}

		public Graph3DControllerWpf(GraphDocument3D graphdoc)
		{
			_doc = graphdoc;
			_mouseState = new GraphControllerMouseHandlers.ObjectPointerMouseHandler(this);
		}

		internal Graph3DControl ViewWpf
		{
			get
			{
				return _view as Graph3DControl;
			}
		}

		internal void SetPanelCursor(Cursor arrow)
		{
			var view = ViewWpf;

			if (null != view)
				view.SetPanelCursor(arrow);
		}

		/// <summary>
		/// Handles the mouse up event onto the graph in the controller class.
		/// </summary>
		/// <param name="position">Mouse position. X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
		/// <param name="e">MouseEventArgs.</param>
		public virtual void EhView_GraphPanelMouseUp(PointD3D position, MouseButtonEventArgs e)
		{
			_mouseState.OnMouseUp(position, e);
		}

		/// <summary>
		/// Handles the mouse down event onto the graph in the controller class.
		/// </summary>
		/// <param name="position">Mouse position. X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
		/// <param name="e">MouseEventArgs.</param>
		public virtual void EhView_GraphPanelMouseDown(PointD3D position, MouseButtonEventArgs e)
		{
			_mouseState.OnMouseDown(position, e);
		}

		/// <summary>
		/// Handles the mouse move event onto the graph in the controller class.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">MouseEventArgs.</param>
		public virtual void EhView_GraphPanelMouseMove(PointD3D position, MouseEventArgs e)
		{
			_mouseState.OnMouseMove(position, e);
		}

		internal void RenderOverlay()
		{
		}

		/// <summary>
		/// Handles the click onto the graph event in the controller class.
		/// </summary>
		/// <param name="position">Mouse position. X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
		/// <param name="e">EventArgs.</param>
		public virtual void EhView_GraphPanelMouseClick(PointD3D position, MouseButtonEventArgs e)
		{
			_mouseState.OnClick(position, e);
		}

		/// <summary>
		/// Handles the double click onto the graph event in the controller class.
		/// </summary>
		/// <param name="position">Mouse position.  X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
		/// <param name="e"></param>
		public virtual void EhView_GraphPanelMouseDoubleClick(PointD3D position, MouseButtonEventArgs e)
		{
			_mouseState.OnDoubleClick(position, e);
		}
	}
}