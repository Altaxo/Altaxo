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

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Provides methods for displaying the information obtained from the data reader (number of item, x data, y data).
  /// </summary>
  public interface IDataDisplayService
  {
    /// <summary>Writes a string to the output.</summary>
    /// <param name="text">The text to write to the output.</param>
    void WriteOneLine(string text);

    /// <summary>
    /// Writes two lines to the window.
    /// </summary>
    /// <param name="line1">First line.</param>
    /// <param name="line2">Second line.</param>
    void WriteTwoLines(string line1, string line2);

    /// <summary>
    /// Writes three lines to the output.
    /// </summary>
    /// <param name="line1">First line.</param>
    /// <param name="line2">Second line.</param>
    /// <param name="line3">Three line.</param>
    void WriteThreeLines(string line1, string line2, string line3);

  

  }
}
