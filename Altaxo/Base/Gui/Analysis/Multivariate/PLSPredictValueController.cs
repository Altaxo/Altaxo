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

namespace Altaxo.Gui.Worksheet
{
  #region Interfaces

  public interface IPLSPredictValueView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for PLSPredictValueController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IPLSPredictValueView))]
  public class PLSPredictValueController : MVCANControllerEditImmutableDocBase<(string CalibrationTable, string DestinationTable), IPLSPredictValueView>
  {
    public string[] CalibrationTables { get; private set; }
    public string[] DestinationTables { get; private set; }

    private string _selectedDestinationTableName;

    public string SelectedDestinationTableName
    {
      get => _selectedDestinationTableName;
      set
      {
        if (!(_selectedDestinationTableName == value))
        {
          _selectedDestinationTableName = value;
          OnPropertyChanged(nameof(SelectedDestinationTableName));
        }
      }
    }

    private string _selectedCalibrationTableName;

    public string SelectedCalibrationTableName
    {
      get => _selectedCalibrationTableName;
      set
      {
        if (!(_selectedCalibrationTableName == value))
        {
          _selectedCalibrationTableName = value;
          OnPropertyChanged(nameof(SelectedCalibrationTableName));
        }
      }
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        CalibrationTables = Altaxo.Worksheet.Commands.Analysis.ChemometricCommands.GetAvailablePLSCalibrationTables();
        DestinationTables = GetAvailableDestinationTables();

      }
    }

    private string[] GetAvailableDestinationTables()
    {
      var result = new System.Collections.ArrayList
      {
        "New table"
      };
      foreach (Altaxo.Data.DataTable table in Current.Project.DataTableCollection)
        result.Add(table.Name);

      return (string[])result.ToArray(typeof(string));
    }

    public override bool Apply(bool disposeController)
    {
      if (string.IsNullOrEmpty(SelectedCalibrationTableName))
      {
        Current.Gui.ErrorMessageBox("Please choose a calibration table");
        return ApplyEnd(false, disposeController);
      }
      if (string.IsNullOrEmpty(SelectedDestinationTableName))
      {
        Current.Gui.ErrorMessageBox("Please choose a destination table");
        return ApplyEnd(false, disposeController);
      }

      _doc = (SelectedCalibrationTableName, SelectedDestinationTableName);

      return ApplyEnd(true, disposeController);
    }


  }
}
