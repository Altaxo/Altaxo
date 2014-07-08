#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using Altaxo.Calc.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.Fourier.Windows
{
	/// <summary>
	/// Common interface for 2D window functions to apply before.
	/// </summary>
	public interface IWindows2D
	{
		/// <summary>
		/// Applies the window function to the specified matrix <paramref name="m"/> by multiplying each matrix element with a factor.
		/// </summary>
		/// <param name="m">The matrix to modify.</param>
		void Apply(IMatrix m);
	}

	/// <summary>
	/// Implements a 2D Hanning window for data pretreatment prior to a 2D Fourier transformation.
	/// </summary>
	public class HanningWindow2D : IWindows2D
	{
		/// <summary>
		/// Applies the Hanning window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
		/// </summary>
		/// <param name="m">The matrix to modify.</param>
		public void Apply(IMatrix m)
		{
			Application(m);
		}

		/// <summary>
		/// Applies the Hanning window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
		/// </summary>
		/// <param name="m">The matrix to modify.</param>
		public static void Application(IMatrix m)
		{
			var rows = m.Rows;
			var cols = m.Columns;

			double facI = 2.0 / (rows - 1);
			double facJ = 2.0 / (cols - 1);

			for (int i = 0; i < rows; ++i)
			{
				double ri = i * facI - 1;
				for (int j = 0; j < cols; ++j)
				{
					double rj = j * facJ - 1;
					double rij = Math.Sqrt(ri * ri + rj * rj);
					double windowFunc = rij <= 1 ? 0.5 * (1 + Math.Cos(Math.PI * rij)) : 0;
					m[i, j] *= windowFunc;
				}
			}
		}
	}
}