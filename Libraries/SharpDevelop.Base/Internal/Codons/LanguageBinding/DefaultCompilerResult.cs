// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Collections;
using System.CodeDom.Compiler;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	/// <summary>
	/// Default implementation of the ICompilerResult interface, this implementation
	/// should be sufficient for most language bindings.
	/// </summary>
	public class DefaultCompilerResult : ICompilerResult
	{
		CompilerResults compilerResults;
		string          compilerOutput;
		
		public CompilerResults CompilerResults {
			get {
				return compilerResults;
			}
		}
		
		public string CompilerOutput {
			get {
				return compilerOutput;
			}
		}
		
		public DefaultCompilerResult(CompilerResults compilerResults, string compilerOutput)
		{
			this.compilerResults = compilerResults;
			this.compilerOutput  = compilerOutput;
		}
	}
}
