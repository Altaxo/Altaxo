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

namespace Altaxo.Main
{
	/// <summary>
	/// Holds a reference to an object. If the object is part of the document, i.e. a document node (implements <see cref="IDocumentLeafNode" />), then only a weak reference is held to this node, and special
	/// measures are used to track the node by its path. The path to the node is stored, and if a new document node with that path exists, the reference to the object is restored.
	/// <see cref="IProxy"/> can also hold non-document objects. To those objects a strong reference is established. The property <see cref="DocumentPath"/> then returns an empty path, and <see cref="ReplaceParts"/> does nothing at all.
	/// </summary>
	public interface IProxy : ICloneable
	{
		/// <summary>
		/// True when both the document is null and there is also no stored path to the document.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Returns the document node. If the stored doc node is null, it is tried to resolve the stored document path.
		/// If that fails too, null is returned.
		/// </summary>
		object DocumentObject { get; }

		/// <summary>
		/// Gets the path to the document, if the <see cref="DocumentObject"/> is or was part of the document.
		/// For non-document objects, an empty path is returned.
		/// </summary>
		/// <value>
		/// The document path.
		/// </value>
		Main.DocumentPath DocumentPath { get; }

		/// <summary>
		/// This functionality is provided only for document nodes. For non-document nodes, this function does nothing.
		/// Replaces parts of the path of the document node by another part. If the replacement was successful, the original document node is cleared.
		/// See <see cref="M:DocumentPath.ReplacePathParts"/> for details of the part replacement.
		/// </summary>
		/// <param name="partToReplace">Part of the path that should be replaced. This part has to match the beginning of this part. The last item of the part
		/// is allowed to be given only partially.</param>
		/// <param name="newPart">The new part to replace that piece of the path, that match the <c>partToReplace</c>.</param>
		/// <returns>True if the path could be replaced. Returns false if the path does not fulfill the presumptions given above.</returns>
		/// <remarks>
		/// As stated above, the last item of the partToReplace can be given only partially. As an example, the path (here separated by space)
		/// <para>Tables Preexperiment1/WDaten Time</para>
		/// <para>should be replaced by </para>
		/// <para>Tables Preexperiment2\WDaten Time</para>
		/// <para>To make this replacement, the partToReplace should be given by</para>
		/// <para>Tables Preexperiment1/</para>
		/// <para>and the newPart should be given by</para>
		/// <para>Tables Preexperiment2\</para>
		/// <para>Note that Preexperiment1\ and Preexperiment2\ are only partially defined items of the path.</para>
		/// </remarks>
		bool ReplacePathParts(DocumentPath partToReplace, DocumentPath newPart);
	}

	/// <summary>
	/// DocNodeProxy holds a reference to an object. If the object is a document node (implements <see cref="IDocumentLeafNode" />), then special
	/// measures are used in the case the document node is disposed. In this case the path to the node is stored, and if a new document node with
	/// that path exists, the reference to the object is restored.
	/// </summary>
	[Serializable]
	public class DocNodeProxy
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		IProxy,
		System.ICloneable,
		System.Runtime.Serialization.ISerializable,
		System.Runtime.Serialization.IDeserializationCallback
	{
		[NonSerialized]
		protected WeakReference _docNodeRef;

		private Main.DocumentPath _docNodePath;

		[NonSerialized]
		protected WeakEventHandler _weakDocNodeChangedHandler;

		[NonSerialized]
		protected WeakActionHandler<object, object, TunnelingEventArgs> _weakDocNodeTunneledEventHandler;

		#region Serialization

		#region ISerializable Members (Clipboard Serialization)

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			var docNode = InternalDocNode;
			if (docNode != null)
				info.AddValue("Node", Main.DocumentPath.GetAbsolutePath((Main.IDocumentLeafNode)docNode));
			else if (_docNodePath != null)
				info.AddValue("Node", _docNodePath);
		}

		protected DocNodeProxy(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			object node = info.GetValue("Node", typeof(object));
			if (node is Main.DocumentPath)
				InternalDocumentPath = (Main.DocumentPath)node;
			else
				SetDocNode(node as IDocumentLeafNode);
		}

		public void OnDeserialization(object sender)
		{
		}

		#endregion ISerializable Members (Clipboard Serialization)

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Main.DocNodeProxy", 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version not supported");
				/*
				DocNodeProxy s = (DocNodeProxy)obj;

				if (null != s.InternalDocNode)
					info.AddValue("Node", Main.DocumentPath.GetAbsolutePath(s.InternalDocNode));
				else if (s._docNodePath != null)
					info.AddValue("Node", s._docNodePath);
				*/
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				object node = info.GetValue("Node", o);

				if (node is DocumentPath)
				{
					var s = (DocNodeProxy)o ?? new DocNodeProxy((DocumentPath)node);
					s.InternalDocumentPath = (DocumentPath)node;
					info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(s.EhXmlDeserializationFinished);
					return s;
				}
				else
				{
					return node;
				}
			}
		}

		/// <summary>
		/// 2014-12-26 Only references are supported now
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DocNodeProxy), 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DocNodeProxy s = (DocNodeProxy)obj;

				var node = s.InternalDocNode;

				if (null != node)
					s.InternalDocumentPath = Main.DocumentPath.GetAbsolutePath(node);

				System.Diagnostics.Debug.Assert(null != s._docNodePath);
				info.AddValue("Path", s._docNodePath);
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (DocNodeProxy)o ?? new DocNodeProxy(info);

				var nodePath = (Main.DocumentPath)info.GetValue("Path", s);

				s.InternalDocumentPath = nodePath;

				System.Diagnostics.Debug.Assert(null != s._docNodePath);

				// create a callback to resolve the instance as early as possible
				info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(s.EhXmlDeserializationFinished);

				return s;
			}
		}

		#endregion Serialization

		public DocNodeProxy(IDocumentLeafNode docNode)
		{
			if (null == docNode)
				throw new ArgumentNullException("docNode");

			SetDocNode(docNode);
		}

		/// <summary>
		/// For deserialization purposes only.
		/// </summary>
		/// <param name="info"></param>
		protected DocNodeProxy(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		protected DocNodeProxy(Main.DocumentPath docNodePath)
		{
			if (null == docNodePath)
				throw new ArgumentNullException("docNodePath");

			InternalDocumentPath = docNodePath;
			InternalCheckAbsolutePath();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="from">Object to clone from.</param>
		public DocNodeProxy(DocNodeProxy from)
		{
			if (null != from.InternalDocNode)
			{
				this.SetDocNode(from.InternalDocNode); // than the new Proxy refers to the same document node
			}
			else
			{
				this.InternalDocumentPath = from._docNodePath.Clone(); // if no current document available, clone only the path
				InternalCheckAbsolutePath();
			}
		}

		/// <summary>
		/// True when both the document and the stored document path are <c>null</c>.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				System.Diagnostics.Debug.Assert(_docNodePath != null);
				return false;
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
			return obj is IDocumentLeafNode;
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

		[System.Diagnostics.Conditional("Debug_CheckDocNodePath")]
		private void InternalCheckAbsolutePath()
		{
			var path = _docNodePath;
			if (!path.IsAbsolutePath)
				throw new InvalidProgramException(string.Format("Path is expected to be an absolute path. Path = {0}", path));

			if (path.Count == 0)
				throw new InvalidProgramException(string.Format("Path is expected to be non-empty. Path = {0}", path));

			var path0 = path[0];

			if (path0 != "Tables" && path0 != "Graphs" && path0 != "TableLayouts" && path0 != "FitFunctionScripts" && path0 != "FolderProperties")
				throw new InvalidProgramException(string.Format("Path is expected to start with Tables or Graphs. Path = {0}", path));
		}

		/// <summary>
		/// Sets the document node that is held by this proxy.
		/// </summary>
		/// <param name="value">The document node. If <c>docNode</c> implements <see cref="Main.IDocumentLeafNode" />,
		/// the document path is stored for this object in addition to the object itself.</param>
		public void SetDocNode(IDocumentLeafNode value)
		{
			if (null == value)
				throw new ArgumentNullException("value");

			var oldValue = InternalDocNode;
			if (object.ReferenceEquals(oldValue, value))
				return;

			if (!IsValidDocument(value))
				throw new ArgumentException("This type of document is not allowed for the proxy of type " + this.GetType().ToString());

			if (oldValue != null)
			{
				ClearDocNode();
			}

			InternalDocumentPath = Main.DocumentPath.GetAbsolutePath(value);
			_docNodeRef = new WeakReference(value);

			InternalCheckAbsolutePath();

			if (value is Main.IEventIndicatedDisposable)
			{
				((Main.IEventIndicatedDisposable)value).TunneledEvent += (_weakDocNodeTunneledEventHandler = new WeakActionHandler<object, object, TunnelingEventArgs>(EhDocNode_TunneledEvent, handler => ((Main.IEventIndicatedDisposable)value).TunneledEvent -= handler));
			}

			if (value is Main.IChangedEventSource)
			{
				((Main.IChangedEventSource)value).Changed += (_weakDocNodeChangedHandler = new WeakEventHandler(EhDocNode_Changed, handler => ((Main.IChangedEventSource)value).Changed -= handler));
			}

			OnAfterSetDocNode();

			EhSelfChanged(new Main.InstanceChangedEventArgs(oldValue, value));
		}

		/// <summary>
		/// Replaces parts of the part of the document node by another part. If the replacement was successful, the original document node is cleared.
		/// See <see cref="M:DocumentPath.ReplacePathParts"/> for details of the part replacement.
		/// </summary>
		/// <param name="partToReplace">Part of the path that should be replaced. This part has to match the beginning of this part. The last item of the part
		/// is allowed to be given only partially.</param>
		/// <param name="newPart">The new part to replace that piece of the path, that match the <c>partToReplace</c>.</param>
		/// <returns>True if the path could be replaced. Returns false if the path does not fulfill the presumptions given above.</returns>
		/// <remarks>
		/// As stated above, the last item of the partToReplace can be given only partially. As an example, the path (here separated by space)
		/// <para>Tables Preexperiment1/WDaten Time</para>
		/// <para>should be replaced by </para>
		/// <para>Tables Preexperiment2\WDaten Time</para>
		/// <para>To make this replacement, the partToReplace should be given by</para>
		/// <para>Tables Preexperiment1/</para>
		/// <para>and the newPart should be given by</para>
		/// <para>Tables Preexperiment2\</para>
		/// <para>Note that Preexperiment1\ and Preexperiment2\ are only partially defined items of the path.</para>
		/// </remarks>
		public bool ReplacePathParts(DocumentPath partToReplace, DocumentPath newPart)
		{
			System.Diagnostics.Debug.Assert(null != _docNodePath);

			var result = _docNodePath.ReplacePathParts(partToReplace, newPart);
			if (result)
				ClearDocNode();
			return result;
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
		/// Event handler that is called when the document node has disposed or name changed. Because the path to the node can have changed too,
		/// the path is renewed in this case. The <see cref="OnChanged" /> method is called then for the proxy itself.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="source">Source of the tunneled event.</param>
		/// <param name="e"></param>
		private void EhDocNode_TunneledEvent(object sender, object source, Main.TunnelingEventArgs e)
		{
			bool shouldFireChangedEvent = false;

			if (e is DisposeEventArgs)
			{
				if (object.ReferenceEquals(source, sender))
				{
					ClearDocNode();
					shouldFireChangedEvent = true;
				}
			}
			else if (e is DocumentPathChangedEventArgs)
			{
				if (null != InternalDocNode)
				{
					InternalDocumentPath = Main.DocumentPath.GetAbsolutePath((Main.IDocumentLeafNode)InternalDocNode);
					InternalCheckAbsolutePath();
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
			if (InternalDocNode != null)
			{
				InternalDocumentPath = Main.DocumentPath.GetAbsolutePath((Main.IDocumentLeafNode)InternalDocNode);
				InternalCheckAbsolutePath();
			}

			EhSelfChanged(EventArgs.Empty);
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

		protected DocumentPath InternalDocumentPath
		{
			get
			{
				return _docNodePath;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException("value");

				_docNodePath = value;
			}
		}

		/// <summary>
		/// Returns the document node. If the stored doc node is null, it is tried to resolve the stored document path.
		/// If that fails too, null is returned.
		/// </summary>
		public object DocumentObject
		{
			get
			{
				return ResolveDocumentObject(Current.Project);
			}
		}

		public Main.DocumentPath DocumentPath
		{
			get
			{
				var docNode = InternalDocNode;
				if (null != docNode)
				{
					InternalDocumentPath = Main.DocumentPath.GetAbsolutePath((Main.IDocumentLeafNode)docNode);
				}

				return InternalDocumentPath;
			}
		}

		protected virtual object ResolveDocumentObject(Main.IDocumentNode startnode)
		{
			System.Diagnostics.Debug.Assert(null != _docNodePath);

			var docNode = InternalDocNode;
			if (docNode == null)
			{
				object obj = Main.DocumentPath.GetObject(_docNodePath, startnode);
				if (null != obj)
					SetDocNode((IDocumentLeafNode)obj);
				docNode = InternalDocNode;
			}
			return docNode;
		}

		protected void EhXmlDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, Main.IDocumentNode documentRoot, bool isFinallyCall)
		{
			if (null != this.ResolveDocumentObject(documentRoot))
				info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhXmlDeserializationFinished);
		}

		#region ICloneable Members

		public virtual object Clone()
		{
			return new DocNodeProxy(this);
		}

		#endregion ICloneable Members
	}
}