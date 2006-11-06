﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1661 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IUsing
	{
		DomRegion Region {
			get;
		}
		
		List<string> Usings {
			get;
		}
		
		bool HasAliases { get; }
		
		void AddAlias(string alias, IReturnType type);
		
		/// <summary>
		/// Gets the list of aliases. Can be null when there are no aliases!
		/// </summary>
		SortedList<string, IReturnType> Aliases {
			get;
		}
		
		IReturnType SearchType(string partialTypeName, int typeParameterCount);
		string SearchNamespace(string partialNamespaceName);
	}
}
