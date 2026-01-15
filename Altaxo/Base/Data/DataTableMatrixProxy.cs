#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using Altaxo.Collections;

namespace Altaxo.Data
{
  /// <summary>
  /// Holds reference to a matrix-like arrangement of data from a <see cref="DataTable"/>. The matrix data consist of 2 or more <see cref="DataColumn"/>s and all or selected data rows.
  /// Furthermore, a row header column and a column header column can deliver corresponding physical values for each matrix row and column, respectively.
  /// </summary>
  public class DataTableMatrixProxy : DataTableMatrixProxyBase
  {

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-07-08 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTableMatrixProxy), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataTableMatrixProxy)obj;
        info.AddValueOrNull("Table", s._dataTable);
        info.AddValue("Group", s._groupNumber);
        info.AddValue("RowHeaderColumn", s._rowHeaderColumn);
        info.AddValueOrNull("ColumnHeaderColumn", s._columnHeaderColumns?.Count > 0 ? s._columnHeaderColumns[0] : null);
        info.AddValue("UseAllAvailableColumnsOfGroup", s._useAllAvailableColumnsOfGroup);
        info.AddValue("UseAllAvailableDataRows", s._useAllAvailableDataRows);

        if (!s._useAllAvailableColumnsOfGroup)
        {
          info.CreateArray("DataColumns", s._dataColumns.Count);
          for (int i = 0; i < s._dataColumns.Count; ++i)
          {
            info.AddValue("e", s._dataColumns[i]);
          }
          info.CommitArray();
        }

        if (!s._useAllAvailableDataRows)
        {
          info.AddValue("DataRows", s._participatingDataRows);
        }
      }

      protected virtual DataTableMatrixProxy SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DataTableMatrixProxy?)o ?? new DataTableMatrixProxy();

        s.ChildSetMember(ref s._dataTable, info.GetValueOrNull<DataTableProxy>("Table", s));
        s._groupNumber = info.GetInt32("Group");
        s.InternalSetRowHeaderColumn((IReadableColumnProxy)info.GetValue("RowHeaderColumn", s));
        s.InternalSetColumnHeaderColumn((IReadableColumnProxy?)info.GetValueOrNull("ColumnHeaderColumn", s));

        s._useAllAvailableColumnsOfGroup = info.GetBoolean("UseAllAvailableColumnsOfGroup");
        s._useAllAvailableDataRows = info.GetBoolean("UseAllAvailableDataRows");

        if (!s._useAllAvailableColumnsOfGroup)
        {
          int count = info.OpenArray();
          s._dataColumns = new List<IReadableColumnProxy>(count);
          for (int i = 0; i < count; i++)
          {
            s.InternalAddDataColumnNoClone((IReadableColumnProxy)info.GetValue("e", s));
          }
          info.CloseArray(count);
        }
        else
        {
          s._dataColumns = new List<IReadableColumnProxy>();
        }

        if (!s._useAllAvailableDataRows)
        {
          s._participatingDataRows = (AscendingIntegerCollection)info.GetValue("DataRows", s);
        }
        else
        {
          s._participatingDataRows = new AscendingIntegerCollection();
        }

        s._participatingDataColumns = new AscendingIntegerCollection(); // this is just to avoid NullExceptions

        s._isDirty = true;

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public override object Clone()
    {
      var result = new DataTableMatrixProxy();
      result.CopyFrom(this);
      return result;
    }

    /// <summary>
    /// Deserialization constructor
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected DataTableMatrixProxy()
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTableMatrixProxy"/> class. The selected collections determine which columns and rows contribute to the matrix, and which
    /// row header column and column header column is used. The group number is determined by the first selected column (or, if no column is selected, by the first column of the data table).
    /// </summary>
    /// <param name="table">The underlying table.</param>
    /// <param name="selectedDataRows">The selected data rows.</param>
    /// <param name="selectedDataColumns">The selected data columns.</param>
    /// <param name="selectedPropertyColumns">The selected property columns.</param>
    /// <exception cref="System.ArgumentNullException">table must not be null.</exception>
    public DataTableMatrixProxy(DataTable table, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns, IAscendingIntegerCollection selectedPropertyColumns)
      : base(table, selectedDataRows, selectedDataColumns, selectedPropertyColumns)
    {

    }

    [Obsolete("This is intended for legacy deserialization (of XYZMeshedColumnPlotData) only.")]
    public static DataTableMatrixProxy CreateEmptyInstance()
    {
      var result = new DataTableMatrixProxy
      {
        _participatingDataColumns = new AscendingIntegerCollection(),
        _participatingDataRows = new AscendingIntegerCollection(),
        _dataColumns = new List<IReadableColumnProxy>(),

        _dataTable = null!
      };
      result.InternalSetRowHeaderColumn(ReadableColumnProxyBase.FromColumn(null));
      result.InternalSetColumnHeaderColumn(ReadableColumnProxyBase.FromColumn(null));

      return result;
    }

    [Obsolete("This is intended for legacy deserialization (of XYZMeshedColumnPlotData) only.")]
    public DataTableMatrixProxy(IReadableColumnProxy xColumn, IReadableColumnProxy yColumn, IReadableColumnProxy[] dataColumns)
      : base(xColumn, yColumn, dataColumns)
    {
    }


    /// <summary>Column that correlate each column of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.</summary>
    [MaybeNull]
    public IReadableColumn ColumnHeaderColumn
    {
      get
      {
        return _columnHeaderColumns.Count == 0 ? null : _columnHeaderColumns[0].Document();
      }
      set
      {
        var oldValue = ColumnHeaderColumn;
        if (!object.ReferenceEquals(oldValue, value))
        {
          InternalSetColumnHeaderColumn(ReadableColumnProxyBase.FromColumn(value));
          _isDirty = true;
        }
      }
    }

  }
}
