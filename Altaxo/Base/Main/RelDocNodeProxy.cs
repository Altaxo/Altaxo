#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Main
{
  /// <summary>
  /// DocNodeProxy holds a reference to an object. If the object is a document node (implements <see cref="IDocumentLeafNode" />), then special
  /// measures are used in the case the document node is disposed. In this case the relative path to the node (from a parent object) is stored, and if a new document node with
  /// that path exists, the reference to the object is restored.
  /// </summary>
  public class RelDocNodeProxy
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs
  {
    #region Members

    /// <summary>
    /// Holds a weak reference to the docNode being proxied.
    /// </summary>
    private WeakReference _docNodeRef;

    /// <summary>
    /// The relative path from our parent object to the docNode being proxied.
    /// </summary>
    private Main.RelativeDocumentPath _docNodePath;

    /// <summary>
    /// Weak handler that is called when the docNode has changed, or if there is no resolved docNode, when the watched node changed.
    /// </summary>
    [NonSerialized]
    protected WeakEventHandler _weakDocNodeChangedHandler;

    /// <summary>
    /// Weak handler that is called when the docNode has a TunneledEvent, or if there is no resolved docNode, when the watched node has a Tunneled event.
    /// </summary>	[NonSerialized]
    protected WeakActionHandler<object, object, TunnelingEventArgs> _weakDocNodeTunneledEventHandler;

    /// <summary>
    /// Fired if the document instance changed, (from null to some value, from a value to null, or from a value to another value).
    /// </summary>
    public event EventHandler<Main.InstanceChangedEventArgs> DocumentInstanceChanged;

    #endregion Members

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Main.RelDocNodeProxy", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version not supported");
        /*
                RelDocNodeProxy s = (RelDocNodeProxy)obj;
                Main.DocumentPath path;
                var docNode = s.InternalDocNode;
                if (null != docNode)
                {
                    path = Main.DocumentPath.GetRelativePathFromTo(s._parent, (Main.IDocumentLeafNode)docNode);
                }
                else
                {
                    path = s._docNodePath;
                }
                info.AddValue("Node", path);
                */
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        if (!(parent is Main.IDocumentNode))
          throw new ArgumentException("Parent should be a valid document node");

        var docNodePath = info.GetValue("Node", null) as RelativeDocumentPath;

        if (null == docNodePath || docNodePath.IsIdentity)
          return null;

        var s = (RelDocNodeProxy)o ?? new RelDocNodeProxy(docNodePath, (IDocumentNode)parent);

        // create a callback to resolve the instance as early as possible
        info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(s.EhXmlDeserializationFinished);

        return s;
      }
    }

    /// <summary>
    /// 2014-01-09 Using new class RelativeDocumentPath instead of DocumentPath
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RelDocNodeProxy), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RelDocNodeProxy)obj;
        if (!(s._parent != null))
          throw new InvalidProgramException();
        Main.RelativeDocumentPath path;
        var docNode = s.InternalDocNode;
        if (null != docNode)
        {
          path = Main.RelativeDocumentPath.GetRelativePathFromTo(s._parent, docNode);
        }
        else
        {
          path = s._docNodePath;
        }
        info.AddValue("Node", path);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        if (!(parent is Main.IDocumentNode))
          throw new ArgumentException("Parent should be a valid document node");

        var docNodePath = (Main.RelativeDocumentPath)info.GetValue("Node", null);

        if (null == docNodePath || docNodePath.IsIdentity)
          return null;

        var s = (RelDocNodeProxy)o ?? new RelDocNodeProxy(docNodePath, (Main.IDocumentNode)parent);

        // create a callback to resolve the instance as early as possible
        info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(s.EhXmlDeserializationFinished);

        return s;
      }
    }

    private void EhXmlDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinallyCall)
    {
      if (Document != null || isFinallyCall)
        info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(EhXmlDeserializationFinished);
    }

    #endregion Serialization

    #region Construction

    /// <summary>
    /// For deserialization purposes only.
    /// </summary>
    /// <param name="docNodePath">The document node path.</param>
    /// <param name="parentNode">The parent node of this proxy. This is also the start node for resolution of the path.</param>
    /// <exception cref="System.ArgumentNullException">
    /// docNodePath
    /// or
    /// parentNode
    /// </exception>
    protected RelDocNodeProxy(Main.RelativeDocumentPath docNodePath, Main.IDocumentNode parentNode)
    {
      if (null == docNodePath)
        throw new ArgumentNullException(nameof(docNodePath));
      if (null == parentNode)
        throw new ArgumentNullException(nameof(parentNode));

      _parent = parentNode;
      _docNodePath = docNodePath;
    }

    public RelDocNodeProxy(Main.IDocumentLeafNode docNode, Main.IDocumentNode parentNode)
    {
      if (null == docNode)
        throw new ArgumentNullException(nameof(docNode));
      if (null == parentNode)
        throw new ArgumentNullException(nameof(parentNode));
      var docNodeRoot = Main.AbsoluteDocumentPath.GetRootNode(docNode);
      var parentNodeRoot = Main.AbsoluteDocumentPath.GetRootNode(parentNode);
      if (!object.ReferenceEquals(docNodeRoot, parentNodeRoot))
        throw new ArgumentException(string.Format("parentNode (type: {0}) and docNode (type: {1}) have no common root. This suggests that one of the items is not rooted. Please report this error! The type of the parent node's root is {2}. The type of the docNode's root is {3}.", parentNode.GetType(), docNode.GetType(), parentNodeRoot.GetType(), docNodeRoot.GetType()));

      InternalSetDocNode(docNode, parentNode);

      if (!(_docNodePath != null))
        throw new InvalidProgramException(); // because we tested above that both nodes have a common root

      _parent = parentNode;
    }

    /// <summary>
    /// Creates a copy of a docnode proxy
    /// </summary>
    /// <param name="from"></param>
    /// <param name="copyPathOnly">If true, only the path is copied, and the document is then resolved using the provided <paramref name="parentNode"/> as start point of the path.
    /// If <c>false</c>, and the proxy to copy from has a valid document, that document is used also for this instance, and a new relative path from  <paramref name="parentNode"/> to the document is calculated.</param>
    /// <param name="parentNode">The parent node of this proxy.</param>
    public RelDocNodeProxy(RelDocNodeProxy from, bool copyPathOnly, Main.IDocumentNode parentNode)
    {
      _parent = parentNode;
      _docNodePath = from._docNodePath.Clone();

      if (!copyPathOnly && null != from.Document)
        InternalSetDocNode(from.Document, parentNode);

      ResolveDocumentObject();
    }

    #endregion Construction

    #region public properties

    /// <summary>
    /// Gets a copy of the document path. You can not change the document path of the proxy instance by this copy.
    /// </summary>
    /// <value>
    /// The document path.
    /// </value>
    public RelativeDocumentPath DocumentPath
    {
      get
      {
        return _docNodePath.Clone();
      }
    }

    /// <summary>
    /// Gets/sets the document node that is held by this proxy. If the stored doc node is null, it is tried to resolve the stored document path.
    /// If that fails too, null is returned.
    /// </summary>
    public IDocumentLeafNode Document
    {
      get
      {
        return ResolveDocumentObject();
      }
      set
      {
        if (!IsValidDocument(value))
          throw new ArgumentException("This type of document is not allowed for the proxy of type " + GetType().ToString());
        if (null == _parent)
          throw new InvalidOperationException("Parent of this node must be set in order to set the docnode.");
        InternalSetDocNode(value, _parent);
      }
    }

    #endregion public properties

    #region Internal functions

    /// <summary>
    /// Can be overriden by derived classes to ensure that the right type of document is stored in
    /// this proxy.
    /// </summary>
    /// <param name="obj">The object to test.</param>
    /// <returns>True if the <c>obj</c> has the right type to store in this proxy, false otherwise.</returns>
    protected virtual bool IsValidDocument(IDocumentLeafNode obj)
    {
      return true;
    }

    /// <summary>
    /// Is called after a document has been set, but before OnChanged() is called. Can be used to set up
    /// additional things, like event handlers, in derived classes.
    /// </summary>
    protected virtual void OnAfterSetDocNode()
    {
    }

    /// <summary>
    /// Is called before the doc node of this proxy is set to null. Can be used in derived classes
    /// to remove additional event handlers.
    /// </summary>
    protected virtual void OnBeforeClearDocNode()
    {
    }

    /// <summary>
    /// Sets the document node to null, but keeps the doc node path.
    /// </summary>
    protected void ClearDocNode()
    {
      if (_docNodeRef == null)
        return;

      OnBeforeClearDocNode();

      if (null != _weakDocNodeTunneledEventHandler)
      {
        _weakDocNodeTunneledEventHandler.Remove();
        _weakDocNodeTunneledEventHandler = null;
      }

      if (null != _weakDocNodeChangedHandler)
      {
        _weakDocNodeChangedHandler.Remove();
        _weakDocNodeChangedHandler = null;
      }

      _docNodeRef = null;
    }

    /// <summary>
    /// Removes the event handlers for the watch
    /// </summary>
    protected void ClearWatch()
    {
      if (null != _weakDocNodeTunneledEventHandler)
      {
        _weakDocNodeTunneledEventHandler.Remove();
        _weakDocNodeTunneledEventHandler = null;
      }

      if (null != _weakDocNodeChangedHandler)
      {
        _weakDocNodeChangedHandler.Remove();
        _weakDocNodeChangedHandler = null;
      }
    }

    /// <summary>
    /// Sets the document node that is held by this proxy.
    /// </summary>
    /// <param name="value">The document node. If <c>docNode</c> implements <see cref="Main.IDocumentLeafNode" />,
    /// the document path is stored for this object in addition to the object itself.</param>
    /// <param name="parentNode">The start point of the document path. Should be equal to the member _parent, but this might be not set now.</param>
    protected void InternalSetDocNode(Main.IDocumentLeafNode value, IDocumentLeafNode parentNode)
    {
      if (!IsValidDocument(value))
        throw new ArgumentException("This type of document is not allowed for the proxy of type " + GetType().ToString());
      if (null == parentNode)
        throw new InvalidOperationException("Parent of this node must be set in order to set the docnode.");

      var oldValue = InternalDocNode;
      if (object.ReferenceEquals(oldValue, value))
        return; // Nothing to do

      if (null != _weakDocNodeChangedHandler)
      {
        _weakDocNodeChangedHandler.Remove();
        _weakDocNodeChangedHandler = null;
      }
      if (null != _weakDocNodeTunneledEventHandler)
      {
        _weakDocNodeTunneledEventHandler.Remove();
        _weakDocNodeTunneledEventHandler = null;
      }

      if (null != oldValue)
      {
        ClearDocNode();
      }

      var newPath = RelativeDocumentPath.GetRelativePathFromTo(parentNode, value);
      if (null != newPath)
        InternalDocumentPath = newPath; // especially in dispose situations, the new path can be null. In this case we leave the path as it was

      _docNodeRef = new WeakReference(value);

#if DEBUG_DOCNODEPROXYLOGGING
			Current.Console.WriteLine("RelDocNodeProxy.SetDocNode, path is <<{0}>>", _docNodePath);
#endif

      value.TunneledEvent += (_weakDocNodeTunneledEventHandler = new WeakActionHandler<object, object, TunnelingEventArgs>(EhDocNode_TunneledEvent, handler => value.TunneledEvent -= handler));

      if (null != _docNodePath && !_docNodePath.IsIdentity) // it does not make sense to watch the changed event of our target node is our parent because the parent can handle the Changed event itself
      {
        value.Changed += (_weakDocNodeChangedHandler = new WeakEventHandler(EhDocNode_Changed, handler => value.Changed -= handler));
      }

      OnAfterSetDocNode();

      EhSelfChanged(new Main.InstanceChangedEventArgs(oldValue, value));
    }

    /// <summary>
    /// Gets the internal document node instance, without changing anything and without trying to resolve the path.
    /// </summary>
    /// <value>
    /// The internal document node.
    /// </value>
    protected IDocumentLeafNode InternalDocNode
    {
      get
      {
        return (_docNodeRef == null ? null : _docNodeRef.Target) as IDocumentLeafNode;
      }
    }

    /// <summary>
    /// Gets the internal document node instance, without changing anything and without trying to resolve the path.
    /// </summary>
    /// <value>
    /// The internal document node.
    /// </value>
    protected virtual IDocumentLeafNode InternalDocumentObject
    {
      get
      {
        return (_docNodeRef == null ? null : _docNodeRef.Target) as IDocumentLeafNode;
      }
    }

    protected RelativeDocumentPath InternalDocumentPath
    {
      get
      {
        return _docNodePath;
      }
      set
      {
        if (null == value)
          throw new ArgumentNullException(nameof(value));

        _docNodePath = value;
      }
    }

    protected virtual IDocumentLeafNode ResolveDocumentObject()
    {
      if (IsDisposeInProgress)
        return null;

      if (!(_docNodePath != null))
        throw new InvalidProgramException();

      var docNode = InternalDocNode;
      if (docNode == null)
      {
        var node = Main.RelativeDocumentPath.GetNodeOrLeastResolveableNode(_docNodePath, _parent, out var wasCompletelyResolved);
        if (null == node)
        {
          // this can happen only if we dived to deep with our relative path, in this case we should use the root node, which should be of type AltaxoDocument
          node = Altaxo.Main.AbsoluteDocumentPath.GetRootNode(_parent);
          wasCompletelyResolved = false;
        }

        if (wasCompletelyResolved)
        {
          InternalSetDocNode(node, _parent);
          docNode = InternalDocNode;
        }
        else // not completely resolved
        {
          SetWatchOnNode(node);
        }
      }
      return docNode;
    }

    #endregion Internal functions

    #region Handling of events from the proxied document node

    /// <summary>
    /// Event handler that is called when the document node has disposed or name changed. Because the path to the node can have changed too,
    /// the path is renewed in this case. The <see cref="OnChanged" /> method is called then for the proxy itself.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void EhDocNode_TunneledEvent(object sender, object source, Main.TunnelingEventArgs e)
    {
      if (IsDisposeInProgress)
        return;

#if DEBUG_DOCNODEPROXYLOGGING
			Current.Console.WriteLine("RelDocNodeProxy.EhDocNode_TunneledEvent: sender={0}, source={1} e={2}", sender, source, e);
#endif

      bool shouldFireChangedEvent = false;
      var senderAsNode = source as IDocumentLeafNode;
      if (!(senderAsNode != null))
        throw new InvalidProgramException();

      if (e is DisposeEventArgs)
      {
        // when our DocNode was disposed, it is probable that the parent of this node (and further parents) are disposed too
        // thus we need to watch the first node that is not disposed
        var docNode = InternalDocNode;
        ClearDocNode();

        if (!(sender is AltaxoDocument)) // if the whole document is disposed, there is no point in trying to watch something
        {
          // note Dispose is designed to let the hierarchy from child to parent (root) valid, but not from root to child!
          // thus trying to get an actual document path here is in must cases unsuccessfull. We have to rely on our stored path, and that it was always updated!
          // the only case were it is successfull if a new node immediately replaces an old document node

          var node = RelativeDocumentPath.GetNodeOrLeastResolveableNode(_docNodePath, senderAsNode, out var wasResolvedCompletely);
          if (wasResolvedCompletely)
          {
            InternalSetDocNode(node, _parent);
          }
          else
          {
            SetWatchOnNode(node);
          }

          shouldFireChangedEvent = true;
        }
      }
      else if (e is DocumentPathChangedEventArgs)
      {
        if (null != InternalDocNode)
        {
          InternalDocumentPath = RelativeDocumentPath.GetRelativePathFromTo(_parent, InternalDocNode);
        }

        shouldFireChangedEvent = true;
      }

      if (shouldFireChangedEvent)
        EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Event handler that is called when the document node has changed. Because the path to the node can have changed too,
    /// the path is renewed in this case. The <see cref="OnChanged" /> method is called then for the proxy itself.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EhDocNode_Changed(object sender, EventArgs e)
    {
      if (IsDisposeInProgress)
        return;

#if DEBUG_DOCNODEPROXYLOGGING
			Current.Console.WriteLine("DocNodeProxy.EhDocNode_Changed: sender={0}, e={1}", sender, e);
#endif

      if (null != InternalDocNode)
      {
        _docNodePath = RelativeDocumentPath.GetRelativePathFromTo(_parent, InternalDocNode);
      }

      EhSelfChanged(EventArgs.Empty);
    }

    #endregion Handling of events from the proxied document node

    #region WatchedNode setting and event handling

    /// <summary>
    /// Sets the watch on a node that is not our document node, but a node lower in the hierarchy. We watch both the Changed event and the TunneledEvent of this node.
    /// </summary>
    /// <param name="node">The node to watch.</param>
    protected virtual void SetWatchOnNode(IDocumentLeafNode node)
    {
      if (null == node)
        throw new ArgumentNullException(nameof(node));

      if (null != _weakDocNodeChangedHandler)
      {
        _weakDocNodeChangedHandler.Remove();
        _weakDocNodeChangedHandler = null;
      }
      if (null != _weakDocNodeTunneledEventHandler)
      {
        _weakDocNodeTunneledEventHandler.Remove();
        _weakDocNodeTunneledEventHandler = null;
      }

      node.TunneledEvent += (_weakDocNodeTunneledEventHandler = new WeakActionHandler<object, object, TunnelingEventArgs>(EhWatchedNode_TunneledEvent, handler => node.TunneledEvent -= handler));
      node.Changed += (_weakDocNodeChangedHandler = new WeakEventHandler(EhWatchedNode_Changed, handler => node.Changed -= handler));

#if DEBUG_DOCNODEPROXYLOGGING
			Current.Console.WriteLine("Start watching node <<{0}>> of total path <<{1}>>", AbsoluteDocumentPath.GetAbsolutePath(node), _docNodePath);
#endif
    }

    /// <summary>
    /// Event handler that is called when the watched node or a parent node below has disposed or its name changed. We then try to resolve the path again.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="source">Source of the tunneled event.</param>
    /// <param name="e"></param>
    private void EhWatchedNode_TunneledEvent(object sender, object source, Main.TunnelingEventArgs e)
    {
      if (IsDisposeInProgress)
        return;

      if (!(_docNodeRef == null))
        throw new InvalidProgramException();
      var senderAsDocNode = sender as IDocumentLeafNode;
      var sourceAsDocNode = source as IDocumentLeafNode;

      if (!(senderAsDocNode != null))
        throw new InvalidProgramException();
      if (!(sourceAsDocNode != null))
        throw new InvalidProgramException();

      // then we try to resolve the path again
      if ((e is DisposeEventArgs) || (e is DocumentPathChangedEventArgs))
      {
#if DEBUG_DOCNODEPROXYLOGGING
				Current.Console.WriteLine("DocNodeProxy.EhWatchedNode_TunneledEvent");
#endif

        var node = RelativeDocumentPath.GetNodeOrLeastResolveableNode(_docNodePath, sourceAsDocNode, out var wasResolvedCompletely);
        if (null == node)
          throw new InvalidProgramException(nameof(node) + " should always be != null, since we use absolute paths, and at least an AltaxoDocument should be resolved here.");

        if (wasResolvedCompletely)
        {
          ClearWatch();
          InternalSetDocNode(node, _parent);
        }
        else // not completely resolved
        {
          if (!object.ReferenceEquals(sender, node))
          {
            ClearWatch();
            SetWatchOnNode(node);
          }
        }
      }
    }

    /// <summary>
    /// Event handler that is called when the watched node (a node that is not the document node) has changed. Maybe this watched node had now created a parent node, and our
    /// document path can resolved now. That's why we try to resolve our document path now.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EhWatchedNode_Changed(object sender, EventArgs e)
    {
      if (IsDisposeInProgress)
        return;

#if DEBUG_DOCNODEPROXYLOGGING
			Current.Console.WriteLine("DocNodeProxy.EhWatchedNode_Changed: sender={0}, e={1}", sender, e);
#endif

      if (!(_docNodeRef == null))
        throw new InvalidProgramException();
      var senderAsDocNode = sender as IDocumentLeafNode;
      if (!(senderAsDocNode != null))
        throw new InvalidProgramException();


      var node = RelativeDocumentPath.GetNodeOrLeastResolveableNode(_docNodePath, senderAsDocNode, out var wasResolvedCompletely);
      if (null == node)
        throw new InvalidProgramException("node should always be != null, since we use absolute paths, and at least an AltaxoDocument should be resolved here.");

      if (wasResolvedCompletely)
      {
        ClearWatch();
        InternalSetDocNode(node, _parent);
      }
      else // not completely resolved
      {
        if (!object.ReferenceEquals(sender, node))
        {
          ClearWatch();
          SetWatchOnNode(node);
        }
      }
    }

    #endregion WatchedNode setting and event handling

    #region Disposing, event handling of our own instance

    public override IDocumentNode ParentObject
    {
      get
      {
        return base.ParentObject;
      }
      set
      {
        if (null != _parent && null != value && !object.ReferenceEquals(_parent, value))
          throw new InvalidOperationException("A RelDocNodeProxy's parent should never change.");

        base.ParentObject = value;
      }
    }

    /// <summary>
    /// Disposing this instance is special - we must not dispose the reference this instance holds.
    /// Instead, we remove all references to the holded document node and also all event handlers-
    /// </summary>
    /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool isDisposing)
    {
      if (null != _weakDocNodeChangedHandler)
      {
        _weakDocNodeChangedHandler.Remove();
        _weakDocNodeChangedHandler = null;
      }
      if (null != _weakDocNodeTunneledEventHandler)
      {
        _weakDocNodeTunneledEventHandler.Remove();
        _weakDocNodeTunneledEventHandler = null;
      }
      _docNodeRef = null;

#if DEBUG_DOCNODEPROXYLOGGING
			Current.Console.WriteLine("DocNodeProxy.Dispose, path was >>>{0}<<<", _docNodePath);
#endif

      _docNodePath = null;

      base.Dispose(isDisposing);
    }

    public override void EhParentTunnelingEventHappened(IDocumentNode sender, IDocumentNode originalSource, TunnelingEventArgs e)
    {
      // since we deal with relative path, we have to watch changes to our path too
      if (e is Main.DocumentPathChangedEventArgs)
      {
        var node = InternalDocNode;
        if (null != node)
        {
          _docNodePath = RelativeDocumentPath.GetRelativePathFromTo(_parent, node);
        }
      }

      base.EhParentTunnelingEventHappened(sender, originalSource, e);
    }

    /// <summary>
    /// Fired if the document node instance changed, is set to null, has changed its document path, or has changed its internal properties.
    /// If the document instance changed, this event is fired <b>after</b> the <see cref="DocumentInstanceChanged"/>  event.
    /// </summary>
    protected override void OnChanged(EventArgs e)
    {
      if (e is Main.InstanceChangedEventArgs && null != DocumentInstanceChanged)
      {
        DocumentInstanceChanged(this, (Main.InstanceChangedEventArgs)e); // fire this event if somebody has subscribed to it
      }

      base.OnChanged(e);
    }

    #endregion Disposing, event handling of our own instance
  }
}
