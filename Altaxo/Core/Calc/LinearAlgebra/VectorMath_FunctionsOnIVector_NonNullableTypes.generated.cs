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

// ******************************************** Definitions for double[] *******************************************

		/// <summary>
		/// Elementwise application of a function to each element of a vector.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="function">The function to apply to every element.</param>
		public static void Map(this double[] vector, Func<double, double> function)
		{
			for (int i = vector.Length - 1; i >= 0; --i)
				vector[i] = function(vector[i]);
		}


		/// <summary>
		/// Multiplies all vector elements with a constant.
		/// </summary>
		/// <param name="v">The vector.</param>
		/// <param name="a">The constant to multiply with.</param>
		public static void Multiply(this double[] v, double a)
		{
			for (int i = v.Length - 1; i >= 0; --i)
				{
				v[i] *= a;
				}
		}

		/// <summary>
		/// Normalizes the vector elements so that the mean of the elements is zero.
		/// </summary>
		/// <param name="v">The vector.</param>
		public static void ToZeroMean(this double[] v)
			{
				var mean = Mean(v);

				for (int i = v.Length - 1; i >= 0; --i)
				{
				v[i] = (double)(v[i]-mean);
				}
			}

		/// <summary>
		/// Normalizes the vector elements so that the mean of the elements is zero.
		/// </summary>
		/// <param name="v">The vector.</param>
		public static void ToZeroMeanAndUnitVariance(this double[] v)
			{
				var (mean, variance) = MeanAndVariance(v);
				var factor = 1/Math.Sqrt(variance);

				for (int i = v.Length - 1; i >= 0; --i)
				{
				v[i] = (double)((v[i]-mean)*factor);
				}
			}

		/// <summary>
		/// Sets the element of the vector to the result of scalar x divided by each element y[i].
		/// </summary>
		/// <param name="x">A scalar value</param>
		/// <param name="y">A vector.</param>
		public static void Divide(double x, double[] y)
		{
			for (int i = 0; i < y.Length; ++i)
				y[i] = x / y[i];
		}
   
		/// <summary>
		/// Fills a vector with a certain value. so that all elements are equal.
		/// </summary>
		/// <param name="vector">The vector to fill.</param>
		/// <param name="val">The value each element is set to.</param>
		public static void FillWith(this double[] vector, double val)
		{
			for (int i = vector.Length - 1; i >= 0; --i)
				vector[i] = val;
		}


	 /// <summary>
		/// Fills the vector v with a linear sequence beginning from start (first element) until end (last element).
		/// </summary>
		/// <param name="vector">The vector to be filled.</param>
		/// <param name="startValue">First value of the vector (value at index 0).</param>
		/// <param name="endValue">Last value of the vector (value at index Length-1).</param>
		public static void FillWithLinearSequenceGivenByStartAndEnd(this double[] vector, double startValue, double endValue)
		{
			int lenM1 = vector.Length - 1;
			if (lenM1 < 0)
			{
				throw new ArgumentException("Vector must have a length > 0", nameof(vector));
			}
			else if (lenM1 == 0)
			{
				if (startValue != endValue)
					throw new ArgumentException("The vector has length=1. Thus startValue and endValue must be equal.");

				vector[0] = startValue;
				return;
			}
			else
			{
				double flenM1 = lenM1;
				for (int i = 0; i <= lenM1; ++i)
				{
					vector[i] = (double)(startValue * ((lenM1 - i) / flenM1) + endValue * (i / flenM1)); // we want to have highest accuracy, even for the end, thus we have to sacrifice speed
				}
			}
		}


		/// <summary>
		/// Reverses the order of elements in the provided vector.
		/// </summary>
		/// <param name="vector">Vector. On return, the elements of this vector are in reverse order.</param>
		public static void Reverse(this double[] vector)
		{
			for (int i = 0, j = vector.Length - 1; i < j; ++i, --j)
			{
				var x_i = vector[i];
				vector[i] = vector[j];
				vector[j] = x_i;
			}
		}


		/// <summary>
		/// Shifts the element of this vector by moving them from index i to index i+<c>increment</c>.
		/// The elements at the end of the vector are wrapped back to the start of the vector. Thus, effectively, the elements of the vector are rotated.
		/// </summary>
		/// <param name="vector">The vector to rotate.</param>
		/// <param name="increment">Offset value.</param>
		public static void Rotate(this double[] vector, int increment)
		{
			int len = vector.Length;
			if (len < 2)
				return; // Nothing to do
			increment = increment % len;
			if (increment == 0)
				return;
			if (increment < 0)
				increment += len;

			// first cycle is to measure number of shifts per cycle
			int shiftsPerCycle = 0;
			int i = 0;
			int k = i;
			var prevVal = vector[k];
			do
			{
				k = (k + increment) % len;
				var currVal = vector[k];
				vector[k] = prevVal;
				prevVal = currVal;
				shiftsPerCycle++;
			} while (i != k);

			// now do the rest of the cycles
			if (!(0 == len % shiftsPerCycle))
				throw new InvalidProgramException();
			int numCycles = len / shiftsPerCycle;
			for (i = 1; i < numCycles; i++)
			{
				k = i;
				prevVal = vector[k];
				do
				{
					k = (k + increment) % len;
					var currVal = vector[k];
					vector[k] = prevVal;
					prevVal = currVal;
				} while (i != k);
			}
		}


// ******************************************** Definitions for float[] *******************************************

		/// <summary>
		/// Elementwise application of a function to each element of a vector.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="function">The function to apply to every element.</param>
		public static void Map(this float[] vector, Func<float, float> function)
		{
			for (int i = vector.Length - 1; i >= 0; --i)
				vector[i] = function(vector[i]);
		}


		/// <summary>
		/// Multiplies all vector elements with a constant.
		/// </summary>
		/// <param name="v">The vector.</param>
		/// <param name="a">The constant to multiply with.</param>
		public static void Multiply(this float[] v, float a)
		{
			for (int i = v.Length - 1; i >= 0; --i)
				{
				v[i] *= a;
				}
		}

		/// <summary>
		/// Normalizes the vector elements so that the mean of the elements is zero.
		/// </summary>
		/// <param name="v">The vector.</param>
		public static void ToZeroMean(this float[] v)
			{
				var mean = Mean(v);

				for (int i = v.Length - 1; i >= 0; --i)
				{
				v[i] = (float)(v[i]-mean);
				}
			}

		/// <summary>
		/// Normalizes the vector elements so that the mean of the elements is zero.
		/// </summary>
		/// <param name="v">The vector.</param>
		public static void ToZeroMeanAndUnitVariance(this float[] v)
			{
				var (mean, variance) = MeanAndVariance(v);
				var factor = 1/Math.Sqrt(variance);

				for (int i = v.Length - 1; i >= 0; --i)
				{
				v[i] = (float)((v[i]-mean)*factor);
				}
			}

		/// <summary>
		/// Sets the element of the vector to the result of scalar x divided by each element y[i].
		/// </summary>
		/// <param name="x">A scalar value</param>
		/// <param name="y">A vector.</param>
		public static void Divide(float x, float[] y)
		{
			for (int i = 0; i < y.Length; ++i)
				y[i] = x / y[i];
		}
   
		/// <summary>
		/// Fills a vector with a certain value. so that all elements are equal.
		/// </summary>
		/// <param name="vector">The vector to fill.</param>
		/// <param name="val">The value each element is set to.</param>
		public static void FillWith(this float[] vector, float val)
		{
			for (int i = vector.Length - 1; i >= 0; --i)
				vector[i] = val;
		}


	 /// <summary>
		/// Fills the vector v with a linear sequence beginning from start (first element) until end (last element).
		/// </summary>
		/// <param name="vector">The vector to be filled.</param>
		/// <param name="startValue">First value of the vector (value at index 0).</param>
		/// <param name="endValue">Last value of the vector (value at index Length-1).</param>
		public static void FillWithLinearSequenceGivenByStartAndEnd(this float[] vector, float startValue, float endValue)
		{
			int lenM1 = vector.Length - 1;
			if (lenM1 < 0)
			{
				throw new ArgumentException("Vector must have a length > 0", nameof(vector));
			}
			else if (lenM1 == 0)
			{
				if (startValue != endValue)
					throw new ArgumentException("The vector has length=1. Thus startValue and endValue must be equal.");

				vector[0] = startValue;
				return;
			}
			else
			{
				double flenM1 = lenM1;
				for (int i = 0; i <= lenM1; ++i)
				{
					vector[i] = (float)(startValue * ((lenM1 - i) / flenM1) + endValue * (i / flenM1)); // we want to have highest accuracy, even for the end, thus we have to sacrifice speed
				}
			}
		}


		/// <summary>
		/// Reverses the order of elements in the provided vector.
		/// </summary>
		/// <param name="vector">Vector. On return, the elements of this vector are in reverse order.</param>
		public static void Reverse(this float[] vector)
		{
			for (int i = 0, j = vector.Length - 1; i < j; ++i, --j)
			{
				var x_i = vector[i];
				vector[i] = vector[j];
				vector[j] = x_i;
			}
		}


		/// <summary>
		/// Shifts the element of this vector by moving them from index i to index i+<c>increment</c>.
		/// The elements at the end of the vector are wrapped back to the start of the vector. Thus, effectively, the elements of the vector are rotated.
		/// </summary>
		/// <param name="vector">The vector to rotate.</param>
		/// <param name="increment">Offset value.</param>
		public static void Rotate(this float[] vector, int increment)
		{
			int len = vector.Length;
			if (len < 2)
				return; // Nothing to do
			increment = increment % len;
			if (increment == 0)
				return;
			if (increment < 0)
				increment += len;

			// first cycle is to measure number of shifts per cycle
			int shiftsPerCycle = 0;
			int i = 0;
			int k = i;
			var prevVal = vector[k];
			do
			{
				k = (k + increment) % len;
				var currVal = vector[k];
				vector[k] = prevVal;
				prevVal = currVal;
				shiftsPerCycle++;
			} while (i != k);

			// now do the rest of the cycles
			if (!(0 == len % shiftsPerCycle))
				throw new InvalidProgramException();
			int numCycles = len / shiftsPerCycle;
			for (i = 1; i < numCycles; i++)
			{
				k = i;
				prevVal = vector[k];
				do
				{
					k = (k + increment) % len;
					var currVal = vector[k];
					vector[k] = prevVal;
					prevVal = currVal;
				} while (i != k);
			}
		}


// ******************************************** Definitions for int[] *******************************************

		/// <summary>
		/// Elementwise application of a function to each element of a vector.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="function">The function to apply to every element.</param>
		public static void Map(this int[] vector, Func<int, int> function)
		{
			for (int i = vector.Length - 1; i >= 0; --i)
				vector[i] = function(vector[i]);
		}


		/// <summary>
		/// Multiplies all vector elements with a constant.
		/// </summary>
		/// <param name="v">The vector.</param>
		/// <param name="a">The constant to multiply with.</param>
		public static void Multiply(this int[] v, int a)
		{
			for (int i = v.Length - 1; i >= 0; --i)
				{
				v[i] *= a;
				}
		}

		/// <summary>
		/// Normalizes the vector elements so that the mean of the elements is zero.
		/// </summary>
		/// <param name="v">The vector.</param>
		public static void ToZeroMean(this int[] v)
			{
				var mean = Mean(v);

				for (int i = v.Length - 1; i >= 0; --i)
				{
				v[i] = (int)(v[i]-mean);
				}
			}

		/// <summary>
		/// Normalizes the vector elements so that the mean of the elements is zero.
		/// </summary>
		/// <param name="v">The vector.</param>
		public static void ToZeroMeanAndUnitVariance(this int[] v)
			{
				var (mean, variance) = MeanAndVariance(v);
				var factor = 1/Math.Sqrt(variance);

				for (int i = v.Length - 1; i >= 0; --i)
				{
				v[i] = (int)((v[i]-mean)*factor);
				}
			}

		/// <summary>
		/// Sets the element of the vector to the result of scalar x divided by each element y[i].
		/// </summary>
		/// <param name="x">A scalar value</param>
		/// <param name="y">A vector.</param>
		public static void Divide(int x, int[] y)
		{
			for (int i = 0; i < y.Length; ++i)
				y[i] = x / y[i];
		}
   
		/// <summary>
		/// Fills a vector with a certain value. so that all elements are equal.
		/// </summary>
		/// <param name="vector">The vector to fill.</param>
		/// <param name="val">The value each element is set to.</param>
		public static void FillWith(this int[] vector, int val)
		{
			for (int i = vector.Length - 1; i >= 0; --i)
				vector[i] = val;
		}


	 /// <summary>
		/// Fills the vector v with a linear sequence beginning from start (first element) until end (last element).
		/// </summary>
		/// <param name="vector">The vector to be filled.</param>
		/// <param name="startValue">First value of the vector (value at index 0).</param>
		/// <param name="endValue">Last value of the vector (value at index Length-1).</param>
		public static void FillWithLinearSequenceGivenByStartAndEnd(this int[] vector, int startValue, int endValue)
		{
			int lenM1 = vector.Length - 1;
			if (lenM1 < 0)
			{
				throw new ArgumentException("Vector must have a length > 0", nameof(vector));
			}
			else if (lenM1 == 0)
			{
				if (startValue != endValue)
					throw new ArgumentException("The vector has length=1. Thus startValue and endValue must be equal.");

				vector[0] = startValue;
				return;
			}
			else
			{
				double flenM1 = lenM1;
				for (int i = 0; i <= lenM1; ++i)
				{
					vector[i] = (int)(startValue * ((lenM1 - i) / flenM1) + endValue * (i / flenM1)); // we want to have highest accuracy, even for the end, thus we have to sacrifice speed
				}
			}
		}


		/// <summary>
		/// Reverses the order of elements in the provided vector.
		/// </summary>
		/// <param name="vector">Vector. On return, the elements of this vector are in reverse order.</param>
		public static void Reverse(this int[] vector)
		{
			for (int i = 0, j = vector.Length - 1; i < j; ++i, --j)
			{
				var x_i = vector[i];
				vector[i] = vector[j];
				vector[j] = x_i;
			}
		}


		/// <summary>
		/// Shifts the element of this vector by moving them from index i to index i+<c>increment</c>.
		/// The elements at the end of the vector are wrapped back to the start of the vector. Thus, effectively, the elements of the vector are rotated.
		/// </summary>
		/// <param name="vector">The vector to rotate.</param>
		/// <param name="increment">Offset value.</param>
		public static void Rotate(this int[] vector, int increment)
		{
			int len = vector.Length;
			if (len < 2)
				return; // Nothing to do
			increment = increment % len;
			if (increment == 0)
				return;
			if (increment < 0)
				increment += len;

			// first cycle is to measure number of shifts per cycle
			int shiftsPerCycle = 0;
			int i = 0;
			int k = i;
			var prevVal = vector[k];
			do
			{
				k = (k + increment) % len;
				var currVal = vector[k];
				vector[k] = prevVal;
				prevVal = currVal;
				shiftsPerCycle++;
			} while (i != k);

			// now do the rest of the cycles
			if (!(0 == len % shiftsPerCycle))
				throw new InvalidProgramException();
			int numCycles = len / shiftsPerCycle;
			for (i = 1; i < numCycles; i++)
			{
				k = i;
				prevVal = vector[k];
				do
				{
					k = (k + increment) % len;
					var currVal = vector[k];
					vector[k] = prevVal;
					prevVal = currVal;
				} while (i != k);
			}
		}


// ******************************************** Definitions for IReadOnlyList<double> *******************************************

		/// <summary>
		/// Elementwise application of a function to each element of a vector.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="function">The function to apply to every element.</param>
		public static void Map(this IVector<double> vector, Func<double, double> function)
		{
			for (int i = vector.Count - 1; i >= 0; --i)
				vector[i] = function(vector[i]);
		}


		/// <summary>
		/// Multiplies all vector elements with a constant.
		/// </summary>
		/// <param name="v">The vector.</param>
		/// <param name="a">The constant to multiply with.</param>
		public static void Multiply(this IVector<double> v, double a)
		{
			for (int i = v.Count - 1; i >= 0; --i)
				{
				v[i] *= a;
				}
		}

		/// <summary>
		/// Normalizes the vector elements so that the mean of the elements is zero.
		/// </summary>
		/// <param name="v">The vector.</param>
		public static void ToZeroMean(this IVector<double> v)
			{
				var mean = Mean(v);

				for (int i = v.Count - 1; i >= 0; --i)
				{
				v[i] = (double)(v[i]-mean);
				}
			}

		/// <summary>
		/// Normalizes the vector elements so that the mean of the elements is zero.
		/// </summary>
		/// <param name="v">The vector.</param>
		public static void ToZeroMeanAndUnitVariance(this IVector<double> v)
			{
				var (mean, variance) = MeanAndVariance(v);
				var factor = 1/Math.Sqrt(variance);

				for (int i = v.Count - 1; i >= 0; --i)
				{
				v[i] = (double)((v[i]-mean)*factor);
				}
			}

		/// <summary>
		/// Sets the element of the vector to the result of scalar x divided by each element y[i].
		/// </summary>
		/// <param name="x">A scalar value</param>
		/// <param name="y">A vector.</param>
		public static void Divide(double x, IVector<double> y)
		{
			for (int i = 0; i < y.Count; ++i)
				y[i] = x / y[i];
		}
   
		/// <summary>
		/// Fills a vector with a certain value. so that all elements are equal.
		/// </summary>
		/// <param name="vector">The vector to fill.</param>
		/// <param name="val">The value each element is set to.</param>
		public static void FillWith(this IVector<double> vector, double val)
		{
			for (int i = vector.Count - 1; i >= 0; --i)
				vector[i] = val;
		}


	 /// <summary>
		/// Fills the vector v with a linear sequence beginning from start (first element) until end (last element).
		/// </summary>
		/// <param name="vector">The vector to be filled.</param>
		/// <param name="startValue">First value of the vector (value at index 0).</param>
		/// <param name="endValue">Last value of the vector (value at index Length-1).</param>
		public static void FillWithLinearSequenceGivenByStartAndEnd(this IVector<double> vector, double startValue, double endValue)
		{
			int lenM1 = vector.Count - 1;
			if (lenM1 < 0)
			{
				throw new ArgumentException("Vector must have a length > 0", nameof(vector));
			}
			else if (lenM1 == 0)
			{
				if (startValue != endValue)
					throw new ArgumentException("The vector has length=1. Thus startValue and endValue must be equal.");

				vector[0] = startValue;
				return;
			}
			else
			{
				double flenM1 = lenM1;
				for (int i = 0; i <= lenM1; ++i)
				{
					vector[i] = (double)(startValue * ((lenM1 - i) / flenM1) + endValue * (i / flenM1)); // we want to have highest accuracy, even for the end, thus we have to sacrifice speed
				}
			}
		}


		/// <summary>
		/// Reverses the order of elements in the provided vector.
		/// </summary>
		/// <param name="vector">Vector. On return, the elements of this vector are in reverse order.</param>
		public static void Reverse(this IVector<double> vector)
		{
			for (int i = 0, j = vector.Count - 1; i < j; ++i, --j)
			{
				var x_i = vector[i];
				vector[i] = vector[j];
				vector[j] = x_i;
			}
		}


		/// <summary>
		/// Shifts the element of this vector by moving them from index i to index i+<c>increment</c>.
		/// The elements at the end of the vector are wrapped back to the start of the vector. Thus, effectively, the elements of the vector are rotated.
		/// </summary>
		/// <param name="vector">The vector to rotate.</param>
		/// <param name="increment">Offset value.</param>
		public static void Rotate(this IVector<double> vector, int increment)
		{
			int len = vector.Count;
			if (len < 2)
				return; // Nothing to do
			increment = increment % len;
			if (increment == 0)
				return;
			if (increment < 0)
				increment += len;

			// first cycle is to measure number of shifts per cycle
			int shiftsPerCycle = 0;
			int i = 0;
			int k = i;
			var prevVal = vector[k];
			do
			{
				k = (k + increment) % len;
				var currVal = vector[k];
				vector[k] = prevVal;
				prevVal = currVal;
				shiftsPerCycle++;
			} while (i != k);

			// now do the rest of the cycles
			if (!(0 == len % shiftsPerCycle))
				throw new InvalidProgramException();
			int numCycles = len / shiftsPerCycle;
			for (i = 1; i < numCycles; i++)
			{
				k = i;
				prevVal = vector[k];
				do
				{
					k = (k + increment) % len;
					var currVal = vector[k];
					vector[k] = prevVal;
					prevVal = currVal;
				} while (i != k);
			}
		}


// ******************************************** Definitions for IReadOnlyList<float> *******************************************

		/// <summary>
		/// Elementwise application of a function to each element of a vector.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="function">The function to apply to every element.</param>
		public static void Map(this IVector<float> vector, Func<float, float> function)
		{
			for (int i = vector.Count - 1; i >= 0; --i)
				vector[i] = function(vector[i]);
		}


		/// <summary>
		/// Multiplies all vector elements with a constant.
		/// </summary>
		/// <param name="v">The vector.</param>
		/// <param name="a">The constant to multiply with.</param>
		public static void Multiply(this IVector<float> v, float a)
		{
			for (int i = v.Count - 1; i >= 0; --i)
				{
				v[i] *= a;
				}
		}

		/// <summary>
		/// Normalizes the vector elements so that the mean of the elements is zero.
		/// </summary>
		/// <param name="v">The vector.</param>
		public static void ToZeroMean(this IVector<float> v)
			{
				var mean = Mean(v);

				for (int i = v.Count - 1; i >= 0; --i)
				{
				v[i] = (float)(v[i]-mean);
				}
			}

		/// <summary>
		/// Normalizes the vector elements so that the mean of the elements is zero.
		/// </summary>
		/// <param name="v">The vector.</param>
		public static void ToZeroMeanAndUnitVariance(this IVector<float> v)
			{
				var (mean, variance) = MeanAndVariance(v);
				var factor = 1/Math.Sqrt(variance);

				for (int i = v.Count - 1; i >= 0; --i)
				{
				v[i] = (float)((v[i]-mean)*factor);
				}
			}

		/// <summary>
		/// Sets the element of the vector to the result of scalar x divided by each element y[i].
		/// </summary>
		/// <param name="x">A scalar value</param>
		/// <param name="y">A vector.</param>
		public static void Divide(float x, IVector<float> y)
		{
			for (int i = 0; i < y.Count; ++i)
				y[i] = x / y[i];
		}
   
		/// <summary>
		/// Fills a vector with a certain value. so that all elements are equal.
		/// </summary>
		/// <param name="vector">The vector to fill.</param>
		/// <param name="val">The value each element is set to.</param>
		public static void FillWith(this IVector<float> vector, float val)
		{
			for (int i = vector.Count - 1; i >= 0; --i)
				vector[i] = val;
		}


	 /// <summary>
		/// Fills the vector v with a linear sequence beginning from start (first element) until end (last element).
		/// </summary>
		/// <param name="vector">The vector to be filled.</param>
		/// <param name="startValue">First value of the vector (value at index 0).</param>
		/// <param name="endValue">Last value of the vector (value at index Length-1).</param>
		public static void FillWithLinearSequenceGivenByStartAndEnd(this IVector<float> vector, float startValue, float endValue)
		{
			int lenM1 = vector.Count - 1;
			if (lenM1 < 0)
			{
				throw new ArgumentException("Vector must have a length > 0", nameof(vector));
			}
			else if (lenM1 == 0)
			{
				if (startValue != endValue)
					throw new ArgumentException("The vector has length=1. Thus startValue and endValue must be equal.");

				vector[0] = startValue;
				return;
			}
			else
			{
				double flenM1 = lenM1;
				for (int i = 0; i <= lenM1; ++i)
				{
					vector[i] = (float)(startValue * ((lenM1 - i) / flenM1) + endValue * (i / flenM1)); // we want to have highest accuracy, even for the end, thus we have to sacrifice speed
				}
			}
		}


		/// <summary>
		/// Reverses the order of elements in the provided vector.
		/// </summary>
		/// <param name="vector">Vector. On return, the elements of this vector are in reverse order.</param>
		public static void Reverse(this IVector<float> vector)
		{
			for (int i = 0, j = vector.Count - 1; i < j; ++i, --j)
			{
				var x_i = vector[i];
				vector[i] = vector[j];
				vector[j] = x_i;
			}
		}


		/// <summary>
		/// Shifts the element of this vector by moving them from index i to index i+<c>increment</c>.
		/// The elements at the end of the vector are wrapped back to the start of the vector. Thus, effectively, the elements of the vector are rotated.
		/// </summary>
		/// <param name="vector">The vector to rotate.</param>
		/// <param name="increment">Offset value.</param>
		public static void Rotate(this IVector<float> vector, int increment)
		{
			int len = vector.Count;
			if (len < 2)
				return; // Nothing to do
			increment = increment % len;
			if (increment == 0)
				return;
			if (increment < 0)
				increment += len;

			// first cycle is to measure number of shifts per cycle
			int shiftsPerCycle = 0;
			int i = 0;
			int k = i;
			var prevVal = vector[k];
			do
			{
				k = (k + increment) % len;
				var currVal = vector[k];
				vector[k] = prevVal;
				prevVal = currVal;
				shiftsPerCycle++;
			} while (i != k);

			// now do the rest of the cycles
			if (!(0 == len % shiftsPerCycle))
				throw new InvalidProgramException();
			int numCycles = len / shiftsPerCycle;
			for (i = 1; i < numCycles; i++)
			{
				k = i;
				prevVal = vector[k];
				do
				{
					k = (k + increment) % len;
					var currVal = vector[k];
					vector[k] = prevVal;
					prevVal = currVal;
				} while (i != k);
			}
		}


	} // class
} // namespace
