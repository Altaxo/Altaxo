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
using System.Collections.ObjectModel;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D;
using Altaxo.Units;
using AUL = Altaxo.Units.Length;

namespace Altaxo.Gui.Graph.Graph3D
{
  public interface IGridPartitioningView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IGridPartitioningView))]
  [UserControllerForObject(typeof(GridPartitioning))]
  public class GridPartitioningController : MVCANControllerEditOriginalDocBase<GridPartitioning, IGridPartitioningView>
  {
    private QuantityWithUnitGuiEnvironment _xSizeEnvironment;
    private QuantityWithUnitGuiEnvironment _ySizeEnvironment;
    private QuantityWithUnitGuiEnvironment _zSizeEnvironment;

    private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("Relative X-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("Relative Y-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerZSizeUnit = new ChangeableRelativePercentUnit("Relative Z-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));

    private VectorD3D _parentLayerSize;

    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    public QuantityWithUnitGuiEnvironment XPartitionEnvironment => _xSizeEnvironment;



    public QuantityWithUnitGuiEnvironment YPartitionEnvironment => _ySizeEnvironment;

    public QuantityWithUnitGuiEnvironment ZPartitionEnvironment => _zSizeEnvironment;

    /// <summary>Sets the default x quantity, i.e. the quantity that is used if the user inserts a new item in the XPartition.</summary>
    private DimensionfulQuantity _defaultXQuantity;

    public DimensionfulQuantity DefaultXQuantity
    {
      get => _defaultXQuantity;
      set
      {
        if (!(_defaultXQuantity == value))
        {
          _defaultXQuantity = value;
          OnPropertyChanged(nameof(DefaultXQuantity));
        }
      }
    }


    /// <summary>Sets the default y quantity, i.e. the quantity that is used if the user inserts a new item in the YPartition.</summary>

    private DimensionfulQuantity _defaultYQuantity;

    public DimensionfulQuantity DefaultYQuantity
    {
      get => _defaultYQuantity;
      set
      {
        if (!(_defaultYQuantity == value))
        {
          _defaultYQuantity = value;
          OnPropertyChanged(nameof(DefaultYQuantity));
        }
      }
    }


    /// <summary>Sets the default z quantity, i.e. the quantity that is used if the user inserts a new item in the YPartition.</summary>

    private DimensionfulQuantity _defaultZQuantity;

    public DimensionfulQuantity DefaultZQuantity
    {
      get => _defaultZQuantity;
      set
      {
        if (!(_defaultZQuantity == value))
        {
          _defaultZQuantity = value;
          OnPropertyChanged(nameof(DefaultZQuantity));
        }
      }
    }


    public ObservableCollection<DimensionfulQuantity> XPartitionValues { get; } = new ObservableCollection<DimensionfulQuantity>();

    public ObservableCollection<DimensionfulQuantity> YPartitionValues { get; } = new ObservableCollection<DimensionfulQuantity>();

    public ObservableCollection<DimensionfulQuantity> ZPartitionValues { get; } = new ObservableCollection<DimensionfulQuantity>();

    #endregion

    public override void Dispose(bool isDisposing)
    {
      XPartitionValues.Clear();
      YPartitionValues.Clear();
      ZPartitionValues.Clear();
      _xSizeEnvironment = null;
      _ySizeEnvironment = null;
      _zSizeEnvironment = null;
      _percentLayerXSizeUnit = null;
      _percentLayerYSizeUnit = null;
      _percentLayerZSizeUnit = null;

      base.Dispose(isDisposing);
    }

    public override bool InitializeDocument(params object[] args)
    {
      if (args.Length < 2)
        return false;
      if (!(args[0] is GridPartitioning))
        return false;

      if (args[1] is VectorD3D)
        _parentLayerSize = (VectorD3D)args[1];
      else if (args[1] is HostLayer)
        _parentLayerSize = ((HostLayer)args[1]).Size;
      else
        _parentLayerSize = new VectorD3D(600, 400, 400);

      return base.InitializeDocument(args);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _percentLayerXSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentLayerSize.X, AUL.Point.Instance);
        _percentLayerYSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentLayerSize.Y, AUL.Point.Instance);
        _percentLayerZSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentLayerSize.Z, AUL.Point.Instance);
        _xSizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerXSizeUnit);
        _ySizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerYSizeUnit);
        _zSizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerZSizeUnit);


        foreach (var xp in _doc.XPartitioning)
          XPartitionValues.Add(xp.IsAbsolute ? new DimensionfulQuantity(xp.Value, AUL.Point.Instance) : new DimensionfulQuantity(xp.Value * 100, _percentLayerXSizeUnit));

        foreach (var yp in _doc.YPartitioning)
          YPartitionValues.Add(yp.IsAbsolute ? new DimensionfulQuantity(yp.Value, AUL.Point.Instance) : new DimensionfulQuantity(yp.Value * 100, _percentLayerYSizeUnit));

        foreach (var zp in _doc.ZPartitioning)
          ZPartitionValues.Add(zp.IsAbsolute ? new DimensionfulQuantity(zp.Value, AUL.Point.Instance) : new DimensionfulQuantity(zp.Value * 100, _percentLayerZSizeUnit));


        DefaultXQuantity = new DimensionfulQuantity(0, _percentLayerXSizeUnit);
        DefaultYQuantity = new DimensionfulQuantity(0, _percentLayerYSizeUnit);
        DefaultZQuantity = new DimensionfulQuantity(0, _percentLayerZSizeUnit);
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.XPartitioning.Clear();
      _doc.YPartitioning.Clear();
      _doc.ZPartitioning.Clear();

      foreach (var val in XPartitionValues)
      {
        if (object.ReferenceEquals(val.Unit, _percentLayerXSizeUnit))
          _doc.XPartitioning.Add(RADouble.NewRel(val.Value / 100));
        else
          _doc.XPartitioning.Add(RADouble.NewAbs(val.AsValueIn(AUL.Point.Instance)));
      }

      foreach (var val in YPartitionValues)
      {
        if (object.ReferenceEquals(val.Unit, _percentLayerYSizeUnit))
          _doc.YPartitioning.Add(RADouble.NewRel(val.Value / 100));
        else
          _doc.YPartitioning.Add(RADouble.NewAbs(val.AsValueIn(AUL.Point.Instance)));
      }

      foreach (var val in ZPartitionValues)
      {
        if (object.ReferenceEquals(val.Unit, _percentLayerZSizeUnit))
          _doc.ZPartitioning.Add(RADouble.NewRel(val.Value / 100));
        else
          _doc.ZPartitioning.Add(RADouble.NewAbs(val.AsValueIn(AUL.Point.Instance)));
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
