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

using Altaxo.Calc.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Fourier.Windows
{
	public class WelchWindow : AbstractWindow
	{
		public WelchWindow(int count, bool periodic)
			: base(count, periodic)
		{
		}

		protected override void InternalCompute(IVector<double> array, bool periodic)
		{
			int len = array.Length;
			int N = periodic ? len : len - 1;
			for (int i = 0; i < len; ++i)
				array[i] = 1 - RMath.Pow2((2 * i - N) / (double)N);
		}

		/// <summary>
		/// Returns the window as an array of doubles.
		/// </summary>
		/// <param name="count">Length of the window.</param>
		/// <param name="periodic">Periodic conditions, see remarks in the base class.</param>
		/// <returns>The window as array of doubles.</returns>
		public static double[] AsDoubleArray(int count, bool periodic)
		{
			return new WelchWindow(count, periodic).AsDoubleArray();
		}

		/// <summary>
		/// Returns the window as an read only vector.
		/// </summary>
		/// <param name="count">Length of the window.</param>
		/// <param name="periodic">Periodic conditions, see remarks in the base class.</param>
		/// <returns>The window as read only vector.</returns>
		public static IROVector<double> AsROVector(int count, bool periodic)
		{
			return new WelchWindow(count, periodic).AsROVector();
		}

		/// <summary>
		/// Returns the window as writeable vector.
		/// </summary>
		/// <param name="count">Length of the window.</param>
		/// <param name="periodic">Periodic conditions, see remarks in the base class.</param>
		/// <returns>The window as writeable vector.</returns>
		public static IVector<double> AsVector(int count, bool periodic)
		{
			return new WelchWindow(count, periodic).AsVector();
		}
	}
}