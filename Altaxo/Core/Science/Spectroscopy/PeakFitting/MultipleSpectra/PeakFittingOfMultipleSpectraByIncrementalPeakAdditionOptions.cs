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

using System.Text;

namespace Altaxo.Science.Spectroscopy.PeakFitting.MultipleSpectra
{
  /// <summary>
  /// Options for peak searching (determination of approximate peak positions) and peak fitting.
  /// </summary>
  public record PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions
  {
    /// <summary>
    /// Gets/sets the methods for spectral preprocessing.
    /// </summary>
    public SpectralPreprocessingOptionsBase Preprocessing { get; init; } = new SpectralPreprocessingOptions();


    /// <summary>
    /// Gets/sets the peak fitting method.
    /// </summary>
    public PeakFittingOfMultipleSpectraByIncrementalPeakAddition PeakFitting { get; init; } = new PeakFittingOfMultipleSpectraByIncrementalPeakAddition();

    /// <summary>
    /// Gets/sets the options that determine which data to include in the output of a peak searching and fitting operation.
    /// </summary>
    public PeakSearchingAndFittingOutputOptions OutputOptions { get; init; } = new();



    #region Serialization
    /// <summary>
    /// 2024-09-11 V0: Initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions)obj;
        info.AddValue("Preprocessing", s.Preprocessing);
        info.AddValue("PeakFitting", s.PeakFitting);
        info.AddValue("OutputOptions", s.OutputOptions);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions?)o ?? new PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions();
        var preprocessing = info.GetValue<SpectralPreprocessingOptionsBase>("Preprocessing", parent);
        var peakFitting = info.GetValue<PeakFittingOfMultipleSpectraByIncrementalPeakAddition>("PeakFitting", parent);
        var outputOptions = info.GetValue<PeakSearchingAndFittingOutputOptions>("OutputOptions", parent);


        return s with
        {
          Preprocessing = preprocessing,
          PeakFitting = peakFitting,
          OutputOptions = outputOptions,
        };
      }
    }

    #endregion

    /// <inheritdoc/>
    public override string ToString()
    {
      var stb = new StringBuilder();
      stb.Append(Preprocessing.ToString());

      stb.Append('[');
      stb.Append(PeakFitting.ToString());
      stb.Append("] ");

      return stb.ToString();
    }
  }
}
