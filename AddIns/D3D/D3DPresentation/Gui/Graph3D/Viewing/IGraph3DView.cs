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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph3D.Viewing
{
	public interface IGraph3DView
	{
		/// <summary>
		/// Returns the control that should be focused initially.
		/// </summary>
		object GuiInitiallyFocusedElement { get; }

		/// <summary>
		/// Sets the controller of the view;
		/// </summary>
		Graph3DController Controller { set; }

		/// <summary>
		/// Sets the number of layers that are in the graph. The view has to reflect the change in the number of layers
		/// by adjusting the number of layer buttons or similar. The current layer number should be preserved.
		/// </summary>
		void SetLayerStructure(Altaxo.Collections.NGTreeNode structure, int[] currentLayerNumber);

		/// <summary>
		/// Sets the currently active layer. If the view has some means to show the
		/// currently active layer (like a toolbar or so), it has to indicate the current
		/// active layer by setting the state of this indicator.
		/// </summary>
		/// <remarks>The view must not send back a event, if the current layer is changed by this property.
		/// It should only send the CurrentLayerChanged event to the controller, if the _user_ changed the current layer.</remarks>
		int[] CurrentLayer { set; }

		/// <summary>
		/// This sets the title of the graph view.
		/// </summary>
		string GraphViewTitle { set; }

		/// <summary>
		/// Returns the size in points (=1/72 inch) of the area, wherein the graph is painted.
		/// </summary>
		Altaxo.Graph.PointD2D ViewportSizeInPoints { get; }

		/// <summary>
		/// Called if a full repaint of the graph is neccessary.
		/// </summary>
		void FullRepaint();
	}
}