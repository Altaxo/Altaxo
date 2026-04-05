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

#nullable enable
using System;
using System.ComponentModel;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Document class that bundles a text question with a predefined set of choices.
  /// </summary>
  public class TextChoice
  {
    /// <summary>
    /// The available choices.
    /// </summary>
    protected string[] _choices;

    /// <summary>
    /// The selected choice index.
    /// </summary>
    protected int _selection;
    private bool _allowFreeText;
    private string? _choosenText;
    private string _description = string.Empty;
    private Func<string, string>? _textValidationFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextChoice"/> class.
    /// </summary>
    public TextChoice(string[] choices, int selection, bool allowFreeText)
    {
      _choices = (string[])choices.Clone();
      _selection = selection;
      _allowFreeText = allowFreeText;
    }

    /// <summary>
    /// Gets the available choices.
    /// </summary>
    public string[] Choices
    {
      get
      {
        return (string[])_choices.Clone();
      }
    }

    /// <summary>
    /// If a predefined choice is selected, this gets the index of the choice. If free text is
    /// given, the value is -1.
    /// </summary>
    public int SelectedIndex
    {
      get
      {
        return _selection;
      }
      set
      {
        _selection = value;
      }
    }

    /// <summary>
    /// Gets/sets whether free text choice is allowed or not allowed.
    /// </summary>
    public bool AllowFreeText
    {
      get
      {
        return _allowFreeText;
      }
      set
      {
        _allowFreeText = value;
      }
    }

    /// <summary>
    /// Get/sets the Text that is choosen by the user or entered as free text.
    /// </summary>
    public string? Text
    {
      get { return _choosenText; }
      set { _choosenText = value; }
    }

    /// <summary>
    /// Description string that is shown close to the gui element where you enter the text.
    /// </summary>
    public string Description
    {
      get { return _description; }
      set { _description = value; }
    }

    /// <summary>
    /// Gets or sets the text validation function.
    /// </summary>
    public Func<string, string>? TextValidationFunction
    {
      get
      {
        return _textValidationFunction;
      }
      set
      {
        _textValidationFunction = value;
      }
    }
  }

  /// <summary>
  /// View interface for choosing either predefined text or free text.
  /// </summary>
  public interface IFreeTextChoiceView
  {
    /// <summary>
    /// Fired if the user has changed the selection from the selection list.
    /// </summary>
    public event Action<int> SelectionChangeCommitted;

    /// <summary>Fired if free text was entered and needs to be validated. Make sure that this event is fired only,
    /// if free text was entered, and is not fired if the user has choosen from the selection list.</summary>
    public event Action<string, CancelEventArgs> TextValidating;

    /// <summary>
    /// Sets the description text.
    /// </summary>
    /// <param name="value">The description text.</param>
    public void SetDescription(string value);

    /// <summary>
    /// Sets the available choices.
    /// </summary>
    /// <param name="values">The available choices.</param>
    /// <param name="initialselection">The initial selection index.</param>
    /// <param name="allowFreeText">If set to <see langword="true"/>, free text input is allowed.</param>
    public void SetChoices(string[] values, int initialselection, bool allowFreeText);
  }


  /// <summary>
  /// Provides controller logic for editing <see cref="TextChoice"/> instances.
  /// </summary>
  [UserControllerForObject(typeof(TextChoice))]
  [ExpectedTypeOfView(typeof(IFreeTextChoiceView))]
  public class TextChoiceController : IMVCANController
  {
    private IFreeTextChoiceView? _view;
    private TextChoice? _doc;

    private Exception NoDocumentException => new InvalidOperationException("This controller is not yet initialized with a document!");


    private void Initialize(bool initData)
    {
      if (_doc is null)
        throw NoDocumentException;

      if (_view is not null)
      {
        _view.SetDescription(_doc.Description);
        _view.SetChoices(_doc.Choices, _doc.SelectedIndex, _doc.AllowFreeText);
      }
    }

    private void EhSelectionChangeCommitted(int selIndex)
    {
      if (_doc is null)
        throw NoDocumentException;

      _doc.SelectedIndex = selIndex;

      if (selIndex >= 0)
        _doc.Text = _doc.Choices[selIndex];
    }

    private void EhTextValidating(string text, CancelEventArgs e)
    {
      if (_doc is null)
        throw NoDocumentException;

      string? validationResult = null;
      if (_doc.TextValidationFunction is not null)
      {
        validationResult = _doc.TextValidationFunction(text);
      }
      if (validationResult is null)
      {
        _doc.Text = text;
        _doc.SelectedIndex = -1;
      }
      else
      {
        e.Cancel = true;
      }
    }

    #region IMVCANController Members

    /// <inheritdoc/>
    public bool InitializeDocument(params object[] args)
    {
      if (args is null || 0 == args.Length || !(args[0] is TextChoice))
        return false;

      _doc = (TextChoice)args[0];
      return true;
    }

    /// <inheritdoc/>
    public UseDocument UseDocumentCopy
    {
      set { }
    }

    #endregion IMVCANController Members

    #region IMVCController Members

    /// <inheritdoc/>
    public object? ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is not null)
        {
          _view.SelectionChangeCommitted -= EhSelectionChangeCommitted;
          _view.TextValidating -= EhTextValidating;
        }

        _view = value as IFreeTextChoiceView;

        if (_view is not null)
        {
          Initialize(false);
          _view.SelectionChangeCommitted += EhSelectionChangeCommitted;
          _view.TextValidating += EhTextValidating;
        }
      }
    }

    /// <inheritdoc/>
    public object ModelObject
    {
      get
      {
        if (_doc is null)
          throw NoDocumentException;
        return _doc;
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
      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successful; <c>false</c> if the revert operation was not possible, that is, because the controller has not stored the original state of the model.
    /// </returns>
    /// <inheritdoc/>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members
  }
}
