#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main
{

  /// <summary>
  /// <see cref="DocNodeProxy2ndLevel"/> behaves similar to <see cref="DocNodeProxy"/>, except after deserialization.
  /// After deserialization, it does not try to resolve the document itself, but its parent. Only if the document itself
  /// is required, it then tries to resolve the document. After the document has been resolved, the behavior
  /// is identical to <see cref="DocNodeProxy"/>.
  /// </summary>
  /// <remarks>
  /// The behavior mentioned above should ensure that <see cref="Altaxo.Data.DataColumn"/>s held by a <see cref="Altaxo.Data.DataColumnCollection"/>
  /// are resolved by the proxy as late as possible. Only if the <see cref="Altaxo.Data.DataColumn"/> is required, it then must be loaded from the file system.
  /// </remarks>
  [Serializable]
  public class DocNodeProxy2ndLevel : DocNodeProxy
  {
    /// <summary>
    /// The name of the child.
    /// If set to null, the entire class behaves like a <see cref="DocNodeProxy"/>.
    /// If this is set (not null), which happens after deserialization, then the parent node is tracked instead of the proxy's document node.
    /// Only when explicitly calling <see cref="DocumentObject"/>, then it is tried to resolve the proxy's document node. 
    /// </summary>
    private string? _childName;

    #region Serialization

    /// <summary>
    /// 2019-09-12 Initial code
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DocNodeProxy2ndLevel), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DocNodeProxy2ndLevel)obj;

        var node = s.InternalDocumentNode;
        if (node is not null && !node.IsDisposeInProgress)
          s.InternalDocumentPath = Main.AbsoluteDocumentPath.GetAbsolutePath(node);

        if (s._docNodePath is null)
          throw new InvalidProgramException();

        info.AddValue("Path", (s._childName is null) ? s._docNodePath : s._docNodePath.Append(s._childName));
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DocNodeProxy2ndLevel?)o ?? new DocNodeProxy2ndLevel(info);

        var nodePath = (Main.AbsoluteDocumentPath)info.GetValue("Path", s);

        s._childName = nodePath.LastPart;
        s.InternalDocumentPath = nodePath.ParentPath;

        if (s._docNodePath is null)
          throw new InvalidProgramException();

        // create a callback to resolve the instance as early as possible
        info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(s.EhXmlDeserializationFinished);

        return s;
      }
    }

    #endregion Serialization

    public DocNodeProxy2ndLevel(IDocumentLeafNode docNode)
      : base(docNode, isCalledFromConstructor: true)
    {

    }

    /// <summary>
    /// For deserialization purposes only.
    /// </summary>
    /// <param name="info"></param>
    protected DocNodeProxy2ndLevel(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(info)
    {
    }

    protected DocNodeProxy2ndLevel(Main.AbsoluteDocumentPath docNodePath)
      : base(docNodePath.ParentPath)
    {
      _childName = docNodePath.LastPart;
    }

    /// <summary>
    /// Cloning constructor.
    /// </summary>
    /// <param name="from">Object to clone from.</param>
    public DocNodeProxy2ndLevel(DocNodeProxy2ndLevel from)
      : base(from)
    {
      _childName = from._childName;
    }

    public override object Clone()
    {
      return new DocNodeProxy2ndLevel(this);
    }

    /// <summary>
    /// Disposing this instance is special - we must not dispose the reference this instance holds.
    /// Instead, we remove all references to the holded document node and also all event handlers-
    /// </summary>
    /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool isDisposing)
    {
      _childName = null;
      base.Dispose(isDisposing);
    }

    /// <summary>
    /// Sets the document node that is held by this proxy.
    /// </summary>
    /// <param name="value">The document node. If <c>docNode</c> implements <see cref="Main.IDocumentLeafNode" />,
    /// the document path is stored for this object in addition to the object itself.</param>
    public override void SetDocNode(IDocumentLeafNode value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      _childName = null;
      base.SetDocNode(value);
    }

    protected override bool InternalIsValidDocument(object obj, bool isCalledFromConstructor)
    {
      if (_childName is null && IsValidDocument(obj))
        return true;
      else if (_childName is null && isCalledFromConstructor)
        return true;
      else
        return obj is IDocumentNode;
    }



    /// <summary>
    /// Called when the doc node has changed. Can be overwritten in derived classes to implement additional functionality.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="source">The source of this event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    /// <returns>True if the something has changed, thus the Changed event of the proxy (!) should be fired. Otherwise, false.</returns>
    protected override bool OnDocNode_TunnelingEvent(object sender, object source, Main.TunnelingEventArgs e)
    {
      bool shouldFireChangedEvent = false;
      if (e is NameChangedEventArgs ncea && _childName is not null) // if we currently track the parent node (childName is not null), then we watch NameChanged events
      {
        if (ncea.OldName == _childName)
        {
          _childName = ncea.NewName;
          shouldFireChangedEvent = true;
        }
      }
      return shouldFireChangedEvent;
    }


    /// <summary>
    /// Returns the document node. If the stored doc node is null, it is tried to resolve the stored document path.
    /// If that fails too, null is returned.
    /// </summary>
    public override object? DocumentObject()
    {
      var result = base.DocumentObject();

      if (_childName is not null && result is IDocumentNode parentNode)
      {
        var child = parentNode.GetChildObjectNamed(_childName);
        if (child is not null)
        {
          _childName = null;
          InternalSetDocNode(child, isCalledFromConstructor: false, doNotTriggerChangedEvent: true); // we are tracking the child now
          result = child;
        }
        else
        {
          result = null; // can not resolved yet
        }
      }
      return result;

    }

    public override Main.AbsoluteDocumentPath DocumentPath()
    {
      var docNode = InternalDocumentNode;
      if (docNode is not null)
      {
        InternalDocumentPath = Main.AbsoluteDocumentPath.GetAbsolutePath(docNode);
      }

      return (_childName is null) ? InternalDocumentPath : InternalDocumentPath.Append(_childName);
    }
  }
}
