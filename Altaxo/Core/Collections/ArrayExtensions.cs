#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2021 Dr. Dirk Lellinger
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
using System.Collections.Generic;

namespace Altaxo.Collections
{
  /// <summary>
  /// Provides some extensions for arrays.
  /// </summary>
  public static class ArrayExtensions
  {
    /// <summary>
    /// Clones the array deep, i.e. the elements will be cloned for the new array.
    /// </summary>
    /// <typeparam name="T">The type of element.</typeparam>
    /// <param name="array">The array to clone.</param>
    /// <returns>Cloned array, with each of the elements a clone from the original element.</returns>
    /// <exception cref="NullReferenceException">nameof(array)</exception>
    /// <inheritdoc/>
    public static T[] CloneDeep<T>(this T[] array) where T : class, ICloneable
    {
      if (array is null)
        throw new NullReferenceException(nameof(array));

      var result = new T[array.Length];
      for (int i = 0; i < result.Length; ++i)
      {
        result[i] = array[i] is { } ele ? (T)ele.Clone() : (T)null!;
      }
      return result;
    }

    /// <summary>
    /// Clones the array deep, i.e. the elements will be cloned for the new array. Here, elements which have a
    /// value of null are allowed.
    /// </summary>
    /// <typeparam name="T">The type of element.</typeparam>
    /// <param name="array">The array to clone.</param>
    /// <returns>Cloned array, with each of the elements a clone from the original element.</returns>
    /// <exception cref="NullReferenceException">nameof(array)</exception>
    /// <inheritdoc/>
    public static T?[] CloneDeepNullable<T>(this T?[] array) where T : class, ICloneable
    {
      if (array is null)
        throw new NullReferenceException(nameof(array));

      var result = new T?[array.Length];
      for (int i = 0; i < result.Length; ++i)
      {
        result[i] = array[i] is { } ele ? (T)ele.Clone() : (T?)null;
      }
      return result;
    }

    /// <summary>
    /// Tests whether two arrays are equal.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="x">The x array.</param>
    /// <param name="y">The y array.</param>
    /// <param name="comparer">The comparer that is used to compare two elements. If <c>null</c> is provided, then the default equality comparer is used.</param>
    /// <returns><c>true</c> if the arrays are equal, or both are <c>null</c>; otherwise, <c>false</c>.</returns>
    public static bool AreEqual<T>(T[]? x, T[]? y, IEqualityComparer<T>? comparer = null)
    {
      if (object.ReferenceEquals(x, y))
        return true;

      if (x is null || y is null)
        return false;

      if (x.Length != y.Length)
        return false;

      comparer ??= EqualityComparer<T>.Default;

      for (int i = 0; i < x.Length; i++)
      {
        if (!comparer.Equals(x[i], y[i]))
          return false;
      }
      return true;
    }
  }
}
