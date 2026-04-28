#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Collections.Generic;

namespace Altaxo.Collections
{
  /// <summary>
  /// Boxes a single value into a class. The value can be get/set by the property <see cref="Value"/>. Since the value is accessible by a property, this class can be used in collections as data sources for data grids, etc.
  /// For this, it additionally implements both <see cref="System.ComponentModel.INotifyPropertyChanged"/> and <see cref="System.ComponentModel.IEditableObject"/>
  /// </summary>
  /// <typeparam name="T">The type of the value to be boxed.</typeparam>
  public class Boxed<T> : System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.IEditableObject
  {
    /// <summary>Event arg for the property changed event.</summary>
    private static readonly System.ComponentModel.PropertyChangedEventArgs _valueChangedEventArg = new System.ComponentModel.PropertyChangedEventArgs(nameof(Value));

    /// <summary>Occurs when the boxed value changed.</summary>
    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public T Value
    {
      get { return field; }
      set
      {
        var oldValue = field;
        field = value;
        if (PropertyChanged is { } propChanged && !object.Equals(field, oldValue))
          propChanged(this, _valueChangedEventArg);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Boxed&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="val">The value to be boxed.</param>
    public Boxed(T val)
    {
      Value = val;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Boxed&lt;T&gt;"/> class with a default value.
    /// </summary>
    public Boxed()
    {
      Value = default!;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return obj is not Boxed<T> other ? false : Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return Value is null ? typeof(T).GetHashCode() : Value.GetHashCode();
    }

    /// <summary>
    /// Determines whether this instance is equal to another boxed value.
    /// </summary>
    /// <param name="other">The object to compare with.</param>
    /// <returns><see langword="true"/> if the boxed values are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(Boxed<T>? other)
    {
      if (other is null)
        return false;
      if (Value is null && other.Value is null)
        return true;
      if (Value is null || other.Value is null)
        return false;
      return Value.Equals(other.Value);
    }





    /// <summary>
    /// Performs an implicit conversion from <see cref="Altaxo.Collections.Boxed&lt;T&gt;"/> to <see cref="T:T"/>.
    /// </summary>
    /// <param name="val">The boxed value</param>
    /// <returns>
    /// The unboxed value.
    /// </returns>
    public static implicit operator T(Boxed<T> val)
    {
      return val.Value;
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="T:T"/> to <see cref="Altaxo.Collections.Boxed&lt;T&gt;"/>.
    /// </summary>
    /// <param name="val">The value to be boxed.</param>
    /// <returns>
    /// The value wrapped in an instance of this class.
    /// </returns>
    public static implicit operator Boxed<T>(T val)
    {
      return new Boxed<T>(val);
    }

    /// <summary>
    /// Begins an edit on an object.
    /// </summary>
    public virtual void BeginEdit()
    {
    }

    /// <summary>
    /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
    /// </summary>
    public virtual void CancelEdit()
    {
    }

    /// <summary>
    /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> or <see cref="M:System.ComponentModel.IBindingList.AddNew"/> call into the underlying object.
    /// </summary>
    public virtual void EndEdit()
    {
    }

    /// <summary>
    /// Wraps the specified items in <see cref="Boxed{T}"/> instances.
    /// </summary>
    /// <param name="items">The items to wrap.</param>
    /// <returns>The boxed items.</returns>
    public static IEnumerable<Boxed<T>> ToBoxedItems(IEnumerable<T> items)
    {
      foreach (var item in items)
        yield return new Boxed<T>(item);
    }

    /// <summary>
    /// Adds boxed versions of the specified source items to the destination collection.
    /// </summary>
    /// <param name="destination">The destination collection.</param>
    /// <param name="sourceItems">The source items to wrap and add.</param>
    public static void AddRange(ICollection<Boxed<T>> destination, IEnumerable<T> sourceItems)
    {
      foreach (var item in sourceItems)
        destination.Add(new Boxed<T>(item));
    }

    /// <summary>
    /// Returns the unboxed values from the specified boxed items.
    /// </summary>
    /// <param name="boxedItems">The boxed items.</param>
    /// <returns>The unboxed values.</returns>
    public static IEnumerable<T> ToUnboxedItems(IEnumerable<Boxed<T>> boxedItems)
    {
      foreach (var item in boxedItems)
        yield return item.Value;
    }

    /// <summary>
    /// Adds unboxed values from the specified boxed items to the destination collection.
    /// </summary>
    /// <param name="destination">The destination collection.</param>
    /// <param name="sourceItems">The boxed items whose values are added.</param>
    public static void AddRange(ICollection<T> destination, IEnumerable<Boxed<T>> sourceItems)
    {
      foreach (var item in sourceItems)
        destination.Add(item.Value);
    }

    /// <summary>
    /// Copies the specified boxed items into a new array of unboxed values.
    /// </summary>
    /// <param name="boxedItems">The boxed items.</param>
    /// <returns>A new array containing the unboxed values.</returns>
    public static T[] ToUnboxedArray(ICollection<Boxed<T>> boxedItems)
    {
      var arr = new T[boxedItems.Count];
      int i = 0;
      foreach (var item in boxedItems)
      {
        arr[i] = item.Value;
        ++i;
      }
      return arr;
    }
  }
}
