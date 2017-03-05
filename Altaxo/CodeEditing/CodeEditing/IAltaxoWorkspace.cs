using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing
{
	public interface IAltaxoWorkspace
	{
		/// <summary>
		/// Gets the roslyn host that hosts this workspace.
		/// </summary>
		/// <value>
		/// The roslyn host.
		/// </value>
		RoslynHost RoslynHost { get; }

		/// <summary>
		/// Gets the preprocessor symbols that are used for code parsing and compilation.
		/// </summary>
		/// <value>
		/// The preprocessor symbols.
		/// </value>
		ImmutableArray<string> PreprocessorSymbols { get; }
	}
}