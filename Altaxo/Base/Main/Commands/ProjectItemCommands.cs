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

using Altaxo.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main.Commands
{
  using Altaxo.Serialization.Clipboard;
  using Gui.Common;

  /// <summary>
  /// Commands on project items like DataTables, Graphs and so.
  /// </summary>
  public static class ProjectItemCommands
  {
    #region Clipboard commands

    public class ProjectItemClipboardListBase
    {
      /// <summary>Folder from which the items are copied.</summary>
      public string BaseFolder { get; set; }

      /// <summary>If true, references will be relocated in the same way as the project items will be relocated.</summary>
      /// <value><c>true</c> if references should be relocated, <c>false</c> otherwise</value>
      public bool? RelocateReferences { get; set; }

      /// <summary>
      /// When true, at serialization the internal references are tried to keep internal, i.e. if for instance a table have to be renamed, the plot items in the deserialized graphs
      /// will be relocated to the renamed table.
      /// </summary>
      public bool? TryToKeepInternalReferences { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// There are two groups of possible options:
    /// <para>Group1: where are the items included?</para>
    /// <para>Possibilities: (i) with absolute names, i.e. the item name remains as is. (ii) relative to the project folder where it is included and where it was copied from.</para>
    /// <para>Group2: what should be happen with the references (for instance in plot items to the table columns)</para>
    /// <para>Possibilities: (i) nothing, they remain as is (ii) they will be relocated taking the source folder and destination folder into account, or (iii) same as (ii) but also
    /// taking the renaming of the copied tables into account, if there is a name conflict with an already existing worksheet for instance.</para>
    /// <para>The first group is controlled by the <c>baseFolder</c> parameters during creation of the object (source base folder) and during pasting of the items (destination base folder).
    /// If source base folder or destination base folder is null, the items will be included with absolute names. This is usually the case when copying or pasting from/into the AllGraph, AllWorksheet or
    /// AllProject items view. If both source base folder and destination base folder have a value, then the items are included relative to both folders.</para>
    /// <para>
    /// </para>
    /// </remarks>
    public class ProjectItemClipboardList : ProjectItemClipboardListBase
    {
      /// <summary>List of project items to serialize/deserialize</summary>
      private List<IProjectItem> _projectItems;

      private ProjectItemClipboardList()
      {
      }

      public ProjectItemClipboardList(IEnumerable<Altaxo.Main.IProjectItem> projectItems, string baseFolder)
      {
        BaseFolder = baseFolder;
        _projectItems = new List<IProjectItem>(projectItems);
      }

      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ProjectItemClipboardList), 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          ProjectItemClipboardList s = (ProjectItemClipboardList)obj;

          info.AddValue("BaseFolder", s.BaseFolder);
          info.AddValue("RelocateReferences", s.RelocateReferences);
          info.AddValue("TryToKeepInternalReferences", s.TryToKeepInternalReferences);
          info.CreateArray("Items", s._projectItems.Count);
          foreach (var item in s._projectItems)
            info.AddValue("e", item);
          info.CommitArray();
        }

        public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
        {
          ProjectItemClipboardList s = null != o ? (ProjectItemClipboardList)o : new ProjectItemClipboardList();

          s.BaseFolder = info.GetString("BaseFolder");

          s.RelocateReferences = info.GetNullableBoolean("RelocateReferences");
          s.TryToKeepInternalReferences = info.GetNullableBoolean("TryToKeepInternalReferences");

          int count = info.OpenArray("Items");
          s._projectItems = new List<IProjectItem>();
          for (int i = 0; i < count; ++i)
          {
            s._projectItems.Add((IProjectItem)info.GetValue("e", s));
          }
          info.CloseArray(count);

          return s;
        }
      }

      public IEnumerable<IProjectItem> ProjectItems
      {
        get { return _projectItems.OfType<IProjectItem>(); }
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// There are two groups of possible options:
    /// <para>Group1: where are the items included?</para>
    /// <para>Possibilities: (i) with absolute names, i.e. the item name remains as is. (ii) relative to the project folder where it is included and where it was copied from.</para>
    /// <para>Group2: what should be happen with the references (for instance in plot items to the table columns)</para>
    /// <para>Possibilities: (i) nothing, they remain as is (ii) they will be relocated taking the source folder and destination folder into account, or (iii) same as (ii) but also
    /// taking the renaming of the copied tables into account, if there is a name conflict with an already existing worksheet for instance.</para>
    /// <para>The first group is controlled by the <c>baseFolder</c> parameters during creation of the object (source base folder) and during pasting of the items (destination base folder).
    /// If source base folder or destination base folder is null, the items will be included with absolute names. This is usually the case when copying or pasting from/into the AllGraph, AllWorksheet or
    /// AllProject items view. If both source base folder and destination base folder have a value, then the items are included relative to both folders.</para>
    /// <para>
    /// </para>
    /// </remarks>
    public class ProjectItemReferenceClipboardList : ProjectItemClipboardListBase
    {
      /// <summary>List of project items to serialize/deserialize</summary>
      private List<DocNodeProxy> _projectItems;

      private ProjectItemReferenceClipboardList()
      {
      }

      public ProjectItemReferenceClipboardList(IEnumerable<DocNodeProxy> projectItemReferences, string baseFolder)
      {
        BaseFolder = baseFolder;
        _projectItems = new List<DocNodeProxy>(projectItemReferences);
      }

      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ProjectItemReferenceClipboardList), 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          var s = (ProjectItemReferenceClipboardList)obj;

          info.AddValue("BaseFolder", s.BaseFolder);
          info.AddValue("RelocateReferences", s.RelocateReferences);
          info.AddValue("TryToKeepInternalReferences", s.TryToKeepInternalReferences);
          info.CreateArray("Items", s._projectItems.Count);
          foreach (var item in s._projectItems)
            info.AddValue("e", item);
          info.CommitArray();
        }

        public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
        {
          var s = null != o ? (ProjectItemReferenceClipboardList)o : new ProjectItemReferenceClipboardList();

          s.BaseFolder = info.GetString("BaseFolder");

          s.RelocateReferences = info.GetNullableBoolean("RelocateReferences");
          s.TryToKeepInternalReferences = info.GetNullableBoolean("TryToKeepInternalReferences");

          int count = info.OpenArray("Items");
          s._projectItems = new List<DocNodeProxy>();
          for (int i = 0; i < count; ++i)
          {
            s._projectItems.Add((DocNodeProxy)info.GetValue("e", s));
          }
          info.CloseArray(count);

          return s;
        }
      }

      public IEnumerable<DocNodeProxy> ProjectItemReferences
      {
        get { return _projectItems.OfType<DocNodeProxy>(); }
      }
    }

    /// <summary>
    /// Identifier used for putting a list of project items on the clipboard.
    /// </summary>
    public const string ClipboardFormat_ListOfProjectItems = "Altaxo.Main.ProjectItems.AsXml";

    /// <summary>Copies the items to the clipboard. In principle, this can be all items that have Altaxo's XML serialization support. But the corresponding clipboard paste operation
    /// is only supported for main project items (DataTables, Graphs).</summary>
    /// <param name="items">The items.</param>
    /// <param name="baseFolder">The base folder of all provided items (if one exists). This determines in which folder the items are pasted in the subsequent paste operation.</param>
    public static void CopyItemsToClipboard(IEnumerable<IProjectItem> items, string baseFolder)
    {
      Altaxo.Serialization.Clipboard.ClipboardSerialization.PutObjectToClipboard(ClipboardFormat_ListOfProjectItems, new ProjectItemClipboardList(items, baseFolder));
    }

    public static bool CanPasteItemsFromClipboard()
    {
      return Altaxo.Serialization.Clipboard.ClipboardSerialization.IsClipboardFormatAvailable(ClipboardFormat_ListOfProjectItems);
    }

    public static void PasteItemsFromClipboard(string baseFolder)
    {
      var list = Altaxo.Serialization.Clipboard.ClipboardSerialization.GetObjectFromClipboard<ProjectItemClipboardList>(ClipboardFormat_ListOfProjectItems);
      PasteItems(baseFolder, list);
    }

    public static void PasteItems(string targetFolder, ProjectItemClipboardList list)
    {
      // first we have to make sure that list has values set for TryToKeepInternalReferences and RelocateReferences -- otherwise we have to show a dialog
      if (list.TryToKeepInternalReferences == null || list.RelocateReferences == null)
      {
        ProjectItemsPasteOptions options = new ProjectItemsPasteOptions { TryToKeepInternalReferences = list.TryToKeepInternalReferences, RelocateReferences = list.RelocateReferences };

        if (!Current.Gui.ShowDialog(ref options, "Paste options", false))
          return;

        list.TryToKeepInternalReferences = options.TryToKeepInternalReferences;
        list.RelocateReferences = options.RelocateReferences;
      }

      if (!(list.TryToKeepInternalReferences.HasValue))
        throw new InvalidProgramException();
      if (!(list.RelocateReferences.HasValue))
        throw new InvalidProgramException();

      var relocationData = new DocNodePathReplacementOptions();

      foreach (IProjectItem item in list.ProjectItems)
      {
        var oldName = item.Name;
        var newName = GetRelocatedName(oldName, list.BaseFolder, targetFolder);
        var oldPath = Current.Project.GetDocumentPathForProjectItem(item);

        item.Name = newName;
        Current.Project.AddItem(item);
        if (list.TryToKeepInternalReferences.Value)
        {
          var newPath = Current.Project.GetDocumentPathForProjectItem(item);
          relocationData.AddProjectItemReplacement(oldPath, newPath); // when trying to keep the references, we use the name the table gets after added to the collection (it can have changed during this operation).
        }
      }

      if (list.RelocateReferences.Value && targetFolder != null)
      {
        string sourceFolder = list.BaseFolder ?? ProjectFolder.RootFolderName;
        relocationData.AddPathReplacementsForAllProjectItemTypes(sourceFolder, targetFolder);
      }

      if (list.TryToKeepInternalReferences.Value || list.RelocateReferences.Value)
      {
        foreach (IProjectItem item in list.ProjectItems)
          item.VisitDocumentReferences(relocationData.Visit);
      }
    }

    private static string GetRelocatedName(string name, string oldBaseFolder, string newBaseFolder)
    {
      string result = name;

      if ((null != oldBaseFolder) && name.StartsWith(oldBaseFolder))
      {
        result = name.Substring(oldBaseFolder.Length);
        result = newBaseFolder + result;
      }
      return result;
    }

    #endregion Clipboard commands

    #region Rename Commands

    /// <summary>
    /// Shows a dialog to rename the table.
    /// </summary>
    /// <param name="projectItem">The project item to rename.</param>
    public static void ShowRenameDialog(IProjectItem projectItem)
    {
      string projectItemTypeName = projectItem.GetType().Name;

      TextValueInputController tvctrl = new TextValueInputController(projectItem.Name, string.Format("Enter a name for the {0}:", projectItem));
      tvctrl.Validator = new RenameValidator(projectItem, projectItemTypeName);
      if (Current.Gui.ShowDialog(tvctrl, string.Format("Rename {0}", projectItemTypeName), false))
        projectItem.Name = tvctrl.InputText.Trim();
    }

    private class RenameValidator : TextValueInputController.NonEmptyStringValidator
    {
      private IProjectItem _projectItem;
      private string _projectItemTypeName;

      public RenameValidator(IProjectItem projectItem, string projectItemTypeName)
        : base(string.Format("The {0} name must not be empty! Please enter a valid name.", projectItemTypeName))
      {
        if (null == projectItem)
          throw new ArgumentNullException(nameof(projectItem));

        _projectItem = projectItem;
        _projectItemTypeName = projectItemTypeName;
        if (null == projectItemTypeName)
          _projectItemTypeName = _projectItem.GetType().Name;
      }

      public override string Validate(string projectItemName)
      {
        string err = base.Validate(projectItemName);
        if (null != err)
          return err;

        if (_projectItem.Name == projectItemName)
          return null; // name is the same => thus no renaming neccessary

        var collection = (IProjectItemCollection)Main.AbsoluteDocumentPath.GetRootNodeImplementing(_projectItem, typeof(IProjectItemCollection));

        if (collection == null)
          return null; // if there is no parent data set we can enter anything

        if (collection.ContainsAnyName(projectItemName))
          return string.Format("This {0} name already exists, please choose another name!", _projectItemTypeName);
        else
          return null;
      }
    }

    #endregion Rename Commands
  }
}
