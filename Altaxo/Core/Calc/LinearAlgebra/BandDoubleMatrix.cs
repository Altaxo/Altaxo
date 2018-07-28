using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.LinearAlgebra
{
  [Obsolete("Do not use this class, it is only temporary, and will be replaced by a more advanced version")]
  public class BandDoubleMatrix : IMatrix<double>, IROBandMatrix<double>, IROMatrixLevel1<double>
  {
    private double[][] _array;
    private int _rowCount;
    private int _columnCount;
    private int _lowerBandwidth;
    private int _upperBandwidth;

    public BandDoubleMatrix(int numberOfRows, int numberOfColumns, int p, int q)
    {
      _rowCount = numberOfRows;
      _columnCount = numberOfColumns;
      _lowerBandwidth = p;
      _upperBandwidth = q;
      _array = new double[_rowCount][];
      for (int i = 0; i < _array.Length; ++i)
        _array[i] = new double[_columnCount];
    }

    public MatrixWrapperStructForLeftSpineJaggedArray<double> InternalData
    {
      get
      {
        return new MatrixWrapperStructForLeftSpineJaggedArray<double>(_array, _rowCount, _columnCount);
      }
    }

    public double this[int row, int col]
    {
      get
      {
        return _array[row][col];
      }
      set
      {
        _array[row][col] = value;
      }
    }

    double IROMatrix<double>.this[int row, int col]
    {
      get
      {
        return _array[row][col];
      }
    }

    public int RowCount => _rowCount;

    public int ColumnCount => _columnCount;

    public int LowerBandwidth { get { return _lowerBandwidth; } }
    public int UpperBandwidth { get { return _upperBandwidth; } }

    public void MapIndexed(Func<int, int, double, double> function, IMatrix<double> result, Zeros zeros = Zeros.AllowSkip)
    {
      int rowCount = _rowCount;
      int columnCount = _columnCount;

      if (zeros != Zeros.Include && !(result is IROBandMatrix<double> bm && bm.LowerBandwidth == _lowerBandwidth && bm.UpperBandwidth == _upperBandwidth))
      {
        MatrixMath.Clear(result);
      }

      switch (zeros)
      {
        case Zeros.AllowSkip:
        case Zeros.AllowSkipButIncludeDiagonal:
          {
            for (int i = 0; i < rowCount; ++i)
            {
              var array_i = _array[i];
              int j_start = Math.Max(0, i - _lowerBandwidth);
              int j_end = Math.Min(columnCount, i + _upperBandwidth + 1);
              for (int j = j_start; j < j_end; ++j)
                result[i, j] = function(i, j, array_i[j]);
            }
          }
          break;

        case Zeros.Include:
          {
            for (int i = 0; i < rowCount; ++i)
            {
              var array_i = _array[i];
              int j_start = Math.Max(0, i - _lowerBandwidth);
              int j_end = Math.Min(columnCount, i + _upperBandwidth + 1);
              for (int j = 0; j < j_start; ++j)
                result[i, j] = function(i, j, 0);
              for (int j = j_start; j < j_end; ++j)
                result[i, j] = function(i, j, array_i[j]);
              for (int j = j_end; j < columnCount; ++j)
                result[i, j] = function(i, j, 0);
            }
          }
          break;

        default:
          throw new NotImplementedException();
      }
    }

    public void MapIndexed<T1>(T1 sourceParameter1, Func<int, int, double, T1, double> function, IMatrix<double> result, Zeros zeros = Zeros.AllowSkip)
    {
      int rowCount = _rowCount;
      int columnCount = _columnCount;

      if (zeros != Zeros.Include && !(result is IROBandMatrix<double> bm && bm.LowerBandwidth == _lowerBandwidth && bm.UpperBandwidth == _upperBandwidth))
      {
        MatrixMath.Clear(result);
      }

      switch (zeros)
      {
        case Zeros.AllowSkip:
        case Zeros.AllowSkipButIncludeDiagonal:
          {
            for (int i = 0; i < rowCount; ++i)
            {
              var array_i = _array[i];
              int j_start = Math.Max(0, i - _lowerBandwidth);
              int j_end = Math.Min(columnCount, i + _upperBandwidth + 1);

              for (int j = j_start; j < j_end; ++j)
              {
                result[i, j] = function(i, j, array_i[j], sourceParameter1);
              }
            }
          }
          break;

        case Zeros.Include:
          {
            for (int i = 0; i < rowCount; ++i)
            {
              var array_i = _array[i];
              int j_start = Math.Max(0, i - _lowerBandwidth);
              int j_end = Math.Min(columnCount, i + _upperBandwidth + 1);
              for (int j = 0; j < j_start; ++j)
                result[i, j] = function(i, j, 0, sourceParameter1);
              for (int j = j_start; j < j_end; ++j)
                result[i, j] = function(i, j, array_i[j], sourceParameter1);
              for (int j = j_end; j < columnCount; ++j)
                result[i, j] = function(i, j, 0, sourceParameter1);
            }
          }
          break;

        default:
          throw new NotImplementedException();
      }
    }

    public IEnumerable<(int row, int column, double value)> EnumerateElementsIndexed(Zeros zeros = Zeros.AllowSkip)
    {
      var rowCount = this._rowCount;
      var colCount = this._columnCount;
      int i, j;

      switch (zeros)
      {
        case Zeros.AllowSkip:
        case Zeros.AllowSkipButIncludeDiagonal:
          {
            for (i = 0; i < rowCount; ++i)
            {
              var array_i = _array[i];
              int j_start = Math.Max(0, i - _lowerBandwidth);
              int j_end = Math.Min(colCount, i + _upperBandwidth + 1);
              for (j = j_start; j < j_end; ++j)
              {
                yield return (i, j, array_i[j]);
              }
            }
          }
          break;

        case Zeros.Include:
          {
            for (i = 0; i < rowCount; ++i)
            {
              var array_i = _array[i];
              int j_start = Math.Max(0, i - _lowerBandwidth);
              int j_end = Math.Min(colCount, i + _upperBandwidth + 1);
              for (j = 0; i < j_start; ++j)
                yield return (i, j, 0);
              for (j = j_start; j < j_end; ++j)
                yield return (i, j, array_i[j]);
              for (j = j_end; j < colCount; ++j)
                yield return (i, j, 0);
            }
          }
          break;

        default:
          throw new NotImplementedException();
      }
    }
  }
}
