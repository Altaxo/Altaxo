#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Process options for multivariate analyses that feature a dimension reduction.
  /// </summary>
  public record DimensionReductionOptions : Main.IImmutable
  {
    /// <summary>
    /// Gets the preprocessing applied to each individual spectrum prior to analysis.
    /// </summary>
    public ISingleSpectrumPreprocessor SinglePreprocessing { get; init; } = new NoopSpectrumPreprocessor();

    /// <summary>
    /// Gets the preprocessing applied to the ensemble of spectra (for example mean-centering and scaling).
    /// </summary>
    public IEnsembleMeanScalePreprocessor EnsemblePreprocessing { get; init; } = new Altaxo.Science.Spectroscopy.EnsembleProcessing.EnsembleMeanAndScaleCorrection();

    /// <summary>
    /// Gets the dimension reduction method.
    /// </summary>
    public IDimensionReductionMethod DimensionReductionMethod { get; init; } = new DimensionReductionByLowRankFactorization();



    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-08-01.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionOptions)obj;
        info.AddValue("SinglePreprocessing", s.SinglePreprocessing);
        info.AddValue("EnsemblePreprocessing", s.EnsemblePreprocessing);
        info.AddValue("Method", s.DimensionReductionMethod);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var singlePreprocessing = info.GetValue<ISingleSpectrumPreprocessor>("SinglePreprocessing", null);
        var ensemblePreprocessing = info.GetValue<IEnsembleMeanScalePreprocessor>("EnsemblePreprocessing", null);
        var analysis = info.GetValue<IDimensionReductionMethod>("Method", null);




        return new DimensionReductionOptions()
        {
          SinglePreprocessing = singlePreprocessing,
          EnsemblePreprocessing = ensemblePreprocessing,
          DimensionReductionMethod = analysis,
        };
      }
    }
    #endregion
  }
}
