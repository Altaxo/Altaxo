// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Diagnostics;

using ICSharpCode.SharpDevelop.Internal.Project;

namespace CSharpBinding
{
	public enum CompileTarget {
		Exe, 
		WinExe, 
		Library,
		Module
	};
	
	public enum CsharpCompiler {
		Csc,
		Mcs
	};
	
	public enum NetRuntime {
		Mono,
		MonoInterpreter,
		MsNet
	};
	
	/// <summary>
	/// This class handles project specific compiler parameters
	/// </summary>
	public class CSharpCompilerParameters : AbstractProjectConfiguration
	{
		[XmlNodeName("CodeGeneration")]
		protected class CodeGeneration 
		{
			[XmlAttribute("runtime")]
			public NetRuntime netRuntime         = NetRuntime.MsNet;
			
			[XmlAttribute("compiler")]
			public CsharpCompiler csharpCompiler = CsharpCompiler.Csc;
			
			[XmlAttribute("warninglevel")]
			public int  warninglevel       = 4;
			
			[XmlAttribute("includedebuginformation")]
			public bool debugmode          = true;
			
			[XmlAttribute("optimize")]
			public bool optimize           = true;
			
			[XmlAttribute("unsafecodeallowed")]
			public bool unsafecode         = false;
			
			[XmlAttribute("generateoverflowchecks")]
			public bool generateOverflowChecks = true;
			
			[XmlAttribute("mainclass")]
			public string         mainclass     = null;
			
			[XmlAttribute("target")]
			public CompileTarget  compiletarget = CompileTarget.Exe;
			
			[XmlAttribute("definesymbols")]
			public string         definesymbols = String.Empty;
			
			[XmlAttribute("generatexmldocumentation")]
			public bool generateXmlDocumentation = false;
			
			[ConvertToRelativePathAttribute()]
			[XmlAttribute("win32Icon")]
			public string         win32Icon     = String.Empty;
		}
		
		[XmlNodeName("Execution")]
		protected class Execution
		{
			[XmlAttribute("commandlineparameters")]
			public string  commandLineParameters = String.Empty;
			
			[XmlAttribute("consolepause")]
			public bool    pauseconsoleoutput = true;
		}
		
		protected CodeGeneration codeGeneration = new CodeGeneration();
		protected Execution      execution      = new Execution();
		
		public CsharpCompiler CsharpCompiler {
			get {
				return codeGeneration.csharpCompiler;
			}
			set {
				codeGeneration.csharpCompiler = value;
			}
		}
		
		public NetRuntime NetRuntime {
			get {
				return codeGeneration.netRuntime;
			}
			set {
				codeGeneration.netRuntime = value;
			}
		}
		
		public bool GenerateXmlDocumentation {
			get {
				return codeGeneration.generateXmlDocumentation;
			}
			set {
				codeGeneration.generateXmlDocumentation = value;
			}
		}
		
		public string Win32Icon {
			get {
				return codeGeneration.win32Icon;
			}
			set {
				codeGeneration.win32Icon = value;
			}
		}
		
		public string DefineSymbols {
			get {
				return codeGeneration.definesymbols;
			}
			set {
				codeGeneration.definesymbols = value;
			}
		}
		
		public string CommandLineParameters {
			get {
				return execution.commandLineParameters;
			}
			set {
				execution.commandLineParameters = value;
			}
		}
		
		public int WarningLevel {
			get {
				return codeGeneration.warninglevel;
			}
			set {
				codeGeneration.warninglevel = value;
			}
		}
		
		public bool PauseConsoleOutput {
			get {
				return execution.pauseconsoleoutput;
			}
			set {
				execution.pauseconsoleoutput = value;
			}
		}
		
		public bool Debugmode {
			get {
				return codeGeneration.debugmode;
			}
			set {
				codeGeneration.debugmode = value;
			}
		}
		
		public bool Optimize {
			get {
				return codeGeneration.optimize;
			}
			set {
				codeGeneration.optimize = value;
			}
		}
		
		public bool UnsafeCode {
			get {
				return codeGeneration.unsafecode;
			}
			set {
				codeGeneration.unsafecode = value;
			}
		}
		
		public bool GenerateOverflowChecks {
			get {
				return codeGeneration.generateOverflowChecks;
			}
			set {
				codeGeneration.generateOverflowChecks = value;
			}
		}
		
		public string MainClass {
			get {
				return codeGeneration.mainclass;
			}
			set {
				codeGeneration.mainclass = value;
			}
		}
		
		public CompileTarget CompileTarget {
			get {
				return codeGeneration.compiletarget;
			}
			set {
				codeGeneration.compiletarget = value;
			}
		}
		
		public CSharpCompilerParameters()
		{
		}
		public CSharpCompilerParameters(string name)
		{
			this.name = name;
		}
	}
}
