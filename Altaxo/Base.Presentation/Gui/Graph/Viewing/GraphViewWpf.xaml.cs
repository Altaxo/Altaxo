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

using Altaxo.Collections;
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

namespace Altaxo.Gui.Graph.Viewing
{
	using Altaxo.Graph;

	/// <summary>
	/// Interaction logic for GraphView.xaml
	/// </summary>
	public partial class GraphViewWpf : UserControl, Altaxo.Gui.Graph.Viewing.IGraphView, IDisposable
	{
		[Browsable(false)]
		private int[] _cachedCurrentLayer = null;

		private WeakReference _controller = new WeakReference(null);

		private PointD2D _cachedGraphSize_96thInch;
		private System.Drawing.Size _cachedGraphSize_Pixels;

		/// <summary>Used for debugging the number of updates to the graph.</summary>
		private int _updateCount;

		static Altaxo.Collections.CachedObjectManagerByMaximumNumberOfItems<System.Drawing.Size, GdiToWpfBitmap> _gdiWpfBitmapManager = new CachedObjectManagerByMaximumNumberOfItems<System.Drawing.Size, GdiToWpfBitmap>(16);

		public GraphViewWpf()
		{
			InitializeComponent();
		}

		public virtual void Dispose()
		{
			_controller = new WeakReference(null);
		}

		private Altaxo.Gui.Graph.Viewing.GraphControllerWpf Controller
		{
			get
			{
				return (Altaxo.Gui.Graph.Viewing.GraphControllerWpf)_controller.Target;
			}
		}

		public void FocusOnGraphPanel()
		{
			bool result = _graphPanel.Focus();
			Keyboard.Focus(_graphPanel);
		}

		public void SetPanelCursor(Cursor cursor)
		{
			_graphPanel.Cursor = cursor;
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
			var p = e.GetPosition(_graphPanel);
			return new Altaxo.Graph.PointD2D(p.X, p.Y);
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
			Keyboard.Focus(_graphPanel);
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

		private void EhGraphPanel_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			var guiController = Controller;
			if (null != guiController)
				guiController.EhView_GraphPanelMouseWheel(GetMousePosition(e), e);
		}

		private void EhGraphPanel_KeyDown(object sender, KeyEventArgs e)
		{
			var guiController = Controller;
			if (null != guiController)
			{
				bool result = guiController.EhView_ProcessCmdKey(e);
				e.Handled = result;
			}
		}

		#region Altaxo.Gui.Graph.Viewing.IGraphView

		object Altaxo.Gui.Graph.Viewing.IGraphView.GuiInitiallyFocusedElement
		{
			get
			{
				// make sure that the returned element implements IInputElement
				var result = _graphPanel;
				System.Diagnostics.Debug.Assert(null != (result as System.Windows.IInputElement));
				return result;
			}
		}

		Altaxo.Gui.Graph.Viewing.IGraphViewEventSink Altaxo.Gui.Graph.Viewing.IGraphView.Controller
		{
			set
			{
				_controller = new WeakReference(value as Altaxo.Gui.Graph.Viewing.GraphController);
				((Altaxo.Gui.Graph.Viewing.IGraphView)this).InvalidateCachedGraphBitmapAndRepaint();
			}
		}

		private void EhGraphPanelSizeChanged(object sender, SizeChangedEventArgs e)
		{
			var s = e.NewSize;

			if (!(s.Width > 0 && s.Height > 0))
				return;

			_cachedGraphSize_96thInch = new PointD2D(s.Width, s.Height);
			var screenResolution = Current.Gui.ScreenResolutionDpi;
			var grap = screenResolution * _cachedGraphSize_96thInch / 96.0;
			_cachedGraphSize_Pixels = new System.Drawing.Size((int)grap.X, (int)grap.Y);

			if (null != Controller)
				Controller.EhView_GraphPanelSizeChanged(); // inform controller

			((Altaxo.Gui.Graph.Viewing.IGraphView)this).InvalidateCachedGraphBitmapAndRepaint();
		}

		/// <summary>
		/// Causes a complete redrawing of the graph. The cached graph bitmap will be marked as dirty and a repainting of the graph area is triggered with Gui render priority.
		/// Note: it is save to call this function from non-Gui threads.
		/// </summary>
		void Altaxo.Gui.Graph.Viewing.IGraphView.InvalidateCachedGraphBitmapAndRepaint()
		{
			var controller = Controller;
			if (null == controller)
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
						GdiToWpfBitmap bmp;
						if (!_gdiWpfBitmapManager.TryTake(size, out bmp))
							Current.Gui.Execute(() => bmp = new GdiToWpfBitmap(size.Width, size.Height));

						var grfx = GetGraphicsContextFromWpfGdiBitmap(bmp);
						controller.ScaleForPaintingGraphDocument(grfx);

						if (!graphDocument.IsDisposeInProgress)
							graphDocument.DoPaint(grfx, false);

						Current.Gui.Execute(() =>
						{
							_graphPanel.Source = bmp.WpfBitmapSource;
							grfx.Dispose();

							RenderOverlay(bmp);

							_gdiWpfBitmapManager.Add(bmp.GdiSize, bmp);
						});
					}
				}
				);

			/*
			//rendering in the renderTrigger Thread
			var grfx = BeginPaintingGraph();

			Controller.DoPaintUnbuffered(grfx, false);

			grfx.Dispose();

			Current.Gui.Execute(() =>
			{
				_graphPanel.Source = _wpfGdiBitmap.WpfBitmapSource;

				grfx = BeginPaintingGraph();
				Controller.ScaleForPaint(grfx, false);
				grfx.Clear(System.Drawing.Color.Transparent);
				Controller.DoPaintOverlay(grfx);
				grfx.Dispose();

				_graphOverlay.Source = _wpfGdiBitmap.WpfBitmapSource;
				//				EndPaintingGraph();
			});

	*/
		}

		/// <summary>
		/// Renders the overlay (the drawing that designates selected rectangles, handles and so on) immediately in the current thread.
		/// Attention: if there is no cached bitmap, a new bitmap is created, but this must be done in the Gui context, so this can lead to deadlocks.
		/// </summary>
		public void RenderOverlay()
		{
			var size = _cachedGraphSize_Pixels;

			if (size.Width > 1 && size.Height > 1)
			{
				GdiToWpfBitmap bmp;
				if (!_gdiWpfBitmapManager.TryTake(size, out bmp))
					Current.Gui.Execute(() => bmp = new GdiToWpfBitmap(size.Width, size.Height));
				try
				{
					RenderOverlay(bmp);
				}
				finally
				{
					_gdiWpfBitmapManager.Add(bmp.GdiSize, bmp);
				}
			}
			else
			{
				_graphOverlay.Source = null;
			}
		}

		/// <summary>
		/// Renders the overlay (the drawing that designates selected rectangles, handles and so on) immediately in the current thread.
		/// </summary>
		public void RenderOverlay(GdiToWpfBitmap bmp)
		{
			var controller = Controller;
			if (null != controller)
			{
				using (var grfx = GetGraphicsContextFromWpfGdiBitmap(bmp))
				{
					controller.DoPaintOverlay(grfx);
					_graphOverlay.Source = bmp.WpfBitmapSource;
				}
			}
			else
			{
				_graphOverlay.Source = null;
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

		public Altaxo.Graph.PointD2D GraphScrollPosition
		{
			get
			{
				return new Altaxo.Graph.PointD2D(_horizontalScrollBar.Value, _verticalScrollBar.Value);
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

		public Altaxo.Graph.PointD2D ViewportSizeInPoints
		{
			get
			{
				const double factor = 72.0 / 96.0;
				return new Altaxo.Graph.PointD2D(_cachedGraphSize_96thInch.X * factor, _cachedGraphSize_96thInch.Y * factor);
			}
		}

		#endregion Altaxo.Gui.Graph.Viewing.IGraphView

		private void EhHorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			if (e.ScrollEventType == ScrollEventType.ThumbTrack)
				return;

			var gc = Controller;
			if (null != gc)
				gc.EhView_Scroll();
		}

		private void EhVerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			if (e.ScrollEventType == ScrollEventType.ThumbTrack)
				return;

			var gc = Controller;
			if (null != gc)
				gc.EhView_Scroll();
		}

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
	}
}