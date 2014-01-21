using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Com
{
	using UnmanagedApi.Advapi32;

	public enum WOW_Mode
	{
		None,
		Reg64,
		Reg32
	}

	public static class RegistryHelper
	{
		public static RegistryKey CreateSubKey(this RegistryKey mainKey, string name, WOW_Mode mode)
		{
			SafeRegistryHandle resultingKey;

			RegResult regResult;

			RegSAM sam = RegSAM.Write | RegSAM.Read | RegSAM.QueryValue;
			if (mode == WOW_Mode.Reg32)
				sam |= RegSAM.WOW64_32Key;
			else if (mode == WOW_Mode.Reg64)
				sam |= RegSAM.WOW64_64Key;

			Advapi32Func.RegCreateKeyEx(
								mainKey.Handle,
								name,
								0,
								null,
								RegOption.NonVolatile,
								sam,
								IntPtr.Zero,
								out resultingKey,
								out regResult);

			return RegistryKey.FromHandle(resultingKey);
		}
	}
}