using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra.Double.Factorization;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Analysis.Multivariate
{
  /// <summary>
  /// Defines the view contract for editing nonnegative matrix-factorization settings.
  /// </summary>
  public interface INonnegativeMatrixFactorizationBaseView : IDataContextAwareView
  {

  }

  /// <summary>
  /// Controller for <see cref="NonnegativeMatrixFactorizationBase"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(INonnegativeMatrixFactorizationBaseView))]
  [UserControllerForObject(typeof(NonnegativeMatrixFactorizationBase))]
  public class NonnegativeMatrixFactorizationBaseController : MVCANControllerEditImmutableDocBase<NonnegativeMatrixFactorizationBase, INonnegativeMatrixFactorizationBaseView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings


    /// <summary>
    /// Gets or sets the available initialization methods.
    /// </summary>
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



    /// <summary>
    /// Gets or sets the maximum number of iterations.
    /// </summary>
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


    /// <summary>
    /// Gets or sets the number of additional trials.
    /// </summary>
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


    /// <summary>
    /// Gets or sets the convergence tolerance.
    /// </summary>
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


    /// <summary>
    /// Gets or sets the regularization value for H.
    /// </summary>
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


    /// <summary>
    /// Gets or sets the regularization value for W.
    /// </summary>
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


    /// <summary>
    /// Gets or sets a value indicating whether regularization controls are visible.
    /// </summary>
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

    /// <inheritdoc/>
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


    /// <inheritdoc/>
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
