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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.Collections
{
  public class WeakDictionary<Key, Value>
  {
    private Dictionary<object, Value> _dict = new Dictionary<object, Value>();

    protected class WeakKey<T> : WeakReference
    {
      private int _hash;

      public WeakKey(T obj)
        : base(obj)
      {
        _hash = obj.GetHashCode();
      }

      public override bool Equals(object obj)
      {
        if (obj is WeakReference)
          return object.ReferenceEquals(obj, this);
        else if (obj != null)
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

      public CleanerRef(WeakDictionaryCleaner cleaner, WeakDictionary<Key, Value> dictionary)
      {
        handle = GCHandle.Alloc(cleaner, GCHandleType.WeakTrackResurrection);
        Dictionary = dictionary;
      }

      public bool IsAlive
      {
        get { return handle.IsAllocated && handle.Target != null; }
      }

      public object Target
      {
        get { return IsAlive ? handle.Target : null; }
      }

      private GCHandle handle;
      public WeakDictionary<Key, Value> Dictionary;
    }

    internal class WeakDictionaryCleaner
    {
      public WeakDictionaryCleaner(WeakDictionary<Key, Value> dict)
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
      var toRemove = new List<KeyValuePair<object, Value>>();
      foreach (var entry in _dict)
      {
        if (ShouldEntryBeRemoved((WeakKey<Key>)entry.Key, entry.Value))
          toRemove.Add(entry);
      }
      foreach (var entry in toRemove)
        _dict.Remove(entry.Key);

      if (_dict.Count != 0)
        new WeakDictionaryCleaner(this); // create again a new instance of WeakDictionaryCleaner without a reference so it can be garbage collected
    }

    protected bool ShouldEntryBeRemoved(WeakKey<Key> key, Value val)
    {
      return !key.IsAlive;
    }

    public void Add(Key key, Value val)
    {
      int countBefore = _dict.Count;

      _dict.Add(new WeakKey<Key>(key), val);

      if (countBefore == 0) // this is the first entry, so create a dictionary cleaner for it and all items added previously
        new WeakDictionaryCleaner(this); // create again a new instance of WeakDictionaryCleaner without a reference so it can be garbage collected
    }

    public virtual bool TryGetValue(Key key, out Value val)
    {
      return _dict.TryGetValue(key, out val);
    }

    public Value this[Key key]
    {
      get
      {
        var isH = TryGetValue(key, out var val);
        if (isH)
          return val;
        else
          throw new ArgumentOutOfRangeException("Specified key has no entry in dictionary");
      }
    }

    public virtual bool Contains(Key key)
    {
      return _dict.ContainsKey(key);
    }

    public virtual void Clear()
    {
      _dict.Clear();
    }
  }
}
