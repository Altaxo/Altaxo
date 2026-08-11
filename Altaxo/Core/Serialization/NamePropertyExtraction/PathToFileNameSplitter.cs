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

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Altaxo.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Splits a file path only into the file name without the extension. The file name is always the first and only element (at index 0), even if empty.
  /// </summary>
  public record PathToFileNameSplitter : NameSplitterBase, INameSplitter
  {
    #region Serialization

    /// <summary>
    /// V0: 2026-08-10 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PathToFileNameSplitter), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PathToFileNameSplitter)o;
        SerializeChildren(info, s);
      }



      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var arr = DeserializeChildren(info);

        return o is null ? new PathToFileNameSplitter { Children = arr.ToImmutableList() } : (PathToFileNameSplitter)o with
        {
          Children = arr.ToImmutableList()
        };
      }


    }

    #endregion


    /// <inheritdoc/>
    public IReadOnlyList<string> Split(string name)
    {
      var result = new List<string>();
      var fileName = System.IO.Path.GetFileNameWithoutExtension(name);
      result.Add(fileName ?? string.Empty);
      return result;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"Path to file name without extension";
    }
  }
}
