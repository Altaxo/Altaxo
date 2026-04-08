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
using Altaxo.AddInItems;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
  /// <summary>
  /// Disables automatic showing of items when selecting a tree node.
  /// </summary>
  public class CmdViewOnSelectOff : ProjectBrowseControllerCommand, ICheckableMenuCommand
  {
    /// <summary>
    /// Occurs when the checked state changes.
    /// </summary>
    public event EventHandler IsCheckedChanged;

    private ViewOnSelect _lastKnownValue;

    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      _lastKnownValue = ViewOnSelect.Off;
      Ctrl.ViewOnSelectTreeNode = _lastKnownValue;
      if (IsCheckedChanged is not null)
        IsCheckedChanged(this, EventArgs.Empty);
    }

    #region ICheckableMenuCommand Members

    /// <inheritdoc/>
    public bool IsChecked(object parameter)
    {
      return ((ProjectBrowseController)parameter).ViewOnSelectTreeNode == ViewOnSelect.Off;
    }

    #endregion ICheckableMenuCommand Members

    #region IMenuCommand Members

    /// <summary>
    /// Gets or sets a value indicating whether the command is enabled.
    /// </summary>
    public bool IsEnabled
    {
      get
      {
        if (Ctrl.ViewOnSelectTreeNode != _lastKnownValue && IsCheckedChanged is not null)
        {
          IsCheckedChanged(this, EventArgs.Empty);
          _lastKnownValue = Ctrl.ViewOnSelectTreeNode;
        }

        return true;
      }
      set
      {
      }
    }

    #endregion IMenuCommand Members
  }

  /// <summary>
  /// Shows items in the selected folder when a tree node is selected.
  /// </summary>
  public class CmdViewOnSelectFolderItems : ProjectBrowseControllerCommand, ICheckableMenuCommand
  {
    /// <summary>
    /// Occurs when the checked state changes.
    /// </summary>
    public event EventHandler IsCheckedChanged;

    private ViewOnSelect _lastKnownValue;

    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      _lastKnownValue = ViewOnSelect.ItemsInFolder;
      ctrl.ViewOnSelectTreeNode = _lastKnownValue;
      IsCheckedChanged?.Invoke(this, EventArgs.Empty);
    }

    #region ICheckableMenuCommand Members

    /// <inheritdoc/>
    public bool IsChecked(object parameter)
    {
      return ((ProjectBrowseController)parameter).ViewOnSelectTreeNode == ViewOnSelect.ItemsInFolder;
    }

    #endregion ICheckableMenuCommand Members

    #region IMenuCommand Members

    /// <summary>
    /// Gets or sets a value indicating whether the command is enabled.
    /// </summary>
    public bool IsEnabled
    {
      get
      {
        if (Ctrl.ViewOnSelectTreeNode != _lastKnownValue && IsCheckedChanged is not null)
        {
          IsCheckedChanged(this, EventArgs.Empty);
          _lastKnownValue = Ctrl.ViewOnSelectTreeNode;
        }

        return true;
      }
      set
      {
      }
    }

    #endregion IMenuCommand Members
  }

  /// <summary>
  /// Shows items in the selected folder and subfolders when a tree node is selected.
  /// </summary>
  public class CmdViewOnSelectFolderAndSubfolderItems : ProjectBrowseControllerCommand, ICheckableMenuCommand
  {
    /// <summary>
    /// Occurs when the checked state changes.
    /// </summary>
    public event EventHandler IsCheckedChanged;

    private ViewOnSelect _lastKnownValue;

    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      _lastKnownValue = ViewOnSelect.ItemsInFolderAndSubfolders;
      ctrl.ViewOnSelectTreeNode = _lastKnownValue;
      IsCheckedChanged?.Invoke(this, EventArgs.Empty);
    }

    #region ICheckableMenuCommand Members

    /// <inheritdoc/>
    public bool IsChecked(object parameter)
    {
      return ((ProjectBrowseController)parameter).ViewOnSelectTreeNode == ViewOnSelect.ItemsInFolderAndSubfolders;
    }

    #endregion ICheckableMenuCommand Members

    #region IMenuCommand Members

    /// <summary>
    /// Gets or sets a value indicating whether the command is enabled.
    /// </summary>
    public bool IsEnabled
    {
      get
      {
        if (Ctrl.ViewOnSelectTreeNode != _lastKnownValue && IsCheckedChanged is not null)
        {
          IsCheckedChanged(this, EventArgs.Empty);
          _lastKnownValue = Ctrl.ViewOnSelectTreeNode;
        }

        return true;
      }
      set
      {
      }
    }

    #endregion IMenuCommand Members
  }

  /// <summary>
  /// Deletes the selected folder and its documents.
  /// </summary>
  public class CmdTreeNodeFolderDelete : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      var items = Current.Project.Folders.GetExpandedProjectItemSet(ctrl.GetAllListItems());
      Altaxo.Main.ProjectFolders.DeleteDocuments(items);
    }
  }

  /// <summary>
  /// Renames the selected folder tree node.
  /// </summary>
  public class CmdTreeNodeFolderRename : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.RenameTreeNode();
    }
  }

  /// <summary>
  /// Shows all documents in the selected tree node.
  /// </summary>
  public class CmdTreeNodeShowWindows : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ProjectBrowserExtensions.ShowDocumentsExclusively(ctrl.GetAllListItems());
    }
  }

  /// <summary>
  /// Shows all documents in the selected tree node recursively.
  /// </summary>
  public class CmdTreeNodeShowWindowsRecursively : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ProjectBrowserExtensions.ShowDocumentsExclusively(Current.Project.Folders.GetExpandedProjectItemSet(ctrl.GetAllListItems()));
    }
  }

  /// <summary>
  /// Hides all documents in the selected tree node.
  /// </summary>
  public class CmdTreeNodeHideWindows : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      var list = ctrl.GetAllListItems();
      foreach (var item in list)
        Current.IProjectService.CloseDocumentViews(item);
    }
  }

  /// <summary>
  /// Hides all documents in the selected tree node recursively.
  /// </summary>
  public class CmdTreeNodeHideWindowsRecursively : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      var list = Current.Project.Folders.GetExpandedProjectItemSet(ctrl.GetAllListItems());
      foreach (var item in list)
        Current.IProjectService.CloseDocumentViews(item);
    }
  }

}
