using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.Com
{
	using UnmanagedApi.Gdi32;
	using UnmanagedApi.Kernel32;
	using UnmanagedApi.Ole32;
	using UnmanagedApi.User32;

	public class GraphDocumentComObjectRenderer : DataObjectBase
	{
		private GraphDocument _document;
		private GraphDocumentComObject _comObject;

		public GraphDocumentComObjectRenderer(GraphDocument doc, GraphDocumentComObject comObject)
		{
			_document = doc;
			_comObject = comObject;
		}

		protected override IList<Rendering> Renderings
		{
			get
			{
				List<Rendering> renderings = new List<Rendering>();

				// We add them in order of preference.  Embedding is best because user can resize and edit), 
				// then enhanced metafile mainly because it contains size information

				// Allows us to be embedded with an OLE container.
				// No callback because this should go via GetDataHere.
				// EMBEDSOURCE and OBJECTDESCRIPTOR should be placed after private data,
				// but before the presentations (Brockschmidt, Inside Ole 2nd ed. page 911)
				renderings.Add(new Rendering(DataObjectHelper.CF_EMBEDSOURCE, TYMED.TYMED_ISTORAGE, null));
				renderings.Add(new Rendering(DataObjectHelper.CF_OBJECTDESCRIPTOR, TYMED.TYMED_HGLOBAL, GraphDocumentDataObject.RenderObjectDescriptor));

				// Nice because it is resolution independent.
				renderings.Add(new Rendering((short)CF.CF_ENHMETAFILE, TYMED.TYMED_ENHMF, RenderEnhMetaFile));

				// And allow linking, where we have a moniker.  This is last because
				// it should not happen by default.
				if (_comObject != null && _comObject.Moniker != null)
				{
					renderings.Add(new Rendering(DataObjectHelper.CF_LINKSOURCE, TYMED.TYMED_ISTREAM, _comObject.RenderLink));
					renderings.Add(new Rendering(DataObjectHelper.CF_LINKSRCDESCRIPTOR, TYMED.TYMED_HGLOBAL, GraphDocumentDataObject.RenderObjectDescriptor));
				}

				return renderings;
			}
		}

		private IntPtr RenderEnhMetaFile(TYMED tymed)
		{
#if COMLOGGING
			Debug.ReportInfo("GraphDocumentDataObject.RenderEnhMetafile");
#endif

			var docSize = _document.Size;
			using (var bmp = Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderAsBitmap(_document, Brushes.Transparent, PixelFormat.Format32bppArgb, GraphExportArea.GraphSize, 300, 300))
			{
				return DataObjectHelper.RenderEnhMetaFile(docSize.X, docSize.Y,
				(grfx) =>
				{
					grfx.DrawImage(bmp, 0, 0);
				}
				);
			}
		}

		protected override ManagedDataAdviseHolder DataAdviseHolder
		{
			get { return null; }
		}

		protected override void InternalGetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
		{
			throw new NotImplementedException();
		}
	}
}