using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Graph.Viewing
{
	/// <summary>
	/// Interaction logic for GraphView.xaml
	/// </summary>
	public partial class GraphViewWpf : UserControl, Altaxo.Gui.Graph.Viewing.IGraphView
	{
		[Browsable(false)]
		private int _cachedCurrentLayer = -1;

		private Altaxo.Gui.Graph.Viewing.GraphController _controller;

		private Altaxo.Gui.Graph.Viewing.PresentationGraphController _guiController;

		GdiToWpfBitmap _wpfGdiBitmap = new GdiToWpfBitmap(100,100);

		public GraphViewWpf()
		{
			InitializeComponent();
			_graphPanel.Source = _wpfGdiBitmap.WpfBitmap;
			_guiController = new PresentationGraphController(this);
		}

		public Altaxo.Gui.Graph.Viewing.GraphController GC
		{
			get
			{
				return _controller;
			}
		}

		public Altaxo.Gui.Graph.Viewing.PresentationGraphController GuiController
		{
			get
			{
				return _guiController;
			}
		}

		public Altaxo.Graph.Gdi.GraphDocument Doc
		{
			get
			{
				return _controller != null ? _controller.Doc : null;
			}
		}

		public Altaxo.Graph.Gdi.XYPlotLayer ActiveLayer
		{
			get
			{
				return _controller.ActiveLayer;
			}
		}

		public void SetGraphToolFromInternal(Altaxo.Gui.Graph.Viewing.GraphToolType value)
		{

			_guiController.GraphTool = value;
			bool result = _graphPanel.Focus();
			Keyboard.Focus(_graphPanel);
			if (null != _controller)
				_controller.EhView_CurrentGraphToolChanged();
		}

		public void SetActiveLayerFromInternal(int layerNumber)
		{
			_controller.EhView_CurrentLayerChoosen(layerNumber, false);
		}

		public void SetPanelCursor(Cursor cursor)
		{
			_graphPanel.Cursor = cursor;
		}




		public System.Drawing.Graphics CreateGraphGraphics()
		{
			var result = _wpfGdiBitmap.GdiGraphics;
			result.ResetTransform();
			result.PageScale = 1;
			result.PageUnit = System.Drawing.GraphicsUnit.Pixel;
			return result;
		}

		public void InvalidateGraph()
		{
			// TODO (Wpf) -> start a background thread which draws the image
			// for the time being, we do the work directly
			if (null != _guiController)
				_guiController.RefreshGraph();

		
		}

		/// <summary>
		/// Called when the graph image has changed.
		/// </summary>
		public void EhGraphImageChanged()
		{
			_wpfGdiBitmap.WpfBitmap.Invalidate();
		}


	

		public void OnViewSelection()
		{

		}

		public void OnViewDeselection()
		{
			/* TODO (Wpf)
			if (null != _graphToolsToolBar)
				_graphToolsToolBar.Parent = null;
			*/
		}

		/// <summary>
		/// Gets the pixel dimensions of the graph bitmap.
		/// </summary>
		public System.Drawing.Size GraphSize
		{
			get
			{
				return _wpfGdiBitmap.GdiBitmap.Size;
			}
		}

		Altaxo.Graph.PointD2D GetMousePosition(MouseEventArgs e)
		{
			var p = e.GetPosition(_graphPanel);
			return new Altaxo.Graph.PointD2D(p.X, p.Y);
		}

		private void EhGraphPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (null != _guiController)
				_guiController.EhView_GraphPanelMouseMove(GetMousePosition(e),e);
		}

		private void EhGraphPanel_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Keyboard.Focus(_graphPanel);
			var pos = GetMousePosition(e);
			if (null != _guiController)
			{
				_guiController.EhView_GraphPanelMouseDown(pos, e);
				if (e.ClickCount >= 2 && e.LeftButton == MouseButtonState.Pressed)
					_guiController.EhView_GraphPanelMouseDoubleClick(pos, e);
				else if (e.ClickCount == 1 && e.LeftButton == MouseButtonState.Pressed)
					_guiController.EhView_GraphPanelMouseClick(pos, e);
			}
		}

		private void EhGraphPanel_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (null != _guiController)
				_guiController.EhView_GraphPanelMouseUp(GetMousePosition(e),e);
		}

		private void EhGraphPanel_KeyDown(object sender, KeyEventArgs e)
		{
			if (null != _guiController)
			{
				bool result = _guiController.EhView_ProcessCmdKey(e);
				e.Handled = result;
			}
		}

		private void EhGraphPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (null != _controller)
			{
				_wpfGdiBitmap.Resize((int)_graphPanel.ActualWidth, (int)_graphPanel.ActualHeight);
				_wpfGdiBitmap.GdiBitmap.SetResolution(96,96);
				_graphPanel.Source = _wpfGdiBitmap.WpfBitmap;
				_controller.EhView_GraphPanelSizeChanged();
			}
		}


		#region  Altaxo.Gui.Graph.Viewing.IGraphView

		object Altaxo.Gui.Graph.Viewing.IGraphView.GuiInitiallyFocusedElement
		{
			get
			{
				return _graphPanel;
			}
		}

		Altaxo.Gui.Graph.Viewing.IGraphViewEventSink Altaxo.Gui.Graph.Viewing.IGraphView.Controller
		{
			set
			{
				_controller = value as Altaxo.Gui.Graph.Viewing.GraphController; 
			}
		}

		void Altaxo.Gui.Graph.Viewing.IGraphView.RefreshGraph()
		{
			if (null != _guiController)
			{
				_guiController.RefreshGraph();
			}
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
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

		public System.Drawing.PointF GraphScrollPosition
		{
			get
			{
				return new System.Drawing.PointF((float)(_horizontalScrollBar.Value / _horizontalScrollBar.Maximum), (float)(_verticalScrollBar.Value / _verticalScrollBar.Maximum));
			}
			set
			{
				var controller = _guiController;
				_guiController = null; // suppress scrollbar events

				this._horizontalScrollBar.Value = (int)(value.X * _horizontalScrollBar.Maximum);
				this._verticalScrollBar.Value = (int)(value.Y * _verticalScrollBar.Maximum);

				_guiController = controller;
			}
		}

		public int NumberOfLayers
		{
			set
			{
				{
					int nNumButtons = _layerToolBar.Children.Count;

					if (value > nNumButtons)
					{
						for (int i = nNumButtons; i < value; i++)
						{

							var newbutton = new ToggleButton() { Content = i.ToString(), Tag = i };
							newbutton.Click += new RoutedEventHandler(EhLayerButton_Click);
							newbutton.ContextMenuOpening += new ContextMenuEventHandler(EhLayerButton_ContextMenuOpening);
							_layerToolBar.Children.Add(newbutton);
						}
					}
					else if (nNumButtons > value)
					{
						for (int i = nNumButtons - 1; i >= value; i--)
							_layerToolBar.Children.RemoveAt(i);
					}

					// now press the currently active layer button
					for (int i = 0; i < _layerToolBar.Children.Count; i++)
					{
						var button = (ToggleButton)_layerToolBar.Children[i];
						button.IsChecked = (i == _cachedCurrentLayer);
					}
				}
			}
		}

		void EhLayerButton_Click(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
			{
				int pushedLayerNumber = (int)((ButtonBase)sender).Tag;

				_controller.EhView_CurrentLayerChoosen(pushedLayerNumber, false);
			}
		}

		void EhLayerButton_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			if (null != _guiController)
			{
				int i = (int)((ToggleButton)sender).Tag;
				_controller.EhView_ShowDataContextMenu(i, this, new System.Drawing.Point((int)e.CursorLeft, (int)e.CursorTop));
			}
		}

		public int CurrentLayer
		{
			set
			{
				_cachedCurrentLayer = value;

				for (int i = 0; i < _layerToolBar.Children.Count; i++)
					((ToggleButton)_layerToolBar.Children[i]).IsChecked = (i == _cachedCurrentLayer);
			}
		}

		public System.Drawing.SizeF ViewportSizeInInch
		{
			get 
			{
				return new System.Drawing.SizeF((float)(_graphPanel.ActualWidth / 96.0), (float)( _graphPanel.ActualHeight / 96.0));
			}
		}

		public IList<Altaxo.Graph.Gdi.IHitTestObject> SelectedObjects
		{
			get 
			{
				return _guiController.SelectedObjects;
			} 
		}

		Altaxo.Gui.Graph.Viewing.GraphToolType Altaxo.Gui.Graph.Viewing.IGraphView.GraphTool
		{
			get
			{
				return _guiController.GraphTool;
			}
			set
			{
				_guiController.GraphTool = value;
			}
		}

		#endregion

		private void EhHorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			if (e.ScrollEventType == ScrollEventType.ThumbTrack)
				return;

			if (null != _controller)
				_controller.EhView_Scroll();
		}

		private void EhVerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			if (e.ScrollEventType == ScrollEventType.ThumbTrack)
				return;

			if (null != _controller)
				_controller.EhView_Scroll();
		}

		private void EhEnableCmdCopy(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = null != _controller && _controller.IsCmdCopyEnabled();
			e.Handled = true;
		}

		private void EhEnableCmdCut(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = null != _controller && _controller.IsCmdCutEnabled();
			e.Handled = true;
		}

		private void EhEnableCmdDelete(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = null != _controller && _controller.IsCmdDeleteEnabled();
			e.Handled = true;
		}

		private void EhEnableCmdPaste(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = null != _controller && _controller.IsCmdPasteEnabled();
			e.Handled = true;
		}

		private void EhCmdCopy(object sender, ExecutedRoutedEventArgs e)
		{
			if (null != _controller)
			{
				_controller.CopySelectedObjectsToClipboard();
				e.Handled = true;
			}
		}

		private void EhCmdCut(object sender, ExecutedRoutedEventArgs e)
		{
			if (null != _controller)
			{
				_controller.CutSelectedObjectsToClipboard();
				e.Handled = true;
			}
		}

		private void EhCmdDelete(object sender, ExecutedRoutedEventArgs e)
		{
			if (null != _controller)
			{
				_controller.CmdDelete();
				e.Handled = true;
			}
		}

		private void EhCmdPaste(object sender, ExecutedRoutedEventArgs e)
		{
			if (null != _controller)
			{
				_controller.PasteObjectsFromClipboard();
				e.Handled = true;
			}
		}
	}
}
