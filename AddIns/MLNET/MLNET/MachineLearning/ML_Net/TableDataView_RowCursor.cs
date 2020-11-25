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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Altaxo.MachineLearning.ML_Net
{
  public partial class TableDataView
  {
    public partial class RowCursor : DataViewRowCursor
    {
      bool _isDisposed;
      TableDataView _dataView;
      int _position;
      int[]? _randomIndices;

      /// <summary>
      /// This is incremented when the underlying contents changes, giving clients a way to detect change. It should be
      /// -1 when the object is in a state where values cannot be fetched. In particular, for an <see cref="T:Microsoft.ML.DataViewRowCursor" />,
      /// this will be before <see cref="M:Microsoft.ML.DataViewRowCursor.MoveNext" /> if ever called for the first time, or after the first time
      /// <see cref="M:Microsoft.ML.DataViewRowCursor.MoveNext" /> is called and returns <see langword="false" />.
      /// Note that this position is not position within the underlying data, but position of this cursor only. If
      /// one, for example, opened a set of parallel streaming cursors, or a shuffled cursor, each such cursor's first
      /// valid entry would always have position 0.
      /// </summary>
      public override long Position => _position;

      /// <summary>
      /// Gets the index of the current row in the data table.
      /// If the data is not shuffled, this is identical to <see cref="Position"/>.
      /// This property is used by the value getters to fetch the data from the underlying data table.
      /// </summary>
      /// <value>
      /// The index of the current row.
      /// </value>
      internal int CurrentRowIndex
      {
        get
        {
          return _randomIndices is null
            ? _position
            : (_position >= 0 && _position < _randomIndices.Length) ? _randomIndices[_position] : _position;
        }
      }

      /// <summary>
      /// The value getters for each column.
      /// </summary>
      private readonly Delegate[] _getters;

      public RowCursor(TableDataView parent, IEnumerable<DataViewSchema.Column> columnsNeeded, Random? rand)
      {
        _dataView = parent;
        Schema = parent.Schema;
        _position = -1;

        if (rand is null)
        {
          _randomIndices = null;
        }
        else
        {
          _randomIndices = new int[parent._rowCount];
          // shuffle the indices (Durstenfeld algorithm, inside-out)
          for (int i = 0; i < _randomIndices.Length; ++i)
          {
            var j = rand.Next(0, i + 1);
            _randomIndices[i] = _randomIndices[j];
            _randomIndices[j] = i;
          }
        }


        _getters = new Delegate[Schema.Count];
        var neededColumns = new HashSet<DataViewSchema.Column>(columnsNeeded);

        for (int i = 0; i < _getters.Length; ++i)
        {
          if (!neededColumns.Contains(Schema[i]))
            continue;

          switch (parent._table[Schema[i].Name])
          {
            case Altaxo.Data.DoubleColumn dc:
              if (Schema[i].Type == Microsoft.ML.Data.NumberDataViewType.Single)
                _getters[i] = (ValueGetter<float>)(new GetterClass(dc, this).FloatGetterImplementation);
              else
                _getters[i] = (ValueGetter<double>)(new GetterClass(dc, this).DoubleGetterImplementation);
              break;
            case Altaxo.Data.DateTimeColumn dtc:
              _getters[i] = (ValueGetter<DateTime>)(new GetterClass(dtc, this).DateTimeGetterImplementation);
              break;
            case Altaxo.Data.TextColumn dtc:
              _getters[i] = (ValueGetter<ReadOnlyMemory<char>>)(new GetterClass(dtc, this).TextGetterImplementation);
              break;
            case Altaxo.Data.BooleanColumn dtc:
              _getters[i] = (ValueGetter<bool>)(new GetterClass(dtc, this).BooleanGetterImplementation);
              break;
            default:
              throw new NotImplementedException();
          }
        }
      }


      public override long Batch => 0;

      public override DataViewSchema Schema { get; }




      public override ValueGetter<TValue> GetGetter<TValue>(DataViewSchema.Column column)
      {
        if (!IsColumnActive(column))
          throw new ArgumentOutOfRangeException(nameof(column));

        return (ValueGetter<TValue>)_getters[column.Index];
      }

      public override bool IsColumnActive(DataViewSchema.Column column)
      {
        return !(_getters[column.Index] is null);
      }


      public override ValueGetter<DataViewRowId> GetIdGetter()
      {
        void Getter(ref DataViewRowId val)
        {
          val = new DataViewRowId((ulong)Position, 0);
        }

        return Getter;
      }


      public override bool MoveNext()
      {
        if (_isDisposed)
          return false;

        ++_position;
        var result = _position < _dataView._rowCount;

        if (!result)
        {
          _isDisposed = true;
        }

        return result;
      }
    }
  }
}
