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

using Altaxo.Main;
using Altaxo.Science.Spectroscopy.BaselineEstimation;
using Altaxo.Science.Spectroscopy.Cropping;
using Altaxo.Science.Spectroscopy.Normalization;
using Altaxo.Science.Spectroscopy.Resampling;
using Altaxo.Science.Spectroscopy.Sanitizing;
using Altaxo.Science.Spectroscopy.Smoothing;
using Altaxo.Science.Spectroscopy.SpikeRemoval;

namespace Altaxo.Science.Spectroscopy
{
  public record SpectralPreprocessingOptions : IImmutable
  {
    #region Serialization

    /// <summary>
    /// 2022-06-09 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpectralPreprocessingOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpectralPreprocessingOptions)obj;
        info.AddValue("Sanitizer", s.Sanitizer);
        info.AddValue("SpikeRemoval", s.SpikeRemoval);
        info.AddValue("Resampling", s.Resampling);
        info.AddValue("Smoothing", s.Smoothing);
        info.AddValue("BaselineEstimation", s.BaselineEstimation);
        info.AddValue("Cropping", s.Cropping);
        info.AddValue("Normalization", s.Normalization);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var sanitizer = info.GetValue<ISanitizer>("Sanitizer", parent);
        var spikeRemoval = info.GetValue<ISpikeRemoval>("SpikeRemoval", parent);
        var resampling = info.GetValue<IResampling>("Resampling", parent);
        var smoothing = info.GetValue<ISmoothing>("Smoothing", parent);
        var baselineEstimation = info.GetValue<IBaselineEstimation>("BaselineEstimation", parent);
        var cropping = info.GetValue<ICropping>("Cropping", parent);
        var normalization = info.GetValue<INormalization>("Normalization", parent);

        return ((SpectralPreprocessingOptions)o ?? new SpectralPreprocessingOptions()) with
        {
          Sanitizer = sanitizer,
          SpikeRemoval = spikeRemoval,
          Resampling = resampling,
          Smoothing = smoothing,
          BaselineEstimation = baselineEstimation,
          Cropping = cropping,
          Normalization = normalization,
        };
      }
    }
    #endregion

    public SpectralPreprocessingOptions()
    {

    }

    public SpectralPreprocessingOptions(SpectralPreprocessingOptions from)
    {
      Sanitizer = from.Sanitizer;
      SpikeRemoval = from.SpikeRemoval;
      Resampling = from.Resampling;
      Smoothing = from.Smoothing;
      BaselineEstimation = from.BaselineEstimation;
      Cropping = from.Cropping;
      Normalization = from.Normalization;
    }

    public ISanitizer Sanitizer { get; init; } = new SanitizerNone();

    public ISpikeRemoval SpikeRemoval { get; init; } = new SpikeRemovalNone();

    public IResampling Resampling { get; init; } = new ResamplingNone();

    public ISmoothing Smoothing { get; init; } = new SmoothingNone();

    public IBaselineEstimation BaselineEstimation { get; init; } = new BaselineEstimationNone();

    public ICropping Cropping { get; init; } = new CroppingNone();

    public INormalization Normalization { get; init; } = new NormalizationNone();
  }
}
