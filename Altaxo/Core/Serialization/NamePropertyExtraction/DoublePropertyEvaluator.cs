#region Copyright

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
  /// Represents a property evaluator that attempts to parse the input text as a double and returns the corresponding property name and value.
  /// </summary>
  public record DoublePropertyEvaluator : PropertyEvaluatorBase, IPropertyEvaluator
  {
    /// <summary>
    /// Gets or sets the LCID (Locale Identifier) used for parsing the double value. The default value is the invariant culture's LCID.
    /// </summary>
    public int LCID { get; init; } = System.Globalization.CultureInfo.InvariantCulture.LCID;

    /// <summary>
    /// Gets or sets the number styles used for parsing the double value. The default value is System.Globalization.NumberStyles.Float.
    /// </summary>
    public System.Globalization.NumberStyles NumberStyles { get; init; } = System.Globalization.NumberStyles.Float;

    /// <summary>
    /// Gets or sets the number of characters to ignore before the value when parsing. This can be useful for skipping prefixes or other non-numeric characters in the input text.
    /// </summary>
    public int NumberOfIgnoredCharactersBefore { get; init; }

    /// <summary>
    /// Gets or sets the number of characters to ignore after the value when parsing. This can be useful for skipping suffixes or other non-numeric characters in the input text.
    /// </summary>
    public int NumberOfIgnoredCharactersAfter { get; init; }

    #region Serialization

    /// <summary>
    /// V0: 2026-08-10 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DoublePropertyEvaluator), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DoublePropertyEvaluator)o;
        info.AddValue("PropertyName", s.PropertyName);
        info.AddValue("NumberOfIgnoredCharactersBefore", s.NumberOfIgnoredCharactersBefore);
        info.AddValue("NumberOfIgnoredCharactersAfter", s.NumberOfIgnoredCharactersAfter);
        info.AddValue("LCID", s.LCID);
        info.AddEnum("NumberStyles", s.NumberStyles);

      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var propertyName = info.GetString("PropertyName");
        var numberOfIgnoredCharactersBefore = info.GetInt32("NumberOfIgnoredCharactersBefore");
        var numberOfIgnoredCharactersAfter = info.GetInt32("NumberOfIgnoredCharactersAfter");
        var lCID = info.GetInt32("LCID");
        var numberStyles = info.GetEnum<System.Globalization.NumberStyles>("NumberStyles");

        return o is null ? new DoublePropertyEvaluator
        {
          PropertyName = propertyName,
          NumberOfIgnoredCharactersBefore = numberOfIgnoredCharactersBefore,
          NumberOfIgnoredCharactersAfter = numberOfIgnoredCharactersAfter,
          LCID = lCID,
          NumberStyles = numberStyles,
        } : ((DoublePropertyEvaluator)o) with
        {
          PropertyName = propertyName,
          NumberOfIgnoredCharactersBefore = numberOfIgnoredCharactersBefore,
          NumberOfIgnoredCharactersAfter = numberOfIgnoredCharactersAfter,
          LCID = lCID,
          NumberStyles = numberStyles,
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

      if (double.TryParse(subtext, NumberStyles, culture, out double value))
      {
        return (PropertyName, value);
      }
      else
      {
        return (PropertyName, $"The text '{subtext}' could not be parsed as an double.");
      }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"Double; NumberStyles: {NumberStyles}; LCID: {LCID}";
    }
  }
}
