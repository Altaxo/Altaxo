#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;

namespace Altaxo.Worksheet
{
  /// <summary>
  /// Summary description for WorksheetLayoutCollection.
  /// </summary>
  public class WorksheetLayoutCollection 
		:
		Main.IDocumentNode, 
		Main.INamedObjectCollection,
		ICollection<WorksheetLayout>
  {
    protected object _documentParent;
    protected Dictionary<string, WorksheetLayout> _items;



    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorksheetLayoutCollection),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        WorksheetLayoutCollection s = (WorksheetLayoutCollection)obj;

        info.CreateArray("TableLayoutArray",s._items.Count);
        foreach(object style in s._items.Values)
          info.AddValue("WorksheetLayout",style);
        info.CommitArray();
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        WorksheetLayoutCollection s = null!=o ? (WorksheetLayoutCollection)o : new WorksheetLayoutCollection();

        int count;
        count = info.OpenArray(); // TableLayouts
        
        for(int i=0;i<count;i++)
        {
          WorksheetLayout style = (WorksheetLayout)info.GetValue("WorksheetLayout",s);
          s._items.Add(style.Guid.ToString(),style);
        }
        info.CloseArray(count);

        return s;
      }
    }
 
    
    #endregion
    
    
    public WorksheetLayoutCollection()
    {
			_items = new Dictionary<string, WorksheetLayout>();
    }

    public WorksheetLayoutCollection(object documentParent)
      : this()
    {
      _documentParent = documentParent;
    }

    public WorksheetLayout this[System.Guid guid]
    {
      get { return _items[guid.ToString()]; }    
    }
    public WorksheetLayout this[string guidAsString]
    {
      get { return _items[guidAsString]; }   
    }

    public void Add(WorksheetLayout layout)
    {
      layout.ParentObject = this;

      // Test if this Guid is already present
			WorksheetLayout o = null;
			_items.TryGetValue(layout.Guid.ToString(), out o);
      if(o!=null && !object.ReferenceEquals(o,layout))
        layout.NewGuid();

      _items[layout.Guid.ToString()] = layout;
    }

	
    #region IDocumentNode Members

    public object ParentObject
    {
      get
      {
        return _documentParent;
      }
      set
      {
        _documentParent = value;
      }
    }

    public string Name
    {
      get
      {
        return "TableLayouts";
      }
    }

    #endregion

    #region INamedObjectCollection Members

    public object GetChildObjectNamed(string name)
    {
      return this[name];
    }

    public string GetNameOfChildObject(object o)
    {
      WorksheetLayout layout = o as WorksheetLayout;
      if(layout==null)
        return null;
      if(null==this[layout.Guid])
        return null; // is not contained in this collection
      return layout.Guid.ToString();
    }

    #endregion

		#region ICollection<WorksheetLayout> Members


		public void Clear()
		{
			_items.Clear();
		}

		public bool Contains(WorksheetLayout item)
		{
			return _items.ContainsKey(item.Guid.ToString());
		}

		public void CopyTo(WorksheetLayout[] array, int arrayIndex)
		{
			_items.Values.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _items.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(WorksheetLayout item)
		{
			return _items.Remove(item.Guid.ToString());
		}

		#endregion

		#region IEnumerable<WorksheetLayout> Members

		public IEnumerator<WorksheetLayout> GetEnumerator()
		{
			return _items.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _items.Values.GetEnumerator();
		}

		#endregion
	}
}
