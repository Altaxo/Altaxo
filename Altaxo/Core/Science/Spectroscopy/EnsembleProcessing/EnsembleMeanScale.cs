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
  /// Ensemble preprocessor that mean-centers and scales each variable (spectral point) to unit variance.
  /// </summary>
  public record EnsembleMeanScale : IEnsemblePreprocessor
  {
    public const string AuxiliaryDataName = "EnsembleMeanScale";
    public const string AuxiliaryDataMeanName = "EnsembleMean";
    public const string AuxiliaryDataScaleName = "EnsembleScale";

    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static readonly EnsembleMeanScale Instance = new EnsembleMeanScale();

    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EnsembleMeanScale), 0)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (EnsembleMeanScale)obj;
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return EnsembleMeanScale.Instance;
      }
    }
    #endregion

    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxiliaryData? auxiliaryData) Execute(double[] x, Matrix<double> y, int[]? regions)
    {
      var ensembleMean = new double[y.ColumnCount];
      var ensembleScale = new double[y.ColumnCount];
      var yNew = y.Clone();
      MatrixMath.ColumnsToZeroMeanAndUnitVariance(yNew, ensembleMean.ToVector(), ensembleScale.ToVector());
      return (x, yNew, regions, new EnsembleAuxiliaryDataCompound
      {
        Name = AuxiliaryDataName,
        Values = [
          new EnsembleAuxiliaryDataVector { Name = AuxiliaryDataMeanName, Value = ensembleMean, VectorType = EnsembleAuxiliaryDataVectorType.Spectrum },
          new EnsembleAuxiliaryDataVector { Name = AuxiliaryDataScaleName, Value = ensembleScale, VectorType = EnsembleAuxiliaryDataVectorType.Spectrum }
          ]
      });
    }

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      return (x, y, regions);
    }

    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> spectraMatrix, int[] regions, IEnsembleProcessingAuxiliaryData? auxillaryData)
    {
      if (auxillaryData is not EnsembleAuxiliaryDataCompound data || data.Name != AuxiliaryDataName)
      {
        throw new System.ArgumentException("Auxillary data is not of expected type EnsembleAuxiliaryDataCompound.", nameof(auxillaryData));
      }
      if (data.Values.Length != 2)
      {
        throw new System.ArgumentException("Auxillary data does not contain two elements.", nameof(auxillaryData));
      }
      if (data.Values[0] is not EnsembleAuxiliaryDataVector aux0 || aux0.Name != AuxiliaryDataMeanName)
      {
        throw new System.ArgumentException($"Auxillary data does not contain {AuxiliaryDataMeanName}.", nameof(auxillaryData));
      }
      if (data.Values[1] is not EnsembleAuxiliaryDataVector aux1 || aux1.Name != AuxiliaryDataScaleName)
      {
        throw new System.ArgumentException($"Auxillary data does not contain {AuxiliaryDataScaleName}.", nameof(auxillaryData));
      }
      var xMean = aux0.Value;
      var xScale = aux1.Value;

      var yNew = spectraMatrix.Clone();
      MatrixMath.SubtractRow(spectraMatrix, xMean, yNew);
      MatrixMath.MultiplyRow(yNew, xScale, yNew);
      return (x, yNew, regions);
    }
  }



}
