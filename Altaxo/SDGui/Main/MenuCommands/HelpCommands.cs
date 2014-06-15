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

using Altaxo;
using Altaxo.Main;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Windows.Forms;

namespace Altaxo.Main.Commands
{
	public class ShowAltaxoProgramHelp : AbstractMenuCommand
	{
		public override void Run()
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

	public class ShowAltaxoClassHelp : AbstractMenuCommand
	{
		public override void Run()
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

	public class DevelopmentTest : AbstractMenuCommand
	{
		public override void Run()
		{
			Run5();
		}

		public void Run5()
		{
			var ctrl = new Altaxo.Gui.Common.Drawing.NamedColorController();
			ctrl.InitializeDocument(Altaxo.Graph.NamedColors.Bisque);
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
			var pen = new Altaxo.Graph.Gdi.PenX(Altaxo.Graph.NamedColors.Red, 2);
			var ctrl = new Altaxo.Gui.Common.Drawing.PenAllPropertiesController(pen);
			Current.Gui.ShowDialog(ctrl, "Pen properties");
		}

		public void Run1()
		{
			var brush = new Altaxo.Graph.Gdi.BrushX(Altaxo.Graph.NamedColors.Black);
			var ctrl = new Altaxo.Gui.Common.Drawing.BrushControllerAdvanced();
			ctrl.InitializeDocument(brush);
			Current.Gui.ShowDialog(ctrl, "Brush pros");
		}
	}
}