#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

  /// <summary>
  /// Provides initialization helpers for non-negative matrix factorization (NMF),
  /// specifically NNDSVD-based initializations.
  /// </summary>
  public abstract record NonnegativeMatrixFactorizationBase : ILowRankMatrixFactorization
  {
    /// <summary>
    /// Gets or sets the initialization method to be used for the factorization.
    public INonnegativeMatrixFactorizationInitializer InitializationMethod { get; set; } = new NNDSVDar();

    public int MaximumNumberOfIterations
    {
      get => field;
      init
      {
        if (value < 1)
          throw new ArgumentOutOfRangeException(nameof(MaximumNumberOfIterations), "Maximum number of iterations must be at least 1.");
        field = value;
      }
    } = 1000;

    public int NumberOfAdditionalTrials
    {
      get => field;
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(NumberOfAdditionalTrials), "Number of trials must be nonnegative.");
        field = value;
      }
    }

    public double Tolerance
    {
      get => field;
      init
      {
        if (!(value > 0 && value < 1))
          throw new ArgumentOutOfRangeException(nameof(Tolerance), "Tolerance must be in the range (0, 1).");
        field = value;
      }
    } = 1E-9;



    /// <summary>
    /// Factorizes a non-negative matrix <paramref name="X"/> into non-negative factors <c>W</c> and <c>H</c>.
    /// </summary>
    /// <param name="X">The input matrix to factorize.</param>
    /// <param name="rank">The factorization rank.</param>
    /// <returns>
    /// A tuple containing the factors <c>W</c> and <c>H</c>.
    /// </returns>
    public abstract (Matrix<double> W, Matrix<double> H) FactorizeOneTrial(Matrix<double> X, int rank);


    /// <summary>
    /// Factorizes a non-negative matrix <paramref name="V"/> into non-negative factors <c>W</c> and <c>H</c> using multiplicative updates.
    /// </summary>
    /// <param name="V">The input matrix to factorize.</param>
    /// <param name="r">The factorization rank.</param>
    /// <returns>
    /// A tuple containing the factors <c>W</c> and <c>H</c> and the final relative reconstruction error <c>relErr</c>.
    /// </returns>
    public (Matrix<double> W, Matrix<double> H) Factorize(Matrix<double> V, int rank)
    {
      var (W, H) = FactorizeOneTrial(V, rank);
      if (NumberOfAdditionalTrials == 0)
      {
        return (W, H);
      }
      else
      {
        var vNorm = V.FrobeniusNorm();
        double bestErr = (V - (W * H)).FrobeniusNorm() / vNorm;
        Matrix<double> bestW = W;
        Matrix<double> bestH = H;

        for (int trial = 0; trial < NumberOfAdditionalTrials; trial++)
        {
          // Initialization is random for the other trials
          if (this.InitializationMethod is not NMFInitializationRandom)
          {
            var method = this with { InitializationMethod = new NMFInitializationRandom() };
            (W, H) = method.FactorizeOneTrial(V, rank);
          }
          else
          {
            (W, H) = FactorizeOneTrial(V, rank);
          }

          double finalErr = (V - (W * H)).FrobeniusNorm() / vNorm;
          if (finalErr < bestErr)
          {
            bestErr = finalErr;
            bestW = W;
            bestH = H;
          }
        }

        return (bestW, bestH);
      }
    }
  }

  /// <summary>
  /// Provides initialization helpers for non-negative matrix factorization (NMF),
  /// specifically NNDSVD-based initializations.
  /// </summary>
  public abstract record NonnegativeMatrixFactorizationWithRegularizationBase : NonnegativeMatrixFactorizationBase
  {
    public double LambdaW
    {
      get => field;
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(LambdaW), "LambdaW must be non-negative.");
        field = value;
      }
    } = 0;

    public double LambdaH
    {
      get => field;
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(LambdaH), "LambdaH must be non-negative.");
        field = value;
      }
    } = 0;
  }
}
