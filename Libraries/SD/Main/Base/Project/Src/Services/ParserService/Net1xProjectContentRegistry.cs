﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2138 $</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Project content registry for .NET 1.0 and .NET 1.1 assemblies.
	/// </summary>
	public abstract class Net1xProjectContentRegistry : ProjectContentRegistry
	{
		protected abstract string DotnetVersion { get; }
		
		public override IProjectContent Mscorlib {
			get {
				return GetProjectContentForReference("mscorlib", "mscorlib");
			}
		}
		
		protected override IProjectContent LoadProjectContent(string itemInclude, string itemFileName)
		{
			if (File.Exists(itemFileName)) {
				return base.LoadProjectContent(itemInclude, itemFileName);
			}
			string netPath = Path.Combine(FileUtility.NETFrameworkInstallRoot, DotnetVersion);
			if (File.Exists(Path.Combine(netPath, "mscorlib.dll"))) {
				string shortName = itemInclude;
				int pos = shortName.IndexOf(',');
				if (pos > 0)
					shortName = shortName.Substring(0, pos);
				
				if (File.Exists(Path.Combine(netPath, shortName + ".dll"))) {
					ReflectionProjectContent rpc = CecilReader.LoadAssembly(Path.Combine(netPath, shortName + ".dll"), this);
					if (rpc != null) {
						redirectedAssemblyNames.Add(shortName, rpc.AssemblyFullName);
					}
					return rpc;
				} else if (File.Exists(Path.Combine(netPath, shortName))) {
					// perhaps shortName includes file extension
					ReflectionProjectContent rpc = CecilReader.LoadAssembly(Path.Combine(netPath, shortName), this);
					if (rpc != null) {
						redirectedAssemblyNames.Add(Path.GetFileNameWithoutExtension(shortName), rpc.AssemblyFullName);
					}
					return rpc;
				}
			} else {
				string message = "Warning: Target .NET Framework version " + DotnetVersion + " is not installed." + Environment.NewLine;
				if (!TaskService.BuildMessageViewCategory.Text.Contains(message)) {
					TaskService.BuildMessageViewCategory.AppendText(message);
				}
			}
			return base.LoadProjectContent(itemInclude, itemFileName);
		}
	}
	
	public class Net10ProjectContentRegistry : Net1xProjectContentRegistry
	{
		protected override string DotnetVersion {
			get { return "v1.0.3705"; }
		}
	}
	public class Net11ProjectContentRegistry : Net1xProjectContentRegistry
	{
		protected override string DotnetVersion {
			get { return "v1.1.4322"; }
		}
	}
}
