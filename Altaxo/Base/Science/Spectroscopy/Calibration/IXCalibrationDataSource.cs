using System;
using System.Collections.Generic;
using Altaxo.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Spectroscopy.Calibration
{
  /// <summary>
  /// Interface to a data source that may contain a x-axis calibration.
  /// </summary>
  public interface IXCalibrationDataSource
  {
    /// <summary>
    /// Determines whether the table belonging to the data source is containing a valid x-axis calibration.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>
    ///   <c>true</c> if the table belonging to the data source is containing a valid x-axis calibration; otherwise, <c>false</c>.
    /// </returns>
    bool IsContainingValidXAxisCalibration(DataTable table);


    /// <summary>
    /// Gets the x axis calibration. Please use <see cref="IsContainingValidXAxisCalibration(DataTable)"/> to test beforehand, if an x-axis calibration is
    /// available.
    /// </summary>
    /// <param name="table">The table belonging to the data source.</param>
    /// <returns>The x-axis calibration data.</returns>
    (double x_uncalibrated, double x_calibrated)[] GetXAxisCalibration(DataTable table);
  }
}
