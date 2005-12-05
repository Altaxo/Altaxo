#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Axes.Boundaries;

namespace Altaxo.Graph
{
  [SerializationSurrogate(0,typeof(PlotItemCollection.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class PlotItemCollection 
    :
    Altaxo.Data.CollectionBase,
    System.Runtime.Serialization.IDeserializationCallback,
    Main.IChangedEventSource,
    Main.IChildChangedEventSink,
    System.ICloneable,
    Main.IDocumentNode,
    Main.INamedObjectCollection
  {
    /// <summary>The parent layer of this list.</summary>
    private XYPlotLayer m_Owner; 

    private PlotGroup.Collection m_PlotGroups;

    #region Serialization
    /// <summary>Used to serialize the PlotItemCollection Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      public object[] m_PlotItems = null; 

      /// <summary>
      /// Serializes PlotItemCollection Version 0.
      /// </summary>
      /// <param name="obj">The PlotItemCollection to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        PlotItemCollection s = (PlotItemCollection)obj;
        info.AddValue("Data",s.myList);
        info.AddValue("PlotGroups",s.m_PlotGroups);

      }

      /// <summary>
      /// Deserializes the PlotItemCollection Version 0.
      /// </summary>
      /// <param name="obj">The empty PlotItemCollection object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized PlotItemCollection.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        PlotItemCollection s = (PlotItemCollection)obj;

        s.myList = (System.Collections.ArrayList)info.GetValue("Data",typeof(System.Collections.ArrayList));
        s.m_PlotGroups = (Graph.PlotGroup.Collection)info.GetValue("PlotGroups",typeof(Graph.PlotGroup.Collection));
        
        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotItemCollection),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PlotItemCollection s = (PlotItemCollection)obj;
        
        info.CreateArray("PlotItems",s.Count);
        for(int i=0;i<s.Count;i++)
          info.AddValue("PlotItem",s[i]);
        info.CommitArray();

        // now serialize the PlotGroups
        info.CreateArray("PlotGroups",s.m_PlotGroups.Count);
        for(int i=0;i<s.m_PlotGroups.Count;i++)
        {
          PlotGroup pg = (PlotGroup)s.m_PlotGroups[i];
          info.AddValue("PlotGroup",new PlotGroup.Memento(pg,s));
        }
        info.CommitArray(); // PlotGroups
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        PlotItemCollection s = null!=o ? (PlotItemCollection)o : new PlotItemCollection();

        int count = info.OpenArray();
        for(int i=0;i<count;i++)
        {
          PlotItem plotitem = (PlotItem)info.GetValue("PlotItem",s);
          s.Add(plotitem);
        }
        info.CloseArray(count);
        

        count = info.OpenArray(); // PlotGroups
        for(int i=0;i<count;i++)
        {
          PlotGroup.Memento pgm = (PlotGroup.Memento)info.GetValue(s);
          s.m_PlotGroups.Add(pgm.GetPlotGroup(s));
        }
        info.CloseArray(count);
        
        return s;
      }
    }

    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      // restore the event chain
      for(int i=0;i<Count;i++)
        WireItem(this[i]);

      if(null!=m_PlotGroups)
        m_PlotGroups.Changed += new EventHandler(this.EhPlotGroups_Changed);

    }
        
    #endregion



    public PlotItemCollection(XYPlotLayer owner)
    {
      m_Owner = owner;
      m_PlotGroups = new PlotGroup.Collection();
    }

    /// <summary>
    /// Empty constructor for deserialization.
    /// </summary>
    protected PlotItemCollection()
    {
      m_PlotGroups = new PlotGroup.Collection();
    }

    /// <summary>
    /// Copy constructor. Clones (!) all items. The parent owner is set to null and has to be set afterwards.
    /// </summary>
    /// <param name="from">The PlotItemCollection to clone this list from.</param>
    public PlotItemCollection(PlotItemCollection from)
      :
      this(null,from)
    {
    }

    /// <summary>
    /// Copy constructor. Clones (!) all the items in the list.
    /// </summary>
    /// <param name="owner">The new owner of the cloned list.</param>
    /// <param name="from">The list to clone all items from.</param>
    public PlotItemCollection(XYPlotLayer owner, PlotItemCollection from)
    {
      m_Owner = owner;
      m_PlotGroups = new PlotGroup.Collection();

      // Clone all the items in the list.
      for(int i=0;i<from.Count;i++)
        Add((PlotItem)from[i].Clone()); // clone the items

      // special way neccessary to handle plot groups
      this.m_PlotGroups = null==from.m_PlotGroups ? null : from.m_PlotGroups.Clone(this,from);
    }

    public object Clone()
    {
      return new PlotItemCollection(this);
    }

    public XYPlotLayer ParentLayer
    {
      get { return m_Owner; }
      set
      {
        SetParentLayer(value,false);
      }
    }
    
    public object ParentObject
    {
      get { return m_Owner; }
    }

    public virtual string Name
    {
      get {return "PlotItems"; }
    }

    /// <summary>
    /// Sets the parent layer.
    /// </summary>
    /// <param name="parent">The parent layer to set for this collection.</param>
    /// <param name="bSuppressEvents">If true, only the parent layer will set, but nothing else. If false, the boundaries of the items in the collection are merged into the parent layer collection.</param>
    /// <remarks>Use this with bSuppressEvents = true if you are in constructor or deserialization code where not all variables are currently initalized.</remarks>
    public void SetParentLayer(XYPlotLayer parent, bool bSuppressEvents)
    {
      if(null==parent)
      {
        throw new ArgumentNullException();
      }
      else
      {
        m_Owner = parent;
            
        if(!bSuppressEvents)
        {
          // if the owner changed, it has possibly other x and y axis boundaries, so we have to set the plot items to this new boundaries
          for(int i=0;i<Count;i++)
            SetItemBoundaries(this[i]);
        }
      }
    }

    public IHitTestObject HitTest(XYPlotLayer layer, PointF hitpoint)
    {
      IHitTestObject result;
      for(int i=0;i<Count;i++)
      {
        if(null!=(result=this[i].HitTest(layer,hitpoint)))
          return result;
      }
      return null;
    }

    /// <summary>
    /// Restores the event chain of a item.
    /// </summary>
    /// <param name="plotitem">The plotitem for which the event chain should be restored.</param>
    public void WireItem(Graph.PlotItem plotitem)
    {
      plotitem.ParentObject = this;
      SetItemBoundaries(plotitem);
      plotitem.Changed += new EventHandler(this.EhChildChanged);
    }

    /// <summary>
    /// This sets the type of the item boundaries to the type of the owner layer
    /// </summary>
    /// <param name="plotitem">The plot item for which the boundary type should be set.</param>
    public void SetItemBoundaries(Graph.PlotItem plotitem)
    {
      if(plotitem is IXBoundsHolder)
      {
        IXBoundsHolder pa = (IXBoundsHolder)plotitem;
        if(null!=m_Owner)
          pa.SetXBoundsFromTemplate(m_Owner.XAxis.DataBoundsObject); // ensure that data bound object is of the right type
        pa.XBoundariesChanged += new BoundaryChangedHandler(this.EhXBoundaryChanged);
        if(null!=m_Owner)
          pa.MergeXBoundsInto(m_Owner.XAxis.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
      }
      if(plotitem is IYBoundsHolder)
      {
        IYBoundsHolder pa = (IYBoundsHolder)plotitem;
        if(null!=m_Owner)
          pa.SetYBoundsFromTemplate(m_Owner.YAxis.DataBoundsObject); // ensure that data bound object is of the right type
        pa.YBoundariesChanged += new BoundaryChangedHandler(this.EhYBoundaryChanged);
        if(null!=m_Owner)
          pa.MergeYBoundsInto(m_Owner.YAxis.DataBoundsObject); // merge the y-boundaries in the y-Axis data boundaries
      }
    }

    public new void Clear()
    {
      m_PlotGroups.Clear();
      base.Clear();
    }

    public void Add(Graph.PlotItem plotitem)
    {
      if(plotitem==null)
        throw new ArgumentNullException();

      base.InnerList.Add(plotitem);
      WireItem(plotitem);
      OnChanged();
    }

    public void Remove(Graph.PlotItem plotitem)
    {
      int idx = IndexOf(plotitem);
      if(idx>=0)
      {
        RemoveAt(idx);
      }
    }

    public PlotItem this[int i]
    {
      get { return (PlotItem)base.InnerList[i]; }
    }
      
    public int IndexOf(PlotItem it)
    {
      return base.InnerList.IndexOf(it,0,Count);
    }

    public void EhXBoundaryChanged(object sender, BoundariesChangedEventArgs args)
    {
      if(null!=this.m_Owner)
        m_Owner.OnPlotAssociationXBoundariesChanged(sender,args);
    }
    
    public void EhYBoundaryChanged(object sender, BoundariesChangedEventArgs args)
    {
      if(null!=this.m_Owner)
        m_Owner.OnPlotAssociationYBoundariesChanged(sender,args);
    }

    public void EhPlotGroups_Changed(object sender, EventArgs e)
    {
      OnChanged();
    }

    #region NamedObjectCollection

    /// <summary>
    /// retrieves the object with the name <code>name</code>.
    /// </summary>
    /// <param name="name">The objects name.</param>
    /// <returns>The object with the specified name.</returns>
    public virtual object GetChildObjectNamed(string name)
    {
      double number;
      if(double.TryParse(name,System.Globalization.NumberStyles.Integer,System.Globalization.NumberFormatInfo.InvariantInfo, out number))
      {
        int idx=(int)number;
        if(idx>=0 && idx<this.Count)
          return this[idx];
      }
      return null;
    }

    /// <summary>
    /// Retrieves the name of the provided object.
    /// </summary>
    /// <param name="o">The object for which the name should be found.</param>
    /// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
    public virtual string GetNameOfChildObject(object o)
    {
      int idx = this.InnerList.IndexOf(o);
      return idx>=0 ? idx.ToString() : null;
    }

    #endregion

    #region IChangedEventSource Members

    public event System.EventHandler Changed;

   

    public virtual void EhChildChanged(object child, EventArgs e)
    {
      if(null!=Changed)
        Changed(this,e);
    }

    protected virtual void OnChanged()
    {
      if(null!=Changed)
        Changed(this,new Main.ChangedEventArgs(this,null));
    }

    #endregion

    #region PlotGroup handling

    public PlotGroup GetPlotGroupOf(PlotItem assoc)
    {
      return m_PlotGroups.GetPlotGroupOf(assoc);
    }

    /// <summary>
    /// Add the PlotGroup and all items in this group to the collection.
    /// </summary>
    /// <param name="pg"></param>
    public void Add(Altaxo.Graph.PlotGroup pg)
    {
      // 1. make sure that all PlotItems of this group are contained in our collection

      for(int i=0;i<pg.Count;i++)
      {
        PlotItem pa = pg[i];
        if(!base.InnerList.Contains(pa))
          this.Add(pa);
      }

      // 2. add the plotgroup to the plotgroup collection
      m_PlotGroups.Add(pg);
    }

    #endregion
  }


}
