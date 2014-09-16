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
using System.Linq;
using System.Text;

namespace Altaxo.UnmanagedApi.Gdi32
{
	public static class MappingMode
	{
		public const int MM_TEXT = 1;
		public const int MM_LOMETRIC = 2;
		public const int MM_HIMETRIC = 3;
		public const int MM_LOENGLISH = 4;
		public const int MM_HIENGLISH = 5;
		public const int MM_TWIPS = 6;
		public const int MM_ISOTROPIC = 7;
		public const int MM_ANISOTROPIC = 8;
	}

	public static class DeviceCap
	{
		public const int LOGPIXELSX = 88;
		public const int LOGPIXELSY = 90;

		public const int HORZSIZE = 4;
		public const int VERTSIZE = 6;
		public const int HORZRES = 8;
		public const int VERTRES = 10;
	}

	/// <summary>
	///     Specifies a raster-operation code. These codes define how the color data for the
	///     source rectangle is to be combined with the color data for the destination
	///     rectangle to achieve the final color.
	/// </summary>
	public enum TernaryRasterOperations : uint
	{
		/// <summary>dest = source</summary>
		SRCCOPY = 0x00CC0020,

		/// <summary>dest = source OR dest</summary>
		SRCPAINT = 0x00EE0086,

		/// <summary>dest = source AND dest</summary>
		SRCAND = 0x008800C6,

		/// <summary>dest = source XOR dest</summary>
		SRCINVERT = 0x00660046,

		/// <summary>dest = source AND (NOT dest)</summary>
		SRCERASE = 0x00440328,

		/// <summary>dest = (NOT source)</summary>
		NOTSRCCOPY = 0x00330008,

		/// <summary>dest = (NOT src) AND (NOT dest)</summary>
		NOTSRCERASE = 0x001100A6,

		/// <summary>dest = (source AND pattern)</summary>
		MERGECOPY = 0x00C000CA,

		/// <summary>dest = (NOT source) OR dest</summary>
		MERGEPAINT = 0x00BB0226,

		/// <summary>dest = pattern</summary>
		PATCOPY = 0x00F00021,

		/// <summary>dest = DPSnoo</summary>
		PATPAINT = 0x00FB0A09,

		/// <summary>dest = pattern XOR dest</summary>
		PATINVERT = 0x005A0049,

		/// <summary>dest = (NOT dest)</summary>
		DSTINVERT = 0x00550009,

		/// <summary>dest = BLACK</summary>
		BLACKNESS = 0x00000042,

		/// <summary>dest = WHITE</summary>
		WHITENESS = 0x00FF0062,

		/// <summary>
		/// Capture window as seen on screen.  This includes layered windows
		/// such as WPF windows with AllowsTransparency="true"
		/// </summary>
		CAPTUREBLT = 0x40000000
	}
}