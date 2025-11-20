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
using System.Runtime.InteropServices;
using Altaxo.Graph.Gdi;

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// Exports graphs to an open Word document.
  /// </summary>
  public class GraphToOpenWordDocumentExporter
  {
    /// <summary>
    /// The prog identifier for word application.
    /// </summary>
    private const string ProgIdForWordApplication = "Word.Application";

    /// <summary>
    /// Exports the graphs to an open Word document, showing a dialog in case of errors.
    /// </summary>
    /// <param name="list">The list of graphs to export.</param>
    /// <param name="forceUsingDropFile">If set to <c>true</c>, a drop file is created for each graph and inserted into the document. If <c>false</c> (default), the clipboard method is used.</param>
    /// <param name="isInteractive">If set to <c>true</c>, an error box is shown if no open Word document is found. If <c>false</c>, an exception is thrown.</param>
    public static void PushGraphsToOpenWordDocument(IEnumerable<GraphDocumentBase> list, bool forceUsingDropFile, bool isInteractive)
    {
      dynamic? wordApp = GetActiveObject(ProgIdForWordApplication); // wordApp is Word.Application
      if (wordApp is null || wordApp.ActiveDocument is null)
      {
        if (isInteractive)
        {
          Current.Gui.ErrorMessageBox("No open Word document found. Please open a Word document and position the cursor where the graphs should be inserted.", "No Open Word Document");
          return;
        }
        else
        {
          throw new InvalidOperationException("No open Word document found. Please open a Word document and position the cursor where the graphs should be inserted.");
        }
      }

      // In order to avoid that selected text is overwritten, we need to collapse the selection to the end
      wordApp.Selection.Collapse(0); // 0 = wdCollapseEnd
      var graphExportOptions = ClipboardRenderingOptions.GetGraphExportOptionsFromCopyPageOptions();

      foreach (var graph in list)
      {
        if (forceUsingDropFile) // create a drop file for each graph and insert it into the document
        {
          // Temporary drop file name
          var tempImageDropFileName = Path.GetTempFileName() + graphExportOptions.GetDefaultFileNameExtension();
          using (Stream myStream = new FileStream(tempImageDropFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) // we need FileAccess.ReadWrite when exporting to EMF/WMF format
          {
            var exporter = Current.ProjectService.GetProjectItemImageExporter(graph);
            exporter?.ExportAsImageToStream(graph, graphExportOptions, myStream);
            myStream.Close();
          }
          // Insert image at current cursor position
          wordApp.Selection.InlineShapes.AddPicture(tempImageDropFileName, LinkToFile: false, SaveWithDocument: true);

          // Delete the temporary image file after use
          File.Delete(tempImageDropFileName);
        }

        else // use the clipboard method
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

            // Paste from clipboard to active Word document at cursor position
            wordApp.Selection.Paste();
          }
        }
      }
    }





    /// <summary>
    /// Gets the OLE object with a given prog identifier.
    /// </summary>
    /// <param name="progId">The OLE prog identifier.</param>
    /// <param name="throwOnError">if set to <c>true</c>, and exception is thrown if the object is not found.</param>
    /// <returns>The OLE object with the progId; if not found, the return value is null.</returns>
    public static object? GetActiveObject(string progId, bool throwOnError = false)
    {
      if (progId == null)
        throw new ArgumentNullException(nameof(progId));

      var hr = CLSIDFromProgIDEx(progId, out var clsid);
      if (hr < 0)
      {
        if (throwOnError)
          System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(hr);

        return null;
      }

      hr = GetActiveObject(clsid, IntPtr.Zero, out var obj);
      if (hr < 0)
      {
        if (throwOnError)
          System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(hr);

        return null;
      }
      return obj;
    }
    [DllImport("ole32")]
    private static extern int CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] string lpszProgID, out Guid lpclsid);

    [DllImport("oleaut32")]
    private static extern int GetActiveObject([MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, IntPtr pvReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);

  }
}
