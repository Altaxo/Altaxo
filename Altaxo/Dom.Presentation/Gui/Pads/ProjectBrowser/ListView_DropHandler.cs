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
    public class ListView_DropHandler : IDropTarget
    {
      private ProjectBrowseControl _projectBrowseControl;

      public ListView_DropHandler(ProjectBrowseControl ctrl)
      {
        _projectBrowseControl = ctrl;
      }

      public void DragOver(IDropInfo dropInfo)
      {
        if (CanAcceptData(dropInfo, out var resultingEffect))
        {
          dropInfo.Effects = resultingEffect;
          dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        }
      }

      protected bool CanAcceptData(IDropInfo dropInfo, out System.Windows.DragDropEffects resultingEffect)
      {
        _projectBrowseControl._controller.ListView_DropCanAcceptData(
          dropInfo.Data is System.Windows.IDataObject ? GuiHelper.ToAltaxo((System.Windows.IDataObject)dropInfo.Data) : dropInfo.Data,
          dropInfo.KeyStates.HasFlag(DragDropKeyStates.ControlKey),
          dropInfo.KeyStates.HasFlag(DragDropKeyStates.ShiftKey),
          out var canCopy, out var canMove);

        resultingEffect = GuiHelper.ConvertCopyMoveToDragDropEffect(canCopy, canMove);

        return canCopy | canMove;
      }

      public void Drop(IDropInfo dropInfo)
      {
        _projectBrowseControl._controller.ListView_Drop(
          dropInfo.Data is System.Windows.IDataObject ? GuiHelper.ToAltaxo((System.Windows.IDataObject)dropInfo.Data) : dropInfo.Data,
          dropInfo.KeyStates.HasFlag(DragDropKeyStates.ControlKey),
          dropInfo.KeyStates.HasFlag(DragDropKeyStates.ShiftKey),
          out var isCopy, out var isMove);

        dropInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(isCopy, isMove); // it is important to get back the resulting effect to dropInfo, because dropInfo informs the drag handler about the resulting effect, which can e.g. delete the items after a move operation
      }
    }
  }
}
