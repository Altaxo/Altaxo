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

// ********************** Definitions for double[] to double[] *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(double[] sourceVector, double[] destinationVector)
		{
			if (sourceVector.Length != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Length);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(double[] sourceVector, int sourceStartIndex, double[] destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for IReadOnlyList<double> to double[] *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(IReadOnlyList<double> sourceVector, double[] destinationVector)
		{
			if (sourceVector.Count != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Count);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(IReadOnlyList<double> sourceVector, int sourceStartIndex, double[] destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for double[] to IVector<double> *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(double[] sourceVector, IVector<double> destinationVector)
		{
			if (sourceVector.Length != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Length);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(double[] sourceVector, int sourceStartIndex, IVector<double> destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for IReadOnlyList<double> to IVector<double> *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(IReadOnlyList<double> sourceVector, IVector<double> destinationVector)
		{
			if (sourceVector.Count != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Count);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(IReadOnlyList<double> sourceVector, int sourceStartIndex, IVector<double> destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for float[] to double[] *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(float[] sourceVector, double[] destinationVector)
		{
			if (sourceVector.Length != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Length);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(float[] sourceVector, int sourceStartIndex, double[] destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for IReadOnlyList<float> to double[] *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(IReadOnlyList<float> sourceVector, double[] destinationVector)
		{
			if (sourceVector.Count != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Count);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(IReadOnlyList<float> sourceVector, int sourceStartIndex, double[] destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for float[] to IVector<double> *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(float[] sourceVector, IVector<double> destinationVector)
		{
			if (sourceVector.Length != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Length);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(float[] sourceVector, int sourceStartIndex, IVector<double> destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for IReadOnlyList<float> to IVector<double> *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(IReadOnlyList<float> sourceVector, IVector<double> destinationVector)
		{
			if (sourceVector.Count != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Count);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(IReadOnlyList<float> sourceVector, int sourceStartIndex, IVector<double> destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for float[] to float[] *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(float[] sourceVector, float[] destinationVector)
		{
			if (sourceVector.Length != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Length);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(float[] sourceVector, int sourceStartIndex, float[] destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for IReadOnlyList<float> to float[] *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(IReadOnlyList<float> sourceVector, float[] destinationVector)
		{
			if (sourceVector.Count != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Count);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(IReadOnlyList<float> sourceVector, int sourceStartIndex, float[] destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for float[] to IVector<float> *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(float[] sourceVector, IVector<float> destinationVector)
		{
			if (sourceVector.Length != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Length);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(float[] sourceVector, int sourceStartIndex, IVector<float> destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for IReadOnlyList<float> to IVector<float> *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(IReadOnlyList<float> sourceVector, IVector<float> destinationVector)
		{
			if (sourceVector.Count != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Count);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(IReadOnlyList<float> sourceVector, int sourceStartIndex, IVector<float> destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for int[] to double[] *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(int[] sourceVector, double[] destinationVector)
		{
			if (sourceVector.Length != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Length);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(int[] sourceVector, int sourceStartIndex, double[] destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for IReadOnlyList<int> to double[] *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(IReadOnlyList<int> sourceVector, double[] destinationVector)
		{
			if (sourceVector.Count != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Count);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(IReadOnlyList<int> sourceVector, int sourceStartIndex, double[] destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for int[] to IVector<double> *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(int[] sourceVector, IVector<double> destinationVector)
		{
			if (sourceVector.Length != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Length);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(int[] sourceVector, int sourceStartIndex, IVector<double> destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for IReadOnlyList<int> to IVector<double> *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(IReadOnlyList<int> sourceVector, IVector<double> destinationVector)
		{
			if (sourceVector.Count != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Count);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(IReadOnlyList<int> sourceVector, int sourceStartIndex, IVector<double> destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for int[] to float[] *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(int[] sourceVector, float[] destinationVector)
		{
			if (sourceVector.Length != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Length);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(int[] sourceVector, int sourceStartIndex, float[] destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for IReadOnlyList<int> to float[] *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(IReadOnlyList<int> sourceVector, float[] destinationVector)
		{
			if (sourceVector.Count != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Count);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(IReadOnlyList<int> sourceVector, int sourceStartIndex, float[] destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for int[] to IVector<float> *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(int[] sourceVector, IVector<float> destinationVector)
		{
			if (sourceVector.Length != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Length);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(int[] sourceVector, int sourceStartIndex, IVector<float> destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for IReadOnlyList<int> to IVector<float> *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(IReadOnlyList<int> sourceVector, IVector<float> destinationVector)
		{
			if (sourceVector.Count != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Count);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(IReadOnlyList<int> sourceVector, int sourceStartIndex, IVector<float> destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for int[] to int[] *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(int[] sourceVector, int[] destinationVector)
		{
			if (sourceVector.Length != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Length);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(int[] sourceVector, int sourceStartIndex, int[] destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for IReadOnlyList<int> to int[] *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(IReadOnlyList<int> sourceVector, int[] destinationVector)
		{
			if (sourceVector.Count != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Count);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(IReadOnlyList<int> sourceVector, int sourceStartIndex, int[] destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for int[] to IVector<int> *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(int[] sourceVector, IVector<int> destinationVector)
		{
			if (sourceVector.Length != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Length);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(int[] sourceVector, int sourceStartIndex, IVector<int> destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


// ********************** Definitions for IReadOnlyList<int> to IVector<int> *************************************


		/// <summary>
		/// Copies the source vector to the destination vector. Both vectors must have the same length.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="destinationVector">The destination vector.</param>
		public static void Copy(IReadOnlyList<int> sourceVector, IVector<int> destinationVector)
		{
			if (sourceVector.Count != destinationVector.Length)
				throw new ArgumentException("source and destination vector have unequal length!");

			Copy(sourceVector, 0, destinationVector, 0, sourceVector.Count);
		}
	

		/// <summary>
		/// Copies elements of a source vector to a destination vector.
		/// </summary>
		/// <param name="sourceVector">The source vector.</param>
		/// <param name="sourceStartIndex">First element of the source vector to copy.</param>
		/// <param name="destinationVector">The destination vector.</param>
		/// <param name="destinationStartIndex">First element of the destination vector to copy to.</param>
		/// <param name="count">Number of elements to copy.</param>
		public static void Copy(IReadOnlyList<int> sourceVector, int sourceStartIndex, IVector<int> destinationVector, int destinationStartIndex, int count)
		{
			for (int i = 0; i < count; ++i)
				destinationVector[i + destinationStartIndex] = sourceVector[i + sourceStartIndex];
		}


		


	} // class
} // namespace
