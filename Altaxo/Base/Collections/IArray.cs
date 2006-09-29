using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Collections
{
  public interface IArray<T>
  {
    T this[int i] { get; set; }
    int Count { get; }

  }
}
