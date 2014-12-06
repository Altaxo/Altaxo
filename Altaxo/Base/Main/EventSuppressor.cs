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

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Designates in classes and function whether events are allowed to be fired.
	/// </summary>
	public enum EventFiring
	{
		/// <summary>Events are allowed to be fired.</summary>
		Enabled,

		/// <summary>Event(s) should be suppressed.</summary>
		Suppressed
	}

	/// <summary>
	/// Interface for a token that is used to suspend events. By creating such a token, the suspend level of the parent object is incremented by one.
	/// If the token is disposed, or by a call to Resume, the suspend level of the object is decremented by one. If the suspend level
	/// falls to zero, the events are enabled again.
	/// </summary>
	public interface ISuspendToken : IDisposable
	{
		/// <summary>
		/// Disarms this SuspendToken and decrements the suspend level of the parent object. If the suspend level falls to zero during this call,
		/// the resume function is <b>not</b> called. Instead, usually another function (e.g. ResumeSilently) is called on the parent object to indicate that the object should be resumed without notifying that the object has changed.
		/// </summary>
		void ResumeSilently();

		/// <summary>
		/// Decrements the suspend level of the parent object. If the suspend level falls to zero during this call,
		/// the resume function is called. The object should then resume all child objects, and then indicate that it has changed to its parent and to any other listeners of the Change event.
		/// </summary>
		void Resume();

		/// <summary>
		/// Either resumes the parent object of this token (using <see cref="Resume()"/>), or resumes silently (using <see cref="ResumeSilently"/>), depending on the provided argument.
		/// </summary>
		/// <param name="eventFiring">Determines whether <see cref="Resume()"/> is used, or <see cref="ResumeSilently"/>.</param>
		void Resume(EventFiring eventFiring);
	}

	/// <summary>Helper class to suspend and resume change events (or other events). In contrast to the simpler class <see cref="TemporaryDisabler"/>,
	/// this class keeps also track of events that happen in the suspend period. Per default, the action on resume is
	/// fired only if some events have happened during the suspend period.</summary>
	public class EventSuppressor : SuspendableObject
	{
		/// <summary>Action that is taken when the suppress levels falls down to zero and the event count is equal to or greater than one (i.e. during the suspend phase, at least an event had occured).</summary>
		private Action _resumeEventHandler;

		private Action _aboutToBeResumedEventHandler;

		/// <summary>
		/// Constructor. You have to provide a callback function, that is been called when the event handling resumes.
		/// </summary>
		/// <param name="resumeEventHandler">The callback function called when the events resume. See remarks when the callback function is called.</param>
		/// <remarks>The callback function is called only (i) if the event resumes (exactly: the _suppressLevel changes from 1 to 0),
		/// and (ii) in that moment the _eventCount is &gt;0.
		/// To get the _eventCount&gt;0, someone must call either GetEnabledWithCounting or GetDisabledWithCounting
		/// during the suspend period.</remarks>
		public EventSuppressor(Action resumeEventHandler)
		{
			_resumeEventHandler = resumeEventHandler;
		}

		/// <summary>
		/// Constructor. You have to provide a callback function, that is been called when the event handling resumes.
		/// </summary>
		/// <param name="resumeEventHandler">The callback function called when the events resume. See remarks when the callback function is called.</param>
		/// <param name="aboutToBeResumedEventHandler">This handler is called when the suppress level is about to fall down to zero, but is still 1.</param>
		/// <remarks>The callback function is called only (i) if the event resumes (exactly: the _suppressLevel changes from 1 to 0),
		/// and (ii) in that moment the _eventCount is &gt;0.
		/// To get the _eventCount&gt;0, someone must call either GetEnabledWithCounting or GetDisabledWithCounting
		/// during the suspend period.</remarks>
		public EventSuppressor(Action resumeEventHandler, Action aboutToBeResumedEventHandler)
		{
			_resumeEventHandler = resumeEventHandler;
			_aboutToBeResumedEventHandler = aboutToBeResumedEventHandler;
		}

		/// <summary>
		/// Is called when the suppress level falls down from 1 to zero and the event count is != 0.
		/// Per default, the resume event handler is called that you provided in the constructor.
		/// </summary>
		protected override void OnResume(int eventCount)
		{
			if (eventCount != 0)
			{
				if (_resumeEventHandler != null)
					_resumeEventHandler();
			}
		}

		protected override void OnAboutToBeResumed(int eventCount)
		{
			var ev = _aboutToBeResumedEventHandler;
			if (null != ev)
				ev();
		}
	}
}