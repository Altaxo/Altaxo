// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	/// <summary>
	/// External language bindings may choose to extend this class.
	/// It makes things a bit easier.
	/// </summary>
	[XmlNodeNameAttribute("Configuration")]
	public abstract class AbstractProjectConfiguration : AbstractConfiguration
	{
		[XmlNodeName("Output")]
		protected class OutputConfiguration
		{
			[XmlAttribute("directory")]
			[ConvertToRelativePath()]
			public string Directory = "." + Path.DirectorySeparatorChar.ToString();
			
			[XmlAttribute("assembly")]
			public string Assembly = "a";
			
			[XmlAttribute("executeScript")]
			[ConvertToRelativePath()]
			public string ExecuteScript = String.Empty;
			
			[XmlAttribute("executeBeforeBuild")]
			[ConvertToRelativePath()]
			public string ExecuteBeforeBuild = String.Empty;
			
			[XmlAttribute("executeAfterBuild")]
			[ConvertToRelativePath()]
			public string ExecuteAfterBuild = String.Empty;
		}
		
		[XmlAttribute("runwithwarnings")]
		protected bool runWithWarnings = false;
		
		protected OutputConfiguration outputConfiguration = new OutputConfiguration();
		
		public virtual string OutputDirectory {
			get {
				return outputConfiguration.Directory;
			}
			set {
				outputConfiguration.Directory = value;
			}
		}
		
		public virtual string OutputAssembly {
			get {
				return outputConfiguration.Assembly;
			}
			set {
				outputConfiguration.Assembly = value;
			}
		}
		
		public virtual string ExecuteScript {
			get {
				return outputConfiguration.ExecuteScript;
			}
			set {
				outputConfiguration.ExecuteScript = value;
			}
		}
		
		public virtual string ExecuteBeforeBuild {
			get {
				return outputConfiguration.ExecuteBeforeBuild;
			}
			set {
				outputConfiguration.ExecuteBeforeBuild = value;
			}
		}
		
		public virtual string ExecuteAfterBuild {
			get {
				return outputConfiguration.ExecuteAfterBuild;
			}
			set {
				outputConfiguration.ExecuteAfterBuild = value;
			}
		}
		
		public virtual bool RunWithWarnings {
			get {
				return runWithWarnings;
			}
			set {
				runWithWarnings = value;
			}
		}
		
		public AbstractProjectConfiguration()
		{
		}
	}
}
