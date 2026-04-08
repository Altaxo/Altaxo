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
  /// Contains the result of analyzing a ZIP archive for structural issues.
  /// </summary>
  public class ZipAnalyzerResult
  {
    /// <summary>
    /// Gets or sets a value indicating whether the end of central directory record was not found.
    /// </summary>
    public bool EndOfCentralDirectoryNotFound { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the central directory is corrupt.
    /// </summary>
    public bool CentralDirectoryCorrupt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the central directory contains duplicate entries.
    /// </summary>
    public bool CentralDirectoryContainsDublettes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether local file headers are missing.
    /// </summary>
    public bool AreLocalFileHeadersMissing { get; set; }

    /// <summary>
    /// Gets the list of central directory records for which the local file headers are missing.
    /// </summary>
    public List<CentralDirectoryRecord> MissingLocalFileHeaders { get; } = new List<CentralDirectoryRecord>();

    /// <summary>
    /// Gets a value indicating whether local file headers are corrupt.
    /// </summary>
    public bool AreLocalFileHeadersCorrupt => CorruptLocalFileHeaders.Count > 0;

    /// <summary>
    /// Gets the list of corrupt local file headers together with their central directory records.
    /// </summary>
    public List<(CentralDirectoryRecord CDE, LocalFileHeader LFH)> CorruptLocalFileHeaders { get; } = new List<(CentralDirectoryRecord CDE, LocalFileHeader LFH)>();

    /// <summary>
    /// Gets or sets the error message produced during analysis.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

  }
}
