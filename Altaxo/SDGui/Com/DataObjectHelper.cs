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
		/// Renders a bitmap to a Gdi bitmap in TYMED_GDI format. It is neccessary that the provided bitmap has a pixel format of PixelFormat.Format24bppRgb.
		/// Attention: do not use bitmap.GetHbitmap(Color backgroundColor). This overload gives not the right colors!
		/// </summary>
		/// <param name="bitmap">The provided bitmap. Must have PixelFormat.Format24bppRgb.</param>
		/// <returns>TYMED_GDI pointer to the Gdi bitmap.</returns>
		public static IntPtr RenderGdiBitmapToTYMED_GDI(Bitmap bitmap)
		{
			if (null == bitmap)
				throw new ArgumentNullException("bitmap");
			if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
				throw new ArgumentException(string.Format("bitmap must have PixelFormat.Format24bppRgb, but it has {0}", bitmap.PixelFormat));

			// Attention: the other overload GetHBitmap(Color c) does not work correctly, thus do not use it
			return bitmap.GetHbitmap(); // this is save because bitmap is ensured to be 24bppRGB
		}

		/// <summary>
		/// Renders a bitmap to a DIB bitmap in HGLOBAL format. It is neccessary that the provided bitmap has a pixel format of PixelFormat.Format24bppRgb.
		/// </summary>
		/// <param name="bitmap">The provided bitmap. Must have PixelFormat.Format24bppRgb.</param>
		/// <returns>HGLOBAL pointer to the DIB bitmap.</returns>
		public static IntPtr RenderDIBBitmapToHGLOBAL(Bitmap bitmap)
		{
			if (null == bitmap)
				throw new ArgumentNullException("bitmap");
			if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
				throw new ArgumentException(string.Format("bitmap must have PixelFormat.Format24bppRgb, but it has {0}", bitmap.PixelFormat));

			var bmpStream = new System.IO.MemoryStream();
			// ImageFormat.MemoryBmp work (will save to PNG!).  Therefore use BMP and strip header.
			bitmap.Save(bmpStream, ImageFormat.Bmp);
			byte[] bmpBytes = bmpStream.ToArray();

			int offset = Marshal.SizeOf(typeof(BITMAPFILEHEADER));
			IntPtr hdib = Kernel32Func.GlobalAlloc(GlobalAllocFlags.GHND, (int)(bmpStream.Length - offset));
			System.Diagnostics.Debug.Assert(hdib != IntPtr.Zero);
			IntPtr buf = Kernel32Func.GlobalLock(hdib);
			Marshal.Copy(bmpBytes, offset, buf, (int)bmpStream.Length - offset);
			Kernel32Func.GlobalUnlock(hdib);

			return hdib;
		}

		/// <summary>
		/// Renders a bitmap to a DIBV5 bitmap in HGLOBAL format. It is neccessary that the provided bitmap has a pixel format of PixelFormat.Format32bppRgb.
		/// </summary>
		/// <param name="bm">The provided bitmap. Must have PixelFormat.Format24bppRgb.</param>
		/// <returns>HGLOBAL pointer to the DIB bitmap.</returns>
		/// <remarks>
		/// See <see href="https://groups.google.com/forum/#!msg/microsoft.public.dotnet.framework.drawing/0sSPCrzf8yE/WNEIU324YtwJ"/> for the original source code and the discussion./>
		/// </remarks>
		public static IntPtr RenderDIBV5BitmapToHGLOBAL(Bitmap bm)
		{
			BitmapData bmData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);
			int bufferLen = (Marshal.SizeOf(typeof(BITMAPV5HEADER)) + bmData.Height * bmData.Stride);
			IntPtr hMem = Kernel32Func.GlobalAlloc(GlobalAllocFlags.GHND, bufferLen);
			IntPtr packedDIBV5 = Kernel32Func.GlobalLock(hMem);
			BITMAPV5HEADER bmi = (BITMAPV5HEADER)Marshal.PtrToStructure(packedDIBV5, typeof(BITMAPV5HEADER));

			bmi.bV5Size = (uint)Marshal.SizeOf(typeof(BITMAPV5HEADER));
			bmi.bV5Width = bmData.Width;
			bmi.bV5Height = -bmData.Height; // Top-down bitmap requires negative height
			bmi.bV5BitCount = 32;
			bmi.bV5Planes = 1;

			bmi.bV5Compression = (uint)BiCompression.BI_RGB;

			bmi.bV5XPelsPerMeter = (int)(bm.HorizontalResolution * (10000 / 254.0));
			bmi.bV5YPelsPerMeter = (int)(bm.VerticalResolution * (10000 / 254.0));
			bmi.bV5ClrUsed = 0;
			bmi.bV5ClrImportant = 0;
			bmi.bV5BlueMask = 0x000000FF;
			bmi.bV5GreenMask = 0x0000FF00;
			bmi.bV5RedMask = 0x00FF0000;
			bmi.bV5AlphaMask = 0xFF000000;
			bmi.bV5CSType = (uint)LcsCsType.LCS_WINDOWS_COLOR_SPACE;
			bmi.bV5GammaBlue = 0;
			bmi.bV5GammaGreen = 0;
			bmi.bV5GammaRed = 0;
			bmi.bV5ProfileData = 0;
			bmi.bV5ProfileSize = 0;
			bmi.bV5Reserved = 0;
			bmi.bV5Intent = (uint)LcsIntent.LCS_GM_IMAGES;
			bmi.bV5SizeImage = (uint)(bmData.Height * bmData.Stride);
			bmi.bV5Endpoints.ciexyzBlue.ciexyzX = bmi.bV5Endpoints.ciexyzBlue.ciexyzY = bmi.bV5Endpoints.ciexyzBlue.ciexyzZ = 0;
			bmi.bV5Endpoints.ciexyzGreen.ciexyzX = bmi.bV5Endpoints.ciexyzGreen.ciexyzY = bmi.bV5Endpoints.ciexyzGreen.ciexyzZ = 0;
			bmi.bV5Endpoints.ciexyzRed.ciexyzX = bmi.bV5Endpoints.ciexyzRed.ciexyzY = bmi.bV5Endpoints.ciexyzRed.ciexyzZ = 0;
			Marshal.StructureToPtr(bmi, packedDIBV5, false);

			int offsetBits = (int)bmi.bV5Size;
			IntPtr bits = IntPtr.Add(packedDIBV5, offsetBits);

			Kernel32Func.CopyMemory(bits, bmData.Scan0, (uint)(bmData.Height * bmData.Stride));

			bm.UnlockBits(bmData);

			Kernel32Func.GlobalUnlock(hMem);

			return hMem;
		}

		#endregion Bitmap rendering

		#region Metafile rendering

		/// <summary>
		/// Converts a length in points (1/72 inch) to a length in HIMETRIC units (1/100 mm).
		/// </summary>
		/// <param name="pt">The length in points.</param>
		/// <returns>Length in HIMETRIC units.</returns>
		public static int PointToHimetric(double pt)
		{
			int x = (int)Math.Round((pt / 72.0) * 2540);
			return x;
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

		/// <summary>
		/// Converts a structure to a byte array.
		/// </summary>
		/// <typeparam name="T">Type of structure.</typeparam>
		/// <param name="str">The structure instance.</param>
		/// <returns>The byte array that is the content of the provided structure.</returns>
		public static byte[] StructureToByteArray<T>(T str) where T : struct
		{
			int size = Marshal.SizeOf(str);
			byte[] arr = new byte[size];
			IntPtr ptr = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(str, ptr, true);
			Marshal.Copy(ptr, arr, 0, size);
			Marshal.FreeHGlobal(ptr);
			return arr;
		}

		/// <summary>
		/// Gets a windows metafile placeable header that can be preprended to the metafile bytes (only for files on disk).
		/// This header contains information about the bounding box of the metafile.
		/// </summary>
		/// <param name="docSize">Size of the document in points (1/72 inch).</param>
		/// <returns>The windows metafile placeable header as byte array.</returns>
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
		/// Converts an enhanced metafile to a windows metafile picture (CF_MFPICT). Please note that the provided enhanced metafile should neither contain
		/// transparancies nor splines. Thus it is best if the provided enhanced metafile contains only an embedded bitmap in 24bppRGB format.
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

		/// <summary>
		/// Saves a metafile to disk.
		/// </summary>
		/// <param name="hEnhMetafile">The handle to the enhanced metafile to save.</param>
		/// <param name="filename">The filename under which the metafile should be saved.</param>
		public static void SaveMetafileToDisk(IntPtr hEnhMetafile, string filename)
		{
			// Export metafile to an image file
			Gdi32Func.CopyEnhMetaFile(hEnhMetafile, filename);
		}

		#endregion Dropfiles rendering
	}
}