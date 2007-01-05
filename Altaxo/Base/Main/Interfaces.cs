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

namespace Altaxo.Main
{
  /// <summary>Designates a object which supports the changed event.</summary>
  public interface IChangedEventSource
  {
    /// <summary>Fired when something in the object has changed.</summary>
    event System.EventHandler Changed;
  }

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
  /// All objects that can be suspended and resumed should implement this interface
  /// </summary>
  public interface ISuspendable
  {
    /// <summary>
    /// Suspend all change notifications.
    /// </summary>
    void Suspend();

    /// <summary>
    /// Resume all change notifications.
    /// </summary>
    void Resume();
  }

  /// <summary>ChangedEventArgs can be used by originators of a Changed event to preserve the originator of the Changed event, even if 
  /// the event is chained through a couple of parent objects.</summary>
  public class ChangedEventArgs : EventArgs
  {
    /// <summary>Stores the original object that caused the Changed event.</summary>
    public object Originator;
    /// <summary>Can be used to store additional information about that Changed event.</summary>
    public object Tag;

    /// <summary>
    /// Creates the ChangedEventArgs.
    /// </summary>
    /// <param name="originator">The originator of the Changed event.</param>
    /// <param name="tag">Additional information about the event, may be null.</param>
    public ChangedEventArgs(object originator, object tag)
    {
      Originator = originator;
      Tag = tag;
    }


  
  }


}
