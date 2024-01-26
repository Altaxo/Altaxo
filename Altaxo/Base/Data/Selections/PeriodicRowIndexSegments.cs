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
  public class PeriodicRowIndexSegments : Main.SuspendableDocumentLeafNodeWithEventArgs, IRowSelection, ICloneable
  {
    public int _firstRowIndexInclusive;

    public int _lengthOfPeriod;

    public int _numberOfItemsPerPeriod;

    /// <summary>
    /// Index of the first selected item. If negative, this value is relative to the item after the last item of the column. Thus -1 designates the last valid item.
    /// </summary>
    /// <value>
    /// Index of the first selected item.
    /// </value>
    public int Start { get { return _firstRowIndexInclusive; } }

    /// <summary>
    /// Gets the number of items per period. This value must be less than or equal to <see cref="LengthOfPeriod"/>.
    /// </summary>
    /// <value>
    /// Number of items per period, a value less than or equal to <see cref="LengthOfPeriod"/>.
    /// </value>
    public int NumberOfItemsPerPeriod
    {
      get { return _numberOfItemsPerPeriod; }
    }

    /// <summary>
    /// Gets the length of one period.
    /// </summary>
    /// <value>
    /// The length of one period.
    /// </value>
    public int LengthOfPeriod
    {
      get
      {
        return _lengthOfPeriod;
      }
    }

    #region Serialization

    /// <summary>
    /// 2016-08-08 initial version.
    /// </summary>

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeriodicRowIndexSegments), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeriodicRowIndexSegments)obj;

        info.AddValue("First", s._firstRowIndexInclusive);
        info.AddValue("PeriodLength", s._lengthOfPeriod);
        info.AddValue("ItemsPerPeriod", s._numberOfItemsPerPeriod);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var first = info.GetInt32("First");
        var period = info.GetInt32("PeriodLength");
        var items = info.GetInt32("ItemsPerPeriod");
        return new PeriodicRowIndexSegments(info, first, period, items);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Deserialization constructor. Initializes a new instance of the <see cref="PeriodicRowIndexSegments"/> class.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
    /// <param name="firstRowIndex">Index of the first selected item.</param>
    /// <param name="lengthOfPeriod">Length of one period.</param>
    /// <param name="itemsPerPeriod">Number of items per period</param>
    protected PeriodicRowIndexSegments(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int firstRowIndex, int lengthOfPeriod, int itemsPerPeriod)
    {
      _firstRowIndexInclusive = firstRowIndex;
      _lengthOfPeriod = lengthOfPeriod;

      _numberOfItemsPerPeriod = itemsPerPeriod;
    }

    public PeriodicRowIndexSegments()
    {
      _firstRowIndexInclusive = 0;
      _lengthOfPeriod = 2;
      _numberOfItemsPerPeriod = 2;
    }

    /// <summary>
    /// Constructor. Initializes a new instance of the <see cref="PeriodicRowIndexSegments"/> class.
    /// </summary>
    /// <param name="firstRowIndex">Index of the first selected item.</param>
    /// <param name="lengthOfPeriod">Length of one period.</param>
    /// <param name="itemsPerPeriod">Number of items per period</param>
    public PeriodicRowIndexSegments(int firstRowIndex, int lengthOfPeriod, int itemsPerPeriod)
    {
      if (!(lengthOfPeriod >= 1))
        throw new ArgumentOutOfRangeException(nameof(lengthOfPeriod), "must be >= 1");

      if (!(itemsPerPeriod >= 1))
        throw new ArgumentOutOfRangeException(nameof(itemsPerPeriod), "must be >= 1");

      if (!(itemsPerPeriod <= lengthOfPeriod))
        throw new ArgumentOutOfRangeException(nameof(itemsPerPeriod), "must be <= " + nameof(lengthOfPeriod));

      _firstRowIndexInclusive = firstRowIndex;
      _lengthOfPeriod = lengthOfPeriod;
      _numberOfItemsPerPeriod = itemsPerPeriod;
    }

    public object Clone()
    {
      return new PeriodicRowIndexSegments
      {
        _firstRowIndexInclusive = _firstRowIndexInclusive,
        _lengthOfPeriod = _lengthOfPeriod,
        _numberOfItemsPerPeriod = _numberOfItemsPerPeriod
      };
    }

    /// <inheritdoc/>
    public IEnumerable<(int start, int endExclusive)> GetSelectedRowIndexSegmentsFromTo(int startIndex, int maxIndex, DataColumnCollection? table, int totalRowCount)
    {
      int start = Math.Max(startIndex, _firstRowIndexInclusive >= 0 ? _firstRowIndexInclusive : _firstRowIndexInclusive + totalRowCount);
      int endExclusive = Math.Min(maxIndex, totalRowCount);

      for (int segmentStart = start; segmentStart < endExclusive; segmentStart += _lengthOfPeriod)
      {
        var segmentEndExclusive = Math.Min(endExclusive, segmentStart + _numberOfItemsPerPeriod);
        yield return (segmentStart, segmentEndExclusive);
      }
    }

    public override int GetHashCode()
    {
      return 13 * _firstRowIndexInclusive.GetHashCode() + 17 * _lengthOfPeriod + 31 * _numberOfItemsPerPeriod.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
      if (obj is PeriodicRowIndexSegments from)
        return _firstRowIndexInclusive == from._firstRowIndexInclusive && _lengthOfPeriod == from._lengthOfPeriod && _numberOfItemsPerPeriod == from._numberOfItemsPerPeriod;
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

  }
}
