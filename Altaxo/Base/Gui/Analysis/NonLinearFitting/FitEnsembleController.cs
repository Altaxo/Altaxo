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
using System.Collections.Generic;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  #region Interfaces

  public interface IFitEnsembleView
  {
    void Initialize(FitEnsemble ensemble, IEnumerable<object> fitEleControls);
  }

  public interface IFitEnsembleController : IMVCAController
  {
    void Refresh();
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for FitEnsembleController.
  /// </summary>
  [UserControllerForObject(typeof(FitEnsemble))]
  [ExpectedTypeOfView(typeof(IFitEnsembleView))]
  public class FitEnsembleController : IFitEnsembleController
  {
    private IFitEnsembleView _view;
    private FitEnsemble _doc;

    private List<IFitElementController> _fitEleController;
    private List<object> _fitEleControls;
    private int _currentFitFunctionSelIndex;

    public FitEnsembleController(FitEnsemble doc)
    {
      _doc = doc;
      Initialize(true);
    }

    private void Uninitialize()
    {
      if (_fitEleController != null)
      {
        for (int i = 0; i < _fitEleController.Count; i++)
        {
          DetachFitElementController(_fitEleController[i]);
        }
      }
      _fitEleController = new List<IFitElementController>();
    }

    public void Initialize(bool initData)
    {
      if (initData)
      {
        Uninitialize();

        _fitEleController = new List<IFitElementController>(_doc.Count);
        _fitEleControls = new List<object>(_doc.Count);

        for (int i = 0; i < _doc.Count; i++)
        {
          _fitEleController.Add((IFitElementController)Current.Gui.GetControllerAndControl(new object[] { _doc[i] }, typeof(IFitElementController)));
          _fitEleControls.Add(_fitEleController[i].ViewObject);
          AttachFitElementController(_fitEleController[i]);
        }
      }
      if (_view != null)
      {
        _view.Initialize(_doc, _fitEleControls);
      }
    }

    private void AttachView()
    {
    }

    private void DetachView()
    {
    }

    private void AttachFitElementController(IFitElementController c)
    {
      c.FitFunctionSelectionChange += EhFitFunctionSelectionChange;
      c.DeletionOfThisFitElementRequested += EhDeletionOfFitElementRequested;
    }

    private void DetachFitElementController(IFitElementController c)
    {
      c.FitFunctionSelectionChange -= EhFitFunctionSelectionChange;
      c.DeletionOfThisFitElementRequested -= EhDeletionOfFitElementRequested;
    }

    private void EhFitFunctionSelectionChange(object sender, System.EventArgs e)
    {
      _currentFitFunctionSelIndex = GetIndexOfController(sender);

      for (int i = 0; i < _fitEleController.Count; i++)
      {
        _fitEleController[i].FitFunctionSelected = (_currentFitFunctionSelIndex == i);
      }
    }

    private void EhDeletionOfFitElementRequested(FitElement fitElement)
    {
      for (int i = _doc.Count - 1; i >= 0; --i)
      {
        if (object.ReferenceEquals(_doc[i], fitElement))
        {
          _doc.RemoveAt(i);
          DetachFitElementController(_fitEleController[i]);
          _fitEleController.RemoveAt(i);
          _fitEleControls.RemoveAt(i);
          break;
        }
      }

      _view.Initialize(_doc, _fitEleControls);
    }

    private int GetIndexOfController(object sender)
    {
      for (int i = 0; i < _fitEleController.Count; i++)
        if (object.ReferenceEquals(sender, _fitEleController[i]))
          return i;

      return -1;
    }

    public void Refresh()
    {
      Initialize(true);
    }

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

        _view = value as IFitEnsembleView;

        if (_view != null)
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
