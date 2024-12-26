using System.Collections.Generic;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Science.Signals;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Signals
{
  public interface IFourPointStepEvaluationToolMouseHandlerOptionsView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(FourPointStepEvaluationToolMouseHandlerOptions))]
  [ExpectedTypeOfView(typeof(IFourPointStepEvaluationToolMouseHandlerOptionsView))]
  public class FourPointStepEvaluationToolMouseHandlerOptionsController : MVCANControllerEditImmutableDocBase<FourPointStepEvaluationToolMouseHandlerOptions, IFourPointStepEvaluationToolMouseHandlerOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(PenController, () => PenController = null!);
    }

    #region Bindings

    private bool _useRegressionForLeftAndRightLine;

    public bool UseRegressionForLeftAndRightLine
    {
      get => _useRegressionForLeftAndRightLine;
      set
      {
        if (!(_useRegressionForLeftAndRightLine == value))
        {
          _useRegressionForLeftAndRightLine = value;
          OnPropertyChanged(nameof(UseRegressionForLeftAndRightLine));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment LevelEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _MiddleRegressionLowerLevel;

    public DimensionfulQuantity MiddleRegressionLowerLevel
    {
      get => _MiddleRegressionLowerLevel;
      set
      {
        if (!(_MiddleRegressionLowerLevel == value))
        {
          _MiddleRegressionLowerLevel = value;
          OnPropertyChanged(nameof(MiddleRegressionLowerLevel));
        }
      }
    }

    private DimensionfulQuantity _MiddleRegressionUpperLevel;

    public DimensionfulQuantity MiddleRegressionUpperLevel
    {
      get => _MiddleRegressionUpperLevel;
      set
      {
        if (!(_MiddleRegressionUpperLevel == value))
        {
          _MiddleRegressionUpperLevel = value;
          OnPropertyChanged(nameof(MiddleRegressionUpperLevel));
        }
      }
    }

    private DimensionfulQuantity _MiddleLineOverlap;

    public DimensionfulQuantity MiddleLineOverlap
    {
      get => _MiddleLineOverlap;
      set
      {
        if (!(_MiddleLineOverlap == value))
        {
          _MiddleLineOverlap = value;
          OnPropertyChanged(nameof(MiddleLineOverlap));
        }
      }
    }

    private ColorTypeThicknessPenController _penController;

    public ColorTypeThicknessPenController PenController
    {
      get => _penController;
      set
      {
        if (!(_penController == value))
        {
          _penController?.Dispose();
          _penController = value;
          OnPropertyChanged(nameof(PenController));
        }
      }
    }





    #endregion


    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        UseRegressionForLeftAndRightLine = _doc.UseRegressionForLeftAndRightLine;
        MiddleRegressionLowerLevel = new DimensionfulQuantity(_doc.MiddleRegressionLevels.LowerLevel, Altaxo.Units.Dimensionless.Unity.Instance);
        MiddleRegressionUpperLevel = new DimensionfulQuantity(_doc.MiddleRegressionLevels.UpperLevel, Altaxo.Units.Dimensionless.Unity.Instance);
        MiddleLineOverlap = new DimensionfulQuantity(_doc.MiddleLineOverlap, Altaxo.Units.Dimensionless.Unity.Instance);
        PenController = new ColorTypeThicknessPenController(_doc.LinePen);
      }
    }
    public override bool Apply(bool disposeController)
    {
      var middleRegressionLowerLevel = MiddleRegressionLowerLevel.AsValueInSIUnits;
      var middleRegressionUpperLevel = MiddleRegressionUpperLevel.AsValueInSIUnits;

      if (!(middleRegressionLowerLevel >= 0 && middleRegressionLowerLevel <= 1))
      {
        Current.Gui.ErrorMessageBox("The value of the lower level of the middle regression must be between 0 and 1.");
        return ApplyEnd(false, disposeController);
      }
      if (!(middleRegressionUpperLevel >= 0 && middleRegressionUpperLevel <= 1))
      {
        Current.Gui.ErrorMessageBox("The value of the upper level of the middle regression must be between 0 and 1.");
        return ApplyEnd(false, disposeController);
      }

      if (!(middleRegressionLowerLevel < middleRegressionUpperLevel))
      {
        Current.Gui.ErrorMessageBox("The value of the lower level of the middle regression must be smaller than the value of the upper level.");
        return ApplyEnd(false, disposeController);
      }

      var middleLineOverlap = MiddleLineOverlap.AsValueInSIUnits;


      _doc = new FourPointStepEvaluationToolMouseHandlerOptions()
      {
        UseRegressionForLeftAndRightLine = UseRegressionForLeftAndRightLine,
        MiddleRegressionLevels = (middleRegressionLowerLevel, middleRegressionUpperLevel),
        MiddleLineOverlap = middleLineOverlap,
        LinePen = PenController.Pen,
      };

      return ApplyEnd(true, disposeController);

    }

  }
}
