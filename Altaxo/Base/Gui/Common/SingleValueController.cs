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

namespace Altaxo.Gui.Common
{
  #region Interfaces

  /// <summary>
  /// Defines the view contract for editing a single string-backed value.
  /// </summary>
  public interface ISingleValueView
  {
    /// <summary>
    /// Sets the description text.
    /// </summary>
    string DescriptionText { set; }

    /// <summary>
    /// Gets or sets the value text.
    /// </summary>
    string ValueText { get; set; }

    /// <summary>
    /// Occurs when the value text is being validated.
    /// </summary>
    event Action<ValidationEventArgs<string>> ValueText_Validating;
  }

  /// <summary>
  /// Defines the controller contract for single-value editors.
  /// </summary>
  public interface ISingleValueController : IMVCAController
  {
    /// <summary>
    /// Gets or sets the description text.
    /// </summary>
    string DescriptionText { get; set; }
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for a single value. This is a string here, but in derived classes, that can be anything that can be converted to and from a string.
  /// </summary>
  [UserControllerForObject(typeof(string), 100)]
  [ExpectedTypeOfView(typeof(ISingleValueView))]
  public class SingleValueController : ISingleValueController
  {
    /// <summary>
    /// The attached view.
    /// </summary>
    protected ISingleValueView? _view;

    /// <summary>
    /// The committed value string.
    /// </summary>
    protected string _value1String;

    /// <summary>
    /// The temporary value string.
    /// </summary>
    protected string _value1StringTemporary;

    /// <summary>
    /// The description text.
    /// </summary>
    protected string _descriptionText = "Enter value:";

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleValueController"/> class.
    /// </summary>
    /// <param name="val">The initial text value.</param>
    public SingleValueController(string val)
    {
      _value1String = val;
      _value1StringTemporary = val;
    }

    /// <summary>
    /// Initializes the view from the current controller state.
    /// </summary>
    protected virtual void Initialize()
    {
      if (_view is not null)
      {
        _view.DescriptionText = _descriptionText;
        _view.ValueText = _value1StringTemporary;
      }
    }

    /// <inheritdoc/>
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
          _view.DescriptionText = _descriptionText;
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
          _view.ValueText_Validating -= EhView_ValidatingValue1;

        _view = value as ISingleValueView;

        if (_view is not null)
        {
          Initialize();
          _view.ValueText_Validating += EhView_ValidatingValue1;
        }
      }
    }

    /// <inheritdoc/>
    public virtual object ModelObject
    {
      get
      {
        return _value1String;
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
      _value1String = _value1StringTemporary;
      return true;
    }

    /// <inheritdoc/>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members

    #region ISingleValueViewEventSink Members

    /// <summary>
    /// Handles validation of the value text.
    /// </summary>
    /// <param name="e">The validation event arguments.</param>
    public virtual void EhView_ValidatingValue1(ValidationEventArgs<string> e)
    {
      _value1StringTemporary = e.ValueToValidate;
      return;
    }

    #endregion ISingleValueViewEventSink Members
  }
}
