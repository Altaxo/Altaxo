using System.Collections.Generic;
using Altaxo.Science.Signals;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Signals
{
  public interface IFourPointStepEvaluationOptionsView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(FourPointStepEvaluationOptions))]
  [ExpectedTypeOfView(typeof(IFourPointStepEvaluationOptionsView))]
  public class FourPointStepEvaluationOptionsController : MVCANControllerEditImmutableDocBase<FourPointStepEvaluationOptions, IFourPointStepEvaluationOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
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


    private bool _IncludeOriginalPointsInOutput;

    public bool IncludeOriginalPointsInOutput
    {
      get => _IncludeOriginalPointsInOutput;
      set
      {
        if (!(_IncludeOriginalPointsInOutput == value))
        {
          _IncludeOriginalPointsInOutput = value;
          OnPropertyChanged(nameof(IncludeOriginalPointsInOutput));
        }
      }
    }


    private double _IndexLeftOuter;

    public double IndexLeftOuter
    {
      get => _IndexLeftOuter;
      set
      {
        if (!(_IndexLeftOuter == value))
        {
          _IndexLeftOuter = value;
          OnPropertyChanged(nameof(IndexLeftOuter));
        }
      }
    }

    private double _IndexLeftInner;

    public double IndexLeftInner
    {
      get => _IndexLeftInner;
      set
      {
        if (!(_IndexLeftInner == value))
        {
          _IndexLeftInner = value;
          OnPropertyChanged(nameof(IndexLeftInner));
        }
      }
    }

    private double _IndexRightOuter;

    public double IndexRightOuter
    {
      get => _IndexRightOuter;
      set
      {
        if (!(_IndexRightOuter == value))
        {
          _IndexRightOuter = value;
          OnPropertyChanged(nameof(IndexRightOuter));
        }
      }
    }

    private double _IndexRightInner;

    public double IndexRightInner
    {
      get => _IndexRightInner;
      set
      {
        if (!(_IndexRightInner == value))
        {
          _IndexRightInner = value;
          OnPropertyChanged(nameof(IndexRightInner));
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
        IncludeOriginalPointsInOutput = _doc.IncludeOriginalPointsInOutput;
        IndexLeftOuter = _doc.IndexLeftOuter;
        IndexLeftInner = _doc.IndexLeftInner;
        IndexRightOuter = _doc.IndexRightOuter;
        IndexRightInner = _doc.IndexRightInner;
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


      _doc = new FourPointStepEvaluationOptions()
      {
        UseRegressionForLeftAndRightLine = UseRegressionForLeftAndRightLine,
        MiddleRegressionLevels = (middleRegressionLowerLevel, middleRegressionUpperLevel),
        MiddleLineOverlap = middleLineOverlap,
        IncludeOriginalPointsInOutput = IncludeOriginalPointsInOutput,
        IndexLeftOuter = IndexLeftOuter,
        IndexLeftInner = IndexLeftInner,
        IndexRightOuter = IndexRightOuter,
        IndexRightInner = IndexRightInner,
      };

      return ApplyEnd(true, disposeController);

    }

  }
}
