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

using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Gui;
using Altaxo.Gui.Text.Viewing;
using Altaxo.Gui.Workbench;
using Altaxo.Text.Renderers;

namespace Altaxo.Text.Commands
{
  /// <summary>
  /// Provides a abstract class for issuing commands that apply to text document controllers.
  /// </summary>
  public abstract class AbstractTextControllerCommand : SimpleCommand
  {
    /// <summary>Determines if the command can be executed.</summary>
    /// <param name="parameter">The parameter (context of the command).</param>
    /// <returns>True if either the <paramref name="parameter"/> or the ActiveViewContent of the workbench is a <see cref="Altaxo.Gui.Text.Viewing.TextDocumentController"/>.
    /// </returns>
    public override bool CanExecute(object parameter)
    {
      if (!(parameter is IViewContent viewContent))
        viewContent = Current.Workbench.ActiveViewContent;
      return viewContent is Altaxo.Gui.Text.Viewing.TextDocumentController;
    }

    /// <summary>
    /// Determines the currently active worksheet and issues the command to that text document controller by calling
    /// Run with the text document controller as a parameter.
    /// </summary>
    public override void Execute(object parameter)
    {
      if (!(parameter is IViewContent activeViewContent))
        activeViewContent = Current.Workbench.ActiveViewContent;

      if (activeViewContent is TextDocumentController ctrl)
        Run(ctrl);
    }

    /// <summary>
    /// Override this function for adding own text document controller commands. You will get
    /// the text document controller in the parameter.
    /// </summary>
    /// <param name="ctrl">The text document controller this command is applied to.</param>
    public abstract void Run(TextDocumentController ctrl);
  }

  public class ExportOpenXML : AbstractTextControllerCommand
  {
    public override void Run(TextDocumentController ctrl)
    {
      OpenXMLExportOptions.ExportShowDialog(ctrl.TextDocument);
    }

    // we need at least one reference to a UIElement in this assembly in order to let the ReflectionService
    // recognize that this is a UI assembly.
    public void Test()
    {
      var ui = new System.Windows.UIElement();
    }
  }
}
