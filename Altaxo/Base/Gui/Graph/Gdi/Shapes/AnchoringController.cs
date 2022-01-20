using System.Collections.Generic;
using System.Windows.Input;
using Altaxo.Geometry;
using Altaxo.Units;
using AUL = Altaxo.Units.Length;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  public interface IAnchoringView : IDataContextAwareView
  {
  }

  public record AnchoringModel
  {
    public RADouble PivotX { get; init; }
    public RADouble PivotY { get; init; }

    public PointD2D ReferenceSize { get; init; }

    public string Title { get; init; }
  }

  public class AnchoringController : MVCANControllerEditImmutableDocBase<AnchoringModel, IAnchoringView>
  {

    private QuantityWithUnitGuiEnvironment _xSizeEnvironment, _ySizeEnvironment;

    private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("% X-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("% Y-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));

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
        _xSizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerXSizeUnit);
        _ySizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerYSizeUnit);

        PivotXQuantity = _doc.PivotX.IsAbsolute ? new DimensionfulQuantity(_doc.PivotX.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.PivotX.Value * 100, _percentLayerXSizeUnit);
        PivotYQuantity = _doc.PivotY.IsAbsolute ? new DimensionfulQuantity(_doc.PivotY.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.PivotY.Value * 100, _percentLayerYSizeUnit);
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
          OnPropertyChanged(nameof(IsLeftTop));
          OnPropertyChanged(nameof(IsLeftCenter));
          OnPropertyChanged(nameof(IsLeftBottom));
          OnPropertyChanged(nameof(IsCenterTop));
          OnPropertyChanged(nameof(IsCenterCenter));
          OnPropertyChanged(nameof(IsCenterBottom));
          OnPropertyChanged(nameof(IsRightTop));
          OnPropertyChanged(nameof(IsRightCenter));
          OnPropertyChanged(nameof(IsRightBottom));
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
          OnPropertyChanged(nameof(IsLeftTop));
          OnPropertyChanged(nameof(IsLeftCenter));
          OnPropertyChanged(nameof(IsLeftBottom));
          OnPropertyChanged(nameof(IsCenterTop));
          OnPropertyChanged(nameof(IsCenterCenter));
          OnPropertyChanged(nameof(IsCenterBottom));
          OnPropertyChanged(nameof(IsRightTop));
          OnPropertyChanged(nameof(IsRightCenter));
          OnPropertyChanged(nameof(IsRightBottom));

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

    public bool IsLeftTop
    {
      get => PivotX.IsRelative && PivotY.IsRelative && PivotX.Value == 0 && PivotY.Value == 0;
      set
      {
        if (value)
        {
          PivotX = RADouble.NewRel(0);
          PivotY = RADouble.NewRel(0);
        }
      }
    }
    public bool IsLeftCenter
    {
      get => PivotX.IsRelative && PivotY.IsRelative && PivotX.Value == 0 && PivotY.Value == 0.5;
      set
      {
        if (value)
        {
          PivotX = RADouble.NewRel(0);
          PivotY = RADouble.NewRel(0.5);
        }
      }
    }
    public bool IsLeftBottom
    {
      get => PivotX.IsRelative && PivotY.IsRelative && PivotX.Value == 0 && PivotY.Value == 1;
      set
      {
        if (value)
        {
          PivotX = RADouble.NewRel(0);
          PivotY = RADouble.NewRel(1);
        }
      }
    }

    public bool IsCenterTop
    {
      get => PivotX.IsRelative && PivotY.IsRelative && PivotX.Value == 0.5 && PivotY.Value == 0;
      set
      {
        if (value)
        {
          PivotX = RADouble.NewRel(0.5);
          PivotY = RADouble.NewRel(0);
        }
      }
    }
    public bool IsCenterCenter
    {
      get => PivotX.IsRelative && PivotY.IsRelative && PivotX.Value == 0.5 && PivotY.Value == 0.5;
      set
      {
        if (value)
        {
          PivotX = RADouble.NewRel(0.5);
          PivotY = RADouble.NewRel(0.5);
        }
      }
    }
    public bool IsCenterBottom
    {
      get => PivotX.IsRelative && PivotY.IsRelative && PivotX.Value == 0.5 && PivotY.Value == 1;
      set
      {
        if (value)
        {
          PivotX = RADouble.NewRel(0.5);
          PivotY = RADouble.NewRel(1);
        }
      }
    }

    public bool IsRightTop
    {
      get => PivotX.IsRelative && PivotY.IsRelative && PivotX.Value == 1 && PivotY.Value == 0;
      set
      {
        if (value)
        {
          PivotX = RADouble.NewRel(1);
          PivotY = RADouble.NewRel(0);
        }
      }
    }
    public bool IsRightCenter
    {
      get => PivotX.IsRelative && PivotY.IsRelative && PivotX.Value == 1 && PivotY.Value == 0.5;
      set
      {
        if (value)
        {
          PivotX = RADouble.NewRel(1);
          PivotY = RADouble.NewRel(0.5);
        }
      }
    }
    public bool IsRightBottom
    {
      get => PivotX.IsRelative && PivotY.IsRelative && PivotX.Value == 1 && PivotY.Value == 1;
      set
      {
        if (value)
        {
          PivotX = RADouble.NewRel(1);
          PivotY = RADouble.NewRel(1);
        }
      }
    }



    #endregion

    public override bool Apply(bool disposeController)
    {
      _doc = new AnchoringModel() { ReferenceSize = _doc.ReferenceSize, Title = _doc.Title, PivotX = PivotX, PivotY = PivotY };
      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }
  }
}
