﻿#region Copyright

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

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Altaxo.Collections;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;

namespace Altaxo.Graph.Gdi.Plot
{
  using System.Collections.Specialized;
  using System.Linq;
  using Geometry;
  using Graph.Plot.Groups;
  using Plot.Groups;

  [Serializable]
  public class PlotItemCollection
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IGPlotItem,
    IEnumerable<IGPlotItem>,
    IXBoundsHolder,
    IYBoundsHolder
  {
    /// <summary>Local collection of plot group styles of this plot item collection.</summary>
    private PlotGroupStyleCollection _plotGroupStyles;

    /// <summary>Collection of plot items.</summary>
    private ObservableList<IGPlotItem> _plotItems;

    [NonSerialized]
    private IGPlotItem[]? _cachedPlotItemsFlattened;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PlotItemCollection", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

      private struct PGTrans
      {
        public PlotGroupMemento PlotGroup;
        public PlotItemCollection PlotItemCollection;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PlotItemCollection?)o ?? new PlotItemCollection(info);

        int count = info.OpenArray();
        var plotItems = new IGPlotItem[count];
        for (int i = 0; i < count; i++)
        {
          plotItems[i] = (IGPlotItem)info.GetValue("PlotItem", s);
        }
        info.CloseArray(count);

        count = info.OpenArray(); // PlotGroups
        var plotGroups = new PGTrans[count];
        for (int i = 0; i < count; i++)
        {
          plotGroups[i].PlotGroup = (PlotGroupMemento)info.GetValue("e", s);
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
            if (plotGroups[foundidx].PlotItemCollection is null)
            {
              var newColl = new PlotItemCollection();
              plotGroups[foundidx].PlotItemCollection = newColl;
              s.Add(plotGroups[foundidx].PlotItemCollection);
              // now set the properties of this new collection
              bool serial = !plotGroups[foundidx].PlotGroup._concurrently;
              IPlotGroupStyle? curr = null;
              IPlotGroupStyle? prev = null;
              if (0 != (plotGroups[foundidx].PlotGroup._plotGroupStyle & Version0PlotGroupStyle.Color))
              {
                curr = ColorGroupStyle.NewExternalGroupStyle();
                newColl.GroupStyles.Add(curr);
              }
              if (0 != (plotGroups[foundidx].PlotGroup._plotGroupStyle & Version0PlotGroupStyle.Line))
              {
                prev = curr;
                curr = new DashPatternGroupStyle();
                newColl.GroupStyles.Add(curr, serial ? (prev is null ? null : prev.GetType()) : null);
              }
              if (0 != (plotGroups[foundidx].PlotGroup._plotGroupStyle & Version0PlotGroupStyle.Symbol))
              {
                prev = curr;
                curr = new ScatterSymbolGroupStyle();
                newColl.GroupStyles.Add(curr, serial ? (prev is null ? null : prev.GetType()) : null);
              }
            }
            // now add the item to this collection
            plotGroups[foundidx].PlotItemCollection.Add(plotItems[pix]);
          }
        }

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PlotItemCollection", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.PlotItemCollection", 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
                PlotItemCollection s = (PlotItemCollection)obj;

                info.CreateArray("PlotItems", s.Count);
                for (int i = 0; i < s.Count; i++)
                    info.AddValue("PlotItem", s[i]);
                info.CommitArray();

                info.AddValue("GroupStyles", s._plotGroupStyles);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PlotItemCollection?)o ?? new PlotItemCollection(info);

        int count = info.OpenArray();
        var plotItems = new IGPlotItem[count];
        for (int i = 0; i < count; i++)
        {
          s.Add((IGPlotItem)info.GetValue("PlotItem", s));
        }
        info.CloseArray(count);

        s.ChildSetMember(ref s._plotGroupStyles, (PlotGroupStyleCollection)info.GetValue("GroupStyles", s));

        return s;
      }
    }

    /// <summary>
    /// 2016-11-18 Now the group styles are serialized before the plot items. Because the group styles save
    /// the style sets anyway, we spare saving the style sets in both the first plot item and the group style.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotItemCollection), 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PlotItemCollection)obj;

        info.AddValue("GroupStyles", s._plotGroupStyles);

        info.CreateArray("PlotItems", s.Count);
        for (int i = 0; i < s.Count; i++)
          info.AddValue("PlotItem", s[i]);
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PlotItemCollection?)o ?? new PlotItemCollection(info);

        s._plotGroupStyles = (PlotGroupStyleCollection)info.GetValue("GroupStyles", s);
        if (s._plotGroupStyles is not null)
          s._plotGroupStyles.ParentObject = s;

        int count = info.OpenArray();
        var plotItems = new IGPlotItem[count];
        for (int i = 0; i < count; i++)
        {
          s.Add((IGPlotItem)info.GetValue("PlotItem", s));
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
      foreach (var item in this)
        item.ParentObject = this;
    }

    public PlotItemCollection(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _plotItems = new ObservableList<IGPlotItem>();
      _plotItems.CollectionChanged += EhPlotItemsCollectionChanged;
      _plotGroupStyles = new PlotGroupStyleCollection() { ParentObject = this };
    }

    #endregion Serialization

    #region Constructors

    public PlotItemCollection(XYPlotLayer owner)
    {
      _parent = owner;
      _plotItems = new ObservableList<IGPlotItem>();
      _plotItems.CollectionChanged += EhPlotItemsCollectionChanged;
      _plotGroupStyles = new PlotGroupStyleCollection() { ParentObject = this };
    }

    public PlotItemCollection()
    {
      _plotItems = new ObservableList<IGPlotItem>();
      _plotItems.CollectionChanged += EhPlotItemsCollectionChanged;
      _plotGroupStyles = new PlotGroupStyleCollection() { ParentObject = this };
    }

    /// <summary>
    /// Copy constructor. Clones (!) all items. The parent owner is set to null and has to be set afterwards.
    /// </summary>
    /// <param name="from">The PlotItemCollection to clone this list from.</param>
    public PlotItemCollection(PlotItemCollection from)
      :
      this(null, from)
    {
    }

    /// <summary>
    /// Copy constructor. Clones (!) all the items in the list.
    /// </summary>
    /// <param name="owner">The new owner of the cloned list.</param>
    /// <param name="from">The list to clone all items from.</param>
    public PlotItemCollection(XYPlotLayer? owner, PlotItemCollection from)
    {
      _parent = owner;
      _plotGroupStyles = new PlotGroupStyleCollection() { ParentObject = this };
      _plotItems = new ObservableList<IGPlotItem>(from.Select(x => { var result = (IGPlotItem)x.Clone(); result.ParentObject = this; return result; })); // Clone all the items in the list.
      _plotItems.CollectionChanged += EhPlotItemsCollectionChanged;

      // special way neccessary to handle plot groups
      ChildCopyToMember(ref _plotGroupStyles, from._plotGroupStyles);
    }

    public void CopyFrom(PlotItemCollection from, GraphCopyOptions options)
    {
      if (ReferenceEquals(this, from))
        return;

      if (GraphCopyOptions.CopyLayerPlotStyles == (GraphCopyOptions.CopyLayerPlotStyles & options))
      {
        var thisFlat = Flattened;
        var fromFlat = from.Flattened;
        int len = Math.Min(thisFlat.Length, fromFlat.Length);
        for (int i = 0; i < len; i++)
        {
          thisFlat[i].SetPlotStyleFromTemplate(fromFlat[i], PlotGroupStrictness.Strict);
        }
      }
    }

    public virtual bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var from = obj as PlotItemCollection;
      if (from is not null)
      {
        CopyFrom(from, GraphCopyOptions.All);
      }
      return false;
    }

    object ICloneable.Clone()
    {
      return new PlotItemCollection(this);
    }

    public PlotItemCollection Clone()
    {
      return new PlotItemCollection(this);
    }

    #endregion Constructors

    #region Other stuff

    public XYPlotLayer ParentLayer
    {
      get
      {
        return Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(this) ?? throw new NotImplementedException();
      }
    }

    public PlotItemCollection? ParentCollection
    {
      get
      {
        return _parent as PlotItemCollection;
      }
    }

    IGPlotItem? INodeWithParentNode<IGPlotItem>.ParentNode
    {
      get
      {
        return _parent as PlotItemCollection;
      }
    }

    IList<IGPlotItem> ITreeListNode<IGPlotItem>.ChildNodes
    {
      get
      {
        return _plotItems;
      }
    }

    IEnumerable<IGPlotItem> ITreeNode<IGPlotItem>.ChildNodes
    {
      get
      {
        return _plotItems;
      }
    }

    public PlotGroupStyleCollection GroupStyles
    {
      get
      {
        return _plotGroupStyles;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException();

        ChildSetMember(ref _plotGroupStyles, value);
      }
    }

    #endregion Other stuff

    #region IG2DPlotItem Members

    /// <summary>
    /// Returns null, because a plot item collection does not have a data object for itself.
    /// </summary>
    public IDocumentLeafNode? DataObject { get { return null; } }

    /// <summary>
    /// Returns null, because this plot item collection does not have a style object for itself.
    /// </summary>
    public IDocumentLeafNode? StyleObject { get { return null; } }

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

    public void PrepareGroupStyles(PlotGroupStyleCollection? parentPlotGroupStyles, IPlotArea layer)
    {
      PrepareGroupStylesForward_HierarchyUpOnly(parentPlotGroupStyles, layer);
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection? parentPlotGroupStyles)
    {
      ApplyGroupStylesForward_HierarchyUpOnly(parentPlotGroupStyles);
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
      PrepareStylesIterativeBackward(pivotidx, ParentLayer);
      PlotItemCollection rootCollection = ApplyStylesIterativeBackward(pivotidx);

      // now prepare and apply the styles forward normally beginning from the root collection
      // we can set the parent styles to null since rootCollection is the lowest collection that don't inherit from a lower group.
      rootCollection.PrepareGroupStylesForward_HierarchyUpOnly(null, ParentLayer);
      rootCollection.ApplyGroupStylesForward_HierarchyUpOnly(null);
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
        _plotGroupStyles.BeginPrepare();
        for (int i = pivotidx; i >= 0; i--)
        {
          IGPlotItem pi = _plotItems[i];
          if (pi is PlotItemCollection)
          {
            _plotGroupStyles.PrepareStep();
            var pic = (PlotItemCollection)pi;
            pic.PrepareStylesBackward_HierarchyUpOnly(_plotGroupStyles, layer);
          }
          else
          {
            pi.PrepareGroupStyles(_plotGroupStyles, layer);
            if (i > 0)
              _plotGroupStyles.PrepareStep();
          }
        }
        _plotGroupStyles.EndPrepare();
      }

      // now use this styles to copy to the parent
      if ( // transferToParentStyles
        ParentCollection is not null &&
      ParentCollection._plotGroupStyles.Count != 0 &&
      ParentCollection._plotGroupStyles.DistributeToChildGroups &&
      _plotGroupStyles.InheritFromParentGroups
      )
      {
        PlotGroupStyleCollection.TransferFromTo(_plotGroupStyles, ParentCollection._plotGroupStyles);
        ParentCollection.ApplyStylesIterativeBackward(ParentCollection._plotGroupStyles.Count - 1);
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
        _plotGroupStyles.BeginApply();
        for (int i = pivotidx; i >= 0; i--)
        {
          IGPlotItem pi = _plotItems[i];
          if (pi is PlotItemCollection)
          {
            _plotGroupStyles.Step(-1);
            var pic = (PlotItemCollection)pi;
            pic.ApplyStylesBackward_HierarchyUpOnly(_plotGroupStyles);
          }
          else
          {
            pi.ApplyGroupStyles(_plotGroupStyles);
            if (i > 0)
              _plotGroupStyles.Step(-1);
          }
        }
        _plotGroupStyles.EndApply();
      }

      // now use this styles to copy to the parent
      PlotItemCollection rootCollection = this;

      if ( // transferToParentStyles
        ParentCollection is not null &&
        ParentCollection._plotGroupStyles.Count != 0 &&
        ParentCollection._plotGroupStyles.DistributeToChildGroups &&
        _plotGroupStyles.InheritFromParentGroups
        )
      {
        PlotGroupStyleCollection.TransferFromTo(_plotGroupStyles, ParentCollection._plotGroupStyles);
        rootCollection = ParentCollection.ApplyStylesIterativeBackward(ParentCollection._plotGroupStyles.Count - 1);
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
      if ( // transferToLocalStyles
                styles is not null &&
        styles.Count != 0 &&
        styles.DistributeToChildGroups &&
        _plotGroupStyles.InheritFromParentGroups
        )
      {
        PlotGroupStyleCollection.TransferFromTo(styles, _plotGroupStyles);

        _plotGroupStyles.BeginApply();
        // now distibute the styles from the first item down to the last item
        int last = _plotItems.Count - 1;
        for (int i = last; i >= 0; i--)
        {
          IGPlotItem pi = _plotItems[i];
          if (pi is PlotItemCollection)
          {
            _plotGroupStyles.PrepareStep();
            ((PlotItemCollection)pi).PrepareStylesBackward_HierarchyUpOnly(_plotGroupStyles, layer);
          }
          else
          {
            pi.PrepareGroupStyles(_plotGroupStyles, layer);
            _plotGroupStyles.PrepareStep();
          }
        }
        _plotGroupStyles.EndPrepare();

        PlotGroupStyleCollection.TransferFromToIfBothSteppingEnabled(_plotGroupStyles, styles);
      }
    }

    /// <summary>
    /// Apply styles backward from the last item to the first, but only upwards in the hierarchy.
    /// </summary>
    /// <param name="styles"></param>
    protected void ApplyStylesBackward_HierarchyUpOnly(PlotGroupStyleCollection styles)
    {
      if ( // transferToLocalStyles
        styles is not null &&
        styles.Count != 0 &&
        styles.DistributeToChildGroups &&
        _plotGroupStyles.InheritFromParentGroups
        )
      {
        PlotGroupStyleCollection.TransferFromTo(styles, _plotGroupStyles);

        _plotGroupStyles.BeginApply();
        // now distibute the styles from the first item down to the last item
        int last = _plotItems.Count - 1;
        for (int i = last; i >= 0; i--)
        {
          IGPlotItem pi = _plotItems[i];
          if (pi is PlotItemCollection)
          {
            _plotGroupStyles.Step(-1);
            ((PlotItemCollection)pi).ApplyStylesBackward_HierarchyUpOnly(_plotGroupStyles);
          }
          else
          {
            pi.ApplyGroupStyles(_plotGroupStyles);
            _plotGroupStyles.Step(-1);
          }
        }
        _plotGroupStyles.EndApply();

        PlotGroupStyleCollection.TransferFromToIfBothSteppingEnabled(_plotGroupStyles, styles);
      }
    }

    /// <summary>
    /// Prepare group styles forward, from the first to the last item.
    /// The function is called recursively for child PlotItemCollections, but only up in the hierarchy.
    /// This function therefore has no influence on items down in hierarchie, i.e. parental PlotItemCollections.
    /// </summary>
    /// <param name="parentGroupStyles">The parent plot group style collection.</param>
    /// <param name="layer">The plot layer.</param>
    /// <remarks>The preparation is used for:
    /// <para>BarGraph: to count items, calculating the width and position of each item afterwards.</para>
    /// <para>It is <b>not</b> used to enumerate colors, line styles etc., since this is done during the Apply stage.</para>
    /// </remarks>
    protected void PrepareGroupStylesForward_HierarchyUpOnly(PlotGroupStyleCollection? parentGroupStyles, IPlotArea layer)
    {
      bool transferFromParentStyles =
       parentGroupStyles is not null &&
       parentGroupStyles.Count != 0 &&
       parentGroupStyles.DistributeToChildGroups &&
       _plotGroupStyles.InheritFromParentGroups;

      // Announce the local plot group styles, that we start preparing
      _plotGroupStyles.BeginPrepare();

      //string thisname = Main.DocumentPath.GetPathString(this, int.MaxValue);
      //System.Diagnostics.Debug.WriteLine(string.Format("{0}:Begin:PrepareFWHUO", thisname));

      // if TransferFromParentStyles was choosen, transfer some of the plot group settings of the parental plot group styles to the local styles
      if (transferFromParentStyles)
      {
        PlotGroupStyleCollection.TransferFromTo(parentGroupStyles!, _plotGroupStyles);
        //System.Diagnostics.Debug.WriteLine(string.Format("{0}:Begin:PrepareFWHUO (transfer from parent style", thisname));
      }

      // for each PlotItem in this collection, announce the preparation, using the local plot group style collection
      // after each item, announce a step to the plot group styles, so that the properties (like color etc.) can be stepped forward
      int last = _plotItems.Count - 1;
      for (int i = 0; i <= last; i++)
      {
        IGPlotItem pi = _plotItems[i];
        if (pi is PlotItemCollection)
        {
          var pic = (PlotItemCollection)pi;
          pic.PrepareGroupStylesForward_HierarchyUpOnly(_plotGroupStyles, layer);
          _plotGroupStyles.PrepareStepIfForeignSteppingFalse(((PlotItemCollection)pi)._plotGroupStyles);
        }
        else
        {
          pi.PrepareGroupStyles(_plotGroupStyles, layer);
          _plotGroupStyles.PrepareStep();
        }
      }

      // after all our own PlotItems are prepared now,
      // if TransferFromParentStyles was choosen, transfer our own plot group settings back to the parental plot group styles
      // so that the parental plot group can continue i.e. with the color etc.
      if (transferFromParentStyles)
      {
        PlotGroupStyleCollection.TransferFromTo(_plotGroupStyles, parentGroupStyles!);
        //System.Diagnostics.Debug.WriteLine(string.Format("{0}:End:PrepareFWHUO (transfer back to parent style", thisname));
      }

      // after preparation of all plot items is done, announce the end of preparation,
      // some of the calculations can be done only now.
      _plotGroupStyles.EndPrepare();
      //System.Diagnostics.Debug.WriteLine(string.Format("{0}:End:PrepareFWHUO", thisname));
    }

    /// <summary>
    /// Apply plot group styles forward, from the first to the last item.
    /// The function is called recursively for child PlotItemCollections, but only up in the hierarchy.
    /// This function therefore has no influence on items down in hierarchie, i.e. parental PlotItemCollections.
    /// </summary>
    /// <param name="parentGroupStyles">The parent plot group style collection.</param>
    /// <remarks>The application is used for example:
    /// <para>BarGraph: to calculate the exact position of each plot item.</para>
    /// <para>Color: To step forward through the available colors and apply each color to another PlotItem.</para>
    /// </remarks>

    protected void ApplyGroupStylesForward_HierarchyUpOnly(PlotGroupStyleCollection? parentGroupStyles)
    {
      bool transferFromParentStyles =
       parentGroupStyles is not null &&
       parentGroupStyles.Count != 0 &&
       parentGroupStyles.DistributeToChildGroups &&
       _plotGroupStyles.InheritFromParentGroups;

      // if TransferFromParentStyles was choosen, transfer some of the plot group settings of the parental plot group styles to the local styles
      if (transferFromParentStyles)
      {
        PlotGroupStyleCollection.TransferFromTo(parentGroupStyles!, _plotGroupStyles);
      }

      // Announce the local plot group styles the begin of the application stage
      _plotGroupStyles.BeginApply();

      // for each PlotItem in this collection, announce the application, using the local plot group style collection
      // after each item, announce an application step (of stepwidth 1) to the plot group styles, so that the properties (like color etc.) can be stepped forward
      int last = _plotItems.Count - 1;
      for (int i = 0; i <= last; i++)
      {
        IGPlotItem pi = _plotItems[i];
        if (pi is PlotItemCollection)
        {
          var pic = (PlotItemCollection)pi;
          pic.ApplyGroupStylesForward_HierarchyUpOnly(_plotGroupStyles);
          _plotGroupStyles.StepIfForeignSteppingFalse(1, ((PlotItemCollection)pi)._plotGroupStyles);
        }
        else
        {
          pi.ApplyGroupStyles(_plotGroupStyles);
          _plotGroupStyles.Step(1);
        }
      }
      // after application of PlotGroupStyles to all the plot items is done, announce the end of application,
      _plotGroupStyles.EndApply();

      if (transferFromParentStyles)
      {
        PlotGroupStyleCollection.TransferFromToIfBothSteppingEnabled(_plotGroupStyles, parentGroupStyles!);
        parentGroupStyles!.SetAllToApplied(); // to indicate that we have applied this styles and so to enable stepping
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
      foreach (IGPlotItem pi in _plotItems)
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
      return string.Format("<Collection of {0} plot items>", _plotItems.Count);
    }

    public string GetName(string style)
    {
      return string.Format("<Collection of {0} plot items>", _plotItems.Count);
    }

    public IndexDirection ChildIndexDirection
    {
      get
      {
        if (_plotGroupStyles.CoordinateTransformingStyle is not null)
          return IndexDirection.Descending;
        else
          return IndexDirection.Ascending;
      }
    }

    public virtual void PaintPreprocessing(IPaintContext paintContext)
    {
    }

    public void Paint(System.Drawing.Graphics g, IPaintContext paintContext, IPlotArea layer, IGPlotItem? previousPlotItem, IGPlotItem? nextPlotItem)
    {
      //var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<IPlotArea>(this);
      if (_plotGroupStyles.CoordinateTransformingStyle is { } coordTransStyle)
      {
        coordTransStyle.PaintPreprocessing(g, paintContext, layer, this);
      }
      else
      {
        // no further preparations neccessary here
      }
    }

    public void PaintChild(System.Drawing.Graphics g, IPaintContext paintContext, IPlotArea layer, int indexOfChild)
    {
      if (_plotGroupStyles.CoordinateTransformingStyle is { } coordTransStyle)
      {
        coordTransStyle.PaintChild(g, paintContext, layer, this, indexOfChild);
      }
      else
      {
        int hi = _plotItems.Count - 1;

        _plotItems[indexOfChild].Paint(g,
          paintContext,
          layer,
          indexOfChild > 0 ? _plotItems[indexOfChild - 1] : null,
          indexOfChild < hi ? _plotItems[indexOfChild + 1] : null);
      }
    }

    /// <summary>
    /// Called after painting has finished. Can be used to release resources.
    /// </summary>
    public void PaintPostprocessing()
    {
    }

    #region Hit test

    public IHitTestObject? HitTest(IPlotArea layer, PointD2D hitpoint)
    {
      IHitTestObject? result = null;
      foreach (IGPlotItem pi in _plotItems)
      {
        result = pi.HitTest(layer, hitpoint);
        if (result is not null)
        {
          if (result.Remove is null)
            result.Remove = new DoubleClickHandler(EhHitTestObject_Remove);

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
    public bool EhHitTestObject_Remove(IHitTestObject target)
    {
      return target.HittedObject is IGPlotItem pi ? Remove(pi) : false;
    }

    #endregion Hit test

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public virtual void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      foreach (var item in this)
        item.VisitDocumentReferences(Report);
    }

    #endregion IG2DPlotItem Members

    #region IEnumerable<IG2DPlotItem> Members

    public IEnumerator<IGPlotItem> GetEnumerator()
    {
      return _plotItems.GetEnumerator();
    }

    #endregion IEnumerable<IG2DPlotItem> Members

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _plotItems.GetEnumerator();
    }

    #endregion IEnumerable Members

    #region Other collection methods

    public int Count
    {
      get { return _plotItems.Count; }
    }

    public void Add(IGPlotItem item)
    {
      if (item is null)
        throw new ArgumentNullException();

      item.ParentObject = this;
      _plotItems.Add(item);
    }

    /// <summary>
    /// Replaces one plot item, which must be part of this collection, by another plot item.
    /// </summary>
    /// <param name="oldItem">The old item.</param>
    /// <param name="newItem">The new item.</param>
    /// <exception cref="ArgumentNullException">
    /// oldItem
    /// or
    /// newItem
    /// </exception>
    /// <exception cref="ArgumentException">OldItem is not member of the collection - oldItem</exception>
    public void Replace(IGPlotItem oldItem, IGPlotItem newItem)
    {
      if (oldItem is null)
        throw new ArgumentNullException(nameof(oldItem));
      if (newItem is null)
        throw new ArgumentNullException(nameof(newItem));

      var idx = _plotItems.IndexOf(oldItem);
      if (idx < 0)
        throw new ArgumentException("OldItem is not member of the collection", nameof(oldItem));

      var tempPlotItem = _plotItems[idx];
      var changed = ChildSetMember(ref tempPlotItem, newItem);
      _plotItems[idx] = tempPlotItem; // ObservableList ensures that Changed event is fired
    }

    public void AddRange(IEnumerable<IGPlotItem> items)
    {
      if (items is null)
        throw new ArgumentNullException();

      foreach (var item in items)
      {
        item.ParentObject = this;
        _plotItems.Add(item);
      }

      OnCollectionChanged();
    }

    public void ClearPlotItems()
    {
      _plotItems.Clear();
    }

    public void ClearPlotItemsAndGroupStyles()
    {
      _plotItems.Clear();
      _plotGroupStyles.Clear();
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
        plotitem = _plotItems[idx];
        _plotItems.RemoveAt(idx);
        plotitem.Dispose();
        return true;
      }
      return false;
    }

    public int IndexOf(IGPlotItem it)
    {
      return _plotItems.IndexOf(it);
    }

    /// <summary>
    /// This method must be called, if plot item members are added or removed to this collection.
    /// </summary>

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
        if (_cachedPlotItemsFlattened is null)
        {
          var list = new List<IGPlotItem>();
          FillPlotItemList(list);
          _cachedPlotItemsFlattened = list.ToArray();
        }
        return _cachedPlotItemsFlattened;
      }
    }

    #endregion Other collection methods

    #region NamedObjectCollection

    /// <summary>
    /// retrieves the object with the name <code>name</code>.
    /// </summary>
    /// <param name="name">The objects name.</param>
    /// <returns>The object with the specified name.</returns>
    public override Main.IDocumentLeafNode? GetChildObjectNamed(string name)
    {
      if (name == "PlotGroupStyles")
        return _plotGroupStyles;

      if (double.TryParse(name, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out var number))
      {
        int idx = (int)number;
        if (idx >= 0 && idx < Count)
          return this[idx];
      }
      return null;
    }

    /// <summary>
    /// Retrieves the name of the provided object.
    /// </summary>
    /// <param name="o">The object for which the name should be found.</param>
    /// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
    public override string? GetNameOfChildObject(Main.IDocumentLeafNode o)
    {
      if (object.ReferenceEquals(o, _plotGroupStyles))
        return "PlotGroupStyles";

      if (o is IGPlotItem pi)
      {
        int idx = _plotItems.IndexOf(pi);
        return idx >= 0 ? idx.ToString() : null;
      }
      return null;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      var index = 0;
      foreach (var item in _plotItems.ToArray())
      {
        yield return new Main.DocumentNodeAndName(item, index.ToString(System.Globalization.CultureInfo.CurrentCulture));
        ++index;
      }

      if (_plotGroupStyles is not null)
      {
        yield return new Main.DocumentNodeAndName(_plotGroupStyles, () => _plotGroupStyles = null!, "PlotGroupStyles");
      }
    }

    protected override void Dispose(bool isDisposing)
    {
      if (_plotItems is not null)
      {
        _plotItems.CollectionChanged -= EhPlotItemsCollectionChanged;
        var oldColl = _plotItems;
        _plotItems = new ObservableList<IGPlotItem>(); // Note: do not wire events here, the sole purpose of this new list is to avoid exceptions
        foreach (var item in oldColl)
          item.Dispose();
      }
      if (_plotGroupStyles is not null)
      {
        _plotGroupStyles.Dispose();
        _plotGroupStyles = null!;
      }

      base.Dispose(isDisposing);
    }

    #endregion NamedObjectCollection

    #region PlotGroup handling

    /// <summary>
    /// Add the PlotGroupStyle.
    /// </summary>
    /// <param name="pg">The plot group style to add.</param>
    public void Add(IPlotGroupStyle pg)
    {
      _plotGroupStyles.Add(pg);
      EhSelfChanged(EventArgs.Empty);
    }

    public void ListPossiblePlotGroupStyles()
    {
      foreach (IGPlotItem pi in _plotItems)
      {
      }
    }

    #endregion PlotGroup handling

    #region Event Handling

    public void EhPlotGroups_Changed(object sender, EventArgs e)
    {
      EhSelfChanged(EventArgs.Empty);
    }

    private void EhPlotItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      OnCollectionChanged();
    }

    protected virtual void OnCollectionChanged()
    {
      _cachedPlotItemsFlattened = null; // invalidate _cachedPlotItemsFlattened
      EhSelfChanged(new SimpleCollectionChangedEventArgs(this)); // notify items down in the hierarchy to invalidate _cachedPlotItemsFlattened
      EhSelfChanged(new BoundariesChangedEventArgs(BoundariesChangedData.ComplexChange)); // notify that possibly the boundaries have been changed
    }

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      var e1 = e as SimpleCollectionChangedEventArgs;
      if (e1 is not null)
      {
        // if in items above the collection has changed, the flattened plot items get invalid
        _cachedPlotItemsFlattened = null;
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Event Handling

    #region IXBoundsHolder Members

    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      if (_plotGroupStyles.CoordinateTransformingStyle is { } coordTransStyle)
        coordTransStyle.MergeXBoundsInto(ParentLayer, pb, this);
      else
        CoordinateTransformingStyleBase.MergeXBoundsInto(pb, this);
    }

    #endregion IXBoundsHolder Members

    #region IYBoundsHolder Members

    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      if (_plotGroupStyles.CoordinateTransformingStyle is { } coordTransStyle)
        coordTransStyle.MergeYBoundsInto(ParentLayer, pb, this);
      else
        CoordinateTransformingStyleBase.MergeYBoundsInto(pb, this);
    }

    #endregion IYBoundsHolder Members

    #region deprecated stuff for deserialisation

    private enum Version0PlotGroupStyle
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

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PlotGroupStyle", 0)]
    private class PlotGroupStyleTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("This is deprectated stuff");
        //info.SetNodeContent(obj.ToString());
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(Version0PlotGroupStyle), val, true);
      }
    }

    /// <summary>
    /// Deprectated stuff neccessary to deserialize PlotItemCollection Version 0.
    /// </summary>
    private class PlotGroupMemento
    {
      public Version0PlotGroupStyle _plotGroupStyle;
      public bool _concurrently;
      public PlotGroupStrictness _plotGroupStrictness;
      public int[] _plotItemIndices; // stores not the plotitems itself, only the position of the items in the list

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
      protected PlotGroupMemento(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
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

      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.PlotGroup+Memento", 0)]
      private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

        public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          PlotGroupMemento s = SDeserialize(o, info, parent);
          return s;
        }

        public virtual PlotGroupMemento SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (PlotGroupMemento?)o ?? new PlotGroupMemento(info);
          s._plotGroupStyle = (Version0PlotGroupStyle)info.GetValue("Style", s);

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
      private class XmlSerializationSurrogate1 : XmlSerializationSurrogate0
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

        public override PlotGroupMemento SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = base.SDeserialize(o, info, parent);

          s._concurrently = info.GetBoolean("Concurrently");
          s._plotGroupStrictness = (PlotGroupStrictness)info.GetEnum("Strict", typeof(PlotGroupStrictness));

          return s;
        }
      }
    }

    #endregion deprecated stuff for deserialisation
  }
}
