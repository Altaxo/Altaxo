﻿#region Copyright

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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Graph.Plot.Data;

namespace Altaxo.Gui.Graph.Plot.Data
{
  public interface IXYZMeshedColumnPlotDataView
  {
    void SetDataView(object viewObject);
  }

  [ExpectedTypeOfView(typeof(IXYZMeshedColumnPlotDataView))]
  [UserControllerForObject(typeof(XYZMeshedColumnPlotData))]
  public class XYZMeshedColumnPlotDataController : MVCANControllerEditOriginalDocBase<XYZMeshedColumnPlotData, IXYZMeshedColumnPlotDataView>
  {
    private IMVCANController _dataProxyController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_dataProxyController, () => _dataProxyController = null);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _dataProxyController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.DataTableMatrix }, typeof(IMVCANController), UseDocument.Directly);
      }
      if (_view is not null)
      {
        _view.SetDataView(_dataProxyController.ViewObject);
      }
    }

    public override bool Apply(bool disposeController)
    {
      bool result;

      result = _dataProxyController.Apply(disposeController);
      if (!result)
        return result;

      return ApplyEnd(result, disposeController);
    }
  }
}
