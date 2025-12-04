#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Represents a dictionary that holds weak references to its keys, allowing keys to be garbage collected when no longer referenced elsewhere.
  /// </summary>
  /// <typeparam name="TKey">Type of the key.</typeparam>
  /// <typeparam name="TValue">Type of the value.</typeparam>
  public class WeakDictionary<TKey, TValue> where TKey : notnull
  {
    /// <summary>
    /// Internal dictionary mapping weak keys to values.
    /// </summary>
    private Dictionary<object, TValue> _dict = new Dictionary<object, TValue>();

    /// <summary>
    /// Represents a weak reference to a key, storing its hash code for dictionary operations.
    /// </summary>
    protected class WeakKey<T> : WeakReference
    {
      private int _hash;

      /// <summary>
      /// Initializes a new instance of the <see cref="WeakKey{T}"/> class.
      /// </summary>
      /// <param name="obj">The key object.</param>
      public WeakKey(T obj)
        : base(obj)
      {
        _hash = obj?.GetHashCode() ?? 0;
      }

      /// <inheritdoc/>
      public override bool Equals(object? obj)
      {
        if (obj is WeakReference)
          return object.ReferenceEquals(obj, this);
        else if (obj is { } _)
          return obj.Equals(Target);
        else
          return false;
      }

      /// <inheritdoc/>
      public override int GetHashCode()
      {
        return _hash;
      }
    }

    /// <summary>
    /// Helper class for cleaning up weak dictionary entries when keys are garbage collected.
    /// </summary>
    internal class CleanerRef
    {
      /// <summary>
      /// Finalizer to free the GCHandle.
      /// </summary>
      ~CleanerRef()
      {
        if (handle.IsAllocated)
          handle.Free();
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CleanerRef"/> class.
      /// </summary>
      /// <param name="cleaner">The cleaner instance.</param>
      /// <param name="dictionary">The weak dictionary to clean.</param>
      public CleanerRef(WeakDictionaryCleaner cleaner, WeakDictionary<TKey, TValue> dictionary)
      {
        handle = GCHandle.Alloc(cleaner, GCHandleType.WeakTrackResurrection);
        Dictionary = dictionary;
      }

      /// <summary>
      /// Gets a value indicating whether the reference is alive.
      /// </summary>
      public bool IsAlive
      {
        get { return handle.IsAllocated && handle.Target is not null; }
      }

      /// <summary>
      /// Gets the target object if alive; otherwise, null.
      /// </summary>
      public object? Target
      {
        get { return IsAlive ? handle.Target : null; }
      }

      private GCHandle handle;
      public WeakDictionary<TKey, TValue> Dictionary;
    }

    /// <summary>
    /// Cleaner class that removes entries from the dictionary when keys are garbage collected.
    /// </summary>
    internal class WeakDictionaryCleaner
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="WeakDictionaryCleaner"/> class.
      /// </summary>
      /// <param name="dict">The weak dictionary to clean.</param>
      public WeakDictionaryCleaner(WeakDictionary<TKey, TValue> dict)
      {
        refs.Add(new CleanerRef(this, dict));
      }

      /// <summary>
      /// Finalizer that triggers cleanup of garbage collected entries.
      /// </summary>
      ~WeakDictionaryCleaner()
      {
        foreach (var cleanerRef in refs)
        {
          if (cleanerRef.Target == this)
          {
            cleanerRef.Dictionary.ClearGcedEntries();
            refs.Remove(cleanerRef);
            break;
          }
        }
      }

      private static readonly List<CleanerRef> refs = new List<CleanerRef>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakDictionary{TKey, TValue}"/> class.
    /// </summary>
    public WeakDictionary()
    {
    }

    /// <summary>
    /// Removes entries whose keys have been garbage collected.
    /// </summary>
    protected void ClearGcedEntries()
    {
      var toRemove = new List<KeyValuePair<object, TValue>>();
      foreach (var entry in _dict)
      {
        if (ShouldEntryBeRemoved((WeakKey<TKey>)entry.Key, entry.Value))
          toRemove.Add(entry);
      }
      foreach (var entry in toRemove)
        _dict.Remove(entry.Key);

      if (_dict.Count != 0)
        new WeakDictionaryCleaner(this); // create again a new instance of WeakDictionaryCleaner without a reference so it can be garbage collected
    }

    /// <summary>
    /// Determines whether an entry should be removed based on whether its key is alive.
    /// </summary>
    /// <param name="key">The weak key.</param>
    /// <param name="val">The value.</param>
    /// <returns>True if the entry should be removed; otherwise, false.</returns>
    protected bool ShouldEntryBeRemoved(WeakKey<TKey> key, TValue val)
    {
      return !key.IsAlive;
    }

    /// <summary>
    /// Adds a key-value pair to the dictionary.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="val">The value.</param>
    public void Add(TKey key, TValue val)
    {
      int countBefore = _dict.Count;

      _dict.Add(new WeakKey<TKey>(key), val);

      if (countBefore == 0) // this is the first entry, so create a dictionary cleaner for it and all items added previously
        new WeakDictionaryCleaner(this); // create again a new instance of WeakDictionaryCleaner without a reference so it can be garbage collected
    }

    /// <summary>
    /// Tries to get the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <param name="val">When this method returns, contains the value associated with the key, if found; otherwise, the default value.</param>
    /// <returns>True if the key was found; otherwise, false.</returns>
    public virtual bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue val)
    {
      return _dict.TryGetValue(key, out val);
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The value associated with the key.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the key does not exist in the dictionary.</exception>
    public TValue this[TKey key]
    {
      get
      {
        if (TryGetValue(key, out var val))
          return val;
        else
          throw new ArgumentOutOfRangeException("Specified key has no entry in dictionary");
      }
    }

    /// <summary>
    /// Determines whether the dictionary contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>True if the dictionary contains the key; otherwise, false.</returns>
    public virtual bool Contains(TKey key)
    {
      return _dict.ContainsKey(key);
    }

    /// <summary>
    /// Removes all entries from the dictionary.
    /// </summary>
    public virtual void Clear()
    {
      _dict.Clear();
    }
  }
}
