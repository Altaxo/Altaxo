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
  /// Represents the result of a dimension reduction method.
  /// </summary>
  public interface IDimensionReductionResult
  {
    /// <summary>
    /// Saves the result to a destination table.
    /// </summary>
    /// <param name="outputOptions">The output options specifying what to save.</param>
    /// <param name="sourceTable">The source table containing the original data.</param>
    /// <param name="destinationTable">The destination table that will receive the result data.</param>
    /// <param name="columnNumbersOfSpectraInSourceTable">Column indices of the spectra in <paramref name="sourceTable"/>.</param>
    /// <param name="xValuesOfPreprocessedSpectra">X values associated with the preprocessed spectra.</param>
    /// <param name="preprocessedSpectra">The matrix of preprocessed spectra.</param>
    /// <param name="auxiliaryData">Auxiliary data resulting from ensemble processing.</param>
    public void SaveResultToTable(DimensionReductionOutputOptions outputOptions, DataTable sourceTable, DataTable destinationTable, int[] columnNumbersOfSpectraInSourceTable, double[] xValuesOfPreprocessedSpectra, Matrix<double> preprocessedSpectra, IEnsembleProcessingAuxiliaryData auxiliaryData);
  }
}
