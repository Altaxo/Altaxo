#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
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

    /// <summary>
    /// Gets a 4 x 3 nonnegative test matrix.
    /// </summary>
    /// <returns>4 x 3 nonnegative test matrix from <see href="https://resources.wolframcloud.com/FunctionRepository/resources/NonNegativeMatrixFactorization/"/></returns>
    public static Matrix<double> GetTestMatrixNN4x3()
    {
      return CreateMatrix.DenseOfRowArrays<double>(
        [4, 7, 4],
        [10, 8, 8],
        [5, 3, 4],
        [5, 4, 5]
        );
    }



    protected static double RelativeError(Matrix<double> scores, Matrix<double> loadings, Matrix<double> originalMatrix)
    {
      var Vhat = scores * loadings;
      return (Vhat - originalMatrix).FrobeniusNorm() / originalMatrix.FrobeniusNorm();
    }
  }
}
