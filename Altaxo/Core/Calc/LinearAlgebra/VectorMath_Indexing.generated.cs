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

        public static double[] ElementsWhere(this double[] array, bool[] condition)
        {
          return array.Where((x, i) => condition[i]).ToArray();
        }
		
        public static double[] ElementsAt(this double[] array, int[] indices)
        {
          return indices.Select(idx => array[idx]).ToArray();
        }


// ******************************************** Definitions for IReadOnlyList<double> *******************************************

        public static IReadOnlyList<double> ElementsWhere(this IReadOnlyList<double> array, bool[] condition)
        {
          return array.Where((x, i) => condition[i]).ToArray();
        }
		
        public static IReadOnlyList<double> ElementsAt(this IReadOnlyList<double> array, int[] indices)
        {
          return indices.Select(idx => array[idx]).ToArray();
        }


// ******************************************** Definitions for float[] *******************************************

        public static float[] ElementsWhere(this float[] array, bool[] condition)
        {
          return array.Where((x, i) => condition[i]).ToArray();
        }
		
        public static float[] ElementsAt(this float[] array, int[] indices)
        {
          return indices.Select(idx => array[idx]).ToArray();
        }


// ******************************************** Definitions for IReadOnlyList<float> *******************************************

        public static IReadOnlyList<float> ElementsWhere(this IReadOnlyList<float> array, bool[] condition)
        {
          return array.Where((x, i) => condition[i]).ToArray();
        }
		
        public static IReadOnlyList<float> ElementsAt(this IReadOnlyList<float> array, int[] indices)
        {
          return indices.Select(idx => array[idx]).ToArray();
        }


// ******************************************** Definitions for int[] *******************************************

        public static int[] ElementsWhere(this int[] array, bool[] condition)
        {
          return array.Where((x, i) => condition[i]).ToArray();
        }
		
        public static int[] ElementsAt(this int[] array, int[] indices)
        {
          return indices.Select(idx => array[idx]).ToArray();
        }


// ******************************************** Definitions for IReadOnlyList<int> *******************************************

        public static IReadOnlyList<int> ElementsWhere(this IReadOnlyList<int> array, bool[] condition)
        {
          return array.Where((x, i) => condition[i]).ToArray();
        }
		
        public static IReadOnlyList<int> ElementsAt(this IReadOnlyList<int> array, int[] indices)
        {
          return indices.Select(idx => array[idx]).ToArray();
        }


	} // class
} // namespace
