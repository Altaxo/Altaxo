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

using Altaxo.Geometry;
using Altaxo.Graph.Graph3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph.Graph3D.Viewing
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
    /// Returns the size in points (=1/72 inch) of the area, wherein the graph is painted.
    /// </summary>
    PointD2D ViewportSizeInPoints { get; }

    /// <summary>
    /// Sets the color that is used to initialize the background of the render scene.
    /// </summary>
    /// <param name="sceneBackColor">Background color of the render scene.</param>
    void SetSceneBackColor(Altaxo.Drawing.AxoColor sceneBackColor);

    /// <summary>
    /// Sets the camera, but does not trigger a new rendering.
    /// </summary>
    /// <param name="camera">The camera.</param>
    /// <param name="lightSettings">The light settings.</param>
    void SetCamera(Altaxo.Graph.Graph3D.Camera.CameraBase camera, Altaxo.Graph.Graph3D.LightSettings lightSettings);

    /// <summary>
    /// Sets a new geometry, but does not trigger rendering (use <see cref="TriggerRendering"/> for triggering rendering.
    /// </summary>
    /// <param name="drawing">The drawing.</param>
    void SetDrawing(Altaxo.Graph.Graph3D.GraphicsContext.IGraphicsContext3D drawing);

    /// <summary>
    /// Triggers a new rendering without building up a new geometry. Could be used for instance if the light or the camera has changed, but not the geometry.
    /// </summary>
    void TriggerRendering();

    /// <summary>
    /// Gets the graphic context that is appropriate for the view.
    /// </summary>
    /// <returns>New graphic context.</returns>
    Altaxo.Graph.Graph3D.GraphicsContext.IGraphicsContext3D GetGraphicContext();

    /// <summary>
    /// Gets the graphic context for root layer markers, i.e. for stuff that doesn't belong to the graph document.
    /// </summary>
    /// <returns>Graphic context for root layer markers</returns>
    Altaxo.Graph.Graph3D.GraphicsContext.IOverlayContext3D GetGraphicContextForMarkers();

    /// <summary>
    /// Sets the marker geometry, brings it into the buffers. It doesn't trigger a new rendering, please use <see cref="TriggerRendering"/> for that.
    /// </summary>
    /// <param name="markerGeometry">The marker geometry.</param>
    void SetMarkerGeometry(Altaxo.Graph.Graph3D.GraphicsContext.IOverlayContext3D markerGeometry);

    /// <summary>
    /// Gets the graphic context for overlay geometry, i.e. for geometry that shows if an object is selected, grips and so on.
    /// </summary>
    /// <returns>Graphic context for overlay geometry</returns>
    Altaxo.Graph.Graph3D.GraphicsContext.IOverlayContext3D GetGraphicContextForOverlay();

    /// <summary>
    /// Sets the overlay geometry, bring it into the buffers. It doesn't trigger a new rendering, please use <see cref="TriggerRendering"/> for that.
    /// </summary>
    /// <param name="overlayGeometry">The overlay geometry.</param>
    void SetOverlayGeometry(Altaxo.Graph.Graph3D.GraphicsContext.IOverlayContext3D overlayGeometry);

    void FocusOnGraphPanel();

    /// <summary>
    /// Sets the panel cursor of the view.
    /// </summary>
    /// <param name="cursor">The cursor (must be of the appropriate Gui type).</param>
    void SetPanelCursor(object cursor);

    GraphToolType CurrentGraphTool { get; set; }

    void RenderOverlay();

    IList<IHitTestObject> SelectedObjects { get; }

    void AnnounceContentVisibilityChanged(bool isVisible);
  }
}
