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
using Altaxo.Data;

namespace Altaxo.Calc.LinearAlgebra
{
  
  public class DataColumnWrapper
  {
    #region Inner classes

    #region ROVector
    /// <summary>
    /// Wraps a <see>DataColumns</see> into a read-only vector.
    /// </summary>
    class NumericColumnToROVectorWrapper : IROVector
    {
      Altaxo.Data.INumericColumn _column;
      int _rows;

    
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="column">The <see>DataColumn</see> to wrap.</param>
      /// <param name="nRows">The number of rows that are part of the vector. (Starting from index 0).</param>
      public NumericColumnToROVectorWrapper(Altaxo.Data.INumericColumn column, int nRows)
      {
        _column = column;
        _rows = nRows;
      }
      #region IROVector Members

  
      /// <summary>The smallest valid index of this vector</summary>
      public int LowerBound { get { return 0; }}
    
      /// <summary>The greates valid index of this vector. Is by definition LowerBound+Length-1.</summary>
      public int UpperBound { get { return _rows-1; }}
    
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

    #endregion

    #region Vector
    /// <summary>
    /// Wraps a <see>DataColumns</see> into a read-only vector.
    /// </summary>
    class DoubleColumnToVectorWrapper : IVector
    {
      DoubleColumn _column;
      int _rows;

    
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="column">The <see>DataColumn</see> to wrap.</param>
      /// <param name="nRows">The number of rows that are part of the vector. (Starting from index 0).</param>
      public DoubleColumnToVectorWrapper(DoubleColumn column, int nRows)
      {
        _column = column;
        _rows = nRows;
      }
      #region IROVector Members

  
      /// <summary>The smallest valid index of this vector</summary>
      public int LowerBound { get { return 0; }}
    
      /// <summary>The greates valid index of this vector. Is by definition LowerBound+Length-1.</summary>
      public int UpperBound { get { return _rows-1; }}
    
      /// <summary>The number of elements of this vector.</summary>
      public int Length { get {; return _rows; }}

      /// <summary>
      /// Element accessor.
      /// </summary>
      public double this[int row]
      {
        get
        {
          return _column[row];
        }
        set
        {
          _column[row]=value;
        }
      }

      #endregion
    }

    #endregion
    #endregion

    /// <summary>
    /// This returns a read-only vector of a <see>INumericColumn</see>
    /// </summary>
    /// <param name="col">The column to wrap as a IVector.</param>
    /// <param name="nRows">The number of rows to use for the vector.</param>
    /// <returns>An IVector wrapping the <see>INumericColumn</see>.</returns>
    public static IROVector ToROVector(INumericColumn col, int nRows)
    {
      return new NumericColumnToROVectorWrapper(col,nRows);
    }

    /// <summary>
    /// This returns a read-only vector of a <see>INumericColumn</see>
    /// </summary>
    /// <param name="col">The column to wrap as a IVector.</param>
    /// <param name="nRows">The number of rows to use for the vector.</param>
    /// <returns>An IVector wrapping the <see>INumericColumn</see>.</returns>
    public static IROVector ToROVector(DataColumn col, int nRows)
    {
      if(!(col is INumericColumn))
        throw new ArgumentException("Argument col can not be wrapped to a vector because it is not a numeric column");
      
      return new NumericColumnToROVectorWrapper((INumericColumn)col,nRows);
    }

    /// <summary>
    /// This returns a read-only vector of a <see>INumericColumn</see>
    /// </summary>
    /// <param name="col">The column to wrap as a IVector.</param>
    /// <returns>An IVector wrapping the <see>INumericColumn</see>.</returns>
    public static IROVector ToROVector(DataColumn col)
    {
      if(!(col is INumericColumn))
        throw new ArgumentException("Argument col can not be wrapped to a vector because it is not a numeric column");
      
      return new NumericColumnToROVectorWrapper((INumericColumn)col,col.Count);
    }


    /// <summary>
    /// This returns a read and writeable vector of a <see>DoubleColumn</see>
    /// </summary>
    /// <param name="col">The column to wrap as a IVector.</param>
    /// <returns>An IVector wrapping the <see>DoubleColumn</see>.</returns>
    public static IVector ToVector(DoubleColumn col)
    {
      return new DoubleColumnToVectorWrapper(col,col.Count);
    }

    /// <summary>
    /// This returns a read and writeable vector of a <see>DoubleColumn</see>
    /// </summary>
    /// <param name="col">The column to wrap as a IVector.</param>
    /// <param name="nRows">The number of rows to use for the vector.</param>
    /// <returns>An IVector wrapping the <see>DoubleColumn</see>.</returns>
    public static IVector ToVector(DoubleColumn col, int nRows)
    {
      return new DoubleColumnToVectorWrapper(col,nRows);
    }

   

    /// <summary>
    /// This returns a read and writeable vector of a <see>DoubleColumn</see>
    /// </summary>
    /// <param name="col">The column to wrap as a IVector.</param>
    /// <param name="nRows">The number of rows to use for the vector.</param>
    /// <returns>An IVector wrapping the <see>DoubleColumn</see>.</returns>
    public static IVector ToVector(DataColumn col, int nRows)
    {
      if(!(col is DoubleColumn))
        throw new ArgumentException("Argument col can not be wrapped to a vector because it is not a DoubleColumn");

      return new DoubleColumnToVectorWrapper((DoubleColumn)col,nRows);
    }

    /// <summary>
    /// This returns a read and writeable vector of a <see>DoubleColumn</see>
    /// </summary>
    /// <param name="col">The column to wrap as a IVector.</param>
    /// <returns>An IVector wrapping the <see>DoubleColumn</see>.</returns>
    public static IVector ToVector(DataColumn col)
    {
      if(!(col is DoubleColumn))
        throw new ArgumentException("Argument col can not be wrapped to a vector because it is not a DoubleColumn");

      return new DoubleColumnToVectorWrapper((DoubleColumn)col,col.Count);
    }

  }
  
}
