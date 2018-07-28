#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Threading;

namespace Altaxo.Gui
{
  /// <summary>
  /// Helps to prevent endless loops when updating Gui control which itself trigger change events.
  /// </summary>
  public struct GuiChangeLocker
  {
    private int _guiChangeLock;

    /// <summary>
    /// Gets a value indicating whether this instance is not locked.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is not locked; otherwise, <c>false</c>.
    /// </value>
    public bool IsNotLocked
    {
      get
      {
        return _guiChangeLock == 0;
      }
    }

    /// <summary>
    /// Executes the provided state only if the state of this instance is "not locked". During execution, the state will be set to "locked".
    /// </summary>
    /// <param name="action">The action to carry out if the state is not locked.</param>
    public void ExecuteLockedButOnlyIfNotLockedBefore(Action action)
    {
      if (Interlocked.Increment(ref _guiChangeLock) > 1)
      {
        Interlocked.Decrement(ref _guiChangeLock);
        return;
      }
      try
      {
        action();
      }
      finally
      {
        Interlocked.Decrement(ref _guiChangeLock);
      }
    }

    /// <summary>
    /// Executes the provided state only if the state of this instance is "not locked". During execution, the state will be set to "locked".
    /// </summary>
    /// <param name="actionToCarryOutLocked">The action that is carried out if the state at beginning of this call was not locked. During execution of this action, the state will be "locked".</param>
    /// <param name="actionToCarryOutNotLocked">Action that is carried out only if the first action was carried out. During execution of this action, the state will be "not locked".</param>
    public void ExecuteLockedButOnlyIfNotLockedBefore(Action actionToCarryOutLocked, Action actionToCarryOutNotLocked)
    {
      bool wasFirstActionExecuted = false;
      if (Interlocked.Increment(ref _guiChangeLock) > 1)
      {
        Interlocked.Decrement(ref _guiChangeLock);
        return;
      }
      try
      {
        actionToCarryOutLocked();
        wasFirstActionExecuted = true;
      }
      finally
      {
        Interlocked.Decrement(ref _guiChangeLock);
      }

      if (wasFirstActionExecuted)
      {
        actionToCarryOutNotLocked();
      }
    }

    /// <summary>
    /// Executes regardless of the state. During execution, the state will be set to "locked".
    /// </summary>
    /// <param name="action">The action to carry out.</param>
    public void ExecuteLocked(Action action)
    {
      Interlocked.Increment(ref _guiChangeLock);
      try
      {
        action();
      }
      finally
      {
        Interlocked.Decrement(ref _guiChangeLock);
      }
    }
  }
}
