#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Represents the state of an unnamed and thus unsaved project.
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.IProjectArchiveManager" />
  public class UnnamedProjectArchiveManager : IProjectArchiveManager
  {
    private bool _isDisposed; // not really neccessary, but useful for debugging purposes

    private PathName _fileOrFolderName;

    public event EventHandler<NameChangedEventArgs> FileOrFolderNameChanged // Never used, because name can not change
    {
      add { }
      remove { }
    }


    public UnnamedProjectArchiveManager()
    {

    }

    public UnnamedProjectArchiveManager(PathName pathName)
    {
      _fileOrFolderName = pathName;
    }

    /// <summary>
    /// Returns null because a unnamed project does not have a file name yet.
    /// </summary>
    public PathName FileOrFolderName => _fileOrFolderName;

    /// <inheritdoc/>
    public bool IsDisposed => _isDisposed;

    /// <inheritdoc/>
    public void Dispose()
    {
      _isDisposed = true;
    }

    /// <inheritdoc/>
    public IProjectArchive GetArchiveReadOnlyThreadSave(object claimer)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void ReleaseArchiveThreadSave(object claimer, ref IProjectArchive archive)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Save(SaveProjectAndWindowsStateDelegate saveProjectAndWindowsState)
    {
      throw new InvalidOperationException("The project does not have any name yet");
    }
  }
}
