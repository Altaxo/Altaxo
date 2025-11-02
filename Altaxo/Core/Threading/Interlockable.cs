#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

namespace Altaxo.Threading
{
  /// <summary>
  /// Manages a value that can be safely accessed by different threads.
  /// </summary>
  /// <typeparam name="T">The type of value.</typeparam>
  public class Interlockable<T>
  {
    private T _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Interlockable{T}"/> class.
    /// </summary>
    /// <param name="t">The initial value.</param>
    public Interlockable(T t)
    {
      _value = t;
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public T Value
    {
      get
      {
        lock (this)
        {
          return _value;
        }
      }
      set
      {
        lock (this)
        {
          _value = value;
        }
      }
    }

    /// <summary>
    /// Replaces the value with a new value.
    /// </summary>
    /// <param name="newValue">The new value.</param>
    /// <returns>The old value.</returns>
    public T Exchange(T newValue)
    {
      lock (this)
      {
        var oldValue = _value;
        _value = newValue;
        return oldValue;
      }
    }
    /// <summary>
    /// Modifies the value, by using the provided function. Since the function is executed in
    /// the locked state, execution must be kept short!
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The old value, and the new value.</returns>
    public (T OldValue, T NewValue) Modify(Func<T, T> func)
    {
      lock (this)
      {
        var oldValue = _value;
        var newValue = func(oldValue);
        _value = newValue;
        return (oldValue, newValue);
      }
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="Interlockable{T}"/> to T/>.
    /// </summary>
    /// <param name="t">The interlockable instance.</param>
    /// <returns>
    /// The value that is managed by the interlockable.
    /// </returns>
    public static implicit operator T(Interlockable<T> t)
    {
      return t.Value;
    }

    /// <summary>
    /// Performs an explicit conversion from the value type T to <see cref="Interlockable{T}"/>.
    /// </summary>
    /// <param name="t">The initial value.</param>
    /// <returns>The <see cref="Interlockable{T}"/> that holds the initial.</returns>
    public static explicit operator Interlockable<T>(T t) // Explicite because otherwise unintended assignment of a new instance of Interlockable can happen
    {
      return new Interlockable<T>(t);
    }
  }
}
