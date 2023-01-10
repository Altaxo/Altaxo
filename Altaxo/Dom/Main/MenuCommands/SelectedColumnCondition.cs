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
using System;
using Altaxo.AddInItems;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// This helps in conditions where the number of selected data columns cares.
  /// Valid values are all (all columns must be selected, none (no column must be selected),
  /// one (exactly one column must be selected), any (one or more columns must be selected),
  /// or the number of columns.
  /// </summary>
  public class SelectedDataConditionEvaluator : IConditionEvaluator
  {
    public bool IsValid(object caller, Condition condition)
    {
      string selectedData = condition.Properties["selected"].ToLower();

      if (Current.Workbench.ActiveViewContent is null)
        return false;
      if (!(Current.Workbench.ActiveViewContent is Altaxo.Gui.Worksheet.Viewing.IWorksheetController ctrl))
        return false;

      int val = ctrl.SelectedDataColumns.Count;

      switch (selectedData)
      {
        case "none":
          return val == 0;

        case "one":
          return val == 1;

        case "two":
          return val == 2;

        case "all":
          return val == ctrl.DataTable.DataColumnCount;

        case "any":
          return val > 0;

        case "*":
          return val > 0;

        default:
          {
            try
            {
              int num = int.Parse(selectedData);
              return val == num;
            }
            catch (Exception)
            {
              return false;
            }
          }
      }
    }
  }

  /// <summary>
  /// This helps in conditions where the number of selected property columns cares.
  /// Valid values are all (all columns must be selected, none (no column must be selected),
  /// one (exactly one column must be selected), any (one or more columns must be selected),
  /// or the number of columns.
  /// </summary>
  public class SelectedPropertyConditionEvaluator : IConditionEvaluator
  {
    public bool IsValid(object caller, Condition condition)
    {
      string selectedData = condition.Properties["selected"].ToLower();

      if (Current.Workbench.ActiveViewContent is null)
        return false;
      if (!(Current.Workbench.ActiveViewContent is Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl))
        return false;

      int val = ctrl.SelectedPropertyColumns.Count;

      switch (selectedData)
      {
        case "none":
          return val == 0;

        case "one":
          return val == 1;

        case "two":
          return val == 2;

        case "all":
          return val == ctrl.DataTable.PropertyColumnCount;

        case "any":
          return val > 0;

        case "*":
          return val > 0;

        default:
          {
            try
            {
              int num = int.Parse(selectedData);
              return val == num;
            }
            catch (Exception)
            {
              return false;
            }
          }
      }
    }
  }
}
