﻿#region Copyright

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
  internal class ReadableColumnProxyForStandaloneColumns : Main.SuspendableDocumentLeafNodeWithEventArgs, IReadableColumnProxy
  {
    private IReadableColumn? _column;

    public static ReadableColumnProxyForStandaloneColumns FromColumn(IReadableColumn? column)
    {
      if (column is IDocumentLeafNode)
        throw new ArgumentException($"column does implement {typeof(IDocumentLeafNode)}. The actual type of column is {column?.GetType()}");
      return new ReadableColumnProxyForStandaloneColumns(column);
    }

    /// <summary>
    /// Constructor by giving a numeric column.
    /// </summary>
    /// <param name="column">The numeric column to hold.</param>
    protected ReadableColumnProxyForStandaloneColumns(IReadableColumn? column)
    {
      _column = column;
    }

    #region Serialization

    /// <summary>
    /// 2014-12-26 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ReadableColumnProxyForStandaloneColumns), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ReadableColumnProxyForStandaloneColumns)obj;
        info.AddValueOrNull("Column", s._column);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ReadableColumnProxyForStandaloneColumns?)o ?? new ReadableColumnProxyForStandaloneColumns(null);
        var node = info.GetValueOrNull("Column", s);
        s._column = (IReadableColumn?)node;
        return s;
      }
    }

    #endregion Serialization

    public IReadableColumn? Document()
    {
      return _column;
    }

    public object Clone()
    {
      return FromColumn(_column);
    }

    public bool IsEmpty
    {
      get { return _column is null; }
    }

    public string GetName(int level)
    {
      return _column?.ToString() ?? string.Empty;
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
}
