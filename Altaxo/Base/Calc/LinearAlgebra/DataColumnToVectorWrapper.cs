#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Calc;

namespace Altaxo.Calc.LinearAlgebra
{
  

  /// <summary>
  /// Wraps a <see>DataColumns</see> into a read-only vector.
  /// </summary>
  class DataColumnToVectorWrapper : IROVector
  {
    Altaxo.Data.INumericColumn _column;
    int _rows;

    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="column">The <see>DataColumn</see> to wrap.</param>
    /// <param name="nRows">The number of rows that are part of the vector. (Starting from index 0).</param>
    public DataColumnToVectorWrapper(Altaxo.Data.INumericColumn column, int nRows)
    {
      _column = column;
      _rows = nRows;
    }
    #region IROVector Members

  
    /// <summary>The smallest valid index of this vector</summary>
    public int LowerBound { get { return 0; }}
    
    /// <summary>The greates valid index of this vector. Is by definition LowerBound+Length-1.</summary>
    public int UpperBound { get { return _rows; }}
    
    /// <summary>The number of elements of this vector.</summary>
    public int Length { get {; return _rows; }}

    /// <summary>
    /// Element accessor.
    /// </summary>
    public double this[int row]
    {
      get
      {
        return _column.GetDoubleAt(row);
      }
    }

    #endregion
  }

  
}
