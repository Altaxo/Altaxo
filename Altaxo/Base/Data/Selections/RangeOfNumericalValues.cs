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
  public class RangeOfNumericalValues : Main.SuspendableDocumentNodeWithEventArgs, IRowSelection
  {
    private AltaxoVariant _lowerValue;
    private bool _isLowerInclusive;

    private AltaxoVariant _upperValue;
    private bool _isUpperInclusive;

    private IReadableColumnProxy? _columnProxy;

    #region Serialization

    /// <summary>
    /// 2016-09-25 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RangeOfNumericalValues), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RangeOfNumericalValues)obj;

        info.AddValue("LowerValue", (object)s._lowerValue);
        info.AddValue("LowerIsInclusive", s._isLowerInclusive);

        info.AddValue("UpperValue", (object)s._upperValue);
        info.AddValue("UpperIsInclusive", s._isUpperInclusive);

        info.AddValueOrNull("Column", s._columnProxy);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var lower = (AltaxoVariant)info.GetValue("LowerValue", parent);
        var isLowerInclusive = info.GetBoolean("LowerIsInclusive");

        var upper = (AltaxoVariant)info.GetValue("UpperValue", parent);
        var isUpperInclusive = info.GetBoolean("UppperIsInclusive");

        var columnProxy = (IReadableColumnProxy?)info.GetValueOrNull("Column", parent);

        return new RangeOfNumericalValues(info, lower, isLowerInclusive, upper, isUpperInclusive, columnProxy);
      }
    }

    #endregion Serialization

    public RangeOfNumericalValues()
    {
      _lowerValue = 0;
      _isLowerInclusive = true;
      _upperValue = 1;
      _isUpperInclusive = true;
      _columnProxy = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeOfNumericalValues"/> class.
    /// </summary>
    /// <param name="from">Instance to copy the values from.</param>
    public RangeOfNumericalValues(RangeOfNumericalValues from)
    {
      _lowerValue = from._lowerValue;
      _isLowerInclusive = from._isLowerInclusive;
      ;
      _upperValue = from._upperValue;
      _isUpperInclusive = from._isUpperInclusive;

      ChildCloneToMember(ref _columnProxy, from._columnProxy);
    }

    public object Clone()
    {
      return new RangeOfNumericalValues(this);
    }

    /// <summary>
    /// Deserialization constructor. Initializes a new instance of the <see cref="RangeOfNumericalValues"/> class.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
    /// <param name="lower">The lower.</param>
    /// <param name="isLowerInclusive">if set to <c>true</c> [is lower inclusive].</param>
    /// <param name="upper">The upper.</param>
    /// <param name="isUpperInclusive">if set to <c>true</c> [is upper inclusive].</param>
    /// <param name="columnProxy">The column.</param>
    protected RangeOfNumericalValues(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, double lower, bool isLowerInclusive, double upper, bool isUpperInclusive, IReadableColumnProxy? columnProxy)
    {
      _lowerValue = lower;
      _isLowerInclusive = isLowerInclusive;
      _upperValue = upper;
      _isUpperInclusive = isUpperInclusive;
      ChildSetMember(ref _columnProxy, columnProxy);
    }

    public RangeOfNumericalValues(double lower, bool isLowerInclusive, double upper, bool isUpperInclusive, IReadableColumn column)
    {
      _lowerValue = lower;
      _isLowerInclusive = isLowerInclusive;
      _upperValue = upper;
      _isUpperInclusive = isUpperInclusive;
      ChildSetMember(ref _columnProxy, ReadableColumnProxyBase.FromColumn(column));
    }

    /// <inheritdoc/>
    public IEnumerable<(int start, int endExclusive)> GetSelectedRowIndexSegmentsFromTo(int startIndex, int maxIndexExclusive, DataColumnCollection? table, int totalRowCount)
    {
      var column = _columnProxy?.Document();

      if (null == column)
        yield break;

      int endExclusive = Math.Min(maxIndexExclusive, totalRowCount);
      if (column.Count.HasValue)
        endExclusive = Math.Min(endExclusive, column.Count.Value);

      bool weAreInsideSegment = false;
      int indexOfStartOfSegment = 0;

      for (int i = startIndex; i < endExclusive; ++i)
      {
        var x = column[i];

        if (
          column.IsElementEmpty(i) ||
          (_isLowerInclusive && !(x >= _lowerValue)) ||
          (!_isLowerInclusive && !(x > _lowerValue)) ||
          (_isUpperInclusive && !(x <= _upperValue)) ||
          (!_isUpperInclusive && !(x < _upperValue))
          )
        {
          if (weAreInsideSegment)
          {
            yield return (indexOfStartOfSegment, i);
          }
          weAreInsideSegment = false;
          continue;
        }
        else
        {
          // this is a index which should be included
          if (!weAreInsideSegment)
          {
            indexOfStartOfSegment = i;
            weAreInsideSegment = true;
          }
        }
      }

      // yield the last segment
      if (weAreInsideSegment)
      {
        yield return (indexOfStartOfSegment, endExclusive);
      }
    }

    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _columnProxy)
        yield return new DocumentNodeAndName(_columnProxy, () => _columnProxy = null, "Column");
    }

    /// <summary>
    /// Data that define the error in the negative direction.
    /// </summary>
    public IReadableColumn? Column
    {
      get
      {
        return _columnProxy?.Document();
      }
      set
      {
        var oldValue = _columnProxy?.Document();
        if (!object.ReferenceEquals(value, oldValue))
        {
          ChildSetMember(ref _columnProxy, null == value ? null : ReadableColumnProxyBase.FromColumn(value));
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets the name of the column, if it is a data column. Otherwise, <see cref="string.Empty"/> is returned.
    /// </summary>
    /// <value>
    /// The name of the column if it is a data column. Otherwise, <see cref="string.Empty"/>.
    /// </value>
    public string ColumnName
    {
      get
      {
        return _columnProxy?.DocumentPath()?.LastPartOrDefault ?? string.Empty;
      }
    }

    public double LowerValue
    {
      get
      {
        return _lowerValue;
      }
      set
      {
        if (double.IsNaN(value))
          throw new ArgumentOutOfRangeException("Value must not be double.NaN", nameof(value));

        if (!(_lowerValue == value))
        {
          _lowerValue = value;
          EhSelfChanged();
        }
      }
    }

    public double UpperValue
    {
      get
      {
        return _upperValue;
      }
      set
      {
        if (double.IsNaN(value))
          throw new ArgumentOutOfRangeException("Value must not be double.NaN", nameof(value));

        if (!(_upperValue == value))
        {
          _upperValue = value;
          EhSelfChanged();
        }
      }
    }

    public bool IsLowerValueInclusive
    {
      get
      {
        return _isLowerInclusive;
      }
      set
      {
        if (!(_isLowerInclusive == value))
        {
          _isLowerInclusive = value;
          EhSelfChanged();
        }
      }
    }

    public bool IsUpperValueInclusive
    {
      get
      {
        return _isUpperInclusive;
      }
      set
      {
        if (!(_isUpperInclusive == value))
        {
          _isUpperInclusive = value;
          EhSelfChanged();
        }
      }
    }

    /// <inheritdoc/>
    public IEnumerable<ColumnInformationSimple> GetAdditionallyUsedColumns()
    {
      yield return new ColumnInformationSimple(GetType().Name, Column, ColumnName, (c) => Column = c);
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      if (!(_columnProxy is null))
        Report(_columnProxy, this, nameof(Column));
    }
  }
}
