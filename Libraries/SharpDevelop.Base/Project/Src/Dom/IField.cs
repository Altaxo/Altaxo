﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IField : IMember
	{
		/// <summary>Gets if this field is a local variable that has been converted into a field.</summary>
		bool IsLocalVariable { get; }
		
		/// <summary>Gets if this field is a parameter that has been converted into a field.</summary>
		bool IsParameter { get; }
	}
}
