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
		/// <returns>The newly created metafile.</returns>
		public static System.Drawing.Imaging.Metafile RenderEnhMetafile(double docSizeX, double docSizeY, Action<Graphics> drawingRoutine)
		{
			Metafile mf;
			using (var pd = new System.Drawing.Printing.PrintDocument())
			{
				using (var grfx = pd.PrinterSettings.CreateMeasurementGraphics())
				{
					mf = RenderEnhMetafile(grfx, docSizeX, docSizeY, drawingRoutine);
				}
			}
			return mf;
		}

		/// <summary>
		/// Creates a new metafile and renders some graphics into it.
		/// </summary>
		/// <param name="docSizeX">The document size x (in points = 1/72 inch).</param>
		/// <param name="docSizeY">The document size y  (in points = 1/72 inch).</param>
		/// <param name="drawingRoutine">The drawing routine. The first argument is the graphics context of the meta file.</param>
		/// <returns>The handle to the newly created metafile.</returns>
		public static IntPtr RenderEnhMetafileIntPtr(double docSizeX, double docSizeY, Action<Graphics> drawingRoutine)
		{
			Metafile mf = RenderEnhMetafile(docSizeX, docSizeY, drawingRoutine);
			return mf.GetHenhmetafile();
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

		#endregion Dropfiles rendering
	}
}