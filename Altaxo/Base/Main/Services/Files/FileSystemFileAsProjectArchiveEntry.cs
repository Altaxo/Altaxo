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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services.Files
{
  public class FileSystemFileAsProjectArchiveEntry : IProjectArchiveEntry
  {
    private string _baseFolder;

    /// <summary>
    /// This is the name of the file, including relative folders. E.g., Table1 in the
    /// Worksheet folder would be named 'Worksheet\Table1.xml'. Note that there should be
    /// no leading directory separator char.
    /// </summary>
    private string _name;


    public FileSystemFileAsProjectArchiveEntry(string baseFolder, string name)
    {
      if (string.IsNullOrEmpty(baseFolder))
        throw new ArgumentNullException(nameof(baseFolder));
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof(name));

      _baseFolder = baseFolder;
      _name = name;
    }

    public string FullName => _name;

    public Stream OpenForReading()
    {
      var finalName = Path.Combine(_baseFolder, _name);
      return new FileStream(finalName, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public Stream OpenForWriting()
    {
      var finalName = Path.Combine(_baseFolder, _name);
      Directory.CreateDirectory(FileName.GetDirectoryName(finalName));
      return new FileStream(finalName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
    }

  }

}
