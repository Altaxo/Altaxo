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
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Fourier.Windows
{
	public abstract class AbstractWindow
	{
		private int _count;
		private bool _periodic;

		public AbstractWindow(int count, bool periodic)
		{
			if (count <= 0)
				throw new ArgumentException("Width of the window must not be zero or negative");

			_count = count;
			_periodic = periodic;
		}

		protected abstract void InternalCompute(IVector array, bool periodic);


		public double[] AsDoubleArray()
		{
			double[] result = new double[_count];
			if (_count == 1)
				result[0] = 1;
			else
				InternalCompute(VectorMath.ToVector(result), _periodic);

			return result;
		}

		public IROVector AsROVector()
		{
			return VectorMath.ToROVector(AsDoubleArray());
		}

		public IVector AsVector()
		{
			return VectorMath.ToVector(AsDoubleArray());
		}

		public void Compute(double[] array, bool periodic)
		{
			if (array == null)
				throw new ArgumentNullException("array is null");
			if (array.Length == 0)
				throw new ArgumentException("array length is null");
			InternalCompute(VectorMath.ToVector(array), periodic);
		}

		public void Compute(double[] array, int startidx, int count, bool periodic)
		{
			if (array == null)
				throw new ArgumentNullException("array is null");
			if (array.Length == 0)
				throw new ArgumentException("array length is null");
			if (startidx < 0)
				throw new ArgumentException("startidx is negative");
			if (count < 1)
				throw new ArgumentException("count is null or negative");
			if ((startidx + count) > array.Length)
				throw new ArgumentException("startidx+count exceeds the length of the array");

			InternalCompute(VectorMath.ToVector(array, startidx, count), periodic);
		}

		public void Compute(IVector array, bool periodic)
		{
			if (array == null)
				throw new ArgumentNullException("array is null");
			if (array.Length == 0)
				throw new ArgumentException("array length is null");
			InternalCompute(array, periodic);
		}

		public void Compute(IVector array, int startidx, int count, bool periodic)
		{
			if (array == null)
				throw new ArgumentNullException("array is null");
			if (array.Length == 0)
				throw new ArgumentException("array length is null");
			if (startidx < 0)
				throw new ArgumentException("startidx is negative");
			if (count < 1)
				throw new ArgumentException("count is null or negative");
			if ((startidx + count) > array.Length)
				throw new ArgumentException("startidx+count exceeds the length of the array");

			InternalCompute(VectorMath.ToVector(array, startidx, count), periodic);
		}


	}

}
