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


using System.Collections.Generic;

namespace Altaxo.Serialization.Omnic
{
  /// <summary>
  /// Import options for importing Omnic SPG (spectrum group) files.
  /// </summary>
  public record OmnicSPGImportOptions : Main.IImmutable
  {
    /// <summary>
    /// Gets a value indicating whether the column name of imported y-columns is set to a neutral, constant value.
    /// </summary>
    /// <remarks>
    /// If <see langword="true"/>, the column name of imported y-columns is set to a neutral, constant value.
    /// If <see langword="false"/>, the column name of imported y-columns is derived from the file name of the imported file.
    /// </remarks>
    public bool UseNeutralColumnName { get; init; } = true;

    /// <summary>
    /// Gets the base neutral column name.
    /// </summary>
    /// <remarks>
    /// The name may be extended as needed, for example by appending a number.
    /// </remarks>
    public string NeutralColumnName { get; init; } = "Y";

    /// <summary>
    /// Gets a value indicating whether the file path of the imported file is included as a column property.
    /// </summary>
    public bool IncludeFilePathAsProperty { get; init; } = true;

    /// <summary>
    /// Gets the indices of imported spectra.
    /// </summary>
    /// <remarks>
    /// If the collection is empty, all spectra are imported.
    /// </remarks>
    public IReadOnlyList<int> IndicesOfImportedSpectra { get; init; } = [];

    #region Serialization

    /// <summary>
    /// V1: 2026-01-27 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OmnicSPGImportOptions), 1)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (OmnicSPGImportOptions)obj;
        info.AddValue("UseNeutralColumnName", s.UseNeutralColumnName);
        info.AddValue("NeutralColumnName", s.NeutralColumnName);
        info.AddValue("IncludeFilePathAsProperty", s.IncludeFilePathAsProperty);
        info.AddArray("IndicesOfImportedGraphs", s.IndicesOfImportedSpectra, s.IndicesOfImportedSpectra.Count);

      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var useNeutralColumnName = info.GetBoolean("UseNeutralColumnName");
        var neutralColumnName = info.GetString("NeutralColumnName");
        var includeFilePathAsProperty = info.GetBoolean("IncludeFilePathAsProperty");
        info.GetArray("IndicesOfImportedGraphs", out int[] indicesOfImportedGraphs);


        return o is null ? new OmnicSPGImportOptions
        {
          UseNeutralColumnName = useNeutralColumnName,
          NeutralColumnName = neutralColumnName,
          IncludeFilePathAsProperty = includeFilePathAsProperty,
          IndicesOfImportedSpectra = indicesOfImportedGraphs,
        } :
          ((OmnicSPGImportOptions)o) with
          {
            UseNeutralColumnName = useNeutralColumnName,
            NeutralColumnName = neutralColumnName,
            IncludeFilePathAsProperty = includeFilePathAsProperty,
            IndicesOfImportedSpectra = indicesOfImportedGraphs,
          };
      }
    }
    #endregion

  }
}
