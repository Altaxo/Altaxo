#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2022 Dr. Dirk Lellinger
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
using Altaxo.Graph.Plot.Data;

namespace Altaxo.Gui.Graph.Plot.Data
{
  public interface IXYZMeshedColumnPlotDataView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IXYZMeshedColumnPlotDataView))]
  [UserControllerForObject(typeof(XYZMeshedColumnPlotData))]
  public class XYZMeshedColumnPlotDataController : MVCANControllerEditOriginalDocBase<XYZMeshedColumnPlotData, IXYZMeshedColumnPlotDataView>
  {


    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_dataProxyController, () => _dataProxyController = null);
    }

    #region Bindings

    private IMVCANController _dataProxyController;

    public IMVCANController DataProxyController
    {
      get => _dataProxyController;
      set
      {
        if (!(_dataProxyController == value))
        {
          _dataProxyController?.Dispose();
          _dataProxyController = value;
          OnPropertyChanged(nameof(DataProxyController));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        DataProxyController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.DataTableMatrix }, typeof(IMVCANController), UseDocument.Directly);
      }

    }

    public override bool Apply(bool disposeController)
    {
      if (false == _dataProxyController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      return ApplyEnd(true, disposeController);
    }
  }
}
