using System.Collections.Generic;
using System.Xml;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.EnsembleMeanScale
{
  /// <summary>
  /// Corrects the spectral ensembly by subtracting the spectra ensembly mean, and (optional) scales the variance of each
  /// spectral slot to 1.
  /// </summary>
  public record EnsembleMeanAndScaleCorrection : IEnsembleMeanScalePreprocessor, Main.IImmutable
  {
    /// <summary>
    /// Gets a value indicating whether the ensembly scale (for each spectral slot) should be calculated.
    /// </summary>
    public bool EnsembleScale { get; init; }

    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="regions">Vector of spectral regions. Each element is the index of the start of a new region.</param>
    /// <param name="xMean">Not used, since this processing sets xMean by itself (to zero).</param>
    /// <param name="xScale">Not used, since the processing sets xScale by itself.</param>
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

    /// <summary>
    /// Processes the spectra in matrix xMatrix for prediction.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    /// <param name="xMean">Must be supplied, and will be subtracted from all spectra (if option set).</param>
    /// <param name="xScale">Must be supplied, and will be multiplied to all spectra (if option set).</param>
    public void ProcessForPrediction(IMatrix<double> xMatrix, int[] regions, IReadOnlyList<double> xMean, IReadOnlyList<double> xScale)
    {
      MatrixMath.SubtractRow(xMatrix, xMean, xMatrix);
      if (EnsembleScale)
      {
        MatrixMath.MultiplyRow(xMatrix, xScale, xMatrix);
      }
    }

    public void Export(XmlWriter writer)
    {
    }
  }
}
