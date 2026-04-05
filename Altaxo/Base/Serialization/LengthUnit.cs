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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Serialization
{
  /// <summary>
  /// Represents a supported length unit and provides unit-conversion helpers.
  /// </summary>
  public class LengthUnit
  {
    private decimal _factorToMeter;
    private string _name;
    private string _shortcut;

    private static LengthUnit _millimeter;
    private static LengthUnit _centimeter;
    private static LengthUnit _mil;
    private static LengthUnit _point;
    private static LengthUnit _inch;
    private static SortedDictionary<string, LengthUnit> _shortcutToUnit;
    private static List<string> _shortcuts;

    static LengthUnit()
    {
      _millimeter = new LengthUnit(((decimal)1) / 1000, "Millimeter", "mm");
      _centimeter = new LengthUnit(((decimal)1) / 100, "Centimeter", "cm");
      _mil = new LengthUnit(((decimal)254) / 10000000, "Mil", "mil");
      _point = new LengthUnit(((decimal)254) / 720000, "Point", "pt");
      _inch = new LengthUnit(((decimal)254) / 10000, "Inch", "\"");

      _shortcutToUnit = new SortedDictionary<string, LengthUnit>
      {
        { _millimeter.Shortcut, _millimeter },
        { _centimeter.Shortcut, _centimeter },
        { _mil.Shortcut, _mil },
        { _point.Shortcut, _point },
        { _inch.Shortcut, _inch },

        // Alternative shortcuts
        { "Mil", _mil },
        { "Inch", _inch },
        { "inch", _inch }
      };

      _shortcuts = new List<string>();
      foreach (string k in _shortcutToUnit.Keys)
        _shortcuts.Add(k);
      _shortcuts.Sort();
    }

    private LengthUnit(decimal factor, string name, string shortcut)
    {
      _factorToMeter = factor;
      _name = name;
      _shortcut = shortcut;
    }

    /// <summary>
    /// Gets the display name of the unit.
    /// </summary>
    public string Name { get { return _name; } }

    /// <summary>
    /// Gets the shortcut of the unit.
    /// </summary>
    public string Shortcut { get { return _shortcut; } }

    /// <summary>
    /// Gets the size of the unit in meters.
    /// </summary>
    public decimal UnitInMeter { get { return _factorToMeter; } }

    /// <summary>
    /// Converts a length value from the specified source unit into this unit.
    /// </summary>
    /// <param name="fromlength">The length value to convert.</param>
    /// <param name="fromunit">The source unit.</param>
    /// <returns>The converted length value.</returns>
    public double ConvertFrom(double fromlength, LengthUnit fromunit)
    {
      return fromlength * (double)(fromunit.UnitInMeter / UnitInMeter);
    }

    /// <summary>
    /// Gets the millimeter unit.
    /// </summary>
    public static LengthUnit Millimeter { get { return _millimeter; } }

    /// <summary>
    /// Gets the centimeter unit.
    /// </summary>
    public static LengthUnit Centimeter { get { return _centimeter; } }

    /// <summary>
    /// Gets the mil unit.
    /// </summary>
    public static LengthUnit Mil { get { return _mil; } }

    /// <summary>
    /// Gets the typographic point unit.
    /// </summary>
    public static LengthUnit Point { get { return _point; } }

    /// <summary>
    /// Gets the inch unit.
    /// </summary>
    public static LengthUnit Inch { get { return _inch; } }

    /// <summary>
    /// Gets a length unit from its shortcut.
    /// </summary>
    /// <param name="shortcut">The unit shortcut.</param>
    /// <returns>The matching length unit.</returns>
    public static LengthUnit FromShortcut(string shortcut)
    {
      return _shortcutToUnit[shortcut];
    }

    /// <summary>
    /// Gets the list of recognized unit shortcuts.
    /// </summary>
    public static string[] Shortcuts
    {
      get
      {
        return _shortcuts.ToArray();
      }
    }

    private static readonly char[] _digitsOrSpace = new char[] { ' ', '\t', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    /// <summary>
    /// Parse a string that ends with a length unit to return the length unit. The string is trimmed at the end before use.
    /// </summary>
    /// <param name="s">String to parse.</param>
    /// <param name="lengthUnit">On success, returns the length unit parsed.</param>
    /// <param name="remainder">On success, returns the part of the input string, which belongs not to the length unit.</param>
    /// <returns>True if successfull, otherwise false.</returns>
    public static bool TryParse(string s, [MaybeNullWhen(false)] out LengthUnit lengthUnit, [MaybeNullWhen(false)] out string remainder)
    {
      s = s.TrimEnd();

      for (int i = _shortcuts.Count - 1; i >= 0; i--)
      {
        if (s.EndsWith(_shortcuts[i]))
        {
          lengthUnit = _shortcutToUnit[_shortcuts[i]];
          remainder = s.Substring(0, s.Length - _shortcuts[i].Length);
          return true;
        }
      }

      lengthUnit = null;
      remainder = null;
      return false;
    }
  }
}
