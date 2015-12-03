using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	/// <summary>
	/// Common expoert actions for all classes derived from GraphDocument.
	/// </summary>
	public static class GraphDocumentBaseExportActions
	{
		/// <summary>
		/// Renders the graph document as enhanced metafile image in vector format with the options given in <paramref name="renderingOptions"/>
		/// </summary>
		/// <param name="document">The graph document used.</param>
		/// <param name="renderingOptions">The embedded rendering export options.</param>
		/// <returns>The rendered enhanced metafile.</returns>
		public static System.Drawing.Imaging.Metafile RenderAsEnhancedMetafileVectorFormat(GraphDocumentBase document, EmbeddedObjectRenderingOptions renderingOptions)
		{
			if (document is Gdi.GraphDocument)
				return Gdi.GraphDocumentExportActions.RenderAsEnhancedMetafileVectorFormat((Gdi.GraphDocument)document, renderingOptions);
			else
				throw new NotImplementedException("Render as metafile is not supported for graph documents of type " + document.GetType());
		}

		/// <summary>
		/// Renders the graph document as bitmap with default PixelFormat.Format32bppArgb.
		/// </summary>
		/// <param name="document">The graph document used.</param>
		/// <param name="renderingOptions">The embedded rendering export options.</param>
		/// <param name="pixelFormat">The pixel format for the bitmap. Default is PixelFormat.Format32bppArgb.</param>
		/// <returns>The rendered enhanced metafile.</returns>
		public static System.Drawing.Bitmap RenderAsBitmap(GraphDocumentBase document, EmbeddedObjectRenderingOptions renderingOptions, PixelFormat pixelFormat)
		{
			if (document is Gdi.GraphDocument)
				return Gdi.GraphDocumentExportActions.RenderAsBitmap((Gdi.GraphDocument)document, renderingOptions, pixelFormat);
			else
				return Graph3D.GraphDocumentExportActions.RenderAsBitmap((Graph3D.GraphDocument)document, renderingOptions, pixelFormat);
		}
	}
}