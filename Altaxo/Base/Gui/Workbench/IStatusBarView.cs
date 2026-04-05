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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Defines the view contract for the workbench status bar.
  /// </summary>
  public interface IStatusBarView
  {
    /// <summary>
    /// Sets a value indicating whether the status bar is visible.
    /// </summary>
    bool IsStatusBarVisible { set; }

    /// <summary>
    /// Sets the content of the cursor status panel.
    /// </summary>
    object CursorStatusBarPanelContent { set; }

    /// <summary>
    /// Sets the content of the selection status panel.
    /// </summary>
    object? SelectionStatusBarPanelContent { set; }

    /// <summary>
    /// Sets the content of the mode status panel.
    /// </summary>
    object ModeStatusBarPanelContent { set; }

    /// <summary>
    /// Displays a status message.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="highlighted">Whether the message should be highlighted.</param>
    /// <param name="icon">An optional icon.</param>
    void SetMessage(string message, bool highlighted, object? icon);

    /// <summary>
    /// Hides the progress display.
    /// </summary>
    void HideProgress();

    /// <summary>
    /// Displays progress information.
    /// </summary>
    /// <param name="taskName">The task name.</param>
    /// <param name="progress">The progress value.</param>
    /// <param name="status">The operation status.</param>
    void DisplayProgress(string taskName, double progress, OperationStatus status);
  }
}
