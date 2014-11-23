#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

using Altaxo;
using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Altaxo.Gui
{
	public static class GuiHelper
	{
		#region Combobox

		public static void Initialize(ComboBox view, SelectableListNodeList data)
		{
			int idx = data.FirstSelectedNodeIndex; // Note: the selected index must be determined _before_ the data are bound to the box (otherwise when a binding is in place, it can happen that the selection is resetted)

			if (view.ItemsSource != data)
			{
				//view.ItemsSource = null;
				view.ItemsSource = data;
			}

			if (idx >= 0)
				view.SelectedItem = data[idx];
		}

		public static void SynchronizeSelectionFromGui(ComboBox view)
		{
			foreach (ISelectableItem it in view.ItemsSource)
				it.IsSelected = false;

			if (null != view.SelectedItem)
				((ISelectableItem)view.SelectedItem).IsSelected = true;
		}

		#endregion Combobox

		#region ListBox

		public static void Initialize(ListBox view, SelectableListNodeList data)
		{
			view.ItemsSource = null;
			view.ItemsSource = data;
			if (view.SelectionMode == SelectionMode.Single)
			{
				view.SelectedIndex = data.FirstSelectedNodeIndex;
			}
			else
			{
				foreach (var n in data)
					if (n.IsSelected)
						view.SelectedItems.Add(n);
			}
		}

		/// <summary>
		/// Sets the items of a list box with <see cref="CheckableSelectableListNode"/> items. We presume here that the ListBox has an appropriate DataTemplate, thus only the ItemsSource
		/// property of the ListBox is set with the data.
		/// </summary>
		/// <param name="view">ListBox to set.</param>
		/// <param name="data">The data to set for the ListBox.</param>
		public static void Initialize(ListBox view, CheckableSelectableListNodeList data)
		{
			view.ItemsSource = data;
		}

		public static void SynchronizeSelectionFromGui(ListBox view)
		{
			if (view.ItemsSource != null)
			{
				foreach (ISelectableItem it in view.ItemsSource)
					it.IsSelected = false;
			}

			foreach (ISelectableItem it in view.SelectedItems)
				it.IsSelected = true;
		}

		#endregion ListBox

		#region ListView

		public static void Initialize(ListView view, SelectableListNodeList data)
		{
			view.ItemsSource = null;
			view.ItemsSource = data;

			if (view.SelectionMode == SelectionMode.Single)
			{
				view.SelectedIndex = data.FirstSelectedNodeIndex;
			}
			else
			{
				if (data != null)
				{
					foreach (var n in data)
						if (n.IsSelected)
							view.SelectedItems.Add(n);
				}
			}
		}

		public static void Refresh(ListView view)
		{
			var h = view.ItemsSource;
			view.ItemsSource = null;
			view.ItemsSource = h;
		}

		public static void SynchronizeSelectionFromGui(ListView listView)
		{
			if (null != listView.ItemsSource)
			{
				foreach (ISelectableItem it in listView.ItemsSource)
					it.IsSelected = false;
			}

			foreach (ISelectableItem it in listView.SelectedItems)
				it.IsSelected = true;
		}

		public static int[] GetColumnWidths(ListView listView)
		{
			GridView gv = (GridView)listView.View;

			int[] ret = new int[gv.Columns.Count];
			for (int i = 0; i < ret.Length; i++)
				ret[i] = (int)gv.Columns[i].ActualWidth;
			return ret;
		}

		public static void SetColumnWidths(ListView listView, int[] widths)
		{
			GridView gv = (GridView)listView.View;

			int len = Math.Min(widths.Length, gv.Columns.Count);
			for (int i = 0; i < len; i++)
				gv.Columns[i].Width = widths[i];
		}

		/// <summary>
		/// Sets the column names of a list view and set up the binding to items that derive from <see cref="Altaxo.Collections.ListNode"/>
		/// </summary>
		/// <param name="listView">The ListView where the columns to set.</param>
		/// <param name="columnHeaders">The text of the column headers.</param>
		/// <remarks>The first column is bind to the property "Text" of the items, the next columns to the property "Text1", "Text2", and so on.</remarks>
		public static void InitializeListViewColumnsAndBindToListNode(ListView listView, string[] columnHeaders)
		{
			if (null == (listView.View as GridView))
				listView.View = new GridView();

			var grid = listView.View as GridView;

			grid.Columns.Clear();

			int colNo = -1;
			foreach (var colName in columnHeaders)
			{
				++colNo;

				var gvCol = new GridViewColumn() { Header = colName };
				var binding = new Binding(colNo == 0 ? "Text " : "Text" + colNo.ToString());
				gvCol.DisplayMemberBinding = binding;
				grid.Columns.Add(gvCol);
			}
		}

		#endregion ListView

		#region TabControl

		public static void Initialize(TabControl view, SelectableListNodeList data)
		{
			int idx = data.FirstSelectedNodeIndex; // Note: the selected index must be determined _before_ the data are bound to the box (otherwise when a binding is in place, it can happen that the selection is resetted)

			if (view.ItemsSource != data)
			{
				//view.ItemsSource = null;
				view.ItemsSource = data;
			}

			if (idx >= 0)
				view.SelectedItem = data[idx];
		}

		public static void SynchronizeSelectionFromGui(TabControl view)
		{
			foreach (ISelectableItem it in view.ItemsSource)
				it.IsSelected = false;

			if (null != view.SelectedItem)
				((ISelectableItem)view.SelectedItem).IsSelected = true;
		}

		#endregion TabControl

		#region Mouse

		public static Altaxo.Gui.AltaxoMouseButtons GetMouseState(MouseDevice mouse)
		{
			var result = Altaxo.Gui.AltaxoMouseButtons.None;
			if (MouseButtonState.Pressed == mouse.LeftButton) result |= AltaxoMouseButtons.Left;
			if (MouseButtonState.Pressed == mouse.MiddleButton) result |= AltaxoMouseButtons.Middle;
			if (MouseButtonState.Pressed == mouse.RightButton) result |= AltaxoMouseButtons.Right;
			if (MouseButtonState.Pressed == mouse.XButton1) result |= AltaxoMouseButtons.XButton1;
			if (MouseButtonState.Pressed == mouse.XButton2) result |= AltaxoMouseButtons.XButton2;

			return result;
		}

		public static Altaxo.Gui.AltaxoMouseButtons GetMouseState(MouseButtonEventArgs mouse)
		{
			var result = Altaxo.Gui.AltaxoMouseButtons.None;
			if (MouseButtonState.Pressed == mouse.LeftButton) result |= AltaxoMouseButtons.Left;
			if (MouseButtonState.Pressed == mouse.MiddleButton) result |= AltaxoMouseButtons.Middle;
			if (MouseButtonState.Pressed == mouse.RightButton) result |= AltaxoMouseButtons.Right;
			if (MouseButtonState.Pressed == mouse.XButton1) result |= AltaxoMouseButtons.XButton1;
			if (MouseButtonState.Pressed == mouse.XButton2) result |= AltaxoMouseButtons.XButton2;

			return result;
		}

		#endregion Mouse

		#region Brush and Pen

		public static System.Windows.Media.Brush ToWpf(this Altaxo.Graph.Gdi.BrushX brushx)
		{
			System.Windows.Media.Color c = ToWpf(brushx.Color);
			var result = new System.Windows.Media.SolidColorBrush(c);
			return result;
		}

		public static System.Windows.Media.Pen ToWpf(this Altaxo.Graph.Gdi.PenX penx)
		{
			System.Windows.Media.Color c = ToWpf(penx.Color);
			var result = new System.Windows.Media.Pen(new System.Windows.Media.SolidColorBrush(c), penx.Width);
			return result;
		}

		public static System.Windows.Media.Color ToWpf(this System.Drawing.Color c)
		{
			return Color.FromArgb(c.A, c.R, c.G, c.B);
		}

		public static Altaxo.Graph.AxoColor ToAxo(this System.Drawing.Color c)
		{
			return Altaxo.Graph.AxoColor.FromArgb(c.A, c.R, c.G, c.B);
		}

		public static System.Drawing.Color ToGdi(this Altaxo.Graph.AxoColor c)
		{
			return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
		}

		public static System.Windows.Media.Color ToWpf(this Altaxo.Graph.AxoColor c)
		{
			if (c.IsFromArgb)
				return Color.FromArgb(c.A, c.R, c.G, c.B);
			else
				return Color.FromScRgb(c.ScA, c.ScR, c.ScG, c.ScB);
		}

		public static System.Windows.Media.Color ToWpf(this Altaxo.Graph.NamedColor c)
		{
			return ToWpf(c.Color);
		}

		public static Altaxo.Graph.AxoColor ToAxo(this System.Windows.Media.Color c)
		{
			return Altaxo.Graph.AxoColor.FromScRgb(c.ScA, c.ScR, c.ScG, c.ScB);
		}

		public static System.Drawing.Color ToSysDraw(this System.Windows.Media.Color c)
		{
			return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
		}

		#endregion Brush and Pen

		#region Graphics primitives

		#region Point

		public static Point ToWpf(this Altaxo.Graph.PointD2D pt)
		{
			return new Point(pt.X, pt.Y);
		}

		public static Point ToWpf(this System.Drawing.PointF pt)
		{
			return new Point(pt.X, pt.Y);
		}

		public static Point ToWpf(this System.Drawing.Point pt)
		{
			return new Point(pt.X, pt.Y);
		}

		public static Altaxo.Graph.PointD2D ToAltaxo(this Point pt)
		{
			return new Altaxo.Graph.PointD2D(pt.X, pt.Y);
		}

		public static System.Drawing.PointF ToSysDraw(this Point pt)
		{
			return new System.Drawing.PointF((float)pt.X, (float)pt.Y);
		}

		public static System.Drawing.Point ToSysDrawInt(this Point pt)
		{
			return new System.Drawing.Point((int)pt.X, (int)pt.Y);
		}

		#endregion Point

		#region Rectangle

		public static Rect ToWpf(this Altaxo.Graph.RectangleD rect)
		{
			return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static Rect ToWpf(this System.Drawing.RectangleF rect)
		{
			return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static Rect ToWpf(this System.Drawing.Rectangle rect)
		{
			return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static Altaxo.Graph.RectangleD ToAltaxo(this Rect rect)
		{
			return new Altaxo.Graph.RectangleD(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static System.Drawing.RectangleF ToSysDraw(this Rect rect)
		{
			return new System.Drawing.RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
		}

		public static System.Drawing.Rectangle ToSysDrawInt(this Rect rect)
		{
			return new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}

		#endregion Rectangle

		#endregion Graphics primitives

		#region Panel Helpers

		public static void InitializeChoicePanel<TChoiceGuiElement>(Panel panel, SelectableListNodeList choices) where TChoiceGuiElement : ToggleButton, new()
		{
			panel.Tag = choices;
			panel.Children.Clear();
			foreach (var choice in choices)
			{
				var rb = new TChoiceGuiElement();
				rb.Content = choice.Text;
				rb.Tag = choice;
				rb.SetBinding(ToggleButton.IsCheckedProperty, new System.Windows.Data.Binding("IsSelected") { Source = choice, Mode = System.Windows.Data.BindingMode.TwoWay });
				panel.Children.Add(rb);
			}
		}

		#endregion Panel Helpers

		#region Image Proxy converters

		/// <summary>Converts <see cref="Altaxo.Graph.ImageProxy"/> instances to Wpf <see cref="ImageSource"/> instances.</summary>
		/// <param name="proxy">The proxy.</param>
		/// <returns></returns>
		public static ImageSource ToWpf(Altaxo.Graph.ImageProxy proxy)
		{
			var stream = proxy.GetContentStream();
			var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(stream, System.Windows.Media.Imaging.BitmapCreateOptions.None, System.Windows.Media.Imaging.BitmapCacheOption.Default);
			return decoder.Frames[0];
		}

		/// <summary>Converts a brush to a Wpf <see cref="ImageSource"/> instance.</summary>
		/// <param name="brush">The brush to convert.</param>
		/// <param name="xsize">The horizontal number of pixels of the image.</param>
		/// <param name="ysize">The vertical number of pixels of the image.</param>
		/// <returns>An image that represents the brush.</returns>
		public static ImageSource ToWpf(Altaxo.Graph.Gdi.BrushX brush, int xsize, int ysize)
		{
			using (var bmp = new System.Drawing.Bitmap(xsize, ysize, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
			{
				using (var g = System.Drawing.Graphics.FromImage(bmp))
				{
					brush.SetEnvironment(new Altaxo.Graph.RectangleD(0, 0, xsize, ysize), 96);
					g.FillRectangle(brush, 0, 0, xsize, ysize);
				}
				var stream = Altaxo.Graph.ImageProxy.ImageToStream(bmp, System.Drawing.Imaging.ImageFormat.Png);
				var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(stream, System.Windows.Media.Imaging.BitmapCreateOptions.None, System.Windows.Media.Imaging.BitmapCacheOption.Default);
				return decoder.Frames[0];
			}
		}

		#endregion Image Proxy converters

		#region Image from System.Drawing to WPF

		public static System.Windows.Media.Imaging.BitmapSource ToWpf(this System.Drawing.Bitmap bitmap)
		{
			using (var stream = new System.IO.MemoryStream())
			{
				var imgFormat = bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ? System.Drawing.Imaging.ImageFormat.Bmp : System.Drawing.Imaging.ImageFormat.Png;
				bitmap.Save(stream, imgFormat);

				stream.Position = 0;
				var result = new System.Windows.Media.Imaging.BitmapImage();
				result.BeginInit();
				// According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
				// Force the bitmap to load right now so we can dispose the stream.
				result.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
				result.StreamSource = stream;
				result.EndInit();
				result.Freeze();
				return result;
			}
		}

		#endregion Image from System.Drawing to WPF

		#region Logical tree

		/// <summary>
		/// Gets the object itself or the first recursive parent of the object that has the type T.
		/// </summary>
		/// <typeparam name="T">The type of the object to find.</typeparam>
		/// <param name="child">The object where the search starts.</param>
		/// <returns>The object itself or the first recursive parent of the object which is of type T. If no such parent exist, <c>null</c> is returned.</returns>
		public static T GetLogicalParentOfType<T>(DependencyObject child) where T : DependencyObject
		{
			if (null == child)
				return null;

			if (child is T)
				return (T)child;

			return GetLogicalParentOfType<T>(LogicalTreeHelper.GetParent(child));
		}

		#endregion Logical tree

		#region Drag-Drop

		public static DragDropEffects ConvertCopyMoveToDragDropEffect(bool copy, bool move)
		{
			var result = DragDropEffects.None;
			if (copy) result |= DragDropEffects.Copy;
			if (move) result |= DragDropEffects.Move;
			return result;
		}

		public static void ConvertDragDropEffectToCopyMove(DragDropEffects effects, out bool copy, out bool move)
		{
			copy = effects.HasFlag(DragDropEffects.Copy);
			move = effects.HasFlag(DragDropEffects.Move);
		}

		public static System.Windows.IDataObject ToWpf(Altaxo.Serialization.Clipboard.IDataObject dao)
		{
			return DataObjectAdapterAltaxoToWpf.FromAltaxoDataObject(dao);
		}

		public static Altaxo.Serialization.Clipboard.IDataObject ToAltaxo(System.Windows.IDataObject dao)
		{
			return DataObjectAdapterWpfToAltaxo.FromWpfDataObject(dao);
		}

		#endregion Drag-Drop
	}
}