// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;

using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Services;

namespace CSharpBinding
{
	/// <summary>
	/// This class controls the compilation of C Sharp files and C Sharp projects
	/// </summary>
	public class CSharpBindingCompilerManager
	{	
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		// we have 2 formats for the error output the csc gives :
		readonly static Regex normalError  = new Regex(@"(?<file>.*)\((?<line>\d+),(?<column>\d+)\):\s+(?<error>\w+)\s+(?<number>[\d\w]+):\s+(?<message>.*)", RegexOptions.Compiled);
		readonly static Regex generalError = new Regex(@"(?<error>.+)\s+(?<number>[\d\w]+):\s+(?<message>.*)", RegexOptions.Compiled);
		
		readonly static Regex monoNormalError = new Regex(@"(?<file>.*)\((?<line>\d+)\)\s+(?<error>\w+)\s+(?<number>[\d\w]+):\s+(?<message>.*)", RegexOptions.Compiled);
		
		public string GetCompiledOutputName(string fileName)
		{
			return Path.ChangeExtension(fileName, ".exe");
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			CSharpProject p = (CSharpProject)project;
			CSharpCompilerParameters compilerparameters = (CSharpCompilerParameters)p.ActiveConfiguration;
			string exe  = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + (compilerparameters.CompileTarget == CompileTarget.Library ? ".dll" : ".exe");
			return exe;
		}
		
		public bool CanCompile(string fileName)
		{
			return Path.GetExtension(fileName).ToUpper() == ".CS";
		}
		
		string GenerateOptions(CSharpCompilerParameters compilerparameters, string outputFileName)
		{
			StringBuilder sb = new StringBuilder();
			
			sb.Append("\"/out:");sb.Append(outputFileName);sb.Append('"');sb.Append(Environment.NewLine);
			
			sb.Append("/nologo");sb.Append(Environment.NewLine);
			sb.Append("/utf8output");sb.Append(Environment.NewLine);
			sb.Append("/w:");sb.Append(compilerparameters.WarningLevel);sb.Append(Environment.NewLine);
			
			if (compilerparameters.NoWarnings != null && compilerparameters.NoWarnings.Trim().Length > 0) {
				sb.Append("/nowarn:");sb.Append(compilerparameters.NoWarnings.Trim());sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.Debugmode) {
				sb.Append("/debug:+");sb.Append(Environment.NewLine);
				sb.Append("/debug:full");sb.Append(Environment.NewLine);
				sb.Append("/d:DEBUG");sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.Optimize) {
				sb.Append("/o");sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.Win32Icon != null && compilerparameters.Win32Icon.Trim().Length > 0 && File.Exists(compilerparameters.Win32Icon.Trim())) {
				sb.Append("\"/win32icon:");sb.Append(compilerparameters.Win32Icon.Trim());sb.Append("\"");sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.UnsafeCode) {
				sb.Append("/unsafe");sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.NoStdLib) {
				sb.Append("/nostdlib+");sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.DefineSymbols != null && compilerparameters.DefineSymbols.Trim().Length > 0) {
				sb.Append("/define:");sb.Append('"');sb.Append(compilerparameters.DefineSymbols);sb.Append('"');sb.Append(Environment.NewLine);
			}
			
			if (compilerparameters.MainClass != null && compilerparameters.MainClass.Trim().Length > 0) {
				sb.Append("/main:");sb.Append(compilerparameters.MainClass.Trim());sb.Append(Environment.NewLine);
			}
			
			switch (compilerparameters.CompileTarget) {
				case CompileTarget.Exe:
					sb.Append("/t:exe");
					break;
				case CompileTarget.WinExe:
					sb.Append("/t:winexe");
					break;
				case CompileTarget.Library:
					sb.Append("/t:library");
					break;
				default:
					throw new NotSupportedException("unknwon compile target:" + compilerparameters.CompileTarget);
			}
			sb.Append(Environment.NewLine);
			
			if (compilerparameters.GenerateXmlDocumentation) {
				sb.Append("\"/doc:");sb.Append(Path.ChangeExtension(outputFileName, ".xml"));sb.Append('"');sb.Append(Environment.NewLine);
			}
			
			return sb.ToString();
		}
		
		public ICompilerResult CompileFile(string filename, CSharpCompilerParameters compilerparameters)
		{
			string output = "";
			string error  = "";
			string exe = Path.ChangeExtension(filename, ".exe");
			if (compilerparameters.OutputAssembly != null && compilerparameters.OutputAssembly.Length > 0) {
				exe = compilerparameters.OutputAssembly;
			}
			string responseFileName = Path.GetTempFileName();
			
			StreamWriter writer = new StreamWriter(responseFileName);
			writer.WriteLine(GenerateOptions(compilerparameters, exe));
			writer.WriteLine('"' + filename + '"');
			writer.Close();
			
			string compilerName = compilerparameters.CsharpCompiler == CsharpCompiler.Csc ? GetCompilerName(compilerparameters.CSharpCompilerVersion) : "mcs";
			string outstr =  String.Concat(compilerName, " \"@", responseFileName, "\"");
			TempFileCollection  tf = new TempFileCollection ();
			Executor.ExecWaitWithCapture(outstr, tf, ref output, ref error);
			
			ICompilerResult result = ParseOutput(tf, output);
			
			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			WriteManifestFile(exe);
			return result;
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			CSharpProject p = (CSharpProject)project;
			CSharpCompilerParameters compilerparameters = (CSharpCompilerParameters)p.ActiveConfiguration;
			
			string exe              = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + (compilerparameters.CompileTarget == CompileTarget.Library ? ".dll" : ".exe");
			string responseFileName = Path.GetTempFileName();
			string optionString = compilerparameters.CsharpCompiler == CsharpCompiler.Csc ? "/" : "-";
			
			StreamWriter writer = new StreamWriter(responseFileName);
			if (compilerparameters.CsharpCompiler == CsharpCompiler.Csc) {
				writer.WriteLine(GenerateOptions(compilerparameters, exe));
				
//				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
//				ArrayList allProjects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
				
				// write references
				foreach (ProjectReference lib in p.ProjectReferences) {
					string fileName = lib.GetReferencedFileName(p);
					writer.WriteLine(String.Concat("\"/r:", fileName, "\""));
				}
				
				// write source files and embedded resources
				foreach (ProjectFile finfo in p.ProjectFiles) {
					if (finfo.Subtype != Subtype.Directory) {
						switch (finfo.BuildAction) {
							case BuildAction.Compile:
								writer.WriteLine(String.Concat('"', finfo.Name, '"'));
								break;
							case BuildAction.EmbedAsResource:
								writer.WriteLine(String.Concat("\"/res:", finfo.Name, "\""));
								break;
						}
					}
				}
			} else {
				writer.WriteLine(String.Concat("-o \"", exe, "\""));
				
				if (compilerparameters.UnsafeCode) {
					writer.WriteLine("--unsafe");
				}
				
				writer.WriteLine(String.Concat("--wlevel ", compilerparameters.WarningLevel));
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				ArrayList allProjects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
				
				foreach (ProjectReference lib in p.ProjectReferences) {
					string fileName = lib.GetReferencedFileName(p);
					writer.WriteLine(String.Concat("-r:\"", fileName, "\""));
				}
				
				switch (compilerparameters.CompileTarget) {
					case CompileTarget.Exe:
						writer.WriteLine("--target exe");
						break;
					case CompileTarget.WinExe:
						writer.WriteLine("--target winexe");
						break;
					case CompileTarget.Library:
						writer.WriteLine("--target library");
						break;
				}
				foreach (ProjectFile finfo in p.ProjectFiles) {
					if (finfo.Subtype != Subtype.Directory) {
						switch (finfo.BuildAction) {
							case BuildAction.Compile:
								writer.WriteLine(String.Concat('"', finfo.Name, '"'));
								break;
							
							case BuildAction.EmbedAsResource:
								writer.WriteLine(String.Concat("--linkres \"", finfo.Name, "\""));
								break;
						}
					}
				}
			}
			writer.Close();
			
			string output = String.Empty;
			string error  = String.Empty; 
			
			string compilerName = compilerparameters.CsharpCompiler == CsharpCompiler.Csc ? GetCompilerName(compilerparameters.CSharpCompilerVersion) : System.Environment.GetEnvironmentVariable("ComSpec") + " /c mcs";

			string outstr = String.Concat(compilerName, compilerparameters.NoConfig ? " /noconfig" : String.Empty, " \"@", responseFileName, "\"");
			TempFileCollection tf = new TempFileCollection();
			Executor.ExecWaitWithCapture(outstr,  tf, ref output, ref error);
			
			ICompilerResult result = ParseOutput(tf, output);
			project.CopyReferencesToOutputPath(false);
			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			if (compilerparameters.CompileTarget != CompileTarget.Library) {
				WriteManifestFile(exe);
			}
			return result;
		}
		
		// code duplication: see VB.NET backend : VBBindingCompilerManager
		void WriteManifestFile(string fileName)
		{
			string manifestFile = String.Concat(fileName, ".manifest");
			if (File.Exists(manifestFile)) {
				return;
			}
			StreamWriter sw = new StreamWriter(manifestFile);
			sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
			sw.WriteLine("");
			sw.WriteLine("<assembly xmlns=\"urn:schemas-microsoft-com:asm.v1\" manifestVersion=\"1.0\">");
			sw.WriteLine("	<dependency>");
			sw.WriteLine("		<dependentAssembly>");
			sw.WriteLine("			<assemblyIdentity");
			sw.WriteLine("				type=\"win32\"");
			sw.WriteLine("				name=\"Microsoft.Windows.Common-Controls\"");
			sw.WriteLine("				version=\"6.0.0.0\"");
			sw.WriteLine("				processorArchitecture=\"X86\"");
			sw.WriteLine("				publicKeyToken=\"6595b64144ccf1df\"");
			sw.WriteLine("				language=\"*\"");
			sw.WriteLine("			/>");
			sw.WriteLine("		</dependentAssembly>");
			sw.WriteLine("	</dependency>");
			sw.WriteLine("</assembly>");
			sw.Close();
		}
		
		string GetCompilerName(string compilerVersion)
		{
			string runtimeDirectory = Path.Combine(fileUtilityService.NETFrameworkInstallRoot, compilerVersion);
			if (compilerVersion.Length == 0 || compilerVersion == "Standard" || !Directory.Exists(runtimeDirectory)) {
				runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
			}
			return String.Concat('"', Path.Combine(runtimeDirectory, "csc.exe"), '"');
		}
		
		ICompilerResult ParseOutput(TempFileCollection tf, string file)
		{
			StringBuilder compilerOutput = new StringBuilder();
			
			StreamReader sr = File.OpenText(file);
			
			// skip fist whitespace line
			sr.ReadLine();
			
			CompilerResults cr = new CompilerResults(tf);
			
			while (true) {
				string curLine = sr.ReadLine();
				compilerOutput.Append(curLine);
				compilerOutput.Append('\n');
				if (curLine == null) {
					break;
				}
				curLine = curLine.Trim();
				if (curLine.Length == 0) {
					continue;
				}
				
				CompilerError error = new CompilerError();
				
				// try to match standard errors
				Match match = normalError.Match(curLine);
				if (match.Success) {
					error.Column      = Int32.Parse(match.Result("${column}"));
					error.Line        = Int32.Parse(match.Result("${line}"));
					error.FileName    = Path.GetFullPath(match.Result("${file}"));
					error.IsWarning   = match.Result("${error}") == "warning"; 
					error.ErrorNumber = match.Result("${number}");
					error.ErrorText   = match.Result("${message}");
				} else {
					match = monoNormalError.Match(curLine); // try to match standard mcs errors
					if (match.Success) {
						error.Column      = 0; // no column info :/
						error.Line        = Int32.Parse(match.Result("${line}"));
						error.FileName    = Path.GetFullPath(match.Result("${file}"));
						error.IsWarning   = match.Result("${error}") == "warning"; 
						error.ErrorNumber = match.Result("${number}");
						error.ErrorText   = match.Result("${message}");
					} else {
						match = generalError.Match(curLine); // try to match general csc errors
						if (match.Success) {
							error.IsWarning   = match.Result("${error}") == "warning"; 
							error.ErrorNumber = match.Result("${number}");
							error.ErrorText   = match.Result("${message}");
						} else { // give up and skip the line
							continue;
	//						error.IsWarning = false;
	//						error.ErrorText = curLine;
						}
					}
				}
				
				cr.Errors.Add(error);
			}
			sr.Close();
			return new DefaultCompilerResult(cr, compilerOutput.ToString());
		}
	}
}
