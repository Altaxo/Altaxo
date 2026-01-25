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
using Altaxo.Collections;

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
    /// </summary>
    public INonnegativeMatrixFactorizationInitializer InitializationMethod { get; set; } = new NNDSVDar();

    /// <summary>
    /// Gets the maximum number of iterations for the factorization algorithm.
    /// </summary>
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

    /// <summary>
    /// Gets the number of additional trials to perform with a random initialization.
    /// </summary>
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

    /// <summary>
    /// Gets the convergence tolerance.
    /// </summary>
    /// <remarks>The default value of 1E-2 means that the iterations are stopped
    /// if the expected improvement of the relative error until the maximum number of iterations is less than 1E-2 times
    /// the current relative error.</remarks>
    public double Tolerance
    {
      get => field;
      init
      {
        if (!(value > 0 && value < 1))
          throw new ArgumentOutOfRangeException(nameof(Tolerance), "Tolerance must be in the range (0, 1).");
        field = value;
      }
    } = 1E-2;



    /// <summary>
    /// Factorizes a non-negative matrix <paramref name="X"/> into non-negative factors <c>W</c> and <c>H</c>.
    /// </summary>
    /// <param name="X">The input matrix to factorize.</param>
    /// <param name="rank">The factorization rank.</param>
    /// <returns>
    /// A tuple containing the factors <c>W</c> and <c>H</c>.
    /// </returns>
    public abstract (Matrix<double> W, Matrix<double> H) FactorizeOneTrial(Matrix<double> X, int rank);


    /// <inheritdoc/>
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

    /// <summary>
    /// Calculates the trace of the product of the transposed matrix A with B: trace(AᵀB). 
    /// </summary>
    /// <param name="A">First matrix (not changed).</param>
    /// <param name="B">Second matrix (not changed).</param>
    /// <returns>The value of trace(AᵀB).</returns>
    public static double TraceOfTransposeAndMultiply(Matrix<double> A, Matrix<double> B)
    {
      var n = B.RowCount;
      var m = B.ColumnCount;

      double sum = 0;
      for (int i = 0; i < n; ++i)
        for (int j = 0; j < m; ++j)
          sum += A[i, j] * B[i, j];

      return sum;
    }

    /// <summary>
    /// Stores the history of chi2 values to determine convergence.
    /// For the ACLS algorithm, we stop if the chi2 value increases for a number of iterations.
    /// </summary>
    protected class Chi2History
    {
      private readonly double _tolerance;
      private int _remainingIterations;

      private Altaxo.Collections.RingBufferEnqueueableOnly<double> _chi2Values;

      private Matrix<double>? _W;
      private Matrix<double>? _H;
      private double _minimalError;

      /// <summary>
      /// Creates a new instance of <see cref="Chi2History"/> with the specified history depth.
      /// </summary>
      /// <param name="depth">The number of historical elements to store.</param>
      public Chi2History(int depth, double tolerance, int iterations)
      {
        _tolerance = tolerance;
        _remainingIterations = iterations;
        _chi2Values = new(depth);
        _minimalError = double.PositiveInfinity;
      }

      /// <summary>
      /// Adds a new chi² value to the history.
      /// </summary>
      /// <param name="chi2">The new chi² value.</param>
      /// <param name="w">The corresponding weight matrix (it is cloned for storage).</param>
      /// <param name="h">The corresponding load matrix (it is cloned for storage).</param>
      /// <returns>A tuple <c>(terminate, w, h)</c>. If <c>terminate</c> is <see langword="true"/>, the iteration can be terminated, and the returned <c>w</c> and <c>h</c> values are the best factors for the factorization. If <c>terminate</c> is <see langword="false"/>, then <c>w</c> and <c>h</c> are <see langword="null"/>.</returns>
      public (bool terminate, Matrix<double>? w, Matrix<double>? h) Add(double chi2, Matrix<double> w, Matrix<double> h)
      {
        if (double.IsNaN(chi2) || double.IsInfinity(chi2))
        {
          if (_W is null || _H is null)
            throw new ArgumentOutOfRangeException(nameof(chi2), "Chi² value must be a valid number.");
          else
            return (true, _W, _H);
        }

        _chi2Values.Enqueue(chi2);
        _remainingIterations--;

        if (chi2 < _minimalError)
        {
          _minimalError = chi2;
          if (_W is null || h is null)
          {
            _W = w.Clone();
            _H = h.Clone();
          }
          else
          {
            w.CopyTo(_W);
            h.CopyTo(_H);
          }

          // try to guess what is the gain in error if we continue
          var countM1 = _chi2Values.Count - 1;
          if (countM1 >= 1)
          {

            double errorSlope = (_chi2Values.OldestValue - _chi2Values.NewestValue) / (countM1);
            double errorGain = errorSlope * _remainingIterations;
            if (errorGain < _tolerance * _minimalError)
            {
              // we can stop the iteration, and return the best w and h
              return (true, _W, this._H);
            }
          }
        }
        else // the error was larger than or equal to the minimal error
        {
          var (minimum, maximum) = _chi2Values.MinMax();

          if (minimum > _minimalError || minimum == maximum) // if all errors in the history are larger than the minimal error, we can stop
          {
            // we can stop the iteration, and return the best w and h
            return (true, _W, this._H);
          }
        }
        return (false, null, null);
      }
    }
  }
}
