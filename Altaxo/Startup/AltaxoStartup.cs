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
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Resources;
using System.Xml;
using System.Threading;
using System.Runtime.Remoting;
using System.Security.Policy;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

// using Altaxo;
// using Altaxo.Main;


namespace BeautyGUI
{
	class Startup
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void OldMain() 
		{
			//App.InitializeMainController(new ICSharpCode.SharpDevelop.Gui.BeautyWorkbench(new ICSharpCode.SharpDevelop.Gui.BeautyWorkbenchWindow(), new AltaxoDocument()));

			// Current.Main();
		}
	}
}

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This Class is the Core main class, it starts the program.
	/// </summary>
	public class SharpDevelopMain
	{
		static string[] commandLineArgs = null;
		
		public static string[] CommandLineArgs 
		{
			get 
			{
				return commandLineArgs;
			}
		}
		
		static void ShowErrorBox(object sender, ThreadExceptionEventArgs eargs)
		{
			DialogResult result = new ExceptionBox(eargs.Exception).ShowDialog();

			DataObject dataObject = new DataObject();
			dataObject.SetData(DataFormats.Text, eargs.Exception.ToString());
			Clipboard.SetDataObject(dataObject, true);
			
			switch (result) 
			{
				case DialogResult.Ignore:
					break;
				case DialogResult.Abort:
					Application.Exit();
					break;
				case DialogResult.Yes:
					Process.Start("http://sourceforge.net/tracker/?func=add&group_id=73395&atid=537651");
					break;
			}
		}
		
		/// <summary>
		/// Starts the core of SharpDevelop.
		/// </summary>
		[STAThread()]
		public static void Main(string[] args)
		{
			commandLineArgs = args;
			bool noLogo = false;
			
			SplashScreenForm.SetCommandLineArgs(args);
			
			foreach (string parameter in SplashScreenForm.GetParameterList()) 
			{
				switch (parameter.ToUpper()) 
				{
					case "NOLOGO":
						noLogo = true;
						break;
				}
			}
			
			bool ignoreDefaultPath = false;
			string [] addInDirs = ICSharpCode.SharpDevelop.AddInSettingsHandler.GetAddInDirectories(out ignoreDefaultPath);
			AddInTreeSingleton.SetAddInDirectories(addInDirs, ignoreDefaultPath);
			
			if (!noLogo) 
			{
				SplashScreenForm.SplashScreen.Show();
			}
			Application.ThreadException += new ThreadExceptionEventHandler(ShowErrorBox);
			
			ArrayList commands = null;
			try 
			{
				ServiceManager.Services.AddService(new MessageService());
				ServiceManager.Services.AddService(new ResourceService());
				ServiceManager.Services.AddService(new IconService());
				ServiceManager.Services.InitializeServicesSubsystem("/Workspace/Services");
			
				commands = AddInTreeSingleton.AddInTree.GetTreeNode("/Workspace/Autostart").BuildChildItems(null);
				for (int i = 0; i < commands.Count - 1; ++i) 
				{
					((ICommand)commands[i]).Run();
				}
			} 
			catch (XmlException e) 
			{
				MessageBox.Show("Could not load XML :\n" + e.Message);
				return;
			} 
			catch (Exception e) 
			{
				MessageBox.Show("Loading error, please reinstall :\n" + e.ToString());
				return;
			} 
			finally 
			{
				if (SplashScreenForm.SplashScreen != null) 
				{
					SplashScreenForm.SplashScreen.Close();
				}
			}
			
			// run the last autostart command, this must be the workbench starting command
			if (commands.Count > 0) 
			{
				((ICommand)commands[commands.Count - 1]).Run();
			}
			
			// unloading services
			ServiceManager.Services.UnloadAllServices();
		}
	}
}




