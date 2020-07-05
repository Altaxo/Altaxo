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
  public class ValidationEventArgs<T> : EventArgs
  {
    private string? _errors;
    private T _valueToValidate;

    public System.Globalization.CultureInfo CultureInfo { get; private set; }

    public ValidationEventArgs(T valueToValidate)
    {
      _valueToValidate = valueToValidate;
      CultureInfo = System.Globalization.CultureInfo.InvariantCulture;
    }

    public ValidationEventArgs(T valueToValidate, System.Globalization.CultureInfo info)
    {
      _valueToValidate = valueToValidate;
      CultureInfo = info;
    }

    public void AddError(string format, params object[] args)
    {
      if (_errors == null)
        _errors = string.Format(format, args);
      else
        _errors += "\n" + string.Format(format, args);
    }

    public T ValueToValidate
    {
      get
      {
        return _valueToValidate;
      }
    }

    public bool Cancel
    {
      get
      {
        return null != _errors;
      }
    }

    public bool HasErrors
    {
      get
      {
        return null != _errors;
      }
    }

    public string ErrorText
    {
      get
      {
        return _errors ?? string.Empty;
      }
    }
  }

  /// <summary>
  /// Event handler to validate content of Gui elements.
  /// </summary>
  /// <param name="sender">Sender of the validation request.</param>
  /// <param name="e">Validating event args. In case that the validation is not successfull, the receiver has to add an error message to the event args.</param>
  public delegate void ValidatingEventHandler<T>(object sender, ValidationEventArgs<T> e);

  /// <summary>
  /// Event handler to validate content of Gui elements that contain strings (like TextBox).
  /// </summary>
  /// <param name="sender">Sender of the validation request.</param>
  /// <param name="e">Validating event args. In case that the validation is not successfull, the receiver has to add an error message to the event args.</param>
  public delegate void ValidatingStringEventHandler(object sender, ValidationEventArgs<string> e);
}
