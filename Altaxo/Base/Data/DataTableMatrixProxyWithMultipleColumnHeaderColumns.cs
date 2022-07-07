#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2022 Dr. Dirk Lellinger
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
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;

namespace Altaxo.Data
{
  /// <summary>
  /// Holds reference to a matrix-like arrangement of data from a <see cref="DataTable"/>. The matrix data consist of 2 or more <see cref="DataColumn"/>s and all or selected data rows.
  /// Furthermore, a row header column and multiple column header columns can deliver corresponding physical values for each matrix row and column, respectively.
  /// This class is intended e.g. to hold spectral data together with one or multiple target variables for chemometrical analysis.
  /// </summary>
  public class DataTableMatrixProxyWithMultipleColumnHeaderColumns : DataTableMatrixProxyBase
  {
    #region Serialization

    #region Version 0

    /// <summary>
    /// 2022-06-25 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTableMatrixProxyWithMultipleColumnHeaderColumns), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataTableMatrixProxyWithMultipleColumnHeaderColumns)obj;
        info.AddValueOrNull("Table", s._dataTable);
        info.AddValue("Group", s._groupNumber);
        info.AddValue("RowHeaderColumn", s._rowHeaderColumn);
        {
          info.CreateArray("ColumnHeaderColumns", s._columnHeaderColumns.Count);
          for (int i = 0; i < s._columnHeaderColumns.Count; ++i)
          {
            info.AddValue("e", s._columnHeaderColumns[i]);
          }
          info.CommitArray();
        }

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

      protected virtual DataTableMatrixProxyBase SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DataTableMatrixProxyWithMultipleColumnHeaderColumns?)o ?? new DataTableMatrixProxyWithMultipleColumnHeaderColumns();

        s.ChildSetMember(ref s._dataTable, info.GetValueOrNull<DataTableProxy>("Table", s));
        s._groupNumber = info.GetInt32("Group");
        s.InternalSetRowHeaderColumn((IReadableColumnProxy)info.GetValue("RowHeaderColumn", s));
        s.InternalSetColumnHeaderColumn((IReadableColumnProxy)info.GetValue("ColumnHeaderColumn", s));

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
    /// Deserialization constructor
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected DataTableMatrixProxyWithMultipleColumnHeaderColumns()
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.


    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public override object Clone()
    {
      var result = new DataTableMatrixProxyWithMultipleColumnHeaderColumns();
      result.CopyFrom(this);
      return result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTableMatrixProxyBase"/> class. The selected collections determine which columns and rows contribute to the matrix, and which
    /// row header column and column header column is used. The group number is determined by the first selected column (or, if no column is selected, by the first column of the data table).
    /// </summary>
    /// <param name="table">The underlying table.</param>
    /// <param name="selectedDataRows">The selected data rows.</param>
    /// <param name="selectedDataColumns">The selected data columns.</param>
    /// <param name="selectedPropertyColumns">The selected property columns.</param>
    /// <exception cref="System.ArgumentNullException">table must not be null.</exception>
    public DataTableMatrixProxyWithMultipleColumnHeaderColumns(DataTable table, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns, IAscendingIntegerCollection selectedPropertyColumns)
    {
      if (table is null)
        throw new ArgumentNullException(nameof(table));

      _dataTable = new DataTableProxy(table)
      {
        ParentObject = this
      };

      var converter = new DataTableToMatrixConverter(table)
      {
        SelectedDataRows = selectedDataRows,
        SelectedDataColumns = selectedDataColumns,
        SelectedPropertyColumns = selectedPropertyColumns,
        ReplacementValueForNaNMatrixElements = 0,
        ReplacementValueForInfiniteMatrixElements = 0,
        MatrixGenerator = DoubleMatrixAsNullDevice.GetMatrix // the data are not needed, thus we send it into a NullDevice
      };

      converter.Execute();

      _groupNumber = converter.DataColumnsGroupNumber;
      _useAllAvailableColumnsOfGroup = converter.AreAllAvailableColumnsOfGroupIncluded();
      _useAllAvailableDataRows = converter.AreAllAvailableRowsIncluded();

      _rowHeaderColumn = ReadableColumnProxyBase.FromColumn(converter.RowHeaderColumn);
      _rowHeaderColumn.ParentObject = this;

      _columnHeaderColumns = new List<IReadableColumnProxy>();
      foreach (var colHeaderCol in converter.ColumnHeaderColumns)
      {
        var columnHeaderColumnProxy = ReadableColumnProxyBase.FromColumn(colHeaderCol);

        columnHeaderColumnProxy.ParentObject = this;
        _columnHeaderColumns.Add(columnHeaderColumnProxy);
      }

      _dataColumns = new List<IReadableColumnProxy>();
      _participatingDataColumns = new AscendingIntegerCollection(converter.GetParticipatingDataColumns());
      for (int i = 0; i < _participatingDataColumns.Count; i++)
      {
        _dataColumns.Add(ReadableColumnProxyBase.FromColumn(table.DataColumns[_participatingDataColumns[i]]));

        // set the event chain
        _dataColumns[i].ParentObject = this;
      }

      _participatingDataRows = new AscendingIntegerCollection(converter.GetParticipatingDataRows());
    }


    /// <summary>
    /// Gets the number of column header columns.
    /// </summary>
   
    public int ColumnHeaderColumnsCount => _columnHeaderColumns.Count;

    /// <summary>Column that correlate each column of the resulting matrix to a corresponding physical value. This value can be used for instance for calculating the x- or y- position in the coordinate system.</summary>
    public IReadableColumnProxy GetColumnHeaderColumn(int idx)
    {
      if(idx< 0 || idx >= _columnHeaderColumns.Count)
        throw new ArgumentOutOfRangeException(nameof(idx));

      return _columnHeaderColumns[idx];
    }

    /// <summary>
    /// Gets a wrapper vector around the column header data.
    /// </summary>
    /// <returns>Wrapper vector around the column header data. Each element of this vector corresponds to the column with the same index of the matrix.</returns>
    public IROVector<double> GetColumnHeaderWrapper(int idx)
    {
      if (idx < 0 || idx >= _columnHeaderColumns.Count)
        throw new ArgumentOutOfRangeException(nameof(idx));

      if (_columnHeaderColumns[idx] is { } chcProxy && chcProxy.Document() is { } columnHeaderColumn)
        return new HeaderColumnWrapper(columnHeaderColumn, _participatingDataColumns);
      else
        throw new InvalidOperationException($"Header column [{idx}] is null. Thus a wrapper could not be created.");
    }

  }
}
