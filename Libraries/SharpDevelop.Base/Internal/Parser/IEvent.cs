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
	public interface IEvent : IMember
	{
		IRegion Region {
			get;
		}
		
		IRegion BodyRegion {
			get;
		}
		
		IMethod AddMethod {
			get;
		}
		
		IMethod RemoveMethod {
			get;
		}
		
		IMethod RaiseMethod {
			get;
		}
	}
}
