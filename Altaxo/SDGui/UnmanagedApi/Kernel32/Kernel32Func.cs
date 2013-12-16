using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.UnmanagedApi.Kernel32
{
	public static class Kernel32Func
	{
		[DllImport("kernel32.dll")]
		public static extern IntPtr GlobalLock(IntPtr hMem);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GlobalUnlock(IntPtr hMem);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GlobalAlloc([MarshalAs(UnmanagedType.U4)]GlobalAllocFlags uFlags, int dwBytes);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GlobalFree(IntPtr hMem);
	}
}