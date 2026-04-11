#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Data;
  using Altaxo.Main;
  using Collections;
  using Data;
  using Geometry;
  using Graph.Plot.Groups;
  using GraphicsContext;
  using Plot.Groups;

  /// <summary>
  /// Represents a collection of three-dimensional plot styles.
  /// </summary>
  public class G3DPlotStyleCollection
    :
    Main.SuspendableDocumentNodeWithSingleAccumulatedData<PlotItemStyleChangedEventArgs>,
    IEnumerable<IG3DPlotStyle>,
    IG3DPlotStyle
  {
    /// <summary>
    /// Holds the plot styles
    /// </summary>
    private List<IG3DPlotStyle> _innerList;

    #region Serialization

    /// <summary>
    /// 2016-05-30 Initial version
    /// </summary>
    /// <summary>
    /// Serializes <see cref="G3DPlotStyleCollection"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G3DPlotStyleCollection), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (G3DPlotStyleCollection)obj;

        info.CreateArray("Styles", s._innerList.Count);
        for (int i = 0; i < s._innerList.Count; i++)
          info.AddValue("e", s._innerList[i]);
        info.CommitArray();
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        int count = info.OpenArray();
        var array = new IG3DPlotStyle[count];
        for (int i = 0; i < count; i++)
          array[i] = (IG3DPlotStyle)info.GetValue("e", null);
        info.CloseArray(count);

        if (o is null)
        {
          return new G3DPlotStyleCollection(array);
        }
        else
        {
          var s = (G3DPlotStyleCollection)o;
          for (int i = 0; i < count; i++)
            s.Add(array[i]);
          return s;
        }
      }
    }

    #endregion Serialization

    #region Copying

    /// <summary>
    /// Copies the contents of another style collection.
    /// </summary>
    /// <param name="from">The collection to copy from.</param>
    [MemberNotNull(nameof(_innerList))]
    public void CopyFrom(G3DPlotStyleCollection from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      using (var suspendToken = SuspendGetToken())
      {
        Clear();

        _innerList = new List<IG3DPlotStyle>();
        for (int i = 0; i < from._innerList.Count; ++i)
          Add((IG3DPlotStyle)from[i].Clone());

        suspendToken.Resume();
      }
    }

    /// <summary>
    /// Copies all styles 1:1 from a template collection, but try to reuse the data columns from
    /// the old styles collection. This function is used if the user has selected the <see cref="PlotGroupStrictness.Strict"/>.
    /// </summary>
    /// <param name="from">The template style collection to copy from.</param>
    /// <returns>On return, this collection has exactly the same styles as the template collection, in
    /// exactly the same order and with the same properties, except for the data of the styles. The style data
    /// are tried to reuse from the old styles. If this is not possible, the data references will be left empty.</returns>
    public bool CopyFromTemplateCollection(G3DPlotStyleCollection from)
    {
      if (ReferenceEquals(this, from))
        return true;

      using (var suspendToken = SuspendGetToken())
      {
        var oldInnerList = _innerList;

        _innerList = new List<IG3DPlotStyle>();

        for (int i = 0; i < from._innerList.Count; ++i)
        {
          var fromStyleType = from[i].GetType();

          // try to find the same style in the old list, and use the data from this style
          int foundIdx = oldInnerList.IndexOfFirst(item => item.GetType() == fromStyleType);

          IG3DPlotStyle clonedStyle;

          if (foundIdx >= 0) // if old style list has such an item, we clone that item, and then CopyFrom (but without data)
          {
            clonedStyle = (IG3DPlotStyle)oldInnerList[foundIdx].Clone(true); // First, clone _with_ the old data because we want to reuse them
            clonedStyle.CopyFrom(from[i], false); // now copy the properties from the template style, but _without_ the data
            oldInnerList.RemoveAt(foundIdx); // remove the used style now
          }
          else // an old style of the same type was not found
          {
            clonedStyle = (IG3DPlotStyle)from[i].Clone(false); // clone the style without data
          }

          Add(clonedStyle);
        }
        suspendToken.Resume();
      }

      return true;
    }

    /// <inheritdoc/>
    public bool CopyFrom(object obj, bool copyWithDataReferences)
    {
      if (ReferenceEquals(this, obj))
        return true;
      var from = obj as G3DPlotStyleCollection;
      if (from is not null)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Copies the member variables from another instance.
    /// </summary>
    /// <param name="obj">Another instance to copy the data from.</param>
    /// <returns>True if data was copied, otherwise false.</returns>
    /// <inheritdoc/>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      return CopyFrom(obj, true);
    }

    /// <inheritdoc/>
    /// <inheritdoc/>
    public object Clone(bool copyWithDataReferences)
    {
      return new G3DPlotStyleCollection(this);
    }

    /// <inheritdoc/>
    object ICloneable.Clone()
    {
      return new G3DPlotStyleCollection(this);
    }

    /// <summary>
    /// Creates a strongly typed clone of this collection.
    /// </summary>
    /// <returns>A cloned style collection.</returns>
    public G3DPlotStyleCollection Clone()
    {
      return new G3DPlotStyleCollection(this);
    }

    #endregion Copying

    #region Construction

    /// <summary>
    /// Creates an empty collection, i.e. without any styles (so the item is not visible). You must manually add styles to make the plot item visible.
    /// </summary>
    /// <summary>
    /// Initializes a new instance of the <see cref="G3DPlotStyleCollection"/> class.
    /// </summary>
    public G3DPlotStyleCollection()
    {
      _innerList = new List<IG3DPlotStyle>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="G3DPlotStyleCollection"/> class with the specified styles.
    /// </summary>
    /// <param name="styles">The styles to add.</param>
    public G3DPlotStyleCollection(IG3DPlotStyle[] styles)
    {
      _innerList = new List<IG3DPlotStyle>();
      for (int i = 0; i < styles.Length; ++i)
        if (styles[i] is not null)
          Add(styles[i], false);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="G3DPlotStyleCollection"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The collection to copy from.</param>
    public G3DPlotStyleCollection(G3DPlotStyleCollection from)
    {
      CopyFrom(from);
    }

    #endregion Construction

    /// <inheritdoc/>
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_innerList is not null)
      {
        for (int i = _innerList.Count - 1; i >= 0; --i)
        {
          if (_innerList[i] is not null)
            yield return new Main.DocumentNodeAndName(_innerList[i], "Style" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
      }
    }

    /// <summary>
    /// Updates this collection from a template collection according to the specified strictness.
    /// </summary>
    /// <param name="from">The template collection.</param>
    /// <param name="strictness">The template application strictness.</param>
    public void SetFromTemplate(G3DPlotStyleCollection from, PlotGroupStrictness strictness)
    {
      if (strictness == PlotGroupStrictness.Strict)
      {
        CopyFromTemplateCollection(from); // take the whole style collection as is from the template, but try to reuse the additionally needed data columns from the old style
      }
      else if (strictness == PlotGroupStrictness.Exact)
      {
        // note one sub style in the 'from' collection can update only one item in the 'this' collection
        using (var suspendToken = SuspendGetToken())
        {
          var indicesFrom = new SortedSet<int>(System.Linq.Enumerable.Range(0, from.Count));

          for (int i = 0; i < Count; ++i)
          {
            var thisStyleType = this[i].GetType();

            // search in from for a style with the same name
            foreach (var fromIndex in indicesFrom)
            {
              if (thisStyleType == from[fromIndex].GetType())
              {
                this[i].CopyFrom(from[fromIndex], false);
                indicesFrom.Remove(fromIndex); // this from style was used, thus remove it
                break;
              }
            }
          }
          suspendToken.Resume();
        }
      }
    }

    /// <summary>
    /// Gets the plot style at the specified index.
    /// </summary>
    /// <param name="i">The zero-based index.</param>
    /// <returns>The plot style at the specified index.</returns>
    public IG3DPlotStyle this[int i]
    {
      get
      {
        return _innerList[i];
      }
    }

    /// <summary>
    /// Gets the number of plot styles in the collection.
    /// </summary>
    public int Count
    {
      get
      {
        return _innerList.Count;
      }
    }

    /// <summary>
    /// Adds a plot style to the collection.
    /// </summary>
    /// <param name="toadd">The plot style to add.</param>
    public void Add(IG3DPlotStyle toadd)
    {
      Add(toadd, true);
    }

    /// <summary>
    /// Adds a plot style to the collection.
    /// </summary>
    /// <param name="toadd">The plot style to add.</param>
    /// <param name="withReorganizationAndEvents">If set to <see langword="true"/>, change events are raised afterwards.</param>
    protected void Add(IG3DPlotStyle toadd, bool withReorganizationAndEvents)
    {
      if (toadd is not null)
      {
        _innerList.Add(toadd);
        toadd.ParentObject = this;

        if (withReorganizationAndEvents)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Replaces the plot style at the specified index.
    /// </summary>
    /// <param name="ps">The replacement plot style.</param>
    /// <param name="idx">The zero-based index to replace.</param>
    /// <param name="withReorganizationAndEvents">If set to <see langword="true"/>, change events are raised afterwards.</param>
    protected void Replace(IG3DPlotStyle ps, int idx, bool withReorganizationAndEvents)
    {
      if (ps is not null)
      {
        _innerList[idx] = ps;
        ps.ParentObject = this;

        if (withReorganizationAndEvents)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Adds a range of plot styles to the collection.
    /// </summary>
    /// <param name="toadd">The plot styles to add.</param>
    public void AddRange(IG3DPlotStyle[] toadd)
    {
      if (toadd is not null)
      {
        for (int i = 0; i < toadd.Length; i++)
        {
          _innerList.Add(toadd[i]);
          toadd[i].ParentObject = this;
        }

        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Inserts a plot style at the specified index.
    /// </summary>
    /// <param name="whichposition">The zero-based insertion index.</param>
    /// <param name="toinsert">The plot style to insert.</param>
    public void Insert(int whichposition, IG3DPlotStyle toinsert)
    {
      if (toinsert is not null)
      {
        _innerList.Insert(whichposition, toinsert);
        toinsert.ParentObject = this;

        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Removes all plot styles from the collection.
    /// </summary>
    public void Clear()
    {
      if (_innerList is not null)
      {
        _innerList.Clear();

        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Removes the plot style at the specified index.
    /// </summary>
    /// <param name="idx">The zero-based index of the plot style to remove.</param>
    public void RemoveAt(int idx)
    {
      var removed = this[idx];
      _innerList.RemoveAt(idx);

      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Exchanges the positions of two plot styles in the collection.
    /// </summary>
    /// <param name="pos1">The first index.</param>
    /// <param name="pos2">The second index.</param>
    public void ExchangeItemPositions(int pos1, int pos2)
    {
      var item1 = this[pos1];
      _innerList[pos1] = _innerList[pos2];
      _innerList[pos2] = item1;

      EhSelfChanged(EventArgs.Empty);
    }

    /// <inheritdoc/>
    protected override void AccumulateChangeData(object? sender, EventArgs e)
    {
      _accumulatedEventData = PlotItemStyleChangedEventArgs.Empty;
    }

    /// <inheritdoc/>
    public void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData? prevItemData, Processed3DPlotData? nextItemData)
    {
      if (pdata is null)
        throw new ArgumentNullException(nameof(pdata));

      for (int i = _innerList.Count - 1; i >= 0; i--)
      {
        this[i].Paint(g, layer, pdata, prevItemData, nextItemData);
      }
    }

    /// <summary>
    /// Prepares the scales of the plot styles. This is intended to be used with plot styles which
    /// have an internal scale, for instance <see cref="Gdi.Plot.Styles.ColumnDrivenColorPlotStyle"/> or
    /// <see cref="Gdi.Plot.Styles.ColumnDrivenSymbolSizePlotStyle"/>, which should act on this call with updating their internal scale.
    /// </summary>
    /// <param name="layer">The parent layer.</param>
    public void PrepareScales(IPlotArea layer)
    {
      for (int i = _innerList.Count - 1; i >= 0; i--)
      {
        this[i].PrepareScales(layer);
      }
    }

    /// <inheritdoc/>
    public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
    {
      for (int i = _innerList.Count - 1; i >= 0; i--)
      {
        bounds = this[i].PaintSymbol(g, bounds);
      }

      return bounds;
    }

    /// <summary>
    /// Distibute changes made to one group style of the collection (at index <c>pivot</c> to all other members of the collection.
    /// </summary>
    /// <param name="pivot">Index of the group style that was changed. This style keeps it's properties.</param>
    /// <param name="layer"></param>
    /// <param name="pdata"></param>
    public void DistributeSubStyleChange(int pivot, IPlotArea layer, Processed3DPlotData pdata)
    {
      var externGroup = new PlotGroupStyleCollection();
      var localGroup = new PlotGroupStyleCollection();
      // because we don't step, the order is essential only for PrepareStyles
      for (int i = 0; i < _innerList.Count; i++)
        CollectLocalGroupStyles(externGroup, localGroup);

      // prepare
      this[pivot].PrepareGroupStyles(externGroup, localGroup, layer, pdata);
      for (int i = 0; i < Count; i++)
        if (i != pivot)
          this[i].PrepareGroupStyles(externGroup, localGroup, layer, pdata);

      // apply
      this[pivot].ApplyGroupStyles(externGroup, localGroup);
      for (int i = 0; i < Count; i++)
        if (i != pivot)
          this[i].ApplyGroupStyles(externGroup, localGroup);
    }

    /// <summary>
    /// Prepares a new substyle (one that is not already in the collection) for becoming member of the collection. The substyle will get
    /// all distributes group properties (local only) of this style collection.
    /// </summary>
    /// <param name="newSubStyle">Sub style to prepare.</param>
    /// <param name="layer"></param>
    /// <param name="pdata"></param>
    public void PrepareNewSubStyle(IG3DPlotStyle newSubStyle, IPlotArea layer, Processed3DPlotData pdata)
    {
      var externGroup = new PlotGroupStyleCollection();
      var localGroup = new PlotGroupStyleCollection();
      // because we don't step, the order is essential only for PrepareStyles
      for (int i = 0; i < _innerList.Count; i++)
        this[i].CollectLocalGroupStyles(externGroup, localGroup);
      newSubStyle.CollectLocalGroupStyles(externGroup, localGroup);

      // prepare
      for (int i = 0; i < Count; i++)
        this[i].PrepareGroupStyles(externGroup, localGroup, layer, pdata);
      newSubStyle.PrepareGroupStyles(externGroup, localGroup, layer, pdata);

      // apply
      for (int i = 0; i < Count; i++)
        this[i].ApplyGroupStyles(externGroup, localGroup);
      newSubStyle.ApplyGroupStyles(externGroup, localGroup);
    }

    #region IEnumerable<IPlotStyle> Members

    /// <inheritdoc/>
    public IEnumerator<IG3DPlotStyle> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion IEnumerable<IPlotStyle> Members

    #region IEnumerable Members

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion IEnumerable Members

    #region IPlotStyle Members

    /// <inheritdoc/>
    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
      foreach (var ps in this)
        ps.CollectExternalGroupStyles(externalGroups);
    }

    /// <inheritdoc/>
    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      foreach (var ps in this)
        ps.CollectLocalGroupStyles(externalGroups, localGroups);
    }

    /// <inheritdoc/>
    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
    {
      foreach (var ps in this)
        ps.PrepareGroupStyles(externalGroups, localGroups, layer, pdata);
    }

    /// <inheritdoc/>
    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      foreach (var ps in this)
        ps.ApplyGroupStyles(externalGroups, localGroups);
    }

    #endregion IPlotStyle Members

    #region IDocumentNode Members

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="options">Information what to replace.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter options)
    {
      foreach (var s in this)
        s.VisitDocumentReferences(options);
    }

    #endregion IDocumentNode Members

    #region IRoutedPropertyReceiver Members

    /// <inheritdoc/>
    public IEnumerable<(
      string ColumnLabel, // Column label
      IReadableColumn? Column, // the column as it was at the time of this call
      string? ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn?> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns()
    {
      yield break; // no additionally used columns
    }

    #endregion IRoutedPropertyReceiver Members

    /// <inheritdoc />
    string INamedObject.Name
    {
      get { return GetType().Name; }
    }
  }
}
