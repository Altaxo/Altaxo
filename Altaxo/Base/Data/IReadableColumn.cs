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

namespace Altaxo.Data
{


  /// <summary>
  /// This designates a vector structure, which holds elements. A single element at a given index can be read out
  /// by returning a AltaxoVariant.
  /// </summary>
  public interface IReadableColumn : ICloneable
  {

    /// <summary>
    /// The indexer property returns the element at index i as an AltaxoVariant.
    /// </summary>
    AltaxoVariant this[int i] 
    {
      get;
    }

    /// <summary>
    /// Returns true, if the value at index i of the column
    /// is null or invalid or in another state comparable to null or empty
    /// </summary>
    /// <param name="i">The index to the element.</param>
    /// <returns>true if element is null/empty, false if the element is valid</returns>
    bool IsElementEmpty(int i);

    /// <summary>
    /// FullName returns a descriptive name for a column
    /// for columns which belongs to a table, the table name and the column
    /// name, separated by a backslash, should be returned
    /// for other columns, a descriptive name should be returned so that the
    /// user knows the location of this column
    /// </summary>
    string FullName
    {
      get;
    }

    /*
    /// <summary>
    /// Returns a descriptive name for the column. A level of zero only returns a basic (short) name. The higher the level,
    /// the longer should be the returned name.
    /// </summary>
    /// <param name="level">Name level.</param>
    /// <returns>The descriptive name of the column according with a length according to the level.</returns>
    string GetName(int level);
    */
  }

}
