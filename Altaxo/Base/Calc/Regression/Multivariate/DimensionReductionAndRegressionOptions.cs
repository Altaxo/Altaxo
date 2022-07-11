#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Data;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.EnsembleMeanScale;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Process options for the multivariate analyses that feature a dimension reduction and regression.
  /// </summary>
  public record DimensionReductionAndRegressionOptions : Main.IImmutable
  {
    public ISingleSpectrumPreprocessor Preprocessing { get; init; } = new NoopSpectrumPreprocessor();

    public IEnsembleMeanScalePreprocessor MeanScaleProcessing { get; init; } = new Altaxo.Science.Spectroscopy.EnsembleMeanScale.EnsembleMeanAndScaleCorrection();

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
    /// Gets the type of the cross validation used to calculate the cross press values.
    /// </summary>
    public ICrossValidationGroupingStrategy CrossValidationGroupingStrategy { get; init; } = new CrossValidationGroupingStrategyExcludeGroupsOfSimilarMeasurements();


    /// <summary>
    /// Gets the additional columns to calculate. Key is the general name of the quantity / column.
    /// The value is a set of integer numbers that designate for which factors the columns must be calcuated.
    /// </summary>
    public ImmutableDictionary<string, ImmutableHashSet<(int, int?)>> ColumnsToCalculate { get; init; } = ImmutableDictionary.Create<string, ImmutableHashSet<(int, int?)>>();


    /// <summary>
    /// Gets a new instance of this class, in which the designated column to calculated is added to the dictionary <see cref="ColumnsToCalculate"/>.
    /// </summary>
    /// <param name="columnName">Name of the column that was calculated.</param>
    /// <param name="first">The first integer parameter (in most cases the number of factors.</param>
    /// <param name="second">The second integer parameter (e.g. the number of the target variable).</param>
    /// <returns>A new instance of this class, in which a calculated column is set.</returns>
    public DimensionReductionAndRegressionOptions WithColumnToCalculate(string columnName, int first, int?second=null)
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
