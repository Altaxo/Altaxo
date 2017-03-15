// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Editor.Windows, CompletionResult.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing
{
	public sealed class CompletionResult
	{
		public CompletionResult(IList<ICompletionDataEx> completionData, IOverloadProviderEx overloadProvider, bool useHardSelection)
		{
			CompletionData = completionData;
			OverloadProvider = overloadProvider;
			UseHardSelection = useHardSelection;
		}

		public bool UseHardSelection { get; }

		public IList<ICompletionDataEx> CompletionData { get; }

		public IOverloadProviderEx OverloadProvider { get; }
	}
}