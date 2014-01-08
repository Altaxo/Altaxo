using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;  // For use of the GuidAttribute, ProgIdAttribute and ClassInterfaceAttribute.
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;


namespace Altaxo.Com
{
	using UnmanagedApi.Ole32;
	using UnmanagedApi.User32;

	public static class DataObjectHelper
	{
		public static short CF_EMBEDDEDOBJECT = User32Func.RegisterClipboardFormat(CFSTR.CFSTR_EMBEDDEDOBJECT);
		public static short CF_EMBEDSOURCE = User32Func.RegisterClipboardFormat(CFSTR.CFSTR_EMBEDSOURCE);
		public static short CF_LINKSOURCE = User32Func.RegisterClipboardFormat(CFSTR.CFSTR_LINKSOURCE);
		public static short CF_OBJECTDESCRIPTOR = User32Func.RegisterClipboardFormat(CFSTR.CFSTR_OBJECTDESCRIPTOR);
		public static short CF_LINKSRCDESCRIPTOR = User32Func.RegisterClipboardFormat(CFSTR.CFSTR_LINKSRCDESCRIPTOR);


		/// <summary>
		/// Creates a new stream and renders the moniker into this stream. This function is intended for use with GetData(), but (!) not with GetDataHere().
		/// </summary>
		/// <param name="tymed">The tymed.</param>
		/// <param name="moniker">The moniker to render into the stream.</param>
		/// <returns>Newly created stream with the moniker rendered into.</returns>
		public static IntPtr RenderMonikerToNewStream(TYMED tymed, IMoniker moniker)
		{
			System.Diagnostics.Debug.Assert(tymed == TYMED.TYMED_ISTREAM);
			IStream strm;
			int hr = Ole32Func.CreateStreamOnHGlobal(IntPtr.Zero, true, out strm);
			System.Diagnostics.Debug.Assert(hr == ComReturnValue.S_OK);
			SaveMonikerToStream(moniker, strm);
			return Marshal.GetIUnknownForObject(strm);  // Increments the reference count
		}

		public static void SaveMonikerToStream(IMoniker moniker, IStream strm)
		{
#if COMLOGGING
			Debug.ReportInfo("SaveMonikerToStream:{0}", GetDisplayName(moniker));
#endif
			int hr = Ole32Func.OleSaveToStream((IPersistStream)moniker, strm);
			System.Diagnostics.Debug.Assert(hr == ComReturnValue.S_OK);
		}

		public static string GetDisplayName(IMoniker m)
		{
			string s;
			IBindCtx bc = CreateBindCtx();
			m.GetDisplayName(bc, null, out s);
			Marshal.ReleaseComObject(bc);  // seems to be recommended
			return s;
		}

		public static IBindCtx CreateBindCtx()
		{
			IBindCtx bc;
			int rc = Ole32Func.CreateBindCtx(0, out bc);
			System.Diagnostics.Debug.Assert(rc == ComReturnValue.S_OK);
			return bc;
		}


		public static string FormatEtcToString(FORMATETC format)
		{
			return String.Format("({0}, {1}, {2})",
													 ClipboardFormatName(format.cfFormat),
													 (DVASPECT)format.dwAspect,
													 (TYMED)format.tymed);
		}

		public static string ClipboardFormatName(short format)
		{
			switch (format)
			{
				case CF.CF_TEXT:
					return "CF_TEXT";

				case CF.CF_BITMAP:
					return "CF_BITMAP";

				case CF.CF_METAFILEPICT:
					return "CF_METAFILEPICT";

				case CF.CF_SYLK:
					return "CF_SYLK";

				case CF.CF_DIF:
					return "CF_DIF";

				case CF.CF_TIFF:
					return "CF_TIFF";

				case CF.CF_OEMTEXT:
					return "CF_OEMTEXT";

				case CF.CF_DIB:
					return "CF_DIB";

				case CF.CF_PALETTE:
					return "CF_PALETTE";

				case CF.CF_PENDATA:
					return "CF_PENDATA";

				case CF.CF_RIFF:
					return "CF_RIFF";

				case CF.CF_WAVE:
					return "CF_WAVE";

				case CF.CF_UNICODETEXT:
					return "CF_UNICODETEXT";

				case CF.CF_ENHMETAFILE:
					return "CF_ENHMETAFILE";

				case CF.CF_HDROP:
					return "CF_HDROP";

				case CF.CF_LOCALE:
					return "CF_LOCALE";

				case CF.CF_MAX:
					return "CF_MAX";

				case CF.CF_OWNERDISPLAY:
					return "CF_OWNERDISPLAY";

				case CF.CF_DSPTEXT:
					return "CF_DSPTEXT";

				case CF.CF_DSPBITMAP:
					return "CF_DSPBITMAP";

				case CF.CF_DSPMETAFILEPICT:
					return "CF_DSPMETAFILEPICT";

				case CF.CF_DSPENHMETAFILE:
					return "CF_DSPENHMETAFILE";

				default:
					{
						StringBuilder formatName = new StringBuilder(100);
						if (User32Func.GetClipboardFormatName((uint)format, formatName, 100) != 0)
							return formatName.ToString();
						else
							return String.Format("unknown ({0})", format);
					}
			}
		}

		#region Metafile rendering

		/// <summary>
		/// Creates a new metafile and renders some graphics into it.
		/// </summary>
		/// <param name="grfxRef">The reference graphics context used to create the meta file.</param>
		/// <param name="docSizeX">The document size x (in points = 1/72 inch).</param>
		/// <param name="docSizeY">The document size y  (in points = 1/72 inch).</param>
		/// <param name="drawingRoutine">The drawing routine. The first argument is the graphics context of the meta file.</param>
		/// <returns>The newly created metafile</returns>
		public static Metafile RenderEnhMetafile(Graphics grfxRef, double docSizeX, double docSizeY, Action<Graphics> drawingRoutine)
		{
			grfxRef.PageUnit = GraphicsUnit.Point;
			IntPtr ipHdc = grfxRef.GetHdc();

			RectangleF metaFileBounds;
			metaFileBounds = new RectangleF(0, 0, (float)(docSizeX), (float)(docSizeY));
			System.Drawing.Imaging.Metafile mf = new System.Drawing.Imaging.Metafile(ipHdc, metaFileBounds, MetafileFrameUnit.Point);
			using (Graphics grfxMF = Graphics.FromImage(mf))
			{
				if (Environment.OSVersion.Version.Major < 6 || !mf.GetMetafileHeader().IsDisplay())
				{
					grfxMF.PageUnit = GraphicsUnit.Point;
					grfxMF.PageScale = 1; // that would not work properly (a bug?) in Windows Vista, instead we have to use the following:
				}
				else
				{
					grfxMF.PageScale = (float)(Math.Min(72.0f / grfxMF.DpiX, 72.0f / grfxMF.DpiY)); // this works in Vista with display mode
				}

				if (null != drawingRoutine)
					drawingRoutine(grfxMF);
			}

			grfxRef.ReleaseHdc(ipHdc);
			return mf;
		}

		/// <summary>
		/// Creates a new metafile and renders some graphics into it.
		/// </summary>
		/// <param name="docSizeX">The document size x (in points = 1/72 inch).</param>
		/// <param name="docSizeY">The document size y  (in points = 1/72 inch).</param>
		/// <param name="drawingRoutine">The drawing routine. The first argument is the graphics context of the meta file.</param>
		/// <returns>The handle to the newly created metafile.</returns>
		public static IntPtr RenderEnhMetaFile(double docSizeX, double docSizeY, Action<Graphics> drawingRoutine)
		{
			Metafile mf;
			using (var pd = new System.Drawing.Printing.PrintDocument())
			{
				using (var grfx = pd.PrinterSettings.CreateMeasurementGraphics())
				{
					mf = RenderEnhMetafile(grfx, docSizeX, docSizeY, drawingRoutine);
				}
			}
			return mf.GetHenhmetafile();
		}


		#endregion
	}
}
