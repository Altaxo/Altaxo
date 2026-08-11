#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Altaxo.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Splits a name into parts using the specified separator strings. The resulting list of name parts will be returned, with empty entries removed if specified.
  /// </summary>
  public record NameSplitterBySeparatorString : NameSplitterBase, INameSplitter
  {
    /// <summary>
    /// Gets or sets the separator string used to split the name into parts.
    /// </summary>
    public string Separator { get; init; } = "_";

    /// <summary>
    /// Gets or sets a value indicating whether to remove empty entries when splitting the name. If set to true, empty entries will be removed from the resulting list of name parts.
    /// </summary>
    public bool RemoveEmptyEntries { get; init; } = false;


    #region Serialization

    /// <summary>
    /// V0: 2026-08-11 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NameSplitterBySeparatorString), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NameSplitterBySeparatorString)o;
        info.AddValue("Separator", s.Separator);
        info.AddValue("RemoveEmptyEntries", s.RemoveEmptyEntries);
        SerializeChildren(info, s);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var separator = info.GetString("Separator");
        var removeEmptyEntries = info.GetBoolean("RemoveEmptyEntries");
        var arr = DeserializeChildren(info);

        return o is null ? new NameSplitterBySeparatorString
        {
          Separator = separator,
          RemoveEmptyEntries = removeEmptyEntries,
          Children = arr.ToImmutableList()
        } : ((NameSplitterBySeparatorString)o) with
        {
          Separator = separator,
          RemoveEmptyEntries = removeEmptyEntries,
          Children = arr.ToImmutableList()
        };
      }
    }
    #endregion

    /// <inheritdoc/>
    public IReadOnlyList<string> Split(string name)
    {
      var parts = name.Split(Separator, RemoveEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
      return parts;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"Split by separator: \"{Separator}\"";
    }
  }
}
