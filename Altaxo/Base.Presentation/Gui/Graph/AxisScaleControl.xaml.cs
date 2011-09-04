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
	/// <summary>
	/// Interaction logic for AxisScaleControl.xaml
	/// </summary>
	public partial class AxisScaleControl : UserControl, IAxisScaleView
	{
		public AxisScaleControl()
		{
			InitializeComponent();
		}

		private void EhAxisType_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != AxisTypeChanged)
			{
				GuiHelper.SynchronizeSelectionFromGui(this.m_Scale_cbType);
				AxisTypeChanged();
			}
		}

		private void EhLinkTarget_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != LinkTargetChanged)
			{
				GuiHelper.SynchronizeSelectionFromGui(this._cbLinkTarget);
				LinkTargetChanged();
			}
		}

		private void EhLinked_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (null != LinkChanged)
			{
				LinkChanged(_chkLinkScale.IsChecked == true);
			}
		}

		private void EhTickSpacingType_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != TickSpacingTypeChanged)
			{
				ComboBox _cbTickSpacingType = (ComboBox)sender;
				GuiHelper.SynchronizeSelectionFromGui(_cbTickSpacingType);
				TickSpacingTypeChanged();
			}
		}

		#region IAxisScaleView

		public void InitializeAxisType(Collections.SelectableListNodeList names)
		{
			GuiHelper.Initialize(this.m_Scale_cbType, names);
		}

		public void InitializeTickSpacingType(Collections.SelectableListNodeList names)
		{
			ComboBox _cbTickSpacingType = (ComboBox)LogicalTreeHelper.FindLogicalNode((DependencyObject)_tickSpacingGroupBox.Header, "_cbTickSpacingType");
			GuiHelper.Initialize(_cbTickSpacingType, names);
		}


		public void InitializeLinkTargets(Collections.SelectableListNodeList names)
		{
			GuiHelper.Initialize(this._cbLinkTarget, names);
		}

		public bool ScaleIsLinked
		{
			get
			{
				return _chkLinkScale.IsChecked == true;
			}
			set
			{
				_chkLinkScale.IsChecked = value;
			}
		}

		private UIElement _boundaryControl = null;
		public void SetBoundaryView(object guiobject)
		{
			if (null != _boundaryControl)
				_tlp_Main.Children.Remove(_boundaryControl);

			_boundaryControl = guiobject as UIElement;


			if (_boundaryControl != null)
			{
				_boundaryControl.SetValue(Grid.ColumnProperty, 0);
				_boundaryControl.SetValue(Grid.ColumnSpanProperty, 2);
				_boundaryControl.SetValue(Grid.RowProperty, 4);
				_tlp_Main.Children.Add(_boundaryControl);
			}
		}

		private UIElement _scaleControl = null;
		public void SetScaleView(object guiobject)
		{
			if (null != _boundaryControl)
				_tlp_Main.Children.Remove(_scaleControl);

			_scaleControl = guiobject as UIElement;


			if (_scaleControl != null)
			{
				_scaleControl.SetValue(Grid.ColumnProperty, 0);
				_scaleControl.SetValue(Grid.ColumnSpanProperty, 2);
				_scaleControl.SetValue(Grid.RowProperty, 4);
				_tlp_Main.Children.Add(_scaleControl);
			}
		}
		private UIElement _tickSpacingControl = null;
		public void SetTickSpacingView(object guiobject)
		{
			if (null != _tickSpacingControl)
				_tickSpacingGroupBox.Content = null;

			_tickSpacingControl = guiobject as UIElement;


			if (_tickSpacingControl != null)
			{
				_tickSpacingGroupBox.Content = _tickSpacingControl;
				/*
				_tickSpacingControl.SetValue(Grid.ColumnProperty, 0);
				_tickSpacingControl.SetValue(Grid.ColumnSpanProperty, 2);
				_tickSpacingControl.SetValue(Grid.RowProperty, 8);
				_tlp_Main.Children.Add(_tickSpacingControl);
				 */
			}
		}

		public event Action AxisTypeChanged;

		public event Action TickSpacingTypeChanged;

		public event Action LinkTargetChanged;

		public event Action<bool> LinkChanged;

		#endregion


	}
}
