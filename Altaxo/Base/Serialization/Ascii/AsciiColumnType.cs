#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Designates the type of a column to import.
  /// </summary>
  public enum AsciiColumnType
  {
    /// <summary>Unspecified, type of the element in the column is not determined (for instance, if sometimes a number is missing in that column).</summary>
    DBNull,

    /// <summary>Column contains integer values.</summary>
    Int64,

    /// <summary>Column contains floating-point numbers.</summary>
    Double,

    /// <summary>Column contains date/time values.</summary>
    DateTime,

    /// <summary>Column contains text (could not parsed as number or data/time).</summary>
    Text
  }
}
