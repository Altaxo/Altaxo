#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Gui.Common;
using Altaxo.Gui.Graph;
using Altaxo.Data;
using Altaxo.Calc.Regression.Nonlinear;


namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  #region interfaces

  public interface IFitElementView
  {
    IFitElementViewEventSink Controller { get; set; }
    void Initialize(FitElement fitElement);
    void Refresh();
    bool FitFunctionSelected { set; }
  }

  public interface IFitElementViewEventSink
  {
    void EhView_ChooseIndependentColumn(int idx);
    void EhView_ChooseDependentColumn(int idx);
    void EhView_ChooseErrorFunction(int idx);
    void EhView_ChooseFitFunction();
    void EhView_ChooseExternalParameter(int idx);
    void EhView_ChooseFitRange();

    void EhView_DeleteDependentVariable(int idx);
  }

  public interface IFitElementController : IMVCAController
  {
    event EventHandler FitFunctionSelectionChange;
    bool FitFunctionSelected { set; }
  }

  #endregion

  /// <summary>
  /// Summary description for FitElementController.
  /// </summary>
  [UserControllerForObject(typeof(FitElement))]
  [ExpectedTypeOfView(typeof(IFitElementView))]
  public class FitElementController: IFitElementViewEventSink, IFitElementController
  {
    IFitElementView _view;
    FitElement _doc;

    public FitElementController(FitElement doc)
    {
      _doc = doc;
    }

    public void Initialize()
    {
      if(_view!=null)
      {
        _view.Initialize(_doc);
      }
    }

    public bool FitFunctionSelected
    {
      set
      {
        if(null!=_view)
          _view.FitFunctionSelected = value;
      }
    }
    #region IFitElementController

    public void EhView_ChooseFitRange()
    {
      object range = _doc.GetRowRange();
      if (Current.Gui.ShowDialog(ref range, "Choose fit range"))
      {
        _doc.SetRowRange((Calc.PositiveIntegerRange)range);
        _view.Refresh();
      }
    }

    public void EhView_ChooseIndependentColumn(int idx)
    {
      SingleColumnChoice choice = new SingleColumnChoice();
      choice.SelectedColumn = _doc.IndependentVariables(idx) as DataColumn;
      object choiceAsObject = choice;
      if(Current.Gui.ShowDialog(ref choiceAsObject,"Select independent column"))
      {
        choice = (SingleColumnChoice)choiceAsObject;

        if (choice.SelectedColumn is INumericColumn)
        {
          _doc.SetIndependentVariable(idx, (INumericColumn)choice.SelectedColumn);
        }
        else
        {
          Current.Gui.ErrorMessageBox("Choosen column is not numeric!");
        }
      }
      _view.Refresh();
    }
    public void EhView_ChooseDependentColumn(int idx)
    {
      SingleColumnChoice choice = new SingleColumnChoice();
      choice.SelectedColumn = _doc.DependentVariables(idx) as DataColumn;
      object choiceAsObject = choice;
      if (Current.Gui.ShowDialog(ref choiceAsObject, "Select dependent column"))
      {
        choice = (SingleColumnChoice)choiceAsObject;

        if (choice.SelectedColumn is INumericColumn)
        {
          _doc.SetDependentVariable(idx, (INumericColumn)choice.SelectedColumn);
        }
        else
        {
          Current.Gui.ErrorMessageBox("Choosen column is not numeric!");
        }
      }
      _view.Refresh();
    }

    public void EhView_ChooseExternalParameter(int idx)
    {
      string choice = _doc.ParameterName(idx);
      object choiceAsObject = choice;
      if (Current.Gui.ShowDialog(ref choiceAsObject, "Edit parameter name"))
      {
        choice = (string)choiceAsObject;

        if (choice.Length>0)
        {
          _doc.SetParameterName(choice,idx);
        }
        else
        {
          Current.Gui.ErrorMessageBox("Choosen parameter name was empty!");
        }
      }
      _view.Refresh();
    }

    public void EhView_DeleteDependentVariable(int idx)
    {
      _doc.SetDependentVariable(idx,null);
      _view.Refresh();
    }
    public void EhView_ChooseErrorFunction(int idx)
    {
      SingleInstanceChoice choice = new SingleInstanceChoice(typeof(IVarianceScaling),_doc.ErrorEvaluation(idx));

      object choiceAsObject = choice;
      if(Current.Gui.ShowDialog(ref choiceAsObject, "Select error norm"))
      {
        choice = (SingleInstanceChoice)choiceAsObject;
        _doc.SetErrorEvaluation(idx,(IVarianceScaling)choice.Instance);
        _view.Refresh();
      }
    }
    public event EventHandler FitFunctionSelectionChange;
    public void EhView_ChooseFitFunction()
    {
      if(null!=FitFunctionSelectionChange)
        FitFunctionSelectionChange(this,EventArgs.Empty);

      _view.Refresh();
    }
  
    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        
        return _view;
      }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value as IFitElementView;
        
        Initialize();

        if(_view!=null)
          _view.Controller = this;
      }
    }

    public object ModelObject
    {
      get
      {
        return _doc;
      }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
    
      return true;
    }

    #endregion
  }
}
