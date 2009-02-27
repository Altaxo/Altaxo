// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3769 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IProjectContent : IDisposable
	{
		XmlDoc XmlDoc {
			get;
		}
		
		/// <summary>
		/// Gets if the project content is representing the current version of the assembly.
		/// This property always returns true for ParseProjectContents but might return false
		/// for ReflectionProjectContent/CecilProjectContent if the file was changed.
		/// </summary>
		bool IsUpToDate {
			get;
		}
		
		ICollection<IClass> Classes {
			get;
		}
		
		/// <summary>
		/// Gets the list of namespaces defined in this project content. Does not include namespaces from
		/// referenced project contents.
		/// </summary>
		ICollection<string> NamespaceNames {
			get;
		}
		
		/// <summary>
		/// Gets the list of referenced project contents.
		/// </summary>
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
		/// The type used for project objects depends on the host application.
		/// </summary>
		object Project {
			get;
		}
		
		/// <summary>
		/// Gets a class that allows to conveniently access commonly used types in the system
		/// namespace.
		/// </summary>
		SystemTypes SystemTypes {
			get;
		}
		
		IList<IAttribute> GetAssemblyAttributes();
		
		string GetXmlDocumentation(string memberTag);
		
		void AddClassToNamespaceList(IClass addClass);
		void RemoveCompilationUnit(ICompilationUnit oldUnit);
		void UpdateCompilationUnit(ICompilationUnit oldUnit, ICompilationUnit parserOutput, string fileName);
		
		IClass GetClass(string typeName, int typeParameterCount);
		bool NamespaceExists(string name);
		ArrayList GetNamespaceContents(string nameSpace);
		
		IClass GetClass(string typeName, int typeParameterCount, LanguageProperties language, GetClassOptions options);
		bool NamespaceExists(string name, LanguageProperties language, bool lookInReferences);
		/// <summary>
		/// Adds the contents of the specified <paramref name="subNameSpace"/> to the <paramref name="list"/>.
		/// </summary>
		void AddNamespaceContents(ArrayList list, string subNameSpace, LanguageProperties language, bool lookInReferences);
		
		[Obsolete]
		string SearchNamespace(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn);
		
		SearchTypeResult SearchType(SearchTypeRequest request);
		
		/// <summary>
		/// Gets the position of a member in this project content (not a referenced one).
		/// </summary>
		/// <param name="fullMemberName">The full class name in Reflection syntax (always case sensitive, ` for generics)</param>
		/// <param name="lookInReferences">Whether to search in referenced project contents.</param>
		IClass GetClassByReflectionName(string fullMemberName, bool lookInReferences);
		
		/// <summary>
		/// Gets the definition position of the class/member.
		/// </summary>
		/// <param name="entity">The entity to get the position from.</param>
		FilePosition GetPosition(IEntity entity);
		
		/// <summary>
		/// Gets whether internals in the project content are visible to the other project content.
		/// </summary>
		bool InternalsVisibleTo(IProjectContent otherProjectContent);
	}
	
	[Flags]
	public enum GetClassOptions
	{
		None = 0,
		/// <summary>
		/// Also look in referenced project contents.
		/// </summary>
		LookInReferences = 1,
		/// <summary>
		/// Try if the class is an inner class.
		/// </summary>
		LookForInnerClass = 2,
		/// <summary>
		/// Default = LookInReferences + LookForInnerClass
		/// </summary>
		Default = LookInReferences | LookForInnerClass
	}
	
	public sealed class SearchTypeRequest
	{
		IUsingScope currentUsingScope;
		ICompilationUnit currentCompilationUnit;
		
		public string Name { get; set; }
		public int TypeParameterCount { get; set; }
		public IClass CurrentType { get; set; }
		public int CaretLine { get; set; }
		public int CaretColumn { get; set; }
		
		public ICompilationUnit CurrentCompilationUnit {
			get { return currentCompilationUnit; }
			set {
				if (value == null)
					throw new ArgumentNullException("CurrentCompilationUnit");
				currentCompilationUnit = value;
			}
		}
		
		public IUsingScope CurrentUsingScope {
			get { return currentUsingScope; }
			set {
				if (value == null)
					throw new ArgumentNullException("CurrentUsingScope");
				currentUsingScope = value;
			}
		}
		
		public SearchTypeRequest(string name, int typeParameterCount, IClass currentType, int caretLine, int caretColumn)
		{
			if (currentType == null)
				throw new ArgumentNullException("currentType");
			this.Name = name;
			this.TypeParameterCount = typeParameterCount;
			this.CurrentCompilationUnit = currentType.CompilationUnit;
			this.CurrentType = currentType != null ? currentType.GetCompoundClass() : null;
			this.CaretLine = caretLine;
			this.CaretColumn = caretColumn;
			this.CurrentUsingScope = currentType.UsingScope;
		}
		
		public SearchTypeRequest(string name, int typeParameterCount, IClass currentType, ICompilationUnit currentCompilationUnit, int caretLine, int caretColumn)
		{
			if (currentCompilationUnit == null)
				throw new ArgumentNullException("currentCompilationUnit");
			this.Name = name;
			this.TypeParameterCount = typeParameterCount;
			this.CurrentCompilationUnit = currentCompilationUnit;
			this.CurrentType = currentType != null ? currentType.GetCompoundClass() : null;
			this.CaretLine = caretLine;
			this.CaretColumn = caretColumn;
			this.CurrentUsingScope = (currentType != null) ? currentType.UsingScope : currentCompilationUnit.UsingScope;
		}
	}
	
	public struct SearchTypeResult
	{
		public static readonly SearchTypeResult Empty = default(SearchTypeResult);
		
		readonly IReturnType result;
		readonly IUsing usedUsing;
		readonly string namespaceResult;
		
		public SearchTypeResult(IReturnType result) : this(result, null) {}
		
		public SearchTypeResult(IClass c) : this(c != null ? c.DefaultReturnType : null) {}
		
		public SearchTypeResult(IReturnType result, IUsing usedUsing)
		{
			this.result = result;
			this.usedUsing = usedUsing;
			this.namespaceResult = null;
		}
		
		public SearchTypeResult(string namespaceResult, IUsing usedUsing)
		{
			this.result = null;
			this.usedUsing = usedUsing;
			this.namespaceResult = namespaceResult;
		}
		
		/// <summary>
		/// Gets the result type.
		/// </summary>
		public IReturnType Result {
			get { return result; }
		}
		
		/// <summary>
		/// Gets the using that was used for this type lookup.
		/// </summary>
		public IUsing UsedUsing {
			get { return usedUsing; }
		}
		
		public string NamespaceResult {
			get { return namespaceResult; }
		}
	}
}

