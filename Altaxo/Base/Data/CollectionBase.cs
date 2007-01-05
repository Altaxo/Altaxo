#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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


// Note by Dirk Lellinger: the code below is originated by the mono project (http://www.go-mono.org)
// and was written by the author below.
// Modified by Dirk Lellinger to support Serialization Surrogates
// Only change so far: change "private ArrayList myList" to "protected ArrayList myList" so the list can be restored in derived classes Serialization Surrogates



// System.Collections.CollectionBase.cs
//
// Author:
//   Nick Drochak II (ndrochak@gol.com)
//
// (C) 2001 Nick Drochak II
//

using System;
using System.Collections;


namespace Altaxo.Data 
{

  [Serializable]
  public abstract class CollectionBase : IList, ICollection, IEnumerable 
  {

    // private instance properties
    protected ArrayList myList;
    
    // public instance properties
    public int Count { get { return InnerList.Count; } }
    
    // Public Instance Methods
    public IEnumerator GetEnumerator() { return InnerList.GetEnumerator(); }
    public void Clear() 
    { 
      OnClear();
      InnerList.Clear(); 
      OnClearComplete();
    }
    public void RemoveAt (int index) 
    {
      object objectToRemove;
      objectToRemove = InnerList[index];
      OnValidate(objectToRemove);
      OnRemove(index, objectToRemove);
      InnerList.RemoveAt(index);
      OnRemoveComplete(index, objectToRemove);
    }
    
    // Protected Instance Constructors
    protected CollectionBase() 
    { 
      this.myList = new ArrayList();
    }
    
    // Protected Instance Properties
    protected ArrayList InnerList {get { return this.myList; } }
    protected IList List {get { return this; } }
    
    // Protected Instance Methods
    protected virtual void OnClear() { }
    protected virtual void OnClearComplete() { }
    
    protected virtual void OnInsert(int index, object value) { }
    protected virtual void OnInsertComplete(int index, object value) { }

    protected virtual void OnRemove(int index, object value) { }
    protected virtual void OnRemoveComplete(int index, object value) { }

    protected virtual void OnSet(int index, object oldValue, object newValue) { }
    protected virtual void OnSetComplete(int index, object oldValue, object newValue) { }

    protected virtual void OnValidate(object value) 
    {
      if (null == value) 
      {
        throw new System.ArgumentNullException("CollectionBase.OnValidate: Invalid parameter value passed to method: null");
      }
    }
    
    // ICollection methods
    void ICollection.CopyTo(Array array, int index) 
    {
      InnerList.CopyTo(array, index);
    }
    object ICollection.SyncRoot 
    {
      get { return InnerList.SyncRoot; }
    }
    bool ICollection.IsSynchronized 
    {
      get { return InnerList.IsSynchronized; }
    }

    // IList methods
    int IList.Add (object value) 
    {
      int newPosition;
      OnValidate(value);
      newPosition = InnerList.Count;
      OnInsert(newPosition, value);
      InnerList.Add(value);
      OnInsertComplete(newPosition, value);
      return newPosition;
    }
    
    bool IList.Contains (object value) 
    {
      return InnerList.Contains(value);
    }

    int IList.IndexOf (object value) 
    {
      return InnerList.IndexOf(value);
    }

    void IList.Insert (int index, object value) 
    {
      OnValidate(value);
      OnInsert(index, value);
      InnerList.Insert(index, value);
      OnInsertComplete(index, value);
    }

    void IList.Remove (object value) 
    {
      int removeIndex;
      OnValidate(value);
      removeIndex = InnerList.IndexOf(value);
      OnRemove(removeIndex, value);
      InnerList.Remove(value);
      OnRemoveComplete(removeIndex, value);
    }

    // IList properties
    bool IList.IsFixedSize 
    { 
      get { return InnerList.IsFixedSize; }
    }

    bool IList.IsReadOnly 
    { 
      get { return InnerList.IsReadOnly; }
    }

    object IList.this[int index] 
    { 
      get { return InnerList[index]; }
      set 
      { 
        object oldValue;
        // make sure we have been given a valid value
        OnValidate(value);
        // save a reference to the object that is in the list now
        oldValue = InnerList[index];
        
        OnSet(index, oldValue, value);
        InnerList[index] = value;
        OnSetComplete(index, oldValue, value);
      }
    }
  }
}
