#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Services;

using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Services
{
	public class AltaxoToolbarService : AbstractService
	{
		readonly static string toolBarPath     = "/Altaxo/Workbench/ToolBar";
		
		IAddInTreeNode node;
		
		public AltaxoToolbarService()
		{
			this.node  = AddInTreeSingleton.AddInTree.GetTreeNode(toolBarPath);
		}
		
		public CommandBar[] CreateToolbars()
		{
			ToolbarItemCodon[] codons = (ToolbarItemCodon[])(node.BuildChildItems(this)).ToArray(typeof(ToolbarItemCodon));
			
			CommandBar[] toolBars = new CommandBar[codons.Length];
			
			for (int i = 0; i < codons.Length; ++i) 
			{
				toolBars[i] = CreateToolBarFromCodon(WorkbenchSingleton.Workbench, codons[i]);
			}
			return toolBars;
		}
		
		public CommandBar CreateToolBarFromCodon(object owner, ToolbarItemCodon codon)
		{
			CommandBar bar = new CommandBar(CommandBarStyle.ToolBar);
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			foreach (ToolbarItemCodon childCodon in codon.SubItems) 
			{
				CommandBarItem item;
				
				if (childCodon.ToolTip != null) 
				{
					if (childCodon.ToolTip == "-") 
					{
						item = new CommandBarSeparator();
					} 
					else 
					{
						ICommand command = null;
						
						if (childCodon.Class != null) 
						{
						command = 	(ICommand)childCodon.AddIn.CreateObject(childCodon.Class);
						}

						if(command is ICheckableMenuCommand)
						{
							item = new SdMenuCheckBox(childCodon.Conditions, owner, childCodon.ToolTip, (ICheckableMenuCommand)command);
						}
						else
						{
							item = new SdMenuCommand(childCodon.Conditions, owner, childCodon.ToolTip);
							((SdMenuCommand)item).Command = command;
						}
						item.Image = resourceService.GetBitmap(childCodon.Icon);
					}
				} 
				else 
				{
					continue;
				}
					bar.Items.Add(item);
			}
			return bar;
		}
		
		void ToolBarButtonClick(object sender, EventArgs e)
		{
			if (sender is CommandBarItem) 
			{
				((ICommand)((CommandBarItem)sender).Tag).Run();
			}
		}
	}
}
