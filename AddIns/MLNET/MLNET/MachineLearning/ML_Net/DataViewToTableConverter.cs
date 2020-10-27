#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using Altaxo.Data;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Altaxo.MachineLearning.ML_Net
{
  /// <summary>
  /// Contains static methods for storing the contents of an <see cref="IDataView"/> into a <see cref="Altaxo.Data.DataTable"/>.
  /// </summary>
  public static class DataViewToTableConverter
  {
    /// <summary>
    /// Sets the table contents of the <see cref="Altaxo.Data.DataTable"/> to the contents of the <see cref="IDataView"/>.
    /// ATTENTION: all existing data in the table will be deleted prior to storing the contents of the <see cref="IDataView"/>!
    /// </summary>
    /// <param name="table">The table to store the data to.</param>
    /// <param name="originalTable">The table that was used to create the <paramref name="view"/>.
    /// This table is used copy the original data from, in order to prevent loss of precision (many methods of ML.NET use single precision only).
    /// The table may or may not be equal to <paramref name="table"/>.
    /// </param>
    /// <param name="view">The source data view.</param>
    public static void SetTableContentsTo(this Altaxo.Data.DataTable table, IDataView view, Altaxo.Data.DataTable originalTable, string keyColumnName)
    {
      if (!object.ReferenceEquals(table, originalTable))
      {
        table.CopyDataAndPropertyColumnsFrom(originalTable);
      }

      var schema = view.Schema;
      var col = table.DataColumns;


      // Determine all columns that are common between originalTable and view
      var commonColumnNames = new List<string>();
      for (int i = 0; i < schema.Count; ++i)
      {
        if (originalTable.DataColumns.Contains(schema[i].Name))
          commonColumnNames.Add(schema[i].Name);
      }

      var keyColumnOriginal = table.DataColumns[keyColumnName];
      var cursor = view.GetRowCursor(schema, null);

      Action act = new Action(() => { }); // collects the setter actions of all columns

      DataViewSchema.Column? keyColumnView = default;
      for (int i = 0; i < schema.Count; ++i)
      {
        var schemaCol = schema[i];
        var columnName = schemaCol.Name;
        if (columnName == keyColumnName)
          keyColumnView = schemaCol;
      }

      if (!keyColumnView.HasValue)
      {
        throw new Exception($"Key column with name {keyColumnName} is not found in the data view!");
      }

      var cursorPosToOriginalPosDictionary = new List<int>();

      // now we have to create a dictionary which maps cursor.Positions onto positions in the original table
      if (keyColumnView.Value.Type == DateTimeDataViewType.Instance)
      {
        if (!(keyColumnOriginal is DateTimeColumn dtc))
          throw new Exception($"Unexpected type of column in the original data table: {keyColumnOriginal?.GetType()}. Expected was {typeof(DateTimeColumn)}");
        var dict = new Dictionary<DateTime, int>();

        for (int i = 0; i < dtc.Count; ++i)
        {
          if (!dtc.IsElementEmpty(i))
          {
            if (dict.ContainsKey(dtc[i]))
              throw new Exception($"The keys in the key columns are not unique. Duplicated detected at positions {i} and {dict[dtc[i]]}");
            dict.Add(dtc[i], i);
          }
        }
        var dateGetter = cursor.GetGetter<DateTime>(keyColumnView.Value);
        // now use the collected setters to set the data for each row
        while (cursor.MoveNext())
        {
          DateTime d = default;
          dateGetter(ref d);
          // Find d in keyColumnOriginal
          if (dict.TryGetValue(d, out var idx))
            cursorPosToOriginalPosDictionary.Add(idx);
          else
            cursorPosToOriginalPosDictionary.Add(-1);
        }
      }
      else if (keyColumnView.Value.Type == TextDataViewType.Instance)
      {
        if (!(keyColumnOriginal is TextColumn tc))
          throw new Exception($"Unexpected type of column in the original data table: {keyColumnOriginal?.GetType()}. Expected was {typeof(TextColumn)}");
        var dict = new Dictionary<string, int>();
        for (int i = 0; i < tc.Count; ++i)
        {
          if (!tc.IsElementEmpty(i))
          {
            if (dict.ContainsKey(tc[i]!))
              throw new Exception($"The keys in the key columns are not unique. Duplicates detected at positions {i} and {dict[tc[i]!]}");
            dict.Add(tc[i]!, i);
          }
        }
        var textGetter = cursor.GetGetter<ReadOnlyMemory<char>>(keyColumnView.Value);
        // now use the collected setters to set the data for each row
        while (cursor.MoveNext())
        {
          ReadOnlyMemory<char> d = default;
          textGetter(ref d);
          // Find d in keyColumnOriginal
          if (dict.TryGetValue(d.ToString(), out var idx))
            cursorPosToOriginalPosDictionary.Add(idx);
          else
            cursorPosToOriginalPosDictionary.Add(-1);
        }
      }
      else
      {
        throw new NotImplementedException($"Sorry - key column of type {keyColumnView?.Type} is not yet implemented!");
      }

      int FuncCursorToOriginal(long cursorPos)
      {
        return cursorPos < 0 || cursorPos >= cursorPosToOriginalPosDictionary.Count ? -1 : cursorPosToOriginalPosDictionary[(int)cursorPos];
      }

      cursor?.Dispose(); // dispose the cursor used to build the index and
      cursor = view.GetRowCursor(schema, null); // get a new cursor

      for (int i = 0; i < schema.Count; ++i)
      {
        var schemaCol = schema[i];
        var columnName = schemaCol.Name;

        if (table.DataColumns.Contains(columnName))
          continue;

        var type = schemaCol.Type;

        if (type == NumberDataViewType.Double)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => DoubleGetter(cursor, c, cursor.GetGetter<double>(schemaCol), FuncCursorToOriginal));
        }
        else if (type == NumberDataViewType.Single)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => SingleGetter(cursor, c, cursor.GetGetter<float>(schemaCol), FuncCursorToOriginal));
        }
        else if (type == NumberDataViewType.Int64)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => Int64Getter(cursor, c, cursor.GetGetter<Int64>(schemaCol), FuncCursorToOriginal));
        }
        else if (type == NumberDataViewType.Int32)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => Int32Getter(cursor, c, cursor.GetGetter<Int32>(schemaCol), FuncCursorToOriginal));
        }
        else if (type == NumberDataViewType.Int16)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => Int16Getter(cursor, c, cursor.GetGetter<Int16>(schemaCol), FuncCursorToOriginal));
        }
        else if (type == NumberDataViewType.SByte)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => SByteGetter(cursor, c, cursor.GetGetter<SByte>(schemaCol), FuncCursorToOriginal));
        }
        else if (type == NumberDataViewType.UInt64)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => UInt64Getter(cursor, c, cursor.GetGetter<UInt64>(schemaCol), FuncCursorToOriginal));
        }
        else if (type == NumberDataViewType.UInt32)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => UInt32Getter(cursor, c, cursor.GetGetter<UInt32>(schemaCol), FuncCursorToOriginal));
        }
        else if (type == NumberDataViewType.UInt16)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => UInt16Getter(cursor, c, cursor.GetGetter<UInt16>(schemaCol), FuncCursorToOriginal));
        }
        else if (type == NumberDataViewType.Byte)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => ByteGetter(cursor, c, cursor.GetGetter<Byte>(schemaCol), FuncCursorToOriginal));
        }
        else if (type == BooleanDataViewType.Instance)
        {
          var c = new BooleanColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => BooleanGetter(cursor, c, cursor.GetGetter<bool>(schemaCol), FuncCursorToOriginal));
        }
        else if (type == DateTimeDataViewType.Instance)
        {
          var c = new DateTimeColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => DateTimeGetter(cursor, c, cursor.GetGetter<DateTime>(schemaCol), FuncCursorToOriginal));
        }
        else if (type == TextDataViewType.Instance)
        {
          var c = new TextColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => TextGetter(cursor, c, cursor.GetGetter<ReadOnlyMemory<char>>(schemaCol), FuncCursorToOriginal));
        }
        else if (type is KeyDataViewType keyDataViewType)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          try
          {
            VBuffer<float> keyValues = default;
            schemaCol.GetKeyValues<float>(ref keyValues);
            act += new Action(() => KeySingleGetter(cursor, c, keyValues.DenseValues().ToArray(), cursor.GetGetter<UInt32>(schemaCol), FuncCursorToOriginal));
          }
          catch (Exception)
          {
            act += new Action(() => UInt32Minus1Getter(cursor, c, cursor.GetGetter<UInt32>(schemaCol), FuncCursorToOriginal));
          }
        }
        else if (type is VectorDataViewType vectorDataViewType)
        {
          if (!vectorDataViewType.IsKnownSize)
            throw new NotImplementedException();
          if (!(1 == vectorDataViewType.Dimensions.Length))
            throw new NotImplementedException();
          if (vectorDataViewType.ItemType == NumberDataViewType.Single)
          {
            var c = Enumerable.Range(0, vectorDataViewType.Dimensions[0])
                    .Select((k) => { var cc = new DoubleColumn(); col.Add(cc, columnName + k.ToString(), ColumnKind.V, 0); return cc; }).ToArray();
            act += new Action(() => Vector1DSingleGetter(cursor, c, cursor.GetGetter<VBuffer<float>>(schemaCol), FuncCursorToOriginal));
          }
          else if (vectorDataViewType.ItemType == NumberDataViewType.Double)
          {
            var c = Enumerable.Range(0, vectorDataViewType.Dimensions[0])
                    .Select((k) => { var cc = new DoubleColumn(); col.Add(cc, columnName + k.ToString(), ColumnKind.V, 0); return cc; }).ToArray();
            act += new Action(() => Vector1DDoubleGetter(cursor, c, cursor.GetGetter<VBuffer<double>>(schemaCol), FuncCursorToOriginal));
          }
          else
            throw new NotImplementedException();
        }

        else
        {
          throw new NotImplementedException($"Sorry - setter for column type {type} not yet implemented!");
        }
      }


      // now use the collected setters to set the data for each row
      while (cursor.MoveNext())
        act();
    }


    /// <summary>
    /// Sets the table contents of the <see cref="Altaxo.Data.DataTable"/> to the contents of the <see cref="IDataView"/>.
    /// ATTENTION: all existing data in the table will be deleted prior to storing the contents of the <see cref="IDataView"/>!
    /// </summary>
    /// <param name="table">The table to store the data into.</param>
    /// <param name="view">The source data view.</param>
    public static void SetTableContentsTo(this Altaxo.Data.DataTable table, IDataView view)
    {
      var col = table.DataColumns;
      col.RemoveColumnsAll();
      table.PropCols.RemoveColumnsAll();

      var schema = view.Schema;
      var cursor = view.GetRowCursor(schema, null);

      Action act = new Action(() => { }); // collects the setter actions of all columns

      for (int i = 0; i < schema.Count; ++i)
      {
        var schemaCol = schema[i];
        var columnName = schemaCol.Name;

        var type = schemaCol.Type;

        if (type == NumberDataViewType.Double)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => DoubleGetter(cursor, c, cursor.GetGetter<double>(schemaCol)));
        }
        else if (type == NumberDataViewType.Single)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => SingleGetter(cursor, c, cursor.GetGetter<float>(schemaCol)));
        }
        else if (type == NumberDataViewType.Int64)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => Int64Getter(cursor, c, cursor.GetGetter<Int64>(schemaCol)));
        }
        else if (type == NumberDataViewType.Int32)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => Int32Getter(cursor, c, cursor.GetGetter<Int32>(schemaCol)));
        }
        else if (type == NumberDataViewType.Int16)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => Int16Getter(cursor, c, cursor.GetGetter<Int16>(schemaCol)));
        }
        else if (type == NumberDataViewType.SByte)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => SByteGetter(cursor, c, cursor.GetGetter<SByte>(schemaCol)));
        }
        else if (type == NumberDataViewType.UInt64)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => UInt64Getter(cursor, c, cursor.GetGetter<UInt64>(schemaCol)));
        }
        else if (type == NumberDataViewType.UInt32)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => UInt32Getter(cursor, c, cursor.GetGetter<UInt32>(schemaCol)));
        }
        else if (type == NumberDataViewType.UInt16)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => UInt16Getter(cursor, c, cursor.GetGetter<UInt16>(schemaCol)));
        }
        else if (type == NumberDataViewType.Byte)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => ByteGetter(cursor, c, cursor.GetGetter<Byte>(schemaCol)));
        }
        else if (type == BooleanDataViewType.Instance)
        {
          var c = new BooleanColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => BooleanGetter(cursor, c, cursor.GetGetter<bool>(schemaCol)));
        }
        else if (type == DateTimeDataViewType.Instance)
        {
          var c = new DateTimeColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => DateTimeGetter(cursor, c, cursor.GetGetter<DateTime>(schemaCol)));
        }
        else if (type == TextDataViewType.Instance)
        {
          var c = new TextColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          act += new Action(() => TextGetter(cursor, c, cursor.GetGetter<ReadOnlyMemory<char>>(schemaCol)));
        }
        else if (type is KeyDataViewType keyDataViewType)
        {
          var c = new DoubleColumn();
          col.Add(c, columnName, ColumnKind.V, 0);
          try
          {
            VBuffer<float> keyValues = default;
            schemaCol.GetKeyValues<float>(ref keyValues);
            act += new Action(() => KeySingleGetter(cursor, c, keyValues.DenseValues().ToArray(), cursor.GetGetter<UInt32>(schemaCol)));
          }
          catch (Exception)
          {
            act += new Action(() => UInt32Minus1Getter(cursor, c, cursor.GetGetter<UInt32>(schemaCol)));
          }
        }
        else if (type is VectorDataViewType vectorDataViewType)
        {
          if (!vectorDataViewType.IsKnownSize)
            throw new NotImplementedException();
          if (!(1 == vectorDataViewType.Dimensions.Length))
            throw new NotImplementedException();
          if (vectorDataViewType.ItemType == NumberDataViewType.Single)
          {
            var c = Enumerable.Range(0, vectorDataViewType.Dimensions[0])
                    .Select((k) => { var cc = new DoubleColumn(); col.Add(cc, columnName + k.ToString(), ColumnKind.V, 0); return cc; }).ToArray();
            act += new Action(() => Vector1DSingleGetter(cursor, c, cursor.GetGetter<VBuffer<float>>(schemaCol)));
          }
          else if (vectorDataViewType.ItemType == NumberDataViewType.Double)
          {
            var c = Enumerable.Range(0, vectorDataViewType.Dimensions[0])
                    .Select((k) => { var cc = new DoubleColumn(); col.Add(cc, columnName + k.ToString(), ColumnKind.V, 0); return cc; }).ToArray();
            act += new Action(() => Vector1DDoubleGetter(cursor, c, cursor.GetGetter<VBuffer<double>>(schemaCol)));
          }
          else
            throw new NotImplementedException();
        }
        else
        {
          throw new NotImplementedException();
        }
      }


      // now use the collected setters to set the data for each row
      while (cursor.MoveNext())
        act();
    }

    private static void DoubleGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<double> getter)
    {
      double value = default;
      getter(ref value);
      c[(int)cursor.Position] = value;
    }

    private static void DoubleGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<double> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        double value = default;
        getter(ref value);
        c[idx] = value;
      }
    }

    private static void SingleGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<float> getter)
    {
      float value = default;
      getter(ref value);
      c[(int)cursor.Position] = value;
    }

    private static void SingleGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<float> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        float value = default;
        getter(ref value);
        c[idx] = value;
      }
    }

    private static void Int64Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<Int64> getter)
    {
      Int64 value = default;
      getter(ref value);
      c[(int)cursor.Position] = value;
    }

    private static void Int64Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<Int64> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        Int64 value = default;
        getter(ref value);
        c[idx] = value;
      }
    }

    private static void Int32Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<Int32> getter)
    {
      Int32 value = default;
      getter(ref value);
      c[(int)cursor.Position] = value;
    }

    private static void Int32Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<Int32> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        Int32 value = default;
        getter(ref value);
        c[idx] = value;
      }
    }

    private static void Int16Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<Int16> getter)
    {
      Int16 value = default;
      getter(ref value);
      c[(int)cursor.Position] = value;
    }

    private static void Int16Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<Int16> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        Int16 value = default;
        getter(ref value);
        c[idx] = value;
      }
    }

    private static void SByteGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<SByte> getter)
    {
      SByte value = default;
      getter(ref value);
      c[(int)cursor.Position] = value;
    }

    private static void SByteGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<SByte> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        SByte value = default;
        getter(ref value);
        c[idx] = value;
      }
    }

    private static void UInt64Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<UInt64> getter)
    {
      UInt64 value = default;
      getter(ref value);
      c[(int)cursor.Position] = value;
    }

    private static void UInt64Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<UInt64> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        UInt64 value = default;
        getter(ref value);
        c[idx] = value;
      }
    }

    private static void UInt32Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<UInt32> getter)
    {
      UInt32 value = default;
      getter(ref value);
      c[(int)cursor.Position] = value;
    }

    private static void UInt32Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<UInt32> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        UInt32 value = default;
        getter(ref value);
        c[idx] = value;
      }
    }

    private static void UInt32Minus1Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<UInt32> getter)
    {
      UInt32 value = default;
      getter(ref value);
      c[(int)cursor.Position] = value - 1.0;
    }

    private static void UInt32Minus1Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<UInt32> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        UInt32 value = default;
        getter(ref value);
        c[idx] = value - 1.0;
      }
    }

    private static void UInt16Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<UInt16> getter)
    {
      UInt16 value = default;
      getter(ref value);
      c[(int)cursor.Position] = value;
    }

    private static void UInt16Getter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<UInt16> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        UInt16 value = default;
        getter(ref value);
        c[idx] = value;
      }
    }

    private static void ByteGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<Byte> getter)
    {
      Byte value = default;
      getter(ref value);
      c[(int)cursor.Position] = value;
    }

    private static void ByteGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, ValueGetter<Byte> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        Byte value = default;
        getter(ref value);
        c[idx] = value;
      }
    }

    private static void BooleanGetter(DataViewRowCursor cursor, Altaxo.Data.BooleanColumn c, ValueGetter<bool> getter)
    {
      bool value = default;
      getter(ref value);
      c[(int)cursor.Position] = value;
    }

    private static void BooleanGetter(DataViewRowCursor cursor, Altaxo.Data.BooleanColumn c, ValueGetter<bool> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        bool value = default;
        getter(ref value);
        c[idx] = value;
      }
    }

    private static void DateTimeGetter(DataViewRowCursor cursor, Altaxo.Data.DateTimeColumn c, ValueGetter<DateTime> getter)
    {
      DateTime value = default;
      getter(ref value);
      c[(int)cursor.Position] = value;
    }

    private static void DateTimeGetter(DataViewRowCursor cursor, Altaxo.Data.DateTimeColumn c, ValueGetter<DateTime> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        DateTime value = default;
        getter(ref value);
        c[idx] = value;
      }
    }

    private static void TextGetter(DataViewRowCursor cursor, Altaxo.Data.TextColumn c, ValueGetter<ReadOnlyMemory<char>> getter)
    {
      ReadOnlyMemory<char> value = default;
      getter(ref value);
      c[(int)cursor.Position] = value.ToString();
    }

    private static void TextGetter(DataViewRowCursor cursor, Altaxo.Data.TextColumn c, ValueGetter<ReadOnlyMemory<char>> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        ReadOnlyMemory<char> value = default;
        getter(ref value);
        c[idx] = value.ToString();
      }
    }

    private static void Vector1DSingleGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn[] c, ValueGetter<VBuffer<float>> getter)
    {
      VBuffer<float> value = default;
      getter(ref value);

      int i = 0;
      foreach (var v in value.DenseValues())
      {
        c[i][(int)cursor.Position] = v;
        ++i;
      }
    }

    private static void Vector1DSingleGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn[] c, ValueGetter<VBuffer<float>> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        VBuffer<float> value = default;
        getter(ref value);
        int i = 0;
        foreach (var v in value.DenseValues())
        {
          c[i][idx] = v;
          ++i;
        }
      }
    }

    private static void Vector1DDoubleGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn[] c, ValueGetter<VBuffer<double>> getter)
    {
      VBuffer<double> value = default;
      getter(ref value);

      int i = 0;
      foreach (var v in value.DenseValues())
      {
        c[i][(int)cursor.Position] = v;
        ++i;
      }
    }

    private static void Vector1DDoubleGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn[] c, ValueGetter<VBuffer<double>> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        VBuffer<double> value = default;
        getter(ref value);
        int i = 0;
        foreach (var v in value.DenseValues())
        {
          c[i][idx] = v;
          ++i;
        }
      }
    }

    private static void KeySingleGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, float[] keys, ValueGetter<UInt32> getter)
    {
      UInt32 value = default;
      getter(ref value);
      c[(int)cursor.Position] = keys[value - 1];
    }

    private static void KeySingleGetter(DataViewRowCursor cursor, Altaxo.Data.DoubleColumn c, float[] keys, ValueGetter<UInt32> getter, Func<long, int> positionToIndex)
    {
      var idx = positionToIndex(cursor.Position);
      if (idx >= 0)
      {
        UInt32 value = default;
        getter(ref value);
        c[idx] = keys[value - 1];
      }
    }

  }
}
