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

using System.Collections.Generic;
using System.Windows.Input;
using Altaxo.Geometry;
using Altaxo.Units;
using AUL = Altaxo.Units.Length;

namespace Altaxo.Gui.Graph.Graph3D.Shapes
{
  public interface IAnchoringView : IDataContextAwareView
  {
  }

  public record AnchoringModel
  {
    public RADouble PivotX { get; init; }
    public RADouble PivotY { get; init; }
    public RADouble PivotZ { get; init; }

    public VectorD3D ReferenceSize { get; init; }

    public string Title { get; init; }
  }

  public class AnchoringController : MVCANControllerEditImmutableDocBase<AnchoringModel, IAnchoringView>
  {

    private QuantityWithUnitGuiEnvironment _xSizeEnvironment, _ySizeEnvironment, _zSizeEnvironment;

    private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("% X-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("% Y-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerZSizeUnit = new ChangeableRelativePercentUnit("% Z-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));

    public AnchoringController()
    {
      CmdSwitchToRadioView = new RelayCommand(() => IsRadioViewVisible = true);
      CmdSwitchToNumericView = new RelayCommand(() => IsRadioViewVisible = false);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _percentLayerXSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_doc.ReferenceSize.X, AUL.Point.Instance);
        _percentLayerYSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_doc.ReferenceSize.Y, AUL.Point.Instance);
        _percentLayerZSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_doc.ReferenceSize.Z, AUL.Point.Instance);

        _xSizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerXSizeUnit);
        _ySizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerYSizeUnit);
        _zSizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerZSizeUnit);

        PivotXQuantity = _doc.PivotX.IsAbsolute ? new DimensionfulQuantity(_doc.PivotX.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.PivotX.Value * 100, _percentLayerXSizeUnit);
        PivotYQuantity = _doc.PivotY.IsAbsolute ? new DimensionfulQuantity(_doc.PivotY.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.PivotY.Value * 100, _percentLayerYSizeUnit);
        PivotZQuantity = _doc.PivotZ.IsAbsolute ? new DimensionfulQuantity(_doc.PivotZ.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.PivotZ.Value * 100, _percentLayerZSizeUnit);
        IsRadioViewVisible = true;
      }
    }

    #region Bindings

    public ICommand CmdSwitchToRadioView { get; set; }
    public ICommand CmdSwitchToNumericView { get; set; }

    public ICommand CmdRadioButtion { get; init; }


    public string? Title => _doc?.Title;


    private bool _isRadioViewVisible;

    public bool IsRadioViewVisible
    {
      get => _isRadioViewVisible;
      set
      {
        if (value == true && !IsSwitchToRadioViewEnabled)
          return;

        if (!(_isRadioViewVisible == value))
        {
          _isRadioViewVisible = value;
          OnPropertyChanged(nameof(IsRadioViewVisible));
          OnPropertyChanged(nameof(IsNumericViewVisible));
        }
      }
    }

    public bool IsSwitchToRadioViewEnabled
    {
      get
      {
        bool useRadioView = true;
        useRadioView &= PivotX.IsRelative && (PivotX.Value == 0 || PivotX.Value == 0.5 || PivotX.Value == 1);
        useRadioView &= PivotY.IsRelative && (PivotY.Value == 0 || PivotY.Value == 0.5 || PivotY.Value == 1);
        return useRadioView;
      }
    }
    public bool IsNumericViewVisible
    {
      get => !_isRadioViewVisible;
      set
      {
        IsRadioViewVisible = !value;
      }
    }

    public QuantityWithUnitGuiEnvironment PivotXEnvironment
    {
      get => _xSizeEnvironment;
    }


    private DimensionfulQuantity _pivotXQuantity;

    public DimensionfulQuantity PivotXQuantity
    {
      get => _pivotXQuantity;
      set
      {
        if (!(_pivotXQuantity == value))
        {
          _pivotXQuantity = value;

          OnPropertyChanged(nameof(PivotXQuantity));
          OnPropertyChanged(nameof(PivotX));
          OnPropertyChanged(nameof(IsSwitchToRadioViewEnabled));
          OnPropertyChanged(nameof(IsNearX));
          OnPropertyChanged(nameof(IsCenterX));
          OnPropertyChanged(nameof(IsFarX));
        }
      }
    }

    private RADouble PivotX
    {
      get
      {
        var value = PivotXQuantity;
        if (object.ReferenceEquals(value.Unit, _percentLayerXSizeUnit))
          return RADouble.NewRel(value.Value / 100);
        else
          return RADouble.NewAbs(value.AsValueIn(AUL.Point.Instance));
      }
      set
      {
        if (!(PivotX == value))
        {
          if (value.IsRelative)
            PivotXQuantity = new DimensionfulQuantity(value.Value * 100, _percentLayerXSizeUnit);
          else
            PivotXQuantity = new DimensionfulQuantity(value.Value, AUL.Point.Instance).AsQuantityIn(PivotXEnvironment.DefaultUnit);
        }
      }
    }


    public QuantityWithUnitGuiEnvironment PivotYEnvironment
    {
      get => _ySizeEnvironment;
    }

    private DimensionfulQuantity _pivotYQuantity;

    public DimensionfulQuantity PivotYQuantity
    {
      get => _pivotYQuantity;
      set
      {
        if (!(_pivotYQuantity == value))
        {
          _pivotYQuantity = value;
          OnPropertyChanged(nameof(PivotYQuantity));
          OnPropertyChanged(nameof(PivotY));
          OnPropertyChanged(nameof(IsSwitchToRadioViewEnabled));
          OnPropertyChanged(nameof(IsNearY));
          OnPropertyChanged(nameof(IsCenterY));
          OnPropertyChanged(nameof(IsFarY));
        }
      }
    }




    private RADouble PivotY
    {
      get
      {
        var value = PivotYQuantity;
        if (object.ReferenceEquals(value.Unit, _percentLayerYSizeUnit))
          return RADouble.NewRel(value.Value / 100);
        else
          return RADouble.NewAbs(value.AsValueIn(AUL.Point.Instance));
      }
      set
      {
        if (!(PivotY == value))
        {
          if (value.IsRelative)
            PivotYQuantity = new DimensionfulQuantity(value.Value * 100, _percentLayerYSizeUnit);
          else
            PivotYQuantity = new DimensionfulQuantity(value.Value, AUL.Point.Instance).AsQuantityIn(PivotYEnvironment.DefaultUnit);
        }
      }
    }

    // z
    public QuantityWithUnitGuiEnvironment PivotZEnvironment
    {
      get => _zSizeEnvironment;
    }

    private DimensionfulQuantity _pivotZQuantity;

    public DimensionfulQuantity PivotZQuantity
    {
      get => _pivotZQuantity;
      set
      {
        if (!(_pivotZQuantity == value))
        {
          _pivotZQuantity = value;
          OnPropertyChanged(nameof(PivotZQuantity));
          OnPropertyChanged(nameof(PivotZ));
          OnPropertyChanged(nameof(IsSwitchToRadioViewEnabled));
          OnPropertyChanged(nameof(IsNearZ));
          OnPropertyChanged(nameof(IsCenterZ));
          OnPropertyChanged(nameof(IsFarZ));
        }
      }
    }




    private RADouble PivotZ
    {
      get
      {
        var value = PivotZQuantity;
        if (object.ReferenceEquals(value.Unit, _percentLayerYSizeUnit))
          return RADouble.NewRel(value.Value / 100);
        else
          return RADouble.NewAbs(value.AsValueIn(AUL.Point.Instance));
      }
      set
      {
        if (!(PivotZ == value))
        {
          if (value.IsRelative)
            PivotZQuantity = new DimensionfulQuantity(value.Value * 100, _percentLayerZSizeUnit);
          else
            PivotZQuantity = new DimensionfulQuantity(value.Value, AUL.Point.Instance).AsQuantityIn(PivotZEnvironment.DefaultUnit);
        }
      }
    }

    public bool IsNearX
    {
      get => PivotX.IsRelative && PivotX.Value == 0;
      set
      {
        if (value)
        {
          PivotX = RADouble.NewRel(0);
        }
      }
    }

    public bool IsCenterX
    {
      get => PivotX.IsRelative && PivotX.Value == 0.5;
      set
      {
        if (value)
        {
          PivotX = RADouble.NewRel(0.5);
        }
      }
    }

    public bool IsFarX
    {
      get => PivotX.IsRelative && PivotX.Value == 1;
      set
      {
        if (value)
        {
          PivotX = RADouble.NewRel(1);
        }
      }
    }

    public bool IsNearY
    {
      get => PivotY.IsRelative && PivotY.Value == 0;
      set
      {
        if (value)
        {
          PivotY = RADouble.NewRel(0);
        }
      }
    }

    public bool IsCenterY
    {
      get => PivotY.IsRelative && PivotY.Value == 0.5;
      set
      {
        if (value)
        {
          PivotY = RADouble.NewRel(0.5);
        }
      }
    }

    public bool IsFarY
    {
      get => PivotY.IsRelative && PivotY.Value == 1;
      set
      {
        if (value)
        {
          PivotY = RADouble.NewRel(1);
        }
      }
    }

    public bool IsNearZ
    {
      get => PivotZ.IsRelative && PivotZ.Value == 0;
      set
      {
        if (value)
        {
          PivotZ = RADouble.NewRel(0);
        }
      }
    }

    public bool IsCenterZ
    {
      get => PivotZ.IsRelative && PivotZ.Value == 0.5;
      set
      {
        if (value)
        {
          PivotZ = RADouble.NewRel(0.5);
        }
      }
    }

    public bool IsFarZ
    {
      get => PivotZ.IsRelative && PivotZ.Value == 1;
      set
      {
        if (value)
        {
          PivotZ = RADouble.NewRel(1);
        }
      }
    }


    #endregion

    public override bool Apply(bool disposeController)
    {
      _doc = new AnchoringModel() { ReferenceSize = _doc.ReferenceSize, Title = _doc.Title, PivotX = PivotX, PivotY = PivotY, PivotZ = PivotZ };
      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }
  }
}
