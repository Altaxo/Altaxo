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

using System.Collections.Generic;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Provides a method to separate tokens in a line of ascii text.
  /// </summary>
  public interface IAsciiSeparationStrategy : Main.IImmutable
  {
    /// <summary>
    /// For a given line of ascii text, this gives the separated tokens as an enumerable list of strings.
    /// </summary>
    /// <param name="line">The ascii text line (should be a single line, because most of the methods assume that no
    /// line feeds occur).</param>
    /// <returns>List of separated strings (tokens).</returns>
    IEnumerable<string> GetTokens(string line);
  }
}
