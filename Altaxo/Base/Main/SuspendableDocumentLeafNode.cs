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
	/// Base class for a suspendable document node which has no children, i.e. is a leaf node of the document tree.
	/// It implements most of the code neccessary to handle own change events and to accumulate data, if this object is suspended.
	/// </summary>
	public abstract class SuspendableDocumentLeafNode : SuspendableDocumentNodeBase
	{
		/// <summary>How many times was the Suspend function called (without corresponding Resume)</summary>
		private int _suspendLevel;

		#region Suspend state questions

		/// <summary>
		/// Suspend will increase the SuspendLevel.
		/// </summary>
		/// <returns>An object, which must be handed to the resume function to decrease the suspend level. Alternatively,
		/// the object can be used in an using statement. In this case, the call to the Resume function is not neccessary.</returns>
		public override ISuspendToken SuspendGetToken()
		{
			return new SuspendToken(this);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is suspended.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is suspended; otherwise, <c>false</c>.
		/// </value>
		public override bool IsSuspended { get { return _suspendLevel != 0; } }

		#endregion Suspend state questions

		#region Call back functions by the suspendToken

		/// <summary>
		/// Called when the suspend level has just gone from 0 to 1, i.e. the object was suspended.
		/// </summary>
		protected virtual void OnSuspended()
		{
		}

		/// <summary>
		/// Is called when the suspend level is still 1 (one), but is about to fall to zero, i.e. shortly before the call to <see cref="OnResume"/>. This function is not called before <see cref="OnResumeSilently"/>!
		/// </summary>
		protected virtual void OnAboutToBeResumed()
		{
		}

		/// <summary>
		/// Is called when the suspend level falls down from 1 to zero  by a call to <see cref="ISuspendToken.Resume()"/> or a call to <see cref="ISuspendToken.Dispose()"/>.
		/// </summary>
		/// <param name="eventCount">The event count. The event count is the number of times the <see cref="CountEvent"/> function was called during the suspended state.</param>
		protected virtual void OnResume()
		{
			EventArgs singleArg;
			if (AccumulatedEventData_HasZeroOrOneEventArg(out singleArg) && null != singleArg) // we have a single event arg accumulated
			{
				if (null == singleArg) // no events during suspended state
				{
					// nothing to do
				}
				else // one (1) event during suspend state
				{
					AccumulatedEventData_Clear();

					var parent = _parent as Main.IChildChangedEventSink;
					if (null != parent)
					{
						parent.EhChildChanged(this, singleArg);
					}
					if (!IsSuspended)
					{
						OnChanged(singleArg); // Fire the changed event
					}
				}
			}
			else // there is more than one event arg accumulated
			{
				var accumulatedEvents = AccumulatedEventData.ToArray();
				AccumulatedEventData_Clear();

				var parent = _parent as Main.IChildChangedEventSink;
				if (null != parent)
				{
					foreach (var eventArg in accumulatedEvents)
						parent.EhChildChanged(this, eventArg);
				}
				if (!IsSuspended)
				{
					foreach (var eventArg in accumulatedEvents)
						OnChanged(eventArg); // Fire the changed event
				}
			}
		}

		/// <summary>
		/// Is called when the suspend level falls down from 1 to zero by a call to <see cref="ISuspendToken.ResumeSilently"/>.
		/// The implementation should delete any accumulated events, should also disarm the suspendTokens of the childs of this object, and should not fire any Changed events nor set the change state of the object to dirty.
		/// </summary>
		/// <param name="eventCount">The event count. The event count is the number of times the <see cref="CountEvent"/> function was called during the suspended state.</param>
		protected virtual void OnResumeSilently()
		{
			AccumulatedEventData_Clear();
		}

		#endregion Call back functions by the suspendToken

		#region Change event handling

		/// <summary>
		/// Handles the cases when a child changes, but a reaction is neccessary only if the table is not suspended currently.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		/// <returns>True if the event has not changed the state of the table (i.e. it requires no further action).</returns>
		protected virtual bool HandleLowPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			return false;
		}

		/// <summary>
		/// Called if some member of this instance itself has changed.
		/// </summary>
		protected void EhSelfChanged(EventArgs e)
		{
			if (!IsSuspended)
			{
				// Notify parent
				if (null != _parent && !_parent.IsDisposeInProgress)
				{
					_parent.EhChildChanged(this, e); // parent may change our suspend state
				}

				if (!IsSuspended)
				{
					OnChanged(e); // Fire change event
					return;
				}
			}

			// at this point we are suspended for sure, or resume is still in progress
			AccumulateChangeData(this, e);  // child is unable to accumulate change data, we have to to it by ourself
		}

		#endregion Change event handling

		#region Inner class SuspendToken

		private class SuspendToken : ISuspendToken
		{
			private SuspendableDocumentLeafNode _parent;

			internal SuspendToken(SuspendableDocumentLeafNode parent)
			{
				var suspendLevel = System.Threading.Interlocked.Increment(ref parent._suspendLevel);
				_parent = parent;

				if (1 == suspendLevel)
				{
					try
					{
						_parent.OnSuspended();
					}
					catch (Exception)
					{
						System.Threading.Interlocked.Decrement(ref parent._suspendLevel);
						_parent = null;
						throw;
					}
				}
			}

			~SuspendToken()
			{
				Dispose();
			}

			/// <summary>
			/// Disarms this SuppressToken so that it can not raise the resume event anymore.
			/// </summary>
			public void ResumeSilently()
			{
				var parent = System.Threading.Interlocked.Exchange<SuspendableDocumentLeafNode>(ref _parent, null);
				if (parent != null)
				{
					int newLevel = System.Threading.Interlocked.Decrement(ref parent._suspendLevel);

					if (0 == newLevel)
					{
						try
						{
							parent.OnResumeSilently();
						}
						finally
						{
						}
					}
					else if (newLevel < 0)
					{
						throw new ApplicationException("Fatal programming error - suppress level has fallen down to negative values");
					}
				}
			}

			public void Resume()
			{
				Dispose();
			}

			public void Resume(EventFiring eventFiring)
			{
				switch (eventFiring)
				{
					case EventFiring.Enabled:
						Resume();
						break;

					case EventFiring.Suppressed:
						ResumeSilently();
						break;

					default:
						throw new NotImplementedException(string.Format("Unknown option: {0}", eventFiring));
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				var parent = System.Threading.Interlocked.Exchange<SuspendableDocumentLeafNode>(ref _parent, null);
				if (parent != null)
				{
					Exception exceptionInAboutToBeResumed = null;
					if (1 == parent._suspendLevel)
					{
						try
						{
							parent.OnAboutToBeResumed();
						}
						catch (Exception ex)
						{
							exceptionInAboutToBeResumed = ex;
						}
					}

					int newLevel = System.Threading.Interlocked.Decrement(ref parent._suspendLevel);

					if (0 == newLevel)
					{
						try
						{
							parent.OnResume();
						}
						finally
						{
						}
					}
					else if (newLevel < 0)
					{
						throw new ApplicationException("Fatal programming error - suppress level has fallen down to negative values");
					}

					if (null != exceptionInAboutToBeResumed)
						throw exceptionInAboutToBeResumed;
				}
			}

			#endregion IDisposable Members
		}

		#endregion Inner class SuspendToken
	}

	/// <summary>
	/// Base class for a suspendable document node. This class stores a single object to accumulate event data.
	/// This class supports document nodes that have children,
	/// and implements most of the code neccessary to handle child events and to suspend the childs when the parent is suspended.
	/// </summary>
	/// <typeparam name="T">Type of accumulated event data, of type <see cref="EventArgs"/> or any derived type.</typeparam>
	public abstract class SuspendableDocumentLeafNodeWithSingleAccumulatedData<T> : SuspendableDocumentLeafNode where T : EventArgs
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

	/// <summary>
	/// Implements a <see cref="SuspendableDocumentLeafNodeWithSingleAccumulatedData{System.EventArgs}"/>. The accumulated data store the event args that you provide in the call to EhSelfChanged.
	/// </summary>
	public class SuspendableDocumentLeafNodeWithEventArgs : SuspendableDocumentLeafNodeWithSingleAccumulatedData<EventArgs>
	{
		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			if (null != e)
				_accumulatedEventData = e;
			else
				_accumulatedEventData = EventArgs.Empty;
		}

		/// <summary>
		/// Calls EhSelfChanged with EventArgs.Empty
		/// </summary>
		public virtual void EhSelfChanged()
		{
			EhSelfChanged(EventArgs.Empty);
		}
	}

	/// <summary>
	/// Base class for a suspendable document node. This class stores the accumulate event data objects in a special set <see cref="ISetOfEventData"/>.
	/// This set takes into account that <see cref="SelfAccumulateableEventArgs"/> can be accumulated. By overriding <see cref="GetHashCode"/> and <see cref="Equals"/> you can control whether only one instance or
	/// multiple instances can be stored in the set.
	/// This class supports document nodes that have children,
	/// and implements most of the code neccessary to handle child events and to suspend the childs when the parent is suspended.
	/// </summary>
	public abstract class SuspendableDocumentLeafNodeWithSetOfEventArgs : SuspendableDocumentLeafNode
	{
		private static EventArgs[] _emptyData = new EventArgs[0];

		/// <summary>
		/// The accumulated event data.
		/// </summary>
		[NonSerialized]
		protected ISetOfEventData _accumulatedEventData = new SetOfEventData();

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
			var count = _accumulatedEventData.Count;
			switch (count)
			{
				case 0:
					singleEventArg = null;
					return true;

				case 1:
					singleEventArg = _accumulatedEventData.First();
					return true;

				default:
					singleEventArg = null;
					return false;
			}
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
					return _accumulatedEventData;
				else
					return _emptyData;
			}
		}

		/// <summary>
		/// Clears the accumulated event data.
		/// </summary>
		protected override void AccumulatedEventData_Clear()
		{
			if (null != _accumulatedEventData)
				_accumulatedEventData.Clear();
		}

		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			_accumulatedEventData.SetOrAccumulate(e);
		}
	}
}