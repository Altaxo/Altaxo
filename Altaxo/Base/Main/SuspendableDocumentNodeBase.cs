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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Main.Services;

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
    protected IDocumentNode? _parent;

    /// <summary>Fired when something in the object has changed, and the object is not suspended.</summary>
    [field: NonSerialized]
    public event EventHandler? Changed;

    /// <summary>
    /// The event that is fired when the object is disposed. First argument is the sender, second argument is the original source, and third argument is the event arg.
    /// </summary>
    [field: NonSerialized]
    public event Action<object, object, Main.TunnelingEventArgs>? TunneledEvent;

    /// <summary>
    /// The dispose state. If 0, the instance is fully functional. If <see cref="DisposeState_DisposeInProgress"/> (1), Dispose is currently in progress. If the value is <see cref="DisposeState_Disposed"/>, this instance is disposed.
    /// </summary>
    [NonSerialized]
    private byte _disposeState;

    /// <summary>Indicates that the object is fully alive, i.e. for the object neither dispose is in progress nor the object is disposed.</summary>
    private const byte DisposeState_None = 0;

    /// <summary>Indicates that dispose is in progress.</summary>
    private const byte DisposeState_DisposeInProgress = 1;

    /// <summary>Indicates that the instance is disposed.</summary>
    private const byte DisposeState_Disposed = 2;

    /// <summary>
    /// Gets/sets the parent object this instance belongs to.
    /// </summary>
    public virtual IDocumentNode? ParentObject
    {
      get
      {
        return _parent;
      }
      set
      {
#if DEBUG && TRACEDOCUMENTNODES
        if (_parent is not null && value is not null && !object.ReferenceEquals(_parent, value))
          throw new InvalidProgramException(string.Format("Try to give object of type {0} a new parent object. Old parent: {1}, New parent {2}", this.GetType(), _parent.GetType(), value.GetType()));

        if (_parent is not null && value is null)
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
    /// Is called by this instance if anything inside this member changed.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected abstract void EhSelfChanged(EventArgs e);

    /// <summary>
    /// Fires the change event with the EventArgs provided in the argument.
    /// </summary>
    protected virtual void OnChanged(EventArgs e)
    {
      Changed?.Invoke(this, e);
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
        return !(_parent is null) || !(Changed is null) || !(TunneledEvent is null);
      }
    }

    /// <summary>
    /// Gets the name of this document node. Null is returned if the name is not set or unknown.
    /// The set accessor will for most nodes throw a <see cref="InvalidOperationException"/>, since the name can only be set on <see cref="IProjectItem"/>s.
    /// </summary>
    /// <value>
    /// The name of this instance.
    /// </value>
    public virtual string Name
    {
      get
      {
        return _parent?.GetNameOfChildObject(this) ?? throw new InvalidOperationException($"The name is not known yet. To avoid this exception, use TryGetName instead.");
      }
      set
      {
        throw new InvalidOperationException("The name of this node cannot be set. The node type is: " + GetType().FullName);
      }
    }

    /// <summary>
    /// Test if this item already has a name.
    /// </summary>
    /// <param name="name">On success, returns the name of the item.</param>
    /// <returns>True if the item already has a name; otherwise false.</returns>
    public virtual bool TryGetName([MaybeNullWhen(false)] out string name)
    {
      name = _parent?.GetNameOfChildObject(this);
      return name is not null;
    }

    #endregion Document functions

    /// <summary>
    /// Determines whether there is no or only one single event arg accumulated. If this is the case, the return value is <c>true</c>. If there is one event arg accumulated, it is returned in the argument <paramref name="singleEventArg"/>.
    /// The return value is false if there is more than one event arg accumulated. In this case the <paramref name="singleEventArg"/> is <c>null</c> on return, and the calling function should use <see cref="AccumulatedEventData"/> to
    /// enumerate all accumulated event args.
    /// </summary>
    /// <param name="singleEventArg">The <see cref="EventArgs"/> instance containing the event data, if there is exactly one event arg accumulated. Otherwise, it is <c>null</c>.</param>
    /// <returns>True if there is zero or one event arg accumulated, otherwise <c>false</c>.</returns>
    protected abstract bool AccumulatedEventData_HasZeroOrOneEventArg(out EventArgs? singleEventArg);

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
    /// Sets the change data without further processing. This function is infrastructure and intended to use only in OnResume after the parent has suspended this node again.
    /// That's why it is presumed that the accumulated event data is empty when this function is called.
    /// </summary>
    /// <param name="e">The event args (one or more).</param>
    protected abstract void AccumulatedChangeData_SetBackAfterResumeAndSuspend(params EventArgs[] e);

    /// <summary>
    /// Accumulates the change event data of the child.
    /// </summary>
    /// <param name="sender">The sender of the change event notification.</param>
    /// <param name="e">The change event args can provide details of the change.</param>
    protected abstract void AccumulateChangeData(object? sender, EventArgs e);

    /// <summary>
    /// Increase the SuspendLevel by one, and return a token that, if disposed, will resume the object.
    /// </summary>
    /// <returns>A token, which must be handed to the resume function to decrease the suspend level. Alternatively,
    /// the object can be used in an using statement. In this case, the call to the Resume function is not neccessary.</returns>
    public abstract ISuspendToken SuspendGetToken();

    /// <summary>
    /// Resumes the object  completely for the time the returned token is referenced and not disposed.
    /// The return value is a token that had 'absorbed' the suspend count of the object, resulting in the suspend count
    /// of the object dropped to 0 (zero). When the returned token is finally disposed, the suspend count of the object is increased again by the 'absorbed' suspend count.
    /// </summary>
    /// <returns>A new token. As long as this token is not disposed, and not other process calls SuspendGetToken, the object is fre (not suspended). The object is suspended again when
    /// the returned token is disposed.</returns>
    public abstract IDisposable ResumeCompleteTemporarilyGetToken();

    /// <summary>
    /// Resumes the object completely for only a short time. Thus, if object was suspended before, it will be suspended again when the function returns.
    /// </summary>
    public abstract void ResumeCompleteTemporarily();

    /// <summary>
    /// Gets a value indicating whether this instance is suspended.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is suspended; otherwise, <c>false</c>.
    /// </value>
    public abstract bool IsSuspended { get; }

    /// <summary>
    /// Resumes changed events by calling <see cref="ISuspendToken.Resume()"/> for the provided suspend token, and setting the reference to the suspend token to null.
    /// If Event data were accumulated during the suspended state, a changed event is triggered for each event data.
    /// </summary>
    /// <param name="suspendToken">The suspend token.</param>
    public void Resume(ref ISuspendToken? suspendToken)
    {
      suspendToken?.Resume();
      suspendToken = null;
    }

    /// <summary>
    /// Resumes changed events by calling <see cref="ISuspendToken.Resume()"/> for the provided suspend token, and setting the reference to the suspend token to null.
    /// All event data accumulated during the suspended state are discarded, and thus no change event is triggered even if the instance has changed during the suspended state.
    /// </summary>
    /// <param name="suspendToken">The suspend token.</param>
    public void ResumeSilently(ref ISuspendToken? suspendToken)
    {
      suspendToken?.ResumeSilently();
      suspendToken = null;
    }

    /// <summary>
    /// Resumes changed events, either with taking the accumulated event data into account (see <see cref="Resume(ref ISuspendToken)"/>) or discarding the accumulated event data (see <see cref="ResumeSilently"/>,
    /// depending on the provided argument <paramref name="eventFiring"/>.
    /// </summary>
    /// <param name="suspendToken">The suspend token.</param>
    /// <param name="eventFiring">This argument determines if the events are resumed taking the event data into account, or the resume is silent, i.e. accumulated event data are discarded.</param>
    public void Resume(ref ISuspendToken? suspendToken, EventFiring eventFiring)
    {
      suspendToken?.Resume(eventFiring);
      suspendToken = null;
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
        if (base.TryGetValue(item, out var containedItem))
        {
          var containedAsSelf = containedItem as SelfAccumulateableEventArgs;
          if (containedAsSelf is not null)
            containedAsSelf.Add((SelfAccumulateableEventArgs)item);
        }
        else // not in the collection already
        {
          base.Add(item, item);
        }
      }

      public void Add(EventArgs item)
      {
        Add(item, item);
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

    /// <summary>
    /// Gets a value indicating whether for this instance dispose is in progress, or the instance is already disposed.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is dispose in progress or already disposed; otherwise, <c>false</c>.
    /// </value>
    public bool IsDisposeInProgress { get { return _disposeState != 0; } }

    /// <summary>
    /// Sets the flag that dispose is in progress for this node and all child nodes recursively.
    /// </summary>
    public virtual void SetDisposeInProgress()
    {
      if (_disposeState == 0)
        _disposeState = DisposeState_DisposeInProgress;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
    /// </value>
    public bool IsDisposed { get { return _disposeState == DisposeState_Disposed; } }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if (_disposeState < DisposeState_DisposeInProgress)
      {
        SetDisposeInProgress();
      }

      if (_disposeState < DisposeState_Disposed)
      {
        Dispose(true);
      }

      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="SuspendableDocumentNodeBase"/> class.
    /// </summary>
    ~SuspendableDocumentNodeBase()
    {
#if DEBUG && TRACEDOCUMENTNODES
      if (!IsDisposed && _parent is not null)
      {
        string msg;
        if (this._releasedBy is null)
          msg = string.Format("Error: not disposed DocumentNode {2}\r\n{0}, constructed\r\n\tby {1}", this.GetType().FullName, this._constructedBy, this.Debug_AbsolutePath);
        else
          msg = string.Format("Error: not disposed DocumentNode {3}\r\n{0}, constructed\r\n\tby {1}\r\nreleased by\r\n\t{2}", this.GetType().FullName, this._constructedBy, this._releasedBy, this.Debug_AbsolutePath);

        System.Diagnostics.Debug.WriteLine(msg); // we may not have console in this moment, as such failures arise often while closing the application
      }
#endif

      Dispose(false);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool isDisposing)
    {
      if (!IsDisposed)
      {
        EhSelfTunnelingEventHappened(Main.DisposeEventArgs.Empty, isDisposing);
        _disposeState = DisposeState_Disposed;
        Changed = null;
        TunneledEvent = null;
        ParentObject = null;
      }
    }

    #endregion Dispose interface

    #region Implementation of Altaxo.Collections.INodeWithParentNode<IDocumentNode> and Altaxo.Collections.ITreeNode<IDocumentLeafNode>

    /// <summary>
    /// Gets the child nodes.
    /// </summary>
    /// <value>
    /// The child nodes.
    /// </value>
    IEnumerable<IDocumentLeafNode> Collections.ITreeNode<IDocumentLeafNode>.ChildNodes
    {
      get { yield break; }
    }

    /// <summary>
    /// Gets the parent node of this node.
    /// </summary>
    /// <value>
    /// The parent node.
    /// </value>
    IDocumentLeafNode? Collections.INodeWithParentNode<IDocumentLeafNode>.ParentNode
    {
      get { return _parent; }
    }

    #endregion Implementation of Altaxo.Collections.INodeWithParentNode<IDocumentNode> and Altaxo.Collections.ITreeNode<IDocumentLeafNode>

    #region Tunneling event handling

    /// <summary>
    /// Is called by the parent when a tunneling event happened into the parent.
    /// </summary>
    /// <param name="sender">The sender (i.e. the parent of this instance).</param>
    /// <param name="originalSource">The original source of the tunneling event.</param>
    /// <param name="e">The <see cref="TunnelingEventArgs"/> instance containing the event data.</param>
    public virtual void EhParentTunnelingEventHappened(IDocumentNode sender, IDocumentNode originalSource, TunnelingEventArgs e)
    {
      OnTunnelingEvent(originalSource, e);
    }

    /// <summary>
    /// Is called by this instance if a tunneling event happened into this instance.
    /// The tunneling event triggers the <see cref="TunneledEvent"/> and is - depending on the provided parameter - also distributed to all childs of this instance.
    /// </summary>
    /// <param name="e">The <see cref="TunnelingEventArgs"/> instance containing the event data.</param>
    /// <param name="distributeThisEventToChilds">if set to <c>true</c>, the tunneling event is distributed to all childs of this instance.</param>
    public virtual void EhSelfTunnelingEventHappened(TunnelingEventArgs e, bool distributeThisEventToChilds)
    {
      OnTunnelingEvent(this, e);
    }

    /// <summary>
    /// Is called by this instance if a tunneling event happened into this instance. The tunneling event triggers the <see cref="TunneledEvent"/> and is additionally distributed to all childs of this instance.
    /// </summary>
    /// <param name="e">The <see cref="TunnelingEventArgs"/> instance containing the event data.</param>
    public void EhSelfTunnelingEventHappened(TunnelingEventArgs e)
    {
      EhSelfTunnelingEventHappened(e, true);
    }

    /// <summary>
    /// Fires the <see cref="TunneledEvent"/> event.
    /// </summary>
    /// <param name="originalSource">The original source of the tunneled event.</param>
    /// <param name="e">The <see cref="TunnelingEventArgs"/> instance containing the event data.</param>
    protected virtual void OnTunnelingEvent(IDocumentLeafNode originalSource, TunnelingEventArgs e)
    {
      TunneledEvent?.Invoke(this, originalSource, e);
    }

    #endregion Tunneling event handling

    #region Helper functions

    /// <summary>
    /// Helper function to dispose a child node of this instance. It helps to ensure the correct order: first, the child node is set to null and only then the child node is disposed.
    /// </summary>
    /// <typeparam name="T">Type of child node.</typeparam>
    /// <param name="childNode">The child node to dispose.</param>
    protected void ChildDisposeMember<T>(ref T? childNode) where T : class, IDisposable
    {
      var tmpNode = childNode;
      childNode = null;
      tmpNode?.Dispose();
    }

    /// <summary>
    /// Sets a member variable of this instance and raise a change event with <see cref="System.EventArgs.Empty"/> if the new value is different from the old value.
    /// The comparison is done using the <see cref="IEquatable{T}"/> interface of the member variable.
    /// Note: to set members that implement <see cref="IDocumentNode"/> please use the Child... functions.
    /// </summary>
    /// <typeparam name="T">Type of member variable.</typeparam>
    /// <param name="memberVariable">The member variable to set.</param>
    /// <param name="value">The new value.</param>
    protected void SetMemberAndRaiseSelfChanged<T>([AllowNull][MaybeNull][NotNullIfNotNull("value")] ref T memberVariable, [AllowNull] T value) where T : IEquatable<T>?
    {
      var oldValue = memberVariable;
      memberVariable = value;

      if (oldValue is not null && value is not null)
      {
        if (!oldValue.Equals(value))
          EhSelfChanged(EventArgs.Empty);
      }
      else if (oldValue is not null || value is not null)
      {
        EhSelfChanged(EventArgs.Empty);
      }
    }


    /// <summary>
    /// Sets a member variable of this instance and raise a change event with <see cref="System.EventArgs.Empty"/> if the new value is different from the old value.
    /// The comparison is done using the <see cref="IEquatable{T}"/> interface of the member variable.
    /// Note: to set members that implement <see cref="IDocumentNode"/> please use the Child... functions.
    /// </summary>
    /// <typeparam name="T">Type of member variable.</typeparam>
    /// <param name="memberVariable">The member variable to set.</param>
    /// <param name="value">The new value.</param>
    protected void SetMemberAndRaiseSelfChanged<T>(ref T? memberVariable, T? value) where T : struct, IEquatable<T>
    {
      var oldValue = memberVariable;
      memberVariable = value;

      if (oldValue is not null && value is not null)
      {
        if (!oldValue.Value.Equals(value.Value))
          EhSelfChanged(EventArgs.Empty);
      }
      else if (oldValue is not null || value is not null)
      {
        EhSelfChanged(EventArgs.Empty);
      }
    }


    /// <summary>
    /// Sets a member variable (which is an Enum) of this instance and raise a change event with <see cref="System.EventArgs.Empty"/> if the new value is different from the old value.
    /// The comparison is done using the <see cref="IEquatable{T}"/> interface of the member variable.
    /// Note: to set members that implement <see cref="IDocumentNode"/> please use the Child... functions.
    /// </summary>
    /// <typeparam name="T">Type of member variable.</typeparam>
    /// <param name="memberVariable">The member variable to set.</param>
    /// <param name="value">The new value.</param>
    protected void SetMemberEnumAndRaiseSelfChanged<T>(ref T memberVariable, T value)
    {
      var oldValue = memberVariable;
      memberVariable = value;

      if (oldValue is not null && value is not null)
      {
        if (!oldValue.Equals(value))
          EhSelfChanged(EventArgs.Empty);
      }
      else if (oldValue is not null || value is not null)
      {
        EhSelfChanged(EventArgs.Empty);
      }
    }

    #endregion Helper functions

    #region Diagnostic support

    /// <summary>
    /// Gets the absolute path of the node for debugging purposes.
    /// </summary>
    /// <value>
    /// The absolute path.
    /// </value>
    protected string? Debug_AbsolutePath
    {
      get
      {
        var rootNode = AbsoluteDocumentPath.GetRootNode(this);
        return RelativeDocumentPath.GetRelativePathFromTo(rootNode, this)?.ToString();
      }
    }

#if DEBUG && TRACEDOCUMENTNODES

    protected static LinkedList<WeakReference> _allDocumentNodes = new LinkedList<WeakReference>();

    private static int _nextID;
    private string _constructedBy;
    private string _releasedBy;
    private int _instanceID = _nextID++;

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
          while (lnode is not null)
          {
            var nextNode = lnode.Next;
            var target = lnode.Value.Target as SuspendableDocumentNodeBase;
            if (target is not null)
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

    /// <summary>
    /// Reports not connected document nodes, i.e. child nodes having no parent.
    /// </summary>
    /// <param name="showStatistics">If set to <c>true</c> a line with statistic information is printed into Altaxo's console.</param>
    /// <returns>True if there were not connected documen nodes; otherwise false.</returns>
    public static bool ReportNotConnectedDocumentNodes(bool showStatistics)
    {
      int numberOfNodes = 0;
      int numberOfNotConnectedNodes = 0;
      GC.Collect();

      var msgDict = new SortedDictionary<string, int>(); // Key is the message, value the number of nodes

      foreach (var node in AllDocumentNodes)
      {
        if (node.ParentObject is null && !object.ReferenceEquals(node, Current.Project))
        {
          string msg;
          if (node._releasedBy is null)
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

      if (showStatistics)
        Current.Console.WriteLine("Tested {0} nodes, {1} not connected", numberOfNodes, numberOfNotConnectedNodes);
      return 0 != numberOfNotConnectedNodes;
    }

#else

    /// <summary>
    /// Reports not connected document nodes, i.e. child nodes having no parent. This functionality is available only in DEBUG mode with TRACEDOCUMENTNODES defined in AltaxoBase.
    /// </summary>
    /// <param name="showStatistics">If set to <c>true</c> a line with statistic information is printed into Altaxo's console.</param>
    /// <returns>True if there were not connected documen nodes; otherwise false.</returns>
    public static bool ReportNotConnectedDocumentNodes(bool showStatistics)
    {
      GC.Collect();
      Current.InfoTextMessageService.WriteLine(MessageLevel.Error, "ReportNotConnectedDocumentNodes", "ReportNotConnectedDocumentNodes: This functionality is available only in DEBUG mode with TRACEDOCUMENTNODES defined in AltaxoBase");
      return false;
    }

#endif

    #endregion Diagnostic support
  }
}
