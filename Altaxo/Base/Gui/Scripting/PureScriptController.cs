using System;

using Altaxo.Data;
using Altaxo.Main.GUI;

namespace Altaxo.Worksheet.GUI
{
  #region Interfaces
  public interface IPureScriptView
  {
    IPureScriptViewEventSink Controller {get; set; }
    string ScriptText { get; set; }

    int ScriptCursorLocation { set; }
    void SetScriptCursorLocation(int line, int column);
    void MarkText(int pos1, int pos2);
  }

  public interface IPureScriptViewEventSink
  {
  }


  public interface IPureScriptController : Main.GUI.IMVCAController
  {

    // Initializes or reinitializes the script controller.
    void SetText(string text);

    void SetScriptCursorLocation(int line, int column);

    void SetScriptCursorLocation(int offset);
    
    /// <summary>
    /// Gets the most current script text (if a view is present, it returns the script text of the view).
    /// </summary>
    /// <returns></returns>
    string GetCurrentScriptText();

    IPureScriptText Model { get; }

  }

  #endregion

	/// <summary>
	/// Summary description for PureScriptController.
	/// </summary>
	[UserControllerForObject(typeof(IPureScriptText))]
	public class PureScriptController : IPureScriptController, IPureScriptViewEventSink
	{
    IPureScriptView _view;
    IPureScriptText _doc;
 
    public PureScriptController(IPureScriptText doc)
		{
			_doc = doc;
    }

    public void SetText(string text)
    {
      if(_view!=null)
        _view.ScriptText = text;
    }

    public void Initialize()
    {
      if (_view != null)
        _view.ScriptText = _doc.ScriptText;
    }

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

        _view = value as IPureScriptView;
        
        Initialize();

        if(_view!=null)
          _view.Controller = this;
      }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      if(_view!=null)
      {
        _doc.ScriptText = _view.ScriptText;
        return true;
      }
      else
      {
        return false;
      }
    }

    #endregion

    #region IPureScriptViewEventSink Members

   

    #endregion

    #region IPureScriptController
    public void SetScriptCursorLocation(int line, int column)
    {
      if(_view!=null)
        _view.SetScriptCursorLocation(line,column);
    }

    public void SetScriptCursorLocation(int offset)
    {
      if(_view!=null)
        _view.ScriptCursorLocation = offset;
    }

    /// <summary>
    /// Gets the most current script text (if a view is present, it returns the script text of the view).
    /// </summary>
    /// <returns></returns>
    public string GetCurrentScriptText()
    {
      if (_view != null)
        return _view.ScriptText;
      else
        return _doc.ScriptText;
    }

    public IPureScriptText Model
    {
      get
      {
        return _doc;
      }
    }
    #endregion
  }
}
