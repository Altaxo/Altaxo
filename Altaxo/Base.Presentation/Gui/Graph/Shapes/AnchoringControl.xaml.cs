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

using Altaxo.Graph;
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
	using Altaxo.Units;

	/// <summary>
	/// Interaction logic for AnchoringControl.xaml
	/// </summary>
	public partial class AnchoringControl : UserControl
	{
		private RadioButton[,] _buttons;

		private RADouble _pivotX, _pivotY;
		private bool _useRadioGrid;

		private QuantityWithUnitGuiEnvironment _xSizeEnvironment, _ySizeEnvironment;

		private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("% X-Size", "%", new DimensionfulQuantity(1, Units.Length.Point.Instance));
		private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("% Y-Size", "%", new DimensionfulQuantity(1, Units.Length.Point.Instance));

		public AnchoringControl()
		{
			InitializeComponent();
			_buttons = new RadioButton[3, 3] { { _guiLeftTop, _guiCenterTop, _guiRightTop }, { _guiLeftCenter, _guiCenterCenter, _guiRightCenter }, { _guiLeftBottom, _guiCenterBottom, _guiRightBottom } };
			SetRadioButton();
		}

		/// <summary>
		/// Sets the title of the group box.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		public string Title
		{
			set
			{
				_guiMainBox.Header = value;
			}
		}

		/// <summary>
		/// Sets the selected pivot values for X and Y. Additionally, the reference size is required.
		/// </summary>
		/// <param name="pivotX">The pivot x value.</param>
		/// <param name="pivotY">The pivot y value.</param>
		/// <param name="referenceSize">Size of the reference area.</param>
		public void SetSelectedPivot(RADouble pivotX, RADouble pivotY, PointD2D referenceSize)
		{
			_pivotX = pivotX;
			_pivotY = pivotY;

			_percentLayerXSizeUnit.ReferenceQuantity = new DimensionfulQuantity(referenceSize.X, Units.Length.Point.Instance);
			_percentLayerYSizeUnit.ReferenceQuantity = new DimensionfulQuantity(referenceSize.Y, Units.Length.Point.Instance);
			_xSizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerXSizeUnit);
			_ySizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerYSizeUnit);

			_guiPivotX.UnitEnvironment = _xSizeEnvironment;
			_guiPivotY.UnitEnvironment = _ySizeEnvironment;
			_guiPivotX.SelectedQuantity = _pivotX.IsAbsolute ? new Units.DimensionfulQuantity(_pivotX.Value, Units.Length.Point.Instance) : new Units.DimensionfulQuantity(_pivotX.Value * 100, _percentLayerXSizeUnit);
			_guiPivotY.SelectedQuantity = _pivotY.IsAbsolute ? new Units.DimensionfulQuantity(_pivotY.Value, Units.Length.Point.Instance) : new Units.DimensionfulQuantity(_pivotY.Value * 100, _percentLayerYSizeUnit);

			if (CanUseRadioGridView())
			{
				SetRadioButton();
				SetUseOfRadioGrid(true);
			}
			else
			{
				SetUseOfRadioGrid(false);
			}
			SetVisibilityOfSwitchButton();
		}

		/// <summary>
		/// Gets the selected pivot x value.
		/// </summary>
		/// <value>
		/// The selected pivot x value.
		/// </value>
		public RADouble SelectedPivotX
		{
			get
			{
				return _pivotX;
			}
		}

		/// <summary>
		/// Gets the selected pivot y value.
		/// </summary>
		/// <value>
		/// The selected pivot y value.
		/// </value>
		public RADouble SelectedPivotY
		{
			get
			{
				return _pivotY;
			}
		}

		private void EhRadioChecked(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < 3; ++i)
			{
				for (int j = 0; j < 3; ++j)
				{
					if (object.Equals(sender, _buttons[j, i]))
					{
						_pivotX = RADouble.NewRel(0.5 * i);
						_pivotY = RADouble.NewRel(0.5 * j);
					}
				}
			}
		}

		private void SetRadioButton()
		{
			int i = (int)(_pivotX.Value * 2);
			int j = (int)(_pivotY.Value * 2);
			_buttons[j, i].IsChecked = true;
		}

		private void SetUseOfRadioGrid(bool useRadioGrid)
		{
			_useRadioGrid = useRadioGrid;
			if (useRadioGrid)
			{
				_guiRadioGridView.Visibility = System.Windows.Visibility.Visible;
				_guiNumericView.Visibility = System.Windows.Visibility.Collapsed;
			}
			else
			{
				_guiRadioGridView.Visibility = System.Windows.Visibility.Collapsed;
				_guiNumericView.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void SetVisibilityOfSwitchButton()
		{
			if (_useRadioGrid)
			{
				_guiSwitchToNumericView.Visibility = System.Windows.Visibility.Visible;
				_guiSwitchToRadioView.Visibility = System.Windows.Visibility.Collapsed;
			}
			else // currently in numeric view
			{
				_guiSwitchToNumericView.Visibility = System.Windows.Visibility.Collapsed;
				_guiSwitchToRadioView.Visibility = System.Windows.Visibility.Visible;
				_guiSwitchToRadioView.IsEnabled = CanUseRadioGridView();
			}
		}

		private bool CanUseRadioGridView()
		{
			bool useRadioView = true;
			useRadioView &= _pivotX.IsRelative && (_pivotX.Value == 0 || _pivotX.Value == 0.5 || _pivotX.Value == 1);
			useRadioView &= _pivotY.IsRelative && (_pivotY.Value == 0 || _pivotY.Value == 0.5 || _pivotY.Value == 1);
			return useRadioView;
		}

		private void EhNumericPivotXChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var quant = _guiPivotX.SelectedQuantity;
			if (object.ReferenceEquals(quant.Unit, _percentLayerXSizeUnit))
				_pivotX = RADouble.NewRel(quant.Value / 100);
			else
				_pivotX = RADouble.NewAbs(quant.AsValueIn(Units.Length.Point.Instance));

			SetVisibilityOfSwitchButton();
		}

		private void EhNumericPivotYChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var quant = _guiPivotY.SelectedQuantity;
			if (object.ReferenceEquals(quant.Unit, _percentLayerYSizeUnit))
				_pivotY = RADouble.NewRel(quant.Value / 100);
			else
				_pivotY = RADouble.NewAbs(quant.AsValueIn(Units.Length.Point.Instance));

			SetVisibilityOfSwitchButton();
		}

		private void EhSwitchToNumericView(object sender, RoutedEventArgs e)
		{
			SetUseOfRadioGrid(false);
			SetVisibilityOfSwitchButton();
		}

		private void EhSwitchToGraphicalView(object sender, RoutedEventArgs e)
		{
			SetUseOfRadioGrid(true);
			SetVisibilityOfSwitchButton();
		}
	}
}