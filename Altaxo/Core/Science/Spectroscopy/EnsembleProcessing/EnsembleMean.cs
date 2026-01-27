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
  /// Ensemble preprocessor that subtracts the ensemble mean from each spectrum.
  /// </summary>
  public record EnsembleMean : IEnsemblePreprocessor
  {
    const string AuxillaryDataName = "EnsembleMean";

    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static readonly EnsembleMean Instance = new EnsembleMean();

    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EnsembleMean), 0)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (EnsembleMean)obj;
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return EnsembleMean.Instance;
      }
    }
    #endregion

    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxiliaryData? auxiliaryData) Execute(double[] x, Matrix<double> y, int[]? regions)
    {
      var yNew = y.Clone();
      var ensembleMean = new double[y.ColumnCount];
      MatrixMath.ColumnsToZeroMean(yNew, ensembleMean.ToVector());
      return (x, yNew, regions, new EnsembleAuxiliaryDataVector { Name = AuxillaryDataName, Value = ensembleMean, VectorType = EnsembleAuxiliaryDataVectorType.Spectrum });
    }

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      return (x, y, regions);
    }

    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> spectraMatrix, int[] regions, IEnsembleProcessingAuxiliaryData? auxillaryData)
    {
      if (auxillaryData is not EnsembleAuxiliaryDataVector data || data.Name != AuxillaryDataName)
      {
        throw new System.ArgumentException("Auxillary data is not of expected type EnsembleAuxillaryDataVector.", nameof(auxillaryData));
      }
      var yNew = spectraMatrix.Clone();
      var ensembleMean = data.Value;
      MatrixMath.SubtractRow(yNew, ensembleMean, yNew);
      return (x, yNew, regions);
    }
  }



}
