using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.AutoUpdates
{
	public static class NetFrameworkVersionDetermination
	{
		public static bool IsVersionInstalled(string version)
		{
			version = version.ToUpperInvariant();

			switch (version)
			{
				case "4.0":
					return true; // this program is compiled using Framework 4.0 so it is installed.

				case "4.5":
					return IsVersion45Installed();

				case "4.5.1":
					return IsVersion451Installed();

				case "4.5.2":
					return IsVersion452Installed();

				case "4.6":
					return IsVersion46Installed();

				case "4.6.1":
					return IsVersion461Installed();

				case "4.6.2":
					return IsVersion462Installed();

				default:
					return false;
			}
		}

		public static int? GetFramework45ReleaseNumber()
		{
			try
			{
				using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
					 RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"))
				{
					int releaseKey = (int)ndpKey.GetValue("Release");
					return releaseKey;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static bool IsVersion45Installed()
		{
			var release = GetFramework45ReleaseNumber();
			if (!release.HasValue)
				return false;
			else
				return true;
		}

		public static bool IsVersion451Installed()
		{
			var release = GetFramework45ReleaseNumber();
			if (!release.HasValue)
				return false;
			else
				return release >= 378675;
		}

		public static bool IsVersion452Installed()
		{
			var release = GetFramework45ReleaseNumber();
			if (!release.HasValue)
				return false;
			else
				return release >= 379893;
		}

		public static bool IsVersion46Installed()
		{
			var release = GetFramework45ReleaseNumber();
			if (!release.HasValue)
				return false;
			else
				return release >= 393273;
		}

		public static bool IsVersion461Installed()
		{
			var release = GetFramework45ReleaseNumber();
			if (!release.HasValue)
				return false;
			else
				return release >= 394254;
		}

		public static bool IsVersion462Installed()
		{
			var release = GetFramework45ReleaseNumber();
			if (!release.HasValue)
				return false;
			else
				return release >= 394802
;
		}
	}
}