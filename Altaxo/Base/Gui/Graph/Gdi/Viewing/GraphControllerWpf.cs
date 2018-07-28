#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Altaxo.Gui.Graph.Gdi.Viewing
{
  using Altaxo.Drawing;
  using Altaxo.Geometry;
  using Altaxo.Gui.Workbench;
  using Altaxo.Main.Services;
  using System.ComponentModel;

  /// <summary>
  /// GraphController is our default implementation to control a graph view.
  /// </summary>

  public partial class GraphController : AbstractViewContent, IGraphController, IGraphViewEventSink, IDisposable
  {
    #region Member variables

    // following default unit is point (1/72 inch)
    /// <summary>For the graph elements all the units are in points. One point is 1/72 inch.</summary>
    protected const double PointsPerInch = 72;

    /// <summary>Inches per point unit.</summary>
    protected const double InchPerPoint = 1 / 72.0;

    private static IList<IHitTestObject> _emptyReadOnlyList;

    /// <summary>
    /// Color for the area of the view, where there is no page.
    /// </summary>
    protected NamedColor _nonPageAreaColor;

    /// <summary>
    /// Brush to fill the page ground. Since the printable area is filled with another brush, in effect
    /// this brush fills only the non printable margins of the page.
    /// </summary>
    protected BrushX _pageGroundBrush;

    /// <summary>
    /// Brush to fill the printable area of the graph.
    /// </summary>
    protected BrushX _graphAreaBrush;

    /// <summary>Screen resolution in dpi (in fact it is the factor that converts physical length on the screen (in inch) to the coordinate system used by Wpf (mouse coordinates, heights, widths, etc.).</summary>
    protected PointD2D _screenResolutionDpi;

    #endregion Member variables

    #region Constructors

    /// <summary>
    /// Set the member variables to default values. Intended only for use in constructors and deserialization code.
    /// </summary>
    protected virtual void SetMemberVariablesToDefault()
    {
      _nonPageAreaColor = NamedColors.Gray;

      _pageGroundBrush = new BrushX(NamedColors.LightGray) { ParentObject = SuspendableDocumentNode.StaticInstance };

      _graphAreaBrush = new BrushX(NamedColors.Snow) { ParentObject = SuspendableDocumentNode.StaticInstance };

      _screenResolutionDpi = Current.Gui.ScreenResolutionDpi;
    }

    #endregion Constructors

    #region Functions used by View

    public void SetGraphToolFromInternal(GraphToolType value)
    {
      _view.CurrentGraphTool = value;
    }

    public void SetPanelCursor(object cursor)
    {
      _view?.SetPanelCursor(cursor);
    }

    /// <summary>
    /// Gets the color of the non page area, i.e. the area that not belongs to the graph.
    /// </summary>
    /// <value>
    /// The color of the non page area.
    /// </value>
    public NamedColor NonPageAreaColor
    {
      get
      {
        return _nonPageAreaColor;
      }
    }

    #endregion Functions used by View

    #region Event handlers forwarded by view

    /// <summary>
    /// Called if the host window is about to be closed.
    /// </summary>
    /// <returns>True if the closing should be canceled, false otherwise.</returns>
    public bool HostWindowClosing()
    {
      if (!Current.GetRequiredService<IShutdownService>().IsApplicationClosing)
      {
        if (false == Current.Gui.YesNoMessageBox("Do you really want to close this graph?", "Attention", false))
        {
          return true; // cancel the closing
        }
      }
      return false;
    }

    private DateTime _nextScrollZoomAcceptTime;

    /// <summary>Handles the mouse wheel event.</summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
    public virtual void EhView_GraphPanelMouseWheel(AltaxoMouseEventArgs e, AltaxoKeyboardModifierKeys keyModifiers, HandledEventArgs eHand)
    {
      var position = e.Position;
      if (keyModifiers.HasFlag(AltaxoKeyboardModifierKeys.Control))
      {
        eHand.Handled = true;

        DateTime now = DateTime.UtcNow;
        if (now < _nextScrollZoomAcceptTime)
          return;

        var oldZoom = ZoomFactor;
        var newZoom = oldZoom;
        var autoZoomFactor = AutoZoomFactor;
        bool isAutoZoomNext = false;
        if (e.Delta > 0)
        {
          newZoom = oldZoom * 1.5;
          isAutoZoomNext = newZoom >= autoZoomFactor && oldZoom < autoZoomFactor;
        }
        else if (e.Delta < 0)
        {
          newZoom = oldZoom / 1.5;
          isAutoZoomNext = newZoom <= autoZoomFactor && oldZoom > autoZoomFactor;
        }
        // Do zoom action here
        if (isAutoZoomNext)
        {
          IsAutoZoomActive = true;
          _nextScrollZoomAcceptTime = now.AddMilliseconds(700);
        }
        else // manual zoom
        {
          var graphCoord = ConvertMouseToRootLayerCoordinates(position);
          ZoomAroundPivotPoint(newZoom, graphCoord);
        }
      }
      else if (keyModifiers.HasFlag(AltaxoKeyboardModifierKeys.Shift))
      {
        MouseWheelScroll(true, e.Delta);
      }
      else
      {
        MouseWheelScroll(false, e.Delta);
      }
    }

    #endregion Event handlers forwarded by view

    #region Event handlers set-up by this controller

    /// <summary>
    /// Handles the double click event onto a plot item.
    /// </summary>
    /// <param name="hit">Object containing information about the double clicked object.</param>
    /// <returns>True if the object should be deleted, false otherwise.</returns>
    protected static bool EhEditPlotItem(IHitTestObject hit)
    {
      XYPlotLayer actLayer = hit.ParentLayer as XYPlotLayer;
      IGPlotItem pa = (IGPlotItem)hit.HittedObject;

      // get plot group
      PlotGroupStyleCollection plotGroup = pa.ParentCollection.GroupStyles;

      Current.Gui.ShowDialog(new object[] { pa }, string.Format("#{0}: {1}", pa.Name, pa.ToString()), true);

      return false;
    }

    /// <summary>
    /// Handles the double click event onto a plot item.
    /// </summary>
    /// <param name="hit">Object containing information about the double clicked object.</param>
    /// <returns>True if the object should be deleted, false otherwise.</returns>
    protected static bool EhEditTextGraphics(IHitTestObject hit)
    {
      var layer = hit.ParentLayer;
      TextGraphic tg = (TextGraphic)hit.HittedObject;

      bool shouldDeleted = false;

      object tgoo = tg;
      if (Current.Gui.ShowDialog(ref tgoo, "Edit text", true))
      {
        tg = (TextGraphic)tgoo;
        if (tg == null || tg.Empty)
        {
          if (null != hit.Remove)
            shouldDeleted = hit.Remove(hit);
          else
            shouldDeleted = false;
        }
        else
        {
          if (tg.ParentObject is IChildChangedEventSink)
            ((IChildChangedEventSink)tg.ParentObject).EhChildChanged(tg, EventArgs.Empty);
        }
      }

      return shouldDeleted;
    }

    internal void CaptureMouse()
    {
      if (null != _view)
        _view.CaptureMouseOnCanvas();
    }

    internal void ReleaseMouseCapture()
    {
      if (null != _view)
        _view.ReleaseCaptureMouseOnCanvas();
    }

    #endregion Event handlers set-up by this controller

    #region Painting

    /// <summary>
    /// This functions scales the graphics context to be ready for painting.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    public void ScaleForPaint(Graphics g)
    {
      // g.SmoothingMode = SmoothingMode.AntiAlias;
      // get the dpi settings of the graphics context,
      // for example; 96dpi on screen, 600dpi for the printer
      // used to adjust grid and margin sizing.

      g.PageUnit = GraphicsUnit.Point;
      g.PageScale = (float)this.ZoomFactor;
    }

    public void ScaleForPaintingGraphDocument(Graphics g)
    {
      ScaleForPaint(g);

      g.Clear(this._nonPageAreaColor);
      // Fill the page with its own color
      //g.FillRectangle(_pageGroundBrush,_doc.PageBounds);
      //g.FillRectangle(m_PrintableAreaBrush,m_Graph.PrintableBounds);
      g.FillRectangle(_graphAreaBrush, (float)-PositionOfViewportsUpperLeftCornerInGraphCoordinates.X, (float)-PositionOfViewportsUpperLeftCornerInGraphCoordinates.Y, (float)Doc.Size.X, (float)Doc.Size.Y);
      // DrawMargins(g);

      // Paint the graph now
      //g.TranslateTransform(m_Graph.PrintableBounds.X,m_Graph.PrintableBounds.Y); // translate the painting to the printable area
      g.TranslateTransform((float)-PositionOfViewportsUpperLeftCornerInGraphCoordinates.X, (float)-PositionOfViewportsUpperLeftCornerInGraphCoordinates.Y);
    }

    /// <summary>
    /// If the cached graph bitmap is valid, the graph area is repainted immediately using the cached bitmap and then the custom mouse handler drawing.
    /// If the cached graph bitmap is invalid, a repaint (and thus a recreation of the cached graph bitmap) is triggered, but only with Gui render priority.
    /// </summary>
    public void RenderOverlay()
    {
      if (_view == null || Doc == null || _view.ViewportSizeInPoints == PointD2D.Empty)
        return;

      _view.EhRenderOverlayTriggered();
    }

    /// <summary>
    /// Infrastructure: intended to be used by graph views to draw the overlay (the selection rectangles and handles of the currently selected tool) into a bitmap.
    /// </summary>
    /// <param name="g">The graphics contexts (ususally created from a bitmap).</param>
    public void DoPaintOverlay(Graphics g, double zoomFactor, PointD2D positionOfViewportsUpperLeftCornerInGraphCoordinates)
    {
      // g.SmoothingMode = SmoothingMode.AntiAlias;
      // get the dpi settings of the graphics context,
      // for example; 96dpi on screen, 600dpi for the printer
      // used to adjust grid and margin sizing.

      g.PageUnit = GraphicsUnit.Point;
      g.PageScale = (float)zoomFactor;

      g.Clear(System.Drawing.Color.Transparent);

      // special painting depending on current selected tool
      g.TranslateTransform((float)-positionOfViewportsUpperLeftCornerInGraphCoordinates.X, (float)-positionOfViewportsUpperLeftCornerInGraphCoordinates.Y);
      _view.MouseState_AfterPaint(g);
    }

    public bool IsOverlayPaintingRequired
    {
      get
      {
        return _view?.MouseState_IsOverlayPaintingRequired ?? false;
      }
    }

    #endregion Painting

    #region Scaling and Positioning

    /// <summary>
    /// Factor for conversion of graph units (in points = 1/72 inch) to mouse coordinates.
    /// The resolution used for this is <see cref="_screenResolutionDpi"/>.
    /// </summary>
    public PointD2D FactorForGraphToMouseCoordinateConversion
    {
      get
      {
        return new PointD2D(96, 96) * (ZoomFactor * InchPerPoint);
      }
    }

    /// <summary>
    /// Converts from mouse coordinates to graph coordinates.
    /// </summary>
    /// <param name="mouseCoord">Mouse coordinates as returned by MouseEvents.</param>
    /// <returns>Position of the provided point in graph coordinates in points (1/72 inch).</returns>
    public PointD2D ConvertMouseToRootLayerCoordinates(PointD2D mouseCoord)
    {
      var offset = PositionOfViewportsUpperLeftCornerInGraphCoordinates;
      var factor = FactorForGraphToMouseCoordinateConversion;
      return new PointD2D(offset.X + mouseCoord.X / factor.X, offset.Y + mouseCoord.Y / factor.Y);
    }

    /// <summary>
    /// Converts graph coordinates to wpf coordinates.
    /// </summary>
    /// <param name="graphCoord">Graph coordinates.</param>
    /// <returns>Pixel coordinates as returned by MouseEvents</returns>
    public PointD2D ConvertGraphToMouseCoordinates(PointD2D graphCoord)
    {
      var offset = PositionOfViewportsUpperLeftCornerInGraphCoordinates;
      var factor = FactorForGraphToMouseCoordinateConversion;
      return new PointD2D((graphCoord.X - offset.X) * factor.X, (graphCoord.Y - offset.Y) * factor.Y);
    }

    #endregion Scaling and Positioning

    #region Finding objects at position

    /// <summary>
    /// Looks for a graph object at pixel position <paramref name="pixelPos"/> and returns true if one is found.
    /// </summary>
    /// <param name="pixelPos">The pixel coordinates (graph panel coordinates)</param>
    /// <param name="plotItemsOnly">If true, only the plot items where hit tested.</param>
    /// <param name="foundObject">Found object if there is one found, else null</param>
    /// <param name="foundInLayerNumber">The layer the found object belongs to, otherwise 0</param>
    /// <returns>True if a object was found at the pixel coordinates <paramref name="pixelPos"/>, else false.</returns>
    public bool FindGraphObjectAtPixelPosition(PointD2D pixelPos, bool plotItemsOnly, out IHitTestObject foundObject, out int[] foundInLayerNumber)
    {
      var mousePT = ConvertMouseToRootLayerCoordinates(pixelPos);
      var hitData = new HitTestPointData(mousePT, this.ZoomFactor);

      foundObject = RootLayer.HitTest(hitData, plotItemsOnly);
      if (null != foundObject && null != foundObject.ParentLayer)
      {
        foundInLayerNumber = foundObject.ParentLayer.IndexOf().ToArray();
        return true;
      }

      foundObject = null;
      foundInLayerNumber = null;
      return false;
    }

    public void FindGraphObjectInRootLayerRectangle(RectangleD2D rectRootLayerCoordinates, out List<IHitTestObject> foundObjects)
    {
      foundObjects = new List<IHitTestObject>();
      var hitData = new HitTestRectangularData(rectRootLayerCoordinates, this.ZoomFactor);
      RootLayer.HitTest(hitData, foundObjects);
    }

    #endregion Finding objects at position
  }
}
