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
	/// <remarks>If you don't need support for child events, consider using <see cref="SuspendableDocumentLeafNode{TEventArgs}"/> instead.</remarks>
	public abstract class SuspendableDocumentNode : Main.SuspendableObject, Main.IDocumentNode, Main.IChangedEventSource, Main.IChildChangedEventSink
	{
		/// <summary>The parent of this object.</summary>
		protected object _parent;

		/// <summary>Stores the suspend tokens of the suspended childs of this object.</summary>
		protected HashSet<ISuspendToken> _suspendTokensOfChilds = new HashSet<ISuspendToken>();

		/// <summary>Fired when something in the object has changed, and the object is not suspended.</summary>
		[field: NonSerialized]
		public event EventHandler Changed;

		/// <summary>
		/// Determines whether there is no or only one single event arg accumulated. If this is the case, the return value is <c>true</c>. If there is one event arg accumulated, it is returned in the argument <paramref name="singleEventArg"/>.
		/// The return value is false if there is more than one event arg accumulated. In this case the <paramref name="singleEventArg"/> is <c>null</c> on return, and the calling function should use <see cref="AccumulatedEventData"/> to
		/// enumerate all accumulated event args.
		/// </summary>
		/// <param name="singleEventArg">The <see cref="EventArgs"/> instance containing the event data, if there is exactly one event arg accumulated. Otherwise, it is <c>null</c>.</param>
		/// <returns>True if there is zero or one event arg accumulated, otherwise <c>false</c>.</returns>
		protected abstract bool AccumulatedEventData_HasZeroOrOneEventArg(out EventArgs singleEventArg);

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
		protected virtual bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			return false;
		}

		/// <summary>
		/// Processes the event args <paramref name="e"/> when this object is not suspended. This function serves two purposes:
		/// i) updating some cached data of this object by processing the event args of the child,
		/// and ii) optional transforming the event args, for instance to a new type, which afterwards is send to the parent and is used as event args in the <see cref="Changed"/> event of this object.
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
			if (HandleHighPriorityChildChangeCases(sender, ref e))
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
				if (null != aedAsSelf && e is SelfAccumulateableEventArgs)
				{
					aedAsSelf.Add((SelfAccumulateableEventArgs)e);
				}
			}
		}
	}

	/// <summary>
	/// Base class for a suspendable document node. This class stores the accumulate event data objects in a special set <see cref="ISetOfEventData"/>.
	/// This set takes into account that <see cref="SelfAccumulateableEventArgs"/> can be accumulated. By overriding <see cref="GetHashCode"/> and <see cref="Equals"/> you can control whether only one instance or
	/// multiple instances can be stored in the set.
	/// This class supports document nodes that have children,
	/// and implements most of the code neccessary to handle child events and to suspend the childs when the parent is suspended.
	/// </summary>
	public abstract class SuspendableDocumentNodeWithSetOfEventArgs : SuspendableDocumentNode
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