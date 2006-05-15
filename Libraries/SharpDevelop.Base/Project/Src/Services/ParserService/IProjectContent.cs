// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1117 $</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Xml;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Core
{
	public interface IProjectContent : IDisposable
	{
		XmlDoc XmlDoc {
			get;
		}
		
		ICollection<IClass> Classes {
			get;
		}
		
		ICollection<string> NamespaceNames {
			get;
		}
		
		ICollection<IProjectContent> ReferencedContents {
			get;
		}
		
		event EventHandler ReferencedContentsChanged;
		
		/// <summary>
		/// Gets the properties of the language this project content was written in.
		/// </summary>
		LanguageProperties Language {
			get;
		}
		
		/// <summary>
		/// Gets the default imports of the project content. Can return null.
		/// </summary>
		IUsing DefaultImports {
			get;
		}
		
		/// <summary>
		/// Gets the project for this project content. Returns null for reflection project contents.
		/// </summary>
		IProject Project {
			get;
		}
		
		string GetXmlDocumentation(string memberTag);
		
		void AddClassToNamespaceList(IClass addClass);
		void RemoveCompilationUnit(ICompilationUnit oldUnit);
		void UpdateCompilationUnit(ICompilationUnit oldUnit, ICompilationUnit parserOutput, string fileName, bool updateCommentTags);
		
		IClass GetClass(string typeName);
		IClass GetClass(string typeName, int typeParameterCount);
		bool NamespaceExists(string name);
		ArrayList GetNamespaceContents(string nameSpace);
		
		IClass GetClass(string typeName, int typeParameterCount, LanguageProperties language, bool lookInReferences);
		bool NamespaceExists(string name, LanguageProperties language, bool lookInReferences);
		/// <summary>
		/// Adds the contents of the specified <paramref name="subNameSpace"/> to the <paramref name="list"/>.
		/// </summary>
		void AddNamespaceContents(ArrayList list, string subNameSpace, LanguageProperties language, bool lookInReferences);
		
		string SearchNamespace(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn);
		IReturnType SearchType(string name, int typeParameterCount, IClass curType, int caretLine, int caretColumn);
		IReturnType SearchType(string name, int typeParameterCount, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn);
		
		Position GetPosition(string fullMemberName);
	}
}
