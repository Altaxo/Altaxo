using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.EnsembleProcessing
{
  public interface IEnsemblePreprocessor
  {
    /// <summary>
    /// Processes multiple spectra together. The spectra are modified in place.
    /// </summary>
    /// <param name="spectraMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="regions">Vector of spectral regions. Each element is the index of the start of a new region.</param>
    /// <returns>An object that can be used during the prediction step to process the spectra to predict in exactly the same way.
    /// For instance, for mean-scale ensemble, the returned object hold the mean and the scale values.</returns>
    object Process(IMatrix<double> spectraMatrix, int[] regions);

    /// <summary>
    /// Processes the spectra for prediction.
    /// For prediction it is necessary to use the spectral mean and scale values previously evaluated by
    /// <see cref="Process(IMatrix{double}, int[])"/>, in order to apply the same treatment to the spectra.
    /// </summary>
    /// <param name="spectraMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="regions">Vector of spectral regions. Each element is the index of the start of a new region.</param>
    /// <param name="auxillaryData">The previously evaluated data from the ensemble preprocessing, i.e. the return value from <see cref="Process(IMatrix{double}, int[])"/>.</param>
    void ProcessForPrediction(IMatrix<double> spectraMatrix, int[] regions, object auxillaryData);
  }

  public record EnsembleProcessingNone : IEnsemblePreprocessor
  {
    public static readonly EnsembleProcessingNone Instance = new EnsembleProcessingNone();
    /// <inheritdoc/>
    public object Process(IMatrix<double> spectraMatrix, int[] regions)
    {
      // No processing
      return new object();
    }

    /// <inheritdoc/>
    public void ProcessForPrediction(IMatrix<double> spectraMatrix, int[] regions, object auxillaryData)
    {
    }
  }

  /*
  public record EnsembleProcessingSubtractMinimum : IEnsemblePreprocessor
  {
    public static readonly EnsembleProcessingNone Instance = new EnsembleProcessingNone();
    /// <inheritdoc/>
    public object Process(IMatrix<double> spectraMatrix, int[] regions)
    {
      var min = spectraMatrix.Min();
      MatrixMath.Map(spectraMatrix, x => x-min, spectraMatrix);

      // No processing
      return min;
    }

    /// <inheritdoc/>
    public void ProcessForPrediction(IMatrix<double> spectraMatrix, int[] regions, object auxillaryData)
    {
      var min = (double)auxillaryData;
      MatrixMath.Add(spectraMatrix, min, spectraMatrix);
    }
  }
  */



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
