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

namespace Altaxo.Main.Services
{
  /// <summary>
  /// IOutputService provides methods for outputting text for the user (for instance results of operations, warning and error messages).
  /// </summary>
  public interface IOutputService
  {
    /// <summary>Writes a string to the output.</summary>
    /// <param name="text">The text to write to the output.</param>
    void Write(string text);

    /// <summary>
    /// Writes a string to the output and then starts a new line.
    /// </summary>
    /// <param name="text">The string to write to the output.</param>
    void WriteLine(string text);
  }
}
