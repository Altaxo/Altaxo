#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Drawing;
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

namespace Altaxo.Gui.Drawing.ColorManagement
{
	/// <summary>
	/// Interaction logic for NamedColorControl.xaml
	/// </summary>
	public partial class NamedColorControl : UserControl, INamedColorView
	{
		public event Action<object> SubViewChanged;

		public NamedColorControl()
		{
			InitializeComponent();
		}

		public void InitializeSubViews(IEnumerable<Tuple<string, object>> tabsNamesAndViews)
		{
			_guiTabControl.Items.Clear();

			foreach (var item in tabsNamesAndViews)
			{
				var tab = new TabItem() { Header = item.Item1, Content = item.Item2 };
				_guiTabControl.Items.Add(tab);
			}
		}

		private void EhAlphaValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
		}

		public void SetOldColor(AxoColor oldColor)
		{
			_guiOldColorRectangle.Fill = new SolidColorBrush(GuiHelper.ToWpf(oldColor));
		}

		public void SetNewColor(AxoColor newColor)
		{
			_guiNewColorRectangle.Fill = new SolidColorBrush(GuiHelper.ToWpf(newColor));
		}

		public string ColorName
		{
			get
			{
				return _guiColorName.Text;
			}
			set
			{
				_guiColorName.Text = value;
			}
		}

		private void EhTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 1 && e.AddedItems[0] is TabItem newTabItem)
			{
				SubViewChanged?.Invoke(newTabItem.Content);
			}
		}
	}
}