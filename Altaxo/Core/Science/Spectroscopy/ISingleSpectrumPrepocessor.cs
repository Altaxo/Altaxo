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
using System.Collections.Immutable;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;

namespace Altaxo.Science.Spectroscopy
{
  /// <summary>
  /// Interface to a class that is able to pre-process a single spectrum.
  /// The convention is: if data needs to be changed in an array, a new instance must be allocated for this.
  /// </summary>
  public interface ISingleSpectrumPreprocessor
  {
    /// <summary>
    /// Executes the processor.
    /// </summary>
    /// <param name="x">The x-values of the spectrum.</param>
    /// <param name="y">The y-values of the spectrum.</param>
    /// <param name="regions">
    /// The spectral regions. Can be <see langword="null"/> (if the array is one region). Each element in this array
    /// is the start index of a new spectral region.
    /// </param>
    /// <returns>X-values, y-values, and regions of the processed spectrum.</returns>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions);


    /// <summary>
    /// Executes the processor for an ensemble of spectra.
    /// </summary>
    /// <param name="x">The x-values of the spectrum.</param>
    /// <param name="y">The spectra. Each row of the matrix represents a spectrum.</param>
    /// <param name="regions">
    /// The spectral regions. Can be <see langword="null"/> (if the array is one region). Each element in this array
    /// is the start index of a new spectral region.
    /// </param>
    /// <returns>X-values, y-values, and regions of the processed spectrum.</returns>
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxiliaryData? auxiliaryData) Execute(double[] x, Matrix<double> y, int[]? regions)
    {
      var (newX, newY, newRegions) = Execute(x, y.Row(0).ToArray(), regions);
      var yresult = Matrix<double>.Build.Dense(y.RowCount, newX.Length);
      yresult.SetRow(0, newY);

      for (int r = 1; r < y.RowCount; r++)
      {
        var (otherX, otherY, otherRegions) = Execute(x, y.Row(r).ToArray(), regions);

        if (!ArrayExtensions.AreEqual(newX, otherX))
          throw new InvalidOperationException("The preprocessor produced different x-values for different rows.");
        if (!ArrayExtensions.AreEqual(newRegions, otherRegions))
          throw new InvalidOperationException("The preprocessor produced different regions for different rows.");

        yresult.SetRow(r, otherY);
      }
      return (newX, yresult, newRegions, null);
    }

    /// <summary>
    /// Executes the prediction algorithm using the specified input features and target values, returning the predicted
    /// results for the given regions.
    /// </summary>
    /// <param name="x">An array of doubles that represents the input features to be used for prediction. Cannot be null.</param>
    /// <param name="y">A matrix containing the target values associated with the input features. Cannot be null.</param>
    /// <param name="regions">An optional array of integers specifying the regions for which predictions should be made. If null, predictions
    /// are performed for all available regions.</param>
    /// <param name="auxiliaryData">Optional auxiliary data that may be used to assist in the prediction process. May be null.</param>
    /// <returns>A tuple containing an array of predicted values and a matrix of results corresponding to the input data.</returns>
    /// <exception cref="NotImplementedException">Thrown in all cases, as this method is not yet implemented.</exception>
    public (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxiliaryData? auxiliaryData)
    {
      // the default implementation of this method simply calls the Execute method, ignoring the auxiliary data. This is a placeholder for future implementations that may utilize the auxiliary data for prediction purposes.
      var (newX, newY, newRegions, _) = Execute(x, y, regions);
      return (newX, newY, newRegions);
    }
  }

  /// <summary>
  /// Additional interface that is used if the spectral preprocessor
  /// is referencing a table, for instance a calibration table.
  /// </summary>
  public interface IReferencingTable
  {
    /// <summary>
    /// Gets the name of the referenced table.
    /// </summary>
    public string? TableName { get; init; }

    /// <summary>
    /// Returns a new instance in which <see cref="TableName"/> is set to the provided value.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>A new instance in which <see cref="TableName"/> is set to the provided value.</returns>
    public IReferencingTable WithTableName(string tableName);
  }

  /// <summary>
  /// Additional interface that is used if the spectral preprocessor
  /// is referencing an x and y column, for instance if another spectrum should be added or subtracted.
  /// </summary>
  public interface IReferencingXYColumns
  {
    /// <summary>
    /// Gets the origin of the referenced x-y data.
    /// </summary>
    /// <value>
    /// The name of the table, the group number, and the names of the x and y columns, or <see langword="null"/>.
    /// </value>
    public (string TableName, int GroupNumber, string XColumnName, string YColumnName)? XYDataOrigin { get; init; }

    /// <summary>
    /// Returns a new instance in which <see cref="XYDataOrigin"/> is set to the provided value.
    /// </summary>
    /// <param name="xyDataOrigin">The name of the table, the group number, and the names of the x and y columns.</param>
    /// <returns>A new instance in which <see cref="XYDataOrigin"/> is set to the provided value.</returns>
    public IReferencingXYColumns WithXYDataOrigin((string TableName, int GroupNumber, string XColumnName, string YColumnName) xyDataOrigin);

    /// <summary>
    /// Stores the data of the curve, consisting of x-y pairs.
    /// </summary>
    public ImmutableArray<(double x, double y)> XYCurve { get; init; }

    /// <summary>
    /// Returns a new instance in which <see cref="XYCurve"/> is set to the provided value.
    /// </summary>
    /// <param name="xyCurve">The x-y curve.</param>
    /// <returns>A new instance with the provided value set.</returns>
    public IReferencingXYColumns WithXYCurve(ImmutableArray<(double x, double y)> xyCurve);
  }
}
