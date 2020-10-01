#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Compression
{
  /// <summary>
  /// Designates options the Zip analyzer should check.
  /// </summary>
  [Flags]
  public enum ZipAnalyzerOptions
  {
    /// <summary>
    /// Tests, whether or not the central directory contains records with the same file name.
    /// </summary>
    TestCentralDirectoryForNameDublettes = 0x01,

    /// <summary>
    /// Tests if the local file headers starts at position 0, ends at the position where the central directory starts,
    /// and are exactly in the same order as the central directory entries. Furthermore, no extra space between local file headers is allowed.
    /// </summary>
    TestStrictOrderOfLocalFileHeaders = 0x02,

    /// <summary>
    /// Tests the existence of the local file headers. For each record in the central directory it is checked, if the corresponding local
    /// file header exists, and if it has the same data as the central directory record. Note: in order to save time, the equality of the file
    /// names is not checked.
    /// </summary>
    TestExistenceOfTheLocalFileHeaders = 0x04,

    /// <summary>
    /// Checks the CRC checksum of all items
    /// </summary>
    CheckCrcOfItems = 0x08,
  }
}
