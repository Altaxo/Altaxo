#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using System.Linq;
using Altaxo.Gui.Pads.ProjectBrowser;

namespace Altaxo.Graph.Commands
{
  /// <summary>
  /// Exports one or multiple <see cref="DataTable"/>s that are currently selected in the project browser into separate Microsoft Excel files.
  /// </summary>
  /// <seealso cref="Altaxo.Gui.Pads.ProjectBrowser.ProjectBrowseControllerCommand" />
  public class ExportGraphsToOpenWordDocument : ProjectBrowseControllerCommand
  {
    /// <inheritdoc />
    protected override void Run(ProjectBrowseController ctrl)
    {
      var list = Current.Project.Folders.GetExpandedProjectItemSet(ctrl.GetSelectedListItems())
                      .OfType<Altaxo.Graph.GraphDocumentBase>();
      int count = list.Count();

      if (count == 0)
        return;
      try
      {
        Altaxo.Graph.Procedures.GraphToOpenWordDocumentExporter.PushGraphsToOpenWordDocument(list, forceUsingDropFile: false, isInteractive: true);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message, "Errors during export");
      }
    }
  }

  /// <summary>
  /// Exports one or multiple <see cref="DataTable"/>s that are currently selected in the project browser into separate Microsoft Excel files.
  /// </summary>
  /// <seealso cref="Altaxo.Gui.Pads.ProjectBrowser.ProjectBrowseControllerCommand" />
  public class ExportGraphsToOpenPowerPointDocument : ProjectBrowseControllerCommand
  {
    /// <inheritdoc />
    protected override void Run(ProjectBrowseController ctrl)
    {
      var list = Current.Project.Folders.GetExpandedProjectItemSet(ctrl.GetSelectedListItems())
                      .OfType<Altaxo.Graph.GraphDocumentBase>();
      int count = list.Count();

      if (count == 0)
        return;

      try
      {
        Altaxo.Graph.Procedures.GraphToOpenPowerPointDocumentExporter.PushGraphsToOpenPowerPointDocument(list, forceUsingDropFile: false, isInteractive: true);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message, "Errors during export");
      }
    }
  }

}
