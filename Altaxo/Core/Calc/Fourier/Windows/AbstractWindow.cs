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
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Fourier.Windows
{
  /// <summary>
  /// Base class for window functions used in Fourier transforms.
  /// Provides helpers to compute window values in different container types and ranges.
  /// </summary>
  public abstract class AbstractWindow
  {
    private int _count;
    private bool _periodic;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractWindow"/> class.
    /// </summary>
    /// <param name="count">The number of samples of the window. Must be greater than zero.</param>
    /// <param name="periodic">If set to <c>true</c>, the window is considered periodic.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="count"/> is zero or negative.</exception>
    public AbstractWindow(int count, bool periodic)
    {
      if (count <= 0)
        throw new ArgumentException("Width of the window must not be zero or negative");

      _count = count;
      _periodic = periodic;
    }

    /// <summary>
    /// Computes the window values into the provided vector.
    /// </summary>
    /// <param name="array">The target vector where the window values are written.</param>
    /// <param name="periodic">If set to <c>true</c> the window is computed in periodic mode.</param>
    protected abstract void InternalCompute(IVector<double> array, bool periodic);

    /// <summary>
    /// Returns the window values as a newly allocated <see cref="double"/> array.
    /// </summary>
    /// <returns>An array containing the window values. If the window length is 1, the single value is 1.</returns>
    public double[] AsDoubleArray()
    {
      double[] result = new double[_count];
      if (_count == 1)
        result[0] = 1;
      else
        InternalCompute(VectorMath.ToVector(result), _periodic);

      return result;
    }

    /// <summary>
    /// Returns the window values as a read-only vector.
    /// </summary>
    /// <returns>A read-only view over the window values.</returns>
    public IReadOnlyList<double> AsROVector()
    {
      return VectorMath.ToROVector(AsDoubleArray());
    }

    /// <summary>
    /// Returns the window values as an <see cref="IVector{Double}"/> instance.
    /// </summary>
    /// <returns>A vector containing the window values.</returns>
    public IVector<double> AsVector()
    {
      return VectorMath.ToVector(AsDoubleArray());
    }

    /// <summary>
    /// Computes the window values into the provided array.
    /// </summary>
    /// <param name="array">The array that receives the window values.</param>
    /// <param name="periodic">If set to <c>true</c> the window is computed in periodic mode.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="array"/> has length zero.</exception>
    public void Compute(double[] array, bool periodic)
    {
      if (array is null)
        throw new ArgumentNullException(nameof(array));
      if (array.Length == 0)
        throw new ArgumentException("array length is null");
      InternalCompute(VectorMath.ToVector(array), periodic);
    }

    /// <summary>
    /// Computes the window values into a sub-range of the provided array.
    /// </summary>
    /// <param name="array">The array that receives the window values.</param>
    /// <param name="startidx">The start index in <paramref name="array"/> to begin writing values.</param>
    /// <param name="count">The number of values to write.</param>
    /// <param name="periodic">If set to <c>true</c> the window is computed in periodic mode.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="array"/> has length zero, when <paramref name="startidx"/> is negative,
    /// when <paramref name="count"/> is less than one, or when <paramref name="startidx"/> + <paramref name="count"/> exceeds the array length.
    /// </exception>
    public void Compute(double[] array, int startidx, int count, bool periodic)
    {
      if (array is null)
        throw new ArgumentNullException(nameof(array));
      if (array.Length == 0)
        throw new ArgumentException("array length is null");
      if (startidx < 0)
        throw new ArgumentException("startidx is negative");
      if (count < 1)
        throw new ArgumentException("count is null or negative");
      if ((startidx + count) > array.Length)
        throw new ArgumentException("startidx+count exceeds the length of the array");

      InternalCompute(VectorMath.ToVector(array, startidx, count), periodic);
    }

    /// <summary>
    /// Computes the window values into the provided vector instance.
    /// </summary>
    /// <param name="array">The vector that receives the window values.</param>
    /// <param name="periodic">If set to <c>true</c> the window is computed in periodic mode.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="array"/> has zero length.</exception>
    public void Compute(IVector<double> array, bool periodic)
    {
      if (array is null)
        throw new ArgumentNullException("array is null");
      if (array.Count == 0)
        throw new ArgumentException("array length is null");
      InternalCompute(array, periodic);
    }

    /// <summary>
    /// Computes the window values into a sub-range of the provided vector instance.
    /// </summary>
    /// <param name="array">The vector that receives the window values.</param>
    /// <param name="startidx">The start index in <paramref name="array"/> to begin writing values.</param>
    /// <param name="count">The number of values to write.</param>
    /// <param name="periodic">If set to <c>true</c> the window is computed in periodic mode.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="array"/> has zero length, when <paramref name="startidx"/> is negative,
    /// when <paramref name="count"/> is less than one, or when <paramref name="startidx"/> + <paramref name="count"/> exceeds the vector length.
    /// </exception>
    public void Compute(IVector<double> array, int startidx, int count, bool periodic)
    {
      if (array is null)
        throw new ArgumentNullException("array is null");
      if (array.Count == 0)
        throw new ArgumentException("array length is null");
      if (startidx < 0)
        throw new ArgumentException("startidx is negative");
      if (count < 1)
        throw new ArgumentException("count is null or negative");
      if ((startidx + count) > array.Count)
        throw new ArgumentException("startidx+count exceeds the length of the array");

      InternalCompute(VectorMath.ToVector(array, startidx, count), periodic);
    }
  }
}
