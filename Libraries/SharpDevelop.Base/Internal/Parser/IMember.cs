// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Reflection;

namespace SharpDevelop.Internal.Parser
{
	public interface IMember : IDecoration
	{
		string FullyQualifiedName {
			get;
		}

		string Name {
			get;
		}

		string Namespace {
			get;
		}

		IReturnType ReturnType {
			get;
		}

		IClass DeclaringType {
			get;
		}
	}
}
