// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;

namespace ICSharpCode.Core.AddIns.Codons
{
	/// <summary>
	/// A basic command interface. A command has simply an owner which "runs" the command
	/// and a Run method which invokes the command.
	/// </summary>
	public interface ICommand
	{
		
		/// <summary>
		/// Returns the owner of the command.
		/// </summary>
		object Owner {
			get;
			set;
		}
		
		/// <summary>
		/// Invokes the command.
		/// </summary>
		void Run();
	}
}
