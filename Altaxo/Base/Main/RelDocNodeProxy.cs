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
		private WeakReference _docNodeRef;
		private Main.DocumentPath _docNodePath;

		[NonSerialized]
		protected WeakEventHandler _weakDocNodeChangedHandler;

		[NonSerialized]
		protected WeakActionHandler<object, object, TunnelingEventArgs> _weakDocNodeTunneledEventHandler;

		/// <summary>
		/// Fired if the document instance changed, (from null to some value, from a value to null, or from a value to another value).
		/// </summary>
		public event EventHandler<Main.InstanceChangedEventArgs> DocumentInstanceChanged;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RelDocNodeProxy), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				RelDocNodeProxy s = (RelDocNodeProxy)obj;
				System.Diagnostics.Debug.Assert(s._parent != null);
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
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				RelDocNodeProxy s = null != o ? (RelDocNodeProxy)o : new RelDocNodeProxy();

				if (!(parent is Main.IDocumentNode))
					throw new ArgumentException("Parent should be a valid document node");

				s._parent = (Main.IDocumentNode)parent;
				s._docNodePath = (Main.DocumentPath)info.GetValue("Node", s);

				// create a callback to resolve the instance as early as possible
				if (s._docNodePath != null && s.InternalDocNode == null)
				{
					info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(s.EhXmlDeserializationFinished);
				}

				return s;
			}
		}

		#endregion Serialization

		public RelDocNodeProxy(Main.IDocumentLeafNode docNode, Main.IDocumentNode parentNode)
		{
			SetDocNode(docNode, parentNode);
		}

		private RelDocNodeProxy(Main.DocumentPath relDocNodePath)
		{
			_docNodePath = relDocNodePath;
		}

		/// <summary>
		/// Creates an empty DocNodeProxy (similar to null for objects)
		/// </summary>
		public RelDocNodeProxy()
		{
		}

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
		/// Copying constructor.
		/// </summary>
		/// <param name="from">Object to clone from.</param>
		/// <param name="newparent"></param>
		public void CopyFrom(RelDocNodeProxy from, Main.IDocumentNode newparent)
		{
			if (object.ReferenceEquals(this, from))
				return;

			this.SetDocNode(from.InternalDocNode, newparent); // than the new Proxy refers to the same document node
		}

		public void CopyPathOnlyFrom(RelDocNodeProxy from, Main.IDocumentNode newparent)
		{
			this.ClearDocNode();
			this._parent = newparent;
			this._docNodePath = from._docNodePath == null ? null : (Main.DocumentPath)from._docNodePath.Clone();
		}

		public RelDocNodeProxy ClonePathOnly(Main.IDocumentNode newparent)
		{
			RelDocNodeProxy result = new RelDocNodeProxy();
			result.CopyPathOnlyFrom(this, newparent);
			return result;
		}

		/// <summary>
		/// True when both the document and the stored document path are <c>null</c>.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return InternalDocNode == null && this._docNodePath == null;
			}
		}

		/// <summary>
		/// Gets a copy of the document path. You can not change the document path of the proxy instance by this copy.
		/// </summary>
		/// <value>
		/// The document path.
		/// </value>
		public DocumentPath DocumentPath
		{
			get
			{
				return _docNodePath == null ? null : (DocumentPath)_docNodePath.Clone();
			}
		}

		/// <summary>
		/// Can be overriden by derived classes to ensure that the right type of document is stored in
		/// this proxy.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		/// <returns>True if the <c>obj</c> has the right type to store in this proxy, false otherwise.</returns>
		protected virtual bool IsValidDocument(object obj)
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
		/// Sets the document node that is held by this proxy.
		/// </summary>
		/// <param name="docNode">The document node. If <c>docNode</c> implements <see cref="Main.IDocumentLeafNode" />,
		/// the document path is stored for this object in addition to the object itself.</param>
		/// <param name="parentNode"></param>
		public void SetDocNode(Main.IDocumentLeafNode docNode, Main.IDocumentNode parentNode)
		{
			if (!IsValidDocument(docNode))
				throw new ArgumentException("This type of document is not allowed for the proxy of type " + this.GetType().ToString());
			if (parentNode == null)
				throw new ArgumentNullException("parentNode");

			var oldDocNode = this.InternalDocNode;
			if (object.ReferenceEquals(oldDocNode, docNode) && object.ReferenceEquals(_parent, parentNode))
				return; // Nothing to do

			if (_docNodeRef != null)
			{
				ClearDocNode();
				this._docNodePath = null;
			}

			_docNodeRef = new WeakReference(docNode);
			_parent = parentNode;

			if (docNode != null)
				_docNodePath = Altaxo.Main.DocumentPath.GetRelativePathFromTo(parentNode, docNode);

			if (docNode is Main.IEventIndicatedDisposable)
			{
				((Main.IEventIndicatedDisposable)docNode).TunneledEvent += (_weakDocNodeTunneledEventHandler = new WeakActionHandler<object, object, TunnelingEventArgs>(EhDocNode_TunneledEvent, handler => ((Main.IEventIndicatedDisposable)docNode).TunneledEvent -= handler));
			}

			if (docNode != null)
			{
				((Main.IChangedEventSource)docNode).Changed += (_weakDocNodeChangedHandler = new WeakEventHandler(EhDocNode_Changed, handler => ((Main.IChangedEventSource)docNode).Changed -= handler));
			}

			OnAfterSetDocNode();

			EhSelfChanged(new Main.InstanceChangedEventArgs(oldDocNode, docNode));
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

			var oldvalue = InternalDocNode;
			_docNodeRef = null;

			EhSelfChanged(new Main.InstanceChangedEventArgs(oldvalue, null));
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
				return (IDocumentLeafNode)(_docNodeRef == null ? null : _docNodeRef.Target);
			}
		}

		/// <summary>
		/// Returns the document node. If the stored doc node is null, it is tried to resolve the stored document path.
		/// If that fails too, null is returned.
		/// </summary>
		public IDocumentLeafNode DocumentObject
		{
			get
			{
				if (InternalDocNode == null && _docNodePath != null)
				{
					if (_docNodePath.IsAbsolutePath)
						return null;

					Main.IDocumentLeafNode obj = (Main.IDocumentLeafNode)Main.DocumentPath.GetObject(_docNodePath, _parent);
					if (obj != null)
						SetDocNode(obj, _parent);
				}
				return InternalDocNode;
			}
		}

		private void EhXmlDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinallyCall)
		{
			if (this.DocumentObject != null)
				info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhXmlDeserializationFinished);
		}

		#region IChangedEventSource Members

		/// <summary>
		/// Event handler that is called when the document node has disposed or name changed. Because the path to the node can have changed too,
		/// the path is renewed in this case. The <see cref="OnChanged" /> method is called then for the proxy itself.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void EhDocNode_TunneledEvent(object sender, object source, Main.TunnelingEventArgs e)
		{
			if (e is DisposeEventArgs)
			{
				if (object.ReferenceEquals(source, sender))
				{
					ClearDocNode(); // Cleardocnode fires the event
					return;
				}
			}
			else if (e is DocumentPathChangedEventArgs)
			{
				var docNode = InternalDocNode;
				if (null != docNode)
					_docNodePath = Main.DocumentPath.GetRelativePathFromTo(_parent, docNode);
				else
					_docNodePath = null;

				EhSelfChanged(EventArgs.Empty);
				return;
			}
		}

		/// <summary>
		/// Event handler that is called when the document node has changed. Because the path to the node can have changed too,
		/// the path is renewed in this case. The <see cref="OnChanged" /> method is called then for the proxy itself.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EhDocNode_Changed(object sender, EventArgs e)
		{
			if (null != InternalDocNode)
				_docNodePath = Main.DocumentPath.GetRelativePathFromTo(_parent, InternalDocNode);
			else
				_docNodePath = null;

			EhSelfChanged(EventArgs.Empty);
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

		#endregion IChangedEventSource Members
	}
}