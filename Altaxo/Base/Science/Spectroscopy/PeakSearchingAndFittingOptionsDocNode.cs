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
using System.Collections.Generic;
using Altaxo.Main;
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Science.Spectroscopy.PeakSearching;

namespace Altaxo.Science.Spectroscopy
{
  public class PeakSearchingAndFittingOptionsDocNode : SpectralPreprocessingOptionsDocNodeBase
  {

    #region Serialization

    /// <summary>
    /// 2022-08-06 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Science.Spectroscopy.PeakSearchingAndFittingOptionsDocNode", 0)]
    public new class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
        var s = (PeakSearchingAndFittingOptionsDocNode)obj;
        info.AddValue("SpectralPreprocessingOptions", s.GetSpectralPreprocessingOptions());
        info.AddValueOrNull("CalibrationTableProxy", s._calibrationTableProxy);
        info.AddValue("PeakSearching", s.PeakSearching);
        info.AddValue("PeakFitting", s.PeakFitting);
        */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var preprocessing = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessingOptions", null);
        var proxyList = SpectralPreprocessingOptionsDocNode.SerializationSurrogate0.DeserializeProxiesVersion0(info, parent, preprocessing);
        var peakSearching = info.GetValue<IPeakSearching>("PeakSearching", null);
        var peakFitting = info.GetValue<IPeakFitting>("PeakFitting", null);

        var options = new PeakSearchingAndFittingOptions
        {
          Preprocessing = preprocessing,
          PeakSearching = peakSearching,
          PeakFitting = peakFitting,
          OutputOptions = new PeakSearchingAndFittingOutputOptions(),
        };
        return new PeakSearchingAndFittingOptionsDocNode(options, proxyList);
      }
    }

    /// <summary>
    /// 2022-08-06 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Science.Spectroscopy.PeakSearchingAndFittingOptionsDocNode", 1)]
    public new class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
                var s = (PeakSearchingAndFittingOptionsDocNode)obj;
                info.AddValue("SpectralPreprocessingOptions", s.GetSpectralPreprocessingOptions());
                info.AddValueOrNull("CalibrationTableProxy", s._calibrationTableProxy);
                info.AddValue("PeakSearching", s.PeakSearching);
                info.AddValue("PeakFitting", s.PeakFitting);
                info.AddValue("OutputOptions", s.OutputOptions);
        */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var preprocessing = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessingOptions", null);
        var proxyList = SpectralPreprocessingOptionsDocNode.SerializationSurrogate0.DeserializeProxiesVersion0(info, parent, preprocessing);
        var peakSearching = info.GetValue<IPeakSearching>("PeakSearching", null);
        var peakFitting = info.GetValue<IPeakFitting>("PeakFitting", null);
        var outputOptions = info.GetValue<PeakSearchingAndFittingOutputOptions>("OutputOptions", null);

        var options = new PeakSearchingAndFittingOptions
        {
          Preprocessing = preprocessing,
          PeakSearching = peakSearching,
          PeakFitting = peakFitting,
          OutputOptions = outputOptions,
        };

        return new PeakSearchingAndFittingOptionsDocNode(options, proxyList);
      }
    }

    /// <summary>
    /// 2023-03-30 A list of proxies now can be serialized.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Science.Spectroscopy.PeakSearchingAndFittingOptionsDocNode", 2)]
    public class SerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
        var s = (PeakSearchingAndFittingOptionsDocNode)obj;
        var preProcessingOptions = s.GetSpectralPreprocessingOptions();
        info.AddValue("SpectralPreprocessingOptions", preProcessingOptions);
        SpectralPreprocessingOptionsDocNode.SerializationSurrogate1.SerializeProxiesVersion1(info, s, preProcessingOptions);
        info.AddValue("PeakSearching", s.PeakSearching);
        info.AddValue("PeakFitting", s.PeakFitting);
        info.AddValue("OutputOptions", s.OutputOptions);
        */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var preprocessing = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessingOptions", null);
        var proxyList = SpectralPreprocessingOptionsDocNodeBase.DeserializeProxiesVersion1(info);
        var peakSearching = info.GetValue<IPeakSearching>("PeakSearching", null);
        var peakFitting = info.GetValue<IPeakFitting>("PeakFitting", null);
        var outputOptions = info.GetValue<PeakSearchingAndFittingOutputOptions>("OutputOptions", null);

        var options = new PeakSearchingAndFittingOptions
        {
          Preprocessing = preprocessing,
          PeakSearching = peakSearching,
          PeakFitting = peakFitting,
          OutputOptions = outputOptions,
        };
        return new PeakSearchingAndFittingOptionsDocNode(options, proxyList);
      }
    }

    /// <summary>
    /// 2023-03-30 A list of proxies now can be serialized.
    /// 2024-04-05 V3: PeakSearchingAndFittingOptions is now contained in _optionsObject
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingAndFittingOptionsDocNode), 3)]
    public class SerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingAndFittingOptionsDocNode)obj;
        var preprocessingOptions = s.InternalGetSpectralPreprocessingOptions();
        s.InternalSpectralPreprocessingOptions = preprocessingOptions;
        info.AddValue("Options", s._optionsObject);
        SpectralPreprocessingOptionsDocNodeBase.SerializeProxiesVersion1(info, s, preprocessingOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var options = info.GetValue<PeakSearchingAndFittingOptions>("Options", null);
        var proxyList = SpectralPreprocessingOptionsDocNodeBase.DeserializeProxiesVersion1(info);
        return new PeakSearchingAndFittingOptionsDocNode(options, proxyList);
      }
    }

    #endregion

    protected PeakSearchingAndFittingOptionsDocNode(PeakSearchingAndFittingOptions options, List<(int number, IDocumentLeafNode proxy)> proxyList)
      : base(options, proxyList)
    {
    }


    public PeakSearchingAndFittingOptionsDocNode(PeakSearchingAndFittingOptions options) : base(options)
    {
    }

    /// <summary>
    /// Gets the wrapped spectral preprocessing options. When neccessary, the calibration is updated to reflect the content of the linked calibration table.
    /// </summary>
    /// <returns>The wrapped spectral preprocessing options</returns>
    public PeakSearchingAndFittingOptions GetPeakSearchingAndFittingOptions()
    {
      InternalSpectralPreprocessingOptions = InternalGetSpectralPreprocessingOptions();
      return (PeakSearchingAndFittingOptions)_optionsObject;
    }

    protected override SpectralPreprocessingOptionsBase InternalSpectralPreprocessingOptions
    {
      get
      {
        return ((PeakSearchingAndFittingOptions)_optionsObject).Preprocessing;
      }
      set
      {
        _optionsObject = ((PeakSearchingAndFittingOptions)_optionsObject) with { Preprocessing = value };
      }
    }
  }
}
