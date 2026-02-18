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
    /// Gets the preprocessing applied to each individual spectrum or to the entire ensemble prior to analysis.
    /// </summary>
    public ISingleSpectrumPreprocessor Preprocessing { get; init; } = new EnsembleMeanScale();

    /// <summary>
    /// Gets the dimension reduction method.
    /// </summary>
    public IDimensionReductionMethod DimensionReductionMethod { get; init; } = new DimensionReductionByLowRankFactorization();

    /// <summary>
    /// Gets the output options to determine what results are to be stored in the destination table.
    /// </summary>
    public DimensionReductionOutputOptions OutputOptions { get; init; } = new DimensionReductionOutputOptions();

    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-14.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionOptions)obj;
        info.AddValue("SinglePreprocessing", s.Preprocessing);
        info.AddValue("Method", s.DimensionReductionMethod);
        info.AddValue("Output", s.OutputOptions);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var singlePreprocessing = info.GetValue<ISingleSpectrumPreprocessor>("SinglePreprocessing", null);
        var analysis = info.GetValue<IDimensionReductionMethod>("Method", null);
        var output = info.GetValue<DimensionReductionOutputOptions>("Output", parent);



        return new DimensionReductionOptions()
        {
          Preprocessing = singlePreprocessing,
          DimensionReductionMethod = analysis,
          OutputOptions = output,
        };
      }
    }
    #endregion
  }


}
