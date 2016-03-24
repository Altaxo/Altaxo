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
	/// Base class for a suspendable document node. This class stores a single object to accumulate event data.
	/// This class supports document nodes that have children,
	/// and implements most of the code neccessary to handle child events and to suspend the childs when the parent is suspended.
	/// </summary>
	/// <typeparam name="T">Type of accumulated event data, of type <see cref="EventArgs"/> or any derived type.</typeparam>
	public abstract class SuspendableDocumentNodeWithSingleAccumulatedData<T> : SuspendableDocumentNode where T : EventArgs
	{
		/// <summary>
		/// Holds the accumulated change data.
		/// </summary>
		[NonSerialized]
		protected T _accumulatedEventData;

		/// <summary>
		/// Determines whether there is no or only one single event arg accumulated. If this is the case, the return value is <c>true</c>. If there is one event arg accumulated, it is returned in the argument <paramref name="singleEventArg" />.
		/// The return value is false if there is more than one event arg accumulated. In this case the <paramref name="singleEventArg" /> is <c>null</c> on return, and the calling function should use <see cref="AccumulatedEventData" /> to
		/// enumerate all accumulated event args.
		/// </summary>
		/// <param name="singleEventArg">The <see cref="EventArgs" /> instance containing the event data, if there is exactly one event arg accumulated. Otherwise, it is <c>null</c>.</param>
		/// <returns>
		/// True if there is zero or one event arg accumulated, otherwise <c>false</c>.
		/// </returns>
		protected override bool AccumulatedEventData_HasZeroOrOneEventArg(out EventArgs singleEventArg)
		{
			singleEventArg = _accumulatedEventData;
			return true;
		}

		/// <summary>
		/// Gets the accumulated event data.
		/// </summary>
		/// <value>
		/// The accumulated event data.
		/// </value>
		protected override IEnumerable<EventArgs> AccumulatedEventData
		{
			get
			{
				if (null != _accumulatedEventData)
					yield return _accumulatedEventData;
			}
		}

		/// <summary>
		/// Clears the accumulated event data.
		/// </summary>
		protected override void AccumulatedEventData_Clear()
		{
			_accumulatedEventData = null;
		}

		/// <summary>
		/// Sets the change data without further processing. This function is infrastructure and intended to use only in OnResume after the parent has suspended this node again.
		/// </summary>
		/// <param name="e">The event args (one or more).</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Not possible to set more than one event arg here.</exception>
		protected override void AccumulatedChangeData_SetBackAfterResumeAndSuspend(params EventArgs[] e)
		{
			if (!(_accumulatedEventData == null)) throw new InvalidProgramException();

			if (e.Length > 1)
				throw new ArgumentOutOfRangeException("Not possible to set more than one event arg here.");
			if (e.Length > 0)
				_accumulatedEventData = (T)e[0];
		}

		/// <summary>
		/// Accumulates the change data of the child. Currently only a flag is set to signal that the table has changed.
		/// </summary>
		/// <param name="sender">The sender of the change notification (currently unused).</param>
		/// <param name="e">The change event args can provide details of the change (currently unused).</param>
		/// <exception cref="System.ArgumentNullException">Argument e is null</exception>
		/// <exception cref="System.ArgumentException"></exception>
		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			if (null == e)
				throw new ArgumentNullException("Argument e is null");
			if (!(e is T))
				throw new ArgumentException(string.Format("Argument e has the wrong type. Type expected: {0}, actual type of e: {1}", typeof(T), e.GetType()));

			if (null == _accumulatedEventData)
			{
				_accumulatedEventData = (T)e;
			}
			else // there is already an event arg present
			{
				var aedAsSelf = _accumulatedEventData as SelfAccumulateableEventArgs;
				if (null != aedAsSelf && aedAsSelf.Equals(e)) // Equals is here (mis)used to ensure compatibility between the two event args
				{
					aedAsSelf.Add((SelfAccumulateableEventArgs)e);
				}
			}
		}
	}
}