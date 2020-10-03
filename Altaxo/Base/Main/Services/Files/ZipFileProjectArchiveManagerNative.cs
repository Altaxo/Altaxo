#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Main.Services.Files;

namespace Altaxo.Main.Services
{

  /// <summary>
  /// Manages the permanent storage of projects into Zip files, including cloning to, and maintaining a safety copy.
  /// This manager uses Zip files zipped with the framework provided Zip routines. As such, no progressive storage is supported (and no deferred loading).
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.IFileBasedProjectArchiveManager" />
  public class ZipFileProjectArchiveManagerNative : ZipFileProjectArchiveManagerBase
  {
    /// <summary>
    /// Creates a new project archive. Here, a function returning a wrapper
    /// using the <see cref="System.IO.Compression.ZipArchive"/> class is returned.
    /// </summary>
    protected override ProjectArchiveCreationFunction CreateProjectArchive
    {
      get
      {
        return InternalCreateProjectArchive;
      }
    }

    private IProjectArchive InternalCreateProjectArchive(Stream stream, ZipArchiveMode zipArchiveMode, bool leaveOpen, IFileBasedProjectArchiveManager archiveManager)
    {
      return new Services.Files.ZipArchiveAsProjectArchiveNative(stream, zipArchiveMode, leaveOpen: leaveOpen, archiveManager);
    }
  }
}
