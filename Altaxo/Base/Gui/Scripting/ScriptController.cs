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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Altaxo.Main.Services.ScriptCompilation;
using Altaxo.Scripting;

namespace Altaxo.Gui.Scripting
{
  #region Interfaces

  /// <summary>
  /// Executes the script provided in the argument.
  /// </summary>
  public delegate bool ScriptExecutionHandler(IScriptText script, IProgressReporter reporter);

  public interface IScriptView : IPureScriptView
  {
    /// <summary>
    /// Occurs when a compiler message was clicked, and the view can not handle this by itself.
    /// </summary>
    event Action<string> CompilerMessageClicked;

    /// <summary>
    /// Compiles the code text. If the view can not compile the text by itself, it should return null.
    /// In this case the controller is responsible for compiling the text, and set the error messages in the view.
    /// </summary>
    IScriptCompilerResult Compile();

    /// <summary>
    /// Sets the compiler errors. Tuple.Item1: line, Tuple.Item2=column, Tuple.Item3: severity level, Tuple.Item4: severity string, Tuple.Item5: message
    /// </summary>
    /// <param name="errors">The errors.</param>
    void SetCompilerErrors(IEnumerable<ICompilerDiagnostic> errors);
  }

  public interface IScriptController : IMVCANController
  {
    void SetText(string text);

    void Compile();

    void Update();

    void Cancel();

    void Execute(IProgressReporter reporter);

    bool HasExecutionErrors();
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for ScriptController.
  /// </summary>
  [UserControllerForObject(typeof(IScriptText), 200)]
  [ExpectedTypeOfView(typeof(IScriptView))]
  public class ScriptController : IScriptController
  {
    private IScriptView _view;
    private IScriptText _doc;
    private IScriptText _tempDoc;
    private IScriptText _compiledDoc;
    protected ScriptExecutionHandler _scriptExecutionHandler;

    //private IPureScriptController _pureScriptController;
    private Regex _compilerErrorRegex = new Regex(@".*\((?<line>\d+),(?<column>\d+)\) : (?<msg>.+)", RegexOptions.Compiled);

    public ScriptController()
    {
    }

    public ScriptController(IScriptText doc)
    {
      InitializeDocument(doc);
    }

    public ScriptController(IScriptText doc, ScriptExecutionHandler exec)
    {
      InitializeDocument(doc, exec);
    }

    #region IMVCANController Members

    public void AttachView()
    {
      _view.CompilerMessageClicked += EhView_GotoCompilerError;
    }

    public void DetachView()
    {
      _view.CompilerMessageClicked -= EhView_GotoCompilerError;
      ;
    }

    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0)
        return false;
      var doc = args[0] as IScriptText;
      if (doc == null)
        return false;

      _doc = doc;

      _tempDoc = _doc.CloneForModification();
      _compiledDoc = null;

      //_pureScriptController = (IPureScriptController)Current.Gui.GetControllerAndControl(new object[] { _tempDoc }, typeof(IPureScriptText), typeof(IPureScriptController), UseDocument.Copy);
      _scriptExecutionHandler = args.Length <= 1 ? null : args[1] as ScriptExecutionHandler;

      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { }
    }

    #endregion IMVCANController Members

    public void SetText(string text)
    {
      if (_view != null)
        _view.ScriptText = text;
    }

    public void Initialize()
    {
      if (_view != null)
      {
        _view.ScriptText = _doc.ScriptText;
        _view.SetCompilerErrors(null);
        _view.InitialScriptCursorLocation = _tempDoc.UserAreaScriptOffset;
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
        int col = int.Parse(scol);

        _view.SetScriptCursorLocation(line, col);
      }
      catch (Exception)
      {
      }
    }

    #endregion IScriptViewEventSink Members

    #region IScriptController Members

    public void Compile()
    {
      var result = _view?.Compile();

      if (null == result) // compilation must be handled by this controller
      {
        InternalCompiling();
      }
      else // Compilation was handled by the view
      {
        _tempDoc.ScriptText = result.ScriptText(0);
        if (result is IScriptCompilerSuccessfulResult succResult)
        {
          _compiledDoc = _tempDoc.CloneForModification();
          _compiledDoc.SetCompilerResult(result);
        }
        else // not successful
        {
          _compiledDoc = null;
        }
      }
    }

    public void InternalCompiling()
    {
      _compiledDoc = null;

      _tempDoc.ScriptText = _view.ScriptText;

      if (null != _view)
        _view.SetCompilerErrors(null);

      IScriptText compiledDoc = _tempDoc.CloneForModification();
      bool result = compiledDoc.Compile();

      var errors = compiledDoc.Errors;
      if (result == false)
      {
        _compiledDoc = null;
        _view.SetCompilerErrors(compiledDoc.Errors);
        Current.Gui.ErrorMessageBox("There were compilation errors");
        return;
      }
      else
      {
        _compiledDoc = compiledDoc;

        _view.SetCompilerErrors(ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Info, DateTime.Now.ToLongTimeString() + " : Compilation successful.")));
      }
    }

    public void Update()
    {
      _tempDoc.ScriptText = _view.ScriptText;

      if (null != _compiledDoc && _tempDoc.ScriptText == _compiledDoc.ScriptText)
      {
        _doc = _compiledDoc;
      }
      else if (_doc.ScriptText != _view.ScriptText)
      {
        if (_doc.IsReadOnly)
          _doc = _doc.CloneForModification();
        _doc.ScriptText = _view.ScriptText;
      }

      _tempDoc = (IScriptText)_doc.Clone();
    }

    public void Cancel()
    {
    }

    public void Execute(IProgressReporter progress)
    {
      _doc.ClearErrors();
      _scriptExecutionHandler?.Invoke(_doc, progress);
    }

    public bool HasExecutionErrors()
    {
      if (null != _doc.Errors && _doc.Errors.Count > 0)
      {
        _view.SetCompilerErrors(_doc.Errors);
        Current.Gui.ErrorMessageBox("There were execution errors");
      }

      return null != _doc.Errors && _doc.Errors.Count > 0;
    }

    #endregion IScriptController Members

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
        if (_view != null)
        {
          DetachView();
        }

        _view = value as IScriptView;

        if (null != _view)
        {
          Initialize();
          AttachView();
        }
      }
    }

    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

    public bool Apply(bool disposeController)
    {
      bool applyresult = false;

      _tempDoc.ScriptText = _view.ScriptText;
      if (null != _compiledDoc && _tempDoc.ScriptText == _compiledDoc.ScriptText)
      {
        _doc = _compiledDoc;
        applyresult = true;
      }
      else
      {
        Compile();
        if (null != _compiledDoc)
        {
          _doc = _compiledDoc;
          applyresult = true;
        }
      }

      return applyresult;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members
  }
}
