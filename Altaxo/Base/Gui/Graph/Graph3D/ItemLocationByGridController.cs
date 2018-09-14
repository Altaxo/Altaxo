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
using Altaxo.Graph.Graph3D;

namespace Altaxo.Gui.Graph.Graph3D
{
  #region Interfaces

  public interface IItemLocationByGridView
  {
    double GridPosX { get; set; }

    double GridPosY { get; set; }

    double GridPosZ { get; set; }

    double GridSpanX { get; set; }

    double GridSpanY { get; set; }

    double GridSpanZ { get; set; }

    double RotationX { get; set; }

    double RotationY { get; set; }

    double RotationZ { get; set; }

    double ShearX { get; set; }

    double ShearY { get; set; }

    double ShearZ { get; set; }

    double ScaleX { get; set; }

    double ScaleY { get; set; }

    double ScaleZ { get; set; }

    bool ForceFitIntoCell { get; set; }
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for LayerPositionController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IItemLocationByGridView))]
  [UserControllerForObject(typeof(ItemLocationByGrid))]
  public class ItemLocationByGridController : MVCANControllerEditOriginalDocBase<ItemLocationByGrid, IItemLocationByGridView>
  {
    private GridPartitioning _parentLayerGrid;

    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override bool InitializeDocument(params object[] args)
    {
      if (args.Length < 2)
        return false;
      if (!(args[1] is GridPartitioning))
        return false;
      _parentLayerGrid = (GridPartitioning)args[1];

      return base.InitializeDocument(args);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
      }
      if (null != _view)
      {
        _view.GridPosX = DocToUserPosition(_doc.GridPosX);
        _view.GridPosY = DocToUserPosition(_doc.GridPosY);
        _view.GridPosZ = DocToUserPosition(_doc.GridPosZ);

        _view.GridSpanX = DocToUserSize(_doc.GridSpanX);
        _view.GridSpanY = DocToUserSize(_doc.GridSpanY);
        _view.GridSpanZ = DocToUserSize(_doc.GridSpanZ);

        _view.RotationX = _doc.RotationX;
        _view.RotationY = _doc.RotationY;
        _view.RotationZ = _doc.RotationZ;

        _view.ShearX = _doc.ShearX;
        _view.ShearY = _doc.ShearY;
        _view.ShearZ = _doc.ShearZ;

        _view.ScaleX = _doc.ScaleX;
        _view.ScaleY = _doc.ScaleY;
        _view.ScaleZ = _doc.ScaleZ;

        _view.ForceFitIntoCell = _doc.ForceFitIntoCell;
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc.GridPosX = UserToDocPosition(_view.GridPosX);
        _doc.GridPosY = UserToDocPosition(_view.GridPosY);
        _doc.GridPosZ = UserToDocPosition(_view.GridPosZ);

        _doc.GridSpanX = UserToDocSize(_view.GridSpanX);
        _doc.GridSpanY = UserToDocSize(_view.GridSpanY);
        _doc.GridSpanZ = UserToDocSize(_view.GridSpanZ);

        _doc.RotationX = _view.RotationX;
        _doc.RotationY = _view.RotationY;
        _doc.RotationZ = _view.RotationZ;

        _doc.ShearX = _view.ShearX;
        _doc.ShearY = _view.ShearY;
        _doc.ShearZ = _view.ShearZ;

        _doc.ScaleX = _view.ScaleX;
        _doc.ScaleY = _view.ScaleY;
        _doc.ScaleZ = _view.ScaleZ;

        _doc.ForceFitIntoCell = _view.ForceFitIntoCell;
      }
      catch (Exception)
      {
        return false; // indicate that something failed
      }
      return ApplyEnd(true, disposeController);
    }

    private static double DocToUserPosition(double x)
    {
      return 1 + (x - 1) / 2;
    } // 1->1, 3->2, 5->3 usw.

    private static double DocToUserSize(double x)
    {
      return 1 + (x - 1) / 2;
    }

    private static double UserToDocPosition(double x)
    {
      return 1 + 2 * (x - 1);
    } // 1 -> 1, 2->3, 3->5 usw.

    private static double UserToDocSize(double x)
    {
      return 1 + 2 * (x - 1);
    } // 1 -> 1, 2->3, 3->5 usw.
  }
}
