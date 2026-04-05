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

#nullable disable
using System.Collections.Generic;
using Altaxo.Data;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Worksheet
{
  /// <summary>
  /// View interface for the PLS predict-value controller.
  /// </summary>
  public interface IPLSPredictValueView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Combines a calibration table with a destination table.
  /// </summary>
  public record CalibrationAndDestinationTable(DataTable CalibrationTable, DataTable DestinationTable);

  /// <summary>
  /// Controller for selecting a PLS calibration table and a destination table.
  /// </summary>
  [ExpectedTypeOfView(typeof(IPLSPredictValueView))]
  public class PLSPredictValueController : MVCANControllerEditImmutableDocBase<CalibrationAndDestinationTable, IPLSPredictValueView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<DataTable> _calibrationTables;

    /// <summary>
    /// Gets or sets the selectable calibration tables.
    /// </summary>
    public ItemsController<DataTable> CalibrationTables
    {
      get => _calibrationTables;
      set
      {
        if (!(_calibrationTables == value))
        {
          _calibrationTables = value;
          OnPropertyChanged(nameof(CalibrationTables));
        }
      }
    }


    private ItemsController<DataTable?> _destinationTables;

    /// <summary>
    /// Gets or sets the selectable destination tables.
    /// </summary>
    public ItemsController<DataTable?> DestinationTables
    {
      get => _destinationTables;
      set
      {
        if (!(_destinationTables == value))
        {
          _destinationTables = value;
          OnPropertyChanged(nameof(DestinationTables));
        }
      }
    }

    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var calibrationTables = Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.GetAvailablePLSCalibrationTables();

        CalibrationTables = new ItemsController<DataTable>(
          new Collections.SelectableListNodeList(
            calibrationTables.Select(t => new SelectableListNode(t.Name, t, false))));
        if (CalibrationTables.Items.Count > 0)
          CalibrationTables.SelectedItem = CalibrationTables.Items[0];


        DestinationTables = new ItemsController<DataTable?>(GetAvailableDestinationTables());
      }
    }

    private SelectableListNodeList GetAvailableDestinationTables()
    {
      var result = new SelectableListNodeList();

      result.Add(new SelectableListNode("New table", null, true));
      foreach (Altaxo.Data.DataTable table in Current.Project.DataTableCollection)
        result.Add(new SelectableListNode(table.Name, table, false));

      return result;
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (CalibrationTables.SelectedValue is null)
      {
        Current.Gui.ErrorMessageBox("Please choose a calibration table");
        return ApplyEnd(false, disposeController);
      }

      _doc = new CalibrationAndDestinationTable(CalibrationTables.SelectedValue, DestinationTables.SelectedValue);

      return ApplyEnd(true, disposeController);
    }


  }
}
