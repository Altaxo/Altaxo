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

namespace Altaxo.Com
{
  /// <summary>
  /// Manages the connection of the Com functions to our application
  /// </summary>
  public class AltaxoComApplicationAdapter
  {
    public bool IsInvokeRequiredForGuiThread()
    {
      return Current.Dispatcher.InvokeRequired;
    }

    public void InvokeGuiThread(Action action)
    {
      Current.Dispatcher.InvokeIfRequired(action);
    }

    public void BeginInvokeGuiThread(Action action)
    {
      Current.Dispatcher.InvokeAndForget(action);
    }

    /// <summary>
    /// Sets the host names. The application should show the containerApplicationName and containerFileName in the title bar. Additionally, if <paramref name="embeddedObject"/> is not <c>null</c>,
    /// the application should switch the user interface.
    /// </summary>
    /// <param name="containerApplicationName">Name of the container application.</param>
    /// <param name="containerFileName">Name of the container file.</param>
    /// <param name="embeddedObject">The Altaxo object (for instance graph document) that is embedded in the container application.</param>
    public void SetHostNames(string containerApplicationName, string containerFileName, object embeddedObject)
    {
      // see Brockschmidt, Inside Ole 2nd ed. page 992
      // calling SetHostNames is the only sign that our object is embedded (and thus not linked)
      // this means that we have to switch the user interface from within this function

      ComDebug.ReportInfo("IOleObject.SetHostNames szContainerApp={0}, szContainerObj={1}", containerApplicationName, containerFileName);

      Current.Workbench.EhProjectChanged(this, new Main.ProjectEventArgs(null, null, Main.ProjectEventKind.ProjectDirtyChanged)); // Set new title in title bar
    }

    /// <summary>
    /// Starts the closing of the application asynchronously.
    /// </summary>
    public void BeginClosingApplication()
    {
      BeginInvokeGuiThread(new Action(() => ((System.Windows.Window)Current.Workbench.ViewObject).Close())); // Begin Closing the main window
    }

    /// <summary>
    /// Makes the main window invisible to the user (but doesn't close the application);
    /// </summary>
    public void HideMainWindow()
    {
      Action hiding = () =>
      {
        ComDebug.ReportInfo("Hide main window");

        ((System.Windows.Window)Current.Workbench.ViewObject).ShowInTaskbar = false;
        ((System.Windows.Window)Current.Workbench.ViewObject).Visibility = System.Windows.Visibility.Hidden;
      };
      InvokeGuiThread(hiding);
    }

    /// <summary>
    /// Makes the main window of the application visible to the user.
    /// </summary>
    public void ShowMainWindow()
    {
      InvokeGuiThread(() =>
      {
        ComDebug.ReportInfo("Make main window visible");

        var mainWindow = System.Windows.Application.Current.MainWindow;

        mainWindow.Visibility = System.Windows.Visibility.Visible;
        mainWindow.ShowInTaskbar = true;
        mainWindow.BringIntoView();

        // how to bring a window to the front ?
        // see http://stackoverflow.com/questions/257587/bring-a-window-to-the-front-in-wpf

        if (!mainWindow.IsVisible)
        {
          mainWindow.Show();
        }

        if (mainWindow.WindowState == System.Windows.WindowState.Minimized)
        {
          mainWindow.WindowState = System.Windows.WindowState.Normal;
        }

        mainWindow.Activate();
        mainWindow.Topmost = true;  // important
        mainWindow.Topmost = false; // important
        mainWindow.Focus();         // important

        ComDebug.ReportInfo("Make main window visible - done!");
      });
    }
  }
}
