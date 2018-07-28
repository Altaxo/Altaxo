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

using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Input;

namespace Altaxo.Gui.Graph.Gdi.Viewing.GraphControllerMouseHandlers
{
  /// <summary>
  /// The abstract base class of all MouseStateHandlers.
  /// </summary>
  /// <remarks>The mouse state handler are used to handle the mouse events of the graph view in different contexts,
  /// depending on which GraphTool is choosen by the user.</remarks>
  public abstract class MouseStateHandler
  {
    protected struct POINT
    {
      public PointD2D RootLayerCoordinates;
      public PointD2D LayerCoordinates;
    }

    /// <summary>Stores the mouse position of the last mouse up event.</summary>
    protected PointD2D _positionLastMouseUpInMouseCoordinates;

    /// <summary>Stores the mouse position of the last mouse down event.</summary>
    protected PointD2D _positionLastMouseDownInMouseCoordinates;

    /// <summary>Active layer at the time of using the tool.</summary>
    protected HostLayer _cachedActiveLayer;

    /// <summary>Transformation that can be used to transform root layer coordinates into the coordinates of the cached active layer.</summary>
    protected MatrixD2D _cachedActiveLayerTransformation;

    /// <summary>Transformation that can be used to transform root layer coordinates into the coordinates of the cached active layer (here the Gdi version for efficiency).</summary>
    protected Matrix _cachedActiveLayerTransformationGdi;

    /// <summary>The current mouse position in root layer coordinates</summary>
    protected PointD2D _positionCurrentMouseInRootLayerCoordinates;

    public abstract GraphToolType GraphToolType { get; }

    /// <summary>
    /// Handles the mouse move event.
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public virtual void OnMouseMove(PointD2D position, MouseEventArgs e)
    {
    }

    /// <summary>
    /// Handles the mouse up event. Stores the position of the mouse into <see cref="_positionLastMouseUpInMouseCoordinates"/>.
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public virtual void OnMouseUp(PointD2D position, MouseButtonEventArgs e)
    {
      _positionLastMouseUpInMouseCoordinates = position;
    }

    /// <summary>
    /// Handles the mouse down event. Stores the position of the mouse into <see cref="_positionLastMouseDownInMouseCoordinates"/>.
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public virtual void OnMouseDown(PointD2D position, MouseButtonEventArgs e)
    {
      _positionLastMouseDownInMouseCoordinates = position;
    }

    /// <summary>
    /// Handles the mouse click event.
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">EventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public virtual void OnClick(PointD2D position, MouseButtonEventArgs e)
    {
    }

    /// <summary>
    /// Handles the mouse doubleclick event.
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">EventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public virtual void OnDoubleClick(PointD2D position, MouseButtonEventArgs e)
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
    /// Returns true when painting the overlay is currently required; and false if it is not required.
    /// </summary>
    /// <returns>True when painting the overlay is currently required; and false if it is not required.</returns>
    public virtual bool IsOverlayPaintingRequired { get { return true; } }

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
