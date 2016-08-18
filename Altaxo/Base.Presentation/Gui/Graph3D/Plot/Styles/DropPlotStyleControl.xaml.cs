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
using Altaxo.Drawing.D3D;
using Altaxo.Gui.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph3D.Plot.Styles
{
	/// <summary>
	/// Interaction logic for XYPlotScatterStyleControl.xaml
	/// </summary>
	public partial class DropPlotStyleControl : UserControl, IDropPlotStyleView
	{
		public event Action IndependentColorChanged;

		public event Action IndependentSkipFreqChanged;

		public event Action IndependentSkipPointsChanged;

		private PenControlsGlue _penGlue;

		private bool _enableDisableAll = false;
		private int _suppressEvents = 0;

		public DropPlotStyleControl()
		{
			InitializeComponent();

			_penGlue = new PenControlsGlue();
			_penGlue.CbBrush = _guiPenMaterial;
			_penGlue.CbDashStyle = _guiPenDashStyle;
		}

		public void InitializeDropLineConditions(SelectableListNodeList names)
		{
			_guiDropLines.Initialize(names);
		}

		public bool IndependentColor
		{
			get
			{
				return true == _guiIndependentColor.IsChecked;
			}
			set
			{
				this._guiIndependentColor.IsChecked = value;
			}
		}

		public PenX3D Pen
		{
			get { return _penGlue.Pen; }
			set { _penGlue.Pen = value; }
		}

		public int SkipPoints
		{
			get
			{
				return _guiSkipFrequency.Value;
			}
			set
			{
				this._guiSkipFrequency.Value = value;
			}
		}

		public bool IndependentSkipPoints
		{
			get
			{
				return true == _guiIndependentSkipFreq.IsChecked;
			}

			set
			{
				_guiIndependentSkipFreq.IsChecked = value;
			}
		}

		public void InitializeRelativePenWidth1(Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
		{
			_guiPenWidth1.UnitEnvironment = env;
			_guiPenWidth1.SelectedQuantity = x;
		}

		public void InitializeRelativePenWidth2(Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
		{
			_guiPenWidth2.UnitEnvironment = env;
			_guiPenWidth2.SelectedQuantity = x;
		}

		public Units.DimensionfulQuantity PenWidth1
		{
			get
			{
				return _guiPenWidth1.SelectedQuantity;
			}
		}

		public Units.DimensionfulQuantity PenWidth2
		{
			get
			{
				return _guiPenWidth2.SelectedQuantity;
			}
		}

		public void InitializeGapAtStart(Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
		{
			_guiGapAtStart.UnitEnvironment = env;
			_guiGapAtStart.SelectedQuantity = x;
		}

		public Units.DimensionfulQuantity GapAtStart
		{
			get
			{
				return _guiGapAtStart.SelectedQuantity;
			}
		}

		public void InitializeGapAtEnd(Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
		{
			_guiGapAtEnd.UnitEnvironment = env;
			_guiGapAtEnd.SelectedQuantity = x;
		}

		public Units.DimensionfulQuantity GapAtEnd
		{
			get
			{
				return _guiGapAtEnd.SelectedQuantity;
			}
		}

		private void EhIndependentColorChanged(object sender, RoutedEventArgs e)
		{
			IndependentColorChanged?.Invoke();
		}

		private void EhIndependentSkipFreqChanged(object sender, RoutedEventArgs e)
		{
			IndependentSkipFreqChanged?.Invoke();
		}

		public bool ShowPlotColorsOnly
		{
			set
			{
				_penGlue.ShowPlotColorsOnly = value;
			}
		}
	}
}