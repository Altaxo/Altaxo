using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Represents a read-only vector of type T.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IROVector<T>
  {
    /// <summary>
    /// The value stored at position <code>i</code>.
    /// </summary>
    T this[int i] { get; }

    /// <summary>
    /// Number of elements.
    /// </summary>
    int Count { get; }
  }

  public interface IVector<T>  : IROVector<T>
  {
    new T this[int i] { get;  set; }
  }
}
