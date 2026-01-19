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
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System.Collections.Immutable;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Result of a dimension reduction by aggregation.
  /// </summary>
  public record DimensionReductionByAggregationResult : IDimensionReductionResult
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DimensionReductionByAggregationResult"/> record.
    /// </summary>
    public DimensionReductionByAggregationResult(IROMatrix<double> result, ImmutableList<KindOfAggregation> aggregationKinds)
    {
      Result = result;
      AggregationKinds = aggregationKinds;
    }

    /// <summary>
    /// Gets the result matrix. Rows correspond to the original data points, columns to the aggregated values.
    /// </summary>
    public IROMatrix<double> Result { get; init; }

    ImmutableList<KindOfAggregation> AggregationKinds { get; init; }


    /// <inheritdoc/>
    public void SaveResultToTable(DimensionReductionOutputOptions outputOptions, DataTable sourceTable, DataTable destinationTable, int[] columnNumbersOfSpectraInSourceTable, double[] xValuesOfPreprocessedSpectra, Matrix<double> preprocessedSpectra, IEnsembleProcessingAuxiliaryData auxiliaryData)
    {
      int numberOfSpectra = Result.RowCount;
      int numberOfFactors = Result.ColumnCount;
      DataColumn column;

      int groupNumber = 0;

      // save the factors
      // first, copy the property columns of the original table (and the name)
      column = destinationTable.DataColumns.EnsureExistence($"ColumnName", typeof(TextColumn), ColumnKind.X, groupNumber);
      for (int idxSpectrum = 0; idxSpectrum < numberOfSpectra; ++idxSpectrum)
        column[idxSpectrum] = sourceTable.DataColumns.GetColumnName(columnNumbersOfSpectraInSourceTable[idxSpectrum]);

      for (int idxPropColumn = 0; idxPropColumn < sourceTable.PropertyColumnCount; ++idxPropColumn)
      {
        column = destinationTable.DataColumns.EnsureExistence(sourceTable.PropertyColumns.GetColumnName(idxPropColumn), sourceTable.PropertyColumns[idxPropColumn].GetType(), ColumnKind.V, groupNumber);
        for (int i = 0; i < numberOfSpectra; ++i)
          column[i] = sourceTable.PropertyColumns[idxPropColumn][columnNumbersOfSpectraInSourceTable[i]];
      }

      // and now the factors
      for (int idxFactor = 0; idxFactor < numberOfFactors; ++idxFactor)
      {
        column = destinationTable.DataColumns.EnsureExistence($"Result_{AggregationKinds[idxFactor]}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
        for (int idxSpectrum = 0; idxSpectrum < numberOfSpectra; ++idxSpectrum)
          column[idxSpectrum] = Result[idxSpectrum, idxFactor];
      }

      ++groupNumber;

      // Save the preprocessed spectra, this should have the same group number as the loadings
      if (outputOptions.IncludePreprocessedSpectra)
      {
        int numberOfSpectralPoints = preprocessedSpectra.ColumnCount;
        for (int idxSpectrum = 0; idxSpectrum < numberOfSpectra; idxSpectrum++)
        {
          var spectrumColumn = destinationTable.DataColumns.EnsureExistence($"PreprocessedData_{idxSpectrum}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
          for (int idxSpectralPoint = 0; idxSpectralPoint < numberOfSpectralPoints; idxSpectralPoint++)
          {
            spectrumColumn[idxSpectralPoint] = preprocessedSpectra[idxSpectrum, idxSpectralPoint];
            var columnNumberOfSpectrumInDestinationTable = destinationTable.DataColumns.GetColumnNumber(spectrumColumn);

            for (int idxPropColumn = 0; idxPropColumn < sourceTable.PropertyColumnCount; ++idxPropColumn)
            {
              var pcolumn = destinationTable.PropertyColumns.EnsureExistence(sourceTable.PropertyColumns.GetColumnName(idxPropColumn), sourceTable.PropertyColumns[idxPropColumn].GetType(), ColumnKind.V, 0);
              pcolumn[columnNumberOfSpectrumInDestinationTable] = sourceTable.PropertyColumns[idxPropColumn][columnNumbersOfSpectraInSourceTable[idxSpectrum]];
            }
          }
        }
      }
    }

    /// <summary>
    /// Saves the auxiliary spectral data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="numberOfSpectralPoints">The number of spectral points.</param>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="groupNumber">The group number.</param>
    void SaveAuxiliarySpectralData(IEnsembleProcessingAuxiliaryData data, int numberOfSpectralPoints, DataTable destinationTable, int groupNumber)
    {
      if (data is EnsembleAuxiliaryDataCompound compound)
      {
        foreach (var item in compound.Values)
        {
          SaveAuxiliarySpectralData(item, numberOfSpectralPoints, destinationTable, groupNumber);
        }
      }
      else if (data is EnsembleAuxiliaryDataVector vector && vector.Value.Length == numberOfSpectralPoints)
      {
        // save the data)
        var column = destinationTable.DataColumns.EnsureExistence(data.Name, typeof(DoubleColumn), ColumnKind.V, groupNumber);
        column.Data = vector.Value;
      }
    }
  }
}
