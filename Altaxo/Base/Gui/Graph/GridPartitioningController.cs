#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Collections.ObjectModel;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Units;
using AUL = Altaxo.Units.Length;

namespace Altaxo.Gui.Graph
{
  public interface IGridPartitioningView
  {
    QuantityWithUnitGuiEnvironment XPartitionEnvironment { set; }

    QuantityWithUnitGuiEnvironment YPartitionEnvironment { set; }

    /// <summary>Sets the default x quantity, i.e. the quantity that is used if the user inserts a new item in the XPartition.</summary>
    DimensionfulQuantity DefaultXQuantity { set; }

    /// <summary>Sets the default y quantity, i.e. the quantity that is used if the user inserts a new item in the YPartition.</summary>
    DimensionfulQuantity DefaultYQuantity { set; }

    ObservableCollection<DimensionfulQuantity> ColumnCollection { set; }

    ObservableCollection<DimensionfulQuantity> RowCollection { set; }
  }

  [ExpectedTypeOfView(typeof(IGridPartitioningView))]
  [UserControllerForObject(typeof(GridPartitioning))]
  public class GridPartitioningController : MVCANControllerEditOriginalDocBase<GridPartitioning, IGridPartitioningView>
  {
    private ObservableCollection<DimensionfulQuantity> _columnCollection;
    private ObservableCollection<DimensionfulQuantity> _rowCollection;

    private QuantityWithUnitGuiEnvironment _xSizeEnvironment;
    private QuantityWithUnitGuiEnvironment _ySizeEnvironment;

    private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("Relative X-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("Relative Y-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));

    private PointD2D _parentLayerSize;

    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _columnCollection = null;
      _rowCollection = null;
      _xSizeEnvironment = null;
      _ySizeEnvironment = null;
      _percentLayerXSizeUnit = null;
      _percentLayerYSizeUnit = null;

      base.Dispose(isDisposing);
    }

    public override bool InitializeDocument(params object[] args)
    {
      if (args.Length < 2)
        return false;
      if (!(args[0] is GridPartitioning))
        return false;

      if (args[1] is PointD2D)
        _parentLayerSize = (PointD2D)args[1];
      else if (args[1] is HostLayer)
        _parentLayerSize = ((HostLayer)args[1]).Size;
      else
        _parentLayerSize = new PointD2D(600, 400);

      return base.InitializeDocument(args);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _percentLayerXSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentLayerSize.X, AUL.Point.Instance);
        _percentLayerYSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentLayerSize.Y, AUL.Point.Instance);
        _xSizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerXSizeUnit);
        _ySizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerYSizeUnit);

        _columnCollection = new ObservableCollection<DimensionfulQuantity>();
        _rowCollection = new ObservableCollection<DimensionfulQuantity>();

        foreach (var xp in _doc.XPartitioning)
          _columnCollection.Add(xp.IsAbsolute ? new DimensionfulQuantity(xp.Value, AUL.Point.Instance) : new DimensionfulQuantity(xp.Value * 100, _percentLayerXSizeUnit));

        foreach (var yp in _doc.YPartitioning)
          _rowCollection.Add(yp.IsAbsolute ? new DimensionfulQuantity(yp.Value, AUL.Point.Instance) : new DimensionfulQuantity(yp.Value * 100, _percentLayerYSizeUnit));
      }
      if (_view is not null)
      {
        _view.XPartitionEnvironment = _xSizeEnvironment;
        _view.YPartitionEnvironment = _ySizeEnvironment;
        _view.DefaultXQuantity = new DimensionfulQuantity(0, _percentLayerXSizeUnit);
        _view.DefaultYQuantity = new DimensionfulQuantity(0, _percentLayerYSizeUnit);

        _view.ColumnCollection = _columnCollection;
        _view.RowCollection = _rowCollection;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.XPartitioning.Clear();
      _doc.YPartitioning.Clear();

      foreach (var val in _columnCollection)
      {
        if (object.ReferenceEquals(val.Unit, _percentLayerXSizeUnit))
          _doc.XPartitioning.Add(RADouble.NewRel(val.Value / 100));
        else
          _doc.XPartitioning.Add(RADouble.NewAbs(val.AsValueIn(AUL.Point.Instance)));
      }

      foreach (var val in _rowCollection)
      {
        if (object.ReferenceEquals(val.Unit, _percentLayerYSizeUnit))
          _doc.YPartitioning.Add(RADouble.NewRel(val.Value / 100));
        else
          _doc.YPartitioning.Add(RADouble.NewAbs(val.AsValueIn(AUL.Point.Instance)));
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
