#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Plot;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;

namespace Altaxo.Graph.Graph3D.Plot
{
  using System.Collections;
  using System.Collections.Specialized;
  using Altaxo.Graph.Plot.Groups;
  using GraphicsContext;
  using Groups;

  /// <summary>
  /// Represents a hierarchical collection of three-dimensional plot items together with shared group styles.
  /// </summary>
  [Serializable]
  public class PlotItemCollection
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IGPlotItem,
    IEnumerable<IGPlotItem>,
    IXBoundsHolder,
    IYBoundsHolder,
    IZBoundsHolder
  {
    /// <summary>Local collection of plot group styles of this plot item collection.</summary>
    private PlotGroupStyleCollection _plotGroupStyles;

    /// <summary>Collection of plot items.</summary>
    private ObservableList<IGPlotItem> _plotItems;

    [NonSerialized]
    private IGPlotItem[]? _cachedPlotItemsFlattened;

    #region Serialization

    /// <summary>
    /// 2015-11-14 initial version.
    /// </summary>
    /// <summary>
    /// Serializes version 0 of <see cref="PlotItemCollection"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Graph3D.Plot.PlotItemCollection", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PlotItemCollection)o;

        info.CreateArray("PlotItems", s.Count);
        for (int i = 0; i < s.Count; i++)
          info.AddValue("PlotItem", s[i]);
        info.CommitArray();

        info.AddValue("GroupStyles", s._plotGroupStyles);
      }

      /// <inheritdoc/>
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

        s._plotGroupStyles = (PlotGroupStyleCollection)info.GetValue("GroupStyles", s);
        if (s._plotGroupStyles is not null)
          s._plotGroupStyles.ParentObject = s;

        return s;
      }
    }

    /// <summary>
    /// 2016-11-19 Now the group styles are serialized before the plot items. Because the group styles save
    /// the style sets anyway, we spare saving the style sets in both the first plot item and the group style.
    /// </summary>
    /// <summary>
    /// Serializes version 1 of <see cref="PlotItemCollection"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotItemCollection), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PlotItemCollection)o;

        info.AddValue("GroupStyles", s._plotGroupStyles);

        info.CreateArray("PlotItems", s.Count);
        for (int i = 0; i < s.Count; i++)
          info.AddValue("PlotItem", s[i]);
        info.CommitArray();
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PlotItemCollection?)o ?? new PlotItemCollection(info);

        s._plotGroupStyles = (PlotGroupStyleCollection)info.GetValue("GroupStyles", s);
        if (s._plotGroupStyles is not null)
          s._plotGroupStyles.ParentObject = s;

        int count = info.OpenArray();
        for (int i = 0; i < count; i++)
        {
          s.Add((IGPlotItem)info.GetValue("PlotItem", s));
        }
        info.CloseArray(count);

        return s;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotItemCollection"/> class during deserialization.
    /// </summary>
    /// <param name="info">The deserialization info.</param>
    protected PlotItemCollection(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _plotItems = new ObservableList<IGPlotItem>();
      _plotItems.CollectionChanged += EhPlotItemsCollectionChanged;
      _plotGroupStyles = new PlotGroupStyleCollection() { ParentObject = this };
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotItemCollection"/> class.
    /// </summary>
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
      this(null, from, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotItemCollection"/> class with the specified owner.
    /// </summary>
    /// <param name="owner">The owning plot layer.</param>
    public PlotItemCollection(XYZPlotLayer owner)
    {
      _parent = owner;
      _plotItems = new ObservableList<IGPlotItem>();
      _plotItems.CollectionChanged += EhPlotItemsCollectionChanged;
      _plotGroupStyles = new PlotGroupStyleCollection() { ParentObject = this };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotItemCollection"/> class.
    /// </summary>
    /// <param name="owner">The owner of this collection.</param>
    /// <param name="plotItems">The plot items that should initially belong to this collection.</param>
    /// <param name="clonePlotItems">If set to <c>true</c> the plot items are cloned before added to this collection. If false, the plot items are added directly to this collection.</param>
    public PlotItemCollection(XYZPlotLayer? owner, PlotItemCollection plotItems, bool clonePlotItems)
    {
      _parent = owner;
      _plotGroupStyles = new PlotGroupStyleCollection() { ParentObject = this };
      if (clonePlotItems)
        _plotItems = new ObservableList<IGPlotItem>(plotItems.Select(pi => { var result = (IGPlotItem)pi.Clone(); result.ParentObject = this; return result; }));
      else
        _plotItems = new ObservableList<IGPlotItem>(plotItems);
      _plotItems.CollectionChanged += EhPlotItemsCollectionChanged;

      // special way neccessary to handle plot groups
      ChildCopyToMember(ref _plotGroupStyles, plotItems._plotGroupStyles);
    }

    /// <summary>
    /// Gets the flattened list of plot items without nested collections.
    /// </summary>
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

    /// <summary>
    /// This method must be called, if plot item members are added or removed to this collection.
    /// </summary>
    /// <param name="list">The list to fill with flattened plot items.</param>
    protected void FillPlotItemList(IList<IGPlotItem> list)
    {
      foreach (IGPlotItem pi in _plotItems)
        if (pi is PlotItemCollection)
          ((PlotItemCollection)pi).FillPlotItemList(list);
        else
          list.Add(pi);
    }

    /// <summary>
    /// Gets the number of direct child plot items.
    /// </summary>
    public int Count
    {
      get { return _plotItems.Count; }
    }

    /// <summary>
    /// Removes all plot items from the collection.
    /// </summary>
    public void ClearPlotItems()
    {
      _plotItems.Clear();
    }

    /// <summary>
    /// Removes all plot items and all group styles from the collection.
    /// </summary>
    public void ClearPlotItemsAndGroupStyles()
    {
      _plotItems.Clear();
      _plotGroupStyles.Clear();
      OnCollectionChanged();
    }

    /// <summary>
    /// Gets the plot item at the specified index.
    /// </summary>
    /// <param name="i">The zero-based index.</param>
    public IGPlotItem this[int i]
    {
      get { return _plotItems[i]; }
    }

    /// <summary>
    /// Gets the child plot items as a list.
    /// </summary>
    public IList<IGPlotItem> ChildNodes
    {
      get
      {
        return _plotItems;
      }
    }

    /// <inheritdoc/>
    IEnumerable<IGPlotItem> ITreeNode<IGPlotItem>.ChildNodes
    {
      get
      {
        return _plotItems;
      }
    }

    /// <inheritdoc/>
    IGPlotItem? INodeWithParentNode<IGPlotItem>.ParentNode
    {
      get
      {
        return _parent as IGPlotItem;
      }
    }

    /// <inheritdoc/>
    public PlotItemCollection? ParentCollection
    {
      get
      {
        return _parent as PlotItemCollection;
      }
    }

    /// <summary>
    /// Gets the parent <see cref="XYZPlotLayer"/> that contains this collection.
    /// </summary>
    public XYZPlotLayer ParentLayer
    {
      get
      {
        return Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYZPlotLayer>(this) ?? throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns an enumerator for the contained plot items.
    /// </summary>
    /// <returns>An enumerator over the plot items.</returns>
    public IEnumerator<IGPlotItem> GetEnumerator()
    {
      return _plotItems.GetEnumerator();
    }

    /// <inheritdoc/>
    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      if (_plotGroupStyles.CoordinateTransformingStyle is { } coordTransStyle)
        coordTransStyle.MergeXBoundsInto(ParentLayer, pb, this);
      else
        CoordinateTransformingStyleBase.MergeXBoundsInto(pb, this);
    }

    /// <inheritdoc/>
    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      if (_plotGroupStyles.CoordinateTransformingStyle is { } coordTransStyle)
        coordTransStyle.MergeYBoundsInto(ParentLayer, pb, this);
      else
        CoordinateTransformingStyleBase.MergeYBoundsInto(pb, this);
    }

    /// <inheritdoc/>
    public void MergeZBoundsInto(IPhysicalBoundaries pb)
    {
      if (_plotGroupStyles.CoordinateTransformingStyle is { } coordTransStyle)
        coordTransStyle.MergeZBoundsInto(ParentLayer, pb, this);
      else
        CoordinateTransformingStyleBase.MergeZBoundsInto(pb, this);
    }

    /// <inheritdoc/>
    public void PaintSymbol(IGraphicsContext3D g, RectangleD3D location)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
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

    /// <summary>
    /// Disposes this collection and all contained items.
    /// </summary>
    /// <param name="isDisposing">Indicates whether managed resources should be disposed.</param>
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

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _plotItems.GetEnumerator();
    }

    /// <inheritdoc/>
    public void PrepareScales(Graph3D.IPlotArea layer)
    {
      foreach (IGPlotItem pi in _plotItems)
        pi.PrepareScales(layer);
    }

    /// <summary>
    /// Gets or sets the group styles associated with this collection.
    /// </summary>
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

    /// <inheritdoc/>
    public void PrepareGroupStyles(PlotGroupStyleCollection? styles, Graph3D.IPlotArea layer)
    {
      PrepareGroupStylesForward_HierarchyUpOnly(styles, layer);
    }

    /// <inheritdoc/>
    public void ApplyGroupStyles(PlotGroupStyleCollection? styles)
    {
      ApplyGroupStylesForward_HierarchyUpOnly(styles);
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
    protected void PrepareGroupStylesForward_HierarchyUpOnly(PlotGroupStyleCollection? parentGroupStyles, Graph3D.IPlotArea layer)
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
      if (
      ParentCollection is not null &&
      ParentCollection._plotGroupStyles.Count != 0 &&
      ParentCollection._plotGroupStyles.DistributeToChildGroups &&
      _plotGroupStyles.InheritFromParentGroups
      )
      {
        // Transfer to parent style?
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

      if (
      ParentCollection is not null &&
      ParentCollection._plotGroupStyles.Count != 0 &&
      ParentCollection._plotGroupStyles.DistributeToChildGroups &&
      _plotGroupStyles.InheritFromParentGroups
       )
      {
        // Transfer to parent style?
        PlotGroupStyleCollection.TransferFromTo(_plotGroupStyles, ParentCollection._plotGroupStyles);
        rootCollection = ParentCollection.ApplyStylesIterativeBackward(ParentCollection._plotGroupStyles.Count - 1);
      }

      return rootCollection;
    }

    /// <summary>
    /// Apply styles backward from the last item to the first, but only upwards in the hierarchy.
    /// </summary>
    /// <param name="styles">The styles to propagate from the parent hierarchy.</param>
    /// <param name="layer">The plot layer.</param>
    protected void PrepareStylesBackward_HierarchyUpOnly(PlotGroupStyleCollection styles, IPlotArea layer)
    {
      if ( // TransferToLocalStyles?
        styles is not null &&
        styles.Count != 0 &&
        styles.DistributeToChildGroups &&
        _plotGroupStyles.InheritFromParentGroups
      )
      {
        // if transfer to local styles ?
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
    /// <param name="styles">The styles to apply from the parent hierarchy.</param>
    protected void ApplyStylesBackward_HierarchyUpOnly(PlotGroupStyleCollection styles)
    {
      if ( //transferToLocalStyles
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
    /// Performs postprocessing for all contained plot items.
    /// </summary>
    public void PaintPostprocessing()
    {
      foreach (var pi in _plotItems)
        pi.PaintPostprocessing();
    }

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

    /// <summary>
    /// Copies plot-style information from another collection according to the specified options.
    /// </summary>
    /// <param name="from">The source collection.</param>
    /// <param name="options">The copy options.</param>
    public void CopyFrom(PlotItemCollection from, Gdi.GraphCopyOptions options)
    {
      if (ReferenceEquals(this, from))
        return;

      if (Gdi.GraphCopyOptions.CopyLayerPlotStyles == (Gdi.GraphCopyOptions.CopyLayerPlotStyles & options))
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

    /// <summary>
    /// Gets a generated name for this collection.
    /// </summary>
    /// <param name="level">The nesting level.</param>
    /// <returns>A generated collection name.</returns>
    public string GetName(int level)
    {
      return string.Format("<Collection of {0} plot items>", _plotItems.Count);
    }

    /// <summary>
    /// Gets a generated name for this collection.
    /// </summary>
    /// <param name="style">The style descriptor.</param>
    /// <returns>A generated collection name.</returns>
    public string GetName(string style)
    {
      return string.Format("<Collection of {0} plot items>", _plotItems.Count);
    }

    /// <summary>
    /// Copies from another object when supported.
    /// </summary>
    /// <param name="obj">The object to copy from.</param>
    /// <returns><see langword="true"/> if the object was handled; otherwise, <see langword="false"/>.</returns>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var from = obj as PlotItemCollection;
      if (from is not null)
      {
        CopyFrom(from, Gdi.GraphCopyOptions.All);
      }
      return false;
    }

    /// <inheritdoc />
    object ICloneable.Clone()
    {
      return new PlotItemCollection(this);
    }

    /// <summary>
    /// Creates a strongly typed copy of this collection.
    /// </summary>
    /// <returns>A cloned plot-item collection.</returns>
    public PlotItemCollection Clone()
    {
      return new PlotItemCollection(this);
    }

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

    /// <summary>
    /// Does nothing because a plot item collection does not apply a single plot-style template to itself.
    /// </summary>
    /// <param name="template">The template item. This value is ignored because collections do not own a single plot style.</param>
    /// <param name="strictness">The copy strictness. This value is ignored because collections do not own a single plot style.</param>
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
    /// Performs preprocessing for painting.
    /// </summary>
    /// <param name="context">The paint context.</param>
    public void PaintPreprocessing(IPaintContext context)
    {
      var coordTransStyle = _plotGroupStyles.CoordinateTransformingStyle;
      if (coordTransStyle is not null)
      {
        var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<IPlotArea>(this) ?? throw new InvalidProgramException("PlotItemCollection has not parent layer"); ;
        coordTransStyle.PaintPreprocessing(context, layer, this);
      }
      else
      {
        foreach (var pi in _plotItems)
          pi.PaintPreprocessing(context);
      }
    }

    /// <summary>
    /// Paints all contained plot items.
    /// </summary>
    /// <param name="g">The graphics context used for drawing.</param>
    /// <param name="context">The paint context.</param>
    /// <param name="layer">The plot layer.</param>
    /// <param name="previousPlotItem">The previous plot item.</param>
    /// <param name="nextPlotItem">The next plot item.</param>
    public void Paint(IGraphicsContext3D g, IPaintContext context, Graph3D.IPlotArea layer, IGPlotItem? previousPlotItem, IGPlotItem? nextPlotItem)
    {
      var coordinateTransformingStyle = _plotGroupStyles.CoordinateTransformingStyle;

      for (int i = 0; i < _plotItems.Count; ++i)
      {
        if (coordinateTransformingStyle is not null)
        {
          coordinateTransformingStyle.PaintChild(g, context, layer, this, i);
        }
        else
        {
          var previousItem = i == 0 ? null : _plotItems[i - 1];
          var nextItem = i == _plotItems.Count - 1 ? null : _plotItems[i + 1];
          _plotItems[i].Paint(g, context, layer, previousItem, nextItem);
        }
      }
    }

    /// <inheritdoc/>
    void IGPlotItem.PaintPostprocessing()
    {
      foreach (var pi in _plotItems)
        pi.PaintPostprocessing();
    }

    /// <summary>
    /// Performs hit testing for the collection.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    /// <param name="hitpoint">The hit-test data.</param>
    /// <returns>The hit-test object.</returns>
    public IHitTestObject HitTest(IPlotArea layer, HitTestPointData hitpoint)
    {
      throw new NotImplementedException();
    }

    #region Collection methods

    /// <summary>
    /// Adds a plot item to the collection.
    /// </summary>
    /// <param name="item">The plot item to add.</param>
    public void Add(IGPlotItem item)
    {
      if (item is null)
        throw new ArgumentNullException();

      item.ParentObject = this;
      _plotItems.Add(item);
    }

    #endregion Collection methods

    #region Change handling

    private void EhPlotItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      OnCollectionChanged();
    }

    /// <summary>
    /// Called when the collection contents change.
    /// </summary>
    protected virtual void OnCollectionChanged()
    {
      _cachedPlotItemsFlattened = null; // invalidate _cachedPlotItemsFlattened
      EhSelfChanged(new SimpleCollectionChangedEventArgs(this)); // notify items down in the hierarchy to invalidate _cachedPlotItemsFlattened
      EhSelfChanged(new BoundariesChangedEventArgs(BoundariesChangedData.ComplexChange)); // notify that possibly the boundaries have been changed
    }

    #endregion Change handling
  }
}
