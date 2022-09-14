#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Complex64T = System.Numerics.Complex;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Implements Burg's algorithm with complex numbers.
  /// </summary>
  public class BurgAlgorithmComplex
  {
    private const string ErrorNoExecution = "No results yet - call Execute first!";

    /// <summary>Forward prediction errors.</summary>
    private Complex64T[]? _f;

    /// <summary>Backward prediction errors.</summary>
    private Complex64T[]? _b;

    /// <summary>Prediction coefficients. Note that for technical reasons _Ak[0] is always 1 and the calculated coefficients start with _Ak[1].</summary>
    private Complex64T[]? _Ak;

    /// <summary>Wrapper for the coefficients that can be returned by <see cref="Coefficients"/>.</summary>
    private ComplexVectorWrapper? _AkWrapper;

    /// <summary>Number of coefficients that were calculated.</summary>
    private int _numberOfCoefficients;

    /// <summary>Mean square error calculated during the last run.</summary>
    private double _meanSquareError;

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
    public IROComplexDoubleVector Coefficients
    {
      get
      {
        if (_AkWrapper is null)
          throw new InvalidOperationException(ErrorNoExecution);

        return _AkWrapper;
      }
    }

    /// <summary>Mean square error calculated during the last run of the algorithm.</summary>
    public double MeanSquareError
    {
      get
      {
        return _meanSquareError;
      }
    }

    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="numberOfCoefficients">Number of coefficients of the model.</param>
    public void Execute(IROComplexDoubleVector x, int numberOfCoefficients)
    {
      EnsureAllocation(x.Length, numberOfCoefficients);
      _meanSquareError = Execution(x, _AkWrapper, null, null, this);
    }

    /// <summary>
    /// Uses th signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
    public void Execute(IROComplexDoubleVector x, IComplexDoubleVector coefficients)
    {
      _meanSquareError = Execution(x, coefficients, null, null, this);
    }

    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
    /// <param name="reflectionCoefficients">Vector to be filled with the reflection coefficients.</param>
    public void Execute(IROComplexDoubleVector x, IComplexDoubleVector coefficients, IComplexDoubleVector reflectionCoefficients)
    {
      _meanSquareError = Execution(x, coefficients, null, reflectionCoefficients, this);
    }

    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
    /// <param name="errors">Vector to be filled with the sum of forward and backward prediction error for every stage of the model.</param>
    public void Execute(IROComplexDoubleVector x, IComplexDoubleVector coefficients, IVector<double> errors)
    {
      _meanSquareError = Execution(x, coefficients, errors, null, this);
    }

    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
    /// <param name="errors">Vector to be filled with the sum of forward and backward prediction error for every stage of the model.</param>
    /// <param name="reflectionCoefficients">Vector to be filled with the reflection coefficients.</param>
    public void Execute(IROComplexDoubleVector x, IComplexDoubleVector coefficients, IVector<double> errors, IComplexDoubleVector reflectionCoefficients)
    {
      _meanSquareError = Execution(x, coefficients, errors, reflectionCoefficients, this);
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
    public void PredictRecursivelyForward(IComplexDoubleVector x, int firstPoint)
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
    public void PredictRecursivelyForward(IComplexDoubleVector x, int firstPoint, int count)
    {
      if (_Ak is null)
        throw new InvalidOperationException(ErrorNoExecution);

      int last = firstPoint + count;
      for (int i = firstPoint; i < last; i++)
      {
        Complex64T sum = 0;
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
    public double GetMeanPredictionErrorNonrecursivelyForward(IROComplexDoubleVector x)
    {
      if (_Ak is null)
        throw new InvalidOperationException(ErrorNoExecution);

      int first = _numberOfCoefficients;
      int last = x.Length;
      double sumsqr = 0;
      for (int i = first; i < last; i++)
      {
        Complex64T sum = 0;
        for (int k = 1; k <= _numberOfCoefficients; k++) // note that Ak[0] is always 1 for technical reasons, thus we start here with index 1
        {
          sum -= _Ak[k] * x[i - k];
        }
        sumsqr += (x[i] - sum).MagnitudeSquared();
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
    public void PredictRecursivelyBackward(IComplexDoubleVector x, int lastPoint)
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
    public void PredictRecursivelyBackward(IComplexDoubleVector x, int lastPoint, int count)
    {
      if (_Ak is null)
        throw new InvalidOperationException(ErrorNoExecution);

      int first = lastPoint - count;
      for (int i = lastPoint; i > first; i--)
      {
        Complex64T sum = 0;
        for (int k = 1; k <= _numberOfCoefficients; k++) // note that Ak[0] is always 1 for technical reasons, thus we start here with index 1
        {
          sum -= _Ak[k].GetConjugate() * x[i + k];
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
    public double GetMeanPredictionErrorNonrecursivelyBackward(IROComplexDoubleVector x)
    {
      if (_Ak is null)
        throw new InvalidOperationException(ErrorNoExecution);

      int last = x.Length - _numberOfCoefficients;
      double sumsqr = 0;
      for (int i = last - 1; i >= 0; i--)
      {
        Complex64T sum = 0;
        for (int k = 1; k <= _numberOfCoefficients; k++) // note that Ak[0] is always 1 for technical reasons, thus we start here with index 1
        {
          sum -= _Ak[k].GetConjugate() * x[i + k];
        }
        sumsqr += (x[i] - sum).MagnitudeSquared();
      }
      return Math.Sqrt(sumsqr / (last));
    }

    /// <summary>
    /// Ensures that temporary arrays are allocated in order to execute the Burg algorithm.
    /// </summary>
    /// <param name="xLength">Length of the vector to build the model.</param>
    /// <param name="coeffLength">Number of parameters of the model.</param>
    [MemberNotNull(nameof(_Ak), nameof(_AkWrapper), nameof(_b), nameof(_f))]
    private void EnsureAllocation(int xLength, int coeffLength)
    {
      if (_AkWrapper is null)
        throw new InvalidOperationException(ErrorNoExecution);

      _numberOfCoefficients = coeffLength;

      if (_Ak is null || _Ak.Length < coeffLength + 1)
      {
        _Ak = new Complex64T[coeffLength + 1];
        _AkWrapper = new ComplexVectorWrapper(_Ak, 1, _numberOfCoefficients);
      }

      if (_numberOfCoefficients != _AkWrapper.Length)
        _AkWrapper = new ComplexVectorWrapper(_Ak, 1, _numberOfCoefficients);

      if (_b is null || _b.Length < xLength)
        _b = new Complex64T[xLength];

      if (_f is null || _f.Length < xLength)
        _f = new Complex64T[xLength];
    }

    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
    /// <returns>The mean square error of backward and forward prediction.</returns>
    public static double Execution(IROComplexDoubleVector x, IComplexDoubleVector coefficients)
    {
      return Execution(x, coefficients, null, null, null);
    }

    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
    /// <param name="reflectionCoefficients">Vector to be filled with the reflection coefficients.</param>
    /// <returns>The mean square error of backward and forward prediction.</returns>
    public static double Execution(IROComplexDoubleVector x, IComplexDoubleVector coefficients, IComplexDoubleVector reflectionCoefficients)
    {
      return Execution(x, coefficients, null, reflectionCoefficients, null);
    }

    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
    /// <param name="errors">Vector to be filled with the sum of forward and backward prediction error for every stage of the model.</param>
    /// <returns>The mean square error of backward and forward prediction.</returns>
    public static double Execution(IROComplexDoubleVector x, IComplexDoubleVector coefficients, IVector<double> errors)
    {
      return Execution(x, coefficients, errors, null, null);
    }

    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
    /// <param name="errors">Vector to be filled with the sum of forward and backward prediction error for every stage of the model.</param>
    /// <param name="reflectionCoefficients">Vector to be filled with the reflection coefficients.</param>
    /// <returns>The mean square error of backward and forward prediction.</returns>
    public static double Execution(IROComplexDoubleVector x, IComplexDoubleVector coefficients, IVector<double> errors, IComplexDoubleVector reflectionCoefficients)
    {
      return Execution(x, coefficients, errors, reflectionCoefficients, null);
    }

    /// <summary>
    /// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
    /// </summary>
    /// <param name="x">Signal for building the model.</param>
    /// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
    /// <param name="errors">Vector to be filled with the sum of forward and backward prediction error for every stage of the model.</param>
    /// <param name="reflectionCoefficients">Vector to be filled with the reflection coefficients.</param>
    /// <param name="tempStorage">Instance of this class used to hold the temporary arrays.</param>
    /// <returns>The mean square error of backward and forward prediction.</returns>
    private static double Execution(IROComplexDoubleVector x, IComplexDoubleVector coefficients, IVector<double>? errors, IComplexDoubleVector? reflectionCoefficients, BurgAlgorithmComplex? tempStorage)
    {
      int N = x.Length - 1;
      int m = coefficients.Length;

      Complex64T[] Ak; // Prediction coefficients, Ak[0] is always 1
      Complex64T[] b; // backward prediction errors
      Complex64T[] f; // forward prediction errors

      if (tempStorage is not null)
      {
        tempStorage.EnsureAllocation(x.Length, coefficients.Length);
        Ak = tempStorage._Ak;
        b = tempStorage._b;
        f = tempStorage._f;
        for (int i = 1; i <= coefficients.Length; i++)
          Ak[i] = 0;
      }
      else
      {
        Ak = new Complex64T[coefficients.Length + 1];
        b = new Complex64T[x.Length];
        f = new Complex64T[x.Length];
      }

      Ak[0] = 1;

      // Initialize forward and backward prediction errors with x
      for (int i = 0; i <= N; i++)
        f[i] = b[i] = x[i];

      double Dk = 0;

      for (int i = 0; i <= N; i++)
        Dk += 2 * f[i].MagnitudeSquared();

      Dk -= f[0].MagnitudeSquared() + b[N].MagnitudeSquared();

      // Burg recursion
      int k;
      double sumE = 0; // error sum
      for (k = 0; (k < m) && (Dk > 0); k++)
      {
        // Compute mu
        Complex64T mu = 0;
        for (int n = 0; n < N - k; n++)
          mu += f[n + k + 1] * b[n].GetConjugate();

        mu *= (-2 / Dk);

        // Update Ak
        for (int n = 0; n <= (k + 1) / 2; n++)
        {
          Complex64T t1 = Ak[n] + mu * Ak[k + 1 - n].GetConjugate();
          Complex64T t2 = Ak[k + 1 - n] + mu * Ak[n].GetConjugate();
          Ak[n] = t1;
          Ak[k + 1 - n] = t2;
        }
        if (reflectionCoefficients is not null)
          reflectionCoefficients[k] = Ak[k + 1];

        // update forward and backward predition error with simultaneous total error calculation
        sumE = 0;
        for (int n = 0; n < N - k; n++)
        {
          Complex64T t1 = f[n + k + 1] + mu * b[n];
          Complex64T t2 = b[n] + mu.GetConjugate() * f[n + k + 1];
          f[n + k + 1] = t1;
          b[n] = t2;
          sumE += t1.MagnitudeSquared() + t2.MagnitudeSquared();
        }
        if (errors is not null)
          errors[k] = sumE / (2 * (N - k));
        // Update Dk
        // Note that it is possible to update Dk without total error calculation because sumE = Dk*(1-mu.GetModulusSquared())
        // but this will render the algorithm numerically unstable especially for higher orders and low noise
        Dk = sumE - (f[k + 1].MagnitudeSquared() + b[N - k - 1].MagnitudeSquared());
      }

      // Assign coefficients
      for (int i = 0; i < m; i++)
        coefficients[i] = Ak[i + 1];

      // Assign the rest of reflection coefficients and errors with zero
      // if not all stages could be calculated because Dk was zero or because of rounding effects smaller than zero
      for (int i = k + 1; i < m; i++)
      {
        if (reflectionCoefficients is not null)
          reflectionCoefficients[i] = 0;
        if (errors is not null)
          errors[i] = 0;
      }

      return sumE / (2 * (N - k));
    }

    /*
        public void ExecuteAlt(IROComplexDoubleVector x, IComplexDoubleVector coefficients, IVector errors, IComplexDoubleVector reflectionCoefficients)
        {
            EnsureAllocation(x.Length, coefficients.Length);
            ExecutionAlt(x, coefficients, errors, reflectionCoefficients);
            for (int i = 0; i < coefficients.Length; i++)
                _Ak[i + 1] = coefficients[i];
        }

        /// <summary>
        /// This is source code adapted from arburg.m of the Octave project. Is is not quite as accurate as the algorithm above.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="coefficients"></param>
        /// <param name="errors"></param>
        public static void ExecutionAlt(IROComplexDoubleVector x, IComplexDoubleVector coefficients, IVector errors, IComplexDoubleVector reflectionCoefficients)
        {
            int N = x.Length;
            int m = coefficients.Length;

            var b = new Complex[N - 1];
            var f = new Complex[N - 1];

            var k = new List<Complex>(); // Reflection coefficients
            var a = new List<Complex>(); // Prediction coefficients
            double v;                    // Error
            var prev_a = new List<Complex>();

            for (int i = 0; i < N - 1; i++)
            {
                f[i] = x[i + 1];
                b[i] = x[i];
            }
            v = 0;
            for (int i = 0; i < N; i++)
                v += x[i].GetModulusSquared();
            v /= N;

            for (int p = 0; p < m; p++)
            {
                Complex last_k = 0;
                double sumf2 = 0, sumb2 = 0;
                for (int i = 0; i < b.Length; i++)
                {
                    sumf2 += f[i].GetModulusSquared();
                    sumb2 += b[i].GetModulusSquared();
                    last_k += f[i] * b[i].GetConjugate();
                }
                last_k *= -2 / (sumf2 + sumb2);

                double new_v = v * (1 - last_k.GetModulusSquared());
                if (errors != null)
                {
                    errors[p] = new_v;
                }

                if (p > 0)
                {
                    for (int i = 0; i < prev_a.Count; i++)
                        a[i] = prev_a[i] + last_k * prev_a[prev_a.Count - 1 - i].GetConjugate();
                    a.Add(last_k);
                }
                else // p==0
                {
                    a.Add(last_k);
                }

                k.Add(last_k);

                // k = [k; last_k]
                v = new_v;
                if (p < m - 1)
                {
                    prev_a = a.ToList();
                    int nn = N - p - 1;
                    var new_f = new Complex[nn - 1];
                    for (int i = 0; i < new_f.Length; i++)
                        new_f[i] = f[i + 1] + last_k * b[i + 1];
                    var new_b = new Complex[nn - 1];
                    for (int i = 0; i < new_b.Length; i++)
                        new_b[i] = b[i] + last_k.GetConjugate() * f[i];
                    f = new_f;
                    b = new_b;
                }
            }

            if (null != coefficients)
            {
                for (int i = 0; i < m; i++)
                    coefficients[i] = a[i];
            }

            if (null != reflectionCoefficients)
            {
                for (int i = 0; i < m; i++)
                    reflectionCoefficients[i] = k[i];
            }
        }
        */

    #region ComplexVectorWrapper

    private class ComplexVectorWrapper : IComplexDoubleVector
    {
      private Complex64T[] _arr;
      private int _start, _count;

      public ComplexVectorWrapper(Complex64T[] arr, int start, int count)
      {
        _arr = arr;
        _start = start;
        _count = count;
      }

      public Complex64T this[int i]
      {
        get
        {
          return _arr[_start + i];
        }
        set
        {
          _arr[_start + i] = value;
        }
      }

      public int Length
      {
        get { return _count; }
      }
    }

    #endregion ComplexVectorWrapper
  }
}
