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
  /// <summary>
  /// Options for aggregating data from one or multiple tables, see also <see cref="DataTablesAggregationDataSource"/>.
  /// </summary>
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

    /// <summary>
    /// If true, the names in <see cref="AggregatedColumnNames"/> are treated as property names of the tables.
    /// </summary>
    public bool AggregatedColumnNamesArePropertyNames { get; init; } = false;

    /// <summary>
    /// Gets the kinds of aggregation that should be applied to the aggregated columns.
    /// </summary>
    public ImmutableList<KindOfAggregation> AggregationKinds { get; init; } = [KindOfAggregation.Mean];


    #region Serialization

    #region Version 0

    /// <summary>
    /// 2025-12-16 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTablesAggregationOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
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



      /// <inheritdoc/>
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

  /// <summary>
  /// Specifies the available kinds of aggregation for aggregated columns.
  /// </summary>
  public enum KindOfAggregation
  {
    /// <summary>
    /// Arithmetic mean of the values.
    /// </summary>
    Mean = 1,

    /// <summary>
    /// Standard deviation of the values, the divisor is N-1 (sample size minus 1, Bessel’s correction).
    /// </summary>
    StdDev = 2,

    /// <summary>
    /// Standard deviation estimated from a sample, the divisor is N (population size).
    /// </summary>
    PopulationStdDev = 3,

    /// <summary>
    /// Median of the values.
    /// </summary>
    Median = 4,

    /// <summary>
    /// Minimum of the values.
    /// </summary>
    Minimum = 5,

    /// <summary>
    /// Maximum of the values.
    /// </summary>
    Maximum = 6,

    /// <summary>
    /// Number of values.
    /// </summary>
    Count = 7,

    /// <summary>
    /// Sum of the values.
    /// </summary>
    Sum = 8,

    /// <summary>
    /// Sample variance of the values, the divisor is N-1 (sample size minus 1, Bessel’s correction).
    /// </summary>
    Variance = 9,

    /// <summary>
    /// Population variance estimated from a sample, the divisor is N (population size).
    /// </summary>
    PopulationVariance = 10,

    MinimumAbsolute = 11,

    MaximumAbsolute = 12,

    GeometricMean = 13,

    HarmonicMean = 14,

    Skewness = 15,

    PopulationSkewness = 16,

    Kurtosis = 17,

    PopulationKurtosis = 18,

    RootMeanSquare = 19,

    LowerQuartile = 20,

    UpperQuartile = 21,
  }
}
