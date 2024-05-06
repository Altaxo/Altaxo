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
using System.Linq;
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;

namespace Altaxo.Science.Spectroscopy.Calibration
{
  public class YCalibrationOptionsDocNode : SpectralPreprocessingOptionsDocNodeBase
  {
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
        var preprocessing = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessingOptions", null);
        var proxyList = SpectralPreprocessingOptionsDocNodeBase.DeserializeProxiesVersion1(info);

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

        var options = new YCalibrationOptions()
        {
          Preprocessing = preprocessing,
          CurveShape = new Altaxo.Calc.FitFunctions.FitFunctionDDWrapper(intensityCurve, arr.Select(x => x.Value).ToArray()),
        };
        return new YCalibrationOptionsDocNode(options, proxyList);
      }
    }

    /// <summary>
    /// 2023-11-20 V1: new property 'InterpolationMethod'
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Science.Spectroscopy.Calibration.YCalibrationOptionsDocNode", 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

        info.AddValueOrNull("InterpolationMethod", s.InterpolationMethod);
        */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var preprocessing = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessingOptions", null);
        var proxyList = SpectralPreprocessingOptionsDocNodeBase.DeserializeProxiesVersion1(info);

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

        var options = new YCalibrationOptions()
        {
          Preprocessing = preprocessing,
          InterpolationMethod = interpolationMethod,
          CurveShape = new Altaxo.Calc.FitFunctions.FitFunctionDDWrapper(intensityCurve, arr.Select(x => x.Value).ToArray()),
        };

        return new YCalibrationOptionsDocNode(options, proxyList);
      }
    }

    /// <summary>
    /// 2023-11-20 V1: new property 'InterpolationMethod'
    /// 2024-04-05 V2: YCalibrationOptions now in _optionsObject
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(YCalibrationOptionsDocNode), 2)]
    public class SerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (YCalibrationOptionsDocNode)obj;
        var preprocessingOptions = s.InternalGetSpectralPreprocessingOptions();
        s.InternalSpectralPreprocessingOptions = preprocessingOptions;
        info.AddValue("Options", s._optionsObject);
        SpectralPreprocessingOptionsDocNodeBase.SerializeProxiesVersion1(info, s, preprocessingOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var options = info.GetValue<YCalibrationOptions>("Options", null);
        var proxyList = SpectralPreprocessingOptionsDocNodeBase.DeserializeProxiesVersion1(info);
        return new YCalibrationOptionsDocNode(options, proxyList);
      }
    }

    #endregion

    protected YCalibrationOptionsDocNode(YCalibrationOptions options, List<(int number, IDocumentLeafNode proxy)> proxyList)
      : base(options, proxyList)
    {
    }


    public YCalibrationOptionsDocNode(YCalibrationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Gets the wrapped spectral preprocessing options. When neccessary, the calibration is updated to reflect the content of the linked calibration table.
    /// </summary>
    /// <returns>The wrapped spectral preprocessing options</returns>
    public YCalibrationOptions GetYCalibrationOptions()
    {
      InternalSpectralPreprocessingOptions = InternalGetSpectralPreprocessingOptions();
      return (YCalibrationOptions)_optionsObject;
    }

    protected override SpectralPreprocessingOptionsBase InternalSpectralPreprocessingOptions
    {
      get
      {
        return ((YCalibrationOptions)_optionsObject).Preprocessing;
      }
      set
      {
        _optionsObject = ((YCalibrationOptions)_optionsObject) with { Preprocessing = value };
      }
    }
  }

}
