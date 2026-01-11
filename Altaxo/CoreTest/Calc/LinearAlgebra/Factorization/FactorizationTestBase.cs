using System;

namespace Altaxo.Calc.LinearAlgebra.Factorization
{
  public class FactorizationTestBase
  {
    protected const int SpectralPoints = 1600;
    protected const int NumberOfSpectra = 100;
    protected const int NumberOfComponents = 3;

    protected static Matrix<double> GetThreeSpectra()
    {
      var loadings = Matrix<double>.Build.Dense(NumberOfComponents, SpectralPoints);
      for (int idxSpectrum = 0; idxSpectrum < NumberOfComponents; ++idxSpectrum)
      {
        var xc = idxSpectrum switch
        {
          0 => SpectralPoints / 2,
          1 => SpectralPoints / 4,
          2 => (SpectralPoints * 3) / 4,
          _ => SpectralPoints / 2
        };

        for (var idxSpectralPoint = 0; idxSpectralPoint < SpectralPoints; idxSpectralPoint++)
        {
          var arg = (idxSpectralPoint - xc) / 80d;
          loadings[idxSpectrum, idxSpectralPoint] = Math.Exp(-0.5 * arg * arg);
        }
      }
      return loadings;
    }

    protected static Matrix<double> GetScores3D(int numberOfSpectra)
    {
      var scores = Matrix<double>.Build.Dense(numberOfSpectra, NumberOfComponents);
      for (int c = 0; c < NumberOfComponents; c++)
      {
        for (int r = 0; r < numberOfSpectra; r++)
        {
          switch (c)
          {
            case 0: scores[r, c] = r / (double)numberOfSpectra; break;
            case 1: scores[r, c] = 1.0 - (r / (double)numberOfSpectra); break;
            case 2:
              {
                var angle = (r / (double)numberOfSpectra) * 4.0 * Math.PI;
                scores[r, c] = 0.5 * (1.0 + Math.Sin(angle));
              }
              break;
          }
        }
      }
      return scores;
    }


    protected static double RelativeError(Matrix<double> scores, Matrix<double> loadings, Matrix<double> originalMatrix)
    {
      var Vhat = scores * loadings;
      return (Vhat - originalMatrix).FrobeniusNorm() / originalMatrix.FrobeniusNorm();
    }
  }
}
