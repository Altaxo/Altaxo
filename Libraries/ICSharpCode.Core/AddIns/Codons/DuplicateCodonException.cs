// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.Core.AddIns
{
	/// <summary>
	/// Is thrown when the AddInTree could not find the requested path.
	/// </summary>
	public class DuplicateCodonException : Exception
	{
		/// <summary>
		/// Constructs a new <see cref="DuplicateCodonException"/> instance.
		/// </summary>
		public DuplicateCodonException(string codon) : base("there already exists a codon with name : " + codon)
		{
		}
	}
}
