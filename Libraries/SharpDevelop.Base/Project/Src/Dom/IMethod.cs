﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.Reflection;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IMethodOrProperty : IMember
	{
		DomRegion BodyRegion {
			get;
		}
		
		IList<IParameter> Parameters {
			get;
		}
		
		bool IsExtensionMethod {
			get;
		}
	}
	
	public interface IMethod : IMethodOrProperty
	{
		IList<ITypeParameter> TypeParameters {
			get;
		}
		
		bool IsConstructor {
			get;
		}
	}
}
