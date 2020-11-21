#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Gui;
using Altaxo.Gui.Workbench;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Provides a abstract class for issuing commands that apply to worksheet controllers.
  /// </summary>
  public abstract class AbstractWorksheetControllerCommand : SimpleCommand
  {
    /// <summary>Determines if the command can be executed.</summary>
    /// <param name="parameter">The parameter (context of the command).</param>
    /// <returns>True if either the <paramref name="parameter"/> or the ActiveViewContent of the workbench is a <see cref="Altaxo.Gui.Worksheet.Viewing.WorksheetController"/>.
    /// </returns>
    public override bool CanExecute(object parameter)
    {
      if (!(parameter is IViewContent viewContent))
        viewContent = Current.Workbench.ActiveViewContent;
      return viewContent is Altaxo.Gui.Worksheet.Viewing.WorksheetController;
    }

    /// <summary>
    /// Determines the currently active worksheet and issues the command to that worksheet by calling
    /// Run with the worksheet as a parameter.
    /// </summary>
    public override void Execute(object parameter)
    {
      if (!(parameter is IViewContent activeViewContent))
        activeViewContent = Current.Workbench.ActiveViewContent;

      if (activeViewContent is Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
        Run(ctrl);
    }

    /// <summary>
    /// Override this function for adding own worksheet commands. You will get
    /// the worksheet controller in the parameter.
    /// </summary>
    /// <param name="ctrl">The worksheet controller this command is applied to.</param>
    public abstract void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl);
  }
}
