#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using Altaxo.AddInItems;
using System;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Evaluates the condition, whether or not a data source is present for a table.
  /// </summary>
  public class TableDataSourcePresentConditionEvaluator : IConditionEvaluator
  {
    public bool IsValid(object caller, Condition condition)
    {
      if (Current.Workbench.ActiveViewContent == null)
        return false;
      if (!(Current.Workbench.ActiveViewContent is Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl))
        return false;

      return ctrl.DataTable.DataSource != null;
    }
  }
}
