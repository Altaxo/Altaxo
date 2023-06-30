#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#nullable disable warnings
using System;
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Main;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
  /// <summary>
  /// Abstracts the handling of the list items that are currently shown.
  /// </summary>
  public abstract class AbstractItemHandler
  {
    protected SelectableListNodeList _list;

    protected event Action<SelectableListNodeList> _listChange;

    protected static Dictionary<Type, ProjectBrowseItemImage> _projectItemTypesToImage;

    static AbstractItemHandler()
    {
      _projectItemTypesToImage = new Dictionary<Type, ProjectBrowseItemImage>()
      {
        [typeof(Altaxo.Data.DataTable)] = ProjectBrowseItemImage.Worksheet,
        [typeof(Altaxo.Graph.Gdi.GraphDocument)] = ProjectBrowseItemImage.Graph,
        [typeof(Altaxo.Graph.Graph3D.GraphDocument)] = ProjectBrowseItemImage.Graph,
        [typeof(Altaxo.Text.TextDocument)] = ProjectBrowseItemImage.TextDocument,
        [typeof(Altaxo.Main.Properties.ProjectFolderPropertyDocument)] = ProjectBrowseItemImage.PropertyBag,
      };
    }

    /// <summary>
    /// Signals that the list has changed, so the view can update the list. When the first receiver registers for
    /// the event, the function <see cref="BeginTracking"/> will be called. If the last receiver unregisters for
    /// the event, the function <see cref="EndTracking"/> will be called.
    /// </summary>
    public event Action<SelectableListNodeList> ListChange
    {
      add
      {
        bool wasEmpty = _listChange is null;
        _listChange += value;
        if (wasEmpty && _listChange is not null)
          BeginTracking();
      }
      remove
      {
        bool wasEmpty = _listChange is null;
        _listChange -= value;
        if (!wasEmpty && _listChange is null)
          EndTracking();
      }
    }

    /// <summary>
    /// Fires the <see cref="ListChange"/> event.
    /// </summary>
    protected virtual void OnListChange()
    {
      if (_listChange is not null)
        _listChange(_list);
    }

    /// <summary>
    /// Fills the list with items.
    /// </summary>
    /// <returns>The list of items that are shown in the listbox.</returns>
    public abstract SelectableListNodeList GetItemList();

    /// <summary>
    /// Begins monitoring of changes that can affect the items in the list.
    /// </summary>
    public abstract void BeginTracking();

    /// <summary>
    /// Ends monitoring of changes that can affect the items in the list.
    /// </summary>
    public abstract void EndTracking();

    public static BrowserListItem GetBrowserListItem(IProjectItem t, bool showFullName)
    {
      var name = showFullName ? t.Name : ProjectFolder.GetNamePart(t.Name);
      if (t is Altaxo.Main.Properties.ProjectFolderPropertyDocument)
        name += "FolderProperties";
      else if (t is Altaxo.Text.TextDocument && Altaxo.Main.ProjectFolder.IsValidFolderName(name))
        name += "FolderNotes";

      if (!_projectItemTypesToImage.TryGetValue(t.GetType(), out var image))
        image = ProjectBrowseItemImage.OpenFolder;

      return new BrowserListItem(name, showFullName, t, false) { Image = image, CreationDate = t.CreationTimeUtc, ChangeDate = t.LastChangeTimeUtc };
    }

    public static BrowserListItem GetBrowserListItem(string folder)
    {
      return new BrowserListItem(ProjectFolder.ConvertFolderNameToDisplayFolderLastPart(folder), false, new ProjectFolder(folder), false) { Image = ProjectBrowseItemImage.OpenFolder };
    }

    public static BrowserListItem GetBrowserListItemFromObject(object t, bool showFullName)
    {
      if (t is IProjectItem projectItem)
        return GetBrowserListItem(projectItem, showFullName);
      else if (t is string folder)
        return GetBrowserListItem(folder);
      else if (t is not null)
        throw new ApplicationException("Unknown type to list: " + t.GetType().ToString());
      else
        throw new ArgumentNullException("Object to list is null");
    }
  }

  /// <summary>
  /// Shows all available items (tables, graphs, ..) in the project.
  /// </summary>
  public class ProjectAllItemHandler : AbstractItemHandler
  {
    /// <summary>
    /// Fills the list with items.
    /// </summary>
    /// <returns>The list of items that are shown in the listbox.</returns>
    public override SelectableListNodeList GetItemList()
    {
      _list = new SelectableListNodeList();

      foreach (var coll in Current.Project.ProjectItemCollections)
      {
        foreach (IProjectItem item in coll.ProjectItems)
        {
          _list.Add(GetBrowserListItem(item, true));
        }
      }
      return _list;
    }

    /// <summary>
    /// Starts monitoring of the table and graph collection.
    /// </summary>
    public override void BeginTracking()
    {
      GetItemList();

      foreach (var coll in Current.Project.ProjectItemCollections)
        coll.CollectionChanged += EhCollectionChanged;

      OnListChange();
    }

    /// <summary>
    /// Ends monitoring of the table and graph collection.
    /// </summary>
    public override void EndTracking()
    {
      foreach (var coll in Current.Project.ProjectItemCollections)
        coll.CollectionChanged -= EhCollectionChanged;
    }

    private void EhCollectionChanged(object sender, Altaxo.Main.NamedObjectCollectionChangedEventArgs e)
    {
      GetItemList();
      OnListChange();
    }
  }

  /// <summary>
  /// Shows all tables in the project.
  /// </summary>
  public class AllWorksheetHandler : AbstractItemHandler
  {
    /// <summary>
    /// Fills the list with all tables in the project.
    /// </summary>
    /// <returns>List of all tables.</returns>
    public override SelectableListNodeList GetItemList()
    {
      _list = new SelectableListNodeList();
      foreach (var t in Current.Project.DataTableCollection)
        _list.Add(GetBrowserListItem(t, true));

      return _list;
    }

    /// <summary>
    /// Starts monitoring of the table collection.
    /// </summary>
    public override void BeginTracking()
    {
      GetItemList();
      Current.Project.DataTableCollection.CollectionChanged += EhCollectionChanged;
      OnListChange();
    }

    /// <summary>
    /// Ends monitoring of the table collection.
    /// </summary>
    public override void EndTracking()
    {
      Current.Project.DataTableCollection.CollectionChanged -= EhCollectionChanged;
    }

    private void EhCollectionChanged(object sender, Altaxo.Main.NamedObjectCollectionChangedEventArgs e)
    {
      GetItemList();
      OnListChange();
    }
  }

  /// <summary>
  /// Shows all graphs in the project.
  /// </summary>
  public class AllGraphHandler : AbstractItemHandler
  {
    public override SelectableListNodeList GetItemList()
    {
      _list = new SelectableListNodeList();
      foreach (Altaxo.Graph.Gdi.GraphDocument t in Current.Project.GraphDocumentCollection)
        _list.Add(GetBrowserListItem(t, true));
      foreach (Altaxo.Graph.Graph3D.GraphDocument t in Current.Project.Graph3DDocumentCollection)
        _list.Add(GetBrowserListItem(t, true));

      return _list;
    }

    /// <summary>
    /// Starts monitoring of the graph collection.
    /// </summary>
    public override void BeginTracking()
    {
      GetItemList();
      Current.Project.GraphDocumentCollection.CollectionChanged += EhCollectionChanged;
      Current.Project.Graph3DDocumentCollection.CollectionChanged += EhCollectionChanged;
      OnListChange();
    }

    /// <summary>
    /// Ends monitoring of the graph collection.
    /// </summary>
    public override void EndTracking()
    {
      Current.Project.GraphDocumentCollection.CollectionChanged -= EhCollectionChanged;
      Current.Project.Graph3DDocumentCollection.CollectionChanged -= EhCollectionChanged;
    }

    private void EhCollectionChanged(object sender, Altaxo.Main.NamedObjectCollectionChangedEventArgs e)
    {
      GetItemList();
      OnListChange();
    }
  }

  /// <summary>
  /// Shows all text documents in the project.
  /// </summary>
  public class AllTextsHandler : AbstractItemHandler
  {
    public override SelectableListNodeList GetItemList()
    {
      _list = new SelectableListNodeList();
      foreach (var t in Current.Project.TextDocumentCollection)
      {
        _list.Add(GetBrowserListItem(t, true));
      }
      return _list;
    }

    /// <summary>
    /// Starts monitoring of the graph collection.
    /// </summary>
    public override void BeginTracking()
    {
      GetItemList();
      Current.Project.TextDocumentCollection.CollectionChanged += EhCollectionChanged;
      OnListChange();
    }

    /// <summary>
    /// Ends monitoring of the graph collection.
    /// </summary>
    public override void EndTracking()
    {
      Current.Project.TextDocumentCollection.CollectionChanged -= EhCollectionChanged;
    }

    private void EhCollectionChanged(object sender, Altaxo.Main.NamedObjectCollectionChangedEventArgs e)
    {
      GetItemList();
      OnListChange();
    }
  }

  /// <summary>
  /// Shows the items in a specified project folder.
  /// </summary>
  public class SpecificProjectFolderHandler : AbstractItemHandler
  {
    private string _folderName;

    /// <summary>
    /// Creates the handler.
    /// </summary>
    /// <param name="folder">The project folder for which to show the items.</param>
    public SpecificProjectFolderHandler(string folder)
    {
      _folderName = folder;
    }

    /// <summary>Gets the current project folder (or null if this list has no current project folder).</summary>
    public string CurrentProjectFolder { get { return _folderName; } }

    /// <summary>
    /// Fills the list with all items (tables, graphs, and subfolders) of the current project folder.
    /// </summary>
    /// <returns>List of items.</returns>
    public override SelectableListNodeList GetItemList()
    {
      _list = new SelectableListNodeList();
      if (!Current.Project.Folders.ContainsFolder(_folderName))
        return _list;

      var subfolderList = Current.Project.Folders.GetSubfoldersAsStringList(_folderName, false);
      //subfolderList.Sort();
      foreach (var o in subfolderList)
      {
        _list.Add(GetBrowserListItem(o));
      }

      var itemList = Current.Project.Folders.GetItemsInFolder(_folderName);
      //itemList.Sort(CompareItemsByName);
      foreach (var o in itemList)
      {
        _list.Add(GetBrowserListItemFromObject(o, false));
      }

      return _list;
    }

    /// <summary>
    /// Starts monitoring of item changes in the current project folder.
    /// </summary>
    public override void BeginTracking()
    {
      GetItemList();
      Current.Project.Folders.CollectionChanged += EhCollectionChanged;
      OnListChange();
    }

    /// <summary>
    /// Ends monitoring of item changes in the current project folder.
    /// </summary>
    public override void EndTracking()
    {
      Current.Project.Folders.CollectionChanged += EhCollectionChanged;
    }

    private void EhCollectionChanged(object sender, Altaxo.Main.NamedObjectCollectionChangedEventArgs e)
    {
      GetItemList();
      OnListChange();
    }
  }
}
