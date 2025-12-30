namespace Altaxo.Science.Spectroscopy.Raman
{
  /// <summary>
  /// Units that describe the x-axis of Raman spectra used for calibration and analysis.
  /// </summary>
  public enum XAxisUnit
  {
    /// <summary>
    /// Relative Raman shift in inverse centimeters (cm⁻¹).
    /// </summary>
    RelativeShiftInverseCentimeter,

    /// <summary>
    /// Absolute wavelength in nanometers (nm).
    /// </summary>
    AbsoluteWavelengthNanometer,

    /// <summary>
    /// Absolute wavenumber in inverse centimeters (cm⁻¹).
    /// </summary>
    AbsoluteWavenumberInverseCentimeter
  }
}
