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
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
	/// <summary>
	/// Interaction logic for XYPlotScatterStyleControl.xaml
	/// </summary>
	public partial class XYPlotScatterStyleControl : UserControl, IXYPlotScatterStyleView
	{
		public event Action IndependentColorChanged;

		private PenControlsGlue _symbolPenGlue;

		private bool _enableDisableAll = false;
		private int _suppressEvents = 0;

		public XYPlotScatterStyleControl()
		{
			InitializeComponent();

			_symbolPenGlue = new PenControlsGlue();
			_symbolPenGlue.CbBrush = _cbColor;
		}

		private void EhSymbolShape_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			if (null != _cbSymbolShape)
			{
				GuiHelper.SynchronizeSelectionFromGui(_cbSymbolShape);
			}
		}

		private void EhSymbolStyle_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			if (null != _cbSymbolStyle)
			{
				GuiHelper.SynchronizeSelectionFromGui(_cbSymbolStyle);
			}
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

		#region IXYPlotScatterStyleView

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
			_lbDropLines.Initialize(names);
		}

		public bool IndependentColor
		{
			get
			{
				return true == _chkIndependentColor.IsChecked;
			}
			set
			{
				this._chkIndependentColor.IsChecked = value;
			}
		}

		public PenX SymbolPen
		{
			get { return _symbolPenGlue.Pen; }
			set { _symbolPenGlue.Pen = value; }
		}

		public SelectableListNode SymbolShape
		{
			get { return (SelectableListNode)_cbSymbolShape.SelectedItem; }
		}

		public bool IndependentSymbolSize
		{
			get { return true == _chkIndependentSize.IsChecked; }
			set { this._chkIndependentSize.IsChecked = value; }
		}

		public SelectableListNode SymbolStyle
		{
			get { return (SelectableListNode)_cbSymbolStyle.SelectedItem; }
		}

		public double SymbolSize
		{
			get { return _cbSymbolSize.SelectedQuantityAsValueInPoints; }
			set { _cbSymbolSize.SelectedQuantityAsValueInPoints = value; }
		}

		public int SkipPoints
		{
			get
			{
				return _edSymbolSkipFrequency.Value;
			}
			set
			{
				this._edSymbolSkipFrequency.Value = value;
			}
		}

		public double RelativePenWidth
		{
			get
			{
				return _edRelativePenWidth.SelectedQuantityAsValueInSIUnits;
			}
			set
			{
				_edRelativePenWidth.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		#endregion IXYPlotScatterStyleView

		private void EhIndependentColorChanged(object sender, RoutedEventArgs e)
		{
			if (null != IndependentColorChanged)
				IndependentColorChanged();
		}

		public bool ShowPlotColorsOnly
		{
			set
			{
				_symbolPenGlue.ShowPlotColorsOnly = value;
			}
		}
	}
}