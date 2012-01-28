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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
namespace Altaxo.Gui.Graph.Viewing
{
	public interface IGraphView 
	{
		/// <summary>
		/// Sets the controller of the view;
		/// </summary>
		IGraphViewEventSink Controller { set; }

		/// <summary>Causes a full redrawing of the plot. If the image is cached, the cache will be invalidated.</summary>
		void InvalidateCachedGraphBitmapAndRepaint();

		/// <summary>
		/// This sets the title of the graph view.
		/// </summary>
		string GraphViewTitle { set; }

		/// <summary>
		/// If true, scrollbars will be shown on the graph.
		/// </summary>
		bool ShowGraphScrollBars { set; }

		/// <summary>
		/// Get /sets the scroll position of the graph
		/// </summary>
		PointF GraphScrollPosition { get; set; }

		void SetHorizontalScrollbarParameter(bool isEnabled, double value, double maximum, double smallIncrement, double largeIncrement);
		void SetVerticalScrollbarParameter(bool isEnabled, double value, double maximum, double smallIncrement, double largeIncrement);
		

		/// <summary>
		/// Sets the number of layers that are in the graph. The view has to reflect the change in the number of layers
		/// by adjusting the number of layer buttons or similar. The current layer number should be preserved.
		/// </summary>
		int NumberOfLayers { set; }

		/// <summary>
		/// Sets the currently active layer. If the view has some means to show the
		/// currently active layer (like a toolbar or so), it has to indicate the current
		/// active layer by setting the state of this indicator.
		/// </summary>
		/// <remarks>The view must not send back a event, if the current layer is changed by this property.
		/// It should only send the CurrentLayerChanged event to the controller, if the _user_ changed the current layer.</remarks>
		int CurrentLayer { set; }


		/// <summary>
		/// Returns the size (in inch) of the area, wherein the graph is painted.
		/// </summary>
		SizeF ViewportSizeInInch { get; }

		IList<IHitTestObject> SelectedObjects { get; }

		GraphToolType GraphTool { get; set; }

		/// <summary>
		/// Returns the control that should be focused initially.
		/// </summary>
		object GuiInitiallyFocusedElement { get; }
	}

	public interface IGraphViewEventSink
	{
		GraphDocument Doc { get; }

		XYPlotLayer ActiveLayer { get; }


			/// <summary>
		/// Handles the selection of the current layer by the <b>user</b>.
		/// </summary>
		/// <param name="currLayer">The current layer number as selected by the user.</param>
		/// <param name="bAlternative">Normally false, can be set to true if the user clicked for instance with the right mouse button on the layer button.</param>
		void EhView_CurrentLayerChoosen(int currLayer, bool bAlternative);

		
		/// <summary>Handles the event when the size of the graph area is changed.</summary>
		void EhView_GraphPanelSizeChanged();

		void EhView_Scroll();

		void EhView_CurrentGraphToolChanged();

		void EhView_ShowDataContextMenu(int layerNumber, object guiParent, Point pt);

	}

	public interface IGraphController : IMVCController
	{
		/// <summary>
		/// This returns the GraphDocument that is managed by this controller.
		/// </summary>
		GraphDocument Doc { get; }

		/// <summary>
		/// Returns the currently active layer, or null if there is no active layer.
		/// </summary>
		XYPlotLayer ActiveLayer { get; }

		/// <summary>
		/// Get / sets the currently active plot by number.
		/// </summary>
		int CurrentPlotNumber { get; set; }

		/// <summary>
		/// check the validity of the CurrentLayerNumber and correct it
		/// </summary>
		void EnsureValidityOfCurrentLayerNumber();

		/// <summary>
		/// This ensures that the current plot number is valid. If there is no plot on the currently active layer,
		/// the current plot number is set to -1.
		/// </summary>
		void EnsureValidityOfCurrentPlotNumber();

		/// <summary>
		/// Does a complete new drawing of the graph, even if the graph is cached in a bitmap.
		/// </summary>
		void RefreshGraph();
	}
}
