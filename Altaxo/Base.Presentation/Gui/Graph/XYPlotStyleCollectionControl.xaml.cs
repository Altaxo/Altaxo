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
	public partial class XYPlotStyleCollectionControl : UserControl, IXYPlotStyleCollectionView, Altaxo.Gui.Graph3D.Plot.Styles.IXYZPlotStyleCollectionView
	{
		public event Action RequestAddStyle;

		public event Action RequestStyleUp;

		public event Action RequestStyleDown;

		public event Action RequestStyleEdit;

		public event Action RequestStyleRemove;

		public event Action PredefinedStyleSelected;

		public XYPlotStyleCollectionControl()
		{
			InitializeComponent();
		}

		private void EhPredefinedSets_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (PredefinedStyleSelected != null && null != _predefinedSetsAvailable.SelectedItem)
			{
				GuiHelper.SynchronizeSelectionFromGui(_predefinedSetsAvailable);
				PredefinedStyleSelected?.Invoke();
			}
		}

		private void EhSingleStylesAvailable_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (RequestAddStyle != null && _singleStylesAvailable.SelectedItem != null)
			{
				GuiHelper.SynchronizeSelectionFromGui(_singleStylesAvailable);
				RequestAddStyle?.Invoke();
			}
		}

		private void EhCurrentStyles_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			RequestStyleEdit?.Invoke();
		}

		private void EhStyleUp_Click(object sender, RoutedEventArgs e)
		{
			RequestStyleUp?.Invoke();
		}

		private void EhStyleDown_Click(object sender, RoutedEventArgs e)
		{
			RequestStyleDown?.Invoke();
		}

		private void EhStyleRemove_Click(object sender, RoutedEventArgs e)
		{
			RequestStyleRemove?.Invoke();
		}

		#region IXYPlotStyleCollectionView

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