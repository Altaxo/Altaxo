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
using Altaxo.Graph.Plot.Data;
using Altaxo.Main;

namespace Altaxo.Data.Selections
{
  /// <summary>
  /// Represents an union of row selections.
  /// </summary>
  /// <seealso cref="Altaxo.Main.SuspendableDocumentNodeWithEventArgs" />
  /// <seealso cref="Altaxo.Data.Selections.IRowSelection" />
  /// <seealso cref="Altaxo.Data.Selections.IRowSelectionCollection" />
  /// <example>If the first row selection is the range [0,10] and the second row selection is the range [5,15], then the union is the range [0,15].
  /// Overlapping segments as in this example are treated as one range, whereas segments that are not overlapping are treated as separate segments.</example>
  public class UnionOfRowSelections : Main.SuspendableDocumentNodeWithEventArgs, IRowSelection, IRowSelectionCollection
  {
    private List<IRowSelection> _rowSelections = new List<IRowSelection>();

    /// <summary>
    /// If true, segments that are adjoin, are merged into one larger segment. If false, segments that are adjoin will remain separated.
    /// </summary>
    private bool _mergeAdjoiningSegments = true;

    #region Serialization

    /// <summary>
    /// 2016-09-26 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(UnionOfRowSelections), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (UnionOfRowSelections)obj;

        info.CreateArray("RowSelections", s._rowSelections.Count);

        for (int i = 0; i < s._rowSelections.Count; ++i)
          info.AddValue("e", s._rowSelections[i]);

        info.CommitArray();

        info.AddValue("MergeAdjoiningSegments", s._mergeAdjoiningSegments);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        int count = info.OpenArray("RowSelections");

        var list = new List<IRowSelection>();

        for (int i = 0; i < count; ++i)
        {
          list.Add((IRowSelection)info.GetValue("e", parent));
        }

        info.CloseArray(count);

        bool mergeAdjoiningSegments = info.GetBoolean("MergeAdjoiningSegments");

        return new UnionOfRowSelections(info, list, mergeAdjoiningSegments);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="UnionOfRowSelections"/> class for deserialization purposes.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
    /// <param name="list">The list of row selections (is directly used).</param>
    /// <param name="mergeAdjoiningSegments">If true, segments that are adjoin, are merged into one larger segment. If false, segments that are adjoin will remain separated.</param>
    protected UnionOfRowSelections(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, List<IRowSelection> list, bool mergeAdjoiningSegments)
    {
      _rowSelections = list;
      foreach (var element in _rowSelections)
        element.ParentObject = this;

      _mergeAdjoiningSegments = mergeAdjoiningSegments;
    }

    /// <summary>
    /// Initializes a new, empty instance of the <see cref="UnionOfRowSelections"/> class.
    /// </summary>
    public UnionOfRowSelections()
    {
      _rowSelections = new List<IRowSelection>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnionOfRowSelections"/> class from an existing collections of row selections.
    /// The existing row selections are cloned before used in this class.
    /// </summary>
    /// <param name="rowSelections">The row selections. They are not used directly, but cloned before stored in this class.</param>
    /// <param name="mergeAdjoiningSegments">If true, segments that are adjoin, are merged into one larger segment. If false, segments that are adjoin will remain separated.</param>
    public UnionOfRowSelections(IEnumerable<IRowSelection> rowSelections, bool mergeAdjoiningSegments)
    {
      _rowSelections = new List<IRowSelection>(rowSelections.Select(itemToClone => { var clonedItem = (IRowSelection)itemToClone.Clone(); clonedItem.ParentObject = this; return clonedItem; }));
      _mergeAdjoiningSegments = mergeAdjoiningSegments;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnionOfRowSelections"/> class.
    /// </summary>
    /// <param name="rowSelectionsHead">The first row selections (cloned before stored).</param>
    /// <param name="selection">Another selection (cloned before stored).</param>
    /// <param name="rowSelectionTail">The last row selections (cloned before stored).</param>
    public UnionOfRowSelections(IEnumerable<IRowSelection> rowSelectionsHead, IRowSelection selection, IEnumerable<IRowSelection> rowSelectionTail)
    {
      _rowSelections = new List<IRowSelection>(rowSelectionsHead.Select(itemToClone => { var clonedItem = (IRowSelection)itemToClone.Clone(); clonedItem.ParentObject = this; return clonedItem; }));

      {
        var item = (IRowSelection)selection.Clone();
        item.ParentObject = this;
        _rowSelections.Add(item);
      }

      _rowSelections.AddRange(rowSelectionTail.Select(itemToClone => { var clonedItem = (IRowSelection)itemToClone.Clone(); clonedItem.ParentObject = this; return clonedItem; }));
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new UnionOfRowSelections(this, _mergeAdjoiningSegments);
    }

    /// <inheritdoc/>
    public bool IsCollectionWithOneItemEquivalentToThisItem { get { return true; } }

    /// <inheritdoc/>
    public IEnumerable<(int start, int endExclusive)> GetSelectedRowIndexSegmentsFromTo(int startIndex, int maxIndexExclusive, DataColumnCollection? table, int totalRowCount)
    {
      var _enumerators = new List<IEnumerator<(int start, int endExclusive)>>(_rowSelections.Count);

      maxIndexExclusive = Math.Min(maxIndexExclusive, totalRowCount);

      // get the enumerators, and move them to the start
      for (int i = 0; i < _rowSelections.Count; ++i)
      {
        var enumerator = _rowSelections[i].GetSelectedRowIndexSegmentsFromTo(startIndex, maxIndexExclusive, table, totalRowCount).GetEnumerator();

        // fast forward until at least the current endExclusive is greater than start parameter

        do
        {
          if (!enumerator.MoveNext())
          {
            enumerator.Dispose();
            enumerator = null;
          }
        } while (enumerator is not null && enumerator.Current.endExclusive <= startIndex);

        if (enumerator is not null)
          _enumerators.Add(enumerator);
      }

      for (; _enumerators.Count > 0;)
      {
        // sort remaining enumerators by their end member, but the lowest endExclusive at the end of the list
        _enumerators.Sort((a, b) => Comparer<int>.Default.Compare(b.Current.endExclusive, a.Current.endExclusive));

        int cM1 = _enumerators.Count - 1;
        var start = _enumerators[cM1].Current.start;
        var endExclusive = _enumerators[cM1].Current.endExclusive;
        if (!_enumerators[cM1].MoveNext())
        {
          _enumerators[cM1].Dispose();
          _enumerators.RemoveAt(cM1);
        }

        bool wasRangeExtended;
        do
        {
          wasRangeExtended = false;
          for (int i = _enumerators.Count - 1; i >= 0; --i)
          {
            if ((_mergeAdjoiningSegments && _enumerators[i].Current.start <= endExclusive) || // use < to separate neighbouring segments; use <= to merge neighbouring segments
                  (!_mergeAdjoiningSegments && _enumerators[i].Current.start < endExclusive))
            {
              wasRangeExtended = true;
              start = Math.Min(start, _enumerators[i].Current.start);
              endExclusive = Math.Max(endExclusive, _enumerators[i].Current.endExclusive);
              if (!_enumerators[i].MoveNext())
              {
                _enumerators[i].Dispose();
                _enumerators.RemoveAt(i);
              }
            }
          }
        } while (wasRangeExtended);

        start = Math.Max(start, startIndex);
        endExclusive = Math.Min(endExclusive, maxIndexExclusive);

        if (endExclusive > start)
          yield return (start, endExclusive);

        if (endExclusive == maxIndexExclusive)
          break;
      } // forever
    }

    /// <summary>
    /// If true, segments that are adjoin, are merged into one larger segment. If false, segments that are adjoin will remain separated.
    /// </summary>
    public bool MergeAdjoinigSegments { get { return _mergeAdjoiningSegments; } }

    /// <summary>
    /// Returns a new instance in which <see cref="MergeAdjoinigSegments"/> is set to the value of the provided argument.
    /// </summary>
    /// <param name="mergeAdjoiningSegments">If true, segments that are adjoin, are merged into one larger segment. If false, segments that are adjoin will remain separated.</param>
    /// <returns>New instance in which <see cref="MergeAdjoinigSegments"/> is set to the value of the provided argument.</returns>
    public UnionOfRowSelections WithMergeAdjoiningSegments(bool mergeAdjoiningSegments)
    {
      if (_mergeAdjoiningSegments == mergeAdjoiningSegments)
        return this;
      else
        return new UnionOfRowSelections(this, mergeAdjoiningSegments);
    }

    /// <summary>
    /// Returns a new instance with all row selections from this instance plus one additional item.
    /// </summary>
    /// <param name="item">The item (cloned before stored).</param>
    /// <returns>New instance with all row selections from this instance plus one additional item.</returns>
    public IRowSelectionCollection WithAdditionalItem(IRowSelection item)
    {
      return new UnionOfRowSelections(_rowSelections.Concat(new[] { item }), _mergeAdjoiningSegments);
    }

    /// <summary>
    /// Returns a new instance that resembles this instance, but with the item at index <paramref name="idx"/> set to another item <paramref name="item"/>.
    /// </summary>
    /// <param name="idx">The index to change.</param>
    /// <param name="item">The new item at this index.</param>
    /// <returns>New instance that resembles this instance, but with the item at index <paramref name="idx"/> set to another item <paramref name="item"/>.</returns>
    public IRowSelectionCollection WithChangedItem(int idx, IRowSelection item)
    {
      return new UnionOfRowSelections(_rowSelections.Take(idx), item, _rowSelections.Skip(idx + 1));
    }

    /// <summary>
    /// Enumerates all row selections in this collection.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<IRowSelection> GetEnumerator()
    {
      return _rowSelections.GetEnumerator();
    }

    /// <summary>
    /// Enumerates all row selections in this collection.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _rowSelections.GetEnumerator();
    }

    /// <summary>
    /// Returns a new instance of <see cref="UnionOfRowSelections"/> with only the provided items (cloned before stored).
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns>New instance of <see cref="UnionOfRowSelections"/> with only the provided items (cloned before stored).</returns>
    public IRowSelectionCollection NewWithItems(IEnumerable<IRowSelection> items)
    {
      return new UnionOfRowSelections(items, _mergeAdjoiningSegments);
    }

    /// <inheritdoc/>
    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      for (int i = 0; i < _rowSelections.Count; ++i)
      {
        yield return new DocumentNodeAndName(_rowSelections[i], () => _rowSelections[i] = null!, "RowSelection#" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
      }
    }

    /// <inheritdoc/>
    public IEnumerable<ColumnInformationSimple> GetAdditionallyUsedColumns()
    {
      for (int i = 0; i < _rowSelections.Count; ++i)
      {
        foreach (var item in _rowSelections[i].GetAdditionallyUsedColumns())
          yield return item;
      }
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      for (int i = 0; i < _rowSelections.Count; ++i)
      {
        _rowSelections[i].VisitDocumentReferences(Report);
      }
    }
  }
}
