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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Main
{
  public delegate void VoidVoidEventHandler();

  /// <summary>
  /// Helper class to suspend and resume change events (or other events).
  /// </summary>
  public class EventSuppressor
  {
    #region Inner class SuppressToken
    private class SuppressToken : IDisposable
    {
      EventSuppressor _parent;

      public SuppressToken(EventSuppressor parent)
      {
        _parent = parent;
        _parent._suppressLevel++;
      }

      #region IDisposable Members

      public void Dispose()
      {
        if (_parent != null)
        {
          EventSuppressor parent = _parent;
          _parent = null;
          int oldLevel = parent._suppressLevel--;
          if (1 == oldLevel)
          {
            parent.OnResume();
          }
        }
      }

      #endregion
    }
    #endregion
    private int _suppressLevel;
    VoidVoidEventHandler _resumeEventHandler;
    int _eventCount;

    /// <summary>
    /// Constructor. You have to provide a callback function, that is been called when the event handling resumes.
    /// </summary>
    /// <param name="resumeEventHandler">The callback function called when the events resume. See remarks when the callback function is called.</param>
    /// <remarks>The callback function is called only (i) if the event resumes (exactly: the _suppressLevel changes from 1 to 0),
    /// and (ii) in that moment the _eventCount is &gt;0.
    /// To get the _eventCount&gt;0, someone must call either GetEnabledWithCounting or GetDisabledWithCounting
    /// during the suspend period.</remarks>
    public EventSuppressor(VoidVoidEventHandler resumeEventHandler)
    {
      _resumeEventHandler = resumeEventHandler;
    }

    /// <summary>
    /// Suspend will increase the SuspendLevel.
    /// </summary>
    /// <returns>An object, which must be handed to the resume function to decrease the suspend level. Alternatively,
    /// the object can be used in an using statement. In this case, the call to the Resume function is not neccessary.</returns>
    public IDisposable Suspend()
    {
      return new SuppressToken(this);
    }


    /// <summary>
    /// Decrease the suspend level by disposing of the object. The object can not be 
    /// </summary>
    /// <param name="token"></param>
    public void Resume(ref IDisposable token)
    {
      if (token != null)
      {
        token.Dispose(); // the OnResume function is called from the SuppressToken
        token = null;
      }
    }

    /// <summary>
    /// For the moment of execution, the suspend status is interrupted, and one time the OnResume function would be called.
    /// </summary>
    public void ResumeShortly()
    {
      if (_suppressLevel != 0)
        OnResume();
    }

    /// <summary>
    /// Returns true when the suppress level is equal to zero (initial state). Otherwise false.
    /// Attention - this function does not increment the event counter.
    /// </summary>
    public bool PeekEnabled { get { return _suppressLevel == 0; } }
    /// <summary>
    /// Returns true when the suppress level is greater than zero (initial state). Returns false if the suppress level is zero.
    /// Attention - this function does not increment the event counter.
    /// </summary>
    public bool PeekDisabled { get { return _suppressLevel != 0; } }

    /// <summary>
    /// Returns true when the suppress level is equal to zero (initial state). Otherwise false.
    /// If the suppress level not zero, this function increases the event count by one.    /// </summary>
    public bool GetEnabledWithCounting()
    {
        if (_suppressLevel != 0)
        {
          _eventCount++;
          return false;
        }
        else
        {
          return true;
        }
    }
    /// <summary>
    /// Returns true when the suppress level is greater than zero (initial state). Returns false if the suppress level is zero.
    /// If the suppress level not zero, this function increases the event counter by one. At a resume, the OnResume handler is only called when the event count is greater than zero.
    /// </summary>
    public bool GetDisabledWithCounting()
    {
      if (_suppressLevel != 0)
      {
        _eventCount++;
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Is called when the suppress level falls down from 1 to zero and the event count is != 0.
    /// Per default, the resume event handler is called that you provided in the constructor.
    /// </summary>
    protected virtual void OnResume()
    {
      if (_eventCount != 0)
      {
        _eventCount = 0;
        if (_resumeEventHandler != null)
          _resumeEventHandler();
      }
    }
  }
}
