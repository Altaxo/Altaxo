using System;
using System.Text;
using System.Collections;
using Altaxo.Scripting;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{

  public interface IFitFunctionSelectionView
  {
   

    IFitFunctionSelectionViewEventSink Controller { get; set; }
    void InitializeFitFunctionList(DictionaryEntry[] entries, System.Type currentSelection);
    void InitializeDocumentFitFunctionList(DictionaryEntry[] entries, object currentSelection);
  }

  public interface IFitFunctionSelectionViewEventSink
  {
    void EhView_SelectionChanged(object selectedtag);
    void EhView_EditItem(object selectedtag);
  }

  public interface IFitFunctionSelectionController : Main.GUI.IMVCAController
  {
    void Refresh();
  }

  public class FitFunctionSelectionController : IFitFunctionSelectionViewEventSink, Main.GUI.IMVCAController
  {
    IFitFunction _doc;
    IFitFunction _tempdoc;
    IFitFunctionSelectionView _view;

    public FitFunctionSelectionController(IFitFunction doc)
    {
      _doc = doc;
      _tempdoc = doc;
      Initialize();
    }


    public void Refresh()
    {
      Initialize();
    }

    public void Initialize()
    {
      if(_view!=null)
      {
        DictionaryEntry[] entries = Altaxo.Main.Services.ReflectionService.GetAttributeInstancesAndClassTypes(typeof(FitFunctionAttribute));
        _view.InitializeFitFunctionList(entries,_tempdoc==null ? null : _tempdoc.GetType());
        _view.InitializeDocumentFitFunctionList(GetDocumentEntries(), null);
      }
    }


    DictionaryEntry[] GetDocumentEntries()
    {
      ArrayList arr = new ArrayList();
      foreach(FitFunctionScript func in Current.Project.FitFunctionScripts)
      {
        arr.Add(new DictionaryEntry(func.FitFunctionCategory+"\\"+func.FitFunctionName,func));
      }

      return (DictionaryEntry[])arr.ToArray(typeof(DictionaryEntry));
    }

    public void EhView_SelectionChanged(object selectedtag)
    {
      if(selectedtag is System.Type)
      {
        _tempdoc = System.Activator.CreateInstance((System.Type)selectedtag) as IFitFunction;
      }
      else if (selectedtag is IFitFunction)
      {
        _tempdoc = (IFitFunction)selectedtag;
      }
    }

    public void EhView_EditItem(object selectedtag)
    {
      if (selectedtag is Altaxo.Scripting.FitFunctionScript)
      {
        Current.Gui.ShowDialog(new object[] { selectedtag }, "Edit fit function script");
      }
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

        _view = value as IFitFunctionSelectionView;
        
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
      
      if(_tempdoc != null)
      {
        _doc = _tempdoc;
        return true;
      }

      return false;
    }

    #endregion
  }
}
