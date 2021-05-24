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
using System.Text;
using System.Threading.Tasks;
using Altaxo.Com;
using DocumentFormat.OpenXml.Packaging;

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// Helper class to extract embedded graphs from MS Word files and save them as separate mini projects.
  /// </summary>
  public class EmbeddedGraphExtractor
  {
    /// <summary>
    /// Extracts from a MS Word file all embedded graphs as miniprojects.
    /// </summary>
    /// <param name="wordSourceFileName">Name of the MS Word source file (.docx).</param>
    /// <param name="destinationFileNameBaseWOExtension">The base file name of the destination files.
    /// This must be a full file name, including the full path, but without extension. To this base name, a number starting from 1 up to
    /// the number of embedded projects is appended, and then the extension (.axoprj) is appended, too.</param>
    /// <returns>List of tuples that contain file name, Altaxo version, and name of the main graph of the extracted mini projects.</returns>
    public static List<(string ProjectFileName, Version AltaxoVersion, string GraphName)> FromWordExtractAllEmbeddedGraphsAsMiniprojects(string wordSourceFileName, string destinationFileNameBaseWOExtension)
    {
      var result = new List<(string ProjectFileName, Version AltaxoVersion, string GraphName)>();
      int documentNumber = 1;

      using (var document = WordprocessingDocument.Open(wordSourceFileName, false))
      {
        foreach (var embeddedOleObject in document.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Vml.Office.OleObject>())
        {
          if (embeddedOleObject.ProgId.ToString().StartsWith("Altaxo.Graph"))
          {
            // we could extract something like a filename like
            // fileName = document.MainDocumentPart.GetPartById(emb.Id.ToString()).Uri.ToString().Remove(0, "/word/embeddings/".Length);
            // but this is unneccessary here, because
            // we save the embedded structured storage object to a temporary file
            var tempFileName = Path.GetTempFileName();
            // Write the stream to the temporary file.
            using (var writeStream = new FileStream(tempFileName, FileMode.Create, FileAccess.Write))
            {
              var readStream = document.MainDocumentPart.GetPartById(embeddedOleObject.Id).GetStream();
              readStream.CopyTo(writeStream);
              writeStream.Close();
            }

            var destinationFileName = $"{destinationFileNameBaseWOExtension}{documentNumber:D4}.axoprj";
            var (version, graphName) = GraphDocumentInStorageFile.ExtractAltaxoProjectFromStructuredStorageFile(tempFileName, destinationFileName);
            result.Add((destinationFileName, version, graphName));
            ++documentNumber;
            File.Delete(tempFileName);
          }
        }
      }
      return result;
    }
  }
}
