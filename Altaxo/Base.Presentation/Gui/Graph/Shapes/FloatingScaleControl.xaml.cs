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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Shapes
{
	using Altaxo.Graph.Gdi.Shapes;
	using Altaxo.Gui.Common;
	using Geometry;

	/// <summary>
	/// Interaction logic for FloatingScaleControl.xaml
	/// </summary>
	public partial class FloatingScaleControl : UserControl, IFloatingScaleView
	{
		private BackgroundControlsGlue _backgroundGlue;

		public event Action TickSpacingTypeChanged;

		public FloatingScaleControl()
		{
			InitializeComponent();

			_backgroundGlue = new BackgroundControlsGlue();
			_backgroundGlue.CbStyle = _guiBackgroundStyle;
			_backgroundGlue.CbBrush = _guiBackgroundBrush;
		}

		public PointD2D DocPosition
		{
			get
			{
				var x = _edPositionX.SelectedQuantity.AsValueIn(Units.Length.Point.Instance);
				var y = _edPositionY.SelectedQuantity.AsValueIn(Units.Length.Point.Instance);
				return new PointD2D(x, y);
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

		public double ScaleSpanValue
		{
			get
			{
				switch (ScaleSpanType)
				{
					default:
					case FloatingScaleSpanType.IsLogicalValue:
						return _guiLogicalScaleSpan.SelectedQuantityAsValueInSIUnits;

					case FloatingScaleSpanType.IsPhysicalEndOrgDifference:
						return _guiSpanDifferenceValue.SelectedValue;

					case FloatingScaleSpanType.IsPhysicalEndOrgRatio:
						return _guiSpanRatioValue.SelectedValue;
				}
			}
			set
			{
				switch (ScaleSpanType)
				{
					case FloatingScaleSpanType.IsLogicalValue:
						_guiLogicalScaleSpan.SelectedQuantityAsValueInSIUnits = value;
						break;

					case FloatingScaleSpanType.IsPhysicalEndOrgDifference:
						_guiSpanDifferenceValue.SelectedValue = value;
						break;

					case FloatingScaleSpanType.IsPhysicalEndOrgRatio:
						_guiSpanRatioValue.SelectedValue = value;
						break;
				}
			}
		}

		public FloatingScaleSpanType ScaleSpanType
		{
			get
			{
				if (_guiIsLogicalValue.IsChecked == true)
					return FloatingScaleSpanType.IsLogicalValue;
				else if (_guiIsPhysicalEndOrgDifference.IsChecked == true)
					return FloatingScaleSpanType.IsPhysicalEndOrgDifference;
				else
					return FloatingScaleSpanType.IsPhysicalEndOrgRatio;
			}
			set
			{
				_guiIsLogicalValue.IsChecked = value == FloatingScaleSpanType.IsLogicalValue;
				_guiIsPhysicalEndOrgDifference.IsChecked = value == FloatingScaleSpanType.IsPhysicalEndOrgDifference;
				_guiIsPhysicalEndOrgRatio.IsChecked = value == FloatingScaleSpanType.IsPhysicalEndOrgRatio;
			}
		}

		public object TitleFormatView
		{
			set
			{
				_guiTabTitleFormat.Content = value;
			}
		}

		public IConditionalDocumentView MajorLabelView
		{
			set
			{
				_guiTabMajorLabels.Content = value;
			}
		}

		public IConditionalDocumentView MinorLabelView
		{
			set
			{
				_guiTabMinorLabels.Content = value;
			}
		}

		private double _lastConvertedScaleSpan;

		private void EhScaleSpanValidating(object sender, ValidationEventArgs<string> e)
		{
			if (!double.TryParse(e.ValueToValidate, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentUICulture, out _lastConvertedScaleSpan))
			{
				e.AddError("The entered text could not be converted to a numeric value");
				return;
			}
		}

		private void EhTickSpacingType_SelectionChange(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != TickSpacingTypeChanged)
			{
				ComboBox _cbTickSpacingType = (ComboBox)sender;
				GuiHelper.SynchronizeSelectionFromGui(_cbTickSpacingType);
				TickSpacingTypeChanged();
			}
		}

		public Altaxo.Graph.Gdi.Shapes.FloatingScale.ScaleSegmentType ScaleSegmentType
		{
			get
			{
				if (true == _guiScaleTypeRatio.IsChecked)
					return Altaxo.Graph.Gdi.Shapes.FloatingScale.ScaleSegmentType.RatioToOrg;
				else if (true == _guiScaleTypeDifference.IsChecked)
					return Altaxo.Graph.Gdi.Shapes.FloatingScale.ScaleSegmentType.DifferenceToOrg;
				else
					return Altaxo.Graph.Gdi.Shapes.FloatingScale.ScaleSegmentType.Normal;
			}
			set
			{
				_guiScaleTypeRatio.IsChecked = (value == Altaxo.Graph.Gdi.Shapes.FloatingScale.ScaleSegmentType.RatioToOrg);
				_guiScaleTypeDifference.IsChecked = (value == Altaxo.Graph.Gdi.Shapes.FloatingScale.ScaleSegmentType.DifferenceToOrg);
				_guiScaleTypeNormal.IsChecked = (value == Altaxo.Graph.Gdi.Shapes.FloatingScale.ScaleSegmentType.Normal);
			}
		}

		public object TickSpacingView
		{
			set { _guiTickSpacingGroupBox.Content = value; }
		}

		public void InitializeTickSpacingTypes(Collections.SelectableListNodeList names)
		{
			GuiHelper.Initialize(_guiTickSpacingTypes, names);
		}

		public Altaxo.Graph.Gdi.Background.IBackgroundStyle SelectedBackground
		{
			get
			{
				return _backgroundGlue.BackgroundStyle;
			}
			set
			{
				_backgroundGlue.BackgroundStyle = value;
			}
		}

		public Margin2D BackgroundPadding
		{
			get
			{
				var result = new Margin2D();
				result.Left = _guiMarginLeft.SelectedQuantity.AsValueIn(Units.Length.Point.Instance);
				result.Top = _guiMarginTop.SelectedQuantity.AsValueIn(Units.Length.Point.Instance);
				result.Right = _guiMarginRight.SelectedQuantity.AsValueIn(Units.Length.Point.Instance);
				result.Bottom = _guiMarginBottom.SelectedQuantity.AsValueIn(Units.Length.Point.Instance);
				return result;
			}
			set
			{
				_guiMarginLeft.SelectedQuantity = new Units.DimensionfulQuantity(value.Left, Units.Length.Point.Instance).AsQuantityIn(_guiMarginLeft.UnitEnvironment.DefaultUnit);
				_guiMarginTop.SelectedQuantity = new Units.DimensionfulQuantity(value.Top, Units.Length.Point.Instance).AsQuantityIn(_guiMarginTop.UnitEnvironment.DefaultUnit);
				_guiMarginRight.SelectedQuantity = new Units.DimensionfulQuantity(value.Right, Units.Length.Point.Instance).AsQuantityIn(_guiMarginRight.UnitEnvironment.DefaultUnit);
				_guiMarginBottom.SelectedQuantity = new Units.DimensionfulQuantity(value.Bottom, Units.Length.Point.Instance).AsQuantityIn(_guiMarginBottom.UnitEnvironment.DefaultUnit);
			}
		}
	}
}