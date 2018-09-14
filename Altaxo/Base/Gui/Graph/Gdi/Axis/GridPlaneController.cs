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

using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Gui.Common;
using Altaxo.Gui.Common.Drawing;

namespace Altaxo.Gui.Graph.Gdi.Axis
{
  [ExpectedTypeOfView(typeof(IMultiChildView))]
  [UserControllerForObject(typeof(GridPlane))]
  public class GridPlaneController : MVCANControllerEditOriginalDocBase<GridPlane, IMultiChildView>
  {
    private MultiChildController _innerController;

    private IMVCANController _grid1;
    private IMVCANController _grid2;
    private IMVCANController _background;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_grid1, () => _grid1 = null);
      yield return new ControllerAndSetNullMethod(_grid2, () => _grid2 = null);
      yield return new ControllerAndSetNullMethod(_background, () => _background = null);

      yield return new ControllerAndSetNullMethod(_innerController, () => _innerController = null);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _grid1 = new XYGridStyleController() { UseDocumentCopy = UseDocument.Directly };
        _grid1.InitializeDocument(_doc.GridStyleFirst ?? new GridStyle() { ShowGrid = false });
        Current.Gui.FindAndAttachControlTo(_grid1);
        var c1 = new ControlViewElement(GridName(_doc.PlaneID.InPlaneAxisNumber1), _grid1, _grid1.ViewObject);

        _grid2 = new XYGridStyleController() { UseDocumentCopy = UseDocument.Directly };
        _grid2.InitializeDocument(_doc.GridStyleSecond ?? new GridStyle() { ShowGrid = false });
        Current.Gui.FindAndAttachControlTo(_grid2);
        var c2 = new ControlViewElement(GridName(_doc.PlaneID.InPlaneAxisNumber2), _grid2, _grid2.ViewObject);

        _background = new BrushControllerAdvanced() { UseDocumentCopy = UseDocument.Directly };
        _background.InitializeDocument(_doc.Background ?? BrushX.Empty);
        Current.Gui.FindAndAttachControlTo(_background);
        var c3 = new ControlViewElement("Background", _background, _background.ViewObject);

        _innerController = new MultiChildController(new ControlViewElement[] { c1, c2, c3 }, false);
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (false == _innerController.Apply(disposeController))
        return false;

      _doc.GridStyleFirst = (GridStyle)_grid1.ModelObject;
      _doc.GridStyleSecond = (GridStyle)_grid2.ModelObject;
      var backBrush = (BrushX)_background.ModelObject;
      _doc.Background = backBrush.IsVisible ? backBrush : null;

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      if (null != _innerController)
      {
        _innerController.ViewObject = _view;
      }
    }

    protected override void DetachView()
    {
      if (null != _innerController)
      {
        _innerController.ViewObject = null;
      }
      base.DetachView();
    }

    private static string GridName(int axisNumber)
    {
      switch (axisNumber)
      {
        case 0:
          return "X-axis grid";

        case 1:
          return "Y-axis grid";

        case 2:
          return "Z-axis grid";

        default:
          return string.Format("Axis[{0}] grid", axisNumber);
      }
    }
  }
}
