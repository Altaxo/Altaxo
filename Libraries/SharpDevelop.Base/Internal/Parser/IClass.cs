// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace SharpDevelop.Internal.Parser
{
	public interface IClass : IDecoration
	{
		ICompilationUnit CompilationUnit {
			get;
		}

		string FullyQualifiedName {
			get;
		}

		string Name {
			get;
		}

		string Namespace {
			get;
		}
		
		ClassType ClassType {
			get;
		}

		IRegion Region {
			get;
		}
		
		IRegion BodyRegion {
			get;
		}
		
		StringCollection BaseTypes {
			get;
		}
		
		ClassCollection InnerClasses {
			get;
		}

		FieldCollection Fields {
			get;
		}

		PropertyCollection Properties {
			get;
		}

		IndexerCollection Indexer {
			get;
		}

		MethodCollection Methods {
			get;
		}

		EventCollection Events {
			get;
		}

		IEnumerable ClassInheritanceTree {
			get;
		}
		
		object DeclaredIn {
			get;
		}
	}
}
