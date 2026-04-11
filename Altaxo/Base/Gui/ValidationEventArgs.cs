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

namespace Altaxo.Gui
{
  /// <summary>
  /// Provides validation state and collected error messages for a value being validated.
  /// </summary>
  /// <typeparam name="T">The type of the value to validate.</typeparam>
  public class ValidationEventArgs<T> : EventArgs
  {
    private string? _errors;
    private T _valueToValidate;

    /// <summary>
    /// Gets the culture information to use during validation.
    /// </summary>
    public System.Globalization.CultureInfo CultureInfo { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationEventArgs{T}"/> class.
    /// </summary>
    /// <param name="valueToValidate">The value to validate.</param>
    public ValidationEventArgs(T valueToValidate)
    {
      _valueToValidate = valueToValidate;
      CultureInfo = System.Globalization.CultureInfo.InvariantCulture;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationEventArgs{T}"/> class.
    /// </summary>
    /// <param name="valueToValidate">The value to validate.</param>
    /// <param name="info">The culture information to use during validation.</param>
    public ValidationEventArgs(T valueToValidate, System.Globalization.CultureInfo info)
    {
      _valueToValidate = valueToValidate;
      CultureInfo = info;
    }

    /// <summary>
    /// Adds an error message to the validation result.
    /// </summary>
    /// <param name="format">The composite format string for the error message.</param>
    /// <param name="args">The arguments used to format the error message.</param>
    public void AddError(string format, params object[] args)
    {
      if (_errors is null)
        _errors = string.Format(format, args);
      else
        _errors += "\n" + string.Format(format, args);
    }

    /// <summary>
    /// Gets the value to validate.
    /// </summary>
    public T ValueToValidate
    {
      get
      {
        return _valueToValidate;
      }
    }

    /// <summary>
    /// Gets a value indicating whether validation should be canceled.
    /// </summary>
    public bool Cancel
    {
      get
      {
        return _errors is not null;
      }
    }

    /// <summary>
    /// Gets a value indicating whether any validation errors have been collected.
    /// </summary>
    public bool HasErrors
    {
      get
      {
        return _errors is not null;
      }
    }

    /// <summary>
    /// Gets the concatenated validation error text.
    /// </summary>
    public string ErrorText
    {
      get
      {
        return _errors ?? string.Empty;
      }
    }
  }

  /// <summary>
  /// Represents an event handler used to validate content of GUI elements.
  /// </summary>
  /// <typeparam name="T">The type of value being validated.</typeparam>
  /// <param name="sender">Sender of the validation request.</param>
  /// <param name="e">Validation event arguments. If validation is not successful, the receiver adds an error message to the event arguments.</param>
  public delegate void ValidatingEventHandler<T>(object sender, ValidationEventArgs<T> e);

  /// <summary>
  /// Represents an event handler used to validate content of GUI elements that contain strings, such as a text box.
  /// </summary>
  /// <param name="sender">Sender of the validation request.</param>
  /// <param name="e">Validation event arguments. If validation is not successful, the receiver adds an error message to the event arguments.</param>
  public delegate void ValidatingStringEventHandler(object sender, ValidationEventArgs<string> e);
}
