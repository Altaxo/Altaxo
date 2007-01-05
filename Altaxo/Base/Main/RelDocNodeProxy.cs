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
  public delegate void DocumentInstanceChangedEventHandler(object sender, object oldvalue, object newvalue);

  /// <summary>
  /// DocNodeProxy holds a reference to an object. If the object is a document node (implements <see cref="IDocumentNode" />), then special
  /// measures are used in the case the document node is disposed. In this case the relative path to the node (from a parent object) is stored, and if a new document node with
  /// that path exists, the reference to the object is restored.
  /// </summary>
  public class RelDocNodeProxy : Main.IChangedEventSource
  {
   Main.IDocumentNode   _docNode;
    Main.IDocumentNode _parentNode;
    Main.DocumentPath _docNodePath;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RelDocNodeProxy), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        RelDocNodeProxy s = (RelDocNodeProxy)obj;
        System.Diagnostics.Debug.Assert(s._parentNode != null);
        Main.DocumentPath path = Main.DocumentPath.GetRelativePathFromTo(s._parentNode, (Main.IDocumentNode)s._docNode);
        info.AddValue("Node", path);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        RelDocNodeProxy s = null != o ? (RelDocNodeProxy)o : new RelDocNodeProxy();

        if (!(parent is Main.IDocumentNode))
          throw new ArgumentException("Parent should be a valid document node");

        s._parentNode = (Main.IDocumentNode)parent;
        s._docNodePath= (Main.DocumentPath)info.GetValue("Node", s);

        // create a callback to resolve the instance as early as possible
        if (s._docNodePath != null && s._docNode == null)
        {
          info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(s.EhXmlDeserializationFinished);
        }


        return s;
      }
    }
    #endregion

    public RelDocNodeProxy(Main.IDocumentNode docNode, Main.IDocumentNode parentNode)
    {
      SetDocNode(docNode,parentNode);
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

    /// <summary>
    /// Copying constructor.
    /// </summary>
    /// <param name="from">Object to clone from.</param>
    /// <param name="newparent"></param>
    public void CopyFrom(RelDocNodeProxy from, Main.IDocumentNode newparent)
    {
        this.SetDocNode(from._docNode, newparent); // than the new Proxy refers to the same document node
    }

    public void CopyPathOnlyFrom(RelDocNodeProxy from, Main.IDocumentNode newparent)
    {
      this.ClearDocNode();
      this._parentNode = newparent;
      this._docNodePath = from._docNodePath==null ? null : (Main.DocumentPath)from._docNodePath.Clone();
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
    /// <param name="parentNode"></param>
    public void SetDocNode(Main.IDocumentNode docNode, Main.IDocumentNode parentNode)
    {
      if (!IsValidDocument(docNode))
        throw new ArgumentException("This type of document is not allowed for the proxy of type " + this.GetType().ToString());
      if (parentNode == null)
        throw new ArgumentNullException("parentNode");

      bool docNodeInstanceChanged = !object.ReferenceEquals(_docNode, docNode) || !object.ReferenceEquals(_parentNode, parentNode);
      if (!docNodeInstanceChanged)
        return;
      Main.IDocumentNode oldvalue = _docNode;

      if (_docNode != null)
      {
        ClearDocNode();
        this._docNodePath = null;
      }

      _docNode = docNode;
      _parentNode = parentNode;
      
      if (_docNode != null)
        _docNodePath = Main.DocumentPath.GetRelativePathFromTo(parentNode, _docNode);

      if (_docNode is Main.IEventIndicatedDisposable)
        ((Main.IEventIndicatedDisposable)_docNode).Disposed += new EventHandler(EhDocNode_Disposed);

      if (_docNode is Main.IChangedEventSource)
        ((Main.IChangedEventSource)_docNode).Changed += new EventHandler(EhDocNode_Changed);

      OnAfterSetDocNode();

      this.OnDocumentInstanceChanged(oldvalue,_docNode);

      OnChanged();
    }

    /// <summary>
    /// Sets the document node to null, but keeps the doc node path.
    /// </summary>
    protected void ClearDocNode()
    {
      if (_docNode == null)
        return;

      OnBeforeClearDocNode();

      if (_docNode is Main.IEventIndicatedDisposable)
        ((Main.IEventIndicatedDisposable)_docNode).Disposed -= new EventHandler(EhDocNode_Disposed);

      if (_docNode is Main.IChangedEventSource)
        ((Main.IChangedEventSource)_docNode).Changed -= new EventHandler(EhDocNode_Changed);

      Main.IDocumentNode oldvalue = _docNode;
      _docNode = null;

      OnDocumentInstanceChanged(oldvalue,_docNode);
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
        _docNodePath = Main.DocumentPath.GetRelativePathFromTo(_parentNode,(Main.IDocumentNode)_docNode);
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
        if (_docNode == null && _docNodePath != null)
        {
          if (_docNodePath.IsAbsolutePath)
            return null;

          Main.IDocumentNode obj = (Main.IDocumentNode)Main.DocumentPath.GetObject(_docNodePath, _parentNode);
          if (obj != null)
            SetDocNode(obj,_parentNode);
        }
        return _docNode;

      }
    }

    private void EhXmlDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
    {
      if (this.DocumentObject != null)
        info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhXmlDeserializationFinished);
    }

    public event DocumentInstanceChangedEventHandler DocumentInstanceChanged;
    protected virtual void OnDocumentInstanceChanged(Main.IDocumentNode oldvalue, Main.IDocumentNode newvalue)
    {
      if (null != DocumentInstanceChanged)
      {
        DocumentInstanceChanged(this, oldvalue, newvalue);
      }

    }

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
