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

using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
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
	public partial class ScatterPlotStyleControl : UserControl, IScatterPlotStyleView
	{
		public event Action IndependentColorChanged;

		public event Action ScatterSymbolChanged;

		public ScatterPlotStyleControl()
		{
			InitializeComponent();
		}

		public void EnableDisableMain(bool bEnable)
		{
			this._chkIndependentColor.IsEnabled = bEnable;
			this._chkIndependentSize.IsEnabled = bEnable;

			this._cbColor.IsEnabled = bEnable;
			this._cbSymbolSize.IsEnabled = bEnable;
			this._edSymbolSkipFrequency.IsEnabled = bEnable;
		}

		#region IXYPlotScatterStyleView

		public void InitializeSymbolStyle(SelectableListNodeList list)
		{
		}

		public void InitializeSymbolShape(SelectableListNodeList list)
		{
			_cbSymbolShape.SelectedItem = list.FirstSelectedNode?.Tag as IScatterSymbol;
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

		public NamedColor Color
		{
			get { return _cbColor.SelectedColor; }
			set { _cbColor.SelectedColor = value; }
		}

		public bool IndependentScatterSymbol
		{
			get
			{
				return true == _guiIndependentScatterSymbol.IsChecked;
			}
			set
			{
				_guiIndependentScatterSymbol.IsChecked = value;
			}
		}

		public IScatterSymbol ScatterSymbol
		{
			get { return _cbSymbolShape.SelectedItem; }
			set { _cbSymbolShape.SelectedItem = value; }
		}

		private void EhScatterSymbolChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			ScatterSymbolChanged?.Invoke();
		}

		public bool UseSymbolFrame
		{
			get
			{
				return _guiUseFrame.IsChecked == true;
			}
			set
			{
				_guiUseFrame.IsChecked = value;
			}
		}

		public SelectableListNodeList Inset
		{
			set
			{
				GuiHelper.Initialize(_guiInset, value);
			}
		}

		private void EhInsetChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiInset);
		}

		public bool IndependentSymbolSize
		{
			get { return true == _chkIndependentSize.IsChecked; }
			set { this._chkIndependentSize.IsChecked = value; }
		}

		public double SymbolSize
		{
			get { return _cbSymbolSize.SelectedQuantityAsValueInPoints; }
			set { _cbSymbolSize.SelectedQuantityAsValueInPoints = value; }
		}

		public int SkipFrequency
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

		public bool IndependentSkipFrequency
		{
			get { return true == _chkIndependentSkipFreq.IsChecked; }
			set { this._chkIndependentSkipFreq.IsChecked = value; }
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
				_cbColor.ShowPlotColorsOnly = value;
			}
		}

		public bool OverrideAbsoluteStructureWidth
		{
			get
			{
				return _guiOverrideAbsoluteStructureWidth.IsChecked == true;
			}
			set
			{
				_guiOverrideAbsoluteStructureWidth.IsChecked = value;
			}
		}

		public double OverriddenAbsoluteStructureWidth
		{
			get
			{
				return _guiOverriddenAbsoluteStructureWidth.SelectedQuantityAsValueInPoints;
			}
			set
			{
				_guiOverriddenAbsoluteStructureWidth.SelectedQuantityAsValueInPoints = value;
			}
		}

		public bool OverrideRelativeStructureWidth
		{
			get
			{
				return _guiOverrideRelativeStructureWidth.IsChecked == true;
			}
			set
			{
				_guiOverrideRelativeStructureWidth.IsChecked = value;
			}
		}

		public double OverriddenRelativeStructureWidth
		{
			get
			{
				return _guiOverriddenRelativeStructureWidth.SelectedQuantityAsValueInSIUnits;
			}
			set
			{
				_guiOverriddenRelativeStructureWidth.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public bool OverridePlotColorInfluence
		{
			get
			{
				return _guiOverridePlotColorInfluence.IsChecked == true;
			}
			set
			{
				_guiOverridePlotColorInfluence.IsChecked = value;
			}
		}

		public PlotColorInfluence OverriddenPlotColorInfluence
		{
			get
			{
				return (PlotColorInfluence)_guiOverriddenPlotColorInfluence.SelectedValue;
			}
			set
			{
				_guiOverriddenPlotColorInfluence.SelectedValue = value;
			}
		}

		public bool OverrideFillColor
		{
			get
			{
				return _guiOverrideFillColor.IsChecked == true;
			}
			set
			{
				_guiOverrideFillColor.IsChecked = value;
			}
		}

		public NamedColor OverriddenFillColor
		{
			get
			{
				return _guiOverriddenFillColor.SelectedColor;
			}
			set
			{
				_guiOverriddenFillColor.SelectedColor = value;
			}
		}

		public bool OverrideFrameColor
		{
			get
			{
				return _guiOverrideFrameColor.IsChecked == true;
			}
			set
			{
				_guiOverrideFrameColor.IsChecked = value;
			}
		}

		public NamedColor OverriddenFrameColor
		{
			get
			{
				return _guiOverriddenFrameColor.SelectedColor;
			}
			set
			{
				_guiOverriddenFrameColor.SelectedColor = value;
			}
		}

		public bool OverrideInsetColor
		{
			get
			{
				return _guiOverrideInsetColor.IsChecked == true;
			}
			set
			{
				_guiOverrideInsetColor.IsChecked = value;
			}
		}

		public NamedColor OverriddenInsetColor
		{
			get
			{
				return _guiOverriddenInsetColor.SelectedColor;
			}
			set
			{
				_guiOverriddenInsetColor.SelectedColor = value;
			}
		}
	}
}