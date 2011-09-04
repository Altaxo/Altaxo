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

namespace Altaxo.Calc.LinearAlgebra
{
	public interface INumericSequence
	{
		/// <summary>Gets the element of the sequence at index i.</summary>
		/// <value>The element at index i.</value>
		double this[int i] { get; }
	}

	/// <summary>
	/// Interface for a read-only vector of double values. The first valid index of this vector is 0, the last one in (<see cref="Length"/>-1).
	/// </summary>
	public interface IROVector : INumericSequence
	{
		/// <summary>The number of elements of this vector.</summary>
		int Length { get; }  // change this later to length property
	}

	/// <summary>
	/// Interface for a readable and writeable vector of double values.
	/// </summary>
	public interface IVector : IROVector
	{
		/// <summary>Read/write Accessor for the element at index i.</summary>
		/// <value>The element at index i.</value>
		new double this[int i] { get; set; }
	}

	/// <summary>
	/// IRightExtensibleMatrix extends IMatrix in a way that another matrix of appropriate dimensions
	/// can be appended to the right of the matrix. 
	/// </summary>
	public interface IExtensibleVector : IVector
	{
		/// <summary>
		/// Append vector a to the end of this vector.
		/// </summary>
		/// <param name="a">The vector to append.</param>
		void Append(IROVector a);
	}


	public abstract class AbstractRODoubleVector : IROVector, System.Collections.Generic.IList<double>
	{
		static public implicit operator AbstractRODoubleVector(double[] src)
		{
			return new RODoubleVector(src);
		}

		#region IROVector Members

		public abstract int Length
		{
			get;
		}

		public abstract double this[int idx]
		{
			get;
			set;
		}

		#endregion

		#region INumericSequence Members



		protected abstract double GetElementAt(int i);

		#endregion

		public int IndexOf(double item)
		{
			int len = Length;
			for (int i = 0; i < len; i++)
				if (this[i] == item)
					return i;
			return -1;
		}

		public void Insert(int index, double item)
		{
			throw new NotImplementedException("This is a read-only vector");
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException("This is a read-only vector");
		}

		public void Add(double item)
		{
			throw new NotImplementedException("This is a read-only vector");
		}

		public void Clear()
		{
			throw new NotImplementedException("This is a read-only vector");
		}

		public bool Contains(double item)
		{
			int len = Length;
			for (int i = 0; i < len; i++)
				if (this[i] == item)
					return true;

			return false;
		}

		public void CopyTo(double[] array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (Length + arrayIndex > array.Length)
				throw new ArgumentException("Provided array is too short");
			int len = Length;
			for (int i = 0; i < len; i++)
				array[i + arrayIndex] = this[i];
		}

		public int Count
		{
			get { return Length; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool Remove(double item)
		{
			throw new NotImplementedException("This is a read-only vector");
		}

		public System.Collections.Generic.IEnumerator<double> GetEnumerator()
		{
			int len = this.Count;
			for (int i = 0; i < len; i++)
				yield return this[i];
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			int len = this.Count;
			for (int i = 0; i < len; i++)
				yield return this[i];
		}
	}


	public abstract class AbstractDoubleVector : AbstractRODoubleVector, IVector
	{
		protected abstract void SetElementAt(int i, double value);
	}


	public class RODoubleVector : AbstractRODoubleVector
	{
		private double[] _data;

		public RODoubleVector(double[] array)
		{
			_data = array;
		}



		static public implicit operator RODoubleVector(double[] src)
		{
			return new RODoubleVector(src);
		}



		#region IROVector Members

		public override int Length
		{
			get { return _data.Length; }
		}

		public override double this[int idx]
		{
			get
			{
				return _data[idx];
			}
			set
			{
				throw new NotImplementedException("This is a read-only vector");
			}
		}

		#endregion

		#region INumericSequence Members

		protected override double GetElementAt(int i)
		{
			return _data[i];
		}

		#endregion
	}
}
