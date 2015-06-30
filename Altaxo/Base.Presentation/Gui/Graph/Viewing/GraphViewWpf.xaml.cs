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

		private static Altaxo.Collections.CachedObjectManagerByMaximumNumberOfItems<System.Drawing.Size, GdiToWpfBitmap> _gdiWpfBitmapManager = new CachedObjectManagerByMaximumNumberOfItems<System.Drawing.Size, GdiToWpfBitmap>(16);

		private static DrawingImage _busyWithRenderingDrawing = (DrawingImage)new DrawingImage(new GlyphRunDrawing(Brushes.Lavender, BuildGlyphRun(" Busy with rendering ... "))).GetAsFrozen();

		private volatile bool _isGraphVisible;

		private volatile bool _isGraphUpToDate;

		private CachedGraphImage _cachedGraphImage;

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
			var tileBrush = new DrawingBrush();
			tileBrush.Drawing = glyphRunDrawing;
			tileBrush.Viewbox = new Rect(0, 0, glyphRunDrawing.Bounds.Right * 1.1, glyphRunDrawing.Bounds.Bottom * 1.5);
			tileBrush.ViewboxUnits = BrushMappingMode.Absolute;
			tileBrush.Viewport = glyphRunDrawing.Bounds;
			tileBrush.ViewportUnits = BrushMappingMode.Absolute;
			tileBrush.TileMode = TileMode.Tile;
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
			bool result = _guiCanvas.Focus();
			Keyboard.Focus(_guiCanvas);
		}

		public void SetPanelCursor(Cursor cursor)
		{
			_guiCanvas.Cursor = cursor;
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

		private void EhGraphPanel_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			var guiController = Controller;
			if (null != guiController)
			{
				guiController.EhView_GraphPanelMouseWheel(GetMousePosition(e), e);
				ShowCachedGraphImage();
			}
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
				var result = _graphImage;
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
			_guiCanvas.Clip = new RectangleGeometry(new Rect(0, 0, s.Width, s.Height));

			if (!(s.Width > 0 && s.Height > 0))
				return;

			_cachedGraphSize_96thInch = new PointD2D(s.Width, s.Height);
			var screenResolution = Current.Gui.ScreenResolutionDpi;
			var graphSizePixels = screenResolution * _cachedGraphSize_96thInch / 96.0;
			_cachedGraphSize_Pixels = new System.Drawing.Size((int)graphSizePixels.X, (int)graphSizePixels.Y);

			if (null != Controller)
				Controller.EhView_GraphPanelSizeChanged(); // inform controller

			ShowCachedGraphImage();

			_isGraphUpToDate = false;
			if (_isGraphVisible)
				((Altaxo.Gui.Graph.Viewing.IGraphView)this).InvalidateCachedGraphBitmapAndRepaint();
		}

		private void EhIsGraphVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_isGraphVisible = (bool)e.NewValue;

			if (_isGraphVisible)
			{
				var ctrl = Controller;
				if (null != ctrl)
					_guiCanvas.Background = new SolidColorBrush(GuiHelper.ToWpf(ctrl.NonPageAreaColor));

				if (_isGraphUpToDate && true == ShowCachedGraphImage())
				{
				}
				else
				{
					_isGraphUpToDate = false;
					_graphImage.Source = _busyWithRenderingDrawing;

					((Altaxo.Gui.Graph.Viewing.IGraphView)this).InvalidateCachedGraphBitmapAndRepaint();
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
			if (null != _cachedGraphImage && null != controller)
			{
				var imgSrc = _cachedGraphImage.CachedGraphImageSource;
				if (null != imgSrc)
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
		void Altaxo.Gui.Graph.Viewing.IGraphView.InvalidateCachedGraphBitmapAndRepaint()
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
						var cachedGraphImage = new CachedGraphImage { ZoomFactor = controller.ZoomFactor, ViewPortsUpperLeftCornerInGraphCoordinates = controller.PositionOfViewportsUpperLeftCornerInGraphCoordinates, Size = _cachedGraphSize_96thInch, BitmapSize_Pixel = size };

						if (!graphDocument.IsDisposeInProgress)
							graphDocument.Paint(grfx, false);

						Current.Gui.Execute(() =>
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
		internal void EhRenderOverlayTriggered()
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
				null != controller &&
				controller.IsOverlayPaintingRequired &&
				null != cachedGraphImage &&
				cachedGraphImage.BitmapSize_Pixel.Width > 1 &&
				cachedGraphImage.BitmapSize_Pixel.Height > 1)
			{
				var size = cachedGraphImage.BitmapSize_Pixel;

				GdiToWpfBitmap bmp;
				if (!_gdiWpfBitmapManager.TryTake(size, out bmp))
					Current.Gui.Execute(() => bmp = new GdiToWpfBitmap(size.Width, size.Height));

				try
				{
					overlay = GetImageSourceByRenderingOverlay(controller, bmp, cachedGraphImage);
				}
				finally
				{
					_gdiWpfBitmapManager.Add(bmp.GdiSize, bmp);
				}
			}
			if (null != _cachedGraphImage)
				_cachedGraphImage.CachedOverlayImageSource = overlay;
			_graphOverlay.Source = overlay;
		}

		/// <summary>
		/// Renders the overlay (the drawing that designates selected rectangles, handles and so on) immediately in the current thread.
		/// Attention: if there is no cached bitmap, a new bitmap is created, but this must be done in the Gui context, so this can lead to deadlocks.
		/// </summary>
		private static ImageSource GetImageSourceByRenderingOverlay(GraphControllerWpf controller, GdiToWpfBitmap bmp, CachedGraphImage cachedGraphImage)
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
			var gc = Controller;
			if (null != gc)
			{
				gc.EhView_Scroll();
				ShowCachedGraphImage();
			}
		}

		private void EhVerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			var gc = Controller;
			if (null != gc)
			{
				gc.EhView_Scroll();
				ShowCachedGraphImage();
			}
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

		public static GlyphRun BuildGlyphRun(string text)
		{
			double fontSize = 50;
			GlyphRun glyphs = null;

			Typeface font = new Typeface("Arial");
			GlyphTypeface glyphFace;
			if (font.TryGetGlyphTypeface(out glyphFace))
			{
				glyphs = new GlyphRun();
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
	}
}