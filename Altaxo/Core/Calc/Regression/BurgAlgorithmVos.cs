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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// A faster algorithm than <see cref="BurgAlgorithm"/>, if the number of coefficients is less than 1/3 the length of the signal.
  /// This algorithm is based on a paper by Koen Vos, 2013.
  /// </summary>
  /// <remarks>
  /// <para>Literature:</para>
  /// <para>[1] Koen Vos, "A Fast Implementation of Burg's method, August 2013, Creative Commons"</para>
  /// </remarks>
  public class BurgAlgorithmVos
  {
    private const string ErrorNoExecution = "No results yet - call Execute first!";

    /// <summary>Prediction coefficients, see eq (28) in [1]. . Note that for technical reasons _Ak[0] is always 1 and the calculated coefficients start with _Ak[1].</summary>
    protected double[]? _Ak;

    /// <summary>Swapping array for <see cref="_Ak"/></summary>
    protected double[]? _Ak_previous;

    /// <summary>Wrapper for the coefficients that can be returned by <see cref="Coefficients"/>.</summary>
    private IROVector<double>? _AkWrapper;

    /// <summary>Number of coefficients that were calculated.</summary>
    private int _numberOfCoefficients;

    /// <summary>Mean square of the signal values.</summary>
    private double _meanSquareSignal;

    /// <summary>Prediction coefficients, see eq (22) in [1]</summary>
    protected double[]? _g;

    /// <summary>Swapping array for <see cref="_g"/></summary>
    protected double[]? _g_previous;

    /// <summary>Reflection coefficients.</summary>
    protected double[]? _k;

    /// <summary>Wrapper for the reflection coefficients that can be returned by <see cref="ReflectionCoefficients"/>.</summary>
    private IROVector<double>? _kWrapper;

    protected double[]? _r;

    /// <summary>Array containing the auto correlation values, see eq (26) in [1]</summary>
    protected double[]? _c;

    /// <summary>Product of matrix ΔR and prediction coefficients Ak, see eq. (23) in [1].</summary>
    private double[]? _deltaRTimesAk;

    /// <summary>
    /// Returns the number of coefficients that were used for the last run of the algorithm.
    /// </summary>
    public int NumberOfCoefficients
    {
      get
      {
        return _numberOfCoefficients;
      }
    }

    /// <summary>
    /// Returns the coefficients that were calculated during the last run of the algorithm.
    /// </summary>
    public IROVector<double> Coefficients
    {
      get
      {
        return _AkWrapper ?? throw new InvalidOperationException(ErrorNoExecution);
      }
    }

    /// <summary>
    /// Returns the reflection coefficients that were calculated during the last run of the algorithm.
    /// </summary>
    public IROVector<double> ReflectionCoefficients
    {
      get
      {
        return _kWrapper ?? throw new InvalidOperationException(ErrorNoExecution);
      }
    }

    /// <summary>Mean square of the signal values.</summary>
    public double MeanSquareSignal
    {
      get
      {
        return _meanSquareSignal;
      }
    }

    /// <summary>Root of mean square of the signal values.</summary>
    public double RMSSignal
    {
      get
      {
        return Math.Sqrt(_meanSquareSignal);
      }
    }

    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="numberOfCoefficients">Number of coefficients of the model.</param>
    /// <param name="regularizationFactor">Default 1. Values greater than 1 leads to more and more regularization of the coefficients.</param>
    public void Execute(IReadOnlyList<double> x, int numberOfCoefficients, double regularizationFactor = 1)
    {
      EnsureAllocation(x.Count, numberOfCoefficients);
      var (_, sumXsqr) = Execution(x, numberOfCoefficients, regularizationFactor, this);
      _meanSquareSignal = sumXsqr / x.Count;
    }


    /// <summary>
    /// Predict values towards the end of the vector. The predicted values are then used to predict more values. See remarks for details.
    /// </summary>
    /// <param name="x">Signal which holds at least <see cref="NumberOfCoefficients"/> valid points (the signal window to start the prediction with) from index (firstPoint-NumberOfCoefficents) to (firstPoint-1). The predicted values are then stored in this vector.</param>
    /// <param name="firstPoint">Index of the first point to predict.</param>
    /// <remarks>
    /// The algorithm uses a signal window of <c>NumberOfCoefficients</c> signal points before the <c>firstPoint</c> to predict the value at <c>firstPoint</c>.
    /// Then the window is shifted by one towards the end of the vecctor, hence including the predicted value, and the point at <c>firstPoint+1</c> is predicted. The procedure is repeated until all points to the end of the vector are predicted.
    /// </remarks>
    public void PredictRecursivelyForward(IVector<double> x, int firstPoint)
    {
      PredictRecursivelyForward(x, firstPoint, x.Length - firstPoint);
    }

    /// <summary>
    /// Predict values towards the end of the vector. The predicted values are then used to predict more values. See remarks for details.
    /// </summary>
    /// <param name="x">Signal which holds at least <see cref="NumberOfCoefficients"/> valid points (the signal window to start the prediction with) from index (firstPoint-NumberOfCoefficents) to (firstPoint-1). The predicted values are then stored in this vector.</param>
    /// <param name="firstPoint">Index of the first point to predict.</param>
    /// <param name="count">Number of points to predict.</param>
    /// <remarks>
    /// The algorithm uses a signal window of <c>NumberOfCoefficients</c> signal points before the <c>firstPoint</c> to predict the value at <c>firstPoint</c>.
    /// Then the window is shifted by one towards the end of the vecctor, hence including the predicted value, and the point at <c>firstPoint+1</c> is predicted. The procedure is repeated until <c>count</c> points are predicted.
    /// </remarks>
    public void PredictRecursivelyForward(IVector<double> x, int firstPoint, int count)
    {
      if (_Ak is null)
        throw new InvalidOperationException(ErrorNoExecution);


      int last = firstPoint + count;
      for (int i = firstPoint; i < last; i++)
      {
        double sum = 0;
        for (int k = 1; k <= _numberOfCoefficients; k++) // note that Ak[0] is always 1 for technical reasons, thus we start here with index 1
        {
          sum -= _Ak[k] * x[i - k];
        }
        x[i] = sum;
      }
    }

    /// <summary>
    /// This algorithm determines the mean forward prediction error using the model stored in this instance. See remarks for details.
    /// </summary>
    /// <param name="x">Signal for which to determine the mean forward prediction error.</param>
    /// <returns>Mean backward prediction error.</returns>
    /// <remarks>
    /// 1. The prediction is done non recursively, i.e. part of the signal (the signal window) is used to predict the signal value immediately after the window, and this predicted signal value is
    /// then compared with the original signal value stored in x to build the sum of errors. But the predicted signal value is <b>not</b> used to make further predictions.
    /// Instead, the signal window is moved by one point to the right and another prediction is made, with the original signal in x. This is repeated until the last point
    /// is predicted. The return value is the square root of the sum of squared differences between predicted signal values and original values, divided by the number of predicted values.
    /// The number of predicted values is the length of the signal x minus the number of coefficents of the model.
    /// </remarks>
    public double GetMeanPredictionErrorNonrecursivelyForward(IReadOnlyList<double> x)
    {
      if (_Ak is null)
        throw new InvalidOperationException(ErrorNoExecution);


      int first = _numberOfCoefficients;
      int last = x.Count;
      double sumsqr = 0;
      for (int i = first; i < last; i++)
      {
        double sum = 0;
        for (int k = 1; k <= _numberOfCoefficients; k++) // note that Ak[0] is always 1 for technical reasons, thus we start here with index 1
        {
          sum -= _Ak[k] * x[i - k];
        }
        sumsqr += Square(x[i] - sum);
      }
      return Math.Sqrt(sumsqr / (last - first));
    }

    /// <summary>
    /// Predict values towards the start of the vector. The predicted values are then used to predict more values. See remarks for details.
    /// </summary>
    /// <param name="x">Signal which holds at least <see cref="NumberOfCoefficients"/> valid points (the signal window to start the prediction with) from index (lastPoint+1) to (lastPoint+NumberOfCoefficents). The predicted values are then stored in the first part of this vector from indices (0) to (lastPoint).</param>
    /// <param name="lastPoint">Index of the last point to predict.</param>
    /// <remarks>
    /// The algorithm uses a signal window of <c>NumberOfCoefficients</c> signal points after the <c>lastPoint</c> to predict the value at <c>lastPoint</c>.
    /// Then the window is shifted by one towards the start of the vecctor, hence including the predicted value, and the point at <c>lastPoint-1</c> is predicted. The procedure is repeated until the value at index 0 is predicted.
    /// </remarks>
    public void PredictRecursivelyBackward(IVector<double> x, int lastPoint)
    {
      PredictRecursivelyBackward(x, lastPoint, lastPoint + 1);
    }

    /// <summary>
    /// Predict values towards the start of the vector. The predicted values are then used to predict more values. See remarks for details.
    /// </summary>
    /// <param name="x">Signal which holds at least <see cref="NumberOfCoefficients"/> valid points (the signal window to start the prediction with) from index (lastPoint+1) to (lastPoint+NumberOfCoefficents). The predicted values are then stored in the first part of this vector from indices (lastPoint-count+1) to (lastPoint).</param>
    /// <param name="lastPoint">Index of the last point to predict.</param>
    /// <param name="count">Number of points to predict.</param>
    /// <remarks>
    /// The algorithm uses a signal window of <c>NumberOfCoefficients</c> signal points after the <c>lastPoint</c> to predict the value at <c>lastPoint</c>.
    /// Then the window is shifted by one towards the start of the vecctor, hence including the predicted value, and the point at <c>lastPoint-1</c> is predicted. The procedure is repeated until <c>count</c> points are predicted.
    /// </remarks>
    public void PredictRecursivelyBackward(IVector<double> x, int lastPoint, int count)
    {
      if (_Ak is null)
        throw new InvalidOperationException(ErrorNoExecution);

      int first = lastPoint - count;
      for (int i = lastPoint; i > first; i--)
      {
        double sum = 0;
        for (int k = 1; k <= _numberOfCoefficients; k++) // note that Ak[0] is always 1 for technical reasons, thus we start here with index 1
        {
          sum -= _Ak[k] * x[i + k];
        }
        x[i] = sum;
      }
    }

    /// <summary>
    /// This algorithm determines the mean backward prediction error using the model stored in this instance. See remarks for details.
    /// </summary>
    /// <param name="x">Signal for which to determine the mean backward prediction error.</param>
    /// <returns>Mean backward prediction error.</returns>
    /// <remarks>
    /// 1. The prediction is done non recursively, i.e. part of the signal (the signal window) is used to predict the signal value before, and this predicted signal value is
    /// then compared with the original signal value stored in x to build the sum of errors. But the predicted signal value is <b>not</b> used to make further predictions.
    /// Instead, the signal window is moved by one point to the left and another prediction is made, with the original signal in x. This is repeated until the first point (index 0)
    /// is predicted. The return value is the square root of the sum of squared differences between predicted signal values and original values, divided by the number of predicted values.
    /// The number of predicted values is the length of the signal x minus the number of coefficents of the model.
    /// </remarks>
    public double GetMeanPredictionErrorNonrecursivelyBackward(IReadOnlyList<double> x)
    {
      if (_Ak is null)
        throw new InvalidOperationException(ErrorNoExecution);

      int last = x.Count - _numberOfCoefficients;
      double sumsqr = 0;
      for (int i = last - 1; i >= 0; i--)
      {
        double sum = 0;
        for (int k = 1; k <= _numberOfCoefficients; k++) // note that Ak[0] is always 1 for technical reasons, thus we start here with index 1
        {
          sum -= _Ak[k] * x[i + k];
        }
        sumsqr += Square(x[i] - sum);
      }
      return Math.Sqrt(sumsqr / (last));
    }

    /// <summary>
    /// Calculates the frequency response for a given frequency.
    /// </summary>
    /// <param name="fdt">Frequency. Must be given as f*dt, meaning the product of frequency and sample interval.</param>
    /// <returns>The complex frequency response at the given frequency.</returns>
    public virtual double GetFrequencyResponse(double fdt)
    {
      var Ak = _Ak ?? throw new InvalidOperationException(ErrorNoExecution);

      double w = fdt * 2 * Math.PI;
      Complex z = new Complex(Math.Cos(w), Math.Sin(w));
      var zz = z;

      Complex denom = Ak[1] * zz;
      for (int i = 2; i < Ak.Length; ++i)
      {
        zz *= z;
        denom += Ak[i] * zz;
      }

      // for the overall amplitude, take into account the root mean square of the signal
      return Math.Sqrt(_meanSquareSignal) / (1+denom).GetModulusSquared();
    }


    /// <summary>
    /// Ensures that temporary arrays are allocated in order to execute the Burg algorithm.
    /// </summary>
    /// <param name="xLength">Length of the vector to build the model.</param>
    /// <param name="coeffLength">Number of parameters of the model.</param>
    [MemberNotNull(nameof(_Ak), nameof(_Ak_previous), nameof(_AkWrapper), nameof(_k), nameof(_kWrapper), nameof(_g), nameof(_g_previous),  nameof(_r), nameof(_c), nameof(_deltaRTimesAk))]
    private void EnsureAllocation(int xLength, int coeffLength)
    {
      _numberOfCoefficients = coeffLength;

      if (_Ak is null || _Ak_previous is null || _AkWrapper is null || _Ak.Length < coeffLength + 1)
      {
        _Ak = new double[coeffLength + 1];
        _Ak_previous = new double[coeffLength + 1];
      }

      if (_AkWrapper is null || _numberOfCoefficients != _AkWrapper.Length)
      {
        _AkWrapper = VectorMath.ToROVector(_Ak, 1, _numberOfCoefficients);
      }

      if (_k is null || _k.Length < (coeffLength + 1))
      {
        _k = new double[coeffLength + 1];
      }

      if (_kWrapper is null || _numberOfCoefficients != _kWrapper.Length)
      {
        _kWrapper = VectorMath.ToROVector(_k, 1, _numberOfCoefficients);
      }

      if (_g is null || _g_previous is null || _g.Length < (coeffLength + 2))
      {
        _g = new double[coeffLength + 2];
        _g_previous = new double[coeffLength + 2];
      }

      if (_r is null || _r.Length < (coeffLength + 1))
      {
        _r = new double[coeffLength + 1];
      }

      if (_c is null || _c.Length < (coeffLength + 1))
      {
        _c = new double[coeffLength + 1];
      }

      if (_deltaRTimesAk is null || _deltaRTimesAk.Length < (coeffLength + 1))
      {
        _deltaRTimesAk = new double[coeffLength + 1];
      }
    }

    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="numberOfCoefficients">Number of coefficients of the model.</param>
    /// <param name="regularizationFactor">Default 1. Values greater than 1 leads to more and more regularization of the coefficients.</param>
    /// <returns>The coefficient array, and the sum of squared signal values.</returns>

    public static (IROVector<double> Ak, double SumXsqr) Execution(IReadOnlyList<double> x, int numberOfCoefficients, double regularizationFactor=1)
    {
      var (Ak, SumXsqr) = Execution(x, numberOfCoefficients, regularizationFactor, null);
      return (VectorMath.ToROVector(Ak, 1, numberOfCoefficients), SumXsqr);
    }

    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="coefficients">Vector of coefficients to be filled.</param>
    /// <param name="regularizationFactor">Default 1. Values greater than 1 leads to more and more regularization of the coefficients.</param>
    /// <returns>The sum of squared signal values.</returns>
    public static double Execution(IReadOnlyList<double> x, IVector<double> coefficients, double regularizationFactor=1)
    {
      var (Ak, SumXsqr) = Execution(x, coefficients.Length, regularizationFactor, null);
      for (int i = 0; i < coefficients.Length; ++i)
        coefficients[i] = Ak[i + 1];
      return SumXsqr;
    }

    


    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="numberOfCoefficients">Number of coefficients of the model.</param>
    /// <param name="regularizationFactor">Default 1. Values greater than 1 leads to more and more regularization of the coefficients.</param>
    /// <param name="tempStorage">Instance of this class used to hold the temporary arrays.</param>
    /// <returns>The coefficient array. Attention: this is 1 longer than number of coefficients, because the coefficients start at index 1. Furthermofe, the sum of squared signal is included.</returns>
    private static (double[] Ak, double SumXsqr) Execution(IReadOnlyList<double> x, int numberOfCoefficients, double regularizationFactor, BurgAlgorithmVos? tempStorage)
    {
      int N = x.Count - 1; // index of the last valid point of the signal 
      int m = numberOfCoefficients; // number of coefficients

      double[] Ak; // Prediction coefficients, Ak[0] is always 1
      double[] Ak_previous; // Previous prediction coefficients
      double[] c; // Autocorrelation array
      double[] g;
      double[] g_previous;
      double[] k;
      double[] r;
      double[] deltaRTimesAk;

      if (tempStorage is not null)
      {
        tempStorage.EnsureAllocation(x.Count, numberOfCoefficients);
        Ak = tempStorage._Ak;
        Ak_previous = tempStorage._Ak_previous;
        c = tempStorage._c;
        k = tempStorage._k;
        g = tempStorage._g;
        g_previous = tempStorage._g_previous;
        r = tempStorage._r;
        deltaRTimesAk = tempStorage._deltaRTimesAk;
      }
      else
      {
        Ak = new double[numberOfCoefficients + 1];
        Ak_previous = new double[numberOfCoefficients + 1];
        c = new double[numberOfCoefficients + 1];
        k = new double[numberOfCoefficients + 1];
        g = new double[numberOfCoefficients + 2];
        g_previous = new double[numberOfCoefficients + 2];
        r = new double[numberOfCoefficients + 1];
        deltaRTimesAk = new double[numberOfCoefficients + 1];
      }

      /*
      // For testing, that not uninitialized parts of the arrays are used, set all array elements to NaN
      NaNArray(Ak);
      NaNArray(Ak_previous);
      NaNArray(c);
      NaNArray(k);
      NaNArray(g);
      NaNArray(g_previous);
      NaNArray(r);
      NaNArray(deltaRTimesAk);
      */
      

      // Step1: Initialization (26) .. (30) in [1]
      // Calculate autocorrelation array c
      // Note that c[0] contains sum of the sqared signal, which can be used to get the rms value of the signal
      for (int j = 0; j <= m; j++)
      {
        c[j] = 0;
        for (int i = 0; i <= N - j; ++i)
          c[j] += x[i] * x[i + j];
      }
      var sumXsqr = c[0]; // save it for the return value;
      c[0] *= regularizationFactor;


      Ak[0] = 1;
      g[0] = 2 * c[0] - x[0] *x[0] -x[N] *x[N];
      g[1] = 2 * c[1];
      r[0] = 2 * c[1]; // in contrast to (3) in [1], this must be index 0


      // Start the iterations
      for (int idxIteration = 0; idxIteration <= m;) // iteration counter is incremented in the middle of the loop
      {
        // Compute the reflection coefficients, see (31) in [1]
        {
          var nominator = 0.0;
          var denominator = 1/double.MaxValue; // in order to avoid division by zero

          for (int i = 0; i <= idxIteration; ++i)
          {
            nominator += Ak[i] * g[idxIteration + 1 - i]; // exchange order see (12) in [1]
            denominator += Ak[i] * g[i];
          }

          k[idxIteration] = -nominator / denominator;
        }


        // update the prediction coefficients, see (32) in [1]
        {
          // Swap Ak arrays
          var Ak_temp = Ak;
          Ak = Ak_previous;
          Ak_previous = Ak_temp;
          Ak_previous[idxIteration + 1] = 0;

          for (int i = 0; i <= idxIteration + 1; ++i)
          {
            Ak[i] = Ak_previous[i] + k[idxIteration] * Ak_previous[idxIteration + 1 - i]; // exchanged order see (12) in [1]
          }
        }

        ++idxIteration;
        if ((idxIteration) == m)
          break;

        // update r array, see step 5. of algorithm and eq. (24) in [1]
        {
          for (int i = idxIteration-1; i >=0; --i) // downcounting, in order not to overwrite the value we need next
          {
            r[i + 1] = r[i] - x[i] * x[idxIteration] -  x[N - i] * x[N - idxIteration];
          }
          r[0] = 2 * c[idxIteration + 1];
        }

        // compute deltaR*Ak, see step 6 of algorithm and eq. (23) in [1]
        {
          var iStart = idxIteration;
          var iEnd = N - idxIteration;

          for (int iRow = 0; iRow <= idxIteration; ++iRow)
          {
            double innerProduct1 = 0;
            double innerProduct2 = 0;
            for (int iCol = 0; iCol <= idxIteration; ++iCol)
            {
              innerProduct1 += x[iStart - iCol] * Ak[iCol];
              innerProduct2 += x[iEnd + iCol] * Ak[iCol];
            }
            deltaRTimesAk[iRow] = -x[iStart - iRow] *  innerProduct1 - x[iEnd + iRow] * innerProduct2;
          }
        }

        // update g, see step 7 of algorithm and eq. (22) in [1]
        {
          // swap g
          var g_temp = g;
          g = g_previous;
          g_previous = g_temp;

          // g.Length is i_iterationCounter + 1
          for (int i = 0; i <= idxIteration; ++i)
          {
            g[i] = g_previous[i] + k[idxIteration - 1] * g_previous[idxIteration-i] + deltaRTimesAk[i]; // exchanged order see (12) in [1]
          }

          double sum = 0;
          for (int i = 0; i <= idxIteration; ++i)
          {
            sum += r[i] * Ak[i];
          }
          g[idxIteration + 1] = sum;
        }
      }

      // because we have swapped the arrays, unswapping would be neccessary if we use the tempStorage
      // since this would rise problems with Ak wrapper, we simply copy the Ak array
      if (tempStorage is not null && !object.ReferenceEquals(Ak, tempStorage._Ak))
      {
        // because we have swapped the arrays, unswapping would be neccessary if we use the tempStorage
        // since this would rise problems with the Ak wrapper, we simply copy the Ak array
        Array.Copy(Ak, Ak_previous, Ak.Length);
      }

      return (Ak, sumXsqr);
    }

    /// <summary>Square of x.</summary>
    /// <param name="x">x</param>
    /// <returns>Square of x.</returns>
    private static double Square(double x)
    {
      return x * x;
    }

    /*
    static void NaNArray(double[] array)
    {
      for (int i = 0; i < array.Length; ++i)
        array[i] = double.NaN;
    }
    */
  }
}
