#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
    /// Interface to a sequence of elements with unknown length.
    /// </summary>
    /// <typeparam name="T">The type of the elements of this sequence.</typeparam>
    public interface INumericSequence<T>
    {
        /// <summary>Gets the element of the sequence at index i.</summary>
        /// <value>The element at index i.</value>
        T this[int i] { get; }
    }

    /// <summary>
    /// Interface for a read-only vector of values. The first valid index of this vector is 0, the last one in (<see cref="Length"/>-1).
    /// </summary>
    /// <typeparam name="T">The type of the elements of this vector.</typeparam>
    /// <seealso cref="System.Collections.Generic.IReadOnlyList{T}" />
    public interface IROVector<T> : System.Collections.Generic.IReadOnlyList<T>
    {
        /// <summary>The number of elements of this vector. This property is for convenience only and should return the same value as <see cref="P:Count"/>.</summary>
        int Length { get; }
    }

    /// <summary>
    /// Interface for a a readable and writeable vector vector of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Altaxo.Calc.LinearAlgebra.IROVector{T}" />
    public interface IVector<T> : IROVector<T>
    {
        /// <summary>Read/write Accessor for the element at index i.</summary>
        /// <value>The element at index i.</value>
        new T this[int i] { get; set; }
    }

    /// <summary>
    /// Extends <see cref="IVector{T}"/> in a way that another vector
    /// can be appended at the end of this vector.
    /// </summary>
    public interface IExtensibleVector<T> : IVector<T>
    {
        /// <summary>
        /// Append vector a to the end of this vector.
        /// </summary>
        /// <param name="vector">The vector to append.</param>
        void Append(IROVector<T> vector);
    }
}
