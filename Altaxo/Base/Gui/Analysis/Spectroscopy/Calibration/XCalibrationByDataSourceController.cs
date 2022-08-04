#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Gui.Common;
using Altaxo.Science.Spectroscopy.Calibration;
using Altaxo.Science.Spectroscopy.Raman;

namespace Altaxo.Gui.Analysis.Spectroscopy.Calibration
{
  public interface IXCalibrationByDataSourceView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IXCalibrationByDataSourceView))]
  [UserControllerForObject(typeof(XCalibrationByDataSource))]
  public class XCalibrationByDataSourceController : MVCANControllerEditImmutableDocBase<XCalibrationByDataSource, IXCalibrationByDataSourceView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<DataTable> _availableCalibrationTables;

    public ItemsController<DataTable> AvailableCalibrationTables
    {
      get => _availableCalibrationTables;
      set
      {
        if (!(_availableCalibrationTables == value))
        {
          _availableCalibrationTables = value;
          OnPropertyChanged(nameof(AvailableCalibrationTables));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      var list = new SelectableListNodeList();

      if(initData)
      {
        DataTable? youngest=null;
        foreach(var t in Current.Project.DataTableCollection)
        {
          if(t.DataSource is RamanCalibrationDataSource rcds && rcds.IsContainingValidXAxisCalibration(t))
          {
            list.Add(new SelectableListNode(t.Name, t, false));

            if (youngest is null || t.CreationTimeUtc > youngest.CreationTimeUtc)
              youngest = t;
          }
        }

        AvailableCalibrationTables = new ItemsController<DataTable>(list);
        if(youngest is not null)
          AvailableCalibrationTables.SelectedValue = youngest;
      }
    }


    public override bool Apply(bool disposeController)
    {
      if(AvailableCalibrationTables.SelectedValue is { } calibTable)
      {
        var rcds = (RamanCalibrationDataSource)calibTable.DataSource;

        var calibrationData = rcds.GetXAxisCalibration(calibTable);
        _doc = _doc with { AbsoluteTableName = calibTable.Name, RelativeTableName = null,  CalibrationTable = calibrationData.ToImmutableArray()};
        return ApplyEnd(true, disposeController);
      }
      else
      {
        string message = AvailableCalibrationTables.Items.Count > 0 ? "Please select a calibration table" : "This calibration method is currently not available, because no calibration table was found.";
        Current.Gui.ErrorMessageBox(message);
        return ApplyEnd(false, disposeController);
      }
    }

  }
}
