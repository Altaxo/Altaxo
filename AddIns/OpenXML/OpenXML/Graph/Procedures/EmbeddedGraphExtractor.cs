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
using Altaxo.Graph.Gdi;
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
    public static List<(string ProjectFileName, Version AltaxoVersion, string GraphName)> FromWordExtractAllEmbeddedGraphsAsMiniprojects(string wordSourceFileName, string destinationFileNameBaseWOExtension, Altaxo.Main.Services.ExternalDrivenBackgroundMonitor? monitor)
    {
      var result = new List<(string ProjectFileName, Version AltaxoVersion, string GraphName)>();
      int documentNumber = 1;

      using (var document = WordprocessingDocument.Open(wordSourceFileName, false))
      {
        int totalNumberOfObjects = document.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Vml.Office.OleObject>().Where(x => x.ProgId.ToString().StartsWith("Altaxo.Graph")).Count();

        foreach (var embeddedOleObject in document.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Vml.Office.OleObject>().Where(x => x.ProgId.ToString().StartsWith("Altaxo.Graph")))
        {
          if (monitor is not null)
          {
            if (monitor.CancellationPending)
            {
              break;
            }
            if (monitor.ShouldReportNow == true)
            {
              monitor.ReportProgress($"Start processing object {documentNumber} of {totalNumberOfObjects}", documentNumber / (double)totalNumberOfObjects);
            }
          }

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
      return result;
    }

    /// <summary>
    /// Replaces all embedded graphs in a MS Word document by plain images. Attention: the current project is forced to close!!!
    /// Please save the project before in order not to loose data!!!
    /// </summary>
    /// <param name="wordSourceFileName">File name of the existing MS Word file that contains the embedded graphs.</param>
    /// <param name="wordDestinationFileName">File name of the MS Word file, to which the modified document is stored.</param>
    /// <param name="graphExportOptions">Export options for all graphs.</param>
    public static void WordReplaceAllEmbeddedGraphsWithImages(string wordSourceFileName, string wordDestinationFileName, GraphExportOptions graphExportOptions, Altaxo.Main.Services.ExternalDrivenBackgroundMonitor? monitor)
    {
      uint documentNumber = 1;

      System.IO.File.Copy(wordSourceFileName, wordDestinationFileName, false);

      using (var document = WordprocessingDocument.Open(wordDestinationFileName, true))
      {
        int totalNumberOfObjects = document.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Vml.Office.OleObject>().Where(x => x.ProgId.ToString().StartsWith("Altaxo.Graph")).Count();

        foreach (var embeddedOleObject in document.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Vml.Office.OleObject>().Where(x => x.ProgId.ToString().StartsWith("Altaxo.Graph")))
        {
          if (monitor is not null)
          {
            if (monitor.CancellationPending)
            {
              break;
            }
            if (monitor.ShouldReportNow == true)
            {
              monitor.ReportProgress($"Start processing object {documentNumber} of {totalNumberOfObjects}", documentNumber / (double)totalNumberOfObjects);
            }
          }

          var parent = embeddedOleObject.Parent as DocumentFormat.OpenXml.Wordprocessing.EmbeddedObject;
          var shape = parent.ChildElements.OfType<DocumentFormat.OpenXml.Vml.Shape>().FirstOrDefault();
          var (imgWidth, imgHeight) = FromShapeGetWidthAndHeight(shape);

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

          var mainGraph = Current.Dispatcher.InvokeIfRequired(

          () => GraphDocumentInStorageFile.OpenAltaxoProjectFromFromStructuredStorageFile(tempFileName));
          var exporter = Current.ProjectService.GetProjectItemImageExporter(mainGraph);

          if (exporter is null)
            throw new ArgumentException("Did not find exporter for document of type " + mainGraph?.GetType().ToString() ?? string.Empty, nameof(mainGraph));

          // now we can directly export the image stream to the word document

          using (Stream imageStream = new MemoryStream())
          {
            var (pixelsX, pixelsY) = exporter.ExportAsImageToStream(mainGraph, graphExportOptions, imageStream);
            var imgPartType = Text.Renderers.OpenXML.Inlines.LinkInlineRenderer.GetImagePartTypeFromExtension(graphExportOptions.GetDefaultFileNameExtension()); // assuming we have a stream containing a .png image
            var mainPart = document.MainDocumentPart;
            var imagePart = mainPart.AddImagePart(imgPartType);  // Create a new image part  
            imageStream.Seek(0, SeekOrigin.Begin);
            imagePart.FeedData(imageStream); // save the image stream to the imagePart

            var drawing = Text.Renderers.OpenXML.Inlines.LinkInlineRenderer.CreateDrawing(
               mainPart.GetIdOfPart(imagePart),
               imgWidth, imgHeight,
               null, null,
               pixelsX, pixelsY,
               graphExportOptions.DestinationDpiResolution, graphExportOptions.DestinationDpiResolution,
               $"EmbeddedAltaxoFig_{documentNumber}",
               ref documentNumber);
            parent.Parent.ReplaceChild(drawing, parent);
          }
          File.Delete(tempFileName);

        }
        document.Save();
      }
    }


    /// <summary>
    /// Gets the width and height of a shape.
    /// </summary>
    /// <param name="shape">The shape to get width and height from.</param>
    /// <returns>Width and height of the shape (as far as they are known).</returns>
    public static (double? width, double? height) FromShapeGetWidthAndHeight(DocumentFormat.OpenXml.Vml.Shape shape)
    {
      double? width = null, height = null;
      if (shape is not null)
      {
        var style = shape.Style.Value;
        var styleParts = style.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var stylePart in styleParts)
        {
          string? number = null;
          if (stylePart.StartsWith("width:"))
          {
            number = stylePart.Substring("width:".Length);
            width = Text.Renderers.OpenXML.Inlines.LinkInlineRenderer.GetLength(number);
          }
          else if (stylePart.StartsWith("height:"))
          {
            number = stylePart.Substring("height:".Length);
            height = Text.Renderers.OpenXML.Inlines.LinkInlineRenderer.GetLength(number);
          }
        }
      }
      return (width, height);
    }
  }
}
