#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Main
{
	/// <summary>
	/// Stores a set of event data. A special function exist to either set the item, if it is not accumulateable, or to accumulate the event data.
	/// </summary>
	public interface ISetOfEventData : ICollection<EventArgs>
	{
		/// <summary>
		/// Puts the specified item in the collection, regardless whether it is already contained or not. If it is not already contained, it is added to the collection.
		/// If it is already contained, and is of type <see cref="SelfAccumulateableEventArgs"/>, the <see cref="SelfAccumulateableEventArgs.Add"/> function is used to add the item to the already contained item.
		/// </summary>
		/// <param name="item">The <see cref="EventArgs"/> instance containing the event data.</param>
		void SetOrAccumulate(EventArgs item);
	}

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
}