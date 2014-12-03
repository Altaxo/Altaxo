using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	public abstract class SuspendableDocumentNode : Main.SuspendableObject, Main.IDocumentNode, Main.IChangedEventSource, Main.IChildChangedEventSink
	{
		/// <summary>The parent of this object.</summary>
		protected object _parent;

		/// <summary>Stores the suspend tokens of the suspended childs of this object.</summary>
		protected HashSet<IDisposable> _suspendTokensOfChilds;

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
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		/// <returns>True if the event has not changed the state of the table (i.e. it requires no further action).</returns>
		protected virtual bool HandleHighPriorityChildChangeCases(object sender, EventArgs e)
		{
			return false;
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
		/// Used by childrens of the table to inform the table of a change in their data.
		/// </summary>
		/// <param name="sender">The sender of the change notification.</param>
		/// <param name="e">The change details.</param>
		public void EhChildChanged(object sender, System.EventArgs e)
		{
			if (HandleHighPriorityChildChangeCases(sender, e))
				return;

			if (!IsSuspendedOrResumeInProgress)
			{
				if (HandleLowPriorityChildChangeCases(sender, ref e))
					return;

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

			// at this point we are suspended for sure
			CountEvent();
			if (sender is Main.ISuspendableByToken)
			{
				_suspendTokensOfChilds.Add(((Main.ISuspendableByToken)sender).SuspendGetToken()); // add sender to suspended child
			}
			else
			{
				AccumulateChangeData(sender, e);  // child is unable to accumulate change data, we have to to it by ourself
			}
		}

		/// <summary>
		/// Called if some member of this instance itself has changed.
		/// </summary>
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
		/// Is called when the suppress level falls down from 1 to zero and the event count is != 0.
		/// Per default, the resume event handler is called that you provided in the constructor.
		/// </summary>
		/// <param name="eventCount">The event count.</param>
		protected override void OnResume(int eventCount)
		{
			foreach (var obj in _suspendTokensOfChilds)
				obj.Dispose();
			_suspendTokensOfChilds.Clear();

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
}