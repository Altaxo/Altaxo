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

using System;

namespace Altaxo.Worksheet
{
  /// <summary>
  /// Summary description for WorksheetLayoutCollection.
  /// </summary>
  public class WorksheetLayoutCollection : Main.IDocumentNode, Main.INamedObjectCollection
  {
    protected object m_DocumentParent;
    protected System.Collections.Hashtable m_TableLayouts;



    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorksheetLayoutCollection),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        WorksheetLayoutCollection s = (WorksheetLayoutCollection)obj;

        info.CreateArray("TableLayoutArray",s.m_TableLayouts.Count);
        foreach(object style in s.m_TableLayouts.Values)
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
          s.m_TableLayouts.Add(style.Guid,style);
        }
        info.CloseArray(count);

        return s;
      }
    }
 
    
    #endregion
    
    
    public WorksheetLayoutCollection()
    {
      m_TableLayouts = new System.Collections.Hashtable();
    }

    public WorksheetLayoutCollection(object documentParent)
      : this()
    {
      m_DocumentParent = documentParent;
    }

    public WorksheetLayout this[System.Guid guid]
    {
      get { return (WorksheetLayout)m_TableLayouts[guid.ToString()]; }    
    }
    public WorksheetLayout this[string guidAsString]
    {
      get { return (WorksheetLayout)m_TableLayouts[guidAsString]; }   
    }

    public void Add(WorksheetLayout layout)
    {
      layout.ParentObject = this;

      // Test if this Guid is already present
      object o = this[layout.Guid];
      if(o!=null && !object.ReferenceEquals(o,layout))
        layout.NewGuid();

      m_TableLayouts[layout.Guid.ToString()] = layout;
    }

    #region "ICollection support"

    public void CopyTo(Array array, int index)
    {
      this.m_TableLayouts.Values.CopyTo(array,index);
    }

    public int Count 
    {
      get { return this.m_TableLayouts.Count; }
    }

    public bool IsSynchronized
    {
      get { return this.m_TableLayouts.IsSynchronized; }
    }

    public object SyncRoot
    {
      get { return this.m_TableLayouts.SyncRoot; }
    }

    public System.Collections.IEnumerator GetEnumerator()
    {
      return this.m_TableLayouts.Values.GetEnumerator();
    }

    #endregion


    #region IDocumentNode Members

    public object ParentObject
    {
      get
      {
        return m_DocumentParent;
      }
      set
      {
        m_DocumentParent = value;
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
  }
}
