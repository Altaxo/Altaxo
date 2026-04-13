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
using System.Collections.Generic;
using Altaxo.Graph.Plot.Data;
using Altaxo.Main;

namespace Altaxo.Data.Selections
{
  /// <summary>
  /// Selects a contiguous range of row indices.
  /// </summary>
  public class RangeOfRowIndices : Main.SuspendableDocumentLeafNodeWithEventArgs, IRowSelection, ICloneable
  {
    /// <summary>
    /// The first row index included in the range.
    /// </summary>
    public int _firstRowIndexInclusive;

    /// <summary>
    /// The last row index included in the range.
    /// </summary>
    public int _lastRowIndexInclusive;

    /// <summary>
    /// Gets the first row index included in the range.
    /// </summary>
    public int Start { get { return _firstRowIndexInclusive; } }

    /// <summary>
    /// Gets the number of rows represented by this range.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Gets the end of the range (the index of the last data row that will be plotted).
    /// </summary>
    /// <value>
    /// The end of the range (inclusive).
    /// </value>
    public int LastInclusive
    {
      get
      {
        return _lastRowIndexInclusive;
      }
    }

    #region Serialization

    /// <summary>
    /// 2016-09-25 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.Selections.RangeOfRows", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old versions");
        /*
                var s = (RangeOfRows)obj;

                info.AddValue("Start", s.Start);
                info.AddValue("Count", s.Count);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var start = info.GetInt32("Start");
        var count = info.GetInt32("Count");
        return RangeOfRowIndices.FromStartAndCount(start, count);
      }
    }

    /// <summary>
    /// 2016-10-02 instead of Count, now use the last (included) row index. Renamed to RangeOfRowIndices
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RangeOfRowIndices), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RangeOfRowIndices)o;

        info.AddValue("First", s._firstRowIndexInclusive);
        info.AddValue("Last", s._lastRowIndexInclusive);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var first = info.GetInt32("First");
        var last = info.GetInt32("Last");
        return new RangeOfRowIndices { _firstRowIndexInclusive = first, _lastRowIndexInclusive = last };
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeOfRowIndices"/> class.
    /// </summary>
    public RangeOfRowIndices()
    {
      _firstRowIndexInclusive = 0;
      _lastRowIndexInclusive = -1;
    }

    /// <summary>
    /// Creates a row selection from a start index and a row count.
    /// </summary>
    /// <param name="start">The first row index.</param>
    /// <param name="count">The number of rows.</param>
    /// <returns>A row selection covering the requested range.</returns>
    public static IRowSelection FromStartAndCount(int start, int count)
    {
      if (!(start >= 0))
        throw new ArgumentOutOfRangeException(nameof(start));
      if (!(count >= 0))
        throw new ArgumentOutOfRangeException(nameof(count));

      if (0 == count)
        return new RangeOfRowIndices { _firstRowIndexInclusive = start + 1, _lastRowIndexInclusive = start };

      int endIncl = start + Math.Min(count - 1, int.MaxValue - start);

      if (0 == start && (int.MaxValue == count || int.MaxValue == endIncl))
        return new AllRows();
      else
        return new RangeOfRowIndices { _firstRowIndexInclusive = start, _lastRowIndexInclusive = endIncl };
    }

    /// <summary>
    /// Creates a row selection from the first and last included row indices.
    /// </summary>
    /// <param name="start">The first included row index.</param>
    /// <param name="endInclusive">The last included row index.</param>
    /// <returns>A row selection covering the requested range.</returns>
    public static RangeOfRowIndices FromStartAndEndInclusive(int start, int endInclusive)
    {
      return new RangeOfRowIndices { _firstRowIndexInclusive = start, _lastRowIndexInclusive = endInclusive };
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new RangeOfRowIndices { _firstRowIndexInclusive = _firstRowIndexInclusive, _lastRowIndexInclusive = _lastRowIndexInclusive };
    }

    /// <inheritdoc/>
    public IEnumerable<(int start, int endExclusive)> GetSelectedRowIndexSegmentsFromTo(int startIndex, int maxIndexExclusive, DataColumnCollection? table, int totalRowCount)
    {
      int start = Math.Max(startIndex, _firstRowIndexInclusive >= 0 ? _firstRowIndexInclusive : _firstRowIndexInclusive + totalRowCount);
      int endInclusive = Math.Min(Math.Min(maxIndexExclusive - 1, totalRowCount - 1), _lastRowIndexInclusive >= 0 ? _lastRowIndexInclusive : _lastRowIndexInclusive + totalRowCount);

      if (endInclusive >= start)
        yield return (start, endInclusive + 1);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is spanning all rows.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is spanning all rows; otherwise, <c>false</c>.
    /// </value>
    public bool IsSpanningAllRows { get { return 0 == Start && (int.MaxValue == _lastRowIndexInclusive || -1 == _lastRowIndexInclusive); } }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return 13 * _firstRowIndexInclusive.GetHashCode() + 31 * _lastRowIndexInclusive.GetHashCode();
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      var from = obj as RangeOfRowIndices;
      if (from is not null)
        return _firstRowIndexInclusive == from._firstRowIndexInclusive && _lastRowIndexInclusive == from._lastRowIndexInclusive;
      else
        return false;
    }

    /// <inheritdoc/>
    public IEnumerable<ColumnInformationSimple> GetAdditionallyUsedColumns()
    {
      yield break;
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
    }

    /// <inheritdoc/>
    public IEnumerable<IRowSelection>? ChildNodes => null;


    /// <inheritdoc/>
    public bool Equals(IRowSelection? other)
    {
      return
        other is RangeOfRowIndices otherX &&
        this._firstRowIndexInclusive == otherX._firstRowIndexInclusive &&
        this._lastRowIndexInclusive == otherX._lastRowIndexInclusive;
    }
  }
}
