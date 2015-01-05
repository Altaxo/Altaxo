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
	///
	/// </summary>
	public abstract class SuspendableDocumentNodeBase : Main.IDocumentLeafNode
	{
		#region Document functions

		/// <summary>
		/// The parent object this instance belongs to.
		/// </summary>
		[NonSerialized]
		protected IDocumentNode _parent;

		/// <summary>Fired when something in the object has changed, and the object is not suspended.</summary>
		[field: NonSerialized]
		public event EventHandler Changed;

		/// <summary>
		/// The event that is fired when the object is disposed. First argument is the sender, second argument is the original source, and third argument is the event arg.
		/// </summary>
		[field: NonSerialized]
		public event Action<object, object, Main.TunnelingEventArgs> TunneledEvent;

		/// <summary>
		/// Gets/sets the parent object this instance belongs to.
		/// </summary>
		public virtual IDocumentNode ParentObject
		{
			get
			{
				return _parent;
			}
			set
			{
#if DEBUG && TRACEDOCUMENTNODES
				if (null != _parent && null == value)
				{
					var stb = new System.Text.StringBuilder();
					var st = new System.Diagnostics.StackTrace(true);

					var len = Math.Min(11, st.FrameCount);
					for (int i = 1; i < len; ++i)
					{
						var frame = st.GetFrame(i);
						var method = frame.GetMethod();

						if (i > 2) stb.Append("\r\n\tin ");

						stb.Append(method.DeclaringType.FullName);
						stb.Append("|");
						stb.Append(method.Name);
						stb.Append("(L");
						stb.Append(frame.GetFileLineNumber());
						stb.Append(")");
					}
					_releasedBy = stb.ToString();
				}
#endif

				_parent = value;
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
		/// Gets the name of this document node. The set accessor will for most nodes throw a <see cref="InvalidOperationException"/>, since the name can only be set on <see cref="IProjectItem"/>s.
		/// </summary>
		/// <value>
		/// The name of this instance.
		/// </value>
		public virtual string Name
		{
			get
			{
				return _parent == null ? null : _parent.GetNameOfChildObject(this);
			}
			set
			{
				throw new InvalidOperationException("The name of this node cannot be set. The node type is: " + this.GetType().FullName);
			}
		}

		#endregion Document functions

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
		/// Accumulates the change data of the child. Currently only a flag is set to signal that the table has changed.
		/// </summary>
		/// <param name="sender">The sender of the change notification (currently unused).</param>
		/// <param name="e">The change event args can provide details of the change (currently unused).</param>
		protected abstract void AccumulateChangeData(object sender, EventArgs e);

		/// <summary>
		/// Increase the SuspendLevel by one, and return a token that, if disposed, will resume the object.
		/// </summary>
		/// <returns>A token, which must be handed to the resume function to decrease the suspend level. Alternatively,
		/// the object can be used in an using statement. In this case, the call to the Resume function is not neccessary.</returns>
		public abstract ISuspendToken SuspendGetToken();

		/// <summary>
		/// Gets a value indicating whether this instance is suspended.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is suspended; otherwise, <c>false</c>.
		/// </value>
		public abstract bool IsSuspended { get; }

		public void Resume(ref ISuspendToken suspendToken)
		{
			if (null != suspendToken)
			{
				suspendToken.Resume();
				suspendToken = null;
			}
		}

		public void ResumeSilently(ref ISuspendToken suspendToken)
		{
			if (null != suspendToken)
			{
				suspendToken.ResumeSilently();
				suspendToken = null;
			}
		}

		public void Resume(ref ISuspendToken suspendToken, EventFiring eventFiring)
		{
			if (null != suspendToken)
			{
				suspendToken.Resume(eventFiring);
				suspendToken = null;
			}
		}

		#region Implementation of a set of accumulated event data

		protected class SetOfEventData : Dictionary<EventArgs, EventArgs>, ISetOfEventData
		{
			/// <summary>
			/// Puts the specified item in the collection, regardless whether it is already contained or not. If it is not already contained, it is added to the collection.
			/// If it is already contained, and is of type <see cref="SelfAccumulateableEventArgs"/>, the <see cref="SelfAccumulateableEventArgs.Add"/> function is used to add the item to the already contained item.
			/// </summary>
			/// <param name="item">The <see cref="EventArgs"/> instance containing the event data.</param>
			public void SetOrAccumulate(EventArgs item)
			{
				EventArgs containedItem;
				if (base.TryGetValue(item, out containedItem))
				{
					var containedAsSelf = containedItem as SelfAccumulateableEventArgs;
					if (null != containedAsSelf)
						containedAsSelf.Add((SelfAccumulateableEventArgs)item);
				}
				else // not in the collection already
				{
					base.Add(item, item);
				}
			}

			public void Add(EventArgs item)
			{
				this.Add(item, item);
			}

			public bool Contains(EventArgs item)
			{
				return base.ContainsKey(item);
			}

			public void CopyTo(EventArgs[] array, int arrayIndex)
			{
				base.Values.CopyTo(array, arrayIndex);
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public new IEnumerator<EventArgs> GetEnumerator()
			{
				return base.Values.GetEnumerator();
			}
		}

		#endregion Implementation of a set of accumulated event data

		#region Dispose interface

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SuspendableDocumentNodeBase()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (_parent != null)
			{
				EhSelfTunnelingEventHappened(Main.DisposeEventArgs.Empty, isDisposing);
				Changed = null;
				TunneledEvent = null;
				this.ParentObject = null;
			}
		}

		#endregion Dispose interface

		#region Implementation of Altaxo.Collections.INodeWithParentNode<IDocumentNode> and Altaxo.Collections.ITreeNode<IDocumentLeafNode>

		IDocumentNode Collections.INodeWithParentNode<IDocumentNode>.ParentNode
		{
			get { return _parent; }
		}

		IEnumerable<IDocumentLeafNode> Collections.ITreeNode<IDocumentLeafNode>.ChildNodes
		{
			get { yield break; }
		}

		IDocumentLeafNode Collections.INodeWithParentNode<IDocumentLeafNode>.ParentNode
		{
			get { return _parent; }
		}

		#endregion Implementation of Altaxo.Collections.INodeWithParentNode<IDocumentNode> and Altaxo.Collections.ITreeNode<IDocumentLeafNode>

		#region Tunneling event handling

		public virtual void EhParentTunnelingEventHappened(IDocumentNode sender, IDocumentNode originalSource, TunnelingEventArgs e)
		{
			OnTunnelingEvent(originalSource, e);
		}

		public virtual void EhSelfTunnelingEventHappened(TunnelingEventArgs e, bool distributeThisEventToChilds)
		{
			OnTunnelingEvent(this, e);
		}

		public void EhSelfTunnelingEventHappened(TunnelingEventArgs e)
		{
			EhSelfTunnelingEventHappened(e, true);
		}

		protected virtual void OnTunnelingEvent(IDocumentLeafNode originalSource, TunnelingEventArgs e)
		{
			var ev = TunneledEvent;
			if (null != ev)
				ev(this, originalSource, e);
		}

		#endregion Tunneling event handling

		#region Diagnostic support

#if DEBUG && TRACEDOCUMENTNODES

		protected static LinkedList<WeakReference> _allDocumentNodes = new LinkedList<WeakReference>();

		private string _constructedBy;
		private string _releasedBy;

		public string ConstructedBy { get { return _constructedBy; } }

		public string ReleasedBy { get { return _releasedBy; } }

		public SuspendableDocumentNodeBase()
		{
			_allDocumentNodes.AddLast(new WeakReference(this));

			var stb = new System.Text.StringBuilder();
			var st = new System.Diagnostics.StackTrace(true);

			var len = Math.Min(11, st.FrameCount);
			for (int i = 2; i < len; ++i)
			{
				var frame = st.GetFrame(i);
				var method = frame.GetMethod();

				if (i > 2) stb.Append("\r\n\tin ");

				stb.Append(method.DeclaringType.FullName);
				stb.Append("|");
				stb.Append(method.Name);
				stb.Append("(L");
				stb.Append(frame.GetFileLineNumber());
				stb.Append(")");
			}
			_constructedBy = stb.ToString();
		}

		public static IEnumerable<SuspendableDocumentNodeBase> AllDocumentNodes
		{
			get
			{
				if (_allDocumentNodes.Count != 0)
				{
					var lnode = _allDocumentNodes.First;
					while (null != lnode)
					{
						var nextNode = lnode.Next;
						var target = lnode.Value.Target as SuspendableDocumentNodeBase;
						if (null != target)
							yield return target;
						else
							_allDocumentNodes.Remove(lnode);

						lnode = nextNode;
					}
				}
			}
		}

#endif

#if DEBUG && TRACEDOCUMENTNODES

		public static bool ReportNotConnectedDocumentNodes()
		{
			int numberOfNodes = 0;
			int numberOfNotConnectedNodes = 0;
			GC.Collect();

			var msgDict = new SortedDictionary<string, int>(); // Key is the message, value the number of nodes

			foreach (var node in AllDocumentNodes)
			{
				if (node.ParentObject == null && !object.ReferenceEquals(node, Current.Project))
				{
					string msg;
					if (null == node._releasedBy)
						msg = string.Format("{0}, constructed\r\n\tby {1}", node.GetType().FullName, node._constructedBy);
					else
						msg = string.Format("{0}, constructed\r\n\tby {1}\r\nreleased by\r\n\t{2}", node.GetType().FullName, node._constructedBy, node._releasedBy);

					int count;
					if (msgDict.TryGetValue(msg, out count))
						msgDict[msg] = count + 1;
					else
						msgDict.Add(msg, 1);

					++numberOfNotConnectedNodes;
				}

				++numberOfNodes;
			}

			foreach (var entry in msgDict)
			{
				Current.Console.WriteLine("Found {0} not connected document node(s) of type {1}", entry.Value, entry.Key);
				Current.Console.WriteLine();
			}

			Current.Console.WriteLine("Tested {0} nodes, {1} not connected", numberOfNodes, numberOfNotConnectedNodes);
			return 0 != numberOfNotConnectedNodes;
		}

#else

		public static bool ReportNotConnectedDocumentNodes()
		{
			Current.Console.WriteLine("ReportNotConnectedDocumentNodes: This functionality is available only in DEBUG mode with TRACEDOCUMENTNODES defined in AltaxoBase");
			return false;
		}

#endif

		#endregion Diagnostic support
	}
}