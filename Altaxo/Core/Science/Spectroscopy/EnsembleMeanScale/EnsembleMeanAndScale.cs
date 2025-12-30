using System.Collections.Generic;
using System.Xml;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.EnsembleMeanScale
{
  /// <summary>
  /// Corrects the spectral ensemble by subtracting the spectra ensemble mean, and (optionally) scales the variance of each
  /// spectral slot to 1.
  /// </summary>
  public record EnsembleMeanAndScaleCorrection : IEnsembleMeanScalePreprocessor, Main.IImmutable
  {
    /// <summary>
    /// Gets a value indicating whether the ensemble scale (for each spectral slot) should be calculated.
    /// </summary>
    public bool EnsembleScale { get; init; }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EnsembleMeanAndScaleCorrection), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (EnsembleMeanAndScaleCorrection)obj;
        info.AddValue("EnsembleScale", s.EnsembleScale);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        bool ensembleScale = info.GetBoolean("EnsembleScale");
        return new EnsembleMeanAndScaleCorrection() { EnsembleScale = ensembleScale };
      }
    }
    #endregion


    /// <inheritdoc/>
    public void Process(IMatrix<double> xMatrix, int[] regions, IVector<double> xMean, IVector<double> xScale)
    {
      if (EnsembleScale)
      {
        MatrixMath.ColumnsToZeroMeanAndUnitVariance(xMatrix, xMean, xScale);
      }
      else
      {
        MatrixMath.ColumnsToZeroMean(xMatrix, xMean);
        for (int i = 0; i < xMatrix.ColumnCount; ++i)
        {
          xScale[i] = 1;
        }
      }
    }

    /// <inheritdoc/>
    public void ProcessForPrediction(IMatrix<double> xMatrix, int[] regions, IReadOnlyList<double> xMean, IReadOnlyList<double> xScale)
    {
      MatrixMath.SubtractRow(xMatrix, xMean, xMatrix);
      if (EnsembleScale)
      {
        MatrixMath.MultiplyRow(xMatrix, xScale, xMatrix);
      }
    }

    /// <summary>
    /// Exports the processing to an XML node.
    /// </summary>
    /// <param name="writer">The writer to export to.</param>
    public void Export(XmlWriter writer)
    {
    }
  }
}
