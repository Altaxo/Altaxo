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

#nullable enable
using System;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// A class from which a project archive can be restored. Ideally, this object should not claim any resources.
  /// </summary>
  public interface IProjectArchiveEntryMemento : IDisposable
  {
    /// <summary>
    /// Gets the archive entry.
    /// </summary>
    /// <returns>The archive entry.</returns>
    IProjectArchiveEntry? GetArchiveEntry();

    /// <summary>
    /// Gets the full name of the archive entry.
    /// </summary>
    /// <value>
    /// The full name of the archive entry.
    /// </value>
    string EntryName { get; }

    /// <summary>
    /// Gives a instance of the memento, with a new entry name (returns the same instance if the entry name is the same).
    /// </summary>
    /// <param name="newEntryName">New name of the entry.</param>
    /// <returns>Instance of the memento with the new entry name.</returns>
    IProjectArchiveEntryMemento WithEntryName(string newEntryName);

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of the instance (without operational data).</returns>
    IProjectArchiveEntryMemento Clone();
  }

}
