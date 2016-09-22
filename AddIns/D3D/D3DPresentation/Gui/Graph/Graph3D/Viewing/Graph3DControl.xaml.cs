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
	using Altaxo.Collections;
	using Altaxo.Geometry;
	using Altaxo.Graph.Graph3D;
	using Altaxo.Graph.Graph3D.GraphicsContext;
	using Altaxo.Graph.Graph3D.GraphicsContext.D3D;
	using Altaxo.Gui.Graph.Graph3D.Common;
	using Graph.Graph3D.Viewing;
	using System.Windows.Controls.Primitives;

	/// <summary>
	/// Interaction logic for Graph3DControl.xaml
	/// </summary>
	public partial class Graph3DControl : UserControl, IGraph3DView, IDisposable
	{
		private WeakReference _controller = new WeakReference(null);

		private D3D10Scene _scene;

		private int[] _cachedCurrentLayer = null;

		private PointD2D _cachedGraphSize_96thInch;
		private System.Drawing.Size _cachedGraphSize_Pixels;

		private D3D10RendererToImageSource _renderer;

		private volatile bool _isGraphVisible;

		private volatile bool _isGraphUpToDate;

		public Graph3DControl()
		{
			InitializeComponent();

			_scene = new D3D10Scene();
			var imageSource = new D3D10ImageSource();
			_renderer = new D3D10RendererToImageSource(_scene, imageSource);
			_d3dCanvas.Source = imageSource;
		}

		public virtual void Dispose()
		{
			_controller = new WeakReference(null);
			_renderer?.Dispose();
		}

		private Graph3DControllerWpf Controller
		{
			get
			{
				return (Graph3DControllerWpf)_controller.Target;
			}
		}

		Graph3DController IGraph3DView.Controller
		{
			set
			{
				var oldcontroller = _controller;
				_controller = new WeakReference(value);
			}
		}

		#region Graph panel mouse and keyboard

		private void EhGraphPanel_KeyDown(object sender, KeyEventArgs e)
		{
			var guiController = Controller;
			if (null != guiController)
			{
				bool result = guiController.EhView_ProcessCmdKey(e);
				e.Handled = result;
			}
		}

		private PointD3D GetMousePosition(MouseEventArgs e)
		{
			var p = e.GetPosition(_d3dCanvas);
			return new PointD3D(p.X / _d3dCanvas.ActualWidth, 1 - p.Y / _d3dCanvas.ActualHeight, _d3dCanvas.ActualHeight / _d3dCanvas.ActualWidth);
		}

		private void EhGraphPanel_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			var mousePosition = e.GetPosition(this._d3dCanvas);
			double relX = mousePosition.X / _d3dCanvas.ActualWidth;
			double relY = 1 - mousePosition.Y / _d3dCanvas.ActualHeight;

			bool isSHIFTpressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
			bool isCTRLpressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
			bool isALTpressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);

			Controller?.EhView_GraphPanelMouseWheel(relX, relY, _d3dCanvas.ActualHeight / _d3dCanvas.ActualWidth, e.Delta, isSHIFTpressed, isCTRLpressed, isALTpressed);
		}

		private void EhGraphPanel_MouseMove(object sender, MouseEventArgs e)
		{
			var guiController = Controller;
			if (null != guiController)
				guiController.EhView_GraphPanelMouseMove(GetMousePosition(e), e);
		}

		private void EhGraphPanel_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var guiController = Controller;
			Keyboard.Focus(_guiCanvas);
			var pos = GetMousePosition(e);
			if (null != guiController)
			{
				guiController.EhView_GraphPanelMouseDown(pos, e);
				if (e.ClickCount >= 2 && e.LeftButton == MouseButtonState.Pressed)
					guiController.EhView_GraphPanelMouseDoubleClick(pos, e);
				else if (e.ClickCount == 1 && e.LeftButton == MouseButtonState.Pressed)
					guiController.EhView_GraphPanelMouseClick(pos, e);
			}
		}

		private void EhGraphPanel_MouseUp(object sender, MouseButtonEventArgs e)
		{
			var guiController = Controller;
			if (null != guiController)
				guiController.EhView_GraphPanelMouseUp(GetMousePosition(e), e);
		}

		#endregion Graph panel mouse and keyboard

		#region Graph panel size and visibility

		private void EhGraphPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var s = e.NewSize;

			if (!(s.Width > 0 && s.Height > 0))
				return;

			_cachedGraphSize_96thInch = new PointD2D(s.Width, s.Height);
			var screenResolution = Current.Gui.ScreenResolutionDpi;
			var graphSizePixels = screenResolution * _cachedGraphSize_96thInch / 96.0;
			_cachedGraphSize_Pixels = new System.Drawing.Size((int)graphSizePixels.X, (int)graphSizePixels.Y);

			_renderer.SetRenderSize(_cachedGraphSize_Pixels.Width, _cachedGraphSize_Pixels.Height);

			Controller?.EhView_GraphPanelSizeChanged(); // inform controller
		}

		private void EhIsGraphVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_isGraphVisible = (bool)e.NewValue;

			if (_isGraphVisible)
			{
			}
			else
			{
			}
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
				newbutton.IsChecked = System.Linq.Enumerable.SequenceEqual((int[])(node.Tag), currentLayerNumber);

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
			if (null != gc)
			{
				var pushedLayerNumber = (int[])((ButtonBase)sender).Tag;

				gc.EhView_CurrentLayerChoosen(pushedLayerNumber, false);
			}
		}

		private void EhLayerButton_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			var gc = Controller;
			if (null != gc)
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
			e.CanExecute = null != gc && gc.IsCmdCopyEnabled();
			e.Handled = true;
		}

		private void EhEnableCmdCut(object sender, CanExecuteRoutedEventArgs e)
		{
			var gc = Controller;
			e.CanExecute = null != gc && gc.IsCmdCutEnabled();
			e.Handled = true;
		}

		private void EhEnableCmdDelete(object sender, CanExecuteRoutedEventArgs e)
		{
			var gc = Controller;
			e.CanExecute = null != gc && gc.IsCmdDeleteEnabled();
			e.Handled = true;
		}

		private void EhEnableCmdPaste(object sender, CanExecuteRoutedEventArgs e)
		{
			var gc = Controller;
			e.CanExecute = null != gc && gc.IsCmdPasteEnabled();
			e.Handled = true;
		}

		private void EhCmdCopy(object sender, ExecutedRoutedEventArgs e)
		{
			var gc = Controller;
			if (null != gc)
			{
				gc.CopySelectedObjectsToClipboard();
				e.Handled = true;
			}
		}

		private void EhCmdCut(object sender, ExecutedRoutedEventArgs e)
		{
			var gc = Controller;
			if (null != gc)
			{
				gc.CutSelectedObjectsToClipboard();
				e.Handled = true;
			}
		}

		private void EhCmdDelete(object sender, ExecutedRoutedEventArgs e)
		{
			var gc = Controller;
			if (null != _controller)
			{
				gc.CmdDelete();
				e.Handled = true;
			}
		}

		private void EhCmdPaste(object sender, ExecutedRoutedEventArgs e)
		{
			var gc = Controller;
			if (null != _controller)
			{
				gc.PasteObjectsFromClipboard();
				e.Handled = true;
			}
		}

		#endregion Command bindings

		internal void SetPanelCursor(Cursor arrow)
		{
			_d3dCanvas.Cursor = arrow;
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
			Current.Gui.Execute(_renderer.TriggerRendering);
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
			return new D3D10GraphicsContext();
		}

		public void SetDrawing(IGraphicsContext3D drawing)
		{
			if (null == drawing)
				throw new ArgumentNullException();

			_scene.SetDrawing((D3D10GraphicsContext)drawing);
		}

		public IOverlayContext3D GetGraphicContextForMarkers()
		{
			return new D3D10OverlayContext();
		}

		public void SetMarkerGeometry(IOverlayContext3D markerGeometry)
		{
			if (null == markerGeometry)
				throw new ArgumentNullException();

			_scene.SetMarkerGeometry((D3D10OverlayContext)markerGeometry);
		}

		public IOverlayContext3D GetGraphicContextForOverlay()
		{
			return new D3D10OverlayContext();
		}

		public void SetOverlayGeometry(IOverlayContext3D overlayGeometry)
		{
			if (null == overlayGeometry)
				throw new ArgumentNullException();

			_scene.SetOverlayGeometry((D3D10OverlayContext)overlayGeometry);
		}
	}
}