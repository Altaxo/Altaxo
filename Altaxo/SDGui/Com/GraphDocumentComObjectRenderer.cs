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

	public class GraphDocumentComObjectRenderer
	{
	

		private GraphDocument _document;
		private GraphDocumentComObject _comObject;

		public GraphDocumentComObjectRenderer(GraphDocument doc, GraphDocumentComObject comObject)
		{
			_document = doc;
			_comObject = comObject;
		}

		private IList<Rendering> Renderings
		{
			get
			{
				List<Rendering> renderings = new List<Rendering>();

				// We add them in order of preference.  Embedding is best fidelty
				// (because user can resize and edit), then metafile because it
				// will print well, and then bitmaps.

				// Allows us to be embedded with an OLE container.
				// No callback because this should go via GetDataHere.
				// EMBEDSOURCE and OBJECTDESCRIPTOR should be placed after private data,
				// but before the presentations (Brockschmidt, Inside Ole 2nd ed. page 911)
				AddFormat(DataObjectHelper.CF_EMBEDSOURCE, TYMED.TYMED_ISTORAGE, null, renderings);

				AddFormat(DataObjectHelper.CF_OBJECTDESCRIPTOR, TYMED.TYMED_HGLOBAL, GraphDocumentDataObject.RenderObjectDescriptor, renderings);

				// Nice because it is resolution independent.
				AddFormat((short)CF.CF_ENHMETAFILE, TYMED.TYMED_ENHMF, RenderEnhMetaFile3, renderings);

				// Nice because it is resolution independent.
				AddFormat(CF.CF_METAFILEPICT, TYMED.TYMED_MFPICT, RenderMetaFile, renderings);

				// Nice because it lets us paste into e-mail, etc.
				AddFormat(CF.CF_DIB, TYMED.TYMED_HGLOBAL, RenderDIB, renderings);

				// Because it is an easy format to support.
				AddFormat(CF.CF_BITMAP, TYMED.TYMED_GDI, RenderHBitmap, renderings);

				// And allow linking, where we have a moniker.  This is last because
				// it should not happen by default.
				if (_comObject != null && _comObject.Moniker != null)
				{
					AddFormat(DataObjectHelper.CF_LINKSOURCE, TYMED.TYMED_ISTREAM, _comObject.RenderLink, renderings);
					AddFormat(DataObjectHelper.CF_LINKSRCDESCRIPTOR, TYMED.TYMED_HGLOBAL, GraphDocumentDataObject.RenderObjectDescriptor, renderings);
				}

				return renderings;
			}
		}

		private void AddFormat(short format, TYMED tymed, RenderData renderer, List<Rendering> renderings)
		{
			Rendering rendering = new Rendering();
			rendering.format = new FORMATETC();
			rendering.format.cfFormat = format;
			rendering.format.ptd = IntPtr.Zero;
			rendering.format.dwAspect = DVASPECT.DVASPECT_CONTENT;
			rendering.format.lindex = -1 /*all*/;
			rendering.format.tymed = tymed;
			rendering.renderer = renderer;
			renderings.Add(rendering);
		}

		private Bitmap RenderBitmap(string mode)
		{
			float dpi_x = 300, dpi_y = 300;

			var docSize_pt = _document.Size;
			int ptx = (int)(dpi_x * docSize_pt.X / 72.0);
			int pty = (int)(dpi_y * docSize_pt.Y / 72.0);

			Bitmap bitmap = new Bitmap(ptx, pty);
			bitmap.SetResolution(dpi_x, dpi_y);
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				g.FillRectangle(Brushes.Yellow, 0, 0, ptx, pty);
				g.PageUnit = GraphicsUnit.World;
				_document.DoPaint(g, false);
			}

			return bitmap;
		}

		private IntPtr RenderDIB(TYMED tymed)
		{
			//Debug.Assert(tymed == TYMED.TYMED_HGLOBAL);
			Bitmap bitmap = RenderBitmap("DIB");
			MemoryStream mem = new MemoryStream();
			// ImageFormat.MemoryBmp work (will save to PNG!).  Therefore use
			// BMP and strip header.
			bitmap.Save(mem, ImageFormat.Bmp);
			byte[] bmp = mem.ToArray();

			int offset = Marshal.SizeOf(typeof(BITMAPFILEHEADER));
			IntPtr hdib = Kernel32Func.GlobalAlloc(GlobalAllocFlags.GHND, (int)(mem.Length - offset));
			System.Diagnostics.Debug.Assert(hdib != IntPtr.Zero);
			IntPtr buf = Kernel32Func.GlobalLock(hdib);
			Marshal.Copy(bmp, offset, buf, (int)mem.Length - offset);
			Kernel32Func.GlobalUnlock(hdib);

			return hdib;
		}

		private IntPtr RenderHBitmap(TYMED tymed)
		{
			//Debug.Assert(tymed == TYMED.TYMED_GDI);

			// Create an HBITMAP the hard way.
			IntPtr hDC = Gdi32Func.GetDC(IntPtr.Zero);
			IntPtr hMemDC = Gdi32Func.CreateCompatibleDC(hDC);
			int dpi_x = Gdi32Func.GetDeviceCaps(hDC, DeviceCap.LOGPIXELSX);
			int dpi_y = Gdi32Func.GetDeviceCaps(hDC, DeviceCap.LOGPIXELSY);

			var sizeF = _document.Size;
			int ptx = (int)(dpi_x * sizeF.X / 72.0);
			int pty = (int)(dpi_y * sizeF.Y / 72.0);

			IntPtr hBmp = Gdi32Func.CreateCompatibleBitmap(hDC, ptx, pty);
			if (IntPtr.Zero != hBmp)
			{
				IntPtr hObj = Gdi32Func.SelectObject(hMemDC, hBmp);
				using (Graphics g = Graphics.FromHdc(hMemDC))
				{
					g.FillRectangle(Brushes.YellowGreen, 0, 0, ptx, pty);
					g.PageUnit = GraphicsUnit.World;
					_document.DoPaint(g, false);
				}
				Gdi32Func.SelectObject(hMemDC, hObj);
			}
			Gdi32Func.DeleteDC(hMemDC);
			User32Func.ReleaseDC(IntPtr.Zero, hDC);
			return hBmp;
		}

		private void GetCoords(float dpi_x, float dpi_y, out Rectangle background, out Rectangle square)
		{
			// Convert HIMETRIC to pixels.
			var sizeF = _document.Size;
			// convert sizeF from points to pixel, using the dpi values
			int width = (int)(dpi_x * sizeF.X / 72.0);
			int height = (int)(dpi_y * sizeF.Y / 72.0);
			background = new Rectangle(0, 0, width, height);
			// Find the coordinates of a square centred within our render area.
			if (sizeF.X > sizeF.Y)
				square = new Rectangle((width - height) / 2, 0, height, height);
			else
				square = new Rectangle(0, (height - width) / 2, width, width);
		}

		private void RenderWin32(IntPtr hdc, int dpi_x, int dpi_y, string mode)
		{
			Rectangle background, square;
			GetCoords(dpi_x, dpi_y, out background, out square);
			IntPtr white_brush = IntPtr.Zero, blue_brush = IntPtr.Zero, red_pen = IntPtr.Zero,
						 hfont = IntPtr.Zero;
			try
			{
				// Warning: If you change this code then change RenderGraphics
				// too.
				RECT RECTbackground = new RECT();
				RECTbackground.Left = background.Left;
				RECTbackground.Top = background.Top;
				RECTbackground.Right = background.Right;
				RECTbackground.Bottom = background.Bottom;
				white_brush = Gdi32Func.CreateSolidBrush(0x00FFFFFF);
				Gdi32Func.FillRect(hdc, ref RECTbackground, white_brush);
				RECT RECTsquare = new RECT();
				RECTsquare.Left = square.Left;
				RECTsquare.Top = square.Top;
				RECTsquare.Right = square.Right;
				RECTsquare.Bottom = square.Bottom;
				blue_brush = Gdi32Func.CreateSolidBrush(0x00FF8080);
				Gdi32Func.FillRect(hdc, ref RECTsquare, blue_brush);
				red_pen = Gdi32Func.CreatePen(0/*solid*/, 3, 0x000000FF);
				Gdi32Func.SelectObject(hdc, red_pen);
				Gdi32Func.MoveToEx(hdc, RECTsquare.Left, RECTsquare.Top, IntPtr.Zero);
				Gdi32Func.LineTo(hdc, RECTsquare.Right, RECTsquare.Bottom);
				Gdi32Func.MoveToEx(hdc, RECTsquare.Left, RECTsquare.Bottom, IntPtr.Zero);
				Gdi32Func.LineTo(hdc, RECTsquare.Right, RECTsquare.Top);
				//	using (Graphics g = Graphics.FromHdc(hdc))
				//	{
				//		_document.DoPaint(g, false);
				//	}
			}
			finally
			{
				if (white_brush != IntPtr.Zero)
					Gdi32Func.DeleteObject(white_brush);
				if (blue_brush != IntPtr.Zero)
					Gdi32Func.DeleteObject(blue_brush);
				if (red_pen != IntPtr.Zero)
					Gdi32Func.DeleteObject(red_pen);
				if (hfont != IntPtr.Zero)
					Gdi32Func.DeleteObject(hfont);
			}
		}

		/// <summary>Create a memory metafile and return its handle.
		///
		/// This will fail to convert bezier curves because they are not
		/// supported in ye olde Windows metafile.  Unfortunately we must
		/// support this format because it is all that Word and Excel are
		/// willing to use!</summary>
		private IntPtr RenderMetaFile(TYMED tymed)
		{
			IntPtr hmf = IntPtr.Zero;
#if COMLOGGING
			Debug.ReportInfo("RenderMetaFile(TYMED tymed)");
#endif

			IntPtr hDC = Gdi32Func.CreateMetaFile(null);
			System.Diagnostics.Debug.Assert(hDC != IntPtr.Zero);

			// This is absolutely essential to the metafile so it
			// can be scaled in the clipboard and any destination
			// application.
			Gdi32Func.SetMapMode(hDC, MappingMode.MM_ANISOTROPIC);
			Gdi32Func.SetWindowOrgEx(hDC, 0, 0, IntPtr.Zero);
			int dpi = 96;
			var docSize = _document.Size;
			int ptx = (int)(dpi * docSize.X / 72.0);
			int pty = (int)(dpi * docSize.Y / 72.0);
			Gdi32Func.SetWindowExtEx(hDC, ptx, pty, IntPtr.Zero);

			// hier kommt die Rendering-Funktion (wenn sie denn kommt)
			RenderWin32(hDC, dpi, dpi, "Windows Metafile");

			hmf = Gdi32Func.CloseMetaFile(hDC);

			// Convert it to a METAFILEPICT.
			IntPtr hMem = Kernel32Func.GlobalAlloc(GlobalAllocFlags.GHND, Marshal.SizeOf(typeof(METAFILEPICT)));
			METAFILEPICT mfp = new METAFILEPICT();
			mfp.mm = MappingMode.MM_ANISOTROPIC;
			mfp.xExt = (int)(2540 * docSize.X / 72.0);
			mfp.yExt = (int)(2540 * docSize.Y / 72.0);

			mfp.hMF = hmf;
			Marshal.StructureToPtr(mfp, Kernel32Func.GlobalLock(hMem), false);
			Kernel32Func.GlobalUnlock(hMem);

			return hMem;
		}

		// This code basically copied from http://support.microsoft.com/kb/145999
		private IntPtr RenderEnhMetaFile(TYMED tymed)
		{
			IntPtr meta_dc;

			IntPtr screen_dc = Gdi32Func.GetDC(IntPtr.Zero);

			var docSize_pt = _document.Size;
			double inches_x = docSize_pt.X / 72.0;
			double inches_y = docSize_pt.Y / 72.0;

			/*
			 * Number of pixels per logical inch along the screen width.
			 * In a system with multiple display monitors, this value is the same for all monitors.
			 *
			 * Number of pixels per logical inch along the screen height.
			 * In a system with multiple display monitors, this value is the same for all monitors.
			 *
			 */
			int dpi_x = Gdi32Func.GetDeviceCaps(screen_dc, DeviceCap.LOGPIXELSX);
			int dpi_y = Gdi32Func.GetDeviceCaps(screen_dc, DeviceCap.LOGPIXELSY);

			//int t = Win32.GetDeviceCaps(screen_dc, (int)DeviceCap.TEXTCAPS);

			/*
			 * Width, in millimeters, of the physical screen.
			 *
			 * Height, in millimeters, of the physical screen.
			 *
			 */
			int iWidthMM = Gdi32Func.GetDeviceCaps(screen_dc, DeviceCap.HORZSIZE);
			int iHeightMM = Gdi32Func.GetDeviceCaps(screen_dc, DeviceCap.VERTSIZE);

			/*
			 * Width, in pixels, of the screen; or for printers,
			 * the width, in pixels, of the printable area of the page.
			 *
			 * Height, in raster lines, of the screen; or for printers,
			 * the height, in pixels, of the printable area of the page.
			 *
			 */
			int iWidthPels = Gdi32Func.GetDeviceCaps(screen_dc, DeviceCap.HORZRES);
			int iHeightPels = Gdi32Func.GetDeviceCaps(screen_dc, DeviceCap.VERTRES);

			// Extent in .01mm units (HIMETRIC).
			RECT extent = new RECT();
			extent.Left = 0;
			extent.Top = 0;

			// in .01-millimeter units
			extent.Right = (int)(docSize_pt.X * 2540 / 72.0);
			extent.Bottom = (int)(docSize_pt.Y * 2540 / 72.0);

#if COMLOGGING
			Debug.ReportInfo(String.Format("Rendering {0} x {1} himetric", extent.Right, extent.Bottom));
#endif

			try
			{
#if COMLOGGING
				Debug.ReportInfo(string.Format("EnhMetafile extend: {0} mm x {1} mm; pels_x_y: {2} X {3}, dpi_x_y: {4} X {5}", iWidthMM, iHeightMM, iWidthPels, iHeightPels, dpi_x, dpi_y));
#endif

				meta_dc = Gdi32Func.CreateEnhMetaFile(screen_dc, null, ref extent, "fiddling\0blue box\0\0");
			}
			finally
			{
				User32Func.ReleaseDC(IntPtr.Zero, screen_dc);
			}

			// Anisotropic mapping mode
			Gdi32Func.SetMapMode(meta_dc, MappingMode.MM_ANISOTROPIC);

			// Win32.SetWindowOrgEx(meta_dc, 100, 100, IntPtr.Zero);
			// Win32.SetViewportExtEx
			// Set the Windows extent
			Gdi32Func.SetWindowExtEx(meta_dc, (int)(inches_x * dpi_x), (int)(inches_y * dpi_y), IntPtr.Zero);

			// Set the viewport extent to reflect
			// dwInchesX" x dwInchesY" in device units
			Gdi32Func.SetViewportExtEx(meta_dc,
					(int)((float)inches_x * 25.4f * iWidthPels / iWidthMM),
					(int)((float)inches_y * 25.4f * iHeightPels / iHeightMM),
					IntPtr.Zero);

			/*
			 * Convert mm to px.
			 */
			int imgWidth = (int)(inches_x * dpi_x);
			int imgHeight = (int)(inches_y * dpi_y);

#if COMLOGGING
			Debug.ReportInfo("Rendering size (pixel)：{0} X {1}", imgWidth, imgHeight);
#endif

			using (Graphics g = Graphics.FromHdc(meta_dc))
			{
#if COMLOGGING
				Debug.ReportInfo("Image: {0} {1}", imgWidth, imgHeight);
#endif

				var bmp = Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderAsBitmap(_document, Brushes.White, PixelFormat.Format32bppArgb, GraphExportArea.GraphSize, 300, 300);
				g.DrawImage(bmp, 0f, 0f);
			}

			IntPtr hemf = Gdi32Func.CloseEnhMetaFile(meta_dc);

			ENHMETAHEADER emfh = EasyGetEnhMetaFileHeader(hemf);

			return hemf;
		}

		public static ENHMETAHEADER EasyGetEnhMetaFileHeader(IntPtr hemf)
		{
			int buf_size = Marshal.SizeOf(typeof(ENHMETAHEADER));
			IntPtr buf = Marshal.AllocCoTaskMem(buf_size);
			try
			{
				Gdi32Func.GetEnhMetaFileHeader(hemf, (uint)buf_size, buf);
				return (ENHMETAHEADER)Marshal.PtrToStructure(buf, typeof(ENHMETAHEADER));
			}
			finally
			{
				Marshal.FreeCoTaskMem(buf);
			}
		}


		public static Metafile RenderAsMetafile(Graphics grfxRef, GraphDocument doc)
		{
			grfxRef.PageUnit = GraphicsUnit.Point;
			IntPtr ipHdc = grfxRef.GetHdc();

			RectangleF metaFileBounds;
			metaFileBounds = new RectangleF(0, 0, (float)(doc.Size.X), (float)(doc.Size.Y));
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


				using (var bmp = Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderAsBitmap(doc, Brushes.White, PixelFormat.Format32bppArgb, GraphExportArea.GraphSize, 300, 300))
				{
					grfxMF.DrawImage(bmp, 0f, 0f);
				}
			}

			grfxRef.ReleaseHdc(ipHdc);
			return mf;
		}


		private IntPtr RenderEnhMetaFile3(TYMED tymed)
		{
			Metafile mf;
			using (var pd = new System.Drawing.Printing.PrintDocument())
			{
				using (var grfx = pd.PrinterSettings.CreateMeasurementGraphics())
				{
					mf = RenderAsMetafile(grfx, _document);
				}
			}
			return mf.GetHenhmetafile();
		}

	

		public int QueryGetData(ref FORMATETC format)
		{
#if COMLOGGING
			Debug.ReportInfo("QueryGetData, tymed={0}, aspect={1}", format.tymed, format.dwAspect);
#endif

			// We only support CONTENT aspect
			if ((DVASPECT.DVASPECT_CONTENT & format.dwAspect) == 0)
			{
				return ComReturnValue.DV_E_DVASPECT;
			}

			int ret = ComReturnValue.DV_E_TYMED;

			// Try to locate the data
			// TODO: The ret, if not S_OK, is only relevant to the last item
			foreach (var rendering in Renderings)
			{
				if ((rendering.format.tymed & format.tymed) > 0)
				{
					if (rendering.format.cfFormat == format.cfFormat)
					{
						// Found it, return S_OK;
						return ComReturnValue.S_OK;
					}
					else
					{
						// Found the medium type, but wrong format
						ret = ComReturnValue.DV_E_FORMATETC;
					}
				}
				else
				{
					// Mismatch on medium type
					ret = ComReturnValue.DV_E_TYMED;
				}
			}

#if COMLOGGING
			Debug.ReportInfo("QueryGetData returning {0:x}", ret);
#endif
			return ret;
		}

		public void GetData(ref FORMATETC format, out STGMEDIUM medium)
		{
			try
			{
				// Locate the data
				foreach (var rendering in Renderings)
				{
					if ((rendering.format.tymed & format.tymed) > 0
							&& rendering.format.dwAspect == format.dwAspect
							&& rendering.format.cfFormat == format.cfFormat
							&& rendering.renderer != null)
					{
						// Found it. Return a copy of the data.

						medium = new STGMEDIUM();
						medium.tymed = format.tymed;
						medium.unionmember = rendering.renderer(format.tymed);
						if (medium.tymed == TYMED.TYMED_ISTORAGE || medium.tymed == TYMED.TYMED_ISTREAM)
							medium.pUnkForRelease = Marshal.GetObjectForIUnknown(medium.unionmember);
						else
							medium.pUnkForRelease = null;
						return;
					}
				}
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("GetData occured an exception.", e);
#endif
				throw;
			}

#if COMLOGGING
			Debug.ReportInfo("-> DV_E_FORMATETC");
#endif
			medium = new STGMEDIUM();
			// Marshal.ThrowExceptionForHR(ComReturnValue.DV_E_FORMATETC);
		}

		public IEnumFORMATETC EnumFormatEtc()
		{
			return new EnumFORMATETC(Renderings);
		}

		/// <summary>
		/// Helps enumerate the formats available in our DataObject class.
		/// </summary>
		[ComVisible(true)]
		private class EnumFORMATETC : IEnumFORMATETC
		{
			// Keep an array of the formats for enumeration
			private IList<Rendering> renderings;

			// The index of the next item
			private int currentIndex = 0;

			/// <summary>
			/// Creates an instance from a list of key value pairs.
			/// </summary>
			/// <param name="storage">List of FORMATETC/STGMEDIUM key value pairs</param>
			internal EnumFORMATETC(IList<Rendering> renderings)
			{
				this.renderings = renderings;
			}

			#region IEnumFORMATETC Members

			/// <summary>
			/// Creates a clone of this enumerator.
			/// </summary>
			/// <param name="newEnum">When this function returns, contains a new instance of IEnumFORMATETC.</param>
			public void Clone(out IEnumFORMATETC newEnum)
			{
				EnumFORMATETC ret = new EnumFORMATETC(renderings);
				ret.currentIndex = currentIndex;
				newEnum = ret;
			}

			/// <summary>
			/// Retrieves the next elements from the enumeration.
			/// </summary>
			/// <param name="celt">The number of elements to retrieve.</param>
			/// <param name="rgelt">An array to receive the formats requested.</param>
			/// <param name="pceltFetched">An array to receive the number of element fetched.</param>
			/// <returns>If the fetched number of formats is the same as the requested number, S_OK is returned.
			/// There are several reasons S_FALSE may be returned: (1) The requested number of elements is less than
			/// or equal to zero. (2) The rgelt parameter equals null. (3) There are no more elements to enumerate.
			/// (4) The requested number of elements is greater than one and pceltFetched equals null or does not
			/// have at least one element in it. (5) The number of fetched elements is less than the number of
			/// requested elements.</returns>
			public int Next(int celt, FORMATETC[] rgelt, int[] pceltFetched)
			{
				// Start with zero fetched, in case we return early
				if (pceltFetched != null && pceltFetched.Length > 0)
					pceltFetched[0] = 0;

				// This will count down as we fetch elements
				int cReturn = celt;

				// Short circuit if they didn't request any elements, or didn't
				// provide room in the return array, or there are not more elements
				// to enumerate.
				if (celt <= 0 || rgelt == null || currentIndex >= renderings.Count)
					return 1; // S_FALSE

				// If the number of requested elements is not one, then we must
				// be able to tell the caller how many elements were fetched.
				if ((pceltFetched == null || pceltFetched.Length < 1) && celt != 1)
					return 1; // S_FALSE

				// If the number of elements in the return array is too small, we
				// throw. This is not a likely scenario, hence the exception.
				if (rgelt.Length < celt)
					throw new ArgumentException("The number of elements in the return array is less than the number of elements requested");

				// Fetch the elements.
				for (int i = 0; currentIndex < renderings.Count && cReturn > 0; i++, cReturn--, currentIndex++)
					rgelt[i] = renderings[currentIndex].format;

				// Return the number of elements fetched
				if (pceltFetched != null && pceltFetched.Length > 0)
					pceltFetched[0] = celt - cReturn;

				// cReturn has the number of elements requested but not fetched.
				// It will be greater than zero, if multiple elements were requested
				// but we hit the end of the enumeration.
				return (cReturn == 0) ? 0 : 1; // S_OK : S_FALSE
			}

			/// <summary>
			/// Resets the state of enumeration.
			/// </summary>
			/// <returns>S_OK</returns>
			public int Reset()
			{
				currentIndex = 0;
				return 0; // S_OK
			}

			/// <summary>
			/// Skips the number of elements requested.
			/// </summary>
			/// <param name="celt">The number of elements to skip.</param>
			/// <returns>If there are not enough remaining elements to skip, returns S_FALSE. Otherwise, S_OK is returned.</returns>
			public int Skip(int celt)
			{
				if (currentIndex + celt > renderings.Count)
					return 1; // S_FALSE

				currentIndex += celt;
				return 0; // S_OK
			}

			#endregion IEnumFORMATETC Members
		}

		public delegate IntPtr RenderData(TYMED tymed);

		public struct Rendering
		{
			public FORMATETC format;
			public RenderData renderer;
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
	}
}