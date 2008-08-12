// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2949 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// The SearchClassReturnType is used when only a part of the class name is known and the
	/// type can only be resolved on demand (the ConvertVisitor uses SearchClassReturnType's).
	/// </summary>
	public sealed class SearchClassReturnType : ProxyReturnType
	{
		IClass declaringClass;
		IProjectContent pc;
		int caretLine;
		int caretColumn;
		string name;
		string shortName;
		int typeParameterCount;
		
		public SearchClassReturnType(IProjectContent projectContent, IClass declaringClass, int caretLine, int caretColumn, string name, int typeParameterCount)
		{
			if (declaringClass == null)
				throw new ArgumentNullException("declaringClass");
			this.declaringClass = declaringClass;
			this.pc = projectContent;
			this.caretLine = caretLine;
			this.caretColumn = caretColumn;
			this.typeParameterCount = typeParameterCount;
			this.name = name;
			int pos = name.LastIndexOf('.');
			if (pos < 0)
				shortName = name;
			else
				shortName = name.Substring(pos + 1);
		}
		
		// we need to use a static Dictionary as cache to provide a easy was to clear all cached
		// BaseTypes.
		// When the cached BaseTypes could not be cleared as soon as the parse information is updated
		// (in contrast to a check if the parse information was updated when the base type is needed
		// the next time), we can get a memory leak:
		// The cached type of a property in Class1 is Class2. Then Class2 is updated, but the property
		// in Class1 is not needed again -> the reference causes the GC to keep the old version
		// of Class2 in memory.
		// The solution is this static cache which is cleared when some parse information updates.
		// That way, there can never be any reference to an out-of-date class.
		static Dictionary<SearchClassReturnType, IReturnType> cache = new Dictionary<SearchClassReturnType, IReturnType>(new ReferenceComparer());
		
		class ReferenceComparer : IEqualityComparer<SearchClassReturnType>
		{
			public bool Equals(SearchClassReturnType x, SearchClassReturnType y)
			{
				return x == y; // don't use x.Equals(y) - Equals might cause a FullyQualifiedName lookup on its own
			}
			
			public int GetHashCode(SearchClassReturnType obj)
			{
				return obj.GetObjectHashCode();
			}
		}
		
		/// <summary>
		/// Clear the static searchclass cache. You should call this method
		/// whenever the DOM changes.
		/// </summary>
		/// <remarks>
		/// automatically called by DefaultProjectContent.UpdateCompilationUnit
		/// and DefaultProjectContent.OnReferencedContentsChanged.
		/// </remarks>
		internal static void ClearCache()
		{
			lock (cache) {
				cache.Clear();
			}
		}
		
		bool isSearching;
		
		public override IReturnType BaseType {
			get {
				IReturnType type;
				lock (cache) {
					if (isSearching)
						return null;
					
					if (cache.TryGetValue(this, out type))
						return type;
					
					isSearching = true;
				}
				try {
					type = pc.SearchType(new SearchTypeRequest(name, typeParameterCount, declaringClass, caretLine, caretColumn)).Result;
					lock (cache) {
						isSearching = false;
						cache[this] = type;
					}
					return type;
				} catch {
					isSearching = false;
					throw;
				}
			}
		}
		
		public override string FullyQualifiedName {
			get {
				string tmp = base.FullyQualifiedName;
				if (tmp == "?") {
					return name;
				}
				return tmp;
			}
		}
		
		public override string Name {
			get {
				return shortName;
			}
		}
		
		public override string DotNetName {
			get {
				string tmp = base.DotNetName;
				if (tmp == "?") {
					return name;
				}
				return tmp;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[SearchClassReturnType: {0}]", name);
		}
	}
}
