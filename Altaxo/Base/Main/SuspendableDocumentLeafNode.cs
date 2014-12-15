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
	public abstract class SuspendableDocumentLeafNode : Main.SuspendableLeafObject, Main.IDocumentNode, Main.IChangedEventSource
	{
		/// <summary>
		/// The parent object this instance belongs to.
		/// </summary>
		[NonSerialized]
		protected object _parent;

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
		/// Gets/sets the parent object this instance belongs to.
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
		/// Gets a value indicating whether someone is listening to changes. For this, either the <see cref="ParentObject"/> or the <see cref="Changed"/> event must be set.
		/// </summary>
		/// <value>
		/// <c>true</c> if someone listening to changes; otherwise, <c>false</c>.
		/// </value>
		public bool IsSomeoneListeningToChanges
		{
			get
			{
				return _parent != null || Changed != null;
			}
		}

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
		/// Accumulates the change data of the child. Currently only a flag is set to signal that the table has changed.
		/// </summary>
		/// <param name="sender">The sender of the change notification (currently unused).</param>
		/// <param name="e">The change event args can provide details of the change (currently unused).</param>
		protected abstract void AccumulateChangeData(object sender, EventArgs e);

		/// <summary>
		/// Called if some member of this instance itself has changed.
		/// </summary>
		protected void EhSelfChanged(EventArgs e)
		{
			if (!IsSuspended)
			{
				// Notify parent
				if (_parent is Main.IChildChangedEventSink)
				{
					((Main.IChildChangedEventSink)_parent).EhChildChanged(this, e); // parent may change our suspend state
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

		protected override void OnResume()
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

		protected override void OnResumeSilently()
		{
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
		/// Gets the name of this document node.
		/// </summary>
		/// <value>
		/// The name of this document node.
		/// </value>
		public abstract string Name { get; }
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
				if (null != aedAsSelf && e is SelfAccumulateableEventArgs)
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

		public override string Name
		{
			get { return this.GetType().Name; }
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