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

namespace Altaxo.Gui.Graph.Gdi.Plot.Groups
{
	/// <summary>
	/// Interaction logic for PlotGroupCollectionControl.xaml
	/// </summary>
	public partial class PlotGroupCollectionControl : UserControl, IPlotGroupCollectionView, Altaxo.Gui.Graph.Graph3D.Plot.Groups.IPlotGroupCollectionView
	{
		public PlotGroupCollectionControl()
		{
			InitializeComponent();
		}

		private void EhGotoAdvanced(object sender, RoutedEventArgs e)
		{
			if (null != GotoAdvanced)
				GotoAdvanced();
		}

		private void EhGotoSimple(object sender, RoutedEventArgs e)
		{
			if (null != GotoSimple)
				GotoSimple();
		}

		#region IPlotGroupCollectionView

		public event Action GotoAdvanced;

		public event Action GotoSimple;

		public void SetSimpleView(object viewObject)
		{
			_controlHost.Child = null;
			_controlHost.Child = (UIElement)viewObject;
			_btGotoSimple.Visibility = System.Windows.Visibility.Collapsed;
			_btGotoAdvanced.Visibility = System.Windows.Visibility.Visible;
		}

		public void SetAdvancedView(object viewObject)
		{
			_controlHost.Child = null;
			_controlHost.Child = (UIElement)viewObject;
			_btGotoAdvanced.Visibility = System.Windows.Visibility.Collapsed;
			_btGotoSimple.Visibility = System.Windows.Visibility.Visible;
		}

		#endregion IPlotGroupCollectionView
	}
}