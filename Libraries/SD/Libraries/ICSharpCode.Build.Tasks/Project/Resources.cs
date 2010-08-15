﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2754 $</version>
// </file>

using System;
using System.Reflection;

namespace ICSharpCode.Build.Tasks
{
	internal static class Resources
	{		
		public static string RunningCodeAnalysis {
			get { return GetTranslation("${res:ICSharpCode.Build.RunningCodeAnalysis}")
					?? "Running Code Analysis..."; }
		}
		
		public static string CannotFindFxCop {
			get { return GetTranslation("${res:ICSharpCode.Build.CannotFindFxCop}")
					?? "SharpDevelop cannot find FxCop. Please specify the MSBuild property 'FxCopDir'."; }
		}
		
		public static string CannotReadFxCopLogFile {
			get { return GetTranslation("${res:ICSharpCode.Build.CannotReadFxCopLogFile}")
					?? "Cannot read FxCop log file:"; }
		}
		
		static bool initialized;
		static Type resourceService;
		
		static string GetTranslation(string key)
		{
			if (!initialized) {
				initialized = true;
				try {
					resourceService = Type.GetType("ICSharpCode.Core.StringParser, ICSharpCode.Core");
				} catch {}
			}
			if (resourceService != null) {
				const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;
				return (string)resourceService.InvokeMember("Parse", flags, null, null, new object[] { key });
			} else {
				return null;
			}
		}
	}
}
