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
using Altaxo.Main;

namespace Altaxo.Data
{
  /// <summary>
  /// Proxy that holds instances of type <see cref="INumericColumnProxy"/>.
  /// </summary>
  public interface INumericColumnProxy : IReadableColumnProxy
  {
    /// <summary>
    /// Returns the holded object. Null can be returned if the object is no longer available (e.g. disposed).
    /// </summary>
    new INumericColumn Document { get; }
  }

  /// <summary>
  /// Static class to create instances of <see cref="INumericColumnProxy"/>.
  /// </summary>
  public static class NumericColumnProxyBase
  {
    /// <summary>
    /// Creates an <see cref="INumericColumnProxy"/> from a given column.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <returns>An instance of <see cref="INumericColumnProxy"/>. The type of instance returned depends on the type of the provided column (e.g. whether the column is part of the document or not).</returns>
    public static INumericColumnProxy FromColumn(INumericColumn column)
    {
      if (column is IDocumentLeafNode)
        return NumericColumnProxy.FromColumn(column);
      else
        return NumericColumnProxyForStandaloneColumns.FromColumn(column);
    }
  }

  public class NumericColumnProxyForStandaloneColumns : Main.SuspendableDocumentLeafNodeWithEventArgs, INumericColumnProxy
  {
    private INumericColumn _column;

    public static NumericColumnProxyForStandaloneColumns FromColumn(INumericColumn column)
    {
      var colAsDocumentNode = column as IDocumentLeafNode;
      if (null != colAsDocumentNode)
        throw new ArgumentException(string.Format("column does implement {0}. The actual type of column is {1}", typeof(IDocumentLeafNode), column.GetType()));

      return new NumericColumnProxyForStandaloneColumns(column);
      ;
    }

    /// <summary>
    /// Constructor by giving a numeric column.
    /// </summary>
    /// <param name="column">The numeric column to hold.</param>
    protected NumericColumnProxyForStandaloneColumns(INumericColumn column)
    {
      _column = column;
    }

    #region Serialization

    /// <summary>
    /// 2014-12-26 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericColumnProxyForStandaloneColumns), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NumericColumnProxyForStandaloneColumns)obj;
        info.AddValue("Column", s._column);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (NumericColumnProxyForStandaloneColumns)o ?? new NumericColumnProxyForStandaloneColumns(null);
        object node = info.GetValue("Column", s);
        s._column = (INumericColumn)node;
        return s;
      }
    }

    #endregion Serialization

    public INumericColumn Document
    {
      get { return _column; }
    }

    IReadableColumn IReadableColumnProxy.Document
    {
      get
      {
        return _column;
      }
    }

    string IReadableColumnProxy.GetName(int level)
    {
      return _column?.FullName;
    }

    public bool IsEmpty
    {
      get { return null == _column; }
    }

    public object Clone()
    {
      return FromColumn(_column);
    }

    public object DocumentObject
    {
      get { return _column; }
    }

    public AbsoluteDocumentPath DocumentPath
    {
      get { return AbsoluteDocumentPath.DocumentPathOfRootNode; }
    }

    public bool ReplacePathParts(AbsoluteDocumentPath partToReplace, AbsoluteDocumentPath newPart, IDocumentLeafNode rootNode)
    {
      return false;
    }
  }

  /// <summary>
  /// Holds a "weak" reference to a numeric column, altogether with a document path to that column.
  /// </summary>
  public class NumericColumnProxy : DocNodeProxy, INumericColumnProxy
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.NumericColumnProxy", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version not supported");
        //info.AddBaseValueEmbedded(obj, typeof(DocNodeProxy)); // serialize the base class
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (NumericColumnProxy)o ?? new NumericColumnProxy(info);

        object baseobj = info.GetBaseValueEmbedded(s, "AltaxoBase,Altaxo.Main.DocNodeProxy,0", parent);         // deserialize the base class

        if (!object.ReferenceEquals(s, baseobj))
        {
          return NumericColumnProxyForStandaloneColumns.FromColumn((INumericColumn)baseobj);
        }
        else
        {
          return s;
        }
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericColumnProxy), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, typeof(DocNodeProxy)); // serialize the base class
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (NumericColumnProxy)o ?? new NumericColumnProxy(info);
        info.GetBaseValueEmbedded(s, typeof(DocNodeProxy), parent);         // deserialize the base class

        return s;
      }
    }

    #endregion Serialization

    public static NumericColumnProxy FromColumn(INumericColumn column)
    {
      if (null == column)
        throw new ArgumentNullException("column");
      var colAsDocumentNode = column as IDocumentLeafNode;
      if (null == colAsDocumentNode)
        throw new ArgumentException(string.Format("column does not implement {0}. The actual type of column is {1}", typeof(IDocumentLeafNode), column.GetType()));

      return new NumericColumnProxy(colAsDocumentNode);
    }

    /// <summary>
    /// Constructor by giving a numeric column.
    /// </summary>
    /// <param name="column">The numeric column to hold.</param>
    protected NumericColumnProxy(IDocumentLeafNode column)
      : base(column)
    {
    }

    /// <summary>
    /// For deserialization purposes only.
    /// </summary>
    protected NumericColumnProxy(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(info)
    {
    }

    /// <summary>
    /// Cloning constructor.
    /// </summary>
    /// <param name="from">Object to clone from.</param>
    public NumericColumnProxy(NumericColumnProxy from)
      : base(from)
    {
    }

    /// <summary>
    /// Tests whether or not the holded object is valid. Here the test returns true if the column
    /// is either of type <see cref="INumericColumn" /> or the holded object is <c>null</c>.
    /// </summary>
    /// <param name="obj">Object to test.</param>
    /// <returns>True if this is a valid document object.</returns>
    protected override bool IsValidDocument(object obj)
    {
      return ((obj is INumericColumn) && obj is IDocumentLeafNode) || obj == null;
    }

    /// <summary>
    /// Returns the holded object. Null can be returned if the object is no longer available (e.g. disposed).
    /// </summary>
    public INumericColumn Document
    {
      get
      {
        return (INumericColumn)base.DocumentObject;
      }
    }

    IReadableColumn IReadableColumnProxy.Document
    {
      get
      {
        return (INumericColumn)base.DocumentObject;
      }
    }

    string IReadableColumnProxy.GetName(int level)
    {
      return Document?.FullName;
    }

    /// <summary>
    /// Clones this holder. For holded objects, which are part of the document hierarchy,
    /// the holded object is <b>not</b> cloned (only the reference is copied). For all other objects, the object
    /// is cloned, too, if the object supports the <see cref="ICloneable" /> interface.
    /// </summary>
    /// <returns>The cloned object holder.</returns>
    public override object Clone()
    {
      return new NumericColumnProxy(this);
    }
  }
}
