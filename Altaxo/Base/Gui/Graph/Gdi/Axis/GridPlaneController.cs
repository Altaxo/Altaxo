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
using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Gui.Common;
using Altaxo.Gui.Common.Drawing;

namespace Altaxo.Gui.Graph.Gdi.Axis
{
  public interface IGridPlanView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IGridPlanView))]
  [UserControllerForObject(typeof(GridPlane))]
  public class GridPlaneController : MVCANControllerEditOriginalDocBase<GridPlane, IGridPlanView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_grid1, () => _grid1 = null);
      yield return new ControllerAndSetNullMethod(_grid2, () => _grid2 = null);
      yield return new ControllerAndSetNullMethod(_background, () => _background = null);
    }

    #region Bindings

    private IMVCANController _grid1;

    public IMVCANController Grid1Controller
    {
      get => _grid1;
      set
      {
        if (!(_grid1 == value))
        {
          _grid1 = value;
          OnPropertyChanged(nameof(Grid1Controller));
        }
      }
    }

    private string _Grid1Name;

    public string Grid1Name
    {
      get => _Grid1Name;
      set
      {
        if (!(_Grid1Name == value))
        {
          _Grid1Name = value;
          OnPropertyChanged(nameof(Grid1Name));
        }
      }
    }


    private IMVCANController _grid2;

    public IMVCANController Grid2Controller
    {
      get => _grid2;
      set
      {
        if (!(_grid2 == value))
        {
          _grid2 = value;
          OnPropertyChanged(nameof(Grid2Controller));
        }
      }
    }

    private string _Grid2Name;

    public string Grid2Name
    {
      get => _Grid2Name;
      set
      {
        if (!(_Grid2Name == value))
        {
          _Grid2Name = value;
          OnPropertyChanged(nameof(Grid2Name));
        }
      }
    }


    private IMVCANController _background;

    public IMVCANController BackgroundController
    {
      get => _background;
      set
      {
        if (!(_background == value))
        {
          _background = value;
          OnPropertyChanged(nameof(BackgroundController));
          OnPropertyChanged(nameof(BackgroundView));
        }
      }
    }

    public object? BackgroundView => _background?.ViewObject;


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var grid1 = new XYGridStyleController() { UseDocumentCopy = UseDocument.Directly };
        grid1.InitializeDocument(_doc.GridStyleFirst ?? new GridStyle() { ShowGrid = false });
        Grid1Controller = grid1;
        Grid1Name = GridName(_doc.PlaneID.InPlaneAxisNumber1);

        var grid2 = new XYGridStyleController() { UseDocumentCopy = UseDocument.Directly };
        grid2.InitializeDocument(_doc.GridStyleSecond ?? new GridStyle() { ShowGrid = false });
        Grid2Controller = grid2;
        Grid2Name = GridName(_doc.PlaneID.InPlaneAxisNumber2);

        var background = new BrushControllerAdvanced() { UseDocumentCopy = UseDocument.Directly };
        background.InitializeDocument(_doc.Background ?? BrushX.Empty);
        Current.Gui.FindAndAttachControlTo(background);
        BackgroundController = background;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_grid1.Apply(disposeController))
        return ApplyEnd(false, disposeController);
      if (!_grid2.Apply(disposeController))
        return ApplyEnd(false, disposeController);
      if (!_background.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      _doc.GridStyleFirst = (GridStyle)_grid1.ModelObject;
      _doc.GridStyleSecond = (GridStyle)_grid2.ModelObject;
      var backBrush = (BrushX)_background.ModelObject;
      _doc.Background = backBrush.IsVisible ? backBrush : null;

      return ApplyEnd(true, disposeController);
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
