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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Altaxo.Geometry;

namespace Altaxo.Gui.Geometry
{
  /// <summary>
  /// Interaction logic for Direction3DSphericalControl.xaml
  /// </summary>
  public partial class Direction3DSphericalControl : UserControl
  {
    private double _polarAngleDegrees;
    private double _elevationAngleDegrees;

    public event EventHandler SelectedValueChanged;

    private GuiChangeLocker _lock;

    public Direction3DSphericalControl()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the direction. The direction value is normalized.
    /// </summary>
    /// <value>
    /// The direction.
    /// </value>
    public VectorD3D SelectedValue
    {
      get
      {
        double polarAngleRadians = _polarAngleDegrees * Math.PI / 180;
        double elevationAngleRadians = _elevationAngleDegrees * Math.PI / 180;
        return new VectorD3D(
          Math.Cos(polarAngleRadians) * Math.Cos(elevationAngleRadians),
          Math.Sin(polarAngleRadians) * Math.Cos(elevationAngleRadians),
          Math.Sin(elevationAngleRadians)
          );
      }
      set
      {
        double len = value.Length;
        if (0 == len || double.IsNaN(len) || double.IsInfinity(len))
          value = new VectorD3D(1, 0, 0);
        else
          value /= len;
        _elevationAngleDegrees = Math.Asin(value.Z) * 180 / Math.PI;
        _polarAngleDegrees = Math.Atan2(value.Y, value.X) * 180 / Math.PI;

        _lock.ExecuteLocked(
          () =>
          {
            _guiPolarAngleBox.SelectedValue = _polarAngleDegrees;
            _guiPolarAngleSlider.Value = _polarAngleDegrees;
            _guiElevationAngleBox.SelectedValue = _elevationAngleDegrees;
            _guiElevationAngleSlider.Value = _elevationAngleDegrees;
          });
      }
    }

    private void EhPolarAngleBoxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      _lock.ExecuteLockedButOnlyIfNotLockedBefore(
      () =>
      {
        _polarAngleDegrees = _guiPolarAngleBox.SelectedValue;
        _guiPolarAngleSlider.Value = _polarAngleDegrees;
      },
      () => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
      );
    }

    private void EhPolarAngleSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      _lock.ExecuteLockedButOnlyIfNotLockedBefore(
      () =>
      {
        _polarAngleDegrees = _guiPolarAngleSlider.Value;
        _guiPolarAngleBox.SelectedValue = _polarAngleDegrees;
      },
      () => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
      );
    }

    private void EhPolarAzimuthBoxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      _lock.ExecuteLockedButOnlyIfNotLockedBefore(
      () =>
      {
        _elevationAngleDegrees = _guiElevationAngleBox.SelectedValue;
        _guiElevationAngleSlider.Value = _elevationAngleDegrees;
      },
      () => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
      );
    }

    private void EhAzimuthAngleSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      _lock.ExecuteLockedButOnlyIfNotLockedBefore(
      () =>
      {
        _elevationAngleDegrees = _guiElevationAngleSlider.Value;
        _guiElevationAngleBox.SelectedValue = _elevationAngleDegrees;
      },
      () => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
      );
    }
  }
}
