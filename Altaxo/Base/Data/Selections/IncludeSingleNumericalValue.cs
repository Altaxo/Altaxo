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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Main;

namespace Altaxo.Data.Selections
{
  public class IncludeSingleNumericalValue : Main.SuspendableDocumentNodeWithEventArgs, IRowSelection
  {
    private AltaxoVariant _value;
    private IReadableColumnProxy _columnProxy;

    #region Serialization

    /// <summary>
    /// 2017-08-08 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(IncludeSingleNumericalValue), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (IncludeSingleNumericalValue)obj;

        info.AddValue("Value", (object)s._value);
        info.AddValue("Column", s._columnProxy);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var value = (AltaxoVariant)info.GetValue("Value", parent);
        var columnProxy = (IReadableColumnProxy)info.GetValue("Column", parent);

        return new IncludeSingleNumericalValue(info, value, columnProxy);
      }
    }

    #endregion Serialization

    public IncludeSingleNumericalValue()
    {
      _value = 0;
      _columnProxy = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncludeSingleNumericalValue"/> class.
    /// </summary>
    /// <param name="from">Instance to copy the values from.</param>
    public IncludeSingleNumericalValue(IncludeSingleNumericalValue from)
    {
      _value = from._value;
      ChildCloneToMember(ref _columnProxy, from._columnProxy);
    }

    public object Clone()
    {
      return new IncludeSingleNumericalValue(this);
    }

    /// <summary>
    /// Deserialization constructor. Initializes a new instance of the <see cref="IncludeSingleNumericalValue"/> class.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
    /// <param name="value">The numerical value.</param>
    /// <param name="columnProxy">The column.</param>
    protected IncludeSingleNumericalValue(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, double value, IReadableColumnProxy columnProxy)
    {
      _value = value;
      ChildSetMember(ref _columnProxy, columnProxy);
    }

    public IncludeSingleNumericalValue(double value, IReadableColumn column)
    {
      _value = value;
      ChildSetMember(ref _columnProxy, ReadableColumnProxyBase.FromColumn(column));
    }

    /// <inheritdoc/>
    public IEnumerable<(int start, int endExclusive)> GetSelectedRowIndexSegmentsFromTo(int startIndex, int maxIndexExclusive, DataColumnCollection table, int totalRowCount)
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
          (!(x == _value))
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
    public IReadableColumn Column
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
    /// Gets the name of the column, if it is a data column. Otherwise, null is returned.
    /// </summary>
    /// <value>
    /// The name of the column if it is a data column. Otherwise, null.
    /// </value>
    public string ColumnName
    {
      get
      {
        return _columnProxy?.DocumentPath()?.LastPartOrDefault;
      }
    }

    public double Value
    {
      get
      {
        return _value;
      }
      set
      {
        if (double.IsNaN(value))
          throw new ArgumentOutOfRangeException("Value must not be double.NaN", nameof(value));

        if (!(_value == value))
        {
          _value = value;
          EhSelfChanged();
        }
      }
    }

    /// <inheritdoc/>
    public IEnumerable<(
      string ColumnLabel, // Column label
      IReadableColumn Column, // the column as it was at the time of this call
      string ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns()
    {
      yield return (GetType().Name, Column, ColumnName, (c) => Column = c);
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      Report(_columnProxy, this, nameof(Column));
    }
  }
}
