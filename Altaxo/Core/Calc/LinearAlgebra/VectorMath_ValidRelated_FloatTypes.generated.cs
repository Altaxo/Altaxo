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
		/// Returns the used length of the vector. This is one more than the highest index of the element that is different from Double.NaN.
		/// </summary>
		/// <param name="vector">The vector for which the used length has to be determined.</param>
		/// <param name="currentLength">The current length of the array. Normally values.Length, but you can provide a value less than this.</param>
		/// <returns>The used length. Elements with indices greater or equal this until <c>currentlength</c> are NaNs.</returns>
		static public int GetUsedLength(this double[] vector, int currentLength)
		{
			if(currentLength > vector.Length)
				currentLength = vector.Length;

			for (int i = currentLength - 1; i >= 0; --i)
			{
				if (!double.IsNaN(vector[i]))
					return i + 1;
			}
			return 0;
		}

		
		/// <summary>
		/// Returns the used length of the vector. This is one more than the highest index of the element that is different from NaN.
		/// </summary>
		/// <param name="vector">The vector for which the used length has to be determined.</param>
		/// <returns>The used length. Elements with indices greater or equal this until the end of the array are NaNs.</returns>
		static public int GetUsedLength(this double[] vector)
		{
			return GetUsedLength(vector, vector.Length);
		}

		/// <summary>
		/// Returns the maximum value of all the valid elements in x (nonvalid elements, i.e. NaN values are not considered).
		/// </summary>
		/// <param name="vector">The vector to search for maximum element.</param>
		/// <returns>Maximum valid element of x. Returns NaN if the array is empty or contains NaN elements only.</returns>
		public static double MaxOfValidElements(this double[]  vector)
		{
			var max = double.NaN;
			int i;
			for (i = vector.Length - 1; i >= 0; --i)
			{
				if (!double.IsNaN(max = vector[i]))
					break;
			}
			for (i = i - 1; i >= 0; --i)
			{
				var tmp = vector[i]; 
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

		/// <summary>
		/// Returns the minimum value of all the valid elements in x (nonvalid elements, i.e. NaN values are not considered).
		/// </summary>
		/// <param name="vector">The vector to search for maximum element.</param>
		/// <returns>Minimum valid element of x. Returns NaN if the array is empty or contains NaN elements only.</returns>
		public static double MinOfValidElements(this double[]  vector)
		{
			var min = double.NaN;
			int i;
			for (i = vector.Length - 1; i >= 0; --i)
			{
				if (!double.IsNaN(min = vector[i]))
					break;
			}
			for (i = i - 1; i >= 0; --i)
			{
				var tmp = vector[i]; 
				if (tmp < min)
					min = tmp;
			}
			return min;
		}


// ******************************************** Definitions for float[] *******************************************

		/// <summary>
		/// Returns the used length of the vector. This is one more than the highest index of the element that is different from Double.NaN.
		/// </summary>
		/// <param name="vector">The vector for which the used length has to be determined.</param>
		/// <param name="currentLength">The current length of the array. Normally values.Length, but you can provide a value less than this.</param>
		/// <returns>The used length. Elements with indices greater or equal this until <c>currentlength</c> are NaNs.</returns>
		static public int GetUsedLength(this float[] vector, int currentLength)
		{
			if(currentLength > vector.Length)
				currentLength = vector.Length;

			for (int i = currentLength - 1; i >= 0; --i)
			{
				if (!float.IsNaN(vector[i]))
					return i + 1;
			}
			return 0;
		}

		
		/// <summary>
		/// Returns the used length of the vector. This is one more than the highest index of the element that is different from NaN.
		/// </summary>
		/// <param name="vector">The vector for which the used length has to be determined.</param>
		/// <returns>The used length. Elements with indices greater or equal this until the end of the array are NaNs.</returns>
		static public int GetUsedLength(this float[] vector)
		{
			return GetUsedLength(vector, vector.Length);
		}

		/// <summary>
		/// Returns the maximum value of all the valid elements in x (nonvalid elements, i.e. NaN values are not considered).
		/// </summary>
		/// <param name="vector">The vector to search for maximum element.</param>
		/// <returns>Maximum valid element of x. Returns NaN if the array is empty or contains NaN elements only.</returns>
		public static double MaxOfValidElements(this float[]  vector)
		{
			var max = float.NaN;
			int i;
			for (i = vector.Length - 1; i >= 0; --i)
			{
				if (!float.IsNaN(max = vector[i]))
					break;
			}
			for (i = i - 1; i >= 0; --i)
			{
				var tmp = vector[i]; 
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

		/// <summary>
		/// Returns the minimum value of all the valid elements in x (nonvalid elements, i.e. NaN values are not considered).
		/// </summary>
		/// <param name="vector">The vector to search for maximum element.</param>
		/// <returns>Minimum valid element of x. Returns NaN if the array is empty or contains NaN elements only.</returns>
		public static double MinOfValidElements(this float[]  vector)
		{
			var min = float.NaN;
			int i;
			for (i = vector.Length - 1; i >= 0; --i)
			{
				if (!float.IsNaN(min = vector[i]))
					break;
			}
			for (i = i - 1; i >= 0; --i)
			{
				var tmp = vector[i]; 
				if (tmp < min)
					min = tmp;
			}
			return min;
		}


// ******************************************** Definitions for IReadOnlyList<double> *******************************************

		/// <summary>
		/// Returns the used length of the vector. This is one more than the highest index of the element that is different from Double.NaN.
		/// </summary>
		/// <param name="vector">The vector for which the used length has to be determined.</param>
		/// <param name="currentLength">The current length of the array. Normally values.Length, but you can provide a value less than this.</param>
		/// <returns>The used length. Elements with indices greater or equal this until <c>currentlength</c> are NaNs.</returns>
		static public int GetUsedLength(this IReadOnlyList<double> vector, int currentLength)
		{
			if(currentLength > vector.Count)
				currentLength = vector.Count;

			for (int i = currentLength - 1; i >= 0; --i)
			{
				if (!double.IsNaN(vector[i]))
					return i + 1;
			}
			return 0;
		}

		
		/// <summary>
		/// Returns the used length of the vector. This is one more than the highest index of the element that is different from NaN.
		/// </summary>
		/// <param name="vector">The vector for which the used length has to be determined.</param>
		/// <returns>The used length. Elements with indices greater or equal this until the end of the array are NaNs.</returns>
		static public int GetUsedLength(this IReadOnlyList<double> vector)
		{
			return GetUsedLength(vector, vector.Count);
		}

		/// <summary>
		/// Returns the maximum value of all the valid elements in x (nonvalid elements, i.e. NaN values are not considered).
		/// </summary>
		/// <param name="vector">The vector to search for maximum element.</param>
		/// <returns>Maximum valid element of x. Returns NaN if the array is empty or contains NaN elements only.</returns>
		public static double MaxOfValidElements(this IReadOnlyList<double>  vector)
		{
			var max = double.NaN;
			int i;
			for (i = vector.Count - 1; i >= 0; --i)
			{
				if (!double.IsNaN(max = vector[i]))
					break;
			}
			for (i = i - 1; i >= 0; --i)
			{
				var tmp = vector[i]; 
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

		/// <summary>
		/// Returns the minimum value of all the valid elements in x (nonvalid elements, i.e. NaN values are not considered).
		/// </summary>
		/// <param name="vector">The vector to search for maximum element.</param>
		/// <returns>Minimum valid element of x. Returns NaN if the array is empty or contains NaN elements only.</returns>
		public static double MinOfValidElements(this IReadOnlyList<double>  vector)
		{
			var min = double.NaN;
			int i;
			for (i = vector.Count - 1; i >= 0; --i)
			{
				if (!double.IsNaN(min = vector[i]))
					break;
			}
			for (i = i - 1; i >= 0; --i)
			{
				var tmp = vector[i]; 
				if (tmp < min)
					min = tmp;
			}
			return min;
		}


// ******************************************** Definitions for IReadOnlyList<float> *******************************************

		/// <summary>
		/// Returns the used length of the vector. This is one more than the highest index of the element that is different from Double.NaN.
		/// </summary>
		/// <param name="vector">The vector for which the used length has to be determined.</param>
		/// <param name="currentLength">The current length of the array. Normally values.Length, but you can provide a value less than this.</param>
		/// <returns>The used length. Elements with indices greater or equal this until <c>currentlength</c> are NaNs.</returns>
		static public int GetUsedLength(this IReadOnlyList<float> vector, int currentLength)
		{
			if(currentLength > vector.Count)
				currentLength = vector.Count;

			for (int i = currentLength - 1; i >= 0; --i)
			{
				if (!float.IsNaN(vector[i]))
					return i + 1;
			}
			return 0;
		}

		
		/// <summary>
		/// Returns the used length of the vector. This is one more than the highest index of the element that is different from NaN.
		/// </summary>
		/// <param name="vector">The vector for which the used length has to be determined.</param>
		/// <returns>The used length. Elements with indices greater or equal this until the end of the array are NaNs.</returns>
		static public int GetUsedLength(this IReadOnlyList<float> vector)
		{
			return GetUsedLength(vector, vector.Count);
		}

		/// <summary>
		/// Returns the maximum value of all the valid elements in x (nonvalid elements, i.e. NaN values are not considered).
		/// </summary>
		/// <param name="vector">The vector to search for maximum element.</param>
		/// <returns>Maximum valid element of x. Returns NaN if the array is empty or contains NaN elements only.</returns>
		public static double MaxOfValidElements(this IReadOnlyList<float>  vector)
		{
			var max = float.NaN;
			int i;
			for (i = vector.Count - 1; i >= 0; --i)
			{
				if (!float.IsNaN(max = vector[i]))
					break;
			}
			for (i = i - 1; i >= 0; --i)
			{
				var tmp = vector[i]; 
				if (tmp > max)
					max = tmp;
			}
			return max;
		}

		/// <summary>
		/// Returns the minimum value of all the valid elements in x (nonvalid elements, i.e. NaN values are not considered).
		/// </summary>
		/// <param name="vector">The vector to search for maximum element.</param>
		/// <returns>Minimum valid element of x. Returns NaN if the array is empty or contains NaN elements only.</returns>
		public static double MinOfValidElements(this IReadOnlyList<float>  vector)
		{
			var min = float.NaN;
			int i;
			for (i = vector.Count - 1; i >= 0; --i)
			{
				if (!float.IsNaN(min = vector[i]))
					break;
			}
			for (i = i - 1; i >= 0; --i)
			{
				var tmp = vector[i]; 
				if (tmp < min)
					min = tmp;
			}
			return min;
		}


	} // class
} // namespace
