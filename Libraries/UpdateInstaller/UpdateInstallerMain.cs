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

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Contains the main entry point of the installer application.
  /// </summary>
  internal class UpdateInstallerMain
  {
    private static System.Windows.Application app;
    private static InstallerMainWindow mainWindow;

    /// <summary>
    /// A text that should occur prior to the error message.
    /// </summary>
    public const string ErrorIntroduction = "An error occured during the auto update installation of Altaxo:\r\n\r\n";

    /// <summary>Main entry point of the application.</summary>
    /// <param name="args">Application arguments.</param>
    [STAThread]
    private static void Main(string[] args)
    {
      try
      {
        // args[0]: the name of the event that must be signalled when the installer is ready to install
        // args[1]: name of the package file to use
        // args[2]: either 0 or 1. If 0, the installation should take place silently. If 1, the installer window should be visible." +
        // args[3]: integer, designates the number of seconds before the installer window is closed after a sucessfull installation" +
        // args[4]: either 0 or 1, if 1 then Altaxo should be restarted after installation. Furthermore, Bit1=1 indicates that this process was started with elevated privileges.
        // args[5]: argument full name of the Altaxo executable
        // args[6]: and more arguments: the original arguments of the Altaxo executable

        if (args.Length < 6)
        {
          StartVisualAppWithErrorMessage(null,
            "Programm called with less than 4 arguments, but at least 4 arguments are required:\r\n\r\n" +
            "args[0]: name of the event the installer is signalling when it is ready to install\r\n" +
            "args[1]: full path to the auto update package file\r\n" +
            "args[2]: either 0 or 1. If 0, the installer window will be hidden. If 1, the installer window is visible." +
            "args[3]: integer, designates the number of seconds before the installer window is closed after a sucessfull installation" +
            "args[4]: either 0,1, or 2. If 1, Altaxo is restarted after the installation is finished. A value of 2 is an indication that this program was started with elevated privileges\r\n" +
            "args[5]: full path to the old Altaxo executable\r\n" +
            "args[6..n]: arguments that will be used to restart Altaxo (if args[4] is 1)\r\n"
            );
          return;
        }

        // try to set the current directory to this of the entry assembly in order to not block the removing of the Altaxo directories
        try
        { System.Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location); }
        catch (Exception) { }

        string eventName = args[0];
        string packageFullFileName = args[1];
        bool showInstallationWindow = 0 != int.Parse(args[2], System.Globalization.CultureInfo.InvariantCulture);
        int timeoutAfterSuccessfullInstallation = int.Parse(args[3], System.Globalization.CultureInfo.InvariantCulture);
        int options = int.Parse(args[4], System.Globalization.CultureInfo.InvariantCulture);
        string fullPathOfTheAltaxoExecutable = args[5];
        bool wasStartedWithElevatedPrivileges = 0 != (options & 2);
        bool restartAltaxo = (0 != (options & 1)) && !wasStartedWithElevatedPrivileges;

        var installer = new UpdateInstaller(eventName, packageFullFileName, fullPathOfTheAltaxoExecutable);
        if (installer.PackListFileExists())
        {
          if (installer.PackListFileIsWriteable())
          {
            StartVisualApp(installer, showInstallationWindow, timeoutAfterSuccessfullInstallation);
          }
          else // Package file is not writetable
          {
            if (0 != (options & 2)) // do we have already elevated privileges?
            {
              StartVisualAppWithErrorMessage(eventName,
                string.Format(ErrorIntroduction + "There is still no write access to the package file of the old installation, try to update again later!"));
              return; // returns is ok here, no need to restart Altaxo since we run with elevated privileges
            }
            else
            {
              // Start a new process with elevated privileges and wait for exit
              var proc = new System.Diagnostics.ProcessStartInfo
              {
                FileName = System.Reflection.Assembly.GetEntryAssembly().Location
              };
              args[4] = "2";
              var stb = new StringBuilder();
              foreach (var s in args)
                stb.AppendFormat("\"{0}\"\t", s);

              proc.Arguments = stb.ToString();
              proc.Verb = "runas";
              var runProcWithElevated = System.Diagnostics.Process.Start(proc);

              if (restartAltaxo)
                runProcWithElevated.WaitForExit();
            }
          }
        }
        else // package file don't exist
        {
          StartVisualAppWithErrorMessage(eventName, string.Format("{0}Package file of old installation was not found (file: {1})!\r\nPlease reinstall Altaxo manually!", ErrorIntroduction, installer.PackListFileFullName));
        }

        if (restartAltaxo)
        {
          var stb = new StringBuilder();
          for (int i = 6; i < args.Length; ++i)
            stb.AppendFormat("\"{0}\"\t", args[i]);
          System.Diagnostics.Process.Start(args[5], stb.ToString());
        }
      }
      catch (Exception ex)
      {
        StartVisualAppWithErrorMessage(args[0], string.Format("{0}{1}", ex.GetType().ToString(), ex.ToString()));
      }
    }

    /// <summary>Starts the window of the application, and then runs the provided installer program.</summary>
    /// <param name="installer">The installer program to run..</param>
    private static void StartVisualApp(UpdateInstaller installer, bool showInstallationWindow, int timeoutAfterSuccessfullInstallation)
    {
      if (null == app)
      {
        app = new System.Windows.Application();
      }
      if (null == mainWindow)
      {
        mainWindow = new InstallerMainWindow(showInstallationWindow, timeoutAfterSuccessfullInstallation)
        {
          _installer = installer
        };
        app.Run(mainWindow);
      }
    }

    /// <summary>Starts the window of the application, and then presents the provided error message message.</summary>
    /// <param name="eventName">Name of the event that is used to signal to Altaxo that Altaxo should be stopped.</param>
    /// <param name="message">The error message to present.</param>
    private static void StartVisualAppWithErrorMessage(string eventName, string message)
    {
      if (null != eventName)
        UpdateInstaller.SetEvent(eventName); // Altaxo is waiting for this event to finish itself

      if (null == app)
      {
        app = new System.Windows.Application();
      }
      if (null == mainWindow)
      {
        mainWindow = new InstallerMainWindow(true, int.MaxValue);
        mainWindow.SetErrorMessage(message);
        app.Run(mainWindow);
      }
      else
      {
        mainWindow.SetErrorMessage(message);
      }
    }
  }
}
