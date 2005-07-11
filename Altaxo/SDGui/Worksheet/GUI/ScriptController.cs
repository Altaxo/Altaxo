using System;
using Altaxo.Data;
using Altaxo.Main.GUI;
using System.Text.RegularExpressions;
using Altaxo.Main.Services;

namespace Altaxo.Worksheet.GUI
{
  #region Interfaces
  public interface IScriptView
  {
    IScriptViewEventSink Controller {get; set; }
    void AddPureScriptView(object scriptView);

    void ClearCompilerErrors();
    void AddCompilerError(string s);
  }

  public interface IScriptViewEventSink
  {
     void EhView_GotoCompilerError(string message);
  }

  public interface IScriptController : Main.GUI.IMVCAController
  {
    void Compile();
    void Update();
    void Cancel();
   
  }



  #endregion

	/// <summary>
	/// Summary description for ScriptController.
	/// </summary>
	public class ScriptController : IScriptViewEventSink, IScriptController
	{
    IScriptView _view;
    IScriptText _doc;

    IPureScriptController _pureScriptController;
    private Regex _compilerErrorRegex = new Regex(@".*\((?<line>\d+),(?<column>\d+)\) : (?<msg>.+)",RegexOptions.Compiled);


		public ScriptController(IScriptText doc)
		{
      _doc = doc;
			_pureScriptController = new PureScriptController(_doc);
      _pureScriptController.ViewObject=new PureScriptControl();
    }

    public void Initialize()
    {
      if(_view!=null)
      {
        _view.ClearCompilerErrors();
        _view.AddPureScriptView(_pureScriptController.ViewObject);
      }
    }
    #region IScriptViewEventSink Members

    public void EhView_GotoCompilerError(string message)
    {
      try
      {
        Match match = _compilerErrorRegex.Match(message);
        string sline = match.Result("${line}");
        string scol = match.Result("${column}");
        int line = int.Parse(sline);
        int col  = int.Parse(scol);

        
          _pureScriptController.SetScriptCursorLocation(line-1,col-1);

      }
      catch(Exception)
      {
      }
    }

    #endregion

    #region IScriptController Members

    public void Compile()
    {
      if(_pureScriptController.Apply())
      {
        if(null!=_view)
          _view.ClearCompilerErrors();

        string[] errors;
        IScriptCompilerResult result = Main.Services.ScriptCompilerService.Compile(new string[]{_doc.ScriptText}, out errors);

        if(null==result)
        {
          foreach(string s in errors)
          {
            System.Text.RegularExpressions.Match match = _compilerErrorRegex.Match(s);
            if(match.Success)
            {
              string news = match.Result("(${line},${column}) : ${msg}");
          
              _view.AddCompilerError(news);
            }
            else
            {
              _view.AddCompilerError(s);
            }
          }
  

          Current.GUIFactoryService.ErrorMessageBox("There were compilation errors");
          return;
        }
        else
        {
          _view.AddCompilerError(DateTime.Now.ToLongTimeString() + " : Compilation successful.");
        }
      }

    }

    public void Update()
    {
      _pureScriptController.Apply();
    }

    public void Cancel()
    {
      
    }

    #endregion

    #region IMVCController Members

    public object ModelObject
    {
      get
      {
        return _doc;
      }
    }

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

        _view = value as IScriptView;
        
        Initialize();

        if(_view!=null)
          _view.Controller = this;
      }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      // TODO:  Add ScriptController.Apply implementation
      return false;
    }

    #endregion
  }
}
