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

#pragma warning disable CS3002 // Disable not CLS-compliant warning for generated code
#pragma warning disable CS3003 // Disable not CLS-compliant warning for generated code
// ******************************************** Definitions for System.Double *******************************************

    /// <summary>
		/// Provides a read-only vector with equal and constant items.
		/// </summary>
		private class RODoubleConstantVector : IReadOnlyList<System.Double>
		{
			private int _length;
			private System.Double _value;

			public RODoubleConstantVector(System.Double value, int length)
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

      public System.Double this[int i]
			{
				get { return _value; }
			}

      public IEnumerator<System.Double> GetEnumerator()
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
		public static IReadOnlyList<System.Double> GetConstantVector(System.Double value, int length)
		{
			return new RODoubleConstantVector(value, length);
		}


		/// <summary>
    /// Provides a read-only vector with equally spaced elements y[i] = start + i * increment.
    /// </summary>
    private class RODoubleEquidistantElementVector : IReadOnlyList<System.Double>
    {
      private int _length;
      private System.Double _startValue;
      private System.Double _incrementValue;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
      public RODoubleEquidistantElementVector(System.Double start, System.Double increment, int length)
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
      public System.Double this[int i]
      {
        get { return (System.Double)(_startValue + i * _incrementValue); }
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      public IEnumerator<System.Double> GetEnumerator()
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
		public static IReadOnlyList<System.Double> CreateEquidistantSequenceByStartStepLength(System.Double start, System.Double step, int length)
		{
			return new RODoubleEquidistantElementVector(start, step, length);
		}

			
			/// <summary>
		/// Provides a read-only vector with equally spaced elements y[i] = start + (i-startOffset) * increment.
		/// </summary>
		private class RODoubleEquidistantElementVectorStartAtOffsetStepLength : IReadOnlyList<System.Double>
		{
			private System.Double _start;
			private int _startOffset;

			private System.Double _increment;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the element at index <paramref name="startOffset"/> of the vector.</param>
			/// <param name="startOffset">The index of the element for which a value is provided in <paramref name="start"/>.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
			public RODoubleEquidistantElementVectorStartAtOffsetStepLength(System.Double start, int startOffset, System.Double increment, int length)
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
			public System.Double this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");
					return (System.Double)(_start + (i - _startOffset) * _increment);
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<System.Double> GetEnumerator()
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
		public static IReadOnlyList<System.Double> CreateEquidistantSequencyByStartAtOffsetStepLength(System.Double start, int startOffset, System.Double step, int length)
		{
			return new RODoubleEquidistantElementVectorStartAtOffsetStepLength(start, startOffset, step, length);
		}


	  	/// <summary>
		/// Provides a read-only vector with equally spaced elements so that y[0] = start and y[length-1] = end.
		/// </summary>
		private class RODoubleEquidistantElementVectorStartEndLength : IReadOnlyList<System.Double>
		{
			private System.Double _start;
			private System.Double _end;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="end">Value of the last element of the vector.</param>
			/// <param name="length">Length of the vector.</param>
			public RODoubleEquidistantElementVectorStartEndLength(System.Double start, System.Double end, int length)
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
			public System.Double this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");

					double r = i / (double)(_length - 1);
					return (System.Double)(_start * (1 - r) + _end * (r));
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<System.Double> GetEnumerator()
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
		public static IReadOnlyList<System.Double> CreateEquidistantSequenceByStartEndLength(System.Double start, System.Double end, int length)
		{
			return new RODoubleEquidistantElementVectorStartEndLength(start, end, length);
		}


		/// <summary>
    /// Serves as wrapper for an RO Vector which returns the inverse of the elements of the original array.
    /// </summary>
    private class RODoubleInverseElementWrapper : IReadOnlyList<System.Double>
    {
      private int _length;
      protected IReadOnlyList<System.Double> _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODoubleInverseElementWrapper(IReadOnlyList<System.Double> x)
      {
        _x = x;
        _length = x.Count;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODoubleInverseElementWrapper(IReadOnlyList<System.Double> x, int usedlength)
      {
        if (usedlength > x.Count)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public System.Double this[int i]
      {
        get
        {
          return 1/(System.Double)_x[i];
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

      public IEnumerator<System.Double> GetEnumerator()
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
		/// Wraps a Double[] array to get an  <see cref="IReadOnlyList{Double}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Double}" /> returning elements that are inverse to those of the original vector.</returns>
		public static IReadOnlyList<System.Double> ToInverseROVector(this IReadOnlyList<System.Double> array)
		{
			return array is null ? null : new RODoubleInverseElementWrapper(array);
		}

		/// <summary>
		/// Wraps a Double[] array till a given length to get an <see cref="IReadOnlyList{Double}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Double}" /> interface that wraps the provided array returning elements that are inverse to those of the original vector..</returns>
		public static IReadOnlyList<System.Double> ToInverseROVector(this IReadOnlyList<System.Double> array, int usedlength)
		{
			return new RODoubleInverseElementWrapper(array, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{Double}" /> is neccessary.
    /// </summary>
    private class RODoubleArrayWrapper : IReadOnlyList<System.Double>
    {
      private int _length;
      protected System.Double[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODoubleArrayWrapper(System.Double[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODoubleArrayWrapper(System.Double[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public System.Double this[int i]
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

      public IEnumerator<System.Double> GetEnumerator()
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
		/// Wraps a Double[] array to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Double> ToROVector(this System.Double[] array)
		{
			return array is null ? null : new RODoubleArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a Double[] array till a given length to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Double> ToROVector(this System.Double[] array, int usedlength)
		{
			return new RODoubleArrayWrapper(array, usedlength);
		}

			/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{Double}" /> is neccessary.
    /// </summary>
    private class RODouble_DoubleArrayWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected System.Double[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODouble_DoubleArrayWrapper(System.Double[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODouble_DoubleArrayWrapper(System.Double[] x, int usedlength)
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
		/// Wraps a Double[] array to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this System.Double[] array)
		{
			return array is null ? null : new RODouble_DoubleArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a Double[] array till a given length to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this System.Double[] array, int usedlength)
		{
			return new RODouble_DoubleArrayWrapper(array, usedlength);
		}


		  /// <summary>
    /// Serves as wrapper for a section of an array to plug-in where an <see cref="IReadOnlyList{Double}" /> is neccessary.
    /// </summary>
    private class RODoubleArraySectionWrapper : IReadOnlyList<System.Double>
    {
      protected System.Double[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public RODoubleArraySectionWrapper(System.Double[] x)
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
      public RODoubleArraySectionWrapper(System.Double[] x, int start, int usedlength)
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
      public System.Double this[int i] { get { return _x[i + _start]; } }

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
      public IEnumerator<System.Double> GetEnumerator()
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
		/// Wraps an array to an <see cref="IReadOnlyList{Double}" />. Start and length of the used section of the array are specified in the parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of the element in <paramref name="array"/> used as the first element of the vector.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper object with the <see cref="IReadOnlyList{Double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Double> ToROVector(this System.Double[] array, int start, int usedlength)
		{
			if (0 == start)
				return new RODoubleArrayWrapper(array, usedlength);
			else
				return new RODoubleArraySectionWrapper(array, start, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{Double}" /> is neccessary.
    /// </summary>
    private class RODoubleArrayWrapperAmendedShifted : IReadOnlyList<System.Double>
    {
      private int _length;
      protected System.Double[] _x;
			protected System.Double _amendedValueAtStart;
			protected int _amendedValuesAtStartCount;
			protected System.Double _amendedValueAtEnd;
			protected int _amendedValuesAtEndCount;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public RODoubleArrayWrapperAmendedShifted(System.Double[] x, System.Double amendedValueAtStart, int amendedValuesAtStartCount, System.Double amendedValueAtEnd, int amendedValuesAtEndCount)
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

      public RODoubleArrayWrapperAmendedShifted(System.Double[] x, int usedlength, System.Double amendedValueAtStart, int amendedValuesAtStartCount, System.Double amendedValueAtEnd, int amendedValuesAtEndCount)
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
      public System.Double this[int i]
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

      public IEnumerator<System.Double> GetEnumerator()
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
		/// Wraps a Double[] array to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Double> ToROVectorAmendedShifted(this System.Double[] array,System.Double amendedValueAtStart, int amendedValuesAtStartCount, System.Double amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return array is null ? null : new RODoubleArrayWrapperAmendedShifted(array, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}

		/// <summary>
		/// Wraps a Double[] array till a given length to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Double}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Double> ToROVectorAmendedShifted(this System.Double[] array, int usedlength,System.Double amendedValueAtStart, int amendedValuesAtStartCount, System.Double amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return new RODoubleArrayWrapperAmendedShifted(array, usedlength, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}


		/// <summary>
    /// Serves as thin wrapper struct for an array when additional data at the start and the end of the array are neccessary.
    /// </summary>
    public struct RODoubleArrayWrapperStructAmendedUnshifted : IReadOnlyList<System.Double>
    {
      private int _length;
      private  System.Double[] _x;
			private  System.Double _amendedValueAtStart;
			private  System.Double _amendedValueAtEnd;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
      public RODoubleArrayWrapperStructAmendedUnshifted(System.Double[] x, System.Double amendedValueAtStart, System.Double amendedValueAtEnd)
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
	      public RODoubleArrayWrapperStructAmendedUnshifted(System.Double[] x, int usedlength, System.Double amendedValueAtStart, System.Double amendedValueAtEnd)
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
      public System.Double this[int i]
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

      public IEnumerator<System.Double> GetEnumerator()
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
		/// Wraps a Double[] array to get a struct with an <see cref="IReadOnlyList{Double}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above Length, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater than or equal to <paramref name="array"/>.Length.</param>.
		/// <returns>A wrapper struct with the <see cref="IReadOnlyList{Double}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static RODoubleArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this System.Double[] array,System.Double amendedValueAtStart, System.Double amendedValueAtEnd)
		{
			return new RODoubleArrayWrapperStructAmendedUnshifted(array, amendedValueAtStart, amendedValueAtEnd);
		}

		/// <summary>
		/// Wraps a Double[] array till a given length to get a struct with an <see cref="IReadOnlyList{Double}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above <paramref name="usedlength"/>, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to <paramref name="usedlength"/>.</param>.
		/// <returns>A wrapper struct with the <see cref="IReadOnlyList{Double}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static RODoubleArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this System.Double[] array, int usedlength,System.Double amendedValueAtStart, System.Double amendedValueAtEnd)
		{
			return new RODoubleArrayWrapperStructAmendedUnshifted(array, usedlength, amendedValueAtStart, amendedValueAtEnd );
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where a IVector is neccessary.
    /// </summary>
		private class RWDoubleArrayWrapper : RODoubleArrayWrapper, IVector<System.Double>
    {
      public RWDoubleArrayWrapper(System.Double[] x)
        : base(x)
      {
      }

      public RWDoubleArrayWrapper(System.Double[] x, int usedlength)
        : base(x, usedlength)
      {
      }

      public new System.Double this[int i]
      {
        get { return _x[i]; }
        set { _x[i] = value; }
      }
    }


			/// <summary>
		/// Wraps an array to get an <see cref="IVector{Double}" />
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Double}" /> interface that wraps the provided array.</returns>
		public static IVector<System.Double> ToVector(this System.Double[] array)
		{
			return new RWDoubleArrayWrapper(array);
		}

		/// <summary>
		/// Wraps an array to get an <see cref="IVector{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Used length of the array to get the wrapped vector (i.e. the vector wraps around <paramref name="array"/>[0..usedLength-1]).</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Double}" /> interface that wraps the provided array.</returns>
		public static IVector<System.Double> ToVector(System.Double[] array, int usedlength)
		{
			return new RWDoubleArrayWrapper(array, usedlength);
		}

		
		private class RWDoubleArraySectionWrapper : RODoubleArraySectionWrapper, IVector<System.Double>
    {
      public RWDoubleArraySectionWrapper(System.Double[] x)
        : base(x)
      {
      }

      public RWDoubleArraySectionWrapper(System.Double[] x, int start, int usedlength)
        : base(x, start, usedlength)
      {
      }

      public new System.Double this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }
    }

		/// <summary>
		/// Wraps a section of an array to get a <see cref="IVector{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of first element of <paramref name="array"/> to use.</param>
		/// <param name="count">Number of elements of <paramref name="array"/> to use.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Double}" /> interface that wraps a section of the provided array.</returns>
		public static IVector<System.Double> ToVector(this System.Double[] array, int start, int count)
		{
			if (0 == start)
				return new RWDoubleArrayWrapper(array, count);
			else
				return new RWDoubleArraySectionWrapper(array, start, count);
		}



		 /// <summary>
    /// Serves as wrapper for an <see cref="IReadOnlyList{Double}" /> to get only a section of the original wrapper.
    /// </summary>
    private class RODoubleVectorSectionWrapper : IReadOnlyList<System.Double>
    {
      protected IReadOnlyList<System.Double> _x;
      private int _start;
      private int _length;


		 /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The vector to wrap.</param>
      /// <param name="start">Start index of the section of the vector to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RODoubleVectorSectionWrapper(IReadOnlyList<System.Double> x, int start, int len)
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
      public System.Double this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<System.Double> GetEnumerator()
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
		/// <returns>An <see cref="IReadOnlyList{Double}" /> that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IReadOnlyList<System.Double> ToROVector(this IReadOnlyList<System.Double> vector, int start, int usedLength)
		{
			return new RODoubleVectorSectionWrapper(vector, start, usedLength);
		}

		  /// <summary>
    /// Serves as wrapper for an IVector to get only a section of the original wrapper.
    /// </summary>
    private class RWDoubleVectorSectionWrapper : IVector<System.Double>
    {
      protected IVector<System.Double> _x;
      private int _start;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Start index of the section to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RWDoubleVectorSectionWrapper(IVector<System.Double> x, int start, int len)
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
      public System.Double this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<System.Double> GetEnumerator()
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
		public static IVector<System.Double> ToVector(this IVector<System.Double> vector, int start, int len)
		{
			return new RWDoubleVectorSectionWrapper(vector, start, len);
		}

    /// <summary>
	/// Returns a clone of the source vector.
	/// </summary>
	/// <param name="sourceVector">The source vector.</param>
    /// <returns>A freshly allocated clone of the sourceVector, with the same element values as the source vector.</returns>
	public static System.Double[] Clone(System.Double[] sourceVector)
	{
		if (sourceVector is null)
			throw new ArgumentNullException(nameof(sourceVector));

        var destVector = new System.Double[sourceVector.Length];
        Array.Copy(sourceVector, destVector, sourceVector.Length);
        return destVector;
	}


#pragma warning restore CS3002 // enable CS3002 again
#pragma warning restore CS3003 // enable CS3003 again
#pragma warning disable CS3002 // Disable not CLS-compliant warning for generated code
#pragma warning disable CS3003 // Disable not CLS-compliant warning for generated code
// ******************************************** Definitions for System.Single *******************************************

    /// <summary>
		/// Provides a read-only vector with equal and constant items.
		/// </summary>
		private class ROFloatConstantVector : IReadOnlyList<System.Single>
		{
			private int _length;
			private System.Single _value;

			public ROFloatConstantVector(System.Single value, int length)
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

      public System.Single this[int i]
			{
				get { return _value; }
			}

      public IEnumerator<System.Single> GetEnumerator()
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
		public static IReadOnlyList<System.Single> GetConstantVector(System.Single value, int length)
		{
			return new ROFloatConstantVector(value, length);
		}


		/// <summary>
    /// Provides a read-only vector with equally spaced elements y[i] = start + i * increment.
    /// </summary>
    private class ROFloatEquidistantElementVector : IReadOnlyList<System.Single>
    {
      private int _length;
      private System.Single _startValue;
      private System.Single _incrementValue;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
      public ROFloatEquidistantElementVector(System.Single start, System.Single increment, int length)
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
      public System.Single this[int i]
      {
        get { return (System.Single)(_startValue + i * _incrementValue); }
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      public IEnumerator<System.Single> GetEnumerator()
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
		public static IReadOnlyList<System.Single> CreateEquidistantSequenceByStartStepLength(System.Single start, System.Single step, int length)
		{
			return new ROFloatEquidistantElementVector(start, step, length);
		}

			
			/// <summary>
		/// Provides a read-only vector with equally spaced elements y[i] = start + (i-startOffset) * increment.
		/// </summary>
		private class ROFloatEquidistantElementVectorStartAtOffsetStepLength : IReadOnlyList<System.Single>
		{
			private System.Single _start;
			private int _startOffset;

			private System.Single _increment;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the element at index <paramref name="startOffset"/> of the vector.</param>
			/// <param name="startOffset">The index of the element for which a value is provided in <paramref name="start"/>.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
			public ROFloatEquidistantElementVectorStartAtOffsetStepLength(System.Single start, int startOffset, System.Single increment, int length)
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
			public System.Single this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");
					return (System.Single)(_start + (i - _startOffset) * _increment);
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<System.Single> GetEnumerator()
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
		public static IReadOnlyList<System.Single> CreateEquidistantSequencyByStartAtOffsetStepLength(System.Single start, int startOffset, System.Single step, int length)
		{
			return new ROFloatEquidistantElementVectorStartAtOffsetStepLength(start, startOffset, step, length);
		}


	  	/// <summary>
		/// Provides a read-only vector with equally spaced elements so that y[0] = start and y[length-1] = end.
		/// </summary>
		private class ROFloatEquidistantElementVectorStartEndLength : IReadOnlyList<System.Single>
		{
			private System.Single _start;
			private System.Single _end;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="end">Value of the last element of the vector.</param>
			/// <param name="length">Length of the vector.</param>
			public ROFloatEquidistantElementVectorStartEndLength(System.Single start, System.Single end, int length)
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
			public System.Single this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");

					double r = i / (double)(_length - 1);
					return (System.Single)(_start * (1 - r) + _end * (r));
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<System.Single> GetEnumerator()
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
		public static IReadOnlyList<System.Single> CreateEquidistantSequenceByStartEndLength(System.Single start, System.Single end, int length)
		{
			return new ROFloatEquidistantElementVectorStartEndLength(start, end, length);
		}


		/// <summary>
    /// Serves as wrapper for an RO Vector which returns the inverse of the elements of the original array.
    /// </summary>
    private class ROFloatInverseElementWrapper : IReadOnlyList<System.Single>
    {
      private int _length;
      protected IReadOnlyList<System.Single> _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROFloatInverseElementWrapper(IReadOnlyList<System.Single> x)
      {
        _x = x;
        _length = x.Count;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROFloatInverseElementWrapper(IReadOnlyList<System.Single> x, int usedlength)
      {
        if (usedlength > x.Count)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public System.Single this[int i]
      {
        get
        {
          return 1/(System.Single)_x[i];
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

      public IEnumerator<System.Single> GetEnumerator()
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
		/// Wraps a Single[] array to get an  <see cref="IReadOnlyList{Single}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Single}" /> returning elements that are inverse to those of the original vector.</returns>
		public static IReadOnlyList<System.Single> ToInverseROVector(this IReadOnlyList<System.Single> array)
		{
			return array is null ? null : new ROFloatInverseElementWrapper(array);
		}

		/// <summary>
		/// Wraps a Single[] array till a given length to get an <see cref="IReadOnlyList{Single}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Single}" /> interface that wraps the provided array returning elements that are inverse to those of the original vector..</returns>
		public static IReadOnlyList<System.Single> ToInverseROVector(this IReadOnlyList<System.Single> array, int usedlength)
		{
			return new ROFloatInverseElementWrapper(array, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{Single}" /> is neccessary.
    /// </summary>
    private class ROFloatArrayWrapper : IReadOnlyList<System.Single>
    {
      private int _length;
      protected System.Single[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROFloatArrayWrapper(System.Single[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROFloatArrayWrapper(System.Single[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public System.Single this[int i]
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

      public IEnumerator<System.Single> GetEnumerator()
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
		/// Wraps a Single[] array to get an <see cref="IReadOnlyList{Single}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Single}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Single> ToROVector(this System.Single[] array)
		{
			return array is null ? null : new ROFloatArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a Single[] array till a given length to get an <see cref="IReadOnlyList{Single}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Single}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Single> ToROVector(this System.Single[] array, int usedlength)
		{
			return new ROFloatArrayWrapper(array, usedlength);
		}

			/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{Single}" /> is neccessary.
    /// </summary>
    private class RODouble_FloatArrayWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected System.Single[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODouble_FloatArrayWrapper(System.Single[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODouble_FloatArrayWrapper(System.Single[] x, int usedlength)
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
		/// Wraps a Single[] array to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Single}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this System.Single[] array)
		{
			return array is null ? null : new RODouble_FloatArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a Single[] array till a given length to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Single}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this System.Single[] array, int usedlength)
		{
			return new RODouble_FloatArrayWrapper(array, usedlength);
		}


		  /// <summary>
    /// Serves as wrapper for a section of an array to plug-in where an <see cref="IReadOnlyList{Single}" /> is neccessary.
    /// </summary>
    private class ROFloatArraySectionWrapper : IReadOnlyList<System.Single>
    {
      protected System.Single[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public ROFloatArraySectionWrapper(System.Single[] x)
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
      public ROFloatArraySectionWrapper(System.Single[] x, int start, int usedlength)
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
      public System.Single this[int i] { get { return _x[i + _start]; } }

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
      public IEnumerator<System.Single> GetEnumerator()
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
		/// Wraps an array to an <see cref="IReadOnlyList{Single}" />. Start and length of the used section of the array are specified in the parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of the element in <paramref name="array"/> used as the first element of the vector.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper object with the <see cref="IReadOnlyList{Single}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Single> ToROVector(this System.Single[] array, int start, int usedlength)
		{
			if (0 == start)
				return new ROFloatArrayWrapper(array, usedlength);
			else
				return new ROFloatArraySectionWrapper(array, start, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{Single}" /> is neccessary.
    /// </summary>
    private class ROFloatArrayWrapperAmendedShifted : IReadOnlyList<System.Single>
    {
      private int _length;
      protected System.Single[] _x;
			protected System.Single _amendedValueAtStart;
			protected int _amendedValuesAtStartCount;
			protected System.Single _amendedValueAtEnd;
			protected int _amendedValuesAtEndCount;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public ROFloatArrayWrapperAmendedShifted(System.Single[] x, System.Single amendedValueAtStart, int amendedValuesAtStartCount, System.Single amendedValueAtEnd, int amendedValuesAtEndCount)
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

      public ROFloatArrayWrapperAmendedShifted(System.Single[] x, int usedlength, System.Single amendedValueAtStart, int amendedValuesAtStartCount, System.Single amendedValueAtEnd, int amendedValuesAtEndCount)
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
      public System.Single this[int i]
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

      public IEnumerator<System.Single> GetEnumerator()
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
		/// Wraps a Single[] array to get an <see cref="IReadOnlyList{Single}" />.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Single}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Single> ToROVectorAmendedShifted(this System.Single[] array,System.Single amendedValueAtStart, int amendedValuesAtStartCount, System.Single amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return array is null ? null : new ROFloatArrayWrapperAmendedShifted(array, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}

		/// <summary>
		/// Wraps a Single[] array till a given length to get an <see cref="IReadOnlyList{Single}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Single}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Single> ToROVectorAmendedShifted(this System.Single[] array, int usedlength,System.Single amendedValueAtStart, int amendedValuesAtStartCount, System.Single amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return new ROFloatArrayWrapperAmendedShifted(array, usedlength, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}


		/// <summary>
    /// Serves as thin wrapper struct for an array when additional data at the start and the end of the array are neccessary.
    /// </summary>
    public struct ROFloatArrayWrapperStructAmendedUnshifted : IReadOnlyList<System.Single>
    {
      private int _length;
      private  System.Single[] _x;
			private  System.Single _amendedValueAtStart;
			private  System.Single _amendedValueAtEnd;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
      public ROFloatArrayWrapperStructAmendedUnshifted(System.Single[] x, System.Single amendedValueAtStart, System.Single amendedValueAtEnd)
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
	      public ROFloatArrayWrapperStructAmendedUnshifted(System.Single[] x, int usedlength, System.Single amendedValueAtStart, System.Single amendedValueAtEnd)
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
      public System.Single this[int i]
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

      public IEnumerator<System.Single> GetEnumerator()
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
		/// Wraps a Single[] array to get a struct with an <see cref="IReadOnlyList{Single}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above Length, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater than or equal to <paramref name="array"/>.Length.</param>.
		/// <returns>A wrapper struct with the <see cref="IReadOnlyList{Single}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROFloatArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this System.Single[] array,System.Single amendedValueAtStart, System.Single amendedValueAtEnd)
		{
			return new ROFloatArrayWrapperStructAmendedUnshifted(array, amendedValueAtStart, amendedValueAtEnd);
		}

		/// <summary>
		/// Wraps a Single[] array till a given length to get a struct with an <see cref="IReadOnlyList{Single}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above <paramref name="usedlength"/>, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to <paramref name="usedlength"/>.</param>.
		/// <returns>A wrapper struct with the <see cref="IReadOnlyList{Single}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROFloatArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this System.Single[] array, int usedlength,System.Single amendedValueAtStart, System.Single amendedValueAtEnd)
		{
			return new ROFloatArrayWrapperStructAmendedUnshifted(array, usedlength, amendedValueAtStart, amendedValueAtEnd );
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where a IVector is neccessary.
    /// </summary>
		private class RWFloatArrayWrapper : ROFloatArrayWrapper, IVector<System.Single>
    {
      public RWFloatArrayWrapper(System.Single[] x)
        : base(x)
      {
      }

      public RWFloatArrayWrapper(System.Single[] x, int usedlength)
        : base(x, usedlength)
      {
      }

      public new System.Single this[int i]
      {
        get { return _x[i]; }
        set { _x[i] = value; }
      }
    }


			/// <summary>
		/// Wraps an array to get an <see cref="IVector{Single}" />
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Single}" /> interface that wraps the provided array.</returns>
		public static IVector<System.Single> ToVector(this System.Single[] array)
		{
			return new RWFloatArrayWrapper(array);
		}

		/// <summary>
		/// Wraps an array to get an <see cref="IVector{Single}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Used length of the array to get the wrapped vector (i.e. the vector wraps around <paramref name="array"/>[0..usedLength-1]).</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Single}" /> interface that wraps the provided array.</returns>
		public static IVector<System.Single> ToVector(System.Single[] array, int usedlength)
		{
			return new RWFloatArrayWrapper(array, usedlength);
		}

		
		private class RWFloatArraySectionWrapper : ROFloatArraySectionWrapper, IVector<System.Single>
    {
      public RWFloatArraySectionWrapper(System.Single[] x)
        : base(x)
      {
      }

      public RWFloatArraySectionWrapper(System.Single[] x, int start, int usedlength)
        : base(x, start, usedlength)
      {
      }

      public new System.Single this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }
    }

		/// <summary>
		/// Wraps a section of an array to get a <see cref="IVector{Single}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of first element of <paramref name="array"/> to use.</param>
		/// <param name="count">Number of elements of <paramref name="array"/> to use.</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Single}" /> interface that wraps a section of the provided array.</returns>
		public static IVector<System.Single> ToVector(this System.Single[] array, int start, int count)
		{
			if (0 == start)
				return new RWFloatArrayWrapper(array, count);
			else
				return new RWFloatArraySectionWrapper(array, start, count);
		}



		 /// <summary>
    /// Serves as wrapper for an <see cref="IReadOnlyList{Single}" /> to get only a section of the original wrapper.
    /// </summary>
    private class ROFloatVectorSectionWrapper : IReadOnlyList<System.Single>
    {
      protected IReadOnlyList<System.Single> _x;
      private int _start;
      private int _length;


		 /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The vector to wrap.</param>
      /// <param name="start">Start index of the section of the vector to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public ROFloatVectorSectionWrapper(IReadOnlyList<System.Single> x, int start, int len)
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
      public System.Single this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<System.Single> GetEnumerator()
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
		/// <returns>An <see cref="IReadOnlyList{Single}" /> that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IReadOnlyList<System.Single> ToROVector(this IReadOnlyList<System.Single> vector, int start, int usedLength)
		{
			return new ROFloatVectorSectionWrapper(vector, start, usedLength);
		}

		  /// <summary>
    /// Serves as wrapper for an IVector to get only a section of the original wrapper.
    /// </summary>
    private class RWFloatVectorSectionWrapper : IVector<System.Single>
    {
      protected IVector<System.Single> _x;
      private int _start;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Start index of the section to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RWFloatVectorSectionWrapper(IVector<System.Single> x, int start, int len)
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
      public System.Single this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<System.Single> GetEnumerator()
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
		public static IVector<System.Single> ToVector(this IVector<System.Single> vector, int start, int len)
		{
			return new RWFloatVectorSectionWrapper(vector, start, len);
		}

    /// <summary>
	/// Returns a clone of the source vector.
	/// </summary>
	/// <param name="sourceVector">The source vector.</param>
    /// <returns>A freshly allocated clone of the sourceVector, with the same element values as the source vector.</returns>
	public static System.Single[] Clone(System.Single[] sourceVector)
	{
		if (sourceVector is null)
			throw new ArgumentNullException(nameof(sourceVector));

        var destVector = new System.Single[sourceVector.Length];
        Array.Copy(sourceVector, destVector, sourceVector.Length);
        return destVector;
	}


#pragma warning restore CS3002 // enable CS3002 again
#pragma warning restore CS3003 // enable CS3003 again
#pragma warning disable CS3002 // Disable not CLS-compliant warning for generated code
#pragma warning disable CS3003 // Disable not CLS-compliant warning for generated code
// ******************************************** Definitions for System.Int32 *******************************************

    /// <summary>
		/// Provides a read-only vector with equal and constant items.
		/// </summary>
		private class ROIntConstantVector : IReadOnlyList<System.Int32>
		{
			private int _length;
			private System.Int32 _value;

			public ROIntConstantVector(System.Int32 value, int length)
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

      public System.Int32 this[int i]
			{
				get { return _value; }
			}

      public IEnumerator<System.Int32> GetEnumerator()
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
		public static IReadOnlyList<System.Int32> GetConstantVector(System.Int32 value, int length)
		{
			return new ROIntConstantVector(value, length);
		}


		/// <summary>
    /// Provides a read-only vector with equally spaced elements y[i] = start + i * increment.
    /// </summary>
    private class ROIntEquidistantElementVector : IReadOnlyList<System.Int32>
    {
      private int _length;
      private System.Int32 _startValue;
      private System.Int32 _incrementValue;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
      public ROIntEquidistantElementVector(System.Int32 start, System.Int32 increment, int length)
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
      public System.Int32 this[int i]
      {
        get { return (System.Int32)(_startValue + i * _incrementValue); }
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      public IEnumerator<System.Int32> GetEnumerator()
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
		public static IReadOnlyList<System.Int32> CreateEquidistantSequenceByStartStepLength(System.Int32 start, System.Int32 step, int length)
		{
			return new ROIntEquidistantElementVector(start, step, length);
		}

			
			/// <summary>
		/// Provides a read-only vector with equally spaced elements y[i] = start + (i-startOffset) * increment.
		/// </summary>
		private class ROIntEquidistantElementVectorStartAtOffsetStepLength : IReadOnlyList<System.Int32>
		{
			private System.Int32 _start;
			private int _startOffset;

			private System.Int32 _increment;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the element at index <paramref name="startOffset"/> of the vector.</param>
			/// <param name="startOffset">The index of the element for which a value is provided in <paramref name="start"/>.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
			public ROIntEquidistantElementVectorStartAtOffsetStepLength(System.Int32 start, int startOffset, System.Int32 increment, int length)
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
			public System.Int32 this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");
					return (System.Int32)(_start + (i - _startOffset) * _increment);
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<System.Int32> GetEnumerator()
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
		public static IReadOnlyList<System.Int32> CreateEquidistantSequencyByStartAtOffsetStepLength(System.Int32 start, int startOffset, System.Int32 step, int length)
		{
			return new ROIntEquidistantElementVectorStartAtOffsetStepLength(start, startOffset, step, length);
		}


	  	/// <summary>
		/// Provides a read-only vector with equally spaced elements so that y[0] = start and y[length-1] = end.
		/// </summary>
		private class ROIntEquidistantElementVectorStartEndLength : IReadOnlyList<System.Int32>
		{
			private System.Int32 _start;
			private System.Int32 _end;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="end">Value of the last element of the vector.</param>
			/// <param name="length">Length of the vector.</param>
			public ROIntEquidistantElementVectorStartEndLength(System.Int32 start, System.Int32 end, int length)
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
			public System.Int32 this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");

					double r = i / (double)(_length - 1);
					return (System.Int32)(_start * (1 - r) + _end * (r));
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<System.Int32> GetEnumerator()
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
		public static IReadOnlyList<System.Int32> CreateEquidistantSequenceByStartEndLength(System.Int32 start, System.Int32 end, int length)
		{
			return new ROIntEquidistantElementVectorStartEndLength(start, end, length);
		}


		/// <summary>
    /// Serves as wrapper for an RO Vector which returns the inverse of the elements of the original array.
    /// </summary>
    private class ROIntInverseElementWrapper : IReadOnlyList<System.Double>
    {
      private int _length;
      protected IReadOnlyList<System.Int32> _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROIntInverseElementWrapper(IReadOnlyList<System.Int32> x)
      {
        _x = x;
        _length = x.Count;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROIntInverseElementWrapper(IReadOnlyList<System.Int32> x, int usedlength)
      {
        if (usedlength > x.Count)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public System.Double this[int i]
      {
        get
        {
          return 1/(System.Double)_x[i];
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

      public IEnumerator<System.Double> GetEnumerator()
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
		/// Wraps a Int32[] array to get an  <see cref="IReadOnlyList{Int32}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int32}" /> returning elements that are inverse to those of the original vector.</returns>
		public static IReadOnlyList<System.Double> ToInverseROVector(this IReadOnlyList<System.Int32> array)
		{
			return array is null ? null : new ROIntInverseElementWrapper(array);
		}

		/// <summary>
		/// Wraps a Int32[] array till a given length to get an <see cref="IReadOnlyList{Int32}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int32}" /> interface that wraps the provided array returning elements that are inverse to those of the original vector..</returns>
		public static IReadOnlyList<System.Double> ToInverseROVector(this IReadOnlyList<System.Int32> array, int usedlength)
		{
			return new ROIntInverseElementWrapper(array, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{Int32}" /> is neccessary.
    /// </summary>
    private class ROIntArrayWrapper : IReadOnlyList<System.Int32>
    {
      private int _length;
      protected System.Int32[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROIntArrayWrapper(System.Int32[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROIntArrayWrapper(System.Int32[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public System.Int32 this[int i]
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

      public IEnumerator<System.Int32> GetEnumerator()
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
		/// Wraps a Int32[] array to get an <see cref="IReadOnlyList{Int32}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Int32> ToROVector(this System.Int32[] array)
		{
			return array is null ? null : new ROIntArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a Int32[] array till a given length to get an <see cref="IReadOnlyList{Int32}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Int32> ToROVector(this System.Int32[] array, int usedlength)
		{
			return new ROIntArrayWrapper(array, usedlength);
		}

			/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{Int32}" /> is neccessary.
    /// </summary>
    private class RODouble_IntArrayWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected System.Int32[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODouble_IntArrayWrapper(System.Int32[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODouble_IntArrayWrapper(System.Int32[] x, int usedlength)
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
		/// Wraps a Int32[] array to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this System.Int32[] array)
		{
			return array is null ? null : new RODouble_IntArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a Int32[] array till a given length to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this System.Int32[] array, int usedlength)
		{
			return new RODouble_IntArrayWrapper(array, usedlength);
		}


		  /// <summary>
    /// Serves as wrapper for a section of an array to plug-in where an <see cref="IReadOnlyList{Int32}" /> is neccessary.
    /// </summary>
    private class ROIntArraySectionWrapper : IReadOnlyList<System.Int32>
    {
      protected System.Int32[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public ROIntArraySectionWrapper(System.Int32[] x)
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
      public ROIntArraySectionWrapper(System.Int32[] x, int start, int usedlength)
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
      public System.Int32 this[int i] { get { return _x[i + _start]; } }

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
      public IEnumerator<System.Int32> GetEnumerator()
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
		/// Wraps an array to an <see cref="IReadOnlyList{Int32}" />. Start and length of the used section of the array are specified in the parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of the element in <paramref name="array"/> used as the first element of the vector.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper object with the <see cref="IReadOnlyList{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Int32> ToROVector(this System.Int32[] array, int start, int usedlength)
		{
			if (0 == start)
				return new ROIntArrayWrapper(array, usedlength);
			else
				return new ROIntArraySectionWrapper(array, start, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{Int32}" /> is neccessary.
    /// </summary>
    private class ROIntArrayWrapperAmendedShifted : IReadOnlyList<System.Int32>
    {
      private int _length;
      protected System.Int32[] _x;
			protected System.Int32 _amendedValueAtStart;
			protected int _amendedValuesAtStartCount;
			protected System.Int32 _amendedValueAtEnd;
			protected int _amendedValuesAtEndCount;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public ROIntArrayWrapperAmendedShifted(System.Int32[] x, System.Int32 amendedValueAtStart, int amendedValuesAtStartCount, System.Int32 amendedValueAtEnd, int amendedValuesAtEndCount)
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

      public ROIntArrayWrapperAmendedShifted(System.Int32[] x, int usedlength, System.Int32 amendedValueAtStart, int amendedValuesAtStartCount, System.Int32 amendedValueAtEnd, int amendedValuesAtEndCount)
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
      public System.Int32 this[int i]
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

      public IEnumerator<System.Int32> GetEnumerator()
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
		/// Wraps a Int32[] array to get an <see cref="IReadOnlyList{Int32}" />.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Int32> ToROVectorAmendedShifted(this System.Int32[] array,System.Int32 amendedValueAtStart, int amendedValuesAtStartCount, System.Int32 amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return array is null ? null : new ROIntArrayWrapperAmendedShifted(array, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}

		/// <summary>
		/// Wraps a Int32[] array till a given length to get an <see cref="IReadOnlyList{Int32}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int32}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Int32> ToROVectorAmendedShifted(this System.Int32[] array, int usedlength,System.Int32 amendedValueAtStart, int amendedValuesAtStartCount, System.Int32 amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return new ROIntArrayWrapperAmendedShifted(array, usedlength, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}


		/// <summary>
    /// Serves as thin wrapper struct for an array when additional data at the start and the end of the array are neccessary.
    /// </summary>
    public struct ROIntArrayWrapperStructAmendedUnshifted : IReadOnlyList<System.Int32>
    {
      private int _length;
      private  System.Int32[] _x;
			private  System.Int32 _amendedValueAtStart;
			private  System.Int32 _amendedValueAtEnd;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
      public ROIntArrayWrapperStructAmendedUnshifted(System.Int32[] x, System.Int32 amendedValueAtStart, System.Int32 amendedValueAtEnd)
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
	      public ROIntArrayWrapperStructAmendedUnshifted(System.Int32[] x, int usedlength, System.Int32 amendedValueAtStart, System.Int32 amendedValueAtEnd)
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
      public System.Int32 this[int i]
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

      public IEnumerator<System.Int32> GetEnumerator()
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
		/// Wraps a Int32[] array to get a struct with an <see cref="IReadOnlyList{Int32}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above Length, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater than or equal to <paramref name="array"/>.Length.</param>.
		/// <returns>A wrapper struct with the <see cref="IReadOnlyList{Int32}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROIntArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this System.Int32[] array,System.Int32 amendedValueAtStart, System.Int32 amendedValueAtEnd)
		{
			return new ROIntArrayWrapperStructAmendedUnshifted(array, amendedValueAtStart, amendedValueAtEnd);
		}

		/// <summary>
		/// Wraps a Int32[] array till a given length to get a struct with an <see cref="IReadOnlyList{Int32}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above <paramref name="usedlength"/>, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to <paramref name="usedlength"/>.</param>.
		/// <returns>A wrapper struct with the <see cref="IReadOnlyList{Int32}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROIntArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this System.Int32[] array, int usedlength,System.Int32 amendedValueAtStart, System.Int32 amendedValueAtEnd)
		{
			return new ROIntArrayWrapperStructAmendedUnshifted(array, usedlength, amendedValueAtStart, amendedValueAtEnd );
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where a IVector is neccessary.
    /// </summary>
		private class RWIntArrayWrapper : ROIntArrayWrapper, IVector<System.Int32>
    {
      public RWIntArrayWrapper(System.Int32[] x)
        : base(x)
      {
      }

      public RWIntArrayWrapper(System.Int32[] x, int usedlength)
        : base(x, usedlength)
      {
      }

      public new System.Int32 this[int i]
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
		public static IVector<System.Int32> ToVector(this System.Int32[] array)
		{
			return new RWIntArrayWrapper(array);
		}

		/// <summary>
		/// Wraps an array to get an <see cref="IVector{Int32}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Used length of the array to get the wrapped vector (i.e. the vector wraps around <paramref name="array"/>[0..usedLength-1]).</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Int32}" /> interface that wraps the provided array.</returns>
		public static IVector<System.Int32> ToVector(System.Int32[] array, int usedlength)
		{
			return new RWIntArrayWrapper(array, usedlength);
		}

		
		private class RWIntArraySectionWrapper : ROIntArraySectionWrapper, IVector<System.Int32>
    {
      public RWIntArraySectionWrapper(System.Int32[] x)
        : base(x)
      {
      }

      public RWIntArraySectionWrapper(System.Int32[] x, int start, int usedlength)
        : base(x, start, usedlength)
      {
      }

      public new System.Int32 this[int i]
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
		public static IVector<System.Int32> ToVector(this System.Int32[] array, int start, int count)
		{
			if (0 == start)
				return new RWIntArrayWrapper(array, count);
			else
				return new RWIntArraySectionWrapper(array, start, count);
		}



		 /// <summary>
    /// Serves as wrapper for an <see cref="IReadOnlyList{Int32}" /> to get only a section of the original wrapper.
    /// </summary>
    private class ROIntVectorSectionWrapper : IReadOnlyList<System.Int32>
    {
      protected IReadOnlyList<System.Int32> _x;
      private int _start;
      private int _length;


		 /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The vector to wrap.</param>
      /// <param name="start">Start index of the section of the vector to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public ROIntVectorSectionWrapper(IReadOnlyList<System.Int32> x, int start, int len)
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
      public System.Int32 this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<System.Int32> GetEnumerator()
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
		/// <returns>An <see cref="IReadOnlyList{Int32}" /> that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IReadOnlyList<System.Int32> ToROVector(this IReadOnlyList<System.Int32> vector, int start, int usedLength)
		{
			return new ROIntVectorSectionWrapper(vector, start, usedLength);
		}

		  /// <summary>
    /// Serves as wrapper for an IVector to get only a section of the original wrapper.
    /// </summary>
    private class RWIntVectorSectionWrapper : IVector<System.Int32>
    {
      protected IVector<System.Int32> _x;
      private int _start;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Start index of the section to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RWIntVectorSectionWrapper(IVector<System.Int32> x, int start, int len)
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
      public System.Int32 this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<System.Int32> GetEnumerator()
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
		public static IVector<System.Int32> ToVector(this IVector<System.Int32> vector, int start, int len)
		{
			return new RWIntVectorSectionWrapper(vector, start, len);
		}

    /// <summary>
	/// Returns a clone of the source vector.
	/// </summary>
	/// <param name="sourceVector">The source vector.</param>
    /// <returns>A freshly allocated clone of the sourceVector, with the same element values as the source vector.</returns>
	public static System.Int32[] Clone(System.Int32[] sourceVector)
	{
		if (sourceVector is null)
			throw new ArgumentNullException(nameof(sourceVector));

        var destVector = new System.Int32[sourceVector.Length];
        Array.Copy(sourceVector, destVector, sourceVector.Length);
        return destVector;
	}


#pragma warning restore CS3002 // enable CS3002 again
#pragma warning restore CS3003 // enable CS3003 again
#pragma warning disable CS3002 // Disable not CLS-compliant warning for generated code
#pragma warning disable CS3003 // Disable not CLS-compliant warning for generated code
// ******************************************** Definitions for System.Int16 *******************************************

    /// <summary>
		/// Provides a read-only vector with equal and constant items.
		/// </summary>
		private class ROShortConstantVector : IReadOnlyList<System.Int16>
		{
			private int _length;
			private System.Int16 _value;

			public ROShortConstantVector(System.Int16 value, int length)
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

      public System.Int16 this[int i]
			{
				get { return _value; }
			}

      public IEnumerator<System.Int16> GetEnumerator()
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
		public static IReadOnlyList<System.Int16> GetConstantVector(System.Int16 value, int length)
		{
			return new ROShortConstantVector(value, length);
		}


		/// <summary>
    /// Provides a read-only vector with equally spaced elements y[i] = start + i * increment.
    /// </summary>
    private class ROShortEquidistantElementVector : IReadOnlyList<System.Int16>
    {
      private int _length;
      private System.Int16 _startValue;
      private System.Int16 _incrementValue;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
      public ROShortEquidistantElementVector(System.Int16 start, System.Int16 increment, int length)
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
      public System.Int16 this[int i]
      {
        get { return (System.Int16)(_startValue + i * _incrementValue); }
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      public IEnumerator<System.Int16> GetEnumerator()
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
		public static IReadOnlyList<System.Int16> CreateEquidistantSequenceByStartStepLength(System.Int16 start, System.Int16 step, int length)
		{
			return new ROShortEquidistantElementVector(start, step, length);
		}

			
			/// <summary>
		/// Provides a read-only vector with equally spaced elements y[i] = start + (i-startOffset) * increment.
		/// </summary>
		private class ROShortEquidistantElementVectorStartAtOffsetStepLength : IReadOnlyList<System.Int16>
		{
			private System.Int16 _start;
			private int _startOffset;

			private System.Int16 _increment;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the element at index <paramref name="startOffset"/> of the vector.</param>
			/// <param name="startOffset">The index of the element for which a value is provided in <paramref name="start"/>.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
			public ROShortEquidistantElementVectorStartAtOffsetStepLength(System.Int16 start, int startOffset, System.Int16 increment, int length)
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
			public System.Int16 this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");
					return (System.Int16)(_start + (i - _startOffset) * _increment);
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<System.Int16> GetEnumerator()
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
		public static IReadOnlyList<System.Int16> CreateEquidistantSequencyByStartAtOffsetStepLength(System.Int16 start, int startOffset, System.Int16 step, int length)
		{
			return new ROShortEquidistantElementVectorStartAtOffsetStepLength(start, startOffset, step, length);
		}


	  	/// <summary>
		/// Provides a read-only vector with equally spaced elements so that y[0] = start and y[length-1] = end.
		/// </summary>
		private class ROShortEquidistantElementVectorStartEndLength : IReadOnlyList<System.Int16>
		{
			private System.Int16 _start;
			private System.Int16 _end;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="end">Value of the last element of the vector.</param>
			/// <param name="length">Length of the vector.</param>
			public ROShortEquidistantElementVectorStartEndLength(System.Int16 start, System.Int16 end, int length)
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
			public System.Int16 this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");

					double r = i / (double)(_length - 1);
					return (System.Int16)(_start * (1 - r) + _end * (r));
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<System.Int16> GetEnumerator()
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
		public static IReadOnlyList<System.Int16> CreateEquidistantSequenceByStartEndLength(System.Int16 start, System.Int16 end, int length)
		{
			return new ROShortEquidistantElementVectorStartEndLength(start, end, length);
		}


		/// <summary>
    /// Serves as wrapper for an RO Vector which returns the inverse of the elements of the original array.
    /// </summary>
    private class ROShortInverseElementWrapper : IReadOnlyList<System.Double>
    {
      private int _length;
      protected IReadOnlyList<System.Int16> _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROShortInverseElementWrapper(IReadOnlyList<System.Int16> x)
      {
        _x = x;
        _length = x.Count;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROShortInverseElementWrapper(IReadOnlyList<System.Int16> x, int usedlength)
      {
        if (usedlength > x.Count)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public System.Double this[int i]
      {
        get
        {
          return 1/(System.Double)_x[i];
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

      public IEnumerator<System.Double> GetEnumerator()
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
		/// Wraps a Int16[] array to get an  <see cref="IReadOnlyList{Int16}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int16}" /> returning elements that are inverse to those of the original vector.</returns>
		public static IReadOnlyList<System.Double> ToInverseROVector(this IReadOnlyList<System.Int16> array)
		{
			return array is null ? null : new ROShortInverseElementWrapper(array);
		}

		/// <summary>
		/// Wraps a Int16[] array till a given length to get an <see cref="IReadOnlyList{Int16}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int16}" /> interface that wraps the provided array returning elements that are inverse to those of the original vector..</returns>
		public static IReadOnlyList<System.Double> ToInverseROVector(this IReadOnlyList<System.Int16> array, int usedlength)
		{
			return new ROShortInverseElementWrapper(array, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{Int16}" /> is neccessary.
    /// </summary>
    private class ROShortArrayWrapper : IReadOnlyList<System.Int16>
    {
      private int _length;
      protected System.Int16[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROShortArrayWrapper(System.Int16[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROShortArrayWrapper(System.Int16[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public System.Int16 this[int i]
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

      public IEnumerator<System.Int16> GetEnumerator()
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
		/// Wraps a Int16[] array to get an <see cref="IReadOnlyList{Int16}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Int16> ToROVector(this System.Int16[] array)
		{
			return array is null ? null : new ROShortArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a Int16[] array till a given length to get an <see cref="IReadOnlyList{Int16}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Int16> ToROVector(this System.Int16[] array, int usedlength)
		{
			return new ROShortArrayWrapper(array, usedlength);
		}

			/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{Int16}" /> is neccessary.
    /// </summary>
    private class RODouble_ShortArrayWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected System.Int16[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODouble_ShortArrayWrapper(System.Int16[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODouble_ShortArrayWrapper(System.Int16[] x, int usedlength)
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
		/// Wraps a Int16[] array to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this System.Int16[] array)
		{
			return array is null ? null : new RODouble_ShortArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a Int16[] array till a given length to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this System.Int16[] array, int usedlength)
		{
			return new RODouble_ShortArrayWrapper(array, usedlength);
		}


		  /// <summary>
    /// Serves as wrapper for a section of an array to plug-in where an <see cref="IReadOnlyList{Int16}" /> is neccessary.
    /// </summary>
    private class ROShortArraySectionWrapper : IReadOnlyList<System.Int16>
    {
      protected System.Int16[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public ROShortArraySectionWrapper(System.Int16[] x)
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
      public ROShortArraySectionWrapper(System.Int16[] x, int start, int usedlength)
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
      public System.Int16 this[int i] { get { return _x[i + _start]; } }

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
      public IEnumerator<System.Int16> GetEnumerator()
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
		/// Wraps an array to an <see cref="IReadOnlyList{Int16}" />. Start and length of the used section of the array are specified in the parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of the element in <paramref name="array"/> used as the first element of the vector.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper object with the <see cref="IReadOnlyList{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Int16> ToROVector(this System.Int16[] array, int start, int usedlength)
		{
			if (0 == start)
				return new ROShortArrayWrapper(array, usedlength);
			else
				return new ROShortArraySectionWrapper(array, start, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{Int16}" /> is neccessary.
    /// </summary>
    private class ROShortArrayWrapperAmendedShifted : IReadOnlyList<System.Int16>
    {
      private int _length;
      protected System.Int16[] _x;
			protected System.Int16 _amendedValueAtStart;
			protected int _amendedValuesAtStartCount;
			protected System.Int16 _amendedValueAtEnd;
			protected int _amendedValuesAtEndCount;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public ROShortArrayWrapperAmendedShifted(System.Int16[] x, System.Int16 amendedValueAtStart, int amendedValuesAtStartCount, System.Int16 amendedValueAtEnd, int amendedValuesAtEndCount)
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

      public ROShortArrayWrapperAmendedShifted(System.Int16[] x, int usedlength, System.Int16 amendedValueAtStart, int amendedValuesAtStartCount, System.Int16 amendedValueAtEnd, int amendedValuesAtEndCount)
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
      public System.Int16 this[int i]
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

      public IEnumerator<System.Int16> GetEnumerator()
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
		/// Wraps a Int16[] array to get an <see cref="IReadOnlyList{Int16}" />.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Int16> ToROVectorAmendedShifted(this System.Int16[] array,System.Int16 amendedValueAtStart, int amendedValuesAtStartCount, System.Int16 amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return array is null ? null : new ROShortArrayWrapperAmendedShifted(array, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}

		/// <summary>
		/// Wraps a Int16[] array till a given length to get an <see cref="IReadOnlyList{Int16}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{Int16}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.Int16> ToROVectorAmendedShifted(this System.Int16[] array, int usedlength,System.Int16 amendedValueAtStart, int amendedValuesAtStartCount, System.Int16 amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return new ROShortArrayWrapperAmendedShifted(array, usedlength, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}


		/// <summary>
    /// Serves as thin wrapper struct for an array when additional data at the start and the end of the array are neccessary.
    /// </summary>
    public struct ROShortArrayWrapperStructAmendedUnshifted : IReadOnlyList<System.Int16>
    {
      private int _length;
      private  System.Int16[] _x;
			private  System.Int16 _amendedValueAtStart;
			private  System.Int16 _amendedValueAtEnd;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
      public ROShortArrayWrapperStructAmendedUnshifted(System.Int16[] x, System.Int16 amendedValueAtStart, System.Int16 amendedValueAtEnd)
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
	      public ROShortArrayWrapperStructAmendedUnshifted(System.Int16[] x, int usedlength, System.Int16 amendedValueAtStart, System.Int16 amendedValueAtEnd)
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
      public System.Int16 this[int i]
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

      public IEnumerator<System.Int16> GetEnumerator()
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
		/// Wraps a Int16[] array to get a struct with an <see cref="IReadOnlyList{Int16}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above Length, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater than or equal to <paramref name="array"/>.Length.</param>.
		/// <returns>A wrapper struct with the <see cref="IReadOnlyList{Int16}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROShortArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this System.Int16[] array,System.Int16 amendedValueAtStart, System.Int16 amendedValueAtEnd)
		{
			return new ROShortArrayWrapperStructAmendedUnshifted(array, amendedValueAtStart, amendedValueAtEnd);
		}

		/// <summary>
		/// Wraps a Int16[] array till a given length to get a struct with an <see cref="IReadOnlyList{Int16}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above <paramref name="usedlength"/>, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to <paramref name="usedlength"/>.</param>.
		/// <returns>A wrapper struct with the <see cref="IReadOnlyList{Int16}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROShortArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this System.Int16[] array, int usedlength,System.Int16 amendedValueAtStart, System.Int16 amendedValueAtEnd)
		{
			return new ROShortArrayWrapperStructAmendedUnshifted(array, usedlength, amendedValueAtStart, amendedValueAtEnd );
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where a IVector is neccessary.
    /// </summary>
		private class RWShortArrayWrapper : ROShortArrayWrapper, IVector<System.Int16>
    {
      public RWShortArrayWrapper(System.Int16[] x)
        : base(x)
      {
      }

      public RWShortArrayWrapper(System.Int16[] x, int usedlength)
        : base(x, usedlength)
      {
      }

      public new System.Int16 this[int i]
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
		public static IVector<System.Int16> ToVector(this System.Int16[] array)
		{
			return new RWShortArrayWrapper(array);
		}

		/// <summary>
		/// Wraps an array to get an <see cref="IVector{Int16}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Used length of the array to get the wrapped vector (i.e. the vector wraps around <paramref name="array"/>[0..usedLength-1]).</param>
		/// <returns>A wrapper objects with the <see cref="IVector{Int16}" /> interface that wraps the provided array.</returns>
		public static IVector<System.Int16> ToVector(System.Int16[] array, int usedlength)
		{
			return new RWShortArrayWrapper(array, usedlength);
		}

		
		private class RWShortArraySectionWrapper : ROShortArraySectionWrapper, IVector<System.Int16>
    {
      public RWShortArraySectionWrapper(System.Int16[] x)
        : base(x)
      {
      }

      public RWShortArraySectionWrapper(System.Int16[] x, int start, int usedlength)
        : base(x, start, usedlength)
      {
      }

      public new System.Int16 this[int i]
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
		public static IVector<System.Int16> ToVector(this System.Int16[] array, int start, int count)
		{
			if (0 == start)
				return new RWShortArrayWrapper(array, count);
			else
				return new RWShortArraySectionWrapper(array, start, count);
		}



		 /// <summary>
    /// Serves as wrapper for an <see cref="IReadOnlyList{Int16}" /> to get only a section of the original wrapper.
    /// </summary>
    private class ROShortVectorSectionWrapper : IReadOnlyList<System.Int16>
    {
      protected IReadOnlyList<System.Int16> _x;
      private int _start;
      private int _length;


		 /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The vector to wrap.</param>
      /// <param name="start">Start index of the section of the vector to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public ROShortVectorSectionWrapper(IReadOnlyList<System.Int16> x, int start, int len)
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
      public System.Int16 this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<System.Int16> GetEnumerator()
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
		/// <returns>An <see cref="IReadOnlyList{Int16}" /> that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IReadOnlyList<System.Int16> ToROVector(this IReadOnlyList<System.Int16> vector, int start, int usedLength)
		{
			return new ROShortVectorSectionWrapper(vector, start, usedLength);
		}

		  /// <summary>
    /// Serves as wrapper for an IVector to get only a section of the original wrapper.
    /// </summary>
    private class RWShortVectorSectionWrapper : IVector<System.Int16>
    {
      protected IVector<System.Int16> _x;
      private int _start;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Start index of the section to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RWShortVectorSectionWrapper(IVector<System.Int16> x, int start, int len)
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
      public System.Int16 this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<System.Int16> GetEnumerator()
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
		public static IVector<System.Int16> ToVector(this IVector<System.Int16> vector, int start, int len)
		{
			return new RWShortVectorSectionWrapper(vector, start, len);
		}

    /// <summary>
	/// Returns a clone of the source vector.
	/// </summary>
	/// <param name="sourceVector">The source vector.</param>
    /// <returns>A freshly allocated clone of the sourceVector, with the same element values as the source vector.</returns>
	public static System.Int16[] Clone(System.Int16[] sourceVector)
	{
		if (sourceVector is null)
			throw new ArgumentNullException(nameof(sourceVector));

        var destVector = new System.Int16[sourceVector.Length];
        Array.Copy(sourceVector, destVector, sourceVector.Length);
        return destVector;
	}


#pragma warning restore CS3002 // enable CS3002 again
#pragma warning restore CS3003 // enable CS3003 again
#pragma warning disable CS3002 // Disable not CLS-compliant warning for generated code
#pragma warning disable CS3003 // Disable not CLS-compliant warning for generated code
// ******************************************** Definitions for System.SByte *******************************************

    /// <summary>
		/// Provides a read-only vector with equal and constant items.
		/// </summary>
		private class ROSByteConstantVector : IReadOnlyList<System.SByte>
		{
			private int _length;
			private System.SByte _value;

			public ROSByteConstantVector(System.SByte value, int length)
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

      public System.SByte this[int i]
			{
				get { return _value; }
			}

      public IEnumerator<System.SByte> GetEnumerator()
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
		public static IReadOnlyList<System.SByte> GetConstantVector(System.SByte value, int length)
		{
			return new ROSByteConstantVector(value, length);
		}


		/// <summary>
    /// Provides a read-only vector with equally spaced elements y[i] = start + i * increment.
    /// </summary>
    private class ROSByteEquidistantElementVector : IReadOnlyList<System.SByte>
    {
      private int _length;
      private System.SByte _startValue;
      private System.SByte _incrementValue;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
      public ROSByteEquidistantElementVector(System.SByte start, System.SByte increment, int length)
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
      public System.SByte this[int i]
      {
        get { return (System.SByte)(_startValue + i * _incrementValue); }
      }

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
      public IEnumerator<System.SByte> GetEnumerator()
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
		public static IReadOnlyList<System.SByte> CreateEquidistantSequenceByStartStepLength(System.SByte start, System.SByte step, int length)
		{
			return new ROSByteEquidistantElementVector(start, step, length);
		}

			
			/// <summary>
		/// Provides a read-only vector with equally spaced elements y[i] = start + (i-startOffset) * increment.
		/// </summary>
		private class ROSByteEquidistantElementVectorStartAtOffsetStepLength : IReadOnlyList<System.SByte>
		{
			private System.SByte _start;
			private int _startOffset;

			private System.SByte _increment;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the element at index <paramref name="startOffset"/> of the vector.</param>
			/// <param name="startOffset">The index of the element for which a value is provided in <paramref name="start"/>.</param>
			/// <param name="increment">Difference between an element of the vector and the previous element.</param>
			/// <param name="length">Length of the vector.</param>
			public ROSByteEquidistantElementVectorStartAtOffsetStepLength(System.SByte start, int startOffset, System.SByte increment, int length)
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
			public System.SByte this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");
					return (System.SByte)(_start + (i - _startOffset) * _increment);
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<System.SByte> GetEnumerator()
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
		public static IReadOnlyList<System.SByte> CreateEquidistantSequencyByStartAtOffsetStepLength(System.SByte start, int startOffset, System.SByte step, int length)
		{
			return new ROSByteEquidistantElementVectorStartAtOffsetStepLength(start, startOffset, step, length);
		}


	  	/// <summary>
		/// Provides a read-only vector with equally spaced elements so that y[0] = start and y[length-1] = end.
		/// </summary>
		private class ROSByteEquidistantElementVectorStartEndLength : IReadOnlyList<System.SByte>
		{
			private System.SByte _start;
			private System.SByte _end;
			private int _length;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="start">Value of the first element of the vector.</param>
			/// <param name="end">Value of the last element of the vector.</param>
			/// <param name="length">Length of the vector.</param>
			public ROSByteEquidistantElementVectorStartEndLength(System.SByte start, System.SByte end, int length)
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
			public System.SByte this[int i]
			{
				get
				{
					if (i < 0 || i >= _length)
						throw new ArgumentOutOfRangeException("i");

					double r = i / (double)(_length - 1);
					return (System.SByte)(_start * (1 - r) + _end * (r));
				}
			}

			/// <summary>
			/// Enumerates all elements of the vector.
			/// </summary>
			/// <returns>Enumerator that enumerates all elements of the vector.</returns>
			public IEnumerator<System.SByte> GetEnumerator()
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
		public static IReadOnlyList<System.SByte> CreateEquidistantSequenceByStartEndLength(System.SByte start, System.SByte end, int length)
		{
			return new ROSByteEquidistantElementVectorStartEndLength(start, end, length);
		}


		/// <summary>
    /// Serves as wrapper for an RO Vector which returns the inverse of the elements of the original array.
    /// </summary>
    private class ROSByteInverseElementWrapper : IReadOnlyList<System.Double>
    {
      private int _length;
      protected IReadOnlyList<System.SByte> _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROSByteInverseElementWrapper(IReadOnlyList<System.SByte> x)
      {
        _x = x;
        _length = x.Count;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROSByteInverseElementWrapper(IReadOnlyList<System.SByte> x, int usedlength)
      {
        if (usedlength > x.Count)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public System.Double this[int i]
      {
        get
        {
          return 1/(System.Double)_x[i];
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

      public IEnumerator<System.Double> GetEnumerator()
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
		/// Wraps a SByte[] array to get an  <see cref="IReadOnlyList{SByte}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{SByte}" /> returning elements that are inverse to those of the original vector.</returns>
		public static IReadOnlyList<System.Double> ToInverseROVector(this IReadOnlyList<System.SByte> array)
		{
			return array is null ? null : new ROSByteInverseElementWrapper(array);
		}

		/// <summary>
		/// Wraps a SByte[] array till a given length to get an <see cref="IReadOnlyList{SByte}" /> with elements = 1 / elements of the original vector.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{SByte}" /> interface that wraps the provided array returning elements that are inverse to those of the original vector..</returns>
		public static IReadOnlyList<System.Double> ToInverseROVector(this IReadOnlyList<System.SByte> array, int usedlength)
		{
			return new ROSByteInverseElementWrapper(array, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{SByte}" /> is neccessary.
    /// </summary>
    private class ROSByteArrayWrapper : IReadOnlyList<System.SByte>
    {
      private int _length;
      protected System.SByte[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public ROSByteArrayWrapper(System.SByte[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public ROSByteArrayWrapper(System.SByte[] x, int usedlength)
      {
        if (usedlength > x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _length = Math.Max(0,usedlength);
        _x = x;
      }

      /// <summary>Gets the value at index i with 0 &lt;= i &lt;=Length-1.</summary>
      /// <value>The element at index i.</value>
      public System.SByte this[int i]
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

      public IEnumerator<System.SByte> GetEnumerator()
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
		/// Wraps a SByte[] array to get an <see cref="IReadOnlyList{SByte}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.SByte> ToROVector(this System.SByte[] array)
		{
			return array is null ? null : new ROSByteArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a SByte[] array till a given length to get an <see cref="IReadOnlyList{SByte}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.SByte> ToROVector(this System.SByte[] array, int usedlength)
		{
			return new ROSByteArrayWrapper(array, usedlength);
		}

			/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{SByte}" /> is neccessary.
    /// </summary>
    private class RODouble_SByteArrayWrapper : IReadOnlyList<double>
    {
      private int _length;
      protected System.SByte[] _x;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
      public RODouble_SByteArrayWrapper(System.SByte[] x)
      {
        _length = x.Length;
        _x = x;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODouble_SByteArrayWrapper(System.SByte[] x, int usedlength)
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
		/// Wraps a SByte[] array to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this System.SByte[] array)
		{
			return array is null ? null : new RODouble_SByteArrayWrapper(array);
		}

		/// <summary>
		/// Wraps a SByte[] array till a given length to get an <see cref="IReadOnlyList{Double}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<double> ToRODoubleVector(this System.SByte[] array, int usedlength)
		{
			return new RODouble_SByteArrayWrapper(array, usedlength);
		}


		  /// <summary>
    /// Serves as wrapper for a section of an array to plug-in where an <see cref="IReadOnlyList{SByte}" /> is neccessary.
    /// </summary>
    private class ROSByteArraySectionWrapper : IReadOnlyList<System.SByte>
    {
      protected System.SByte[] _x;
      protected int _start;
      protected int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public ROSByteArraySectionWrapper(System.SByte[] x)
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
      public ROSByteArraySectionWrapper(System.SByte[] x, int start, int usedlength)
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
      public System.SByte this[int i] { get { return _x[i + _start]; } }

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
      public IEnumerator<System.SByte> GetEnumerator()
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
		/// Wraps an array to an <see cref="IReadOnlyList{SByte}" />. Start and length of the used section of the array are specified in the parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="start">Index of the element in <paramref name="array"/> used as the first element of the vector.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
		/// <returns>A wrapper object with the <see cref="IReadOnlyList{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.SByte> ToROVector(this System.SByte[] array, int start, int usedlength)
		{
			if (0 == start)
				return new ROSByteArrayWrapper(array, usedlength);
			else
				return new ROSByteArraySectionWrapper(array, start, usedlength);
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where an <see cref="IReadOnlyList{SByte}" /> is neccessary.
    /// </summary>
    private class ROSByteArrayWrapperAmendedShifted : IReadOnlyList<System.SByte>
    {
      private int _length;
      protected System.SByte[] _x;
			protected System.SByte _amendedValueAtStart;
			protected int _amendedValuesAtStartCount;
			protected System.SByte _amendedValueAtEnd;
			protected int _amendedValuesAtEndCount;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
			/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
			/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
			/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>

      public ROSByteArrayWrapperAmendedShifted(System.SByte[] x, System.SByte amendedValueAtStart, int amendedValuesAtStartCount, System.SByte amendedValueAtEnd, int amendedValuesAtEndCount)
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

      public ROSByteArrayWrapperAmendedShifted(System.SByte[] x, int usedlength, System.SByte amendedValueAtStart, int amendedValuesAtStartCount, System.SByte amendedValueAtEnd, int amendedValuesAtEndCount)
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
      public System.SByte this[int i]
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

      public IEnumerator<System.SByte> GetEnumerator()
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
		/// Wraps a SByte[] array to get an <see cref="IReadOnlyList{SByte}" />.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.SByte> ToROVectorAmendedShifted(this System.SByte[] array,System.SByte amendedValueAtStart, int amendedValuesAtStartCount, System.SByte amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return array is null ? null : new ROSByteArrayWrapperAmendedShifted(array, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}

		/// <summary>
		/// Wraps a SByte[] array till a given length to get an <see cref="IReadOnlyList{SByte}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index <paramref name="amendedValuesAtStartCount"/> in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at the first <paramref name="amendedValuesAtStartCount"/> indices</param>.
		/// <param name="amendedValuesAtStartCount">Number of indices at the start of the vector that take the value of <paramref name="amendedValueAtStart"/>. The first element of the wrapped array starts at index <paramref name="amendedValuesAtStartCount"/>.</param>
		/// <param name="amendedValueAtEnd">Value of the vector at the last <paramref name="amendedValuesAtEndCount"/> indices</param>.
		/// <param name="amendedValuesAtEndCount">Number of indices at the end of the vector that take the value of <paramref name="amendedValueAtEnd"/>.</param>
		/// <returns>A wrapper objects with the <see cref="IReadOnlyList{SByte}" /> interface that wraps the provided array.</returns>
		public static IReadOnlyList<System.SByte> ToROVectorAmendedShifted(this System.SByte[] array, int usedlength,System.SByte amendedValueAtStart, int amendedValuesAtStartCount, System.SByte amendedValueAtEnd, int amendedValuesAtEndCount)
		{
			return new ROSByteArrayWrapperAmendedShifted(array, usedlength, amendedValueAtStart, amendedValuesAtStartCount, amendedValueAtEnd, amendedValuesAtEndCount);
		}


		/// <summary>
    /// Serves as thin wrapper struct for an array when additional data at the start and the end of the array are neccessary.
    /// </summary>
    public struct ROSByteArrayWrapperStructAmendedUnshifted : IReadOnlyList<System.SByte>
    {
      private int _length;
      private  System.SByte[] _x;
			private  System.SByte _amendedValueAtStart;
			private  System.SByte _amendedValueAtEnd;
			
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The array to wrap. The array is used directly (without copying).</param>
			/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
			/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to Length.</param>.
      public ROSByteArrayWrapperStructAmendedUnshifted(System.SByte[] x, System.SByte amendedValueAtStart, System.SByte amendedValueAtEnd)
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
	      public ROSByteArrayWrapperStructAmendedUnshifted(System.SByte[] x, int usedlength, System.SByte amendedValueAtStart, System.SByte amendedValueAtEnd)
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
      public System.SByte this[int i]
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

      public IEnumerator<System.SByte> GetEnumerator()
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
		/// Wraps a SByte[] array to get a struct with an <see cref="IReadOnlyList{SByte}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above Length, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater than or equal to <paramref name="array"/>.Length.</param>.
		/// <returns>A wrapper struct with the <see cref="IReadOnlyList{SByte}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROSByteArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this System.SByte[] array,System.SByte amendedValueAtStart, System.SByte amendedValueAtEnd)
		{
			return new ROSByteArrayWrapperStructAmendedUnshifted(array, amendedValueAtStart, amendedValueAtEnd);
		}

		/// <summary>
		/// Wraps a SByte[] array till a given length to get a struct with an <see cref="IReadOnlyList{SByte}" /> implementation. The wrapping is done lazily, i.e. you can access elements with indices below zero and
		/// above <paramref name="usedlength"/>, which is normally forbidden. The values for that are given as parameters.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array. The first element of the array has index 0 in the returned vector.</param>
		/// <param name="amendedValueAtStart">Value of the vector at indices less than zero.</param>.
		/// <param name="amendedValueAtEnd">Value of the vector at indices greater then or equal to <paramref name="usedlength"/>.</param>.
		/// <returns>A wrapper struct with the <see cref="IReadOnlyList{SByte}" /> interface that wraps the provided array, and allows access to elements below and above the valid indices of the array.</returns>
		public static ROSByteArrayWrapperStructAmendedUnshifted ToROVectorStructAmendedUnshifted(this System.SByte[] array, int usedlength,System.SByte amendedValueAtStart, System.SByte amendedValueAtEnd)
		{
			return new ROSByteArrayWrapperStructAmendedUnshifted(array, usedlength, amendedValueAtStart, amendedValueAtEnd );
		}


		/// <summary>
    /// Serves as wrapper for an array to plug-in where a IVector is neccessary.
    /// </summary>
		private class RWSByteArrayWrapper : ROSByteArrayWrapper, IVector<System.SByte>
    {
      public RWSByteArrayWrapper(System.SByte[] x)
        : base(x)
      {
      }

      public RWSByteArrayWrapper(System.SByte[] x, int usedlength)
        : base(x, usedlength)
      {
      }

      public new System.SByte this[int i]
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
		public static IVector<System.SByte> ToVector(this System.SByte[] array)
		{
			return new RWSByteArrayWrapper(array);
		}

		/// <summary>
		/// Wraps an array to get an <see cref="IVector{SByte}" />.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="usedlength">Used length of the array to get the wrapped vector (i.e. the vector wraps around <paramref name="array"/>[0..usedLength-1]).</param>
		/// <returns>A wrapper objects with the <see cref="IVector{SByte}" /> interface that wraps the provided array.</returns>
		public static IVector<System.SByte> ToVector(System.SByte[] array, int usedlength)
		{
			return new RWSByteArrayWrapper(array, usedlength);
		}

		
		private class RWSByteArraySectionWrapper : ROSByteArraySectionWrapper, IVector<System.SByte>
    {
      public RWSByteArraySectionWrapper(System.SByte[] x)
        : base(x)
      {
      }

      public RWSByteArraySectionWrapper(System.SByte[] x, int start, int usedlength)
        : base(x, start, usedlength)
      {
      }

      public new System.SByte this[int i]
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
		public static IVector<System.SByte> ToVector(this System.SByte[] array, int start, int count)
		{
			if (0 == start)
				return new RWSByteArrayWrapper(array, count);
			else
				return new RWSByteArraySectionWrapper(array, start, count);
		}



		 /// <summary>
    /// Serves as wrapper for an <see cref="IReadOnlyList{SByte}" /> to get only a section of the original wrapper.
    /// </summary>
    private class ROSByteVectorSectionWrapper : IReadOnlyList<System.SByte>
    {
      protected IReadOnlyList<System.SByte> _x;
      private int _start;
      private int _length;


		 /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x">The vector to wrap.</param>
      /// <param name="start">Start index of the section of the vector to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public ROSByteVectorSectionWrapper(IReadOnlyList<System.SByte> x, int start, int len)
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
      public System.SByte this[int i] { get { return _x[i + _start]; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<System.SByte> GetEnumerator()
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
		/// <returns>An <see cref="IReadOnlyList{SByte}" /> that contains the section from <c>start</c> to <c>start+len-1</c> of the original vector.</returns>
		public static IReadOnlyList<System.SByte> ToROVector(this IReadOnlyList<System.SByte> vector, int start, int usedLength)
		{
			return new ROSByteVectorSectionWrapper(vector, start, usedLength);
		}

		  /// <summary>
    /// Serves as wrapper for an IVector to get only a section of the original wrapper.
    /// </summary>
    private class RWSByteVectorSectionWrapper : IVector<System.SByte>
    {
      protected IVector<System.SByte> _x;
      private int _start;
      private int _length;

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="start">Start index of the section to wrap.</param>
      /// <param name="len">Length of the section to wrap.</param>
      public RWSByteVectorSectionWrapper(IVector<System.SByte> x, int start, int len)
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
      public System.SByte this[int i]
      {
        get { return _x[i + _start]; }
        set { _x[i + _start] = value; }
      }

      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; } }

      /// <summary>The number of elements of this vector.</summary>
      public int Count { get { return _length; } }

      public IEnumerator<System.SByte> GetEnumerator()
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
		public static IVector<System.SByte> ToVector(this IVector<System.SByte> vector, int start, int len)
		{
			return new RWSByteVectorSectionWrapper(vector, start, len);
		}

    /// <summary>
	/// Returns a clone of the source vector.
	/// </summary>
	/// <param name="sourceVector">The source vector.</param>
    /// <returns>A freshly allocated clone of the sourceVector, with the same element values as the source vector.</returns>
	public static System.SByte[] Clone(System.SByte[] sourceVector)
	{
		if (sourceVector is null)
			throw new ArgumentNullException(nameof(sourceVector));

        var destVector = new System.SByte[sourceVector.Length];
        Array.Copy(sourceVector, destVector, sourceVector.Length);
        return destVector;
	}


#pragma warning restore CS3002 // enable CS3002 again
#pragma warning restore CS3003 // enable CS3003 again
	} // class
} // namespace
