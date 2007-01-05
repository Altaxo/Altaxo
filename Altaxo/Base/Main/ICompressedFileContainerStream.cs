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

namespace Altaxo.Main
{
  /// <summary>
  /// Summary description for ICompressedFileContainerStream.
  /// </summary>
  public interface ICompressedFileContainerStream
  {
    void StartFile(string filename, int level);
    //ZipEntry ZipEntry = new ZipEntry(filename);
    //zippedStream.PutNextEntry(ZipEntry);
    //zippedStream.SetLevel(level);

    System.IO.Stream Stream { get; }

  }

  public interface IFileContainerItem
  {
    bool IsDirectory { get; }
    string Name { get; }
  }
  

  public interface ICompressedFileContainer : System.Collections.IEnumerable
  {
    System.IO.Stream GetInputStream(IFileContainerItem item);   
  }
}
