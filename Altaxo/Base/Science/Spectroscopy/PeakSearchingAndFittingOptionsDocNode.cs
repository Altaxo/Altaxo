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

using Altaxo.Data;
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Science.Spectroscopy.PeakSearching;

namespace Altaxo.Science.Spectroscopy
{
  public class PeakSearchingAndFittingOptionsDocNode : SpectralPreprocessingOptionsDocNode
  {

    public IPeakSearching PeakSearching { get; }

    public IPeakFitting PeakFitting { get; }

    private PeakSearchingAndFittingOutputOptions OutputOptions { get; }


    #region Serialization

    /// <summary>
    /// 2022-08-06 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Science.Spectroscopy.PeakSearchingAndFittingOptionsDocNode", 0)]
    public new class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingAndFittingOptionsDocNode)obj;
        info.AddValue("SpectralPreprocessingOptions", s.GetSpectralPreprocessingOptions());
        info.AddValueOrNull("CalibrationTableProxy", s._calibrationTableProxy);
        info.AddValue("PeakSearching", s.PeakSearching);
        info.AddValue("PeakFitting", s.PeakFitting);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var options = info.GetValue<SpectralPreprocessingOptions>("SpectralPreprocessingOptions", null);
        var calibrationTableProxy = info.GetValueOrNull<DataTableProxy>("CalibrationTableProxy", parent);
        var peakSearching = info.GetValue<IPeakSearching>("PeakSearching", null);
        var peakFitting = info.GetValue<IPeakFitting>("PeakFitting", null);
        return new PeakSearchingAndFittingOptionsDocNode(options, calibrationTableProxy, peakSearching, peakFitting, new PeakSearchingAndFittingOutputOptions());
      }
    }

    /// <summary>
    /// 2022-08-06 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingAndFittingOptionsDocNode), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingAndFittingOptionsDocNode)obj;
        info.AddValue("SpectralPreprocessingOptions", s.GetSpectralPreprocessingOptions());
        info.AddValueOrNull("CalibrationTableProxy", s._calibrationTableProxy);
        info.AddValue("PeakSearching", s.PeakSearching);
        info.AddValue("PeakFitting", s.PeakFitting);
        info.AddValue("OutputOptions", s.OutputOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var options = info.GetValue<SpectralPreprocessingOptions>("SpectralPreprocessingOptions", null);
        var calibrationTableProxy = info.GetValueOrNull<DataTableProxy>("CalibrationTableProxy", parent);
        var peakSearching = info.GetValue<IPeakSearching>("PeakSearching", null);
        var peakFitting = info.GetValue<IPeakFitting>("PeakFitting", null);
        var outputOptions = info.GetValue<PeakSearchingAndFittingOutputOptions>("OutputOptions", null);
        return new PeakSearchingAndFittingOptionsDocNode(options, calibrationTableProxy, peakSearching, peakFitting, outputOptions);
      }
    }

    #endregion

    protected PeakSearchingAndFittingOptionsDocNode(SpectralPreprocessingOptions options, DataTableProxy? calibrationTableProxy, IPeakSearching peakSearching, IPeakFitting peakFitting, PeakSearchingAndFittingOutputOptions outputOptions)
      : base(options, calibrationTableProxy)
    {
      PeakSearching = peakSearching;
      PeakFitting = peakFitting;
      OutputOptions = outputOptions;
    }


    public PeakSearchingAndFittingOptionsDocNode(PeakSearchingAndFittingOptions options) : base(options.Preprocessing)
    {
      PeakSearching = options.PeakSearching;
      PeakFitting = options.PeakFitting;
      OutputOptions = options.OutputOptions;
    }

    /// <summary>
    /// Gets the wrapped spectral preprocessing options. When neccessary, the calibration is updated to reflect the content of the linked calibration table.
    /// </summary>
    /// <returns>The wrapped spectral preprocessing options</returns>
    public PeakSearchingAndFittingOptions GetPeakSearchingAndFittingOptions()
    {
      var preprocessing = GetSpectralPreprocessingOptions();

      return new PeakSearchingAndFittingOptions
      {
        Preprocessing = preprocessing,
        PeakSearching = PeakSearching,
        PeakFitting = PeakFitting,
        OutputOptions = OutputOptions,
      };
    }
  }
}
