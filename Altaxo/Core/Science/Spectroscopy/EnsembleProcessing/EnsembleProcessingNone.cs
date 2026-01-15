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

using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.EnsembleProcessing
{
  /// <summary>
  /// Ensemble preprocessor that performs no processing.
  /// </summary>
  public record EnsembleProcessingNone : IEnsemblePreprocessor
  {
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static readonly EnsembleProcessingNone Instance = new EnsembleProcessingNone();

    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EnsembleProcessingNone), 0)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (EnsembleProcessingNone)obj;
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return EnsembleProcessingNone.Instance;
      }
    }
    #endregion


    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxiliaryData? auxiliaryData) Execute(double[] x, Matrix<double> y, int[]? regions)
    {
      return (x, y, regions, null);
    }

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      return (x, y, regions);
    }

    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> y, int[] regions, IEnsembleProcessingAuxiliaryData? auxillaryData)
    {
      return (x, y, regions);
    }
  }



}
