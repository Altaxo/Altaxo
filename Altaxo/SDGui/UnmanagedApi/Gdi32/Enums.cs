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
}