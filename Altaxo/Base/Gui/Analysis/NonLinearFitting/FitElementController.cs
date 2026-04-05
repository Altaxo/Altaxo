#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Data.Transformations;
using Altaxo.Gui.Common;
using Altaxo.Gui.Graph;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  #region interfaces

  /// <summary>
  /// View contract for <see cref="FitElementController"/>.
  /// </summary>
  public interface IFitElementView
  {
    /// <summary>
    /// Initializes the view from the specified fit element.
    /// </summary>
    void Initialize(FitElement fitElement);

    /// <summary>
    /// Refreshes the view.
    /// </summary>
    void Refresh();

    /// <summary>
    /// Sets a value indicating whether a fit function is selected.
    /// </summary>
    bool FitFunctionSelected { set; }

    /// <summary>
    /// Occurs when the user chooses an error function.
    /// </summary>
    event Action<int> ChooseErrorFunction;

    /// <summary>
    /// Occurs when the user chooses a fit function.
    /// </summary>
    event Action ChooseFitFunction;

    /// <summary>
    /// Occurs when the user chooses an external parameter.
    /// </summary>
    event Action<int> ChooseExternalParameter;

    /// <summary>
    /// Occurs when the variables and range should be configured.
    /// </summary>
    event Action SetupVariablesAndRange;

    /// <summary>
    /// Occurs when the fit function should be edited.
    /// </summary>
    event Action EditFitFunction;

    /// <summary>
    /// Occurs when this fit element should be deleted.
    /// </summary>
    event Action DeleteThisFitElement;
  }

  /// <summary>
  /// Controller contract for <see cref="FitElement"/>.
  /// </summary>
  public interface IFitElementController : IMVCAController
  {
    /// <summary>
    /// Occurs when the fit-function selection changes.
    /// </summary>
    event EventHandler FitFunctionSelectionChange;

    /// <summary>
    /// Occurs when the deletion of this fit element is requested by the user.
    /// </summary>
    event Action<FitElement> DeletionOfThisFitElementRequested;

    /// <summary>
    /// Sets a value indicating whether a fit function is selected.
    /// </summary>
    bool FitFunctionSelected { set; }

    /// <summary>
    /// Gets or sets the index of the fit element.
    /// </summary>
    int Index { get; set; }
  }

  #endregion interfaces

  /// <summary>
  /// Controller for <see cref="FitElement"/>.
  /// </summary>
  [UserControllerForObject(typeof(FitElement))]
  [ExpectedTypeOfView(typeof(IFitElementView))]
  public class FitElementController : IFitElementController
  {
    private IFitElementView _view;
    private FitElement _doc;

    /// <inheritdoc/>
    public event EventHandler FitFunctionSelectionChange;

    /// <inheritdoc/>
    public event Action<FitElement> DeletionOfThisFitElementRequested;

    /// <inheritdoc/>
    public int Index { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FitElementController"/> class.
    /// </summary>
    /// <param name="doc">The fit element document.</param>
    public FitElementController(FitElement doc)
    {
      _doc = doc;
    }

    /// <summary>
    /// Initializes the controller and view state.
    /// </summary>
    public void Initialize(bool initData)
    {
      if (_view is not null)
      {
        _view.Initialize(_doc);
      }
    }

    private void AttachView()
    {
      _view.ChooseErrorFunction += EhView_ChooseErrorFunction;

      _view.ChooseFitFunction += EhView_ChooseFitFunction;

      _view.ChooseExternalParameter += EhView_ChooseExternalParameter;

      _view.SetupVariablesAndRange += EhView_SetupVariablesAndRange;

      _view.EditFitFunction += EhView_EditFitFunction;

      _view.DeleteThisFitElement += EhView_DeleteThisFitElement;
    }

    private void DetachView()
    {
      _view.ChooseErrorFunction -= EhView_ChooseErrorFunction;

      _view.ChooseFitFunction -= EhView_ChooseFitFunction;

      _view.ChooseExternalParameter -= EhView_ChooseExternalParameter;

      _view.SetupVariablesAndRange -= EhView_SetupVariablesAndRange;

      _view.EditFitFunction -= EhView_EditFitFunction;

      _view.DeleteThisFitElement -= EhView_DeleteThisFitElement;
    }

    /// <inheritdoc/>
    public bool FitFunctionSelected
    {
      set
      {
        if (_view is not null)
          _view.FitFunctionSelected = value;
      }
    }

    #region IFitElementController

    /// <summary>
    /// Handles configuration of the fit-element variables and range.
    /// </summary>
    public void EhView_SetupVariablesAndRange()
    {
      var controller = new FitElementDataController();
      controller.InitializeDocument(_doc);

      Current.Gui.ShowDialog(controller, "Fit element variables", true);

      _view.Refresh();
    }

    /// <summary>
    /// Handles selection of an external parameter.
    /// </summary>
    public void EhView_ChooseExternalParameter(int idx)
    {
      string choice = _doc.ParameterName(idx);
      object choiceAsObject = choice;
      if (Current.Gui.ShowDialog(ref choiceAsObject, "Edit parameter name"))
      {
        choice = (string)choiceAsObject;

        if (choice.Length > 0)
        {
          _doc.SetParameterName(choice, idx);
        }
        else
        {
            Current.Gui.ErrorMessageBox("Chosen parameter name was empty!");
        }
      }
      _view.Refresh();
    }

    /// <summary>
    /// Handles selection of an error function.
    /// </summary>
    public void EhView_ChooseErrorFunction(int idx)
    {
      var docTrans = _doc.GetDependentVariableTransformation(idx);
      var controller = new DependentVariableTransformationController(docTrans);
      Current.Gui.FindAndAttachControlTo(controller);
      if (true == Current.Gui.ShowDialog(controller, "Choose dependent variable transformation"))
      {
        docTrans = (IDoubleToDoubleTransformation)controller.ModelObject;
        _doc.SetDependentVariableTransformation(idx, docTrans);
        _view.Refresh();
      }

      /*
      var choice = new SingleInstanceChoice(typeof(IDoubleToDoubleTransformation), _doc.GetDependentVariableTransformation(idx));

      object choiceAsObject = choice;
      if (Current.Gui.ShowDialog(ref choiceAsObject, "Select error norm"))
      {
        choice = (SingleInstanceChoice)choiceAsObject;
        _doc.SetDependentVariableTransformation(idx, (IDoubleToDoubleTransformation)choice.Instance);
        _view.Refresh();
      }
      */
    }

    /// <summary>
    /// Handles editing of the fit function.
    /// </summary>
    public void EhView_EditFitFunction()
    {
      object fitFunc = _doc.FitFunction;
      if (Current.Gui.ShowDialog(ref fitFunc, "Edit fit function"))
        _doc.FitFunction = (IFitFunction)fitFunc;
    }

    private void EhView_DeleteThisFitElement()
    {
      DeletionOfThisFitElementRequested?.Invoke(_doc);
    }

    /// <summary>
    /// Handles selection of the fit function.
    /// </summary>
    public void EhView_ChooseFitFunction()
    {
      FitFunctionSelectionChange?.Invoke(this, EventArgs.Empty);

      _view.Refresh();
    }

    #endregion IFitElementController

    #region IMVCController Members

    /// <inheritdoc/>
    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is not null)
        {
          DetachView();
        }

        _view = value as IFitElementView;

        if (_view is not null)
        {
          Initialize(false);
          AttachView();
        }
      }
    }

    /// <inheritdoc/>
    public object ModelObject
    {
      get
      {
        return _doc;
      }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

    /// <inheritdoc/>
    public bool Apply(bool disposeController)
    {
      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successful; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members
  }
}
