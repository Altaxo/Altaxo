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

    #region private Helper functions

    private static double Square(double x)
    {
      return x * x;
    }

    #endregion private Helper functions

    #region Inner types

    /// <summary>
    /// Provides a read-only vector with equal and constant items.
    /// </summary>
    private class ROConstantVector : IROVector
    {
      private int _length;
      private double _value;

      public ROConstantVector(double value, int length)
      {
        _length = length;
        _value = value;
      }

      #region IROVector Members

      public int Length
      {
        get { return _length; }
      }

      #endregion IROVector Members

      #region INumericSequence Members

      public double this[int i]
      {
        get { return _value; }
      }

      #endregion INumericSequence Members
    }

    /// <summary>
    /// Provides a read-only vector with equal and constant items.
    /// </summary>
    private class ROEquallySpacedVector : IROVector
    {
      private int _length;
      private double _startValue;
      private double _incrementValue;

      public ROEquallySpacedVector(double start, double increment, int length)
      {
        _length = length;
        _startValue = start;
        _incrementValue = increment;
      }

      #region IROVector Members

      public int Length
      {
        get { return _length; }
      }

      #endregion IROVector Members

      #region INumericSequence Members

      public double this[int i]
      {
        get { return _startValue + i * _incrementValue; }
      }

      #endregion INumericSequence Members
    }

    /// <summary>
    /// Serves as Wrapper for an double array to plug-in where a IROVector is neccessary.
    /// </summary>
    private class RODoubleArrayWrapper : IROVector
    {
      protected double[] _x;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public RODoubleArrayWrapper(double[] x)
      {
        _x = x;
        _length = _x.Length;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODoubleArrayWrapper(double[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _x = x;
        _length = usedlength;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i] { get { return _x[i]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }  // change this later to length property
    }

    private class RWDoubleArrayWrapper : RODoubleArrayWrapper, IVector
    {
      public RWDoubleArrayWrapper(double[] x)
        : base(x)
      {
      }

      public RWDoubleArrayWrapper(double[] x, int usedlength)
        : base(x, usedlength)
      {
      }

      #region IVector Members

      public new double this[int i]
      {
        get { return _x[i]; }
        set { _x[i] = value; }
      }

      #endregion IVector Members
    }

    /// <summary>
    /// Serves as Wrapper for an double array to plug-in where a IROVector is neccessary.
    /// </summary>
    private class RODoubleArraySectionWrapper : IROVector
    {
      protected double[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public RODoubleArraySectionWrapper(double[] x)
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
      public RODoubleArraySectionWrapper(double[] x, int start, int usedlength)
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

    private class RWDoubleArraySectionWrapper : RODoubleArraySectionWrapper, IVector
    {
      public RWDoubleArraySectionWrapper(double[] x)
        : base(x)
      {
      }

      public RWDoubleArraySectionWrapper(double[] x, int start, int usedlength)
        : base(x, start, usedlength)
      {
      }

      #region IVector Members

      public new double this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }

      #endregion IVector Members
    }

    /// <summary>
    /// Serves as wrapper for an IROVector to get only a section of the original wrapper.
    /// </summary>
    private class ROVectorSectionWrapper : IROVector
    {
      protected IROVector _x;
      private int _start;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Start index of the section to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public ROVectorSectionWrapper(IROVector x, int start, int len)
      {
        if (start >= x.Length)
          throw new ArgumentException("Start of the section is beyond length of the vector");
        if (start + len > x.Length)
          throw new ArgumentException("End of the section is beyond length of the vector");

        _x = x;
        _start = start;
        _length = len;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }  // change this later to length property
    }

    /// <summary>
    /// Serves as wrapper for an IVector to get only a section of the original wrapper.
    /// </summary>
    private class RWVectorSectionWrapper : IVector
    {
      protected IVector _x;
      private int _start;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Start index of the section to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RWVectorSectionWrapper(IVector x, int start, int len)
      {
        if (start >= x.Length)
          throw new ArgumentException("Start of the section is beyond length of the vector");
        if (start + len >= x.Length)
          throw new ArgumentException("End of the section is beyond length of the vector");

        _x = x;
        _start = start;
        _length = len;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }  // change this later to length property
    }

    /// <summary>
    /// Serves as Wrapper for an short array to plug-in where a IROVector is neccessary.
    /// </summary>
    private class ROIntArraySectionWrapper : IROVector
    {
      protected int[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public ROIntArraySectionWrapper(int[] x)
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
      public ROIntArraySectionWrapper(int[] x, int start, int usedlength)
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

    #region Filling

    /// <summary>
    /// Fills a vector with a certain value.
    /// </summary>
    /// <param name="x">The vector to fill.</param>
    /// <param name="val">The value each element is set to.</param>
    public static void Fill(IVector x, double val)
    {
      for (int i = x.Length - 1; i >= 0; --i)
        x[i] = val;
    }

    /// <summary>
    /// Reverses the order of elements in the provided vector.
    /// </summary>
    /// <param name="x">Vector. On return, the elements of this vector are in reverse order.</param>
    public static void Reverse(IVector x)
    {
      for (int i = 0, j = x.Length - 1; i < j; i++, j--)
      {
        var x_i = x[i];
        x[i] = x[j];
        x[j] = x_i;
      }
    }

    #endregion Filling

    #region copy

    /// <summary>
    /// Copies the source vector to the destination vector. Both vectors must have the same length.
    /// </summary>
    /// <param name="src">The source vector.</param>
    /// <param name="dest">The destination vector.</param>
    public static void Copy(IROVector src, IVector dest)
    {
      if (src.Length != dest.Length)
        throw new ArgumentException("src and destination vector have unequal length!");

      Copy(src, 0, dest, 0, src.Length);
    }

    /// <summary>
    /// Copies elements of a source vector to a destination vector.
    /// </summary>
    /// <param name="src">The source vector.</param>
    /// <param name="srcstart">First element of the source vector to copy.</param>
    /// <param name="dest">The destination vector.</param>
    /// <param name="deststart">First element of the destination vector to copy to.</param>
    /// <param name="count">Number of elements to copy.</param>
    public static void Copy(IROVector src, int srcstart, IVector dest, int deststart, int count)
    {
      for (int i = 0; i < count; i++)
        dest[i + deststart] = src[i + srcstart];
    }

    #endregion copy

    #region Arithmetic

    /// <summary>
    /// Multiplies all vector elements with a constant.
    /// </summary>
    /// <param name="v">The vector.</param>
    /// <param name="a">The constant to multiply with.</param>
    public static void Multiply(this IVector v, double a)
    {
      for (int i = v.Length - 1; i >= 0; --i)
        v[i] *= a;
    }

    /// <summary>
    /// Divides the specified x by y and returns the resulting vector.
    /// </summary>
    /// <param name="x">A scalar value</param>
    /// <param name="y">A vector.</param>
    /// <returns>The result of x/y.</returns>
    public static IVector Divide(double x, IROVector y)
    {
      var result = new DoubleVector(y.Length);
      for (int i = 0; i < y.Length; ++i)
        result[i] = x / y[i];
      return result;
    }

    #endregion Arithmetic

    #region Minimum / Maximum

    /// <summary>
    /// Returns the maximum value of all the valid elements in x (nonvalid elements, i.e. NaN values are not considered).
    /// </summary>
    /// <param name="x">The vector to search for maximum element.</param>
    /// <returns>Maximum valid element of x. Returns NaN if the array is empty or contains NaN elements only.</returns>
    public static double GetMaximumOfValidElements(this IROVector x)
    {
      double max = double.NaN;
      int i;
      for (i = x.Length - 1; i >= 0; --i)
      {
        if (!double.IsNaN(max = x[i]))
          break;
      }
      for (i = i - 1; i >= 0; --i)
      {
        if (x[i] > max)
          max = x[i];
      }
      return max;
    }

    /// <summary>
    /// Creates a new vector, whose elements are the maximum of the elements of a given input vector and a given number.
    /// </summary>
    /// <param name="v">Input vector.</param>
    /// <param name="b">Number.</param>
    /// <param name="result">Provide a vector whose elements are filled with the result. If null is provided, then a new vector will be allocated and returned.</param>
    /// <returns>A vector (either the provided result vector or a new one). Each element r[i] is the maximum of v[i] and b.</returns>
    public static IVector Max(this IROVector v, double b, IVector result)
    {
      if (null == result)
        result = _funcCreateNewVector(v.Length);

      for (int i = 0; i < v.Length; i++)
        result[i] = Math.Max(v[i], b);

      return result;
    }

    #endregion Minimum / Maximum

    /// <summary>
    /// Returns the used length of the vector. This is one more than the highest index of the element that is different from Double.NaN.
    /// </summary>
    /// <param name="values">The vector for which the used length has to be determined.</param>
    /// <param name="currentlength">The current length of the array. Normally values.Length, but you can provide a value less than this.</param>
    /// <returns>The used length. Elements with indices greater or equal this until <c>currentlength</c> are NaNs.</returns>
    static public int GetUsedLength(this IROVector values, int currentlength)
    {
      for (int i = currentlength - 1; i >= 0; --i)
      {
        if (!Double.IsNaN(values[i]))
          return i + 1;
      }
      return 0;
    }

    /// <summary>
    /// Returns the used length of the vector. This is one more than the highest index of the element that is different from Double.NaN.
    /// </summary>
    /// <param name="values">The vector for which the used length has to be determined.</param>
    /// <returns>The used length. Elements with indices greater or equal this until the end of the array are NaNs.</returns>
    static public int GetUsedLength(IROVector values)
    {
      return GetUsedLength(values, values.Length);
    }

    /// <summary>
    /// Returns the sum of the elements in the vector.
    /// </summary>
    /// <param name="xarray">The vector.</param>
    /// <returns>The sum of all elements in xarray.</returns>
    public static double Sum(this IROVector xarray)
    {
      double sum = 0;
      for (int i = 0; i < xarray.Length; i++)
        sum += xarray[i];

      return sum;
    }

    /// <summary>
    /// Returns the sum of squared differences of the elements of xarray and yarray.
    /// </summary>
    /// <param name="xarray">The first array.</param>
    /// <param name="yarray">The other array.</param>
    /// <returns>The sum of squared differences all elements of xarray and yarray.</returns>
    public static double SumOfSquaredDifferences(double[] xarray, double[] yarray)
    {
      if (xarray.Length != yarray.Length)
        throw new ArgumentException("Length of xarray is unequal length of yarray");

      double sum = 0;
      for (int i = 0; i < xarray.Length; i++)
        sum += Square(xarray[i] - yarray[i]);

      return sum;
    }

    /// <summary>
    /// Returns the sum of squared differences of the elements of xarray and yarray.
    /// </summary>
    /// <param name="xarray">The first array.</param>
    /// <param name="yarray">The other array.</param>
    /// <returns>The sum of squared differences all elements of xarray and yarray.</returns>
    public static double SumOfSquaredDifferences(IROVector xarray, IROVector yarray)
    {
      if (xarray.Length != yarray.Length)
        throw new ArgumentException("Length of xarray is unequal length of yarray");

      double sum = 0;
      for (int i = 0; i < xarray.Length; i++)
        sum += Square(xarray[i] - yarray[i]);

      return sum;
    }

    private static double sqr(double x)
    {
      return x * x;
    }

    /// <summary>Given an n-vector x, this function calculates the
    /// euclidean norm of x.
    /// </summary>
    /// <param name="x">An input array. </param>
    /// <returns>The euclidian norm of the vector of length n, i.e. the square root of the sum of squares of the elements.</returns>
    public static double GetNorm(double[] x)
    {
      return GetNorm(x, 0, x.Length);
    }

    /// <summary>Given an n-vector x, this function calculates the
    /// euclidean norm of x.
    /// </summary>
    /// <param name="n">A positive integer input variable of the number of elements to process.</param>
    /// <param name="x">An input array of length n. </param>
    /// <param name="startindex">The index of the first element in x to process.</param>
    /// <returns>The euclidian norm of the vector of length n, i.e. the square root of the sum of squares of the elements.</returns>
    /// <remarks>
    ///     the euclidean norm is computed by accumulating the sum of
    ///     squares in three different sums. the sums of squares for the
    ///     small and large components are scaled so that no overflows
    ///     occur. non-destructive underflows are permitted. underflows
    ///     and overflows do not occur in the computation of the unscaled
    ///     sum of squares for the intermediate components.
    ///     the definitions of small, intermediate and large components
    ///     depend on two constants, rdwarf and rgiant. the main
    ///     restrictions on these constants are that rdwarf**2 not
    ///     underflow and rgiant**2 not overflow. the constants
    ///     given here are suitable for every known computer.
    ///     <para>burton s. garbow, kenneth e. hillstrom, jorge j. more</para>
    ///
    /// </remarks>
    public static double GetNorm(double[] x, int startindex, int n)
    {
      const double rdwarf = 3.834e-20;
      const double rgiant = 1.304e19;
      double ret_val = 0.0, xabs, x1max, x3max, s1, s2, s3, agiant, floatn;
      int i;

      // Parameter adjustments
      // --x; LELLID!!

      s1 = s2 = s3 = x1max = x3max = 0.0;
      floatn = (double)n;
      agiant = rgiant / floatn;

      for (i = 0; i < n; i++)
      { // LELLID!!
        xabs = Math.Abs(x[i + startindex]);
        if (xabs > rdwarf && xabs < agiant) goto L70;
        if (xabs <= rdwarf) goto L30;

        //sum for large components
        if (xabs <= x1max) goto L10;
        s1 = 1.0 + s1 * sqr(x1max / xabs);
        x1max = xabs;
        goto L80;

        L10:
        s1 += sqr(xabs / x1max);
        goto L80;

        L30:
        // sum for small components
        if (xabs <= x3max) goto L40;
        s3 = 1.0 + s3 * sqr(x3max / xabs);
        x3max = xabs;
        goto L80;

        L40:
        if (xabs != 0.0) s3 += sqr(xabs / x3max);
        goto L80;

        L70:
        // sum for intermediate components
        s2 += sqr(xabs);

        L80:;
      }

      // calculation of norm
      if (s1 == 0.0) goto L100;
      ret_val = x1max * Math.Sqrt(s1 + s2 / x1max / x1max);
      goto L130;

      L100:
      if (s2 == 0.0) goto L110;
      if (s2 >= x3max)
        ret_val = Math.Sqrt(s2 * (1.0 + x3max / s2 * (x3max * s3)));
      if (s2 < x3max)
        ret_val = Math.Sqrt(x3max * (s2 / x3max + x3max * s3));
      goto L130;

      L110:
      ret_val = x3max * Math.Sqrt(s3);

      L130:
      return ret_val;
    }

    /// <summary>
    /// Returns true if and only if both vectors contain the same elements. Both vectors must have the same length.
    /// </summary>
    /// <param name="x">First vector.</param>
    /// <param name="y">Second vector.</param>
    /// <returns>True if both vectors contain the same elements.</returns>
    public static bool AreValuesEqual(IROVector x, IROVector y)
    {
      if (x.Length != y.Length)
        throw new ArgumentException("Length of x and y are unequal");

      for (int i = x.Length - 1; i >= 0; i--)
        if (!(x[i] == y[i]))
          return false;

      return true;
    }

    /// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing or decreasing.
    /// </summary>
    /// <param name="x">Vector (sequence) to test.</param>
    /// <returns>True if the sequence is strictly increasing or decreasing.</returns>
    public static bool IsStrictlyIncreasingOrDecreasing(IROVector x)
    {
      bool isDecreasing;
      return IsStrictlyIncreasingOrDecreasing(x, out isDecreasing);
    }

    /// <summary>
    /// Returns true if the sequence given by the vector argument is strictly increasing or decreasing.
    /// </summary>
    /// <param name="x">Vector (sequence) to test.</param>
    /// <param name="isDecreasing">On return, this argument is set to true if the sequence is strictly decreasing. If increasing, this argument is set to false.</param>
    /// <returns>True if the sequence is strictly increasing or decreasing.</returns>
    public static bool IsStrictlyIncreasingOrDecreasing(IROVector x, out bool isDecreasing)
    {
      isDecreasing = false;
      if (x.Length == 0)
        return false;
      int sign = Math.Sign(x[x.Length - 1] - x[0]);
      if (sign == 0)
        return false;

      isDecreasing = (sign < 0);

      for (int i = x.Length - 1; i >= 1; --i)
        if (Math.Sign(x[i] - x[i - 1]) != sign)
          return false;

      return true;
    }

    /// <summary>
    /// Fills the vector v with a linear sequence beginning from start (first element) until end (last element).
    /// </summary>
    /// <param name="v">The vector to be filled.</param>
    /// <param name="start">First value of the sequence.</param>
    /// <param name="end">Last value of the sequence.</param>
    public static void FillWithLinearSequenceGivenByStartEnd(this IVector v, double start, double end)
    {
      int len = v.Length;
      if (len == 1)
      {
        v[0] = start;
        return;
      }
      else if (len > 1)
      {
        for (int i = 0; i < len; ++i)
          v[i] = start + i * (end - start) / (len - 1);
      }
    }

    /// <summary>
    /// Applies a function to every element of a vector.
    /// </summary>
    /// <param name="v">The vector.</param>
    /// <param name="func">The function to apply to every element.</param>
    public static void Apply(this IVector v, Func<double, double> func)
    {
      for (int i = v.Length - 1; i >= 0; --i)
        v[i] = func(v[i]);
    }

    public static bool Any(this IROVector v, Func<double, bool> func)
    {
      int firstIndex;
      return Any(v, func, out firstIndex);
    }

    public static bool Any(this IROVector v, Func<double, bool> func, out int firstIndex)
    {
      int len = v.Length;
      for (int i = 0; i < len; ++i)
        if (func(v[i]))
        {
          firstIndex = i;
          return true;
        }

      firstIndex = -1;
      return false;
    }

    /// <summary>
    /// Shifts the element of this vector by moving them <c>increment</c> times to the right. The elements on the rightmost side are shifted back into the left side of the vector. Thus, effectively, the elements are rotated to the right.
    /// </summary>
    /// <param name="x">The vector to rotate.</param>
    /// <param name="increment"></param>
    public static void Rotate(this IVector x, int increment)
    {
      int xLen = x.Length;
      if (xLen < 2)
        return; // Nothing to do
      increment = increment % xLen;
      if (increment == 0)
        return;
      if (increment < 0)
        increment += xLen;

      // first cycle is to measure number of shifts per cycle
      int shiftsPerCycle = 0;
      int i = 0;
      int k = i;
      double prevVal = x[k];
      do
      {
        k = (k + increment) % xLen;
        double currVal = x[k];
        x[k] = prevVal;
        prevVal = currVal;
        shiftsPerCycle++;
      } while (i != k);

      // now do the rest of the cycles
      if (!(0 == xLen % shiftsPerCycle)) throw new InvalidProgramException();
      int numCycles = xLen / shiftsPerCycle;
      for (i = 1; i < numCycles; i++)
      {
        k = i;
        prevVal = x[k];
        do
        {
          k = (k + increment) % xLen;
          double currVal = x[k];
          x[k] = prevVal;
          prevVal = currVal;
        } while (i != k);
      }
    }
  }
}