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

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Process options for multivariate analyses that feature a dimension reduction.
  /// </summary>
  public record DimensionReductionOutputOptions : Main.IImmutable
  {

    public bool IncludeEnsemblePreprocessingAuxiliaryData { get; init; } = true;

    public bool IncludePreprocessedSpectra { get; init; } = false;



    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>V0: 2026-01-14.</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionOutputOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionOutputOptions)obj;
        info.AddValue("IncludeAuxiliaryData", s.IncludeEnsemblePreprocessingAuxiliaryData);
        info.AddValue("IncludePreprocessedSpectra", s.IncludePreprocessedSpectra);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        bool singlePreprocessing = info.GetBoolean("IncludePreprocessedSpectra");
        bool ensemblePreprocessing = info.GetBoolean("IncludeAuxiliaryData");

        return new DimensionReductionOutputOptions()
        {
          IncludePreprocessedSpectra = singlePreprocessing,
          IncludeEnsemblePreprocessingAuxiliaryData = ensemblePreprocessing
        };
      }
    }
    #endregion
  }
}
