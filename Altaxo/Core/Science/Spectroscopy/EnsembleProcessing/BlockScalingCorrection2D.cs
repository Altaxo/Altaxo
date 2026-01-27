using System;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Science.Signals;

namespace Altaxo.Science.Spectroscopy.EnsembleProcessing
{
  /// <summary>
  /// Represents a 2D block scaling correction preprocessor for spectroscopic ensemble data.
  /// This preprocessor scales individual spectra within defined blocks by their mean intensity
  /// over a specified spectral range.
  /// </summary>
  public record BlockScalingCorrection2D : IEnsemblePreprocessor
  {
    /// <summary>
    /// Gets or initializes the size of the first dimension of the block structure.
    /// </summary>
    public int SizeOfDimension0 { get; init; } = 1;

    /// <summary>
    /// Gets or initializes the optional size of the second dimension of the block structure.
    /// If not set, it is inferred from the total number of rows and <see cref="SizeOfDimension0"/>.
    /// </summary>
    public int? SizeOfDimension1 { get; init; }

    /// <summary>
    /// Gets or initializes the index of the dimension along which the averaging is performed (0 or 1).
    /// Used to determine the number of blocks and spectra per block.
    /// </summary>
    public int IndexOfDimensionToAverage { get; init; } = 0;

    /// <summary>
    /// Gets or initializes the minimum X-value for the range over which the mean is calculated.
    /// </summary>
    public double MinimumX { get; init; } = double.NegativeInfinity;

    /// <summary>
    /// Gets or initializes the maximum X-value for the range over which the mean is calculated.
    /// </summary>
    public double MaximumX { get; init; } = double.PositiveInfinity;

    /// <summary>
    /// Gets or initializes a value indicating whether the X-range values (<see cref="MinimumX"/> and <see cref="MaximumX"/>)
    /// are specified in spectral units (true) or as row indices (false).
    /// </summary>
    public bool XIsInSpectralUnits { get; init; } = true;

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="BlockScalingCorrection2D"/>.
    /// </summary>
    /// <remarks>
    /// V0: 2026-06-26 initial version
    /// </remarks> 
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BlockScalingCorrection2D), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (BlockScalingCorrection2D)obj;
        info.AddValue("SizeOfDimension0", s.SizeOfDimension0);
        info.AddValue("SizeOfDimension1", s.SizeOfDimension1);
        info.AddValue("IndexOfDimensionToAverage", s.IndexOfDimensionToAverage);
        info.AddValue("XIsInSpectralUnits", s.XIsInSpectralUnits);
        info.AddValue("MinimumX", s.MinimumX);
        info.AddValue("MaximumX", s.MaximumX);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var sizeOfDimension0 = info.GetInt32("SizeOfDimension0");
        var sizeOfDimension1 = info.GetNullableInt32("SizeOfDimension1");
        var indexOfDimensionToAverage = info.GetInt32("IndexOfDimensionToAverage");
        var xIsInSpectralUnits = info.GetBoolean("XIsInSpectralUnits");
        var minimalValue = info.GetDouble("MinimumX");
        var maximalValue = info.GetDouble("MaximumX");

        return (o as BlockScalingCorrection2D ?? new BlockScalingCorrection2D()) with
        {
          SizeOfDimension0 = sizeOfDimension0,
          SizeOfDimension1 = sizeOfDimension1,
          IndexOfDimensionToAverage = indexOfDimensionToAverage,
          XIsInSpectralUnits = xIsInSpectralUnits,
          MinimumX = minimalValue,
          MaximumX = maximalValue
        };
      }
    }
    #endregion

    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxiliaryData? auxiliaryData) Execute(double[] x, Matrix<double> y, int[]? regions)
    {
      var sizeOfDimension0 = SizeOfDimension0;
      var sizeOfDimension1 = SizeOfDimension1 ?? y.RowCount / SizeOfDimension0;
      // if SizeOfDimension1 is not set, we test whether SizeOfDimension0*sizeOfDimension1 == y.RowCount
      if (y.RowCount % SizeOfDimension0 != 0 || y.RowCount / SizeOfDimension0 != sizeOfDimension1)
      {
        throw new InvalidOperationException($"The number of rows in the spectra matrix ({y.RowCount}) is not compatible with the specified SizeOfDimension0 ({SizeOfDimension0}) and SizeOfDimension1 ({sizeOfDimension1}).");
      }

      var (numberOfBlocks, numberOfSpectraPerBlock) = IndexOfDimensionToAverage == 0 ? (sizeOfDimension1, sizeOfDimension0) : (sizeOfDimension0, sizeOfDimension1);

      int firstSpectrumIndex;
      int lastSpectrumIndex;
      if (XIsInSpectralUnits)
      {
        firstSpectrumIndex = SignalMath.GetIndexOfXInAscendingArray(x, MinimumX, roundUp: true);
        lastSpectrumIndex = SignalMath.GetIndexOfXInAscendingArray(x, MaximumX, roundUp: false);
      }
      else
      {
        firstSpectrumIndex = (int)Math.Max(0, MinimumX);
        lastSpectrumIndex = (int)Math.Min(y.RowCount - 1, MaximumX);
      }

      var yy = y.Clone();
      var blockScale = new double[y.RowCount];
      blockScale.FillWith(1);
      for (int blockIndex = 0; blockIndex < numberOfBlocks; blockIndex++)
      {
        // compute the mean spectrum for the block
        double sum = 0;
        long count = 0;
        for (int idxS = 0; idxS < numberOfSpectraPerBlock; idxS++)
        {
          int rowIndex = IndexOfDimensionToAverage == 0 ? idxS * numberOfBlocks + blockIndex : blockIndex * numberOfSpectraPerBlock + idxS;

          for (int j = firstSpectrumIndex; j <= lastSpectrumIndex; j++)
          {
            sum += y[rowIndex, j];
            count++;
          }
        }
        var scale = (count == 0 || sum == 0) ? 1 : count / sum;
        for (int idxS = 0; idxS < numberOfSpectraPerBlock; idxS++)
        {
          int rowIndex = IndexOfDimensionToAverage == 0 ? idxS * numberOfBlocks + blockIndex : blockIndex * numberOfSpectraPerBlock + idxS;
          blockScale[rowIndex] = scale;

          for (int j = 0; j < y.ColumnCount; j++)
          {
            yy[rowIndex, j] = y[rowIndex, j] * scale;
          }
        }
      }

      return (x, yy, regions, new EnsembleAuxiliaryDataVector() { Name = "BlockScale", Value = blockScale, VectorType = EnsembleAuxiliaryDataVectorType.Samples });
    }

    /// <summary>
    /// Executes the preprocessor on a single spectrum.
    /// In this context, block scaling correction cannot be meaningfully applied to a single spectrum.
    /// Hence, the input spectrum is returned unchanged.
    /// </summary>
    /// <param name="x">The array of X-values for the spectrum.</param>
    /// <param name="y">The array of Y-values for the spectrum.</param>
    /// <param name="regions">Optional array of region indices corresponding to the spectrum data.</param>
    /// <returns>The original X-values, Y-values, and regions, as no operation is performed.</returns>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      // here we can't do anything meaningful
      return (x, y, regions);
    }

    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> spectraMatrix, int[] regions, IEnsembleProcessingAuxiliaryData? auxillaryData)
    {
      throw new InvalidOperationException($"The {this.GetType().Name} preprocessor is not suitable for prediction. You have to remove it from the training process.");
    }
  }
}
