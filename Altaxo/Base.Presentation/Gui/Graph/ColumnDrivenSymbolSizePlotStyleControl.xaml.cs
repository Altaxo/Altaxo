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
	/// Interaction logic for ColumnDrivenColorPlotStyleControl.xaml
	/// </summary>
	public partial class ColumnDrivenSymbolSizePlotStyleControl : UserControl, IColumnDrivenSymbolSizePlotStyleView
	{
		public ColumnDrivenSymbolSizePlotStyleControl()
		{
			InitializeComponent();
		}

		private void _btSelectDataColumn_Click(object sender, RoutedEventArgs e)
		{
			if (null != ChooseDataColumn)
				ChooseDataColumn();
		}

		private void _btClearDataColumn_Click(object sender, RoutedEventArgs e)
		{
			if (null != ClearDataColumn)
				ClearDataColumn();
		}

		#region  IColumnDrivenSymbolSizePlotStyleView


		public IDensityScaleView ScaleView
		{
			get { return _ctrlScale; }
		}

		public event Action ChooseDataColumn;

		public event Action ClearDataColumn;

		public string DataColumnName
		{
			set { _edDataColumn.Text = value; }
		}

		public double SymbolSizeAt0
		{
			get
			{
				return _cbSymbolSizeAt0.SelectedQuantityInPoints;
			}
			set
			{
				_cbSymbolSizeAt0.SelectedQuantityInPoints = value;
			}
		}

		public double SymbolSizeAt1
		{
			get
			{
				return _cbSymbolSizeAt1.SelectedQuantityInPoints;
			}
			set
			{
				_cbSymbolSizeAt1.SelectedQuantityInPoints = value;
			}
		}

		public double SymbolSizeAbove
		{
			get
			{
				return _cbSymbolSizeAbove.SelectedQuantityInPoints;
			}
			set
			{
				_cbSymbolSizeAbove.SelectedQuantityInPoints = value;
			}
		}

		public double SymbolSizeBelow
		{
			get
			{
				return _cbSymbolSizeBelow.SelectedQuantityInPoints;
			}
			set
			{
				_cbSymbolSizeBelow.SelectedQuantityInPoints = value;
			}
		}

		public double SymbolSizeInvalid
		{
			get
			{
				return _cbSymbolSizeInvalid.SelectedQuantityInPoints;
			}
			set
			{
				_cbSymbolSizeInvalid.SelectedQuantityInPoints = value;
			}
		}

		public int NumberOfSteps
		{
			get
			{
				return (int)_edNumberOfSteps.Value;
			}
			set
			{
				_edNumberOfSteps.Value = value;
			}
		}

		#endregion
	}
}
