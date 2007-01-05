#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;

namespace Altaxo.Main
{
  /// <summary>
  /// DocNodeProxy holds a reference to an object. If the object is a document node (implements <see cref="IDocumentNode" />), then special
  /// measures are used in the case the document node is disposed. In this case the path to the node is stored, and if a new document node with
  /// that path exists, the reference to the object is restored.
  /// </summary>
  public class DocNodeProxy : System.ICloneable, Main.IChangedEventSource
  {
    protected object _docNode;
    protected Main.DocumentPath _docNodePath;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DocNodeProxy),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        DocNodeProxy s = (DocNodeProxy)obj;
        
        if(s._docNode is Main.IDocumentNode)
          info.AddValue("Node",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s._docNode));
        else if(s._docNodePath != null)
          info.AddValue("Node",s._docNodePath);
        else
          info.AddValue("Node",s._docNode);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DocNodeProxy s = null!=o ?(DocNodeProxy)o : new DocNodeProxy();

        object node = info.GetValue("Node",typeof(object));

        if(node is Main.DocumentPath)
          s._docNodePath = (Main.DocumentPath)node;
        else
          s.SetDocNode(node);

        // create a callback to resolve the instance as early as possible
        if (s._docNodePath != null && s._docNode == null)
        {
          info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(s.EhXmlDeserializationFinished);
        }


        return s;
      }
    }
    #endregion

    public DocNodeProxy(object docNode)
    {
      SetDocNode(docNode);
    }

    private DocNodeProxy(Main.DocumentPath docNodePath)
    {
      _docNodePath = docNodePath;
    }

    /// <summary>
    /// Creates an empty DocNodeProxy (similar to null for objects)
    /// </summary>
    public DocNodeProxy()
    {
    }

    /// <summary>
    /// Cloning constructor.
    /// </summary>
    /// <param name="from">Object to clone from.</param>
    public DocNodeProxy(DocNodeProxy from)
    {
      if (from._docNode is Main.IDocumentNode)
        this.SetDocNode(from._docNode); // than the new Proxy refers to the same document node
      else if (from._docNode is ICloneable)
        this.SetDocNode(((System.ICloneable)from._docNode).Clone()); // clone the underlying object
      else if (from._docNode != null)
        this.SetDocNode(from._docNode); // the underlying object is not cloneable, so refer directly to it
      else if(from._docNodePath != null)
        this._docNodePath= from._docNodePath.Clone(); // if no current document available, clone only the path
    }

    /// <summary>
    /// True when both the document and the stored document path are <c>null</c>.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return this._docNode == null && this._docNodePath == null;
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
    /// <param name="docNode">The document node. If <c>docNode</c> implements <see cref="Main.IDocumentNode" />,
    /// the document path is stored for this object in addition to the object itself.</param>
    public void SetDocNode(object docNode)
    {
      if(!IsValidDocument(docNode))
        throw new ArgumentException("This type of document is not allowed for the proxy of type " + this.GetType().ToString());

      if (_docNode != null)
      {
        ClearDocNode();
        this._docNodePath = null;
      }

      _docNode = docNode;

      if(_docNode is Main.IDocumentNode)
        _docNodePath = Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)_docNode);
      else
        _docNodePath = null;

      if(_docNode is Main.IEventIndicatedDisposable)
        ((Main.IEventIndicatedDisposable)_docNode).Disposed += new EventHandler(EhDocNode_Disposed);

      if (_docNode is Main.IChangedEventSource)
        ((Main.IChangedEventSource)_docNode).Changed += new EventHandler(EhDocNode_Changed);

      OnAfterSetDocNode();

      OnChanged();
    }

    /// <summary>
    /// Sets the document node to null, but keeps the doc node path.
    /// </summary>
    protected void ClearDocNode()
    {
      if(_docNode == null)
        return;

      OnBeforeClearDocNode();

      if(_docNode is Main.IEventIndicatedDisposable)
        ((Main.IEventIndicatedDisposable)_docNode).Disposed -= new EventHandler(EhDocNode_Disposed);

      if (_docNode is Main.IChangedEventSource)
        ((Main.IChangedEventSource)_docNode).Changed -= new EventHandler(EhDocNode_Changed);

      _docNode = null;
    }

    /// <summary>
    /// Event handler that is called when the document is disposed. In this case the doc node is set to null, but the path is kept.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void EhDocNode_Disposed(object sender, EventArgs e)
    {
      ClearDocNode();

      OnChanged();
    }

    /// <summary>
    /// Event handler that is called when the document node has changed. Because the path to the node can have changed too,
    /// the path is renewed in this case. The <see cref="OnChanged" /> method is called then for the proxy itself.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void EhDocNode_Changed(object sender, EventArgs e)
    {
      if (_docNode is Main.IDocumentNode)
        _docNodePath = Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)_docNode);
      else
        _docNodePath = null;

      OnChanged();
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

    protected virtual object ResolveDocumentObject(object startnode)
    {
      if (_docNode == null && _docNodePath != null)
      {
        object obj = Main.DocumentPath.GetObject(_docNodePath, startnode);
        if (obj != null)
          SetDocNode(obj);
      }
      return _docNode; 
    }

    protected void EhXmlDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
    {
      if (null!=this.ResolveDocumentObject(documentRoot))
        info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhXmlDeserializationFinished);
    }

    #region ICloneable Members

    public virtual object Clone()
    {
      return new DocNodeProxy(this);
    }

    #endregion

    #region IChangedEventSource Members

    public event EventHandler Changed;

    protected virtual void OnChanged()
    {
      if (null != Changed)
        Changed(this, EventArgs.Empty);
    }
    #endregion
  }

}
