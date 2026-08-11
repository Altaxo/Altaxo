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
  /// Splits a file path into its constituent parts, including drive or server, the directory parts, file name, and extension.
  /// The extension is always the last element (at index -1), even if empty, and the file name is always the second to last element (at index -2).
  /// </summary>
  public record PathSplitter : NameSplitterBase, INameSplitter
  {
    #region Serialization

    /// <summary>
    /// V0: 2026-08-10 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PathSplitter), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PathSplitter)o;
        SerializeChildren(info, s);

      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var arr = DeserializeChildren(info);

        return o is null ? new PathSplitter { Children = arr.ToImmutableList() } : (PathSplitter)o with
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

      var directory = System.IO.Path.GetDirectoryName(name);
      var fileName = System.IO.Path.GetFileNameWithoutExtension(name);
      var extension = System.IO.Path.GetExtension(name) ?? string.Empty;


      result.Add(fileName ?? string.Empty);
      result.Add(extension);

      while (fileName is not null)
      {
        var newDirectory = System.IO.Path.GetDirectoryName(directory);
        if (string.IsNullOrEmpty(newDirectory) || newDirectory == directory)
        {
          break;
        }

        fileName = System.IO.Path.GetFileName(directory);
        if (fileName is not null)
        {
          result.Insert(0, fileName);
        }
        directory = newDirectory;
      }

      result.Insert(0, directory ?? string.Empty);

      return result;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"Split into path parts, file name and extension";
    }
  }
}
