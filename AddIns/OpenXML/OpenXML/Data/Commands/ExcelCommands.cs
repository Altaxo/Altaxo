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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Gui;
using Altaxo.Gui.Pads.ProjectBrowser;
using Altaxo.Gui.Workbench;
using Altaxo.Gui.Worksheet.Viewing;
using Altaxo.Worksheet.Commands;

namespace Altaxo.Data.Commands
{

  /// <summary>
  /// Exports the currently open <see cref="DataTable"/> into an Microsoft Excel file.
  /// </summary>
  /// <seealso cref="Altaxo.Worksheet.Commands.AbstractWorksheetControllerCommand" />
  public class ExportDataTableToExcel : AbstractWorksheetControllerCommand
  {
    /// <inheritdoc />
    public override void Run(WorksheetController ctrl)
    {
      Altaxo.Data.ExcelExporter.DoExportTableShowDialog(ctrl.DataTable);
    }
  }

  /// <summary>
  /// Exports one or multiple <see cref="DataTable"/>s that are currently selected in the project browser into separate Microsoft Excel files.
  /// </summary>
  /// <seealso cref="Altaxo.Gui.Pads.ProjectBrowser.ProjectBrowseControllerCommand" />
  public class ExportDataTablesToMultipleExcelFiles : ProjectBrowseControllerCommand
  {
    /// <inheritdoc />
    protected override void Run(ProjectBrowseController ctrl)
    {
      var list = Current.Project.Folders.GetExpandedProjectItemSet(ctrl.GetSelectedListItems())
                      .OfType<Altaxo.Data.DataTable>();
      int count = list.Count();

      if (count == 0)
        return;
      else if (count == 1)
        Altaxo.Data.ExcelExporter.DoExportTableShowDialog(list.First());
      else
        Altaxo.Data.ExcelExporter.ShowExportMultipleExcelFilesDialog(list);
    }
  }
}
