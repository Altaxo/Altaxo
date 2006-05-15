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

	public interface IParameter: IComparable
	{
		string Name {
			get;
		}

		IReturnType ReturnType {
			get;
			set;
		}

		IList<IAttribute> Attributes {
			get;
		}

		ParameterModifiers Modifiers {
			get;
		}
		
		DomRegion Region {
			get;
		}
		
		string Documentation {
			get;
		}

		bool IsOut {
			get;
		}

		bool IsRef {
			get;
		}

		bool IsParams {
			get;
		}
		
		bool IsOptional {
			get;
		}
	}
}
