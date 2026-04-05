using System.Collections.Generic;
using System.Windows.Input;
using Altaxo.Geometry;
using Altaxo.Units;
using AUL = Altaxo.Units.Length;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  /// <summary>
  /// View interface for editing 2D anchoring.
  /// </summary>
  public interface IAnchoringView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Immutable model for 2D anchoring settings.
  /// </summary>
  public record AnchoringModel
  {
    /// <summary>
    /// Gets the x pivot.
    /// </summary>
    public RADouble PivotX { get; init; }
    /// <summary>
    /// Gets the y pivot.
    /// </summary>
    public RADouble PivotY { get; init; }

    /// <summary>
    /// Gets the reference size.
    /// </summary>
    public PointD2D ReferenceSize { get; init; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Title { get; init; }
  }

  /// <summary>
  /// Controller for editing 2D anchoring.
  /// </summary>
  public class AnchoringController : MVCANControllerEditImmutableDocBase<AnchoringModel, IAnchoringView>
  {

    private QuantityWithUnitGuiEnvironment _xSizeEnvironment, _ySizeEnvironment;

    private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("% X-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("% Y-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));

    /// <summary>
    /// Initializes a new instance of the <see cref="AnchoringController"/> class.
    /// </summary>
    public AnchoringController()
    {
      CmdSwitchToRadioView = new RelayCommand(() => IsRadioViewVisible = true);
      CmdSwitchToNumericView = new RelayCommand(() => IsRadioViewVisible = false);
    }

    /// <inheritdoc/>
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

    /// <summary>
    /// Gets or sets the command to switch to radio view.
    /// </summary>
    public ICommand CmdSwitchToRadioView { get; set; }
    /// <summary>
    /// Gets or sets the command to switch to numeric view.
    /// </summary>
    public ICommand CmdSwitchToNumericView { get; set; }

    /// <summary>
    /// Gets or sets the radio button command.
    /// </summary>
    public ICommand CmdRadioButtion { get; init; }


    /// <summary>
    /// Gets the title of the document.
    /// </summary>
    public string? Title => _doc?.Title;


    private bool _isRadioViewVisible;

    /// <summary>
    /// Gets or sets a value indicating whether the radio view is visible.
    /// </summary>
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

    /// <summary>
    /// Gets a value indicating whether switching to radio view is enabled.
    /// </summary>
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
    /// <summary>
    /// Gets or sets a value indicating whether the numeric view is visible.
    /// </summary>
    public bool IsNumericViewVisible
    {
      get => !_isRadioViewVisible;
      set
      {
        IsRadioViewVisible = !value;
      }
    }





    /// <summary>
    /// Gets the environment for the X pivot quantity.
    /// </summary>
    public QuantityWithUnitGuiEnvironment PivotXEnvironment
    {
      get => _xSizeEnvironment;
    }


    private DimensionfulQuantity _pivotXQuantity;

    /// <summary>
    /// Gets or sets the quantity for the X pivot.
    /// </summary>
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


    /// <summary>
    /// Gets the environment for the Y pivot quantity.
    /// </summary>
    public QuantityWithUnitGuiEnvironment PivotYEnvironment
    {
      get => _ySizeEnvironment;
    }

    private DimensionfulQuantity _pivotYQuantity;

    /// <summary>
    /// Gets or sets the quantity for the Y pivot.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the pivot is anchored to the left top.
    /// </summary>
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
    /// <summary>
    /// Gets or sets a value indicating whether the pivot is anchored to the left center.
    /// </summary>
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
    /// <summary>
    /// Gets or sets a value indicating whether the pivot is anchored to the left bottom.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the pivot is anchored to the center top.
    /// </summary>
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
    /// <summary>
    /// Gets or sets a value indicating whether the pivot is anchored to the center center.
    /// </summary>
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
    /// <summary>
    /// Gets or sets a value indicating whether the pivot is anchored to the center bottom.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the pivot is anchored to the right top.
    /// </summary>
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
    /// <summary>
    /// Gets or sets a value indicating whether the pivot is anchored to the right center.
    /// </summary>
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
    /// <summary>
    /// Gets or sets a value indicating whether the pivot is anchored to the right bottom.
    /// </summary>
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

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = new AnchoringModel() { ReferenceSize = _doc.ReferenceSize, Title = _doc.Title, PivotX = PivotX, PivotY = PivotY };
      return ApplyEnd(true, disposeController);
    }

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }
  }
}
