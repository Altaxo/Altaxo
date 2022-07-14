using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression;

namespace Altaxo.Science.Spectroscopy.EnsembleMeanScale
{
  /// <summary>
  /// This class processes the spectra for influence of multiplicative scattering.
  /// </summary>
  public record MultiplicativeScatterCorrection : IEnsembleMeanScalePreprocessor, Main.IImmutable
  {
    /// <summary>
    /// Gets a value indicating whether the ensembly scale (for each spectral slot) should be calculated.
    /// </summary>
    public bool EnsembleScale { get; init; }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MultiplicativeScatterCorrection), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MultiplicativeScatterCorrection)obj;
        info.AddValue("EnsembleScale", s.EnsembleScale);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        bool ensembleScale = info.GetBoolean("EnsembleScale");
        return new MultiplicativeScatterCorrection() { EnsembleScale = ensembleScale };
      }
    }
    #endregion


    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    /// <param name="xMean">Output: On return, contains the ensemble mean of the spectra.</param>
    /// <param name="xScale">Not used.</param>
    public void Process(IMatrix<double> xMatrix, int[] regions, IVector<double> xMean, IVector<double> xScale)
    {
      // note: we have a light deviation here to the literature:
      // we repeat the multiple scattering correction until the xMean vector is self consistent,
      // in detail: after each MSC correction, we calculate the new xMean and compare with the xMean
      // of the step before. We repeat until the deviation of the xMean to the xMean_before is
      // reasonable small.
      // The reason for this deviation is that we don't want to store two separate xMean vectors: one used
      // for MSC (the x in linear regression) and another to center the MSC corrected spectra

      IVector<double>? xMeanBefore = null;
      double threshold = 1E-14 * MatrixMath.SumOfSquares(xMatrix) / xMatrix.RowCount;
      for (int cycle = 0; cycle < 50; cycle++)
      {
        // 1.) Get the mean spectrum
        // we want to have the mean of each matrix column, but not center the matrix now, since this
        // is done later on
        int cols = xMatrix.ColumnCount;
        int rows = xMatrix.RowCount;
        for (int n = 0; n < cols; n++)
        {
          double sum = 0;
          for (int i = 0; i < rows; i++)
            sum += xMatrix[i, n];
          xMean[n] = sum / rows;
        }

        // 2.) Process the spectras
        ProcessForPrediction(xMatrix, regions, xMean, xScale);

        // 3. Compare the xMean with the xMean_before
        if (xMeanBefore is null)
        {
          xMeanBefore = VectorMath.CreateExtensibleVector<double>(xMean.Length);
          VectorMath.Copy(xMean, xMeanBefore);
        }
        else
        {
          double sumdiffsquare = VectorMath.SumOfSquaredDifferences(xMean, xMeanBefore);
          if (sumdiffsquare < threshold)
            break;
          else
            VectorMath.Copy(xMean, xMeanBefore);
        }
      }

      if (EnsembleScale)
      {
        // because by the previous MSC-Correction, the mean is already zero, we only need the scale here
        MatrixMath.ColumnsToZeroMeanAndUnitVariance(xMatrix, null, xScale);
      }
    }

    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Output: On return, contains the ensemble mean of the spectra.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public void ProcessForPrediction(IMatrix<double> xMatrix, int[] regions, IReadOnlyList<double> xMean, IReadOnlyList<double> xScale)
    {
      foreach(var (start, end) in RegionHelper.GetRegionRanges(regions, xMatrix.ColumnCount))
      { 
        ProcessForPrediction(xMatrix, xMean, xScale,start, end);
      }

      if (EnsembleScale)
      {
        MatrixMath.MultiplyRow(xMatrix, xScale, xMatrix);
      }
    }

    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Output: On return, contains the ensemble mean of the spectra.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regionstart">Starting index of the region to process.</param>
    /// <param name="regionend">End index of the region to process (exclusive).</param>
    private void ProcessForPrediction(IMatrix<double> xMatrix, IReadOnlyList<double> xMean, IReadOnlyList<double> xScale, int regionstart, int regionend)
    {
      int regionlength = regionend - regionstart;

      for (int n = 0; n < xMatrix.RowCount; n++)
      {
        // 2.) Do linear regression of the current spectrum versus the mean spectrum
        var regression = new QuickLinearRegression();
        for (int i = regionstart; i < regionend; i++)
          regression.Add(xMean[i], xMatrix[n, i]);

        double intercept = regression.GetA0();
        double slope = regression.GetA1();

        // 3.) Subtract intercept and divide by slope
        for (int i = regionstart; i < regionend; i++)
          xMatrix[n, i] = (xMatrix[n, i] - intercept) / slope;
      }
    }

    /// <summary>
    /// Exports the processing to an xml node.
    /// </summary>
    /// <param name="writer">The writer to export to</param>
    public void Export(XmlWriter writer)
    {
      writer.WriteElementString("MultiplicativeScatterCorrection", string.Empty);
    }
  }
}
