﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Text;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Graph3D.Plot;
using Altaxo.Gui.Common;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Plot.Data;

namespace Altaxo.Gui.Graph.Graph3D.Plot
{
  [UserControllerForObject(typeof(DataMeshPlotItem))]
  [ExpectedTypeOfView(typeof(ITabbedElementView))]
  internal class DataMeshPlotItemController : MVCANControllerEditOriginalDocBase<DataMeshPlotItem, ITabbedElementView>
  {
    private TabbedElementController _innerController;

    private IMVCANController _styleController;

    /// <summary>Controls the option view where users can copy the image to disc or save the image.</summary>
    private IMVCANController _optionsController;

    /// <summary>Controls the data view, in which the user can chose which columns to use in the plot item.</summary>
    private IMVCANController _dataController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_styleController, () => _styleController = null);
      yield return new ControllerAndSetNullMethod(_optionsController, () => _optionsController = null);
      yield return new ControllerAndSetNullMethod(_dataController, () => _dataController = null);
      yield return new ControllerAndSetNullMethod(_innerController, () => _innerController = null);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _innerController = new TabbedElementController();
        InitializeStyle();
        InitializeDataView();
        InitializeOptionView();
        _innerController.BringTabToFront(0);
      }
    }

    public override bool Apply(bool disposeController)
    {
      bool result = true;

      if (_styleController is not null)
      {
        if (!_styleController.Apply(disposeController))
          return false;
      }

      if (_dataController is not null)
      {
        if (!_dataController.Apply(disposeController))
          return false;
      }

      return ApplyEnd(result, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      if (_innerController is not null)
        _innerController.ViewObject = _view;
    }

    protected override void DetachView()
    {
      if (_innerController is not null)
        _innerController.ViewObject = null;
      base.DetachView();
    }

    private void InitializeStyle()
    {
      _styleController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.Style }, typeof(IMVCANController), UseDocument.Directly);
      _innerController.AddTab("Style", _styleController, _styleController.ViewObject);
    }

    private void InitializeOptionView()
    {
      _optionsController = new Gdi.Plot.DensityImagePlotItemOptionController() { UseDocumentCopy = UseDocument.Directly };
      _optionsController.InitializeDocument(_doc);
      Current.Gui.FindAndAttachControlTo(_optionsController);
      _innerController.AddTab("Options", _optionsController, _optionsController.ViewObject);
    }

    private void InitializeDataView()
    {
      _dataController = new XYZMeshedColumnPlotDataController { UseDocumentCopy = UseDocument.Directly };
      _dataController.InitializeDocument(_doc.Data);
      Current.Gui.FindAndAttachControlTo(_dataController);
      _innerController.AddTab("Data", _dataController, _dataController.ViewObject);
    }
  }
}
