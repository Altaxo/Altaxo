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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Altaxo.Data;
using Altaxo.Main;
using Altaxo.Main.Services;

namespace Altaxo.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Options for bulk importing, in which the file name is parsed to extract properties, which are then put into property bags at different levels, and the file is imported into a target table.
  /// </summary>
  public record ImportWithFileNameDerivedPropertiesAction : Main.IImmutable, Main.IAction
  {
    /// <summary>
    /// List of file names to import. The file names can contain wildcard characters (* and ?), which will be resolved to actual file paths.
    /// </summary>
    public ImmutableList<string> FileNamePatternsIncluded { get; init; } = ImmutableList<string>.Empty;

    /// <summary>
    /// List of file names to import. The file names can contain wildcard characters (* and ?), which will be resolved to actual file paths.
    /// </summary>
    public ImmutableList<string> FileNamePatternsExcluded { get; init; } = ImmutableList<string>.Empty;


    /// <summary>
    /// The target table name, which can contain placeholders for properties extracted from the file name. For example, "MyTable_&lt;&lt;&lt;Date&gt;&gt;&gt;" would create a table named "MyTable_2023-01-01" if the file name contains a date property with that value.
    /// </summary>
    public string TargetTableNameTemplate { get; init; } = string.Empty;

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

    /// <summary>
    /// Gets or sets the name of a folder or atable that is used as a template if the target table does not exist.
    /// In this case, if the string represents a folder, the contents of the folder is copied to the folder of the target table, or,
    /// if the string represents a table, the table is copied to the target table before importing the files.
    /// </summary>
    public string FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether existing items in the target folder should be overridden when copying the project items using <see cref="FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing"/>.
    /// </summary>
    public OverwriteBehavior OverwriteProjectItems { get; init; } = OverwriteBehavior.Overwrite;


    #region Serialization

    /// <summary>
    /// V0: 2026-08-10 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ImportWithFileNameDerivedPropertiesAction), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ImportWithFileNameDerivedPropertiesAction)o;
        info.AddArray("FileNamesIncluded", s.FileNamePatternsIncluded, s.FileNamePatternsIncluded.Count);
        info.AddArray("FileNamesExcluded", s.FileNamePatternsExcluded, s.FileNamePatternsExcluded.Count);
        info.AddValue("TargetTableNameTemplate", s.TargetTableNameTemplate);
        info.AddValue("NameSplitter", s.NameSplitter);
        info.AddArray("ActionsOnProperties", s.ActionsOnProperties, s.ActionsOnProperties.Count);
        info.AddEnum("BehaviorOnConflictWithExistingProperties", s.BehaviorOnConflictWithExistingProperties);
        info.AddValue("FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing", s.FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing);
        info.AddEnum("OverwriteProjectItems", s.OverwriteProjectItems);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fileNamesUnresolved = info.GetArrayOfStrings("FileNamesIncluded").ToImmutableList();
        var fileNamesExcluded = info.GetArrayOfStrings("FileNamesExcluded").ToImmutableList();
        var targetTableNameTemplate = info.GetString("TargetTableNameTemplate");
        var nameSplitter = info.GetValue<IPropertyExtractionTreeNode>("NameSplitter", null);
        var actionsOnProperties = info.GetArrayOfValues<IActionOnProperty>("ActionsOnProperties", null).ToImmutableList();
        var behaviorOnConflictWithExistingProperties = info.GetEnum<BehaviorOnConflictWithExistingProperties>("BehaviorOnConflictWithExistingProperties");
        var folderOrTableNameUsedAsTemplateIfTargetTableIsMissing = info.GetString("FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing");
        OverwriteBehavior overwriteProjectItems = OverwriteBehavior.Overwrite;
        if (info.CurrentElementName == "OverwriteProjectItems")
          overwriteProjectItems = info.GetEnum<OverwriteBehavior>("OverwriteProjectItems");

        return o is null ? new ImportWithFileNameDerivedPropertiesAction
        {
          FileNamePatternsIncluded = fileNamesUnresolved,
          FileNamePatternsExcluded = fileNamesExcluded,
          TargetTableNameTemplate = targetTableNameTemplate,
          NameSplitter = nameSplitter,
          ActionsOnProperties = actionsOnProperties,
          BehaviorOnConflictWithExistingProperties = behaviorOnConflictWithExistingProperties,
          FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing = folderOrTableNameUsedAsTemplateIfTargetTableIsMissing,
          OverwriteProjectItems = overwriteProjectItems,
        } : ((ImportWithFileNameDerivedPropertiesAction)o) with
        {
          FileNamePatternsIncluded = fileNamesUnresolved,
          FileNamePatternsExcluded = fileNamesExcluded,
          TargetTableNameTemplate = targetTableNameTemplate,
          NameSplitter = nameSplitter,
          ActionsOnProperties = actionsOnProperties,
          BehaviorOnConflictWithExistingProperties = behaviorOnConflictWithExistingProperties,
          FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing = folderOrTableNameUsedAsTemplateIfTargetTableIsMissing,
          OverwriteProjectItems = overwriteProjectItems,
        };
      }
    }
    #endregion

    /// <summary>
    /// Gets the target table name by replacing placeholders in the target template name with actual property values from the provided properties enumeration.
    /// </summary>
    /// <param name="targetTemplateName">The target template name containing placeholders.</param>
    /// <param name="properties">The enumeration of property names and values to replace the placeholders.</param>
    /// <returns>The target table name with placeholders replaced by actual property values.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GetProjectItemNameFromTemplateName(string targetTemplateName, IEnumerable<(string PropertyName, object PropertyValue)> properties)
    {
      // assume that the TargetName is a string in interpolated string format, even with the usual C# syntax
      // e.g. "MyTable_{Date}_{Experiment}
      // where 'Date' and 'Experiment' are property names that are provided in the properties enumeration
      // those placeholders will be replaced with the actual property values

      if (properties is null)
        throw new ArgumentNullException(nameof(properties));

      // Create a case-insensitive dictionary from the property enumeration
      var propDict = properties
          .GroupBy(p => p.PropertyName, StringComparer.OrdinalIgnoreCase)
          .ToDictionary(g => g.Key, g => g.First().PropertyValue, StringComparer.OrdinalIgnoreCase);

      // Matches placeholders in the format {PropertyName} or {PropertyName:FormatSpecifier}
      return Regex.Replace(targetTemplateName, @"\{([a-zA-Z0-9_]+)(?::([^}]+))?\}", match =>
      {
        string propName = match.Groups[1].Value;
        string format = match.Groups[2].Success ? match.Groups[2].Value : null;

        if (propDict.TryGetValue(propName, out var val) && val != null)
        {
          if (!string.IsNullOrEmpty(format) && val is IFormattable formattable)
          {
            try
            {
              return formattable.ToString(format, CultureInfo.InvariantCulture);
            }
            catch (System.FormatException ex)
            {
              return ex.Message;
            }
          }
          return val.ToString() ?? string.Empty;
        }

        // Keep original placeholder if property name is not found in the enumeration
        return match.Value;
      });
    }

    /// <summary>
    /// Gets a dictionary mapping target table names to lists of file paths, based on the provided file paths, target template name, and name splitter. Each file path is processed to extract properties and determine the corresponding target table name.
    /// </summary>
    /// <param name="filePaths">The file paths to process.</param>
    /// <param name="targetTemplateName">The target template name with placeholders.</param>
    /// <param name="nameSplitter">The name splitter used to extract properties from file names.</param>
    /// <returns>A dictionary mapping target table names to lists of file paths.</returns>
    public static Dictionary<string, (List<string> FileNames, Dictionary<string, object> CommonProperties)> GetTableNamesToFileNamesRelationship(IEnumerable<string> filePaths, string targetTemplateName, IPropertyExtractionTreeNode nameSplitter)
    {
      var relationship = new Dictionary<string, (List<string> FileNames, Dictionary<string, object> CommonProperties)>(StringComparer.OrdinalIgnoreCase);
      foreach (var filePath in filePaths)
      {
        var properties = nameSplitter.ExtractProperties(filePath);
        var targetTableName = GetProjectItemNameFromTemplateName(targetTemplateName, properties);
        if (!relationship.TryGetValue(targetTableName, out var filesAndProps))
        {
          filesAndProps = (new List<string>(), new Dictionary<string, object>());
          relationship[targetTableName] = filesAndProps;
        }
        if (!filesAndProps.FileNames.Contains(filePath, StringComparer.OrdinalIgnoreCase))
        {
          filesAndProps.FileNames.Add(filePath);
        }

        foreach (var prop in properties)
        {
          if (!filesAndProps.CommonProperties.ContainsKey(prop.PropertyName))
          {
            filesAndProps.CommonProperties[prop.PropertyName] = prop.PropertyValue;
          }
          else
          {
            // If the property already exists, check if the value is the same. If not, remove it from common properties.
            if (!object.Equals(filesAndProps.CommonProperties[prop.PropertyName], prop.PropertyValue))
            {
              filesAndProps.CommonProperties.Remove(prop.PropertyName);
            }
          }
        }
      }
      return relationship;
    }

    /// <summary>
    /// Get the target kind where to place property-value tuples. The target kind can be a property bag, a table, or table columns.
    /// </summary>
    public enum TargetItemKind
    {
      /// <summary>
      /// The property-value tuples should be placed in a property document.
      /// </summary>
      PropertyDocument,

      /// <summary>
      /// The property-value tuples should be placed in the property bag of the table.
      /// </summary>
      Table,

      /// <summary>
      /// The property-value tuples should be placed in property columns of each of the table columns.
      /// </summary>
      TableColumns,
    }

    /// <summary>
    /// Gets a list of tuples containing the target item kind, project item name, property name, and property value for each file path based on the provided target template name, name splitter, and actions on properties. Each file path is processed to extract properties and determine where to place them in the project structure.
    /// </summary>
    /// <param name="filePaths">The file paths to process.</param>
    /// <param name="nameSplitter">The name splitter used to extract properties from file names.</param>
    /// <param name="targetTemplateName">The target template name with placeholders.</param>
    /// <param name="actionsOnProperties">The actions to perform on the extracted properties.</param>
    /// <returns>A list of tuples containing the target item kind, project item name, property name, and property value.</returns>
    /// <exception cref="Exception"></exception>
    public static List<(TargetItemKind Kind, string ProjectItemName, string PropertyName, object PropertyValue)> GetPropertiesPlacedInProjectItems(IEnumerable<string> filePaths, IPropertyExtractionTreeNode nameSplitter, string targetTemplateName, IEnumerable<IActionOnProperty> actionsOnProperties)
    {
      var result = new List<(TargetItemKind Kind, string ProjectItemName, string PropertyName, object PropertyValue)>();
      foreach (var filePath in filePaths)
      {
        var properties = nameSplitter.ExtractProperties(filePath);
        var targetTableName = GetProjectItemNameFromTemplateName(targetTemplateName, properties);
        foreach (var action in actionsOnProperties.OfType<ActionPutToPropertyBag>())
        {
          // get the property value from the properties extracted from the file name

          var propertyValue = properties.FirstOrDefault(p => p.PropertyName == action.PropertyName).PropertyValue;

          if (propertyValue is null)
            continue;


          // find the property bag depending on the level
          if (action.Level == 1)
          {
            result.Add((TargetItemKind.TableColumns, targetTableName, action.PropertyName, propertyValue));
          }
          if (action.Level == 0)
          {
            result.Add((TargetItemKind.Table, targetTableName, action.PropertyName, propertyValue));
          }
          else
          {
            var folder = Altaxo.Main.ProjectFolder.GetFolderPart(targetTableName);
            for (int i = -1; i > action.Level; i--)
            {
              var newfolder = Altaxo.Main.ProjectFolder.GetFoldersParentFolder(folder);

              if (newfolder == folder)
              {
                throw new Exception($"Cannot find parent folder for level {action.Level} in target table name {targetTableName}");
              }

              folder = newfolder;
            }

            result.Add((TargetItemKind.PropertyDocument, folder, action.PropertyName, propertyValue));
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Gets a list of tuples containing the target item kind, project item name, property name, property value, and diagnostic message for each file path based on the provided target template name, name splitter, and actions on properties. Each file path is processed to extract properties and determine where to place them in the project structure. If there are inconsistent property values for the same (TargetItemKind, ProjectItemName, PropertyName) tuple across different file paths, a diagnostic message will be included in the result.
    /// </summary>
    /// <param name="filePaths">The file paths to process.</param>
    /// <param name="nameSplitter">The name splitter to use for extracting properties from file names.</param>
    /// <param name="targetTemplateName">The target template name to use for determining the target table name.</param>
    /// <param name="actionsOnProperties">The actions to perform on the extracted properties.</param>
    /// <returns>A list of tuples containing the target item kind, project item name, property name, property value, and diagnostic message.</returns>
    public static List<(TargetItemKind Kind, string ProjectItemName, string PropertyName, object PropertyValue, string Diagnostic)> GetPropertiesPlacedInProjectItemsWithDiagnostics(IEnumerable<string> filePaths, IPropertyExtractionTreeNode nameSplitter, string targetTemplateName, IEnumerable<IActionOnProperty> actionsOnProperties)
    {
      // Test the consistency of the property values: each tuple of(TargetItemKind, ProjectItemName, PropertyName) should have the same property value for all file paths. If not, we will add a diagnostic message to the result.
      var dict1 = new Dictionary<(TargetItemKind Kind, string ProjectItemName, string PropertyName), (object PropertyValue, string Diagnostic)>();
      var allProperties = GetPropertiesPlacedInProjectItems(filePaths, nameSplitter, targetTemplateName, actionsOnProperties).ToList();

      foreach (var tuple in allProperties)
      {
        var key = (tuple.Kind, tuple.ProjectItemName, tuple.PropertyName);
        if (dict1.TryGetValue(key, out var existingValue))
        {
          if (!object.Equals(existingValue.PropertyValue, tuple.PropertyValue))
          {
            // add a diagnostic message to the result
            dict1[key] = (null, $"Inconsistent property value: {existingValue} versus {tuple.PropertyValue}");
          }
        }
        else
        {
          dict1[key] = (tuple.PropertyValue, null);
        }
      }

      var result = new List<(TargetItemKind Kind, string ProjectItemName, string PropertyName, object PropertyValue, string Diagnostic)>();
      foreach (var kvp in dict1)
      {
        result.Add((kvp.Key.Kind, kvp.Key.ProjectItemName, kvp.Key.PropertyName, kvp.Value.PropertyValue, kvp.Value.Diagnostic));
      }

      result.Sort((x, y) =>
      {
        var r = string.Compare(x.ProjectItemName, y.ProjectItemName, StringComparison.OrdinalIgnoreCase);
        if (r != 0) return r;
        return Comparer<int>.Default.Compare((int)x.Kind, (int)y.Kind);

      });

      return result;
    }


    /// <summary>
    /// Bulk import files, by extracting properties from the file name, putting them into property bags at different levels, and importing the file into the target table.
    /// </summary>
    public void Execute(IProgressReporter reporter, params object[] args)
    {
      reporter?.ReportProgress("Scan file system and resolve file names", 0d);
      var files = ResolveFileNames(FileNamePatternsIncluded, reporter.CancellationToken);
      BulkImportFiles(files, reporter.CancellationToken, reporter);
    }

    /// <summary>
    /// Ensures the existence of the target table with the specified name. If the table does not exist, it will be created. If a folder or table is specified as a template, it will be used to create the target table if it is missing.
    /// </summary>
    /// <param name="targetTableName">The name of the target table.</param>
    /// <param name="properties">The properties to be used for conditional template matching.</param>
    /// <returns>The existing or newly created target table.</returns>
    public Altaxo.Data.DataTable EnsureExistenceOfTargetTable(string targetTableName, IReadOnlyDictionary<string, object> properties)
    {
      var targetFolder = Altaxo.Main.ProjectFolder.GetFolderPart(targetTableName);

      // First of all, use the common template
      if (!string.IsNullOrEmpty(FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing))
      {
        if (Current.Project.DataTableCollection.TryGetValue(FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing, out var templateTable))
        {
          var newTable = (DataTable)templateTable.Clone();
          newTable.Name = targetTableName;
          Current.Project.DataTableCollection.Add(newTable);
          return newTable;
        }

        if (Altaxo.Main.ProjectFolder.IsValidFolderName(FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing) &&
            Current.Project.Folders.GetItemsInFolder(FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing).Any() &&
            !Current.Project.Folders.GetItemsInFolder(targetFolder).Any())
        {
          Current.Project.Folders.CopyItemsFromFolderToFolder(FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing, targetFolder);
        }
      }

      // Second, lets see if we have conditional templates
      var conditionalTemplates = ActionsOnProperties.OfType<IActionConditionForTemplate>().ToList();

      if (conditionalTemplates.Count > 0)
      {
        foreach (var conditionalTemplate in conditionalTemplates)
        {
          if (conditionalTemplate.Matches(properties))
          {
            foreach (var itemNameTemplate in conditionalTemplate.ProjectItemsUsedAsTemplate)
            {
              var itemName = GetProjectItemNameFromTemplateName(itemNameTemplate, properties.Select(kvp => (kvp.Key, kvp.Value)));

              if (ProjectFolder.IsValidFolderName(itemName))
              {

              }
              else
              {
                var sourceItems = Current.Project.Folders.GetProjectItemsByName(itemName);
                if (sourceItems.Count == 0)
                {
                  continue;
                }
                Current.Project.Folders.CopyItemsToFolder(sourceItems.ToList<object>(), targetFolder, null, conditionalTemplate.OverwriteProjectItems);
              }
            }
          }
        }
      }

      if (Current.Project.DataTableCollection.TryGetValue(targetTableName, out var targetTable))
      {
        return targetTable;
      }
      else
      {
        targetTable = new DataTable() { Name = targetTableName };
        Current.Project.DataTableCollection.Add(targetTable);
        return targetTable;
      }
    }


    /// <summary>
    /// Bulk import files, by extracting properties from the file name, putting them into property bags at different levels, and importing the file into the target table.
    /// </summary>
    /// <param name="files">The files to import.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="progressReporter">The progress reporter.</param>
    /// <exception cref="Exception"></exception>
    public void BulkImportFiles(IEnumerable<FileInfo> files, CancellationToken cancellationToken, IProgress<(string text, double fraction)>? progressReporter)
    {
      var tableNamesToFileNames = GetTableNamesToFileNamesRelationship(files.Select(f => f.FullName), TargetTableNameTemplate, NameSplitter);
      var propertiesPlacedInProjectItems = GetPropertiesPlacedInProjectItemsWithDiagnostics(files.Select(f => f.FullName), NameSplitter, TargetTableNameTemplate, ActionsOnProperties);
      var tableNamesToTables = new Dictionary<string, DataTable>();

      double index = 0;
      foreach (var tableNameToFileNames in tableNamesToFileNames)
      {
        cancellationToken.ThrowIfCancellationRequested();

        progressReporter?.Report(($"Import into table '{tableNameToFileNames.Key}'", index / tableNamesToFileNames.Count));
        ++index;

        var targetTable = EnsureExistenceOfTargetTable(tableNameToFileNames.Key, tableNameToFileNames.Value.CommonProperties);
        tableNamesToTables[tableNameToFileNames.Key] = targetTable;

        if (targetTable.DataSource is FileImportTableDataSourceBase fitds)
        {
          fitds.SourceFileNames = tableNameToFileNames.Value.FileNames;
          fitds.FillData(targetTable, DummyProgressReporter.Instance);
        }
        else
        {
          var importers = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IDataFileImporter))
                   .Select(x => (IDataFileImporter)Activator.CreateInstance(x))
                   .ToList();

          var importer = DataFileImporterBase.GetDataFileImporterForFile(tableNameToFileNames.Value.FileNames.First(), importers);

          var importOptions = importer.CheckOrCreateImportOptions(null);

          var newDS = importer.CreateTableDataSource(tableNameToFileNames.Value.FileNames, importOptions);
          targetTable.DataSource = newDS;
          newDS?.FillData(targetTable, DummyProgressReporter.Instance);
        }
      }

      // now, add the properties

      foreach (var entry in propertiesPlacedInProjectItems)
      {
        if (!string.IsNullOrEmpty(entry.Diagnostic))
        {
          throw new Exception($"Inconsistent property value for {entry.Kind} '{entry.ProjectItemName}', property '{entry.PropertyName}': {entry.Diagnostic}");
        }
        Main.Properties.PropertyBag? pb = null;
        switch (entry.Kind)
        {
          case TargetItemKind.PropertyDocument:
            if (!Current.Project.ProjectFolderProperties.TryGetValue(entry.ProjectItemName, out var pbdoc))
            {
              pbdoc = new Main.Properties.ProjectFolderPropertyDocument(entry.ProjectItemName);
              Current.Project.ProjectFolderProperties.Add(pbdoc);
            }
            pb = pbdoc.PropertyBagNotNull;
            break;
          case TargetItemKind.Table:
            if (tableNamesToTables.TryGetValue(entry.ProjectItemName, out var targetTable))
            {
              pb = targetTable.PropertyBagNotNull;
            }
            break;
          case TargetItemKind.TableColumns:
            throw new NotImplementedException("Setting properties on table columns is not implemented yet.");
          default:
            throw new Exception($"Unknown TargetItemKind: {entry.Kind}");
        }
        pb?.SetValue(entry.PropertyName, entry.PropertyValue);
      }
      ;

    }

    #region File name resolution

    static IEnumerable<FileInfo> ResolvePathWithJokerChars(string path, CancellationToken cancellationToken)
    {
      if (path is null)
        throw new ArgumentNullException(nameof(path));

      var normalizedPath = path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
      var root = Path.GetPathRoot(normalizedPath);
      var relativePath = root is not null && normalizedPath.StartsWith(root, StringComparison.OrdinalIgnoreCase) ? normalizedPath[root.Length..] : normalizedPath;
      var segments = relativePath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
      var startDirectory = string.IsNullOrEmpty(root) ? Directory.GetCurrentDirectory() : root;

      foreach (var file in MatchPath(startDirectory, segments, 0, cancellationToken))
      {
        yield return file;
      }
    }

    static IEnumerable<FileInfo> MatchPath(string currentPath, IReadOnlyList<string> segments, int segmentIndex, CancellationToken cancellationToken)
    {
      if (!Directory.Exists(currentPath))
        yield break;

      if (segmentIndex >= segments.Count)
      {
        if (File.Exists(currentPath))
          yield return new FileInfo(currentPath);
        yield break;
      }

      var segment = segments[segmentIndex];
      if (segment == "**")
      {
        foreach (var match in MatchPath(currentPath, segments, segmentIndex + 1, cancellationToken))
        {
          yield return match;
        }

        foreach (var subDirectory in Directory.EnumerateDirectories(currentPath))
        {
          cancellationToken.ThrowIfCancellationRequested();
          foreach (var match in MatchPath(subDirectory, segments, segmentIndex, cancellationToken))
          {
            yield return match;
          }
        }

        if (segmentIndex == segments.Count - 1)
        {
          foreach (var file in Directory.EnumerateFiles(currentPath))
          {
            yield return new FileInfo(file);
          }
        }

        yield break;
      }

      foreach (var entry in Directory.EnumerateFileSystemEntries(currentPath))
      {
        cancellationToken.ThrowIfCancellationRequested();

        var entryName = Path.GetFileName(entry);
        if (!MatchesWildcardSegment(segment, entryName))
          continue;

        if (segmentIndex == segments.Count - 1)
        {
          if (File.Exists(entry))
            yield return new FileInfo(entry);
        }
        else if (Directory.Exists(entry))
        {
          foreach (var match in MatchPath(entry, segments, segmentIndex + 1, cancellationToken))
          {
            yield return match;
          }
        }
      }
    }

    static bool MatchesWildcardSegment(string pattern, string value)
    {
      var regexPattern = "^" + Regex.Escape(pattern)
        .Replace("\\*", ".*")
        .Replace("\\?", ".") + "$";
      return Regex.IsMatch(value, regexPattern, RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Resolves a list of file names, which may contain wildcard characters (* and ?), into a list of FileInfo objects. If a file name does not exist, a FileNotFoundException is thrown.
    /// </summary>
    /// <param name="unresolvedFileNames">The list of file names to resolve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while resolving file names.</param>
    /// <returns>A list of FileInfo objects representing the resolved files.</returns>
    /// <exception cref="FileNotFoundException">Thrown if a file does not exist.</exception>
    public static IEnumerable<FileInfo> ResolveFileNames(IEnumerable<string> unresolvedFileNames, CancellationToken cancellationToken)
    {
      var result = new List<FileInfo>();

      foreach (var fileName in unresolvedFileNames)
      {
        cancellationToken.ThrowIfCancellationRequested();

        var trimmedFileName = fileName.Trim();

        if (trimmedFileName.Contains('*') || trimmedFileName.Contains('?'))
        {
          foreach (var file in ResolvePathWithJokerChars(trimmedFileName, cancellationToken))
          {
            cancellationToken.ThrowIfCancellationRequested();
            result.Add(file);
          }
        }
        else
        {
          if (File.Exists(trimmedFileName))
          {
            result.Add(new FileInfo(trimmedFileName));
          }
        }
      }
      return result;
    }

    #endregion File name resolution

  }
}
