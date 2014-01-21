using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.UnmanagedApi.Advapi32
{
	public static class Advapi32Func
	{
		[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int RegCreateKeyEx(
								[In] SafeRegistryHandle hKey,
								[In] string lpSubKey,
								[In, MarshalAs(UnmanagedType.U4)] int Reserved,
								[In] string lpClass,
								[In, MarshalAs(UnmanagedType.U4)] RegOption dwOptions,
								[In, MarshalAs(UnmanagedType.U4)] RegSAM samDesired,
								[In] IntPtr lpSecurityAttributes,
								[Out] out SafeRegistryHandle phkResult,
								[Out] out RegResult lpdwDisposition);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
		internal static extern int RegDeleteKey(SafeRegistryHandle hKey, String lpSubKey);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
		internal static extern int RegDeleteValue(SafeRegistryHandle hKey, String lpValueName);
	}
}