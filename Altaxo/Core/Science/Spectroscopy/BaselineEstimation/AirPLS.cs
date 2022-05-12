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
  /// Implements the adaptive iteratively reweighted penalized least squares algorithm proposed by Zhang et al [1].
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Z.-M. Zhang et al., Baseline correction using adaptive iteratively reweighted penalized least
  /// squares, Analyst, 2010, 135, 1138–1146, doi:10.1039/b922045c</para>
  // </remarks>
  public record AirPLS : ALSBase, IBaselineEstimation
  {
    private double _lambda = 100;

    /// <summary>
    /// Gets or sets the smoothing parameter lambda.
    /// The default value is 100.
    /// The higher lambda is, the smoother the resulting curve will be.
    /// </summary>
    /// <exception cref="System.ArgumentException">Value must be &gt; 0</exception>
    public double Lambda
    {
      get => _lambda;
      init
      {
        if (!(value > 0))
          throw new ArgumentException("Value must be > 0", nameof(Lambda));
        _lambda = value;
      }
    }

    private bool _scaleLambdaWithXUnits;

    /// <summary>
    /// If true, lambda is scaled with the x units, so that the effect of baseline estimation is independent on the resolution of the spectrum.
    /// </summary>
    /// <value>
    ///   <c>true</c> lambda is scaled with the x units, so that the effect of baseline estimation is independent on the resolution of the spectrum; otherwise, <c>false</c>.
    /// </value>
    public bool ScaleLambdaWithXUnits
    {
      get => _scaleLambdaWithXUnits;
      init => _scaleLambdaWithXUnits = value;
    }

    private double _terminationRatio = 1e-3;
    /// <summary>
    /// Gets or sets the criterion for terminating the iteration (0..1). Default is 1E-3.
    /// The iterations stops, if the L1 norm of points lying below the baseline is smaller than (TerminationRatio x L1 norm of the original spectrum).
    /// The lower the value of the StopCriterion, the less points will remain below the baseline (and the more iteration it takes).
    /// </summary>
    /// <exception cref="ArgumentException">Value must be &gt; 0 and &lt; 1</exception>
    public double TerminationRatio
    {
      get => _terminationRatio;
      init
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

    /// <summary>
    /// Gets or sets the maximum number of iterations. The default value is 100.
    /// Usually, the number of iterations is determined by the <see cref="TerminationRatio"/>, but
    /// with this value, the maximum number of iterations can be limited to a smaller value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Value must be &gt;=1</exception>
    public int MaximumNumberOfIterations
    {
      get => _maximumNumberOfIterations;
      init
      {
        if (!(value >= 1))
          throw new ArgumentOutOfRangeException("Value must be >=1", nameof(MaximumNumberOfIterations));
        _maximumNumberOfIterations = value;
      }
    }

    private int _order = 2;

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

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AirPLS), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AirPLS)obj;
        info.AddValue("Lambda", s.Lambda);
        info.AddValue("ScaleLambdaWithXUnits", s.ScaleLambdaWithXUnits);
        info.AddValue("TerminationRatio", s.TerminationRatio);
        info.AddValue("Order", s.Order);
        info.AddValue("MaxNumberOfIterations", s.MaximumNumberOfIterations);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var lambda = info.GetDouble("Lambda");
        var scaleLambdaWithXUnits = info.GetBoolean("ScaleLambdaWithXUnits");
        var terminationRatio = info.GetDouble("TerminationRatio");
        var order = info.GetInt32("Order");
        var maxNumberOfIterations = info.GetInt32("MaxNumberOfIterations");

        return o is null ? new AirPLS
        {
          Lambda = lambda,
          ScaleLambdaWithXUnits = scaleLambdaWithXUnits,
          TerminationRatio  = terminationRatio,
          Order = order,
          MaximumNumberOfIterations = maxNumberOfIterations,
        } :
          ((AirPLS)o) with
          {
            Lambda = lambda,
            ScaleLambdaWithXUnits = scaleLambdaWithXUnits,
            TerminationRatio = terminationRatio,
            Order = order,
            MaximumNumberOfIterations = maxNumberOfIterations,
          };
      }
    }
    #endregion


    /// <inheritdoc/>
    public double[] Execute(double[] xArray, double[] yArray)
    {
      var y = yArray;
      var countM1 = y.Length - 1;
      var wx = new double[countM1 + 1]; // x multiplied with weights
      var z = new double[countM1 + 1];

      // set up the weigths with initial value of 1
      var weights = new double[countM1 + 1];
      for (int i = 0; i <= countM1; ++i)
        weights[i] = 1;

      // Calculate L1-norm of x
      double l1NormOfX = 0;
      for (int i = 0; i < y.Length; ++i)
      {
        l1NormOfX += Math.Abs(y[i]);
      }


      // Set up the band matrix (W + lambda D'D)
      // TODO: when band matrices are available, replace with true band matrix (will save storage space)
      var m = new MatrixWrapperStructForLeftSpineJaggedArray<double>(countM1 + 1, countM1 + 1);
      var lambda = _lambda;

      // Fill the Band matrix for the first time
      // as long as lambda is constant (it is here),
      // only the diagonal of the band matrix changes when the weight changes
      switch (_order)
      {
        case 1:
          {
            FillBandMatrixOrder1(m, weights, lambda, countM1);
          }
          break;
        case 2:
          {
            FillBandMatrixOrder2(m, weights, lambda, countM1); // Fill the (1,1) band matrix with (W + lambda D'D) (Eq.(6) in Ref.[1])
          }
          break;
        default:
          {
            throw new NotImplementedException($"A order of {_order} is not implemented yet");
          }
      }


      object tempStorage = null;
      for (int iteration = 1; iteration <= _maximumNumberOfIterations; ++iteration)
      {
        // Update wx by multiply weights with original x
        for (int i = 0; i <= countM1; ++i)
          wx[i] = y[i] * weights[i];

        switch (_order)
        {
          case 1:
            {
              UpdateBandMatrixDiagonalOrder1(m, weights, lambda, countM1);
              GaussianEliminationSolver.SolveTriDiagonal(m, wx, z, ref tempStorage);
            }
            break;
          case 2:
            {
              UpdateBandMatrixDiagonalOrder2(m, weights, lambda, countM1); // Fill the (1,1) band matrix with (W + lambda D'D) (Eq.(6) in Ref.[1])
              GaussianEliminationSolver.SolvePentaDiagonal(m, wx, z, ref tempStorage);
            }
            break;
          default:
            {
              throw new NotImplementedException($"A order of {_order} is not implemented yet");
            }
        }

        // Calculate L1 norm of the vector of the negative differences between x and z
        double l1normOfNegativeDifferences = 0;
        for (int i = 0; i <= countM1; ++i)
        {
          var diff = z[i] - y[i];
          if (diff > 0)
          {
            l1normOfNegativeDifferences += diff;
          }
        }

        // Stop criterion l1normOfNegativeDifferences < 1E-3 * l1NormOfX (Eq.(10) in Ref.[1])
        ActualNumberOfIterations = iteration;
        if (l1normOfNegativeDifferences < _terminationRatio * l1NormOfX)
          break;

        // update weights (Eq.(9) in Ref.[1])
        for (int i = 0; i <= countM1; ++i)
        {
          var diff = y[i] - z[i];
          weights[i] = diff <= 0 ? Math.Exp(iteration * diff / l1normOfNegativeDifferences) : 0;
        }
      }

      return z;
    }
  }
}
