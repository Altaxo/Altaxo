﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IAttribute : IComparable
	{
		AttributeTarget AttributeTarget {
			get;
		}
		
		string Name {
			get;
		}
		
		/*
		 * These properties are not stored in DomPersistence and cannot not be used!
		List<AttributeArgument> PositionalArguments {
			get;
		}
		
		SortedList<string, AttributeArgument> NamedArguments {
			get;
		}
		*/
	}
	
	public enum AttributeTarget
	{
		None,
		Assembly,
		Field,
		Event,
		Method,
		Module,
		Param,
		Property,
		Return,
		Type
	}
	
	public struct AttributeArgument
	{
		public readonly IReturnType Type;
		public readonly object Value;
		
		public AttributeArgument(IReturnType type, object value)
		{
			this.Type = type;
			this.Value = value;
		}
	}
}
