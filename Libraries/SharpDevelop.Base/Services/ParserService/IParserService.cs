// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core.AddIns;

using SharpDevelop.Internal.Parser;
using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	public interface IParseInformation
	{
		ICompilationUnitBase ValidCompilationUnit {
			get;
		}
		ICompilationUnitBase DirtyCompilationUnit {
			get;
		}

		ICompilationUnitBase BestCompilationUnit {
			get;
		}

		ICompilationUnitBase MostRecentCompilationUnit {
			get;
		}
	}

	public interface IParserService
	{
		IParseInformation ParseFile(string fileName);
		IParseInformation ParseFile(string fileName, string fileContent);

		IParseInformation GetParseInformation(string fileName);

		IParser GetParser(string fileName);

		// Default Parser Layer dependent functions
		IClass    GetClass(string typeName);
		string[]  GetNamespaceList(string subNameSpace);
		ArrayList GetNamespaceContents(string subNameSpace);
		bool      NamespaceExists(string name);
		////////////////////////////////////////////

		/// <summary>
		/// Resolves an expression.
		/// The caretLineNumber and caretColumn is 1 based.
		/// </summary>
		ResolveResult Resolve(string expression,
		                      int caretLineNumber,
		                      int caretColumn,
		                      string fileName,
		                      string fileContent);

		void AddReferenceToCompletionLookup(IProject project, ProjectReference reference);

		event ParseInformationEventHandler ParseInformationAdded;
		event ParseInformationEventHandler ParseInformationRemoved;
		event ParseInformationEventHandler ParseInformationChanged;
	}
}
