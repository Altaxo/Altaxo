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

using Altaxo.Geometry;
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

namespace Altaxo.Gui.Geometry
{
	/// <summary>
	/// Interaction logic for Direction3DSphericalControl.xaml
	/// </summary>
	public partial class Position3DSphericalControl : UserControl
	{
		private double _polarAngleDegrees;
		private double _elevationAngleDegrees;
		private double _distance;

		public event EventHandler SelectedValueChanged;

		public Position3DSphericalControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets or sets the direction. The direction value is normalized.
		/// </summary>
		/// <value>
		/// The direction.
		/// </value>
		public PointD3D SelectedValue
		{
			get
			{
				double polarAngleRadians = _polarAngleDegrees * Math.PI / 180;
				double elevationAngleRadians = _elevationAngleDegrees * Math.PI / 180;
				return new PointD3D(
					_distance * Math.Cos(polarAngleRadians) * Math.Cos(elevationAngleRadians),
					_distance * Math.Sin(polarAngleRadians) * Math.Cos(elevationAngleRadians),
					_distance * Math.Sin(elevationAngleRadians)
					);
			}
			set
			{
				_distance = ((VectorD3D)value).Length;

				if (_guiDistanceSlider.Maximum < _distance)
					_guiDistanceSlider.Maximum = _distance;
				_guiDistanceBox.SelectedValue = _distance;
				_guiDistanceSlider.Value = _distance;

				if (_distance > 0)
				{
					_elevationAngleDegrees = Math.Asin(value.Z / _distance) * 180 / Math.PI;
					_polarAngleDegrees = Math.Atan2(value.Y, value.X) * 180 / Math.PI;
				}
				else
				{
					_elevationAngleDegrees = 0;
					_polarAngleDegrees = 0;
				}

				_guiPolarAngleBox.SelectedValue = _polarAngleDegrees;
				_guiElevationAngleBox.SelectedValue = _elevationAngleDegrees;
			}
		}

		private void EhPolarAngleBoxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_polarAngleDegrees = _guiPolarAngleBox.SelectedValue;
			_guiPolarAngleSlider.Value = _polarAngleDegrees;

			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhPolarAngleSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_polarAngleDegrees = _guiPolarAngleSlider.Value;
			_guiPolarAngleBox.SelectedValue = _polarAngleDegrees;

			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhPolarAzimuthBoxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_elevationAngleDegrees = _guiElevationAngleBox.SelectedValue;
			_guiElevationAngleSlider.Value = _elevationAngleDegrees;

			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhAzimuthAngleSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_elevationAngleDegrees = _guiElevationAngleSlider.Value;
			_guiElevationAngleBox.SelectedValue = _elevationAngleDegrees;

			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhDistanceBoxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_distance = _guiDistanceBox.SelectedValue;
			if (_guiDistanceSlider.Maximum < _distance)
				_guiDistanceSlider.Maximum = _distance;
			_guiDistanceSlider.Value = _distance;

			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhDistanceSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_distance = _guiDistanceSlider.Value;
			_guiDistanceBox.SelectedValue = _distance;

			SelectedValueChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}