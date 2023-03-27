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

#nullable disable warnings
using System.IO;
using System.Windows.Forms;
using Altaxo.Gui;
using Altaxo.Main.Services;

namespace Altaxo.Main.Commands
{
  public class ShowAltaxoProgramHelp : SimpleCommand
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
        var startInfo = new System.Diagnostics.ProcessStartInfo("https://altaxo.github.io/AltaxoClassReference/html/1B7FE024E7E614BFA13DAA1FD005CB2E.htm")
        {
          UseShellExecute = true,
        };

        System.Diagnostics.Process.Start(startInfo);
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
          System.Diagnostics.Process.Start("https://altaxo.github.io/AltaxoClassReference/html/R_Project_Documentation.htm");
        }
      }
    }
  }
}
