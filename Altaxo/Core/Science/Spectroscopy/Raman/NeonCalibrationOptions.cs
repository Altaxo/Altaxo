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

    /// <summary>
    /// If true, peaks in the measured spectrum, that corresponds to multiple Nist peaks, are filtered out.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [filter out peaks corresponding to multiple nist peaks]; otherwise, <c>false</c>.
    /// </value>
    public bool FilterOutPeaksCorrespondingToMultipleNistPeaks { get; init; } = true;

    public PeakSearchingAndFittingOptions PeakFindingOptions { get; init; } = new PeakSearchingAndFittingOptions()
    {
      Preprocessing = new SpectralPreprocessingOptions
      {
        BaselineEstimation = new BaselineEstimation.SNIP_Linear
        {
          HalfWidth = 15,
          IsHalfWidthInXUnits = true
        }
      },
      PeakSearching = new PeakSearching.PeakSearchingByCwt
      {
         MinimalRelativeGaussianAmplitude = 0.0005,
      },
      PeakFitting = new PeakFitting.PeakFittingSeparately()
      {
        FitFunction = new Altaxo.Calc.FitFunctions.Probability.GaussAmplitude(),
        FitWidthScalingFactor = 2,
      }
    };

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NeonCalibrationOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NeonCalibrationOptions)obj;
        info.AddEnum("XAxisUnit", s.XAxisUnit);
        info.AddValue("ApproximateLaserWavelength(nm)", s.LaserWavelength_Nanometer);
        info.AddValue("WavelengthTolerance(nm)", s.Wavelength_Tolerance_nm);
        info.AddValue("FilterOutPeaksCorrespondingToMultipleNistPeaks", s.FilterOutPeaksCorrespondingToMultipleNistPeaks);
        info.AddValue("PeakFindingOptions", s.PeakFindingOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var xAxisUnit = (XAxisUnit)info.GetEnum("XAxisUnit", typeof(XAxisUnit));
        var approximateLaserWavelength = info.GetDouble("ApproximateLaserWavelength(nm)");
        var wavelengthTol = info.GetDouble("WavelengthTolerance(nm)");
        var filterOut = info.GetBoolean("FilterOutPeaksCorrespondingToMultipleNistPeaks");
        var peakOptions = info.GetValue<PeakSearchingAndFittingOptions>("PeakFindingOptions", null);


        return new NeonCalibrationOptions()
        {
          XAxisUnit = xAxisUnit,
          LaserWavelength_Nanometer = approximateLaserWavelength,
          Wavelength_Tolerance_nm = wavelengthTol,
          FilterOutPeaksCorrespondingToMultipleNistPeaks = filterOut,
          PeakFindingOptions = peakOptions,
        };
      }
    }
    #endregion


  }
}
