#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2021 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Properties
{
  /// <summary>
  /// Parser for a property string that consists of key-value pairs like this: 'key1=value1 key2=value2 key3=value3 ...'. The keys must
  /// not contain whitespace; whereas for the values it is allowed to contain whitespace.
  /// </summary>
  public class PropertyStringParser
  {
    private Dictionary<string, string> _properties;
    private CultureInfo _culture;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyStringParser"/> class. Parsing is done with the invariant culture.
    /// </summary>
    /// <param name="propertyString">The property string to parse.</param>
    public PropertyStringParser(string propertyString)
      : this(propertyString, CultureInfo.InvariantCulture)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyStringParser"/> class.
    /// </summary>
    /// <param name="propertyString">The property string to parse.</param>
    /// <param name="culture">The culture that is used for parsing.</param>
    /// <exception cref="ArgumentNullException">propertyString</exception>
    public PropertyStringParser(string propertyString, CultureInfo culture)
    {
      if (propertyString is null)
        throw new ArgumentNullException(nameof(propertyString));

      _culture = culture;
      _properties = new Dictionary<string, string>();

      var parts1 = propertyString.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
      var parts2 = parts1.Select(s => s.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)).ToArray();

      // parts2 now is a jagged array, in which the last 2nd level entry is the key of the next 1st level
      for (int i = 1; i < parts2.Length; ++i)
      {
        var key = parts2[i - 1][^1];

        int offs = (i == parts2.Length - 1 ? 0 : 1); // offset is 1 with exception of the last entry (0)
        string value = string.Join(" ", parts2[i], 0, parts2[i].Length - offs);
        _properties.Add(key, value);
      }
    }

    /// <summary>
    /// Gets the properties as key-value pairs.
    /// </summary>
    /// <value>
    /// The properties.
    /// </value>
    public IReadOnlyDictionary<string, string> Properties => _properties;


    /// <summary>
    /// Gets a property as a string.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value that is returned if the property was not found.</param>
    /// <returns>The value of the property as a string; or <paramref name="defaultValue"/> if the property was not found.</returns>
    [return: NotNullIfNotNull("defaultValue")]
    public string? GetString(string key, string? defaultValue)
    {
      if (!_properties.TryGetValue(key, out var val))
        return defaultValue;
      else
        return val;
    }

    /// <summary>
    /// Gets a property as a <see cref="Double"/> value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value that is returned if the property was not found.</param>
    /// <returns>The value of the property as <see cref="Double"/> value; or <paramref name="defaultValue"/> if the property was not found.</returns>
    /// <exception cref="InvalidOperationException">Is thrown if the property could not be successfully parsed as <see cref="Double"/>.</exception>
    public double GetDouble(string key, double defaultValue)
    {
      if (!_properties.TryGetValue(key, out var val))
        return defaultValue;

      if (!double.TryParse(val, System.Globalization.NumberStyles.Float, _culture, out var value))
        throw new InvalidOperationException($"The property {key} is expected to be a number, but its value is: {val}");
      else
        return value;
    }

    /// <summary>
    /// Gets a property as a <see cref="Double"/> value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value of the property as <see cref="Double"/> value; or <c>null</c> if the property was not found.</returns>
    /// <exception cref="InvalidOperationException">Is thrown if the property could not be successfully parsed as <see cref="Double"/>.</exception>
    public double? GetDouble(string key)
    {
      if (!_properties.TryGetValue(key, out var val))
        return null;

      if (!double.TryParse(val, System.Globalization.NumberStyles.Float, _culture, out var value))
        throw new InvalidOperationException($"The property {key} is expected to be a number, but its value is: {val}");
      else
        return value;
    }

    /// <summary>
    /// Gets a property as an <see cref="Int32"/> value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value that is returned if the property was not found.</param>
    /// <returns>The value of the property as <see cref="Int32"/> value; or <paramref name="defaultValue"/> if the property was not found.</returns>
    /// <exception cref="InvalidOperationException">Is thrown if the property could not be successfully parsed as <see cref="Int32"/>.</exception>
    public Int32 GetInt32(string key, Int32 defaultValue)
    {
      if (!_properties.TryGetValue(key, out var val))
        return defaultValue;

      if (!int.TryParse(val, System.Globalization.NumberStyles.Integer, _culture, out var value))
        throw new InvalidOperationException($"The property {key} is expected to be an integer number, but its value is: {val}");
      else
        return value;
    }

    /// <summary>
    /// Gets a property as a <see cref="Int32"/> value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value of the property as <see cref="Int32"/> value; or <c>null</c> if the property was not found.</returns>
    /// <exception cref="InvalidOperationException">Is thrown if the property could not be successfully parsed as <see cref="Int32"/>.</exception>
    public Int32? GetInt32(string key)
    {
      if (!_properties.TryGetValue(key, out var val))
        return null;

      if (!int.TryParse(val, System.Globalization.NumberStyles.Integer, _culture, out var value))
        throw new InvalidOperationException($"The property {key} is expected to be an integer number, but its value is: {val}");
      else
        return value;
    }


    /// <summary>
    /// Gets a property as an <see cref="Int64"/> value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value that is returned if the property was not found.</param>
    /// <returns>The value of the property as <see cref="Int64"/> value; or <paramref name="defaultValue"/> if the property was not found.</returns>
    /// <exception cref="InvalidOperationException">Is thrown if the property could not be successfully parsed as <see cref="Int64"/>.</exception>
    public Int64 GetInt64(string key, Int64 defaultValue)
    {
      if (!_properties.TryGetValue(key, out var val))
        return defaultValue;

      if (!Int64.TryParse(val, System.Globalization.NumberStyles.Integer, _culture, out var value))
        throw new InvalidOperationException($"The property {key} is expected to be an integer number, but its value is: {val}");
      else
        return value;
    }

    /// <summary>
    /// Gets a property as a <see cref="Int64"/> value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value of the property as <see cref="Int64"/> value; or <c>null</c> if the property was not found.</returns>
    /// <exception cref="InvalidOperationException">Is thrown if the property could not be successfully parsed as <see cref="Int64"/>.</exception>
    public Int64? GetInt64(string key)
    {
      if (!_properties.TryGetValue(key, out var val))
        return null;

      if (!Int64.TryParse(val, System.Globalization.NumberStyles.Integer, _culture, out var value))
        throw new InvalidOperationException($"The property {key} is expected to be an integer number, but its value is: {val}");
      else
        return value;
    }

    /// <summary>
    /// Gets a property as an <see cref="DateTimeOffset"/> value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value that is returned if the property was not found.</param>
    /// <returns>The value of the property as <see cref="DateTimeOffset"/> value; or <paramref name="defaultValue"/> if the property was not found.</returns>
    /// <exception cref="InvalidOperationException">Is thrown if the property could not be successfully parsed as <see cref="DateTimeOffset"/>.</exception>
    public DateTimeOffset GetDateTimeOffset(string key, DateTimeOffset defaultValue)
    {
      if (!_properties.TryGetValue(key, out var val))
        return defaultValue;

      if (!DateTimeOffset.TryParse(val, _culture, DateTimeStyles.None, out var value))
        throw new InvalidOperationException($"The property {key} is expected to be a DateTimeOffset, but its value is: {val}");
      else
        return value;
    }

    /// <summary>
    /// Gets a property as a <see cref="DateTimeOffset"/> value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value of the property as <see cref="DateTimeOffset" /> value; or <c>null</c> if the property was not found.</returns>
    /// <exception cref="InvalidOperationException">Is thrown if the property could not be successfully parsed as <see cref="DateTimeOffset"/>.</exception>
    public DateTimeOffset? GetDateTimeOffset(string key)
    {
      if (!_properties.TryGetValue(key, out var val))
        return null;

      if (!DateTimeOffset.TryParse(val, _culture, DateTimeStyles.None, out var value))
        throw new InvalidOperationException($"The property {key} is expected to be a DateTimeOffset, but its value is: {val}");
      else
        return value;
    }

    /// <summary>
    /// Gets a property as an <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value that is returned if the property was not found.</param>
    /// <returns>The value of the property as <see cref="DateTime"/> value; or <paramref name="defaultValue"/> if the property was not found.</returns>
    /// <exception cref="InvalidOperationException">Is thrown if the property could not be successfully parsed as <see cref="DateTime"/>.</exception>
    public DateTime GetDateTime(string key, DateTime defaultValue)
    {
      if (!_properties.TryGetValue(key, out var val))
        return defaultValue;

      if (!DateTime.TryParse(val, _culture, DateTimeStyles.AssumeLocal, out var value))
        throw new InvalidOperationException($"The property {key} is expected to be a DateTime, but its value is: {val}");
      else
        return value;
    }

    /// <summary>
    /// Gets a property as a <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value of the property as <see cref="DateTime"/> value; or <c>null</c> if the property was not found.</returns>
    /// <exception cref="InvalidOperationException">Is thrown if the property could not be successfully parsed as <see cref="DateTime"/>.</exception>
    public DateTime? GetDateTime(string key)
    {
      if (!_properties.TryGetValue(key, out var val))
        return null;

      if (!DateTime.TryParse(val, _culture, DateTimeStyles.AssumeLocal, out var value))
        throw new InvalidOperationException($"The property {key} is expected to be a DateTime, but its value is: {val}");
      else
        return value;
    }

    /// <summary>
    /// Gets a property as a <see cref="bool"/> value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value of the property as <see cref="bool"/> value; or <c>null</c> if the property was not found.</returns>
    /// <exception cref="InvalidOperationException">Is thrown if the property could not be successfully parsed as <see cref="bool"/>.</exception>
    public bool? GetBoolean(string key)
    {
      if (!_properties.TryGetValue(key, out var val))
        return null;

      switch (val.ToLower())
      {
        case "yes":
        case "true":
        case "1":
          return true;

        case "no":
        case "false":
        case "0":
          return false;

        default:
          throw new InvalidOperationException($"The property {key} is expected to be a boolean value (1, 0, yes, no, true, false), but its value is: {val}");
      }
    }
  }
}
