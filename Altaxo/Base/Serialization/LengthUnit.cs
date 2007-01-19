#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Serialization
{
  public class LengthUnit
  {
    decimal _factorToMeter;
    string _name;
    string _shortcut;

    static LengthUnit _millimeter;
    static LengthUnit _centimeter;
    static LengthUnit _mil;
    static LengthUnit _point;
    static LengthUnit _inch;
    static Dictionary<string, LengthUnit> _shortcutToUnit;
    static List<string> _shortcuts;

    static LengthUnit()
    {
      _millimeter = new LengthUnit(((decimal)1)/1000, "Millimeter", "mm");
      _centimeter = new LengthUnit(((decimal)1)/ 100, "Centimeter", "cm");
      _mil = new LengthUnit(((decimal)254) / 10000000, "Mil", "mil");
      _point = new LengthUnit(((decimal)254) / 720000, "Point", "pt");
      _inch = new LengthUnit(((decimal)254) /   10000, "Inch", "\"");

      
     
      _shortcutToUnit = new Dictionary<string, LengthUnit>();
      _shortcutToUnit.Add(_millimeter.Shortcut,_millimeter);
      _shortcutToUnit.Add(_centimeter.Shortcut, _centimeter);
      _shortcutToUnit.Add(_mil.Shortcut, _mil);
      _shortcutToUnit.Add(_point.Shortcut, _point);
      _shortcutToUnit.Add(_inch.Shortcut, _inch);


      _shortcuts = new List<string>();
      foreach (string k in _shortcutToUnit.Keys)
        _shortcuts.Add(k);
      _shortcuts.Sort();
    }


    private LengthUnit(decimal factor, string name, string shortcut )
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


  }
}
