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
using System.Collections.Generic;
using System.Collections.Immutable;
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;

namespace Altaxo.Science.Spectroscopy.Calibration
{
  public class YCalibrationOptionsDocNode : SpectralPreprocessingOptionsDocNode
  {
    /// <summary>
    /// Gets the intensity curve of calibration source. This is usually a peak function, for instance a Gaussian shape with one or more terms (and baseline polynomial).
    /// </summary>
    public IFitFunction CurveShape { get; init; } = new Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude(1, -1);

    /// <summary>
    /// Gets the curve parameters for the <see cref="CurveShape"/> function.
    /// </summary>
    public ImmutableArray<(string Name, double Value)> CurveParameters { get; init; } = ImmutableArray<(string Name, double Value)>.Empty;

    /// <summary>
    /// Gets or sets the smoothing interpolation that is used to smooth the resulting curve.
    /// The value can be null: in this case, no smoothing is performed.
    /// </summary>
    public IInterpolationFunctionOptions? InterpolationMethod { get; init; }

    #region Serialization

    /// <summary>
    /// 2023-03-31 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Science.Spectroscopy.Calibration.YCalibrationOptionsDocNode", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
        var s = (YCalibrationOptionsDocNode)obj;
        var preProcessingOptions = s.GetSpectralPreprocessingOptions();
        info.AddValue("SpectralPreprocessingOptions", preProcessingOptions);
        SpectralPreprocessingOptionsDocNode.SerializationSurrogate1.SerializeProxiesVersion1(info, s, preProcessingOptions);
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
        var options = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessingOptions", null);
        var proxyList = SpectralPreprocessingOptionsDocNode.SerializationSurrogate1.DeserializeProxiesVersion1(info);

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


        return new YCalibrationOptionsDocNode(options, proxyList, intensityCurve, arr.ToImmutableArray(), null);
      }
    }

    /// <summary>
    /// 2023-11-20 V1: new property 'InterpolationMethod'
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(YCalibrationOptionsDocNode), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (YCalibrationOptionsDocNode)obj;
        var preProcessingOptions = s.GetSpectralPreprocessingOptions();
        info.AddValue("SpectralPreprocessingOptions", preProcessingOptions);
        SpectralPreprocessingOptionsDocNode.SerializationSurrogate1.SerializeProxiesVersion1(info, s, preProcessingOptions);
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
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var options = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessingOptions", null);
        var proxyList = SpectralPreprocessingOptionsDocNode.SerializationSurrogate1.DeserializeProxiesVersion1(info);

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

        return new YCalibrationOptionsDocNode(options, proxyList, intensityCurve, arr.ToImmutableArray(), interpolationMethod);
      }
    }
    #endregion

    protected YCalibrationOptionsDocNode(SpectralPreprocessingOptionsBase options, List<(int number, IDocumentLeafNode proxy)> proxyList, IFitFunction curve, ImmutableArray<(string Name, double Value)> curveParameters, IInterpolationFunctionOptions? interpolationMethod)
      : base(options, proxyList)
    {
      CurveShape = curve;
      CurveParameters = curveParameters;
      InterpolationMethod = interpolationMethod;
    }


    public YCalibrationOptionsDocNode(YCalibrationOptions options) : base(options.Preprocessing)
    {
      CurveShape = options.CurveShape;
      CurveParameters = options.CurveParameters;
      InterpolationMethod = options.InterpolationMethod;
    }

    /// <summary>
    /// Gets the wrapped spectral preprocessing options. When neccessary, the calibration is updated to reflect the content of the linked calibration table.
    /// </summary>
    /// <returns>The wrapped spectral preprocessing options</returns>
    public YCalibrationOptions GetYCalibrationOptions()
    {
      var preprocessing = GetSpectralPreprocessingOptions();

      return new YCalibrationOptions
      {
        Preprocessing = preprocessing,
        CurveShape = CurveShape,
        CurveParameters = CurveParameters,
        InterpolationMethod = InterpolationMethod,
      };
    }
  }

}
