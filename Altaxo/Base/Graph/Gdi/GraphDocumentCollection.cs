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
using System.Collections.Generic;
using Altaxo;
using Altaxo.Main;

namespace Altaxo.Graph.Gdi
{
  public class GraphDocumentCollection : 
    System.Runtime.Serialization.IDeserializationCallback,
    IEnumerable<GraphDocument>,
    Altaxo.Main.IDocumentNode,
    Altaxo.Main.IChangedEventSource,
    Altaxo.Main.IChildChangedEventSink,
    Altaxo.Main.INamedObjectCollection
  {
    #region ChangedEventArgs
    /// <summary>
    /// Holds information about what has changed in the table.
    /// </summary>
    protected class ChangedEventArgs : System.EventArgs
    {
      /// <summary>
      /// If true, one or more tables where added.
      /// </summary>
      public bool ItemAdded;
      /// <summary>
      /// If true, one or more table where removed.
      /// </summary>
      public bool ItemRemoved;
      /// <summary>
      /// If true, one or more tables where renamed.
      /// </summary>
      public bool ItemRenamed;

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
      public static ChangedEventArgs IfItemAdded
      {
        get
        { 
          ChangedEventArgs e =  new ChangedEventArgs();
          e.ItemAdded=true;
          return e;
        }
      }
      /// <summary>
      /// Returns an instance with TableRemoved set to true.
      /// </summary>
      public static ChangedEventArgs IfItemRemoved
      {
        get
        { 
          ChangedEventArgs e =  new ChangedEventArgs();
          e.ItemRemoved=true;
          return e;
        }
      }
      /// <summary>
      /// Returns an  instance with TableRenamed set to true.
      /// </summary>
      public static ChangedEventArgs IfItemRenamed
      {
        get
        { 
          ChangedEventArgs e =  new ChangedEventArgs();
          e.ItemRenamed=true;
          return e;
        }
      }

      
      /// <summary>
      /// Merges information from another instance in this ChangedEventArg.
      /// </summary>
      /// <param name="from"></param>
      public void Merge(ChangedEventArgs from)
      {
        this.ItemAdded |= from.ItemAdded;
        this.ItemRemoved |= from.ItemRemoved;
        this.ItemRenamed |= from.ItemRenamed;
      }

      /// <summary>
      /// Returns true when the collection has changed (addition, removal or renaming of tables).
      /// </summary>
      public bool CollectionChanged
      {
        get { return ItemAdded | ItemRemoved | ItemRenamed; }
      }
    }

    #endregion

    // Data
    protected SortedDictionary<string,GraphDocument> m_GraphsByName = new SortedDictionary<string,GraphDocument>();
    protected bool bIsDirty=false;

    [NonSerialized]
    protected object m_Parent = null;

    [NonSerialized]
    protected ChangedEventArgs m_ChangeData=null;

    // Events

    /// <summary>
    /// Fired when one or more graphs are added, deleted or renamed. Not fired when content in the graph has changed.
    /// </summary>
    public event System.EventHandler CollectionChanged;

    #region IChangedEventSource Members

    public event System.EventHandler Changed;

    #endregion


    public GraphDocumentCollection(AltaxoDocument parent)
    {
      this.m_Parent = parent;
    }

    public object Parent
    {
      get { return this.m_Parent; }
      set { this.m_Parent=value; }
    }

    public object ParentObject
    {
      get { return this.m_Parent; }
    }

    public string Name
    {
      get { return "Graphs"; }
    }

    #region Serialization
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        GraphDocumentCollection s = (GraphDocumentCollection)obj;
        // info.AddValue("Parent",s.parent);
        info.AddValue("Graphs",s.m_GraphsByName);
      }
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        GraphDocumentCollection s = (GraphDocumentCollection)obj;
        // s.parent = (AltaxoDocument)(info.GetValue("Parent",typeof(AltaxoDocument)));
        s.m_GraphsByName = (SortedDictionary<string,GraphDocument>)(info.GetValue("Graphs",typeof(SortedDictionary<string,GraphDocument>)));
      
        return s;
      }
    }

    public void OnDeserialization(object obj)
    {
    }

    #endregion

    


    public bool IsDirty
    {
      get
      {
        return bIsDirty;
      }
    }

    public string[] GetSortedGraphNames()
    {
      string[] arr = new string[m_GraphsByName.Count];
      this.m_GraphsByName.Keys.CopyTo(arr,0);
      System.Array.Sort(arr);
      return arr;
    }


    public GraphDocument this[string name]
    {
      get
      {
        return (GraphDocument)m_GraphsByName[name];
      }
    }

    public bool Contains(string graphname)
    {
      return m_GraphsByName.ContainsKey(graphname);
    }

    public void Add(GraphDocument theGraph)
    {
      if(!string.IsNullOrEmpty(theGraph.Name) && m_GraphsByName.ContainsKey(theGraph.Name) && theGraph.Equals(m_GraphsByName[theGraph.Name]))
        return; // do silently nothing if the graph (the same!) is already registered
      if(string.IsNullOrEmpty(theGraph.Name)) // if no table name provided
        theGraph.Name = FindNewName();                  // find a new one
      else if(m_GraphsByName.ContainsKey(theGraph.Name)) // else if this table name is already in use
        theGraph.Name = FindNewName(theGraph.Name); // find a new table name based on the original name

      // now the table has a unique name in any case
      m_GraphsByName.Add(theGraph.Name,theGraph);
      theGraph.ParentObject = this; 
      theGraph.NameChanged += new NameChangedEventHandler(this.EhChild_NameChanged);
      this.OnSelfChanged(ChangedEventArgs.IfItemAdded);
    }

    public void Remove(GraphDocument theGraph)
    {
      if(theGraph!=null && theGraph.Name!=null)
      {
        GraphDocument gr = (GraphDocument)m_GraphsByName[theGraph.Name];
        if(gr.Equals(theGraph))
        {
          m_GraphsByName.Remove(theGraph.Name);
          theGraph.ParentObject = null;
          theGraph.NameChanged -= new NameChangedEventHandler(this.EhChild_NameChanged);
          this.OnSelfChanged(ChangedEventArgs.IfItemRemoved);
        }
      }
    }

    protected void EhChild_NameChanged(object sender, NameChangedEventArgs e)
    {
      // we remove the old value from the hash and store it under the new value
      GraphDocument graph = m_GraphsByName[e.OldName];
      if(graph!=null)
      {
        if(m_GraphsByName.ContainsKey(e.NewName))
          throw new ApplicationException(string.Format("The GraphDocumentCollection contains already a Graph named {0}, renaming the old graph {1} fails.",e.NewName,e.OldName));
        m_GraphsByName.Remove(e.OldName);
        m_GraphsByName[e.NewName] = graph;
        this.OnSelfChanged(ChangedEventArgs.IfItemRenamed);
      }
    }
    /// <summary>
    /// Looks for the next free standard  name.
    /// </summary>
    /// <returns>A new table name unique for this set.</returns>
    public string FindNewName()
    {
      return FindNewName("GRAPH");
    } 

    /// <summary>
    /// Looks for the next unique name base on a basic name.
    /// </summary>
    /// <returns>A new  name unique for this  set.</returns>
    public string FindNewName(string basicname)
    {
      for(int i=0;;i++)
      {
        if(!m_GraphsByName.ContainsKey(basicname+i.ToString()))
          return basicname+i; 
      }
    } 
  
    public object GetChildObjectNamed(string name)
    {
      GraphDocument result=null;
      if (m_GraphsByName.TryGetValue(name, out result))
        return result;
      else return null;
    }

    public string GetNameOfChildObject(object o)
    {
      if(o is GraphDocument)
      {
        GraphDocument gr = (GraphDocument)o;
        if(m_GraphsByName.ContainsKey(gr.Name))
          return gr.Name;
      }
      return null;
    }

    #region Change event handling

    [NonSerialized()]
    protected bool             m_ResumeInProgress=false;
    [NonSerialized()]
    protected System.Collections.ArrayList m_SuspendedChildCollection=new System.Collections.ArrayList();

    
    public bool IsSuspended
    {
      get 
      {
        return false; // m_SuspendCount>0;
      }
    }

#if false
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
            ((Main.IChildChangedEventSink)m_Parent).OnChildChanged(this, m_ChangeData);
          }
          if(!IsSuspended)
          {
            OnChanged(); // Fire the changed event
          }   
        }
      }
    }

#endif


    /// <summary>
    /// Fires the Invalidate event.
    /// </summary>
    /// <param name="sender">The layer which needs to be repainted.</param>
    protected internal virtual void OnInvalidate(XYPlotLayer sender)
    {
      OnChanged();
    }

    void AccumulateChildChangeData(object sender, EventArgs e)
    {
      if(m_ChangeData==null)
        this.m_ChangeData=ChangedEventArgs.Empty;

      if(e is ChangedEventArgs)
        m_ChangeData.Merge((ChangedEventArgs)e);
    }   

    protected bool HandleImmediateChildChangeCases(object sender, EventArgs e)
    {
      return false; // not handled
    }


    protected virtual void OnSelfChanged(EventArgs e)
    {
      EhChildChanged(null,e);
    }


    /// <summary>
    /// Handle the change notification from the child layers.
    /// </summary>
    /// <param name="sender">The sender of the change notification.</param>
    /// <param name="e">The change details.</param>
    public void EhChildChanged(object sender, System.EventArgs e)
    {
      if(HandleImmediateChildChangeCases(sender, e))
        return;

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

      OnChanged(); // Fire the changed event
    }

    protected virtual void OnChanged()
    {
      if(null!=Changed)
        Changed(this, m_ChangeData);

      m_ChangeData=null;
    }

    protected virtual void OnCollectionChanged()
    {
      if(this.CollectionChanged!=null)
        CollectionChanged(this,m_ChangeData);
    }

    #endregion


    /// <summary>
    /// Gets the parent GraphDocumentCollection of a child graph.
    /// </summary>
    /// <param name="child">A graph for which the parent collection is searched.</param>
    /// <returns>The parent GraphDocumentCollection, if it exists, or null otherwise.</returns>
    public static GraphDocumentCollection GetParentGraphDocumentCollectionOf(Main.IDocumentNode child)
    {
      return (GraphDocumentCollection)Main.DocumentPath.GetRootNodeImplementing(child, typeof(GraphDocumentCollection));
    }


  
    #region IEnumerable<GraphDocument> Members

    IEnumerator<GraphDocument> IEnumerable<GraphDocument>.GetEnumerator()
    {
      return m_GraphsByName.Values.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    public System.Collections.IEnumerator GetEnumerator()
    {
      return m_GraphsByName.Values.GetEnumerator();
    }

    #endregion
  }
}
