#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

using Altaxo.Data;

namespace Altaxo.Science.Spectroscopy.Calibration
{
  /// <summary>
  /// Interface to a data source that may contain an X-axis calibration.
  /// </summary>
  public interface IXCalibrationDataSource
  {
    /// <summary>
    /// Determines whether the table belonging to the data source contains a valid X-axis calibration.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>
    ///   <c>true</c> if the table belonging to the data source contains a valid X-axis calibration; otherwise, <c>false</c>.
    /// </returns>
    bool IsContainingValidXAxisCalibration(DataTable table);


    /// <summary>
    /// Gets the X-axis calibration. Please use <see cref="IsContainingValidXAxisCalibration(DataTable)"/> beforehand to test whether an X-axis calibration is
    /// available.
    /// </summary>
    /// <param name="table">The table belonging to the data source.</param>
    /// <returns>The x-axis calibration data.</returns>
    (double x_uncalibrated, double x_calibrated)[] GetXAxisCalibration(DataTable table);
  }
}
