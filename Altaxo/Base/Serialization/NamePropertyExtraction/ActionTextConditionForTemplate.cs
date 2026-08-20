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
using Altaxo.Main;

namespace Altaxo.Serialization.NamePropertyExtraction
{


  /// <summary>
  /// Represents an action that conditionally copies project items to a target folder based on a specified condition. The action checks if a property in the property bag matches the specified condition value, and if it does, the associated project items are copied to the target folder.
  /// </summary>
  public record ActionTextConditionForTemplate : IActionConditionForTemplate
  {
    /// <summary>
    /// Gets or sets the name of the property to be put into the property bag.
    /// </summary>
    public required string PropertyName { get; init; }

    /// <summary>
    /// Gets or sets the condition value that must be met for the action to be executed.
    /// If the property matches the condition value, the <see cref="ProjectItemsUsedAsTemplate"/> will be copied to the target folder.
    /// </summary>
    public required string Condition { get; init; }

    /// <summary>
    /// Designates how to compare the property value to the <see cref="Condition"/>.
    /// </summary>
    public StringComparisonKind ConditionComparisonKind { get; init; } = StringComparisonKind.Equality;

    /// <summary>
    /// Gets or sets a value indicating whether the condition check should be case-sensitive.
    /// </summary>
    public bool IsConditionCaseSensitive { get; init; } = true;

    /// <summary>
    /// List of project items to be copied to the target folder if the condition is met.
    /// </summary>
    public required ImmutableList<string> ProjectItemsUsedAsTemplate { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether existing items in the target folder should be overridden when copying the project items.
    /// </summary>
    public OverwriteBehavior OverwriteProjectItems { get; init; } = OverwriteBehavior.Overwrite;

    /// <inheritdoc/>
    public string Description
    {
      get
      {
        return ConditionComparisonKind switch
        {
          StringComparisonKind.Equality => $"if == \"{Condition}\" copy \"{string.Join("; ", ProjectItemsUsedAsTemplate)}\"",
          StringComparisonKind.Inequality => $"if != \"{Condition}\" copy \"{string.Join("; ", ProjectItemsUsedAsTemplate)}\"",
          StringComparisonKind.Contains => $"if contains \"{Condition}\" copy \"{string.Join("; ", ProjectItemsUsedAsTemplate)}\"",
          StringComparisonKind.StartsWith => $"if starts with \"{Condition}\" copy \"{string.Join("; ", ProjectItemsUsedAsTemplate)}\"",
          StringComparisonKind.EndsWith => $"if ends with \"{Condition}\" copy \"{string.Join("; ", ProjectItemsUsedAsTemplate)}\"",
          _ => $"if {ConditionComparisonKind} \"{Condition}\" copy \"{string.Join("; ", ProjectItemsUsedAsTemplate)}\"",
        };
      }
    }


    #region Serialization

    /// <summary>
    /// V0: 2026-08-19 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ActionTextConditionForTemplate), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ActionTextConditionForTemplate)o;
        info.AddValue("PropertyName", s.PropertyName);
        info.AddValue("Condition", s.Condition);
        info.AddEnum("ConditionComparisonKind", s.ConditionComparisonKind);
        info.AddValue("IsConditionCaseSensitive", s.IsConditionCaseSensitive);
        info.AddArray("ProjectItemsUsedAsTemplate", s.ProjectItemsUsedAsTemplate, s.ProjectItemsUsedAsTemplate.Count);
        info.AddEnum("OverwriteProjectItems", s.OverwriteProjectItems);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var propertyName = info.GetString("PropertyName");
        var condition = info.GetString("Condition");
        var conditionComparisonKind = info.GetEnum<StringComparisonKind>("ConditionComparisonKind");
        var isConditionCaseSensitive = info.GetBoolean("IsConditionCaseSensitive");
        var projectItemsUsedAsTemplate = info.GetArrayOfStrings("ProjectItemsUsedAsTemplate").ToImmutableList();
        var overwriteProjectItems = info.GetEnum<OverwriteBehavior>("OverwriteProjectItems");

        return o is null ? new ActionTextConditionForTemplate
        {
          PropertyName = propertyName,
          Condition = condition,
          IsConditionCaseSensitive = isConditionCaseSensitive,
          ProjectItemsUsedAsTemplate = projectItemsUsedAsTemplate,
          OverwriteProjectItems = overwriteProjectItems,
        } : ((ActionTextConditionForTemplate)o) with
        {
          PropertyName = propertyName,
          Condition = condition,
          IsConditionCaseSensitive = isConditionCaseSensitive,
          ProjectItemsUsedAsTemplate = projectItemsUsedAsTemplate,
          OverwriteProjectItems = overwriteProjectItems,
        };
      }
    }
    #endregion


    /// <summary>
    /// Evaluates whether the specified properties match the condition defined by this action. The condition is based on the property name and value specified in the action. If a property with the same name exists in the provided properties and its value matches the condition, the method returns true; otherwise, it returns false.
    /// </summary>
    /// <param name="properties">The existing properties and respective values.</param>
    /// <returns>True if the properties match the condition; otherwise, false.</returns>
    public bool Matches(IReadOnlyDictionary<string, object> properties)
    {
      var result = false;

      if (properties.TryGetValue(PropertyName, out var value))
      {
        var valueString = value?.ToString() ?? string.Empty;
        var compareCase = IsConditionCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        result = valueString.Matches(Condition, ConditionComparisonKind, compareCase);
      }
      return result;
    }

  }
}
