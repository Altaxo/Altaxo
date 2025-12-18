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

using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Gui
{
  /// <summary>
  /// Helper class that wraps a single value of type <typeparamref name="T"/> and implements the <see cref="System.ComponentModel.INotifyPropertyChanged"/> interface,
  /// so that changes to the value can be monitored by the GUI.
  /// This class is required when using a <c>DataGrid</c>, since plain values (such as <see cref="string"/>, <see cref="int"/>, etc.) are not directly editable.
  /// </summary>
  /// <typeparam name="T">Type of the value to wrap.</typeparam>
  public class NotifyChangedValue<T> : System.ComponentModel.INotifyPropertyChanged
  {
    [MaybeNull]
    private T _value;

    /// <inheritdoc/>
    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyChangedValue{T}"/> class, setting the value to its default.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public NotifyChangedValue()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyChangedValue{T}"/> class with the given value.
    /// </summary>
    /// <param name="value">Initial value.</param>
    public NotifyChangedValue(T value)
    {
      _value = value;
    }

    /// <summary>
    /// Gets or sets the wrapped value. If the new value differs from the old one, the <see cref="PropertyChanged"/> event is raised.
    /// </summary>
    [MaybeNull]
    public T Value
    {
      get
      {
        return _value;
      }
      set
      {
        var oldValue = _value;
        _value = value;
        if (!object.Equals(oldValue, value))
          PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Value)));
      }
    }
  }
}
