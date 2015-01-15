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
	/// Base class for a suspendable document node.
	/// This class supports document nodes that have children, and implements most of the code neccessary to handle child events and to suspend the childs when the parent is suspended.
	/// </summary>
	/// <remarks>If you don't need support for child events, consider using <see cref="T:Altaxo.Main.SuspendableDocumentLeafNode{TEventArgs}"/> instead.</remarks>
	public abstract partial class SuspendableDocumentNode : SuspendableDocumentNodeBase, Main.IDocumentNode
	{
		/// <summary>How many times was the Suspend function called (without corresponding Resume)</summary>
		private int _suspendLevel;

		/// <summary>Counts the number of events during the suspend phase.</summary>
		private int _eventCount;

		/// <summary>
		/// If the resume operation is currently in progress, this member is <c>1</c>. Otherwise, it is 0.
		/// </summary>
		private int _resumeInProgress;

		/// <summary>Stores the suspend tokens of the suspended childs of this object.</summary>
		protected HashSet<ISuspendToken> _suspendTokensOfChilds = new HashSet<ISuspendToken>();

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
		/// <param name="eventCount">The event count. The event count is the number of times the <see cref="CountEvent"/> function was called during the suspended state.</param>
		protected virtual void OnAboutToBeResumed(int eventCount)
		{
		}

		/// <summary>
		/// Is called when the object is resumed, i.e. change notifications are allowed again. During the execution of this function, the <see cref="P:IsResumeInProgress"/> property will return <c>true</c> to indicate that the resume is currently in progress.
		/// </summary>
		/// <param name="eventCount">The event count. The event count is the number of times the <see cref="M:CountEvent"/> function was called during the suspend state.</param>
		protected virtual void OnResume(int eventCount)
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
		protected virtual void OnResumeSilently(int eventCount)
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

		#endregion Call back functions by the suspendToken

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

		/// <summary>
		/// Gets a value indicating whether this instance is currently resuming the events.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if resume is in progress; otherwise, <c>false</c>.
		/// </value>
		public bool IsResumeInProgress { get { return _resumeInProgress > 0; } }

		/// <summary>
		/// Gets a value indicating whether this instance is suspended or resume is currently in progress.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is suspended or resume is currently in progress; otherwise, <c>false</c>.
		/// </value>
		public bool IsSuspendedOrResumeInProgress { get { return _resumeInProgress != 0 || _suspendLevel != 0; } }

		/// <summary>
		/// Counts the number of events during the suspend state. Every call to this function will increment the event counter by 1 (but only in the suspended state). The event counter will be reset to zero when the object is resumed.
		/// </summary>
		public void CountEvent()
		{
			if (_suspendLevel != 0)
			{
				_eventCount++;
			}
		}

		#endregion Suspend state questions

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
		/// and ii) optional transforming the event args, for instance to a new type, which afterwards is send to the parent and is used as event args in the <see cref="E:Changed"/> event of this object.
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
				if (null != _parent && !_parent.IsDisposeInProgress)
				{
					_parent.EhChildChanged(this, eventArgsTransformed); // inform parent with transformed event args. Attention: parent may change our suspend state
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
				if (null != _parent && !_parent.IsDisposeInProgress)
				{
					_parent.EhChildChanged(this, e); // parent may change our suspend state
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

		#endregion Change event handling

		#region Tunneling event handling

		public override void EhParentTunnelingEventHappened(IDocumentNode sender, IDocumentNode originalSource, TunnelingEventArgs e)
		{
			OnTunnelingEvent(originalSource, e);
			NotifyChildrenTunnelingEventHappened(originalSource, e);
		}

		public override void EhSelfTunnelingEventHappened(TunnelingEventArgs e, bool distributeThisEventToChilds)
		{
			OnTunnelingEvent(this, e);

			if (distributeThisEventToChilds)
				NotifyChildrenTunnelingEventHappened(this, e);
		}

		protected virtual void NotifyChildrenTunnelingEventHappened(IDocumentNode originalSource, TunnelingEventArgs e)
		{
			foreach (var tuple in GetDocumentNodeChildrenWithName())
			{
				var child = tuple.DocumentNode;
				child.EhParentTunnelingEventHappened(this, originalSource, e);
			}
		}

		protected abstract IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName();

		public virtual IDocumentLeafNode GetChildObjectNamed(string name)
		{
			if (null == name)
				throw new ArgumentNullException("name");

			return GetDocumentNodeChildrenWithName().FirstOrDefault(tuple => tuple.Name == name).DocumentNode;
		}

		public virtual string GetNameOfChildObject(IDocumentLeafNode docNode)
		{
			if (null == docNode)
				throw new ArgumentNullException("docNode");

			return GetDocumentNodeChildrenWithName().FirstOrDefault(tuple => object.ReferenceEquals(tuple.DocumentNode, docNode)).Name;
		}

		#endregion Tunneling event handling

		#region Dispose interface

		protected override void Dispose(bool isDisposing)
		{
			if (!IsDisposed)
			{
				if (isDisposing) // Dispose all childs (but only if calling dispose, and not in the finalizer)
				{
					Action setMemberToNullAction;
					foreach (var tuple in GetDocumentNodeChildrenWithName())
					{
						if (null != (setMemberToNullAction = tuple.SetMemberToNullAction))
							setMemberToNullAction(); // set the node to null in the parent __before__ we dispose the node

						tuple.DocumentNode.Dispose();
					}
				}
				base.Dispose(isDisposing);
			}
		}

		#endregion Dispose interface

		#region Inner class SuspendToken

		private class SuspendToken : ISuspendToken
		{
			private SuspendableDocumentNode _parent;

			internal SuspendToken(SuspendableDocumentNode parent)
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
				{
					var parent = System.Threading.Interlocked.Exchange<SuspendableDocumentNode>(ref _parent, null);
					if (parent != null)
					{
						int newLevel = System.Threading.Interlocked.Decrement(ref parent._suspendLevel);

						if (0 == newLevel)
						{
							System.Threading.Interlocked.Increment(ref parent._resumeInProgress);
							try
							{
								var count = parent._eventCount;
								parent._eventCount = 0;
								parent.OnResumeSilently(count);
							}
							finally
							{
								System.Threading.Interlocked.Decrement(ref parent._resumeInProgress);
							}
						}
						else if (newLevel < 0)
						{
							throw new ApplicationException("Fatal programming error - suppress level has fallen down to negative values");
						}
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
				var parent = System.Threading.Interlocked.Exchange<SuspendableDocumentNode>(ref _parent, null);
				if (parent != null)
				{
					Exception exceptionInAboutToBeResumed = null;
					if (1 == parent._suspendLevel)
					{
						try
						{
							parent.OnAboutToBeResumed(parent._eventCount);
						}
						catch (Exception ex)
						{
							exceptionInAboutToBeResumed = ex;
						}
					}

					int newLevel = System.Threading.Interlocked.Decrement(ref parent._suspendLevel);

					if (0 == newLevel)
					{
						System.Threading.Interlocked.Increment(ref parent._resumeInProgress);
						try
						{
							var count = parent._eventCount;
							parent._eventCount = 0;
							parent.OnResume(count);
						}
						finally
						{
							System.Threading.Interlocked.Decrement(ref parent._resumeInProgress);
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

		#region Helper functions

		/// <summary>
		/// Set a member variable that holds a child node of this instance.  It helps to ensure the correct order: first, the child node is set to the new instance and then the  old child node is disposed.
		/// </summary>
		/// <typeparam name="T">Type of child node.</typeparam>
		/// <param name="childNode">The child node member variable to set.</param>
		/// <param name="instanceToSet">The instance to set the variable with.</param>
		/// <returns><c>True</c> if the child has been set. If the old child reference equals to the new child, nothing is done, and <c>false</c> is returned.</returns>
		protected bool ChildSetMember<T>(ref T childNode, T instanceToSet) where T : class, IDocumentLeafNode
		{
			if (object.ReferenceEquals(childNode, instanceToSet))
				return false;

			var tmpNode = childNode;

			childNode = instanceToSet;
			if (null != childNode)
				childNode.ParentObject = this;

			if (null != tmpNode)
				tmpNode.Dispose();

			return true;
		}

		/// <summary>
		/// Set a member variable that holds a child node of this instance. The child node may or may not implement <see cref="IDocumentLeafNode"/>.
		/// It helps to ensure the correct order: first, the child node is set to the new instance and then the  old child node is disposed.
		/// </summary>
		/// <typeparam name="T">Type of child node.</typeparam>
		/// <param name="childNode">The child node member variable to set.</param>
		/// <param name="instanceToSet">The instance to set the variable with.</param>
		/// <returns><c>True</c> if the child has been set. If the old child reference equals to the new child, nothing is done, and <c>false</c> is returned.</returns>
		protected bool ChildSetMemberAlt<T>(ref T childNode, T instanceToSet) where T : class
		{
			if (object.ReferenceEquals(childNode, instanceToSet))
				return false;

			var tmpNode = childNode;

			childNode = instanceToSet;
			if (childNode is IDocumentLeafNode)
				((IDocumentLeafNode)childNode).ParentObject = this;

			if (tmpNode is IDisposable)
				((IDisposable)tmpNode).Dispose();

			return true;
		}

		/// <summary>
		/// Copies a document node from another source into a member of this instance.
		/// If an old instance member (provided in <paramref name="myChild"/> exists and can not be used, it is disposed first.
		/// The node is then copied using either Main.ICopyFrom or System.ICloneable. The resulting node's <see cref="M:IDocumentLeafNode.ParentObject"/>
		/// is then set to this instance in order to maintain the parent-child relationship.
		/// </summary>
		/// <typeparam name="T">Type of the node to copy.</typeparam>
		/// <param name="myChild">Reference to a member variable of this instance that holds a child node.</param>
		/// <param name="fromAnotherChild">Another child node to copy from. If null, the child node of this instance is also set to null.</param>
		protected bool ChildCopyToMember<T>(ref T myChild, T fromAnotherChild) where T : IDocumentLeafNode, ICloneable
		{
			if (object.ReferenceEquals(myChild, fromAnotherChild))
				return false;

			var oldChild = myChild;

			if (null == fromAnotherChild)
			{
				myChild = default(T);
				if (null != oldChild)
					oldChild.Dispose();
			}
			else if ((myChild is Main.ICopyFrom) && myChild.GetType() == fromAnotherChild.GetType())
			{
				((Main.ICopyFrom)myChild).CopyFrom(fromAnotherChild);
			}
			else
			{
				myChild = (T)(fromAnotherChild.Clone());
				myChild.ParentObject = this;

				if (null != oldChild)
					oldChild.Dispose();
			}

			return true;
		}

		/// <summary>
		/// Copies a document node from another source into a member of this instance.
		/// If an old instance member (provided in <paramref name="myChild"/> exists and can not be used, it is disposed first.
		/// If the node is not null, the node is then copied using either Main.ICopyFrom or System.ICloneable. If the node is <c>null</c>, a new node is created using the provided generation function.
		/// The resulting node's <see cref="M:IDocumentLeafNode.ParentObject"/> is then set to this instance in order to maintain the parent-child relationship.
		/// </summary>
		/// <typeparam name="T">Type of the node to copy.</typeparam>
		/// <param name="myChild">Reference to a member variable of this instance that holds a child node.</param>
		/// <param name="fromAnotherChild">Another child node to copy from. If null, the child node of this instance is also set to null.</param>
		/// <param name="createNew">If the parameter <paramref name="fromAnotherChild"/> is null, the provided function is used to create a new object of type <typeparamref name="T"/>. This object is then used to set the member.</param>
		protected bool ChildCopyToMemberOrCreateNew<T>(ref T myChild, T fromAnotherChild, Func<T> createNew) where T : class, IDocumentLeafNode, ICloneable
		{
			if (null != fromAnotherChild)
			{
				return ChildCopyToMember(ref myChild, fromAnotherChild);
			}
			else
			{
				return ChildSetMember(ref myChild, createNew());
			}
		}

		/// <summary>
		/// Sets a member variable that holds a child with a cloned instance of another variable.
		/// If an old instance member (provided in <paramref name="myChild"/> exists and can not be used, it is disposed first.
		/// The node is then cloned using System.ICloneable. The resulting node's <see cref="M:IDocumentLeafNode.ParentObject"/>
		/// is then set to this instance in order to maintain the parent-child relationship.
		/// </summary>
		/// <typeparam name="T">Type of the node to copy.</typeparam>
		/// <param name="myChild">Reference to a member variable of this instance that holds a child node.</param>
		/// <param name="fromAnotherChild">Another child node to copy from. If null, the child node of this instance is also set to null.</param>
		protected void ChildCloneToMember<T>(ref T myChild, T fromAnotherChild) where T : IDocumentLeafNode, ICloneable
		{
			if (object.ReferenceEquals(myChild, fromAnotherChild))
				return;

			if (null == fromAnotherChild)
			{
				if (null != myChild)
					myChild.Dispose();
				myChild = default(T);
			}
			else
			{
				if (null != myChild)
					myChild.Dispose();
				myChild = (T)(fromAnotherChild.Clone());
			}

			if (null != myChild)
				myChild.ParentObject = this;
		}

		#endregion Helper functions

		IEnumerable<IDocumentLeafNode> Collections.ITreeNode<IDocumentLeafNode>.ChildNodes
		{
			get { return GetDocumentNodeChildrenWithName().Select(x => x.DocumentNode); }
		}

		IDocumentLeafNode Collections.INodeWithParentNode<IDocumentLeafNode>.ParentNode
		{
			get { return _parent; }
		}

		#region Diagnostic support

#if DEBUG && TRACEDOCUMENTNODES

		public static bool ReportChildListProblems()
		{
			bool areThereAnyProblems = false;
			GC.Collect();

			var childListErrors = new SortedSet<string>();

			foreach (var node in AllDocumentNodes)
			{
				var parent = node.ParentObject as SuspendableDocumentNode;
				if (null == parent)
					continue;

				var tuple = parent.GetDocumentNodeChildrenWithName().FirstOrDefault(x => object.ReferenceEquals(node, x.DocumentNode));

				if (tuple.DocumentNode == null)
				{
					childListErrors.Add(string.Format("Parent of type {0} did not list child node of type {1}, which was constructed {2}", parent.GetType().FullName, node.GetType().FullName, node.ConstructedBy));
					areThereAnyProblems = true;
				}
				else
				{
					var nameOfNode1 = tuple.Name;
					if (null == nameOfNode1)
					{
						Current.Console.WriteLine("Parent of type {0} lists child node of type {1}, but without a name", parent.GetType().FullName, node.GetType().FullName);
						areThereAnyProblems = true;
					}

					var nameOfNode2 = parent.GetNameOfChildObject(node);
					if (nameOfNode2 != nameOfNode1)
					{
						Current.Console.WriteLine("Parent of type {0} has child node of type {1}, Name: {2}, but GetNameOfChildObject returns a different name ({3})", parent.GetType().FullName, node.GetType().FullName, nameOfNode1, nameOfNode2);
						areThereAnyProblems = true;
					}

					var nodeAlt = parent.GetChildObjectNamed(nameOfNode1);

					if (!object.ReferenceEquals(node, nodeAlt))
					{
						Current.Console.WriteLine("Parent of type {0} has child node of type {1}, Name: {2}, but GetChildObjectNamed returns a different object ({3})", parent.GetType().FullName, node.GetType().FullName, nameOfNode1, nodeAlt);
						areThereAnyProblems = true;
					}
				}
			}

			foreach (var error in childListErrors)
			{
				Current.Console.WriteLine(error);
			}

			return areThereAnyProblems;
		}

		public static bool ReportWrongChildParentRelations()
		{
			bool areThereAnyProblems = false;
			GC.Collect();

			var hashOfAllNodes = new HashSet<IDocumentLeafNode>(_allDocumentNodes.Where(x => x.IsAlive).Select(x => (IDocumentLeafNode)x.Target));

			var errors = new HashSet<string>();

			foreach (var parentNode in hashOfAllNodes.OfType<SuspendableDocumentNode>())
			{
				foreach (var entry in parentNode.GetDocumentNodeChildrenWithName())
				{
					var child = entry.DocumentNode;

					if (!object.ReferenceEquals(child.ParentObject, parentNode))
					{
						errors.Add(string.Format("Parent of type {0} has child node of type {1} whose ParentObject (type {2}) is not identical with the Parent", parentNode.GetType().FullName, child.GetType().FullName, child.ParentObject == null ? "<<NULL>>" : child.ParentObject.GetType().FullName));
						areThereAnyProblems = true;
					}
				}
			}

			foreach (var error in errors)
			{
				Current.Console.WriteLine(error);
			}

			return areThereAnyProblems;
		}

#else

		public static bool ReportChildListProblems()
		{
			Current.Console.WriteLine("ReportChildListProblems: This functionality is available only in DEBUG mode with TRACEDOCUMENTNODES defined in AltaxoBase");
			return false;
		}

		public static bool ReportWrongChildParentRelations()
		{
			Current.Console.WriteLine("ReportWrongChildParentRelations: This functionality is available only in DEBUG mode with TRACEDOCUMENTNODES defined in AltaxoBase");
			return false;
		}

#endif

		#endregion Diagnostic support
	}
}