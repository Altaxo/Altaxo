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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
  /// <summary>
  /// Indicates that the owner can announce tunneling events by firing <see cref="TunneledEvent"/>.
  /// </summary>
  public interface ITunnelingEventSource
  {
    /// <summary>
    /// The event that is fired when a tunneling event occurs.
    /// First argument is the sender,
    /// second argument is the original source,
    /// and third argument is the event arguments.
    /// </summary>
    event Action<object, object, Main.TunnelingEventArgs> TunneledEvent;
  }

  /// <summary>
  /// Base class for tunneled event arguments.
  /// </summary>
  public class TunnelingEventArgs : EventArgs
  {
  }

  /// <summary>
  /// Event arguments indicating that an object was disposed.
  /// </summary>
  public class DisposeEventArgs : TunnelingEventArgs
  {
    /// <summary>
    /// Gets the shared empty instance.
    /// </summary>
    public static new readonly DisposeEventArgs Empty = new DisposeEventArgs();

    private DisposeEventArgs()
    {
    }
  }

  /// <summary>
  /// Event arguments indicating that a document path changed.
  /// </summary>
  public class DocumentPathChangedEventArgs : TunnelingEventArgs
  {
    /// <summary>
    /// Gets the shared empty instance.
    /// </summary>
    public static new readonly DocumentPathChangedEventArgs Empty = new DocumentPathChangedEventArgs();

    private DocumentPathChangedEventArgs()
    {
    }
  }

  /// <summary>
  /// Happens when the dirty status of the main document is cleared (usually after the project was saved).
  /// </summary>
  /// <seealso cref="Altaxo.Main.TunnelingEventArgs" />
  public class DirtyResetEventArgs : TunnelingEventArgs
  {
    /// <summary>
    /// Gets the shared empty instance.
    /// </summary>
    public static new readonly DirtyResetEventArgs Empty = new DirtyResetEventArgs();

    private DirtyResetEventArgs()
    {
    }
  }
}
