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
using System.Windows.Forms;
using System.IO;
using ICSharpCode.Core;
using Altaxo;
using Altaxo.Main;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpDevelop.Gui;

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
			if (FileUtility.TestFileExists(fileName))
			{
				Help.ShowHelp(null, fileName);
			}
		}
	}

	public class DevelopmentTest : AbstractMenuCommand
	{

		public override void Run()
		{
			var graphView	= Current.Workbench.ActiveViewContent	as Altaxo.Gui.SharpDevelop.SDGraphViewContent;
						
			var doc = null==graphView ? null : graphView.Controller.Doc;

			Altaxo.Gui.Graph.PrintingController ctrl = new Gui.Graph.PrintingController();
			ctrl.InitializeDocument(doc);
			Current.Gui.FindAndAttachControlTo(ctrl);

			Current.Gui.ShowDialog(ctrl, "Print");
		}

		public void Run2()
		{
			var pen = new Altaxo.Graph.Gdi.PenX(Altaxo.Graph.NamedColor.Red, 2);
			var ctrl = new Altaxo.Gui.Common.Drawing.PenAllPropertiesController(pen);
			Current.Gui.ShowDialog(ctrl, "Pen properties");
		}

		public void Run1()
		{
			var brush = new Altaxo.Graph.Gdi.BrushX(Altaxo.Graph.NamedColor.Black);
			var ctrl = new Altaxo.Gui.Common.Drawing.BrushControllerAdvanced(brush);
			Current.Gui.ShowDialog(ctrl, "Brush pros");
		}
	}


}
