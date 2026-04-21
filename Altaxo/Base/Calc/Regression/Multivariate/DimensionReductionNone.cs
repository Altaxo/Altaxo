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

using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Represents a dimension reduction method that performs no operation and returns nothing.
  /// </summary>
  /// <remarks>Although this method returns nothing, it can nevertheless be used in conjuction with preprocessing the output, in case the dimension reduction itself is not neccessary (in the moment).</remarks>
  public record DimensionReductionNone : IDimensionReductionMethod
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-04-21.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionNone), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionNone)o;
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return ((o as DimensionReductionNone) ?? new DimensionReductionNone());
      }
    }
    #endregion

    /// <inheritdoc />

    public string DisplayName => "None (no operation)";

    /// <inheritdoc />
    public IDimensionReductionResult ExecuteDimensionReduction(IROMatrix<double> processData)
    {
      return new DimensionReductionNoneResult();
    }
  }

  /// <summary>
  /// Result of a no-operational dimension reduction.
  /// </summary>
  public record DimensionReductionNoneResult : IDimensionReductionResult
  {
    /// <inheritdoc />
    public void SaveResultToTable(DimensionReductionOutputOptions outputOptions, DataTable sourceTable, DataTable destinationTable, int[] columnNumbersOfSpectraInSourceTable, double[] xValuesOfPreprocessedSpectra, Matrix<double> preprocessedSpectra, IEnsembleProcessingAuxiliaryData auxiliaryData)
    {
      int numberOfSpectra = preprocessedSpectra.RowCount;
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

      ++groupNumber;

      if (outputOptions.IncludeEnsemblePreprocessingAuxiliaryData && auxiliaryData is not null)
      {
        DimensionReductionByFactorizationResult.SaveAuxiliarySpectralData(auxiliaryData, destinationTable, groupNumber);
      }

      // Save the preprocessed spectra, this should have the same group number as the loadings
      if (outputOptions.IncludePreprocessedSpectra)
      {
        int numberOfSpectralPoints = preprocessedSpectra.ColumnCount;

        column = destinationTable.DataColumns.EnsureExistence($"PreprocessedData_X", typeof(DoubleColumn), ColumnKind.X, groupNumber);
        column.Data = xValuesOfPreprocessedSpectra;

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
  }
}
