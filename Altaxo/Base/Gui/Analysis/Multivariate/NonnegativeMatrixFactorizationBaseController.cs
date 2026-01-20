using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra.Double.Factorization;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;

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


    public ItemsController<System.Type> InitializationMethod
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(InitializationMethod));
        }
      }
    }



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

        {
          // initialization method
          var initializationTypes = ReflectionService.GetNonAbstractSubclassesOf(typeof(INonnegativeMatrixFactorizationInitializer));
          var list = new SelectableListNodeList(initializationTypes.Select(t => new SelectableListNode(t.Name, t, false)));
          InitializationMethod = new ItemsController<System.Type>(list);
          InitializationMethod.SelectedValue = _doc.InitializationMethod.GetType();
        }

        if (_doc is NonnegativeMatrixFactorizationWithRegularizationBase withRegularization)
        {
          LambdaH = withRegularization.LambdaH;
          LambdaW = withRegularization.LambdaW;
        }
      }
    }


    public override bool Apply(bool disposeController)
    {
      var initializationMethod = (INonnegativeMatrixFactorizationInitializer)Activator.CreateInstance(InitializationMethod.SelectedValue);

      _doc = _doc with
      {
        InitializationMethod = initializationMethod,
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
