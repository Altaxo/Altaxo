#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    AltaxoMarkdownEditing
//    Copyright (C) 2018 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//    See the LICENSE.md file in the root of the AltaxoMarkdownEditing library for more information.
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace Altaxo.Gui.Markdown
{
	public class Printing
	{
		/// <summary>
		/// Prints a flow document, showing a print dialog to the user.
		/// </summary>
		/// <param name="flowDocument">The flow document to print.</param>
		/// <param name="documentName">Name of the document (is used as name of the print job).</param>
		public static void PrintShowDialog(FlowDocument flowDocument, string documentName)
		{
			var printDlg = new PrintDialog()
			{
				UserPageRangeEnabled = true,
			};

			if (printDlg.ShowDialog() == true)
			{
				// TODO retrieve resolution from the print ticked, and then
				// render the images in the flow document with that resolution (instead of the default screen resolution)

				flowDocument.PageWidth = printDlg.PrintableAreaWidth;
				flowDocument.PageHeight = printDlg.PrintableAreaHeight;
				flowDocument.ColumnWidth = flowDocument.PageWidth;
				flowDocument.PagePadding = new System.Windows.Thickness(96, 48, 48, 48);

				var (fdoc, stream) = CreateFixedDocument(flowDocument);

				printDlg.PrintDocument(fdoc.DocumentPaginator, documentName);

				stream?.Dispose();
			}
		}

		/// <summary>
		/// Creates a fixed document from the given flow document.
		/// </summary>
		/// <param name="flowDocument">The flow document.</param>
		/// <returns>The fixed document and a stream. The stream must be left open as long as the fixed document is used, e.g. printed.</returns>
		private static (FixedDocument fixedDocument, Stream stream) CreateFixedDocument(FlowDocument flowDocument)
		{
			var paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
			var stream = new MemoryStream();
			var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
			var packUri = new Uri("pack://temp.xps");
			PackageStore.RemovePackage(packUri);
			PackageStore.AddPackage(packUri, package);
			var xps = new XpsDocument(package, CompressionOption.NotCompressed, packUri.ToString());
			XpsDocument.CreateXpsDocumentWriter(xps).Write(paginator);
			FixedDocument doc = xps.GetFixedDocumentSequence().References[0].GetDocument(true);
			// stream.Dispose();

			return (doc, stream);
		}
	}
}
