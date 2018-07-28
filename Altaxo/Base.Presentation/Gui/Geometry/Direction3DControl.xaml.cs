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

namespace Altaxo.Gui.Geometry
{
  /// <summary>
  /// Interaction logic for Direction3DSphericalControl.xaml
  /// </summary>
  public partial class Direction3DControl : UserControl
  {
    private VectorD3D _direction;

    private GuiChangeLocker _lock;

    public event EventHandler SelectedValueChanged;

    public Direction3DControl()
    {
      InitializeComponent();
    }

    public VectorD3D SelectedValue
    {
      get
      {
        if (_guiCartesian.IsKeyboardFocusWithin)
          return _guiCartesian.SelectedValue;
        else if (_guiSpherical.IsKeyboardFocusWithin)
          return _guiSpherical.SelectedValue;
        else
          return _direction;
      }
      set
      {
        _lock.ExecuteLocked(
          () =>
          {
            var len = value.Length;
            if (0 == len || double.IsNaN(len) || double.IsInfinity(len))
              value = new VectorD3D(1, 0, 0);
            else
              value /= len;

            _direction = value;
            _guiSpherical.SelectedValue = _direction;
            _guiCartesian.SelectedValue = _direction;
          });
      }
    }

    private void EhCartesianValueChanged(object sender, EventArgs e)
    {
      _lock.ExecuteLockedButOnlyIfNotLockedBefore(
        () =>
        {
          _direction = _guiCartesian.SelectedValue;
          _guiSpherical.SelectedValue = _direction;
        },
        () => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
        );
    }

    private void EhSphericalValueChanged(object sender, EventArgs e)
    {
      _lock.ExecuteLockedButOnlyIfNotLockedBefore(
        () =>
        {
          _direction = _guiSpherical.SelectedValue;
          _guiCartesian.SelectedValue = _direction;
        },
        () => SelectedValueChanged?.Invoke(this, EventArgs.Empty)
        );
    }

    private void EhRadioButtonChanged(object sender, RoutedEventArgs e)
    {
      if (null == _guiCartesian || null == _guiSpherical)
        return; // Design mode or just during control creation

      string tag = (string)((RadioButton)sender).Tag;

      switch (tag)
      {
        case "Cartesian":
          _guiSpherical.Visibility = Visibility.Collapsed;
          _guiCartesian.Visibility = Visibility.Visible;
          break;

        case "Spherical":
          _guiCartesian.Visibility = Visibility.Collapsed;
          _guiSpherical.Visibility = Visibility.Visible;
          break;

        case "Both":
          _guiCartesian.Visibility = Visibility.Visible;
          _guiSpherical.Visibility = Visibility.Visible;
          break;

        default:
          throw new NotImplementedException("Tag");
      }
    }
  }
}
