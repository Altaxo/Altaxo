using System.Collections.Generic;
using System.Xml;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;

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

    /// <summary>
    /// Serializes <see cref="EnsembleMeanAndScaleCorrection"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EnsembleMeanAndScaleCorrection), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (EnsembleMeanAndScaleCorrection)o;
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
    public void Process(IMatrix<double> spectraMatrix, int[] regions, IVector<double> spectraMean, IVector<double> spectraScale)
    {
      if (EnsembleScale)
      {
        MatrixMath.ColumnsToZeroMeanAndUnitVariance(spectraMatrix, spectraMean, spectraScale);
      }
      else
      {
        MatrixMath.ColumnsToZeroMean(spectraMatrix, spectraMean);
        for (int i = 0; i < spectraMatrix.ColumnCount; ++i)
        {
          spectraScale[i] = 1;
        }
      }
    }

    /// <inheritdoc/>
    public void ProcessForPrediction(IMatrix<double> spectraMatrix, int[] regions, IReadOnlyList<double> spectraMean, IReadOnlyList<double> spectraScale)
    {
      MatrixMath.SubtractRow(spectraMatrix, spectraMean, spectraMatrix);
      if (EnsembleScale)
      {
        MatrixMath.MultiplyRow(spectraMatrix, spectraScale, spectraMatrix);
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
