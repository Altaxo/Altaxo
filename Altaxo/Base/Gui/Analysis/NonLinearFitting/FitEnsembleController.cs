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


using Altaxo.Calc.Regression.Nonlinear;


namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  #region Interfaces
  public interface IFitEnsembleView
  {
    IFitEnsembleViewEventSink Controller { get; set; }

    void Initialize(FitEnsemble ensemble, object[] fitEleControls);
  }

  public interface IFitEnsembleViewEventSink
  {
  }

  public interface IFitEnsembleController : IMVCAController
  {
    void Refresh();
  }

  #endregion
  /// <summary>
  /// Summary description for FitEnsembleController.
  /// </summary>
  [UserControllerForObject(typeof(FitEnsemble))]
  [ExpectedTypeOfView(typeof(IFitEnsembleView))]
  public class FitEnsembleController : IFitEnsembleController, IFitEnsembleViewEventSink
  {
    IFitEnsembleView _view;
    FitEnsemble _doc;

    IFitElementController[] _fitEleController;
    int _currentFitFunctionSelIndex;

    public FitEnsembleController(FitEnsemble doc)
    {
      _doc = doc;
      Initialize();

    }

    void Uninitialize()
    {
      if(_fitEleController!=null)
      {
        for(int i=0;i<_fitEleController.Length;i++)
        {
          _fitEleController[i].FitFunctionSelectionChange -= new EventHandler(EhFitFunctionSelectionChange);
          _fitEleController[i]=null;
        }
      }
    }
    public void Initialize()
    {
      Uninitialize();

      _fitEleController = new IFitElementController[_doc.Count];

      object[] fitEleControls = new object[_doc.Count];
      for(int i=0;i<_doc.Count;i++)
      {
        _fitEleController[i] = (IFitElementController)Current.Gui.GetControllerAndControl(new object[]{_doc[i]},typeof(IFitElementController));
        fitEleControls[i] = _fitEleController[i].ViewObject;

        _fitEleController[i].FitFunctionSelectionChange += new EventHandler(EhFitFunctionSelectionChange);
      }

      if(_view!=null)
        _view.Initialize(_doc,fitEleControls);
    }

    void EhFitFunctionSelectionChange(object sender, System.EventArgs e)
    {
      _currentFitFunctionSelIndex = GetIndexOfController(sender);

      for(int i=0;i<_fitEleController.Length;i++)
      {
        _fitEleController[i].FitFunctionSelected = (_currentFitFunctionSelIndex==i);
      }

     

    }

    int GetIndexOfController(object sender)
    {
      for(int i=0;i<_fitEleController.Length;i++)
        if(object.ReferenceEquals(sender,_fitEleController[i]))
          return i;

      return -1;
    }

    public void Refresh()
    {
      Initialize();
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
        if(_view!=null)
          _view.Controller = null;

        _view = value as IFitEnsembleView;
        
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
