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
    public static bool Is<T>(this System.Enum type, T value)
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

    public static T WithFlag<T>(this System.Enum type, T flag, bool value)
    {
      if (value)
        return WithSetFlag(type, flag);
      else
        return WithClearedFlag(type, flag);
    }

    public static T WithSetFlag<T>(this System.Enum type, T value)
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

    public static T WithClearedFlag<T>(this System.Enum type, T value)
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
