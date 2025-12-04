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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Extends the operations for Flag Enumerations with setting and clearing of individual flags.
  /// </summary>
  public static class EnumerationExtensions
  {
    /// <summary>
    /// Determines whether the enumeration value is equal to the specified value.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="type">The enum value to check.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>True if the values are equal; otherwise, false.</returns>
    public static bool Is<T>(this System.Enum type, T value) where T : struct
    {
      try
      {
        return (int)(object)type == (int)(object)value;
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the enum value with the specified flag set or cleared, depending on the value argument.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="type">The enum value to modify.</param>
    /// <param name="flag">The flag to set or clear.</param>
    /// <param name="value">If true, the flag is set; if false, the flag is cleared.</param>
    /// <returns>The modified enum value.</returns>
    public static T WithFlag<T>(this System.Enum type, T flag, bool value) where T : struct
    {
      if (value)
        return WithSetFlag(type, flag);
      else
        return WithClearedFlag(type, flag);
    }

    /// <summary>
    /// Returns the enum value with the specified flag set.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="type">The enum value to modify.</param>
    /// <param name="value">The flag to set.</param>
    /// <returns>The modified enum value with the flag set.</returns>
    public static T WithSetFlag<T>(this System.Enum type, T value) where T : struct
    {
      try
      {
        return (T)(object)(((int)(object)type | (int)(object)value));
      }
      catch (Exception ex)
      {
        throw new ArgumentException(
            string.Format(
                "Could not append value from enumerated type '{0}'.",
                typeof(T).Name
                ), ex);
      }
    }

    /// <summary>
    /// Returns the enum value with the specified flag cleared.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="type">The enum value to modify.</param>
    /// <param name="value">The flag to clear.</param>
    /// <returns>The modified enum value with the flag cleared.</returns>
    public static T WithClearedFlag<T>(this System.Enum type, T value) where T : struct
    {
      try
      {
        return (T)(object)(((int)(object)type & ~(int)(object)value));
      }
      catch (Exception ex)
      {
        throw new ArgumentException(
            string.Format(
                "Could not remove value from enumerated type '{0}'.",
                typeof(T).Name
                ), ex);
      }
    }
  }
}
