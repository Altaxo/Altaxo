﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1388 $</version>
// </file>

using System;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IMember : IDecoration, ICloneable
	{
		string FullyQualifiedName {
			get;
		}
		
		/// <summary>
		/// Declaration region of the member (without body!)
		/// </summary>
		DomRegion Region {
			get;
		}
		
		string Name {
			get;
		}
		
		string Namespace {
			get;
		}
		
		string DotNetName {
			get;
		}
		
		IReturnType ReturnType {
			get;
			set;
		}
	}
}
