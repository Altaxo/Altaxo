#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.LinearAlgebra
{
  // Unary functions not returning a vector, valid for all vector types

  public static partial class VectorMath
  {

// ******************************************* Unary functions not returning a vector, valid for all non-null vector types  ********************

// ******************************************** Definitions for double *******************************************

    /// <summary>
		/// Provides a read-only vector with equal and constant items.
		/// </summary>
		private class RODoubleConstantVector : IReadOnlyList<double>
		{
			private int _length;
			private double _value;

			public RODoubleConstantVector(double value, int length)
			{
				_length = length;
				_value = value;
			}

			public int Length
			{
				get { return _length; }
			}

      public int Count
      {
        get { return _length; }
      }

      public double this[int i]
			{
				get { return _value; }
			}

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return _value;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return _value;
      }
    }

			/// <summary>
		/// Gets a vector with all elements equal to a provided value.
		/// </summary>
		/// <param name="value">Value of all elements.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with all elements equal to the provided <paramref name="value"/>.</returns>
		public static IReadOnlyList<double> GetConstantVector(double value, int length)
		{
			return new RODoubleConstantVector(value, length);
		}


		/// <summary>
    /// Provides a read-only vector with equally spaced elements y[i] = start + i * increment.
    /// </summary>
    private class RODoubleEquidistantElementVector : IReadOnlyList<double>
    {
      private int _length;
      private double _startValue;
      private double _incrementValue;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
      public RODoubleEquidistantElementVector(double start, double increment, int length)
      {
        _length = length;
        _startValue = start;
        _incrementValue = increment;
      }

			/// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get { return _length; }
      }

      	/// <summary>The number of elements of this vector.</summary>
			public int Count
      {
        get { return _length; }
      }

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
      public double this[int i]
      {
        get { return (double)(_startValue + i * _incrementValue); }
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }


			/// <summary>
		/// Creates a read-only vector with equidistant elements with values from start to start+(length-1)*step. The created vector
		/// consumes memory only for the three variables, independent of its length.
		/// </summary>
		/// <param name="start">First element of the vector.</param>
		/// <param name="step">Difference between two successive elements.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with equidistant elements with values from start to start+(length-1)*step.</returns>
		public static IReadOnlyList<double> CreateEquidistantSequenceByStartStepLength(double start, double step, int length)
		{
			return new RODoubleEquidistantElementVector(start, step, length);
		}

			
			/// <summary>
		/// Provides a read-only vector with equally spaced elements y[i] = start + (i-startOffset) * increment.
		/// </summary>
		private class RODoubleEquidistantElementVectorStartAtOffsetStepLength : IReadOnlyList<double>
		{
			private double _start;
			private int _startOffset;

			private double _increment;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the element at index <paramref name="startOffset"/> of the vector.</param>
			/// <param name="startOffset">The index of the element for which a value is provided in <paramref name="start"/>.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
			public RODoubleEquidistantElementVectorStartAtOffsetStepLength(double start, int startOffset, double increment, int length)
			{
				_start = start;
				_startOffset = startOffset;
				_increment = increment;
				_length = length;
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Length
			{
				get { return _length; }
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Count
			{
				get { return _length; }
			}

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
			public double this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");
					return (double)(_start + (i - _startOffset) * _increment);
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i<_length; ++i)
          yield return this[i];
      }

		/// <summary>
		/// Enumerates all elements of the vector.
		/// </summary>
		/// <returns>Enumerator that enumerates all elements of the vector.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < _length; ++i)
				yield return this[i];
		}
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
		public static IReadOnlyList<double> CreateEquidistantSequencyByStartAtOffsetStepLength(double start, int startOffset, double step, int length)
		{
			return new RODoubleEquidistantElementVectorStartAtOffsetStepLength(start, startOffset, step, length);
		}


	  	/// <summary>
		/// Provides a read-only vector with equally spaced elements so that y[0] = start and y[length-1] = end.
		/// </summary>
		private class RODoubleEquidistantElementVectorStartEndLength : IReadOnlyList<double>
		{
			private double _start;
			private double _end;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="end">Value of the last element of the vector.</param>
			/// <param name="length">Length of the vector.</param>
			public RODoubleEquidistantElementVectorStartEndLength(double start, double end, int length)
			{
				_start = start;
				_end = end;
				_length = length;
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Length
			{
				get { return _length; }
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Count
			{
				get { return _length; }
			}

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
			public double this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");

					double r = i / (double)(_length - 1);
					return (double)(_start * (1 - r) + _end * (r));
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i<_length; ++i)
          yield return this[i];
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _length; ++i)
					yield return this[i];
			}
		}

		/// <summary>
		/// Creates a read-only vector with equidistant element values from start to end. The created vector
		/// consumes memory only for the three variables, independent of its length.
		/// </summary>
		/// <param name="start">First element of the vector.</param>
		/// <param name="end">Last element of the vector.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with equidistant element values from start to end.</returns>
		public static IReadOnlyList<double> CreateEquidistantSequenceByStartEndLength(double start, double end, int length)
		{
			return new RODoubleEquidistantElementVectorStartEndLength(start, end, length);
		}


		/// <summary>
    /// Serves as wrapper for an RO Vector which returns the inverse of the elements of the original array.
    /// </summary>
    private class RODoubleInverseElementWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected IReadOnlyList<double> _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODoubleInverseElementWrapper(IReadOnlyList<double> x)
      {
        _x = x;
        _length = x.Count;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODoubleInverseElementWrapper(IReadOnlyList<double> x, int usedlength)
      {
        if (usedlength > x.Count)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
          return 1/(double)_x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a double[] array to get an  <see cref="IROVector{double}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{double}" /> returning elements that are inverse to those of the original vector.</returns>
		public static IReadOnlyList<double> ToInverseROVector(this IReadOnlyList<double> array)
		{
			return array is null ? null : new RODoubleInverseElementWrapper(array);
		}

		/// <summary>
		/// Wraps a double[] array till a given length to get an <see cref="IROVector{double}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{double}" /> interface that wraps the provided array returning elements that are inverse to those of the original vector..</returns>
		public static IReadOnlyList<double> ToInverseROVector(this IReadOnlyList<double> array, int usedlength)
		{
			return new RODoubleInverseElementWrapper(array, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{double}" /> is neccessary.
    /// </summary>
    private class RODoubleArrayWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected double[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODoubleArrayWrapper(double[] x)
      {
        _length = x.Length;
        _x = x;
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

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
          return _x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a double[] array to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToROVector(this double[] array)
		{
			return array is null ? null : new RODoubleArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a double[] array till a given length to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToROVector(this double[] array, int usedlength)
		{
			return new RODoubleArrayWrapper(array, usedlength);
		}

			/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{double}" /> is neccessary.
    /// </summary>
    private class RODouble_DoubleArrayWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected double[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODouble_DoubleArrayWrapper(double[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODouble_DoubleArrayWrapper(double[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
          return _x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return (double)this[i];
      }
    }

		/// <summary>
		/// Wraps a double[] array to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this double[] array)
		{
			return array is null ? null : new RODouble_DoubleArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a double[] array till a given length to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this double[] array, int usedlength)
		{
			return new RODouble_DoubleArrayWrapper(array, usedlength);
		}


		  /// <summary>
    /// Serves as wrapper for a section of an array to plug-in where an <see cref="IROVector{double}" /> is neccessary.
    /// </summary>
    private class RODoubleArraySectionWrapper : IReadOnlyList<double>
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
        _length = x.Length;
        _start = 0;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array, and start and length of the section, for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap.</param>
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
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      /// <summary>
      /// Returns an enumerator that iterates through the elements of the vector.
      /// </summary>
      /// <returns>
      /// An enumerator that can be used to iterate through the elements of the vector.
      /// </returns>
      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps an array to an <see cref="IROVector{double}" />. Start and length of the used section of the array are specified in the parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of the element in <paramref name="array"/> used as the first element of the vector.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper object with the <see cref="IROVector{double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToROVector(this double[] array, int start, int usedlength)
		{
			if (0 == start)
				return new RODoubleArrayWrapper(array, usedlength);
			else
				return new RODoubleArraySectionWrapper(array, start, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{double}" /> is neccessary.
    /// </summary>
    private class RODoubleArrayWrapperAmendedShifted : IReadOnlyList<double>
    {
      private int _length;
      protected double[] _x;
			protected double _amendedValueAtStart;
			protected int _amendedValuesAtStartCount;
			protected double _amendedValueAtEnd;
			protected int _amendedValuesAtEndCount;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public RODoubleArrayWrapperAmendedShifted(double[] x, double amendedValueAtStart, int amendedValuesAtStartCount, double amendedValueAtEnd, int amendedValuesAtEndCount)
      {
        _length = x.Length;
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValuesAtStartCount = amendedValuesAtStartCount;
			 _amendedValueAtEnd = amendedValueAtEnd;
			 _amendedValuesAtEndCount = amendedValuesAtEndCount;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public RODoubleArrayWrapperAmendedShifted(double[] x, int usedlength, double amendedValueAtStart, int amendedValuesAtStartCount, double amendedValueAtEnd, int amendedValuesAtEndCount)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValuesAtStartCount = amendedValuesAtStartCount;
			 _amendedValueAtEnd = amendedValueAtEnd;
			 _amendedValuesAtEndCount = amendedValuesAtEndCount;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
					if(i<_amendedValuesAtStartCount)
						return _amendedValueAtStart;
          else if(i<_length +_amendedValuesAtStartCount)
						return _x[i - _amendedValuesAtStartCount];
					else 
						return _amendedValueAtEnd;
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length + _amendedValuesAtStartCount + _amendedValuesAtEndCount;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
           return _length + _amendedValuesAtStartCount + _amendedValuesAtEndCount;
        }
      }

      public IEnumerator<double> GetEnumerator()
      {
				for (int i = 0; i < _amendedValuesAtStartCount; ++i)
					yield return _amendedValueAtStart;
				for (int i = 0; i < _length; ++i)
					yield return _x[i];
				for (int i = 0; i < _amendedValuesAtEndCount; ++i)
					yield return _amendedValueAtEnd;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
				for (int i = 0; i < _amendedValuesAtStartCount; ++i)
					yield return _amendedValueAtStart;
				for (int i = 0; i < _length; ++i)
					yield return _x[i];
				for (int i = 0; i < _amendedValuesAtEndCount; ++i)
					yield return _amendedValueAtEnd;
      }
    }

			/// <summary>
		/// Wraps a double[] array to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToROVectorAmendedShifted(this double[] array,double amendedValueAtStart, int amendedValuesAtStartCount, double amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return array is null ? null : new RODoubleArrayWrapperAmendedShifted(array, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}

		/// <summary>
		/// Wraps a double[] array till a given length to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToROVectorAmendedShifted(this double[] array, int usedlength,double amendedValueAtStart, int amendedValuesAtStartCount, double amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return new RODoubleArrayWrapperAmendedShifted(array, usedlength, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}


		/// <summary>
    /// Serves as thin wrapper struct for an array when additional data at the start and the end of the array are neccessary.
    /// </summary>
    public struct RODoubleArrayWrapperStructAmendedUnshifted : IReadOnlyList<double>
    {
      private int _length;
      private  double[] _x;
			private  double _amendedValueAtStart;
			private  double _amendedValueAtEnd;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
      public RODoubleArrayWrapperStructAmendedUnshifted(double[] x, double amendedValueAtStart, double amendedValueAtEnd)
      {
        _length = x.Length;
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValueAtEnd = amendedValueAtEnd;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
	      public RODoubleArrayWrapperStructAmendedUnshifted(double[] x, int usedlength, double amendedValueAtStart, double amendedValueAtEnd)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValueAtEnd = amendedValueAtEnd;
      }

      /// <summary>Gets the value at index i. For indices &lt;0, the value amendedValueAtStart is returned.
			/// For indices &gt;=Length, the value amendedValueAtEnd is returned.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
					if(i<0)
						return _amendedValueAtStart;
          else if(i<_length)
						return _x[i];
					else 
						return _amendedValueAtEnd;
        }
      }

      /// <summary>Attention! Returns the length of the wrapped part of the array.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>Attention! Returns the length of the wrapped part of the array.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<double> GetEnumerator()
      {
				yield return _amendedValueAtStart;

				for (int i = 0; i < _length; ++i)
					yield return _x[i];

				yield return _amendedValueAtEnd;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
				yield return _amendedValueAtStart;

				for (int i = 0; i < _length; ++i)
					yield return _x[i];

				yield return _amendedValueAtEnd;
      }
    }

		
		/// <summary>
		/// Wraps a double[] array to get a struct with an <see cref="IROVector{double}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above Length, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater than or equal to <paramref name="array"/>.Length.</param>.
		/// <returns>A wrapper struct with the <see cref="IROVector{double}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static RODoubleArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this double[] array,double amendedValueAtStart, double amendedValueAtEnd)
		{
			return new RODoubleArrayWrapperStructAmendedUnshifted(array, amendedValueAtStart, amendedValueAtEnd);
		}

		/// <summary>
		/// Wraps a double[] array till a given length to get a struct with an <see cref="IROVector{double}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above <paramref name="usedlength"/>, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to <paramref name="usedlength"/>.</param>.
		/// <returns>A wrapper struct with the <see cref="IROVector{double}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static RODoubleArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this double[] array, int usedlength,double amendedValueAtStart, double amendedValueAtEnd)
		{
			return new RODoubleArrayWrapperStructAmendedUnshifted(array, usedlength, amendedValueAtStart, amendedValueAtEnd );
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where a IVector is neccessary.
    /// </summary>
		private class RWDoubleArrayWrapper : RODoubleArrayWrapper, IVector<double>
    {
      public RWDoubleArrayWrapper(double[] x)
        : base(x)
      {
      }

      public RWDoubleArrayWrapper(double[] x, int usedlength)
        : base(x, usedlength)
      {
      }

      public new double this[int i]
      {
        get { return _x[i]; }
        set { _x[i] = value; }
      }
    }


			/// <summary>
		/// Wraps an array to get an <see cref="IVector{double}" />
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{double}" /> interface that wraps the provided array.</returns>
		public static IVector<double> ToVector(this double[] array)
		{
			return new RWDoubleArrayWrapper(array);
		}

		/// <summary>
		/// Wraps an array to get an <see cref="IVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Used length of the array to get the wrapped vector (i.e. the vector wraps around <paramref name="array"/>[0..usedLength-1]).</param>
		/// <returns>A wrapper objects with the <see cref="IVector{double}" /> interface that wraps the provided array.</returns>
		public static IVector<double> ToVector(double[] array, int usedlength)
		{
			return new RWDoubleArrayWrapper(array, usedlength);
		}

		
		private class RWDoubleArraySectionWrapper : RODoubleArraySectionWrapper, IVector<double>
    {
      public RWDoubleArraySectionWrapper(double[] x)
        : base(x)
      {
      }

      public RWDoubleArraySectionWrapper(double[] x, int start, int usedlength)
        : base(x, start, usedlength)
      {
      }

      public new double this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }
    }

		/// <summary>
		/// Wraps a section of an array to get a <see cref="IVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of first element of <paramref name="array"/> to use.</param>
		/// <param name="count">Number of elements of <paramref name="array"/> to use.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{double}" /> interface that wraps a section of the provided array.</returns>
		public static IVector<double> ToVector(this double[] array, int start, int count)
		{
			if (0 == start)
				return new RWDoubleArrayWrapper(array, count);
			else
				return new RWDoubleArraySectionWrapper(array, start, count);
		}



		 /// <summary>
    /// Serves as wrapper for an <see cref="IROVector{double}" /> to get only a section of the original wrapper.
    /// </summary>
    private class RODoubleVectorSectionWrapper : IReadOnlyList<double>
    {
      protected IReadOnlyList<double> _x;
      private int _start;
      private int _length;


		 /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The vector to wrap.</param>
      /// <param name="start">Start index of the section of the vector to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RODoubleVectorSectionWrapper(IReadOnlyList<double> x, int start, int len)
      {
        if (start >= x.Count)
          throw new ArgumentException("Start of the section is beyond length of the vector");
        if (start + len > x.Count)
          throw new ArgumentException("End of the section is beyond length of the vector");

        _x = x;
        _start = start;
        _length = len;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<double> GetEnumerator()
      {
        for(int i = 0; i < _length; ++i)
					yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for(int i = 0; i < _length; ++i)
					yield return this[i];
      }
    }


		/// <summary>
		/// Wraps a section of an original vector into a new vector.
		/// </summary>
		/// <param name="vector">Original vector.</param>
		/// <param name="start">Index of the start of the section to wrap.</param>
		/// <param name="usedLength">Length (=number of elements) of the section to wrap.</param>
		/// <returns>An <see cref="IROVector{double}" /> that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IReadOnlyList<double> ToROVector(this IReadOnlyList<double> vector, int start, int usedLength)
		{
			return new RODoubleVectorSectionWrapper(vector, start, usedLength);
		}

		  /// <summary>
    /// Serves as wrapper for an IVector to get only a section of the original wrapper.
    /// </summary>
    private class RWDoubleVectorSectionWrapper : IVector<double>
    {
      protected IVector<double> _x;
      private int _start;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Start index of the section to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RWDoubleVectorSectionWrapper(IVector<double> x, int start, int len)
      {
        if (start >= x.Count)
          throw new ArgumentException("Start of the section is beyond length of the vector");
        if (start + len > x.Count)
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
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a section of an original vector into a new vector.
		/// </summary>
		/// <param name="vector">Original vector.</param>
		/// <param name="start">Index of the start of the section to wrap.</param>
		/// <param name="len">Length (=number of elements) of the section to wrap.</param>
		/// <returns>A IVector that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IVector<double> ToVector(this IVector<double> vector, int start, int len)
		{
			return new RWDoubleVectorSectionWrapper(vector, start, len);
		}

    /// <summary>
	/// Returns a clone of the source vector.
	/// </summary>
	/// <param name="sourceVector">The source vector.</param>
    /// <returns>A freshly allocated clone of the sourceVector, with the same element values as the source vector.</returns>
	public static double[] Clone(double[] sourceVector)
	{
		if (sourceVector is null)
			throw new ArgumentNullException(nameof(sourceVector));

        var destVector = new double[sourceVector.Length];
        Array.Copy(sourceVector, destVector, sourceVector.Length);
        return destVector;
	}



// ******************************************** Definitions for float *******************************************

    /// <summary>
		/// Provides a read-only vector with equal and constant items.
		/// </summary>
		private class ROFloatConstantVector : IReadOnlyList<float>
		{
			private int _length;
			private float _value;

			public ROFloatConstantVector(float value, int length)
			{
				_length = length;
				_value = value;
			}

			public int Length
			{
				get { return _length; }
			}

      public int Count
      {
        get { return _length; }
      }

      public float this[int i]
			{
				get { return _value; }
			}

      public IEnumerator<float> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return _value;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return _value;
      }
    }

			/// <summary>
		/// Gets a vector with all elements equal to a provided value.
		/// </summary>
		/// <param name="value">Value of all elements.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with all elements equal to the provided <paramref name="value"/>.</returns>
		public static IReadOnlyList<float> GetConstantVector(float value, int length)
		{
			return new ROFloatConstantVector(value, length);
		}


		/// <summary>
    /// Provides a read-only vector with equally spaced elements y[i] = start + i * increment.
    /// </summary>
    private class ROFloatEquidistantElementVector : IReadOnlyList<float>
    {
      private int _length;
      private float _startValue;
      private float _incrementValue;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
      public ROFloatEquidistantElementVector(float start, float increment, int length)
      {
        _length = length;
        _startValue = start;
        _incrementValue = increment;
      }

			/// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get { return _length; }
      }

      	/// <summary>The number of elements of this vector.</summary>
			public int Count
      {
        get { return _length; }
      }

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
      public float this[int i]
      {
        get { return (float)(_startValue + i * _incrementValue); }
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      public IEnumerator<float> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }


			/// <summary>
		/// Creates a read-only vector with equidistant elements with values from start to start+(length-1)*step. The created vector
		/// consumes memory only for the three variables, independent of its length.
		/// </summary>
		/// <param name="start">First element of the vector.</param>
		/// <param name="step">Difference between two successive elements.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with equidistant elements with values from start to start+(length-1)*step.</returns>
		public static IReadOnlyList<float> CreateEquidistantSequenceByStartStepLength(float start, float step, int length)
		{
			return new ROFloatEquidistantElementVector(start, step, length);
		}

			
			/// <summary>
		/// Provides a read-only vector with equally spaced elements y[i] = start + (i-startOffset) * increment.
		/// </summary>
		private class ROFloatEquidistantElementVectorStartAtOffsetStepLength : IReadOnlyList<float>
		{
			private float _start;
			private int _startOffset;

			private float _increment;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the element at index <paramref name="startOffset"/> of the vector.</param>
			/// <param name="startOffset">The index of the element for which a value is provided in <paramref name="start"/>.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
			public ROFloatEquidistantElementVectorStartAtOffsetStepLength(float start, int startOffset, float increment, int length)
			{
				_start = start;
				_startOffset = startOffset;
				_increment = increment;
				_length = length;
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Length
			{
				get { return _length; }
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Count
			{
				get { return _length; }
			}

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
			public float this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");
					return (float)(_start + (i - _startOffset) * _increment);
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<float> GetEnumerator()
      {
        for (int i = 0; i<_length; ++i)
          yield return this[i];
      }

		/// <summary>
		/// Enumerates all elements of the vector.
		/// </summary>
		/// <returns>Enumerator that enumerates all elements of the vector.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < _length; ++i)
				yield return this[i];
		}
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
		public static IReadOnlyList<float> CreateEquidistantSequencyByStartAtOffsetStepLength(float start, int startOffset, float step, int length)
		{
			return new ROFloatEquidistantElementVectorStartAtOffsetStepLength(start, startOffset, step, length);
		}


	  	/// <summary>
		/// Provides a read-only vector with equally spaced elements so that y[0] = start and y[length-1] = end.
		/// </summary>
		private class ROFloatEquidistantElementVectorStartEndLength : IReadOnlyList<float>
		{
			private float _start;
			private float _end;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="end">Value of the last element of the vector.</param>
			/// <param name="length">Length of the vector.</param>
			public ROFloatEquidistantElementVectorStartEndLength(float start, float end, int length)
			{
				_start = start;
				_end = end;
				_length = length;
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Length
			{
				get { return _length; }
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Count
			{
				get { return _length; }
			}

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
			public float this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");

					double r = i / (double)(_length - 1);
					return (float)(_start * (1 - r) + _end * (r));
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<float> GetEnumerator()
      {
        for (int i = 0; i<_length; ++i)
          yield return this[i];
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _length; ++i)
					yield return this[i];
			}
		}

		/// <summary>
		/// Creates a read-only vector with equidistant element values from start to end. The created vector
		/// consumes memory only for the three variables, independent of its length.
		/// </summary>
		/// <param name="start">First element of the vector.</param>
		/// <param name="end">Last element of the vector.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with equidistant element values from start to end.</returns>
		public static IReadOnlyList<float> CreateEquidistantSequenceByStartEndLength(float start, float end, int length)
		{
			return new ROFloatEquidistantElementVectorStartEndLength(start, end, length);
		}


		/// <summary>
    /// Serves as wrapper for an RO Vector which returns the inverse of the elements of the original array.
    /// </summary>
    private class ROFloatInverseElementWrapper : IReadOnlyList<float>
    {
      private int _length;
      protected IReadOnlyList<float> _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROFloatInverseElementWrapper(IReadOnlyList<float> x)
      {
        _x = x;
        _length = x.Count;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROFloatInverseElementWrapper(IReadOnlyList<float> x, int usedlength)
      {
        if (usedlength > x.Count)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public float this[int i]
      {
        get
        {
          return 1/(float)_x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<float> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a float[] array to get an  <see cref="IROVector{float}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{float}" /> returning elements that are inverse to those of the original vector.</returns>
		public static IReadOnlyList<float> ToInverseROVector(this IReadOnlyList<float> array)
		{
			return array is null ? null : new ROFloatInverseElementWrapper(array);
		}

		/// <summary>
		/// Wraps a float[] array till a given length to get an <see cref="IROVector{float}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{float}" /> interface that wraps the provided array returning elements that are inverse to those of the original vector..</returns>
		public static IReadOnlyList<float> ToInverseROVector(this IReadOnlyList<float> array, int usedlength)
		{
			return new ROFloatInverseElementWrapper(array, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{float}" /> is neccessary.
    /// </summary>
    private class ROFloatArrayWrapper : IReadOnlyList<float>
    {
      private int _length;
      protected float[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROFloatArrayWrapper(float[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROFloatArrayWrapper(float[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public float this[int i]
      {
        get
        {
          return _x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<float> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a float[] array to get an <see cref="IROVector{float}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{float}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<float> ToROVector(this float[] array)
		{
			return array is null ? null : new ROFloatArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a float[] array till a given length to get an <see cref="IROVector{float}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{float}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<float> ToROVector(this float[] array, int usedlength)
		{
			return new ROFloatArrayWrapper(array, usedlength);
		}

			/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{float}" /> is neccessary.
    /// </summary>
    private class RODouble_FloatArrayWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected float[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODouble_FloatArrayWrapper(float[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODouble_FloatArrayWrapper(float[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
          return _x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return (double)this[i];
      }
    }

		/// <summary>
		/// Wraps a float[] array to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{float}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this float[] array)
		{
			return array is null ? null : new RODouble_FloatArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a float[] array till a given length to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{float}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this float[] array, int usedlength)
		{
			return new RODouble_FloatArrayWrapper(array, usedlength);
		}


		  /// <summary>
    /// Serves as wrapper for a section of an array to plug-in where an <see cref="IROVector{float}" /> is neccessary.
    /// </summary>
    private class ROFloatArraySectionWrapper : IReadOnlyList<float>
    {
      protected float[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public ROFloatArraySectionWrapper(float[] x)
      {
        _length = x.Length;
        _start = 0;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array, and start and length of the section, for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap.</param>
      /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROFloatArraySectionWrapper(float[] x, int start, int usedlength)
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
      public float this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      /// <summary>
      /// Returns an enumerator that iterates through the elements of the vector.
      /// </summary>
      /// <returns>
      /// An enumerator that can be used to iterate through the elements of the vector.
      /// </returns>
      public IEnumerator<float> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps an array to an <see cref="IROVector{float}" />. Start and length of the used section of the array are specified in the parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of the element in <paramref name="array"/> used as the first element of the vector.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper object with the <see cref="IROVector{float}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<float> ToROVector(this float[] array, int start, int usedlength)
		{
			if (0 == start)
				return new ROFloatArrayWrapper(array, usedlength);
			else
				return new ROFloatArraySectionWrapper(array, start, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{float}" /> is neccessary.
    /// </summary>
    private class ROFloatArrayWrapperAmendedShifted : IReadOnlyList<float>
    {
      private int _length;
      protected float[] _x;
			protected float _amendedValueAtStart;
			protected int _amendedValuesAtStartCount;
			protected float _amendedValueAtEnd;
			protected int _amendedValuesAtEndCount;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public ROFloatArrayWrapperAmendedShifted(float[] x, float amendedValueAtStart, int amendedValuesAtStartCount, float amendedValueAtEnd, int amendedValuesAtEndCount)
      {
        _length = x.Length;
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValuesAtStartCount = amendedValuesAtStartCount;
			 _amendedValueAtEnd = amendedValueAtEnd;
			 _amendedValuesAtEndCount = amendedValuesAtEndCount;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public ROFloatArrayWrapperAmendedShifted(float[] x, int usedlength, float amendedValueAtStart, int amendedValuesAtStartCount, float amendedValueAtEnd, int amendedValuesAtEndCount)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValuesAtStartCount = amendedValuesAtStartCount;
			 _amendedValueAtEnd = amendedValueAtEnd;
			 _amendedValuesAtEndCount = amendedValuesAtEndCount;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public float this[int i]
      {
        get
        {
					if(i<_amendedValuesAtStartCount)
						return _amendedValueAtStart;
          else if(i<_length +_amendedValuesAtStartCount)
						return _x[i - _amendedValuesAtStartCount];
					else 
						return _amendedValueAtEnd;
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length + _amendedValuesAtStartCount + _amendedValuesAtEndCount;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
           return _length + _amendedValuesAtStartCount + _amendedValuesAtEndCount;
        }
      }

      public IEnumerator<float> GetEnumerator()
      {
				for (int i = 0; i < _amendedValuesAtStartCount; ++i)
					yield return _amendedValueAtStart;
				for (int i = 0; i < _length; ++i)
					yield return _x[i];
				for (int i = 0; i < _amendedValuesAtEndCount; ++i)
					yield return _amendedValueAtEnd;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
				for (int i = 0; i < _amendedValuesAtStartCount; ++i)
					yield return _amendedValueAtStart;
				for (int i = 0; i < _length; ++i)
					yield return _x[i];
				for (int i = 0; i < _amendedValuesAtEndCount; ++i)
					yield return _amendedValueAtEnd;
      }
    }

			/// <summary>
		/// Wraps a float[] array to get an <see cref="IROVector{float}" />.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{float}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<float> ToROVectorAmendedShifted(this float[] array,float amendedValueAtStart, int amendedValuesAtStartCount, float amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return array is null ? null : new ROFloatArrayWrapperAmendedShifted(array, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}

		/// <summary>
		/// Wraps a float[] array till a given length to get an <see cref="IROVector{float}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{float}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<float> ToROVectorAmendedShifted(this float[] array, int usedlength,float amendedValueAtStart, int amendedValuesAtStartCount, float amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return new ROFloatArrayWrapperAmendedShifted(array, usedlength, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}


		/// <summary>
    /// Serves as thin wrapper struct for an array when additional data at the start and the end of the array are neccessary.
    /// </summary>
    public struct ROFloatArrayWrapperStructAmendedUnshifted : IReadOnlyList<float>
    {
      private int _length;
      private  float[] _x;
			private  float _amendedValueAtStart;
			private  float _amendedValueAtEnd;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
      public ROFloatArrayWrapperStructAmendedUnshifted(float[] x, float amendedValueAtStart, float amendedValueAtEnd)
      {
        _length = x.Length;
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValueAtEnd = amendedValueAtEnd;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
	      public ROFloatArrayWrapperStructAmendedUnshifted(float[] x, int usedlength, float amendedValueAtStart, float amendedValueAtEnd)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValueAtEnd = amendedValueAtEnd;
      }

      /// <summary>Gets the value at index i. For indices &lt;0, the value amendedValueAtStart is returned.
			/// For indices &gt;=Length, the value amendedValueAtEnd is returned.</summary>
      /// <value>The element at index i.</value>
      public float this[int i]
      {
        get
        {
					if(i<0)
						return _amendedValueAtStart;
          else if(i<_length)
						return _x[i];
					else 
						return _amendedValueAtEnd;
        }
      }

      /// <summary>Attention! Returns the length of the wrapped part of the array.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>Attention! Returns the length of the wrapped part of the array.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<float> GetEnumerator()
      {
				yield return _amendedValueAtStart;

				for (int i = 0; i < _length; ++i)
					yield return _x[i];

				yield return _amendedValueAtEnd;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
				yield return _amendedValueAtStart;

				for (int i = 0; i < _length; ++i)
					yield return _x[i];

				yield return _amendedValueAtEnd;
      }
    }

		
		/// <summary>
		/// Wraps a float[] array to get a struct with an <see cref="IROVector{float}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above Length, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater than or equal to <paramref name="array"/>.Length.</param>.
		/// <returns>A wrapper struct with the <see cref="IROVector{float}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROFloatArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this float[] array,float amendedValueAtStart, float amendedValueAtEnd)
		{
			return new ROFloatArrayWrapperStructAmendedUnshifted(array, amendedValueAtStart, amendedValueAtEnd);
		}

		/// <summary>
		/// Wraps a float[] array till a given length to get a struct with an <see cref="IROVector{float}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above <paramref name="usedlength"/>, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to <paramref name="usedlength"/>.</param>.
		/// <returns>A wrapper struct with the <see cref="IROVector{float}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROFloatArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this float[] array, int usedlength,float amendedValueAtStart, float amendedValueAtEnd)
		{
			return new ROFloatArrayWrapperStructAmendedUnshifted(array, usedlength, amendedValueAtStart, amendedValueAtEnd );
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where a IVector is neccessary.
    /// </summary>
		private class RWFloatArrayWrapper : ROFloatArrayWrapper, IVector<float>
    {
      public RWFloatArrayWrapper(float[] x)
        : base(x)
      {
      }

      public RWFloatArrayWrapper(float[] x, int usedlength)
        : base(x, usedlength)
      {
      }

      public new float this[int i]
      {
        get { return _x[i]; }
        set { _x[i] = value; }
      }
    }


			/// <summary>
		/// Wraps an array to get an <see cref="IVector{float}" />
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{float}" /> interface that wraps the provided array.</returns>
		public static IVector<float> ToVector(this float[] array)
		{
			return new RWFloatArrayWrapper(array);
		}

		/// <summary>
		/// Wraps an array to get an <see cref="IVector{float}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Used length of the array to get the wrapped vector (i.e. the vector wraps around <paramref name="array"/>[0..usedLength-1]).</param>
		/// <returns>A wrapper objects with the <see cref="IVector{float}" /> interface that wraps the provided array.</returns>
		public static IVector<float> ToVector(float[] array, int usedlength)
		{
			return new RWFloatArrayWrapper(array, usedlength);
		}

		
		private class RWFloatArraySectionWrapper : ROFloatArraySectionWrapper, IVector<float>
    {
      public RWFloatArraySectionWrapper(float[] x)
        : base(x)
      {
      }

      public RWFloatArraySectionWrapper(float[] x, int start, int usedlength)
        : base(x, start, usedlength)
      {
      }

      public new float this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }
    }

		/// <summary>
		/// Wraps a section of an array to get a <see cref="IVector{float}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of first element of <paramref name="array"/> to use.</param>
		/// <param name="count">Number of elements of <paramref name="array"/> to use.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{float}" /> interface that wraps a section of the provided array.</returns>
		public static IVector<float> ToVector(this float[] array, int start, int count)
		{
			if (0 == start)
				return new RWFloatArrayWrapper(array, count);
			else
				return new RWFloatArraySectionWrapper(array, start, count);
		}



		 /// <summary>
    /// Serves as wrapper for an <see cref="IROVector{float}" /> to get only a section of the original wrapper.
    /// </summary>
    private class ROFloatVectorSectionWrapper : IReadOnlyList<float>
    {
      protected IReadOnlyList<float> _x;
      private int _start;
      private int _length;


		 /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The vector to wrap.</param>
      /// <param name="start">Start index of the section of the vector to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public ROFloatVectorSectionWrapper(IReadOnlyList<float> x, int start, int len)
      {
        if (start >= x.Count)
          throw new ArgumentException("Start of the section is beyond length of the vector");
        if (start + len > x.Count)
          throw new ArgumentException("End of the section is beyond length of the vector");

        _x = x;
        _start = start;
        _length = len;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public float this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<float> GetEnumerator()
      {
        for(int i = 0; i < _length; ++i)
					yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for(int i = 0; i < _length; ++i)
					yield return this[i];
      }
    }


		/// <summary>
		/// Wraps a section of an original vector into a new vector.
		/// </summary>
		/// <param name="vector">Original vector.</param>
		/// <param name="start">Index of the start of the section to wrap.</param>
		/// <param name="usedLength">Length (=number of elements) of the section to wrap.</param>
		/// <returns>An <see cref="IROVector{float}" /> that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IReadOnlyList<float> ToROVector(this IReadOnlyList<float> vector, int start, int usedLength)
		{
			return new ROFloatVectorSectionWrapper(vector, start, usedLength);
		}

		  /// <summary>
    /// Serves as wrapper for an IVector to get only a section of the original wrapper.
    /// </summary>
    private class RWFloatVectorSectionWrapper : IVector<float>
    {
      protected IVector<float> _x;
      private int _start;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Start index of the section to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RWFloatVectorSectionWrapper(IVector<float> x, int start, int len)
      {
        if (start >= x.Count)
          throw new ArgumentException("Start of the section is beyond length of the vector");
        if (start + len > x.Count)
          throw new ArgumentException("End of the section is beyond length of the vector");

        _x = x;
        _start = start;
        _length = len;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public float this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<float> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a section of an original vector into a new vector.
		/// </summary>
		/// <param name="vector">Original vector.</param>
		/// <param name="start">Index of the start of the section to wrap.</param>
		/// <param name="len">Length (=number of elements) of the section to wrap.</param>
		/// <returns>A IVector that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IVector<float> ToVector(this IVector<float> vector, int start, int len)
		{
			return new RWFloatVectorSectionWrapper(vector, start, len);
		}

    /// <summary>
	/// Returns a clone of the source vector.
	/// </summary>
	/// <param name="sourceVector">The source vector.</param>
    /// <returns>A freshly allocated clone of the sourceVector, with the same element values as the source vector.</returns>
	public static float[] Clone(float[] sourceVector)
	{
		if (sourceVector is null)
			throw new ArgumentNullException(nameof(sourceVector));

        var destVector = new float[sourceVector.Length];
        Array.Copy(sourceVector, destVector, sourceVector.Length);
        return destVector;
	}



// ******************************************** Definitions for Int32 *******************************************

    /// <summary>
		/// Provides a read-only vector with equal and constant items.
		/// </summary>
		private class ROIntConstantVector : IReadOnlyList<Int32>
		{
			private int _length;
			private Int32 _value;

			public ROIntConstantVector(Int32 value, int length)
			{
				_length = length;
				_value = value;
			}

			public int Length
			{
				get { return _length; }
			}

      public int Count
      {
        get { return _length; }
      }

      public Int32 this[int i]
			{
				get { return _value; }
			}

      public IEnumerator<Int32> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return _value;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return _value;
      }
    }

			/// <summary>
		/// Gets a vector with all elements equal to a provided value.
		/// </summary>
		/// <param name="value">Value of all elements.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with all elements equal to the provided <paramref name="value"/>.</returns>
		public static IReadOnlyList<Int32> GetConstantVector(Int32 value, int length)
		{
			return new ROIntConstantVector(value, length);
		}


		/// <summary>
    /// Provides a read-only vector with equally spaced elements y[i] = start + i * increment.
    /// </summary>
    private class ROIntEquidistantElementVector : IReadOnlyList<Int32>
    {
      private int _length;
      private Int32 _startValue;
      private Int32 _incrementValue;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
      public ROIntEquidistantElementVector(Int32 start, Int32 increment, int length)
      {
        _length = length;
        _startValue = start;
        _incrementValue = increment;
      }

			/// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get { return _length; }
      }

      	/// <summary>The number of elements of this vector.</summary>
			public int Count
      {
        get { return _length; }
      }

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
      public Int32 this[int i]
      {
        get { return (Int32)(_startValue + i * _incrementValue); }
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      public IEnumerator<Int32> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }


			/// <summary>
		/// Creates a read-only vector with equidistant elements with values from start to start+(length-1)*step. The created vector
		/// consumes memory only for the three variables, independent of its length.
		/// </summary>
		/// <param name="start">First element of the vector.</param>
		/// <param name="step">Difference between two successive elements.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with equidistant elements with values from start to start+(length-1)*step.</returns>
		public static IReadOnlyList<Int32> CreateEquidistantSequenceByStartStepLength(Int32 start, Int32 step, int length)
		{
			return new ROIntEquidistantElementVector(start, step, length);
		}

			
			/// <summary>
		/// Provides a read-only vector with equally spaced elements y[i] = start + (i-startOffset) * increment.
		/// </summary>
		private class ROIntEquidistantElementVectorStartAtOffsetStepLength : IReadOnlyList<Int32>
		{
			private Int32 _start;
			private int _startOffset;

			private Int32 _increment;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the element at index <paramref name="startOffset"/> of the vector.</param>
			/// <param name="startOffset">The index of the element for which a value is provided in <paramref name="start"/>.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
			public ROIntEquidistantElementVectorStartAtOffsetStepLength(Int32 start, int startOffset, Int32 increment, int length)
			{
				_start = start;
				_startOffset = startOffset;
				_increment = increment;
				_length = length;
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Length
			{
				get { return _length; }
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Count
			{
				get { return _length; }
			}

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
			public Int32 this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");
					return (Int32)(_start + (i - _startOffset) * _increment);
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<Int32> GetEnumerator()
      {
        for (int i = 0; i<_length; ++i)
          yield return this[i];
      }

		/// <summary>
		/// Enumerates all elements of the vector.
		/// </summary>
		/// <returns>Enumerator that enumerates all elements of the vector.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < _length; ++i)
				yield return this[i];
		}
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
		public static IReadOnlyList<Int32> CreateEquidistantSequencyByStartAtOffsetStepLength(Int32 start, int startOffset, Int32 step, int length)
		{
			return new ROIntEquidistantElementVectorStartAtOffsetStepLength(start, startOffset, step, length);
		}


	  	/// <summary>
		/// Provides a read-only vector with equally spaced elements so that y[0] = start and y[length-1] = end.
		/// </summary>
		private class ROIntEquidistantElementVectorStartEndLength : IReadOnlyList<Int32>
		{
			private Int32 _start;
			private Int32 _end;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="end">Value of the last element of the vector.</param>
			/// <param name="length">Length of the vector.</param>
			public ROIntEquidistantElementVectorStartEndLength(Int32 start, Int32 end, int length)
			{
				_start = start;
				_end = end;
				_length = length;
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Length
			{
				get { return _length; }
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Count
			{
				get { return _length; }
			}

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
			public Int32 this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");

					double r = i / (double)(_length - 1);
					return (Int32)(_start * (1 - r) + _end * (r));
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<Int32> GetEnumerator()
      {
        for (int i = 0; i<_length; ++i)
          yield return this[i];
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _length; ++i)
					yield return this[i];
			}
		}

		/// <summary>
		/// Creates a read-only vector with equidistant element values from start to end. The created vector
		/// consumes memory only for the three variables, independent of its length.
		/// </summary>
		/// <param name="start">First element of the vector.</param>
		/// <param name="end">Last element of the vector.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with equidistant element values from start to end.</returns>
		public static IReadOnlyList<Int32> CreateEquidistantSequenceByStartEndLength(Int32 start, Int32 end, int length)
		{
			return new ROIntEquidistantElementVectorStartEndLength(start, end, length);
		}


		/// <summary>
    /// Serves as wrapper for an RO Vector which returns the inverse of the elements of the original array.
    /// </summary>
    private class ROIntInverseElementWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected IReadOnlyList<Int32> _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROIntInverseElementWrapper(IReadOnlyList<Int32> x)
      {
        _x = x;
        _length = x.Count;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROIntInverseElementWrapper(IReadOnlyList<Int32> x, int usedlength)
      {
        if (usedlength > x.Count)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
          return 1/(double)_x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a Int32[] array to get an  <see cref="IROVector{Int32}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int32}" /> returning elements that are inverse to those of the original vector.</returns>
		public static IReadOnlyList<double> ToInverseROVector(this IReadOnlyList<Int32> array)
		{
			return array is null ? null : new ROIntInverseElementWrapper(array);
		}

		/// <summary>
		/// Wraps a Int32[] array till a given length to get an <see cref="IROVector{Int32}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int32}" /> interface that wraps the provided array returning elements that are inverse to those of the original vector..</returns>
		public static IReadOnlyList<double> ToInverseROVector(this IReadOnlyList<Int32> array, int usedlength)
		{
			return new ROIntInverseElementWrapper(array, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{Int32}" /> is neccessary.
    /// </summary>
    private class ROIntArrayWrapper : IReadOnlyList<Int32>
    {
      private int _length;
      protected Int32[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROIntArrayWrapper(Int32[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROIntArrayWrapper(Int32[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public Int32 this[int i]
      {
        get
        {
          return _x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<Int32> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a Int32[] array to get an <see cref="IROVector{Int32}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<Int32> ToROVector(this Int32[] array)
		{
			return array is null ? null : new ROIntArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a Int32[] array till a given length to get an <see cref="IROVector{Int32}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<Int32> ToROVector(this Int32[] array, int usedlength)
		{
			return new ROIntArrayWrapper(array, usedlength);
		}

			/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{Int32}" /> is neccessary.
    /// </summary>
    private class RODouble_IntArrayWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected Int32[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODouble_IntArrayWrapper(Int32[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODouble_IntArrayWrapper(Int32[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
          return _x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return (double)this[i];
      }
    }

		/// <summary>
		/// Wraps a Int32[] array to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this Int32[] array)
		{
			return array is null ? null : new RODouble_IntArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a Int32[] array till a given length to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this Int32[] array, int usedlength)
		{
			return new RODouble_IntArrayWrapper(array, usedlength);
		}


		  /// <summary>
    /// Serves as wrapper for a section of an array to plug-in where an <see cref="IROVector{Int32}" /> is neccessary.
    /// </summary>
    private class ROIntArraySectionWrapper : IReadOnlyList<Int32>
    {
      protected Int32[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public ROIntArraySectionWrapper(Int32[] x)
      {
        _length = x.Length;
        _start = 0;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array, and start and length of the section, for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap.</param>
      /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROIntArraySectionWrapper(Int32[] x, int start, int usedlength)
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
      public Int32 this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      /// <summary>
      /// Returns an enumerator that iterates through the elements of the vector.
      /// </summary>
      /// <returns>
      /// An enumerator that can be used to iterate through the elements of the vector.
      /// </returns>
      public IEnumerator<Int32> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps an array to an <see cref="IROVector{Int32}" />. Start and length of the used section of the array are specified in the parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of the element in <paramref name="array"/> used as the first element of the vector.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper object with the <see cref="IROVector{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<Int32> ToROVector(this Int32[] array, int start, int usedlength)
		{
			if (0 == start)
				return new ROIntArrayWrapper(array, usedlength);
			else
				return new ROIntArraySectionWrapper(array, start, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{Int32}" /> is neccessary.
    /// </summary>
    private class ROIntArrayWrapperAmendedShifted : IReadOnlyList<Int32>
    {
      private int _length;
      protected Int32[] _x;
			protected Int32 _amendedValueAtStart;
			protected int _amendedValuesAtStartCount;
			protected Int32 _amendedValueAtEnd;
			protected int _amendedValuesAtEndCount;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public ROIntArrayWrapperAmendedShifted(Int32[] x, Int32 amendedValueAtStart, int amendedValuesAtStartCount, Int32 amendedValueAtEnd, int amendedValuesAtEndCount)
      {
        _length = x.Length;
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValuesAtStartCount = amendedValuesAtStartCount;
			 _amendedValueAtEnd = amendedValueAtEnd;
			 _amendedValuesAtEndCount = amendedValuesAtEndCount;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public ROIntArrayWrapperAmendedShifted(Int32[] x, int usedlength, Int32 amendedValueAtStart, int amendedValuesAtStartCount, Int32 amendedValueAtEnd, int amendedValuesAtEndCount)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValuesAtStartCount = amendedValuesAtStartCount;
			 _amendedValueAtEnd = amendedValueAtEnd;
			 _amendedValuesAtEndCount = amendedValuesAtEndCount;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public Int32 this[int i]
      {
        get
        {
					if(i<_amendedValuesAtStartCount)
						return _amendedValueAtStart;
          else if(i<_length +_amendedValuesAtStartCount)
						return _x[i - _amendedValuesAtStartCount];
					else 
						return _amendedValueAtEnd;
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length + _amendedValuesAtStartCount + _amendedValuesAtEndCount;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
           return _length + _amendedValuesAtStartCount + _amendedValuesAtEndCount;
        }
      }

      public IEnumerator<Int32> GetEnumerator()
      {
				for (int i = 0; i < _amendedValuesAtStartCount; ++i)
					yield return _amendedValueAtStart;
				for (int i = 0; i < _length; ++i)
					yield return _x[i];
				for (int i = 0; i < _amendedValuesAtEndCount; ++i)
					yield return _amendedValueAtEnd;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
				for (int i = 0; i < _amendedValuesAtStartCount; ++i)
					yield return _amendedValueAtStart;
				for (int i = 0; i < _length; ++i)
					yield return _x[i];
				for (int i = 0; i < _amendedValuesAtEndCount; ++i)
					yield return _amendedValueAtEnd;
      }
    }

			/// <summary>
		/// Wraps a Int32[] array to get an <see cref="IROVector{Int32}" />.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<Int32> ToROVectorAmendedShifted(this Int32[] array,Int32 amendedValueAtStart, int amendedValuesAtStartCount, Int32 amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return array is null ? null : new ROIntArrayWrapperAmendedShifted(array, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}

		/// <summary>
		/// Wraps a Int32[] array till a given length to get an <see cref="IROVector{Int32}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<Int32> ToROVectorAmendedShifted(this Int32[] array, int usedlength,Int32 amendedValueAtStart, int amendedValuesAtStartCount, Int32 amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return new ROIntArrayWrapperAmendedShifted(array, usedlength, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}


		/// <summary>
    /// Serves as thin wrapper struct for an array when additional data at the start and the end of the array are neccessary.
    /// </summary>
    public struct ROIntArrayWrapperStructAmendedUnshifted : IReadOnlyList<Int32>
    {
      private int _length;
      private  Int32[] _x;
			private  Int32 _amendedValueAtStart;
			private  Int32 _amendedValueAtEnd;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
      public ROIntArrayWrapperStructAmendedUnshifted(Int32[] x, Int32 amendedValueAtStart, Int32 amendedValueAtEnd)
      {
        _length = x.Length;
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValueAtEnd = amendedValueAtEnd;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
	      public ROIntArrayWrapperStructAmendedUnshifted(Int32[] x, int usedlength, Int32 amendedValueAtStart, Int32 amendedValueAtEnd)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValueAtEnd = amendedValueAtEnd;
      }

      /// <summary>Gets the value at index i. For indices &lt;0, the value amendedValueAtStart is returned.
			/// For indices &gt;=Length, the value amendedValueAtEnd is returned.</summary>
      /// <value>The element at index i.</value>
      public Int32 this[int i]
      {
        get
        {
					if(i<0)
						return _amendedValueAtStart;
          else if(i<_length)
						return _x[i];
					else 
						return _amendedValueAtEnd;
        }
      }

      /// <summary>Attention! Returns the length of the wrapped part of the array.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>Attention! Returns the length of the wrapped part of the array.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<Int32> GetEnumerator()
      {
				yield return _amendedValueAtStart;

				for (int i = 0; i < _length; ++i)
					yield return _x[i];

				yield return _amendedValueAtEnd;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
				yield return _amendedValueAtStart;

				for (int i = 0; i < _length; ++i)
					yield return _x[i];

				yield return _amendedValueAtEnd;
      }
    }

		
		/// <summary>
		/// Wraps a Int32[] array to get a struct with an <see cref="IROVector{Int32}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above Length, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater than or equal to <paramref name="array"/>.Length.</param>.
		/// <returns>A wrapper struct with the <see cref="IROVector{Int32}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROIntArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this Int32[] array,Int32 amendedValueAtStart, Int32 amendedValueAtEnd)
		{
			return new ROIntArrayWrapperStructAmendedUnshifted(array, amendedValueAtStart, amendedValueAtEnd);
		}

		/// <summary>
		/// Wraps a Int32[] array till a given length to get a struct with an <see cref="IROVector{Int32}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above <paramref name="usedlength"/>, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to <paramref name="usedlength"/>.</param>.
		/// <returns>A wrapper struct with the <see cref="IROVector{Int32}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROIntArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this Int32[] array, int usedlength,Int32 amendedValueAtStart, Int32 amendedValueAtEnd)
		{
			return new ROIntArrayWrapperStructAmendedUnshifted(array, usedlength, amendedValueAtStart, amendedValueAtEnd );
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where a IVector is neccessary.
    /// </summary>
		private class RWIntArrayWrapper : ROIntArrayWrapper, IVector<Int32>
    {
      public RWIntArrayWrapper(Int32[] x)
        : base(x)
      {
      }

      public RWIntArrayWrapper(Int32[] x, int usedlength)
        : base(x, usedlength)
      {
      }

      public new Int32 this[int i]
      {
        get { return _x[i]; }
        set { _x[i] = value; }
      }
    }


			/// <summary>
		/// Wraps an array to get an <see cref="IVector{Int32}" />
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Int32}" /> interface that wraps the provided array.</returns>
		public static IVector<Int32> ToVector(this Int32[] array)
		{
			return new RWIntArrayWrapper(array);
		}

		/// <summary>
		/// Wraps an array to get an <see cref="IVector{Int32}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Used length of the array to get the wrapped vector (i.e. the vector wraps around <paramref name="array"/>[0..usedLength-1]).</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Int32}" /> interface that wraps the provided array.</returns>
		public static IVector<Int32> ToVector(Int32[] array, int usedlength)
		{
			return new RWIntArrayWrapper(array, usedlength);
		}

		
		private class RWIntArraySectionWrapper : ROIntArraySectionWrapper, IVector<Int32>
    {
      public RWIntArraySectionWrapper(Int32[] x)
        : base(x)
      {
      }

      public RWIntArraySectionWrapper(Int32[] x, int start, int usedlength)
        : base(x, start, usedlength)
      {
      }

      public new Int32 this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }
    }

		/// <summary>
		/// Wraps a section of an array to get a <see cref="IVector{Int32}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of first element of <paramref name="array"/> to use.</param>
		/// <param name="count">Number of elements of <paramref name="array"/> to use.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Int32}" /> interface that wraps a section of the provided array.</returns>
		public static IVector<Int32> ToVector(this Int32[] array, int start, int count)
		{
			if (0 == start)
				return new RWIntArrayWrapper(array, count);
			else
				return new RWIntArraySectionWrapper(array, start, count);
		}



		 /// <summary>
    /// Serves as wrapper for an <see cref="IROVector{Int32}" /> to get only a section of the original wrapper.
    /// </summary>
    private class ROIntVectorSectionWrapper : IReadOnlyList<Int32>
    {
      protected IReadOnlyList<Int32> _x;
      private int _start;
      private int _length;


		 /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The vector to wrap.</param>
      /// <param name="start">Start index of the section of the vector to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public ROIntVectorSectionWrapper(IReadOnlyList<Int32> x, int start, int len)
      {
        if (start >= x.Count)
          throw new ArgumentException("Start of the section is beyond length of the vector");
        if (start + len > x.Count)
          throw new ArgumentException("End of the section is beyond length of the vector");

        _x = x;
        _start = start;
        _length = len;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public Int32 this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<Int32> GetEnumerator()
      {
        for(int i = 0; i < _length; ++i)
					yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for(int i = 0; i < _length; ++i)
					yield return this[i];
      }
    }


		/// <summary>
		/// Wraps a section of an original vector into a new vector.
		/// </summary>
		/// <param name="vector">Original vector.</param>
		/// <param name="start">Index of the start of the section to wrap.</param>
		/// <param name="usedLength">Length (=number of elements) of the section to wrap.</param>
		/// <returns>An <see cref="IROVector{Int32}" /> that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IReadOnlyList<Int32> ToROVector(this IReadOnlyList<Int32> vector, int start, int usedLength)
		{
			return new ROIntVectorSectionWrapper(vector, start, usedLength);
		}

		  /// <summary>
    /// Serves as wrapper for an IVector to get only a section of the original wrapper.
    /// </summary>
    private class RWIntVectorSectionWrapper : IVector<Int32>
    {
      protected IVector<Int32> _x;
      private int _start;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Start index of the section to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RWIntVectorSectionWrapper(IVector<Int32> x, int start, int len)
      {
        if (start >= x.Count)
          throw new ArgumentException("Start of the section is beyond length of the vector");
        if (start + len > x.Count)
          throw new ArgumentException("End of the section is beyond length of the vector");

        _x = x;
        _start = start;
        _length = len;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public Int32 this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<Int32> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a section of an original vector into a new vector.
		/// </summary>
		/// <param name="vector">Original vector.</param>
		/// <param name="start">Index of the start of the section to wrap.</param>
		/// <param name="len">Length (=number of elements) of the section to wrap.</param>
		/// <returns>A IVector that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IVector<Int32> ToVector(this IVector<Int32> vector, int start, int len)
		{
			return new RWIntVectorSectionWrapper(vector, start, len);
		}

    /// <summary>
	/// Returns a clone of the source vector.
	/// </summary>
	/// <param name="sourceVector">The source vector.</param>
    /// <returns>A freshly allocated clone of the sourceVector, with the same element values as the source vector.</returns>
	public static Int32[] Clone(Int32[] sourceVector)
	{
		if (sourceVector is null)
			throw new ArgumentNullException(nameof(sourceVector));

        var destVector = new Int32[sourceVector.Length];
        Array.Copy(sourceVector, destVector, sourceVector.Length);
        return destVector;
	}



// ******************************************** Definitions for Int16 *******************************************

    /// <summary>
		/// Provides a read-only vector with equal and constant items.
		/// </summary>
		private class ROShortConstantVector : IReadOnlyList<Int16>
		{
			private int _length;
			private Int16 _value;

			public ROShortConstantVector(Int16 value, int length)
			{
				_length = length;
				_value = value;
			}

			public int Length
			{
				get { return _length; }
			}

      public int Count
      {
        get { return _length; }
      }

      public Int16 this[int i]
			{
				get { return _value; }
			}

      public IEnumerator<Int16> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return _value;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return _value;
      }
    }

			/// <summary>
		/// Gets a vector with all elements equal to a provided value.
		/// </summary>
		/// <param name="value">Value of all elements.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with all elements equal to the provided <paramref name="value"/>.</returns>
		public static IReadOnlyList<Int16> GetConstantVector(Int16 value, int length)
		{
			return new ROShortConstantVector(value, length);
		}


		/// <summary>
    /// Provides a read-only vector with equally spaced elements y[i] = start + i * increment.
    /// </summary>
    private class ROShortEquidistantElementVector : IReadOnlyList<Int16>
    {
      private int _length;
      private Int16 _startValue;
      private Int16 _incrementValue;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
      public ROShortEquidistantElementVector(Int16 start, Int16 increment, int length)
      {
        _length = length;
        _startValue = start;
        _incrementValue = increment;
      }

			/// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get { return _length; }
      }

      	/// <summary>The number of elements of this vector.</summary>
			public int Count
      {
        get { return _length; }
      }

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
      public Int16 this[int i]
      {
        get { return (Int16)(_startValue + i * _incrementValue); }
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      public IEnumerator<Int16> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }


			/// <summary>
		/// Creates a read-only vector with equidistant elements with values from start to start+(length-1)*step. The created vector
		/// consumes memory only for the three variables, independent of its length.
		/// </summary>
		/// <param name="start">First element of the vector.</param>
		/// <param name="step">Difference between two successive elements.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with equidistant elements with values from start to start+(length-1)*step.</returns>
		public static IReadOnlyList<Int16> CreateEquidistantSequenceByStartStepLength(Int16 start, Int16 step, int length)
		{
			return new ROShortEquidistantElementVector(start, step, length);
		}

			
			/// <summary>
		/// Provides a read-only vector with equally spaced elements y[i] = start + (i-startOffset) * increment.
		/// </summary>
		private class ROShortEquidistantElementVectorStartAtOffsetStepLength : IReadOnlyList<Int16>
		{
			private Int16 _start;
			private int _startOffset;

			private Int16 _increment;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the element at index <paramref name="startOffset"/> of the vector.</param>
			/// <param name="startOffset">The index of the element for which a value is provided in <paramref name="start"/>.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
			public ROShortEquidistantElementVectorStartAtOffsetStepLength(Int16 start, int startOffset, Int16 increment, int length)
			{
				_start = start;
				_startOffset = startOffset;
				_increment = increment;
				_length = length;
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Length
			{
				get { return _length; }
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Count
			{
				get { return _length; }
			}

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
			public Int16 this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");
					return (Int16)(_start + (i - _startOffset) * _increment);
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<Int16> GetEnumerator()
      {
        for (int i = 0; i<_length; ++i)
          yield return this[i];
      }

		/// <summary>
		/// Enumerates all elements of the vector.
		/// </summary>
		/// <returns>Enumerator that enumerates all elements of the vector.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < _length; ++i)
				yield return this[i];
		}
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
		public static IReadOnlyList<Int16> CreateEquidistantSequencyByStartAtOffsetStepLength(Int16 start, int startOffset, Int16 step, int length)
		{
			return new ROShortEquidistantElementVectorStartAtOffsetStepLength(start, startOffset, step, length);
		}


	  	/// <summary>
		/// Provides a read-only vector with equally spaced elements so that y[0] = start and y[length-1] = end.
		/// </summary>
		private class ROShortEquidistantElementVectorStartEndLength : IReadOnlyList<Int16>
		{
			private Int16 _start;
			private Int16 _end;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="end">Value of the last element of the vector.</param>
			/// <param name="length">Length of the vector.</param>
			public ROShortEquidistantElementVectorStartEndLength(Int16 start, Int16 end, int length)
			{
				_start = start;
				_end = end;
				_length = length;
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Length
			{
				get { return _length; }
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Count
			{
				get { return _length; }
			}

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
			public Int16 this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");

					double r = i / (double)(_length - 1);
					return (Int16)(_start * (1 - r) + _end * (r));
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<Int16> GetEnumerator()
      {
        for (int i = 0; i<_length; ++i)
          yield return this[i];
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _length; ++i)
					yield return this[i];
			}
		}

		/// <summary>
		/// Creates a read-only vector with equidistant element values from start to end. The created vector
		/// consumes memory only for the three variables, independent of its length.
		/// </summary>
		/// <param name="start">First element of the vector.</param>
		/// <param name="end">Last element of the vector.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with equidistant element values from start to end.</returns>
		public static IReadOnlyList<Int16> CreateEquidistantSequenceByStartEndLength(Int16 start, Int16 end, int length)
		{
			return new ROShortEquidistantElementVectorStartEndLength(start, end, length);
		}


		/// <summary>
    /// Serves as wrapper for an RO Vector which returns the inverse of the elements of the original array.
    /// </summary>
    private class ROShortInverseElementWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected IReadOnlyList<Int16> _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROShortInverseElementWrapper(IReadOnlyList<Int16> x)
      {
        _x = x;
        _length = x.Count;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROShortInverseElementWrapper(IReadOnlyList<Int16> x, int usedlength)
      {
        if (usedlength > x.Count)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
          return 1/(double)_x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a Int16[] array to get an  <see cref="IROVector{Int16}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int16}" /> returning elements that are inverse to those of the original vector.</returns>
		public static IReadOnlyList<double> ToInverseROVector(this IReadOnlyList<Int16> array)
		{
			return array is null ? null : new ROShortInverseElementWrapper(array);
		}

		/// <summary>
		/// Wraps a Int16[] array till a given length to get an <see cref="IROVector{Int16}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int16}" /> interface that wraps the provided array returning elements that are inverse to those of the original vector..</returns>
		public static IReadOnlyList<double> ToInverseROVector(this IReadOnlyList<Int16> array, int usedlength)
		{
			return new ROShortInverseElementWrapper(array, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{Int16}" /> is neccessary.
    /// </summary>
    private class ROShortArrayWrapper : IReadOnlyList<Int16>
    {
      private int _length;
      protected Int16[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROShortArrayWrapper(Int16[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROShortArrayWrapper(Int16[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public Int16 this[int i]
      {
        get
        {
          return _x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<Int16> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a Int16[] array to get an <see cref="IROVector{Int16}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<Int16> ToROVector(this Int16[] array)
		{
			return array is null ? null : new ROShortArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a Int16[] array till a given length to get an <see cref="IROVector{Int16}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<Int16> ToROVector(this Int16[] array, int usedlength)
		{
			return new ROShortArrayWrapper(array, usedlength);
		}

			/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{Int16}" /> is neccessary.
    /// </summary>
    private class RODouble_ShortArrayWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected Int16[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODouble_ShortArrayWrapper(Int16[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODouble_ShortArrayWrapper(Int16[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
          return _x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return (double)this[i];
      }
    }

		/// <summary>
		/// Wraps a Int16[] array to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this Int16[] array)
		{
			return array is null ? null : new RODouble_ShortArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a Int16[] array till a given length to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this Int16[] array, int usedlength)
		{
			return new RODouble_ShortArrayWrapper(array, usedlength);
		}


		  /// <summary>
    /// Serves as wrapper for a section of an array to plug-in where an <see cref="IROVector{Int16}" /> is neccessary.
    /// </summary>
    private class ROShortArraySectionWrapper : IReadOnlyList<Int16>
    {
      protected Int16[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public ROShortArraySectionWrapper(Int16[] x)
      {
        _length = x.Length;
        _start = 0;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array, and start and length of the section, for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap.</param>
      /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROShortArraySectionWrapper(Int16[] x, int start, int usedlength)
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
      public Int16 this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      /// <summary>
      /// Returns an enumerator that iterates through the elements of the vector.
      /// </summary>
      /// <returns>
      /// An enumerator that can be used to iterate through the elements of the vector.
      /// </returns>
      public IEnumerator<Int16> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps an array to an <see cref="IROVector{Int16}" />. Start and length of the used section of the array are specified in the parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of the element in <paramref name="array"/> used as the first element of the vector.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper object with the <see cref="IROVector{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<Int16> ToROVector(this Int16[] array, int start, int usedlength)
		{
			if (0 == start)
				return new ROShortArrayWrapper(array, usedlength);
			else
				return new ROShortArraySectionWrapper(array, start, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{Int16}" /> is neccessary.
    /// </summary>
    private class ROShortArrayWrapperAmendedShifted : IReadOnlyList<Int16>
    {
      private int _length;
      protected Int16[] _x;
			protected Int16 _amendedValueAtStart;
			protected int _amendedValuesAtStartCount;
			protected Int16 _amendedValueAtEnd;
			protected int _amendedValuesAtEndCount;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public ROShortArrayWrapperAmendedShifted(Int16[] x, Int16 amendedValueAtStart, int amendedValuesAtStartCount, Int16 amendedValueAtEnd, int amendedValuesAtEndCount)
      {
        _length = x.Length;
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValuesAtStartCount = amendedValuesAtStartCount;
			 _amendedValueAtEnd = amendedValueAtEnd;
			 _amendedValuesAtEndCount = amendedValuesAtEndCount;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public ROShortArrayWrapperAmendedShifted(Int16[] x, int usedlength, Int16 amendedValueAtStart, int amendedValuesAtStartCount, Int16 amendedValueAtEnd, int amendedValuesAtEndCount)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValuesAtStartCount = amendedValuesAtStartCount;
			 _amendedValueAtEnd = amendedValueAtEnd;
			 _amendedValuesAtEndCount = amendedValuesAtEndCount;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public Int16 this[int i]
      {
        get
        {
					if(i<_amendedValuesAtStartCount)
						return _amendedValueAtStart;
          else if(i<_length +_amendedValuesAtStartCount)
						return _x[i - _amendedValuesAtStartCount];
					else 
						return _amendedValueAtEnd;
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length + _amendedValuesAtStartCount + _amendedValuesAtEndCount;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
           return _length + _amendedValuesAtStartCount + _amendedValuesAtEndCount;
        }
      }

      public IEnumerator<Int16> GetEnumerator()
      {
				for (int i = 0; i < _amendedValuesAtStartCount; ++i)
					yield return _amendedValueAtStart;
				for (int i = 0; i < _length; ++i)
					yield return _x[i];
				for (int i = 0; i < _amendedValuesAtEndCount; ++i)
					yield return _amendedValueAtEnd;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
				for (int i = 0; i < _amendedValuesAtStartCount; ++i)
					yield return _amendedValueAtStart;
				for (int i = 0; i < _length; ++i)
					yield return _x[i];
				for (int i = 0; i < _amendedValuesAtEndCount; ++i)
					yield return _amendedValueAtEnd;
      }
    }

			/// <summary>
		/// Wraps a Int16[] array to get an <see cref="IROVector{Int16}" />.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<Int16> ToROVectorAmendedShifted(this Int16[] array,Int16 amendedValueAtStart, int amendedValuesAtStartCount, Int16 amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return array is null ? null : new ROShortArrayWrapperAmendedShifted(array, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}

		/// <summary>
		/// Wraps a Int16[] array till a given length to get an <see cref="IROVector{Int16}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<Int16> ToROVectorAmendedShifted(this Int16[] array, int usedlength,Int16 amendedValueAtStart, int amendedValuesAtStartCount, Int16 amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return new ROShortArrayWrapperAmendedShifted(array, usedlength, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}


		/// <summary>
    /// Serves as thin wrapper struct for an array when additional data at the start and the end of the array are neccessary.
    /// </summary>
    public struct ROShortArrayWrapperStructAmendedUnshifted : IReadOnlyList<Int16>
    {
      private int _length;
      private  Int16[] _x;
			private  Int16 _amendedValueAtStart;
			private  Int16 _amendedValueAtEnd;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
      public ROShortArrayWrapperStructAmendedUnshifted(Int16[] x, Int16 amendedValueAtStart, Int16 amendedValueAtEnd)
      {
        _length = x.Length;
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValueAtEnd = amendedValueAtEnd;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
	      public ROShortArrayWrapperStructAmendedUnshifted(Int16[] x, int usedlength, Int16 amendedValueAtStart, Int16 amendedValueAtEnd)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValueAtEnd = amendedValueAtEnd;
      }

      /// <summary>Gets the value at index i. For indices &lt;0, the value amendedValueAtStart is returned.
			/// For indices &gt;=Length, the value amendedValueAtEnd is returned.</summary>
      /// <value>The element at index i.</value>
      public Int16 this[int i]
      {
        get
        {
					if(i<0)
						return _amendedValueAtStart;
          else if(i<_length)
						return _x[i];
					else 
						return _amendedValueAtEnd;
        }
      }

      /// <summary>Attention! Returns the length of the wrapped part of the array.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>Attention! Returns the length of the wrapped part of the array.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<Int16> GetEnumerator()
      {
				yield return _amendedValueAtStart;

				for (int i = 0; i < _length; ++i)
					yield return _x[i];

				yield return _amendedValueAtEnd;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
				yield return _amendedValueAtStart;

				for (int i = 0; i < _length; ++i)
					yield return _x[i];

				yield return _amendedValueAtEnd;
      }
    }

		
		/// <summary>
		/// Wraps a Int16[] array to get a struct with an <see cref="IROVector{Int16}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above Length, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater than or equal to <paramref name="array"/>.Length.</param>.
		/// <returns>A wrapper struct with the <see cref="IROVector{Int16}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROShortArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this Int16[] array,Int16 amendedValueAtStart, Int16 amendedValueAtEnd)
		{
			return new ROShortArrayWrapperStructAmendedUnshifted(array, amendedValueAtStart, amendedValueAtEnd);
		}

		/// <summary>
		/// Wraps a Int16[] array till a given length to get a struct with an <see cref="IROVector{Int16}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above <paramref name="usedlength"/>, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to <paramref name="usedlength"/>.</param>.
		/// <returns>A wrapper struct with the <see cref="IROVector{Int16}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROShortArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this Int16[] array, int usedlength,Int16 amendedValueAtStart, Int16 amendedValueAtEnd)
		{
			return new ROShortArrayWrapperStructAmendedUnshifted(array, usedlength, amendedValueAtStart, amendedValueAtEnd );
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where a IVector is neccessary.
    /// </summary>
		private class RWShortArrayWrapper : ROShortArrayWrapper, IVector<Int16>
    {
      public RWShortArrayWrapper(Int16[] x)
        : base(x)
      {
      }

      public RWShortArrayWrapper(Int16[] x, int usedlength)
        : base(x, usedlength)
      {
      }

      public new Int16 this[int i]
      {
        get { return _x[i]; }
        set { _x[i] = value; }
      }
    }


			/// <summary>
		/// Wraps an array to get an <see cref="IVector{Int16}" />
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Int16}" /> interface that wraps the provided array.</returns>
		public static IVector<Int16> ToVector(this Int16[] array)
		{
			return new RWShortArrayWrapper(array);
		}

		/// <summary>
		/// Wraps an array to get an <see cref="IVector{Int16}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Used length of the array to get the wrapped vector (i.e. the vector wraps around <paramref name="array"/>[0..usedLength-1]).</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Int16}" /> interface that wraps the provided array.</returns>
		public static IVector<Int16> ToVector(Int16[] array, int usedlength)
		{
			return new RWShortArrayWrapper(array, usedlength);
		}

		
		private class RWShortArraySectionWrapper : ROShortArraySectionWrapper, IVector<Int16>
    {
      public RWShortArraySectionWrapper(Int16[] x)
        : base(x)
      {
      }

      public RWShortArraySectionWrapper(Int16[] x, int start, int usedlength)
        : base(x, start, usedlength)
      {
      }

      public new Int16 this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }
    }

		/// <summary>
		/// Wraps a section of an array to get a <see cref="IVector{Int16}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of first element of <paramref name="array"/> to use.</param>
		/// <param name="count">Number of elements of <paramref name="array"/> to use.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Int16}" /> interface that wraps a section of the provided array.</returns>
		public static IVector<Int16> ToVector(this Int16[] array, int start, int count)
		{
			if (0 == start)
				return new RWShortArrayWrapper(array, count);
			else
				return new RWShortArraySectionWrapper(array, start, count);
		}



		 /// <summary>
    /// Serves as wrapper for an <see cref="IROVector{Int16}" /> to get only a section of the original wrapper.
    /// </summary>
    private class ROShortVectorSectionWrapper : IReadOnlyList<Int16>
    {
      protected IReadOnlyList<Int16> _x;
      private int _start;
      private int _length;


		 /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The vector to wrap.</param>
      /// <param name="start">Start index of the section of the vector to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public ROShortVectorSectionWrapper(IReadOnlyList<Int16> x, int start, int len)
      {
        if (start >= x.Count)
          throw new ArgumentException("Start of the section is beyond length of the vector");
        if (start + len > x.Count)
          throw new ArgumentException("End of the section is beyond length of the vector");

        _x = x;
        _start = start;
        _length = len;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public Int16 this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<Int16> GetEnumerator()
      {
        for(int i = 0; i < _length; ++i)
					yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for(int i = 0; i < _length; ++i)
					yield return this[i];
      }
    }


		/// <summary>
		/// Wraps a section of an original vector into a new vector.
		/// </summary>
		/// <param name="vector">Original vector.</param>
		/// <param name="start">Index of the start of the section to wrap.</param>
		/// <param name="usedLength">Length (=number of elements) of the section to wrap.</param>
		/// <returns>An <see cref="IROVector{Int16}" /> that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IReadOnlyList<Int16> ToROVector(this IReadOnlyList<Int16> vector, int start, int usedLength)
		{
			return new ROShortVectorSectionWrapper(vector, start, usedLength);
		}

		  /// <summary>
    /// Serves as wrapper for an IVector to get only a section of the original wrapper.
    /// </summary>
    private class RWShortVectorSectionWrapper : IVector<Int16>
    {
      protected IVector<Int16> _x;
      private int _start;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Start index of the section to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RWShortVectorSectionWrapper(IVector<Int16> x, int start, int len)
      {
        if (start >= x.Count)
          throw new ArgumentException("Start of the section is beyond length of the vector");
        if (start + len > x.Count)
          throw new ArgumentException("End of the section is beyond length of the vector");

        _x = x;
        _start = start;
        _length = len;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public Int16 this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<Int16> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a section of an original vector into a new vector.
		/// </summary>
		/// <param name="vector">Original vector.</param>
		/// <param name="start">Index of the start of the section to wrap.</param>
		/// <param name="len">Length (=number of elements) of the section to wrap.</param>
		/// <returns>A IVector that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IVector<Int16> ToVector(this IVector<Int16> vector, int start, int len)
		{
			return new RWShortVectorSectionWrapper(vector, start, len);
		}

    /// <summary>
	/// Returns a clone of the source vector.
	/// </summary>
	/// <param name="sourceVector">The source vector.</param>
    /// <returns>A freshly allocated clone of the sourceVector, with the same element values as the source vector.</returns>
	public static Int16[] Clone(Int16[] sourceVector)
	{
		if (sourceVector is null)
			throw new ArgumentNullException(nameof(sourceVector));

        var destVector = new Int16[sourceVector.Length];
        Array.Copy(sourceVector, destVector, sourceVector.Length);
        return destVector;
	}



// ******************************************** Definitions for SByte *******************************************

    /// <summary>
		/// Provides a read-only vector with equal and constant items.
		/// </summary>
		private class ROSByteConstantVector : IReadOnlyList<SByte>
		{
			private int _length;
			private SByte _value;

			public ROSByteConstantVector(SByte value, int length)
			{
				_length = length;
				_value = value;
			}

			public int Length
			{
				get { return _length; }
			}

      public int Count
      {
        get { return _length; }
      }

      public SByte this[int i]
			{
				get { return _value; }
			}

      public IEnumerator<SByte> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return _value;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return _value;
      }
    }

			/// <summary>
		/// Gets a vector with all elements equal to a provided value.
		/// </summary>
		/// <param name="value">Value of all elements.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with all elements equal to the provided <paramref name="value"/>.</returns>
		public static IReadOnlyList<SByte> GetConstantVector(SByte value, int length)
		{
			return new ROSByteConstantVector(value, length);
		}


		/// <summary>
    /// Provides a read-only vector with equally spaced elements y[i] = start + i * increment.
    /// </summary>
    private class ROSByteEquidistantElementVector : IReadOnlyList<SByte>
    {
      private int _length;
      private SByte _startValue;
      private SByte _incrementValue;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
      public ROSByteEquidistantElementVector(SByte start, SByte increment, int length)
      {
        _length = length;
        _startValue = start;
        _incrementValue = increment;
      }

			/// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get { return _length; }
      }

      	/// <summary>The number of elements of this vector.</summary>
			public int Count
      {
        get { return _length; }
      }

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
      public SByte this[int i]
      {
        get { return (SByte)(_startValue + i * _incrementValue); }
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      public IEnumerator<SByte> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }


			/// <summary>
		/// Creates a read-only vector with equidistant elements with values from start to start+(length-1)*step. The created vector
		/// consumes memory only for the three variables, independent of its length.
		/// </summary>
		/// <param name="start">First element of the vector.</param>
		/// <param name="step">Difference between two successive elements.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with equidistant elements with values from start to start+(length-1)*step.</returns>
		public static IReadOnlyList<SByte> CreateEquidistantSequenceByStartStepLength(SByte start, SByte step, int length)
		{
			return new ROSByteEquidistantElementVector(start, step, length);
		}

			
			/// <summary>
		/// Provides a read-only vector with equally spaced elements y[i] = start + (i-startOffset) * increment.
		/// </summary>
		private class ROSByteEquidistantElementVectorStartAtOffsetStepLength : IReadOnlyList<SByte>
		{
			private SByte _start;
			private int _startOffset;

			private SByte _increment;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the element at index <paramref name="startOffset"/> of the vector.</param>
			/// <param name="startOffset">The index of the element for which a value is provided in <paramref name="start"/>.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
			public ROSByteEquidistantElementVectorStartAtOffsetStepLength(SByte start, int startOffset, SByte increment, int length)
			{
				_start = start;
				_startOffset = startOffset;
				_increment = increment;
				_length = length;
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Length
			{
				get { return _length; }
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Count
			{
				get { return _length; }
			}

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
			public SByte this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");
					return (SByte)(_start + (i - _startOffset) * _increment);
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<SByte> GetEnumerator()
      {
        for (int i = 0; i<_length; ++i)
          yield return this[i];
      }

		/// <summary>
		/// Enumerates all elements of the vector.
		/// </summary>
		/// <returns>Enumerator that enumerates all elements of the vector.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < _length; ++i)
				yield return this[i];
		}
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
		public static IReadOnlyList<SByte> CreateEquidistantSequencyByStartAtOffsetStepLength(SByte start, int startOffset, SByte step, int length)
		{
			return new ROSByteEquidistantElementVectorStartAtOffsetStepLength(start, startOffset, step, length);
		}


	  	/// <summary>
		/// Provides a read-only vector with equally spaced elements so that y[0] = start and y[length-1] = end.
		/// </summary>
		private class ROSByteEquidistantElementVectorStartEndLength : IReadOnlyList<SByte>
		{
			private SByte _start;
			private SByte _end;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="end">Value of the last element of the vector.</param>
			/// <param name="length">Length of the vector.</param>
			public ROSByteEquidistantElementVectorStartEndLength(SByte start, SByte end, int length)
			{
				_start = start;
				_end = end;
				_length = length;
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Length
			{
				get { return _length; }
			}

			/// <summary>The number of elements of this vector.</summary>
			public int Count
			{
				get { return _length; }
			}

			/// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
			/// <value>The element at index i.</value>
			public SByte this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");

					double r = i / (double)(_length - 1);
					return (SByte)(_start * (1 - r) + _end * (r));
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<SByte> GetEnumerator()
      {
        for (int i = 0; i<_length; ++i)
          yield return this[i];
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _length; ++i)
					yield return this[i];
			}
		}

		/// <summary>
		/// Creates a read-only vector with equidistant element values from start to end. The created vector
		/// consumes memory only for the three variables, independent of its length.
		/// </summary>
		/// <param name="start">First element of the vector.</param>
		/// <param name="end">Last element of the vector.</param>
		/// <param name="length">Length of the vector.</param>
		/// <returns>Read-only vector with equidistant element values from start to end.</returns>
		public static IReadOnlyList<SByte> CreateEquidistantSequenceByStartEndLength(SByte start, SByte end, int length)
		{
			return new ROSByteEquidistantElementVectorStartEndLength(start, end, length);
		}


		/// <summary>
    /// Serves as wrapper for an RO Vector which returns the inverse of the elements of the original array.
    /// </summary>
    private class ROSByteInverseElementWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected IReadOnlyList<SByte> _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROSByteInverseElementWrapper(IReadOnlyList<SByte> x)
      {
        _x = x;
        _length = x.Count;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROSByteInverseElementWrapper(IReadOnlyList<SByte> x, int usedlength)
      {
        if (usedlength > x.Count)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
          return 1/(double)_x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a SByte[] array to get an  <see cref="IROVector{SByte}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{SByte}" /> returning elements that are inverse to those of the original vector.</returns>
		public static IReadOnlyList<double> ToInverseROVector(this IReadOnlyList<SByte> array)
		{
			return array is null ? null : new ROSByteInverseElementWrapper(array);
		}

		/// <summary>
		/// Wraps a SByte[] array till a given length to get an <see cref="IROVector{SByte}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{SByte}" /> interface that wraps the provided array returning elements that are inverse to those of the original vector..</returns>
		public static IReadOnlyList<double> ToInverseROVector(this IReadOnlyList<SByte> array, int usedlength)
		{
			return new ROSByteInverseElementWrapper(array, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{SByte}" /> is neccessary.
    /// </summary>
    private class ROSByteArrayWrapper : IReadOnlyList<SByte>
    {
      private int _length;
      protected SByte[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROSByteArrayWrapper(SByte[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROSByteArrayWrapper(SByte[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public SByte this[int i]
      {
        get
        {
          return _x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<SByte> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a SByte[] array to get an <see cref="IROVector{SByte}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<SByte> ToROVector(this SByte[] array)
		{
			return array is null ? null : new ROSByteArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a SByte[] array till a given length to get an <see cref="IROVector{SByte}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<SByte> ToROVector(this SByte[] array, int usedlength)
		{
			return new ROSByteArrayWrapper(array, usedlength);
		}

			/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{SByte}" /> is neccessary.
    /// </summary>
    private class RODouble_SByteArrayWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected SByte[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODouble_SByteArrayWrapper(SByte[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODouble_SByteArrayWrapper(SByte[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public double this[int i]
      {
        get
        {
          return _x[i];
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<double> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return (double)this[i];
      }
    }

		/// <summary>
		/// Wraps a SByte[] array to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this SByte[] array)
		{
			return array is null ? null : new RODouble_SByteArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a SByte[] array till a given length to get an <see cref="IROVector{double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this SByte[] array, int usedlength)
		{
			return new RODouble_SByteArrayWrapper(array, usedlength);
		}


		  /// <summary>
    /// Serves as wrapper for a section of an array to plug-in where an <see cref="IROVector{SByte}" /> is neccessary.
    /// </summary>
    private class ROSByteArraySectionWrapper : IReadOnlyList<SByte>
    {
      protected SByte[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public ROSByteArraySectionWrapper(SByte[] x)
      {
        _length = x.Length;
        _start = 0;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array, and start and length of the section, for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap.</param>
      /// <param name="start">Index of the element in <paramref name="x"/> used as the first element of the vector.</param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROSByteArraySectionWrapper(SByte[] x, int start, int usedlength)
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
      public SByte this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      /// <summary>
      /// Returns an enumerator that iterates through the elements of the vector.
      /// </summary>
      /// <returns>
      /// An enumerator that can be used to iterate through the elements of the vector.
      /// </returns>
      public IEnumerator<SByte> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps an array to an <see cref="IROVector{SByte}" />. Start and length of the used section of the array are specified in the parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of the element in <paramref name="array"/> used as the first element of the vector.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper object with the <see cref="IROVector{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<SByte> ToROVector(this SByte[] array, int start, int usedlength)
		{
			if (0 == start)
				return new ROSByteArrayWrapper(array, usedlength);
			else
				return new ROSByteArraySectionWrapper(array, start, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IROVector{SByte}" /> is neccessary.
    /// </summary>
    private class ROSByteArrayWrapperAmendedShifted : IReadOnlyList<SByte>
    {
      private int _length;
      protected SByte[] _x;
			protected SByte _amendedValueAtStart;
			protected int _amendedValuesAtStartCount;
			protected SByte _amendedValueAtEnd;
			protected int _amendedValuesAtEndCount;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public ROSByteArrayWrapperAmendedShifted(SByte[] x, SByte amendedValueAtStart, int amendedValuesAtStartCount, SByte amendedValueAtEnd, int amendedValuesAtEndCount)
      {
        _length = x.Length;
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValuesAtStartCount = amendedValuesAtStartCount;
			 _amendedValueAtEnd = amendedValueAtEnd;
			 _amendedValuesAtEndCount = amendedValuesAtEndCount;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public ROSByteArrayWrapperAmendedShifted(SByte[] x, int usedlength, SByte amendedValueAtStart, int amendedValuesAtStartCount, SByte amendedValueAtEnd, int amendedValuesAtEndCount)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValuesAtStartCount = amendedValuesAtStartCount;
			 _amendedValueAtEnd = amendedValueAtEnd;
			 _amendedValuesAtEndCount = amendedValuesAtEndCount;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public SByte this[int i]
      {
        get
        {
					if(i<_amendedValuesAtStartCount)
						return _amendedValueAtStart;
          else if(i<_length +_amendedValuesAtStartCount)
						return _x[i - _amendedValuesAtStartCount];
					else 
						return _amendedValueAtEnd;
        }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length
      {
        get
        {
          return _length + _amendedValuesAtStartCount + _amendedValuesAtEndCount;
        }
      } 

     /// <summary>The number of elements of this vector.</summary>
     public int Count
      {
        get
        {
           return _length + _amendedValuesAtStartCount + _amendedValuesAtEndCount;
        }
      }

      public IEnumerator<SByte> GetEnumerator()
      {
				for (int i = 0; i < _amendedValuesAtStartCount; ++i)
					yield return _amendedValueAtStart;
				for (int i = 0; i < _length; ++i)
					yield return _x[i];
				for (int i = 0; i < _amendedValuesAtEndCount; ++i)
					yield return _amendedValueAtEnd;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
				for (int i = 0; i < _amendedValuesAtStartCount; ++i)
					yield return _amendedValueAtStart;
				for (int i = 0; i < _length; ++i)
					yield return _x[i];
				for (int i = 0; i < _amendedValuesAtEndCount; ++i)
					yield return _amendedValueAtEnd;
      }
    }

			/// <summary>
		/// Wraps a SByte[] array to get an <see cref="IROVector{SByte}" />.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<SByte> ToROVectorAmendedShifted(this SByte[] array,SByte amendedValueAtStart, int amendedValuesAtStartCount, SByte amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return array is null ? null : new ROSByteArrayWrapperAmendedShifted(array, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}

		/// <summary>
		/// Wraps a SByte[] array till a given length to get an <see cref="IROVector{SByte}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IROVector{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<SByte> ToROVectorAmendedShifted(this SByte[] array, int usedlength,SByte amendedValueAtStart, int amendedValuesAtStartCount, SByte amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return new ROSByteArrayWrapperAmendedShifted(array, usedlength, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}


		/// <summary>
    /// Serves as thin wrapper struct for an array when additional data at the start and the end of the array are neccessary.
    /// </summary>
    public struct ROSByteArrayWrapperStructAmendedUnshifted : IReadOnlyList<SByte>
    {
      private int _length;
      private  SByte[] _x;
			private  SByte _amendedValueAtStart;
			private  SByte _amendedValueAtEnd;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
      public ROSByteArrayWrapperStructAmendedUnshifted(SByte[] x, SByte amendedValueAtStart, SByte amendedValueAtEnd)
      {
        _length = x.Length;
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValueAtEnd = amendedValueAtEnd;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
	      public ROSByteArrayWrapperStructAmendedUnshifted(SByte[] x, int usedlength, SByte amendedValueAtStart, SByte amendedValueAtEnd)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
			 _amendedValueAtStart = amendedValueAtStart;
			 _amendedValueAtEnd = amendedValueAtEnd;
      }

      /// <summary>Gets the value at index i. For indices &lt;0, the value amendedValueAtStart is returned.
			/// For indices &gt;=Length, the value amendedValueAtEnd is returned.</summary>
      /// <value>The element at index i.</value>
      public SByte this[int i]
      {
        get
        {
					if(i<0)
						return _amendedValueAtStart;
          else if(i<_length)
						return _x[i];
					else 
						return _amendedValueAtEnd;
        }
      }

      /// <summary>Attention! Returns the length of the wrapped part of the array.</summary>
      public int Length
      {
        get
        {
          return _length;
        }
      } 

     /// <summary>Attention! Returns the length of the wrapped part of the array.</summary>
     public int Count
      {
        get
        {
          return _length;
        }
      }

      public IEnumerator<SByte> GetEnumerator()
      {
				yield return _amendedValueAtStart;

				for (int i = 0; i < _length; ++i)
					yield return _x[i];

				yield return _amendedValueAtEnd;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
				yield return _amendedValueAtStart;

				for (int i = 0; i < _length; ++i)
					yield return _x[i];

				yield return _amendedValueAtEnd;
      }
    }

		
		/// <summary>
		/// Wraps a SByte[] array to get a struct with an <see cref="IROVector{SByte}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above Length, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater than or equal to <paramref name="array"/>.Length.</param>.
		/// <returns>A wrapper struct with the <see cref="IROVector{SByte}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROSByteArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this SByte[] array,SByte amendedValueAtStart, SByte amendedValueAtEnd)
		{
			return new ROSByteArrayWrapperStructAmendedUnshifted(array, amendedValueAtStart, amendedValueAtEnd);
		}

		/// <summary>
		/// Wraps a SByte[] array till a given length to get a struct with an <see cref="IROVector{SByte}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above <paramref name="usedlength"/>, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to <paramref name="usedlength"/>.</param>.
		/// <returns>A wrapper struct with the <see cref="IROVector{SByte}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROSByteArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this SByte[] array, int usedlength,SByte amendedValueAtStart, SByte amendedValueAtEnd)
		{
			return new ROSByteArrayWrapperStructAmendedUnshifted(array, usedlength, amendedValueAtStart, amendedValueAtEnd );
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where a IVector is neccessary.
    /// </summary>
		private class RWSByteArrayWrapper : ROSByteArrayWrapper, IVector<SByte>
    {
      public RWSByteArrayWrapper(SByte[] x)
        : base(x)
      {
      }

      public RWSByteArrayWrapper(SByte[] x, int usedlength)
        : base(x, usedlength)
      {
      }

      public new SByte this[int i]
      {
        get { return _x[i]; }
        set { _x[i] = value; }
      }
    }


			/// <summary>
		/// Wraps an array to get an <see cref="IVector{SByte}" />
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{SByte}" /> interface that wraps the provided array.</returns>
		public static IVector<SByte> ToVector(this SByte[] array)
		{
			return new RWSByteArrayWrapper(array);
		}

		/// <summary>
		/// Wraps an array to get an <see cref="IVector{SByte}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Used length of the array to get the wrapped vector (i.e. the vector wraps around <paramref name="array"/>[0..usedLength-1]).</param>
		/// <returns>A wrapper objects with the <see cref="IVector{SByte}" /> interface that wraps the provided array.</returns>
		public static IVector<SByte> ToVector(SByte[] array, int usedlength)
		{
			return new RWSByteArrayWrapper(array, usedlength);
		}

		
		private class RWSByteArraySectionWrapper : ROSByteArraySectionWrapper, IVector<SByte>
    {
      public RWSByteArraySectionWrapper(SByte[] x)
        : base(x)
      {
      }

      public RWSByteArraySectionWrapper(SByte[] x, int start, int usedlength)
        : base(x, start, usedlength)
      {
      }

      public new SByte this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }
    }

		/// <summary>
		/// Wraps a section of an array to get a <see cref="IVector{SByte}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of first element of <paramref name="array"/> to use.</param>
		/// <param name="count">Number of elements of <paramref name="array"/> to use.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{SByte}" /> interface that wraps a section of the provided array.</returns>
		public static IVector<SByte> ToVector(this SByte[] array, int start, int count)
		{
			if (0 == start)
				return new RWSByteArrayWrapper(array, count);
			else
				return new RWSByteArraySectionWrapper(array, start, count);
		}



		 /// <summary>
    /// Serves as wrapper for an <see cref="IROVector{SByte}" /> to get only a section of the original wrapper.
    /// </summary>
    private class ROSByteVectorSectionWrapper : IReadOnlyList<SByte>
    {
      protected IReadOnlyList<SByte> _x;
      private int _start;
      private int _length;


		 /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The vector to wrap.</param>
      /// <param name="start">Start index of the section of the vector to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public ROSByteVectorSectionWrapper(IReadOnlyList<SByte> x, int start, int len)
      {
        if (start >= x.Count)
          throw new ArgumentException("Start of the section is beyond length of the vector");
        if (start + len > x.Count)
          throw new ArgumentException("End of the section is beyond length of the vector");

        _x = x;
        _start = start;
        _length = len;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public SByte this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<SByte> GetEnumerator()
      {
        for(int i = 0; i < _length; ++i)
					yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for(int i = 0; i < _length; ++i)
					yield return this[i];
      }
    }


		/// <summary>
		/// Wraps a section of an original vector into a new vector.
		/// </summary>
		/// <param name="vector">Original vector.</param>
		/// <param name="start">Index of the start of the section to wrap.</param>
		/// <param name="usedLength">Length (=number of elements) of the section to wrap.</param>
		/// <returns>An <see cref="IROVector{SByte}" /> that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IReadOnlyList<SByte> ToROVector(this IReadOnlyList<SByte> vector, int start, int usedLength)
		{
			return new ROSByteVectorSectionWrapper(vector, start, usedLength);
		}

		  /// <summary>
    /// Serves as wrapper for an IVector to get only a section of the original wrapper.
    /// </summary>
    private class RWSByteVectorSectionWrapper : IVector<SByte>
    {
      protected IVector<SByte> _x;
      private int _start;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Start index of the section to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RWSByteVectorSectionWrapper(IVector<SByte> x, int start, int len)
      {
        if (start >= x.Count)
          throw new ArgumentException("Start of the section is beyond length of the vector");
        if (start + len > x.Count)
          throw new ArgumentException("End of the section is beyond length of the vector");

        _x = x;
        _start = start;
        _length = len;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public SByte this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<SByte> GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        for (int i = 0; i < _length; ++i)
          yield return this[i];
      }
    }

		/// <summary>
		/// Wraps a section of an original vector into a new vector.
		/// </summary>
		/// <param name="vector">Original vector.</param>
		/// <param name="start">Index of the start of the section to wrap.</param>
		/// <param name="len">Length (=number of elements) of the section to wrap.</param>
		/// <returns>A IVector that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IVector<SByte> ToVector(this IVector<SByte> vector, int start, int len)
		{
			return new RWSByteVectorSectionWrapper(vector, start, len);
		}

    /// <summary>
	/// Returns a clone of the source vector.
	/// </summary>
	/// <param name="sourceVector">The source vector.</param>
    /// <returns>A freshly allocated clone of the sourceVector, with the same element values as the source vector.</returns>
	public static SByte[] Clone(SByte[] sourceVector)
	{
		if (sourceVector is null)
			throw new ArgumentNullException(nameof(sourceVector));

        var destVector = new SByte[sourceVector.Length];
        Array.Copy(sourceVector, destVector, sourceVector.Length);
        return destVector;
	}



	} // class
} // namespace
