using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Base class for a suspendable document node.
	/// This class supports document nodes that have children, and implements most of the code neccessary to handle child events and to suspend the childs when the parent is suspended.
	/// </summary>
	/// <remarks>If you don't need support for child events, consider using <see cref="SuspendableLeafDocumentNode"/> instead.</remarks>
	public abstract class SuspendableDocumentNode : Main.SuspendableObject, Main.IDocumentNode, Main.IChangedEventSource, Main.IChildChangedEventSink
	{
		/// <summary>The parent of this object.</summary>
		protected object _parent;

		/// <summary>Stores the suspend tokens of the suspended childs of this object.</summary>
		protected HashSet<ISuspendToken> _suspendTokensOfChilds = new HashSet<ISuspendToken>();

		/// <summary>
		/// Gets the accumulated event data.
		/// </summary>
		/// <value>
		/// The accumulated event data.
		/// </value>
		protected abstract IEnumerable<EventArgs> AccumulatedEventData { get; }

		/// <summary>
		/// Clears the accumulated event data.
		/// </summary>
		protected abstract void AccumulatedEventData_Clear();

		/// <summary>Fired when something in the object has changed, and the object is not suspended.</summary>
		public event EventHandler Changed;

		/// <summary>
		/// Gets/sets the parent object. In derived classes, setting the parent might be forbidden and will throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		public virtual object ParentObject
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		#region Change event handling

		/// <summary>
		/// Handles the case when a child changes, and a reaction is neccessary independently on the suspend state of the table.
		/// </summary>
		/// <param name="sender">The sender of the event, usually a child of this object.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		/// <returns>True if the event will not change the state of the object and the handling of the event is completely done. Thus, if returning <c>true</c>, the object is considered as 'not changed'.
		/// If in doubt, return <c>false</c>. This will allow the further processing of the event.
		/// </returns>
		protected virtual bool HandleHighPriorityChildChangeCases(object sender, EventArgs e)
		{
			return false;
		}

		/// <summary>
		/// Processes the event args <paramref name="e"/> when this object is not suspended. This function serves two purposes:
		/// i) updating some cached data of this object by processing the event args of the child,
		/// and ii) optional transforming the event args, for instance to a new type, which afterwards is send to the parent and is used as event args in the <see cref="Change"/> event of this object.
		/// The transformed event args is <b>not</b> used if this object is suspended (in this case the original event args is used).
		/// </summary>
		/// <param name="sender">The sender of the event args, usually a child of this object.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data. On return, you can provided transformed event args by this parameter.</param>
		/// <returns><c>True</c> if the event will not change this object, and further processing of the event is not neccessary.
		/// If in doubt, return <c>false</c>. This will allow the further processing of the event.
		/// </returns>
		protected virtual bool HandleLowPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			return false;
		}

		/// <summary>
		/// Accumulates the change data of the child (sometimes also the change data of this object itself).
		/// </summary>
		/// <param name="sender">The sender of the event args, usually a child of this object.</param>
		/// <param name="e">The change event args, that provide details of the change.</param>
		protected abstract void AccumulateChangeData(object sender, EventArgs e);

		/// <summary>
		/// Used by childs of this object to inform us about a change in their state.
		/// </summary>
		/// <param name="sender">The sender of this event, usually a child of this object.</param>
		/// <param name="e">The change details.</param>
		public void EhChildChanged(object sender, System.EventArgs e)
		{
			if (HandleHighPriorityChildChangeCases(sender, e))
				return;

			if (!IsSuspendedOrResumeInProgress)
			{
				EventArgs eventArgsTransformed = e;
				if (HandleLowPriorityChildChangeCases(sender, ref eventArgsTransformed))
					return;

				// Notify parent
				if (_parent is Main.IChildChangedEventSink)
				{
					((Main.IChildChangedEventSink)_parent).EhChildChanged(this, eventArgsTransformed); // inform parent with transformed event args. Attention: parent may change our suspend state
				}

				if (!IsSuspendedOrResumeInProgress)
				{
					OnChanged(eventArgsTransformed); // Fire change event with transformed event args
					return;
				}
			}

			// at this point we are either suspended or resume is currently in progress
			if (IsSuspended)
			{
				CountEvent();
				if (sender is Main.ISuspendableByToken)
				{
					if (null == _suspendTokensOfChilds)
						_suspendTokensOfChilds = new HashSet<ISuspendToken>();
					_suspendTokensOfChilds.Add(((Main.ISuspendableByToken)sender).SuspendGetToken()); // add sender to suspended child
				}
				else
				{
					AccumulateChangeData(sender, e);  // child is unable to accumulate change data, we have to to it by ourself
				}
			}
			else // Resume is in Progress
			{
				AccumulateChangeData(sender, e);  // child is sending us data, we accumulate them until resume is finished
			}
		}

		/// <summary>
		/// Called if some (simple) member or property of this instance itself has changed.
		/// </summary>
		/// <param name="e">The change details.</param>
		protected void EhSelfChanged(EventArgs e)
		{
			if (!IsSuspendedOrResumeInProgress)
			{
				// Notify parent
				if (_parent is Main.IChildChangedEventSink)
				{
					((Main.IChildChangedEventSink)_parent).EhChildChanged(this, e); // parent may change our suspend state
				}

				if (!IsSuspendedOrResumeInProgress)
				{
					OnChanged(e); // Fire change event
					return;
				}
			}

			// at this point we are suspended for sure, or resume is still in progress
			CountEvent();
			AccumulateChangeData(this, e);  // child is unable to accumulate change data, we have to to it by ourself
		}

		/// <summary>
		/// Is called when the object is resumed, i.e. change notifications are allowed again. During the execution of this function, the <see cref="SuspendableObject.IsResumeInProgress"/> property will return <c>true</c> to indicate that the resume is currently in progress.
		/// </summary>
		/// <param name="eventCount">The event count. The event count is the number of times the <see cref="SuspendableObject.CountEvent"/> function was called during the suspend state.</param>
		protected override void OnResume(int eventCount)
		{
			// resume the suspended childs
			var suspendTokensOfChilds = _suspendTokensOfChilds;
			if (null != suspendTokensOfChilds)
			{
				_suspendTokensOfChilds = null;
				foreach (var obj in suspendTokensOfChilds)
					obj.Dispose();
			}

			// send accumulated data if available and release it thereafter
			var accumulatedEvents = AccumulatedEventData.ToArray();
			AccumulatedEventData_Clear();

			if (accumulatedEvents.Length > 0)
			{
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
		/// Is called when the suspend level falls down from 1 to zero by a call to <see cref="ISuspendToken.ResumeSilently" />.
		/// This implementation disarma the suspendTokens of the childs of this object, deletes any accumulated events, and does not send any change event to the parent or the listeners of the Change event.
		/// </summary>
		/// <param name="eventCount">The event count. The event count is the number of times the <see cref="CountEvent" /> function was called during the suspended state.</param>
		protected override void OnResumeSilently(int eventCount)
		{
			// resume the suspended childs
			var suspendTokensOfChilds = _suspendTokensOfChilds;
			if (null != suspendTokensOfChilds)
			{
				_suspendTokensOfChilds = null;
				foreach (var obj in suspendTokensOfChilds)
					obj.ResumeSilently();
			}

			AccumulatedEventData_Clear();
		}

		/// <summary>
		/// Fires the change event with the EventArgs provided in the argument.
		/// </summary>
		protected virtual void OnChanged(EventArgs e)
		{
			var ev = Changed;
			if (null != ev)
				ev(this, e);
		}

		#endregion Change event handling

		/// <summary>
		/// Gets/sets the name of this document node. Depending on the type of node, setting the name might be forbidden, and will throw an <see cref="InvalidOperationException"/>.
		/// </summary>
		/// <value>
		/// The name of this document node.
		/// </value>
		public abstract string Name { get; set; }
	}

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
	}

	/// <summary>
	/// Base class for a suspendable document node. This class stores the accumulate event data objects in a <see cref="HashSet"/>.
	/// This class supports document nodes that have children,
	/// and implements most of the code neccessary to handle child events and to suspend the childs when the parent is suspended.
	/// </summary>
	/// <typeparam name="T">Type of accumulated event data, of type <see cref="EventArgs"/> or any derived type.</typeparam>
	public abstract class SuspendableDocumentNodeWithHashSetOfAccumulatedData<T> : SuspendableDocumentNode where T : EventArgs
	{
		private static T[] _emptyData = new T[0];

		/// <summary>
		/// The accumulated event data.
		/// </summary>
		[NonSerialized]
		protected HashSet<T> _accumulatedEventData = new HashSet<T>();

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
	}
}