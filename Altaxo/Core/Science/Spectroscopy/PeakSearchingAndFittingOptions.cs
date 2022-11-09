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
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Science.Spectroscopy.PeakSearching;

namespace Altaxo.Science.Spectroscopy
{
  /// <summary>
  /// Options for peak searching (determination of approximate peak positions) and peak fitting.
  /// </summary>
  public record PeakSearchingAndFittingOptions
  {
    /// <summary>
    /// Gets/sets the methods for spectral preprocessing.
    /// </summary>
    public SpectralPreprocessingOptions Preprocessing { get; init; } = new SpectralPreprocessingOptions();

    /// <summary>
    /// Gets/sets the peak searching method (method to find approximate peak positions).
    /// </summary>
    public IPeakSearching PeakSearching { get; init; } = new PeakSearching.PeakSearchingByTopology();

    /// <summary>
    /// Gets/sets the peak fitting method.
    /// </summary>
    public IPeakFitting PeakFitting { get; init; } = new PeakFittingNone();

    /// <summary>
    /// Gets/set the options that determine which data to include in the output of a peak searching and fitting operation.
    /// </summary>
    public PeakSearchingAndFittingOutputOptions OutputOptions { get; init; } = new();



    #region Serialization

    /// <summary>
    /// 2022-06-09 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.PeakSearchingAndFittingOptions", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Try to serialize old version");
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PeakSearchingAndFittingOptions?)o ?? new PeakSearchingAndFittingOptions();

        var ds = new SpectralPreprocessingOptions.SerializationSurrogate0();
        info.GetString("BaseType");
        var preprocessing = (SpectralPreprocessingOptions)ds.Deserialize(null, info, null);


        // var preprocessing = info.GetValue<SpectralPreprocessingOptions>("Preprocessing", parent);
        var peakSearching = info.GetValue<IPeakSearching>("PeakSearching", parent);
        var peakFitting = info.GetValue<IPeakFitting>("PeakFitting", parent);


        return s with
        {
          Preprocessing = preprocessing,
          PeakSearching = peakSearching,
          PeakFitting = peakFitting,
        };
      }
    }

    /// <summary>
    /// 2022-07-14 this class no longer derives from SpectralPreprocessOptions, but has it as member
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.PeakSearchingAndFittingOptions", 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingAndFittingOptions)obj;
        info.AddValue("Preprocessing", s.Preprocessing);
        info.AddValue("PeakSearching", s.PeakSearching);
        info.AddValue("PeakFitting", s.PeakFitting);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PeakSearchingAndFittingOptions?)o ?? new PeakSearchingAndFittingOptions();
        var preprocessing = info.GetValue<SpectralPreprocessingOptions>("Preprocessing", parent);
        var peakSearching = info.GetValue<IPeakSearching>("PeakSearching", parent);
        var peakFitting = info.GetValue<IPeakFitting>("PeakFitting", parent);


        return s with
        {
          Preprocessing = preprocessing,
          PeakSearching = peakSearching,
          PeakFitting = peakFitting,
        };
      }
    }

    /// <summary>
    /// 2022-11-09 add output options
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingAndFittingOptions), 2)]
    public class SerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingAndFittingOptions)obj;
        info.AddValue("Preprocessing", s.Preprocessing);
        info.AddValue("PeakSearching", s.PeakSearching);
        info.AddValue("PeakFitting", s.PeakFitting);
        info.AddValue("OutputOptions", s.OutputOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PeakSearchingAndFittingOptions?)o ?? new PeakSearchingAndFittingOptions();
        var preprocessing = info.GetValue<SpectralPreprocessingOptions>("Preprocessing", parent);
        var peakSearching = info.GetValue<IPeakSearching>("PeakSearching", parent);
        var peakFitting = info.GetValue<IPeakFitting>("PeakFitting", parent);
        var outputOptions = info.GetValue<PeakSearchingAndFittingOutputOptions>("OutputOptions", parent);


        return s with
        {
          Preprocessing = preprocessing,
          PeakSearching = peakSearching,
          PeakFitting = peakFitting,
          OutputOptions = outputOptions,
        };
      }
    }

    #endregion


  }

  /// <summary>
  /// Options to choose which data will be included in the results of a peak searching and fitting operation.
  /// </summary>
  public record PeakSearchingAndFittingOutputOptions
  {
    /// <summary>
    /// Gets a value indicating whether the preprocessed spectrum (i.e. the spectrum that is used for peak fitting) is included in the output.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the preprocessed spectrum is included in the output; otherwise, <c>false</c>.
    /// </value>
    public bool OutputPreprocessedCurve { get; init; }

    /// <summary>
    /// Gets a value indicating whether the fit curve of the peak fitting(s) is included in the output.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the fit curve of the peak fitting(s) is included in the output; otherwise, <c>false</c>.
    /// </value>
    public bool OutputFitCurve { get; init; }

    public bool OutputFitCurveAsSeparatePeaks { get; init; }

    private int _outputFitCurveSamplingFactor = 10;

    /// <summary>
    /// Gets/sets the sampling factor of the fit curve. A sampling factor of 1 samples the fit curve at
    /// the same positions at the fitted curve, a factor of 2 takes one additional point inbetween to original points, etc.
    /// </summary>
    /// <value>
    /// The output fit curve sampling factor.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">Value must be >=1, nameof(OutputFitCurveScalingFactor)</exception>
    public int OutputFitCurveSamplingFactor
    {
      get => _outputFitCurveSamplingFactor;
      init
      {
        if (!(_outputFitCurveSamplingFactor >= 1))
          throw new ArgumentOutOfRangeException("Value must be >=1", nameof(OutputFitCurveSamplingFactor));

        _outputFitCurveSamplingFactor = value;
      }
    }

    #region Serialization



    /// <summary>
    /// 2022-11-09 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingAndFittingOutputOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingAndFittingOutputOptions)obj;
        info.AddValue("OutputPreprocessed", s.OutputPreprocessedCurve);
        info.AddValue("OutputFit", s.OutputFitCurve);
        info.AddValue("OutputSeparatePeaks", s.OutputFitCurveAsSeparatePeaks);
        info.AddValue("SamplingFactor", s.OutputFitCurveSamplingFactor);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PeakSearchingAndFittingOutputOptions?)o ?? new PeakSearchingAndFittingOutputOptions();
        var preprocessed = info.GetBoolean("OutputPreprocessed");
        var fit = info.GetBoolean("OutputFit");
        var separate = info.GetBoolean("OutputSeparatePeaks");
        var sampling = info.GetInt32("SamplingFactor");

        return s with
        {
          OutputPreprocessedCurve = preprocessed,
          OutputFitCurve = fit,
          OutputFitCurveAsSeparatePeaks = separate,
          OutputFitCurveSamplingFactor = sampling,
        };
      }
    }
    #endregion

  }
}
