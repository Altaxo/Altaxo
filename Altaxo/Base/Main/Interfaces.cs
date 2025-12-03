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
  /// Interface for instances that are able to copy properties from another object.
  /// </summary>
  public interface ICopyFrom : ICloneable
  {
    /// <summary>
    /// Try to copy from another object. Should try to copy even if the object to copy from is not of
    /// the same type, but a base type. In this case only the base properties should be copied.
    /// </summary>
    /// <param name="obj">Object to copy from.</param>
    /// <returns>True if at least parts of the object could be copied, false if the object to copy from is incompatible.</returns>
    bool CopyFrom(object obj);
  }

  /// <summary>Designates a object which supports the changed event.</summary>
  public interface IChangedEventSource
  {
    /// <summary>Fired when something in the object has changed.</summary>
    event System.EventHandler? Changed;
  }

  /// <summary>
  /// Interface for objects that receive child change notifications from owned components.
  /// Implementations should handle child changes and propagate state updates.
  /// </summary>
  public interface IChildChangedEventSink
  {
    /// <summary>
    /// This function is used by the childs of an object to signal an parent object that they have changed. If the function returns true, the child have to suspend it's
    /// change notifications (if this is supported by the child).
    /// </summary>
    /// <param name="child">The child object.</param>
    /// <param name="e">EventArgs, can be a derived class to provide details of the change.</param>
    /// <returns>The parent returns false normally. If the parent is suspended, it returns true to signal the child
    /// that it should also suspend its notification. </returns>
    void EhChildChanged(object child, EventArgs e);
  }

  /// <summary>
  /// Interface to objects that can be suspended by receiving a suspend token. When the suspend token is disposed, the event handling of the object is resumed.
  /// </summary>
  public interface ISuspendableByToken
  {
    /// <summary>
    /// Suspends the event handling of the object by getting a suspend token. The event handling of the object is resumed when the suspend token is disposed.
    /// </summary>
    /// <returns>The suspend token. To resume events on this object, you have to dispose the returned suspend token.</returns>
    ISuspendToken SuspendGetToken();

    /// <summary>
    /// Gets a value indicating whether this instance is suspended.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is suspended; otherwise, <c>false</c>.
    /// </value>
    bool IsSuspended { get; }
  }

  /// <summary>ChangedEventArgs can be used by originators of a Changed event to preserve the originator of the Changed event, even if
  /// the event is chained through a couple of parent objects.</summary>
  public class ChangedEventArgs : EventArgs
  {
    /// <summary>Stores the original object that caused the Changed event.</summary>
    public object Originator;

    /// <summary>Can be used to store additional information about that Changed event.</summary>
    public object? Tag;

    /// <summary>
    /// Creates the ChangedEventArgs.
    /// </summary>
    /// <param name="originator">The originator of the Changed event.</param>
    /// <param name="tag">Additional information about the event, may be null.</param>
    public ChangedEventArgs(object originator, object? tag)
    {
      Originator = originator;
      Tag = tag;
    }
  }
}
