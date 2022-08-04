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


namespace Altaxo.Science.Spectroscopy.Raman
{
  /// <summary>
  /// Options for x-axis calibration of a Raman spectrum by a spectrum measured from a Neon lamp.
  /// </summary>
  public record SiliconCalibrationOptions
  {
    public const double ReferenceTemperature_OfficialShiftValue_Silicon_K = 20 + 273.15;
    public const double OfficialShiftValue_Silicone_invcm = 520.45; // Ref. Nat. Metro. Inst. Japan, 
    public const double OfficialShiftValueTemperatureCoefficient_Silicon_invcm = -0.022; // Si (1,1,1), Ref: Saltonstall et al., Rev.Sci.Instr. 84, 064903 (2013)

    /// <summary>
    /// Wavelength tolerance in nm (this is the value how much the spectrometer can be differ from calibration)
    /// </summary>
    public double RelativeShift_Tolerance_invcm { get; init; } = 10;

    public double GetOfficialShiftValue_Silicon_invcm()
    {
      return OfficialShiftValue_Silicone_invcm + OfficialShiftValueTemperatureCoefficient_Silicon_invcm * (Temperature.AsValueInSIUnits - ReferenceTemperature_OfficialShiftValue_Silicon_K);
    }

    public Altaxo.Units.DimensionfulQuantity Temperature { get; init; } = new Units.DimensionfulQuantity(20, Units.Temperature.DegreesCelsius.Instance);

    public PeakSearchingAndFittingOptions PeakFindingOptions { get; init; } = new PeakSearchingAndFittingOptions()
    {
      Preprocessing = new SpectralPreprocessingOptions
      {
        BaselineEstimation = new BaselineEstimation.SNIP_Linear
        {
          HalfWidth = 15,
          IsHalfWidthInXUnits = true,
        }
      },
      PeakFitting = new PeakFitting.PeakFittingSeparately()
      {
        FitFunction = new Altaxo.Calc.FitFunctions.Peaks.PearsonIVAmplitude(),
        FitWidthScalingFactor = 2,
      }
    };

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SiliconCalibrationOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SiliconCalibrationOptions)obj;
        info.AddValue("Temperature", s.Temperature);
        info.AddValue("RelativeShiftTolerance", s.RelativeShift_Tolerance_invcm);
        info.AddValue("PeakFindingOptions", s.PeakFindingOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var temperature = info.GetValue<Units.DimensionfulQuantity>("Temperature", null);
        var tolerance = info.GetDouble("RelativeShiftTolerance");
        var peakOptions = info.GetValue<PeakSearchingAndFittingOptions>("PeakFindingOptions", null);


        return new SiliconCalibrationOptions()
        {
          Temperature = temperature,
          RelativeShift_Tolerance_invcm = tolerance,
          PeakFindingOptions = peakOptions,
        };
      }
    }
    #endregion



  }
}
