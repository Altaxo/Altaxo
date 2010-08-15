#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.IO;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Policy;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Commands;

using Altaxo.Gui;
using Altaxo.Gui.SharpDevelop;

namespace Altaxo.Main.Commands // ICSharpCode.SharpDevelop.Commands
{


	public class AutostartCommand : AbstractCommand
	{
		const string workbenchMemento = "SharpDevelop.Workbench.WorkbenchMemento";

		public static void EarlyRun()
		{
			Altaxo.Current.SetPropertyService(new PropertyServiceWrapper());
			Altaxo.Current.SetResourceService(new ResourceServiceWrapper());
			Altaxo.Current.SetProjectService(new Altaxo.Main.ProjectService());
			Altaxo.Current.SetGUIFactoryService(new Altaxo.Gui.WinFormsGuiFactoryService());
			Altaxo.Current.Gui.ContextMenuProvider = ICSharpCode.Core.WinForms.MenuService.CreateContextMenu;
			Altaxo.Current.SetPrintingService(new Altaxo.Main.PrintingService());

			// we construct the main document (for now)
			Altaxo.Current.ProjectService.CurrentOpenProject = new AltaxoDocument();

		}

		public override void Run()
		{
			Altaxo.Current.ProjectService.ProjectChanged += new ProjectEventHandler(Altaxo.Current.Workbench.EhProjectChanged);
			// less important services follow now
			Altaxo.Main.Services.FitFunctionService fitFunctionService = new Altaxo.Main.Services.FitFunctionService();
			Altaxo.Current.SetFitFunctionService(fitFunctionService);
			AddInTree.GetTreeNode("/Altaxo/BuiltinTextures").BuildChildItems(this);
			Altaxo.Main.Services.ParserServiceConnector.Initialize();
		}

		private class ResourceServiceWrapper : Altaxo.Main.Services.IResourceService
		{
			public ResourceServiceWrapper()
			{
			}

			public string GetString(string name)
			{
				return ICSharpCode.Core.ResourceService.GetString(name);
			}

			public System.Drawing.Bitmap GetBitmap(string name)
			{
				return ICSharpCode.Core.WinForms.WinFormsResourceService.GetBitmap(name);
			}
		}


		private class PropertyServiceWrapper : Altaxo.Main.Services.IPropertyService
		{
			public string ConfigDirectory
			{
				get
				{
					return ICSharpCode.Core.PropertyService.ConfigDirectory;
				}
			}
		}
	}
}
