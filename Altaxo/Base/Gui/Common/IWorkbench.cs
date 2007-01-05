#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Summary description for IWorkbench.
  /// </summary>
  public interface IWorkbench
  {
    /// <summary>Gets the corresponding workbench GUI object, i.e for Windows the main windows form.</summary>
    object ViewObject { get; }

    /// <summary>Gets the active view content, i.e. in most cases it returns the controller that controls the content.</summary>
    object ActiveViewContent { get; }
    
    /// <summary>The view content collection.</summary>
    System.Collections.ICollection ViewContentCollection { get; }

    /// <summary>
    /// Shows the view content. The type of object content depends on the GUI type. SharpDevelop's GUI
    /// requires an object of type IViewContent; 
    /// </summary>
    /// <param name="content">The view content that should be shown.</param>
    void ShowView(object content);

    /// <summary>
    /// Closes the view content. The type of object content depends on the GUI type. SharpDevelop's GUI
    /// requires an object of type IViewContent; 
    /// </summary>
    /// <param name="content">The view content that should be shown.</param>
    void CloseContent(object content);

    /// <summary>
    /// Closes all views.
    /// </summary>
    void CloseAllViews();

    /// <summary>Fired if the current view (and so the view content) changed.</summary>
    event EventHandler ActiveWorkbenchWindowChanged;
  }

}
