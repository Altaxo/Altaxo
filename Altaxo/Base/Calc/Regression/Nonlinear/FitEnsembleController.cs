using System;

using Altaxo.Main.GUI;

namespace Altaxo.Calc.Regression.Nonlinear
{
  #region Interfaces
  public interface IFitEnsembleView
  {
    IFitEnsembleViewEventSink Controller { get; set; }

    void Initialize(FitEnsemble ensemble);
  }

  public interface IFitEnsembleViewEventSink
  {
  }

  public interface IFitEnsembleController : IMVCAController
  {
  }

  #endregion
	/// <summary>
	/// Summary description for FitEnsembleController.
	/// </summary>
	[UserControllerForObject(typeof(FitEnsemble))]
	public class FitEnsembleController : IFitEnsembleController, IFitEnsembleViewEventSink
	{
    IFitEnsembleView _view;
    FitEnsemble _doc;

		public FitEnsembleController(FitEnsemble doc)
		{
      _doc = doc;
      Initialize();

    }

    public void Initialize()
    {
      if(_view!=null)
        _view.Initialize(_doc);
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
