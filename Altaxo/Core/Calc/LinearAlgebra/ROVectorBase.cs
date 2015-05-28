using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
	public abstract class ROVectorBase<T> : IList<T> // TODO for net>4.0, use IReadOnlyList<T> instead of IList<T>
	{
		#region IList<T>

		public abstract T this[int index] { get; set; }

		public abstract int Count { get; }

		public int IndexOf(T item)
		{
			var cnt = Count;
			for (int i = 0; i < Count; ++i)
				if (item.Equals(this[i]))
					return i;

			return -1;
		}

		public bool Contains(T item)
		{
			var cnt = Count;
			for (int i = 0; i < Count; ++i)
				if (item.Equals(this[i]))
					return true;

			return false;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException("arrayIndex is < 0");

			var cnt = Count;

			if (!(arrayIndex + cnt <= array.Length))
				throw new ArgumentOutOfRangeException("Array too small for the provided data.");

			for (int i = 0; i < cnt; ++i)
				array[i + arrayIndex] = this[i];
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public IEnumerator<T> GetEnumerator()
		{
			var cnt = Count;
			for (int i = 0; i < cnt; ++i)
				yield return this[i];
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			var cnt = Count;
			for (int i = 0; i < cnt; ++i)
				yield return this[i];
		}

		public void Insert(int index, T item)
		{
			throw new InvalidOperationException("This list is read-only.");
		}

		public void RemoveAt(int index)
		{
			throw new InvalidOperationException("This list is read-only.");
		}

		public void Add(T item)
		{
			throw new InvalidOperationException("This list is read-only.");
		}

		public void Clear()
		{
			throw new InvalidOperationException("This list is read-only.");
		}

		public bool Remove(T item)
		{
			throw new InvalidOperationException("This list is read-only.");
		}

		#endregion IList<T>
	}
}