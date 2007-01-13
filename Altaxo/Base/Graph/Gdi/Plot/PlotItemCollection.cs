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
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Gdi.Plot
{
  using Plot.Groups;
  using Graph.Plot.Groups;

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
    PlotGroupStyleCollection _styles;
    List<IGPlotItem> _plotItems;

    [NonSerialized]
    IGPlotItem[] _cachedPlotItemsFlattened;

    /// <summary>The parent layer of this list.</summary>
    [NonSerialized]
    private object _parent;

    [field: NonSerialized]
    public event System.EventHandler Changed;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PlotItemCollection", 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
                newColl.GroupStyles.Add(curr, serial ? (prev==null?null:prev.GetType()) : null);
              }
              if (0 != (plotGroups[foundidx].PlotGroup._plotGroupStyle & Version0PlotGroupStyle.Symbol))
              {
                prev = curr;
                curr = new SymbolShapeStyleGroupStyle();
                newColl.GroupStyles.Add(curr, serial ? (prev == null ? null : prev.GetType()) : null);
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
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

        s._styles = (PlotGroupStyleCollection)info.GetValue("GroupStyles", s);

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
      _styles = new PlotGroupStyleCollection();
    }

    public PlotItemCollection()
    {
      _plotItems = new List<IGPlotItem>();
      _styles = new PlotGroupStyleCollection();
    }

    /// <summary>
    /// Empty constructor for deserialization.
    /// </summary>
    protected PlotItemCollection(int x)
    {
      _styles = new PlotGroupStyleCollection();
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
      _styles = new PlotGroupStyleCollection();
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
      get 
      {
        return Main.DocumentPath.GetRootNodeImplementing<XYPlotLayer>(this); 
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

    public PlotGroupStyleCollection GroupStyles
    {
      get
      {
        return this._styles;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        this._styles = value;
      }
    }

    public virtual string Name
    {
      get { return "PlotItems"; }
    }
   
    #endregion
 
    #region IG2DPlotItem Members

    /// <summary>
    /// Collects all possible group styles that can be applied to this plot item in
    /// styles.
    /// </summary>
    /// <param name="styles">The collection of group styles.</param>
    public void CollectStyles(PlotGroupStyleCollection styles)
    {
      foreach (IGPlotItem pi in _plotItems)
        pi.CollectStyles(styles);
    }

    public void PrepareScales(IPlotArea layer)
    {
      foreach (IGPlotItem pi in _plotItems)
        pi.PrepareScales(layer);
    }

    public void PrepareStyles(PlotGroupStyleCollection styles, IPlotArea layer)
    {
      PrepareStylesForward_HierarchyUpOnly(styles,layer);
    }

    public void ApplyStyles(PlotGroupStyleCollection styles)
    {
      ApplyStylesForward_HierarchyUpOnly(styles);
    }


    /// <summary>
    /// Distribute the changes made to the plotitem 'pivotitem' to all other items in the collection and if neccessary, also up and down the plot item tree.
    /// </summary>
    /// <param name="pivotitem">The plot item where changes to the plot item's styles were made.</param>
    public void DistributeChanges(IGPlotItem pivotitem)
    {
      int pivotidx = _plotItems.IndexOf(pivotitem);
      if (pivotidx < 0)
        return;

      // Distribute the changes backward to the first item
      PrepareStylesIterativeBackward(pivotidx, this.ParentLayer);
      PlotItemCollection rootCollection = ApplyStylesIterativeBackward(pivotidx);

      // now prepare and apply the styles forward normally beginning from the root collection
      // we can set the parent styles to null since rootCollection is the lowest collection that don't inherit from a lower group.
      rootCollection.PrepareStylesForward_HierarchyUpOnly(null, this.ParentLayer);
      rootCollection.ApplyStylesForward_HierarchyUpOnly(null);
    }

    /// <summary>
    /// Prepare styles, beginning at item 'pivotidx' in this collection, iterative backwards up and down the hierarchy.
    /// It stops at the first item of a collection here or down the hierarchy that do not inherit from it's parent collection.
    /// </summary>
    /// <param name="pivotidx">The index of the item where the application process starts.</param>
    /// <param name="layer">The plot layer.</param>
    protected void PrepareStylesIterativeBackward(int pivotidx, IPlotArea layer)
    {
      // if the pivot is lower than 0, we first distibute all changes to the first item and
      // then from the first item again down the line
      if (pivotidx > 0)
      {
        _styles.BeginPrepare();
        for (int i = pivotidx; i >= 0; i--)
        {
          IGPlotItem pi = _plotItems[i];
          if (pi is PlotItemCollection)
          {
            _styles.PrepareStep();
            PlotItemCollection pic = (PlotItemCollection)pi;
            pic.PrepareStylesBackward_HierarchyUpOnly(_styles,layer);
          }
          else
          {
            pi.PrepareStyles(_styles,layer);
            if (i > 0) _styles.PrepareStep();
          }
        }
        _styles.EndPrepare();
      }


      // now use this styles to copy to the parent
      bool transferToParentStyles =
      ParentCollection != null &&
      ParentCollection._styles.Count != 0 &&
      ParentCollection._styles.DistributeToChildGroups &&
      this._styles.InheritFromParentGroups;

      if (transferToParentStyles)
      {
        PlotGroupStyleCollection.TransferFromTo(_styles, ParentCollection._styles);
        ParentCollection.ApplyStylesIterativeBackward(ParentCollection._styles.Count - 1);
      }
    }


    /// <summary>
    /// Apply styles, beginning at item 'pivotidx' in this collection, iterative backwards up and down the hierarchy.
    /// It stops at the first item of a collection here or down the hierarchy that do not inherit from it's parent collection.
    /// </summary>
    /// <param name="pivotidx">The index of the item where the application process starts.</param>
    /// <returns>The plot item collection where the process stops.</returns>
    protected PlotItemCollection ApplyStylesIterativeBackward(int pivotidx)
    {
      // if the pivot is lower than 0, we first distibute all changes to the first item and
      // then from the first item again down the line
      if (pivotidx > 0)
      {
        _styles.BeginApply();
        for (int i = pivotidx; i >= 0; i--)
        {
          IGPlotItem pi = _plotItems[i];
          if (pi is PlotItemCollection)
          {
            _styles.Step(-1);
            PlotItemCollection pic = (PlotItemCollection)pi;
            pic.ApplyStylesBackward_HierarchyUpOnly(_styles);
          }
          else
          {
            pi.ApplyStyles(_styles);
             if(i>0) _styles.Step(-1);
          }
        }
        _styles.EndApply();
      }
     

      // now use this styles to copy to the parent
      bool transferToParentStyles =
      ParentCollection != null &&
      ParentCollection._styles.Count!=0 &&
      ParentCollection._styles.DistributeToChildGroups &&
      this._styles.InheritFromParentGroups;

      PlotItemCollection rootCollection = this;
      if (transferToParentStyles)
      {
        PlotGroupStyleCollection.TransferFromTo(_styles, ParentCollection._styles);
        rootCollection = ParentCollection.ApplyStylesIterativeBackward(ParentCollection._styles.Count - 1);
      }

      return rootCollection;
    }

    /// <summary>
    /// Apply styles backward from the last item to the first, but only upwards in the hierarchy.
    /// </summary>
    /// <param name="styles"></param>
    /// <param name="layer">The plot layer.</param>
    protected void PrepareStylesBackward_HierarchyUpOnly(PlotGroupStyleCollection styles, IPlotArea layer)
    {
      bool transferToLocalStyles =
        styles != null &&
        styles.Count != 0 &&
        styles.DistributeToChildGroups &&
        this._styles.InheritFromParentGroups;

      if (!transferToLocalStyles)
        return;

      PlotGroupStyleCollection.TransferFromTo(styles, _styles);

      _styles.BeginApply();
      // now distibute the styles from the first item down to the last item
      int last = _plotItems.Count - 1;
      for (int i = last; i >= 0; i--)
      {
        IGPlotItem pi = _plotItems[i];
        if (pi is PlotItemCollection)
        {
          _styles.PrepareStep();
          ((PlotItemCollection)pi).PrepareStylesBackward_HierarchyUpOnly(_styles,layer);
        }
        else
        {
          pi.PrepareStyles(_styles,layer);
          _styles.PrepareStep();
        }
      }
      _styles.EndPrepare();

      PlotGroupStyleCollection.TransferFromToIfBothSteppingEnabled(_styles, styles);
    }

    /// <summary>
    /// Apply styles backward from the last item to the first, but only upwards in the hierarchy.
    /// </summary>
    /// <param name="styles"></param>
    protected void ApplyStylesBackward_HierarchyUpOnly(PlotGroupStyleCollection styles)
    {
      bool transferToLocalStyles =
        styles != null &&
        styles.Count != 0 &&
        styles.DistributeToChildGroups &&
        this._styles.InheritFromParentGroups;

      if (!transferToLocalStyles)
        return;

      PlotGroupStyleCollection.TransferFromTo(styles, _styles);

      _styles.BeginApply();
      // now distibute the styles from the first item down to the last item
      int last = _plotItems.Count - 1;
      for (int i = last; i >= 0; i--)
      {
        IGPlotItem pi = _plotItems[i];
        if (pi is PlotItemCollection)
        {
          _styles.Step(-1);
          ((PlotItemCollection)pi).ApplyStylesBackward_HierarchyUpOnly(_styles);
        }
        else
        {
          pi.ApplyStyles(_styles);
          _styles.Step(-1);
        }
      }
      _styles.EndApply();

      PlotGroupStyleCollection.TransferFromToIfBothSteppingEnabled(_styles, styles);

    }

    /// <summary>
    /// Prepare styles forward, but only up in the hierarchy.
    /// </summary>
    /// <param name="parentstyles">The parent group style collection.</param>
    /// <param name="layer">The plot layer.</param>
    protected void PrepareStylesForward_HierarchyUpOnly(PlotGroupStyleCollection parentstyles, IPlotArea layer)
    {
      bool transferFromParentStyles =
       parentstyles != null &&
       parentstyles.Count != 0 &&
       parentstyles.DistributeToChildGroups &&
       this._styles.InheritFromParentGroups;

      _styles.BeginPrepare();

      string thisname = Main.DocumentPath.GetPathString(this, int.MaxValue);
      System.Diagnostics.Debug.WriteLine(string.Format("{0}:Begin:PrepareFWHUO",thisname));
      if (transferFromParentStyles)
      {
        PlotGroupStyleCollection.TransferFromTo(parentstyles, _styles);
        System.Diagnostics.Debug.WriteLine(string.Format("{0}:Begin:PrepareFWHUO (transfer from parent style", thisname));
      }


      // now distibute the styles from the first item down to the last item
      int last = _plotItems.Count - 1;
      for (int i = 0; i <= last; i++)
      {
        IGPlotItem pi = _plotItems[i];
        if (pi is PlotItemCollection)
        {
          PlotItemCollection pic = (PlotItemCollection)pi;
          pic.PrepareStylesForward_HierarchyUpOnly(_styles,layer);
          _styles.PrepareStepIfForeignSteppingFalse(((PlotItemCollection)pi)._styles);
        }
        else
        {
          pi.PrepareStyles(_styles,layer);
          _styles.PrepareStep();
        }
      }

      if (transferFromParentStyles)
      {
        PlotGroupStyleCollection.TransferFromTo(_styles, parentstyles);
        System.Diagnostics.Debug.WriteLine(string.Format("{0}:End:PrepareFWHUO (transfer back to parent style", thisname));
      }

      _styles.EndPrepare();
      System.Diagnostics.Debug.WriteLine(string.Format("{0}:End:PrepareFWHUO", thisname));
    }

    /// <summary>
    /// Apply styles forward, but only up in the hierarchy.
    /// </summary>
    /// <param name="parentstyles">The parent group style collection.</param>
    protected void ApplyStylesForward_HierarchyUpOnly(PlotGroupStyleCollection parentstyles)
    {
      bool transferFromParentStyles =
       parentstyles != null &&
       parentstyles.Count != 0 &&
       parentstyles.DistributeToChildGroups &&
       this._styles.InheritFromParentGroups;

      if (transferFromParentStyles)
        PlotGroupStyleCollection.TransferFromTo(parentstyles, _styles);

      _styles.BeginApply();

      // now distibute the styles from the first item down to the last item
      int last = _plotItems.Count - 1;
      for(int i=0;i<=last;i++)
      {
        IGPlotItem pi = _plotItems[i];
        if (pi is PlotItemCollection)
        {
          PlotItemCollection pic = (PlotItemCollection)pi;
          pic.ApplyStylesForward_HierarchyUpOnly(_styles);
          _styles.StepIfForeignSteppingFalse(1,((PlotItemCollection)pi)._styles);
        }
        else
        {
          pi.ApplyStyles(_styles);
          _styles.Step(1);
        }
      }
      _styles.EndApply();

      if (transferFromParentStyles)
      {
        PlotGroupStyleCollection.TransferFromToIfBothSteppingEnabled(_styles, parentstyles);
        parentstyles.SetAllToApplied(); // to indicate that we have applied this styles and so to enable stepping
      }
    }

  

    /// <summary>
    /// Does nothing because a plot item collection doesn't distibute item styles from members of the outer group into it's own members.
    /// </summary>
    /// <param name="template">Ignored.</param>
    /// <param name="strictness">Ignored.</param>
    public void SetPlotStyleFromTemplate(IGPlotItem template, PlotGroupStrictness strictness)
    {
    }

    /// <summary>
    /// Sets the plot style (or sub plot styles) of all plot items in this collection according to a template provided by the plot item in the template argument.
    /// </summary>
    /// <param name="template">The template item to copy the plot styles from.</param>
    /// <param name="strictness">Denotes the strictness the styles are copied from the template. See <see cref="PlotGroupStrictness" /> for more information.</param>
    public void DistributePlotStyleFromTemplate(IGPlotItem template, PlotGroupStrictness strictness)
    {
      foreach(IGPlotItem pi in this._plotItems)
      {
        if (!object.ReferenceEquals(pi, template))
          pi.SetPlotStyleFromTemplate(template, strictness);
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

  

    public void Paint(System.Drawing.Graphics g, IPlotArea layer)
    {
      ICoordinateTransformingGroupStyle coordTransStyle;
      if (null != (coordTransStyle = _styles.CoordinateTransformingStyle))
      {
        coordTransStyle.Paint(g, layer, this);
      }
      else
      {
        for (int i = _plotItems.Count - 1; i >= 0; i--)
        {
          _plotItems[i].Paint(g, layer);
        }
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




    public virtual void EhChildChanged(object child, EventArgs e)
    {
      OnChanged();
    }

    protected virtual void OnChanged()
    {
      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

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
      ICoordinateTransformingGroupStyle coordTransStyle;
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
      ICoordinateTransformingGroupStyle coordTransStyle;
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
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
      class XmlSerializationSurrogate1 : XmlSerializationSurrogate0
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
