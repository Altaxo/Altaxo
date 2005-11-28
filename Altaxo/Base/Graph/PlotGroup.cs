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
using Altaxo.Serialization;

namespace Altaxo.Graph
{
  [Flags]
  [Serializable]
  public enum PlotGroupStyle
  {
    // Note: we must provide every (!) combination a name, because of xml serialization
    None  = 0x00,
    Color = 0x01,
    Line  = 0x02,
    LineAndColor = Line | Color,
    Symbol = 0x04,
    SymbolAndColor= Symbol | Color,
    SymbolAndLine = Symbol | Line,
    All           = Symbol | Line | Color
  }

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotGroupStyle),0)]
  public class PlotGroupStyleTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      info.SetNodeContent(obj.ToString());  
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      
      string val = info.GetNodeContent();
      return System.Enum.Parse(typeof(PlotGroupStyle),val,true);
    }
  }

  
  /// <summary>
  /// Summary description for PlotGroup.
  /// </summary>
  [SerializationSurrogate(0,typeof(PlotGroup.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class PlotGroup : System.Runtime.Serialization.IDeserializationCallback
  {
    /// <summary>
    /// Designates which dependencies the plot styles have on each other.
    /// </summary>
    PlotGroupStyle _changeStyle;
    bool _changeStylesConcurrently;
    bool _changeStylesStrictly;

    System.Collections.ArrayList _plotItems;
    
    private PlotGroup.Collection _parent;

    private int _suppressStyleChangedEvents=0;

    #region Serialization
    /// <summary>Used to serialize the PlotGroup Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes PlotGroup Version 0.
      /// </summary>
      /// <param name="obj">The PlotGroup to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        PlotGroup s = (PlotGroup)obj;
        info.AddValue("Style",s._changeStyle);  
        info.AddValue("Group",s._plotItems);  
      }
      /// <summary>
      /// Deserializes the PlotGroup Version 0.
      /// </summary>
      /// <param name="obj">The empty axis object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized PlotGroup.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        PlotGroup s = (PlotGroup)obj;

        s._changeStyle = (PlotGroupStyle)info.GetValue("Style",typeof(PlotGroupStyle));
        s._plotItems = (System.Collections.ArrayList)info.GetValue("Group",typeof(System.Collections.ArrayList));
        return s;
      }
    }


    public class Memento
    {
      PlotGroupStyle m_Style;
      bool _concurrently;
      bool _strict;
      int[] _plotItemIndices; // stores not the plotitems itself, only the position of the items in the list
    
      public Memento(PlotGroup pg, PlotItemCollection plotlist)
      {
        m_Style = pg.Style;
        _concurrently = pg.ChangeStylesConcurrently;
        _strict = pg.ChangeStylesStrictly;

        _plotItemIndices = new int[pg.Count];
        for(int i=0;i<_plotItemIndices.Length;i++)
          _plotItemIndices[i] = plotlist.IndexOf(pg[i]);
      }
    
      protected Memento()
      {
      }

      public PlotGroup GetPlotGroup(PlotItemCollection plotlist)
      {
        PlotGroup pg = new PlotGroup(m_Style,_concurrently,_strict);
        for(int i=0;i<_plotItemIndices.Length;i++)
          pg.Add(plotlist[i]);
        return pg;
      }

      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotGroup.Memento),0)]
        public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          PlotGroup.Memento s = (PlotGroup.Memento)obj;
          info.AddValue("Style",s.m_Style);  
          info.CreateArray("PlotItems", s._plotItemIndices.Length);
          for(int i=0;i<s._plotItemIndices.Length;i++)
            info.AddValue("PlotItem",s._plotItemIndices[i]);
          info.CommitArray();
        }

        public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
        {
          
          PlotGroup.Memento s = null!=o ? (PlotGroup.Memento)o : new PlotGroup.Memento();
          s.m_Style = (PlotGroupStyle)info.GetValue("Style",typeof(PlotGroupStyle));

          int count = info.OpenArray();
          s._plotItemIndices = new int[count];
          for(int i=0;i<count;i++)
          {
            s._plotItemIndices[i] = info.GetInt32();
          }
          info.CloseArray(count);

          return s;
        }
      }
    }
    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      if(_plotItems.Count>0)
        ((PlotItem)_plotItems[0]).StyleChanged += new EventHandler(this.OnMasterStyleChangedEventHandler);
    }
    #endregion



    public PlotGroup(PlotItem master, PlotGroupStyle style, bool concurrently, bool strict)
    {
      _changeStyle = style;
      _changeStylesConcurrently = concurrently;
      _changeStylesStrictly = strict;

      _plotItems = new System.Collections.ArrayList();
      _plotItems.Add(master);
    }

    public PlotGroup(PlotGroupStyle style, bool concurrently, bool strict)
    {
      _changeStyle = style;
      _changeStylesConcurrently = concurrently;
      _changeStylesStrictly = strict;

      _plotItems = new System.Collections.ArrayList();
    }

    protected PlotGroup()
    {
    }

    /// <summary>
    /// This is !!! not !!! cloneable, since the PlotGroup itself stores only references to PlotItems! Since the only use
    /// is to clone a XYPlotLayer, and the PlotItems of the layer are cloned into new objects, it is not usefull here.
    /// </summary>
    /// <returns>Null!</returns>
    private object Clone()
    {
      return null; // see above for explanation
    }

    /// <summary>
    /// Clones the PlotGroup into a new collection and fixed the references to the PlotItems. It presumes, that the PlotItemCollection, from which the PlotItems are referred in the PlotGroup items,
    /// are cloned before so that the PlotItemCollection <paramref name="newList"/> is an exact copy of the PlotItemCollection <paramref name="oldList"/>.
    /// </summary>
    /// <param name="newList">The new PlotItemCollection, which was cloned from oldList before.</param>
    /// <param name="oldList">The old PlotItemCollection, to which the items in these PlotGroupList refers to.</param>
    /// <returns></returns>
    public PlotGroup Clone(Altaxo.Graph.PlotItemCollection newList, Altaxo.Graph.PlotItemCollection oldList)
    {
      PlotGroup newGroup = new PlotGroup(this.Style,this._changeStylesConcurrently,this._changeStylesStrictly);

      for(int i=0;i<this.Count;i++)
      {
        // look for the position of the PlotItem in the old list
        int position = oldList.IndexOf(this[i]);

        if(position>=0)
          newGroup.Add(newList[position]);
      }
      return newGroup;
    }

    public int Count
    {
      get { return null!=_plotItems ? _plotItems.Count : 0; }
    }

    public PlotItem this[int i]
    {
      get { return (PlotItem)_plotItems[i]; }
    }

    public void Add(PlotItem assoc)
    {
      if(null!=assoc)
      {
        /*
        int cnt = _plotItems.Count;
        if(cnt==0) // this is the first, i.e. the master item, it must be wired by a Changed event handler
        {
          assoc.StyleChanged += new EventHandler(this.OnMasterStyleChangedEventHandler);
        }
        if(cnt>0)
        {
          if(assoc is I2DPlotItem && _plotItems[0] is I2DPlotItem)
            ((I2DPlotItem)assoc).SetIncrementalStyle((I2DPlotItem)_plotItems[0],_changeStyle,_changeStylesConcurrently,_changeStylesStrictly, cnt);
        }
        */
        _plotItems.Add(assoc);

        OnChanged();
      }
    }

    /// <summary>
    /// Sets the style properties of this plot group.
    /// </summary>
    /// <param name="style">The information about what to vary.</param>
    /// <param name="concurrently">If true, all styles are varied concurrently.</param>
    /// <param name="strict">If true, the slave plot items are enforced to have the same properties than the master plot item.</param>
    /// <returns>True when at least one of the properties was changed (i.e. different).</returns>
    public bool SetPropertiesOnly(PlotGroupStyle style, bool concurrently, bool strict)
    {
      bool bChanged = _changeStyle != style || _changeStylesConcurrently != concurrently || _changeStylesStrictly != strict;

      _changeStyle = style;
      _changeStylesConcurrently = concurrently;
      _changeStylesStrictly = strict;

      if (bChanged)
        OnChanged();

      return bChanged;
    }

    public bool Contains(PlotItem assoc)
    {
      return this._plotItems.Contains(assoc);
    }

    public void Clear()
    {
      _plotItems.Clear();

      OnChanged();
    }

    public PlotItem MasterItem
    {
      get { return _plotItems.Count>0 ? (PlotItem)_plotItems[0] : null; }
    }

    /*
    public bool IsIndependent 
    {
      get { return this._changeStyle == 0; }
    }
    */

    public PlotGroupStyle Style
    {
      get { return this._changeStyle; }
      set 
      {
        bool changed = (this._changeStyle!=value);
        this._changeStyle = value;
        
        // update the styles beginning from the master item
        if(changed)
        {
          UpdateMembers();
          OnChanged();
        }
      }
    }

    public bool ChangeStylesConcurrently
    {
      get { return this._changeStylesConcurrently; }
      set
      {
        bool changed = (_changeStylesConcurrently != value);
        this._changeStylesConcurrently = value;

        // update the styles beginning from the master item
        if (changed)
        {
          UpdateMembers();
          OnChanged();
        }
      }
    }

    public bool ChangeStylesStrictly
    {
      get { return this._changeStylesStrictly; }
      set
      {
        bool changed = (_changeStylesStrictly != value);
        this._changeStylesStrictly = value;

        // update the styles beginning from the master item
        if (changed)
        {
          UpdateMembers();
          OnChanged();
        }
      }
    }

    public void UpdateMembers(PlotGroupStyle groupstyle, object plotitem)
    {
      bool changed = (this._changeStyle!=groupstyle);
      _changeStyle = groupstyle;

      UpdateMembers(plotitem);
      if(changed)
        OnChanged();

    }
   

    public void UpdateMembers( object plotitem)
    {
      for(int i=0;i<Count;i++)
      {
        if(object.ReferenceEquals(this[i],plotitem))
        {
          UpdateMembers(i);
          return;
        }
      }
      
    }

    public void UpdateMembers(int masteritem)
    {
      ++_suppressStyleChangedEvents;
      // update the styles beginning from the master item
      if(masteritem<Count)
      {
        I2DGroupablePlotStyle masterstyle = this[masteritem].StyleObject as I2DGroupablePlotStyle;
        if(masterstyle!=null)
        {
          for(int i=0;i<Count;i++)
          {
            if(i==masteritem)
              continue;
            if(this[i].StyleObject is I2DGroupablePlotStyle)
              ((I2DGroupablePlotStyle)this[i].StyleObject).SetIncrementalStyle(masterstyle,this._changeStyle, this._changeStylesConcurrently, this._changeStylesStrictly, i-masteritem);
          }
        }
        // no changed event here since we track only the members structure and the grouping style
      }
      --_suppressStyleChangedEvents;
    }

    public void UpdateMembers()
    {
      UpdateMembers(0);
    }

    protected void OnMasterStyleChangedEventHandler(object sender, EventArgs e)
    {
      /*
      if(this._suppressStyleChangedEvents<=0)
        UpdateMembers();
       */
    }

    protected virtual void OnChanged()
    {
      if(null!=_parent)
        _parent.OnChildChangedEventHandler(this);
    }

    [SerializationSurrogate(0,typeof(PlotGroup.Collection.SerializationSurrogate0))]
      [SerializationVersion(0)]
      public class Collection : Altaxo.Data.CollectionBase, Main.IChangedEventSource
    {
      #region "Serialization"

      /// <summary>Used to serialize the Collection Version 0.</summary>
      public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
      {

        /// <summary>
        /// Serializes XYPlotLayerCollection Version 0.
        /// </summary>
        /// <param name="obj">The Collection to serialize.</param>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
        {
          PlotGroup.Collection s = (PlotGroup.Collection)obj;
          info.AddValue("Data",s.myList);
        }

        /// <summary>
        /// Deserializes the Collection Version 0.
        /// </summary>
        /// <param name="obj">The empty  object to deserialize into.</param>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        /// <param name="selector">The deserialization surrogate selector.</param>
        /// <returns>The deserialized Collection.</returns>
        public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
        {
          PlotGroup.Collection s = (PlotGroup.Collection)obj;

          s.myList =  (System.Collections.ArrayList)info.GetValue("Data",typeof(System.Collections.ArrayList));
    
          return s;
        }
      }

      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotGroup.Collection),0)]
        public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          PlotGroup.Collection s = (PlotGroup.Collection)obj;
            
          info.CreateArray("PlotGroups",s.Count);
          for(int i=0;i<s.Count;i++)
            info.AddValue("PlotGroup",s.myList[i]);
          info.CommitArray();
        }
        public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
        {
            
          PlotGroup.Collection s = null!=o ? (PlotGroup.Collection)o : new PlotGroup.Collection();

          int count = info.OpenArray();
          for(int i=0;i<count;i++)
          {
            PlotGroup gr = (PlotGroup)info.GetValue("PlotGroup",s);
            s.Add(gr);
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
      }
      #endregion


      /// <summary>
      /// Clones the collection into a new collection and fixed the references. It presumes, that the PlotItemCollection, from which the PlotItems are referred in the PlotGroup items,
      /// are cloned before so that the PlotItemCollection <paramref name="newList"/> is an exact copy of the PlotItemCollection <paramref name="oldList"/>.
      /// </summary>
      /// <param name="newList">The new PlotItemCollection, which was cloned from oldList before.</param>
      /// <param name="oldList">The old PlotItemCollection, to which the items in these PlotGroupList refers to.</param>
      /// <returns></returns>
      public PlotGroup.Collection Clone(Altaxo.Graph.PlotItemCollection newList, Altaxo.Graph.PlotItemCollection oldList)
      {
        PlotGroup.Collection coto = new PlotGroup.Collection();
        for(int i=0;i<this.Count;i++)
          coto.Add(((PlotGroup)base.InnerList[i]).Clone(newList,oldList));
        
        return coto;
      }

      public void Add(PlotGroup g)
      {
        g._parent=this;
        base.InnerList.Add(g);

        OnChanged();
      }

      public new void Clear()
      {
        base.InnerList.Clear();

        OnChanged();
      }

      public PlotGroup GetPlotGroupOf(PlotItem assoc)
      {
        // search for the (first) plot group, to which assoc belongs,
        // and return this group

        for(int i=0;i<Count;i++)
          if(((PlotGroup)base.InnerList[i]).Contains(assoc) )
            return ((PlotGroup)base.InnerList[i]);

        return null; // assoc belongs not to any plot group
      }

      public PlotGroup this[int i]
      {
        get { return (PlotGroup)this.InnerList[i]; }
      }


      #region IChangedEventSource Members

      public event System.EventHandler Changed;

      protected virtual void OnChanged()
      {
        if(null!=Changed)
          Changed(this,new EventArgs());
      }

      public virtual void OnChildChangedEventHandler(object sender)
      {
        OnChanged();
      }


      #endregion
    } // end of class Collection

  }
}
