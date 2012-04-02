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
	/// Interaction logic for TitleFormatLayerControl.xaml
	/// </summary>
	public partial class TitleFormatLayerControl : UserControl, ITitleFormatLayerView
	{
		public TitleFormatLayerControl()
		{
			InitializeComponent();
		}

		#region ITitleFormatLayerView

		public bool ShowAxisLine
		{
			get
			{
				return ((UIElement)_axisLineGroupBox.Content).IsEnabled == true;
			}
			set
			{
				((UIElement)_axisLineGroupBox.Content).IsEnabled = value;
			}
		}

		public bool ShowMajorLabels
		{
			get
			{
				return _chkShowMajorLabels.IsChecked == true;
			}
			set
			{
				_chkShowMajorLabels.IsChecked = value;
			}
		}

		public bool ShowMinorLabels
		{
			get
			{
				return _chkShowMinorLabels.IsChecked == true;
			}
			set
			{
				_chkShowMinorLabels.IsChecked = value;
			}
		}

		public event Action ShowAxisLineChanged;
		public event Action ShowMajorLabelsChanged;
		public event Action ShowMinorLabelsChanged;

		UIElement _lineStyleControl;
		public object LineStyleView
		{
			set
			{
				var oldControl = (UIElement)_axisLineGroupBox.Content;
				bool wasEnabled = oldControl.IsEnabled == true;

				var newControl = value as UIElement;

				if (newControl == null)
					newControl = new Label();

				newControl.IsEnabled = wasEnabled;
				_axisLineGroupBox.Content = newControl;
			}
		}

		public string AxisTitle
		{
			get
			{
				return m_Format_edTitle.Text;
			}
			set
			{
				m_Format_edTitle.Text = value;
			}
		}

		public double PositionOffset
		{
			get
			{
				return (double)m_Format_edAxisPositionValue.Value;
			}
		}

		#endregion

		private void EhShowAxisLineChanged(object sender, RoutedEventArgs e)
		{
			if (null != ShowAxisLineChanged)
				ShowAxisLineChanged();
		}

		private void EhShowMajorLabelsChanged(object sender, RoutedEventArgs e)
		{
			if (null != ShowMajorLabelsChanged)
				ShowMajorLabelsChanged();
		}

		private void EhShowMinorLabelsChanged(object sender, RoutedEventArgs e)
		{
			if (null != ShowMinorLabelsChanged)
				ShowMinorLabelsChanged();
		}
	}
}
