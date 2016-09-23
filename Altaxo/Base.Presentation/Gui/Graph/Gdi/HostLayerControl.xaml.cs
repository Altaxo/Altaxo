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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Gdi
{
	/// <summary>
	/// Interaction logic for LayerControl.xaml
	/// </summary>
	public partial class HostLayerControl : UserControl, IHostLayerView
	{
		public event Action<string> PageChanged;

		public event System.ComponentModel.CancelEventHandler TabValidating;

		public HostLayerControl()
		{
			InitializeComponent();
		}

		#region ILayerView Members

		public void AddTab(string name, string text)
		{
			var tc = new TabItem();
			tc.Name = name;
			tc.Header = text;
			this._tabCtrl.Items.Add(tc);
		}

		public object CurrentContent
		{
			get
			{
				int sel = _tabCtrl.SelectedIndex;
				var tp = (TabItem)_tabCtrl.Items[sel];
				return tp.Content;
			}
			set
			{
				int sel = _tabCtrl.SelectedIndex;
				var tp = (TabItem)_tabCtrl.Items[sel];
				if (tp.Content != null)
					tp.Content = null;

				tp.Content = (UIElement)value;
			}
		}

		public void SelectTab(string name)
		{
			foreach (TabItem page in this._tabCtrl.Items)
			{
				if ((string)page.Name == name)
				{
					this._tabCtrl.SelectedItem = page;
					break;
				}
			}
		}

		#endregion ILayerView Members

		private int _tabControl_SelectionChanged_Calls;

		private void EhTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!object.ReferenceEquals(e.OriginalSource, _tabCtrl))
				return;
			e.Handled = true;

			if (0 == _tabControl_SelectionChanged_Calls)
			{
				++_tabControl_SelectionChanged_Calls;
				bool shouldBeCancelled = false;

				if (e.RemovedItems.Count > 0 && null != TabValidating)
				{
					if (!(e.RemovedItems[0] is TabItem))
					{
						Current.Gui.ErrorMessageBox(string.Format("Homework for the programmer: SelectionChangeHandler is not finalized with e.Handled=true"));
						e.Handled = true;
						goto end_of_function;
					}

					var tp = (TabItem)e.RemovedItems[0];
					var cancelEventArgs = new System.ComponentModel.CancelEventArgs();
					if (null != TabValidating)
						TabValidating(this, cancelEventArgs);
					shouldBeCancelled = cancelEventArgs.Cancel;

					if (shouldBeCancelled)
						_tabCtrl.SelectedItem = tp;
				}

				if (!shouldBeCancelled)
				{
					foreach (var it in e.RemovedItems)
						if (it is TabItem)
							((TabItem)it).Content = null;

					if (null != PageChanged)
					{
						var tp = (TabItem)_tabCtrl.SelectedItem;
						PageChanged(tp.Name);
					}
				}

			end_of_function:
				--_tabControl_SelectionChanged_Calls;
			}
		}
	}
}