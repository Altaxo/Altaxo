#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Main;

namespace Altaxo.Gui.ProjectBrowser
{
  /// <summary>
  /// Data for copying items to multiple folders.
  /// </summary>
  public class CopyItemsToMultipleFolderData : ICloneable
  {
    /// <summary>
    /// Gets or sets a value indicating whether references should be relocated.
    /// </summary>
    public bool RelocateReferences { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether existing items should be overwritten.
    /// </summary>
    public bool OverwriteExistingItems { get; set; }

    /// <summary>
    /// The folders selected as copy targets.
    /// </summary>
    protected List<string> _foldersToCopyTo = new List<string>();

    /// <summary>
    /// Gets the folders to copy to.
    /// </summary>
    public List<string> FoldersToCopyTo { get { return _foldersToCopyTo; } }

    /// <inheritdoc/>
    public object Clone()
    {
      var result = (CopyItemsToMultipleFolderData)MemberwiseClone();
      result._foldersToCopyTo = new List<string>(FoldersToCopyTo);
      return result;
    }
  }

  /// <summary>
  /// View interface for copying items to multiple folders.
  /// </summary>
  public interface ICopyItemsToMultipleFolderView
  {
    /// <summary>
    /// Initializes the folder tree.
    /// </summary>
    /// <param name="rootNode">The root node.</param>
    void InitializeFolderTree(NGTreeNode rootNode);

    /// <summary>
    /// Gets or sets a value indicating whether references should be relocated.
    /// </summary>
    bool RelocateReferences { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether existing items should be overwritten.
    /// </summary>
    bool OverwriteExistingItems { get; set; }

    /// <summary>
    /// Gets the additional folders, one per line.
    /// </summary>
    string AdditionalFoldersLineByLine { get; }

    /// <summary>
    /// Occurs when the selected folder names should be copied.
    /// </summary>
    event Action CopySelectedFolderNames;

    /// <summary>
    /// Occurs when all folders should be unselected.
    /// </summary>
    event Action UnselectAllFolders;
  }

  /// <summary>
  /// Controller for <see cref="CopyItemsToMultipleFolderData"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(ICopyItemsToMultipleFolderView))]
  [UserControllerForObject(typeof(CopyItemsToMultipleFolderData))]
  public class CopyItemsToMultipleFoldersController : MVCANControllerEditCopyOfDocBase<CopyItemsToMultipleFolderData, ICopyItemsToMultipleFolderView>
  {
    private NGTreeNode _projectFolders;

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _projectFolders = new NGTreeNode("/") { Tag = ProjectFolder.RootFolderName, IsExpanded = true };
        CreateDirectoryNode(ProjectFolder.RootFolderName, _projectFolders);
      }

      if (_view is not null)
      {
        _view.InitializeFolderTree(_projectFolders);
        _view.RelocateReferences = _doc.RelocateReferences;
        _view.OverwriteExistingItems = _doc.OverwriteExistingItems;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      var foldersToCopyTo = new HashSet<string>();

      foldersToCopyTo.AddRange(_projectFolders.TakeFromFirstLeavesToHere(true).Where(node => node.IsSelected).Select(node => (string)node.Tag));
      _doc.OverwriteExistingItems = _view.OverwriteExistingItems;
      _doc.RelocateReferences = _view.RelocateReferences;

      string additionalFoldersLineByLine = _view.AdditionalFoldersLineByLine;
      var additionalFolders = additionalFoldersLineByLine.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

      foreach (var folderRaw in additionalFolders)
      {
        string folderName = folderRaw.Trim();
        if (string.IsNullOrEmpty(folderName))
          continue;
        if (folderName.Last() != ProjectFolder.DirectorySeparatorChar)
          folderName = folderName + ProjectFolder.DirectorySeparatorChar;

        foldersToCopyTo.Add(folderName);
      }

      _doc.FoldersToCopyTo.AddRange(foldersToCopyTo);

      return ApplyEnd(true, disposeController);
    }

    /// <inheritdoc/>
    protected override void AttachView()
    {
      base.AttachView();
      _view.CopySelectedFolderNames += EhCopySelectedFolderNames;
      _view.UnselectAllFolders += EhUnselectAllFolders;
    }

    /// <inheritdoc/>
    protected override void DetachView()
    {
      _view.CopySelectedFolderNames -= EhCopySelectedFolderNames;
      _view.UnselectAllFolders -= EhUnselectAllFolders;
      base.DetachView();
    }

    private void EhCopySelectedFolderNames()
    {
      var foldersToCopyTo = new List<string>();
      foldersToCopyTo.AddRange(_projectFolders.TakeFromFirstLeavesToHere(true).Where(node => node.IsSelected).Select(node => (string)node.Tag));
      var dao = Current.Gui.GetNewClipboardDataObject();
      dao.SetData("Text", string.Join("\r\n", foldersToCopyTo));
      Current.Gui.SetClipboardDataObject(dao);
    }

    private void EhUnselectAllFolders()
    {
      _projectFolders.TakeFromFirstLeavesToHere(true).ForEachDo(x => x.IsSelected = false);
    }

    private void CreateDirectoryNode(string dir, NGTreeNode node)
    {
      var subfolders = Current.Project.Folders.GetSubfoldersAsStringList(dir, false);
      subfolders.Sort();
      foreach (var subfolder in subfolders)
      {
        var subnode = new NGTreeNode(ProjectFolder.ConvertFolderNameToDisplayFolderLastPart(subfolder))
        {
          Tag = subfolder,
          IsExpanded = true
        };

        node.Nodes.Add(subnode);
        CreateDirectoryNode(subfolder, subnode);
      }
    }
  }
}
