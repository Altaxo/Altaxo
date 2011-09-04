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

using Altaxo.Collections;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Graph;

using sd = System.Drawing;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for XYPlotScatterStyleControl.xaml
	/// </summary>
	public partial class XYPlotScatterStyleControl : UserControl, IXYPlotScatterStyleView
	{
		private bool _enableDisableAll = false;
		private int _suppressEvents = 0;

		public XYPlotScatterStyleControl()
		{
			InitializeComponent();
		}

		private void EhSymbolShape_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			if (this._enableDisableAll)
				EnableDisableMain(this.ShouldEnableMain());
		}


		public void EnableDisableMain(bool bEnable)
		{
			this._chkIndependentColor.IsEnabled = bEnable;
			this._chkIndependentSize.IsEnabled = bEnable;

			this._cbColor.IsEnabled = bEnable;
			this._cbSymbolSize.IsEnabled = bEnable;
			this._cbSymbolStyle.IsEnabled = bEnable;
			this._edSymbolSkipFrequency.IsEnabled = bEnable;
		}

		bool ShouldEnableMain()
		{
			return this._cbSymbolShape.SelectedIndex != 0 ||
				this._lbDropLines.SelectedItems.Count > 0;

		}

		#region IXYPlotScatterStyleView

	


		public void SetEnableDisableMain(bool bActivate)
		{
			this._enableDisableAll = bActivate;
			this.EnableDisableMain(_enableDisableAll == false || this.ShouldEnableMain());
		}

		public void InitializePlotStyleColor(NamedColor sel)
		{
			_cbColor.SelectedBrush = new Altaxo.Graph.Gdi.BrushX(sel);
		}

		public void InitializeSymbolSize(double size)
		{
			_cbSymbolSize.SelectedQuantityInPoints = size;
		}

		public void InitializeIndependentSymbolSize(bool val)
		{
			this._chkIndependentSize.IsChecked = val;
		}

		public void InitializeSymbolStyle(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbSymbolStyle, list);
		}

		public void InitializeSymbolShape(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbSymbolShape, list);
		}

		public void InitializeDropLineConditions(SelectableListNodeList names)
		{
			_lbDropLines.ItemsSource = names;
		}

		public void InitializeIndependentColor(bool val)
		{
			this._chkIndependentColor.IsChecked = val;
		}

		public void InitializeSkipPoints(int val)
		{
			this._edSymbolSkipFrequency.Value = val;
		}

		public bool IndependentColor
		{
			get
			{
				return true==_chkIndependentColor.IsChecked;
			}
		}

		public NamedColor SymbolColor
		{
			get { return _cbColor.SelectedBrush.Color; }
		}

		public SelectableListNode SymbolShape
		{
			get { return (SelectableListNode)_cbSymbolShape.SelectedItem; }
		}

		public bool IndependentSymbolSize
		{
			get { return true==_chkIndependentSize.IsChecked; }
		}

		public SelectableListNode SymbolStyle
		{
			get { return (SelectableListNode)_cbSymbolStyle.SelectedItem; }
		}

		public double SymbolSize
		{
			get { return _cbSymbolSize.SelectedQuantityInPoints; }
		}

		public SelectableListNodeList DropLines
		{
			get { return (SelectableListNodeList)(_lbDropLines.ItemsSource); }
		}

		public int SkipPoints
		{
			get
			{
					return _edSymbolSkipFrequency.Value;
			}
		}

		public string RelativePenWidth
		{
			get
			{
				return _edRelativePenWidth.Text;
			}
			set
			{
				_edRelativePenWidth.Text = value;
			}
		}

		#endregion
	}
}
