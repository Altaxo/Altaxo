#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Ode
{
  /// <summary>
  /// Provides a base implementation for multistep methods used to solve ordinary differential equations (ODEs).
  /// </summary>
  public partial class MultiStepMethodBase
  {
    /// <summary>
    /// Temporary storage for the initialization data.
    /// </summary>
    protected InitializationData? _initialization;


    /// <summary>
    /// Delegate to evaluate the Jacobian, i.e. the derivative of <c>F</c> with respect to <c>y</c>.
    /// </summary>
    /// <param name="x">The current <c>x</c> value.</param>
    /// <param name="y">The current <c>y</c> values.</param>
    /// <param name="jac">
    /// At the end of this call, contains the Jacobian matrix. The implementer must check whether a matrix is provided and
    /// must check the dimensions of the provided matrix.
    /// In case the provided matrix is <see langword="null"/> or does not fit the requirements, the implementer must allocate an appropriate matrix.
    /// At the end of the call, the matrix must be filled with the elements of the Jacobian.
    /// </param>
    public delegate void CalculateJacobian(double x, double[] y, [AllowNull][NotNull] ref IMatrix<double> jac);

    /// <summary>
    /// Stores the initialization data.
    /// </summary>
    public class InitializationData
    {
      /// <summary>
      /// Gets or sets the step size to be used.
      /// </summary>
      /// <remarks>
      /// If <see langword="null"/>, the step size is chosen automatically by the algorithm (if supported by the concrete implementation).
      /// </remarks>
      public double? StepSize { get; set; }

      /// <summary>
      /// Gets or sets the initial <c>x</c> value.
      /// </summary>
      public double X0 { get; set; }

      /// <summary>
      /// Gets or sets the initial <c>y</c> values.
      /// </summary>
      public double[] Y0 { get; set; }

      /// <summary>
      /// Gets or sets the function used to calculate the derivatives.
      /// </summary>
      public Action<double, double[], double[]> F { get; set; }

      /// <summary>
      /// Gets or sets the function to evaluate the Jacobian, i.e. the derivative of <see cref="F"/> with respect to <c>y</c>.
      /// </summary>
      public CalculateJacobian? EvaluateJacobian { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="InitializationData"/> class.
      /// </summary>
      /// <param name="x0">The initial <c>x</c> value.</param>
      /// <param name="y0">The initial <c>y</c> values.</param>
      /// <param name="f">
      /// Function to calculate the derivatives.
      /// The first argument is the <c>x</c> value, the second argument are the current <c>y</c> values.
      /// The third argument is an array that stores the derivatives.
      /// </param>
      public InitializationData(double x0, double[] y0, Action<double, double[], double[]> f)
      {
        X0 = x0;
        Y0 = y0;
        F = f;
      }
    }

    #region Static helpers

    /// <summary>
    /// Swaps two values <paramref name="a"/> and <paramref name="b"/>.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    public static void Swap<T>(ref T a, ref T b)
    {
      var h = a;
      a = b;
      b = h;
    }

    /// <summary>
    /// Creates a new jagged array.
    /// </summary>
    /// <typeparam name="T">Type of the elements.</typeparam>
    /// <param name="i">First dimension of the array (spine dimension).</param>
    /// <param name="k">Second dimension of the array.</param>
    /// <returns>The newly created jagged array.</returns>
    public static T[][] NewJaggedArray<T>(int i, int k) where T : struct
    {
      var result = new T[i][];
      for (int n = 0; n < result.Length; ++n)
      {
        result[n] = new T[k];
      }
      return result;
    }

    /*
    protected static T[] NewJaggedArray<T>(int i, int k, Func<int, T> spineElementInitialization) where T : struct
    {
      var result = new T[i];
      for (int n = 0; n < result.Length; ++n)
      {
        result[n] = spineElementInitialization(k);
      }
      return result;
    }
    */


    /// <summary>
    /// Rotates the elements of <paramref name="array"/> one position upwards, i.e. <c>array[0]</c> to <c>array[1]</c>, <c>array[1]</c> to <c>array[2]</c>, etc.
    /// and the last element back to <c>array[0]</c>.
    /// </summary>
    /// <typeparam name="T">Type of the array elements.</typeparam>
    /// <param name="array">The array.</param>
    protected static void RotateElementsUpwards<T>(T[] array)
    {
      var last = array[array.Length - 1];
      for (int i = array.Length - 1; i > 0; --i)
      {
        array[i] = array[i - 1];
      }
      array[0] = last;
    }

    /// <summary>
    /// Multiplies the elements of array <paramref name="a"/> by the factor <paramref name="b"/>.
    /// </summary>
    /// <param name="a">Array <c>a</c>.</param>
    /// <param name="b">Factor <c>b</c>.</param>
    protected static void Multiply(double[] a, double b)
    {
      for (int i = 0; i < a.Length; ++i)
      {
        a[i] *= b;
      }
    }

    /// <summary>
    /// Calculates the L2 norm of array <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The array <c>x</c>.</param>
    /// <returns>The L2 norm of array <paramref name="x"/>.</returns>
    protected static double L2Norm(double[] x)
    {
      double sum = 0;
      for (int i = 0; i < x.Length; ++i)
      {
        sum += x[i] * x[i];
      }
      return Math.Sqrt(sum);
    }

    /// <summary>
    /// Calculates the factorial of parameter <paramref name="x"/> (in the range of 0..12).
    /// </summary>
    /// <param name="x">The value.</param>
    /// <returns>The factorial of <paramref name="x"/>.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="x"/> is outside the supported range.</exception>
    protected static double Faculty(int x)
    {
      return x switch
      {
        0 => 1,
        1 => 1,
        2 => 2,
        3 => 6,
        4 => 24,
        5 => 120,
        6 => 720,
        7 => 5040,
        8 => 40320,
        9 => 362880,
        10 => 3628800,
        11 => 39916800,
        12 => 479001600,
        _ => throw new NotImplementedException()
      };
    }

    /// <summary>
    /// Determines whether <paramref name="x1"/> and <paramref name="x2"/> are equal within a given relative tolerance.
    /// </summary>
    /// <param name="x1">The value <c>x1</c>.</param>
    /// <param name="x2">The value <c>x2</c>.</param>
    /// <param name="relTol">The relative tolerance value.</param>
    /// <returns><see langword="true"/> if the values are equal within tolerance; otherwise, <see langword="false"/>.</returns>
    protected static bool AreEqual(double x1, double x2, double relTol)
    {
      return (0 == x1 && 0 == x2) || Math.Abs(x1 - x2) < relTol * Math.Max(Math.Abs(x1), Math.Abs(x2));
    }

    /// <summary>
    /// Creates an enumerator from <paramref name="enumerable"/> and tries to move to the first element.
    /// If the enumeration is <see langword="null"/> or empty, <see langword="null"/> is returned instead of the enumerator.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The enumerator, or <see langword="null"/> if the enumeration is <see langword="null"/> or empty.</returns>
    protected static IEnumerator<double>? EnumerationInitialize(IEnumerable<double>? enumerable)
    {
      var enumerator = enumerable?.GetEnumerator();
      if (enumerator is not null && !enumerator.MoveNext())
      {
        enumerator.Dispose();
        enumerator = null;
      }

      return enumerator;
    }

    /// <summary>
    /// Moves <paramref name="enumerator"/> so that the current element is greater than or equal to the provided <paramref name="x"/> value.
    /// </summary>
    /// <param name="enumerator">The enumerator. If the enumerator no longer has any elements, this reference is set to <see langword="null"/>.</param>
    /// <param name="x">The <c>x</c> value.</param>
    /// <returns>
    /// The current element that is greater than or equal to the provided <paramref name="x"/> value,
    /// or <see langword="null"/> if the enumeration has run out of elements.
    /// </returns>
    protected static double? EnumerationForwardToGreaterThanOrEqual(ref IEnumerator<double>? enumerator, double x)
    {
      if (enumerator is not null)
      {
        while (!(enumerator.Current >= x))
        {
          if (!(enumerator.MoveNext()))
          {
            enumerator.Dispose();
            enumerator = null;
            break;
          }
        }
      }
      return enumerator?.Current;
    }

    /// <summary>
    /// Moves <paramref name="enumerator"/> so that the current element is greater than the provided <paramref name="x"/> value.
    /// </summary>
    /// <param name="enumerator">The enumerator. If the enumerator no longer has any elements, this reference is set to <see langword="null"/>.</param>
    /// <param name="x">The <c>x</c> value.</param>
    /// <returns>
    /// The current element that is greater than the provided <paramref name="x"/> value,
    /// or <see langword="null"/> if the enumeration has run out of elements.
    /// </returns>
    protected static double? EnumerationForwardToGreaterThan(ref IEnumerator<double>? enumerator, double x)
    {
      if (enumerator is not null)
      {
        while (!(enumerator.Current > x))
        {
          if (!(enumerator.MoveNext()))
          {
            enumerator.Dispose();
            enumerator = null;
            break;
          }
        }
      }
      return enumerator?.Current;
    }


    #endregion
  }
}
