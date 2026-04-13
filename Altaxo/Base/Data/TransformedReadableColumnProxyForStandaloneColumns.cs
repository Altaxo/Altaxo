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
  /// Proxy for a transformed readable column whose underlying column is not part of the document hierarchy.
  /// </summary>
  internal class TransformedReadableColumnProxyForStandaloneColumns : Main.SuspendableDocumentLeafNodeWithEventArgs, IReadableColumnProxy
  {
    private IReadableColumn? _underlyingColumn;
    private IVariantToVariantTransformation _transformation;
    private IReadableColumn? _cachedResultingColumn;

    /// <summary>
    /// Creates a proxy for a transformed readable column whose underlying column does not belong to the document hierarchy.
    /// </summary>
    /// <param name="column">The transformed readable column to proxy.</param>
    /// <returns>A proxy for the specified transformed readable column.</returns>
    public static TransformedReadableColumnProxyForStandaloneColumns FromColumn(ITransformedReadableColumn column)
    {
      if (column is null)
        throw new ArgumentNullException(nameof(column));

      var colAsDocumentNode = column.UnderlyingReadableColumn as IDocumentLeafNode;
      if (colAsDocumentNode is not null)
        throw new ArgumentException(string.Format("Column does implement {0}. The actual type of column is {1}", typeof(IDocumentLeafNode), column.GetType()));

      return new TransformedReadableColumnProxyForStandaloneColumns(column);
      ;
    }

    /// <summary>
    /// Constructor by giving a numeric column.
    /// </summary>
    /// <param name="column">The numeric column to hold.</param>
    protected TransformedReadableColumnProxyForStandaloneColumns(ITransformedReadableColumn column)
    {
      _underlyingColumn = column.UnderlyingReadableColumn;
      _transformation = column.Transformation;
      _cachedResultingColumn = column;
    }

    /// <summary>
    /// Constructor for deserialization purposes.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected TransformedReadableColumnProxyForStandaloneColumns(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    #region Serialization

    /// <summary>
    /// 2016-06-24 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TransformedReadableColumnProxyForStandaloneColumns), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TransformedReadableColumnProxyForStandaloneColumns)o;
        info.AddValueOrNull("Column", s._underlyingColumn);
        info.AddValue("Transformation", s._transformation);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (TransformedReadableColumnProxyForStandaloneColumns?)o ?? new TransformedReadableColumnProxyForStandaloneColumns(info);
        s._underlyingColumn = (IReadableColumn?)info.GetValueOrNull("Column", s);
        s._transformation = (IVariantToVariantTransformation)info.GetValue("Transformation", s);
        if (s._underlyingColumn is not null)
          s._cachedResultingColumn = new TransformedReadableColumn(s._underlyingColumn, s._transformation);

        return s;
      }
    }

    #endregion Serialization

    /// <inheritdoc />
    public IReadableColumn? Document()
    {
      return _cachedResultingColumn;
    }

    /// <inheritdoc />
    public object Clone()
    {
      return (TransformedReadableColumnProxyForStandaloneColumns)MemberwiseClone();
    }

    /// <inheritdoc />
    public bool IsEmpty
    {
      get { return _underlyingColumn is null; }
    }

    /// <inheritdoc />
    public string GetName(int level)
    {
      if (_underlyingColumn is null)
        return string.Empty;
      else
        return (_transformation.RepresentationAsOperator ?? _transformation.RepresentationAsFunction) + " " + _underlyingColumn.FullName;
    }

    /// <inheritdoc />
    public object? DocumentObject()
    {
      return _cachedResultingColumn;
    }

    /// <inheritdoc />
    public AbsoluteDocumentPath DocumentPath()
    {
      return AbsoluteDocumentPath.DocumentPathOfRootNode;
    }

    /// <inheritdoc />
    public bool ReplacePathParts(AbsoluteDocumentPath partToReplace, AbsoluteDocumentPath newPart, IDocumentLeafNode rootNode)
    {
      return false;
    }
  }
}
