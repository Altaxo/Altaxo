﻿#region Copyright

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
using System.Linq;
using System.Text;
using Altaxo.Gui.Workbench;

namespace Altaxo.Gui.Pads.FileBrowser
{
  public interface IFileBrowserView : IFileTreeView, IFileListView
  {
  }

  [ExpectedTypeOfView(typeof(IFileBrowserView))]
  public class FileBrowserController : AbstractPadContent
  {
    private FileSystemTreeController _treeController;
    private FileListController _listController;

    private IFileBrowserView _view;

    public FileBrowserController()
    {
      _treeController = new FileSystemTreeController();
      _listController = new FileListController();

      _treeController.SelectedPathChanged += EhTreeController_SelectedPathChanged;
    }

    private void AttachView()
    {
      _treeController.ViewObject = _view;
      _listController.ViewObject = _view;
    }

    private void DetachView()
    {
      _treeController.ViewObject = null;
      _listController.ViewObject = null;
    }

    public override object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (!object.ReferenceEquals(_view, value))
        {
          if (_view is not null)
          {
            DetachView();
          }

          _view = value as IFileBrowserView;

          if (_view is not null)
          {
            AttachView();
          }
        }
      }
    }

    public override object InitiallyFocusedControl
    {
      get
      {
        object result = null;
        try
        {
          dynamic d = _view;
          result = d?.InitiallyFocusedControl;
        }
        catch (Exception)
        {
        }
        return result;
      }
    }

    /// <summary>
    /// Returns null (because the model would be the whole file system).
    /// </summary>
    /// <value>
    /// The model object.
    /// </value>
    public override object ModelObject
    {
      get
      {
        return null;
      }
    }

    public override void Dispose()
    {
      ViewObject = null;
    }

    private void EhTreeController_SelectedPathChanged(string path)
    {
      _listController.ShowFilesInPath(path);
    }
  }
}
