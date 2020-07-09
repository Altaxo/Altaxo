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

namespace Altaxo.Serialization
{
  /// <summary>
  /// Helper methods to convert numbers to strings.
  /// Conversion is using the invariant culture.
  /// </summary>
  public class NumberConversion
  {

    public static bool IsDouble(string? txt, out double parsedNumber)
    {
      return double.TryParse(txt, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out parsedNumber);
    }


    public static bool IsDouble(string? txt)
    {
      return IsDouble(txt, out var _);
    }

    /// <summary>
    /// Tests if the provided string represents a number.
    /// </summary>
    /// <param name="txt">The string to test.</param>
    /// <returns>True if the string represents a number.</returns>

    public static bool IsNumeric(string? txt)
    {
      return txt is null ?
        false :
        double.TryParse(txt, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var _);
    }


    public static string ToString(double d)
    {
      return d.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
  }
}
