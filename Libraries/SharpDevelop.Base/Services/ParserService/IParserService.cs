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
		IParseInformation ParseFile(string fileName, string fileContent, bool updateCommentTags);
		
		IParseInformation GetParseInformation(string fileName);
		
		IParser GetParser(string fileName);
		IExpressionFinder GetExpressionFinder(string fileName);
		
		// Default Parser Layer dependent functions
		IClass    GetClass(string typeName);
		IClass    GetClass(string typeName, bool caseSensitive);
		
		string[]  GetNamespaceList(string subNameSpace);
		string[]  GetNamespaceList(string subNameSpace, bool caseSensitive);
		
		ArrayList GetNamespaceContents(string subNameSpace);
		ArrayList GetNamespaceContents(string subNameSpace, bool caseSensitive);
		
		bool      NamespaceExists(string name);
		bool      NamespaceExists(string name, bool caseSensitive);
		
		string    SearchNamespace(string name, ICompilationUnit unit, int caretLine, int caretColumn);
		string    SearchNamespace(string name, ICompilationUnit unit, int caretLine, int caretColumn, bool caseSensitive);
		
		IClass    SearchType(string name, IClass curType, int caretLine, int caretColumn);
		IClass    SearchType(string name, IClass curType, int caretLine, int caretColumn, bool caseSensitive);
		
		IClass    SearchType(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn);
		IClass    SearchType(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn, bool caseSensitive);
		
		bool      IsClassInInheritanceTree(IClass possibleBaseClass, IClass c);
		bool      IsClassInInheritanceTree(IClass possibleBaseClass, IClass c, bool caseSensitive);
		
		Position  GetPosition(string fullMemberName);
		
		IClass    BaseClass(IClass curClass);
		IClass    BaseClass(IClass curClass, bool caseSensitive);
		
		IClass    GetInnermostClass(ICompilationUnit cu, int caretLine, int caretColumn);
		ClassCollection GetOuterClasses(ICompilationUnit cu, int caretLine, int caretColumn);

		
		bool      IsAccessible(IClass c, IDecoration member, IClass callingClass, bool isClassInInheritanceTree);
		bool      MustBeShown(IClass c, IDecoration member, IClass callingClass, bool showStatic, bool isClassInInheritanceTree);
		ArrayList ListMembers(ArrayList members, IClass curType, IClass callingClass, bool showStatic);
		
		
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
		ArrayList CtrlSpace(IParserService parserService, int caretLine, int caretColumn, string fileName);
		void AddReferenceToCompletionLookup(IProject project, ProjectReference reference);

		event ParseInformationEventHandler ParseInformationAdded;
		event ParseInformationEventHandler ParseInformationRemoved;
		event ParseInformationEventHandler ParseInformationChanged;
	}
}
