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

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for PlotGroupCollectionControl.xaml
	/// </summary>
	public partial class PlotGroupCollectionControlAdvanced : UserControl, IPlotGroupCollectionViewAdvanced
	{
		public PlotGroupCollectionControlAdvanced()
		{
			InitializeComponent();
		}

		private void _cbCoordTransfoStyle_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			if (null != _controller)
			{
				GuiHelper.SynchronizeSelectionFromGui(_cbCoordTransfoStyle);
				_controller.EhView_CoordinateTransformingGroupStyleChanged();
			}
		}

		private void _btEditCSTransfoStyle_Click(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
				_controller.EhView_CoordinateTransformingGroupStyleEdit();
		}

		private void _btAddNormalGroupStyle_Click(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
			{
				// SynchronizeListBoxWithSelectableListNodes(_lbGroupStylesAvailable);
				_controller.EhView_AddNormalGroupStyle();
			}
		}

		private void _btRemoveNormalGroupStyle_Click(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
			{
				// SynchronizeListBoxWithCheckableSelectableListNodes(_lbGroupStyles);
				_controller.EhView_RemoveNormalGroupStyle();
			}
		}

		private void _btUnindent_Click(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
			{
				// SynchronizeListBoxWithCheckableSelectableListNodes(_lbGroupStyles);
				_controller.EhView_UnindentGroupStyle();
			}
		}

		private void _btIndent_Click(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
			{
				// SynchronizeListBoxWithCheckableSelectableListNodes(_lbGroupStyles);
				_controller.EhView_IndentGroupStyle();
			}
		}

		private void _btUp_Click(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
			{
				// SynchronizeListBoxWithCheckableSelectableListNodes(_lbGroupStyles);
				_controller.EhView_MoveUpGroupStyle();
			}
		}

		private void _btDown_Click(object sender, RoutedEventArgs e)
		{
			if (null != _controller)
			{
				// SynchronizeListBoxWithCheckableSelectableListNodes(_lbGroupStyles);
				_controller.EhView_MoveDownGroupStyle();
			}
		}

		#region IPlotGroupCollectionView

		private IPlotGroupCollectionViewAdvancedEventSink _controller;

		public IPlotGroupCollectionViewAdvancedEventSink Controller
		{
			set { _controller = value; }
		}

		public void InitializeAvailableCoordinateTransformingGroupStyles(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(this._cbCoordTransfoStyle, list);
		}

		public void InitializeAvailableNormalGroupStyles(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_lbGroupStylesAvailable, list);
		}

		public void InitializeUpdateMode(Collections.SelectableListNodeList list, bool inheritFromParent, bool distributeToChilds)
		{
			GuiHelper.Initialize(_cbGroupStrictness, list);
			_chkUpdateFromParentGroups.IsChecked = inheritFromParent;
			_chkDistributeToSubGroups.IsChecked = distributeToChilds;
		}

		public void InitializeCurrentNormalGroupStyles(Collections.CheckableSelectableListNodeList list)
		{
			GuiHelper.Initialize(_lbGroupStyles, list);
		}

		public void SynchronizeCurrentNormalGroupStyles()
		{
			//SynchronizeListBoxWithCheckableSelectableListNodes(_lbGroupStyles);
		}

		public void QueryUpdateMode(out bool inheritFromParent, out bool distributeToChilds)
		{
			GuiHelper.SynchronizeSelectionFromGui(_cbGroupStrictness);
			inheritFromParent = true == _chkUpdateFromParentGroups.IsChecked;
			distributeToChilds = true == _chkDistributeToSubGroups.IsChecked;
		}

		#endregion IPlotGroupCollectionView
	}
}