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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
  }
}
