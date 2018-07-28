#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
  /// <summary>
  /// Handler to report owned <see cref="DocNodeProxy"/> instances to a visitor. If the visited object owns <see cref="DocNodeProxy"/> objects as childs, it must call this delegate with
  /// the first argument being the owned <see cref="DocNodeProxy"/>, the second argument being the visited object itself, and the third argument being the property to access the <see cref="DocNodeProxy"/> instance.
  /// </summary>
  /// <param name="proxy">The proxy that is owned by the instance.</param>
  /// <param name="owner">The instance that owns the proxy.</param>
  /// <param name="propertyName">Name of the property that accesses the proxy in this instance.</param>
  public delegate void DocNodeProxyReporter(IProxy proxy, object owner, string propertyName);

  /// <summary>
  /// Can be used to to relocate the <see cref="AbsoluteDocumentPath"/> that the <see cref="DocNodeProxy"/> is holding. The <see cref="Visit"/> function implements
  /// the <see cref="DocNodeProxyReporter"/> delegate that is used to enumerate all proxies to <see cref="Altaxo.Data.DataColumn"/>s and change there references
  /// to the corresponding <see cref="Altaxo.Data.DataColumn"/>s of another table.
  /// </summary>
  public class DocNodePathReplacementOptions
  {
    /// <summary>
    /// The item relocation dictionary. This dictionary contains the whole <see cref="AbsoluteDocumentPath"/> of items that were relocated. The key is the old <see cref="AbsoluteDocumentPath"/> of the item,
    /// the value is the new <see cref="AbsoluteDocumentPath"/> of the item. This dictionary has a higher priority than the _replacementDictionary.
    /// </summary>
    private Dictionary<AbsoluteDocumentPath, AbsoluteDocumentPath> _itemRelocationDictionary = new Dictionary<AbsoluteDocumentPath, AbsoluteDocumentPath>();

    /// <summary>
    /// This are two lists: the first list contains part replacement lists on different levels.
    /// Thus the first item at index 0 in this list are the immediate replacements from the original path to the replaced path.
    /// The second item at index 1 in the list are the replacements, where we have both the original path and the replace path without the topmost folder level.
    /// The following items remove successivly the topmost folder level from both original path and replacement path
    ///
    /// </summary>
    private List<List<KeyValuePair<AbsoluteDocumentPath, AbsoluteDocumentPath>>> _pathPartReplacementDictionary = new List<List<KeyValuePair<AbsoluteDocumentPath, AbsoluteDocumentPath>>>();

    /// <summary>
    /// Visits a <see cref="DocNodeProxy"/> and applies the modifications to the document path of that proxy.
    /// </summary>
    /// <param name="proxy">The <see cref="DocNodeProxy"/> to modify.</param>
    /// <param name="owner">The instance that owns the proxy.</param>
    /// <param name="propertyName">Name of the property that accesses the proxy in this instance.</param>
    public void Visit(IProxy proxy, object owner, string propertyName)
    {
      if (null == proxy)
        return;

      var docPath = proxy.DocumentPath;

      // the _itemRelocationDictionary has first priority
      if (_itemRelocationDictionary != null && _itemRelocationDictionary.Count > 0)
      {
        for (int i = docPath.Count; i >= 2; --i)
        {
          var subPath = docPath.SubPath(0, i);
          AbsoluteDocumentPath replacePath;
          if (_itemRelocationDictionary.TryGetValue(subPath, out replacePath))
          {
            proxy.ReplacePathParts(subPath, replacePath, (IDocumentLeafNode)owner);
            return;
          }
        }
      }

      // the pathReplacementDictionary has 2nd priority
      foreach (var levelEntry in _pathPartReplacementDictionary)
      {
        foreach (var entry in levelEntry)
        {
          if (proxy.ReplacePathParts(entry.Key, entry.Value, (IDocumentLeafNode)owner))
            return;
        }
      }
    }

    /// <summary>
    /// Adds a replacement entry for a project item. The paths given should be complete paths of the project item (original path and new path).
    /// </summary>
    /// <param name="originalPath">The original path of the project item (DataTable, Graph, ProjectFolderPropertyBag).</param>
    /// <param name="newPath">The new path of the project item.</param>
    public void AddProjectItemReplacement(AbsoluteDocumentPath originalPath, AbsoluteDocumentPath newPath)
    {
      _itemRelocationDictionary.Add(originalPath, newPath);
    }

    /// <summary>
    /// Adds replacement rules for path parts for all project item types.
    /// </summary>
    /// <param name="originalItemNamePart">The original item name part. Usually, that is a subfolder part of the original item name.</param>
    /// <param name="newItemNamePart">The new item name part. Usually, this is the subfolder part of the new item name. </param>
    public void AddPathReplacementsForAllProjectItemTypes(string originalItemNamePart, string newItemNamePart)
    {
      if (null == originalItemNamePart)
        throw new NullReferenceException("originalItemPart");
      if (null == newItemNamePart)
        throw new NullReferenceException("newItemPart");

      int level = 0;

      var currentProject = Current.IProjectService.CurrentProject;
      if (null == currentProject)
        return;

      while (originalItemNamePart != newItemNamePart)
      {
        if (!(level < _pathPartReplacementDictionary.Count))
        {
          _pathPartReplacementDictionary.Add(new List<KeyValuePair<AbsoluteDocumentPath, AbsoluteDocumentPath>>());
        }

        foreach (var itemType in currentProject.ProjectItemTypes)
        {
          var orgPath = currentProject.GetRootPathForProjectItemType(itemType).Append(originalItemNamePart);
          var newPath = currentProject.GetRootPathForProjectItemType(itemType).Append(newItemNamePart);
          _pathPartReplacementDictionary[level].Add(new KeyValuePair<AbsoluteDocumentPath, AbsoluteDocumentPath>(orgPath, newPath));
        }

        if (string.IsNullOrEmpty(originalItemNamePart) || string.IsNullOrEmpty(newItemNamePart))
        {
          break;
        }
        else // strip the topmost folder of both original and replacement
        {
          originalItemNamePart = ProjectFolder.GetFoldersParentFolder(originalItemNamePart);
          newItemNamePart = ProjectFolder.GetFoldersParentFolder(newItemNamePart);
          ++level;
        }
      }
    }
  }
}
