#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// VectorMath provides common static functions concerning vectors.
  /// </summary>
  public static partial class VectorMath
  {
    private static Func<int, IVector> _funcCreateNewVector = DefaultCreateNewVector;

    public static IVector DefaultCreateNewVector(int length)
    {
      return new DoubleVector(length);
    }

    #region Inner types

    /// <summary>
    /// Serves as Wrapper for an short array to plug-in where a IROVector is neccessary.
    /// </summary>
    private class ROShortArraySectionWrapper : IROVector
    {
      protected short[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public ROShortArraySectionWrapper(short[] x)
      {
        _x = x;
        _start = 0;
        _length = _x.Length;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROShortArraySectionWrapper(short[] x, int start, int usedlength)
      {
        if (start < 0)
          throw new ArgumentException("start is negative");
        if (usedlength < 0)
          throw new ArgumentException("usedlength is negative");

        if ((start + usedlength) > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _x = x;
        _start = start;
        _length = usedlength;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }  // change this later to length property
    }

    /// <summary>
    /// Serves as Wrapper for an signed byte array to plug-in where a IROVector is neccessary.
    /// </summary>
    private class ROSByteArraySectionWrapper : IROVector
    {
      protected sbyte[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public ROSByteArraySectionWrapper(sbyte[] x)
      {
        _x = x;
        _start = 0;
        _length = _x.Length;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROSByteArraySectionWrapper(sbyte[] x, int start, int usedlength)
      {
        if (start < 0)
          throw new ArgumentException("start is negative");
        if (usedlength < 0)
          throw new ArgumentException("usedlength is negative");

        if ((start + usedlength) > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _x = x;
        _start = start;
        _length = usedlength;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }  // change this later to length property
    }

    private class ExtensibleVector : IExtensibleVector
    {
      private double[] _arr;
      private int _length;

      public ExtensibleVector(int initiallength)
      {
        _arr = new double[initiallength];
        _length = initiallength;
      }

      #region IVector Members

      public double this[int i]
      {
        get
        {
          return _arr[i];
        }
        set
        {
          _arr[i] = value;
        }
      }

      #endregion IVector Members

      #region IROVector Members

      double Altaxo.Calc.LinearAlgebra.INumericSequence.this[int i]
      {
        get
        {
          return _arr[i];
        }
      }

      public int Length
      {
        get
        {
          return _length;
        }
      }

      #endregion IROVector Members

      #region IExtensibleVector Members

      public void Append(IROVector a)
      {
        if (_length + a.Length >= _arr.Length)
          Redim((int)(32 + 1.3 * (_length + a.Length)));

        for (int i = 0; i < a.Length; i++)
          _arr[i + _length] = a[i];
        _length += a.Length;
      }

      #endregion IExtensibleVector Members

      private void Redim(int newsize)
      {
        if (newsize > _arr.Length)
        {
          double[] oldarr = _arr;
          _arr = new double[newsize];
          Array.Copy(oldarr, 0, _arr, 0, _length);
        }
      }
    }

    private class EquidistantSequenceVectorStartStepLength : IROVector
    {
      private double _start;
      private double _step;
      private int _len;

      public EquidistantSequenceVectorStartStepLength(double start, double step, int length)
      {
        _start = start;
        _step = step;
        _len = length;
      }

      #region IROVector Members

      public int Length
      {
        get { return _len; }
      }

      #endregion IROVector Members

      #region INumericSequence Members

      public double this[int i]
      {
        get
        {
          if (i < 0 || i >= _len)
            throw new ArgumentOutOfRangeException("i");
          return _start + i * _step;
        }
      }

      #endregion INumericSequence Members
    }

    private class EquidistantSequenceVectorStartAtOffsetStepLength : IROVector
    {
      private double _start;
      private int _startOffset;

      private double _step;
      private int _len;

      public EquidistantSequenceVectorStartAtOffsetStepLength(double start, int startOffset, double step, int length)
      {
        _start = start;
        _startOffset = startOffset;
        _step = step;
        _len = length;
      }

      #region IROVector Members

      public int Length
      {
        get { return _len; }
      }

      #endregion IROVector Members

      #region INumericSequence Members

      public double this[int i]
      {
        get
        {
          if (i < 0 || i >= _len)
            throw new ArgumentOutOfRangeException("i");
          return _start + (i - _startOffset) * _step;
        }
      }

      #endregion INumericSequence Members
    }

    private class EquidistantSequenceVectorStartEndLength : IROVector
    {
      private double _start;
      private double _end;
      private int _len;

      public EquidistantSequenceVectorStartEndLength(double start, double end, int length)
      {
        _start = start;
        _end = end;
        _len = length;
      }

      #region IROVector Members

      public int Length
      {
        get { return _len; }
      }

      #endregion IROVector Members

      #region INumericSequence Members

      public double this[int i]
      {
        get
        {
          if (i < 0 || i >= _len)
            throw new ArgumentOutOfRangeException("i");

          double r = i / (double)(_len - 1);
          return _start * (1 - r) + _end * (r);
        }
      }

      #endregion INumericSequence Members
    }

    #endregion Inner types

    #region Type conversion

    public static IROVector GetConstantVector(double value, int length)
    {
      return new ROConstantVector(value, length);
    }

    #region From/To double[]

    /// <summary>
    /// Wraps a double[] array to get a IROVector.
    /// </summary>
    /// <param name="x">The array to wrap.</param>
    /// <returns>A wrapper objects with the <see cref="IROVector" /> interface that wraps the provided array.</returns>
    public static IROVector ToROVector(double[] x)
    {
      return null == x ? null : new RODoubleArrayWrapper(x);
    }

    /// <summary>
    /// Wraps a double[] array till a given length to get a IROVector.
    /// </summary>
    /// <param name="x">The array to wrap.</param>
    /// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
    /// <returns>A wrapper objects with the <see cref="IROVector" /> interface that wraps the provided array.</returns>
    public static IROVector ToROVector(double[] x, int usedlength)
    {
      return new RODoubleArrayWrapper(x, usedlength);
    }

    /// <summary>
    /// Wraps a double[] array till a given length to get a IROVector.
    /// </summary>
    /// <param name="x">The array to wrap.</param>
    /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
    /// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
    /// <returns>A wrapper objects with the <see cref="IROVector" /> interface that wraps the provided array.</returns>
    public static IROVector ToROVector(double[] x, int start, int usedlength)
    {
      if (0 == start)
        return new RODoubleArrayWrapper(x, usedlength);
      else
        return new RODoubleArraySectionWrapper(x, start, usedlength);
    }

    /// <summary>
    /// Wraps an int[] array till a given length to get a IROVector.
    /// </summary>
    /// <param name="x">The array to wrap.</param>
    /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
    /// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
    /// <returns>A wrapper objects with the <see cref="IROVector" /> interface that wraps the provided array.</returns>
    public static IROVector ToROVector(int[] x, int start, int usedlength)
    {
      return new ROIntArraySectionWrapper(x, start, usedlength);
    }

    /// <summary>
    /// Wraps a short[] array till a given length to get a IROVector.
    /// </summary>
    /// <param name="x">The array to wrap.</param>
    /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
    /// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
    /// <returns>A wrapper objects with the <see cref="IROVector" /> interface that wraps the provided array.</returns>
    public static IROVector ToROVector(short[] x, int start, int usedlength)
    {
      return new ROShortArraySectionWrapper(x, start, usedlength);
    }

    /// <summary>
    /// Wraps a sbyte[] array till a given length to get a IROVector.
    /// </summary>
    /// <param name="x">The array to wrap.</param>
    /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
    /// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
    /// <returns>A wrapper objects with the <see cref="IROVector" /> interface that wraps the provided array.</returns>
    public static IROVector ToROVector(sbyte[] x, int start, int usedlength)
    {
      return new ROSByteArraySectionWrapper(x, start, usedlength);
    }

    /// <summary>
    /// Wraps a double[] array to get a IVector.
    /// </summary>
    /// <param name="x">The array to wrap.</param>
    /// <returns>A wrapper objects with the <see cref="IVector" /> interface that wraps the provided array.</returns>
    public static IVector ToVector(double[] x)
    {
      return new RWDoubleArrayWrapper(x);
    }

    /// <summary>
    /// Wraps a double[] array to get a IVector.
    /// </summary>
    /// <param name="x">The array to wrap.</param>
    /// <param name="usedlength">Used length of the array to get the wrapped vector (i.e. the vector wraps around <paramref name="x"/>[0..usedLength-1]).</param>
    /// <returns>A wrapper objects with the <see cref="IVector" /> interface that wraps the provided array.</returns>
    public static IVector ToVector(double[] x, int usedlength)
    {
      return new RWDoubleArrayWrapper(x, usedlength);
    }

    /// <summary>
    /// Wraps part of a double[] array to get a IVector.
    /// </summary>
    /// <param name="x">The array to wrap.</param>
    /// <param name="start">Index of first element of <paramref name="x"/> to use.</param>
    /// <param name="count">Number of elements of <paramref name="x"/> to use.</param>
    /// <returns>A wrapper objects with the <see cref="IVector" /> interface that wraps part of the provided array.</returns>
    public static IVector ToVector(double[] x, int start, int count)
    {
      if (0 == start)
        return new RWDoubleArrayWrapper(x, count);
      else
        return new RWDoubleArraySectionWrapper(x, start, count);
    }

    #endregion From/To double[]

    #region from/to IROVector

    /// <summary>
    /// Wraps a section of a original vector <c>x</c> into a new vector.
    /// </summary>
    /// <param name="x">Original vector.</param>
    /// <param name="start">Index of the start of the section to wrap.</param>
    /// <param name="len">Length (=number of elements) of the section to wrap.</param>
    /// <returns>A IROVector that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
    public static IROVector ToROVector(this IROVector x, int start, int len)
    {
      return new ROVectorSectionWrapper(x, start, len);
    }

    #endregion from/to IROVector

    #region from/to IVector

    /// <summary>
    /// Wraps a section of a original vector <c>x</c> into a new vector.
    /// </summary>
    /// <param name="x">Original vector.</param>
    /// <param name="start">Index of the start of the section to wrap.</param>
    /// <param name="len">Length (=number of elements) of the section to wrap.</param>
    /// <returns>A IVector that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
    public static IVector ToVector(IVector x, int start, int len)
    {
      return new RWVectorSectionWrapper(x, start, len);
    }

    #endregion from/to IVector

    /// <summary>
    /// Creates a new extensible vector of length <c>length</c>
    /// </summary>
    /// <param name="length">The inital length of the vector.</param>
    /// <returns>An instance of a extensible vector.</returns>
    public static IExtensibleVector CreateExtensibleVector(int length)
    {
      return new ExtensibleVector(length);
    }

    /// <summary>
    /// Creates a read-only vector with equidistant elements from start to start+(length-1)*step. The created vector
    /// consumes memory only for the three variables, independent of its length.
    /// </summary>
    /// <param name="start">First element of the vector.</param>
    /// <param name="step">Difference between two successive elements.</param>
    /// <param name="length">Length of the vector.</param>
    /// <returns></returns>
    public static IROVector CreateEquidistantSequenceByStartStepLength(double start, double step, int length)
    {
      return new EquidistantSequenceVectorStartStepLength(start, step, length);
    }

    /// <summary>
    /// Creates a read-only vector with equidistant elements from start to end. The created vector
    /// consumes memory only for the three variables, independent of its length.
    /// </summary>
    /// <param name="start">First element of the vector.</param>
    /// <param name="end">Last element of the vector.</param>
    /// <param name="length">Length of the vector.</param>
    /// <returns></returns>
    public static IROVector CreateEquidistantSequenceByStartEndLength(double start, double end, int length)
    {
      return new EquidistantSequenceVectorStartEndLength(start, end, length);
    }

    /// <summary>
    /// Creates a read-only vector with equidistant elements from start - startOffset*step to start + (length - 1 -startOffset)*step. The created vector
    /// consumes memory only for the three variables, independent of its length.
    /// </summary>
    /// <param name="start">Value of the element of the vector at index <paramref name="startOffset"/>).</param>
    /// <param name="startOffset">Index of the element of the vector which gets the value of <paramref name="start"/>.</param>
    /// <param name="step">Difference between two successive elements.</param>
    /// <param name="length">Length of the vector.</param>
    /// <returns>Read-only vector with equidistant elements from start - startOffset*step to start + (length - 1 -startOffset)*step.</returns>
    public static IROVector CreateEquidistantSequencyByStartAtOffsetStepLength(double start, int startOffset, double step, int length)
    {
      return new EquidistantSequenceVectorStartAtOffsetStepLength(start, startOffset, step, length);
    }

    #endregion Type conversion
  }
}