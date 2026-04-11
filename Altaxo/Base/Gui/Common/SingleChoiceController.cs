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

namespace Altaxo.Gui.Common
{
  #region Interfaces

  /// <summary>
  /// Event sink for single-choice view notifications.
  /// </summary>
  public interface ISingleChoiceViewEventSink
  {
    /// <summary>
    /// Handles a change of the selected choice.
    /// </summary>
    /// <param name="newchoice">The new choice index.</param>
    void EhChoiceChanged(int newchoice);
  }

  /// <summary>
  /// This interface must be implemented by controls that allow to choose a single
  /// selection out of multiple values.
  /// </summary>
  public interface ISingleChoiceView
  {
    /// <summary>
    /// Sets the controller.
    /// </summary>
    ISingleChoiceViewEventSink? Controller { set; }

    /// <summary>
    /// Initializes a descriptive text.
    /// </summary>
    /// <param name="value">The descriptive text.</param>
    void InitializeDescription(string value);

    /// <summary>
    /// Initializes the choices and the initial selection.
    /// </summary>
    /// <param name="values">The choices.</param>
    /// <param name="initialchoice">The index of the initial selected choice.</param>
    void InitializeChoice(string[] values, int initialchoice);
  }

  /// <summary>
  /// Provides a model for a single selection among several choices.
  /// </summary>
  public interface ISingleChoiceObject
  {
    /// <summary>
    /// Gets the available choices.
    /// </summary>
    string[] Choices { get; }

    /// <summary>
    /// Gets or sets the selected choice index.
    /// </summary>
    int Selection { get; set; }
  }

  /// <summary>
  /// Default implementation of <see cref="ISingleChoiceObject"/>.
  /// </summary>
  public class SingleChoiceObject : ISingleChoiceObject
  {
    /// <summary>
    /// The available choices.
    /// </summary>
    protected string[] _choices;

    /// <summary>
    /// The selected choice index.
    /// </summary>
    protected int _selection;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleChoiceObject"/> class.
    /// </summary>
    /// <param name="choices">The available choices.</param>
    /// <param name="selection">The initially selected index.</param>
    public SingleChoiceObject(string[] choices, int selection)
    {
      _choices = (string[])choices.Clone();
      _selection = selection;
    }

    /// <inheritdoc/>
    public string[] Choices
    {
      get
      {
        return (string[])_choices.Clone();
      }
    }

    /// <inheritdoc/>
    public int Selection
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
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for a single value. This is a string here, but in derived classes, that can be anything that can be converted to and from a string.
  /// </summary>
  /// <summary>
  /// Controller for a single choice selection.
  /// </summary>
  public class SingleChoiceController : IMVCAController, ISingleChoiceViewEventSink
  {
    /// <summary>
    /// The attached view.
    /// </summary>
    protected ISingleChoiceView? _view;

    /// <summary>
    /// The selectable values.
    /// </summary>
    protected string[] _values = new string[0];

    /// <summary>
    /// The committed choice index.
    /// </summary>
    protected int _choice;

    /// <summary>
    /// The temporary choice index.
    /// </summary>
    protected int _choiceTemp;

    /// <summary>
    /// The description text shown by the controller.
    /// </summary>
    protected string _descriptionText = "Your choice:";

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleChoiceController"/> class.
    /// </summary>
    /// <param name="values">The selectable values.</param>
    /// <param name="selected">The initially selected index.</param>
    public SingleChoiceController(string[] values, int selected)
    {
      Initialize(values, selected);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleChoiceController"/> class.
    /// </summary>
    protected SingleChoiceController()
    {
    }

    /// <summary>
    /// Initializes the selectable values and the selected index.
    /// </summary>
    /// <param name="values">The selectable values.</param>
    /// <param name="selected">The initially selected index.</param>
    protected void Initialize(string[] values, int selected)
    {
      _values = values;
      _choice = _choiceTemp = selected;
    }

    /// <summary>
    /// Initializes the view from the current controller state.
    /// </summary>
    protected virtual void Initialize()
    {
      if (_view is not null)
      {
        _view.InitializeDescription(_descriptionText);
        _view.InitializeChoice(_values, _choice);
      }
    }

    /// <summary>
    /// Gets or sets the description text.
    /// </summary>
    public string DescriptionText
    {
      get
      {
        return _descriptionText;
      }
      set
      {
        _descriptionText = value;
        if (_view is not null)
        {
          _view.InitializeDescription(_descriptionText);
        }
      }
    }

    #region IMVCController Members

    /// <inheritdoc/>
    public virtual object? ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is not null)
          _view.Controller = null;

        _view = value as ISingleChoiceView;

        Initialize();

        if (_view is not null)
          _view.Controller = this;
      }
    }

    /// <inheritdoc/>
    public virtual object ModelObject
    {
      get
      {
        return _choice;
      }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

    /// <inheritdoc/>
    public virtual bool Apply(bool disposeController)
    {
      _choice = _choiceTemp;
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

    #region ISingleChoiceViewEventSink Members

    /// <inheritdoc/>
    public virtual void EhChoiceChanged(int val)
    {
      _choiceTemp = val;
    }

    #endregion ISingleChoiceViewEventSink Members
  }
}
