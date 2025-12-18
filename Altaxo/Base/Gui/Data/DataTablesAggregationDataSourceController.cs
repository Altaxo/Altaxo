#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2015 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Gui.Data
{
  /// <summary>
  /// Controller for configuring a <see cref="DataTablesAggregationDataSource"/> in the GUI.
  /// </summary>
  [UserControllerForObject(typeof(DataTablesAggregationDataSource))]
  public class DataTablesAggregationSourceController : DataSourceControllerBase<DataTablesAggregationDataSource>
  {
    /// <inheritdoc/>
    protected override IMVCANController GetProcessDataController()
    {
      var pdc = base.GetProcessDataController();

      if (pdc is DataTablesAggregationDataController pdcDataController)
      {
        pdcDataController.TestDataAndOptions = EhTestDataAndOptions;
      }
      return pdc;
    }

    /// <summary>
    /// Tests the current data and options of the aggregation process and shows a message box with the result.
    /// </summary>
    private void EhTestDataAndOptions()
    {
      if (!this.Apply(false)) { return; }

      var errors = _doc.CheckDataAndOptions().ToList();

      if (errors.Any())
      {
        var stb = new StringBuilder();
        stb.AppendLine($"The test revealed {errors.Count} errors");
        var maxCount = Math.Min(16, errors.Count);

        if (maxCount != errors.Count)
        {
          stb.AppendLine($"The first {maxCount} errors are:");
        }
        else
        {
          stb.AppendLine("The errors are:");
        }
        for (int i = 0; i < maxCount; i++)
        {
          stb.AppendLine(errors[i]);
        }
        Current.Gui.ErrorMessageBox(stb.ToString(), $"Test failed, {errors.Count} errors!");
      }
      else
      {
        Current.Gui.InfoMessageBox("The test was finished successfully.");
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (disposeController && base.GetProcessDataController() is DataTablesAggregationDataController pdcDataController)
      {
        pdcDataController.TestDataAndOptions = null;
      }

      return base.Apply(disposeController);
    }
  }
}
