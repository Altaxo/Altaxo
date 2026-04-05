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

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Data;

namespace Altaxo.Graph.Gdi.LabelFormatting
{
  /// <summary>
  /// Formatting with the help of a .NET framework formatting string.
  /// </summary>
  public class FreeLabelFormatting : MultiLineLabelFormattingBase
  {
    private string _formatString = "{0}";

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FreeLabelFormatting), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FreeLabelFormatting)obj;
        info.AddBaseValueEmbedded(s, typeof(MultiLineLabelFormattingBase));
        info.AddValue("FormatString", s._formatString);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (FreeLabelFormatting?)o ?? new FreeLabelFormatting();
        info.GetBaseValueEmbedded(s, typeof(MultiLineLabelFormattingBase), parent);
        s._formatString = info.GetString("FormatString");
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="FreeLabelFormatting"/> class.
    /// </summary>
    public FreeLabelFormatting()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FreeLabelFormatting"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public FreeLabelFormatting(FreeLabelFormatting from)
      : base(from) // everything is done here, since CopyFrom is virtual
    {
    }

    /// <inheritdoc />
    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      var isCopied = base.CopyFrom(obj);
      if (isCopied && !object.ReferenceEquals(this, obj))
      {
        var from = obj as FreeLabelFormatting;
        if (from is not null)
        {
          _formatString = from._formatString;
        }
      }
      return isCopied;
    }

    /// <inheritdoc />
    public override object Clone()
    {
      return new FreeLabelFormatting(this);
    }

    /// <inheritdoc />
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    /// <summary>
    /// Gets or sets the .NET format string.
    /// </summary>
    public string FormatString
    {
      get
      {
        return _formatString;
      }
      set
      {
        _formatString = value;
      }
    }

    /// <inheritdoc />
    protected override string FormatItem(AltaxoVariant item)
    {
      if (!string.IsNullOrEmpty(_formatString))
      {
        try
        {
          return string.Format(_formatString, item.ToObject());
        }
        catch (Exception)
        {
        }
      }
      return item.ToString();
    }
  }
}
