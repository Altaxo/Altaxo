#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2019 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// View contract for selecting and ordering multiple file names.
  /// </summary>
  public interface IMultipleFilesView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for editing a collection of file names.
  /// </summary>
  [ExpectedTypeOfView(typeof(IMultipleFilesView))]
  public class MultipleFilesController : MVCANControllerEditImmutableDocBase<IEnumerable<string>, IMultipleFilesView>, IMVCSupportsApplyCallback
  {
    /// <summary>
    /// Occurs after the controller was applied successfully.
    /// </summary>
    public event Action? SuccessfullyApplied;

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// List of file names to be shown in the view. Each node's Tag property should contain the file name as string.
    /// </summary>
    public SelectableListNodeList FileNames
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(FileNames));
        }
      }
    }

    /// <summary>
    /// Occurs when the file names should be browsed.
    /// </summary>
    public ICommand CmdBrowseSelectedFileNames { get => field ??= new RelayCommand(EhBrowseFileName); }

    /// <summary>
    /// Occurs when the selected file name should be deleted.
    /// </summary>
    public ICommand CmdDeleteSelectedFileNames { get => field ??= new RelayCommand(EhDeleteFileName); }

    /// <summary>
    /// Occurs when the selected file name should move up.
    /// </summary>
    public ICommand CmdMoveUpSelectedFileNames { get => field ??= new RelayCommand(EhMoveUpFileName); }

    /// <summary>
    /// Occurs when the selected file name should move down.
    /// </summary>
    public ICommand CmdMoveDownSelectedFileNames { get => field ??= new RelayCommand(EhMoveDownFileName); }

    /// <summary>
    /// Occurs when a new file name should be added.
    /// </summary>
    public ICommand CmdAddNewFileName { get => field ??= new RelayCommand(EhAddNewFileName); }

    /// <summary>
    /// Occurs when a new file name should replace the current list.
    /// </summary>
    public ICommand CmdNewFileNameExclusively { get => field ??= new RelayCommand(EhNewFileNameExclusively); }

    /// <summary>
    /// Occurs when the file names should be sorted in ascending order.
    /// </summary>
    public ICommand CmdSortFileNamesAscending { get => field ??= new RelayCommand(EhSortFileNamesAscending); }

    /// <summary>
    /// Occurs when the file should be shown in the explorer.
    /// </summary>
    public ICommand CmdShowSelectedFileInExplorer { get => field ??= new RelayCommand(EhShowFileInExplorer); }



    #endregion Bindings

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      if (_doc is null)
        throw NoDocumentException;

      base.Initialize(initData);

      if (initData)
      {
        //_doc.SourceFileName

        var fileNames = new SelectableListNodeList();
        foreach (var fileName in _doc)
        {
          fileNames.Add(new SelectableListNode(fileName, fileName, false));
        }
        FileNames = fileNames;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = FileNames.Select(x => (string)x.Tag!).ToArray();

      SuccessfullyApplied?.Invoke();
      return ApplyEnd(true, disposeController);
    }

    private void EhDeleteFileName()
    {
      FileNames.RemoveSelectedItems();
    }

    private void EhMoveUpFileName()
    {
      FileNames.MoveSelectedItemsUp();

    }

    private void EhMoveDownFileName()
    {
      FileNames.MoveSelectedItemsDown();

    }

    private (string Filter, string Description)[] _fileFilters = new[] { ("*.csv;*.dat;*.txt", "Text files (*.csv;*.dat;*.txt)"), ("*.*", "All files (*.*)") };

    /// <summary>
    /// Gets or sets the file filters.
    /// </summary>
    public IEnumerable<(string Filter, string Description)> FileFilters
    {
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(FileFilters));

        _fileFilters = value.ToArray();
      }
    }

    private void EhBrowseFileName()
    {
      var node = FileNames.FirstSelectedNode;
      if (node is null)
        return;

      var options = new OpenFileOptions();
      foreach (var tuple in _fileFilters)
        options.AddFilter(tuple.Filter, tuple.Description);
      options.InitialDirectory = System.IO.Path.GetDirectoryName((string)node.Tag!);

      if (Current.Gui.ShowOpenFileDialog(options))
      {
        node.Tag = node.Text = options.FileName;
      }
    }

    private void EhAddNewFileName(bool addExclusively)
    {
      var node = FileNames.Count > 0 ? FileNames[FileNames.Count - 1] : null;
      var options = new OpenFileOptions();
      foreach (var filter in _fileFilters)
        options.AddFilter(filter.Filter, filter.Description);
      options.AddFilter("*.*", "All files (*.*)");
      if (node is not null)
        options.InitialDirectory = System.IO.Path.GetDirectoryName((string)node.Tag!);
      options.Multiselect = true;
      if (Current.Gui.ShowOpenFileDialog(options))
      {
        if (addExclusively)
        {
          FileNames.Clear();
        }

        foreach (var filename in options.FileNames)
          FileNames.Add(new SelectableListNode(filename, filename, false));
      }
    }

    private void EhAddNewFileName()
    {
      EhAddNewFileName(false);
    }

    private void EhNewFileNameExclusively()
    {
      EhAddNewFileName(true);
    }

    private void EhSortFileNamesAscending()
    {
      var listOfNamesSorted = new List<string>(FileNames.OrderBy(x => (string)x.Tag!).Select(x => (string)x.Tag!));
      FileNames.Clear();
      foreach (var name in listOfNamesSorted)
        FileNames.Add(new SelectableListNode(name, name, false));
    }

    private void EhShowFileInExplorer()
    {
      var selectedNode = FileNames.FirstSelectedNode;

      // Start a new explorer process with the file selected. If this fails, try to start the explorer process with the file's directory.
      if (selectedNode is not null)
      {
        var fileName = selectedNode.Tag as string;

        var processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
          FileName = "explorer.exe",
          Arguments = $"/select,\"{fileName}\"",
          UseShellExecute = true
        };
        System.Diagnostics.Process.Start(processStartInfo);
      }
    }
  }
}
