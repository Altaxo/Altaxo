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
  /// Implements the Asymmetric Least Squares method for baseline estimation, proposed by Eilers and Boelens 2005 [1].
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] P. H. C. Eilers and H. F. M. Boelens, Baseline correction with
  /// asymmetric least squares smoothing, Leiden University Medical Centre report, 2005</para>
  // </remarks>
  public record ALS : IBaselineEstimation
  {
    private double _lambda = 1E6;

    /// <summary>
    /// Gets or sets the smoothing parameter lambda.
    /// The default value is 1E6.
    /// The higher lambda is, the smoother the resulting curve will be.
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

    private double _p = 0.1;
    /// <summary>
    /// Gets or sets the weighting parameter.
    /// The default value is 0.1.
    /// A value of 0.5 leads to symmetric weighting of positive and negative deviations.
    /// Values less than 0.5 leads to stronger suppression of (positive) peaks.
    /// </summary>
    /// <exception cref="ArgumentException">Value must be &gt; 0 and &lt; 1</exception>
    public double P
    {
      get => _p;
      set
      {
        if (!(value > 0 && value < 1))
          throw new ArgumentException("Value must be > 0 and < 1", nameof(P));
        _p = value;
      }
    }


    private int _maximumNumberOfIterations = 10;

    /// <summary>
    /// Gets or sets the maximum number of iterations. The default value is 10.
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

    private int _order = 1;

    public int Order
    {
      get => _order;
      init
      {
        if (!(value >= 1 && value <= 2))
          throw new ArgumentOutOfRangeException("Order must be 1 or 2", nameof(Order));
        _order = value;
      }
    }


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ALS), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ALS)obj;
        info.AddValue("Lambda", s.Lambda);
        info.AddValue("ScaleLambdaWithXUnits", s.ScaleLambdaWithXUnits);
        info.AddValue("P", s.P);
        info.AddValue("Order", s.Order);
        info.AddValue("MaxNumberOfIterations", s.MaximumNumberOfIterations);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var lambda = info.GetDouble("Lambda");
        var scaleLambdaWithXUnits = info.GetBoolean("ScaleLambdaWithXUnits");
        var p = info.GetDouble("P");
        var order = info.GetInt32("Order");
        var maxNumberOfIterations = info.GetInt32("MaxNumberOfIterations");

        return o is null ? new ALS
        {
          Lambda = lambda,
          ScaleLambdaWithXUnits = scaleLambdaWithXUnits,
          P = p,
          Order = order,
          MaximumNumberOfIterations = maxNumberOfIterations,
        } :
          ((ALS)o) with
          {
            Lambda = lambda,
            ScaleLambdaWithXUnits = scaleLambdaWithXUnits,
            P = p,
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

      var solver = new GaussianEliminationSolver();

      for (int iteration = 1; iteration <= _maximumNumberOfIterations; ++iteration)
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

        // Update wx by multiply weights with original x
        for (int i = 0; i <= countM1; ++i)
          wx[i] = y[i] * weights[i];

        solver.SolveDestructiveBanded(m, 1, 1, wx, z);

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

        // update weights 
        for (int i = 0; i <= countM1; ++i)
        {
          var diff = y[i] - z[i];
          weights[i] = diff <= 0 ? 1-_p : _p;
        }
      }

      return z;
    }
  }
}
