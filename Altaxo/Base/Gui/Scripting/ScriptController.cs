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

#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Main.Services.ScriptCompilation;
using Altaxo.Scripting;

namespace Altaxo.Gui.Scripting
{
  #region Interfaces

  /// <summary>
  /// Executes the script provided in the argument.
  /// </summary>
  /// <param name="script">The script to execute.</param>
  /// <param name="reporter">The progress reporter used during execution.</param>
  /// <returns><c>true</c> if execution succeeded; otherwise, <c>false</c>.</returns>
  public delegate bool ScriptExecutionHandler(IScriptText script, IProgressReporter reporter);

  /// <summary>
  /// View contract for script editing with compiler diagnostics.
  /// </summary>
  public interface IScriptView : IPureScriptView
  {
    /// <summary>
    /// Occurs when a compiler message was clicked, and the view can not handle this by itself.
    /// </summary>
    event Action<string> CompilerMessageClicked;

    /// <summary>
    /// Sets the compiler errors. Tuple.Item1: line, Tuple.Item2=column, Tuple.Item3: severity level, Tuple.Item4: severity string, Tuple.Item5: message
    /// </summary>
    /// <param name="errors">The errors.</param>
    void SetCompilerErrors(IEnumerable<ICompilerDiagnostic> errors);
  }

  /// <summary>
  /// Contract for script controllers.
  /// </summary>
  public interface IScriptController : IMVCANController
  {
    /// <summary>
    /// Sets the script text in the view.
    /// </summary>
    /// <param name="text">The script text.</param>
    void SetText(string text);

    /// <summary>
    /// Compiles the current script.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel compilation.</param>
    /// <returns>A task that represents the compilation operation.</returns>
    Task Compile(CancellationToken cancellationToken);

    /// <summary>
    /// Updates the model with the current script text.
    /// </summary>
    void Update();

    /// <summary>
    /// Cancels the current operation.
    /// </summary>
    void Cancel();

    /// <summary>
    /// Executes the script.
    /// </summary>
    /// <param name="reporter">The progress reporter used during execution.</param>
    void Execute(IProgressReporter reporter);

    /// <summary>
    /// Determines whether execution errors are present.
    /// </summary>
    /// <returns><c>true</c> if execution errors are present; otherwise, <c>false</c>.</returns>
    bool HasExecutionErrors();
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for editing, compiling, and executing scripts.
  /// </summary>
  [UserControllerForObject(typeof(IScriptText), 200)]
  [ExpectedTypeOfView(typeof(IScriptView))]
  public class ScriptController : IScriptController
  {
    private IScriptView _view;
    private IScriptText _doc;
    private IScriptText _tempDoc;
    private IScriptText _compiledDoc;
    /// <summary>
    /// Delegate that executes the current script.
    /// </summary>
    protected ScriptExecutionHandler _scriptExecutionHandler;

    //private IPureScriptController _pureScriptController;
    private Regex _compilerErrorRegex = new Regex(@".*\((?<line>\d+),(?<column>\d+)\) : (?<msg>.+)", RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptController"/> class.
    /// </summary>
    public ScriptController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptController"/> class.
    /// </summary>
    /// <param name="doc">The script document.</param>
    public ScriptController(IScriptText doc)
    {
      InitializeDocument(doc);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptController"/> class.
    /// </summary>
    /// <param name="doc">The script document.</param>
    /// <param name="exec">The script execution handler.</param>
    public ScriptController(IScriptText doc, ScriptExecutionHandler exec)
    {
      InitializeDocument(doc, exec);
    }

    #region IMVCANController Members

    /// <inheritdoc/>
    public void AttachView()
    {
      _view.CompilerMessageClicked += EhView_GotoCompilerError;
    }

    /// <inheritdoc/>
    public void DetachView()
    {
      _view.CompilerMessageClicked -= EhView_GotoCompilerError;
      ;
    }

    /// <inheritdoc/>
    public bool InitializeDocument(params object[] args)
    {
      if (args is null || args.Length == 0)
        return false;
      var doc = args[0] as IScriptText;
      if (doc is null)
        return false;

      _doc = doc;

      _tempDoc = _doc.CloneForModification();
      _compiledDoc = null;

      //_pureScriptController = (IPureScriptController)Current.Gui.GetControllerAndControl(new object[] { _tempDoc }, typeof(IPureScriptText), typeof(IPureScriptController), UseDocument.Copy);
      _scriptExecutionHandler = args.Length <= 1 ? null : args[1] as ScriptExecutionHandler;

      return true;
    }

    /// <inheritdoc/>
    public UseDocument UseDocumentCopy
    {
      set { }
    }

    #endregion IMVCANController Members

    /// <inheritdoc/>
    public void SetText(string text)
    {
      if (_view is not null)
        _view.ScriptText = text;
    }

    /// <summary>
    /// Initializes the view with the current script data.
    /// </summary>
    public void Initialize()
    {
      if (_view is not null)
      {
        _view.ScriptText = _doc.ScriptText;
        _view.SetCompilerErrors(null);
        _view.InitialScriptCursorLocation = _tempDoc.UserAreaScriptOffset;
      }
    }

    #region IScriptViewEventSink Members

    /// <summary>
    /// Moves the script cursor to the location described by a compiler message.
    /// </summary>
    /// <param name="message">The compiler message that contains the target location.</param>
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

    /// <inheritdoc/>
    public async Task Compile(CancellationToken cancellationToken)
    {
      var scriptCompilerService = Current.GetRequiredService<IScriptCompilerService>();
      var result = await scriptCompilerService.Compile([_view.ScriptText],cancellationToken).ConfigureAwait(true); // need to return to Gui context

      if (result is null) // compilation must be handled by this controller
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
          _view.SetCompilerErrors(ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Info, DateTime.Now.ToLongTimeString() + " : Compilation successful.")));
        }
        else if (result is IScriptCompilerFailedResult failedResult) // not successful
        {
          _compiledDoc = null;
          _view.SetCompilerErrors(failedResult.CompileErrors);
          Current.Gui.ErrorMessageBox("There were compilation errors");
          return;
        }
        else
        {
          throw new NotImplementedException($"Unknown compiler result of type {result?.GetType()}");
        }
      }
    }

    /// <summary>
    /// Performs compilation using the built-in script implementation.
    /// </summary>
    public void InternalCompiling()
    {
      _compiledDoc = null;

      _tempDoc.ScriptText = _view.ScriptText;

      if (_view is not null)
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

    /// <inheritdoc/>
    public void Update()
    {
      _tempDoc.ScriptText = _view.ScriptText;

      if (_compiledDoc is not null && _tempDoc.ScriptText == _compiledDoc.ScriptText)
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

    /// <inheritdoc/>
    public void Cancel()
    {
    }

    /// <inheritdoc/>
    public void Execute(IProgressReporter reporter)
    {
      _doc.ClearErrors();
      _scriptExecutionHandler?.Invoke(_doc, reporter);
    }

    /// <inheritdoc/>
    public bool HasExecutionErrors()
    {
      if (_doc.Errors is not null && _doc.Errors.Count > 0)
      {
        _view.SetCompilerErrors(_doc.Errors);
        Current.Gui.ErrorMessageBox("There were execution errors");
      }

      return _doc.Errors is not null && _doc.Errors.Count > 0;
    }

    #endregion IScriptController Members

    #region IMVCController Members

    /// <inheritdoc/>
    public object ModelObject
    {
      get
      {
        return _doc;
      }
    }

    /// <inheritdoc/>
    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is not null)
        {
          DetachView();
        }

        _view = value as IScriptView;

        if (_view is not null)
        {
          Initialize();
          AttachView();
        }
      }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

    /// <inheritdoc/>
    public bool Apply(bool disposeController)
    {
      bool applyresult = false;

      _tempDoc.ScriptText = _view.ScriptText;
      if (_compiledDoc is not null && _tempDoc.ScriptText == _compiledDoc.ScriptText)
      {
        _doc = _compiledDoc;
        applyresult = true;
      }
      else
      {
        var task =  Compile(default);

        while(true)
        {
          if(task.IsCanceled)
            { break; }
          if(task.IsCompleted)
            { break; }
          Thread.Yield();
        }

        if (_compiledDoc is not null)
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
