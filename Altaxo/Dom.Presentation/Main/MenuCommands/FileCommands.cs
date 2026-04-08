#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using Altaxo.Gui;

namespace Altaxo.Main.Commands
{
  /// <summary>
  /// Closes the main application window.
  /// </summary>
  public class FileExit : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      ((System.Windows.Window)Current.Workbench.ViewObject).Close();
    }
  }

  /// <summary>
  /// Shows the About dialog for Altaxo.
  /// </summary>
  public class HelpAboutAltaxo : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      var ctrl = new Altaxo.Gui.Common.HelpAboutControl();

      ctrl.ShowDialog();
    }
  }
}
