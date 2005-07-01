using System;
using Altaxo.Main.GUI;

namespace Altaxo.Calc.Regression.Nonlinear
{
  #region interfaces
  public interface INonlinearFitView
  {
    INonlinearFitViewEventSink Controller { get; set; }
    void SetParameterControl(object control);
    void SetSelectFunctionControl(object control);
  }

  public interface INonlinearFitViewEventSink
  {
    void EhView_DoFit();
    void EhView_SelectFitFunction();
  }

  #endregion
  /// <summary>
  /// Summary description for NonlinearFitController.
  /// </summary>
  [UserControllerForObject(typeof(NonlinearFitDocument))]
  public class NonlinearFitController : INonlinearFitViewEventSink, Main.GUI.IMVCAController
  {
    NonlinearFitDocument _doc;
    INonlinearFitView _view;

    Main.GUI.IMVCAController _parameterController;
    FitFunctionSelectionController _funcselController;

    public NonlinearFitController(NonlinearFitDocument doc)
    {
      _doc = doc;
      _parameterController = (Main.GUI.IMVCAController)Current.GUIFactoryService.GetControllerAndControl(new object[]{_doc.CurrentParameters},typeof(Main.GUI.IMVCAController));
      _funcselController = new FitFunctionSelectionController(_doc.FitEnsemble.Count==0 ? null : _doc.FitEnsemble[0].FitFunction);
      Current.GUIFactoryService.GetControl(_funcselController);
    
    }

    public void Initialize()
    {
      if(_view!=null)
      {
        _view.SetParameterControl(_parameterController.ViewObject);
        _view.SetSelectFunctionControl(_funcselController.ViewObject);
      }
    }

    #region  INonlinearFitViewEventSink
    
    public void EhView_DoFit()
    {
      if(true==this._parameterController.Apply())
      {
        _doc.FitEnsemble.InitializeParametersFromParameterSet(_doc.CurrentParameters);
        _doc.FitEnsemble.Fit();
        _doc.FitEnsemble.InitializeParameterSetFromEnsembleParameters(_doc.CurrentParameters);
      }
      else
      {
        Current.GUIFactoryService.ErrorMessageBox("Some of your parameter input is not valid!");
      }
    }
    public void EhView_SelectFitFunction()
    {
      bool changed = false;

      if(_funcselController.Apply())
      {
        if(_doc.FitEnsemble.Count>0)
        {
          if(_doc.FitEnsemble[_doc.FitEnsemble.Count-1].FitFunction==null)
          {
            _doc.FitEnsemble[_doc.FitEnsemble.Count-1].FitFunction = (IFitFunction)_funcselController.ModelObject;
            changed = true;
          }
        }
      }

      if(changed)
      {
        _doc.FitEnsemble.InitializeFittingSession();
        _doc.FitEnsemble.InitializeParameterSetFromEnsembleParameters(_doc.CurrentParameters);
      }
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

        _view = value as INonlinearFitView;
        
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
