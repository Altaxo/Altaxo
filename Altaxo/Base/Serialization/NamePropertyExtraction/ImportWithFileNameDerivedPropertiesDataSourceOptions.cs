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

namespace Altaxo.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Options for a <see cref="FileImportTableDataSourceBase"/> that is used to import data from files and derive properties from the file name.
  /// </summary>
  public record ImportWithFileNameDerivedPropertiesDataSourceOptions : Main.IImmutable
  {

    /// <summary>
    /// The name splitter to use for extracting properties from the file name. The default is a PathToFileNameSplitter, which extracts properties from the file name based on a predefined pattern.
    /// </summary>
    public IPropertyExtractionTreeNode NameSplitter { get; init; } = new PathToFileNameSplitter();

    /// <summary>
    /// Designate, which properties should be put into which property bag. The level indicates the level of the property bag, where 0 is the property bag of the target table, -1 is the property bag of the parent folder, -2 is the property bag of the grandparent folder, and so on.
    /// </summary>
    public ImmutableList<IActionOnProperty> ActionsOnProperties { get; init; } = ImmutableList<IActionOnProperty>.Empty;

    /// <summary>
    /// Gets or sets the behavior when there is a conflict with existing properties in the target property bag. The default is to override the existing properties with the new values.
    /// </summary>
    public BehaviorOnConflictWithExistingProperties BehaviorOnConflictWithExistingProperties { get; init; } = BehaviorOnConflictWithExistingProperties.Override;


    #region Serialization

    /// <summary>
    /// V0: 2026-08-10 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ImportWithFileNameDerivedPropertiesDataSourceOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ImportWithFileNameDerivedPropertiesDataSourceOptions)o;
        info.AddValue("NameSplitter", s.NameSplitter);
        info.AddArray("ActionsOnProperties", s.ActionsOnProperties, s.ActionsOnProperties.Count);
        info.AddEnum("BehaviorOnConflictWithExistingProperties", s.BehaviorOnConflictWithExistingProperties);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var nameSplitter = info.GetValue<IPropertyExtractionTreeNode>("NameSplitter", null);
        var actionsOnProperties = info.GetArrayOfValues<IActionOnProperty>("ActionsOnProperties", null).ToImmutableList();
        var behaviorOnConflictWithExistingProperties = info.GetEnum<BehaviorOnConflictWithExistingProperties>("BehaviorOnConflictWithExistingProperties");

        return o is null ? new ImportWithFileNameDerivedPropertiesDataSourceOptions
        {
          NameSplitter = nameSplitter,
          ActionsOnProperties = actionsOnProperties,
          BehaviorOnConflictWithExistingProperties = behaviorOnConflictWithExistingProperties,
        } : ((ImportWithFileNameDerivedPropertiesDataSourceOptions)o) with
        {
          NameSplitter = nameSplitter,
          ActionsOnProperties = actionsOnProperties,
          BehaviorOnConflictWithExistingProperties = behaviorOnConflictWithExistingProperties,
        };
      }
    }
    #endregion



  }
}
