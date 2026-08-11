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

using System.Collections.Immutable;
using Altaxo.Serialization.Xml;

namespace Altaxo.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Represents a base implementation of a name splitter node, providing a dictionary of child nodes.
  /// </summary>
  public abstract record NameSplitterBase : IPropertyExtractionTreeNode
  {
    /// <inheritdoc/>
    public ImmutableList<(int IndexOfNamePart, IPropertyExtractionTreeNode Node)> Children { get; init; } = ImmutableList<(int, IPropertyExtractionTreeNode)>.Empty;

    /// <summary>
    /// Serializes the children of the given <see cref="NameSplitterBase"/> instance into the provided <see cref="IXmlSerializationInfo"/>.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="s"></param>
    protected static void SerializeChildren(Xml.IXmlSerializationInfo info, NameSplitterBase s)
    {
      info.CreateArray("Children", s.Children.Count);
      foreach (var tu in s.Children)
      {
        info.CreateElement("Child");
        {
          info.AddValue("IndexOfNamePart", tu.IndexOfNamePart);
          info.AddValue("Node", tu.Node);
        }
        info.CommitElement();
      }
      info.CommitArray();
    }

    /// <summary>
    /// Deserializes the children from the provided <see cref="IXmlDeserializationInfo"/>.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    protected static (int IndexOfNamePart, IPropertyExtractionTreeNode Node)[] DeserializeChildren(Xml.IXmlDeserializationInfo info)
    {
      var count = info.OpenArray("Children");
      var arr = new (int IndexOfNamePart, IPropertyExtractionTreeNode Node)[count];
      {
        for (int i = 0; i < count; i++)
        {
          info.OpenElement(); // "Child"
          {
            var indexOfNamePart = info.GetInt32("IndexOfNamePart");
            var node = (IPropertyExtractionTreeNode)info.GetValue("Node", null);
            arr[i] = (indexOfNamePart, node);
          }
          info.CloseElement();
        }
      }
      info.CloseArray(count);

      return arr;
    }
  }
}
