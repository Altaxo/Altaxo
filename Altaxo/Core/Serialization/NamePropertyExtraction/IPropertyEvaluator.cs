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

namespace Altaxo.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Defines a contract for evaluating text and extracting property names and values.
  /// </summary>
  public interface IPropertyEvaluator : IPropertyExtractionTreeNode
  {
    /// <summary>
    /// Evaluates a text and extracts a property name and its value.
    /// </summary>
    /// <param name="text">The text to evaluate.</param>
    /// <returns>A tuple containing the property name and its value.</returns>
    (string PropertyName, object PropertyValue) Evaluate(string text);

    /// <summary>
    /// Gets the name of the property that is evaluated.
    /// </summary>
    string PropertyName { get; }
  }
}
