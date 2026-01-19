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

using System;
using System.Collections.Immutable;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Statistics;
using Altaxo.Data;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Dimension reduction analyses based on aggregation of data, for instance mean, min, or max.
  /// </summary>
  public record DimensionReductionByAggregation : IDimensionReductionMethod
  {

    /// <summary>
    /// Gets the kinds of aggregation that should be applied to the aggregated columns.
    /// </summary>
    public ImmutableList<KindOfAggregation> AggregationKinds { get; init; } = [KindOfAggregation.Mean];


    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-09.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionByAggregation), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionByAggregation)obj;
        info.CreateArray("AggregationKinds", s.AggregationKinds.Count);
        foreach (var v in s.AggregationKinds)
          info.AddEnum("e", v);
        info.CommitArray();
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        int count = info.OpenArray("AggregationKinds");
        var kinds = new KindOfAggregation[count];
        for (int i = 0; i < count; ++i)
          kinds[i] = info.GetEnum<KindOfAggregation>("e");
        info.CloseArray(count);

        return ((o as DimensionReductionByAggregation) ?? new DimensionReductionByAggregation())
          with
        {
          AggregationKinds = kinds.ToImmutableList(),
        };

      }
    }
    #endregion

    /// <inheritdoc/>
    public virtual string DisplayName
    {
      get => "Data aggregation";

    }

    /// <inheritdoc/>
    public virtual IDimensionReductionResult ExecuteDimensionReduction(IROMatrix<double> x)
    {
      var result = CreateMatrix.Dense<double>(x.RowCount, AggregationKinds.Count);
      var aggregation = new double[x.ColumnCount];
      var c = new double[AggregationKinds.Count];

      for (int idxRow = 0; idxRow < x.RowCount; idxRow++)
      {
        MatrixMath.CopyRow(x, idxRow, aggregation);

        for (int idxCol = 0; idxCol < AggregationKinds.Count; idxCol++)
        {
          switch (AggregationKinds[idxCol])
          {
            case KindOfAggregation.Mean:
              if (aggregation.Length == 1)
                c[idxCol] = aggregation[0]; // if there is only one item, we use the item directly. Thus even DataTime or Text could be possible for the kind of item
              else
                c[idxCol] = aggregation.Average();
              break;
            case KindOfAggregation.StdDev:
              c[idxCol] = aggregation.StandardDeviation();
              break;

            case KindOfAggregation.PopulationStdDev:
              c[idxCol] = aggregation.PopulationStandardDeviation();
              break;

            case KindOfAggregation.Median:
              c[idxCol] = aggregation.Median();
              break;

            case KindOfAggregation.Minimum:
              if (aggregation.Length == 1)
                c[idxCol] = aggregation[0]; // if there is only one item, we use the item directly. Thus even DataTime or Text could be possible for the kind of item
              else
                c[idxCol] = aggregation.Min();
              break;

            case KindOfAggregation.Maximum:
              if (aggregation.Length == 1)
                c[idxCol] = aggregation[0]; // if there is only one item, we use the item directly. Thus even DataTime or Text could be possible for the kind of item
              else
                c[idxCol] = aggregation.Max();
              break;

            case KindOfAggregation.Count:
              c[idxCol] = aggregation.Length;
              break;

            case KindOfAggregation.Sum:
              if (aggregation.Length == 1)
                c[idxCol] = aggregation[0]; // if there is only one item, we use the item directly. Thus even DataTime or Text could be possible for the kind of item
              else
                c[idxCol] = aggregation.Sum();
              break;

            case KindOfAggregation.Variance:
              c[idxCol] = aggregation.Variance();
              break;

            case KindOfAggregation.PopulationVariance:
              c[idxCol] = aggregation.PopulationVariance();
              break;
            case KindOfAggregation.MinimumAbsolute:
              c[idxCol] = aggregation.MinimumAbsolute();
              break;

            case KindOfAggregation.MaximumAbsolute:
              c[idxCol] = aggregation.MaximumAbsolute();
              break;

            case KindOfAggregation.GeometricMean:
              c[idxCol] = aggregation.GeometricMean();
              break;

            case KindOfAggregation.HarmonicMean:
              c[idxCol] = aggregation.HarmonicMean();
              break;

            case KindOfAggregation.Skewness:
              c[idxCol] = aggregation.Skewness();
              break;

            case KindOfAggregation.PopulationSkewness:
              c[idxCol] = aggregation.PopulationSkewness();
              break;

            case KindOfAggregation.Kurtosis:
              c[idxCol] = aggregation.Kurtosis();
              break;

            case KindOfAggregation.PopulationKurtosis:
              c[idxCol] = aggregation.PopulationKurtosis();
              break;

            case KindOfAggregation.RootMeanSquare:
              c[idxCol] = aggregation.RootMeanSquare();
              break;

            case KindOfAggregation.LowerQuartile:
              c[idxCol] = aggregation.LowerQuartile();
              break;

            case KindOfAggregation.UpperQuartile:
              c[idxCol] = aggregation.UpperQuartile();
              break;
            default:
              throw new NotImplementedException($"The aggregation kind {AggregationKinds[idxCol]} is not implemented!");
          }
        }
        MatrixMath.SetRow(result, idxRow, c);
      }
      return new DimensionReductionByAggregationResult(result, AggregationKinds);
    }
  }
}
