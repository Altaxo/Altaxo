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
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  public interface IMultipleFilesView
  {
    SelectableListNodeList FileNames { set; }

    event Action? BrowseSelectedFileName;

    event Action? DeleteSelectedFileName;

    event Action? MoveUpSelectedFileName;

    event Action? MoveDownSelectedFileName;

    event Action? AddNewFileName;

    event Action? NewFileNameExclusively;

    event Action? SortFileNamesAscending;
  }

  [ExpectedTypeOfView(typeof(IMultipleFilesView))]
  public class MultipleFilesController : MVCANControllerEditImmutableDocBase<IEnumerable<string>, IMultipleFilesView>, IMVCSupportsApplyCallback
  {
    private SelectableListNodeList _fileNames = new SelectableListNodeList();

    public event Action? SuccessfullyApplied;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      if (_doc is null)
        throw NoDocumentException;

      base.Initialize(initData);

      if (initData)
      {
        //_doc.SourceFileName

        _fileNames = new SelectableListNodeList();
        foreach (var fileName in _doc)
        {
          _fileNames.Add(new SelectableListNode(fileName, fileName, false));
        }
      }

      if (_view is not null)
      {
        _view.FileNames = _fileNames;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _fileNames.Select(x => (string)x.Tag!).ToArray();

      SuccessfullyApplied?.Invoke();
      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      if (_view is null)
        throw NoViewException;

      base.AttachView();
      _view.BrowseSelectedFileName += EhBrowseFileName;
      _view.DeleteSelectedFileName += EhDeleteFileName;
      _view.MoveUpSelectedFileName += EhMoveUpFileName;
      _view.MoveDownSelectedFileName += EhMoveDownFileName;
      _view.AddNewFileName += EhAddNewFileName;
      _view.NewFileNameExclusively += EhNewFileNameExclusively;
      _view.SortFileNamesAscending += EhSortFileNamesAscending;
    }

    protected override void DetachView()
    {
      if (_view is null)
        throw NoViewException;

      _view.BrowseSelectedFileName -= EhBrowseFileName;
      _view.DeleteSelectedFileName -= EhDeleteFileName;
      _view.MoveUpSelectedFileName -= EhMoveUpFileName;
      _view.MoveDownSelectedFileName -= EhMoveDownFileName;
      _view.AddNewFileName -= EhAddNewFileName;
      _view.NewFileNameExclusively -= EhNewFileNameExclusively;

      _view.SortFileNamesAscending -= EhSortFileNamesAscending;

      base.DetachView();
    }

    private void EhDeleteFileName()
    {
      _fileNames.RemoveSelectedItems();
    }

    private void EhMoveUpFileName()
    {
      _fileNames.MoveSelectedItemsUp();
      if (_view is { } view)
      {
        view.FileNames = _fileNames;
      }
    }

    private void EhMoveDownFileName()
    {
      _fileNames.MoveSelectedItemsDown();
      if (_view is { } view)
      {
        view.FileNames = _fileNames;
      }
    }

    private (string Filter, string Description)[] _fileFilters = new[] { ("*.csv;*.dat;*.txt", "Text files (*.csv;*.dat;*.txt)"), ("*.*", "All files (*.*)") };

    /// <summary>
    /// Set the file filters.
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
      var node = _fileNames.FirstSelectedNode;
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
      var node = _fileNames.Count > 0 ? _fileNames[_fileNames.Count - 1] : null;
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
          _fileNames.Clear();
        }

        foreach (var filename in options.FileNames)
          _fileNames.Add(new SelectableListNode(filename, filename, false));
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
      var listOfNamesSorted = new List<string>(_fileNames.OrderBy(x => (string)x.Tag!).Select(x => (string)x.Tag!));
      _fileNames.Clear();
      foreach (var name in listOfNamesSorted)
        _fileNames.Add(new SelectableListNode(name, name, false));
    }
  }
}
