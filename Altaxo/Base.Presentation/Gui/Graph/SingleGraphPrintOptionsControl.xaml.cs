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
	/// Interaction logic for SingleGraphPrintOptionsControl.xaml
	/// </summary>
	public partial class SingleGraphPrintOptionsControl : UserControl, ISingleGraphPrintOptionsView
	{
		public SingleGraphPrintOptionsControl()
		{
			InitializeComponent();
		}

		private void EhPrintLocationChanged(object sender, SelectionChangedEventArgs e)
		{
			if (null != PrintLocationChanged)
				PrintLocationChanged();
			e.Handled = true;
		}

		private void EhRotatePageAutomaticallyChanged(object sender, RoutedEventArgs e)
		{
			if (null != RotatePageAutomaticallyChanged)
				RotatePageAutomaticallyChanged(_chkRotatePageAutomatically.IsChecked == true);
			e.Handled = true;
		}

		private void EhFitGraphToPrintIfSmallerChanged(object sender, RoutedEventArgs e)
		{
			if (null != FitGraphToPrintIfSmallerChanged)
				FitGraphToPrintIfSmallerChanged(_chkFitGraphToPrintIfSmaller.IsChecked == true);
			e.Handled = true;
		}

		private void EhFitGraphToPrintIfLargerChanged(object sender, RoutedEventArgs e)
		{
			if (null != FitGraphToPrintIfLargerChanged)
				FitGraphToPrintIfLargerChanged(_chkFitGraphToPrintIfLarger.IsChecked == true);
			e.Handled = true;
		}

		private void EhPrintCropMarksChanged(object sender, RoutedEventArgs e)
		{
			if (null != PrintCropMarksChanged)
				PrintCropMarksChanged(_chkPrintCropMarks.IsChecked == true);
			e.Handled = true;
		}

		private void EhTilePagesChanged(object sender, RoutedEventArgs e)
		{
			if (null != TilePagesChanged)
				TilePagesChanged(_chkTilePages.IsChecked == true);
			e.Handled = true;
		}

		private void EhUseFixedZoomFactorChanged(object sender, RoutedEventArgs e)
		{
			if (null != UseFixedZoomFactorChanged)
				UseFixedZoomFactorChanged(_chkUseFixedZoomFactor.IsChecked == true);
			e.Handled = true;

		}

		private void EhZoomFactorChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (null != ZoomFactorChanged)
				ZoomFactorChanged(_edZoomFactor.SelectedScale);
		}

		#region  ISingleGraphPrintOptionsView

		public void Init_PrintLocation(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbPrintLocation, list);
		}

		public void Init_FitGraphToPrintIfLarger(bool val)
		{
			_chkFitGraphToPrintIfLarger.IsChecked = val;
		}

		public void Init_FitGraphToPrintIfSmaller(bool val)
		{
			_chkFitGraphToPrintIfSmaller.IsChecked = val;
		}

		public void Init_PrintCopMarks(bool val)
		{
			_chkPrintCropMarks.IsChecked = val;
		}

		public void Init_RotatePageAutomatically(bool value)
		{
			_chkRotatePageAutomatically.IsChecked = value;
		}

		public void Init_TilePages(bool value)
		{
			_chkTilePages.IsChecked = value;
		}

		public void Init_UseFixedZoomFactor(bool val)
		{
			_chkUseFixedZoomFactor.IsChecked = val;
		}

		public void Init_ZoomFactor(double val)
		{
			_edZoomFactor.SelectedScale = val;
		}

		public event Action PrintLocationChanged;

		public event Action<bool> FitGraphToPrintIfLargerChanged;

		public event Action<bool> FitGraphToPrintIfSmallerChanged;

		public event Action<bool> PrintCropMarksChanged;

		public event Action<bool> RotatePageAutomaticallyChanged;

		public event Action<bool> TilePagesChanged;

		public event Action<bool> UseFixedZoomFactorChanged;

		public event Action<double> ZoomFactorChanged;

		#endregion
	}
}
