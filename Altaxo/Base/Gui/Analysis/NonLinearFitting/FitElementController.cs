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

using System;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Gui.Common;
using Altaxo.Gui.Graph;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  #region interfaces

  public interface IFitElementView
  {
    void Initialize(FitElement fitElement);

    void Refresh();

    bool FitFunctionSelected { set; }

    event Action<int> ChooseErrorFunction;

    event Action ChooseFitFunction;

    event Action<int> ChooseExternalParameter;

    event Action SetupVariablesAndRange;

    event Action EditFitFunction;

    event Action DeleteThisFitElement;
  }

  public interface IFitElementController : IMVCAController
  {
    event EventHandler FitFunctionSelectionChange;

    /// <summary>
    /// Occurs when the deletion of this fit element is requested by the user.
    /// </summary>
    event Action<FitElement> DeletionOfThisFitElementRequested;

    bool FitFunctionSelected { set; }
  }

  #endregion interfaces

  /// <summary>
  /// Summary description for FitElementController.
  /// </summary>
  [UserControllerForObject(typeof(FitElement))]
  [ExpectedTypeOfView(typeof(IFitElementView))]
  public class FitElementController : IFitElementController
  {
    private IFitElementView _view;
    private FitElement _doc;

    public event EventHandler FitFunctionSelectionChange;

    public event Action<FitElement> DeletionOfThisFitElementRequested;

    public FitElementController(FitElement doc)
    {
      _doc = doc;
    }

    public void Initialize(bool initData)
    {
      if (_view != null)
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

    public bool FitFunctionSelected
    {
      set
      {
        if (null != _view)
          _view.FitFunctionSelected = value;
      }
    }

    #region IFitElementController

    public void EhView_SetupVariablesAndRange()
    {
      var controller = new FitElementDataController();
      controller.InitializeDocument(_doc);

      Current.Gui.ShowDialog(controller, "Fit element variables", true);

      _view.Refresh();
    }

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
          Current.Gui.ErrorMessageBox("Choosen parameter name was empty!");
        }
      }
      _view.Refresh();
    }

    public void EhView_ChooseErrorFunction(int idx)
    {
      var choice = new SingleInstanceChoice(typeof(IVarianceScaling), _doc.GetErrorEvaluation(idx));

      object choiceAsObject = choice;
      if (Current.Gui.ShowDialog(ref choiceAsObject, "Select error norm"))
      {
        choice = (SingleInstanceChoice)choiceAsObject;
        _doc.SetErrorEvaluation(idx, (IVarianceScaling)choice.Instance);
        _view.Refresh();
      }
    }

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

    public void EhView_ChooseFitFunction()
    {
      FitFunctionSelectionChange?.Invoke(this, EventArgs.Empty);

      _view.Refresh();
    }

    #endregion IFitElementController

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view != null)
        {
          DetachView();
        }

        _view = value as IFitElementView;

        if (null != _view)
        {
          Initialize(false);
          AttachView();
        }
      }
    }

    public object ModelObject
    {
      get
      {
        return _doc;
      }
    }

    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

    public bool Apply(bool disposeController)
    {
      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members
  }
}
