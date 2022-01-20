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
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi
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

    private DimensionfulQuantity _ShearX;

    public DimensionfulQuantity ShearX
    {
      get => _ShearX;
      set
      {
        if (!(_ShearX == value))
        {
          _ShearX = value;
          OnPropertyChanged(nameof(ShearX));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment ScaleEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _ScaleX;

    public DimensionfulQuantity ScaleX
    {
      get => _ScaleX;
      set
      {
        if (!(_ScaleX == value))
        {
          _ScaleX = value;
          OnPropertyChanged(nameof(ScaleX));
        }
      }
    }


    private DimensionfulQuantity _ScaleY;

    public DimensionfulQuantity ScaleY
    {
      get => _ScaleY;
      set
      {
        if (!(_ScaleY == value))
        {
          _ScaleY = value;
          OnPropertyChanged(nameof(ScaleY));
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

    private double _GridRow;

    public double GridRow
    {
      get => _GridRow;
      set
      {
        if (!(_GridRow == value))
        {
          _GridRow = value;
          OnPropertyChanged(nameof(GridRow));
        }
      }
    }

    private double _GridRowSpan;

    public double GridRowSpan
    {
      get => _GridRowSpan;
      set
      {
        if (!(_GridRowSpan == value))
        {
          _GridRowSpan = value;
          OnPropertyChanged(nameof(GridRowSpan));
        }
      }
    }

    private double _GridColumn;

    public double GridColumn
    {
      get => _GridColumn;
      set
      {
        if (!(_GridColumn == value))
        {
          _GridColumn = value;
          OnPropertyChanged(nameof(GridColumn));
        }
      }
    }
    private double _GridColumnSpan;

    public double GridColumnSpan
    {
      get => _GridColumnSpan;
      set
      {
        if (!(_GridColumnSpan == value))
        {
          _GridColumnSpan = value;
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
