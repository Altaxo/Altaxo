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
using Altaxo.Gui.Drawing.D3D;
using Altaxo.Gui.Graph.Graph3D.Plot.Data;
using Altaxo.Gui.Graph.Plot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
	/// <summary>
	/// Interaction logic for ErrorBarPlotStyleControl.xaml
	/// </summary>
	public partial class ErrorBarPlotStyleControl : UserControl, IErrorBarPlotStyleView
	{
		private PenControlsGlue _strokePenGlue;

		public event Action IndependentColorChanged;

		public event Action IndependentDashPatternChanged;

		public event Action<bool> UseCommonErrorColumnChanged;

		public ErrorBarPlotStyleControl()
		{
			InitializeComponent();

			_strokePenGlue = new PenControlsGlue();
			_strokePenGlue.CbBrush = _guiPenColor;
			_strokePenGlue.CbLineEndCap = _guiLineEndCap;
			_strokePenGlue.CbDashPattern = _guiDashPattern;
		}

		#region IErrorBarPlotStyleView Members

		public bool IndependentColor
		{
			get
			{
				return true == _chkIndependentColor.IsChecked;
			}
			set
			{
				_chkIndependentColor.IsChecked = value;
			}
		}

		public bool IndependentDashPattern
		{
			get
			{
				return true == _chkIndependentDashPattern.IsChecked;
			}
			set
			{
				_chkIndependentDashPattern.IsChecked = value;
			}
		}

		public PenX3D Pen
		{
			get
			{
				return _strokePenGlue.Pen;
			}
			set
			{
				_strokePenGlue.Pen = value;
			}
		}

		public bool IndependentOnShiftingGroupStyles
		{
			get
			{
				return true == _guiIndependentOnShiftingGroupStyles.IsChecked;
			}
			set
			{
				_guiIndependentOnShiftingGroupStyles.IsChecked = value;
			}
		}

		public int SkipFrequency
		{
			get
			{
				return _edSkipFrequency.Value;
			}
			set
			{
				_edSkipFrequency.Value = value;
			}
		}

		#endregion IErrorBarPlotStyleView Members

		public bool ShowPlotColorsOnly
		{
			set { _strokePenGlue.ShowPlotColorsOnly = value; }
		}

		public bool IndependentSymbolSize
		{
			get
			{
				return true == _guiIndependentSymbolSize.IsChecked;
			}

			set
			{
				_guiIndependentSymbolSize.IsChecked = value;
			}
		}

		public double SymbolSize
		{
			get
			{
				return _guiSymbolSize.SelectedQuantityAsValueInPoints;
			}
			set
			{
				_guiSymbolSize.SelectedQuantityAsValueInPoints = value;
			}
		}

		public double LineWidth1Offset
		{
			get
			{
				return _guiLineWidth1Offset.SelectedQuantityAsValueInPoints;
			}

			set
			{
				_guiLineWidth1Offset.SelectedQuantityAsValueInPoints = value;
			}
		}

		public double LineWidth1Factor
		{
			get
			{
				return _guiLineWidth1Factor.SelectedQuantityAsValueInSIUnits;
			}

			set
			{
				_guiLineWidth1Factor.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public double LineWidth2Offset
		{
			get
			{
				return _guiLineWidth2Offset.SelectedQuantityAsValueInPoints;
			}

			set
			{
				_guiLineWidth2Offset.SelectedQuantityAsValueInPoints = value;
			}
		}

		public double LineWidth2Factor
		{
			get
			{
				return _guiLineWidth2Factor.SelectedQuantityAsValueInSIUnits;
			}

			set
			{
				_guiLineWidth2Factor.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public double EndCapSizeOffset
		{
			get
			{
				return _guiEndCapSizeOffset.SelectedQuantityAsValueInPoints;
			}

			set
			{
				_guiEndCapSizeOffset.SelectedQuantityAsValueInPoints = value;
			}
		}

		public double EndCapSizeFactor
		{
			get
			{
				return _guiEndCapSizeFactor.SelectedQuantityAsValueInSIUnits;
			}

			set
			{
				_guiEndCapSizeFactor.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public bool UseSymbolGap
		{
			get
			{
				return _guiUseLineSymbolGap.IsChecked == true;
			}

			set
			{
				_guiUseLineSymbolGap.IsChecked = value;
			}
		}

		public double SymbolGapOffset
		{
			get
			{
				return _guiSymbolGapOffset.SelectedQuantityAsValueInPoints;
			}

			set
			{
				_guiSymbolGapOffset.SelectedQuantityAsValueInPoints = value;
			}
		}

		public double SymbolGapFactor
		{
			get
			{
				return _guiSymbolGapFactor.SelectedQuantityAsValueInSIUnits;
			}

			set
			{
				_guiSymbolGapFactor.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public bool ForceVisibilityOfEndCap
		{
			get
			{
				return _guiForceVisibilityOfEndCap.IsChecked == true;
			}
			set
			{
				_guiForceVisibilityOfEndCap.IsChecked = value;
			}
		}

		public bool IndependentSkipFrequency
		{
			get
			{
				return _guiIndependentSkipFrequency.IsChecked == true;
			}

			set
			{
				_guiIndependentSkipFrequency.IsChecked = value;
			}
		}

		private void EhIndependentColorChanged(object sender, RoutedEventArgs e)
		{
			IndependentColorChanged?.Invoke();
		}

		private void EhIndependentDashPatternChanged(object sender, RoutedEventArgs e)
		{
			IndependentDashPatternChanged?.Invoke();
		}

		private void EhUseCommonErrorColumnCheckedChanged(object sender, RoutedEventArgs e)
		{
			UseCommonErrorColumnChanged?.Invoke(_guiUseCommonErrorColumn.IsChecked == true);
		}

		public bool UseCommonErrorColumn
		{
			get
			{
				return _guiUseCommonErrorColumn.IsChecked == true;
			}
			set
			{
				_guiUseCommonErrorColumn.IsChecked = value;

				var commonVisibility = value ? Visibility.Visible : Visibility.Collapsed;
				var posnegVisibility = value ? Visibility.Collapsed : Visibility.Visible;

				_guiCommonErrorColumnLabel.Visibility = commonVisibility;
				_guiCommonErrorColumn.Visibility = commonVisibility;
				_guiCommonErrorColumnTransformation.Visibility = commonVisibility;

				_guiPositiveErrorColumnLabel.Visibility = posnegVisibility;
				_guiPositiveErrorColumn.Visibility = posnegVisibility;
				_guiPositiveErrorColumnTransformation.Visibility = posnegVisibility;

				_guiNegativeErrorColumnLabel.Visibility = posnegVisibility;
				_guiNegativeErrorColumn.Visibility = posnegVisibility;
				_guiNegativeErrorColumnTransformation.Visibility = posnegVisibility;
			}
		}

		public void Initialize_CommonErrorColumn(string boxText, string toolTip, int status)
		{
			this._guiCommonErrorColumn.Text = boxText;
			this._guiCommonErrorColumn.ToolTip = toolTip;
			this._guiCommonErrorColumn.Background = DefaultSeverityColumnColors.GetSeverityColor(status);
		}

		public void Initialize_CommonErrorColumnTransformation(string transformationTextToShow, string transformationToolTip)
		{
			if (null == transformationTextToShow)
			{
				this._guiCommonErrorColumnTransformation.Visibility = Visibility.Collapsed;
			}
			else
			{
				this._guiCommonErrorColumnTransformation.Text = transformationTextToShow;
				this._guiCommonErrorColumnTransformation.ToolTip = transformationToolTip;
				this._guiCommonErrorColumnTransformation.Visibility = Visibility.Visible;
			}
		}

		public void Initialize_PositiveErrorColumn(string boxText, string toolTip, int status)
		{
			this._guiPositiveErrorColumn.Text = boxText;
			this._guiPositiveErrorColumn.ToolTip = toolTip;
			this._guiPositiveErrorColumn.Background = DefaultSeverityColumnColors.GetSeverityColor(status);
		}

		public void Initialize_PositiveErrorColumnTransformation(string transformationTextToShow, string transformationToolTip)
		{
			if (null == transformationTextToShow)
			{
				this._guiPositiveErrorColumnTransformation.Visibility = Visibility.Collapsed;
			}
			else
			{
				this._guiPositiveErrorColumnTransformation.Text = transformationTextToShow;
				this._guiPositiveErrorColumnTransformation.ToolTip = transformationToolTip;
				this._guiPositiveErrorColumnTransformation.Visibility = Visibility.Visible;
			}
		}

		public void Initialize_NegativeErrorColumn(string boxText, string toolTip, int status)
		{
			this._guiNegativeErrorColumn.Text = boxText;
			this._guiNegativeErrorColumn.ToolTip = toolTip;
			this._guiNegativeErrorColumn.Background = DefaultSeverityColumnColors.GetSeverityColor(status);
		}

		public void Initialize_NegativeErrorColumnTransformation(string transformationTextToShow, string transformationToolTip)
		{
			if (null == transformationTextToShow)
			{
				this._guiNegativeErrorColumnTransformation.Visibility = Visibility.Collapsed;
			}
			else
			{
				this._guiNegativeErrorColumnTransformation.Text = transformationTextToShow;
				this._guiNegativeErrorColumnTransformation.ToolTip = transformationToolTip;
				this._guiNegativeErrorColumnTransformation.Visibility = Visibility.Visible;
			}
		}

		public void Initialize_MeaningOfValues(SelectableListNodeList list)
		{
			_guiMeaningOfValues.Initialize(list);
		}
	}
}