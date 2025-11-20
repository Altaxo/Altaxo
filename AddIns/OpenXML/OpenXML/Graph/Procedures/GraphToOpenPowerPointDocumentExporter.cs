#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using Altaxo.Graph.Gdi;

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// Exports graphs to an open PowerPoint document.
  /// </summary>
  public class GraphToOpenPowerPointDocumentExporter
  {
    /// <summary>
    /// The prog identifier for PowerPoint application.
    /// </summary>
    private const string ProgIdForPowerPointApplication = "PowerPoint.Application";

    /// <summary>
    /// Exports the graphs to an open PowerPoint document, showing a error dialog in case of errors.
    /// </summary>
    /// <param name="list">The list of graphs to export.</param>
    /// <param name="forceUsingDropFile">If set to <c>true</c>, a drop file is created for each graph and inserted into the document. If <c>false</c> (default), the clipboard method is used.</param>
    /// <param name="isInteractive">If set to <c>true</c>, an error box is shown if no open PowerPoint document is found. If <c>false</c>, an exception is thrown.</param>
    public static void PushGraphsToOpenPowerPointDocument(IEnumerable<GraphDocumentBase> list, bool forceUsingDropFile, bool isInteractive)
    {
      // Connect to running PowerPoint (Microsoft.Office.Interop.PowerPoint.Application)
      dynamic? pptApp = GraphToOpenWordDocumentExporter.GetActiveObject(ProgIdForPowerPointApplication);
      dynamic? slideToInsertTo = pptApp?.ActiveWindow?.View?.Slide;

      if (pptApp is null || slideToInsertTo is null)
      {
        if (isInteractive)
        {
          Current.Gui.ErrorMessageBox("No open PowerPoint document found. Please open a PowerPoint document and select the slide where the first graph should be inserted.", "No Open PowerPoint Document");
          return;
        }
        else
        {
          throw new InvalidOperationException("No open PowerPoint document found. Please open a PowerPoint document and select the slide where the first graph should be inserted.");
        }
      }

      var graphExportOptions = ClipboardRenderingOptions.GetGraphExportOptionsFromCopyPageOptions();
      int downCounter = list.Count() - 1;
      foreach (var graph in list)
      {
        // before inserting the image, make a copy of the current slide to keep its content
        // this is not neccessary for the last slide
        dynamic? /*Slide*/ newSlide = null;
        if (downCounter > 0)
        {
          /*Slide*/
          newSlide = slideToInsertTo!.Duplicate()[1];
        }

        if (forceUsingDropFile)
        {
          // Export graph to image file
          var tempImageDropFileName = Path.GetTempFileName() + graphExportOptions.GetDefaultFileNameExtension();
          using (Stream myStream = new FileStream(tempImageDropFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) // we need FileAccess.ReadWrite when exporting to EMF/WMF format
          {
            var exporter = Current.ProjectService.GetProjectItemImageExporter(graph);
            exporter?.ExportAsImageToStream(graph, graphExportOptions, myStream);
            myStream.Close();
          } // end openfile ok


          // Insert the image into the active slide
          slideToInsertTo!.Shapes.AddPicture(tempImageDropFileName,
             0, // Microsoft.Office.Core.MsoTriState.msoFalse,
             -1, // Microsoft.Office.Core.MsoTriState.msoTrue,
              0, 0, -1, -1); // Position (0,0) left upper edge; size (-1,-1) to keep original size

          // Delete the temporary image file after use
          File.Delete(tempImageDropFileName);
        }
        else // use the clipboard
        {
          if (Current.ComManager is null)
          {
            throw new InvalidOperationException("COM Manager not available.");
          }

          // Copy graph to clipboard
          var dataObject = Current.ComManager.GetDocumentsDataObjectForDocument(graph);

          if (dataObject is not null)
          {
            System.Windows.Clipboard.SetDataObject(dataObject);

            // Paste the image from clipboard into the active slide
            slideToInsertTo!.Shapes.Paste();
          }
        }

        slideToInsertTo = newSlide;
        --downCounter;
      }
    }
  }
}
