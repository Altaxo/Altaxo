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

using GongSolutions.Wpf.DragDrop;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	public partial class ProjectBrowseControl : UserControl, IProjectBrowseView, ICSharpCode.SharpDevelop.Gui.IPadContent
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
				DragDropEffects resultingEffect;
				if (CanAcceptData(dropInfo, out resultingEffect))
				{
					dropInfo.Effects = resultingEffect;
					dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
				}
			}

			protected bool CanAcceptData(IDropInfo dropInfo, out System.Windows.DragDropEffects resultingEffect)
			{
				bool canCopy, canMove;
				_projectBrowseControl._controller.ListView_DropCanAcceptData(
					dropInfo.Data is System.Windows.IDataObject ? GuiHelper.ToAltaxo((System.Windows.IDataObject)dropInfo.Data) : dropInfo.Data,
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ControlKey),
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ShiftKey),
					out canCopy, out canMove);

				resultingEffect = GuiHelper.ConvertCopyMoveToDragDropEffect(canCopy, canMove);

				return canCopy | canMove;
			}

			public void Drop(IDropInfo dropInfo)
			{
				bool isCopy, isMove;
				_projectBrowseControl._controller.ListView_Drop(
					dropInfo.Data is System.Windows.IDataObject ? GuiHelper.ToAltaxo((System.Windows.IDataObject)dropInfo.Data) : dropInfo.Data,
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ControlKey),
					dropInfo.KeyStates.HasFlag(DragDropKeyStates.ShiftKey),
					out isCopy, out isMove);

				dropInfo.Effects = GuiHelper.ConvertCopyMoveToDragDropEffect(isCopy, isMove); // it is important to get back the resulting effect to dropInfo, because dropInfo informs the drag handler about the resulting effect, which can e.g. delete the items after a move operation
			}
		}
	}
}