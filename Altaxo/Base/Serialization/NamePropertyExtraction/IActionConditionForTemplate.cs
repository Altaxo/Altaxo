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
using Altaxo.Main;

namespace Altaxo.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Represents an action that conditionally copies project items to a target folder based on a specified condition. The action checks if a property in the property bag matches the specified condition value, and if it does, the associated project items are copied to the target folder.
  /// </summary>
  public interface IActionConditionForTemplate : IActionOnProperty
  {
    /// <summary>
    /// Evaluates whether the specified properties match the condition defined by this action. The condition is based on the property name and value specified in the action. If a property with the same name exists in the provided properties and its value matches the condition, the method returns true; otherwise, it returns false.
    /// </summary>
    /// <param name="properties">The existing properties and respective values.</param>
    /// <returns>True if the properties match the condition; otherwise, false.</returns>
    bool Matches(IReadOnlyDictionary<string, object> properties);


    /// <summary>
    /// List of project items to be copied to the target folder if the condition is met.
    /// </summary>
    public ImmutableList<string> ProjectItemsUsedAsTemplate { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether existing items in the target folder should be overridden when copying the project items.
    /// </summary>
    public OverwriteBehavior OverwriteProjectItems { get; init; }
  }
}
