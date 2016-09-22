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

namespace Altaxo.Gui.Graph.Graph3D.Lighting
{
	/// <summary>
	/// Interaction logic for HemisphericAmbientLightControl.xaml
	/// </summary>
	public partial class SpotLightControl : UserControl, IDiscreteLightControl
	{
		public event EventHandler SelectedValueChanged;

		private double _lightAmplitude;

		private double _lightRange;

		private double _innerConeAngle, _outerConeAngle;

		private GuiChangeLocker _lock;

		public SpotLightControl()
		{
			InitializeComponent();
		}

		private void EhColorChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		public Altaxo.Graph.Graph3D.Lighting.SpotLight SelectedValue
		{
			get
			{
				return new Altaxo.Graph.Graph3D.Lighting.SpotLight(
					_lightAmplitude,
					_guiColor.SelectedColor,
					_guiPosition.SelectedValue,
					_guiDirection.SelectedValue,
					_lightRange,
					_outerConeAngle * Math.PI / 180,
					_innerConeAngle * Math.PI / 180,
					_guiAttachedToCamera.IsChecked == true
				);
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				_lock.ExecuteLocked(
					() =>
					{
						_lightAmplitude = value.LightAmplitude;
						if (_guiLightAmplitudeSlider.Maximum < _lightAmplitude)
							_guiLightAmplitudeSlider.Maximum = _lightAmplitude;
						_guiLightAmplitudeSlider.Value = _lightAmplitude;
						_guiLightAmplitudeBox.SelectedValue = _lightAmplitude;

						_guiColor.SelectedColor = value.Color;
						_guiPosition.SelectedValue = value.Position;
						_guiDirection.SelectedValue = value.DirectionToLight;

						_lightRange = value.Range;
						if (_guiLightRangeSlider.Maximum < _lightRange)
							_guiLightRangeSlider.Maximum = _lightRange;
						_guiLightRangeBox.SelectedValue = _lightRange;
						_guiLightRangeSlider.Value = _lightRange;

						_outerConeAngle = 180 * value.OuterConeAngle / Math.PI;
						_innerConeAngle = 180 * value.InnerConeAngle / Math.PI;

						_guiInnerConeAngleBox.SelectedValue = _innerConeAngle;
						_guiInnerConeAngleSlider.Value = _innerConeAngle;

						_guiOuterConeAngleBox.SelectedValue = _outerConeAngle;
						_guiOuterConeAngleSlider.Value = _outerConeAngle;

						_guiAttachedToCamera.IsChecked = value.IsAffixedToCamera;
					});
			}
		}

		public Altaxo.Graph.Graph3D.Lighting.IDiscreteLight SelectedValueAsIDiscreteLight
		{
			get
			{
				return SelectedValue;
			}
		}

		protected virtual void OnSelectedValueChanged()
		{
			if (_lock.IsNotLocked)
				SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhAttachedToCameraChanged(object sender, RoutedEventArgs e)
		{
			OnSelectedValueChanged();
		}

		private void EhLightAmplitudeBoxChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_lightAmplitude = _guiLightAmplitudeBox.SelectedValue;

			if (_lightAmplitude > _guiLightAmplitudeSlider.Maximum)
				_guiLightAmplitudeSlider.Maximum = _lightAmplitude;

			_guiLightAmplitudeSlider.Value = _lightAmplitude;

			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhLightAmplitudeSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_lightAmplitude = _guiLightAmplitudeSlider.Value;
			_guiLightAmplitudeBox.SelectedValue = _lightAmplitude;

			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhPositionChanged(object sender, EventArgs e)
		{
			OnSelectedValueChanged();
		}

		private void EhDirectionChanged(object sender, EventArgs e)
		{
			OnSelectedValueChanged();
		}

		private void EhLightRangeBoxChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_lock.ExecuteLockedButOnlyIfNotLockedBefore(
				() =>
				{
					_lightRange = _guiLightRangeBox.SelectedValue;
					if (_guiLightRangeSlider.Maximum < _lightRange)
						_guiLightRangeSlider.Maximum = _lightRange;
					_guiLightRangeSlider.Value = _lightRange;
				},
					() => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
				);
		}

		private void EhLighRangeSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_lock.ExecuteLockedButOnlyIfNotLockedBefore(
				() =>
				{
					_lightRange = _guiLightRangeSlider.Value;
					_guiLightRangeBox.SelectedValue = _lightRange;
				},
					() => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
				);
		}

		private void EhInnerConeAngleBoxChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_lock.ExecuteLockedButOnlyIfNotLockedBefore(
				() =>
				{
					_innerConeAngle = _guiInnerConeAngleBox.SelectedValue;
					_guiInnerConeAngleSlider.Value = _innerConeAngle;

					if (_outerConeAngle < _innerConeAngle)
					{
						_outerConeAngle = _innerConeAngle;
						_guiOuterConeAngleBox.SelectedValue = _outerConeAngle;
						_guiOuterConeAngleSlider.Value = _outerConeAngle;
					}
				},
					() => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
				);
		}

		private void EhInnerConeAngleSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_lock.ExecuteLockedButOnlyIfNotLockedBefore(
				() =>
				{
					_innerConeAngle = _guiInnerConeAngleSlider.Value;
					_guiInnerConeAngleBox.SelectedValue = _innerConeAngle;

					if (_outerConeAngle < _innerConeAngle)
					{
						_outerConeAngle = _innerConeAngle;
						_guiOuterConeAngleBox.SelectedValue = _outerConeAngle;
						_guiOuterConeAngleSlider.Value = _outerConeAngle;
					}
				},
					() => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
				);
		}

		private void EhOuterConeAngleBoxChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_lock.ExecuteLockedButOnlyIfNotLockedBefore(
				() =>
				{
					_outerConeAngle = _guiOuterConeAngleBox.SelectedValue;
					_guiOuterConeAngleSlider.Value = _outerConeAngle;

					if (_outerConeAngle < _innerConeAngle)
					{
						_innerConeAngle = _outerConeAngle;
						_guiInnerConeAngleBox.SelectedValue = _innerConeAngle;
						_guiInnerConeAngleSlider.Value = _innerConeAngle;
					}
				},
					() => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
				);
		}

		private void EhOuterConeAngleSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_lock.ExecuteLockedButOnlyIfNotLockedBefore(
					() =>
					{
						_outerConeAngle = _guiOuterConeAngleSlider.Value;
						_guiOuterConeAngleBox.SelectedValue = _outerConeAngle;

						if (_outerConeAngle < _innerConeAngle)
						{
							_innerConeAngle = _outerConeAngle;
							_guiInnerConeAngleBox.SelectedValue = _innerConeAngle;
							_guiInnerConeAngleSlider.Value = _innerConeAngle;
						}
					},
					() => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
				);
		}
	}
}