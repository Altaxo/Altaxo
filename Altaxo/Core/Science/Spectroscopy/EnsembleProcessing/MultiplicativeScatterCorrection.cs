using System.Collections.Generic;
using System.Xml;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression;

namespace Altaxo.Science.Spectroscopy.EnsembleProcessing
{
  /// <summary>
  /// This class processes the spectra for the influence of multiplicative scattering.
  /// </summary>
  public record MultiplicativeScatterCorrection : IEnsembleMeanScalePreprocessor, Main.IImmutable
  {
    /// <summary>
    /// Gets a value indicating whether the ensemble scale (for each spectral slot) should be calculated.
    /// </summary>
    public bool EnsembleScale { get; init; }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.EnsembleMeanScale.MultiplicativeScatterCorrection", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MultiplicativeScatterCorrection), 1)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MultiplicativeScatterCorrection)obj;
        info.AddValue("EnsembleScale", s.EnsembleScale);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        bool ensembleScale = info.GetBoolean("EnsembleScale");
        return new MultiplicativeScatterCorrection() { EnsembleScale = ensembleScale };
      }
    }
    #endregion

    /// <inheritdoc/>
    public void Process(IMatrix<double> xMatrix, int[] regions, IVector<double> xMean, IVector<double> xScale)
    {
      // Note: We have a slight deviation from the literature:
      // we repeat the multiple scattering correction until the xMean vector is self-consistent.
      // In detail: after each MSC correction, we calculate the new xMean and compare it with the xMean
      // of the step before. We repeat until the deviation of xMean relative to xMeanBefore is
      // reasonably small.
      // The reason for this deviation is that we don't want to store two separate xMean vectors: one used
      // for MSC (the x in linear regression) and another to center the MSC-corrected spectra.

      var xMatrixOrg = MatrixMath.DenseOfMatrix(xMatrix);


      VectorMath.FillWith(xMean, 0);

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
          {
            sum += xMatrix[i, n];
          }
          xMean[n] += sum / rows; // xMean is in the first cycle 0 before, thus becomes mean. In later cycles it is incrementally updated.
        }

        // 2.) Process the spectra
        foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, xMatrix.ColumnCount))
        {
          ProcessForPrediction(xMatrixOrg, xMean, start, end, xMatrix);
        }

        // 3. Compare the xMean with the xMeanBefore
        if (xMeanBefore is null)
        {
          xMeanBefore = VectorMath.CreateExtensibleVector<double>(xMean.Count);
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
        // because by the previous MSC correction, the mean is already zero, we only need the scale here
        MatrixMath.ColumnsToZeroMeanAndUnitVariance(xMatrix, null, xScale);
      }
    }

    /// <inheritdoc/>
    public void ProcessForPrediction(IMatrix<double> xMatrix, int[] regions, IReadOnlyList<double> xMean, IReadOnlyList<double> xScale)
    {
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, xMatrix.ColumnCount))
      {
        ProcessForPrediction(xMatrix, xMean, start, end, xMatrix);
      }

      if (EnsembleScale)
      {
        MatrixMath.MultiplyRow(xMatrix, xScale, xMatrix);
      }
    }

    /// <summary>
    /// Processes the spectra in <paramref name="xMatrix"/>.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">The ensemble mean of the spectra.</param>
    /// <param name="regionstart">Starting index of the region to process.</param>
    /// <param name="regionend">End index of the region to process (exclusive).</param>
    /// <param name="resultMatrix">The resulting matrix.</param>
    private void ProcessForPrediction(IROMatrix<double> xMatrix, IReadOnlyList<double> xMean, int regionstart, int regionend, IMatrix<double> resultMatrix)
    {
      for (int n = 0; n < xMatrix.RowCount; n++)
      {
        // 2.) Do linear regression of the current spectrum versus the mean spectrum
        var regression = new QuickLinearRegression();
        for (int i = regionstart; i < regionend; i++)
        {
          regression.Add(xMean[i], xMatrix[n, i]);
        }

        double intercept = regression.GetA0();
        double slope = regression.GetA1();

        // 3.) Subtract intercept and divide by slope
        for (int i = regionstart; i < regionend; i++)
        {
          resultMatrix[n, i] = xMatrix[n, i] - (intercept + slope * xMean[i]);
        }
      }
    }

    /// <summary>
    /// Exports the processing to an XML node.
    /// </summary>
    /// <param name="writer">The writer to export to.</param>
    public void Export(XmlWriter writer)
    {
      writer.WriteElementString("MultiplicativeScatterCorrection", string.Empty);
    }
  }
}
