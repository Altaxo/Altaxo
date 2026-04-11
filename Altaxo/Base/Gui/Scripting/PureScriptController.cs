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
using Altaxo.Scripting;

namespace Altaxo.Gui.Scripting
{
  #region Interfaces

  /// <summary>
  /// View interface for editing a pure script text.
  /// </summary>
  public interface IPureScriptView
  {
    /// <summary>
    /// Gets or sets the script text.
    /// </summary>
    string ScriptText { get; set; }

    /// <summary>
    /// Sets the current script cursor location.
    /// </summary>
    int ScriptCursorLocation { set; }

    /// <summary>
    /// Sets the initial script cursor location.
    /// </summary>
    int InitialScriptCursorLocation { set; }

    /// <summary>
    /// Sets the script cursor location by line and column.
    /// </summary>
    /// <param name="line">The one-based line number.</param>
    /// <param name="column">The one-based column number.</param>
    void SetScriptCursorLocation(int line, int column);

    /// <summary>
    /// Marks the specified text range.
    /// </summary>
    /// <param name="pos1">The start position of the marked range.</param>
    /// <param name="pos2">The end position of the marked range.</param>
    void MarkText(int pos1, int pos2);
  }

  /// <summary>
  /// Controller interface for editing <see cref="IPureScriptText"/>.
  /// </summary>
  public interface IPureScriptController : IMVCANController
  {
    /// <summary>
    /// Initializes or reinitializes the script controller.
    /// </summary>
    /// <param name="text">The script text to display.</param>
    void SetText(string text);

    /// <summary>
    /// Sets the cursor location inside the script. Line and column are starting with 1.
    /// </summary>
    /// <param name="line">Script line (1-based).</param>
    /// <param name="column">Script column (1-based).</param>
    void SetScriptCursorLocation(int line, int column);

    /// <summary>
    /// Sets the cursor location using a character offset.
    /// </summary>
    /// <param name="offset">The zero-based character offset.</param>
    void SetScriptCursorLocation(int offset);

    /// <summary>
    /// Sets the initial cursor location using a character offset.
    /// </summary>
    /// <param name="offset">The zero-based character offset.</param>
    void SetInitialScriptCursorLocation(int offset);

    /// <summary>
    /// Gets the most current script text (if a view is present, it returns the script text of the view).
    /// </summary>
    /// <returns>The current script text.</returns>
    string GetCurrentScriptText();

    /// <summary>
    /// Gets the script model.
    /// </summary>
    IPureScriptText Model { get; }
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for editing <see cref="IPureScriptText"/>.
  /// </summary>
  [UserControllerForObject(typeof(IPureScriptText))]
  [ExpectedTypeOfView(typeof(IPureScriptView))]
  public class PureScriptController : IPureScriptController
  {
    private IPureScriptView _view;
    private IPureScriptText _doc;

    /// <summary>
    /// Initializes a new instance of the <see cref="PureScriptController"/> class.
    /// </summary>
    public PureScriptController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PureScriptController"/> class.
    /// </summary>
    /// <param name="doc">Script document to edit.</param>
    public PureScriptController(IPureScriptText doc)
    {
      InitializeDocument(doc);
    }

    #region IMVCANController Members

    /// <inheritdoc/>
    public bool InitializeDocument(params object[] args)
    {
      if (args is null || args.Length == 0)
        return false;
      var doc = args[0] as IPureScriptText;
      if (doc is null)
        return false;

      _doc = doc;

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
    /// Initializes the view from the current document.
    /// </summary>
    public void Initialize()
    {
      if (_view is not null)
        _view.ScriptText = _doc.ScriptText;
    }

    /// <summary>
    /// Attaches the view.
    /// </summary>
    public void AttachView()
    {
    }

    /// <summary>
    /// Detaches the view.
    /// </summary>
    public void DetachView()
    {
    }

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

        _view = value as IPureScriptView;

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
      if (_view is not null)
      {
        _doc.ScriptText = _view.ScriptText;
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Tries to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>true</c> if the revert operation was successful; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members

    #region IPureScriptController

    /// <summary>
    /// Sets the cursor location inside the script. Line and column are starting with 1.
    /// </summary>
    /// <param name="line">Script line (1-based).</param>
    /// <param name="column">Script column (1-based).</param>
    public void SetScriptCursorLocation(int line, int column)
    {
      if (_view is not null)
        _view.SetScriptCursorLocation(line, column);
    }

    /// <inheritdoc/>
    public void SetScriptCursorLocation(int offset)
    {
      if (_view is not null)
        _view.ScriptCursorLocation = offset;
    }

    /// <inheritdoc/>
    public void SetInitialScriptCursorLocation(int offset)
    {
      if (_view is not null)
        _view.InitialScriptCursorLocation = offset;
    }

    /// <summary>
    /// Gets the most current script text (if a view is present, it returns the script text of the view).
    /// </summary>
    /// <returns></returns>
    public string GetCurrentScriptText()
    {
      if (_view is not null)
        return _view.ScriptText;
      else
        return _doc.ScriptText;
    }

    /// <inheritdoc/>
    public IPureScriptText Model
    {
      get
      {
        return _doc;
      }
    }

    #endregion IPureScriptController
  }
}
