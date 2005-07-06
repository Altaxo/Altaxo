using System;
using Altaxo.Main.GUI;
using Altaxo.Graph;

namespace Altaxo.Calc.Regression.Nonlinear
{
  #region interfaces
  public interface INonlinearFitView
  {
    INonlinearFitViewEventSink Controller { get; set; }
    void SetParameterControl(object control);
    void SetSelectFunctionControl(object control);
    void SetFitEnsembleControl(object control);
    void SetChiSquare(double chiSquare);
  }

  public interface INonlinearFitViewEventSink
  {
    void EhView_DoFit();
    void EhView_EvaluateChiSqr();
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
    IFitEnsembleController _fitEnsembleController;

    public NonlinearFitController(NonlinearFitDocument doc)
    {
      _doc = doc;
      _parameterController = (Main.GUI.IMVCAController)Current.GUIFactoryService.GetControllerAndControl(new object[]{_doc.CurrentParameters},typeof(Main.GUI.IMVCAController));
      _fitEnsembleController = (IFitEnsembleController)Current.GUIFactoryService.GetControllerAndControl(new object[]{_doc.FitEnsemble},typeof(IFitEnsembleController));

      _funcselController = new FitFunctionSelectionController(_doc.FitEnsemble.Count==0 ? null : _doc.FitEnsemble[0].FitFunction);
      Current.GUIFactoryService.GetControl(_funcselController);
    
    }

    public void Initialize()
    {
      if(_view!=null)
      {
        _view.SetParameterControl(_parameterController.ViewObject);
        _view.SetSelectFunctionControl(_funcselController.ViewObject);
        _view.SetFitEnsembleControl(_fitEnsembleController.ViewObject);
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
        _doc.FitEnsemble.DistributeParameters();
        OnAfterFittingStep();
      }
      else
      {
        Current.GUIFactoryService.ErrorMessageBox("Some of your parameter input is not valid!");
      }
    }

    public     void EhView_EvaluateChiSqr()
    {
      if(true==this._parameterController.Apply())
      {
        _doc.FitEnsemble.InitializeParametersFromParameterSet(_doc.CurrentParameters);
        _doc.FitEnsemble.DistributeParameters();
        OnAfterFittingStep();
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
          else
        {
            FitElement newele = new FitElement();
            newele.FitFunction = (IFitFunction)_funcselController.ModelObject;
            _doc.FitEnsemble.Add(newele);
            changed=true;
        }
        }
        else // Count==0
        {
          FitElement newele = new FitElement();
          newele.FitFunction = (IFitFunction)_funcselController.ModelObject;
          _doc.FitEnsemble.Add(newele);
          changed=true;
        }
      }

      if(changed)
      {
        _doc.FitEnsemble.InitializeFittingSession();
        _doc.FitEnsemble.InitializeParameterSetFromEnsembleParameters(_doc.CurrentParameters);
        
        this._fitEnsembleController.Refresh();

      }
    }


    System.Collections.ArrayList _functionPlotItems = new System.Collections.ArrayList();
    public void OnAfterFittingStep()
    {
      double chiSquare = _doc.FitEnsemble.GetChiSqr();
      if(_view!=null)
        _view.SetChiSquare(chiSquare);


      if(_doc.FitContext is Altaxo.Graph.GUI.GraphController)
      {
        // for every dependent variable in the FitEnsemble, create a function graph
        Altaxo.Graph.GUI.GraphController graph = _doc.FitContext as Altaxo.Graph.GUI.GraphController;

        int funcNumber=0;
        for(int i=0;i<_doc.FitEnsemble.Count;i++)
        {
          FitElement fitEle = _doc.FitEnsemble[i];

          for(int k=0;k<fitEle.NumberOfDependentVariables;k++, funcNumber++)
          {
            if(funcNumber<_functionPlotItems.Count && _functionPlotItems[funcNumber]!=null)
            {
              Altaxo.Graph.XYFunctionPlotItem plotItem = (Altaxo.Graph.XYFunctionPlotItem)_functionPlotItems[funcNumber];
              FitFunctionToScalarFunctionDDWrapper wrapper = (FitFunctionToScalarFunctionDDWrapper)plotItem.Data.Function;
              wrapper.Initialize(fitEle.FitFunction,k,fitEle.ParameterValues);
            }
            else
            {
              FitFunctionToScalarFunctionDDWrapper wrapper = new FitFunctionToScalarFunctionDDWrapper(fitEle.FitFunction,k, fitEle.ParameterValues);
              Altaxo.Graph.XYFunctionPlotData plotdata = new Altaxo.Graph.XYFunctionPlotData(wrapper);
              Altaxo.Graph.XYFunctionPlotItem plotItem = new Altaxo.Graph.XYFunctionPlotItem(plotdata,new Altaxo.Graph.XYLineScatterPlotStyle(LineScatterPlotStyleKind.Line));
              graph.ActiveLayer.PlotItems.Add(plotItem);
              _functionPlotItems.Add(plotItem);
            }
          }
        }

        // if there are more elements in _functionPlotItems, remove them from the graph
        for(int i=_functionPlotItems.Count-1;i>=funcNumber;--i)
        {
          if(_functionPlotItems[i]!=null)
          {
            graph.ActiveLayer.PlotItems.Remove((Altaxo.Graph.PlotItem)_functionPlotItems[i]);
            _functionPlotItems.RemoveAt(i);

          }
        }
        graph.RefreshGraph();
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
