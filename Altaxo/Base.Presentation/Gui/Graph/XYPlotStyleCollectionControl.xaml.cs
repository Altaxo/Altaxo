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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for XYPlotStyleCollectionControl.xaml
	/// </summary>
	[UserControlForController(typeof(IXYPlotStyleCollectionViewEventSink))]
	public partial class XYPlotStyleCollectionControl : UserControl, IXYPlotStyleCollectionView
	{
		public XYPlotStyleCollectionControl()
		{
			InitializeComponent();
		}

		private void EhPredefinedSets_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (_controller != null && null != _predefinedSetsAvailable.SelectedItem)
			{
				GuiHelper.SynchronizeSelectionFromGui(_predefinedSetsAvailable);
				_controller.EhView_PredefinedStyleSelected();
			}
		}

		private void EhSingleStylesAvailable_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (_controller != null && _singleStylesAvailable.SelectedItem != null)
			{
				GuiHelper.SynchronizeSelectionFromGui(_singleStylesAvailable);
				_controller.EhView_AddStyle();
			}
		}

		private void EhCurrentStyles_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (_controller != null)
			{
				_controller.EhView_StyleEdit();
			}
		}

		private void EhStyleUp_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
			{
				_controller.EhView_StyleUp();
			}
		}

		private void EhStyleDown_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
			{
				_controller.EhView_StyleDown();
			}
		}

		private void EhStyleRemove_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
			{
				_controller.EhView_StyleRemove();
			}
		}

		#region IXYPlotStyleCollectionView

		private IXYPlotStyleCollectionViewEventSink _controller;

		public IXYPlotStyleCollectionViewEventSink Controller
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

		public void InitializePredefinedStyles(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_predefinedSetsAvailable, list);
		}

		public void InitializeStyleList(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_lbStyles, list);
		}

		public void InitializeAvailableStyleList(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_singleStylesAvailable, list);
		}

		#endregion IXYPlotStyleCollectionView
	}
}