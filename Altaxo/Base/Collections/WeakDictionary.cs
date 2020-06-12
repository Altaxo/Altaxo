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
  public class WeakDictionary<TKey, TValue> where TKey : notnull
  {
    private Dictionary<object, TValue> _dict = new Dictionary<object, TValue>();

    protected class WeakKey<T> : WeakReference
    {
      private int _hash;

      public WeakKey(T obj)
        : base(obj)
      {
        _hash = obj?.GetHashCode() ?? 0;
      }

      public override bool Equals(object? obj)
      {
        if (obj is WeakReference)
          return object.ReferenceEquals(obj, this);
        else if (obj is { } _)
          return obj.Equals(Target);
        else
          return false;
      }

      public override int GetHashCode()
      {
        return _hash;
      }
    }

    internal class CleanerRef
    {
      ~CleanerRef()
      {
        if (handle.IsAllocated)
          handle.Free();
      }

      public CleanerRef(WeakDictionaryCleaner cleaner, WeakDictionary<TKey, TValue> dictionary)
      {
        handle = GCHandle.Alloc(cleaner, GCHandleType.WeakTrackResurrection);
        Dictionary = dictionary;
      }

      public bool IsAlive
      {
        get { return handle.IsAllocated && handle.Target != null; }
      }

      public object? Target
      {
        get { return IsAlive ? handle.Target : null; }
      }

      private GCHandle handle;
      public WeakDictionary<TKey, TValue> Dictionary;
    }

    internal class WeakDictionaryCleaner
    {
      public WeakDictionaryCleaner(WeakDictionary<TKey, TValue> dict)
      {
        refs.Add(new CleanerRef(this, dict));
      }

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

    public WeakDictionary()
    {
    }

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

    protected bool ShouldEntryBeRemoved(WeakKey<TKey> key, TValue val)
    {
      return !key.IsAlive;
    }

    public void Add(TKey key, TValue val)
    {
      int countBefore = _dict.Count;

      _dict.Add(new WeakKey<TKey>(key), val);

      if (countBefore == 0) // this is the first entry, so create a dictionary cleaner for it and all items added previously
        new WeakDictionaryCleaner(this); // create again a new instance of WeakDictionaryCleaner without a reference so it can be garbage collected
    }

    public virtual bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue val)
    {
      return _dict.TryGetValue(key, out val);
    }

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

    public virtual bool Contains(TKey key)
    {
      return _dict.ContainsKey(key);
    }

    public virtual void Clear()
    {
      _dict.Clear();
    }
  }
}
