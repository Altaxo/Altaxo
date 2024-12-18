﻿#region Copyright

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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization
{
  /// <summary>
  /// Responsible for converting user input (dialogs and controls) into data and vice versa. The user preferences for locality are
  /// used by this class.
  /// </summary>
  public static partial class GUIConversion
  {
    /// <summary>Settings how the data entered by the user should be interpreted and how the data should be presented in the user interface.</summary>
    private static System.Globalization.CultureInfo _cultureSettings;

    static GUIConversion()
    {
      _cultureSettings = System.Globalization.CultureInfo.InvariantCulture;
      if (Current.PropertyService is not null)
      {
        Current.PropertyService.PropertyChanged += EhPropertyService_PropertyChanged;
        EhPropertyService_PropertyChanged(null, new PropertyChangedEventArgs(Altaxo.Settings.CultureSettings.PropertyKeyUICulture.GuidString));
      }
    }

    private static void EhPropertyService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == Altaxo.Settings.CultureSettings.PropertyKeyUICulture.GuidString)
      {
        _cultureSettings = Current.PropertyService.GetValue<Altaxo.Settings.CultureSettings>(Altaxo.Settings.CultureSettings.PropertyKeyUICulture, Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin).Culture;
      }
    }

    public static System.Globalization.CultureInfo CultureSettings
    {
      get
      {
        return _cultureSettings;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException("value");
        _cultureSettings = value;
      }
    }

    public static string ToNumberStringWithUnit(double number, string unit)
    {
      int l = (int)Math.Floor(Math.Log10(Math.Abs(number)) / 3);

      string? pre = null;
      switch (l)
      {
        case -5:
          pre = "a";
          break;

        case -4:
          pre = "p";
          break;

        case -3:
          pre = "n";
          break;

        case -2:
          pre = "µ";
          break;

        case -1:
          pre = "m";
          break;

        case 0:
          pre = "";
          break;

        case 1:
          pre = "K";
          break;

        case 2:
          pre = "M";
          break;

        case 3:
          pre = "G";
          break;

        case 4:
          pre = "T";
          break;
      }

      if (pre is null)
        return number.ToString() + " " + unit;

      double m = number / Math.Pow(10, 3 * l);
      return m.ToString(_cultureSettings) + " " + pre + unit;
    }

    public static string? ToNumberStringNullIfNaN(double val)
    {
      if (double.IsNaN(val))
        return null;
      else
        return val.ToString(_cultureSettings);
    }

    #region DateTime

    /// <summary>
    /// Is the provided string a date/time?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <returns>True if the string can successfully parsed to a DateTime object.</returns>
    public static bool IsDateTime(string s)
    {
      return IsDateTime(s, out var o);
    }

    /// <summary>
    /// Is the provided string a date/time?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="val">The value parsed (only valid if parsing was successful).</param>
    /// <returns>True if the string can successfully parsed to a DateTime object.</returns>
    public static bool IsDateTime(string s, out DateTime val)
    {
      return DateTime.TryParse(s, _cultureSettings, System.Globalization.DateTimeStyles.AssumeLocal, out val);
    }

    public static string ToString(DateTime o)
    {
      return o.ToString(_cultureSettings);
    }

    /// <summary>
    /// Is the provided string a time span?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <returns>True if the string can successfully parsed to a TimeSpan object.</returns>
    public static bool IsTimeSpan(string s)
    {
      return IsTimeSpan(s, out var o);
    }

    /// <summary>
    /// Is the provided string a date/time?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="val">The value parsed (only valid if parsing was successful).</param>
    /// <returns>True if the string can successfully parsed to a DateTime object.</returns>
    public static bool IsTimeSpan(string s, out TimeSpan val)
    {
      return TimeSpan.TryParse(s, _cultureSettings.DateTimeFormat, out val);
    }

    public static string ToString(TimeSpan o)
    {
      return o.ToString(string.Empty, _cultureSettings);
    }

    #endregion DateTime

    #region Double

    /// <summary>
    /// Is the provided string a double?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <returns>True if the string can successfully parsed to a double.</returns>
    public static bool IsDouble(string s)
    {
      return IsDouble(s, out var o);
    }

    /// <summary>
    /// Is the provided string a floating point value?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="val">The value parsed (only valid if parsing was successful).</param>
    /// <returns>True if the string can successfully parsed to a double.</returns>
    public static bool IsDouble(string s, out double val)
    {
      return double.TryParse(s, System.Globalization.NumberStyles.Float, _cultureSettings, out val);
    }

    public static bool IsDoubleOrNull(string s, out double? val)
    {
      if (string.IsNullOrEmpty(s))
      {
        val = null;
        return true;
      }
      else
      {
        if (IsDouble(s, out var val1))
        {
          val = val1;
          return true;
        }
        else
        {
          val = null;
          return false;
        }
      }
    }

    public static bool IsInt32OrNull(string s, out int? val)
    {
      if (string.IsNullOrEmpty(s))
      {
        val = null;
        return true;
      }
      else
      {
        if (int.TryParse(s, System.Globalization.NumberStyles.Integer, _cultureSettings, out var val1))
        {
          val = val1;
          return true;
        }
        else
        {
          val = null;
          return false;
        }
      }
    }

    public static string ToString(double val)
    {
      return val.ToString(_cultureSettings);
    }

    public static string ToString(double? val)
    {
      if (val is null)
        return string.Empty;
      else
        return ((double)val).ToString(_cultureSettings);
    }

    public static string ToString(double val, int accuracy)
    {
      return val.ToString("G" + accuracy.ToString(), _cultureSettings);
    }

    public static string ToString(double val, string format)
    {
      return val.ToString(format, _cultureSettings);
    }

    /*
        public static string ToString(double[] vals)
        {
            StringBuilder stb = new StringBuilder();
            foreach (int v in vals)
            {
                stb.Append(ToString(v));
                stb.Append(" ");
            }
            return stb.ToString().TrimEnd();
        }
        */

    public static string ToString(ICollection<double> vals)
    {
      var stb = new StringBuilder();
      foreach (int v in vals)
      {
        stb.Append(ToString(v));
        stb.Append(" ");
      }
      return stb.ToString().TrimEnd();
    }

    public static bool TryParseMultipleDouble(string s, [MaybeNullWhen(false)] out double[] vals)
    {
      vals = null;
      bool failed = false;
      string[] parts = s.Split(new char[] { ' ', '\t', '\r', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries);
      double[] result = new double[parts.Length];

      for (int i = 0; i < result.Length; i++)
      {
        if (!IsDouble(parts[i], out result[i]))
        {
          failed = true;
          break;
        }
      }

      if (failed)
      {
        return false;
      }
      else
      {
        vals = result;
        return true;
      }
    }

    #endregion Double

    #region Integer

    /// <summary>
    /// Is the provided string an integer value?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="val">The value parsed (only valid if parsing was successful).</param>
    /// <returns>True if the string can successfully parsed to a DateTime object.</returns>
    public static bool IsInteger(string s, out int val)
    {
      return int.TryParse(s, System.Globalization.NumberStyles.Integer, _cultureSettings, out val);
    }

    public static string ToString(int val)
    {
      return val.ToString(_cultureSettings);
    }

    public static string ToString(int? val)
    {
      if (val is null)
        return string.Empty;
      else
        return ((int)val).ToString(_cultureSettings);
    }

    public static string ToString(int[] vals)
    {
      var stb = new StringBuilder();
      foreach (int v in vals)
      {
        stb.Append(v.ToString());
        stb.Append(" ");
      }
      return stb.ToString().TrimEnd();
    }

    public static string ToString(ICollection<int> vals)
    {
      var stb = new StringBuilder();
      foreach (int v in vals)
      {
        stb.Append(v.ToString());
        stb.Append(" ");
      }
      return stb.ToString().TrimEnd();
    }

    public static bool TryParseMultipleInt32(string s, [MaybeNullWhen(false)] out int[] vals)
    {
      vals = null;
      bool failed = false;
      string[] parts = s.Split(new char[] { ' ', '\t', '\r', '\n', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
      int[] result = new int[parts.Length];

      for (int i = 0; i < result.Length; i++)
      {
        if (!int.TryParse(parts[i], System.Globalization.NumberStyles.Integer, _cultureSettings, out result[i]))
        {
          failed = true;
          break;
        }
      }

      if (failed)
        return false;

      vals = result;
      return true;
    }

    #endregion Integer

    #region Percent-Units

    /// <summary>
    /// Converts a value (0 to 1) in percents (i.e. 0 to 100) and returns it as text together with the percent char.
    /// </summary>
    /// <param name="value">Value (0..1)</param>
    /// <returns>A text string: the value together with the unit.</returns>
    public static string GetPercentMeasureText(double value)
    {
      return ToString(value * 100, "G5") + " %";
    }

    /// <summary>
    /// Get a percentage value from a text string.
    /// </summary>
    /// <param name="txt">Text string. Consists of a number and optionally a percent char.</param>
    /// <param name="value">On return, gives the actual value, but in relative units (not 0..100, but 0..1).</param>
    /// <returns>True if the conversion was successful, false otherwise.</returns>
    public static bool GetPercentMeasureValue(
      string txt,
      ref double value)
    {
      txt = txt.Trim().ToLower();
      string end = "%";
      if (txt.EndsWith(end))
      {
        txt = txt.Substring(0, txt.Length - end.Length).TrimEnd();
      }

      if (IsDouble(txt, out var v))
      {
        value = v / 100;
        return true;
      }
      else
      {
        return false;
      }
    }

    #endregion Percent-Units

    #region Selection lists

    /// <summary>
    /// For a given enum value, this gives the list of possible choices for that enumeration (must not be a flag enumeration).
    /// </summary>
    /// <param name="value">The enum value that is currently selected.</param>
    /// <returns>List of all enumeration values. The current value is marked as (Selected is true for this list node).</returns>
    public static Altaxo.Collections.SelectableListNodeList GetListOfChoices(Enum value)
    {
      var list = new Altaxo.Collections.SelectableListNodeList();
      GetListOfChoices(value, list);
      return list;
    }

    /// <summary>
    /// For a given enum value, this gives the list of possible choices for that enumeration (must not be a flag enumeration).
    /// </summary>
    /// <param name="value">The enum value that is currently selected.</param>
    /// <param name="list">List to be filled with choices</param>
    public static void GetListOfChoices(Enum value, Altaxo.Collections.SelectableListNodeList list)
    {
      list.Clear();
      Type enumtype = value.GetType();
      foreach (var v in Enum.GetValues(enumtype).OfType<Enum>())
      {
        string name = Current.Gui.GetUserFriendlyName(v);
        list.Add(new Altaxo.Collections.SelectableListNode(name, v, Enum.Equals(v, value)));
      }
    }

    #endregion Selection lists
  }
}
