﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.IO;
using System.Reflection;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench.Commands
{
  public class OpenSettingsDirectory : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      var appName = StringParser.Parse("${AppName}");

      string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);

      string args = "/e," + dir;

      var processInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", args)
      {
        CreateNoWindow = false,
        WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
      };

      try
      {
        var proc = System.Diagnostics.Process.Start(processInfo);
      }
      catch (Exception)
      {
      }
    }
  }

  public class OpenApplicationSettingsDirectory : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      var appName = StringParser.Parse("${AppName}");

      var commonApp = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
      string dir = Path.Combine(commonApp, appName);

      if (!Directory.Exists(dir))
      {
        dir = commonApp;
      }

      string args = "/e," + dir;

      var processInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", args)
      {
        CreateNoWindow = false,
        WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
      };

      try
      {
        var proc = System.Diagnostics.Process.Start(processInfo);
      }
      catch (Exception)
      {
      }
    }
  }

  public class OpenProgramDirectory : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      var entryAssemblyPath = Assembly.GetEntryAssembly()?.Location;
      if (entryAssemblyPath is null || !System.IO.Path.IsPathRooted(entryAssemblyPath))
      {
        Current.Gui.ErrorMessageBox("Unable to evaluate path of the entry assembly!", "Error");
        return;
      }

      string dir = FileName.GetDirectoryName(entryAssemblyPath);

      string args = "/e," + dir;

      var processInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", args)
      {
        CreateNoWindow = false,
        WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
      };

      try
      {
        var proc = System.Diagnostics.Process.Start(processInfo);
      }
      catch (Exception)
      {
      }
    }
  }

}
