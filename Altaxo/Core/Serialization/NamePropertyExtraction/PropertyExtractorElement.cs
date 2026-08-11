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

namespace Altaxo.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Represents an element that extracts properties from names using a name splitter and property evaluators.
  /// </summary>
  public class PropertyExtractorElement
  {
    /// <summary>
    /// Gets or sets the name splitter used to split names into parts.
    /// </summary>
    public required INameSplitter NameSplitter { get; init; }

    /// <summary>
    /// Gets a dictionary of property evaluators, where the key is the index of the name part to evaluate and the value is the corresponding property evaluator.
    /// </summary>
    public Dictionary<int, IPropertyEvaluator> PropertyEvaluators
    {
      get;
    } = new Dictionary<int, IPropertyEvaluator>();

    /// <summary>
    /// Extracts properties from the given text using the configured name splitter and property evaluators.
    /// </summary>
    /// <param name="text">The text to extract properties from.</param>
    /// <returns>An enumerable of property name and value tuples.</returns>
    public IEnumerable<(string PropertyName, object PropertyValue)> ExtractProperties(string text)
    {
      var parts = NameSplitter.Split(text);

      foreach (var entry in PropertyEvaluators)
      {
        int index = entry.Key;
        if (index < 0)
        {
          index = parts.Count + index;
        }

        if (!(index >= 0 && index < parts.Count))
        {
          yield return (entry.Value.PropertyName, $"The index {entry.Key} is out of range of [{-parts.Count}, {parts.Count - 1}].");
          continue;
        }

        var result = entry.Value.Evaluate(parts[index >= 0 ? index : parts.Count + index]);
        yield return result;
      }
    }
  }
}
