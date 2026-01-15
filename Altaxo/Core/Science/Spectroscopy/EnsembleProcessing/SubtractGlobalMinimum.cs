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

using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.EnsembleProcessing
{
  /// <summary>
  /// Ensemble preprocessor that subtracts the global minimum intensity from all spectra.
  /// </summary>
  public record SubtractGlobalMinimum : IEnsemblePreprocessor
  {
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static readonly SubtractGlobalMinimum Instance = new SubtractGlobalMinimum();

    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SubtractGlobalMinimum), 0)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SubtractGlobalMinimum)obj;
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return SubtractGlobalMinimum.Instance;
      }
    }
    #endregion



    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxiliaryData? auxiliaryData) Execute(double[] x, Matrix<double> y, int[]? regions)
    {
      var min = y.Min();
      return (x, y - min, regions, new EnsembleAuxiliaryDataScalar { Name = "GlobalMinimum", Value = min });
    }

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var min = y.Min();
      var newY = y.Select(v => v - min).ToArray();
      return (x, newY, regions);
    }

    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> spectraMatrix, int[] regions, IEnsembleProcessingAuxiliaryData? auxillaryData)
    {
      if (auxillaryData is not EnsembleAuxiliaryDataScalar data)
      {
        throw new System.ArgumentException("Auxiliary data is not of expected type EnsembleAuxiliaryDataScalar.", nameof(auxillaryData));
      }
      var min = data.Value;
      return (x, spectraMatrix - min, regions);
    }
  }



}
