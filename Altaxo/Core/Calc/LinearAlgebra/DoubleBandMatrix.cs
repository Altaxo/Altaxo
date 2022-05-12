#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Represents a band matrix in compact storage format (stored as a linear array).
  /// </summary>
  public class DoubleBandMatrix : IMatrix<double>
  {
    private int _kl;
    private int _ku;
    private int _m; // number of rows
    private int _n; // number of columns
    private double[] _array; // kl+ku+1 rows x n columns


    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleBandMatrix"/> class.
    /// </summary>
    /// <param name="rows">Number of rows.</param>
    /// <param name="cols">Number of columns</param>
    /// <param name="lowerBands">Lower bandwidth.</param>
    /// <param name="upperBands">Upper bandwidth.</param>
    public DoubleBandMatrix(int rows, int cols, int lowerBands, int upperBands)
    {
      _m = rows;
      _n = cols;
      _kl = lowerBands;
      _ku = upperBands;
      _array = new double[(_kl + _ku + 1) * _n];
    }

    public double this[int i, int j]
    {
      get
      {
        if (Math.Max(0, j - _ku) <= i && i <= Math.Min(_n - 1, j + _kl))
        {
          // internal row:  _ku + i - j;
          // internal col: j;
          return _array[_n * (_ku + i - j) + j];
        }
        else
        {
          return 0;
        }
      }
      set
      {
        if (Math.Max(0, j - _ku) <= i && i <= Math.Min(_n - 1, j + _kl))
        {
          // row:  _ku + i - j;
          // col: j;
          _array[_n * (_ku + i - j) + j] = value;
        }
        else
        {
          throw new InvalidOperationException($"Try to set the element [{i}, {j}] of an {_m}x{_n} {_kl}-{_ku} band matrix");
        }
      }
    }

    public int RowCount => _m;

    public int ColumnCount => _n;
  }
}
