#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Gui.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
	/// <summary>
	/// Interaction logic for ColumnDrivenColorPlotStyleControl.xaml
	/// </summary>
	public partial class DataMeshPlotStyleControl : UserControl, IDataMeshPlotStyleView
	{
		public DataMeshPlotStyleControl()
		{
			InitializeComponent();
		}

		#region IDataMeshPlotStyleView

		public IDensityScaleView ColorScaleView
		{
			get { return _guiColorScale; }
		}

		public bool IsCustomColorScaleUsed
		{
			get
			{
				return _guiUseCustomColorScale.IsChecked == true;
			}
			set
			{
				_guiUseCustomColorScale.IsChecked = value;
				UpdateVisibilityOfColorScale();
			}
		}

		public IColorProviderView ColorProviderView
		{
			get { return _colorProviderControl; }
		}

		public bool ClipToLayer
		{
			get
			{
				return true == _chkClipToLayer.IsChecked;
			}
			set
			{
				_chkClipToLayer.IsChecked = value;
			}
		}

		public object MaterialViewObject
		{
			get
			{
				return _guiMaterial;
			}
		}

		#endregion IDataMeshPlotStyleView

		private void EhUseCustomColorScaleChanged(object sender, System.Windows.RoutedEventArgs e)
		{
			UpdateVisibilityOfColorScale();
		}

		private void UpdateVisibilityOfColorScale()
		{
			if (_guiUseCustomColorScale.IsChecked == true)
			{
				if (null != _guiColorScale)
					_guiColorScale.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				if (null != _guiColorScale)
					_guiColorScale.Visibility = System.Windows.Visibility.Collapsed;
			}
		}
	}
}