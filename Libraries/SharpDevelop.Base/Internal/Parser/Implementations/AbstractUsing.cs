// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using ICSharpCode.SharpDevelop.Services;

namespace SharpDevelop.Internal.Parser
{
	[Serializable]
	public abstract class AbstractUsing : MarshalByRefObject, IUsing
	{
		protected IRegion region;
		
		protected StringCollection usings  = new StringCollection();
		protected SortedList       aliases = new SortedList();
		
		public IRegion Region {
			get {
				return region;
			}
		}
		
		public StringCollection Usings {
			get {
				return usings;
			}
		}
		
		public SortedList Aliases {
			get {
				return aliases;
			}
		}
		
		static IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
		public string SearchNamespace(string partitialNamespaceName)
		{
			return SearchNamespace(partitialNamespaceName, true);
		}
		
		public string SearchNamespace(string partitialNamespaceName, bool caseSensitive)
		{
//			Console.WriteLine("SearchNamespace : >{0}<", partitialNamespaceName);
			if (parserService.NamespaceExists(partitialNamespaceName, caseSensitive)) {
				return partitialNamespaceName;
			}
			
			// search for partitial namespaces
			string declaringNamespace = (string)aliases[""];
			if (declaringNamespace != null) {
				while (declaringNamespace.Length > 0) {
					if ((caseSensitive ? declaringNamespace.EndsWith(partitialNamespaceName) : declaringNamespace.ToLower().EndsWith(partitialNamespaceName.ToLower()) ) && parserService.NamespaceExists(declaringNamespace, caseSensitive)) {
						return declaringNamespace;
					}
					int index = declaringNamespace.IndexOf('.');
					if (index > 0) {
						declaringNamespace = declaringNamespace.Substring(0, index);
					} else {
						break;
					}
				}
			}
			
			// Remember:
			//     Each namespace has an own using object
			//     The namespace name is an alias which has the key ""
			foreach (DictionaryEntry entry in aliases) {
				string aliasString = entry.Key.ToString();
				if (caseSensitive ? partitialNamespaceName.StartsWith(aliasString) : partitialNamespaceName.ToLower().StartsWith(aliasString.ToLower())) {
					if (aliasString.Length >= 0) {
						string nsName = nsName = String.Concat(entry.Value.ToString(), partitialNamespaceName.Remove(0, aliasString.Length));
						if (parserService.NamespaceExists(nsName, caseSensitive)) {
							return nsName;
						}
					}
				}
			}
			return null;
		}

		public IClass SearchType(string partitialTypeName)
		{
			return SearchType(partitialTypeName, true);
		}
		
		public IClass SearchType(string partitialTypeName, bool caseSensitive)
		{
//			Console.WriteLine("Search type : >{0}<", partitialTypeName);
			IClass c = parserService.GetClass(partitialTypeName, caseSensitive);
			if (c != null) {
				return c;
			}
			
			foreach (string str in usings) {
//				Console.WriteLine(str);
				string possibleType = String.Concat(str, ".", partitialTypeName);
//				Console.WriteLine("looking for " + possibleType);
				c = parserService.GetClass(possibleType, caseSensitive);
				if (c != null) {
//					Console.WriteLine("Found!");
					return c;
				}
			}
			
			// search class in partitial namespaces
			string declaringNamespace = (string)aliases[""];
			if (declaringNamespace != null) {
				while (declaringNamespace.Length > 0) {
					string className = String.Concat(declaringNamespace, ".", partitialTypeName);
//					Console.WriteLine("looking for " + className);
					c = parserService.GetClass(className, caseSensitive);
					if (c != null) {
//						Console.WriteLine("Found!");
						return c;
					}
					int index = declaringNamespace.IndexOf('.');
					if (index > 0) {
						declaringNamespace = declaringNamespace.Substring(0, index);
					} else {
						break;
					}
				}
			}
			
			foreach (DictionaryEntry entry in aliases) {
				string aliasString = entry.Key.ToString();
				if (caseSensitive ? partitialTypeName.StartsWith(aliasString) : partitialTypeName.ToLower().StartsWith(aliasString.ToLower())) {
					string className = null;
					if (aliasString.Length > 0) {
						className = String.Concat(entry.Value.ToString(), partitialTypeName.Remove(0, aliasString.Length));
//						Console.WriteLine("looking for " + className);
						c = parserService.GetClass(className, caseSensitive);
						if (c != null) {
//							Console.WriteLine("Found!");
							return c;
						}
					}
				}
			}
			
			return null;
		}
		
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("[AbstractUsing: using list=");
			foreach (string str in usings) {
				builder.Append(str);
				builder.Append(", ");
			}
			builder.Append("]");
			return builder.ToString();
		}
	}
}
