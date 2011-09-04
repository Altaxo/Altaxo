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
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Graph
{
	using Altaxo.Gui.Common;

	/// <summary>
	/// Interaction logic for LayerControl.xaml
	/// </summary>
	public partial class LayerControl : UserControl, ILayerView
	{
		private ILayerController _controller;
		private int _suppressEventCounter = 0;
		CheckableGroupBox _chkPageEnable;

		public LayerControl()
		{
			InitializeComponent();
			_chkPageEnable = new CheckableGroupBox() { EnableContentWithCheck = true };
			_chkPageEnable.Checked += EhControlEnable_Checked;
			_chkPageEnable.Unchecked += EhControlEnable_Unchecked;
		}


		#region ILayerView Members

		public ILayerController Controller
		{
			get
			{
				return _controller;
			}
			set
			{
				_controller = value;
			}
		}

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

		public void SetCurrentContentWithEnable(object guielement, bool enable, string title)
		{
			++_suppressEventCounter;

			int sel = _tabCtrl.SelectedIndex;
			var tp = (TabItem)_tabCtrl.Items[sel];
			if (tp.Content != null)
				tp.Content = null;

			_chkPageEnable.IsChecked = enable;
			_chkPageEnable.Header = title;



			_chkPageEnable.Content = (UIElement)guielement;


			tp.Content = _chkPageEnable;

			--_suppressEventCounter;
		}

		void EhControlEnable_Checked(object sender, RoutedEventArgs e)
		{
			if (null != _controller && _suppressEventCounter == 0)
				_controller.EhView_PageEnabledChanged(true);
		}

		void EhControlEnable_Unchecked(object sender, RoutedEventArgs e)
		{
			if (null != _controller && _suppressEventCounter == 0)
				_controller.EhView_PageEnabledChanged(false);
		}





		public bool IsPageEnabled
		{
			get
			{
				if (_tabCtrl.SelectedContent is CheckableGroupBox)
					return (_tabCtrl.SelectedContent as CheckableGroupBox).IsChecked == true;
				else
					return true;
			}
			set
			{
				if (_tabCtrl.SelectedContent is CheckableGroupBox)
					(_tabCtrl.SelectedContent as CheckableGroupBox).IsChecked = value;
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

		public void InitializeSecondaryChoice(string[] names, string name)
		{
			++_suppressEventCounter;
			this._lbEdges.Items.Clear();
			foreach (var n in names)
				this._lbEdges.Items.Add(n);

			this._lbEdges.SelectedItem = name;
			--_suppressEventCounter;

		}

		public event System.ComponentModel.CancelEventHandler TabValidating;

		#endregion

		private void EhSecondChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != _controller && _suppressEventCounter == 0)
				_controller.EhView_SecondChoiceChanged(this._lbEdges.SelectedIndex, (string)this._lbEdges.SelectedItem);
		}


		int _tabControl_SelectionChanged_Calls;
		private void EhTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!object.ReferenceEquals(e.OriginalSource, _tabCtrl))
				return;
			e.Handled = true;

			if (0 == _tabControl_SelectionChanged_Calls)
			{
				++_tabControl_SelectionChanged_Calls;
				bool shouldBeCancelled = false;

				if (null != _controller && e.RemovedItems.Count > 0)
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
					_chkPageEnable.Content = null;
					foreach (var it in e.RemovedItems)
						if (it is TabItem)
							((TabItem)it).Content = null;

					if (null != _controller)
					{
						var tp = (TabItem)_tabCtrl.SelectedItem;
						_controller.EhView_PageChanged(tp.Name);
					}
				}

			end_of_function:
				--_tabControl_SelectionChanged_Calls;
			}
		}
	}
}
