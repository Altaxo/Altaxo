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
        Help.ShowHelp((Form)WorkbenchSingleton.Workbench, fileName);
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
        Help.ShowHelp((Form)WorkbenchSingleton.Workbench, fileName);
      }
    }
  }
}
