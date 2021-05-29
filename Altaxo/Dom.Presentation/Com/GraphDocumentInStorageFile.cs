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
    /// Extracts an Altaxo project file from an structured storage file and opens it. Attention:
    /// the current project is forced to close!!!
    /// </summary>
    /// <param name="fileNameOfStructuredStorageFile">The file name of structured storage file.</param>
    /// <returns>At return, the project that was contained in the structured storage file is currently open.
    /// The return value is the main graph of the mini project.</returns>
    public static Altaxo.Graph.GraphDocumentBase? OpenAltaxoProjectFromFromStructuredStorageFile(string fileNameOfStructuredStorageFile)
    {
      Ole32Func.StgOpenStorage(fileNameOfStructuredStorageFile, null, STGM.READWRITE | STGM.SHARE_EXCLUSIVE, IntPtr.Zero, 0, out IStorage iStorage);
      try
      {
        using (var streamWrapper = OpenAltaxoProjectStreamFromIStorage(iStorage, out var version, out var graphName))
        {
          try
          {
            Current.IProjectService.CloseProject(true);
          }
          catch (Exception ex)
          {
            return null;
          }

          try
          {
            using (var archive = new Altaxo.Main.Services.Files.ZipArchiveAsProjectArchive(streamWrapper, System.IO.Compression.ZipArchiveMode.Read, true))
            {
              Current.IProjectService.OpenProjectFromArchive(archive);
            }
          }
          catch (Exception ex2)
          {
            return null;
          }

          if (Current.Project.GraphDocumentCollection.Contains(graphName))
            return Current.Project.GraphDocumentCollection[graphName];
          else if (Current.Project.Graph3DDocumentCollection.Contains(graphName))
            return Current.Project.Graph3DDocumentCollection[graphName];
          else if (Current.Project.GraphDocumentCollection.Count == 1)
            return Current.Project.GraphDocumentCollection.First();
          else if (Current.Project.Graph3DDocumentCollection.Count == 1)
            return Current.Project.Graph3DDocumentCollection.First();
          else
            return null;
        }
      }
      finally
      {
        Marshal.ReleaseComObject(iStorage);
      }
    }

    /// <summary>
    /// Extracts the Altaxo project stream and the other properties from an <see cref="IStorage"/> object.
    /// </summary>
    /// <param name="pstg">The <see cref="IStorage"/> object that contains the Altaxo project.</param>
    /// <param name="altaxoVersion">>Returns the Altaxo version this object was created with.</param>
    /// <param name="graphName">Returns the graph name of the main graph of this mini project.</param>
    /// <returns>The opened project stream of the Altaxo project. You as the caller are responsible for properly closing the stream again.</returns>
    public static Stream OpenAltaxoProjectStreamFromIStorage(IStorage pstg, out Version altaxoVersion, out string graphName)
    {
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
          graphName = System.Text.Encoding.UTF8.GetString(bytes);
        }
      }
      catch (Exception)
      {
        throw;
      }

      try
      {
        var streamWrapper = new ComStreamWrapper(pstg.OpenStream("AltaxoProjectZip", IntPtr.Zero, (int)(STGM.READ | STGM.SHARE_EXCLUSIVE), 0), true);
        return streamWrapper;
      }
      catch (Exception)
      {
        throw;
      }
    }

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
      using(var projectStream = OpenAltaxoProjectStreamFromIStorage(pstg, out var altaxoVersion, out var graphName))
      {
        using (var destStream = new System.IO.FileStream(destinationFileName, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
          projectStream.CopyTo(destStream);
        }
        return (altaxoVersion, graphName);
      }
    }
  }
}
