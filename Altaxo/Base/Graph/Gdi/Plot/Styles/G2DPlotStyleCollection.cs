#region Copyright

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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Data;
  using Altaxo.Main;
  using Collections;
  using Data;
  using Graph.Plot.Groups;
  using Plot.Groups;

  /// <summary>
  /// Represents a collection of two-dimensional plot styles.
  /// </summary>
  public class G2DPlotStyleCollection
      :
      Main.SuspendableDocumentNodeWithSingleAccumulatedData<PlotItemStyleChangedEventArgs>,
      IEnumerable<IG2DPlotStyle>,
      IG2DPlotStyle
  {
    /// <summary>
    /// Holds the plot styles
    /// </summary>
    private List<IG2DPlotStyle> _innerList;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotStyleCollection", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Styles.G2DPlotStyleCollection", 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions is not supported");
        /*
G2DPlotStyleCollection s = (G2DPlotStyleCollection)obj;

info.CreateArray("Styles", s._innerList.Count);
for (int i = 0; i < s._innerList.Count; i++)
    info.AddValue("e", s._innerList[i]);
info.CommitArray();
*/
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        int count = info.OpenArray();
        var array = new IG2DPlotStyle[count];
        for (int i = 0; i < count; i++)
          array[i] = (IG2DPlotStyle)info.GetValue("e", null);
        info.CloseArray(count);

        if (o is null)
        {
          return new G2DPlotStyleCollection(array);
        }
        else
        {
          var s = (G2DPlotStyleCollection)o;
          for (int i = count - 1; i >= 0; i--)
            s.Add(array[i]);
          return s;
        }
      }
    }

    /// <summary>
    /// 2006-12-06 We changed the order in which the substyles are plotted.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DPlotStyleCollection), 2)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (G2DPlotStyleCollection)obj;

        info.CreateArray("Styles", s._innerList.Count);
        for (int i = 0; i < s._innerList.Count; i++)
          info.AddValue("e", s._innerList[i]);
        info.CommitArray();
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (G2DPlotStyleCollection?)o ?? new G2DPlotStyleCollection();

        int count = info.OpenArray();
        for (int i = 0; i < count; i++)
        {
          var item = info.GetValue("e", null);
          if (item is object[])
          {
            foreach (var itemInner in (object[])item)
              s.Add((IG2DPlotStyle)itemInner, false);
          }
          else
          {
            s.Add((IG2DPlotStyle)item, false);
          }
        }

        info.CloseArray(count);
        return s;
      }
    }

    #endregion Serialization

    #region Copying

    /// <summary>
    /// Copies all styles from another collection.
    /// </summary>
    /// <param name="from">The source collection.</param>
    [MemberNotNull(nameof(_innerList))]
    public void CopyFrom(G2DPlotStyleCollection from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      using (var suspendToken = SuspendGetToken())
      {
        Clear();

        _innerList = new List<IG2DPlotStyle>();
        for (int i = 0; i < from._innerList.Count; ++i)
          Add((IG2DPlotStyle)from[i].Clone());

        suspendToken.Resume();
      }
    }

    /// <summary>
    /// Copies styles from a template collection while attempting to reuse existing data references.
    /// </summary>
    /// <param name="from">The template collection.</param>
    /// <returns><c>true</c> if the operation completed.</returns>
    public bool CopyFromTemplateCollection(G2DPlotStyleCollection from)
    {
      if (ReferenceEquals(this, from))
        return true;

      using (var suspendToken = SuspendGetToken())
      {
        var oldInnerList = _innerList;

        _innerList = new List<IG2DPlotStyle>();

        for (int i = 0; i < from._innerList.Count; ++i)
        {
          var fromStyleType = from[i].GetType();

          // try to find the same style in the old list, and use the data from this style
          int foundIdx = oldInnerList.IndexOfFirst(item => item.GetType() == fromStyleType);

          IG2DPlotStyle clonedStyle;

          if (foundIdx >= 0) // if old style list has such an item, we clone that item, and then CopyFrom (but without data)
          {
            clonedStyle = (IG2DPlotStyle)oldInnerList[foundIdx].Clone(true); // First, clone _with_ the old data because we want to reuse them
            clonedStyle.CopyFrom(from[i], false); // now copy the properties from the template style, but _without_ the data
            oldInnerList.RemoveAt(foundIdx); // remove the used style now
          }
          else // an old style of the same type was not found
          {
            clonedStyle = (IG2DPlotStyle)from[i].Clone(false); // clone the style without data
          }

          Add(clonedStyle);
        }
        suspendToken.Resume();
      }

      return true;
    }

    /// <inheritdoc />
    public bool CopyFrom(object obj, bool copyWithDataReferences)
    {
      if (ReferenceEquals(this, obj))
        return true;
      var from = obj as G2DPlotStyleCollection;
      if (from is not null)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }

    /// <inheritdoc />
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      var from = obj as G2DPlotStyleCollection;
      if (from is not null)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }

    /// <inheritdoc />
    public object Clone(bool copyWithDataReferences)
    {
      return new G2DPlotStyleCollection(this);
    }

    /// <inheritdoc/>
    object ICloneable.Clone()
    {
      return new G2DPlotStyleCollection(this);
    }

    /// <summary>
    /// Creates a copy of this collection.
    /// </summary>
    /// <returns>The cloned collection.</returns>
    public G2DPlotStyleCollection Clone()
    {
      return new G2DPlotStyleCollection(this);
    }

    #endregion Copying

    /// <summary>
    /// Initializes a new empty instance of the <see cref="G2DPlotStyleCollection"/> class.
    /// </summary>
    public G2DPlotStyleCollection()
    {
      _innerList = new List<IG2DPlotStyle>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="G2DPlotStyleCollection"/> class with the specified styles.
    /// </summary>
    /// <param name="styles">The styles to add.</param>
    public G2DPlotStyleCollection(IG2DPlotStyle?[] styles)
    {
      _innerList = new List<IG2DPlotStyle>();
      for (int i = 0; i < styles.Length; ++i)
        if (styles[i] is { } style)
          Add(style, false);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="G2DPlotStyleCollection"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public G2DPlotStyleCollection(G2DPlotStyleCollection from)
    {
      CopyFrom(from);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="G2DPlotStyleCollection"/> class based on a predefined kind.
    /// </summary>
    /// <param name="kind">The style collection kind.</param>
    /// <param name="context">The property context.</param>
    public G2DPlotStyleCollection(LineScatterPlotStyleKind kind, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      _innerList = new List<IG2DPlotStyle>();

      switch (kind)
      {
        case LineScatterPlotStyleKind.Line:
          Add(new LinePlotStyle(context));
          break;

        case LineScatterPlotStyleKind.Scatter:
          Add(new ScatterPlotStyle(context));
          break;

        case LineScatterPlotStyleKind.LineAndScatter:
          Add(new ScatterPlotStyle(context));
          Add(new LinePlotStyle(context));
          break;
      }
    }

    /// <summary>
    /// Sets this collection from a template collection.
    /// </summary>
    /// <param name="from">The template collection.</param>
    /// <param name="strictness">The template-application strictness.</param>
    public void SetFromTemplate(G2DPlotStyleCollection from, PlotGroupStrictness strictness)
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IEnumerable<(
      string ColumnLabel, // Column label
      IReadableColumn? Column, // the column as it was at the time of this call
      string? ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn?> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns()
    {
      yield break; // no additionally used columns
    }

    /// <summary>
    /// Gets the style at the specified index.
    /// </summary>
    /// <param name="i">The style index.</param>
    public IG2DPlotStyle this[int i]
    {
      get
      {
        return _innerList[i];
      }
    }

    /// <summary>
    /// Gets the number of styles in the collection.
    /// </summary>
    public int Count
    {
      get
      {
        return _innerList.Count;
      }
    }

    /// <summary>
    /// Gets the styles contained in this collection.
    /// </summary>
    public IReadOnlyList<IG2DPlotStyle> Styles
    {
      get
      {
        return _innerList;
      }
    }

    /// <summary>
    /// Adds a style to the collection.
    /// </summary>
    /// <param name="toadd">The style to add.</param>
    public void Add(IG2DPlotStyle toadd)
    {
      Add(toadd, true);
    }

    /// <summary>
    /// Adds a style to the collection with optional reorganization and event handling.
    /// </summary>
    /// <param name="toadd">The style to add.</param>
    /// <param name="withReorganizationAndEvents">If set to <see langword="true"/>, change events are raised.</param>
    protected void Add(IG2DPlotStyle toadd, bool withReorganizationAndEvents)
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
    /// Replaces the style at the specified index with optional reorganization and event handling.
    /// </summary>
    /// <param name="ps">The replacement style.</param>
    /// <param name="idx">The index to replace.</param>
    /// <param name="withReorganizationAndEvents">If set to <see langword="true"/>, change events are raised.</param>
    protected void Replace(IG2DPlotStyle ps, int idx, bool withReorganizationAndEvents)
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
    /// Adds a range of styles to the collection.
    /// </summary>
    /// <param name="toadd">The styles to add.</param>
    public void AddRange(IG2DPlotStyle[] toadd)
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
    /// Inserts a style at the specified position.
    /// </summary>
    /// <param name="whichposition">The insertion index.</param>
    /// <param name="toinsert">The style to insert.</param>
    public void Insert(int whichposition, IG2DPlotStyle toinsert)
    {
      if (toinsert is not null)
      {
        _innerList.Insert(whichposition, toinsert);
        toinsert.ParentObject = this;

        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Removes all styles from the collection.
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
    /// Removes the style at the specified index.
    /// </summary>
    /// <param name="idx">The index to remove.</param>
    public void RemoveAt(int idx)
    {
      IG2DPlotStyle removed = this[idx];
      _innerList.RemoveAt(idx);

      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Exchanges the positions of two styles.
    /// </summary>
    /// <param name="pos1">The first position.</param>
    /// <param name="pos2">The second position.</param>
    public void ExchangeItemPositions(int pos1, int pos2)
    {
      IG2DPlotStyle item1 = this[pos1];
      _innerList[pos1] = _innerList[pos2];
      _innerList[pos2] = item1;

      EhSelfChanged(EventArgs.Empty);
    }

    /// <inheritdoc />
    protected override void AccumulateChangeData(object? sender, EventArgs e)
    {
      _accumulatedEventData = PlotItemStyleChangedEventArgs.Empty;
    }

    /// <inheritdoc />
    public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData? prevItemData, Processed2DPlotData? nextItemData)
    {
      if (pdata is null)
        throw new ArgumentNullException(nameof(pdata));

      for (int i = _innerList.Count - 1; i >= 0; i--)
      {
        this[i].Paint(g, layer, pdata, prevItemData, nextItemData);
      }
    }

    /// <inheritdoc />
    public RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
    {
      for (int i = _innerList.Count - 1; i >= 0; i--)
      {
        bounds = this[i].PaintSymbol(g, bounds);
      }

      return bounds;
    }

    /// <inheritdoc />
    public void PrepareScales(IPlotArea layer)
    {
      for (int i = 0; i < _innerList.Count; ++i)
      {
        this[i].PrepareScales(layer);
      }
    }

    /// <summary>
    /// Distributes changes from one sub-style to the other styles in the collection.
    /// </summary>
    /// <param name="pivot">The index of the changed sub-style.</param>
    /// <param name="layer">The plot layer.</param>
    /// <param name="pdata">The processed plot data.</param>
    public void DistributeSubStyleChange(int pivot, IPlotArea layer, Processed2DPlotData pdata)
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
    /// Prepares a new sub-style so that it can be added consistently to the collection.
    /// </summary>
    /// <param name="newSubStyle">The new sub-style.</param>
    /// <param name="layer">The plot layer.</param>
    /// <param name="pdata">The processed plot data.</param>
    public void PrepareNewSubStyle(IG2DPlotStyle newSubStyle, IPlotArea layer, Processed2DPlotData pdata)
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

    /// <summary>
    /// Returns an enumerator for the styles in the collection.
    /// </summary>
    /// <returns>An enumerator.</returns>
    public IEnumerator<IG2DPlotStyle> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion IEnumerable<IPlotStyle> Members

    #region IEnumerable Members

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion IEnumerable Members

    #region IPlotStyle Members

    /// <inheritdoc />
    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
      foreach (IG2DPlotStyle ps in this)
        ps.CollectExternalGroupStyles(externalGroups);
    }

    /// <inheritdoc />
    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      foreach (IG2DPlotStyle ps in this)
        ps.CollectLocalGroupStyles(externalGroups, localGroups);
    }

    /// <inheritdoc />
    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
    {
      foreach (IG2DPlotStyle ps in this)
        ps.PrepareGroupStyles(externalGroups, localGroups, layer, pdata);
    }

    /// <inheritdoc />
    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      foreach (IG2DPlotStyle ps in this)
        ps.ApplyGroupStyles(externalGroups, localGroups);
    }

    #endregion IPlotStyle Members

    #region IDocumentNode Members

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="options">Information what to replace.</param>
    /// <inheritdoc />
    public void VisitDocumentReferences(DocNodeProxyReporter options)
    {
      foreach (var s in this)
        s.VisitDocumentReferences(options);
    }

    #endregion IDocumentNode Members

    /// <inheritdoc />
    string INamedObject.Name
    {
      get { return GetType().Name; }
    }
  }
}
