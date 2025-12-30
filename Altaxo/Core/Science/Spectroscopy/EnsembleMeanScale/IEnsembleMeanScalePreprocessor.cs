using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.EnsembleMeanScale
{
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
