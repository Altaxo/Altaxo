#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Windows.Controls;

namespace Altaxo.Gui.Analysis.Fourier
{
	/// <summary>
	/// Interaction logic for RealFourierTransformationControl.xaml
	/// </summary>
	public partial class RealFourierTransformation2DControl : UserControl, IRealFourierTransformation2DView
	{
		public RealFourierTransformation2DControl()
		{
			InitializeComponent();
		}

		public void SetOutputQuantities(Collections.SelectableListNodeList list)
		{
			GuiHelper.InitializeChoicePanel<RadioButton>(_guiOutputQuantities, list);
		}

		public double XIncrement
		{
			get
			{
				return _guiXIncrement.SelectedValue;
			}
			set
			{
				_guiXIncrement.SelectedValue = value;
			}
		}

		public double YIncrement
		{
			get
			{
				return _guiYIncrement.SelectedValue;
			}
			set
			{
				_guiYIncrement.SelectedValue = value;
			}
		}

		public void SetXIncrementWarning(string warning)
		{
			_guiXIncrement.ToolTip = warning;
		}

		public void SetYIncrementWarning(string warning)
		{
			_guiYIncrement.ToolTip = warning;
		}

		public bool CenterFrequencies
		{
			get
			{
				return _guiCenterFrequencies.IsChecked == true;
			}
			set
			{
				_guiCenterFrequencies.IsChecked = value;
			}
		}

		public double ResultingFractionOfRowsUsed
		{
			get
			{
				return _guiRowFrequencyFraction.SelectedQuantityAsValueInSIUnits;
			}
			set
			{
				_guiRowFrequencyFraction.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public double ResultingFractionOfColumnsUsed
		{
			get
			{
				return _guiColumnFrequencyFraction.SelectedQuantityAsValueInSIUnits;
			}
			set
			{
				_guiColumnFrequencyFraction.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public int? DataPretreatmentOrder
		{
			get
			{
				if (_guiUseDataPretreatment.IsChecked == true)
					return _guiDataPretreatmentOrder.Value;
				else
					return null;
			}
			set
			{
				if (value.HasValue)
				{
					_guiUseDataPretreatment.IsChecked = true;
					_guiDataPretreatmentOrder.Value = value.Value;
				}
				else
				{
					_guiUseDataPretreatment.IsChecked = false;
					_guiDataPretreatmentOrder.Value = 0;
				}
			}
		}

		public double? ReplacementValueForNaNMatrixElements
		{
			get
			{
				if (_guiUseReplacementValueforNaN.IsChecked == true)
					return _guiReplacementValueForNaN.SelectedValue;
				else
					return null;
			}
			set
			{
				if (value.HasValue)
				{
					_guiReplacementValueForNaN.SelectedValue = value.Value;
					_guiUseReplacementValueforNaN.IsChecked = true;
				}
				else
				{
					_guiReplacementValueForNaN.SelectedValue = 0;
					_guiUseReplacementValueforNaN.IsChecked = false;
				}
			}
		}

		public double? ReplacementValueForInfiniteMatrixElements
		{
			get
			{
				return _guiUseReplacementValueForInfinity.IsChecked == true ? (double?)_guiReplacementValueForInfinite.SelectedValue : null;
			}
			set
			{
				_guiReplacementValueForInfinite.SelectedValue = value.HasValue ? value.Value : 0;
				_guiUseReplacementValueForInfinity.IsChecked = value.HasValue;
			}
		}

		public Collections.SelectableListNodeList FourierWindowChoice
		{
			set { GuiHelper.Initialize(_guiFourierWindow, value); }
		}

		private void EhFourierWindowChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiFourierWindow);
		}

		public bool OutputFrequencyHeaderColumns
		{
			get
			{
				return _guiOutputFrequencyHeaderColumns.IsChecked == true;
			}
			set
			{
				_guiOutputFrequencyHeaderColumns.IsChecked = value;
			}
		}

		public string FrequencyRowHeaderColumnName
		{
			get
			{
				return _guiFrequencyRowHeaderColumnName.Text;
			}
			set
			{
				_guiFrequencyRowHeaderColumnName.Text = value;
			}
		}

		public string FrequencyColumnHeaderColumnName
		{
			get
			{
				return _guiFrequencyColumnHeaderColumnName.Text;
			}
			set
			{
				_guiFrequencyColumnHeaderColumnName.Text = value;
			}
		}

		public bool OutputPeriodHeaderColumns
		{
			get
			{
				return _guiOutputPeriodHeaderColumns.IsChecked == true;
			}
			set
			{
				_guiOutputPeriodHeaderColumns.IsChecked = value;
			}
		}

		public string PeriodRowHeaderColumnName
		{
			get
			{
				return _guiPeriodRowHeaderColumnName.Text;
			}
			set
			{
				_guiPeriodRowHeaderColumnName.Text = value;
			}
		}

		public string PeriodColumnHeaderColumnName
		{
			get
			{
				return _guiPeriodColumnHeaderColumnName.Text;
			}
			set
			{
				_guiPeriodColumnHeaderColumnName.Text = value;
			}
		}
	}
}