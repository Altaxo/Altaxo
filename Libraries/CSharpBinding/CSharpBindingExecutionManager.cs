// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.CodeDom.Compiler;
using System.Threading;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Services;

namespace CSharpBinding
{
	/// <summary>
	/// This class describes the main functionalaty of a language codon
	/// </summary>
	public class CSharpBindingExecutionManager
	{
		public void Execute(string filename, bool debug)
		{
			string exe = Path.ChangeExtension(filename, ".exe");
			DebuggerService debuggerService  = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			if (debug) {
				debuggerService.Start(exe, Path.GetDirectoryName(exe), "");
			} else {
				ProcessStartInfo psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c " + "\"" + exe + "\"" + " & pause");
				psi.WorkingDirectory = Path.GetDirectoryName(exe);
				psi.UseShellExecute = false;
				
				debuggerService.StartWithoutDebugging(psi);
			}
		}
		
		public void Execute(IProject project, bool debug)
		{
			CSharpCompilerParameters parameters = (CSharpCompilerParameters)project.ActiveConfiguration;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			string directory = fileUtilityService.GetDirectoryNameWithSeparator(((CSharpCompilerParameters)project.ActiveConfiguration).OutputDirectory);
			string exe = ((CSharpCompilerParameters)project.ActiveConfiguration).OutputAssembly + ".exe";
			string args = ((CSharpCompilerParameters)project.ActiveConfiguration).CommandLineParameters;
			
			
			bool customStartup = false;
			ProcessStartInfo psi;
			if (parameters.ExecuteScript != null && parameters.ExecuteScript.Length > 0) {
				customStartup = true;
				psi = new ProcessStartInfo("\"" + parameters.ExecuteScript + "\"", args);
			} else {
				if (parameters.CompileTarget == CompileTarget.Library) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError("${res:BackendBindings.ExecutionManager.CantExecuteDLLError}");
					return;
				}
			
				string runtimeStarter = String.Empty;
				
				switch (parameters.NetRuntime) {
					case NetRuntime.Mono:
						runtimeStarter = "mono ";
						break;
					case NetRuntime.MonoInterpreter:
						runtimeStarter = "mint ";
						break;
				}
				
				if (parameters.CompileTarget != CompileTarget.WinExe && parameters.PauseConsoleOutput) {
					psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c " + runtimeStarter + "\"" + directory + exe + "\" " + args +  " & pause");
				} else {
					psi = new ProcessStartInfo(runtimeStarter + "\"" + directory + exe + "\"");
					psi.Arguments = args;
				}
			}
			
			psi.WorkingDirectory = Path.GetDirectoryName(directory);
			psi.UseShellExecute  =  false;
			DebuggerService debuggerService  = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			if (debug && !customStartup) {
				debuggerService.Start(Path.Combine(directory, exe), directory, args);
			} else {
				debuggerService.StartWithoutDebugging(psi);
			}
		}
	}
}
