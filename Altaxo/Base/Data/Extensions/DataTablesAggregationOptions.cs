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

#nullable enable
using System.Collections.Immutable;

namespace Altaxo.Data
{
  public record DataTablesAggregationOptions : Main.IImmutable
  {
    /// <summary>
    /// Gets the names of the properties that should be clustered.
    /// </summary>
    public ImmutableList<string> ClusteredPropertiesNames { get; init; } = ImmutableList<string>.Empty;

    /// <summary>
    /// Gets the name of the columns that should be aggregated.
    /// </summary>
    public ImmutableList<string> AggregatedColumnNames { get; init; } = ImmutableList<string>.Empty;

    public ImmutableList<KindOfAggregation> AggregationKinds { get; init; } = [KindOfAggregation.Mean];


    #region Serialization

    #region Version 0

    /// <summary>
    /// 2025-12-16 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTablesAggregationOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataTablesAggregationOptions)obj;

        info.AddArray("ClusteredPropertiesNames", s.ClusteredPropertiesNames, s.ClusteredPropertiesNames.Count);
        info.AddArray("AggregatedColumnNames", s.AggregatedColumnNames, s.AggregatedColumnNames.Count);
        info.CreateArray("AggregationKinds", s.AggregationKinds.Count);
        foreach (var v in s.AggregationKinds)
          info.AddEnum("e", v);
        info.CommitArray();
      }



      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var clusteredPropertiesNames = info.GetArrayOfStrings("ClusteredPropertiesNames");
        var aggregatedColumnNames = info.GetArrayOfStrings("AggregatedColumnNames");

        int count = info.OpenArray("AggregationKinds");
        var kinds = new KindOfAggregation[count];
        for (int i = 0; i < count; ++i)
          kinds[i] = info.GetEnum<KindOfAggregation>("e");
        info.CloseArray(count);

        return new DataTablesAggregationOptions()
        {
          ClusteredPropertiesNames = clusteredPropertiesNames.ToImmutableList(),
          AggregatedColumnNames = aggregatedColumnNames.ToImmutableList(),
          AggregationKinds = kinds.ToImmutableList(),
        };
      }
    }

    #endregion Version 0

    #endregion Serialization
  }

  public enum KindOfAggregation
  {
    Mean = 1,
    StdDev = 2,
    SStdDev = 3,
    Median = 4,
    Minimum = 5,
    Maximum = 6,
    Count = 7,
    Sum = 8,
  }
}
