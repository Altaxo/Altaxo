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
using System.Linq;
using System.Text;
using Altaxo.Gui.Workbench;

namespace Altaxo.Gui.Pads.FileBrowser
{
  /// <summary>
  /// Defines a combined view for the file system tree and file list.
  /// </summary>
  public interface IFileBrowserView : IFileTreeView, IFileListView
  {
  }

  /// <summary>
  /// Controls the file browser pad.
  /// </summary>
  [ExpectedTypeOfView(typeof(IFileBrowserView))]
  public class FileBrowserController : AbstractPadContent
  {
    private FileSystemTreeController _treeController;
    private FileListController _listController;

    private IFileBrowserView? _view;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileBrowserController"/> class.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the attached file browser view.
    /// </summary>
    public override object? ViewObject
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

    /// <inheritdoc/>
    public override object? InitiallyFocusedControl
    {
      get
      {
        if(
          (_view is { } view) &&
          (view.GetType().GetProperty(nameof(InitiallyFocusedControl)) is { } prop) &&
          (prop.GetGetMethod() is { } getter)
          )
        {
          return getter.Invoke(_view, null);
        }


        return null;
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
        return new object();
      }
    }

    /// <inheritdoc/>
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
