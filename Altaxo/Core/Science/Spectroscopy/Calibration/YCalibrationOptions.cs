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

using System;
using System.Linq;
using Altaxo.Calc;
using Altaxo.Calc.FitFunctions;
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Science.Spectroscopy.Calibration
{
  /// <summary>
  /// Options for spectroscopic intensity calibration by means of a light source with known intensity curve.
  /// </summary>
  public record YCalibrationOptions
  {
    /// <summary>
    /// Gets the intensity curve of calibration source. This is usually a peak function, for instance a Gaussian shape with one or more terms (and baseline polynomial).
    /// </summary>
    public IScalarFunctionDD CurveShape { get; init; } = new FitFunctionDDWrapper(new Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude(1, -1), new double[] { 1, 0, 1 });

    public SpectralPreprocessingOptionsBase Preprocessing { get; init; } = new SpectralPreprocessingOptions();

    /// <summary>
    /// Gets or sets the smoothing interpolation that is used to smooth the resulting curve.
    /// The value can be null: in this case, no smoothing is performed.
    /// </summary>
    public IInterpolationFunctionOptions? InterpolationMethod { get; init; }

    /// <summary>
    /// Gets the minimal valid x value of the calibration curve that comes with the certificate of the source.
    /// </summary>
    public double MinimalValidXValueOfCurve { get; init; } = double.NegativeInfinity;

    /// <summary>
    /// Gets the maximal valid x value of the calibration curve that comes with the certificate of the source.
    /// </summary>
    public double MaximalValidXValueOfCurve { get; init; } = double.PositiveInfinity;

    private double _maximalGainRatio = double.PositiveInfinity;
    /// <summary>
    /// Gets the maximal allowed ratio of the gain that is caused by the intensity correction.
    /// Example: if the value is 10, and in the center of the spectrum the gain is 1, then
    /// the spectrum is cropped at the ends, where the gain reaches 10.
    /// </summary>
    public double MaximalGainRatio
    {
      get => _maximalGainRatio;
      set
      {
        if (!(value > 1))
          throw new ArgumentException("The maximal gain ratio has to be a value > 1");
        _maximalGainRatio = value;
      }
    }

    #region Serialization

    /// <summary>
    /// 2023-03-30 Initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.Calibration.YCalibrationOptions", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
        var s = (YCalibrationOptions)obj;
        info.AddValue("SpectralPreprocessing", s.Preprocessing);
        info.AddValue("CurveShape", s.CurveShape);

        info.CreateArray("CurveParameters", s.CurveParameters.Length);
        {
          for (int i = 0; i < s.CurveParameters.Length; i++)
          {
            info.CreateElement("e");
            {
              info.AddValue("Name", s.CurveParameters[i].Name);
              info.AddValue("Value", s.CurveParameters[i].Value);
            }
            info.CommitElement();
          }
        }
        info.CommitArray();
        */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var spectralPreprocessing = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessing", parent);
        var intensityCurve = info.GetValue<IFitFunction>("CurveShape", null);

        var count = info.OpenArray("CurveParameters");
        var arr = new (string Name, double Value)[count];
        {
          for (int i = 0; i < count; ++i)
          {
            info.OpenElement(); // "e"
            var s = info.GetString("Name");
            var v = info.GetDouble("Value");
            info.CloseElement();
            arr[i] = (s, v);
          }
        }
        info.CloseArray(count);


        return new YCalibrationOptions
        {
          Preprocessing = spectralPreprocessing,
          CurveShape = new Altaxo.Calc.FitFunctions.FitFunctionDDWrapper(intensityCurve, arr.Select(x => x.Value).ToArray()),
          InterpolationMethod = null,
        };
      }
    }

    /// <summary>
    /// 2023-11-20 V1: new property 'InterpolationMethod' added
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.Calibration.YCalibrationOptions", 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
        var s = (YCalibrationOptions)obj;
        info.AddValue("SpectralPreprocessing", s.Preprocessing);
        info.AddValue("CurveShape", s.CurveShape);

        info.CreateArray("CurveParameters", s.CurveParameters.Length);
        {
          for (int i = 0; i < s.CurveParameters.Length; i++)
          {
            info.CreateElement("e");
            {
              info.AddValue("Name", s.CurveParameters[i].Name);
              info.AddValue("Value", s.CurveParameters[i].Value);
            }
            info.CommitElement();
          }
        }
        info.CommitArray();

        info.AddValueOrNull("InterpolationMethod", s.InterpolationMethod);
        */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var spectralPreprocessing = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessing", parent);
        var intensityCurve = info.GetValue<IFitFunction>("CurveShape", null);

        var count = info.OpenArray("CurveParameters");
        var arr = new (string Name, double Value)[count];
        {
          for (int i = 0; i < count; ++i)
          {
            info.OpenElement(); // "e"
            var s = info.GetString("Name");
            var v = info.GetDouble("Value");
            info.CloseElement();
            arr[i] = (s, v);
          }
        }
        info.CloseArray(count);

        var interpolationMethod = info.GetValueOrNull<IInterpolationFunctionOptions>("InterpolationMethod", null);

        return new YCalibrationOptions
        {
          Preprocessing = spectralPreprocessing,
          CurveShape = new Altaxo.Calc.FitFunctions.FitFunctionDDWrapper(intensityCurve, arr.Select(x => x.Value).ToArray()),
          InterpolationMethod = interpolationMethod,
        };
      }
    }

    /// <summary>
    /// 2024-04-04 V2: new properties MinimalValidXValueOfCurve, MaximalValidXValueOfCurve and MaximalGainRatio
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.Calibration.YCalibrationOptions", 2)]
    public class SerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");

        /*
        var s = (YCalibrationOptions)obj;
        info.AddValue("SpectralPreprocessing", s.Preprocessing);
        info.AddValue("CurveShape", s.CurveShape);

        info.CreateArray("CurveParameters", s.CurveParameters.Length);
        {
          for (int i = 0; i < s.CurveParameters.Length; i++)
          {
            info.CreateElement("e");
            {
              info.AddValue("Name", s.CurveParameters[i].Name);
              info.AddValue("Value", s.CurveParameters[i].Value);
            }
            info.CommitElement();
          }
        }
        info.CommitArray();

        info.AddValueOrNull("InterpolationMethod", s.InterpolationMethod);

        info.AddValue("MinimalValidXValueOfCurve", s.MinimalValidXValueOfCurve);
        info.AddValue("MaximalValidXValueOfCurve", s.MaximalValidXValueOfCurve);
        info.AddValue("MaximalGainRatio", s.MaximalGainRatio);
        */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var spectralPreprocessing = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessing", parent);
        var intensityCurve = info.GetValue<IFitFunction>("CurveShape", null);

        var count = info.OpenArray("CurveParameters");
        var arr = new (string Name, double Value)[count];
        {
          for (int i = 0; i < count; ++i)
          {
            info.OpenElement(); // "e"
            var s = info.GetString("Name");
            var v = info.GetDouble("Value");
            info.CloseElement();
            arr[i] = (s, v);
          }
        }
        info.CloseArray(count);

        var interpolationMethod = info.GetValueOrNull<IInterpolationFunctionOptions>("InterpolationMethod", null);

        var minimalValidXValueOfCurve = info.GetDouble("MinimalValidXValueOfCurve");
        var maximalValidXValueOfCurve = info.GetDouble("MaximalValidXValueOfCurve");
        var maximalGainRatio = info.GetDouble("MaximalGainRatio");

        return new YCalibrationOptions
        {
          Preprocessing = spectralPreprocessing,
          CurveShape = new Altaxo.Calc.FitFunctions.FitFunctionDDWrapper(intensityCurve, arr.Select(x => x.Value).ToArray()),
          InterpolationMethod = interpolationMethod,
          MinimalValidXValueOfCurve = minimalValidXValueOfCurve,
          MaximalValidXValueOfCurve = maximalValidXValueOfCurve,
          MaximalGainRatio = maximalGainRatio,
        };
      }
    }

    /// <summary>
    /// 2024-04-04 V2: new properties MinimalValidXValueOfCurve, MaximalValidXValueOfCurve and MaximalGainRatio
    /// 2024-05-06 V3: CurveShape now is IScalarFunctionDD, thus parameters are stored together with curve
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(YCalibrationOptions), 3)]
    public class SerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (YCalibrationOptions)obj;
        info.AddValue("SpectralPreprocessing", s.Preprocessing);
        info.AddValue("CurveShape", s.CurveShape);
        info.AddValueOrNull("InterpolationMethod", s.InterpolationMethod);
        info.AddValue("MinimalValidXValueOfCurve", s.MinimalValidXValueOfCurve);
        info.AddValue("MaximalValidXValueOfCurve", s.MaximalValidXValueOfCurve);
        info.AddValue("MaximalGainRatio", s.MaximalGainRatio);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var spectralPreprocessing = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessing", parent);
        var intensityCurve = info.GetValue<IScalarFunctionDD>("CurveShape", null);
        var interpolationMethod = info.GetValueOrNull<IInterpolationFunctionOptions>("InterpolationMethod", null);

        var minimalValidXValueOfCurve = info.GetDouble("MinimalValidXValueOfCurve");
        var maximalValidXValueOfCurve = info.GetDouble("MaximalValidXValueOfCurve");
        var maximalGainRatio = info.GetDouble("MaximalGainRatio");

        return new YCalibrationOptions
        {
          Preprocessing = spectralPreprocessing,
          CurveShape = intensityCurve,
          InterpolationMethod = interpolationMethod,
          MinimalValidXValueOfCurve = minimalValidXValueOfCurve,
          MaximalValidXValueOfCurve = maximalValidXValueOfCurve,
          MaximalGainRatio = maximalGainRatio,
        };
      }
    }

    #endregion

  }
}
