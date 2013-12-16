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