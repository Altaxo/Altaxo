﻿#region Copyright

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

#nullable enable
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
    new INumericColumn? Document();
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
    public static INumericColumnProxy FromColumn(INumericColumn? column)
    {
      if (column is IDocumentLeafNode)
        return NumericColumnProxy.FromColumn(column);
      else
        return NumericColumnProxyForStandaloneColumns.FromColumn(column);
    }
  }

  public class NumericColumnProxyForStandaloneColumns : Main.SuspendableDocumentLeafNodeWithEventArgs, INumericColumnProxy
  {
    private INumericColumn? _column;

    public static NumericColumnProxyForStandaloneColumns FromColumn(INumericColumn? column)
    {
      if (column is IDocumentLeafNode colAsDocumentNode)
        throw new ArgumentException($"Column does implement {typeof(IDocumentLeafNode)}. The actual type of column is {column?.GetType()}");
      return new NumericColumnProxyForStandaloneColumns(column);
    }

    /// <summary>
    /// Constructor for deserialization purposes.
    /// </summary>
    /// <param name="info">Unused.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected NumericColumnProxyForStandaloneColumns(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {

    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    /// <summary>
    /// Constructor by giving a numeric column.
    /// </summary>
    /// <param name="column">The numeric column to hold.</param>
    protected NumericColumnProxyForStandaloneColumns(INumericColumn? column)
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
        info.AddValueOrNull("Column", s._column);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (NumericColumnProxyForStandaloneColumns?)o ?? new NumericColumnProxyForStandaloneColumns(info);
        var node = info.GetValueOrNull("Column", s);
        s._column = (INumericColumn?)node;
        return s;
      }
    }

    #endregion Serialization

    public INumericColumn? Document()
    {
      return _column;
    }

    IReadableColumn? IReadableColumnProxy.Document()
    {
      return _column;
    }

    string IReadableColumnProxy.GetName(int level)
    {
      return _column?.FullName ?? string.Empty;
    }

    public bool IsEmpty
    {
      get { return _column is null; }
    }

    public object Clone()
    {
      return FromColumn(_column);
    }

    public object? DocumentObject()
    {
      return _column;
    }

    public AbsoluteDocumentPath DocumentPath()
    {
      return AbsoluteDocumentPath.DocumentPathOfRootNode;
    }

    public bool ReplacePathParts(AbsoluteDocumentPath partToReplace, AbsoluteDocumentPath newPart, IDocumentLeafNode rootNode)
    {
      return false;
    }
  }

  /// <summary>
  /// Holds a "weak" reference to a numeric column, altogether with a document path to that column.
  /// </summary>
  public class NumericColumnProxy : DocNodeProxy2ndLevel, INumericColumnProxy
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

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (NumericColumnProxy?)o ?? new NumericColumnProxy(info);

#pragma warning disable CS0618 // Type or member is obsolete
        object? baseobj = info.GetBaseValueEmbeddedOrNull(s, "AltaxoBase,Altaxo.Main.DocNodeProxy,0", parent);         // deserialize the base class
#pragma warning restore CS0618 // Type or member is obsolete

        if (!object.ReferenceEquals(s, baseobj))
        {
          return NumericColumnProxyForStandaloneColumns.FromColumn((INumericColumn?)baseobj);
        }
        else
        {
          return s;
        }
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.NumericColumnProxy", 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, typeof(DocNodeProxy)); // serialize the base class
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (NumericColumnProxy?)o ?? new NumericColumnProxy(info);
        info.GetBaseValueEmbedded(s, typeof(DocNodeProxy), parent);         // deserialize the base class

        return s;
      }
    }

    /// <summary>
    /// 2019-09-12 Class now inherits from <see cref="DocNodeProxy2ndLevel"/> instead of <see cref="DocNodeProxy"/>.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericColumnProxy), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, typeof(DocNodeProxy2ndLevel)); // serialize the base class
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (NumericColumnProxy?)o ?? new NumericColumnProxy(info);
        info.GetBaseValueEmbedded(s, typeof(DocNodeProxy2ndLevel), parent);         // deserialize the base class

        return s;
      }
    }

    #endregion Serialization

    public static NumericColumnProxy FromColumn(INumericColumn column)
    {
      if (column is null)
        throw new ArgumentNullException("column");
      var colAsDocumentNode = column as IDocumentLeafNode;
      if (colAsDocumentNode is null)
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
      return ((obj is INumericColumn) && obj is IDocumentLeafNode) || obj is null;
    }

    /// <summary>
    /// Returns the holded object. Null can be returned if the object is no longer available (e.g. disposed).
    /// </summary>
    public INumericColumn? Document()
    {
      return (INumericColumn?)base.DocumentObject();
    }

    IReadableColumn? IReadableColumnProxy.Document()
    {
      return (INumericColumn?)base.DocumentObject();
    }

    string IReadableColumnProxy.GetName(int level)
    {
      return Document()?.FullName ?? string.Empty;
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
