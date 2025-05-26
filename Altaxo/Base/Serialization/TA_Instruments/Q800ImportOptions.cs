#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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


namespace Altaxo.Serialization.TA_Instruments
{
  /// <summary>
  /// Import options for importing TA Instruments Q800 series files.
  /// </summary>
  public record Q800ImportOptions : Main.IImmutable
  {
    /// <summary>
    /// If true, the data are tried to convert to SI units.
    /// </summary>
    public bool ConvertUnitsToSIUnits { get; init; } = true;

    /// <summary>
    /// If true, the file name of the imported file is included as a column property.
    /// </summary>
    public bool IncludeFilePathAsProperty { get; init; } = true;

    #region Serialization

    /// <summary>
    /// V0: 2025-05-26 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Q800ImportOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Q800ImportOptions)obj;
        info.AddValue("ConvertUnitsToSIUnits", s.ConvertUnitsToSIUnits);
        info.AddValue("IncludeFilePathAsProperty", s.IncludeFilePathAsProperty);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var convertUnitsToSIUnits = info.GetBoolean("ConvertUnitsToSIUnits");
        var includeFilePathAsProperty = info.GetBoolean("IncludeFilePathAsProperty");

        return o is null ? new Q800ImportOptions
        {
          ConvertUnitsToSIUnits = convertUnitsToSIUnits,
          IncludeFilePathAsProperty = includeFilePathAsProperty,
        } :
          ((Q800ImportOptions)o) with
          {
            ConvertUnitsToSIUnits = convertUnitsToSIUnits,
            IncludeFilePathAsProperty = includeFilePathAsProperty,
          };
      }
    }
    #endregion

  }
}
