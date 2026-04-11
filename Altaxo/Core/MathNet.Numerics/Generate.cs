// <copyright file="Generate.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2016 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.Distributions;
using Altaxo.Calc.Random;
using Altaxo.Calc.Threading;
using BigInteger = System.Numerics.BigInteger;
using Complex = System.Numerics.Complex;

namespace Altaxo.Calc
{
  /// <summary>
  /// Provides helper methods to generate numeric samples and sequences.
  /// </summary>
  public static class Generate
  {
    /// <summary>
    /// Generate samples by sampling a function at the provided points.
    /// </summary>
    /// <typeparam name="TA">The type of the input points.</typeparam>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="points">The points at which the function is sampled.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The sampled values.</returns>
    public static T[] Map<TA, T>(TA[] points, Func<TA, T> map)
    {
      var res = new T[points.Length];
      for (int i = 0; i < points.Length; i++)
      {
        res[i] = map(points[i]);
      }
      return res;
    }

    /// <summary>
    /// Generate a sample sequence by sampling a function at the provided point sequence.
    /// </summary>
    /// <typeparam name="TA">The type of the input points.</typeparam>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="points">The points at which the function is sampled.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The sampled value sequence.</returns>
    public static IEnumerable<T> MapSequence<TA, T>(IEnumerable<TA> points, Func<TA, T> map)
    {
      return points.Select(map);
    }

    /// <summary>
    /// Generate samples by sampling a function at the provided points.
    /// </summary>
    /// <typeparam name="TA">The type of the first input points.</typeparam>
    /// <typeparam name="TB">The type of the second input points.</typeparam>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="pointsA">The first set of points.</param>
    /// <param name="pointsB">The second set of points.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The sampled values.</returns>
    public static T[] Map2<TA, TB, T>(TA[] pointsA, TB[] pointsB, Func<TA, TB, T> map)
    {
      if (pointsA.Length != pointsB.Length)
      {
        throw new ArgumentException("The array arguments must have the same length.", nameof(pointsB));
      }

      var res = new T[pointsA.Length];
      for (int i = 0; i < res.Length; i++)
      {
        res[i] = map(pointsA[i], pointsB[i]);
      }
      return res;
    }

    /// <summary>
    /// Generate a sample sequence by sampling a function at the provided point sequence.
    /// </summary>
    /// <typeparam name="TA">The type of the first input points.</typeparam>
    /// <typeparam name="TB">The type of the second input points.</typeparam>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="pointsA">The first set of points.</param>
    /// <param name="pointsB">The second set of points.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The sampled value sequence.</returns>
    public static IEnumerable<T> Map2Sequence<TA, TB, T>(IEnumerable<TA> pointsA, IEnumerable<TB> pointsB, Func<TA, TB, T> map)
    {
      return pointsA.Zip(pointsB, map);
    }

    /// <summary>
    /// Generate a linearly spaced sample vector of the given length between the specified values (inclusive).
    /// Equivalent to MATLAB linspace but with the length as first instead of last argument.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="start">The start value of the range.</param>
    /// <param name="stop">The end value of the range.</param>
    /// <returns>The linearly spaced samples.</returns>
    public static double[] LinearSpaced(int length, double start, double stop)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      if (length == 0) return Array.Empty<double>();
      if (length == 1) return new[] { stop };

      double step = (stop - start) / (length - 1);

      var data = new double[length];
      for (int i = 0; i < data.Length; i++)
      {
        data[i] = start + i * step;
      }
      data[data.Length - 1] = stop;
      return data;
    }

    /// <summary>
    /// Generate samples by sampling a function at linearly spaced points between the specified values (inclusive).
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="start">The start value of the range.</param>
    /// <param name="stop">The end value of the range.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The sampled values.</returns>
    public static T[] LinearSpacedMap<T>(int length, double start, double stop, Func<double, T> map)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      if (length == 0) return Array.Empty<T>();
      if (length == 1) return new[] { map(stop) };

      double step = (stop - start) / (length - 1);

      var data = new T[length];
      for (int i = 0; i < data.Length; i++)
      {
        data[i] = map(start + i * step);
      }
      data[data.Length - 1] = map(stop);
      return data;
    }

    /// <summary>
    /// Generate a base 10 logarithmically spaced sample vector of the given length between the specified decade exponents (inclusive).
    /// Equivalent to MATLAB logspace but with the length as first instead of last argument.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="startExponent">The start exponent of the range.</param>
    /// <param name="stopExponent">The end exponent of the range.</param>
    /// <returns>The logarithmically spaced samples.</returns>
    public static double[] LogSpaced(int length, double startExponent, double stopExponent)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      if (length == 0) return Array.Empty<double>();
      if (length == 1) return new[] { Math.Pow(10, stopExponent) };

      double step = (stopExponent - startExponent) / (length - 1);

      var data = new double[length];
      for (int i = 0; i < data.Length; i++)
      {
        data[i] = Math.Pow(10, startExponent + i * step);
      }
      data[data.Length - 1] = Math.Pow(10, stopExponent);
      return data;
    }

    /// <summary>
    /// Generate samples by sampling a function at base 10 logarithmically spaced points between the specified decade exponents (inclusive).
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="startExponent">The start exponent of the range.</param>
    /// <param name="stopExponent">The end exponent of the range.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The sampled values.</returns>
    public static T[] LogSpacedMap<T>(int length, double startExponent, double stopExponent, Func<double, T> map)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      if (length == 0) return Array.Empty<T>();
      if (length == 1) return new[] { map(Math.Pow(10, stopExponent)) };

      double step = (stopExponent - startExponent) / (length - 1);

      var data = new T[length];
      for (int i = 0; i < data.Length; i++)
      {
        data[i] = map(Math.Pow(10, startExponent + i * step));
      }
      data[data.Length - 1] = map(Math.Pow(10, stopExponent));
      return data;
    }

    /// <summary>
    /// Generate a linearly spaced sample vector within the inclusive interval (start, stop) and step 1.
    /// Equivalent to MATLAB colon operator (:).
    /// </summary>
    /// <param name="start">The start value of the range.</param>
    /// <param name="stop">The end value of the range.</param>
    /// <returns>The generated sample vector.</returns>
    public static double[] LinearRange(int start, int stop)
    {
      if (start == stop) return new double[] { start };
      if (start < stop)
      {
        var data = new double[stop - start + 1];
        for (int i = 0; i < data.Length; i++)
        {
          data[i] = start + i;
        }
        return data;
      }
      else
      {
        var data = new double[start - stop + 1];
        for (int i = 0; i < data.Length; i++)
        {
          data[i] = start - i;
        }
        return data;
      }
    }

    /// <summary>
    /// Generate a linearly spaced sample vector within the inclusive interval (start, stop) and step 1.
    /// Equivalent to MATLAB colon operator (:).
    /// </summary>
    /// <param name="start">The start value of the range.</param>
    /// <param name="stop">The end value of the range.</param>
    /// <returns>The generated sample vector.</returns>
    public static int[] LinearRangeInt32(int start, int stop)
    {
      if (start == stop) return new[] { start };
      if (start < stop)
      {
        var data = new int[stop - start + 1];
        for (int i = 0; i < data.Length; i++)
        {
          data[i] = start + i;
        }
        return data;
      }
      else
      {
        var data = new int[start - stop + 1];
        for (int i = 0; i < data.Length; i++)
        {
          data[i] = start - i;
        }
        return data;
      }
    }

    /// <summary>
    /// Generate a linearly spaced sample vector within the inclusive interval (start, stop) and the provided step.
    /// The start value is always included as first value, but stop is only included if it stop-start is a multiple of step.
    /// Equivalent to MATLAB double colon operator (::).
    /// </summary>
    /// <param name="start">The start value of the range.</param>
    /// <param name="step">The step size.</param>
    /// <param name="stop">The end value of the range.</param>
    /// <returns>The generated sample vector.</returns>
    public static double[] LinearRange(int start, int step, int stop)
    {
      if (start == stop) return new double[] { start };
      if (start < stop && step < 0 || start > stop && step > 0 || step == 0d)
      {
        return Array.Empty<double>();
      }

      var data = new double[(stop - start) / step + 1];
      for (int i = 0; i < data.Length; i++)
      {
        data[i] = start + i * step;
      }
      return data;
    }

    /// <summary>
    /// Generate a linearly spaced sample vector within the inclusive interval (start, stop) and the provided step.
    /// The start value is always included as first value, but stop is only included if it stop-start is a multiple of step.
    /// Equivalent to MATLAB double colon operator (::).
    /// </summary>
    /// <param name="start">The start value of the range.</param>
    /// <param name="step">The step size.</param>
    /// <param name="stop">The end value of the range.</param>
    /// <returns>The generated sample vector.</returns>
    public static int[] LinearRangeInt32(int start, int step, int stop)
    {
      if (start == stop) return new[] { start };
      if (start < stop && step < 0 || start > stop && step > 0 || step == 0d)
      {
        return Array.Empty<int>();
      }

      var data = new int[(stop - start) / step + 1];
      for (int i = 0; i < data.Length; i++)
      {
        data[i] = start + i * step;
      }
      return data;
    }

    /// <summary>
    /// Generate a linearly spaced sample vector within the inclusive interval (start, stop) and the provided step.
    /// The start value is always included as first value, but stop is only included if it stop-start is a multiple of step.
    /// Equivalent to MATLAB double colon operator (::).
    /// </summary>
    /// <param name="start">The start value of the range.</param>
    /// <param name="step">The step size.</param>
    /// <param name="stop">The end value of the range.</param>
    /// <returns>The generated sample vector.</returns>
    public static double[] LinearRange(double start, double step, double stop)
    {
      if (start == stop) return new[] { start };
      if (start < stop && step < 0 || start > stop && step > 0 || step == 0d)
      {
        return Array.Empty<double>();
      }

      var data = new double[(int)Math.Floor((stop - start) / step + 1d)];
      for (int i = 0; i < data.Length; i++)
      {
        data[i] = start + i * step;
      }
      return data;
    }

    /// <summary>
    /// Generate samples by sampling a function at linearly spaced points within the inclusive interval (start, stop) and the provided step.
    /// The start value is always included as first value, but stop is only included if it stop-start is a multiple of step.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="start">The start value of the range.</param>
    /// <param name="step">The step size.</param>
    /// <param name="stop">The end value of the range.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The sampled values.</returns>
    public static T[] LinearRangeMap<T>(double start, double step, double stop, Func<double, T> map)
    {
      if (start == stop) return new[] { map(start) };
      if (start < stop && step < 0 || start > stop && step > 0 || step == 0d)
      {
        return Array.Empty<T>();
      }

      var data = new T[(int)Math.Floor((stop - start) / step + 1d)];
      for (int i = 0; i < data.Length; i++)
      {
        data[i] = map(start + i * step);
      }
      return data;
    }

    /// <summary>
    /// Create a periodic wave.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="samplingRate">Samples per time unit (Hz). Must be larger than twice the frequency to satisfy the Nyquist criterion.</param>
    /// <param name="frequency">Frequency in periods per time unit (Hz).</param>
    /// <param name="amplitude">The length of the period when sampled at one sample per time unit. This is the interval of the periodic domain, a typical value is 1.0, or 2*Pi for angular functions.</param>
    /// <param name="phase">Optional phase offset.</param>
    /// <param name="delay">Optional delay, relative to the phase.</param>
    /// <returns>The periodic samples.</returns>
    public static double[] Periodic(int length, double samplingRate, double frequency, double amplitude = 1.0, double phase = 0.0, int delay = 0)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      double step = frequency / samplingRate * amplitude;
      phase = Euclid.Modulus(phase - delay * step, amplitude);

      var data = new double[length];
      for (int i = 0, k = 0; i < data.Length; i++, k++)
      {
        var x = phase + k * step;
        if (x >= amplitude)
        {
          x %= amplitude;
          phase = x;
          k = 0;
        }

        data[i] = x;
      }
      return data;
    }

    /// <summary>
    /// Create a periodic wave.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="map">The function to apply to each of the values and evaluate the resulting sample.</param>
    /// <param name="samplingRate">Samples per time unit (Hz). Must be larger than twice the frequency to satisfy the Nyquist criterion.</param>
    /// <param name="frequency">Frequency in periods per time unit (Hz).</param>
    /// <param name="amplitude">The length of the period when sampled at one sample per time unit. This is the interval of the periodic domain, a typical value is 1.0, or 2*Pi for angular functions.</param>
    /// <param name="phase">Optional phase offset.</param>
    /// <param name="delay">Optional delay, relative to the phase.</param>
    /// <returns>The sampled periodic wave.</returns>
    public static T[] PeriodicMap<T>(int length, Func<double, T> map, double samplingRate, double frequency, double amplitude = 1.0, double phase = 0.0, int delay = 0)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      double step = frequency / samplingRate * amplitude;
      phase = Euclid.Modulus(phase - delay * step, amplitude);

      var data = new T[length];
      for (int i = 0, k = 0; i < data.Length; i++, k++)
      {
        var x = phase + k * step;
        if (x >= amplitude)
        {
          x %= amplitude;
          phase = x;
          k = 0;
        }

        data[i] = map(x);
      }
      return data;
    }

    /// <summary>
    /// Create an infinite periodic wave sequence.
    /// </summary>
    /// <param name="samplingRate">Samples per time unit (Hz). Must be larger than twice the frequency to satisfy the Nyquist criterion.</param>
    /// <param name="frequency">Frequency in periods per time unit (Hz).</param>
    /// <param name="amplitude">The length of the period when sampled at one sample per time unit. This is the interval of the periodic domain, a typical value is 1.0, or 2*Pi for angular functions.</param>
    /// <param name="phase">Optional phase offset.</param>
    /// <param name="delay">Optional delay, relative to the phase.</param>
    /// <returns>The periodic sample sequence.</returns>
    public static IEnumerable<double> PeriodicSequence(double samplingRate, double frequency, double amplitude = 1.0, double phase = 0.0, int delay = 0)
    {
      double step = frequency / samplingRate * amplitude;
      phase = Euclid.Modulus(phase - delay * step, amplitude);

      int k = 0;
      while (true)
      {
        var x = phase + (k++) * step;
        if (x >= amplitude)
        {
          x %= amplitude;
          phase = x;
          k = 1;
        }

        yield return x;
      }
    }

    /// <summary>
    /// Create an infinite periodic wave sequence.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="map">The function to apply to each of the values and evaluate the resulting sample.</param>
    /// <param name="samplingRate">Samples per time unit (Hz). Must be larger than twice the frequency to satisfy the Nyquist criterion.</param>
    /// <param name="frequency">Frequency in periods per time unit (Hz).</param>
    /// <param name="amplitude">The length of the period when sampled at one sample per time unit. This is the interval of the periodic domain, a typical value is 1.0, or 2*Pi for angular functions.</param>
    /// <param name="phase">Optional phase offset.</param>
    /// <param name="delay">Optional delay, relative to the phase.</param>
    /// <returns>The sampled periodic wave sequence.</returns>
    public static IEnumerable<T> PeriodicMapSequence<T>(Func<double, T> map, double samplingRate, double frequency, double amplitude = 1.0, double phase = 0.0, int delay = 0)
    {
      double step = frequency / samplingRate * amplitude;
      phase = Euclid.Modulus(phase - delay * step, amplitude);

      int k = 0;
      while (true)
      {
        var x = phase + (k++) * step;
        if (x >= amplitude)
        {
          x %= amplitude;
          phase = x;
          k = 1;
        }

        yield return map(x);
      }
    }

    /// <summary>
    /// Create a Sine wave.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="samplingRate">Samples per time unit (Hz). Must be larger than twice the frequency to satisfy the Nyquist criterion.</param>
    /// <param name="frequency">Frequency in periods per time unit (Hz).</param>
    /// <param name="amplitude">The maximal reached peak.</param>
    /// <param name="mean">The mean, or DC part, of the signal.</param>
    /// <param name="phase">Optional phase offset.</param>
    /// <param name="delay">Optional delay, relative to the phase.</param>
    /// <returns>The sinusoidal samples.</returns>
    public static double[] Sinusoidal(int length, double samplingRate, double frequency, double amplitude, double mean = 0.0, double phase = 0.0, int delay = 0)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      double step = frequency / samplingRate * Constants.Pi2;
      phase = (phase - delay * step) % Constants.Pi2;

      var data = new double[length];
      for (int i = 0; i < data.Length; i++)
      {
        data[i] = mean + amplitude * Math.Sin(phase + i * step);
      }
      return data;
    }

    /// <summary>
    /// Create an infinite Sine wave sequence.
    /// </summary>
    /// <param name="samplingRate">Samples per unit.</param>
    /// <param name="frequency">Frequency in samples per unit.</param>
    /// <param name="amplitude">The maximal reached peak.</param>
    /// <param name="mean">The mean, or DC part, of the signal.</param>
    /// <param name="phase">Optional phase offset.</param>
    /// <param name="delay">Optional delay, relative to the phase.</param>
    /// <returns>The sinusoidal sample sequence.</returns>
    public static IEnumerable<double> SinusoidalSequence(double samplingRate, double frequency, double amplitude, double mean = 0.0, double phase = 0.0, int delay = 0)
    {
      double step = frequency / samplingRate * Constants.Pi2;
      phase = (phase - delay * step) % Constants.Pi2;

      while (true)
      {
        for (int i = 0; i < 1000; i++)
        {
          yield return mean + amplitude * Math.Sin(phase + i * step);
        }
        phase = (phase + 1000 * step) % Constants.Pi2;
      }
    }

    /// <summary>
    /// Create a periodic square wave, starting with the high phase.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="highDuration">Number of samples of the high phase.</param>
    /// <param name="lowDuration">Number of samples of the low phase.</param>
    /// <param name="lowValue">Sample value to be emitted during the low phase.</param>
    /// <param name="highValue">Sample value to be emitted during the high phase.</param>
    /// <param name="delay">Optional delay.</param>
    /// <returns>The square-wave samples.</returns>
    public static double[] Square(int length, int highDuration, int lowDuration, double lowValue, double highValue, int delay = 0)
    {
      var duration = highDuration + lowDuration;
      return PeriodicMap(length, x => x < highDuration ? highValue : lowValue, 1.0, 1.0 / duration, duration, 0.0, delay);
    }

    /// <summary>
    /// Create an infinite periodic square wave sequence, starting with the high phase.
    /// </summary>
    /// <param name="highDuration">Number of samples of the high phase.</param>
    /// <param name="lowDuration">Number of samples of the low phase.</param>
    /// <param name="lowValue">Sample value to be emitted during the low phase.</param>
    /// <param name="highValue">Sample value to be emitted during the high phase.</param>
    /// <param name="delay">Optional delay.</param>
    /// <returns>The square-wave sample sequence.</returns>
    public static IEnumerable<double> SquareSequence(int highDuration, int lowDuration, double lowValue, double highValue, int delay = 0)
    {
      var duration = highDuration + lowDuration;
      return PeriodicMapSequence(x => x < highDuration ? highValue : lowValue, 1.0, 1.0 / duration, duration, 0.0, delay);
    }

    /// <summary>
    /// Create a periodic triangle wave, starting with the raise phase from the lowest sample.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="raiseDuration">Number of samples of the raise phase.</param>
    /// <param name="fallDuration">Number of samples of the fall phase.</param>
    /// <param name="lowValue">Lowest sample value.</param>
    /// <param name="highValue">Highest sample value.</param>
    /// <param name="delay">Optional delay.</param>
    /// <returns>The triangle-wave samples.</returns>
    public static double[] Triangle(int length, int raiseDuration, int fallDuration, double lowValue, double highValue, int delay = 0)
    {
      var duration = raiseDuration + fallDuration;
      var height = highValue - lowValue;
      var raise = height / raiseDuration;
      var fall = height / fallDuration;
      return PeriodicMap(length, x => x < raiseDuration ? lowValue + x * raise : highValue - (x - raiseDuration) * fall, 1.0, 1.0 / duration, duration, 0.0, delay);
    }

    /// <summary>
    /// Create an infinite periodic triangle wave sequence, starting with the raise phase from the lowest sample.
    /// </summary>
    /// <param name="raiseDuration">Number of samples of the raise phase.</param>
    /// <param name="fallDuration">Number of samples of the fall phase.</param>
    /// <param name="lowValue">Lowest sample value.</param>
    /// <param name="highValue">Highest sample value.</param>
    /// <param name="delay">Optional delay.</param>
    /// <returns>The triangle-wave sample sequence.</returns>
    public static IEnumerable<double> TriangleSequence(int raiseDuration, int fallDuration, double lowValue, double highValue, int delay = 0)
    {
      var duration = raiseDuration + fallDuration;
      var height = highValue - lowValue;
      var raise = height / raiseDuration;
      var fall = height / fallDuration;
      return PeriodicMapSequence(x => x < raiseDuration ? lowValue + x * raise : highValue - (x - raiseDuration) * fall, 1.0, 1.0 / duration, duration, 0.0, delay);
    }

    /// <summary>
    /// Create a periodic sawtooth wave, starting with the lowest sample.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="period">Number of samples a full sawtooth period.</param>
    /// <param name="lowValue">Lowest sample value.</param>
    /// <param name="highValue">Highest sample value.</param>
    /// <param name="delay">Optional delay.</param>
    /// <returns>The sawtooth-wave samples.</returns>
    public static double[] Sawtooth(int length, int period, double lowValue, double highValue, int delay = 0)
    {
      var height = highValue - lowValue;
      return PeriodicMap(length, x => x + lowValue, 1.0, 1.0 / period, height * period / (period - 1), 0.0, delay);
    }

    /// <summary>
    /// Create an infinite periodic sawtooth wave sequence, starting with the lowest sample.
    /// </summary>
    /// <param name="period">Number of samples a full sawtooth period.</param>
    /// <param name="lowValue">Lowest sample value.</param>
    /// <param name="highValue">Highest sample value.</param>
    /// <param name="delay">Optional delay.</param>
    /// <returns>The sawtooth-wave sample sequence.</returns>
    public static IEnumerable<double> SawtoothSequence(int period, double lowValue, double highValue, int delay = 0)
    {
      var height = highValue - lowValue;
      return PeriodicMapSequence(x => x + lowValue, 1.0, 1.0 / period, height * period / (period - 1), 0.0, delay);
    }

    /// <summary>
    /// Create an array with each field set to the same value.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="value">The value that each field should be set to.</param>
    /// <returns>The generated array.</returns>
    public static T[] Repeat<T>(int length, T value)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var data = new T[length];
      CommonParallel.For(0, data.Length, 4096, (a, b) =>
      {
        for (int i = a; i < b; i++)
        {
          data[i] = value;
        }
      });
      return data;
    }

    /// <summary>
    /// Create an infinite sequence where each element has the same value.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="value">The value that each element should be set to.</param>
    /// <returns>The generated sequence.</returns>
    public static IEnumerable<T> RepeatSequence<T>(T value)
    {
      while (true)
      {
        yield return value;
      }
    }

    /// <summary>
    /// Create a Heaviside Step sample vector.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="amplitude">The maximal reached peak.</param>
    /// <param name="delay">Offset to the time axis.</param>
    /// <returns>The step samples.</returns>
    public static double[] Step(int length, double amplitude, int delay)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var data = new double[length];
      for (int i = Math.Max(0, delay); i < data.Length; i++)
      {
        data[i] = amplitude;
      }
      return data;
    }

    /// <summary>
    /// Create an infinite Heaviside Step sample sequence.
    /// </summary>
    /// <param name="amplitude">The maximal reached peak.</param>
    /// <param name="delay">Offset to the time axis.</param>
    /// <returns>The step sample sequence.</returns>
    public static IEnumerable<double> StepSequence(double amplitude, int delay)
    {
      for (int i = 0; i < delay; i++)
      {
        yield return 0d;
      }

      while (true)
      {
        yield return amplitude;
      }
    }

    /// <summary>
    /// Create a Kronecker Delta impulse sample vector.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="amplitude">The maximal reached peak.</param>
    /// <param name="delay">Offset to the time axis. Zero or positive.</param>
    /// <returns>The impulse samples.</returns>
    public static double[] Impulse(int length, double amplitude, int delay)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var data = new double[length];
      if (delay >= 0 && delay < length)
      {
        data[delay] = amplitude;
      }
      return data;
    }

    /// <summary>
    /// Create a Kronecker Delta impulse sample vector.
    /// </summary>
    /// <param name="amplitude">The maximal reached peak.</param>
    /// <param name="delay">Offset to the time axis, hence the sample index of the impulse.</param>
    /// <returns>The impulse sample sequence.</returns>
    public static IEnumerable<double> ImpulseSequence(double amplitude, int delay)
    {
      if (delay >= 0)
      {
        for (int i = 0; i < delay; i++)
        {
          yield return 0d;
        }

        yield return amplitude;
      }

      while (true)
      {
        yield return 0d;
      }
    }

    /// <summary>
    /// Create a periodic Kronecker Delta impulse sample vector.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="period">impulse sequence period.</param>
    /// <param name="amplitude">The maximal reached peak.</param>
    /// <param name="delay">Offset to the time axis. Zero or positive.</param>
    /// <returns>The periodic impulse samples.</returns>
    public static double[] PeriodicImpulse(int length, int period, double amplitude, int delay)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var data = new double[length];
      delay = Euclid.Modulus(delay, period);
      while (delay < length)
      {
        data[delay] = amplitude;
        delay += period;
      }
      return data;
    }

    /// <summary>
    /// Create a Kronecker Delta impulse sample vector.
    /// </summary>
    /// <param name="period">impulse sequence period.</param>
    /// <param name="amplitude">The maximal reached peak.</param>
    /// <param name="delay">Offset to the time axis. Zero or positive.</param>
    /// <returns>The periodic impulse sample sequence.</returns>
    public static IEnumerable<double> PeriodicImpulseSequence(int period, double amplitude, int delay)
    {
      delay = Euclid.Modulus(delay, period);

      for (int i = 0; i < delay; i++)
      {
        yield return 0d;
      }

      while (true)
      {
        yield return amplitude;

        for (int i = 1; i < period; i++)
        {
          yield return 0d;
        }
      }
    }

    /// <summary>
    /// Generate samples generated by the given computation.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <typeparam name="TState">The type of the unfolding state.</typeparam>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="f">The computation to perform.</param>
    /// <param name="state">The initial state.</param>
    /// <returns>The generated values.</returns>
    public static T[] Unfold<T, TState>(int length, Func<TState, Tuple<T, TState>> f, TState state)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var data = new T[length];
      for (int i = 0; i < data.Length; i++)
      {
        (data[i], state) = f(state);
      }
      return data;
    }

    /// <summary>
    /// Generate samples generated by the given computation.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <typeparam name="TState">The type of the unfolding state.</typeparam>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="f">The computation to perform.</param>
    /// <param name="state">The initial state.</param>
    /// <returns>The generated values.</returns>
    public static T[] Unfold<T, TState>(int length, Func<TState, (T, TState)> f, TState state)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var data = new T[length];
      for (int i = 0; i < data.Length; i++)
      {
        (data[i], state) = f(state);
      }
      return data;
    }

    /// <summary>
    /// Generate an infinite sequence generated by the given computation.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <typeparam name="TState">The type of the unfolding state.</typeparam>
    /// <param name="f">The computation to perform.</param>
    /// <param name="state">The initial state.</param>
    /// <returns>The generated sequence.</returns>
    public static IEnumerable<T> UnfoldSequence<T, TState>(Func<TState, Tuple<T, TState>> f, TState state)
    {
      while (true)
      {
        var (item, nextState) = f(state);
        state = nextState;
        yield return item;
      }
    }

    /// <summary>
    /// Generate an infinite sequence generated by the given computation.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <typeparam name="TState">The type of the unfolding state.</typeparam>
    /// <param name="f">The computation to perform.</param>
    /// <param name="state">The initial state.</param>
    /// <returns>The generated sequence.</returns>
    public static IEnumerable<T> UnfoldSequence<T, TState>(Func<TState, (T, TState)> f, TState state)
    {
      while (true)
      {
        var (item, nextState) = f(state);
        state = nextState;
        yield return item;
      }
    }

    /// <summary>
    /// Generate a Fibonacci sequence, including zero as first value.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <returns>The Fibonacci numbers.</returns>
    public static BigInteger[] Fibonacci(int length)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var data = new BigInteger[length];
      if (data.Length > 0)
      {
        data[0] = BigInteger.Zero;
      }
      if (data.Length > 1)
      {
        data[1] = BigInteger.One;
      }
      for (int i = 2; i < data.Length; i++)
      {
        data[i] = data[i - 1] + data[i - 2];
      }
      return data;
    }

    /// <summary>
    /// Generate an infinite Fibonacci sequence, including zero as first value.
    /// </summary>
    /// <returns>The Fibonacci sequence.</returns>
    public static IEnumerable<BigInteger> FibonacciSequence()
    {
      BigInteger a = BigInteger.Zero;
      yield return a;

      BigInteger b = BigInteger.One;
      yield return b;

      while (true)
      {
        a = a + b;
        yield return a;

        b = a + b;
        yield return b;
      }
    }

    /// <summary>
    /// Create random samples, uniform between 0 and 1.
    /// Faster than other methods but with reduced guarantees on randomness.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <returns>The generated samples.</returns>
    public static double[] Uniform(int length)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      return SystemRandomSource.FastDoubles(length);
    }

    /// <summary>
    /// Create an infinite random sample sequence, uniform between 0 and 1.
    /// Faster than other methods but with reduced guarantees on randomness.
    /// </summary>
    /// <returns>The generated sample sequence.</returns>
    public static IEnumerable<double> UniformSequence()
    {
      return SystemRandomSource.DoubleSequence();
    }


    /// <summary>
    /// Generate samples by sampling a function at samples from a probability distribution, uniform between 0 and 1.
    /// Faster than other methods but with reduced guarantees on randomness.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The generated samples.</returns>
    public static T[] UniformMap<T>(int length, Func<double, T> map)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var samples = SystemRandomSource.FastDoubles(length);
      return Map(samples, map);
    }

    /// <summary>
    /// Generate a sample sequence by sampling a function at samples from a probability distribution, uniform between 0 and 1.
    /// Faster than other methods but with reduced guarantees on randomness.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="map">The function to sample.</param>
    /// <returns>The generated sample sequence.</returns>
    public static IEnumerable<T> UniformMapSequence<T>(Func<double, T> map)
    {
      return SystemRandomSource.DoubleSequence().Select(map);
    }

    /// <summary>
    /// Generate samples by sampling a function at sample pairs from a probability distribution, uniform between 0 and 1.
    /// Faster than other methods but with reduced guarantees on randomness.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The generated samples.</returns>
    public static T[] UniformMap2<T>(int length, Func<double, double, T> map)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var samples1 = SystemRandomSource.FastDoubles(length);
      var samples2 = SystemRandomSource.FastDoubles(length);
      return Map2(samples1, samples2, map);
    }

    /// <summary>
    /// Generate a sample sequence by sampling a function at sample pairs from a probability distribution, uniform between 0 and 1.
    /// Faster than other methods but with reduced guarantees on randomness.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="map">The function to sample.</param>
    /// <returns>An infinite sequence of generated values.</returns>
    public static IEnumerable<T> UniformMap2Sequence<T>(Func<double, double, T> map)
    {
      var rnd1 = SystemRandomSource.Default;
      for (int i = 0; i < 128; i++)
      {
        yield return map(rnd1.NextDouble(), rnd1.NextDouble());
      }

      var rnd2 = new System.Random(RandomSeed.Robust());
      while (true)
      {
        yield return map(rnd2.NextDouble(), rnd2.NextDouble());
      }
    }

    /// <summary>
    /// Create samples with independent amplitudes of standard distribution.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <returns>The generated samples.</returns>
    public static double[] Standard(int length)
    {
      return Normal(length, 0.0, 1.0);
    }

    /// <summary>
    /// Create an infinite sample sequence with independent amplitudes of standard distribution.
    /// </summary>
    /// <returns>The generated sample sequence.</returns>
    public static IEnumerable<double> StandardSequence()
    {
      return NormalSequence(0.0, 1.0);
    }

    /// <summary>
    /// Create samples with independent amplitudes of normal distribution and a flat spectral density.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="mean">The mean of the distribution.</param>
    /// <param name="standardDeviation">The standard deviation of the distribution.</param>
    /// <returns>The generated samples.</returns>
    public static double[] Normal(int length, double mean, double standardDeviation)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var samples = new double[length];
      Distributions.Normal.Samples(SystemRandomSource.Default, samples, mean, standardDeviation);
      return samples;
    }

    /// <summary>
    /// Create an infinite sample sequence with independent amplitudes of normal distribution and a flat spectral density.
    /// </summary>
    /// <param name="mean">The mean of the distribution.</param>
    /// <param name="standardDeviation">The standard deviation of the distribution.</param>
    /// <returns>The generated sample sequence.</returns>
    public static IEnumerable<double> NormalSequence(double mean, double standardDeviation)
    {
      return Distributions.Normal.Samples(SystemRandomSource.Default, mean, standardDeviation);
    }

    /// <summary>
    /// Create random samples.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="distribution">The probability distribution to sample from.</param>
    /// <returns>The generated samples.</returns>
    public static double[] Random(int length, IContinuousDistribution distribution)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var samples = new double[length];
      distribution.Samples(samples);
      return samples;
    }

    /// <summary>
    /// Create an infinite random sample sequence.
    /// </summary>
    /// <param name="distribution">The probability distribution to sample from.</param>
    /// <returns>The generated sample sequence.</returns>
    public static IEnumerable<double> Random(IContinuousDistribution distribution)
    {
      return distribution.Samples();
    }

    /// <summary>
    /// Create random samples.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="distribution">The probability distribution to sample from.</param>
    /// <returns>The generated samples.</returns>
    public static float[] RandomSingle(int length, IContinuousDistribution distribution)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var samples = new double[length];
      distribution.Samples(samples);
      return Map(samples, v => (float)v);
    }

    /// <summary>
    /// Create an infinite random sample sequence.
    /// </summary>
    /// <param name="distribution">The probability distribution to sample from.</param>
    /// <returns>The generated sample sequence.</returns>
    public static IEnumerable<float> RandomSingle(IContinuousDistribution distribution)
    {
      return distribution.Samples().Select(v => (float)v);
    }

    /// <summary>
    /// Create random samples.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="distribution">The probability distribution to sample from.</param>
    /// <returns>The generated samples.</returns>
    public static Complex[] RandomComplex(int length, IContinuousDistribution distribution)
    {
      return RandomMap2(length, distribution, (r, i) => new Complex(r, i));
    }

    /// <summary>
    /// Create an infinite random sample sequence.
    /// </summary>
    /// <param name="distribution">The probability distribution to sample from.</param>
    /// <returns>The generated sample sequence.</returns>
    public static IEnumerable<Complex> RandomComplex(IContinuousDistribution distribution)
    {
      return RandomMap2Sequence(distribution, (r, i) => new Complex(r, i));
    }

    /// <summary>
    /// Create random samples.
    /// </summary>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="distribution">The probability distribution to sample from.</param>
    /// <returns>The generated samples.</returns>
    public static Complex32[] RandomComplex32(int length, IContinuousDistribution distribution)
    {
      return RandomMap2(length, distribution, (r, i) => new Complex32((float)r, (float)i));
    }

    /// <summary>
    /// Create an infinite random sample sequence.
    /// </summary>
    /// <param name="distribution">The probability distribution to sample from.</param>
    /// <returns>The generated sample sequence.</returns>
    public static IEnumerable<Complex32> RandomComplex32(IContinuousDistribution distribution)
    {
      return RandomMap2Sequence(distribution, (r, i) => new Complex32((float)r, (float)i));
    }

    /// <summary>
    /// Generate samples by sampling a function at samples from a probability distribution.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="distribution">The probability distribution to sample from.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The generated samples.</returns>
    public static T[] RandomMap<T>(int length, IContinuousDistribution distribution, Func<double, T> map)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var samples = new double[length];
      distribution.Samples(samples);
      return Map(samples, map);
    }

    /// <summary>
    /// Generate a sample sequence by sampling a function at samples from a probability distribution.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="distribution">The probability distribution to sample from.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The generated sample sequence.</returns>
    public static IEnumerable<T> RandomMapSequence<T>(IContinuousDistribution distribution, Func<double, T> map)
    {
      return distribution.Samples().Select(map);
    }

    /// <summary>
    /// Generate samples by sampling a function at sample pairs from a probability distribution.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="length">The number of samples to generate.</param>
    /// <param name="distribution">The probability distribution to sample from.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The generated samples.</returns>
    public static T[] RandomMap2<T>(int length, IContinuousDistribution distribution, Func<double, double, T> map)
    {
      if (length < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }

      var samples1 = new double[length];
      var samples2 = new double[length];
      distribution.Samples(samples1);
      distribution.Samples(samples2);
      return Map2(samples1, samples2, map);
    }

    /// <summary>
    /// Generate a sample sequence by sampling a function at sample pairs from a probability distribution.
    /// </summary>
    /// <typeparam name="T">The type of the generated values.</typeparam>
    /// <param name="distribution">The probability distribution to sample from.</param>
    /// <param name="map">The function to sample.</param>
    /// <returns>The generated sample sequence.</returns>
    public static IEnumerable<T> RandomMap2Sequence<T>(IContinuousDistribution distribution, Func<double, double, T> map)
    {
      return distribution.Samples().Zip(distribution.Samples(), map);
    }
  }
}
