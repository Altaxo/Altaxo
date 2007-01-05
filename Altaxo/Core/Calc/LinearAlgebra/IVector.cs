#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.LinearAlgebra
{
  public interface INumericSequence
  {
    /// <summary>Gets the element of the sequence at index i.</summary>
    /// <value>The element at index i.</value>
    double this[int i] { get; }
  }

  /// <summary>
  /// Interface for a read-only vector of double values.
  /// </summary>
  public interface IROVector : INumericSequence
  {
    /// <summary>The smallest valid index of this vector</summary>
    int LowerBound { get; }
    
    /// <summary>The greates valid index of this vector. Is by definition LowerBound+Length-1.</summary>
    int UpperBound { get; }
    
    /// <summary>The number of elements of this vector.</summary>
    int Length { get; }  // change this later to length property

  }

  /// <summary>
  /// Interface for a readable and writeable vector of double values.
  /// </summary>
  public interface IVector : IROVector
  {
    /// <summary>Read/write Accessor for the element at index i.</summary>
    /// <value>The element at index i.</value>
    new double this[int i] { get; set; }
  }

  /// <summary>
  /// IRightExtensibleMatrix extends IMatrix in a way that another matrix of appropriate dimensions
  /// can be appended to the right of the matrix. 
  /// </summary>
  public interface IExtensibleVector : IVector
  {
    /// <summary>
    /// Append vector a to the end of this vector.
    /// </summary>
    /// <param name="a">The vector to append.</param>
    void Append(IROVector a);
  }


  public abstract class AbstractRODoubleVector : IROVector
  {
    static public implicit operator AbstractRODoubleVector(double[] src)
    {
      return new RODoubleVector(src);
    }

    #region IROVector Members

    public virtual int LowerBound
    {
      get { return 0; }
    }

    public virtual int UpperBound
    {
      get
      {
        return Length - 1;
      }
    }

    public abstract int Length
    {
      get;
    }

    #endregion

    #region INumericSequence Members

    public abstract double this[int i]
    {
      get; 
    }

    #endregion
  }

  public class RODoubleVector : AbstractRODoubleVector
  {
    private double[] _data;

    public RODoubleVector(double[] array)
    {
      _data = array;
    }

   
   
    static public implicit operator RODoubleVector(double[] src)
    {
      return new RODoubleVector(src);
    }

    

    #region IROVector Members

    public override int LowerBound
    {
      get { return 0; }
    }

    public override int UpperBound
    {
      get { return _data.Length-1; }
    }

    public override int Length
    {
      get { return _data.Length; }
    }

    #endregion

    #region INumericSequence Members

    public override double this[int i]
    {
      get { return _data[i]; }
    }

    #endregion
  }
}
