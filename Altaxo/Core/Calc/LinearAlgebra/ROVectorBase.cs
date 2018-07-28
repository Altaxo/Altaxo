using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
  public abstract class ROVectorBase<T> : IROVector<T>
  {
    public abstract T this[int index] { get; set; }

    public abstract int Count { get; }

    public int Length { get { return Count; } }

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
      if (array == null)
        throw new ArgumentNullException(nameof(array));
      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException(nameof(arrayIndex), "arrayIndex is < 0");
      var cnt = Count;
      if (!(arrayIndex + cnt <= array.Length))
        throw new ArgumentOutOfRangeException("Array too small for the provided data.");

      for (int i = 0; i < cnt; ++i)
        array[i + arrayIndex] = this[i];
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
  }
}
