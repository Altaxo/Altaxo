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

namespace Altaxo.Gui.Graph3D.Lighting
{
	/// <summary>
	/// Interaction logic for HemisphericAmbientLightControl.xaml
	/// </summary>
	public partial class DirectionalLightControl : UserControl, IDiscreteLightControl
	{
		public event EventHandler SelectedValueChanged;

		private double _lightAmplitude;

		public DirectionalLightControl()
		{
			InitializeComponent();
		}

		private void EhColorChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		public Altaxo.Graph.Graph3D.Lighting.DirectionalLight SelectedValue
		{
			get
			{
				return new Altaxo.Graph.Graph3D.Lighting.DirectionalLight(
					_lightAmplitude,
					_guiColor.SelectedColor,
					_guiDirection.SelectedValue,
					_guiAttachedToCamera.IsChecked == true
				);
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				_lightAmplitude = value.LightAmplitude;
				_guiLightAmplitudeSlider.Value = _lightAmplitude;
				_guiLightAmplitudeBox.SelectedValue = _lightAmplitude;

				_guiColor.SelectedColor = value.Color;
				_guiDirection.SelectedValue = value.DirectionFromLight;
				_guiAttachedToCamera.IsChecked = value.IsAffixedToCamera;
			}
		}

		public Altaxo.Graph.Graph3D.Lighting.IDiscreteLight SelectedValueAsIDiscreteLight
		{
			get
			{
				return SelectedValue;
			}
		}

		private void EhAttachedToCameraChanged(object sender, RoutedEventArgs e)
		{
			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhLightAmplitudeBoxChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_lightAmplitude = _guiLightAmplitudeBox.SelectedValue;
			_guiLightAmplitudeSlider.Value = _lightAmplitude;

			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhLightAmplitudeSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_lightAmplitude = _guiLightAmplitudeSlider.Value;
			_guiLightAmplitudeBox.SelectedValue = _lightAmplitude;

			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhDirectionChanged(object sender, EventArgs e)
		{
			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}