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

using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Result of a dimension reduction by factorization.
  /// </summary>
  public record DimensionReductionByFactorizationResult : IDimensionReductionResult
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DimensionReductionByFactorizationResult"/> record.
    /// </summary>
    /// <param name="factors">The factor scores.</param>
    /// <param name="loads">The loadings.</param>
    /// <param name="residualVariances">Residual variances per factor, or <c>null</c> if not available.</param>
    /// <param name="meanOfProcessData">Mean values used for preprocessing/centering of the process data, or <c>null</c> if not available.</param>
    public DimensionReductionByFactorizationResult(IROMatrix<double> factors, IROMatrix<double> loads, IROMatrix<double> residualVariances, IReadOnlyCollection<double> meanOfProcessData)
    {
      Factors = factors;
      Loadings = loads;
      ResidualVariances = residualVariances;
      MeanOfProcessData = meanOfProcessData;
    }

    /// <summary>
    /// Gets the factor score matrix.
    /// </summary>
    public IROMatrix<double> Factors { get; init; }

    /// <summary>
    /// Gets the loadings matrix.
    /// </summary>
    public IROMatrix<double> Loadings { get; init; }

    /// <summary>
    /// Gets the residual variances.
    /// </summary>
    public IROMatrix<double> ResidualVariances { get; init; }

    /// <summary>
    /// Gets the mean values of the process data.
    /// </summary>
    public IReadOnlyCollection<double> MeanOfProcessData { get; init; }



    /// <inheritdoc/>
    public void SaveResultToTable(DimensionReductionOutputOptions outputOptions, DataTable sourceTable, DataTable destinationTable, int[] columnNumbersOfSpectraInSourceTable, double[] xValuesOfPreprocessedSpectra, Matrix<double> preprocessedSpectra, IEnsembleProcessingAuxiliaryData auxiliaryData)
    {
      int numberOfSpectralPoints = Loadings.ColumnCount;
      int numberOfSpectra = Factors.RowCount;
      int numberOfFactors = Factors.ColumnCount;
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
        column = destinationTable.DataColumns.EnsureExistence($"Factor_{idxFactor}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
        for (int idxSpectrum = 0; idxSpectrum < numberOfSpectra; ++idxSpectrum)
          column[idxSpectrum] = Factors[idxSpectrum, idxFactor];
      }

      ++groupNumber;

      // Save the loadings
      column = destinationTable.DataColumns.EnsureExistence($"Loading_X", typeof(DoubleColumn), ColumnKind.X, groupNumber);
      column.Data = xValuesOfPreprocessedSpectra;
      for (int idxFactor = 0; idxFactor < numberOfFactors; idxFactor++)
      {
        var scoreColumn = destinationTable.DataColumns.EnsureExistence($"Loading_{idxFactor}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
        for (int idxSpectralPoint = 0; idxSpectralPoint < numberOfSpectralPoints; idxSpectralPoint++)
        {
          scoreColumn[idxSpectralPoint] = Loadings[idxFactor, idxSpectralPoint];
        }
      }

      if (outputOptions.IncludeEnsemblePreprocessingAuxiliaryData && auxiliaryData is not null)
      {
        SaveAuxiliarySpectralData(auxiliaryData, numberOfSpectralPoints, destinationTable, groupNumber);
      }

      // Save the preprocessed spectra, this should have the same group number as the loadings
      if (outputOptions.IncludePreprocessedSpectra)
      {
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
