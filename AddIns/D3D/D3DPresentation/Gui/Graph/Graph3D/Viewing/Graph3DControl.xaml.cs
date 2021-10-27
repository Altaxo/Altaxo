#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Graph.Graph3D.Viewing
{
  using System.Windows.Controls.Primitives;
  using Altaxo.Collections;
  using Altaxo.Geometry;
  using Altaxo.Graph.Graph3D;
  using Altaxo.Graph.Graph3D.GraphicsContext;
  using Altaxo.Graph.Graph3D.GraphicsContext.D3D;
  using Altaxo.Gui.Graph.Graph3D.Common;
  using Altaxo.Gui.Graph.Graph3D.Viewing.GraphControllerMouseHandlers;
  using Graph.Graph3D.Viewing;

  /// <summary>
  /// Interaction logic for Graph3DControl.xaml
  /// </summary>
  public partial class Graph3DControl : UserControl, IGraph3DView, IDisposable
  {
    private WeakReference _controller = new WeakReference(null);

    private D3D11Scene _scene;

    private int[]? _cachedCurrentLayer = null;

    private PointD2D _cachedGraphSize_96thInch;
    private System.Drawing.Size _cachedGraphSize_Pixels;

    private D3D11RendererToImageSource? _renderer;

    private volatile bool _isGraphVisible;

    /// <summary>A instance of a mouse handler class that currently handles the mouse events..</summary>
    protected MouseStateHandler _mouseState;

    private bool _isDisposed;

    private static IList<IHitTestObject> _emptyReadOnlyList = new List<IHitTestObject>().AsReadOnly();

    public Graph3DControl()
    {
      InitializeComponent();

      _scene = new D3D11Scene();
      _mouseState = null!; // set when controller is set.
    }

    public virtual void Dispose()
    {
      _isDisposed = true;
      _controller = new WeakReference(null);
      _renderer?.Dispose();
      var imgSource = _d3dCanvas?.Source as D3D11ImageSource;
      if (_d3dCanvas is not null)
        _d3dCanvas.Source = null;
      imgSource?.Dispose();
      _scene = null!;
      _mouseState = null!;
    }

    private Graph3DController Controller
    {
      get
      {
        return (Graph3DController)(_controller.Target ?? throw new InvalidOperationException($"Controller not available. IsDisposed: {_isDisposed}"));
      }
    }

    Graph3DController IGraph3DView.Controller
    {
      set
      {
        var oldcontroller = _controller;
        _controller = new WeakReference(value);

        if (value is not null)
        {
          if (_isDisposed)
            throw new ObjectDisposedException(nameof(Graph3DControl));

          _mouseState = new ObjectPointerMouseHandler(value);
        }
        else // new Controller is null, so free any resources
        {
          Dispose();
        }
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
              _mouseState = new GraphControllerMouseHandlers.ObjectPointerMouseHandler(Controller);
              break;

            case GraphToolType.ObjectPointer:
              _mouseState = new GraphControllerMouseHandlers.ObjectPointerMouseHandler(Controller);
              break;

            case GraphToolType.TextDrawing:
              _mouseState = new GraphControllerMouseHandlers.TextToolMouseHandler(Controller);
              break;

            case GraphToolType.EllipseDrawing:
              _mouseState = new GraphControllerMouseHandlers.EllipseDrawingMouseHandler(Controller);
              break;

            case GraphToolType.SingleLineDrawing:
              _mouseState = new GraphControllerMouseHandlers.SingleLineDrawingMouseHandler(Controller);
              break;

            /*

					case GraphToolType.ArrowLineDrawing:
						_mouseState = new GraphControllerMouseHandlers.ArrowLineDrawingMouseHandler(this);
						break;

					case GraphToolType.CurlyBraceDrawing:
						_mouseState = new GraphControllerMouseHandlers.CurlyBraceDrawingMouseHandler(this);
						break;

					case GraphToolType.ReadPlotItemData:
						_mouseState = new GraphControllerMouseHandlers.ReadPlotItemDataMouseHandler(this);
						break;

					case GraphToolType.ReadXYCoordinates:
						_mouseState = new GraphControllerMouseHandlers.ReadXYCoordinatesMouseHandler(this);
						break;

					case GraphToolType.RectangleDrawing:
						_mouseState = new GraphControllerMouseHandlers.RectangleDrawingMouseHandler(this);
						break;

					case GraphToolType.RegularPolygonDrawing:
						_mouseState = new GraphControllerMouseHandlers.RegularPolygonDrawingMouseHandler(this);
						break;

					case GraphToolType.ZoomAxes:
						_mouseState = new GraphControllerMouseHandlers.ZoomAxesMouseHandler(this);
						break;

					case GraphToolType.OpenCardinalSplineDrawing:
						_mouseState = new OpenCardinalSplineMouseHandler(this);
						break;

					case GraphToolType.ClosedCardinalSplineDrawing:
						_mouseState = new ClosedCardinalSplineMouseHandler(this);
						break;

					case GraphToolType.EditGrid:
						_mouseState = new EditGridMouseHandler(this);
						break;

						*/

            default:
              throw new NotImplementedException("Type not implemented: " + value.ToString());
          } // end switch

          FocusOnGraphPanel();

          Controller?.EhView_CurrentGraphToolChanged();

          TriggerRendering();
        }
      }
    }

    public IList<IHitTestObject> SelectedObjects
    {
      get
      {
        if (_mouseState is ObjectPointerMouseHandler)
          return ((ObjectPointerMouseHandler)_mouseState).SelectedObjects;
        else
          return _emptyReadOnlyList;
      }
    }

    public void RenderOverlay()
    {
      var g = GetGraphicContextForOverlay();
      _mouseState.AfterPaint(g);
      SetOverlayGeometry(g);
      TriggerRendering();
    }

    #region Graph panel mouse and keyboard

    private void EhGraphPanel_KeyDown(object sender, KeyEventArgs e)
    {
      _mouseState?.ProcessCmdKey(e);
    }

    private PointD3D GetMousePosition(MouseEventArgs e)
    {
      var p = e.GetPosition(_d3dCanvas);
      return new PointD3D(p.X / _d3dCanvas.ActualWidth, 1 - p.Y / _d3dCanvas.ActualHeight, _d3dCanvas.ActualHeight / _d3dCanvas.ActualWidth);
    }

    private void EhGraphPanel_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (!(_d3dCanvas.ActualWidth > 0 && _d3dCanvas.ActualHeight > 0))
        return; // _d3dCanvas was not measured till now

      var mousePosition = e.GetPosition(_d3dCanvas);
      double relX = mousePosition.X / _d3dCanvas.ActualWidth;
      double relY = 1 - mousePosition.Y / _d3dCanvas.ActualHeight;

      bool isSHIFTpressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
      bool isCTRLpressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
      bool isALTpressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);

      Controller?.EhView_GraphPanelMouseWheel(relX, relY, _d3dCanvas.ActualHeight / _d3dCanvas.ActualWidth, e.Delta, isSHIFTpressed, isCTRLpressed, isALTpressed);
    }

    private void EhGraphPanel_MouseMove(object sender, MouseEventArgs e)
    {
      var position = GetMousePosition(e);
      _mouseState.OnMouseMove(position, e);
      Controller?.EhView_GraphPanelMouseMove(position, GuiHelper.GetMouseState(e));
    }

    private void EhGraphPanel_MouseDown(object sender, MouseButtonEventArgs e)
    {
      var guiController = Controller;
      Keyboard.Focus(_guiCanvas);
      var pos = GetMousePosition(e);

      _mouseState.OnMouseDown(pos, e);
      if (guiController is not null)
      {
        guiController.EhView_GraphPanelMouseDown(pos, GuiHelper.ToAltaxo(e, _d3dCanvas), GuiHelper.ToAltaxo(Keyboard.Modifiers));
        if (e.ClickCount >= 2 && e.LeftButton == MouseButtonState.Pressed)
          _mouseState.OnDoubleClick(pos, e);
        else if (e.ClickCount == 1 && e.LeftButton == MouseButtonState.Pressed)
          _mouseState.OnClick(pos, e);
      }
    }

    private void EhGraphPanel_MouseUp(object sender, MouseButtonEventArgs e)
    {
      var position = GetMousePosition(e);
      _mouseState.OnMouseUp(position, e);
      Controller?.EhView_GraphPanelMouseUp(position, GuiHelper.ToAltaxo(e, _d3dCanvas));
    }

    #endregion Graph panel mouse and keyboard

    #region Graph panel size and visibility

    private void EhGraphPanel_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      // System.Diagnostics.Debug.WriteLine("Graph3DControl.EhGraphPanel_SizeChanged, Name={0}, NewSize={1}x{2}", Controller?.Doc?.Name, e.NewSize.Width, e.NewSize.Height);
      OnGraphPanel_SizeChanged(new PointD2D(e.NewSize.Width, e.NewSize.Height));
    }

    private void OnGraphPanel_SizeChanged(PointD2D newSize)
    {
      if (!(newSize.X > 0 && newSize.Y > 0))
        return;

      if (_renderer is null)
      {
        _d3dCanvas.Source = new D3D11ImageSource();
        _renderer = new D3D11RendererToImageSource(_scene, (D3D11ImageSource)_d3dCanvas.Source, Controller.Doc.Name);
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

        _renderer.SetRenderSize(_cachedGraphSize_Pixels.Width, _cachedGraphSize_Pixels.Height);

        Controller?.EhView_GraphPanelSizeChanged(); // inform controller
      }
    }

    public void AnnounceContentVisibilityChanged(bool isVisible)
    {
      _isGraphVisible = isVisible;
      // System.Diagnostics.Debug.WriteLine("Visibility of Graph {0} is now {1} Canvas: {2}x{3}", Controller?.Doc?.Name, _isGraphVisible, _guiCanvas.ActualWidth, _guiCanvas.ActualHeight);

      if (_isGraphVisible)
      {
        OnGraphPanel_SizeChanged(new PointD2D(_guiCanvas.ActualWidth, _guiCanvas.ActualHeight));
      }
      else
      {
        var tempRenderer = _renderer;
        _renderer = null;
        tempRenderer?.Dispose();

        var oldSource = (D3D11ImageSource)_d3dCanvas.Source;
        _d3dCanvas.Source = null;
        oldSource?.Dispose();
      }
    }

    public static void DisposeObject(ref IDisposable? obj)
    {
      var tempObj = obj;
      obj = null;
      tempObj?.Dispose();
    }

    #endregion Graph panel size and visibility

    #region Layer buttons

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
        newbutton.IsChecked = System.Linq.Enumerable.SequenceEqual((int[])(node.Tag!), currentLayerNumber);

        _layerToolBar.Children.Add(newbutton);
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
        gc.EhView_ShowDataContextMenu(i, this, new PointD2D(e.CursorLeft, e.CursorTop));
      }
    }

    #endregion Layer buttons

    #region Command bindings

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

    #endregion Command bindings

    public void SetPanelCursor(Cursor arrow)
    {
      _d3dCanvas.Cursor = arrow;
    }

    public void SetPanelCursor(object cursor)
    {
      _d3dCanvas.Cursor = (Cursor)cursor;
    }

    public void FocusOnGraphPanel()
    {
      bool result = _guiCanvas.Focus();
      Keyboard.Focus(_guiCanvas);
    }

    public object GuiInitiallyFocusedElement
    {
      get
      {
        return _guiCanvas;
      }
    }

    public PointD2D ViewportSizeInPoints
    {
      get
      {
        const double factor = 72.0 / 96.0;
        return new PointD2D(_d3dCanvas.ActualWidth * factor, _d3dCanvas.ActualHeight * factor);
      }
    }

    public string GraphViewTitle
    {
      set
      {
        // TODO (Wpf)
      }
    }

    /// <summary>
    /// Triggers a new rendering without building up a new geometry. Could be used for instance if the light or the camera has changed, but not the geometry.
    /// </summary>
    public void TriggerRendering()
    {
      if (_isGraphVisible && _renderer is not null)
      {
        Current.Dispatcher.InvokeIfRequired(_renderer.TriggerRendering);
      }
    }

    public void SetSceneBackColor(Altaxo.Drawing.AxoColor sceneBackColor)
    {
      _scene.SetSceneBackColor(sceneBackColor);
    }

    public void SetCamera(Altaxo.Graph.Graph3D.Camera.CameraBase camera, Altaxo.Graph.Graph3D.LightSettings lightSettings)
    {
      _scene.SetCamera(camera);
      _scene.SetLighting(lightSettings);
    }

    public IGraphicsContext3D GetGraphicContext()
    {
      return new D3DGraphicsContext();
    }

    public void SetDrawing(IGraphicsContext3D drawing)
    {
      if (drawing is null)
        throw new ArgumentNullException();

      _scene.SetDrawing((D3DGraphicsContext)drawing);
    }

    public IOverlayContext3D GetGraphicContextForMarkers()
    {
      return new D3DOverlayContext();
    }

    public void SetMarkerGeometry(IOverlayContext3D markerGeometry)
    {
      if (markerGeometry is null)
        throw new ArgumentNullException();

      _scene.SetMarkerGeometry((D3DOverlayContext)markerGeometry);
    }

    public IOverlayContext3D GetGraphicContextForOverlay()
    {
      return new D3DOverlayContext();
    }

    public void SetOverlayGeometry(IOverlayContext3D overlayGeometry)
    {
      if (overlayGeometry is null)
        throw new ArgumentNullException();

      _scene.SetOverlayGeometry((D3DOverlayContext)overlayGeometry);
    }
  }
}
