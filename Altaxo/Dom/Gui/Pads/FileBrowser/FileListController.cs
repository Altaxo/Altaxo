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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Pads.FileBrowser
{
  public interface IFileListView
  {
    void Initialize_FileListColumnNames(ICollection<string> names);

    void Initialize_FileList(SelectableListNodeList files);

    /// <summary>
    /// Occurs when the user activates the selected items (either by double-clicking on it, or by pressing Enter).
    /// </summary>
    event Action SelectedItemsActivated;
  }

  public class FileListController
  {
    #region FileItem

    private class FileListItem : SelectableListNode
    {
      private string _text1;
      private string _text2;

      public string FullName { get { return (string)Tag; } }

      public FileListItem(string fullPath)
        : base(Path.GetFileName(fullPath), fullPath, false)
      {
        InternalUpdate(false);
      }

      public void Rename(string fullPath)
      {
        Tag = fullPath;
        Text = Path.GetFileName(fullPath);
        InternalUpdate(true);
      }

      public void Update()
      {
        InternalUpdate(true);
      }

      private void InternalUpdate(bool triggerEvents)
      {
        var info = new FileInfo(FullName);

        try
        {
          _text1 = Math.Round((double)info.Length / 1024).ToString() + " KB";
          _text2 = info.LastWriteTime.ToString();
        }
        catch (IOException)
        {
          // ignore IO errors
        }
        if (triggerEvents)
        {
          OnPropertyChanged("Text1");
          OnPropertyChanged("Text2");
        }
      }

      public override string Text1
      {
        get
        {
          return _text1;
        }
      }

      public override string Text2
      {
        get
        {
          return _text2;
        }
      }

      public void SetText1(string value)
      {
        _text1 = value;
        OnPropertyChanged("Text1");
      }

      public void SetText2(string value)
      {
        _text2 = value;
        OnPropertyChanged("Text2");
      }
    }

    #endregion FileItem

    private IFileListView _view;
    private FileSystemWatcher watcher;
    private List<string> _columnNames;

    private SelectableListNodeList _fileList = new SelectableListNodeList();

    public FileListController()
    {
      Initialize(true);
    }

    private void Initialize(bool initData)
    {
      if (initData)
      {
        _columnNames = new List<string>
        {
          Current.ResourceService.GetString("CompilerResultView.FileText"),
          Current.ResourceService.GetString("MainWindow.Windows.FileScout.Size"),
          Current.ResourceService.GetString("MainWindow.Windows.FileScout.LastModified")
        };

        try
        {
          watcher = new FileSystemWatcher();
        }
        catch { }

        if (watcher is not null)
        {
          watcher.NotifyFilter = NotifyFilters.FileName;
          watcher.EnableRaisingEvents = false;

          watcher.Renamed += new RenamedEventHandler(fileRenamed);
          watcher.Deleted += new FileSystemEventHandler(fileDeleted);
          watcher.Created += new FileSystemEventHandler(fileCreated);
          watcher.Changed += new FileSystemEventHandler(fileChanged);
        }
      }

      if (_view is not null)
      {
        _view.Initialize_FileListColumnNames(_columnNames);
        _view.Initialize_FileList(_fileList);
      }
    }

    #region FileSystemWatcher handlers

    private void fileDeleted(object sender, FileSystemEventArgs e)
    {
      Action method = delegate
      {
        for (int i = _fileList.Count - 1; i >= 0; --i)
        {
          var fileItem = (FileListItem)_fileList[i];
          if (fileItem.FullName.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase))
          {
            _fileList.Remove(fileItem);
            break;
          }
        }
      };

      Current.Dispatcher.InvokeIfRequired(method);
    }

    private void fileChanged(object sender, FileSystemEventArgs e)
    {
      Action method = delegate
      {
        foreach (FileListItem fileItem in _fileList)
        {
          if (fileItem.FullName.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase))
          {
            fileItem.Update();
            break;
          }
        }
      };
      Current.Dispatcher.InvokeIfRequired(method);
    }

    private void fileCreated(object sender, FileSystemEventArgs e)
    {
      Action method = delegate
      {
        var info = new FileInfo(e.FullPath);
        _fileList.Add(new FileListItem(e.FullPath));
      };

      Current.Dispatcher.InvokeIfRequired(method);
    }

    private void fileRenamed(object sender, RenamedEventArgs e)
    {
      Action method = delegate
      {
        foreach (FileListItem fileItem in _fileList)
        {
          if (fileItem.FullName.Equals(e.OldFullPath, StringComparison.OrdinalIgnoreCase))
          {
            fileItem.Rename(e.FullPath);
            break;
          }
        }
      };

      Current.Dispatcher.InvokeIfRequired(method);
    }

    #endregion FileSystemWatcher handlers

    #region User handlers

    public void EhView_ActivateSelectedItems()
    {
      foreach (FileListItem item in _fileList.Where(x => x.IsSelected))
      {
        var fileExtension = Path.GetExtension(item.FullName).ToUpperInvariant();

        if (Current.IProjectService.IsProjectFileExtension(fileExtension))
        {
          Current.IProjectService.OpenProject(new FileName(item.FullName), showUserInteraction: true);
          break;
        }

        switch (fileExtension)
        {
          case ".SPC":
            if (Current.Workbench.ActiveViewContent is Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
            {
              string[] files = new string[] { item.FullName };
              Altaxo.Serialization.Galactic.Import.ImportSpcFiles(files, ctrl.DataTable);
            }
            break;

          case ".DAT":
          case ".TXT":
          case ".CSV":
            {
              Altaxo.Serialization.Ascii.AsciiImporter.ImportFilesIntoSeparateNewTables(Altaxo.Main.ProjectFolder.RootFolder, new string[] { item.FullName }, true, true);
            }
            break;

          default:
            Current.Gui.InfoMessageBox(string.Format("No idea how to open file {0}", item.FullName), "For your information");
            break;
        }
      }
    }

    public void ShowFilesInPath(string path)
    {
      string[] files;
      _fileList.Clear();

      try
      {
        if (Directory.Exists(path))
        {
          files = Directory.GetFiles(path);
        }
        else
        {
          return;
        }
      }
      catch (Exception)
      {
        return;
      }

      watcher.Path = path;
      watcher.EnableRaisingEvents = true;

      foreach (string file in files)
      {
        _fileList.Add(new FileListItem(file));
      }
    }

    #endregion User handlers

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is not null)
        {
          _view.SelectedItemsActivated -= EhView_ActivateSelectedItems;
        }

        _view = value as IFileListView;

        if (_view is not null)
        {
          Initialize(false);
          _view.SelectedItemsActivated += EhView_ActivateSelectedItems;
        }
      }
    }
  }
}
