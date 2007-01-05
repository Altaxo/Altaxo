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
  /// IOutputService provides methods for outputting text for the user (for instance results of operations, warning and error messages).
  /// </summary>
  public interface IOutputService
  {
    /// <summary>Writes a string to the output.</summary>
    /// <param name="text">The text to write to the output.</param>
    void Write(string text);

    /// <summary>
    /// Writes a line using the format string and the parameters
    /// </summary>
    /// <param name="format">Format string.</param>
    /// <param name="list">Variable number of parameters</param>
    void Write(string format, params object[] list);

    /// <summary>
    /// Write a line using a format provider and a formatted string.
    /// </summary>
    /// <param name="provider">The format provider.</param>
    /// <param name="format">The formatted string.</param>
    /// <param name="args">The optional arguments.</param>
    void Write(System.IFormatProvider provider, string format, params object[] args);

    /// <summary>
    /// Starts a new line.
    /// </summary>
    void WriteLine();

    /// <summary>
    /// Writes a string to the output and then starts a new line.
    /// </summary>
    /// <param name="text">The string to write to the output.</param>
    void WriteLine(string text);

    /// <summary>
    /// Writes a line using the format string and the parameters, then starts a new line.
    /// </summary>
    /// <param name="format">Format string.</param>
    /// <param name="list">Variable number of parameters</param>
    void WriteLine(string format, params object[] list);

    /// <summary>
    /// Write a line using a format provider and a formatted string, then starts a new line.
    /// </summary>
    /// <param name="provider">The format provider.</param>
    /// <param name="format">The formatted string.</param>
    /// <param name="args">The optional arguments.</param>
    void WriteLine(System.IFormatProvider provider, string format, params object[] args);

  }
}
