// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.SharpDevelop.Services
{
	public class MethodCall
	{
		public static MethodCall NoDebugInformation = new MethodCall("<no debug information>", String.Empty);
		public static MethodCall Unknown            = new MethodCall("<unknown>", String.Empty);
		
		string methodName;
		string methodLanguage;
		
		public string Name {
			get {
				return methodName;
			}
		}
		
		public string Language {
			get {
				return methodLanguage;
			}
		}
		
		
		public MethodCall(string methodName, string methodLanguage)
		{
			this.methodName = methodName;
			this.methodLanguage = methodLanguage;
		}
	}
}

