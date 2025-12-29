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
using Altaxo.Calc.LinearAlgebra;
using Complex64T = System.Numerics.Complex;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// A faster algorithm than <see cref="BurgAlgorithm"/>, if the number of coefficients is less than 1/3 the length of the signal.
  /// This algorithm is based on a paper by Koen Vos, 2013.
  /// </summary>
  /// <remarks>
  /// <para>Literature:</para>
  /// <para>[1] Koen Vos, "A Fast Implementation of Burg's method", August 2013, Creative Commons.</para>
  /// </remarks>
  public class BurgAlgorithmVos
  {
    private const string ErrorNoExecution = "No results yet - call Execute first!";

    /// <summary>
    /// Prediction coefficients, see eq. (28) in [1].
    /// Note that for technical reasons <c>_Ak[0]</c> is always 1 and the calculated coefficients start with <c>_Ak[1]</c>.
    /// </summary>
    protected double[]? _Ak;

    /// <summary>Swapping array for <see cref="_Ak"/>.</summary>
    protected double[]? _Ak_previous;

    /// <summary>Wrapper for the coefficients that can be returned by <see cref="Coefficients"/>.</summary>
    private IReadOnlyList<double>? _AkWrapper;

    /// <summary>Number of coefficients that were calculated.</summary>
    private int _numberOfCoefficients;

    /// <summary>Mean square of the signal values.</summary>
    private double _meanSquareSignal;

    /// <summary>Prediction coefficients, see eq. (22) in [1].</summary>
    protected double[]? _g;

    /// <summary>Swapping array for <see cref="_g"/>.</summary>
    protected double[]? _g_previous;

    /// <summary>Reflection coefficients.</summary>
    protected double[]? _k;

    /// <summary>Wrapper for the reflection coefficients that can be returned by <see cref="ReflectionCoefficients"/>.</summary>
    private IReadOnlyList<double>? _kWrapper;

    /// <summary>Temporary array used by the algorithm.</summary>
    protected double[]? _r;

    /// <summary>Array containing the autocorrelation values, see eq. (26) in [1].</summary>
    protected double[]? _c;

    /// <summary>Product of matrix ΔR and prediction coefficients Ak, see eq. (23) in [1].</summary>
    private double[]? _deltaRTimesAk;

    /// <summary>
    /// Gets the number of coefficients that were used for the last run of the algorithm.
    /// </summary>
    public int NumberOfCoefficients
    {
      get
      {
        return _numberOfCoefficients;
      }
    }

    /// <summary>
    /// Gets the coefficients that were calculated during the last run of the algorithm.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the algorithm has not been executed yet.
    /// </exception>
    public IReadOnlyList<double> Coefficients
    {
      get
      {
        return _AkWrapper ?? throw new InvalidOperationException(ErrorNoExecution);
      }
    }

    /// <summary>
    /// Gets the reflection coefficients that were calculated during the last run of the algorithm.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the algorithm has not been executed yet.
    /// </exception>
    public IReadOnlyList<double> ReflectionCoefficients
    {
      get
      {
        return _kWrapper ?? throw new InvalidOperationException(ErrorNoExecution);
      }
    }

    /// <summary>
    /// Gets the mean square of the signal values.
    /// </summary>
    public double MeanSquareSignal
    {
      get
      {
        return _meanSquareSignal;
      }
    }

    /// <summary>
    /// Gets the root mean square (RMS) of the signal values.
    /// </summary>
    public double RMSSignal
    {
      get
      {
        return Math.Sqrt(_meanSquareSignal);
      }
    }

    /// <summary>
    /// Uses the signal in <paramref name="x"/> to build a model with <paramref name="numberOfCoefficients"/> parameters.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="numberOfCoefficients">Number of coefficients of the model.</param>
    /// <param name="regularizationFactor">
    /// Default is 1.
    /// Values greater than 1 lead to increasing regularization of the coefficients.
    /// </param>
    public void Execute(IReadOnlyList<double> x, int numberOfCoefficients, double regularizationFactor = 1)
    {
      EnsureAllocation(x.Count, numberOfCoefficients);
      var (_, sumXsqr) = Execution(x, numberOfCoefficients, regularizationFactor, this);
      _meanSquareSignal = sumXsqr / x.Count;
    }


    /// <summary>
    /// Predicts values towards the end of the vector.
    /// The predicted values are then used to predict further values.
    /// </summary>
    /// <param name="x">
    /// Signal which holds at least <see cref="NumberOfCoefficients"/> valid points (the signal window to start the prediction with)
    /// from index <c>(firstPoint - NumberOfCoefficients)</c> to <c>(firstPoint - 1)</c>. The predicted values are stored into this
    /// vector.
    /// </param>
    /// <param name="firstPoint">Index of the first point to predict.</param>
    /// <remarks>
    /// The algorithm uses a signal window of <see cref="NumberOfCoefficients"/> signal points before <paramref name="firstPoint"/>
    /// to predict the value at <paramref name="firstPoint"/>.
    /// Then the window is shifted by one towards the end of the vector (thus including the predicted value), and the point at
    /// <c>firstPoint + 1</c> is predicted.
    /// The procedure is repeated until all points to the end of the vector are predicted.
    /// </remarks>
    public void PredictRecursivelyForward(IVector<double> x, int firstPoint)
    {
      PredictRecursivelyForward(x, firstPoint, x.Count - firstPoint);
    }

    /// <summary>
    /// Predicts values towards the end of the vector.
    /// The predicted values are then used to predict further values.
    /// </summary>
    /// <param name="x">
    /// Signal which holds at least <see cref="NumberOfCoefficients"/> valid points (the signal window to start the prediction with)
    /// from index <c>(firstPoint - NumberOfCoefficients)</c> to <c>(firstPoint - 1)</c>. The predicted values are stored into this
    /// vector.
    /// </param>
    /// <param name="firstPoint">Index of the first point to predict.</param>
    /// <param name="count">Number of points to predict.</param>
    /// <remarks>
    /// The algorithm uses a signal window of <see cref="NumberOfCoefficients"/> signal points before <paramref name="firstPoint"/>
    /// to predict the value at <paramref name="firstPoint"/>.
    /// Then the window is shifted by one towards the end of the vector (thus including the predicted value), and the point at
    /// <c>firstPoint + 1</c> is predicted.
    /// The procedure is repeated until <paramref name="count"/> points are predicted.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the algorithm has not been executed yet.
    /// </exception>
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
    /// Determines the mean forward prediction error using the model stored in this instance.
    /// </summary>
    /// <param name="x">Signal for which to determine the mean forward prediction error.</param>
    /// <returns>Mean forward prediction error.</returns>
    /// <remarks>
    /// <para>
    /// The prediction is done non-recursively, i.e. a part of the signal (the signal window) is used to predict the signal value
    /// immediately after the window, and this predicted signal value is then compared with the original signal value stored in
    /// <paramref name="x"/> to build the sum of errors.
    /// </para>
    /// <para>
    /// The predicted signal value is <b>not</b> used to make further predictions. Instead, the signal window is moved by one point
    /// to the right and another prediction is made, using the original signal in <paramref name="x"/>. This is repeated until the
    /// last point is predicted.
    /// </para>
    /// <para>
    /// The return value is the square root of the sum of squared differences between predicted signal values and original values,
    /// divided by the number of predicted values. The number of predicted values is the length of the signal <paramref name="x"/>
    /// minus the number of coefficients of the model.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the algorithm has not been executed yet.
    /// </exception>
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
    /// Predicts values towards the start of the vector.
    /// The predicted values are then used to predict further values.
    /// </summary>
    /// <param name="x">
    /// Signal which holds at least <see cref="NumberOfCoefficients"/> valid points (the signal window to start the prediction with)
    /// from index <c>(lastPoint + 1)</c> to <c>(lastPoint + NumberOfCoefficients)</c>. The predicted values are stored in the first
    /// part of this vector from indices <c>0</c> to <paramref name="lastPoint"/>.
    /// </param>
    /// <param name="lastPoint">Index of the last point to predict.</param>
    /// <remarks>
    /// The algorithm uses a signal window of <see cref="NumberOfCoefficients"/> signal points after <paramref name="lastPoint"/>
    /// to predict the value at <paramref name="lastPoint"/>.
    /// Then the window is shifted by one towards the start of the vector (thus including the predicted value), and the point at
    /// <c>lastPoint - 1</c> is predicted.
    /// The procedure is repeated until the value at index 0 is predicted.
    /// </remarks>
    public void PredictRecursivelyBackward(IVector<double> x, int lastPoint)
    {
      PredictRecursivelyBackward(x, lastPoint, lastPoint + 1);
    }

    /// <summary>
    /// Predicts values towards the start of the vector.
    /// The predicted values are then used to predict further values.
    /// </summary>
    /// <param name="x">
    /// Signal which holds at least <see cref="NumberOfCoefficients"/> valid points (the signal window to start the prediction with)
    /// from index <c>(lastPoint + 1)</c> to <c>(lastPoint + NumberOfCoefficients)</c>. The predicted values are stored in the first
    /// part of this vector from indices <c>(lastPoint - count + 1)</c> to <paramref name="lastPoint"/>.
    /// </param>
    /// <param name="lastPoint">Index of the last point to predict.</param>
    /// <param name="count">Number of points to predict.</param>
    /// <remarks>
    /// The algorithm uses a signal window of <see cref="NumberOfCoefficients"/> signal points after <paramref name="lastPoint"/>
    /// to predict the value at <paramref name="lastPoint"/>.
    /// Then the window is shifted by one towards the start of the vector (thus including the predicted value), and the point at
    /// <c>lastPoint - 1</c> is predicted.
    /// The procedure is repeated until <paramref name="count"/> points are predicted.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the algorithm has not been executed yet.
    /// </exception>
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
    /// Determines the mean backward prediction error using the model stored in this instance.
    /// </summary>
    /// <param name="x">Signal for which to determine the mean backward prediction error.</param>
    /// <returns>Mean backward prediction error.</returns>
    /// <remarks>
    /// <para>
    /// The prediction is done non-recursively, i.e. a part of the signal (the signal window) is used to predict the signal value
    /// before the window, and this predicted signal value is then compared with the original signal value stored in
    /// <paramref name="x"/> to build the sum of errors.
    /// </para>
    /// <para>
    /// The predicted signal value is <b>not</b> used to make further predictions. Instead, the signal window is moved by one point
    /// to the left and another prediction is made, using the original signal in <paramref name="x"/>. This is repeated until the
    /// first point (index 0) is predicted.
    /// </para>
    /// <para>
    /// The return value is the square root of the sum of squared differences between predicted signal values and original values,
    /// divided by the number of predicted values. The number of predicted values is the length of the signal <paramref name="x"/>
    /// minus the number of coefficients of the model.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the algorithm has not been executed yet.
    /// </exception>
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
    /// <param name="fdt">
    /// Frequency specified as <c>f * dt</c>, i.e. the product of frequency and sample interval.
    /// </param>
    /// <returns>The (real-valued) power spectrum estimate at the given frequency.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the algorithm has not been executed yet.
    /// </exception>
    public virtual double GetFrequencyResponse(double fdt)
    {
      var Ak = _Ak ?? throw new InvalidOperationException(ErrorNoExecution);

      double w = fdt * 2 * Math.PI;
      Complex64T z = new Complex64T(Math.Cos(w), Math.Sin(w));
      var zz = z;

      Complex64T denom = Ak[1] * zz;
      for (int i = 2; i < Ak.Length; ++i)
      {
        zz *= z;
        denom += Ak[i] * zz;
      }

      // for the overall amplitude, take into account the root mean square of the signal
      return Math.Sqrt(_meanSquareSignal) / (1 + denom).MagnitudeSquared();
    }


    /// <summary>
    /// Ensures that temporary arrays are allocated in order to execute the Burg algorithm.
    /// </summary>
    /// <param name="xLength">Length of the vector to build the model.</param>
    /// <param name="coeffLength">Number of parameters of the model.</param>
    [MemberNotNull(nameof(_Ak))]
    [MemberNotNull(nameof(_Ak_previous))]
    [MemberNotNull(nameof(_AkWrapper))]
    [MemberNotNull(nameof(_k))]
    [MemberNotNull(nameof(_kWrapper))]
    [MemberNotNull(nameof(_g))]
    [MemberNotNull(nameof(_g_previous))]
    [MemberNotNull(nameof(_r))]
    [MemberNotNull(nameof(_c))]
    [MemberNotNull(nameof(_deltaRTimesAk))]
    private void EnsureAllocation(int xLength, int coeffLength)
    {
      _numberOfCoefficients = coeffLength;

      if (_Ak is null || _Ak_previous is null || _AkWrapper is null || _Ak.Length < coeffLength + 1)
      {
        _Ak = new double[coeffLength + 1];
        _Ak_previous = new double[coeffLength + 1];
      }

      if (_AkWrapper is null || _numberOfCoefficients != _AkWrapper.Count)
      {
        _AkWrapper = VectorMath.ToROVector(_Ak, 1, _numberOfCoefficients);
      }

      if (_k is null || _k.Length < (coeffLength + 1))
      {
        _k = new double[coeffLength + 1];
      }

      if (_kWrapper is null || _numberOfCoefficients != _kWrapper.Count)
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
    /// Uses the signal in <paramref name="x"/> to build a model with <paramref name="numberOfCoefficients"/> parameters.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="numberOfCoefficients">Number of coefficients of the model.</param>
    /// <param name="regularizationFactor">
    /// Default is 1.
    /// Values greater than 1 lead to increasing regularization of the coefficients.
    /// </param>
    /// <returns>The coefficient vector (starting at index 1) and the sum of squared signal values.</returns>
    public static (IReadOnlyList<double> Ak, double SumXsqr) Execution(IReadOnlyList<double> x, int numberOfCoefficients, double regularizationFactor = 1)
    {
      var (Ak, SumXsqr) = Execution(x, numberOfCoefficients, regularizationFactor, null);
      return (VectorMath.ToROVector(Ak, 1, numberOfCoefficients), SumXsqr);
    }

    /// <summary>
    /// Uses the signal in <paramref name="x"/> to build a model with as many parameters as there is space in <paramref name="coefficients"/>.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="coefficients">Vector of coefficients to be filled.</param>
    /// <param name="regularizationFactor">
    /// Default is 1.
    /// Values greater than 1 lead to increasing regularization of the coefficients.
    /// </param>
    /// <returns>The sum of squared signal values.</returns>
    public static double Execution(IReadOnlyList<double> x, IVector<double> coefficients, double regularizationFactor = 1)
    {
      var (Ak, SumXsqr) = Execution(x, coefficients.Count, regularizationFactor, null);
      for (int i = 0; i < coefficients.Count; ++i)
        coefficients[i] = Ak[i + 1];
      return SumXsqr;
    }




    /// <summary>
    /// Uses the signal in <paramref name="x"/> to build a model with <paramref name="numberOfCoefficients"/> parameters.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="numberOfCoefficients">Number of coefficients of the model.</param>
    /// <param name="regularizationFactor">Regularization factor.</param>
    /// <param name="tempStorage">Instance of this class used to hold the temporary arrays.</param>
    /// <returns>
    /// The coefficient array.
    /// Note that this is one longer than the number of coefficients, because the coefficients start at index 1.
    /// The sum of squared signal values is returned as well.
    /// </returns>
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
      g[0] = 2 * c[0] - x[0] * x[0] - x[N] * x[N];
      g[1] = 2 * c[1];
      r[0] = 2 * c[1]; // in contrast to (3) in [1], this must be index 0


      // Start the iterations
      for (int idxIteration = 0; idxIteration <= m;) // iteration counter is incremented in the middle of the loop
      {
        // Compute the reflection coefficients, see (31) in [1]
        {
          var nominator = 0.0;
          var denominator = 1 / double.MaxValue; // in order to avoid division by zero

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

        // update r array, see step 5 of algorithm and eq. (24) in [1]
        {
          for (int i = idxIteration - 1; i >= 0; --i) // downcounting, in order not to overwrite the value we need next
          {
            r[i + 1] = r[i] - x[i] * x[idxIteration] - x[N - i] * x[N - idxIteration];
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
            deltaRTimesAk[iRow] = -x[iStart - iRow] * innerProduct1 - x[iEnd + iRow] * innerProduct2;
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
            g[i] = g_previous[i] + k[idxIteration - 1] * g_previous[idxIteration - i] + deltaRTimesAk[i]; // exchanged order see (12) in [1]
          }

          double sum = 0;
          for (int i = 0; i <= idxIteration; ++i)
          {
            sum += r[i] * Ak[i];
          }
          g[idxIteration + 1] = sum;
        }
      }

      // because we have swapped the arrays, unswapping would be necessary if we use the tempStorage
      // since this would rise problems with Ak wrapper, we simply copy the Ak array
      if (tempStorage is not null && !object.ReferenceEquals(Ak, tempStorage._Ak))
      {
        // because we have swapped the arrays, unswapping would be neccessary if we use the tempStorage
        // since this would rise problems with the Ak wrapper, we simply copy the Ak array
        Array.Copy(Ak, Ak_previous, Ak.Length);
      }

      return (Ak, sumXsqr);
    }

    /// <summary>
    /// Returns the square of <paramref name="x"/>.
    /// </summary>
    /// <param name="x">Value.</param>
    /// <returns>The square of <paramref name="x"/>.</returns>
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
