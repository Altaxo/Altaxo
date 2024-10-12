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


using System.Collections.Generic;

namespace Altaxo.Serialization.PrincetonInstruments
{
  /// <summary>
  /// Import options for importing Princeton Instruments .SPE files.
  /// </summary>
  public record PrincetonInstrumentsSPEImportOptions : Main.IImmutable
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

    /// <summary>
    /// Gets a value indicating whether the frame metadata are stored in property columns.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [include frame meta data as properties]; otherwise, <c>false</c>.
    /// </value>
    public bool IncludeFrameMetaDataAsProperties { get; init; } = true;

    /// <summary>
    /// Gets the indices of imported frames.
    /// If the collection is empty, all frames are imported.
    /// </summary>
    public IReadOnlyList<int> IndicesOfImportedFrames { get; init; } = [];


    /// <summary>
    /// Gets the indices of imported regions of interest.
    /// If the collection is empty, all regions of interest are imported.
    /// </summary>
    public IReadOnlyList<int> IndicesOfImportedRegions { get; init; } = [];

    #region Serialization

    /// <summary>
    /// V0: 2024-09-14 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PrincetonInstrumentsSPEImportOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PrincetonInstrumentsSPEImportOptions)obj;
        info.AddValue("UseNeutralColumnName", s.UseNeutralColumnName);
        info.AddValue("NeutralColumnName", s.NeutralColumnName);
        info.AddValue("IncludeFilePathAsProperty", s.IncludeFilePathAsProperty);
        info.AddValue("IncludeFrameMetaDataAsProperties", s.IncludeFrameMetaDataAsProperties);
        info.AddArray("IndicesOfImportedFrames", s.IndicesOfImportedFrames, s.IndicesOfImportedFrames.Count);
        info.AddArray("IndicesOfImportedRegions", s.IndicesOfImportedRegions, s.IndicesOfImportedRegions.Count);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var useNeutralColumnName = info.GetBoolean("UseNeutralColumnName");
        var neutralColumnName = info.GetString("NeutralColumnName");
        var includeFilePathAsProperty = info.GetBoolean("IncludeFilePathAsProperty");
        var includeFrameMetaDataAsProperties = info.GetBoolean("IncludeFrameMetaDataAsProperties");
        info.GetArray("IndicesOfImportedFrames", out int[] indicesOfImportedFrames);
        info.GetArray("IndicesOfImportedRegions", out int[] indicesOfImportedRegions);



        return o is null ? new PrincetonInstrumentsSPEImportOptions
        {
          UseNeutralColumnName = useNeutralColumnName,
          NeutralColumnName = neutralColumnName,
          IncludeFilePathAsProperty = includeFilePathAsProperty,
          IncludeFrameMetaDataAsProperties = includeFrameMetaDataAsProperties,
          IndicesOfImportedFrames = indicesOfImportedFrames,
          IndicesOfImportedRegions = indicesOfImportedRegions,
        } :
          ((PrincetonInstrumentsSPEImportOptions)o) with
          {
            UseNeutralColumnName = useNeutralColumnName,
            NeutralColumnName = neutralColumnName,
            IncludeFilePathAsProperty = includeFilePathAsProperty,
            IncludeFrameMetaDataAsProperties = includeFrameMetaDataAsProperties,
            IndicesOfImportedFrames = indicesOfImportedFrames,
            IndicesOfImportedRegions = indicesOfImportedRegions,
          };
      }
    }
    #endregion

  }
}
