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
#endregion

using System;
using System.IO;
using System.Collections;
using System.CodeDom.Compiler;
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

			// set as early as possible the UI culture
			Altaxo.Serialization.GUIConversion.CultureSettings = Current.PropertyService.Get(Altaxo.Settings.UICultureSettings.SettingsStoragePath, Altaxo.Settings.UICultureSettings.FromDefault());
			System.Threading.Thread.CurrentThread.CurrentCulture = Current.PropertyService.Get(Altaxo.Settings.DocumentCultureSettings.SettingsStoragePath, Altaxo.Settings.DocumentCultureSettings.FromDefault()).ToCulture();

			Altaxo.Current.SetResourceService(new ResourceServiceWrapper());
			Altaxo.Current.SetProjectService(new Altaxo.Main.ProjectService());
			Altaxo.Current.SetGUIFactoryService(new Altaxo.Gui.GuiFactoryServiceWpfWin());

			Altaxo.Current.Gui.RegistedContextMenuProviders.Add(typeof(System.Windows.UIElement), ShowWpfContextMenu);

			Altaxo.Current.Gui.RegisteredGuiTechnologies.Add(typeof(System.Windows.UIElement));

			Altaxo.Current.SetPrintingService(new Altaxo.Main.PrintingService());

			// we construct the main document (for now)
			Altaxo.Current.ProjectService.CurrentOpenProject = new AltaxoDocument();

		}

		static void ShowWinFormContextMenu(object parent, object owner, string addInPath, double x, double y)
		{
			ICSharpCode.Core.WinForms.MenuService.ShowContextMenu(owner, addInPath, (System.Windows.Forms.Control)parent, (int)x, (int)y);
		}

		static void ShowWpfContextMenu(object parent, object owner, string addInPath, double x, double y)
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


		private class PropertyServiceWrapper : Altaxo.Main.Services.IPropertyService
		{
			/// <summary>Occurs when a property changed, argument is the key to the property that changed.</summary>
			public event Action<string> PropertyChanged;



			public PropertyServiceWrapper()
			{
				ICSharpCode.Core.PropertyService.PropertyChanged += new PropertyChangedEventHandler(PropertyService_PropertyChanged);

			}

			void PropertyService_PropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (null != PropertyChanged)
					PropertyChanged(e.Key);
			}


			public string ConfigDirectory
			{
				get
				{
					return ICSharpCode.Core.PropertyService.ConfigDirectory;
				}
			}

			public string Get(string property)
			{
				return ICSharpCode.Core.PropertyService.Get(property);
			}

			public T Get<T>(string property, T defaultValue)
			{
				return ICSharpCode.Core.PropertyService.Get(property, defaultValue);
			}

			public void Set<T>(string property, T value)
			{
				ICSharpCode.Core.PropertyService.Set(property, value);
			}
		}
	}
}
