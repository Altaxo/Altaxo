/*******************************************************************************
* Author    :  Angus Johnson                                                   *
* Date      :  7 October 2025                                                 *
* Website   :  http://www.angusj.com                                           *
* Copyright :  Angus Johnson 2010-2025                                         *
* Purpose   :  A pool of reusable vertex objects.                    *
* Thanks    :  Special thanks to Thong Nguyen, Guus Kuiper, Phil Stopford,     *
*           :  and Daniel Gosnell for their invaluable assistance with C#.     *
* License   :  http://www.boost.org/LICENSE_1_0.txt                            *
*******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if USINGZ
namespace Clipper2ZLib
#else
namespace Clipper2Lib
#endif
{
  /// <summary>
  /// A pool of reusable Vertex objects
  /// </summary>
  internal class VertexPoolList : PooledList<Vertex>
  {
    /// <summary>
    /// Adds a vertex to the pool and returns the pooled instance.
    /// </summary>
    /// <param name="point">The vertex position.</param>
    /// <param name="flags">The vertex flags.</param>
    /// <param name="prev">The previous vertex in the linked sequence.</param>
    /// <returns>The pooled vertex instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vertex Add(Point64 point, VertexFlags flags, Vertex? prev)
    {
      TryGrow();
      Vertex poolVtx = _items[_size];
      if (poolVtx == null)
      {
        poolVtx = new Vertex(point, flags, prev);
        _items[_size] = poolVtx;
      }
      else
      {
        //reuse already allocated vertex
        poolVtx.pt = point;
        poolVtx.flags = flags;
        poolVtx.prev = prev;
        poolVtx.next = null;
      }
      _size++;
      return poolVtx;
    }
  }

  /// <summary>
  /// A pool of reusable OutPt objects
  /// </summary>
  internal class OutPtPoolList : PooledList<OutPt>
  {
    /// <summary>
    /// Gets or sets a value indicating whether the output point pool is used.
    /// </summary>
    public static bool UseOutPtPool = true;

    /// <summary>
    /// Adds an output point to the pool and returns the pooled instance.
    /// </summary>
    /// <param name="pt">The point to add.</param>
    /// <param name="outrec">The owning output record.</param>
    /// <returns>The pooled output point instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OutPt Add(Point64 pt, OutRec outrec)
    {
      TryGrow();
      OutPt poolPt = _items[_size];
      if (poolPt == null)
      {
        poolPt = new OutPt(pt, outrec);
        _items[_size] = poolPt;
      }
      else
      {
        //reuse already allocated OutPt
        poolPt.pt = pt;
        poolPt.outrec = outrec;
        poolPt.next = poolPt;
        poolPt.prev = poolPt;
        poolPt.horz = null;
      }
      _size++;
      outrec.outPtCount++;
      return poolPt;
    }
  }

  /// <summary>
  /// A pool of reusable OutRec objects
  /// </summary>
  internal class OutRecPoolList : PooledList<OutRec>
  {
    /// <summary>
    /// Adds an output record to the pool and returns the pooled instance.
    /// </summary>
    /// <returns>The pooled output record instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OutRec Add()
    {
      TryGrow();
      OutRec outRec = _items[_size];
      if (outRec == null)
      {
        outRec = new OutRec();
        _items[_size] = outRec;
      }
      else
      {
        //reuse already allocated OutRec
        outRec.idx = 0;
        outRec.outPtCount = 0;
        outRec.owner = null;
        outRec.frontEdge = null;
        outRec.backEdge = null;
        outRec.pts = null;
        outRec.bounds = new Rect64();
        outRec.path = new Path64();
        outRec.polypath = null;
        outRec.isOpen = false;
        outRec.splits?.Clear();
        outRec.recursiveSplit = null;
      }
      _size++;
      return outRec;
    }

    //tombStone marks cleared OutRec in the pool
    /// <summary>
    /// Marks pooled output records that have been cleared.
    /// </summary>
    private static readonly Path64 tombStone = new Path64();

    /// <inheritdoc />
    public override void Clear()
    {
      base.Clear();
      //we only clear refs here to allow GC
      for (int i = 0; i < _items.Length; i++)
      {
        OutRec active = _items[i];
        if (active == null || active.path == tombStone) break;
        //we use path as tombStone because it is not nullable
        active.path = tombStone;
        active.owner = null;
        active.frontEdge = null;
        active.backEdge = null;
        active.pts = null;
        active.polypath = null;
        active.recursiveSplit = null;
      }
    }
  }

  /// <summary>
  /// A pool of reusable HorzJoin objects
  /// </summary>
  internal class HorzJoinPoolList : PooledList<HorzJoin>
  {
    /// <summary>
    /// Adds a horizontal join to the pool and returns the pooled instance.
    /// </summary>
    /// <param name="ltor">The left-to-right endpoint.</param>
    /// <param name="rtol">The right-to-left endpoint.</param>
    /// <returns>The pooled horizontal join instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HorzJoin Add(OutPt ltor, OutPt rtol)
    {
      TryGrow();
      HorzJoin hJoin = _items[_size];
      if (hJoin == null)
      {
        hJoin = new HorzJoin(ltor, rtol);
        _items[_size] = hJoin;
      }
      else
      {
        //reuse already allocated HorzJoin
        hJoin.op1 = ltor;
        hJoin.op2 = rtol;
      }
      _size++;
      return hJoin;
    }
  }

  /// <summary>
  /// A List that pools objects added to it for reuse.
  /// Indexing, growing and enumeration implementation is identical to <see cref="System.Collections.Generic.List{T}"/>.
  /// The pooled list reuses allocated reference objects. Operations are limited to read, add and clear.
  /// </summary>
  /// <typeparam name="T">The pooled reference type.</typeparam>
  internal abstract class PooledList<T> : IReadOnlyList<T> where T : class
  {
    private const int DefaultCapacity = 4;

    /// <summary>
    /// The backing array that stores pooled items.
    /// </summary>
    protected T[] _items;

    /// <summary>
    /// The number of active items in the pool.
    /// </summary>
    protected int _size;

    /// <summary>
    /// Gets the pooled item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to retrieve.</param>
    /// <returns>The pooled item at the specified index.</returns>
    public T this[int index]
    {
      get
      {
        if ((uint) index >= (uint) _size)
        {
          throw new ArgumentOutOfRangeException("index must be greater or equal to zero and less than the size of the collection");
        };
        return _items[index];
      }
    }

    /// <summary>
    /// Gets the number of active items in the pool.
    /// </summary>
    public int Count => _size;

    /// <summary>
    /// Initializes a new instance of the <see cref="PooledList{T}"/> class.
    /// </summary>
    public PooledList()
    {
      _items = Array.Empty<T>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PooledList{T}"/> class with the specified capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity.</param>
    public PooledList(int capacity)
    {
      _items = new T[capacity];
    }

    /// <summary>
    /// Gets or sets the backing array capacity.
    /// </summary>
    public int Capacity
    {
      get => _items.Length;
      set
      {
        if (value > _items.Length)
        {
          //pooled list is reused by definition, using exact capacity does not make sense.
          //Implicitly use EnsureCapacity semantics by rounding to next power of two
          value = (int)RoundUpToPowerOf2((uint)value);
          T[] newItems = new T[value];
          if (_size > 0)
          {
            Array.Copy(_items, newItems, _size);
          }
          _items = newItems;
        }
      }
    }

    //in DOTNET6 and newer this is available in BitOperations
    /// <summary>
    /// Rounds the specified value up to the next power of two.
    /// </summary>
    /// <param name="value">The value to round.</param>
    /// <returns>The rounded value.</returns>
    private static uint RoundUpToPowerOf2(uint value)
    {
      // Based on https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
      --value;
      value |= value >> 1;
      value |= value >> 2;
      value |= value >> 4;
      value |= value >> 8;
      value |= value >> 16;
      return value + 1;
    }

    /// <summary>
    /// Ensures that the pool can hold at least the specified number of items.
    /// </summary>
    /// <param name="capacity">The minimum required capacity.</param>
    public void EnsureCapacity(int capacity) { Capacity = capacity; }

    /// <summary>
    /// Grows the backing storage when adding another item would exceed the current capacity.
    /// </summary>
    protected void TryGrow()
    {
      int newSize = _size + 1;
      if (newSize > _items.Length)
      {
        //grow the array
        int newCapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;
        Capacity = newCapacity;
      }
    }

    /// <summary>
    /// Clears the active item count while keeping pooled objects for reuse.
    /// </summary>
    public virtual void Clear()
    {
      //unlike List<T>, DO NOT null the objects in the list, even if they are reference types. We reuse them
      _size = 0;
    }

    /// <summary>
    /// Returns an enumerator for the active items in the pool.
    /// </summary>
    /// <returns>An enumerator over the active pooled items.</returns>
    public ListEnumerator<T> GetEnumerator()
    {
      return new ListEnumerator<T>(this);
    }

    /// <inheritdoc />
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return new ListEnumerator<T>(this);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new ListEnumerator<T>(this);
    }

    /// <summary>
    /// Enumerates the active items of a <see cref="PooledList{T}"/>.
    /// </summary>
    /// <typeparam name="T2">The element type.</typeparam>
    internal struct ListEnumerator<T2> : IEnumerator<T2> where T2 : class
    {
      private readonly PooledList<T2> _list;
      private int _index;
      private T2? _current;
      
      /// <summary>
      /// Initializes a new instance of the <see cref="ListEnumerator{T2}"/> struct.
      /// </summary>
      /// <param name="list">The pooled list to enumerate.</param>
      public ListEnumerator(PooledList<T2> list)
      {
        _list = list;
        _index = 0;
        _current = default;
      }

      /// <summary>
      /// Gets the current item in the enumeration.
      /// </summary>
      public T2 Current => _current!;

      /// <inheritdoc />
      object IEnumerator.Current => _current!;

      /// <summary>
      /// Releases resources used by the enumerator.
      /// </summary>
      public void Dispose()
      {
      }

      /// <summary>
      /// Advances the enumerator to the next item.
      /// </summary>
      /// <returns><see langword="true"/> if the enumerator advanced to the next item; otherwise, <see langword="false"/>.</returns>
      public bool MoveNext()
      {
        int count = _list._size;
        if ((uint) _index < (uint) count)
        {
          _current = _list[_index];
          _index++;
          return true;
        }
        return MoveNextRare(count);
      }

      /// <summary>
      /// Handles the end-of-sequence transition for the enumerator.
      /// </summary>
      /// <param name="count">The number of active items in the list.</param>
      /// <returns><see langword="false"/>.</returns>
      private bool MoveNextRare(int count)
      {
        _index = count + 1;
        _current = default;
        return false;
      }

      /// <summary>
      /// Resets the enumerator to its initial position.
      /// </summary>
      public void Reset()
      {
        _index = 0;
        _current = default;
      }
    }
  }
} // namespace
