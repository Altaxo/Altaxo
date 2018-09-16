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
  public partial class Direction3DCartesianControl : UserControl
  {
    private double[] _direction = new double[3];

    public event EventHandler SelectedValueChanged;

    private Common.NumericDoubleTextBox[] _guiDirectionBoxes;

    private Slider[] _guiDirectionSliders;

    private GuiChangeLocker _lock;

    public Direction3DCartesianControl()
    {
      InitializeComponent();

      _guiDirectionBoxes = new Common.NumericDoubleTextBox[3] { _guiDirectionXBox, _guiDirectionYBox, _guiDirectionZBox };
      _guiDirectionSliders = new Slider[3] { _guiDirectionXSlider, _guiDirectionYSlider, _guiDirectionZSlider };
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
        return new VectorD3D(_direction[0], _direction[1], _direction[2]);
      }
      set
      {
        var len = value.Length;
        if (0 == len || double.IsNaN(len) || double.IsInfinity(len))
          value = new VectorD3D(1, 0, 0);
        else
          value /= len;

        _direction[0] = value.X;
        _direction[1] = value.Y;
        _direction[2] = value.Z;

        _lock.ExecuteLocked(
          () =>
          {
            for (int i = 0; i < 3; ++i)
            {
              _guiDirectionSliders[i].Value = _direction[i];
              _guiDirectionBoxes[i].SelectedValue = _direction[i];
            }
          });
      }
    }

    private Tuple<int, int> GetOtherIndices(int i)
    {
      switch (i)
      {
        case 0:
          return new Tuple<int, int>(1, 2);
        case 1:
          return new Tuple<int, int>(0, 2);
        case 2:
          return new Tuple<int, int>(0, 1);
        default:
          throw new InvalidOperationException();
      }
    }

    private void CorrectDirectionValues(int index)
    {
      var indices = GetOtherIndices(index);
      var valA = _direction[index];
      var valB = _direction[indices.Item1];
      var valC = _direction[indices.Item2];

      if (valB == 0 && valC == 0)
      {
        valB = valC = Math.Sqrt((1 - valA * valA) / 2);
      }
      else
      {
        double f = Math.Sqrt((1 - valA * valA) / (valB * valB + valC * valC));
        valB *= f;
        valC *= f;
      }

      _direction[indices.Item1] = valB;
      _direction[indices.Item2] = valC;

      _guiDirectionBoxes[indices.Item1].SelectedValue = valB;
      _guiDirectionBoxes[indices.Item2].SelectedValue = valC;

      _guiDirectionSliders[indices.Item1].Value = valB;
      _guiDirectionSliders[indices.Item2].Value = valC;
    }

    private void EhDirectionBoxValueChanged(int index)
    {
      _lock.ExecuteLockedButOnlyIfNotLockedBefore(
        () =>
        {
          _direction[index] = _guiDirectionBoxes[index].SelectedValue;
          _guiDirectionSliders[index].Value = _direction[index];

          CorrectDirectionValues(index);
        },
        () => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
        );
    }

    private void DirectionSliderValueChanged(int index)
    {
      _lock.ExecuteLockedButOnlyIfNotLockedBefore(
      () =>
      {
        _direction[index] = _guiDirectionSliders[index].Value;
        _guiDirectionBoxes[index].SelectedValue = _direction[index];

        CorrectDirectionValues(index);
      },
      () => SelectedValueChanged?.Invoke(this, EventArgs.Empty));
    }

    private void EhDirectionXBoxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      EhDirectionBoxValueChanged(0);
    }

    private void EhDirectionYBoxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      EhDirectionBoxValueChanged(1);
    }

    private void EhDirectionZBoxValueChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      EhDirectionBoxValueChanged(2);
    }

    private void EhDirectionXSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      DirectionSliderValueChanged(0);
    }

    private void EhDirectionYSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      DirectionSliderValueChanged(1);
    }

    private void EhDirectionZSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      DirectionSliderValueChanged(2);
    }
  }
}
