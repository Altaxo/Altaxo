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

namespace Altaxo.Gui.Graph.Shapes
{
	/// <summary>
	/// Interaction logic for FloatingScaleControl.xaml
	/// </summary>
	public partial class FloatingScaleControl : UserControl, IFloatingScaleView
	{
		public FloatingScaleControl()
		{
			InitializeComponent();
		}

		public Altaxo.Graph.PointD2D DocPosition
		{
			get
			{
				var x = _edPositionX.SelectedQuantity.AsValueIn(Units.Length.Point.Instance);
				var y = _edPositionY.SelectedQuantity.AsValueIn(Units.Length.Point.Instance);
				return new Altaxo.Graph.PointD2D(x, y);
			}
			set
			{
				_edPositionX.SelectedQuantity = new Units.DimensionfulQuantity(value.X, Units.Length.Point.Instance).AsQuantityIn(_edPositionX.UnitEnvironment.DefaultUnit);
				_edPositionY.SelectedQuantity = new Units.DimensionfulQuantity(value.Y, Units.Length.Point.Instance).AsQuantityIn(_edPositionY.UnitEnvironment.DefaultUnit);
			}
		}

		public int ScaleNumber
		{
			get
			{
				if (_guiScale0.IsChecked == true)
					return 0;
				else
					return 1;
			}
			set
			{
				_guiScale0.IsChecked = (0 == value);
				_guiScale1.IsChecked = (1 == value);
			}
		}

		public double ScaleSpan
		{
			get
			{
				if (ScaleSpanIsPhysicalValue)
					return _lastConvertedScaleSpan;
				else
					return _guiLogicalScaleSpan.SelectedQuantityAsValueInSIUnits;
			}
			set
			{
				_lastConvertedScaleSpan = value;
				_guiScaleSpan.Text = value.ToString(System.Globalization.CultureInfo.CurrentUICulture);
				_guiLogicalScaleSpan.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public bool ScaleSpanIsPhysicalValue
		{
			get
			{
				return _guiIsPhysicalValue.IsChecked == true;
			}
			set
			{
				_guiIsPhysicalValue.IsChecked = value;
				_guiIsLogicalValue.IsChecked = !value;
			}
		}

		public IXYAxisLabelStyleView LabelStyleView
		{
			get { return _guiLabelStyleView; }
		}

		double _lastConvertedScaleSpan;
		private void EhScaleSpanValidating(object sender, ValidationEventArgs<string> e)
		{
			if (!double.TryParse(e.ValueToValidate, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentUICulture, out _lastConvertedScaleSpan))
			{
				e.AddError("The entered text could not be converted to a numeric value");
				return;
			}
		}
	}
}
