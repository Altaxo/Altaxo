﻿#region Copyright

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
  public partial class ProjectBrowseControl : UserControl, IProjectBrowseView
  {
    public class TreeView_DragHandler : IDragSource
    {
      private ProjectBrowseControl _projectBrowseControl;

      public TreeView_DragHandler(ProjectBrowseControl ctrl)
      {
        _projectBrowseControl = ctrl;
      }

      public void StartDrag(IDragInfo dragInfo)
      {
        _projectBrowseControl._controller.FolderTree_StartDrag(out var dao, out var canCopy, out var canMove);

        dragInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(canCopy, canMove);

        if (dao is not null)
          dragInfo.DataObject = GuiHelper.ToWpf(dao);
      }

      public bool CanStartDrag(IDragInfo dragInfo)
      {
        return _projectBrowseControl._controller.FolderTree_CanStartDrag();
      }

      public void Dropped(IDropInfo dropInfo, DragDropEffects effects)
      {
        GuiHelper.ConvertDragDropEffectToCopyMove(effects, out var isCopy, out var isMove);

        _projectBrowseControl._controller.FolderTree_DragEnded(isCopy, isMove);
      }

      public void DragCancelled()
      {
        _projectBrowseControl._controller.FolderTree_DragCancelled();
      }
    }
  }
}
