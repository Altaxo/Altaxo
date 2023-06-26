using Altaxo.Calc.Interpolation;

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
    public double Wavelength_Tolerance_nm { get; init; } = 15;

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
        MinimalSignalToNoiseRatio = 5,
      },
      PeakFitting = new PeakFitting.PeakFittingSeparately()
      {
        FitFunction = new Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude(),
        FitWidthScalingFactor = 2,
      }
    };

    /// <summary>
    /// Gets the interpolation method used for interpolating the differences of Nist wavelength and measured wavelength
    /// in dependence on the measured wavelength.
    /// </summary>
    public IInterpolationFunctionOptions InterpolationMethod { get; init; } = new PolyharmonicSpline1DOptions { RegularizationParameter = 50, DerivativeOrder = 2 };

    /// <summary>
    /// Gets a value indicating whether the position error obtained from the peak fit should be ignored
    /// during the interpolation step. If set to true, the position error is ignored and all points are weighted equally.
    /// </summary>
    public bool InterpolationIgnoreStdDev { get; init; } = true;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.Raman.NeonCalibrationOptions", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NeonCalibrationOptions)obj;
        info.AddEnum("XAxisUnit", s.XAxisUnit);
        info.AddValue("ApproximateLaserWavelength", s.LaserWavelength_Nanometer);
        info.AddValue("WavelengthTolerance", s.Wavelength_Tolerance_nm);
        info.AddValue("FilterOutPeaksCorrespondingToMultipleNistPeaks", s.FilterOutPeaksCorrespondingToMultipleNistPeaks);
        info.AddValue("PeakFindingOptions", s.PeakFindingOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var xAxisUnit = (XAxisUnit)info.GetEnum("XAxisUnit", typeof(XAxisUnit));
        var approximateLaserWavelength = info.GetDouble("ApproximateLaserWavelength");
        var wavelengthTol = info.GetDouble("WavelengthTolerance");
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

    /// <summary>
    /// 2022-08-17 Added InterpolationMethod, and InterpolationIgnoreStdDev
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NeonCalibrationOptions), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NeonCalibrationOptions)obj;
        info.AddEnum("XAxisUnit", s.XAxisUnit);
        info.AddValue("ApproximateLaserWavelength", s.LaserWavelength_Nanometer);
        info.AddValue("WavelengthTolerance", s.Wavelength_Tolerance_nm);
        info.AddValue("FilterOutPeaksCorrespondingToMultipleNistPeaks", s.FilterOutPeaksCorrespondingToMultipleNistPeaks);
        info.AddValue("PeakFindingOptions", s.PeakFindingOptions);
        info.AddValue("InterpolationMethod", s.InterpolationMethod);
        info.AddValue("InterpolationIgnoreStdDev", s.InterpolationIgnoreStdDev);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var xAxisUnit = (XAxisUnit)info.GetEnum("XAxisUnit", typeof(XAxisUnit));
        var approximateLaserWavelength = info.GetDouble("ApproximateLaserWavelength");
        var wavelengthTol = info.GetDouble("WavelengthTolerance");
        var filterOut = info.GetBoolean("FilterOutPeaksCorrespondingToMultipleNistPeaks");
        var peakOptions = info.GetValue<PeakSearchingAndFittingOptions>("PeakFindingOptions", null);
        var interpolationMethod = info.GetValue<IInterpolationFunctionOptions>("InterpolationMethod", null);
        var interpolationIgnore = info.GetBoolean("InterpolationIgnoreStdDev");

        return new NeonCalibrationOptions()
        {
          XAxisUnit = xAxisUnit,
          LaserWavelength_Nanometer = approximateLaserWavelength,
          Wavelength_Tolerance_nm = wavelengthTol,
          FilterOutPeaksCorrespondingToMultipleNistPeaks = filterOut,
          PeakFindingOptions = peakOptions,
          InterpolationMethod = interpolationMethod,
          InterpolationIgnoreStdDev = interpolationIgnore,
        };
      }
    }

    #endregion


    public override string ToString()
    {
      return $"Unit={XAxisUnit}, LWL={LaserWavelength_Nanometer}nm, Tol={Wavelength_Tolerance_nm}nm FilterMult={FilterOutPeaksCorrespondingToMultipleNistPeaks} PeakFind={PeakFindingOptions} Interpol={InterpolationMethod} IgnoreStdDev={InterpolationIgnoreStdDev}";
    }
  }
}
