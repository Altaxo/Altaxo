using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Collections
{
  /// <summary>
  /// Interface that combines a read-only dictionary and a list of its values.
  /// </summary>
  /// <typeparam name="TKey">The type of the key.</typeparam>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  /// <seealso cref="System.Collections.Generic.IReadOnlyList&lt;TValue&gt;" />
  /// <seealso cref="System.Collections.Generic.IReadOnlyDictionary&lt;TKey, TValue&gt;" />
  public interface IReadOnlyListDictionary<TKey, TValue> : IReadOnlyList<TValue>, IReadOnlyDictionary<TKey, TValue>
  {
  }
}

