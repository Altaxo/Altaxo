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
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Settings
{
  /// <summary>
  /// Interaction logic for AutoUpdateOptionsControl.xaml
  /// </summary>
  public partial class AutoUpdateSettingsControl : UserControl, IAutoUpdateSettingsView
  {
    public AutoUpdateSettingsControl()
    {
      InitializeComponent();
    }

    private void EhEnableAutoUpdatesChanged(object sender, RoutedEventArgs e)
    {
      var content = _guiMainGroup.Content as FrameworkElement;
      if (null != content)
        content.IsEnabled = true == _guiEnableAutoUpdates.IsChecked;
    }

    public bool EnableAutoUpdates
    {
      get
      {
        return true == _guiEnableAutoUpdates.IsChecked;
      }
      set
      {
        _guiEnableAutoUpdates.IsChecked = value;
      }
    }

    public bool DownloadUnstableVersion
    {
      get
      {
        return true == _guiDownloadStableAndUnstable.IsChecked;
      }
      set
      {
        if (value == true)
          _guiDownloadStableAndUnstable.IsChecked = true;
        else
          _guiDownloadStableOnly.IsChecked = true;
      }
    }

    public bool ShowDownloadWindow
    {
      get
      {
        return true == _guiShowDownloadWindow.IsChecked;
      }
      set
      {
        _guiShowDownloadWindow.IsChecked = value;
      }
    }

    public int DownloadInterval
    {
      get
      {
        if (true == _guiDownloadMonthly.IsChecked)
          return 30;
        else if (true == _guiDownloadWeekly.IsChecked)
          return 7;
        else
          return 0;
      }
      set
      {
        if (value >= 30)
          _guiDownloadMonthly.IsChecked = true;
        else if (value >= 7)
          _guiDownloadWeekly.IsChecked = true;
        else
          _guiDownloadAlways.IsChecked = true;
      }
    }

    public int InstallAt
    {
      get
      {
        if (true == _guiInstallAtStartup.IsChecked)
          return 1;
        else if (true == _guiInstallAtShutdown.IsChecked)
          return 2;
        else
          return 3;
      }
      set
      {
        switch (value)
        {
          case 1:
            _guiInstallAtStartup.IsChecked = true;
            break;

          case 2:
            _guiInstallAtShutdown.IsChecked = true;
            break;

          default:
            _guiInstallEitherStartupOrShutdown.IsChecked = true;
            break;
        }
      }
    }

    public bool ShowInstallationWindow
    {
      get
      {
        return _guiShowInstallationWindow.IsChecked == true;
      }
      set
      {
        _guiShowInstallationWindow.IsChecked = value;
      }
    }

    public bool ConfirmInstallation
    {
      get
      {
        return _guiConfirmInstallation.IsChecked == true;
      }
      set
      {
        _guiConfirmInstallation.IsChecked = value;
      }
    }

    public int InstallationWindowClosingTime
    {
      get
      {
        return _guiCloseInstallationWindowTime.Value;
      }
      set
      {
        _guiCloseInstallationWindowTime.Value = value;
      }
    }
  }
}
