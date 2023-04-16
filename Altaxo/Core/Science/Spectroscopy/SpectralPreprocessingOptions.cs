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
using System.Collections.Immutable;
using System.Linq;
using Altaxo.Science.Spectroscopy.BaselineEstimation;
using Altaxo.Science.Spectroscopy.Calibration;
using Altaxo.Science.Spectroscopy.Cropping;
using Altaxo.Science.Spectroscopy.DarkSubtraction;
using Altaxo.Science.Spectroscopy.Normalization;
using Altaxo.Science.Spectroscopy.Resampling;
using Altaxo.Science.Spectroscopy.Sanitizing;
using Altaxo.Science.Spectroscopy.Smoothing;
using Altaxo.Science.Spectroscopy.SpikeRemoval;

namespace Altaxo.Science.Spectroscopy
{

  public record SpectralPreprocessingOptions : SpectralPreprocessingOptionsBase
  {
    private const int IndexSanitzer = 0;
    private const int IndexDarkSubtraction = 1;
    private const int IndexSpikeRemoval = 2;
    private const int IndexYCalibration = 3;
    private const int IndexXCalibration = 4;
    private const int IndexResampling = 5;
    private const int IndexSmoothing = 6;
    private const int IndexBaselineEstimation = 7;
    private const int IndexCropping = 8;
    private const int IndexNormalization = 9;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2022-06-09 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.SpectralPreprocessingOptions", 0)]
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

        return new SpectralPreprocessingOptions(new ISingleSpectrumPreprocessor[]
        {
          sanitizer,
          new DarkSubtractionNone(),
          spikeRemoval,
          new YCalibrationNone(),
          new XCalibrationNone(),
          resampling,
          smoothing,
          baselineEstimation,
          cropping,
          normalization,
        });
      }
    }
    #endregion

    #region Version 1

    /// <summary>
    /// 2022-06-09 V0: Initial version
    /// 2022-08-04 V1: Added Calibration element
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpectralPreprocessingOptions), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpectralPreprocessingOptions)obj;
        info.AddValue("Sanitizer", s.Sanitizer);
        info.AddValue("SpikeRemoval", s.SpikeRemoval);
        info.AddValue("Calibration", s.XCalibration);
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
        var xcalibration = info.GetValue<IXCalibration>("Calibration", parent);
        var resampling = info.GetValue<IResampling>("Resampling", parent);
        var smoothing = info.GetValue<ISmoothing>("Smoothing", parent);
        var baselineEstimation = info.GetValue<IBaselineEstimation>("BaselineEstimation", parent);
        var cropping = info.GetValue<ICropping>("Cropping", parent);
        var normalization = info.GetValue<INormalization>("Normalization", parent);

        return new SpectralPreprocessingOptions(new ISingleSpectrumPreprocessor[]
        {
          sanitizer,
          new DarkSubtractionNone(),
          spikeRemoval,
          new YCalibrationNone(),
          xcalibration,
          resampling,
          smoothing,
          baselineEstimation,
          cropping,
          normalization,
        });
      }
    }

    /// <summary>
    /// 2022-06-09 V0: Initial version
    /// 2022-08-04 V1: Added Calibration element
    /// 2023-03-30 V2: Added DarkSubtraction and YCalibration, rename Calibration to XCalibration
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpectralPreprocessingOptions), 2)]
    public class SerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpectralPreprocessingOptions)obj;
        info.AddValue("Sanitizer", s.Sanitizer);
        info.AddValue("DarkSubtraction", s.DarkSubtraction);
        info.AddValue("SpikeRemoval", s.SpikeRemoval);
        info.AddValue("YCalibration", s.YCalibration);
        info.AddValue("XCalibration", s.XCalibration);
        info.AddValue("Resampling", s.Resampling);
        info.AddValue("Smoothing", s.Smoothing);
        info.AddValue("BaselineEstimation", s.BaselineEstimation);
        info.AddValue("Cropping", s.Cropping);
        info.AddValue("Normalization", s.Normalization);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var sanitizer = info.GetValue<ISanitizer>("Sanitizer", parent);
        var darkSubtraction = info.GetValue<IDarkSubtraction>("DarkSubtraction", parent);
        var spikeRemoval = info.GetValue<ISpikeRemoval>("SpikeRemoval", parent);
        var ycalibration = info.GetValue<IYCalibration>("YCalibration", parent);
        var xcalibration = info.GetValue<IXCalibration>("XCalibration", parent);
        var resampling = info.GetValue<IResampling>("Resampling", parent);
        var smoothing = info.GetValue<ISmoothing>("Smoothing", parent);
        var baselineEstimation = info.GetValue<IBaselineEstimation>("BaselineEstimation", parent);
        var cropping = info.GetValue<ICropping>("Cropping", parent);
        var normalization = info.GetValue<INormalization>("Normalization", parent);


        return new SpectralPreprocessingOptions(new ISingleSpectrumPreprocessor[]
        {
          sanitizer,
          darkSubtraction,
          spikeRemoval,
          ycalibration,
          xcalibration,
          resampling,
          smoothing,
          baselineEstimation,
          cropping,
          normalization,
        });
      }
    }
    #endregion

    #endregion

    protected IEnumerable<Type> ExpectedOrderOfElements
    {
      get
      {
        yield return typeof(ISanitizer);
        yield return typeof(IDarkSubtraction);
        yield return typeof(ISpikeRemoval);
        yield return typeof(IYCalibration);
        yield return typeof(IXCalibration);
        yield return typeof(IResampling);
        yield return typeof(ISmoothing);
        yield return typeof(IBaselineEstimation);
        yield return typeof(ICropping);
        yield return typeof(INormalization);
      }
    }

    public SpectralPreprocessingOptions()
    {
      InnerList = ImmutableList.Create<ISingleSpectrumPreprocessor>(
        new SanitizerNone(),
        new DarkSubtractionNone(),
        new SpikeRemovalNone(),
        new YCalibrationNone(),
        new XCalibrationNone(),
        new ResamplingNone(),
        new SmoothingNone(),
        new BaselineEstimationNone(),
        new CroppingNone(),
        new NormalizationNone()
        ); ;
    }

    public SpectralPreprocessingOptions(IEnumerable<ISingleSpectrumPreprocessor> list)
    {
      var ilist = list.ToImmutableList();
      int idx = 0;
      foreach (var expectedType in ExpectedOrderOfElements)
      {
        if (idx >= ilist.Count)
          throw new ArgumentException("List has too less elements", nameof(list));
        if (ilist[idx] is null)
          throw new ArgumentException($"List element [{idx}] is null", nameof(list));
        if (!(expectedType.IsAssignableFrom(ilist[idx].GetType())))
          throw new ArgumentException($"In list element[{idx}, the type {expectedType} is expected, but it is {ilist[idx].GetType()}");
        ++idx;
      }
      if (ilist.Count != idx)
        throw new ArgumentException("List has too many elements", nameof(list));

      InnerList = ilist;
    }

    public ISanitizer Sanitizer
    {
      get
      {
        return (ISanitizer)InnerList[IndexSanitzer];
      }
      init
      {

        InnerList = InnerList.SetItem(IndexSanitzer, value ?? throw new ArgumentNullException(nameof(Sanitizer)));
      }
    }

    public IDarkSubtraction DarkSubtraction
    {
      get
      {
        return (IDarkSubtraction)InnerList[IndexDarkSubtraction];
      }
      init
      {

        InnerList = InnerList.SetItem(IndexDarkSubtraction, value ?? throw new ArgumentNullException(nameof(DarkSubtraction)));
      }
    }


    public ISpikeRemoval SpikeRemoval
    {
      get
      {
        return (ISpikeRemoval)InnerList[IndexSpikeRemoval];
      }
      init
      {
        InnerList = InnerList.SetItem(IndexSpikeRemoval, value ?? throw new ArgumentNullException(nameof(SpikeRemoval)));
      }
    }

    public IYCalibration YCalibration
    {
      get
      {
        return (IYCalibration)InnerList[IndexYCalibration];
      }
      init
      {
        InnerList = InnerList.SetItem(IndexYCalibration, value ?? throw new ArgumentNullException(nameof(YCalibration)));
      }
    }


    public IXCalibration XCalibration
    {
      get
      {
        return (IXCalibration)InnerList[IndexXCalibration];
      }
      init
      {
        InnerList = InnerList.SetItem(IndexXCalibration, value ?? throw new ArgumentNullException(nameof(XCalibration)));
      }
    }

    public IResampling Resampling
    {
      get
      {
        return (IResampling)InnerList[IndexResampling];
      }
      init
      {
        InnerList = InnerList.SetItem(IndexResampling, value ?? throw new ArgumentNullException(nameof(Resampling)));
      }
    }

    public ISmoothing Smoothing
    {
      get
      {
        return (ISmoothing)InnerList[IndexSmoothing];
      }
      init
      {
        InnerList = InnerList.SetItem(IndexSmoothing, value ?? throw new ArgumentNullException(nameof(Smoothing)));
      }
    }

    public IBaselineEstimation BaselineEstimation
    {
      get
      {
        return (IBaselineEstimation)InnerList[IndexBaselineEstimation];
      }
      init
      {
        InnerList = InnerList.SetItem(IndexBaselineEstimation, value ?? throw new ArgumentNullException(nameof(BaselineEstimation)));
      }
    }

    public ICropping Cropping
    {
      get
      {
        return (ICropping)InnerList[IndexCropping];
      }
      init
      {
        InnerList = InnerList.SetItem(IndexCropping, value ?? throw new ArgumentNullException(nameof(Cropping)));
      }
    }

    public INormalization Normalization
    {
      get
      {
        return (INormalization)InnerList[IndexNormalization];
      }
      init
      {
        InnerList = InnerList.SetItem(IndexNormalization, value ?? throw new ArgumentNullException(nameof(Normalization)));
      }
    }

    public static SpectralPreprocessingOptions? TryCreateFrom(SpectralPreprocessingOptionsBase options)
    {
      if (options is null)
        throw new ArgumentNullException(nameof(options));

      if (options is SpectralPreprocessingOptions s)
      {
        return s;
      }
      else if (options is SpectralPreprocessingOptionsList l)
      {
        var standardOptions = new SpectralPreprocessingOptions();
        var dstElements = standardOptions.ToArray();
        var dstTypes = standardOptions.ExpectedOrderOfElements.ToArray();
        var sourceElements = options.Where(e => e is not null && !e.GetType().Name.Contains("None")).ToList();
        int idxDst = 0;
        for (int idxSrc = 0; idxSrc < sourceElements.Count; ++idxSrc)
        {
          bool wasAssigned = false;
          for (; idxDst < dstElements.Length; ++idxDst)
          {
            if (dstTypes[idxDst].IsAssignableFrom(options[idxSrc].GetType()))
            {
              dstElements[idxDst] = options[idxSrc];
              ++idxDst;
              wasAssigned = true;
              break;
            }
          }
          if (!wasAssigned)
          {
            return null; // conversion failed
          }
        }
        return new SpectralPreprocessingOptions(dstElements);
      }
      else
      {
        throw new NotImplementedException($"The type to convert ({options?.GetType()}) is not implemented here");
      }
    }
  }
}
