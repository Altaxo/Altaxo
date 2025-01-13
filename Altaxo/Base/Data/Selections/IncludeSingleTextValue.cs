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
  public class IncludeSingleTextValue : Main.SuspendableDocumentNodeWithEventArgs, IRowSelection
  {
    private string _value;
    private bool _ignoreCase;
    private IReadableColumnProxy? _columnProxy;

    #region Serialization

    /// <summary>
    /// 2017-08-13 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(IncludeSingleTextValue), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (IncludeSingleTextValue)obj;

        info.AddValue("Value", s._value);
        info.AddValue("IgnoreCase", s._ignoreCase);
        info.AddValueOrNull("Column", s._columnProxy);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var value = info.GetString("Value");
        var ignoreCase = info.GetBoolean("IgnoreCase");
        var columnProxy = (IReadableColumnProxy?)info.GetValueOrNull("Column", parent);

        return new IncludeSingleTextValue(info, value, ignoreCase, columnProxy);
      }
    }

    #endregion Serialization

    public IncludeSingleTextValue()
    {
      _value = string.Empty;
      _columnProxy = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncludeSingleTextValue"/> class.
    /// </summary>
    /// <param name="from">Instance to copy the values from.</param>
    public IncludeSingleTextValue(IncludeSingleTextValue from)
    {
      _value = from._value;
      _ignoreCase = from._ignoreCase;
      ChildCloneToMember(ref _columnProxy, from._columnProxy);
    }

    public object Clone()
    {
      return new IncludeSingleTextValue(this);
    }

    /// <summary>
    /// Deserialization constructor. Initializes a new instance of the <see cref="IncludeSingleTextValue"/> class.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
    /// <param name="value">The numerical value.</param>
    /// <param name="ignoreCase">If true, string comparison is done case-insensitive.</param>
    /// <param name="columnProxy">The column.</param>
    protected IncludeSingleTextValue(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, string value, bool ignoreCase, IReadableColumnProxy? columnProxy)
    {
      _value = value;
      _ignoreCase = ignoreCase;
      ChildSetMember(ref _columnProxy, columnProxy);
    }

    public IncludeSingleTextValue(string value, bool ignoreCase, IReadableColumn column)
    {
      _value = value;
      _ignoreCase = ignoreCase;
      ChildSetMember(ref _columnProxy, ReadableColumnProxyBase.FromColumn(column));
    }

    /// <inheritdoc/>
    public IEnumerable<(int start, int endExclusive)> GetSelectedRowIndexSegmentsFromTo(int startIndex, int maxIndexExclusive, DataColumnCollection? table, int totalRowCount)
    {
      var column = _columnProxy?.Document();

      if (column is null)
        yield break;

      int endExclusive = Math.Min(maxIndexExclusive, totalRowCount);
      if (column.Count.HasValue)
        endExclusive = Math.Min(endExclusive, column.Count.Value);

      bool weAreInsideSegment = false;
      int indexOfStartOfSegment = 0;

      string compareString = _ignoreCase ? (_value ?? string.Empty).ToLowerInvariant() : (_value ?? string.Empty);

      for (int i = startIndex; i < endExclusive; ++i)
      {
        var x = column[i].ToString();
        if (_ignoreCase)
          x = x.ToLowerInvariant();

        if (
          column.IsElementEmpty(i) ||
          (!(x == compareString))
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
      if (_columnProxy is not null)
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
          ChildSetMember(ref _columnProxy, value is null ? null : ReadableColumnProxyBase.FromColumn(value));
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

    public string Value
    {
      get
      {
        return _value;
      }
      set
      {
        if (!(_value == value))
        {
          _value = value;
          EhSelfChanged();
        }
      }
    }

    public bool IgnoreCase
    {
      get
      {
        return _ignoreCase;
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

    /// <inheritdoc/>
    public IEnumerable<IRowSelection>? ChildNodes => null;

    /// <inheritdoc/>
    public bool Equals(IRowSelection? rowSel)
    {
      return
        rowSel is IncludeSingleTextValue other &&
        this._value == other._value &&
        this._ignoreCase == other._ignoreCase &&
        object.ReferenceEquals(this._columnProxy?.Document(), other._columnProxy?.Document());
    }

  }
}
