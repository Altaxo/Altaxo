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
using System.Text;
using System.Drawing;
using System.Collections.Generic;

namespace Altaxo.Graph
{
  /// <summary>
  /// Type of colors that is shown e.g. in comboboxes.
  /// </summary>
  [Serializable]
  public enum ColorType
  {
    /// <summary>
    /// Known colors and system colors are shown.
    /// </summary>
    KnownAndSystemColor,
    /// <summary>
    /// Known colors are shown.
    /// </summary>
    KnownColor,
    /// <summary>
    /// Only plot colors are shown.
    /// </summary>
    PlotColor
  }

  public static class ColorDictionary
  {
    static Dictionary<string, Color> _nameToColor;
    static Dictionary<Color, string> _colorToName;
    static List<Color> _knownAndSystemColors;
    static List<Color> _knownColors;
    static List<Color> _plotColors;

    static ColorDictionary()
    {
      _nameToColor = new Dictionary<string, Color>();
      _colorToName = new Dictionary<Color, string>(new MyColorComparer());
      _knownAndSystemColors = new List<Color>();
      _knownColors = new List<Color>();

      foreach (object o in Enum.GetValues(typeof(KnownColor)))
      {
        Color c = Color.FromKnownColor((KnownColor)o);
        try
        {
          _nameToColor.Add(c.Name, c);
          
          if(!_colorToName.ContainsKey(c)) // Some different names give the same color, so we can use only the first name here
            _colorToName.Add(c, c.Name);
          
          _knownAndSystemColors.Add(c);
          if (!c.IsSystemColor)
            _knownColors.Add(c);
        }
        catch (Exception)
        {

        }
      }
    }

    public static string GetColorName(Color c)
    {
      if (c.IsNamedColor)
        return c.Name;
      else
        return _colorToName[c];
    }
    public static string GetBaseColorName(Color c)
    {
      if (c.IsNamedColor)
        return c.Name;

      if(_colorToName.ContainsKey(c))
        return _colorToName[c];
      else
        return _colorToName[Color.FromArgb(c.R, c.G, c.B)];
    }

    public static bool IsColorNamed(Color c)
    {
      return c.IsNamedColor || _colorToName.ContainsKey(c);
    }

    public static bool IsBaseColorNamed(Color c)
    {
      if (c.IsNamedColor)
        return true;

      if (_colorToName.ContainsKey(c))
        return true;
      else
        return _colorToName.ContainsKey(Color.FromArgb(c.R, c.G, c.B));
    }


    public static bool IsNameKnown(string name)
    {
      return _nameToColor.ContainsKey(name);
    }

    public static Color GetColor(string name)
    {
      return _nameToColor[name];
    }


    public static Color[] GetKnownAndSystemColors()
    {
      return _knownAndSystemColors.ToArray();
    }

    public static Color[] GetKnownColors()
    {
      return _knownColors.ToArray();
    }

    public static Color[] GetPlotColors()
    {
      return PlotColors.Colors.ToArray();
    }

    public static Color[] GetColorsOfType(ColorType type)
    {
      Color[] result = null;
      switch (type)
      {
        case ColorType.PlotColor:
          result = GetPlotColors();
          break;
        case ColorType.KnownColor:
          result =GetKnownColors();
          break;
        case ColorType.KnownAndSystemColor:
          result =  GetKnownAndSystemColors();
          break;
      }
      return result;
    }

    public static bool IsColorOfType(Color c, ColorType type)
    {
      bool result = false;
      switch (type)
      {
        case ColorType.PlotColor:
          {
            result = PlotColors.Colors.IsPlotColor(c);
          }
          break;
        case ColorType.KnownColor:
          {
            if (_colorToName.ContainsKey(c))
            {
              Color cc = _nameToColor[_colorToName[c]];
              result = !(cc.IsSystemColor);
            }
          }
          break;
        case ColorType.KnownAndSystemColor:
          {
            result = _colorToName.ContainsKey(c);
          }
          break;
      }
      return result;
    }

    public static Color GetNormalizedColor(Color c, ColorType type)
    {
      Color result = c;
      switch (type)
      {
        case ColorType.PlotColor:
          if (_colorToName.ContainsKey(c))
          {
            result = _nameToColor[_colorToName[c]];
          }
          break;
        case ColorType.KnownColor:
        case ColorType.KnownAndSystemColor:
          if (_colorToName.ContainsKey(c))
          {
            result = _nameToColor[_colorToName[c]];
          }
          break;
      }
      return result;
    }


    class MyColorComparer : IEqualityComparer<Color>
    {
      #region IEqualityComparer<Color> Members

      public bool Equals(Color x, Color y)
      {
        if (x.IsSystemColor || y.IsSystemColor)
          return ( x == y );
        else
          return ( x.R == y.R && x.G == y.G && x.B == y.B && x.A == y.A );
      }

      public int GetHashCode(Color x)
      {
        if (x.IsSystemColor)
          return x.GetHashCode();
        else
          return Color.FromArgb(x.A, x.R, x.G, x.B).GetHashCode();
      }

      #endregion
    }
  }

  public class PlotColor
  {
    System.Drawing.Color _color;
    string _name;

    public PlotColor(System.Drawing.Color color, string name)
    {
      _color = color;
      _name = name;
    }

    public System.Drawing.Color Color
    {
      get { return _color; }
    }

    public string Name
    {
      get { return _name; }
    }

    public static implicit operator System.Drawing.Color(PlotColor c)
    {
      return c._color;
    }

    public override bool Equals(object obj)
    {
      if (obj is PlotColor)
        return this._color == ((PlotColor)obj)._color;

      return false;
    }

    public override int GetHashCode()
    {
      return this._color.GetHashCode();
    }
  }

  public class PlotColors : System.Collections.CollectionBase
  {

    static PlotColors _instance = new PlotColors();
    public static PlotColors Colors
    {
      get
      {
        return _instance;
      }
    }

    private PlotColors()
    {
      Add(new PlotColor(System.Drawing.Color.Black, "Black"));
      Add(new PlotColor(System.Drawing.Color.Red, "Red"));
      Add(new PlotColor(System.Drawing.Color.Green, "Green"));
      Add(new PlotColor(System.Drawing.Color.Blue, "Blue"));
      Add(new PlotColor(System.Drawing.Color.Magenta, "Magenta"));
      Add(new PlotColor(System.Drawing.Color.Yellow, "Yellow"));
      Add(new PlotColor(System.Drawing.Color.Coral, "Coral"));
    }

    public PlotColor this[int i]
    {
      get
      {
        return (PlotColor)InnerList[i];
      }
      set
      {
        InnerList[i] = value;
      }
    }

    public void Add(PlotColor c)
    {
      InnerList.Add(c);
    }

    public Color[] ToArray()
    {
      Color[] result = new Color[Count];
      for (int i = Count - 1; i >= 0; i--)
        result[i] = this[i].Color;
      return result;
    }

    public int IndexOf(Color c)
    {
      for (int i = 0; i < Count; i++)
      {
        if (c.ToArgb() == this[i].Color.ToArgb())
        {
          return i;
        }
      }
      return -1;
    }

    public PlotColor GetPlotColor(Color c)
    {
      int i = IndexOf(c);
      return i < 0 ? null : this[i];
    }

    public bool IsPlotColor(Color c)
    {
      return IndexOf(c) >= 0;
    }

    public string GetPlotColorName(Color c)
    {
      int i = IndexOf(c);
      return i < 0 ? null : this[i].Name;
    }

    public PlotColor GetNextPlotColor(Color c)
    {
      return GetNextPlotColor(c, 1);
    }

    public PlotColor GetNextPlotColor(Color c, int step)
    {
      int wraps;
      return GetNextPlotColor(c, step, out wraps);
    }

    public PlotColor GetNextPlotColor(Color c, int step, out int wraps)
    {
      int i = IndexOf(c);
      if (i >= 0)
      {
        wraps = Calc.BasicFunctions.NumberOfWraps(Count, i, step);
        return this[Calc.BasicFunctions.PMod(i + step, Count)];

      }
      else
      {
        // default if the color was not found
        wraps = 0;
        return this[0];
      }
    }


  }
}
