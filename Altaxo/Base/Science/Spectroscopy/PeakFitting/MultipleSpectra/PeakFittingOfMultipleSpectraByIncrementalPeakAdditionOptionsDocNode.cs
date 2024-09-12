#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using Altaxo.Main;

namespace Altaxo.Science.Spectroscopy.PeakFitting.MultipleSpectra
{
  public class PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptionsDocNode : SpectralPreprocessingOptionsDocNodeBase
  {

    #region Serialization

    /// <summary>
    /// 2024-09-11 V0: Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptionsDocNode), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptionsDocNode)obj;
        var preprocessingOptions = s.InternalGetSpectralPreprocessingOptions();
        s.InternalSpectralPreprocessingOptions = preprocessingOptions;
        info.AddValue("Options", s._optionsObject);
        SpectralPreprocessingOptionsDocNodeBase.SerializeProxiesVersion1(info, s, preprocessingOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var options = info.GetValue<PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions>("Options", null);
        var proxyList = SpectralPreprocessingOptionsDocNodeBase.DeserializeProxiesVersion1(info);
        return new PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptionsDocNode(options, proxyList);
      }
    }

    #endregion

    protected PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptionsDocNode(PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions options, List<(int number, IDocumentLeafNode proxy)> proxyList)
      : base(options, proxyList)
    {
    }


    public PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptionsDocNode(PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions options) : base(options)
    {
    }

    /// <summary>
    /// Gets the wrapped spectral preprocessing options. When neccessary, the calibration is updated to reflect the content of the linked calibration table.
    /// </summary>
    /// <returns>The wrapped spectral preprocessing options</returns>
    public PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions GetPeakSearchingAndFittingOptions()
    {
      InternalSpectralPreprocessingOptions = InternalGetSpectralPreprocessingOptions();
      return (PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions)_optionsObject;
    }

    protected override SpectralPreprocessingOptionsBase InternalSpectralPreprocessingOptions
    {
      get
      {
        return ((PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions)_optionsObject).Preprocessing;
      }
      set
      {
        _optionsObject = ((PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions)_optionsObject) with { Preprocessing = value };
      }
    }
  }
}
