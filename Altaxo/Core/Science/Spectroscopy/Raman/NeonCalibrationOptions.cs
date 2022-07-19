using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc;

namespace Altaxo.Science.Spectroscopy.Raman
{
  /// <summary>
  /// Options for x-axis calibration of a Raman spectrum by a spectrum measured from a Neon lamp.
  /// </summary>
  public record NeonCalibrationOptions
  {
    /// <summary>
    /// Gets the x axis unit of the Neon spectra.
    /// </summary>
    public XAxisUnit XAxisUnit { get; init; }

    /// <summary>
    /// Gets the approximate laser wavelength in nanometer.
    /// This is neccessary to convert Raman shift values to absolute wavelength.
    /// It is enough to give the value approximately here, because a calibration of the laser wavelength itself is done elsewhere.
    /// </summary>
    public double LaserWavelength_Nanometer { get; init; }

    /// <summary>
    /// Wavelength tolerance in nm (this is the value how much the spectrometer can be differ from calibration)
    /// </summary>
    public double Wavelength_Tolerance_nm { get; init; } = 30;

    public PeakSearchingAndFittingOptions PeakFindingOptions { get; init; } = new PeakSearchingAndFittingOptions() { Preprocessing = new SpectralPreprocessingOptions {  BaselineEstimation = new BaselineEstimation.SNIP_Linear { HalfWidth = 15, IsHalfWidthInXUnits = true} } };
   
  }
}
