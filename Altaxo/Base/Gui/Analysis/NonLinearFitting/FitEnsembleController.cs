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
using System.Collections.Generic;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  #region Interfaces

  public interface IFitEnsembleView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for FitEnsembleController.
  /// </summary>
  [UserControllerForObject(typeof(FitEnsemble))]
  [ExpectedTypeOfView(typeof(IFitEnsembleView))]
  public class FitEnsembleController : MVCANControllerEditImmutableDocBase<FitEnsemble, IFitEnsembleView>
  {
    private int _currentFitFunctionSelIndex;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public FitEnsembleController(FitEnsemble doc)
    {
      _doc = doc;
      Initialize(true);
    }

    public FitEnsembleController()
    {
    }

    #region Bindings

    private SelectableListNodeList    _fitElementControllers;

    public SelectableListNodeList FitElementControllers
    {
      get => _fitElementControllers;
      set
      {
        if (!(_fitElementControllers == value))
        {
          _fitElementControllers = value;
          OnPropertyChanged(nameof(FitElementControllers));
        }
      }
    }


    #endregion

    public override void Dispose(bool isDisposing)
    {
      base.Dispose(isDisposing);
      Uninitialize();
    }

    private void Uninitialize()
    {
      if (_fitElementControllers is not null)
      {
        for (int i = 0; i < _fitElementControllers.Count; i++)
        {
          DetachFitElementController(GetFitElementController(i));
        }
        _fitElementControllers.Clear();
      }
    }

   protected override void Initialize(bool initData)
    {
      if (initData)
      {
        Uninitialize();

        var fitElementControllers = new SelectableListNodeList();
        for (int i = 0; i < _doc.Count; i++)
        {
          var ctrl = (IFitElementController)Current.Gui.GetControllerAndControl(new object[] { _doc[i] }, typeof(IFitElementController));
          ctrl.Index = i;

          fitElementControllers.Add(new SelectableListNodeWithController("", i, false)
          {
            Controller = ctrl
          });
          AttachFitElementController(ctrl);
        }
        FitElementControllers = fitElementControllers;
      }
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

    private IFitElementController GetFitElementController(int idx)
    {
      var ctrl = ((SelectableListNodeWithController)_fitElementControllers[idx]).Controller;
      return (IFitElementController)ctrl;
    }

    private void EhFitFunctionSelectionChange(object sender, System.EventArgs e)
    {
      _currentFitFunctionSelIndex = ((IFitElementController)sender).Index;

      for (int i = 0; i < _fitElementControllers.Count; i++)
      {
        GetFitElementController(i).FitFunctionSelected = (_currentFitFunctionSelIndex == i);
      }
    }

    private void EhDeletionOfFitElementRequested(FitElement fitElement)
    {
      for (int i = _doc.Count - 1; i >= 0; --i)
      {
        if (object.ReferenceEquals(_doc[i], fitElement))
        {
          _doc.RemoveAt(i);
          DetachFitElementController(GetFitElementController(i));
          _fitElementControllers.RemoveAt(i);
          break;
        }
      }
    }

   

    public void Refresh()
    {
      Initialize(true);
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }
  }
}
