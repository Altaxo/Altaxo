using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra.Double.Factorization;

namespace Altaxo.Gui.Analysis.Multivariate
{
  public interface INonnegativeMatrixFactorizationBaseView : IDataContextAwareView
  {

  }

  [ExpectedTypeOfView(typeof(INonnegativeMatrixFactorizationBaseView))]
  [UserControllerForObject(typeof(NonnegativeMatrixFactorizationBase))]
  public class NonnegativeMatrixFactorizationBaseController : MVCANControllerEditImmutableDocBase<NonnegativeMatrixFactorizationBase, INonnegativeMatrixFactorizationBaseView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings


    public int MaximumNumberOfIterations
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(MaximumNumberOfIterations));
        }
      }
    }


    public int NumberOfAdditionalTrials
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(NumberOfAdditionalTrials));
        }
      }
    }


    public double Tolerance
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Tolerance));
        }
      }
    }


    public double LambdaH
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(LambdaH));
        }
      }
    }


    public double LambdaW
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(LambdaW));
        }
      }
    }


    public bool AreLambdasVisible
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(AreLambdasVisible));
        }
      }
    }



    #endregion Bindings

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        MaximumNumberOfIterations = _doc.MaximumNumberOfIterations;
        NumberOfAdditionalTrials = _doc.NumberOfAdditionalTrials;
        Tolerance = _doc.Tolerance;

        if (_doc is NonnegativeMatrixFactorizationWithRegularizationBase withRegularization)
        {
          LambdaH = withRegularization.LambdaH;
          LambdaW = withRegularization.LambdaW;
        }
      }
    }


    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        MaximumNumberOfIterations = MaximumNumberOfIterations,
        NumberOfAdditionalTrials = NumberOfAdditionalTrials,
        Tolerance = Tolerance
      };

      if (_doc is NonnegativeMatrixFactorizationWithRegularizationBase withRegularization)
      {
        _doc = withRegularization with
        {
          LambdaH = LambdaH,
          LambdaW = LambdaW
        };
        AreLambdasVisible = true;
      }

      return ApplyEnd(true, disposeController);
    }

  }
}
