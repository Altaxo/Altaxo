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
using Altaxo.Main;

namespace Altaxo.Data.Selections
{
  public class RangeOfRowIndices : Main.SuspendableDocumentLeafNodeWithEventArgs, IRowSelection, ICloneable
  {
    public int _firstRowIndexInclusive;

    public int _lastRowIndexInclusive;

    public int Start { get { return _firstRowIndexInclusive; } }

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
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
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
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RangeOfRowIndices)obj;

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

    public RangeOfRowIndices()
    {
      _firstRowIndexInclusive = 0;
      _lastRowIndexInclusive = -1;
    }

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

    public static RangeOfRowIndices FromStartAndEndInclusive(int start, int endInclusive)
    {
      return new RangeOfRowIndices { _firstRowIndexInclusive = start, _lastRowIndexInclusive = endInclusive };
    }

    public object Clone()
    {
      return new RangeOfRowIndices { _firstRowIndexInclusive = _firstRowIndexInclusive, _lastRowIndexInclusive = _lastRowIndexInclusive };
    }

    /// <inheritdoc/>
    public IEnumerable<(int start, int endExclusive)> GetSelectedRowIndexSegmentsFromTo(int startIndex, int maxIndex, DataColumnCollection table, int totalRowCount)
    {
      int start = Math.Max(startIndex, _firstRowIndexInclusive >= 0 ? _firstRowIndexInclusive : _firstRowIndexInclusive + totalRowCount);
      int endInclusive = Math.Min(Math.Min(maxIndex - 1, totalRowCount - 1), _lastRowIndexInclusive >= 0 ? _lastRowIndexInclusive : _lastRowIndexInclusive + totalRowCount);

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

    public override int GetHashCode()
    {
      return 13 * _firstRowIndexInclusive.GetHashCode() + 31 * _lastRowIndexInclusive.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
      var from = obj as RangeOfRowIndices;
      if (null != from)
        return _firstRowIndexInclusive == from._firstRowIndexInclusive && _lastRowIndexInclusive == from._lastRowIndexInclusive;
      else
        return false;
    }

    /// <inheritdoc/>
    public IEnumerable<(
      string ColumnLabel, // Column label
      IReadableColumn? Column, // the column as it was at the time of this call
      string ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns()
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
  }
}
