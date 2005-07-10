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
   void EhView_RegisterEditableContent(object editableContent);
   void EhView_UnregisterEditableContent(object editableContent);
  }


  public interface IPureScriptController : Main.GUI.IMVCAController
  {
    void SetScriptCursorLocation(int line, int column);
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

    ICSharpCode.SharpDevelop.Services.DefaultParserService _parserService = (ICSharpCode.SharpDevelop.Services.DefaultParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ICSharpCode.SharpDevelop.Services.DefaultParserService));

    public PureScriptController(IPureScriptText doc)
		{
			_doc = doc;
    }


    public void Initialize()
    {
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

    public void EhView_RegisterEditableContent(object editableContent)
    {
      if(_parserService!=null)
      {
        _parserService.RegisterModalContent(editableContent);
      }

    }

    public void EhView_UnregisterEditableContent(object editableContent)
    {
      if(_parserService!=null)
      {
        _parserService.UnregisterModalContent();
      }
    }

    #endregion

    #region IPureScriptController
    public void SetScriptCursorLocation(int line, int column)
    {
      if(_view!=null)
        _view.SetScriptCursorLocation(line,column);
    }
    #endregion
  }
}
