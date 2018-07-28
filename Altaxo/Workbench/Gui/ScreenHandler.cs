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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Altaxo.Gui
{
  /// <summary>
  /// Helper class to retrieve on which screen a Wpf window is mainly located.
  /// See <see href=" https://social.msdn.microsoft.com/Forums/vstudio/en-US/2ca2fab6-b349-4c08-915f-373c71bd636a/show-and-maximize-wpf-window-on-a-specific-screen?forum=wpf"/>
  /// </summary>
  public static class ScreenHandler
  {
    /// <summary>
    /// Gets the current screen from a window
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns>The screen the window is located in.</returns>
    public static Screen GetCurrentScreen(System.Windows.Window window)
    {
      var parentArea = new System.Drawing.Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height);
      return Screen.FromRectangle(parentArea);
    }

    /// <summary>
    /// Gets a screen by number.
    /// </summary>
    /// <param name="requestedScreen">The requested screen number.</param>
    /// <returns>The screen.</returns>
    public static Screen GetScreen(int requestedScreen)
    {
      var screens = Screen.AllScreens;
      var mainScreen = 0;
      if (screens.Length > 1 && mainScreen < screens.Length)
      {
        return screens[requestedScreen];
      }
      return screens[0];
    }
  }
}
