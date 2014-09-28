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
	using UnmanagedApi.Gdi32;
	using UnmanagedApi.GdiPlus;
	using UnmanagedApi.Kernel32;
	using UnmanagedApi.Ole32;
	using UnmanagedApi.User32;

	/// <summary>
	/// Designates which DC should be used to create a metafile.
	/// </summary>
	public enum UseMetafileDC
	{
		/// <summary>Use the screen device context to create a metafile.</summary>
		Screen,

		/// <summary>Use the printer context to create a metafile.</summary>
		Printer
	};

	/// <summary>
	/// Static helper class for clipboard communication.
	/// </summary>
	/// <remarks>
	/// MS Word/Powerpoint 2010 accepts both enhanced metafiles created with a printer context or created with the screen context. But when using
	/// in conjunction with an embedded object, they seem to calculate the size of the object wrong if the EnhMF was created with a screen device context.
	///
	/// LibreOffice works fine if the metafile is created with a screen device context. However, if the metafile is created with a printer device context,
	/// the picture is shown too small inside of the boundaries (the boundaries itself are OK).
	///
	/// </remarks>
	public static class DataObjectHelper
	{
		public static short CF_EMBEDDEDOBJECT = User32Func.RegisterClipboardFormat(CFSTR.CFSTR_EMBEDDEDOBJECT);
		public static short CF_EMBEDSOURCE = User32Func.RegisterClipboardFormat(CFSTR.CFSTR_EMBEDSOURCE);
		public static short CF_LINKSOURCE = User32Func.RegisterClipboardFormat(CFSTR.CFSTR_LINKSOURCE);
		public static short CF_OBJECTDESCRIPTOR = User32Func.RegisterClipboardFormat(CFSTR.CFSTR_OBJECTDESCRIPTOR);
		public static short CF_LINKSRCDESCRIPTOR = User32Func.RegisterClipboardFormat(CFSTR.CFSTR_LINKSRCDESCRIPTOR);

		/// <summary>
		/// Creates a new IStream as global data and renders data into it.
		/// </summary>
		/// <param name="tymed">The tymed. Used only to make sure we have the right tymed.</param>
		/// <param name="RenderToStreamProcedure">The render to stream procedure.</param>
		/// <returns>Global pointer to that stream.</returns>
		public static IntPtr RenderToNewStream(TYMED tymed, Action<IStream> RenderToStreamProcedure)
		{
			System.Diagnostics.Debug.Assert(tymed == TYMED.TYMED_ISTREAM);
			IStream strm;
			int hr = Ole32Func.CreateStreamOnHGlobal(IntPtr.Zero, true, out strm);
			System.Diagnostics.Debug.Assert(hr == ComReturnValue.S_OK);
			RenderToStreamProcedure(strm);
			return Marshal.GetIUnknownForObject(strm);  // Increments the reference count
		}

		public static IntPtr RenderToNewStream(TYMED tymed, Action<System.IO.Stream> RenderToStreamProcedure)
		{
			System.Diagnostics.Debug.Assert(tymed == TYMED.TYMED_ISTREAM);
			IStream strm;
			int hr = Ole32Func.CreateStreamOnHGlobal(IntPtr.Zero, true, out strm);
			System.Diagnostics.Debug.Assert(hr == ComReturnValue.S_OK);
			using (var strmWrapper = new ComStreamWrapper(strm, true))
			{
				RenderToStreamProcedure(strmWrapper);
			}
			return Marshal.GetIUnknownForObject(strm);  // Increments the reference count
		}

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
			ComDebug.ReportInfo("SaveMonikerToStream:{0}", GetDisplayName(moniker));
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

		public static string NormalStringToMonikerNameString(string rawString)
		{
			var stb = new StringBuilder(rawString.Length + 20);

			foreach (char c in rawString)
			{
				switch (c)
				{
					case '!':
						stb.Append("%21");
						break;

					case '%':
						stb.Append("%25");
						break;

					default:
						stb.Append(c);
						break;
				}
			}
			return stb.ToString();
		}

		public static string MonikerNameStringToNormalString(string rawString)
		{
			var stb = new StringBuilder(rawString.Length);

			int startIndex = 0;
			while (startIndex < rawString.Length)
			{
				int idx = rawString.IndexOf('%', startIndex);

				if (idx < 0) // no Escape char found -> we append the rest of the string
				{
					stb.Append(rawString.Substring(startIndex, rawString.Length - startIndex)); // no Escape sequence found -> we append the rest of the string
					break;
				}
				else
				{
					stb.Append(rawString.Substring(startIndex, idx - startIndex)); // possible escape sequence found, -> we append until (but not including) the escape char
					startIndex = idx;
				}

				int remainingChars = rawString.Length - idx;

				if (remainingChars < 3) // too few remaining chars
				{
					stb.Append(rawString.Substring(startIndex, remainingChars)); // append the rest of the string
					break;
				}

				string subString = rawString.Substring(idx, 3);
				switch (subString)
				{
					case "%21":
						stb.Append('!');
						startIndex += 3;
						break;

					case "%25":
						stb.Append('%');
						startIndex += 3;
						break;

					default:
						stb.Append(rawString[idx]);
						startIndex += 1;
						break;
				}
			}
			return stb.ToString();
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

				case CF.CF_DIBV5:
					return "CF_DIBV5";

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

		#region Bitmap rendering

		/// <summary>
		/// Renders the provided image in a windows Gdi bitmap (no transparency supported).
		/// </summary>
		/// <param name="tymed">The tymed.</param>
		/// <param name="imgToDraw">The img to draw.</param>
		/// <param name="pixelsX">The width of the bitmap (in pixels) to create.</param>
		/// <param name="pixelsY">The height of the bitmap (in pixels) to create.</param>
		/// <param name="backgroundOpaqueColor">Color that is used as fully opaque background color of the resulting bitmap. This ensures that the resulting bitmap does not contain transparent or semi transparent pixels.</param>
		/// <returns>Pointer to the bitmap. This is a Gdi object (TYMED_GDI).</returns>
		/// <remarks>Using System.Drawing.Bitmap.GetHbitmap() claims also to produce a Gdi bitmap, but this is not working for all clipboard client programs.</remarks>
		public static IntPtr RenderHBitmap(TYMED tymed, Image imgToDraw, int pixelsX, int pixelsY, Color backgroundOpaqueColor)
		{
			System.Diagnostics.Debug.Assert(tymed == TYMED.TYMED_GDI);

			IntPtr hDC = Gdi32Func.GetDC(IntPtr.Zero);
			IntPtr hMemDC = Gdi32Func.CreateCompatibleDC(hDC);
			IntPtr hBmp = Gdi32Func.CreateCompatibleBitmap(hDC, pixelsX, pixelsY);
			if (IntPtr.Zero != hBmp)
			{
				IntPtr hObj = Gdi32Func.SelectObject(hMemDC, hBmp);
				using (Graphics g = Graphics.FromHdc(hMemDC))
				{
					g.PageUnit = GraphicsUnit.Pixel;
					using (var brush = new SolidBrush(backgroundOpaqueColor))
					{
						g.FillRectangle(brush, 0, 0, pixelsX, pixelsY);
					}
					g.DrawImage(imgToDraw, 0, 0, pixelsX, pixelsY);
				}
				Gdi32Func.SelectObject(hMemDC, hObj);
			}
			Gdi32Func.DeleteDC(hMemDC);
			User32Func.ReleaseDC(IntPtr.Zero, hDC);
			return hBmp;
		}

		/// <summary>
		/// Renders a device independent bitmap to a HGLOBAL pointer
		/// </summary>
		/// <param name="imgToDraw">The image to draw.</param>
		/// <param name="pixelsX">The width in pixels of the bitmap.</param>
		/// <param name="pixelsY">The height in pixels of the bitmap.</param>
		/// <param name="backgroundOpaqueColor">Color that is used as fully opaque background color of the resulting bitmap. This ensures that the resulting bitmap does not contain transparent or semi transparent pixels.</param>
		/// <returns>Pointer to the DIB bitmap (TYMED.TYMED_HGLOBAL).</returns>
		public static IntPtr RenderDIBBitmapToHGLOBAL(Image imgToDraw, int pixelsX, int pixelsY, Color backgroundOpaqueColor)
		{
			Bitmap bitmap = imgToDraw as System.Drawing.Bitmap;
			if (null != bitmap && bitmap.PixelFormat == PixelFormat.Format24bppRgb)
			{
				bitmap = (System.Drawing.Bitmap)imgToDraw;
			}
			else
			{
				bitmap = new Bitmap(pixelsX, pixelsY, PixelFormat.Format24bppRgb);
				using (Graphics g = Graphics.FromImage(bitmap))
				{
					using (var brush = new SolidBrush(backgroundOpaqueColor))
					{
						g.FillRectangle(brush, 0, 0, pixelsX, pixelsY);
					}
					g.DrawImage(imgToDraw, 0, 0, pixelsX, pixelsY);
				}
			}

			var bmpStream = new System.IO.MemoryStream();
			// ImageFormat.MemoryBmp work (will save to PNG!).  Therefore use
			// BMP and strip header.
			bitmap.Save(bmpStream, ImageFormat.Bmp);
			byte[] bmpBytes = bmpStream.ToArray();

			int offset = Marshal.SizeOf(typeof(BITMAPFILEHEADER));
			IntPtr hdib = Kernel32Func.GlobalAlloc(GlobalAllocFlags.GHND, (int)(bmpStream.Length - offset));
			System.Diagnostics.Debug.Assert(hdib != IntPtr.Zero);
			IntPtr buf = Kernel32Func.GlobalLock(hdib);
			Marshal.Copy(bmpBytes, offset, buf, (int)bmpStream.Length - offset);
			Kernel32Func.GlobalUnlock(hdib);

			if (!object.ReferenceEquals(bitmap, imgToDraw))
				bitmap.Dispose();

			return hdib;
		}

		// when considering creating a DIBV5 bitmap, read the following:
		// https://groups.google.com/forum/#!msg/microsoft.public.dotnet.framework.drawing/0sSPCrzf8yE/WNEIU324YtwJ

		#endregion Bitmap rendering

		#region Metafile rendering

		public static int PointToHimetric(double pt)
		{
			int x = (int)Math.Round((pt / 72.0) * 2540);
			return x;
		}

		/// <summary>
		/// Creates a new windows metafile and renders some graphics into it.
		/// </summary>
		/// <param name="img">The image to render.</param>
		/// <param name="docSizeX">The document size x (in points = 1/72 inch).</param>
		/// <param name="docSizeY">The document size y  (in points = 1/72 inch).</param>
		/// <param name="useMetafileDC">Designates either to use a screen context or a printer context to create the metafile.</param>
		/// <returns>The newly created metafile picture (type: CF_MFPICT)</returns>
		public static IntPtr RenderWindowsMetafilePict(System.Drawing.Image img, double docSizeX, double docSizeY, UseMetafileDC useMetafileDC)
		{
			using (var enhancedMetafile = RenderEnhancedMetafile(img, docSizeX, docSizeY, useMetafileDC))
			{
				var hEmf = enhancedMetafile.GetHenhmetafile();
				return ConvertEnhancedMetafileToWindowsMetafilePict(hEmf, docSizeX, docSizeY);
			}
		}

		public static IntPtr RenderWindowsMetafilePict(double docSizeX, double docSizeY, Metafile enhMF)
		{
			var hEmf = enhMF.GetHenhmetafile();

			return ConvertEnhancedMetafileToWindowsMetafilePict(hEmf, docSizeX, docSizeY);
		}

		/// <summary>
		/// Converts an enhanced metafile to a windows metafile picture (CF_MFPICT) and returns the byte buffer of the windows metafile.
		/// </summary>
		/// <param name="hEmf">The handle to the enhanced metafile.</param>
		/// <returns>Byte buffer containing the converted windows meta file.</returns>
		public static byte[] ConvertEnhancedMetafileToWindowsMetafileBytes(IntPtr hEmf)
		{
			uint size = 0;
			byte[] buffer = null;

			for (; ; )
			{
				// Convert the EMF records to WMF records.
				// Determine the size of the buffer that will receive the converted records (first loop), and then fill the buffer (second loop).
				// EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault is neccessary for LibreOffice (when using EmfToWmfBitsFlags.EmfToWmfBitsFlagsEmbedEmf, LibreOffice shows graph too small)
				size = GdiPlusFunc.GdipEmfToWmfBits(hEmf, size, buffer, MappingMode.MM_ANISOTROPIC, EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
				if (null == buffer)
					buffer = new byte[size];
				else
					break;
			}

			return buffer;
		}

		public static byte[] StructureToByteArray(WmfPlaceableFileHeader str)
		{
			int size = Marshal.SizeOf(str);
			byte[] arr = new byte[size];
			IntPtr ptr = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(str, ptr, true);
			Marshal.Copy(ptr, arr, 0, size);
			Marshal.FreeHGlobal(ptr);
			return arr;
		}

		public static byte[] GetWmfPlaceableHeaderBytes(Altaxo.Graph.PointD2D docSize)
		{
			WmfPlaceableFileHeader header = new WmfPlaceableFileHeader();
			const ushort twips_per_inch = 1440;
			header.key = 0x9aC6CDD7;  // magic number
			header.hmf = 0;
			header.bboxLeft = 0;
			header.bboxRight = (ushort)(twips_per_inch * docSize.X / 72);
			header.bboxTop = 0;
			header.bboxBottom = (ushort)(twips_per_inch * docSize.Y / 72);
			header.inch = twips_per_inch;
			header.reserved = 0;

			// Calculate checksum for first 10 WORDs.
			ushort checksum = 0;
			byte[] header_bytes = StructureToByteArray(header);
			for (int i = 0; i < 10; i++)
				checksum ^= BitConverter.ToUInt16(header_bytes, i * 2);
			header.checksum = checksum;

			// Construct the file in-memory.
			header_bytes = StructureToByteArray(header);

			return header_bytes;
		}

		/// <summary>
		/// Converts an enhanced metafile to a windows metafile picture (CF_MFPICT).
		/// </summary>
		/// <param name="hEmf">The handle to the enhanced metafile.</param>
		/// <param name="docSizeX">The document size x in points.</param>
		/// <param name="docSizeY">The document size y in points.</param>
		/// <returns>Handle to a windows meta file picture (CF_MFPICT).</returns>
		public static IntPtr ConvertEnhancedMetafileToWindowsMetafilePict(IntPtr hEmf, double docSizeX, double docSizeY)
		{
			byte[] buffer = ConvertEnhancedMetafileToWindowsMetafileBytes(hEmf);

			// Get a handle to the converted metafile.
			IntPtr hmf = Gdi32Func.SetMetaFileBitsEx((uint)buffer.Length, buffer);

			// Convert the Metafile to a METAFILEPICT.
			IntPtr hMem = Kernel32Func.GlobalAlloc(GlobalAllocFlags.GHND, Marshal.SizeOf(typeof(METAFILEPICT)));
			var mfp = new METAFILEPICT()
			{
				mm = MappingMode.MM_ANISOTROPIC,
				xExt = PointToHimetric(docSizeX),
				yExt = PointToHimetric(docSizeY),
				hMF = hmf
			};

			Marshal.StructureToPtr(mfp, Kernel32Func.GlobalLock(hMem), false);
			Kernel32Func.GlobalUnlock(hMem);

			return hMem;
		}

		#endregion Metafile rendering

		#region Enhanced Metafile rendering

		/// <summary>
		/// Creates a new metafile and renders some graphics into it.
		/// </summary>
		/// <param name="img">The image to render.</param>
		/// <param name="docSizeX">The document size x (in points = 1/72 inch).</param>
		/// <param name="docSizeY">The document size y  (in points = 1/72 inch).</param>
		/// <param name="useMetafileDC">Designates either to use a screen context or a printer context to create the metafile.</param>
		/// <returns>The newly created metafile</returns>
		public static Metafile RenderEnhancedMetafile(System.Drawing.Image img, double docSizeX, double docSizeY, UseMetafileDC useMetafileDC)
		{
			switch (useMetafileDC)
			{
				case UseMetafileDC.Printer:
					{
						using (var pd = new System.Drawing.Printing.PrintDocument())
						{
							using (var grfx = pd.PrinterSettings.CreateMeasurementGraphics())
							{
								grfx.PageUnit = GraphicsUnit.Point;
								return RenderEnhancedMetafile(img, docSizeX, docSizeY, grfx);
							}
						}
					}

				case UseMetafileDC.Screen:
					{
						using (var grfx = Graphics.FromImage(img))
						{
							return RenderEnhancedMetafile(img, docSizeX, docSizeY, grfx);
						}
					}
				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Creates a new metafile and renders some graphics into it.
		/// </summary>
		/// <param name="bmp">The image to render.</param>
		/// <param name="docSizeX">The document size x (in points = 1/72 inch).</param>
		/// <param name="docSizeY">The document size y  (in points = 1/72 inch).</param>
		/// <param name="referenceGraphicsContext">Graphics context used to create the metafile. This can either be a screen context or a printer context.</param>
		/// <returns>The newly created metafile</returns>
		public static Metafile RenderEnhancedMetafile(System.Drawing.Image bmp, double docSizeX, double docSizeY, Graphics referenceGraphicsContext)
		{
			IntPtr ipHdc = referenceGraphicsContext.GetHdc();

			MetafileFrameUnit mfUnit = MetafileFrameUnit.GdiCompatible; // HIMETRIC
			double scale = 2540 / 72.0; // Point to HIMETRIC

			RectangleF metaFileBounds;
			metaFileBounds = new RectangleF(0, 0, (float)(docSizeX * scale), (float)(docSizeY * scale));
			System.Drawing.Imaging.Metafile mf = new System.Drawing.Imaging.Metafile(ipHdc, metaFileBounds, mfUnit, EmfType.EmfPlusDual);
			var metafileHeader = mf.GetMetafileHeader();
			using (Graphics grfxMF = Graphics.FromImage(mf))
			{
				if (metafileHeader.IsDisplay())
				{
					// Display units are pixel, thus we can calculate pixels directly from the metafile resolution
					float displX = (float)((docSizeX / 72.0) * metafileHeader.DpiX); // TODO check on a high resolution monitor, maybe it is simply docSizeX
					float displY = (float)((docSizeY / 72.0) * metafileHeader.DpiY); // dito
					grfxMF.DrawImage(bmp, 0, 0, displX, displY);
				}
				else // not display DC - thus it must be a printer DC
				{
					// if (Environment.OSVersion.Version.Major < 6) // in former times it was neccessary to treat Windows XP special

					if (grfxMF.PageUnit == GraphicsUnit.Display)
					{
						// Note: we have to compensate the scale with the dimensions of the image (for Word 2010 it is not neccessary, but it is neccessary for LibreOffice)
						// thus to be most robust, the image dimensions must be given in page units of the metafile, which is given by the metafile resolution

						double GCPageUnitsPerInch = 100; // for a printer metafile graphics context in display units, the page unit is 100 dpi (see GraphicsUnit documentation)
						double scaleX = GCPageUnitsPerInch / metafileHeader.DpiX; // or maybe grfxMF.DpiX; ???
						double scaleY = GCPageUnitsPerInch / metafileHeader.DpiY; // or maybe grfxMF.DpiY; ???
						grfxMF.ScaleTransform((float)(scaleX), (float)(scaleY));
						float displX = (float)((docSizeX / 72.0) * metafileHeader.DpiX); // calculates the dots (page unit) in x-direction
						float displY = (float)((docSizeY / 72.0) * metafileHeader.DpiY); // calculated the dots (page unit) in y-direction
						grfxMF.DrawImage(bmp, 0, 0, displX, displY);
					}
					else
					{
						throw new NotImplementedException("Page Unit is not Display. Please report this to the forum along with the circumstances (especially what printer is the default printer in your system");
					}
				}
			}

			referenceGraphicsContext.ReleaseHdc(ipHdc);
			return mf;
		}

		#endregion Enhanced Metafile rendering

		#region Dropfiles rendering

		/// <summary>
		/// Renders the file names in a DROPFILE structure that can be used along with the CF_DROP.
		/// </summary>
		/// <param name="fileNames">The file names.</param>
		/// <returns>A pointer to a global memory handle containing the DROPFILES structure with the file names appended behind.</returns>
		public static IntPtr RenderFiles(IEnumerable<string> fileNames)
		{
			// build array with zero terminated file names and double zero at the end
			var str = new List<byte>();
			var enc = System.Text.Encoding.Unicode;
			foreach (var fileName in fileNames)
			{
				str.AddRange(enc.GetBytes(fileName));
				str.AddRange(enc.GetBytes("\0")); // Terminate each fileName with a zero
			}
			str.AddRange(enc.GetBytes("\0")); // Terminate with an additional zero
			var byteData = str.ToArray();

			// Allocate and get pointer to global memory
			DROPFILES dropFiles = new DROPFILES();
			int totalLength = Marshal.SizeOf(dropFiles) + byteData.Length;
			var ipGlobal = Marshal.AllocHGlobal(totalLength);
			if (ipGlobal != IntPtr.Zero)
			{
				// Build DROPFILES structure in global memory.
				dropFiles.pFiles = Marshal.SizeOf(dropFiles);
				dropFiles.fWide = 1; // for unicode-encoding
				Marshal.StructureToPtr(dropFiles, ipGlobal, true);
				IntPtr ipNew = new IntPtr(ipGlobal.ToInt64() + Marshal.SizeOf(dropFiles));
				Marshal.Copy(byteData, 0, ipNew, byteData.Length);
			}
			return ipGlobal;
		}

		public static void SaveMetafileToDisk(IntPtr hEnhMetafile, string filename)
		{
			// Export metafile to an image file
			Gdi32Func.CopyEnhMetaFile(hEnhMetafile, filename);
		}

		#endregion Dropfiles rendering
	}
}