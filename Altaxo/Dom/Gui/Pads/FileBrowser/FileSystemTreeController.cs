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
  #region Interfaces

  public interface IFileTreeView
  {
    void Initialize_FolderTree(NGTreeNodeCollection nodes);

    event Action<NGTreeNode> FolderTreeNodeSelected;
  }

  #endregion Interfaces

  public class FileSystemTreeController
  {
    #region TreeNode

    private class TreeNode : NGTreeNode
    {
      private int _imageIndex;
      private int _selectedImageIndex;

      public TreeNode(string text, bool lazyLoad)
        : base(lazyLoad)
      {
        _text = text;
      }

      public override int ImageIndex
      {
        get { return _isSelected ? _selectedImageIndex : _imageIndex; }
        set { _imageIndex = value; }
      }

      public override int SelectedImageIndex
      {
        get { return _selectedImageIndex; }
        set { _selectedImageIndex = value; }
      }

      protected override void OnPropertyChanged(string name)
      {
        base.OnPropertyChanged(name);
        if (name == "IsSelected" && _imageIndex != _selectedImageIndex)
          base.OnPropertyChanged("ImageIndex");
      }

      public string FullPath
      {
        get
        {
          if (_tag is DriveInfo)
          {
            return ((DriveInfo)_tag).RootDirectory.FullName;
          }
          else if (_tag is string)
          {
            return (string)_tag;
          }
          else

            return null;
        }
      }

      /// <summary>
      /// Invoked when the child items need to be loaded on demand.
      /// Subclasses can override this to populate the Children collection.
      /// </summary>
      protected override void LoadChildren()
      {
        if (_tag is DriveInfo)
        {
          PopulateSubDirectory(FullPath);
        }
        else if (_tag is string)
        {
          PopulateSubDirectory(FullPath);
        }
      }

      #region Population functions

      private void PopulateSubDirectory(string fullPath)
      {
        if (fullPath[fullPath.Length - 1] != Path.DirectorySeparatorChar)
          fullPath += Path.DirectorySeparatorChar;

        Nodes.Clear();

        string[] directories;
        try
        {
          directories = Directory.GetDirectories(fullPath);
        }
        catch (Exception)
        {
          return;
        }

        foreach (string fulldir in directories)
        {
          FileAttributes attr = FileAttributes.Normal;
          try
          {
            attr = File.GetAttributes(fulldir);
          }
          catch (Exception)
          {
          }
          if ((attr & FileAttributes.Hidden) == 0)
          {
            string dir = System.IO.Path.GetFileName(fulldir);
            var node = new TreeNode(dir, true)
            {
              Tag = fulldir,
              ImageIndex = 0,
              SelectedImageIndex = 1
            };

            Nodes.Add(node);
          }
        }
      }

      #endregion Population functions
    }

    #endregion TreeNode

    private IFileTreeView _view;
    private NGTreeNode _rootNode;
    private NGTreeNodeCollection Nodes;

    public event Action<string> SelectedPathChanged;

    public FileSystemTreeController()
    {
      // Sorted = true;
      _rootNode = new NGTreeNode();
      Nodes = _rootNode.Nodes;

      Initialize(true);
    }

    private void Initialize(bool initData)
    {
      if (initData)
      {
        Nodes.Clear();

        var rootNode = new TreeNode(Path.GetFileName(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)), false)
        {
          ImageIndex = 6,
          SelectedImageIndex = 6,
          Tag = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
        };
        Nodes.Add(rootNode);

        var myFilesNode = new TreeNode(Current.ResourceService.GetString("MainWindow.Windows.FileScout.MyDocuments"), true)
        {
          ImageIndex = 7,
          SelectedImageIndex = 7
        };

        try
        {
          myFilesNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
        catch (Exception)
        {
          myFilesNode.Tag = "C:\\";
        }

        rootNode.Nodes.Add(myFilesNode);

        var computerNode = new TreeNode(Current.ResourceService.GetString("MainWindow.Windows.FileScout.MyComputer"), false)
        {
          ImageIndex = 8,
          SelectedImageIndex = 8
        };
        try
        {
          computerNode.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
        catch (Exception)
        {
          computerNode.Tag = "C:\\";
        }

        rootNode.Nodes.Add(computerNode);

        foreach (DriveInfo info in DriveInfo.GetDrives())
        {
          string text = info.Name.Substring(0, 2);

          switch (info.DriveType)
          {
            case DriveType.Removable:
              text += " (${res:MainWindow.Windows.FileScout.DriveType.Removeable})";
              break;

            case DriveType.Fixed:
              text += " (${res:MainWindow.Windows.FileScout.DriveType.Fixed})";
              break;

            case DriveType.CDRom:
              text += " (${res:MainWindow.Windows.FileScout.DriveType.CD})";
              break;

            case DriveType.Network:
              text += " (${res:MainWindow.Windows.FileScout.DriveType.Remote})";
              break;
          }
          text = StringParser.Parse(text);

          var node = new TreeNode(text, true)
          {
            Tag = info
          };
          switch (info.DriveType)
          {
            case DriveType.Removable:
              node.ImageIndex = node.SelectedImageIndex = 2;
              break;

            case DriveType.Fixed:
              node.ImageIndex = node.SelectedImageIndex = 3;
              break;

            case DriveType.CDRom:
              node.ImageIndex = node.SelectedImageIndex = 4;
              break;

            case DriveType.Network:
              node.ImageIndex = node.SelectedImageIndex = 5;
              break;

            default:
              node.ImageIndex = node.SelectedImageIndex = 3;
              break;
          }

          computerNode.Nodes.Add(node);
        } // foreach drive

        foreach (string directory in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)))
        {
          var node = new TreeNode(Path.GetFileName(directory), true)
          {
            Tag = directory,
            ImageIndex = 0,
            SelectedImageIndex = 1
          };
          rootNode.Nodes.Add(node);
        }

        rootNode.IsExpanded = true;
        computerNode.IsExpanded = true;
      }

      if (_view is not null)
      {
        _view.Initialize_FolderTree(Nodes);
      }
    }

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
          _view.FolderTreeNodeSelected -= EhView_FolderTreeNodeSelected;
        }

        _view = value as IFileTreeView;

        if (_view is not null)
        {
          Initialize(false);
          _view.FolderTreeNodeSelected += EhView_FolderTreeNodeSelected;
        }
      }
    }

    private void EhView_FolderTreeNodeSelected(NGTreeNode obj)
    {
      var node = obj as TreeNode;
      string path = node is null ? null : node.FullPath;

      if (SelectedPathChanged is not null && path is not null)
        SelectedPathChanged(path);
    }
  }
}
