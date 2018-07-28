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
using System.Text;

namespace Altaxo.Serialization
{
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

      _shortcutToUnit = new SortedDictionary<string, LengthUnit>();
      _shortcutToUnit.Add(_millimeter.Shortcut, _millimeter);
      _shortcutToUnit.Add(_centimeter.Shortcut, _centimeter);
      _shortcutToUnit.Add(_mil.Shortcut, _mil);
      _shortcutToUnit.Add(_point.Shortcut, _point);
      _shortcutToUnit.Add(_inch.Shortcut, _inch);

      // Alternative shortcuts
      _shortcutToUnit.Add("Mil", _mil);
      _shortcutToUnit.Add("Inch", _inch);
      _shortcutToUnit.Add("inch", _inch);

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

    public string Name { get { return _name; } }

    public string Shortcut { get { return _shortcut; } }

    public decimal UnitInMeter { get { return _factorToMeter; } }

    public double ConvertFrom(double fromlength, LengthUnit fromunit)
    {
      return fromlength * (double)(fromunit.UnitInMeter / this.UnitInMeter);
    }

    public static LengthUnit Millimeter { get { return _millimeter; } }

    public static LengthUnit Centimeter { get { return _centimeter; } }

    public static LengthUnit Mil { get { return _mil; } }

    public static LengthUnit Point { get { return _point; } }

    public static LengthUnit Inch { get { return _inch; } }

    public static LengthUnit FromShortcut(string shortcut)
    {
      return _shortcutToUnit[shortcut];
    }

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
    public static bool TryParse(string s, out LengthUnit lengthUnit, out string remainder)
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
