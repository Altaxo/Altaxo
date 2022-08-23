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
using Altaxo.AddInItems;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// This condition is true if the active view content is a worksheet which contains PLS model data.
  /// </summary>
  public class PLSModelConditionEvaluator : IConditionEvaluator
  {
    public bool IsValid(object caller, Condition condition)
    {
      var selectedData = condition.Properties["ContainsPLSModelData"];

      if (Current.Workbench.ActiveViewContent is null)
      {
        return false;
      }

      if (Current.Workbench.ActiveViewContent is not Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl || ctrl.DataTable is not { } dataTable)
      {
        return false;
      }

      if(dataTable?.GetTableProperty("Content") is Altaxo.Calc.Regression.Multivariate.MultivariateContentMemento plsMemo)
      {
        if(Altaxo.Calc.Regression.Multivariate.MultivariateContentMemento.TryConvertToDatasource(plsMemo, out var dataSource))
        {
          dataTable.DataSource = dataSource;
          dataTable.RemoveTableProperty("Content");
        }
      }

      return
        // 2022-07-12 we switched to DimensionReductionAndRegressionDataSource
        dataTable.DataSource is Altaxo.Calc.Regression.Multivariate.DimensionReductionAndRegressionDataSource;
    }
  }
}
