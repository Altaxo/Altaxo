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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Serialization
{
  /// <summary>
  /// Responsible for converting user input (dialogs and controls) into data and vice versa. The user preferences for locality are
  /// used by this class.
  /// </summary>
  public static partial class GUIConversion
  {
    #region AltaxoVariant

    public static string ToString(IEnumerable<AltaxoVariant> vals)
    {
      var stb = new StringBuilder();
      bool first = true;
      foreach (AltaxoVariant v in vals)
      {
        if (first)
          first = false;
        else
          stb.Append("; ");

        stb.Append(v.ToString());
      }
      return stb.ToString();
    }

    public static bool TryParseMultipleAltaxoVariant(string s, out AltaxoVariant[] vals)
    {
      vals = null;
      bool failed = false;
      string[] parts = s.Split(new char[] { '\t', '\r', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries);
      var result = new AltaxoVariant[parts.Length];

      for (int i = 0; i < result.Length; i++)
      {
        if (IsDouble(parts[i], out var dd))
        {
          result[i] = dd;
        }
        else if (IsDateTime(parts[i], out var dt))
        {
          result[i] = dt;
        }
        else
        {
          result[i] = parts[i];
        }
      }

      if (failed)
        return false;

      vals = result;
      return true;
    }

    #endregion AltaxoVariant

    #region Length-Units (mm, cm, inch and so on)

    private static LengthUnit _lastLengthUnitUsed = LengthUnit.Point;

    public static LengthUnit LastUsedLengthUnit
    {
      get
      {
        return _lastLengthUnitUsed;
      }
      set
      {
        _lastLengthUnitUsed = value;
      }
    }

    /// <summary>
    /// Converts a value (unit: points) in a given unit and returns it as text together with the unit.
    /// </summary>
    /// <param name="value">Value of length in points.</param>
    /// <param name="lastUnit">The unit to convert to.</param>
    /// <returns>A text string: the value together with the unit.</returns>
    public static string GetLengthMeasureText(double value, LengthUnit lastUnit)
    {
      double v = lastUnit.ConvertFrom(value, LengthUnit.Point);
      return GUIConversion.ToString(v, "G5") + " " + lastUnit.Shortcut;
    }

    /// <summary>
    /// Converts a value (unit: points) in the length unit last used and returns it as text together with the unit.
    /// </summary>
    /// <param name="value">Value of length in points.</param>
    /// <returns>A text string: the value together with the unit.</returns>
    public static string GetLengthMeasureText(double value)
    {
      return GetLengthMeasureText(value, LastUsedLengthUnit);
    }

    /// <summary>
    /// Get a length value from a text string.
    /// </summary>
    /// <param name="txt">Text string. Consists of a number and optionally a unit.</param>
    /// <param name="unit">Gives the default unit to use if the text string don't contain a unit.
    /// On return, contains the unit actually used.</param>
    /// <param name="value">On return, gives the actual length (unit:points).</param>
    /// <returns>True if the conversion was successful, false otherwise.</returns>
    public static bool GetLengthMeasureValue(
      string txt,
      ref LengthUnit unit,
      ref double value)
    {
      txt = txt.Trim().ToLower();
      LengthUnit tempUnit = unit;
      foreach (string end in LengthUnit.Shortcuts)
      {
        if (txt.EndsWith(end))
        {
          tempUnit = LengthUnit.FromShortcut(end);
          txt = txt.Substring(0, txt.Length - end.Length).TrimEnd();
          break;
        }
      }

      if (IsDouble(txt, out var v))
      {
        value = LengthUnit.Point.ConvertFrom(v, tempUnit);
        unit = tempUnit;
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Get a length value from a text string.
    /// </summary>
    /// <param name="txt">Text string. Consists of a number and optionally a unit.</param>
    /// <param name="value">On return, gives the actual length (unit:points).</param>
    /// <returns>True if the conversion was successful, false otherwise. The last used length unit is updated by this function.</returns>
    public static bool GetLengthMeasureValue(
      string txt,
      ref double value)
    {
      return GetLengthMeasureValue(txt, ref _lastLengthUnitUsed, ref value);
    }

    #endregion Length-Units (mm, cm, inch and so on)
  }
}
