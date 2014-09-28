#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;  // For use of the GuidAttribute, ProgIdAttribute and ClassInterfaceAttribute.
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.Com
{
	/// <summary>
	/// Helper class for rendering of Graphs embedded or linked in other documents. The rendering is done using options provided by <see cref="T:Altaxo.Graph.EmbeddedObjectRenderingOptions"/>.
	/// </summary>
	public static class EmbeddedGraphDocumentRenderingHelper
	{
		/// <summary>
		/// Gets the rendering options for the provided graph document.
		/// </summary>
		/// <param name="document">The graph document.</param>
		/// <returns>The rendering options for the graph document. If the graph itself has no rendering options stored (key: <see cref="Altaxo.Graph.Gdi.EmbeddedObjectRenderingOptions.PropertyKeyEmbeddedObjectRenderingOptions"/>, the hierarchy (folders, document, etc.) is walked down to find the rendering options.</returns>
		public static EmbeddedObjectRenderingOptions GetRenderingOptions(GraphDocument document)
		{
			var embe = Altaxo.PropertyExtensions.GetPropertyValue(document, EmbeddedObjectRenderingOptions.PropertyKeyEmbeddedObjectRenderingOptions, null);
			if (null != embe)
				return embe;

			var clip = Altaxo.PropertyExtensions.GetPropertyValue(document, ClipboardRenderingOptions.PropertyKeyClipboardRenderingOptions, null);
			if (null != clip)
				return clip;

			return new EmbeddedObjectRenderingOptions();
		}

		/// <summary>
		/// Renders the provided graph document to an enhanced metafile (TYMED_ENHMF).
		/// </summary>
		/// <param name="tymed">The tymed to check.</param>
		/// <param name="document">The graph document.</param>
		/// <returns>Pointer to the enhanced metafile (TYMED_ENHMF).</returns>
		public static IntPtr RenderEnhancedMetafile_TYMED_ENHMF(TYMED tymed, GraphDocument document)
		{
			System.Diagnostics.Debug.Assert(tymed == TYMED.TYMED_ENHMF);

			var renderingOptions = GetRenderingOptions(document);
			if (renderingOptions.RenderEnhancedMetafileAsVectorFormat)
			{
				var metafile = Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderAsEnhancedMetafileVectorFormat(document, renderingOptions);
				return metafile.GetHenhmetafile();
			}
			else
			{
				using (var bmp = Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderAsBitmap(document, renderingOptions, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
				{
					var docSize = document.Size * renderingOptions.OutputScalingFactor;
					return DataObjectHelper.RenderEnhancedMetafile(bmp, docSize.X, docSize.Y, UseMetafileDC.Printer).GetHenhmetafile();
				}
			}
		}

		/// <summary>
		/// Renders the provided graph document to an windows metafile picture (TYMED_MFPICT). Please not that this format does not support transparancy, thus the back color provided in the rendering options is used as ground brush first.
		/// </summary>
		/// <param name="tymed">The tymed to check.</param>
		/// <param name="document">The graph document.</param>
		/// <returns>Pointer to windows metafile picture (TYMED_MFPICT).</returns>
		public static IntPtr RenderWindowsMetafilePict_TYMED_MFPICT(TYMED tymed, GraphDocument document)
		{
			System.Diagnostics.Debug.Assert(tymed == TYMED.TYMED_MFPICT);

			var renderingOptions = GetRenderingOptions(document);
			using (var bmp = Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderAsBitmap(document, renderingOptions, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
			{
				var docSize = document.Size * renderingOptions.OutputScalingFactor;
				return DataObjectHelper.RenderWindowsMetafilePict(bmp, docSize.X, docSize.Y, UseMetafileDC.Printer);
			}
		}

		/// <summary>
		/// Renders the provided graph document to an Gdi bitmap (TYMED_GDI). Please not that this format does not support transparancy, thus the back color provided in the rendering options is used as ground brush first.
		/// </summary>
		/// <param name="tymed">The tymed to check.</param>
		/// <param name="document">The graph document.</param>
		/// <returns>Pointer to the Gdi bitmap (TYMED_GDI).</returns>
		public static IntPtr RenderAsGdiBitmap_TYMED_GDI(TYMED tymed, GraphDocument document)
		{
			System.Diagnostics.Debug.Assert(tymed == TYMED.TYMED_GDI);

			var renderingOptions = GetRenderingOptions(document);
			using (var bmp = Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderAsBitmap(document, renderingOptions, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
			{
				return DataObjectHelper.RenderHBitmap(tymed, bmp, bmp.Width, bmp.Height, renderingOptions.BackgroundColorForFormatsWithoutAlphaChannel);
			}
		}

		/// <summary>
		/// Renders the provided graph document to an device independent bitmap (TYMED_HGLOBAL). Please not that this format does not support transparancy, thus the back color provided in the rendering options is used as ground brush first.
		/// </summary>
		/// <param name="tymed">The tymed to check.</param>
		/// <param name="document">The graph document.</param>
		/// <returns>Pointer to the device independent bitmap (TYMED_HGLOBAL).</returns>
		public static IntPtr RenderAsDIBBitmap_TYMED_HGLOBAL(TYMED tymed, GraphDocument document)
		{
			System.Diagnostics.Debug.Assert(tymed == TYMED.TYMED_HGLOBAL);

			var renderingOptions = GetRenderingOptions(document);
			using (var bmp = Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderAsBitmap(document, renderingOptions, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
			{
				return DataObjectHelper.RenderDIBBitmapToHGLOBAL(bmp, bmp.Width, bmp.Height, renderingOptions.BackgroundColorForFormatsWithoutAlphaChannel);
			}
		}
	}
}