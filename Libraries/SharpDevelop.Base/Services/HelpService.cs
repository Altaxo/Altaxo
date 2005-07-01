// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Threading;
using Microsoft.Win32;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DynamicHelpService
	{
		DynamicHelpService() {}
		
		static DynamicHelpService()
		{
			ScanForLocalizedHelpPrefix();
		}
		
		static string HelpPrefix = "ms-help://MS.NETFrameworkSDK";
		
		static void ScanForLocalizedHelpPrefix()
		{
			string localHelp = String.Concat("0x", Thread.CurrentThread.CurrentCulture.LCID.ToString("X4"));
			RegistryKey helpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSDN\7.0\Help");
			if (helpKey == null) {
				return;
			}
			
			RegistryKey k = helpKey.OpenSubKey(localHelp);
			bool found = false;
			if (k != null) {
				string v = ScanSubKeys(k);
				if (v != null) {
					HelpPrefix = v;
					found = true;
				}
			}
			
			if (!found) {
				// use default english subkey
				k = helpKey.OpenSubKey("0x0409");
				string v = k != null ? ScanSubKeys(k) : null;
				if (v != null) {
					HelpPrefix = v;
				} else {
					string[] subKeys = helpKey.GetSubKeyNames();
					foreach (string subKey in subKeys) {
						if (subKey.StartsWith("0x")) {
							HelpPrefix = ScanSubKeys(helpKey.OpenSubKey(subKey));
							break;
						}
					}
				}
			}
		}
		
		static string ScanSubKeys(RegistryKey key)
		{
			if (key != null) {
				string[] subKeys = key.GetSubKeyNames();
				if (subKeys != null) {
					foreach (string subKey in subKeys) {
						RegistryKey sub = key.OpenSubKey(subKey);
						if (sub == null) {
							continue;
						}
						object o = sub.GetValue(null);
						if (o == null) {
							continue;
						}
						if (o.ToString().StartsWith("Microsoft .NET Framework SDK")) {
							return sub.GetValue("Filename").ToString();
						}
					}
				}
			}
			return null;
		}
		static string GetHelpString(string word)
		{
			int i = 0;
			while ((i = word.IndexOf('.')) != -1) {
				word = word.Remove(i,1);
			}
			return word;
		}
		
		public static void ShowHelpFromType(string type)
		{
			string url = String.Format("{0}/cpref/html/frlrf{1}ClassTopic.htm",
			                           HelpPrefix,
			                           GetHelpString(type));
			ShowHelpBrowser(url);
		}
		
		public static void ShowHelpFromType(string type, string member)
		{
			string url = String.Format("{0}/cpref/html/frlrf{1}Class{2}Topic.htm",
			                           HelpPrefix,
			                           GetHelpString(type),
			                           member);
			ShowHelpBrowser(url);
		}
		
		public static void ShowHelpBrowser(string url)
		{
			IFileService fileService = (IFileService)ServiceManager.Services.GetService(typeof(IFileService));
			fileService.OpenFile(url);
		}
	}
}
