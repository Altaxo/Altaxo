// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	/// <summary>
	/// The <code>ILanguageBinding</code> interface is the base interface
	/// of all language bindings avaiable.
	/// </summary>
	public interface ILanguageBinding
	{
		/// <returns>
		/// The language for this language binding.
		/// </returns>
		string Language {
			get;
		}
		
		/// <summary>
		/// This function executes a file, the filename is given by filename,
		/// the file was compiled by the compiler object before.
		/// </summary>
		void Execute(string fileName);
		
		/// <summary>
		/// This function executes a project, the project is given as a parameter,
		/// the project was compiled by the compiler object before.
		/// </summary>
		void Execute(IProject project);
		
		string GetCompiledOutputName(string fileName);
		
		string GetCompiledOutputName(IProject project);
		
		/// <returns>
		/// True, if this language binding can compile >fileName<
		/// </returns>
		bool CanCompile(string fileName);
		
		ICompilerResult CompileFile(string fileName);
		
		ICompilerResult CompileProject(IProject project);
		
		ICompilerResult RecompileProject(IProject project);
		
		/// <summary>
		/// Creates a IProject out of the given ProjetCreateInformation object.
		/// Each language binding must provide a representation of the project
		/// it 'controls'.
		/// </summary>
		IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions);
	}
}
