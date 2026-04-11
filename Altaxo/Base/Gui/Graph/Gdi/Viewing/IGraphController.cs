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

#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Graph.Gdi.Viewing
{
  /// <summary>
  /// Provides the view contract for 2D graph views.
  /// </summary>
  public interface IGraphView
  {
    /// <summary>
    /// Sets the controller of the view.
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
    /// Gets the scroll position of the graph.
    /// </summary>
    PointD2D GraphScrollPosition { get; }

    /// <summary>Sets the horizontal scrollbar parameter.</summary>
    /// <param name="isEnabled">If set to <c>true</c>, the scrollbar is enabled.</param>
    /// <param name="value">The scroll value.</param>
    /// <param name="maximum">The maximum value of the scroll bar.</param>
    /// <param name="portSize">Size of the scrollbars view port (length of the thumb).</param>
    /// <param name="largeIncrement">The large increment value.</param>
    /// <param name="smallIncrement">The small increment value.</param>
    void SetHorizontalScrollbarParameter(bool isEnabled, double value, double maximum, double portSize, double largeIncrement, double smallIncrement);

    /// <summary>Sets the vertical scrollbar parameter.</summary>
    /// <param name="isEnabled">If set to <c>true</c>, the scrollbar is enabled.</param>
    /// <param name="value">The scroll value.</param>
    /// <param name="maximum">The maximum value of the scroll bar.</param>
    /// <param name="portSize">Size of the scrollbars view port (length of the thumb).</param>
    /// <param name="largeIncrement">The large increment value.</param>
    /// <param name="smallIncrement">The small increment value.</param>
    void SetVerticalScrollbarParameter(bool isEnabled, double value, double maximum, double portSize, double largeIncrement, double smallIncrement);

    /// <summary>
    /// Sets the number of layers that are in the graph. The view has to reflect the change in the number of layers
    /// by adjusting the number of layer buttons or similar. The current layer number should be preserved.
    /// </summary>
    /// <param name="structure">The layer tree to display.</param>
    /// <param name="currentLayerNumber">The currently active layer index path.</param>
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
    /// Returns the size in points (=1/72 inch) of the area, wherein the graph is painted.
    /// </summary>
    PointD2D ViewportSizeInPoints { get; }

    /// <summary>
    /// Returns the control that should be focused initially.
    /// </summary>
    object GuiInitiallyFocusedElement { get; }

    /// <summary>
    /// Captures the mouse on the graph canvas.
    /// </summary>
    void CaptureMouseOnCanvas();

    /// <summary>
    /// Releases mouse capture on the graph canvas.
    /// </summary>
    void ReleaseCaptureMouseOnCanvas();

    /// <summary>
    /// Sets the focus to the graph panel.
    /// </summary>
    void FocusOnGraphPanel();

    /// <summary>
    /// Sets the panel cursor of the view.
    /// </summary>
    /// <param name="cursor">The cursor (must be of the appropriate Gui type).</param>
    void SetPanelCursor(object cursor);

    /// <summary>
    /// Requests rendering of the current overlay.
    /// </summary>
    void EhRenderOverlayTriggered();

    /// <summary>
    /// Gets or sets the current graph tool type.
    /// </summary>
    GraphToolType CurrentGraphTool { get; set; }

    /// <summary>
    /// Gets the selected objects (if mouse handler is the ObjectPointerMouseHandler). Otherwise, null or an empty list is returned.
    /// </summary>
    /// <value>
    /// The selected objects.
    /// </value>
    IList<IHitTestObject> SelectedObjects { get; }

    /// <summary>
    /// Gets a value indicating whether overlay painting is required for the current mouse state.
    /// </summary>
    bool MouseState_IsOverlayPaintingRequired { get; }

    /// <summary>
    /// Performs mouse-state specific painting after the graph has been rendered.
    /// </summary>
    /// <param name="g">The graphics surface used for overlay painting.</param>
    void MouseState_AfterPaint(System.Drawing.Graphics g);

    /// <summary>
    /// Announces that the visibility of the graph has changed.
    /// </summary>
    /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
    void AnnounceContentVisibilityChanged(bool isVisible);
  }

  /// <summary>
  /// Provides the event sink contract used by <see cref="IGraphView"/>.
  /// </summary>
  public interface IGraphViewEventSink
  {
    /// <summary>
    /// Gets the graph document associated with the view.
    /// </summary>
    GraphDocument Doc { get; }

    /// <summary>
    /// Gets the currently active host layer.
    /// </summary>
    HostLayer ActiveLayer { get; }

    /// <summary>
    /// Handles the selection of the current layer by the <b>user</b>.
    /// </summary>
    /// <param name="currLayer">The current layer number as selected by the user.</param>
    /// <param name="bAlternative">Normally false, can be set to true if the user clicked for instance with the right mouse button on the layer button.</param>
    void EhView_CurrentLayerChoosen(int[] currLayer, bool bAlternative);

    /// <summary>Handles the event when the size of the graph area is changed.</summary>
    void EhView_GraphPanelSizeChanged();

    /// <summary>
    /// Handles scrolling of the graph view.
    /// </summary>
    void EhView_Scroll();

    /// <summary>
    /// Handles a change of the current graph tool.
    /// </summary>
    void EhView_CurrentGraphToolChanged();

    /// <summary>
    /// Shows the data context menu for the specified layer.
    /// </summary>
    /// <param name="layerNumber">The layer index path for which the context menu should be shown.</param>
    /// <param name="guiParent">The parent GUI element for the context menu.</param>
    /// <param name="pt">The screen position where the context menu should appear.</param>
    void EhView_ShowDataContextMenu(int[] layerNumber, object guiParent, Point pt);
  }

  /// <summary>
  /// Provides the controller contract for 2D graph views.
  /// </summary>
  public interface IGraphController : IMVCANController
  {
    /// <summary>
    /// This returns the GraphDocument that is managed by this controller.
    /// </summary>
    GraphDocument Doc { get; }

    /// <summary>
    /// Returns the currently active layer, or null if there is no active layer.
    /// </summary>
    HostLayer ActiveLayer { get; }

    /// <summary>
    /// Gets or sets the currently active plot number.
    /// </summary>
    int CurrentPlotNumber { get; set; }

    /// <summary>
    /// Gets the current graph tool.
    /// </summary>
    ITool? GraphTool { get; }

    /// <summary>
    /// Checks the validity of the current layer number and corrects it if necessary.
    /// </summary>
    /// <returns>The currently active layer.</returns>
    HostLayer EnsureValidityOfCurrentLayerNumber();

    /// <summary>
    /// This ensures that the current plot number is valid. If there is no plot on the currently active layer,
    /// the current plot number is set to -1.
    /// </summary>
    void EnsureValidityOfCurrentPlotNumber();

    /// <summary>
    /// Does a complete new drawing of the graph, even if the graph is cached in a bitmap.
    /// </summary>
    void RefreshGraph();

    /// <summary>
    /// Gets the objects currently selected. The returned objects are not the <see cref="HitTestObject"/>s, but the hitted objects itself.
    /// </summary>
    /// <value>
    /// The objects currently selected in the graph.
    /// </value>
    IEnumerable<object> SelectedRealObjects { get; }

    /// <summary>
    /// Gets the objects currently selected. The returned objects are not the <see cref="HitTestObject"/>s, but the hitted objects itself.
    /// </summary>
    /// <value>
    /// The objects currently selected in the graph.
    /// </value>
    IList<IHitTestObject> SelectedObjects { get; }
  }
}
