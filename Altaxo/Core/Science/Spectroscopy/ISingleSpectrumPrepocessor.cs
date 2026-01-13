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
    (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions);


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
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxillaryData? auxillaryData) Execute(double[] x, Matrix<double> y, int[]? regions)
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
      return (x, yresult, newRegions, null);
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
    string? TableName { get; init; }

    /// <summary>
    /// Returns a new instance in which <see cref="TableName"/> is set to the provided value.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>A new instance in which <see cref="TableName"/> is set to the provided value.</returns>
    IReferencingTable WithTableName(string tableName);
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
    (string TableName, int GroupNumber, string XColumnName, string YColumnName)? XYDataOrigin { get; init; }

    /// <summary>
    /// Returns a new instance in which <see cref="XYDataOrigin"/> is set to the provided value.
    /// </summary>
    /// <param name="xyDataOrigin">The name of the table, the group number, and the names of the x and y columns.</param>
    /// <returns>A new instance in which <see cref="XYDataOrigin"/> is set to the provided value.</returns>
    IReferencingXYColumns WithXYDataOrigin((string TableName, int GroupNumber, string XColumnName, string YColumnName) xyDataOrigin);

    /// <summary>
    /// Stores the data of the curve, consisting of x-y pairs.
    /// </summary>
    ImmutableArray<(double x, double y)> XYCurve { get; init; }

    /// <summary>
    /// Returns a new instance in which <see cref="XYCurve"/> is set to the provided value.
    /// </summary>
    /// <param name="xyCurve">The x-y curve.</param>
    /// <returns>A new instance with the provided value set.</returns>
    IReferencingXYColumns WithXYCurve(ImmutableArray<(double x, double y)> xyCurve);
  }
}
