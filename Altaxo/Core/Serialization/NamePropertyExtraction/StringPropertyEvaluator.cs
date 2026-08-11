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
  /// Represents a property evaluator that simply returns the input text as the property value, along with the specified property name.
  /// </summary>
  public record StringPropertyEvaluator : PropertyEvaluatorBase, IPropertyEvaluator
  {
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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(StringPropertyEvaluator), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (StringPropertyEvaluator)o;
        info.AddValue("PropertyName", s.PropertyName);
        info.AddValue("NumberOfIgnoredCharactersBefore", s.NumberOfIgnoredCharactersBefore);
        info.AddValue("NumberOfIgnoredCharactersAfter", s.NumberOfIgnoredCharactersAfter);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var propertyName = info.GetString("PropertyName");
        var numberOfIgnoredCharactersBefore = info.GetInt32("NumberOfIgnoredCharactersBefore");
        var numberOfIgnoredCharactersAfter = info.GetInt32("NumberOfIgnoredCharactersAfter");

        return o is null ? new StringPropertyEvaluator
        {
          PropertyName = propertyName,
          NumberOfIgnoredCharactersBefore = numberOfIgnoredCharactersBefore,
          NumberOfIgnoredCharactersAfter = numberOfIgnoredCharactersAfter,
        } : ((StringPropertyEvaluator)o) with
        {
          PropertyName = propertyName,
          NumberOfIgnoredCharactersBefore = numberOfIgnoredCharactersBefore,
          NumberOfIgnoredCharactersAfter = numberOfIgnoredCharactersAfter,
        };
      }
    }
    #endregion

    /// <inheritdoc/>
    public (string PropertyName, object PropertyValue) Evaluate(string text)
    {
      var subtext = NumberOfIgnoredCharactersBefore + NumberOfIgnoredCharactersAfter >= text.Length ?
        string.Empty :
        text.Substring(NumberOfIgnoredCharactersBefore, text.Length - NumberOfIgnoredCharactersBefore - NumberOfIgnoredCharactersAfter);

      return (PropertyName, subtext);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"String";
    }
  }
}
