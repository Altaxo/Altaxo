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
  public record PeakSearchingAndFittingOptions 
  {
   public SpectralPreprocessingOptions Preprocessing { get; init; } = new SpectralPreprocessingOptions();

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
        var s = (PeakSearchingAndFittingOptions)o ?? new PeakSearchingAndFittingOptions();

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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingAndFittingOptions), 1)]
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
        var s = (PeakSearchingAndFittingOptions)o ?? new PeakSearchingAndFittingOptions();
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


    #endregion

    public PeakSearchingAndFittingOptions()
    {
    }

    public PeakSearchingAndFittingOptions(PeakSearchingAndFittingOptions from)
    {
      Preprocessing = from.Preprocessing;
      PeakSearching = from.PeakSearching;
      PeakFitting = from.PeakFitting;
    }

    public IPeakSearching PeakSearching { get; init; } = new PeakSearchingNone();

    public IPeakFitting PeakFitting { get; init; } = new PeakFittingNone();


   
  }
}
