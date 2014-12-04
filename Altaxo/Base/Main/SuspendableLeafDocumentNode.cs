using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	public abstract class SuspendableLeafDocumentNode : Main.SuspendableLeafObject, Main.IDocumentNode
	{
		/// <summary>
		/// The parent object this instance belongs to.
		/// </summary>
		[NonSerialized]
		protected object _parent;

		[field: NonSerialized]
		public event EventHandler Changed;

		[NonSerialized]
		protected EventArgs _accumulatedEventData;

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
			// send accumulated data if available and release it thereafter
			if (null != _accumulatedEventData)
			{
				var accumulatedEvent = _accumulatedEventData;
				_accumulatedEventData = null;

				var parent = _parent as Main.IChildChangedEventSink;
				if (null != parent)
				{
					parent.EhChildChanged(this, accumulatedEvent);
				}
				if (!IsSuspended)
				{
					OnChanged(accumulatedEvent); // Fire the changed event
				}
			}
		}

		protected override void OnResumeSilently()
		{
			_accumulatedEventData = null;
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
}