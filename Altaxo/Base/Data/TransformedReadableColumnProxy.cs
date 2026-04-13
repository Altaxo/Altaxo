#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
  /// Proxy for a transformed readable column whose underlying column is part of the document hierarchy.
  /// </summary>
  /// <seealso cref="Altaxo.Main.DocNodeProxy" />
  /// <seealso cref="Altaxo.Data.IReadableColumnProxy" />
  internal class TransformedReadableColumnProxy : DocNodeProxy2ndLevel, IReadableColumnProxy
  {
    private IVariantToVariantTransformation _transformation;

    #region Serialization

    /// <summary>
    /// 2016-06-24 intial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.TransformedReadableColumnProxy", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TransformedReadableColumnProxy)o;
        info.AddBaseValueEmbedded(o, typeof(DocNodeProxy)); // serialize the base class
        info.AddValue("Transformation", s._transformation);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (TransformedReadableColumnProxy?)o ?? new TransformedReadableColumnProxy(info);
        info.GetBaseValueEmbedded(s, typeof(DocNodeProxy), parent);         // deserialize the base class
        s._transformation = (IVariantToVariantTransformation)info.GetValue("Transformation", s);
        return s;
      }
    }

    /// <summary>
    /// 2019-09-23 Class now derives from <see cref="DocNodeProxy2ndLevel"/> instead of <see cref="DocNodeProxy"/>.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TransformedReadableColumnProxy), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TransformedReadableColumnProxy)o;
        info.AddBaseValueEmbedded(o, typeof(DocNodeProxy2ndLevel)); // serialize the base class
        info.AddValue("Transformation", s._transformation);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (TransformedReadableColumnProxy?)o ?? new TransformedReadableColumnProxy(info);
        info.GetBaseValueEmbedded(s, typeof(DocNodeProxy2ndLevel), parent);         // deserialize the base class
        s._transformation = (IVariantToVariantTransformation)info.GetValue("Transformation", s);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates a proxy for a transformed readable column whose underlying column belongs to the document hierarchy.
    /// </summary>
    /// <param name="column">The transformed readable column to proxy.</param>
    /// <returns>A proxy for the specified transformed readable column.</returns>
    public static TransformedReadableColumnProxy FromColumn(ITransformedReadableColumn column)
    {
      if (column is null)
        throw new ArgumentNullException(nameof(column));
      var colAsDocumentNode = column.UnderlyingReadableColumn as IDocumentLeafNode;
      if (colAsDocumentNode is null)
        throw new ArgumentException(string.Format("column does not implement {0}. The actual type of column is {1}", typeof(IDocumentLeafNode), column.GetType()));

      return new TransformedReadableColumnProxy(column);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransformedReadableColumnProxy"/> class.
    /// </summary>
    /// <param name="column">The transformed readable column to proxy.</param>
    protected TransformedReadableColumnProxy(ITransformedReadableColumn column)
      : base((IDocumentLeafNode)column.UnderlyingReadableColumn)
    {
      _transformation = column.Transformation;
    }

    /// <summary>
    /// For deserialization purposes only.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected TransformedReadableColumnProxy(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    /// <summary>
    /// Cloning constructor.
    /// </summary>
    /// <param name="from">Object to clone from.</param>
    public TransformedReadableColumnProxy(TransformedReadableColumnProxy from)
      : base(from)
    {
      _transformation = from._transformation; // transformation is immutable
    }

    /// <inheritdoc />
    protected override bool IsValidDocument(object obj)
    {
      return (obj is IReadableColumn) || obj is null;
    }

    /// <inheritdoc />
    public IReadableColumn? Document()
    {
      var originalColumn = (IReadableColumn?)base.DocumentObject();
      if (originalColumn is null)
        return null;
      else
        return new TransformedReadableColumn(originalColumn, _transformation);
    }

    /// <inheritdoc />
    public override object Clone()
    {
      return new TransformedReadableColumnProxy(this);
    }

    /// <inheritdoc />
    public string GetName(int level)
    {
      string trans = _transformation.RepresentationAsOperator ?? _transformation.RepresentationAsFunction;
      return trans + " " + ReadableColumnProxy.GetName(level, (IReadableColumn?)base.DocumentObject(), InternalDocumentPath);
    }
  }
}
