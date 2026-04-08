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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Represents a view content that is backed by an <see cref="OpenedFile"/>.
  /// </summary>
  public interface IFileViewContent : IViewContent
  {
    /// <summary>
    /// Gets the primary file name.
    /// </summary>
    FileName PrimaryFileName { get; }

    /// <summary>
    /// Gets the primary opened file.
    /// </summary>
    OpenedFile PrimaryFile { get; }

    /// <summary>
    /// Loads the view content from the specified opened file and source stream.
    /// </summary>
    /// <param name="openedFile">The opened file.</param>
    /// <param name="sourceStream">The source stream to read from.</param>
    void Load(OpenedFile openedFile, Stream sourceStream);

    /// <summary>
    /// Saves the view content to the specified opened file and stream.
    /// </summary>
    /// <param name="openedFile">The opened file.</param>
    /// <param name="stream">The target stream to write to.</param>
    void Save(OpenedFile openedFile, Stream stream);

    /// <summary>
    /// Determines whether the current view can switch to this view without saving and loading.
    /// </summary>
    /// <param name="openedFile">The opened file.</param>
    /// <param name="currentView">The currently active view.</param>
    /// <returns><see langword="true"/> if the switch can happen without save/load; otherwise, <see langword="false"/>.</returns>
    bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile openedFile, IFileViewContent currentView);

    /// <summary>
    /// Determines whether this view can switch away without saving and loading.
    /// </summary>
    /// <param name="openedFile">The opened file.</param>
    /// <param name="newView">The target view.</param>
    /// <returns><see langword="true"/> if the switch can happen without save/load; otherwise, <see langword="false"/>.</returns>
    bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile openedFile, IFileViewContent newView);

    /// <summary>
    /// Switches away from this view without saving and loading.
    /// </summary>
    /// <param name="openedFile">The opened file.</param>
    /// <param name="newView">The target view.</param>
    void SwitchFromThisWithoutSaveLoad(OpenedFile openedFile, IFileViewContent newView);

    /// <summary>
    /// Switches to this view without saving and loading.
    /// </summary>
    /// <param name="openedFile">The opened file.</param>
    /// <param name="currentView">The previously active view.</param>
    void SwitchToThisWithoutSaveLoad(OpenedFile openedFile, IFileViewContent currentView);
  }
}
