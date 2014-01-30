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

using Altaxo.Gui;
using Altaxo.Gui.SharpDevelop;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Policy;

namespace Altaxo.Main.Commands // ICSharpCode.SharpDevelop.Commands
{
	using Altaxo.Settings;

	public class AutostartCommand : AbstractCommand
	{
		private const string workbenchMemento = "SharpDevelop.Workbench.WorkbenchMemento";
		private static string[] _commandLineArgs;

		public static void EarlyRun(string[] commandLineArgs)
		{
			_commandLineArgs = commandLineArgs;

			Altaxo.Current.SetPropertyService(new Altaxo.Main.Services.PropertyService());
			Altaxo.Current.PropertyService.BuiltinSettings.SetValue(Altaxo.Settings.CultureSettings.PropertyKeyUICulture, new Altaxo.Settings.CultureSettings(Altaxo.Settings.CultureSettings.StartupUICultureInfo));
			Altaxo.Current.PropertyService.BuiltinSettings.SetValue(Altaxo.Settings.CultureSettings.PropertyKeyDocumentCulture, new Altaxo.Settings.CultureSettings(Altaxo.Settings.CultureSettings.StartupDocumentCultureInfo));
			// set the document culture and the UI culture as early as possible
			CultureSettings.PropertyKeyUICulture.ApplyProperty(Current.PropertyService.GetValue<CultureSettings>(CultureSettings.PropertyKeyUICulture, Services.RuntimePropertyKind.UserAndApplicationAndBuiltin));
			CultureSettings.PropertyKeyDocumentCulture.ApplyProperty(Current.PropertyService.GetValue<CultureSettings>(CultureSettings.PropertyKeyDocumentCulture, Services.RuntimePropertyKind.UserAndApplicationAndBuiltin));

			Altaxo.Current.SetResourceService(new ResourceServiceWrapper());
			Altaxo.Current.SetProjectService(new Altaxo.Main.ProjectService());
			Altaxo.Current.SetGUIFactoryService(new Altaxo.Gui.GuiFactoryServiceWpfWin());

			Altaxo.Current.Gui.RegistedContextMenuProviders.Add(typeof(System.Windows.UIElement), ShowWpfContextMenu);

			Altaxo.Current.Gui.RegisteredGuiTechnologies.Add(typeof(System.Windows.UIElement));

			Altaxo.Current.SetPrintingService(new Altaxo.Main.PrintingService());

			Altaxo.Graph.Gdi.GdiFontManager.Register();
			Altaxo.Gui.WpfFontManager.Register();

			// we construct the main document (for now)
			Altaxo.Current.ProjectService.CreateInitialDocument();
		}

		private static void ShowWinFormContextMenu(object parent, object owner, string addInPath, double x, double y)
		{
			ICSharpCode.Core.WinForms.MenuService.ShowContextMenu(owner, addInPath, (System.Windows.Forms.Control)parent, (int)x, (int)y);
		}

		private static void ShowWpfContextMenu(object parent, object owner, string addInPath, double x, double y)
		{
			ICSharpCode.Core.Presentation.MenuService.ShowContextMenu((System.Windows.UIElement)parent, owner, addInPath);
		}

		public override void Run()
		{
			Altaxo.Current.ProjectService.ProjectChanged += new ProjectEventHandler(Altaxo.Current.Workbench.EhProjectChanged);
			// less important services follow now
			Altaxo.Main.Services.FitFunctionService fitFunctionService = new Altaxo.Main.Services.FitFunctionService();
			Altaxo.Current.SetFitFunctionService(fitFunctionService);
			AddInTree.GetTreeNode("/Altaxo/BuiltinTextures").BuildChildItems<object>(this);
			Altaxo.Graph.ColorManagement.ColorSetManager.Instance.AddRange(AddInTree.GetTreeNode("/Altaxo/ApplicationColorSets").BuildChildItems<Altaxo.Graph.ColorManagement.ColorSet>(this));
			Altaxo.Main.Services.ParserServiceConnector.Initialize();
			Altaxo.Serialization.AutoUpdates.UpdateDownloaderStarter.Run();
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
	}
}