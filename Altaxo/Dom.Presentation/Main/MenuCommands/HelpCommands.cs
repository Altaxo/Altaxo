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

using Altaxo.Drawing;
using Altaxo.Gui;
using Altaxo.Gui.AddInItems;
using Altaxo.Main.Services;
using System;
using System.IO;
using System.Windows.Forms;

namespace Altaxo.Main.Commands
{
	public class ShowAltaxoProgramHelp : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			string fileName = FileUtility.ApplicationRootPath +
				Path.DirectorySeparatorChar + "doc" +
				Path.DirectorySeparatorChar + "help" +
				Path.DirectorySeparatorChar + "AltaxoHelp.chm";
			if (FileUtility.TestFileExists(fileName))
			{
				Help.ShowHelp(null, fileName);
			}
		}
	}

	public class ShowAltaxoClassHelp : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			string fileName = FileUtility.ApplicationRootPath +
				Path.DirectorySeparatorChar + "doc" +
				Path.DirectorySeparatorChar + "help" +
				Path.DirectorySeparatorChar + "AltaxoClassRef.chm";

			if (System.IO.File.Exists(fileName))
			{
				Help.ShowHelp(null, fileName);
			}
			else
			{
				if (Current.Gui.YesNoMessageBox("Altaxo class reference was not found on local computer. Do you want to open the online class reference instead?", "Local class ref not found!", true))
				{
					System.Diagnostics.Process.Start("http://altaxo.sourceforge.net/AltaxoClassRef/");
				}
			}
		}
	}

	public class DevelopmentTest : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			Run6();
		}

		public void Run6()
		{
			GC.Collect();
			System.Threading.Thread.Sleep(1000);
			Altaxo.Main.SuspendableDocumentNodeBase.ReportNotConnectedDocumentNodes(true);
			Altaxo.Main.SuspendableDocumentNode.ReportChildListProblems();
			Altaxo.Main.SuspendableDocumentNode.ReportWrongChildParentRelations();
		}

		public void Run5()
		{
			var ctrl = new Altaxo.Gui.Common.Drawing.NamedColorController();
			ctrl.InitializeDocument(NamedColors.Bisque);
			Current.Gui.ShowDialog(ctrl, "ddd");
		}

		public void Run3()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		public void Run2()
		{
			var pen = new Altaxo.Graph.Gdi.PenX(NamedColors.Red, 2);
			var ctrl = new Altaxo.Gui.Common.Drawing.PenAllPropertiesController(pen);
			Current.Gui.ShowDialog(ctrl, "Pen properties");
		}

		public void Run1()
		{
			var brush = new Altaxo.Graph.Gdi.BrushX(NamedColors.Black);
			var ctrl = new Altaxo.Gui.Common.Drawing.BrushControllerAdvanced();
			ctrl.InitializeDocument(brush);
			Current.Gui.ShowDialog(ctrl, "Brush pros");
		}
	}

	public class ReportParentChildProblemsInDocument : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			GC.Collect();
			Altaxo.Main.SuspendableDocumentNode.ReportParentChildAndDisposedProblems(Current.Project, true);
			Altaxo.Main.SuspendableDocumentNode.ReportSuspendedNodesProblems(Current.Project);
		}
	}

	public class OpenDownloadDirectory : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			var dir1 = Altaxo.Serialization.AutoUpdates.PackageInfo.GetDownloadDirectory(false);
			var dir2 = Altaxo.Serialization.AutoUpdates.PackageInfo.GetDownloadDirectory(true);

			int len = Math.Min(dir1.Length, dir2.Length);
			int i;
			for (i = 0; i < len; ++i)
				if (dir1[i] != dir2[i])
					break;

			string dir = dir1.Substring(0, i);

			string args = "/e," + dir;

			var processInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", args);

			processInfo.CreateNoWindow = false;
			processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

			try
			{
				var proc = System.Diagnostics.Process.Start(processInfo);
			}
			catch (Exception)
			{
			}
		}
	}

	public class OpenSettingsDirectory : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Altaxo");

			string args = "/e," + dir;

			var processInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", args);

			processInfo.CreateNoWindow = false;
			processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

			try
			{
				var proc = System.Diagnostics.Process.Start(processInfo);
			}
			catch (Exception)
			{
			}
		}
	}

	/// <summary>
	/// This command opens the project directory (i.e. the directory where the current project is stored) in Windows explorer.
	/// </summary>
	public class OpenProjectDirectory : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			if (string.IsNullOrEmpty(Current.IProjectService.CurrentProjectFileName))
			{
				Current.Gui.ErrorMessageBox("Sorry, can't open project directory because the current project is not saved yet.", "Error");
				return;
			}

			string dir = System.IO.Path.GetDirectoryName(Current.IProjectService.CurrentProjectFileName);

			string args = "/e," + dir;

			var processInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", args);

			processInfo.CreateNoWindow = false;
			processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

			try
			{
				var proc = System.Diagnostics.Process.Start(processInfo);
			}
			catch (Exception)
			{
			}
		}
	}
}