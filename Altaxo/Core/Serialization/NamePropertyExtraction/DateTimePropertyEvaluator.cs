﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

namespace Altaxo.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Represents a property evaluator that attempts to parse the input text as a date and time and returns the corresponding property name and value.
  /// </summary>
  public record DateTimePropertyEvaluator : PropertyEvaluatorBase, IPropertyEvaluator
  {
    /// <summary>
    /// Gets or sets the LCID (Locale Identifier) used for parsing the date and time value. The default value is the invariant culture's LCID.
    /// </summary>
    public int LCID { get; init; } = System.Globalization.CultureInfo.InvariantCulture.LCID;

    /// <summary>
    /// Gets or sets the date and time styles used for parsing. The default value is System.Globalization.DateTimeStyles.None.
    /// </summary>
    public System.Globalization.DateTimeStyles DateTimeStyles { get; init; } = System.Globalization.DateTimeStyles.None;

    /// <summary>
    /// Gets or sets a value indicating whether the parsed value should be returned as a DateTimeOffset instead of a DateTime.
    /// </summary>
    public bool UseDateTimeOffset { get; init; }

    /// <summary>
    /// Gets or sets a custom set of date and time formats. When non-empty, the parser tries these formats before attempting a general parse.
    /// </summary>
    public string[] CustomFormats { get; init; } = System.Array.Empty<string>();

    /// <summary>
    /// Gets or sets the number of characters to ignore before the value when parsing. This can be useful for skipping prefixes or other non-date characters in the input text.
    /// </summary>
    public int NumberOfIgnoredCharactersBefore { get; init; }

    /// <summary>
    /// Gets or sets the number of characters to ignore after the value when parsing. This can be useful for skipping suffixes or other non-date characters in the input text.
    /// </summary>
    public int NumberOfIgnoredCharactersAfter { get; init; }

    #region Serialization

    /// <summary>
    /// V0: 2026-09-10 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DateTimePropertyEvaluator), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DateTimePropertyEvaluator)o;
        info.AddValue("PropertyName", s.PropertyName);
        info.AddValue("NumberOfIgnoredCharactersBefore", s.NumberOfIgnoredCharactersBefore);
        info.AddValue("NumberOfIgnoredCharactersAfter", s.NumberOfIgnoredCharactersAfter);
        info.AddValue("LCID", s.LCID);
        info.AddEnum("DateTimeStyles", s.DateTimeStyles);
        info.AddValue("UseDateTimeOffset", s.UseDateTimeOffset);
        info.AddArray("CustomFormats", s.CustomFormats, s.CustomFormats.Length);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var propertyName = info.GetString("PropertyName");
        var numberOfIgnoredCharactersBefore = info.GetInt32("NumberOfIgnoredCharactersBefore");
        var numberOfIgnoredCharactersAfter = info.GetInt32("NumberOfIgnoredCharactersAfter");
        var lCID = info.GetInt32("LCID");
        var dateTimeStyles = info.GetEnum<System.Globalization.DateTimeStyles>("DateTimeStyles");
        var useDateTimeOffset = info.GetBoolean("UseDateTimeOffset");
        var customFormats = info.GetArrayOfStrings("CustomFormats");

        return o is null ? new DateTimePropertyEvaluator
        {
          PropertyName = propertyName,
          NumberOfIgnoredCharactersBefore = numberOfIgnoredCharactersBefore,
          NumberOfIgnoredCharactersAfter = numberOfIgnoredCharactersAfter,
          LCID = lCID,
          DateTimeStyles = dateTimeStyles,
          UseDateTimeOffset = useDateTimeOffset,
          CustomFormats = customFormats,
        } : ((DateTimePropertyEvaluator)o) with
        {
          PropertyName = propertyName,
          NumberOfIgnoredCharactersBefore = numberOfIgnoredCharactersBefore,
          NumberOfIgnoredCharactersAfter = numberOfIgnoredCharactersAfter,
          LCID = lCID,
          DateTimeStyles = dateTimeStyles,
          UseDateTimeOffset = useDateTimeOffset,
          CustomFormats = customFormats,
        };
      }
    }
    #endregion

    /// <inheritdoc/>
    public (string PropertyName, object PropertyValue) Evaluate(string text)
    {
      var culture = System.Globalization.CultureInfo.GetCultureInfo(LCID);

      var subtext = NumberOfIgnoredCharactersBefore + NumberOfIgnoredCharactersAfter >= text.Length ?
        string.Empty :
        text.Substring(NumberOfIgnoredCharactersBefore, text.Length - NumberOfIgnoredCharactersBefore - NumberOfIgnoredCharactersAfter);

      if (UseDateTimeOffset)
      {
        if (TryParseDateTimeOffset(subtext, culture, out var dateTimeOffsetValue))
        {
          return (PropertyName, dateTimeOffsetValue);
        }

        return (PropertyName, $"The text '{subtext}' could not be parsed as a DateTimeOffset.");
      }

      if (TryParseDateTime(subtext, culture, out var dateTimeValue))
      {
        return (PropertyName, dateTimeValue);
      }

      return (PropertyName, $"The text '{subtext}' could not be parsed as a DateTime.");
    }

    private bool TryParseDateTime(string text, System.Globalization.CultureInfo culture, out System.DateTime value)
    {
      if (CustomFormats.Length > 0 && System.DateTime.TryParseExact(text, CustomFormats, culture, DateTimeStyles, out value))
      {
        return true;
      }

      return System.DateTime.TryParse(text, culture, DateTimeStyles, out value);
    }

    private bool TryParseDateTimeOffset(string text, System.Globalization.CultureInfo culture, out System.DateTimeOffset value)
    {
      if (CustomFormats.Length > 0 && System.DateTimeOffset.TryParseExact(text, CustomFormats, culture, DateTimeStyles, out value))
      {
        return true;
      }

      return System.DateTimeOffset.TryParse(text, culture, DateTimeStyles, out value);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      var typeName = UseDateTimeOffset ? "DateTimeOffset" : "DateTime";
      var customFormats = CustomFormats.Length > 0 ? $"; Formats: {string.Join(", ", CustomFormats)}" : string.Empty;
      return $"{typeName}; DateTimeStyles: {DateTimeStyles}; LCID: {LCID}{customFormats}";
    }
  }
}
