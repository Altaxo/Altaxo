#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  /// <summary>
  /// Implements the asymmetrically reweighted penalized least squares algorithm proposed by Baek et al [1].
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Sung-June Baek et al., Baseline correction using asymmetrically reweighted penalized least squares smoothing,
  /// Analyst, 2015, 140, 250-257 doi: 10.1039/C4AN01061B</para>
  // </remarks>
  public class ArPLS : IBaselineEstimation
  {
    private double _lambda = 1E5;

    /// <summary>
    /// Gets or sets the smoothing parameter lambda. The higher lambda is, the smoother the resulting curve will be.
    /// </summary>
    /// <exception cref="System.ArgumentException">Value must be &gt; 0</exception>
    public double Lambda
    {
      get => _lambda;
      set
      {
        if (!(value > 0))
          throw new ArgumentException("Value must be > 0", nameof(Lambda));
        _lambda = value;
      }
    }

    private double _terminationRatio = 0.05;
    /// <summary>
    /// Gets or sets the criterion for terminating the iteration (0..1). Default is 0.05.
    /// The iterations stops, if the L2 norm of the differences between actual and previous weights falls below (TerminationRatio x L2 norm of the previous weights).
    /// The lower the value is, the more iterations will be executed.
    /// </summary>
    /// <exception cref="ArgumentException">Value must be &gt; 0 and &lt; 1</exception>
    public double TerminationRatio
    {
      get => _terminationRatio;
      set
      {
        if (!(value > 0 && value < 1))
          throw new ArgumentException("Value must be > 0 and < 1", nameof(TerminationRatio));
        _terminationRatio = value;
      }
    }

    /// <summary>
    /// Gets the number of iterations that were executed during the last call to <see cref="Execute(IEnumerable{double})"/>.
    /// </summary>
    public int ActualNumberOfIterations { get; private set; }

    private int _maximumNumberOfIterations = 100;

    private int _order = 1;

    public int Order
    {
      get => _order;
      set
      {
        if (!(value >= 1 && value <= 2))
          throw new ArgumentOutOfRangeException("Order must be 1 or 2", nameof(Order));
        _order = value;
      }
    }

    /// <summary>
    /// Gets or sets the maximum number of iterations. The default value is 100.
    /// Usually, the number of iterations is determined by the <see cref="TerminationRatio"/>, but
    /// with this value, the maximum number of iterations can be limited to a smaller value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Value must be &gt;=1</exception>
    public int MaximumNumberOfIterations
    {
      get => _maximumNumberOfIterations;
      set
      {
        if (!(value >= 1))
          throw new ArgumentOutOfRangeException("Value must be >=1", nameof(MaximumNumberOfIterations));
        _maximumNumberOfIterations = value;
      }
    }

    /// <inheritdoc/>
    public double[] Execute(IEnumerable<double> array)
    {
      var x = array.ToArray();
      var countM1 = x.Length - 1;
      var wx = new double[countM1 + 1]; // x multiplied with weights
      var z = new double[countM1 + 1];

      // set up the weigths with initial value of 1
      var weights = new double[countM1 + 1];
      var nextWeights = new double[countM1 + 1];
      for (int i = 0; i <= countM1; ++i)
        weights[i] = 1;

      // Calculate L1-norm of x
      double l1NormOfX = 0;
      for (int i = 0; i < x.Length; ++i)
      {
        l1NormOfX += Math.Abs(x[i]);
      }


      // Set up the band matrix (W + lambda D'D)
      // TODO: when band matrices are available, replace with true band matrix (will save storage space)
      var m = new MatrixWrapperStructForLeftSpineJaggedArray<double>(countM1 + 1, countM1 + 1);
      var lambda = _lambda;

      var solver = new GaussianEliminationSolver();

      for (int iteration = 1; iteration <= _maximumNumberOfIterations; ++iteration)
      {
        // Update wx by multiply weights with original x
        for (int i = 0; i <= countM1; ++i)
        {
          wx[i] = x[i] * weights[i];
        }

        switch (_order)
        {
          case 1:
            {
              FillBandMatrixOrder1(m, weights, lambda, countM1);
              solver.SolveDestructiveBanded(m, 1, 1, wx, z);
            }
            break;
          case 2:
            {
              FillBandMatrixOrder2(m, weights, lambda, countM1);
              solver.SolveDestructiveBanded(m, 2, 2, wx, z);
            }
            break;
          default:
            {
              throw new NotImplementedException($"A order of {_order} is not implemented yet");
            }
        }

        // Calculate mean and standard deviation of the vector of the negative differences between x and z
        int numberOfNegativeDifferences = 0;
        double sumOfNegativeDifferences = 0;
        double sum2OfNegativeDifferences = 0;
        for (int i = 0; i <= countM1; ++i)
        {
          var diff = x[i] - z[i];
          if (diff < 0)
          {
            ++numberOfNegativeDifferences;
            sumOfNegativeDifferences += diff;
            sum2OfNegativeDifferences += diff * diff;
          }
        }
        double meanOfNegativeDifferences = sumOfNegativeDifferences / numberOfNegativeDifferences;
        double standardDeviationOfNegativeDifferences = Math.Sqrt(Math.Max(0, sum2OfNegativeDifferences / numberOfNegativeDifferences - meanOfNegativeDifferences * meanOfNegativeDifferences));

        // calculate new weights, using a logistic equation
        for (int i = 0; i <= countM1; ++i)
        {
          var diff = x[i] - z[i];
          nextWeights[i] = 1 / (1 + Math.Exp(2 * (diff - (2 * standardDeviationOfNegativeDifferences - meanOfNegativeDifferences)) / standardDeviationOfNegativeDifferences));
        }

        // Stop criterion Norm(weights-nextWeights)/Norm(weigths) < ratio (pseudo code in Ref.[1])
        ActualNumberOfIterations = iteration;
        double sum2Wdiff = 0; double sum2W = 0;
        for (int i = 0; i <= countM1; ++i)
        {
          var wdiff = weights[i] - nextWeights[i];
          sum2Wdiff += wdiff * wdiff;
          sum2W += weights[i] * weights[i];
        }
        if (iteration > 1 && Math.Sqrt(sum2Wdiff) < _terminationRatio * Math.Sqrt(sum2W))
          break;

        // nextWeights now become the new weights - both arrays are swapped
        (nextWeights, weights) = (weights, nextWeights);
      }

      return z;
    }

    public void FillBandMatrixOrder1(MatrixWrapperStructForLeftSpineJaggedArray<double> m, double[] weights, double lambda, int countM1)
    {
      // Fill the (1,1) band matrix with (W + lambda D'D) (Eq.(6) in Ref.[1])
      m[0, 0] = weights[0] + lambda;
      m[0, 1] = -lambda;

      for (int i = 1; i < countM1; ++i)
      {
        m[i, i - 1] = -lambda;
        m[i, i] = weights[i] + 2 * lambda;
        m[i, i + 1] = -lambda;
      }
      m[countM1, countM1 - 1] = -lambda;
      m[countM1, countM1] = weights[countM1] + lambda;
    }

    public void FillBandMatrixOrder2(MatrixWrapperStructForLeftSpineJaggedArray<double> m, double[] weights, double lambda, int countM1)
    {
      // Fill the (2,2) band matrix with (W + lambda D'D) (Eq.(6) in Ref.[1])
      m[0, 0] = weights[0] + lambda;
      m[0, 1] = -2 * lambda;
      m[0, 2] = lambda;

      m[1, 0] = -2 * lambda;
      m[1, 1] = weights[1] + 5 * lambda;
      m[1, 2] = -4 * lambda;
      m[1, 3] = lambda;

      for (int i = 2; i < countM1 - 1; ++i)
      {
        m[i, i - 2] = lambda;
        m[i, i - 1] = -4 * lambda;
        m[i, i] = weights[i] + 6 * lambda;
        m[i, i + 1] = -4 * lambda;
        m[i, i + 2] = lambda;
      }

      m[countM1 - 1, countM1 - 3] = lambda;
      m[countM1 - 1, countM1 - 2] = -4 * lambda;
      m[countM1 - 1, countM1 - 1] = weights[countM1 - 1] + 5 * lambda;
      m[countM1 - 1, countM1] = -2 * lambda;


      m[countM1, countM1 - 2] = lambda;
      m[countM1, countM1 - 1] = -2 * lambda;
      m[countM1, countM1] = weights[countM1] + lambda;
    }
  }
}
