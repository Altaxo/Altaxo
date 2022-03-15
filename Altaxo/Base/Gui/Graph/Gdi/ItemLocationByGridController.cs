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
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi
{
  public interface IItemLocationByGridView : IDataContextAwareView
  {
  }

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

    private DimensionfulQuantity _rotation;

    public DimensionfulQuantity Rotation
    {
      get => _rotation;
      set
      {
        if (!(_rotation == value))
        {
          _rotation = value;
          OnPropertyChanged(nameof(Rotation));
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

    private bool _forceFitIntoCell;

    public bool ForceFitIntoCell
    {
      get => _forceFitIntoCell;
      set
      {
        if (!(_forceFitIntoCell == value))
        {
          _forceFitIntoCell = value;
          OnPropertyChanged(nameof(ForceFitIntoCell));
        }
      }
    }

    private double _gridRow;

    public double GridRow
    {
      get => _gridRow;
      set
      {
        if (!(_gridRow == value))
        {
          _gridRow = value;
          OnPropertyChanged(nameof(GridRow));
        }
      }
    }

    private double _gridRowSpan;

    public double GridRowSpan
    {
      get => _gridRowSpan;
      set
      {
        if (!(_gridRowSpan == value))
        {
          _gridRowSpan = value;
          OnPropertyChanged(nameof(GridRowSpan));
        }
      }
    }

    private double _gridColumn;

    public double GridColumn
    {
      get => _gridColumn;
      set
      {
        if (!(_gridColumn == value))
        {
          _gridColumn = value;
          OnPropertyChanged(nameof(GridColumn));
        }
      }
    }
    private double _gridColumnSpan;

    public double GridColumnSpan
    {
      get => _gridColumnSpan;
      set
      {
        if (!(_gridColumnSpan == value))
        {
          _gridColumnSpan = value;
          OnPropertyChanged(nameof(GridColumnSpan));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        Rotation = new DimensionfulQuantity(_doc.Rotation, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);
        ShearX = new DimensionfulQuantity(_doc.ShearX, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ShearEnvironment.DefaultUnit);
        ScaleX = new DimensionfulQuantity(_doc.ScaleX, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);
        ScaleY = new DimensionfulQuantity(_doc.ScaleY, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ScaleEnvironment.DefaultUnit);
        ForceFitIntoCell = _doc.ForceFitIntoCell;
        GridColumn = DocToUserPosition(_doc.GridColumn);
        GridRow = DocToUserPosition(_doc.GridRow);
        GridColumnSpan = DocToUserSize(_doc.GridColumnSpan);
        GridRowSpan = DocToUserSize(_doc.GridRowSpan);
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc.GridColumn = UserToDocPosition(GridColumn);
        _doc.GridRow = UserToDocPosition(GridRow);
        _doc.GridColumnSpan = UserToDocSize(GridColumnSpan);
        _doc.GridRowSpan = UserToDocSize(GridRowSpan);
        _doc.Rotation = Rotation.AsValueIn(Altaxo.Units.Angle.Degree.Instance);
        _doc.ShearX = ShearX.AsValueInSIUnits;
        _doc.ScaleX = ScaleX.AsValueInSIUnits;
        _doc.ScaleY = ScaleY.AsValueInSIUnits;

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
