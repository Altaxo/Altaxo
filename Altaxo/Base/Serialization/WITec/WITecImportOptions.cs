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

namespace Altaxo.Serialization.WITec
{
  /// <summary>
  /// Import options for importing WITec project files.
  /// </summary>
  public record WITecImportOptions : Main.IImmutable
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
    /// Gets the indices of imported graphs.
    /// If the collection is empty, all graphs are imported.
    /// </summary>
    public IReadOnlyList<int> IndicesOfImportedGraphs { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether to ignore secondary data, i.e. data that belong to other
    /// graph data. Example: in a spectra time series, there is separate graph data for the times of the spectra.
    /// This data is not imported if this flag is set to true, because it belongs to the spectra series.
    /// </summary>
    public bool IgnoreSecondaryData { get; init; } = true;

    #region Serialization

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WITecImportOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (WITecImportOptions)obj;
        info.AddValue("UseNeutralColumnName", s.UseNeutralColumnName);
        info.AddValue("NeutralColumnName", s.NeutralColumnName);
        info.AddValue("IncludeFilePathAsProperty", s.IncludeFilePathAsProperty);
        info.AddValue("IgnoreSecondaryData", s.IgnoreSecondaryData);
        info.AddArray("IndicesOfImportedGraphs", s.IndicesOfImportedGraphs, s.IndicesOfImportedGraphs.Count);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var useNeutralColumnName = info.GetBoolean("UseNeutralColumnName");
        var neutralColumnName = info.GetString("NeutralColumnName");
        var includeFilePathAsProperty = info.GetBoolean("IncludeFilePathAsProperty");
        var ignoreSecondaryData = info.GetBoolean("IgnoreSecondaryData");
        info.GetArray("IndicesOfImportedGraphs", out int[] indicesOfImportedGraphs);

        return o is null ? new WITecImportOptions
        {
          UseNeutralColumnName = useNeutralColumnName,
          NeutralColumnName = neutralColumnName,
          IncludeFilePathAsProperty = includeFilePathAsProperty,
          IgnoreSecondaryData = ignoreSecondaryData,
          IndicesOfImportedGraphs = indicesOfImportedGraphs,
        } :
          ((WITecImportOptions)o) with
          {
            UseNeutralColumnName = useNeutralColumnName,
            NeutralColumnName = neutralColumnName,
            IncludeFilePathAsProperty = includeFilePathAsProperty,
            IgnoreSecondaryData = ignoreSecondaryData,
            IndicesOfImportedGraphs = indicesOfImportedGraphs,
          };
      }
    }
    #endregion
  }
}
