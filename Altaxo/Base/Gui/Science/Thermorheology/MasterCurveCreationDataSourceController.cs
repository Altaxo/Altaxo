#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2024 Dr. Dirk Lellinger
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
using Altaxo.Gui.Data;
using Altaxo.Science.Thermorheology.MasterCurves;

namespace Altaxo.Gui.Science.Thermorheology
{
  [UserControllerForObject(typeof(MasterCurveCreationDataSource))]
  public class MasterCurveCreationDataSourceController : DataSourceControllerBase<MasterCurveCreationDataSource>
  {
    protected override IMVCANController GetProcessDataController()
    {
      var processDataController = new MasterCurveDataController() { UseDocumentCopy = UseDocument.Directly };
      processDataController.InitializeDocument(_doc.ProcessData);
      Current.Gui.FindAndAttachControlTo(processDataController);
      return processDataController;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        if (ProcessOptionsController is MasterCurveCreationOptionsController optionsController)
        {
          optionsController.NumberOfGroups = _doc.ProcessData.CurveData.Count;
        }

        if (this.ProcessOptionsController is IMVCANDController andController)
        {
          andController.MadeDirty += EhOptionsMadeDirty;
        }
      }
    }

    private void EhOptionsMadeDirty(IMVCANDController controller)
    {
      if (controller is MasterCurveCreationOptionsController optionsController &&
         ProcessDataController is MasterCurveDataController dataController)
      {
        dataController.HintOptionValues(optionsController.NumberOfGroups, optionsController.Property1, optionsController.Property2);
      }
    }
  }
}
