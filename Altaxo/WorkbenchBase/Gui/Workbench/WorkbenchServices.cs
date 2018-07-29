#region Copyright

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

using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Static entry point for retrieving Workbench services.
  /// </summary>
  public static class WorkbenchServices
  {
    /// <summary>
    /// Equivalent to <code>SD.Workbench.ActiveViewContent.GetService(type)</code>,
    /// but does not throw a NullReferenceException when ActiveViewContent is null.
    /// (instead, null is returned).
    /// </summary>
    public static object GetActiveViewContentService(Type type)
    {
      var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();
      if (workbench != null)
      {
        var activeViewContent = workbench.ActiveViewContent;
        if (activeViewContent != null)
        {
          return activeViewContent.GetService(type);
        }
      }
      return null;
    }

    /// <inheritdoc see="IWorkbench"/>
    public static IWorkbench Workbench
    {
      get { return Altaxo.Current.GetRequiredService<IWorkbench>(); }
    }

    public static System.Windows.Window MainWindow
    {
      get { return (System.Windows.Window)Altaxo.Current.GetRequiredService<IWorkbench>().ViewObject; }
    }

    /// <summary>
    /// Gets the <see cref="IDispatcherMessageLoopWpf"/> representing the main UI thread.
    /// </summary>
    public static IDispatcherMessageLoopWpf MainThread
    {
      get { return Altaxo.Current.GetRequiredService<IDispatcherMessageLoopWpf>(); }
    }

    /// <inheritdoc see="IWinFormsService"/>
    public static IWinFormsService WinForms
    {
      get { return Altaxo.Current.GetRequiredService<IWinFormsService>(); }
    }

    /// <inheritdoc see="IStatusBarService"/>
    public static IStatusBarService StatusBar
    {
      get { return Altaxo.Current.GetRequiredService<IStatusBarService>(); }
    }

    public static event Action WorkbenchCreated;

    public static void OnWorkbenchCreated()
    {
      WorkbenchCreated?.Invoke();
    }
  }
}
