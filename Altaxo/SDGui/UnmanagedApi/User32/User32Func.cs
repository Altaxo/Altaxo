using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.UnmanagedApi.User32
{
	public static class User32Func
	{
		// 'short' to agree with FORMATETC.cfFormat
		[DllImport("user32.dll", SetLastError = true)]
		public static extern short RegisterClipboardFormat(string lpszFormat);

		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("user32.dll")]
		public static extern int GetClipboardFormatName(uint format, [Out] StringBuilder
			 lpszFormatName, int cchMaxCount);
	}
}