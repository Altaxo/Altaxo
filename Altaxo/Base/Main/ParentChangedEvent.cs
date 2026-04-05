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

#nullable enable
using System;

namespace Altaxo.Main
{
  /// <summary>
  /// Provides data for notifications that report a changed parent object.
  /// </summary>
  public class ParentChangedEventArgs : System.EventArgs
  {
    /// <summary>
    /// The previous parent object.
    /// </summary>
    protected object? _oldParent;

    /// <summary>
    /// The new parent object.
    /// </summary>
    protected object? _newParent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParentChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldParent">The previous parent object.</param>
    /// <param name="newParent">The new parent object.</param>
    public ParentChangedEventArgs(object? oldParent, object? newParent)
    {
      _oldParent = oldParent;
      _newParent = newParent;
    }

    /// <summary>
    /// Gets the new parent object.
    /// </summary>
    public object? NewParent
    {
      get { return _newParent; }
    }

    /// <summary>
    /// Gets the previous parent object.
    /// </summary>
    public object? OldParent
    {
      get { return _oldParent; }
    }
  }

  /// <summary>
  /// Represents the method that handles a parent-changed event.
  /// </summary>
  /// <param name="sender">The event source.</param>
  /// <param name="e">The event arguments.</param>
  public delegate void ParentChangedEventHandler(object sender, ParentChangedEventArgs e);
}
