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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Altaxo.Collections;

namespace Altaxo.Gui.Graph.Gdi.Viewing
{
  using Altaxo.Geometry;
  using Altaxo.Graph;
  using Altaxo.Graph.Gdi;

  /// <summary>
  /// Interaction logic for GraphView.xaml
  /// </summary>
  public partial class GraphViewWpf : UserControl, Altaxo.Gui.Graph.Gdi.Viewing.IGraphView, IDisposable
  {
    [Browsable(false)]
    private int[] _cachedCurrentLayer = null;

    private WeakReference _controller = new WeakReference(null);

    private PointD2D _cachedGraphSize_96thInch;
    private System.Drawing.Size _cachedGraphSize_Pixels;

    /// <summary>Used for debugging the number of updates to the graph.</summary>
#pragma warning disable CS0169
    private int _updateCount;
#pragma warning restore CS0169 

    private static Altaxo.Collections.CachedObjectManagerByMaximumNumberOfItems<System.Drawing.Size, GdiToWpfBitmap> _gdiWpfBitmapManager = new CachedObjectManagerByMaximumNumberOfItems<System.Drawing.Size, GdiToWpfBitmap>(16);

    private static DrawingImage _busyWithRenderingDrawing = (DrawingImage)new DrawingImage(new GlyphRunDrawing(Brushes.Lavender, BuildGlyphRun(" Busy with rendering ... "))).GetAsFrozen();

    private volatile bool _isGraphVisible;

    private volatile bool _isGraphUpToDate;

    private CachedGraphImage _cachedGraphImage;

    private bool _isDisposed;

    /// <summary>A instance of a mouse handler class that currently handles the mouse events. Is mirrored from
    /// the controller to here.</summary>
    protected GraphControllerMouseHandlers.MouseStateHandler _mouseState;

    private class CachedGraphImage
    {
      public PointD2D ViewPortsUpperLeftCornerInGraphCoordinates;
      public PointD2D Size;
      public System.Drawing.Size BitmapSize_Pixel;
      public double ZoomFactor;
      private WeakReference _cachedGraphImageSource;
      private WeakReference _cachedOverlayImageSource;

      public ImageSource CachedGraphImageSource
      {
        get
        {
          return _cachedGraphImageSource?.Target as ImageSource;
        }
        set
        {
          _cachedGraphImageSource = new WeakReference(value);
        }
      }

      public ImageSource CachedOverlayImageSource
      {
        get
        {
          return _cachedOverlayImageSource?.Target as ImageSource;
        }
        set
        {
          _cachedOverlayImageSource = new WeakReference(value);
        }
      }
    }

    static GraphViewWpf()
    {
      var glyphRun = BuildGlyphRun(" Busy with rendering...");
      var glyphRunDrawing = new GlyphRunDrawing(Brushes.Lavender, glyphRun);
      var tileBrush = new DrawingBrush
      {
        Drawing = glyphRunDrawing,
        Viewbox = new Rect(0, 0, glyphRunDrawing.Bounds.Right * 1.1, glyphRunDrawing.Bounds.Bottom * 1.5),
        ViewboxUnits = BrushMappingMode.Absolute,
        Viewport = glyphRunDrawing.Bounds,
        ViewportUnits = BrushMappingMode.Absolute,
        TileMode = TileMode.Tile
      };
      var drawingRectangle = new RectangleGeometry(new Rect(0, 0, 2000, 2000));
      var drawing = new GeometryDrawing(tileBrush, null, drawingRectangle);
      _busyWithRenderingDrawing = new DrawingImage(drawing);
    }

    public GraphViewWpf()
    {
      InitializeComponent();
      _graphImage.Source = _busyWithRenderingDrawing;
    }

    public virtual void Dispose()
    {
      _isDisposed = true;
      _controller = new WeakReference(null);
      _mouseState = null;
      _graphImage.Source = null;
      _graphOverlay.Source = null;
    }

    private Altaxo.Gui.Graph.Gdi.Viewing.GraphController Controller
    {
      get
      {
        return (GraphController)_controller.Target;
      }
    }

    public GraphToolType CurrentGraphTool
    {
      get
      {
        return _mouseState is null ? GraphToolType.None : _mouseState.GraphToolType;
      }
      set
      {
        GraphToolType oldType = CurrentGraphTool;
        if (oldType != value)
        {
          switch (value)
          {
            case GraphToolType.None:
              _mouseState = null;
              break;

            case GraphToolType.ArrowLineDrawing:
              _mouseState = new GraphControllerMouseHandlers.ArrowLineDrawingMouseHandler(Controller);
              break;

            case GraphToolType.CurlyBraceDrawing:
              _mouseState = new GraphControllerMouseHandlers.CurlyBraceDrawingMouseHandler(Controller);
              break;

            case GraphToolType.EllipseDrawing:
              _mouseState = new GraphControllerMouseHandlers.EllipseDrawingMouseHandler(Controller);
              break;

            case GraphToolType.ObjectPointer:
              _mouseState = new GraphControllerMouseHandlers.ObjectPointerMouseHandler(Controller);
              break;

            case GraphToolType.ReadPlotItemData:
              _mouseState = new GraphControllerMouseHandlers.ReadPlotItemDataMouseHandler(Controller);
              break;

            case GraphToolType.ReadXYCoordinates:
              _mouseState = new GraphControllerMouseHandlers.ReadXYCoordinatesMouseHandler(Controller);
              break;

            case GraphToolType.RectangleDrawing:
              _mouseState = new GraphControllerMouseHandlers.RectangleDrawingMouseHandler(Controller);
              break;

            case GraphToolType.RegularPolygonDrawing:
              _mouseState = new GraphControllerMouseHandlers.RegularPolygonDrawingMouseHandler(Controller);
              break;

            case GraphToolType.SingleLineDrawing:
              _mouseState = new GraphControllerMouseHandlers.SingleLineDrawingMouseHandler(Controller);
              break;

            case GraphToolType.TextDrawing:
              _mouseState = new GraphControllerMouseHandlers.TextToolMouseHandler(Controller);
              break;

            case GraphToolType.ZoomAxes:
              _mouseState = new GraphControllerMouseHandlers.ZoomAxesMouseHandler(Controller);
              break;

            case GraphToolType.OpenCardinalSplineDrawing:
              _mouseState = new GraphControllerMouseHandlers.OpenCardinalSplineMouseHandler(Controller);
              break;

            case GraphToolType.ClosedCardinalSplineDrawing:
              _mouseState = new GraphControllerMouseHandlers.ClosedCardinalSplineMouseHandler(Controller);
              break;

            case GraphToolType.EditGrid:
              _mouseState = new GraphControllerMouseHandlers.EditGridMouseHandler(Controller);
              break;

            default:
              throw new NotImplementedException("Type not implemented: " + value.ToString());
          } // end switch

          FocusOnGraphPanel();

          Controller?.EhView_CurrentGraphToolChanged();

          EhGraphToolChanged();
        }
      }
    }

    private static IList<IHitTestObject> _emptyReadOnlyList = new List<IHitTestObject>().AsReadOnly();

    public IList<IHitTestObject> SelectedObjects
    {
      get
      {
        if (_mouseState is GraphControllerMouseHandlers.ObjectPointerMouseHandler opmh)
          return opmh.SelectedObjects;
        else
          return _emptyReadOnlyList;
      }
    }

    public bool MouseState_IsOverlayPaintingRequired { get { return _mouseState?.IsOverlayPaintingRequired ?? false; } }

    public void MouseState_AfterPaint(System.Drawing.Graphics g)
    {
      _mouseState?.AfterPaint(g);
    }

    public void FocusOnGraphPanel()
    {
      bool result = _guiCanvas.Focus();
      Keyboard.Focus(_guiCanvas);
    }

    public void SetPanelCursor(Cursor cursor)
    {
      _guiCanvas.Cursor = cursor;
    }

    public void SetPanelCursor(object cursor)
    {
      _guiCanvas.Cursor = (Cursor)cursor;
    }

    /// <summary>
    /// Called by the PresentationGraphController to get a new graphics context for painting.
    /// </summary>
    /// <returns></returns>
    public static System.Drawing.Graphics GetGraphicsContextFromWpfGdiBitmap(GdiToWpfBitmap gdiWpfBitmap)
    {
      var grfx = gdiWpfBitmap.BeginGdiPainting();
      grfx.ResetTransform();
      grfx.PageScale = 1;
      grfx.PageUnit = System.Drawing.GraphicsUnit.Pixel;
      return grfx;
    }

    private PointD2D GetMousePosition(MouseEventArgs e)
    {
      var p = e.GetPosition(_guiCanvas);
      return new PointD2D(p.X, p.Y);
    }

    private void EhGraphPanel_MouseMove(object sender, MouseEventArgs e)
    {
      _mouseState.OnMouseMove(GetMousePosition(e), e);
    }

    private void EhGraphPanel_MouseDown(object sender, MouseButtonEventArgs e)
    {
      var guiController = Controller;
      Keyboard.Focus(_guiCanvas);
      var pos = GetMousePosition(e);
      if (guiController is not null)
      {
        _mouseState.OnMouseDown(pos, e);
        if (e.ClickCount >= 2 && e.LeftButton == MouseButtonState.Pressed)
          _mouseState.OnDoubleClick(pos, e);
        else if (e.ClickCount == 1 && e.LeftButton == MouseButtonState.Pressed)
          _mouseState.OnClick(pos, e);
      }
    }

    private void EhGraphPanel_MouseUp(object sender, MouseButtonEventArgs e)
    {
      _mouseState.OnMouseUp(GetMousePosition(e), e);
    }

    private void EhGraphPanel_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      var guiController = Controller;
      if (guiController is not null)
      {
        var eHand = new HandledEventArgs();
        guiController.EhView_GraphPanelMouseWheel(GuiHelper.ToAltaxo(e, _guiCanvas), GuiHelper.ToAltaxo(Keyboard.Modifiers), eHand);
        e.Handled = eHand.Handled;
        ShowCachedGraphImage();
      }
    }

    private void EhGraphPanel_KeyDown(object sender, KeyEventArgs e)
    {
      e.Handled = _mouseState?.ProcessCmdKey(e) ?? false;
    }

    #region Altaxo.Gui.Graph.Viewing.IGraphView

    object Altaxo.Gui.Graph.Gdi.Viewing.IGraphView.GuiInitiallyFocusedElement
    {
      get
      {
        // make sure that the returned element implements IInputElement
        var result = _graphImage;
        if ((result as System.Windows.IInputElement) is null)
          throw new InvalidOperationException(nameof(result) + " should be an IInputElement");

        return result;
      }
    }

    IGraphViewEventSink IGraphView.Controller
    {
      set
      {
        var oldcontroller = _controller;
        _controller = new WeakReference(value as GraphController);

        if (value is not null)
        {
          if (_isDisposed)
            throw new ObjectDisposedException(nameof(GraphViewWpf));

          ((IGraphView)this).InvalidateCachedGraphBitmapAndRepaint();
        }
        else
        {
          Dispose();
        }
      }
    }

    private void EhGraphPanelSizeChanged(object sender, SizeChangedEventArgs e)
    {
      // System.Diagnostics.Debug.WriteLine("GraphViewWpf.EhGraphPanel_SizeChanged, Name={0}, NewSize={1}x{2}", Controller?.Doc?.Name, e.NewSize.Width, e.NewSize.Height);
      OnGraphPanel_SizeChanged(new PointD2D(e.NewSize.Width, e.NewSize.Height));
    }

    private void OnGraphPanel_SizeChanged(PointD2D newSize)
    {
      if (!(newSize.X > 0 && newSize.Y > 0))
        return;

      _guiCanvas.Clip = new RectangleGeometry(new Rect(0, 0, newSize.X, newSize.Y));

      if (_graphImage.Source is null)
      {
        // invalidate the cached graph sizes in order to force a new rendering
        _cachedGraphSize_Pixels = new System.Drawing.Size(0, 0);
        _cachedGraphSize_96thInch = new PointD2D(0, 0);
      }

      if (newSize != _cachedGraphSize_96thInch)
      {
        _cachedGraphSize_96thInch = newSize;
        var screenResolution = Current.Gui.ScreenResolutionDpi;
        var graphSizePixels = screenResolution * _cachedGraphSize_96thInch / 96.0;
        _cachedGraphSize_Pixels = new System.Drawing.Size((int)graphSizePixels.X, (int)graphSizePixels.Y);

        Controller?.EhView_GraphPanelSizeChanged(); // inform controller

        ShowCachedGraphImage();

        _isGraphUpToDate = false;
        if (_isGraphVisible)
          ((IGraphView)this).InvalidateCachedGraphBitmapAndRepaint();
      }
    }

    public void AnnounceContentVisibilityChanged(bool isVisible)
    {
      _isGraphVisible = isVisible;

      if (_isGraphVisible)
      {
        var ctrl = Controller;
        if (ctrl is not null)
          _guiCanvas.Background = new SolidColorBrush(GuiHelper.ToWpf(ctrl.NonPageAreaColor));

        if (_isGraphUpToDate && true == ShowCachedGraphImage())
        {
        }
        else
        {
          _isGraphUpToDate = false;
          _graphImage.Source = _busyWithRenderingDrawing;

          ((IGraphView)this).InvalidateCachedGraphBitmapAndRepaint();
        }
      }
      else
      {
        _graphImage.Source = null;
        _graphOverlay.Source = null;
      }
    }

    private bool ShowCachedGraphImage()
    {
      var controller = Controller;
      if (_cachedGraphImage is not null && controller is not null)
      {
        var imgSrc = _cachedGraphImage.CachedGraphImageSource;
        if (imgSrc is not null)
        {
          var zoom1 = controller.ZoomFactor; // current zoom factor
          var vpulcgc1 = controller.PositionOfViewportsUpperLeftCornerInGraphCoordinates; // current position of viewports upper left corner

          var size1 = zoom1 * _cachedGraphImage.Size / _cachedGraphImage.ZoomFactor; // size that the graph image should take at the canvas' surface
          var pos1 = (96 / 72.0) * zoom1 * (_cachedGraphImage.ViewPortsUpperLeftCornerInGraphCoordinates - vpulcgc1); // position of the upper left corner of the graph image that the graph image should take at the canvas surface

          _graphImage.SetValue(Canvas.LeftProperty, pos1.X);
          _graphImage.SetValue(Canvas.TopProperty, pos1.Y);
          _graphImage.Width = size1.X;
          _graphImage.Height = size1.Y;

          _graphImage.Source = _cachedGraphImage.CachedGraphImageSource;

          _graphOverlay.SetValue(Canvas.LeftProperty, pos1.X);
          _graphOverlay.SetValue(Canvas.TopProperty, pos1.Y);
          _graphOverlay.Width = size1.X;
          _graphOverlay.Height = size1.Y;
          _graphOverlay.Source = _cachedGraphImage.CachedOverlayImageSource;

          // Current.Console.WriteLine("ShowCGI, Zoom=({0}|{1}), PVULCGC=({2}|{3}), Size=({4}|{5}), CurrSize={6}, Pos={7}", _cachedGraphImage.ZoomFactor, zoom1, _cachedGraphImage.ViewPortsUpperLeftCornerInGraphCoordinates, vpulcgc1, _cachedGraphImage.Size, size1, _cachedGraphSize_96thInch, pos1);

          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Causes a complete redrawing of the graph. The cached graph bitmap will be marked as dirty and a repainting of the graph area is triggered with Gui render priority.
    /// Note: it is save to call this function from non-Gui threads.
    /// </summary>
    void IGraphView.InvalidateCachedGraphBitmapAndRepaint()
    {
      if (_isGraphVisible)
      {
        StartCompleteRepaint();
      }
      else
      {
        _isGraphUpToDate = false;
      }
    }

    /// <summary>
    /// Causes a complete redrawing of the graph. The cached graph bitmap will be marked as dirty and a repainting of the graph area is triggered with Gui render priority.
    /// Note: it is save to call this function from non-Gui threads.
    /// </summary>
    private void StartCompleteRepaint()
    {
      var controller = Controller;
      if (controller is null)
        return;

      // rendering in the background
      Altaxo.Graph.Gdi.GraphDocumentRenderManager.Instance.AddTask(
        controller,
        controller.Doc,
(graphDocument, token) =>
        {
          if (graphDocument.IsDisposeInProgress)
            return;

          var size = _cachedGraphSize_Pixels;

          if (size.Width > 1 && size.Height > 1)
          {
            if (!_gdiWpfBitmapManager.TryTake(size, out var bmp))
              Current.Dispatcher.InvokeIfRequired((Action)(() => bmp = new GdiToWpfBitmap(size.Width, size.Height)));

            var grfx = GetGraphicsContextFromWpfGdiBitmap(bmp);
            controller.ScaleForPaintingGraphDocument(grfx);
            var cachedGraphImage = new CachedGraphImage { ZoomFactor = controller.ZoomFactor, ViewPortsUpperLeftCornerInGraphCoordinates = controller.PositionOfViewportsUpperLeftCornerInGraphCoordinates, Size = _cachedGraphSize_96thInch, BitmapSize_Pixel = size };

            if (!graphDocument.IsDisposeInProgress)
              graphDocument.Paint(grfx, false);

            Current.Dispatcher.InvokeIfRequired(() =>
            {
              var bmpSource = bmp.WpfBitmapSource;
              cachedGraphImage.CachedGraphImageSource = bmpSource;
              _cachedGraphImage = cachedGraphImage;
              _graphImage.Source = bmpSource;
              _isGraphUpToDate = true;
              grfx.Dispose();

              var overlay = GetImageSourceByRenderingOverlay(controller, bmp, cachedGraphImage);
              _graphOverlay.Source = overlay;
              cachedGraphImage.CachedOverlayImageSource = overlay;
              _gdiWpfBitmapManager.Add(bmp.GdiSize, bmp);

              ShowCachedGraphImage();
            });
          }
        }
        );
    }

    /// <summary>
    /// Called from the controller when the graph tool has changed.
    /// </summary>
    internal void EhGraphToolChanged()
    {
      RenderOverlayAndShowImmediately();
    }

    /// <summary>
    /// Called from the controller when the overlay changed and should be rendered anew.
    /// </summary>
    public void EhRenderOverlayTriggered()
    {
      RenderOverlayAndShowImmediately();
    }

    /// <summary>
    /// Renders the overlay (the drawing that designates selected rectangles, handles and so on) immediately in the current thread. Then it is stored
    /// in cachedGraphImage and is also immediatly assigned to be shown in the view.
    /// Attention: if there is no cached bitmap, a new bitmap is created, but this must be done in the Gui context, so this can lead to deadlocks.
    /// </summary>
    private void RenderOverlayAndShowImmediately()
    {
      var controller = Controller;
      var cachedGraphImage = _cachedGraphImage;
      ImageSource overlay = null;

      if (
        controller is not null &&
        controller.IsOverlayPaintingRequired &&
        cachedGraphImage is not null &&
        cachedGraphImage.BitmapSize_Pixel.Width > 1 &&
        cachedGraphImage.BitmapSize_Pixel.Height > 1)
      {
        var size = cachedGraphImage.BitmapSize_Pixel;

        if (!_gdiWpfBitmapManager.TryTake(size, out var bmp))
          Current.Dispatcher.InvokeIfRequired(() => bmp = new GdiToWpfBitmap(size.Width, size.Height));

        try
        {
          overlay = GetImageSourceByRenderingOverlay(controller, bmp, cachedGraphImage);
        }
        finally
        {
          _gdiWpfBitmapManager.Add(bmp.GdiSize, bmp);
        }
      }
      if (_cachedGraphImage is not null)
        _cachedGraphImage.CachedOverlayImageSource = overlay;
      _graphOverlay.Source = overlay;
    }

    /// <summary>
    /// Renders the overlay (the drawing that designates selected rectangles, handles and so on) immediately in the current thread.
    /// Attention: if there is no cached bitmap, a new bitmap is created, but this must be done in the Gui context, so this can lead to deadlocks.
    /// </summary>
    private static ImageSource GetImageSourceByRenderingOverlay(GraphController controller, GdiToWpfBitmap bmp, CachedGraphImage cachedGraphImage)
    {
      if (controller.IsOverlayPaintingRequired)
      {
        using (var grfx = GetGraphicsContextFromWpfGdiBitmap(bmp))
        {
          controller.DoPaintOverlay(grfx, cachedGraphImage.ZoomFactor, cachedGraphImage.ViewPortsUpperLeftCornerInGraphCoordinates);
          return bmp.WpfBitmapSource;
        }
      }
      else
      {
        return null;
      }
    }

    public string GraphViewTitle
    {
      set
      {
        // TODO (Wpf)
      }
    }

    public bool ShowGraphScrollBars
    {
      set
      {
        _horizontalScrollBar.IsEnabled = value;
        _verticalScrollBar.IsEnabled = value;
      }
    }

    public PointD2D GraphScrollPosition
    {
      get
      {
        return new PointD2D(_horizontalScrollBar.Value, _verticalScrollBar.Value);
      }
    }

    public void SetHorizontalScrollbarParameter(bool isEnabled, double value, double maximum, double viewportSize, double largeIncrement, double smallIncrement)
    {
      var controller = _controller;
      _controller = new WeakReference(null); // suppress scrollbar events

      _horizontalScrollBar.IsEnabled = isEnabled;
      _horizontalScrollBar.Maximum = maximum;
      _horizontalScrollBar.ViewportSize = viewportSize;
      _horizontalScrollBar.SmallChange = smallIncrement;
      _horizontalScrollBar.LargeChange = largeIncrement;
      _horizontalScrollBar.Value = value;

      _controller = controller;
    }

    public void SetVerticalScrollbarParameter(bool isEnabled, double value, double maximum, double viewportSize, double largeIncrement, double smallIncrement)
    {
      var controller = _controller;
      _controller = new WeakReference(null); // suppress scrollbar events

      _verticalScrollBar.IsEnabled = isEnabled;
      _verticalScrollBar.Maximum = maximum;
      _verticalScrollBar.ViewportSize = viewportSize;
      _verticalScrollBar.SmallChange = smallIncrement;
      _verticalScrollBar.LargeChange = largeIncrement;
      _verticalScrollBar.Value = value;

      _controller = controller;
    }

    public void SetLayerStructure(NGTreeNode value, int[] currentLayerNumber)
    {
      _layerToolBar.Children.Clear();

      foreach (var node in value.TakeFromHereToFirstLeaves())
      {
        var newbutton = new ToggleButton() { Content = node.Text, Tag = node.Tag };
        newbutton.Margin = new Thickness(node.Level * 8, 2, 1, 0);
        newbutton.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        newbutton.Click += new RoutedEventHandler(EhLayerButton_Click);
        newbutton.ContextMenuOpening += new ContextMenuEventHandler(EhLayerButton_ContextMenuOpening);
        newbutton.IsChecked = System.Linq.Enumerable.SequenceEqual((int[])(node.Tag), currentLayerNumber);

        _layerToolBar.Children.Add(newbutton);
      }
    }

    private void EhLayerButton_Click(object sender, RoutedEventArgs e)
    {
      var gc = Controller;
      if (gc is not null)
      {
        var pushedLayerNumber = (int[])((ButtonBase)sender).Tag;

        gc.EhView_CurrentLayerChoosen(pushedLayerNumber, false);
      }
    }

    private void EhLayerButton_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
      var gc = Controller;
      if (gc is not null)
      {
        var i = (int[])((ToggleButton)sender).Tag;
        Controller.EhView_ShowDataContextMenu(i, this, new System.Drawing.Point((int)e.CursorLeft, (int)e.CursorTop));
      }
    }

    public int[] CurrentLayer
    {
      set
      {
        _cachedCurrentLayer = value;

        var ccl = (IList<int>)_cachedCurrentLayer;

        for (int i = 0; i < _layerToolBar.Children.Count; i++)
        {
          var button = (ToggleButton)_layerToolBar.Children[i];
          button.IsChecked = ccl.SequenceEqual((int[])button.Tag);
        }
      }
    }

    public PointD2D ViewportSizeInPoints
    {
      get
      {
        const double factor = 72.0 / 96.0;
        return new PointD2D(_cachedGraphSize_96thInch.X * factor, _cachedGraphSize_96thInch.Y * factor);
      }
    }

    #endregion Altaxo.Gui.Graph.Viewing.IGraphView

    private void EhHorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
      var gc = Controller;
      if (gc is not null)
      {
        gc.EhView_Scroll();
        ShowCachedGraphImage();
      }
    }

    private void EhVerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
      var gc = Controller;
      if (gc is not null)
      {
        gc.EhView_Scroll();
        ShowCachedGraphImage();
      }
    }

    private void EhEnableCmdCopy(object sender, CanExecuteRoutedEventArgs e)
    {
      var gc = Controller;
      e.CanExecute = gc is not null && gc.IsCmdCopyEnabled();
      e.Handled = true;
    }

    private void EhEnableCmdCut(object sender, CanExecuteRoutedEventArgs e)
    {
      var gc = Controller;
      e.CanExecute = gc is not null && gc.IsCmdCutEnabled();
      e.Handled = true;
    }

    private void EhEnableCmdDelete(object sender, CanExecuteRoutedEventArgs e)
    {
      var gc = Controller;
      e.CanExecute = gc is not null && gc.IsCmdDeleteEnabled();
      e.Handled = true;
    }

    private void EhEnableCmdPaste(object sender, CanExecuteRoutedEventArgs e)
    {
      var gc = Controller;
      e.CanExecute = gc is not null && gc.IsCmdPasteEnabled();
      e.Handled = true;
    }

    private void EhCmdCopy(object sender, ExecutedRoutedEventArgs e)
    {
      var gc = Controller;
      if (gc is not null)
      {
        gc.CopySelectedObjectsToClipboard();
        e.Handled = true;
      }
    }

    private void EhCmdCut(object sender, ExecutedRoutedEventArgs e)
    {
      var gc = Controller;
      if (gc is not null)
      {
        gc.CutSelectedObjectsToClipboard();
        e.Handled = true;
      }
    }

    private void EhCmdDelete(object sender, ExecutedRoutedEventArgs e)
    {
      var gc = Controller;
      if (_controller is not null)
      {
        gc.CmdDelete();
        e.Handled = true;
      }
    }

    private void EhCmdPaste(object sender, ExecutedRoutedEventArgs e)
    {
      var gc = Controller;
      if (_controller is not null)
      {
        gc.PasteObjectsFromClipboard();
        e.Handled = true;
      }
    }

    public static GlyphRun BuildGlyphRun(string text)
    {
      double fontSize = 50;
      GlyphRun glyphs = null;

      var font = new Typeface("Arial");
      if (font.TryGetGlyphTypeface(out var glyphFace))
      {
        glyphs = new GlyphRun(1);
        System.ComponentModel.ISupportInitialize isi = glyphs;
        isi.BeginInit();
        glyphs.GlyphTypeface = glyphFace;
        glyphs.FontRenderingEmSize = fontSize;

        char[] textChars = text.ToCharArray();
        glyphs.Characters = textChars;
        ushort[] glyphIndices = new ushort[textChars.Length];
        double[] advanceWidths = new double[textChars.Length];

        for (int i = 0; i < textChars.Length; ++i)
        {
          int codepoint = textChars[i];
          ushort glyphIndex = glyphFace.CharacterToGlyphMap[codepoint];
          double glyphWidth = glyphFace.AdvanceWidths[glyphIndex];

          glyphIndices[i] = glyphIndex;
          advanceWidths[i] = glyphWidth * fontSize;
        }
        glyphs.GlyphIndices = glyphIndices;
        glyphs.AdvanceWidths = advanceWidths;

        glyphs.BaselineOrigin = new Point(0, glyphFace.Baseline * fontSize);
        isi.EndInit();
      }
      return glyphs;
    }

    void IGraphView.CaptureMouseOnCanvas()
    {
      _guiCanvas.CaptureMouse();
    }

    void IGraphView.ReleaseCaptureMouseOnCanvas()
    {
      _guiCanvas.ReleaseMouseCapture();
    }
  }
}
