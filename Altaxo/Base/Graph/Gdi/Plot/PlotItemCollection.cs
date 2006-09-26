using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Gdi.Plot
{
  using Plot.Groups;
  using PlotGroups;

  [Serializable]
  public class PlotItemCollection 
    :
    IGPlotItem, 
    IEnumerable<IGPlotItem>,
    Main.IChangedEventSource,
    Main.IChildChangedEventSink,
    Main.IDocumentNode,
    Main.INamedObjectCollection,
    IXBoundsHolder,
    IYBoundsHolder
  {
    G2DPlotGroupStyleCollection _styles;
    List<IGPlotItem> _plotItems;

    [NonSerialized]
    IGPlotItem[] _cachedPlotItemsFlattened;

    /// <summary>The parent layer of this list.</summary>
    [NonSerialized]
    private object _parent;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PlotItemCollection", 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("Programming error - trying to serialize an old version of PlotItemCollection");
        /*
        PlotItemCollection s = (PlotItemCollection)obj;

        info.CreateArray("PlotItems", s.Count);
        for (int i = 0; i < s.Count; i++)
          info.AddValue("PlotItem", s[i]);
        info.CommitArray();

        // now serialize the PlotGroups
        info.CreateArray("PlotGroups", s.m_PlotGroups.Count);
        for (int i = 0; i < s.m_PlotGroups.Count; i++)
        {
          PlotGroup pg = (PlotGroup)s.m_PlotGroups[i];
          info.AddValue("PlotGroup", new PlotGroup.Memento(pg, s));
        }
        info.CommitArray(); // PlotGroups
        */
      }

      struct PGTrans
      {
        public PlotGroupMemento PlotGroup;
        public PlotItemCollection PlotItemCollection;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        PlotItemCollection s = null != o ? (PlotItemCollection)o : new PlotItemCollection();

        int count = info.OpenArray();
        IGPlotItem[] plotItems = new IGPlotItem[count];
        for (int i = 0; i < count; i++)
        {
          plotItems[i] = (IGPlotItem)info.GetValue("PlotItem", s);
        }
        info.CloseArray(count);


        count = info.OpenArray(); // PlotGroups
        PGTrans[] plotGroups = new PGTrans[count];
        for (int i = 0; i < count; i++)
        {
          plotGroups[i].PlotGroup = (PlotGroupMemento)info.GetValue(s);
        }
        info.CloseArray(count);

        // now assemble the new tree based collection based on the both fields

        for (int pix = 0; pix < plotItems.Length; pix++)
        {
          // look if this plotItem is member of some group
          int foundidx = -1;
          for (int grx = 0; grx < plotGroups.Length; grx++)
          {
            if (Array.IndexOf<int>(plotGroups[grx].PlotGroup._plotItemIndices, pix) >= 0)
            {
              foundidx = grx; 
            break;
            }
          }
          if (foundidx < 0) // if not found in some group, add the item directly
          {
            s.Add(plotItems[pix]); 
          }
          else
          {
            
            if (plotGroups[foundidx].PlotItemCollection == null)
            {
              PlotItemCollection newColl = new PlotItemCollection();
              plotGroups[foundidx].PlotItemCollection = newColl;
              s.Add(plotGroups[foundidx].PlotItemCollection);
              // now set the properties of this new collection
              bool serial = !plotGroups[foundidx].PlotGroup._concurrently;
              IPlotGroupStyle curr = null;
              IPlotGroupStyle prev = null;
              if (0 != (plotGroups[foundidx].PlotGroup._plotGroupStyle & Version0PlotGroupStyle.Color))
              {
                curr = new ColorGroupStyle();
                newColl.GroupStyles.Add(curr);
              }
              if (0 != (plotGroups[foundidx].PlotGroup._plotGroupStyle & Version0PlotGroupStyle.Line))
              {
                prev = curr;
                curr = new LineStyleGroupStyle();
                newColl.GroupStyles.Add(curr, serial ? prev.GetType() : null);
              }
              if (0 != (plotGroups[foundidx].PlotGroup._plotGroupStyle & Version0PlotGroupStyle.Symbol))
              {
                prev = curr;
                curr = new SymbolShapeStyleGroupStyle();
                newColl.GroupStyles.Add(curr, serial ? prev.GetType() : null);
              }
            }
            // now add the item to this collection
            plotGroups[foundidx].PlotItemCollection.Add(plotItems[pix]);
          }
        }

        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.PlotItemCollection", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotItemCollection), 2)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PlotItemCollection s = (PlotItemCollection)obj;

        info.CreateArray("PlotItems", s.Count);
        for (int i = 0; i < s.Count; i++)
          info.AddValue("PlotItem", s[i]);
        info.CommitArray();

        info.AddValue("GroupStyles", s._styles);
      }
     

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        PlotItemCollection s = null != o ? (PlotItemCollection)o : new PlotItemCollection();

        int count = info.OpenArray();
        IGPlotItem[] plotItems = new IGPlotItem[count];
        for (int i = 0; i < count; i++)
        {
          s.Add((IGPlotItem)info.GetValue("PlotItem", s));
        }
        info.CloseArray(count);

        s._styles = (G2DPlotGroupStyleCollection)info.GetValue("GroupStyles", s);

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
      for (int i = 0; i < Count; i++)
        WireItem(this[i]);
    }

    #endregion

    #region Constructors

    public PlotItemCollection(XYPlotLayer owner)
    {
      _parent = owner;
      _plotItems = new List<IGPlotItem>();
      _styles = new G2DPlotGroupStyleCollection();
    }

    public PlotItemCollection()
    {
      _plotItems = new List<IGPlotItem>();
      _styles = new G2DPlotGroupStyleCollection();
    }

    /// <summary>
    /// Empty constructor for deserialization.
    /// </summary>
    protected PlotItemCollection(int x)
    {
      _styles = new G2DPlotGroupStyleCollection();
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
      _parent = owner;
      _styles = new G2DPlotGroupStyleCollection();
      _plotItems = new List<IGPlotItem>();


      // Clone all the items in the list.
      for(int i=0;i<from.Count;i++)
        Add((IGPlotItem)from[i].Clone()); // clone the items

      // special way neccessary to handle plot groups
      this._styles = null == from._styles ? null : from._styles.Clone();
    }

    object ICloneable.Clone()
    {
      return new PlotItemCollection(this);
    }

    public PlotItemCollection Clone()
    {
      return new PlotItemCollection(this);
    }

    #endregion

    #region Other stuff

    public XYPlotLayer ParentLayer
    {
      get { return _parent as XYPlotLayer; }
      set
      {
        SetParentLayer(value, false);
      }
    }

    public PlotItemCollection ParentCollection
    {
      get
      {
        return _parent as PlotItemCollection;
      }
    }

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public G2DPlotGroupStyleCollection GroupStyles
    {
      get
      {
        return this._styles;
      }
    }

    public virtual string Name
    {
      get { return "PlotItems"; }
    }

    /// <summary>
    /// Sets the parent layer.
    /// </summary>
    /// <param name="parent">The parent layer to set for this collection.</param>
    /// <param name="bSuppressEvents">If true, only the parent layer will set, but nothing else. If false, the boundaries of the items in the collection are merged into the parent layer collection.</param>
    /// <remarks>Use this with bSuppressEvents = true if you are in constructor or deserialization code where not all variables are currently initalized.</remarks>
    public void SetParentLayer(XYPlotLayer parent, bool bSuppressEvents)
    {
      if (null == parent)
      {
        throw new ArgumentNullException();
      }
      else
      {
        _parent = parent;
      }
    }
    #endregion
 
    #region IG2DPlotItem Members

    /// <summary>
    /// Collects all possible group styles that can be applied to this plot item in
    /// styles.
    /// </summary>
    /// <param name="styles">The collection of group styles.</param>
    public void CollectStyles(G2DPlotGroupStyleCollection styles)
    {
      foreach (IGPlotItem pi in _plotItems)
        pi.CollectStyles(styles);
    }

    public void PrepareStyles(G2DPlotGroupStyleCollection styles)
    {
        _styles.BeginPrepare();

        foreach (IGPlotItem pi in _plotItems)
        {
          pi.PrepareStyles(_styles);
          _styles.PrepareStep();
        }

        _styles.EndPrepare();
    }

    public void ApplyStyles(G2DPlotGroupStyleCollection styles)
    {
      ApplyStyles(styles, 0);
    }

    public void ApplyStyles(G2DPlotGroupStyleCollection styles, IGPlotItem pivot)
    {
      int pivotidx = _plotItems.IndexOf(pivot);
      if (pivotidx < 0)
        pivotidx = 0;
      ApplyStyles(styles, pivotidx);
    }


    protected void ApplyStyles(G2DPlotGroupStyleCollection styles, int pivotidx)
    {
      _styles.BeginApply();
      for(int i=pivotidx;i<_plotItems.Count;i++)
      {
        IGPlotItem pi = _plotItems[i];
        pi.ApplyStyles(_styles);
        _styles.Step(1);
      }
      _styles.EndApply();

      if (pivotidx > 0)
      {
        _styles.BeginApply();
        for (int i = pivotidx; i >=0; i--)
        {
          IGPlotItem pi = _plotItems[i];
          pi.ApplyStyles(_styles);
          _styles.Step(-1);
        }
        _styles.EndApply();
      }
    }


    /// <summary>
    /// Paints a symbol for this plot item for use in a legend.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="location">The rectangle where the symbol should be painted into.</param>
    public virtual void PaintSymbol(Graphics g, RectangleF location)
    {
    }

    public string GetName(int level)
    {
      return string.Format("<Collection of {0} plot items>", this._plotItems.Count); 
    }

    public string GetName(string style)
    {
      return string.Format("<Collection of {0} plot items>", this._plotItems.Count);
    }

    public void PreparePainting(IPlotArea layer)
    {
      foreach (IGPlotItem pi in _plotItems)
        pi.PreparePainting(layer);
    }

    public void Paint(System.Drawing.Graphics g, IPlotArea layer)
    {
      IG2DCoordinateTransformingGroupStyle coordTransStyle;
      if (null != (coordTransStyle = _styles.CoordinateTransformingStyle))
        coordTransStyle.Paint(g, layer, this);
      else
      {
        foreach (IGPlotItem pi in _plotItems)
          pi.Paint(g, layer);
      }
    }


    #region Hit test

    public IHitTestObject HitTest(IPlotArea layer, System.Drawing.PointF hitpoint)
    {
      IHitTestObject result = null;
      foreach (IGPlotItem pi in _plotItems)
      {
        result = pi.HitTest(layer, hitpoint);
        if (null != result)
        {
          if(result.Remove==null)
            result.Remove = new DoubleClickHandler(this.EhHitTestObject_Remove);

          return result;
        }
      }
      return null;
    }


    /// <summary>
    /// Handles the remove of a plot item.
    /// </summary>
    /// <param name="target">The target hit (should be plot item) to remove.</param>
    /// <returns>True if the item was removed.</returns>
    bool EhHitTestObject_Remove(IHitTestObject target)
    {
      return this.Remove(target.HittedObject as IGPlotItem);
    }

    #endregion
  
    #endregion

    #region IEnumerable<IG2DPlotItem> Members

    public IEnumerator<IGPlotItem> GetEnumerator()
    {
      return _plotItems.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _plotItems.GetEnumerator();
    }

    #endregion

    #region Other collection methods

    public int Count
    {
      get { return _plotItems.Count; }
    }

    public void Add(IGPlotItem item)
    {
      if (item == null)
        throw new ArgumentNullException();

      _plotItems.Add(item);

      WireItem(item);
      OnCollectionChanged();
      OnChanged();
    }

    public void Clear()
    {
      _plotItems.Clear();
      _styles.Clear();
      OnCollectionChanged();
    }

    public IGPlotItem this[int i]
    {
      get { return _plotItems[i]; }
    }

    /// <summary>
    /// Removes an plot item from the list.
    /// </summary>
    /// <param name="plotitem">The item to remove.</param>
    /// <returns>True if the item was removed. False otherwise.</returns>
    public bool Remove(IGPlotItem plotitem)
    {
      int idx = _plotItems.IndexOf(plotitem);
      if (idx >= 0)
      {
        _plotItems.RemoveAt(idx);
        OnCollectionChanged();
        return true;
      }
      return false;
    }

    public int IndexOf(IGPlotItem it)
    {
      return _plotItems.IndexOf(it, 0, Count);
    }

    /// <summary>
    /// This method must be called, if plot item members are added or removed to this collection.
    /// </summary>
    protected virtual void OnCollectionChanged()
    {
     _cachedPlotItemsFlattened = null;

      if (_parent is PlotItemCollection)
        ((PlotItemCollection)_parent).OnCollectionChanged();
    }

    protected void FillPlotItemList(IList<IGPlotItem> list)
    {
      foreach (IGPlotItem pi in _plotItems)
        if (pi is PlotItemCollection)
          ((PlotItemCollection)pi).FillPlotItemList(list);
        else
          list.Add(pi);
    }

    public IGPlotItem[] Flattened
    {
      get
      {
        if (_cachedPlotItemsFlattened == null)
        {
          List<IGPlotItem> list = new List<IGPlotItem>();
          FillPlotItemList(list);
          _cachedPlotItemsFlattened = list.ToArray();
        }
        return _cachedPlotItemsFlattened;
      }
    }

    #endregion

    #region NamedObjectCollection

    /// <summary>
    /// retrieves the object with the name <code>name</code>.
    /// </summary>
    /// <param name="name">The objects name.</param>
    /// <returns>The object with the specified name.</returns>
    public virtual object GetChildObjectNamed(string name)
    {
      double number;
      if (double.TryParse(name, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out number))
      {
        int idx = (int)number;
        if (idx >= 0 && idx < this.Count)
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
      if (o is IGPlotItem)
      {
        int idx = _plotItems.IndexOf((IGPlotItem)o);
        return idx >= 0 ? idx.ToString() : null;
      }
      return null;
    }

    #endregion

    #region IChangedEventSource Members

    public event System.EventHandler Changed;



    public virtual void EhChildChanged(object child, EventArgs e)
    {
      if (null != Changed)
        Changed(this, e);
    }

    protected virtual void OnChanged()
    {
      if (null != Changed)
        Changed(this, new Main.ChangedEventArgs(this, null));
    }

    #endregion

    #region PlotGroup handling

    

    /// <summary>
    /// Add the PlotGroupStyle.
    /// </summary>
    /// <param name="pg">The plot group style to add.</param>
    public void Add(IPlotGroupStyle pg)
    {
      this._styles.Add(pg);
      OnChanged();
    }

    public void ListPossiblePlotGroupStyles()
    {
      foreach (IGPlotItem pi in _plotItems)
      {
        
      }
    }



    #endregion

    #region Plot Item bounds

    /// <summary>
    /// Restores the event chain of a item.
    /// </summary>
    /// <param name="plotitem">The plotitem for which the event chain should be restored.</param>
    public void WireItem(IGPlotItem plotitem)
    {
      plotitem.ParentObject = this;
      WireBoundaryEvents(plotitem);
      plotitem.Changed += new EventHandler(this.EhChildChanged);
    }

    /// <summary>
    /// This sets the type of the item boundaries to the type of the owner layer
    /// </summary>
    /// <param name="plotitem">The plot item for which the boundary type should be set.</param>
    void WireBoundaryEvents(IGPlotItem plotitem)
    {
      if (plotitem is IXBoundsHolder)
      {
        IXBoundsHolder xholder = (IXBoundsHolder)plotitem;
        xholder.XBoundariesChanged += new BoundaryChangedHandler(this.EhXBoundaryChanged);
      }
      if (plotitem is IYBoundsHolder)
      {
        IYBoundsHolder yholder = (IYBoundsHolder)plotitem;
        yholder.YBoundariesChanged += new BoundaryChangedHandler(this.EhYBoundaryChanged);
      }
    }

    #endregion

    #region Event Handling

    public void EhXBoundaryChanged(object sender, BoundariesChangedEventArgs args)
    {
      if (this._parent is XYPlotLayer)
        ((XYPlotLayer)_parent).OnPlotAssociationXBoundariesChanged(sender, args);
      else if (this._parent is PlotItemCollection)
        ((PlotItemCollection)_parent).EhXBoundaryChanged(sender, args);
    }

    public void EhYBoundaryChanged(object sender, BoundariesChangedEventArgs args)
    {
      if (this._parent is XYPlotLayer)
        ((XYPlotLayer)_parent).OnPlotAssociationYBoundariesChanged(sender, args);
      else if (this._parent is PlotItemCollection)
        ((PlotItemCollection)_parent).EhYBoundaryChanged(sender, args);
    }

    public void EhPlotGroups_Changed(object sender, EventArgs e)
    {
      OnChanged();
    }



    #endregion

    #region IXBoundsHolder Members

    [field:NonSerialized]
    public event BoundaryChangedHandler XBoundariesChanged;

    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      IG2DCoordinateTransformingGroupStyle coordTransStyle;
      if (null != (coordTransStyle = _styles.CoordinateTransformingStyle))
        coordTransStyle.MergeXBoundsInto(this.ParentLayer, pb, this);
      else
        CoordinateTransformingStyleBase.MergeXBoundsInto(pb, this);
    }

    #endregion

    #region IYBoundsHolder Members

    [field:NonSerialized]
    public event BoundaryChangedHandler YBoundariesChanged;

    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      IG2DCoordinateTransformingGroupStyle coordTransStyle;
      if (null != (coordTransStyle = _styles.CoordinateTransformingStyle))
        coordTransStyle.MergeYBoundsInto(this.ParentLayer, pb, this);
      else
        CoordinateTransformingStyleBase.MergeYBoundsInto(pb, this);

    }

    #endregion

    #region deprecated stuff for deserialisation


    enum Version0PlotGroupStyle
    {
      // Note: we must provide every (!) combination a name, because of xml serialization
      None = 0x00,
      Color = 0x01,
      Line = 0x02,
      LineAndColor = Line | Color,
      Symbol = 0x04,
      SymbolAndColor = Symbol | Color,
      SymbolAndLine = Symbol | Line,
      All = Symbol | Line | Color
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.PlotGroupStyle", 0)]
    class PlotGroupStyleTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("This is deprectated stuff");
        //info.SetNodeContent(obj.ToString());
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(Version0PlotGroupStyle), val, true);
      }
    }

    /// <summary>
    /// Deprectated stuff neccessary to deserialize PlotItemCollection Version 0.
    /// </summary>
     class PlotGroupMemento
    {
      public Version0PlotGroupStyle _plotGroupStyle;
      public bool _concurrently;
      public PlotGroupStrictness _plotGroupStrictness;
      public int[] _plotItemIndices; // stores not the plotitems itself, only the position of the items in the list

      protected PlotGroupMemento()
      {
      }
      /*
      public PlotGroupMemento(PlotGroup pg, PlotItemCollection plotlist)
      {
        m_Style = pg.Style;
        _concurrently = pg.ChangeStylesConcurrently;
        _strict = pg.ChangeStylesStrictly;

        _plotItemIndices = new int[pg.Count];
        for (int i = 0; i < _plotItemIndices.Length; i++)
          _plotItemIndices[i] = plotlist.IndexOf(pg[i]);
      }

     

      public PlotGroup GetPlotGroup(PlotItemCollection plotlist)
      {
        PlotGroup pg = new PlotGroup(m_Style, _concurrently, _strict);
        for (int i = 0; i < _plotItemIndices.Length; i++)
          pg.Add(plotlist[i]);
        return pg;
      }
      */


      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.PlotGroup+Memento", 0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          throw new NotImplementedException("This is deprecated stuff");
          /*
          PlotGroup.Memento s = (PlotGroup.Memento)obj;
          info.AddValue("Style", s.m_Style);
          info.CreateArray("PlotItems", s._plotItemIndices.Length);
          for (int i = 0; i < s._plotItemIndices.Length; i++)
            info.AddValue("PlotItem", s._plotItemIndices[i]);
          info.CommitArray();
          */
        }


        public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
        {
          PlotGroupMemento s = SDeserialize(o, info, parent);
          return s;
        }
        public virtual PlotGroupMemento SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
        {

          PlotGroupMemento s = null != o ? (PlotGroupMemento)o : new PlotGroupMemento();
          s._plotGroupStyle = (Version0PlotGroupStyle)info.GetValue("Style", typeof(Version0PlotGroupStyle));

          int count = info.OpenArray();
          s._plotItemIndices = new int[count];
          for (int i = 0; i < count; i++)
          {
            s._plotItemIndices[i] = info.GetInt32();
          }
          info.CloseArray(count);

          return s;
        }
      }

      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PlotGroup+Memento", 1)]
      public class XmlSerializationSurrogate1 : XmlSerializationSurrogate0
      {
        public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          throw new NotImplementedException("This is deprecated stuff");
          /*
          base.Serialize(obj, info);
          PlotGroup.Memento s = (PlotGroup.Memento)obj;
          info.AddValue("Concurrently", s._concurrently);
          info.AddEnum("Strict", s._strict);
          */
        }

        public override PlotGroupMemento SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
        {

          PlotGroupMemento s = base.SDeserialize(o, info, parent);

          s._concurrently = info.GetBoolean("Concurrently");
          s._plotGroupStrictness = (PlotGroupStrictness)info.GetEnum("Strict", typeof(PlotGroupStrictness));

          return s;
        }
      }
    }
  
    #endregion
  }
}
