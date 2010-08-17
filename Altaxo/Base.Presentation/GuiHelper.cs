using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Altaxo;
using Altaxo.Collections;

namespace Altaxo.Gui
{
	public class GuiHelper
	{
		#region Combobox

		public static void Initialize(ComboBox view, SelectableListNodeList data)
		{
			view.ItemsSource = null;
			view.ItemsSource = data;
			view.SelectedIndex = data.FindIndex(n => n.Selected);
		}

		public static void SynchronizeSelectionFromGui(ComboBox view)
		{
			foreach (ISelectableItem it in view.ItemsSource)
				it.Selected = false;

			if (null != view.SelectedItem)
				((ISelectableItem)view.SelectedItem).Selected = true;
		}
		#endregion

		#region ListBox

		public static void Initialize(ListBox view,SelectableListNodeList data)
		{
			view.ItemsSource = null;
			view.ItemsSource = data;
			if (view.SelectionMode == SelectionMode.Single)
			{
				view.SelectedIndex = data.FindIndex(n => n.Selected);
			}
			else
			{
				foreach (var n in data)
					if (n.Selected)
						view.SelectedItems.Add(n);
			}
		}

		public static void SynchronizeSelectionFromGui(ListBox view)
		{
			foreach (ISelectableItem it in view.ItemsSource)
				it.Selected = false;

			if (null != view.SelectedItem)
				((ISelectableItem)view.SelectedItem).Selected = true;
		}

		#endregion

		#region ListView


		public static void Initialize(ListView view, SelectableListNodeList data)
		{
			view.ItemsSource = null;
			view.ItemsSource = data;

			if (view.SelectionMode == SelectionMode.Single)
			{
				view.SelectedIndex = data.FindIndex(n => n.Selected);
			}
			else
			{
				foreach (var n in data)
					if (n.Selected)
						view.SelectedItems.Add(n);
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
					it.Selected = false;
			}

			foreach (ISelectableItem it in listView.SelectedItems)
				it.Selected = true;
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


		#endregion

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

		#endregion
	}
}
