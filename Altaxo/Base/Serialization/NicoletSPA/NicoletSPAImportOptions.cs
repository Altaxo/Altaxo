#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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


namespace Altaxo.Serialization.NicoletSPA
{
  /// <summary>
  /// Import options for importing Galactic SPC files.
  /// </summary>
  public record NicoletSPAImportOptions : Main.IImmutable
  {
    /// <summary>
    /// If true, the column name of the imported y-columns is set to a neutral, constant value.
    /// If false, the column name of the imported y-columns is derived from the file name of the imported file.
    /// </summary>
    public bool UseNeutralColumnName { get; init; } = true;

    /// <summary>
    /// Gets the neutral column name (base). The name is at request extended, e.g. by a number at the end.
    /// </summary>
    public string NeutralColumnName { get; init; } = "Y";

    /// <summary>
    /// If true, the file name of the imported file is included as a column property.
    /// </summary>
    public bool IncludeFilePathAsProperty { get; init; } = true;

    #region Serialization

    /// <summary>
    /// V0: 2024-09-14 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NicoletSPAImportOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NicoletSPAImportOptions)obj;
        info.AddValue("UseNeutralColumnName", s.UseNeutralColumnName);
        info.AddValue("NeutralColumnName", s.NeutralColumnName);
        info.AddValue("IncludeFilePathAsProperty", s.IncludeFilePathAsProperty);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var useNeutralColumnName = info.GetBoolean("UseNeutralColumnName");
        var neutralColumnName = info.GetString("NeutralColumnName");
        var includeFilePathAsProperty = info.GetBoolean("IncludeFilePathAsProperty");

        return o is null ? new NicoletSPAImportOptions
        {
          UseNeutralColumnName = useNeutralColumnName,
          NeutralColumnName = neutralColumnName,
          IncludeFilePathAsProperty = includeFilePathAsProperty,
        } :
          ((NicoletSPAImportOptions)o) with
          {
            UseNeutralColumnName = useNeutralColumnName,
            NeutralColumnName = neutralColumnName,
            IncludeFilePathAsProperty = includeFilePathAsProperty,
          };
      }
    }
    #endregion

  }
}
