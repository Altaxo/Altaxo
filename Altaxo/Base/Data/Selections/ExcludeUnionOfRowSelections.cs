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
  /// Represents an exclusion of a union of row selections.
  /// </summary>
  /// <seealso cref="Altaxo.Main.SuspendableDocumentNodeWithEventArgs" />
  /// <seealso cref="Altaxo.Data.Selections.IRowSelection" />
  /// <seealso cref="Altaxo.Data.Selections.IRowSelectionCollection" />
  /// <example>If the first row selection is the range [3,10] and the second row selection is the range [8,15], then the union is the range [3,15].
  /// If our entire plotting range was e.g. [0,40], then the effective ranges that will be plot are [0,2] and [16,40].
  /// Overlapping segments as in this example are treated as one range, whereas segments that are not overlapping are treated as separate segments.</example>
  public class ExcludeUnionOfRowSelections : Main.SuspendableDocumentNodeWithEventArgs, IRowSelection, IRowSelectionCollection
  {
    private List<IRowSelection> _rowSelections = new List<IRowSelection>();

    #region Serialization

    /// <summary>
    /// 2017-08-16 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExcludeUnionOfRowSelections), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExcludeUnionOfRowSelections)obj;

        info.CreateArray("RowSelections", s._rowSelections.Count);

        for (int i = 0; i < s._rowSelections.Count; ++i)
          info.AddValue("e", s._rowSelections[i]);

        info.CommitArray();
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

        return new ExcludeUnionOfRowSelections(info, list);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcludeUnionOfRowSelections"/> class for deserialization purposes.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
    /// <param name="list">The list of row selections (is directly used).</param>
    protected ExcludeUnionOfRowSelections(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, List<IRowSelection> list)
    {
      _rowSelections = list;
      foreach (var element in _rowSelections)
        element.ParentObject = this;
    }

    /// <summary>
    /// Initializes a new, empty instance of the <see cref="ExcludeUnionOfRowSelections"/> class.
    /// </summary>
    public ExcludeUnionOfRowSelections()
    {
      _rowSelections = new List<IRowSelection>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcludeUnionOfRowSelections"/> class from an existing collections of row selections.
    /// The existing row selections are cloned before used in this class.
    /// </summary>
    /// <param name="rowSelections">The row selections. They are not used directly, but cloned before stored in this class.</param>
    public ExcludeUnionOfRowSelections(IEnumerable<IRowSelection> rowSelections)
    {
      _rowSelections = new List<IRowSelection>(rowSelections.Select(itemToClone => { var clonedItem = (IRowSelection)itemToClone.Clone(); clonedItem.ParentObject = this; return clonedItem; }));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcludeUnionOfRowSelections"/> class.
    /// </summary>
    /// <param name="rowSelectionsHead">The first row selections (cloned before stored).</param>
    /// <param name="selection">Another selection (cloned before stored).</param>
    /// <param name="rowSelectionTail">The last row selections (cloned before stored).</param>
    public ExcludeUnionOfRowSelections(IEnumerable<IRowSelection> rowSelectionsHead, IRowSelection selection, IEnumerable<IRowSelection> rowSelectionTail)
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
    public IEnumerable<IRowSelection>? ChildNodes => _rowSelections;


    /// <inheritdoc/>
    public object Clone()
    {
      return new ExcludeUnionOfRowSelections(this);
    }

    /// <inheritdoc/>
    public bool IsCollectionWithOneItemEquivalentToThisItem { get { return false; } }

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

      int startOfSegment = startIndex;

      for (; _enumerators.Count > 0;)
      {
        // sort remaining enumerators by their end member, but the lowest endExclusive at the end of the list
        _enumerators.Sort((a, b) => Comparer<int>.Default.Compare(b.Current.endExclusive, a.Current.endExclusive));

        int cM1 = _enumerators.Count - 1;
        var startOfUnion = _enumerators[cM1].Current.start;
        var endOfUnionExclusive = _enumerators[cM1].Current.endExclusive;
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
            if (_enumerators[i].Current.start <= endOfUnionExclusive) // use < to separate neighbouring segments; use <= to merge neighbouring segments

            {
              wasRangeExtended = true;
              startOfUnion = Math.Min(startOfUnion, _enumerators[i].Current.start);
              endOfUnionExclusive = Math.Max(endOfUnionExclusive, _enumerators[i].Current.endExclusive);
              if (!_enumerators[i].MoveNext())
              {
                _enumerators[i].Dispose();
                _enumerators.RemoveAt(i);
              }
            }
          }
        } while (wasRangeExtended);

        startOfUnion = Math.Max(startOfUnion, startIndex);
        endOfUnionExclusive = Math.Min(endOfUnionExclusive, maxIndexExclusive);

        if (startOfSegment < startOfUnion)
          yield return (startOfSegment, startOfUnion);

        startOfSegment = endOfUnionExclusive;

        if (endOfUnionExclusive == maxIndexExclusive)
          break;
      } // forever

      // the last segment
      if (startOfSegment < maxIndexExclusive)
        yield return (startOfSegment, maxIndexExclusive);
    }

    /// <summary>
    /// Returns a new instance with all row selections from this instance plus one additional item.
    /// </summary>
    /// <param name="item">The item (cloned before stored).</param>
    /// <returns>New instance with all row selections from this instance plus one additional item.</returns>
    public IRowSelectionCollection WithAdditionalItem(IRowSelection item)
    {
      return new ExcludeUnionOfRowSelections(_rowSelections.Concat(new[] { item }));
    }

    /// <summary>
    /// Returns a new instance that resembles this instance, but with the item at index <paramref name="idx"/> set to another item <paramref name="item"/>.
    /// </summary>
    /// <param name="idx">The index to change.</param>
    /// <param name="item">The new item at this index.</param>
    /// <returns>New instance that resembles this instance, but with the item at index <paramref name="idx"/> set to another item <paramref name="item"/>.</returns>
    public IRowSelectionCollection WithChangedItem(int idx, IRowSelection item)
    {
      return new ExcludeUnionOfRowSelections(_rowSelections.Take(idx), item, _rowSelections.Skip(idx + 1));
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
    /// Returns a new instance of <see cref="ExcludeUnionOfRowSelections"/> with only the provided items (cloned before stored).
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns>New instance of <see cref="ExcludeUnionOfRowSelections"/> with only the provided items (cloned before stored).</returns>
    public IRowSelectionCollection NewWithItems(IEnumerable<IRowSelection> items)
    {
      return new ExcludeUnionOfRowSelections(items);
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
