#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for AscendingIntegerControl.xaml
	/// </summary>
	public partial class AscendingIntegerCollectionControl : UserControl, IAscendingIntegerCollectionView
	{
		public event Action SwitchToAdvandedView;

		public event Action<object> InitializingNewRangeItem;

		public event Action<int, int> AdvancedAddRange;

		public event Action<int, int> AdvancedRemoveRange;

		private IEnumerable<object> _ranges;

		public AscendingIntegerCollectionControl()
		{
			InitializeComponent();
		}

		public void SetRangeListSource(IEnumerable<object> ranges)
		{
			_ranges = ranges;
			_guiDataGrid.ItemsSource = _ranges;
		}

		public void SwitchEasyAdvanced(bool showAdvanced)
		{
			if (showAdvanced)
			{
				_guiSimpleGrid.Visibility = Visibility.Collapsed;
				_guiAdvancedGrid.Visibility = Visibility.Visible;
				_guiDataGrid.ItemsSource = _ranges;
			}
			else // Show easy
			{
				_guiSimpleGrid.Visibility = Visibility.Visible;
				_guiAdvancedGrid.Visibility = Visibility.Collapsed;

				_guiDataGrid.ItemsSource = null;
			}
		}

		private void EhSwitchToAdvanced(object sender, RoutedEventArgs e)
		{
			var ev = SwitchToAdvandedView;
			if (null != ev)
				ev();
		}

		public int EasyRangeFrom
		{
			get
			{
				return _guiSimpleFrom.Value;
			}
			set
			{
				_guiSimpleFrom.Value = value;
			}
		}

		public int EasyRangeTo
		{
			get
			{
				return _guiSimpleTo.Value;
			}
			set
			{
				_guiSimpleTo.Value = value;
			}
		}

		private void EhInitializingNewItem(object sender, InitializingNewItemEventArgs e)
		{
			var ev = InitializingNewRangeItem;
			if (null != ev)
				ev(e.NewItem);
		}

		private void EhAddRemoveFrom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			var value = _guiAddRemoveFrom.Value;
			if (value > _guiAddRemoveTo.Value)
				_guiAddRemoveTo.Value = value;
		}

		private void EhAddRemoveTo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			var value = _guiAddRemoveTo.Value;
			if (value < _guiAddRemoveFrom.Value)
				_guiAddRemoveFrom.Value = value;
		}

		private void EhSimpleFrom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			var value = _guiSimpleFrom.Value;
			if (value > _guiSimpleTo.Value)
				_guiSimpleTo.Value = value;
		}

		private void EhSimpleTo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			var value = _guiSimpleTo.Value;
			if (value < _guiSimpleFrom.Value)
				_guiSimpleFrom.Value = value;
		}

		private void EhAdvancedRangeAdd(object sender, RoutedEventArgs e)
		{
			var ev = AdvancedAddRange;
			if (null != ev)
				ev(_guiAddRemoveFrom.Value, _guiAddRemoveTo.Value);
		}

		private void EhAdvancedRangeRemove(object sender, RoutedEventArgs e)
		{
			var ev = AdvancedRemoveRange;
			if (null != ev)
				ev(_guiAddRemoveFrom.Value, _guiAddRemoveTo.Value);
		}
	}
}