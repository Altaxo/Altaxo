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
using Altaxo.Data;
using Altaxo.Main;
using Altaxo.Science.Spectroscopy.Calibration;
using Altaxo.Serialization.Xml;

namespace Altaxo.Science.Spectroscopy
{
  /// <summary>
  /// Wrapper around <see cref="SpectralPreprocessingOptionsBase"/>
  /// that keeps track of nodes that have references to tables
  /// (currently only XCalibrationByDataSource).
  /// </summary>
  public class SpectralPreprocessingOptionsDocNode : SpectralPreprocessingOptionsDocNodeBase
  {
    /// <summary>
    /// Dictionary that contains the spectral preprocessor as value, and a proxy (DataTableProxy, .. other proxies) as value
    /// </summary>
    protected Dictionary<ISingleSpectrumPreprocessor, IDocumentLeafNode> _proxyCache;


    #region Serialization

    /// <summary>
    /// 2022-08-06 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Science.Spectroscopy.SpectralPreprocessingOptionsDocNode", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
        var s = (SpectralPreprocessingOptionsDocNode)obj;
        info.AddValue("SpectralPreprocessingOptions", s.GetSpectralPreprocessingOptions());
        info.AddValueOrNull("CalibrationTableProxy", s._calibrationTableProxy);
        */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {

        var options = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessingOptions", null);
        var proxyList = DeserializeProxiesVersion0(info, parent, options);
        return new SpectralPreprocessingOptionsDocNode(options, proxyList);
      }

      public static List<(int number, IDocumentLeafNode proxy)> DeserializeProxiesVersion0(IXmlDeserializationInfo info, object? parent, SpectralPreprocessingOptionsBase options)
      {
        var calibrationTableProxy = info.GetValueOrNull<DataTableProxy>("CalibrationTableProxy", parent);
        var proxyList = new List<(int number, IDocumentLeafNode proxy)>(1);
        if (calibrationTableProxy is not null)
        {
          int processorNumber = -1;
          foreach (var processor in options.GetProcessorElements())
          {
            ++processorNumber;
            if (processor is IXCalibration)
            {
              proxyList.Add((processorNumber, calibrationTableProxy));
            }
          }
        }

        return proxyList;
      }
    }

    /// <summary>
    /// 2023-03-29 Extensions to arbitrary processors
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpectralPreprocessingOptionsDocNode), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpectralPreprocessingOptionsDocNode)obj;
        var processingOptions = s.InternalGetSpectralPreprocessingOptions();
        info.AddValue("SpectralPreprocessingOptions", processingOptions);

        SerializeProxiesVersion1(info, s, processingOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {

        var options = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessingOptions", null);
        var proxyList = DeserializeProxiesVersion1(info);

        return new SpectralPreprocessingOptionsDocNode(options, proxyList);
      }

    }

    #endregion

    protected SpectralPreprocessingOptionsDocNode(SpectralPreprocessingOptionsBase options, List<(int number, IDocumentLeafNode proxy)> proxyList)
      : base(options, proxyList)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpectralPreprocessingOptionsDocNode"/> class.
    /// </summary>
    /// <param name="options">The spectral preprocessing options to wrap.</param>
    public SpectralPreprocessingOptionsDocNode(SpectralPreprocessingOptionsBase options)
      : base(options)
    {
    }

    public SpectralPreprocessingOptionsDocNode(SpectralPreprocessingOptionsDocNode from)
      : base(from)
    {
    }

    /// <summary>
    /// Gets the wrapped spectral preprocessing options. When neccessary, the calibration is updated to reflect the content of the linked calibration table.
    /// </summary>
    /// <returns>The wrapped spectral preprocessing options</returns>
    public SpectralPreprocessingOptionsBase GetSpectralPreprocessingOptions()
    {
      return InternalGetSpectralPreprocessingOptions();
    }

    protected override SpectralPreprocessingOptionsBase InternalSpectralPreprocessingOptions
    {
      get => (SpectralPreprocessingOptionsBase)_optionsObject;
      set => _optionsObject = value;
    }
  }
}
