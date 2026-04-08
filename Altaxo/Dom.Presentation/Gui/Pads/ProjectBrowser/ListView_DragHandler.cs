#region Copyright

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
    /// <summary>
    /// Gets a service of the specified type.
    /// </summary>
    /// <param name="serviceType">The type of service to retrieve.</param>
    /// <returns>The requested service instance.</returns>
    public object GetService(Type serviceType)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Handles drag operations for the project browser list view.
    /// </summary>
    public class ListView_DragHandler : IDragSource
    {
      private ProjectBrowseControl _projectBrowseControl;

      /// <summary>
      /// Initializes a new instance of the <see cref="ListView_DragHandler"/> class.
      /// </summary>
      /// <param name="ctrl">The owning project browser control.</param>
      public ListView_DragHandler(ProjectBrowseControl ctrl)
      {
        _projectBrowseControl = ctrl;
      }

      /// <summary>
      /// Starts a drag operation from the project browser list view.
      /// </summary>
      /// <param name="dragInfo">Information about the drag operation.</param>
      public void StartDrag(IDragInfo dragInfo)
      {
        _projectBrowseControl._controller.ItemList_StartDrag(out var dao, out var canCopy, out var canMove);

        dragInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(canCopy, canMove);

        if (dao is not null)
          dragInfo.DataObject = GuiHelper.ToWpf(dao);
      }

      /// <summary>
      /// Determines whether a drag operation can start from the list view.
      /// </summary>
      /// <param name="dragInfo">Information about the drag operation.</param>
      /// <returns><c>true</c> if dragging can start; otherwise, <c>false</c>.</returns>
      public bool CanStartDrag(IDragInfo dragInfo)
      {
        return _projectBrowseControl._controller.ItemList_CanStartDrag();
      }

      /// <summary>
      /// Completes a drag operation from the list view.
      /// </summary>
      /// <param name="dropInfo">Information about the drop target.</param>
      /// <param name="effects">The resulting drag-and-drop effects.</param>
      public void Dropped(IDropInfo dropInfo, DragDropEffects effects)
      {
        GuiHelper.ConvertDragDropEffectToCopyMove(effects, out var isCopy, out var isMove);

        _projectBrowseControl._controller.ItemList_DragEnded(isCopy, isMove);
      }

      /// <summary>
      /// Cancels the current drag operation.
      /// </summary>
      public void DragCancelled()
      {
        _projectBrowseControl._controller.ItemList_DragCancelled();
      }
    }
  }
}
