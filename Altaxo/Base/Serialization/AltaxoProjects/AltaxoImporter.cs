#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using System.IO;

namespace Altaxo.Serialization.AltaxoProjects
{
  /// <summary>
  /// Helper class to import an Altaxo project into an already open Altaxo project.
  /// </summary>
  public record class AltaxoImporter
  {
    /// <summary>
    /// Gets the supported file extensions and their description.
    /// </summary>
    /// <returns>The supported file extensions and their description.</returns>
    public /*override*/ (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".axoprj"], "Altaxo project files (*.axoprj)");
    }

    /// <summary>
    /// Determines the probability that the specified file or directory is an Altaxo project.
    /// </summary>
    /// <param name="fileName">The file or directory name to inspect.</param>
    /// <returns>A probability value between 0 and 1.</returns>
    public /*override*/ double GetProbabilityForBeingThisFileFormat(string fileName)
    {
      // we have a speciality here: filename can either be a folder or a file

      double p = 0;
      bool foundTables = false;
      bool foundGraphs = false;
      bool foundWorkbench = false;
      bool foundDocumentInformation = false;

      if (File.Exists(fileName))
      {
        try
        {
          using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
          using var zipArch = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read, false);


          foreach (var entry in zipArch.Entries)
          {
            var entryNameLower = entry.Name.ToLowerInvariant();
            if ((entryNameLower.StartsWith("tables/") || entryNameLower.StartsWith("tables\\")) && entryNameLower.EndsWith(".xml"))
              foundTables = true;
            if ((entryNameLower.StartsWith("graphs/") || entryNameLower.StartsWith("graphs\\")) && entryNameLower.EndsWith(".xml"))
              foundGraphs = true;
            if ((entryNameLower.StartsWith("graphs3d/") || entryNameLower.StartsWith("graphs3d\\")) && entryNameLower.EndsWith(".xml"))
              foundGraphs = true;
            if (entryNameLower == "workbench/mainwindow.xml" || entryNameLower == "workbench\\mainwindow.xml")
              foundWorkbench = true;
            if (entryNameLower == "documentinformation.xml")
              foundDocumentInformation = true;
          }

          p = foundTables || foundGraphs || foundWorkbench || foundDocumentInformation ? 1 : 0;
        }
        catch
        {
          p = 0;
        }
      }
      else if (Directory.Exists(fileName))
      {
        try
        {
          var dirInfo = new DirectoryInfo(fileName);
          var files = dirInfo.GetFiles("*", SearchOption.AllDirectories);

          foreach (var file in files)
          {
            var entryNameLower = file.FullName.Substring(dirInfo.FullName.Length + 1).ToLowerInvariant();

            if ((entryNameLower.StartsWith("tables/") || entryNameLower.StartsWith("tables\\")) && entryNameLower.EndsWith(".xml"))
              foundTables = true;
            if ((entryNameLower.StartsWith("graphs/") || entryNameLower.StartsWith("graphs\\")) && entryNameLower.EndsWith(".xml"))
              foundGraphs = true;
            if ((entryNameLower.StartsWith("graphs3d/") || entryNameLower.StartsWith("graphs3d\\")) && entryNameLower.EndsWith(".xml"))
              foundGraphs = true;
            if (entryNameLower == "workbench/mainwindow.xml" || entryNameLower == "workbench\\mainwindow.xml")
              foundWorkbench = true;
            if (entryNameLower == "documentinformation.xml")
              foundDocumentInformation = true;
          }
          p = foundTables || foundGraphs || foundWorkbench || foundDocumentInformation ? 1 : 0;
        }
        catch
        {
          p = 0;
        }
      }

      return p;
    }
  }
}
