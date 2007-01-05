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
  public interface IComplexDoubleSequence
  {
    /// <summary>Gets the element of the sequence at index i.</summary>
    /// <value>The element at index i.</value>
    Complex this[int i] { get; }
  }

  /// <summary>
  /// Interface for a read-only vector of Complex values.
  /// </summary>
  public interface IROComplexDoubleVector : IComplexDoubleSequence
  {
    /// <summary>The smallest valid index of this vector</summary>
    int LowerBound { get; }
    
    /// <summary>The greates valid index of this vector. Is by definition LowerBound+Length-1.</summary>
    int UpperBound { get; }
    
    /// <summary>The number of elements of this vector.</summary>
    int Length { get; }  // change this later to length property

  }

  /// <summary>
  /// Interface for a readable and writeable vector of Complex values.
  /// </summary>
  public interface IComplexDoubleVector : IROComplexDoubleVector
  {
    /// <summary>Read/write Accessor for the element at index i.</summary>
    /// <value>The element at index i.</value>
    new Complex this[int i] { get; set; }
  }

  /// <summary>
  /// Special vector to which another vector can be appended to.
  /// </summary>
  public interface IExtensibleComplexDoubleVector : IComplexDoubleVector
  {
    /// <summary>
    /// Append vector a to the end of this vector.
    /// </summary>
    /// <param name="a">The vector to append.</param>
    void Append(IROComplexDoubleVector a);
  }

}
