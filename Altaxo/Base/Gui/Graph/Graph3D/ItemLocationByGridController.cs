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

#nullable disable
using System;
using Altaxo.Graph.Graph3D;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Graph3D
{
  #region Interfaces

  public interface IItemLocationByGridView : IDataContextAwareView
  {
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

    #region Bindings

    public QuantityWithUnitGuiEnvironment RotationEnvironment => AngleEnvironment.Instance;

    private DimensionfulQuantity _rotationX;

    public DimensionfulQuantity RotationX
    {
      get => _rotationX;
      set
      {
        if (!(_rotationX == value))
        {
          _rotationX = value;
          OnPropertyChanged(nameof(RotationX));
        }
      }
    }

    private DimensionfulQuantity _rotationY;

    public DimensionfulQuantity RotationY
    {
      get => _rotationY;
      set
      {
        if (!(_rotationY == value))
        {
          _rotationY = value;
          OnPropertyChanged(nameof(RotationY));
        }
      }
    }

    private DimensionfulQuantity _rotationZ;

    public DimensionfulQuantity RotationZ
    {
      get => _rotationZ;
      set
      {
        if (!(_rotationZ == value))
        {
          _rotationZ = value;
          OnPropertyChanged(nameof(RotationZ));
        }
      }
    }



    public QuantityWithUnitGuiEnvironment ShearEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _shearX;

    public DimensionfulQuantity ShearX
    {
      get => _shearX;
      set
      {
        if (!(_shearX == value))
        {
          _shearX = value;
          OnPropertyChanged(nameof(ShearX));
        }
      }
    }

    private DimensionfulQuantity _shearY;

    public DimensionfulQuantity ShearY
    {
      get => _shearY;
      set
      {
        if (!(_shearY == value))
        {
          _shearY = value;
          OnPropertyChanged(nameof(ShearY));
        }
      }
    }

    private DimensionfulQuantity _shearZ;

    public DimensionfulQuantity ShearZ
    {
      get => _shearZ;
      set
      {
        if (!(_shearZ == value))
        {
          _shearZ = value;
          OnPropertyChanged(nameof(ShearZ));
        }
      }
    }



    public QuantityWithUnitGuiEnvironment ScaleEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _scaleX;

    public DimensionfulQuantity ScaleX
    {
      get => _scaleX;
      set
      {
        if (!(_scaleX == value))
        {
          _scaleX = value;
          OnPropertyChanged(nameof(ScaleX));
        }
      }
    }


    private DimensionfulQuantity _scaleY;

    public DimensionfulQuantity ScaleY
    {
      get => _scaleY;
      set
      {
        if (!(_scaleY == value))
        {
          _scaleY = value;
          OnPropertyChanged(nameof(ScaleY));
        }
      }
    }

    private DimensionfulQuantity _scaleZ;

    public DimensionfulQuantity ScaleZ
    {
      get => _scaleZ;
      set
      {
        if (!(_scaleZ == value))
        {
          _scaleZ = value;
          OnPropertyChanged(nameof(ScaleZ));
        }
      }
    }


    private bool _ForceFitIntoCell;

    public bool ForceFitIntoCell
    {
      get => _ForceFitIntoCell;
      set
      {
        if (!(_ForceFitIntoCell == value))
        {
          _ForceFitIntoCell = value;
          OnPropertyChanged(nameof(ForceFitIntoCell));
        }
      }
    }



    private double _gridPosX;

    public double GridPosX
    {
      get => _gridPosX;
      set
      {
        if (!(_gridPosX == value))
        {
          _gridPosX = value;
          OnPropertyChanged(nameof(GridPosX));
        }
      }
    }
    private double _gridSpanX;

    public double GridSpanX
    {
      get => _gridSpanX;
      set
      {
        if (!(_gridSpanX == value))
        {
          _gridSpanX = value;
          OnPropertyChanged(nameof(GridSpanX));
        }
      }
    }

    private double _gridPosY;

    public double GridPosY
    {
      get => _gridPosY;
      set
      {
        if (!(_gridPosY == value))
        {
          _gridPosY = value;
          OnPropertyChanged(nameof(GridPosY));
        }
      }
    }

    private double _gridSpanY;

    public double GridSpanY
    {
      get => _gridSpanY;
      set
      {
        if (!(_gridSpanY == value))
        {
          _gridSpanY = value;
          OnPropertyChanged(nameof(GridSpanY));
        }
      }
    }


    private double _gridPosZ;

    public double GridPosZ
    {
      get => _gridPosZ;
      set
      {
        if (!(_gridPosZ == value))
        {
          _gridPosZ = value;
          OnPropertyChanged(nameof(GridPosZ));
        }
      }
    }

    private double _gridSpanZ;

    public double GridSpanZ
    {
      get => _gridSpanZ;
      set
      {
        if (!(_gridSpanZ == value))
        {
          _gridSpanZ = value;
          OnPropertyChanged(nameof(GridSpanZ));
        }
      }
    }


    #endregion


    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {

        GridPosX = DocToUserPosition(_doc.GridPosX);
        GridPosY = DocToUserPosition(_doc.GridPosY);
        GridPosZ = DocToUserPosition(_doc.GridPosZ);

        GridSpanX = DocToUserSize(_doc.GridSpanX);
        GridSpanY = DocToUserSize(_doc.GridSpanY);
        GridSpanZ = DocToUserSize(_doc.GridSpanZ);

        RotationX = new DimensionfulQuantity(_doc.RotationX, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);
        RotationY = new DimensionfulQuantity(_doc.RotationY, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);
        RotationZ = new DimensionfulQuantity(_doc.RotationZ, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);

        ShearX = new DimensionfulQuantity(_doc.ShearX, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ShearEnvironment.DefaultUnit);
        ShearY = new DimensionfulQuantity(_doc.ShearY, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ShearEnvironment.DefaultUnit);
        ShearZ = new DimensionfulQuantity(_doc.ShearZ, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ShearEnvironment.DefaultUnit);

        ScaleX = new DimensionfulQuantity(_doc.ScaleX, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);
        ScaleY = new DimensionfulQuantity(_doc.ScaleY, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);
        ScaleZ = new DimensionfulQuantity(_doc.ScaleZ, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);

        ForceFitIntoCell = _doc.ForceFitIntoCell;
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc.GridPosX = UserToDocPosition(GridPosX);
        _doc.GridPosY = UserToDocPosition(GridPosY);
        _doc.GridPosZ = UserToDocPosition(GridPosZ);

        _doc.GridSpanX = UserToDocSize(GridSpanX);
        _doc.GridSpanY = UserToDocSize(GridSpanY);
        _doc.GridSpanZ = UserToDocSize(GridSpanZ);

        _doc.RotationX = RotationX.AsValueIn(Altaxo.Units.Angle.Degree.Instance);
        _doc.RotationY = RotationY.AsValueIn(Altaxo.Units.Angle.Degree.Instance);
        _doc.RotationZ = RotationZ.AsValueIn(Altaxo.Units.Angle.Degree.Instance);

        _doc.ShearX = ShearX.AsValueInSIUnits;
        _doc.ShearY = ShearY.AsValueInSIUnits;
        _doc.ShearZ = ShearZ.AsValueInSIUnits;

        _doc.ScaleX = ScaleX.AsValueInSIUnits;
        _doc.ScaleY = ScaleY.AsValueInSIUnits;
        _doc.ScaleZ = ScaleZ.AsValueInSIUnits;

        _doc.ForceFitIntoCell = ForceFitIntoCell;
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
