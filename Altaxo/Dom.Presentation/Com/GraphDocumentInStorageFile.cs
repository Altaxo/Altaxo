#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Altaxo.UnmanagedApi.Ole32;

namespace Altaxo.Com
{
  /// <summary>
  /// Helper class to extract a graph document that is embedded in an OLE structured storage file.
  /// </summary>
  public class GraphDocumentInStorageFile
  {
    /// <summary>
    /// Extracts an Altaxo project file from an structured storage file.
    /// </summary>
    /// <param name="fileNameOfStructuredStorageFile">The file name of structured storage file.</param>
    /// <param name="destinationFileName">Name of the destination file that accomodates the extracted Altaxo project. This file must not exist before!</param>
    /// <returns>Altaxo version this object was created with, and name of the main graph in the extracted project.</returns>
    public static (Version AltaxoVersion, string GraphName) ExtractAltaxoProjectFromStructuredStorageFile(string fileNameOfStructuredStorageFile, string destinationFileName)
    {
      Ole32Func.StgOpenStorage(fileNameOfStructuredStorageFile, null, STGM.READWRITE | STGM.SHARE_EXCLUSIVE, IntPtr.Zero, 0, out IStorage iStorage);
      try
      {
        return ExtractAltaxoProjectFromIStorage(iStorage, destinationFileName);
      }
      finally
      {
        Marshal.ReleaseComObject(iStorage);
      }
    }

    /// <summary>
    /// Extracts an Altaxo project file from an <see cref="IStorage"/> object.
    /// </summary>
    /// <param name="pstg">The <see cref="IStorage"/> object that contains the Altaxo project.</param>
    /// <param name="destinationFileName">Name of the destination file which accomodates the extracted Altaxo project. The file must not exist beforehand.</param>
    /// <returns>Altaxo version this object was created with, and name of the main graph in the extracted project.</returns>
    public static (Version AltaxoVersion, string GraphName) ExtractAltaxoProjectFromIStorage(IStorage pstg, string destinationFileName)
    {
      string? documentName = null;
      Version altaxoVersion;

      try
      {
        using (var stream = new ComStreamWrapper(pstg.OpenStream("AltaxoVersion", IntPtr.Zero, (int)(STGM.READ | STGM.SHARE_EXCLUSIVE), 0), true))
        {
          var bytes = new byte[stream.Length];
          stream.Read(bytes, 0, bytes.Length);
          var versionString = System.Text.Encoding.UTF8.GetString(bytes);
          altaxoVersion = Version.Parse(versionString);
        }
      }
      catch (Exception)
      {
        throw;
      }

      try
      {
        using (var stream = new ComStreamWrapper(pstg.OpenStream("AltaxoGraphName", IntPtr.Zero, (int)(STGM.READ | STGM.SHARE_EXCLUSIVE), 0), true))
        {
          var bytes = new byte[stream.Length];
          stream.Read(bytes, 0, bytes.Length);
          documentName = System.Text.Encoding.UTF8.GetString(bytes);
        }
      }
      catch (Exception)
      {
        throw;
      }

      try
      {
        using (var streamWrapper = new ComStreamWrapper(pstg.OpenStream("AltaxoProjectZip", IntPtr.Zero, (int)(STGM.READ | STGM.SHARE_EXCLUSIVE), 0), true))
        {
          using (var destStream = new System.IO.FileStream(destinationFileName, FileMode.CreateNew, FileAccess.Write, FileShare.None))
          {
            streamWrapper.CopyTo(destStream);
          }
        }
      }
      catch (Exception)
      {
        throw;
      }
      return (altaxoVersion, documentName);
    }
  }
}
