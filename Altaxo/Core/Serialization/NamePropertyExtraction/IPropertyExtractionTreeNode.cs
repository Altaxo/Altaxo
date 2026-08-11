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
  /// Defines a contract for a node in the name splitter tree, which can either be a name splitter or a property evaluator.
  /// </summary>
  public interface IPropertyExtractionTreeNode
  {
    /// <summary>
    /// Gets a list of child nodes, where each tuple contains the index of the name part and the corresponding child node.
    /// </summary>
    ImmutableList<(int IndexOfNamePart, IPropertyExtractionTreeNode Node)> Children { get; }

    /// <summary>
    /// Extracts properties from the given text using the configured name splitter and property evaluators.
    /// </summary>
    /// <param name="text">The text to extract properties from.</param>
    /// <returns>An enumerable of property name and value tuples.</returns>
    public IEnumerable<(string PropertyName, object PropertyValue)> ExtractProperties(string text)
    {
      return ExtractProperties(this, text);
    }

    /// <summary>
    /// Extracts properties from the given text using the configured name splitter and property evaluators.
    /// </summary>
    /// <param name="text">The text to extract properties from.</param>
    /// <param name="rootNode">The root node of the name splitter tree.</param>
    /// <returns>An enumerable of property name and value tuples.</returns>
    public static IEnumerable<(string PropertyName, object PropertyValue)> ExtractProperties(IPropertyExtractionTreeNode rootNode, string text)
    {
      if (rootNode is IPropertyEvaluator evaluator)
      {
        yield return evaluator.Evaluate(text);
      }
      else if (rootNode is INameSplitter splitter)
      {
        var parts = splitter.Split(text);
        foreach (var child in splitter.Children)
        {
          var index = child.IndexOfNamePart < 0 ? parts.Count + child.IndexOfNamePart : child.IndexOfNamePart;

          if (!(index >= 0 && index < parts.Count))
          {
            foreach (var propertyName in child.Node.EnumeratePropertyNames())
            {
              yield return (propertyName, $"The index {child.IndexOfNamePart} is outside the range [{-parts.Count}, {parts.Count - 1}] ({child.Node})");
            }
          }
          else
          {

            foreach (var propertyNameValue in ExtractProperties(child.Node, parts[index]))
            {
              yield return propertyNameValue;
            }
          }
        }
      }
    }

    /// <summary>
    /// Enumerates all property names in the name splitter tree, starting from the specified root node.
    /// </summary>
    /// <param name="rootNode">The root node of the name splitter tree.</param>
    /// <returns>An enumerable of property names.</returns>
    public static IEnumerable<string> EnumeratePropertyNames(IPropertyExtractionTreeNode rootNode)
    {
      if (rootNode is IPropertyEvaluator evaluator)
      {
        yield return evaluator.PropertyName;
      }
      else if (rootNode is INameSplitter splitter)
      {
        foreach (var child in splitter.Children)
        {
          foreach (var property in EnumeratePropertyNames(child.Node))
          {
            yield return property;
          }
        }
      }
    }

    /// <summary>
    /// Enumerates all property names in the name splitter tree.
    /// </summary>
    /// <returns>An enumerable of property names.</returns>
    public IEnumerable<string> EnumeratePropertyNames()
    {
      return EnumeratePropertyNames(this);
    }
  }
}
