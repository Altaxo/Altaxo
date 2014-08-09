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

using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Serialization
{
	/// <summary>
	/// Responsible for converting user input (dialogs and controls) into data and vice versa. The user preferences for locality are
	/// used by this class.
	/// </summary>
	public static class GUIConversion
	{
		/// <summary>Settings how the data entered by the user should be interpreted and how the data should be presented in the user interface.</summary>
		private static System.Globalization.CultureInfo _cultureSettings;

		static GUIConversion()
		{
			if (null != Current.PropertyService)
			{
				Current.PropertyService.PropertyChanged += new Action<string>(EhPropertyService_PropertyChanged);
				EhPropertyService_PropertyChanged(Altaxo.Settings.CultureSettings.PropertyKeyUICulture.GuidString);
			}
		}

		private static void EhPropertyService_PropertyChanged(string key)
		{
			if (key == Altaxo.Settings.CultureSettings.PropertyKeyUICulture.GuidString)
			{
				_cultureSettings = Current.PropertyService.GetValue<Altaxo.Settings.CultureSettings>(Altaxo.Settings.CultureSettings.PropertyKeyUICulture, Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin).Culture;
			}
		}

		public static System.Globalization.CultureInfo CultureSettings
		{
			set
			{
				if (null == value)
					throw new ArgumentNullException("value");
				_cultureSettings = value;
			}
		}

		#region DateTime

		/// <summary>
		/// Is the provided string a date/time?
		/// </summary>
		/// <param name="s">The string to parse</param>
		/// <returns>True if the string can successfully parsed to a DateTime object.</returns>
		public static bool IsDateTime(string s)
		{
			DateTime o;
			return IsDateTime(s, out o);
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
			TimeSpan o;
			return IsTimeSpan(s, out o);
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
			return o.ToString(String.Empty, _cultureSettings);
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
			double o;
			return IsDouble(s, out o);
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
				double val1;
				if (IsDouble(s, out val1))
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
				int val1;
				if (int.TryParse(s, System.Globalization.NumberStyles.Integer, _cultureSettings, out val1))
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
			if (val == null)
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
			StringBuilder stb = new StringBuilder();
			foreach (int v in vals)
			{
				stb.Append(ToString(v));
				stb.Append(" ");
			}
			return stb.ToString().TrimEnd();
		}

		public static bool TryParseMultipleDouble(string s, out double[] vals)
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
				return false;

			vals = result;
			return true;
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
			if (val == null)
				return string.Empty;
			else
				return ((int)val).ToString(_cultureSettings);
		}

		public static string ToString(int[] vals)
		{
			StringBuilder stb = new StringBuilder();
			foreach (int v in vals)
			{
				stb.Append(v.ToString());
				stb.Append(" ");
			}
			return stb.ToString().TrimEnd();
		}

		public static string ToString(ICollection<int> vals)
		{
			StringBuilder stb = new StringBuilder();
			foreach (int v in vals)
			{
				stb.Append(v.ToString());
				stb.Append(" ");
			}
			return stb.ToString().TrimEnd();
		}

		public static bool TryParseMultipleInt32(string s, out int[] vals)
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

		#region AltaxoVariant

		public static string ToString(ICollection<AltaxoVariant> vals)
		{
			StringBuilder stb = new StringBuilder();
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
			AltaxoVariant[] result = new AltaxoVariant[parts.Length];

			for (int i = 0; i < result.Length; i++)
			{
				DateTime dt;
				double dd;
				if (IsDouble(parts[i], out dd))
				{
					result[i] = dd;
				}
				else if (IsDateTime(parts[i], out dt))
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

			double v;
			if (IsDouble(txt, out v))
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

		#region Percent-Units

		/// <summary>
		/// Converts a value (0 to 1) in percents (i.e. 0 to 100) and returns it as text together with the percent char.
		/// </summary>
		/// <param name="value">Value (0..1)</param>
		/// <returns>A text string: the value together with the unit.</returns>
		public static string GetPercentMeasureText(double value)
		{
			return GUIConversion.ToString(value * 100, "G5") + " %";
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

			double v;
			if (IsDouble(txt, out v))
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
			Altaxo.Collections.SelectableListNodeList list = new Altaxo.Collections.SelectableListNodeList();
			Type enumtype = value.GetType();
			foreach (Enum v in Enum.GetValues(enumtype))
			{
				string name = Current.Gui.GetUserFriendlyName(v);
				list.Add(new Altaxo.Collections.SelectableListNode(name, v, Enum.Equals(v, value)));
			}
			return list;
		}

		#endregion Selection lists
	}
}