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
using Altaxo.Serialization;


namespace Altaxo.Data
{
  /// <summary>
  /// Summary description for Altaxo.Data.DataTableCollection.
  /// </summary>
  [SerializationSurrogate(0,typeof(Altaxo.Data.DataTableCollection.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class DataTableCollection 
    :
    System.Runtime.Serialization.IDeserializationCallback, 
    System.Collections.ICollection, 
    Altaxo.Main.IDocumentNode,
    Main.INamedObjectCollection,
    Altaxo.Main.IChangedEventSource,
    Altaxo.Main.IChildChangedEventSink,
    Main.ISuspendable
  {
    // Data
    protected System.Collections.SortedList m_TablesByName = new System.Collections.SortedList();
    protected object m_Parent=null;

    // helper data
    /// <summary>
    /// Fired when table(s) are added, removed or renamed, and when the content of one table has changed.
    /// </summary>
    public event System.EventHandler Changed;

    /// <summary>
    /// Fired when one or more tables are added, deleted or renamed. Not fired when content in the table has changed.
    /// </summary>
    public event System.EventHandler CollectionChanged;

    [NonSerialized()]
    protected System.Collections.ArrayList m_SuspendedChildCollection = new System.Collections.ArrayList();
    [NonSerialized()]
    protected int  m_SuspendCount=0;
    [NonSerialized()]
    private   bool m_ResumeInProgress=false;
    [NonSerialized()]
    protected ChangedEventArgs m_ChangeData=null;
    [NonSerialized()]
    private bool m_DeserializationFinished=false;

    #region ChangedEventArgs

    /// <summary>
    /// Holds information about what has changed in the table.
    /// </summary>
    protected class ChangedEventArgs : System.EventArgs
    {
      /// <summary>
      /// If true, one or more tables where added.
      /// </summary>
      public bool TableAdded;
      /// <summary>
      /// If true, one or more table where removed.
      /// </summary>
      public bool TableRemoved;
      /// <summary>
      /// If true, one or more tables where renamed.
      /// </summary>
      public bool TableRenamed;

      /// <summary>
      /// Empty constructor.
      /// </summary>
      public ChangedEventArgs()
      {
      }

      /// <summary>
      /// Returns an empty instance.
      /// </summary>
      public static new ChangedEventArgs Empty
      {
        get { return new ChangedEventArgs(); }
      }

      /// <summary>
      /// Returns an instance with TableAdded set to true;.
      /// </summary>
      public static ChangedEventArgs IfTableAdded
      {
        get
        { 
          ChangedEventArgs e =  new ChangedEventArgs();
          e.TableAdded=true;
          return e;
        }
      }
      /// <summary>
      /// Returns an instance with TableRemoved set to true.
      /// </summary>
      public static ChangedEventArgs IfTableRemoved
      {
        get
        { 
          ChangedEventArgs e =  new ChangedEventArgs();
          e.TableRemoved=true;
          return e;
        }
      }
      /// <summary>
      /// Returns an  instance with TableRenamed set to true.
      /// </summary>
      public static ChangedEventArgs IfTableRenamed
      {
        get
        { 
          ChangedEventArgs e =  new ChangedEventArgs();
          e.TableRenamed=true;
          return e;
        }
      }

      
      /// <summary>
      /// Merges information from another instance in this ChangedEventArg.
      /// </summary>
      /// <param name="from"></param>
      public void Merge(ChangedEventArgs from)
      {
        this.TableAdded |= from.TableAdded;
        this.TableRemoved |= from.TableRemoved;
        this.TableRenamed |= from.TableRenamed;
      }

      /// <summary>
      /// Returns true when the collection has changed (addition, removal or renaming of tables).
      /// </summary>
      public bool CollectionChanged
      {
        get { return TableAdded | TableRemoved | TableRenamed; }
      }
    }

    #endregion

    public DataTableCollection(AltaxoDocument _parent)
    {
      this.m_Parent = _parent;
    }
  

    public object ParentObject
    {
      get { return this.m_Parent; }
      set { this.m_Parent = value; }
    }

    public string Name
    {
      get { return "Tables"; }
    }

    #region Serialization
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        Altaxo.Data.DataTableCollection s = (Altaxo.Data.DataTableCollection)obj;
        // info.AddValue("Parent",s._parent);
        info.AddValue("Tables",s.m_TablesByName);
      }
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        Altaxo.Data.DataTableCollection s = (Altaxo.Data.DataTableCollection)obj;
        // s._parent = (AltaxoDocument)(info.GetValue("Parent",typeof(AltaxoDocument)));
        s.m_TablesByName = (System.Collections.SortedList)(info.GetValue("Tables",typeof(System.Collections.SortedList)));

        // setup helper objects
        s.m_SuspendedChildCollection = new System.Collections.ArrayList();
        return s;
      }
    }

    public void OnDeserialization(object obj)
    {
      if(!m_DeserializationFinished && obj is DeserializationFinisher) // if deserialization has completely finished now
      {
        m_DeserializationFinished = true;
        DeserializationFinisher finisher = new DeserializationFinisher(this);
        // set the _parent object for the data tables
        foreach(DataTable dt in m_TablesByName.Values)
        {
          dt.ParentObject = this;
          dt.OnDeserialization(finisher);
        }
      }
    }

    #endregion

    #region "ICollection support"

    public void CopyTo(Array array, int index)
    {
      m_TablesByName.Values.CopyTo(array,index);
    }

    public int Count 
    {
      get { return m_TablesByName.Count; }
    }

    public bool IsSynchronized
    {
      get { return m_TablesByName.IsSynchronized; }
    }

    public object SyncRoot
    {
      get { return m_TablesByName.SyncRoot; }
    }

    public System.Collections.IEnumerator GetEnumerator()
    {
      return m_TablesByName.Values.GetEnumerator();
    }



    #endregion

    #region Suspend and resume

    public bool IsSuspended
    {
      get { return m_SuspendCount>0; }
    }

    public void Suspend()
    {
      System.Diagnostics.Debug.Assert(m_SuspendCount>=0,"SuspendCount must always be greater or equal to zero");    

      ++m_SuspendCount; // suspend one step higher
    }

    public void Resume()
    {
      System.Diagnostics.Debug.Assert(m_SuspendCount>=0,"SuspendCount must always be greater or equal to zero");    
      if(m_SuspendCount>0 && (--m_SuspendCount)==0)
      {
        this.m_ResumeInProgress = true;
        foreach(Main.ISuspendable obj in m_SuspendedChildCollection)
          obj.Resume();
        m_SuspendedChildCollection.Clear();
        this.m_ResumeInProgress = false;

        // send accumulated data if available and release it thereafter
        if(null!=m_ChangeData)
        {
          if(m_Parent is Main.IChildChangedEventSink)
          {
            ((Main.IChildChangedEventSink)m_Parent).EhChildChanged(this, m_ChangeData);
          }
          if(!IsSuspended)
          {
            if(m_ChangeData.CollectionChanged)
              OnCollectionChanged();
            
            OnDataChanged(); // Fire the changed event
          }   
        }
      }
    }


    void AccumulateChildChangeData(object sender, System.EventArgs e)
    {

      if(m_ChangeData==null)
        m_ChangeData = ChangedEventArgs.Empty;

      if(e is ChangedEventArgs)
        m_ChangeData.Merge((ChangedEventArgs)e);
    }
  
    public void EhChildChanged(object sender, System.EventArgs e)
    {
      if(this.IsSuspended &&  sender is Main.ISuspendable)
      {
        m_SuspendedChildCollection.Add(sender); // add sender to suspended child
        ((Main.ISuspendable)sender).Suspend();
        return;
      }

      AccumulateChildChangeData(sender,e);  // AccumulateNotificationData
      
      if(m_ResumeInProgress || IsSuspended)
        return;

      if(m_Parent is Main.IChildChangedEventSink )
      {
        ((Main.IChildChangedEventSink)m_Parent).EhChildChanged(this, m_ChangeData);
        if(IsSuspended) // maybe parent has suspended us now
        {
          this.EhChildChanged(sender, e); // we call the function recursively, but now we are suspended
          return;
        }
      }
      
      if(m_ChangeData.CollectionChanged)
        OnCollectionChanged();

      OnDataChanged(); // Fire the changed event
    }

    private void SelfChanged(ChangedEventArgs e)
    {
      EhChildChanged(null,e);
    }

    protected virtual void OnCollectionChanged()
    {
      if(this.CollectionChanged!=null)
        CollectionChanged(this,m_ChangeData);
    }


    protected virtual void OnChanged(EventArgs e)
    {
      if(null!=Changed)
        Changed(this,e);
    }

    protected virtual void OnDataChanged()
    {
      if(null!=Changed)
        Changed(this,m_ChangeData);
    
      m_ChangeData=null;
    }


    #endregion


    public bool IsDirty
    {
      get
      {
        return m_ChangeData!=null;
      }
    }

    public string[] GetSortedTableNames()
    {
      string[] arr = new string[m_TablesByName.Count];
      this.m_TablesByName.Keys.CopyTo(arr,0);
      // System.Array.Sort(arr);
      return arr;
    }

    public Altaxo.Data.DataTable this[string name]
    {
      get
      {
        return (Altaxo.Data.DataTable)m_TablesByName[name];
      }
    }

    public bool ContainsTable(string tablename)
    {
      return m_TablesByName.ContainsKey(tablename);
    }

    public bool ContainsTable(DataTable table)
    {
      DataTable found = this[table.Name];
      return found != null && object.ReferenceEquals(found,table);
    }

    public void Add(Altaxo.Data.DataTable theTable)
    {
      if(null==theTable.Name || 0==theTable.Name.Length) // if no table name provided
        theTable.Name = FindNewTableName();                 // find a new one
      else if(m_TablesByName.ContainsKey(theTable.Name)) // else if this table name is already in use
        theTable.Name = FindNewTableName(theTable.Name); // find a new table name based on the original name

      // now the table has a unique name in any case
      m_TablesByName.Add(theTable.Name,theTable);
      theTable.ParentObject = this; 
      theTable.NameChanged += new Main.NameChangedEventHandler(this.EhTableNameChanged);
      theTable.ParentChanged += new Main.ParentChangedEventHandler(this.EhTableParentChanged);

      // raise data event to all listeners
      this.SelfChanged(ChangedEventArgs.IfTableAdded);

    }

    public void Remove(Altaxo.Data.DataTable theTable)
    {
      if(m_TablesByName.ContainsValue(theTable))
      {
        m_TablesByName.Remove(theTable.Name);
        theTable.ParentChanged -= new Main.ParentChangedEventHandler(this.EhTableParentChanged);
        theTable.NameChanged -= new Main.NameChangedEventHandler(this.EhTableNameChanged);
        theTable.ParentObject=null;
      }

      this.SelfChanged(ChangedEventArgs.IfTableRemoved);
    }

    protected void EhTableParentChanged(object sender, Main.ParentChangedEventArgs pce)
    {
      if(object.ReferenceEquals(this,pce.OldParent) && this.ContainsTable((DataTable)sender))
        this.Remove((DataTable)sender);
      else
        if(!this.ContainsTable((DataTable)sender))
        throw new ApplicationException("Not allowed to set child's parent to this collection before adding it to the collection");
    }

    protected void EhTableNameChanged(object sender, Main.NameChangedEventArgs nce)
    {
      if(object.ReferenceEquals(this[nce.NewName],sender))
        return; // Table alredy renamed

      if(this.ContainsTable(nce.NewName))
        throw new ApplicationException("Table with name " + nce.NewName + " already exists!");

      if(!this.ContainsTable(nce.OldName))
        throw new ApplicationException("Error renaming table " + nce.OldName + " : this table name was not found in the collection!" );
        
      if(!object.ReferenceEquals(this[nce.OldName],sender))
        throw new ApplicationException("Names between DataTableCollection and Tables not in sync");

      m_TablesByName.Remove(nce.OldName);
      m_TablesByName.Add(nce.NewName,(DataTable)sender);

      SelfChanged(ChangedEventArgs.IfTableRenamed);
    }

    /// <summary>
    /// Looks for the next free standard table name.
    /// </summary>
    /// <returns>A new table name unique for this data set.</returns>
    public string FindNewTableName()
    {
      return FindNewTableName("WKS");
    } 

    /// <summary>
    /// Looks for the next unique table name base on a basic name.
    /// </summary>
    /// <returns>A new table name unique for this data set.</returns>
    public string FindNewTableName(string basicname)
    {
      for(int i=0;;i++)
      {
        if(null==m_TablesByName[basicname+i.ToString()])
          return basicname+i; 
      }
    } 

    public object GetChildObjectNamed(string name)
    {
      return m_TablesByName[name];
    }

    public string GetNameOfChildObject(object o)
    {
      if(o is DataTable)
      {
        DataTable gr = (DataTable)o;
        if(m_TablesByName.ContainsKey(gr.Name))
          return gr.Name;
      }
      return null;
    }


    /// <summary>
    /// Gets the parent DataTableCollection of a child table, a child ColumnCollection, or a child column.
    /// </summary>
    /// <param name="child">Can be a DataTable, a DataColumnCollection, or a DataColumn for which the parent table collection is searched.</param>
    /// <returns>The parent DataTableCollection, if it exists, or null otherwise.</returns>
    public static Altaxo.Data.DataTableCollection GetParentDataTableCollectionOf(Main.IDocumentNode child)
    {
      return (DataTableCollection)Main.DocumentPath.GetRootNodeImplementing(child,typeof(DataTableCollection));
    }
  }
}
