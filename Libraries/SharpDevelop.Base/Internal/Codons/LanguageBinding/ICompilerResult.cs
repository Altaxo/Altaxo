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
	/// Each language module which is capable of compiling source
	/// files gives back an ICompilerResult object which contains
	/// the output of the compiler and the compilerresults which contain
	/// all warnings and errors.
	/// </summary>
	public interface ICompilerResult
	{
		/// <summary>
		/// the compilerresults which contain all warnings and errors the compiler
		/// produces.
		/// </summary>
		CompilerResults CompilerResults {
			get;
		}
		
		/// <summary>
		/// The console output of the compiler as string.
		/// </summary>
		string CompilerOutput {
			get;
		}
	}
}
