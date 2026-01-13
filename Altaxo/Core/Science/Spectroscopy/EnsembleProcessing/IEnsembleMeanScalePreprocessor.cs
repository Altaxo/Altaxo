using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.EnsembleProcessing
{
  public interface IEnsembleProcessingAuxillaryData
  {
    string Name { get; }
  }

  public record EnsembleAuxillaryDataScalar : IEnsembleProcessingAuxillaryData
  {
    public required string Name { get; init; }
    public double Value { get; init; }
  }

  public record EnsembleAuxillaryDataVector : IEnsembleProcessingAuxillaryData
  {
    public required string Name { get; init; }
    public double[] Value { get; init; }
  }

  public record EnsembleAuxillaryDataCompound : IEnsembleProcessingAuxillaryData
  {
    public required string Name { get; init; }
    public IEnsembleProcessingAuxillaryData[] Values { get; init; }
  }


  public interface IEnsemblePreprocessor : ISingleSpectrumPreprocessor
  {
    /// <summary>
    /// Executes the processor for an ensemble of spectra.
    /// </summary>
    /// <param name="x">The x-values of the spectrum.</param>
    /// <param name="y">The spectra. Each row of the matrix represents a spectrum.</param>
    /// <param name="regions">
    /// The spectral regions. Can be <see langword="null"/> (if the array is one region). Each element in this array
    /// is the start index of a new spectral region.
    /// </param>
    /// <returns>X-values, y-values, and regions of the processed spectrum.</returns>
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxillaryData? auxillaryData) Execute(double[] x, Matrix<double> y, int[]? regions);

    /// <summary>
    /// Processes the spectra for prediction.
    /// For prediction it is necessary to use the spectral mean and scale values previously evaluated by
    /// <see cref="Execute(double[], double[], int[]?)"/>, in order to apply the same treatment to the spectra.
    /// </summary>
    /// <param name="spectraMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="regions">Vector of spectral regions. Each element is the index of the start of a new region.</param>
    /// <param name="auxillaryData">The previously evaluated data from the ensemble preprocessing, i.e. the return value from <see cref="Process(IMatrix{double}, int[])"/>.</param>
    (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> spectraMatrix, int[] regions, IEnsembleProcessingAuxillaryData? auxillaryData);

    /// <summary>
    /// Executes the single spectrum processor. By definition, in an ensemble preprocessor this does nothing.
    /// </summary>
    /// <param name="x">The x-values of the spectrum.</param>
    /// <param name="y">The y-values of the spectrum.</param>
    /// <param name="regions">
    /// The spectral regions. Can be <see langword="null"/> (if the array is one region). Each element in this array
    /// is the start index of a new spectral region.
    /// </param>
    /// <returns>X-values, y-values, and regions of the processed spectrum.</returns>
    public new (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      // No processing for single spectrum
      return (x, y, regions);
    }
  }

  public record EnsembleProcessingNone : IEnsemblePreprocessor
  {
    public static readonly EnsembleProcessingNone Instance = new EnsembleProcessingNone();

    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxillaryData? auxillaryData) Execute(double[] x, Matrix<double> y, int[]? regions)
    {
      return (x, y, regions, null);
    }

    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      return (x, y, regions);
    }

    public (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> y, int[] regions, IEnsembleProcessingAuxillaryData? auxillaryData)
    {
      return (x, y, regions);
    }
  }


  public record EnsembleProcessingSubtractMinimum : IEnsemblePreprocessor
  {
    public static readonly EnsembleProcessingNone Instance = new EnsembleProcessingNone();
    /// <inheritdoc/>
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxillaryData? auxillaryData) Execute(double[] x, Matrix<double> y, int[]? regions)
    {
      var min = y.Min();
      return (x, y - min, regions, new EnsembleAuxillaryDataScalar { Name = "GlobalMinimum", Value = min });
    }

    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var min = y.Min();
      var newY = y.Select(v => v - min).ToArray();
      return (x, newY, regions);
    }

    public (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> spectraMatrix, int[] regions, IEnsembleProcessingAuxillaryData? auxillaryData)
    {
      if (auxillaryData is not EnsembleAuxillaryDataScalar data)
      {
        throw new System.ArgumentException("Auxillary data is not of expected type EnsembleAuxillaryDataScalar.", nameof(auxillaryData));
      }
      var min = data.Value;
      return (x, spectraMatrix - min, regions);
    }
  }

  public record SubtractEnsembleMean : IEnsemblePreprocessor
  {
    const string AuxillaryDataName = "EnsembleMean";
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxillaryData? auxillaryData) Execute(double[] x, Matrix<double> y, int[]? regions)
    {
      var ensembleMean = new double[y.ColumnCount];
      for (int j = 0; j < y.ColumnCount; j++)
      {
        ensembleMean[j] = y.Row(j).Average();
      }
      var yNew = y.Clone();
      for (int i = 0; i < yNew.RowCount; i++)
      {
        for (int j = 0; j < yNew.ColumnCount; j++)
        {
          yNew[i, j] -= ensembleMean[j];
        }
      }
      return (x, yNew, regions, new EnsembleAuxillaryDataVector { Name = AuxillaryDataName, Value = ensembleMean });
    }

    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      return (x, y, regions);
    }

    public (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> spectraMatrix, int[] regions, IEnsembleProcessingAuxillaryData? auxillaryData)
    {
      if (auxillaryData is not EnsembleAuxillaryDataVector data || data.Name != AuxillaryDataName)
      {
        throw new System.ArgumentException("Auxillary data is not of expected type EnsembleAuxillaryDataVector.", nameof(auxillaryData));
      }
      var yNew = spectraMatrix.Clone();
      var ensembleMean = data.Value;
      for (int i = 0; i < yNew.RowCount; i++)
      {
        for (int j = 0; j < yNew.ColumnCount; j++)
        {
          yNew[i, j] -= ensembleMean[j];
        }
      }
      return (x, yNew, regions);
    }
  }
  public record EnsembleMeanScale : IEnsemblePreprocessor
  {
    const string AuxillaryDataName = "EnsembleMeanScale";
    public (double[] x, Matrix<double> y, int[]? regions, IEnsembleProcessingAuxillaryData? auxillaryData) Execute(double[] x, Matrix<double> y, int[]? regions)
    {
      var ensembleMean = new double[y.ColumnCount];
      var ensembleScale = new double[y.ColumnCount];

      var yNew = y.Clone();
      for (int col = 0; col < y.ColumnCount; col++)
      {
        double sum = 0;
        double sumsqr = 0;
        for (int row = 0; row < y.RowCount; row++)
        {
          sum += y[row, col];
          sumsqr += RMath.Pow2(y[row, col]);
        }
        double mean = sum / y.RowCount; // calculate the mean
        double scor;
        if (y.RowCount > 1 && sumsqr - mean * sum > 0)
          scor = Math.Sqrt((y.RowCount - 1) / (sumsqr - mean * sum));
        else
          scor = 1;

        ensembleMean[col] = mean;
        ensembleScale[col] = scor;

        for (int row = 0; row < y.RowCount; row++)
          yNew[row, col] = (y[row, col] - mean) * scor;
      }

      return (x, yNew, regions, new EnsembleAuxillaryDataCompound
      {
        Name = AuxillaryDataName,
        Values = [
          new EnsembleAuxillaryDataVector { Name = "EnsembleMean", Value = ensembleMean },
          new EnsembleAuxillaryDataVector { Name = "EnsembleScale", Value = ensembleScale }
          ]
      });
    }

    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      return (x, y, regions);
    }

    public (double[] x, Matrix<double> y, int[]? regions) ExecuteForPrediction(double[] x, Matrix<double> spectraMatrix, int[] regions, IEnsembleProcessingAuxillaryData? auxillaryData)
    {
      if (auxillaryData is not EnsembleAuxillaryDataVector data || data.Name != AuxillaryDataName)
      {
        throw new System.ArgumentException("Auxillary data is not of expected type EnsembleAuxillaryDataVector.", nameof(auxillaryData));
      }
      var yNew = spectraMatrix.Clone();
      var ensembleScale = data.Value;
      for (int i = 0; i < yNew.RowCount; i++)
      {
        for (int j = 0; j < yNew.ColumnCount; j++)
        {
          yNew[i, j] *= ensembleScale[j];
        }
      }
      return (x, yNew, regions);
    }
  }

  /// <summary>
  /// Interface for spectral preprocessing steps that process multiple spectra together.
  /// </summary>
  public interface IEnsembleMeanScalePreprocessor
  {
    /// <summary>
    /// Processes multiple spectra together.
    /// </summary>
    /// <param name="spectraMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="regions">Vector of spectral regions. Each element is the index of the start of a new region.</param>
    /// <param name="spectraMean">Output: On return, contains the ensemble mean of the spectra.</param>
    /// <param name="spectraScale">Output: On return, contains scaling factors for each spectral slot.</param>
    void Process(IMatrix<double> spectraMatrix, int[] regions, IVector<double> spectraMean, IVector<double> spectraScale);


    /// <summary>
    /// Processes the spectra for prediction.
    /// For prediction it is necessary to use the spectral mean and scale values previously evaluated by
    /// <see cref="Process(IMatrix{double}, int[], IVector{double}, IVector{double})"/>, in order to apply the same treatment to the spectra.
    /// </summary>
    /// <param name="spectraMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="regions">Vector of spectral regions. Each element is the index of the start of a new region.</param>
    /// <param name="spectraMean">The previously evaluated ensemble mean of the spectra.</param>
    /// <param name="spectraScale">The previously evaluated ensemble scale of the spectra.</param>
    void ProcessForPrediction(IMatrix<double> spectraMatrix, int[] regions, IReadOnlyList<double> spectraMean, IReadOnlyList<double> spectraScale);
  }



}
