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
using System.Collections.Immutable;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Process options for the multivariate analyses that feature a dimension reduction and regression.
  /// </summary>
  public record DimensionReductionAndRegressionOptions : Main.IImmutable
  {
    /// <summary>
    /// Gets the preprocessing applied to each individual spectrum prior to analysis.
    /// </summary>
    public ISingleSpectrumPreprocessor Preprocessing { get; init; } = new NoopSpectrumPreprocessor();

    /// <summary>
    /// Gets the preprocessing applied to the ensemble of spectra (for example mean-centering and scaling).
    /// </summary>
    public IEnsembleMeanScalePreprocessor MeanScaleProcessing { get; init; } = new Altaxo.Science.Spectroscopy.EnsembleProcessing.EnsembleMeanAndScaleCorrection();

    /// <summary>
    /// Gets the analysis method.
    /// </summary>
    public WorksheetAnalysis WorksheetAnalysis { get; init; } = new PLS2WorksheetAnalysis();

    /// <summary>
    /// Gets the maximum number of factors to calculate.
    /// </summary>
    /// <value>
    /// The maximum number of factors.
    /// </value>
    public int MaximumNumberOfFactors { get; init; } = 20;

    /// <summary>
    /// Gets the type of the cross validation used to calculate the cross-press values.
    /// </summary>
    public ICrossValidationGroupingStrategy CrossValidationGroupingStrategy { get; init; } = new CrossValidationGroupingStrategyExcludeGroupsOfSimilarMeasurements();


    /// <summary>
    /// Gets the additional columns to calculate. Key is the general name of the quantity/column.
    /// The value is a set of tuples that designate for which factors the columns must be calculated.
    /// </summary>
    public ImmutableDictionary<string, ImmutableHashSet<(int, int?)>> ColumnsToCalculate { get; init; } = ImmutableDictionary.Create<string, ImmutableHashSet<(int, int?)>>();


    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionAndRegressionOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionAndRegressionOptions)obj;
        info.AddValue("SinglePreprocessing", s.Preprocessing);
        info.AddValue("EnsemblePreprocessing", s.MeanScaleProcessing);
        info.AddValue("Analysis", s.WorksheetAnalysis);
        info.AddValue("MaximumNumberOfFactors", s.MaximumNumberOfFactors);
        info.AddValue("CrossValidationGroupingStrategy", s.CrossValidationGroupingStrategy);

        info.CreateArray("ColumnsToCalculate", s.ColumnsToCalculate.Count);
        foreach (var dictEntry in s.ColumnsToCalculate)
        {
          info.CreateElement("e");
          info.AddValue("ColumnName", dictEntry.Key);
          info.CreateArray("Indices", dictEntry.Value.Count);
          foreach (var pair in dictEntry.Value)
          {
            info.CreateElement("e");
            info.AddValue("NF", pair.Item1);
            info.AddValue("WY", pair.Item2);

            info.CommitElement();
          }
          info.CommitArray(); // Indices
          info.CommitElement(); // e
        }

        info.CommitArray(); // ColumnsToCalculate
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var singlePreprocessing = info.GetValue<ISingleSpectrumPreprocessor>("SinglePreprocessing", null);
        var ensemblePreprocessing = info.GetValue<IEnsembleMeanScalePreprocessor>("EnsemblePreprocessing", null);
        var analysis = info.GetValue<WorksheetAnalysis>("Analysis", null);
        var maxNumberOfFactors = info.GetInt32("MaximumNumberOfFactors");
        var crossValidationStrategy = info.GetValue<ICrossValidationGroupingStrategy>("CrossValidationGroupingStrategy", null);

        var dict = new Dictionary<string, ImmutableHashSet<(int, int?)>>();
        var countColumnsToCalculate = info.OpenArray("ColumnsToCalculate");
        {
          for (int i = 0; i < countColumnsToCalculate; i++)
          {
            info.OpenElement();
            var name = info.GetString("ColumnName");
            var countIndices = info.OpenArray("Indices");
            {
              var hashSet = new HashSet<(int, int?)>();
              for (int j = 0; j < countIndices; j++)
              {
                info.OpenElement();
                {
                  var first = info.GetInt32("NF");
                  var second = info.GetNullableInt32("WY");
                  hashSet.Add((first, second));
                }
                info.CloseElement();

              }
              dict.Add(name, hashSet.ToImmutableHashSet());
            }
            info.CloseArray(countIndices);
            info.CloseElement();
          }
        }
        info.CloseArray(countColumnsToCalculate);


        return new DimensionReductionAndRegressionOptions()
        {
          Preprocessing = singlePreprocessing,
          MeanScaleProcessing = ensemblePreprocessing,
          WorksheetAnalysis = analysis,
          MaximumNumberOfFactors = maxNumberOfFactors,
          CrossValidationGroupingStrategy = crossValidationStrategy,
          ColumnsToCalculate = dict.ToImmutableDictionary(),
        };
      }
    }
    #endregion


    /// <summary>
    /// Gets a new instance of this class in which the designated column to calculate is added to the dictionary <see cref="ColumnsToCalculate"/>.
    /// </summary>
    /// <param name="columnName">Name of the column that should be calculated.</param>
    /// <param name="first">The first integer parameter (in most cases the number of factors).</param>
    /// <param name="second">The second integer parameter (e.g. the number of the target variable).</param>
    /// <returns>A new instance of this class in which a calculated column is set.</returns>
    public DimensionReductionAndRegressionOptions WithColumnToCalculate(string columnName, int first, int? second = null)
    {
      var additionalColumns = ColumnsToCalculate;
      if (additionalColumns.TryGetValue(columnName, out var columnInfo))
      {
        additionalColumns = additionalColumns.SetItem(columnName, columnInfo.Add((first, second)));
      }
      else
      {
        var hashSet = ImmutableHashSet.Create<(int, int?)>().Add((first, second));
        additionalColumns = additionalColumns.Add(columnName, hashSet);
      }

      return this with { ColumnsToCalculate = additionalColumns };
    }
  }
}
