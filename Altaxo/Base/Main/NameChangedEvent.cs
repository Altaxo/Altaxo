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

namespace Altaxo.Main
{
  /// <summary>
  /// Event args that accompany a NameChanged event.
  /// </summary>
  /// <seealso cref="Altaxo.Main.TunnelingEventArgs" />
  public class NameChangedEventArgs : TunnelingEventArgs
  {
    /// <summary>
    /// Gets the source of the event, i.e. the instance whose name has changed.
    /// </summary>
    /// <value>
    /// The source of the event.
    /// </value>
    public object Source { get; private set; }

    /// <summary>
    /// Gets the old name of the instance that was renamed.
    /// </summary>
    /// <value>
    /// The old name of the instance that was renamed.
    /// </value>
    public string OldName { get; private set; }

    /// <summary>
    /// Gets the new name of the instance that was renamed.
    /// </summary>
    /// <value>
    /// The new name of the instance that was renamed.
    /// </value>
    public string NewName { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NameChangedEventArgs"/> class.
    /// </summary>
    /// <param name="source">The source of the event, i.e. the instance whose name has changed.</param>
    /// <param name="oldName">The old name of that instance.</param>
    /// <param name="newName">The new name of that instance.</param>
    public NameChangedEventArgs(object source, string oldName, string newName)
    {
      Source = source;
      OldName = oldName;
      NewName = newName;
    }
  }

  /// <summary>
  /// Event handler of the name changed event.
  /// </summary>
  /// <param name="sender">The sender of the event (not neccessarily the instance whose name has changed).</param>
  /// <param name="e">The <see cref="NameChangedEventArgs"/> instance containing the event data.</param>
  public delegate void NameChangedEventHandler(object sender, NameChangedEventArgs e);
}
