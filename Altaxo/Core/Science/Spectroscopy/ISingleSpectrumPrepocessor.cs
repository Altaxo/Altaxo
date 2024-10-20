﻿#region Copyright

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


using System.Collections.Immutable;

namespace Altaxo.Science.Spectroscopy
{
  /// <summary>
  /// Interface to a class that is able to pre-process a single spectrum.
  /// The convention is: if data need to be changed in an array, a new instance must be allocated for this.
  /// </summary>
  public interface ISingleSpectrumPreprocessor
  {
    /// <summary>
    /// Executes the processor.
    /// </summary>
    /// <param name="x">The x-values of the spectrum.</param>
    /// <param name="y">The y-values of the spectrum.</param>
    /// <param name="regions">The spectral regions. Can be null (if the array is one region). Each element in this array
    /// is the start index of a new spectral region.</param>
    /// <returns>X-values, y-values and regions of the processed spectrum.</returns>
    (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions);
  }

  /// <summary>
  /// Additional interface that is used if the spectral preprocessor
  /// is referencing a table, for instance a calibration table.
  /// </summary>
  public interface IReferencingTable
  {
    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    string? TableName { get; init; }

    /// <summary>
    /// Returns a new instance, in which the table name is set to the provided name.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <returns>New instance, in which the table name is set to the provided name.</returns>
    IReferencingTable WithTableName(string tableName);
  }

  /// <summary>
  /// Additional interface that is used if the spectral preprocessor
  /// is referencing an x and y column, for instance if another spectrum should be added or subtracted.
  /// </summary>
  public interface IReferencingXYColumns
  {
    /// <summary>
    /// Gets the name of the table, the group number, and the names of the x and y columns.
    /// </summary>
    (string TableName, int GroupNumber, string XColumnName, string YColumnName)? XYDataOrigin { get; init; }

    /// <summary>
    /// Returns a new instance, in which the column information is set to the provided name of the table, the group number, and the names of the x and y columns.
    /// </summary>
    /// <param name="xyDataOrigin">The name of the table, the group number, and the names of the x and y columns..</param>
    /// <returns>A new instance, in which the column information is set to the provided name of the table, the group number, and the names of the x and y columns.</returns>
    IReferencingXYColumns WithXYDataOrigin((string TableName, int GroupNumber, string XColumnName, string YColumnName) xyDataOrigin);

    /// <summary>
    /// Stores the data of the curve (consisting of x-y pairs).
    /// </summary>
    ImmutableArray<(double x, double y)> XYCurve { get; init; }

    /// <summary>
    /// Returns a new instance, in which the XYCurve is set to a new instance.
    /// </summary>
    /// <param name="xyCurve">The x-y curve.</param>
    /// <returns>New instance with the provided value set.</returns>
    IReferencingXYColumns WithXYCurve(ImmutableArray<(double x, double y)> xyCurve);
  }
}
