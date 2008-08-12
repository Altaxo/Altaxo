﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2929 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IUsing : IFreezable
	{
		DomRegion Region {
			get;
		}
		
		IList<string> Usings {
			get;
		}
		
		bool HasAliases { get; }
		
		void AddAlias(string alias, IReturnType type);
		
		/// <summary>
		/// Gets the list of aliases. Can be null when there are no aliases!
		/// </summary>
		IDictionary<string, IReturnType> Aliases {
			get;
		}
		
		/// <summary>
		/// Returns a collection of possible types that could be meant when using this Import
		/// to search the type.
		/// Types with the incorrect type parameter count might be returned, but for each
		/// same using entry or alias entry at most one (the best matching) type should be returned.
		/// </summary>
		/// <returns>An IEnumerable with zero or more non-null return types.</returns>
		IEnumerable<IReturnType> SearchType(string partialTypeName, int typeParameterCount);
		
		string SearchNamespace(string partialNamespaceName);
	}
}
