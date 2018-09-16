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
  public partial class Position3DCartesianControl : UserControl
  {
    private double[] _position = new double[3];

    public event EventHandler SelectedValueChanged;

    private Common.NumericDoubleTextBox[] _guiPositionBoxes;

    private Slider[] _guiPositionSliders;

    private GuiChangeLocker _lock;

    public Position3DCartesianControl()
    {
      InitializeComponent();

      _guiPositionBoxes = new Common.NumericDoubleTextBox[3] { _guiPositionXBox, _guiPositionYBox, _guiPositionZBox };
      _guiPositionSliders = new Slider[3] { _guiPositionXSlider, _guiPositionYSlider, _guiPositionZSlider };
    }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>
    /// The position.
    /// </value>
    public PointD3D SelectedValue
    {
      get
      {
        return new PointD3D(_position[0], _position[1], _position[2]);
      }
      set
      {
        _position[0] = value.X;
        _position[1] = value.Y;
        _position[2] = value.Z;

        _lock.ExecuteLocked(
          () =>
          {
            for (int i = 0; i < 3; ++i)
            {
              _guiPositionBoxes[i].SelectedValue = _position[i];
              if (_guiPositionSliders[i].Minimum > _position[i])
                _guiPositionSliders[i].Minimum = _position[i];
              if (_guiPositionSliders[i].Maximum < _position[i])
                _guiPositionSliders[i].Maximum = _position[i];
              _guiPositionSliders[i].Value = _position[i];
            }
          });
      }
    }

    private void EhPositionBoxValueChanged(int index)
    {
      _lock.ExecuteLockedButOnlyIfNotLockedBefore(
        () =>
        {
          _position[index] = _guiPositionBoxes[index].SelectedValue;

          if (_guiPositionSliders[index].Minimum > _position[index])
            _guiPositionSliders[index].Minimum = _position[index];
          if (_guiPositionSliders[index].Maximum < _position[index])

            _guiPositionSliders[index].Value = _position[index];
        },
        () => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
        );
    }

    private void PositionSliderValueChanged(int index)
    {
      _lock.ExecuteLockedButOnlyIfNotLockedBefore(
      () =>
      {
        _position[index] = _guiPositionSliders[index].Value;
        _guiPositionBoxes[index].SelectedValue = _position[index];
      },
      () => SelectedValueChanged?.Invoke(this, EventArgs.Empty));
    }

    private void EhPositionXBoxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      EhPositionBoxValueChanged(0);
    }

    private void EhPositionYBoxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      EhPositionBoxValueChanged(1);
    }

    private void EhPositionZBoxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      EhPositionBoxValueChanged(2);
    }

    private void EhPositionXSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      PositionSliderValueChanged(0);
    }

    private void EhPositionYSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      PositionSliderValueChanged(1);
    }

    private void EhPositionZSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      PositionSliderValueChanged(2);
    }
  }
}
