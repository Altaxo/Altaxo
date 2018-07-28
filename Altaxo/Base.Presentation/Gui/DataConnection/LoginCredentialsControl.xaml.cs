#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.DataConnection
{
  /// <summary>
  /// Interaction logic for LoginCredentialsControl.xaml
  /// </summary>
  public partial class LoginCredentialsControl : UserControl, ILoginCredentialsView
  {
    private bool _isPasswordVisible;

    public LoginCredentialsControl()
    {
      InitializeComponent();
    }

    private void EhMakePasswordVisible(object sender, RoutedEventArgs e)
    {
      var pass = Password;
      _isPasswordVisible = true;
      Password = pass;
      SetPasswordControlsVisibility();
    }

    private void EhMakePasswordHidden(object sender, RoutedEventArgs e)
    {
      var pass = Password;
      _isPasswordVisible = false;
      Password = pass;
      SetPasswordControlsVisibility();
    }

    private void SetPasswordControlsVisibility()
    {
      _guiPasswordHidden.Visibility = _isPasswordVisible ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
      _guiPasswordVisible.Visibility = _isPasswordVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
    }

    public string Username
    {
      get
      {
        return _guiUsername.Text;
      }
      set
      {
        _guiUsername.Text = value;
      }
    }

    public string Password
    {
      get
      {
        return _isPasswordVisible ? _guiPasswordVisible.Text : _guiPasswordHidden.Password;
      }
      set
      {
        if (_isPasswordVisible)
          _guiPasswordVisible.Text = value;
        else
          _guiPasswordHidden.Password = value;
      }
    }
  }
}
