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
	/// Interaction logic for PlottingRangeControl.xaml
	/// </summary>
	[UserControlForController(typeof(IPlottingRangeViewEventSink))]
	public partial class PlottingRangeControl : UserControl, IPlottingRangeView
	{
		public PlottingRangeControl()
		{
			InitializeComponent();
		}

		private void _edFrom_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (_controller != null)
				_controller.EhView_Changed(_edFrom.Value, _edTo.Value);
		}

		private void _edTo_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (_controller != null)
				_controller.EhView_Changed(_edFrom.Value, _edTo.Value);
		}

		#region IPlottingRangeView

		IPlottingRangeViewEventSink _controller;
		public IPlottingRangeViewEventSink Controller
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

		/// <summary>
		/// Initializes the view.
		/// </summary>
		/// <param name="from">First value of plot range.</param>
		/// <param name="to">Last value of plot range.</param>
		/// <param name="isInfinity">True if the plot range is infinite large.</param>
		public void Initialize(int from, int to, bool isInfinity)
		{
			_edFrom.Value = from;
			if (isInfinity)
				_edTo.Value = _edTo.Maximum;
			else
				_edTo.Value = to;
		}

		#endregion


	}
}
