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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Science.Spectroscopy
{
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
    public bool OutputPreprocessedCurve { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the fit curve of the peak fitting(s) is included in the output.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the fit curve of the peak fitting(s) is included in the output; otherwise, <c>false</c>.
    /// </value>
    public bool OutputFitCurve { get; init; } = true;

    public bool OutputFitCurveAsSeparatePeaks { get; init; } = true;

    private int _outputFitCurveSamplingFactor = 3;

    private int _outputFitCurveAsSeparatePeaksSamplingFactor = 5;

    private IReadOnlyList<string> _propertyNames = [];

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

    /// <summary>
    /// Gets/sets the sampling factor of the separate peak curves. A sampling factor of 1 samples the fit curve at
    /// the same positions at the fitted curve, a factor of 2 takes one additional point inbetween to original points, etc.
    /// </summary>
    /// <value>
    /// The output separate peaks fit curves sampling factor.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">Value must be >=1, nameof(OutputFitCurveAsSeparatePeaksSamplingFactor)</exception>
    public int OutputFitCurveAsSeparatePeaksSamplingFactor
    {
      get => _outputFitCurveAsSeparatePeaksSamplingFactor;
      init
      {
        if (!(_outputFitCurveAsSeparatePeaksSamplingFactor >= 1))
          throw new ArgumentOutOfRangeException("Value must be >=1", nameof(OutputFitCurveAsSeparatePeaksSamplingFactor));

        _outputFitCurveAsSeparatePeaksSamplingFactor = value;
      }
    }

    /// <summary>
    /// Gets or sets name of additional properties. These properties are intended to be shown in the columns of the resulting peak fitting table.
    /// </summary>
    public IReadOnlyList<string> PropertyNames
    {
      get => _propertyNames;
      set
      {
        if (value is null)
          throw new System.ArgumentNullException(nameof(value));
        _propertyNames = value.Where(x => !string.IsNullOrEmpty(x)).ToArray();
      }
    }

    #region Serialization

    /// <summary>
    /// 2022-11-09 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.PeakSearchingAndFittingOutputOptions", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingAndFittingOutputOptions)obj;
        info.AddValue("OutputPreprocessed", s.OutputPreprocessedCurve);
        info.AddValue("OutputFitCurve", s.OutputFitCurve);
        info.AddValue("FitCurveSamplingFactor", s.OutputFitCurveSamplingFactor);
        info.AddValue("OutputSeparatePeaks", s.OutputFitCurveAsSeparatePeaks);
        info.AddValue("SeparatePeaksSamplingFactor", s.OutputFitCurveAsSeparatePeaksSamplingFactor);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PeakSearchingAndFittingOutputOptions?)o ?? new PeakSearchingAndFittingOutputOptions();
        var preprocessed = info.GetBoolean("OutputPreprocessed");
        var fit = info.GetBoolean("OutputFitCurve");
        var fitSampling = info.GetInt32("FitCurveSamplingFactor");
        var separate = info.GetBoolean("OutputSeparatePeaks");
        var separateSampling = info.GetInt32("SeparatePeaksSamplingFactor");

        return s with
        {
          OutputPreprocessedCurve = preprocessed,
          OutputFitCurve = fit,
          OutputFitCurveSamplingFactor = fitSampling,
          OutputFitCurveAsSeparatePeaks = separate,
          OutputFitCurveAsSeparatePeaksSamplingFactor = separateSampling,
        };
      }
    }

    /// <summary>
    /// 2024-09-15 V1: added list of properties
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingAndFittingOutputOptions), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingAndFittingOutputOptions)obj;
        info.AddValue("OutputPreprocessed", s.OutputPreprocessedCurve);
        info.AddValue("OutputFitCurve", s.OutputFitCurve);
        info.AddValue("FitCurveSamplingFactor", s.OutputFitCurveSamplingFactor);
        info.AddValue("OutputSeparatePeaks", s.OutputFitCurveAsSeparatePeaks);
        info.AddValue("SeparatePeaksSamplingFactor", s.OutputFitCurveAsSeparatePeaksSamplingFactor);
        info.AddArray("PropertyNames", s.PropertyNames, s.PropertyNames.Count);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PeakSearchingAndFittingOutputOptions?)o ?? new PeakSearchingAndFittingOutputOptions();
        var preprocessed = info.GetBoolean("OutputPreprocessed");
        var fit = info.GetBoolean("OutputFitCurve");
        var fitSampling = info.GetInt32("FitCurveSamplingFactor");
        var separate = info.GetBoolean("OutputSeparatePeaks");
        var separateSampling = info.GetInt32("SeparatePeaksSamplingFactor");
        var propertyNames = info.GetArrayOfStrings("PropertyNames");

        return s with
        {
          OutputPreprocessedCurve = preprocessed,
          OutputFitCurve = fit,
          OutputFitCurveSamplingFactor = fitSampling,
          OutputFitCurveAsSeparatePeaks = separate,
          OutputFitCurveAsSeparatePeaksSamplingFactor = separateSampling,
          PropertyNames = propertyNames,
        };
      }
    }
    #endregion

  }
}
